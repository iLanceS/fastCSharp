using System;
using System.Collections.Generic;
using System.Threading;
using fastCSharp.code.cSharp;
using System.Linq.Expressions;

namespace fastCSharp.sql.cache.whole.events
{
    /// <summary>
    /// 关键字整表缓存(反射模式)
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    public sealed class primaryKey<valueType, modelType, memberCacheType, keyType> : key<valueType, modelType, memberCacheType, keyType>
        where valueType : class, modelType
        where modelType : class
        where memberCacheType : class
        where keyType : IEquatable<keyType>
    {
        /// <summary>
        /// 字典缓存数据
        /// </summary>
        private version<Dictionary<randomKey<keyType>, valueType>> dictionary;
        /// <summary>
        /// 数据集合,请使用GetArray
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
        /// <param name="key">关键字</param>
        /// <returns>数据</returns>
        public override valueType this[keyType key]
        {
            get
            {
                do
                {
                    valueType value;
                    uint version = dictionary.Version;
                    if (dictionary.Value.TryGetValue(key, out value))
                    {
                        if (GetKey(value).Equals(key)) return value;
                    }
                    else if (dictionary.IsVersion(version) == 0) return null;
                    dictionary.Wait();
                }
                while (true);
            }
        }
        /// <summary>
        /// 缓存数据数量
        /// </summary>
        public int Count
        {
            get { return dictionary.Value.Count; }
        }
        /// <summary>
        /// 关键字整表缓存
        /// </summary>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="memberCache">成员缓存</param>
        /// <param name="group">数据分组</param>
        public primaryKey(fastCSharp.emit.sqlTable<valueType, modelType, keyType> sqlTool, Expression<Func<valueType, memberCacheType>> memberCache, int group = 0)
            : base(sqlTool, memberCache, sqlTool.GetPrimaryKey, group)
        {
            sqlTool.OnInsertedLock += onInserted;
            sqlTool.OnUpdatedLock += onUpdated;
            sqlTool.OnDeletedLock += onDeleted;

            resetLock();
        }
        /// <summary>
        /// 重新加载数据
        /// </summary>
        protected override void reset()
        {
            Dictionary<randomKey<keyType>, valueType> dictionary = dictionary<randomKey<keyType>>.Create<valueType>();
            foreach (valueType value in SqlTool.Where(null, memberMap))
            {
                setMemberCacheAndValue(value);
                dictionary[GetKey(value)] = value;
            }
            this.dictionary.SetVersion(dictionary);
        }
        /// <summary>
        /// 增加数据
        /// </summary>
        /// <param name="value">新增的数据</param>
        private void onInserted(valueType value)
        {
            valueType newValue = fastCSharp.emit.constructor<valueType>.New();
            fastCSharp.emit.memberCopyer<modelType>.Copy(newValue, value, memberMap);
            setMemberCacheAndValue(newValue);
            keyType key = GetKey(value);
            ++dictionary.Version;
            try
            {
                dictionary.Value.Add(key, newValue);
            }
            finally { ++dictionary.Version; }
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
            valueType cacheValue;
            if (dictionary.Value.TryGetValue(GetKey(value), out cacheValue))
            {
                update(cacheValue, value, oldValue, memberMap);
                callOnUpdated(cacheValue, value, oldValue, memberMap);
            }
            else log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">被删除的数据</param>
        private void onDeleted(valueType value)
        {
            valueType cacheValue;
            keyType key = GetKey(value);
            if (dictionary.Value.TryGetValue(key, out cacheValue))
            {
                ++dictionary.Version;
                dictionary.Value.Remove(key);
                ++dictionary.Version;
                callOnDeleted(cacheValue);
            }
            else log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
        }
        /// <summary>
        /// 获取数据数组集合
        /// </summary>
        /// <returns>数据数组集合</returns>
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
