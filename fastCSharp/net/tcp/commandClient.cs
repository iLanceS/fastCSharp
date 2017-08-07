using System;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using fastCSharp.io;
using fastCSharp.io.compression;
using fastCSharp.code.cSharp;
using fastCSharp.threading;
using System.IO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace fastCSharp.net.tcp
{
    /// <summary>
    /// TCP调用客户端
    /// </summary>
    public class commandClient : IDisposable
    {
        /// <summary>
        /// TCP调用客户端接口
        /// </summary>
        public interface IClient : IDisposable
        {
            /// <summary>
            /// TCP调用客户端
            /// </summary>
            fastCSharp.net.tcp.commandClient TcpCommandClient { get; }
        }
        /// <summary>
        /// 命令信息
        /// </summary>
        public sealed class dataCommand
        {
            /// <summary>
            /// TCP调用命令
            /// </summary>
            public byte[] Command;
            /// <summary>
            /// 输入参数最大数据长度,0表示不限
            /// </summary>
            public int MaxInputSize;
            /// <summary>
            /// 是否保持异步回调
            /// </summary>
            public byte IsKeepCallback;
            /// <summary>
            /// 客户端是否仅仅发送数据(不需要应答)
            /// </summary>
            public byte IsSendOnly;
        }
        /// <summary>
        /// 命令信息
        /// </summary>
        public sealed class identityCommand
        {
            /// <summary>
            /// TCP调用命令
            /// </summary>
            public int Command;
            /// <summary>
            /// 输入参数最大数据长度,0表示不限
            /// </summary>
            public int MaxInputSize;
            /// <summary>
            /// 是否保持异步回调
            /// </summary>
            public byte IsKeepCallback;
            /// <summary>
            /// 客户端是否仅仅发送数据(不需要应答)
            /// </summary>
            public byte IsSendOnly;
        }
        /// <summary>
        /// 创建TCP调用客户端
        /// </summary>
        public abstract class routerClientCreator
        {
            /// <summary>
            /// 创建TCP调用客户端
            /// </summary>
            internal abstract void CreateThread();
            /// <summary>
            /// 创建TCP调用客户端
            /// </summary>
            /// <param name="socket"></param>
            internal abstract void OnCreated(streamCommandSocket socket);
        }
        /// <summary>
        /// TCP客户端套接字
        /// </summary>
        public abstract class socket : commandSocket<commandClient>
        {
            /// <summary>
            /// TCP调用客户端
            /// </summary>
            protected commandClient commandClient;
            /// <summary>
            /// 配置信息
            /// </summary>
            protected fastCSharp.code.cSharp.tcpServer attribute;
            /// <summary>
            /// TCP服务端口信息
            /// </summary>
            private host host;
            /// <summary>
            /// 最后一次检测时间
            /// </summary>
            protected DateTime lastCheckTime;
            /// <summary>
            /// 检测时间周期
            /// </summary>
            protected readonly long checkTimeTicks;
            /// <summary>
            /// TCP客户端套接字
            /// </summary>
            /// <param name="commandClient">TCP调用客户端</param>
            /// <param name="client">TCP调用客户端</param>
            /// <param name="sendData">接收数据缓冲区</param>
            /// <param name="receiveData">发送数据缓冲区</param>
            protected socket(commandClient commandClient, Socket client, byte[] sendData, byte[] receiveData)
                : base(client, sendData, receiveData, commandClient, true)
            {
                attribute = (this.commandClient = commandClient).Attribute;
                host.Set(attribute.Host, attribute.Port);
                if (attribute.ClientCheckSeconds > 0) checkTimeTicks = new TimeSpan(0, 0, attribute.ClientCheckSeconds).Ticks;
            }
            /// <summary>
            /// TCP套接字添加到池
            /// </summary>
            internal override void PushPool()
            {
                log.Error.Throw(log.exceptionType.ErrorOperation);
            }
            /// <summary>
            /// 检测TCP服务端口信息是否匹配
            /// </summary>
            /// <param name="attribute"></param>
            /// <returns></returns>
            internal bool CheckHost(fastCSharp.code.cSharp.tcpServer attribute)
            {
                return attribute.Port == host.Port && attribute.Host == host.Host;
            }
            /// <summary>
            /// 负载均衡连接检测
            /// </summary>
            protected abstract void loadBalancingCheck();
            /// <summary>
            /// 客户端验证
            /// </summary>
            /// <returns>是否验证成功</returns>
            protected unsafe bool verify()
            {
                fixed (byte* dataFixed = receiveData) *(int*)dataFixed = attribute.IsIdentityCommand ? commandServer.IdentityVerifyIdentity : commandServer.VerifyIdentity;
                if (send(receiveData, 0, sizeof(int)) && (commandSocketProxy.Verify == null || commandSocketProxy.Verify.Verify(this)))
                {
                    identity.Identity = attribute.IsIdentityCommand ? commandServer.IdentityVerifyIdentity : commandServer.VerifyIdentity;
                    if (IsSuccess()) return true;
                }
                log.Error.Add(null, "TCP客户端验证失败", false);
                Dispose();
                return false;
            }
            ///// <summary>
            ///// 设置会话标识
            ///// </summary>
            //protected internal void setIdentity()
            //{
            //    identity.Identity = ((int)fastCSharp.pub.Identity ^ (int)fastCSharp.pub.StartTime.Ticks) & int.MaxValue;
            //    if (identity.Identity == 0) identity.Identity = int.MaxValue;
            //}
            /// <summary>
            /// 判断操作状态是否成功
            /// </summary>
            /// <returns>操作状态是否成功</returns>
            public unsafe bool IsSuccess()
            {
                if (tryReceive(0, sizeof(int), DateTime.MaxValue) == sizeof(int))
                {
                    fixed (byte* dataFixed = receiveData)
                    {
                        if (*(int*)dataFixed == identity.Identity) return true;
                    }
                }
                Dispose();
                return false;
            }
            ///// <summary>
            ///// 判断操作状态是否成功并获取反馈数据
            ///// </summary>
            ///// <returns>反馈数据,失败为null</returns>
            //protected unsafe internal memoryPool.pushSubArray getData()
            //{
            //    int receiveLength = tryReceive(0, sizeof(int) + sizeof(int), DateTime.MaxValue);
            //    if (receiveLength >= sizeof(int) + sizeof(int))
            //    {
            //        fixed (byte* dataFixed = receiveData)
            //        {
            //            if (*(int*)dataFixed == identity.Identity)
            //            {
            //                int length = *(int*)(dataFixed + sizeof(int));
            //                if (length != 0)
            //                {
            //                    return getData(dataFixed, sizeof(int) + sizeof(int), receiveLength, length, DateTime.MaxValue);
            //                }
            //                else if (receiveLength == sizeof(int) + sizeof(int))
            //                {
            //                    return new memoryPool.pushSubArray { SubArray = subArray<byte>.Unsafe(receiveData, 0, 0) };
            //                }
            //            }
            //        }
            //    }
            //    Dispose();
            //    return default(memoryPool.pushSubArray);
            //}
        }
        /// <summary>
        /// TCP客户端命令流处理套接字
        /// </summary>
        public sealed unsafe class streamCommandSocket : socket
        {
            /// <summary>
            /// 关闭连接命令数据
            /// </summary>
            private static readonly byte[] closeCommandData;
            /// <summary>
            /// 关闭连接命令数据
            /// </summary>
            private static readonly byte[] closeIdentityCommandData;
            /// <summary>
            /// 连接检测命令数据
            /// </summary>
            private static readonly identityCommand checkIdentityCommand = new identityCommand { Command = commandServer.CheckIdentityCommand, IsSendOnly = 1 };
            /// <summary>
            /// 负载均衡连接检测命令数据
            /// </summary>
            private static readonly identityCommand loadBalancingCheckIdentityCommand = new identityCommand { Command = commandServer.LoadBalancingCheckIdentityCommand, IsSendOnly = 1 };
            /// <summary>
            /// TCP流回应命令数据
            /// </summary>
            private static readonly identityCommand tcpStreamIdentityCommand = new identityCommand { Command = commandServer.TcpStreamCommand, MaxInputSize = int.MaxValue };
            /// <summary>
            /// 忽略分组命令数据
            /// </summary>
            private static readonly identityCommand ignoreGroupIdentityCommand = new identityCommand { Command = commandServer.IgnoreGroupCommand, MaxInputSize = int.MaxValue };
            /// <summary>
            /// 连接检测命令数据
            /// </summary>
            private static readonly dataCommand checkDataCommand;
            /// <summary>
            /// 负载均衡连接检测命令数据
            /// </summary>
            private static readonly dataCommand loadBalancingCheckDataCommand;
            /// <summary>
            /// TCP流回应命令数据
            /// </summary>
            private static readonly dataCommand tcpStreamDataCommand;
            /// <summary>
            /// 忽略分组命令数据
            /// </summary>
            private static readonly dataCommand ignoreGroupDataCommand;
            /// <summary>
            /// 命令索引信息
            /// </summary>
            private struct commandIndex
            {
                /// <summary>
                /// 客户端命令
                /// </summary>
                public command Command;
                /// <summary>
                /// 索引编号
                /// </summary>
                public int Identity;
                /// <summary>
                /// 回调是否使用任务池
                /// </summary>
                public byte IsTask;
                /// <summary>
                /// 是否保持回调
                /// </summary>
                public byte IsKeep;
                /// <summary>
                /// 清除命令信息
                /// </summary>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Clear()
                {
                    ++Identity;
                    Command = null;
                    IsKeep = IsTask = 0;
                }
                /// <summary>
                /// 设置接收数据回调
                /// </summary>
                /// <param name="command">客户端命令</param>
                /// <param name="isTask">回调是否使用任务池</param>
                /// <param name="isKeep">是否保持回调</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Set(command command, byte isTask, byte isKeep)
                {
                    Command = command;
                    IsKeep = isKeep;
                    IsTask = isTask;
                }
                ///// <summary>
                ///// 取消接收数据
                ///// </summary>
                ///// <returns>接收数据回调+回调是否使用任务池</returns>
                //[MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                //public keyValue<pushPool<memoryPool.pushSubArray>, byte> Cancel()
                //{
                //    pushPool<memoryPool.pushSubArray> onReceive = OnReceive;
                //    byte isTask = IsTask;
                //    ++Identity;
                //    OnReceive = null;
                //    IsKeep = IsTask = 0;
                //    return new keyValue<pushPool<memoryPool.pushSubArray>, byte>(onReceive, isTask);
                //}
                /// <summary>
                /// 取消接收数据
                /// </summary>
                /// <returns>接收数据回调+回调是否使用任务池</returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public command Cancel()
                {
                    command command = Command;
                    ++Identity;
                    Command = null;
                    IsKeep = IsTask = 0;
                    return command;
                }
                /// <summary>
                /// 取消接收数据回调
                /// </summary>
                /// <param name="identity">索引编号</param>
                /// <returns>是否释放</returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public bool Cancel(int identity)
                {
                    if (identity == Identity)
                    {
                        ++Identity;
                        Command = null;
                        IsKeep = IsTask = 0;
                        return true;
                    }
                    return false;
                }
                /// <summary>
                /// 获取接收数据回调
                /// </summary>
                /// <param name="identity">索引编号</param>
                /// <param name="command">客户端命令</param>
                /// <param name="isTask">回调是否使用任务池</param>
                /// <returns>是否释放</returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public bool Get(int identity, ref command command, ref byte isTask)
                {
                    if (identity == Identity)
                    {
                        command = Command;
                        isTask = IsTask;
                        if (IsKeep == 0)
                        {
                            ++Identity;
                            Command = null;
                            IsTask = 0;
                            return true;
                        }
                    }
                    return false;
                }
            }
            /// <summary>
            /// 保持回调
            /// </summary>
            public sealed class keepCallback : IDisposable
            {
                /// <summary>
                /// 默认空保持回调
                /// </summary>
                internal static readonly keepCallback Null = new keepCallback();
                /// <summary>
                /// 客户端命令
                /// </summary>
                private asyncCommand command;
                /// <summary>
                /// 保持回调序号
                /// </summary>
                private int identity;
                /// <summary>
                /// 命令集合索引
                /// </summary>
                private int commandIndex;
                /// <summary>
                /// 命令序号
                /// </summary>
                private int commandIdentity;
                /// <summary>
                /// 客户端命令
                /// </summary>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                internal keepCallback() { }
                /// <summary>
                /// 客户端命令
                /// </summary>
                /// <param name="command"></param>
                /// <param name="identity"></param>
                /// <param name="commandIndex"></param>
                /// <param name="commandIdentity">命令序号</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                internal keepCallback(asyncCommand command, int identity, int commandIndex, int commandIdentity)
                {
                    this.command = command;
                    this.identity = identity;
                    this.commandIndex = commandIndex;
                    this.commandIdentity = commandIdentity;
                }
                /// <summary>
                /// 终止回调
                /// </summary>
                public void Dispose()
                {
                    if (this.command != null)
                    {
                        asyncCommand command = this.command;
                        this.command = null;
                        command.CancelKeep(identity, commandIndex, commandIdentity);
                    }
                }
            }
            /// <summary>
            /// 关闭连接回调命令索引
            /// </summary>
            internal const int CloseCallbackIndex = 0;
            /// <summary>
            /// 合并回调命令索引
            /// </summary>
            internal const int MergeCallbackIndex = CloseCallbackIndex + 1;
            /// <summary>
            /// TCP流回调命令索引
            /// </summary>
            internal const int TcpStreamCallbackIndex = MergeCallbackIndex + 1;
            /// <summary>
            /// 服务器端负载均衡联通测试
            /// </summary>
            internal const int LoadBalancingCheckTimeCallbackIndex = TcpStreamCallbackIndex + 1;
            /// <summary>
            /// 命令索引起始位置
            /// </summary>
            private const int commandPoolIndex = LoadBalancingCheckTimeCallbackIndex + 1;
            /// <summary>
            /// 命令索引池
            /// </summary>
            private indexValuePool<commandIndex> commandPool;
            ///// <summary>
            ///// 命令索引信息集合
            ///// </summary>
            //private commandIndex[] commandIndexs;
            ///// <summary>
            ///// 命令信息空闲索引集合
            ///// </summary>
            //private readonly list<int> freeIndexs = new list<int>();
            ///// <summary>
            ///// 命令信息集合最大索引号
            ///// </summary>
            //private int maxIndex;
            ///// <summary>
            ///// 命令索引信息集合访问锁
            ///// </summary>
            //private int commandIndexLock;
            /// <summary>
            /// 接收数据结束位置
            /// </summary>
            private int receiveEndIndex;
            /// <summary>
            /// 接收数据起始位置
            /// </summary>
            private byte* receiveDataFixed;
            /// <summary>
            /// 当前接收会话标识
            /// </summary>
            private commandServer.streamIdentity currentIdentity;
            ///// <summary>
            ///// 是否输出调试信息
            ///// </summary>
            //private bool isOutputDebug;
            /// <summary>
            /// TCP客户端套接字
            /// </summary>
            /// <param name="commandClient">TCP调用客户端</param>
            /// <param name="client">TCP调用客户端</param>
            /// <param name="sendData">接收数据缓冲区</param>
            /// <param name="receiveData">发送数据缓冲区</param>
            private streamCommandSocket(commandClient commandClient, Socket client, byte[] sendData, byte[] receiveData)
                : base(commandClient, client, sendData, receiveData)
            {
                commandPool = new indexValuePool<commandIndex>(255);
                commandPool.Pool[CloseCallbackIndex].Set(new closeCommand { Socket = this }, 0, 1);
                commandPool.Pool[MergeCallbackIndex].Set(new mergeCommand { Socket = this }, 0, 1);
                commandPool.Pool[TcpStreamCallbackIndex].Set(new doTcpStreamCommand { Socket = this }, 0, 1);
                commandPool.Pool[LoadBalancingCheckTimeCallbackIndex].Set(new loadBalancingCheckTimeCommand { Socket = this }, 0, 1);
                commandPool.PoolIndex = commandPoolIndex;

            }
            /// <summary>
            /// 是否可以添加命令
            /// </summary>
            private byte disabledCommand;
            /// <summary>
            /// 命令池是否可用
            /// </summary>
            private byte disabledCommandPool;
            /// <summary>
            /// 关闭套接字连接
            /// </summary>
            protected override void dispose()
            {
                commandClient commandClient = this.commandClient;
                if (commandClient != null)
                {
                    this.commandClient = null;
                    Interlocked.CompareExchange(ref commandClient.streamSocket, null, this);
                }
                interlocked.CompareSetYield(ref commandLock);
                disabledCommand = 1;
                commands.Clear();
                commandLock = 0;
                try
                {
                    if (attribute.IsIdentityCommand) send(closeIdentityCommandData, 0, closeIdentityCommandData.Length);
                    else send(closeCommandData, 0, closeCommandData.Length);
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                try
                {
                    command[] onReceives = null;
                    disabledCommandPool = 1;
                    if (commandPool.Enter())
                    {
                        try
                        {
                            int poolIndex = commandPool.PoolIndex;
                            if (poolIndex > commandPoolIndex)
                            {
                                commandIndex[] pool = commandPool.Pool;
                                onReceives = new command[poolIndex];
                                do
                                {
                                    --poolIndex;
                                    onReceives[poolIndex] = pool[poolIndex].Cancel();
                                }
                                while (poolIndex != commandPoolIndex);
                            }
                            commandPool.ClearIndexContinue();
                        }
                        finally { commandPool.Exit(); }
                        if (onReceives != null)
                        {
                            foreach (command onReceive in onReceives)
                            {
                                if (onReceive != null)
                                {
                                    fastCSharp.threading.task.Tiny.Add(cancelOnReceives, onReceives, null);
                                    break;
                                }
                            }
                        }
                    }
                    buildCommands.Clear();
                }
                finally { base.dispose(); }
            }
            /// <summary>
            /// 取消命令回调
            /// </summary>
            /// <param name="commands">命令回调集合</param>
            private static void cancelOnReceives(command[] commands)
            {
                memoryPool.pushSubArray pushData = default(memoryPool.pushSubArray);
                foreach (command onReceive in commands)
                {
                    if (onReceive != null) onReceive.OnReceive(ref pushData);
                }
            }
            /// <summary>
            /// 连接检测设置
            /// </summary>
            internal void SetCheck()
            {
                IsVerifyMethod = true;
                if (commandSocketProxy.Attribute.IsLoadBalancing) loadBalancingCheck();
                else if (checkTimeTicks != 0)
                {
                    lastCheckTime = date.nowTime.Now;
                    Check();
                }
            }
            /// <summary>
            /// 连接检测
            /// </summary>
            internal void Check()
            {
                if (isDisposed == 0)
                {
                    DateTime checkTime = lastCheckTime.AddTicks(checkTimeTicks);
                    if (checkTime <= date.nowTime.Now)
                    {
                        if (attribute.IsIdentityCommand) Call((callback<fastCSharp.net.returnValue>)null, checkIdentityCommand, false);
                        else Call((callback<fastCSharp.net.returnValue>)null, checkDataCommand, false);
                        timerTask.Default.Add(this, thread.callType.TcpCommandClientSocketCheck, (lastCheckTime = date.nowTime.Now).AddTicks(checkTimeTicks));
                    }
                    else timerTask.Default.Add(this, thread.callType.TcpCommandClientSocketCheck, checkTime);
                }
            }
            /// <summary>
            /// 负载均衡连接检测
            /// </summary>
            protected override void loadBalancingCheck()
            {
                if (attribute.IsIdentityCommand) Call((callback<fastCSharp.net.returnValue>)null, loadBalancingCheckIdentityCommand, false);
                else Call((callback<fastCSharp.net.returnValue>)null, loadBalancingCheckDataCommand, false);
            }
            /// <summary>
            /// 负载均衡连接检测
            /// </summary>
            /// <returns>是否成功</returns>
            internal returnValue.type LoadBalancingCheck()
            {
                fastCSharp.net.waitCall wait = fastCSharp.net.waitCall.Get();
                if (wait != null)
                {
                    if (attribute.IsIdentityCommand) Call(wait, loadBalancingCheckIdentityCommand, false);
                    else Call(wait, loadBalancingCheckDataCommand, false);
                    returnValue returnValue;
                    wait.Get(out returnValue);
                    return returnValue.Type;
                }
                return net.returnValue.type.ClientException;
            }
            /// <summary>
            /// 忽略TCP调用分组
            /// </summary>
            /// <param name="groupId">分组标识</param>
            /// <returns>是否调用成功</returns>
            internal returnValue.type IgnoreGroup(int groupId)
            {
                waitCall wait = waitCall.Get();
                if (wait != null)
                {
                    if (attribute.IsIdentityCommand) Call(wait, ignoreGroupIdentityCommand, ref groupId, false);
                    else Call(wait, ignoreGroupDataCommand, ref groupId, false);
                    returnValue returnValue;
                    wait.Get(out returnValue);
                    return returnValue.Type;
                }
                return net.returnValue.type.ClientException;
            }
            /// <summary>
            /// 获取命令信息集合索引
            /// </summary>
            /// <param name="command">客户端命令</param>
            /// <param name="isKeep">是否保持回调</param>
            private bool newIndex(command command, byte isKeep)
            {
                if (commandPool.Enter())
                {
                    if (disabledCommandPool == 0)
                    {
                        int index;
                        try
                        {
                            index = commandPool.GetIndexContinue();//不能写成一行，可能造成Pool先入栈然后被修改，导致索引溢出
                            commandPool.Pool[index].Set(command, command.IsTask, isKeep);
                        }
                        finally { commandPool.Exit(); }
                        command.Identity.Set(commandPool.Pool[index].Identity, index);
                        return true;
                    }
                    commandPool.Exit();
                }
                return false;
            }
            /// <summary>
            /// 释放命令信息集合索引
            /// </summary>
            /// <param name="index">命令信息集合索引</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private void freeIndex(int index)
            {
                if (commandPool.Enter())
                {
                    commandPool.Pool[index].Clear();
                    commandPool.FreeExit(index);
                }
            }
            ///// <summary>
            ///// 释放命令信息集合索引
            ///// </summary>
            ///// <param name="index">命令信息集合索引</param>
            //[MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            //private void freeIndexLock(int index)
            //{
            //    if (freeIndexs.FreeLength == 0)
            //    {
            //        try
            //        {
            //            freeIndexs.Add(index);
            //        }
            //        finally { commandIndexLock = 0; }
            //    }
            //    else
            //    {
            //        freeIndexs.UnsafeAdd(index);
            //        commandIndexLock = 0;
            //    }
            //}
            /// <summary>
            /// TCP流读取器
            /// </summary>
            private sealed class tcpStreamReader
            {
                /// <summary>
                /// TCP客户端命令流处理套接字
                /// </summary>
                private streamCommandSocket socket;
                /// <summary>
                /// 字节流
                /// </summary>
                private Stream stream;
                /// <summary>
                /// TCP流参数
                /// </summary>
                private commandServer.tcpStreamParameter parameter;
                /// <summary>
                /// 读取回调
                /// </summary>
                private AsyncCallback callback;
                /// <summary>
                /// TCP流读取器
                /// </summary>
                private tcpStreamReader()
                {
                    callback = onRead;
                }
                /// <summary>
                /// 读取回调
                /// </summary>
                /// <param name="result">回调状态</param>
                private void onRead(IAsyncResult result)
                {
                    try
                    {
                        this.parameter.Offset = this.stream.EndRead(result);
                        this.parameter.IsCommand = true;
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, null, false);
                    }
                    streamCommandSocket socket = this.socket;
                    commandServer.tcpStreamParameter parameter = this.parameter;
                    this.stream = null;
                    this.socket = null;
                    this.parameter = null;
                    try
                    {
                        typePool<tcpStreamReader>.PushNotNull(this);
                    }
                    finally
                    {
                        socket.doTcpStream(parameter);
                        asyncBuffers.Push(ref parameter.Data.array);
                    }
                }
                /// <summary>
                /// 读取数据
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="stream">字节流</param>
                /// <param name="parameter">TCP流参数</param>
                /// <param name="buffer"></param>
                public void Read(streamCommandSocket socket, Stream stream, ref commandServer.tcpStreamParameter parameter, ref byte[] buffer)
                {
                    this.parameter = parameter;
                    this.socket = socket;
                    parameter.Data.UnsafeSet(buffer, 0, 0);
                    this.stream = stream;
                    buffer = null;
                    stream.BeginRead(this.parameter.Data.array, 0, (int)this.parameter.Offset, callback, this);
                }
                /// <summary>
                /// 获取TCP流读取器
                /// </summary>
                /// <returns>TCP流读取器</returns>
                public static tcpStreamReader Get()
                {
                    tcpStreamReader tcpStreamReader = typePool<tcpStreamReader>.Pop();
                    if (tcpStreamReader == null)
                    {
                        try
                        {
                            tcpStreamReader = new tcpStreamReader();
                        }
                        catch { }
                        return null;
                    }
                    return tcpStreamReader;
                }
            }
            /// <summary>
            /// TCP流写入器
            /// </summary>
            private sealed class tcpStreamWriter
            {
                /// <summary>
                /// TCP客户端命令流处理套接字
                /// </summary>
                private streamCommandSocket socket;
                /// <summary>
                /// 字节流
                /// </summary>
                private Stream stream;
                /// <summary>
                /// TCP流参数
                /// </summary>
                private commandServer.tcpStreamParameter parameter;
                /// <summary>
                /// 写入回调
                /// </summary>
                private AsyncCallback callback;
                /// <summary>
                /// TCP流写入器
                /// </summary>
                private tcpStreamWriter()
                {
                    callback = onWrite;
                }
                /// <summary>
                /// 写入回调
                /// </summary>
                /// <param name="result">回调状态</param>
                private void onWrite(IAsyncResult result)
                {
                    try
                    {
                        this.stream.EndWrite(result);
                        this.parameter.IsCommand = true;
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, null, false);
                    }
                    streamCommandSocket socket = this.socket;
                    commandServer.tcpStreamParameter parameter = this.parameter;
                    this.stream = null;
                    this.socket = null;
                    this.parameter = null;
                    try
                    {
                        typePool<tcpStreamWriter>.PushNotNull(this);
                    }
                    finally
                    {
                        socket.doTcpStream(parameter);
                    }
                }
                /// <summary>
                /// 写入数据
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="stream">字节流</param>
                /// <param name="parameter">TCP流参数</param>
                public void Write(streamCommandSocket socket, Stream stream, commandServer.tcpStreamParameter parameter)
                {
                    subArray<byte> data = parameter.Data;
                    this.socket = socket;
                    this.stream = stream;
                    this.parameter = parameter;
                    stream.BeginWrite(data.array, data.startIndex, data.length, callback, this);
                }
                /// <summary>
                /// 获取TCP流写入器
                /// </summary>
                /// <returns>TCP流写入器</returns>
                public static tcpStreamWriter Get()
                {
                    tcpStreamWriter tcpStreamWriter = typePool<tcpStreamWriter>.Pop();
                    if (tcpStreamWriter == null)
                    {
                        try
                        {
                            tcpStreamWriter = new tcpStreamWriter();
                        }
                        catch { }
                    }
                    return tcpStreamWriter;
                }
            }
            /// <summary>
            /// 默认预定义命令
            /// </summary>
            private abstract class defaultCommand : command
            {
                /// <summary>
                /// 创建第一个命令输入数据
                /// </summary>
                /// <param name="stream">命令内存流</param>
                /// <returns>数据起始位置,失败返回0</returns>
                public override int BuildIndex(unmanagedStream stream)
                {
                    fastCSharp.log.Error.Throw(log.exceptionType.ErrorOperation);
                    return 0;
                }
            }
            /// <summary>
            /// 关闭客户端命令
            /// </summary>
            private sealed class closeCommand : defaultCommand
            {
                /// <summary>
                /// 接收数据回调处理
                /// </summary>
                /// <param name="data">输出数据</param>
                internal override void OnReceive(ref memoryPool.pushSubArray data)
                {
                    Socket.Dispose();
                }
            }
            /// <summary>
            /// 合并回调处理
            /// </summary>
            private sealed class mergeCommand : defaultCommand
            {
                /// <summary>
                /// 接收数据回调处理
                /// </summary>
                /// <param name="data">输出数据</param>
                internal override void OnReceive(ref memoryPool.pushSubArray data)
                {
                    Socket.merge(ref data);
                }
            }
            /// <summary>
            /// 合并回调处理
            /// </summary>
            /// <param name="data"></param>
            private void merge(ref memoryPool.pushSubArray data)
            {
                try
                {
                    memoryPool.pushSubArray pushData = default(memoryPool.pushSubArray);
                    byte[] dataArray = data.UnsafeArray;
                    int index = data.Value.startIndex, receiveEndIndex = data.Value.EndIndex;
                    fixed (byte* dataFixed = dataArray)
                    {
                        do
                        {
                            int receiveLength = receiveEndIndex - index;
                            if (receiveLength < sizeof(commandServer.streamIdentity) + sizeof(int) + sizeof(int))
                            {
                                if (receiveLength != 0) Dispose();
                                return;
                            }
                            byte* start = dataFixed + index;
                            commandServer.streamIdentity identity = *(commandServer.streamIdentity*)start;
                            int dataLength = *(int*)(start + sizeof(commandServer.streamIdentity));
                            index += sizeof(commandServer.streamIdentity) + sizeof(int);
                            if (dataLength == 0)
                            {
                                pushData.Value.UnsafeSet(null, int.MaxValue, *(start + (sizeof(commandServer.streamIdentity) + sizeof(int))));
                                onReceive(identity, ref pushData, 1, false);
                                index += sizeof(int);
                                continue;
                            }
                            if ((uint)dataLength > (uint)(receiveLength - (sizeof(commandServer.streamIdentity) + sizeof(int))))
                            {
                                Dispose();
                                return;
                            }
                            pushData.Value.UnsafeSet(dataArray, index, dataLength);
                            onReceive(identity, ref pushData, 1, true);
                            index += dataLength;
                        }
                        while (true);
                    }
                }
                finally { data.Push(); }
            }
            /// <summary>
            /// TCP流处理
            /// </summary>
            private sealed class doTcpStreamCommand : defaultCommand
            {
                /// <summary>
                /// 接收数据回调处理
                /// </summary>
                /// <param name="data">输出数据</param>
                internal override void OnReceive(ref memoryPool.pushSubArray data)
                {
                    Socket.doTcpStream(ref data);
                }
            }
            /// <summary>
            /// TCP流处理
            /// </summary>
            /// <param name="data">输出数据</param>
            private void doTcpStream(ref memoryPool.pushSubArray data)
            {
                byte[] buffer = null;
                try
                {
                    commandServer.tcpStreamParameter parameter = fastCSharp.emit.dataDeSerializer.DeSerialize<commandServer.tcpStreamParameter>(ref data.Value);
                    if (parameter != null)
                    {
                        Stream stream = commandSocketProxy.getTcpStream(parameter.ClientIndex, parameter.ClientIdentity);
                        if (stream != null)
                        {
                            parameter.IsClientStream = true;
                            try
                            {
                                switch (parameter.Command)
                                {
                                    case commandServer.tcpStreamCommand.GetLength:
                                        parameter.Offset = stream.Length;
                                        break;
                                    case commandServer.tcpStreamCommand.SetLength:
                                        stream.SetLength(parameter.Offset);
                                        break;
                                    case commandServer.tcpStreamCommand.GetPosition:
                                        parameter.Offset = stream.Position;
                                        break;
                                    case commandServer.tcpStreamCommand.SetPosition:
                                        stream.Position = parameter.Offset;
                                        break;
                                    case commandServer.tcpStreamCommand.GetReadTimeout:
                                        parameter.Offset = stream.ReadTimeout;
                                        break;
                                    case commandServer.tcpStreamCommand.SetReadTimeout:
                                        stream.ReadTimeout = (int)parameter.Offset;
                                        break;
                                    case commandServer.tcpStreamCommand.GetWriteTimeout:
                                        parameter.Offset = stream.WriteTimeout;
                                        break;
                                    case commandServer.tcpStreamCommand.SetWriteTimeout:
                                        stream.WriteTimeout = (int)parameter.Offset;
                                        break;
                                    case commandServer.tcpStreamCommand.BeginRead:
                                        buffer = asyncBuffers.Get((int)parameter.Offset);
                                        tcpStreamReader tcpStreamReader = tcpStreamReader.Get();
                                        if (tcpStreamReader == null) doTcpStream(parameter);
                                        else tcpStreamReader.Read(this, stream, ref parameter, ref buffer);
                                        return;
                                    case commandServer.tcpStreamCommand.Read:
                                        parameter.Data.UnsafeSet(buffer = asyncBuffers.Get((int)parameter.Offset), 0, stream.Read(buffer, 0, (int)parameter.Offset));
                                        buffer = null;
                                        break;
                                    case commandServer.tcpStreamCommand.ReadByte:
                                        parameter.Offset = stream.ReadByte();
                                        break;
                                    case commandServer.tcpStreamCommand.BeginWrite:
                                        tcpStreamWriter tcpStreamWriter = tcpStreamWriter.Get();
                                        if (tcpStreamWriter == null) doTcpStream(parameter);
                                        else tcpStreamWriter.Write(this, stream, parameter);
                                        return;
                                    case commandServer.tcpStreamCommand.Write:
                                        stream.Write(parameter.Data.array, parameter.Data.startIndex, parameter.Data.length);
                                        parameter.Data.Null();
                                        break;
                                    case commandServer.tcpStreamCommand.WriteByte:
                                        stream.WriteByte((byte)parameter.Offset);
                                        break;
                                    case commandServer.tcpStreamCommand.Seek:
                                        parameter.Offset = stream.Seek(parameter.Offset, parameter.SeekOrigin);
                                        break;
                                    case commandServer.tcpStreamCommand.Flush:
                                        stream.Flush();
                                        break;
                                    case commandServer.tcpStreamCommand.Close:
                                        commandSocketProxy.closeTcpStream(parameter.ClientIndex, parameter.ClientIdentity);
                                        stream.Dispose();
                                        break;
                                }
                                parameter.IsCommand = true;
                            }
                            catch (Exception error)
                            {
                                log.Error.Add(error, null, false);
                            }
                        }
                        doTcpStream(parameter);
                    }
                }
                finally
                {
                    data.Push();
                    asyncBuffers.PushOnly(buffer);
                }
            }
            /// <summary>
            /// 服务器端负载均衡联通测试
            /// </summary>
            private sealed class loadBalancingCheckTimeCommand : defaultCommand
            {
                /// <summary>
                /// 接收数据回调处理
                /// </summary>
                /// <param name="data">输出数据</param>
                internal override void OnReceive(ref memoryPool.pushSubArray data)
                {
                    Socket.commandSocketProxy.LoadBalancingCheckTime = date.nowTime.Now;
                }
            }
            /// <summary>
            /// 发送TCP流参数
            /// </summary>
            /// <param name="parameter">TCP流参数</param>
            private void doTcpStream(commandServer.tcpStreamParameter parameter)
            {
                if (isDisposed == 0)
                {
                    if (attribute.IsIdentityCommand) Call(parameter.PushClientBuffer, tcpStreamIdentityCommand, ref parameter, false);
                    else Call(parameter.PushClientBuffer, tcpStreamDataCommand, ref parameter, false);
                }
            }
            /// <summary>
            /// 命令队列集合
            /// </summary>
            private struct commandQueue
            {
                /// <summary>
                /// 第一个节点
                /// </summary>
                public command Head;
                /// <summary>
                /// 最后一个节点
                /// </summary>
                public command End;
                /// <summary>
                /// 是否只存在一个节点
                /// </summary>
                public bool IsSingle
                {
                    get
                    {
                        return Head == End;
                    }
                }
                /// <summary>
                /// 清除命令
                /// </summary>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Clear()
                {
                    Head = End = null;
                }
                /// <summary>
                /// 添加命令
                /// </summary>
                /// <param name="command"></param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Push(command command)
                {
                    if (Head == null) Head = End = command;
                    else
                    {
                        End.Next = command;
                        End = command;
                    }
                }
                /// <summary>
                /// 添加命令
                /// </summary>
                /// <param name="command"></param>
                /// <returns>是否第一个命令</returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public bool IsPushHead(command command)
                {
                    if (Head == null)
                    {
                        Head = End = command;
                        return true;
                    }
                    End.Next = command;
                    End = command;
                    return false;
                }
                /// <summary>
                /// 获取命令
                /// </summary>
                /// <returns></returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public command Pop()
                {
                    if (Head == null) return null;
                    command command = Head;
                    Head = Head.Next;
                    command.Next = null;
                    return command;
                }
            }
            /// <summary>
            /// 命令队列集合
            /// </summary>
            private commandQueue commands;
            /// <summary>
            /// 已经创建命令集合
            /// </summary>
            private commandQueue buildCommands;
            /// <summary>
            /// 命令数据输出缓冲区
            /// </summary>
            private readonly unmanagedStream commandStream = new unmanagedStream((byte*)fastCSharp.emit.pub.PuzzleValue, 1);
            /// <summary>
            /// 命令集合访问锁
            /// </summary>
            private int commandLock;
            /// <summary>
            /// 是否正在创建命令
            /// </summary>
            private readonly object isCommandBuilding = new object();
            /// <summary>
            /// 当前同步命令
            /// </summary>
            private command synchronizeCommand;
            /// <summary>
            /// 是否正在创建命令
            /// </summary>
            private byte isBuildCommand;
            ///// <summary>
            ///// 命令输入数据是否饱和
            ///// </summary>
            //internal bool IsBusy;
            /// <summary>
            /// 添加命令
            /// </summary>
            /// <param name="command">当前命令</param>
            private void pushCommand(command command)
            {
                interlocked.CompareSetYield(ref commandLock);
                if (disabledCommand == 0)
                {
                    byte isBuildCommand = this.isBuildCommand;
                    commands.Push(command);
                    this.isBuildCommand = 1;
                    commandLock = 0;
                    if (isBuildCommand == 0)
                    {
                        if (command.IsAsynchronous == 0)
                        {
                            synchronizeCommand = command;
                            BuildCommand();
                        }
                        else threadPool.TinyPool.FastStart(this, thread.callType.TcpCommandClientSocketBuildCommand);
                    }
                }
                else
                {
                    commandLock = 0;
                    command.Cancel();
                }
            }
            /// <summary>
            /// 创建命令输入数据并执行
            /// </summary>
            internal unsafe void BuildCommand()
            {
                TimeSpan sleepTime = new TimeSpan(0, 0, 0, 0, commandSocketProxy.Attribute.ClientSendSleep);
                int bufferSize = BigBuffers.Size, bufferSize2 = bufferSize >> 1, maxCount = commandSocketProxy.Attribute.MaxClientSendCount;
                commandBuilder commandBuilder = new commandBuilder
                {
                    Socket = this,
                    CommandStream = commandStream,
                    MergeIndex = attribute.IsIdentityCommand ? (sizeof(int) * 3 + sizeof(commandServer.streamIdentity)) : (sizeof(int) * 4 + sizeof(commandServer.streamIdentity))
                };
                byte isSynchronizeCommand = 0;
                Monitor.Enter(isCommandBuilding);
                try
                {
                START:
                    byte[] buffer = sendData;
                    if (buffer == null)
                    {
                        Monitor.Exit(isCommandBuilding);
                        return;
                    }
                    fixed (byte* dataFixed = buffer)
                    {
                        commandBuilder.Reset(dataFixed, buffer.Length);
                        do
                        {
                            interlocked.CompareSetYield(ref commandLock);
                            command command = commands.Pop();
                            if (command == null)
                            {
                                if (buildCommands.Head == null)
                                {
                                    isBuildCommand = 0;
                                    commandLock = 0;
                                    commandStream.Dispose();
                                    Monitor.Exit(isCommandBuilding);
                                    return;
                                }
                                commandLock = 0;
                                commandBuilder.Send();
                                if (isSynchronizeCommand == 0)
                                {
                                    if (sendData != buffer) goto START;
                                }
                                else goto SYNCHRONIZE;
                            }
                            else
                            {
                                commandLock = 0;
                                if (command == synchronizeCommand)
                                {
                                    isSynchronizeCommand = 1;
                                    synchronizeCommand = null;
                                }
                                commandBuilder.Build(command);
                                if (commandBuilder.CommandCount == maxCount || commandStream.Length + commandBuilder.MaxCommandLength > bufferSize || !IsVerifyMethod)
                                {
                                    //IsBusy = true;
                                    commandBuilder.Send();
                                    if (isSynchronizeCommand == 0)
                                    {
                                        //IsBusy = false;
                                        if (sendData != buffer) goto START;
                                    }
                                    else goto SYNCHRONIZE;
                                }
                                if (commands.Head == null && commandStream.Length <= bufferSize2 && (isSynchronizeCommand == 0 || sleepTime.Ticks == 0))
                                {
                                    //IsBusy = false;
                                    Thread.Sleep(sleepTime);
                                }
                            }
                        }
                        while (true);
                    }
                SYNCHRONIZE:
                    interlocked.CompareSetYield(ref commandLock);
                    if (commands.Head == null)
                    {
                        isBuildCommand = 0;
                        commandLock = 0;
                        commandStream.Dispose();
                        Monitor.Exit(isCommandBuilding);
                    }
                    else
                    {
                        commandLock = 0;
                        commandStream.Dispose();
                        Monitor.Exit(isCommandBuilding);
                        threadPool.TinyPool.FastStart(this, thread.callType.TcpCommandClientSocketBuildCommand);
                    }
                }
                catch (Exception error)
                {
                    commandBuilder.Cancel();
                    buildCommands.Clear();
                    interlocked.CompareSetYield(ref commandLock);
                    isBuildCommand = 0;
                    commandLock = 0;
                    synchronizeCommand = null;
                    commandStream.Dispose();
                    Monitor.Exit(isCommandBuilding);
                    Socket.shutdown();
                    log.Error.Add(error, attribute.ServiceName + "[" + attribute.TcpRegisterName + "]", false);
                }
                //finally { IsBusy = false; }
            }
            /// <summary>
            /// 命令创建
            /// </summary>
            private struct commandBuilder
            {
                /// <summary>
                /// TCP客户端命令流处理套接字
                /// </summary>
                public streamCommandSocket Socket;
                /// <summary>
                /// 命令流
                /// </summary>
                public unmanagedStream CommandStream;
                /// <summary>
                /// 命令流数据起始位置
                /// </summary>
                private byte* dataFixed;
                /// <summary>
                /// 当前命令
                /// </summary>
                private command currentCommand;
                /// <summary>
                /// 命令数据
                /// </summary>
                private subArray<byte> data;
                /// <summary>
                /// 命令流字节长度
                /// </summary>
                private int bufferLength;
                /// <summary>
                /// 命令流数据位置
                /// </summary>
                public int MergeIndex;
                /// <summary>
                /// 当前命令数量
                /// </summary>
                public int CommandCount;
                /// <summary>
                /// 最大命令长度
                /// </summary>
                public int MaxCommandLength;
                /// <summary>
                /// 第一个命令数据其实位置
                /// </summary>
                private int buildIndex;
                /// <summary>
                /// 仅发送数据命令数量
                /// </summary>
                private int sendOnlyCommandCount;
                /// <summary>
                /// 重置命令流
                /// </summary>
                /// <param name="data">命令流数据起始位置</param>
                /// <param name="length">命令流字节长度</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Reset(byte* data, int length)
                {
                    CommandStream.UnsafeReset(dataFixed = data, bufferLength = length);
                    CommandStream.UnsafeSetLength(MergeIndex);
                    MaxCommandLength = CommandCount = sendOnlyCommandCount = 0;
                }
                /// <summary>
                /// 创建命令流
                /// </summary>
                /// <param name="command">命令</param>
                public void Build(command command)
                {
                    currentCommand = command;
                    int streamLength = CommandStream.Length, buildIndex = command.BuildIndex(CommandStream);
                    currentCommand = null;
                    if (buildIndex == 0) command.Cancel();
                    else
                    {
                        if (Socket.buildCommands.IsPushHead(command)) this.buildIndex = buildIndex;
                        int commandLength = CommandStream.Length - streamLength;
                        sendOnlyCommandCount += command.IsSendOnly;
                        ++CommandCount;
                        if (commandLength > MaxCommandLength) MaxCommandLength = commandLength;
                    }
                }
                /// <summary>
                /// 发送数据
                /// </summary>
                public void Send()
                {
                    MaxCommandLength = 0;
                    ulong markData = Socket.commandSocketProxy.SendMarkData;
                    memoryPool pushPool = null;
                    int commandLength = CommandStream.Length, dataLength = commandLength - MergeIndex, isNewBuffer = 0;
                    if (Socket.buildCommands.IsSingle)
                    {
                        if (commandLength <= bufferLength)
                        {
                            if (CommandStream.data.sizeValue != bufferLength)
                            {
                                unsafer.memory.Copy(CommandStream.data.Byte + MergeIndex, dataFixed + MergeIndex, dataLength);
                                CommandStream.UnsafeReset(dataFixed, bufferLength);
                            }
                            data.UnsafeSet(Socket.sendData, MergeIndex, dataLength);
                        }
                        else
                        {
                            byte[] newCommandBuffer = CommandStream.GetSizeArray(MergeIndex, bufferLength << 1);
                            fastCSharp.memoryPool.StreamBuffers.Push(ref Socket.sendData);
                            data.UnsafeSet(Socket.sendData = newCommandBuffer, MergeIndex, dataLength);
                            isNewBuffer = 1;
                        }
                        if (Socket.attribute.IsCompress && dataLength > unmanagedStreamBase.DefaultLength)
                        {
                            int startIndex = buildIndex - MergeIndex;
                            subArray<byte> compressData = stream.Deflate.GetCompressUnsafe(data.array, buildIndex, dataLength - startIndex, startIndex, fastCSharp.memoryPool.StreamBuffers);
                            if (compressData.array != null)
                            {
                                fixed (byte* compressFixed = compressData.array, sendFixed = data.array)
                                {
                                    unsafer.memory.SimpleCopy(sendFixed + MergeIndex, compressFixed, startIndex - sizeof(int));
                                    *(int*)(compressFixed + startIndex - sizeof(int)) = -compressData.length;
                                }
                                data.UnsafeSet(compressData.array, 0, compressData.length + startIndex);
                                pushPool = fastCSharp.memoryPool.StreamBuffers;
                                buildIndex = startIndex;
                            }
                        }
                    }
                    else
                    {
                        if (commandLength > bufferLength)
                        {
                            byte[] newCommandBuffer = CommandStream.GetSizeArray(MergeIndex, bufferLength << 1);
                            fastCSharp.memoryPool.StreamBuffers.Push(ref Socket.sendData);
                            data.UnsafeSet(Socket.sendData = newCommandBuffer, 0, commandLength);
                            isNewBuffer = 1;
                        }
                        else
                        {
                            if (CommandStream.data.sizeValue != bufferLength)
                            {
                                unsafer.memory.Copy(CommandStream.data.Byte + MergeIndex, dataFixed + MergeIndex, dataLength);
                                CommandStream.UnsafeReset(dataFixed, bufferLength);
                            }
                            data.UnsafeSet(Socket.sendData, 0, commandLength);
                        }
                        if (Socket.attribute.IsCompress && dataLength > unmanagedStreamBase.DefaultLength)
                        {
                            subArray<byte> compressData = stream.Deflate.GetCompressUnsafe(data.array, MergeIndex, dataLength, MergeIndex, fastCSharp.memoryPool.StreamBuffers);
                            if (compressData.array != null)
                            {
                                dataLength = -compressData.length;
                                data.UnsafeSet(compressData.array, 0, compressData.length + MergeIndex);
                                pushPool = fastCSharp.memoryPool.StreamBuffers;
                            }
                        }
                        fixed (byte* megerDataFixed = data.array)
                        {
                            byte* write;
                            if (Socket.attribute.IsIdentityCommand) *(int*)(write = megerDataFixed) = commandServer.StreamMergeIdentityCommand;
                            else
                            {
                                write = megerDataFixed + sizeof(int);
                                *(int*)megerDataFixed = sizeof(int) * 4 + sizeof(commandServer.streamIdentity);
                                *(int*)write = commandServer.StreamMergeIdentityCommand + commandServer.CommandDataIndex;
                            }
                            //*(commandServer.streamIdentity*)(write + sizeof(int)) = default(commandServer.streamIdentity);
                            *(int*)(write + (sizeof(int) * 2 + sizeof(commandServer.streamIdentity))) = dataLength;
                        }
                        buildIndex = MergeIndex;
                    }
                    if (markData != 0 && (dataLength = data.EndIndex - buildIndex) != 0) commandServer.Mark(data.array, markData, buildIndex, dataLength);
                    if (isNewBuffer == 0) CommandStream.UnsafeSetLength(MergeIndex);
                    Socket.lastCheckTime = date.nowTime.Now;
                    try
                    {
                        CommandCount = 0;
                        //if (Socket.isOutputDebug) commandServer.DebugLog.Add(Socket.attribute.ServiceName + ".Send(" + data.Length.toString() + ")", false, false);
                        if (Socket.send(ref data))
                        {
                            if (sendOnlyCommandCount == 0) Socket.buildCommands.Clear();
                            else
                            {
                                command command = Socket.buildCommands.Head;
                                Socket.buildCommands.Clear();
                                while (command != null)
                                {
                                    command next = command.Next;
                                    command.Next = null;
                                    if (command.IsSendOnly != 0)
                                    {
                                        command.OnSendOnly();
                                        if (--sendOnlyCommandCount == 0) break;
                                    }
                                    command = next;
                                }
                            }
                        }
                        else Cancel();
                    }
                    finally
                    {
                        if (pushPool != null) pushPool.Push(ref data.array);
                    }
                }
                /// <summary>
                /// 取消命令
                /// </summary>
                public void Cancel()
                {
                    if (currentCommand != null)
                    {
                        currentCommand.Cancel();
                        currentCommand = null;
                    }
                    command command = Socket.buildCommands.Head;
                    Socket.buildCommands.Clear();
                    while (command != null)
                    {
                        command next = command.Next;
                        command.Next = null;
                        command.Cancel();
                        command = next;
                    }
                }
            }
            /// <summary>
            /// 客户端命令
            /// </summary>
            internal abstract class command
            {
                /// <summary>
                /// 下一个客户端命令
                /// </summary>
                public command Next;
                /// <summary>
                /// TCP客户端命令流处理套接字
                /// </summary>
                public streamCommandSocket Socket;
                /// <summary>
                /// 会话标识
                /// </summary>
                public commandServer.streamIdentity Identity;
                /// <summary>
                /// 保持回调
                /// </summary>
                public keepCallback KeepCallback;
                /// <summary>
                /// 命令参数
                /// </summary>
                public commandServer.commandFlags Flags;
                /// <summary>
                /// 回调是否使用任务池
                /// </summary>
                public byte IsTask;
                /// <summary>
                /// 是否异步命令
                /// </summary>
                public byte IsAsynchronous;
                /// <summary>
                /// 是否仅发送数据
                /// </summary>
                public byte IsSendOnly;
                /// <summary>
                /// 创建第一个命令输入数据
                /// </summary>
                /// <param name="stream">命令内存流</param>
                /// <returns>数据起始位置,失败返回0</returns>
                public abstract int BuildIndex(unmanagedStream stream);
                /// <summary>
                /// 接收数据回调处理
                /// </summary>
                /// <param name="data">输出数据</param>
                internal abstract void OnReceive(ref memoryPool.pushSubArray data);
                /// <summary>
                /// 接收数据
                /// </summary>
                internal memoryPool.pushSubArray ReceiveData;
                /// <summary>
                /// 接收数据回调处理
                /// </summary>
                internal void OnRecieveData()
                {
                    OnReceive(ref ReceiveData);
                    ReceiveData.Value.Null();
                }
                /// <summary>
                /// 取消错误命令
                /// </summary>
                public virtual void Cancel()
                {
                    memoryPool.pushSubArray pushData = default(memoryPool.pushSubArray);
                    Flags |= commandServer.commandFlags.JsonSerialize;
                    Socket.onReceive(Identity, ref pushData, 1, false);
                }
                /// <summary>
                /// 发送数据完成操作
                /// </summary>
                public void OnSendOnly()
                {
                    memoryPool.pushSubArray pushData = default(memoryPool.pushSubArray);
                    pushData.Value.array = nullValue<byte>.Array;
                    Socket.onReceive(Identity, ref pushData, 0, false);
                }
                /// <summary>
                /// 重新添加命令，用于二进制转JSON
                /// </summary>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void PushCommand()
                {
                    if (Socket.newIndex(this, (byte)0)) Socket.pushCommand(this);
                    else
                    {
                        memoryPool.pushSubArray data = default(memoryPool.pushSubArray);
                        OnReceive(ref data);
                    }
                }
                /// <summary>
                /// 获取调用状态
                /// </summary>
                /// <param name="data"></param>
                /// <returns></returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public returnValue.type GetReturnType(ref subArray<byte> data)
                {
                    if (data.array == null)
                    {
                        if (data.startIndex == int.MaxValue && (data.length & 0x7fffff00) == 0 && (byte)data.length != (byte)returnValue.type.ClientDeSerializeError)
                        {
                            return (returnValue.type)(byte)data.length;
                        }
                        return returnValue.type.ClientNullData;
                    }
                    return returnValue.type.ClientDeSerializeError;
                }
            }
            /// <summary>
            /// 客户端命令
            /// </summary>
            internal abstract class asyncCommand : command
            {
                /// <summary>
                /// 终止保持回调
                /// </summary>
                /// <param name="identity">保持回调序号</param>
                /// <param name="commandIndex">命令集合索引</param>
                /// <param name="commandIdentity">命令序号</param>
                internal abstract void CancelKeep(int identity, int commandIndex, int commandIdentity);
            }
            /// <summary>
            /// 客户端命令
            /// </summary>
            /// <typeparam name="commandType">客户端命令类型</typeparam>
            private abstract class asyncCommand<commandType> : asyncCommand
                where commandType : asyncCommand<commandType>
            {
                /// <summary>
                /// 客户端命令
                /// </summary>
                protected asyncCommand()
                {
                    IsAsynchronous = 1;
                    thisCommand = (commandType)this;
                }
                /// <summary>
                /// 当前命令
                /// </summary>
                private commandType thisCommand;
                /// <summary>
                /// 保持回调序号
                /// </summary>
                protected int keepCallbackIdentity;
                /// <summary>
                /// 保持回调
                /// </summary>
                public bool SetKeepCallback()
                {
                    try
                    {
                        KeepCallback = new keepCallback(this, ++keepCallbackIdentity, Identity.Index, Socket.commandPool.Pool[Identity.Index].Identity);
                        //KeepCallback = new keepCallback(cancelKeepCallback, ++keepCallbackIdentity, Identity.Index, Socket.commandIndexs[Identity.Index].Identity);
                        return true;
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, null, false);
                    }
                    Socket.freeIndex(Identity.Index);
                    return false;
                }
                /// <summary>
                /// 取消错误命令
                /// </summary>
                public override void Cancel()
                {
                    if (KeepCallback == null) base.Cancel();
                    else KeepCallback.Dispose();
                }
                /// <summary>
                /// 添加到对象池
                /// </summary>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                protected void push()
                {
                    if (KeepCallback == null)
                    {
                        Next = null;
                        Socket = null;
                        typePool<commandType>.PushNotNull(thisCommand);
                    }
                }
            }
            /// <summary>
            /// 客户端命令
            /// </summary>
            /// <typeparam name="commandType">客户端命令类型</typeparam>
            /// <typeparam name="callbackType">回调数据类型</typeparam>
            private abstract class asyncCommand<commandType, callbackType> : asyncCommand<commandType>
                where commandType : asyncCommand<commandType, callbackType>
            {
                /// <summary>
                /// 回调委托
                /// </summary>
                public Action<callbackType> Callback;
                /// <summary>
                /// 异步回调
                /// </summary>
                public callback<callbackType> CallbackValue;
                /// <summary>
                /// 添加回调对象
                /// </summary>
                /// <param name="value">回调值</param>
                protected void push(callbackType value)
                {
                    if (Callback == null)
                    {
                        callback<callbackType> callbackValue = CallbackValue;
                        CallbackValue = null;
                        push();
                        if (callbackValue != null)
                        {
                            try
                            {
                                callbackValue.Callback(ref value);
                            }
                            catch (Exception error)
                            {
                                fastCSharp.log.Error.Add(error, null, false);
                            }
                        }
                    }
                    else
                    {
                        Action<callbackType> callback = Callback;
                        Callback = null;
                        push();
                        if (callback != null)
                        {
                            try
                            {
                                callback(value);
                            }
                            catch (Exception error)
                            {
                                fastCSharp.log.Error.Add(error, null, false);
                            }
                        }
                    }
                }
                /// <summary>
                /// 回调处理
                /// </summary>
                /// <param name="value">回调值</param>
                protected void onlyCallback(callbackType value)
                {
                    if (Callback == null)
                    {
                        if (CallbackValue != null)
                        {
                            try
                            {
                                CallbackValue.Callback(ref value);
                            }
                            catch (Exception error)
                            {
                                fastCSharp.log.Error.Add(error, null, false);
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            Callback(value);
                        }
                        catch (Exception error)
                        {
                            fastCSharp.log.Error.Add(error, null, false);
                        }
                    }
                }
                /// <summary>
                /// 终止保持回调
                /// </summary>
                /// <param name="identity">保持回调序号</param>
                /// <param name="commandIndex">命令集合索引</param>
                /// <param name="commandIdentity">命令序号</param>
                internal override void CancelKeep(int identity, int commandIndex, int commandIdentity)
                {
                    if (Interlocked.CompareExchange(ref keepCallbackIdentity, identity + 1, identity) == identity)
                    {
                        memoryPool.pushSubArray pushData = default(memoryPool.pushSubArray);
                        Callback = null;
                        CallbackValue = null;
                        Socket.cancel(commandIndex, commandIdentity);
                        OnReceive(ref pushData);
                    }
                }
            }
            /// <summary>
            /// 客户端命令
            /// </summary>
            /// <typeparam name="commandType">客户端命令类型</typeparam>
            /// <typeparam name="callbackType">回调数据类型</typeparam>
            private abstract class asyncDataCommand<commandType, callbackType> : asyncCommand<commandType, callbackType>
                where commandType : asyncDataCommand<commandType, callbackType> 
            {
                /// <summary>
                /// TCP调用命令
                /// </summary>
                public dataCommand Command;
                /// <summary>
                /// 创建第一个命令输入数据
                /// </summary>
                /// <param name="stream">命令内存流</param>
                /// <returns>数据起始位置,失败返回0</returns>
                public unsafe override int BuildIndex(unmanagedStream stream)
                {
                    byte[] command = Command.Command;
                    stream.PrepLength(command.Length + (sizeof(int) * 2 + sizeof(commandServer.streamIdentity)));
                    byte* write = stream.CurrentData;
                    unsafer.memory.Copy(command, write, command.Length);
                    *(int*)(write += command.Length) = (int)(uint)Flags;
                    *(commandServer.streamIdentity*)(write + sizeof(int)) = Identity;
                    *(int*)(write + (sizeof(int) + sizeof(commandServer.streamIdentity))) = 0;
                    stream.UnsafeAddLength(command.Length + (sizeof(int) * 2 + sizeof(commandServer.streamIdentity)));
                    return stream.Length;
                }
            }
            /// <summary>
            /// 客户端命令
            /// </summary>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            private sealed class asyncOutputDataCommand<outputParameterType> : asyncDataCommand<asyncOutputDataCommand<outputParameterType>, fastCSharp.net.returnValue<outputParameterType>>
            {
                /// <summary>
                /// 输出参数
                /// </summary>
                public outputParameterType OutputParameter;
                /// <summary>
                /// 输出参数访问锁
                /// </summary>
                private readonly object outputLock = new object();
                /// <summary>
                /// 接收数据回调处理
                /// </summary>
                /// <param name="data">输出数据</param>
                internal override void OnReceive(ref memoryPool.pushSubArray data)
                {
                    returnValue.type returnType = GetReturnType(ref data.Value);
                    if (KeepCallback == null)
                    {
                        if ((Flags & commandServer.commandFlags.JsonSerialize) == 0)
                        {
                            try
                            {
                                if (returnType == returnValue.type.ClientDeSerializeError && fastCSharp.emit.dataDeSerializer.DeSerialize(ref data.Value, ref OutputParameter))
                                {
                                    returnType = returnValue.type.Success;
                                }
                            }
                            finally
                            {
                                if (returnType == returnValue.type.ServerDeSerializeError && Socket.commandPool.Pool.Length != 0 && Socket.attribute.IsTryJsonSerializable)
                                {
                                    Flags |= commandServer.commandFlags.JsonSerialize;
                                    PushCommand();
                                }
                                else
                                {
                                    outputParameterType outputParameter = OutputParameter;
                                    OutputParameter = default(outputParameterType);
                                    push(new fastCSharp.net.returnValue<outputParameterType> { Type = returnType, Value = outputParameter });
                                    data.Push();
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                if (returnType == returnValue.type.ClientDeSerializeError)
                                {
                                    tcpBase.parameterJsonToSerialize<outputParameterType> outputParameter = new tcpBase.parameterJsonToSerialize<outputParameterType> { Return = OutputParameter };
                                    if (fastCSharp.emit.dataDeSerializer.DeSerialize(ref data.Value, ref outputParameter))
                                    {
                                        OutputParameter = outputParameter.Return;
                                        returnType = returnValue.type.Success;
                                    }
                                }
                            }
                            finally
                            {
                                outputParameterType outputParameter = OutputParameter;
                                OutputParameter = default(outputParameterType);
                                push(new fastCSharp.net.returnValue<outputParameterType> { Type = returnType, Value = outputParameter });
                                data.Push();
                            }
                        }
                    }
                    else if (returnType == returnValue.type.ClientDeSerializeError)
                    {
                        Monitor.Enter(outputLock);
                        outputParameterType outputParameter = OutputParameter;
                        try
                        {
                            if ((Flags & commandServer.commandFlags.JsonSerialize) == 0)
                            {
                                if (fastCSharp.emit.dataDeSerializer.DeSerialize(ref data.Value, ref outputParameter)) returnType = returnValue.type.Success;
                            }
                            else
                            {
                                tcpBase.parameterJsonToSerialize<outputParameterType> jsonOutputParameter = new tcpBase.parameterJsonToSerialize<outputParameterType> { Return = outputParameter };
                                if (fastCSharp.emit.dataDeSerializer.DeSerialize(ref data.Value, ref jsonOutputParameter))
                                {
                                    outputParameter = jsonOutputParameter.Return;
                                    returnType = returnValue.type.Success;
                                }
                            }
                        }
                        finally
                        {
                            Monitor.Exit(outputLock);
                            onlyCallback(new fastCSharp.net.returnValue<outputParameterType> { Type = returnType, Value = outputParameter });
                            data.Push();
                        }
                    }
                    else
                    {
                        onlyCallback(new fastCSharp.net.returnValue<outputParameterType> { Type = returnValue.type.ClientNullData });
                    }
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="onGet">回调委托</param>
                /// <param name="callback">异步回调</param>
                /// <param name="command">命令信息</param>
                /// <param name="flags">命令参数</param>
                /// <param name="outputParameter">输出参数</param>
                /// <param name="isTask">客户端回调是否使用任务池</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, Action<returnValue<outputParameterType>> onGet, callback<returnValue<outputParameterType>> callback
                    , dataCommand command, commandServer.commandFlags flags, ref outputParameterType outputParameter, bool isTask)
                {
                    Socket = socket;
                    Callback = onGet;
                    CallbackValue = callback;
                    Command = command;
                    Flags = flags;
                    OutputParameter = outputParameter;
                    IsSendOnly = command.IsSendOnly;
                    IsTask = isTask ? (byte)1 : (byte)0;
                    if (socket.newIndex(this, command.IsKeepCallback) && (command.IsKeepCallback == 0 || SetKeepCallback())) return true;
                    OutputParameter = default(outputParameterType);
                    Callback = null;
                    CallbackValue = null;
                    push();
                    return false;
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="onGet">回调委托</param>
                /// <param name="command">命令信息</param>
                /// <param name="flags">命令参数</param>
                /// <param name="outputParameter">输出参数</param>
                /// <param name="isTask">客户端回调是否使用任务池</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, Action<returnValue<outputParameterType>> onGet
                    , dataCommand command, commandServer.commandFlags flags, ref outputParameterType outputParameter, bool isTask)
                {
                    Socket = socket;
                    Callback = onGet;
                    Command = command;
                    Flags = flags;
                    OutputParameter = outputParameter;
                    IsSendOnly = command.IsSendOnly;
                    IsTask = isTask ? (byte)1 : (byte)0;
                    if (socket.newIndex(this, command.IsKeepCallback) && (command.IsKeepCallback == 0 || SetKeepCallback())) return true;
                    OutputParameter = default(outputParameterType);
                    Callback = null;
                    push();
                    return false;
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="callback">异步回调</param>
                /// <param name="command">命令信息</param>
                /// <param name="flags">命令参数</param>
                /// <param name="outputParameter">输出参数</param>
                /// <param name="isTask">客户端回调是否使用任务池</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, callback<returnValue<outputParameterType>> callback
                    , dataCommand command, commandServer.commandFlags flags, ref outputParameterType outputParameter, bool isTask)
                {
                    Socket = socket;
                    CallbackValue = callback;
                    Command = command;
                    Flags = flags;
                    OutputParameter = outputParameter;
                    IsSendOnly = command.IsSendOnly;
                    IsTask = isTask ? (byte)1 : (byte)0;
                    if (socket.newIndex(this, command.IsKeepCallback) && (command.IsKeepCallback == 0 || SetKeepCallback())) return true;
                    OutputParameter = default(outputParameterType);
                    CallbackValue = null;
                    push();
                    return false;
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <returns>客户端命令</returns>
                public static asyncOutputDataCommand<outputParameterType> Get()
                {
                    asyncOutputDataCommand<outputParameterType> command = typePool<asyncOutputDataCommand<outputParameterType>>.Pop();
                    if (command == null)
                    {
                        try
                        {
                            command = new asyncOutputDataCommand<outputParameterType>();
                        }
                        catch (Exception error)
                        {
                            log.Error.Add(error, null, false);
                        }
                    }
                    return command;
                }
            }
            /// <summary>
            /// TCP调用并返回参数值
            /// </summary>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            /// <param name="onGet">回调委托,返回null表示失败</param>
            /// <param name="callback">异步回调</param>
            /// <param name="dataCommand">命令信息</param>
            /// <param name="outputParameter">输出参数</param>
            /// <param name="isTask">客户端回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback Get<outputParameterType>
                (Action<fastCSharp.net.returnValue<outputParameterType>> onGet, callback<fastCSharp.net.returnValue<outputParameterType>> callback, dataCommand dataCommand
                , ref outputParameterType outputParameter, bool isTask)
            {
                fastCSharp.net.returnValue<outputParameterType> returnType;
                if (isDisposed == 0)
                {
                    asyncOutputDataCommand<outputParameterType> command = asyncOutputDataCommand<outputParameterType>.Get();
                    if (command != null && command.Set(this, onGet, callback, dataCommand, default(commandServer.commandFlags), ref outputParameter, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnType = new returnValue<outputParameterType> { Type = returnValue.type.ClientException };
                }
                else returnType = new returnValue<outputParameterType> { Type = returnValue.type.ClientDisposed };
                if (callback != null) callback.Callback(ref returnType);
                else if (onGet != null) onGet(returnType);
                return null;
            }
            /// <summary>
            /// TCP调用并返回参数值
            /// </summary>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            /// <param name="onGet">回调委托,返回null表示失败</param>
            /// <param name="dataCommand">命令信息</param>
            /// <param name="outputParameter">输出参数</param>
            /// <param name="isTask">客户端回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback Get<outputParameterType>
                (Action<fastCSharp.net.returnValue<outputParameterType>> onGet, dataCommand dataCommand
                , ref outputParameterType outputParameter, bool isTask)
            {
                returnValue.type returnType;
                if (isDisposed == 0)
                {
                    asyncOutputDataCommand<outputParameterType> command = asyncOutputDataCommand<outputParameterType>.Get();
                    if (command != null && command.Set(this, onGet, dataCommand, default(commandServer.commandFlags), ref outputParameter, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnType = returnValue.type.ClientException;
                }
                else returnType = returnValue.type.ClientDisposed;
                if (onGet != null) onGet(new fastCSharp.net.returnValue<outputParameterType> { Type = returnType });
                return null;
            }
            /// <summary>
            /// TCP调用并返回参数值
            /// </summary>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            /// <param name="callback">异步回调</param>
            /// <param name="dataCommand">命令信息</param>
            /// <param name="outputParameter">输出参数</param>
            /// <param name="isTask">客户端回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback Get<outputParameterType>
                (callback<fastCSharp.net.returnValue<outputParameterType>> callback, dataCommand dataCommand
                , ref outputParameterType outputParameter, bool isTask)
            {
                fastCSharp.net.returnValue<outputParameterType> returnType;
                if (isDisposed == 0)
                {
                    asyncOutputDataCommand<outputParameterType> command = asyncOutputDataCommand<outputParameterType>.Get();
                    if (command != null && command.Set(this, callback, dataCommand, default(commandServer.commandFlags), ref outputParameter, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnType = new returnValue<outputParameterType> { Type = returnValue.type.ClientException };
                }
                else returnType = new returnValue<outputParameterType> { Type = returnValue.type.ClientDisposed };
                if (callback != null) callback.Callback(ref returnType);
                return null;
            }
            /// <summary>
            /// TCP调用并返回参数值
            /// </summary>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            /// <param name="onGet">回调委托,返回null表示失败</param>
            /// <param name="callback">异步回调</param>
            /// <param name="dataCommand">命令信息</param>
            /// <param name="outputParameter">输出参数</param>
            /// <param name="isTask">客户端回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback GetJson<outputParameterType>
                (Action<fastCSharp.net.returnValue<outputParameterType>> onGet, callback<fastCSharp.net.returnValue<outputParameterType>> callback
                , dataCommand dataCommand, ref outputParameterType outputParameter, bool isTask)
            {
                fastCSharp.net.returnValue<outputParameterType> returnType;
                if (isDisposed == 0)
                {
                    asyncOutputDataCommand<outputParameterType> command = asyncOutputDataCommand<outputParameterType>.Get();
                    if (command != null && command.Set(this, onGet, callback, dataCommand, commandServer.commandFlags.JsonSerialize, ref outputParameter, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnType = new returnValue<outputParameterType> { Type = returnValue.type.ClientException };
                }
                else returnType = new returnValue<outputParameterType> { Type = returnValue.type.ClientDisposed };
                if (callback != null) callback.Callback(ref returnType);
                else if (onGet != null) onGet(returnType);
                return null;
            }
            /// <summary>
            /// TCP调用并返回参数值
            /// </summary>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            /// <param name="onGet">回调委托,返回null表示失败</param>
            /// <param name="dataCommand">命令信息</param>
            /// <param name="outputParameter">输出参数</param>
            /// <param name="isTask">客户端回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback GetJson<outputParameterType>
                (Action<fastCSharp.net.returnValue<outputParameterType>> onGet
                , dataCommand dataCommand, ref outputParameterType outputParameter, bool isTask)
            {
                returnValue.type returnType;
                if (isDisposed == 0)
                {
                    asyncOutputDataCommand<outputParameterType> command = asyncOutputDataCommand<outputParameterType>.Get();
                    if (command != null && command.Set(this, onGet, dataCommand, commandServer.commandFlags.JsonSerialize, ref outputParameter, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnType = returnValue.type.ClientException;
                }
                else returnType = returnValue.type.ClientDisposed;
                if (onGet != null) onGet(new fastCSharp.net.returnValue<outputParameterType> { Type = returnType });
                return null;
            }
            /// <summary>
            /// TCP调用并返回参数值
            /// </summary>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            /// <param name="callback">异步回调</param>
            /// <param name="dataCommand">命令信息</param>
            /// <param name="outputParameter">输出参数</param>
            /// <param name="isTask">客户端回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback GetJson<outputParameterType>
                (callback<fastCSharp.net.returnValue<outputParameterType>> callback
                , dataCommand dataCommand, ref outputParameterType outputParameter, bool isTask)
            {
                fastCSharp.net.returnValue<outputParameterType> returnType;
                if (isDisposed == 0)
                {
                    asyncOutputDataCommand<outputParameterType> command = asyncOutputDataCommand<outputParameterType>.Get();
                    if (command != null && command.Set(this, callback, dataCommand, commandServer.commandFlags.JsonSerialize, ref outputParameter, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnType = new returnValue<outputParameterType> { Type = returnValue.type.ClientException };
                }
                else returnType = new returnValue<outputParameterType> { Type = returnValue.type.ClientDisposed };
                if (callback != null) callback.Callback(ref returnType);
                return null;
            }
            /// <summary>
            /// 客户端命令
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            private sealed class asyncInputOutputDataCommand<inputParameterType, outputParameterType> : asyncInputDataCommand<inputParameterType, fastCSharp.net.returnValue<outputParameterType>, asyncInputOutputDataCommand<inputParameterType, outputParameterType>>
            {
                /// <summary>
                /// 输出参数
                /// </summary>
                public outputParameterType OutputParameter;
                /// <summary>
                /// 输出参数访问锁
                /// </summary>
                private readonly object outputLock = new object();
                /// <summary>
                /// 接收数据回调处理
                /// </summary>
                /// <param name="data">输出数据</param>
                internal override void OnReceive(ref memoryPool.pushSubArray data)
                {
                    returnValue.type returnType = GetReturnType(ref data.Value);
                    if (KeepCallback == null)
                    {
                        if ((Flags & commandServer.commandFlags.JsonSerialize) == 0)
                        {
                            try
                            {
                                if (returnType == returnValue.type.ClientDeSerializeError && fastCSharp.emit.dataDeSerializer.DeSerialize(ref data.Value, ref OutputParameter))
                                {
                                    returnType = returnValue.type.Success;
                                }
                            }
                            finally
                            {
                                if (returnType == returnValue.type.ServerDeSerializeError && Socket.commandPool.Pool.Length != 0 && Socket.attribute.IsTryJsonSerializable)
                                {
                                    Flags |= commandServer.commandFlags.JsonSerialize;
                                    PushCommand();
                                }
                                else
                                {
                                    outputParameterType outputParameter = OutputParameter;
                                    InputParameter = default(inputParameterType);
                                    OutputParameter = default(outputParameterType);
                                    push(new fastCSharp.net.returnValue<outputParameterType> { Type = returnType, Value = outputParameter });
                                    data.Push();
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                if (returnType == returnValue.type.ClientDeSerializeError)
                                {
                                    tcpBase.parameterJsonToSerialize<outputParameterType> outputParameter = new tcpBase.parameterJsonToSerialize<outputParameterType> { Return = OutputParameter };
                                    if (fastCSharp.emit.dataDeSerializer.DeSerialize(ref data.Value, ref outputParameter))
                                    {
                                        OutputParameter = outputParameter.Return;
                                        returnType = returnValue.type.Success;
                                    }
                                }
                            }
                            finally
                            {
                                outputParameterType outputParameter = OutputParameter;
                                InputParameter = default(inputParameterType);
                                OutputParameter = default(outputParameterType);
                                push(new fastCSharp.net.returnValue<outputParameterType> { Type = returnType, Value = outputParameter });
                                data.Push();
                            }
                        }
                    }
                    else if (returnType == returnValue.type.ClientDeSerializeError)
                    {
                        Monitor.Enter(outputLock);
                        outputParameterType outputParameter = OutputParameter;
                        try
                        {
                            if ((Flags & commandServer.commandFlags.JsonSerialize) == 0)
                            {
                                if (fastCSharp.emit.dataDeSerializer.DeSerialize(ref data.Value, ref outputParameter)) returnType = returnValue.type.Success;
                            }
                            else
                            {
                                tcpBase.parameterJsonToSerialize<outputParameterType> jsonOutputParameter = new tcpBase.parameterJsonToSerialize<outputParameterType> { Return = outputParameter };
                                if (fastCSharp.emit.dataDeSerializer.DeSerialize(ref data.Value, ref jsonOutputParameter))
                                {
                                    outputParameter = jsonOutputParameter.Return;
                                    returnType = returnValue.type.Success;
                                }
                            }
                        }
                        finally
                        {
                            Monitor.Exit(outputLock);
                            onlyCallback(new fastCSharp.net.returnValue<outputParameterType> { Type = returnType, Value = outputParameter });
                            data.Push();
                        }
                    }
                    else
                    {
                        onlyCallback(new fastCSharp.net.returnValue<outputParameterType> { Type = returnType });
                    }
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="onGet">回调委托</param>
                /// <param name="callback">异步回调</param>
                /// <param name="command">命令信息</param>
                /// <param name="flags">命令参数</param>
                /// <param name="inputParameter">输入参数</param>
                /// <param name="outputParameter">输出参数</param>
                /// <param name="isTask">客户端回调是否使用任务池</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, Action<returnValue<outputParameterType>> onGet, callback<returnValue<outputParameterType>> callback
                    , dataCommand command, commandServer.commandFlags flags, ref inputParameterType inputParameter, ref outputParameterType outputParameter, bool isTask)
                {
                    Socket = socket;
                    Callback = onGet;
                    CallbackValue = callback;
                    Command = command;
                    Flags = flags;
                    InputParameter = inputParameter;
                    OutputParameter = outputParameter;
                    IsSendOnly = command.IsSendOnly;
                    IsTask = isTask ? (byte)1 : (byte)0;
                    if (socket.newIndex(this, command.IsKeepCallback) && (command.IsKeepCallback == 0 || SetKeepCallback())) return true;
                    InputParameter = default(inputParameterType);
                    OutputParameter = default(outputParameterType);
                    Callback = null;
                    CallbackValue = null;
                    push();
                    return false;
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="onGet">回调委托</param>
                /// <param name="command">命令信息</param>
                /// <param name="flags">命令参数</param>
                /// <param name="inputParameter">输入参数</param>
                /// <param name="outputParameter">输出参数</param>
                /// <param name="isTask">客户端回调是否使用任务池</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, Action<returnValue<outputParameterType>> onGet
                    , dataCommand command, commandServer.commandFlags flags, ref inputParameterType inputParameter, ref outputParameterType outputParameter, bool isTask)
                {
                    Socket = socket;
                    Callback = onGet;
                    Command = command;
                    Flags = flags;
                    InputParameter = inputParameter;
                    OutputParameter = outputParameter;
                    IsSendOnly = command.IsSendOnly;
                    IsTask = isTask ? (byte)1 : (byte)0;
                    if (socket.newIndex(this, command.IsKeepCallback) && (command.IsKeepCallback == 0 || SetKeepCallback())) return true;
                    InputParameter = default(inputParameterType);
                    OutputParameter = default(outputParameterType);
                    Callback = null;
                    push();
                    return false;
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="callback">异步回调</param>
                /// <param name="command">命令信息</param>
                /// <param name="flags">命令参数</param>
                /// <param name="inputParameter">输入参数</param>
                /// <param name="outputParameter">输出参数</param>
                /// <param name="isTask">客户端回调是否使用任务池</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, callback<returnValue<outputParameterType>> callback
                    , dataCommand command, commandServer.commandFlags flags, ref inputParameterType inputParameter, ref outputParameterType outputParameter, bool isTask)
                {
                    Socket = socket;
                    CallbackValue = callback;
                    Command = command;
                    Flags = flags;
                    InputParameter = inputParameter;
                    OutputParameter = outputParameter;
                    IsSendOnly = command.IsSendOnly;
                    IsTask = isTask ? (byte)1 : (byte)0;
                    if (socket.newIndex(this, command.IsKeepCallback) && (command.IsKeepCallback == 0 || SetKeepCallback())) return true;
                    InputParameter = default(inputParameterType);
                    OutputParameter = default(outputParameterType);
                    CallbackValue = null;
                    push();
                    return false;
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <returns>客户端命令</returns>
                public static asyncInputOutputDataCommand<inputParameterType, outputParameterType> Get()
                {
                    asyncInputOutputDataCommand<inputParameterType, outputParameterType> command = typePool<asyncInputOutputDataCommand<inputParameterType, outputParameterType>>.Pop();
                    if (command == null)
                    {
                        try
                        {
                            command = new asyncInputOutputDataCommand<inputParameterType, outputParameterType>();
                        }
                        catch (Exception error)
                        {
                            log.Error.Add(error, null, false);
                        }
                    }
                    return command;
                }
            }
            /// <summary>
            /// TCP调用并返回参数值
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            /// <param name="onGet">回调委托,返回null表示失败</param>
            /// <param name="callback">异步回调</param>
            /// <param name="dataCommand">命令信息</param>
            /// <param name="inputParameter">输入参数</param>
            /// <param name="outputParameter">输出参数</param>
            /// <param name="isTask">客户端回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback Get<inputParameterType, outputParameterType>
                (Action<fastCSharp.net.returnValue<outputParameterType>> onGet, callback<fastCSharp.net.returnValue<outputParameterType>> callback, dataCommand dataCommand
                , ref inputParameterType inputParameter, ref outputParameterType outputParameter, bool isTask)
            {
                fastCSharp.net.returnValue<outputParameterType> returnType;
                if (isDisposed == 0)
                {
                    asyncInputOutputDataCommand<inputParameterType, outputParameterType> command = asyncInputOutputDataCommand<inputParameterType, outputParameterType>.Get();
                    if (command != null && command.Set(this, onGet, callback, dataCommand, default(commandServer.commandFlags), ref inputParameter, ref outputParameter, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnType = new returnValue<outputParameterType> { Type = returnValue.type.ClientException };
                }
                else returnType = new returnValue<outputParameterType> { Type = returnValue.type.ClientDisposed };
                if (callback != null) callback.Callback(ref returnType);
                else if (onGet != null) onGet(returnType);
                return null;
            }
            /// <summary>
            /// TCP调用并返回参数值
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            /// <param name="onGet">回调委托,返回null表示失败</param>
            /// <param name="dataCommand">命令信息</param>
            /// <param name="inputParameter">输入参数</param>
            /// <param name="outputParameter">输出参数</param>
            /// <param name="isTask">客户端回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback Get<inputParameterType, outputParameterType>
                (Action<fastCSharp.net.returnValue<outputParameterType>> onGet, dataCommand dataCommand
                , ref inputParameterType inputParameter, ref outputParameterType outputParameter, bool isTask)
            {
                returnValue.type returnType;
                if (isDisposed == 0)
                {
                    asyncInputOutputDataCommand<inputParameterType, outputParameterType> command = asyncInputOutputDataCommand<inputParameterType, outputParameterType>.Get();
                    if (command != null && command.Set(this, onGet, dataCommand, default(commandServer.commandFlags), ref inputParameter, ref outputParameter, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnType = returnValue.type.ClientException;
                }
                else returnType = returnValue.type.ClientDisposed;
                if (onGet != null) onGet(new fastCSharp.net.returnValue<outputParameterType> { Type = returnType });
                return null;
            }
            /// <summary>
            /// TCP调用并返回参数值
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            /// <param name="callback">异步回调</param>
            /// <param name="dataCommand">命令信息</param>
            /// <param name="inputParameter">输入参数</param>
            /// <param name="outputParameter">输出参数</param>
            /// <param name="isTask">客户端回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback Get<inputParameterType, outputParameterType>
                (callback<fastCSharp.net.returnValue<outputParameterType>> callback, dataCommand dataCommand
                , ref inputParameterType inputParameter, ref outputParameterType outputParameter, bool isTask)
            {
                fastCSharp.net.returnValue<outputParameterType> returnType;
                if (isDisposed == 0)
                {
                    asyncInputOutputDataCommand<inputParameterType, outputParameterType> command = asyncInputOutputDataCommand<inputParameterType, outputParameterType>.Get();
                    if (command != null && command.Set(this, callback, dataCommand, default(commandServer.commandFlags), ref inputParameter, ref outputParameter, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnType = new returnValue<outputParameterType> { Type = returnValue.type.ClientException };
                }
                else returnType = new returnValue<outputParameterType> { Type = returnValue.type.ClientDisposed };
                if (callback != null) callback.Callback(ref returnType);
                return null;
            }
            /// <summary>
            /// TCP调用并返回参数值
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            /// <param name="onGet">回调委托,返回null表示失败</param>
            /// <param name="callback">异步回调</param>
            /// <param name="dataCommand">命令信息</param>
            /// <param name="inputParameter">输入参数</param>
            /// <param name="outputParameter">输出参数</param>
            /// <param name="isTask">客户端回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback GetJson<inputParameterType, outputParameterType>
                (Action<fastCSharp.net.returnValue<outputParameterType>> onGet, callback<fastCSharp.net.returnValue<outputParameterType>> callback
                , dataCommand dataCommand, ref inputParameterType inputParameter, ref outputParameterType outputParameter, bool isTask)
            {
                fastCSharp.net.returnValue<outputParameterType> returnType;
                if (isDisposed == 0)
                {
                    asyncInputOutputDataCommand<inputParameterType, outputParameterType> command = asyncInputOutputDataCommand<inputParameterType, outputParameterType>.Get();
                    if (command != null && command.Set(this, onGet, callback, dataCommand, commandServer.commandFlags.JsonSerialize, ref inputParameter, ref outputParameter, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnType = new returnValue<outputParameterType> { Type = returnValue.type.ClientException };
                }
                else returnType = new returnValue<outputParameterType> { Type = returnValue.type.ClientDisposed };
                if (callback != null) callback.Callback(ref returnType);
                else if (onGet != null) onGet(returnType);
                return null;
            }
            /// <summary>
            /// TCP调用并返回参数值
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            /// <param name="onGet">回调委托,返回null表示失败</param>
            /// <param name="dataCommand">命令信息</param>
            /// <param name="inputParameter">输入参数</param>
            /// <param name="outputParameter">输出参数</param>
            /// <param name="isTask">客户端回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback GetJson<inputParameterType, outputParameterType>
                (Action<fastCSharp.net.returnValue<outputParameterType>> onGet
                , dataCommand dataCommand, ref inputParameterType inputParameter, ref outputParameterType outputParameter, bool isTask)
            {
                returnValue.type returnType;
                if (isDisposed == 0)
                {
                    asyncInputOutputDataCommand<inputParameterType, outputParameterType> command = asyncInputOutputDataCommand<inputParameterType, outputParameterType>.Get();
                    if (command != null && command.Set(this, onGet, dataCommand, commandServer.commandFlags.JsonSerialize, ref inputParameter, ref outputParameter, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnType = returnValue.type.ClientException;
                }
                else returnType = returnValue.type.ClientDisposed;
                if (onGet != null) onGet(new fastCSharp.net.returnValue<outputParameterType> { Type = returnType });
                return null;
            }
            /// <summary>
            /// TCP调用并返回参数值
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            /// <param name="callback">异步回调</param>
            /// <param name="dataCommand">命令信息</param>
            /// <param name="inputParameter">输入参数</param>
            /// <param name="outputParameter">输出参数</param>
            /// <param name="isTask">客户端回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback GetJson<inputParameterType, outputParameterType>
                (callback<fastCSharp.net.returnValue<outputParameterType>> callback
                , dataCommand dataCommand, ref inputParameterType inputParameter, ref outputParameterType outputParameter, bool isTask)
            {
                fastCSharp.net.returnValue<outputParameterType> returnType;
                if (isDisposed == 0)
                {
                    asyncInputOutputDataCommand<inputParameterType, outputParameterType> command = asyncInputOutputDataCommand<inputParameterType, outputParameterType>.Get();
                    if (command != null && command.Set(this, callback, dataCommand, commandServer.commandFlags.JsonSerialize, ref inputParameter, ref outputParameter, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnType = new returnValue<outputParameterType> { Type = returnValue.type.ClientException };
                }
                else returnType = new returnValue<outputParameterType> { Type = returnValue.type.ClientDisposed };
                if (callback != null) callback.Callback(ref returnType);
                return null;
            }
            /// <summary>
            /// 客户端命令
            /// </summary>
            /// <typeparam name="commandType">客户端命令类型</typeparam>
            /// <typeparam name="callbackType">回调数据类型</typeparam>
            private abstract class asyncIdentityCommand<commandType, callbackType> : asyncCommand<commandType, callbackType>
                where commandType : asyncIdentityCommand<commandType, callbackType>
            {
                /// <summary>
                /// TCP调用命令
                /// </summary>
                public identityCommand Command;
                /// <summary>
                /// 创建第一个命令输入数据
                /// </summary>
                /// <param name="stream">命令内存流</param>
                /// <returns>数据起始位置,失败返回0</returns>
                public unsafe override int BuildIndex(unmanagedStream stream)
                {
                    stream.PrepLength(sizeof(int) * 3 + sizeof(commandServer.streamIdentity));
                    byte* write = stream.CurrentData;
                    *(int*)(write) = Command.Command;
                    *(int*)(write + sizeof(int)) = (int)(uint)Flags;
                    *(commandServer.streamIdentity*)(write + sizeof(int) * 2) = Identity;
                    *(int*)(write + (sizeof(int) * 2 + sizeof(commandServer.streamIdentity))) = 0;
                    stream.UnsafeAddLength(sizeof(int) * 3 + sizeof(commandServer.streamIdentity));
                    return stream.Length;
                }
            }
            /// <summary>
            /// 客户端命令
            /// </summary>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            private sealed class asyncOutputIdentityCommand<outputParameterType> : asyncIdentityCommand<asyncOutputIdentityCommand<outputParameterType>, fastCSharp.net.returnValue<outputParameterType>>
            {
                /// <summary>
                /// 输出参数
                /// </summary>
                public outputParameterType OutputParameter;
                /// <summary>
                /// 输出参数访问锁
                /// </summary>
                private readonly object outputLock = new object();
                /// <summary>
                /// 接收数据回调处理
                /// </summary>
                /// <param name="data">输出数据</param>
                internal override void OnReceive(ref memoryPool.pushSubArray data)
                {
                    returnValue.type returnType = GetReturnType(ref data.Value);
                    if (KeepCallback == null)
                    {
                        if ((Flags & commandServer.commandFlags.JsonSerialize) == 0)
                        {
                            try
                            {
                                if (returnType == returnValue.type.ClientDeSerializeError && fastCSharp.emit.dataDeSerializer.DeSerialize(ref data.Value, ref OutputParameter))
                                {
                                    returnType = returnValue.type.Success;
                                }
                            }
                            finally
                            {
                                if (returnType == returnValue.type.ServerDeSerializeError && Socket.commandPool.Pool.Length != 0 && Socket.attribute.IsTryJsonSerializable)
                                {
                                    Flags |= commandServer.commandFlags.JsonSerialize;
                                    PushCommand();
                                }
                                else
                                {
                                    outputParameterType outputParameter = OutputParameter;
                                    OutputParameter = default(outputParameterType);
                                    push(new fastCSharp.net.returnValue<outputParameterType> { Type = returnType, Value = outputParameter });
                                    data.Push();
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                if (returnType == returnValue.type.ClientDeSerializeError)
                                {
                                    tcpBase.parameterJsonToSerialize<outputParameterType> outputParameter = new tcpBase.parameterJsonToSerialize<outputParameterType> { Return = OutputParameter };
                                    if (fastCSharp.emit.dataDeSerializer.DeSerialize(ref data.Value, ref outputParameter))
                                    {
                                        OutputParameter = outputParameter.Return;
                                        returnType = returnValue.type.Success;
                                    }
                                }
                            }
                            finally
                            {
                                outputParameterType outputParameter = OutputParameter;
                                OutputParameter = default(outputParameterType);
                                push(new fastCSharp.net.returnValue<outputParameterType> { Type = returnType, Value = outputParameter });
                                data.Push();
                            }
                        }
                    }
                    else if (returnType == returnValue.type.ClientDeSerializeError)
                    {
                        Monitor.Enter(outputLock);
                        outputParameterType outputParameter = OutputParameter;
                        try
                        {
                            if ((Flags & commandServer.commandFlags.JsonSerialize) == 0)
                            {
                                if (fastCSharp.emit.dataDeSerializer.DeSerialize(ref data.Value, ref outputParameter)) returnType = returnValue.type.Success;
                            }
                            else
                            {
                                tcpBase.parameterJsonToSerialize<outputParameterType> jsonOutputParameter = new tcpBase.parameterJsonToSerialize<outputParameterType> { Return = outputParameter };
                                if (fastCSharp.emit.dataDeSerializer.DeSerialize(ref data.Value, ref jsonOutputParameter))
                                {
                                    outputParameter = jsonOutputParameter.Return;
                                    returnType = returnValue.type.Success;
                                }
                            }
                        }
                        finally
                        {
                            Monitor.Exit(outputLock);
                            onlyCallback(new fastCSharp.net.returnValue<outputParameterType> { Type = returnType, Value = outputParameter });
                            data.Push();
                        }
                    }
                    else
                    {
                        onlyCallback(new fastCSharp.net.returnValue<outputParameterType> { Type = returnType });
                    }
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="onGet">回调委托</param>
                /// <param name="callback">异步回调</param>
                /// <param name="command">命令信息</param>
                /// <param name="flags">命令参数</param>
                /// <param name="outputParameter">输出参数</param>
                /// <param name="isTask">回调是否使用任务池</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, Action<returnValue<outputParameterType>> onGet, callback<returnValue<outputParameterType>> callback
                    , identityCommand command, commandServer.commandFlags flags, ref outputParameterType outputParameter, bool isTask)
                {
                    Socket = socket;
                    Callback = onGet;
                    CallbackValue = callback;
                    Command = command;
                    Flags = flags;
                    OutputParameter = outputParameter;
                    IsSendOnly = command.IsSendOnly;
                    IsTask = isTask ? (byte)1 : (byte)0;
                    if (socket.newIndex(this, command.IsKeepCallback) && (command.IsKeepCallback == 0 || SetKeepCallback())) return true;
                    OutputParameter = default(outputParameterType);
                    Callback = null;
                    CallbackValue = null;
                    push();
                    return false;
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="onGet">回调委托</param>
                /// <param name="command">命令信息</param>
                /// <param name="flags">命令参数</param>
                /// <param name="outputParameter">输出参数</param>
                /// <param name="isTask">回调是否使用任务池</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, Action<returnValue<outputParameterType>> onGet
                    , identityCommand command, commandServer.commandFlags flags, ref outputParameterType outputParameter, bool isTask)
                {
                    Socket = socket;
                    Callback = onGet;
                    Command = command;
                    Flags = flags;
                    OutputParameter = outputParameter;
                    IsSendOnly = command.IsSendOnly;
                    IsTask = isTask ? (byte)1 : (byte)0;
                    if (socket.newIndex(this, command.IsKeepCallback) && (command.IsKeepCallback == 0 || SetKeepCallback())) return true;
                    OutputParameter = default(outputParameterType);
                    Callback = null;
                    push();
                    return false;
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="callback">异步回调</param>
                /// <param name="command">命令信息</param>
                /// <param name="flags">命令参数</param>
                /// <param name="outputParameter">输出参数</param>
                /// <param name="isTask">回调是否使用任务池</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, callback<returnValue<outputParameterType>> callback
                    , identityCommand command, commandServer.commandFlags flags, ref outputParameterType outputParameter, bool isTask)
                {
                    Socket = socket;
                    CallbackValue = callback;
                    Command = command;
                    Flags = flags;
                    OutputParameter = outputParameter;
                    IsSendOnly = command.IsSendOnly;
                    IsTask = isTask ? (byte)1 : (byte)0;
                    if (socket.newIndex(this, command.IsKeepCallback) && (command.IsKeepCallback == 0 || SetKeepCallback())) return true;
                    OutputParameter = default(outputParameterType);
                    CallbackValue = null;
                    push();
                    return false;
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <returns>客户端命令</returns>
                public static asyncOutputIdentityCommand<outputParameterType> Get()
                {
                    asyncOutputIdentityCommand<outputParameterType> command = typePool<asyncOutputIdentityCommand<outputParameterType>>.Pop();
                    if (command == null)
                    {
                        try
                        {
                            command = new asyncOutputIdentityCommand<outputParameterType>();
                        }
                        catch (Exception error)
                        {
                            log.Error.Add(error, null, false);
                        }
                    }
                    return command;
                }
            }
            /// <summary>
            /// TCP调用并返回参数值
            /// </summary>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            /// <param name="onGet">回调委托,返回null表示失败</param>
            /// <param name="callback">异步回调</param>
            /// <param name="identityCommand">命令信息</param>
            /// <param name="outputParameter">输出参数</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback Get<outputParameterType>
                (Action<fastCSharp.net.returnValue<outputParameterType>> onGet, callback<fastCSharp.net.returnValue<outputParameterType>> callback, identityCommand identityCommand
                , ref outputParameterType outputParameter, bool isTask)
            {
                fastCSharp.net.returnValue<outputParameterType> returnType;
                if (isDisposed == 0)
                {
                    asyncOutputIdentityCommand<outputParameterType> command = asyncOutputIdentityCommand<outputParameterType>.Get();
                    if (command != null && command.Set(this, onGet, callback, identityCommand, default(commandServer.commandFlags), ref outputParameter, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnType = new returnValue<outputParameterType> { Type = returnValue.type.ClientException };
                }
                else returnType = new returnValue<outputParameterType> { Type = returnValue.type.ClientDisposed };
                if (callback != null) callback.Callback(ref returnType);
                else if (onGet != null) onGet(returnType);
                return null;
            }
            /// <summary>
            /// TCP调用并返回参数值
            /// </summary>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            /// <param name="onGet">回调委托,返回null表示失败</param>
            /// <param name="identityCommand">命令信息</param>
            /// <param name="outputParameter">输出参数</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback Get<outputParameterType>
                (Action<fastCSharp.net.returnValue<outputParameterType>> onGet, identityCommand identityCommand
                , ref outputParameterType outputParameter, bool isTask)
            {
                returnValue.type returnType;
                if (isDisposed == 0)
                {
                    asyncOutputIdentityCommand<outputParameterType> command = asyncOutputIdentityCommand<outputParameterType>.Get();
                    if (command != null && command.Set(this, onGet, identityCommand, default(commandServer.commandFlags), ref outputParameter, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnType = returnValue.type.ClientException;
                }
                else returnType = returnValue.type.ClientDisposed;
                if (onGet != null) onGet(new fastCSharp.net.returnValue<outputParameterType> { Type = returnType });
                return null;
            }
            /// <summary>
            /// TCP调用并返回参数值
            /// </summary>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            /// <param name="callback">异步回调</param>
            /// <param name="identityCommand">命令信息</param>
            /// <param name="outputParameter">输出参数</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback Get<outputParameterType>
                (callback<fastCSharp.net.returnValue<outputParameterType>> callback, identityCommand identityCommand
                , ref outputParameterType outputParameter, bool isTask)
            {
                fastCSharp.net.returnValue<outputParameterType> returnType;
                if (isDisposed == 0)
                {
                    asyncOutputIdentityCommand<outputParameterType> command = asyncOutputIdentityCommand<outputParameterType>.Get();
                    if (command != null && command.Set(this, callback, identityCommand, default(commandServer.commandFlags), ref outputParameter, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnType = new returnValue<outputParameterType> { Type = returnValue.type.ClientException };
                }
                else returnType = new returnValue<outputParameterType> { Type = returnValue.type.ClientDisposed };
                if (callback != null) callback.Callback(ref returnType);
                return null;
            }
            /// <summary>
            /// TCP调用并返回参数值
            /// </summary>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            /// <param name="onGet">回调委托,返回null表示失败</param>
            /// <param name="callback">异步回调</param>
            /// <param name="identityCommand">命令信息</param>
            /// <param name="outputParameter">输出参数</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback GetJson<outputParameterType>
                (Action<fastCSharp.net.returnValue<outputParameterType>> onGet, callback<fastCSharp.net.returnValue<outputParameterType>> callback
                , identityCommand identityCommand, ref outputParameterType outputParameter, bool isTask)
            {
                fastCSharp.net.returnValue<outputParameterType> returnType;
                if (isDisposed == 0)
                {
                    asyncOutputIdentityCommand<outputParameterType> command = asyncOutputIdentityCommand<outputParameterType>.Get();
                    if (command != null && command.Set(this, onGet, callback, identityCommand, commandServer.commandFlags.JsonSerialize, ref outputParameter, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnType = new returnValue<outputParameterType> { Type = returnValue.type.ClientException };
                }
                else returnType = new returnValue<outputParameterType> { Type = returnValue.type.ClientDisposed };
                if (callback != null) callback.Callback(ref returnType);
                else if (onGet != null) onGet(returnType);
                return null;
            }
            /// <summary>
            /// TCP调用并返回参数值
            /// </summary>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            /// <param name="onGet">回调委托,返回null表示失败</param>
            /// <param name="identityCommand">命令信息</param>
            /// <param name="outputParameter">输出参数</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback GetJson<outputParameterType>
                (Action<fastCSharp.net.returnValue<outputParameterType>> onGet
                , identityCommand identityCommand, ref outputParameterType outputParameter, bool isTask)
            {
                returnValue.type returnType;
                if (isDisposed == 0)
                {
                    asyncOutputIdentityCommand<outputParameterType> command = asyncOutputIdentityCommand<outputParameterType>.Get();
                    if (command != null && command.Set(this, onGet, identityCommand, commandServer.commandFlags.JsonSerialize, ref outputParameter, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnType = returnValue.type.ClientException;
                }
                else returnType = returnValue.type.ClientDisposed;
                if (onGet != null) onGet(new fastCSharp.net.returnValue<outputParameterType> { Type = returnType });
                return null;
            }
            /// <summary>
            /// TCP调用并返回参数值
            /// </summary>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            /// <param name="callback">异步回调</param>
            /// <param name="identityCommand">命令信息</param>
            /// <param name="outputParameter">输出参数</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback GetJson<outputParameterType>
                (callback<fastCSharp.net.returnValue<outputParameterType>> callback
                , identityCommand identityCommand, ref outputParameterType outputParameter, bool isTask)
            {
                fastCSharp.net.returnValue<outputParameterType> returnType;
                if (isDisposed == 0)
                {
                    asyncOutputIdentityCommand<outputParameterType> command = asyncOutputIdentityCommand<outputParameterType>.Get();
                    if (command != null && command.Set(this, callback, identityCommand, commandServer.commandFlags.JsonSerialize, ref outputParameter, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnType = new returnValue<outputParameterType> { Type = returnValue.type.ClientException };
                }
                else returnType = new returnValue<outputParameterType> { Type = returnValue.type.ClientDisposed };
                if (callback != null) callback.Callback(ref returnType);
                return null;
            }
            /// <summary>
            /// 客户端命令
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            private sealed class asyncInputOutputIdentityCommand<inputParameterType, outputParameterType> : asyncInputIdentityCommand<inputParameterType, fastCSharp.net.returnValue<outputParameterType>, asyncInputOutputIdentityCommand<inputParameterType, outputParameterType>>
            {
                /// <summary>
                /// 输出参数
                /// </summary>
                public outputParameterType OutputParameter;
                /// <summary>
                /// 输出参数访问锁
                /// </summary>
                private readonly object outputLock = new object();
                /// <summary>
                /// 接收数据回调处理
                /// </summary>
                /// <param name="data">输出数据</param>
                internal override void OnReceive(ref memoryPool.pushSubArray data)
                {
                    returnValue.type returnType = GetReturnType(ref data.Value);
                    if (KeepCallback == null)
                    {
                        if ((Flags & commandServer.commandFlags.JsonSerialize) == 0)
                        {
                            try
                            {
                                if (returnType == returnValue.type.ClientDeSerializeError && fastCSharp.emit.dataDeSerializer.DeSerialize(ref data.Value, ref OutputParameter))
                                {
                                    returnType = returnValue.type.Success;
                                }
                            }
                            finally
                            {
                                if (returnType == returnValue.type.ServerDeSerializeError && Socket.commandPool.Pool.Length != 0 && Socket.attribute.IsTryJsonSerializable)
                                {
                                    Flags |= commandServer.commandFlags.JsonSerialize;
                                    PushCommand();
                                }
                                else
                                {
                                    outputParameterType outputParameter = OutputParameter;
                                    InputParameter = default(inputParameterType);
                                    OutputParameter = default(outputParameterType);
                                    push(new fastCSharp.net.returnValue<outputParameterType> { Type = returnType, Value = outputParameter });
                                    data.Push();
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                if (returnType == returnValue.type.ClientDeSerializeError)
                                {
                                    tcpBase.parameterJsonToSerialize<outputParameterType> outputParameter = new tcpBase.parameterJsonToSerialize<outputParameterType> { Return = OutputParameter };
                                    if (fastCSharp.emit.dataDeSerializer.DeSerialize(ref data.Value, ref outputParameter))
                                    {
                                        OutputParameter = outputParameter.Return;
                                        returnType = returnValue.type.Success;
                                    }
                                }
                            }
                            finally
                            {
                                outputParameterType outputParameter = OutputParameter;
                                InputParameter = default(inputParameterType);
                                OutputParameter = default(outputParameterType);
                                push(new fastCSharp.net.returnValue<outputParameterType> { Type = returnType, Value = outputParameter });
                                data.Push();
                            }
                        }
                    }
                    else if (returnType == returnValue.type.ClientDeSerializeError)
                    {
                        Monitor.Enter(outputLock);
                        outputParameterType outputParameter = OutputParameter;
                        try
                        {
                            if ((Flags & commandServer.commandFlags.JsonSerialize) == 0)
                            {
                                if (fastCSharp.emit.dataDeSerializer.DeSerialize(ref data.Value, ref outputParameter))
                                {
                                    returnType = returnValue.type.Success;
                                }
                            }
                            else
                            {
                                tcpBase.parameterJsonToSerialize<outputParameterType> jsonOutputParameter = new tcpBase.parameterJsonToSerialize<outputParameterType> { Return = outputParameter };
                                if (fastCSharp.emit.dataDeSerializer.DeSerialize(ref data.Value, ref jsonOutputParameter))
                                {
                                    outputParameter = jsonOutputParameter.Return;
                                    returnType = returnValue.type.Success;
                                }
                            }
                        }
                        finally
                        {
                            Monitor.Exit(outputLock);
                            onlyCallback(new fastCSharp.net.returnValue<outputParameterType> { Type = returnType, Value = outputParameter });
                            data.Push();
                        }
                    }
                    else
                    {
                        onlyCallback(new fastCSharp.net.returnValue<outputParameterType> { Type = returnType });
                    }
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="onGet">回调委托</param>
                /// <param name="callback">异步回调</param>
                /// <param name="command">命令信息</param>
                /// <param name="flags">命令参数</param>
                /// <param name="inputParameter">输入参数</param>
                /// <param name="outputParameter">输出参数</param>
                /// <param name="isTask">回调是否使用任务池</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, Action<returnValue<outputParameterType>> onGet, callback<returnValue<outputParameterType>> callback, identityCommand command
                    , ref inputParameterType inputParameter, commandServer.commandFlags flags, ref outputParameterType outputParameter
                    , bool isTask)
                {
                    Socket = socket;
                    Callback = onGet;
                    CallbackValue = callback;
                    Command = command;
                    Flags = flags;
                    InputParameter = inputParameter;
                    OutputParameter = outputParameter;
                    IsSendOnly = command.IsSendOnly;
                    IsTask = isTask ? (byte)1 : (byte)0;
                    if (socket.newIndex(this, command.IsKeepCallback) && (command.IsKeepCallback == 0 || SetKeepCallback())) return true;
                    InputParameter = default(inputParameterType);
                    OutputParameter = default(outputParameterType);
                    Callback = null;
                    CallbackValue = null;
                    push();
                    return false;
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="onGet">回调委托</param>
                /// <param name="command">命令信息</param>
                /// <param name="flags">命令参数</param>
                /// <param name="inputParameter">输入参数</param>
                /// <param name="outputParameter">输出参数</param>
                /// <param name="isTask">回调是否使用任务池</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, Action<returnValue<outputParameterType>> onGet, identityCommand command
                    , ref inputParameterType inputParameter, commandServer.commandFlags flags, ref outputParameterType outputParameter
                    , bool isTask)
                {
                    Socket = socket;
                    Callback = onGet;
                    Command = command;
                    Flags = flags;
                    InputParameter = inputParameter;
                    OutputParameter = outputParameter;
                    IsSendOnly = command.IsSendOnly;
                    IsTask = isTask ? (byte)1 : (byte)0;
                    if (socket.newIndex(this, command.IsKeepCallback) && (command.IsKeepCallback == 0 || SetKeepCallback())) return true;
                    InputParameter = default(inputParameterType);
                    OutputParameter = default(outputParameterType);
                    Callback = null;
                    push();
                    return false;
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="callback">异步回调</param>
                /// <param name="command">命令信息</param>
                /// <param name="flags">命令参数</param>
                /// <param name="inputParameter">输入参数</param>
                /// <param name="outputParameter">输出参数</param>
                /// <param name="isTask">回调是否使用任务池</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, callback<returnValue<outputParameterType>> callback, identityCommand command
                    , ref inputParameterType inputParameter, commandServer.commandFlags flags, ref outputParameterType outputParameter
                    , bool isTask)
                {
                    Socket = socket;
                    CallbackValue = callback;
                    Command = command;
                    Flags = flags;
                    InputParameter = inputParameter;
                    OutputParameter = outputParameter;
                    IsSendOnly = command.IsSendOnly;
                    IsTask = isTask ? (byte)1 : (byte)0;
                    if (socket.newIndex(this, command.IsKeepCallback) && (command.IsKeepCallback == 0 || SetKeepCallback())) return true;
                    InputParameter = default(inputParameterType);
                    OutputParameter = default(outputParameterType);
                    CallbackValue = null;
                    push();
                    return false;
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <returns>客户端命令</returns>
                public static asyncInputOutputIdentityCommand<inputParameterType, outputParameterType> Get()
                {
                    asyncInputOutputIdentityCommand<inputParameterType, outputParameterType> command = typePool<asyncInputOutputIdentityCommand<inputParameterType, outputParameterType>>.Pop();
                    if (command == null)
                    {
                        try
                        {
                            command = new asyncInputOutputIdentityCommand<inputParameterType, outputParameterType>();
                        }
                        catch (Exception error)
                        {
                            log.Error.Add(error, null, false);
                        }
                    }
                    return command;
                }
            }
            /// <summary>
            /// TCP调用并返回参数值
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            /// <param name="onGet">回调委托,返回null表示失败</param>
            /// <param name="callback">异步回调</param>
            /// <param name="identityCommand">命令信息</param>
            /// <param name="inputParameter">输入参数</param>
            /// <param name="outputParameter">输出参数</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback Get<inputParameterType, outputParameterType>
                (Action<fastCSharp.net.returnValue<outputParameterType>> onGet, callback<fastCSharp.net.returnValue<outputParameterType>> callback, identityCommand identityCommand, ref inputParameterType inputParameter
                , ref outputParameterType outputParameter, bool isTask)
            {
                fastCSharp.net.returnValue<outputParameterType> returnType;
                if (isDisposed == 0)
                {
                    asyncInputOutputIdentityCommand<inputParameterType, outputParameterType> command = asyncInputOutputIdentityCommand<inputParameterType, outputParameterType>.Get();
                    if (command != null && command.Set(this, onGet, callback, identityCommand, ref inputParameter, default(commandServer.commandFlags), ref outputParameter, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnType = new returnValue<outputParameterType> { Type = returnValue.type.ClientException };
                }
                else returnType = new returnValue<outputParameterType> { Type = returnValue.type.ClientDisposed };
                if (callback != null) callback.Callback(ref returnType);
                else if (onGet != null) onGet(returnType);
                return null;
            }
            /// <summary>
            /// TCP调用并返回参数值
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            /// <param name="onGet">回调委托,返回null表示失败</param>
            /// <param name="identityCommand">命令信息</param>
            /// <param name="inputParameter">输入参数</param>
            /// <param name="outputParameter">输出参数</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback Get<inputParameterType, outputParameterType>
                (Action<fastCSharp.net.returnValue<outputParameterType>> onGet, identityCommand identityCommand, ref inputParameterType inputParameter
                , ref outputParameterType outputParameter, bool isTask)
            {
                returnValue.type returnType;
                if (isDisposed == 0)
                {
                    asyncInputOutputIdentityCommand<inputParameterType, outputParameterType> command = asyncInputOutputIdentityCommand<inputParameterType, outputParameterType>.Get();
                    if (command != null && command.Set(this, onGet, identityCommand, ref inputParameter, default(commandServer.commandFlags), ref outputParameter, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnType = returnValue.type.ClientException;
                }
                else returnType = returnValue.type.ClientDisposed;
                if (onGet != null) onGet(new fastCSharp.net.returnValue<outputParameterType> { Type = returnType });
                return null;
            }
            /// <summary>
            /// TCP调用并返回参数值
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            /// <param name="callback">异步回调</param>
            /// <param name="identityCommand">命令信息</param>
            /// <param name="inputParameter">输入参数</param>
            /// <param name="outputParameter">输出参数</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback Get<inputParameterType, outputParameterType>
                (callback<fastCSharp.net.returnValue<outputParameterType>> callback, identityCommand identityCommand, ref inputParameterType inputParameter
                , ref outputParameterType outputParameter, bool isTask)
            {
                fastCSharp.net.returnValue<outputParameterType> returnType;
                if (isDisposed == 0)
                {
                    asyncInputOutputIdentityCommand<inputParameterType, outputParameterType> command = asyncInputOutputIdentityCommand<inputParameterType, outputParameterType>.Get();
                    if (command != null && command.Set(this, callback, identityCommand, ref inputParameter, default(commandServer.commandFlags), ref outputParameter, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnType = new returnValue<outputParameterType> { Type = returnValue.type.ClientException };
                }
                else returnType = new returnValue<outputParameterType> { Type = returnValue.type.ClientDisposed };
                if (callback != null) callback.Callback(ref returnType);
                return null;
            }
            /// <summary>
            /// TCP调用并返回参数值
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            /// <param name="onGet">回调委托,返回null表示失败</param>
            /// <param name="callback">异步回调</param>
            /// <param name="identityCommand">命令信息</param>
            /// <param name="inputParameter">输入参数</param>
            /// <param name="outputParameter">输出参数</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback GetJson<inputParameterType, outputParameterType>
                (Action<fastCSharp.net.returnValue<outputParameterType>> onGet, callback<fastCSharp.net.returnValue<outputParameterType>> callback, identityCommand identityCommand
                , ref inputParameterType inputParameter, ref outputParameterType outputParameter, bool isTask)
            {
                fastCSharp.net.returnValue<outputParameterType> returnType;
                if (isDisposed == 0)
                {
                    asyncInputOutputIdentityCommand<inputParameterType, outputParameterType> command = asyncInputOutputIdentityCommand<inputParameterType, outputParameterType>.Get();
                    if (command != null && command.Set(this, onGet, callback, identityCommand, ref inputParameter, commandServer.commandFlags.JsonSerialize, ref outputParameter, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnType = new returnValue<outputParameterType> { Type = returnValue.type.ClientException };
                }
                else returnType = new returnValue<outputParameterType> { Type = returnValue.type.ClientDisposed };
                if (callback != null) callback.Callback(ref returnType);
                else if (onGet != null) onGet(returnType);
                return null;
            }
            /// <summary>
            /// TCP调用并返回参数值
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            /// <param name="onGet">回调委托,返回null表示失败</param>
            /// <param name="identityCommand">命令信息</param>
            /// <param name="inputParameter">输入参数</param>
            /// <param name="outputParameter">输出参数</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback GetJson<inputParameterType, outputParameterType>
                (Action<fastCSharp.net.returnValue<outputParameterType>> onGet, identityCommand identityCommand
                , ref inputParameterType inputParameter, ref outputParameterType outputParameter, bool isTask)
            {
                returnValue.type returnType;
                if (isDisposed == 0)
                {
                    asyncInputOutputIdentityCommand<inputParameterType, outputParameterType> command = asyncInputOutputIdentityCommand<inputParameterType, outputParameterType>.Get();
                    if (command != null && command.Set(this, onGet, identityCommand, ref inputParameter, commandServer.commandFlags.JsonSerialize, ref outputParameter, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnType = returnValue.type.ClientException;
                }
                else returnType = returnValue.type.ClientDisposed;
                if (onGet != null) onGet(new fastCSharp.net.returnValue<outputParameterType> { Type = returnType });
                return null;
            }
            /// <summary>
            /// TCP调用并返回参数值
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            /// <param name="callback">异步回调</param>
            /// <param name="identityCommand">命令信息</param>
            /// <param name="inputParameter">输入参数</param>
            /// <param name="outputParameter">输出参数</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback GetJson<inputParameterType, outputParameterType>
                (callback<fastCSharp.net.returnValue<outputParameterType>> callback, identityCommand identityCommand
                , ref inputParameterType inputParameter, ref outputParameterType outputParameter, bool isTask)
            {
                fastCSharp.net.returnValue<outputParameterType> returnValue;
                if (isDisposed == 0)
                {
                    asyncInputOutputIdentityCommand<inputParameterType, outputParameterType> command = asyncInputOutputIdentityCommand<inputParameterType, outputParameterType>.Get();
                    if (command != null && command.Set(this, callback, identityCommand, ref inputParameter, commandServer.commandFlags.JsonSerialize, ref outputParameter, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnValue = new returnValue<outputParameterType> { Type = net.returnValue.type.ClientException };
                }
                else returnValue = new returnValue<outputParameterType> { Type = net.returnValue.type.ClientDisposed };
                if (callback != null) callback.Callback(ref returnValue);
                return null;
            }
            /// <summary>
            /// 客户端命令
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <typeparam name="callbackType">回调数据类型</typeparam>
            /// <typeparam name="commandType">客户端命令类型</typeparam>
            private abstract class asyncInputDataCommand<inputParameterType, callbackType, commandType> : asyncDataCommand<commandType, callbackType>
                where commandType : asyncInputDataCommand<inputParameterType, callbackType, commandType>
            {
                /// <summary>
                /// 输入参数
                /// </summary>
                public inputParameterType InputParameter;
                /// <summary>
                /// 创建第一个命令输入数据
                /// </summary>
                /// <param name="stream">命令内存流</param>
                /// <returns>数据起始位置,失败返回0</returns>
                public unsafe override int BuildIndex(unmanagedStream stream)
                {
                    byte[] command = Command.Command;
                    int streamLength = stream.Length, commandLength = command.Length + sizeof(commandServer.streamIdentity) + sizeof(int) * 2;
                    stream.PrepLength(commandLength);
                    stream.UnsafeAddLength(commandLength);
                    int serializeIndex = stream.Length;
                    if ((Flags & commandServer.commandFlags.JsonSerialize) == 0)
                    {
                        fastCSharp.emit.dataSerializer.Serialize(InputParameter, stream);
                    }
                    else
                    {
                        fastCSharp.emit.dataSerializer.Serialize(new tcpBase.parameterJsonToSerialize<inputParameterType> { Return = InputParameter }, stream);
                    }
                    int dataLength = stream.Length - serializeIndex;
                    if (KeepCallback != null) InputParameter = default(inputParameterType);
                    if (dataLength <= Command.MaxInputSize)
                    {
                        byte* write = stream.data.Byte + streamLength;
                        unsafer.memory.Copy(command, write, command.Length);
                        *(int*)(write += command.Length) = (int)(uint)Flags;
                        *(commandServer.streamIdentity*)(write + sizeof(int)) = Identity;
                        *(int*)(write + (sizeof(int) + sizeof(commandServer.streamIdentity))) = dataLength;
                        return stream.Length - dataLength;
                    }
                    stream.UnsafeSetLength(streamLength);
                    return 0;
                }
            }
            /// <summary>
            /// 客户端命令
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            private sealed class asyncInputDataCommand<inputParameterType> : asyncInputDataCommand<inputParameterType, fastCSharp.net.returnValue, asyncInputDataCommand<inputParameterType>>
            {
                /// <summary>
                /// 接收数据回调处理
                /// </summary>
                /// <param name="data">输出数据</param>
                internal override void OnReceive(ref memoryPool.pushSubArray data)
                {
                    if (KeepCallback == null)
                    {
                        returnValue.type returnType = GetReturnType(ref data.Value);
                        if (returnType == returnValue.type.ServerDeSerializeError)
                        {
                            if ((Flags & commandServer.commandFlags.JsonSerialize) == 0 && Socket.commandPool.Pool.Length != 0 && Socket.attribute.IsTryJsonSerializable)
                            {
                                Flags |= commandServer.commandFlags.JsonSerialize;
                                PushCommand();
                                return;
                            }
                        }
                        InputParameter = default(inputParameterType);
                        push(new fastCSharp.net.returnValue { Type = returnType });
                    }
                    else onlyCallback(new fastCSharp.net.returnValue { Type = GetReturnType(ref data.Value) });
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="onCall">回调委托</param>
                /// <param name="callback">异步回调</param>
                /// <param name="command">命令信息</param>
                /// <param name="flags">命令参数</param>
                /// <param name="inputParameter">输入参数</param>
                /// <param name="isTask">客户端回调是否使用任务池</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, Action<returnValue> onCall, callback<returnValue> callback
                    , dataCommand command, commandServer.commandFlags flags, ref inputParameterType inputParameter, bool isTask)
                {
                    Socket = socket;
                    Callback = onCall;
                    CallbackValue = callback;
                    Command = command;
                    Flags = flags;
                    InputParameter = inputParameter;
                    IsSendOnly = command.IsSendOnly;
                    IsTask = isTask ? (byte)1 : (byte)0;
                    if (socket.newIndex(this, command.IsKeepCallback) && (command.IsKeepCallback == 0 || SetKeepCallback())) return true;
                    InputParameter = default(inputParameterType);
                    Callback = null;
                    CallbackValue = null;
                    push();
                    return false;
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="onCall">回调委托</param>
                /// <param name="command">命令信息</param>
                /// <param name="flags">命令参数</param>
                /// <param name="inputParameter">输入参数</param>
                /// <param name="isTask">客户端回调是否使用任务池</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, Action<returnValue> onCall
                    , dataCommand command, commandServer.commandFlags flags, ref inputParameterType inputParameter, bool isTask)
                {
                    Socket = socket;
                    Callback = onCall;
                    Command = command;
                    Flags = flags;
                    InputParameter = inputParameter;
                    IsSendOnly = command.IsSendOnly;
                    IsTask = isTask ? (byte)1 : (byte)0;
                    if (socket.newIndex(this, command.IsKeepCallback) && (command.IsKeepCallback == 0 || SetKeepCallback())) return true;
                    InputParameter = default(inputParameterType);
                    Callback = null;
                    push();
                    return false;
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="callback">异步回调</param>
                /// <param name="command">命令信息</param>
                /// <param name="flags">命令参数</param>
                /// <param name="inputParameter">输入参数</param>
                /// <param name="isTask">客户端回调是否使用任务池</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, callback<returnValue> callback
                    , dataCommand command, commandServer.commandFlags flags, ref inputParameterType inputParameter, bool isTask)
                {
                    Socket = socket;
                    CallbackValue = callback;
                    Command = command;
                    Flags = flags;
                    InputParameter = inputParameter;
                    IsSendOnly = command.IsSendOnly;
                    IsTask = isTask ? (byte)1 : (byte)0;
                    if (socket.newIndex(this, command.IsKeepCallback) && (command.IsKeepCallback == 0 || SetKeepCallback())) return true;
                    InputParameter = default(inputParameterType);
                    CallbackValue = null;
                    push();
                    return false;
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <returns>客户端命令</returns>
                public static asyncInputDataCommand<inputParameterType> Get()
                {
                    asyncInputDataCommand<inputParameterType> command = typePool<asyncInputDataCommand<inputParameterType>>.Pop();
                    if (command == null)
                    {
                        try
                        {
                            command = new asyncInputDataCommand<inputParameterType>();
                        }
                        catch (Exception error)
                        {
                            log.Error.Add(error, null, false);
                        }
                    }
                    return command;
                }
            }
            /// <summary>
            /// TCP调用
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <param name="onCall">回调委托</param>
            /// <param name="callback">异步回调</param>
            /// <param name="dataCommand">命令信息</param>
            /// <param name="inputParameter">输入参数</param>
            /// <param name="isTask">客户端回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback Call<inputParameterType>
                (Action<fastCSharp.net.returnValue> onCall, callback<fastCSharp.net.returnValue> callback, dataCommand dataCommand, ref inputParameterType inputParameter, bool isTask)
            {
                fastCSharp.net.returnValue returnValue;
                if (isDisposed == 0)
                {
                    asyncInputDataCommand<inputParameterType> command = asyncInputDataCommand<inputParameterType>.Get();
                    if (command != null && command.Set(this, onCall, callback, dataCommand, default(commandServer.commandFlags), ref inputParameter, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnValue.Type = returnValue.type.ClientException; 
                }
                else returnValue.Type = returnValue.type.ClientDisposed;
                if (callback != null) callback.Callback(ref returnValue);
                else if (onCall != null) onCall(returnValue);
                return null;
            }
            /// <summary>
            /// TCP调用
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <param name="onCall">回调委托</param>
            /// <param name="dataCommand">命令信息</param>
            /// <param name="inputParameter">输入参数</param>
            /// <param name="isTask">客户端回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback Call<inputParameterType>
                (Action<fastCSharp.net.returnValue> onCall, dataCommand dataCommand, ref inputParameterType inputParameter, bool isTask)
            {
                returnValue.type returnType;
                if (isDisposed == 0)
                {
                    asyncInputDataCommand<inputParameterType> command = asyncInputDataCommand<inputParameterType>.Get();
                    if (command != null && command.Set(this, onCall, dataCommand, default(commandServer.commandFlags), ref inputParameter, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnType = returnValue.type.ClientException;
                }
                else returnType = returnValue.type.ClientDisposed;
                if (onCall != null) onCall(new fastCSharp.net.returnValue { Type = returnType });
                return null;
            }
            /// <summary>
            /// TCP调用
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <param name="callback">异步回调</param>
            /// <param name="dataCommand">命令信息</param>
            /// <param name="inputParameter">输入参数</param>
            /// <param name="isTask">客户端回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback Call<inputParameterType>
                (callback<fastCSharp.net.returnValue> callback, dataCommand dataCommand, ref inputParameterType inputParameter, bool isTask)
            {
                fastCSharp.net.returnValue returnValue;
                if (isDisposed == 0)
                {
                    asyncInputDataCommand<inputParameterType> command = asyncInputDataCommand<inputParameterType>.Get();
                    if (command != null && command.Set(this, callback, dataCommand, default(commandServer.commandFlags), ref inputParameter, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnValue.Type = returnValue.type.ClientException;
                }
                else returnValue.Type = returnValue.type.ClientDisposed;
                if (callback != null) callback.Callback(ref returnValue);
                return null;
            }
            /// <summary>
            /// TCP调用
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <param name="onCall">回调委托</param>
            /// <param name="callback">异步回调</param>
            /// <param name="dataCommand">命令信息</param>
            /// <param name="inputParameter">输入参数</param>
            /// <param name="isTask">客户端回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public keepCallback CallJson<inputParameterType>
                (Action<fastCSharp.net.returnValue> onCall, callback<fastCSharp.net.returnValue> callback, dataCommand dataCommand, ref inputParameterType inputParameter, bool isTask)
            {
                fastCSharp.net.returnValue returnValue;
                if (isDisposed == 0)
                {
                    asyncInputDataCommand<inputParameterType> command = asyncInputDataCommand<inputParameterType>.Get();
                    if (command != null && command.Set(this, onCall, callback, dataCommand, commandServer.commandFlags.JsonSerialize, ref inputParameter, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnValue.Type = returnValue.type.ClientException; 
                }
                else returnValue.Type = returnValue.type.ClientDisposed;
                if (callback != null) callback.Callback(ref returnValue);
                else if (onCall != null) onCall(returnValue);
                return null;
            }
            /// <summary>
            /// TCP调用
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <param name="onCall">回调委托</param>
            /// <param name="dataCommand">命令信息</param>
            /// <param name="inputParameter">输入参数</param>
            /// <param name="isTask">客户端回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public keepCallback CallJson<inputParameterType>
                (Action<fastCSharp.net.returnValue> onCall, dataCommand dataCommand, ref inputParameterType inputParameter, bool isTask)
            {
                returnValue.type returnType;
                if (isDisposed == 0)
                {
                    asyncInputDataCommand<inputParameterType> command = asyncInputDataCommand<inputParameterType>.Get();
                    if (command != null && command.Set(this, onCall, dataCommand, commandServer.commandFlags.JsonSerialize, ref inputParameter, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnType = returnValue.type.ClientException;
                }
                else returnType = returnValue.type.ClientDisposed;
                if (onCall != null) onCall(new fastCSharp.net.returnValue { Type = returnType });
                return null;
            }
            /// <summary>
            /// TCP调用
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <param name="callback">异步回调</param>
            /// <param name="dataCommand">命令信息</param>
            /// <param name="inputParameter">输入参数</param>
            /// <param name="isTask">客户端回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public keepCallback CallJson<inputParameterType>
                (callback<fastCSharp.net.returnValue> callback, dataCommand dataCommand, ref inputParameterType inputParameter, bool isTask)
            {
                fastCSharp.net.returnValue returnValue;
                if (isDisposed == 0)
                {
                    asyncInputDataCommand<inputParameterType> command = asyncInputDataCommand<inputParameterType>.Get();
                    if (command != null && command.Set(this, callback, dataCommand, commandServer.commandFlags.JsonSerialize, ref inputParameter, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnValue.Type = returnValue.type.ClientException;
                }
                else returnValue.Type = returnValue.type.ClientDisposed;
                if (callback != null) callback.Callback(ref returnValue);
                return null;
            }
            /// <summary>
            /// 客户端命令
            /// </summary>
            private sealed class asyncDataCommand : asyncDataCommand<asyncDataCommand, fastCSharp.net.returnValue>
            {
                /// <summary>
                /// 接收数据回调处理
                /// </summary>
                /// <param name="data">输出数据</param>
                internal override void OnReceive(ref memoryPool.pushSubArray data)
                {
                    if (KeepCallback == null) push(new fastCSharp.net.returnValue { Type = GetReturnType(ref data.Value) });
                    else onlyCallback(new fastCSharp.net.returnValue { Type = GetReturnType(ref data.Value) });
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="onCall">回调委托</param>
                /// <param name="callback">异步回调</param>
                /// <param name="command">命令信息</param>
                /// <param name="flags">命令参数</param>
                /// <param name="isTask">客户端回调是否使用任务池</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, Action<returnValue> onCall, callback<returnValue> callback, dataCommand command, commandServer.commandFlags flags, bool isTask)
                {
                    Socket = socket;
                    Callback = onCall;
                    CallbackValue = callback;
                    Command = command;
                    Flags = flags;
                    IsSendOnly = command.IsSendOnly;
                    IsTask = isTask ? (byte)1 : (byte)0;
                    if (socket.newIndex(this, command.IsKeepCallback) && (command.IsKeepCallback == 0 || SetKeepCallback())) return true;
                    Callback = null;
                    CallbackValue = null;
                    push();
                    return false;
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="onCall">回调委托</param>
                /// <param name="command">命令信息</param>
                /// <param name="flags">命令参数</param>
                /// <param name="isTask">客户端回调是否使用任务池</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, Action<returnValue> onCall, dataCommand command, commandServer.commandFlags flags, bool isTask)
                {
                    Socket = socket;
                    Callback = onCall;
                    Command = command;
                    Flags = flags;
                    IsSendOnly = command.IsSendOnly;
                    IsTask = isTask ? (byte)1 : (byte)0;
                    if (socket.newIndex(this, command.IsKeepCallback) && (command.IsKeepCallback == 0 || SetKeepCallback())) return true;
                    Callback = null;
                    push();
                    return false;
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="callback">异步回调</param>
                /// <param name="command">命令信息</param>
                /// <param name="flags">命令参数</param>
                /// <param name="isTask">客户端回调是否使用任务池</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, callback<returnValue> callback, dataCommand command, commandServer.commandFlags flags, bool isTask)
                {
                    Socket = socket;
                    CallbackValue = callback;
                    Command = command;
                    Flags = flags;
                    IsSendOnly = command.IsSendOnly;
                    IsTask = isTask ? (byte)1 : (byte)0;
                    if (socket.newIndex(this, command.IsKeepCallback) && (command.IsKeepCallback == 0 || SetKeepCallback())) return true;
                    CallbackValue = null;
                    push();
                    return false;
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <returns>客户端命令</returns>
                public static asyncDataCommand Get()
                {
                    asyncDataCommand command = typePool<asyncDataCommand>.Pop();
                    if (command == null)
                    {
                        try
                        {
                            command = new asyncDataCommand();
                        }
                        catch (Exception error)
                        {
                            log.Error.Add(error, null, false);
                        }
                    }
                    return command;
                }
            }
            /// <summary>
            /// TCP调用
            /// </summary>
            /// <param name="onCall">回调委托</param>
            /// <param name="callback">异步回调</param>
            /// <param name="dataCommand">命令信息</param>
            /// <param name="isTask">客户端回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback Call(Action<fastCSharp.net.returnValue> onCall, callback<fastCSharp.net.returnValue> callback, dataCommand dataCommand, bool isTask)
            {
                fastCSharp.net.returnValue returnValue;
                if (isDisposed == 0)
                {
                    asyncDataCommand command = asyncDataCommand.Get();
                    if (command != null && command.Set(this, onCall, callback, dataCommand, default(commandServer.commandFlags), isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnValue.Type = returnValue.type.ClientException; 
                }
                else returnValue.Type = returnValue.type.ClientDisposed;
                if (callback != null) callback.Callback(ref returnValue);
                else if (onCall != null) onCall(returnValue);
                return null;
            }
            /// <summary>
            /// TCP调用
            /// </summary>
            /// <param name="onCall">回调委托</param>
            /// <param name="dataCommand">命令信息</param>
            /// <param name="isTask">客户端回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback Call(Action<fastCSharp.net.returnValue> onCall, dataCommand dataCommand, bool isTask)
            {
                returnValue.type returnType;
                if (isDisposed == 0)
                {
                    asyncDataCommand command = asyncDataCommand.Get();
                    if (command != null && command.Set(this, onCall, dataCommand, default(commandServer.commandFlags), isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnType = returnValue.type.ClientException;
                }
                else returnType = returnValue.type.ClientDisposed;
                if (onCall != null) onCall(new fastCSharp.net.returnValue { Type = returnType });
                return null;
            }
            /// <summary>
            /// TCP调用
            /// </summary>
            /// <param name="callback">异步回调</param>
            /// <param name="dataCommand">命令信息</param>
            /// <param name="isTask">客户端回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback Call(callback<fastCSharp.net.returnValue> callback, dataCommand dataCommand, bool isTask)
            {
                fastCSharp.net.returnValue returnValue;
                if (isDisposed == 0)
                {
                    asyncDataCommand command = asyncDataCommand.Get();
                    if (command != null && command.Set(this, callback, dataCommand, default(commandServer.commandFlags), isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnValue.Type = returnValue.type.ClientException;
                }
                else returnValue.Type = returnValue.type.ClientDisposed;
                if (callback != null) callback.Callback(ref returnValue);
                return null;
            }
            /// <summary>
            /// 客户端命令
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <typeparam name="callbackType">回调数据类型</typeparam>
            /// <typeparam name="commandType">客户端命令类型</typeparam>
            private abstract class asyncInputIdentityCommand<inputParameterType, callbackType, commandType> : asyncIdentityCommand<commandType, callbackType>
                where commandType : asyncInputIdentityCommand<inputParameterType, callbackType, commandType>
            {
                /// <summary>
                /// 输入参数
                /// </summary>
                public inputParameterType InputParameter;
                /// <summary>
                /// 创建第一个命令输入数据
                /// </summary>
                /// <param name="stream">命令内存流</param>
                /// <returns>数据起始位置,失败返回0</returns>
                public unsafe override int BuildIndex(unmanagedStream stream)
                {
                    int streamLength = stream.Length;
                    stream.PrepLength(sizeof(int) * 3 + sizeof(commandServer.streamIdentity));
                    stream.UnsafeAddLength(sizeof(int) * 3 + sizeof(commandServer.streamIdentity));
                    if ((Flags & commandServer.commandFlags.JsonSerialize) == 0)
                    {
                        fastCSharp.emit.dataSerializer.Serialize(InputParameter, stream);
                    }
                    else
                    {
                        fastCSharp.emit.dataSerializer.Serialize(new tcpBase.parameterJsonToSerialize<inputParameterType> { Return = InputParameter }, stream);
                    }
                    int dataLength = stream.Length - streamLength - (sizeof(int) * 3 + sizeof(commandServer.streamIdentity));
                    if (KeepCallback != null) InputParameter = default(inputParameterType);
                    if (dataLength <= Command.MaxInputSize)
                    {
                        byte* write = stream.data.Byte + streamLength;
                        *(int*)(write) = Command.Command;
                        *(int*)(write + sizeof(int)) = (int)(uint)Flags;
                        *(commandServer.streamIdentity*)(write + sizeof(int) * 2) = Identity;
                        *(int*)(write + (sizeof(int) * 2 + sizeof(commandServer.streamIdentity))) = dataLength;
                        return stream.Length - dataLength;
                    }
                    stream.UnsafeSetLength(streamLength);
                    return 0;
                }
            }
            /// <summary>
            /// 客户端命令
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            private sealed class asyncInputIdentityCommand<inputParameterType> : asyncInputIdentityCommand<inputParameterType, fastCSharp.net.returnValue, asyncInputIdentityCommand<inputParameterType>>
            {
                /// <summary>
                /// 接收数据回调处理
                /// </summary>
                /// <param name="data">输出数据</param>
                internal override void OnReceive(ref memoryPool.pushSubArray data)
                {
                    if (KeepCallback == null)
                    {
                        returnValue.type returnType = GetReturnType(ref data.Value);
                        if (returnType == returnValue.type.ServerDeSerializeError)
                        {
                            if ((Flags & commandServer.commandFlags.JsonSerialize) == 0 && Socket.commandPool.Pool.Length != 0 && Socket.attribute.IsTryJsonSerializable)
                            {
                                Flags |= commandServer.commandFlags.JsonSerialize;
                                PushCommand();
                                return;
                            }
                        }
                        InputParameter = default(inputParameterType);
                        push(new fastCSharp.net.returnValue { Type = returnType });
                    }
                    else onlyCallback(new fastCSharp.net.returnValue { Type = GetReturnType(ref data.Value) });
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="onCall">回调委托</param>
                /// <param name="callback">异步回调</param>
                /// <param name="command">命令信息</param>
                /// <param name="inputParameter">输入参数</param>
                /// <param name="flags">命令参数</param>
                /// <param name="isTask">回调是否使用任务池</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, Action<returnValue> onCall, callback<returnValue> callback, identityCommand command
                    , ref inputParameterType inputParameter, commandServer.commandFlags flags, bool isTask)
                {
                    Socket = socket;
                    Callback = onCall;
                    CallbackValue = callback;
                    Command = command;
                    Flags = flags;
                    InputParameter = inputParameter;
                    IsSendOnly = command.IsSendOnly;
                    IsTask = isTask ? (byte)1 : (byte)0;
                    if (socket.newIndex(this, command.IsKeepCallback) && (command.IsKeepCallback == 0 || SetKeepCallback())) return true;
                    InputParameter = default(inputParameterType);
                    Callback = null;
                    CallbackValue = null;
                    push();
                    return false;
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="onCall">回调委托</param>
                /// <param name="command">命令信息</param>
                /// <param name="inputParameter">输入参数</param>
                /// <param name="flags">命令参数</param>
                /// <param name="isTask">回调是否使用任务池</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, Action<returnValue> onCall, identityCommand command
                    , ref inputParameterType inputParameter, commandServer.commandFlags flags, bool isTask)
                {
                    Socket = socket;
                    Callback = onCall;
                    Command = command;
                    Flags = flags;
                    InputParameter = inputParameter;
                    IsSendOnly = command.IsSendOnly;
                    IsTask = isTask ? (byte)1 : (byte)0;
                    if (socket.newIndex(this, command.IsKeepCallback) && (command.IsKeepCallback == 0 || SetKeepCallback())) return true;
                    InputParameter = default(inputParameterType);
                    Callback = null;
                    push();
                    return false;
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="callback">异步回调</param>
                /// <param name="command">命令信息</param>
                /// <param name="inputParameter">输入参数</param>
                /// <param name="flags">命令参数</param>
                /// <param name="isTask">回调是否使用任务池</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, callback<returnValue> callback, identityCommand command
                    , ref inputParameterType inputParameter, commandServer.commandFlags flags, bool isTask)
                {
                    Socket = socket;
                    CallbackValue = callback;
                    Command = command;
                    Flags = flags;
                    InputParameter = inputParameter;
                    IsSendOnly = command.IsSendOnly;
                    IsTask = isTask ? (byte)1 : (byte)0;
                    if (socket.newIndex(this, command.IsKeepCallback) && (command.IsKeepCallback == 0 || SetKeepCallback())) return true;
                    InputParameter = default(inputParameterType);
                    CallbackValue = null;
                    push();
                    return false;
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <returns>客户端命令</returns>
                public static asyncInputIdentityCommand<inputParameterType> Get()
                {
                    asyncInputIdentityCommand<inputParameterType> command = typePool<asyncInputIdentityCommand<inputParameterType>>.Pop();
                    if (command == null)
                    {
                        try
                        {
                            command = new asyncInputIdentityCommand<inputParameterType>();
                        }
                        catch (Exception error)
                        {
                            log.Error.Add(error, null, false);
                        }
                    }
                    return command;
                }
            }
            /// <summary>
            /// TCP调用
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <param name="onCall">回调委托</param>
            /// <param name="callback">异步回调</param>
            /// <param name="identityCommand">命令信息</param>
            /// <param name="inputParameter">输入参数</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback Call<inputParameterType>
                (Action<fastCSharp.net.returnValue> onCall, callback<fastCSharp.net.returnValue> callback, identityCommand identityCommand, ref inputParameterType inputParameter, bool isTask)
            {
                fastCSharp.net.returnValue returnValue;
                if (isDisposed == 0)
                {
                    asyncInputIdentityCommand<inputParameterType> command = asyncInputIdentityCommand<inputParameterType>.Get();
                    if (command != null && command.Set(this, onCall, callback, identityCommand, ref inputParameter, default(commandServer.commandFlags), isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnValue.Type = returnValue.type.ClientException; 
                }
                else returnValue.Type = returnValue.type.ClientDisposed;
                if (callback != null) callback.Callback(ref returnValue);
                else if (onCall != null) onCall(returnValue);
                return null;
            }
            /// <summary>
            /// TCP调用
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <param name="onCall">回调委托</param>
            /// <param name="identityCommand">命令信息</param>
            /// <param name="inputParameter">输入参数</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback Call<inputParameterType>
                (Action<fastCSharp.net.returnValue> onCall, identityCommand identityCommand, ref inputParameterType inputParameter, bool isTask)
            {
                returnValue.type returnType;
                if (isDisposed == 0)
                {
                    asyncInputIdentityCommand<inputParameterType> command = asyncInputIdentityCommand<inputParameterType>.Get();
                    if (command != null && command.Set(this, onCall, identityCommand, ref inputParameter, default(commandServer.commandFlags), isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnType = returnValue.type.ClientException;
                }
                else returnType = returnValue.type.ClientDisposed;
                if (onCall != null) onCall(new fastCSharp.net.returnValue { Type = returnType });
                return null;
            }
            /// <summary>
            /// TCP调用
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <param name="callback">异步回调</param>
            /// <param name="identityCommand">命令信息</param>
            /// <param name="inputParameter">输入参数</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback Call<inputParameterType>
                (callback<fastCSharp.net.returnValue> callback, identityCommand identityCommand, ref inputParameterType inputParameter, bool isTask)
            {
                fastCSharp.net.returnValue returnValue;
                if (isDisposed == 0)
                {
                    asyncInputIdentityCommand<inputParameterType> command = asyncInputIdentityCommand<inputParameterType>.Get();
                    if (command != null && command.Set(this, callback, identityCommand, ref inputParameter, default(commandServer.commandFlags), isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnValue.Type = returnValue.type.ClientException;
                }
                else returnValue.Type = returnValue.type.ClientDisposed;
                if (callback != null) callback.Callback(ref returnValue);
                return null;
            }
            /// <summary>
            /// TCP调用
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <param name="onCall">回调委托</param>
            /// <param name="callback">异步回调</param>
            /// <param name="identityCommand">命令信息</param>
            /// <param name="inputParameter">输入参数</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback CallJson<inputParameterType>
                (Action<fastCSharp.net.returnValue> onCall, callback<fastCSharp.net.returnValue> callback, identityCommand identityCommand, ref inputParameterType inputParameter, bool isTask)
            {
                fastCSharp.net.returnValue returnValue;
                if (isDisposed == 0)
                {
                    asyncInputIdentityCommand<inputParameterType> command = asyncInputIdentityCommand<inputParameterType>.Get();
                    if (command != null && command.Set(this, onCall, callback, identityCommand, ref inputParameter, commandServer.commandFlags.JsonSerialize, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnValue.Type = returnValue.type.ClientException; 
                }
                else returnValue.Type = returnValue.type.ClientDisposed;
                if (callback != null) callback.Callback(ref returnValue);
                else if (onCall != null) onCall(returnValue);
                return null;
            }
            /// <summary>
            /// TCP调用
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <param name="onCall">回调委托</param>
            /// <param name="identityCommand">命令信息</param>
            /// <param name="inputParameter">输入参数</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback CallJson<inputParameterType>
                (Action<fastCSharp.net.returnValue> onCall, identityCommand identityCommand, ref inputParameterType inputParameter, bool isTask)
            {
                returnValue.type returnType;
                if (isDisposed == 0)
                {
                    asyncInputIdentityCommand<inputParameterType> command = asyncInputIdentityCommand<inputParameterType>.Get();
                    if (command != null && command.Set(this, onCall, identityCommand, ref inputParameter, commandServer.commandFlags.JsonSerialize, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnType = returnValue.type.ClientException;
                }
                else returnType = returnValue.type.ClientDisposed;
                if (onCall != null) onCall(new fastCSharp.net.returnValue { Type = returnType });
                return null;
            }
            /// <summary>
            /// TCP调用
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <param name="callback">异步回调</param>
            /// <param name="identityCommand">命令信息</param>
            /// <param name="inputParameter">输入参数</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback CallJson<inputParameterType>
                (callback<fastCSharp.net.returnValue> callback, identityCommand identityCommand, ref inputParameterType inputParameter, bool isTask)
            {
                fastCSharp.net.returnValue returnValue;
                if (isDisposed == 0)
                {
                    asyncInputIdentityCommand<inputParameterType> command = asyncInputIdentityCommand<inputParameterType>.Get();
                    if (command != null && command.Set(this, callback, identityCommand, ref inputParameter, commandServer.commandFlags.JsonSerialize, isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnValue.Type = returnValue.type.ClientException;
                }
                else returnValue.Type = returnValue.type.ClientDisposed;
                if (callback != null) callback.Callback(ref returnValue);
                return null;
            }
            /// <summary>
            /// 客户端命令
            /// </summary>
            private sealed class asyncIdentityCommand : asyncIdentityCommand<asyncIdentityCommand, fastCSharp.net.returnValue>
            {
                /// <summary>
                /// 接收数据回调处理
                /// </summary>
                /// <param name="data">输出数据</param>
                internal override void OnReceive(ref memoryPool.pushSubArray data)
                {
                    if (KeepCallback == null) push(new fastCSharp.net.returnValue { Type = GetReturnType(ref data.Value) });
                    else onlyCallback(new fastCSharp.net.returnValue { Type = GetReturnType(ref data.Value) });
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="onCall">回调委托</param>
                /// <param name="callback">异步回调</param>
                /// <param name="command">命令信息</param>
                /// <param name="flags">命令参数</param>
                /// <param name="isTask">回调是否使用任务池</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, Action<returnValue> onCall, callback<returnValue> callback, identityCommand command, commandServer.commandFlags flags, bool isTask)
                {
                    Socket = socket;
                    Callback = onCall;
                    CallbackValue = callback;
                    Command = command;
                    Flags = flags;
                    IsSendOnly = command.IsSendOnly;
                    IsTask = isTask ? (byte)1 : (byte)0;
                    if (socket.newIndex(this, command.IsKeepCallback) && (command.IsKeepCallback == 0 || SetKeepCallback())) return true;
                    Callback = null;
                    CallbackValue = null;
                    push();
                    return false;
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="onCall">回调委托</param>
                /// <param name="command">命令信息</param>
                /// <param name="flags">命令参数</param>
                /// <param name="isTask">回调是否使用任务池</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, Action<returnValue> onCall, identityCommand command, commandServer.commandFlags flags, bool isTask)
                {
                    Socket = socket;
                    Callback = onCall;
                    Command = command;
                    Flags = flags;
                    IsSendOnly = command.IsSendOnly;
                    IsTask = isTask ? (byte)1 : (byte)0;
                    if (socket.newIndex(this, command.IsKeepCallback) && (command.IsKeepCallback == 0 || SetKeepCallback())) return true;
                    Callback = null;
                    push();
                    return false;
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="callback">异步回调</param>
                /// <param name="command">命令信息</param>
                /// <param name="flags">命令参数</param>
                /// <param name="isTask">回调是否使用任务池</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, callback<returnValue> callback, identityCommand command, commandServer.commandFlags flags, bool isTask)
                {
                    Socket = socket;
                    CallbackValue = callback;
                    Command = command;
                    Flags = flags;
                    IsSendOnly = command.IsSendOnly;
                    IsTask = isTask ? (byte)1 : (byte)0;
                    if (socket.newIndex(this, command.IsKeepCallback) && (command.IsKeepCallback == 0 || SetKeepCallback())) return true;
                    CallbackValue = null;
                    push();
                    return false;
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <returns>客户端命令</returns>
                public static asyncIdentityCommand Get()
                {
                    asyncIdentityCommand command = typePool<asyncIdentityCommand>.Pop();
                    if (command == null)
                    {
                        try
                        {
                            command = new asyncIdentityCommand();
                        }
                        catch (Exception error)
                        {
                            log.Error.Add(error, null, false);
                        }
                    }
                    return command;
                }
            }
            /// <summary>
            /// TCP调用
            /// </summary>
            /// <param name="onCall">回调委托</param>
            /// <param name="callback">异步回调</param>
            /// <param name="identityCommand">命令信息</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback Call(Action<fastCSharp.net.returnValue> onCall, callback<fastCSharp.net.returnValue> callback, identityCommand identityCommand, bool isTask)
            {
                fastCSharp.net.returnValue returnValue;
                if (isDisposed == 0)
                {
                    asyncIdentityCommand command = asyncIdentityCommand.Get();
                    if (command != null && command.Set(this, onCall, callback, identityCommand, default(commandServer.commandFlags), isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnValue.Type = returnValue.type.ClientException; 
                }
                else returnValue.Type = returnValue.type.ClientDisposed;
                if (callback != null) callback.Callback(ref returnValue);
                else if (onCall != null) onCall(returnValue);
                return null;
            }
            /// <summary>
            /// TCP调用
            /// </summary>
            /// <param name="onCall">回调委托</param>
            /// <param name="identityCommand">命令信息</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback Call(Action<fastCSharp.net.returnValue> onCall, identityCommand identityCommand, bool isTask)
            {
                returnValue.type returnType;
                if (isDisposed == 0)
                {
                    asyncIdentityCommand command = asyncIdentityCommand.Get();
                    if (command != null && command.Set(this, onCall, identityCommand, default(commandServer.commandFlags), isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnType = returnValue.type.ClientException;
                }
                else returnType = returnValue.type.ClientDisposed;
                if (onCall != null) onCall(new fastCSharp.net.returnValue { Type = returnType });
                return null;
            }
            /// <summary>
            /// TCP调用
            /// </summary>
            /// <param name="callback">异步回调</param>
            /// <param name="identityCommand">命令信息</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <returns>保持回调</returns>
            public keepCallback Call(callback<fastCSharp.net.returnValue> callback, identityCommand identityCommand, bool isTask)
            {
                fastCSharp.net.returnValue returnValue;
                if (isDisposed == 0)
                {
                    asyncIdentityCommand command = asyncIdentityCommand.Get();
                    if (command != null && command.Set(this, callback, identityCommand, default(commandServer.commandFlags), isTask))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                    returnValue.Type = returnValue.type.ClientException;
                }
                else returnValue.Type = returnValue.type.ClientDisposed;
                if (callback != null) callback.Callback(ref returnValue);
                return null;
            }

            /// <summary>
            /// 取消保持回调
            /// </summary>
            /// <param name="index">命令集合索引</param>
            /// <param name="identity">会话标识</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private void cancel(int index, int identity)
            {
                if (commandPool.Enter())
                {
                    if (commandPool.Pool[index].Cancel(identity)) commandPool.FreeExit(index);
                    else commandPool.Exit();
                }
            }
            /// <summary>
            /// 客户端命令接收数据
            /// </summary>
            internal sealed class commandReceiver
            {
                /// <summary>
                /// 客户端命令
                /// </summary>
                public command Command;
                /// <summary>
                /// 接收数据
                /// </summary>
                public memoryPool.pushSubArray Data;
                /// <summary>
                /// 客户端命令接收数据
                /// </summary>
                public void Receive()
                {
                    Command.OnReceive(ref Data);
                }
            }
            /// <summary>
            /// 接收数据回调
            /// </summary>
            /// <param name="identity">会话标识</param>
            /// <param name="data">输出数据</param>
            /// <param name="checkTask">检测回调是否使用任务池</param>
            /// <param name="isTaskCopyData">回调任务池是否复制数据</param>
            private void onReceive(commandServer.streamIdentity identity, ref memoryPool.pushSubArray data, byte checkTask, bool isTaskCopyData)
            {
                command command = null;
                byte isTask = 0;
                if (commandPool.Enter())
                {
                    if (commandPool.Pool[identity.Index].Get(identity.Identity, ref command, ref isTask)) commandPool.FreeExit(identity.Index);
                    else commandPool.Exit();
                    if (command != null) 
                    {
                        if (isTask == 0 || checkTask == 0)
                        {
                            try
                            {
                                command.OnReceive(ref data);
                            }
                            catch (Exception error)
                            {
                                log.Error.Add(error, null, false);
                            }
                        }
                        else if (isTaskCopyData)
                        {
                            subArray<byte> dataArray = data.Value;
                            memoryPool.pushSubArray pushData = default(memoryPool.pushSubArray);
                            byte[] buffer = asyncBuffers.Get(dataArray.length);
                            pushData.PushPool = asyncBuffers;
                            Buffer.BlockCopy(dataArray.array, dataArray.startIndex, buffer, 0, dataArray.length);
                            pushData.Value.UnsafeSet(buffer, 0, dataArray.length);
                            if (command.KeepCallback == null)
                            {
                                command.ReceiveData = pushData;
                                fastCSharp.threading.task.Tiny.Add(command, thread.callType.TcpCommandClientCommandOnRecieveData);
                            }
                            else fastCSharp.threading.task.Tiny.Add(new commandReceiver { Command = command, Data = pushData }, thread.callType.TcpCommandClientSocketReceiver);
                        }
                        else if (command.KeepCallback == null)
                        {
                            command.ReceiveData = data;
                            fastCSharp.threading.task.Tiny.Add(command, thread.callType.TcpCommandClientCommandOnRecieveData);
                        }
                        else fastCSharp.threading.task.Tiny.Add(new commandReceiver { Command = command, Data = data }, thread.callType.TcpCommandClientSocketReceiver);
                    }
                }
            }
            /// <summary>
            /// 接收服务器端数据
            /// </summary>
            internal void Receive()
            {
                if (attribute.IsClientAsynchronousReceive) receiveAsynchronous();
                else threadPool.TinyPool.FastStart(this, thread.callType.TcpCommandClientSocketSynchronousReceive);
            }
            /// <summary>
            /// 同步接收服务器端数据
            /// </summary>
            internal void SynchronousReceive()
            {
                try
                {
                    memoryPool.pushSubArray pushData = default(memoryPool.pushSubArray);
                    int index = receiveEndIndex = 0;
                    fixed (byte* dataFixed = receiveData)
                    {
                        do
                        {
                            int receiveLength = receiveEndIndex - index;
                            if (receiveLength < sizeof(commandServer.streamIdentity) + sizeof(int) + sizeof(int))
                            {
                                if (receiveLength != 0) unsafer.memory.Copy(dataFixed + index, dataFixed, receiveLength);
                                receiveEndIndex = tryReceive(receiveLength, sizeof(commandServer.streamIdentity) + sizeof(int) + sizeof(int), DateTime.MaxValue);
                                if (receiveEndIndex < sizeof(commandServer.streamIdentity) + sizeof(int) + sizeof(int))
                                {
                                    //log.Error.Add("receiveEndIndex " + receiveEndIndex.toString(), false, false);
                                    break;
                                }
                                receiveLength = receiveEndIndex;
                                index = 0;
                            }
                            byte* start = dataFixed + index;
                            commandServer.streamIdentity identity = *(commandServer.streamIdentity*)start;
                            //if (identity.Identity < 0)
                            //{
                            //    //log.Error.Add("identity.Identity " + identity.Identity.toString(), false, false);
                            //    break;
                            //}
                            int length = *(int*)(start + sizeof(commandServer.streamIdentity));
                            index += sizeof(commandServer.streamIdentity) + sizeof(int);
                            if (length == 0)
                            {
                                pushData.Value.UnsafeSet(null, int.MaxValue, *(start + (sizeof(commandServer.streamIdentity) + sizeof(int))));
                                onReceive(identity, ref pushData, 1, false);
                                index += sizeof(int);
                                continue;
                            }
                            int dataLength = length >= 0 ? length : -length;
                            receiveLength -= sizeof(commandServer.streamIdentity) + sizeof(int);
                            if (dataLength <= receiveData.Length)
                            {
                                if (dataLength > receiveLength)
                                {
                                    unsafer.memory.Copy(dataFixed + index, dataFixed, receiveLength);
                                    receiveEndIndex = tryReceive(receiveLength, dataLength, DateTime.MaxValue);
                                    if (receiveEndIndex < dataLength)
                                    {
                                        //log.Error.Add("receiveEndIndex[" + receiveEndIndex.toString() + "] < dataLength[" + dataLength.toString() + "]", false, false);
                                        break;
                                    }
                                    index = 0;
                                }
                                if (commandSocketProxy.ReceiveMarkData != 0) commandServer.Mark(receiveData, commandSocketProxy.ReceiveMarkData, index, dataLength);
                                if (length > 0)
                                {
                                    pushData.Value.UnsafeSet(receiveData, index, dataLength);
                                    onReceive(identity, ref pushData, 1, true);
                                }
                                else
                                {
                                    memoryPool.pushSubArray newPushData = new memoryPool.pushSubArray
                                    {
                                        Value = fastCSharp.io.compression.stream.Deflate.GetDeCompressUnsafe(receiveData, index, dataLength, fastCSharp.memoryPool.StreamBuffers),
                                        PushPool = fastCSharp.memoryPool.StreamBuffers
                                    };
                                    onReceive(identity, ref newPushData, 1, false);
                                }
                                index += dataLength;
                            }
                            else
                            {
                                byte[] buffer = BigBuffers.Get(dataLength);
                                unsafer.memory.Copy(dataFixed + index, buffer, receiveLength);
                                if (!receive(buffer, receiveLength, dataLength, DateTime.MaxValue))
                                {
                                    //log.Error.Add("receive Error", false, false);
                                    break;
                                }
                                if (commandSocketProxy.ReceiveMarkData != 0) commandServer.Mark(buffer, commandSocketProxy.ReceiveMarkData, dataLength);
                                if (length > 0)
                                {
                                    pushData.Value.UnsafeSet(buffer, 0, dataLength);
                                    onReceive(identity, ref pushData, 1, false);
                                }
                                else
                                {
                                    memoryPool.pushSubArray newPushData = new memoryPool.pushSubArray
                                    {
                                        Value = fastCSharp.io.compression.stream.Deflate.GetDeCompressUnsafe(buffer, 0, dataLength, fastCSharp.memoryPool.StreamBuffers),
                                        PushPool = fastCSharp.memoryPool.StreamBuffers
                                    };
                                    onReceive(identity, ref newPushData, 1, false);
                                }
                                BigBuffers.PushNotNull(buffer);
                                index = receiveEndIndex = 0;
                            }
                        }
                        while (true);
                    }
                }
                //catch (Exception error)
                //{
                //    log.Error.Add(error, null, false);
                //}
                finally
                {
                    Dispose();
                }
            }
            /// <summary>
            /// 接收数据处理递归深度
            /// </summary>
            private int receiveDepth;
            /// <summary>
            /// 接收服务器端数据
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private void receiveAsynchronous()
            {
                tryReceive(tryReceiver.type.ClientOnReceiveIdentity, 0, sizeof(commandServer.streamIdentity) + sizeof(int) + sizeof(int), DateTime.MaxValue);
            }
            /// <summary>
            /// 接收会话标识
            /// </summary>
            /// <param name="receiveEndIndex">接收数据结束位置</param>
            internal unsafe void OnReceiveIdentity(int receiveEndIndex)
            {
                //if (isOutputDebug) commandServer.DebugLog.Add(attribute.ServiceName + ".onReceiveIdentity(" + receiveEndIndex.toString() + ")", false, false);
                if (receiveEndIndex >= sizeof(commandServer.streamIdentity) + sizeof(int) + sizeof(int))
                {
                    fixed (byte* dataFixed = receiveData)
                    {
                        this.receiveEndIndex = receiveEndIndex;
                        receiveDataFixed = dataFixed;
                        receiveDepth = 512;
                        onReceiveIdentity();
                        return;
                    }
                }
                Dispose();
            }
            /// <summary>
            /// 接收会话标识
            /// </summary>
            private void onReceiveIdentity()
            {
                commandServer.streamIdentity identity = *(commandServer.streamIdentity*)receiveDataFixed;
                //if (identity.Identity >= 0)
                //{
                    int length = *(int*)(receiveDataFixed + sizeof(commandServer.streamIdentity));
                    if (length == 0)
                    {
                        byte type = *(receiveDataFixed + (sizeof(commandServer.streamIdentity) + sizeof(int)));
                        receiveIdentity(sizeof(commandServer.streamIdentity) + sizeof(int) + sizeof(int));
                        memoryPool.pushSubArray pushData = default(memoryPool.pushSubArray);
                        pushData.Value.UnsafeSet(null, int.MaxValue, type);
                        onReceive(identity, ref pushData, 0, false);
                    }
                    else
                    {
                        int index = onReceiveIdentity(0, length);
                        if (index != 0) receiveIdentity(index);
                    }
                    //return;
                //}
                //Dispose();
            }
            /// <summary>
            /// 接收会话标识起始位置
            /// </summary>
            private int receiveNextIdentityIndex;
            /// <summary>
            /// 接收会话标识
            /// </summary>
            internal void ReceiveNextIdentity()
            {
                receiveDepth = 512;
                fixed (byte* dataFixed = receiveData)
                {
                    receiveDataFixed = dataFixed;
                    receiveIdentity(receiveNextIdentityIndex);
                }
            }
            /// <summary>
            /// 接收会话标识
            /// </summary>
            /// <param name="index">起始位置</param>
            private void receiveIdentity(int index)
            {
                if (--receiveDepth == 0)
                {
                    receiveNextIdentityIndex = index;
                    threadPool.Default.FastStart(this, thread.callType.TcpCommandClientSocketReceiveNextIdentity);
                    return;
                }
                NEXT:
                int receiveLength = receiveEndIndex - index;
                if (receiveLength >= sizeof(commandServer.streamIdentity) + sizeof(int) + sizeof(int))
                {
                    byte* start = receiveDataFixed + index;
                    commandServer.streamIdentity identity = *(commandServer.streamIdentity*)start;
                    //if (identity.Identity >= 0)
                    //{
                        int length = *(int*)(start + sizeof(commandServer.streamIdentity));
                        if (length == 0)
                        {
                            byte type = *(start + (sizeof(commandServer.streamIdentity) + sizeof(int)));
                            receiveIdentity(index + sizeof(commandServer.streamIdentity) + sizeof(int) + sizeof(int));
                            memoryPool.pushSubArray pushData = default(memoryPool.pushSubArray);
                            pushData.Value.UnsafeSet(null, int.MaxValue, type);
                            onReceive(identity, ref pushData, 0, false);
                        }
                        else if ((index = onReceiveIdentity(index, length)) != 0) goto NEXT;
                    //}
                    //else Dispose();
                }
                else
                {
                    if (receiveLength != 0) unsafer.memory.Copy(receiveDataFixed + index, receiveDataFixed, receiveLength);
                    tryReceive(tryReceiver.type.ClientOnReceiveIdentity, receiveLength, sizeof(commandServer.streamIdentity) + sizeof(int) + sizeof(int), DateTime.MaxValue);
                }
            }
            /// <summary>
            /// 接收会话标识
            /// </summary>
            /// <param name="index">起始位置</param>
            /// <param name="length">数据长度</param>
            /// <returns>下一个数据起始位置,失败返回0</returns>
            private int onReceiveIdentity(int index, int length)
            {
                commandServer.streamIdentity identity = *(commandServer.streamIdentity*)(receiveDataFixed + index);
                //if (identity.Identity >= 0)
                //{
                    int dataLength = length >= 0 ? length : -length, receiveLength = receiveEndIndex - (index += (sizeof(commandServer.streamIdentity) + sizeof(int)));
                    if (dataLength <= receiveLength)
                    {
                        if (commandSocketProxy.ReceiveMarkData != 0) commandServer.Mark(receiveData, commandSocketProxy.ReceiveMarkData, index, dataLength);
                        if (length > 0)
                        {
                            memoryPool.pushSubArray pushData = default(memoryPool.pushSubArray);
                            pushData.Value.UnsafeSet(receiveData, index, dataLength);
                            onReceive(identity, ref pushData, 1, true);
                            return index + dataLength;
                        }
                        else
                        {
                            memoryPool.pushSubArray pushData = new memoryPool.pushSubArray
                            {
                                Value = fastCSharp.io.compression.stream.Deflate.GetDeCompressUnsafe(receiveData, index, dataLength, fastCSharp.memoryPool.StreamBuffers),
                                PushPool = fastCSharp.memoryPool.StreamBuffers
                            };
                            receiveIdentity(index + dataLength);
                            onReceive(identity, ref pushData, 0, false);
                        }
                    }
                    else
                    {
                        currentIdentity = identity;
                        unsafer.memory.Copy(receiveDataFixed + index, currentReceiveData = BigBuffers.Get(dataLength), receiveLength);
                        if (length > 0)
                        {
                            receive(this, receiver.type.ClientReceiveNoCompress, receiveLength, dataLength - receiveLength, DateTime.MaxValue);
                        }
                        else
                        {
                            receive(this, receiver.type.ClientReceiveCompress, receiveLength, dataLength - receiveLength, DateTime.MaxValue);
                        }
                    }
                //}
                //else Dispose();
                return 0;
            }
            /// <summary>
            /// 获取非压缩数据
            /// </summary>
            /// <param name="isSocket">是否成功</param>
            internal void ReceiveNoCompress(bool isSocket)
            {
                //if (isOutputDebug) commandServer.DebugLog.Add(attribute.ServiceName + ".receiveNoCompress(" + isSocket.ToString() + ")", false, false);
                if (isSocket)
                {
                    memoryPool.pushSubArray pushData = default(memoryPool.pushSubArray);
                    commandServer.streamIdentity identity = currentIdentity;
                    int dataLength = currentReceiveEndIndex;
                    pushData.Value.UnsafeSet(currentReceiveData, 0, currentReceiveEndIndex);
                    pushData.PushPool = BigBuffers;
                    currentReceiveData = receiveData;
                    receiveAsynchronous();
                    if (commandSocketProxy.ReceiveMarkData != 0) commandServer.Mark(pushData.UnsafeArray, commandSocketProxy.ReceiveMarkData, dataLength);
                    onReceive(identity, ref pushData, 0, false);
                }
                else Dispose();
            }
            /// <summary>
            /// 获取压缩数据
            /// </summary>
            /// <param name="isSocket">是否成功</param>
            internal void ReceiveCompress(bool isSocket)
            {
                //if (isOutputDebug) commandServer.DebugLog.Add(attribute.ServiceName + ".receiveCompress(" + isSocket.ToString() + ")", false, false);
                if (isSocket)
                {
                    if (commandSocketProxy.ReceiveMarkData != 0) commandServer.Mark(currentReceiveData, commandSocketProxy.ReceiveMarkData, currentReceiveEndIndex);
                    memoryPool.pushSubArray pushData = new memoryPool.pushSubArray
                    {
                        Value = fastCSharp.io.compression.stream.Deflate.GetDeCompressUnsafe(currentReceiveData, 0, currentReceiveEndIndex, fastCSharp.memoryPool.StreamBuffers),
                        PushPool = fastCSharp.memoryPool.StreamBuffers
                    };
                    commandServer.streamIdentity identity = currentIdentity;
                    BigBuffers.Push(ref currentReceiveData);
                    currentReceiveData = receiveData;
                    receiveAsynchronous();
                    onReceive(identity, ref pushData, 0, false);
                }
                else Dispose();
            }

            /// <summary>
            /// 创建TCP客户端套接字
            /// </summary>
            /// <param name="commandClient">TCP调用客户端</param>
            /// <returns>TCP客户端套接字</returns>
            internal static streamCommandSocket Create(commandClient commandClient)
            {
                Socket socket = client.Create(commandClient.Attribute);
                if (socket != null)
                {
                    bool isVerify = false;
                    try
                    {
                        memoryPool pool = fastCSharp.memoryPool.StreamBuffers;
                        byte[] receiveData = pool.Size == commandClient.Attribute.SendBufferSize ? pool.Get() : new byte[commandClient.Attribute.SendBufferSize];
                        byte[] sendData = pool.Size == commandClient.Attribute.ReceiveBufferSize ? pool.Get() : new byte[commandClient.Attribute.ReceiveBufferSize];
                        streamCommandSocket commandSocket = new streamCommandSocket(commandClient, socket, sendData, receiveData);
                        isVerify = commandSocket.verify();
                        if (isVerify) return commandSocket;
                    }
                    finally
                    {
                        if (!isVerify) socket.shutdown();
                        //if (!isVerify && tcpClient != null) tcpClient.Close();
                    }
                }
                return null;
            }
            /// <summary>
            /// 创建TCP客户端套接字
            /// </summary>
            /// <param name="commandClient">TCP调用客户端</param>
            /// <param name="socket">套接字</param>
            /// <returns>TCP客户端套接字</returns>
            internal static streamCommandSocket Create(commandClient commandClient, Socket socket)
            {
                try
                {
                    memoryPool pool = fastCSharp.memoryPool.StreamBuffers;
                    byte[] receiveData = pool.Size == commandClient.Attribute.SendBufferSize ? pool.Get() : new byte[commandClient.Attribute.SendBufferSize];
                    byte[] sendData = pool.Size == commandClient.Attribute.ReceiveBufferSize ? pool.Get() : new byte[commandClient.Attribute.ReceiveBufferSize];
                    streamCommandSocket commandSocket = new streamCommandSocket(commandClient, socket, sendData, receiveData);
                    if (commandSocket.verify()) return commandSocket;
                }
                catch { }
                return null;
            }
            static unsafe streamCommandSocket()
            {
                closeIdentityCommandData = new byte[(sizeof(int) * 3 + sizeof(commandServer.streamIdentity))];
                fixed (byte* commandFixed = closeIdentityCommandData)
                {
                    *(int*)(commandFixed) = commandServer.CloseIdentityCommand;
                    *(int*)(commandFixed + sizeof(int)) = (int)(uint)commandServer.commandFlags.JsonSerialize;
                    (*(commandServer.streamIdentity*)(commandFixed + sizeof(int) * 2)).Set(commandClient.streamCommandSocket.CloseCallbackIndex);
                    *(int*)(commandFixed + (sizeof(int) * 2 + sizeof(commandServer.streamIdentity))) = 0;
                }
                closeCommandData = new byte[(sizeof(int) * 4 + sizeof(commandServer.streamIdentity))];
                fixed (byte* commandFixed = closeCommandData)
                {
                    *(int*)(commandFixed) = sizeof(int) * 4 + sizeof(commandServer.streamIdentity);
                    *(int*)(commandFixed + sizeof(int)) = commandServer.CloseIdentityCommand + commandServer.CommandDataIndex;
                    *(int*)(commandFixed + sizeof(int) * 2) = (int)(uint)commandServer.commandFlags.JsonSerialize;
                    (*(commandServer.streamIdentity*)(commandFixed + sizeof(int) * 3)).Set(commandClient.streamCommandSocket.CloseCallbackIndex);
                    *(int*)(commandFixed + sizeof(int) * 3 + sizeof(commandServer.streamIdentity)) = 0;
                }

                checkDataCommand = new dataCommand { Command = new byte[sizeof(int) + sizeof(int)], IsSendOnly = 1 };
                fixed (byte* commandFixed = checkDataCommand.Command)
                {
                    *(int*)commandFixed = sizeof(int) * 3 + sizeof(commandServer.streamIdentity);
                    *(int*)(commandFixed + sizeof(int)) = commandServer.CheckIdentityCommand + commandServer.CommandDataIndex;
                }
                loadBalancingCheckDataCommand = new dataCommand { Command = new byte[sizeof(int) + sizeof(int)], IsSendOnly = 1 };
                fixed (byte* commandFixed = loadBalancingCheckDataCommand.Command)
                {
                    *(int*)commandFixed = sizeof(int) * 3 + sizeof(commandServer.streamIdentity);
                    *(int*)(commandFixed + sizeof(int)) = commandServer.LoadBalancingCheckIdentityCommand + commandServer.CommandDataIndex;
                }
                tcpStreamDataCommand = new dataCommand { Command = new byte[sizeof(int) + sizeof(int)], MaxInputSize = int.MaxValue };
                fixed (byte* commandFixed = tcpStreamDataCommand.Command)
                {
                    *(int*)commandFixed = sizeof(int) * 3 + sizeof(commandServer.streamIdentity);
                    *(int*)(commandFixed + sizeof(int)) = commandServer.TcpStreamCommand + commandServer.CommandDataIndex;
                }
                ignoreGroupDataCommand = new dataCommand { Command = new byte[sizeof(int) + sizeof(int)], MaxInputSize = int.MaxValue };
                fixed (byte* commandFixed = ignoreGroupDataCommand.Command)
                {
                    *(int*)commandFixed = sizeof(int) * 3 + sizeof(commandServer.streamIdentity);
                    *(int*)(commandFixed + sizeof(int)) = commandServer.IgnoreGroupCommand + commandServer.CommandDataIndex;
                }
            }
        }
        /// <summary>
        /// 配置信息
        /// </summary>
        public fastCSharp.code.cSharp.tcpServer Attribute { get; private set; }
        /// <summary>
        /// TCP客户端命令流处理套接字
        /// </summary>
        private streamCommandSocket streamSocket;
        /// <summary>
        /// TCP客户端命令流处理套接字
        /// </summary>
        public streamCommandSocket StreamSocket
        {
            get
            {
                return streamSocket ?? createStreamSocket();
            }
        }
        /// <summary>
        /// 创建TCP客户端命令流处理套接字
        /// </summary>
        private waitHandle createStreamSocketWait = new waitHandle(false);
        /// <summary>
        /// TCP客户端命令流处理套接字访问锁
        /// </summary>
        private int createStreamSocketLock;
        /// <summary>
        /// 创建TCP客户端命令流处理套接字
        /// </summary>
        /// <returns></returns>
        private streamCommandSocket createStreamSocket()
        {
            if (Attribute.Port != 0)
            {
                if (Interlocked.CompareExchange(ref createStreamSocketLock, 1, 0) == 0)
                {
                    try
                    {
                        createStreamSocketWait.Reset();
                        streamCommandSocket streamSocket = streamCommandSocket.Create(this);
                        if (streamSocket != null)
                        {
                            if (isDisposed == 0)
                            {
                                streamSocket.Receive();
                                verifyStreamSocket = streamSocket;
                                if (callVerifyMethod())
                                {
                                    streamSocket.SetCheck();
                                    this.streamSocket = streamSocket;
                                }
                                verifyStreamSocket = null;
                            }
                            else streamSocket.Dispose();
                        }
                    }
                    finally
                    {
                        createStreamSocketWait.Set();
                        createStreamSocketLock = 0;
                    }
                }
                else createStreamSocketWait.Wait();
            }
            return this.streamSocket;
        }
        /// <summary>
        /// TCP服务信息集合版本变化处理
        /// </summary>
        internal event Action OnChangeTcpRegisterServicesVersion;
        /// <summary>
        /// TCP服务信息集合版本变化处理
        /// </summary>
        internal void ChangeTcpRegisterServicesVersion()
        {
            fastCSharp.net.tcp.tcpRegister.client tcpRegisterClient = TcpRegisterClient;
            if (tcpRegisterClient != null && TcpRegisterClient.GetHost(this))
            {
                streamCommandSocket streamSocket;
                while (Interlocked.CompareExchange(ref createStreamSocketLock, 1, 0) != 0) createStreamSocketWait.Wait();
                try
                {
                    createStreamSocketWait.Reset();
                    if ((streamSocket = this.streamSocket) != null)
                    {
                        if (streamSocket.CheckHost(Attribute)) streamSocket = null;
                        else Interlocked.CompareExchange(ref this.streamSocket, null, streamSocket);
                    }
                }
                finally
                {
                    createStreamSocketWait.Set();
                    createStreamSocketLock = 0;
                }
                if (streamSocket != null)
                {
                    log.Default.Add("TCP服务更新，关闭客户端 " + Attribute.ServiceName + "[" + Attribute.TcpRegisterName + "]", new System.Diagnostics.StackFrame(), false);
                    fastCSharp.threading.timerTask.Default.Add(streamSocket, thread.callType.TcpCommandClientSocketDispose, date.nowTime.Now.AddMinutes(1));
                }
            }
            if (OnChangeTcpRegisterServicesVersion != null) OnChangeTcpRegisterServicesVersion();
        }
        /// <summary>
        /// 验证函数TCP客户端命令流处理套接字
        /// </summary>
        protected streamCommandSocket verifyStreamSocket;
        /// <summary>
        /// 验证函数TCP客户端命令流处理套接字
        /// </summary>
        public streamCommandSocket VerifyStreamSocket
        {
            get { return verifyStreamSocket ?? streamSocket; }
        }
        /// <summary>
        /// 验证接口
        /// </summary>
        internal fastCSharp.code.cSharp.tcpBase.ITcpClientVerify Verify;
        /// <summary>
        /// 验证函数接口
        /// </summary>
        private fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod verifyMethod;
        /// <summary>
        /// 发送变换数据
        /// </summary>
        internal ulong SendMarkData;
        /// <summary>
        /// 接收变换数据
        /// </summary>
        internal ulong ReceiveMarkData;
        /// <summary>
        /// 获取TCP客户端命令流处理套接字委托集合
        /// </summary>
        private subArray<commandClient.routerClientCreator> onStreamSockets;
        /// <summary>
        /// 是否正在创建TCP客户端命令流处理套接字
        /// </summary>
        private byte isCreateStreamSocket;
        /// <summary>
        /// TCP客户端命令流处理套接字访问锁
        /// </summary>
        private readonly object getStreamSocketLock = new object();
        /// <summary>
        /// 创建TCP客户端命令流处理套接字
        /// </summary>
        internal sealed class streamSocketCreator : IDisposable
        {
            /// <summary>
            /// TCP调用客户端
            /// </summary>
            private commandClient commandClient;
            /// <summary>
            /// 套接字
            /// </summary>
            private Socket socket;
            /// <summary>
            /// 异步连接操作
            /// </summary>
            private SocketAsyncEventArgs connectAsync;
            /// <summary>
            /// TCP客户端命令流处理套接字
            /// </summary>
            private streamCommandSocket streamCommandSocket;
            /// <summary>
            /// 是否占用TCP客户端命令流处理套接字访问锁
            /// </summary>
            private byte isCreate;
            /// <summary>
            /// 创建TCP客户端命令流处理套接字
            /// </summary>
            public streamSocketCreator()
            {
                connectAsync = new SocketAsyncEventArgs();
                connectAsync.UserToken = this;
                connectAsync.Completed += onConnect;
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                pub.Dispose(ref connectAsync);
            }
            /// <summary>
            /// 创建TCP客户端命令流处理套接字
            /// </summary>
            internal void Create()
            {
                fastCSharp.code.cSharp.tcpServer attribute = commandClient.Attribute;
                if (attribute.Port != 0)
                {
                    if (Interlocked.CompareExchange(ref commandClient.createStreamSocketLock, 1, 0) == 0)
                    {
                        isCreate = 1;
                        commandClient.createStreamSocketWait.Reset();
                        if (client.Create(ref socket, attribute, connectAsync)) return;
                    }
                    else commandClient.createStreamSocketWait.Wait();
                }
                onCreate(commandClient.streamSocket);
            }
            /// <summary>
            /// 套接字连接处理
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="async"></param>
            private void onConnect(object sender, SocketAsyncEventArgs async)
            {
                if (async.SocketError == SocketError.Success)
                {
                    streamCommandSocket = streamCommandSocket.Create(commandClient, socket);
                    if (streamCommandSocket != null)
                    {
                        onCreate(commandClient.callVerifyMethod(streamCommandSocket) ? streamCommandSocket : null);
                        return;
                    }
                }
                else
                {
                    tcpServer attribute = commandClient.Attribute;
                    log.Error.Add("客户端TCP连接失败 " + async.SocketError.ToString() + "(" + attribute.ServiceName + "[" + attribute.TcpRegisterName + "] " + attribute.IpAddress.ToString() + ":" + attribute.Port.toString() + ")", new System.Diagnostics.StackFrame(), false);
                }
                onCreate(null);
            }
            /// <summary>
            /// 创建TCP客户端命令流处理套接字
            /// </summary>
            /// <param name="socket"></param>
            private void onCreate(streamCommandSocket socket)
            {
                commandClient commandClient = this.commandClient;
                if (socket == null)
                {
                    pub.Dispose(ref streamCommandSocket);
                    this.socket.shutdown();
                }
                else streamCommandSocket = null;
                byte isCreate = this.isCreate;
                this.commandClient = null;
                this.socket = null;
                this.isCreate = 0;
                commandClient.onGetStreamSocket(socket, isCreate);
                typePool<streamSocketCreator>.PushNotNull(this);
            }
            /// <summary>
            /// 创建TCP客户端命令流处理套接字
            /// </summary>
            /// <param name="commandClient"></param>
            /// <returns></returns>
            public static streamSocketCreator Get(commandClient commandClient)
            {
                streamSocketCreator value = typePool<streamSocketCreator>.Pop();
                if (value == null)
                {
                    try
                    {
                        value = new streamSocketCreator();
                    }
                    catch
                    {
                        return null;
                    }
                }
                value.commandClient = commandClient;
                return value;
            }
        }
        /// <summary>
        /// 获取TCP客户端命令流处理套接字
        /// </summary>
        /// <param name="onStreamSocket"></param>
        public void GetStreamSocket(commandClient.routerClientCreator onStreamSocket)
        {
            streamCommandSocket streamSocket = this.streamSocket;
            if (streamSocket == null)
            {
                Monitor.Enter(getStreamSocketLock);
                byte isCreateStreamSocket = this.isCreateStreamSocket;
                try
                {
                    onStreamSockets.Add(onStreamSocket);
                    onStreamSocket = null;
                    if (isCreateStreamSocket == 0) this.isCreateStreamSocket = 1;
                }
                finally
                {
                    Monitor.Exit(getStreamSocketLock);
                    if (onStreamSocket == null)
                    {
                        if (isCreateStreamSocket == 0)
                        {
                            streamSocketCreator streamSocketCreator = streamSocketCreator.Get(this);
                            if (streamSocketCreator == null) onGetStreamSocket(null, 0);
                            else fastCSharp.threading.threadPool.TinyPool.FastStart(streamSocketCreator, thread.callType.TcpCommandClientSocketCreator);
                        }
                    }
                    else onStreamSocket.OnCreated(null);
                }
                return;
            }
            onStreamSocket.OnCreated(streamSocket);
        }
        /// <summary>
        /// 创建TCP客户端命令流处理套接字
        /// </summary>
        /// <param name="streamSocket"></param>
        /// <param name="isCreate">是否占用TCP客户端命令流处理套接字访问锁</param>
        private void onGetStreamSocket(streamCommandSocket streamSocket, byte isCreate)
        {
            Monitor.Enter(getStreamSocketLock);
            subArray<commandClient.routerClientCreator> onStreamSockets = this.onStreamSockets;
            isCreateStreamSocket = 0;
            this.onStreamSockets.Null();
            Monitor.Exit(getStreamSocketLock);
            if (isCreate != 0)
            {
                this.streamSocket = streamSocket;
                createStreamSocketWait.Set();
                createStreamSocketLock = 0;
            }
            foreach (commandClient.routerClientCreator onStreamSocket in onStreamSockets)
            {
                try
                {
                    onStreamSocket.OnCreated(streamSocket);
                }
                catch (Exception error)
                {
                    log.Default.Add(error, null, false);
                }
            }
        }
        /// <summary>
        /// 是否释放资源
        /// </summary>
        private int isDisposed;
        /// <summary>
        /// 是否释放资源
        /// </summary>
        public bool IsDisposed
        {
            get { return isDisposed != 0; }
        }
        /// <summary>
        /// TCP注册服务 客户端
        /// </summary>
        internal fastCSharp.net.tcp.tcpRegister.client TcpRegisterClient;
        /// <summary>
        /// TCP服务信息集合
        /// </summary>
        internal fastCSharp.net.tcp.tcpRegister.services TcpRegisterServices;
        ///// <summary>
        ///// TCP服务信息集合版本
        ///// </summary>
        //internal int TcpRegisterServicesVersion;
        /// <summary>
        /// 服务器端负载均衡联通测试时间
        /// </summary>
        public DateTime LoadBalancingCheckTime { get; private set; }
        /// <summary>
        /// 服务名称
        /// </summary>
        internal string ServiceName { get { return Attribute.ServiceName; } }
        /// <summary>
        /// TCP调用服务端
        /// </summary>
        /// <param name="attribute">配置信息</param>
        /// <param name="maxCommandLength">最大命令长度</param>
        public unsafe commandClient(fastCSharp.code.cSharp.tcpServer attribute, int maxCommandLength)
        {
            this.Attribute = attribute;
            if (attribute.SendBufferSize <= (sizeof(int) * 2 + sizeof(commandServer.streamIdentity))) attribute.SendBufferSize = Math.Max(sizeof(int) * 2 + sizeof(commandServer.streamIdentity), fastCSharp.config.appSetting.StreamBufferSize);
            if (attribute.ReceiveBufferSize <= maxCommandLength + (sizeof(int) * 3 + sizeof(commandServer.streamIdentity))) attribute.ReceiveBufferSize = Math.Max(maxCommandLength + (sizeof(int) * 3 + sizeof(commandServer.streamIdentity)), fastCSharp.config.appSetting.StreamBufferSize);
            if (attribute.TcpRegisterName == null) TcpRegisterServices = fastCSharp.net.tcp.tcpRegister.services.Null;
            else
            {
                TcpRegisterClient = fastCSharp.net.tcp.tcpRegister.client.Get(attribute.TcpRegisterName);
                TcpRegisterClient.Register(this);
            }
        }
        /// <summary>
        /// TCP调用服务端
        /// </summary>
        /// <param name="attribute">配置信息</param>
        /// <param name="maxCommandLength">最大命令长度</param>
        /// <param name="verify">验证接口</param>
        public commandClient(fastCSharp.code.cSharp.tcpServer attribute, int maxCommandLength, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify)
            : this(attribute, maxCommandLength)
        {
            this.Verify = verify;
        }
        /// <summary>
        /// TCP调用服务端
        /// </summary>
        /// <param name="attribute">配置信息</param>
        /// <param name="maxCommandLength">最大命令长度</param>
        /// <param name="verifyMethod">验证函数接口</param>
        public commandClient(fastCSharp.code.cSharp.tcpServer attribute, int maxCommandLength, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod verifyMethod)
            : this(attribute, maxCommandLength)
        {
            this.verifyMethod = verifyMethod;
        }
        /// <summary>
        /// TCP调用服务端
        /// </summary>
        /// <param name="attribute">配置信息</param>
        /// <param name="maxCommandLength">最大命令长度</param>
        /// <param name="verifyMethod">验证函数接口</param>
        /// <param name="verify">验证接口</param>
        public commandClient(fastCSharp.code.cSharp.tcpServer attribute, int maxCommandLength, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod verifyMethod, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify)
            : this(attribute, maxCommandLength)
        {
            this.Verify = verify;
            this.verifyMethod = verifyMethod;
        }
        /// <summary>
        /// 停止客户端链接
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.Increment(ref isDisposed) == 1)
            {
                log.Default.Add("关闭TCP客户端 " + Attribute.ServiceName + "[" + Attribute.TcpRegisterName + "]", null, log.cacheType.Last);
                if (TcpRegisterClient != null)
                {
                    TcpRegisterClient.Remove(this);
                    TcpRegisterClient = null;
                }
                pub.Dispose(ref streamSocket);
                Stream[] streams = nullValue<Stream>.Array;
                Monitor.Enter(tcpStreamLock);
                try
                {
                    streams = new Stream[tcpStreams.length()];
                    for (int index = streams.Length; index != 0; )
                    {
                        --index;
                        streams[index] = tcpStreams[index].Cancel();
                    }
                }
                finally { Monitor.Exit(tcpStreamLock); }
                foreach (Stream stream in streams) pub.Dispose(stream);
                onGetStreamSocket(null, 0);
                verifyStreamSocket = null;
                createStreamSocketWait.Set();
            }
        }
        ///// <summary>
        ///// 等待命令缓存区空闲
        ///// </summary>
        //public void WaitFree()
        //{
        //    streamCommandSocket streamSocket = this.streamSocket;
        //    if (streamSocket != null) while (streamSocket.IsBusy) Thread.Sleep(0);
        //}
        /// <summary>
        /// 函数验证
        /// </summary>
        /// <returns>是否验证成功</returns>
        protected virtual bool callVerifyMethod()
        {
            if (verifyMethod == null) return true;
            bool isError = false;
            try
            {
                if (verifyMethod.Verify()) return true;
            }
            catch (Exception error)
            {
                isError = true;
                log.Error.Add(error, "TCP客户端验证失败", false);
            }
            if (!isError) log.Error.Add("TCP客户端验证失败", null, false);
            //Dispose();
            return false;
        }
        /// <summary>
        /// 函数验证
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        protected virtual bool callVerifyMethod(streamCommandSocket socket)
        {
            return false;
        }

        /// <summary>
        /// TCP参数流
        /// </summary>
        private struct tcpStream
        {
            /// <summary>
            /// 字节流
            /// </summary>
            public Stream Stream;
            /// <summary>
            /// 当前序号
            /// </summary>
            public int Identity;
            /// <summary>
            /// 设置TCP参数流
            /// </summary>
            /// <param name="stream">字节流</param>
            /// <returns>当前序号</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public int Set(Stream stream)
            {
                Stream = stream;
                return Identity;
            }
            /// <summary>
            /// 获取TCP参数流
            /// </summary>
            /// <param name="identity">当前序号</param>
            /// <returns>TCP参数流</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public Stream Get(int identity)
            {
                return identity == Identity ? Stream : null;
            }
            /// <summary>
            /// 取消TCP参数流
            /// </summary>
            /// <returns>字节流</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public Stream Cancel()
            {
                ++Identity;
                Stream stream = Stream;
                Stream = null;
                return stream;
            }
            /// <summary>
            /// 关闭TCP参数流
            /// </summary>
            /// <param name="identity">当前序号</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Close(int identity)
            {
                if (identity == Identity)
                {
                    ++Identity;
                    Stream = null;
                }
            }
        }
        /// <summary>
        /// TCP参数流集合
        /// </summary>
        private tcpStream[] tcpStreams;
        /// <summary>
        /// TCP参数流集合访问锁
        /// </summary>
        private readonly object tcpStreamLock = new object();
        /// <summary>
        /// 获取TCP参数流
        /// </summary>
        /// <param name="stream">字节流</param>
        /// <returns>TCP参数流</returns>
        public tcpBase.tcpStream GetTcpStream(Stream stream)
        {
            if (stream != null)
            {
                try
                {
                    tcpBase.tcpStream tcpStream = new tcpBase.tcpStream { CanRead = stream.CanRead, CanWrite = stream.CanWrite, CanSeek = stream.CanSeek, CanTimeout = stream.CanTimeout };
                START:
                    Monitor.Enter(tcpStreamLock);
                    if (tcpStreams == null)
                    {
                        try
                        {
                            tcpStreams = new tcpStream[4];
                            tcpStream.ClientIndex = tcpStream.ClientIdentity = 0;
                            tcpStreams[0].Stream = stream;
                        }
                        finally { Monitor.Exit(tcpStreamLock); }
                    }
                    else
                    {
                        foreach (tcpStream value in tcpStreams)
                        {
                            if (value.Stream == null)
                            {
                                tcpStream.ClientIdentity = tcpStreams[tcpStream.ClientIndex].Set(stream);
                                Monitor.Exit(tcpStreamLock);
                                break;
                            }
                            ++tcpStream.ClientIndex;
                        }
                        if (tcpStream.ClientIndex == tcpStreams.Length)
                        {
                            if (tcpStream.ClientIndex == tcpStreams.Length)
                            {
                                try
                                {
                                    tcpStream[] newTcpStreams = new tcpStream[tcpStream.ClientIndex << 1];
                                    tcpStreams.CopyTo(newTcpStreams, 0);
                                    tcpStreams = newTcpStreams;
                                    tcpStream.ClientIdentity = tcpStreams[tcpStream.ClientIndex].Set(stream);
                                }
                                finally { Monitor.Exit(tcpStreamLock); }
                            }
                            else
                            {
                                Monitor.Exit(tcpStreamLock);
                                tcpStream.ClientIndex = 0;
                                goto START;
                            }
                        }
                    }
                    stream = null;
                    tcpStream.IsStream = true;
                    return tcpStream;
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                finally { pub.Dispose(ref stream); }
            }
            return default(tcpBase.tcpStream);
        }
        /// <summary>
        /// 获取TCP参数流
        /// </summary>
        /// <param name="index">TCP参数流索引</param>
        /// <param name="identity">TCP参数流序号</param>
        /// <returns>TCP参数流</returns>
        private Stream getTcpStream(int index, int identity)
        {
            Stream stream;
            Monitor.Enter(tcpStreamLock);
            try
            {
                stream = tcpStreams[index].Get(identity);
            }
            finally { Monitor.Exit(tcpStreamLock); }
            return stream;
        }
        /// <summary>
        /// 关闭TCP参数流
        /// </summary>
        /// <param name="index">TCP参数流索引</param>
        /// <param name="identity">TCP参数流序号</param>
        private void closeTcpStream(int index, int identity)
        {
            Monitor.Enter(tcpStreamLock);
            try
            {
                tcpStreams[index].Close(identity);
            }
            finally { Monitor.Exit(tcpStreamLock); }
        }

        /// <summary>
        /// 忽略TCP调用分组
        /// </summary>
        /// <param name="groupId">分组标识</param>
        /// <returns>是否调用成功</returns>
        public returnValue.type IgnoreGroup(int groupId)
        {
            try
            {
                streamCommandSocket socket = StreamSocket;
                if (socket != null) return socket.IgnoreGroup(groupId);
            }
            catch (Exception error)
            {
                log.Error.Add(error, null, false);
            }
            return returnValue.type.ClientException;
        }

        /// <summary>
        /// 负载均衡超时检测
        /// </summary>
        /// <returns>客户端是否可用</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public returnValue.type LoadBalancingCheck()
        {
            streamCommandSocket socket = StreamSocket;
            return socket == null ? returnValue.type.ClientDisposed : socket.LoadBalancingCheck();
        }
    }
    /// <summary>
    /// TCP调用客户端(tcpServer)
    /// </summary>
    /// <typeparam name="clientType">客户端类型</typeparam>
    public class commandClient<clientType> : commandClient where clientType : class, commandClient.IClient
    {
        /// <summary>
        /// TCP调用客户端路由
        /// </summary>
        public abstract class routerBase
        {
            /// <summary>
            /// TCP服务调用配置
            /// </summary>
            protected fastCSharp.code.cSharp.tcpServer attribute;
            /// <summary>
            /// 验证接口
            /// </summary>
            protected fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify;
            /// <summary>
            /// TCP调用客户端
            /// </summary>
            protected clientType client;
            /// <summary>
            /// TCP调用客户端
            /// </summary>
            protected commandClient<clientType> commandClient;
            ///// <summary>
            ///// TCP注册服务 客户端
            ///// </summary>
            //protected fastCSharp.net.tcp.tcpRegister.client tcpRegisterClient;
            /// <summary>
            /// 可用TCP调用客户端数量
            /// </summary>
            protected int count;
            /// <summary>
            /// TCP服务端口信息数量
            /// </summary>
            protected int hostCount;
            /// <summary>
            /// TCP调用客户端集合访问锁
            /// </summary>
            protected readonly object clientLock = new object();
            /// <summary>
            /// 是否已经释放资源
            /// </summary>
            protected int isDisposed;
#if NOJIT
            /// <summary>
            /// 验证函数接口
            /// </summary>
            protected fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject verifyMethod;
            /// <summary>
            /// 创建TCP调用客户端委托
            /// </summary>
            protected Func<fastCSharp.code.cSharp.tcpServer, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify, clientType> getClient;
            /// <summary>
            /// TCP调用客户端路由
            /// </summary>
            /// <param name="getClient"></param>
            /// <param name="attribute"></param>
            /// <param name="verifyMethod"></param>
            /// <param name="verify"></param>
            public routerBase(Func<fastCSharp.code.cSharp.tcpServer, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify, clientType> getClient
                , fastCSharp.code.cSharp.tcpServer attribute, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject verifyMethod, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify)
#else
            /// <summary>
            /// 创建TCP调用客户端委托
            /// </summary>
            protected Func<fastCSharp.code.cSharp.tcpServer, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<clientType>, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify, clientType> getClient;
            /// <summary>
            /// 验证函数接口
            /// </summary>
            protected fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<clientType> verifyMethod;
            /// <summary>
            /// TCP调用客户端路由
            /// </summary>
            /// <param name="getClient"></param>
            /// <param name="attribute"></param>
            /// <param name="verifyMethod"></param>
            /// <param name="verify"></param>
            public routerBase(Func<fastCSharp.code.cSharp.tcpServer, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<clientType>, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify, clientType> getClient
                , fastCSharp.code.cSharp.tcpServer attribute, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<clientType> verifyMethod, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify)
#endif
            {
                if (getClient == null) log.Error.Throw(log.exceptionType.Null);
                client = (this.getClient = getClient)(attribute, verifyMethod, verify);
                commandClient = (commandClient<clientType>)client.TcpCommandClient;
                this.attribute = commandClient.Attribute;
                this.verifyMethod = commandClient.verifyMethod;
                this.verify = commandClient.Verify;
            }
        }
        /// <summary>
        ///  TCP调用客户端路由
        /// </summary>
        /// <typeparam name="customType">调用附加信息类型</typeparam>
        public abstract class router<customType> : routerBase, IDisposable
        {
            /// <summary>
            /// TCP调用客户端服务端口信息
            /// </summary>
            public struct clientHost
            {
                /// <summary>
                /// TCP调用客户端
                /// </summary>
                public clientType Client;
                /// <summary>
                /// TCP服务端口信息
                /// </summary>
                public host Host;
                /// <summary>
                /// 调用附加信息
                /// </summary>
                public customType Custom;
                /// <summary>
                /// 设置TCP调用客户端服务端口信息
                /// </summary>
                /// <param name="client"></param>
                /// <param name="host"></param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Set(clientType client, ref host host)
                {
                    Client = client;
                    Host = host;
                    Custom = default(customType);
                }
                /// <summary>
                /// 设置TCP调用客户端服务端口信息
                /// </summary>
                /// <param name="client"></param>
                /// <param name="host"></param>
                /// <param name="custom"></param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Set(clientType client, ref host host, customType custom)
                {
                    Client = client;
                    Host = host;
                    Custom = custom;
                }
            }
            /// <summary>
            /// 创建TCP调用客户端
            /// </summary>
            protected sealed class clientCreator : routerClientCreator
            {
                /// <summary>
                /// TCP调用客户端路由
                /// </summary>
                private router<customType> router;
                /// <summary>
                /// TCP服务端口信息
                /// </summary>
                private host host;
                /// <summary>
                /// TCP调用客户端
                /// </summary>
                private clientType client;
                /// <summary>
                /// TCP服务调用配置
                /// </summary>
                private fastCSharp.code.cSharp.tcpServer attribute;
                /// <summary>
                /// 创建客户端重试间隔秒数
                /// </summary>
                private int retrySeconds;
                /// <summary>
                /// 创建TCP调用客户端
                /// </summary>
                /// <param name="router"></param>
                /// <param name="host"></param>
                public clientCreator(router<customType> router, host host)
                {
                    this.router = router;
                    this.host = host;
                    retrySeconds = router.attribute.LoadBalancingRouterRetrySeconds;
                }
                /// <summary>
                /// 创建TCP调用客户端
                /// </summary>
                internal override void CreateThread()
                {
                    Create();
                }
                /// <summary>
                /// 创建TCP调用客户端
                /// </summary>
                public void Create()
                {
                    if (attribute == null)
                    {
                        attribute = router.attribute.Clone();
                        attribute.TcpRegister = null;
                        attribute.Host = host.Host;
                        attribute.Port = host.Port;
                    }
                    (client = router.getClient(attribute, router.verifyMethod, router.verify)).TcpCommandClient.GetStreamSocket(this);
                }
                /// <summary>
                /// 创建TCP调用客户端
                /// </summary>
                /// <param name="socket"></param>
                internal override void OnCreated(streamCommandSocket socket)
                {
                    if (socket == null)
                    {
                        pub.Dispose(ref client);
                        if (retrySeconds > 0)
                        {
                            DateTime time = date.Now.AddSeconds(retrySeconds);
                            retrySeconds = 0;
                            fastCSharp.threading.timerTask.Default.Add(this, thread.callType.TcpCommandClientRouterClientCreator, time);
                            return;
                        }
                    }
                    router.onCreated(client, host);
                }
            }
            /// <summary>
            /// TCP调用客户端集合
            /// </summary>
            protected clientHost[] clients;
            /// <summary>
            /// TCP调用客户端路由
            /// </summary>
            /// <param name="getClient"></param>
            /// <param name="attribute"></param>
            /// <param name="verifyMethod"></param>
            /// <param name="verify"></param>
#if NOJIT
            public router(Func<fastCSharp.code.cSharp.tcpServer, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify, clientType> getClient
                , fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject verifyMethod = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify = null)
#else
            public router(Func<fastCSharp.code.cSharp.tcpServer, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<clientType>, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify, clientType> getClient
                , fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<clientType> verifyMethod = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify = null)
#endif
                : base(getClient, attribute, verifyMethod, verify)
            {
                clients = new clientHost[4];
                //tcpRegisterClient = commandClient.TcpRegisterClient;
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                Monitor.Enter(clientLock);
                if (isDisposed == 0)
                {
                    isDisposed = 1;
                    Monitor.Exit(clientLock);
                    if (commandClient != null) commandClient.OnChangeTcpRegisterServicesVersion -= onServiceChanged;
                    pub.Dispose(ref this.client);
                    commandClient = null;
                    Monitor.Enter(clientLock);
                    int count = this.count;
                    this.count = hostCount = 0;
                    Monitor.Exit(clientLock);
                    while (count != 0) pub.Dispose(ref clients[--count].Client);
                    //tcpRegisterClient = null;
                    verifyMethod = null;
                    verify = null;
                }
                else Monitor.Exit(clientLock);
            }
            /// <summary>
            /// 设置TCP服务信息更新事件
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected void start()
            {
                commandClient.OnChangeTcpRegisterServicesVersion += onServiceChanged;
                onServiceChanged();
            }
            /// <summary>
            /// TCP服务信息更新事件
            /// </summary>
            private void onServiceChanged()
            {
                subArray<clientType> removeClients = default(subArray<clientType>);
                subArray<host> newHosts = default(subArray<host>);
                Monitor.Enter(clientLock);
                if (isDisposed == 0)// && commandClient.TcpRegisterServices.Version != commandClient.TcpRegisterServicesVersion
                {
                    //commandClient.TcpRegisterServicesVersion = commandClient.TcpRegisterServices.Version;
                    host[] hosts = commandClient.TcpRegisterServices.Hosts;
                    try
                    {
                        for (int index = hostCount; index != 0; )
                        {
                            host clientHost = clients[--index].Host;
                            byte isHost = 0;
                            foreach (host host in hosts)
                            {
                                if (host.Equals(ref clientHost))
                                {
                                    isHost = 1;
                                    break;
                                }
                            }
                            if (isHost == 0)
                            {
                                if (index >= count) clients[index].Host = clients[--hostCount].Host;
                                else
                                {
                                    removeClients.Add(clients[index].Client);
                                    clients[index] = clients[--count];
                                    clients[count] = clients[--hostCount];
                                }
                            }
                        }
                        foreach (host host in hosts)
                        {
                            byte isHost = 0;
                            for (int index = hostCount; index != 0; )
                            {
                                if (host.Equals(ref clients[--index].Host))
                                {
                                    isHost = 1;
                                    break;
                                }
                            }
                            if (isHost == 0)
                            {
                                if (hostCount == clients.Length)
                                {
                                    clientHost[] newClients = new clientHost[hostCount << 1];
                                    clients.CopyTo(newClients, 0);
                                    clients = newClients;
                                }
                                clients[hostCount++].Host = host;
                                newHosts.Add(host);
                            }
                        }
                    }
                    finally
                    {
                        Monitor.Exit(clientLock);
                        foreach (clientType client in removeClients) pub.Dispose(client);
                        foreach (host host in newHosts) new clientCreator(this, host).Create();
                    }
                }
                else Monitor.Exit(clientLock);
            }
            /// <summary>
            /// 创建TCP调用客户端
            /// </summary>
            /// <param name="client"></param>
            /// <param name="host"></param>
            protected virtual void onCreated(clientType client, host host)
            {
                if (client == null)
                {
                    Monitor.Enter(clientLock);
                    if (isDisposed == 0)
                    {
                        for (int index = hostCount; index != count; )
                        {
                            if (clients[--index].Host.Equals(host))
                            {
                                clients[index].Host = clients[--hostCount].Host;
                                Monitor.Exit(clientLock);
                                return;
                            }
                        }
                    }
                    Monitor.Exit(clientLock);
                }
                else
                {
                    Monitor.Enter(clientLock);
                    if (isDisposed == 0)
                    {
                        for (int index = hostCount; index != count; )
                        {
                            if (clients[--index].Host.Equals(host))
                            {
                                clients[index].Host = clients[count].Host;
                                clients[count++].Set(client, ref host);
                                Monitor.Exit(clientLock);
                                client = null;
                                return;
                            }
                        }
                    }
                    Monitor.Exit(clientLock);
                    pub.Dispose(ref client);
                }
            }
            /// <summary>
            /// 随机获取TCP调用客户端
            /// </summary>
            /// <returns></returns>
            public clientType GetRandom()
            {
                do
                {
                    if (isDisposed != 0) return null;
                    int count = this.count;
                    if (count == 0) return null;
                    int index = random.Default.Next(count);
                    clientType value = clients[index].Client;
                    if (this.count == count && value != null) return isDisposed == 0 ? value : null;
                }
                while (true);
            }
            /// <summary>
            /// 获取所有TCP调用客户端
            /// </summary>
            /// <returns></returns>
            public clientType[] GetAll()
            {
                Monitor.Enter(clientLock);
                if (isDisposed == 0 && count != 0)
                {
                    try
                    {
                        clientType[] values = new clientType[count];
                        int index = count;
                        do
                        {
                            --index;
                            values[index] = clients[index].Client;
                        }
                        while (index != 0);
                        return values;
                    }
                    finally { Monitor.Exit(clientLock); }
                }
                Monitor.Exit(clientLock);
                return nullValue<clientType>.Array;
            }
        }
        /// <summary>
        /// 验证函数客户端
        /// </summary>
        private clientType client;
        /// <summary>
        /// TCP调用服务端
        /// </summary>
        /// <param name="attribute">配置信息</param>
        /// <param name="maxCommandLength">最大命令长度</param>
        /// <param name="verify">验证接口</param>
        public commandClient(fastCSharp.code.cSharp.tcpServer attribute, int maxCommandLength, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify)
            : base(attribute, maxCommandLength, verify)
        {
        }
        /// <summary>
        /// 验证函数接口
        /// </summary>
#if NOJIT
        private fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject verifyMethod;
        /// <summary>
        /// TCP调用服务端
        /// </summary>
        /// <param name="attribute">配置信息</param>
        /// <param name="maxCommandLength">最大命令长度</param>
        /// <param name="verifyMethod">验证函数接口</param>
        /// <param name="client">验证函数客户端</param>
        public commandClient(fastCSharp.code.cSharp.tcpServer attribute, int maxCommandLength, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject verifyMethod, clientType client)
            : base(attribute, maxCommandLength, (fastCSharp.code.cSharp.tcpBase.ITcpClientVerify)null)
        {
            this.verifyMethod = verifyMethod;
            this.client = client;
        }
        /// <summary>
        /// TCP调用服务端
        /// </summary>
        /// <param name="attribute">配置信息</param>
        /// <param name="maxCommandLength">最大命令长度</param>
        /// <param name="verifyMethod">验证函数接口</param>
        /// <param name="client">验证函数客户端</param>
        /// <param name="verify">验证接口</param>
        public commandClient(fastCSharp.code.cSharp.tcpServer attribute, int maxCommandLength, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject verifyMethod, clientType client, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify)
            : base(attribute, maxCommandLength, verify)
        {
            this.verifyMethod = verifyMethod;
            this.client = client;
        }
#else
        private fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<clientType> verifyMethod;
        /// <summary>
        /// TCP调用服务端
        /// </summary>
        /// <param name="attribute">配置信息</param>
        /// <param name="maxCommandLength">最大命令长度</param>
        /// <param name="verifyMethod">验证函数接口</param>
        /// <param name="client">验证函数客户端</param>
        public commandClient(fastCSharp.code.cSharp.tcpServer attribute, int maxCommandLength, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<clientType> verifyMethod, clientType client)
            : base(attribute, maxCommandLength, (fastCSharp.code.cSharp.tcpBase.ITcpClientVerify)null)
        {
            this.verifyMethod = verifyMethod;
            this.client = client;
        }
        /// <summary>
        /// TCP调用服务端
        /// </summary>
        /// <param name="attribute">配置信息</param>
        /// <param name="maxCommandLength">最大命令长度</param>
        /// <param name="verifyMethod">验证函数接口</param>
        /// <param name="client">验证函数客户端</param>
        /// <param name="verify">验证接口</param>
        public commandClient(fastCSharp.code.cSharp.tcpServer attribute, int maxCommandLength, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<clientType> verifyMethod, clientType client, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify)
            : base(attribute, maxCommandLength, verify)
        {
            this.verifyMethod = verifyMethod;
            this.client = client;
        }
#endif
        /// <summary>
        /// 函数验证
        /// </summary>
        /// <returns>是否验证成功</returns>
        protected override bool callVerifyMethod()
        {
            if (verifyMethod == null) return true;
            bool isError = false;
            try
            {
                if (verifyMethod.Verify(client)) return true;
            }
            catch (Exception error)
            {
                isError = true;
                log.Error.Add(error, "TCP客户端验证失败", false);
            }
            if (!isError) log.Error.Add("TCP客户端验证失败", null, false);
            //Dispose();
            return false;
        }
        /// <summary>
        /// 函数验证
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        protected override bool callVerifyMethod(streamCommandSocket socket)
        {
            bool isError = false;
            try
            {
                if (verifyMethod == null)
                {
                    socket.Receive();
                    socket.SetCheck();
                    return true;
                }
                (verifyStreamSocket = socket).Receive();
                if (verifyMethod.Verify(client))
                {
                    verifyStreamSocket = null;
                    socket.SetCheck();
                    return true;
                }
            }
            catch (Exception error)
            {
                isError = true;
                log.Error.Add(error, "TCP客户端验证失败", false);
            }
            verifyStreamSocket = null;
            if (!isError) log.Error.Add("TCP客户端验证失败", null, false);
            //Dispose();
            return false;
        }
    }
}
