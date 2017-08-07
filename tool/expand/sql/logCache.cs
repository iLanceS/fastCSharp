using System;
using fastCSharp.net.tcp;
using fastCSharp.threading;
using fastCSharp.net;
using System.Threading;

namespace fastCSharp.sql
{
    /// <summary>
    /// 日志缓存
    /// </summary>
    public abstract class logCache
    {
        /// <summary>
        /// 日志流处理保持回调
        /// </summary>
        protected commandClient.streamCommandSocket.keepCallback keepCallback;
        /// <summary>
        /// 日志标识
        /// </summary>
        protected long logIdentity;
        /// <summary>
        /// 日志是否有效
        /// </summary>
        protected bool isLog;
    }
    /// <summary>
    /// 日志缓存
    /// </summary>
    public sealed class logCache<valueType> : logCache
    {
        /// <summary>
        /// 缓存数据
        /// </summary>
        public struct value
        {
            /// <summary>
            /// 日志标识
            /// </summary>
            public long Identity;
            /// <summary>
            /// 缓存数据
            /// </summary>
            public valueType Value;
        }
        /// <summary>
        /// 获取日志数据委托
        /// </summary>
        private Func<Action<fastCSharp.net.returnValue<byte>>, commandClient.streamCommandSocket.keepCallback> getLog;
        /// <summary>
        /// 数据集合
        /// </summary>
        private valueType cache;
        /// <summary>
        /// 访问锁
        /// </summary>
        private int logLock;
        /// <summary>
        /// 是否存在缓存数据
        /// </summary>
        private bool isValue;
        /// <summary>
        /// 日志缓存
        /// </summary>
        /// <param name="getLog">获取日志数据委托</param>
        public logCache(Func<Action<fastCSharp.net.returnValue<byte>>, commandClient.streamCommandSocket.keepCallback> getLog)
        {
            if (getLog == null) log.Error.Throw(log.exceptionType.ErrorOperation);
            this.getLog = getLog;
            load();
        }
        /// <summary>
        /// 加载数据
        /// </summary>
        private void load()
        {
            keepCallback = getLog(onLog);
            //if (keepCallback == null) fastCSharp.threading.timerTask.Default.Add(load, date.NowSecond.AddSeconds(5));
            //else
            if (keepCallback != null)
            {
                interlocked.CompareSetYield(ref logLock);
                ++logIdentity;
                isValue = false;
                isLog = true;
                logLock = 0;
            }
        }
        /// <summary>
        /// 日志处理
        /// </summary>
        /// <param name="log"></param>
        private void onLog(fastCSharp.net.returnValue<byte> log)
        {
            if (log.Type == fastCSharp.net.returnValue.type.Success)
            {
                interlocked.CompareSetYield(ref logLock);
                ++logIdentity;
                isValue = false;
                logLock = 0;
                return;
            }
            pub.Dispose(ref keepCallback);
            isLog = false;
            fastCSharp.threading.timerTask.Default.Add(load, date.NowSecond.AddSeconds(2));
        }
        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public value Value
        {
            get
            {
                if (isLog)
                {
                    return isValue ? new value { Value = cache } : new value { Identity = logIdentity };
                }
                return default(value);
            }
        }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="value"></param>
        public void Set(value value)
        {
            Set(ref value);
        }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="value"></param>
        public void Set(ref value value)
        {
            if (isLog && value.Identity == logIdentity)
            {
                interlocked.CompareSetYield(ref logLock);
                if (value.Identity == logIdentity)
                {
                    cache = value.Value;
                    isValue = true;
                }
                logLock = 0;
            }
        }
    }
    /// <summary>
    /// 日志缓存
    /// </summary>
    public abstract class logCache<keyType, valueType> : logCache
        where keyType : IEquatable<keyType>
    {
        /// <summary>
        /// 获取日志数据委托
        /// </summary>
        protected Func<Action<fastCSharp.net.returnValue<keyType>>, commandClient.streamCommandSocket.keepCallback> getLog;
        /// <summary>
        /// 日志缓存
        /// </summary>
        /// <param name="getLog">获取日志数据委托</param>
        protected logCache(Func<Action<fastCSharp.net.returnValue<keyType>>, commandClient.streamCommandSocket.keepCallback> getLog)
        {
            if (getLog == null) log.Error.Throw(log.exceptionType.ErrorOperation);
            this.getLog = getLog;
        }
        /// <summary>
        /// 小容量数组缓存
        /// </summary>
        public sealed class array : logCache<keyType, valueType>
        {
            /// <summary>
            /// 缓存数据
            /// </summary>
            public struct value
            {
                /// <summary>
                /// 日志标识
                /// </summary>
                public long Identity;
                /// <summary>
                /// 关键字
                /// </summary>
                public keyType Key;
                /// <summary>
                /// 缓存数据
                /// </summary>
                public valueType Value;
            }
            /// <summary>
            /// 数据集合
            /// </summary>
            private keyValue<keyType, valueType>[] values;
            /// <summary>
            /// 数据数量
            /// </summary>
            private int count;
            /// <summary>
            /// 访问锁
            /// </summary>
            private readonly object logLock = new object();
            /// <summary>
            /// 小容量数组缓存
            /// </summary>
            /// <param name="getLog">获取日志数据委托</param>
            /// <param name="count">数据数量</param>
            public array(Func<Action<fastCSharp.net.returnValue<keyType>>, commandClient.streamCommandSocket.keepCallback> getLog, int count)
                : base(getLog)
            {
                values = new keyValue<keyType, valueType>[Math.Max(count, 2)];
                load();
            }
            /// <summary>
            /// 加载数据
            /// </summary>
            private void load()
            {
                keepCallback = getLog(onLog);
                //if (keepCallback == null) fastCSharp.threading.timerTask.Default.Add(load, date.NowSecond.AddSeconds(5));
                //else
                if (keepCallback != null)
                {
                    Monitor.Enter(logLock);
                    ++logIdentity;
                    count = 0;
                    isLog = true;
                    Monitor.Exit(logLock);
                }
            }
            /// <summary>
            /// 日志处理
            /// </summary>
            /// <param name="log"></param>
            private void onLog(fastCSharp.net.returnValue<keyType> log)
            {
                if (log.Type == fastCSharp.net.returnValue.type.Success)
                {
                    Monitor.Enter(logLock);
                    ++logIdentity;
                    if (count != 0)
                    {
                        int index = 0;
                        foreach (keyValue<keyType, valueType> keyValue in values)
                        {
                            if (keyValue.Key.Equals(log.Value))
                            {
                                values[index] = values[--count];
                                Monitor.Exit(logLock);
                                return;
                            }
                            if (++index == count) break;
                        }
                    }
                    Monitor.Exit(logLock);
                    return;
                }
                pub.Dispose(ref keepCallback);
                isLog = false;
                fastCSharp.threading.timerTask.Default.Add(load, date.NowSecond.AddSeconds(2));
            }
            /// <summary>
            /// 获取缓存数据
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public value Get(keyType key)
            {
                if (isLog)
                {
                    if (count != 0)
                    {
                        int index = 0;
                        foreach (keyValue<keyType, valueType> keyValue in values)
                        {
                            if (keyValue.Key.Equals(key))
                            {
                                if (index != 0)
                                {
                                    Monitor.Enter(logLock);
                                    if (values[index].Key.Equals(key))
                                    {
                                        values[index] = values[0];
                                        values[0] = keyValue;
                                    }
                                    Monitor.Exit(logLock);
                                }
                                return new value { Key = keyValue.Key, Value = keyValue.Value };
                            }
                            if (++index == count) break;
                        }
                    }
                    return new value { Identity = logIdentity, Key  = key };
                }
                return default(value);
            }
            /// <summary>
            /// 设置缓存数据
            /// </summary>
            /// <param name="value"></param>
            public void Set(value value)
            {
                Set(ref value);
            }
            /// <summary>
            /// 设置缓存数据
            /// </summary>
            /// <param name="value"></param>
            public void Set(ref value value)
            {
                if (isLog && value.Identity == logIdentity)
                {
                    int randomIndex = count == values.Length ? fastCSharp.random.Default.Next(count) : 0;
                    Monitor.Enter(logLock);
                    if (value.Identity == logIdentity)
                    {
                        if (count == 0)
                        {
                            values[0].Set(value.Key, value.Value);
                            count = 1;
                        }
                        else
                        {
                            int index = 0;
                            foreach (keyValue<keyType, valueType> keyValue in values)
                            {
                                if (keyValue.Key.Equals(value.Key))
                                {
                                    if (index == 0) values[0].Value = value.Value;
                                    else
                                    {
                                        values[index] = values[0];
                                        values[0].Set(value.Key, value.Value);
                                    }
                                    break;
                                }
                                if (++index == count)
                                {
                                    if (count == values.Length) values[randomIndex].Set(value.Key, value.Value);
                                    else
                                    {
                                        values[index].Set(value.Key, value.Value);
                                        ++count;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    Monitor.Exit(logLock);
                }
            }
        }
    }
}
