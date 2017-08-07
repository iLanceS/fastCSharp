using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.IO;
using fastCSharp.threading;
using System.Runtime.CompilerServices;

namespace fastCSharp.net.tcp
{
    /// <summary>
    /// TCP注册服务
    /// </summary>
#if NotFastCSharpCode
    [fastCSharp.code.cSharp.tcpServer(Service = tcpRegister.ServiceName, IsIdentityCommand = true)]
#else
    [fastCSharp.code.cSharp.tcpServer(Service = tcpRegister.ServiceName, IsIdentityCommand = true, VerifyMethodType = typeof(tcpRegister.tcpClient.timeVerifyMethod))]
#endif
    public partial class tcpRegister : tcpRegisterReader
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        internal new const string ServiceName = "tcpRegister";
        /// <summary>
        /// 注册状态
        /// </summary>
        public enum registerState : byte
        {
            /// <summary>
            /// 客户端不可用
            /// </summary>
            NoClient,
            /// <summary>
            /// 客户端标识错误
            /// </summary>
            ClientError,
            /// <summary>
            /// 单例服务冲突
            /// </summary>
            SingleError,
            /// <summary>
            /// TCP 服务端口信息不合法
            /// </summary>
            HostError,
            /// <summary>
            /// TCP 服务端口信息已存在
            /// </summary>
            HostExists,
            /// <summary>
            /// 没有可用的端口号
            /// </summary>
            PortError,
            /// <summary>
            /// TCP 服务信息检测被更新,需要重试
            /// </summary>
            ServiceChange,
            /// <summary>
            /// 注册成功
            /// </summary>
            Success,
        }
        /// <summary>
        /// 注册结果
        /// </summary>
        [fastCSharp.emit.dataSerialize(IsReferenceMember = false, IsMemberMap = false)]
        public struct registerResult
        {
            /// <summary>
            /// 注册状态
            /// </summary>
            public registerState State;
            /// <summary>
            /// 注册成功的TCP服务信息
            /// </summary>
            public service Service;
        }
        /// <summary>
        /// 客户端索引编号
        /// </summary>
        private struct clientIdentity
        {
            /// <summary>
            /// 索引编号
            /// </summary>
            public int Identity;
            /// <summary>
            /// 客户端信息
            /// </summary>
            public clientInfo Client;
            /// <summary>
            /// 设置客户端信息
            /// </summary>
            /// <param name="client"></param>
            /// <returns></returns>
            public int Set(clientInfo client)
            {
                Client = client;
                return Identity;
            }
            /// <summary>
            /// 获取客户端信息
            /// </summary>
            /// <param name="identity"></param>
            /// <returns></returns>
            public clientInfo Get(int identity)
            {
                return identity == Identity ? Client : null;
            }
            /// <summary>
            /// 释放客户端索引编号
            /// </summary>
            /// <param name="identity"></param>
            /// <returns></returns>
            public bool Free(int identity)
            {
                if (identity == Identity)
                {
                    Client = null;
                    ++Identity;
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// 客户端信息
        /// </summary>
        private sealed class clientInfo
        {
            /// <summary>
            /// 日志回调
            /// </summary>
            public Func<fastCSharp.net.returnValue<log>, bool> OnLog;
            /// <summary>
            /// 日志回调
            /// </summary>
            public readonly Func<fastCSharp.net.returnValue<log>, bool> OnLogHandle;
            /// <summary>
            /// 客户端信息
            /// </summary>
            public clientInfo()
            {
                OnLogHandle = GetLog;
            }
            /// <summary>
            /// 设置日志回调
            /// </summary>
            /// <param name="onLog"></param>
            /// <returns></returns>

            public Func<fastCSharp.net.returnValue<log>, bool> SetOnLog(Func<fastCSharp.net.returnValue<log>, bool> onLog)
            {
                return Interlocked.Exchange(ref OnLog, onLog);
            }
            /// <summary>
            /// 日志回调
            /// </summary>
            /// <param name="log"></param>
            /// <returns></returns>
            public bool GetLog(fastCSharp.net.returnValue<log> log)
            {
                Func<fastCSharp.net.returnValue<log>, bool> onLog = OnLog;
                if (onLog != null)
                {
                    //if (log.Value == null) onLog(new code.cSharp.fastCSharp.net.returnValue<log> { Type = fastCSharp.net.returnValue.type.LogStreamExpired });
                    //else if (onLog(log)) return true;
                    if (onLog(log)) return true;
                    Interlocked.CompareExchange(ref OnLog, null, onLog);
                }
                return false;
            }
        }
        /// <summary>
        /// TCP注册服务 客户端
        /// </summary>
        public sealed class client : IDisposable
        {
            /// <summary>
            /// TCP注册服务名称
            /// </summary>
            private string serviceName;
            /// <summary>
            /// TCP服务端标识
            /// </summary>
            private clientId clientId;
#if NotFastCSharpCode
#else
            /// <summary>
            /// TCP注册服务客户端
            /// </summary>
            private fastCSharp.net.tcp.tcpRegisterReader.tcpClient registerReaderClient;
            /// <summary>
            /// TCP注册服务客户端
            /// </summary>
            private fastCSharp.net.tcp.tcpRegister.tcpClient registerClient;
#endif
            /// <summary>
            /// TCP服务信息
            /// </summary>
            private Dictionary<hashString, services> services = dictionary.CreateHashString<services>();
            /// <summary>
            /// TCP服务信息访问锁
            /// </summary>
            private readonly object servicesLock = new object();
            /// <summary>
            /// TCP注册服务访问锁
            /// </summary>
            private readonly object clientLock = new object();
            ///// <summary>
            ///// TCP服务信息更新事件
            ///// </summary>
            //internal event Action<string> OnServiceChanged;
            /////// <summary>
            /////// 创建TCP注册服务客户端失败是否输出日志
            /////// </summary>
            ////private bool isNewClientErrorLog;
            /// <summary>
            /// 客户端轮询
            /// </summary>
            private Action<fastCSharp.net.returnValue<log>> logHandle;
            /// <summary>
            /// 客户端保持回调
            /// </summary>
            private commandClient.streamCommandSocket.keepCallback logKeep;
            /// <summary>
            /// TCP注册服务客户端
            /// </summary>
            /// <param name="serviceName">TCP注册服务服务名称</param>
            public client(string serviceName)
            {
#if NotFastCSharpCode
                fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.NotFastCSharpCode);
#else
                //attribute = fastCSharp.config.pub.LoadConfig(new fastCSharp.code.cSharp.tcpServer(), serviceName);
                //attribute.IsIdentityCommand = true;
                //attribute.TcpRegister = null;
                registerClient = new fastCSharp.net.tcp.tcpRegister.tcpClient(fastCSharp.code.cSharp.tcpServer.GetConfig(serviceName, typeof(fastCSharp.net.tcp.tcpRegisterReader)));
                registerReaderClient = new fastCSharp.net.tcp.tcpRegisterReader.tcpClient(fastCSharp.code.cSharp.tcpServer.GetConfig(serviceName + "Reader", typeof(fastCSharp.net.tcp.tcpRegisterReader)));
                this.serviceName = serviceName;
                //isNewClientErrorLog = true;
                logHandle = onLog;
                Start();
#endif
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
#if NotFastCSharpCode
                fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.NotFastCSharpCode);
#else
                Monitor.Enter(clientLock);
                try
                {
                    if (registerReaderClient != null) close();
                    pub.Dispose(ref registerReaderClient);
                    pub.Dispose(ref registerClient);
                }
                finally { Monitor.Exit(clientLock); }
                Monitor.Enter(servicesLock);
                foreach (services services in this.services.Values) services.Clients = null;
                Monitor.Exit(servicesLock);
#endif
            }
#if NotFastCSharpCode
#else
            /// <summary>
            /// 关闭客户端
            /// </summary>
            private void close()
            {
                pub.Dispose(ref logKeep);
                if (clientId.Tick != 0)
                {
                    try
                    {
                        registerClient.removeRegister(clientId);
                    }
                    catch (Exception error)
                    {
                        fastCSharp.log.Error.Add(error, null, false);
                    }
                    clientId.Tick = 0;
                }
            }
            /// <summary>
            /// 启动TCP注册服务客户端
            /// </summary>
            internal void Start()
            {
                bool isStart = false;
                Monitor.Enter(clientLock);
                try
                {
                    if (registerReaderClient != null)
                    {
                        close();
                        if ((clientId = registerReaderClient.register().Value).Tick != 0)
                        {
                            isStart = getLog();
                        }
                    }
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
                finally { Monitor.Exit(clientLock); }
                fastCSharp.log.Default.Add("TCP注册客户端启动 " + (isStart ? "成功" : "失败"), new System.Diagnostics.StackFrame(), false);
                if (!isStart && registerReaderClient != null)
                {
                    fastCSharp.threading.timerTask.Default.Add(this, thread.callType.TcpRegisterClientStart, fastCSharp.date.nowTime.Now.AddSeconds(2));
                }
            }
            /// <summary>
            /// 获取日志
            /// </summary>
            /// <returns></returns>
            private bool getLog()
            {
                int logIdentity;
                services[] services = registerReaderClient.getServices(out logIdentity).Value;
                if (services != null)
                {
                    services cacheServices;
                    Monitor.Enter(servicesLock);
                    try
                    {
                        foreach (services service in services)
                        {
                            hashString name = service.Name;
                            if (this.services.TryGetValue(name, out cacheServices)) cacheServices.Copy(service);
                            else
                            {
                                //service.Version = 1;
                                this.services.Add(name, service);
                            }
                        }
                    }
                    finally { Monitor.Exit(servicesLock); }
                    //if (OnServiceChanged != null) OnServiceChanged(null);
                    logKeep = registerReaderClient.getLog(clientId, logIdentity, logHandle);
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 客户端轮询
            /// </summary>
            /// <param name="result">轮询结果</param>
            private void onLog(fastCSharp.net.returnValue<log> result)
            {
                if (result.Type == fastCSharp.net.returnValue.type.Success)
                {
                    hashString name;
                    services services;
                    switch (result.Value.Type)
                    {
                        case log.type.ClientError:
                            Monitor.Enter(clientLock);
                            try
                            {
                                close();
                            }
                            finally { Monitor.Exit(clientLock); }
                            break;
                        case log.type.Check: return;
                        case log.type.NewGetter:
                            fastCSharp.log.Default.Add(serviceName + " 日志轮询客户端冲突", new System.Diagnostics.StackFrame(), false);
                            return;
                        //case log.type.VersionError:
                        //    Monitor.Enter(clientLock);
                        //    try
                        //    {
                        //        pub.Dispose(ref logKeep);
                        //        if (getLog()) return;
                        //    }
                        //    finally { Monitor.Exit(clientLock); }
                        //    break;
                        case log.type.HostChanged:
                            name = result.Value.Services.Name;
                            Monitor.Enter(servicesLock);
                            try
                            {
                                if (this.services.TryGetValue(name, out services)) services.SetHosts(result.Value.Services.Hosts);
                                else
                                {
                                    services = result.Value.Services;
                                    //services.Version = 1;
                                    this.services.Add(name, services);
                                }
                            }
                            finally { Monitor.Exit(servicesLock); }
                            //if (OnServiceChanged != null) OnServiceChanged(result.Value.Services.Name);
                            return;
                        case log.type.RemoveServiceName:
                            //bool isChanged = false;
                            name = result.Value.Services.Name;
                            Monitor.Enter(servicesLock);
                            //if (this.services.TryGetValue(name, out services)) isChanged = services.Remove();
                            if (this.services.TryGetValue(name, out services)) services.Remove();
                            Monitor.Exit(servicesLock);
                            //if (isChanged && OnServiceChanged != null) OnServiceChanged(result.Value.Services.Name);
                            return;
                        default:
                            fastCSharp.log.Error.Add("不可识别的日志类型 " + result.Value.Type.ToString(), new System.Diagnostics.StackFrame(), false);
                            break;
                    }
                }
                else if (result.Type == fastCSharp.net.returnValue.type.LogStreamExpired)
                {
                    Monitor.Enter(clientLock);
                    try
                    {
                        pub.Dispose(ref logKeep);
                        if (getLog()) return;
                    }
                    finally { Monitor.Exit(clientLock); }
                }
                Start();
            }
#endif
            /// <summary>
            /// 注册TCP服务端
            /// </summary>
            /// <param name="attribute">TCP服务配置</param>
            /// <returns>是否注册成功</returns>
            public registerState Register(fastCSharp.code.cSharp.tcpServer attribute)
            {
                registerResult result = new registerResult { State = registerState.NoClient };
#if NotFastCSharpCode
                fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.NotFastCSharpCode);
#else
                Monitor.Enter(clientLock);
                try
                {
                    result = registerClient.register(clientId, new service { Host = new host { Host = attribute.RegisterHost, Port = attribute.RegisterPort }, IsCheck = attribute.IsRegisterCheckHost, Name = attribute.ServiceName, IsSingle = attribute.IsSingleRegister, IsPerp = attribute.IsPerpleRegister }).Value;
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
                finally { Monitor.Exit(clientLock); }
                if (result.State == registerState.Success)
                {
                    attribute.RegisterHost = result.Service.Host.Host;
                    attribute.RegisterPort = result.Service.Host.Port;
                }
#endif
                return result.State;
            }
            /// <summary>
            /// 删除注册TCP服务端
            /// </summary>
            /// <param name="attribute">TCP服务配置</param>
            public void RemoveRegister(fastCSharp.code.cSharp.tcpServer attribute)
            {
#if NotFastCSharpCode
                fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.NotFastCSharpCode);
#else
                Monitor.Enter(clientLock);
                try
                {
                    registerClient.removeRegister(clientId, attribute.ServiceName);
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
                finally { Monitor.Exit(clientLock); }
#endif
            }
            /// <summary>
            /// 绑定TCP调用客户端
            /// </summary>
            /// <param name="commandClient">TCP调用客户端</param>
            public void Register(commandClient commandClient)
            {
                hashString name = commandClient.ServiceName;
                Monitor.Enter(servicesLock);
                try
                {
                    if (!this.services.TryGetValue(name, out commandClient.TcpRegisterServices))
                    {
                        services.Add(name, commandClient.TcpRegisterServices = new services { Hosts = nullValue<host>.Array });
                    }
                    commandClient.TcpRegisterServices.AddClient(commandClient);
                }
                finally { Monitor.Exit(servicesLock); }
            }
            /// <summary>
            /// 删除TCP调用客户端
            /// </summary>
            /// <param name="commandClient">TCP调用客户端</param>
            internal void Remove(commandClient commandClient)
            {
                services services = commandClient.TcpRegisterServices;
                if (services != null)
                {
                    commandClient.TcpRegisterServices = null;
                    Monitor.Enter(servicesLock);
                    services.RemoveClient(commandClient);
                    Monitor.Exit(servicesLock);
                }
            }
            /// <summary>
            /// 获取TCP服务端口信息
            /// </summary>
            /// <param name="commandClient">TCP调用客户端</param>
            /// <returns>TCP服务端口信息是否更新</returns>
            internal bool GetHost(commandClient commandClient)
            {
                services services = commandClient.TcpRegisterServices;
                if (services == null)
                {
                    fastCSharp.code.cSharp.tcpServer attribute = commandClient.Attribute;
                    attribute.Port = 0;
                    return false;
                }
                Monitor.Enter(servicesLock);
                bool isHost = services.GetHost(commandClient);
                Monitor.Exit(servicesLock);
                return isHost;
            }
            /// <summary>
            /// TCP注册服务客户端缓存
            /// </summary>
            private static Dictionary<hashString, client> clients = dictionary.CreateHashString<client>();
            /// <summary>
            /// TCP注册服务客户端 访问锁
            /// </summary>
            private static readonly object clientsLock = new object();
            /// <summary>
            /// 关闭TCP注册服务客户端
            /// </summary>
            internal static void DisposeClients()
            {
                client[] clientArray = null;
                Monitor.Enter(clientsLock);
                try
                {
                    clientArray = clients.Values.getArray();
                    clients = null;
                }
                finally { Monitor.Exit(clientsLock); }
                foreach (client client in clientArray) client.Dispose();
            }
            /// <summary>
            /// 获取TCP注册服务客户端
            /// </summary>
            /// <param name="serviceName">服务名称</param>
            /// <returns>TCP注册服务客户端,失败返回null</returns>
            public static client Get(string serviceName)
            {
                if (!string.IsNullOrEmpty(serviceName))
                {
                    int count = int.MinValue;
                    client client = null;
                    hashString nameKey = serviceName;
                    Monitor.Enter(clientsLock);
                    try
                    {
                        if (clients != null && !clients.TryGetValue(nameKey, out client))
                        {
                            try
                            {
                                client = new client(serviceName);
                            }
                            catch (Exception error)
                            {
                                fastCSharp.log.Error.Add(error, null, false);
                            }
                            if (client != null)
                            {
                                count = clients.Count;
                                clients.Add(nameKey, client);
                            }
                        }
                    }
                    finally { Monitor.Exit(clientsLock); }
                    if (count == 0) fastCSharp.domainUnload.Add(null, domainUnload.unloadType.TcpRegisterClientDispose);
                    return client;
                }
                return null;
            }
        }


        /// <summary>
        /// 设置TCP服务端
        /// </summary>
        /// <param name="tcpServer">TCP服务端</param>
        public override void SetCommandServer(fastCSharp.net.tcp.commandServer tcpServer)
        {
            base.SetCommandServer(tcpServer);
            cacheFile = fastCSharp.config.pub.Default.CachePath + tcpServer.ServiceName + @".cache";
            fromCacheFile();

            //onRegisterHandle = onRegister;
            //onRegistersHandle = onRegister;
            //removeRegisterHandle = removeRegister;
            //fromCacheFile();
        }
        /// <summary>
        /// 是否保存缓存文件
        /// </summary>
        private int isSaveCacheFile;
        /// <summary>
        /// 缓存文件名称
        /// </summary>
        private string cacheFile;
        /// <summary>
        /// 缓存信息
        /// </summary>
        [fastCSharp.emit.dataSerialize(IsReferenceMember = false, IsMemberMap = false)]
        private struct cache
        {
            /// <summary>
            /// TCP服务信息集合
            /// </summary>
            public services[] Services;
            /// <summary>
            /// TCP服务端口信息集合
            /// </summary>
            public keyValue<string, int>[] HostPorts;
        }
        /// <summary>
        /// 保存TCP服务信息集合到缓存文件
        /// </summary>
        internal unsafe void SaveCacheFile()
        {
            cache cache = new cache();
            byte[] buffer = fastCSharp.memoryPool.StreamBuffers.Get();
            try
            {
                fixed (byte* bufferFixed = buffer)
                {
                    using (unmanagedStream stream = new unmanagedStream(bufferFixed, buffer.Length))
                    {
                        Monitor.Enter(serviceLock);
                        try
                        {
                            cache.Services = serviceCache.Values.getArray();
                            cache.HostPorts = hostPorts.getArray(value => new keyValue<string, int>(value.Key.ToString(), value.Value));
                            fastCSharp.emit.dataSerializer.Serialize(cache, stream);
                            if (stream.data.data == bufferFixed)
                            {
                                using (FileStream file = new FileStream(cacheFile, FileMode.Create, FileAccess.Write, FileShare.None))
                                {
                                    file.Write(buffer, 0, stream.Length);
                                }
                            }
                            else File.WriteAllBytes(cacheFile, stream.GetArray());
                        }
                        finally
                        {
                            isSaveCacheFile = 0;
                            Monitor.Exit(serviceLock);
                        }
                    }
                }
            }
            finally { fastCSharp.memoryPool.StreamBuffers.PushNotNull(buffer); }
        }
        /// <summary>
        /// 从缓存文件恢复TCP服务信息集合
        /// </summary>
        private void fromCacheFile()
        {
            cache cache = new cache();
            if (File.Exists(cacheFile))
            {
                int isCache = 0;
                try
                {
                    if (fastCSharp.emit.dataDeSerializer.DeSerialize(File.ReadAllBytes(cacheFile), ref cache)) isCache = 1;
                    else cache = new cache();
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
                if (isCache == 0)
                {
                    try
                    {
                        File.Delete(cacheFile);
                    }
                    catch (Exception error)
                    {
                        fastCSharp.log.Default.Add(error, null, false);
                    }
                }
            }
            Monitor.Enter(serviceLock);
            try
            {
                hostPorts = dictionary.CreateHashString<int>();
                serviceCache = dictionary.CreateHashString<services>();
                hostClients = dictionary<host>.Create<indexIdentity>();
                foreach (services value in cache.Services.notNull()) serviceCache.Add(value.Name, value);
                foreach (keyValue<string, int> value in cache.HostPorts.notNull()) hostPorts.Add(value.Key, value.Value);
            }
            finally { Monitor.Exit(serviceLock); }
        }

        /// <summary>
        /// TCP服务端口信息集合
        /// </summary>
        private Dictionary<hashString, int> hostPorts;
        /// <summary>
        /// TCP服务端信息集合
        /// </summary>
        private Dictionary<host, indexIdentity> hostClients;
        /// <summary>
        /// 日志流
        /// </summary>
        private logStream<log> logStream = new logStream<log>(fastCSharp.config.tcpRegister.Default.LogStreamSize);
        /// <summary>
        /// 客户端信息池
        /// </summary>
        private indexValuePool<clientIdentity> clientPool = new indexValuePool<clientIdentity>(fastCSharp.config.tcpRegister.Default.LogStreamSize);
        /// <summary>
        /// TCP服务端注册
        /// </summary>
        /// <returns>TCP服务端标识</returns>
        internal clientId Register()
        {
            clientInfo clientInfo = new clientInfo();
            indexIdentity identity;
            if (clientPool.Enter())
            {
                try
                {
                    identity.Index = clientPool.GetIndexContinue();//不能写成一行，可能造成Pool先入栈然后被修改，导致索引溢出
                    identity.Identity = clientPool.Pool[identity.Index].Set(clientInfo);
                }
                finally { clientPool.Exit(); }
                return new clientId { Tick = logStream.Ticks, Identity = identity };
            }
            clientPool.Exit();
            return default(clientId);
        }
        /// <summary>
        /// TCP服务信息 访问锁
        /// </summary>
        private readonly object serviceLock = new object();
        /// <summary>
        /// TCP服务信息集合
        /// </summary>
        private Dictionary<hashString, services> serviceCache;
        /// <summary>
        /// 获取TCP服务信息集合
        /// </summary>
        /// <param name="logIdentity">日志流编号</param>
        /// <returns>TCP服务信息集合</returns>
        internal services[] GetServices(out int logIdentity)
        {
            Monitor.Enter(serviceLock);
            try
            {
                do
                {
                    logIdentity = logStream.EndIdentity;
                    services[] services = serviceCache.Values.getArray();
                    interlocked.CompareSetYield(ref logStream.Lock);
                    if (logIdentity == logStream.EndIdentity)
                    {
                        logStream.Lock = 0;
                        return services;
                    }
                    logStream.Lock = 0;
                }
                while (true);
            }
            finally { Monitor.Exit(serviceLock); }
        }
        /// <summary>
        /// 获取客户端
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        private clientInfo getClient(indexIdentity identity)
        {
            if (clientPool.Enter())
            {
                clientInfo client = clientPool.Pool[identity.Index].Get(identity.Identity);
                clientPool.Exit();
                return client;
            }
            return null;
        }
        /// <summary>
        /// 获取客户端
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        private clientInfo getClient(ref clientId clientId)
        {
            return clientId.Tick == logStream.Ticks ? getClient(clientId.Identity) : null;
        }
        /// <summary>
        /// TCP服务端轮询
        /// </summary>
        /// <param name="clientId">TCP服务端标识</param>
        /// <param name="logIdentity">日志编号</param>
        /// <param name="onLog">TCP服务注册通知委托</param>
        internal void GetLog(ref clientId clientId, int logIdentity, Func<fastCSharp.net.returnValue<log>, bool> onLog)
        {
            clientInfo client = getClient(ref clientId);
            if (client != null)
            {
                onLog = Interlocked.Exchange(ref client.OnLog, onLog);
                if (onLog == null) logStream.Get(logIdentity, client.OnLogHandle);
                else onLog(log.NewGetter);
                return;
            }
            onLog(log.ClientError);
        }

        /// <summary>
        /// 注册TCP服务信息
        /// </summary>
        /// <param name="clientId">TCP服务端标识</param>
        /// <param name="service">TCP服务信息</param>
        /// <returns>注册状态</returns>
        [fastCSharp.code.cSharp.tcpMethod(IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private registerResult register(clientId clientId, service service)
        {
            if (getClient(ref clientId) == null) return new registerResult { State = registerState.ClientError };
            if (!service.Host.HostToIpAddress()) return new registerResult { State = registerState.HostError };
            services services;
            hashString serviceName = service.Name;
            Monitor.Enter(serviceLock);
            try
            {
                if (serviceCache.TryGetValue(serviceName, out services)) return register(clientId.Identity, service, services, ref serviceName);
                getPort(clientId.Identity, ref service.Host, true);
                if (service.Host.Port == 0) return new registerResult { State = registerState.PortError };
                if (service.IsCheck && hostClients.ContainsKey(service.Host)) return new registerResult { State = registerState.HostExists };
                hostClients[service.Host] = clientId.Identity;
                services = new services { Name = service.Name, Hosts = new host[] { service.Host }, IsSingle = service.IsSingle };
                serviceCache.Add(serviceName, services);
                appendLog(new log { Type = log.type.HostChanged, Services = new services { Name = service.Name, Hosts = new host[] { service.Host } } });
            }
            finally { Monitor.Exit(serviceLock); }
            return new registerResult { State = registerState.Success, Service = service };
        }
        /// <summary>
        /// 注册TCP服务信息
        /// </summary>
        /// <param name="identity">TCP服务端标识</param>
        /// <param name="service">TCP服务信息</param>
        /// <param name="services">TCP服务信息集合</param>
        /// <param name="serviceName">TCP服务名称标识</param>
        /// <returns>注册状态</returns>
        private registerResult register(indexIdentity identity, service service, services services, ref hashString serviceName)
        {
            int hostCount = 0;
            if (services.IsSingle || service.IsSingle)
            {
                foreach (host host in services.Hosts)
                {
                    indexIdentity oldClientIdentity;
                    if (hostClients.TryGetValue(host, out oldClientIdentity))
                    {
                        clientInfo oldClient = getClient(oldClientIdentity);
                        if (oldClient != null && oldClient.OnLog != null && oldClient.OnLog(log.Check)) services.Hosts[hostCount++] = host;
                        else hostClients.Remove(host);
                    }
                }
                if (hostCount != 0)
                {
                    if (hostCount != services.Hosts.Length)
                    {
                        Array.Resize(ref services.Hosts, hostCount);
                        appendLog(new log { Type = log.type.HostChanged, Services = new services { Name = service.Name, Hosts = services.Hosts.copy() } });
                    }
                    if (service.IsPerp)
                    {
                        getPort(identity, ref service.Host, true);
                        if (service.Host.Port == 0) return new registerResult { State = registerState.PortError };
                        services.SetPerpService(identity, service);
                        return new registerResult { State = registerState.Success, Service = service };
                    }
                    return new registerResult { State = registerState.SingleError };
                }
                services.IsSingle = false;
                services.Hosts = nullValue<host>.Array;
                getPort(identity, ref service.Host, true);
                if (service.Host.Port == 0 || (service.IsCheck && hostClients.ContainsKey(service.Host)))
                {
                    serviceCache.Remove(serviceName);
                    appendLog(new log { Type = log.type.RemoveServiceName, Services = new services { Name = service.Name } });
                    return new registerResult { State = service.Host.Port == 0 ? registerState.PortError : registerState.HostExists };
                }
                hostClients[service.Host] = identity;
                services.Hosts = new host[] { service.Host };
                services.IsSingle = service.IsSingle;
                appendLog(new log { Type = log.type.HostChanged, Services = new services { Name = service.Name, Hosts = new host[] { service.Host } } });
            }
            else
            {
                getPort(identity, ref service.Host, true);
                if (service.Host.Port == 0) return new registerResult { State = registerState.PortError };
                if (service.IsCheck && hostClients.ContainsKey(service.Host)) return new registerResult { State = registerState.HostExists };
                hostClients[service.Host] = identity;
                host[] hosts = new host[services.Hosts.Length + 1];
                Array.Copy(services.Hosts, 0, hosts, 1, services.Hosts.Length);
                hosts[0] = service.Host;
                services.Hosts = hosts;
                appendLog(new log { Type = log.type.HostChanged, Services = new services { Name = service.Name, Hosts = services.Hosts.copy() } });
            }
            return new registerResult { State = registerState.Success, Service = service };
        }
        /// <summary>
        /// 获取TCP服务端口号
        /// </summary>
        /// <param name="identity">TCP服务端标识</param>
        /// <param name="host">TCP服务端口信息</param>
        /// <param name="isPerp"></param>
        private void getPort(indexIdentity identity, ref host host, bool isPerp)
        {
            if (host.Port == 0)
            {
                hashString ipKey = host.Host;
                if (!hostPorts.TryGetValue(ipKey, out host.Port)) host.Port = fastCSharp.config.tcpRegister.Default.PortStart;
                int startPort = host.Port;
                while (hostClients.ContainsKey(host)) ++host.Port;
                if (host.Port >= 65536)
                {
                    host.Port = fastCSharp.config.tcpRegister.Default.PortStart;
                    while (host.Port != startPort && hostClients.ContainsKey(host)) ++host.Port;
                    if (host.Port == startPort)
                    {
                        host.Port = 0;
                        return;
                    }
                }
                hostPorts[ipKey] = host.Port + 1;
                if (!isPerp) hostClients.Add(host, identity);
            }
        }
        /// <summary>
        /// 注销TCP服务信息
        /// </summary>
        /// <param name="clientId">TCP服务端标识</param>
        /// <param name="serviceName">TCP服务名称</param>
        [fastCSharp.code.cSharp.tcpMethod(IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private void removeRegister(clientId clientId, string serviceName)
        {
            if (getClient(ref clientId) != null)
            {
                services services;
                hashString nameKey = serviceName;
                bool isRemove = false;
                Monitor.Enter(serviceLock);
                try
                {
                    if (serviceCache.TryGetValue(nameKey, out services))
                    {
                        isRemove = removeRegister(clientId.Identity, services, ref nameKey);
                    }
                }
                finally { Monitor.Exit(serviceLock); }
                if (isRemove) fastCSharp.log.Default.Add("TCP服务 " + serviceName + " 被注销", new System.Diagnostics.StackFrame(), false);
            }
        }
        /// <summary>
        /// 注销TCP服务信息
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="services"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        private bool removeRegister(indexIdentity identity, services services, ref hashString serviceName)
        {
            services.RemovePerpService(identity);
            if (removeRegister(identity, services))
            {
                if (services.Hosts.Length == 0)
                {
                    services.IsSingle = false;
                    foreach (keyValue<indexIdentity, service> perpService in services.GetPerpServices())
                    {
                        if (register(perpService.Key, perpService.Value, services, ref serviceName).State != registerState.Success) break;
                    }
                    if (services.Hosts.Length == 0)
                    {
                        serviceCache.Remove(serviceName);
                        appendLog(new log { Type = log.type.RemoveServiceName, Services = new services { Name = services.Name } });
                    }
                    return true;
                }
                else appendLog(new log { Type = log.type.HostChanged, Services = new services { Name = services.Name, Hosts = services.Hosts.copy() } });
            }
            return false;
        }
        /// <summary>
        /// 注销TCP服务信息
        /// </summary>
        /// <param name="identity">TCP服务端标识</param>
        /// <param name="service">TCP服务信息</param>
        /// <returns>TCP服务端口信息集合信息是否被修改</returns>
        private unsafe bool removeRegister(indexIdentity identity, services service)
        {
            int count = (service.Hosts.Length + 7) >> 3, index = 0;
            byte* isRemove = stackalloc byte[count];
            fixedMap removeMap = new fixedMap(isRemove, count);
            count = 0;
            indexIdentity hostIdentity;
            foreach (host host in service.Hosts)
            {
                if (hostClients.TryGetValue(host, out hostIdentity) && hostIdentity.Equals(identity) == 0) removeMap.Set(index);
                else ++count;
                ++index;
            }
            if (count == service.Hosts.Length) return false;
            hashString serviceName = service.Name;
            if (count == 0)
            {
                foreach (host host in service.Hosts) hostClients.Remove(host);
                service.Hosts = nullValue<host>.Array;
                return true;
            }
            host[] hosts = new host[count];
            count = index = 0;
            foreach (host host in service.Hosts)
            {
                if (removeMap.Get(index++)) hostClients.Remove(host);
                else hosts[count++] = host;
            }
            service.Hosts = hosts;
            serviceCache[serviceName] = service;
            return true;
        }
        /// <summary>
        /// 注销TCP服务信息
        /// </summary>
        /// <param name="clientId">TCP服务端标识</param>
        [fastCSharp.code.cSharp.tcpMethod(IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private void removeRegister(clientId clientId)
        {
            if (clientId.Tick == logStream.Ticks && clientPool.Enter())
            {
                if (clientPool.Pool[clientId.Identity.Index].Free(clientId.Identity.Identity))
                {
                    clientPool.FreeExit(clientId.Identity.Index);
                    subArray<string> removeNames = default(subArray<string>);
                    Monitor.Enter(serviceLock);
                    try
                    {
                        foreach (services service in serviceCache.Values.getArray())
                        {
                            hashString serviceName = service.Name;
                            if (removeRegister(clientId.Identity, service, ref serviceName)) removeNames.Add(service.Name);
                        }
                    }
                    finally { Monitor.Exit(serviceLock); }
                    if (removeNames.length != 0) fastCSharp.log.Default.Add("TCP服务 " + removeNames.ToArray().joinString(',') + " 被注销", null, false);
                }
                else clientPool.Exit();
            }
        }
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="log"></param>
        private void appendLog(log log)
        {
            //if (log.Services == null) Console.WriteLine(log.Type.ToString());
            //else if (log.Services.Hosts == null) Console.WriteLine(log.Type.ToString() + "[" + log.Services.Name + "]");
            //else Console.WriteLine(log.Type.ToString() + "[" + log.Services.Name + "] " + log.Services.Hosts.ToJson());
            logStream.Append(log);
            if (isSaveCacheFile == 0)
            {
                isSaveCacheFile = 1;
                fastCSharp.threading.task.Tiny.Add(this, thread.callType.TcpRegisterSaveCacheFile);
            }
        }
    }
}
