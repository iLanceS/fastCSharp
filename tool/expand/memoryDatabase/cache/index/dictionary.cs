using System;
using System.Collections.Generic;
using fastCSharp.code.cSharp;
using System.Threading;

namespace fastCSharp.memoryDatabase.cache.index
{
    /// <summary>
    /// 关键字二级缓存
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    /// <typeparam name="memberType">数据成员类型</typeparam>
    /// <typeparam name="keyType1">关键字类型1</typeparam>
    /// <typeparam name="keyType2">关键字类型2</typeparam>
    public class dictionary<valueType, memberType, keyType1, keyType2>
        where valueType : class, fastCSharp.data.IPrimaryKey<fastCSharp.data.primaryKey<keyType1, keyType2>>
        where memberType : IMemberMap<memberType>
        where keyType1 : IEquatable<keyType1>, IComparable<keyType1>
        where keyType2 : IEquatable<keyType2>, IComparable<keyType2>
    {
        /// <summary>
        /// 数据集合
        /// </summary>
        protected Dictionary<keyType1, Dictionary<keyType2, valueType>> values = dictionary<keyType1>.Create<Dictionary<keyType2, valueType>>();
        /// <summary>
        /// 关键字二级缓存
        /// </summary>
        /// <param name="cache">关键字缓存</param>
        public dictionary(fastCSharp.memoryDatabase.cache.dictionaryBase<valueType, memberType, fastCSharp.data.primaryKey<keyType1, keyType2>> cache)
        {
            object cacheLock = cache.CacheLock;
            Monitor.Enter(cacheLock);
            try
            {
                cache.OnInserted += onInserted;
                cache.OnDeleted += onDeleted;
                foreach (valueType value in cache.Array) onInserted(value);
            }
            finally { Monitor.Exit(cacheLock); }
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value">数据对象</param>
        private void onInserted(valueType value)
        {
            Dictionary<keyType2, valueType> keyValues;
            fastCSharp.data.primaryKey<keyType1, keyType2> key = value.PrimaryKey;
            if (!values.TryGetValue(key.Key1, out keyValues)) values.Add(key.Key1, keyValues = dictionary<keyType2>.Create<valueType>());
            keyValues[key.Key2] = value;
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">数据对象</param>
        private void onDeleted(valueType value)
        {
            Dictionary<keyType2, valueType> keyValues;
            fastCSharp.data.primaryKey<keyType1, keyType2> key = value.PrimaryKey;
            if (values.TryGetValue(key.Key1, out keyValues)) keyValues.Remove(key.Key2);
        }
        /// <summary>
        /// 根据关键字获取数据集合
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>数据集合</returns>
        public ICollection<valueType> Get(keyType1 key)
        {
            Dictionary<keyType2, valueType> keyValues;
            return values.TryGetValue(key, out keyValues) ? keyValues.Values : null;
        }
    }
    /// <summary>
    /// 关键字二级缓存
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    /// <typeparam name="keyType1">关键字类型1</typeparam>
    /// <typeparam name="keyType2">关键字类型2</typeparam>
    public sealed class dictionary<valueType, keyType1, keyType2> : dictionary<valueType, memberMap<valueType>, keyType1, keyType2>
        where valueType : class, fastCSharp.data.IPrimaryKey<fastCSharp.data.primaryKey<keyType1, keyType2>>
        where keyType1 : IEquatable<keyType1>, IComparable<keyType1>
        where keyType2 : IEquatable<keyType2>, IComparable<keyType2>
    {
        /// <summary>
        /// 关键字二级缓存
        /// </summary>
        /// <param name="cache">关键字缓存</param>
        public dictionary(fastCSharp.memoryDatabase.cache.dictionaryBase<valueType, memberMap<valueType>, fastCSharp.data.primaryKey<keyType1, keyType2>> cache) : base(cache) { }
    }
}
