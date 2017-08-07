using System;
using System.Collections.Generic;
using System.Threading;
using fastCSharp.code.cSharp;

namespace fastCSharp.memoryDatabase.cache
{
    /// <summary>
    /// 关键字缓存
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    /// <typeparam name="memberType">数据成员类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    public abstract class dictionaryBase<valueType, memberType, keyType> : loadCache<valueType, memberType, keyType>
        where valueType : class, fastCSharp.data.IPrimaryKey<keyType>
        where memberType : IMemberMap<memberType>
        where keyType : IEquatable<keyType>
    {
        /// <summary>
        /// 数据集合
        /// </summary>
        private Dictionary<keyType, valueLock> values;
        /// <summary>
        /// 加载日志添加对象
        /// </summary>
        /// <param name="value">添加的对象</param>
        /// <param name="logSize">日志字节长度</param>
        protected override void loadInsert(valueType value, int logSize)
        {
            if (values == null)
            {
                values = dictionary<keyType>.Create<valueLock>();
                values.Add(value.PrimaryKey, new valueLock { Value = value, Lock = new object(), LogSize = logSize });
            }
            else
            {
                keyType key = value.PrimaryKey;
                if (!values.ContainsKey(key)) values.Add(key, new valueLock { Value = value, Lock = new object(), LogSize = logSize });
            }
        }
        /// <summary>
        /// 加载日志修改对象
        /// </summary>
        /// <param name="value">修改的对象</param>
        /// <param name="memberMap">修改对象成员位图</param>
        protected override void loadUpdate(valueType value, memberType memberMap)
        {
            if (values != null)
            {
                valueLock cacheValue;
                keyType key = value.PrimaryKey;
                if (values.TryGetValue(key, out cacheValue)) copy(cacheValue.Value, value, memberMap);
            }
        }
        /// <summary>
        /// 加载日志删除对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>日志字节长度</returns>
        protected override int loadDelete(keyType key)
        {
            if (values != null)
            {
                valueLock value;
                if (values.TryGetValue(key, out value))
                {
                    values.Remove(key);
                    return value.LogSize;
                }
            }
            return 0;
        }
        /// <summary>
        /// 日志数据加载完成
        /// </summary>
        protected override void loaded()
        {
            if (values == null) values = dictionary<keyType>.Create<valueLock>();
        }
        /// <summary>
        /// 获取当前数据集合
        /// </summary>
        protected override valueType[] getArray()
        {
            if (values != null)
            {
                Monitor.Enter(cacheLock);
                try
                {
                    if (values != null) return values.Values.getArray(value => value.Value);
                }
                finally { Monitor.Exit(cacheLock); }
            }
            return null;
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>数据对象</returns>
        private valueLock getNoLock(keyType key)
        {
            Dictionary<keyType, valueLock> dictionary = this.values;
            if (dictionary != null)
            {
                valueLock value;
                if (dictionary.TryGetValue(key, out value) && value.Value != null && value.Value.PrimaryKey.Equals(key))
                {
                    return value;
                }
            }
            return default(valueLock);
        }
        /// <summary>
        /// 获取操作队列锁
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>操作队列锁,失败返回null</returns>
        protected override object getLock(keyType key)
        {
            valueLock value = getNoLock(key);
            if (value.Value != null) return value.Lock;
            Monitor.Enter(cacheLock);
            try
            {
                if (values != null && values.TryGetValue(key, out value)) return value.Lock;
            }
            finally { Monitor.Exit(cacheLock); }
            return null;
        }
        /// <summary>
        /// 添加对象
        /// </summary>
        /// <param name="value">添加的对象</param>
        /// <param name="logSize">日志字节长度</param>
        protected override void insert(valueType value, int logSize)
        {
            keyType key = value.PrimaryKey;
            Monitor.Enter(cacheLock);
            try
            {
                if (values != null && !values.ContainsKey(key)) values.Add(key, new valueLock { Value = value, Lock = new object(), LogSize = logSize });
            }
            finally { Monitor.Exit(cacheLock); }
        }
        /// <summary>
        /// 根据关键字获取对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>对象数据,失败返回null</returns>
        private valueType get(keyType key)
        {
            valueLock valueLock = getNoLock(key);
            if (valueLock.Value == null)
            {
                Monitor.Enter(cacheLock);
                try
                {
                    if (values != null) values.TryGetValue(key, out valueLock);
                }
                finally { Monitor.Exit(cacheLock); }
            }
            return valueLock.Value;
        }
        /// <summary>
        /// 修改对象
        /// </summary>
        /// <param name="value">修改的对象</param>
        /// <param name="memberMap">修改对象成员位图</param>
        /// <returns>修改后的对象,失败返回null</returns>
        protected override valueType update(valueType value, memberType memberMap)
        {
            valueType cacheValue = get(value.PrimaryKey);
            if (cacheValue != null) copy(cacheValue, value, memberMap);
            return cacheValue;
        }
        /// <summary>
        /// 修改对象
        /// </summary>
        /// <param name="value">修改的对象</param>
        /// <param name="memberMap">修改对象成员位图</param>
        /// <returns>修改后的对象,修改前的对象</returns>
        protected override keyValue<valueType, valueType> update2(valueType value, memberType memberMap)
        {
            valueType cacheValue = get(value.PrimaryKey), oldValue = null;
            if (cacheValue != null)
            {
                oldValue = fastCSharp.emit.memberCopyer<valueType>.MemberwiseClone(cacheValue);
                copy(cacheValue, value, memberMap);
            }
            return new keyValue<valueType, valueType>(cacheValue, oldValue);
        }
        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>被删除对象+日志字节长度,失败返回null</returns>
        protected override keyValue<valueType, int> delete(keyType key)
        {
            valueLock cacheValue = new valueLock();
            Monitor.Enter(cacheLock);
            try
            {
                if (values != null && values.TryGetValue(key, out cacheValue)) values.Remove(key);
            }
            finally { Monitor.Exit(cacheLock); }
            return new keyValue<valueType, int>(cacheValue.Value, cacheValue.LogSize);
        }
        /// <summary>
        /// 根据关键字获取数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">数据对象</param>
        /// <returns>是否存在对象</returns>
        protected override bool get(keyType key, ref valueType value)
        {
            valueLock valueLock = getNoLock(key);
            if (valueLock.Value == null)
            {
                Monitor.Enter(cacheLock);
                if (values != null) values.TryGetValue(key, out valueLock);
                Monitor.Exit(cacheLock);
            }
            if (valueLock.Value != null)
            {
                value = valueLock.Value;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        protected override void dispose()
        {
            values = null;
        }
    }
    /// <summary>
    /// 关键字缓存
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    /// <typeparam name="memberType">数据成员类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    public class dictionary<valueType, memberType, keyType> : dictionaryBase<valueType, memberType, keyType>
        where valueType : class, fastCSharp.data.IPrimaryKey<keyType>, ICopy<valueType, memberType>
        where memberType : IMemberMap<memberType>
        where keyType : IEquatable<keyType>
    {
        /// <summary>
        /// 复制数据
        /// </summary>
        /// <param name="value">目标数据</param>
        /// <param name="copyValue">被复制数据</param>
        /// <param name="memberMap">复制成员位图</param>
        protected override void copy(valueType value, valueType copyValue, memberType memberMap)
        {
            value.CopyFrom(copyValue, memberMap);
        }
    }
    /// <summary>
    /// 关键字缓存(反射模式)
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    public class dictionary<valueType, keyType> : dictionaryBase<valueType, memberMap<valueType>, keyType>
        where valueType : class, fastCSharp.data.IPrimaryKey<keyType>
        where keyType : IEquatable<keyType>
    {
        /// <summary>
        /// 复制数据
        /// </summary>
        /// <param name="value">目标数据</param>
        /// <param name="copyValue">被复制数据</param>
        /// <param name="memberMap">复制成员位图</param>
        protected override void copy(valueType value, valueType copyValue, memberMap<valueType> memberMap)
        {
            copy<valueType>.Copy(value, copyValue, code.memberFilters.Instance, memberMap);
        }
    }
}
