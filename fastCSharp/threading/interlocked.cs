using System;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace fastCSharp.threading
{
    /// <summary>
    /// 原子操作扩张
    /// </summary>
    public static class interlocked
    {
        /// <summary>
        /// Yield调用次数
        /// </summary>
        private static long staticYieldCount;
        /// <summary>
        /// Yield调用次数
        /// </summary>
        public static long StaticYieldCount
        {
            get { return staticYieldCount; }
        }
        /// <summary>
        /// 将目标值从0改置为1,循环等待周期切换(适应于等待时间极短的情况)
        /// </summary>
        /// <param name="value">目标值</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static void CompareSetYield(ref int value)
        {
            while (Interlocked.CompareExchange(ref value, 1, 0) != 0)
            {
                Interlocked.Increment(ref staticYieldCount);
                Thread.Yield();
            }
        }
        /// <summary>
        /// 将目标值从0改置为1,循环等待周期切换(适应于等待时间较短的情况)
        /// </summary>
        /// <param name="value">目标值</param>
        public static void CompareSetSleep1(ref int value)
        {
            if (Interlocked.CompareExchange(ref value, 1, 0) == 0) return;
            Thread.Sleep(0);
            if (Interlocked.CompareExchange(ref value, 1, 0) == 0) return;
            DateTime time = date.nowTime.Now.AddSeconds(2);
            do
            {
                Thread.Sleep(1);
                if (Interlocked.CompareExchange(ref value, 1, 0) == 0) return;
            }
            while (date.nowTime.Now < time);
            log.Default.Add("线程等待时间过长", null, false);
            while (Interlocked.CompareExchange(ref value, 1, 0) != 0) Thread.Sleep(1);
        }
        /// <summary>
        /// 字典
        /// </summary>
        /// <typeparam name="keyType"></typeparam>
        /// <typeparam name="valueType"></typeparam>
        public sealed class dictionary<keyType, valueType>
        {
            /// <summary>
            /// 字典
            /// </summary>
            private readonly Dictionary<keyType, valueType> values = dictionary.CreateAny<keyType, valueType>();
            /// <summary>
            /// 访问锁
            /// </summary>
            private readonly object valueLock = new object();
            /// <summary>
            /// 获取数据
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <returns>是否存在数据</returns>
            public bool TryGetValue(keyType key, out valueType value)
            {
                Monitor.Enter(valueLock);
                if (values.TryGetValue(key, out value))
                {
                    Monitor.Exit(valueLock); 
                    return true;
                }
                Monitor.Exit(valueLock); 
                return false;
            }
            /// <summary>
            /// 设置数据
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Set(keyType key, valueType value)
            {
                Monitor.Enter(valueLock);
                try
                {
                    values[key] = value;
                }
                finally { Monitor.Exit(valueLock); }
            }
            /// <summary>
            /// 获取数据
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <param name="oldValue">被替换的数据</param>
            /// <returns>是否存在数据</returns>
            public bool Set(keyType key, valueType value, out valueType oldValue)
            {
                Monitor.Enter(valueLock);
                if (values.TryGetValue(key, out oldValue))
                {
                    try
                    {
                        values[key] = value;
                    }
                    finally { Monitor.Exit(valueLock); }
                    return true;
                }
                try
                {
                    values.Add(key, value);
                }
                finally { Monitor.Exit(valueLock); }
                return false;
            }
        }
        /// <summary>
        /// 字典
        /// </summary>
        /// <typeparam name="keyType"></typeparam>
        /// <typeparam name="valueType"></typeparam>
        public sealed class lastDictionary<keyType, valueType> where keyType : struct, IEquatable<keyType>
        {
            /// <summary>
            /// 字典
            /// </summary>
            private readonly Dictionary<keyType, valueType> values = dictionary<keyType>.Create<valueType>();
            /// <summary>
            /// 访问锁
            /// </summary>
            private readonly object valueLock = new object();
            /// <summary>
            /// 最后一次访问的关键字
            /// </summary>
            private keyType lastKey;
            /// <summary>
            /// 最后一次访问的数据
            /// </summary>
            private valueType lastValue;
            /// <summary>
            /// 获取数据
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <returns>是否存在数据</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public bool TryGetValue(ref keyType key, out valueType value)
            {
                if (TryGetValueEnter(ref key, out value)) return true;
                Monitor.Exit(valueLock);
                return false;
            }
            /// <summary>
            /// 获取数据
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <returns>是否存在数据</returns>
            internal bool TryGetValueEnter(ref keyType key, out valueType value)
            {
                Monitor.Enter(valueLock);
                if (lastKey.Equals(key))
                {
                    value = lastValue;
                    Monitor.Exit(valueLock);
                    return true;
                }
                if (values.TryGetValue(key, out value))
                {
                    lastKey = key;
                    lastValue = value;
                    Monitor.Exit(valueLock);
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 设置数据
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Set(ref keyType key, valueType value)
            {
                Monitor.Enter(valueLock);
                try
                {
                    values[lastKey = key] = lastValue = value;
                }
                finally { Monitor.Exit(valueLock); }
            }
            /// <summary>
            /// 设置数据
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void SetOnly(ref keyType key, valueType value)
            {
                values[lastKey = key] = lastValue = value;
            }
            /// <summary>
            /// 释放目标
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void Exit()
            {
                Monitor.Exit(valueLock);
            }
        }
        /// <summary>
        /// 字典
        /// </summary>
        /// <typeparam name="keyType"></typeparam>
        /// <typeparam name="valueType"></typeparam>
        internal sealed class classLastDictionary<keyType, valueType> where keyType : class
        {
            /// <summary>
            /// 字典
            /// </summary>
            private readonly Dictionary<keyType, valueType> values = dictionary.CreateOnly<keyType, valueType>();
            /// <summary>
            /// 访问锁
            /// </summary>
            private readonly object valueLock = new object();
            /// <summary>
            /// 最后一次访问数据锁
            /// </summary>
            private int lastLock;
            /// <summary>
            /// 最后一次访问的关键字
            /// </summary>
            private keyType lastKey;
            /// <summary>
            /// 最后一次访问的数据
            /// </summary>
            private valueType lastValue;
            /// <summary>
            /// 获取数据
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <returns>是否存在数据</returns>
            public bool TryGetValue(keyType key, out valueType value)
            {
                CompareSetYield(ref lastLock);
                if (lastKey == key)
                {
                    value = lastValue;
                    lastLock = 0;
                    return true;
                }
                lastLock = 0;
                Monitor.Enter(valueLock);
                if (values.TryGetValue(key, out value))
                {
                    CompareSetYield(ref lastLock);
                    lastKey = key;
                    lastValue = value;
                    lastLock = 0;
                    Monitor.Exit(valueLock);
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 设置数据
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Set(keyType key, valueType value)
            {
                CompareSetYield(ref lastLock);
                lastKey = key;
                lastValue = value;
                lastLock = 0;
                values[key] = value;
            }
            /// <summary>
            /// 释放目标
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Exit()
            {
                Monitor.Exit(valueLock);
            }
        }
    }
}
