using System;
using System.IO;

namespace fastCSharp.processCopy
{
    /// <summary>
    /// 进程复制重启服务
    /// </summary>
    public class console : fastCSharp.diagnostics.consoleLog
    {
        /// <summary>
        /// 进程复制重启服务端
        /// </summary>
        private fastCSharp.diagnostics.processCopyServer.tcpServer processCopyServer;
        /// <summary>
        /// 是否自动设置进程复制重启
        /// </summary>
        protected override bool isSetProcessCopy
        {
            get { return false; }
        }
        /// <summary>
        /// 启动进程复制重启服务
        /// </summary>
        protected override void start()
        {
            dispose();
            try
            {
                if ((processCopyServer = new fastCSharp.diagnostics.processCopyServer.tcpServer()).Start())
                {
                    output("processCopy服务已启动");
                    return;
                }
            }
            catch (Exception error)
            {
                output(error);
            }
            processCopyServer = null;
            output("processCopy服务启动失败");
            fastCSharp.threading.timerTask.Default.Add(start, date.NowSecond.AddSeconds(1));
        }
        /// <summary>
        /// 关闭进程复制重启服务
        /// </summary>
        protected override void dispose()
        {
            pub.Dispose(ref processCopyServer);
        }
    }
}
