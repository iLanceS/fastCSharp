using System;
using fastCSharp.code.cSharp;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace fastCSharp.sql.cache.whole.events
{
    /// <summary>
    /// 自增ID整表缓存
    /// </summary>
    /// <typeparam name="valueType">表格类型</typeparam>
    /// <typeparam name="modelType">模型类型</typeparam>
    public abstract class identityCache<valueType, modelType, memberCacheType> : identityMemberMap<valueType, modelType, memberCacheType>
        where valueType : class, modelType
        where modelType : class
        where memberCacheType : class
    {
        /// <summary>
        /// 缓存数据集合
        /// </summary>
        protected identityArray<valueType> values;
        /// <summary>
        /// 数据集合
        /// </summary>
        public override IEnumerable<valueType> Values
        {
            get
            {
                foreach (valueType[] array in values.Arrays)
                {
                    foreach (valueType value in array)
                    {
                        if (value != null) yield return value;
                    }
                }
            }
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="identity">数据自增ID</param>
        /// <returns>数据</returns>
        public override valueType this[int identity]
        {
            get
            {
                return (uint)identity < (uint)values.Length ? values[identity] : null;
            }
        }
        /// <summary>
        /// SQL操作缓存
        /// </summary>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="baseIdentity">基础ID</param>
        /// <param name="group">数据分组</param>
        /// <param name="isEvent">是否绑定更新事件</param>
        protected identityCache(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, Expression<Func<valueType, memberCacheType>> memberCache, int group, int baseIdentity, bool isEvent)
            : base(sqlTool, memberCache, group, baseIdentity)
        {
            if (isEvent)
            {
                sqlTool.OnUpdatedLock += onUpdated;
            }
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="value">更新后的数据</param>
        /// <param name="oldValue">更新前的数据</param>
        /// <param name="memberMap">更新成员位图</param>
        protected void onUpdated(valueType value, valueType oldValue, fastCSharp.code.memberMap<modelType> memberMap)
        {
            valueType cacheValue = values[GetKey(value)];
            update(cacheValue, value, oldValue, memberMap);
            callOnUpdated(cacheValue, value, oldValue, memberMap);
        }
    }
}
