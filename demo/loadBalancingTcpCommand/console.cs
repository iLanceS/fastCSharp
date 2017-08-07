using System;

namespace fastCSharp.demo.loadBalancingTcpCommand
{
    /// <summary>
    /// 负载均衡测试服务
    /// </summary>
    public class console : fastCSharp.diagnostics.consoleLog
    {
        /// <summary>
        /// 服务端口
        /// </summary>
        internal static int Port = 1;
#if NotFastCSharpCode
#else
        /// <summary>
        /// 负载均衡测试服务
        /// </summary>
        private server.tcpServer loadBalancingServer;
#endif
        /// <summary>
        /// 负载均衡服务TCP服务调用配置
        /// </summary>
        private fastCSharp.code.cSharp.tcpServer loadBalancingServerAttribute = fastCSharp.code.cSharp.tcpServer.GetConfig(server.ServiceName + "LoadBalancing", typeof(fastCSharp.demo.loadBalancingTcpCommand.server));
        /// <summary>
        /// 负载均衡服务TCP服务调用配置
        /// </summary>
        private fastCSharp.code.cSharp.tcpServer serverAttribute = fastCSharp.code.cSharp.tcpServer.GetConfig(server.ServiceName, typeof(fastCSharp.demo.loadBalancingTcpCommand.server));
        /// <summary>
        /// TCP服务调用配置
        /// </summary>
        private fastCSharp.code.cSharp.tcpServer attribute;
        /// <summary>
        /// 启动负载均衡测试服务
        /// </summary>
        protected override void start()
        {
#if NotFastCSharpCode
            fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.NotFastCSharpCode);
#else
            if (attribute == null) attribute = fastCSharp.emit.memberCopyer<fastCSharp.code.cSharp.tcpServer>.MemberwiseClone(serverAttribute);
            dispose();
            try
            {
                attribute.Port = serverAttribute.Port + Port + (fastCSharp.random.Default.NextUShort() & 1023);
                if ((loadBalancingServer = new server.tcpServer(attribute)).StartLoadBalancing(loadBalancingServerAttribute, null, output))
                {
                    output("负载均衡测试服务已启动");
                    return;
                }
            }
            catch (Exception error)
            {
                output(error);
            }
            loadBalancingServer = null;
            output("负载均衡测试服务启动失败");
            fastCSharp.threading.timerTask.Default.Add(start, date.NowSecond.AddSeconds(1));
#endif
        }
        /// <summary>
        /// 关闭负载均衡测试服务
        /// </summary>
        protected override void dispose()
        {
#if NotFastCSharpCode
            fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.NotFastCSharpCode);
#else
            pub.Dispose(ref loadBalancingServer);
#endif
        }
    }
}
