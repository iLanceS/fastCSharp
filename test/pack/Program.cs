using System;

namespace fastCSharp.test.pack
{
    class Program
    {
        /// <summary>
        /// 项目打包
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            new zip(@"..\..\..\..\", "fastCSharp.zip");
            Console.ReadKey();
        }
    }
}
