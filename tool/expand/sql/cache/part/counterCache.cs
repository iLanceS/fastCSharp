using System;
using fastCSharp.code.cSharp;

namespace fastCSharp.sql.cache.part
{
    /// <summary>
    /// 计数缓存
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    public abstract class counterCache<valueType, modelType, keyType>
        where valueType : class, modelType
        where modelType : class
        where keyType : IEquatable<keyType>
    {
        /// <summary>
        /// 缓存计数器
        /// </summary>
        protected events.counter<valueType, modelType, keyType> counter;
        /// <summary>
        /// 计数缓存
        /// </summary>
        /// <param name="counter">缓存计数器</param>
        protected counterCache(events.counter<valueType, modelType, keyType> counter)
        {
            if (counter == null) log.Error.Throw(log.exceptionType.Null);
            this.counter = counter;
        }
    }
}
