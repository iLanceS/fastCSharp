using System;
using System.Collections.Generic;
using System.Threading;
using fastCSharp.threading;

namespace fastCSharp.sql.cache.whole.client
{
    /// <summary>
    /// 成员缓存
    /// </summary>
    /// <typeparam name="valueType">缓存数据类型</typeparam>
    public abstract class identityMemberCache<valueType>
        where valueType : identityMemberCache<valueType>
    {
        /// <summary>
        /// 自增标识
        /// </summary>
        public int Id;
        ///// <summary>
        ///// 缓存数据
        ///// </summary>
        //public valueType Value;
        /// <summary>
        /// 缓存信息
        /// </summary>
        private static identityArray<valueType> cache;
        /// <summary>
        /// 缓存信息访问锁
        /// </summary>
        private static readonly object cacheLock = new object();
        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static valueType Get(int id)
        {
            valueType value;
            Monitor.Enter(cacheLock);
            if (id >= cache.Length)
            {
                try
                {
                    cache.ToSize(id + 1);
                    (value = fastCSharp.emit.constructor<valueType>.New()).Id = id;
                    return cache[id] = value;
                }
                finally { Monitor.Exit(cacheLock); }
            }
            if ((value = cache[id]) == null)
            {
                try
                {
                    (value = fastCSharp.emit.constructor<valueType>.New()).Id = id;
                    return cache[id] = value;
                }
                finally { Monitor.Exit(cacheLock); }
            }
            Monitor.Exit(cacheLock);
            return value;
        }
        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<valueType> GetAll()
        {
            foreach (valueType[] array in cache.Arrays)
            {
                foreach (valueType value in array)
                {
                    if (value != null) yield return value;
                }
            }
        }
    }
}
