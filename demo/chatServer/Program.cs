using System;
using fastCSharp.io;

namespace fastCSharp.demo.chatServer
{
    class Program
    {
        static void Main(string[] args)
        {
#if NotFastCSharpCode
            fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.NotFastCSharpCode);
#else
            server server = new server();
            using (server.tcpServer tcpServer = new server.tcpServer(null, null, server))
            {
                if (tcpServer.Start())
                {
                    Console.WriteLine("Server Start.");
#if DEBUG
                    string clientFileName = (@"..\..\..\chatClient\bin\Debug\fastCSharp.demo.chatClient.exe").pathSeparator();
#else
                    string clientFileName = (@"..\..\..\chatClient\bin\Release\fastCSharp.demo.chatClient.exe").pathSeparator();
#endif
                    server.OnLogin += (user) => Console.WriteLine(user + " 登录");
                    server.OnLogout += (user) => Console.WriteLine(user + " 退出"); ;
                    server.OnMessage += (message) => Console.WriteLine(message.User + " 发送消息");
                    fastCSharp.diagnostics.process.StartNew(clientFileName, "user1");
                    fastCSharp.diagnostics.process.StartNew(clientFileName, "user2");
                    fastCSharp.diagnostics.process.StartNew(clientFileName, "user3");
                    fastCSharp.diagnostics.process.StartNew(clientFileName, "user4");
                }
                else Console.WriteLine("Server Error.");
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
#endif
        }
    }
}
