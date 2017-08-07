using System;
using System.Reflection;

namespace fastCSharp.demo.loadBalancingTcpCommand
{
    class Program
    {
        static void Main(string[] args)
        {
#if MONO
            if (args.length() != 0)
            {
                int port;
                if (int.TryParse(args[0], out port)) console.Port = port;
            }
            fastCSharp.diagnostics.consoleLog.Start(new console());
#else
            if (args.length() == 0)
            {
                Console.WriteLine(@"请使用 fastCSharp.demo.loadBalancingTcpCommandWeb.exe 启动示例
Press any key to exit.");
                Console.ReadKey();
            }
            else
            {
                int port;
                if (int.TryParse(args[0], out port)) console.Port = port;
                fastCSharp.diagnostics.consoleLog.Start(new console());
            }
#endif
        }
    }
}
