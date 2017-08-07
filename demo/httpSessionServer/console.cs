using System;

namespace fastCSharp.demo.httpSessionServer
{
    /// <summary>
    /// HTTP服务
    /// </summary>
    public class console : fastCSharp.diagnostics.consoleLog
    {
        /// <summary>
        /// Session服务端
        /// </summary>
        private fastCSharp.net.tcp.http.session<string>.tcpServer sessionServer;
        /// <summary>
        /// 启动HTTP服务
        /// </summary>
        protected override void start()
        {
            dispose();
            try
            {
                if ((sessionServer = new fastCSharp.net.tcp.http.session<string>.tcpServer()).Start())
                {
                    output("Http Session服务已启动");
                    return;
                }
            }
            catch (Exception error)
            {
                output(error);
            }
            sessionServer = null;
            output("Http Session服务启动失败");
            fastCSharp.threading.timerTask.Default.Add(start, date.NowSecond.AddSeconds(1));
        }
        /// <summary>
        /// 关闭Session服务
        /// </summary>
        protected override void dispose()
        {
            pub.Dispose(ref sessionServer);
        }
    }
}
