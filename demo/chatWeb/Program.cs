﻿using System;
using System.Reflection;
using System.Threading;

namespace fastCSharp.demo.chatWeb
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                webConfig webConfig = new webConfig();
                using (fastCSharp.net.tcp.http.servers server = fastCSharp.net.tcp.http.servers.Create<webServer>(fastCSharp.net.tcp.host.FromDomain(webConfig.MainDomain), fastCSharp.net.tcp.host.FromDomain(webConfig.PollDomain)))
                {
                    if (server == null) Console.WriteLine("HTTP服务启动失败");
                    else
                    {
#if MONO
                        Console.WriteLine("请在浏览器中打开下面页面");
                        Console.WriteLine("http://" + webConfig.MainDomain + "/chat.html?user=user1");
                        Console.WriteLine("http://" + webConfig.MainDomain + "/chat.html?user=user2");
                        Console.WriteLine("http://" + webConfig.MainDomain + "/chat.html?user=user3");
#else
                        Thread.Sleep(100);
                        fastCSharp.diagnostics.process.StartNew("http://" + webConfig.MainDomain + "/chat.html?user=user1");
                        fastCSharp.diagnostics.process.StartNew("http://" + webConfig.MainDomain + "/chat.html?user=user2");
                        fastCSharp.diagnostics.process.StartNew("http://" + webConfig.MainDomain + "/chat.html?user=user3");
#endif
                        Console.WriteLine("Press quit to exit.");
                        while (Console.ReadLine() != "quit") ;
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
    }
}
