using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql.cache.whole
{
    /// <summary>
    /// 字典+搜索树缓存
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType">分组关键字类型</typeparam>
    /// <typeparam name="sortType">排序关键字类型</typeparam>
    public sealed class dictionarySearchTreeWhere<valueType, modelType, memberCacheType, keyType, sortType> : dictionarySearchTree<valueType, modelType, memberCacheType, keyType, sortType>
        where valueType : class, modelType
        where modelType : class
        where memberCacheType : class
        where keyType : IEquatable<keyType>
        where sortType : IComparable<sortType>
    {
        /// <summary>
        /// 缓存值判定
        /// </summary>
        private Func<valueType, bool> isValue;
        /// <summary>
        /// 分组列表缓存
        /// </summary>
        /// <param name="cache">整表缓存</param>
        /// <param name="getKey">分组关键字获取器</param>
        /// <param name="getSort">排序关键字获取器</param>
        /// <param name="isValue">缓存值判定</param>
        public dictionarySearchTreeWhere(events.cache<valueType, modelType, memberCacheType> cache, Func<valueType, keyType> getKey, Func<valueType, sortType> getSort, Func<valueType, bool> isValue)
            : base(cache, getKey, getSort, false)
        {
            if (isValue == null) log.Error.Throw(log.exceptionType.Null);
            this.isValue = isValue;

            cache.OnReset += reset;
            cache.OnInserted += onInserted;
            cache.OnUpdated += onUpdated;
            cache.OnDeleted += onDeleted;
            resetLock();
        }
        /// <summary>
        /// 重新加载数据
        /// </summary>
        protected override void reset()
        {
            Dictionary<randomKey<keyType>, fastCSharp.searchTree<sortType, valueType>> newValues = dictionary<randomKey<keyType>>.Create<fastCSharp.searchTree<sortType, valueType>>();
            fastCSharp.searchTree<sortType, valueType> tree;
            foreach (valueType value in cache.Values)
            {
                if (isValue(value))
                {
                    keyType key = getKey(value);
                    if (!newValues.TryGetValue(key, out tree)) newValues.Add(key, tree = new fastCSharp.searchTree<sortType, valueType>());
                    tree.Add(getSort(value), value);
                }
            }
            groups = newValues;
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private new void onInserted(valueType value)
        {
            if (isValue(value)) base.onInserted(value);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="value">更新后的数据</param>
        /// <param name="oldValue">更新前的数据</param>
        private new void onUpdated(valueType cacheValue, valueType value, valueType oldValue, fastCSharp.code.memberMap<modelType> memberMap)
        {
            if (isValue(value))
            {
                if (isValue(oldValue)) base.onUpdated(cacheValue, value, oldValue, memberMap);
                else base.onInserted(cacheValue);
            }
            else if (isValue(oldValue)) onDeleted(oldValue, getKey(oldValue));
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">被删除的数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private new void onDeleted(valueType value)
        {
            if (isValue(value)) base.onDeleted(value);
        }
    }
}
