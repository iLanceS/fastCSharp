using System;

namespace fastCSharp.sql.cache.whole
{

    /// <summary>
    /// 成员计数缓存+综合访问锁
    /// </summary>
    /// <typeparam name="valueType">缓存数据类型</typeparam>
    /// <typeparam name="memberCacheType">成员缓存类型</typeparam>
    public abstract class memberCacheCounterLock<valueType, memberCacheType> : memberCacheCounter<valueType, memberCacheType>
        where memberCacheType : memberCacheCounter<valueType, memberCacheType>
    {
        /// <summary>
        /// 综合访问锁
        /// </summary>
        public readonly object Lock = new object();
    }
}
