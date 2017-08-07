using System;
#if MONO
#else
using System.ServiceProcess;
#endif

namespace fastCSharp.tcpRegister
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main(string[] args)
        {
#if MONO
#else
            if (fastCSharp.config.pub.Default.IsService)
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
			    { 
				    new service() 
			    };
                ServiceBase.Run(ServicesToRun);
            }
            else
#endif
            {
                fastCSharp.diagnostics.consoleLog.Start(new console());
            }
        }
    }
}
