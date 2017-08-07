using System;
using System.Threading;

namespace fastCSharp.sql.cache.whole
{
    /// <summary>
    /// 延时排序缓存数组
    /// </summary>
    /// <typeparam name="valueType"></typeparam>
    public sealed class ladyOrderArray<valueType>
        where valueType : class
    {
        /// <summary>
        /// 分组数据
        /// </summary>
        internal subArray<valueType> Array;
        /// <summary>
        /// 当前数据集合(不排序)
        /// </summary>
        internal subArray<valueType> CurrentArray
        {
            get
            {
                int count = Array.Count;
                return subArray<valueType>.Unsafe(Array.UnsafeArray, 0, count == 0 ? Array.StartIndex : count);
            }
        }
        /// <summary>
        /// 数据数量
        /// </summary>
        public int Count
        {
            get { return Array.StartIndex + Array.Count; }
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value"></param>
        internal void Insert(valueType value)
        {
            subArray<valueType> array = subArray<valueType>.Unsafe(Array.UnsafeArray, 0, Array.StartIndex + Array.Count);
            array.Add(value);
            Array.UnsafeSet(array.UnsafeArray, array.Count, 0);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="value"></param>
        internal void Update(valueType value)
        {
            int count = Array.StartIndex + Array.Count, index = System.Array.IndexOf(Array.UnsafeArray, value, 0, count);
            if (index == -1) log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
            else Array.UnsafeSet(count, 0);
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value"></param>
        internal void Delete(valueType value)
        {
            int index = System.Array.IndexOf(Array.UnsafeArray, value, 0, Array.StartIndex + Array.Count);
            if (index == -1) log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
            else if (Array.Count == 0)
            {
                valueType[] array = Array.UnsafeArray;
                Array.UnsafeSet(Array.StartIndex - 1, 0);
                array[index] = array[Array.StartIndex];
                array[Array.StartIndex] = null;
            }
            else Array.RemoveAt(index);
        }
        /// <summary>
        /// 获取排序数组
        /// </summary>
        /// <param name="sqlLock"></param>
        /// <param name="sorter"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        internal valueType At(object sqlLock, Func<subArray<valueType>, subArray<valueType>> sorter, int index)
        {
            Monitor.Enter(sqlLock);
            try
            {
                if (Array.StartIndex != 0)
                {
                    if ((uint)index < Array.StartIndex)
                    {
                        Array = sorter(subArray<valueType>.Unsafe(Array.UnsafeArray, 0, Array.StartIndex));
                        return Array.UnsafeArray[index];
                    }
                }
                else if ((uint)index < Array.Count) return Array.UnsafeArray[index];
            }
            finally { Monitor.Exit(sqlLock); }
            return null;
        }
        /// <summary>
        /// 获取排序数组
        /// </summary>
        /// <param name="sqlLock"></param>
        /// <param name="sorter"></param>
        /// <returns></returns>
        internal valueType[] GetArray(object sqlLock, Func<subArray<valueType>, subArray<valueType>> sorter)
        {
            Monitor.Enter(sqlLock);
            try
            {
                if (Array.StartIndex != 0) Array = sorter(subArray<valueType>.Unsafe(Array.UnsafeArray, 0, Array.StartIndex));
                return Array.GetArray();
            }
            finally { Monitor.Exit(sqlLock); }
        }
        /// <summary>
        /// 获取分页数据集合
        /// </summary>
        /// <param name="sqlLock"></param>
        /// <param name="sorter"></param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="currentPage">分页页号</param>
        /// <param name="count">数据总数</param>
        /// <returns>分页数据集合</returns>
        public valueType[] GetPage(object sqlLock, Func<subArray<valueType>, subArray<valueType>> sorter, int pageSize, int currentPage, out int count)
        {
            Monitor.Enter(sqlLock);
            try
            {
                if (Array.StartIndex != 0) Array = sorter(subArray<valueType>.Unsafe(Array.UnsafeArray, 0, Array.StartIndex));
                return Array.Page(pageSize, currentPage, out count).GetArray();
            }
            finally { Monitor.Exit(sqlLock); }
        }
        /// <summary>
        /// 获取分页数据集合
        /// </summary>
        /// <typeparam name="arrayType"></typeparam>
        /// <param name="sqlLock"></param>
        /// <param name="sorter"></param>
        /// <param name="getValue">数组转换</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="currentPage">分页页号</param>
        /// <param name="count">数据总数</param>
        /// <returns>分页数据集合</returns>
        public arrayType[] GetPage<arrayType>(object sqlLock, Func<subArray<valueType>, subArray<valueType>> sorter, Func<valueType, arrayType> getValue, int pageSize, int currentPage, out int count)
        {
            Monitor.Enter(sqlLock);
            try
            {
                if (Array.StartIndex != 0) Array = sorter(subArray<valueType>.Unsafe(Array.UnsafeArray, 0, Array.StartIndex));
                return Array.GetPage(getValue, pageSize, currentPage, out count);
            }
            finally { Monitor.Exit(sqlLock); }
        }
        /// <summary>
        /// 获取分页数据集合
        /// </summary>
        /// <param name="sqlLock">SQL表格锁</param>
        /// <param name="sorter">数据排序委托</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="currentPage">分页页号</param>
        /// <param name="count">数据总数</param>
        /// <returns>分页数据集合</returns>
        public valueType[] GetPageDesc(object sqlLock, Func<subArray<valueType>, subArray<valueType>> sorter, int pageSize, int currentPage, out int count)
        {
            Monitor.Enter(sqlLock);
            try
            {
                if (Array.StartIndex != 0) Array = sorter(subArray<valueType>.Unsafe(Array.UnsafeArray, 0, Array.StartIndex));
                return Array.GetPageDesc(pageSize, currentPage, out count);
            }
            finally { Monitor.Exit(sqlLock); }
        }
        /// <summary>
        /// 获取分页数据集合
        /// </summary>
        /// <typeparam name="arrayType"></typeparam>
        /// <param name="sqlLock">SQL表格锁</param>
        /// <param name="sorter">数据排序委托</param>
        /// <param name="getValue">数组转换</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="currentPage">分页页号</param>
        /// <param name="count">数据总数</param>
        /// <returns>分页数据集合</returns>
        public arrayType[] GetPageDesc<arrayType>(object sqlLock, Func<subArray<valueType>, subArray<valueType>> sorter, Func<valueType, arrayType> getValue, int pageSize, int currentPage, out int count)
        {
            Monitor.Enter(sqlLock);
            try
            {
                if (Array.StartIndex != 0) Array = sorter(subArray<valueType>.Unsafe(Array.UnsafeArray, 0, Array.StartIndex));
                return Array.GetPageDesc(getValue, pageSize, currentPage, out count);
            }
            finally { Monitor.Exit(sqlLock); }
        }
    }
}
