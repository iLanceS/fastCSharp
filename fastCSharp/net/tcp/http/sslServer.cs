using System;
using System.IO;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace fastCSharp.net.tcp.http
{
    /// <summary>
    /// 基于安全连接的HTTP服务
    /// </summary>
    internal sealed class sslServer : server
    {
        /// <summary>
        /// HTTP服务
        /// </summary>
        /// <param name="servers">HTTP服务器</param>
        /// <param name="host">TCP服务端口信息</param>
        public sslServer(servers servers, ref host host)
            : base(servers, ref host)
        {
            certificate = fastCSharp.config.http.Default.GetCertificate(ref host, ref protocol);
            if (certificate == null) fastCSharp.log.Error.Add("安全证书获取失败 " + host.Host + ":" + host.Port.toString(), null, false);
            else if (protocol == SslProtocols.None) protocol = SslProtocols.Tls;
        }
    }
}
