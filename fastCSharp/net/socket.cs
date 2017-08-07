using System;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using fastCSharp.threading;
using System.Runtime.CompilerServices;

namespace fastCSharp.net
{
    /// <summary>
    /// 套接字
    /// </summary>
    public class socket : IDisposable
    {
        /// <summary>
        /// 服务器端套接字单次最大发送数据量
        /// </summary>
        internal static readonly int MaxServerSendSize = fastCSharp.config.pub.Default.MaxServerSocketSendSize;
        /// <summary>
        /// 服务器端套接字发送缓冲区
        /// </summary>
        internal static readonly memoryPool ServerSendBuffers = memoryPool.GetOrCreate(MaxServerSendSize);
        /// <summary>
        /// 操作套接字
        /// </summary>
        protected internal Socket Socket;
        /// <summary>
        /// 当前接收数据
        /// </summary>
        protected byte[] currentReceiveData;
        /// <summary>
        /// 当前接收数据起始位置
        /// </summary>
        private int currentReceiveStartIndex;
        /// <summary>
        /// 当前接收数据结束位置
        /// </summary>
        protected int currentReceiveEndIndex { get; private set; }
        /// <summary>
        /// 套接字错误
        /// </summary>
        protected SocketError socketError;
        /// <summary>
        /// 最后一次异常
        /// </summary>
        protected Exception lastException;
        /// <summary>
        /// 最后一次异常
        /// </summary>
        public Exception LastException
        {
            get
            {
                if (lastException != null) return lastException;
                if (socketError != SocketError.Success) return new SocketException((int)socketError);
                return null;
            }
        }
        /// <summary>
        /// 是否释放资源
        /// </summary>
        protected int isDisposed;
        /// <summary>
        /// 是否释放资源
        /// </summary>
        public bool IsDisposed
        {
            get { return isDisposed != 0; }
        }
        /// <summary>
        /// 操作错误是否自动调用析构函数
        /// </summary>
        private bool isErrorDispose;
        /// <summary>
        /// 初始化同步套接字
        /// </summary>
        /// <param name="socket">操作套接字</param>
        /// <param name="isErrorDispose">操作错误是否自动调用析构函数</param>
        public socket(Socket socket, bool isErrorDispose = true)
            : this(isErrorDispose)
        {
            if (socket == null) log.Error.Throw(null, "缺少套接字", true);
            Socket = socket;
        }
        /// <summary>
        /// 初始化同步套接字
        /// </summary>
        /// <param name="isErrorDispose">操作错误是否自动调用析构函数</param>
        protected socket(bool isErrorDispose) 
        {
            this.isErrorDispose = isErrorDispose;
        }
        /// <summary>
        /// 关闭套接字连接
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.Increment(ref isDisposed) == 1)
            {
                //log.Default.Add("关闭套接字连接 " + socketError.ToString(), socketError == SocketError.Success ? new System.Diagnostics.StackFrame() : null, false);
                log.Default.Add("关闭套接字连接 " + socketError.ToString(), null, false);
                try
                {
                    dispose();
                }
                finally
                {
                    close();
                }
            }
        }
        /// <summary>
        /// 关闭套接字连接
        /// </summary>
        protected virtual void dispose() { }
        /// <summary>
        /// 设置错误异常
        /// </summary>
        /// <param name="error">错误异常</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void setException(Exception error)
        {
            lastException = error;
        }
        /// <summary>
        /// 操作错误
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void error()
        {
            if (isErrorDispose) Dispose();
            else close();
        }
        /// <summary>
        /// 关闭套接字
        /// </summary>
        protected virtual void close()
        {
            //if (Socket != null) Socket.Close();
            Socket.shutdown();
            Socket = null;
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="data">数据缓存</param>
        /// <param name="index">起始位置</param>
        /// <param name="endIndex">结束位置</param>
        /// <param name="timeout">超时时间</param>
        /// <returns>是否成功</returns>
        protected internal bool receive(byte[] data, int index, int endIndex, DateTime timeout)
        {
            Socket socket = Socket;
            if (socket == null) return false;
            while (index != endIndex)
            {
                SocketError error;
                int length = Socket.Receive(data, index, endIndex - index, SocketFlags.None, out error);
                if (error != SocketError.Success)
                {
                    socketError = error;
                    this.error();
                    return false;
                }
                if (length == 0 || date.nowTime.Now > timeout)
                {
                    this.error();
                    return false;
                }
                index += length;
            }
            return true;
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="data">数据缓存</param>
        /// <param name="index">起始位置</param>
        /// <param name="endIndex">结束位置</param>
        /// <param name="timeout">超时时间</param>
        /// <returns>数据结束位置</returns>
        protected internal int tryReceive(byte[] data, int index, int endIndex, DateTime timeout)
        {
            Socket socket = Socket;
            if (socket == null) return -1;
            while (index < endIndex)
            {
                SocketError error;
                int length = socket.Receive(data, index, data.Length - index, SocketFlags.None, out error);
                if (error != SocketError.Success)
                {
                    socketError = error;
                    this.error();
                    return -1;
                }
                if (length == 0 || date.nowTime.Now > timeout)
                {
                    this.error();
                    return -1;
                }
                index += length;
            }
            return index;
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="index">起始位置</param>
        /// <param name="endIndex">结束位置</param>
        /// <param name="timeout">超时时间</param>
        /// <returns>数据结束位置</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected internal int tryReceive(int index, int endIndex, DateTime timeout)
        {
            return tryReceive(currentReceiveData, index, endIndex, timeout);
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>是否成功</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected internal bool send(ref subArray<byte> data)
        {
            if (Socket.send(data.array, data.startIndex, data.length, ref socketError)) return true;
            error();
            return false;
        }
        /// <summary>
        /// 服务器端发送数据(限制单次发送数据量)
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>是否成功</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected internal bool serverSend(ref subArray<byte> data)
        {
            if (Socket.serverSend(data.array, data.startIndex, data.length, ref socketError)) return true;
            error();
            return false;
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="length">发送长度</param>
        /// <returns>是否成功</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected internal bool send(byte[] data, int startIndex, int length)
        {
            if (Socket.send(data, startIndex, length, ref socketError)) return true;
            error();
            return false;
        }
        ///// <summary>
        ///// 服务器端发送数据(限制单次发送数据量)
        ///// </summary>
        ///// <param name="data">数据</param>
        ///// <param name="startIndex">起始位置</param>
        ///// <param name="length">发送长度</param>
        ///// <returns>是否成功</returns>
        //protected internal bool serverSend(byte[] data, int startIndex, int length)
        //{
        //    if (Socket.serverSend(data, startIndex, length, ref socketError)) return true;
        //    error();
        //    return false;
        //}
        /// <summary>
        /// 数据接收器
        /// </summary>
        protected sealed class tryReceiver
        {
            /// <summary>
            /// 接收超时
            /// </summary>
            public DateTime Timeout;
            /// <summary>
            /// 异步套接字
            /// </summary>
            public socket Socket;
            /// <summary>
            /// 数据接收完成后的回调委托
            /// </summary>
            private EventHandler<SocketAsyncEventArgs> asyncCallback;
            /// <summary>
            /// 数据接收器
            /// </summary>
            public tryReceiver()
            {
                asyncCallback = onReceive;
            }
            /// <summary>
            /// 数据接收类型
            /// </summary>
            public type Type;
            /// <summary>
            /// 接收数据
            /// </summary>
            public void Receive(socket socket, type type, DateTime timeout)
            {
                SocketAsyncEventArgs async = null;
                Socket = socket;
                Type = type;
                Timeout = timeout;
                try
                {
                    async = socketAsyncEventArgs.Get();
                    async.UserToken = this;
                    async.Completed += asyncCallback;
                    if (receive(async)) return;
                }
                catch (Exception error)
                {
                    socket.lastException = error;
                }
                this.callback(async);
            }
            /// <summary>
            /// 继续接收数据
            /// </summary>
            /// <param name="async">异步套接字操作</param>
            /// <returns>是否接收成功</returns>
            private bool receive(SocketAsyncEventArgs async)
            {
                RECEIVE:
                async.SetBuffer(Socket.currentReceiveData, Socket.currentReceiveStartIndex, Socket.currentReceiveData.Length - Socket.currentReceiveStartIndex);
                if (Socket.Socket.ReceiveAsync(async)) return true;
                if (async.SocketError == SocketError.Success)
                {
                    int count = async.BytesTransferred;
                    if (count > 0)
                    {
                        Socket.currentReceiveStartIndex += count;
                        if (Socket.currentReceiveStartIndex >= Socket.currentReceiveEndIndex)
                        {
                            callback(Socket.currentReceiveStartIndex, async);
                            return true;
                        }
                        goto RECEIVE;
                    }
                }
                else Socket.socketError = async.SocketError;
                return false;
            }
            /// <summary>
            /// 数据接收完成后的回调委托
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="async">异步回调参数</param>
            private void onReceive(object sender, SocketAsyncEventArgs async)
            {
                bool isSuccess = false;
                try
                {
                    if (async.SocketError == SocketError.Success)
                    {
                        int count = async.BytesTransferred;
                        if (count != 0)
                        {
                            Socket.currentReceiveStartIndex += count;
                            if (Socket.currentReceiveStartIndex >= Socket.currentReceiveEndIndex)
                            {
                                isSuccess = true;
                                callback(Socket.currentReceiveStartIndex, async);
                                return;
                            }
                            else if (receive(async)) return;
                        }
                    }
                    else Socket.socketError = async.SocketError;
                }
                catch (Exception error)
                {
                    Socket.lastException = error;
                }
                if (!isSuccess) callback(async);
            }
            /// <summary>
            /// 异步回调
            /// </summary>
            /// <param name="count">接收数据长度</param>
            /// <param name="async">异步回调参数</param>
            private void callback(int count, SocketAsyncEventArgs async)
            {
                if (async == null) push(count);
                else
                {
                    try
                    {
                        async.Completed -= asyncCallback;
                        socketAsyncEventArgs.PushNotNull(async);
                    }
                    finally
                    {
                        push(count);
                    }
                }
            }
            /// <summary>
            /// 异步回调
            /// </summary>
            /// <param name="async">异步回调参数</param>
            private void callback(SocketAsyncEventArgs async)
            {
                try
                {
                    Socket.error();
                }
                finally { callback(-1, async); }
            }
            /// <summary>
            /// 添加回调对象
            /// </summary>
            /// <param name="value">回调值</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private void push(int value)
            {
                socket socket = Socket;
                type type = Type;
                Socket = null;
                try
                {
                    typePool<tryReceiver>.PushNotNull(this);
                }
                finally { OnReceive(socket, type, value); }
            }
            /// <summary>
            /// 数据接收回调类型
            /// </summary>
            public enum type
            {
                /// <summary>
                /// 
                /// </summary>
                None,
                /// <summary>
                /// 服务器端接收命令长度处理
                /// </summary>
                ServerOnReceiveStreamCommandLength,
                /// <summary>
                /// 服务器端接收命令处理
                /// </summary>
                ServerOnReceiveStreamIdentityCommand,
                /// <summary>
                /// 服务器端接收命令处理
                /// </summary>
                ServerOnReceiveStreamCommand,
                /// <summary>
                /// 客户端接收会话标识
                /// </summary>
                ClientOnReceiveIdentity
            }
            /// <summary>
            /// 异步回调
            /// </summary>
            /// <param name="socket"></param>
            /// <param name="type"></param>
            /// <param name="value"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public static void OnReceive(socket socket, type type, int value)
            {
                try
                {
                    switch (type)
                    {
                        case tryReceiver.type.ServerOnReceiveStreamCommandLength: new unionType { Value = socket }.TcpCommandServerSocket.OnReceiveStreamCommandLength(value); return;
                        case tryReceiver.type.ServerOnReceiveStreamIdentityCommand: new unionType { Value = socket }.TcpCommandServerSocket.OnReceiveStreamIdentityCommand(value); return;
                        case tryReceiver.type.ServerOnReceiveStreamCommand: new unionType { Value = socket }.TcpCommandServerSocket.OnReceiveStreamCommand(value); return;
                        case tryReceiver.type.ClientOnReceiveIdentity: new unionType { Value = socket }.TcpCommandClientSocket.OnReceiveIdentity(value); return;
                    }
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
            }
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="type">数据接收回调类型</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="endIndex">结束位置</param>
        /// <param name="timeout">接收超时</param>
        protected void tryReceive(tryReceiver.type type, int startIndex, int endIndex, DateTime timeout)
        {
            tryReceiver tryReceiver = typePool<tryReceiver>.Pop();
            if (tryReceiver == null)
            {
                try
                {
                    tryReceiver = new tryReceiver();
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                if (tryReceiver == null)
                {
                    tryReceiver.OnReceive(this, type, -1);
                    return;
                }
            }
            currentReceiveStartIndex = startIndex;
            currentReceiveEndIndex = endIndex;
            tryReceiver.Receive(this, type, timeout);
        }
        /// <summary>
        /// 数据接收器
        /// </summary>
        internal sealed class receiver
        {
            /// <summary>
            /// 超时时间
            /// </summary>
            public DateTime Timeout;
            /// <summary>
            /// 异步套接字
            /// </summary>
            public socket Socket;
            /// <summary>
            /// 回调对象
            /// </summary>
            public object Value;
            /// <summary>
            /// 数据接收完成后的回调委托
            /// </summary>
            private EventHandler<SocketAsyncEventArgs> asyncCallback;
            /// <summary>
            /// 数据接收回调类型
            /// </summary>
            public type Type;
            /// <summary>
            /// 数据接收器
            /// </summary>
            public receiver()
            {
                asyncCallback = onReceive;
            }
            /// <summary>
            /// 接收数据
            /// </summary>
            public void Receive(socket socket, object value, type type, DateTime timeout)
            {
                SocketAsyncEventArgs async = null;
                Timeout = timeout;
                Socket = socket;
                Value = value;
                Type = type;
                try
                {
                    async = socketAsyncEventArgs.Get();
                    async.UserToken = this;
                    async.Completed += asyncCallback;
                    if (receive(async)) return;
                }
                catch (Exception error)
                {
                    socket.lastException = error;
                }
                this.callback(async);
            }
            /// <summary>
            /// 继续接收数据
            /// </summary>
            /// <param name="async">异步套接字操作</param>
            /// <returns>是否接收成功</returns>
            private bool receive(SocketAsyncEventArgs async)
            {
                RECEIVE:
                async.SetBuffer(Socket.currentReceiveData, Socket.currentReceiveStartIndex, Socket.currentReceiveEndIndex - Socket.currentReceiveStartIndex);
                if (Socket.Socket.ReceiveAsync(async)) return true;
                if (async.SocketError == SocketError.Success)
                {
                    int count = async.BytesTransferred;
                    if (count > 0)
                    {
                        Socket.currentReceiveStartIndex += count;
                        if (Socket.currentReceiveStartIndex == Socket.currentReceiveEndIndex)
                        {
                            callback(true, async);
                            return true;
                        }
                        goto RECEIVE;
                    }
                }
                else Socket.socketError = async.SocketError;
                return false;
            }
            /// <summary>
            /// 数据接收完成后的回调委托
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="async">异步回调参数</param>
            private void onReceive(object sender, SocketAsyncEventArgs async)
            {
                bool isSuccess = false;
                if (date.nowTime.Now <= Timeout)
                {
                    try
                    {
                        if (async.SocketError == SocketError.Success)
                        {
                            int count = async.BytesTransferred;
                            if (count != 0)
                            {
                                Socket.currentReceiveStartIndex += count;
                                if (Socket.currentReceiveStartIndex == Socket.currentReceiveEndIndex)
                                {
                                    isSuccess = true;
                                    callback(true, async);
                                    return;
                                }
                                else if (receive(async)) return;
                            }
                        }
                        else Socket.socketError = async.SocketError;
                    }
                    catch (Exception error)
                    {
                        Socket.lastException = error;
                    }
                }
                if (!isSuccess) callback(async);
            }
            /// <summary>
            /// 异步回调
            /// </summary>
            /// <param name="isReceive">数据接收是否成功</param>
            /// <param name="async">异步回调参数</param>
            private void callback(bool isReceive, SocketAsyncEventArgs async)
            {
                Socket = null;
                if (async == null) push(isReceive);
                else
                {
                    try
                    {
                        async.Completed -= asyncCallback;
                        socketAsyncEventArgs.PushNotNull(async);
                    }
                    finally
                    {
                        push(isReceive);
                    }
                }
            }
            /// <summary>
            /// 异步回调
            /// </summary>
            /// <param name="async">异步回调参数</param>
            private void callback(SocketAsyncEventArgs async)
            {
                try
                {
                    Socket.error();
                }
                finally { callback(false, async); }
            }
            /// <summary>
            /// 添加回调对象
            /// </summary>
            /// <param name="value">回调值</param>
            private void push(bool value)
            {
                object socket = Value;
                type type = Type;
                Value = null;
                try
                {
                    typePool<receiver>.PushNotNull(this);
                }
                finally { OnReceive(socket, type, value); }
            }
            /// <summary>
            /// 数据接收回调类型
            /// </summary>
            public enum type
            {
                /// <summary>
                /// 服务端读取数据回调操作
                /// </summary>
                ServerStreamReceiverOnReadData,
                /// <summary>
                /// 服务端读取数据回调操作
                /// </summary>
                ServerStreamReceiverOnReadCompressData,
                /// <summary>
                /// 服务端获取TCP调用客户端套接字类型
                /// </summary>
                ServerOnSocketType,
                /// <summary>
                /// 客户端获取非压缩数据
                /// </summary>
                ClientReceiveNoCompress,
                /// <summary>
                /// 获取压缩数据
                /// </summary>
                ClientReceiveCompress
            }
            /// <summary>
            /// 异步回调
            /// </summary>
            /// <param name="socket"></param>
            /// <param name="type"></param>
            /// <param name="value"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public static void OnReceive(object socket, type type, bool value)
            {
                try
                {
                    switch (type)
                    {
                        case receiver.type.ServerStreamReceiverOnReadData: new unionType { Value = socket }.TcpCommandServerSocketStreamReceiver.OnReadData(value); return;
                        case receiver.type.ServerStreamReceiverOnReadCompressData: new unionType { Value = socket }.TcpCommandServerSocketStreamReceiver.OnReadCompressData(value); return;
                        case receiver.type.ServerOnSocketType: new unionType { Value = socket }.TcpCommandServerSocket.OnSocketType(value); return;
                        case receiver.type.ClientReceiveNoCompress: new unionType { Value = socket }.TcpCommandClientSocket.ReceiveNoCompress(value); return;
                        case receiver.type.ClientReceiveCompress: new unionType { Value = socket }.TcpCommandClientSocket.ReceiveCompress(value); return;
                    }
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
            }
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="data">数据缓存</param>
        /// <param name="length">待接收数据长度</param>
        /// <param name="timeout">超时时间</param>
        internal void receive(object value, receiver.type type, byte[] data, int startIndex, int length, DateTime timeout)
        {
            receiver receiver = typePool<receiver>.Pop();
            if (receiver == null)
            {
                try
                {
                    receiver = new receiver();
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                if (receiver == null)
                {
                    receiver.OnReceive(value, type, false);
                    return;
                }
            }
            currentReceiveData = data;
            currentReceiveStartIndex = startIndex;
            currentReceiveEndIndex = length + startIndex;
            receiver.Receive(this, value, type, timeout);
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="length">待接收数据长度</param>
        /// <param name="timeout">超时时间</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void receive(object value, receiver.type type, int startIndex, int length, DateTime timeout)
        {
            receive(value, type, currentReceiveData, startIndex, length, timeout);
        }
        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="error">错误异常</param>
        public static void ErrorLog(Exception error)
        {
            if (error != null)
            {
                IOException ioError = error as IOException;
                if (ioError != null) error = ioError.InnerException;
                ObjectDisposedException disposedError = error.InnerException as ObjectDisposedException;
                if (disposedError == null)
                {
                    System.Net.Sockets.SocketException socketException = error as System.Net.Sockets.SocketException;
                    if (socketException == null || socketException.ErrorCode != 10053 || socketException.ErrorCode != 10054) log.Default.Add(error, null, true);
                }
                else log.Default.Add(error, null, true);
            }
        }
    }
    /// <summary>
    /// 套接字扩展
    /// </summary>
    public static class socketExpand
    {
        /// <summary>
        /// 关闭套接字
        /// </summary>
        /// <param name="socket">套接字</param>
        public static void shutdown(this Socket socket)
        {
            if (socket != null)
            {
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                }
                catch { }
                finally
                {
                    socket.Dispose();
                }
            }
        }
        /// <summary>
        /// 关闭套接字
        /// </summary>
        /// <param name="socket">套接字</param>
        internal static void shutdownConnected(this Socket socket)
        {
            if (socket.Connected)
            {
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                }
                catch { }
                finally
                {
                    socket.Dispose();
                }
                //try
                //{
                //    socket.Shutdown(SocketShutdown.Both);
                //}
                //catch(Exception error)
                //{
                //    log.Default.Add(error, null, false);
                //}
            }
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <param name="data">数据</param>
        /// <param name="error">套接字错误</param>
        /// <returns>是否成功</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static bool send(this Socket socket, ref subArray<byte> data, ref SocketError error)
        {
            return send(socket, data.array, data.startIndex, data.length, ref error);
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <param name="data">数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="length">发送长度</param>
        /// <param name="error">套接字错误</param>
        /// <returns>是否成功</returns>
        public static bool send(this Socket socket, byte[] data, int startIndex, int length, ref SocketError error)
        {
            if (socket != null)
            {
                while (length > 0)
                {
                    int count = socket.Send(data, startIndex, length, SocketFlags.None, out error);
                    if (count <= 0 || error != SocketError.Success) break;
                    if ((length -= count) == 0) return true;
                    startIndex += count;
                }
            }
            return false;
        }
        /// <summary>
        /// 服务器端发送数据(限制单次发送数据量)
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <param name="data">数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="length">发送长度</param>
        /// <param name="error">套接字错误</param>
        /// <returns>是否成功</returns>
        internal static bool serverSend(this Socket socket, byte[] data, int startIndex, int length, ref SocketError error)
        {
            int maxSendSize = net.socket.MaxServerSendSize;
            while (length > maxSendSize)
            {
                int count = socket.Send(data, startIndex, maxSendSize, SocketFlags.None, out error);
                if (count <= 0 || error != SocketError.Success) return false;
                length -= count;
                startIndex += count;
            }
            while (length > 0)
            {
                //Thread.Sleep(0);
                int count = socket.Send(data, startIndex, length, SocketFlags.None, out error);
                if (count <= 0 || error != SocketError.Success) break;
                if ((length -= count) == 0) return true;
                startIndex += count;
            }
            return false;
        }
    }
}
