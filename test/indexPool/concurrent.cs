using System;
using System.Threading;
using fastCSharp.threading;

namespace fastCSharp.test.indexPool
{
    /// <summary>
    /// 并发测试
    /// </summary>
    class concurrent
    {
        /// <summary>
        /// 线程数量
        /// </summary>
        int pushThread;
        /// <summary>
        /// 索引池
        /// </summary>
        fastCSharp.indexPool<node> pool;
        /// <summary>
        /// 并发测试
        /// </summary>
        /// <param name="thread"></param>
        /// <param name="nodes"></param>
        public concurrent(int thread, node[][] nodes)
        {
            pool = new fastCSharp.indexPool<node>();
            using (task task = new task(pushThread = thread))
            {
                for (int loop = 0; loop != thread; ++loop) task.Add(push, nodes[loop]);
            }
            subArray<node> values = pool.GetArray();
            if (values.Count != 0)
            {
                Console.WriteLine("ERROR Count " + values.Count.toString());
            }
            pool.Dispose();
        }
        /// <summary>
        /// 并发测试
        /// </summary>
        /// <param name="nodes"></param>
        void push(node[] nodes)
        {
            Interlocked.Decrement(ref pushThread);
            while (pushThread != 0) Thread.Sleep(0);
            foreach (node node in nodes)
            {
                if ((node.Value & 511) == 0) Thread.Sleep(0);
                node.Index = pool.Push(node);
            }
            foreach (node node in nodes)
            {
                if ((node.Value & 511) == 0) Thread.Sleep(0);
                if (node != pool.GetUnsafeFree(node.Index))
                {
                    Console.WriteLine("ERROR node");
                }
            }
        }
    }
}
