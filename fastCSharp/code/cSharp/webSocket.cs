using System;
using System.Reflection;
using fastCSharp.reflection;
using fastCSharp.net.tcp.http;
using System.Net.Sockets;
using fastCSharp.threading;
using System.Threading;
using System.Net.Security;
using System.Runtime.CompilerServices;

namespace fastCSharp.code.cSharp
{
    /// <summary>
    /// WebSocket调用配置
    /// </summary>
    public sealed partial class webSocket : ignoreMember
    {
        /// <summary>
        /// WebSocket调用接口
        /// </summary>
        public interface IWebSocket
        {
        }
        /// <summary>
        /// WebSocket调用
        /// </summary>
        public abstract class socket : webPage.pageBase
        {
            /// <summary>
            /// 数据解析
            /// </summary>
            protected unsafe struct buffer
            {
                /// <summary>
                /// 当前读取位置
                /// </summary>
                private byte* read;
                /// <summary>
                /// 数据结束位置
                /// </summary>
                private byte* end;
                /// <summary>
                /// 数据解析
                /// </summary>
                /// <param name="read">当前读取位置</param>
                /// <param name="end">数据结束位置</param>
                public buffer(byte* read, byte* end)
                {
                    this.read = read;
                    this.end = end;
                }
                /// <summary>
                /// 剩余数据长度
                /// </summary>
                public int Length
                {
                    get { return (int)(end - read); }
                }
                /// <summary>
                /// 读取字节
                /// </summary>
                /// <returns></returns>
                public byte Byte()
                {
                    return *read++;
                }
                /// <summary>
                /// 读取数据
                /// </summary>
                /// <returns></returns>
                public ushort UShort()
                {
                    ushort value = *(ushort*)read;
                    read += 2;
                    return value;
                }
                /// <summary>
                /// 读取数据
                /// </summary>
                /// <returns></returns>
                public short Short()
                {
                    ushort value = *(ushort*)read;
                    read += 2;
                    return (value & 0x8000) == 0 ? (short)value : (short)-(value & short.MaxValue);
                }
                /// <summary>
                /// 读取数据
                /// </summary>
                /// <returns></returns>
                public uint UInt()
                {
                    uint value = *(uint*)read;
                    read += 4;
                    return value;
                }
                /// <summary>
                /// 读取数据
                /// </summary>
                /// <returns></returns>
                public int Int()
                {
                    uint value = *(uint*)read;
                    read += 4;
                    return (value & 0x80000000U) == 0 ? (int)value : (int)-(value & int.MaxValue);
                }
                /// <summary>
                /// 读取短字符串(Length小于65536)
                /// </summary>
                /// <returns>null表示失败</returns>
                public string UShortString()
                {
                    int size = *(ushort*)read;
                    read += 2;
                    return deCodeString(size);
                }
                /// <summary>
                /// 读取字符串
                /// </summary>
                /// <returns></returns>
                public string String()
                {
                    int size = *(int*)read;
                    if ((size & (1 << 15)) == 0)
                    {
                        read += 2;
                        return deCodeString(size);
                    }
                    size = (size & int.MaxValue) + (((int)*(ushort*)(read + 2)) << 15);
                    if (size > short.MaxValue)
                    {
                        read += 4;
                        return deCodeString(size);
                    }
                    return null;
                }
                /// <summary>
                /// 字符串解码
                /// </summary>
                /// <param name="length"></param>
                /// <returns></returns>
                private string deCodeString(int length)
                {
                    if (length == 0) return string.Empty;
                    if ((int)(end - read) >= length + 2)
                    {
                        string value = fastCSharp.String.FastAllocateString(length);
                        fixed (char* valueFixed = value)
                        {
                            char* write = valueFixed;
                            do
                            {
                                int size = *(ushort*)read;
                                if ((length -= size) < 0 || (read += size + 2) > end) return null;
                                for (byte* start = read - size; start != read; *write++ = (char)*start++) ;
                                if (length == 0) return value;
                                size = *(ushort*)read;
                                if ((length -= size) < 0) return null;
                                if ((read += (size <<= 1) + 2) > end) return null;
                                fastCSharp.unsafer.memory.Copy(read - size, write, size);
                                write += size;
                                if (length == 0) return value;
                            }
                            while (true);
                        }
                    }
                    return null;
                }
            }
            /// <summary>
            /// 发送消息
            /// </summary>
            internal struct message
            {
                /// <summary>
                /// 字符串消息
                /// </summary>
                public string Message;
                /// <summary>
                /// 数据消息
                /// </summary>
                public byte[] Data;
                /// <summary>
                /// 清除消息
                /// </summary>
                /// <param name="message"></param>
                public void Clear(ref message message)
                {
                    message = this;
                    Message = null;
                    Data = null;
                }
            }
            /// <summary>
            /// 客户端
            /// </summary>
            internal Socket Client;
            /// <summary>
            /// 安全网络流
            /// </summary>
            internal SslStream SslStream;
            /// <summary>
            /// 发送消息集合
            /// </summary>
            private fastCSharp.collection<message> messages;
            /// <summary>
            /// 发送消息数量
            /// </summary>
            internal int MessageCount
            {
                get { return messages.Count; }
            }
            /// <summary>
            /// 发送消息集合访问锁
            /// </summary>
            private readonly object messageLock = new object();
            /// <summary>
            /// 是否已经关闭
            /// </summary>
            private int isClose;
            /// <summary>
            /// 是否已经关闭
            /// </summary>
            protected bool IsClose
            {
                get { return isClose != 0; }
            }
            /// <summary>
            /// 是否正在发送消息
            /// </summary>
            private byte isSendMessage;
            /// <summary>
            /// 关闭连接
            /// </summary>
            internal void Close()
            {
                if (Interlocked.CompareExchange(ref isClose, 1, 0) == 0) onClose();
            }
            /// <summary>
            /// 关闭连接处理
            /// </summary>
            protected virtual void onClose() { }
            /// <summary>
            /// WebSocket调用加载
            /// </summary>
            /// <param name="domainServer">域名服务</param>
            /// <param name="socket">HTTP套接字接口设置</param>
            /// <param name="socketIdentity">套接字操作编号</param>
            internal void Load(domainServer domainServer, socketBase socket, long socketIdentity)
            {
                Socket = socket;
                try
                {
                    if (!socket.ResponseWebSocket101(ref socketIdentity, this)) return;
                    DomainServer = domainServer;
                    SocketIdentity = socketIdentity;
                    requestHeader = socket.RequestHeader;
                    if (loadSocket()) return;
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                socket.ResponseError(socketIdentity, net.tcp.http.response.state.ServerError500);
            }
            /// <summary>
            /// WebSocket调用加载
            /// </summary>
            /// <returns>是否成功</returns>
            protected virtual bool loadSocket()
            {
                return true;
            }
            /// <summary>
            /// 发送文本消息
            /// </summary>
            /// <param name="message"></param>
            protected void send(string message)
            {
                if (!string.IsNullOrEmpty(message)) send(new message { Message = message });
            }
            /// <summary>
            /// 发送二进制数据
            /// </summary>
            /// <param name="data"></param>
            protected void send(byte[] data)
            {
                if (data != null && data.Length != 0) send(new message { Data = data });
            }
            /// <summary>
            /// 发送消息
            /// </summary>
            /// <param name="message"></param>
            private void send(message message)
            {
                if (isClose == 0)
                {
                    Monitor.Enter(messageLock);
                    byte isSendMessage = this.isSendMessage;
                    this.isSendMessage = 1;
                    try
                    {
                        if (messages == null) messages = new collection<message>();
                        messages.Add(message);
                    }
                    finally
                    {
                        Monitor.Exit(messageLock);
                        if (isSendMessage == 0) socket.WebSocketSend(this);
                    }
                }
            }
            /// <summary>
            /// 发送消息
            /// </summary>
            /// <param name="messages"></param>
            protected void send(params string[] messages)
            {
                if (isClose == 0 && messages.Length != 0)
                {
                    Monitor.Enter(messageLock);
                    byte isSendMessage = this.isSendMessage;
                    this.isSendMessage = 1;
                    try
                    {
                        if (this.messages == null) this.messages = new collection<message>();
                        foreach (string message in messages)
                        {
                            if (!string.IsNullOrEmpty(message)) this.messages.Add(new message { Message = message });
                        }
                    }
                    finally
                    {
                        Monitor.Exit(messageLock);
                        if (isSendMessage == 0) socket.WebSocketSend(this);
                    }
                }
            }
            /// <summary>
            /// 获取发送消息
            /// </summary>
            /// <param name="message"></param>
            /// <returns></returns>
            internal bool GetMessage(ref message message)
            {
                if (isClose == 0)
                {
                    Monitor.Enter(messageLock);
                    if (messages.Count == 0)
                    {
                        this.isSendMessage = 0;
                        Monitor.Exit(messageLock);
                        return false;
                    }
                    messages.UnsafeArray[messages.StartIndex].Clear(ref message);
                    messages.UnsafeNextExpand();
                    Monitor.Exit(messageLock);
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 数据解析
            /// </summary>
            internal struct receiveParser
            {
                /// <summary>
                /// 操作编码
                /// </summary>
                private enum typeCode : byte
                {
                    /// <summary>
                    /// 连续消息片断
                    /// </summary>
                    Continuous = 0,
                    /// <summary>
                    /// 文本消息片断
                    /// </summary>
                    Text = 1,
                    /// <summary>
                    /// 二进制消息片断
                    /// </summary>
                    Binary = 2,
                    /// <summary>
                    /// 连接关闭
                    /// </summary>
                    Close = 8,
                    /// <summary>
                    /// 心跳检查的ping
                    /// </summary>
                    Ping = 9,
                    /// <summary>
                    /// 心跳检查的pong
                    /// </summary>
                    Pong = 10,
                }
                /// <summary>
                /// 数据解析步骤
                /// </summary>
                private enum receiveType
                {
                    /// <summary>
                    /// 数据帧开始
                    /// </summary>
                    None,
                    /// <summary>
                    /// 已知长度数据帧
                    /// </summary>
                    Next126,
                    /// <summary>
                    /// 已知长度数据帧
                    /// </summary>
                    Next127
                }
                /// <summary>
                /// HTTP套接字缓冲区
                /// </summary>
                internal byte[] Buffer;
                /// <summary>
                /// WebSocket调用
                /// </summary>
                private socket socket;
                /// <summary>
                /// 最大接收消息大小
                /// </summary>
                private uint maxMessageBufferSize;
                /// <summary>
                /// 最大接收消息大小
                /// </summary>
                private uint maxMessageSize;
                /// <summary>
                /// 最大接收数据帧大小
                /// </summary>
                internal int MaxFrameBufferSize;
                /// <summary>
                /// 当前没有接收完的消息缓冲区
                /// </summary>
                private byte[] messageBuffer;
                /// <summary>
                /// 当前没有接收完的消息缓冲区位置
                /// </summary>
                private uint messageIndex;
                /// <summary>
                /// 当前没有接收完的数据帧缓冲区
                /// </summary>
                private byte[] frameBuffer;
                /// <summary>
                /// 当前没有接收完的数据帧缓冲区位置
                /// </summary>
                private int frameIndex;
                /// <summary>
                /// 当前没有接收完的数据帧大小
                /// </summary>
                private int nextFrameSize;
                /// <summary>
                /// 当前接收数据帧大小
                /// </summary>
                private uint frameDataSize;
                /// <summary>
                /// 数据解析步骤
                /// </summary>
                private receiveType type;
                /// <summary>
                /// 当前接收消息类型
                /// </summary>
                private typeCode code;
                /// <summary>
                /// 最大接收消息大小
                /// </summary>
                /// <param name="socket"></param>
                public void Set(socket socket)
                {
                    this.socket = socket;
                    maxMessageSize = (uint)socket.maxMessageSize;
                    if (maxMessageSize >= 65536)
                    {
                        MaxFrameBufferSize = (int)(maxMessageSize + (2 + 8 + 4 + 7));
                        if (MaxFrameBufferSize < 0)
                        {
                            MaxFrameBufferSize = int.MaxValue - 7;
                            maxMessageSize = int.MaxValue - (2 + 8 + 4);
                            maxMessageBufferSize = (int.MaxValue - (2 + 8 + 4 + 7)) & (int.MaxValue - 7);
                        }
                        else
                        {
                            MaxFrameBufferSize &= int.MaxValue - 7;
                            maxMessageBufferSize = (maxMessageSize + 7) & (int.MaxValue - 7);
                        }
                    }
                    else if (maxMessageSize >= 126)
                    {
                        MaxFrameBufferSize = (int)(maxMessageSize + (2 + 2 + 4 + 7)) & (int.MaxValue - 7);
                        maxMessageBufferSize = (maxMessageSize + 7) & (int.MaxValue - 7);
                    }
                    else if (maxMessageSize > 0)
                    {
                        MaxFrameBufferSize = (int)(maxMessageSize + (2 + 4 + 7)) & (int.MaxValue - 7);
                        maxMessageBufferSize = (maxMessageSize + 7) & (int.MaxValue - 7);
                    }
                    else
                    {
                        MaxFrameBufferSize = (2 + 4 + 7) & (int.MaxValue - 7);
                        maxMessageBufferSize = maxMessageSize = 0;
                    }
                }
                /// <summary>
                /// 数据解析
                /// </summary>
                /// <param name="length"></param>
                /// <returns></returns>
                public unsafe int Parse(int length)
                {
                    int startIndex = 0;
                    fixed (byte* bufferFixed = Buffer)
                    {
                        switch (type)
                        {
                            case receiveType.None:
                                NONE:
                                do
                                {
                                    byte* start = bufferFixed + startIndex;
                                    if (length == 1)
                                    {
                                        if (startIndex == 0) return -1;
                                        *bufferFixed = *start;
                                        return 1;
                                    }
                                    uint value = *(uint*)start;
                                    if ((value & 0x8000) == 0) return -1;
                                    switch (value & 0xf)
                                    {
                                        case (byte)typeCode.Continuous:
                                            if (code == typeCode.Continuous) return -1;
                                            break;
                                        case (byte)typeCode.Close: return -1;
                                    }
                                    frameDataSize = (value >> 8) & 0x7f;
                                    byte* read;
                                    if (frameDataSize < 126)
                                    {
                                        if (messageIndex + frameDataSize > maxMessageSize) return -1;
                                        if (length < (nextFrameSize = (int)frameDataSize + (2 + 4)))
                                        {
                                            if (startIndex == 0) return -1;
                                            fastCSharp.unsafer.memory.UnsafeSimpleCopy(start, bufferFixed, length);
                                            return length;
                                        }
                                        read = start + (2 + 4);
                                    }
                                    else if (frameDataSize == 126)
                                    {
                                        if (messageIndex + 126 > maxMessageSize) return -1;
                                        if (length < 4)
                                        {
                                            *(uint*)bufferFixed = value;
                                            return length;
                                        }
                                        if ((frameDataSize = ((uint)*(start + 2) << 8) + *(start + 3)) < 126 || messageIndex + frameDataSize > maxMessageSize) return -1;
                                        if (length < (nextFrameSize = (int)frameDataSize + (2 + 2 + 4)))
                                        {
                                            if (nextFrameSize <= Buffer.Length)
                                            {
                                                if (startIndex != 0) System.Buffer.BlockCopy(Buffer, startIndex, Buffer, 0, length);
                                                return length;
                                            }
                                            if (frameBuffer == null) frameBuffer = new byte[Math.Min(MaxFrameBufferSize, (1 << 16) + (2 + 2 + 4))];
                                            System.Buffer.BlockCopy(Buffer, startIndex, frameBuffer, 0, frameIndex = length);
                                            nextFrameSize -= length;
                                            type = receiveType.Next126;
                                            return 0;
                                        }
                                        read = start + (2 + 2 + 4);
                                    }
                                    else
                                    {
                                        if (messageIndex + 65536 > maxMessageSize) return -1;
                                        if (length < 2 + 8)
                                        {
                                            *(ulong*)bufferFixed = *(ulong*)start;
                                            *(bufferFixed + 8) = *(start + 8);
                                            return length;
                                        }
                                        if (*(uint*)(start + 2) != 0) return -1;
                                        frameDataSize = ((uint)*(start + 6) << 24) + ((uint)*(start + 7) << 16) + ((uint)*(start + 8) << 8) + (uint)*(start + 9);
                                        if (frameDataSize - ushort.MaxValue >= int.MaxValue - ushort.MaxValue || messageIndex + frameDataSize > maxMessageSize) return -1;
                                        if (length < (nextFrameSize = (int)frameDataSize + (2 + 8 + 4)))
                                        {
                                            if (frameBuffer == null || frameBuffer.Length < nextFrameSize)
                                            {
                                                int bufferSize = (nextFrameSize + 7) & (int.MaxValue - 7);
                                                frameBuffer = new byte[bufferSize >= MaxFrameBufferSize >> 1 ? MaxFrameBufferSize : (bufferSize << 1)];
                                            }
                                            System.Buffer.BlockCopy(Buffer, startIndex, frameBuffer, 0, frameIndex = length);
                                            nextFrameSize -= length;
                                            type = receiveType.Next127;
                                            return 0;
                                        }
                                        read = start + (2 + 8 + 4);
                                    }
                                    if (messageIndex == 0)
                                    {
                                        typeCode currentCode = (typeCode)(byte)(value & 0xf);
                                        if (currentCode != typeCode.Continuous) code = currentCode;
                                        if ((value & 0x80) == 0)
                                        {
                                            switch (code)
                                            {
                                                case typeCode.Text:
                                                case typeCode.Binary:
                                                    int bufferSize = (int)Math.Min(((frameDataSize << 1) + 7) & (uint.MaxValue - 7), maxMessageBufferSize);
                                                    if (messageBuffer == null || messageBuffer.Length < bufferSize) messageBuffer = new byte[Math.Max(bufferSize, 1 << 10)];
                                                    appendMessage(read, frameDataSize);
                                                    break;
                                                default: return -1;
                                            }
                                        }
                                        else
                                        {
                                            switch (code)
                                            {
                                                case typeCode.Text:
                                                case typeCode.Binary:
                                                    markMessage(read, frameDataSize);
                                                    try
                                                    {
                                                        if (code == typeCode.Text) socket.onMessage(frameDataSize == 0 ? string.Empty : System.Text.Encoding.UTF8.GetString(Buffer, (int)(read - bufferFixed), (int)frameDataSize));
                                                        else socket.onMessage(subArray<byte>.Unsafe(Buffer, (int)(read - bufferFixed), (int)frameDataSize));
                                                    }
                                                    catch (Exception error)
                                                    {
                                                        fastCSharp.log.Default.Add(error, null, false);
                                                    }
                                                    code = typeCode.Continuous;
                                                    break;
                                                case typeCode.Ping:
                                                case typeCode.Pong:
                                                    break;
                                                default: return -1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if ((value & 0xf) != 0) return -1;
                                        switch (code)
                                        {
                                            case typeCode.Text:
                                            case typeCode.Binary:
                                                int bufferSize = (int)(messageIndex + frameDataSize);
                                                if (messageBuffer.Length < bufferSize)
                                                {
                                                    byte[] newMessageBuffer = new byte[Math.Max((bufferSize + 7) & (int.MaxValue - 7), 1 << 10)];
                                                    System.Buffer.BlockCopy(messageBuffer, 0, newMessageBuffer, 0, (int)messageIndex);
                                                    messageBuffer = newMessageBuffer;
                                                }
                                                appendMessage(read, frameDataSize);
                                                if ((value & 0x80) != 0)
                                                {
                                                    try
                                                    {
                                                        if (code == typeCode.Text) socket.onMessage(System.Text.Encoding.UTF8.GetString(messageBuffer, 0, (int)messageIndex));
                                                        else socket.onMessage(subArray<byte>.Unsafe(messageBuffer, 0, (int)messageIndex));
                                                    }
                                                    catch (Exception error)
                                                    {
                                                        fastCSharp.log.Default.Add(error, null, false);
                                                    }
                                                    messageIndex = 0;
                                                    code = typeCode.Continuous;
                                                }
                                                break;
                                            default: return -1;
                                        }
                                    }
                                    if ((length -= nextFrameSize) == 0) return 0;
                                    startIndex += nextFrameSize;
                                }
                                while (true);
                            case receiveType.Next126:
                                if (length < nextFrameSize) break;
                                System.Buffer.BlockCopy(Buffer, startIndex, frameBuffer, frameIndex, nextFrameSize);
                                frameIndex = 2 + 2 + 4;
                                END:
                                fixed (byte* frameBufferFixed = frameBuffer)
                                {
                                    if (messageIndex == 0)
                                    {
                                        typeCode currentCode = (typeCode)(byte)(*frameBufferFixed & 0xf);
                                        if (currentCode != typeCode.Continuous) code = currentCode;
                                        if ((*frameBufferFixed & 0x80) == 0)
                                        {
                                            switch (code)
                                            {
                                                case typeCode.Text:
                                                case typeCode.Binary:
                                                    int bufferSize = (int)Math.Min(((frameDataSize << 1) + 7) & (uint.MaxValue - 7), maxMessageBufferSize);
                                                    if (messageBuffer == null || messageBuffer.Length < bufferSize) messageBuffer = new byte[Math.Max(bufferSize, 1 << 10)];
                                                    appendMessage(frameBufferFixed + frameIndex, frameDataSize);
                                                    break;
                                                default: return -1;
                                            }
                                        }
                                        else
                                        {
                                            switch (code)
                                            {
                                                case typeCode.Text:
                                                case typeCode.Binary:
                                                    markMessage(frameBufferFixed + frameIndex, frameDataSize);
                                                    try
                                                    {
                                                        if (code == typeCode.Text) socket.onMessage(frameDataSize == 0 ? string.Empty : System.Text.Encoding.UTF8.GetString(frameBuffer, frameIndex, (int)frameDataSize));
                                                        else socket.onMessage(subArray<byte>.Unsafe(frameBuffer, frameIndex, (int)frameDataSize));
                                                    }
                                                    catch (Exception error)
                                                    {
                                                        fastCSharp.log.Default.Add(error, null, false);
                                                    }
                                                    code = typeCode.Continuous;
                                                    break;
                                                case typeCode.Ping:
                                                case typeCode.Pong:
                                                    break;
                                                default: return -1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if ((*frameBufferFixed & 0xf) != 0) return -1;
                                        switch (code)
                                        {
                                            case typeCode.Text:
                                            case typeCode.Binary:
                                                int bufferSize = (int)(messageIndex + frameDataSize);
                                                if (messageBuffer.Length < bufferSize)
                                                {
                                                    byte[] newMessageBuffer = new byte[Math.Max((bufferSize + 7) & (int.MaxValue - 7), 1 << 10)];
                                                    System.Buffer.BlockCopy(messageBuffer, 0, newMessageBuffer, 0, (int)messageIndex);
                                                    messageBuffer = newMessageBuffer;
                                                }
                                                appendMessage(frameBufferFixed + frameIndex, frameDataSize);
                                                if ((*frameBufferFixed & 0x80) != 0)
                                                {
                                                    try
                                                    {
                                                        if (code == typeCode.Text) socket.onMessage(System.Text.Encoding.UTF8.GetString(messageBuffer, 0, (int)messageIndex));
                                                        else socket.onMessage(subArray<byte>.Unsafe(messageBuffer, 0, (int)messageIndex));
                                                    }
                                                    catch (Exception error)
                                                    {
                                                        fastCSharp.log.Default.Add(error, null, false);
                                                    }
                                                    messageIndex = 0;
                                                    code = typeCode.Continuous;
                                                }
                                                break;
                                            default: return -1;
                                        }
                                    }
                                }
                                type = receiveType.None;
                                if ((length -= nextFrameSize) == 0) return 0;
                                startIndex += nextFrameSize;
                                goto NONE;
                            case receiveType.Next127:
                                if (length < nextFrameSize) break;
                                System.Buffer.BlockCopy(Buffer, startIndex, frameBuffer, frameIndex, nextFrameSize);
                                frameIndex = 2 + 8 + 4;
                                goto END;
                            default: return -1;
                        }
                    }
                    System.Buffer.BlockCopy(Buffer, startIndex, frameBuffer, frameIndex, length);
                    nextFrameSize -= length;
                    frameIndex += length;
                    return 0;
                }
                /// <summary>
                /// 消息格式化处理
                /// </summary>
                /// <param name="read"></param>
                /// <param name="size"></param>
                private static unsafe void markMessage(byte* read, uint size)
                {
                    ulong mark = *(uint*)(read - sizeof(uint));
                    mark |= mark << 32;
                    for (byte* end = read + (size & (int.MaxValue - 7)); read != end; read += sizeof(ulong)) *(ulong*)read ^= mark;
                    switch (size & 7)
                    {
                        case 1:
                            *read ^= (byte)mark;
                            break;
                        case 2:
                            *(ushort*)read ^= (ushort)mark;
                            break;
                        case 3:
                            *(uint*)read ^= (uint)mark & 0xffffffU;
                            break;
                        case 4:
                            *(uint*)read ^= (uint)mark;
                            break;
                        case 5:
                            *(ulong*)read ^= mark & 0xffffffffffUL;
                            break;
                        case 6:
                            *(ulong*)read ^= mark & 0xffffffffffffUL;
                            break;
                        case 7:
                            *(ulong*)read ^= mark & 0xffffffffffffffUL;
                            break;
                    }
                }
                /// <summary>
                /// 追加消息
                /// </summary>
                /// <param name="read"></param>
                /// <param name="size"></param>
                private unsafe void appendMessage(byte* read, uint size)
                {
                    ulong mark = *(uint*)(read - sizeof(uint));
                    mark |= mark << 32;
                    fixed (byte* messageFixed = messageBuffer)
                    {
                        for (byte* write = messageFixed + messageIndex, end = write + ((size + 7) & (int.MaxValue - 7)); write != end; write += sizeof(ulong), read += sizeof(ulong))
                        {
                            *(ulong*)write = *(ulong*)read ^ mark;
                        }
                    }
                    messageIndex += size;
                }
            }
            /// <summary>
            /// 最大接收消息大小
            /// </summary>
            protected virtual int maxMessageSize { get { return ushort.MaxValue - 8; } }
            /// <summary>
            /// 数据解析
            /// </summary>
            internal receiveParser Parser;
            /// <summary>
            /// 消息处理
            /// </summary>
            /// <param name="message"></param>
            protected virtual void onMessage(string message) { }
            /// <summary>
            /// 消息处理
            /// </summary>
            /// <param name="data"></param>
            protected virtual void onMessage(subArray<byte> data) { }
            /// <summary>
            /// 设置套接字请求编号
            /// </summary>
            /// <param name="identity">套接字请求编号</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]

            internal void SetSocketIdentity(long identity)
            {
                SocketIdentity = identity;
                Parser.Set(this);
            }
        }
        /// <summary>
        /// WebSocket调用
        /// </summary>
        /// <typeparam name="socketType">WebSocket调用类型</typeparam>
        public abstract class socket<socketType> : socket where socketType : socket<socketType>
        {
        }
        /// <summary>
        /// WebSocket调用类型名称
        /// </summary>
        public string TypeCallName;
        /// <summary>
        /// WebSocket调用函数名称
        /// </summary>
        public string MethodName;
        /// <summary>
        /// 默认空WebSocket调用配置
        /// </summary>
        internal static readonly webSocket Null = new webSocket();
        /// <summary>
        /// 获取WebSocket加载函数+WebSocket加载函数配置
        /// </summary>
        /// <param name="type">WebSocket调用类型</param>
        /// <returns>WebSocket加载函数+WebSocket加载函数配置</returns>
        public static keyValue<methodInfo, webSocket> GetLoadMethod(Type type)
        {
            methodInfo loadMethod = null;
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (method.ReturnType == typeof(bool))
                {
                    webSocket loadWebSocket = method.customAttribute<webSocket>();
                    if (loadWebSocket != null)
                    {
                        return new keyValue<methodInfo, webSocket>(new methodInfo(method, code.memberFilters.Instance), loadWebSocket);
                    }
                    if (loadMethod == null && method.Name == "loadSocket" && method.GetParameters().Length != 0)
                    {
                        loadMethod = new methodInfo(method, code.memberFilters.Instance);
                    }
                }
            }
            return new keyValue<methodInfo, webSocket>(loadMethod, loadMethod == null ? null : Null);
        }
    }
}
