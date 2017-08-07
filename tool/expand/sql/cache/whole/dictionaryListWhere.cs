using System;
using fastCSharp.code.cSharp;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql.cache.whole
{
    /// <summary>
    /// 分组列表缓存(反射模式)
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType">分组字典关键字类型</typeparam>
    public sealed class dictionaryListWhere<valueType, modelType, memberCacheType, keyType> : dictionaryList<valueType, modelType, memberCacheType, keyType>
        where valueType : class, modelType
        where modelType : class
        where memberCacheType : class
        where keyType : IEquatable<keyType>
    {
        /// <summary>
        /// 数据匹配器
        /// </summary>
        private Func<valueType, bool> isValue;
        /// <summary>
        /// 分组列表缓存
        /// </summary>
        /// <param name="cache">整表缓存</param>
        /// <param name="getKey">分组字典关键字获取器</param>
        /// <param name="isValue">数据匹配器</param>
        /// <param name="isRemoveEnd">移除数据并使用最后一个数据移动到当前位置</param>
        public dictionaryListWhere
            (events.cache<valueType, modelType, memberCacheType> cache, Func<valueType, keyType> getKey, Func<valueType, bool> isValue, bool isRemoveEnd)
            : base(cache, getKey, isRemoveEnd, false)
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
            insert insert = new insert { Groups = dictionary<randomKey<keyType>>.Create<keyValue<keyType, list<valueType>>>(), GetKey = getKey };
            foreach (valueType value in cache.Values)
            {
                if (isValue(value)) insert.OnInserted(value);
            }
            this.groups = insert.Groups;
            ++keyVersion;
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
            else if (isValue(oldValue)) base.onDeleted(cacheValue, getKey(oldValue));
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
