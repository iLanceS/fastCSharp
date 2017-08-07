using System;
using fastCSharp.threading;

namespace fastCSharp.test.concurrentNode
{
    /// <summary>
    /// 测试数据
    /// </summary>
    class node : queue<node>.node
    {
        /// <summary>
        /// 测试数据
        /// </summary>
        public int Value;
    }
}
