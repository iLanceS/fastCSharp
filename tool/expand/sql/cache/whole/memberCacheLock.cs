using System;

namespace fastCSharp.sql.cache.whole
{
    /// <summary>
    /// 成员缓存+综合访问锁
    /// </summary>
    /// <typeparam name="valueType">缓存数据类型</typeparam>
    public abstract class memberCacheLock<valueType> : memberCache<valueType>
    {
        /// <summary>
        /// 综合访问锁
        /// </summary>
        public readonly object Lock = new object();
    }
}
