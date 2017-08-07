using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using fastCSharp.reflection;

namespace fastCSharp.sql.cache.whole.events
{
    /// <summary>
    /// 计数成员
    /// </summary>
    public class counterMember
    {
        /// <summary>
        /// 计数成员
        /// </summary>
        protected counterMember() { }
        /// <summary>
        /// 增加计数
        /// </summary>
        /// <param name="id">数据标识</param>
        public virtual void Inc(int id) { }
        /// <summary>
        /// 获取计数
        /// </summary>
        /// <param name="id">数据标识</param>
        /// <returns></returns>
        public virtual int Get(int id) { return -1; }
        /// <summary>
        /// 计数总量
        /// </summary>
        public virtual long TotalCount { get { return -1; } }
        /// <summary>
        /// 默认空计数成员
        /// </summary>
        public static readonly counterMember Null = new counterMember();
    }
    /// <summary>
    /// 计数成员
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="memberCacheType">扩展缓存绑定类型</typeparam>
    public sealed class counterMember<valueType, modelType, memberCacheType> : counterMember, IDisposable
        where valueType : class, modelType
        where modelType : class
        where memberCacheType : class
    {
        /// <summary>
        /// 待计数缓存
        /// </summary>
        private identityCache<valueType, modelType, memberCacheType> cache;
        /// <summary>
        /// 获取未处理计数
        /// </summary>
        private Func<memberCacheType, int> getCount;
        /// <summary>
        /// 设置未处理计数
        /// </summary>
        private Action<memberCacheType, int> setCount;
        /// <summary>
        /// 获取缓存计数
        /// </summary>
        private Func<modelType, int> getCacheCount;
        /// <summary>
        /// 设置缓存计数
        /// </summary>
        private Action<modelType, int> setCacheCount;
        /// <summary>
        /// 超时更新队列
        /// </summary>
        private fastCSharp.threading.timeoutQueue timeoutQueue;
        /// <summary>
        /// 设置数据标识
        /// </summary>
        private Action<valueType, long> setIdentity;
        /// <summary>
        /// 更新数据对象
        /// </summary>
        private valueType updateValue;
        /// <summary>
        /// 计数成员
        /// </summary>
        private fastCSharp.code.memberMap<modelType> memberMap;
        /// <summary>
        /// 未处理计数数据标识列表
        /// </summary>
        private int[] ids = new int[256];
        /// <summary>
        /// 未处理计数数据标识索引位置
        /// </summary>
        private int idIndex;
        /// <summary>
        /// 计数总量
        /// </summary>
        private long totalCount;
        /// <summary>
        /// 计数总量
        /// </summary>
        public override long TotalCount
        {
            get { return totalCount; }
        }
        /// <summary>
        /// 计数访问锁
        /// </summary>
        private readonly object counterLock;
        /// <summary>
        /// 浏览计数
        /// </summary>
        /// <param name="cache">待计数缓存</param>
        /// <param name="member">计数成员</param>
        /// <param name="timeout">超时秒数</param>
        public counterMember(fastCSharp.sql.cache.whole.events.identityCache<valueType, modelType, memberCacheType> cache, Expression<Func<modelType, int>> member, int timeout)
        {
            memberExpression<modelType, int> memberExpression = new memberExpression<modelType, int>(member);
            if (memberExpression.Field == null) log.Error.Throw(log.exceptionType.ErrorOperation);
            FieldInfo filed = typeof(memberCacheType).GetField(memberExpression.Field.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (filed == null) log.Error.Throw(typeof(memberCacheType).fullName() + " 没有找到计数字段 " + memberExpression.Field.Name, new System.Diagnostics.StackFrame(), false);
            this.cache = cache;
            getCacheCount = memberExpression.GetMember;
            setCacheCount = memberExpression.SetMember;
            getCount = fastCSharp.emit.pub.UnsafeGetField<memberCacheType, int>(filed);
            setCount = fastCSharp.emit.pub.UnsafeSetField<memberCacheType, int>(filed);
            setIdentity = fastCSharp.emit.sqlModel<modelType>.SetIdentity;
            updateValue = fastCSharp.emit.constructor<valueType>.New();
            memberMap = cache.SqlTool.CreateMemberMap().Append(member);
            foreach (valueType value in cache.Values) totalCount += getCacheCount(value);
            counterLock = new object();
            (timeoutQueue = fastCSharp.threading.timeoutQueue.Get(timeout)).Add(update);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (counterLock != null) update();
        }
        /// <summary>
        /// 获取计数
        /// </summary>
        /// <param name="id">数据标识</param>
        /// <returns></returns>
        public override int Get(int id)
        {
            valueType value = cache[id];
            return value == null ? 0 : getCacheCount(value) + getCount(cache.GetMemberCache(value));
        }
        /// <summary>
        /// 增加计数
        /// </summary>
        /// <param name="id">数据标识</param>
        public override void Inc(int id)
        {
            memberCacheType memberCache = cache.GetMemberCacheByKey(id);
            if (memberCache != null)
            {
                Interlocked.Increment(ref totalCount);
                Monitor.Enter(counterLock);
                int count = getCount(memberCache);
                if (count == 0)
                {
                    if (idIndex == ids.Length)
                    {
                        try
                        {
                            int[] newIds = new int[idIndex << 1];
                            ids.CopyTo(newIds, 0);
                            ids[idIndex++] = id;
                            setCount(memberCache, 1);
                        }
                        finally { Monitor.Exit(counterLock); }
                    }
                    else
                    {
                        ids[idIndex++] = id;
                        setCount(memberCache, 1);
                        Monitor.Exit(counterLock);
                    }
                }
                else
                {
                    setCount(memberCache, count + 1);
                    Monitor.Exit(counterLock);
                }
            }
        }
        /// <summary>
        /// 浏览量处理
        /// </summary>
        private void update()
        {
            do
            {
                try
                {
                    while (idIndex != 0)
                    {
                        Monitor.Enter(counterLock);
                        int id = ids[--idIndex];
                        valueType value = cache[id];
                        while (value == null)
                        {
                            if (idIndex == 0)
                            {
                                Monitor.Exit(counterLock);
                                break;
                            }
                            value = cache[id = ids[--idIndex]];
                        }
                        memberCacheType memberCache = cache.GetMemberCache(value);
                        int count = getCount(memberCache);
                        setCount(memberCache, 0);
                        Monitor.Exit(counterLock);
                        setIdentity(updateValue, id);
                        setCacheCount(updateValue, getCacheCount(value) + count);
                        cache.Update(updateValue, memberMap);
                    }
                }
                catch (Exception error)
                {
                    fastCSharp.log.Default.Add(error, typeof(valueType).fullName(), false);
                }
            }
            while (idIndex != 0);
            timeoutQueue.Add(update);
        }
    }
}
