using System;
using System.Linq.Expressions;

namespace fastCSharp.memoryDatabase.cache.index
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
#if NOJIT
        protected ILoadCache cache;
#else
        protected ILoadCache<valueType, modelType> cache;
#endif
        /// <summary>
        /// 分组字典关键字获取器
        /// </summary>
        protected Func<modelType, keyType> getKey;
        /// <summary>
        /// 获取缓存目标对象
        /// </summary>
        protected Func<keyType, targetType> getValue;
        /// <summary>
        /// 获取缓存委托
        /// </summary>
        protected Func<targetType, cacheType> getMember;
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
#if NOJIT
        public member(ILoadCache cache, Func<modelType, keyType> getKey, Func<keyType, targetType> getValue, Expression<Func<targetType, cacheType>> member)
#else
        public member(ILoadCache<valueType, modelType> cache, Func<modelType, keyType> getKey, Func<keyType, targetType> getValue, Expression<Func<targetType, cacheType>> member)
#endif
        {
            if (cache == null || getKey == null || getValue == null || member == null) log.Error.Throw(log.exceptionType.Null);
            sql.memberExpression<targetType, cacheType> expression = new sql.memberExpression<targetType, cacheType>(member);
            if (expression.Field == null) log.Error.Throw(log.exceptionType.ErrorOperation);
            this.cache = cache;
            this.getKey = getKey;
            this.getValue = getValue;
            getMember = expression.GetMember;
            setMember = expression.SetMember;
        }
    }
}