using System;
using System.Collections.Generic;
using fastCSharp.threading;
using System.Threading;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// trie图
    /// </summary>
    /// <typeparam name="keyType">关键字类型</typeparam>
    /// <typeparam name="valueType">节点值类型</typeparam>
    public abstract class trieGraph<keyType, valueType> : IDisposable
        where keyType : struct, IEquatable<keyType>
        where valueType : class
    {
        /// <summary>
        /// trie图节点
        /// </summary>
        protected internal struct node
        {
            /// <summary>
            /// 子节点
            /// </summary>
            internal Dictionary<keyType, int> Nodes;
            /// <summary>
            /// 子节点数量
            /// </summary>
            internal int NodeCount
            {
                get { return Nodes == null ? 0 : Nodes.Count; }
            }
            /// <summary>
            /// 失败节点
            /// </summary>
            internal int Link;
            /// <summary>
            /// 节点值
            /// </summary>
            public valueType Value;
            /// <summary>
            /// 清除节点信息
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void Clear()
            {
                Link = 0;
                Value = null;
            }
            /// <summary>
            /// 设置子节点
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void SetNodes()
            {
                if (Nodes == null) Nodes = dictionary.Create<keyType, int>();
            }
            /// <summary>
            /// 重置为根节点
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void SetBoot()
            {
                Clear();
                SetNodes();
            }
            /// <summary>
            /// 释放节点
            /// </summary>
            internal void Free()
            {
                Value = null;
                if (Nodes != null)
                {
                    foreach (int node in Nodes.Values) freeNodeNoLock(node);
                    Nodes.Clear();
                }
            }
            /// <summary>
            /// 创建子节点
            /// </summary>
            /// <param name="letter">当前字符</param>
            /// <returns>子节点</returns>
            public int Create(keyType letter)
            {
                int node;
                if (Nodes == null)
                {
                    Nodes = dictionary.Create<keyType, int>();
                    Nodes[letter] = node = nextNodeNoLock();
                }
                else if (!Nodes.TryGetValue(letter, out node))
                {
                    Nodes[letter] = node = nextNodeNoLock();
                }
                return node;
            }
            /// <summary>
            /// 设置失败节点并获取子节点数量
            /// </summary>
            /// <param name="link">失败节点</param>
            /// <returns>子节点数量</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal int GetNodeCount(int link)
            {
                Link = link;
                return Nodes == null ? 0 : Nodes.Count;
            }
            /// <summary>
            /// 设置失败节点并获取子节点数量
            /// </summary>
            /// <param name="boot">根节点</param>
            /// <param name="letter">当前字符</param>
            /// <returns>子节点数量</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal int GetNodeCount(Dictionary<keyType, int> boot, keyType letter)
            {
                boot.TryGetValue(letter, out Link);
                return Nodes == null ? 0 : Nodes.Count;
            }
            /// <summary>
            /// 子节点不存在时获取失败节点
            /// </summary>
            /// <param name="letter">当前字符</param>
            /// <param name="node">子节点</param>
            /// <param name="link">失败节点</param>
            /// <returns>失败节点</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal int GetLinkWhereNull(keyType letter, ref int node, ref int link)
            {
                if (Nodes == null || Nodes.Count == 0 || !Nodes.TryGetValue(letter, out node))
                {
                    return link = Link;
                }
                return 0;
            }
            /// <summary>
            /// 子节点不存在时获取失败节点
            /// </summary>
            /// <param name="letter">当前字符</param>
            /// <param name="node">子节点</param>
            /// <param name="link">失败节点</param>
            /// <param name="value">节点值</param>
            /// <returns>失败节点</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal int GetNodeOrLink(keyType letter, ref int node, ref int link, out valueType value)
            {
                value = Value;
                if (Nodes == null || Nodes.Count == 0 || !Nodes.TryGetValue(letter, out node))
                {
                    return link = Link;
                }
                return 0;
            }
            /// <summary>
            /// 获取失败节点
            /// </summary>
            /// <param name="link">失败节点</param>
            /// <returns>节点值</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal valueType GetLink(ref int link)
            {
                link = Link;
                return Value;
            }
        }
        /// <summary>
        /// trie图节点池
        /// </summary>
        internal static node[] NodePool = new node[256];
        /// <summary>
        /// 空闲节点集合
        /// </summary>
        private static readonly list<int> freeNodes = new list<int>();
        /// <summary>
        /// 当前分配节点
        /// </summary>
        private static int currentNode = 1;
        /// <summary>
        /// trie图节点池访问锁
        /// </summary>
        protected static readonly object nodeLock = new object();
        ///// <summary>
        ///// 获取下一个节点
        ///// </summary>
        ///// <returns>下一个节点</returns>
        //private static int nextNode()
        //{
        //    int node;
        //    interlocked.CompareSetSleep0NoCheck(ref nodeLock);
        //    if (freeNodes.Count == 0)
        //    {
        //        if (currentNode == nodePool.Length)
        //        {
        //            try
        //            {
        //                newNode[] nodes = new newNode[currentNode << 1];
        //                node = currentNode;
        //                nodePool.CopyTo(nodes, 0);
        //                ++currentNode;
        //                nodePool = nodes;
        //            }
        //            finally { nodeLock = 0; }
        //        }
        //        else
        //        {
        //            node = currentNode++;
        //            nodeLock = 0;
        //        }
        //    }
        //    else
        //    {
        //        nodePool[node = freeNodes.Unsafer.Pop()].Clear();
        //        nodeLock = 0;
        //    }
        //    return node;
        //}
        /// <summary>
        /// 获取根节点
        /// </summary>
        /// <returns>根节点</returns>
        private static int getBootNode()
        {
            int node;
            Monitor.Enter(nodeLock);
            if (freeNodes.Count == 0)
            {
                if (currentNode == NodePool.Length)
                {
                    try
                    {
                        node[] newNodes = new node[currentNode << 1];
                        node = currentNode;
                        NodePool.CopyTo(newNodes, 0);
                        ++currentNode;
                        NodePool = newNodes;
                        newNodes[node].SetNodes();
                    }
                    finally { Monitor.Exit(nodeLock); }
                }
                else
                {
                    NodePool[node = currentNode].SetNodes();
                    ++currentNode;
                    Monitor.Exit(nodeLock);
                }
            }
            else
            {
                NodePool[node = freeNodes.UnsafePop()].SetBoot();
                Monitor.Exit(nodeLock);
            }
            return node;
        }
        /// <summary>
        /// 获取下一个节点
        /// </summary>
        /// <returns>下一个节点</returns>
        private static int nextNodeNoLock()
        {
            if (freeNodes.Count == 0)
            {
                if (currentNode == NodePool.Length)
                {
                    node[] newNodes = new node[currentNode << 1];
                    NodePool.CopyTo(newNodes, 0);
                    NodePool = newNodes;
                }
                return currentNode++;
            }
            int node = freeNodes.UnsafePop();
            NodePool[node].Clear();
            return node;
        }
        /// <summary>
        /// 释放节点
        /// </summary>
        /// <param name="node">节点</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static void freeNode(int node)
        {
            Monitor.Enter(nodeLock);
            try
            {
                freeNodeNoLock(node);
            }
            finally { Monitor.Exit(nodeLock); }
        }
        /// <summary>
        /// 释放节点
        /// </summary>
        /// <param name="node">节点</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static void freeNodeNoLock(int node)
        {
            NodePool[node].Free();
            freeNodes.Add(node);
        }
        /// <summary>
        /// 图创建器
        /// </summary>
        private struct graphBuilder
        {
            /// <summary>
            /// trie图节点池
            /// </summary>
            public node[] pool;
            /// <summary>
            /// 根节点
            /// </summary>
            public Dictionary<keyType, int> Boot;
            /// <summary>
            /// 当前处理结果节点集合
            /// </summary>
            public list<int> Writer;
            /// <summary>
            /// 当前处理节点集合
            /// </summary>
            private int[] reader;
            /// <summary>
            /// 处理节点起始索引位置
            /// </summary>
            private int startIndex;
            /// <summary>
            /// 处理节点节点索引位置
            /// </summary>
            private int endIndex;
            /// <summary>
            /// 设置根节点
            /// </summary>
            /// <param name="node">根节点</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Set(ref node node)
            {
                Boot = node.Nodes;
                pool = NodePool;
                Writer = new list<int>();
            }
            /// <summary>
            /// 设置当前处理节点集合
            /// </summary>
            /// <param name="reader">当前处理节点集合</param>
            /// <param name="startIndex">处理节点起始索引位置</param>
            /// <param name="endIndex">处理节点节点索引位置</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Set(int[] reader, int startIndex, int endIndex)
            {
                this.reader = reader;
                this.startIndex = startIndex;
                this.endIndex = endIndex;
            }
            /// <summary>
            /// 建图
            /// </summary>
            public unsafe void Build()
            {
                Writer.Empty();
                fixed (int* readerFixed = reader)
                {
                    while (startIndex != endIndex)
                    {
                        node fatherNode = pool[readerFixed[startIndex++]];
                        if (fatherNode.Link == 0)
                        {
                            foreach (KeyValuePair<keyType, int> nextNode in fatherNode.Nodes)
                            {
                                if (pool[nextNode.Value].GetNodeCount(Boot, nextNode.Key) != 0) Writer.Add(nextNode.Value);
                            }
                        }
                        else
                        {
                            foreach (KeyValuePair<keyType, int> nextNode in fatherNode.Nodes)
                            {
                                int link = fatherNode.Link, linkNode = 0;
                                while (pool[link].GetLinkWhereNull(nextNode.Key, ref linkNode, ref link) != 0) ;
                                if (linkNode == 0) Boot.TryGetValue(nextNode.Key, out linkNode);
                                if (pool[nextNode.Value].GetNodeCount(linkNode) != 0) Writer.Add(nextNode.Value);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 根节点
        /// </summary>
        protected int boot = getBootNode();
        /// <summary>
        /// 建图
        /// </summary>
        /// <param name="threadCount">并行线程数量</param>
        public void BuildGraph(int threadCount = 0)
        {
            if (threadCount > fastCSharp.pub.CpuCount) threadCount = fastCSharp.pub.CpuCount;
            if (threadCount > 1) buildGraph(threadCount);
            else buildGraph();
        }
        /// <summary>
        /// 释放节点
        /// </summary>
        public void Dispose()
        {
            int boot = Interlocked.Exchange(ref this.boot, int.MinValue);
            if (boot > 0) freeNode(boot);
        }
        /// <summary>
        /// 单线程建图
        /// </summary>
        private unsafe void buildGraph()
        {
            node bootNode = NodePool[boot];
            graphBuilder builder = new graphBuilder();
            builder.Set(ref bootNode);
            Monitor.Enter(nodeLock);
            try
            {
                list<int> reader = getBuildGraphReader(ref bootNode);
                while (reader.Count != 0)
                {
                    builder.Set(reader.UnsafeArray, 0, reader.Count);
                    builder.Build();
                    list<int> values = reader;
                    reader = builder.Writer;
                    builder.Writer = values;
                }
            }
            finally { Monitor.Exit(nodeLock); }
        }
        /// <summary>
        /// 多线程并行建图
        /// </summary>
        /// <param name="threadCount">并行线程数量</param>
        private unsafe void buildGraph(int threadCount)
        {
            node bootNode = NodePool[boot];
            Monitor.Enter(nodeLock);
            try
            {
                list<int> reader = getBuildGraphReader(ref bootNode);
                if (reader.Count != 0)
                {
                    int taskCount = threadCount - 1;
                    graphBuilder[] builders = new graphBuilder[threadCount];
                    for (int builderIndex = 0; builderIndex != builders.Length; builders[builderIndex++].Set(ref bootNode)) ;
                    using (fastCSharp.threading.task task = new fastCSharp.threading.task(taskCount))
                    {
                        do
                        {
                            int[] readerArray = reader.UnsafeArray;
                            int count = reader.Count / threadCount, index = 0;
                            for (int builderIndex = 0; builderIndex != taskCount; ++builderIndex)
                            {
                                builders[builderIndex].Set(readerArray, index, index += count);
                                task.Add(builders[builderIndex].Build);
                            }
                            builders[taskCount].Set(readerArray, index, reader.Count);
                            builders[taskCount].Build();
                            task.WaitFree();
                            reader.Empty();
                            for (int builderIndex = 0; builderIndex != builders.Length; ++builderIndex)
                            {
                                list<int> writer = builders[builderIndex].Writer;
                                if (writer.Count != 0) reader.Add(writer.UnsafeArray, 0, writer.Count);
                            }
                        }
                        while (reader.Count != 0);
                    }
                }
            }
            finally { Monitor.Exit(nodeLock); }
        }
        /// <summary>
        /// 获取建图初始节点集合
        /// </summary>
        /// <param name="boot">根节点</param>
        /// <returns>建图初始节点集合</returns>
        private static unsafe list<int> getBuildGraphReader(ref node boot)
        {
            node[] pool = NodePool;
            list<int> reader = new list<int>(boot.Nodes.Count);
            fixed (int* readerFixed = reader.UnsafeArray)
            {
                int* write = readerFixed;
                foreach (int node in boot.Nodes.Values)
                {
                    node nodeValue = pool[node];
                    if (nodeValue.Nodes != null && nodeValue.Nodes.Count != 0) *write++ = node;
                }
                reader.AddLength((int)(write - readerFixed));
            }
            return reader;
        }
        static trieGraph()
        {
            if (fastCSharp.config.appSetting.IsCheckMemory) checkMemory.Add(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }
    }
}
