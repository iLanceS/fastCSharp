using System;
using System.IO;
using System.Threading;

namespace fastCSharp.demo.sqlTableWeb
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
#if DEBUG
                FileInfo serverFile = new FileInfo((@"..\..\..\sqlTableCacheServer\bin\Debug\fastCSharp.demo.sqlTableCacheServer.exe").pathSeparator());
#else
                FileInfo serverFile = new FileInfo((@"..\..\..\sqlTableCacheServer\bin\Release\fastCSharp.demo.sqlTableCacheServer.exe").pathSeparator());
#endif
                if (serverFile.Exists)
                {
                    fastCSharp.diagnostics.process.StartNew(serverFile.FullName, "1");

                    webConfig webConfig = new webConfig();
                    using (fastCSharp.net.tcp.http.servers server = fastCSharp.net.tcp.http.servers.Create<webServer>(fastCSharp.net.tcp.host.FromDomain(webConfig.MainDomain)))
                    {
                        if (server == null) Console.WriteLine("HTTP服务启动失败");
                        else
                        {
                            Console.WriteLine("HTTP服务启动成功");
                            Thread.Sleep(1000);
                            fastCSharp.diagnostics.process.StartNew("http://" + webConfig.MainDomain + "/");

                            Console.WriteLine("Press quit to exit.");
                            while (Console.ReadLine() != "quit") ;
                            return;
                        }
                    }
                }
                else Console.WriteLine("没有找到数据服务 " + serverFile.FullName);
            }
            catch (Exception error)
            {
                Console.WriteLine(error.ToString());
            }
            Console.ReadKey();
        }
    }
}
