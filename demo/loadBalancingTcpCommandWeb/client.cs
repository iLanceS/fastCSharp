using System;
using System.Threading;
using fastCSharp.net;
using System.Text;
using System.Net;
using System.Net.Sockets;
using fastCSharp.threading;

namespace fastCSharp.demo.loadBalancingTcpCommandWeb
{
    /// <summary>
    /// 客户端
    /// </summary>
    internal sealed class client : IDisposable
    {
        /// <summary>
        /// 客户端任务池
        /// </summary>
        public sealed class task : IDisposable
        {
            /// <summary>
            /// 客户端集合
            /// </summary>
            private client[] clients;
            /// <summary>
            /// 空闲事件
            /// </summary>
            private EventWaitHandle freeWait;
            /// <summary>
            /// 客户端集合访问锁
            /// </summary>
            private readonly object clientLock = new object();
            /// <summary>
            /// 空闲客户端数量
            /// </summary>
            private int clientIndex;
            /// <summary>
            /// 当前实例数量
            /// </summary>
            private int clientCount;
            /// <summary>
            /// 请求任务数量
            /// </summary>
            private int taskCount;
            ///// <summary>
            ///// 任务完成输出数量
            ///// </summary>
            //private int outputCount;
            /// <summary>
            /// 错误数量
            /// </summary>
            internal int ErrorCount;
            /// <summary>
            /// 保持连接最大次数
            /// </summary>
            internal int KeepAliveCount { get; private set; }
            /// <summary>
            /// 客户端任务池
            /// </summary>
            /// <param name="maxClientCount">最大实例数量</param>
            /// <param name="isKeepAlive">保持连接最大次数</param>
            public task(int maxClientCount, int keepAliveCount)
            {
                clients = new client[maxClientCount];
                //outputCount = client.outputCount;
                KeepAliveCount = keepAliveCount;
                freeWait = new EventWaitHandle(true, EventResetMode.ManualReset);
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                Monitor.Enter(clientLock);
                try
                {
                    if (clientIndex != 0)
                    {
                        foreach (client client in clients)
                        {
                            pub.Dispose(client);
                            if (--clientIndex == 0) break;
                        }
                    }
                }
                finally { Monitor.Exit(clientLock); }
            }
            /// <summary>
            /// 添加请求数量
            /// </summary>
            /// <param name="count">请求数量</param>
            public void Add(int count)
            {
                freeWait.Reset();
                Monitor.Enter(clientLock);
                taskCount += count;
                Monitor.Exit(clientLock);
                while (count-- != 0) add();
            }
            /// <summary>
            /// 添加任务
            /// </summary>
            private void add()
            {
                client client = null;
                bool isNewClient = false;
                Monitor.Enter(clientLock);
                if (clientIndex == 0)
                {
                    if (clientCount != clients.Length)
                    {
                        isNewClient = true;
                        ++clientCount;
                        --taskCount;
                    }
                }
                else
                {
                    --taskCount;
                    client = clients[--clientIndex];
                }
                Monitor.Exit(clientLock);
                if (client != null) client.request();
                else if (isNewClient)
                {
                    try
                    {
                        (client = new client(this)).request();
                        return;
                    }
                    catch { }
                    Monitor.Enter(clientLock);
                    --clientCount;
                    Monitor.Exit(clientLock);
                }
            }
            /// <summary>
            /// 下一个任务
            /// </summary>
            /// <param name="client">客户端</param>
            public void Next(client client)
            {
                int clientCount = int.MinValue;
                Monitor.Enter(clientLock);
                if (this.taskCount == 0)
                {
                    clients[clientIndex++] = client;
                    clientCount = this.clientCount - clientIndex;
                }
                else --this.taskCount;
                Monitor.Exit(clientLock);
                if (clientCount == int.MinValue) client.request();
                //if (Interlocked.Decrement(ref outputCount) == 0)
                //{
                //    Interlocked.Add(ref outputCount, client.outputCount);
                //    Console.Write('.');
                //}
                if (clientCount == 0) freeWait.Set();
            }
            /// <summary>
            /// 等待空闲
            /// </summary>
            public void Wait()
            {
                freeWait.WaitOne();
            }
            /// <summary>
            /// 关闭客户端
            /// </summary>
            public void CloseClient()
            {
                Monitor.Enter(clientLock);
                for (int index = 0; index != clientIndex; ++index) clients[index].CloseSocket();
                Monitor.Exit(clientLock);
            }
        }
        ///// <summary>
        ///// 任务完成输出数量
        ///// </summary>
        //private const int outputCount = 1 << 10;
        /// <summary>
        /// 收发数据缓冲区字节数
        /// </summary>
        private const int bufferSize = 512;
        /// <summary>
        /// fastCSharp爬虫标识
        /// </summary>
        private static readonly byte[] fastCSharpSpiderUserAgent = fastCSharp.net.tcp.http.requestHeader.fastCSharpSpiderUserAgent.getBytes();
        /// <summary>
        /// 服务器IP地址与客户端号
        /// </summary>
        private static readonly IPEndPoint serverEndPoint;
        /// <summary>
        /// 客户端标识名称
        /// </summary>
        private static readonly byte[] userAgentName = (fastCSharp.web.header.UserAgent + ": ").getBytes();
        /// <summary>
        /// 关闭套接字0超时设置
        /// </summary>
        private static readonly LingerOption lingerOption = new LingerOption(true, 0);
        /// <summary>
        /// 收发数据缓冲区
        /// </summary>
        private byte[] buffer;
        /// <summary>
        /// 异步连接操作
        /// </summary>
        private SocketAsyncEventArgs connectAsync;
        /// <summary>
        /// 异步发送操作
        /// </summary>
        private SocketAsyncEventArgs sendAsync;
        /// <summary>
        /// 异步读取操作
        /// </summary>
        private SocketAsyncEventArgs receiveAsync;
        /// <summary>
        /// 套接字
        /// </summary>
        private Socket socket;
        /// <summary>
        /// 客户端任务池
        /// </summary>
        private task clientTask;
        /// <summary>
        /// 请求类型
        /// </summary>
        private int requestType;
        /// <summary>
        /// 测试左值
        /// </summary>
        private int left;
        /// <summary>
        /// 测试右值
        /// </summary>
        private int right;
        /// <summary>
        /// 客户端标识
        /// </summary>
        private byte[] userAgent;
#if HELLO
        /// <summary>
        /// 请求URL
        /// </summary>
        private static readonly byte[] keepUrl = (@"GET /hello.html HTTP/1.1
Host: " + webConfig.config.Default.Domain + @"
Connection: Keep-Alive

").getBytes();
        /// <summary>
        /// 请求URL
        /// </summary>
        private static readonly byte[] url = (@"GET /hello.html HTTP/1.1
Host: " + webConfig.config.Default.Domain + @"

").getBytes();
#else
        /// <summary>
        /// HTTP版本
        /// </summary>
        private static readonly byte[] httpVersion = (@" HTTP/1.1
Host: " + webConfig.config.Default.Domain + @"
").getBytes();
        /// <summary>
        /// HTTP版本
        /// </summary>
        private static readonly byte[] httpVersionKeepAlive = (@" HTTP/1.1
Host: " + webConfig.config.Default.Domain + @"
Connection: Keep-Alive
").getBytes();
        /// <summary>
        /// 请求URL
        /// </summary>
        private string url;
#endif
        /// <summary>
        /// 连接拒绝次数
        /// </summary>
        private int connectionRefusedCount;
        /// <summary>
        /// 保持连接最大次数
        /// </summary>
        private int keepAliveCount;
        /// <summary>
        /// 客户端
        /// </summary>
        /// <param name="task">客户端任务池</param>
        private client(task task)
        {
            clientTask = task;
            buffer = new byte[bufferSize];

            connectAsync = new SocketAsyncEventArgs();
            connectAsync.RemoteEndPoint = serverEndPoint;
            connectAsync.UserToken = this;
            connectAsync.Completed += onConnect;

            sendAsync = new SocketAsyncEventArgs();
            sendAsync.UserToken = this;
#if HELLO
#else
            sendAsync.SetBuffer(buffer, 0, buffer.Length);
#endif
            sendAsync.Completed += onSend;

            receiveAsync = new SocketAsyncEventArgs();
            receiveAsync.UserToken = this;
            receiveAsync.SetBuffer(buffer, 0, buffer.Length);
            receiveAsync.Completed += onReceive;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            pub.Dispose(ref connectAsync);
            pub.Dispose(ref sendAsync);
            pub.Dispose(ref receiveAsync);
        }
        /// <summary>
        /// 客户端请求
        /// </summary>
        private void request()
        {
            try
            {
                connectionRefusedCount = 0;
#if HELLO
#else
                int random = Math.Abs(fastCSharp.random.Default.Next());
                left = random >> 8;
                right = random & 0xffffff;
                switch (requestType = (random & 7))
                {
                    case 0:
                        userAgent = null;
                        url = "/webCall/Add?left=" + left.toString() + "&right=" + right.toString();
                        break;
                    case 1:
                        userAgent = null;
                        url = "/webCall/Xor?left=" + left.toString() + "&right=" + right.toString();
                        break;
                    case 2:
                        userAgent = fastCSharpSpiderUserAgent;
                        url = "/webView.html?left=" + left.toString() + "&right=" + right.toString();
                        break;
                    case 3:
                        userAgent = fastCSharpSpiderUserAgent;
                        url = "/webView.html?isAsynchronous=1&left=" + left.toString() + "&right=" + right.toString();
                        break;
                    case 4:
                        userAgent = null;
                        url = "/ajax?n=/webView.html&left=" + left.toString() + "&right=" + right.toString();
                        break;
                    case 5:
                        userAgent = null;
                        url = "/ajax?n=/webView.html&isAsynchronous=1&left=" + left.toString() + "&right=" + right.toString();
                        break;
                    case 6:
                        userAgent = null;
                        url = "/ajax?n=loadBalancing.Add&left=" + left.toString() + "&right=" + right.toString();
                        break;
                    case 7:
                        userAgent = null;
                        url = "/ajax?n=loadBalancing.Xor&left=" + left.toString() + "&right=" + right.toString();
                        break;
                }
#endif
                if (keepAliveCount == 0)
                {
                    socket = new Socket(serverEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    socket.SendBufferSize = socket.ReceiveBufferSize = bufferSize;
                    socket.LingerState = lingerOption;
                    keepAliveCount = clientTask.KeepAliveCount;
                    if (socket.ConnectAsync(connectAsync)) return;
                }
                else
                {
                    connectAsync.SocketError = SocketError.Success;
                    onConnect(null, connectAsync);
                    return;
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error.ToString());
            }
            onCrawl(-1);
        }
        /// <summary>
        /// 异步连接操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="async"></param>
        private unsafe void onConnect(object sender, SocketAsyncEventArgs async)
        {
            if (async.SocketError == SocketError.Success)
            {
                try
                {
#if HELLO
                    if (--keepAliveCount == 0) sendAsync.SetBuffer(url, 0, url.Length);
                    else sendAsync.SetBuffer(keepUrl, 0, keepUrl.Length);
                    if (socket.SendAsync(sendAsync)) return;
#else
                    fixed (byte* bufferFixed = buffer)
                    fixed (char* urlFixed = url)
                    {
                        *(int*)bufferFixed = 'G' + ('E' << 8) + ('T' << 16) + (' ' << 24);
                        fastCSharp.unsafer.String.WriteBytes(urlFixed, url.Length, bufferFixed + sizeof(int));
                        int index = url.Length + sizeof(int);
                        if (--keepAliveCount == 0)
                        {
                            Buffer.BlockCopy(httpVersion, 0, buffer, index, httpVersion.Length);
                            index += httpVersion.Length;
                        }
                        else
                        {
                            Buffer.BlockCopy(httpVersionKeepAlive, 0, buffer, index, httpVersionKeepAlive.Length);
                            index += httpVersionKeepAlive.Length;
                        }
                        if (userAgent != null)
                        {
                            Buffer.BlockCopy(userAgentName, 0, buffer, index, userAgentName.Length);
                            Buffer.BlockCopy(userAgent, 0, buffer, index += userAgentName.Length, userAgent.Length);
                            *(short*)(bufferFixed + (index += userAgent.Length)) = 0x0a0d;
                            index += sizeof(short);
                        }
                        *(short*)(bufferFixed + index) = 0x0a0d;
                        sendAsync.SetBuffer(0, index + sizeof(short));
                        if (socket.SendAsync(sendAsync)) return;
                    }
#endif
                }
                catch (Exception error)
                {
                    Console.WriteLine(error.ToString());
                }
            }
            else if (async.SocketError == SocketError.ConnectionRefused)
            {
                if (++connectionRefusedCount != 3)
                {
                    try
                    {
                        socket = new Socket(serverEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        socket.SendBufferSize = socket.ReceiveBufferSize = bufferSize;
                        socket.LingerState = lingerOption;
                        if (socket.ConnectAsync(connectAsync)) return;
                    }
                    catch (Exception error)
                    {
                        Console.WriteLine(error.ToString());
                    }
                    onCrawl(-1);
                    return;
                }
                Console.WriteLine(SocketError.ConnectionRefused.ToString());
            }
            onCrawl(-2);
        }
        /// <summary>
        /// 异步发送操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="async"></param>
        private void onSend(object sender, SocketAsyncEventArgs async)
        {
            if (async.SocketError == SocketError.Success)
            {
                try
                {
                    receiveAsync.SetBuffer(0, buffer.Length);
                    if (socket.ReceiveAsync(receiveAsync)) return;
                }
                catch (Exception error)
                {
                    Console.WriteLine(error.ToString());
                }
            }
            onCrawl(-3);
        }
        /// <summary>
        /// 异步接收操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="async"></param>
        private void onReceive(object sender, SocketAsyncEventArgs async)
        {
            if (async.SocketError == SocketError.Success) onCrawl(async.BytesTransferred);
            else
            {
                onCrawl(0);
                Console.WriteLine("Error " + async.SocketError.ToString());
            }
        }
        /// <summary>
        /// 抓取完成回调
        /// </summary>
        /// <param name="data">抓取数据</param>
        private unsafe void onCrawl(int count)
        {
#if HELLO
            if (count <= 10)
            {
                keepAliveCount = 0;
                Interlocked.Increment(ref clientTask.ErrorCount);
                Console.WriteLine("Error[" + count.toString() + "]");
            }
            else if (buffer[9] != '2')
            {
                keepAliveCount = 0;
                Interlocked.Increment(ref clientTask.ErrorCount);
                Console.WriteLine("Error " + System.Text.Encoding.ASCII.GetString(buffer, 0, 12));
            }
            if (keepAliveCount == 0)
            {
                Socket socket = this.socket;
                clientTask.Next(this);
                socket.Close();
            }
            else clientTask.Next(this);
#else
            try
            {
                int bodyCount = -1;
                if (count > 0)
                {
                    fixed (byte* bufferFixed = buffer)
                    {
                        byte* end = bufferFixed + count, start = fastCSharp.unsafer.memory.FindLast(bufferFixed, end, 10);
                        if (start != null && (int)(++start - bufferFixed) >= (int)(end - start))
                        {
                            char* write = (char*)bufferFixed;
                            while (start != end) *write++ = (char)*start++;
                            bodyCount = (int)(write - (char*)bufferFixed);
                            int value = (requestType & 1) == 0 ? left + right : (left ^ right), returnValue;
                            switch (requestType)
                            {
                                case 0:
                                case 1:
                                    returnValue = value + 1;
                                    fastCSharp.emit.jsonParser.UnsafeParse<int>((char*)bufferFixed, bodyCount, ref returnValue);
                                    break;
                                case 2:
                                case 3:
                                case 4:
                                case 5:
                                case 6:
                                case 7:
                                    fastCSharp.code.cSharp.tcpBase.parameterJsonToSerialize<int> return2 = default(fastCSharp.code.cSharp.tcpBase.parameterJsonToSerialize<int>);
                                    if (fastCSharp.emit.jsonParser.UnsafeParse<fastCSharp.code.cSharp.tcpBase.parameterJsonToSerialize<int>>((char*)bufferFixed, bodyCount, ref return2)) returnValue = return2.Return;
                                    else returnValue = value + 1;
                                    break;
                                default:
                                    returnValue = value + 1;
                                    break;
                            }
                            if (returnValue == value)
                            {
                                //Console.WriteLine("OK : " + left.toString() + "," + right.toString() + "[" + value.toString() + "]");
                                return;
                            }
                            Interlocked.Increment(ref clientTask.ErrorCount);
                            Console.WriteLine("Error : " + url + " " + left.toString() + "," + right.toString() + "[" + value.toString() + "] <> " + new string((char*)bufferFixed, 0, bodyCount));
                            return;
                        }
                    }
                }
                keepAliveCount = 0;
                Interlocked.Increment(ref clientTask.ErrorCount);
                Console.WriteLine("Error[" + count.toString() + "," + bodyCount.toString() + "] : " + url);
            }
            finally
            {
                if (keepAliveCount == 0)
                {
                    Socket socket = this.socket;
                    clientTask.Next(this);
                    socket.Close();
                }
                else clientTask.Next(this);
            }
#endif
        }
        /// <summary>
        /// 关闭客户端
        /// </summary>
        public void CloseSocket()
        {
            keepAliveCount = 0;
            socket.Close();
        }

        ///// <summary>
        ///// JS类型
        ///// </summary>
        //private static readonly string jsonContentType = "application/json; charset=" + Encoding.ASCII.WebName;
        /// <summary>
        /// 启动负载均衡服务
        /// </summary>
        /// <returns>是否成功</returns>
        public static bool LoadBalancing()
        {
            using (webClient webClient = new webClient())
            {
                webClient.KeepAlive = false;
                return fastCSharp.emit.jsonParser.Parse<fastCSharp.code.cSharp.tcpBase.parameterJsonToSerialize<int>>(webClient.CrawlHtml("http://" + webConfig.config.Default.Domain + "/ajax?n=loadBalancing.Start", Encoding.ASCII)).Return != 0;
            }
        }
        static client()
        {
            fastCSharp.net.tcp.host host = fastCSharp.net.tcp.host.FromDomain(webConfig.config.Default.Domain);
            serverEndPoint = new IPEndPoint(IPAddress.Parse(host.Host), host.Port);
        }
    }
}
