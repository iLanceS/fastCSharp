using System;
using System.Threading;
using fastCSharp.threading;
using fastCSharp;

namespace fastCSharp.test.concurrentNode
{
    /// <summary>
    /// 并发测试
    /// </summary>
    class concurrent
    {
        /// <summary>
        /// 添加数据线程数量
        /// </summary>
        int pushThread;
        /// <summary>
        /// 结束添加数据线程数量
        /// </summary>
        int pushEndThread;
        /// <summary>
        /// 获取数据线程数量
        /// </summary>
        int popThread;
        /// <summary>
        /// 结束获取数据线程数量
        /// </summary>
        int popEndThread;
        /// <summary>
        /// 并发节点队列
        /// </summary>
        queue<node>.concurrent queue;
        /// <summary>
        /// 数据集合
        /// </summary>
        subArray<node>[] values;
        /// <summary>
        /// 并发测试
        /// </summary>
        /// <param name="thread"></param>
        /// <param name="nodes"></param>
        public concurrent(int thread, node[][] nodes)
        {
            int popThread = thread >> 2;
            queue = new queue<node>.concurrent(new node());
            values = new subArray<node>[popThread];
            using (task task = new task((pushThread = pushEndThread = thread) + (this.popThread = popEndThread = popThread)))
            {
                for (int loop = 0; loop != popThread; ++loop) task.Add(pop);
                for (int loop = 0; loop != thread; ++loop) task.Add(push, nodes[loop]);
            }
            node[] array = values.concat(node => node.ToArray());
            if (array.Length != thread * nodes[0].Length)
            {
                Console.WriteLine("ERROR");
            }
            else
            {
                int value = 0;
                foreach (node node in (array = array.getSort(node => node.Value)))
                {
                    if (value != node.Value)
                    {
                        Console.WriteLine("ERROR");
                        break;
                    }
                    node.UnsafeClearQueueNextNode();
                    ++value;
                }
            }
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="nodes"></param>
        void push(node[] nodes)
        {
            Interlocked.Decrement(ref pushThread);
            while (pushThread != 0) Thread.Sleep(0);
            foreach (node node in nodes)
            {
                if ((node.Value & 1023) == 0) Thread.Sleep(0);
                queue.Enqueue(node);
            }
            if (Interlocked.Decrement(ref pushEndThread) == 0) Console.WriteLine("concurrent push end");
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        void pop()
        {
            int index = Interlocked.Decrement(ref popThread);
            while (popThread != 0) Thread.Sleep(0);
            subArray<node> nodes = default(subArray<node>);
            do
            {
                node node = queue.Dequeue();
                if (node == null)
                {
                    if (pushEndThread == 0) break;
                    Console.Write(".");
                }
                else
                {
                    if ((node.Value & 511) == 0) Thread.Sleep(0);
                    nodes.Add(node);
                }
            }
            while (true);
            values[index] = nodes;
            if (Interlocked.Decrement(ref popEndThread) == 0) Console.WriteLine("concurrent pop end");
        }
    }
}
