using System;
using System.Threading;
using System.Runtime.CompilerServices;

namespace fastCSharp.threading
{
    /// <summary>
    /// 节点队列
    /// </summary>
    public static class queue
    {
        /// <summary>
        /// 单线程读取队列节点
        /// </summary>
        public abstract class singleDequeueNode
        {
            /// <summary>
            /// 队列处理线程
            /// </summary>
            internal abstract void Thread();
        }
    }
    /// <summary>
    /// 节点队列(支持1读1写同时处理)
    /// </summary>
    /// <typeparam name="nodeType">队列类型</typeparam>
    public struct queue<nodeType> where nodeType : queue<nodeType>.node
    {
        /// <summary>
        /// 节点
        /// </summary>
        public abstract class node
        {
            /// <summary>
            /// 下一个节点
            /// </summary>
            internal nodeType QueueNextNode;
            /// <summary>
            /// 清除第一个节点数据
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void UnsafeClearQueueNextNode()
            {
                QueueNextNode = null;
            }
        }
        /// <summary>
        /// 并发节点队列(警告：节点不能重用)
        /// </summary>
        public struct concurrent
        {
            /// <summary>
            /// 单线程读取队列节点
            /// </summary>
            public sealed class singleDequeueNode : queue.singleDequeueNode
            {
                /// <summary>
                /// 并发节点队列
                /// </summary>
                private concurrent queue;
                /// <summary>
                /// 数据处理委托
                /// </summary>
                private Action<nodeType> onNode;
                /// <summary>
                /// 异常处理
                /// </summary>
                private Action<Exception> onError;
                /// <summary>
                /// 是否正在线程处理中
                /// </summary>
                private int isThread;
                /// <summary>
                /// 单线程读取队列
                /// </summary>
                /// <param name="node"></param>
                /// <param name="onNode">数据处理委托</param>
                /// <param name="onError">异常处理</param>
                public singleDequeueNode(nodeType node, Action<nodeType> onNode, Action<Exception> onError = null)
                {
                    if (onNode == null) log.Error.Throw(log.exceptionType.Null);
                    this.onError = onError;
                    this.onNode = onNode;
                    queue = new concurrent(node);
                }
                /// <summary>
                /// 多线程并发添加节点
                /// </summary>
                /// <param name="node"></param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Enqueue(nodeType node)
                {
                    if (node == null) log.Error.Throw(log.exceptionType.Null);
                    EnqueueNotNull(node);
                }
                /// <summary>
                /// 多线程并发添加节点
                /// </summary>
                /// <param name="node"></param>
                internal void EnqueueNotNull(nodeType node)
                {
                    queue.EnqueueNotNull(node);
                    if (Interlocked.CompareExchange(ref isThread, 1, 0) == 0)
                    {
                        fastCSharp.threading.threadPool.TinyPool.FastStart(this, threading.thread.callType.SingleDequeueNode);
                    }
                }
                /// <summary>
                /// 队列处理线程
                /// </summary>
                internal override void Thread()
                {
                    thread();
                }
                /// <summary>
                /// 队列处理线程
                /// </summary>
                private void thread()
                {
                    do
                    {
                        try
                        {
                            nodeType node;
                            do
                            {
                                if ((node = queue.UnsafeDequeue()) == null)
                                {
                                    isThread = 0;
                                    if (queue.head.QueueNextNode == null || Interlocked.CompareExchange(ref isThread, 1, 0) != 0) return;
                                    node = queue.UnsafeDequeue();
                                }
                                onNode(node);
                            }
                            while (true);
                        }
                        catch (Exception error)
                        {
                            if (onError != null)
                            {
                                try
                                {
                                    onError(error);
                                }
                                catch { }
                            }
                        }
                    }
                    while (true);
                }
            }
            /// <summary>
            /// 第一个节点
            /// </summary>
            private nodeType head;
            /// <summary>
            /// 最后一个节点
            /// </summary>
            private nodeType end;
            /// <summary>
            /// 获取下一个单线程弹出数据
            /// </summary>
            public nodeType Next
            {
                get { return head.QueueNextNode; }
            }
            /// <summary>
            /// 节点队列
            /// </summary>
            /// <param name="node">默认节点，不能为null</param>
            public concurrent(nodeType node)
            {
                if (node == null) log.Error.Throw(log.exceptionType.Null);
                head = end = node;
            }
            /// <summary>
            /// 多线程并发添加节点
            /// </summary>
            /// <param name="node"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Enqueue(nodeType node)
            {
                if (node == null) log.Error.Throw(log.exceptionType.Null);
                EnqueueNotNull(node);
            }
            /// <summary>
            /// 多线程并发添加节点
            /// </summary>
            /// <param name="node"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void EnqueueNotNull(nodeType node)
            {
                while (Interlocked.CompareExchange(ref end.QueueNextNode, node, null) != null)
                {
                    Thread.Yield();
                    if (Interlocked.CompareExchange(ref end.QueueNextNode, node, null) == null) break;
                    Thread.Sleep(0);
                }
                end = node;
            }
            /// <summary>
            /// 单线程添加节点
            /// </summary>
            /// <param name="node">节点不能为null</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void UnsafeEnqueue(nodeType node)
            {
                end.QueueNextNode = node;
                end = node;
            }
            /// <summary>
            /// 多线程并发弹出节点
            /// </summary>
            /// <returns></returns>
            public nodeType Dequeue()
            {
                nodeType head = this.head, node = head.QueueNextNode;
                while (node != null)
                {
                    if (Interlocked.CompareExchange(ref this.head, node, head) == head) return node;
                    Thread.Yield();
                    if ((node = (head = this.head).QueueNextNode) == null) return null;
                    if (Interlocked.CompareExchange(ref this.head, node, head) == head) return node;
                    Thread.Sleep(0);
                    node = (head = this.head).QueueNextNode;
                }
                return null;
            }
            /// <summary>
            /// 单线程弹出节点
            /// </summary>
            /// <returns></returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public nodeType UnsafeDequeue()
            {
                nodeType node = head.QueueNextNode;
                if (node != null) head = node;
                return node;
            }
            /// <summary>
            /// 清除节点数据
            /// </summary>
            public void Clear()
            {
                do
                {
                    nodeType head = this.head, end = this.end;
                    if (head == end) return;
                    if (Interlocked.CompareExchange(ref this.head, end, head) == head) return;
                }
                while (true);
            }
            /// <summary>
            /// 单线程清除节点数据
            /// </summary>
            public void UnsafeClear()
            {
                head = end;
            }
        }
        /// <summary>
        /// 对象池节点
        /// </summary>
        public abstract class poolNode
        {
            /// <summary>
            /// 下一个节点
            /// </summary>
            internal poolNode QueueNextNode;
            /// <summary>
            /// 引用次数
            /// </summary>
            internal int UsedCount;
            /// <summary>
            /// 对象池节点
            /// </summary>
            /// <param name="usedCount">引用次数</param>
            protected poolNode(int usedCount)
            {
                UsedCount = usedCount;
            }
            /// <summary>
            /// 增加引用次数
            /// </summary>
            /// <returns>0表示成功</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal int TryUse()
            {
                int usedCount = UsedCount;
                if (usedCount == 0) return 1;
                return Interlocked.CompareExchange(ref UsedCount, usedCount + 1, usedCount) ^ usedCount;
            }
            /// <summary>
            /// 添加到对象池
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected internal void tryPush()
            {
                if (Interlocked.Decrement(ref UsedCount) == 0)
                {
                    QueueNextNode = null;
                    push();
                }
            }
            /// <summary>
            /// 添加到对象池
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void TryPush2()
            {
                if (Interlocked.Add(ref UsedCount, -2) == 0)
                {
                    QueueNextNode = null;
                    push();
                }
            }
            /// <summary>
            /// 添加到对象池
            /// </summary>
            protected abstract void push();
        }
        /// <summary>
        /// 对象池并发节点队列
        /// </summary>
        public struct concurrentPool
        {
            /// <summary>
            /// 第一个节点
            /// </summary>
            private poolNode head;
            /// <summary>
            /// 最后一个节点
            /// </summary>
            private poolNode end;
            /// <summary>
            /// 获取下一个单线程弹出数据
            /// </summary>
            public poolNode Next
            {
                get { return head.QueueNextNode; }
            }
            /// <summary>
            /// 节点队列
            /// </summary>
            /// <param name="node">默认节点，不能为null</param>
            public concurrentPool(poolNode node)
            {
                node.UsedCount += 2;
                head = end = node;
            }
            /// <summary>
            /// 多线程并发添加节点
            /// </summary>
            /// <param name="node"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Enqueue(poolNode node)
            {
                if (node == null) log.Error.Throw(log.exceptionType.Null);
                EnqueueNotNull(node);
            }
            /// <summary>
            /// 多线程并发添加节点
            /// </summary>
            /// <param name="node"></param>
            internal void EnqueueNotNull(poolNode node)
            {
                ++node.UsedCount;
                poolNode end;
                do
                {
                    while ((end = this.end).TryUse() != 0)
                    {
                        Thread.Yield();
                        if ((end = this.end).TryUse() == 0) break;
                        Thread.Sleep(0);
                    }
                    if (end == this.end)
                    {
                        if (Interlocked.CompareExchange(ref end.QueueNextNode, node, null) == null)
                        {
                            this.end = node;
                            end.tryPush();
                            return;
                        }
                        while (end == this.end) Thread.Sleep(0);
                    }
                    end.tryPush();
                }
                while (true);
            }
            /// <summary>
            /// 单线程添加节点
            /// </summary>
            /// <param name="node">节点不能为null</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void UnsafeEnqueue(poolNode node)
            {
                ++node.UsedCount;
                end.QueueNextNode = node;
                end = node;
            }
            /// <summary>
            /// 多线程并发弹出节点
            /// </summary>
            /// <returns></returns>
            public poolNode Dequeue()
            {
                poolNode head;
                do
                {
                    if ((head = this.head).QueueNextNode == null) return null;
                    if (head.TryUse() != 0)
                    {
                        Thread.Yield();
                        if ((head = this.head).QueueNextNode == null) return null;
                        if (head.TryUse() != 0)
                        {
                            Thread.Sleep(0);
                            continue;
                        }
                    }
                    if (head == this.head)
                    {
                        poolNode node = head.QueueNextNode;
                        if (Interlocked.CompareExchange(ref this.head, node, head) == head)
                        {
                            head.TryPush2();
                            return node;
                        }
                    }
                    head.tryPush();
                }
                while (true);
            }
            /// <summary>
            /// 单线程弹出节点
            /// </summary>
            /// <returns></returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public poolNode UnsafeDequeue()
            {
                if (this.head.QueueNextNode == null) return null;
                poolNode head = this.head;
                this.head = this.head.QueueNextNode;
                head.tryPush();
                return this.head;
            }
            /// <summary>
            /// 多线程并发清除节点数据
            /// </summary>
            public void Clear()
            {
                poolNode head;
                do
                {
                    if ((head = this.head) == this.end) return;
                    if (head.TryUse() != 0)
                    {
                        Thread.Yield();
                        if ((head = this.head) == this.end) return;
                        if (head.TryUse() != 0)
                        {
                            Thread.Sleep(0);
                            continue;
                        }
                    }
                    if (head == this.head)
                    {
                        poolNode end = this.end;
                        if (Interlocked.CompareExchange(ref this.head, end, head) == head)
                        {
                            poolNode node = head.QueueNextNode;
                            head.TryPush2();
                            while (node != end)
                            {
                                head = node.QueueNextNode;
                                node.tryPush();
                                if (head == end) return;
                                node = head.QueueNextNode;
                                head.tryPush();
                            }
                            return;
                        }
                    }
                    head.tryPush();
                }
                while (true);
            }
            /// <summary>
            /// 单线程清除节点数据
            /// </summary>
            public void UnsafeClear()
            {
                poolNode node;
                while (head != end)
                {
                    node = head.QueueNextNode;
                    head.tryPush();
                    if (node == end)
                    {
                        head = node;
                        return;
                    }
                    head = node.QueueNextNode;
                    node.tryPush();
                }
            }
        }
        /// <summary>
        /// 第一个节点
        /// </summary>
        private nodeType head;
        /// <summary>
        /// 最后一个节点
        /// </summary>
        private nodeType end;
        /// <summary>
        /// 获取下一个单线程弹出数据
        /// </summary>
        public nodeType Next
        {
            get { return head.QueueNextNode; }
        }
        /// <summary>
        /// 节点队列
        /// </summary>
        /// <param name="node">默认节点，不能为null</param>
        public queue(nodeType node)
        {
            node.QueueNextNode = null;
            head = end = node;
        }
        /// <summary>
        /// 单线程添加节点
        /// </summary>
        /// <param name="node"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Enqueue(nodeType node)
        {
            node.QueueNextNode = null;
            end.QueueNextNode = node;
            end = node;
        }
        /// <summary>
        /// 单线程弹出节点
        /// </summary>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public nodeType Dequeue()
        {
            nodeType node = head.QueueNextNode;
            if (node != null) head = node;
            return node;
        }
        /// <summary>
        /// 清除节点数据
        /// </summary>
        public void Clear()
        {
            head = end;
        }
    }
}
