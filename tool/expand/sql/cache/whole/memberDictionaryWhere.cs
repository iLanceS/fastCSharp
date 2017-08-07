using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql.cache.whole
{
    /// <summary>
    /// 分组字典缓存
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType">分组字典关键字类型</typeparam>
    /// <typeparam name="valueKeyType">目标数据关键字类型</typeparam>
    /// <typeparam name="targetType">目标数据类型</typeparam>
    public sealed class memberDictionaryWhere<valueType, modelType, keyType, valueKeyType, targetType> : memberDictionary<valueType, modelType, keyType, valueKeyType, targetType>
        where valueType : class, modelType
        where modelType : class
        where keyType : IEquatable<keyType>
        where targetType : class
        where valueKeyType : IEquatable<valueKeyType>
    {
        /// <summary>
        /// 缓存值判定
        /// </summary>
        private Func<valueType, bool> isValue;
        /// <summary>
        /// 分组字典缓存
        /// </summary>
        /// <param name="cache">整表缓存</param>
        /// <param name="getKey">分组字典关键字获取器</param>
        /// <param name="getValue">获取目标对象委托</param>
        /// <param name="member">缓存字段表达式</param>
        /// <param name="getValueKey">获取数据关键字委托</param>
        /// <param name="isReset">是否绑定事件并重置数据</param>
        public memberDictionaryWhere(events.cache<valueType, modelType> cache, Func<modelType, keyType> getKey
            , Func<keyType, targetType> getValue, Expression<Func<targetType, Dictionary<randomKey<valueKeyType>, valueType>>> member
            , Func<IEnumerable<targetType>> getTargets, Func<modelType, valueKeyType> getValueKey, Func<valueType, bool> isValue)
            : base(cache, getKey, getValue, member, getTargets, getValueKey, false)
        {
            if (isValue == null) log.Error.Throw(log.exceptionType.Null);
            this.isValue = isValue;

            cache.OnReset += reset;
            cache.OnInserted += onInserted;
            cache.OnUpdated += onUpdated;
            cache.OnDeleted += onDeleted;
            resetLock();
        }
        /// <summary>
        /// 重新加载数据
        /// </summary>
        protected override void reset()
        {
            resetAll();
            foreach (valueType value in cache.Values) if (isValue(value)) onInserted(value);
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private new void onInserted(valueType value)
        {
            if (isValue(value)) base.onInserted(value);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="value">更新后的数据</param>
        /// <param name="oldValue">更新前的数据</param>
        private new void onUpdated(valueType cacheValue, valueType value, valueType oldValue, fastCSharp.code.memberMap<modelType> memberMap)
        {
            if (isValue(value))
            {
                if (isValue(oldValue)) base.onUpdated(cacheValue, value, oldValue, memberMap);
                else base.onInserted(cacheValue);
            }
            else if (isValue(oldValue)) onDeleted(cacheValue, getKey(oldValue));
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">被删除的数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private new void onDeleted(valueType value)
        {
            if (isValue(value)) base.onDeleted(value);
        }
    }
}
