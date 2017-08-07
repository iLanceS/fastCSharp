using System;
using System.Collections.Generic;
using fastCSharp.threading;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql
{
    /// <summary>
    /// 当前时间
    /// </summary>
    public sealed class nowTime
    {
        /// <summary>
        /// 增加
        /// </summary>
        private static readonly long ticks = 4 * date.MillisecondTicks;
        /// <summary>
        /// 下一次最小时间
        /// </summary>
        private DateTime minTime;
        /// <summary>
        /// 时间访问锁
        /// </summary>
        private int timeLock;
        /// <summary>
        /// 获取下一个时间
        /// </summary>
        public DateTime Next
        {
            get
            {
                DateTime now = date.NowSecond;
                interlocked.CompareSetYield(ref timeLock);
                if (now < minTime) now = minTime;
                minTime = now.AddTicks(ticks);
                timeLock = 0;
                return now;
            }
        }
        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="time"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Set(DateTime time)
        {
            interlocked.CompareSetYield(ref timeLock);
            minTime = time.AddTicks(ticks);
            timeLock = 0;
        }
        /// <summary>
        /// 初始化最大时间
        /// </summary>
        private DateTime maxTime = fastCSharp.date.NowSecond;
        /// <summary>
        /// 在初始化循环中设置最大时间
        /// </summary>
        /// <param name="time"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void SetMaxTime(DateTime time)
        {
            if (time > maxTime) maxTime = time;
        }
        /// <summary>
        /// 在初始化循环中设置最大时间
        /// </summary>
        /// <param name="time"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void SetMaxTime(DateTime? time)
        {
            if (time != null) SetMaxTime((DateTime)time);
        }
        /// <summary>
        /// 在初始化循环结束后确认最大时间
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void SetMaxTime()
        {
            Set(maxTime);
        }
        /// <summary>
        /// 设置时间
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="values"></param>
        /// <param name="getTime"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Set<valueType>(IEnumerable<valueType> values, Func<valueType, DateTime> getTime)
        {
            foreach (valueType value in values) SetMaxTime(getTime(value));
            Set(maxTime);
        }
    }
}
