using System;
using System.Collections.Generic;
using System.Threading;
using fastCSharp.code.cSharp;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql.cache.part.events
{
    /// <summary>
    /// 缓存计数器
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType"></typeparam>
    /// <typeparam name="targetType"></typeparam>
    public abstract class memberCounter<valueType, modelType, memberCacheType, keyType, targetType> : copy<valueType, modelType>
        where valueType : class, modelType
        where modelType : class
        where memberCacheType : class
        where keyType : struct, IEquatable<keyType>
        where targetType : class
    {
        ///// <summary>
        ///// 整表缓存
        ///// </summary>
        //private whole.events.cache<valueType, modelType, memberCacheType> cache;
        /// <summary>
        /// 分组字典关键字获取器
        /// </summary>
        internal Func<modelType, keyType> GetKey { get; private set; }
        /// <summary>
        /// 获取缓存目标对象
        /// </summary>
        internal Func<keyType, targetType> GetByKey { get; private set; }
        /// <summary>
        /// 获取缓存目标对象
        /// </summary>
        internal Func<valueType, targetType> GetValue { get; private set; }
        /// <summary>
        /// 获取缓存委托
        /// </summary>
        internal Func<targetType, keyValue<valueType, int>> GetMember { get; private set; }
        /// <summary>
        /// 设置缓存委托
        /// </summary>
        private Action<targetType, keyValue<valueType, int>> setMember;
        /// <summary>
        /// 缓存数据数量
        /// </summary>
        public int Count { get; private set; }
        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>缓存值</returns>
        public valueType this[keyType key]
        {
            get
            {
                return Get(key);
            }
        }
        ///// <summary>
        ///// 缓存计数
        ///// </summary>
        ///// <param name="sqlTool">SQL操作工具</param>
        ///// <param name="getKey">缓存关键字获取器</param>
        ///// <param name="group">数据分组</param>
        //protected memberCounterBase(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, int group, Expression<Func<modelType, keyType>> getKey
        //    , Func<keyType, targetType> getValue, Expression<Func<targetType, keyValue<valueType, int>>> member)
        //    : this(sqlTool, group, getKey == null ? null : getKey.Compile(), getValue, member)
        //{
        //    sqlTool.SetSelectMember(getKey);
        //}
        /// <summary>
        /// 缓存计数
        /// </summary>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="getKey">缓存关键字获取器</param>
        /// <param name="group">数据分组</param>
        protected memberCounter(whole.events.cache<valueType, modelType, memberCacheType> cache, int group, Func<modelType, keyType> getKey
            , Func<keyType, targetType> getByKey, Func<valueType, targetType> getValue, Expression<Func<targetType, keyValue<valueType, int>>> member)
            : base(cache.SqlTool, group)
        {
            if (getKey == null || getByKey == null || getValue == null || member == null) log.Error.Throw(log.exceptionType.Null);
            memberExpression<targetType, keyValue<valueType, int>> expression = new memberExpression<targetType, keyValue<valueType, int>>(member);
            if (expression.Field == null) log.Error.Throw(log.exceptionType.ErrorOperation);
            GetKey = getKey;
            GetByKey = getByKey;
            this.GetValue = getValue;
            GetMember = expression.GetMember;
            setMember = expression.SetMember;

            cache.SqlTool.OnUpdatedLock += onUpdated;
            //cache.OnUpdated += onUpdated;
            cache.OnDeleted += onDeleted;
        }
        /// <summary>
        /// 重置缓存
        /// </summary>
        protected override void reset()
        {
        }
        /// <summary>
        /// 更新记录事件
        /// </summary>
        public event Action<valueType, valueType, valueType, fastCSharp.code.memberMap<modelType>> OnUpdated;
        ///// <summary>
        ///// 更新数据
        ///// </summary>
        ///// <param name="value">更新后的数据</param>
        ///// <param name="oldValue">更新前的数据</param>
        ///// <param name="memberMap">更新成员位图</param>
        //private void onUpdated(valueType cacheValue, valueType value, valueType oldValue, fastCSharp.code.memberMap<modelType> memberMap)
        //{
        //    targetType memberCache = getValue(cacheValue);
        //    keyValue<valueType, int> cache = GetMember(memberCache);
        //    if (cache.Key != null) update(cache.Key, value, memberMap);
        //    if (OnUpdated != null) OnUpdated(cache.Key, value, oldValue);
        //}
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="value">更新后的数据</param>
        /// <param name="oldValue">更新前的数据</param>
        /// <param name="memberMap">更新成员位图</param>
        private void onUpdated(valueType value, valueType oldValue, fastCSharp.code.memberMap<modelType> memberMap)
        {
            targetType memberCache = GetByKey(GetKey(value));
            keyValue<valueType, int> cache = GetMember(memberCache);
            if (cache.Key != null) update(cache.Key, value, oldValue, memberMap);
            if (OnUpdated != null) OnUpdated(cache.Key, value, oldValue, memberMap);
        }
        /// <summary>
        /// 删除记录事件
        /// </summary>
        public event Action<valueType> OnDeleted;
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">被删除的数据</param>
        private void onDeleted(valueType value)
        {
            targetType cache = GetValue(value);
            keyValue<valueType, int> cacheValue = GetMember(cache);
            if (cacheValue.Key != null)
            {
                if (OnDeleted != null) OnDeleted(value);
                setMember(cache, new keyValue<valueType, int>(null, cacheValue.Value));
                --Count;
            }
        }
        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>缓存数据</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal valueType Get(keyType key)
        {
            targetType value = GetByKey(key);
            return value == null ? null : GetMember(value).Key;
        }
        ///// <summary>
        ///// 获取缓存数据
        ///// </summary>
        ///// <param name="value">查询数据</param>
        ///// <returns>缓存数据</returns>
        //[MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        //internal valueType Get(valueType value)
        //{
        //    return Get(GetKey(value));
        //}
        /// <summary>
        /// 添加缓存数据
        /// </summary>
        /// <param name="value">缓存数据</param>
        /// <returns>缓存数据</returns>
        internal valueType Add(valueType value)
        {
            targetType cache = GetByKey(GetKey(value));
            keyValue<valueType, int> valueCount = GetMember(cache);
            if (valueCount.Key != null)
            {
                ++valueCount.Value;
                setMember(cache, valueCount);
                return valueCount.Key;
            }
            valueType copyValue = fastCSharp.emit.constructor<valueType>.New();
            fastCSharp.emit.memberCopyer<modelType>.Copy(copyValue, value, memberMap);
            setMember(cache, new keyValue<valueType, int>(copyValue, 0));
            ++Count;
            return copyValue;
        }
        /// <summary>
        /// 添加缓存数据
        /// </summary>
        /// <param name="value">缓存数据</param>
        /// <returns>缓存数据</returns>
        internal targetType AddGetTarget(valueType value)
        {
            targetType cache = GetByKey(GetKey(value));
            keyValue<valueType, int> valueCount = GetMember(cache);
            if (valueCount.Key != null)
            {
                ++valueCount.Value;
                setMember(cache, valueCount);
            }
            else
            {
                valueType copyValue = fastCSharp.emit.constructor<valueType>.New();
                fastCSharp.emit.memberCopyer<modelType>.Copy(copyValue, value, memberMap);
                setMember(cache, new keyValue<valueType, int>(copyValue, 0));
                ++Count;
            }
            return cache;
        }
        /// <summary>
        /// 删除缓存数据
        /// </summary>
        /// <param name="value">缓存数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void Remove(valueType value)
        {
            Remove(GetByKey(GetKey(value)));
        }
        /// <summary>
        /// 删除缓存数据
        /// </summary>
        /// <param name="value">缓存数据</param>
        internal void Remove(targetType cache)
        {
            keyValue<valueType, int> valueCount = GetMember(cache);
            if (valueCount.Key != null)
            {
                if (valueCount.Value == 0)
                {
                    setMember(cache, new keyValue<valueType, int>());
                    --Count;
                }
                else
                {
                    --valueCount.Value;
                    setMember(cache, valueCount);
                }
            }
            else log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
        }
    }
}