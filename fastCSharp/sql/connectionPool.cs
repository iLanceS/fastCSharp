using System;
using System.Data.Common;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using fastCSharp.threading;

namespace fastCSharp.sql
{
    /// <summary>
    /// 连接池
    /// </summary>
    internal class connectionPool
    {
        /// <summary>
        /// 连接池
        /// </summary>
        private objectPool<DbConnection> pool;
        /// <summary>
        /// 获取连接
        /// </summary>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public DbConnection Pop()
        {
            return pool.Pop();
        }
        /// <summary>
        /// 连接池处理
        /// </summary>
        /// <param name="connection"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Push(ref DbConnection connection)
        {
            pool.Push(connection);
            connection = null;
        }
        /// <summary>
        /// 连接池标识
        /// </summary>
        private struct key : IEquatable<key>
        {
            /// <summary>
            /// 连接字符串
            /// </summary>
            public string Connection;
            /// <summary>
            /// 哈希值
            /// </summary>
            public int HashCode;
            /// <summary>
            /// SQL类型
            /// </summary>
            public type Type;
            /// <summary>
            /// 连接池标识
            /// </summary>
            /// <param name="type">SQL类型</param>
            /// <param name="connection">连接字符串</param>
            public key(type type, string connection)
            {
                HashCode = (Connection = connection).GetHashCode() ^ (int)(byte)(Type = type);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public bool Equals(key other)
            {
                return Type == other.Type && Connection == other.Connection;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return HashCode;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                return Equals((key)obj);
            }
        }
        /// <summary>
        /// 连接池集合访问锁
        /// </summary>
        private static interlocked.lastDictionary<key, connectionPool> poolLock = new interlocked.lastDictionary<key, connectionPool>();
        /// <summary>
        /// 获取连接池
        /// </summary>
        /// <param name="type">SQL类型</param>
        /// <param name="connection">连接字符串</param>
        /// <returns></returns>
        public static connectionPool Get(type type, string connection)
        {
            connectionPool pool;
            key key = new key(type, connection);
            if (poolLock.TryGetValueEnter(ref key, out pool)) return pool;
            try
            {
                poolLock.SetOnly(ref key, pool = new connectionPool { pool = objectPool<DbConnection>.Create() });
            }
            finally { poolLock.Exit(); }
            return pool;
        }
    }
}
