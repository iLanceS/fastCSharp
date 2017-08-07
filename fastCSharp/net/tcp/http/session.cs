using System;
using System.Collections.Generic;
using fastCSharp.threading;
using fastCSharp.code.cSharp;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace fastCSharp.net.tcp.http
{
    /// <summary>
    /// 会话标识接口
    /// </summary>
    public interface ISession
    {
        /// <summary>
        /// 删除Session
        /// </summary>
        /// <param name="sessionId">Session名称</param>
        void Remove(ref sessionId sessionId);
        /// <summary>
        /// 设置Session值
        /// </summary>
        /// <param name="sessionId">Session名称</param>
        /// <param name="value">值</param>
        /// <returns>Session是否被更新</returns>
        bool Set(ref sessionId sessionId, object value);
        /// <summary>
        /// 获取Session值
        /// </summary>
        /// <param name="sessionId">Session名称</param>
        /// <param name="nullValue">失败返回值</param>
        /// <returns>Session值</returns>
        object Get(ref sessionId sessionId, object nullValue);
    }
#if NOJIT
#else
    /// <summary>
    /// 会话标识接口
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    public interface ISession<valueType> : ISession
    {
        /// <summary>
        /// 设置Session值
        /// </summary>
        /// <param name="sessionId">Session名称</param>
        /// <param name="value">值</param>
        /// <returns>Session是否被更新</returns>
        bool Set(ref sessionId sessionId, valueType value);
        /// <summary>
        /// 获取Session值
        /// </summary>
        /// <param name="sessionId">Session名称</param>
        /// <param name="nullValue">失败返回值</param>
        /// <returns>Session值</returns>
        valueType Get(ref sessionId sessionId, valueType nullValue);
    }
#endif
    /// <summary>
    /// 会话标识
    /// </summary>
    public abstract unsafe class session : timeVerifyServer
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        internal const string ServiceName = "session";
        /// <summary>
        /// 超时时钟周期
        /// </summary>
        protected long timeoutTicks;
        /// <summary>
        /// 超时刷新时钟周期
        /// </summary>
        protected long refreshTicks;
        /// <summary>
        /// 会话标识
        /// </summary>
        protected session() { }
        /// <summary>
        /// 会话标识
        /// </summary>
        /// <param name="timeoutMinutes">超时分钟数</param>
        /// <param name="refreshMinutes">超时刷新分钟数</param>
        protected session(int timeoutMinutes, int refreshMinutes)
        {
            timeoutTicks = new TimeSpan(0, timeoutMinutes, 0).Ticks;
            refreshTicks = new TimeSpan(0, refreshMinutes, 0).Ticks;
            timerTask.Default.Add(this, thread.callType.HttpSessionRefreshTimeout, date.nowTime.Now.AddTicks(refreshTicks));
        }
        /// <summary>
        /// 超时检测
        /// </summary>
        internal virtual void RefreshTimeout() { }
    }
    /// <summary>
    /// 会话标识
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
#if NotFastCSharpCode
    [fastCSharp.code.cSharp.tcpServer(Filter = code.memberFilters.Instance, Service = session.ServiceName, IsIdentityCommand = true)]
#else
    [fastCSharp.code.cSharp.tcpServer(Filter = code.memberFilters.Instance, Service = session.ServiceName, VerifyMethodType = typeof(session<>.tcpClient.timeVerifyMethod), IsIdentityCommand = true)]
#endif
    public partial class session<valueType> : session
#if NOJIT
        , ISession
#else
        , ISession<valueType>
#endif
    {
        /// <summary>
        /// Session值
        /// </summary>
        internal struct value
        {
            /// <summary>
            /// 超时时间
            /// </summary>
            public DateTime Timeout;
            /// <summary>
            /// 随机数低64位
            /// </summary>
            public ulong Low;
            /// <summary>
            /// 随机数高64位
            /// </summary>
            public ulong High;
            /// <summary>
            /// Session值集合
            /// </summary>
            public valueType Value;
            /// <summary>
            /// 索引标识
            /// </summary>
            public uint Identity;
            /// <summary>
            /// 检测会话是否有效
            /// </summary>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            public bool Check(ref sessionId sessionId)
            {
                return Identity == sessionId.IndexIdentity && ((Low ^ sessionId.Low) | (High ^ sessionId.High)) == 0 && Timeout != DateTime.MinValue;
            }
            /// <summary>
            /// 设置会话信息
            /// </summary>
            /// <param name="sessionId"></param>
            /// <param name="timeout"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public bool Set(ref sessionId sessionId, DateTime timeout, valueType value)
            {
                if (((Low ^ sessionId.Low) | (High ^ sessionId.High) | (Identity ^ sessionId.IndexIdentity)) == 0 && Timeout != DateTime.MinValue)
                {
                    Value = value;
                    Timeout = timeout;
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 新建会话信息
            /// </summary>
            /// <param name="sessionId"></param>
            /// <param name="timeout"></param>
            /// <param name="value"></param>
            public void New(ref sessionId sessionId, DateTime timeout, valueType value)
            {
                Timeout = timeout;
                Low = sessionId.Low;
                High = sessionId.High;
                Value = value;
                sessionId.IndexIdentity = Identity;
            }
            /// <summary>
            /// 获取会话信息
            /// </summary>
            /// <param name="sessionId"></param>
            /// <param name="timeout"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public bool Get(ref sessionId sessionId, DateTime timeout, ref valueType value)
            {
                if (((Low ^ sessionId.Low) | (High ^ sessionId.High) | (Identity ^ sessionId.IndexIdentity)) == 0 && Timeout != DateTime.MinValue)
                {
                    value = Value;
                    Timeout = timeout;
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 删除会话信息
            /// </summary>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            public bool Remove(ref sessionId sessionId)
            {
                if (((Low ^ sessionId.Low) | (High ^ sessionId.High) | (Identity ^ sessionId.IndexIdentity)) == 0 && Timeout != DateTime.MinValue)
                {
                    Value = default(valueType);
                    ++Identity;
                    Timeout = DateTime.MinValue;
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 超时检测
            /// </summary>
            /// <param name="now"></param>
            /// <returns></returns>
            public bool CheckTimeout(DateTime now)
            {
                if (Timeout < now && Timeout != DateTime.MinValue)
                {
                    Value = default(valueType);
                    ++Identity;
                    Timeout = DateTime.MinValue;
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// Session值索引池
        /// </summary>
        private struct pool
        {
            /// <summary>
            /// Session值索引池
            /// </summary>
            public indexValuePool<value> Pool;
            /// <summary>
            /// 超时时钟周期
            /// </summary>
            private long timeoutTicks;
            /// <summary>
            /// 设置Session值索引池
            /// </summary>
            /// <param name="timeoutTicks"></param>
            public void Set(long timeoutTicks)
            {
                this.timeoutTicks = timeoutTicks;
                Pool = new indexValuePool<value>(63);
            }
            /// <summary>
            /// 超时检测
            /// </summary>
            public void Refresh()
            {
                int index = Pool.PoolIndex;
                while (index != 0)
                {
                    if (Pool.Enter())
                    {
                        DateTime now = date.nowTime.Now;
                        if (index > Pool.PoolIndex) index = Pool.PoolIndex;
                        try
                        {
                            for (int endIndex = Math.Max(index - 1024, 0); index != endIndex; )
                            {
                                if (Pool.Pool[--index].CheckTimeout(now)) Pool.FreeContinue(index);
                            }
                        }
                        finally { Pool.Exit(); }
                    }
                    else return;
                }
            }
            /// <summary>
            /// 设置Session值
            /// </summary>
            /// <param name="sessionId">Session名称</param>
            /// <param name="value">值</param>
            /// <returns>是否设置成功</returns>
            public unsafe int Set(ref sessionId sessionId, valueType value)
            {
                if (sessionId.Low != 0 && (uint)sessionId.Index < (uint)Pool.Pool.Length && Pool.Pool[sessionId.Index].Check(ref sessionId) && Pool.Enter())
                {
                    if (Pool.Pool[sessionId.Index].Set(ref sessionId, date.nowTime.Now.AddTicks(timeoutTicks), value))
                    {
                        Pool.Exit();
                        return 1;
                    }
                    Pool.Exit();
                }
                return 0;
            }
            /// <summary>
            /// 设置Session值
            /// </summary>
            /// <param name="sessionId">Session名称</param>
            /// <param name="value">值</param>
            /// <returns>Session是否被更新</returns>
            public unsafe bool New(ref sessionId sessionId, valueType value)
            {
                if (Pool.Enter())
                {
                    try
                    {
                        sessionId.Index = Pool.GetIndexContinue();
                        Pool.Pool[sessionId.Index].New(ref sessionId, date.nowTime.Now.AddTicks(timeoutTicks), value);
                    }
                    finally { Pool.Exit(); }
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 获取Session值
            /// </summary>
            /// <param name="sessionId">Session名称</param>
            /// <param name="value">返回值</param>
            /// <returns>是否存在返回值</returns>
            public bool TryGet(ref sessionId sessionId, out valueType value)
            {
                value = default(valueType);
                if (sessionId.Low != 0 && (uint)sessionId.Index < (uint)Pool.Pool.Length && Pool.Enter())
                {
                    bool isValue = Pool.Pool[sessionId.Index].Get(ref sessionId, date.nowTime.Now.AddTicks(timeoutTicks), ref value);
                    Pool.Exit();
                    return isValue;
                }
                return false;
            }
            /// <summary>
            /// 删除Session
            /// </summary>
            /// <param name="sessionId">Session名称</param>
            public void Remove(ref sessionId sessionId)
            {
                if (sessionId.Low != 0 && (uint)sessionId.Index < (uint)Pool.Pool.Length && Pool.Enter())
                {
                    if (Pool.Pool[sessionId.Index].Remove(ref sessionId)) Pool.FreeExit(sessionId.Index);
                    else Pool.Exit();
                }
            }
        }
        /// <summary>
        /// Session值索引池
        /// </summary>
        private pool[] valuePool;
        /// <summary>
        /// 会话标识
        /// </summary>
        public session() : base(fastCSharp.config.http.Default.SessionMinutes, fastCSharp.config.http.Default.SessionRefreshMinutes)
        {
            valuePool = new pool[256];
            for (int index = valuePool.Length; index != 0; valuePool[--index].Set(timeoutTicks)) ;
        }
        /// <summary>
        /// 会话标识
        /// </summary>
        /// <param name="timeoutMinutes">超时分钟数</param>
        /// <param name="refreshMinutes">超时刷新分钟数</param>
        public session(int timeoutMinutes, int refreshMinutes) : base(timeoutMinutes, refreshMinutes) { }
        /// <summary>
        /// 超时检测
        /// </summary>
        internal unsafe override void RefreshTimeout()
        {
            try
            {
                for (int index = valuePool.Length; index != 0; valuePool[--index].Refresh()) ;
            }
            finally { timerTask.Default.Add(this, thread.callType.HttpSessionRefreshTimeout, date.nowTime.Now.AddTicks(refreshTicks)); }
        }
        /// <summary>
        /// 设置Session值
        /// </summary>
        /// <param name="sessionId">Session名称</param>
        /// <param name="value">值</param>
        /// <returns>Session是否被更新</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool Set(ref sessionId sessionId, object value)
        {
            return Set(ref sessionId, (valueType)value);
        }
        /// <summary>
        /// 设置Session值
        /// </summary>
        /// <param name="sessionId">Session名称</param>
        /// <param name="value">值</param>
        /// <returns>Session是否被更新</returns>
        public unsafe bool Set(ref sessionId sessionId, valueType value)
        {
            if (sessionId.Ticks != (ulong)pub.StartTime.Ticks || valuePool[(byte)sessionId.Low].Set(ref sessionId, value) == 0)
            {
                sessionId.NewNoIndex();
                return valuePool[(byte)sessionId.Low].New(ref sessionId, value);
            }
            return false;
        }
        /// <summary>
        /// 设置Session值
        /// </summary>
        /// <param name="sessionId">Session名称</param>
        /// <param name="value">值</param>
        /// <returns>Session名称</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        public unsafe sessionId Set(sessionId sessionId, valueType value)
        {
            Set(ref sessionId, value);
            return sessionId;
        }
        /// <summary>
        /// 获取Session值
        /// </summary>
        /// <param name="sessionId">Session名称</param>
        /// <param name="nullValue">失败返回值</param>
        /// <returns>Session值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public object Get(ref sessionId sessionId, object nullValue)
        {
            valueType value;
            return TryGet(ref sessionId, out value) ? value : nullValue;
        }
        /// <summary>
        /// 获取Session值
        /// </summary>
        /// <param name="sessionId">Session名称</param>
        /// <param name="nullValue">失败返回值</param>
        /// <returns>Session值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType Get(ref sessionId sessionId, valueType nullValue)
        {
            valueType value;
            return TryGet(ref sessionId, out value) ? value : nullValue;
        }
        /// <summary>
        /// 获取Session值
        /// </summary>
        /// <param name="sessionId">Session名称</param>
        /// <param name="value">返回值</param>
        /// <returns>是否存在返回值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        public bool TryGet(sessionId sessionId, out valueType value)
        {
            return TryGet(ref sessionId, out value);
        }
        /// <summary>
        /// 获取Session值
        /// </summary>
        /// <param name="sessionId">Session名称</param>
        /// <param name="value">返回值</param>
        /// <returns>是否存在返回值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool TryGet(ref sessionId sessionId, out valueType value)
        {
            if (sessionId.Ticks == (ulong)pub.StartTime.Ticks) return valuePool[(byte)sessionId.Low].TryGet(ref sessionId, out value);
            value = default(valueType);
            return false;
        }
        /// <summary>
        /// 删除Session
        /// </summary>
        /// <param name="sessionId">Session名称</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        public void Remove(sessionId sessionId)
        {
            Remove(ref sessionId);
        }
        /// <summary>
        /// 删除Session
        /// </summary>
        /// <param name="sessionId">Session名称</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Remove(ref sessionId sessionId)
        {
            if (sessionId.Ticks == (ulong)pub.StartTime.Ticks) valuePool[(byte)sessionId.Low].Remove(ref sessionId);
        }
    }
#if NotFastCSharpCode
#else
    /// <summary>
    /// 会话标识服务客户端
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    public sealed class sessionClient<valueType> : session<valueType>.tcpClient
#if NOJIT
        , ISession
#else
        , ISession<valueType>
#endif
    {
        /// <summary>
        /// 客户端缓存队列
        /// </summary>
        private fifoPriorityQueue<sessionId, valueType> queue;
        /// <summary>
        /// 客户端缓存队列访问锁
        /// </summary>
        private readonly object queueLock = new object();
        /// <summary>
        /// 客户端缓存队列最大数量
        /// </summary>
        private int queueCount;
        /// <summary>
        /// TCP调用客户端
        /// </summary>
        /// <param name="timeoutMinutes">超时分钟数</param>
        /// <param name="queueCount">客户端缓存队列最大数量</param>
        /// <param name="attribute">TCP调用服务器端配置信息</param>
        /// <param name="verifyMethod">TCP验证方法</param>
#if NOJIT
        public sessionClient(int timeoutMinutes, int queueCount = 0, fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject verifyMethod = null)
#else
        public sessionClient(int timeoutMinutes, int queueCount = 0, fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<session<valueType>.tcpClient> verifyMethod = null)
#endif
            : base(attribute, verifyMethod)
        {
            queue = new fifoPriorityQueue<sessionId, valueType>();
            this.queueCount = Math.Max(queueCount <= 0 ? fastCSharp.config.http.Default.ClientSessionQueueCount : queueCount, 1);
        }
        /// <summary>
        /// 设置Session值
        /// </summary>
        /// <param name="sessionId">Session名称</param>
        /// <param name="value">值</param>
        /// <returns>Session是否被更新</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool Set(ref sessionId sessionId, object value)
        {
            return Set(ref sessionId, (valueType)value);
        }
        /// <summary>
        /// 设置Session值
        /// </summary>
        /// <param name="sessionId">Session名称</param>
        /// <param name="value">值</param>
        /// <returns>Session是否被更新</returns>
        public bool Set(ref sessionId sessionId, valueType value)
        {
            fastCSharp.net.returnValue<sessionId> newSessionId = Set(sessionId, value);
            if (newSessionId.IsReturn)
            {
                set(ref newSessionId.Value, value);
                return newSessionId.Value.Equals(ref sessionId) != 0;
            }
            return false;
        }
        /// <summary>
        /// 设置Session缓存
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="value"></param>
        private void set(ref sessionId sessionId, valueType value)
        {
            Monitor.Enter(queueLock);
            try
            {
                queue.Set(sessionId, value);
                if (queue.Count > queueCount) queue.UnsafePopValue();
            }
            finally { Monitor.Exit(queueLock); }
        }
        /// <summary>
        /// 获取Session值
        /// </summary>
        /// <param name="sessionId">Session名称</param>
        /// <param name="nullValue">失败返回值</param>
        /// <returns>Session值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public object Get(ref sessionId sessionId, object nullValue)
        {
            valueType value;
            if (TryGet(sessionId, out value).Value)
            {
                set(ref sessionId, value);
                return value;
            }
            return nullValue;
        }
        /// <summary>
        /// 获取Session值
        /// </summary>
        /// <param name="sessionId">Session名称</param>
        /// <param name="nullValue">失败返回值</param>
        /// <returns>Session值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType Get(ref sessionId sessionId, valueType nullValue)
        {
            valueType value;
            if (TryGet(sessionId, out value).Value)
            {
                set(ref sessionId, value);
                return value;
            }
            return nullValue;
        }
        /// <summary>
        /// 删除Session
        /// </summary>
        /// <param name="sessionId">Session名称</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Remove(ref sessionId sessionId)
        {
            valueType value;
            Monitor.Enter(queueLock);
            queue.Remove(sessionId, out value);
            Monitor.Exit(queueLock);
            Remove(sessionId);
        }
    }
#endif
}
