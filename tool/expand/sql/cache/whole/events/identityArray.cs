using System;
using System.Collections.Generic;
using fastCSharp.code.cSharp;
using System.Linq.Expressions;

namespace fastCSharp.sql.cache.whole.events
{
    /// <summary>
    /// 自增ID整表数组缓存
    /// </summary>
    /// <typeparam name="valueType">表格类型</typeparam>
    /// <typeparam name="modelType">模型类型</typeparam>
    public class identityArray<valueType, modelType, memberCacheType> : identityCache<valueType, modelType, memberCacheType>
        where valueType : class, modelType
        where modelType : class
        where memberCacheType : class
    {
        /// <summary>
        /// SQL操作缓存
        /// </summary>
        /// <param name="sqlTool">SQL操作工具</param
        /// <param name="memberCache">成员缓存</param>
        /// <param name="group">数据分组</param>
        /// <param name="baseIdentity">基础ID</param>
        /// <param name="isReset">是否初始化事件与数据</param>
        public identityArray(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, Expression<Func<valueType, memberCacheType>> memberCache, int group = 0, int baseIdentity = 0, bool isReset = true)
            : base(sqlTool, memberCache, group, baseIdentity, isReset)
        {
            if (isReset)
            {
                sqlTool.OnInsertedLock += onInserted;
                sqlTool.OnDeletedLock += onDeleted;

                resetLock();
            }
        }
        /// <summary>
        /// 重新加载数据
        /// </summary>
        /// <param name="values">数据集合</param>
        protected void reset(valueType[] values)
        {
            int maxIdentity = values.maxKey(value => GetKey(value), 0);
            if (memberGroup == 0) SqlTool.Identity64 = maxIdentity + baseIdentity;
            identityArray<valueType> newValues = new identityArray<valueType>(maxIdentity + 1);
            foreach (valueType value in values)
            {
                setMemberCacheAndValue(value);
                newValues[GetKey(value)] = value;
            }
            this.values = newValues;
            Count = values.Length;
        }
        /// <summary>
        /// 重新加载数据
        /// </summary>
        protected override void reset()
        {
            reset(SqlTool.Where(null, memberMap).getArray());
        }
        /// <summary>
        /// 增加数据
        /// </summary>
        /// <param name="value">新增的数据</param>
        protected void onInserted(valueType value)
        {
            int identity = GetKey(value);
            if (identity >= values.Length) values.ToSize(identity + 1);
            valueType newValue = fastCSharp.emit.constructor<valueType>.New();
            fastCSharp.emit.memberCopyer<modelType>.Copy(newValue, value, memberMap);
            setMemberCacheAndValue(newValue);
            values[identity] = newValue;
            ++Count;
            callOnInserted(newValue);
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">被删除的数据</param>
        protected void onDeleted(valueType value)
        {
            int identity = GetKey(value);
            valueType cacheValue = values.GetRemove(identity);
            --Count;
            callOnDeleted(cacheValue);
        }
    }
}
