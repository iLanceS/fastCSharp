using System;
using fastCSharp.code.cSharp;
using fastCSharp.net;

namespace fastCSharp.demo.loadBalancingTcpCommandWeb
{
    /// <summary>
    /// TCP调用负载均衡服务测试调用
    /// </summary>
    internal static class loadBalancing
    {
        /// <summary>
        /// TCP调用负载均衡服务
        /// </summary>
        private static loadBalancingTcpCommand.server.tcpLoadBalancing server;
        /// <summary>
        /// TCP调用负载均衡服务是否启动成功
        /// </summary>
        public static bool IsServer
        {
            get { return server != null; }
        }
        /// <summary>
        /// 关闭TCP调用负载均衡服务
        /// </summary>
        public static void Close()
        {
            pub.Dispose(ref server);
        }

        /// <summary>
        /// 同步测试
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static returnValue<int> add(int left, int right)
        {
            returnValue<int> value = server.add(left, right);
            if (value.Type == returnValue.type.Success)
            {
                if (left + right != value.Value) Console.WriteLine("add 调用错误 : " + left.toString() + " + " + right.toString() + " = " + (left + right).toString() + " <> " + value.Value.toString());
            }
            else Console.WriteLine("add 调用失败"); 
            return value;
        }
        /// <summary>
        /// 异步测试
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="onReturn"></param>
        public static void xor(int left, int right, Action<returnValue<int>> onReturn)
        {
            server.xor(left, right, value =>
            {
                if (value.Type == returnValue.type.Success)
                {
                    if ((left ^ right) != value.Value) Console.WriteLine("xor 调用错误 : " + left.toString() + " ^ " + right.toString() + " = " + (left ^ right).toString() + " <> " + value.Value.toString());
                }
                else Console.WriteLine("xor 调用失败");
                onReturn(value);
            });
        }

        static loadBalancing()
        {
            server = new loadBalancingTcpCommand.server.tcpLoadBalancing();
            if (server.StartLoadBalancingServer()) domainUnload.Add(Close);
            else pub.Dispose(ref server);
        }
    }
}
