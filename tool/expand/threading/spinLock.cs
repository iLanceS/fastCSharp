using System;
using System.Threading;
using System.Runtime.CompilerServices;

namespace fastCSharp.threading
{
    /// <summary>
    /// 1读1写自旋锁(必须手动初始化)
    /// </summary>
    public struct spinLock
    {
        /// <summary>
        /// 左旋数
        /// </summary>
        private volatile int left;
        /// <summary>
        /// 右旋数
        /// </summary>
        private volatile int right;
        /// <summary>
        /// 进入左旋锁(循环等待周期切换睡眠周期)
        /// </summary>
        public void EnterLeftSleep()
        {
            int sleep = 0;
            do
            {
                if (++left == 0)
                {
                    if (++right == 0) break;
                    left = -1;
                }
                Thread.Sleep(sleep);
                sleep ^= 1;
            }
            while (true);
        }
        /// <summary>
        /// 进入左旋锁
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void EnterLeft()
        {
            do
            {
                if (++left == 0)
                {
                    if (++right == 0) break;
                    left = -1;
                }
                Thread.Yield();
                if (++left == 0)
                {
                    if (++right == 0) break;
                    left = -1;
                }
                Thread.Sleep(0);
            }
            while (true);
        }
        /// <summary>
        /// 进入右旋锁(循环等待周期切换睡眠周期)
        /// </summary>
        public void EnterRightSleep()
        {
            int sleep = 0;
            do
            {
                if (++right == 0)
                {
                    if (++left == 0) break;
                    right = -1;
                }
                Thread.Sleep(sleep);
                sleep ^= 1;
            }
            while (true);
        }
        /// <summary>
        /// 进入右旋锁
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void EnterRight()
        {
            do
            {
                if (++right == 0)
                {
                    if (++left == 0) break;
                    right = -1;
                }
                Thread.Yield();
                if (++right == 0)
                {
                    if (++left == 0) break;
                    right = -1;
                }
                Thread.Sleep(0);
            }
            while (true);
        }
        /// <summary>
        /// 锁复位
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Exit()
        {
            left = right = -1;
        }
    }
}
