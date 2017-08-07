using System;
using fastCSharp.code.cSharp;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql.cache.whole
{
    /// <summary>
    /// 字典缓存
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    public class dictionary<valueType, modelType, keyType>
        where valueType : class, modelType
        where modelType : class
        where keyType : IEquatable<keyType>
    {
        /// <summary>
        /// 整表缓存
        /// </summary>
        protected events.cache<valueType, modelType> cache;
        /// <summary>
        /// 分组字典关键字获取器
        /// </summary>
        protected Func<valueType, keyType> getKey;
        /// <summary>
        /// 字典缓存
        /// </summary>
        protected Dictionary<randomKey<keyType>, valueType> values;
        /// <summary>
        /// 缓存数据数量
        /// </summary>
        public int Count
        {
            get { return values.Count; }
        }
        /// <summary>
        /// 缓存数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>缓存数据,失败返回null</returns>
        public valueType this[keyType key]
        {
            get
            {
                valueType value;
                return values.TryGetValue(key, out value) ? value : null;
            }
        }
        /// <summary>
        /// 字典缓存
        /// </summary>
        /// <param name="cache">整表缓存</param>
        /// <param name="getKey">分组字典关键字获取器</param>
        /// <param name="isReset">是否初始化</param>
        public dictionary(events.cache<valueType, modelType> cache, Func<valueType, keyType> getKey, bool isReset = true)
        {
            if (cache == null || getKey == null) log.Error.Throw(log.exceptionType.Null);
            this.cache = cache;
            this.getKey = getKey;

            if (isReset)
            {
                cache.OnReset += reset;
                cache.OnInserted += onInserted;
                cache.OnUpdated += onUpdated;
                cache.OnDeleted += onDeleted;
                resetLock();
            }
        }
        /// <summary>
        /// 重新加载数据
        /// </summary>
        protected void resetLock()
        {
            Monitor.Enter(cache.SqlTool.Lock);
            try
            {
                reset();
            }
            finally { Monitor.Exit(cache.SqlTool.Lock); }
        }
        /// <summary>
        /// 重新加载数据
        /// </summary>
        protected virtual void reset()
        {
            Dictionary<randomKey<keyType>, valueType> newValues = dictionary<randomKey<keyType>>.Create<valueType>();
            foreach (valueType value in cache.Values) newValues.Add(getKey(value), value);
            values = newValues;
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value">数据对象</param>
        protected virtual void onInserted(valueType value)
        {
            onInserted(value, getKey(value));
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value">数据对象</param>
        /// <param name="key">数据对象的关键字</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void onInserted(valueType value, keyType key)
        {
            values.Add(key, value);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="value">更新后的数据</param>
        /// <param name="oldValue">更新前的数据</param>
        protected virtual void onUpdated(valueType cacheValue, valueType value, valueType oldValue, fastCSharp.code.memberMap<modelType> memberMap)
        {
            keyType key = getKey(value), oldKey = getKey(oldValue);
            if (!key.Equals(oldKey))
            {
                onInserted(cacheValue, key);
                onDeleted(oldKey);
            }
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="key">被删除数据的关键字</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void onDeleted(keyType key)
        {
            if (!values.Remove(key))
            {
                log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
            }
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">被删除的数据</param>
        protected virtual void onDeleted(valueType value)
        {
            onDeleted(getKey(value));
        }
        /// <summary>
        /// 获取数据集合
        /// </summary>
        /// <returns></returns>
        public valueType[] GetArray()
        {
            Monitor.Enter(cache.SqlTool.Lock);
            try
            {
                return values.Values.getArray();
            }
            finally { Monitor.Exit(cache.SqlTool.Lock); }
        }
        /// <summary>
        /// 获取数据集合
        /// </summary>
        /// <param name="isValue"></param>
        /// <returns></returns>
        public subArray<valueType> GetFindArray(Func<valueType, bool> isValue)
        {
            if (isValue == null) log.Default.Throw(log.exceptionType.Null);
            valueType[] array;
            int count = 0;
            Monitor.Enter(cache.SqlTool.Lock);
            try
            {
                array = new valueType[values.Count];
                foreach (valueType value in values.Values)
                {
                    if (isValue(value)) array[count++] = value;
                }
            }
            finally { Monitor.Exit(cache.SqlTool.Lock); }
            return subArray<valueType>.Unsafe(array, 0, count);
        }
    }
}
