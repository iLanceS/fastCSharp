using System;
using System.Threading;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql.cache.whole
{
    /// <summary>
    /// 数组列表缓存
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    public class arrayList<valueType, modelType>
        where valueType : class, modelType
        where modelType : class
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
        /// 分组数据
        /// </summary>
        protected version<list<valueType>>[] array;
        /// <summary>
        /// 获取分组数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public version<list<valueType>> this[int index]
        {
            get
            {
                return array[index];
            }
        }
        /// <summary>
        /// 移除数据并使用最后一个数据移动到当前位置
        /// </summary>
        protected bool isRemoveEnd;
        /// <summary>
        /// 分组列表缓存
        /// </summary>
        /// <param name="cache">整表缓存</param>
        /// <param name="getIndex">数组索引获取器</param>
        /// <param name="arraySize">数组容器大小</param>
        /// <param name="isRemoveEnd">移除数据并使用最后一个数据移动到当前位置</param>
        /// <param name="isReset">是否绑定事件并重置数据</param>
        public arrayList(events.cache<valueType, modelType> cache, Func<valueType, int> getIndex, int arraySize, bool isRemoveEnd, bool isReset)
        {
            if (cache == null || getIndex == null) log.Error.Throw(log.exceptionType.Null);
            array = new version<list<valueType>>[arraySize];
            this.cache = cache;
            this.getIndex = getIndex;
            this.isRemoveEnd = isRemoveEnd;

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
            list<valueType>[] lists = new list<valueType>[array.Length];
            int index;
            foreach (valueType value in cache.Values)
            {
                index = getIndex(value);
                list<valueType> list = lists[index];
                if (list == null) lists[index] = list = new list<valueType>();
                list.Add(value);
            }
            index = 0;
            foreach (list<valueType> list in lists) array[index++].SetVersion(list);
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
        private void onInserted(valueType value, int index)
        {
            list<valueType> list = array[index].Value;
            if (list == null)
            {
                (list = new list<valueType>()).Add(value);
                array[index].SetVersion(list);
            }
            else
            {
                ++array[index].Version;
                list.Add(value);
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
            if (index != oldIndex)
            {
                onInserted(cacheValue, index);
                onDeleted(cacheValue, oldIndex);
            }
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">被删除的数据</param>
        /// <param name="index"></param>
        protected void onDeleted(valueType value, int index)
        {
            list<valueType> list = array[index].Value;
            if (list != null)
            {
                int valueIndex = Array.LastIndexOf(list.UnsafeArray, value, list.Count - 1);
                if (valueIndex != -1)
                {
                    if (isRemoveEnd)
                    {
                        ++array[index].Version;
                        list.RemoveAtEnd(valueIndex);
                    }
                    else
                    {
                        ++array[index].Version;
                        list.RemoveAt(valueIndex);
                    }
                    ++array[index].Version;
                    return;
                }
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
        /// 获取逆序分页数据
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <param name="currentPage"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public valueType[] GetPageDesc(int index, int pageSize, int currentPage, out int count)
        {
            version<list<valueType>> list = default(version<list<valueType>>);
            do
            {
                array[index].Get(ref list);
                count = list.Value.count();
                valueType[] values = list.Value.toSubArray().GetPageDesc(pageSize, currentPage);
                if (array[index].Version == list.Version) return values;
            }
            while (true);
        }
    }
}
