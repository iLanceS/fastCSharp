using System;
using System.Threading;
using System.Timers;
using System.Runtime.CompilerServices;
using System.Net.Sockets;

namespace fastCSharp.threading
{
    /// <summary>
    /// 垃圾定时清理
    /// </summary>
    public sealed class disposeTimer : IDisposable
    {
        /// <summary>
        /// 清理类型
        /// </summary>
        internal enum type : byte
        {
            /// <summary>
            /// 关闭套接字
            /// </summary>
            SocketClose,
        }
        /// <summary>
        /// 任务信息
        /// </summary>
        private struct task
        {
            /// <summary>
            /// 清理目标对象
            /// </summary>
            public object Value;
            /// <summary>
            /// 清理类型
            /// </summary>
            internal type Type;
            /// <summary>
            /// 设置任务调用
            /// </summary>
            /// <param name="value"></param>
            /// <param name="type"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Set(object value, type type)
            {
                Value = value;
                Type = type;
            }
            /// <summary>
            /// 任务调用
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Call()
            {
                switch(Type)
                {
                    case type.SocketClose: new unionType { Value = Value }.Socket.Close(); Value = null; return;
                }
            }
        }
        /// <summary>
        /// 定时器
        /// </summary>
        private System.Timers.Timer timer = new System.Timers.Timer();
        /// <summary>
        /// 添加垃圾清理任务
        /// </summary>
        private task[] pushTasks;
        /// <summary>
        /// 当前处理中的垃圾清理任务
        /// </summary>
        private task[] tasks;
        /// <summary>
        /// 垃圾清理任务访问锁
        /// </summary>
        private readonly object taskLock = new object();
        /// <summary>
        /// 添加垃圾清理任务位置
        /// </summary>
        private int pushTaskIndex;
        /// <summary>
        /// 当前处理中的垃圾清理任务位置
        /// </summary>
        private int taskIndex;
        /// <summary>
        /// 定时任务信息
        /// </summary>
        public disposeTimer()
        {
            pushTasks = new task[256];
            tasks = new task[256];
            timer.Interval = fastCSharp.config.pub.Default.DisposeTimerInterval;
            timer.Elapsed += onTimer;
            timer.AutoReset = false;
            timer.Start();
            fastCSharp.domainUnload.Add(this, domainUnload.unloadType.DisposeTimerDispose);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            fastCSharp.domainUnload.Remove(this, domainUnload.unloadType.DisposeTimerDispose, false);
            pub.Dispose(ref timer);
        }
        /// <summary>
        /// 添加垃圾清理任务
        /// </summary>
        /// <param name="socket"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void AddSocketClose(Socket socket)
        {
            if (socket != null) addSocketClose(socket);
        }
        /// <summary>
        /// 添加垃圾清理任务
        /// </summary>
        /// <param name="socket"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void AddSocketClose(ref Socket socket)
        {
            if (socket != null)
            {
                addSocketClose(socket);
                socket = null;
            }
        }
        /// <summary>
        /// 添加垃圾清理任务
        /// </summary>
        /// <param name="socket"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void addSocketClose(ref Socket socket)
        {
            addSocketClose(socket);
            socket = null;
        }
        /// <summary>
        /// 添加垃圾清理任务
        /// </summary>
        /// <param name="socket"></param>
        internal void addSocketClose(Socket socket)
        {
            Monitor.Enter(taskLock);
            if (pushTaskIndex == pushTasks.Length)
            {
                try
                {
                    task[] newTasks = new task[pushTaskIndex << 1];
                    pushTasks.CopyTo(newTasks, 0);
                    newTasks[pushTaskIndex].Set(socket, type.SocketClose);
                    pushTasks = newTasks;
                    ++pushTaskIndex;
                }
                finally { Monitor.Exit(taskLock); }
                return;
            }
            pushTasks[pushTaskIndex++].Set(socket, type.SocketClose);
            Monitor.Exit(taskLock);
        }
        /// <summary>
        /// 触发定时任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onTimer(object sender, ElapsedEventArgs e)
        {
            Thread thread = Thread.CurrentThread;
            ThreadPriority threadPriority = thread.Priority;
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            try
            {
                while (this.pushTaskIndex != 0)
                {
                    Monitor.Enter(taskLock);
                    task[] pushTasks = this.pushTasks;
                    int pushTaskIndex = this.pushTaskIndex;
                    this.pushTasks = this.tasks;
                    this.pushTaskIndex = this.taskIndex;
                    Monitor.Exit(taskLock);
                    this.tasks = pushTasks;
                    this.taskIndex = pushTaskIndex;
                    do
                    {
                        pushTasks[--this.taskIndex].Call();
                    }
                    while (this.taskIndex != 0);
                }
            }
            catch (Exception error)
            {
                fastCSharp.log.Error.Add(error, null, false);
            }
            finally
            {
                timer.Start();
                thread.Priority = threadPriority;
            }
        }
        
        /// <summary>
        /// 默认垃圾定时清理
        /// </summary>
        public static readonly disposeTimer Default = new disposeTimer();
    }
}
