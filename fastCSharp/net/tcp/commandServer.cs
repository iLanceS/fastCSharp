using System;
using System.Net.Sockets;
using fastCSharp.threading;
using fastCSharp.io;
using fastCSharp.io.compression;
using fastCSharp.code.cSharp;
using System.Threading;
using System.IO;
using fastCSharp.reflection;
using System.Text;
using fastCSharp.net.tcp.http;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Net;

namespace fastCSharp.net.tcp
{
    /// <summary>
    /// TCP调用服务端
    /// </summary>
    public abstract class commandServer : server
    {
        /// <summary>
        /// 数据命令验证会话标识
        /// </summary>
        internal const int VerifyIdentity = 0x060C5113;
        /// <summary>
        /// 序号命令验证会话标识
        /// </summary>
        internal const int IdentityVerifyIdentity = 0x10035113;
        /// <summary>
        /// 空验证命令序号
        /// </summary>
        private const int nullVerifyCommandIdentity = -VerifyIdentity;
        /// <summary>
        /// 用户命令起始位置
        /// </summary>
        public const int CommandStartIndex = 128;
        /// <summary>
        /// 用户命令数据起始位置
        /// </summary>
        public const int CommandDataIndex = 0x20202000;
        /// <summary>
        /// 关闭命令
        /// </summary>
        public const int CloseIdentityCommand = CommandStartIndex - 1;
        /// <summary>
        /// 连接检测命令
        /// </summary>
        public const int CheckIdentityCommand = CloseIdentityCommand - 1;
        /// <summary>
        /// 负载均衡连接检测命令
        /// </summary>
        public const int LoadBalancingCheckIdentityCommand = CheckIdentityCommand - 1;
        /// <summary>
        /// 流合并命令
        /// </summary>
        public const int StreamMergeIdentityCommand = LoadBalancingCheckIdentityCommand - 1;
        /// <summary>
        /// TCP流回应命令
        /// </summary>
        public const int TcpStreamCommand = StreamMergeIdentityCommand - 1;
        /// <summary>
        /// 忽略分组命令
        /// </summary>
        public const int IgnoreGroupCommand = TcpStreamCommand - 1;
        /// <summary>
        /// 最小系统命令
        /// </summary>
        private const int minCommand = IgnoreGroupCommand;
        /// <summary>
        /// 关闭链接命令
        /// </summary>
        private static readonly byte[] closeCommandData = BitConverter.GetBytes(CloseIdentityCommand + CommandDataIndex);
        /// <summary>
        /// 流合并命令
        /// </summary>
        private static readonly byte[] streamMergeCommandData = BitConverter.GetBytes(StreamMergeIdentityCommand + CommandDataIndex);
        /// <summary>
        /// 连接检测命令
        /// </summary>
        private static readonly byte[] checkCommandData = BitConverter.GetBytes(CheckIdentityCommand + CommandDataIndex);
        /// <summary>
        /// 负载均衡连接检测命令
        /// </summary>
        private static readonly byte[] loadBalancingCheckCommandData = BitConverter.GetBytes(LoadBalancingCheckIdentityCommand + CommandDataIndex);
        /// <summary>
        /// TCP流回馈命令
        /// </summary>
        private static readonly byte[] tcpStreamCommandData = BitConverter.GetBytes(TcpStreamCommand + CommandDataIndex);
        /// <summary>
        /// 忽略分组命令
        /// </summary>
        private static readonly byte[] ignoreGroupCommandData = BitConverter.GetBytes(IgnoreGroupCommand + CommandDataIndex);
        /// <summary>
        /// 连接检测套接字
        /// </summary>
        private static readonly command mergeCheckCommand = new command(StreamMergeIdentityCommand, 0);
        /// <summary>
        /// 命令处理委托
        /// </summary>
        public struct command
        {
            /// <summary>
            /// 命令处理索引
            /// </summary>
            public int CommandIndex;
            /// <summary>
            /// 最大参数数据长度,0表示不接受参数数据
            /// </summary>
            public int MaxDataLength;
            /// <summary>
            /// 命令处理委托
            /// </summary>
            /// <param name="index">命令处理索引</param>
            /// <param name="maxDataLength">最大参数数据长度,0表示不接受参数数据</param>
            public command(int index, int maxDataLength = int.MaxValue)
            {
                CommandIndex = index;
                MaxDataLength = maxDataLength;
            }
            /// <summary>
            /// 设置命令处理委托
            /// </summary>
            /// <param name="index">命令处理索引</param>
            /// <param name="maxDataLength">最大参数数据长度,0表示不接受参数数据</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Set(int index, int maxDataLength = int.MaxValue)
            {
                CommandIndex = index;
                MaxDataLength = maxDataLength;
            }
            /// <summary>
            /// 命令处理委托
            /// </summary>
            /// <param name="socket"></param>
            /// <param name="data"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void DoCommand(commandServer.socket socket, ref subArray<byte> data)
            {
                socket.commandSocketProxy.doCommand(CommandIndex, socket, ref data);
            }
            /// <summary>
            /// 命令处理委托
            /// </summary>
            /// <param name="socket"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void DoCommand(commandServer.socket socket)
            {
                subArray<byte> data = default(subArray<byte>);
                socket.commandSocketProxy.doCommand(CommandIndex, socket, ref data);
            }
            /// <summary>
            /// 命令处理委托
            /// </summary>
            /// <param name="socket"></param>
            /// <param name="data"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void DoCommandMark(commandServer.socket socket, subArray<byte> data)
            {
                DoCommandMark(socket, ref data);
            }
            /// <summary>
            /// 命令处理委托
            /// </summary>
            /// <param name="socket"></param>
            /// <param name="data"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public unsafe void DoCommandMark(commandServer.socket socket, ref subArray<byte> data)
            {
                if (socket.MarkData != 0) Mark(ref data, socket.MarkData);
                socket.commandSocketProxy.doCommand(CommandIndex, socket, ref data);
            }
        }
        /// <summary>
        /// 会话标识
        /// </summary>
        public struct streamIdentity
        {
            /// <summary>
            /// 请求标识
            /// </summary>
            public int Identity;
            /// <summary>
            /// 请求索引
            /// </summary>
            public int Index;
            /// <summary>
            /// 会话标识
            /// </summary>
            /// <param name="index"></param>
            internal void Set(int index)
            {
                Identity = 0;
                Index = index;
            }
            /// <summary>
            /// 会话标识
            /// </summary>
            /// <param name="identity"></param>
            /// <param name="index"></param>
            internal void Set(int identity, int index)
            {
                Identity = identity;
                Index = index;
            }
        }
        /// <summary>
        /// 命令参数
        /// </summary>
        [Flags]
        public enum commandFlags : uint
        {
            /// <summary>
            /// 是否采用JSON序列化,否则使用二进制序列化
            /// </summary>
            JsonSerialize = 1,
        }
        /// <summary>
        /// 服务器端调用
        /// </summary>
        public abstract class socketCall
        {
            /// <summary>
            /// 套接字
            /// </summary>
            protected socket socket;
            /// <summary>
            /// 回话标识
            /// </summary>
            protected streamIdentity identity;
            /// <summary>
            /// 套接字重用标识
            /// </summary>
            protected int pushIdentity;
            /// <summary>
            /// 命令参数
            /// </summary>
            protected commandFlags flags;
            /// <summary>
            /// 判断套接字是否有效
            /// </summary>
            protected int isVerify
            {
                get { return pushIdentity ^ socket.PushIdentity; }
            }
            /// <summary>
            /// 调用处理
            /// </summary>
            public abstract void Call();
        }
        /// <summary>
        /// 服务器端调用
        /// </summary>
        /// <typeparam name="callType">套接字类型</typeparam>
        public abstract class socketCall<callType> : socketCall
            where callType : socketCall<callType>
        {
            /// <summary>
            /// 设置参数
            /// </summary>
            /// <param name="socket">套接字</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private void set(socket socket)
            {
                this.socket = socket;
                this.identity = socket.identity;
                this.flags = socket.flags;
                pushIdentity = socket.PushIdentity;
            }
            /// <summary>
            /// 获取服务器端调用
            /// </summary>
            /// <param name="socket"></param>
            /// <returns></returns>
            public static socketCall GetCall(socket socket)
            {
                callType value = fastCSharp.typePool<callType>.Pop();
                if (value == null)
                {
                    try
                    {
                        value = fastCSharp.emit.constructor<callType>.New();
                    }
                    catch (Exception error)
                    {
                        fastCSharp.log.Error.Add(error, null, false);
                        return null;
                    }
                }
                value.set(socket);
                return value;
            }
            /// <summary>
            /// 设置参数
            /// </summary>
            /// <param name="socket">套接字</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private void call(socket socket)
            {
                this.socket = socket;
                this.identity = socket.identity;
                this.flags = socket.flags;
                pushIdentity = socket.PushIdentity;
                Call();
            }
            /// <summary>
            /// 获取服务器端调用
            /// </summary>
            /// <param name="socket"></param>
            /// <returns></returns>
            public static void Call(socket socket)
            {
                callType value = fastCSharp.typePool<callType>.Pop();
                if (value == null)
                {
                    try
                    {
                        value = fastCSharp.emit.constructor<callType>.New();
                    }
                    catch (Exception error)
                    {
                        fastCSharp.log.Error.Add(error, null, false);
                        return;
                    }
                }
                value.call(socket);
            }
        }
        /// <summary>
        /// 服务器端调用
        /// </summary>
        /// <typeparam name="callType">套接字类型</typeparam>
        /// <typeparam name="inputParameterType">输入参数类型</typeparam>
        public abstract class socketCall<callType ,inputParameterType> : socketCall
            where callType : socketCall<callType, inputParameterType>
        {
            /// <summary>
            /// 输入参数
            /// </summary>
            protected inputParameterType inputParameter;
            /// <summary>
            /// 设置参数
            /// </summary>
            /// <param name="socket">套接字</param>
            /// <param name="inputParameter">输入参数</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private void set(socket socket, ref inputParameterType inputParameter)
            {
                this.socket = socket;
                this.identity = socket.identity;
                this.flags = socket.flags;
                this.inputParameter = inputParameter;
                pushIdentity = socket.PushIdentity;
            }
            /// <summary>
            /// 获取服务器端调用
            /// </summary>
            /// <param name="socket"></param>
            /// <param name="inputParameter"></param>
            /// <returns></returns>
            public static socketCall GetCall(socket socket, ref inputParameterType inputParameter)
            {
                callType value = fastCSharp.typePool<callType>.Pop();
                if (value == null)
                {
                    try
                    {
                        value = fastCSharp.emit.constructor<callType>.New();
                    }
                    catch (Exception error)
                    {
                        fastCSharp.log.Error.Add(error, null, false);
                        return null;
                    }
                }
                value.set(socket, ref inputParameter);
                return value;
            }
            /// <summary>
            /// 设置参数
            /// </summary>
            /// <param name="socket">套接字</param>
            /// <param name="inputParameter">输入参数</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private void call(socket socket, ref inputParameterType inputParameter)
            {
                this.socket = socket;
                this.identity = socket.identity;
                this.flags = socket.flags;
                this.inputParameter = inputParameter;
                pushIdentity = socket.PushIdentity;
                Call();
            }
            /// <summary>
            /// 获取服务器端调用
            /// </summary>
            /// <param name="socket"></param>
            /// <param name="inputParameter"></param>
            /// <returns></returns>
            public static void Call(socket socket, ref inputParameterType inputParameter)
            {
                callType value = fastCSharp.typePool<callType>.Pop();
                if (value == null)
                {
                    try
                    {
                        value = fastCSharp.emit.constructor<callType>.New();
                    }
                    catch (Exception error)
                    {
                        fastCSharp.log.Error.Add(error, null, false);
                        return;
                    }
                }
                value.call(socket, ref inputParameter);
            }
            ///// <summary>
            ///// 设置参数
            ///// </summary>
            ///// <param name="socket">套接字</param>
            ///// <param name="identity">回话标识</param>
            ///// <param name="inputParameter">输入参数</param>
            //private void set(socket socket, streamIdentity identity, inputParameterType inputParameter)
            //{
            //    this.socket = socket;
            //    this.identity = identity;
            //    this.inputParameter = inputParameter;
            //    pushIdentity = socket.PushIdentity;
            //}
            ///// <summary>
            ///// 获取服务器端调用
            ///// </summary>
            ///// <param name="socket"></param>
            ///// <param name="identity"></param>
            ///// <param name="inputParameter"></param>
            ///// <returns></returns>
            //public static Action Call(socket socket, fastCSharp.net.tcp.commandServer.streamIdentity identity, inputParameterType inputParameter)
            //{
            //    callType value = fastCSharp.typePool<callType>.Pop();
            //    if (value == null)
            //    {
            //        try
            //        {
            //            value = fastCSharp.emit.constructor<callType>.New();
            //        }
            //        catch (Exception error)
            //        {
            //            fastCSharp.log.Error.Add(error, null, false);
            //            return null;
            //        }
            //    }
            //    value.set(socket, identity, inputParameter);
            //    return value.callHandle;
            //}
        }
        /// <summary>
        /// 服务器端调用
        /// </summary>
        /// <typeparam name="callType">套接字类型</typeparam>
        /// <typeparam name="serverType">服务器目标对象类型</typeparam>
        public abstract class serverCall<callType, serverType> : socketCall
            where callType : serverCall<callType, serverType>
        {
            /// <summary>
            /// 服务器目标对象
            /// </summary>
            protected serverType serverValue;
            /// <summary>
            /// 设置参数
            /// </summary>
            /// <param name="socket">套接字</param>
            /// <param name="serverValue">服务器目标对象</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private void set(socket socket, serverType serverValue)
            {
                this.socket = socket;
                this.serverValue = serverValue;
                this.identity = socket.identity;
                this.flags = socket.flags;
                pushIdentity = socket.PushIdentity;
            }
            /// <summary>
            /// 获取服务器端调用
            /// </summary>
            /// <param name="socket"></param>
            /// <param name="serverValue"></param>
            /// <returns></returns>
            public static socketCall GetCall(socket socket, serverType serverValue)
            {
                callType value = fastCSharp.typePool<callType>.Pop();
                if (value == null)
                {
                    try
                    {
                        value = fastCSharp.emit.constructor<callType>.New();
                    }
                    catch (Exception error)
                    {
                        fastCSharp.log.Error.Add(error, null, false);
                        return null;
                    }
                }
                value.set(socket, serverValue);
                return value;
            }
            /// <summary>
            /// 设置参数
            /// </summary>
            /// <param name="socket">套接字</param>
            /// <param name="serverValue">服务器目标对象</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private void call(socket socket, serverType serverValue)
            {
                this.socket = socket;
                this.serverValue = serverValue;
                this.identity = socket.identity;
                this.flags = socket.flags;
                pushIdentity = socket.PushIdentity;
                Call();
            }
            /// <summary>
            /// 获取服务器端调用
            /// </summary>
            /// <param name="socket"></param>
            /// <param name="serverValue"></param>
            /// <returns></returns>
            public static void Call(socket socket, serverType serverValue)
            {
                callType value = fastCSharp.typePool<callType>.Pop();
                if (value == null)
                {
                    try
                    {
                        value = fastCSharp.emit.constructor<callType>.New();
                    }
                    catch (Exception error)
                    {
                        fastCSharp.log.Error.Add(error, null, false);
                        return;
                    }
                }
                value.call(socket, serverValue);
            }
        }
        /// <summary>
        /// 服务器端调用
        /// </summary>
        /// <typeparam name="callType">套接字类型</typeparam>
        /// <typeparam name="serverType">服务器目标对象类型</typeparam>
        /// <typeparam name="inputParameterType">输入参数类型</typeparam>
        public abstract class serverCall<callType, serverType, inputParameterType> : serverCall<callType, serverType>
            where callType : serverCall<callType, serverType, inputParameterType>
        {
            /// <summary>
            /// 输入参数
            /// </summary>
            protected inputParameterType inputParameter;
            /// <summary>
            /// 设置参数
            /// </summary>
            /// <param name="socket">套接字</param>
            /// <param name="serverValue">服务器目标对象</param>
            /// <param name="inputParameter">输入参数</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private void set(socket socket, serverType serverValue, ref inputParameterType inputParameter)
            {
                this.socket = socket;
                this.serverValue = serverValue;
                this.identity = socket.identity;
                this.flags = socket.flags;
                this.inputParameter = inputParameter;
                pushIdentity = socket.PushIdentity;
            }
            /// <summary>
            /// 获取服务器端调用
            /// </summary>
            /// <param name="socket"></param>
            /// <param name="serverValue"></param>
            /// <param name="inputParameter"></param>
            /// <returns></returns>
            public static socketCall GetCall(socket socket, serverType serverValue, ref inputParameterType inputParameter)
            {
                callType value = fastCSharp.typePool<callType>.Pop();
                if (value == null)
                {
                    try
                    {
                        value = fastCSharp.emit.constructor<callType>.New();
                    }
                    catch (Exception error)
                    {
                        fastCSharp.log.Error.Add(error, null, false);
                        return null;
                    }
                }
                value.set(socket, serverValue, ref inputParameter);
                return value;
            }
            /// <summary>
            /// 设置参数
            /// </summary>
            /// <param name="socket">套接字</param>
            /// <param name="serverValue">服务器目标对象</param>
            /// <param name="inputParameter">输入参数</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private void call(socket socket, serverType serverValue, ref inputParameterType inputParameter)
            {
                this.socket = socket;
                this.serverValue = serverValue;
                this.identity = socket.identity;
                this.flags = socket.flags;
                this.inputParameter = inputParameter;
                pushIdentity = socket.PushIdentity;
                Call();
            }
            /// <summary>
            /// 获取服务器端调用
            /// </summary>
            /// <param name="socket"></param>
            /// <param name="serverValue"></param>
            /// <param name="inputParameter"></param>
            /// <returns></returns>
            public static void Call(socket socket, serverType serverValue, ref inputParameterType inputParameter)
            {
                callType value = fastCSharp.typePool<callType>.Pop();
                if (value == null)
                {
                    try
                    {
                        value = fastCSharp.emit.constructor<callType>.New();
                    }
                    catch (Exception error)
                    {
                        fastCSharp.log.Error.Add(error, null, false);
                        return;
                    }
                }
                value.call(socket, serverValue, ref inputParameter);
            }
            ///// <summary>
            ///// 设置参数
            ///// </summary>
            ///// <param name="socket">套接字</param>
            ///// <param name="serverValue">服务器目标对象</param>
            ///// <param name="identity">回话标识</param>
            ///// <param name="inputParameter">输入参数</param>
            //private void set(socket socket, serverType serverValue, streamIdentity identity, inputParameterType inputParameter)
            //{
            //    this.socket = socket;
            //    this.serverValue = serverValue;
            //    this.identity = identity;
            //    this.inputParameter = inputParameter;
            //    pushIdentity = socket.PushIdentity;
            //}
            ///// <summary>
            ///// 获取服务器端调用
            ///// </summary>
            ///// <param name="socket"></param>
            ///// <param name="serverValue"></param>
            ///// <param name="identity"></param>
            ///// <param name="inputParameter"></param>
            ///// <returns></returns>
            //public static Action Call(socket socket, serverType serverValue, fastCSharp.net.tcp.commandServer.streamIdentity identity, inputParameterType inputParameter)
            //{
            //    callType value = fastCSharp.typePool<callType>.Pop();
            //    if (value == null)
            //    {
            //        try
            //        {
            //            value = fastCSharp.emit.constructor<callType>.New();
            //        }
            //        catch (Exception error)
            //        {
            //            fastCSharp.log.Error.Add(error, null, false);
            //            return null;
            //        }
            //    }
            //    value.set(socket, serverValue, identity, inputParameter);
            //    return value.callHandle;
            //}
        }
        /// <summary>
        /// TCP流命令类型
        /// </summary>
        internal enum tcpStreamCommand : byte
        {
            /// <summary>
            /// 获取流字节长度
            /// </summary>
            GetLength,
            /// <summary>
            /// 设置流字节长度
            /// </summary>
            SetLength,
            /// <summary>
            /// 获取当前位置
            /// </summary>
            GetPosition,
            /// <summary>
            /// 设置当前位置
            /// </summary>
            SetPosition,
            /// <summary>
            /// 获取读取超时
            /// </summary>
            GetReadTimeout,
            /// <summary>
            /// 设置读取超时
            /// </summary>
            SetReadTimeout,
            /// <summary>
            /// 获取写入超时
            /// </summary>
            GetWriteTimeout,
            /// <summary>
            /// 设置写入超时
            /// </summary>
            SetWriteTimeout,
            /// <summary>
            /// 异步读取
            /// </summary>
            BeginRead,
            /// <summary>
            /// 读取字节序列
            /// </summary>
            Read,
            /// <summary>
            /// 读取字节
            /// </summary>
            ReadByte,
            /// <summary>
            /// 异步写入
            /// </summary>
            BeginWrite,
            /// <summary>
            /// 写入字节序列
            /// </summary>
            Write,
            /// <summary>
            /// 写入字节
            /// </summary>
            WriteByte,
            /// <summary>
            /// 设置流位置
            /// </summary>
            Seek,
            /// <summary>
            /// 清除缓冲区
            /// </summary>
            Flush,
            /// <summary>
            /// 关闭流
            /// </summary>
            Close
        }
        /// <summary>
        /// TCP流异步接口
        /// </summary>
        internal interface ITcpStreamCallback
        {
            /// <summary>
            /// TCP流异步回调
            /// </summary>
            /// <param name="tcpStreamAsyncResult">TCP流异步操作状态</param>
            /// <param name="parameter">TCP流参数</param>
            void Callback(tcpStreamAsyncResult tcpStreamAsyncResult, tcpStreamParameter parameter);
        }
        /// <summary>
        /// TCP流参数
        /// </summary>
        internal sealed class tcpStreamParameter : callback<fastCSharp.net.returnValue>
        {
            /// <summary>
            /// 命令集合索引
            /// </summary>
            public int Index;
            /// <summary>
            /// 命令序号
            /// </summary>
            public int Identity;
            /// <summary>
            /// 客户端索引
            /// </summary>
            public int ClientIndex;
            /// <summary>
            /// 客户端序号
            /// </summary>
            public int ClientIdentity;
            /// <summary>
            /// 位置参数
            /// </summary>
            public long Offset;
            /// <summary>
            /// 数据参数
            /// </summary>
            public subArray<byte> Data;
            /// <summary>
            /// 查找类型参数
            /// </summary>
            public SeekOrigin SeekOrigin;
            /// <summary>
            /// 命令类型
            /// </summary>
            public tcpStreamCommand Command;
            /// <summary>
            /// 客户端流是否存在
            /// </summary>
            public bool IsClientStream;
            /// <summary>
            /// 客户端命令是否成功
            /// </summary>
            public bool IsCommand;
            /// <summary>
            /// 缓冲区处理
            /// </summary>
            public callback<fastCSharp.net.returnValue> PushClientBuffer
            {
                get
                {
                    if (Data.array != null && Data.array.Length == commandSocket.asyncBuffers.Size) return this;
                    return null;
                }
            }
            /// <summary>
            /// 缓冲区处理
            /// </summary>
            public override void Callback(ref fastCSharp.net.returnValue _)
            {
                commandSocket.asyncBuffers.Push(ref Data.array);
            }
            /// <summary>
            /// 反序列化
            /// </summary>
            /// <param name="deSerializer">对象反序列化器</param>
            internal unsafe void DeSerialize(fastCSharp.emit.dataDeSerializer deSerializer)
            {
                byte* start = deSerializer.Read;
                int bufferSize = *(int*)(start + (sizeof(int) * 5 + sizeof(long))), dataSize = sizeof(int) * 7 + sizeof(long) + ((bufferSize + 3) & (int.MaxValue - 3));
                if (deSerializer.VerifyRead(dataSize) && *(int*)(start + dataSize - sizeof(int)) == dataSize)
                {
                    Index = *(int*)start;
                    Identity = *(int*)(start + sizeof(int));
                    ClientIndex = *(int*)(start + sizeof(int) * 2);
                    ClientIdentity = *(int*)(start + sizeof(int) * 3);
                    Offset = *(long*)(start + sizeof(int) * 4);
                    SeekOrigin = (SeekOrigin)(*(start + (sizeof(int) * 4 + sizeof(long))));
                    Command = (tcpStreamCommand)(*(start + (sizeof(int) * 4 + sizeof(long) + 1)));
                    IsClientStream = *(start + (sizeof(int) * 4 + sizeof(long) + 2)) != 0;
                    IsCommand = *(start + (sizeof(int) * 4 + sizeof(long) + 3)) != 0;
                    if (bufferSize == 0) Data.UnsafeSet(nullValue<byte>.Array, 0, 0);
                    else
                    {
                        Data.UnsafeSet(new byte[bufferSize], 0, bufferSize);
                        fastCSharp.unsafer.memory.Copy(start + (sizeof(int) * 6 + sizeof(long)), Data.array, bufferSize);
                    }
                }
            }
            /// <summary>
            /// 反序列化
            /// </summary>
            /// <param name="deSerializer">对象反序列化器</param>
            /// <param name="value"></param>
            [fastCSharp.emit.dataSerialize.custom]
            private static unsafe void deSerialize(fastCSharp.emit.dataDeSerializer deSerializer, ref tcpStreamParameter value)
            {
                //if (deSerializer.CheckNull() == 0) value = null;
                //else
                //{
                //    if (value == null) value = new tcpStreamParameter();
                //    value.DeSerialize(deSerializer);
                //}
                (value = new tcpStreamParameter()).DeSerialize(deSerializer);
            }
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="serializer"></param>
            private unsafe void serialize(fastCSharp.emit.dataSerializer serializer)
            {
                unmanagedStream stream = serializer.Stream;
                int length = sizeof(int) * 7 + sizeof(long) + ((Data.length + 3) & (int.MaxValue - 3));
                stream.PrepLength(length);
                stream.UnsafeWrite(Index);
                stream.UnsafeWrite(Identity);
                stream.UnsafeWrite(ClientIndex);
                stream.UnsafeWrite(ClientIdentity);
                stream.UnsafeWrite(Offset);
                stream.UnsafeWrite((byte)SeekOrigin);
                stream.UnsafeWrite((byte)Command);
                stream.UnsafeWrite(IsClientStream ? (byte)1 : (byte)0);
                stream.UnsafeWrite(IsCommand ? (byte)1 : (byte)0);
                stream.UnsafeWrite(Data.length);
                if (Data.length != 0)
                {
                    fixed (byte* dataFixed = Data.array)
                    {
                        fastCSharp.unsafer.memory.Copy(dataFixed + Data.startIndex, stream.CurrentData, Data.length);
                    }
                    stream.UnsafeAddLength((Data.length + 3) & (int.MaxValue - 3));
                }
                stream.UnsafeWrite(length);
            }
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="serializer"></param>
            /// <param name="value"></param>
            [fastCSharp.emit.dataSerialize.custom]
            private static void serialize(fastCSharp.emit.dataSerializer serializer, tcpStreamParameter value)
            {
                value.serialize(serializer);
                //if (value == null) serializer.Stream.Write(fastCSharp.emit.binarySerializer.NullValue);
                //else value.serialize(serializer);
            }
            /// <summary>
            /// 空TCP流参数
            /// </summary>
            public static readonly tcpStreamParameter Null = new tcpStreamParameter();
        }
        /// <summary>
        /// TCP流异步操作状态
        /// </summary>
        internal class tcpStreamAsyncResult : IAsyncResult
        {
            /// <summary>
            /// TCP流异步接口
            /// </summary>
            public ITcpStreamCallback TcpStreamCallback;
            /// <summary>
            /// TCP流参数
            /// </summary>
            public tcpStreamParameter Parameter;
            /// <summary>
            /// 异步回调
            /// </summary>
            public AsyncCallback Callback;
            /// <summary>
            /// 用户定义的对象
            /// </summary>
            public object AsyncState { get; set; }
            /// <summary>
            /// 等待异步操作完成
            /// </summary>
            private EventWaitHandle asyncWaitHandle;
            /// <summary>
            /// 等待异步操作完成访问锁
            /// </summary>
            private readonly object asyncWaitHandleLock = new object();
            /// <summary>
            /// 等待异步操作是否完成
            /// </summary>
            public bool IsCallback;
            /// <summary>
            /// 等待异步操作完成
            /// </summary>
            public WaitHandle AsyncWaitHandle
            {
                get
                {
                    if (asyncWaitHandle == null)
                    {
                        Monitor.Enter(asyncWaitHandleLock);
                        try
                        {
                            if (asyncWaitHandle == null) asyncWaitHandle = new EventWaitHandle(IsCallback, EventResetMode.ManualReset);
                        }
                        finally { Monitor.Exit(asyncWaitHandleLock); }
                    }
                    return asyncWaitHandle;
                }
            }
            /// <summary>
            /// 是否同步完成
            /// </summary>
            public bool CompletedSynchronously
            {
                get { return false; }
            }
            /// <summary>
            /// 异步操作是否已完成
            /// </summary>
            public bool IsCompleted { get; set; }
            /// <summary>
            /// 异步回调
            /// </summary>
            /// <param name="parameter">TCP流参数</param>
            public void OnCallback(tcpStreamParameter parameter)
            {
                try
                {
                    TcpStreamCallback.Callback(this, parameter);
                }
                finally
                {
                    Monitor.Enter(asyncWaitHandleLock);
                    IsCallback = true;
                    EventWaitHandle asyncWaitHandle = this.asyncWaitHandle;
                    Monitor.Exit(asyncWaitHandleLock);
                    if (asyncWaitHandle != null) asyncWaitHandle.Set();
                    if (Callback != null) Callback(this);
                }
            }
        }
        /// <summary>
        /// TCP流读取器
        /// </summary>
        private struct tcpStreamReceiver
        {
            /// <summary>
            /// 当前处理序号
            /// </summary>
            public int Identity;
            /// <summary>
            /// 异步状态
            /// </summary>
            public tcpStreamAsyncResult AsyncResult;
            /// <summary>
            /// TCP流参数
            /// </summary>
            public tcpStreamParameter Parameter;
            /// <summary>
            /// TCP流读取等待
            /// </summary>
            public EventWaitHandle ReceiveWait;
            /// <summary>
            /// 获取读取数据
            /// </summary>
            /// <param name="identity">当前处理序号</param>
            /// <param name="parameter">TCP流参数</param>
            /// <returns>是否成功</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public bool Get(int identity, ref tcpStreamParameter parameter)
            {
                if (identity == Identity)
                {
                    parameter = Parameter;
                    AsyncResult = null;
                    ++Identity;
                    Parameter = null;
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 设置异步状态
            /// </summary>
            /// <param name="asyncResult">异步状态</param>
            public void SetAsyncResult(tcpStreamAsyncResult asyncResult)
            {
                if (asyncResult == null)
                {
                    if (ReceiveWait == null) ReceiveWait = new EventWaitHandle(false, EventResetMode.AutoReset);
                    else ReceiveWait.Reset();
                }
                else AsyncResult = asyncResult;
            }
            /// <summary>
            /// 取消读取
            /// </summary>
            /// <param name="isSetWait">是否设置结束状态</param>
            public void Cancel(bool isSetWait)
            {
                tcpStreamAsyncResult asyncResult = AsyncResult;
                ++Identity;
                Parameter = null;
                AsyncResult = null;
                if (asyncResult == null)
                {
                    if (isSetWait && ReceiveWait != null) ReceiveWait.Set();
                }
                else asyncResult.OnCallback(null);
            }
            /// <summary>
            /// 取消读取
            /// </summary>
            /// <param name="identity">当前处理序号</param>
            /// <param name="isSetWait">是否设置结束状态</param>
            /// <returns>是否成功</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public bool Cancel(int identity, bool isSetWait)
            {
                if (identity == Identity)
                {
                    Cancel(isSetWait);
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 设置TCP流参数
            /// </summary>
            /// <param name="parameter">TCP流参数</param>
            /// <param name="asyncResult">异步状态</param>
            /// <returns>是否成功</returns>
            public bool Set(tcpStreamParameter parameter, ref tcpStreamAsyncResult asyncResult)
            {
                if (Identity == parameter.Identity)
                {
                    asyncResult = AsyncResult;
                    if (AsyncResult == null)
                    {
                        Parameter = parameter;
                        ReceiveWait.Set();
                    }
                    else
                    {
                        ++Identity;
                        AsyncResult = null;
                    }
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// HTTP命令处理委托
        /// </summary>
        public struct httpCommand
        {
            /// <summary>
            /// 命令处理委托编号
            /// </summary>
            public int CommandIndex;
            /// <summary>
            /// 最大参数数据长度,0表示不接受参数数据
            /// </summary>
            public int MaxDataLength;
            /// <summary>
            /// 是否仅支持POST调用
            /// </summary>
            public bool IsPostOnly;
            ///// <summary>
            ///// HTTP命令处理委托
            ///// </summary>
            ///// <param name="onCommand">命令处理委托</param>
            ///// <param name="IsPostOnly">是否仅支持POST调用</param>
            ///// <param name="maxDataLength">最大参数数据长度,0表示不接受参数数据</param>
            //public httpCommand(Action<http.socketBase> onCommand, bool isPostOnly, int maxDataLength = int.MaxValue)
            //{
            //    OnCommand = onCommand;
            //    MaxDataLength = maxDataLength;
            //    IsPostOnly = isPostOnly;
            //}
            /// <summary>
            /// 设置命令处理委托
            /// </summary>
            /// <param name="commandIndex">命令处理委托编号</param>
            /// <param name="isPostOnly">是否仅支持POST调用</param>
            /// <param name="maxDataLength">最大参数数据长度,0表示不接受参数数据</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Set(int commandIndex, bool isPostOnly, int maxDataLength = int.MaxValue)
            {
                CommandIndex = commandIndex;
                MaxDataLength = maxDataLength;
                IsPostOnly = isPostOnly;
            }
        }
        /// <summary>
        /// HTTP服务器
        /// </summary>
        internal sealed class httpServers : http.servers
        {
            /// <summary>
            /// HTTP服务
            /// </summary>
            public new sealed class server : http.domainServer
            {
                /// <summary>
                /// HTTP服务器
                /// </summary>
                private httpServers httpServers;
                /// <summary>
                /// TCP调用服务端
                /// </summary>
                public commandServer CommandServer
                {
                    get { return httpServers.commandServer; }
                }
                /// <summary>
                /// 客户端缓存时间(单位:秒)
                /// </summary>
                protected override int clientCacheSeconds
                {
                    get { return 0; }
                }
                /// <summary>
                /// 最大文件缓存字节数(单位KB)
                /// </summary>
                protected override int maxCacheFileSize
                {
                    get { return 0; }
                }
                /// <summary>
                /// 文件路径
                /// </summary>
                protected override int maxCacheSize
                {
                    get { return 0; }
                }
                /// <summary>
                /// HTTP服务
                /// </summary>
                /// <param name="httpServers">HTTP服务器</param>
                public server(httpServers httpServers)
                {
                    this.httpServers = httpServers;
                    Session = new http.session<object>();
                }
                /// <summary>
                /// 启动HTTP服务
                /// </summary>
                /// <param name="domains">域名信息集合</param>
                /// <param name="onStop">停止服务处理</param>
                /// <returns>是否启动成功</returns>
                public override bool Start(http.domain[] domains, Action onStop)
                {
                    return false;
                }
                /// <summary>
                /// HTTP请求处理
                /// </summary>
                /// <param name="socket">HTTP套接字</param>
                /// <param name="socketIdentity">套接字操作编号</param>
                public override void Request(http.socketBase socket, long socketIdentity)
                {
                    requestHeader request = socket.RequestHeader;
                    response response = null;
                    try
                    {
                        subArray<byte> commandName = request.Path;
                        if (commandName.length != 0)
                        {
                            commandName.UnsafeSet(commandName.startIndex + 1, commandName.length - 1);
                            commandServer commandServer = httpServers.commandServer;
                            httpCommand command = default(httpCommand);
                            if (commandServer.httpCommands.Get(ref commandName, ref command))
                            {
                                if (request.Method == web.http.methodType.GET)
                                {
                                    if (request.ContentLength == 0 && !command.IsPostOnly)
                                    {
                                        tcpBase.httpPage page = typePool<tcpBase.httpPage>.Pop() ?? new tcpBase.httpPage();
                                        page.Set(socket, this, socketIdentity, request, null);
                                        ((webPage.page)page).Response = (response = response.Get());
                                        socket.TcpCommandSocket.HttpPage = page;
                                        commandServer.doHttpCommand(command.CommandIndex, socket);
                                        return;
                                    }
                                }
                                else if (request.PostType != http.requestHeader.postType.None && (uint)request.ContentLength <= command.MaxDataLength)
                                {
                                    httpLoadForm loadForm = typePool<httpLoadForm>.Pop() ?? new httpLoadForm();
                                    loadForm.Set(socket, this, socketIdentity, request, ref command);
                                    socket.GetForm(socketIdentity, loadForm);
                                    return;
                                }
                                socket.ResponseError(socketIdentity, http.response.state.MethodNotAllowed405);
                                return;
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, null, false);
                    }
                    finally { response.Push(ref response); }
                    socket.ResponseError(socketIdentity, http.response.state.NotFound404);
                }
                /// <summary>
                /// 创建错误输出数据
                /// </summary>
                protected override void createErrorResponse() { }
            }
            /// <summary>
            /// TCP调用服务端
            /// </summary>
            private commandServer commandServer;
            /// <summary>
            /// HTTP服务器
            /// </summary>
            /// <param name="commandServer">TCP调用服务端</param>
            public httpServers(commandServer commandServer)
            {
                this.commandServer = commandServer;
                LocalDomainServer = new server(this);
            }
            /// <summary>
            /// 获取HTTP转发代理服务客户端
            /// </summary>
            /// <returns>HTTP转发代理服务客户端,失败返回null</returns>
            internal override client GetForwardClient() { return null; }
        }
        /// <summary>
        /// HTTP表单数据加载处理
        /// </summary>
        private sealed class httpLoadForm : requestForm.ILoadForm
        {
            /// <summary>
            /// HTTP套接字
            /// </summary>
            private http.socketBase socket;
            /// <summary>
            /// HTTP服务
            /// </summary>
            private httpServers.server domainServer;
            /// <summary>
            /// 请求头部信息
            /// </summary>
            private http.requestHeader request;
            /// <summary>
            /// 套接字操作编号
            /// </summary>
            private long socketIdentity;
            /// <summary>
            /// HTTP命令处理委托
            /// </summary>
            private httpCommand command;
            /// <summary>
            /// 表单回调处理
            /// </summary>
            /// <param name="form">HTTP请求表单</param>
            public void OnGetForm(requestForm form)
            {
                http.socketBase socket = this.socket;
                response response = null;
                try
                {
                    if (form != null)
                    {
                        socketIdentity = form.Identity;
                        tcpBase.httpPage page = typePool<tcpBase.httpPage>.Pop() ?? new tcpBase.httpPage();
                        page.Set(socket, domainServer, socketIdentity, request, form);
                        ((webPage.page)page).Response = (response = response.Get());
                        socket.TcpCommandSocket.HttpPage = page;
                        domainServer.CommandServer.doHttpCommand(command.CommandIndex, socket);
                        return;
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                finally
                {
                    this.socket = null;
                    domainServer = null;
                    request = null;
                    typePool<httpLoadForm>.PushNotNull(this);
                    response.Push(ref response);
                }
                socket.ResponseError(socketIdentity, http.response.state.ServerError500);
            }
            /// <summary>
            /// 根据HTTP请求表单值获取内存流最大字节数
            /// </summary>
            /// <param name="value">HTTP请求表单值</param>
            /// <returns>内存流最大字节数</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public int MaxMemoryStreamSize(ref fastCSharp.net.tcp.http.requestForm.value value) { return 0; }
            /// <summary>
            /// 根据HTTP请求表单值获取保存文件全称
            /// </summary>
            /// <param name="value">HTTP请求表单值</param>
            /// <returns>文件全称</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public string GetSaveFileName(ref fastCSharp.net.tcp.http.requestForm.value value) { return null; }
            /// <summary>
            /// 获取HTTP请求表单数据加载处理委托
            /// </summary>
            /// <param name="socket">HTTP套接字</param>
            /// <param name="domainServer">HTTP服务</param>
            /// <param name="socketIdentity">套接字操作编号</param>
            /// <param name="request">请求头部信息</param>
            /// <param name="command">HTTP命令处理委托</param>
            /// <returns>HTTP请求表单数据加载处理委托</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void Set
                (http.socketBase socket, httpServers.server domainServer, long socketIdentity, http.requestHeader request, ref httpCommand command)
            {
                this.socket = socket;
                this.domainServer = domainServer;
                this.socketIdentity = socketIdentity;
                this.request = request;
                this.command = command;
            }
        }
        /// <summary>
        /// 资源释放异常
        /// </summary>
        private static readonly Exception objectDisposedException = new ObjectDisposedException("tcpStream");
        /// <summary>
        /// 错误支持异常
        /// </summary>
        private static readonly Exception notSupportedException = new NotSupportedException();
        /// <summary>
        /// 错误操作异常
        /// </summary>
        private static readonly Exception invalidOperationException = new InvalidOperationException();
        /// <summary>
        /// IO异常
        /// </summary>
        private static readonly Exception ioException = new IOException();
        /// <summary>
        /// 空参数异常
        /// </summary>
        private static readonly Exception argumentNullException = new ArgumentNullException();
        /// <summary>
        /// 参数超出范围异常
        /// </summary>
        private static readonly Exception argumentOutOfRangeException = new ArgumentOutOfRangeException();
        /// <summary>
        /// 参数异常
        /// </summary>
        private static readonly Exception argumentException = new ArgumentException();
        /// <summary>
        /// TCP调用套接字
        /// </summary>
        public sealed unsafe class socket : commandSocket<commandServer>
        {
            /// <summary>
            /// 异步回调
            /// </summary>
            public sealed class callback
            {
                /// <summary>
                /// 会话标识
                /// </summary>
                private streamIdentity identity;
                /// <summary>
                /// 异步套接字
                /// </summary>
                private socket socket;
                /// <summary>
                /// 异步回调
                /// </summary>
                private Func<fastCSharp.net.returnValue, bool> onReturnHandle;
                /// <summary>
                /// 套接字重用标识
                /// </summary>
                private int pushIdentity;
                /// <summary>
                /// 异步回调
                /// </summary>
                /// <param name="isKeep">是否保持回调</param>
                private callback(byte isKeep)
                {
                    if (isKeep == 0) onReturnHandle = onReturn;
                }
                /// <summary>
                /// 异步回调
                /// </summary>
                /// <param name="returnValue">返回值</param>
                /// <returns>是否成功加入回调队列</returns>
                private bool onReturn(fastCSharp.net.returnValue returnValue)
                {
                    if (this.socket.PushIdentity == pushIdentity)
                    {
                        socket socket = this.socket;
                        streamIdentity identity = this.identity;
                        this.socket = null;
                        typePool<callback>.PushNotNull(this);
                        return socket.SendStream(ref identity, returnValue.Type);
                    }
                    this.socket = null;
                    typePool<callback>.PushNotNull(this);
                    return false;
                }
                /// <summary>
                /// 异步回调
                /// </summary>
                /// <param name="returnValue">返回值</param>
                /// <returns>是否成功加入回调队列</returns>
                private bool onlyCallback(fastCSharp.net.returnValue returnValue)
                {
                    return this.socket.PushIdentity == pushIdentity && socket.SendStream(ref identity, returnValue.Type);
                }
                /// <summary>
                /// 异步回调
                /// </summary>
                /// <param name="socket"></param>
                /// <returns></returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                private Func<fastCSharp.net.returnValue, bool> set(socket socket)
                {
                    this.socket = socket;
                    identity = socket.identity;
                    pushIdentity = socket.PushIdentity;
                    return onReturnHandle;
                }
                /// <summary>
                /// 异步回调
                /// </summary>
                /// <param name="socket">异步套接字</param>
                /// <param name="isClientSendOnly"></param>
                /// <returns>异步回调</returns>
                public static Func<fastCSharp.net.returnValue, bool> Get(socket socket, byte isClientSendOnly = 0)
                {
                    callback value = typePool<callback>.Pop();
                    if (value == null)
                    {
                        try
                        {
                            value = new callback(0);
                        }
                        catch (Exception error)
                        {
                            log.Error.Add(error, null, false);
                            if (isClientSendOnly == 0) socket.SendStream(ref socket.identity, returnValue.type.ServerException);
                            return null;
                        }
                    }
                    return value.set(socket);
                }
                /// <summary>
                /// 异步回调
                /// </summary>
                /// <param name="socket"></param>
                /// <returns></returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                private Func<fastCSharp.net.returnValue, bool> setKeep(socket socket)
                {
                    this.socket = socket;
                    identity = socket.identity;
                    pushIdentity = socket.PushIdentity;
                    return onlyCallback;
                }
                /// <summary>
                /// 异步回调
                /// </summary>
                /// <param name="socket">异步套接字</param>
                /// <param name="isClientSendOnly"></param>
                /// <returns>异步回调</returns>
                public static Func<fastCSharp.net.returnValue, bool> GetKeep(socket socket, byte isClientSendOnly = 0)
                {
                    try
                    {
                        callback value = new callback(1);
                        return value.setKeep(socket);
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, null, false);
                    }
                    if (isClientSendOnly == 0) socket.SendStream(ref socket.identity, returnValue.type.ServerException);
                    return null;
                }
            }
            /// <summary>
            /// 异步回调
            /// </summary>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            /// <typeparam name="returnType">返回值类型</typeparam>
            public sealed class callback<outputParameterType, returnType>
#if NOJIT
                where outputParameterType : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                where outputParameterType : fastCSharp.net.asynchronousMethod.IReturnParameter<returnType>
#endif
            {
                /// <summary>
                /// 会话标识
                /// </summary>
                private streamIdentity identity;
                /// <summary>
                /// 异步套接字
                /// </summary>
                private socket socket;
                /// <summary>
                /// 异步回调
                /// </summary>
                private Func<fastCSharp.net.returnValue<returnType>, bool> onReturnHandle;
                /// <summary>
                /// 输出参数
                /// </summary>
                private outputParameterType outputParameter;
                /// <summary>
                /// 套接字重用标识
                /// </summary>
                private int pushIdentity;
                /// <summary>
                /// 当前命令参数
                /// </summary>
                private commandServer.commandFlags flags;
                /// <summary>
                /// 异步回调
                /// </summary>
                /// <param name="isKeep">是否保持回调</param>
                private callback(byte isKeep)
                {
                    if (isKeep == 0) onReturnHandle = onReturn;
                }
                /// <summary>
                /// 异步回调
                /// </summary>
                /// <param name="returnValue">返回值</param>
                /// <returns>是否成功加入回调队列</returns>
                private bool onReturn(fastCSharp.net.returnValue<returnType> returnValue)
                {
                    if (this.socket.PushIdentity == pushIdentity)
                    {
                        fastCSharp.net.returnValue<outputParameterType> outputParameter = new fastCSharp.net.returnValue<outputParameterType> { Type = returnValue.Type };
                        if (returnValue.Type == fastCSharp.net.returnValue.type.Success)
                        {
#if NOJIT
                            this.outputParameter.ReturnObject = returnValue.Value;
#else
                            this.outputParameter.Return = returnValue.Value;
#endif
                            outputParameter.Value = this.outputParameter;
                        }
                        socket socket = this.socket;
                        streamIdentity identity = this.identity;
                        this.outputParameter = default(outputParameterType);
                        this.socket = null;
                        typePool<callback<outputParameterType, returnType>>.PushNotNull(this);
                        return socket.SendStream(ref identity, ref outputParameter, flags);
                    }
                    this.outputParameter = default(outputParameterType);
                    this.socket = null;
                    typePool<callback<outputParameterType, returnType>>.PushNotNull(this);
                    return false;
                }
                /// <summary>
                /// 异步回调
                /// </summary>
                /// <param name="returnValue">返回值</param>
                /// <returns>是否成功加入回调队列</returns>
                private bool onlyCallback(fastCSharp.net.returnValue<returnType> returnValue)
                {
                    if (this.socket.PushIdentity == pushIdentity)
                    {
                        fastCSharp.net.returnValue<outputParameterType> outputParameter = new fastCSharp.net.returnValue<outputParameterType> { Type = returnValue.Type };
                        if (returnValue.Type == fastCSharp.net.returnValue.type.Success)
                        {
#if NOJIT
                            this.outputParameter.ReturnObject = returnValue.Value;
#else
                            this.outputParameter.Return = returnValue.Value;
#endif
                            outputParameter.Value = this.outputParameter;
                        }
                        return socket.SendStream(ref identity, ref outputParameter, flags);
                    }
                    return false;
                }
                /// <summary>
                /// 异步回调
                /// </summary>
                /// <param name="socket"></param>
                /// <param name="outputParameter"></param>
                /// <returns></returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                private Func<fastCSharp.net.returnValue<returnType>, bool> set(socket socket, ref outputParameterType outputParameter)
                {
                    this.socket = socket;
                    this.outputParameter = outputParameter;
                    identity = socket.identity;
                    flags = socket.flags;
                    pushIdentity = socket.PushIdentity;
                    return onReturnHandle;
                }
                /// <summary>
                /// 异步回调
                /// </summary>
                /// <param name="socket">异步套接字</param>
                /// <param name="outputParameter">输出参数</param>
                /// <param name="isClientSendOnly"></param>
                /// <returns>异步回调</returns>
                public static Func<fastCSharp.net.returnValue<returnType>, bool> Get(socket socket, ref outputParameterType outputParameter, byte isClientSendOnly = 0)
                {
                    callback<outputParameterType, returnType> value = typePool<callback<outputParameterType, returnType>>.Pop();
                    if (value == null)
                    {
                        try
                        {
                            value = new callback<outputParameterType, returnType>(0);
                        }
                        catch (Exception error)
                        {
                            log.Error.Add(error, null, false);
                            if (isClientSendOnly == 0) socket.SendStream(ref socket.identity, returnValue.type.ServerException);
                            return null;
                        }
                    }
                    return value.set(socket, ref outputParameter);
                }
                /// <summary>
                /// 异步回调
                /// </summary>
                /// <param name="socket"></param>
                /// <param name="outputParameter"></param>
                /// <returns></returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                private Func<fastCSharp.net.returnValue<returnType>, bool> setKeep(socket socket, ref outputParameterType outputParameter)
                {
                    this.socket = socket;
                    this.outputParameter = outputParameter;
                    identity = socket.identity;
                    flags = socket.flags;
                    pushIdentity = socket.PushIdentity;
                    return onlyCallback;
                }
                /// <summary>
                /// 异步回调
                /// </summary>
                /// <param name="socket">异步套接字</param>
                /// <param name="outputParameter">输出参数</param>
                /// <param name="isClientSendOnly"></param>
                /// <returns>异步回调</returns>
                public static Func<fastCSharp.net.returnValue<returnType>, bool> GetKeep(socket socket, ref outputParameterType outputParameter, byte isClientSendOnly = 0)
                {
                    try
                    {
                        callback<outputParameterType, returnType> value = new callback<outputParameterType, returnType>(1);
                        return value.setKeep(socket, ref outputParameter);
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, null, false);
                    }
                    if (isClientSendOnly == 0) socket.SendStream(ref socket.identity, returnValue.type.ServerException);
                    return null;
                }
            }
            /// <summary>
            /// 验证函数异步回调
            /// </summary>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            public sealed class callback<outputParameterType>
#if NOJIT
                where outputParameterType : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                where outputParameterType : fastCSharp.net.asynchronousMethod.IReturnParameter<bool>
#endif
            {
                /// <summary>
                /// 会话标识
                /// </summary>
                private streamIdentity identity;
                /// <summary>
                /// 异步套接字
                /// </summary>
                private socket socket;
                /// <summary>
                /// 异步回调
                /// </summary>
                private Func<fastCSharp.net.returnValue<bool>, bool> onReturnHandle;
                /// <summary>
                /// 输出参数
                /// </summary>
                private outputParameterType outputParameter;
                /// <summary>
                /// 当前命令参数
                /// </summary>
                private commandServer.commandFlags flags;
                /// <summary>
                /// 套接字重用标识
                /// </summary>
                private int pushIdentity;
                /// <summary>
                /// 异步回调
                /// </summary>
                private callback()
                {
                    onReturnHandle = onReturn;
                }
                /// <summary>
                /// 异步回调
                /// </summary>
                /// <param name="returnValue">返回值</param>
                /// <returns>是否成功加入回调队列</returns>
                private bool onReturn(fastCSharp.net.returnValue<bool> returnValue)
                {
                    if (this.socket.PushIdentity == pushIdentity)
                    {
                        fastCSharp.net.returnValue<outputParameterType> outputParameter = new fastCSharp.net.returnValue<outputParameterType>();
                        if (returnValue.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            try
                            {
                                if (returnValue.Value)
                                {
                                    outputParameter.Type = fastCSharp.net.returnValue.type.Success;
                                    this.socket.SetVerifyMethod();
#if NOJIT
                                    this.outputParameter.ReturnObject = returnValue.Value;
#else
                                    this.outputParameter.Return = returnValue.Value;
#endif
                                    outputParameter.Value = this.outputParameter;
                                }
                            }
                            catch (Exception error)
                            {
                                fastCSharp.log.Error.Add(error, null, true);
                            }
                        }
                        socket socket = this.socket;
                        streamIdentity identity = this.identity;
                        this.outputParameter = default(outputParameterType);
                        this.socket = null;
                        typePool<callback<outputParameterType>>.PushNotNull(this);
                        bool isReturn = socket.SendStream(ref identity, ref outputParameter, flags);
                        if (!this.socket.IsVerifyMethod) log.Default.Add("TCP调用客户端验证失败 " + this.socket.Socket.RemoteEndPoint.ToString(), new System.Diagnostics.StackFrame(), false);
                        return isReturn;
                    }
                    this.outputParameter = default(outputParameterType);
                    this.socket = null;
                    typePool<callback<outputParameterType>>.PushNotNull(this);
                    return false;
                }
                /// <summary>
                /// 异步回调
                /// </summary>
                /// <param name="socket"></param>
                /// <param name="outputParameter"></param>
                /// <returns></returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                private Func<fastCSharp.net.returnValue<bool>, bool> set(socket socket, ref outputParameterType outputParameter)
                {
                    this.socket = socket;
                    this.outputParameter = outputParameter;
                    identity = socket.identity;
                    flags = socket.flags;
                    pushIdentity = socket.PushIdentity;
                    return onReturnHandle;
                }
                /// <summary>
                /// 异步回调
                /// </summary>
                /// <param name="socket">异步套接字</param>
                /// <param name="outputParameter">输出参数</param>
                /// <param name="isClientSendOnly"></param>
                /// <returns>异步回调</returns>
                public static Func<fastCSharp.net.returnValue<bool>, bool> Get(socket socket, ref outputParameterType outputParameter, byte isClientSendOnly = 0)
                {
                    callback<outputParameterType> value = typePool<callback<outputParameterType>>.Pop();
                    if (value == null)
                    {
                        try
                        {
                            value = new callback<outputParameterType>();
                        }
                        catch (Exception error)
                        {
                            log.Error.Add(error, null, false);
                            if (isClientSendOnly == 0) socket.SendStream(ref socket.identity, returnValue.type.ServerException);
                            return null;
                        }
                    }
                    return value.set(socket, ref outputParameter);
                }
            }
            /// <summary>
            /// 异步回调
            /// </summary>
            public sealed class callbackHttp
            {
                /// <summary>
                /// HTTP页面
                /// </summary>
                private tcpBase.httpPage httpPage;
                /// <summary>
                /// 异步回调
                /// </summary>
                private Func<fastCSharp.net.returnValue, bool> onReturnHandle;
                /// <summary>
                /// 异步回调
                /// </summary>
                private callbackHttp()
                {
                    onReturnHandle = onReturn;
                }
                /// <summary>
                /// 异步回调
                /// </summary>
                /// <param name="returnValue">返回值</param>
                /// <returns>是否成功加入回调队列</returns>
                private bool onReturn(fastCSharp.net.returnValue returnValue)
                {
                    tcpBase.httpPage httpPage = this.httpPage;
                    this.httpPage = null;
                    bool isResponse = false;
                    try
                    {
                        typePool<callbackHttp>.PushNotNull(this);
                    }
                    finally
                    {
                        if (httpPage.Response(returnValue)) isResponse = true;
                    }
                    return isResponse;
                }
                /// <summary>
                /// 异步回调
                /// </summary>
                /// <param name="httpPage">HTTP页面</param>
                /// <returns>异步回调</returns>
                public static Func<fastCSharp.net.returnValue, bool> Get(tcpBase.httpPage httpPage)
                {
                    callbackHttp value = typePool<callbackHttp>.Pop();
                    if (value == null)
                    {
                        try
                        {
                            value = new callbackHttp();
                        }
                        catch (Exception error)
                        {
                            log.Error.Add(error, null, false);
                        }
                        if (value == null)
                        {
                            httpPage.Socket.ResponseError(httpPage.SocketIdentity, http.response.state.ServerError500);
                            return null;
                        }
                    }
                    value.httpPage = httpPage;
                    return value.onReturnHandle;
                }
            }
            /// <summary>
            /// 异步回调
            /// </summary>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            /// <typeparam name="returnType">返回值类型</typeparam>
            public sealed class callbackHttp<outputParameterType, returnType>
#if NOJIT
                where outputParameterType : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                where outputParameterType : fastCSharp.net.asynchronousMethod.IReturnParameter<returnType>
#endif
            {
                /// <summary>
                /// HTTP页面
                /// </summary>
                private tcpBase.httpPage httpPage;
                /// <summary>
                /// 输出参数
                /// </summary>
                private outputParameterType outputParameter;
                /// <summary>
                /// 异步回调
                /// </summary>
                private Func<fastCSharp.net.returnValue<returnType>, bool> onReturnHandle;
                /// <summary>
                /// 异步回调
                /// </summary>
                private callbackHttp()
                {
                    onReturnHandle = onReturn;
                }
                /// <summary>
                /// 异步回调
                /// </summary>
                /// <param name="returnValue">返回值</param>
                /// <returns>是否成功加入回调队列</returns>
                private bool onReturn(fastCSharp.net.returnValue<returnType> returnValue)
                {
                    fastCSharp.net.returnValue<outputParameterType> outputParameter = new fastCSharp.net.returnValue<outputParameterType> { Type = returnValue.Type };
                    if (returnValue.Type == fastCSharp.net.returnValue.type.Success)
                    {
#if NOJIT
                        this.outputParameter.ReturnObject = returnValue.Value;
#else
                        this.outputParameter.Return = returnValue.Value;
#endif
                        outputParameter.Value = this.outputParameter;
                    }
                    tcpBase.httpPage httpPage = this.httpPage;
                    this.outputParameter = default(outputParameterType);
                    this.httpPage = null;
                    bool isResponse = false;
                    try
                    {
                        typePool<callbackHttp<outputParameterType, returnType>>.PushNotNull(this);
                    }
                    finally
                    {
                        if (httpPage.Response(outputParameter)) isResponse = true;
                    }
                    return isResponse;
                }
                /// <summary>
                /// 设置输出参数
                /// </summary>
                /// <param name="httpPage"></param>
                /// <param name="outputParameter"></param>
                private void set(tcpBase.httpPage httpPage, ref outputParameterType outputParameter)
                {
                    this.httpPage = httpPage;
                    this.outputParameter = outputParameter;
                }
                /// <summary>
                /// 异步回调
                /// </summary>
                /// <param name="httpPage">HTTP页面</param>
                /// <param name="outputParameter">输出参数</param>
                /// <returns>异步回调</returns>
                public static Func<fastCSharp.net.returnValue<returnType>, bool> Get(tcpBase.httpPage httpPage, ref outputParameterType outputParameter)
                {
                    callbackHttp<outputParameterType, returnType> value = typePool<callbackHttp<outputParameterType, returnType>>.Pop();
                    if (value == null)
                    {
                        try
                        {
                            value = new callbackHttp<outputParameterType, returnType>();
                        }
                        catch (Exception error)
                        {
                            log.Error.Add(error, null, false);
                        }
                        if (value == null)
                        {
                            httpPage.Socket.ResponseError(httpPage.SocketIdentity, http.response.state.ServerError500);
                            return null;
                        }
                    }
                    value.set(httpPage, ref outputParameter);
                    return value.onReturnHandle;
                }
            }

            /// <summary>
            /// 套接字池下一个TCP调用套接字
            /// </summary>
            internal socket PoolNext;
            /// <summary>
            /// 套接字重用标识
            /// </summary>
            internal int PushIdentity;
            ///// <summary>
            ///// 是否已经设置流接收超时
            ///// </summary>
            //private int isStreamReceiveTimeout;
            /// <summary>
            /// 接收数据缓冲区起始位置
            /// </summary>
            private byte* receiveDataFixed;
            /// <summary>
            /// 接收数据结束位置
            /// </summary>
            private int receiveEndIndex;
            /// <summary>
            /// 接收数据起始位置
            /// </summary>
            private int receiveStartIndex;
            /// <summary>
            /// 验证超时
            /// </summary>
            private DateTime verifyTimeout;
            /// <summary>
            /// 默认HTTP内容编码
            /// </summary>
            internal override Encoding HttpEncoding
            {
                get { return commandSocketProxy.attribute.HttpEncoding; }
            }
            /// <summary>
            /// 客户端IP地址
            /// </summary>
            internal int Ipv4;
            /// <summary>
            /// 客户端IP地址
            /// </summary>
            internal ipv6Hash Ipv6;
            /// <summary>
            /// 当前处理命令
            /// </summary>
            private command command;
            /// <summary>
            /// 变换数据
            /// </summary>
            internal ulong MarkData;
            /// <summary>
            /// 验证时钟周期
            /// </summary>
            internal long VerifyTimeTicks;
            ///// <summary>
            ///// 是否输出调试信息
            ///// </summary>
            //private bool isOutputDebug;
            /// <summary>
            /// 初始化同步套接字
            /// </summary>
            /// <param name="socket"></param>
            /// <param name="sendData">发送数据缓冲区</param>
            /// <param name="receiveData">接收数据缓冲区</param>
            /// <param name="server">TCP调用服务</param>
            /// <param name="ip"></param>
            internal socket(Socket socket, byte[] sendData, byte[] receiveData, commandServer server, ref ipv6Hash ip)
                : base(socket, sendData, receiveData, server, false)
            {
                Ipv6 = ip;
                Ipv4 = 0;
                //isOutputDebug = server.attribute.IsOutputDebug;
            }
            /// <summary>
            /// 初始化同步套接字
            /// </summary>
            /// <param name="socket"></param>
            /// <param name="sendData">发送数据缓冲区</param>
            /// <param name="receiveData">接收数据缓冲区</param>
            /// <param name="server">TCP调用服务</param>
            /// <param name="ip"></param>
            internal socket(Socket socket, byte[] sendData, byte[] receiveData, commandServer server, int ip)
                : base(socket, sendData, receiveData, server, false)
            {
                Ipv4 = ip;
                //isOutputDebug = server.attribute.IsOutputDebug;
            }
            /// <summary>
            /// 重新设置套接字
            /// </summary>
            /// <param name="socket">客户端信息</param>
            /// <param name="ip"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void SetSocket(Socket socket, ref ipv6Hash ip)
            {
                Socket = socket;
                Ipv6 = ip;
                Ipv4 = 0;
                reset();
            }
            /// <summary>
            /// 重新设置套接字
            /// </summary>
            /// <param name="socket">客户端信息</param>
            /// <param name="ip"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void SetSocket(Socket socket, int ip)
            {
                Socket = socket;
                Ipv4 = ip;
                reset();
            }
            /// <summary>
            /// 重新设置套接字
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private void reset()
            {
                PoolNext = null;
                //IsVerifyMethod = false;
                //isStreamReceiveTimeout = 0;
                socketError = SocketError.Success;
                lastException = null;
                HttpPage = null;
                ClientUserInfo = null;
                MarkData = 0;
                LoadBalancingCheckIdentity = 0;
            }
            /// <summary>
            /// 保存套接字
            /// </summary>
            internal void Push()
            {
                Close();
                if (Ipv6.Ip == null)
                {
                    if ((Socket = commandSocketProxy.socketEnd(Ipv4)) == null)
                    {
                        commandSocketProxy.push(this);
                        return;
                    }
                }
                else if ((Socket = commandSocketProxy.socketEnd(ref Ipv6)) == null)
                {
                    Ipv6.Null();
                    commandSocketProxy.push(this);
                    return;
                }
                reset();
                VerifySocketType();
            }
            /// <summary>
            /// 关闭套接字连接
            /// </summary>
            protected override void dispose()
            {
                base.dispose();
                HttpPage = null;
                ClientUserInfo = null;
                Monitor.Enter(tcpStreamReceiveLock);
                try
                {
                    if (tcpStreamReceivers != null)
                    {
                        cancelTcpStream();
                        foreach (tcpStreamReceiver tcpStreamReceiver in tcpStreamReceivers)
                        {
                            if (tcpStreamReceiver.ReceiveWait != null)
                            {
                                tcpStreamReceiver.ReceiveWait.Set();
                                tcpStreamReceiver.ReceiveWait.Close();
                            }
                        }
                    }
                }
                finally { Monitor.Exit(tcpStreamReceiveLock); }
                interlocked.CompareSetYield(ref outputLock);
                outputs.Clear();
                outputLock = 0;
            }
            /// <summary>
            /// 取消TCP流
            /// </summary>
            private void cancelTcpStream()
            {
                while (tcpStreamReceiveIndex != 0) tcpStreamReceivers[--tcpStreamReceiveIndex].Cancel(true);
                freeTcpStreamIndexs.Empty();
            }
            /// <summary>
            /// 临时数据缓冲区
            /// </summary>
            private static readonly byte[] closeData = new byte[sizeof(commandServer.streamIdentity) + sizeof(int)];
            /// <summary>
            /// 通过验证方法
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void SetVerifyMethod()
            {
                if (!IsVerifyMethod)
                {
                    IsVerifyMethod = true;
                    if (commandSocketProxy.OnClientVerify != null) commandSocketProxy.OnClientVerify(this);
                }
            }

            static socket()
            {
                fixed (byte* dataFixed = closeData)
                {
                    (*(streamIdentity*)dataFixed).Set(commandClient.streamCommandSocket.CloseCallbackIndex);
                    *(int*)(dataFixed + sizeof(streamIdentity)) = 0;
                }
            }
            /// <summary>
            /// 关闭套接字
            /// </summary>
            protected override void close()
            {
                if (Socket != null)
                {
                    try
                    {
                        Monitor.Enter(tcpStreamReceiveLock);
                        try
                        {
                            cancelTcpStream();
                        }
                        finally { Monitor.Exit(tcpStreamReceiveLock); }
                        Socket.Send(closeData, 0, closeData.Length, SocketFlags.None, out socketError);
                    }
                    catch { }
                    finally { base.close(); }
                }
            }
            /// <summary>
            /// 关闭套接字
            /// </summary>
            internal void Close()
            {
                close();
                if (IsVerifyMethod)
                {
                    IsVerifyMethod = false;
                    if (commandSocketProxy.OnCloseVerifyClient != null) commandSocketProxy.OnCloseVerifyClient(this);
                }
                Monitor.Enter(isOutputBuilding);
                Monitor.Exit(isOutputBuilding);
            }
            /// <summary>
            /// TCP套接字添加到池
            /// </summary>
            internal override void PushPool()
            {
                Push();
            }
            /// <summary>
            /// 负载均衡联通测试标识
            /// </summary>
            internal int LoadBalancingCheckIdentity;
            /// <summary>
            /// 负载均衡联通测试
            /// </summary>
            /// <param name="identity">负载均衡联通测试标识</param>
            /// <returns>是否成功</returns>
            internal bool LoadBalancingCheck(int identity)
            {
                if (Socket != null && identity == LoadBalancingCheckIdentity)
                {
                    try
                    {
                        streamIdentity streamIdentity = new streamIdentity { Index = commandClient.streamCommandSocket.LoadBalancingCheckTimeCallbackIndex };
                        outputParameter output = outputParameter.Get(ref streamIdentity, returnValue.type.Success);
                        if (output != null) return pushOutput(output);
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, null, false);
                    }
                }
                return false;
            }
            /// <summary>
            /// 获取TCP调用客户端套接字类型
            /// </summary>
            internal void VerifySocketType()
            {
                int verifySeconds = commandSocketProxy.attribute.VerifySeconds;
                if (verifySeconds > 0) verifyTimeout = date.nowTime.Now.AddSeconds(verifySeconds + 1);
                else
                {
                    verifyTimeout = DateTime.MaxValue;
                    verifySeconds = config.tcpCommand.Default.DefaultTimeout;
                }
                if ((verifySeconds *= 1000) <= 0) verifySeconds = int.MaxValue;
                Socket.ReceiveTimeout = Socket.SendTimeout = verifySeconds;
                receive(this, receiver.type.ServerOnSocketType, 0, sizeof(int), verifyTimeout);
            }
            /// <summary>
            /// 获取TCP调用客户端套接字类型
            /// </summary>
            /// <param name="isSocket">是否成功</param>
            internal void OnSocketType(bool isSocket)
            {
                if (isSocket)
                {
                    fixed (byte* receiveDataFixed = receiveData)
                    {
                        if (*(int*)receiveDataFixed == (commandSocketProxy.attribute.IsIdentityCommand ? commandServer.IdentityVerifyIdentity : commandServer.VerifyIdentity))
                        {
                            if (commandSocketProxy.verify == null)
                            {
                                verifyMethod();
                                return;
                            }
                            try
                            {
                                if (commandSocketProxy.verify.Verify(this))
                                {
                                    verifyMethod();
                                    return;
                                }
                                log.Default.Add("TCP调用客户端验证失败 " + Socket.RemoteEndPoint.ToString(), new System.Diagnostics.StackFrame(), false);
                            }
                            catch (Exception error)
                            {
                                log.Error.Add(error, "TCP调用客户端验证失败 " + Socket.RemoteEndPoint.ToString(), false);
                            }
                        }
                        else if (commandSocketProxy.attribute.IsHttpClient && fastCSharp.web.http.GetMethod(receiveDataFixed) != web.http.methodType.Unknown)
                        {
                            http.socket.Start(this);
                            return;
                        }
                        else if (*(int*)receiveDataFixed == (commandSocketProxy.attribute.IsIdentityCommand ? commandServer.VerifyIdentity : commandServer.IdentityVerifyIdentity))
                        {
                            log.Error.Add("TCP调用客户端命令模式不匹配" + Socket.RemoteEndPoint.ToString(), new System.Diagnostics.StackFrame(), false);
                        }
                    }
                }
                Push();
            }
            /// <summary>
            /// 接收命令长度处理
            /// </summary>
            private tryReceiver.type onReceiveStreamCommandLengthType;
            /// <summary>
            /// 异步套接字方法验证
            /// </summary>
            private void verifyMethod()
            {
                if (commandSocketProxy.identityOnCommands == null ? commandSocketProxy.verifyCommand.Length == 0 : commandSocketProxy.verifyCommandIdentity == nullVerifyCommandIdentity) SetVerifyMethod();
                fixed (byte* dataFixed = sendData) *(int*)dataFixed = commandSocketProxy.attribute.IsIdentityCommand ? commandServer.IdentityVerifyIdentity : commandServer.VerifyIdentity;
                if (send(sendData, 0, sizeof(int)))
                {
                    if (commandSocketProxy.attribute.IsServerAsynchronousReceive)
                    {
                        if (onReceiveStreamCommandLengthType == tryReceiver.type.None)
                        {
                            if (commandSocketProxy.identityOnCommands == null)
                            {
                                onReceiveStreamCommandLengthType = tryReceiver.type.ServerOnReceiveStreamCommandLength;
                            }
                            else onReceiveStreamCommandLengthType = tryReceiver.type.ServerOnReceiveStreamIdentityCommand;
                        }
                        receiveEndIndex = receiveStartIndex = 0;
                        receiveStreamCommand();
                    }
                    else threadPool.TinyPool.FastStart(this, thread.callType.TcpCommandServerSocketReceiveCommand);
                    return;
                }
                Push();
            }
            /// <summary>
            /// 同步接收命令
            /// </summary>
            internal void ReceiveCommand()
            {
                try
                {
                    receiveStartIndex = 0;
                    fixed (byte* receiveDataFixed = receiveData)
                    {
                        this.receiveDataFixed = receiveDataFixed;
                        if (commandSocketProxy.identityOnCommands == null) receiveDataCommand();
                        else receiveIdentityCommand();
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                finally { Push(); }
            }
            /// <summary>
            /// 接收命令
            /// </summary>
            private void receiveIdentityCommand()
            {
                command[] commands = commandSocketProxy.identityOnCommands;
                if (commandSocketProxy.verifyCommandIdentity == nullVerifyCommandIdentity)
                {
                    receiveEndIndex = 0;
                    SetVerifyMethod();
                }
                else
                {
                    if ((receiveEndIndex = tryReceive(0, sizeof(int) * 3 + sizeof(commandServer.streamIdentity), verifyTimeout)) >= sizeof(int) * 3 + sizeof(commandServer.streamIdentity))
                    {
                        if (*(int*)receiveDataFixed == commandSocketProxy.verifyCommandIdentity)
                        {
                            command = commands[commandSocketProxy.verifyCommandIdentity];
                            setIdentityFlags(receiveDataFixed + sizeof(int));
                            receiveStartIndex = sizeof(int) * 3 + sizeof(commandServer.streamIdentity);
                            doCommand(*(int*)(receiveDataFixed + (sizeof(int) * 2 + sizeof(commandServer.streamIdentity))));
                        }
                        else log.Error.Add(null, "TCP验证函数命令匹配失败 " + (*(int*)receiveDataFixed).toString() + "<>" + commandSocketProxy.verifyCommandIdentity.toString(), false);
                    }
                    else log.Error.Add(null, "TCP验证函数命令数据接受失败 " + receiveEndIndex.toString() + "<" + (sizeof(int) * 3 + sizeof(commandServer.streamIdentity)).toString(), false);
                }
                if (IsVerifyMethod)
                {
                    Socket.ReceiveTimeout = commandSocketProxy.receiveCommandTimeout == 0 ? -1 : commandSocketProxy.receiveCommandTimeout;
                    while (tryReceiveIdentityCommand())
                    {
                        byte* start = receiveDataFixed + receiveStartIndex;
                        int commandIdentity = *(int*)start;
                        if ((uint)commandIdentity < commands.Length)
                        {
                            command = commands[commandIdentity];
                            setIdentityFlags(start + sizeof(int));
                            receiveStartIndex += sizeof(int) * 3 + sizeof(commandServer.streamIdentity);
                            if (doCommand(*(int*)(start + (sizeof(int) * 2 + sizeof(commandServer.streamIdentity))))) continue;
                        }
                        log.Default.Add(commandSocketProxy.attribute.ServiceName + " 缺少命令处理委托 [" + commandIdentity.toString() + "]", new System.Diagnostics.StackFrame(), false);
                        break;
                    }
                }
            }
            /// <summary>
            /// 接收命令
            /// </summary>
            /// <returns>是否成功</returns>
            private bool tryReceiveIdentityCommand()
            {
                int receiveLength = receiveEndIndex - receiveStartIndex;
                if (receiveLength >= sizeof(int) * 3 + sizeof(commandServer.streamIdentity)) return true;
                if (receiveLength != 0) Buffer.BlockCopy(receiveData, receiveStartIndex, receiveData, 0, receiveLength);
                receiveEndIndex = tryReceive(receiveLength, sizeof(int) * 3 + sizeof(commandServer.streamIdentity), date.NowSecond.AddTicks(commandSocketProxy.receiveCommandTicks));
                if (receiveEndIndex >= sizeof(int) * 3 + sizeof(commandServer.streamIdentity))
                {
                    receiveStartIndex = 0;
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 接收命令
            /// </summary>
            private void receiveDataCommand()
            {
                fastCSharp.stateSearcher.ascii<command> commands = commandSocketProxy.onCommands;
                if (commandSocketProxy.verifyCommand.Length == 0)
                {
                    receiveEndIndex = 0;
                    SetVerifyMethod();
                }
                else
                {
                    receiveStartIndex = commandSocketProxy.verifyCommand.Length + (sizeof(int) * 3 + sizeof(commandServer.streamIdentity));
                    if ((receiveEndIndex = tryReceive(0, receiveStartIndex, verifyTimeout)) >= receiveStartIndex && *(int*)receiveDataFixed == receiveStartIndex
                        && commandSocketProxy.verifyCommand.Equals(subArray<byte>.Unsafe(receiveData, sizeof(int), commandSocketProxy.verifyCommand.Length)))
                    {
                        byte* start = receiveDataFixed + receiveStartIndex;
                        command = commands.Get(commandSocketProxy.verifyCommand);
                        setIdentityFlags(start - (sizeof(int) * 2 + sizeof(commandServer.streamIdentity)));
                        doCommand(*(int*)(start - (sizeof(int))));
                    }
                    else log.Error.Add(null, "TCP验证函数命令匹配失败", false);
                }
                if (IsVerifyMethod)
                {
                    Socket.ReceiveTimeout = commandSocketProxy.receiveCommandTimeout == 0 ? -1 : commandSocketProxy.receiveCommandTimeout;
                    while (tryReceiveDataCommand())
                    {
                        byte* start = receiveDataFixed + receiveStartIndex;
                        int commandLength = *(int*)start;
                        if (commands.Get(subArray<byte>.Unsafe(receiveData, receiveStartIndex + sizeof(int), commandLength - (sizeof(int) * 3 + sizeof(commandServer.streamIdentity))), ref command))
                        {
                            start += commandLength;
                            receiveStartIndex += commandLength;
                            setIdentityFlags(start - (sizeof(int) * 2 + sizeof(commandServer.streamIdentity)));
                            if (doCommand(*(int*)(start - (sizeof(int))))) continue;
                        }
                        log.Default.Add(commandSocketProxy.attribute.ServiceName + " 缺少命令处理委托 " + subArray<byte>.Unsafe(receiveData, receiveStartIndex + sizeof(int), commandLength - (sizeof(int) * 3 + sizeof(commandServer.streamIdentity))).GetReverse().deSerialize(), new System.Diagnostics.StackFrame(), false);
                        break;
                    }
                }
            }
            /// <summary>
            /// 接收命令
            /// </summary>
            /// <returns>是否成功</returns>
            private bool tryReceiveDataCommand()
            {
                int receiveLength = receiveEndIndex - receiveStartIndex;
                if (receiveLength >= sizeof(int) * 4 + sizeof(commandServer.streamIdentity))
                {
                    int commandLength = *(int*)(receiveDataFixed + receiveStartIndex);
                    if (receiveLength >= commandLength) return true;
                    Buffer.BlockCopy(receiveData, receiveStartIndex, receiveData, 0, receiveLength);
                    receiveEndIndex = tryReceive(receiveLength, commandLength, date.nowTime.Now.AddTicks(commandSocketProxy.receiveCommandTicks));
                    if (receiveEndIndex >= commandLength)
                    {
                        receiveStartIndex = 0;
                        return true;
                    }
                }
                else
                {
                    if (receiveLength != 0) Buffer.BlockCopy(receiveData, receiveStartIndex, receiveData, 0, receiveLength);
                    receiveEndIndex = tryReceive(receiveLength, sizeof(int) * 4 + sizeof(commandServer.streamIdentity), date.nowTime.Now.AddTicks(commandSocketProxy.receiveCommandTicks));
                    if (receiveEndIndex >= sizeof(int) * 4 + sizeof(commandServer.streamIdentity))
                    {
                        int commandLength = *(int*)receiveDataFixed;
                        if (receiveEndIndex >= commandLength)
                        {
                            receiveStartIndex = 0;
                            return true;
                        }
                    }
                }
                return false;
            }
            /// <summary>
            /// 接收命令数据并执行命令
            /// </summary>
            /// <param name="length">数据长度</param>
            private bool doCommand(int length)
            {
                if (length == 0)
                {
                    if (command.MaxDataLength == 0)
                    {
                        command.DoCommand(this);
                        return true;
                    }
                }
                else
                {
                    int dataLength = length > 0 ? length : -length;
                    if (dataLength <= command.MaxDataLength)
                    {
                        int receiveLength = receiveEndIndex - receiveStartIndex;
                        if (dataLength <= receiveData.Length)
                        {
                            if (dataLength > receiveLength)
                            {
                                if (receiveLength != 0) Buffer.BlockCopy(receiveData, receiveStartIndex, receiveData, 0, receiveLength);
                                receiveEndIndex = tryReceive(receiveLength, dataLength, commandSocketProxy.getReceiveTimeout(dataLength));
                                if (receiveEndIndex < dataLength) return false;
                                receiveStartIndex = 0;
                            }
                            if (length >= 0)
                            {
                                subArray<byte> data = subArray<byte>.Unsafe(receiveData, receiveStartIndex, dataLength);
                                receiveStartIndex += dataLength;
                                command.DoCommandMark(this, ref data);
                            }
                            else
                            {
                                if (MarkData != 0) Mark(receiveData, MarkData, receiveStartIndex, dataLength);
                                subArray<byte> data = stream.Deflate.GetDeCompressUnsafe(receiveData, receiveStartIndex, dataLength, fastCSharp.memoryPool.StreamBuffers);
                                receiveStartIndex += dataLength;
                                command.DoCommand(this, ref data);
                                fastCSharp.memoryPool.StreamBuffers.PushOnly(data.array);
                            }
                            return true;
                        }
                        byte[] buffer = BigBuffers.Get(dataLength);
                        if (receiveLength != 0) Buffer.BlockCopy(receiveData, receiveStartIndex, buffer, 0, receiveLength);
                        if (receive(buffer, receiveLength, dataLength, commandSocketProxy.getReceiveTimeout(dataLength)))
                        {
                            if (length >= 0) command.DoCommandMark(this, subArray<byte>.Unsafe(buffer, 0, dataLength));
                            else
                            {
                                if (MarkData != 0) Mark(buffer, MarkData, dataLength);
                                subArray<byte> data = stream.Deflate.GetDeCompressUnsafe(buffer, 0, dataLength, fastCSharp.memoryPool.StreamBuffers);
                                command.DoCommand(this, ref data);
                                fastCSharp.memoryPool.StreamBuffers.PushOnly(data.array);
                            }
                            receiveStartIndex = receiveEndIndex = 0;
                            BigBuffers.PushNotNull(buffer);
                            return true;
                        }
                        BigBuffers.PushNotNull(buffer);
                    }
                    else
                    {
                        log.Default.Add("接收数据长度超限 " + (length > 0 ? length : -length).toString() + " > " + command.MaxDataLength.toString(), new System.Diagnostics.StackFrame(), false);
                    }
                }
                return false;
            }
            /// <summary>
            /// 接收命令
            /// </summary>
            private void receiveStreamCommand()
            {
                //if (IsVerifyMethod)
                //{
                //    if (Socket == null)
                //    {
                //        commandSocketProxy.pushSocket(this);
                //        return;
                //    }
                //    Socket.ReceiveTimeout = commandSocketProxy.receiveCommandTimeout == 0 ? -1 : commandSocketProxy.receiveCommandTimeout;
                //}
                try
                {
                NEXT:
                    int receiveLength = receiveEndIndex - receiveStartIndex;
                    if (commandSocketProxy.identityOnCommands == null)
                    {
                        if (receiveLength >= sizeof(int) * 4 + sizeof(commandServer.streamIdentity))
                        {
                            if (receiveStreamCommandLength()) goto NEXT;
                        }
                        else
                        {
                            if (receiveLength != 0) Buffer.BlockCopy(receiveData, receiveStartIndex, receiveData, 0, receiveLength);
                            tryReceive(onReceiveStreamCommandLengthType, receiveLength, (sizeof(int) * 4 + sizeof(commandServer.streamIdentity)) - receiveLength, IsVerifyMethod ? date.nowTime.Now.AddTicks(commandSocketProxy.receiveCommandTicks) : verifyTimeout);
                        }
                    }
                    else if (receiveLength >= sizeof(int) * 3 + sizeof(commandServer.streamIdentity))
                    {
                        if (receiveStreamIdentityCommand()) goto NEXT;
                    }
                    else
                    {
                        if (receiveLength != 0) Buffer.BlockCopy(receiveData, receiveStartIndex, receiveData, 0, receiveLength);
                        tryReceive(onReceiveStreamCommandLengthType, receiveLength, (sizeof(int) * 3 + sizeof(commandServer.streamIdentity)) - receiveLength, IsVerifyMethod ? date.nowTime.Now.AddTicks(commandSocketProxy.receiveCommandTicks) : verifyTimeout);
                    }
                    return;
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                Push();
            }
            /// <summary>
            /// 接收命令长度处理
            /// </summary>
            /// <param name="receiveEndIndex">接收数据结束位置</param>
            internal void OnReceiveStreamCommandLength(int receiveEndIndex)
            {
                try
                {
                    //if (isOutputDebug) DebugLog.Add(commandSocketProxy.attribute.ServiceName + ".onReceiveStreamCommandLength(" + receiveEndIndex.toString() + ")", false, false);
                    if (receiveEndIndex >= sizeof(int) * 4 + sizeof(commandServer.streamIdentity))
                    {
                        this.receiveEndIndex = receiveEndIndex;
                        receiveStartIndex = 0;
                        if (receiveStreamCommandLength()) receiveStreamCommand();
                        return;
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                Push();
            }
            /// <summary>
            /// 接收命令长度处理
            /// </summary>
            /// <returns>是否继续处理下一个命令</returns>
            private bool receiveStreamCommandLength()
            {
                fixed (byte* receiveDataFixed = receiveData)
                {
                    int commandLength = *(int*)(receiveDataFixed + receiveStartIndex);
                    if ((uint)commandLength <= commandSocketProxy.maxCommandLength)
                    {
                        int receiveLength = receiveEndIndex - receiveStartIndex;
                        if (receiveLength >= commandLength)
                        {
                            this.receiveDataFixed = receiveDataFixed;
                            return getStreamCommand();
                        }
                        if (receiveLength != 0) fastCSharp.unsafer.memory.Copy(receiveDataFixed + receiveStartIndex, receiveDataFixed, receiveLength);
                        tryReceive(tryReceiver.type.ServerOnReceiveStreamCommand, receiveLength, commandLength, IsVerifyMethod ? date.nowTime.Now.AddTicks(commandSocketProxy.receiveCommandTicks) : verifyTimeout);
                        return false;
                    }
                }
                Push();
                return false;
            }
            /// <summary>
            /// 接收命令处理
            /// </summary>
            /// <param name="receiveEndIndex">接收数据结束位置</param>
            internal void OnReceiveStreamCommand(int receiveEndIndex)
            {
                try
                {
                    //if (isOutputDebug) DebugLog.Add(commandSocketProxy.attribute.ServiceName + ".onReceiveStreamCommand(" + receiveEndIndex.toString() + ")", false, false);
                    fixed (byte* receiveDataFixed = receiveData)
                    {
                        if (receiveEndIndex >= *(int*)receiveDataFixed)
                        {
                            this.receiveEndIndex = receiveEndIndex;
                            this.receiveDataFixed = receiveDataFixed;
                            receiveStartIndex = 0;
                            if (getStreamCommand()) receiveStreamCommand();
                            return;
                        }
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                Push();
            }
            /// <summary>
            /// 获取命令委托
            /// </summary>
            /// <returns>是否继续处理下一个命令</returns>
            private bool getStreamCommand()
            {
                int commandLength = *(int*)(receiveDataFixed + receiveStartIndex);
                subArray<byte> commandData = subArray<byte>.Unsafe(receiveData, receiveStartIndex + sizeof(int), receiveStartIndex + commandLength - (sizeof(int) * 3 + sizeof(commandServer.streamIdentity)));
                if (IsVerifyMethod)
                {
                    if (commandSocketProxy.onCommands.Get(ref commandData, ref command))
                    {
                        receiveStartIndex += commandLength;
                        return getStreamIdentity();
                    }
                    log.Default.Add(commandSocketProxy.attribute.ServiceName + " 缺少命令处理委托 " + commandData.GetReverse().deSerialize(), null, false);
                }
                else if (commandSocketProxy.verifyCommand.Equals(commandData))
                {
                    command = commandSocketProxy.onCommands.Get(commandSocketProxy.verifyCommand);
                    receiveStartIndex += commandLength;
                    return getStreamIdentity();
                }
                Push();
                return false;
            }
            /// <summary>
            /// 获取会话标识
            /// </summary>
            /// <returns>是否继续处理下一个命令</returns>
            private bool getStreamIdentity()
            {
                byte* start = receiveDataFixed + receiveStartIndex;
                int length = *(int*)(start - sizeof(int));
                if (length == 0)
                {
                    if (command.MaxDataLength == 0)
                    {
                        setIdentityFlags(start - (sizeof(int) * 2 + sizeof(commandServer.streamIdentity)));
                        command.DoCommand(this);
                        //if ((receiveEndIndex -= receiveStartIndex) != 0) unsafer.memory.Copy(receiveDataFixed + receiveStartIndex, receiveDataFixed, receiveEndIndex);
                        return true;
                    }
                }
                else
                {
                    int dataLength = length > 0 ? length : -length;
                    if (dataLength <= command.MaxDataLength)
                    {
                        int receiveLength = receiveEndIndex - receiveStartIndex;
                        setIdentityFlags(start - (sizeof(int) * 2 + sizeof(commandServer.streamIdentity)));
                        if (dataLength <= receiveLength)
                        {
                            if (length >= 0)
                            {
                                subArray<byte> data = subArray<byte>.Unsafe(receiveData, receiveStartIndex, dataLength);
                                receiveStartIndex += dataLength;
                                command.DoCommandMark(this, ref data);
                                return true;
                            }
                            else
                            {
                                if (MarkData != 0) Mark(receiveData, MarkData, receiveStartIndex, dataLength);
                                memoryPool.pushSubArray pushData = new memoryPool.pushSubArray
                                {
                                    Value = stream.Deflate.GetDeCompressUnsafe(receiveData, receiveStartIndex, dataLength, fastCSharp.memoryPool.StreamBuffers), 
                                    PushPool = fastCSharp.memoryPool.StreamBuffers
                                };
                                receiveStartIndex += dataLength;
                                return isDoStreamCommand(ref pushData);
                            }
                        }
                        streamReceiver streamReceiver = streamReceiver.Pop();
                        if (streamReceiver == null)
                        {
                            memoryPool.pushSubArray pushData = default(memoryPool.pushSubArray);
                            doStreamCommand(ref pushData);
                        }
                        else streamReceiver.Receive(this, receiveDataFixed, length, IsVerifyMethod ? commandSocketProxy.getReceiveTimeout(dataLength) : verifyTimeout);
                        return false;
                    }
                    log.Default.Add("接收数据长度超限 " + (length > 0 ? length : -length).toString() + " > " + command.MaxDataLength.toString(), new System.Diagnostics.StackFrame(), false);
                }
                Push();
                return false;
            }
            /// <summary>
            /// 执行命令委托
            /// </summary>
            /// <param name="data">输出数据</param>
            private void doStreamCommand(ref memoryPool.pushSubArray data)
            {
                try
                {
                    if (isDoStreamCommand(ref data)) receiveStreamCommand();
                    return;
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                Push();
            }
            /// <summary>
            /// 执行命令委托
            /// </summary>
            /// <param name="data">输出数据</param>
            /// <returns>是否继续处理下一个命令</returns>
            private bool isDoStreamCommand(ref memoryPool.pushSubArray data)
            {
                byte[] buffer = data.Value.array;
                if (buffer != null)
                {
                    command.DoCommand(this, ref data.Value);
                    data.Push();
                    return true;
                }
                Push();
                return false;
            }
            /// <summary>
            /// 接收命令处理
            /// </summary>
            /// <param name="receiveEndIndex">接收数据结束位置</param>
            internal void OnReceiveStreamIdentityCommand(int receiveEndIndex)
            {
                try
                {
                    //if (isOutputDebug) DebugLog.Add(commandSocketProxy.attribute.ServiceName + ".onReceiveStreamIdentityCommand(" + receiveEndIndex.toString() + ")", false, false);
                    if (receiveEndIndex >= sizeof(int) * 3 + sizeof(commandServer.streamIdentity))
                    {
                        this.receiveEndIndex = receiveEndIndex;
                        receiveStartIndex = 0;
                        if (receiveStreamIdentityCommand()) receiveStreamCommand();
                        return;
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                Push();
            }
            /// <summary>
            /// 接收命令处理
            /// </summary>
            /// <returns>是否继续处理下一个命令</returns>
            private bool receiveStreamIdentityCommand()
            {
                fixed (byte* receiveDataFixed = receiveData)
                {
                    int command = *(int*)(receiveDataFixed + receiveStartIndex);
                    if (IsVerifyMethod)
                    {
                        if ((uint)command < commandSocketProxy.identityOnCommands.Length)
                        {
                            this.command = commandSocketProxy.identityOnCommands[command];
                            if (this.command.CommandIndex != 0)
                            {
                                this.receiveDataFixed = receiveDataFixed;
                                receiveStartIndex += sizeof(int) * 3 + sizeof(commandServer.streamIdentity);
                                return getStreamIdentity();
                            }
                        }
                        log.Default.Add(commandSocketProxy.attribute.ServiceName + " 缺少命令处理委托 [" + command.toString() + "]", null, false);
                    }
                    else if (command == commandSocketProxy.verifyCommandIdentity)
                    {
                        this.receiveDataFixed = receiveDataFixed;
                        this.command = commandSocketProxy.identityOnCommands[command];
                        receiveStartIndex += sizeof(int) * 3 + sizeof(commandServer.streamIdentity);
                        return getStreamIdentity();
                    }
                }
                Push();
                return false;
            }

            /// <summary>
            /// 数据读取器
            /// </summary>
            internal sealed class streamReceiver
            {
                /// <summary>
                /// TCP客户端套接字
                /// </summary>
                public socket Socket;
                /// <summary>
                /// 读取数据是否大缓存
                /// </summary>
                private bool isBigBuffer;
                /// <summary>
                /// 读取数据
                /// </summary>
                /// <param name="socket">套接字</param>
                /// <param name="dataFixed">接收数据起始位置</param>
                /// <param name="length">数据长度</param>
                /// <param name="timeout">接收超时</param>
                public unsafe void Receive(socket socket, byte* dataFixed, int length, DateTime timeout)
                {
                    Socket = socket;
                    isBigBuffer = false;
                    int dataLength = length >= 0 ? length : -length, receiveLength = socket.receiveEndIndex - socket.receiveStartIndex;
                    if (dataLength <= socket.receiveData.Length)
                    {
                        unsafer.memory.Copy(dataFixed + socket.receiveStartIndex, dataFixed, receiveLength);
                        socket.receiveStartIndex = socket.receiveEndIndex = 0;
                        if (length >= 0) socket.receive(this, receiver.type.ServerStreamReceiverOnReadData, receiveLength, dataLength - receiveLength, timeout);
                        else socket.receive(this, receiver.type.ServerStreamReceiverOnReadCompressData, receiveLength, dataLength - receiveLength, timeout);
                    }
                    else
                    {
                        byte[] data = BigBuffers.Get(dataLength);
                        isBigBuffer = true;
                        unsafer.memory.Copy(dataFixed + socket.receiveStartIndex, data, receiveLength);
                        socket.receiveStartIndex = socket.receiveEndIndex = 0;
                        if (length >= 0) socket.receive(this, receiver.type.ServerStreamReceiverOnReadData, data, receiveLength, dataLength - receiveLength, timeout);
                        else socket.receive(this, receiver.type.ServerStreamReceiverOnReadCompressData, data, receiveLength, dataLength - receiveLength, timeout);
                    }
                }
                /// <summary>
                /// 读取数据回调操作
                /// </summary>
                /// <param name="isSocket">是否操作成功</param>
                internal void OnReadData(bool isSocket)
                {
                    byte[] data = Socket.currentReceiveData;
                    Socket.currentReceiveData = Socket.receiveData;
                    if (isSocket)
                    {
                        memoryPool.pushSubArray pushData = default(memoryPool.pushSubArray);
                        pushData.Value.UnsafeSet(data, 0, Socket.currentReceiveEndIndex);
                        if (isBigBuffer) pushData.PushPool = BigBuffers;
                        if (Socket.MarkData != 0) Mark(data, Socket.MarkData, Socket.currentReceiveEndIndex);
                        push(ref pushData);
                    }
                    else
                    {
                        Socket socket = Socket.Socket;
                        try
                        {
                            memoryPool.pushSubArray pushData = default(memoryPool.pushSubArray);
                            if (isBigBuffer) BigBuffers.PushOnly(data);
                            push(ref pushData);
                        }
                        finally { socket.shutdown(); }
                    }
                }
                /// <summary>
                /// 读取数据回调操作
                /// </summary>
                /// <param name="isSocket">是否操作成功</param>
                internal void OnReadCompressData(bool isSocket)
                {
                    byte[] data = Socket.currentReceiveData;
                    Socket.currentReceiveData = Socket.receiveData;
                    if (isSocket) onReadCompressData(data);
                    else
                    {
                        try
                        {
                            memoryPool.pushSubArray pushData = default(memoryPool.pushSubArray);
                            if (isBigBuffer) BigBuffers.PushOnly(data);
                            push(ref pushData);
                        }
                        finally { Socket.close(); }
                    }
                }
                /// <summary>
                /// 读取数据回调操作
                /// </summary>
                private void onReadCompressData(byte[] data)
                {
                    try
                    {
                        if (Socket.MarkData != 0) Mark(data, Socket.MarkData, Socket.currentReceiveEndIndex);
                        memoryPool.pushSubArray pushData = new memoryPool.pushSubArray 
                        {
                            Value = stream.Deflate.GetDeCompressUnsafe(data, 0, Socket.currentReceiveEndIndex, fastCSharp.memoryPool.StreamBuffers), 
                            PushPool = fastCSharp.memoryPool.StreamBuffers
                        };
                        push(ref pushData);
                        return;
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, null, false);
                    }
                    finally
                    {
                        if (isBigBuffer) BigBuffers.PushOnly(data);
                    }
                    try
                    {
                        memoryPool.pushSubArray pushData = default(memoryPool.pushSubArray);
                        push(ref pushData);
                    }
                    finally { Socket.close(); }
                }
                /// <summary>
                /// 添加回调对象
                /// </summary>
                /// <param name="data">输出数据</param>
                private void push(ref memoryPool.pushSubArray data)
                {
                    socket socket = Socket;
                    Socket = null;
                    try
                    {
                        typePool<streamReceiver>.PushNotNull(this);
                    }
                    finally
                    {
                        try
                        {
                            socket.doStreamCommand(ref data);
                        }
                        catch (Exception error)
                        {
                            fastCSharp.log.Error.Add(error, null, false);
                        }
                    }
                }
                /// <summary>
                /// 数据读取器
                /// </summary>
                /// <returns></returns>
                public static streamReceiver Pop()
                {
                    streamReceiver streamReceiver = typePool<streamReceiver>.Pop();
                    if (streamReceiver == null)
                    {
                        try
                        {
                            streamReceiver = new streamReceiver();
                        }
                        catch (Exception error)
                        {
                            log.Error.Add(error, null, false);
                        }
                    }
                    return streamReceiver;
                }
            }

            /// <summary>
            /// 输出信息队列集合
            /// </summary>
            private struct outputQueue
            {
                /// <summary>
                /// 第一个节点
                /// </summary>
                public output Head;
                /// <summary>
                /// 最后一个节点
                /// </summary>
                public output End;
                /// <summary>
                /// 清除输出信息
                /// </summary>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Clear()
                {
                    Head = End = null;
                }
                /// <summary>
                /// 添加输出信息
                /// </summary>
                /// <param name="output"></param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Push(output output)
                {
                    if (Head == null) Head = End = output;
                    else
                    {
                        End.Next = output;
                        End = output;
                    }
                }
                /// <summary>
                /// 获取输出信息
                /// </summary>
                /// <returns></returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public output Pop()
                {
                    if (Head == null) return null;
                    output command = Head;
                    Head = Head.Next;
                    command.Next = null;
                    return command;
                }
            }
            /// <summary>
            /// 输出信息队列集合
            /// </summary>
            private outputQueue outputs;
            /// <summary>
            /// 输出数据输出缓冲区
            /// </summary>
            private readonly unmanagedStream outputStream = new unmanagedStream((byte*)fastCSharp.emit.pub.PuzzleValue, 1);
            /// <summary>
            /// 输出信息集合访问锁
            /// </summary>
            private int outputLock;
            /// <summary>
            /// 是否正在创建输出信息
            /// </summary>
            private readonly object isOutputBuilding = new object();
            /// <summary>
            /// 是否正在创建输出信息
            /// </summary>
            private byte isBuildOutput;
            /// <summary>
            /// 添加输出信息
            /// </summary>
            /// <param name="output">当前输出信息</param>
            /// <returns>是否成功加入输出队列</returns>
            private bool pushOutput(output output)
            {
                if (Socket != null)
                {
                    interlocked.CompareSetYield(ref outputLock);
                    byte isBuildOutput = this.isBuildOutput;
                    outputs.Push(output);
                    this.isBuildOutput = 1;
                    outputLock = 0;
                    if (isBuildOutput == 0) threadPool.TinyPool.FastStart(this, thread.callType.TcpCommandServerSocketBuildOutput);
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 创建输出数据并执行
            /// </summary>
            internal unsafe void BuildOutput()
            {
                TimeSpan sleepTime = new TimeSpan(0, 0, 0, 0, commandSocketProxy.attribute.ServerSendSleep);
                int bufferSize = BigBuffers.Size, bufferSize2 = bufferSize >> 1, maxCount = commandSocketProxy.attribute.MaxServerSendCount;
                outputBuilder outputBuilder = new outputBuilder { Socket = this, OutputStream = outputStream };
                Monitor.Enter(isOutputBuilding);
                try
                {
                START:
                    byte[] buffer = sendData;
                    fixed (byte* dataFixed = buffer)
                    {
                        outputBuilder.Reset(dataFixed, buffer.Length);
                        do
                        {
                            interlocked.CompareSetYield(ref outputLock);
                            output output = outputs.Pop();
                            if (output == null)
                            {
                                if (outputStream.Length <= sizeof(commandServer.streamIdentity) + sizeof(int))
                                {
                                    isBuildOutput = 0;
                                    outputLock = 0;
                                    outputStream.Dispose();
                                    Monitor.Exit(isOutputBuilding);
                                    return;
                                }
                                outputLock = 0;
                                outputBuilder.Send();
                                if (sendData != buffer) goto START;
                            }
                            else
                            {
                                outputLock = 0;
                                outputBuilder.Build(output);
                                if (outputBuilder.OutputCount == maxCount || outputStream.Length + outputBuilder.MaxOutputLength > bufferSize || !IsVerifyMethod)
                                {
                                    outputBuilder.Send();
                                    if (sendData != buffer) goto START;
                                }
                                if (outputs.Head == null && outputStream.Length <= bufferSize2) Thread.Sleep(sleepTime);
                            }
                        }
                        while (true);
                    }
                }
                catch (Exception error)
                {
                    interlocked.CompareSetYield(ref outputLock);
                    isBuildOutput = 0;
                    outputLock = 0;
                    outputStream.Dispose();
                    Monitor.Exit(isOutputBuilding);
                    Socket.shutdown();
                    log.Error.Add(error, commandSocketProxy.attribute.ServiceName, false);
                }
            }
            /// <summary>
            /// 输出创建
            /// </summary>
            private unsafe struct outputBuilder
            {
                /// <summary>
                /// TCP客户端输出流处理套接字
                /// </summary>
                public socket Socket;
                /// <summary>
                /// 输出数据流
                /// </summary>
                public unmanagedStream OutputStream;
                /// <summary>
                /// 输出流数据起始位置
                /// </summary>
                private byte* dataFixed;
                /// <summary>
                /// 输出数据
                /// </summary>
                private subArray<byte> data;
                /// <summary>
                /// 输出流字节长度
                /// </summary>
                private int bufferLength;
                /// <summary>
                /// 当前处理数量
                /// </summary>
                public int OutputCount;
                /// <summary>
                /// 最大输出长度
                /// </summary>
                public int MaxOutputLength;
                /// <summary>
                /// 重置输出流
                /// </summary>
                /// <param name="data">输出流数据起始位置</param>
                /// <param name="length">输出流字节长度</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Reset(byte* data, int length)
                {
                    OutputStream.UnsafeReset(dataFixed = data, bufferLength = length);
                    OutputStream.UnsafeSetLength(sizeof(commandServer.streamIdentity) + sizeof(int));
                    OutputCount = 0;
                }
                /// <summary>
                /// 创建输出流
                /// </summary>
                /// <param name="output">输出</param>
                public void Build(output output)
                {
                    int streamLength = OutputStream.Length;
                    output.Build(OutputStream, Socket.MarkData);
                    int outputLength = OutputStream.Length - streamLength;
                    ++OutputCount;
                    if (outputLength > MaxOutputLength) MaxOutputLength = outputLength;
                }
                /// <summary>
                /// 发送数据
                /// </summary>
                public void Send()
                {
                    memoryPool pushPool = null;
                    int outputLength = OutputStream.Length, dataLength = outputLength - (sizeof(commandServer.streamIdentity) + sizeof(int)), isNewBuffer = MaxOutputLength = 0;
                    if (OutputCount == 1)
                    {
                        if (outputLength <= bufferLength)
                        {
                            if (OutputStream.data.sizeValue != bufferLength)
                            {
                                unsafer.memory.Copy(OutputStream.data.Byte + (sizeof(commandServer.streamIdentity) + sizeof(int)), dataFixed + (sizeof(commandServer.streamIdentity) + sizeof(int)), dataLength);
                                OutputStream.UnsafeReset(dataFixed, bufferLength);
                            }
                            data.UnsafeSet(Socket.sendData, sizeof(commandServer.streamIdentity) + sizeof(int), dataLength);
                        }
                        else
                        {
                            byte[] newOutputBuffer = OutputStream.GetSizeArray(sizeof(commandServer.streamIdentity) + sizeof(int), bufferLength << 1);
                            fastCSharp.memoryPool.StreamBuffers.Push(ref Socket.sendData);
                            data.UnsafeSet(Socket.sendData = newOutputBuffer, sizeof(commandServer.streamIdentity) + sizeof(int), dataLength);
                            isNewBuffer = 1;
                        }
                        if (Socket.commandSocketProxy.attribute.IsCompress && dataLength > unmanagedStreamBase.DefaultLength)
                        {
                            subArray<byte> compressData = stream.Deflate.GetCompressUnsafe(data.array, (sizeof(commandServer.streamIdentity) + sizeof(int)) << 1, dataLength - (sizeof(commandServer.streamIdentity) + sizeof(int)), sizeof(commandServer.streamIdentity) + sizeof(int), fastCSharp.memoryPool.StreamBuffers);
                            if (compressData.array != null)
                            {
                                fixed (byte* compressFixed = compressData.array, sendFixed = data.array)
                                {
                                    *(commandServer.streamIdentity*)compressFixed = *(commandServer.streamIdentity*)(sendFixed + (sizeof(commandServer.streamIdentity) + sizeof(int)));
                                    *(int*)(compressFixed + sizeof(commandServer.streamIdentity)) = -compressData.length;
                                }
                                data.UnsafeSet(compressData.array, 0, compressData.length + (sizeof(commandServer.streamIdentity) + sizeof(int)));
                                pushPool = fastCSharp.memoryPool.StreamBuffers;
                            }
                        }
                    }
                    else
                    {
                        if (outputLength > bufferLength)
                        {
                            byte[] newCommandBuffer = OutputStream.GetSizeArray(sizeof(commandServer.streamIdentity) + sizeof(int), bufferLength << 1);
                            fastCSharp.memoryPool.StreamBuffers.Push(ref Socket.sendData);
                            data.UnsafeSet(Socket.sendData = newCommandBuffer, 0, outputLength);
                            isNewBuffer = 1;
                        }
                        else
                        {
                            if (OutputStream.data.sizeValue != bufferLength)
                            {
                                unsafer.memory.Copy(OutputStream.data.Byte + (sizeof(commandServer.streamIdentity) + sizeof(int)), dataFixed + (sizeof(commandServer.streamIdentity) + sizeof(int)), dataLength);
                                OutputStream.UnsafeReset(dataFixed, bufferLength);
                            }
                            data.UnsafeSet(Socket.sendData, 0, outputLength);
                        }
                        if (Socket.commandSocketProxy.attribute.IsCompress && dataLength > unmanagedStreamBase.DefaultLength)
                        {
                            subArray<byte> compressData = stream.Deflate.GetCompressUnsafe(data.array, sizeof(commandServer.streamIdentity) + sizeof(int), dataLength, sizeof(commandServer.streamIdentity) + sizeof(int), fastCSharp.memoryPool.StreamBuffers);
                            if (compressData.array != null)
                            {
                                dataLength = -compressData.length;
                                data.UnsafeSet(compressData.array, 0, compressData.length + (sizeof(commandServer.streamIdentity) + sizeof(int)));
                                pushPool = fastCSharp.memoryPool.StreamBuffers;
                            }
                        }
                        fixed (byte* megerDataFixed = data.array)
                        {
                            (*(commandServer.streamIdentity*)megerDataFixed).Set(commandClient.streamCommandSocket.MergeCallbackIndex);
                            *(int*)(megerDataFixed + sizeof(commandServer.streamIdentity)) = dataLength;
                        }
                    }
                    if (Socket.MarkData != 0)
                    {
                        fixed (byte* dataFixed = data.array)
                        {
                            byte* start = dataFixed + (data.startIndex + sizeof(commandServer.streamIdentity));
                            if (*(int*)start != 0)
                            {
                                commandServer.mark32(start + sizeof(int), Socket.MarkData, (data.length - (sizeof(commandServer.streamIdentity) + sizeof(int) - 3)) & (int.MaxValue - 3));
                            }
                        }
                    }
                    if (isNewBuffer == 0) OutputStream.UnsafeSetLength(sizeof(commandServer.streamIdentity) + sizeof(int));
                    try
                    {
                        OutputCount = 0;
                        //if (Socket.isOutputDebug) DebugLog.Add(Socket.commandSocketProxy.attribute.ServiceName + ".Send(" + data.Length.toString() + ")", false, false);
                        Socket.serverSend(ref data);
                    }
                    finally
                    {
                        if (pushPool != null) pushPool.Push(ref data.array);
                    }
                }
            }

            /// <summary>
            /// 输出信息
            /// </summary>
            private abstract class output
            {
                /// <summary>
                /// 下一个输出信息
                /// </summary>
                public output Next;
                /// <summary>
                /// 会话标识
                /// </summary>
                public commandServer.streamIdentity Identity;
                /// <summary>
                /// 创建输出信息
                /// </summary>
                /// <param name="stream">命令内存流</param>
                /// <param name="markData">变换数据</param>
                public abstract void Build(unmanagedStream stream, ulong markData);
            }
            /// <summary>
            /// 输出信息
            /// </summary>
            private sealed class outputParameter : output
            {
                /// <summary>
                /// 输出参数
                /// </summary>
                public fastCSharp.net.returnValue OutputParameter;
                /// <summary>
                /// 创建输出信息
                /// </summary>
                /// <param name="stream">命令内存流</param>
                /// <param name="markData">变换数据</param>
                public override void Build(unmanagedStream stream, ulong markData)
                {
                    stream.PrepLength(sizeof(commandServer.streamIdentity) + sizeof(int) + sizeof(int));
                    byte* dataFixed = stream.CurrentData;
                    *(commandServer.streamIdentity*)dataFixed = Identity;
                    *(int*)(dataFixed + sizeof(commandServer.streamIdentity)) = 0;
                    if (markData == 0) *(int*)(dataFixed + sizeof(commandServer.streamIdentity) + sizeof(int)) = (int)(byte)OutputParameter.Type;
                    else *(uint*)(dataFixed + sizeof(commandServer.streamIdentity) + sizeof(int)) = ((uint)fastCSharp.random.Default.Next() << 8) | (byte)OutputParameter.Type;
                    stream.UnsafeAddLength(sizeof(commandServer.streamIdentity) + sizeof(int) + sizeof(int));
                    Next = null;
                    typePool<outputParameter>.PushNotNull(this);
                }
                /// <summary>
                /// 获取输出信息
                /// </summary>
                /// <param name="identity">会话标识</param>
                /// <param name="outputParameter">输出参数</param>
                /// <returns>输出信息</returns>
                public static outputParameter Get(ref commandServer.streamIdentity identity,
                    fastCSharp.net.returnValue.type outputParameter)
                {
                    outputParameter output = typePool<outputParameter>.Pop();
                    if (output == null)
                    {
                        try
                        {
                            output = new outputParameter();
                        }
                        catch (Exception error)
                        {
                            log.Error.Add(error, null, false);
                            return null;
                        }
                    }
                    output.Identity = identity;
                    output.OutputParameter.Type = outputParameter;
                    return output;
                }
            }
            /// <summary>
            /// 发送数据
            /// </summary>
            /// <param name="identity">会话标识</param>
            /// <param name="value">返回值</param>
            /// <returns>是否成功加入输出队列</returns>
            public bool SendStream(commandServer.streamIdentity identity, fastCSharp.net.returnValue.type value)
            {
                outputParameter output = outputParameter.Get(ref identity, value);
                if (output != null) return pushOutput(output);
                close();
                return false;
            }
            /// <summary>
            /// 发送数据
            /// </summary>
            /// <param name="identity">会话标识</param>
            /// <param name="value">返回值</param>
            /// <returns>是否成功加入输出队列</returns>
            public bool SendStream(ref commandServer.streamIdentity identity, fastCSharp.net.returnValue.type value)
            {
                outputParameter output = outputParameter.Get(ref identity, value);
                if (output != null) return pushOutput(output);
                close();
                return false;
            }
            /// <summary>
            /// 发送数据
            /// </summary>
            /// <param name="identity">会话标识</param>
            /// <param name="value">返回值</param>
            /// <returns>是否成功加入输出队列</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public bool SendStream(ref commandServer.streamIdentity identity, ref fastCSharp.net.returnValue value)
            {
                return SendStream(ref identity, value.Type);
            }
            /// <summary>
            /// 发送数据
            /// </summary>
            /// <param name="identity">会话标识</param>
            /// <param name="value">返回值</param>
            /// <param name="flags">命令参数</param>
            /// <returns>是否成功加入输出队列</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public bool SendStream(ref commandServer.streamIdentity identity, ref fastCSharp.net.returnValue value, commandServer.commandFlags flags)
            {
                return SendStream(ref identity, value.Type);
            }
            /// <summary>
            /// 输出信息
            /// </summary>
            /// <typeparam name="outputParameterType">输出数据类型</typeparam>
            private sealed class outputParameter<outputParameterType> : output
            {
                /// <summary>
                /// 输出参数
                /// </summary>
                public outputParameterType OutputParameter;
                /// <summary>
                /// 创建输出信息
                /// </summary>
                /// <param name="stream">命令内存流</param>
                /// <param name="markData">变换数据</param>
                public override void Build(unmanagedStream stream, ulong markData)
                {
                    int streamLength = stream.Length;
                    stream.PrepLength(sizeof(commandServer.streamIdentity) + sizeof(int));
                    stream.UnsafeAddLength(sizeof(commandServer.streamIdentity) + sizeof(int));
                    fastCSharp.emit.dataSerializer.Serialize(OutputParameter, stream);
                    int dataLength = stream.Length - streamLength - (sizeof(commandServer.streamIdentity) + sizeof(int));
                    byte* dataFixed = stream.data.Byte + streamLength;
                    *(commandServer.streamIdentity*)dataFixed = Identity;
                    *(int*)(dataFixed + sizeof(commandServer.streamIdentity)) = dataLength;
                    OutputParameter = default(outputParameterType);
                    Next = null;
                    typePool<outputParameter<outputParameterType>>.PushNotNull(this);
                }
                /// <summary>
                /// 设置输出参数
                /// </summary>
                /// <param name="identity"></param>
                /// <param name="outputParameter"></param>
                private void set(ref commandServer.streamIdentity identity, ref outputParameterType outputParameter)
                {
                    Identity = identity;
                    OutputParameter = outputParameter;
                }
                /// <summary>
                /// 获取输出信息
                /// </summary>
                /// <param name="identity">会话标识</param>
                /// <param name="outputParameter">输出参数</param>
                /// <returns>输出信息</returns>
                public static outputParameter<outputParameterType> Get
                    (ref commandServer.streamIdentity identity, ref outputParameterType outputParameter)
                {
                    outputParameter<outputParameterType> output = typePool<outputParameter<outputParameterType>>.Pop();
                    if (output == null)
                    {
                        try
                        {
                            output = new outputParameter<outputParameterType>();
                        }
                        catch (Exception error)
                        {
                            log.Error.Add(error, null, false);
                            return null;
                        }
                    }
                    output.set(ref identity, ref outputParameter);
                    return output;
                }
            }
            ///// <summary>
            ///// 发送数据
            ///// </summary>
            ///// <typeparam name="outputParameterType">输出数据类型</typeparam>
            ///// <param name="identity">会话标识</param>
            ///// <param name="flags">命令参数</param>
            ///// <param name="outputParameter">返回值</param>
            ///// <returns>是否成功加入输出队列</returns>
            //[MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            //public bool SendStream<outputParameterType>(ref commandServer.streamIdentity identity, commandServer.commandFlags flags, fastCSharp.net.returnValue<outputParameterType> outputParameter)
            //{
            //    return SendStream(ref identity, flags, ref outputParameter);
            //}
            /// <summary>
            /// 发送数据
            /// </summary>
            /// <typeparam name="outputParameterType">输出数据类型</typeparam>
            /// <param name="identity">会话标识</param>
            /// <param name="flags">命令参数</param>
            /// <param name="outputParameter">返回值</param>
            /// <returns>是否成功加入输出队列</returns>
            public bool SendStream<outputParameterType>(ref commandServer.streamIdentity identity, ref fastCSharp.net.returnValue<outputParameterType> outputParameter, commandServer.commandFlags flags)
            {
                if (outputParameter.Type == returnValue.type.Success)
                {
                    if ((flags & commandFlags.JsonSerialize) == 0)
                    {
                        outputParameter<outputParameterType> output = outputParameter<outputParameterType>.Get(ref identity, ref outputParameter.Value);
                        if (output != null) return pushOutput(output);
                    }
                    else
                    {
                        tcpBase.parameterJsonToSerialize<outputParameterType> jsonParameter = new tcpBase.parameterJsonToSerialize<outputParameterType> { Return = outputParameter.Value };
                        outputParameter<tcpBase.parameterJsonToSerialize<outputParameterType>> output = outputParameter<tcpBase.parameterJsonToSerialize<outputParameterType>>.Get(ref identity, ref jsonParameter);
                        if (output != null) return pushOutput(output);
                    }
                    close();
                    return false;
                }
                return SendStream(ref identity, outputParameter.Type);
            }

            ///// <summary>
            ///// 设置流接收超时
            ///// </summary>
            //private void setStreamReceiveTimeout()
            //{
            //    if (IsVerifyMethod && isStreamReceiveTimeout == 0)
            //    {
            //        isStreamReceiveTimeout = 1;
            //        Socket.ReceiveTimeout = commandSocketProxy.receiveCommandTimeout == 0 ? -1 : commandSocketProxy.receiveCommandTimeout;
            //    }
            //}
            /// <summary>
            /// TCP参数流
            /// </summary>
            private class tcpStream : Stream, ITcpStreamCallback
            {
                /// <summary>
                /// 默认空TCP流异步操作状态
                /// </summary>
                private static readonly tcpStreamAsyncResult nullTcpStreamAsyncResult = new tcpStreamAsyncResult { Parameter = new tcpStreamParameter { Data = subArray<byte>.Unsafe(nullValue<byte>.Array, 0, 0) }, IsCompleted = true };
                /// <summary>
                /// TCP调用套接字
                /// </summary>
                private socket socket;
                /// <summary>
                /// TCP参数流
                /// </summary>
                private tcpBase.tcpStream stream;
                /// <summary>
                /// 套接字重用标识
                /// </summary>
                private int pushIdentity;
                /// <summary>
                /// 是否已经释放资源
                /// </summary>
                private int isDisposed;
                /// <summary>
                /// TCP参数流
                /// </summary>
                /// <param name="socket">TCP调用套接字</param>
                /// <param name="stream">TCP参数流</param>
                public tcpStream(socket socket, ref tcpBase.tcpStream stream)
                {
                    this.socket = socket;
                    this.stream = stream;
                    pushIdentity = socket.PushIdentity;
                }
                /// <summary>
                /// 发送命令获取客户端回馈
                /// </summary>
                /// <param name="parameter">TCP流参数</param>
                /// <returns>客户端回馈</returns>
                private tcpStreamParameter get(tcpStreamParameter parameter)
                {
                    if (pushIdentity == socket.PushIdentity)
                    {
                        parameter.Index = socket.getTcpStreamIndex(null);
                        parameter.Identity = socket.tcpStreamReceivers[parameter.Index].Identity;
                        parameter.ClientIndex = stream.ClientIndex;
                        parameter.ClientIdentity = stream.ClientIdentity;
                        try
                        {
                            commandServer.streamIdentity identity = new commandServer.streamIdentity { Index = commandClient.streamCommandSocket.TcpStreamCallbackIndex };
                            fastCSharp.net.returnValue<tcpStreamParameter> output = new fastCSharp.net.returnValue<tcpStreamParameter> { Type = returnValue.type.Success, Value = parameter };
                            socket.SendStream(ref identity, ref output, default(commandFlags));
                            tcpStreamParameter outputParameter = socket.waitTcpStream(parameter.Index, parameter.Identity);
                            if (outputParameter.IsClientStream) return outputParameter;
                            error();
                        }
                        finally
                        {
                            socket.cancelTcpStreamIndex(parameter.Index, parameter.Identity, false);
                        }
                    }
                    return tcpStreamParameter.Null;
                }
                /// <summary>
                /// 发送异步命令
                /// </summary>
                /// <param name="tcpStreamAsyncResult">TCP流异步操作状态</param>
                private void send(tcpStreamAsyncResult tcpStreamAsyncResult)
                {
                    if (pushIdentity == socket.PushIdentity)
                    {
                        tcpStreamParameter parameter = tcpStreamAsyncResult.Parameter;
                        parameter.Index = socket.getTcpStreamIndex(tcpStreamAsyncResult);
                        parameter.Identity = socket.tcpStreamReceivers[parameter.Index].Identity;
                        parameter.ClientIndex = stream.ClientIndex;
                        parameter.ClientIdentity = stream.ClientIdentity;
                        try
                        {
                            commandServer.streamIdentity identity = new commandServer.streamIdentity { Index = commandClient.streamCommandSocket.TcpStreamCallbackIndex };
                            fastCSharp.net.returnValue<tcpStreamParameter> output = new fastCSharp.net.returnValue<tcpStreamParameter> { Type = returnValue.type.Success, Value = parameter };
                            socket.SendStream(ref identity, ref output, default(commandFlags));
                            socket.setTcpStreamTimeout(parameter.Index, parameter.Identity);
                            return;
                        }
                        catch (Exception error)
                        {
                            log.Error.Add(error, null, false);
                        }
                        socket.cancelTcpStreamIndex(parameter.Index, parameter.Identity, false);
                    }
                    throw ioException;
                }
                /// <summary>
                /// TCP流异步回调
                /// </summary>
                /// <param name="tcpStreamAsyncResult">TCP流异步操作状态</param>
                /// <param name="parameter">TCP流参数</param>
                public void Callback(tcpStreamAsyncResult tcpStreamAsyncResult, tcpStreamParameter parameter)
                {
                    if (parameter != null && parameter.IsClientStream)
                    {
                        if (parameter.IsCommand)
                        {
                            switch (tcpStreamAsyncResult.Parameter.Command)
                            {
                                case tcpStreamCommand.BeginRead:
                                    subArray<byte> data = parameter.Data;
                                    if (data.length != 0)
                                    {
                                        subArray<byte> buffer = tcpStreamAsyncResult.Parameter.Data;
                                        Buffer.BlockCopy(data.array, data.startIndex, buffer.array, buffer.startIndex, data.length);
                                    }
                                    tcpStreamAsyncResult.Parameter.Offset = data.length;
                                    break;
                                case tcpStreamCommand.BeginWrite:
                                    tcpStreamAsyncResult.IsCompleted = true;
                                    break;
                            }
                        }
                    }
                    else Close();
                }
                /// <summary>
                /// 否支持读取
                /// </summary>
                public override bool CanRead
                {
                    get { return stream.CanRead; }
                }
                /// <summary>
                /// 否支持查找
                /// </summary>
                public override bool CanSeek
                {
                    get { return stream.CanSeek; }
                }
                /// <summary>
                /// 是否可以超时
                /// </summary>
                public override bool CanTimeout
                {
                    get { return stream.CanTimeout; }
                }
                /// <summary>
                /// 否支持写入
                /// </summary>
                public override bool CanWrite
                {
                    get { return stream.CanWrite; }
                }
                /// <summary>
                /// 流字节长度
                /// </summary>
                public override long Length
                {
                    get
                    {
                        if (isDisposed != 0) throw objectDisposedException;
                        tcpStreamParameter parameter = get(new tcpStreamParameter { Command = tcpStreamCommand.GetLength });
                        if (parameter.IsCommand) return parameter.Offset;
                        throw notSupportedException;
                    }
                }
                /// <summary>
                /// 当前位置
                /// </summary>
                public override long Position
                {
                    get
                    {
                        if (isDisposed != 0) throw objectDisposedException;
                        tcpStreamParameter parameter = get(new tcpStreamParameter { Command = tcpStreamCommand.GetPosition });
                        if (parameter.IsCommand) return parameter.Offset;
                        throw notSupportedException;
                    }
                    set
                    {
                        if (isDisposed != 0) throw objectDisposedException;
                        tcpStreamParameter parameter = get(new tcpStreamParameter { Command = tcpStreamCommand.SetPosition, Offset = value });
                        if (parameter.IsCommand) return;
                        throw notSupportedException;
                    }
                }
                /// <summary>
                /// 读超时毫秒
                /// </summary>
                public override int ReadTimeout
                {
                    get
                    {
                        if (isDisposed == 0)
                        {
                            tcpStreamParameter parameter = get(new tcpStreamParameter { Command = tcpStreamCommand.GetReadTimeout });
                            if (parameter.IsCommand) return (int)parameter.Offset;
                        }
                        throw invalidOperationException;
                    }
                    set
                    {
                        if (isDisposed == 0)
                        {
                            tcpStreamParameter parameter = get(new tcpStreamParameter { Command = tcpStreamCommand.SetReadTimeout, Offset = value });
                            if (parameter.IsCommand) return;
                        }
                        throw invalidOperationException;
                    }
                }
                /// <summary>
                /// 写超时毫秒
                /// </summary>
                public override int WriteTimeout
                {
                    get
                    {
                        if (isDisposed == 0)
                        {
                            tcpStreamParameter parameter = get(new tcpStreamParameter { Command = tcpStreamCommand.GetWriteTimeout });
                            if (parameter.IsCommand) return (int)parameter.Offset;
                        }
                        throw invalidOperationException;
                    }
                    set
                    {
                        if (isDisposed == 0)
                        {
                            tcpStreamParameter parameter = get(new tcpStreamParameter { Command = tcpStreamCommand.SetWriteTimeout, Offset = value });
                            if (parameter.IsCommand) return;
                        }
                        throw invalidOperationException;
                    }
                }
                /// <summary>
                /// 异步读取
                /// </summary>
                /// <param name="buffer">缓冲区</param>
                /// <param name="offset">起始位置</param>
                /// <param name="count">接收字节数</param>
                /// <param name="callback">异步回调</param>
                /// <param name="state">用户对象</param>
                /// <returns>异步读取结果</returns>
                public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
                {
                    if (isDisposed != 0) throw objectDisposedException;
                    if (!stream.CanRead) throw notSupportedException;
                    if (buffer == null || offset < 0 || count < 0 || offset + count > buffer.Length) throw argumentException;
                    if (count != 0)
                    {
                        tcpStreamAsyncResult result = new tcpStreamAsyncResult { TcpStreamCallback = this, Parameter = new tcpStreamParameter { Command = tcpStreamCommand.BeginRead, Data = subArray<byte>.Unsafe(buffer, offset, count) }, Callback = callback, AsyncState = state };
                        send(result);
                        return result;
                    }
                    callback(nullTcpStreamAsyncResult);
                    return nullTcpStreamAsyncResult;
                }
                /// <summary>
                /// 等待挂起的异步读取完成
                /// </summary>
                /// <param name="asyncResult">异步读取结果</param>
                /// <returns>读取的字节数</returns>
                public override int EndRead(IAsyncResult asyncResult)
                {
                    if (isDisposed != 0) throw objectDisposedException;
                    tcpStreamAsyncResult result = asyncResult as tcpStreamAsyncResult;
                    if (result == null) throw argumentNullException;
                    if (!result.IsCallback) result.AsyncWaitHandle.WaitOne();
                    if (result.IsCompleted) return (int)result.Parameter.Offset;
                    throw ioException;
                }
                /// <summary>
                /// 读取字节序列
                /// </summary>
                /// <param name="buffer">缓冲区</param>
                /// <param name="offset">起始位置</param>
                /// <param name="count">读取字节数</param>
                /// <returns>读取字节数</returns>
                public override unsafe int Read(byte[] buffer, int offset, int count)
                {
                    if (isDisposed != 0) throw objectDisposedException;
                    if (!stream.CanRead) throw notSupportedException;
                    if (buffer == null) throw argumentNullException;
                    if (offset < 0 || count <= 0) throw argumentOutOfRangeException;
                    if (offset + count > buffer.Length) throw argumentException;
                    tcpStreamParameter parameter = get(new tcpStreamParameter { Command = tcpStreamCommand.Read, Offset = count });
                    if (parameter.IsCommand)
                    {
                        subArray<byte> data = parameter.Data;
                        if (data.length != 0) Buffer.BlockCopy(data.array, data.startIndex, buffer, offset, data.length);
                        return data.length;
                    }
                    throw ioException;
                }
                /// <summary>
                /// 读取字节
                /// </summary>
                /// <returns>字节</returns>
                public override int ReadByte()
                {
                    if (isDisposed != 0) throw objectDisposedException;
                    if (!stream.CanRead) throw notSupportedException;
                    tcpStreamParameter parameter = get(new tcpStreamParameter { Command = tcpStreamCommand.ReadByte });
                    if (parameter.IsCommand) return (int)parameter.Offset;
                    throw ioException;
                }
                /// <summary>
                /// 异步写入
                /// </summary>
                /// <param name="buffer">缓冲区</param>
                /// <param name="offset">起始位置</param>
                /// <param name="count">接收字节数</param>
                /// <param name="callback">异步回调</param>
                /// <param name="state">用户对象</param>
                /// <returns>异步写入结果</returns>
                public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
                {
                    if (isDisposed != 0) throw objectDisposedException;
                    if (!stream.CanWrite) throw notSupportedException;
                    if (buffer == null || offset < 0 || count < 0 || offset + count > buffer.Length) throw argumentException;
                    if (count != 0)
                    {
                        tcpStreamAsyncResult result = new tcpStreamAsyncResult { TcpStreamCallback = this, Parameter = new tcpStreamParameter { Command = tcpStreamCommand.BeginWrite, Data = subArray<byte>.Unsafe(buffer, offset, count) }, Callback = callback, AsyncState = state };
                        send(result);
                        return result;
                    }
                    callback(nullTcpStreamAsyncResult);
                    return nullTcpStreamAsyncResult;
                }
                /// <summary>
                /// 结束异步写操作
                /// </summary>
                /// <param name="asyncResult">异步写入结果</param>
                /// <returns>写入的字节数</returns>
                public override void EndWrite(IAsyncResult asyncResult)
                {
                    if (isDisposed != 0) throw objectDisposedException;
                    tcpStreamAsyncResult result = asyncResult as tcpStreamAsyncResult;
                    if (result == null) throw argumentNullException;
                    if (!result.IsCallback) result.AsyncWaitHandle.WaitOne();
                    if (result.IsCompleted) return;
                    throw ioException;
                }
                /// <summary>
                /// 写入字节序列
                /// </summary>
                /// <param name="buffer">缓冲区</param>
                /// <param name="offset">起始位置</param>
                /// <param name="count">读取写入数</param>
                public override void Write(byte[] buffer, int offset, int count)
                {
                    if (isDisposed != 0) throw objectDisposedException;
                    if (!stream.CanWrite) throw notSupportedException;
                    if (buffer == null) throw argumentNullException;
                    if (offset < 0 || count < 0) throw argumentOutOfRangeException;
                    if (offset + count > buffer.Length) throw argumentException;
                    if (count != 0)
                    {
                        tcpStreamParameter parameter = get(new tcpStreamParameter { Command = tcpStreamCommand.Write, Data = subArray<byte>.Unsafe(buffer, offset, count) });
                        if (parameter.IsCommand) return;
                        throw ioException;
                    }
                }
                /// <summary>
                /// 写入字节
                /// </summary>
                /// <param name="value">字节</param>
                public override void WriteByte(byte value)
                {
                    if (isDisposed != 0) throw objectDisposedException;
                    if (!stream.CanWrite) throw notSupportedException;
                    tcpStreamParameter parameter = get(new tcpStreamParameter { Command = tcpStreamCommand.WriteByte, Offset = value });
                    if (parameter.IsCommand) return;
                    throw ioException;
                }
                /// <summary>
                /// 设置流位置
                /// </summary>
                /// <param name="offset">位置</param>
                /// <param name="origin">类型</param>
                /// <returns>流中的新位置</returns>
                public override long Seek(long offset, SeekOrigin origin)
                {
                    if (isDisposed != 0) throw objectDisposedException;
                    tcpStreamParameter parameter = get(new tcpStreamParameter { Command = tcpStreamCommand.Seek, Offset = offset, SeekOrigin = origin });
                    if (parameter.IsCommand) return parameter.Offset;
                    throw notSupportedException;
                }
                /// <summary>
                /// 设置流长度
                /// </summary>
                /// <param name="value">字节长度</param>
                public override void SetLength(long value)
                {
                    if (isDisposed != 0) throw objectDisposedException;
                    tcpStreamParameter parameter = get(new tcpStreamParameter { Command = tcpStreamCommand.SetLength, Offset = value });
                    if (parameter.IsCommand) return;
                    throw notSupportedException;
                }
                /// <summary>
                /// 清除缓冲区
                /// </summary>
                public override void Flush()
                {
                    if (isDisposed == 0)
                    {
                        tcpStreamParameter parameter = get(new tcpStreamParameter { Command = tcpStreamCommand.Flush });
                        if (parameter.IsCommand) return;
                        error();
                    }
                }
                /// <summary>
                /// 错误
                /// </summary>
                private void error()
                {
                    Close();
                    throw ioException;
                }
                /// <summary>
                /// 关闭流
                /// </summary>
                public override void Close()
                {
                    if (Interlocked.Increment(ref isDisposed) == 1)
                    {
                        get(new tcpStreamParameter { Command = tcpStreamCommand.Close });
                        base.Dispose();
                    }
                }
                /// <summary>
                /// 是否资源
                /// </summary>
                public new void Dispose()
                {
                    Close();
                }
            }
            /// <summary>
            /// TCP流读取器集合
            /// </summary>
            private tcpStreamReceiver[] tcpStreamReceivers;
            /// <summary>
            /// TCP流读取器索引
            /// </summary>
            private int tcpStreamReceiveIndex;
            /// <summary>
            /// TCP流读取器空闲索引集合
            /// </summary>
            private subArray<int> freeTcpStreamIndexs;
            /// <summary>
            /// TCP流读取器访问锁
            /// </summary>
            private readonly object tcpStreamReceiveLock = new object();
            /// <summary>
            /// 取消TCP流读取
            /// </summary>
            private Action<long> cancelTcpStreamHandle;
            /// <summary>
            /// 获取TCP流读取器索引
            /// </summary>
            /// <param name="tcpStreamAsyncResult">TCP流异步操作状态</param>
            /// <returns>TCP流读取器索引</returns>
            private int getTcpStreamIndex(tcpStreamAsyncResult tcpStreamAsyncResult)
            {
                int index = -1;
                Monitor.Enter(tcpStreamReceiveLock);
                try
                {
                    if (freeTcpStreamIndexs.length == 0)
                    {
                        if (tcpStreamReceivers == null)
                        {
                            tcpStreamReceivers = new tcpStreamReceiver[4];
                            index = 0;
                            tcpStreamReceiveIndex = 1;
                        }
                        else
                        {
                            if (tcpStreamReceiveIndex == tcpStreamReceivers.Length)
                            {
                                tcpStreamReceiver[] newTcpStreamReceivers = new tcpStreamReceiver[tcpStreamReceiveIndex << 1];
                                tcpStreamReceivers.CopyTo(newTcpStreamReceivers, 0);
                                tcpStreamReceivers = newTcpStreamReceivers;
                            }
                            index = tcpStreamReceiveIndex++;
                        }
                    }
                    else index = freeTcpStreamIndexs.UnsafePop();
                    tcpStreamReceivers[index].SetAsyncResult(tcpStreamAsyncResult);
                }
                finally { Monitor.Exit(tcpStreamReceiveLock); }
                return index;
            }
            /// <summary>
            /// 取消TCP流读取
            /// </summary>
            /// <param name="indexIdentity">TCP流读取器索引+当前处理序号</param>
            private void cancelTcpStream(long indexIdentity)
            {
                cancelTcpStreamIndex((int)(indexIdentity >> 32), (int)indexIdentity, true);
            }
            /// <summary>
            /// 取消TCP流读取
            /// </summary>
            /// <param name="index">TCP流读取器索引</param>
            /// <param name="identity">当前处理序号</param>
            /// <param name="isSetWait">是否设置结束状态</param>
            private void cancelTcpStreamIndex(int index, int identity, bool isSetWait)
            {
                if (tcpStreamReceivers[index].Identity == identity)
                {
                    Monitor.Enter(tcpStreamReceiveLock);
                    try
                    {
                        if (tcpStreamReceivers[index].Cancel(identity, isSetWait)) freeTcpStreamIndexs.Add(index);
                    }
                    finally { Monitor.Exit(tcpStreamReceiveLock); }
                }
            }
            /// <summary>
            /// 等待TCP流读取
            /// </summary>
            /// <param name="index">TCP流读取器索引</param>
            /// <param name="identity">当前处理序号</param>
            /// <returns>读取的数据</returns>
            private tcpStreamParameter waitTcpStream(int index, int identity)
            {
                setTcpStreamTimeout(index, identity);
                EventWaitHandle receiveWait = tcpStreamReceivers[index].ReceiveWait;
                if (receiveWait.WaitOne())
                {
                    tcpStreamParameter parameter = tcpStreamParameter.Null;
                    Monitor.Enter(tcpStreamReceiveLock);
                    try
                    {
                        if (tcpStreamReceivers[index].Get(identity, ref parameter)) freeTcpStreamIndexs.Add(index);
                    }
                    finally { Monitor.Exit(tcpStreamReceiveLock); }
                    return parameter;
                }
                cancelTcpStreamIndex(index, identity, false);
                return tcpStreamParameter.Null;
            }
            /// <summary>
            /// 设置TCP流读取超时
            /// </summary>
            /// <param name="index">TCP流读取器索引</param>
            /// <param name="identity">当前处理序号</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private void setTcpStreamTimeout(int index, int identity)
            {
                if (cancelTcpStreamHandle == null) cancelTcpStreamHandle = cancelTcpStream;
                threading.timerTask.Default.Add(cancelTcpStreamHandle, ((long)index << 32) + identity, date.nowTime.Now.AddSeconds(fastCSharp.config.tcpCommand.Default.TcpStreamTimeout), null);
            }
            /// <summary>
            /// TCP流回馈
            /// </summary>
            /// <param name="parameter">TCP流参数</param>
            internal void OnTcpStream(tcpStreamParameter parameter)
            {
                tcpStreamAsyncResult asyncResult = null;
                int index = parameter.Index;
                Monitor.Enter(tcpStreamReceiveLock); 
                try
                {
                    if (tcpStreamReceivers[index].Set(parameter, ref asyncResult) && asyncResult != null) freeTcpStreamIndexs.Add(index);
                }
                finally
                {
                    Monitor.Exit(tcpStreamReceiveLock); 
                    if (asyncResult != null) asyncResult.OnCallback(parameter);
                }
            }
            /// <summary>
            /// 获取TCP参数流
            /// </summary>
            /// <param name="stream">TCP参数流</param>
            /// <returns>字节流</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public Stream GetTcpStream(tcpBase.tcpStream stream)
            {
                return stream.IsStream ? new tcpStream(this, ref stream) : null;
            }
            /// <summary>
            /// 获取TCP参数流
            /// </summary>
            /// <param name="stream">TCP参数流</param>
            /// <returns>字节流</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public Stream GetTcpStream(ref tcpBase.tcpStream stream)
            {
                return stream.IsStream ? new tcpStream(this, ref stream) : null;
            }
        }
        /// <summary>
        /// 套接字队列
        /// </summary>
        private struct socketQueue
        {
            /// <summary>
            /// 套接字队列节点
            /// </summary>
            private struct node
            {
                /// <summary>
                /// 套接字数量
                /// </summary>
                public int Count;
                /// <summary>
                /// 最后一个套接字索引
                /// </summary>
                public int End;
                /// <summary>
                /// 下一个套接字索引
                /// </summary>
                public int Next;
                /// <summary>
                /// 当前套接字
                /// </summary>
                public Socket Socket;
                /// <summary>
                /// 新增套接字
                /// </summary>
                /// <param name="maxActiveCount"></param>
                /// <returns></returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public bool NewActiveCount(int maxActiveCount)
                {
                    if (Count < maxActiveCount)
                    {
                        ++Count;
                        return true;
                    }
                    return false;
                }
                /// <summary>
                /// 添加套接字到队列中
                /// </summary>
                /// <param name="socket"></param>
                /// <param name="socketQueue"></param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void NewQueue(Socket socket, ref socketQueue socketQueue)
                {
                    if (Socket == null) Socket = socket;
                    else if (End == 0) End = Next = socketQueue.getSocketQueueIndex(socket);
                    else socketQueue.getSocketQueueIndex(socket, ref End);
                    ++Count;
                }
                /// <summary>
                /// 初始化套接字
                /// </summary>
                /// <param name="socket"></param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Set(Socket socket)
                {
                    Socket = socket;
                    Count = 1;
                }
                /// <summary>
                /// 释放套接字
                /// </summary>
                /// <param name="socketQueues"></param>
                /// <returns></returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public int Free(node[] socketQueues)
                {
                    if (--Count == 0) return 0;
                    if (End == 0)
                    {
                        if (Socket == null) return -2;
                        return -1;
                    }
                    int index = Next;
                    if (Next == End) Next = End = 0;
                    else Next = socketQueues[Next].Next;
                    return index;
                }
                /// <summary>
                /// 释放套接字
                /// </summary>
                /// <returns></returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public Socket FreeSocket()
                {
                    Socket socket = Socket;
                    Socket = null;
                    return socket;
                }
                /// <summary>
                /// 关闭套接字
                /// </summary>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Close()
                {
                    Next = End = 0;
                    if (Socket != null) fastCSharp.threading.disposeTimer.Default.addSocketClose(ref Socket);
                }
            }
            /// <summary>
            /// HTTP服务
            /// </summary>
            private commandServer server;
            /// <summary>
            /// 套接字队列
            /// </summary>
            private node[] nodes;
            /// <summary>
            /// 当前套接字队列索引位置
            /// </summary>
            private int nodeIndex;
            /// <summary>
            /// 空闲套接字队列索引位置集合
            /// </summary>
            private subArray<int> nodeIndexs;
            /// <summary>
            /// IPv4套接字队列
            /// </summary>
            private Dictionary<int, int> ipv4Queue;
            /// <summary>
            /// IPv6套接字队列
            /// </summary>
            private Dictionary<ipv6Hash, int> ipv6Queue;
            /// <summary>
            /// 套接字队列访问锁
            /// </summary>
            private object nodeLock;
            /// <summary>
            /// 最大活动套接字数量
            /// </summary>
            private int maxActiveSocketCount;
            /// <summary>
            /// 最大套接字数量
            /// </summary>
            private int maxSocketCount;
            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="server"></param>
            /// <param name="maxActiveSocketCount"></param>
            /// <param name="maxSocketCount"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Set(commandServer server, int maxActiveSocketCount, int maxSocketCount)
            {
                this.server = server;
                this.maxActiveSocketCount = maxActiveSocketCount;
                this.maxSocketCount = maxSocketCount;
                nodeLock = new object();
                nodes = new node[256];
                ipv4Queue = dictionary.CreateInt<int>();
                nodeIndex = 1;
            }
            /// <summary>
            /// 关闭队列
            /// </summary>
            public void Close()
            {
                Monitor.Enter(nodeLock);
                try
                {
                    nodeIndexs.Clear();
                    while (nodeIndex != 1) nodes[--nodeIndex].Close();
                }
                finally { Monitor.Exit(nodeLock); }
            }
            /// <summary>
            /// 客户端请求处理
            /// </summary>
            /// <param name="socket"></param>
            /// <param name="ipv6"></param>
            /// <returns></returns>
            public bool NewSocket(Socket socket, ref ipv6Hash ipv6)
            {
                int index;
                Monitor.Enter(nodeLock);
                if (ipv6Queue != null && ipv6Queue.TryGetValue(ipv6, out index))
                {
                    if (nodes[index].NewActiveCount(maxActiveSocketCount))
                    {
                        Monitor.Exit(nodeLock);
                        server.newSocket(socket, ref ipv6);
                        return true;
                    }
                    if (nodes[index].Count < maxSocketCount)
                    {
                        try
                        {
                            nodes[index].NewQueue(socket, ref this);
                        }
                        finally { Monitor.Exit(nodeLock); }
                        return true;
                    }
                    Monitor.Exit(nodeLock);
                }
                else
                {
                    try
                    {
                        if (ipv6Queue == null) ipv6Queue = dictionary.Create<ipv6Hash, int>();
                        index = getSocketQueueIndex();
                        nodes[index].Set(socket);
                        ipv6Queue.Add(ipv6, index);
                    }
                    finally { Monitor.Exit(nodeLock); }
                    server.newSocket(socket, ref ipv6);
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 请求处理结束
            /// </summary>
            /// <param name="ipv6"></param>
            /// <returns></returns>
            public Socket Next(ref ipv6Hash ipv6)
            {
                int index;
                Monitor.Enter(nodeLock);
                if (ipv6Queue != null && ipv6Queue.TryGetValue(ipv6, out index))
                {
                    Socket socket;
                    int freeIndex = nodes[index].Free(nodes);
                    switch (freeIndex + 2)
                    {
                        case -2 + 2: Monitor.Exit(nodeLock); return null;
                        case -1 + 2:
                            socket = nodes[index].FreeSocket();
                            Monitor.Exit(nodeLock);
                            return socket;
                        case 0 + 2:
                            try
                            {
                                ipv6Queue.Remove(ipv6);
                                nodeIndexs.Add(index);
                            }
                            finally { Monitor.Exit(nodeLock); }
                            return null;
                        default:
                            socket = nodes[freeIndex].FreeSocket();
                            try
                            {
                                nodeIndexs.Add(freeIndex);
                            }
                            finally { Monitor.Exit(nodeLock); }
                            return socket;
                    }
                }
                Monitor.Exit(nodeLock);
                return null;
            }
            /// <summary>
            /// 客户端请求处理
            /// </summary>
            /// <param name="socket"></param>
            /// <param name="ipv4"></param>
            /// <returns></returns>
            public bool NewSocket(Socket socket, int ipv4)
            {
                int ipKey = ipv4 ^ random.Hash, index;
                Monitor.Enter(nodeLock);
                if (ipv4Queue.TryGetValue(ipKey, out index))
                {
                    if (nodes[index].NewActiveCount(maxActiveSocketCount))
                    {
                        Monitor.Exit(nodeLock);
                        server.newSocket(socket, ipv4);
                        return true;
                    }
                    if (nodes[index].Count < maxSocketCount)
                    {
                        try
                        {
                            nodes[index].NewQueue(socket, ref this);
                        }
                        finally { Monitor.Exit(nodeLock); }
                        return true;
                    }
                    Monitor.Exit(nodeLock);
                }
                else
                {
                    try
                    {
                        index = getSocketQueueIndex();
                        nodes[index].Set(socket);
                        ipv4Queue.Add(ipKey, index);
                    }
                    finally { Monitor.Exit(nodeLock); }
                    server.newSocket(socket, ipv4);
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 请求处理结束
            /// </summary>
            /// <param name="ipv4"></param>
            /// <returns></returns>
            public Socket Next(int ipv4)
            {
                int ipKey = ipv4 ^ random.Hash, index;
                Monitor.Enter(nodeLock);
                if (ipv4Queue.TryGetValue(ipKey, out index))
                {
                    Socket socket;
                    int freeIndex = nodes[index].Free(nodes);
                    switch (freeIndex + 2)
                    {
                        case -2 + 2: Monitor.Exit(nodeLock); return null;
                        case -1 + 2:
                            socket = nodes[index].FreeSocket();
                            Monitor.Exit(nodeLock);
                            return socket;
                        case 0 + 2:
                            try
                            {
                                ipv4Queue.Remove(ipKey);
                                nodeIndexs.Add(index);
                            }
                            finally { Monitor.Exit(nodeLock); }
                            return null;
                        default:
                            socket = nodes[freeIndex].FreeSocket();
                            try
                            {
                                nodeIndexs.Add(freeIndex);
                            }
                            finally { Monitor.Exit(nodeLock); }
                            return socket;
                    }
                }
                Monitor.Exit(nodeLock);
                return null;
            }
            /// <summary>
            /// 获取可用套接字索引
            /// </summary>
            /// <param name="socket"></param>
            /// <param name="endIndex"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private void getSocketQueueIndex(Socket socket, ref int endIndex)
            {
                int index = getSocketQueueIndex();
                nodes[endIndex].Next = index;
                nodes[index].Socket = socket;
                endIndex = index;
            }
            /// <summary>
            /// 获取可用套接字索引
            /// </summary>
            /// <param name="socket"></param>
            /// <returns></returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private int getSocketQueueIndex(Socket socket)
            {
                int index = getSocketQueueIndex();
                nodes[index].Socket = socket;
                return index;
            }
            /// <summary>
            /// 获取可用套接字索引
            /// </summary>
            /// <returns></returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private int getSocketQueueIndex()
            {
                return nodeIndexs.length == 0 ? createNodes() : nodeIndexs.UnsafePop();
            }
            /// <summary>
            /// 重建套接字队列
            /// </summary>
            /// <returns></returns>
            private int createNodes()
            {
                if (nodeIndex == nodes.Length)
                {
                    node[] newNodes = new node[nodeIndex << 1];
                    Array.Copy(nodes, 0, newNodes, 0, nodeIndex);
                    nodes = newNodes;
                }
                return nodeIndex++;
            }
        }
        /// <summary>
        /// 最大命令长度
        /// </summary>
        protected int maxCommandLength;
        /// <summary>
        /// 验证命令
        /// </summary>
        protected byte[] verifyCommand = nullValue<byte>.Array;
        /// <summary>
        /// 验证命令序号
        /// </summary>
        protected int verifyCommandIdentity = nullVerifyCommandIdentity;
        ///// <summary>
        ///// 接收数据超时
        ///// </summary>
        //private int receiveTimeout;
        ///// <summary>
        ///// 发送数据超时
        ///// </summary>
        //private int sendTimeout;
        /// <summary>
        /// 每秒最低接收字节数
        /// </summary>
        private int minReceivePerSecond;
        /// <summary>
        /// 接收命令超时
        /// </summary>
        private int receiveCommandTimeout;
        /// <summary>
        /// 接收命令超时时钟周期
        /// </summary>
        private long receiveCommandTicks;
        /// <summary>
        /// HTTP命令处理委托集合
        /// </summary>
        protected fastCSharp.stateSearcher.ascii<httpCommand> httpCommands;
        /// <summary>
        /// HTTP服务器
        /// </summary>
        internal httpServers HttpServers;
        /// <summary>
        /// 套接字队列
        /// </summary>
        private socketQueue[] socketQueues;
        /// <summary>
        /// TCP客户端验证接口
        /// </summary>
        private fastCSharp.code.cSharp.tcpBase.ITcpVerify verify;
        /// <summary>
        /// 命令处理委托集合
        /// </summary>
        protected fastCSharp.stateSearcher.ascii<command> onCommands;
        /// <summary>
        /// 序号识别命令处理委托集合
        /// </summary>
        protected command[] identityOnCommands;
        /// <summary>
        /// 套接字池第一个节点
        /// </summary>
        private socket socketPoolHead;
        /// <summary>
        /// 套接字池最后一个节点
        /// </summary>
        private socket socketPoolEnd;
        /// <summary>
        /// 套接字池访问锁
        /// </summary>
        private int socketPoolLock;
        /// <summary>
        /// 客户端通过验证事件
        /// </summary>
        public event Action<fastCSharp.net.tcp.commandServer.socket> OnClientVerify;
        /// <summary>
        /// 关闭通过验证客户端
        /// </summary>
        public event Action<fastCSharp.net.tcp.commandServer.socket> OnCloseVerifyClient;
        /// <summary>
        /// TCP调用服务端
        /// </summary>
        /// <param name="attribute">配置信息</param>
        public commandServer(fastCSharp.code.cSharp.tcpServer attribute) : this(attribute, null) { }
        /// <summary>
        /// TCP调用服务端
        /// </summary>
        /// <param name="attribute">配置信息</param>
        /// <param name="verify">TCP客户端验证接口</param>
        public unsafe commandServer(fastCSharp.code.cSharp.tcpServer attribute, fastCSharp.code.cSharp.tcpBase.ITcpVerify verify)
            : base(attribute)
        {
            if (attribute.SendBufferSize <= (sizeof(int) * 2 + sizeof(commandServer.streamIdentity))) attribute.SendBufferSize = Math.Max(sizeof(int) * 2 + sizeof(commandServer.streamIdentity), fastCSharp.config.appSetting.StreamBufferSize);
            if (attribute.ReceiveBufferSize <= maxCommandLength + (sizeof(int) * 3 + sizeof(commandServer.streamIdentity))) attribute.ReceiveBufferSize = Math.Max(maxCommandLength + (sizeof(int) * 3 + sizeof(commandServer.streamIdentity)), fastCSharp.config.appSetting.StreamBufferSize);
            //if (attribute.ReceiveTimeout > 0)
            //{
            //    receiveTimeout = attribute.ReceiveTimeout * 1000;
            //    if (receiveTimeout <= 0) receiveTimeout = int.MaxValue;
            //    //sendTimeout = receiveTimeout;
            //}
            //else sendTimeout = config.tcpCommand.Default.DefaultTimeout * 1000;
            if (attribute.MinReceivePerSecond > 0)
            {
                minReceivePerSecond = attribute.MinReceivePerSecond << 10;
                if (minReceivePerSecond <= 0) minReceivePerSecond = int.MaxValue;
            }
            receiveCommandTimeout = (attribute.RecieveCommandMinutes > 0 ? attribute.RecieveCommandMinutes * 60 : config.tcpCommand.Default.DefaultTimeout) * 1000;
            if (receiveCommandTimeout <= 0) receiveCommandTimeout = int.MaxValue;
            receiveCommandTicks = date.MillisecondTicks * receiveCommandTimeout;

            loadBalancingCheckTicks = new TimeSpan(0, 0, Math.Max(attribute.LoadBalancingCheckSeconds - 2, 1)).Ticks;

            this.verify = verify;

            int maxActiveSocketCount = attribute.MaxActiveClientCountIpAddress;
            if (maxActiveSocketCount > 0)
            {
                int maxSocketCount = Math.Max(attribute.MaxClientCountPerIpAddress, maxActiveSocketCount);
                if (attribute.IsIpSocketQueues)
                {
                    socketQueues = new socketQueue[256];
                    for (int index = 256; index != 0; socketQueues[--index].Set(this, maxActiveSocketCount, maxSocketCount)) ;
                }
                else
                {
                    socketQueues = new socketQueue[1];
                    socketQueues[0].Set(this, maxActiveSocketCount, maxSocketCount);
                }
            }
        }
        /// <summary>
        /// TcpCall客户端虚拟
        /// </summary>
        /// <param name="tcpCallVerifyType"></param>
        internal commandServer(Type tcpCallVerifyType) : base(fastCSharp.code.cSharp.tcpServer.GetTcpCallConfig(tcpCallVerifyType)) { }
        /// <summary>
        /// 负载均衡服务TCP服务调用配置
        /// </summary>
        private fastCSharp.code.cSharp.tcpServer loadBalancingAttribute;
        /// <summary>
        /// 负载均衡服务TCP验证方
        /// </summary>
#if NOJIT
        private tcpBase.ITcpClientVerifyMethodAsObject loadBalancingVerifyMethod;
#else
        private tcpBase.ITcpClientVerifyMethod<commandLoadBalancingServer.commandClient> loadBalancingVerifyMethod;
#endif
        /// <summary>
        /// 负载均衡服务TCP验证方
        /// </summary>
        private Action<Exception> loadBalancingOnException;
        /// <summary>
        /// 启动服务并添加到负载均衡服务
        /// </summary>
        /// <param name="attribute">负载均衡服务TCP服务调用配置</param>
        /// <param name="verifyMethod">TCP验证方法</param>
        /// <param name="onException">异常处理</param>
        /// <returns>是否成功</returns>
        public bool StartLoadBalancing(fastCSharp.code.cSharp.tcpServer attribute
#if NOJIT
            , tcpBase.ITcpClientVerifyMethodAsObject verifyMethod = null
#else
            , tcpBase.ITcpClientVerifyMethod<commandLoadBalancingServer.commandClient> verifyMethod = null
#endif
            , Action<Exception> onException = null)
        {
            if (Start())
            {
                if (attribute != null)
                {
                    attribute.IsLoadBalancing = false;
                    loadBalancingAttribute = attribute;
                    loadBalancingVerifyMethod = verifyMethod ?? new commandLoadBalancingServer.verifyMethod();
                    loadBalancingOnException = onException;
                    LoadBalancing();
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// TCP调用服务添加负载均衡服务
        /// </summary>
        internal void LoadBalancing()
        {
            if (isStart != 0)
            {
                try
                {
                    if (new commandLoadBalancingServer.commandClient(loadBalancingAttribute, loadBalancingVerifyMethod)
                        .NewServer(new fastCSharp.net.tcp.host { Host = attribute.Host, Port = attribute.Port }))
                    {
                        return;
                    }
                }
                catch (Exception error)
                {
                    if (loadBalancingOnException == null) log.Error.Add(error, null, false);
                    else loadBalancingOnException(error);
                }
                fastCSharp.threading.timerTask.Default.Add(this, thread.callType.TcpCommandServerLoadBalancing, date.nowTime.Now.AddSeconds(1));
            }
        }
        /// <summary>
        /// 初始化序号识别命令处理委托集合
        /// </summary>
        /// <param name="count">命令数量</param>
        protected void setCommands(int count)
        {
            identityOnCommands = new command[count + CommandStartIndex];
            identityOnCommands[CloseIdentityCommand].Set(CloseIdentityCommand, 0);
            identityOnCommands[CheckIdentityCommand].Set(CheckIdentityCommand, 0);
            identityOnCommands[LoadBalancingCheckIdentityCommand].Set(LoadBalancingCheckIdentityCommand, 0);
            identityOnCommands[StreamMergeIdentityCommand].Set(StreamMergeIdentityCommand);
            identityOnCommands[TcpStreamCommand].Set(TcpStreamCommand);
            identityOnCommands[IgnoreGroupCommand].Set(IgnoreGroupCommand);
        }
        /// <summary>
        /// 命令处理
        /// </summary>
        /// <param name="index"></param>
        /// <param name="socket"></param>
        /// <param name="data"></param>
        protected virtual void doCommand(int index, socket socket, ref subArray<byte> data)
        {
            switch (index - minCommand)
            {
                case CloseIdentityCommand - minCommand: socket.Close(); return;
//                case CheckIdentityCommand - minCommand: return;
                case LoadBalancingCheckIdentityCommand - minCommand: loadBalancingCheck(socket); return;
                case StreamMergeIdentityCommand - minCommand:
                    if (attribute.IsIdentityCommand) mergeStreamIdentity(socket, ref data);
                    else mergeStream(socket, ref data);
                    return;
                case TcpStreamCommand - minCommand: tcpStream(socket, ref data); return;
                case IgnoreGroupCommand - minCommand: ignoreGroup(socket, ref data); return;
            }
        }
        /// <summary>
        /// 命令处理
        /// </summary>
        /// <param name="index"></param>
        /// <param name="socket"></param>
        protected virtual void doHttpCommand(int index, fastCSharp.net.tcp.http.socketBase socket) { }
        /// <summary>
        /// 初始化命令处理委托集合
        /// </summary>
        /// <param name="count">命令数量</param>
        /// <param name="index">命令索引位置</param>
        /// <returns>命令处理委托集合</returns>
        protected keyValue<byte[][], command[]> getCommands(int count, out int index)
        {
            index = 6;
            byte[][] datas = new byte[count + index][];
            command[] commands = new command[count + index];
            datas[0] = closeCommandData;
            commands[0].Set(CloseIdentityCommand, 0);
            datas[1] = checkCommandData;
            commands[1].Set(CheckIdentityCommand, 0);
            datas[2] = loadBalancingCheckCommandData;
            commands[2].Set(LoadBalancingCheckIdentityCommand, 0);
            datas[3] = streamMergeCommandData;
            commands[3].Set(StreamMergeIdentityCommand);
            datas[4] = tcpStreamCommandData;
            commands[4].Set(TcpStreamCommand);
            datas[5] = ignoreGroupCommandData;
            commands[5].Set(IgnoreGroupCommand);
            return new keyValue<byte[][], command[]>(datas, commands);
        }
        /// <summary>
        /// 客户端请求处理
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="ipv6"></param>
        private void newSocket(Socket socket, ref ipv6Hash ipv6)
        {
            socket commandSocket = null;
            try
            {
                interlocked.CompareSetYield(ref socketPoolLock);
                if (socketPoolHead != null)
                {
                    commandSocket = socketPoolHead;
                    socketPoolHead = socketPoolHead.PoolNext;
                }
                socketPoolLock = 0;
                if (commandSocket == null)
                {
                    memoryPool pool = fastCSharp.memoryPool.StreamBuffers;
                    byte[] sendData = pool.Size == attribute.SendBufferSize ? pool.Get() : new byte[attribute.SendBufferSize];
                    byte[] receiveData = pool.Size == attribute.ReceiveBufferSize ? pool.Get() : new byte[attribute.ReceiveBufferSize];
                    commandSocket = new socket(socket, sendData, receiveData, this, ref ipv6);
                }
                else commandSocket.SetSocket(socket, ref ipv6);
                socket = null;
                commandSocket.VerifySocketType();
                commandSocket = null;
                return;
            }
            catch (Exception error)
            {
                log.Error.Add(error, null, false);
            }
            finally
            {
                if (socket != null) fastCSharp.threading.disposeTimer.Default.addSocketClose(socket);
                if (commandSocket != null) commandSocket.Push();
            }
            if (commandSocket == null) Interlocked.Increment(ref freeClientCount);
        }
        /// <summary>
        /// 客户端请求处理
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="ipv4"></param>
        private void newSocket(Socket socket, int ipv4)
        {
            socket commandSocket = null;
            try
            {
                interlocked.CompareSetYield(ref socketPoolLock);
                if (socketPoolHead != null)
                {
                    commandSocket = socketPoolHead;
                    socketPoolHead = socketPoolHead.PoolNext;
                }
                socketPoolLock = 0;
                if (commandSocket == null)
                {
                    memoryPool pool = fastCSharp.memoryPool.StreamBuffers;
                    byte[] sendData = pool.Size == attribute.SendBufferSize ? pool.Get() : new byte[attribute.SendBufferSize];
                    byte[] receiveData = pool.Size == attribute.ReceiveBufferSize ? pool.Get() : new byte[attribute.ReceiveBufferSize];
                    commandSocket = new socket(socket, sendData, receiveData, this, ipv4);
                }
                else commandSocket.SetSocket(socket, ipv4);
                socket = null;
                commandSocket.VerifySocketType();
                commandSocket = null;
                return;
            }
            catch (Exception error)
            {
                log.Error.Add(error, null, false);
            }
            finally
            {
                if (socket != null) fastCSharp.threading.disposeTimer.Default.addSocketClose(socket);
                if (commandSocket != null) commandSocket.Push();
            }
            if (commandSocket == null) Interlocked.Increment(ref freeClientCount);
        }
        /// <summary>
        /// 客户端请求处理
        /// </summary>
        /// <param name="socket">客户端套接字</param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private int newSocketQueue(Socket socket)
        {
            IPEndPoint ipEndPoint = new unionType { Value = socket.RemoteEndPoint }.IPEndPoint;
            if (ipEndPoint.AddressFamily == AddressFamily.InterNetworkV6)
            {
                ipv6Hash ipv6 = ipEndPoint.Address;
                if (ipv6.Ip != null && socketQueues[socketQueues.Length == 1 ? 0 : getQueueIndex((uint)ipv6.HashCode)].NewSocket(socket, ref ipv6)) return 1;
            }
            else
            {
#pragma warning disable 618
                int ipv4 = (int)(uint)(ulong)ipEndPoint.Address.Address;
#pragma warning restore 618
                if (socketQueues[socketQueues.Length == 1 ? 0 : getQueueIndex((uint)ipv4)].NewSocket(socket, ipv4)) return 1;

            }
            fastCSharp.threading.disposeTimer.Default.addSocketClose(socket);
            return 0;
        }
        /// <summary>
        /// 客户端请求处理
        /// </summary>
        /// <param name="socket">客户端套接字</param>
        protected override void newSocket(Socket socket)
        {
            if (socketQueues == null) newSocket(socket, 0);
            else if (newSocketQueue(socket) == 0) --currentClientCount;
        }
        /// <summary>
        /// 客户端请求处理
        /// </summary>
        /// <param name="socket">客户端套接字</param>
        protected override void newSocketMany(Socket socket)
        {
            if (socketQueues == null) newSocket(socket, 0);
            else if (newSocketQueue(socket) == 0) Interlocked.Decrement(ref currentClientCount);
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        public override void Dispose()
        {
            fastCSharp.code.cSharp.tcpServer loadBalancingAttribute = this.loadBalancingAttribute;
            this.loadBalancingAttribute = null;
            if (loadBalancingAttribute != null)
            {
                try
                {
                    new commandLoadBalancingServer.commandClient(loadBalancingAttribute, loadBalancingVerifyMethod)
                        .RemoveServer(new fastCSharp.net.tcp.host { Host = attribute.Host, Port = attribute.Port });
                }
                catch (Exception error)
                {
                    if (loadBalancingOnException == null) log.Error.Add(error, null, false);
                    else loadBalancingOnException(error);
                }
            }
            base.Dispose();

            if (socketQueues != null)
            {
                for (int index = socketQueues.Length; index != 0; socketQueues[--index].Close()) ;
            }

            interlocked.CompareSetYield(ref socketPoolLock);
            socket header = socketPoolHead;
            socketPoolHead = socketPoolEnd = null;
            socketPoolLock = 0;

            pub.Dispose(ref onCommands);
            pub.Dispose(ref httpCommands);

            while (header != null)
            {
                socket next = header.PoolNext;
                header.Dispose();
                header = next;
            }
        }
        /// <summary>
        /// 保存套接字
        /// </summary>
        /// <param name="ip"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected Socket socketEnd(ref ipv6Hash ip)
        {
            Interlocked.Increment(ref freeClientCount);
            return socketQueues == null ? null : socketQueues[socketQueues.Length == 1 ? 0 : getQueueIndex((uint)ip.HashCode)].Next(ref ip);
        }
        /// <summary>
        /// 保存套接字
        /// </summary>
        /// <param name="ip"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected Socket socketEnd(int ip)
        {
            Interlocked.Increment(ref freeClientCount);
            return socketQueues == null ? null : socketQueues[socketQueues.Length == 1 ? 0 : getQueueIndex((uint)ip)].Next(ip);
        }
        /// <summary>
        /// 保存套接字
        /// </summary>
        /// <param name="socket">套接字</param>
        private void push(socket socket)
        {
            interlocked.CompareSetYield(ref socketPoolLock);
            ++socket.PushIdentity;
            if (socketPoolHead == null) socketPoolHead = socketPoolEnd = socket;
            else
            {
                socketPoolEnd.PoolNext = socket;
                socketPoolEnd = socket;
            }
            socketPoolLock = 0;
        }
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <returns>是否成功</returns>
        public override bool Start()
        {
            if (start())
            {
                if (verify == null && (identityOnCommands == null ? verifyCommand.Length == 0 : verifyCommandIdentity == int.MinValue)) log.Error.Add("缺少TCP客户端验证接口或者方法", null, false);
                int bufferLength = maxCommandLength + sizeof(int);
                if (attribute.ReceiveBufferSize < bufferLength) attribute.ReceiveBufferSize = bufferLength;
                if (attribute.IsHttpClient) HttpServers = new httpServers(this);
                getSocket();
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取接收数据超时时间
        /// </summary>
        /// <param name="length">接收数据字节长度</param>
        /// <returns>接收数据超时时间</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private DateTime getReceiveTimeout(int length)
        {
            return minReceivePerSecond == 0 ? DateTime.MaxValue : date.nowTime.Now.AddSeconds(length / minReceivePerSecond + 2);
        }
        /// <summary>
        /// 流合并命令处理
        /// </summary>
        /// <param name="socket">流套接字</param>
        /// <param name="data">输入数据</param>
        private unsafe void mergeStreamIdentity(socket socket, ref subArray<byte> data)
        {
            int isClose = 0;
            try
            {
                byte[] dataArray = data.array;
                command command = default(command);
                fixed (byte* dataFixed = dataArray)
                {
                    int dataLength = data.length;
                    byte* start = dataFixed + data.startIndex;
                    do
                    {
                        int isCloseCommand = 0;
                        if (dataLength >= sizeof(int) * 3 + sizeof(commandServer.streamIdentity))
                        {
                            int commandIdentity = *(int*)start;
                            command.CommandIndex = 0;
                            if ((uint)commandIdentity < identityOnCommands.Length)
                            {
                                if (commandIdentity == CloseIdentityCommand)
                                {
                                    isCloseCommand = 1;
                                    command = mergeCheckCommand;
                                }
                                else command = identityOnCommands[commandIdentity];
                            }
                            if (command.CommandIndex != 0)
                            {
                                int length = *(int*)((start += sizeof(int) * 3 + sizeof(commandServer.streamIdentity)) - sizeof(int));
                                if ((uint)length <= command.MaxDataLength && (dataLength -= length + (sizeof(int) * 3 + sizeof(commandServer.streamIdentity))) >= 0)
                                {
                                    socket.setIdentityFlags(start - (sizeof(int) * 2 + sizeof(commandServer.streamIdentity)));
                                    if (isCloseCommand == 0)
                                    {
                                        data.UnsafeSet((int)(start - dataFixed), length);
                                        doCommand(command.CommandIndex, socket, ref data);
                                    }
                                    else isClose = 1;
                                    start += length;
                                    if (dataLength != 0) continue;
                                }
                            }
                        }
                        break;
                    }
                    while (true);
                }
            }
            catch (Exception error)
            {
                log.Error.Add(error, null, false);
            }
            if (isClose != 0) socket.Close();
        }
        /// <summary>
        /// 流合并命令处理
        /// </summary>
        /// <param name="socket">流套接字</param>
        /// <param name="data">输入数据</param>
        private unsafe void mergeStream(socket socket, ref subArray<byte> data)
        {
            int isClose = 0;
            try
            {
                byte[] dataArray = data.array;
                command command = default(command);
                fixed (byte* dataFixed = dataArray)
                {
                    int dataLength = data.length;
                    byte* start = dataFixed + data.startIndex;
                    do
                    {
                        int commandLength = *(int*)start, isCloseCommand = 0;// + (sizeof(commandServer.streamIdentity) - sizeof(int))
                        if ((uint)commandLength < maxCommandLength)
                        {
                            byte* commandIdentity = start + sizeof(int);
                            data.UnsafeSet((int)(commandIdentity - dataFixed), commandLength - (sizeof(int) * 3 + sizeof(commandServer.streamIdentity)));
                            if (onCommands.Get(ref data, ref command))
                            {
                                if (((*(int*)commandIdentity ^ (CloseIdentityCommand + CommandDataIndex)) | (commandLength ^ (sizeof(int) * 4 + sizeof(commandServer.streamIdentity)))) == 0)
                                {
                                    command = mergeCheckCommand;
                                    isCloseCommand = 1;
                                }
                                int length = *(int*)((start += commandLength) - sizeof(int));
                                if ((uint)length <= command.MaxDataLength && (dataLength -= commandLength + length) >= 0)
                                {
                                    socket.setIdentityFlags(start - (sizeof(int) * 2 + sizeof(commandServer.streamIdentity)));
                                    if (isCloseCommand == 0)
                                    {
                                        data.UnsafeSet((int)(start - dataFixed), length);
                                        doCommand(command.CommandIndex, socket, ref data);
                                    }
                                    else isClose = 1;
                                    start += length;
                                    if (dataLength != 0) continue;
                                }
                            }
                        }
                        break;
                    }
                    while (true);
                }
            }
            catch (Exception error)
            {
                log.Error.Add(error, null, false);
            }
            if (isClose != 0) socket.Close();
        }
        /// <summary>
        /// TCP流回馈
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <param name="data">输入数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static void tcpStream(socket socket, ref subArray<byte> data)
        {
            socket.SendStream(ref socket.identity, returnValue.type.Success);
            tcpStreamParameter parameter = fastCSharp.emit.dataDeSerializer.DeSerialize<commandServer.tcpStreamParameter>(ref data);
            if (parameter != null) socket.OnTcpStream(parameter);
        }

        /// <summary>
        /// 忽略分组事件
        /// </summary>
        public event Action<int> OnIgnoreGroup;
        /// <summary>
        /// 忽略分组
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <param name="data">输入数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void ignoreGroup(socket socket, ref subArray<byte> data)
        {
            int groupId = 0;
            try
            {
                if (!fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref groupId)) groupId = 0;
            }
            catch (Exception error)
            {
                log.Error.Add(error, null, false);
            }
            finally
            {
                task.Tiny.Add(ignoreGroup, new keyValue<socket, int>(socket, groupId), null);
            }
        }
        /// <summary>
        /// 忽略分组
        /// </summary>
        /// <param name="socket">套接字+分组标识</param>
        private void ignoreGroup(keyValue<socket, int> socket)
        {
            if (OnIgnoreGroup != null) OnIgnoreGroup(socket.Value);
            ignoreGroup(socket.Value);
            fastCSharp.domainUnload.WaitTransaction();
            socket.Key.SendStream(ref socket.Key.identity, returnValue.type.Success);
        }
        /// <summary>
        /// 忽略分组
        /// </summary>
        /// <param name="groupId">分组标识</param>
        protected virtual void ignoreGroup(int groupId) { }
        /// <summary>
        /// 最后一次负载均衡联通测试时间
        /// </summary>
        private DateTime loadBalancingCheckTime;
        /// <summary>
        /// 负载均衡联通测试时钟周期
        /// </summary>
        private long loadBalancingCheckTicks;
        /// <summary>
        /// 当前负载均衡套接字
        /// </summary>
        private socket loadBalancingSocket;
        /// <summary>
        /// 当前负载均衡套接字
        /// </summary>
        private socket nextLoadBalancingSocket;
        /// <summary>
        /// 负载均衡联通测试标识
        /// </summary>
        private int loadBalancingCheckIdentity;
        /// <summary>
        /// 负载均衡联通测试
        /// </summary>
        internal void LoadBalancingCheck()
        {
            if (loadBalancingSocket.LoadBalancingCheck(loadBalancingCheckIdentity))
            {
                DateTime now = date.nowTime.Now;
                if (loadBalancingCheckTime < now) loadBalancingCheckTime = now;
                fastCSharp.threading.timerTask.Default.Add(this, thread.callType.TcpCommandServerLoadBalancingCheck, loadBalancingCheckTime = loadBalancingCheckTime.AddTicks(loadBalancingCheckTicks));
            }
            else if (isStart == 0) loadBalancingSocket = nextLoadBalancingSocket = null;
            else
            {
                socket socket = nextLoadBalancingSocket;
                if (socket == null)
                {
                    loadBalancingSocket = null;
                    LoadBalancing();
                }
                else
                {
                    nextLoadBalancingSocket = null;
                    loadBalancingSocket = socket;
                    socket.LoadBalancingCheckIdentity = ++loadBalancingCheckIdentity;
                    LoadBalancingCheck();
                }
            }
        }
        /// <summary>
        /// 负载均衡连接检测套接字
        /// </summary>
        /// <param name="socket">套接字</param>
        private void loadBalancingCheck(socket socket)
        {
            if (loadBalancingAttribute != null)
            {
                if (Interlocked.CompareExchange(ref loadBalancingSocket, socket, null) == null)
                {
                    socket.LoadBalancingCheckIdentity = ++loadBalancingCheckIdentity;
                    fastCSharp.threading.threadPool.TinyPool.FastStart(this, thread.callType.TcpCommandServerLoadBalancingCheck);
                }
                else nextLoadBalancingSocket = socket;
            }
            //socket.SendStream(socket.identity, new fastCSharp.net.returnValue { IsReturn = true });
        }
        /// <summary>
        /// 方法标识名称转TCP调用命令
        /// </summary>
        /// <param name="name">方法标识名称</param>
        /// <returns>TCP调用命令</returns>
        public unsafe static byte[] GetMethodKeyNameCommand(string name)
        {
            int length = name.Length, commandLength = (length + 3) & (int.MaxValue - 3);
            byte[] data = new byte[commandLength + sizeof(int)];
            fixed (byte* dataFixed = data)
            {
                *(int*)dataFixed = commandLength + sizeof(int) * 2 + sizeof(commandServer.streamIdentity) + sizeof(int);
                if ((length & 3) != 0) *(int*)(dataFixed + sizeof(int) + (length & (int.MaxValue - 3))) = 0x20202020;
                formatMethodKeyName(name, dataFixed + sizeof(int));
            }
            return data;
        }
        /// <summary>
        /// 格式化方法标识名称
        /// </summary>
        /// <param name="name">方法标识名称</param>
        /// <returns>方法标识名称</returns>
        protected internal unsafe static byte[] formatMethodKeyName(string name)
        {
            int length = name.Length;
            byte[] data = new byte[(length + 3) & (int.MaxValue - 3)];
            fixed (byte* dataFixed = data)
            {
                if ((length & 3) != 0) *(int*)(dataFixed + (length & (int.MaxValue - 3))) = 0x20202020;
                formatMethodKeyName(name, dataFixed);
            }
            return data;
        }
        /// <summary>
        /// 格式化方法标识名称
        /// </summary>
        /// <param name="name">方法标识名称</param>
        /// <param name="write">写入数据起始位置</param>
        protected internal unsafe static void formatMethodKeyName(string name, byte* write)
        {
            fixed (char* commandFixed = name)
            {
                for (char* start = commandFixed + name.Length, end = commandFixed; start != end; *write++ = (byte)*--start) ;
            }
        }
        /// <summary>
        /// 变换数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="markData"></param>
        /// <param name="startIndex"></param>
        /// <param name="dataLength"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe static void Mark(byte[] data, ulong markData, int startIndex, int dataLength)
        {
            fixed (byte* dataFixed = data) mark(dataFixed + startIndex, markData, dataLength);
        }
        /// <summary>
        /// 变换数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="markData"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe static void Mark(ref subArray<byte> data, ulong markData)
        {
            fixed (byte* dataFixed = data.array) mark(dataFixed + data.startIndex, markData, data.length);
        }
        /// <summary>
        /// 变换数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="markData"></param>
        /// <param name="dataLength"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe static void Mark(byte[] data, ulong markData, int dataLength)
        {
            fixed (byte* dataFixed = data) mark64(dataFixed, markData, dataLength);
        }
        /// <summary>
        /// 变换数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="markData"></param>
        /// <param name="dataLength"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe static void mark(byte* data, ulong markData, int dataLength)
        {
            if (((int)data & 7) == 4) mark32(data, markData, dataLength);
            else mark64(data, markData, dataLength);
        }
        /// <summary>
        /// 变换数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="markData"></param>
        /// <param name="dataLength"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe static void mark32(byte* data, ulong markData, int dataLength)
        {
            *(uint*)data ^= (uint)markData;
            mark64(data + sizeof(int), ((ulong)(uint)markData << 32) | markData >> 32, dataLength - sizeof(uint));
        }
        /// <summary>
        /// 变换数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="markData"></param>
        /// <param name="dataLength"></param>
        private unsafe static void mark64(byte* data, ulong markData, int dataLength)
        {
            for (byte* end = data + (dataLength & (int.MaxValue - (sizeof(ulong) * 4 - 1))); data != end; data += sizeof(ulong) * 4)
            {
                *(ulong*)data ^= markData;
                *(ulong*)(data + sizeof(ulong)) ^= markData;
                *(ulong*)(data + sizeof(ulong) * 2) ^= markData;
                *(ulong*)(data + sizeof(ulong) * 3) ^= markData;
            }
            if ((dataLength & (sizeof(ulong) * 2)) != 0)
            {
                *(ulong*)data ^= markData;
                *(ulong*)(data + sizeof(ulong)) ^= markData;
                data += sizeof(ulong) * 2;
            }
            if ((dataLength & sizeof(ulong)) != 0)
            {
                *(ulong*)data ^= markData;
                data += sizeof(ulong);
            }
            if ((dataLength &= (sizeof(ulong) - 1)) != 0)
            {
                if ((dataLength & sizeof(uint)) != 0)
                {
                    *(uint*)data ^= (uint)markData;
                    data += sizeof(uint);
                    markData >>= 32;
                }
                if ((dataLength & sizeof(ushort)) != 0)
                {
                    *(ushort*)data ^= (ushort)markData;
                    data += sizeof(ushort);
                    markData >>= 16;
                }
                if ((dataLength & 1) != 0) *(byte*)data ^= (byte)markData;
            }
        }
        ///// <summary>
        ///// 调试日志
        ///// </summary>
        //private static log debugLog;
        ///// <summary>
        ///// 调试日志访问锁
        ///// </summary>
        //private static int debugLogLock;
        ///// <summary>
        ///// 调试日志
        ///// </summary>
        //internal static log DebugLog
        //{
        //    get
        //    {
        //        if (debugLog == null)
        //        {
        //            interlocked.CompareSetSleep0NoCheck(ref debugLogLock);
        //            try
        //            {
        //                debugLog = new log(fastCSharp.config.appSetting.LogPath + "socketDebug.txt");
        //            }
        //            finally { debugLogLock = 0; }
        //        }
        //        return debugLog;
        //    }
        //}
    }
}
