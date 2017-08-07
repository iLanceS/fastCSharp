using System;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 计数队列缓存
    /// </summary>
    /// <typeparam name="keyType">关键字类型</typeparam>
    /// <typeparam name="valueType">数据类型</typeparam>
    public sealed class countQueueCache<keyType, valueType>
        where keyType : struct, IEquatable<keyType>
        where valueType : class
    {
        /// <summary>
        /// 缓存队列
        /// </summary>
        private fifoPriorityQueue<keyType, valueType> queue = new fifoPriorityQueue<keyType, valueType>();
        /// <summary>
        /// 最大尺寸
        /// </summary>
        private int maxSize;
        /// <summary>
        /// 计数队列
        /// </summary>
        /// <param name="maxSize">最大尺寸</param>
        public countQueueCache(int maxSize)
        {
            this.maxSize = maxSize;
            if (maxSize <= 0) maxSize = 1;
        }
        /// <summary>
        /// 获取或者设置缓存
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>缓存值</returns>
        public valueType this[keyType key]
        {
            get { return queue[key]; }
            set
            {
                queue.Set(key, value);
                if (queue.Count > maxSize) queue.UnsafePopValue();
            }
        }
        /// <summary>
        /// 判断是否包含关键字
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool ContainsKey(keyType key)
        {
            return queue.ContainsKey(key);
        }
        /// <summary>
        /// 清除缓存
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Clear()
        {
            queue.Clear();
        }
        /// <summary>
        /// 删除缓存值
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>是否存在缓存值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool Remove(keyType key)
        {
            valueType value;
            return queue.Remove(key, out value);
        }
    }
}
