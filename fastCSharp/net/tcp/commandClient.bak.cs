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
        /// TCP客户端套接字
        /// </summary>
        public abstract class socket : commandSocket<commandClient>
        {
            /// <summary>
            /// 配置信息
            /// </summary>
            protected fastCSharp.code.cSharp.tcpServer attribute;
            /// <summary>
            /// 最后一次检测时间
            /// </summary>
            protected DateTime lastCheckTime;
            /// <summary>
            /// 检测时间周期
            /// </summary>
            protected readonly long checkTimeTicks;
            /// <summary>
            /// 连接检测设置
            /// </summary>
            protected Action checkHandle;
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
                attribute = commandClient.Attribute;
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
            /// 连接检测设置
            /// </summary>
            internal void SetCheck()
            {
                IsVerifyMethod = true;
                if (commandSocketProxy.Attribute.IsLoadBalancing) loadBalancingCheck();
                else if (checkTimeTicks != 0)
                {
                    lastCheckTime = date.NowSecond;
                    checkHandle();
                }
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
            /// <summary>
            /// 设置会话标识
            /// </summary>
            protected internal void setIdentity()
            {
                identity.Identity = ((int)fastCSharp.pub.Identity ^ (int)fastCSharp.pub.StartTime.Ticks) & int.MaxValue;
                if (identity.Identity == 0) identity.Identity = int.MaxValue;
            }
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
            private static readonly byte[] checkCommandData;
            /// <summary>
            /// 负载均衡连接检测命令数据
            /// </summary>
            private static readonly byte[] loadBalancingCheckCommandData;
            /// <summary>
            /// TCP流回应命令数据
            /// </summary>
            private static readonly byte[] tcpStreamCommandData;
            /// <summary>
            /// 忽略分组命令数据
            /// </summary>
            private static readonly byte[] ignoreGroupCommandData;
            /// <summary>
            /// 命令索引信息
            /// </summary>
            private struct commandIndex
            {
                /// <summary>
                /// 接收数据回调
                /// </summary>
                public pushPool<memoryPool.pushSubArray> OnReceive;
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
                    OnReceive = null;
                    IsKeep = IsTask = 0;
                }
                /// <summary>
                /// 设置接收数据回调
                /// </summary>
                /// <param name="onReceive">接收数据回调</param>
                /// <param name="isTask">回调是否使用任务池</param>
                /// <param name="isKeep">是否保持回调</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Set(pushPool<memoryPool.pushSubArray> onReceive, byte isTask, byte isKeep)
                {
                    OnReceive = onReceive;
                    IsKeep = isKeep;
                    IsTask = isTask;
                }
                /// <summary>
                /// 取消接收数据
                /// </summary>
                /// <returns>接收数据回调+回调是否使用任务池</returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public keyValue<pushPool<memoryPool.pushSubArray>, byte> Cancel()
                {
                    pushPool<memoryPool.pushSubArray> onReceive = OnReceive;
                    byte isTask = IsTask;
                    ++Identity;
                    OnReceive = null;
                    IsKeep = IsTask = 0;
                    return new keyValue<pushPool<memoryPool.pushSubArray>, byte>(onReceive, isTask);
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
                        OnReceive = null;
                        IsKeep = IsTask = 0;
                        return true;
                    }
                    return false;
                }
                /// <summary>
                /// 获取接收数据回调
                /// </summary>
                /// <param name="identity">索引编号</param>
                /// <param name="onReceive">接收数据回调</param>
                /// <param name="isTask">回调是否使用任务池</param>
                /// <returns>是否释放</returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public bool Get(int identity, ref pushPool<memoryPool.pushSubArray> onReceive, ref byte isTask)
                {
                    if (identity == Identity)
                    {
                        onReceive = OnReceive;
                        isTask = IsTask;
                        if (IsKeep == 0)
                        {
                            ++Identity;
                            OnReceive = null;
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
                internal static readonly keepCallback Null = new keepCallback(null, 0, 0, 0);
                /// <summary>
                /// 终止回调委托
                /// </summary>
                private Action<int, int, int> cancel;
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
                /// 保持回调
                /// </summary>
                /// <param name="cancel">终止回调委托</param>
                /// <param name="identity">保持回调序号</param>
                /// <param name="commandIndex">命令集合索引</param>
                /// <param name="commandIdentity">命令序号</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                internal keepCallback(Action<int, int, int> cancel, int identity, int commandIndex, int commandIdentity)
                {
                    this.cancel = cancel;
                    this.identity = identity;
                    this.commandIndex = commandIndex;
                    this.commandIdentity = commandIdentity;
                }
                /// <summary>
                /// 终止回调
                /// </summary>
                public void Dispose()
                {
                    Action<int, int, int> cancel = this.cancel;
                    this.cancel = null;
                    if (cancel != null) cancel(identity, commandIndex, commandIdentity);
                }
            }
            /// <summary>
            /// 命令索引信息集合
            /// </summary>
            private commandIndex[] commandIndexs;
            /// <summary>
            /// 命令信息空闲索引集合
            /// </summary>
            private readonly list<int> freeIndexs = new list<int>();
            /// <summary>
            /// 命令信息集合最大索引号
            /// </summary>
            private int maxIndex;
            /// <summary>
            /// 命令索引信息集合访问锁
            /// </summary>
            private int commandIndexLock;
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
            /// <summary>
            /// 接收会话标识
            /// </summary>
            private Action<int> onReceiveIdentityHandle;
            /// <summary>
            /// 创建命令输入数据并执行
            /// </summary>
            private Action buildCommandHandle;
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
                commandIndexs = new commandIndex[255];
                maxIndex = 2;
                commandIndexs[0].Set(doTcpStream, 0, 1);
                commandIndexs[1].Set(loadBalancingCheckTime, 0, 1);
                onReceiveIdentityHandle = onReceiveIdentity;
                checkHandle = check;
                buildCommandHandle = buildCommand;
                //isOutputDebug = commandClient.attribute.IsOutputDebug;
            }
            /// <summary>
            /// 是否可以添加命令
            /// </summary>
            private byte disabledCommand;
            /// <summary>
            /// 关闭套接字连接
            /// </summary>
            protected override void dispose()
            {
                interlocked.CompareSetYieldSleep0(ref commandLock);
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
                    pushPool<memoryPool.pushSubArray>[] onReceives = null;
                    interlocked.NoCheckCompareSetSleep0(ref commandIndexLock);
                    try
                    {
                        int count = freeIndexs.Count;
                        if (count != 0)
                        {
                            foreach (int index in freeIndexs.array)
                            {
                                commandIndexs[index].Clear();
                                if (--count == 0) break;
                            }
                            freeIndexs.Empty();
                        }
                        if (maxIndex != 0)
                        {
                            onReceives = new pushPool<memoryPool.pushSubArray>[maxIndex];
                            do
                            {
                                --maxIndex;
                                onReceives[maxIndex] = commandIndexs[maxIndex].Cancel().Key;
                            }
                            while (maxIndex != 0);
                        }
                    }
                    finally { commandIndexLock = 0; }
                    if (onReceives != null) fastCSharp.threading.task.Tiny.Add(cancelOnReceives, onReceives, null);
                    buildCommands.Clear();
                }
                finally { base.dispose(); }
            }
            /// <summary>
            /// 取消命令回调
            /// </summary>
            /// <param name="onReceives">命令回调集合</param>
            private static void cancelOnReceives(pushPool<memoryPool.pushSubArray>[] onReceives)
            {
                memoryPool.pushSubArray pushData = default(memoryPool.pushSubArray);
                foreach (pushPool<memoryPool.pushSubArray> onReceive in onReceives)
                {
                    if (onReceive != null) onReceive(ref pushData);
                }
            }
            /// <summary>
            /// 连接检测设置
            /// </summary>
            private void check()
            {
                if (isDisposed == 0)
                {
                    DateTime checkTime = lastCheckTime.AddTicks(checkTimeTicks);
                    if (checkTime <= date.NowSecond)
                    {
                        if (attribute.IsIdentityCommand) Call(null, commandServer.CheckIdentityCommand, false, false);
                        else Call(null, checkCommandData, false, false);
                        timerTask.Default.Add(checkHandle, (lastCheckTime = date.NowSecond).AddTicks(checkTimeTicks), null);
                    }
                    else timerTask.Default.Add(checkHandle, checkTime, null);
                }
            }
            /// <summary>
            /// 负载均衡连接检测
            /// </summary>
            protected override void loadBalancingCheck()
            {
                if (attribute.IsIdentityCommand) Call(null, commandServer.LoadBalancingCheckIdentityCommand, false, false);
                else Call(null, loadBalancingCheckCommandData, false, false);
            }
            /// <summary>
            /// 负载均衡连接检测
            /// </summary>
            /// <returns>是否成功</returns>
            internal bool LoadBalancingCheck()
            {
                fastCSharp.code.cSharp.asynchronousMethod.waitCall wait = fastCSharp.code.cSharp.asynchronousMethod.waitCall.Get();
                if (wait != null)
                {
                    if (attribute.IsIdentityCommand) Call(wait.OnReturn, commandServer.LoadBalancingCheckIdentityCommand, false, false);
                    else Call(wait.OnReturn, loadBalancingCheckCommandData, false, false);
                    return wait.Value.IsReturn;
                }
                return false;
            }
            /// <summary>
            /// 忽略TCP调用分组
            /// </summary>
            /// <param name="groupId">分组标识</param>
            /// <returns>是否调用成功</returns>
            internal bool IgnoreGroup(int groupId)
            {
                asynchronousMethod.waitCall wait = asynchronousMethod.waitCall.Get();
                if (wait != null)
                {
                    if (attribute.IsIdentityCommand) Call(wait.OnReturn, ref groupId, commandServer.IgnoreGroupCommand, int.MaxValue, false, false);
                    else Call(wait.OnReturn, ignoreGroupCommandData, ref groupId, int.MaxValue, false, false);
                    return wait.Value.IsReturn;
                }
                return false;
            }
            /// <summary>
            /// 获取命令信息集合索引
            /// </summary>
            /// <param name="onReceive">接收数据回调</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <param name="isKeep">是否保持回调</param>
            /// <returns>命令信息集合索引</returns>
            private commandServer.streamIdentity newIndex(pushPool<memoryPool.pushSubArray> onReceive, byte isTask, byte isKeep)
            {
                int index;
                interlocked.CompareSetYieldSleep0(ref commandIndexLock);
                if (freeIndexs.Count != 0)
                {
                    index = freeIndexs.UnsafePop();
                    commandIndexLock = 0;
                }
                else if (maxIndex == commandIndexs.Length)
                {
                    try
                    {
                        commandIndex[] newCommands = new commandIndex[maxIndex << 1];
                        Array.Copy(commandIndexs, 0, newCommands, 0, maxIndex);
                        index = maxIndex++;
                        commandIndexs = newCommands;
                    }
                    finally { commandIndexLock = 0; }
                }
                else
                {
                    index = maxIndex++;
                    commandIndexLock = 0;
                }
                commandIndexs[index].Set(onReceive, isTask, isKeep);
                return new commandServer.streamIdentity { Index = index, Identity = commandIndexs[index].Identity };
            }
            /// <summary>
            /// 释放命令信息集合索引
            /// </summary>
            /// <param name="index">命令信息集合索引</param>
            private void freeIndex(int index)
            {
                interlocked.CompareSetYieldSleep0(ref commandIndexLock);
                if ((uint)index < commandIndexs.Length)
                {
                    commandIndexs[index].Clear();
                    freeIndexLock(index);
                }
                else commandIndexLock = 0;
            }
            /// <summary>
            /// 释放命令信息集合索引
            /// </summary>
            /// <param name="index">命令信息集合索引</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private void freeIndexLock(int index)
            {
                if (freeIndexs.FreeLength == 0)
                {
                    try
                    {
                        freeIndexs.Add(index);
                    }
                    finally { commandIndexLock = 0; }
                }
                else
                {
                    freeIndexs.UnsafeAdd(index);
                    commandIndexLock = 0;
                }
            }
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
                        typePool<tcpStreamReader>.Push(this);
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
                public void Read(streamCommandSocket socket, Stream stream, ref commandServer.tcpStreamParameter parameter, ref byte[] buffer)
                {
                    this.parameter = parameter;
                    this.socket = socket;
                    parameter.Data.UnsafeSet(buffer, 0, 0);
                    this.stream = stream;
                    buffer = null;
                    stream.BeginRead(this.parameter.Data.UnsafeArray, 0, (int)this.parameter.Offset, callback, this);
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
                        typePool<tcpStreamWriter>.Push(this);
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
                    stream.BeginWrite(data.UnsafeArray, data.StartIndex, data.Count, callback, this);
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
                                        stream.Write(parameter.Data.UnsafeArray, parameter.Data.StartIndex, parameter.Data.Count);
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
                    asyncBuffers.Push(ref buffer);
                }
            }
            /// <summary>
            /// 服务器端负载均衡联通测试
            /// </summary>
            /// <param name="data">输出数据</param>
            private void loadBalancingCheckTime(ref memoryPool.pushSubArray data)
            {
                commandSocketProxy.LoadBalancingCheckTime = date.NowSecond;
            }
            /// <summary>
            /// 发送TCP流参数
            /// </summary>
            /// <param name="parameter">TCP流参数</param>
            private void doTcpStream(commandServer.tcpStreamParameter parameter)
            {
                if (isDisposed == 0)
                {
                    if (attribute.IsIdentityCommand) Call(parameter.PushClientBuffer, ref parameter, commandServer.TcpStreamCommand, int.MaxValue, false, false);
                    else Call(parameter.PushClientBuffer, tcpStreamCommandData, ref parameter, int.MaxValue, false, false);
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
            private int isCommandBuilding;
            /// <summary>
            /// 是否正在创建命令
            /// </summary>
            private byte isBuildCommand;
            /// <summary>
            /// 命令输入数据是否饱和
            ///// </summary>
            //internal bool IsBusy;
            /// <summary>
            /// 添加命令
            /// </summary>
            /// <param name="command">当前命令</param>
            private void pushCommand(command command)
            {
                interlocked.CompareSetYieldSleep0(ref commandLock);
                if (disabledCommand == 0)
                {
                    byte isBuildCommand = this.isBuildCommand;
                    commands.Push(command);
                    this.isBuildCommand = 1;
                    commandLock = 0;
                    if (isBuildCommand == 0) threadPool.TinyPool.FastStart(buildCommandHandle, null, null);
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
            private unsafe void buildCommand()
            {
                TimeSpan sleepTime = new TimeSpan(0, 0, 0, 0, commandSocketProxy.Attribute.ClientSendSleep);
                int bufferSize = BigBuffers.Size, bufferSize2 = bufferSize >> 1, maxCount = commandSocketProxy.Attribute.MaxClientSendCount;
                commandBuilder commandBuilder = new commandBuilder
                {
                    Socket = this,
                    CommandStream = commandStream,
                    MergeIndex = attribute.IsIdentityCommand ? (sizeof(int) * 2 + sizeof(commandServer.streamIdentity)) : (sizeof(int) * 3 + sizeof(commandServer.streamIdentity))
                };
                interlocked.NoCheckCompareSetSleep0(ref isCommandBuilding);
                try
                {
                START:
                    byte[] buffer = sendData;
                    fixed (byte* dataFixed = buffer)
                    {
                        commandBuilder.Reset(dataFixed, buffer.Length);
                        do
                        {
                            interlocked.CompareSetYieldSleep0(ref commandLock);
                            command command = commands.Pop();
                            if (command == null)
                            {
                                if (buildCommands.Head == null)
                                {
                                    isBuildCommand = 0;
                                    commandLock = 0;
                                    commandStream.Dispose();
                                    isCommandBuilding = 0;
                                    return;
                                }
                                commandLock = 0;
                                commandBuilder.Send();
                                if (sendData != buffer) goto START;
                            }
                            else
                            {
                                commandLock = 0;
                                commandBuilder.Build(command);
                                if (commandBuilder.CommandCount == maxCount || commandStream.Length + commandBuilder.MaxCommandLength > bufferSize || !IsVerifyMethod)
                                {
                                    //IsBusy = true;
                                    commandBuilder.Send();
                                    //IsBusy = false;
                                    if (sendData != buffer) goto START;
                                }
                                if (commands.Head == null && commandStream.Length <= bufferSize2)
                                {
                                    //IsBusy = false;
                                    Thread.Sleep(sleepTime);
                                }
                            }
                        }
                        while (true);
                    }
                }
                catch (Exception error)
                {
                    commandBuilder.Cancel();
                    buildCommands.Clear();
                    interlocked.CompareSetYieldSleep0(ref commandLock);
                    isBuildCommand = 0;
                    commandLock = 0;
                    commandStream.Dispose();
                    isCommandBuilding = 0;
                    Socket.shutdown();
                    log.Error.Add(error, attribute.ServiceName, false);
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
                /// 重置命令流
                /// </summary>
                /// <param name="data">命令流数据起始位置</param>
                /// <param name="length">命令流字节长度</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Reset(byte* data, int length)
                {
                    CommandStream.UnsafeReset(dataFixed = data, bufferLength = length);
                    CommandStream.UnsafeSetLength(MergeIndex);
                    MaxCommandLength = CommandCount = 0;
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
                    pushPool<byte[]> pushPool = null;
                    int commandLength = CommandStream.Length, dataLength = commandLength - MergeIndex, isNewBuffer = 0;
                    if (Socket.buildCommands.IsSingle)
                    {
                        if (commandLength <= bufferLength)
                        {
                            if (CommandStream.DataLength != bufferLength)
                            {
                                unsafer.memory.Copy(CommandStream.Data + MergeIndex, dataFixed + MergeIndex, dataLength);
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
                            subArray<byte> compressData = stream.Deflate.GetCompressUnsafe(data.UnsafeArray, buildIndex, dataLength - startIndex, startIndex, fastCSharp.memoryPool.StreamBuffers);
                            if (compressData.array != null)
                            {
                                fixed (byte* compressFixed = compressData.array, sendFixed = data.UnsafeArray)
                                {
                                    unsafer.memory.Copy(sendFixed + MergeIndex, compressFixed, startIndex - sizeof(int));
                                    *(int*)(compressFixed + startIndex - sizeof(int)) = -compressData.Count;
                                }
                                data.UnsafeSet(compressData.array, 0, compressData.Count + startIndex);
                                pushPool = fastCSharp.memoryPool.StreamBuffers.PushHandle;
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
                            if (CommandStream.DataLength != bufferLength)
                            {
                                unsafer.memory.Copy(CommandStream.Data + MergeIndex, dataFixed + MergeIndex, commandLength - MergeIndex);
                                CommandStream.UnsafeReset(dataFixed, bufferLength);
                            }
                            data.UnsafeSet(Socket.sendData, 0, commandLength);
                        }
                        if (Socket.attribute.IsCompress && dataLength > unmanagedStreamBase.DefaultLength)
                        {
                            subArray<byte> compressData = stream.Deflate.GetCompressUnsafe(data.UnsafeArray, MergeIndex, dataLength, MergeIndex, fastCSharp.memoryPool.StreamBuffers);
                            if (compressData.array != null)
                            {
                                dataLength = -compressData.Count;
                                data.UnsafeSet(compressData.array, 0, compressData.Count + MergeIndex);
                                pushPool = fastCSharp.memoryPool.StreamBuffers.PushHandle;
                            }
                        }
                        fixed (byte* megerDataFixed = data.UnsafeArray)
                        {
                            byte* write;
                            if (Socket.attribute.IsIdentityCommand) *(int*)(write = megerDataFixed) = commandServer.StreamMergeIdentityCommand;
                            else
                            {
                                write = megerDataFixed + sizeof(int);
                                *(int*)megerDataFixed = sizeof(int) * 3 + sizeof(commandServer.streamIdentity);
                                *(int*)write = commandServer.StreamMergeIdentityCommand + commandServer.CommandDataIndex;
                            }
                            //*(commandServer.streamIdentity*)(write + sizeof(int)) = default(commandServer.streamIdentity);
                            *(int*)(write + (sizeof(int) + sizeof(commandServer.streamIdentity))) = dataLength;
                        }
                    }
                    if (isNewBuffer == 0) CommandStream.UnsafeSetLength(MergeIndex);
                    Socket.lastCheckTime = date.NowSecond;
                    try
                    {
                        CommandCount = 0;
                        //if (Socket.isOutputDebug) commandServer.DebugLog.Add(Socket.attribute.ServiceName + ".Send(" + data.Length.toString() + ")", false, false);
                        if (Socket.send(ref data)) Socket.buildCommands.Clear();
                        else Cancel();
                    }
                    finally
                    {
                        if (pushPool != null) pushPool(ref data.array);
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
            private abstract class command
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
                /// 接收数据回调处理
                /// </summary>
                public pushPool<memoryPool.pushSubArray> OnReceive;
                /// <summary>
                /// 客户端命令
                /// </summary>
                protected command()
                {
                    OnReceive = onReceive;
                }
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
                protected abstract void onReceive(ref memoryPool.pushSubArray data);
                /// <summary>
                /// 取消错误命令
                /// </summary>
                public virtual void Cancel()
                {
                    memoryPool.pushSubArray pushData = default(memoryPool.pushSubArray);
                    Socket.onReceive(Identity, ref pushData, 1, false);
                }
            }
            /// <summary>
            /// 客户端命令
            /// </summary>
            /// <typeparam name="commandType">客户端命令类型</typeparam>
            private abstract class asyncCommand<commandType> : command
                where commandType : asyncCommand<commandType>
            {
                /// <summary>
                /// 客户端命令
                /// </summary>
                protected asyncCommand()
                {
                    thisCommand = (commandType)this;
                }
                /// <summary>
                /// 当前命令
                /// </summary>
                private commandType thisCommand;
                /// <summary>
                /// 保持回调
                /// </summary>
                public keepCallback KeepCallback;
                /// <summary>
                /// 终止保持回调
                /// </summary>
                protected Action<int, int, int> cancelKeepCallback;
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
                        KeepCallback = new keepCallback(cancelKeepCallback, ++keepCallbackIdentity, Identity.Index, Socket.commandIndexs[Identity.Index].Identity);
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
                        typePool<commandType>.Push(thisCommand);
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
                /// 客户端命令
                /// </summary>
                protected asyncCommand()
                {
                    cancelKeepCallback = cancelKeep;
                }
                /// <summary>
                /// 添加回调对象
                /// </summary>
                /// <param name="value">回调值</param>
                protected void push(callbackType value)
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
                /// <summary>
                /// 回调处理
                /// </summary>
                /// <param name="value">回调值</param>
                protected void onlyCallback(callbackType value)
                {
                    Action<callbackType> callback = Callback;
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
                /// <summary>
                /// 终止保持回调
                /// </summary>
                /// <param name="identity">保持回调序号</param>
                /// <param name="commandIndex">命令集合索引</param>
                /// <param name="commandIdentity">命令序号</param>
                private void cancelKeep(int identity, int commandIndex, int commandIdentity)
                {
                    if (Interlocked.CompareExchange(ref keepCallbackIdentity, identity + 1, identity) == identity)
                    {
                        memoryPool.pushSubArray pushData = default(memoryPool.pushSubArray);
                        Callback = null;
                        Socket.cancel(commandIndex, commandIdentity);
                        onReceive(ref pushData);
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
                public byte[] Command;
                /// <summary>
                /// 创建第一个命令输入数据
                /// </summary>
                /// <param name="stream">命令内存流</param>
                /// <returns>数据起始位置,失败返回0</returns>
                public unsafe override int BuildIndex(unmanagedStream stream)
                {
                    stream.PrepLength(Command.Length + (sizeof(int) + sizeof(commandServer.streamIdentity)));
                    byte* write = stream.CurrentData;
                    unsafer.memory.Copy(Command, write, Command.Length);
                    write += Command.Length;
                    *(commandServer.streamIdentity*)(write) = Identity;
                    *(int*)(write + sizeof(commandServer.streamIdentity)) = 0;
                    stream.UnsafeAddLength(Command.Length + (sizeof(int) + sizeof(commandServer.streamIdentity)));
                    return stream.Length;
                }
            }
            /// <summary>
            /// 客户端命令
            /// </summary>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            private sealed class asyncOutputDataCommand<outputParameterType> : asyncDataCommand<asyncOutputDataCommand<outputParameterType>, asynchronousMethod.returnValue<outputParameterType>>
            {
                /// <summary>
                /// 输出参数
                /// </summary>
                public outputParameterType OutputParameter;
                /// <summary>
                /// 输出参数访问锁
                /// </summary>
                private int outputLock;
                /// <summary>
                /// 接收数据回调处理
                /// </summary>
                /// <param name="data">输出数据</param>
                protected override void onReceive(ref memoryPool.pushSubArray data)
                {
                    bool isReturn = false;
                    byte[] buffer = data.Value.UnsafeArray;
                    if (KeepCallback == null)
                    {
                        try
                        {
                            if (buffer != null && fastCSharp.emit.dataDeSerializer.DeSerialize(ref data.Value, ref OutputParameter)) isReturn = true;
                        }
                        finally
                        {
                            outputParameterType outputParameter = OutputParameter;
                            OutputParameter = default(outputParameterType);
                            push(new fastCSharp.code.cSharp.asynchronousMethod.returnValue<outputParameterType> { IsReturn = isReturn, Value = outputParameter });
                            data.Push();
                        }
                    }
                    else if (buffer == null)
                    {
                        onlyCallback(new fastCSharp.code.cSharp.asynchronousMethod.returnValue<outputParameterType> { IsReturn = false });
                    }
                    else
                    {
                        interlocked.CompareSetYieldSleep0(ref outputLock);
                        outputParameterType outputParameter = OutputParameter;
                        try
                        {
                            if (fastCSharp.emit.dataDeSerializer.DeSerialize(ref data.Value, ref outputParameter)) isReturn = true;
                        }
                        finally
                        {
                            outputLock = 0;
                            onlyCallback(new fastCSharp.code.cSharp.asynchronousMethod.returnValue<outputParameterType> { IsReturn = isReturn, Value = outputParameter });
                            data.Push();
                        }
                    }
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="commandData">TCP调用命令</param>
                /// <param name="outputParameter">输出参数</param>
                /// <param name="isTask">回调是否使用任务池</param>
                /// <param name="isKeepCallback">是否保持回调</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, Action<asynchronousMethod.returnValue<outputParameterType>> onGet
                    , byte[] commandData, outputParameterType outputParameter, bool isTask, bool isKeepCallback)
                {
                    Socket = socket;
                    Callback = onGet;
                    Command = commandData;
                    OutputParameter = outputParameter;
                    Identity = socket.newIndex(OnReceive, isTask ? (byte)1 : (byte)0, isKeepCallback ? (byte)1 : (byte)0);
                    if (isKeepCallback)
                    {
                        if (SetKeepCallback()) return true;
                        OutputParameter = default(outputParameterType);
                        Callback = null;
                        push();
                        return false;
                    }
                    return true;
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
            /// <param name="commandData">TCP调用命令</param>
            /// <param name="outputParameter">输出参数</param>
            /// <param name="maxLength">输入参数数据最大长度</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <param name="isKeepCallback">是否保持回调</param>
            /// <returns>保持回调</returns>
            public keepCallback Get<outputParameterType>
                (Action<asynchronousMethod.returnValue<outputParameterType>> onGet, byte[] commandData
                , outputParameterType outputParameter, bool isTask, bool isKeepCallback)
            {
                if (isDisposed == 0)
                {
                    asyncOutputDataCommand<outputParameterType> command = asyncOutputDataCommand<outputParameterType>.Get();
                    if (command != null && command.Set(this, onGet, commandData, outputParameter, isTask, isKeepCallback))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                }
                onGet(new asynchronousMethod.returnValue<outputParameterType> { IsReturn = false });
                return null;
            }
            /// <summary>
            /// 客户端命令
            /// </summary>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            private sealed class asyncOutputJsonDataCommand<outputParameterType> : asyncDataCommand<asyncOutputJsonDataCommand<outputParameterType>, asynchronousMethod.returnValue<outputParameterType>>
            {
                /// <summary>
                /// 输出参数
                /// </summary>
                public tcpBase.parameterJsonToSerialize<outputParameterType> OutputParameter;
                /// <summary>
                /// 输出参数访问锁
                /// </summary>
                private int outputLock;
                /// <summary>
                /// 接收数据回调处理
                /// </summary>
                /// <param name="data">输出数据</param>
                protected override void onReceive(ref memoryPool.pushSubArray data)
                {
                    bool isReturn = false;
                    byte[] buffer = data.Value.UnsafeArray;
                    if (KeepCallback == null)
                    {
                        try
                        {
                            if (buffer != null && fastCSharp.emit.dataDeSerializer.DeSerialize(ref data.Value, ref OutputParameter)) isReturn = true;
                        }
                        finally
                        {
                            outputParameterType outputParameter = OutputParameter.Return;
                            OutputParameter.Return = default(outputParameterType);
                            push(new fastCSharp.code.cSharp.asynchronousMethod.returnValue<outputParameterType> { IsReturn = isReturn, Value = outputParameter });
                            data.Push();
                        }
                    }
                    else if (buffer == null)
                    {
                        onlyCallback(new fastCSharp.code.cSharp.asynchronousMethod.returnValue<outputParameterType> { IsReturn = false });
                    }
                    else
                    {
                        interlocked.CompareSetYieldSleep0(ref outputLock);
                        tcpBase.parameterJsonToSerialize<outputParameterType> outputParameter = OutputParameter;
                        try
                        {
                            if (fastCSharp.emit.dataDeSerializer.DeSerialize(ref data.Value, ref outputParameter)) isReturn = true;
                        }
                        finally
                        {
                            outputLock = 0;
                            onlyCallback(new fastCSharp.code.cSharp.asynchronousMethod.returnValue<outputParameterType> { IsReturn = isReturn, Value = outputParameter.Return });
                            data.Push();
                        }
                    }
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="commandData">TCP调用命令</param>
                /// <param name="outputParameter">输出参数</param>
                /// <param name="isTask">回调是否使用任务池</param>
                /// <param name="isKeepCallback">是否保持回调</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, Action<asynchronousMethod.returnValue<outputParameterType>> onGet
                    , byte[] commandData, outputParameterType outputParameter, bool isTask, bool isKeepCallback)
                {
                    Socket = socket;
                    Callback = onGet;
                    Command = commandData;
                    OutputParameter = new tcpBase.parameterJsonToSerialize<outputParameterType> { Return = outputParameter };
                    Identity = socket.newIndex(OnReceive, isTask ? (byte)1 : (byte)0, isKeepCallback ? (byte)1 : (byte)0);
                    if (isKeepCallback)
                    {
                        if (SetKeepCallback()) return true;
                        OutputParameter.Return = default(outputParameterType);
                        Callback = null;
                        push();
                        return false;
                    }
                    return true;
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <returns>客户端命令</returns>
                public static asyncOutputJsonDataCommand<outputParameterType> Get()
                {
                    asyncOutputJsonDataCommand<outputParameterType> command = typePool<asyncOutputJsonDataCommand<outputParameterType>>.Pop();
                    if (command == null)
                    {
                        try
                        {
                            command = new asyncOutputJsonDataCommand<outputParameterType>();
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
            /// <param name="commandData">TCP调用命令</param>
            /// <param name="outputParameter">输出参数</param>
            /// <param name="maxLength">输入参数数据最大长度</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <param name="isKeepCallback">是否保持回调</param>
            /// <returns>保持回调</returns>
            public keepCallback GetJson<outputParameterType>
                (Action<asynchronousMethod.returnValue<outputParameterType>> onGet
                , byte[] commandData, outputParameterType outputParameter, bool isTask, bool isKeepCallback)
            {
                if (isDisposed == 0)
                {
                    asyncOutputJsonDataCommand<outputParameterType> command = asyncOutputJsonDataCommand<outputParameterType>.Get();
                    if (command != null && command.Set(this, onGet, commandData, outputParameter, isTask, isKeepCallback))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                }
                onGet(new asynchronousMethod.returnValue<outputParameterType> { IsReturn = false });
                return null;
            }
            /// <summary>
            /// 客户端命令
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            private sealed class asyncInputOutputDataCommand<inputParameterType, outputParameterType> : asyncInputDataCommand<inputParameterType, asynchronousMethod.returnValue<outputParameterType>, asyncInputOutputDataCommand<inputParameterType, outputParameterType>>
            {
                /// <summary>
                /// 输出参数
                /// </summary>
                public outputParameterType OutputParameter;
                /// <summary>
                /// 输出参数访问锁
                /// </summary>
                private int outputLock;
                /// <summary>
                /// 接收数据回调处理
                /// </summary>
                /// <param name="data">输出数据</param>
                protected override void onReceive(ref memoryPool.pushSubArray data)
                {
                    bool isReturn = false;
                    byte[] buffer = data.Value.UnsafeArray;
                    if (KeepCallback == null)
                    {
                        try
                        {
                            if (buffer != null && fastCSharp.emit.dataDeSerializer.DeSerialize(ref data.Value, ref OutputParameter)) isReturn = true;
                        }
                        finally
                        {
                            outputParameterType outputParameter = OutputParameter;
                            OutputParameter = default(outputParameterType);
                            push(new fastCSharp.code.cSharp.asynchronousMethod.returnValue<outputParameterType> { IsReturn = isReturn, Value = outputParameter });
                            data.Push();
                        }
                    }
                    else if (buffer == null)
                    {
                        onlyCallback(new fastCSharp.code.cSharp.asynchronousMethod.returnValue<outputParameterType> { IsReturn = false });
                    }
                    else
                    {
                        interlocked.CompareSetYieldSleep0(ref outputLock);
                        outputParameterType outputParameter = OutputParameter;
                        try
                        {
                            if (fastCSharp.emit.dataDeSerializer.DeSerialize(ref data.Value, ref outputParameter)) isReturn = true;
                        }
                        finally
                        {
                            outputLock = 0;
                            onlyCallback(new fastCSharp.code.cSharp.asynchronousMethod.returnValue<outputParameterType> { IsReturn = isReturn, Value = outputParameter });
                            data.Push();
                        }
                    }
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="onCall">回调委托,返回false表示失败</param>
                /// <param name="commandData">TCP调用命令</param>
                /// <param name="inputParameter">输入参数</param>
                /// <param name="maxLength">输入参数数据最大长度</param>
                /// <param name="isTask">回调是否使用任务池</param>
                /// <param name="isKeepCallback">是否保持回调</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, Action<asynchronousMethod.returnValue<outputParameterType>> onGet
                    , byte[] commandData, ref inputParameterType inputParameter, int maxLength, outputParameterType outputParameter
                    , bool isTask, bool isKeepCallback)
                {
                    Socket = socket;
                    Callback = onGet;
                    Command = commandData;
                    InputParameter = inputParameter;
                    MaxInputSize = maxLength;
                    OutputParameter = outputParameter;
                    Identity = socket.newIndex(OnReceive, isTask ? (byte)1 : (byte)0, isKeepCallback ? (byte)1 : (byte)0);
                    if (isKeepCallback)
                    {
                        if (SetKeepCallback()) return true;
                        InputParameter = default(inputParameterType);
                        OutputParameter = default(outputParameterType);
                        Callback = null;
                        push();
                        return false;
                    }
                    return true;
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
            /// <param name="commandData">TCP调用命令</param>
            /// <param name="inputParameter">输入参数</param>
            /// <param name="maxLength">输入参数数据最大长度</param>
            /// <param name="outputParameter">输出参数</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <param name="isKeepCallback">是否保持回调</param>
            /// <returns>保持回调</returns>
            public keepCallback Get<inputParameterType, outputParameterType>
                (Action<asynchronousMethod.returnValue<outputParameterType>> onGet, byte[] commandData
                , ref inputParameterType inputParameter, int maxLength, outputParameterType outputParameter, bool isTask, bool isKeepCallback)
            {
                if (isDisposed == 0)
                {
                    asyncInputOutputDataCommand<inputParameterType, outputParameterType> command = asyncInputOutputDataCommand<inputParameterType, outputParameterType>.Get();
                    if (command != null && command.Set(this, onGet, commandData, ref inputParameter, maxLength, outputParameter, isTask, isKeepCallback))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                }
                onGet(new asynchronousMethod.returnValue<outputParameterType> { IsReturn = false });
                return null;
            }
            /// <summary>
            /// 客户端命令
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            private sealed class asyncInputOutputJsonDataCommand<inputParameterType, outputParameterType> : asyncInputDataCommand<tcpBase.parameterJsonToSerialize<inputParameterType>, asynchronousMethod.returnValue<outputParameterType>, asyncInputOutputJsonDataCommand<inputParameterType, outputParameterType>>
            {
                /// <summary>
                /// 输出参数
                /// </summary>
                public tcpBase.parameterJsonToSerialize<outputParameterType> OutputParameter;
                /// <summary>
                /// 输出参数访问锁
                /// </summary>
                private int outputLock;
                /// <summary>
                /// 接收数据回调处理
                /// </summary>
                /// <param name="data">输出数据</param>
                protected override void onReceive(ref memoryPool.pushSubArray data)
                {
                    bool isReturn = false;
                    byte[] buffer = data.Value.UnsafeArray;
                    if (KeepCallback == null)
                    {
                        try
                        {
                            if (buffer != null && fastCSharp.emit.dataDeSerializer.DeSerialize(ref data.Value, ref OutputParameter)) isReturn = true;
                        }
                        finally
                        {
                            outputParameterType outputParameter = OutputParameter.Return;
                            OutputParameter.Return = default(outputParameterType);
                            push(new fastCSharp.code.cSharp.asynchronousMethod.returnValue<outputParameterType> { IsReturn = isReturn, Value = outputParameter });
                            data.Push();
                        }
                    }
                    else if (buffer == null)
                    {
                        onlyCallback(new fastCSharp.code.cSharp.asynchronousMethod.returnValue<outputParameterType> { IsReturn = false });
                    }
                    else
                    {
                        interlocked.CompareSetYieldSleep0(ref outputLock);
                        tcpBase.parameterJsonToSerialize<outputParameterType> outputParameter = OutputParameter;
                        try
                        {
                            if (fastCSharp.emit.dataDeSerializer.DeSerialize(ref data.Value, ref outputParameter)) isReturn = true;
                        }
                        finally
                        {
                            outputLock = 0;
                            onlyCallback(new fastCSharp.code.cSharp.asynchronousMethod.returnValue<outputParameterType> { IsReturn = isReturn, Value = outputParameter.Return });
                            data.Push();
                        }
                    }
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="onCall">回调委托,返回false表示失败</param>
                /// <param name="commandData">TCP调用命令</param>
                /// <param name="inputParameter">输入参数</param>
                /// <param name="maxLength">输入参数数据最大长度</param>
                /// <param name="isTask">回调是否使用任务池</param>
                /// <param name="isKeepCallback">是否保持回调</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, Action<asynchronousMethod.returnValue<outputParameterType>> onGet
                    , byte[] commandData, ref inputParameterType inputParameter, int maxLength, outputParameterType outputParameter
                    , bool isTask, bool isKeepCallback)
                {
                    Socket = socket;
                    Callback = onGet;
                    Command = commandData;
                    InputParameter = new tcpBase.parameterJsonToSerialize<inputParameterType> { Return = inputParameter };
                    MaxInputSize = maxLength;
                    OutputParameter = new tcpBase.parameterJsonToSerialize<outputParameterType> { Return = outputParameter };
                    Identity = socket.newIndex(OnReceive, isTask ? (byte)1 : (byte)0, isKeepCallback ? (byte)1 : (byte)0);
                    if (isKeepCallback)
                    {
                        if (SetKeepCallback()) return true;
                        InputParameter.Return = default(inputParameterType);
                        OutputParameter.Return = default(outputParameterType);
                        Callback = null;
                        push();
                        return false;
                    }
                    return true;
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <returns>客户端命令</returns>
                public static asyncInputOutputJsonDataCommand<inputParameterType, outputParameterType> Get()
                {
                    asyncInputOutputJsonDataCommand<inputParameterType, outputParameterType> command = typePool<asyncInputOutputJsonDataCommand<inputParameterType, outputParameterType>>.Pop();
                    if (command == null)
                    {
                        try
                        {
                            command = new asyncInputOutputJsonDataCommand<inputParameterType, outputParameterType>();
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
            /// <param name="commandData">TCP调用命令</param>
            /// <param name="inputParameter">输入参数</param>
            /// <param name="maxLength">输入参数数据最大长度</param>
            /// <param name="outputParameter">输出参数</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <param name="isKeepCallback">是否保持回调</param>
            /// <returns>保持回调</returns>
            public keepCallback GetJson<inputParameterType, outputParameterType>
                (Action<asynchronousMethod.returnValue<outputParameterType>> onGet
                , byte[] commandData, ref inputParameterType inputParameter, int maxLength, outputParameterType outputParameter
                , bool isTask, bool isKeepCallback)
            {
                if (isDisposed == 0)
                {
                    asyncInputOutputJsonDataCommand<inputParameterType, outputParameterType> command = asyncInputOutputJsonDataCommand<inputParameterType, outputParameterType>.Get();
                    if (command != null && command.Set(this, onGet, commandData, ref inputParameter, maxLength, outputParameter, isTask, isKeepCallback))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                }
                onGet(new asynchronousMethod.returnValue<outputParameterType> { IsReturn = false });
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
                public int Command;
                /// <summary>
                /// 创建第一个命令输入数据
                /// </summary>
                /// <param name="stream">命令内存流</param>
                /// <returns>数据起始位置,失败返回0</returns>
                public unsafe override int BuildIndex(unmanagedStream stream)
                {
                    stream.PrepLength(sizeof(int) * 2 + sizeof(commandServer.streamIdentity));
                    byte* write = stream.CurrentData;
                    *(int*)(write) = Command;
                    *(commandServer.streamIdentity*)(write + sizeof(int)) = Identity;
                    *(int*)(write + (sizeof(int) + sizeof(commandServer.streamIdentity))) = 0;
                    stream.UnsafeAddLength(sizeof(int) * 2 + sizeof(commandServer.streamIdentity));
                    return stream.Length;
                }
            }
            /// <summary>
            /// 客户端命令
            /// </summary>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            private sealed class asyncOutputIdentityCommand<outputParameterType> : asyncIdentityCommand<asyncOutputIdentityCommand<outputParameterType>, asynchronousMethod.returnValue<outputParameterType>>
            {
                /// <summary>
                /// 输出参数
                /// </summary>
                public outputParameterType OutputParameter;
                /// <summary>
                /// 输出参数访问锁
                /// </summary>
                private int outputLock;
                /// <summary>
                /// 接收数据回调处理
                /// </summary>
                /// <param name="data">输出数据</param>
                protected override void onReceive(ref memoryPool.pushSubArray data)
                {
                    bool isReturn = false;
                    byte[] buffer = data.Value.UnsafeArray;
                    if (KeepCallback == null)
                    {
                        try
                        {
                            if (buffer != null && fastCSharp.emit.dataDeSerializer.DeSerialize(ref data.Value, ref OutputParameter)) isReturn = true;
                        }
                        finally
                        {
                            outputParameterType outputParameter = OutputParameter;
                            OutputParameter = default(outputParameterType);
                            push(new fastCSharp.code.cSharp.asynchronousMethod.returnValue<outputParameterType> { IsReturn = isReturn, Value = outputParameter });
                            data.Push();
                        }
                    }
                    else if (buffer == null)
                    {
                        onlyCallback(new fastCSharp.code.cSharp.asynchronousMethod.returnValue<outputParameterType> { IsReturn = false });
                    }
                    else
                    {
                        interlocked.CompareSetYieldSleep0(ref outputLock);
                        outputParameterType outputParameter = OutputParameter;
                        try
                        {
                            if (fastCSharp.emit.dataDeSerializer.DeSerialize(ref data.Value, ref outputParameter)) isReturn = true;
                        }
                        finally
                        {
                            outputLock = 0;
                            onlyCallback(new fastCSharp.code.cSharp.asynchronousMethod.returnValue<outputParameterType> { IsReturn = isReturn, Value = outputParameter });
                            data.Push();
                        }
                    }
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="commandIdentity">TCP调用命令</param>
                /// <param name="outputParameter">输出参数</param>
                /// <param name="isTask">回调是否使用任务池</param>
                /// <param name="isKeepCallback">是否保持回调</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, Action<asynchronousMethod.returnValue<outputParameterType>> onGet
                    , int commandIdentity, outputParameterType outputParameter, bool isTask, bool isKeepCallback)
                {
                    Socket = socket;
                    Callback = onGet;
                    Command = commandIdentity;
                    OutputParameter = outputParameter;
                    Identity = socket.newIndex(OnReceive, isTask ? (byte)1 : (byte)0, isKeepCallback ? (byte)1 : (byte)0);
                    if (isKeepCallback)
                    {
                        if (SetKeepCallback()) return true;
                        OutputParameter = default(outputParameterType);
                        Callback = null;
                        push();
                        return false;
                    }
                    return true;
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="commandIdentity">TCP调用命令</param>
                /// <param name="outputParameter">输出参数</param>
                /// <param name="isTask">回调是否使用任务池</param>
                /// <param name="isKeepCallback">是否保持回调</param>
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
            /// <param name="commandIdentity">TCP调用命令</param>
            /// <param name="outputParameter">输出参数</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <param name="isKeepCallback">是否保持回调</param>
            /// <returns>保持回调</returns>
            public keepCallback Get<outputParameterType>
                (Action<asynchronousMethod.returnValue<outputParameterType>> onGet, int commandIdentity
                , outputParameterType outputParameter, bool isTask, bool isKeepCallback)
            {
                if (isDisposed == 0)
                {
                    asyncOutputIdentityCommand<outputParameterType> command = asyncOutputIdentityCommand<outputParameterType>.Get();
                    if (command != null && command.Set(this, onGet, commandIdentity, outputParameter, isTask, isKeepCallback))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                }
                onGet(new asynchronousMethod.returnValue<outputParameterType> { IsReturn = false });
                return null;
            }
            /// <summary>
            /// 客户端命令
            /// </summary>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            private sealed class asyncOutputJsonIdentityCommand<outputParameterType> : asyncIdentityCommand<asyncOutputJsonIdentityCommand<outputParameterType>, asynchronousMethod.returnValue<outputParameterType>>
            {
                /// <summary>
                /// 输出参数
                /// </summary>
                public tcpBase.parameterJsonToSerialize<outputParameterType> OutputParameter;
                /// <summary>
                /// 输出参数访问锁
                /// </summary>
                private int outputLock;
                /// <summary>
                /// 接收数据回调处理
                /// </summary>
                /// <param name="data">输出数据</param>
                protected override void onReceive(ref memoryPool.pushSubArray data)
                {
                    bool isReturn = false;
                    byte[] buffer = data.Value.UnsafeArray;
                    if (KeepCallback == null)
                    {
                        try
                        {
                            if (buffer != null && fastCSharp.emit.dataDeSerializer.DeSerialize(ref data.Value, ref OutputParameter)) isReturn = true;
                        }
                        finally
                        {
                            outputParameterType outputParameter = OutputParameter.Return;
                            OutputParameter.Return = default(outputParameterType);
                            push(new fastCSharp.code.cSharp.asynchronousMethod.returnValue<outputParameterType> { IsReturn = isReturn, Value = outputParameter });
                            data.Push();
                        }
                    }
                    else if (buffer == null)
                    {
                        onlyCallback(new fastCSharp.code.cSharp.asynchronousMethod.returnValue<outputParameterType> { IsReturn = false });
                    }
                    else
                    {
                        interlocked.CompareSetYieldSleep0(ref outputLock);
                        tcpBase.parameterJsonToSerialize<outputParameterType> outputParameter = OutputParameter;
                        try
                        {
                            if (fastCSharp.emit.dataDeSerializer.DeSerialize(ref data.Value, ref outputParameter)) isReturn = true;
                        }
                        finally
                        {
                            outputLock = 0;
                            onlyCallback(new fastCSharp.code.cSharp.asynchronousMethod.returnValue<outputParameterType> { IsReturn = isReturn, Value = outputParameter.Return });
                            data.Push();
                        }
                    }
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="commandIdentity">TCP调用命令</param>
                /// <param name="outputParameter">输出参数</param>
                /// <param name="isTask">回调是否使用任务池</param>
                /// <param name="isKeepCallback">是否保持回调</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, Action<asynchronousMethod.returnValue<outputParameterType>> onGet
                    , int commandIdentity, outputParameterType outputParameter, bool isTask, bool isKeepCallback)
                {
                    Socket = socket;
                    Callback = onGet;
                    Command = commandIdentity;
                    OutputParameter = new tcpBase.parameterJsonToSerialize<outputParameterType> { Return = outputParameter };
                    Identity = socket.newIndex(OnReceive, isTask ? (byte)1 : (byte)0, isKeepCallback ? (byte)1 : (byte)0);
                    if (isKeepCallback)
                    {
                        if (SetKeepCallback()) return true;
                        OutputParameter.Return = default(outputParameterType);
                        Callback = null;
                        push();
                        return false;
                    }
                    return true;
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="commandIdentity">TCP调用命令</param>
                /// <param name="outputParameter">输出参数</param>
                /// <param name="isTask">回调是否使用任务池</param>
                /// <param name="isKeepCallback">是否保持回调</param>
                /// <returns>客户端命令</returns>
                public static asyncOutputJsonIdentityCommand<outputParameterType> Get()
                {
                    asyncOutputJsonIdentityCommand<outputParameterType> command = typePool<asyncOutputJsonIdentityCommand<outputParameterType>>.Pop();
                    if (command == null)
                    {
                        try
                        {
                            command = new asyncOutputJsonIdentityCommand<outputParameterType>();
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
            /// <param name="commandIdentity">TCP调用命令</param>
            /// <param name="outputParameter">输出参数</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <param name="isKeepCallback">是否保持回调</param>
            /// <returns>保持回调</returns>
            public keepCallback GetJson<outputParameterType>
                (Action<asynchronousMethod.returnValue<outputParameterType>> onGet
                , int commandIdentity, outputParameterType outputParameter, bool isTask, bool isKeepCallback)
            {
                if (isDisposed == 0)
                {
                    asyncOutputJsonIdentityCommand<outputParameterType> command = asyncOutputJsonIdentityCommand<outputParameterType>.Get();
                    if (command != null && command.Set(this, onGet, commandIdentity, outputParameter, isTask, isKeepCallback))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                }
                onGet(new asynchronousMethod.returnValue<outputParameterType> { IsReturn = false });
                return null;
            }
            /// <summary>
            /// 客户端命令
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            private sealed class asyncInputOutputIdentityCommand<inputParameterType, outputParameterType> : asyncInputIdentityCommand<inputParameterType, asynchronousMethod.returnValue<outputParameterType>, asyncInputOutputIdentityCommand<inputParameterType, outputParameterType>>
            {
                /// <summary>
                /// 输出参数
                /// </summary>
                public outputParameterType OutputParameter;
                /// <summary>
                /// 输出参数访问锁
                /// </summary>
                private int outputLock;
                /// <summary>
                /// 接收数据回调处理
                /// </summary>
                /// <param name="data">输出数据</param>
                protected override void onReceive(ref memoryPool.pushSubArray data)
                {
                    bool isReturn = false;
                    byte[] buffer = data.Value.UnsafeArray;
                    if (KeepCallback == null)
                    {
                        try
                        {
                            if (buffer != null && fastCSharp.emit.dataDeSerializer.DeSerialize(ref data.Value, ref OutputParameter)) isReturn = true;
                        }
                        finally
                        {
                            outputParameterType outputParameter = OutputParameter;
                            OutputParameter = default(outputParameterType);
                            push(new fastCSharp.code.cSharp.asynchronousMethod.returnValue<outputParameterType> { IsReturn = isReturn, Value = outputParameter });
                            data.Push();
                        }
                    }
                    else if (buffer == null)
                    {
                        onlyCallback(new fastCSharp.code.cSharp.asynchronousMethod.returnValue<outputParameterType> { IsReturn = false });
                    }
                    else
                    {
                        interlocked.CompareSetYieldSleep0(ref outputLock);
                        outputParameterType outputParameter = OutputParameter;
                        try
                        {
                            if (fastCSharp.emit.dataDeSerializer.DeSerialize(ref data.Value, ref outputParameter)) isReturn = true;
                        }
                        finally
                        {
                            outputLock = 0;
                            onlyCallback(new fastCSharp.code.cSharp.asynchronousMethod.returnValue<outputParameterType> { IsReturn = isReturn, Value = outputParameter });
                            data.Push();
                        }
                    }
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="onCall">回调委托,返回false表示失败</param>
                /// <param name="commandIdentity">TCP调用命令</param>
                /// <param name="inputParameter">输入参数</param>
                /// <param name="maxLength">输入参数数据最大长度</param>
                /// <param name="isTask">回调是否使用任务池</param>
                /// <param name="isKeepCallback">是否保持回调</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, Action<asynchronousMethod.returnValue<outputParameterType>> onGet
                    , ref inputParameterType inputParameter, int commandIdentity, int maxLength, outputParameterType outputParameter
                    , bool isTask, bool isKeepCallback)
                {
                    Socket = socket;
                    Callback = onGet;
                    Command = commandIdentity;
                    InputParameter = inputParameter;
                    MaxInputSize = maxLength;
                    OutputParameter = outputParameter;
                    Identity = socket.newIndex(OnReceive, isTask ? (byte)1 : (byte)0, isKeepCallback ? (byte)1 : (byte)0);
                    if (isKeepCallback)
                    {
                        if (SetKeepCallback()) return true;
                        InputParameter = default(inputParameterType);
                        OutputParameter = default(outputParameterType);
                        Callback = null;
                        push();
                        return false;
                    }
                    return true;
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
            /// <param name="commandIdentity">TCP调用命令</param>
            /// <param name="inputParameter">输入参数</param>
            /// <param name="maxLength">输入参数数据最大长度</param>
            /// <param name="outputParameter">输出参数</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <param name="isKeepCallback">是否保持回调</param>
            /// <returns>保持回调</returns>
            public keepCallback Get<inputParameterType, outputParameterType>
                (Action<asynchronousMethod.returnValue<outputParameterType>> onGet, ref inputParameterType inputParameter, int commandIdentity
                , int maxLength, outputParameterType outputParameter, bool isTask, bool isKeepCallback)
            {
                if (isDisposed == 0)
                {
                    asyncInputOutputIdentityCommand<inputParameterType, outputParameterType> command = asyncInputOutputIdentityCommand<inputParameterType, outputParameterType>.Get();
                    if (command != null && command.Set(this, onGet, ref inputParameter, commandIdentity, maxLength, outputParameter, isTask, isKeepCallback))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                }
                onGet(new asynchronousMethod.returnValue<outputParameterType> { IsReturn = false });
                return null;
            }
            /// <summary>
            /// 客户端命令
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <typeparam name="outputParameterType">输出参数类型</typeparam>
            private sealed class asyncInputOutputJsonIdentityCommand<inputParameterType, outputParameterType> : asyncInputIdentityCommand<tcpBase.parameterJsonToSerialize<inputParameterType>, asynchronousMethod.returnValue<outputParameterType>, asyncInputOutputJsonIdentityCommand<inputParameterType, outputParameterType>>
            {
                /// <summary>
                /// 输出参数
                /// </summary>
                public tcpBase.parameterJsonToSerialize<outputParameterType> OutputParameter;
                /// <summary>
                /// 输出参数访问锁
                /// </summary>
                private int outputLock;
                /// <summary>
                /// 接收数据回调处理
                /// </summary>
                /// <param name="data">输出数据</param>
                protected override void onReceive(ref memoryPool.pushSubArray data)
                {
                    bool isReturn = false;
                    byte[] buffer = data.Value.UnsafeArray;
                    if (KeepCallback == null)
                    {
                        try
                        {
                            if (buffer != null && fastCSharp.emit.dataDeSerializer.DeSerialize(ref data.Value, ref OutputParameter)) isReturn = true;
                        }
                        finally
                        {
                            outputParameterType outputParameter = OutputParameter.Return;
                            OutputParameter.Return = default(outputParameterType);
                            push(new fastCSharp.code.cSharp.asynchronousMethod.returnValue<outputParameterType> { IsReturn = isReturn, Value = outputParameter });
                            data.Push();
                        }
                    }
                    else if (buffer == null)
                    {
                        onlyCallback(new fastCSharp.code.cSharp.asynchronousMethod.returnValue<outputParameterType> { IsReturn = false });
                    }
                    else
                    {
                        interlocked.CompareSetYieldSleep0(ref outputLock);
                        tcpBase.parameterJsonToSerialize<outputParameterType> outputParameter = OutputParameter;
                        try
                        {
                            if (fastCSharp.emit.dataDeSerializer.DeSerialize(ref data.Value, ref outputParameter)) isReturn = true;
                        }
                        finally
                        {
                            outputLock = 0;
                            onlyCallback(new fastCSharp.code.cSharp.asynchronousMethod.returnValue<outputParameterType> { IsReturn = isReturn, Value = outputParameter.Return });
                            data.Push();
                        }
                    }
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="onCall">回调委托,返回false表示失败</param>
                /// <param name="commandIdentity">TCP调用命令</param>
                /// <param name="inputParameter">输入参数</param>
                /// <param name="maxLength">输入参数数据最大长度</param>
                /// <param name="isTask">回调是否使用任务池</param>
                /// <param name="isKeepCallback">是否保持回调</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, Action<asynchronousMethod.returnValue<outputParameterType>> onGet
                    , ref inputParameterType inputParameter, int commandIdentity, int maxLength, outputParameterType outputParameter
                    , bool isTask, bool isKeepCallback)
                {
                    Socket = socket;
                    Callback = onGet;
                    Command = commandIdentity;
                    InputParameter = new tcpBase.parameterJsonToSerialize<inputParameterType> { Return = inputParameter };
                    MaxInputSize = maxLength;
                    OutputParameter = new tcpBase.parameterJsonToSerialize<outputParameterType> { Return = outputParameter };
                    Identity = socket.newIndex(OnReceive, isTask ? (byte)1 : (byte)0, isKeepCallback ? (byte)1 : (byte)0);
                    if (isKeepCallback)
                    {
                        if (SetKeepCallback()) return true;
                        InputParameter.Return = default(inputParameterType);
                        OutputParameter.Return = default(outputParameterType);
                        Callback = null;
                        push();
                        return false;
                    }
                    return true;
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <returns>客户端命令</returns>
                public static asyncInputOutputJsonIdentityCommand<inputParameterType, outputParameterType> Get()
                {
                    asyncInputOutputJsonIdentityCommand<inputParameterType, outputParameterType> command = typePool<asyncInputOutputJsonIdentityCommand<inputParameterType, outputParameterType>>.Pop();
                    if (command == null)
                    {
                        try
                        {
                            command = new asyncInputOutputJsonIdentityCommand<inputParameterType, outputParameterType>();
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
            /// <param name="commandIdentity">TCP调用命令</param>
            /// <param name="inputParameter">输入参数</param>
            /// <param name="maxLength">输入参数数据最大长度</param>
            /// <param name="outputParameter">输出参数</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <param name="isKeepCallback">是否保持回调</param>
            /// <returns>保持回调</returns>
            public keepCallback GetJson<inputParameterType, outputParameterType>
                (Action<asynchronousMethod.returnValue<outputParameterType>> onGet
                , ref inputParameterType inputParameter, int commandIdentity, int maxLength, outputParameterType outputParameter, bool isTask, bool isKeepCallback)
            {
                if (isDisposed == 0)
                {
                    asyncInputOutputJsonIdentityCommand<inputParameterType, outputParameterType> command = asyncInputOutputJsonIdentityCommand<inputParameterType, outputParameterType>.Get();
                    if (command != null && command.Set(this, onGet, ref inputParameter, commandIdentity, maxLength, outputParameter, isTask, isKeepCallback))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                }
                onGet(new asynchronousMethod.returnValue<outputParameterType> { IsReturn = false });
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
                /// 输入参数数据最大长度
                /// </summary>
                public int MaxInputSize;
                /// <summary>
                /// 创建第一个命令输入数据
                /// </summary>
                /// <param name="stream">命令内存流</param>
                /// <returns>数据起始位置,失败返回0</returns>
                public unsafe override int BuildIndex(unmanagedStream stream)
                {
                    int streamLength = stream.Length, commandLength = Command.Length + sizeof(commandServer.streamIdentity) + sizeof(int);
                    stream.PrepLength(commandLength);
                    stream.UnsafeAddLength(commandLength);
                    int serializeIndex = stream.Length;
                    fastCSharp.emit.dataSerializer.Serialize(InputParameter, stream);
                    int dataLength = stream.Length - serializeIndex;
                    InputParameter = default(inputParameterType);
                    if (dataLength <= MaxInputSize)
                    {
                        byte* write = stream.Data + streamLength;
                        unsafer.memory.Copy(Command, write, Command.Length);
                        *(commandServer.streamIdentity*)(write += Command.Length) = Identity;
                        *(int*)(write + sizeof(commandServer.streamIdentity)) = dataLength;
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
            private sealed class asyncInputDataCommand<inputParameterType> : asyncInputDataCommand<inputParameterType, bool, asyncInputDataCommand<inputParameterType>>
            {
                /// <summary>
                /// 接收数据回调处理
                /// </summary>
                /// <param name="data">输出数据</param>
                protected override void onReceive(ref memoryPool.pushSubArray data)
                {
                    if (data.Value.UnsafeArray == null)
                    {
                        if (KeepCallback == null) push(false);
                        else onlyCallback(false);
                    }
                    else
                    {
                        if (KeepCallback == null) push(true);
                        else onlyCallback(true);
                    }
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="onCall">回调委托,返回false表示失败</param>
                /// <param name="commandData">TCP调用命令</param>
                /// <param name="inputParameter">输入参数</param>
                /// <param name="maxLength">输入参数数据最大长度</param>
                /// <param name="isTask">回调是否使用任务池</param>
                /// <param name="isKeepCallback">是否保持回调</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, Action<bool> onCall
                    , byte[] commandData, ref inputParameterType inputParameter, int maxLength, bool isTask, bool isKeepCallback)
                {
                    Socket = socket;
                    Callback = onCall;
                    Command = commandData;
                    InputParameter = inputParameter;
                    MaxInputSize = maxLength;
                    Identity = socket.newIndex(OnReceive, isTask ? (byte)1 : (byte)0, isKeepCallback ? (byte)1 : (byte)0);
                    if (isKeepCallback)
                    {
                        if (SetKeepCallback()) return true;
                        InputParameter = default(inputParameterType);
                        Callback = null;
                        push();
                        return false;
                    }
                    return true;
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
            /// <param name="onCall">回调委托,返回false表示失败</param>
            /// <param name="commandData">TCP调用命令</param>
            /// <param name="inputParameter">输入参数</param>
            /// <param name="maxLength">输入参数数据最大长度</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <param name="isKeepCallback">是否保持回调</param>
            /// <returns>保持回调</returns>
            public keepCallback Call<inputParameterType>
                (Action<bool> onCall, byte[] commandData, ref inputParameterType inputParameter, int maxLength, bool isTask, bool isKeepCallback)
            {
                if (isDisposed == 0)
                {
                    asyncInputDataCommand<inputParameterType> command = asyncInputDataCommand<inputParameterType>.Get();
                    if (command != null && command.Set(this, onCall, commandData, ref inputParameter, maxLength, isTask, isKeepCallback))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                }
                onCall(false);
                return null;
            }
            /// <summary>
            /// TCP调用
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <param name="onCall">回调委托,返回false表示失败</param>
            /// <param name="commandData">TCP调用命令</param>
            /// <param name="inputParameter">输入参数</param>
            /// <param name="maxLength">输入参数数据最大长度</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <param name="isKeepCallback">是否保持回调</param>
            /// <returns>保持回调</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public keepCallback CallJson<inputParameterType>
                (Action<bool> onCall, byte[] commandData, ref inputParameterType inputParameter, int maxLength, bool isTask, bool isKeepCallback)
            {
                tcpBase.parameterJsonToSerialize<inputParameterType> input = new tcpBase.parameterJsonToSerialize<inputParameterType> { Return = inputParameter };
                return Call(onCall, commandData, ref input, maxLength, isTask, isKeepCallback);
            }
            /// <summary>
            /// 客户端命令
            /// </summary>
            private sealed class asyncDataCommand : asyncDataCommand<asyncDataCommand, bool>
            {
                /// <summary>
                /// 接收数据回调处理
                /// </summary>
                /// <param name="data">输出数据</param>
                protected override void onReceive(ref memoryPool.pushSubArray data)
                {
                    if (data.Value.UnsafeArray == null)
                    {
                        if (KeepCallback == null) push(false);
                        else onlyCallback(false);
                    }
                    else
                    {
                        if (KeepCallback == null) push(true);
                        else onlyCallback(true);
                    }
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="onCall">回调委托,返回false表示失败</param>
                /// <param name="commandData">TCP调用命令</param>
                /// <param name="isTask">回调是否使用任务池</param>
                /// <param name="isKeepCallback">是否保持回调</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, Action<bool> onCall, byte[] commandData, bool isTask, bool isKeepCallback)
                {
                    Socket = socket;
                    Callback = onCall;
                    Command = commandData;
                    Identity = socket.newIndex(OnReceive, isTask ? (byte)1 : (byte)0, isKeepCallback ? (byte)1 : (byte)0);
                    if (isKeepCallback)
                    {
                        if (SetKeepCallback()) return true;
                        Callback = null;
                        push();
                        return false;
                    }
                    return true;
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
            /// <param name="onCall">回调委托,返回false表示失败</param>
            /// <param name="commandData">TCP调用命令</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <param name="isKeepCallback">是否保持回调</param>
            /// <returns>保持回调</returns>
            public keepCallback Call(Action<bool> onCall, byte[] commandData, bool isTask, bool isKeepCallback)
            {
                if (isDisposed == 0)
                {
                    asyncDataCommand command = asyncDataCommand.Get();
                    if (command != null && command.Set(this, onCall, commandData, isTask, isKeepCallback))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                }
                onCall(false);
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
                /// 输入参数数据最大长度
                /// </summary>
                public int MaxInputSize;
                /// <summary>
                /// 创建第一个命令输入数据
                /// </summary>
                /// <param name="stream">命令内存流</param>
                /// <returns>数据起始位置,失败返回0</returns>
                public unsafe override int BuildIndex(unmanagedStream stream)
                {
                    int streamLength = stream.Length;
                    stream.PrepLength(sizeof(int) * 2 + sizeof(commandServer.streamIdentity));
                    stream.UnsafeAddLength(sizeof(int) * 2 + sizeof(commandServer.streamIdentity));
                    fastCSharp.emit.dataSerializer.Serialize(InputParameter, stream);
                    int dataLength = stream.Length - streamLength - (sizeof(int) * 2 + sizeof(commandServer.streamIdentity));
                    InputParameter = default(inputParameterType);
                    if (dataLength <= MaxInputSize)
                    {
                        byte* write = stream.Data + streamLength;
                        *(int*)(write) = Command;
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
            private sealed class asyncInputIdentityCommand<inputParameterType> : asyncInputIdentityCommand<inputParameterType, bool, asyncInputIdentityCommand<inputParameterType>>
            {
                /// <summary>
                /// 接收数据回调处理
                /// </summary>
                /// <param name="data">输出数据</param>
                protected override void onReceive(ref memoryPool.pushSubArray data)
                {
                    if (data.Value.UnsafeArray == null)
                    {
                        if (KeepCallback == null) push(false);
                        else onlyCallback(false);
                    }
                    else
                    {
                        if (KeepCallback == null) push(true);
                        else onlyCallback(true);
                    }
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="onCall">回调委托,返回false表示失败</param>
                /// <param name="inputParameter">输入参数</param>
                /// <param name="commandIdentity">TCP调用命令</param>
                /// <param name="maxLength">输入参数数据最大长度</param>
                /// <param name="isTask">回调是否使用任务池</param>
                /// <param name="isKeepCallback">是否保持回调</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, Action<bool> onCall
                    , ref inputParameterType inputParameter, int commandIdentity, int maxLength, bool isTask, bool isKeepCallback)
                {
                    Socket = socket;
                    Callback = onCall;
                    Command = commandIdentity;
                    InputParameter = inputParameter;
                    MaxInputSize = maxLength;
                    Identity = socket.newIndex(OnReceive, isTask ? (byte)1 : (byte)0, isKeepCallback ? (byte)1 : (byte)0);
                    if (isKeepCallback)
                    {
                        if (SetKeepCallback()) return true;
                        InputParameter = default(inputParameterType);
                        Callback = null;
                        push();
                        return false;
                    }
                    return true;
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
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
            /// <param name="onCall">回调委托,返回false表示失败</param>
            /// <param name="inputParameter">输入参数</param>
            /// <param name="commandIdentity">TCP调用命令</param>
            /// <param name="maxLength">输入参数数据最大长度</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <param name="isKeepCallback">是否保持回调</param>
            /// <returns>保持回调</returns>
            public keepCallback Call<inputParameterType>
                (Action<bool> onCall, ref inputParameterType inputParameter, int commandIdentity, int maxLength, bool isTask, bool isKeepCallback)
            {
                if (isDisposed == 0)
                {
                    asyncInputIdentityCommand<inputParameterType> command = asyncInputIdentityCommand<inputParameterType>.Get();
                    if (command != null && command.Set(this, onCall, ref inputParameter, commandIdentity, maxLength, isTask, isKeepCallback))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                }
                onCall(false);
                return null;
            }
            /// <summary>
            /// TCP调用
            /// </summary>
            /// <typeparam name="inputParameterType">输入参数类型</typeparam>
            /// <param name="onCall">回调委托,返回false表示失败</param>
            /// <param name="commandIdentity">TCP调用命令</param>
            /// <param name="inputParameter">输入参数</param>
            /// <param name="maxLength">输入参数数据最大长度</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <param name="isKeepCallback">是否保持回调</param>
            /// <returns>保持回调</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public keepCallback CallJson<inputParameterType>
                (Action<bool> onCall, ref inputParameterType inputParameter, int commandIdentity, int maxLength, bool isTask, bool isKeepCallback)
            {
                tcpBase.parameterJsonToSerialize<inputParameterType> input = new tcpBase.parameterJsonToSerialize<inputParameterType> { Return = inputParameter };
                return Call(onCall, ref input, commandIdentity, maxLength, isTask, isKeepCallback);
            }
            /// <summary>
            /// 客户端命令
            /// </summary>
            private sealed class asyncIdentityCommand : asyncIdentityCommand<asyncIdentityCommand, bool>
            {
                /// <summary>
                /// 接收数据回调处理
                /// </summary>
                /// <param name="data">输出数据</param>
                protected override void onReceive(ref memoryPool.pushSubArray data)
                {
                    if (data.Value.UnsafeArray == null)
                    {
                        if (KeepCallback == null) push(false);
                        else onlyCallback(false);
                    }
                    else
                    {
                        if (KeepCallback == null) push(true);
                        else onlyCallback(true);
                    }
                }
                /// <summary>
                /// 获取客户端命令
                /// </summary>
                /// <param name="socket">TCP客户端命令流处理套接字</param>
                /// <param name="onCall">回调委托,返回false表示失败</param>
                /// <param name="commandIdentity">TCP调用命令</param>
                /// <param name="isTask">回调是否使用任务池</param>
                /// <param name="isKeepCallback">是否保持回调</param>
                /// <returns>客户端命令</returns>
                public bool Set(streamCommandSocket socket, Action<bool> onCall, int commandIdentity, bool isTask, bool isKeepCallback)
                {
                    Socket = socket;
                    Callback = onCall;
                    Command = commandIdentity;
                    Identity = socket.newIndex(OnReceive, isTask ? (byte)1 : (byte)0, isKeepCallback ? (byte)1 : (byte)0);
                    if (isKeepCallback)
                    {
                        if (SetKeepCallback()) return true;
                        Callback = null;
                        push();
                        return false;
                    }
                    return true;
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
            /// <param name="onCall">回调委托,返回false表示失败</param>
            /// <param name="commandIdentity">TCP调用命令</param>
            /// <param name="isTask">回调是否使用任务池</param>
            /// <param name="isKeepCallback">是否保持回调</param>
            /// <returns>保持回调</returns>
            public keepCallback Call(Action<bool> onCall, int commandIdentity, bool isTask, bool isKeepCallback)
            {
                if (isDisposed == 0)
                {
                    asyncIdentityCommand command = asyncIdentityCommand.Get();
                    if (command != null && command.Set(this, onCall, commandIdentity, isTask, isKeepCallback))
                    {
                        pushCommand(command);
                        return command.KeepCallback;
                    }
                }
                onCall(false);
                return null;
            }

            /// <summary>
            /// 取消保持回调
            /// </summary>
            /// <param name="index">命令集合索引</param>
            /// <param name="identity">会话标识</param>
            private void cancel(int index, int identity)
            {
                interlocked.CompareSetYieldSleep0(ref commandIndexLock);
                if ((uint)index < commandIndexs.Length && commandIndexs[index].Cancel(identity)) freeIndexLock(index);
                else commandIndexLock = 0;
            }
            /// <summary>
            /// 接收数据回调
            /// </summary>
            /// <param name="identity">会话标识</param>
            /// <param name="data">输出数据</param>
            /// <param name="isTask">检测回调是否使用任务池</param>
            /// <param name="isTaskCopyData">回调任务池是否复制数据</param>
            private void onReceive(commandServer.streamIdentity identity, ref memoryPool.pushSubArray data, byte checkTask, bool isTaskCopyData)
            {
                pushPool<memoryPool.pushSubArray> onReceive = null;
                byte isTask = 0;
                interlocked.CompareSetYieldSleep0(ref commandIndexLock);
                if ((uint)identity.Index < commandIndexs.Length)
                {
                    if (commandIndexs[identity.Index].Get(identity.Identity, ref onReceive, ref isTask)) freeIndexLock(identity.Index);
                    else commandIndexLock = 0;
                }
                else
                {
                    commandIndexLock = 0;
                    log.Error.Add(attribute.ServiceName + " " + commandIndexs.Length.toString() + "[" + identity.Index.toString() + "," + identity.Identity.toString() + "] Data[" + data.Value.StartIndex.toString() + "," + data.Value.Count.toString() + "]", false, false);
                    log.Error.Add(data.Value.UnsafeArray == receiveData ? "[" + receiveEndIndex.toString() + "]" + receiveData.sub(0, receiveEndIndex).ToArray().joinString(',') : null, true, false);
                }
                if (onReceive != null)
                {
                    if (isTask == 0 || checkTask == 0)
                    {
                        try
                        {
                            onReceive(ref data);
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
                        byte[] buffer = asyncBuffers.Get(dataArray.Count);
                        pushData.PushPool = asyncBuffers.PushHandle;
                        Buffer.BlockCopy(dataArray.UnsafeArray, dataArray.StartIndex, buffer, 0, dataArray.Count);
                        pushData.Value.UnsafeSet(buffer, 0, dataArray.Count);
                        fastCSharp.threading.task.Tiny.Add(onReceive, ref pushData, null);
                    }
                    else fastCSharp.threading.task.Tiny.Add(onReceive, ref data, null);
                }
            }
            /// <summary>
            /// 接收服务器端数据
            /// </summary>
            private Action receiveHandle;
            /// <summary>
            /// 接收服务器端数据
            /// </summary>
            internal void Receive()
            {
                if (attribute.IsClientAsynchronousReceive) receiveAsynchronous();
                else
                {
                    if (receiveHandle == null) receiveHandle = receive;
                    threadPool.TinyPool.FastStart(receiveHandle, null, null);
                }
            }
            /// <summary>
            /// 同步接收服务器端数据
            /// </summary>
            private void receive()
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
                            if (receiveLength < sizeof(commandServer.streamIdentity) + sizeof(int))
                            {
                                if (receiveLength != 0) unsafer.memory.Copy(dataFixed + index, dataFixed, receiveLength);
                                receiveEndIndex = tryReceive(receiveLength, sizeof(commandServer.streamIdentity) + sizeof(int), DateTime.MaxValue);
                                if (receiveEndIndex < sizeof(commandServer.streamIdentity) + sizeof(int))
                                {
                                    //log.Error.Add("receiveEndIndex " + receiveEndIndex.toString(), false, false);
                                    break;
                                }
                                receiveLength = receiveEndIndex;
                                index = 0;
                            }
                            byte* start = dataFixed + index;
                            commandServer.streamIdentity identity = *(commandServer.streamIdentity*)start;
                            if (identity.Identity < 0)
                            {
                                //log.Error.Add("identity.Identity " + identity.Identity.toString(), false, false);
                                break;
                            }
                            int length = *(int*)(start + sizeof(commandServer.streamIdentity));
                            index += sizeof(commandServer.streamIdentity) + sizeof(int);
                            if (length == 0)
                            {
                                pushData.Value.UnsafeSet(receiveData, 0, 0);
                                onReceive(identity, ref pushData, 1, false);
                                continue;
                            }
                            if (length == commandServer.ErrorStreamReturnLength)
                            {
                                pushData.Value.Null();
                                onReceive(identity, ref pushData, 1, false);
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
                                        PushPool = fastCSharp.memoryPool.StreamBuffers.PushHandle 
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
                                        PushPool = fastCSharp.memoryPool.StreamBuffers.PushHandle
                                    };
                                    onReceive(identity, ref newPushData, 1, false);
                                }
                                BigBuffers.Push(ref buffer);
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
            /// 接收会话标识
            /// </summary>
            private Action<int> receiveIdentityHandle;
            /// <summary>
            /// 接收服务器端数据
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private void receiveAsynchronous()
            {
                tryReceive(onReceiveIdentityHandle, 0, sizeof(commandServer.streamIdentity) + sizeof(int), DateTime.MaxValue);
            }
            /// <summary>
            /// 接收会话标识
            /// </summary>
            /// <param name="receiveEndIndex">接收数据结束位置</param>
            private unsafe void onReceiveIdentity(int receiveEndIndex)
            {
                //if (isOutputDebug) commandServer.DebugLog.Add(attribute.ServiceName + ".onReceiveIdentity(" + receiveEndIndex.toString() + ")", false, false);
                if (receiveEndIndex >= sizeof(commandServer.streamIdentity) + sizeof(int))
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
                if (identity.Identity >= 0)
                {
                    int length = *(int*)(receiveDataFixed + sizeof(commandServer.streamIdentity));
                    if (length == 0)
                    {
                        memoryPool.pushSubArray pushData = default(memoryPool.pushSubArray);
                        receiveIdentity(sizeof(commandServer.streamIdentity) + sizeof(int));
                        pushData.Value.UnsafeSet(receiveData, 0, 0);
                        onReceive(identity, ref pushData, 0, false);
                    }
                    else if (length == commandServer.ErrorStreamReturnLength)
                    {
                        memoryPool.pushSubArray pushData = default(memoryPool.pushSubArray);
                        receiveIdentity(sizeof(commandServer.streamIdentity) + sizeof(int));
                        onReceive(identity, ref pushData, 0, false);
                    }
                    else
                    {
                        int index = onReceiveIdentity(0, length);
                        if (index != 0) receiveIdentity(index);
                    }
                    return;
                }
                Dispose();
            }
            /// <summary>
            /// 接收会话标识
            /// </summary>
            /// <param name="index">起始位置</param>
            private void receiveNextIdentity(int index)
            {
                receiveDepth = 512;
                fixed (byte* dataFixed = receiveData)
                {
                    receiveDataFixed = dataFixed;
                    receiveIdentity(index);
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
                    if (receiveIdentityHandle == null) receiveIdentityHandle = receiveNextIdentity;
                    threadPool.Default.FastStart(receiveIdentityHandle, index, null, null);
                    return;
                }
                NEXT:
                int receiveLength = receiveEndIndex - index;
                if (receiveLength >= sizeof(commandServer.streamIdentity) + sizeof(int))
                {
                    byte* start = receiveDataFixed + index;
                    commandServer.streamIdentity identity = *(commandServer.streamIdentity*)start;
                    if (identity.Identity >= 0)
                    {
                        int length = *(int*)(start + sizeof(commandServer.streamIdentity));
                        if (length == 0)
                        {
                            memoryPool.pushSubArray pushData = default(memoryPool.pushSubArray);
                            receiveIdentity(index + sizeof(commandServer.streamIdentity) + sizeof(int));
                            pushData.Value.UnsafeSet(receiveData, 0, 0);
                            onReceive(identity, ref pushData, 0, false);
                        }
                        else if (length == commandServer.ErrorStreamReturnLength)
                        {
                            memoryPool.pushSubArray pushData = default(memoryPool.pushSubArray);
                            receiveIdentity(index + sizeof(commandServer.streamIdentity) + sizeof(int));
                            onReceive(identity, ref pushData, 0, false);
                        }
                        else if ((index = onReceiveIdentity(index, length)) != 0) goto NEXT;
                    }
                    else Dispose();
                }
                else
                {
                    if (receiveLength != 0) unsafer.memory.Copy(receiveDataFixed + index, receiveDataFixed, receiveLength);
                    tryReceive(onReceiveIdentityHandle, receiveLength, sizeof(commandServer.streamIdentity) + sizeof(int), DateTime.MaxValue);
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
                if (identity.Identity >= 0)
                {
                    int dataLength = length >= 0 ? length : -length, receiveLength = receiveEndIndex - (index += (sizeof(commandServer.streamIdentity) + sizeof(int)));
                    if (dataLength <= receiveLength)
                    {
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
                                PushPool = fastCSharp.memoryPool.StreamBuffers.PushHandle
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
                            if (receiveNoCompressHandle == null) receiveNoCompressHandle = receiveNoCompress;
                            receive(receiveNoCompressHandle, receiveLength, dataLength - receiveLength, DateTime.MaxValue);
                        }
                        else
                        {
                            if (receiveCompressHandle == null) receiveCompressHandle = receiveCompress;
                            receive(receiveCompressHandle, receiveLength, dataLength - receiveLength, DateTime.MaxValue);
                        }
                    }
                }
                else Dispose();
                return 0;
            }
            /// <summary>
            /// 获取非压缩数据
            /// </summary>
            private Action<bool> receiveNoCompressHandle;
            /// <summary>
            /// 获取压缩数据
            /// </summary>
            private Action<bool> receiveCompressHandle;
            /// <summary>
            /// 获取非压缩数据
            /// </summary>
            /// <param name="isSocket">是否成功</param>
            private void receiveNoCompress(bool isSocket)
            {
                //if (isOutputDebug) commandServer.DebugLog.Add(attribute.ServiceName + ".receiveNoCompress(" + isSocket.ToString() + ")", false, false);
                if (isSocket)
                {
                    memoryPool.pushSubArray pushData = default(memoryPool.pushSubArray);
                    commandServer.streamIdentity identity = currentIdentity;
                    pushData.Value.UnsafeSet(currentReceiveData, 0, currentReceiveEndIndex);
                    pushData.PushPool = BigBuffers.PushHandle;
                    currentReceiveData = receiveData;
                    receiveAsynchronous();
                    onReceive(identity, ref pushData, 0, false);
                }
                else Dispose();
            }
            /// <summary>
            /// 获取压缩数据
            /// </summary>
            /// <param name="isSocket">是否成功</param>
            private void receiveCompress(bool isSocket)
            {
                //if (isOutputDebug) commandServer.DebugLog.Add(attribute.ServiceName + ".receiveCompress(" + isSocket.ToString() + ")", false, false);
                if (isSocket)
                {
                    memoryPool.pushSubArray pushData = new memoryPool.pushSubArray
                    {
                        Value = fastCSharp.io.compression.stream.Deflate.GetDeCompressUnsafe(currentReceiveData, 0, currentReceiveEndIndex, fastCSharp.memoryPool.StreamBuffers),
                        PushPool = fastCSharp.memoryPool.StreamBuffers.PushHandle
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
                closeIdentityCommandData = new byte[(sizeof(int) * 2 + sizeof(commandServer.streamIdentity))];
                fixed (byte* commandFixed = closeIdentityCommandData)
                {
                    *(int*)(commandFixed) = commandServer.CloseIdentityCommand;
                    *(commandServer.streamIdentity*)(commandFixed + sizeof(int)) = new commandServer.streamIdentity { Index = 0, Identity = int.MinValue };
                    *(int*)(commandFixed + sizeof(int) + sizeof(commandServer.streamIdentity)) = 0;
                }
                closeCommandData = new byte[(sizeof(int) * 3 + sizeof(commandServer.streamIdentity))];
                fixed (byte* commandFixed = closeCommandData)
                {
                    *(int*)(commandFixed) = sizeof(int) * 3 + sizeof(commandServer.streamIdentity);
                    *(int*)(commandFixed + sizeof(int)) = commandServer.CloseIdentityCommand + commandServer.CommandDataIndex;
                    *(commandServer.streamIdentity*)(commandFixed + sizeof(int) * 2) = new commandServer.streamIdentity { Index = 0, Identity = int.MinValue };
                    *(int*)(commandFixed + sizeof(int) * 2 + sizeof(commandServer.streamIdentity)) = 0;
                }

                checkCommandData = new byte[sizeof(int) + sizeof(int)];
                fixed (byte* commandFixed = checkCommandData)
                {
                    *(int*)commandFixed = sizeof(int) * 3 + sizeof(commandServer.streamIdentity);
                    *(int*)(commandFixed + sizeof(int)) = commandServer.CheckIdentityCommand + commandServer.CommandDataIndex;
                }
                loadBalancingCheckCommandData = new byte[sizeof(int) + sizeof(int)];
                fixed (byte* commandFixed = loadBalancingCheckCommandData)
                {
                    *(int*)commandFixed = sizeof(int) * 3 + sizeof(commandServer.streamIdentity);
                    *(int*)(commandFixed + sizeof(int)) = commandServer.LoadBalancingCheckIdentityCommand + commandServer.CommandDataIndex;
                }
                tcpStreamCommandData = new byte[sizeof(int) + sizeof(int)];
                fixed (byte* commandFixed = tcpStreamCommandData)
                {
                    *(int*)commandFixed = sizeof(int) * 3 + sizeof(commandServer.streamIdentity);
                    *(int*)(commandFixed + sizeof(int)) = commandServer.TcpStreamCommand + commandServer.CommandDataIndex;
                }
                ignoreGroupCommandData = new byte[sizeof(int) + sizeof(int)];
                fixed (byte* commandFixed = ignoreGroupCommandData)
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
        /// 当前验证函数调用客户端套接字类型访问锁
        /// </summary>
        protected int verifyMethodLock;
        /// <summary>
        /// TCP客户端流命令处理客户端验证函数调用访问锁
        /// </summary>
        protected int streamVerifyMethodLock;
        /// <summary>
        /// TCP客户端命令流处理套接字访问锁
        /// </summary>
        private int streamSocketLock;
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
                if (TcpRegisterServices.Version != TcpRegisterServicesVersion)
                {
                    streamCommandSocket socket = this.streamSocket;
                    if (TcpRegisterClient.GetHost(this) && socket != null)
                    {
                        interlocked.CompareSetSleep1(ref streamSocketLock);
                        if (this.streamSocket == socket) this.streamSocket = null;
                        streamSocketLock = 0;
                        log.Default.Add("TCP服务更新，关闭客户端 " + Attribute.ServiceName, false, false);
                        pub.Dispose(ref socket);
                    }
                }
                streamCommandSocket streamSocket = this.streamSocket;
                if ((streamSocket == null || streamSocket.IsDisposed) && Attribute.Port != 0)
                {
                    bool isCreate = false;
                    interlocked.CompareSetSleep1(ref streamSocketLock);
                    try
                    {
                        if (isDisposed == 0)
                        {
                            if (this.streamSocket == null || this.streamSocket.IsDisposed)
                            {
                                this.streamSocket = streamCommandSocket.Create(this);
                                if (this.streamSocket != null)
                                {
                                    if (isDisposed == 0)
                                    {
                                        isCreate = true;
                                        while (streamVerifyMethodLock != 0) Thread.Sleep(0);
                                        streamVerifyMethodLock = 1;
                                    }
                                    else this.streamSocket.Dispose();
                                }
                            }
                            streamSocket = this.streamSocket;
                        }
                    }
                    finally
                    {
                        streamSocketLock = 0;
                    }
                    if (isCreate)
                    {
                        try
                        {
                            streamSocket.Receive();
                            if (callVerifyMethod()) streamSocket.SetCheck();
                        }
                        finally { streamVerifyMethodLock = 0; }
                    }
                }
                if (streamSocket != null)
                {
                    while (streamVerifyMethodLock != 0) Thread.Sleep(1);
                    if (streamSocket.IsVerifyMethod) return streamSocket;
                }
                return null;
            }
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
            get { return streamSocket ?? verifyStreamSocket; }
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
        /// 获取TCP客户端命令流处理套接字委托集合
        /// </summary>
        private subArray<Action<streamCommandSocket>> onStreamSockets;
        /// <summary>
        /// 是否正在创建TCP客户端命令流处理套接字
        /// </summary>
        private byte isCreateStreamSocket;
        /// <summary>
        /// 创建TCP客户端命令流处理套接字
        /// </summary>
        private sealed class streamSocketCreator : IDisposable
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
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Create()
            {
                if (!client.Create(ref socket, commandClient.Attribute, connectAsync)) onCreate(null);
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
                this.commandClient = null;
                this.socket = null;
                commandClient.onGetStreamSocket(socket);
                typePool<streamSocketCreator>.Push(this);
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
        public void GetStreamSocket(Action<streamCommandSocket> onStreamSocket)
        {
            streamCommandSocket streamSocket = this.streamSocket;
            if (streamSocket == null || streamSocket.IsDisposed)
            {
                byte isAdd = 0;
                interlocked.NoCheckCompareSetSleep0(ref streamSocketLock);
                if (isDisposed == 0)
                {
                    if (this.streamSocket == null || this.streamSocket.IsDisposed)
                    {
                        byte isCreateStreamSocket = this.isCreateStreamSocket;
                        try
                        {
                            onStreamSockets.Add(onStreamSocket);
                            this.isCreateStreamSocket = isAdd = 1;
                        }
                        finally
                        {
                            streamSocketLock = 0;
                            if (isAdd == 0) onStreamSocket(null);
                        }
                        if (isCreateStreamSocket == 0)
                        {
                            streamSocketCreator streamSocketCreator = streamSocketCreator.Get(this);
                            if (streamSocketCreator == null) onGetStreamSocket(null);
                            else streamSocketCreator.Create();
                        }
                    }
                    else
                    {
                        streamSocket = this.streamSocket;
                        streamSocketLock = 0;
                        onStreamSocket(streamSocket);
                    }
                }
                else
                {
                    streamSocketLock = 0;
                    onStreamSocket(null);
                }
            }
            else onStreamSocket(streamSocket);
        }
        /// <summary>
        /// 创建TCP客户端命令流处理套接字
        /// </summary>
        /// <param name="socket"></param>
        private void onGetStreamSocket(streamCommandSocket streamSocket)
        {
            interlocked.NoCheckCompareSetSleep0(ref streamSocketLock);
            subArray<Action<streamCommandSocket>> onStreamSockets = this.onStreamSockets;
            this.streamSocket = streamSocket;
            isCreateStreamSocket = 0;
            this.onStreamSockets.Null();
            streamSocketLock = 0;
            foreach (Action<streamCommandSocket> onStreamSocket in onStreamSockets)
            {
                try
                {
                    onStreamSocket(streamSocket);
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
        /// <summary>
        /// TCP服务信息集合版本
        /// </summary>
        internal int TcpRegisterServicesVersion;
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
        /// <param name="isStart">是否启动客户端</param>
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
        /// <param name="isStart">是否启动客户端</param>
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
        /// <param name="isStart">是否启动客户端</param>
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
                log.Default.Add("关闭TCP客户端 " + Attribute.ServiceName, true, log.cacheType.Last);
                if (TcpRegisterClient != null)
                {
                    TcpRegisterClient.Remove(this);
                    TcpRegisterClient = null;
                }
                pub.Dispose(ref streamSocket);
                Stream[] streams = nullValue<Stream>.Array;
                interlocked.NoCheckCompareSetSleep0(ref tcpStreamLock);
                try
                {
                    streams = new Stream[tcpStreams.length()];
                    for (int index = streams.Length; index != 0; )
                    {
                        --index;
                        streams[index] = tcpStreams[index].Cancel();
                    }
                }
                finally { tcpStreamLock = 0; }
                foreach (Stream stream in streams) pub.Dispose(stream);
                onGetStreamSocket(null);
                verifyStreamSocket = null;
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
            interlocked.CompareSetSleep1(ref verifyMethodLock);
            try
            {
                if (verifyMethod.Verify()) return true;
            }
            catch (Exception error)
            {
                isError = true;
                log.Error.Add(error, "TCP客户端验证失败", false);
            }
            finally
            {
                verifyMethodLock = 0;
            }
            if (!isError) log.Error.Add("TCP客户端验证失败", true, false);
            Dispose();
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
        private int tcpStreamLock;
        /// <summary>
        /// TCP参数流集合访问锁
        /// </summary>
        private readonly object tcpStreamCreateLock = new object();
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
                    interlocked.NoCheckCompareSetSleep0(ref tcpStreamLock);
                    if (tcpStreams == null)
                    {
                        try
                        {
                            tcpStreams = new tcpStream[4];
                            tcpStream.ClientIndex = tcpStream.ClientIdentity = 0;
                            tcpStreams[0].Stream = stream;
                        }
                        finally { tcpStreamLock = 0; }
                    }
                    else
                    {
                        foreach (tcpStream value in tcpStreams)
                        {
                            if (value.Stream == null)
                            {
                                tcpStream.ClientIdentity = tcpStreams[tcpStream.ClientIndex].Set(stream);
                                tcpStreamLock = 0;
                                break;
                            }
                            ++tcpStream.ClientIndex;
                        }
                        if (tcpStream.ClientIndex == tcpStreams.Length)
                        {
                            tcpStreamLock = 0;
                            Monitor.Enter(tcpStreamCreateLock);
                            if (tcpStream.ClientIndex == tcpStreams.Length)
                            {
                                try
                                {
                                    tcpStream[] newTcpStreams = new tcpStream[tcpStream.ClientIndex << 1];
                                    interlocked.NoCheckCompareSetSleep0(ref tcpStreamLock);
                                    tcpStreams.CopyTo(newTcpStreams, 0);
                                    tcpStreams = newTcpStreams;
                                    tcpStream.ClientIdentity = tcpStreams[tcpStream.ClientIndex].Set(stream);
                                    tcpStreamLock = 0;
                                }
                                finally { Monitor.Exit(tcpStreamCreateLock); }
                            }
                            else
                            {
                                Monitor.Exit(tcpStreamCreateLock);
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
            interlocked.NoCheckCompareSetSleep0(ref tcpStreamLock);
            try
            {
                stream = tcpStreams[index].Get(identity);
            }
            finally { tcpStreamLock = 0; }
            return stream;
        }
        /// <summary>
        /// 关闭TCP参数流
        /// </summary>
        /// <param name="index">TCP参数流索引</param>
        /// <param name="identity">TCP参数流序号</param>
        private void closeTcpStream(int index, int identity)
        {
            interlocked.NoCheckCompareSetSleep0(ref tcpStreamLock);
            try
            {
                tcpStreams[index].Close(identity);
            }
            finally { tcpStreamLock = 0; }
        }

        /// <summary>
        /// 忽略TCP调用分组
        /// </summary>
        /// <param name="groupId">分组标识</param>
        /// <returns>是否调用成功</returns>
        public bool IgnoreGroup(int groupId)
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
            return false;
        }

        /// <summary>
        /// 负载均衡超时检测
        /// </summary>
        /// <returns>客户端是否可用</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool LoadBalancingCheck()
        {
            streamCommandSocket socket = StreamSocket;
            return socket != null && socket.LoadBalancingCheck();
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
            /// 创建TCP调用客户端委托
            /// </summary>
            protected Func<fastCSharp.code.cSharp.tcpServer, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<clientType>, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify, clientType> getClient;
            /// <summary>
            /// TCP服务调用配置
            /// </summary>
            protected fastCSharp.code.cSharp.tcpServer attribute;
            /// <summary>
            /// 验证函数接口
            /// </summary>
            protected fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<clientType> verifyMethod;
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
            /// <summary>
            /// TCP注册服务 客户端
            /// </summary>
            protected fastCSharp.net.tcp.tcpRegister.client tcpRegisterClient;
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
            protected int clientLock;
            /// <summary>
            /// 是否已经释放资源
            /// </summary>
            protected int isDisposed;
            /// <summary>
            /// TCP调用客户端路由
            /// </summary>
            /// <param name="getClient"></param>
            /// <param name="attribute"></param>
            /// <param name="verifyMethod"></param>
            /// <param name="verify"></param>
            public routerBase(Func<fastCSharp.code.cSharp.tcpServer, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<clientType>, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify, clientType> getClient
                , fastCSharp.code.cSharp.tcpServer attribute, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<clientType> verifyMethod, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify)
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
            protected sealed class clientCreator
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
                /// 创建TCP调用客户端
                /// </summary>
                /// <param name="router"></param>
                /// <param name="host"></param>
                public clientCreator(router<customType> router, host host)
                {
                    this.router = router;
                    this.host = host;
                }
                /// <summary>
                /// 创建TCP调用客户端
                /// </summary>
                public void Create()
                {
                    fastCSharp.code.cSharp.tcpServer attribute = fastCSharp.emit.memberCopyer<fastCSharp.code.cSharp.tcpServer>.MemberwiseClone(router.attribute);
                    attribute.TcpRegister = null;
                    attribute.Host = host.Host;
                    attribute.Port = host.Port;
                    (client = router.getClient(attribute, router.verifyMethod, router.verify)).TcpCommandClient.GetStreamSocket(onCreated);
                }
                /// <summary>
                /// 创建TCP调用客户端
                /// </summary>
                /// <param name="socket"></param>
                private void onCreated(streamCommandSocket socket)
                {
                    if (socket == null) pub.Dispose(ref client);
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
            public router(Func<fastCSharp.code.cSharp.tcpServer, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<clientType>, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify, clientType> getClient
                , fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<clientType> verifyMethod = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify = null)
                :base(getClient, attribute, verifyMethod, verify)
            {
                clients = new clientHost[4];
                tcpRegisterClient = commandClient.TcpRegisterClient;
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                interlocked.CompareSetSleep1(ref clientLock);
                if (isDisposed == 0)
                {
                    isDisposed = 1;
                    clientLock = 0;
                    if (tcpRegisterClient != null) tcpRegisterClient.OnServiceChanged -= onServiceChanged;
                    pub.Dispose(ref this.client);
                    commandClient = null;
                    interlocked.NoCheckCompareSetSleep0(ref clientLock);
                    int count = this.count;
                    this.count = hostCount = 0;
                    clientLock = 0;
                    while (count != 0) pub.Dispose(ref clients[--count].Client);
                    tcpRegisterClient = null;
                    verifyMethod = null;
                    verify = null;
                }
                else clientLock = 0;
            }
            /// <summary>
            /// 设置TCP服务信息更新事件
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected void start()
            {
                tcpRegisterClient.OnServiceChanged += onServiceChanged;
                onServiceChanged();
            }
            /// <summary>
            /// TCP服务信息更新事件
            /// </summary>
            private void onServiceChanged()
            {
                subArray<clientType> removeClients = default(subArray<clientType>);
                subArray<host> newHosts = default(subArray<host>);
                interlocked.CompareSetSleep1(ref clientLock);
                if (isDisposed == 0 && commandClient.TcpRegisterServices.Version != commandClient.TcpRegisterServicesVersion)
                {
                    commandClient.TcpRegisterServicesVersion = commandClient.TcpRegisterServices.Version;
                    host[] hosts = commandClient.TcpRegisterServices.Hosts;
                    try
                    {
                        for (int index = hostCount; index != 0; )
                        {
                            host clientHost = clients[--index].Host;
                            byte isHost = 0;
                            foreach (host host in hosts)
                            {
                                if (clientHost.Equals(host))
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
                                if (clients[--index].Host.Equals(host))
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
                        clientLock = 0;
                        foreach (clientType client in removeClients) pub.Dispose(client);
                        foreach (host host in newHosts) new clientCreator(this, host).Create();
                    }
                }
                else clientLock = 0;
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
                    interlocked.CompareSetSleep1(ref clientLock);
                    if (isDisposed == 0)
                    {
                        for (int index = hostCount; index != count; )
                        {
                            if (clients[--index].Host.Equals(host))
                            {
                                clients[index].Host = clients[--hostCount].Host;
                                clientLock = 0;
                                return;
                            }
                        }
                    }
                    clientLock = 0;
                }
                else
                {
                    interlocked.CompareSetSleep1(ref clientLock);
                    if (isDisposed == 0)
                    {
                        for (int index = hostCount; index != count; )
                        {
                            if (clients[--index].Host.Equals(host))
                            {
                                clients[index].Host = clients[count].Host;
                                clients[count++].Set(client, ref host);
                                clientLock = 0;
                                client = null;
                                return;
                            }
                        }
                    }
                    clientLock = 0;
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
                interlocked.CompareSetSleep1(ref clientLock);
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
                    finally { clientLock = 0; }
                }
                clientLock = 0;
                return nullValue<clientType>.Array;
            }
        }
        /// <summary>
        /// 验证函数接口
        /// </summary>
        private fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<clientType> verifyMethod;
        /// <summary>
        /// 验证函数客户端
        /// </summary>
        private clientType client;
        /// <summary>
        /// TCP调用服务端
        /// </summary>
        /// <param name="attribute">配置信息</param>
        /// <param name="isStart">是否启动客户端</param>
        /// <param name="maxCommandLength">最大命令长度</param>
        /// <param name="verify">验证接口</param>
        public commandClient(fastCSharp.code.cSharp.tcpServer attribute, int maxCommandLength, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify)
            : base(attribute, maxCommandLength, verify)
        {
        }
        /// <summary>
        /// TCP调用服务端
        /// </summary>
        /// <param name="attribute">配置信息</param>
        /// <param name="isStart">是否启动客户端</param>
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
        /// <param name="isStart">是否启动客户端</param>
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
        /// <summary>
        /// 函数验证
        /// </summary>
        /// <returns>是否验证成功</returns>
        protected override bool callVerifyMethod()
        {
            if (verifyMethod == null) return true;
            bool isError = false;
            interlocked.CompareSetSleep1(ref verifyMethodLock);
            try
            {
                if (verifyMethod.Verify(client)) return true;
            }
            catch (Exception error)
            {
                isError = true;
                log.Error.Add(error, "TCP客户端验证失败", false);
            }
            finally
            {
                verifyMethodLock = 0;
            }
            if (!isError) log.Error.Add("TCP客户端验证失败", true, false);
            Dispose();
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
            if (!isError) log.Error.Add("TCP客户端验证失败", true, false);
            Dispose();
            return false;
        }
    }
}
