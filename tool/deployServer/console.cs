using System;

namespace fastCSharp.deployServer
{
    /// <summary>
    /// 部署服务
    /// </summary>
    class console : fastCSharp.diagnostics.consoleLog
    {
        /// <summary>
        /// 部署服务
        /// </summary>
        private fastCSharp.net.tcp.deployServer.tcpServer deployServer;
        /// <summary>
        /// 当前进程是否正在尝试忽略部署服务端操作
        /// </summary>
        private bool isTryIgnoreServer;
        /// <summary>
        /// 启动部署服务
        /// </summary>
        protected override void start()
        {
            isTryIgnoreServer = true;
            try
            {
                using (fastCSharp.net.tcp.deployServer.tcpClient client = new fastCSharp.net.tcp.deployServer.tcpClient())
                {
                    client.TcpIgnoreGroup(int.MaxValue);
                    client.TcpIgnoreGroup(0);
                }
            }
            catch (Exception error)
            {
                output(error);
            }
            finally
            {
                isTryIgnoreServer = false;
                deploy();
            }
        }
        /// <summary>
        /// 启动部署服务
        /// </summary>
        private void deploy()
        {
            try
            {
                if ((deployServer = new fastCSharp.net.tcp.deployServer.tcpServer()).Start())
                {
                    setTcpIgnoreGroup(() => deployServer, () => isTryIgnoreServer);
                    output("部署服务 启动成功");
                    return;
                }
            }
            catch (Exception error)
            {
                output(error);
            }
            deployServer = null;
            output("部署服务 启动失败");
            fastCSharp.threading.timerTask.Default.Add(deploy, date.NowSecond.AddSeconds(1));
        }
        /// <summary>
        /// 关闭部署服务
        /// </summary>
        protected override void dispose()
        {
            pub.Dispose(ref deployServer);
        }
    }
}
