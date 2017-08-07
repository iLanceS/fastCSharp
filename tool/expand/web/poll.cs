using System;
using System.Collections.Generic;
using fastCSharp.threading;
using fastCSharp;
using System.Runtime.CompilerServices;
using System.Threading;

namespace fastCSharp.web
{
    /// <summary>
    /// 长连接轮询
    /// </summary>
    public sealed class poll
    {
        /// <summary>
        /// 轮询验证信息
        /// </summary>
        public struct verifyInfo
        {
            /// <summary>
            /// 用户标识
            /// </summary>
            public int UserId;
            /// <summary>
            /// 验证信息
            /// </summary>
            public string Verify;
        }
        /// <summary>
        /// 会话超时
        /// </summary>
        private struct sessionTimeout
        {
            /// <summary>
            /// 超时
            /// </summary>
            public DateTime Timeout;
            /// <summary>
            /// 会话标识
            /// </summary>
            public sessionId SessionId;
            /// <summary>
            /// 新建会话标识
            /// </summary>
            /// <param name="timeout"></param>
            /// <returns></returns>
            public sessionId New(DateTime timeout)
            {
                SessionId.New();
                Timeout = timeout;
                return SessionId;
            }
            /// <summary>
            /// 会话标识验证
            /// </summary>
            /// <param name="verify"></param>
            /// <returns></returns>
            public bool Check(string verify)
            {
                return Timeout > date.NowSecond && SessionId.CheckHex(verify);
            }
            /// <summary>
            /// 清除数据
            /// </summary>
            public void Clear()
            {
                Timeout = DateTime.MinValue;
                SessionId.Null();
            }
        }
        /// <summary>
        /// 会话超时集合
        /// </summary>
        private struct session
        {
            /// <summary>
            /// 长连接轮询验证集合
            /// </summary>
            private sessionTimeout[] sessions;
            /// <summary>
            /// 长连接轮询验证访问锁
            /// </summary>
            private readonly object sessionLock;
            /// <summary>
            /// 超时时钟周期
            /// </summary>
            private long timeoutTicks;
            /// <summary>
            /// 会话超时集合
            /// </summary>
            /// <param name="timeoutTicks">超时时钟周期</param>
            public session(long timeoutTicks)
            {
                sessions = nullValue<sessionTimeout>.Array;
                sessionLock = new object();
                this.timeoutTicks = timeoutTicks;
            }
            /// <summary>
            /// 获取用户长连接轮询验证
            /// </summary>
            /// <param name="userId">用户标识</param>
            /// <param name="sessionId">长连接轮询验证,0表示失败</param>
            public void Get(int userId, out sessionId sessionId)
            {
                int index = userId >> 8;
                if ((uint)index < (uint)sessions.Length)
                {
                    Monitor.Enter(sessionLock);
                    sessionId = sessions[index].Timeout > date.NowSecond ? sessions[index].SessionId : sessions[index].New(date.NowSecond.AddTicks(timeoutTicks));
                    Monitor.Exit(sessionLock);
                }
                else sessionId = default(sessionId);
            }
            /// <summary>
            /// 添加用户长连接轮询验证
            /// </summary>
            /// <param name="userId"></param>
            /// <returns></returns>
            public string Add(int userId)
            {
                sessionId sessionId;
                int index = userId >> 8;
                if (index < sessions.Length)
                {
                    Monitor.Enter(sessionLock);
                    sessionId = sessions[index].Timeout > date.NowSecond ? sessions[index].SessionId : sessions[index].New(date.NowSecond.AddTicks(timeoutTicks));
                    Monitor.Exit(sessionLock);
                }
                else
                {
                    Monitor.Enter(sessionLock);
                    if (index < sessions.Length)
                    {
                        sessionId = sessions[index].Timeout > date.NowSecond ? sessions[index].SessionId : sessions[index].New(date.NowSecond.AddTicks(timeoutTicks));
                        Monitor.Exit(sessionLock);
                    }
                    else
                    {
                        try
                        {
                            sessionTimeout[] newSessions = new sessionTimeout[Math.Max(sessions.Length << 1, index + 256)];
                            sessions.CopyTo(newSessions, 0);
                            sessions = newSessions;
                            sessionId = newSessions[index].New(date.NowSecond.AddTicks(timeoutTicks));
                        }
                        finally { Monitor.Exit(sessionLock); }
                    }
                }
                return sessionId.ToHexString();
            }
            /// <summary>
            /// 删除用户长连接轮询验证
            /// </summary>
            /// <param name="userId"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Remove(int userId)
            {
                int index = userId >> 8;
                if ((uint)index < (uint)sessions.Length)
                {
                    Monitor.Enter(sessionLock);
                    sessions[index].Clear();
                    Monitor.Exit(sessionLock);
                }
            }
            /// <summary>
            /// 轮询验证检测
            /// </summary>
            /// <param name="verify"></param>
            /// <returns></returns>
            public bool Verify(ref verifyInfo verify)
            {
                if (verify.Verify != null)
                {
                    int index = verify.UserId >> 8;
                    return (uint)index < (uint)sessions.Length && sessions[index].Check(verify.Verify);
                }
                return false;
            }
        }
        /// <summary>
        /// 长连接轮询验证集合
        /// </summary>
        private session[] sessions;
        /// <summary>
        /// 超时秒数
        /// </summary>
        /// <param name="timeoutSeconds"></param>
        public poll(int timeoutSeconds = 3600)
        {
            long timeoutTicks = fastCSharp.date.SecondTicks * Math.Max(timeoutSeconds, 60);
            sessions = new session[256];
            for (int index = sessions.Length; index != 0; sessions[--index] = new session(timeoutTicks)) ;
        }
        /// <summary>
        /// 获取用户长连接轮询验证
        /// </summary>
        /// <param name="userId">用户标识</param>
        /// <returns>长连接轮询验证</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private string getString(int userId)
        {
            sessionId sessionId;
            get(userId, out sessionId);
            return sessionId.IsNull ? null : sessionId.ToHexString();
        }
        /// <summary>
        /// 获取用户长连接轮询验证
        /// </summary>
        /// <param name="userId">用户标识</param>
        /// <param name="sessionId">长连接轮询验证,0表示失败</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void get(int userId, out sessionId sessionId)
        {
            sessions[userId & 0xff].Get(userId, out sessionId);
        }
        /// <summary>
        /// 获取用户长连接轮询验证
        /// </summary>
        /// <param name="userId">用户标识</param>
        /// <returns>长连接轮询验证</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public verifyInfo Get(int userId)
        {
            return new verifyInfo { UserId = userId, Verify = userId < 0 ? null : (getString(userId) ?? add(userId)) };
        }
        /// <summary>
        /// 添加用户长连接轮询验证
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private string add(int userId)
        {
            return sessions[userId & 0xff].Add(userId);
        }
        /// <summary>
        /// 删除用户长连接轮询验证
        /// </summary>
        /// <param name="userId"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Remove(int userId)
        {
            sessions[userId & 0xff].Remove(userId);
        }
        /// <summary>
        /// 轮询验证检测
        /// </summary>
        /// <param name="verify"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool Verify(ref verifyInfo verify)
        {
            return sessions[verify.UserId & 0xff].Verify(ref verify);
        }
        /// <summary>
        /// 长连接轮询
        /// </summary>
        public sealed class dictionary
        {
            /// <summary>
            /// 会话超时集合
            /// </summary>
            private struct session
            {
                /// <summary>
                /// 长连接轮询验证集合
                /// </summary>
                private readonly Dictionary<int, sessionTimeout> sessions;
                /// <summary>
                /// 长连接轮询验证访问锁
                /// </summary>
                private readonly object sessionLock;
                /// <summary>
                /// 超时时钟周期
                /// </summary>
                private long timeoutTicks;
                /// <summary>
                /// 会话超时集合
                /// </summary>
                /// <param name="timeoutTicks"></param>
                public session(long timeoutTicks)
                {
                    sessions = fastCSharp.dictionary.CreateInt<sessionTimeout>();
                    sessionLock = new object();
                    this.timeoutTicks = timeoutTicks;
                }
                /// <summary>
                /// 获取用户长连接轮询验证
                /// </summary>
                /// <param name="userId">用户标识</param>
                /// <param name="sessionId">长连接轮询验证,0表示失败</param>
                public void Get(int userId, ref sessionId sessionId)
                {
                    sessionTimeout value;
                    if (sessions.TryGetValue(userId, out value))
                    {
                        if (value.Timeout > date.NowSecond)
                        {
                            sessionId = value.SessionId;
                            return;
                        }
                        sessionId.New();
                        Monitor.Enter(sessionLock);
                        try
                        {
                            sessions[userId] = new sessionTimeout { SessionId = sessionId, Timeout = date.NowSecond.AddTicks(timeoutTicks) };
                        }
                        finally { Monitor.Exit(sessionLock); }
                        return;
                    }
                    sessionId.Null();
                }
                /// <summary>
                /// 添加用户长连接轮询验证
                /// </summary>
                /// <param name="userId"></param>
                /// <returns></returns>
                public string Add(int userId)
                {
                    sessionTimeout value;
                    if (!sessions.TryGetValue(userId, out value))
                    {
                        sessionId sessionId = default(sessionId);
                        sessionId.New();
                        Monitor.Enter(sessionLock);
                        try
                        {
                            if (!sessions.ContainsKey(userId))
                            {
                                sessions.Add(userId, value = new sessionTimeout { SessionId = sessionId, Timeout = date.NowSecond.AddTicks(timeoutTicks) });
                            }
                        }
                        finally { Monitor.Exit(sessionLock); }
                    }
                    return value.SessionId.ToHexString();
                }
                /// <summary>
                /// 删除用户长连接轮询验证
                /// </summary>
                /// <param name="userId"></param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Remove(int userId)
                {
                    Monitor.Enter(sessionLock);
                    sessions.Remove(userId);
                    Monitor.Exit(sessionLock);
                }
                /// <summary>
                /// 轮询验证检测
                /// </summary>
                /// <param name="verify"></param>
                /// <returns></returns>
                public bool Verify(ref verifyInfo verify)
                {
                    if (verify.Verify != null)
                    {
                        sessionTimeout value;
                        return sessions.TryGetValue(verify.UserId, out value) && value.Check(verify.Verify);
                    }
                    return true;
                }
            }
            /// <summary>
            /// 长连接轮询验证集合
            /// </summary>
            private session[] sessions;
            /// <summary>
            /// 超时秒数
            /// </summary>
            /// <param name="timeoutSeconds"></param>
            public dictionary(int timeoutSeconds = 3600)
            {
                long timeoutTicks = fastCSharp.date.SecondTicks * Math.Max(timeoutSeconds, 60);
                sessions = new session[256];
                for (int index = sessions.Length; index != 0; sessions[--index] = new session(timeoutTicks)) ;
            }
            /// <summary>
            /// 获取用户长连接轮询验证
            /// </summary>
            /// <param name="userId">用户标识</param>
            /// <returns>长连接轮询验证</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private string getString(int userId)
            {
                sessionId sessionId = new sessionId();
                get(userId, ref sessionId);
                return sessionId.IsNull ? null : sessionId.ToHexString();
            }
            /// <summary>
            /// 获取用户长连接轮询验证
            /// </summary>
            /// <param name="userId">用户标识</param>
            /// <param name="sessionId">长连接轮询验证,0表示失败</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private void get(int userId, ref sessionId sessionId)
            {
                sessions[userId & 0xff].Get(userId, ref sessionId);
            }
            /// <summary>
            /// 获取用户长连接轮询验证
            /// </summary>
            /// <param name="userId">用户标识</param>
            /// <returns>长连接轮询验证</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public verifyInfo Get(int userId)
            {
                return new verifyInfo { UserId = userId, Verify = getString(userId) ?? add(userId) };
            }
            /// <summary>
            /// 添加用户长连接轮询验证
            /// </summary>
            /// <param name="userId"></param>
            /// <returns></returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private string add(int userId)
            {
                return sessions[userId & 0xff].Add(userId);
            }
            /// <summary>
            /// 删除用户长连接轮询验证
            /// </summary>
            /// <param name="userId"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Remove(int userId)
            {
                sessions[userId & 0xff].Remove(userId);
            }
            /// <summary>
            /// 轮询验证检测
            /// </summary>
            /// <param name="verify"></param>
            /// <returns></returns>
            public bool Verify(ref verifyInfo verify)
            {
                return sessions[verify.UserId & 0xff].Verify(ref verify);
            }
        }
    }
}
