using System;
using System.Threading;
using fastCSharp.code.cSharp;
using fastCSharp.net;

namespace fastCSharp.demo.loadBalancingTcpCommand
{
    /// <summary>
    /// 负载均衡测试服务
    /// </summary>
    [fastCSharp.code.cSharp.tcpServer(Service = loadBalancingTcpCommand.server.ServiceName, IsLoadBalancing = true, IsIdentityCommand = true, IsServerAsynchronousReceive = false, IsClientAsynchronousReceive = false, IsRememberIdentityCommand = false)]//, VerifyMethodType = typeof(server.tcpClient.timeVerifyMethod)
    public partial class server
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public const string ServiceName = "loadBalancingTest";
        ///// <summary>
        ///// 任务完成输出数量
        ///// </summary>
        //private const int outputCount = 1 << 10;
        ///// <summary>
        ///// 当前任务完成输出数量
        ///// </summary>
        //private static int currentCount = outputCount;
        /// <summary>
        /// 同步测试
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false, IsClientCallbackTask = false, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private int add(int left, int right)
        {
            //if (Interlocked.Decrement(ref currentCount) == 0)
            //{
            //    Interlocked.Add(ref currentCount, outputCount);
            //    Console.Write('.');
            //}
            return left + right;
        }
        /// <summary>
        /// 异步测试
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="onReturn"></param>
        [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false, IsClientCallbackTask = false, IsClientAsynchronous = true, IsClientSynchronous = false, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private void xor(int left, int right, Func<returnValue<int>, bool> onReturn)
        {
            //if (Interlocked.Decrement(ref currentCount) == 0)
            //{
            //    Interlocked.Add(ref currentCount, outputCount);
            //    Console.Write('.');
            //}
            onReturn(left ^ right);
        }
    }
}
