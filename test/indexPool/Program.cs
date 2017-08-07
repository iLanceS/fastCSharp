using System;
using System.Threading;
using fastCSharp.threading;

namespace fastCSharp.test.indexPool
{
    class Program
    {
        /// <summary>
        /// 索引池并发测试
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            int thread = 16, count = 100 * 10000;
            node[][] nodes = new node[thread][];
            for (int loop = 0, value = 0; loop != thread; ++loop)
            {
                node[] node = nodes[loop] = new node[count];
                for (int index = 0; index != count; node[index++] = new node { Value = value++ }) ;
            }
            do
            {
                new concurrent(thread, nodes);
                GC.Collect();
                Console.WriteLine("press quit to exit.");
            }
            while (Console.ReadLine()!= "quit");
        }

    }
}
