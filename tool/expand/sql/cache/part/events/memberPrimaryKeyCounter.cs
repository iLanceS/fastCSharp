using System;
using System.Linq.Expressions;

namespace fastCSharp.sql.cache.part.events
{
    /// <summary>
    /// 关键字缓存计数器(反射模式)
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="targetType"></typeparam>
    public class memberPrimaryKeyCounter<valueType, modelType, memberCacheType, keyType, targetType> : memberCounter<valueType, modelType, memberCacheType, keyType, targetType>
        where valueType : class, modelType
        where modelType : class
        where memberCacheType: class
        where keyType : struct, IEquatable<keyType>
        where targetType : class
    {
        /// <summary>
        /// 自增id标识缓存计数器
        /// </summary>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="group">数据分组</param>
        public memberPrimaryKeyCounter(whole.events.cache<valueType, modelType, memberCacheType> cache
            , Func<modelType, keyType> getKey, Func<keyType, targetType> getByKey, Func<valueType, targetType> getValue
            , Expression<Func<targetType, keyValue<valueType, int>>> member, int group = 1)
            : base(cache, group, getKey, getByKey, getValue, member)
        {
        }
    }
    /// <summary>
    /// 关键字缓存计数器(反射模式)
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="targetType"></typeparam>
    public sealed class memberPrimaryKeyCounter<valueType, modelType, memberCacheType, keyType> : memberPrimaryKeyCounter<valueType, modelType, memberCacheType, keyType, memberCacheType>
        where valueType : class, modelType
        where modelType : class
        where memberCacheType : class
        where keyType : struct, IEquatable<keyType>
    {
        /// <summary>
        /// 自增id标识缓存计数器
        /// </summary>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="group">数据分组</param>
        public memberPrimaryKeyCounter(whole.events.cache<valueType, modelType, memberCacheType> cache
            , Func<modelType, keyType> getKey, Func<keyType, memberCacheType> getByKey
            , Expression<Func<memberCacheType, keyValue<valueType, int>>> member, int group = 1)
            : base(cache, getKey, getByKey, cache.GetMemberCache, member, group)
        {
        }
    }
    ///// <summary>
    ///// 关键字缓存计数器(反射模式)
    ///// </summary>
    ///// <typeparam name="valueType">表格绑定类型</typeparam>
    ///// <typeparam name="modelType">表格模型类型</typeparam>
    ///// <typeparam name="targetType"></typeparam>
    //public sealed class memberPrimaryKeyCounter<valueType, modelType, memberCacheType, keyType> : memberCounter<valueType, modelType, memberCacheType, keyType, memberCacheType>
    //    where valueType : class, modelType
    //    where modelType : class
    //    where memberCacheType : class
    //    where keyType : struct, IEquatable<keyType>
    //{
    //    /// <summary>
    //    /// 自增id标识缓存计数器
    //    /// </summary>
    //    /// <param name="sqlTool">SQL操作工具</param>
    //    /// <param name="group">数据分组</param>
    //    public memberPrimaryKeyCounter(whole.events.key<valueType, modelType, memberCacheType, keyType> cache
    //        , Expression<Func<memberCacheType, keyValue<valueType, int>>> member, int group = 1)
    //        : base(cache, group, cache.GetKey, cache.GetMemberCacheByKey, cache.GetMemberCache, member)
    //    {
    //    }
    //}
}
