using System;
using System.Net.Sockets;
using fastCSharp.threading;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;

namespace fastCSharp.net
{
    /// <summary>
    /// 超时队列
    /// </summary>
    public sealed class timeoutQueue
    {
        /// <summary>
        /// 超时检测接口
        /// </summary>
        public interface ICheckTimeout
        {
            /// <summary>
            /// 超时检测
            /// </summary>
            /// <param name="identity">超时标识</param>
            /// <returns>是否超时</returns>
            bool CheckTimeout(int identity);
        }
        /// <summary>
        /// 超时回调类型
        /// </summary>
        internal enum callbackType : byte
        {
            /// <summary>
            /// 超时检测接口
            /// </summary>
            ICheckTimeout,
            /// <summary>
            /// HTTP套接字
            /// </summary>
            Http,
        }
        /// <summary>
        /// 回调信息
        /// </summary>
        private struct callback
        {
            /// <summary>
            /// 超时时间
            /// </summary>
            public DateTime Timeout;
            /// <summary>
            /// 套接字
            /// </summary>
            private Socket socket;
            /// <summary>
            /// 是否超时判断
            /// </summary>
            private object checkTimeout;
            /// <summary>
            /// 超时判断标识
            /// </summary>
            private int identity;
            /// <summary>
            /// 超时回调类型
            /// </summary>
            private callbackType type;
            /// <summary>
            /// 设置回调信息
            /// </summary>
            /// <param name="checkTimeout">是否超时判断</param>
            /// <param name="timeout">超时时间</param>
            /// <param name="socket">套接字</param>
            /// <param name="idnetity">超时判断标识</param>
            /// <param name="type">超时回调类型</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Set(object checkTimeout, DateTime timeout, Socket socket, int idnetity, callbackType type)
            {
                Timeout = timeout;
                this.socket = socket;
                this.checkTimeout = checkTimeout;
                this.identity = idnetity;
                this.type = type;
            }
            /// <summary>
            /// 超时检测
            /// </summary>
            /// <param name="now"></param>
            /// <returns></returns>
            public int Check(ref DateTime now)
            {
                if (now < Timeout) now = date.Now;
                if (now >= Timeout)
                {
                    switch (type)
                    {
                        case callbackType.ICheckTimeout:
                            if (new unionType { Value = checkTimeout }.NetICheckTimeout.CheckTimeout(identity)) fastCSharp.threading.disposeTimer.Default.addSocketClose(socket);
                            break;
                        case callbackType.Http:
                            if (new unionType { Value = checkTimeout }.HttpSocket.CheckTimeout(identity)) fastCSharp.threading.disposeTimer.Default.addSocketClose(socket);
                            break;
                    }
                    socket = null;
                    checkTimeout = null;
                    return 1;
                }
                return 0;
            }
        }
        /// <summary>
        /// 队列节点
        /// </summary>
        private sealed class queueNode : fastCSharp.threading.timeoutQueue.queueNode<callback>
        {
            /// <summary>
            /// 下一个队列节点
            /// </summary>
            public queueNode NextNode;
            /// <summary>
            /// 是否需要检测下一个队列节点
            /// </summary>
            public queueNode CheckNextNode
            {
                get
                {
                    return endIndex == callbacks.Length ? NextNode : null;
                }
            }
            /// <summary>
            /// 下一个检测时间
            /// </summary>
            public DateTime CheckTime
            {
                get
                {
                    return startIndex == endIndex || startIndex == callbacks.Length ? DateTime.MinValue : callbacks[startIndex].Timeout;
                }
            }
            /// <summary>
            /// 是否存在未处理回调信息
            /// </summary>
            public bool IsAny
            {
                get
                {
                    return startIndex != endIndex || NextNode != null;
                }
            }
            /// <summary>
            /// 队列节点
            /// </summary>
            /// <param name="timeoutTicks">超时时钟周期</param>
            private queueNode(long timeoutTicks) : base(timeoutTicks) { }
            /// <summary>
            /// 添加超时回调信息
            /// </summary>
            /// <param name="checkTimeout">是否超时判断</param>
            /// <param name="socket">套接字</param>
            /// <param name="identity">超时判断标识</param>
            /// <param name="type">超时回调类型</param>
            /// <returns></returns>
            public int Add(object checkTimeout, Socket socket, int identity, callbackType type)
            {
                Monitor.Enter(callbackLock);
                if (endIndex == callbacks.Length)
                {
                    Monitor.Exit(callbackLock);
                    return 0;
                }
                callbacks[endIndex++].Set(checkTimeout, date.nowTime.Now.AddTicks(timeoutTicks), socket, identity, type);
                Monitor.Exit(callbackLock);
                return 1;
            }
            /// <summary>
            /// 超时检测
            /// </summary>
            public void Check()
            {
                Monitor.Enter(callbackLock);
                try
                {
                    for (DateTime now = date.nowTime.Now; startIndex != endIndex; ++startIndex)
                    {
                        if (callbacks[startIndex].Check(ref now) == 0) return;
                    }
                }
                finally { Monitor.Exit(callbackLock); }
            }
            /// <summary>
            /// 获取队列节点
            /// </summary>
            /// <param name="timeoutTicks"></param>
            /// <returns></returns>
            public static queueNode Get(long timeoutTicks)
            {
                queueNode node = typePool<queueNode>.Pop();
                if (node == null) node = new queueNode(timeoutTicks);
                else node.timeoutTicks = timeoutTicks;
                return node;
            }
        }
        /// <summary>
        /// 超时时钟周期
        /// </summary>
        private long timeoutTicks;
        /// <summary>
        /// 超时时钟周期
        /// </summary>
        public long CallbackTimeoutTicks
        {
            get { return timeoutTicks + date.SecondTicks; }
        }
        /// <summary>
        /// 起始队列节点
        /// </summary>
        private queueNode startNode;
        /// <summary>
        /// 结束队列节点
        /// </summary>
        private volatile queueNode endNode;
        /// <summary>
        /// 结束队列节点访问锁
        /// </summary>
        private readonly object endNodeLock = new object();
        /// <summary>
        /// 是否正在处理超时集合
        /// </summary>
        private int isTask;
        /// <summary>
        /// 超时队列
        /// </summary>
        /// <param name="seconds">超时秒数</param>
        private timeoutQueue(int seconds)
        {
            startNode = endNode = queueNode.Get(timeoutTicks = date.SecondTicks * seconds);
        }
        /// <summary>
        /// 添加超时回调信息
        /// </summary>
        /// <param name="checkTimeout">是否超时判断</param>
        /// <param name="socket">套接字</param>
        /// <param name="identity">超时判断标识</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Add(ICheckTimeout checkTimeout, Socket socket, int identity = 0)
        {
            Add(checkTimeout, socket, identity, callbackType.ICheckTimeout);
        }
        /// <summary>
        /// 添加超时回调信息
        /// </summary>
        /// <param name="checkTimeout">是否超时判断</param>
        /// <param name="socket">套接字</param>
        /// <param name="identity">超时判断标识</param>
        /// <param name="type">超时回调类型</param>
        internal void Add(object checkTimeout, Socket socket, int identity, callbackType type)
        {
            for (queueNode node = endNode; node.Add(checkTimeout, socket, identity, type) == 0; node = endNode)
            {
                Monitor.Enter(endNodeLock);
                if (node == endNode)
                {
                    Exception exception = null;
                    try
                    {
                        endNode.NextNode = queueNode.Get(timeoutTicks);
                        endNode = endNode.NextNode;
                    }
                    catch (Exception error)
                    {
                        exception = error;
                    }
                    finally { Monitor.Exit(endNodeLock); }
                    if (exception != null)
                    {
                        socket.shutdownConnected();
                        log.Error.Add(exception, null, false);
                        return;
                    }
                }
                else Monitor.Exit(endNodeLock);
            }
            if (Interlocked.CompareExchange(ref isTask, 1, 0) == 0) fastCSharp.threading.timerTask.Default.Add(this, thread.callType.NetTimeoutQueueCheck, date.Now.AddTicks(timeoutTicks));
        }
        /// <summary>
        /// 超时检测
        /// </summary>
        internal void Check()
        {
            do
            {
                startNode.Check();
                queueNode nextNode = startNode.CheckNextNode;
                if (nextNode == null)
                {
                    DateTime time = startNode.CheckTime;
                    if (time == DateTime.MinValue)
                    {
                        isTask = 0;
                        if (startNode.IsAny && Interlocked.CompareExchange(ref isTask, 1, 0) == 0)
                        {
                            fastCSharp.threading.timerTask.Default.Add(this, thread.callType.NetTimeoutQueueCheck, date.Now.AddTicks(timeoutTicks));
                        }
                    }
                    else fastCSharp.threading.timerTask.Default.Add(this, thread.callType.NetTimeoutQueueCheck, time.AddTicks(date.SecondTicks));
                    return;
                }
                queueNode node = startNode;
                startNode = nextNode;
                node.NextNode = null;
                typePool<queueNode>.PushNotNull(node);
            }
            while (true);
        }

        /// <summary>
        /// 超时队列
        /// </summary>
        private static readonly Dictionary<int, timeoutQueue> queues = dictionary.CreateInt<timeoutQueue>();
        /// <summary>
        /// 超时队列访问锁
        /// </summary>
        private static readonly object queueLock = new object();
        /// <summary>
        /// 获取超时队列
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static timeoutQueue Get(int seconds)
        {
            if (seconds <= 0) return null;
            timeoutQueue queue;
            Monitor.Enter(queueLock);
            if(queues.TryGetValue(seconds, out queue))
            {
                Monitor.Exit(queueLock);
                return queue;
            }
            try
            {
                queue = new timeoutQueue(seconds);
                queues.Add(seconds, queue);
            }
            finally { Monitor.Exit(queueLock); }
            return queue;
        }
    }
}
