using System;
using System.Collections.Generic;
using fastCSharp.threading;
using System.Runtime.CompilerServices;
using System.Threading;

namespace fastCSharp
{
    /// <summary>
    /// 字节数组trie图
    /// </summary>
    public abstract class memoryTrieGraph : trieGraph<byte, byte[]>
    {
        /// <summary>
        /// 树创建器
        /// </summary>
        private unsafe struct treeBuilder
        {
            /// <summary>
            /// 结束字符
            /// </summary>
            private byte* end;
            /// <summary>
            /// 当前节点
            /// </summary>
            public int Node;
            /// <summary>
            /// 创建树
            /// </summary>
            /// <param name="keys">字符数组</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Build(byte[] keys)
            {
                fixed (byte* start = keys)
                {
                    end = start + keys.Length;
                    build(start);
                }
                NodePool[Node].Value = keys;
            }
            /// <summary>
            /// 创建树
            /// </summary>
            /// <param name="start">当前字符位置</param>
            private void build(byte* start)
            {
                Node = NodePool[Node].Create(*start);
                if (++start != end) build(start);
            }
        }
        /// <summary>
        /// 创建trie树
        /// </summary>
        /// <param name="keys">关键字集合</param>
        public void BuildTree(IEnumerable<byte[]> keys)
        {
            if (keys != null)
            {
                treeBuilder treeBuilder = new treeBuilder();
                Monitor.Enter(nodeLock);
                try
                {
                    foreach (byte[] key in keys)
                    {
                        if (key != null && key.Length != 0)
                        {
                            treeBuilder.Node = boot;
                            treeBuilder.Build(key);
                        }
                    }
                }
                finally { Monitor.Exit(nodeLock); }
            }
        }
        /// <summary>
        /// 是否存在最小匹配
        /// </summary>
        /// <param name="values">匹配字节数组</param>
        /// <returns>是否存在最小匹配</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe bool IsMatchLess(byte[] values)
        {
            if (values != null && values.Length != 0)
            {
                fixed (byte* valueFixed = values) return isMatchLess(valueFixed, valueFixed + values.Length);
            }
            return false;
        }
        /// <summary>
        /// 是否存在最小匹配
        /// </summary>
        /// <param name="values">匹配字节数组</param>
        /// <returns>是否存在最小匹配</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe bool IsMatchLess(ref subArray<byte> values)
        {
            if (values.Count != 0)
            {
                fixed (byte* valueFixed = values.UnsafeArray)
                {
                    byte* start = valueFixed + values.StartIndex;
                    return isMatchLess(start, start + values.Count);
                }
            }
            return false;
        }
        /// <summary>
        /// 是否存在最小匹配
        /// </summary>
        /// <param name="start">匹配起始位置</param>
        /// <param name="end">匹配结束位置</param>
        /// <returns>是否存在最小匹配</returns>
        private unsafe bool isMatchLess(byte* start, byte* end)
        {
            node[] pool = NodePool;
            Dictionary<byte, int> bootNode = pool[boot].Nodes;
            byte[] value;
            for (int node = boot, nextNode = 0; start != end; ++start)
            {
                byte letter = *start;
                if (pool[node].GetLinkWhereNull(letter, ref nextNode, ref node) != 0)
                {
                    while (node != 0)
                    {
                        int isGetValue = pool[node].GetNodeOrLink(letter, ref nextNode, ref node, out value);
                        if (value != null) return true;
                        if (isGetValue == 0) break;
                    }
                    if (node == 0 && !bootNode.TryGetValue(letter, out nextNode)) nextNode = boot;
                }
                if (pool[nextNode].Value != null) return true;
                node = nextNode;
            }
            return false;
        }
    }
}
