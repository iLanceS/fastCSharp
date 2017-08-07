using System;

namespace fastCSharp.deployServer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (fastCSharp.config.appSetting.IsCheckMemory) checkMemory.Add(typeof(Program));
            fastCSharp.diagnostics.consoleLog.Start(new console());
        }
    }
}
