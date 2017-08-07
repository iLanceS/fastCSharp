using System;

namespace fastCSharp.deployClient
{
    class Program
    {
        static void Main(string[] args)
        {
            fastCSharp.net.tcp.deployServer.client client = new fastCSharp.net.tcp.deployServer.client();
            fastCSharp.net.tcp.deployServer.client.deployInfo[] deployInfos = client.Deploys;
            do
            {
                int index = 0;
                foreach (fastCSharp.net.tcp.deployServer.client.deployInfo deployInfo in deployInfos)
                {
                    Console.ForegroundColor = (index & 1) == 0 ? ConsoleColor.Red : ConsoleColor.White;
                    Console.WriteLine((index++).toString() + " -> " + deployInfo.ToString());
                }
                Console.ResetColor();
                Console.WriteLine("press quit to exit.");
                string command = Console.ReadLine();
                if (command == "quit") return;
                if (int.TryParse(command, out index) && (uint)index < (uint)client.Deploys.Length)
                {
                    try
                    {
                        Console.WriteLine("正在启动部署 " + deployInfos[index].ToString());
                        if (deployInfos[index].Deploy(client)) Console.WriteLine("部署启动完毕 " + deployInfos[index].ToString());
                        else Console.WriteLine("部署启动失败 " + deployInfos[index].ToString());
                    }
                    catch (Exception error)
                    {
                        Console.WriteLine(error.ToString());
                    }
                }
                else Console.WriteLine("Error Command");
                Console.WriteLine();
            }
            while (true);
        }
    }
}
