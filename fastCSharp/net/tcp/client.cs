using System;
using System.Net;
using System.Net.Sockets;
using fastCSharp.net;

namespace fastCSharp.net.tcp
{
    /// <summary>
    /// TCP客户端
    /// </summary>
    public class client : IDisposable
    {
        /// <summary>
        /// 最后一次连接是否没有指定IP地址
        /// </summary>
        private static bool isAnyIpAddress;
        /// <summary>
        /// 配置信息
        /// </summary>
        protected fastCSharp.code.cSharp.tcpServer attribute;
        /// <summary>
        /// TCP客户端
        /// </summary>
        protected Socket tcpClient;
        /// <summary>
        /// 套接字
        /// </summary>
        private net.socket netSocket;
        /// <summary>
        /// 套接字
        /// </summary>
        public net.socket NetSocket
        {
            get
            {
                if (tcpClient == null) netSocket = new net.socket(tcpClient, true);
                return netSocket;
            }
        }
        /// <summary>
        /// 是否正在释放资源
        /// </summary>
        protected bool isDispose;
        /// <summary>
        /// 是否启动TCP客户端
        /// </summary>
        public bool IsStart
        {
            get
            {
                return tcpClient != null;
            }
        }
        /// <summary>
        /// TCP调用服务端
        /// </summary>
        /// <param name="attribute">配置信息</param>
        /// <param name="isStart">是否启动客户端</param>
        public client(fastCSharp.code.cSharp.tcpServer attribute, bool isStart)
        {
            this.attribute = attribute;
            if (isStart) start();
        }
        /// <summary>
        /// 启动客户端链接
        /// </summary>
        protected virtual void start()
        {
            tcpClient = Create(attribute);
        }
        /// <summary>
        /// 停止客户端链接
        /// </summary>
        public virtual void Dispose()
        {
            if (!isDispose)
            {
                isDispose = true;
                dispose();
            }
        }
        /// <summary>
        /// 停止客户端链接
        /// </summary>
        protected void dispose()
        {
            //if (tcpClient != null) tcpClient.Close();
            tcpClient.shutdown();
            tcpClient = null;
            netSocket = null;
        }
        /// <summary>
        /// 创建TCP客户端
        /// </summary>
        /// <param name="attribute">配置信息</param>
        /// <returns>TCP客户端,失败返回null</returns>
        internal static Socket Create(fastCSharp.code.cSharp.tcpServer attribute)
        {
            Socket socket = null;
            try
            {
                if (attribute.IpAddress == IPAddress.Any)
                {
                    if (!isAnyIpAddress) log.Error.Add("客户端TCP连接失败(" + attribute.ServiceName + "[" + attribute.TcpRegisterName + "] " + attribute.Host + ":" + attribute.Port.toString() + ")", null, false);
                    isAnyIpAddress = true;
                    return null;
                }
                isAnyIpAddress = false;
                socket = new Socket(attribute.IpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(attribute.IpAddress, attribute.Port);
                return socket;
            }
            catch (Exception error)
            {
                log.Error.Add(error, "客户端TCP连接失败(" + attribute.ServiceName + "[" + attribute.TcpRegisterName + "] " + attribute.IpAddress.ToString() + ":" + attribute.Port.toString() + ")", false);
                if (socket != null) fastCSharp.threading.disposeTimer.Default.addSocketClose(socket);
            }
            return null;
        }
        /// <summary>
        /// 创建TCP客户端
        /// </summary>
        /// <param name="socket">TCP套接字</param>
        /// <param name="attribute">配置信息</param>
        /// <param name="connectAsync"></param>
        /// <returns></returns>
        internal static bool Create(ref Socket socket, fastCSharp.code.cSharp.tcpServer attribute, SocketAsyncEventArgs connectAsync)
        {
            try
            {
                if (attribute.IpAddress == IPAddress.Any)
                {
                    if (!isAnyIpAddress) log.Error.Add("客户端TCP连接失败(" + attribute.ServiceName + "[" + attribute.TcpRegisterName + "] " + attribute.Host + ":" + attribute.Port.toString() + ")", null, false);
                    isAnyIpAddress = true;
                }
                else
                {
                    isAnyIpAddress = false;
                    connectAsync.RemoteEndPoint = new IPEndPoint(attribute.IpAddress, attribute.Port);
                    connectAsync.SocketError = SocketError.Success;
                    socket = new Socket(attribute.IpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    if (socket.ConnectAsync(connectAsync)) return true;
                }
            }
            catch (Exception error)
            {
                log.Error.Add(error, "客户端TCP连接失败(" + attribute.ServiceName + "[" + attribute.TcpRegisterName + "] " + attribute.IpAddress.ToString() + ":" + attribute.Port.toString() + ")", false);
                if (socket != null) fastCSharp.threading.disposeTimer.Default.addSocketClose(socket);
            }
            return false;
        }
    }
}
