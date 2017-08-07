using System;

namespace fastCSharp.ui
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            fastCSharp.log.Default.Add("args : " + args.joinString('	'));
            //args = (@"demo.testCase	C:\fastCSharp\demo\testCase\	C:\fastCSharp\demo\testCase\bin\Release\fastCSharp.demo.testCase.dll	fastCSharp.demo.testCase	1").Split('	');
            fastCSharp.code.console.Start(args);
        }
    }
}
