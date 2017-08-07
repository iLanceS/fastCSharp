using System;

namespace fastCSharp.test.objectPointer
{
    class Program
    {
        /// <summary>
        /// 获取对象指针测试
        /// </summary>
        /// <param name="args"></param>
        static unsafe void Main(string[] args)
        {
            Console.WriteLine(((ulong)(long)objectPointUnion.GetPoint(new object())).toHex16());
            Console.ReadKey();
        }
    }
}
