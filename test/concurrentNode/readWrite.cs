using System;
using System.Threading;
using fastCSharp.threading;

namespace fastCSharp.test.concurrentNode
{
    /// <summary>
    /// 节点队列(支持1读1写同时处理)测试
    /// </summary>
    class readWrite
    {
        /// <summary>
        /// 线程数量
        /// </summary>
        int thread;
        /// <summary>
        /// 添加数据线程是否结束
        /// </summary>
        int pushEnd;
        /// <summary>
        /// 节点队列(支持1读1写同时处理)
        /// </summary>
        queue<node> queue;
        /// <summary>
        /// 数据集合
        /// </summary>
        subArray<node> values;
        /// <summary>
        /// 节点队列(支持1读1写同时处理)测试
        /// </summary>
        /// <param name="nodes"></param>
        public readWrite(node[] nodes)
        {
            queue = new queue<node>(new node());
            values = new subArray<node>(nodes.Length);
            using (task task = new task(thread = 2))
            {
                task.Add(push, nodes);
                task.Add(pop);
            }
            if (values.Count != nodes.Length)
            {
                Console.WriteLine("ERROR");
            }
            else
            {
                int value = 0;
                foreach (node node in values.UnsafeArray)
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
            Interlocked.Decrement(ref thread);
            while (thread != 0) Thread.Sleep(0);
            foreach (node node in nodes)
            {
                if ((node.Value & 1023) == 0) Thread.Sleep(0);
                queue.Enqueue(node);
            }
            pushEnd = 1;
            Console.WriteLine("push end");
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        void pop()
        {
            Interlocked.Decrement(ref thread);
            while (thread != 0) Thread.Sleep(0);
            do
            {
                node node = queue.Dequeue();
                if (node == null)
                {
                    if (pushEnd == 0) Thread.Sleep(0);
                    else break;
                }
                else
                {
                    if ((node.Value & 511) == 0) Thread.Sleep(0);
                    values.Add(node);
                }
            }
            while (true);
            Console.WriteLine("pop end");
        }
    }
}
