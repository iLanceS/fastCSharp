using System;
using System.Reflection;
using fastCSharp.net.tcp;

namespace fastCSharp.demo.virtualHost
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (fastCSharp.net.tcp.http.servers server = fastCSharp.net.tcp.http.servers.Create("127.0.0.1"))
                {
                    if (server == null) Console.WriteLine("HTTP服务启动失败");
                    else
                    {
#if MONO
                        Console.WriteLine(@"请将 ajax.googleapis.com 映射到 127.0.0.1");
#else
                        Console.WriteLine(@"请修改 C:\WINDOWS\system32\drivers\etc\hosts 文件
127.0.0.1	ajax.googleapis.com

Press any key to request
http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js");
                        Console.ReadKey();
                        fastCSharp.diagnostics.process.StartNew("http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js");
#endif
                        Console.WriteLine(@"Press quit to exit.");
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
