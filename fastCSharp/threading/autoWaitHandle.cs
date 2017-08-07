using System;
using System.Threading;
using System.Runtime.CompilerServices;

namespace fastCSharp.threading
{
    /// <summary>
    /// 一次性等待锁
    /// </summary>
    internal struct autoWaitHandle
    {
        /// <summary>
        /// 同步等待锁
        /// </summary>
        private object waitLock;
        /// <summary>
        /// 是否等待中
        /// </summary>
        private int isWait;
        /// <summary>
        /// 一次性等待锁
        /// </summary>
        /// <param name="isSet">是否默认结束等待</param>
        public autoWaitHandle(bool isSet)
        {
            waitLock = new object();
            isWait = isSet ? 1 : 0;
        }
        /// <summary>
        /// 等待结束
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Wait()
        {
            Monitor.Enter(waitLock);
            if (isWait == 0)
            {
                isWait = 1;
                Monitor.Wait(waitLock);
            }
            isWait = 0;
            Monitor.Exit(waitLock);
        }
        /// <summary>
        /// 结束等待
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Set()
        {
            Monitor.Enter(waitLock);
            if (isWait == 0) isWait = 1;
            else Monitor.Pulse(waitLock);
            Monitor.Exit(waitLock);
        }
    }
}
