using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Net;
using System.Diagnostics;
using fastCSharp.code;
using fastCSharp.threading;
using fastCSharp.io;
using fastCSharp.diagnostics;
using System.Runtime.CompilerServices;

namespace fastCSharp.net.tcp.http
{
    /// <summary>
    /// HTTP服务器
    /// </summary>
#if NotFastCSharpCode
    [fastCSharp.code.cSharp.tcpServer(Service = servers.ServiceName, IsIdentityCommand = true, Host = "127.0.0.1")]
#else
    [fastCSharp.code.cSharp.tcpServer(Service = servers.ServiceName, IsIdentityCommand = true, Host = "127.0.0.1", VerifyMethodType = typeof(servers.tcpClient.timeVerifyMethod))]
#endif
    public partial class servers : timeVerifyServer, IDisposable
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        internal const string ServiceName = "httpServer";
        /// <summary>
        /// HTTP服务启动状态
        /// </summary>
        public enum startState
        {
            /// <summary>
            /// 未知状态
            /// </summary>
            Unknown,
            /// <summary>
            /// HTTP服务已经关闭
            /// </summary>
            Disposed,
            /// <summary>
            /// 主机名称合法
            /// </summary>
            HostError,
            /// <summary>
            /// 域名不合法
            /// </summary>
            DomainError,
            /// <summary>
            /// 域名冲突
            /// </summary>
            DomainExists,
            /// <summary>
            /// 安全连接匹配错误
            /// </summary>
            SslMatchError,
            /// <summary>
            /// 服务创建失败
            /// </summary>
            CreateServerError,
            /// <summary>
            /// 安全证书获取失败
            /// </summary>
            CertificateError,
            /// <summary>
            /// 程序集文件未找到
            /// </summary>
            NotFoundAssembly,
            /// <summary>
            /// 服务启动失败
            /// </summary>
            StartError,
            /// <summary>
            /// TCP监听服务启动失败
            /// </summary>
            TcpError,
            /// <summary>
            /// 启动成功
            /// </summary>
            Success,
        }
        /// <summary>
        /// 保存信息
        /// </summary>
        [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
        private sealed class saveInfo
        {
            /// <summary>
            /// 域名服务信息
            /// </summary>
            public sealed class domainServer
            {
                /// <summary>
                /// 程序集文件名,包含路径
                /// </summary>
                public string AssemblyPath;
                /// <summary>
                /// 服务程序类型名称
                /// </summary>
                public string ServerType;
                /// <summary>
                /// 域名信息集合
                /// </summary>
                public domain[] Domains;
                /// <summary>
                /// 是否共享程序集
                /// </summary>
                public bool IsShareAssembly;
            }
            /// <summary>
            /// 域名服务信息集合
            /// </summary>
            [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
            public domainServer[] Domains;
            /// <summary>
            /// 转发服务端口信息
            /// </summary>
            public host ForwardHost;
        }
        /// <summary>
        /// 域名服务信息
        /// </summary>
        internal sealed class domainServer : IDisposable
        {
            /// <summary>
            /// HTTP服务器
            /// </summary>
            public servers Servers;
            /// <summary>
            /// 程序集文件名,包含路径
            /// </summary>
            public string AssemblyPath;
            /// <summary>
            /// 服务程序类型名称
            /// </summary>
            public string ServerType;
            /// <summary>
            /// 是否共享程序集
            /// </summary>
            public bool IsShareAssembly;
            /// <summary>
            /// 域名信息集合
            /// </summary>
            public keyValue<domain, int>[] Domains;
            /// <summary>
            /// 有效域名数量
            /// </summary>
            public int DomainCount;
            /// <summary>
            /// 域名服务
            /// </summary>
            public http.domainServer Server;
            /// <summary>
            /// 文件监视路径
            /// </summary>
            public string FileWatcherPath;
            /// <summary>
            /// 是否已经启动
            /// </summary>
            public bool IsStart;
            /// <summary>
            /// 删除文件监视路径
            /// </summary>
            public void RemoveFileWatcher()
            {
                createFlieTimeoutWatcher fileWatcher = Servers.fileWatcher;
                if (fileWatcher != null)
                {
                    string path = Interlocked.Exchange(ref FileWatcherPath, null);
                    if (path != null) fileWatcher.Remove(path);
                }
            }
            /// <summary>
            /// 停止监听
            /// </summary>
            public void StopListen()
            {
                if (Server != null) Server.StopListen();
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                RemoveFileWatcher();
                pub.Dispose(ref Server);
            }
        }
        /// <summary>
        /// 域名搜索
        /// </summary>
        private unsafe sealed class domainSearcher : IDisposable
        {
            /// <summary>
            /// 字节数组搜索器
            /// </summary>
            private struct searcher
            {
                /// <summary>
                /// 状态集合
                /// </summary>
                private byte* state;
                /// <summary>
                /// 字节查找表
                /// </summary>
                private byte* bytes;
                /// <summary>
                /// 当前状态
                /// </summary>
                private byte* currentState;
                /// <summary>
                /// 查询矩阵单位尺寸类型
                /// </summary>
                private byte tableType;
                /// <summary>
                /// ASCII字节搜索器
                /// </summary>
                /// <param name="data">数据起始位置</param>
                public searcher(ref pointer.size data)
                {
                    int stateCount = *data.Int;
                    currentState = state = data.Byte + sizeof(int);
                    bytes = state + stateCount * 3 * sizeof(int);
                    if (stateCount < 256) tableType = 0;
                    else if (stateCount < 65536) tableType = 1;
                    else tableType = 2;
                }
                /// <summary>
                /// 获取状态索引
                /// </summary>
                /// <param name="end">匹配起始位置</param>
                /// <param name="start">匹配结束位置</param>
                /// <returns>状态索引,失败返回-1</returns>
                internal int Search(byte* start, byte* end)
                {
                    int dotIndex = -1, value = 0;
                    currentState = state;
                    do
                    {
                        byte* prefix = currentState + *(int*)currentState;
                        int prefixSize = *(ushort*)(prefix - sizeof(ushort));
                        if (prefixSize != 0)
                        {
                            for (byte* endPrefix = prefix + prefixSize; prefix != endPrefix; ++prefix)
                            {
                                if (end == start) return dotIndex;
                                if ((uint)((value = *--end) - 'A') < 26) value |= 0x20;
                                if (value != *prefix) return dotIndex;
                            }
                        }
                        if (end == start) return *(int*)(currentState + sizeof(int) * 2);
                        if (value == '.' && (value = *(int*)(currentState + sizeof(int) * 2)) >= 0) dotIndex = value;
                        if (*(int*)(currentState + sizeof(int)) == 0) return dotIndex;
                        if ((uint)((value = *--end) - 'A') < 26) value |= 0x20;
                        int index = (int)*(bytes + value);
                        byte* table = currentState + *(int*)(currentState + sizeof(int));
                        if (tableType == 0)
                        {
                            if ((index = *(table + index)) == 0) return dotIndex;
                            currentState = state + index * 3 * sizeof(int);
                        }
                        else if (tableType == 1)
                        {
                            if ((index = (int)*(ushort*)(table + index * sizeof(ushort))) == 0) return dotIndex;
                            currentState = state + index * 3 * sizeof(int);
                        }
                        else
                        {
                            if ((index = *(int*)(table + index * sizeof(int))) == 0) return dotIndex;
                            currentState = state + index;
                        }
                    }
                    while (true);
                }
                ///// <summary>
                ///// 获取状态索引
                ///// </summary>
                ///// <param name="data">匹配状态</param>
                ///// <returns>状态索引,失败返回-1</returns>
                //[MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                //public int Search(ref subArray<byte> data)
                //{
                //    fixed (byte* dataFixed = data.array)
                //    {
                //        byte* start = dataFixed + data.StartIndex;
                //        return Search(start, start + data.Count);
                //    }
                //}
                /// <summary>
                /// 获取状态索引
                /// </summary>
                /// <param name="data">匹配状态</param>
                /// <returns>状态索引,失败返回-1</returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public int Search(byte[] data)
                {
                    fixed (byte* dataFixed = data) return Search(dataFixed, dataFixed + data.Length);
                }
            }
            /// <summary>
            /// 域名信息集合
            /// </summary>
            private byte[][] domains;
            /// <summary>
            /// 域名服务信息集合
            /// </summary>
            public domainServer[] Servers { get; private set; }
            /// <summary>
            /// 域名搜索数据
            /// </summary>
            private pointer.size data;
            /// <summary>
            /// 最后一次查询域名
            /// </summary>
            private pointer.size lastDomain;
            /// <summary>
            /// 最后一次查询域名服务信息
            /// </summary>
            private domainServer lastServer;
            /// <summary>
            /// 最后一次查询域名长度
            /// </summary>
            private int lastDomainSize;
            /// <summary>
            /// 最后一次查询访问锁
            /// </summary>
            private readonly object lastLock = new object();
            /// <summary>
            /// 域名搜索
            /// </summary>
            public domainSearcher() { }
            /// <summary>
            /// 域名搜索
            /// </summary>
            /// <param name="domains">域名信息集合</param>
            /// <param name="servers">域名服务信息集合</param>
            private domainSearcher(byte[][] domains, domainServer[] servers)
            {
                this.domains = domains;
                Servers = servers;
                data = fastCSharp.stateSearcher.byteArraySearcher.Create(domains, false);
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                unmanaged.Free(ref data);
                Monitor.Enter(lastLock);
                unmanaged.Free(ref lastDomain);
                Monitor.Exit(lastLock);
            }
            /// <summary>
            /// 获取域名服务信息
            /// </summary>
            /// <param name="domain">域名</param>
            /// <param name="size">域名长度</param>
            /// <returns>域名服务信息</returns>
            public domainServer Get(byte* domain, int size)
            {
                pointer.size data = this.data;
                if (data.Data != null)
                {
                    if (lastDomainSize == size && Monitor.TryEnter(lastLock))
                    {
                        if (lastDomainSize == size && unsafer.memory.SimpleEqual(lastDomain.Byte, domain, lastDomainSize))
                        {
                            domainServer server = lastServer;
                            Monitor.Exit(lastLock);
                            return server;
                        }
                        Monitor.Exit(lastLock);
                    }
                    int index = new searcher(ref data).Search(domain, domain + size);
                    if (index >= 0)
                    {
                        domainServer server = Servers[index];
                        if (Monitor.TryEnter(lastLock))
                        {
                            if (lastDomain.Byte == null)
                            {
                                try
                                {
                                    lastDomain = unmanaged.Get(maxDomainSize, false);
                                    unsafer.memory.SimpleCopy(domain, lastDomain.Byte, lastDomainSize = size);
                                    lastServer = server;
                                }
                                finally { Monitor.Exit(lastLock); }

                            }
                            else
                            {
                                unsafer.memory.SimpleCopy(domain, lastDomain.Byte, lastDomainSize = size);
                                lastServer = server;
                                Monitor.Exit(lastLock);
                            }
                        }
                        return server;
                    }
                }
                return null;
            }
            /// <summary>
            /// 添加域名服务信息
            /// </summary>
            /// <param name="domain"></param>
            /// <param name="server"></param>
            /// <param name="removeDomains">域名搜索</param>
            /// <returns></returns>
            public domainSearcher Add(byte[] domain, domainServer server, out domainSearcher removeDomains)
            {  
                byte[][] domains = this.domains;
                domainServer[] servers = Servers;
                pointer.size data = this.data;
                if (domain.Length != 0)
                {
                    fixed (byte* domainFixed = domain)
                    {
                        byte* domainEnd = domainFixed + domain.Length;
                        if ((data.Data == null || new searcher(ref data).Search(domainFixed, domainEnd) < 0))
                        {
                            byte[] reverseDomain = new byte[domain.Length];
                            fixed (byte* reverseDomainFixed = reverseDomain)
                            {
                                for (byte* start = domainFixed, write = reverseDomainFixed + domain.Length; start != domainEnd; *--write = *start++) ;
                            }
                            domainSearcher searcher = new domainSearcher(domains.getAdd(reverseDomain), servers.getAdd(server));
                            removeDomains = this;
                            return searcher;
                        }
                    }
                }
                removeDomains = null;
                return this;
            }
            /// <summary>
            /// 删除域名服务信息
            /// </summary>
            /// <param name="domain"></param>
            /// <param name="removeDomains">域名搜索</param>
            /// <returns></returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public domainSearcher Remove(byte[] domain, out domainSearcher removeDomains)
            {
                domainServer server;
                return Remove(domain, out removeDomains, out server);
            }
            /// <summary>
            /// 删除域名服务信息
            /// </summary>
            /// <param name="domain"></param>
            /// <param name="removeDomains">域名搜索</param>
            /// <param name="server">域名服务信息</param>
            /// <returns></returns>
            public domainSearcher Remove(byte[] domain, out domainSearcher removeDomains, out domainServer server)
            {
                byte[][] domains = this.domains;
                domainServer[] servers = Servers;
                pointer.size data = this.data;
                if (data.Data != null && domain.Length != 0)
                {
                    int index = new searcher(ref data).Search(domain);
                    if (index >= 0)
                    {
                        domainSearcher searcher = Default;
                        if (domains.Length != 1)
                        {
                            int length = domains.Length - 1;
                            byte[][] newDomains = new byte[length][];
                            domainServer[] newServers = new domainServer[length];
                            Array.Copy(domains, 0, newDomains, 0, index);
                            Array.Copy(servers, 0, newServers, 0, index);
                            Array.Copy(domains, index + 1, newDomains, index, length - index);
                            Array.Copy(servers, index + 1, newServers, index, length - index);
                            searcher = new domainSearcher(newDomains, newServers);
                        }
                        server = servers[index];
                        removeDomains = this;
                        return searcher;
                    }
                }
                server = null;
                removeDomains = null;
                return this;
            }
            /// <summary>
            /// 关闭所有域名服务
            /// </summary>
            public void Close()
            {
                foreach (domainServer domain in Servers) domain.Dispose();
            }
            /// <summary>
            /// 停止监听
            /// </summary>
            public void StopListen()
            {
                foreach (domainServer domain in Servers) domain.StopListen();
            }
            /// <summary>
            /// 默认空域名搜索
            /// </summary>
            public static readonly domainSearcher Default = new domainSearcher();
        }
        /// <summary>
        /// 最大域名字节长度
        /// </summary>
        private const int maxDomainSize = 256 + 8;
        /// <summary>
        /// 程序集信息缓存
        /// </summary>
        private static readonly Dictionary<hashString, Assembly> assemblyCache = dictionary.CreateHashString<Assembly>();
        /// <summary>
        /// 程序集信息访问锁
        /// </summary>
        private static readonly object assemblyLock = new object();
        /// <summary>
        /// 本地服务程序集运行目录
        /// </summary>
        private string serverPath = fastCSharp.config.web.Default.HttpServerPath;
        /// <summary>
        /// 域名搜索
        /// </summary>
        private domainSearcher domains = domainSearcher.Default;
        /// <summary>
        /// HTTP域名服务集合访问锁
        /// </summary>
        private readonly object domainLock = new object();
        /// <summary>
        /// TCP服务端口信息集合
        /// </summary>
        private Dictionary<host, server> hosts = dictionary.Create<host, server>();
        /// <summary>
        /// TCP服务端口信息集合访问锁
        /// </summary>
        private readonly object hostLock = new object();
        /// <summary>
        /// HTTP转发代理服务信息
        /// </summary>
        private fastCSharp.code.cSharp.tcpServer forwardHost;
        /// <summary>
        /// 是否已经释放资源
        /// </summary>
        private int isDisposed;
        /// <summary>
        /// TCP域名服务缓存文件名
        /// </summary>
        private string cacheFileName
        {
            get { return fastCSharp.config.pub.Default.CachePath + "httpServer_" + server.attribute.ServiceName + ".cache"; }
        }
        /// <summary>
        /// 文件监视器
        /// </summary>
        private createFlieTimeoutWatcher fileWatcher;
        /// <summary>
        /// 文件监视是否超时
        /// </summary>
        private int isFileWatcherTimeout;
        /// <summary>
        /// 缓存加载访问锁
        /// </summary>
        private int loadCacheLock;
        /// <summary>
        /// 是否加载缓存信息
        /// </summary>
        public bool IsLoadCache = true;
        /// <summary>
        /// 是否已经加载缓存信息
        /// </summary>
        public bool IsLoadedCache { get; private set; }
        /// <summary>
        /// HTTP头部是否解析未知名称
        /// </summary>
        public bool IsParseHeader;
        /// <summary>
        /// 缓存域名服务加载事件
        /// </summary>
        public event Action OnLoadCacheDomain;
        /// <summary>
        /// 本地服务
        /// </summary>
        internal http.domainServer LocalDomainServer;
        /// <summary>
        /// TCP服务调用配置
        /// </summary>
        internal code.cSharp.tcpServer Attribute
        {
            get { return server.attribute; }
        }
        /// <summary>
        /// 设置TCP服务端
        /// </summary>
        /// <param name="tcpServer">TCP服务端</param>
        public override void SetCommandServer(fastCSharp.net.tcp.commandServer tcpServer)
        {
            base.SetCommandServer(tcpServer);
            fileWatcher = new createFlieTimeoutWatcher(fastCSharp.config.processCopy.Default.CheckTimeoutSeconds, this, createFlieTimeoutWatcher.timeoutType.HttpServers, fastCSharp.diagnostics.processCopyServer.DefaultFileWatcherFilter);
            if (!fastCSharp.config.pub.Default.IsService && fastCSharp.config.processCopy.Default.WatcherPath != null)
            {
                try
                {
                    fileWatcher.Add(fastCSharp.config.processCopy.Default.WatcherPath);
                }
                catch (Exception error)
                {
                    log.Error.Add(error, fastCSharp.config.processCopy.Default.WatcherPath, false);
                }
            }
            if (IsLoadCache)
            {
                try
                {
                    string cacheFileName = this.cacheFileName;
                    if (File.Exists(cacheFileName))
                    {
                        interlocked.CompareSetSleep1(ref loadCacheLock);
                        try
                        {
                            if (!IsLoadedCache)
                            {
                                saveInfo saveInfo = fastCSharp.emit.dataDeSerializer.DeSerialize<saveInfo>(File.ReadAllBytes(cacheFileName));
                                if (saveInfo.ForwardHost.Port != 0) setForward(saveInfo.ForwardHost);
                                if (saveInfo.Domains.length() != 0)
                                {
                                    if (OnLoadCacheDomain != null) OnLoadCacheDomain();
                                    foreach (saveInfo.domainServer domain in saveInfo.Domains)
                                    {
                                        try
                                        {
                                            start(domain.AssemblyPath, domain.ServerType, domain.Domains, domain.IsShareAssembly);
                                        }
                                        catch (Exception error)
                                        {
                                            log.Error.Add(error, null, false);
                                        }
                                    }
                                }
                                IsLoadedCache = true;
                            }
                        }
                        finally { loadCacheLock = 0; }
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
            }
        }
        /// <summary>
        /// 启动本地服务
        /// </summary>
        /// <typeparam name="domainType"></typeparam>
        /// <param name="hosts"></param>
        /// <returns></returns>
        private startState start<domainType>(host[] hosts) where domainType : http.domainServer
        {
            return start<domainType>(hosts.getArray(host => new domain { Host = host }));
        }
        /// <summary>
        /// 启动本地服务
        /// </summary>
        /// <typeparam name="domainType"></typeparam>
        /// <param name="domains"></param>
        /// <returns></returns>
        private startState start<domainType>(domain[] domains) where domainType : http.domainServer
        {
            startState state = startState.StartError;
#if NotFastCSharpCode
            fastCSharp.log.Error.Throw(log.exceptionType.NotFastCSharpCode);
#else
            fastCSharp.net.tcp.commandServer tcpServer = new fastCSharp.net.tcp.http.servers.tcpServer(null, this);
            base.SetCommandServer(tcpServer);
            Assembly assembly = typeof(domainType).Assembly;
            LocalDomainServer = fastCSharp.emit.constructor<domainType>.New();
            LocalDomainServer.LoadCheckPath = getLoadCheckPath(new DirectoryInfo(fastCSharp.pub.ApplicationPath)).FullName;
            if (LocalDomainServer.Start(domains, null))
            {
                if ((state = start(domains)) == startState.Success)
                {
                    log.Default.Add(@"domain success
" + domains.joinString(@"
", domain => domain.DomainData.deSerialize() + (domain.Host.Host == null ? null : (" [" + domain.Host.Host + ":" + domain.Host.Port.toString() + "]")) + (domain.SslHost.Host == null ? null : (" [" + domain.SslHost.Host + ":" + domain.SslHost.Port.toString() + "]"))), new System.Diagnostics.StackFrame(), false);
                    return startState.Success;
                }
            }
#endif
            return state;
        }
        /// <summary>
        /// 保存域名服务器参数集合
        /// </summary>
        internal void Save()
        {
            try
            {
                saveInfo saveInfo = new saveInfo();
                if (forwardHost != null)
                {
                    saveInfo.ForwardHost.Host = forwardHost.Host;
                    saveInfo.ForwardHost.Port = forwardHost.Port;
                }
                Monitor.Enter(domainLock);
                try
                {
                    saveInfo.Domains = domains.Servers.getHash().getArray(domain => new saveInfo.domainServer
                    {
                        AssemblyPath = domain.AssemblyPath,
                        ServerType = domain.ServerType,
                        IsShareAssembly = domain.IsShareAssembly,
                        Domains = domain.Domains.getFindArray(value => value.Value == 0, value => value.Key)
                    }).getFindArray(value => value.Domains.length() != 0);
                    File.WriteAllBytes(cacheFileName, fastCSharp.emit.dataSerializer.Serialize(saveInfo));
                }
                finally { Monitor.Exit(domainLock); }
            }
            catch (Exception error)
            {
                log.Error.Add(error, null, false);
            }
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.Increment(ref isDisposed) == 1)
            {
                pub.Dispose(ref base.server);
                if (LocalDomainServer == null)
                {
                    Save();
                    forwardHost = null;

                    pub.Dispose(ref fileWatcher);

                    Monitor.Enter(domainLock);
                    domainSearcher domains = this.domains;
                    this.domains = domainSearcher.Default;
                    Monitor.Exit(domainLock);
                    domains.Close();
                    domains.Dispose();

                    server[] servers = null;
                    Monitor.Enter(hostLock);
                    try
                    {
                        servers = hosts.Values.getArray();
                        hosts = null;
                    }
                    finally
                    {
                        Monitor.Exit(hostLock);
                        if (servers != null) foreach (server server in servers) server.Dispose();
                    }
                }
                else pub.Dispose(ref LocalDomainServer);
            }
        }
        /// <summary>
        /// 域名状态检测
        /// </summary>
        /// <param name="domain">域名信息</param>
        /// <param name="server">域名服务</param>
        /// <returns>域名状态</returns>
        private startState checkDomain(ref domain domain, domainServer server)
        {
            byte[] domainData = domain.DomainData;
            if (domain.Host.Host == null)
            {
                if (domain.SslHost.Host == null)
                {
                    if (domainData.length() == 0)
                    {
                        Console.WriteLine("F");
                        return servers.startState.DomainError;
                    }
                    int portIndex = fastCSharp.memory.indexOf(domainData, (byte)':');
                    if (portIndex == -1) domain.Host.Set(domainData.deSerialize(), 80);
                    else if (portIndex == 0)
                    {
                        Console.WriteLine("E");
                        return servers.startState.DomainError;
                    }
                    else
                    {
                        string domainString = domainData.deSerialize();
                        if (!int.TryParse(domainString.Substring(portIndex + 1), out domain.Host.Port))
                        {
                            Console.WriteLine("D");
                            return servers.startState.DomainError;
                        }
                        domain.Host.Host = domainString.Substring(0, portIndex);
                    }
                    if (!domain.Host.HostToIpAddress())
                    {
                        Console.WriteLine("C");
                        return servers.startState.DomainError;
                    }
                }
                else
                {
                    if (domain.SslHost.Port == 0) domain.SslHost.Port = 443;
                    if (domainData.length() == 0)
                    {
                        if (domain.SslHost.Host.Length == 0) return servers.startState.HostError;
                        string host = domain.SslHost.Host;
                        if (domain.SslHost.Port != 443) host += ":" + domain.SslHost.Port.toString();
                        domain.DomainData = domainData = host.getBytes();
                        fastCSharp.log.Default.Add(domain.SslHost.Host + " 缺少指定域名", new System.Diagnostics.StackFrame(), false);
                    }
                    else if (domain.SslHost.Port != 443 && fastCSharp.memory.indexOf(domainData, (byte)':') == -1)
                    {
                        domain.DomainData = domainData = (domainData.deSerialize() + ":" + domain.SslHost.Port.toString()).getBytes();
                    }
                    if (!domain.SslHost.HostToIpAddress()) return servers.startState.HostError;
                    if (fastCSharp.config.http.Default.GetCertificate(ref domain.SslHost) == null) return servers.startState.CertificateError;
                }
            }
            else
            {
                if (domain.Host.Port == 0) domain.Host.Port = 80;
                if (domain.SslHost.Host != null && domain.SslHost.Port == 0) domain.SslHost.Port = 443;
                if (domainData.length() == 0)
                {
                    if (domain.Host.Host.Length == 0) return servers.startState.HostError;
                    string host = domain.Host.Host;
                    if (domain.Host.Port != 80) host += ":" + domain.Host.Port.toString();
                    domain.DomainData = domainData = host.getBytes();
                    fastCSharp.log.Default.Add(domain.Host.Host + " 缺少指定域名", new System.Diagnostics.StackFrame(), false);
                }
                else if (domain.SslHost.Host == null)
                {
                    if (domain.Host.Port != 80 && fastCSharp.memory.indexOf(domainData, (byte)':') == -1)
                    {
                        domain.DomainData = domainData = (domainData.deSerialize() + ":" + domain.Host.Port.toString()).getBytes();
                    }
                }
                else
                {
                    if (!domain.SslHost.HostToIpAddress()) return servers.startState.HostError;
                    if (fastCSharp.config.http.Default.GetCertificate(ref domain.SslHost) == null) return servers.startState.CertificateError;
                }
                if (!domain.Host.HostToIpAddress()) return servers.startState.HostError;
            }
            if (domainData.Length > maxDomainSize)
            {
                Console.WriteLine("B");
                return servers.startState.DomainError;
            }
            domainData.toLower();
            domainSearcher removeDomains = null;
            Monitor.Enter(domainLock);
            try
            {
                domains = domains.Add(domainData, server, out removeDomains);
            }
            finally
            {
                Monitor.Exit(domainLock);
                if (removeDomains != null) removeDomains.Dispose();
            }
            return removeDomains == null ? startState.DomainExists : servers.startState.Success;
        }
        /// <summary>
        /// 删除域名信息
        /// </summary>
        /// <param name="domain">域名信息</param>
        private void removeDomain(domain domain)
        {
            domainSearcher removeDomains = null;
            Monitor.Enter(domainLock);
            try
            {
                domains = domains.Remove(domain.DomainData, out removeDomains);
            }
            finally
            {
                Monitor.Exit(domainLock);
                if (removeDomains != null) removeDomains.Dispose();
            }
        }
        /// <summary>
        /// 启动域名服务
        /// </summary>
        /// <param name="assemblyPath">程序集文件名,包含路径</param>
        /// <param name="serverType">服务程序类型名称</param>
        /// <param name="domain">域名信息</param>
        /// <param name="isShareAssembly">是否共享程序集</param>
        /// <returns>域名服务启动状态</returns>
        [fastCSharp.code.cSharp.tcpMethod(IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private unsafe startState start(string assemblyPath, string serverType, domain domain, bool isShareAssembly)
        {
            return start(assemblyPath, serverType, new domain[] { domain }, isShareAssembly);
        }
        /// <summary>
        /// 启动域名服务
        /// </summary>
        /// <param name="assemblyPath">程序集文件名,包含路径</param>
        /// <param name="serverType">服务程序类型名称</param>
        /// <param name="domains">域名信息集合</param>
        /// <param name="isShareAssembly">是否共享程序集</param>
        /// <returns>域名服务启动状态</returns>
        [fastCSharp.code.cSharp.tcpMethod(IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private unsafe startState start(string assemblyPath, string serverType, domain[] domains, bool isShareAssembly)
        {
            if (isDisposed != 0) return startState.Disposed;
            if (domains.length() == 0) return startState.DomainError;
            FileInfo assemblyFile = new FileInfo(assemblyPath);
            if (!File.Exists(assemblyPath))
            {
                log.Error.Add("未找到程序集 " + assemblyPath, new System.Diagnostics.StackFrame(), false);
                return startState.NotFoundAssembly;
            }
            int domainCount = 0;
            startState state = startState.Unknown;
            domainServer server = new domainServer { AssemblyPath = assemblyPath, ServerType = serverType, Servers = this, IsShareAssembly = isShareAssembly };
            foreach (domain domain in domains)
            {
                if ((state = checkDomain(ref domains[domainCount], server)) != startState.Success) break;
                ++domainCount;
            }
            try
            {
                if (state == startState.Success)
                {
                    state = startState.StartError;
                    Assembly assembly = null;
                    DirectoryInfo directory = assemblyFile.Directory;
                    keyValue<domain, int>[] domainFlags = domains.getArray(value => new keyValue<domain, int>(value, 0));
                    hashString pathKey = assemblyPath;
                    Monitor.Enter(assemblyLock);
                    try
                    {
                        if (!isShareAssembly || !assemblyCache.TryGetValue(pathKey, out assembly))
                        {
                            string serverPath = this.serverPath + ((ulong)fastCSharp.pub.StartTime.Ticks).toHex16() + ((ulong)fastCSharp.pub.Identity).toHex16() + fastCSharp.directory.DirectorySeparator;
                            Directory.CreateDirectory(serverPath);
                            foreach (FileInfo file in directory.GetFiles()) file.CopyTo(serverPath + file.Name);
                            assembly = Assembly.LoadFrom(serverPath + assemblyFile.Name);
                            if (isShareAssembly) assemblyCache.Add(pathKey, assembly);
                        }
                    }
                    finally { Monitor.Exit(assemblyLock); }
                    server.Server = (http.domainServer)Activator.CreateInstance(assembly.GetType(serverType));
                    server.Server.LoadCheckPath = getLoadCheckPath(directory).FullName;
                    if (server.Server.Start(domains, server.RemoveFileWatcher))
                    {
                        if (fileWatcher != null) fileWatcher.Add(directory.FullName);
                        server.FileWatcherPath = directory.FullName;
                        if ((state = start(domains)) == startState.Success)
                        {
                            server.DomainCount = domains.Length;
                            server.Domains = domainFlags;
                            server.IsStart = true;
                            if (loadCacheLock == 0) fastCSharp.threading.task.Tiny.Add(this, thread.callType.HttpServersSave);
                            log.Default.Add(@"domain success
" + domains.joinString(@"
", domain => domain.DomainData.deSerialize() + (domain.Host.Host == null ? null : (" [" + domain.Host.Host + ":" + domain.Host.Port.toString() + "]")) + (domain.SslHost.Host == null ? null : (" [" + domain.SslHost.Host + ":" + domain.SslHost.Port.toString() + "]"))), new System.Diagnostics.StackFrame(), false);
                            return startState.Success;
                        }
                    }
                }
            }
            catch (Exception error)
            {
                log.Error.Add(error, null, false);
            }
            foreach (domain domain in domains)
            {
                if (domainCount-- == 0) break;
                removeDomain(domain);
            }
            server.Dispose();
            return state;
        }
        /// <summary>
        /// 获取域名服务加载检测路径
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        private static DirectoryInfo getLoadCheckPath(DirectoryInfo directory)
        {
            DirectoryInfo loadDirectory = directory;
            do
            {
                string loadPath = loadDirectory.Name.fileNameToLower();
#if MONO
                if (loadPath == "Release" || loadPath == "bin" || loadPath == "Debug")
#else
                if (loadPath == "release" || loadPath == "bin" || loadPath == "debug")
#endif
                {
                    loadDirectory = loadDirectory.Parent;
                    if (loadDirectory == null) return directory;
                }
                else return loadDirectory;
            }
            while (true);
        }
        /// <summary>
        /// 启动TCP服务
        /// </summary>
        /// <param name="domains">域名信息集合</param>
        /// <returns>HTTP服务启动状态</returns>
        private startState start(domain[] domains)
        {
            int hostCount = 0, startCount = 0, stopCount = 0;
            foreach (domain domain in domains)
            {
                if (!domain.IsOnlyHost)
                {
                    if (domain.Host.Host != null)
                    {
                        startState state = start(ref domain.Host);
                        if (state != startState.Success) break;
                    }
                    if (domain.SslHost.Host != null)
                    {
                        startState state = startSsl(ref domain.SslHost);
                        if (state != startState.Success)
                        {
                            if (domain.Host.Host != null) ++stopCount;
                            break;
                        }
                    }
                    ++startCount;
                }
                ++hostCount;
                ++stopCount;
            }
            if (startCount != 0 && hostCount == domains.Length) return startState.Success;
            foreach (domain domain in domains)
            {
                if (stopCount-- == 0) break;
                if (!domain.IsOnlyHost)
                {
                    stop(ref domain.Host);
                    stop(ref domain.SslHost);
                }
            }
            return startState.TcpError;
        }
        /// <summary>
        /// 启动TCP服务
        /// </summary>
        /// <param name="host">TCP服务端口信息</param>
        /// <returns>HTTP服务启动状态</returns>
        private startState start(ref host host)
        {
            startState state = startState.TcpError;
            server server = null;
            Monitor.Enter(hostLock);
            try
            {
                if (hosts.TryGetValue(host, out server))
                {
                    if (server.Certificate == null)
                    {
                        ++server.DomainCount;
                        return startState.Success;
                    }
                    server = null;
                    state = startState.SslMatchError;
                }
                else
                {
                    state = startState.CreateServerError;
                    server = new server(this, ref host);
                    state = startState.TcpError;
                    if (server.Start())
                    {
                        hosts.Add(host, server);
                        return startState.Success;
                    }
                }
            }
            catch (Exception error)
            {
                log.Error.Add(error, null, false);
            }
            finally { Monitor.Exit(hostLock); }
            pub.Dispose(ref server);
            return state;
        }
        /// <summary>
        /// 启动TCP服务
        /// </summary>
        /// <param name="host">TCP服务端口信息</param>
        /// <returns>HTTP服务启动状态</returns>
        private startState startSsl(ref host host)
        {
            startState state = startState.TcpError;
            server server = null;
            Monitor.Enter(hostLock);
            try
            {
                if (hosts.TryGetValue(host, out server))
                {
                    if (server.Certificate != null)
                    {
                        ++server.DomainCount;
                        return startState.Success;
                    }
                    server = null;
                    state = startState.SslMatchError;
                }
                else
                {
                    state = startState.CreateServerError;
                    server = new sslServer(this, ref host);
                    state = startState.TcpError;
                    if (server.Start())
                    {
                        hosts.Add(host, server);
                        return startState.Success;
                    }
                }
            }
            catch (Exception error)
            {
                log.Error.Add(error, null, false);
            }
            finally { Monitor.Exit(hostLock); }
            pub.Dispose(ref server);
            return state;
        }
        /// <summary>
        /// 停止域名服务
        /// </summary>
        /// <param name="domain">域名信息</param>
        [fastCSharp.code.cSharp.tcpMethod(IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private unsafe void stop(domain domain)
        {
            domainSearcher removeDomains = null;
            domainServer domainServer = null;
            byte[] domainData = domain.DomainData.toLower();
            Monitor.Enter(domainLock);
            try
            {
                domains = domains.Remove(domainData, out removeDomains, out domainServer);
            }
            finally
            {
                Monitor.Exit(domainLock);
                if (removeDomains != null) removeDomains.Dispose();
            }
            if (domainServer != null && domainServer.Domains != null)
            {
                for (int index = domainServer.Domains.Length; index != 0; )
                {
                    keyValue<domain, int> stopDomain = domainServer.Domains[--index];
                    if ((stopDomain.Value | (stopDomain.Key.DomainData.Length ^ domainData.Length)) == 0
                        && unsafer.memory.SimpleEqual(stopDomain.Key.DomainData, domainData, domainData.Length)
                        && Interlocked.CompareExchange(ref domainServer.Domains[index].Value, 1, 0) == 0)
                    {
                        if (!stopDomain.Key.IsOnlyHost)
                        {
                            stop(ref stopDomain.Key.Host);
                            stop(ref stopDomain.Key.SslHost);
                        }
                        if (Interlocked.Decrement(ref domainServer.DomainCount) == 0) domainServer.Dispose();
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// 停止域名服务
        /// </summary>
        /// <param name="domains">域名信息集合</param>
        [fastCSharp.code.cSharp.tcpMethod(IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private unsafe void stop(domain[] domains)
        {
            if (domains != null)
            {
                foreach (domain domain in domains) stop(domain);
            }
        }
        ///// <summary>
        ///// 停止域名服务
        ///// </summary>
        ///// <param name="domainServer">域名服务</param>
        //private unsafe void stop(domainServer domainServer)
        //{
        //    if (domainServer != null && domainServer.Domains != null)
        //    {
        //        try
        //        {
        //            for (int index = domainServer.Domains.Length; index != 0; )
        //            {
        //                if (Interlocked.CompareExchange(ref domainServer.Domains[--index].Value, 1, 0) == 0)
        //                {
        //                    stop(domainServer.Domains[index].Key);
        //                    Interlocked.Decrement(ref domainServer.DomainCount);
        //                }
        //            }
        //        }
        //        catch (Exception error)
        //        {
        //            log.Default.Add(error, null, false);
        //        }
        //        finally
        //        {
        //            domainServer.Dispose();
        //        }
        //    }
        //}
        /// <summary>
        /// 停止TCP服务
        /// </summary>
        /// <param name="host">TCP服务端口信息</param>
        private void stop(ref host host)
        {
            server server;
            Monitor.Enter(hostLock);
            try
            {
                if (hosts.TryGetValue(host, out server))
                {
                    if (--server.DomainCount == 0) hosts.Remove(host);
                    else server = null;
                }
            }
            finally { Monitor.Exit(hostLock); }
            if (server != null) server.Dispose();
        }
        /// <summary>
        /// 停止所有端口监听
        /// </summary>
        public void StopListen()
        {
            pub.Dispose(ref fileWatcher);
            Monitor.Enter(hostLock);
            try
            {
                foreach (server server in hosts.Values) server.StopListen();
            }
            catch (Exception error)
            {
                log.Error.Add(error, null, false);
            }
            finally { Monitor.Exit(hostLock); }
            try
            {
                domains.StopListen();
            }
            catch (Exception error)
            {
                log.Error.Add(error, null, false);
            }
        }
        /// <summary>
        /// 文件监视超时处理
        /// </summary>
        internal void OnFileWatcherTimeout()
        {
            if (Interlocked.CompareExchange(ref isFileWatcherTimeout, 1, 0) == 0)
            {
                using (Process process = Process.GetCurrentProcess())
                {
                    FileInfo file = new FileInfo(process.MainModule.FileName);
                    if (fastCSharp.config.processCopy.Default.WatcherPath == null)
                    {
                        ProcessStartInfo info = new ProcessStartInfo(file.FullName, null);
                        info.UseShellExecute = true;
                        info.WorkingDirectory = file.DirectoryName;
                        using (Process newProcess = Process.Start(info)) Environment.Exit(-1);
                    }
                    else
                    {
                        processCopyServer.Remove();
                        FileWatcherTimeout();
                    }
                }
            }
        }
        /// <summary>
        /// 文件监视超时处理
        /// </summary>
        internal void FileWatcherTimeout()
        {
            if (processCopyServer.CopyStart())
            {
                Dispose();
                server.Dispose();
                Environment.Exit(-1);
            }
            else
            {
                timerTask.Default.Add(this, thread.callType.HttpServersFileWatcherTimeout, date.nowTime.Now.AddSeconds(fastCSharp.config.processCopy.Default.CheckTimeoutSeconds));
            }
        }
        /// <summary>
        /// 设置HTTP转发代理服务信息
        /// </summary>
        /// <param name="host">HTTP转发代理服务信息</param>
        /// <returns>是否设置成功</returns>
        [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private bool setForward(host host)
        {
            if (isDisposed == 0 && host.HostToIpAddress())
            {
                fastCSharp.code.cSharp.tcpServer tcpServer = new fastCSharp.code.cSharp.tcpServer { Host = host.Host, Port = host.Port };
                if (tcpServer.IpAddress != IPAddress.Any)
                {
                    forwardHost = tcpServer;
                    if (loadCacheLock == 0) fastCSharp.threading.task.Tiny.Add(this, thread.callType.HttpServersSave);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 清除HTTP转发代理服务信息
        /// </summary>
        [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private void removeForward()
        {
            forwardHost = null;
            fastCSharp.threading.task.Tiny.Add(this, thread.callType.HttpServersSave);
        }
        /// <summary>
        /// 获取HTTP转发代理服务客户端
        /// </summary>
        /// <returns>HTTP转发代理服务客户端,失败返回null</returns>
        internal virtual client GetForwardClient()
        {
            fastCSharp.code.cSharp.tcpServer host = forwardHost;
            if (host != null)
            {
                client client = new client(host, true);
                if (client.IsStart) return client;
                client.Dispose();
            }
            return null;
        }
        /// <summary>
        /// 获取域名服务信息
        /// </summary>
        /// <param name="domain">域名</param>
        /// <param name="size">域名长度</param>
        /// <returns>域名服务信息</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe http.domainServer GetServer(byte* domain, int size)
        {
            if (LocalDomainServer == null)
            {
                if (size > maxDomainSize || size == 0) return null;
                domainServer server = domains.Get(domain, size);
                return server != null && server.IsStart ? server.Server : null;
            }
            return LocalDomainServer;
        }
        /// <summary>
        /// 本地模式
        /// </summary>
        /// <typeparam name="domainServerType"></typeparam>
        /// <param name="hosts"></param>
        /// <returns></returns>
        public static servers Create<domainServerType>(params host[] hosts) where domainServerType : http.domainServer
        {
            fastCSharp.net.tcp.http.servers server = new fastCSharp.net.tcp.http.servers();
            startState state = startState.Unknown;
            try
            {
                if ((state = server.start<domainServerType>(hosts)) == startState.Success) return server;
                fastCSharp.log.Default.Add("HTTP服务启动失败 " + state.ToString(), null, false);
            }
            finally
            {
                if (state != startState.Success) server.Dispose();
            }
            return null;
        }
        /// <summary>
        /// 本地模式
        /// </summary>
        /// <typeparam name="domainServerType"></typeparam>
        /// <param name="hosts"></param>
        /// <returns></returns>
        public static servers Create<domainServerType>(params string[] hosts) where domainServerType : http.domainServer
        {
            return Create<domainServerType>(hosts.getArray(host => new host { Host = host, Port = 80 }));
        }
        /// <summary>
        /// 本地模式
        /// </summary>
        /// <typeparam name="domainServerType"></typeparam>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static servers Create<domainServerType>(string host, int port = 80) where domainServerType : http.domainServer
        {
            return Create<domainServerType>(new host { Host = host, Port = port });
        }
        /// <summary>
        /// 本地模式
        /// </summary>
        /// <param name="hosts"></param>
        /// <returns></returns>
        public static servers Create(params host[] hosts)
        {
            return Create<fastCSharp.net.tcp.http.domainServer.fileServer>(hosts);
        }
        /// <summary>
        /// 本地模式
        /// </summary>
        /// <param name="hosts"></param>
        /// <returns></returns>
        public static servers Create(params string[] hosts)
        {
            return Create<fastCSharp.net.tcp.http.domainServer.fileServer>(hosts.getArray(host => new host { Host = host, Port = 80 }));
        }
        /// <summary>
        /// 本地模式
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static servers Create(string host, int port = 80)
        {
            return Create<fastCSharp.net.tcp.http.domainServer.fileServer>(new host { Host = host, Port = port });
        }
    }
}
