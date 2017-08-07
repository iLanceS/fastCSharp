using System;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using System.Threading;
using fastCSharp.threading;
using fastCSharp;

namespace fastCSharp.demo.udpPortServer
{
    /// <summary>
    /// UDP穿透端口服务客户端
    /// </summary>
    public sealed class portClient : IDisposable
    {
        /// <summary>
        /// UDP套接字
        /// </summary>
        public sealed class udpSocket : IDisposable
        {
            /// <summary>
            /// UDP穿透端口服务客户端
            /// </summary>
            private portClient client;
            /// <summary>
            /// IP端口信息
            /// </summary>
            internal ipPort IpPort;
            /// <summary>
            /// 套接字
            /// </summary>
            public Socket Socket { get; private set; }
            /// <summary>
            /// 套接字注册名称
            /// </summary>
            private byte[] name;
            /// <summary>
            /// 远程套接字注册名称
            /// </summary>
            private fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer remoteName;
            /// <summary>
            /// 获取套接字名称
            /// </summary>
            private byte[] recieveName;
            /// <summary>
            /// 开始穿透
            /// </summary>
            private Action start;
            /// <summary>
            /// 发送握手验证数据
            /// </summary>
            private Action sendName;
            /// <summary>
            /// 超时时间
            /// </summary>
            private DateTime timeout;
            /// <summary>
            /// 远程客户端IP端口信息
            /// </summary>
            public IPEndPoint RemoteIp { get; private set; }
            /// <summary>
            /// 是否接收到数据
            /// </summary>
            private int isReceive;
            /// <summary>
            /// 是否接收到数据
            /// </summary>
            public bool IsReceive
            {
                get { return isReceive == 2; }
            }
            /// <summary>
            /// 是否正在发送数据
            /// </summary>
            private int isSend;
            /// <summary>
            /// UDP穿透端口服务客户端
            /// </summary>
            /// <param name="client">UDP穿透端口服务客户端</param>
            /// <param name="name">套接字注册名称</param>
            /// <param name="ip">套接字绑定端口信息</param>
            /// <param name="remoteName">远程套接字注册名称</param>
            internal udpSocket(portClient client, byte[] name, ipPort ipPort, byte[] remoteName)
            {
                IpPort = ipPort;
                Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                Socket.Bind(new IPEndPoint(new IPAddress(ipPort.Ip), ipPort.Port));
                this.client = client;
                this.name = name;
                this.remoteName = new fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer { Buffer = subArray<byte>.Unsafe(remoteName, 0, remoteName.Length) };
                timeout = date.NowSecond.AddTicks(client.timeoutTicks);
            }
            /// <summary>
            /// 开始穿透
            /// </summary>
            internal void Start()
            {
                if (Socket != null)
                {
                    if (register())
                    {
                        get();
                        return;
                    }
                    if (date.NowSecond < timeout)
                    {
                        if (start == null) start = Start;
                        timerTask.Default.Add(start, date.NowSecond.AddSeconds(1), null);
                        return;
                    }
                }
                client.onSocket(this);
            }
            /// <summary>
            /// 向服务器发送注册信息
            /// </summary>
            /// <returns>是否成功</returns>
            private bool register()
            {
                try
                {
                    if (Socket.SendTo(name, 0, name.Length, SocketFlags.None, client.udpServerIp) == name.Length) return true;
                }
                catch (Exception error)
                {
                    Console.WriteLine(error.ToString());
                }
                return false;
            }
            /// <summary>
            /// 获取客户端IP端口信息
            /// </summary>
            private unsafe void get()
            {
                try
                {
                    fastCSharp.net.returnValue<ipPort> ipPort = client.client.get(remoteName);
                    if (ipPort.Type == fastCSharp.net.returnValue.type.Success && ipPort.Value.Ip != 0 && ipPort.Value.Port != 0)
                    {
                        RemoteIp = new IPEndPoint(new IPAddress(ipPort.Value.Ip), ipPort.Value.Port);
                        isReceive = 0;
                        if (sendName == null) sendName = send;
                        isSend = 1;
                        fastCSharp.threading.threadPool.TinyPool.Start(sendName, null, null);
                        if (recieveName == null) recieveName = new byte[Math.Max(Math.Max(remoteName.Buffer.Count, name.Length), sizeof(int))];
                        do
                        {
                            Console.WriteLine("Receive 1");
                            EndPoint endPoint = RemoteIp;
                            if (Socket.ReceiveFrom(recieveName, 0, recieveName.Length, SocketFlags.None, ref endPoint) == remoteName.Buffer.Count && remoteName.Buffer.UnsafeArray.equal(recieveName, remoteName.Buffer.Count))
                            {
                                isReceive = 1;
                                break;
                            }
                        }
                        while (DateTime.Now < timeout);
                        if (isReceive != 0)
                        {
                            do
                            {
                                Console.WriteLine("Receive 2");
                                EndPoint endPoint = RemoteIp;
                                int receiveLength = Socket.ReceiveFrom(recieveName, 0, recieveName.Length, SocketFlags.None, ref endPoint);
                                if (receiveLength == name.Length && name.equal(recieveName, name.Length))
                                {
                                    while (isSend != 0) Thread.Sleep(1);
                                    isReceive = 2;
                                    client.onSocket(this);
                                    return;
                                }
                                else if (receiveLength == remoteName.Buffer.Count && remoteName.Buffer.UnsafeArray.equal(recieveName, remoteName.Buffer.Count)) continue;
                                else
                                {
                                    Console.WriteLine("Receive Error " + receiveLength.toString());
                                }
                            }
                            while (DateTime.Now < timeout);
                        }
                    }
                }
                catch (Exception error)
                {
                    Console.WriteLine(error.ToString());
                }
                while (isSend != 0) Thread.Sleep(1);
                if (date.NowSecond < timeout) fastCSharp.threading.timerTask.Default.Add(Start, date.NowSecond.AddSeconds(1));
                else client.onSocket(this);
            }
            /// <summary>
            /// 发送握手验证数据
            /// </summary>
            private void send()
            {
                try
                {
                    do
                    {
                        try
                        {
                            Console.WriteLine("Send 1");
                            Socket.SendTo(name, 0, name.Length, SocketFlags.None, RemoteIp);
                            if (isReceive != 0) break;
                        }
                        catch (Exception error)
                        {
                            Console.WriteLine(error.ToString());
                        }
                        if (DateTime.Now >= timeout) break;
                        Thread.Sleep(1000);
                    }
                    while (isReceive == 0 && Socket != null);
                    Console.WriteLine("Send End " + isReceive.toString());
                    if (isReceive == 1 && Socket != null)
                    {
                        try
                        {
                            Socket.SendTo(remoteName.Buffer.UnsafeArray, 0, remoteName.Buffer.Count, SocketFlags.None, RemoteIp);
                        }
                        catch (Exception error)
                        {
                            Console.WriteLine(error.ToString());
                        }
                    }
                }
                finally { isSend = 0; }
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                if (Socket != null)
                {
                    Socket.Close();
                    Socket = null;
                }
                isReceive = 0;
            }
        }
        /// <summary>
        ///  UDP穿透端口服务客户端
        /// </summary>
        private portServer.tcpClient client;
        /// <summary>
        /// UPD服务端IP地址
        /// </summary>
        private IPEndPoint udpServerIp;
        /// <summary>
        /// UDP套接字集合
        /// </summary>
        private Dictionary<ipPort, udpSocket> sockets =  dictionary.Create<ipPort, udpSocket>();
        /// <summary>
        /// UDP套接字连接完成事件
        /// </summary>
        public event Action<udpSocket> OnUdpSocket;
        /// <summary>
        /// UDP套接字访问锁
        /// </summary>
        private readonly object socketLock = new object();
        /// <summary>
        /// 超时时钟周期
        /// </summary>
        private long timeoutTicks;
        /// <summary>
        /// UDP穿透端口服务客户端
        /// </summary>
        /// <param name="tcpServer">TCP服务调用配置</param>
        /// <param name="timeoutSeconds">超时秒数</param>
        public portClient(fastCSharp.code.cSharp.tcpServer tcpServer, int timeoutSeconds)
        {
            udpServerIp = new IPEndPoint(IPAddress.Parse(tcpServer.Host), tcpServer.Port);
            client = new portServer.tcpClient(tcpServer, null);
            if (timeoutSeconds <= 0) timeoutSeconds = udpServer.TimeoutSeconds;
            timeoutTicks = new TimeSpan(0, 0, timeoutSeconds).Ticks;
        }
        /// <summary>
        /// 添加UDP套接字
        /// </summary>
        /// <param name="name">套接字注册名称</param>
        /// <param name="ipPort">IP端口信息</param>
        /// <param name="remoteName">远程套接字注册名称</param>
        public void Add(byte[] name, ipPort ipPort, byte[] remoteName)
        {
            if (name == null || name.Length > udpServer.MaxPacketSize || remoteName == null || remoteName.Length > udpServer.MaxPacketSize)
            {
                log.Error.Throw(log.exceptionType.IndexOutOfRange);
            }
            udpSocket socket = null, oldSocket;
            try
            {
                socket = new udpSocket(this, name, ipPort, remoteName);
                Monitor.Enter(socketLock);
                try
                {
                    if (sockets.TryGetValue(ipPort, out oldSocket))
                    {
                        pub.Dispose(ref oldSocket);
                        sockets[ipPort] = socket;
                    }
                    else sockets.Add(ipPort, socket);
                }
                finally { Monitor.Exit(socketLock); }
                socket.Start();
                socket = null;
            }
            finally { pub.Dispose(socket); }
        }
        /// <summary>
        /// UDP套接字连接完成事件
        /// </summary>
        /// <param name="socket">UDP套接字</param>
        private void onSocket(udpSocket socket)
        {
            udpSocket oldSocket;
            Monitor.Enter(socketLock);
            try
            {
                if (sockets.TryGetValue(socket.IpPort, out oldSocket) && socket == oldSocket) sockets.Remove(socket.IpPort);
            }
            finally { Monitor.Exit(socketLock); }
            if (!socket.IsReceive) socket.Dispose();
            if (OnUdpSocket != null) OnUdpSocket(socket);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            pub.Dispose(ref client);
            Monitor.Enter(socketLock);
            try
            {
                foreach (udpSocket socket in sockets.Values) socket.Dispose();
                sockets.Clear();
            }
            finally { Monitor.Exit(socketLock); }
        }
    }
}
