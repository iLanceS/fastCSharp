using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Linq.Expressions;

namespace fastCSharp.sql.cache.part
{
    /// <summary>
    /// 先进先出优先队列自定义缓存
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    public sealed class memberQueueCustom<valueType, modelType, memberCacheType, keyType, cacheValueType>
        : memberQueue<memberCacheType, cacheValueType>
        where valueType : class, modelType
        where modelType : class
        where keyType : struct, IEquatable<keyType>
        where memberCacheType : class
#if NOJIT
        where cacheValueType : class, ICustom
#else
        where cacheValueType : class, ICustom<valueType>
#endif
    {
        /// <summary>
        /// SQL操作工具
        /// </summary>
        private fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool;
        /// <summary>
        /// 缓存关键字获取器
        /// </summary>
        private Func<modelType, keyType> getKey;
        /// <summary>
        /// 获取节点
        /// </summary>
        private Func<keyType, memberCacheType> getTarget;
        /// <summary>
        /// 获取缓存数据
        /// </summary>
        private Func<keyType, cacheValueType> getCache;
        /// <summary>
        /// 缓存数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>缓存数据</returns>
        public cacheValueType this[keyType key]
        {
            get
            {
                memberCacheType node = getTarget(key);
                if (node != null)
                {
                    cacheValueType cache;
                    Monitor.Enter(sqlTool.Lock);
                    try
                    {
                        if ((cache = get(node)) == null)
                        {
                            cache = getCache(key);
                            appendNode(node, cache);
                        }
                    }
                    finally { Monitor.Exit(sqlTool.Lock); }
                    return cache;
                }
                return null;
            }
        }
        /// <summary>
        /// 先进先出优先队列缓存
        /// </summary>
        /// <param name="counter">缓存计数器</param>
        /// <param name="getCache"></param>
        /// <param name="maxCount">缓存默认最大容器大小</param>
        public memberQueueCustom(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, Func<modelType, keyType> getKey, Func<keyType, memberCacheType> getTarget
            , Func<keyType, cacheValueType> getCache, Expression<Func<memberCacheType, cacheValueType>> valueMember
            , Expression<Func<memberCacheType, memberCacheType>> previousMember, Expression<Func<memberCacheType, memberCacheType>> nextMember
            , bool isRemoveEnd = false, int maxCount = 0)
            : base(valueMember, previousMember, nextMember, maxCount)
        {
            if (sqlTool == null || getKey == null || getTarget == null) log.Error.Throw(log.exceptionType.Null);
            this.sqlTool = sqlTool;
            this.getKey = getKey;
            this.getTarget = getTarget;
            this.getCache = getCache;

            sqlTool.OnInsertedLock += onInserted;
            sqlTool.OnUpdatedLock += onUpdated;
            sqlTool.OnDeletedLock += onDeleted;
        }
        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="node"></param>
        protected override void removeCounter(memberCacheType node)
        {
            //--count;
        }
        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>缓存数据,失败返回null</returns>
        public cacheValueType TryGet(keyType key)
        {
            memberCacheType node = getTarget(key);
            if (node != null)
            {
                cacheValueType value;
                Monitor.Enter(sqlTool.Lock);
                try
                {
                    value = get(node);
                }
                finally { Monitor.Exit(sqlTool.Lock); }
                return value;
            }
            return null;
        }
        /// <summary>
        /// 增加数据
        /// </summary>
        /// <param name="value">新增的数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void onInserted(valueType value)
        {
            memberCacheType node = getTarget(getKey(value));
            if (node != null)
            {
                cacheValueType cache = get(node);
                if (cache != null) cache.Add(value);
            }
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="value">更新后的数据</param>
        /// <param name="oldValue">更新前的数据</param>
        /// <param name="memberMap">更新成员位图</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void onUpdated(valueType value, valueType oldValue, fastCSharp.code.memberMap<modelType> memberMap)
        {
            onInserted(value);
            onDeleted(oldValue);
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="node">被删除的数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void onDeleted(valueType value)
        {
            memberCacheType node = getTarget(getKey(value));
            if (node != null)
            {
                cacheValueType cache = get(node);
                if (cache != null) cache.Remove(value);
            }
        }
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key"></param>
        public void Remove(keyType key)
        {
            memberCacheType node = getTarget(key);
            if (node != null) removeNode(node);
        }
    }
}
