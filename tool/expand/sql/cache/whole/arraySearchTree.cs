using System;
using System.Threading;
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
    public class arraySearchTree<valueType, modelType, sortType>
        where valueType : class, modelType
        where modelType : class
        where sortType : IComparable<sortType>
    {
        /// <summary>
        /// 整表缓存
        /// </summary>
        protected events.cache<valueType, modelType> cache;
        /// <summary>
        /// 数组索引获取器
        /// </summary>
        protected Func<valueType, int> getIndex;
        /// <summary>
        /// 排序关键字获取器
        /// </summary>
        protected Func<valueType, sortType> getSort;
        /// <summary>
        /// 字典+搜索树缓存
        /// </summary>
        protected version<fastCSharp.searchTree<sortType, valueType>>[] array;
        /// <summary>
        /// 分组列表缓存
        /// </summary>
        /// <param name="cache">整表缓存</param>
        /// <param name="getIndex">数组索引获取器</param>
        /// <param name="getSort">排序关键字获取器</param>
        /// <param name="isReset">是否初始化</param>
        public arraySearchTree
            (events.cache<valueType, modelType> cache, Func<valueType, int> getIndex, int arraySize, Func<valueType, sortType> getSort, bool isReset = true)
        {
            if (cache == null || getIndex == null || getSort == null) log.Error.Throw(log.exceptionType.Null);
            array = new version<fastCSharp.searchTree<sortType, valueType>>[arraySize];
            this.cache = cache;
            this.getIndex = getIndex;
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
            fastCSharp.searchTree<sortType, valueType>[] trees = new fastCSharp.searchTree<sortType, valueType>[array.Length];
            int index;
            foreach (valueType value in cache.Values)
            {
                index = getIndex(value);
                fastCSharp.searchTree<sortType, valueType> tree = trees[index];
                if (tree == null) trees[index] = tree = new fastCSharp.searchTree<sortType, valueType>();
                tree.Add(getSort(value), value);
            }
            index = 0;
            foreach (fastCSharp.searchTree<sortType, valueType> tree in trees) array[index++].SetVersion(tree);
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void onInserted(valueType value)
        {
            onInserted(value, getIndex(value));
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value">数据对象</param>
        /// <param name="index"></param>
        protected void onInserted(valueType value, int index)
        {
            fastCSharp.searchTree<sortType, valueType> tree = array[index].Value;
            if (tree == null)
            {
                (tree = new fastCSharp.searchTree<sortType, valueType>()).Add(getSort(value), value);
                array[index].SetVersion(tree);
            }
            else
            {
                ++array[index].Version;
                tree.Add(getSort(value), value);
                ++array[index].Version;
            }
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="value">更新后的数据</param>
        /// <param name="oldValue">更新前的数据</param>
        protected void onUpdated(valueType cacheValue, valueType value, valueType oldValue, fastCSharp.code.memberMap<modelType> memberMap)
        {
            int index = getIndex(value), oldIndex = getIndex(oldValue);
            if (index == oldIndex)
            {
                sortType sortKey = getSort(value), oldSortKey = getSort(oldValue);
                if (!sortKey.Equals(oldSortKey))
                {
                    fastCSharp.searchTree<sortType, valueType> tree = array[index].Value;
                    if (tree != null)
                    {
                        ++array[index].Version;
                        try
                        {
                            if (tree.Remove(oldSortKey))
                            {
                                tree.Add(sortKey, cacheValue);
                                return;
                            }
                        }
                        finally { ++array[index].Version; }
                    }
                    log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
                }
            }
            else
            {
                onInserted(cacheValue, index);
                onDeleted(oldValue, oldIndex);
            }
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">被删除的数据</param>
        /// <param name="index"></param>
        protected void onDeleted(valueType value, int index)
        {
            fastCSharp.searchTree<sortType, valueType> tree = array[index].Value;
            if (tree != null)
            {
                sortType sortKey = getSort(value);
                ++array[index].Version;
                try
                {
                    if (tree.Remove(sortKey)) return;
                }
                finally { ++array[index].Version; }
            }
            log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">被删除的数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void onDeleted(valueType value)
        {
            onDeleted(value, getIndex(value));
        }
        /// <summary>
        /// 获取数据数量
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public int GetCount(int index)
        {
            fastCSharp.searchTree<sortType, valueType> tree = array[index].Value;
            return tree != null ? tree.Count : 0;
        }
        /// <summary>
        /// 获取数据集合
        /// </summary>
        /// <param name="index"></param>
        /// <returns>数据集合</returns>
        public valueType[] GetArray(int index)
        {
            fastCSharp.searchTree<sortType, valueType> tree = array[index].Value;
            if (tree != null)
            {
                Monitor.Enter(cache.SqlTool.Lock);
                try
                {
                    return tree.GetArray();
                }
                finally { Monitor.Exit(cache.SqlTool.Lock); }
            }
            return nullValue<valueType>.Array;
        }
        /// <summary>
        /// 获取数据集合
        /// </summary>
        /// <param name="index"></param>
        /// <param name="isValue">数据匹配委托</param>
        /// <returns>数据集合</returns>
        public subArray<valueType> GetFind(int index, Func<valueType, bool> isValue)
        {
            fastCSharp.searchTree<sortType, valueType> tree = array[index].Value;
            if (tree != null)
            {
                Monitor.Enter(cache.SqlTool.Lock);
                try
                {
                    return tree.GetFind(isValue);
                }
                finally { Monitor.Exit(cache.SqlTool.Lock); }
            }
            return default(subArray<valueType>);
        }
        /// <summary>
        /// 获取逆序分页数据集合
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="currentPage">分页页号</param>
        /// <param name="count">数据总数</param>
        /// <returns>逆序分页数据集合</returns>
        public valueType[] GetPageDesc(int index, int pageSize, int currentPage, out int count)
        {
            valueType[] values = null;
            count = 0;
            fastCSharp.searchTree<sortType, valueType> tree = array[index].Value;
            if (tree != null)
            {
                Monitor.Enter(cache.SqlTool.Lock);
                try
                {
                    array.page page = new array.page(count = tree.Count, pageSize, currentPage);
                    values = tree.GetRange(count - page.SkipCount - page.CurrentPageSize, page.CurrentPageSize);
                }
                finally { Monitor.Exit(cache.SqlTool.Lock); }
            }
            return values.reverse();
        }

        /// <summary>
        /// 获取逆序分页数据集合
        /// </summary>
        /// <typeparam name="arrarType"></typeparam>
        /// <param name="index"></param>
        /// <param name="getValue"></param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="currentPage">分页页号</param>
        /// <param name="count">数据总数</param>
        /// <returns>逆序分页数据集合</returns>
        public arrarType[] GetPageDesc<arrarType>(int index, Func<valueType, arrarType> getValue, int pageSize, int currentPage, out int count)
        {
            arrarType[] values = null;
            count = 0;
            fastCSharp.searchTree<sortType, valueType> tree = array[index].Value;
            if (tree != null)
            {
                Monitor.Enter(cache.SqlTool.Lock);
                try
                {
                    array.page page = new array.page(count = tree.Count, pageSize, currentPage);
                    values = tree.GetRange(count - page.SkipCount - page.CurrentPageSize, page.CurrentPageSize, getValue);
                }
                finally { Monitor.Exit(cache.SqlTool.Lock); }
            }
            return values.reverse();
        }
        /// <summary>
        /// 根据关键字比它小的节点数量
        /// </summary>
        /// <param name="index"></param>
        /// <param name="key">关键字</param>
        /// <returns>节点数量</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public int CountLess(int index, sortType key)
        {
            fastCSharp.searchTree<sortType, valueType> tree = array[index].Value;
            return tree != null ? tree.CountLess(key) : 0;
        }
        /// <summary>
        /// 根据关键字比它大的节点数量
        /// </summary>
        /// <param name="index"></param>
        /// <param name="key">关键字</param>
        /// <returns>节点数量</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public int CountThan(int index, sortType key)
        {
            fastCSharp.searchTree<sortType, valueType> tree = array[index].Value;
            return tree != null ? tree.CountThan(key) : 0;
        }
    }
}
