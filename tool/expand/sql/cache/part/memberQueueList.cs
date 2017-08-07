using System;
using System.Linq.Expressions;
using System.Threading;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql.cache.part
{
    /// <summary>
    /// 先进先出优先队列缓存
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    /// <typeparam name="memberCacheType"></typeparam>
    public abstract class memberQueueList<valueType, modelType, memberCacheType, couterKeyType, keyType, cacheValueType>
        : memberQueue<memberCacheType, cacheValueType>
        where valueType : class, modelType
        where modelType : class
        where couterKeyType : struct, IEquatable<couterKeyType>
        where keyType : struct, IEquatable<keyType>
        where memberCacheType : class
        where cacheValueType : class
    {
        /// <summary>
        /// 缓存计数器
        /// </summary>
        protected events.counter<valueType, modelType, couterKeyType> counter;
        /// <summary>
        /// 缓存关键字获取器
        /// </summary>
        protected Func<modelType, keyType> getKey;
        /// <summary>
        /// 先进先出优先队列缓存
        /// </summary>
        /// <param name="counter">缓存计数器</param>
        /// <param name="getKey">缓存关键字获取器</param>
        /// <param name="maxCount">缓存默认最大容器大小</param>
        protected memberQueueList(events.counter<valueType, modelType, couterKeyType> counter, Expression<Func<modelType, keyType>> getKey
            , Expression<Func<memberCacheType, cacheValueType>> valueMember, Expression<Func<memberCacheType, memberCacheType>> previousMember
            , Expression<Func<memberCacheType, memberCacheType>> nextMember, int maxCount)
            : this(counter, getKey.Compile(), valueMember, previousMember, nextMember, maxCount)
        {
            counter.SqlTool.SetSelectMember(getKey);
        }
        /// <summary>
        /// 先进先出优先队列缓存
        /// </summary>
        /// <param name="counter">缓存计数器</param>
        /// <param name="getKey">缓存关键字获取器</param>
        /// <param name="maxCount">缓存默认最大容器大小</param>
        protected memberQueueList(events.counter<valueType, modelType, couterKeyType> counter, Func<modelType, keyType> getKey
            , Expression<Func<memberCacheType, cacheValueType>> valueMember, Expression<Func<memberCacheType, memberCacheType>> previousMember
            , Expression<Func<memberCacheType, memberCacheType>> nextMember, int maxCount)
            : base(valueMember, previousMember, nextMember, maxCount)
        {
            if (counter == null || getKey == null) log.Error.Throw(log.exceptionType.Null);
            this.counter = counter;
            this.getKey = getKey;
        }
    }
    /// <summary>
    /// 先进先出优先队列缓存
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    public sealed class memberQueueList<valueType, modelType, memberCacheType, couterKeyType, keyType>
        : memberQueueList<valueType, modelType, memberCacheType, couterKeyType, keyType, list<valueType>>
        where valueType : class, modelType
        where modelType : class
        where couterKeyType : struct, IEquatable<couterKeyType>
        where keyType : struct, IEquatable<keyType>
        where memberCacheType : class
    {
        /// <summary>
        /// 获取节点
        /// </summary>
        private Func<keyType, memberCacheType> getTarget;
        /// <summary>
        /// 条件表达式获取器
        /// </summary>
        private Func<keyType, Expression<Func<modelType, bool>>> getWhere;
        /// <summary>
        /// 移除数据并使用最后一个数据移动到当前位置
        /// </summary>
        private bool isRemoveEnd;
        /// <summary>
        /// 缓存数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>缓存数据</returns>
        public subArray<valueType> this[keyType key]
        {
            get
            {
                memberCacheType node = getTarget(key);
                if (node != null)
                {
                    list<valueType> list;
                    Monitor.Enter(counter.SqlTool.Lock);
                    try
                    {
                        if ((list = get(node)) == null)
                        {
                            list = counter.SqlTool.Where(getWhere(key), counter.MemberMap)
                                .getSubArray(value => counter.Add(value))
                                .ToList() ?? new list<valueType>();
                            appendNode(node, list);
                        }
                    }
                    finally { Monitor.Exit(counter.SqlTool.Lock); }
                    return list.toSubArray();
                }
                return default(subArray<valueType>);
            }
        }
        /// <summary>
        /// 先进先出优先队列缓存
        /// </summary>
        /// <param name="counter">缓存计数器</param>
        /// <param name="getWhere"></param>
        /// <param name="maxCount">缓存默认最大容器大小</param>
        public memberQueueList(events.counter<valueType, modelType, couterKeyType> counter
            , Expression<Func<modelType, keyType>> getKey, Func<keyType, memberCacheType> getTarget
            , Func<keyType, Expression<Func<modelType, bool>>> getWhere, Expression<Func<memberCacheType, list<valueType>>> valueMember
            , Expression<Func<memberCacheType, memberCacheType>> previousMember, Expression<Func<memberCacheType, memberCacheType>> nextMember
            , bool isRemoveEnd = false, int maxCount = 0)
            : base(counter, getKey, valueMember, previousMember, nextMember, maxCount)
        {
            if (getMemberValue == null || getTarget == null) log.Error.Throw(log.exceptionType.Null);
            this.getTarget = getTarget;
            this.getWhere = getWhere;
            this.isRemoveEnd = isRemoveEnd;

            counter.OnReset += reset;
            counter.SqlTool.OnInsertedLock += onInserted;
            counter.OnUpdated += onUpdated;
            counter.OnDeleted += onDeleted;
        }
        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="node"></param>
        protected override void removeCounter(memberCacheType node)
        {
            foreach (valueType value in getMemberValue(node).toSubArray()) counter.Remove(value);
            //--count;
        }
        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>缓存数据,失败返回null</returns>
        public subArray<valueType> TryGet(keyType key)
        {
            memberCacheType node = getTarget(key);
            if (node != null)
            {
                list<valueType> value;
                Monitor.Enter(counter.SqlTool.Lock);
                try
                {
                    value = get(node);
                }
                finally { Monitor.Exit(counter.SqlTool.Lock); }
                return value.toSubArray();
            }
            return default(subArray<valueType>);
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
                list<valueType> list = get(node);
                if (list != null) list.Add(counter.Add(value));
            }
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="cacheValue">缓存数据</param>
        /// <param name="value">更新后的数据</param>
        /// <param name="oldValue">更新前的数据</param>
        private void onUpdated(valueType cacheValue, valueType value, valueType oldValue, fastCSharp.code.memberMap<modelType> memberMap)
        {
            keyType key = getKey(value);
            if (cacheValue == null)
            {
                memberCacheType node = getTarget(key);
                if (node != null)
                {
                    list<valueType> values = getMemberValue(node);
                    if (values != null)
                    {
                        removeNode(node);
                        foreach (valueType removeValue in values) counter.Remove(removeValue);
                    }
                }
            }
            else
            {
                keyType oldKey = getKey(oldValue);
                if (!key.Equals(oldKey))
                {
                    memberCacheType target = getTarget(key), oldTarget = getTarget(oldKey);
                    list<valueType> values = target == null ? null : getMemberValue(getTarget(key)), oldValues = oldTarget == null ? null : getMemberValue(getTarget(oldKey));
                    if (values != null)
                    {
                        if (oldValues != null)
                        {
                            values.Add(cacheValue);
                            if (remove(oldValues, cacheValue) == 0) counter.Remove(cacheValue);
                        }
                        else values.Add(counter.Add(cacheValue));
                    }
                    else if (oldValues != null && remove(oldValues, cacheValue) == 0) counter.Remove(cacheValue);
                }
            }
        }
        /// <summary>
        /// 删除缓存数据
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <returns>0表示存在数据</returns>
        private int remove(list<valueType> list, valueType value)
        {
            int index = list.IndexOf(value);
            if (index == -1)
            {
                log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
                return 1;
            }
            if (isRemoveEnd) list.RemoveAtEnd(index);
            else list.RemoveAt(index);
            return 0;
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="node">被删除的数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void onDeleted(valueType value)
        {
            list<valueType> list = getMemberValue(getTarget(getKey(value)));
            if (list != null) remove(list, value);
        }
    }
    /// <summary>
    /// 先进先出优先队列缓存(不适应于update/delete)
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    public sealed class memberQueueList<valueType, modelType, memberCacheType, keyType>
        : memberQueue<memberCacheType, list<valueType>>
        where valueType : class, modelType
        where modelType : class
        where keyType : struct, IEquatable<keyType>
        where memberCacheType : class
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
        /// 条件表达式获取器
        /// </summary>
        private Func<keyType, Expression<Func<modelType, bool>>> getWhere;
        ///// <summary>
        ///// 移除数据并使用最后一个数据移动到当前位置
        ///// </summary>
        //private bool isRemoveEnd;
        /// <summary>
        /// 缓存数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>缓存数据</returns>
        public subArray<valueType> this[keyType key]
        {
            get
            {
                memberCacheType node = getTarget(key);
                if (node != null)
                {
                    list<valueType> list;
                    Monitor.Enter(sqlTool.Lock);
                    try
                    {
                        if ((list = get(node)) == null)
                        {
                            list = sqlTool.Where(getWhere(key)).getSubArray().ToList() ?? new list<valueType>();
                            appendNode(node, list);
                        }
                    }
                    finally { Monitor.Exit(sqlTool.Lock); }
                    return list.toSubArray();
                }
                return default(subArray<valueType>);
            }
        }
        /// <summary>
        /// 先进先出优先队列缓存
        /// </summary>
        /// <param name="counter">缓存计数器</param>
        /// <param name="getWhere"></param>
        /// <param name="maxCount">缓存默认最大容器大小</param>
        public memberQueueList(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, Func<modelType, keyType> getKey, Func<keyType, memberCacheType> getTarget
            , Func<keyType, Expression<Func<modelType, bool>>> getWhere, Expression<Func<memberCacheType, list<valueType>>> valueMember
            , Expression<Func<memberCacheType, memberCacheType>> previousMember, Expression<Func<memberCacheType, memberCacheType>> nextMember
            , int maxCount = 0)//, bool isRemoveEnd = false
            : base(valueMember, previousMember, nextMember, maxCount)
        {
            if (sqlTool == null || getKey == null || getTarget == null) log.Error.Throw(log.exceptionType.Null);
            this.sqlTool = sqlTool;
            this.getKey = getKey;
            this.getTarget = getTarget;
            this.getWhere = getWhere;
            //this.isRemoveEnd = isRemoveEnd;

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
        public subArray<valueType> TryGet(keyType key)
        {
            memberCacheType node = getTarget(key);
            if (node != null)
            {
                list<valueType> value;
                Monitor.Enter(sqlTool.Lock);
                try
                {
                    value = get(node);
                }
                finally { Monitor.Exit(sqlTool.Lock); }
                return value.toSubArray();
            }
            return default(subArray<valueType>);
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
                list<valueType> list = get(node);
                if (list != null) list.Add(value);
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
            keyType key = getKey(value), oldKey = getKey(oldValue);
            memberCacheType node = getTarget(key);
            if (node != null) removeNode(node);
            if (!key.Equals(oldKey))
            {
                node = getTarget(key);
                if (node != null) removeNode(node);
            }
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="node">被删除的数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void onDeleted(valueType value)
        {
            memberCacheType node = getTarget(getKey(value));
            if (node != null) removeNode(node);
        }
    }
}
