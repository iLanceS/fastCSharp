using System;
using System.Linq.Expressions;
using fastCSharp.code.cSharp;

namespace fastCSharp.sql.cache.part
{
    /// <summary>
    /// 先进先出优先队列缓存
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="counterKeyType">缓存统计关键字类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    /// <typeparam name="cacheValueType">缓存数据类型</typeparam>
    public abstract class queueExpression<valueType, modelType, counterKeyType, keyType, cacheValueType>
        : queue<valueType, modelType, counterKeyType, keyType, cacheValueType>
        where valueType : class, modelType
        where modelType : class
        where keyType : IEquatable<keyType>
        where counterKeyType : IEquatable<counterKeyType>
        where cacheValueType : class
    {
        /// <summary>
        /// 条件表达式获取器
        /// </summary>
        protected Func<keyType, Expression<Func<modelType, bool>>> getWhere;
        /// <summary>
        /// 先进先出优先队列缓存
        /// </summary>
        /// <param name="counter">缓存计数器</param>
        /// <param name="getKey">缓存关键字获取器</param>
        /// <param name="maxCount">缓存默认最大容器大小</param>
        /// <param name="getWhere">条件表达式获取器</param>
        protected queueExpression(events.counter<valueType, modelType, counterKeyType> counter, Expression<Func<modelType, keyType>> getKey, int maxCount
            , Func<keyType, Expression<Func<modelType, bool>>> getWhere)
            : base(counter, getKey, maxCount)
        {
            if (getWhere == null) log.Error.Throw(log.exceptionType.Null);
            this.getWhere = getWhere;
        }
    }
}
