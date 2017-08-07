using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using fastCSharp.code.cSharp;

namespace fastCSharp.sql.cache.part
{
    /// <summary>
    /// 先进先出优先队列自定义缓存
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="counterKeyType">缓存统计关键字类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    /// <typeparam name="cacheValueType">缓存数据类型</typeparam>
    public class queueExpressionCustom<valueType, modelType, counterKeyType, keyType, cacheValueType>
        : queueExpression<valueType, modelType, counterKeyType, keyType, cacheValueType>
        where valueType : class, modelType
        where modelType : class
        where counterKeyType : IEquatable<counterKeyType>
        where keyType : IEquatable<keyType>
#if NOJIT
        where cacheValueType : class, ICustom
#else
        where cacheValueType : class, ICustom<valueType>
#endif
    {
        /// <summary>
        /// 自定义缓存获取器
        /// </summary>
        private Func<keyType, IEnumerable<valueType>, cacheValueType> getValue;
        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>缓存数据</returns>
        public cacheValueType this[keyType key]
        {
            get
            {
                Monitor.Enter(counter.SqlTool.Lock);
                try
                {
                    cacheValueType values = queueCache.Get(key, null);
                    if (values != null) return values;
                    queueCache[key] = values = getValue(key, counter.SqlTool.Where(getWhere(key), counter.MemberMap).getArray(value => counter.Add(value)));
                    if (queueCache.Count > maxCount)
                    {
                        foreach (valueType value in queueCache.UnsafePopValue().Values) counter.Remove(value);
                    }
                    return values;
                }
                finally { Monitor.Exit(counter.SqlTool.Lock); }
            }
        }
        /// <summary>
        /// 先进先出优先队列自定义缓存
        /// </summary>
        /// <param name="counter">缓存计数器</param>
        /// <param name="getKey">缓存关键字获取器</param>
        /// <param name="getWhere">条件表达式获取器</param>
        /// <param name="getValue">自定义缓存获取器</param>
        /// <param name="maxCount">缓存默认最大容器大小</param>
        public queueExpressionCustom(events.counter<valueType, modelType, counterKeyType> counter, Expression<Func<modelType, keyType>> getKey
            , Func<keyType, Expression<Func<modelType, bool>>> getWhere, Func<keyType, IEnumerable<valueType>, cacheValueType> getValue, int maxCount = 0)
            : base(counter, getKey, maxCount, getWhere)
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
        private void onInserted(valueType value)
        {
            keyType key = getKey(value);
            cacheValueType values = queueCache.Get(key, null);
            if (values != null && !values.Add(value = counter.Add(value))) counter.Remove(value);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="cacheValue">缓存数据</param>
        /// <param name="value">更新后的数据</param>
        /// <param name="oldValue">更新前的数据</param>
        private void onUpdated(valueType cacheValue, valueType value, valueType oldValue, fastCSharp.code.memberMap<modelType> memberMap)
        {
            keyType key = getKey(value);
            if (cacheValue == null)
            {
                cacheValueType values;
                if (queueCache.Remove(key, out values))
                {
                    foreach (valueType removeValue in values.Values) counter.Remove(removeValue);
                }
            }
            else
            {
                keyType oldKey = getKey(oldValue);
                cacheValueType values = queueCache.Get(key, null);
                if (key.Equals(oldKey))
                {
                    if (values != null)
                    {
                        int updateValue = values.Update(cacheValue, oldValue);
                        if (updateValue != 0)
                        {
                            if (updateValue > 1) counter.Add(cacheValue);
                            else counter.Remove(cacheValue);
                        }
                    }
                }
                else
                {
                    cacheValueType oldValues = queueCache.Get(oldKey, null);
                    if (values != null)
                    {
                        if (oldValues != null)
                        {
                            if (oldValues.Remove(oldValue))
                            {
                                if (!values.Add(cacheValue)) counter.Remove(cacheValue);
                            }
                            else if (values.Add(cacheValue)) counter.Add(cacheValue);
                        }
                        else if (values.Add(cacheValue)) counter.Add(cacheValue);
                    }
                    else if (oldValues != null && oldValues.Remove(oldValue)) counter.Remove(cacheValue);
                }
            }
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">被删除的数据</param>
        private void onDeleted(valueType value)
        {
            cacheValueType values = queueCache.Get(getKey(value), null);
            if (values != null) values.Remove(value);
        }
    }
}
