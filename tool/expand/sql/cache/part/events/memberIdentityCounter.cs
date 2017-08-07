using System;
using System.Linq.Expressions;

namespace fastCSharp.sql.cache.part.events
{
    /// <summary>
    /// 自增id标识缓存计数器(反射模式)
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="targetType"></typeparam>
    public sealed class memberIdentityCounter<valueType, modelType, memberCacheType, targetType> : memberCounter<valueType, modelType, memberCacheType, long, targetType>
        where valueType : class, modelType
        where modelType : class
        where memberCacheType : class
        where targetType : class
    {
        /// <summary>
        /// 自增id标识缓存计数器
        /// </summary>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="group">数据分组</param>
        public memberIdentityCounter(whole.events.cache<valueType, modelType, memberCacheType> cache
            , Func<long, targetType> getByKey, Func<valueType, targetType> getValue, Expression<Func<targetType, keyValue<valueType, int>>> member, int group = 1)
            : base(cache, group, fastCSharp.emit.sqlModel<modelType>.GetIdentity, getByKey, getValue, member)
        {
        }
    }
    /// <summary>
    /// 自增id标识缓存计数器(反射模式)
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="targetType"></typeparam>
    public class memberIdentityCounter32<valueType, modelType, memberCacheType, targetType> : memberCounter<valueType, modelType, memberCacheType, int, targetType>
        where valueType : class, modelType
        where modelType : class
        where memberCacheType : class
        where targetType : class
    {
        /// <summary>
        /// 自增id标识缓存计数器
        /// </summary>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="group">数据分组</param>
        public memberIdentityCounter32(whole.events.cache<valueType, modelType, memberCacheType> cache
            , Func<int, targetType> getByKey, Func<valueType, targetType> getValue, Expression<Func<targetType, keyValue<valueType, int>>> member, int group = 1)
            : base(cache, group, fastCSharp.emit.sqlModel<modelType>.GetIdentity32, getByKey, getValue, member)
        {
        }
    }
    /// <summary>
    /// 自增id标识缓存计数器(反射模式)
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="memberCacheType"></typeparam>
    public sealed class memberIdentityCounter32<valueType, modelType, memberCacheType> : memberIdentityCounter32<valueType, modelType, memberCacheType, memberCacheType>
        where valueType : class, modelType
        where modelType : class
        where memberCacheType : class
    {
        /// <summary>
        /// 自增id标识缓存计数器
        /// </summary>
        /// <param name="cache">关键字整表缓存</param>
        /// <param name="member">缓存字段表达式</param>
        /// <param name="group">数据分组</param>
        public memberIdentityCounter32(whole.events.key<valueType, modelType, memberCacheType, int> cache
            , Expression<Func<memberCacheType, keyValue<valueType, int>>> member, int group = 1)
            : base(cache, cache.GetMemberCacheByKey, cache.GetMemberCache, member, group)
        {
        }
        /// <summary>
        /// 创建先进先出优先队列缓存
        /// </summary>
        /// <param name="valueMember">节点成员</param>
        /// <param name="previousMember">前一个节点成员</param>
        /// <param name="nextMember">后一个节点成员</param>
        /// <param name="maxCount">缓存默认最大容器大小</param>
        /// <returns></returns>
        public fastCSharp.sql.cache.part.memberQueue<valueType, modelType, memberCacheType, int> CreateMemberQueue(Expression<Func<memberCacheType, valueType>> valueMember
            , Expression<Func<memberCacheType, memberCacheType>> previousMember, Expression<Func<memberCacheType, memberCacheType>> nextMember, int maxCount = 0)
        {
            return new fastCSharp.sql.cache.part.memberQueue<valueType, modelType, memberCacheType, int>(this, SqlTool.GetByIdentity, valueMember, previousMember, nextMember, maxCount);
        }
    }
}
