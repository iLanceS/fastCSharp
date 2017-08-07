using System;
using System.Threading;
using System.Net.Sockets;
using fastCSharp.threading;
using System.Reflection;
using fastCSharp.io;
using System.Net;
using System.Runtime.CompilerServices;

namespace fastCSharp.net.tcp
{
    /// <summary>
    /// TCP调用服务端基类
    /// </summary>
    public abstract class server : IDisposable
    {
        ///// <summary>
        ///// 客户端请求队列失败次数
        ///// </summary>
        //private static int acceptQueueLessCount;
        ///// <summary>
        ///// 客户端请求队列失败次数
        ///// </summary>
        //public static int AcceptQueueLessCount
        //{
        //    get { return acceptQueueLessCount; }
        //}
        /// <summary>
        /// 配置信息
        /// </summary>
        protected internal fastCSharp.code.cSharp.tcpServer attribute;
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName
        {
            get { return attribute.ServiceName; }
        }
        /// <summary>
        /// TCP注册服务 客户端
        /// </summary>
        private fastCSharp.net.tcp.tcpRegister.client tcpRegisterClient;
        /// <summary>
        /// TCP监听服务器端套接字
        /// </summary>
        private Socket socket;
        /// <summary>
        /// 最大客户端数量
        /// </summary>
        private int maxClientCount;
        /// <summary>
        /// 当前客户端数量
        /// </summary>
        protected int currentClientCount;
        /// <summary>
        /// 当前释放客户端数量
        /// </summary>
        protected int freeClientCount;
        /// <summary>
        /// 当前客户端数量
        /// </summary>
        public int CurrentClientCount
        {
            get { return currentClientCount - freeClientCount; }
        }
        /// <summary>
        /// 是否已启动服务
        /// </summary>
        protected int isStart;
        /// <summary>
        /// 是否已启动服务
        /// </summary>
        public bool IsStart
        {
            get { return isStart != 0; }
        }
        /// <summary>
        /// TCP调用服务端
        /// </summary>
        /// <param name="attribute">配置信息</param>
        protected server(fastCSharp.code.cSharp.tcpServer attribute)
        {
            if (attribute == null) log.Error.Throw(log.exceptionType.Null);
            if (attribute.TcpRegisterName != null)
            {
                tcpRegisterClient = fastCSharp.net.tcp.tcpRegister.client.Get(attribute.TcpRegisterName);
                if (tcpRegisterClient == null) log.Error.Throw("TCP注册服务 " + attribute.TcpRegisterName + " 链接失败", null, false);
                if (attribute.RegisterHost == null) attribute.RegisterHost = attribute.Host;
                if (attribute.RegisterPort == 0) attribute.RegisterPort = attribute.Port;
                fastCSharp.net.tcp.tcpRegister.registerState state = tcpRegisterClient.Register(attribute);
                if (state != fastCSharp.net.tcp.tcpRegister.registerState.Success) log.Error.Throw("TCP服务注册 " + attribute.ServiceName + " 失败 " + state.ToString(), null, false);
                if (attribute.Port == 0) attribute.Port = attribute.RegisterPort;
                log.Default.Add(attribute.ServiceName + " 注册 " + attribute.Host + ":" + attribute.Port.toString() + " => " + attribute.RegisterHost + ":" + attribute.RegisterPort.toString(), new System.Diagnostics.StackFrame(), false);
            }
            if (!attribute.IsServer) log.Default.Add("配置未指明的TCP服务端 " + attribute.ServiceName, null, false);
            this.attribute = attribute;
            if ((maxClientCount = attribute.MaxClientCount) <= 0) maxClientCount = int.MaxValue;
#if MONO
            //if (attribute.AcceptQueueSize > 30) attribute.AcceptQueueSize = 30;
            //else if (attribute.AcceptQueueSize < 4) attribute.AcceptQueueSize = 4;
#endif
        }
        /// <summary>
        /// 停止服务事件
        /// </summary>
        public event Action OnDisposed;
        /// <summary>
        /// 停止服务监听
        /// </summary>
        public void StopListen()
        {
            pub.Dispose(ref this.socket);
            fastCSharp.net.tcp.tcpRegister.client tcpRegisterClient = Interlocked.Exchange(ref this.tcpRegisterClient, null);
            if (tcpRegisterClient != null) tcpRegisterClient.RemoveRegister(attribute);
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        public virtual void Dispose()
        {
            if (Interlocked.CompareExchange(ref isStart, 0, 1) == 1)
            {
                log.Default.Add("停止服务 " + attribute.ServiceName + "[" + attribute.Host + ":" + attribute.Port.toString() + "]", null, false);
                fastCSharp.domainUnload.Remove(this, domainUnload.unloadType.TcpServerDispose, false);
                StopListen();
                if (OnDisposed != null) OnDisposed();
            }
        }
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <returns>是否成功</returns>
        protected bool start()
        {
            if (Interlocked.CompareExchange(ref isStart, 1, 0) == 0)
            {
                try
                {
                    socket = new Socket(attribute.IpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    socket.Bind(new IPEndPoint(attribute.IpAddress, attribute.Port));
                    socket.Listen(int.MaxValue);
                    if (attribute.Port == 0) log.Default.Add(GetType().FullName + "服务器端口为 0", null, false);
                }
                catch (Exception error)
                {
                    Dispose();
                    log.Error.ThrowReal(error, GetType().FullName + "服务器端口 " + attribute.Host + ":" + attribute.Port.toString() + " TCP连接失败)", false);
                }
                return isStart != 0;
            }
            return false;
        }
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <returns>是否成功</returns>
        public virtual bool Start()
        {
            if (start())
            {
                getSocket();
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取客户端请求
        /// </summary>
        protected void getSocket()
        {
            int threadCount = attribute.AcceptThreadCount;
            if (threadCount < 2) threadPool.TinyPool.FastStart(this, thread.callType.TcpServerGetSocket);
            else
            {
                do
                {
                    threadPool.TinyPool.FastStart(this, thread.callType.TcpServerGetSocketMany);
                }
                while (--threadCount != 0);
            }
            Thread.Sleep(0);
            fastCSharp.domainUnload.Add(this, domainUnload.unloadType.TcpServerDispose);
        }
        /// <summary>
        /// 获取客户端请求
        /// </summary>
        internal void GetSocket()
        {
            fastCSharp.threading.disposeTimer disposeTimer = fastCSharp.threading.disposeTimer.Default;
            while (isStart != 0)
            {
                Socket listenSocket = this.socket, socket = null;
                try
                {
                    if (maxClientCount == int.MaxValue)
                    {
                        do
                        {
                            newSocket(socket = listenSocket.Accept());
                            ++currentClientCount;
                            socket = null;
                        }
                        while (true);
                    }
                    else
                    {
                        do
                        {
                            socket = listenSocket.Accept();
                            if (currentClientCount < maxClientCount || (currentClientCount -= Interlocked.Exchange(ref freeClientCount, 0)) < maxClientCount)
                            {
                                newSocket(socket);
                                ++currentClientCount;
                                socket = null;
                            }
                            else
                            {
                                disposeTimer.addSocketClose(ref socket);
                                log.Default.Add(attribute.ServiceName + " 超出最大连接数量 " + maxClientCount.toString(), new System.Diagnostics.StackFrame(), false);
                                Thread.Sleep(1);
                            }
                        }
                        while (true);
                    }
                }
                catch (Exception error)
                {
                    if (this.socket == null) return;
                    disposeTimer.addSocketClose(socket);
                    if (isStart != 0)
                    {
                        log.Error.Add(error, null, false);
                        Thread.Sleep(1);
                    }
                }
            }
        }
        /// <summary>
        /// 获取客户端请求
        /// </summary>
        internal void GetSocketMany()
        {
            fastCSharp.threading.disposeTimer disposeTimer = fastCSharp.threading.disposeTimer.Default;
            while (isStart != 0)
            {
                Socket listenSocket = this.socket, socket = null;
                try
                {
                    if (maxClientCount == int.MaxValue)
                    {
                        do
                        {
                            newSocket(socket = listenSocket.Accept());
                            Interlocked.Increment(ref currentClientCount);
                            socket = null;
                        }
                        while (true);
                    }
                    else
                    {
                        do
                        {
                            socket = listenSocket.Accept();
                            if (currentClientCount < maxClientCount || Interlocked.Add(ref currentClientCount, -Interlocked.Exchange(ref freeClientCount, 0)) < maxClientCount)
                            {
                                newSocket(socket);
                                Interlocked.Increment(ref currentClientCount);
                                socket = null;
                            }
                            else
                            {
                                disposeTimer.addSocketClose(ref socket);
                                log.Default.Add(attribute.ServiceName + " 超出最大连接数量 " + maxClientCount.toString(), new System.Diagnostics.StackFrame(), false);
                                Thread.Sleep(1);
                            }
                        }
                        while (true);
                    }
                }
                catch (Exception error)
                {
                    if (this.socket == null) return;
                    disposeTimer.addSocketClose(socket);
                    if (isStart != 0)
                    {
                        log.Error.Add(error, null, false);
                        Thread.Sleep(1);
                    }
                }
            }
        }
        ///// <summary>
        ///// 客户端队列(无锁队列处理)
        ///// </summary>
        //private sealed class newClientQueue : IDisposable
        //{
        //    /// <summary>
        //    /// 默认等待计数
        //    /// </summary>
        //    private static int defaultWaitCount = fastCSharp.pub.CpuCount << 3;
        //    /// <summary>
        //    /// TCP服务
        //    /// </summary>
        //    private server server;
        //    /// <summary>
        //    /// 套接字集合
        //    /// </summary>
        //    private array.value<Socket>[] sockets;
        //    /// <summary>
        //    /// 队列等待事件
        //    /// </summary>
        //    private EventWaitHandle queueWait;
        //    /// <summary>
        //    /// 写入位置
        //    /// </summary>
        //    private int writeIndex;
        //    /// <summary>
        //    /// 读取位置
        //    /// </summary>
        //    private int readIndex;
        //    /// <summary>
        //    /// 位置值复位
        //    /// </summary>
        //    private int andIndex;
        //    /// <summary>
        //    /// 当前等待计数
        //    /// </summary>
        //    private int waitCount;
        //    /// <summary>
        //    /// 队列等待访问锁
        //    /// </summary>
        //    private int waitLock;
        //    /// <summary>
        //    /// 是否已经释放资源
        //    /// </summary>
        //    private bool isDisposed;
        //    /// <summary>
        //    /// 客户端队列
        //    /// </summary>
        //    /// <param name="server">TCP服务</param>
        //    /// <param name="bits">缓冲区容器大小(2^n)</param>
        //    public newClientQueue(server server, int bits)
        //    {
        //        andIndex = 1 << bits;
        //        sockets = new array.value<Socket>[andIndex];
        //        this.server = server;
        //        readIndex = --andIndex;
        //        waitCount = defaultWaitCount;
        //        queueWait = new EventWaitHandle(false, EventResetMode.ManualReset);
        //        threadPool.TinyPool.FastStart(waitThread, null, null);
        //    }
        //    /// <summary>
        //    /// 客户端队列
        //    /// </summary>
        //    /// <param name="queue">客户端队列</param>
        //    /// <param name="socket">客户端队列</param>
        //    private newClientQueue(newClientQueue queue, Socket socket)
        //    {
        //        andIndex = (queue.andIndex << 1) + 1;
        //        server = queue.server;
        //        if (andIndex == int.MaxValue) log.Error.ThrowReal(log.exceptionType.IndexOutOfRange);
        //        readIndex = andIndex;
        //        sockets = new array.value<Socket>[andIndex + 1];
        //        writeIndex = 1;
        //        sockets[0].Value = socket;
        //        waitCount = defaultWaitCount;
        //        queueWait = new EventWaitHandle(true, EventResetMode.ManualReset);
        //        threadPool.TinyPool.FastStart(callThread, null, null);
        //    }
        //    /// <summary>
        //    /// 释放资源
        //    /// </summary>
        //    public void Dispose()
        //    {
        //        if (!isDisposed)
        //        {
        //            interlocked.CompareSetYieldSleep0(ref waitLock);
        //            isDisposed = true;
        //            queueWait.Set();
        //            waitLock = 0;
        //        }
        //    }
        //    /// <summary>
        //    /// 添加队列
        //    /// </summary>
        //    /// <param name="socket">套接字</param>
        //    /// <returns></returns>
        //    public newClientQueue Push(Socket socket)
        //    {
        //        if (readIndex == this.writeIndex)
        //        {
        //            try
        //            {
        //                newClientQueue queue = new newClientQueue(this, socket);
        //                Dispose();
        //                return queue;
        //            }
        //            catch (Exception error)
        //            {
        //                log.Error.Add(error, null, true);
        //            }
        //            return this;
        //        }
        //        sockets[this.writeIndex].Value = socket;
        //        int writeIndex = (this.writeIndex + 1) & andIndex;
        //        interlocked.CompareSetYieldSleep0(ref waitLock);
        //        this.writeIndex = writeIndex;
        //        if (--waitCount == 0) queueWait.Set();
        //        waitLock = 0;
        //        return this;
        //    }
        //    /// <summary>
        //    /// 队列处理线程
        //    /// </summary>
        //    private void waitThread()
        //    {
        //        ThreadPriority oldPriority = Thread.CurrentThread.Priority;
        //        Thread.CurrentThread.Priority = ThreadPriority.Highest;
        //        queueWait.WaitOne();
        //        queueThread();
        //        Thread.CurrentThread.Priority = oldPriority;
        //    }
        //    /// <summary>
        //    /// 队列处理线程
        //    /// </summary>
        //    private void callThread()
        //    {
        //        ThreadPriority oldPriority = Thread.CurrentThread.Priority;
        //        Thread.CurrentThread.Priority = ThreadPriority.Highest;
        //        queueThread();
        //        Thread.CurrentThread.Priority = oldPriority;
        //    }
        //    /// <summary>
        //    /// 队列处理线程
        //    /// </summary>
        //    private void queueThread()
        //    {
        //        do
        //        {
        //            int index = (readIndex + 1) & andIndex;
        //            interlocked.CompareSetYieldSleep0(ref waitLock);
        //            if (index == writeIndex)
        //            {
        //                if (isDisposed)
        //                {
        //                    waitLock = 0;
        //                    return;
        //                }
        //                waitCount = defaultWaitCount;
        //                queueWait.Reset();
        //                waitLock = 0;
        //            WAIT:
        //                if (queueWait.WaitOne(1))
        //                {
        //                    if (index == writeIndex) return;
        //                }
        //                else if (index == writeIndex)
        //                {
        //                    if (isDisposed) return;
        //                    goto WAIT;
        //                }
        //            }
        //            else waitLock = 0;
        //            Socket socket = sockets[index].Free();
        //            readIndex = index;
        //            if (server.isStart == 0) socket.shutdown();
        //            else server.newSocket(socket);
        //        }
        //        while (true);
        //    }
        //}
        ///// <summary>
        ///// 获取客户端请求(无锁队列处理)
        ///// </summary>
        //protected void acceptSocket()
        //{
        //    ThreadPriority oldPriority = Thread.CurrentThread.Priority;
        //    Thread.CurrentThread.Priority = ThreadPriority.Highest;
        //    newClientQueue queue = new newClientQueue(this, attribute.AcceptQueueSize);
        //    while (isStart != 0)
        //    {
        //        Socket listenSocket = this.socket;
        //        try
        //        {
        //            if (maxClientCount == int.MaxValue)
        //            {
        //                do
        //                {
        //                    queue = queue.Push(listenSocket.Accept());
        //                    Interlocked.Increment(ref currentClientCount);
        //                }
        //                while (true);
        //            }
        //            else
        //            {
        //                do
        //                {
        //                    Socket socket = listenSocket.Accept();
        //                    if (currentClientCount < maxClientCount)
        //                    {
        //                        queue = queue.Push(socket);
        //                        Interlocked.Increment(ref currentClientCount);
        //                    }
        //                    else
        //                    {
        //                        task.Tiny.Add(socket.Dispose);
        //                        log.Default.Add(attribute.ServiceName + " 超出最大连接数量 " + maxClientCount.toString(), new System.Diagnostics.StackFrame(), false);
        //                        Thread.Sleep(1);
        //                    }
        //                }
        //                while (true);
        //            }
        //        }
        //        catch (Exception error)
        //        {
        //            if (this.socket == null)
        //            {
        //                Thread.CurrentThread.Priority = oldPriority;
        //                queue.Dispose();
        //                //onAcceptEnd();
        //                return;
        //            }
        //            if (isStart != 0)
        //            {
        //                log.Error.Add(error, null, false);
        //                Thread.Sleep(1);
        //            }
        //        }
        //    }
        //    Thread.CurrentThread.Priority = oldPriority;
        //    queue.Dispose();
        //    //onAcceptEnd();
        //}
        ///// <summary>
        ///// 获取客户端请求结束处理
        ///// </summary>
        //protected virtual void onAcceptEnd() { }
        /// <summary>
        /// 客户端请求处理
        /// </summary>
        /// <param name="socket">客户端套接字</param>
        protected virtual void newSocket(Socket socket)
        {
            newSocketMany(socket);
        }
        /// <summary>
        /// 客户端请求处理
        /// </summary>
        /// <param name="socket">客户端套接字</param>
        protected abstract void newSocketMany(Socket socket);
        /// <summary>
        /// 套接字处理完毕
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void SocketEnd()
        {
            Interlocked.Increment(ref freeClientCount);
        }
        /// <summary>
        /// 获取队列索引
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected static int getQueueIndex(uint ip)
        {
            ip ^= ip >> 16;
            return (int)((ip ^ (ip >> 8)) & 0xff);
        }
    }
}
