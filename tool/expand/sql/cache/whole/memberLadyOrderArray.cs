using System;
using System.Collections.Generic;
using System.Threading;
using fastCSharp.code.cSharp;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql.cache.whole
{
    /// <summary>
    /// 分组列表 延时排序缓存
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType">分组字典关键字类型</typeparam>
    /// <typeparam name="targetType">目标数据类型</typeparam>
    public class memberLadyOrderArray<valueType, modelType, keyType, targetType> : member<valueType, modelType, keyType, targetType, ladyOrderArray<valueType>>
        where valueType : class, modelType
        where modelType : class
        where keyType : IEquatable<keyType>
        where targetType : class
    {
        /// <summary>
        /// 排序器
        /// </summary>
        public Func<subArray<valueType>, subArray<valueType>> Sorter { get; private set; }
        /// <summary>
        /// 分组列表 延时排序缓存
        /// </summary>
        /// <param name="cache">整表缓存</param>
        /// <param name="getKey">分组字典关键字获取器</param>
        /// <param name="getValue">获取目标对象委托</param>
        /// <param name="member">缓存字段表达式</param>
        /// <param name="sorter">排序器</param>
        /// <param name="isReset">是否初始化</param>
        public memberLadyOrderArray(events.cache<valueType, modelType> cache, Func<modelType, keyType> getKey
            , Func<keyType, targetType> getValue, Expression<Func<targetType, ladyOrderArray<valueType>>> member
            , Func<IEnumerable<targetType>> getTargets, Func<subArray<valueType>, subArray<valueType>> sorter, bool isReset)
            : base(cache, getKey, getValue, member, getTargets)
        {
            if (sorter == null) log.Error.Throw(log.exceptionType.Null);
            Sorter = sorter;

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
            foreach (valueType value in cache.Values) onInserted(value);
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
                ladyOrderArray<valueType> array = GetMember(target);
                if (array == null) setMember(target, array = new ladyOrderArray<valueType>());
                array.Insert(value);
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
                    ladyOrderArray<valueType> array = GetMember(target);
                    if (array == null) log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
                    else array.Update(cacheValue);
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
                ladyOrderArray<valueType> array = GetMember(target);
                if (array == null) log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
                else array.Delete(value);
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
        private ladyOrderArray<valueType> getCache(keyType key)
        {
            targetType target = getValue(key);
            return target != null ? GetMember(target) : null;
        }
        /// <summary>
        /// 获取匹配数量
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>匹配数量</returns>
        public int Count(keyType key)
        {
            ladyOrderArray<valueType> array = getCache(key);
            return array == null ? 0 : array.Count;
        }
        /// <summary>
        /// 获取匹配数量
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="isValue">数据匹配器,禁止锁操作</param>
        /// <returns>匹配数量</returns>
        public int Count(keyType key, Func<valueType, bool> isValue)
        {
            ladyOrderArray<valueType> array = getCache(key);
            return array == null ? 0 : array.CurrentArray.GetCount(isValue);
        }
        /// <summary>
        /// 查找第一个匹配的数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>第一个匹配的数据,失败返回null</returns>
        public valueType FirstOrDefault(keyType key, Func<valueType, bool> isValue)
        {
            ladyOrderArray<valueType> array = getCache(key);
            return array == null ? null : array.CurrentArray.FirstOrDefault(isValue);
        }
        /// <summary>
        /// 获取匹配的数据集合
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>数据集合</returns>
        public valueType[] GetFindArray(keyType key, Func<valueType, bool> isValue)
        {
            ladyOrderArray<valueType> array = getCache(key);
            return array == null ? nullValue<valueType>.Array : array.CurrentArray.GetFindArray(isValue);
        }
        /// <summary>
        /// 获取不排序的数据集合
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>数据集合</returns>
        public subArray<valueType> GetCache(keyType key)
        {
            ladyOrderArray<valueType> array = getCache(key);
            return array == null ? default(subArray<valueType>) : array.CurrentArray;
        }
        /// <summary>
        /// 获取有序数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="index">关键字</param>
        /// <returns>获取有序数据</returns>
        public valueType At(keyType key, int index)
        {
            ladyOrderArray<valueType> array = getCache(key);
            return array == null ? null : array.At(cache.SqlTool.Lock, Sorter, index);
        }
        /// <summary>
        /// 获取排序数据范围集合
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>排序数据范围集合</returns>
        public valueType[] GetArray(keyType key)
        {
            ladyOrderArray<valueType> array = getCache(key);
            return array == null ? nullValue<valueType>.Array : array.GetArray(cache.SqlTool.Lock, Sorter);
        }
        /// <summary>
        /// 获取分页数据集合
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="currentPage">分页页号</param>
        /// <param name="count">数据总数</param>
        /// <returns>分页数据集合</returns>
        public valueType[] GetPage(keyType key, int pageSize, int currentPage, out int count)
        {
            ladyOrderArray<valueType> array = getCache(key);
            if (array != null) return array.GetPage(cache.SqlTool.Lock, Sorter, pageSize, currentPage, out count);
            count = 0;
            return nullValue<valueType>.Array;
        }
        /// <summary>
        /// 获取分页数据集合
        /// </summary>
        /// <typeparam name="arrayType"></typeparam>
        /// <param name="key">关键字</param>
        /// <param name="getValue">数组转换</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="currentPage">分页页号</param>
        /// <param name="count">数据总数</param>
        /// <returns>分页数据集合</returns>
        public arrayType[] GetPage<arrayType>(keyType key, Func<valueType, arrayType> getValue, int pageSize, int currentPage, out int count)
        {
            ladyOrderArray<valueType> array = getCache(key);
            if (array != null) return array.GetPage(cache.SqlTool.Lock, Sorter, getValue, pageSize, currentPage, out count);
            count = 0;
            return nullValue<arrayType>.Array;
        }
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
            ladyOrderArray<valueType> array = getCache(key);
            if (array != null) return array.GetPageDesc(cache.SqlTool.Lock, Sorter, pageSize, currentPage, out count);
            count = 0;
            return nullValue<valueType>.Array;
        }
        /// <summary>
        /// 获取逆序分页数据集合
        /// </summary>
        /// <typeparam name="arrayType"></typeparam>
        /// <param name="key">关键字</param>
        /// <param name="getValue">数组转换</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="currentPage">分页页号</param>
        /// <param name="count">数据总数</param>
        /// <returns>逆序分页数据集合</returns>
        public arrayType[] GetPageDesc<arrayType>(keyType key, Func<valueType, arrayType> getValue, int pageSize, int currentPage, out int count)
        {
            ladyOrderArray<valueType> array = getCache(key);
            if (array != null) return array.GetPageDesc(cache.SqlTool.Lock, Sorter, getValue, pageSize, currentPage, out count);
            count = 0;
            return nullValue<arrayType>.Array;
        }
    }
}