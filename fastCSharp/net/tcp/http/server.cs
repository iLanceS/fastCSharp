using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using fastCSharp.threading;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Runtime.InteropServices;

namespace fastCSharp.net.tcp.http
{
    /// <summary>
    /// HTTP服务
    /// </summary>
    internal class server : tcp.server
    {
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
            private server server;
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
            public void Set(server server, int maxActiveSocketCount, int maxSocketCount)
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
                        if (server.Certificate == null) ++socketBase.queueCount;
                        else ++socketBase.sslQueueCount;
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
                        if (server.Certificate == null) ++socketBase.queueCount;
                        else ++socketBase.sslQueueCount;
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
        /// HTTP服务器
        /// </summary>
        internal servers Servers;
        /// <summary>
        /// 套接字队列
        /// </summary>
        private socketQueue[] socketQueues;
        /// <summary>
        /// 已绑定域名数量
        /// </summary>
        internal int DomainCount;
        /// <summary>
        /// SSL证书
        /// </summary>
        protected X509Certificate certificate;
        /// <summary>
        /// SSL证书
        /// </summary>
        internal X509Certificate Certificate { get { return certificate; } }
        /// <summary>
        /// SSL协议
        /// </summary>
        protected SslProtocols protocol;
        /// <summary>
        /// SSL协议
        /// </summary>
        internal SslProtocols Protocol
        {
            get { return protocol; }
        }
        /// <summary>
        /// HTTP服务
        /// </summary>
        /// <param name="servers">HTTP服务器</param>
        /// <param name="host">TCP服务端口信息</param>
        public server(servers servers, ref host host)
            : base(new code.cSharp.tcpServer { Host = host.Host, Port = host.Port, AcceptThreadCount = fastCSharp.config.http.Default.AcceptThreadCount, IsServer = true })
        {
            this.Servers = servers;
            DomainCount = 1;

            int maxActiveSocketCount = servers.Attribute.MaxActiveClientCountIpAddress;
            if (maxActiveSocketCount > 0)
            {
                int maxSocketCount = Math.Max(servers.Attribute.MaxClientCountPerIpAddress, maxActiveSocketCount);
                socketQueues = new socketQueue[256];
                for (int index = 256; index != 0; socketQueues[--index].Set(this, maxActiveSocketCount, maxSocketCount)) ;
            }
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            if (socketQueues != null)
            {
                for (int index = socketQueues.Length; index != 0; socketQueues[--index].Close()) ;
            }
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
                if (ipv6.Ip != null && socketQueues[getQueueIndex((uint)ipv6.HashCode)].NewSocket(socket, ref ipv6)) return 1;
            }
            else
            {
#pragma warning disable 618
                int ipv4 = (int)(uint)(ulong)ipEndPoint.Address.Address;
#pragma warning restore 618
                if (socketQueues[getQueueIndex((uint)ipv4)].NewSocket(socket, ipv4)) return 1;

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
            if (socketQueues == null)
            {
                if (Certificate == null) (typePool<socket>.Pop() ?? http.socket.NewSocket()).Start(this, socket);
                else (typePool<sslStream>.Pop() ?? sslStream.NewStream()).Start(this, socket);
            }
            else if (newSocketQueue(socket) == 0) --currentClientCount;
        }
        /// <summary>
        /// 客户端请求处理
        /// </summary>
        /// <param name="socket">客户端套接字</param>
        protected override void newSocketMany(Socket socket)
        {
            if (socketQueues == null)
            {
                if (Certificate == null) (typePool<socket>.Pop() ?? http.socket.NewSocket()).Start(this, socket);
                else (typePool<sslStream>.Pop() ?? sslStream.NewStream()).Start(this, socket);
            }
            else if (newSocketQueue(socket) == 0) Interlocked.Decrement(ref currentClientCount);
        }
        /// <summary>
        /// 客户端请求处理
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="ipv6"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void newSocket(Socket socket, ref ipv6Hash ipv6)
        {
            if (Certificate == null) (typePool<socket>.Pop() ?? http.socket.NewSocket()).Start(this, socket, ref ipv6);
            else (typePool<sslStream>.Pop() ?? sslStream.NewStream()).Start(this, socket, ref ipv6);
        }
        /// <summary>
        /// 客户端请求处理
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="ipv4"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void newSocket(Socket socket, int ipv4)
        {
            if (Certificate == null) (typePool<socket>.Pop() ?? http.socket.NewSocket()).Start(this, socket, ipv4);
            else (typePool<sslStream>.Pop() ?? sslStream.NewStream()).Start(this, socket, ipv4);
        }
        /// <summary>
        /// 请求处理结束
        /// </summary>
        /// <param name="ipv4"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal Socket SocketEnd(int ipv4)
        {
            Interlocked.Increment(ref freeClientCount);
            return socketQueues == null ? null : socketQueues[getQueueIndex((uint)ipv4)].Next(ipv4);
        }
        /// <summary>
        /// 请求处理结束
        /// </summary>
        /// <param name="ipv6"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal Socket SocketEnd(ref ipv6Hash ipv6)
        {
            Interlocked.Increment(ref freeClientCount);
            return socketQueues == null ? null : socketQueues[getQueueIndex((uint)ipv6.HashCode)].Next(ref ipv6);
        }
        static server()
        {
            if (fastCSharp.config.appSetting.IsCheckMemory) checkMemory.Add(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }
    }
}
