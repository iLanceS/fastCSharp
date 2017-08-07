using System;
using System.IO;
using System.Collections.Generic;

namespace fastCSharp.testCase
{
    /// <summary>
    /// TCP服务客户端识别测试
    /// </summary>
    [fastCSharp.code.cSharp.tcpServer(IsIdentityCommand = true, Host = "127.0.0.1", Port = 12345, IsRememberIdentityCommand = false)]
    internal partial class tcpSession
    {
        /// <summary>
        /// 服务器端写客户端标识测试+服务器端验证函数测试
        /// </summary>
        /// <param name="client">客户端标识[必须定义为第一个参数]</param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [fastCSharp.code.cSharp.tcpMethod(IsVerifyMethod = true, InputParameterMaxLength = 1 << 10)]
        private unsafe bool login(fastCSharp.net.tcp.commandServer.socket client, string user, string password)
        {
            switch (user)
            {
                case "userA":
                    if (password == "A")
                    {
                        client.ClientUserInfo = user;
                        return true;
                    }
                    break;
                case "userB":
                    if (password == "B")
                    {
                        client.ClientUserInfo = user;
                        return true;
                    }
                    break;
            }
            return false;
        }
        /// <summary>
        /// 服务器端读客户端标识测试
        /// </summary>
        /// <param name="client">客户端标识[必须定义为第一个参数]</param>
        /// <returns></returns>
        [fastCSharp.code.cSharp.tcpMethod]
        private unsafe string myName(fastCSharp.net.tcp.commandServer.socket client)
        {
            return (string)client.ClientUserInfo;
        }
#if NotFastCSharpCode
#else
        /// <summary>
        /// 客户端验证函数接口类型定义
        /// </summary>
#if NOJIT
        private sealed class veify : fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject
#else
        private sealed class veify : fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<tcpSession.tcpClient>
#endif
        {
            /// <summary>
            /// 
            /// </summary>
            private string user;
            /// <summary>
            /// 
            /// </summary>
            private string password;
            /// <summary>
            /// 客户端验证函数接口类型定义
            /// </summary>
            /// <param name="user"></param>
            /// <param name="password"></param>
            public veify(string user, string password)
            {
                this.user = user;
                this.password = password;
            }
            /// <summary>
            /// 客户端验证函数调用[在客户端连接掉服务端后触发调用]
            /// </summary>
            /// <param name="client">TCP调用客户端</param>
            /// <returns></returns>
#if NOJIT
            public bool Verify(object client)
            {
                return ((tcpSession.tcpClient)client).login(user, password).Value;
            }
#else
            public bool Verify(tcpSession.tcpClient client)
            {
                return client.login(user, password).Value;
            }
#endif
        }
        /// <summary>
        /// TCP服务客户端识别测试
        /// </summary>
        /// <returns></returns>
        [fastCSharp.code.testCase]
        internal static bool TestCase()
        {
            using (tcpSession.tcpServer server = new tcpSession.tcpServer()) return server.Start() && testCase();
        }
        /// <summary>
        /// TCP服务客户端识别测试
        /// </summary>
        /// <returns></returns>
        private static bool testCase()
        {
            using (tcpSession.tcpClient clientA = new tcpSession.tcpClient(null, new veify("userA", "A")))
            using (tcpSession.tcpClient clientB = new tcpSession.tcpClient(null, new veify("userB", "B")))
            {
                if (clientA.myName().Value != "userA") return false;
                if (clientB.myName().Value != "userB") return false;
            }
            return true;
        }
#endif
    }
}
