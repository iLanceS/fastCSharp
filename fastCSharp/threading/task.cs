using System;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.CompilerServices;

namespace fastCSharp.threading
{
    /// <summary>
    /// 任务处理基类
    /// </summary>
    public abstract class taskBase : IDisposable
    {
        /// <summary>
        /// 线程池
        /// </summary>
        protected threadPool threadPool;
        /// <summary>
        /// 新任务集合数量
        /// </summary>
        protected abstract int pushTaskCount { get; }
        /// <summary>
        /// 等待空闲事件
        /// </summary>
        protected waitHandle freeWaitHandle = new waitHandle(true);
        /// <summary>
        /// 任务访问锁
        /// </summary>
        protected readonly object taskLock = new object();
        /// <summary>
        /// 线程数量
        /// </summary>
        protected int threadCount;
        /// <summary>
        /// 默认释放资源是否等待线程结束
        /// </summary>
        protected bool isDisposeWait;
        /// <summary>
        /// 是否停止任务
        /// </summary>
        protected byte isStop;
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            fastCSharp.domainUnload.Remove(this, domainUnload.unloadType.TaskDispose, false);
            Dispose(isDisposeWait);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="isWait">是否等待线程结束</param>
        public void Dispose(bool isWait)
        {
            Monitor.Enter(taskLock);
            int threadCount = this.threadCount | pushTaskCount;
            isStop = 1;
            Monitor.Exit(taskLock);
            if (isWait && threadCount != 0) freeWaitHandle.Wait();
        }
        /// <summary>
        /// 单线程添加任务后，等待所有线程空闲
        /// </summary>
        public void WaitFree()
        {
            Monitor.Enter(taskLock);
            int threadCount = this.threadCount | pushTaskCount;
            Monitor.Exit(taskLock);
            if (threadCount != 0) freeWaitHandle.Wait();
        }
    }
    /// <summary>
    /// 任务处理类(适用于短小任务，因为处理阻塞)
    /// </summary>
    public sealed class task : taskBase
    {
        /// <summary>
        /// 新任务集合
        /// </summary>
        private collection<taskInfo> pushTasks = new collection<taskInfo>();
        /// <summary>
        /// 新任务集合数量
        /// </summary>
        protected override int pushTaskCount
        {
            get { return pushTasks.Count; }
        }
        /// <summary>
        /// 最大线程数量
        /// </summary>
        public int MaxThreadCount { get; private set; }
        /// <summary>
        /// 任务处理
        /// </summary>
        /// <param name="count">线程数</param>
        /// <param name="isDisposeWait">默认释放资源是否等待线程结束</param>
        /// <param name="threadPool">线程池</param>
        public task(int count, bool isDisposeWait = true, threadPool threadPool = null)
        {
            if (count <= 0 || count > config.pub.Default.TaskMaxThreadCount) fastCSharp.log.Error.Throw(log.exceptionType.IndexOutOfRange);
            MaxThreadCount = count;
            this.isDisposeWait = isDisposeWait;
            this.threadPool = threadPool ?? fastCSharp.threading.threadPool.TinyPool;
            fastCSharp.domainUnload.Add(this, domainUnload.unloadType.TaskDispose, false);
        }
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="task">任务信息</param>
        /// <returns>任务添加是否成功</returns>
        internal bool Add(taskInfo task)
        {
            Monitor.Enter(taskLock);
            if (isStop == 0)
            {
                try
                {
                    pushTasks.Add(task);
                    if (this.threadCount == MaxThreadCount) return true;
                    if (this.threadCount++ == 0) freeWaitHandle.Reset();
                }
                finally { Monitor.Exit(taskLock); }
                try
                {
                    threadPool.FastStart(this, thread.callType.TaskRun);
                }
                catch (Exception error)
                {
                    Monitor.Enter(taskLock);
                    int count = --this.threadCount | pushTasks.Count;
                    Monitor.Exit(taskLock);
                    if (count == 0) freeWaitHandle.Set();
                    log.Error.Add(error, null, false);
                }
                return true;
            }
            else Monitor.Exit(taskLock);
            return false;
        }
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="run">任务执行委托</param>
        /// <param name="type"></param>
        /// <returns>任务添加是否成功</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal bool Add(object run, thread.callType type)
        {
            return Add(new taskInfo { Call = run, Type = type });
        }
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="run">任务执行委托</param>
        /// <param name="onError">任务执行出错委托,停止任务参数null</param>
        /// <returns>任务添加是否成功</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool Add(Action run, Action<Exception> onError = null)
        {
            return run != null && Add(new taskInfo { Call = run, Type = thread.callType.Action, OnError = onError });
        }
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <typeparam name="parameterType">执行参数类型</typeparam>
        /// <param name="run">任务执行委托</param>
        /// <param name="parameter">执行参数</param>
        /// <param name="onError">任务执行出错委托,停止任务参数null</param>
        /// <returns>任务添加是否成功</returns>
        public bool Add<parameterType>(Action<parameterType> run, ref parameterType parameter, Action<Exception> onError = null)
        {
            if (run != null)
            {
                run<parameterType> action = run<parameterType>.Pop();
                action.Set(run, ref parameter);
                return Add(new taskInfo { Call = action, Type = thread.callType.Run, OnError = onError });
            }
            return false;
        }
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <typeparam name="parameterType">执行参数类型</typeparam>
        /// <param name="run">任务执行委托</param>
        /// <param name="parameter">执行参数</param>
        /// <param name="onError">任务执行出错委托,停止任务参数null</param>
        /// <returns>任务添加是否成功</returns>
        public bool Add<parameterType>(Action<parameterType> run, parameterType parameter, Action<Exception> onError = null)
        {
            if (run != null)
            {
                run<parameterType> action = run<parameterType>.Pop();
                action.Set(run, parameter);
                return Add(new taskInfo { Call = action, Type = thread.callType.Run, OnError = onError });
            }
            return false;
        }
        ///// <summary>
        ///// 添加任务
        ///// </summary>
        ///// <typeparam name="parameterType">执行参数类型</typeparam>
        ///// <param name="run">任务执行委托</param>
        ///// <param name="parameter">执行参数</param>
        ///// <param name="onError">任务执行出错委托,停止任务参数null</param>
        ///// <returns>任务添加是否成功</returns>
        //public bool Add<parameterType>(pushPool<parameterType> run, ref parameterType parameter, Action<Exception> onError = null)
        //{
        //    if (run != null)
        //    {
        //        runPushPool<parameterType> action = runPushPool<parameterType>.Pop();
        //        action.Set(run, ref parameter);
        //        return Add(new taskInfo { Call = action, Type = thread.callType.Run, OnError = onError });
        //    }
        //    return false;
        //}
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="socketCall"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool Add(fastCSharp.net.tcp.commandServer.socketCall socketCall)
        {
            return socketCall != null && Add(new taskInfo { Call = socketCall, Type = thread.callType.TcpCommandServerSocketCall });
        }
        /// <summary>
        /// 执行任务
        /// </summary>
        internal void Run()
        {
            do
            {
                Monitor.Enter(taskLock);
                if (pushTasks.Count == 0)
                {
                    int threadCount = --this.threadCount;
                    Monitor.Exit(taskLock);
                    if (threadCount == 0) freeWaitHandle.Set();
                    break;
                }
                taskInfo task = pushTasks.UnsafePopExpandReset();
                Monitor.Exit(taskLock);
                task.Run();
            }
            while (true);
        }
        /// <summary>
        /// 微型线程任务
        /// </summary>
        public static readonly task Tiny = new task(config.pub.Default.TinyThreadCount);
        /// <summary>
        /// 默认任务
        /// </summary>
        public static readonly task Default = new task(config.pub.Default.TaskThreadCount, true, threadPool.Default);
        static task()
        {
            if (fastCSharp.config.appSetting.IsCheckMemory) checkMemory.Add(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }
    }
}
