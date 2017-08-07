using System;
using System.Threading;
using System.Runtime.CompilerServices;

namespace fastCSharp.threading
{
    /// <summary>
    /// 同步等待锁
    /// </summary>
    public struct waitHandle
    {
        /// <summary>
        /// 同步等待锁
        /// </summary>
        private object waitLock;
        /// <summary>
        /// 是否等待中
        /// </summary>
        private int isSet;
        /// <summary>
        /// 同步等待锁
        /// </summary>
        /// <param name="isSet">是否默认结束等待</param>
        public waitHandle(bool isSet)
        {
            waitLock = new object();
            this.isSet = isSet ? 1 : 0;
        }
        /// <summary>
        /// 等待结束
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Wait()
        {
            if (isSet == 0)
            {
                Monitor.Enter(waitLock);
                while (isSet == 0) Monitor.Wait(waitLock);
                Monitor.Pulse(waitLock);
                Monitor.Exit(waitLock);
            }
        }
        /// <summary>
        /// 重置等待
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Reset()
        {
            Monitor.Enter(waitLock);
            if (isSet != 0) isSet = 0;
            Monitor.Exit(waitLock);
        }
        /// <summary>
        /// 结束等待
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Set()
        {
            Monitor.Enter(waitLock);
            if (isSet == 0)
            {
                isSet = 1;
                Monitor.Pulse(waitLock);
            }
            Monitor.Exit(waitLock);
        }
    }
}