using System;
using System.Collections.Generic;
using fastCSharp.threading;
using System.Threading;
using System.Runtime.CompilerServices;
using fastCSharp.code.cSharp;

namespace fastCSharp.net.tcp
{
    /// <summary>
    /// TCP调用负载均衡服务端
    /// </summary>
    public abstract class commandLoadBalancingServer
    {
        /// <summary>
        /// 添加TCP调用服务端命令索引位置
        /// </summary>
        internal const int NewServerCommandIndex = tcp.commandServer.CommandStartIndex + 1;
        /// <summary>
        /// 移除TCP调用服务端命令索引位置
        /// </summary>
        internal const int RemoveServerCommandIndex = NewServerCommandIndex + 1;
        /// <summary>
        /// 最大错误间隔时钟周期
        /// </summary>
        protected static readonly long maxErrorTimeTicks = new TimeSpan(0, 0, 2).Ticks;
        /// <summary>
        /// 验证接口
        /// </summary>
        protected fastCSharp.code.cSharp.tcpBase.ITcpClientVerify _verify_;
        /// <summary>
        /// TCP调用服务端信息
        /// </summary>
        internal sealed class serverInfo
        {
            /// <summary>
            /// TCP调用负载均衡服务端
            /// </summary>
            public commandLoadBalancingServer Server;
            /// <summary>
            /// TCP调用套接字
            /// </summary>
            public commandServer.socket Socket;
            /// <summary>
            /// TCP调用套接字回话标识
            /// </summary>
            public commandServer.streamIdentity Identity;
            /// <summary>
            /// TCP调用服务端端口信息
            /// </summary>
            public host Host;
            /// <summary>
            /// 添加TCP调用服务端
            /// </summary>
            public void NewServer()
            {
                Server.NewServer(this);
            }
        }
        /// <summary>
        /// 负载均衡服务客户端验证函数
        /// </summary>
#if NOJIT
        public sealed class verifyMethod : fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject
#else
        public sealed class verifyMethod : fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<fastCSharp.net.tcp.commandLoadBalancingServer.commandClient>
#endif
        {
            /// <summary>
            /// 负载均衡服务客户端验证
            /// </summary>
            /// <param name="client">负载均衡服务客户端</param>
            /// <returns>是否通过验证</returns>
#if NOJIT
            public bool Verify(object clientObject)
#else
            public bool Verify(fastCSharp.net.tcp.commandLoadBalancingServer.commandClient client)
#endif
            {
#if NOJIT
                fastCSharp.net.tcp.commandLoadBalancingServer.commandClient client = (fastCSharp.net.tcp.commandLoadBalancingServer.commandClient)clientObject;
#endif
                fastCSharp.net.tcp.timeVerifyServer.input input = new fastCSharp.net.tcp.timeVerifyServer.input();
                fastCSharp.net.tcp.commandClient commandClient = client.TcpCommandClient;
                fastCSharp.code.cSharp.tcpServer attribute = commandClient.Attribute;
                string verifyString = attribute.VerifyString;
                if (verifyString == null)
                {
                    return client.Verify(ref input).Value.Ret;
                }
                ulong markData = 0;
                if (attribute.IsMarkData) markData = attribute.VerifyHashCode;
                input.ticks = date.UtcNow.Ticks;
                do
                {
                    input.randomPrefix = random.Default.SecureNextULongNotZero();
                    while (input.randomPrefix == markData) input.randomPrefix = random.Default.SecureNextULongNotZero();
                    commandClient.ReceiveMarkData = attribute.IsMarkData ? markData ^ input.randomPrefix : 0UL;
                    commandClient.SendMarkData = 0;
                    input.MD5(verifyString);
                    long lastTicks = input.ticks;
                    fastCSharp.net.returnValue<fastCSharp.net.tcp.timeVerifyServer.output> isVerify = client.Verify(ref input);
                    if (isVerify.Value.Ret)
                    {
                        commandClient.SendMarkData = commandClient.ReceiveMarkData;
                        return true;
                    }
                    if (isVerify.Type != fastCSharp.net.returnValue.type.Success || input.ticks <= lastTicks) return false;
                    ++input.ticks;
                    log.Error.Add("TCP客户端验证时间失败重试", null, false);
                }
                while (true);
            }
        }
        /// <summary>
        /// TCP调用负载均衡客户端
        /// </summary>
        public sealed class commandClient : net.tcp.commandClient.IClient
        {
            /// <summary>
            /// 添加TCP调用服务端回调处理
            /// </summary>
            private sealed class serverReturn : callback<fastCSharp.net.returnValue<bool>>
            {
                /// <summary>
                /// 创建完成回调处理
                /// </summary>
                public Action<bool> OnReturn;
                /// <summary>
                /// 创建完成回调处理
                /// </summary>
                /// <param name="returnValue">返回值</param>
                public override void Callback(ref fastCSharp.net.returnValue<bool> returnValue)
                {
                    OnReturn(returnValue.Type == fastCSharp.net.returnValue.type.Success && returnValue.Value);
                }
            }
            /// <summary>
            /// TCP调用客户端
            /// </summary>
            private commandClient<commandClient> client;
            /// <summary>
            /// TCP调用客户端
            /// </summary>
            public fastCSharp.net.tcp.commandClient TcpCommandClient { get { return client; } }
            /// <summary>
            /// TCP调用客户端
            /// </summary>
            /// <param name="attribute">TCP调用服务器端配置信息</param>
            public commandClient(fastCSharp.code.cSharp.tcpServer attribute) : this(attribute, null) { }
            /// <summary>
            /// TCP调用客户端
            /// </summary>
            /// <param name="attribute">TCP调用服务器端配置信息</param>
            /// <param name="verifyMethod">TCP验证方法</param>
#if NOJIT
            public commandClient(fastCSharp.code.cSharp.tcpServer attribute, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject verifyMethod)
#else
            public commandClient(fastCSharp.code.cSharp.tcpServer attribute, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<commandClient> verifyMethod)
#endif
            {
                if (attribute == null) log.Error.Throw(log.exceptionType.Null);
                client = new commandClient<commandClient>(attribute, 1024, verifyMethod ?? new verifyMethod(), this);
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                pub.Dispose(ref client);
            }
            /// <summary>
            /// TCP调用客户端验证命令
            /// </summary>
            private static readonly net.tcp.commandClient.identityCommand verifyCommand = new tcp.commandClient.identityCommand { Command = commandServer.CommandStartIndex, MaxInputSize = 1024 };
            /// <summary>
            /// TCP调用客户端验证
            /// </summary>
            /// <param name="input"></param>
            /// <returns>是否验证成功</returns>
            internal fastCSharp.net.returnValue<fastCSharp.net.tcp.timeVerifyServer.output> Verify(ref fastCSharp.net.tcp.timeVerifyServer.input input)
            {
                keyValue<tcp.commandClient.streamCommandSocket, fastCSharp.net.waitCall<fastCSharp.net.tcp.timeVerifyServer.output>> wait = getWait<fastCSharp.net.tcp.timeVerifyServer.output>(true);
                if (wait.Value != null)
                {
                    try
                    {
                        fastCSharp.net.tcp.timeVerifyServer.output output = new fastCSharp.net.tcp.timeVerifyServer.output();
                        wait.Key.Get(wait.Value, verifyCommand, ref input, ref output, false);
                        fastCSharp.net.returnValue<fastCSharp.net.tcp.timeVerifyServer.output> outputReturn;
                        wait.Value.Get(out outputReturn);
                        return outputReturn;
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, null, false);
                    }
                }
                return default(fastCSharp.net.returnValue<fastCSharp.net.tcp.timeVerifyServer.output>);
            }
            /// <summary>
            /// 添加TCP调用服务端命令
            /// </summary>
            private static readonly net.tcp.commandClient.identityCommand newServerCommand = new tcp.commandClient.identityCommand { Command = NewServerCommandIndex, MaxInputSize = 1024 };
            /// <summary>
            /// 添加TCP调用服务端
            /// </summary>
            /// <param name="host">TCP调用服务端端口信息</param>
            /// <returns>是否添加成功</returns>
            public bool NewServer(host host)
            {
                keyValue<tcp.commandClient.streamCommandSocket, fastCSharp.net.waitCall<bool>> wait = getWait<bool>(false);
                if (wait.Value != null)
                {
                    try
                    {
                        fastCSharp.net.returnValue<bool> value = new returnValue<bool>();
                        wait.Key.Get(wait.Value, newServerCommand, ref host, ref value.Value, false);
                        wait.Value.Get(out value);
                        return value.Type == returnValue.type.Success && value.Value;
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, null, false);
                    }
                }
                return false;
            }
            /// <summary>
            /// 添加TCP调用服务端
            /// </summary>
            /// <param name="host">TCP调用服务端端口信息</param>
            /// <param name="onReturn">创建完成回调处理</param>
            public void NewServer(ref host host, Action<bool> onReturn)
            {
                try
                {
                    bool output = false;
                    client.StreamSocket.Get(onReturn == null ? null : new serverReturn { OnReturn = onReturn }, newServerCommand, ref host, ref output, true);
                    return;
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                if (onReturn != null) onReturn(false);
            }
            /// <summary>
            /// 移除TCP调用服务端命令
            /// </summary>
            private static readonly net.tcp.commandClient.identityCommand removeServerCommand = new tcp.commandClient.identityCommand { Command = RemoveServerCommandIndex, MaxInputSize = 1024 };
            /// <summary>
            /// 移除TCP调用服务端
            /// </summary>
            /// <param name="host">TCP调用服务端端口信息</param>
            /// <returns>是否移除成功</returns>
            public bool RemoveServer(host host)
            {
                keyValue<tcp.commandClient.streamCommandSocket, fastCSharp.net.waitCall<bool>> wait = getWait<bool>(false);
                if (wait.Value != null)
                {
                    try
                    {
                        fastCSharp.net.returnValue<bool> value = new returnValue<bool>();
                        wait.Key.Get(wait.Value, removeServerCommand, ref host, ref value.Value, false);
                        wait.Value.Get(out value);
                        return value.Type == returnValue.type.Success && value.Value;
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, null, false);
                    }
                }
                return false;
            }
            /// <summary>
            /// 获取同步等待调用
            /// </summary>
            /// <param name="isVerify">是否验证调用</param>
            /// <returns>TCP客户端套接字+同步等待调用</returns>
            private keyValue<tcp.commandClient.streamCommandSocket, fastCSharp.net.waitCall<returnType>> getWait<returnType>(bool isVerify)
            {
                try
                {
                    if (client != null)
                    {
                        tcp.commandClient.streamCommandSocket socket = isVerify ? client.VerifyStreamSocket : client.StreamSocket;
                        if (socket != null)
                        {
                            return new keyValue<tcp.commandClient.streamCommandSocket, fastCSharp.net.waitCall<returnType>>(socket, fastCSharp.net.waitCall<returnType>.Get());
                        }
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                return default(keyValue<tcp.commandClient.streamCommandSocket, fastCSharp.net.waitCall<returnType>>);
            }
        }
        /// <summary>
        /// 时间验证服务
        /// </summary>
        internal sealed class timeVerify : timeVerifyServer
        {
            /// <summary>
            /// 时间验证函数
            /// </summary>
            /// <param name="socket"></param>
            /// <param name="randomPrefix"></param>
            /// <param name="md5Data"></param>
            /// <param name="ticks"></param>
            /// <returns>是否验证成功</returns>
            public bool Verify(commandServer.socket socket, ulong randomPrefix, byte[] md5Data, ref long ticks)
            {
                return verify(socket, randomPrefix, md5Data, ref ticks);
            }
        }
        /// <summary>
        /// 检测任务
        /// </summary>
        internal abstract void Check();
        /// <summary>
        /// 添加TCP调用服务端
        /// </summary>
        /// <param name="server">TCP调用服务端信息</param>
        internal abstract void NewServer(serverInfo server);
    }
    /// <summary>
    /// TCP调用负载均衡服务端
    /// </summary>
    /// <typeparam name="clientType">TCP调用客户端类型</typeparam>
    public abstract class commandLoadBalancingServer<clientType> : commandLoadBalancingServer, IDisposable where clientType : class, IDisposable
    {
        /// <summary>
        /// TCP调用负载均衡服务端
        /// </summary>
        private sealed class commandServer : fastCSharp.net.tcp.commandServer
        {
            /// <summary>
            /// TCP调用负载均衡服务端目标对象
            /// </summary>
            private readonly commandLoadBalancingServer<clientType> server;
            /// <summary>
            /// TCP调用时间验证服务
            /// </summary>
            private readonly timeVerify timeVerify;
            /// <summary>
            /// TCP调用负载均衡服务端
            /// </summary>
            /// <param name="server">TCP调用负载均衡服务端目标对象</param>
            public commandServer(commandLoadBalancingServer<clientType> server)
                : base(server.attribute)
            {
                this.server = server;
                setCommands(3);
                identityOnCommands[verifyCommandIdentity = CommandStartIndex].Set(CommandStartIndex, 1024);
                identityOnCommands[commandLoadBalancingServer.NewServerCommandIndex].Set(commandLoadBalancingServer.NewServerCommandIndex, 1024);
                identityOnCommands[commandLoadBalancingServer.RemoveServerCommandIndex].Set(commandLoadBalancingServer.RemoveServerCommandIndex, 1024);
                (timeVerify = new timeVerify()).SetCommandServer(this);
            }
            /// <summary>
            /// 命令处理
            /// </summary>
            /// <param name="index"></param>
            /// <param name="socket"></param>
            /// <param name="data"></param>
            protected override void doCommand(int index, socket socket, ref subArray<byte> data)
            {
                if (index < CommandStartIndex) base.doCommand(index, socket, ref data);
                else
                {
                    switch (index - CommandStartIndex)
                    {
                        case CommandStartIndex - CommandStartIndex: verify(socket, ref data); return;
                        case commandLoadBalancingServer.NewServerCommandIndex - CommandStartIndex: newServer(socket, ref data); return;
                        case commandLoadBalancingServer.RemoveServerCommandIndex - CommandStartIndex: removeServer(socket, ref data); return;
                    }
                }
            }
            /// <summary>
            /// TCP调用服务端验证
            /// </summary>
            /// <param name="socket">TCP调用套接字</param>
            /// <param name="data">参数序列化数据</param>
            private void verify(socket socket, ref subArray<byte> data)
            {
                fastCSharp.net.returnValue.type returnType;
                try
                {
                    fastCSharp.net.tcp.timeVerifyServer.input inputParameter = default(fastCSharp.net.tcp.timeVerifyServer.input);
                    if (fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter))
                    {
                        fastCSharp.net.returnValue<fastCSharp.net.tcp.timeVerifyServer.output> returnValue = new returnValue<timeVerifyServer.output> { Value = new fastCSharp.net.tcp.timeVerifyServer.output { Ret = timeVerify.Verify(socket, inputParameter.randomPrefix, inputParameter.md5Data, ref inputParameter.ticks) } };
                        returnValue.Value.ticks = inputParameter.ticks;
                        if (returnValue.Value.Ret) socket.SetVerifyMethod();
                        returnValue.Type = net.returnValue.type.Success;
                        socket.SendStream(ref socket.identity, ref returnValue, default(tcp.commandServer.commandFlags));
                        return;
                    }
                    returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                }
                catch (Exception error)
                {
                    returnType = fastCSharp.net.returnValue.type.ServerException;
                    fastCSharp.log.Error.Add(error, null, true);
                }
                socket.SendStream(ref socket.identity, returnType);
            }
            /// <summary>
            /// 添加TCP调用服务端
            /// </summary>
            /// <param name="socket">TCP调用套接字</param>
            /// <param name="data">参数序列化数据</param>
            private void newServer(socket socket, ref subArray<byte> data)
            {
                fastCSharp.net.returnValue.type returnType;
                try
                {
                    host host = new host();
                    if (fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref host))
                    {
                        fastCSharp.threading.threadPool.TinyPool.FastStart(new serverInfo { Server = server, Socket = socket, Identity = socket.Identity, Host = host }, thread.callType.TcpCommandLoadBalancingServerInfo);
                        return;
                    }
                    returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                }
                catch (Exception error)
                {
                    returnType = fastCSharp.net.returnValue.type.ServerException;
                    fastCSharp.log.Error.Add(error, null, true);
                }
                socket.SendStream(ref socket.identity, returnType);
            }
            /// <summary>
            /// 移除TCP调用服务端
            /// </summary>
            /// <param name="socket">TCP调用套接字</param>
            /// <param name="data">参数序列化数据</param>
            private void removeServer(socket socket, ref subArray<byte> data)
            {
                fastCSharp.net.returnValue.type returnType;
                try
                {
                    host host = new host();
                    if (fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref host))
                    {
                        fastCSharp.net.returnValue<bool> output = new fastCSharp.net.returnValue<bool> { Type = returnValue.type.Success, Value = server.removeServer(ref host) };
                        socket.SendStream(ref socket.identity, ref output, default(tcp.commandServer.commandFlags));
                        return;
                    }
                    returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                }
                catch (Exception error)
                {
                    returnType = fastCSharp.net.returnValue.type.ServerException;
                    fastCSharp.log.Error.Add(error, null, true);
                }
                socket.SendStream(ref socket.identity, returnType);
            }
        }
        /// <summary>
        /// TCP调用服务器信息
        /// </summary>
        public struct clientIdentity
        {
            /// <summary>
            /// TCP调用客户端
            /// </summary>
            public clientType Client;
            /// <summary>
            /// TCP调用客户端索引
            /// </summary>
            public int Index;
            /// <summary>
            /// 验证编号
            /// </summary>
            public int Identity;
            /// <summary>
            /// 设置TCP调用客户端
            /// </summary>
            /// <param name="client">TCP调用客户端</param>
            /// <param name="index">TCP调用客户端索引</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void Set(clientType client, int index)
            {
                Client = client;
                Index = index;
            }
            /// <summary>
            /// 重置TCP调用客户端
            /// </summary>
            /// <param name="client">TCP调用客户端</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void Set(clientType client)
            {
                Client = client;
                ++Identity;
            }
            /// <summary>
            /// 移除TCP调用客户端
            /// </summary>
            /// <returns>TCP调用客户端</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal clientType GetRemove()
            {
                clientType client = Client;
                Client = null;
                ++Identity;
                return client;
            }
            /// <summary>
            /// 移除TCP调用客户端
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void Remove()
            {
                Client = null;
                ++Identity;
            }
        }
        /// <summary>
        /// TCP调用服务器信息
        /// </summary>
        public struct clientHost
        {
            /// <summary>
            /// TCP调用服务器信息
            /// </summary>
            public clientIdentity Client;
            /// <summary>
            /// 最后响应时间
            /// </summary>
            public DateTime LastTime;
            /// <summary>
            /// TCP调用端口信息
            /// </summary>
            public host Host;
            /// <summary>
            /// 当前处理数量
            /// </summary>
            public int Count;
            /// <summary>
            /// 设置TCP调用客户端
            /// </summary>
            /// <param name="host">TCP调用端口信息</param>
            /// <param name="client">TCP调用客户端</param>
            /// <param name="index">TCP调用客户端索引</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void Set(ref host host, clientType client, int index)
            {
                Client.Set(client, index);
                Host = host;
                Count = 0;
            }
            /// <summary>
            /// 重置TCP调用客户端
            /// </summary>
            /// <param name="host">TCP调用端口信息</param>
            /// <param name="client">TCP调用客户端</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void Set(ref host host, clientType client)
            {
                Client.Client = client;
                Host = host;
                Count = 0;
            }
            /// <summary>
            /// 重置TCP调用客户端
            /// </summary>
            /// <param name="host">TCP调用端口信息</param>
            /// <param name="client">TCP调用客户端</param>
            /// <returns>TCP调用客户端+未完成处理数量</returns>
            internal keyValue<clientType, int> ReSet(ref host host, clientType client)
            {
                if (Client.Client != null && Host.Equals(host))
                {
                    clientType removeClient = Client.Client;
                    int count = Count;
                    Client.Set(client);
                    Count = 0;
                    return new keyValue<clientType, int>(removeClient, count);
                }
                return default(keyValue<clientType, int>);
            }
            /// <summary>
            /// 移除TCP调用客户端
            /// </summary>
            /// <param name="host">TCP调用端口信息</param>
            /// <returns>TCP调用客户端+未完成处理数量</returns>
            internal keyValue<clientType, int> Remove(ref host host)
            {
                if (Client.Client != null && Host.Equals(host)) return Remove();
                return default(keyValue<clientType, int>);
            }
            /// <summary>
            /// 移除TCP调用客户端
            /// </summary>
            /// <returns>TCP调用客户端+未完成处理数量</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal keyValue<clientType, int> Remove()
            {
                clientType client = Client.GetRemove();
                int count = Count;
                return new keyValue<clientType, int>(client, count);
            }
            /// <summary>
            /// 移除TCP调用客户端
            /// </summary>
            /// <param name="client">TCP调用客户端</param>
            /// <returns>TCP调用端口信息+未完成处理数量</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal keyValue<host, int> Remove(clientType client)
            {
                if (Client.Client == client)
                {
                    Client.Remove();
                    return new keyValue<host, int>(Host, Count);
                }
                return default(keyValue<host, int>);
            }
            /// <summary>
            /// 测试当前处理数量
            /// </summary>
            /// <param name="count">最大处理数量</param>
            /// <returns>是否测试成功</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal bool TryCount(int count)
            {
                if (Count <= count && Client.Client != null)
                {
                    ++Count;
                    return true;
                }
                return false;
            }
            /// <summary>
            /// TCP调用客户端调用结束
            /// </summary>
            /// <param name="identity">验证编号</param>
            /// <returns>是否验证成功</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal int End(int identity)
            {
                if (Client.Identity == identity)
                {
                    LastTime = date.nowTime.Now;
                    --Count;
                    return 1;
                }
                return 0;
            }
            /// <summary>
            /// TCP调用客户端调用错误
            /// </summary>
            /// <param name="identity">验证编号</param>
            /// <returns>开始错误时间</returns>
            internal keyValue<clientType, keyValue<host, int>> Error(int identity)
            {
                if (Client.Identity == identity)
                {
                    clientType client = Client.GetRemove();
                    return new keyValue<clientType, keyValue<host, int>>(client, new keyValue<host, int>(Host, Count - 1));
                }
                return default(keyValue<clientType, keyValue<host, int>>);
            }
            /// <summary>
            /// 超时检测
            /// </summary>
            /// <param name="timeout">超时时间</param>
            /// <returns>TCP调用客户端</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal clientType CheckTimeout(DateTime timeout)
            {
                return LastTime < timeout ? Client.Client : null;
            }
        }
        /// <summary>
        /// TCP调用服务器端配置信息
        /// </summary>
        private fastCSharp.code.cSharp.tcpServer attribute;
        /// <summary>
        /// 验证函数接口
        /// </summary>
#if NOJIT
        protected fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject _verifyMethod_;
#else
        protected fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<clientType> _verifyMethod_;
#endif
        /// <summary>
        /// TCP调用客户端集合
        /// </summary>
        private clientHost[] clients = new clientHost[sizeof(int)];
        /// <summary>
        /// TCP调用客户端空闲索引
        /// </summary>
        private list<int> freeIndexs = new list<int>();
        /// <summary>
        /// 已使用的TCP调用客户端数量(包括空闲索引)
        /// </summary>
        private int currentCount;
        /// <summary>
        /// 当前访问TCP调用客户端索引
        /// </summary>
        private int currentIndex;
        /// <summary>
        /// 当前调用总数
        /// </summary>
        private int callCount;
        /// <summary>
        /// TCP调用客户端访问锁
        /// </summary>
        private readonly object clientLock = new object();
        /// <summary>
        /// 最后一次检测时间
        /// </summary>
        private DateTime checkTime;
        /// <summary>
        /// 移除TCP调用客户端集合
        /// </summary>
        private keyValue<clientType, int>[] removeClients = nullValue<keyValue<clientType, int>>.Array;
        /// <summary>
        /// 是否启动检测任务
        /// </summary>
        private byte isCheckTask;
        /// <summary>
        /// 是否已经释放资源
        /// </summary>
        private byte isDisposed;
        /// <summary>
        /// TCP调用负载均衡服务端
        /// </summary>
        /// <param name="attribute">TCP调用服务器端配置信息</param>
        /// <param name="verifyMethod">验证函数接口</param>
        /// <param name="verify">验证接口</param>
#if NOJIT
        protected commandLoadBalancingServer(fastCSharp.code.cSharp.tcpServer attribute, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject verifyMethod, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify)
#else
        protected commandLoadBalancingServer(fastCSharp.code.cSharp.tcpServer attribute, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<clientType> verifyMethod, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify)
#endif
        {
        }
        /// <summary>
        /// TCP调用负载均衡服务端
        /// </summary>
        /// <param name="attribute">TCP调用服务器端配置信息</param>
        /// <param name="verifyMethod">验证函数接口</param>
#if NOJIT
        protected commandLoadBalancingServer(fastCSharp.code.cSharp.tcpServer attribute, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject verifyMethod)
#else
        protected commandLoadBalancingServer(fastCSharp.code.cSharp.tcpServer attribute, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<clientType> verifyMethod)
#endif
        {
            this.attribute = attribute;
            _verifyMethod_ = verifyMethod;
        }
        /// <summary>
        /// TCP调用负载均衡服务端
        /// </summary>
        /// <param name="attribute">TCP调用服务器端配置信息</param>
        /// <param name="verify">验证接口</param>
        protected commandLoadBalancingServer(fastCSharp.code.cSharp.tcpServer attribute, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify)
        {
            this.attribute = attribute;
            _verify_ = verify;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            isDisposed = 1;
            Monitor.Enter(serverLock);
            pub.Dispose(ref server);
            Monitor.Exit(serverLock);
            int count = 0;
            clientType[] clients;
            do
            {
                clients = new clientType[this.clients.Length];
                Monitor.Enter(clientLock);
                currentIndex = callCount = 0;
                if (currentCount <= clients.Length)
                {
                    while (count != currentCount) clients[count++] = this.clients[count++].Client.GetRemove();
                    freeIndexs.Clear();
                    currentCount = 0;
                    Monitor.Exit(clientLock);
                    break;
                }
                else Monitor.Exit(clientLock);
            }
            while (true);
            while (count != 0) pub.Dispose(ref clients[--count]);
        }
        /// <summary>
        /// TCP调用负载均衡服务端
        /// </summary>
        private commandServer server;
        /// <summary>
        /// TCP调用负载均衡服务端访问锁
        /// </summary>
        private object serverLock = new object();
        /// <summary>
        /// 超时检测时钟周期
        /// </summary>
        private long checkTicks;
        /// <summary>
        /// 启动负载均衡服务
        /// </summary>
        /// <returns>是否成功</returns>
        public bool StartLoadBalancingServer()
        {
            attribute.IsLoadBalancing = false;
            checkTicks = new TimeSpan(0, 0, Math.Max(attribute.LoadBalancingCheckSeconds + 2, 2)).Ticks;
            Monitor.Enter(serverLock);
            try
            {
                if (server == null)
                {
                    server = new commandServer(this);
                    if (server.Start()) return true;
                    pub.Dispose(ref server);
                }
            }
            finally { Monitor.Exit(serverLock); }
            return false;
        }
        /// <summary>
        /// 获取一个TCP调用客户端
        /// </summary>
        /// <returns>TCP调用服务器信息</returns>
        protected clientIdentity _getClient_()
        {
            if (isDisposed == 0)
            {
                Monitor.Enter(clientLock);
                int count = currentCount - freeIndexs.length;
                if (count != 0)
                {
                    int callCount = this.callCount / count + 1, index = currentIndex;
                    do
                    {
                        if (clients[currentIndex].TryCount(callCount))
                        {
                            ++this.callCount;
                            clientIdentity value = clients[currentIndex].Client;
                            Monitor.Exit(clientLock);
                            return value;
                        }
                    }
                    while (++currentIndex != currentCount);
                    for (currentIndex = 0; currentIndex != index; ++currentIndex)
                    {
                        if (clients[currentIndex].TryCount(callCount))
                        {
                            ++this.callCount;
                            clientIdentity value = clients[currentIndex].Client;
                            Monitor.Exit(clientLock);
                            return value;
                        }
                    }
                }
                Monitor.Exit(clientLock);
            }
            return default(clientIdentity);
        }
        /// <summary>
        /// TCP调用客户端调用结束
        /// </summary>
        /// <param name="client">TCP调用服务器信息</param>
        /// <param name="returnType">是否回调成功</param>
        protected void _end_(ref clientIdentity client, fastCSharp.net.returnValue.type returnType)
        {
            if (isDisposed == 0)
            {
                if (returnType == fastCSharp.net.returnValue.type.Success)
                {
                    Monitor.Enter(clientLock);
                    callCount -= clients[client.Index].End(client.Identity);
                    Monitor.Exit(clientLock);
                }
                else
                {
                    Monitor.Enter(clientLock);
                    keyValue<clientType, keyValue<host, int>> errorClient = clients[client.Index].Error(client.Identity);
                    if (errorClient.Key == null) Monitor.Exit(clientLock);
                    else
                    {
                        callCount -= errorClient.Value.Value;
                        try
                        {
                            freeIndexs.Add(client.Index);
                        }
                        finally
                        {
                            Monitor.Exit(clientLock);
                            pub.Dispose(ref errorClient.Key);

                            host host = errorClient.Value.Key;
                            bool isCreate = newServer(ref host);
                            if (isCreate)
                            {
                                tryCheck();
                                log.Default.Add("恢复TCP调用服务端[调用错误] " + host.Host + ":" + host.Port.toString(), new System.Diagnostics.StackFrame(), false);
                            }
                            else log.Default.Add("移除TCP调用服务端[调用错误] " + host.Host + ":" + host.Port.toString(), new System.Diagnostics.StackFrame(), false);
                        }
                    }
                }
            }
            client.Identity = int.MinValue;
            client.Client = null;
        }
        /// <summary>
        /// 创建TCP调用客户端
        /// </summary>
        /// <param name="attribute">TCP调用服务器端配置信息</param>
        /// <returns>TCP调用客户端</returns>
        protected abstract clientType _createClient_(fastCSharp.code.cSharp.tcpServer attribute);
        /// <summary>
        /// 获取负载均衡联通最后检测时间
        /// </summary>
        /// <param name="client">TCP调用客户端</param>
        /// <returns>负载均衡联通最后检测时间</returns>
        protected abstract DateTime _loadBalancingCheckTime_(clientType client);
        /// <summary>
        /// 负载均衡超时检测
        /// </summary>
        /// <param name="client">TCP调用客户端</param>
        /// <returns>TCP调用客户端是否可用</returns>
        protected abstract fastCSharp.net.returnValue.type _loadBalancingCheck_(clientType client);
        /// <summary>
        /// 添加TCP调用服务端
        /// </summary>
        /// <param name="server">TCP调用服务端信息</param>
        internal override void NewServer(serverInfo server)
        {
            bool isCreate = newServer(ref server.Host);
            if (isCreate)
            {
                tryCheck();
                log.Default.Add("添加TCP调用服务端 " + server.Host.Host + ":" + server.Host.Port.toString(), new System.Diagnostics.StackFrame(), false);
            }
            fastCSharp.net.returnValue<bool> output = new fastCSharp.net.returnValue<bool> { Type = returnValue.type.Success, Value = isCreate };
            server.Socket.SendStream(ref server.Identity, ref output, default(tcp.commandServer.commandFlags));
        }
        /// <summary>
        /// 添加TCP调用服务端
        /// </summary>
        /// <param name="host">TCP服务端口信息</param>
        /// <returns>是否添加成功</returns>
        private bool newServer(ref host host)
        {
            if (isDisposed == 0)
            {
                try
                {
                    fastCSharp.code.cSharp.tcpServer attribute = this.attribute.Clone();
                    attribute.IsLoadBalancing = true;
                    attribute.Host = host.Host;
                    attribute.Port = host.Port;
                    clientType client = _createClient_(attribute);
                    if (client != null)
                    {
                        Monitor.Enter(clientLock);
                        for (int index = 0; index != currentCount; ++index)
                        {
                            keyValue<clientType, int> removeClient = this.clients[index].ReSet(ref host, client);
                            if (removeClient.Key != null)
                            {
                                callCount -= removeClient.Value;
                                Monitor.Exit(clientLock);
                                pub.Dispose(ref removeClient.Key);
                                return true;
                            }
                        }
                        if (freeIndexs.length == 0)
                        {
                            if (currentCount == this.clients.Length)
                            {
                                try
                                {
                                    clientHost[] clients = new clientHost[currentCount << 1];
                                    this.clients.CopyTo(clients, 0);
                                    clients[currentCount].Set(ref host, client, currentCount);
                                    this.clients = clients;
                                    ++currentCount;
                                    client = null;
                                }
                                finally
                                {
                                    Monitor.Exit(clientLock);
                                    pub.Dispose(ref client);
                                }
                            }
                            else
                            {
                                clients[currentCount].Set(ref host, client, currentCount);
                                ++currentCount;
                                Monitor.Exit(clientLock);
                            }
                        }
                        else
                        {
                            clients[freeIndexs.UnsafePop()].Set(ref host, client);
                            Monitor.Exit(clientLock);
                        }
                        return true;
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
            }
            return false;
        }
        /// <summary>
        /// 移除TCP调用服务端
        /// </summary>
        /// <param name="host">TCP调用服务端端口信息</param>
        /// <returns>是否移除成功</returns>
        private bool removeServer(ref host host)
        {
            if (isDisposed == 0)
            {
                Monitor.Enter(clientLock);
                for (int index = 0; index != currentCount; ++index)
                {
                    keyValue<clientType, int> removeClient = this.clients[index].Remove(ref host);
                    if (removeClient.Key != null)
                    {
                        callCount -= removeClient.Value;
                        try
                        {
                            freeIndexs.Add(index);
                        }
                        finally
                        {
                            Monitor.Exit(clientLock);
                            pub.Dispose(ref removeClient.Key);
                        }
                        log.Default.Add("移除TCP调用服务端 " + host.Host + ":" + host.Port.toString(), new System.Diagnostics.StackFrame(), false);
                        return true;
                    }
                }
                Monitor.Exit(clientLock);
            }
            return false;
        }
        /// <summary>
        /// 添加检测任务
        /// </summary>
        private void tryCheck()
        {
            Monitor.Enter(clientLock);
            if (isCheckTask == 0)
            {
                isCheckTask = 1;
                Monitor.Exit(clientLock);
                addCheck();
            }
            else Monitor.Exit(clientLock);
        }
        /// <summary>
        /// 添加检测任务
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void addCheck()
        {
            DateTime now = date.nowTime.Now;
            if (checkTime < now) checkTime = now;
            fastCSharp.threading.timerTask.Default.Add(this, thread.callType.TcpCommandLoadBalancingServerCheck, checkTime = checkTime.AddSeconds(1));
        }
        /// <summary>
        /// 检测任务
        /// </summary>
        internal override void Check()
        {
            if (isDisposed == 0)
            {
                int count = 0;
                DateTime now = date.nowTime.Now.AddTicks(-checkTicks);
                Monitor.Enter(clientLock);
                try
                {
                    if (removeClients.Length < currentCount) removeClients = new keyValue<clientType, int>[clients.Length];
                    for (int index = 0; index != currentCount; ++index)
                    {
                        clientType client = clients[index].CheckTimeout(now);
                        if (client != null && _loadBalancingCheckTime_(client) < now) removeClients[count++].Set(client, index);
                    }
                }
                finally { Monitor.Exit(clientLock); }
                while (count != 0)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                    clientType client = removeClients[--count].Key;
                    try
                    {
                        returnType = _loadBalancingCheck_(client);
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, null, false);
                    }
                    if (returnType != fastCSharp.net.returnValue.type.Success)
                    {
                        int index = removeClients[count].Value;
                        Monitor.Enter(clientLock);
                        keyValue<host, int> host = clients[index].Remove(client);
                        if (host.Key.Host == null) Monitor.Exit(clientLock);
                        else
                        {
                            callCount -= host.Value;
                            try
                            {
                                freeIndexs.Add(index);
                            }
                            finally { Monitor.Exit(clientLock); }
                            pub.Dispose(ref client);

                            if (newServer(ref host.Key))
                            {
                                log.Default.Add("恢复TCP调用服务端[检测超时] " + host.Key.Host + ":" + host.Key.Port.toString(), new System.Diagnostics.StackFrame(), false);
                            }
                            else log.Default.Add("移除TCP调用服务端[检测超时] " + host.Key.Host + ":" + host.Key.Port.toString(), new System.Diagnostics.StackFrame(), false);
                        }
                    }
                }
                Monitor.Enter(clientLock);
                if (currentCount == freeIndexs.length)
                {
                    isCheckTask = 0;
                    Monitor.Exit(clientLock);
                }
                else
                {
                    Monitor.Exit(clientLock);
                    addCheck();
                }
            }
        }
    }
}
