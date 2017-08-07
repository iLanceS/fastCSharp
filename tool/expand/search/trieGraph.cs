using System;
using System.Collections.Generic;
using fastCSharp.threading;
using System.Runtime.CompilerServices;
using System.Threading;

namespace fastCSharp.search
{
    /// <summary>
    /// 字符串trie图
    /// </summary>
    public sealed class trieGraph : fastCSharp.trieGraph<char, string>
    {
        /// <summary>
        /// 树创建器
        /// </summary>
        private unsafe struct treeBuilder
        {
            /// <summary>
            /// 结束字符
            /// </summary>
            private char* end;
            /// <summary>
            /// 当前节点
            /// </summary>
            public int Node;
            /// <summary>
            /// 创建树
            /// </summary>
            /// <param name="keys">字符数组</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Build(string keys)
            {
                fixed (char* start = keys)
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
            private void build(char* start)
            {
                Node = NodePool[Node].Create(*start);
                if (++start != end) build(start);
            }
        }
        /// <summary>
        /// 字符串trie图
        /// </summary>
        public trieGraph() { }
        /// <summary>
        /// 字符串trie图
        /// </summary>
        /// <param name="words">分词集合</param>
        /// <param name="threadCount">并行线程数量</param>
        public trieGraph(string[] words, int threadCount = 0)
        {
            BuildTree(words);
            BuildGraph(threadCount);
        }
        /// <summary>
        /// 创建trie树
        /// </summary>
        /// <param name="keys">关键字集合</param>
        public void BuildTree(IEnumerable<string> keys)
        {
            if (keys != null)
            {
                treeBuilder treeBuilder = new treeBuilder();
                Monitor.Enter(nodeLock);
                try
                {
                    foreach (string key in keys)
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
        public unsafe bool IsMatchLess(string values)
        {
            if (values != null && values.Length != 0)
            {
                fixed (char* valueFixed = values) return isMatchLess(valueFixed, valueFixed + values.Length);
            }
            return false;
        }
        /// <summary>
        /// 是否存在最小匹配
        /// </summary>
        /// <param name="values">匹配字节数组</param>
        /// <returns>是否存在最小匹配</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe bool IsMatchLess(ref subString values)
        {
            if (values.Length != 0)
            {
                fixed (char* valueFixed = values.Value)
                {
                    char* start = valueFixed + values.StartIndex;
                    return isMatchLess(start, start + values.Length);
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
        private unsafe bool isMatchLess(char* start, char* end)
        {
            node[] pool = NodePool;
            Dictionary<char, int> bootNode = pool[boot].Nodes;
            string value;
            for (int node = boot, nextNode = 0; start != end; ++start)
            {
                char letter = *start;
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
        /// <summary>
        /// 从左到右最大匹配
        /// </summary>
        /// <param name="text">匹配文本</param>
        /// <param name="matchs">匹配结果集合</param>
        private unsafe void leftRightMatchs(ref subString text, list<keyValue<int, int>> matchs)
        {
            node[] pool = NodePool;
            Dictionary<char, int> bootNode = pool[boot].Nodes;
            string value;
            int node = boot, nextNode = 0, index = text.StartIndex - 1, matchCount;
            fixed (char* valueFixed = text.Value)
            {
                for (char* start = valueFixed + text.StartIndex, end = start + text.Length; start != end; ++start)
                {
                    char letter = *start;
                    if (pool[node].GetLinkWhereNull(letter, ref nextNode, ref node) != 0)
                    {
                        while (node != 0)
                        {
                            int isGetValue = pool[node].GetNodeOrLink(letter, ref nextNode, ref node, out value);
                            if (value != null)
                            {
                                matchCount = matchs.Count;
                                matchs.AddLength();
                                matchs.UnsafeArray[matchCount].Set(index - value.Length + 1, value.Length);
                            }
                            if (isGetValue == 0) break;
                        }
                        if (node == 0 && !bootNode.TryGetValue(letter, out nextNode)) nextNode = boot;
                    }
                    ++index;
                    if ((value = pool[node = nextNode].Value) != null)
                    {
                        matchCount = matchs.Count;
                        matchs.AddLength();
                        matchs.UnsafeArray[matchCount].Set(index - value.Length + 1, value.Length);
                    }
                }
            }
            node = pool[node].Link;
            while (node != 0)
            {
                if ((value = pool[node].GetLink(ref node)) != null)
                {
                    matchCount = matchs.Count;
                    matchs.AddLength();
                    matchs.UnsafeArray[matchCount].Set(index - value.Length + 1, value.Length);
                }
            }
        }
        /// <summary>
        /// 从左到右最大匹配
        /// </summary>
        /// <param name="text">匹配文本</param>
        /// <param name="matchs">匹配结果集合</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe void LeftRightMatchs(ref subString text, list<keyValue<int, int>> matchs)
        {
            if (text != null && text.Length != 0 && matchs != null) leftRightMatchs(ref text, matchs);
        }
    }
}
