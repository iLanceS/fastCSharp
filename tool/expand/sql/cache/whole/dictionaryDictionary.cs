using System;
using fastCSharp.code.cSharp;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql.cache.whole
{
    /// <summary>
    /// 分组字典缓存
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="groupKeyType">分组关键字类型</typeparam>
    /// <typeparam name="keyType">字典关键字类型</typeparam>
    public class dictionaryDictionary<valueType, modelType, groupKeyType, keyType>
        where valueType : class, modelType
        where modelType : class
        where groupKeyType : IEquatable<groupKeyType>
        where keyType : IEquatable<keyType>
    {
        /// <summary>
        /// 整表缓存
        /// </summary>
        protected events.cache<valueType, modelType> cache;
        /// <summary>
        /// 分组关键字获取器
        /// </summary>
        protected Func<valueType, groupKeyType> getGroupKey;
        /// <summary>
        /// 字典关键字获取器
        /// </summary>
        protected Func<valueType, keyType> getKey;
        /// <summary>
        /// 分组数据
        /// </summary>
        protected Dictionary<randomKey<groupKeyType>, Dictionary<randomKey<keyType>, valueType>> groups;
        /// <summary>
        /// 分组字典缓存
        /// </summary>
        /// <param name="cache">整表缓存</param>
        /// <param name="getGroupKey">分组关键字获取器</param>
        /// <param name="getKey">字典关键字获取器</param>
        /// <param name="isReset">是否初始化数据</param>
        public dictionaryDictionary(events.cache<valueType, modelType> cache
            , Func<valueType, groupKeyType> getGroupKey, Func<valueType, keyType> getKey, bool isReset)
        {
            if (cache == null || getGroupKey == null || getKey == null) log.Error.Throw(log.exceptionType.Null);
            this.cache = cache;
            this.getGroupKey = getGroupKey;
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
            groups = dictionary<randomKey<groupKeyType>>.Create<Dictionary<randomKey<keyType>, valueType>>();
            foreach (valueType value in cache.Values) onInserted(value);
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void onInserted(valueType value)
        {
            onInserted(value, getGroupKey(value));
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value">数据对象</param>
        /// <param name="key"></param>
        protected void onInserted(valueType value, groupKeyType key)
        {
            Dictionary<randomKey<keyType>, valueType> values;
            if (!groups.TryGetValue(key, out values)) groups.Add(key, values = dictionary<randomKey<keyType>>.Create<valueType>());
            values.Add(getKey(value), value);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="value">更新后的数据</param>
        /// <param name="oldValue">更新前的数据</param>
        protected void onUpdated(valueType cacheValue, valueType value, valueType oldValue, fastCSharp.code.memberMap<modelType> memberMap)
        {
            groupKeyType groupKey = getGroupKey(value), oldGroupKey = getGroupKey(oldValue);
            if (groupKey.Equals(oldGroupKey))
            {
                keyType key = getKey(value), oldKey = getKey(oldValue);
                if (!key.Equals(oldKey))
                {
                    Dictionary<randomKey<keyType>, valueType> dictionary;
                    if (groups.TryGetValue(groupKey, out dictionary) && dictionary.Remove(oldKey))
                    {
                        dictionary.Add(key, cacheValue);
                    }
                    else log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
                }
            }
            else
            {
                onInserted(cacheValue, groupKey);
                onDeleted(oldValue, oldGroupKey);
            }
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">被删除的数据</param>
        /// <param name="groupKey">分组关键字</param>
        protected void onDeleted(valueType value, groupKeyType groupKey)
        {
            Dictionary<randomKey<keyType>, valueType> dictionary;
            if (groups.TryGetValue(groupKey, out dictionary) && dictionary.Remove(getKey(value)))
            {
                if (dictionary.Count == 0) groups.Remove(groupKey);
            }
            else log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">被删除的数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void onDeleted(valueType value)
        {
            onDeleted(value, getGroupKey(value));
        }
        /// <summary>
        /// 获取关键字集合
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public ICollection<randomKey<keyType>> GetKeys(groupKeyType key)
        {
            Dictionary<randomKey<keyType>, valueType> dictionary;
            if (groups.TryGetValue(key, out dictionary)) return dictionary.Keys;
            return nullValue<randomKey<keyType>>.Array;
        }
    }
}