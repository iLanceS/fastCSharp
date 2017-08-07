using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Timers;

namespace fastCSharp.threading
{
    /// <summary>
    /// 定时任务队列
    /// </summary>
    public sealed class timerQueue : timerTaskBase
    {
        /// <summary>
        /// 已排序任务
        /// </summary>
        private arrayHeap<taskInfo> taskHeap = new arrayHeap<taskInfo>(true);
        /// <summary>
        /// 定时任务信息
        /// </summary>
        /// <param name="threadPool">任务处理线程池</param>
        public timerQueue(threadPool threadPool)
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
            add(run, type, null, runTime);
        }
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="run">任务执行委托</param>
        /// <param name="runTime">执行时间</param>
        public void Add(Action run, DateTime runTime)
        {
            if (run != null) add(run, thread.callType.Action, null, runTime);
        }
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="run">任务执行委托</param>
        /// <param name="runTime">执行时间</param>
        /// <param name="onError">任务执行出错委托,停止任务参数null</param>
        public void Add(Action run, DateTime runTime, Action<Exception> onError)
        {
            if (run != null) add(run, thread.callType.Action, onError, runTime);
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
                run<parameterType> action = run<parameterType>.Pop();
                action.Set(run, ref parameter);
                add(action, thread.callType.Run, onError, runTime);
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
                run<parameterType> action = run<parameterType>.Pop();
                action.Set(run, parameter);
                add(action, thread.callType.Run, onError, runTime);
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
                        task.Value.Run();
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
        /// 默认定时任务队列
        /// </summary>
        public static readonly timerQueue Default = new timerQueue(null);
        static timerQueue()
        {
            if (fastCSharp.config.appSetting.IsCheckMemory) checkMemory.Add(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }
    }
}
