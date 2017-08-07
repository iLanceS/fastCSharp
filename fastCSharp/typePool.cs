using System;
using System.Collections.Generic;
using fastCSharp.threading;
using System.Threading;
using fastCSharp.reflection;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 类型对象池
    /// </summary>
    public static class typePool
    {
        /// <summary>
        /// 对象池操作
        /// </summary>
        internal struct pool
        {
            /// <summary>
            /// 清除对象池+保留对象数量
            /// </summary>
            public Action<int> Clear;
        }
        /// <summary>
        /// 类型对象池操作集合
        /// </summary>
        private static readonly Dictionary<Type, pool> pools = dictionary.CreateOnly<Type, pool>();
        /// <summary>
        /// 类型对象池操作集合访问锁
        /// </summary>
        private static readonly object poolLock = new object();
        /// <summary>
        /// 添加类型对象池
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="pool">类型对象池</param>
        internal static void Add(Type type, pool pool)
        {
            Monitor.Enter(poolLock);
            try
            {
                pools.Add(type, pool);
            }
            finally { Monitor.Exit(poolLock); }
        }
        /// <summary>
        /// 清除类型对象池
        /// </summary>
        /// <param name="count">保留对象数量</param>
        public static void ClearPool(int count = 0)
        {
            if (count <= 0) count = 0;
            Monitor.Enter(poolLock);
            foreach (pool pool in pools.Values) pool.Clear(count);
            Monitor.Exit(poolLock);
        }
        static typePool()
        {
            if (fastCSharp.config.appSetting.IsCheckMemory) checkMemory.Add(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }
    }
    /// <summary>
    /// 类型对象池
    /// </summary>
    /// <typeparam name="valueType">对象类型</typeparam>
    public static class typePool<valueType> where valueType : class
    {
        /// <summary>
        /// 类型对象池访问锁
        /// </summary>
        private static readonly object poolLock = new object();
        ///// <summary>
        ///// 类型对象池对象数量
        ///// </summary>
        //private static int poolCount;
        /// <summary>
        /// 添加类型对象
        /// </summary>
        /// <param name="value">类型对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static void Push(ref valueType value)
        {
            valueType pushValue = Interlocked.Exchange(ref value, null);
            if (pushValue != null) PushNotNull(pushValue);
        }
        /// <summary>
        /// 添加类型对象
        /// </summary>
        /// <param name="value">类型对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static void PushOnly(valueType value)
        {
            if (value != null) PushNotNull(value);
        }
        ///// <summary>
        ///// 添加类型对象集合
        ///// </summary>
        ///// <param name="values"></param>
        ///// <param name="count"></param>
        //internal static void Push(ref array.value<valueType>[] values, int count)
        //{
        //    if (fastCSharp.config.appSetting.IsPoolDebug)
        //    {
        //        foreach (array.value<valueType> value in values)
        //        {
        //            push(value.Value);
        //            if (--count == 0) break;
        //        }
        //    }
        //    else pool.Push(values, count);
        //    values = null;
        //}
        ///// <summary>
        ///// 获取类型对象集合
        ///// </summary>
        ///// <param name="values"></param>
        ///// <returns></returns>
        //internal static int Pop(array.value<valueType>[] values)
        //{
        //    if (fastCSharp.config.appSetting.IsPoolDebug)
        //    {
        //        int index = 0;
        //        poolLock.NoCheckCompareSetSleep0();
        //        foreach (valueType poolValue in debugPool)
        //        {
        //            values[index++].Value = poolValue;
        //            if (index == values.Length) break;
        //        }
        //        if (index != 0)
        //        {
        //            int count = index;
        //            foreach (array.value<valueType> value in values)
        //            {
        //                debugPool.Remove(value.Value);
        //                if (--count == 0) break;
        //            }
        //        }
        //        poolLock.Exit();
        //        return index;
        //    }
        //    return pool.Pop(values);
        //}
#if DEBUGPOOL
        /// <summary>
        /// 类型对象池
        /// </summary>
        private static readonly HashSet<valueType> debugPool;
        /// <summary>
        /// 对象数量
        /// </summary>
        /// <returns>对象数量</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static int Count()
        {
            return debugPool.Count;
        }
        /// <summary>
        /// 添加类型对象
        /// </summary>
        /// <param name="value">类型对象</param>
        public static void PushNotNull(valueType value)
        {
            bool isAdd, isMax = false;
            Monitor.Enter(poolLock);
            try
            {
                if ((isAdd = debugPool.Add(value)) && (isMax = debugPool.Count > poolCount))
                {
                    poolCount <<= 1;
                }
            }
            finally { Monitor.Exit(poolLock); }
            if (isAdd)
            {
                if (isMax)
                {
                    log.Default.Add("类型对象池扩展实例数量 " + typeof(valueType).fullName() + "[" + debugPool.Count.toString() + "]", new System.Diagnostics.StackFrame(), false);
                }
            }
            else log.Error.Add("对象池释放冲突 " + typeof(valueType).fullName(), null, false);
        }
        /// <summary>
        /// 获取类型对象
        /// </summary>
        /// <returns>类型对象</returns>
        public static valueType Pop()
        {
            valueType value = null;
            Monitor.Enter(poolLock);
            foreach (valueType poolValue in debugPool)
            {
                value = poolValue;
                break;
            }
            if (value != null) debugPool.Remove(value);
            Monitor.Exit(poolLock);
            return value;
        }
        /// <summary>
        /// 清除对象池
        /// </summary>
        /// <param name="count">保留对象数量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static void Clear(int count = 0)
        {
            ClearDebug(debugPool, poolLock, count);
        }
        /// <summary>
        /// 清除对象池
        /// </summary>
        /// <param name="pool">类型对象池</param>
        /// <param name="poolLock">类型对象池访问锁</param>
        /// <param name="count">保留对象数量</param>
        internal static void ClearDebug(HashSet<valueType> pool, object poolLock, int count)
        {
            valueType[] removeValues = null;
            Monitor.Enter(poolLock);
            int removeCount = pool.Count - count;
            if (removeCount > 0)
            {
                try
                {
                    removeValues = new valueType[removeCount];
                    foreach (valueType value in pool)
                    {
                        removeValues[--removeCount] = value;
                        if (removeCount == 0) break;
                    }
                    foreach (valueType value in removeValues) pool.Remove(value);
                }
                finally { Monitor.Exit(poolLock); }
                Action<valueType> dispose = objectPool<valueType>.Dispose;
                if (dispose != null)
                {
                    foreach (valueType value in removeValues)
                    {
                        try
                        {
                            dispose(value);
                        }
                        catch (Exception error)
                        {
                            log.Default.Add(error, null, false);
                        }
                    }
                }
            }
            else Monitor.Exit(poolLock); 
        }
        static typePool()
        {
            Type type = typeof(valueType);
            debugPool = hashSet.CreateOnly<valueType>();
            typePool.Add(type, new typePool.pool { Clear = Clear });
            poolCount = fastCSharp.config.appSetting.PoolSize;
        }
#else
        /// <summary>
        /// 类型对象池
        /// </summary>
        private static objectPool<valueType> pool;
        /// <summary>
        /// 对象数量
        /// </summary>
        /// <returns>对象数量</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static int Count()
        {
            return pool.Count;
        }
        /// <summary>
        /// 添加类型对象
        /// </summary>
        /// <param name="value">类型对象</param>
        public static void PushNotNull(valueType value)
        {
            pool.Push(value);
        }
        /// <summary>
        /// 获取类型对象
        /// </summary>
        /// <returns>类型对象</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType Pop()
        {
            return pool.Pop();
        }
        /// <summary>
        /// 清除对象池
        /// </summary>
        /// <param name="count">保留对象数量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static void clear(int count)
        {
            pool.Clear(count);
        }
        /// <summary>
        /// 清除对象池
        /// </summary>
        /// <param name="count">保留对象数量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static void Clear(int count = 0)
        {
            clear(0);
        }
        static typePool()
        {
            Type type = typeof(valueType);
            pool = objectPool<valueType>.Create();
            typePool.Add(type, new typePool.pool { Clear = clear });
        }
#endif
    }
    /// <summary>
    /// 类型对象池
    /// </summary>
    /// <typeparam name="markType">标识类型</typeparam>
    /// <typeparam name="valueType">对象类型</typeparam>
    public sealed class typePool<markType, valueType> where valueType : class
    {
        /// <summary>
        /// 添加类型对象
        /// </summary>
        /// <param name="value">类型对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Push(valueType value)
        {
            if (value != null) push(value);
        }
        /// <summary>
        /// 添加类型对象
        /// </summary>
        /// <param name="value">类型对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Push(ref valueType value)
        {
            valueType pushValue = Interlocked.Exchange(ref value, null);
            if (pushValue != null) push(pushValue);
        }
#if DEBUGPOOL
        /// <summary>
        /// 类型对象池
        /// </summary>
        private HashSet<valueType> pool;
        /// <summary>
        /// 类型对象池访问锁
        /// </summary>
        private readonly object poolLock = new object();
        /// <summary>
        /// 当前最大缓冲区数量
        /// </summary>
        private int maxCount = config.appSetting.PoolSize;
        /// <summary>
        /// 纠错模式对象池
        /// </summary>
        public typePool()
        {
            pool = hashSet.CreateOnly<valueType>();
            typePool.Add(typeof(typePool<markType, valueType>), new typePool.pool { Clear = clear });
        }
        /// <summary>
        /// 添加类型对象
        /// </summary>
        /// <param name="value">类型对象</param>
        private void push(valueType value)
        {
            bool isAdd, isMax = false;
            Monitor.Enter(poolLock);
            try
            {
                if ((isAdd = pool.Add(value)) && (isMax = pool.Count > maxCount))
                {
                    maxCount <<= 1;
                }
            }
            finally { Monitor.Exit(poolLock); }
            if (isAdd)
            {
                if (isMax)
                {
                    log.Default.Add("类型对象池扩展实例数量 " + typeof(valueType).fullName() + "[" + pool.Count.toString() + "]@" + typeof(markType).fullName(), new System.Diagnostics.StackFrame(), false);
                }
            }
            else log.Error.Add("对象池释放冲突 " + typeof(markType).fullName() + " -> " + typeof(valueType).fullName(), null, false);
        }
        /// <summary>
        /// 清除对象池
        /// </summary>
        /// <param name="count">保留对象数量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void clear(int count)
        {
            typePool<valueType>.ClearDebug(pool, poolLock, count);
        }
        /// <summary>
        /// 获取类型对象
        /// </summary>
        /// <returns>类型对象</returns>
        public valueType Pop()
        {
            valueType value = null;
            Monitor.Enter(poolLock);
            foreach (valueType poolValue in pool)
            {
                value = poolValue;
                break;
            }
            if (value != null) pool.Remove(value);
            Monitor.Exit(poolLock);
            return value;
        }
#else
        /// <summary>
        /// 类型对象池
        /// </summary>
        private objectPool<valueType> pool;
        /// <summary>
        /// 数组模式对象池
        /// </summary>
        public typePool()
        {
            pool = objectPool<valueType>.Create();
            typePool.Add(typeof(typePool<markType, valueType>), new typePool.pool { Clear = clear });
        }
        /// <summary>
        /// 添加类型对象
        /// </summary>
        /// <param name="value">类型对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void push(valueType value)
        {
            pool.Push(value);
        }
        /// <summary>
        /// 清除对象池
        /// </summary>
        /// <param name="count">保留对象数量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void clear(int count)
        {
            pool.Clear(count);
        }
        /// <summary>
        /// 获取类型对象
        /// </summary>
        /// <returns>类型对象</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType Pop()
        {
            return pool.Pop();
        }
#endif
        /// <summary>
        /// 类型对象池
        /// </summary>
        public static typePool<markType, valueType> Default = new typePool<markType, valueType>();
    }
}
