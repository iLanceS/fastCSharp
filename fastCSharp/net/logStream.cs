using System;
using fastCSharp.threading;
using System.Threading;

namespace fastCSharp.net
{
    /// <summary>
    /// 日志流
    /// </summary>
    public abstract class logStream
    {
        /// <summary>
        /// 回调调用线程
        /// </summary>
        internal abstract void Callback();
    }
    /// <summary>
    /// 日志流
    /// </summary>
    /// <typeparam name="valueType"></typeparam>
    public sealed class logStream<valueType> : logStream
    {
        /// <summary>
        /// 日志回调
        /// </summary>
        private struct callback
        {
            /// <summary>
            /// 日志编号
            /// </summary>
            public int Identity;
            /// <summary>
            /// 日志回调
            /// </summary>
            public Func<fastCSharp.net.returnValue<valueType>, bool> Callback;
            /// <summary>
            /// 日志回调
            /// </summary>
            /// <param name="identity"></param>
            /// <param name="callback"></param>
            public void Set(int identity, Func<fastCSharp.net.returnValue<valueType>, bool> callback)
            {
                Identity = identity;
                Callback = callback;
            }
        }
        /// <summary>
        /// 时钟周期标识
        /// </summary>
        public readonly long Ticks;
        /// <summary>
        /// 日志集合
        /// </summary>
        private valueType[] logs;
        /// <summary>
        /// 起始位置
        /// </summary>
        private int startIndex;
        /// <summary>
        /// 结束位置
        /// </summary>
        private int endIndex;
        /// <summary>
        /// 起始编号
        /// </summary>
        private int identity;
        /// <summary>
        /// 结束编号
        /// </summary>
        public int EndIdentity
        {
            get { return identity + (endIndex == int.MinValue ? logs.Length : endIndex); }
        }
        /// <summary>
        /// 日志集合访问锁
        /// </summary>
        internal int Lock;
        /// <summary>
        /// 日志回调集合
        /// </summary>
        private callback[] callbacks;
        /// <summary>
        /// 回调数量
        /// </summary>
        private int callbackCount;
        /// <summary>
        /// 日志回调集合访问锁
        /// </summary>
        private readonly object callbackLock = new object();
        /// <summary>
        /// 是否启动回调线程
        /// </summary>
        private bool isCallbackThread;
        /// <summary>
        /// 是否存在新的日志
        /// </summary>
        private bool isNewLog;
        /// <summary>
        /// 日志流
        /// </summary>
        /// <param name="size"></param>
        public logStream(int size)
        {
            if (size <= 0) log.Error.Throw(log.exceptionType.IndexOutOfRange);
            Ticks = fastCSharp.pub.StartTime.Ticks;
            callbacks = new callback[sizeof(int)];
            logs = new valueType[size];
        }
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="log"></param>
        public void Append(valueType log)
        {
            interlocked.CompareSetYield(ref Lock);
            if (endIndex == int.MinValue)
            {
                logs[startIndex] = log;
                ++identity;
                if (++startIndex == logs.Length) startIndex = 0;
            }
            else
            {
                logs[endIndex] = log;
                if (++endIndex == logs.Length) endIndex = int.MinValue;
            }
            Lock = 0;
            Monitor.Enter(callbackLock);
            if (callbackCount == 0) Monitor.Exit(callbackLock);
            else
            {
                bool isCallbackThread = this.isCallbackThread;
                this.isCallbackThread = isNewLog = true;
                Monitor.Exit(callbackLock);
                if (!isCallbackThread) fastCSharp.threading.threadPool.TinyPool.FastStart(this, thread.callType.LogStreamCallback);
            }
        }
        /// <summary>
        /// 回调调用线程
        /// </summary>
        internal override void Callback()
        {
            int callbackIndex = 0;
            Monitor.Enter(callbackLock);
        CHECK:
            if (isNewLog)
            {
                callbackIndex = 0;
                isNewLog = false;
            }
            if (callbackIndex == callbackCount)
            {
                isCallbackThread = false;
                Monitor.Exit(callbackLock);
                return;
            }
            callback callback = callbacks[callbackIndex];
            Monitor.Exit(callbackLock);

#if MONO
            uint isError = 0, isCallback = 0;
#else
            uint isError, isCallback = 0;
#endif
            do
            {
                valueType log;
                interlocked.CompareSetYield(ref Lock);
                int logIndex = callback.Identity - identity;
                if (endIndex == int.MinValue)
                {
                    if ((uint)logIndex >= (uint)logs.Length)
                    {
                        isError = (uint)logIndex ^ (uint)logs.Length;
                        Lock = 0;
                        break;
                    }
                    if ((logIndex += startIndex) >= logs.Length) logIndex -= logs.Length;
                }
                else if ((uint)logIndex >= (uint)endIndex)
                {
                    isError = (uint)logIndex ^ (uint)endIndex;
                    Lock = 0;
                    break;
                }
                log = logs[logIndex];
                Lock = 0;
                try
                {
                    if (callback.Callback(new fastCSharp.net.returnValue<valueType> { Type = fastCSharp.net.returnValue.type.Success, Value = log })) ++callback.Identity;
                    else
                    {
                        isError = isCallback = 1;
                        break;
                    }
                }
                catch
                {
                    isError = isCallback = 1;
                    break;
                }
            }
            while (true);

            if (isError == 0)
            {
                Monitor.Enter(callbackLock);
                callbacks[callbackIndex++].Identity = callback.Identity;
                goto CHECK;
            }
            Monitor.Enter(callbackLock);
            if (callbackIndex + 1 == callbackCount)
            {
                callbacks[callbackIndex].Callback = null;
                callbackCount = callbackIndex;
            }
            else
            {
                callbacks[callbackIndex] = callbacks[--callbackCount];
                callbacks[callbackCount].Callback = null;
            }
            if (isCallback == 0)
            {
                Monitor.Exit(callbackLock);
                try
                {
                    callback.Callback(new fastCSharp.net.returnValue<valueType> { Type = fastCSharp.net.returnValue.type.LogStreamExpired });
                }
                catch { }
                Monitor.Enter(callbackLock);
            }
            goto CHECK;
        }
        /// <summary>
        /// 获取日志
        /// </summary>
        /// <param name="ticks">时钟周期标识</param>
        /// <param name="identity">日志编号</param>
        /// <param name="callback"></param>
        public void Get(long ticks, int identity, Func<fastCSharp.net.returnValue<valueType>, bool> callback)
        {
            if (callback == null) log.Error.Throw(log.exceptionType.Null);
            if (ticks == this.Ticks) get(identity, callback);
            else callback(new fastCSharp.net.returnValue<valueType> { Type = fastCSharp.net.returnValue.type.LogStreamExpired });
        }
        /// <summary>
        /// 获取日志
        /// </summary>
        /// <param name="identity">日志编号</param>
        /// <param name="callback"></param>
        internal void Get(int identity, Func<fastCSharp.net.returnValue<valueType>, bool> callback)
        {
            if (callback == null) log.Error.Throw(log.exceptionType.Null);
            get(identity, callback);
        }
        /// <summary>
        /// 获取日志
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="callback"></param>
        private void get(int identity, Func<fastCSharp.net.returnValue<valueType>, bool> callback)
        {
            if ((uint)(identity - this.identity) <= (uint)(endIndex == int.MinValue ? logs.Length : endIndex))
            {
                bool isCallbackThread = true, isCallback;
                Monitor.Enter(callbackLock);
                if (callbackCount == callbacks.Length)
                {
                    isCallback = false;
                    try
                    {
                        callback[] newCallbacks = new callback[callbackCount << 1];
                        Array.Copy(callbacks, 0, newCallbacks, 0, callbackCount);
                        isCallbackThread = this.isCallbackThread;
                        newCallbacks[callbackCount].Set(identity, callback);
                        this.isCallbackThread = isCallback = true;
                        callbacks = newCallbacks;
                        ++callbackCount;
                    }
                    finally
                    {
                        Monitor.Exit(callbackLock);
                        if (isCallback)
                        {
                            if (!isCallbackThread) fastCSharp.threading.threadPool.TinyPool.FastStart(this, thread.callType.LogStreamCallback);
                        }
                        else callback(new fastCSharp.net.returnValue<valueType> { Type = fastCSharp.net.returnValue.type.LogStreamExpired });
                    }
                }
                else
                {
                    callbacks[callbackCount].Set(identity, callback);
                    isCallbackThread = this.isCallbackThread;
                    ++callbackCount;
                    this.isCallbackThread = true;
                    Monitor.Exit(callbackLock);
                    if (!isCallbackThread) fastCSharp.threading.threadPool.TinyPool.FastStart(this, thread.callType.LogStreamCallback);
                }
            }
            else callback(new fastCSharp.net.returnValue<valueType> { Type = fastCSharp.net.returnValue.type.LogStreamExpired });
        }
        /// <summary>
        /// 无验证获取日志
        /// </summary>
        /// <param name="callback"></param>
        public void Get(Func<fastCSharp.net.returnValue<valueType>, bool> callback)
        {
            get(EndIdentity, callback);
        }
    }
}
