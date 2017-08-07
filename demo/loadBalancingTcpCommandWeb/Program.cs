using System;
using System.Reflection;
using fastCSharp.io;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace fastCSharp.demo.loadBalancingTcpCommandWeb
{
    class Program
    {
        static void Main(string[] args)
        {
#if MONO
            Console.WriteLine("Press 1 start client, else start server ...");
            if (Console.ReadKey().KeyChar != '1')
#else
            if (args.length() == 0 && !webConfig.config.Default.IsClient)
#endif
            {
                try
                {
#if HELLO
                    using (fastCSharp.net.tcp.http.servers server = fastCSharp.net.tcp.http.servers.Create<helloServer>(fastCSharp.net.tcp.host.FromDomain(webConfig.config.Default.Domain)))
#else
                    using (fastCSharp.net.tcp.http.servers server = fastCSharp.net.tcp.http.servers.Create<webServer>(fastCSharp.net.tcp.host.FromDomain(webConfig.config.Default.Domain)))
#endif
                    {
                        if (server == null) Console.WriteLine("HTTP服务启动失败");
                        else
                        {
#if ONLYWEB
#else
                            if (client.LoadBalancing())
#endif
                            {
#if MONO
                                Console.WriteLine("请以测试客户端模式打开一个新的 fastCSharp.demo.loadBalancingTcpCommandWeb.exe");
#else
                                if (webConfig.config.Default.IsStartClient)
                                {
                                    Thread.Sleep(2000);
                                    Console.WriteLine("Press any key to start request.");
                                    Console.ReadKey();
                                    fastCSharp.diagnostics.process.StartNew("fastCSharp.demo.loadBalancingTcpCommandWeb.exe", "0");
                                }
#endif
                                Console.WriteLine("Press quit to exit.");
                                while (Console.ReadLine() != "quit") ;
                            }
#if ONLYWEB
#else
                            else
                            {
                                Console.WriteLine("负载均衡服务启动失败");
                                Console.WriteLine("Press any key to exit.");
                                Console.ReadKey();
                            }
#endif
                            return;
                        }
                    }
                }
                catch (Exception error)
                {
                    Console.WriteLine(error.ToString());
                }
                Console.ReadKey();
            }
            else
            {
                int cpuCount = fastCSharp.pub.CpuCount, count = fastCSharp.pub.CpuCount * 64 * 1024;
                int maxSocketCount = cpuCount * webConfig.config.Default.ClientCountPerCPU, keepAliveCount = webConfig.config.Default.IsKeepAlive ? ((count <<= 2) + (maxSocketCount - 1)) / maxSocketCount : 1;
                //i5-4570 3.20GHz       物理机/1024     WMware/1024     WMware+Ubuntu/512
                //ONLYWEB,HELLO,KEEP    14W             11.5W           7.8/8.6W
                //ONLYWEB,KEEP          12W             9W              6.7/7.1W
                //KEEP                  7.8W            4.5W            3.2W/3.7W
                //ONLYWEB,HELLO         3W              2.3W            1.6W/2.2W
                //ONLYWEB               2.9W            2.2W            1.5W/1.6W
                //                      2.4W            1.8W            1.5W/1.8W
                //ONLYWEB 表示不启用TCP负载均衡节点。
                //HELLO   表示静态文件测试，否则表示动态请求测试。
                //KEEP    表示启用 Keep-Alive 长链接模式。
                Console.WriteLine("Press quit to exit.");
                using (client.task task = new client.task(maxSocketCount, keepAliveCount))
                {
                    Stopwatch time = new Stopwatch();
                    do
                    {
                        Console.WriteLine("Start request " + cpuCount.toString() + " * " + webConfig.config.Default.ClientCountPerCPU.toString() + " = " + maxSocketCount.toString());
                        task.ErrorCount = 0;
                        time.Restart();
                        task.Add(count);
                        task.Wait();
                        time.Stop();
                        task.CloseClient();
                        long milliseconds = time.ElapsedMilliseconds;
                        Console.WriteLine(@"
Finally[" + count.toString() + "] Error[" + task.ErrorCount.toString() + "] " + milliseconds.toString() + "ms" + (milliseconds == 0 ? null : ("[" + ((1000L * count) / milliseconds).toString() + "/s]")));
                    }
                    while (Console.ReadLine() != "quit");
                }
            }
        }
    }
}
