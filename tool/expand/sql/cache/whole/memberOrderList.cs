using System;
using System.Collections.Generic;
using System.Threading;
using fastCSharp.code.cSharp;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql.cache.whole
{
    /// <summary>
    /// 分组列表排序缓存
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType">分组字典关键字类型</typeparam>
    /// <typeparam name="targetType">目标数据类型</typeparam>
    public class memberOrderList<valueType, modelType, keyType, targetType> : member<valueType, modelType, keyType, targetType, list<valueType>>
        where valueType : class, modelType
        where modelType : class
        where keyType : IEquatable<keyType>
        where targetType : class
    {
        /// <summary>
        /// 排序器
        /// </summary>
        private Func<subArray<valueType>, subArray<valueType>> sorter;
        /// <summary>
        /// 分组列表 延时排序缓存
        /// </summary>
        /// <param name="cache">整表缓存</param>
        /// <param name="getKey">分组字典关键字获取器</param>
        /// <param name="getValue">获取目标对象委托</param>
        /// <param name="member">缓存字段表达式</param>
        /// <param name="sorter">排序器</param>
        /// <param name="isReset">是否初始化</param>
        public memberOrderList(events.cache<valueType, modelType> cache, Func<modelType, keyType> getKey
            , Func<keyType, targetType> getValue, Expression<Func<targetType, list<valueType>>> member
            , Func<IEnumerable<targetType>> getTargets, Func<subArray<valueType>, subArray<valueType>> sorter, bool isReset)
            : base(cache, getKey, getValue, member, getTargets)
        {
            if (sorter == null) log.Error.Throw(log.exceptionType.Null);
            this.sorter = sorter;

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
            resetAll();
            HashSet<keyType> keys = hashSet<keyType>.Create();
            foreach (valueType value in cache.Values)
            {
                keyType key = getKey(value);
                targetType target = getValue(key);
                if (target == null) log.Error.Add(typeof(valueType).FullName + " 没有找到缓存目标对象 " + key.ToString(), new System.Diagnostics.StackFrame(), true);
                else
                {
                    list<valueType> list = GetMember(target);
                    if (list == null)
                    {
                        (list = new list<valueType>()).Add(value);
                        setMember(target, list);
                    }
                    else
                    {
                        list.Add(value);
                        keys.Add(key);
                    }
                }
            }
            foreach (keyType key in keys)
            {
                list<valueType> list = GetMember(getValue(key));
                list.UnsafeArray = sorter(list.ToSubArray()).UnsafeArray.notNull();
            }
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
        /// <param name="key"></param>
        protected void onInserted(valueType value, keyType key)
        {
            targetType target = getValue(key);
            if (target == null) log.Error.Add(typeof(valueType).FullName + " 没有找到缓存目标对象 " + key.ToString(), new System.Diagnostics.StackFrame(), true);
            else
            {
                list<valueType> list = GetMember(target);
                if (list == null)
                {
                    (list = new list<valueType>()).Add(value);
                    setMember(target, list);
                }
                else
                {
                    subArray<valueType> array = list.ToSubArray();
                    array.Add(value);
                    setMember(target, sorter(array).ToList());
                }
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
            if (key.Equals(oldKey))
            {
                targetType target = getValue(key);
                if (target == null) log.Error.Add(typeof(valueType).FullName + " 没有找到缓存目标对象 " + key.ToString(), new System.Diagnostics.StackFrame(), true);
                else
                {
                    list<valueType> list = GetMember(target);
                    if (list == null) log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
                    else setMember(target, sorter(list.ToSubArray()).ToList());
                }
            }
            else
            {
                onInserted(cacheValue, key);
                onDeleted(cacheValue, oldKey);
            }
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">被删除的数据</param>
        /// <param name="key">被删除数据的关键字</param>
        protected void onDeleted(valueType value, keyType key)
        {
            targetType target = getValue(key);
            if (target == null) log.Error.Add(typeof(valueType).FullName + " 没有找到缓存目标对象 " + key.ToString(), new System.Diagnostics.StackFrame(), true);
            else
            {
                list<valueType> list = GetMember(target);
                if (list == null) log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
                else
                {
                    valueType[] array = list.UnsafeArray;
                    int index = System.Array.IndexOf(array, value, 0, list.Count);
                    if (index == -1) log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
                    else if (list.Count == 1) setMember(target, null);
                    else
                    {
                        valueType[] newArray = new valueType[list.Count - 1];
                        Array.Copy(array, 0, newArray, 0, index);
                        Array.Copy(array, index + 1, newArray, index, newArray.Length - index);
                        setMember(target, newArray.toList());
                    }
                }
            }
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
        /// 获取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public list<valueType> GetCache(keyType key)
        {
            targetType target = getValue(key);
            return target != null ? GetMember(target) : null;
        }
    }
}