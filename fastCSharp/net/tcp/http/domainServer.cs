using System;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using fastCSharp.io.compression;
using fastCSharp.web;
using fastCSharp.threading;
using fastCSharp.code.cSharp;
using System.Text;
using System.Runtime.CompilerServices;

namespace fastCSharp.net.tcp.http
{
    /// <summary>
    /// 域名服务
    /// </summary>
    public abstract class domainServer : IDisposable
    {
        /// <summary>
        /// 网站生成配置
        /// </summary>
        private sealed class nullWebConfig : fastCSharp.code.webConfig
        {
            /// <summary>
            /// 默认主域名
            /// </summary>
            public override string MainDomain
            {
                get { return null; }
            }
            /// <summary>
            /// 视图加载失败重定向
            /// </summary>
            public override string NoViewLocation
            {
                get { return null; }
            }
            /// <summary>
            /// 网站生成配置
            /// </summary>
            public static readonly nullWebConfig Default = new nullWebConfig();
        }
        /// <summary>
        /// 文件缓存
        /// </summary>
        protected sealed class fileCache : IDisposable
        {
            /// <summary>
            /// HTTP头部预留字节数
            /// </summary>
            public const int HttpHeaderSize = 256 + 64;
            /// <summary>
            /// 文件数据
            /// </summary>
            private subArray<byte> data;
            /// <summary>
            /// 文件数据
            /// </summary>
            public subArray<byte> Data
            {
                get
                {
                    wait();
                    return data;
                }
            }
            /// <summary>
            /// 文件HTTP响应输出
            /// </summary>
            private response response;
            /// <summary>
            /// 文件HTTP响应输出
            /// </summary>
            public response Response
            {
                get
                {
                    wait();
                    return response;
                }
            }
            /// <summary>
            /// 文件压缩数据
            /// </summary>
            private subArray<byte> gZipData;
            /// <summary>
            /// 文件数据
            /// </summary>
            public subArray<byte> GZipData
            {
                get
                {
                    wait();
                    return gZipData;
                }
            }
            /// <summary>
            /// 文件HTTP响应输出
            /// </summary>
            private response gZipResponse;
            /// <summary>
            /// 文件HTTP响应输出
            /// </summary>
            public response GZipResponse
            {
                get
                {
                    wait();
                    return gZipResponse;
                }
            }
            /// <summary>
            /// 最后修改时间
            /// </summary>
            internal byte[] lastModified;
            /// <summary>
            /// 最后修改时间
            /// </summary>
            public byte[] LastModified { get { return lastModified; } }
            /// <summary>
            /// HTTP响应输出内容类型
            /// </summary>
            public byte[] ContentType { get; internal set; }
            /// <summary>
            /// 是否已经获取数据
            /// </summary>
            internal int IsData;
            /// <summary>
            /// 数据加载访问锁
            /// </summary>
            private readonly object dataLock;
            /// <summary>
            /// 文件缓存
            /// </summary>
            internal fileCache()
            {
                Monitor.Enter(dataLock = new object());
            }
            /// <summary>
            /// 数据加载完毕
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void PulseAll()
            {
                IsData = 1;
                Monitor.PulseAll(dataLock);
                Monitor.Exit(dataLock);
            }
            /// <summary>
            /// 等待数据加载
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private void wait()
            {
                if (IsData == 0)
                {
                    Monitor.Enter(dataLock);
                    if (IsData == 0) Monitor.Wait(dataLock);
                    Monitor.Exit(dataLock);
                }
            }
            /// <summary>
            /// 是否HTML
            /// </summary>
            internal bool IsHtml;
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                date.ByteBuffers.Push(ref lastModified);
            }
            /// <summary>
            /// 文件缓存
            /// </summary>
            /// <param name="data">文件数据</param>
            /// <param name="contentType">HTTP响应输出内容类型</param>
            /// <param name="isGZip">是否压缩</param>
            /// <param name="lastModified">最后修改时间</param>
            /// <param name="isHtml">是否HTML文件</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void Set(ref subArray<byte> data, byte[] contentType, bool isGZip, byte[] lastModified, bool isHtml)
            {
                lastModified = date.CopyBytes(lastModified);
                Set(ref data, contentType, isGZip, isHtml);
            }
            /// <summary>
            /// 文件缓存
            /// </summary>
            /// <param name="data">文件数据</param>
            /// <param name="contentType">HTTP响应输出内容类型</param>
            /// <param name="isGZip">是否压缩</param>
            /// <param name="isHtml">是否HTML文件</param>
            internal void Set(ref subArray<byte> data, byte[] contentType, bool isGZip, bool isHtml)
            {
                ContentType = contentType;
                IsHtml = isHtml;
                try
                {
                    this.data = this.gZipData = data;
                    if (data.startIndex == fileCache.HttpHeaderSize)
                    {
                        response response = response.New();
                        response.State = http.response.state.Ok200;
                        response.SetBody(ref data, true, true);
                        response.CacheControl = isHtml ? response.ZeroAgeBytes : response.StaticFileCacheControl;
                        response.ContentType = contentType;
                        response.LastModified = lastModified;
                        this.response = gZipResponse = response;
                    }
                    if (isGZip)
                    {
                        subArray<byte> gZipData = response.GetCompress(ref data, null, data.startIndex);
                        if (gZipData.length != 0)
                        {
                            this.gZipData = gZipData;
                            response gZipResponse = response.New();
                            gZipResponse.State = http.response.state.Ok200;
                            gZipResponse.SetBody(ref gZipData, true, true);
                            response.CacheControl = isHtml ? response.ZeroAgeBytes : response.StaticFileCacheControl;
                            gZipResponse.ContentType = contentType;
                            gZipResponse.LastModified = lastModified;
                            gZipResponse.ContentEncoding = response.GZipEncoding;
                            this.gZipResponse = gZipResponse;
                        }
                    }
                }
                finally { PulseAll(); }
            }
            /// <summary>
            /// 文件数据字节数
            /// </summary>
            public int Size
            {
                get
                {
                    int size = data.length;
                    if (data.array != gZipData.array) size += gZipData.length;
                    return size;
                }
            }
        }
        /// <summary>
        /// 制定路径下的文件缓存
        /// </summary>
        private sealed class pathCache : IDisposable
        {
            /// <summary>
            /// 最大缓存字节数
            /// </summary>
            private long cacheSize;
            /// <summary>
            /// 当前缓存字节数
            /// </summary>
            private long currentCacheSize;
            /// <summary>
            /// 文件缓存队列
            /// </summary>
            private fifoPriorityQueue<hashBytes, fileCache> queue = new fifoPriorityQueue<hashBytes, fileCache>();
            /// <summary>
            /// 文件缓存队列访问锁
            /// </summary>
            private readonly object queueLock = new object();
            /// <summary>
            /// 最大文件缓存字节数
            /// </summary>
            private int fileSize;
            /// <summary>
            /// 域名服务引用数量
            /// </summary>
            private int serverCount;
            /// <summary>
            /// 添加域名服务
            /// </summary>
            /// <param name="server">域名服务</param>
            private void append(domainServer server)
            {
                ++serverCount;
                server.cache = this;
                if (server.cacheSize > cacheSize) cacheSize = server.cacheSize;
                if (server.fileSize > fileSize) fileSize = server.fileSize;
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                Monitor.Enter(queueLock);
                try
                {
                    while (queue.Count != 0) queue.UnsafePopValue().Dispose();
                }
                finally
                {
                    queue.Clear();
                    currentCacheSize = 0;
                    Monitor.Exit(queueLock); 
                }
            }
            /// <summary>
            /// 获取文件缓存
            /// </summary>
            /// <param name="path"></param>
            /// <returns>文件缓存</returns>
            public fileCache Get(ref hashBytes path)
            {
                fileCache fileCache;
                if (Monitor.TryEnter(queueLock))
                {
                    fileCache = queue.Get(ref path, null);
                    Monitor.Exit(queueLock);
                    return fileCache;
                }
                queue.TryGetOnlyCheck(ref path, out fileCache);
                return fileCache;
            }
            /// <summary>
            /// 获取文件缓存，失败时创建缓存对象
            /// </summary>
            /// <param name="path"></param>
            /// <param name="fileCache"></param>
            /// <returns></returns>
            public byte GetNew(ref hashBytes path, out fileCache fileCache)
            {
                byte isNewFileCache = 0;
                Monitor.Enter(queueLock);
                if ((fileCache = queue.Get(ref path, null)) == null)
                {
                    byte isLock = 0;
                    try
                    {
                        fileCache = new fileCache();
                        isLock = 1;
                        queue.Set(ref path, fileCache);
                        return isNewFileCache = 1;
                    }
                    finally
                    {
                        Monitor.Exit(queueLock);
                        if (isNewFileCache == 0 && isLock != 0) fileCache.PulseAll();
                    }
                }
                else Monitor.Exit(queueLock);
                return 0;
            }
            /// <summary>
            /// 设置文件缓存
            /// </summary>
            /// <param name="path"></param>
            /// <param name="fileCache"></param>
            public void Set(ref hashBytes path, fileCache fileCache)
            {
                long fileSize = fileCache.Size, minSize = cacheSize <= fileSize ? fileSize : cacheSize;
                Monitor.Enter(queueLock);
                try
                {
                    fileCache oldFileCache = queue.Set(ref path, fileCache);
                    currentCacheSize += fileSize;
                    if (oldFileCache == null)
                    {
                        while (currentCacheSize > minSize)
                        {
                            fileCache removeFileCache = queue.UnsafePopValue();
                            currentCacheSize -= removeFileCache.Size;
                            removeFileCache.Dispose();
                        }
                    }
                    else if (oldFileCache != fileCache)
                    {
                        currentCacheSize -= oldFileCache.Size;
                        oldFileCache.Dispose();
                    }
                }
                finally { Monitor.Exit(queueLock); }
            }
            /// <summary>
            /// 删除缓存
            /// </summary>
            /// <param name="path"></param>
            public void Remove(ref hashBytes path)
            {
                fileCache fileCache;
                Monitor.Enter(queueLock);
                try
                {
                    if (queue.Remove(ref path, out fileCache))
                    {
                        currentCacheSize -= fileCache.Size;
                        fileCache.Dispose();
                    }
                }
                finally { Monitor.Exit(queueLock); }
            }
            /// <summary>
            /// 删除新建的缓存
            /// </summary>
            /// <param name="path"></param>
            public void RemoveOnly(ref hashBytes path)
            {
                fileCache fileCache;
                Monitor.Enter(queueLock);
                try
                {
                    if (queue.Remove(ref path, out fileCache)) fileCache.Dispose();
                }
                finally { Monitor.Exit(queueLock); }
            }
            /// <summary>
            /// 缓存集合
            /// </summary>
            private static readonly Dictionary<string, pathCache> caches = dictionary.CreateOnly<string, pathCache>();
            /// <summary>
            /// 缓存集合访问锁
            /// </summary>
            private static readonly object cacheLock = new object();
            /// <summary>
            /// 获取文件缓存
            /// </summary>
            /// <param name="server">域名服务</param>
            /// <returns></returns>
            public static void Get(domainServer server)
            {
                pathCache cache;
                Monitor.Enter(cacheLock);
                try
                {
                    if (server.cache == null)
                    {
                        if (!caches.TryGetValue(server.WorkPath, out cache)) caches.Add(server.WorkPath, cache = new pathCache());
                        cache.append(server);
                    }
                }
                finally { Monitor.Exit(cacheLock); }

            }
            /// <summary>
            /// 释放文件缓存
            /// </summary>
            /// <param name="server"></param>
            public static void Free(domainServer server)
            {
                pathCache removeCache = null;
                Monitor.Enter(cacheLock);
                try
                {
                    pathCache cache = Interlocked.Exchange(ref server.cache, null);
                    if (cache != null)
                    {
                        if (--cache.serverCount == 0)
                        {
                            removeCache = cache;
                            caches.Remove(server.WorkPath);
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(cacheLock);
                    if (removeCache != null) removeCache.Dispose();
                }
            }
        }
        /// <summary>
        /// 内容类型
        /// </summary>
        protected static byte[] ContentTypeBytes { get { return header.ContentTypeBytes; } }
        /// <summary>
        /// 加载检测路径
        /// </summary>
        internal string LoadCheckPath;
        /// <summary>
        /// 文件路径
        /// </summary>
        protected virtual string path { get { return null; } }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string WorkPath { get; private set; }
        /// <summary>
        /// 最大缓存字节数(单位MB)
        /// </summary>
        protected virtual int maxCacheSize { get { return config.web.Default.MaxCacheSize; } }
        /// <summary>
        /// 最大缓存字节数
        /// </summary>
        private long cacheSize;
        /// <summary>
        /// 最大文件缓存字节数(单位KB)
        /// </summary>
        protected virtual int maxCacheFileSize { get { return config.web.Default.MaxCacheFileSize; } }
        /// <summary>
        /// 是否输出日期
        /// </summary>
        protected virtual bool isResponseDate { get { return true; } }
        /// <summary>
        /// 是否输出日期
        /// </summary>
        internal bool IsResponseDate;
        /// <summary>
        /// 是否输出服务器信息
        /// </summary>
        protected virtual bool isResponseServer { get { return true; } }
        /// <summary>
        /// 是否输出服务器信息
        /// </summary>
        internal bool IsResponseServer;
        /// <summary>
        /// 是否输出缓存参数
        /// </summary>
        protected virtual bool isResponseCacheControl { get { return true; } }
        /// <summary>
        /// 是否输出缓存参数
        /// </summary>
        internal bool IsResponseCacheControl;
        /// <summary>
        /// 输出内容类型
        /// </summary>
        protected virtual bool isResponseContentType { get { return true; } }
        /// <summary>
        /// 输出内容类型
        /// </summary>
        internal bool IsResponseContentType;
        /// <summary>
        /// 网站生成配置
        /// </summary>
        protected internal readonly fastCSharp.code.webConfig WebConfig;
        /// <summary>
        /// 输出编码
        /// </summary>
        internal readonly Encoding ResponseEncoding;
        /// <summary>
        /// 默认内容类型头部
        /// </summary>
        internal readonly byte[] HtmlContentType;
        /// <summary>
        /// 默认内容类型头部
        /// </summary>
        internal readonly byte[] JsContentType;
        /// <summary>
        /// 获取Session
        /// </summary>
        public ISession Session { get; protected set; }
        /// <summary>
        /// 最大文件缓存字节数
        /// </summary>
        private int fileSize;
        /// <summary>
        /// 客户端缓存时间(单位:秒)
        /// </summary>
        protected virtual int clientCacheSeconds { get { return config.web.Default.ClientCacheSeconds; } }
        /// <summary>
        /// 缓存控制参数
        /// </summary>
        protected byte[] cacheControl;
        /// <summary>
        /// 是否输出缓存控制参数
        /// </summary>
        protected virtual bool isCacheControl { get { return true; } }
        /// <summary>
        /// 文件缓存是否预留HTTP头部
        /// </summary>
        protected virtual bool isCacheHttpHeader { get { return false; } }
        /// <summary>
        /// 文件缓存是否预留HTTP头部
        /// </summary>
        private bool isCacheHeader;
        /// <summary>
        /// HTML文件缓存是否预留HTTP头部
        /// </summary>
        protected virtual bool isCacheHtmlHttpHeader { get { return false; } }
        /// <summary>
        /// HTML文件缓存是否预留HTTP头部
        /// </summary>
        private bool isCacheHtmlHeader;
        /// <summary>
        /// 域名信息集合
        /// </summary>
        private domain[] domains;
        /// <summary>
        /// 停止服务处理
        /// </summary>
        protected event Action onStop;
        /// <summary>
        /// 文件缓存
        /// </summary>
        private pathCache cache;
        ///// <summary>
        ///// 是否支持请求范围
        ///// </summary>
        //protected internal virtual bool isRequestRange { get { return false; } }
        /// <summary>
        /// 是否启动服务
        /// </summary>
        private int isStart;
        /// <summary>
        /// 是否停止服务
        /// </summary>
        private int isDisposed;
        /// <summary>
        ///错误输出数据
        /// </summary>
        protected keyValue<response, response>[] errorResponse;
        /// <summary>
        /// 域名服务
        /// </summary>
        protected domainServer()
        {
            WebConfig = getWebConfig() ?? nullWebConfig.Default;
            IsResponseDate = isResponseDate;
            IsResponseServer = isResponseServer;
            IsResponseCacheControl = isResponseCacheControl;
            IsResponseContentType = isResponseContentType;
            if (WebConfig != null) ResponseEncoding = WebConfig.Encoding;
            if (ResponseEncoding == null) ResponseEncoding = fastCSharp.config.appSetting.Encoding;
            if (ResponseEncoding.CodePage == fastCSharp.config.appSetting.Encoding.CodePage)
            {
                HtmlContentType = response.HtmlContentType;
                JsContentType = response.JsContentType;
            }
            else
            {
                HtmlContentType = ("text/html; charset=" + ResponseEncoding.WebName).getBytes();
                JsContentType = ("application/x-javascript; charset=" + ResponseEncoding.WebName).getBytes();
            }
        }
        /// <summary>
        /// 网站生成配置
        /// </summary>
        /// <returns>网站生成配置</returns>
        protected virtual fastCSharp.code.webConfig getWebConfig() { return null; }
        /// <summary>
        /// 启动HTTP服务
        /// </summary>
        /// <param name="domains">域名信息集合</param>
        /// <param name="onStop">停止服务处理</param>
        /// <returns>是否启动成功</returns>
        public abstract bool Start(domain[] domains, Action onStop);
        /// <summary>
        /// 创建错误输出数据
        /// </summary>
        protected unsafe virtual void createErrorResponse()
        {
            keyValue<response, response>[] errorResponse = new keyValue<response, response>[Enum.GetMaxValue<response.state>(-1) + 1];
            int isResponse = 0;
            try
            {
                byte[] path = new byte[9];
                fixed (byte* pathFixed = path)
                {
                    *pathFixed = (byte)'/';
                    *(int*)(pathFixed + sizeof(int)) = '.' + ('h' << 8) + ('t' << 16) + ('m' << 24);
                    *(pathFixed + sizeof(int) * 2) = (byte)'l';
                    foreach (response.state type in System.Enum.GetValues(typeof(response.state)))
                    {
                        response.stateInfo state = Enum<response.state, response.stateInfo>.Array((int)type);
                        if (state != null && state.IsError)
                        {
                            int stateValue = state.Number, value = stateValue / 100;
                            *(pathFixed + 1) = (byte)(value + '0');
                            stateValue -= value * 100;
                            *(pathFixed + 2) = (byte)((value = stateValue / 10) + '0');
                            *(pathFixed + 3) = (byte)((stateValue - value * 10) + '0');
                            response response = null;
                            fileCache fileCache = file(path, default(subArray<byte>), ref response);
                            if (fileCache == null)
                            {
                                if (response != null)
                                {
                                    response.CancelPool();
                                    errorResponse[(int)type].Set(response, response);
                                    isResponse = 1;
                                }
                            }
                            else
                            {
                                response gzipResponse;
                                if ((response = fileCache.Response) == null)
                                {
                                    (response = response.New()).State = (gzipResponse = response.New()).State = type;
                                    response.SetBody(fileCache.Data, true, fileCache.Data.startIndex == fileCache.HttpHeaderSize);
                                    gzipResponse.SetBody(fileCache.GZipData, true, fileCache.GZipData.startIndex == fileCache.HttpHeaderSize);
                                    gzipResponse.ContentEncoding = response.GZipEncoding;
                                }
                                else gzipResponse = fileCache.GZipResponse ?? response;
                                errorResponse[(int)type].Set(response, gzipResponse);
                                isResponse = 1;
                            }
                        }
                    }
                }
            }
            catch (Exception error)
            {
                log.Error.Add(error, null, false);
            }
            if (isResponse != 0) this.errorResponse = errorResponse;
        }
        /// <summary>
        /// HTTP请求处理
        /// </summary>
        /// <param name="socket">HTTP套接字</param>
        /// <param name="socketIdentity">套接字操作编号</param>
        public abstract void Request(socketBase socket, long socketIdentity);
        /// <summary>
        /// WebSocket请求处理
        /// </summary>
        /// <param name="socket">HTTP套接字</param>
        /// <param name="socketIdentity">套接字操作编号</param>
        public virtual void WebSocketRequest(socketBase socket, long socketIdentity)
        {
            socket.ResponseError(socketIdentity, response.state.NotFound404);
        }
        /// <summary>
        /// 获取WEB视图URL重写路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public virtual byte[] GetViewRewrite(subArray<byte> path)
        {
            return null;
        }
        /// <summary>
        /// 获取错误数据
        /// </summary>
        /// <param name="state">错误状态</param>
        /// <param name="isGzip">是否支持GZip压缩</param>
        /// <returns>错误数据</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal response GetErrorResponseData(response.state state, bool isGzip)
        {
            if (errorResponse != null)
            {
                return isGzip ? errorResponse[(int)state].Value : errorResponse[(int)state].Key;
            }
            return null;
        }
        /// <summary>
        /// 设置文件缓存
        /// </summary>
        protected void setCache()
        {
            cacheSize = (long)maxCacheSize << 20;
            if (cacheSize < 0) log.Default.Add("最大缓存字节数(单位MB) " + maxCacheSize.toString() + " << 20 = " + cacheSize.toString(), new System.Diagnostics.StackFrame(), false);
            fileSize = maxCacheFileSize << 10;
            if (fileSize < 0) log.Default.Add("最大文件缓存字节数(单位MB) " + maxCacheSize.toString() + " << 10 = " + fileSize.toString(), new System.Diagnostics.StackFrame(), false);
            if (fileSize > cacheSize) fileSize = (int)cacheSize;
            pathCache.Get(this);
            if (isCacheControl)
            {
                int clientCacheSeconds = this.clientCacheSeconds;
                if (clientCacheSeconds == response.StaticFileCacheControlSeconds) cacheControl = response.StaticFileCacheControl;
                else if (clientCacheSeconds == 0) cacheControl = response.ZeroAgeBytes;
                else cacheControl = ("public, max-age=" + clientCacheSeconds.toString()).getBytes();
            }
            isCacheHeader = isCacheHttpHeader;
            isCacheHtmlHeader = isCacheHtmlHttpHeader;
        }
        /// <summary>
        /// HTTP文件请求处理
        /// </summary>
        /// <param name="path">请求路径</param>
        /// <param name="ifModifiedSince">文件修改时间</param>
        /// <param name="response">HTTP响应输出</param>
        /// <returns>文件缓存</returns>
        protected unsafe fileCache file(byte[] path, subArray<byte> ifModifiedSince, ref response response)
        {
            string cacheFileName = null;
            try
            {
                if (path.Length != 0 && WorkPath.Length + path.Length <= fastCSharp.io.file.MaxFullNameLength)
                {
                    byte[] contentType = null;
                    bool isCompress = true;
                    fixed (byte* pathFixed = path)
                    {
                        byte* pathStart = pathFixed, pathEnd = pathStart + path.Length;
                        if (isFile(pathEnd, ref contentType, ref isCompress) == 0)
                        {
                            //byte directorySeparatorChar = (byte)Path.DirectorySeparatorChar;// || *pathStart == directorySeparatorChar
                            if (*pathStart == '/') ++pathStart;
                            for (byte* formatStart = pathStart; formatStart != pathEnd; ++formatStart)
                            {
                                if (*formatStart == ':')
                                {
                                    response = response.Blank;
                                    return null;
                                }
                            }
                            hashBytes cacheKey = subArray<byte>.Unsafe(path, (int)(pathStart - pathFixed), (int)(pathEnd - pathStart));
                            fileCache fileCache = cache.Get(ref cacheKey);
                            if (fileCache == null)
                            {
                                cacheFileName = fastCSharp.String.FastAllocateString(WorkPath.Length + cacheKey.Length);
                                fixed (char* nameFixed = cacheFileName)
                                {
                                    char* write = nameFixed + WorkPath.Length;
                                    char directorySeparatorChar = Path.DirectorySeparatorChar;
                                    unsafer.String.Copy(WorkPath, nameFixed);
                                    for (byte* start = pathStart; start != pathEnd; ++start) *write++ = *start == '/' ? directorySeparatorChar : (char)*start;
                                }
                                FileInfo file = new FileInfo(cacheFileName);
                                if (file.Exists)
                                {
                                    string fileName = file.FullName;
                                    if (fileName.Length > WorkPath.Length && fastCSharp.unsafer.String.LowerEqualCase(WorkPath, fileName, WorkPath.Length))
                                    {
                                        if (fileName.Length <= fastCSharp.io.file.MaxFullNameLength && file.Length <= fileSize)
                                        {
                                            cacheKey = cacheKey.Copy();
                                            if (cache.GetNew(ref cacheKey, out fileCache) != 0)
                                            {
                                                try
                                                {
                                                    fileCache.lastModified = file.LastWriteTimeUtc.universalToBytes();
                                                    int extensionNameLength = (int)(pathEnd - getExtensionNameStart(pathEnd));
                                                    subArray<byte> fileData = readCacheFile(subString.Unsafe(fileName, fileName.Length - extensionNameLength, extensionNameLength));
                                                    fileCache.Set(ref fileData, contentType, isCompress, contentType == HtmlContentType);
                                                    cache.Set(ref cacheKey, fileCache);
                                                    if (ifModifiedSince.length == fileCache.lastModified.Length)
                                                    {
                                                        fixed (byte* ifModifiedSinceFixed = ifModifiedSince.array)
                                                        {
                                                            if (fastCSharp.unsafer.memory.Equal(fileCache.lastModified, ifModifiedSinceFixed + ifModifiedSince.startIndex, ifModifiedSince.length))
                                                            {
                                                                response = response.NotChanged304;
                                                                return null;
                                                            }
                                                        }
                                                    }
                                                }
                                                finally
                                                {
                                                    if (fileCache.IsData == 0)
                                                    {
                                                        fileCache.PulseAll();
                                                        fileCache = null;
                                                        cache.RemoveOnly(ref cacheKey);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (ifModifiedSince.length == date.ToByteLength && date.UniversalByteEquals(file.LastWriteTimeUtc, ifModifiedSince) == 0)
                                            {
                                                response = response.NotChanged304;
                                                return null;
                                            }
                                            response = response.Get();
                                            response.State = http.response.state.Ok200;
                                            response.BodyFile = fileName;
                                            response.CacheControl = cacheControl;
                                            response.ContentType = contentType;
                                            response.LastModified = file.LastWriteTimeUtc.universalNewBytes();
                                            return null;
                                        }
                                    }
                                }
                            }
                            return fileCache;
                        }
                    }
                }
            }
            catch (Exception error)
            {
                log.Error.Add(error, cacheFileName, false);
            }
            return null;
        }
        /// <summary>
        /// HTTP文件请求处理
        /// </summary>
        /// <param name="request">请求头部</param>
        /// <param name="response">HTTP响应输出</param>
        /// <returns>文件缓存</returns>
        protected unsafe fileCache file(requestHeader request, ref response response)
        {
            subArray<byte> path = request.Path;
            string cacheFileName = null;
            try
            {
                if (path.length != 0 && WorkPath.Length + path.length <= fastCSharp.io.file.MaxFullNameLength)
                {
                    byte[] contentType = null;
                    bool isCompress = true;
                    fixed (byte* pathFixed = request.Buffer)
                    {
                        byte* pathStart = pathFixed + path.startIndex, pathEnd = pathStart + path.length;
                        if (isFile(pathEnd, ref contentType, ref isCompress) == 0)
                        {
                            if (*pathStart == '/') ++pathStart;
                            for (byte* formatStart = pathStart; formatStart != pathEnd; ++formatStart)
                            {
                                if (*formatStart == ':')
                                {
                                    response = response.Blank;
                                    return null;
                                }
#if MONO
#else
                                if ((uint)(*formatStart - 'A') < 26) *formatStart |= 0x20;
#endif
                            }
                            hashBytes cacheKey = subArray<byte>.Unsafe(path.array, (int)(pathStart - pathFixed), (int)(pathEnd - pathStart));
                            fileCache fileCache = cache.Get(ref cacheKey);
                            if (fileCache == null)
                            {
                                cacheFileName = fastCSharp.String.FastAllocateString(WorkPath.Length + cacheKey.Length);
                                fixed (char* nameFixed = cacheFileName)
                                {
                                    char* write = nameFixed + WorkPath.Length;
                                    char directorySeparatorChar = Path.DirectorySeparatorChar;
                                    unsafer.String.Copy(WorkPath, nameFixed);
                                    for (byte* start = pathStart; start != pathEnd; ++start) *write++ = *start == '/' ? directorySeparatorChar : (char)*start;
                                }
                                FileInfo file = new FileInfo(cacheFileName);
                                if (file.Exists)
                                {
                                    string fileName = file.FullName;
                                    if (fileName.Length > WorkPath.Length && fastCSharp.unsafer.String.LowerEqualCase(WorkPath, fileName, WorkPath.Length))
                                    {
                                        if (fileName.Length <= fastCSharp.io.file.MaxFullNameLength && file.Length <= fileSize)
                                        {
                                            cacheKey = cacheKey.Copy();
                                            if (cache.GetNew(ref cacheKey, out fileCache) != 0)
                                            {
                                                try
                                                {
                                                    fileCache.lastModified = file.LastWriteTimeUtc.universalToBytes();
                                                    int extensionNameLength = (int)(pathEnd - getExtensionNameStart(pathEnd));
                                                    subArray<byte> fileData = readCacheFile(subString.Unsafe(fileName, fileName.Length - extensionNameLength, extensionNameLength));
                                                    fileCache.Set(ref fileData, contentType, isCompress, contentType == HtmlContentType);
                                                    cache.Set(ref cacheKey, fileCache);
                                                    if (request.ifModifiedSince.Length == fileCache.lastModified.Length)
                                                    {
                                                        if (fastCSharp.unsafer.memory.Equal(fileCache.lastModified, pathFixed + request.ifModifiedSince.StartIndex, request.ifModifiedSince.Length))
                                                        {
                                                            response = response.NotChanged304;
                                                            return null;
                                                        }
                                                    }
                                                }
                                                finally
                                                {
                                                    if (fileCache.IsData == 0)
                                                    {
                                                        fileCache.PulseAll();
                                                        fileCache = null;
                                                        cache.RemoveOnly(ref cacheKey);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (request.ifModifiedSince.Length == date.ToByteLength && date.UniversalByteEquals(file.LastWriteTimeUtc, request.IfModifiedSince) == 0)
                                            {
                                                response = response.NotChanged304;
                                                return null;
                                            }
                                            response = response.Get();
                                            response.State = http.response.state.Ok200;
                                            response.BodyFile = fileName;
                                            response.CacheControl = cacheControl;
                                            response.ContentType = contentType;
                                            response.LastModified = file.LastWriteTimeUtc.universalNewBytes();
                                            return null;
                                        }
                                    }
                                }
                            }
                            return fileCache;
                        }
                    }
                }
            }
            catch (Exception error)
            {
                log.Error.Add(error, cacheFileName, false);
            }
            return null;
        }
        /// <summary>
        /// 获取扩展名起始位置
        /// </summary>
        /// <param name="end"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe static byte* getExtensionNameStart(byte* end)
        {
            byte* start = end;
            while (*--start != '.') ;
            return start + 1;
        }
        /// <summary>
        /// 读取缓存文件内容
        /// </summary>
        /// <param name="extensionName">文件扩展名</param>
        /// <returns>文件内容</returns>
        protected virtual subArray<byte> readCacheFile(subString extensionName)
        {
            return ReadCacheFile(extensionName.value, WebConfig.IsFileCacheHeader);
        }
        /// <summary>
        /// 读取缓存文件内容
        /// </summary>
        /// <param name="extensionName"></param>
        /// <param name="isFileCacheHeader"></param>
        /// <returns></returns>
        public static subArray<byte> ReadCacheFile(string extensionName, bool isFileCacheHeader)
        {
            if (isFileCacheHeader)
            {
                using (FileStream fileStream = new FileStream(extensionName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    int length = (int)fileStream.Length;
                    byte[] data = new byte[fileCache.HttpHeaderSize + length];
                    fileStream.Read(data, fileCache.HttpHeaderSize, length);
                    return subArray<byte>.Unsafe(data, fileCache.HttpHeaderSize, length);
                }
            }
            return new subArray<byte>(File.ReadAllBytes(extensionName));
        }
        /// <summary>
        /// HTTP文件请求处理
        /// </summary>
        /// <param name="request">请求头部信息</param>
        /// <returns>HTTP响应</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected response file(requestHeader request)
        {
            response response = null;
            file(request, file(request, ref response), ref response);
            return response;
        }
        /// <summary>
        /// HTTP文件请求处理
        /// </summary>
        /// <param name="request">请求头部信息</param>
        /// <param name="path">重定向URL</param>
        /// <returns>HTTP响应</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected response file(requestHeader request, byte[] path)
        {
            response response = null;
            if (request.UnsafeSetPath(path)) file(request, file(request, ref response), ref response);
            else file(request, file(path, request.IfModifiedSince, ref response), ref response);
            return response;
        }
        /// <summary>
        /// HTTP文件请求处理
        /// </summary>
        /// <param name="request">请求头部信息</param>
        /// <param name="fileCache">文件输出信息</param>
        /// <param name="response">HTTP响应</param>
        private unsafe void file(requestHeader request, fileCache fileCache, ref response response)
        {
            if (fileCache == null)
            {
                if (response != null)
                {
                    if (response.BodyFile != null && request.IsRange && !request.FormatRange(response.BodySize))
                    {
                        response = fastCSharp.net.tcp.http.response.RangeNotSatisfiable416;
                        return;
                    }
                    if (response.IsPool && request.origin.Length != 0 && this.isOrigin(request.Origin, request.IsSsl))
                    {
                        response.AccessControlAllowOrigin = request.origin;
                    }
                }
                return;
            }
            bool isRange = request.IsRange;
            if (isRange && !request.FormatRange(fileCache.Data.length))
            {
                response = fastCSharp.net.tcp.http.response.RangeNotSatisfiable416;
                return;
            }
            byte[] cacheControl = isStaticFileCacheControl(request.Path) ? response.StaticFileCacheControl : this.cacheControl;
            bool isOrigin = request.origin.Length != 0 && this.isOrigin(request.Origin, request.IsSsl), isHeader = !isOrigin && !isRange && (fileCache.IsHtml ? isCacheHtmlHeader : isCacheHeader);
            if (isHeader && (response = request.IsGZip ? fileCache.GZipResponse : fileCache.Response) != null && response.CacheControl == cacheControl)
            {
                return;
            }
            subArray<byte> body = request.IsGZip && !isRange ? fileCache.GZipData : fileCache.Data;
            response = response.Get();
            response.State = http.response.state.Ok200;
            response.SetBody(ref body, true, isHeader && body.startIndex == fileCache.HttpHeaderSize);
            response.CacheControl = cacheControl;
            response.ContentType = fileCache.ContentType;
            if (body.array != fileCache.Data.array) response.ContentEncoding = response.GZipEncoding;
            response.LastModified = fileCache.lastModified;
            if (isOrigin) response.AccessControlAllowOrigin = request.origin;
            return;
        }
        /// <summary>
        /// 是否采用静态文件缓存控制策略
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected virtual bool isStaticFileCacheControl(subArray<byte> path)
        {
            return false;
        }
        /// <summary>
        /// 是否支持访问控制权限
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="isSsl">是否SSL链接</param>
        /// <returns></returns>
        protected virtual bool isOrigin(subArray<byte> origin, bool isSsl) { return false; }
        /// <summary>
        /// 设置文件缓存
        /// </summary>
        /// <param name="request">请求头部信息</param>
        /// <param name="response">HTTP响应信息</param>
        /// <param name="contentType">HTTP响应输出类型</param>
        protected unsafe void setCache(requestHeader request, response response, byte[] contentType)
        {
            subArray<byte> path = request.Path;
            if (path.length != 0 && path.length <= fastCSharp.io.file.MaxFullNameLength)
            {
                try
                {
                    fixed (byte* pathFixed = path.array)
                    {
                        byte[] buffer;
                        byte* pathStart = pathFixed + path.startIndex;
                        if (*pathStart == '/')
                        {
                            ++pathStart;
                            buffer = new byte[path.length - 1];
                        }
                        else buffer = new byte[path.length];
#if MONO
                        fixed (byte* bufferFixed = buffer) unsafer.memory.SimpleCopy(pathStart, bufferFixed, buffer.Length);
#else
                        fixed (byte* bufferFixed = buffer) unsafer.memory.ToLower(pathStart, pathStart + buffer.Length, bufferFixed);
#endif
                        fileCache fileCache = new fileCache();
                        fileCache.Set(ref response.Body, contentType, false, response.LastModified, false);
                        hashBytes key = subArray<byte>.Unsafe(buffer, 0, buffer.Length);
                        cache.Set(ref key, fileCache);
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
            }
        }
        /// <summary>
        /// 内容类型头部
        /// </summary>
        private readonly byte[] mp3ContentType = fastCSharp.web.contentTypeInfo.GetContentType("mp3");
        /// <summary>
        /// 内容类型头部
        /// </summary>
        private readonly byte[] mp4ContentType = fastCSharp.web.contentTypeInfo.GetContentType("mp4");
        /// <summary>
        /// 内容类型头部
        /// </summary>
        private readonly byte[] rmvbContentType = fastCSharp.web.contentTypeInfo.GetContentType("rmvb");
        /// <summary>
        /// 内容类型头部
        /// </summary>
        private readonly byte[] docContentType = fastCSharp.web.contentTypeInfo.GetContentType("doc");
        /// <summary>
        /// 内容类型头部
        /// </summary>
        private readonly byte[] woffContentType = fastCSharp.web.contentTypeInfo.GetContentType("woff");
        /// <summary>
        /// 内容类型头部
        /// </summary>
        private readonly byte[] gifContentType = fastCSharp.web.contentTypeInfo.GetContentType("gif");
        /// <summary>
        /// 内容类型头部
        /// </summary>
        private readonly byte[] swfContentType = fastCSharp.web.contentTypeInfo.GetContentType("swf");
        /// <summary>
        /// 内容类型头部
        /// </summary>
        private readonly byte[] pdfContentType = fastCSharp.web.contentTypeInfo.GetContentType("pdf");
        /// <summary>
        /// 内容类型头部
        /// </summary>
        private readonly byte[] otfContentType = fastCSharp.web.contentTypeInfo.GetContentType("otf");
        /// <summary>
        /// 内容类型头部
        /// </summary>
        private readonly byte[] jpegContentType = fastCSharp.web.contentTypeInfo.GetContentType("jpeg");
        /// <summary>
        /// 内容类型头部
        /// </summary>
        private readonly byte[] jpgContentType = fastCSharp.web.contentTypeInfo.GetContentType("jpg");
        /// <summary>
        /// 内容类型头部
        /// </summary>
        private readonly byte[] pngContentType = fastCSharp.web.contentTypeInfo.GetContentType("png");
        /// <summary>
        /// 内容类型头部
        /// </summary>
        private readonly byte[] mpgContentType = fastCSharp.web.contentTypeInfo.GetContentType("mpg");
        /// <summary>
        /// 内容类型头部
        /// </summary>
        private readonly byte[] svgContentType = fastCSharp.web.contentTypeInfo.GetContentType("svg");
        /// <summary>
        /// 内容类型头部
        /// </summary>
        private readonly byte[] aviContentType = fastCSharp.web.contentTypeInfo.GetContentType("avi");
        /// <summary>
        /// 内容类型头部
        /// </summary>
        private readonly byte[] apkContentType = fastCSharp.web.contentTypeInfo.GetContentType("apk");
        /// <summary>
        /// 内容类型头部
        /// </summary>
        private readonly byte[] xmlContentType = fastCSharp.web.contentTypeInfo.GetContentType("xml");
        /// <summary>
        /// 内容类型头部
        /// </summary>
        private readonly byte[] rmContentType = fastCSharp.web.contentTypeInfo.GetContentType("rm");
        /// <summary>
        /// 内容类型头部
        /// </summary>
        private readonly byte[] icoContentType = fastCSharp.web.contentTypeInfo.GetContentType("ico");
        /// <summary>
        /// 内容类型头部
        /// </summary>
        private readonly byte[] zipContentType = fastCSharp.web.contentTypeInfo.GetContentType("zip");
        /// <summary>
        /// 内容类型头部
        /// </summary>
        private readonly byte[] bmpContentType = fastCSharp.web.contentTypeInfo.GetContentType("bmp");
        /// <summary>
        /// 内容类型头部
        /// </summary>
        private readonly byte[] rarContentType = fastCSharp.web.contentTypeInfo.GetContentType("rar");
        /// <summary>
        /// 内容类型头部
        /// </summary>
        private readonly byte[] curContentType = fastCSharp.web.contentTypeInfo.GetContentType("cur");
        /// <summary>
        /// 内容类型头部
        /// </summary>
        private readonly byte[] cssContentType = fastCSharp.web.contentTypeInfo.GetContentType("css");
        /// <summary>
        /// 内容类型头部
        /// </summary>
        private readonly byte[] xlsContentType = fastCSharp.web.contentTypeInfo.GetContentType("xls");
        /// <summary>
        /// 内容类型头部
        /// </summary>
        private readonly byte[] txtContentType = fastCSharp.web.contentTypeInfo.GetContentType("txt");
        /// <summary>
        /// 内容类型头部
        /// </summary>
        private readonly byte[] eotContentType = fastCSharp.web.contentTypeInfo.GetContentType("eot");
        /// <summary>
        /// 内容类型头部
        /// </summary>
        private readonly byte[] wavContentType = fastCSharp.web.contentTypeInfo.GetContentType("wav");
        /// <summary>
        /// 内容类型头部
        /// </summary>
        private readonly byte[] docxContentType = fastCSharp.web.contentTypeInfo.GetContentType("docx");
        /// <summary>
        /// 内容类型头部
        /// </summary>
        private readonly byte[] xlsxContentType = fastCSharp.web.contentTypeInfo.GetContentType("xlsx");
        /// <summary>
        /// 内容类型头部
        /// </summary>
        private readonly byte[] _7zContentType = fastCSharp.web.contentTypeInfo.GetContentType("7z");
        
        /// <summary>
        /// 是否允许文件扩展名
        /// </summary>
        /// <param name="pathEnd">文件路径</param>
        /// <param name="contentType">文件类型</param>
        /// <param name="isCompress">是否需要压缩</param>
        /// <returns>是否允许文件扩展名</returns>
        protected unsafe virtual int isFile(byte* pathEnd, ref byte[] contentType, ref bool isCompress)
        {
            int code = *(int*)(pathEnd - 4);
            if (code < ('A' << 24))
            {
                switch ((code >> 24) - '3')
                {
                    case '3' - '3':
                        if ((code | 0x202000) == '.' + ('m' << 8) + ('p' << 16) + ('3' << 24))
                        {
                            contentType = mp3ContentType;
                            isCompress = false;
                            return 0;
                        }
                        return 1;
                    case '4' - '3':
                        if ((code | 0x202000) == '.' + ('m' << 8) + ('p' << 16) + ('4' << 24))
                        {
                            contentType = mp4ContentType;
                            isCompress = false;
                            return 0;
                        }
                        return 1;
                }
            }
            else
            {
                switch (((code >> 24) | 0x20) - 'b')
                {
                    case 'b' - 'b':
                        if ((((code | 0x20202020) ^ ('r' + ('m' << 8) + ('v' << 16) + ('b' << 24))) | (*(pathEnd - 5) ^ '.')) == 0)
                        {
                            contentType = rmvbContentType;
                            isCompress = false;
                            return 0;
                        }
                        return 1;
                    case 'c' - 'b':
                        if ((code | 0x20202000) == '.' + ('d' << 8) + ('o' << 16) + ('c' << 24))
                        {
                            contentType = docContentType;
                            return 0;
                        }
                        return 1;
                    case 'f' - 'b':
                        if (*(pathEnd - 5) == '.')
                        {
                            if ((code | 0x20202020) == 'w' + ('o' << 8) + ('f' << 16) + ('f' << 24))
                            {
                                contentType = woffContentType;
                                isCompress = false;
                                return 0;
                            }
                            return 1;
                        }
                        if ((code |= 0x20202000) == ('.' + ('g' << 8) + ('i' << 16) + ('f' << 24)))
                        {
                            contentType = gifContentType;
                            isCompress = false;
                            return 0;
                        }
                        if (code == ('.' + ('s' << 8) + ('w' << 16) + ('f' << 24)))
                        {
                            contentType = swfContentType;
                            return 0;
                        }
                        if (code == ('.' + ('p' << 8) + ('d' << 16) + ('f' << 24)))
                        {
                            contentType = pdfContentType;
                            return 0;
                        }
                        if (code == '.' + ('o' << 8) + ('t' << 16) + ('f' << 24))
                        {
                            contentType = otfContentType;
                            isCompress = false;
                            return 0;
                        }
                        return 1;
                    case 'g' - 'b':
                        if (*(pathEnd - 5) == '.')
                        {
                            if ((code | 0x20202020) == 'j' + ('p' << 8) + ('e' << 16) + ('g' << 24))
                            {
                                contentType = jpegContentType;
                                isCompress = false;
                                return 0;
                            }
                            return 1;
                        }
                        if ((code |= 0x20202000) == ('.' + ('j' << 8) + ('p' << 16) + ('g' << 24)))
                        {
                            contentType = jpgContentType;
                            isCompress = false;
                            return 0;
                        }
                        if (code == ('.' + ('p' << 8) + ('n' << 16) + ('g' << 24)))
                        {
                            contentType = pngContentType;
                            isCompress = false;
                            return 0;
                        }
                        if (code == ('.' + ('m' << 8) + ('p' << 16) + ('g' << 24)))
                        {
                            contentType = mpgContentType;
                            isCompress = false;
                            return 0;
                        }
                        if (code == '.' + ('s' << 8) + ('v' << 16) + ('g' << 24))
                        {
                            contentType = svgContentType;
                            return 0;
                        }
                        return 1;
                    case 'i' - 'b':
                        if ((code | 0x20202000) == '.' + ('a' << 8) + ('v' << 16) + ('i' << 24))
                        {
                            contentType = aviContentType;
                            isCompress = false;
                            return 0;
                        }
                        return 1;
                    case 'k' - 'b':
                        if ((code | 0x20202000) == '.' + ('a' << 8) + ('p' << 16) + ('k' << 24))
                        {
                            contentType = apkContentType;
                            isCompress = false;
                            return 0;
                        }
                        return 1;
                    case 'l' - 'b':
                        if ((code | 0x20202020) == ('h' + ('t' << 8) + ('m' << 16) + ('l' << 24)))
                        {
                            if (*(pathEnd - 5) == '.')
                            {
                                contentType = HtmlContentType;
                                return 0;
                            }
                            return 1;
                        }
                        if((code | 0x20202000) == '.' + ('x' << 8) + ('m' << 16) + ('l' << 24))
                        {
                            contentType = xmlContentType;
                            return 0;
                        }
                        return 1;
                    case 'm' - 'b':
                        if ((code | 0x20202000) == ('.' + ('h' << 8) + ('t' << 16) + ('m' << 24)))
                        {
                            contentType = HtmlContentType;
                            return 0;
                        }
                        if ((code | 0x202000ff) == 0xff + ('.' << 8) + ('r' << 16) + ('m' << 24))
                        {
                            contentType = rmContentType;
                            isCompress = false;
                        }
                        return 1;
                    case 'o' - 'b':
                        if ((code | 0x20202000) == '.' + ('i' << 8) + ('c' << 16) + ('o' << 24))
                        {
                            contentType = icoContentType;
                            isCompress = false;
                            return 0;
                        }
                        return 1;
                    case 'p' - 'b':
                        if ((code |= 0x20202000) == ('.' + ('z' << 8) + ('i' << 16) + ('p' << 24)))
                        {
                            contentType = zipContentType;
                            isCompress = false;
                            return 0;
                        }
                        if (code == '.' + ('b' << 8) + ('m' << 16) + ('p' << 24))
                        {
                            contentType = bmpContentType;
                            return 0;
                        }
                        return 1;
                    case 'r' - 'b':
                        if ((code |= 0x20202000) == ('.' + ('r' << 8) + ('a' << 16) + ('r' << 24)))
                        {
                            contentType = rarContentType;
                            isCompress = false;
                            return 0;
                        }
                        if (code == '.' + ('c' << 8) + ('u' << 16) + ('r' << 24))
                        {
                            contentType = curContentType;
                            isCompress = false;
                            return 0;
                        }
                        return 1;
                    case 's' - 'b':
                        if ((code | 0x202000ff) == (0xff + ('.' << 8) + ('j' << 16) + ('s' << 24)))
                        {
                            contentType = JsContentType;
                            return 0;
                        }
                        if ((code |= 0x20202000) == ('.' + ('c' << 8) + ('s' << 16) + ('s' << 24)))
                        {
                            contentType = cssContentType;
                            return 0;
                        }
                        if (code == ('.' + ('x' << 8) + ('l' << 16) + ('s' << 24)))
                        {
                            contentType = xlsContentType;
                            return 0;
                        }
                        return 1;
                    case 't' - 'b':
                        if ((code |= 0x20202000) == ('.' + ('t' << 8) + ('x' << 16) + ('t' << 24)))
                        {
                            contentType = txtContentType;
                            return 0;
                        }
                        if (code == '.' + ('e' << 8) + ('o' << 16) + ('t' << 24))
                        {
                            contentType = eotContentType;
                            isCompress = false;
                            return 0;
                        }
                        return 1;
                    case 'v' - 'b':
                        if ((code | 0x20202000) == '.' + ('w' << 8) + ('a' << 16) + ('v' << 24))
                        {
                            contentType = wavContentType;
                            isCompress = false;
                            return 0;
                        }
                        return 1;
                    case 'x' - 'b':
                        if (*(pathEnd - 5) == '.')
                        {
                            if ((code |= 0x20202020) == ('d' + ('o' << 8) + ('c' << 16) + ('x' << 24)))
                            {
                                contentType = docxContentType;
                                return 0;
                            }
                            if (code == 'x' + ('l' << 8) + ('s' << 16) + ('x' << 24))
                            {
                                contentType = xlsxContentType;
                                return 0;
                            }
                        }
                        return 1;
                    case 'z' - 'b':
                        if ((code | 0x200000ff) == 0xff + ('.' << 8) + ('7' << 16) + ('z' << 24))
                        {
                            contentType = _7zContentType;
                            isCompress = false;
                            return 0;
                        }
                        return 1;
                }
            }
            return 1;
        }
        /// <summary>
        /// 停止监听
        /// </summary>
        protected virtual void stopListen() { }
        /// <summary>
        /// 停止监听
        /// </summary>
        internal void StopListen()
        {
            stopListen();
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        protected virtual bool dispose()
        {
            isStart = 1;
            if (Interlocked.Increment(ref isDisposed) == 1)
            {
                stopListen();
                if (onStop != null) onStop();
                pathCache.Free(this);
                return true;
            }
            isDisposed = 1;
            return false;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            dispose();
        }
        /// <summary>
        /// 重定向服务
        /// </summary>
        public abstract class locationServer : domainServer
        {
            /// <summary>
            /// 重定向域名
            /// </summary>
            private byte[] locationDomain;
            /// <summary>
            /// SSL重定向域名
            /// </summary>
            private byte[] sslLocationDomain;
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
            /// 启动HTTP服务
            /// </summary>
            /// <param name="domains">域名信息集合</param>
            /// <param name="onStop">停止服务处理</param>
            /// <returns>是否启动成功</returns>
            public override bool Start(domain[] domains, Action onStop)
            {
                if (isStart == 0)
                {
                    string domain = getLocationDomain();
                    if (!string.IsNullOrEmpty(domain))
                    {
                        if (domain[domain.Length - 1] != '/') domain += "/";
                        byte[] domainData = domain.getBytes();
                        if (Interlocked.CompareExchange(ref isStart, 1, 0) == 0)
                        {
                            locationDomain = sslLocationDomain = domainData;
                            string sslDomain = getSslLocationDomain();
                            if (!string.IsNullOrEmpty(sslDomain))
                            {
                                if (sslDomain[sslDomain.Length - 1] != '/') sslDomain += "/";
                                sslLocationDomain = sslDomain.getBytes();
                            }
                            this.domains = domains;
                            this.onStop += onStop;
                            return true;
                        }
                    }
                }
                return false;
            }
            /// <summary>
            /// 获取包含协议的重定向域名,比如 http://www.ligudan.com
            /// </summary>
            /// <returns>获取包含协议的重定向域名</returns>
            protected abstract string getLocationDomain();
            /// <summary>
            /// 获取包含协议的重定向域名,比如 https://www.ligudan.com
            /// </summary>
            /// <returns>获取包含协议的重定向域名</returns>
            protected virtual string getSslLocationDomain()
            {
                return getLocationDomain();
            }
            /// <summary>
            /// HTTP请求处理
            /// </summary>
            /// <param name="socket">HTTP套接字</param>
            /// <param name="socketIdentity">套接字操作编号</param>
            public override unsafe void Request(socketBase socket, long socketIdentity)
            {
                requestHeader request = socket.RequestHeader;
                if ((request.ContentLength | request.Boundary.length) == 0)
                {
                    response response = response.Get();
                    response.State = http.response.state.MovedPermanently301;
                    byte[] domain = socket.IsSsl ? sslLocationDomain : locationDomain, buffer = request.Buffer;
                    bufferIndex uri = request.uri;
                    int length = domain.Length + uri.Length;
                    if (uri.Length != 0 && length <= buffer.Length)
                    {
                        if (buffer[uri.StartIndex] == '/')
                        {
                            uri.Next();
                            if (uri.Length == 0) goto END;
                            --length;
                        }
                        int startIndex = uri.StartIndex - domain.Length;
                        if (startIndex >= 0)
                        {
                            Buffer.BlockCopy(domain, 0, buffer, startIndex, domain.Length);
                            response.Location.UnsafeSet(buffer, startIndex, length);
                            socket.Response(socketIdentity, ref response);
                            return;
                        }
                        int endIndex = uri.EndIndex;
                        if (buffer.Length - endIndex - 7 >= length)
                        {
                            fixed (byte* bufferFixed = buffer)
                            {
                                byte* write = bufferFixed + endIndex;
                                fastCSharp.unsafer.memory.UnsafeSimpleCopy(domain, write, domain.Length);
                                fastCSharp.unsafer.memory.UnsafeSimpleCopy(bufferFixed + uri.StartIndex, write + domain.Length, uri.Length);
                            }
                            response.Location.UnsafeSet(buffer, endIndex, length);
                            socket.Response(socketIdentity, ref response);
                            return;
                        }
                    }
                END:
                    response.Location.UnsafeSet(domain, 0, domain.Length);
                    socket.Response(socketIdentity, ref response);
                }
                else socket.ResponseError(socketIdentity, http.response.state.BadRequest400);
            }
        }
        /// <summary>
        /// 文件服务
        /// </summary>
        public class fileServer : domainServer
        {
            /// <summary>
            /// 文件监视器
            /// </summary>
            private FileSystemWatcher fileWatcher;
            /// <summary>
            /// 文件监视器缓冲区
            /// </summary>
            private byte[] fileWatcherBuffer;
            /// <summary>
            /// 
            /// </summary>
            private object fileWatcherLock;
            /// <summary>
            /// 启动HTTP服务
            /// </summary>
            /// <param name="domains">域名信息集合</param>
            /// <param name="onStop">停止服务处理</param>
            /// <returns>是否启动成功</returns>
            public override bool Start(domain[] domains, Action onStop)
            {
                string path = fastCSharp.io.file.FileNameToLower((this.path.fileNameToLower() ?? LoadCheckPath).pathSuffix());
                if (Directory.Exists(path) && Interlocked.CompareExchange(ref isStart, 1, 0) == 0)
                {
                    WorkPath = path;
                    this.domains = domains;
                    this.onStop += onStop;
                    setCache();
                    fileWatcherBuffer = new byte[fastCSharp.io.file.MaxFullNameLength];
                    fileWatcherLock = new object();
                    fileWatcher = new FileSystemWatcher(path);
                    fileWatcher.IncludeSubdirectories = true;
                    fileWatcher.EnableRaisingEvents = true;
                    fileWatcher.Changed += fileChanged;
                    fileWatcher.Deleted += fileChanged;
                    createErrorResponse();
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 文件更新事件
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private unsafe void fileChanged(object sender, FileSystemEventArgs e)
            {
                try
                {
                    string fullPath = e.FullPath;
                    if (fullPath.Length - WorkPath.Length <= fileWatcherBuffer.Length)
                    {
                        char directorySeparatorChar = Path.DirectorySeparatorChar;
                        Monitor.Enter(fileWatcherLock);
                        try
                        {
                            fixed (byte* bufferFixed = fileWatcherBuffer)
                            fixed (char* pathFixed = fullPath)
                            {
                                byte* write = bufferFixed;
                                char* start = pathFixed + WorkPath.Length, end = pathFixed + fullPath.Length;
                                while (start != end)
                                {
                                    char value = *start++;
                                    if ((uint)(value - 'A') < 26) *write++ = (byte)(value | 0x20);
                                    else *write++ = value == directorySeparatorChar ? (byte)'/' : (byte)value;
                                }
                                hashBytes key = subArray<byte>.Unsafe(fileWatcherBuffer, 0, (int)(write - bufferFixed));
                                cache.Remove(ref key);
                            }
                        }
                        finally { Monitor.Exit(fileWatcherLock); }
                    }
                }
                catch (Exception error)
                {
                    fastCSharp.log.Default.Add(error, null, false);
                }
            }
            /// <summary>
            /// HTTP请求处理
            /// </summary>
            /// <param name="socket">HTTP套接字</param>
            /// <param name="socketIdentity">套接字操作编号</param>
            public override void Request(socketBase socket, long socketIdentity)
            {
                response response = file(socket.RequestHeader);
                if (response != null) socket.Response(socketIdentity, ref response);
                else socket.ResponseError(socketIdentity, http.response.state.NotFound404);
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            protected override bool dispose()
            {
                if (base.dispose())
                {
                    if (fileWatcher != null)
                    {
                        fileWatcher.EnableRaisingEvents = false;
                        fileWatcher.Changed -= fileChanged;
                        fileWatcher.Deleted -= fileChanged;
#if XAMARIN
#else
                        fileWatcher.Dispose();
#endif
                    }
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// 静态文件服务
        /// </summary>
        public abstract class staticFileServer : fileServer
        {
            /// <summary>
            /// 客户端缓存时间(单位:秒)
            /// </summary>
            protected override int clientCacheSeconds
            {
                get { return response.StaticFileCacheControlSeconds; }
            }
            /// <summary>
            /// 文件缓存是否预留HTTP头部
            /// </summary>
            protected override bool isCacheHttpHeader { get { return true; } }
            /// <summary>
            /// HTTP请求处理
            /// </summary>
            /// <param name="socket">HTTP套接字</param>
            /// <param name="socketIdentity">套接字操作编号</param>
            public override void Request(socketBase socket, long socketIdentity)
            {
                requestHeader request = socket.RequestHeader;
                if (request.IfModifiedSince.length == 0) this.request(socket, socketIdentity, request);
                else socket.Response(socketIdentity, response.NotChanged304);
            }
            /// <summary>
            /// HTTP请求处理
            /// </summary>
            /// <param name="socket">HTTP套接字</param>
            /// <param name="socketIdentity">套接字操作编号</param>
            /// <param name="request">请求头部信息</param>
            protected virtual void request(socketBase socket, long socketIdentity, requestHeader request)
            {
                response response = file(request);
                if (response != null) socket.Response(socketIdentity, ref response);
                else socket.ResponseError(socketIdentity, http.response.state.NotFound404);
            }
        }
        /// <summary>
        /// WEB视图服务
        /// </summary>
        public abstract class viewServer : fileServer
        {
            /// <summary>
            /// 每秒动态请求数量(不包括搜索引擎)
            /// </summary>
            private static secondCount secondCount = new secondCount(fastCSharp.config.http.Default.CountSeconds);
            /// <summary>
            /// 每秒动态请求数量(不包括搜索引擎)
            /// </summary>
            public static int[] SecondCount
            {
                get { return secondCount.Counts; }
            }
            /// <summary>
            /// 表单加载
            /// </summary>
            private class loadForm<callType, webType> : requestForm.ILoadForm
                where callType : webCall.callPool<callType, webType>
                where webType : webPage.page, webCall.IWebCall
            {
                /// <summary>
                /// HTTP套接字接口
                /// </summary>
                private socketBase socket;
                ///// <summary>
                ///// HTTP请求头
                ///// </summary>
                //private requestHeader request;
                /// <summary>
                /// WEB调用
                /// </summary>
                private callType webCall;
                /// <summary>
                /// 内存流最大字节数
                /// </summary>
                private int maxMemoryStreamSize;
                /// <summary>
                /// 表单回调处理
                /// </summary>
                /// <param name="form">HTTP请求表单</param>
                public void OnGetForm(requestForm form)
                {
                    long identity;
                    if (form == null)
                    {
                        identity = webCall.WebCall.SocketIdentity;
                        webCall.WebCall.PushPool();
                        webCall.WebCall = null;
                        typePool<callType>.PushNotNull(webCall);
                    }
                    else
                    {
                        identity = form.Identity;
                        //response response = null;
                        try
                        {
                            webType call = webCall.WebCall;
                            //call.Response = response.Get();
                            call.SocketIdentity = identity;
                            call.RequestForm = form;
                            if (webCall.Call()) return;
                        }
                        catch (Exception error)
                        {
                            log.Error.Add(error, null, false);
                        }
                        //finally { response.Push(ref response); }
                    }
                    socket.ResponseError(identity, fastCSharp.net.tcp.http.response.state.ServerError500);
                    socket = null;
                    //request = null;
                    webCall = null;
                    typePool<loadForm<callType, webType>>.PushNotNull(this);
                }
                /// <summary>
                /// 根据HTTP请求表单值获取内存流最大字节数
                /// </summary>
                /// <param name="value">HTTP请求表单值</param>
                /// <returns>内存流最大字节数</returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public int MaxMemoryStreamSize(ref requestForm.value value)
                {
                    return maxMemoryStreamSize > 0 ? maxMemoryStreamSize : fastCSharp.config.appSetting.StreamBufferSize;
                }
                /// <summary>
                /// 根据HTTP请求表单值获取保存文件全称
                /// </summary>
                /// <param name="value">HTTP请求表单值</param>
                /// <returns>文件全称</returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public string GetSaveFileName(ref requestForm.value value)
                {
                    return webCall.WebCall.GetSaveFileName(ref value);
                }
                /// <summary>
                /// 获取表单加载
                /// </summary>
                /// <param name="socket">HTTP套接字接口</param>
                /// <param name="webCall">WEB调用接口</param>
                /// <param name="maxMemoryStreamSize">内存流最大字节数</param>
                /// <returns>表单加载</returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Set(socketBase socket, callType webCall, int maxMemoryStreamSize)
                {
                    this.socket = socket;
                    //this.request = request;
                    this.webCall = webCall;
                    this.maxMemoryStreamSize = maxMemoryStreamSize;
                }
            }
            /// <summary>
            /// WEB视图页面索引集合
            /// </summary>
            protected virtual string[] views
            {
                get { return nullValue<string>.Array; }
            }
            /// <summary>
            /// WEB视图页面索引集合
            /// </summary>
            private pointer.size viewIndexs;
            /// <summary>
            /// WEB视图URL重写路径集合
            /// </summary>
            protected virtual keyValue<string[], string[]> rewrites
            {
                get { return new keyValue<string[], string[]>(nullValue<string>.Array, nullValue<string>.Array); }
            }
            /// <summary>
            /// WEB视图URL重写路径集合
            /// </summary>
            private stateSearcher.ascii<byte[]> rewritePaths;
            /// <summary>
            /// WEB视图URL重写索引集合
            /// </summary>
            protected virtual string[] viewRewrites
            {
                get { return nullValue<string>.Array; }
            }
            /// <summary>
            /// WEB视图URL重写索引集合
            /// </summary>
            private pointer.size rewriteIndexs;
            /// <summary>
            /// WEB调用处理索引集合
            /// </summary>
            protected virtual string[] calls
            {
                get { return nullValue<string>.Array; }
            }
            /// <summary>
            /// WEB调用处理索引集合
            /// </summary>
            private pointer.size callIndexs;
            /// <summary>
            /// WebSocket调用处理集合
            /// </summary>
            protected virtual string[] webSockets
            {
                get { return nullValue<string>.Array; }
            }
            /// <summary>
            /// WebSocket调用处理委托集合
            /// </summary>
            private pointer.size webSocketIndexs;
            /// <summary>
            /// HTML文件缓存是否预留HTTP头部
            /// </summary>
            protected override bool isCacheHtmlHttpHeader { get { return true; } }
            /// <summary>
            /// 释放资源
            /// </summary>
            /// <returns></returns>
            protected override bool dispose()
            {
                if (base.dispose())
                {
                    pub.Dispose(ref rewritePaths);
                    unmanaged.Free(ref viewIndexs);
                    unmanaged.Free(ref callIndexs);
                    unmanaged.Free(ref rewriteIndexs);
                    unmanaged.Free(ref webSocketIndexs);
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 启动HTTP服务
            /// </summary>
            /// <param name="domains">域名信息集合</param>
            /// <param name="onStop">停止服务处理</param>
            /// <returns>是否启动成功</returns>
            public override bool Start(domain[] domains, Action onStop)
            {
                keyValue<string[], string[]> rewrites = this.rewrites;
                rewritePaths = new stateSearcher.ascii<byte[]>(rewrites.Key, rewrites.Value.getArray(value => value.getBytes()), false);
                viewIndexs = stateSearcher.asciiSearcher.Create(views, false);
                callIndexs = stateSearcher.asciiSearcher.Create(calls, false);
                rewriteIndexs = stateSearcher.asciiSearcher.Create(viewRewrites, false);
                webSocketIndexs = stateSearcher.asciiSearcher.Create(webSockets, false);
                return base.Start(domains, onStop);
            }
            /// <summary>
            /// HTTP请求处理
            /// </summary>
            /// <param name="socket">HTTP套接字</param>
            /// <param name="socketIdentity">套接字操作编号</param>
            public override void Request(socketBase socket, long socketIdentity)
            {
                requestHeader request = socket.RequestHeader;
#if MONO
                subArray<byte> path = request.Path;
#else
                subArray<byte> path = WebConfig.IgnoreCase ? request.LowerPath : request.Path;
#endif
                int index;
                if (request.IsSearchEngine)
                {
                    //request.IsViewPath && 
                    if ((index = new stateSearcher.asciiSearcher(ref rewriteIndexs).Search(ref path)) >= 0)
                    {
                        this.request(index, socket, socketIdentity);
                        return;
                    }
                    if ((index = new stateSearcher.asciiSearcher(ref viewIndexs).Search(ref path)) >= 0)
                    {
                        this.request(index, socket, socketIdentity);
                        return;
                    }
                    if ((index = new stateSearcher.asciiSearcher(ref callIndexs).Search(ref path)) >= 0)
                    {
                        call(index, socket, socketIdentity);
                        return;
                    }
                }
                else
                {
                    if ((index = new stateSearcher.asciiSearcher(ref callIndexs).Search(ref path)) >= 0)
                    {
                        call(index, socket, socketIdentity);
                        secondCount.Add();
                        return;
                    }
                    byte[] rewritePath = rewritePaths.Get(ref path);
                    if (rewritePath != null)
                    {
                        response response = file(request, rewritePath);
                        if (response != null)
                        {
                            socket.Response(socketIdentity, ref response);
                            return;
                        }
                        socket.ResponseError(socketIdentity, http.response.state.NotFound404);
                    }
                }
                if (beforeFile(socket, socketIdentity)) base.Request(socket, socketIdentity);
            }
            /// <summary>
            /// 视图页面处理
            /// </summary>
            /// <param name="viewIndex"></param>
            /// <param name="socket"></param>
            /// <param name="socketIdentity"></param>
            protected virtual void request(int viewIndex, socketBase socket, long socketIdentity) { }
            /// <summary>
            /// WEB调用处理
            /// </summary>
            /// <param name="callIndex"></param>
            /// <param name="socket"></param>
            /// <param name="socketIdentity"></param>
            protected virtual void call(int callIndex, socketBase socket, long socketIdentity) { }
            /// <summary>
            /// 获取WEB视图URL重写路径
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public override byte[] GetViewRewrite(subArray<byte> path)
            {
                return rewritePaths.Get(ref path);
            }
            /// <summary>
            /// 加载页面视图
            /// </summary>
            /// <param name="socket">HTTP套接字接口</param>
            /// <param name="socketIdentity">套接字操作编号</param>
            /// <param name="view">WEB视图接口</param>
            /// <param name="isPool">是否使用WEB视图池</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected void load<viewType>(socketBase socket, long socketIdentity, viewType view, bool isPool)
                where viewType : webView.view
            {
                requestHeader request = socket.RequestHeader;
                if ((request.ContentLength | request.Boundary.length) == 0 && request.Method == web.http.methodType.GET)
                {
                    view.Socket = socket;
                    view.DomainServer = this;
                    if (view.LoadHeader(socketIdentity, request, ref isPool))
                    {
                        view.Load(null, false);
                        return;
                    }
                }
                else if (isPool) typePool<viewType>.PushNotNull(view);
                socket.ResponseError(socketIdentity, response.state.ServerError500);
            }
            /// <summary>
            /// 加载web调用
            /// </summary>
            /// <param name="socket">HTTP套接字接口</param>
            /// <param name="socketIdentity">套接字操作编号</param>
            /// <param name="call">web调用</param>
            /// <param name="webCall"></param>
            /// <param name="maxPostDataSize"></param>
            /// <param name="maxMemoryStreamSize"></param>
            /// <param name="isOnlyPost"></param>
            /// <param name="isPool"></param>
            protected void load<callType, webType>(socketBase socket, long socketIdentity, callType call, webType webCall
                , int maxPostDataSize, int maxMemoryStreamSize, bool isOnlyPost, bool isPool)
                where callType : webCall.callPool<callType, webType>
                where webType : webPage.page, webCall.IWebCall
            {
                call.WebCall = webCall;
                requestHeader request = socket.RequestHeader;
                if (request.ContentLength <= maxPostDataSize && (request.Method == web.http.methodType.POST || !isOnlyPost))
                {
                    webCall.Socket = socket;
                    webCall.DomainServer = this;
                    webCall.LoadHeader(socketIdentity, request, ref isPool);
                    if (request.Method == web.http.methodType.POST)
                    {
                        loadForm<callType, webType> loadForm = typePool<loadForm<callType, webType>>.Pop() ?? new loadForm<callType, webType>();
                        loadForm.Set(socket, call, maxMemoryStreamSize);
                        socket.GetForm(socketIdentity, loadForm);
                        return;
                    }
                    webCall.RequestForm = null;
                    //response response = webCall.Response = response.Get();
                    //try
                    //{
                    if (call.Call()) return;
                    //}
                    //finally { response.Push(ref response); }
                }
                else
                {
                    if (isPool) typePool<webType>.Push(ref call.WebCall);
                    typePool<callType>.PushNotNull(call);
                }
                socket.ResponseError(socketIdentity, http.response.state.ServerError500);
            }
            /// <summary>
            /// 加载web调用
            /// </summary>
            /// <param name="socket">HTTP套接字接口</param>
            /// <param name="socketIdentity">套接字操作编号</param>
            /// <param name="call">web调用</param>
            /// <param name="webCall"></param>
            protected void loadAjax<callType, webType>(socketBase socket, long socketIdentity, callType call, webType webCall)
                where callType : webCall.callPool<callType, webType>
                where webType : webCall.call, webCall.IWebCall
            {
                call.WebCall = webCall;
                webCall.Socket = socket;
                webCall.DomainServer = this;
                bool isPool = true;
                if (webCall.LoadHeader(socketIdentity, socket.RequestHeader, ref isPool) && call.Call()) return;
                socket.ResponseError(socketIdentity, response.state.ServerError500);
            }
            /// <summary>
            /// WebSocket请求处理
            /// </summary>
            /// <param name="socket">HTTP套接字</param>
            /// <param name="socketIdentity">套接字操作编号</param>
            public override void WebSocketRequest(socketBase socket, long socketIdentity)
            {
                int index = new stateSearcher.asciiSearcher(ref webSocketIndexs).Search(socket.RequestHeader.Path);
                if (index >= 0) 
                {
                    try
                    {
                        callWebSocket(index, socket, socketIdentity);
                        return;
                    }
                    catch (Exception error)
                    {
                        socket.ResponseError(socketIdentity, response.state.ServerError500);
                        log.Error.Add(error, null, false);
                    }
                }
                else socket.ResponseError(socketIdentity, response.state.NotFound404);
            }
            /// <summary>
            /// webSocket调用处理
            /// </summary>
            /// <param name="callIndex"></param>
            /// <param name="socket"></param>
            /// <param name="socketIdentity"></param>
            protected virtual void callWebSocket(int callIndex, fastCSharp.net.tcp.http.socketBase socket, long socketIdentity) { }
            /// <summary>
            /// 加载WebSocket
            /// </summary>
            /// <param name="value"></param>
            /// <param name="socket">HTTP套接字接口</param>
            /// <param name="socketIdentity">套接字操作编号</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected void loadWebSocket<socketType>(socketType value, socketBase socket, long socketIdentity)
                where socketType : webSocket.socket
            {
                value.Load(this, socket, socketIdentity);
            }
            /// <summary>
            /// 文件服务处理之前
            /// </summary>
            /// <param name="socket"></param>
            /// <param name="socketIdentity"></param>
            /// <returns>是否需要继续提交给文件服务处理</returns>
            protected virtual bool beforeFile(socketBase socket, long socketIdentity)
            {
                return true;
            }
        }
        /// <summary>
        /// WEB视图服务
        /// </summary>
        public abstract class viewServer<sessionType> : viewServer
        {
#if NOJIT
            /// <summary>
            /// 启动HTTP服务
            /// </summary>
            /// <param name="domains">域名信息集合</param>
            /// <param name="onStop">停止服务处理</param>
            /// <returns>是否启动成功</returns>
            public override bool Start(domain[] domains, Action onStop)
            {
                Session = getSession();
                return base.Start(domains, onStop);
            }
            /// <summary>
            /// 获取Session
            /// </summary>
            /// <returns>Session</returns>
            protected virtual ISession getSession()
            {
                return new session<sessionType>();
            }
#else
            /// <summary>
            /// Session
            /// </summary>
            private ISession<sessionType> session;
            /// <summary>
            /// 启动HTTP服务
            /// </summary>
            /// <param name="domains">域名信息集合</param>
            /// <param name="onStop">停止服务处理</param>
            /// <returns>是否启动成功</returns>
            public override bool Start(domain[] domains, Action onStop)
            {
                Session = session = getSession();
                return base.Start(domains, onStop);
            }
            /// <summary>
            /// 获取Session
            /// </summary>
            /// <returns>Session</returns>
            protected virtual ISession<sessionType> getSession()
            {
                return new session<sessionType>();
            }
#endif
        }
    }
}
