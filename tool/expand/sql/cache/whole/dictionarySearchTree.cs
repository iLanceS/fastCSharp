using System;
using System.Collections.Generic;
using System.Threading;
using fastCSharp.code.cSharp;
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
    public class dictionarySearchTree<valueType, modelType, memberCacheType, keyType, sortType>
        where valueType : class, modelType
        where modelType : class
        where memberCacheType : class
        where keyType : IEquatable<keyType>
        where sortType : IComparable<sortType>
    {
        /// <summary>
        /// 整表缓存
        /// </summary>
        protected events.cache<valueType, modelType, memberCacheType> cache;
        /// <summary>
        /// 分组关键字获取器
        /// </summary>
        protected Func<valueType, keyType> getKey;
        /// <summary>
        /// 排序关键字获取器
        /// </summary>
        protected Func<valueType, sortType> getSort;
        /// <summary>
        /// 字典+搜索树缓存
        /// </summary>
        protected Dictionary<randomKey<keyType>, fastCSharp.searchTree<sortType, valueType>> groups;
        /// <summary>
        /// 分组列表缓存
        /// </summary>
        /// <param name="cache">整表缓存</param>
        /// <param name="getKey">分组关键字获取器</param>
        /// <param name="getSort">排序关键字获取器</param>
        /// <param name="isReset">是否初始化</param>
        public dictionarySearchTree
            (events.cache<valueType, modelType, memberCacheType> cache, Func<valueType, keyType> getKey, Func<valueType, sortType> getSort, bool isReset = true)
        {
            if (cache == null || getKey == null || getSort == null) log.Error.Throw(log.exceptionType.Null);
            this.cache = cache;
            this.getKey = getKey;
            this.getSort = getSort;

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
            Dictionary<randomKey<keyType>, fastCSharp.searchTree<sortType, valueType>> newValues = dictionary<randomKey<keyType>>.Create<fastCSharp.searchTree<sortType, valueType>>();
            fastCSharp.searchTree<sortType, valueType> tree;
            foreach (valueType value in cache.Values)
            {
                keyType key = getKey(value);
                if (!newValues.TryGetValue(key, out tree)) newValues.Add(key, tree = new fastCSharp.searchTree<sortType, valueType>());
                tree.Add(getSort(value), value);
            }
            groups = newValues;
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void onInserted(valueType value)
        {
            onInserted(value, getKey(value));
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value">数据对象</param>
        /// <param name="key">数据对象的关键字</param>
        protected void onInserted(valueType value, keyType key)
        {
            fastCSharp.searchTree<sortType, valueType> tree;
            if (!groups.TryGetValue(key, out tree)) groups.Add(key, tree = new fastCSharp.searchTree<sortType, valueType>());
            tree.Add(getSort(value), value);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="value">更新后的数据</param>
        /// <param name="oldValue">更新前的数据</param>
        protected void onUpdated(valueType cacheValue, valueType value, valueType oldValue, fastCSharp.code.memberMap<modelType> memberMap)
        {
            keyType key = getKey(value), oldKey = getKey(oldValue);
            if (key.Equals(oldKey))
            {
                sortType sortKey = getSort(value), oldSortKey = getSort(oldValue);
                if (!sortKey.Equals(oldSortKey))
                {
                    fastCSharp.searchTree<sortType, valueType> tree;
                    if (groups.TryGetValue(key, out tree) && tree.Remove(oldSortKey))
                    {
                        tree.Add(sortKey, cacheValue);
                    }
                    else log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
                }
            }
            else
            {
                onInserted(cacheValue, key);
                onDeleted(oldValue, oldKey);
            }
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">被删除的数据</param>
        /// <param name="key">分组关键字</param>
        protected void onDeleted(valueType value, keyType key)
        {
            fastCSharp.searchTree<sortType, valueType> tree;
            if (groups.TryGetValue(key, out tree) && tree.Remove(getSort(value)))
            {
                if (tree.Count == 0) groups.Remove(key);
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
            onDeleted(value, getKey(value));
        }
        /// <summary>
        /// 获取数据数量
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public int GetCount(keyType key)
        {
            fastCSharp.searchTree<sortType, valueType> tree;
            return groups.TryGetValue(key, out tree) ? tree.Count : 0;
        }
        /// <summary>
        /// 获取数据集合
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>数据集合</returns>
        public valueType[] GetArray(keyType key)
        {
            fastCSharp.searchTree<sortType, valueType> tree;
            Monitor.Enter(cache.SqlTool.Lock);
            try
            {
                if (groups.TryGetValue(key, out tree)) return tree.GetArray();
            }
            finally { Monitor.Exit(cache.SqlTool.Lock); }
            return nullValue<valueType>.Array;
        }
        /// <summary>
        /// 获取数据集合
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="isValue">数据匹配委托</param>
        /// <returns>数据集合</returns>
        public subArray<valueType> GetFind(keyType key, Func<valueType, bool> isValue)
        {
            fastCSharp.searchTree<sortType, valueType> tree;
            Monitor.Enter(cache.SqlTool.Lock);
            try
            {
                if (groups.TryGetValue(key, out tree)) return tree.GetFind(isValue);
            }
            finally { Monitor.Exit(cache.SqlTool.Lock); }
            return default(subArray<valueType>);
        }
        ///// <summary>
        ///// 获取逆序数据集合
        ///// </summary>
        ///// <param name="key">关键字</param>
        ///// <returns>逆序数据集合</returns>
        //[MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        //public valueType[] GetArrayDesc(keyType key)
        //{
        //    return GetArray(key).reverse();
        //}
        /// <summary>
        /// 获取逆序分页数据集合
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="currentPage">分页页号</param>
        /// <param name="count">数据总数</param>
        /// <returns>逆序分页数据集合</returns>
        public valueType[] GetPageDesc(keyType key, int pageSize, int currentPage, out int count)
        {
            valueType[] values = null;
            count = 0;
            fastCSharp.searchTree<sortType, valueType> tree;
            Monitor.Enter(cache.SqlTool.Lock);
            try
            {
                if (groups.TryGetValue(key, out tree))
                {
                    array.page page = new array.page(count = tree.Count, pageSize, currentPage);
                    values = tree.GetRange(count - page.SkipCount - page.CurrentPageSize, page.CurrentPageSize);
                }
            }
            finally { Monitor.Exit(cache.SqlTool.Lock); }
            return values.reverse();
        }
    }
}
