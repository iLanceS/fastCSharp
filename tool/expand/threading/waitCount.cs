using System;
using System.Threading;
using System.Runtime.CompilerServices;

namespace fastCSharp.threading
{
    /// <summary>
    /// 计数等待
    /// </summary>
    public sealed class waitCount : IDisposable
    {
        /// <summary>
        /// 当前计数
        /// </summary>
        private int count;
        /// <summary>
        /// 等待计数
        /// </summary>
        private int wait;
        /// <summary>
        /// 等待事件
        /// </summary>
        private readonly EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        /// <summary>
        /// 计数等待
        /// </summary>
        /// <param name="count">当前计数</param>
        public waitCount(int count = 0)
        {
            this.count = count + 1;
            wait = 1;
        }
        /// <summary>
        /// 重置计数等待
        /// </summary>
        /// <param name="count">当前计数</param>
        public void Reset(int count)
        {
            waitHandle.Reset();
            this.count = count + 1;
            wait = 1;
        }
        /// <summary>
        /// 增加计数
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Increment()
        {
            Interlocked.Increment(ref count);
        }
        /// <summary>
        /// 减少计数
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Decrement()
        {
            if (Interlocked.Decrement(ref count) == 0) waitHandle.Set();
        }
        /// <summary>
        /// 等待计数完成
        /// </summary>
        /// <returns>当前未完成计数,0表示正常结束</returns>
        public int Wait()
        {
            if (Interlocked.Decrement(ref wait) == 0) Interlocked.Decrement(ref count);
            if (count != 0) waitHandle.WaitOne();
            return count;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            waitHandle.Set();
            waitHandle.Close();
        }
    }
}
