using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Timers;

namespace fastCSharp.threading
{
    /// <summary>
    /// 定时任务
    /// </summary>
    public abstract class timerTaskBase : IDisposable
    {
        /// <summary>
        /// 任务处理线程池
        /// </summary>
        protected threadPool threadPool;
        /// <summary>
        /// 定时器
        /// </summary>
        protected System.Timers.Timer timer = new System.Timers.Timer();
        /// <summary>
        /// 最近时间
        /// </summary>
        protected long nearTime = long.MaxValue;
        /// <summary>
        /// 任务访问锁
        /// </summary>
        protected readonly object taskLock = new object();
        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
                timer = null;
            }
        }
    }
    /// <summary>
    /// 定时任务
    /// </summary>
    public sealed class timerTask : timerTaskBase
    {
        /// <summary>
        /// 已排序任务
        /// </summary>
        private arrayHeap<taskInfo> taskHeap = new arrayHeap<taskInfo>(true);
        /// <summary>
        /// 定时任务信息
        /// </summary>
        /// <param name="threadPool">任务处理线程池</param>
        public timerTask(threadPool threadPool)
        {
            this.threadPool = threadPool ?? threadPool.TinyPool;
            timer.Elapsed += onTimer;
            timer.AutoReset = false;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            pub.Dispose(ref taskHeap);
        }
        /// <summary>
        /// 添加新任务
        /// </summary>
        /// <param name="run">任务执行委托</param>
        /// <param name="type">调用类型</param>
        /// <param name="onError">任务执行出错委托,停止任务参数null</param>
        /// <param name="runTime">执行时间</param>
        private void add(object run, thread.callType type, Action<Exception> onError, DateTime runTime)
        {
            bool isThread = false;
            long runTimeTicks = runTime.Ticks;
            Monitor.Enter(taskLock);
            try
            {
                taskHeap.Push(runTimeTicks, new taskInfo { Call = run, Type = type, OnError = onError });
                if (runTimeTicks < nearTime)
                {
                    timer.Stop();
                    nearTime = runTimeTicks;
                    double time = (runTime - date.Now).TotalMilliseconds + 1;
                    if (time > 0)
                    {
                        timer.Interval = Math.Min(time, int.MaxValue);
                        timer.Start();
                    }
                    else isThread = true;
                }
            }
            finally { Monitor.Exit(taskLock); }
            if (isThread) threadPool.FastStart(this, thread.callType.TimerTaskRun);
        }
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="run">任务执行委托</param>
        /// <param name="type">调用类型</param>
        /// <param name="runTime">执行时间</param>
        internal void Add(object run, thread.callType type, DateTime runTime)
        {
            if (runTime > date.Now) add(run, type, null, runTime);
            else threadPool.FastStart(run, type);
        }
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="run">任务执行委托</param>
        /// <param name="runTime">执行时间</param>
        public void Add(Action run, DateTime runTime)
        {
            if (run != null)
            {
                if (runTime > date.Now) add(run, thread.callType.Action, null, runTime);
                else threadPool.FastStart(run);
            }
        }
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="run">任务执行委托</param>
        /// <param name="runTime">执行时间</param>
        /// <param name="onError">任务执行出错委托,停止任务参数null</param>
        public void Add(Action run, DateTime runTime, Action<Exception> onError)
        {
            if (run != null)
            {
                if (runTime > date.Now) add(run, thread.callType.Action, onError, runTime);
                else threadPool.FastStart(run, onError);
            }
        }
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <typeparam name="parameterType">执行参数类型</typeparam>
        /// <param name="run">任务执行委托</param>
        /// <param name="parameter">执行参数</param>
        /// <param name="runTime">执行时间</param>
        /// <param name="onError">任务执行出错委托,停止任务参数null</param>
        public void Add<parameterType>
            (Action<parameterType> run, ref parameterType parameter, DateTime runTime, Action<Exception> onError)
        {
            if (run != null)
            {
                if (runTime > date.Now)
                {
                    run<parameterType> action = run<parameterType>.Pop();
                    action.Set(run, ref parameter);
                    add(action, thread.callType.Run, onError, runTime);
                }
                else threadPool.FastStart(run, ref parameter, onError);
            }
        }
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <typeparam name="parameterType">执行参数类型</typeparam>
        /// <param name="run">任务执行委托</param>
        /// <param name="parameter">执行参数</param>
        /// <param name="runTime">执行时间</param>
        /// <param name="onError">任务执行出错委托,停止任务参数null</param>
        public void Add<parameterType>
            (Action<parameterType> run, parameterType parameter, DateTime runTime, Action<Exception> onError)
        {
            if (run != null)
            {
                if (runTime > date.Now)
                {
                    run<parameterType> action = run<parameterType>.Pop();
                    action.Set(run, parameter);
                    add(action, thread.callType.Run, onError, runTime);
                }
                else threadPool.FastStart(run, ref parameter, onError);
            }
        }
        /// <summary>
        /// 线程池任务
        /// </summary>
        internal void Run()
        {
            Monitor.Enter(taskLock);
            try
            {
                while (taskHeap.Count != 0)
                {
                    keyValue<long, taskInfo> task = taskHeap.UnsafeTop();
                    if (task.Key <= date.Now.Ticks)
                    {
                        taskHeap.RemoveTop();
                        task.Value.Start(threadPool);
                    }
                    else
                    {
                        if (task.Key != nearTime)
                        {
                            nearTime = task.Key;
                            timer.Interval = Math.Min(Math.Max(new TimeSpan(task.Key - date.Now.Ticks).TotalMilliseconds + 1, 1), int.MaxValue);
                            timer.Start();
                        }
                        return;
                    }
                }
                nearTime = long.MaxValue;
            }
            finally { Monitor.Exit(taskLock); }
        }
        /// <summary>
        /// 触发定时任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onTimer(object sender, ElapsedEventArgs e)
        {
            Run();
        }
        /// <summary>
        /// 默认定时任务
        /// </summary>
        public static readonly timerTask Default = new timerTask(null);
        static timerTask()
        {
            if (fastCSharp.config.appSetting.IsCheckMemory) checkMemory.Add(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }
    }
    /// <summary>
    /// 可取消定时任务
    /// </summary>
    public sealed class timerCancelTask : timerTaskBase
    {
        /// <summary>
        /// 任务信息
        /// </summary>
        public sealed class task
        {
            /// <summary>
            /// 任务信息
            /// </summary>
            internal taskInfo Task;
            /// <summary>
            /// 取消任务
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Cancel()
            {
                Task.Cancel();
            }
            /// <summary>
            /// 任务信息
            /// </summary>
            /// <param name="run">任务执行委托</param>
            /// <param name="type">调用类型</param>
            internal task(object run, thread.callType type)
            {
                Task.Set(run, type);
            }
            /// <summary>
            /// 任务信息
            /// </summary>
            /// <param name="run">任务执行委托</param>
            /// <param name="type">调用类型</param>
            /// <param name="onError">任务执行出错委托,停止任务参数null</param>
            internal task(object run, thread.callType type, Action<Exception> onError)
            {
                Task.Set(run, type, onError);
            }
        }
        /// <summary>
        /// 已排序任务
        /// </summary>
        private arrayHeap<task> taskHeap = new arrayHeap<task>(true);
        /// <summary>
        /// 定时任务信息
        /// </summary>
        /// <param name="threadPool">任务处理线程池</param>
        public timerCancelTask(threadPool threadPool)
        {
            this.threadPool = threadPool ?? threadPool.TinyPool;
            timer.Elapsed += onTimer;
            timer.AutoReset = false;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            pub.Dispose(ref taskHeap);
        }
        /// <summary>
        /// 添加新任务
        /// </summary>
        /// <param name="task">任务信息</param>
        /// <param name="runTime">执行时间</param>
        private void add(task task, DateTime runTime)
        {
            bool isThread = false;
            long runTimeTicks = runTime.Ticks;
            Monitor.Enter(taskLock);
            try
            {
                taskHeap.Push(runTimeTicks, task);
                if (runTimeTicks < nearTime)
                {
                    timer.Stop();
                    nearTime = runTimeTicks;
                    double time = (runTime - date.Now).TotalMilliseconds + 1;
                    if (time > 0)
                    {
                        timer.Interval = Math.Min(time, int.MaxValue);
                        timer.Start();
                    }
                    else isThread = true;
                }
            }
            finally { Monitor.Exit(taskLock); }
            if (isThread) threadPool.FastStart(this, thread.callType.TimerCancelTaskRun);
        }
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="run">任务执行委托</param>
        /// <param name="type">调用类型</param>
        /// <param name="runTime">执行时间</param>
        /// <returns>任务信息,null表示不可取消</returns>
        internal task Add(object run, thread.callType type, DateTime runTime)
        {
            if (runTime > date.Now)
            {
                task task = new task(run, type);
                add(task, runTime);
                return task;
            }
            threadPool.FastStart(run, type);
            return null;
        }
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="run">任务执行委托</param>
        /// <param name="runTime">执行时间</param>
        /// <returns>任务信息,null表示不可取消</returns>
        public task Add(Action run, DateTime runTime)
        {
            if (run != null)
            {
                if (runTime > date.Now)
                {
                    task task = new task(run, thread.callType.Action);
                    add(task, runTime);
                    return task;
                }
                threadPool.FastStart(run);
            }
            return null;
        }
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="run">任务执行委托</param>
        /// <param name="runTime">执行时间</param>
        /// <param name="onError">任务执行出错委托,停止任务参数null</param>
        /// <returns>任务信息,null表示不可取消</returns>
        public task Add(Action run, DateTime runTime, Action<Exception> onError)
        {
            if (run != null)
            {
                if (runTime > date.Now)
                {
                    task task = new task(run, thread.callType.Action, onError);
                    add(task, runTime);
                    return task;
                }
                threadPool.FastStart(run, onError);
            }
            return null;
        }
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <typeparam name="parameterType">执行参数类型</typeparam>
        /// <param name="run">任务执行委托</param>
        /// <param name="parameter">执行参数</param>
        /// <param name="runTime">执行时间</param>
        /// <param name="onError">任务执行出错委托,停止任务参数null</param>
        /// <returns>任务信息,null表示不可取消</returns>
        public task Add<parameterType>
            (Action<parameterType> run, ref parameterType parameter, DateTime runTime, Action<Exception> onError)
        {
            if (run != null)
            {
                if (runTime > date.Now)
                {
                    run<parameterType> action = run<parameterType>.Pop();
                    action.Set(run, ref parameter);
                    task task = new task(action, thread.callType.Run, onError);
                    add(task, runTime);
                    return task;
                }
                threadPool.FastStart(run, ref parameter, onError);
            }
            return null;
        }
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <typeparam name="parameterType">执行参数类型</typeparam>
        /// <param name="run">任务执行委托</param>
        /// <param name="parameter">执行参数</param>
        /// <param name="runTime">执行时间</param>
        /// <param name="onError">任务执行出错委托,停止任务参数null</param>
        /// <returns>任务信息,null表示不可取消</returns>
        public task Add<parameterType>
            (Action<parameterType> run, parameterType parameter, DateTime runTime, Action<Exception> onError)
        {
            if (run != null)
            {
                if (runTime > date.Now)
                {
                    run<parameterType> action = run<parameterType>.Pop();
                    action.Set(run, parameter);
                    task task = new task(action, thread.callType.Run, onError);
                    add(task, runTime);
                    return task;
                }
                else threadPool.FastStart(run, ref parameter, onError);
            }
            return null;
        }
        /// <summary>
        /// 线程池任务
        /// </summary>
        internal void Run()
        {
            Monitor.Enter(taskLock);
            try
            {
                while (taskHeap.Count != 0)
                {
                    keyValue<long, task> task = taskHeap.UnsafeTop();
                    if (task.Key <= date.Now.Ticks)
                    {
                        task.Value.Task.CheckStart(threadPool);
                        taskHeap.RemoveTop();
                    }
                    else if (task.Value.Task.Type == thread.callType.None) taskHeap.RemoveTop();
                    else
                    {
                        if (task.Key != nearTime)
                        {
                            nearTime = task.Key;
                            timer.Interval = Math.Min(Math.Max(new TimeSpan(task.Key - date.Now.Ticks).TotalMilliseconds + 1, 1), int.MaxValue);
                            timer.Start();
                        }
                        return;
                    }
                }
                nearTime = long.MaxValue;
            }
            finally { Monitor.Exit(taskLock); }
        }
        /// <summary>
        /// 触发定时任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onTimer(object sender, ElapsedEventArgs e)
        {
            Run();
        }
        /// <summary>
        /// 默认定时任务
        /// </summary>
        public static readonly timerCancelTask Default = new timerCancelTask(null);
        static timerCancelTask()
        {
            if (fastCSharp.config.appSetting.IsCheckMemory) checkMemory.Add(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }
    }
}
