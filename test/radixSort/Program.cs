using System;
using System.Diagnostics;

namespace fastCSharp.test.radixSort
{
    class Program
    {
        /// <summary>
        /// 基数排序性能测试
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            int count = (1 << 10) * 10000;
            do
            {
                int32.Test(count);
                uint32.Test(count);
                int64.Test(count);
                uint64.Test(count);
                keyInt32.Test(count);
                keyUInt32.Test(count);
                keyInt64.Test(count);
                keyUInt64.Test(count);
                Console.WriteLine("press quit to exit.");
                
            }
            while (Console.ReadLine() != "quit");
        }
    }
}
