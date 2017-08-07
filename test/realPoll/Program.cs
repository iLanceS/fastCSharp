using System;
using System.Runtime.InteropServices;
using System.Threading;
using fastCSharp;

namespace fastCSharp.test.realPoll
{
    class Program
    {
        /// <summary>
        /// 计数器
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        internal static extern bool QueryPerformanceCounter(out long value);
        /// <summary>
        /// windows高精度实时轮询测试
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            long millisecondTicks = date.MillisecondTicks, maxTicks = 0, ticks100 = millisecondTicks / 100, ticks10 = millisecondTicks / 10, ticks, count = 0;
            int count100 = 0, count10 = 0, count1 = 0;
            Console.WriteLine("MillisecondTicks " + millisecondTicks.toString());
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            QueryPerformanceCounter(out ticks);
            ticks += millisecondTicks;
            do
            {
                ++count;
                do
                {
                    long currentTicks;
                    QueryPerformanceCounter(out currentTicks);
                    long value = currentTicks - ticks;
                    if (value >= 0)
                    {
                        if (value > ticks100)
                        {
                            ++count100;
                            if (value > ticks10)
                            {
                                ++count10;
                                if (value > millisecondTicks) ++count1;
                            }
                            Console.WriteLine("测试总数[" + count.toString() + "] 0.01ms 失败次数[" + count100.toString() + "] 0.1ms 失败次数[" + count10.toString() + "] 1ms 失败次数[" + count10.toString() + "]");
                        }
                        if (value > maxTicks)
                        {
                            maxTicks = value;
                            Console.WriteLine("最大误差周期值[" + maxTicks.toString() + "]");
                        }
                        break;
                    }
                }
                while (true);
                ticks += millisecondTicks;
            }
            while (true);
        }
    }
}
