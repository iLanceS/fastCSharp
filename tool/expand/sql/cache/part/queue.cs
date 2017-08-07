using System;
using System.Threading;
using System.Linq.Expressions;
using fastCSharp.code.cSharp;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql.cache.part
{
    /// <summary>
    /// 先进先出优先队列缓存
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="counterKeyType">缓存统计关键字类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    /// <typeparam name="cacheValueType">缓存数据类型</typeparam>
    public abstract class queue<valueType, modelType, counterKeyType, keyType, cacheValueType>
        : counterCache<valueType, modelType, counterKeyType>
        where valueType : class, modelType
        where modelType : class
        where keyType : IEquatable<keyType>
        where counterKeyType : IEquatable<counterKeyType>
        where cacheValueType : class
    {
        /// <summary>
        /// 缓存关键字获取器
        /// </summary>
        protected Func<modelType, keyType> getKey;
        /// <summary>
        /// 缓存默认最大容器大小
        /// </summary>
        protected int maxCount;
        /// <summary>
        /// 数据集合
        /// </summary>
        protected fifoPriorityQueue<keyType, cacheValueType> queueCache = new fifoPriorityQueue<keyType, cacheValueType>();
        /// <summary>
        /// 先进先出优先队列缓存
        /// </summary>
        /// <param name="counter">缓存计数器</param>
        /// <param name="getKey">缓存关键字获取器</param>
        /// <param name="maxCount">缓存默认最大容器大小</param>
        protected queue(events.counter<valueType, modelType, counterKeyType> counter, Expression<Func<modelType, keyType>> getKey, int maxCount)
            : base(counter)
        {
            if (getKey == null) log.Error.Throw(log.exceptionType.Null);
            counter.SqlTool.SetSelectMember(getKey);
            this.getKey = getKey.Compile();
            this.maxCount = maxCount <= 0 ? config.sql.Default.CacheMaxCount : maxCount;
        }
        /// <summary>
        /// 先进先出优先队列缓存
        /// </summary>
        /// <param name="counter">缓存计数器</param>
        /// <param name="getKey">缓存关键字获取器</param>
        /// <param name="maxCount">缓存默认最大容器大小</param>
        protected queue(events.counter<valueType, modelType, counterKeyType> counter, Func<modelType, keyType> getKey, int maxCount)
            : base(counter)
        {
            if (getKey == null) log.Error.Throw(log.exceptionType.Null);
            this.getKey = getKey;
            this.maxCount = maxCount <= 0 ? config.sql.Default.CacheMaxCount : maxCount;
        }
        /// <summary>
        /// 重置缓存
        /// </summary>
        protected void reset()
        {
            //showjim 没有清除缓存？
            queueCache.Clear();
        }
        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>缓存数据,失败返回null</returns>
        public cacheValueType TryGet(keyType key)
        {
            Monitor.Enter(counter.SqlTool.Lock);
            try
            {
                return queueCache.Get(key, null);
            }
            finally { Monitor.Exit(counter.SqlTool.Lock); }
        }
    }
    /// <summary>
    /// 先进先出优先队列缓存
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    public sealed class queue<valueType, modelType, keyType>
        : queue<valueType, modelType, keyType, keyType, valueType>
        where valueType : class, modelType
        where modelType : class
        where keyType : IEquatable<keyType>
    {
        /// <summary>
        /// 数据获取器
        /// </summary>
        private Func<keyType, fastCSharp.code.memberMap<modelType>, valueType> getValue;
        /// <summary>
        /// 缓存数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>缓存数据</returns>
        public valueType this[keyType key]
        {
            get
            {
                Monitor.Enter(counter.SqlTool.Lock);
                try
                {
                    valueType value = queueCache.Get(key, null);
                    if (value != null) return value;
                    if (getKey == counter.GetKey)
                    {
                        value = counter.Get(key);
                        if (value != null) return value;
                    }
                    if ((value = getValue(key, counter.MemberMap)) != null) onInserted(value);
                    return value;
                }
                finally { Monitor.Exit(counter.SqlTool.Lock); }
            }
        }
        /// <summary>
        /// 先进先出优先队列缓存
        /// </summary>
        /// <param name="counter">缓存计数器</param>
        /// <param name="getValue">数据获取器,禁止锁操作</param>
        /// <param name="maxCount">缓存默认最大容器大小</param>
        public queue(events.counter<valueType, modelType, keyType> counter
            , Func<keyType, fastCSharp.code.memberMap<modelType>, valueType> getValue, int maxCount = 0)
            : base(counter, counter.GetKey, maxCount)
        {
            if (getValue == null) log.Error.Throw(log.exceptionType.Null);
            this.getValue = getValue;

            counter.OnReset += reset;
            counter.SqlTool.OnInsertedLock += onInserted;
            counter.OnUpdated += onUpdated;
            counter.OnDeleted += onDeleted;
        }
        /// <summary>
        /// 增加数据
        /// </summary>
        /// <param name="value">新增的数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void onInserted(valueType value)
        {
            onInserted(value, getKey(value));
        }
        /// <summary>
        /// 增加数据
        /// </summary>
        /// <param name="value">新增的数据</param>
        /// <param name="key">关键字</param>
        private void onInserted(valueType value, keyType key)
        {
            queueCache[key] = counter.Add(value);
            if (queueCache.Count > maxCount) counter.Remove(queueCache.UnsafePopValue());
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="cacheValue">缓存数据</param>
        /// <param name="value">更新后的数据</param>
        /// <param name="oldValue">更新前的数据</param>
        private void onUpdated(valueType cacheValue, valueType value, valueType oldValue, fastCSharp.code.memberMap<modelType> memberMap)
        {
            if (cacheValue != null)
            {
                keyType key = getKey(value), oldKey = getKey(oldValue);
                if (!key.Equals(oldKey))
                {
                    valueType removeValue;
                    if (queueCache.Remove(oldKey, out removeValue)) queueCache.Set(key, cacheValue);
                    else onInserted(cacheValue, key);
                }
            }
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">被删除的数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void onDeleted(valueType value)
        {
            queueCache.Remove(getKey(value), out value);
        }
    }
}
