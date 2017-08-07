using System;
using System.IO;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace fastCSharp.config
{
    /// <summary>
    /// HTTP服务相关参数
    /// </summary>
    public sealed class http
    {
        /// <summary>
        /// HTTP服务名称
        /// </summary>
        private string serviceName = "fastCSharp.httpServer";
        /// <summary>
        /// HTTP服务名称
        /// </summary>
        public string ServiceName
        {
            get { return string.IsNullOrEmpty(serviceName) ? "fastCSharp.httpServer" : serviceName; }
        }
        /// <summary>
        /// HTTP服务用户名
        /// </summary>
        public string ServiceUsername;
        /// <summary>
        /// HTTP服务密码
        /// </summary>
        public string ServicePassword;
        /// <summary>
        /// HTTP头部缓存数据大小
        /// </summary>
        private int headerBufferLength = 1 << 11;
        /// <summary>
        /// HTTP头部缓存数据大小
        /// </summary>
        public int HeaderBufferLength
        {
            get
            {
                return headerBufferLength >= (1 << 10) && headerBufferLength < (1 << 15) ? headerBufferLength : (1 << 11);
            }
        }
        /// <summary>
        /// HTTP头部最大未定义项数
        /// </summary>
        private int maxHeaderCount = 16;
        /// <summary>
        /// HTTP头部最大未定义项数
        /// </summary>
        public int MaxHeaderCount
        {
            get { return Math.Max(maxHeaderCount, 0); }
        }
        /// <summary>
        /// URI最大查询参数项数
        /// </summary>
        private int maxQueryCount = 32;
        /// <summary>
        /// URI最大查询参数项数
        /// </summary>
        public int MaxQueryCount
        {
            get { return Math.Max(maxQueryCount, 0); }
        }
        /// <summary>
        /// HTTP每秒最小表单数据接收字节数(单位KB)
        /// </summary>
        private int minReceiveSizePerSecond = 8;
        /// <summary>
        /// HTTP每秒最小表单数据接收字节数
        /// </summary>
        public int MinReceiveSizePerSecond
        {
            get { return minReceiveSizePerSecond > 0 ? minReceiveSizePerSecond << 10 : 0; }
        }
        /// <summary>
        /// 套接字接收超时
        /// </summary>
        private int receiveSeconds = 15;
        /// <summary>
        /// 套接字接收超时
        /// </summary>
        public int ReceiveSeconds
        {
            get { return receiveSeconds; }
        }
        /// <summary>
        /// 套接字二次接收超时
        /// </summary>
        private int keepAliveReceiveSeconds = 60 + 15;
        /// <summary>
        /// 套接字二次接收超时
        /// </summary>
        public int KeepAliveReceiveSeconds
        {
            get { return keepAliveReceiveSeconds; }
        }
        /// <summary>
        /// WebSocket超时
        /// </summary>
        private int webSocketReceiveSeconds = 60 + 15;
        /// <summary>
        /// WebSocket超时
        /// </summary>
        public int WebSocketReceiveSeconds
        {
            get { return webSocketReceiveSeconds; }
        }
        /// <summary>
        /// HTTP最大接收数据字节数(单位:MB)
        /// </summary>
        private int maxPostDataSize = 4;
        /// <summary>
        /// HTTP最大接收数据字节数(单位:MB)
        /// </summary>
        public int MaxPostDataSize
        {
            get { return maxPostDataSize > 0 ? maxPostDataSize : 4; }
        }
        /// <summary>
        /// HTTP内存流最大字节数(单位:KB)
        /// </summary>
        private int maxMemoryStreamSize = 64;
        /// <summary>
        /// HTTP内存流最大字节数(单位:KB)
        /// </summary>
        public int MaxMemoryStreamSize
        {
            get { return maxMemoryStreamSize >= 0 ? maxMemoryStreamSize : 64; }
        }
        /// <summary>
        /// 大数据缓存字节数(单位:KB)
        /// </summary>
        private int bigBufferSize = 64;
        /// <summary>
        /// 大数据缓存字节数
        /// </summary>
        public int BigBufferSize
        {
            get { return Math.Max(Math.Max(bigBufferSize << 10, headerBufferLength << 1), fastCSharp.config.appSetting.StreamBufferSize); }
        }
        /// <summary>
        /// HTTP服务启动后启动的进程
        /// </summary>
        public string[] OnStartProcesses;
        /// <summary>
        /// Session名称
        /// </summary>
        private string sessionName = "fastCSharpSession";
        /// <summary>
        /// Session名称
        /// </summary>
        public string SessionName
        {
            get { return sessionName ?? "fastCSharpSession"; }
        }
        /// <summary>
        /// Session服务名称
        /// </summary>
        private string sessionServiceName = "fastCSharp.httpSessionServer";
        /// <summary>
        /// Session服务名称
        /// </summary>
        public string SessionServiceName
        {
            get { return string.IsNullOrEmpty(sessionServiceName) ? "fastCSharp.httpSessionServer" : sessionServiceName; }
        }
        /// <summary>
        /// Session服务用户名
        /// </summary>
        public string SessionServiceUsername;
        /// <summary>
        /// Session服务密码
        /// </summary>
        public string SessionServicePassword;
        /// <summary>
        /// Session超时分钟数
        /// </summary>
        private int sessionMinutes = 60;
        /// <summary>
        /// Session超时分钟数
        /// </summary>
        public int SessionMinutes { get { return sessionMinutes; } }
        /// <summary>
        /// Session超时刷新分钟数
        /// </summary>
        private int sessionRefreshMinutes = 10;
        /// <summary>
        /// Session超时刷新分钟数
        /// </summary>
        public int SessionRefreshMinutes
        {
            get { return sessionRefreshMinutes > sessionMinutes ? sessionMinutes : sessionRefreshMinutes; }
        }
        /// <summary>
        /// 客户端队列缓存数量
        /// </summary>
        private int clientSessionQueueCount = 1 << 10;
        /// <summary>
        /// 客户端队列缓存数量
        /// </summary>
        public int ClientSessionQueueCount { get { return clientSessionQueueCount; } }
        /// <summary>
        /// 域名转IP地址缓存时间
        /// </summary>
        private int ipAddressTimeoutMinutes = 60;
        /// <summary>
        /// 域名转IP地址缓存时间
        /// </summary>
        public int IpAddressTimeoutMinutes
        {
            get { return Math.Max(ipAddressTimeoutMinutes, 1); }
        }
        /// <summary>
        /// 域名转IP地址缓存数量(小于等于0表示不限)
        /// </summary>
        private int ipAddressCacheCount = 1 << 10;
        /// <summary>
        /// 域名转IP地址缓存时间
        /// </summary>
        public int IpAddressCacheCount
        {
            get { return ipAddressCacheCount; }
        }
        /// <summary>
        /// 统计数量秒数
        /// </summary>
        private int countSeconds = 0;
        /// <summary>
        /// 统计数量秒数
        /// </summary>
        public int CountSeconds { get { return Math.Max(countSeconds, 0); } }
        /// <summary>
        /// 接收请求线程数量(一般根据CPU核心数量调整)
        /// </summary>
        public int AcceptThreadCount;
        /// <summary>
        /// TCP服务端口证书集合
        /// </summary>
        private fastCSharp.net.tcp.http.sslStream.certificate[] certificates = null;
        /// <summary>
        /// 获取安全证书
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        internal X509Certificate GetCertificate(ref fastCSharp.net.tcp.host host)
        {
            SslProtocols protocol = SslProtocols.None;
            return GetCertificate(ref host, ref protocol);
        }
        /// <summary>
        /// 获取安全证书
        /// </summary>
        /// <param name="host"></param>
        /// <param name="protocol"></param>
        /// <returns></returns>
        internal X509Certificate GetCertificate(ref fastCSharp.net.tcp.host host, ref SslProtocols protocol)
        {
            if (certificates != null)
            {
                int index = 0;
                foreach (fastCSharp.net.tcp.http.sslStream.certificate certificate in certificates)
                {
                    if (certificate.Host.Equals(host)) return certificates[index].Get(out protocol);
                    ++index;
                }
            }
            return null;
        }
        /// <summary>
        /// HTTP服务相关参数
        /// </summary>
        private http()
        {
            pub.LoadConfig(this);
        }
        /// <summary>
        /// 默认HTTP服务相关参数
        /// </summary>
        public static readonly http Default = new http();
    }
}
