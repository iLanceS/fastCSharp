using System;
using System.IO;
using fastCSharp.diagnostics;
using fastCSharp.threading;

namespace fastCSharp.httpServer
{
    /// <summary>
    /// HTTP服务
    /// </summary>
    public class console : fastCSharp.diagnostics.consoleLog
    {
        /// <summary>
        /// HTTP服务器实例
        /// </summary>
        private fastCSharp.net.tcp.http.servers server;
        /// <summary>
        /// HTTP服务端
        /// </summary>
        private fastCSharp.net.tcp.http.servers.tcpServer httpServer;
        /// <summary>
        /// 当前进程是否正在尝试忽略HTTP服务端操作
        /// </summary>
        private bool isTryIgnoreHttpServer;
        /// <summary>
        /// 是否自动设置进程复制重启
        /// </summary>
        protected override bool isSetProcessCopy
        {
            get { return false; }
        }
        /// <summary>
        /// 启动HTTP服务
        /// </summary>
        protected override void start()
        {
            dispose();
            try
            {
                server = new fastCSharp.net.tcp.http.servers();
                server.OnLoadCacheDomain += () =>
                {
                    isTryIgnoreHttpServer = true;
                    try
                    {
                        using (fastCSharp.net.tcp.http.servers.tcpClient httpClient = new fastCSharp.net.tcp.http.servers.tcpClient())
                        {
                            httpClient.TcpIgnoreGroup(int.MaxValue);
                            httpClient.TcpIgnoreGroup(0);
                        }
                    }
                    catch { }
                    finally { isTryIgnoreHttpServer = false; }
                };
                if ((httpServer = new fastCSharp.net.tcp.http.servers.tcpServer(null, server)).Start())
                {
                    setTcpIgnoreGroup(() => httpServer, () => isTryIgnoreHttpServer, () =>
                    {
                        fastCSharp.net.tcp.http.servers serverValue = server;
                        if (server != null) server.StopListen();
                    });
                    output("HttpServer服务已启动");
                    if (!server.IsLoadCache)
                    {
                        foreach (string process in fastCSharp.config.http.Default.OnStartProcesses.notNull())
                        {
                            try
                            {
                                startProcess(process);
                            }
                            catch (Exception error)
                            {
                                output(error);
                            }
                        }
                    }
                    if (!fastCSharp.config.pub.Default.IsService && fastCSharp.config.processCopy.Default.WatcherPath != null)
                    {
                        threadPool.TinyPool.Start(processCopyServer.Guard);
                    }
                    return;
                }
            }
            catch (Exception error)
            {
                output(error);
            }
            httpServer = null;
            output("HttpServer服务启动失败");
            fastCSharp.threading.timerTask.Default.Add(start, date.NowSecond.AddSeconds(1));
        }
        /// <summary>
        /// 删除进程复制重启
        /// </summary>
        protected override void removeProcessCopy()
        {
            processCopyServer.Remove();
        }
        /// <summary>
        /// 关闭HTTP服务
        /// </summary>
        protected override void dispose()
        {
            pub.Dispose(ref server);
            pub.Dispose(ref httpServer);
        }
    }
}
