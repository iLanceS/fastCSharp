using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;

namespace fastCSharp.sql.cache.part
{
    /// <summary>
    /// 先进先出优先队列缓存
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    /// <typeparam name="memberCacheType"></typeparam>
    public abstract class memberQueue<memberCacheType, cacheValueType>
        where memberCacheType : class
        where cacheValueType : class
    {
        /// <summary>
        /// 获取节点数据
        /// </summary>
        protected Func<memberCacheType, cacheValueType> getMemberValue;
        /// <summary>
        /// 设置节点数据
        /// </summary>
        protected Action<memberCacheType, cacheValueType> setMemberValue;
        /// <summary>
        /// 获取前一个节点
        /// </summary>
        protected Func<memberCacheType, memberCacheType> getPrevious;
        /// <summary>
        /// 设置前一个节点
        /// </summary>
        protected Action<memberCacheType, memberCacheType> setPrevious;
        /// <summary>
        /// 获取后一个节点
        /// </summary>
        protected Func<memberCacheType, memberCacheType> getNext;
        /// <summary>
        /// 设置后一个节点
        /// </summary>
        protected Action<memberCacheType, memberCacheType> setNext;
        /// <summary>
        /// 缓存默认最大容器大小
        /// </summary>
        protected int maxCount;
        /// <summary>
        /// 数据数量
        /// </summary>
        protected int count;
        /// <summary>
        /// 头节点
        /// </summary>
        protected memberCacheType header;
        /// <summary>
        /// 尾节点
        /// </summary>
        protected memberCacheType end;
        /// <summary>
        /// 先进先出优先队列缓存(非计数缓存)
        /// </summary>
        /// <param name="valueMember">节点成员</param>
        /// <param name="previousMember">前一个节点成员</param>
        /// <param name="nextMember">后一个节点成员</param>
        /// <param name="maxCount">缓存默认最大容器大小</param>
        protected memberQueue(Expression<Func<memberCacheType, cacheValueType>> valueMember, Expression<Func<memberCacheType, memberCacheType>> previousMember
            , Expression<Func<memberCacheType, memberCacheType>> nextMember, int maxCount)
        {
            if (valueMember == null || previousMember == null || nextMember == null) log.Error.Throw(log.exceptionType.Null);
            memberExpression<memberCacheType, cacheValueType> valueExpression = new memberExpression<memberCacheType, cacheValueType>(valueMember);
            if (valueExpression.Field == null) log.Error.Throw(log.exceptionType.ErrorOperation);
            memberExpression<memberCacheType, memberCacheType> previousExpression = new memberExpression<memberCacheType, memberCacheType>(previousMember);
            if (previousExpression.Field == null) log.Error.Throw(log.exceptionType.ErrorOperation);
            memberExpression<memberCacheType, memberCacheType> nextExpression = new memberExpression<memberCacheType, memberCacheType>(nextMember);
            if (nextExpression.Field == null) log.Error.Throw(log.exceptionType.ErrorOperation);
            getMemberValue = valueExpression.GetMember;
            setMemberValue = valueExpression.SetMember;
            getPrevious = previousExpression.GetMember;
            setPrevious = previousExpression.SetMember;
            getNext = nextExpression.GetMember;
            setNext = nextExpression.SetMember;
            this.maxCount = maxCount <= 0 ? config.sql.Default.CacheMaxCount : maxCount;
        }
        /// <summary>
        /// 重置缓存
        /// </summary>
        protected void reset()
        {
            while (header != null)
            {
                end = getNext(header);
                setMemberValue(header, null);
                setPrevious(header, null);
                setNext(header, null);
                removeCounter(header);
                header = end;
            }
            count = 0;
        }
        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="node"></param>
        protected abstract void removeCounter(memberCacheType node);
        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected cacheValueType get(memberCacheType node)
        {
            cacheValueType value = getMemberValue(node);
            if (value != null && node != end)
            {
                memberCacheType previous = getPrevious(node), next = getNext(node);
                if (previous == null) setPrevious(header = next, null);
                else
                {
                    setNext(previous, next);
                    setPrevious(next, previous);
                }
                setNext(end, node);
                setPrevious(node, end);
                setNext(node, null);
                end = node;
            }
            return value;
        }
        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="node"></param>
        /// <param name="value"></param>
        protected void appendNode(memberCacheType node, cacheValueType value)
        {
            setMemberValue(node, value);
            if (end == null) header = end = node;
            else
            {
                setPrevious(node, end);
                setNext(end, node);
                end = node;
            }
            if (count == maxCount)
            {
                setMemberValue(node = header, null);
                setPrevious(header = getNext(header), null);
                setNext(node, null);
                removeCounter(node);
            }
            else ++count;
        }
        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="node"></param>
        protected void removeNode(memberCacheType node)
        {
            setMemberValue(node, null);
            memberCacheType previous = getPrevious(node), next = getNext(node);
            if (previous == null)
            {
                if (next == null) header = end = null;
                else
                {
                    setNext(node, null);
                    setPrevious(header = next, null);
                }
            }
            else
            {
                setPrevious(node, null);
                if (next == null) setNext(end = previous, null);
                else
                {
                    setNext(node, null);
                    setNext(previous, next);
                    setPrevious(next, previous);
                }
            }
            --count;
        }
    }
    /// <summary>
    /// 先进先出优先队列缓存
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    /// <typeparam name="memberCacheType"></typeparam>
    public abstract class memberQueue<valueType, modelType, memberCacheType, couterKeyType, counterTargetType, keyType, cacheValueType>
        : memberQueue<memberCacheType, cacheValueType>
        where valueType : class, modelType
        where modelType : class
        where couterKeyType : struct, IEquatable<couterKeyType>
        where counterTargetType : class
        where keyType : struct, IEquatable<keyType>
        where memberCacheType : class
        where cacheValueType : class
    {
        /// <summary>
        /// 缓存计数器
        /// </summary>
        protected events.memberCounter<valueType, modelType, memberCacheType, couterKeyType, counterTargetType> counter;
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
        protected memberQueue(events.memberCounter<valueType, modelType, memberCacheType, couterKeyType, counterTargetType> counter, Expression<Func<modelType, keyType>> getKey
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
        /// <param name="valueMember">节点成员</param>
        /// <param name="previousMember">前一个节点成员</param>
        /// <param name="nextMember">后一个节点成员</param>
        /// <param name="maxCount">缓存默认最大容器大小</param>
        protected memberQueue(events.memberCounter<valueType, modelType, memberCacheType, couterKeyType, counterTargetType> counter, Func<modelType, keyType> getKey
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
    public sealed class memberQueue<valueType, modelType, memberCacheType, keyType>
        : memberQueue<valueType, modelType, memberCacheType, keyType, memberCacheType, keyType, valueType>
        where valueType : class, modelType
        where modelType : class
        where keyType : struct, IEquatable<keyType>
        where memberCacheType : class
    {
        /// <summary>
        /// 数据获取器
        /// </summary>
        private Func<keyType, fastCSharp.code.memberMap<modelType>, valueType> getValue;
        /// <summary>
        /// 缓存数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>缓存数据</returns>
        public valueType this[keyType key]
        {
            get
            {
                memberCacheType node = counter.GetByKey(key);
                if (node != null)
                {
                    valueType value;
                    Monitor.Enter(counter.SqlTool.Lock);
                    try
                    {
                        if ((value = get(node)) == null && (value = counter.Get(key)) == null)
                        {
                            if ((value = getValue(key, counter.MemberMap)) != null) onInserted(value);
                        }
                    }
                    finally { Monitor.Exit(counter.SqlTool.Lock); }
                    return value;
                }
                return null;
            }
        }
        /// <summary>
        /// 先进先出优先队列缓存
        /// </summary>
        /// <param name="counter">缓存计数器</param>
        /// <param name="getValue">数据获取器,禁止锁操作</param>
        /// <param name="valueMember">节点成员</param>
        /// <param name="previousMember">前一个节点成员</param>
        /// <param name="nextMember">后一个节点成员</param>
        /// <param name="maxCount">缓存默认最大容器大小</param>
        public memberQueue(events.memberCounter<valueType, modelType, memberCacheType, keyType, memberCacheType> counter
            , Func<keyType, fastCSharp.code.memberMap<modelType>, valueType> getValue, Expression<Func<memberCacheType, valueType>> valueMember
            , Expression<Func<memberCacheType, memberCacheType>> previousMember, Expression<Func<memberCacheType, memberCacheType>> nextMember, int maxCount = 0)
            : base(counter, counter.GetKey, valueMember, previousMember, nextMember, maxCount)
        {
            if (getValue == null) log.Error.Throw(log.exceptionType.Null);
            this.getValue = getValue;

            counter.OnReset += reset;
            counter.SqlTool.OnInsertedLock += onInserted;
            counter.OnDeleted += onDeleted;
        }
        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="node"></param>
        protected override void removeCounter(memberCacheType node)
        {
            counter.Remove(node);
        }
        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>缓存数据,失败返回null</returns>
        public valueType TryGet(keyType key)
        {
            memberCacheType node = counter.GetByKey(key);
            if (node != null)
            {
                valueType value;
                Monitor.Enter(counter.SqlTool.Lock);
                try
                {
                    value = get(node);
                }
                finally { Monitor.Exit(counter.SqlTool.Lock); }
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
            memberCacheType node = counter.AddGetTarget(value);
            appendNode(node, counter.GetMember(node).Key);
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="node">被删除的数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void onDeleted(valueType value)
        {
            memberCacheType node = counter.GetValue(value);
            //memberCacheType node = counter.GetByKey(getKey(value));
            if (getMemberValue(node) != null) removeNode(node);
        }
    }
    /// <summary>
    /// 先进先出优先队列缓存
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    public sealed class memberQueue<valueType, modelType, memberCacheType, couterKeyType, counterTargetType, keyType>
        : memberQueue<valueType, modelType, memberCacheType, couterKeyType, counterTargetType, keyType, valueType>
        where valueType : class, modelType
        where modelType : class
        where couterKeyType : struct, IEquatable<couterKeyType>
        where counterTargetType : class
        where keyType : struct, IEquatable<keyType>
        where memberCacheType : class
    {
        /// <summary>
        /// 获取节点
        /// </summary>
        private Func<keyType, memberCacheType> getTarget;
        /// <summary>
        /// 数据匹配
        /// </summary>
        private Func<valueType, bool> isValue;
        /// <summary>
        /// 条件表达式获取器
        /// </summary>
        private Func<keyType, Expression<Func<modelType, bool>>> getWhere;
        /// <summary>
        /// 默认空值
        /// </summary>
        private valueType nullValue;
        /// <summary>
        /// 缓存数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>缓存数据</returns>
        public valueType this[keyType key]
        {
            get
            {
                memberCacheType node = getTarget(key);
                if (node != null)
                {
                    valueType value;
                    Monitor.Enter(counter.SqlTool.Lock);
                    try
                    {
                        if ((value = get(node)) == null)
                        {
                            foreach (valueType getValue in counter.SqlTool.Where(getWhere(key), counter.MemberMap))
                            {
                                value = getValue;
                                break;
                            }
                            if (value == null) value = nullValue;
                            onInserted(value);
                        }
                    }
                    finally { Monitor.Exit(counter.SqlTool.Lock); }
                    return value == nullValue ? null : value;
                }
                return null;
            }
        }
        /// <summary>
        /// 先进先出优先队列缓存
        /// </summary>
        /// <param name="counter">缓存计数器</param>
        /// <param name="getValue">数据获取器,禁止锁操作</param>
        /// <param name="maxCount">缓存默认最大容器大小</param>
        public memberQueue(events.memberCounter<valueType, modelType, memberCacheType, couterKeyType, counterTargetType> counter
            , Expression<Func<modelType, keyType>> getKey, Func<keyType, memberCacheType> getTarget, Func<valueType, bool> isValue
            , Func<keyType, Expression<Func<modelType, bool>>> getWhere, valueType nullValue, Expression<Func<memberCacheType, valueType>> valueMember
            , Expression<Func<memberCacheType, memberCacheType>> previousMember, Expression<Func<memberCacheType, memberCacheType>> nextMember, int maxCount = 0)
            : base(counter, getKey, valueMember, previousMember, nextMember, maxCount)
        {
            if (getWhere == null || getTarget == null || isValue == null || nullValue == null) log.Error.Throw(log.exceptionType.Null);
            this.getTarget = getTarget;
            this.isValue = isValue;
            this.getWhere = getWhere;
            this.nullValue = nullValue;

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
            counter.Remove(getMemberValue(node));
        }
        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>缓存数据,失败返回null</returns>
        public valueType TryGet(keyType key)
        {
            memberCacheType node = getTarget(key);
            if (node != null)
            {
                valueType value;
                Monitor.Enter(counter.SqlTool.Lock);
                try
                {
                    value = get(node);
                }
                finally { Monitor.Exit(counter.SqlTool.Lock); }
                return value == nullValue ? null : value;
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
            if (isValue(value))
            {
                memberCacheType node = getTarget(getKey(value = counter.Add(value)));
                if (getMemberValue(node) == null) appendNode(node, value);
                else setMemberValue(node, value);
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
            if (cacheValue == null) onDeleted(value);
            else
            {
                keyType key = getKey(value), oldKey = getKey(oldValue);
                if (!key.Equals(oldKey))
                {
                    if (isValue(oldValue))
                    {
                        memberCacheType node = getTarget(oldKey);
                        if (getMemberValue(node) != null) removeNode(node);
                    }
                    if (isValue(value))
                    {
                        memberCacheType node = getTarget(key);
                        if (getMemberValue(node) == null) appendNode(node, cacheValue);
                        else setMemberValue(node, cacheValue);
                    }
                }
            }
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="node">被删除的数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void onDeleted(valueType value)
        {
            if (isValue(value))
            {
                memberCacheType node = getTarget(getKey(value));
                if (getMemberValue(node) != null) removeNode(node);
            }
        }
    }
}
