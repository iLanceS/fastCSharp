using System;

namespace fastCSharp.demo.httpSessionServer
{
    /// <summary>
    /// Session服务
    /// </summary>
    [fastCSharp.code.cSharp.tcpServer(Filter = code.memberFilters.Instance, Host = "127.0.0.1", Port = 12300, IsIdentityCommand = true, ClientInterfaceType = typeof(fastCSharp.net.tcp.http.ISessionClient<string>), VerifyMethodType = typeof(sessionServer.verifyMethod), IsRememberIdentityCommeand = false)]
    public partial class sessionServer : fastCSharp.net.tcp.http.session<string>
    {
        /// <summary>
        /// 默认TCP调用验证函数
        /// </summary>
        public sealed class verifyMethod : fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<tcpClient>
        {
            /// <summary>
            /// HTTP服务客户端验证
            /// </summary>
            /// <param name="client">TCP调用客户端</param>
            /// <returns>是否通过验证</returns>
            public bool Verify(tcpClient client)
            {
                return client.verify(fastCSharp.config.http.Default.SessionVerify).Value;
            }
        }
    }
}
