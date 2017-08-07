using System;
using System.Threading;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace fastCSharp.threading
{
    /// <summary>
    /// 线程池
    /// </summary>
    public sealed class threadPool
    {
        /// <summary>
        /// 最低线程堆栈大小
        /// </summary>
        private const int minStackSize = 128 << 10;
        /// <summary>
        /// 线程堆栈大小
        /// </summary>
        private int stackSize;
        /// <summary>
        /// 线程集合
        /// </summary>
        private objectPool<thread> threads = objectPool<thread>.Create();
        /// <summary>
        /// 是否已经释放资源
        /// </summary>
        private bool isDisposed;
        /// <summary>
        /// 线程池
        /// </summary>
        /// <param name="stackSize">线程堆栈大小</param>
        private threadPool(int stackSize = 1 << 20)
        {
            this.stackSize = stackSize < minStackSize ? minStackSize : stackSize;
            fastCSharp.domainUnload.Add(this, domainUnload.unloadType.ThreadPoolDispose);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        internal void Dispose()
        {
            isDisposed = true;
            disposePool();
        }
        /// <summary>
        /// 释放线程池
        /// </summary>
        private void disposePool()
        {
            foreach (array.value<thread> value in threads.GetClear(0)) value.Value.Stop();
        }
        ///// <summary>
        ///// 获取一个线程并执行任务
        ///// </summary>
        ///// <param name="task">任务委托</param>
        ///// <param name="domainUnload">应用程序退出处理</param>
        ///// <param name="onError">应用程序退出处理</param>
        //[MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        //internal void FastStart(Action task, Action<Exception> onError, Action domainUnload)
        //{
        //    thread thread = threads.Pop();
        //    if (thread == null) new thread(this, stackSize, task, onError, domainUnload, threading.thread.callType.Action, threading.thread.errorType.Action, fastCSharp.domainUnload.unloadType.Action);
        //    else thread.RunTask(task, onError, domainUnload, threading.thread.callType.Action, threading.thread.errorType.Action, fastCSharp.domainUnload.unloadType.Action);
        //}
        /// <summary>
        /// 获取一个线程并执行任务
        /// </summary>
        /// <param name="task">任务委托</param>
        /// <param name="onError">应用程序退出处理</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void FastStart(Action task, Action<Exception> onError)
        {
            if (onError == null) FastStart(task);
            else
            {
                thread thread = threads.Pop();
                if (thread == null) new thread(this, stackSize, task, onError, threading.thread.callType.Action, threading.thread.errorType.Action);
                else thread.RunTask(task, onError, threading.thread.callType.Action, threading.thread.errorType.Action);
            }
        }
        /// <summary>
        /// 获取一个线程并执行任务
        /// </summary>
        /// <param name="task">任务委托</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void FastStart(Action task)
        {
            FastStart(task, threading.thread.callType.Action);
        }
        /// <summary>
        /// 获取一个线程并执行任务
        /// </summary>
        /// <param name="task">任务委托</param>
        /// <param name="taskType">任务委托调用类型</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void FastStart(object task, fastCSharp.threading.thread.callType taskType)
        {
            thread thread = threads.Pop();
            if (thread == null) new thread(this, stackSize, task, taskType);
            else thread.RunTask(task, taskType);
        }
        /// <summary>
        /// 获取一个线程并执行任务
        /// </summary>
        /// <param name="task">任务委托</param>
        /// <param name="onError">应用程序退出处理</param>
        /// <param name="taskType">任务委托调用类型</param>
        /// <param name="errorType">应用程序退出处理调用类型</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void FastStart(object task, object onError, fastCSharp.threading.thread.callType taskType, threading.thread.errorType errorType)
        {
            thread thread = threads.Pop();
            if (thread == null) new thread(this, stackSize, task, onError, taskType, errorType);
            else thread.RunTask(task, onError, taskType, errorType);
        }
        /// <summary>
        /// 获取一个线程并执行任务
        /// </summary>
        /// <typeparam name="parameterType">参数类型</typeparam>
        /// <param name="task">任务委托</param>
        /// <param name="parameter">线程参数</param>
        /// <param name="onError">应用程序退出处理</param>
        internal void FastStart<parameterType>(Action<parameterType> task, ref parameterType parameter, Action<Exception> onError)
        {
            run<parameterType> run = run<parameterType>.Pop();
            run.Set(task, ref parameter);
            if (onError == null) FastStart(run, fastCSharp.threading.thread.callType.Run);
            else FastStart(run, onError, fastCSharp.threading.thread.callType.Run, threading.thread.errorType.Action);
        }
        /// <summary>
        /// 获取一个线程并执行任务
        /// </summary>
        /// <typeparam name="parameterType">参数类型</typeparam>
        /// <param name="task">任务委托</param>
        /// <param name="parameter">线程参数</param>
        /// <param name="onError">应用程序退出处理</param>
        internal void FastStart<parameterType>(Action<parameterType> task, parameterType parameter, Action<Exception> onError)
        {
            run<parameterType> run = run<parameterType>.Pop();
            run.Set(task, parameter);
            if (onError == null) FastStart(run, fastCSharp.threading.thread.callType.Run);
            else FastStart(run, onError, fastCSharp.threading.thread.callType.Run, threading.thread.errorType.Action);
        }
        /// <summary>
        /// 获取一个线程并执行任务
        /// </summary>
        /// <typeparam name="parameterType">参数类型</typeparam>
        /// <param name="task">任务委托</param>
        /// <param name="parameter">线程参数</param>
        internal void FastStart<parameterType>(Action<parameterType> task, ref parameterType parameter)
        {
            run<parameterType> run = run<parameterType>.Pop();
            run.Set(task, ref parameter);
            FastStart(run, fastCSharp.threading.thread.callType.Run);
        }
        /// <summary>
        /// 获取一个线程并执行任务
        /// </summary>
        /// <typeparam name="parameterType">参数类型</typeparam>
        /// <param name="task">任务委托</param>
        /// <param name="parameter">线程参数</param>
        internal void FastStart<parameterType>(Action<parameterType> task, parameterType parameter)
        {
            run<parameterType> run = run<parameterType>.Pop();
            run.Set(task, parameter);
            FastStart(run, fastCSharp.threading.thread.callType.Run);
        }
        ///// <summary>
        ///// 获取一个线程并执行任务
        ///// </summary>
        ///// <typeparam name="parameterType">参数类型</typeparam>
        ///// <param name="task">任务委托</param>
        ///// <param name="parameter">线程参数</param>
        //internal void FastStart<parameterType>(pushPool<parameterType> task, ref parameterType parameter)
        //{
        //    runPushPool<parameterType> run = runPushPool<parameterType>.Pop();
        //    run.Set(task, ref parameter);
        //    FastStart(run, fastCSharp.threading.thread.callType.Run);
        //}
        /// <summary>
        /// 获取一个线程并执行任务
        /// </summary>
        /// <param name="task">任务委托</param>
        /// <param name="domainUnload">应用程序退出处理</param>
        /// <param name="onError">应用程序退出处理</param>
        private void start(Action task, Action<Exception> onError, Action domainUnload)
        {
            if (task == null) log.Error.Throw(null, "缺少 线程委托", false);
            if (isDisposed) log.Default.Real("线程池已经被释放", null, false);
            else if (domainUnload == null)
            {
                if (onError == null) FastStart(task);
                else FastStart(task, onError);
            }
            else
            {
                thread thread = threads.Pop();
                if (thread == null) new thread(this, stackSize, task, onError, domainUnload, threading.thread.callType.Action, onError == null ? threading.thread.errorType.None : threading.thread.errorType.Action, fastCSharp.domainUnload.unloadType.Action);
                else thread.RunTask(task, onError, domainUnload, threading.thread.callType.Action, onError == null ? threading.thread.errorType.None : threading.thread.errorType.Action, fastCSharp.domainUnload.unloadType.Action);
            }
        }
        /// <summary>
        /// 获取一个线程并执行任务
        /// </summary>
        /// <param name="task">任务委托</param>
        /// <param name="domainUnload">应用程序退出处理</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Start(Action task, Action domainUnload)
        {
            start(task, null, domainUnload);
        }
        /// <summary>
        /// 获取一个线程并执行任务
        /// </summary>
        /// <param name="task">任务委托</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Start(Action task)
        {
            if (task == null) log.Error.Throw(null, "缺少 线程委托", false);
            FastStart(task);
        }
        ///// <summary>
        ///// 获取一个线程并执行任务
        ///// </summary>
        ///// <typeparam name="parameterType">参数类型</typeparam>
        ///// <param name="task">任务委托</param>
        ///// <param name="parameter">线程参数</param>
        ///// <param name="domainUnload">应用程序退出处理</param>
        ///// <param name="onError">应用程序退出处理</param>
        //public void Start<parameterType>
        //    (Action<parameterType> task, ref parameterType parameter, Action domainUnload = null, Action<Exception> onError = null)
        //{
        //    if (task == null) log.Error.Throw(null, "缺少 线程委托", false);
        //    run<parameterType> run = run<parameterType>.Pop();
        //    start(run.Set(task, ref parameter), domainUnload, onError);
        //}
        /// <summary>
        /// 获取一个线程并执行任务
        /// </summary>
        /// <typeparam name="parameterType">参数类型</typeparam>
        /// <param name="task">任务委托</param>
        /// <param name="parameter">线程参数</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Start<parameterType>(Action<parameterType> task, parameterType parameter)
        {
            if (task == null) log.Error.Throw(null, "缺少 线程委托", false);
            FastStart(task, parameter);
        }
        /// <summary>
        /// 获取一个线程并执行任务
        /// </summary>
        /// <typeparam name="parameterType">参数类型</typeparam>
        /// <param name="task">任务委托</param>
        /// <param name="parameter">线程参数</param>
        public void Start<parameterType>(Action<parameterType> task, ref parameterType parameter)
        {
            if (task == null) log.Error.Throw(null, "缺少 线程委托", false);
            FastStart(task, ref parameter);
        }
        ///// <summary>
        ///// 获取一个线程并执行任务
        ///// </summary>
        ///// <typeparam name="parameterType">参数类型</typeparam>
        ///// <typeparam name="returnType">返回值类型</typeparam>
        ///// <param name="task">任务委托</param>
        ///// <param name="parameter">线程参数</param>
        ///// <param name="onReturn">返回值执行委托</param>
        ///// <param name="domainUnload">应用程序退出处理</param>
        ///// <param name="onError">应用程序退出处理</param>
        //public void Start<parameterType, returnType>(Func<parameterType, returnType> task, ref parameterType parameter,
        //    Action<returnType> onReturn, Action domainUnload = null, Action<Exception> onError = null)
        //{
        //    if (task == null) log.Error.Throw(null, "缺少 线程委托", false);
        //    run<parameterType, returnType> run = run<parameterType, returnType>.Pop();
        //    start(run.Set(task, ref parameter, onReturn), domainUnload, onError);
        //}
        /// <summary>
        /// 线程入池
        /// </summary>
        /// <param name="thread">线程池线程</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void Push(thread thread)
        {
            if (isDisposed) thread.Stop();
            else
            {
                threads.Push(thread);
                if (isDisposed) disposePool();
            }
        }
        /// <summary>
        /// 检测日志输出
        /// </summary>
        public static void CheckLog()
        {
            subArray<Thread> threads = threading.thread.Threads;
            log.Default.Add("活动线程数量 " + threads.length.toString(), new System.Diagnostics.StackFrame(), false);
            int currentId = Thread.CurrentThread.ManagedThreadId;
            foreach (Thread thread in threads)
            {
                if (thread.ManagedThreadId != currentId)
                {
                    StackTrace stack = null;
                    Exception exception = null;
                    bool isSuspend = false;
                    try
                    {
#pragma warning disable 618
                        if ((thread.ThreadState & (System.Threading.ThreadState.StopRequested | System.Threading.ThreadState.Unstarted | System.Threading.ThreadState.Stopped | System.Threading.ThreadState.WaitSleepJoin | System.Threading.ThreadState.Suspended | System.Threading.ThreadState.AbortRequested | System.Threading.ThreadState.Aborted)) == 0)
                        {
                            thread.Suspend();
                            isSuspend = true;
                        }
#pragma warning disable 612
                        stack = new StackTrace(thread, true);
#pragma warning restore 612
#pragma warning restore 618
                        if (stack.FrameCount == threading.thread.DefaultFrameCount) stack = null;
                    }
                    catch (Exception error)
                    {
                        exception = error;
                    }
                    finally
                    {
#pragma warning disable 618
                        if (isSuspend) thread.Resume();
#pragma warning restore 618
                    }
                    if (exception != null)
                    {
                        try
                        {
                            log.Default.Add(exception, null, false);
                        }
                        catch { }
                    }
                    if (stack != null)
                    {
                        try
                        {
                            log.Default.Add(stack.ToString(), new System.Diagnostics.StackFrame(), false);
                        }
                        catch { }
                    }
                }
            }
        }
        /// <summary>
        /// 微型线程池,堆栈大小可能只有128K
        /// </summary>
        public static readonly threadPool TinyPool = new threadPool(fastCSharp.config.appSetting.TinyThreadStackSize);
        /// <summary>
        /// 默认线程池
        /// </summary>
        public static readonly threadPool Default = fastCSharp.config.appSetting.ThreadStackSize != fastCSharp.config.appSetting.TinyThreadStackSize ? new threadPool(fastCSharp.config.appSetting.ThreadStackSize) : TinyPool;
        static threadPool()
        {
            if (fastCSharp.config.appSetting.IsCheckMemory) checkMemory.Add(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }
    }
}
