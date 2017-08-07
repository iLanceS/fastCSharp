using System;
using fastCSharp.code.cSharp;
using System.Collections.Generic;
using System.Threading;
using System.Linq.Expressions;

namespace fastCSharp.sql.cache.whole.events
{
    /// <summary>
    /// 关键字整表缓存
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    public abstract class key<valueType, modelType, memberCacheType, keyType> : cache<valueType, modelType, memberCacheType>
        where valueType : class, modelType
        where modelType : class
        where keyType : IEquatable<keyType>
        where memberCacheType : class
    {
        /// <summary>
        /// 键值获取器
        /// </summary>
        internal Func<modelType, keyType> GetKey { get; private set; }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>数据</returns>
        public abstract valueType this[keyType key] { get; }
        /// <summary>
        /// 获取成员缓存
        /// </summary>
        public readonly Func<keyType, memberCacheType> GetMemberCacheByKey;
        /// <summary>
        /// 关键字整表缓存
        /// </summary>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="getKey">键值获取器</param>
        /// <param name="group">数据分组</param>
        public key(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, Expression<Func<valueType, memberCacheType>> memberCache, Func<modelType, keyType> getKey, int group = 0)
            : base(sqlTool, memberCache, group)
        {
            if (getKey == null) log.Error.Throw(log.exceptionType.Null);
            GetKey = getKey;
            GetMemberCacheByKey = getMemberCacheByKey;
        }
        /// <summary>
        /// 获取成员缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal memberCacheType getMemberCacheByKey(keyType key)
        {
            valueType value = this[key];
            return value != null ? base.GetMemberCache(value) : null;
        }
        /// <summary>
        /// 创建分组字典缓存
        /// </summary>
        /// <typeparam name="keyType">分组字典关键字类型</typeparam>
        /// <typeparam name="targetType">目标表格类型</typeparam>
        /// <typeparam name="targetModelType">目标模型类型</typeparam>
        /// <typeparam name="targetMemberCacheType">目标缓存绑定类型</typeparam>
        /// <typeparam name="valueKeyType">目标数据关键字类型</typeparam>
        /// <param name="targetCache">目标缓存</param>
        /// <param name="getKey">分组字典关键字获取器</param>
        /// <param name="member">缓存字段表达式</param>
        /// <param name="getValueKey">获取数据关键字委托</param>
        /// <param name="isReset">是否绑定事件并重置数据</param>
        /// <param name="isSave">是否保存缓存对象防止被垃圾回收</param>
        /// <returns></returns>
        public memberDictionary<valueType, modelType, keyType, keyType, targetMemberCacheType> CreateMemberDictionary<targetType, targetModelType, targetMemberCacheType>
            (key<targetType, targetModelType, targetMemberCacheType, keyType> targetCache, Func<modelType, keyType> getKey
            , Expression<Func<targetMemberCacheType, Dictionary<randomKey<keyType>, valueType>>> member
            , bool isReset = true, bool isSave = true)
            where targetType : class, targetModelType
            where targetModelType : class
            where targetMemberCacheType : class
        {
            memberDictionary<valueType, modelType, keyType, keyType, targetMemberCacheType> cache = new memberDictionary<valueType, modelType, keyType, keyType, targetMemberCacheType>(this, getKey, targetCache.GetMemberCacheByKey, member, targetCache.GetAllMemberCache, GetKey, isReset);
            if (isSave) memberCaches.Add(cache);
            return cache;
        }
    }
}
