using System;
using System.Threading;
using fastCSharp.threading;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql.cache.whole
{
    /// <summary>
    /// 缓存时间事件
    /// </summary>
    /// <typeparam name="valueType"></typeparam>
    /// <typeparam name="modelType"></typeparam>
    public class timer<valueType, modelType>
        where valueType : class, modelType
        where modelType : class
    {
        /// <summary>
        /// 整表缓存
        /// </summary>
        protected events.cache<valueType, modelType> cache;
        /// <summary>
        /// 时间获取器
        /// </summary>
        protected Func<valueType, DateTime> getTime;
        /// <summary>
        /// 事件委托
        /// </summary>
        private Action runTimeHandle;
        /// <summary>
        /// 事件委托
        /// </summary>
        private Action run;
        /// <summary>
        /// 最小事件时间
        /// </summary>
        private DateTime minTime;
        /// <summary>
        /// 事件时间集合
        /// </summary>
        private subArray<DateTime> times;
        /// <summary>
        /// 事件时间访问锁
        /// </summary>
        private readonly object timeLock = new object();
        /// <summary>
        /// 缓存时间事件
        /// </summary>
        /// <param name="cache">整表缓存</param>
        /// <param name="getTime">时间获取器</param>
        /// <param name="run">事件委托</param>
        /// <param name="isReset">是否绑定事件与重置数据</param>
        public timer(events.cache<valueType, modelType> cache, Func<valueType, DateTime> getTime, Action run, bool isReset)
        {
            if (cache == null || getTime == null || run == null) log.Error.Throw(log.exceptionType.Null);
            runTimeHandle = runTime;
            this.cache = cache;
            this.getTime = getTime;
            this.run = run;
            minTime = DateTime.MaxValue;

            if (isReset)
            {
                cache.OnReset += reset;
                cache.OnInserted += onInserted;
                cache.OnUpdated += onUpdated;
                resetLock();
            }
        }
        /// <summary>
        /// 重新加载数据
        /// </summary>
        protected void resetLock()
        {
            Monitor.Enter(cache.SqlTool.Lock);
            try
            {
                reset();
            }
            finally { Monitor.Exit(cache.SqlTool.Lock); }
        }
        /// <summary>
        /// 重新加载数据
        /// </summary>
        protected virtual void reset()
        {
            DateTime minTime = DateTime.MaxValue;
            foreach (valueType value in cache.Values)
            {
                DateTime time = getTime(value);
                if (time < minTime && time > fastCSharp.pub.MinTime) minTime = time;
            }
            Append(minTime);
        }
        /// <summary>
        /// 添加事件时间
        /// </summary>
        /// <param name="time"></param>
        public void Append(DateTime time)
        {
            if (time < minTime && time > fastCSharp.pub.MinTime)
            {
                Monitor.Enter(timeLock);
                if (time < minTime)
                {
                    try
                    {
                        times.Add(time);
                        minTime = time;
                    }
                    finally { Monitor.Exit(timeLock); }
                    if (time <= date.NowSecond) fastCSharp.threading.threadPool.TinyPool.Start(runTimeHandle);
                    else fastCSharp.threading.timerTask.Default.Add(runTimeHandle, time);
                }
                else Monitor.Exit(timeLock);
            }
        }
        /// <summary>
        /// 时间事件
        /// </summary>
        private unsafe void runTime()
        {
            DateTime now = date.Now;
            Monitor.Enter(timeLock);
            if (times.Count != 0)
            {
                fixed (DateTime* timeFixed = times.UnsafeArray)
                {
                    if (*timeFixed <= now)
                    {
                        times.Empty();
                        minTime = DateTime.MaxValue;
                    }
                    else
                    {
                        DateTime* end = timeFixed + times.Count;
                        while (*--end <= now) ;
                        minTime = *end;
                        times.UnsafeSetLength((int)(end - timeFixed) + 1);
                    }
                }
            }
            Monitor.Exit(timeLock);
            run();
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void onInserted(valueType value)
        {
            Append(getTime(value));
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="value">更新后的数据</param>
        /// <param name="oldValue">更新前的数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void onUpdated(valueType cacheValue, valueType value, valueType oldValue, fastCSharp.code.memberMap<modelType> memberMap)
        {
            Append(getTime(value));
        }
    }
}
