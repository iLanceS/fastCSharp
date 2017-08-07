using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace fastCSharp.sql.cache.whole
{
    /// <summary>
    /// 成员绑定缓存
    /// </summary>
    /// <typeparam name="valueType"></typeparam>
    /// <typeparam name="modelType"></typeparam>
    /// <typeparam name="keyType"></typeparam>
    /// <typeparam name="targetType"></typeparam>
    /// <typeparam name="cacheType"></typeparam>
    public abstract class member<valueType, modelType, keyType, targetType, cacheType>
        where valueType : class, modelType
        where modelType : class
        where keyType : IEquatable<keyType>
        where targetType : class
        where cacheType : class
    {
        /// <summary>
        /// 整表缓存
        /// </summary>
        protected events.cache<valueType, modelType> cache;
        /// <summary>
        /// 分组字典关键字获取器
        /// </summary>
        protected Func<modelType, keyType> getKey;
        /// <summary>
        /// 获取缓存目标对象
        /// </summary>
        protected Func<keyType, targetType> getValue;
        /// <summary>
        /// 获取缓存目标对象集合
        /// </summary>
        protected Func<IEnumerable<targetType>> getTargets;
        /// <summary>
        /// 获取缓存委托
        /// </summary>
        public Func<targetType, cacheType> GetMember { get; private set; }
        /// <summary>
        /// 设置缓存委托
        /// </summary>
        protected Action<targetType, cacheType> setMember;
        /// <summary>
        /// 分组列表缓存
        /// </summary>
        /// <param name="cache">整表缓存</param>
        /// <param name="getKey">分组字典关键字获取器</param>
        /// <param name="getValue">获取目标对象委托</param>
        /// <param name="member">缓存字段表达式</param>
        /// <param name="getTargets">获取缓存目标对象集合</param>
        public member(events.cache<valueType, modelType> cache, Func<modelType, keyType> getKey, Func<keyType, targetType> getValue
            , Expression<Func<targetType, cacheType>> member, Func<IEnumerable<targetType>> getTargets)
        {
            if (cache == null) log.Error.Throw("cache is null", null, false);
            if (getKey == null || getValue == null || getTargets == null || member == null) log.Error.Throw(log.exceptionType.Null);
            memberExpression<targetType, cacheType> expression = new memberExpression<targetType, cacheType>(member);
            if (expression.Field == null) log.Error.Throw(log.exceptionType.ErrorOperation);
            this.cache = cache;
            this.getKey = getKey;
            this.getValue = getValue;
            this.getTargets = getTargets;
            GetMember = expression.GetMember;
            setMember = expression.SetMember;
        }
        /// <summary>
        /// 重新加载数据
        /// </summary>
        protected void resetAll()
        {
            cacheType value = default(cacheType);
            foreach (targetType target in getTargets()) setMember(target, value);
        }
    }
}
