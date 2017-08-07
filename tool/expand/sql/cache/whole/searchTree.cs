using System;
using fastCSharp.code.cSharp;
using System.Threading;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql.cache.whole
{
    /// <summary>
    /// 搜索树缓存
    /// </summary>
    public interface ISearchTree
    {
        /// <summary>
        /// 获取缓存数量
        /// </summary>
        int Count { get; }
    }
    /// <summary>
    /// 搜索树缓存
    /// </summary>
    /// <typeparam name="valueType"></typeparam>
    public interface ISearchTree<valueType> : ISearchTree
    {
        /// <summary>
        /// 获取数据集合
        /// </summary>
        /// <returns>数据集合</returns>
        valueType[] GetArray();
        /// <summary>
        /// 获取逆序数据集合
        /// </summary>
        /// <returns>逆序数据集合</returns>
        valueType[] GetArrayDesc();
        /// <summary>
        /// 获取分页数据集合
        /// </summary>
        /// <param name="pageSize">分页大小</param>
        /// <param name="currentPage">分页页号</param>
        /// <param name="count">数据总数</param>
        /// <returns>分页数据集合</returns>
        valueType[] GetPage(int pageSize, int currentPage, out int count);
        /// <summary>
        /// 获取分页数据集合
        /// </summary>
        /// <typeparam name="arrayType"></typeparam>
        /// <param name="getValue"></param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="currentPage">分页页号</param>
        /// <param name="count">数据总数</param>
        /// <returns>分页数据集合</returns>
        arrayType[] GetPage<arrayType>(Func<valueType, arrayType> getValue, int pageSize, int currentPage, out int count);
        /// <summary>
        /// 获取逆序分页数据集合
        /// </summary>
        /// <param name="pageSize">分页大小</param>
        /// <param name="currentPage">分页页号</param>
        /// <param name="count">数据总数</param>
        /// <returns>逆序分页数据集合</returns>
        valueType[] GetPageDesc(int pageSize, int currentPage, out int count);
        /// <summary>
        /// 获取逆序分页数据集合
        /// </summary>
        /// <typeparam name="arrayType"></typeparam>
        /// <param name="getValue"></param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="currentPage">分页页号</param>
        /// <param name="count">数据总数</param>
        /// <returns>逆序分页数据集合</returns>
        arrayType[] GetPageDesc<arrayType>(Func<valueType, arrayType> getValue, int pageSize, int currentPage, out int count);
    }
    /// <summary>
    /// 搜索树缓存
    /// </summary>
    /// <typeparam name="valueType"></typeparam>
    /// <typeparam name="sortType"></typeparam>
    public interface ISearchTree<valueType, sortType> : ISearchTree<valueType>
    {
        /// <summary>
        /// 获取上一个数据
        /// </summary>
        /// <param name="key">排序关键字</param>
        /// <returns>上一个数据,失败返回null</returns>
        valueType GetPrevious(sortType key);
        /// <summary>
        /// 获取上一个数据
        /// </summary>
        /// <param name="key">排序关键字</param>
        /// <returns>上一个数据,失败返回null</returns>
        valueType GetNext(sortType key);
        /// <summary>
        /// 根据关键字比它小的节点数量
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>节点数量</returns>
        int CountLess(sortType key);
        /// <summary>
        /// 根据关键字比它大的节点数量
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>节点数量</returns>
        int CountThan(sortType key);
    }
    /// <summary>
    /// 搜索树缓存
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="sortType">排序关键字类型</typeparam>
    public class searchTree<valueType, modelType, sortType> : ISearchTree<valueType, sortType>
        where valueType : class, modelType
        where modelType : class
        where sortType : IComparable<sortType>
    {
        /// <summary>
        /// 整表缓存
        /// </summary>
        protected events.cache<valueType, modelType> cache;
        /// <summary>
        /// 排序关键字获取器
        /// </summary>
        protected Func<valueType, sortType> getSort;
        /// <summary>
        /// 搜索树缓存
        /// </summary>
        protected fastCSharp.searchTree<sortType, valueType> tree;
        /// <summary>
        /// 获取缓存数量
        /// </summary>
        public int Count
        {
            get { return tree.Count; }
        }
        /// <summary>
        /// 分组列表缓存
        /// </summary>
        /// <param name="cache">整表缓存</param>
        /// <param name="getSort">排序关键字获取器</param>
        /// <param name="isReset">是否绑定事件与重置数据</param>
        public searchTree(events.cache<valueType, modelType> cache, Func<valueType, sortType> getSort, bool isReset = true)
        {
            if (cache == null || getSort == null) log.Error.Throw(log.exceptionType.Null);
            this.cache = cache;
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
            fastCSharp.searchTree<sortType, valueType> newTree = new fastCSharp.searchTree<sortType, valueType>();
            foreach (valueType value in cache.Values)
            {
                newTree.Add(getSort(value), value);
            }
            tree = newTree;
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value">数据对象</param>
        protected virtual void onInserted(valueType value)
        {
            tree.Add(getSort(value), value);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="value">更新后的数据</param>
        /// <param name="oldValue">更新前的数据</param>
        protected virtual void onUpdated(valueType cacheValue, valueType value, valueType oldValue, fastCSharp.code.memberMap<modelType> memberMap)
        {
            onDeleted(oldValue);
            onInserted(cacheValue);
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">被删除的数据</param>
        protected virtual void onDeleted(valueType value)
        {
            if (!tree.Remove(getSort(value)))
            {
                log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
            }
        }
        /// <summary>
        /// 获取数据集合
        /// </summary>
        /// <returns>数据集合</returns>
        public valueType[] GetArray()
        {
            valueType[] values = null;
            Monitor.Enter(cache.SqlTool.Lock);
            try
            {
                values = tree.GetArray();
            }
            finally { Monitor.Exit(cache.SqlTool.Lock); }
            return values;
        }
        /// <summary>
        /// 获取逆序数据集合
        /// </summary>
        /// <returns>逆序数据集合</returns>
        public valueType[] GetArrayDesc()
        {
            valueType[] values = null;
            Monitor.Enter(cache.SqlTool.Lock);
            try
            {
                values = tree.GetArray();
            }
            finally { Monitor.Exit(cache.SqlTool.Lock); }
            return values.reverse();
        }
        /// <summary>
        /// 获取分页数据集合
        /// </summary>
        /// <param name="pageSize">分页大小</param>
        /// <param name="currentPage">分页页号</param>
        /// <param name="count">数据总数</param>
        /// <returns>分页数据集合</returns>
        public valueType[] GetPage(int pageSize, int currentPage, out int count)
        {
            valueType[] values = null;
            count = 0;
            Monitor.Enter(cache.SqlTool.Lock);
            try
            {
                array.page page = new array.page(count = tree.Count, pageSize, currentPage);
                values = tree.GetRange(page.SkipCount, page.CurrentPageSize);
            }
            finally { Monitor.Exit(cache.SqlTool.Lock); }
            return values;
        }
        /// <summary>
        /// 获取分页数据集合
        /// </summary>
        /// <typeparam name="arrayType"></typeparam>
        /// <param name="getValue"></param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="currentPage">分页页号</param>
        /// <param name="count">数据总数</param>
        /// <returns>分页数据集合</returns>
        public arrayType[] GetPage<arrayType>(Func<valueType, arrayType> getValue, int pageSize, int currentPage, out int count)
        {
            arrayType[] values = null;
            count = 0;
            Monitor.Enter(cache.SqlTool.Lock);
            try
            {
                array.page page = new array.page(count = tree.Count, pageSize, currentPage);
                values = tree.GetRange(page.SkipCount, page.CurrentPageSize, getValue);
            }
            finally { Monitor.Exit(cache.SqlTool.Lock); }
            return values;
        }
        /// <summary>
        /// 获取逆序分页数据集合
        /// </summary>
        /// <param name="pageSize">分页大小</param>
        /// <param name="currentPage">分页页号</param>
        /// <param name="count">数据总数</param>
        /// <returns>逆序分页数据集合</returns>
        public valueType[] GetPageDesc(int pageSize, int currentPage, out int count)
        {
            valueType[] values = null;
            count = 0;
            Monitor.Enter(cache.SqlTool.Lock);
            try
            {
                array.page page = new array.page(count = tree.Count, pageSize, currentPage);
                values = tree.GetRange(count - page.SkipCount - page.CurrentPageSize, page.CurrentPageSize);
            }
            finally { Monitor.Exit(cache.SqlTool.Lock); }
            return values.reverse();
        }
        /// <summary>
        /// 获取逆序分页数据集合
        /// </summary>
        /// <typeparam name="arrayType"></typeparam>
        /// <param name="getValue"></param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="currentPage">分页页号</param>
        /// <param name="count">数据总数</param>
        /// <returns>逆序分页数据集合</returns>
        public arrayType[] GetPageDesc<arrayType>(Func<valueType, arrayType> getValue, int pageSize, int currentPage, out int count)
        {
            arrayType[] values = null;
            count = 0;
            Monitor.Enter(cache.SqlTool.Lock);
            try
            {
                array.page page = new array.page(count = tree.Count, pageSize, currentPage);
                values = tree.GetRange(count - page.SkipCount - page.CurrentPageSize, page.CurrentPageSize, getValue);
            }
            finally { Monitor.Exit(cache.SqlTool.Lock); }
            return values.reverse();
        }
        /// <summary>
        /// 获取上一个数据
        /// </summary>
        /// <param name="key">排序关键字</param>
        /// <returns>上一个数据,失败返回null</returns>
        public valueType GetPrevious(sortType key)
        {
            Monitor.Enter(cache.SqlTool.Lock);
            try
            {
                int index = tree.IndexOf(key);
                if (index > 0) return tree.GetIndex(index - 1);
            }
            finally { Monitor.Exit(cache.SqlTool.Lock); }
            return null;
        }
        /// <summary>
        /// 获取上一个数据
        /// </summary>
        /// <param name="key">排序关键字</param>
        /// <returns>上一个数据,失败返回null</returns>
        public valueType GetNext(sortType key)
        {
            Monitor.Enter(cache.SqlTool.Lock);
            try
            {
                int index = tree.IndexOf(key) + 1;
                if (index != 0 && index < tree.Count) return tree.GetIndex(index + 1);
            }
            finally { Monitor.Exit(cache.SqlTool.Lock); }
            return null;
        }
        /// <summary>
        /// 根据关键字比它小的节点数量
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>节点数量</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public int CountLess(sortType key)
        {
            return tree.CountLess(key);
        }
        /// <summary>
        /// 根据关键字比它大的节点数量
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>节点数量</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public int CountThan(sortType key)
        {
            return tree.CountThan(key);
        }
    }
}
