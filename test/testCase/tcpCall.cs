using System;

namespace fastCSharp.testCase
{
    /// <summary>
    /// 跨类型静态调用测试[IsServer = true]表示主配置
    /// </summary>
    [fastCSharp.code.cSharp.tcpCall(Service = TcpCall.ServiceName, IsIdentityCommand = true, Host = "127.0.0.1", Port = 12345, IsRememberIdentityCommand = false, IsServer = true)]
    internal static partial class addTcpCall
    {
        [fastCSharp.code.cSharp.tcpMethod]
        private static int add(int a, int b)
        {
            return a + b;
        }
    }
    /// <summary>
    /// 跨类型静态调用测试，绑定配置[TcpCallService]
    /// </summary>
    [fastCSharp.code.cSharp.tcpCall(Service = TcpCall.ServiceName)]
    internal static partial class subTcpCall
    {
        [fastCSharp.code.cSharp.tcpMethod]
        private static int sub(int a, int b)
        {
            return a - b;
        }
    }
    /// <summary>
    /// 跨类型静态调用测试，绑定配置[TcpCallService]
    /// </summary>
    [fastCSharp.code.cSharp.tcpCall(Service = TcpCall.ServiceName)]
    internal static partial class xorTcpCall
    {
        [fastCSharp.code.cSharp.tcpMethod]
        private static int xor(int a, int b)
        {
            return a ^ b;
        }
    }
    /// <summary>
    /// 跨类型静态调用测试
    /// </summary>
    internal static partial class TcpCall
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string ServiceName = "TcpCallService";
#if NotFastCSharpCode
#else
        /// <summary>
        /// 跨类型静态调用测试
        /// </summary>
        /// <returns></returns>
        [fastCSharp.code.testCase]
        internal static bool TestCase()
        {
            using (tcpServer.TcpCallService server = new tcpServer.TcpCallService()) return server.Start() && testCase();
        }
        /// <summary>
        /// 跨类型静态调用测试
        /// </summary>
        /// <returns></returns>
        private static bool testCase()
        {
            if (tcpCall.addTcpCall.add(5, 3) != (5 + 3)) return false;
            if (tcpCall.subTcpCall.sub(5, 3) != (5 - 3)) return false;
            if (tcpCall.xorTcpCall.xor(5, 3) != (5 ^ 3)) return false;
            return true;
        }
#endif
    }
}
