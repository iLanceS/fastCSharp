using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace fastCSharp.threading
{
    /// <summary>
    /// 时间段数量统计
    /// </summary>
    public struct secondCount
    {
        /// <summary>
        /// 下一个时钟周期
        /// </summary>
        private long nextTicks;
        /// <summary>
        /// 时间段计时周期数
        /// </summary>
        private long ticks;
        /// <summary>
        /// 最大时钟周期
        /// </summary>
        private long maxTicks;
        /// <summary>
        /// 最小历史纪录时钟周期
        /// </summary>
        private long minTicks;
        /// <summary>
        /// 数量统计
        /// </summary>
        private int[] counts;
        /// <summary>
        /// 数量统计
        /// </summary>
        public int[] Counts { get { return counts; } }
        /// <summary>
        /// 数量访问锁
        /// </summary>
        private readonly object countLock;
        /// <summary>
        /// 当前数量索引
        /// </summary>
        private int currentIndex;
        /// <summary>
        /// 统计秒数
        /// </summary>
        /// <param name="count">时间段数量</param>
        /// <param name="seconds">时间段秒数</param>
        /// <param name="isHistory">是否准备加载历史纪录</param>
        public secondCount(int count, int seconds = 1, bool isHistory = false)
        {
            counts = count <= 0 ? nullValue<int>.Array : new int[count];
            countLock = counts.Length == 0 ? null : new object();
            ticks = Math.Max(seconds, 1) * date.SecondTicks;
            maxTicks = (long)count * ticks;
            if (isHistory)
            {
                minTicks = date.nowTime.Now.Ticks - maxTicks;
                nextTicks = minTicks + ticks;
            }
            else
            {
                nextTicks = date.nowTime.Now.Ticks + ticks;
                minTicks = 0;
            }
            currentIndex = 0;
        }
        /// <summary>
        /// 添加数量
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Add()
        {
            if (counts.Length != 0 && Monitor.TryEnter(countLock))
            {
                add(date.nowTime.Now.Ticks);
                Monitor.Exit(countLock);
            }
        }
        /// <summary>
        /// 添加历史纪录数量
        /// </summary>
        /// <param name="time"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void AddHistory(DateTime time)
        {
            AddHistory(time.Ticks);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ticks"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void AddHistory(long ticks)
        {
            if (ticks > minTicks) add(ticks);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentTick"></param>
        private void add(long currentTick)
        {
            if ((currentTick -= nextTicks) < 0) ++counts[currentIndex];
            else if (currentTick >= maxTicks)
            {
                Array.Clear(counts, 0, counts.Length);
                nextTicks = date.nowTime.Now.Ticks + ticks;
                currentIndex = 0;
                counts[0] = 1;
            }
            else
            {
                do
                {
                    if (++currentIndex == counts.Length) currentIndex = 0;
                    nextTicks += ticks;
                    counts[currentIndex] = 0;
                }
                while ((currentTick -= ticks) >= 0);
                counts[currentIndex] = 1;
            }
        }
    }
}
