using System;
using System.Collections.Generic;
using fastCSharp.threading;
using System.Runtime.CompilerServices;
using System.Threading;

namespace fastCSharp
{
    /// <summary>
    /// 超时字典
    /// </summary>
    /// <typeparam name="keyType">关键字类型</typeparam>
    /// <typeparam name="valueType">数据类型</typeparam>
    public class timeoutDictionary<keyType, valueType> : IDisposable
        where keyType : IEquatable<keyType>
    {
        /// <summary>
        /// 数据超时信息
        /// </summary>
        private struct value
        {
            /// <summary>
            /// 超时时间
            /// </summary>
            public DateTime Timeout;
            /// <summary>
            /// 数据
            /// </summary>
            public valueType Value;
        }
        /// <summary>
        /// 超时时钟周期
        /// </summary>
        private long timeoutTicks;
        /// <summary>
        /// 数据集合
        /// </summary>
        private Dictionary<keyType, value> values = dictionary<keyType>.Create<value>();
        /// <summary>
        /// 移除事件
        /// </summary>
        public event Action<keyType, valueType> OnRemovedLock;
        /// <summary>
        /// 字典访问锁
        /// </summary>
        private readonly object dictionaryLock = new object();
        /// <summary>
        /// 是否正在刷新
        /// </summary>
        private int isRefresh;
        /// <summary>
        /// 超时检测
        /// </summary>
        private Action refresh;
        /// <summary>
        /// 超时检测数据集合
        /// </summary>
        private list<keyValue<keyType, valueType>> refreshValues = new list<keyValue<keyType, valueType>>();
        /// <summary>
        /// 获取或者设置数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>数据</returns>
        public valueType this[keyType key]
        {
            get
            {
                valueType value = default(valueType);
                if (TryGetValue(key, ref value)) return value;
                log.Error.Throw(log.exceptionType.IndexOutOfRange);
                return value;
            }
            set
            {
                int isRefresh = 1;
                DateTime timeout = date.NowSecond.AddTicks(timeoutTicks);
                value timeValue;
                Monitor.Enter(dictionaryLock);
                try
                {
                    if (values.Count == 0)
                    {
                        isRefresh = this.isRefresh;
                        this.isRefresh = 1;
                        values[key] = new value { Timeout = timeout, Value = value };
                    }
                    else
                    {
                        if (values.TryGetValue(key, out timeValue)) remove(key, timeValue.Value);
                        values.Add(key, new value { Timeout = timeout, Value = value });
                    }
                }
                finally { Monitor.Exit(dictionaryLock); }
                if (isRefresh == 0) timerTask.Default.Add(refresh, timeout);
            }
        }
        /// <summary>
        /// 超时字典
        /// </summary>
        /// <param name="timeoutSeconds">超时秒数</param>
        public timeoutDictionary(int timeoutSeconds)
        {
            timeoutTicks = new TimeSpan(0, 0, timeoutSeconds <= 0 ? 1 : timeoutSeconds).Ticks;
            refresh = refreshTimeout;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            refresh = null;
            Monitor.Enter(dictionaryLock);
            try
            {
                if (OnRemovedLock == null) this.values.Clear();
                else
                {
                    foreach (KeyValuePair<keyType, value> values in this.values.getArray()) remove(values.Key, values.Value.Value);
                }
            }
            finally { Monitor.Exit(dictionaryLock); }
        }
        /// <summary>
        /// 超时检测
        /// </summary>
        private void refreshTimeout()
        {
            DateTime time = date.NowSecond;
            int count = 0, isRefresh = 1;
            Monitor.Enter(dictionaryLock);
            try
            {
                if (refreshValues.Count < this.values.Count)
                {
                    refreshValues.Empty();
                    refreshValues.AddLength(this.values.Count);
                }
                keyValue<keyType, valueType>[] refreshValueArray = refreshValues.UnsafeArray;
                foreach (KeyValuePair<keyType, value> values in this.values)
                {
                    if (time >= values.Value.Timeout) refreshValueArray[count++].Set(values.Key, values.Value.Value);
                }
                if (count != 0)
                {
                    foreach (keyValue<keyType, valueType> value in refreshValueArray)
                    {
                        remove(value.Key, value.Value);
                        if (--count == 0) break;
                    }
                }
                if (this.values.Count == 0) isRefresh = this.isRefresh = 0;
            }
            finally { Monitor.Exit(dictionaryLock); }
            if (isRefresh != 0) timerTask.Default.Add(refresh, date.NowSecond.AddTicks(timeoutTicks));
        }
        /// <summary>
        /// 判断是否存在键值
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>是否存在键值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool ContainsKey(keyType key)
        {
            return values.ContainsKey(key);
        }
        /// <summary>
        /// 根据关键字获取数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">数据</param>
        /// <returns>是否存在数据</returns>
        public bool TryGetValue(keyType key, ref valueType value)
        {
            DateTime now = date.NowSecond;
            value timeValue;
            Monitor.Enter(dictionaryLock);
            if (values.TryGetValue(key, out timeValue))
            {
                if (timeValue.Timeout > date.NowSecond)
                {
                    Monitor.Exit(dictionaryLock);
                    value = timeValue.Value;
                    return true;
                }
                else
                {
                    try
                    {
                        remove(key, timeValue.Value);
                    }
                    finally { Monitor.Exit(dictionaryLock); }
                }
            }
            else Monitor.Exit(dictionaryLock);
            return false;
        }
        /// <summary>
        /// 根据关键字获取数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="nullValue">失败返回默认空值</param>
        /// <returns>数据</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType Get(keyType key, valueType nullValue)
        {
            valueType value = default(valueType);
            return TryGetValue(key, ref value) ? value : default(valueType);
        }
        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>是否存在被移除数据</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool Remove(keyType key)
        {
            valueType value = default(valueType);
            return Remove(key, ref value);
        }
        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">被移除数据</param>
        /// <returns>是否存在被移除数据</returns>
        public bool Remove(keyType key, ref valueType value)
        {
            bool isRemove = false;
            value timeValue;
            Monitor.Enter(dictionaryLock);
            try
            {
                if (values.TryGetValue(key, out timeValue))
                {
                    isRemove = true;
                    remove(key, value = timeValue.Value);
                }
            }
            finally { Monitor.Exit(dictionaryLock); }
            return isRemove;
        }
        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">数据</param>
        private void remove(keyType key, valueType value)
        {
            values.Remove(key);
            if (OnRemovedLock != null) OnRemovedLock(key, value);
        }
        /// <summary>
        /// 根据关键字刷新过期时间
        /// </summary>
        /// <param name="key">关键字</param>
        public void RefreshTimeout(keyType key)
        {
            DateTime timeout = date.NowSecond.AddTicks(timeoutTicks);
            value value;
            Monitor.Enter(dictionaryLock);
            try
            {
                if (values.TryGetValue(key, out value))
                {
                    values[key] = new value { Timeout = timeout, Value = value.Value };
                }
            }
            finally { Monitor.Exit(dictionaryLock); }
        }
    }
}
