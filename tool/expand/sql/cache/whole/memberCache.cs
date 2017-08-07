using System;

namespace fastCSharp.sql.cache.whole
{
    /// <summary>
    /// 成员缓存
    /// </summary>
    /// <typeparam name="valueType">缓存数据类型</typeparam>
    public abstract class memberCache<valueType>
    {
        /// <summary>
        /// 缓存数据
        /// </summary>
        public valueType Value;
    }
}
