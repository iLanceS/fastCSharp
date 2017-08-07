using System;
using System.Collections.Generic;
using System.Threading;
using fastCSharp.code.cSharp;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;

namespace fastCSharp.sql.cache.whole.events
{
    /// <summary>
    /// 自增ID整表缓存
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    public sealed class identityDictionaryWhere<valueType, modelType, memberCacheType> : identityMemberMap<valueType, modelType, memberCacheType>
        where valueType : class, modelType
        where modelType : class
        where memberCacheType : class
    {
        /// <summary>
        /// 数据匹配器
        /// </summary>
        private Func<valueType, bool> isValue;
        /// <summary>
        /// 数据缓存集合
        /// </summary>
        private version<Dictionary<int, valueType>> dictionary;
        /// <summary>
        /// 数据集合,请使用GetArray()
        /// </summary>
        public override IEnumerable<valueType> Values
        {
            get
            {
                return GetArray();
            }
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="id">关键字</param>
        /// <returns>数据</returns>
        public override valueType this[int id]
        {
            get
            {
                do
                {
                    valueType value;
                    uint version = dictionary.Version;
                    if (dictionary.Value.TryGetValue(id, out value))
                    {
                        if (GetKey(value) == id) return value;
                    }
                    else if (dictionary.IsVersion(version) == 0) return null;
                    dictionary.Wait();
                }
                while (true);
            }
        }
        /// <summary>
        /// 自增ID整表数组缓存
        /// </summary>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="isValue">数据匹配器,必须保证更新数据的匹配一致性</param>
        /// <param name="baseIdentity">基础ID</param>
        /// <param name="group">数据分组</param>
        public identityDictionaryWhere(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool
            , Expression<Func<valueType, memberCacheType>> memberCache, Func<valueType, bool> isValue, int group = 0, int baseIdentity = 0)
            : base(sqlTool, memberCache, group, baseIdentity)
        {
            if (isValue == null) log.Error.Throw(log.exceptionType.Null);
            this.isValue = isValue;

            sqlTool.OnInserted += onInserted;
            sqlTool.OnUpdated += onUpdated;
            sqlTool.OnDeleted += onDeleted;

            resetLock();
        }
        /// <summary>
        /// 重新加载数据
        /// </summary>
        protected override void reset()
        {
            valueType[] values = SqlTool.Where(null, memberMap).getFindArray(isValue);
            Dictionary<int, valueType> newValues = fastCSharp.dictionary.CreateInt<valueType>(values.Length);
            int maxIdentity = 0;
            foreach (valueType value in values)
            {
                setMemberCacheAndValue(value);
                int identity = GetKey(value);
                if (identity > maxIdentity) maxIdentity = identity;
                newValues.Add(identity, value);
            }
            if (memberGroup == 0) SqlTool.Identity64 = maxIdentity + baseIdentity;
            this.dictionary.Set(newValues);
            Count = values.Length;
            ++this.dictionary.Version;
        }
        /// <summary>
        /// 增加数据
        /// </summary>
        /// <param name="value">新增的数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void onInserted(valueType value)
        {
            if (isValue(value)) add(value);
        }
        /// <summary>
        /// 增加数据
        /// </summary>
        /// <param name="value">新增的数据</param>
        private void add(valueType value)
        {
            valueType newValue = fastCSharp.emit.constructor<valueType>.New();
            fastCSharp.emit.memberCopyer<modelType>.Copy(newValue, value, memberMap);
            setMemberCacheAndValue(newValue);
            ++dictionary.Version;
            try
            {
                dictionary.Value.Add(GetKey(value), newValue);
                ++Count;
            }
            finally
            {
                ++dictionary.Version;
            }
            callOnInserted(newValue);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="value">更新后的数据</param>
        /// <param name="oldValue">更新前的数据</param>
        /// <param name="memberMap">更新成员位图</param>
        private void onUpdated(valueType value, valueType oldValue, fastCSharp.code.memberMap<modelType> memberMap)
        {
            if (isValue(value))
            {
                valueType cacheValue;
                if (dictionary.Value.TryGetValue(GetKey(value), out cacheValue))
                {
                    update(cacheValue, value, oldValue, memberMap);
                    callOnUpdated(cacheValue, value, oldValue, memberMap);
                }
                else log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
            }
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">被删除的数据</param>
        private void onDeleted(valueType value)
        {
            if (isValue(value))
            {
                valueType cacheValue;
                int identity = GetKey(value);
                if (dictionary.Value.TryGetValue(identity, out cacheValue))
                {
                    ++dictionary.Version;
                    dictionary.Value.Remove(identity);
                    --Count;
                    ++dictionary.Version;
                    callOnDeleted(cacheValue);
                }
                else log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
            }
        }
        /// <summary>
        /// 获取数据集合
        /// </summary>
        /// <returns>数据集合</returns>
        public valueType[] GetArray()
        {
            Monitor.Enter(SqlTool.Lock);
            try
            {
                return dictionary.Value.Values.getArray();
            }
            finally { Monitor.Exit(SqlTool.Lock); }
        }
    }
}
