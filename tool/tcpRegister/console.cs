using System;
using System.IO;

namespace fastCSharp.tcpRegister
{
    /// <summary>
    /// TCP注册服务
    /// </summary>
    public sealed class console : fastCSharp.diagnostics.consoleLog
    {
        /// <summary>
        /// TCP注册服务
        /// </summary>
        private fastCSharp.net.tcp.tcpRegisterReader.tcpServer readerServer;
        /// <summary>
        /// TCP注册服务
        /// </summary>
        private fastCSharp.net.tcp.tcpRegister.tcpServer server;
        /// <summary>
        /// 是否自动设置进程复制重启
        /// </summary>
        protected override bool isSetProcessCopy
        {
            get { return false; }
        }
        /// <summary>
        /// 启动TCP注册服务
        /// </summary>
        protected override void start()
        {
            dispose();
            fastCSharp.net.tcp.tcpRegisterReader value = fastCSharp.net.tcp.tcpRegisterReader.Create();
            if ((server = new fastCSharp.net.tcp.tcpRegister.tcpServer(null, value.TcpRegister)).Start())
            {
                output("TCP注册服务已启动");
                if ((readerServer = new fastCSharp.net.tcp.tcpRegisterReader.tcpServer(null, value)).Start())
                {
                    output("TCP注册读取服务已启动");
                    foreach (string process in fastCSharp.config.tcpRegister.Default.OnStartProcesses.notNull()) startProcess(process);
                }
                else output("TCP注册读取服务启动失败");
            }
            else output("TCP注册服务启动失败"); 
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        protected override void dispose()
        {
            if (readerServer != null)
            {
                pub.Dispose(ref server);
                pub.Dispose(ref readerServer);
                output("TCP注册服务已关闭");
            }
        }
    }
}
