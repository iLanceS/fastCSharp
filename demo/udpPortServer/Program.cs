using System;
using fastCSharp.io;

namespace fastCSharp.demo.udpPortServer
{
    class Program
    {
        /// <summary>
        /// UDP穿透端口服务
        /// </summary>
        private static portServer portServerTarget;
#if NotFastCSharpCode
#else
        /// <summary>
        /// UDP穿透端口服务
        /// </summary>
        private static portServer.tcpServer portServer;
#endif
        /// <summary>
        /// UPD服务端
        /// </summary>
        private static udpServer updServer;
        /// <summary>
        /// TCP服务调用配置
        /// </summary>
        private static readonly fastCSharp.code.cSharp.tcpServer tcpServer = fastCSharp.code.cSharp.tcpServer.GetConfig("portServer", typeof(fastCSharp.demo.udpPortServer.portServer));

        static void Main(string[] args)
        {
#if NotFastCSharpCode
            fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.NotFastCSharpCode);
#else
            try
            {
                using (updServer = new udpServer(tcpServer))
                {
                    (portServerTarget = new portServer()).UpdServer = updServer;
                    using (portServer = new portServer.tcpServer(tcpServer, null, portServerTarget))
                    {
                        if (portServer.Start())
                        {
                            Console.WriteLine("UDP穿透端口服务已启动");
#if MONO
                            string path = "demo.udpPortClient";
#else
                            string path = "udpPortClient";
#endif
#if DEBUG
                            string clientFileName = (@"..\..\..\" + path + @"\bin\Debug\fastCSharp.demo.udpPortClient.exe").pathSeparator();
#else
                            string clientFileName = (@"..\..\..\" + path + @"\bin\Release\fastCSharp.demo.udpPortClient.exe").pathSeparator();
#endif
                            fastCSharp.diagnostics.process.StartNew(clientFileName, "1");
                            fastCSharp.diagnostics.process.StartNew(clientFileName, "2");
                            Console.WriteLine("Press quit to exit.");
                            while (Console.ReadLine() != "quit") ;
                            return;
                        }
                        Console.WriteLine("UDP穿透端口服务启动失败");
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error.ToString());
            }
            Console.ReadKey();
#endif
        }
    }
}
