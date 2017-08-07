using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;

namespace fastCSharp.sql.cache.whole
{
    /// <summary>
    /// 分组列表缓存
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType">分组字典关键字类型</typeparam>
    /// <typeparam name="targetType"></typeparam>
    public class memberArrayList<valueType, modelType, keyType, targetType>
        : member<valueType, modelType, keyType, targetType, list<valueType>[]>
        where valueType : class, modelType
        where modelType : class
        where keyType : struct, IEquatable<keyType>
        where targetType : class
    {
        /// <summary>
        /// 数组索引获取器
        /// </summary>
        protected Func<valueType, int> getIndex;
        /// <summary>
        /// 数组容器大小
        /// </summary>
        protected int arraySize;
        /// <summary>
        /// 移除数据并使用最后一个数据移动到当前位置
        /// </summary>
        protected bool isRemoveEnd;
        /// <summary>
        /// 分组列表缓存
        /// </summary>
        /// <param name="cache">整表缓存</param>
        /// <param name="getKey">分组字典关键字获取器</param>
        /// <param name="getIndex">获取数组索引</param>
        /// <param name="arraySize">数组容器大小</param>
        /// <param name="member">缓存字段表达式</param>
        /// <param name="isRemoveEnd">移除数据并使用最后一个数据移动到当前位置</param>
        /// <param name="isReset">是否绑定事件并重置数据</param>
        public memberArrayList(events.cache<valueType, modelType> cache
            , Func<modelType, keyType> getKey, Func<valueType, int> getIndex, int arraySize
            , Func<keyType, targetType> getValue, Expression<Func<targetType, list<valueType>[]>> member
            , Func<IEnumerable<targetType>> getTargets, bool isRemoveEnd, bool isReset = true)
            : base(cache, getKey, getValue, member, getTargets)
        {
            if (getIndex == null) log.Error.Throw(log.exceptionType.Null);
            if (arraySize <= 0) log.Error.Throw(log.exceptionType.IndexOutOfRange);
            this.getIndex = getIndex;
            this.arraySize = arraySize;
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
            foreach (targetType target in getTargets())
            {
                foreach (list<valueType> list in GetMember(target).toSubArray()) list.clear();
            }
            foreach (valueType value in cache.Values) onInserted(value);
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void onInserted(valueType value)
        {
            onInserted(value, getKey(value), getIndex(value));
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value">数据对象</param>
        private void onInserted(valueType value, keyType key, int index)
        {
            targetType target = getValue(key);
            if (target == null) log.Error.Add(typeof(valueType).FullName + " 没有找到缓存目标对象 " + key.ToString(), new System.Diagnostics.StackFrame(), true);
            else
            {
                list<valueType>[] lists = GetMember(target);
                list<valueType> list;
                if (lists == null)
                {
                    setMember(target, lists = new list<valueType>[arraySize]);
                    list = null;
                }
                else list = lists[index];
                if (list == null) lists[index] = list = new list<valueType>();
                list.Add(value);
            }
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="value">更新后的数据</param>
        /// <param name="oldValue">更新前的数据</param>
        protected void onUpdated(valueType cacheValue, valueType value, valueType oldValue, fastCSharp.code.memberMap<modelType> memberMap)
        {
            keyType key = getKey(value), oldKey = getKey(oldValue);
            int index = getIndex(value), oldIndex = getIndex(oldValue);
            if (key.Equals(oldKey))
            {
                if (index != oldIndex)
                {
                    targetType target = getValue(key);
                    if (target == null) log.Error.Add(typeof(valueType).FullName + " 没有找到缓存目标对象 " + key.ToString(), new System.Diagnostics.StackFrame(), true);
                    else
                    {
                        list<valueType>[] lists = GetMember(target);
                        if (lists != null)
                        {
                            list<valueType> list = lists[index];
                            if (list == null) lists[index] = list = new list<valueType>();
                            list.Add(cacheValue);
                            if ((list = lists[oldIndex]) != null)
                            {
                                index = Array.LastIndexOf(list.UnsafeArray, cacheValue, list.Count - 1);
                                if (index != -1)
                                {
                                    if (isRemoveEnd) list.RemoveAtEnd(index);
                                    else list.RemoveAt(index);
                                    return;
                                }
                            }
                        }
                        log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
                    }
                }
            }
            else
            {
                onInserted(cacheValue, key, index);
                onDeleted(cacheValue, oldKey, oldIndex);
            }
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">被删除的数据</param>
        /// <param name="key">被删除的数据关键字</param>
        protected void onDeleted(valueType value, keyType key, int index)
        {
            targetType target = getValue(key);
            if (target == null) log.Error.Add(typeof(valueType).FullName + " 没有找到缓存目标对象 " + key.ToString(), new System.Diagnostics.StackFrame(), true);
            else
            {
                list<valueType>[] lists = GetMember(target);
                if (lists != null)
                {
                    list<valueType> list = lists[index];
                    if (list != null)
                    {
                        index = Array.LastIndexOf(list.UnsafeArray, value, list.Count - 1);
                        if (index != -1)
                        {
                            if (isRemoveEnd) list.RemoveAtEnd(index);
                            else list.RemoveAt(index);
                            return;
                        }
                    }
                }
                log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
            }
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">被删除的数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void onDeleted(valueType value)
        {
            onDeleted(value, getKey(value), getIndex(value));
        }
    }
}