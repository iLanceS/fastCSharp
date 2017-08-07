using System;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using fastCSharp.threading;
using fastCSharp.web;
using fastCSharp.net.tcp.http;
using System.Runtime.CompilerServices;

namespace fastCSharp.net
{
    /// <summary>
    /// HTML标题获取客户端
    /// </summary>
    public sealed class htmlTitleClient : timeoutQueue.ICheckTimeout, IDisposable
    {
        /// <summary>
        /// 最小URI字节长度
        /// </summary>
        private const int minUriSize = 10;
        /// <summary>
        /// 最小缓存区字节长度
        /// </summary>
        private const int minBufferSize = 1 << 10;
        /// <summary>
        /// 超时处理队列
        /// </summary>
        private static readonly timeoutQueue defaultTimeoutQueue = timeoutQueue.Get(10);
        /// <summary>
        /// HTML标题获取客户端任务池
        /// </summary>
        public sealed class task : IDisposable
        {
            /// <summary>
            /// Uri与回调函数信息
            /// </summary>
            private sealed class uriInfo
            {
                /// <summary>
                /// 下一个Uri与回调函数信息
                /// </summary>
                private uriInfo next;
                /// <summary>
                /// 获取HTML标题回调函数
                /// </summary>
                private Action<string> callbackHandle;
                /// <summary>
                /// HTML标题获取客户端任务池
                /// </summary>
                private task task;
                /// <summary>
                /// HTML标题获取客户端
                /// </summary>
                private htmlTitleClient client;
                /// <summary>
                /// Uri字符串
                /// </summary>
                private string uriString;
                /// <summary>
                /// Uri
                /// </summary>
                private subArray<byte> uri;
                /// <summary>
                /// 获取HTML标题回调函数
                /// </summary>
                private Action<string> onGet;
                /// <summary>
                /// 默认编码格式
                /// </summary>
                private Encoding encoding;
                /// <summary>
                /// Uri与回调函数信息
                /// </summary>
                private uriInfo()
                {
                    callbackHandle = callback;
                }
                /// <summary>
                /// 获取HTML标题
                /// </summary>
                /// <param name="task">HTML标题获取客户端任务池</param>
                /// <param name="client">HTML标题获取客户端</param>
                public void Get(task task, htmlTitleClient client)
                {
                    this.task = task;
                    this.client = client;
                    if (uriString == null) client.get(ref uri, callbackHandle, encoding, 0, false);
                    else client.get(uriString, callbackHandle, encoding);
                }
                /// <summary>
                /// 获取HTML标题回调
                /// </summary>
                /// <param name="title">HTML标题</param>
                private void callback(string title)
                {
                    uriString = null;
                    uri.Null();
                    task.push(client);
                    task = null;
                    client = null;
                    next = null;
                    try
                    {
                        onGet(title);
                    }
                    finally { typePool<uriInfo>.PushNotNull(this); }
                }
                /// <summary>
                /// 取消调用
                /// </summary>
                public void Cancel()
                {
                    uriString = null;
                    uri.Null();
                    next = null;
                    try
                    {
                        onGet(null);
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, null, false);
                    }
                    finally { typePool<uriInfo>.PushNotNull(this); }
                }
                /// <summary>
                /// 获取Uri与回调函数信息
                /// </summary>
                /// <param name="uri">Uri</param>
                /// <param name="onGet">获取HTML标题回调函数</param>
                /// <param name="encoding">默认编码格式</param>
                /// <returns>Uri与回调函数信息</returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Set(string uri, Action<string> onGet, Encoding encoding)
                {
                    uriString = uri;
                    this.onGet = onGet;
                    this.encoding = encoding;
                }
                /// <summary>
                /// 获取Uri与回调函数信息
                /// </summary>
                /// <param name="uri">Uri</param>
                /// <param name="onGet">获取HTML标题回调函数</param>
                /// <param name="encoding">默认编码格式</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Set(ref subArray<byte> uri, Action<string> onGet, Encoding encoding)
                {
                    this.uri = uri;
                    this.onGet = onGet;
                    this.encoding = encoding;
                }
                /// <summary>
                /// 获取Uri与回调函数信息
                /// </summary>
                /// <returns>Uri与回调函数信息</returns>
                public static uriInfo Get()
                {
                    uriInfo value = typePool<uriInfo>.Pop();
                    if (value == null)
                    {
                        try
                        {
                            value = new uriInfo();
                        }
                        catch (Exception error)
                        {
                            log.Error.Add(error, null, false);
                        }
                    }
                    return value;
                }
                /// <summary>
                /// 取消调用
                /// </summary>
                /// <param name="value"></param>
                public static void CancelQueue(uriInfo value)
                {
                    while (value != null)
                    {
                        uriInfo next = value.next;
                        value.next = null;
                        value.Cancel();
                        value = next;
                    }
                }
                /// <summary>
                /// Uri队列
                /// </summary>
                public struct queue
                {
                    /// <summary>
                    /// 第一个节点
                    /// </summary>
                    public uriInfo Head;
                    /// <summary>
                    /// 最后一个节点
                    /// </summary>
                    public uriInfo End;
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
                    public void Push(uriInfo output)
                    {
                        if (Head == null) Head = End = output;
                        else
                        {
                            End.next = output;
                            End = output;
                        }
                    }
                    /// <summary>
                    /// 获取输出信息
                    /// </summary>
                    /// <returns></returns>
                    [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                    public uriInfo Pop()
                    {
                        if (Head == null) return null;
                        uriInfo command = Head;
                        Head = Head.next;
                        command.next = null;
                        return command;
                    }
                }
            }
            /// <summary>
            /// 客户端集合
            /// </summary>
            private htmlTitleClient[] clients;
            /// <summary>
            /// Uri与回调函数信息集合
            /// </summary>
            private uriInfo.queue uris;
            /// <summary>
            /// 超时处理队列
            /// </summary>
            private timeoutQueue timeoutQueue;
            /// <summary>
            /// 当前客户端位置
            /// </summary>
            private int clientIndex;
            /// <summary>
            /// 客户端集合访问锁
            /// </summary>
            private readonly object clientLock = new object();
            /// <summary>
            /// 当前实例数量
            /// </summary>
            private int clientCount;
            /// <summary>
            /// 收发数据缓冲区字节数
            /// </summary>
            private int bufferSize;
            /// <summary>
            /// 最大搜索字节数
            /// </summary>
            private int maxSearchSize;
            ///// <summary>
            ///// 超时值（以毫秒为单位）
            ///// </summary>
            //public int ReceiveTimeout
            //{
            //    set
            //    {
            //        timeoutQueue.TimeoutSeconds = value;
            //    }
            //}
            /// <summary>
            /// 是否验证安全证书
            /// </summary>
            private bool isValidateCertificate;
            /// <summary>
            /// 是否验证安全证书
            /// </summary>
            public bool IsValidateCertificate
            {
                get { return isValidateCertificate; }
                set
                {
                    if (isValidateCertificate ^ value)
                    {
                        Monitor.Enter(clientLock);
                        if (clientIndex != 0)
                        {
                            int count = clientIndex;
                            foreach (htmlTitleClient client in clients)
                            {
                                client.IsValidateCertificate = value;
                                if (--count == 0) break;
                            }
                        }
                        isValidateCertificate = value;
                        Monitor.Exit(clientLock);
                    }
                }
            }
            /// <summary>
            /// 是否已经释放资源
            /// </summary>
            private int isDisposed;
            /// <summary>
            /// HTML标题获取客户端任务池
            /// </summary>
            /// <param name="maxClientCount">最大实例数量</param>
            /// <param name="bufferSize">收发数据缓冲区字节数</param>
            /// <param name="maxSearchSize">最大搜索字节数</param>
            public task(int maxClientCount = 1, int bufferSize = 1 << 11, int maxSearchSize = 0)
            {
                this.bufferSize = Math.Max(minBufferSize, bufferSize);
                this.maxSearchSize = maxSearchSize;
                clients = new htmlTitleClient[maxClientCount <= 0 ? 1 : maxClientCount];
                timeoutQueue = timeoutQueue.Get(10);
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                isDisposed = 1;
                uriInfo uriInfo = null;
                Monitor.Enter(clientLock);
                try
                {
                    if (clientIndex != 0)
                    {
                        foreach (htmlTitleClient client in clients)
                        {
                            pub.Dispose(client);
                            --clientCount;
                            if (--clientIndex == 0) break;
                        }
                    }
                    Array.Clear(clients, 0, clients.Length);
                    uriInfo = uris.Head;
                    uris.Clear();
                }
                finally { Monitor.Exit(clientLock); }
                uriInfo.CancelQueue(uriInfo);
            }
            /// <summary>
            /// 获取HTML标题
            /// </summary>
            /// <param name="uri">Uri</param>
            /// <param name="onGet">获取HTML标题回调函数</param>
            /// <param name="encoding">默认编码格式</param>
            public void Get(string uri, Action<string> onGet, Encoding encoding = null)
            {
                if (onGet == null) log.Error.Throw(log.exceptionType.Null);
                if (isDisposed == 0 && uri.length() >= minUriSize)
                {
                    uriInfo uriInfo = uriInfo.Get();
                    if (uriInfo != null)
                    {
                        uriInfo.Set(uri, onGet, encoding);
                        get(uriInfo);
                        return;
                    }
                }
                onGet(null);
            }
            /// <summary>
            /// 获取HTML标题
            /// </summary>
            /// <param name="uri">Uri</param>
            /// <param name="onGet">获取HTML标题回调函数</param>
            /// <param name="encoding">默认编码格式</param>
            public void Get(ref subArray<byte> uri, Action<string> onGet, Encoding encoding = null)
            {
                if (onGet == null) log.Error.Throw(log.exceptionType.Null);
                if (isDisposed == 0 && uri.Count >= minUriSize)
                {
                    uriInfo uriInfo = uriInfo.Get();
                    if (uriInfo != null)
                    {
                        uriInfo.Set(ref uri, onGet, encoding);
                        get(uriInfo);
                        return;
                    }
                }
                onGet(null);
            }
            /// <summary>
            /// 获取HTML标题
            /// </summary>
            /// <param name="uri">Uri与回调函数信息</param>
            private void get(uriInfo uri)
            {
                if (uri != null)
                {
                    htmlTitleClient client = null;
                    bool newClient = false;
                    Monitor.Enter(clientLock);
                    if (clientIndex == 0)
                    {
                        if (clientCount == clients.Length) uris.Push(uri);
                        else
                        {
                            ++clientCount;
                            newClient = true;
                        }
                    }
                    else client = clients[--clientIndex];
                    Monitor.Exit(clientLock);
                    if (newClient)
                    {
                        try
                        {
                            client = new htmlTitleClient(bufferSize, maxSearchSize);
                            client.IsValidateCertificate = isValidateCertificate;
                            client.timeoutQueue = timeoutQueue;
                        }
                        catch (Exception error)
                        {
                            log.Error.Add(error, null, false);
                        }
                        if (client == null)
                        {
                            Monitor.Enter(clientLock);
                            uris.Push(uri);
                            --clientCount;
                            Monitor.Exit(clientLock);
                        }
                    }
                    if (client != null) uri.Get(this, client);
                }
            }
            /// <summary>
            /// 添加HTML标题获取客户端
            /// </summary>
            /// <param name="client">HTML标题获取客户端</param>
            private void push(htmlTitleClient client)
            {
                Monitor.Enter(clientLock);
                if (uris.Head == null)
                {
                    if (isDisposed == 0)
                    {
                        clients[clientIndex++] = client;
                        client.IsValidateCertificate = isValidateCertificate;
                        Monitor.Exit(clientLock);
                    }
                    else
                    {
                        --clientCount;
                        Monitor.Exit(clientLock);
                        pub.Dispose(ref client);
                    }
                }
                else
                {
                    client.IsValidateCertificate = isValidateCertificate;
                    uriInfo uri = uris.Pop();
                    Monitor.Exit(clientLock);
                    uri.Get(this, client);
                }
            }
        }
        /// <summary>
        /// HTTP服务版本号
        /// </summary>
        private static readonly byte[] httpVersion = (@" HTTP/1.1
Connection: Close
User-Agent: Mozilla/5.0 (" + tcp.http.requestHeader.fastCSharpSpiderUserAgent + @")
Host: ").getBytes();
        /// <summary>
        /// HASH重定向名称
        /// </summary>
        private static readonly byte[] googleEscapedFragment = ("?_escaped_fragment_=").getBytes();
        /// <summary>
        /// 关闭套接字0超时设置
        /// </summary>
        private static readonly LingerOption lingerOption = new LingerOption(true, 0);
        /// <summary>
        /// 安全连接证书验证
        /// </summary>
        private RemoteCertificateValidationCallback validateCertificate;
        /// <summary>
        /// 证书验证完成处理
        /// </summary>
        private AsyncCallback validateCertificateCallback;
        /// <summary>
        /// 安全连接写入数据完成处理
        /// </summary>
        private AsyncCallback writeCallback;
        /// <summary>
        /// 安全连接读取数据完成处理
        /// </summary>
        private AsyncCallback readCallback;
        /// <summary>
        /// 请求域名缓冲区
        /// </summary>
        private byte[] hostBuffer;
        /// <summary>
        /// 收发数据缓冲区
        /// </summary>
        private byte[] buffer;
        /// <summary>
        /// 套接字
        /// </summary>
        private Socket socket;
        /// <summary>
        /// 异步连接操作
        /// </summary>
        private SocketAsyncEventArgs connectAsync;
        /// <summary>
        /// 异步发送操作
        /// </summary>
        private SocketAsyncEventArgs sendAsync;
        /// <summary>
        /// 异步接收操作
        /// </summary>
        private SocketAsyncEventArgs receiveAsync;
        /// <summary>
        /// 安全连接
        /// </summary>
        private SslStream sslStream;
        /// <summary>
        /// 获取HTML标题回调函数
        /// </summary>
        private Action<string> onGet;
        /// <summary>
        /// 用户指定编码
        /// </summary>
        private Encoding defaultEncoding;
        /// <summary>
        /// HTTP响应编码
        /// </summary>
        private Encoding responseEncoding;
        /// <summary>
        /// HTML页面编码
        /// </summary>
        private Encoding htmlEncoding;
        /// <summary>
        /// 收发数据缓冲区字节长度
        /// </summary>
        private int bufferSize;
        /// <summary>
        /// 最大搜索字节数
        /// </summary>
        private int maxSearchSize;
        /// <summary>
        /// 当前剩余搜索字节数
        /// </summary>
        private int currentSearchSize;
        /// <summary>
        /// 请求域名字节长度
        /// </summary>
        private int hostSize;
        /// <summary>
        /// 数据缓冲区有效位置
        /// </summary>
        private int bufferIndex;
        /// <summary>
        /// 当前处理位置
        /// </summary>
        private int currentIndex;
        /// <summary>
        /// 是否正在获取数据
        /// </summary>
        private int isGetting;
        /// <summary>
        /// 输出内容字节长度
        /// </summary>
        private int contentLength;
        /// <summary>
        /// 最后一次分段字节长度
        /// </summary>
        private int chunkedLength;
        /// <summary>
        /// 超时检测标识
        /// </summary>
        private int timeoutIdentity;
        /// <summary>
        /// 
        /// </summary>
        private timeoutQueue timeoutQueue;
        /// <summary>
        /// 安全连接错误信息
        /// </summary>
        private SslPolicyErrors? sslPolicyErrors;
        /// <summary>
        /// HTTP响应内容类型
        /// </summary>
        private bufferIndex contentType;
        /// <summary>
        /// HTML标题
        /// </summary>
        private bufferIndex title;
        /// <summary>
        /// 是否验证安全证书
        /// </summary>
        public bool IsValidateCertificate;
        /// <summary>
        /// 是否安全连接
        /// </summary>
        private bool isHttps;
        /// <summary>
        /// 是否压缩数据
        /// </summary>
        private bool isGzip;
        /// <summary>
        /// 是否已经解析头部
        /// </summary>
        private bool isHeader;
        /// <summary>
        /// 是否存在HTML节点
        /// </summary>
        private bool isHtml;
        /// <summary>
        /// 是否分段传输
        /// </summary>
        private bool isChunked;
        /// <summary>
        /// 是否关闭连接
        /// </summary>
        private bool isCloseConnection;
        /// <summary>
        /// 是否位置压缩编码类型
        /// </summary>
        private bool isUnknownEncoding;
        /// <summary>
        /// 是否重定向
        /// </summary>
        private bool isLocation;
        /// <summary>
        /// HTML标题获取客户端
        /// </summary>
        /// <param name="bufferSize">收发数据缓冲区字节数</param>
        /// <param name="maxSearchSize">最大搜索字节数</param>
        public htmlTitleClient(int bufferSize = 1 << 11, int maxSearchSize = 0)
        {
            if (bufferSize < (minBufferSize) || bufferSize > short.MaxValue)
            {
                log.Error.Throw(log.exceptionType.IndexOutOfRange);
            }
            buffer = new byte[bufferSize];
            this.maxSearchSize = Math.Max(this.bufferSize = bufferSize - sizeof(int), maxSearchSize);
            setAsync();
        }
        /// <summary>
        /// HTML标题获取客户端
        /// </summary>
        /// <param name="buffer">收发数据缓冲区</param>
        /// <param name="maxSearchSize">最大搜索字节数</param>
        public htmlTitleClient(byte[] buffer, int maxSearchSize = 0)
        {
            if (buffer.length() < (minBufferSize) || buffer.Length > short.MaxValue)
            {
                log.Error.Throw(log.exceptionType.IndexOutOfRange);
            }
            this.buffer = buffer;
            this.maxSearchSize = Math.Max(bufferSize = buffer.Length - sizeof(int), maxSearchSize);
            setAsync();
        }
        /// <summary>
        /// 设置异步操作
        /// </summary>
        private void setAsync()
        {
            hostBuffer = memoryPool.TinyBuffers.Get();
#if NOEXPAND
            connectAsync = socketAsyncEventArgs.Get();
            sendAsync = socketAsyncEventArgs.Get();
            receiveAsync = socketAsyncEventArgs.Get();
#else
            connectAsync = socketAsyncEventArgsProxy.Get();
            sendAsync = socketAsyncEventArgsProxy.Get();
            receiveAsync = socketAsyncEventArgsProxy.Get();
#endif
            connectAsync.UserToken = sendAsync.UserToken = receiveAsync.UserToken = this;
            connectAsync.Completed += onConnect;
            sendAsync.Completed += onSend;
            sendAsync.SetBuffer(buffer, 0, buffer.Length);
            receiveAsync.Completed += onReceive;
            receiveAsync.SetBuffer(buffer, 0, bufferSize);
            timeoutQueue = defaultTimeoutQueue;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            interlocked.CompareSetSleep1(ref isGetting);

            connectAsync.Completed -= onConnect;
            sendAsync.Completed -= onSend;
            receiveAsync.Completed -= onReceive;
#if NOEXPAND
            socketAsyncEventArgs.Push(ref connectAsync);
            socketAsyncEventArgs.Push(ref sendAsync);
            socketAsyncEventArgs.Push(ref receiveAsync);
#else
            socketAsyncEventArgsProxy.Push(ref connectAsync);
            socketAsyncEventArgsProxy.Push(ref sendAsync);
            socketAsyncEventArgsProxy.Push(ref receiveAsync);
#endif

            memoryPool.TinyBuffers.Push(ref hostBuffer);
        }
        /// <summary>
        /// 设置超时
        /// </summary>
        /// <param name="identity">超时标识</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void setTimeout(int identity)
        {
            if (identity == timeoutIdentity) timeoutQueue.Add(this, socket, identity);
        }
        /// <summary>
        /// 超时检测
        /// </summary>
        /// <param name="identity">超时标识</param>
        /// <returns>是否超时</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool CheckTimeout(int identity)
        {
            return identity == timeoutIdentity;
        }
        /// <summary>
        /// 获取HTML标题
        /// </summary>
        /// <param name="uri">Uri</param>
        /// <param name="onGet">获取HTML标题回调函数</param>
        /// <param name="encoding">默认编码格式</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Get(string uri, Action<string> onGet, Encoding encoding = null)
        {
            if (onGet == null) log.Error.Throw(log.exceptionType.Null);
            if (uri.length() < minUriSize) onGet(null);
            else get(uri, onGet, encoding);
        }
        /// <summary>
        /// 获取HTML标题
        /// </summary>
        /// <param name="uri">Uri</param>
        /// <param name="onGet">获取HTML标题回调函数</param>
        /// <param name="encoding">默认编码格式</param>
        private unsafe void get(string uri, Action<string> onGet, Encoding encoding)
        {
            memoryPool pool = memoryPool.GetDefaultPool(uri.Length);
            byte[] data = pool.Get(uri.Length);
            try
            {
                fixed (char* uriFixed = uri)
                fixed (byte* dataFixed = data)
                {
                    unsafer.String.WriteBytes(uriFixed, uri.Length, dataFixed);
                }
                subArray<byte> uriArray = subArray<byte>.Unsafe(data, 0, uri.Length);
                get(ref uriArray, onGet, encoding, 0, false);
            }
            finally { pool.PushNotNull(data); }
        }
        /// <summary>
        /// 获取HTML标题
        /// </summary>
        /// <param name="uri">Uri</param>
        /// <param name="onGet">获取HTML标题回调函数</param>
        /// <param name="encoding">默认编码格式</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Get(ref subArray<byte> uri, Action<string> onGet, Encoding encoding = null)
        {
            if (onGet == null) log.Error.Throw(log.exceptionType.Null);
            if (uri.Count < minUriSize) onGet(null);
            else get(ref uri, onGet, encoding, 0, false);
        }
        /// <summary>
        /// 获取HTML标题
        /// </summary>
        /// <param name="uri">Uri</param>
        /// <param name="onGet">获取HTML标题回调函数</param>
        /// <param name="encoding">默认编码格式</param>
        /// <param name="defaultPort">默认请求端口</param>
        /// <param name="isLocation">是否重定向请求</param>
        private unsafe void get(ref subArray<byte> uri, Action<string> onGet, Encoding encoding, int defaultPort, bool isLocation)
        {
            fixed (byte* uriFixed = uri.UnsafeArray)
            {
                byte* start = uriFixed + uri.StartIndex;
                int hostIndex = 0;
                if ((*(int*)start | 0x20202020) == 'h' + ('t' << 8) + ('t' << 16) + ('p' << 24))
                {
                    int code = *(int*)(start + sizeof(int));
                    if ((code & 0xffffff) == (':' + ('/' << 8) + ('/' << 16))) hostIndex = 7;
                    else if ((code | 0x20) == 's' + (':' << 8) + ('/' << 16) + ('/' << 24)) hostIndex = 8;
                }
                if (hostIndex != 0)
                {
                    byte* end = start + uri.Count, nameStart = (start += hostIndex);
                    while (start != end && *start != '/' && *start != '?' && *start != '#' && *start != ':') ++start;
                    int hostSize = (int)(start - nameStart);
                    if (hostSize <= hostBuffer.Length)
                    {
                        subArray<byte> host = subArray<byte>.Unsafe(uri.UnsafeArray, (int)(nameStart - uriFixed), hostSize);
                        IPAddress[] ips = httpClient.GetIPAddress(ref host);
                        if (ips.length() != 0)
                        {
                            int port;
                            if (start != end && *start == ':')
                            {
                                port = 0;
                                while (++start != end)
                                {
                                    byte value = *start;
                                    if ((value -= (byte)'0') < 10)
                                    {
                                        port *= 10;
                                        port += value;
                                    }
                                    else break;
                                }
                                if (start != end && *start != '/' && *start != '?' && *start != '#') port = 0;
                            }
                            else port = defaultPort == 0 ? (hostIndex == 8 ? 443 : 80) : defaultPort;
                            if (port != 0)
                            {
                                subArray<byte> path = default(subArray<byte>), hash = default(subArray<byte>);
                                for (nameStart = start; start != end && *start != '?' && *start != '#'; ++start) ;
                                if (start == end) path.UnsafeSet(uri.UnsafeArray, (int)(nameStart - uriFixed), (int)(start - nameStart));
                                else
                                {
                                    if (*start == '?')
                                    {
                                        while (++start != end && *start != '#') ;
                                        path.UnsafeSet(uri.UnsafeArray, (int)(nameStart - uriFixed), (int)(start - nameStart));
                                    }
                                    else
                                    {
                                        path.UnsafeSet(uri.UnsafeArray, (int)(nameStart - uriFixed), (int)(start - nameStart));
                                        if (++start != end || *start == '!')
                                        {
                                            hash.UnsafeSet(uri.UnsafeArray, (int)(++start - uriFixed), (int)(end - start));
                                        }
                                    }
                                }
                                //int urlSize = path.StartIndex + path.Count - uri.StartIndex;
                                //if (urlSize <= fastCSharp.memoryPool.TinyBuffers.Size)
                                //{

                                //}
                                if (Interlocked.CompareExchange(ref isGetting, 1, 0) == 0)
                                {
                                    int index = sizeof(int) * 2 + path.Count + httpVersion.Length + hostSize, hashCount = 0; 
                                    if (hash.Count != 0)
                                    {
                                        index += googleEscapedFragment.Length + hash.Count;
                                        for (start = uriFixed + hash.StartIndex, end = start + hash.Count; start != end; ++start)
                                        {
                                            if ((uint)(*start - '0') >= 10 && (uint)((*start | 0x20) - 'a') >= 26) ++hashCount;
                                        }
                                        index += hashCount << 1;
                                    }
                                    if (index < buffer.Length)
                                    {
                                        fixed (byte* bufferFixed = buffer)
                                        {
                                            *(int*)bufferFixed = 'G' + ('E' << 8) + ('T' << 16) + (' ' << 24);
                                            if (path.Count == 0)
                                            {
                                                bufferFixed[sizeof(int)] = (byte)'/';
                                                index = sizeof(int) + 1;
                                            }
                                            else
                                            {
                                                if (uriFixed[path.StartIndex] != '/')
                                                {
                                                    bufferFixed[sizeof(int)] = (byte)'/';
                                                    index = sizeof(int) + 1;
                                                }
                                                else index = sizeof(int);
                                                Buffer.BlockCopy(uri.UnsafeArray, path.StartIndex, buffer, index, path.Count);
                                                index += path.Count;
                                                if (hash.Count != 0)
                                                {
                                                    Buffer.BlockCopy(googleEscapedFragment, 0, buffer, index, googleEscapedFragment.Length);
                                                    index += googleEscapedFragment.Length;
                                                    if (hashCount == 0)
                                                    {
                                                        Buffer.BlockCopy(uri.UnsafeArray, hash.StartIndex, buffer, index, hash.Count);
                                                        index += hash.Count;
                                                    }
                                                    else
                                                    {
                                                        nameStart = bufferFixed + index;
                                                        for (start = uriFixed + hash.StartIndex, end = start + hash.Count; start != end; ++start)
                                                        {
                                                            if ((uint)(*start - '0') >= 10 && (uint)((*start | 0x20) - 'a') >= 26)
                                                            {
                                                                *nameStart++ = (byte)'%';
                                                                *nameStart++ = (byte)numberExpand.ToHex((uint)*start >> 4);
                                                                *nameStart++ = (byte)numberExpand.ToHex((uint)*start & 0xf);
                                                            }
                                                            else *nameStart++ = *start;
                                                        }
                                                        index = (int)(nameStart - bufferFixed);
                                                    }
                                                }
                                            }
                                            Buffer.BlockCopy(httpVersion, 0, buffer, index, httpVersion.Length);
                                            index += httpVersion.Length;
                                            Buffer.BlockCopy(uri.UnsafeArray, host.StartIndex, buffer, index, hostSize);
                                            *(int*)(bufferFixed + (index += hostSize)) = 0x0a0d0a0d;
                                            index += sizeof(int);
                                        }
                                        bufferIndex = index;
                                        isHttps = hostIndex == 8;
                                        try
                                        {
                                            IPAddress ip = ips[0];
                                            socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                                            if (!isHttps)
                                            {
                                                socket.SendBufferSize = minBufferSize;
                                                socket.ReceiveBufferSize = Math.Min(4 << 10, buffer.Length);
                                            }
                                            connectAsync.RemoteEndPoint = new IPEndPoint(ip, port);
                                            this.onGet = onGet;
                                            this.isLocation = isLocation;
                                            defaultEncoding = encoding;
                                            Buffer.BlockCopy(uri.UnsafeArray, host.StartIndex, hostBuffer, 0, this.hostSize = hostSize);
                                            socket.LingerState = lingerOption;
                                            int timeoutIdentity = this.timeoutIdentity;
                                            if (socket.ConnectAsync(connectAsync))
                                            {
                                                setTimeout(timeoutIdentity);
                                                return;
                                            }
                                            else this.onGet = null;
                                        }
                                        catch (Exception error)
                                        {
                                            fastCSharp.threading.disposeTimer.Default.AddSocketClose(ref socket);
                                            log.Error.Add(error, null, false);
                                        }
                                    }
                                    isGetting = 0;
                                }
                            }
                        }
                    }
                }
            }
            onGet(null);
        }
        /// <summary>
        /// 获取HTML标题回调
        /// </summary>
        /// <param name="title">HTML标题</param>
        private void callback(string title)
        {
            Action<string> onGet = this.onGet;
            this.onGet = null;
            pub.Dispose(ref sslStream);
            fastCSharp.threading.disposeTimer.Default.AddSocketClose(ref socket);
            isGetting = 0;
            onGet(title);
        }
        /// <summary>
        /// 获取HTML标题回调
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <param name="encoding">HTML标题编码</param>
        private void callback(byte[] buffer, Encoding encoding)
        {
            string title = encoding.GetString(buffer, this.title.StartIndex, this.title.Length - this.title.StartIndex);
            try
            {
                callback(title);
            }
            catch (Exception error)
            {
                log.Error.Add(error, null, false);
            }
        }
        /// <summary>
        /// 套接字连接结束操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="async"></param>
        private unsafe void onConnect(object sender, SocketAsyncEventArgs async)
        {
            int timeoutIdentity = ++this.timeoutIdentity;
            if (async.SocketError == SocketError.Success)
            {
                try
                {
                    if (isHttps)
                    {
                        string host;
                        fixed (byte* hostFixed = hostBuffer) host = String.UnsafeDeSerialize(hostFixed, -hostSize);
                        if (validateCertificate == null)
                        {
                            validateCertificate = onValidateCertificate;
                            validateCertificateCallback = onValidateCertificate;
                        }
                        sslPolicyErrors = null;
                        sslStream = new SslStream(new NetworkStream(socket, true), false, validateCertificate, null);
                        sslStream.BeginAuthenticateAsClient(host, validateCertificateCallback, this);
                        setTimeout(timeoutIdentity);
                        return;
                    }
                    sendAsync.SetBuffer(0, bufferIndex);
                    if (socket.SendAsync(sendAsync))
                    {
                        setTimeout(timeoutIdentity);
                        return;
                    }
                }
                catch (Exception error)
                {
                    log.Default.Add(error, null, false);
                }
            }
            //else log.Default.Add(connectAsync.RemoteEndPoint.ToString() + " " + async.SocketError.ToString(), false, false);
            callback(null);
        }
        /// <summary>
        /// 安全连接证书验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        private bool onValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            this.sslPolicyErrors = sslPolicyErrors;
            return !IsValidateCertificate || sslPolicyErrors == SslPolicyErrors.None;
        }
        /// <summary>
        /// 证书验证完成处理
        /// </summary>
        /// <param name="result">异步操作状态</param>
        private void onValidateCertificate(IAsyncResult result)
        {
            int timeoutIdentity = ++this.timeoutIdentity;
            try
            {
                if (!IsValidateCertificate || sslPolicyErrors == SslPolicyErrors.None)
                {
                    sslStream.EndAuthenticateAsClient(result);
                    if (writeCallback == null)
                    {
                        writeCallback = onWrite;
                        readCallback = onRead;
                    }
                    socket.SendBufferSize = minBufferSize;
                    socket.ReceiveBufferSize = Math.Min(4 << 10, buffer.Length);
                    sslStream.BeginWrite(buffer, 0, bufferIndex, writeCallback, this);
                    setTimeout(timeoutIdentity);
                    return;
                }
            }
            catch (Exception error)
            {
                log.Default.Add(error, null, false);
            }
            callback(null);
        }
        /// <summary>
        /// 发送数据操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="async"></param>
        private void onSend(object sender, SocketAsyncEventArgs async)
        {
            int timeoutIdentity = ++this.timeoutIdentity;
            if (async.SocketError == SocketError.Success)
            {
                if (async.BytesTransferred == bufferIndex)
                {
                    try
                    {
                        isHeader = false;
                        receiveAsync.SetBuffer(bufferIndex = currentIndex = 0, bufferSize);
                        if (socket.ReceiveAsync(receiveAsync))
                        {
                            setTimeout(timeoutIdentity);
                            return;
                        }
                    }
                    catch (Exception error)
                    {
                        log.Default.Add(error, null, false);
                    }
                }
            }
            //else log.Default.Add("onSend " + async.SocketError.ToString(), false, false);
            callback(null);
        }
        /// <summary>
        /// 安全连接写入数据完成处理
        /// </summary>
        /// <param name="result"></param>
        private void onWrite(IAsyncResult result)
        {
            int timeoutIdentity = ++this.timeoutIdentity;
            try
            {
                sslStream.EndWrite(result);
                isHeader = false;
                sslStream.BeginRead(buffer, bufferIndex = currentIndex = 0, bufferSize, readCallback, this);
                setTimeout(timeoutIdentity);
                return;
            }
            catch (Exception error)
            {
                log.Default.Add(error, null, false);
            }
            callback(null);
        }
        /// <summary>
        /// 接收数据操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="async"></param>
        private void onReceive(object sender, SocketAsyncEventArgs async)
        {
            ++timeoutIdentity;
            if (async.SocketError == SocketError.Success)
            {
                try
                {
                    if (onReceive(async.BytesTransferred)) return;
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
            }
            //else log.Default.Add("onReceive " + async.SocketError.ToString(), false, false);
            callback(null);
        }
        /// <summary>
        /// 安全连接读取数据完成处理
        /// </summary>
        /// <param name="result"></param>
        private void onRead(IAsyncResult result)
        {
            ++timeoutIdentity;
            try
            {
                if (onReceive(sslStream.EndRead(result))) return;
            }
            catch (Exception error)
            {
                log.Error.Add(error, null, false);
            }
            callback(null);
        }
        /// <summary>
        /// 接收数据操作
        /// </summary>
        /// <param name="count">接收数据字节数</param>
        /// <returns>是否处理完毕</returns>
        private unsafe bool onReceive(int count)
        {
            if (count > 0)
            {
                bufferIndex += count;
                if (isHeader)
                {
                    fixed (byte* bufferFixed = buffer)
                    {
                        if (isChunked)
                        {
                            checkChunked(bufferFixed);
                            if (bufferIndex > contentLength && !isChunked) return false;
                        }
                        if (isGzip)
                        {
                            if (bufferIndex == contentLength && !isChunked) return parseGZip();
                        }
                        else if (parseTitle(buffer)) return true;
                        else if (bufferIndex == bufferSize && isHtml && currentSearchSize > 0)
                        {
                            int index = currentIndex;
                            if (title.StartIndex != 0 && index > title.StartIndex) index = title.StartIndex - 1;
                            if (index == 0)
                            {
                                if (isHeader && title.StartIndex != 0 && title.Length != 0)
                                {
                                    if (responseEncoding != null) callback(buffer, responseEncoding);
                                    else if (defaultEncoding != null) callback(buffer, defaultEncoding);
                                    else callback(buffer, chineseEncoder.ChineseEncoding(bufferFixed, isChunked || contentLength > bufferSize ? bufferSize : contentLength) ?? Encoding.UTF8);
                                    return true;
                                }
                                return false;
                            }
                            Buffer.BlockCopy(buffer, index, buffer, 0, bufferIndex -= index);
                            currentSearchSize -= index;
                            currentIndex -= index;
                            if (title.StartIndex != 0)
                            {
                                title.StartIndex -= (short)index;
                                if (title.Length != 0) title.Length -= (short)index;
                            }
                        }
                    }
                }
                else
                {
                    int searchEndIndex = bufferIndex - sizeof(int);
                    if (currentIndex <= searchEndIndex)
                    {
                        fixed (byte* bufferFixed = buffer)
                        {
                            byte* start = bufferFixed + currentIndex, searchEnd = bufferFixed + searchEndIndex, end = bufferFixed + bufferIndex;
                            *end = 13;
                            do
                            {
                                while (*start != 13) ++start;
                                if (start <= searchEnd)
                                {
                                    if (*(int*)start == 0x0a0d0a0d)
                                    {
                                        currentIndex = (int)(start - bufferFixed);
                                        bool isLocation = false;
                                        if (parseHeader(bufferFixed, ref isLocation))
                                        {
                                            if (isLocation) return true;
                                            if ((count = bufferIndex - (currentIndex += sizeof(int))) <= contentLength || isChunked)
                                            {
                                                Buffer.BlockCopy(buffer, currentIndex, buffer, 0, count);
                                                currentIndex = 0;
                                                bufferIndex = count;
                                                if (isChunked)
                                                {
                                                    chunkedLength = int.MinValue;
                                                    checkChunked(bufferFixed);
                                                    if (bufferIndex > contentLength && !isChunked) return false;
                                                }
                                                htmlEncoding = null;
                                                title.Null();
                                                isHeader = true;
                                                isHtml = false;
                                                currentSearchSize = maxSearchSize - bufferSize;
                                                if (isGzip)
                                                {
                                                    if (bufferIndex == contentLength && !isChunked) return parseGZip();
                                                }
                                                else if (parseTitle(buffer)) return true;
                                                break;
                                            }
                                        }
                                        return false;
                                    }
                                    ++start;
                                }
                                else
                                {
                                    currentIndex = (int)(start - bufferFixed);
                                    break;
                                }
                            }
                            while (true);
                        }
                    }
                }
                if ((count = (!isHeader || isChunked || contentLength > bufferSize ? bufferSize : contentLength) - bufferIndex) > 0)
                {
                    int timeoutIdentity = this.timeoutIdentity;
                    if (isHttps)
                    {
                        sslStream.BeginRead(buffer, bufferIndex, count, readCallback, this);
                        setTimeout(timeoutIdentity);
                        return true;
                    }
                    receiveAsync.SetBuffer(bufferIndex, count);
                    if (socket.ReceiveAsync(receiveAsync))
                    {
                        setTimeout(timeoutIdentity);
                        return true;
                    }
                }
                if (isGzip) return parseGZip();
            }
            return false;
        }
        /// <summary>
        /// 解析头部数据
        /// </summary>
        /// <param name="bufferFixed">数据起始位置</param>
        /// <param name="isLocation">是否重定向处理</param>
        /// <returns>是否成功</returns>
        private unsafe bool parseHeader(byte* bufferFixed, ref bool isLocation)
        {
            byte* end = bufferFixed + currentIndex, current = bufferFixed;
            for (*end = 32; *current != 32; ++current) ;
            if (current == end) return false;
            *end = 13;
            while (*++current == 32) ;
            if (current == end) return false;
            int state = 0;
            do
            {
                uint number = (uint)(*current - '0');
                if (number >= 10) break;
                state *= 10;
                state += (int)number;
                ++current;
            }
            while (true);
            while (*current != 13) ++current;
            if (state == 200)
            {
                contentType.Null();
                contentLength = 0;
                isGzip = isUnknownEncoding = isChunked = isCloseConnection = false;
                while (current != end)
                {
                    if ((current += 2) >= end) return false;
                    byte* start = current;
                    for (*end = (byte)':'; *current != (byte)':'; ++current) ;
                    if (current == end) return false;
                    subArray<byte> name = subArray<byte>.Unsafe(buffer, (int)(start - bufferFixed), (int)(current - start));
                    *end = 13;
                    while (*++current == ' ');
                    for (start = current; *current != 13; ++current) ;
                    Action<htmlTitleClient, bufferIndex> parseHeaderName = parses.Get(name, null);
                    if (parseHeaderName != null) parseHeaderName(this, new bufferIndex { StartIndex = (short)(start - bufferFixed), Length = (short)(current - start) });
                }
                if (!isChunked && isCloseConnection && contentLength == 0) contentLength = int.MaxValue;
                if ((isChunked ? contentLength == 0 : (contentLength > 0 && (!isGzip || contentLength <= bufferSize))) && !isUnknownEncoding)
                {
                    if (contentType.Length == 0) return true;
                    int isHtml = 0, isEncoding = 0;
                    current = bufferFixed + contentType.StartIndex;
                    end = current + contentType.Length;
                    while (current != end)
                    {
                        if (isHtml == 0)
                        {
                            if ((((*(int*)current | 0x20202020) ^ ('t' + ('e' << 8) + ('x' << 16) + ('t' << 24)))
                                | ((*(int*)(current + sizeof(int)) | 0x20202000) ^ ('/' + ('h' << 8) + ('t' << 16) + ('m' << 24)))
                                | ((*(current + sizeof(int) * 2) | 0x20) ^ 'l')) == 0)
                            {
                                isHtml = 1;
                                if (defaultEncoding != null || isEncoding != 0) return true;
                                current += 9;
                            }
                        }
                        if (isEncoding == 0)
                        {
                            if ((((*(int*)current | 0x20202020) ^ ('c' + ('h' << 8) + ('a' << 16) + ('r' << 24)))
                                | ((*(int*)(current + sizeof(int)) | 0x202020) ^ ('s' + ('e' << 8) + ('t' << 16) + ('=' << 24)))) == 0)
                            {
                                if ((current += 8) >= end) return false;
                                isEncoding = 1;
                                byte* start = current;
                                while (*current != 13 && *current != ';') ++current;
                                try
                                {
                                    responseEncoding = Encoding.GetEncoding(String.UnsafeDeSerialize(start, (int)(start - current)));
                                }
                                catch { }
                                if (isHtml != 0) return true;
                            }
                        }
                        for (*end = (byte)';'; *current != ';'; ++current) ;
                        if (current == end) break;
                        while (*++current == ' ');
                    }
                    if (isHtml != 0) return true;
                }
                return false;
            }
            if (!this.isLocation && (state == 301 || state == 302))
            {
                this.isLocation = true;
                while (current != end)
                {
                    if ((current += 2) >= end) return false;
                    if (current + 10 < end && (((*(int*)current | 0x20202020) ^ ('l' + ('o' << 8) + ('c' << 16) + ('a' << 24)))
                        | ((*(int*)(current + sizeof(int)) | 0x20202020) ^ ('t' + ('i' << 8) + ('o' << 16) + ('n' << 24)))
                        | (*(short*)(current + sizeof(int) * 2) ^ (':' + (' ' << 8)))) == 0)
                    {
                        if (*(current += 10) == '/' || (((*(int*)current | 0x20202020) ^ ('h' + ('t' << 8) + ('t' << 16) + ('p' << 24)))
                            | ((*(int*)(current + sizeof(int)) & 0xffffff) ^ (':' + ('/' << 8) + ('/' << 16)))) == 0)
                        {
                            byte* start = current;
                            while (*++current != 13);
                            int length = (int)(current - start);
                            if (*start == '/')
                            {
                                int hostIndex = isHttps ? 8 : 7, uriLength = length + hostSize + hostIndex, port = ((IPEndPoint)connectAsync.RemoteEndPoint).Port;
                                memoryPool pool = memoryPool.GetDefaultPool(uriLength);
                                byte[] uri = pool.Get(uriLength);
                                try
                                {
                                    fixed (byte* uriFixed = uri)
                                    {
                                        *(int*)uriFixed = 'h' + ('t' << 8) + ('t' << 16) + ('p' << 24);
                                        *(int*)(uriFixed + sizeof(int)) = isHttps ? ('s' + (':' << 8) + ('/' << 16) + ('/' << 24)) : (':' + ('/' << 8) + ('/' << 16));
                                    }
                                    Buffer.BlockCopy(hostBuffer, 0, uri, hostIndex, hostSize);
                                    Buffer.BlockCopy(buffer, (int)(start - bufferFixed), uri, hostSize + hostIndex, length);
                                    Action<string> onGet = this.onGet;
                                    Encoding encoding = defaultEncoding;
                                    fastCSharp.threading.disposeTimer.Default.AddSocketClose(ref socket);
                                    this.onGet = null;
                                    defaultEncoding = null;
                                    isGetting = 0;
                                    subArray<byte> uriArray = subArray<byte>.Unsafe(uri, 0, uriLength);
                                    get(ref uriArray, onGet, encoding, port, true);
                                }
                                finally { pool.PushNotNull(uri); }
                            }
                            else
                            {
                                memoryPool pool = memoryPool.GetDefaultPool(length);
                                byte[] uri = pool.Get(length);
                                try
                                {
                                    Buffer.BlockCopy(buffer, (int)(start - bufferFixed), uri, 0, length);
                                    Action<string> onGet = this.onGet;
                                    Encoding encoding = defaultEncoding;
                                    fastCSharp.threading.disposeTimer.Default.AddSocketClose(ref socket);
                                    this.onGet = null;
                                    defaultEncoding = null;
                                    isGetting = 0;
                                    subArray<byte> uriArray = subArray<byte>.Unsafe(uri, 0, length);
                                    get(ref uriArray, onGet, encoding, 80, true);
                                }
                                finally { pool.PushNotNull(uri); }
                            }
                            isLocation = true;
                            return true;
                        }
                    }
                    while (*current != 13) ++current;
                }
            }
            return false;
        }
        /// <summary>
        /// 检测分段传输长度
        /// </summary>
        /// <param name="bufferFixed">数据起始位置</param>
        private unsafe void checkChunked(byte* bufferFixed)
        {
            int startIndex = contentLength, copyIndex = contentLength;
            if (chunkedLength == int.MinValue)
            {
                if (bufferIndex < 3) return;
                byte* start = bufferFixed, end = bufferFixed + bufferIndex;
                *end = 13;
                int length = 0;
                do
                {
                    uint code = (uint)(*start - '0');
                    if (code < 10)
                    {
                        length <<= 4;
                        length += (int)code;
                    }
                    else if ((code = (uint)((*start | 0x20) - 'a')) <= ('f' - 'a'))
                    {
                        length <<= 4;
                        length += (int)code + 10;
                    }
                    else
                    {
                        int count = (int)(end - start);
                        if (count < 2) return;
                        if (*(short*)start != 0x0a0d)// || (isGzip && length > bufferSize)
                        {
                            isChunked = false;
                            contentLength = 0;
                            return;
                        }
                        chunkedLength = contentLength = length;
                        int index = (int)(start - bufferFixed) + 2;
                        if ((count -= 2) < length)
                        {
                            Buffer.BlockCopy(buffer, index, buffer, 0, bufferIndex -= index);
                            return;
                        }
                        Buffer.BlockCopy(buffer, index, buffer, 0, length);
                        startIndex = length;
                        copyIndex = length + index;
                        break;
                    }
                    ++start;
                }
                while (true);
            }
            do
            {
                int count = bufferIndex - copyIndex;
                if (count < 2) goto COPY;
                byte* start = bufferFixed + copyIndex;
                if (*(short*)start != 0x0a0d)
                {
                    contentLength = 0;
                    isChunked = false;
                    return;
                }
                if (chunkedLength == 0)
                {
                    if (count == 2) bufferIndex = contentLength;
                    else contentLength = 0;
                    isChunked = false;
                    return;
                }
                if (count < 5) goto COPY;
                byte* end = bufferFixed + bufferIndex;
                start += 2;
                *end = 13;
                int length = 0;
                do
                {
                    uint code = (uint)(*start - '0');
                    if (code < 10)
                    {
                        length <<= 4;
                        length += (int)code;
                    }
                    else if ((code = (uint)((*start | 0x20) - 'a')) <= ('f' - 'a'))
                    {
                        length <<= 4;
                        length += (int)code + 10;
                    }
                    else
                    {
                        if ((count = (int)(end - start)) < 2) goto COPY;
                        if (*(short*)start != 0x0a0d)// || ((contentLength + length) > bufferSize && isGzip)
                        {
                            isChunked = false;
                            contentLength = 0;
                            return;
                        }
                        contentLength += length;
                        chunkedLength = length;
                        int index = (int)(start - bufferFixed) + 2;
                        if ((count -= 2) < length)
                        {
                            Buffer.BlockCopy(buffer, index, buffer, startIndex, count);
                            bufferIndex = startIndex + count;
                            return;
                        }
                        Buffer.BlockCopy(buffer, index, buffer, startIndex, length);
                        startIndex += length;
                        copyIndex = length + index;
                        break;
                    }
                    ++start;
                }
                while (true);
            }
            while (true);
        COPY:
            if (startIndex != copyIndex)
            {
                int count = bufferIndex - copyIndex;
                Buffer.BlockCopy(buffer, copyIndex, buffer, startIndex, count);
                bufferIndex = startIndex + count;
            }
        }
        /// <summary>
        /// 解析HTML标题
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <returns>是否成功</returns>
        private unsafe bool parseTitle(byte[] buffer)
        {
            fixed (byte* bufferFixed = buffer)
            {
                byte* current = bufferFixed + currentIndex, end = bufferFixed + bufferIndex;
                do
                {
                    byte* start = current;
                    for (*end = (byte)'<'; *current != '<'; ++current) ;
                    if (current == end)
                    {
                        if ((title.StartIndex | title.Length) == 0) currentIndex = isChunked ? Math.Min(contentLength, bufferIndex) : bufferIndex;
                        return false;
                    }
                    if (title.StartIndex != 0 && title.Length == 0)
                    {
                        title.Length = (short)(current - bufferFixed);
                        if (htmlEncoding != null)
                        {
                            callback(buffer, htmlEncoding);
                            return true;
                        }
                        bool isAscii = true;
                        while (*start != '<')
                        {
                            if ((uint)(*start++ - 32) > (126 - 32))
                            {
                                isAscii = false;
                                break;
                            }
                        }
                        if (isAscii)
                        {
                            callback(buffer, Encoding.ASCII);
                            return true;
                        }
                    }
                    currentIndex = (int)(current - bufferFixed);
                    for (*end = (byte)'>', start = current; *current != '>'; ++current) ;
                    if (current == end) return false;
                    int tagName = *(int*)++start | 0x20202020;
                    if (tagName == 'm' + ('e' << 8) + ('t' << 16) + ('a' << 24) && htmlEncoding == null)
                    {
                        *current = (byte)'=';
                        start += sizeof(int);
                        do
                        {
                            while (*start != '=') ++start;
                            if (start == current) break;
                            if ((*(int*)(++start - sizeof(int)) | 0x202020) == 's' + ('e' << 8) + ('t' << 16) + ('=' << 24)
                                && (*(int*)(start - sizeof(int) * 2) | 0x20202020) == 'c' + ('h' << 8) + ('a' << 16) + ('r' << 24))
                            {
                                while (*start == '"' || *start == '\'') ++start;
                                byte* encoding = start;
                                while ((uint)((*start | 0x20) - 'a') < 26 || (uint)(*start - '0') < 10 || *start == '-') ++start;
                                try
                                {
                                    htmlEncoding = Encoding.GetEncoding(String.UnsafeDeSerialize(encoding, (int)(encoding - start)));
                                }
                                catch { }
                                if (htmlEncoding != null && title.StartIndex != 0)
                                {
                                    callback(buffer, htmlEncoding);
                                    return true;
                                }
                                break;
                            }
                        }
                        while (true);
                    }
                    else if (tagName == 'h' + ('t' << 8) + ('m' << 16) + ('l' << 24)) isHtml = true;
                    else if (tagName == 't' + ('i' << 8) + ('t' << 16) + ('l' << 24))
                    {
                        if ((*(start + sizeof(int)) | 0x20) == 'e') title.StartIndex = (short)((int)(current - bufferFixed) + 1);
                    }
                    else if (tagName == 'O' + ('h' << 8) + ('e' << 16) + ('a' << 24)
                        || tagName == 'b' + ('o' << 8) + ('d' << 16) + ('y' << 24))
                    {
                        if (title.StartIndex == 0) callback(null);
                        else if (responseEncoding != null) callback(buffer, responseEncoding);
                        else if (defaultEncoding != null) callback(buffer, defaultEncoding);
                        else
                        {
                            int length = (int)(start - bufferFixed);
                            callback(buffer, chineseEncoder.ChineseEncoding(bufferFixed, length)
                                ?? chineseEncoder.ChineseEncoding(start, bufferIndex - length)
                                ?? Encoding.UTF8);
                        }
                        return true;
                    }
                    currentIndex = (int)(++current - bufferFixed);
                }
                while (true);
            }
        }
        /// <summary>
        /// 解析gzip+HTML标题
        /// </summary>
        /// <returns>是否成功</returns>
        private unsafe bool parseGZip()
        {
            if (bufferIndex > 0)
            {
                subArray<byte> data = fastCSharp.io.compression.stream.GZip.GetDeCompress(buffer, 0, bufferIndex, fastCSharp.memoryPool.StreamBuffers);
                byte[] newData = data.UnsafeArray;
                if (newData != null)
                {
                    try
                    {
                        if ((bufferIndex = data.Count) != 0)
                        {
                            currentIndex = 0;
                            if (parseTitle(newData)) return true;
                        }
                    }
                    finally { fastCSharp.memoryPool.StreamBuffers.PushNotNull(newData); }
                }
            }
            return false;
        }

        /// <summary>
        /// 提交内容数据长度解析
        /// </summary>
        /// <param name="client">HTML标题获取客户端</param>
        /// <param name="value">提交内容数据长度索引位置</param>
        private unsafe static void parseContentLength(htmlTitleClient client, bufferIndex value)
        {
            fixed (byte* dataFixed = client.buffer)
            {
                for (byte* start = dataFixed + value.StartIndex, end = start + value.Length; start != end; ++start)
                {
                    client.contentLength *= 10;
                    client.contentLength += *start - '0';
                }
            }
        }
        /// <summary>
        /// HTTP响应内容类型解析
        /// </summary>
        /// <param name="client">HTML标题获取客户端</param>
        /// <param name="value">HTTP响应内容类型索引位置</param>
        private static void parseContentType(htmlTitleClient client, bufferIndex value)
        {
            client.contentType = value;
        }
        /// <summary>
        /// HTTP响应压缩编码类型解析
        /// </summary>
        /// <param name="client">HTML标题获取客户端</param>
        /// <param name="value">HTTP响应压缩编码类型索引位置</param>
        private unsafe static void parseContentEncoding(htmlTitleClient client, bufferIndex value)
        {
            if (value.Length == 4)
            {
                fixed (byte* bufferFixed = client.buffer)
                {
                    if ((*(int*)(bufferFixed + value.StartIndex) | 0x20202020) == ('g' + ('z' << 8) + ('i' << 16) + ('p' << 24)))
                    {
                        client.isGzip = true;
                        return;
                    }
                }
            }
            client.isUnknownEncoding = true;
        }
        /// <summary>
        /// HTTP响应传输编码类型解析
        /// </summary>
        /// <param name="client">HTML标题获取客户端</param>
        /// <param name="value">HTTP响应传输编码类型索引位置</param>
        private unsafe static void parseTransferEncoding(htmlTitleClient client, bufferIndex value)
        {
            if (value.Length == 7)
            {
                fixed (byte* bufferFixed = client.buffer)
                {
                    byte* start = bufferFixed + value.StartIndex;
                    if ((((*(int*)start | 0x20202020) ^ ('c' + ('h' << 8) + ('u' << 16) + ('n' << 24)))
                        | ((*(int*)(start + 3) | 0x20202020) ^ ('n' + ('k' << 8) + ('e' << 16) + ('d' << 24)))) == 0)
                    {
                        client.isChunked = true;
                    }
                }
            }
        }
        /// <summary>
        /// HTTP连接状态解析
        /// </summary>
        /// <param name="client">HTML标题获取客户端</param>
        /// <param name="value">HTTP连接状态索引位置</param>
        private unsafe static void parseConnection(htmlTitleClient client, bufferIndex value)
        {
            if (value.Length == 5)
            {
                fixed (byte* bufferFixed = client.buffer)
                {
                    byte* start = bufferFixed + value.StartIndex;
                    if ((((*(int*)start | 0x20202020) ^ ('c' + ('l' << 8) + ('o' << 16) + ('s' << 24)))
                        | ((*(start + sizeof(int)) | 0x20) ^ ('e'))) == 0)
                    {
                        client.isCloseConnection = true;
                    }
                }
            }
        }
        /// <summary>
        /// HTTP头名称唯一哈希
        /// </summary>
        private struct headerName : IEquatable<headerName>
        {
            //string[] keys = new string[] { "transfer-encoding", "content-length", "content-type","content-encoding", "connection" };
            /// <summary>
            /// HTTP头名称
            /// </summary>
            public subArray<byte> Name;
            /// <summary>
            /// 隐式转换
            /// </summary>
            /// <param name="name">HTTP头名称</param>
            /// <returns>HTTP头名称唯一哈希</returns>
            public static implicit operator headerName(byte[] name) { return new headerName { Name = subArray<byte>.Unsafe(name, 0, name.Length) }; }
            /// <summary>
            /// 隐式转换
            /// </summary>
            /// <param name="name">HTTP头名称</param>
            /// <returns>HTTP头名称唯一哈希</returns>
            public static implicit operator headerName(subArray<byte> name) { return new headerName { Name = name }; }
            /// <summary>
            /// 获取哈希值
            /// </summary>
            /// <returns>哈希值</returns>
            public unsafe override int GetHashCode()
            {
                return Name.Count < 10 ? 0 : Name.UnsafeArray[Name.StartIndex + Name.Count - 10] & 7;
            }
            /// <summary>
            /// 判断是否相等
            /// </summary>
            /// <param name="other">待匹配数据</param>
            /// <returns>是否相等</returns>
            public unsafe bool Equals(headerName other)
            {
                if (Name.Count == other.Name.Count)
                {
                    fixed (byte* nameFixed = Name.UnsafeArray, otherNameFixed = other.Name.UnsafeArray)
                    {
                        return fastCSharp.unsafer.memory.EqualCase(nameFixed + Name.StartIndex, otherNameFixed + other.Name.StartIndex, Name.Count);
                    }
                }
                return false;
            }
            /// <summary>
            /// 判断是否相等
            /// </summary>
            /// <param name="obj">待匹配数据</param>
            /// <returns>是否相等</returns>
            public override bool Equals(object obj)
            {
                return Equals((headerName)obj);
            }
        }
        /// <summary>
        /// HTTP头名称解析委托
        /// </summary>
        private static readonly uniqueDictionary<headerName, Action<htmlTitleClient, bufferIndex>> parses;
        static htmlTitleClient()
        {
            list<keyValue<headerName, Action<htmlTitleClient, bufferIndex>>> parseList = new list<keyValue<headerName, Action<htmlTitleClient, bufferIndex>>>();
            parseList.Add(new keyValue<headerName, Action<htmlTitleClient, bufferIndex>>(header.ContentLengthBytes, parseContentLength));
            parseList.Add(new keyValue<headerName, Action<htmlTitleClient, bufferIndex>>(header.ContentTypeBytes, parseContentType));
            parseList.Add(new keyValue<headerName, Action<htmlTitleClient, bufferIndex>>(header.ContentEncodingBytes, parseContentEncoding));
            parseList.Add(new keyValue<headerName, Action<htmlTitleClient, bufferIndex>>(header.TransferEncodingBytes, parseTransferEncoding));
            parseList.Add(new keyValue<headerName, Action<htmlTitleClient, bufferIndex>>(header.ConnectionBytes, parseConnection));
            parses = new uniqueDictionary<headerName, Action<htmlTitleClient, bufferIndex>>(parseList, 8);
        }
    }
}
