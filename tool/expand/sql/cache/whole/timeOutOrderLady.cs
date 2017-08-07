using System;
using System.Collections.Generic;
using fastCSharp.code.cSharp;
using System.Threading;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql.cache.whole
{
    /// <summary>
    /// 超时缓存
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    public class timeOutOrderLady<valueType, modelType, memberCacheType>
        where valueType : class, modelType
        where modelType : class
        where memberCacheType : class
    {
        /// <summary>
        /// 整表缓存
        /// </summary>
        protected events.cache<valueType, modelType, memberCacheType> cache;
        /// <summary>
        /// 超时秒数
        /// </summary>
        protected double timeOutSeconds;
        /// <summary>
        /// 最小有效时间
        /// </summary>
        protected DateTime outTime
        {
            get
            {
                return date.NowSecond.AddSeconds(timeOutSeconds);
            }
        }
        /// <summary>
        /// 排序数据最小时间
        /// </summary>
        protected DateTime minTime;
        /// <summary>
        /// 时间获取器
        /// </summary>
        protected Func<valueType, DateTime> getTime;
        /// <summary>
        /// 数据集合
        /// </summary>
        protected HashSet<valueType> values;
        /// <summary>
        /// 数据数组
        /// </summary>
        protected valueType[] array;
        /// <summary>
        /// 数据数组是否排序
        /// </summary>
        protected bool isSort;
        /// <summary>
        /// 超时缓存
        /// </summary>
        /// <param name="cache">整表缓存</param>
        /// <param name="timeOutSeconds">超时秒数</param>
        /// <param name="getTime">时间获取器</param>
        /// <param name="isReset">是否绑定事件与重置数据</param>
        public timeOutOrderLady(events.cache<valueType, modelType, memberCacheType> cache
            , double timeOutSeconds, Func<valueType, DateTime> getTime, bool isReset)
        {
            if (cache == null || timeOutSeconds < 1 || getTime == null) log.Error.Throw(log.exceptionType.Null);
            this.cache = cache;
            this.timeOutSeconds = -timeOutSeconds;
            this.getTime = getTime;

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
            HashSet<valueType> newValues = hashSet<valueType>.Create();
            DateTime minTime = this.outTime;
            foreach (valueType value in cache.Values)
            {
                if (getTime(value) > minTime) newValues.Add(value);
            }
            values = newValues;
            array = null;
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void onInserted(valueType value)
        {
            if (getTime(value) > outTime)
            {
                values.Add(value);
                array = null;
            }
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="value">更新后的数据</param>
        /// <param name="oldValue">更新前的数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void onUpdated(valueType cacheValue, valueType value, valueType oldValue, fastCSharp.code.memberMap<modelType> memberMap)
        {
            onInserted(cacheValue);
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">被删除的数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void onDeleted(valueType value)
        {
            if (values.Remove(value)) array = null;
        }
        /// <summary>
        /// 删除过期数据
        /// </summary>
        private void remove()
        {
            DateTime outTime = this.outTime;
            subArray<valueType> removeValues = this.values.getFind(value => getTime(value) < outTime);
            int count = removeValues.Count;
            if (count != 0)
            {
                foreach (valueType value in removeValues.UnsafeArray)
                {
                    this.values.Remove(value);
                    if (--count == 0) break;
                }
            }
        }
        /// <summary>
        /// 获取数据数量
        /// </summary>
        /// <returns>数据数量</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public int GetCount()
        {
            return GetArray().Length;
        }
        /// <summary>
        /// 获取数据集合
        /// </summary>
        /// <returns>数据集合</returns>
        public valueType[] GetArray()
        {
            valueType[] values = array;
            if (values == null || minTime < date.NowSecond)
            {
                Monitor.Enter(cache.SqlTool.Lock);
                try
                {
                    if ((values = array) == null || minTime < date.NowSecond)
                    {
                        remove();
                        values = array = this.values.getArray();
                        minTime = array.Length != 0 ? array.maxKey(getTime, DateTime.MinValue).AddSeconds(timeOutSeconds) : DateTime.MaxValue;
                        isSort = array.Length < 2;
                    }
                }
                finally { Monitor.Exit(cache.SqlTool.Lock); }
            }
            return values;
        }
        /// <summary>
        /// 获取排序后的数据集合
        /// </summary>
        /// <returns>排序后的数据集合</returns>
        public valueType[] GetSort()
        {
            Monitor.Enter(cache.SqlTool.Lock);
            try
            {
                if (array == null || minTime < date.NowSecond)
                {
                    remove();
                    array = this.values.getArray().getSortDesc(getTime);
                    minTime = array.Length != 0 ? getTime(array[array.Length - 1]).AddSeconds(timeOutSeconds) : DateTime.MaxValue;
                    isSort = true;
                }
                else if (!isSort) array = array.getSortDesc(getTime);
                return array;
            }
            finally { Monitor.Exit(cache.SqlTool.Lock); }
        }
    }
}
