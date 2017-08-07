using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace fastCSharp.demo.udpPortClient
{
    class Program
    {
        /// <summary>
        /// TCP服务调用配置
        /// </summary>
        private static readonly fastCSharp.code.cSharp.tcpServer tcpServer = fastCSharp.code.cSharp.tcpServer.GetConfig("portServer", typeof(fastCSharp.demo.udpPortServer.portServer));

        static void Main(string[] args)
        {
            if (args.length() == 0)
            {
                Console.WriteLine(@"请使用 fastCSharp.demo.udpPortServer.exe 启动示例
Press any key to exit.");
            }
            else
            {
                bool isQuit = false;
                try
                {
                    byte name = (byte)(args[0][0] & 1);
                    using (udpPortServer.portClient client = new udpPortServer.portClient(tcpServer, 30))
                    {
                        client.OnUdpSocket += socket =>
                        {
                            using (socket)
                            {
                                Console.WriteLine("IsReceive " + socket.IsReceive);
                                if (socket.IsReceive)
                                {
                                    int index = 0;
                                    byte[] receiveData = new byte[sizeof(int)];
                                    while (!isQuit)
                                    {
                                        try
                                        {
                                            EndPoint endPoint = socket.RemoteIp;
                                            socket.Socket.SendTo(BitConverter.GetBytes(index++), socket.RemoteIp);
                                            socket.Socket.ReceiveFrom(receiveData, ref endPoint);
                                            Console.WriteLine(BitConverter.ToInt32(receiveData, 0));
                                            Thread.Sleep(1000);
                                        }
                                        catch (Exception error)
                                        {
                                            Console.WriteLine(error.ToString());
                                            break;
                                        }
                                    }
                                }
                            }
                        };
                        client.Add(new byte[] { name }, new IPEndPoint(IPAddress.Parse(tcpServer.Host), tcpServer.Port + 2 + name), new byte[] { (byte)(name ^ 1) });
                        Console.WriteLine("Press quit to exit.");
                        while (Console.ReadLine() != "quit") ;
                        isQuit = true;
                        return;
                    }
                }
                catch (Exception error)
                {
                    Console.WriteLine(error.ToString());
                }
                isQuit = true;
            }
            Console.ReadKey();
        }
    }
}
