using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;

namespace fastCSharp.demo.udpPortServer
{
    /// <summary>
    /// UPD服务端
    /// </summary>
    public sealed class udpServer : IDisposable
    {
        /// <summary>
        /// 最大数据包
        /// </summary>
        public const int MaxPacketSize = 256;
        /// <summary>
        /// 超时秒数
        /// </summary>
        public const int TimeoutSeconds = 30;
        /// <summary>
        /// 任意IP端口
        /// </summary>
        private static readonly IPEndPoint anyIp = new IPEndPoint(IPAddress.Any, 0);
        /// <summary>
        /// 套接字
        /// </summary>
        private Socket socket;
        /// <summary>
        /// IP端口信息集合
        /// </summary>
        private timeoutDictionary<ipPort, hashBytes> ports = new timeoutDictionary<ipPort, hashBytes>(TimeoutSeconds);
        /// <summary>
        /// IP端口名称集合
        /// </summary>
        private Dictionary<hashBytes, ipPort> names = dictionary.CreateHashBytes<ipPort>();
        /// <summary>
        /// UPD服务端
        /// </summary>
        /// <param name="tcpServer">TCP服务调用配置</param>
        public udpServer(fastCSharp.code.cSharp.tcpServer tcpServer)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Parse(tcpServer.Host), tcpServer.Port));
            ports.OnRemovedLock += remove;
            fastCSharp.threading.threadPool.TinyPool.Start(receive);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            fastCSharp.threading.disposeTimer.Default.AddSocketClose(ref socket);
            pub.Dispose(ref ports);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipPort"></param>
        /// <param name="name"></param>
        private void remove(ipPort ipPort, hashBytes name)
        {
            names.Remove(name);
        }
        /// <summary>
        /// 接受数据
        /// </summary>
        private unsafe void receive()
        {
            EndPoint endPoint = null;
            byte[] data = new byte[MaxPacketSize];
            fixed (byte* dataFixed = data)
            {
                while (socket != null)
                {
                    try
                    {
                        endPoint = anyIp;
                        int count = socket.ReceiveFrom(data, 0, MaxPacketSize, SocketFlags.None, ref endPoint);
                        if (count > 0)
                        {
                            ipPort oldPort, ipPort = (IPEndPoint)endPoint;
                            byte[] name = new byte[count];
                            Buffer.BlockCopy(data, 0, name, 0, count);
                            hashBytes nameKey = name;
                            if (names.TryGetValue(nameKey, out oldPort))
                            {
                                if (ipPort.Equals(oldPort)) ports.RefreshTimeout(oldPort);
                                else
                                {
                                    ports.Remove(oldPort);
                                    ports[ipPort] = nameKey;
                                    names[nameKey] = ipPort;
                                }
                            }
                            else
                            {
                                ports[ipPort] = nameKey;
                                names[nameKey] = ipPort;
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        Console.WriteLine(error.ToString());
                        Thread.Sleep(1);
                    }
                }
            }
        }
        /// <summary>
        /// 根据名称获取IP端口信息
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>IP端口信息</returns>
        internal ipPort Get(subArray<byte> name)
        {
            ipPort value;
            return names.TryGetValue(name, out value) ? value : default(ipPort);
        }
    }
}
