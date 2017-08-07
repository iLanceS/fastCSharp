using System;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.CompilerServices;

namespace fastCSharp.threading
{
    /// <summary>
    /// 任务队列
    /// </summary>
    public sealed class taskQueue : taskBase
    {
        /// <summary>
        /// 新任务集合
        /// </summary>
        private list<taskInfo> pushTasks = new list<taskInfo>();
        /// <summary>
        /// 新任务集合数量
        /// </summary>
        protected override int pushTaskCount
        {
            get { return pushTasks.length; }
        }
        /// <summary>
        /// 当前执行任务集合
        /// </summary>
        private list<taskInfo> currentTasks = new list<taskInfo>();
        /// <summary>
        /// 任务处理
        /// </summary>
        /// <param name="isDisposeWait">默认释放资源是否等待线程结束</param>
        /// <param name="threadPool">线程池</param>
        public taskQueue(bool isDisposeWait = true, threadPool threadPool = null)
        {
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
                int threadCount = this.threadCount;
                try
                {
                    pushTasks.Add(task);
                    if (this.threadCount == 0)
                    {
                        this.threadCount = 1;
                        freeWaitHandle.Reset();
                    }
                }
                finally { Monitor.Exit(taskLock); }
                if (threadCount == 0)
                {
                    try
                    {
                        threadPool.FastStart(this, thread.callType.TaskQueueRun);
                        return true;
                    }
                    catch (Exception error)
                    {
                        Monitor.Enter(taskLock);
                        this.threadCount = 0;
                        Monitor.Exit(taskLock);
                        log.Error.Add(error, null, false);
                    }
                }
                else return true;
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
        /// <summary>
        /// 执行任务
        /// </summary>
        internal void Run()
        {
            do
            {
                Monitor.Enter(taskLock);
                int taskCount = pushTasks.length;
                if (taskCount == 0)
                {
                    threadCount = 0;
                    Monitor.Exit(taskLock);
                    freeWaitHandle.Set();
                    break;
                }
                list<taskInfo> runTasks = pushTasks;
                pushTasks = currentTasks;
                currentTasks = runTasks;
                Monitor.Exit(taskLock);
                taskInfo[] taskArray = runTasks.array;
                int index = 0;
                do
                {
                    taskArray[index++].RunClear();
                }
                while (index != taskCount);
                runTasks.Empty();
            }
            while (true);
        }
        /// <summary>
        /// 微型线程任务队列
        /// </summary>
        public static readonly taskQueue Tiny = new taskQueue();
        /// <summary>
        /// 默认任务队列
        /// </summary>
        public static readonly taskQueue Default = new taskQueue(true, threadPool.Default);
        static taskQueue()
        {
            if (fastCSharp.config.appSetting.IsCheckMemory) checkMemory.Add(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }
    }
}
