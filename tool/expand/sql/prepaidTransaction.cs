using System;
using System.Runtime.CompilerServices;
using System.Threading;
using fastCSharp.reflection;

namespace fastCSharp.sql
{
    /// <summary>
    /// 预付款事务
    /// </summary>
    public abstract class prepaidTransaction : IDisposable
    {
        /// <summary>
        /// 预付款事务访问锁
        /// </summary>
        private readonly object transactionLock;
        /// <summary>
        /// 是否处于预付款状态
        /// </summary>
        private int isPrepaid;
        /// <summary>
        /// 预付款事务
        /// </summary>
        /// <param name="transactionLock">预付款事务锁</param>
        protected prepaidTransaction(object transactionLock)
        {
            if (transactionLock == null) fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.Null);
            this.transactionLock = transactionLock;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref isPrepaid, 0, 1) != 0) onDisposeError();
        }
        /// <summary>
        /// 预付款事务开始
        /// </summary>
        public void Start()
        {
            byte isRefund = 0;
            Monitor.Enter(transactionLock);
            try
            {
                if (prepaid())
                {
                    isPrepaid = 1;
                    pay();
                }
            }
            finally
            {
                if (Interlocked.CompareExchange(ref isPrepaid, 0, 1) != 0)
                {
                    isRefund = 1;
                    refund();
                }
                Monitor.Exit(transactionLock);
                if (isRefund != 0) onRefund();
            }
        }
        /// <summary>
        /// 释放资源时发现未完成付款错误处理
        /// </summary>
        protected virtual void onDisposeError()
        {
            fastCSharp.log.Error.Add(GetType().fullName() + " 未完成付款", new System.Diagnostics.StackFrame(), false);
        }
        /// <summary>
        /// 预付款处理
        /// </summary>
        /// <returns>是否继续操作</returns>
        protected abstract bool prepaid();
        /// <summary>
        /// 退回预付款处理(禁止抛出异常)
        /// </summary>
        protected abstract void refund();
        /// <summary>
        /// 退回预付款后处理
        /// </summary>
        protected virtual void onRefund() { }
        /// <summary>
        /// 付款操作
        /// </summary>
        protected abstract void pay();
        /// <summary>
        /// 付款完成操作
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void onPayCompleted()
        {
            if (Interlocked.CompareExchange(ref isPrepaid, 0, 1) == 0) onPayCompletedError();
        }
        /// <summary>
        /// 付款完成操作状态错误
        /// </summary>
        protected virtual void onPayCompletedError()
        {
            log.Error.Add(GetType().fullName() + " 已经付款完成，重复操作错误", new System.Diagnostics.StackFrame(), false);
        }
    }
}
