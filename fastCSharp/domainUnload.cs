using System;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.CompilerServices;
using fastCSharp.threading;
//#if MONO
//#else
//using System.Windows.Forms;
//#endif

namespace fastCSharp
{
    /// <summary>
    /// 应用程序卸载处理
    /// </summary>
    public static class domainUnload
    {
        /// <summary>
        /// 卸载状态
        /// </summary>
        private enum unloadState
        {
            /// <summary>
            /// 正常运行状态
            /// </summary>
            Run,
            /// <summary>
            /// 卸载中，等待事务结束
            /// </summary>
            WaitTransaction,
            /// <summary>
            /// 卸载事件处理
            /// </summary>
            Event,
            /// <summary>
            /// 已经卸载
            /// </summary>
            Unloaded
        }
        /// <summary>
        /// 卸载处理类型
        /// </summary>
        internal enum unloadType : byte
        {
            None,
            /// <summary>
            /// 委托回调
            /// </summary>
            Action,
            /// <summary>
            /// 释放垃圾定时清理
            /// </summary>
            DisposeTimerDispose,
            /// <summary>
            /// 释放内存数据库物理层
            /// </summary>
            MemoryDatabasePhysicalDispose,
            /// <summary>
            /// 释放数据库物理层集合
            /// </summary>
            MemoryDatabasePhysicalSetDispose,
            /// <summary>
            /// 释放TCP调用服务端
            /// </summary>
            TcpServerDispose,
            /// <summary>
            /// 关闭TCP注册服务客户端
            /// </summary>
            TcpRegisterClientDispose,
            /// <summary>
            /// 释放任务处理
            /// </summary>
            TaskDispose,
            /// <summary>
            /// 释放线程池
            /// </summary>
            ThreadPoolDispose,
            /// <summary>
            /// 释放日志
            /// </summary>
            LogDispose
        }
        /// <summary>
        /// 委托回调
        /// </summary>
        internal struct unload : IEquatable<unload>
        {
            /// <summary>
            /// 卸载处理对象
            /// </summary>
            public object Unload;
            /// <summary>
            /// 卸载处理类型
            /// </summary>
            public unloadType Type;
            /// <summary>
            /// 委托回调
            /// </summary>
            /// <param name="unload"></param>
            /// <param name="type"></param>
            public void Set(object unload, unloadType type)
            {
                Unload = unload;
                Type = type;
            }
            /// <summary>
            /// 卸载处理
            /// </summary>
            public void Call()
            {
                switch (Type)
                {
                    case unloadType.Action: new unionType { Value = Unload }.Action(); return;
                    case unloadType.DisposeTimerDispose: fastCSharp.threading.disposeTimer.Default.Dispose(); return;
                    case unloadType.MemoryDatabasePhysicalDispose: new unionType { Value = Unload }.MemoryDatabasePhysical.Dispose(); return;
                    case unloadType.MemoryDatabasePhysicalSetDispose: new unionType { Value = Unload }.MemoryDatabasePhysicalSet.Dispose(); return;
                    case unloadType.TcpServerDispose: new unionType { Value = Unload }.TcpServer.Dispose(); return;
                    case unloadType.TcpRegisterClientDispose: fastCSharp.net.tcp.tcpRegister.client.DisposeClients(); return;
                    case unloadType.TaskDispose: new unionType { Value = Unload }.TaskBase.Dispose(); return;
                    case unloadType.ThreadPoolDispose: new unionType { Value = Unload }.ThreadPool.Dispose(); return;
                    case unloadType.LogDispose: new unionType { Value = Unload }.Log.Dispose(); return;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public bool Equals(unload other)
            {
                return Type == other.Type && Unload == other.Unload;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                if (Unload == null) return (byte)Type;
                return Unload.GetHashCode() ^ (byte)Type;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                return Equals((unload)obj);
            }
        }
        /// <summary>
        /// 是否已关闭
        /// </summary>
        private static unloadState state = unloadState.Run;
        /// <summary>
        /// 卸载处理函数集合
        /// </summary>
        private static readonly HashSet<unload> unloaders = hashSet<unload>.Create();
        /// <summary>
        /// 卸载处理函数集合
        /// </summary>
        private static readonly HashSet<unload> lastUnloaders = hashSet<unload>.Create();
        /// <summary>
        /// 卸载处理函数访问锁
        /// </summary>
        private static readonly object unloaderLock = new object();
        /// <summary>
        /// 事务数量
        /// </summary>
        private static volatile int transactionCount;
        ///// <summary>
        ///// 事务锁
        ///// </summary>
        //private readonly object transactionLock = new object();
        /// <summary>
        /// 添加应用程序卸载处理
        /// </summary>
        /// <param name="onUnload">卸载处理函数</param>
        /// <param name="isLog">添加失败是否输出日志</param>
        /// <returns>是否添加成功</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static bool Add(Action onUnload, bool isLog = true)
        {
            return Add(onUnload, unloadType.Action, isLog);
        }
        /// <summary>
        /// 添加应用程序卸载处理
        /// </summary>
        /// <param name="onUnload">卸载处理函数</param>
        /// <param name="unloadType"></param>
        /// <param name="isLog">添加失败是否输出日志</param>
        /// <returns>是否添加成功</returns>
        internal static bool Add(object onUnload, unloadType unloadType, bool isLog = true)
        {
            bool isAdd = false;
            Monitor.Enter(unloaderLock);
            try
            {
                if (state == unloadState.Run || state == unloadState.WaitTransaction)
                {
                    unloaders.Add(new unload { Unload = onUnload, Type = unloadType });
                    isAdd = true;
                }
            }
            finally { Monitor.Exit(unloaderLock); }
            if (!isAdd && isLog) log.Default.Real("应用程序正在退出", null, false);
            return isAdd;
        }
        /// <summary>
        /// 添加应用程序卸载处理
        /// </summary>
        /// <param name="unload">卸载处理函数</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static void Add(ref unload unload)
        {
            Monitor.Enter(unloaderLock);
            try
            {
                if (state == unloadState.Run || state == unloadState.WaitTransaction)
                {
                    unloaders.Add(unload);
                    return;
                }
            }
            finally { Monitor.Exit(unloaderLock); }
            log.Default.Real("应用程序正在退出", null, false);
        }
        /// <summary>
        /// 添加应用程序卸载处理
        /// </summary>
        /// <param name="onUnload">卸载处理函数</param>
        /// <param name="unloadType"></param>
        /// <returns>是否添加成功</returns>
        internal static bool AddLast(object onUnload, unloadType unloadType)
        {
            bool isAdd = false;
            Monitor.Enter(unloaderLock);
            try
            {
                if (state == unloadState.Run || state == unloadState.WaitTransaction)
                {
                    lastUnloaders.Add(new unload { Unload = onUnload, Type = unloadType });
                    isAdd = true;
                }
            }
            finally { Monitor.Exit(unloaderLock); }
            if (!isAdd) log.Default.Real("应用程序正在退出", null, false);
            return isAdd;
        }
        /// <summary>
        /// 删除卸载处理函数
        /// </summary>
        /// <param name="onUnload">卸载处理函数</param>
        /// <param name="isRun">是否执行删除的函数</param>
        /// <returns>是否删除成功</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static bool Remove(Action onUnload, bool isRun)
        {
            return Remove(onUnload, unloadType.Action, isRun);
        }
        /// <summary>
        /// 删除卸载处理函数
        /// </summary>
        /// <param name="onUnload">卸载处理函数</param>
        /// <param name="unloadType"></param>
        /// <param name="isRun">是否执行删除的函数</param>
        /// <returns>是否删除成功</returns>
        internal static bool Remove(object onUnload, unloadType unloadType, bool isRun)
        {
            Monitor.Enter(unloaderLock);
            bool isRemove = unloaders.Remove(new unload { Unload = onUnload, Type = unloadType });
            Monitor.Exit(unloaderLock);
            if (isRemove && isRun) new unload { Unload = onUnload, Type = unloadType }.Call();
            return isRemove;
        }
        /// <summary>
        /// 删除卸载处理函数
        /// </summary>
        /// <param name="unload">卸载处理函数</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static void Remove(ref unload unload)
        {
            Monitor.Enter(unloaderLock);
            unloaders.Remove(unload);
            Monitor.Exit(unloaderLock);
            unload.Unload = null;
        }
        /// <summary>
        /// 删除卸载处理函数
        /// </summary>
        /// <param name="onUnload">卸载处理函数</param>
        /// <param name="unloadType"></param>
        /// <param name="isRun">是否执行删除的函数</param>
        internal static void RemoveLast(object onUnload, unloadType unloadType, bool isRun)
        {
            if (Monitor.TryEnter(unloaderLock))
            {
                bool isRemove = lastUnloaders.Remove(new unload { Unload = onUnload, Type = unloadType });
                Monitor.Exit(unloaderLock);
                if (isRemove && isRun) new unload { Unload = onUnload, Type = unloadType }.Call();
            }
            else if (isRun) fastCSharp.threading.threadPool.TinyPool.FastStart(new unload { Unload = onUnload, Type = unloadType }, thread.callType.DomainUnloadRemoveLastRun);
            else fastCSharp.threading.threadPool.TinyPool.FastStart(new unload { Unload = onUnload, Type = unloadType }, thread.callType.DomainUnloadRemoveLast);
        }
        /// <summary>
        /// 删除卸载处理函数
        /// </summary>
        /// <param name="onUnload"></param>
        internal static void RemoveLast(unload onUnload)
        {
            Monitor.Enter(unloaderLock);
            lastUnloaders.Remove(onUnload);
            Monitor.Exit(unloaderLock);
        }
        /// <summary>
        /// 删除卸载处理函数
        /// </summary>
        /// <param name="onUnload"></param>
        internal static void RemoveLastRun(unload onUnload)
        {
            Monitor.Enter(unloaderLock);
            bool isRemove = lastUnloaders.Remove(onUnload);
            Monitor.Exit(unloaderLock);
            if (isRemove) onUnload.Call();
        }
        /// <summary>
        /// 新事务开始,请保证唯一调用TransactionEnd,否则将导致卸载事件不被执行
        /// </summary>
        /// <param name="ignoreWait">忽略卸载中的等待事务，用于事务派生的事务</param>
        /// <returns>是否成功</returns>
        public static bool TransactionStart(bool ignoreWait)
        {
            Monitor.Enter(unloaderLock);
            if (state == unloadState.Run || (ignoreWait && state == unloadState.WaitTransaction))
            {
                ++transactionCount;
                Monitor.Exit(unloaderLock);
                return true;
            }
            Monitor.Exit(unloaderLock);
            log.Default.Real("应用程序正在退出", null, false);
            return false;
        }
        /// <summary>
        /// 请保证TransactionStart与TransactionEnd一一对应，否则将导致卸载事件不被执行
        /// </summary>
        public static void TransactionEnd()
        {
            Monitor.Enter(unloaderLock);
            --transactionCount;
            Monitor.Exit(unloaderLock);
        }
        /// <summary>
        /// 事务结束
        /// </summary>
        private sealed class transactionEnd
        {
            /// <summary>
            /// 任务执行委托
            /// </summary>
            public object Value;
            /// <summary>
            /// 事务委托类型
            /// </summary>
            public thread.callType Type;
            /// <summary>
            /// 任务执行
            /// </summary>
            public void Run()
            {
                try
                {
                    new thread.call { Value = Value, Type = Type }.Call();
                }
                finally { TransactionEnd(); }
            }
        }
        /// <summary>
        /// 获取事务结束委托
        /// </summary>
        /// <param name="run">任务执行委托</param>
        /// <returns>事务结束委托</returns>
        public static Action Transaction(Action run)
        {
            if (TransactionStart(true))
            {
                return new transactionEnd { Value = run, Type = thread.callType.Action }.Run;
            }
            log.Error.Throw(log.exceptionType.ErrorOperation);
            return null;
        }
        /// <summary>
        /// 获取事务结束委托
        /// </summary>
        /// <typeparam name="parameterType">参数类型</typeparam>
        /// <param name="run">任务执行委托</param>
        /// <param name="parameter">参数</param>
        /// <returns>事务结束委托</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static Action Transaction<parameterType>(Action<parameterType> run, parameterType parameter)
        {
            return Transaction(run, ref parameter);
        }
        /// <summary>
        /// 获取事务结束委托
        /// </summary>
        /// <typeparam name="parameterType">参数类型</typeparam>
        /// <param name="run">任务执行委托</param>
        /// <param name="parameter">参数</param>
        /// <returns>事务结束委托</returns>
        public static Action Transaction<parameterType>(Action<parameterType> run, ref parameterType parameter)
        {
            if (TransactionStart(true))
            {
                run<parameterType> action = run<parameterType>.Pop();
                action.Set(run, ref parameter);
                return new transactionEnd { Value = action, Type = thread.callType.Run }.Run;
            }
            log.Error.Throw(log.exceptionType.ErrorOperation);
            return null;
        }
        /// <summary>
        /// 等待事务结束
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static void WaitTransaction()
        {
            while (transactionCount != 0) Thread.Sleep(1);
        }
        /// <summary>
        /// 退出程序
        /// </summary>
        public static void Exit()
        {
            unloadEvent(null, null);
            Environment.Exit(-1);
        }
        /// <summary>
        /// 应用程序卸载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void unloadEvent(object sender, EventArgs e)
        {
            unloadEvent();
        }
        /// <summary>
        /// 应用程序卸载事件
        /// </summary>
        private static void unloadEvent()
        {
            Monitor.Enter(unloaderLock);
            if (state == unloadState.Run)
            {
                if (transactionCount != 0)
                {
                    state = unloadState.WaitTransaction;
                    Monitor.Exit(unloaderLock);
                    for (DateTime logTime = DateTime.MinValue; transactionCount != 0; Thread.Sleep(1))
                    {
                        if (date.nowTime.Now > logTime)
                        {
                            log.Default.Real("事务未结束 " + transactionCount.toString(), new System.Diagnostics.StackFrame(), false);
                            logTime = date.nowTime.Now.AddTicks(date.MinutesTicks);
                        }
                    }
                    Monitor.Enter(unloaderLock);
                }
                state = unloadState.Event;
                try
                {
                    foreach (unload value in unloaders.getArray())
                    {
                        try { value.Call(); }
                        catch (Exception error)
                        {
                            fastCSharp.log.Error.Real(error, null, false);
                        }
                    }
                    foreach (unload value in lastUnloaders.getArray())
                    {
                        try { value.Call(); }
                        catch (Exception error)
                        {
                            fastCSharp.log.Error.Real(error, null, false);
                        }
                    }
                    state = unloadState.Unloaded;
                }
                finally { Monitor.Exit(unloaderLock); }
            }
            else Monitor.Exit(unloaderLock);
        }
        /// <summary>
        /// 线程错误事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="error"></param>
        private static void onError(object sender, UnhandledExceptionEventArgs error)
        {
            Exception exception = error.ExceptionObject as Exception;
            if (exception != null) log.Error.Real(exception, null, false);
            else log.Error.Real(null, error.ExceptionObject.ToString(), false);
            unloadEvent(null, null);
        }
//#if MONO
//#else
//        /// <summary>
//        /// UI线程错误事件
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private static void onError(object sender, ThreadExceptionEventArgs e)
//        {
//            log.Error.Real(e.Exception, null, false);
//            if (IsThrowThreadException)
//            {
//                unload();
//                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
//                throw e.Exception;
//            }
//        }
//        /// <summary>
//        /// 绑定到WinForm应用程序
//        /// </summary>
//        public static void BindWinFormApplication()
//        {
//            Application.ApplicationExit += unload;
//            Application.ThreadException += onError;
//            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
//        }
//#endif
        /// <summary>
        /// 是否抛出UI线程异常
        /// </summary>
        public static bool IsThrowThreadException;
        static domainUnload()
        {
            if (AppDomain.CurrentDomain.IsDefaultAppDomain()) AppDomain.CurrentDomain.ProcessExit += unloadEvent;
            else AppDomain.CurrentDomain.DomainUnload += unloadEvent;
            AppDomain.CurrentDomain.UnhandledException += onError;
            if (fastCSharp.config.appSetting.IsCheckMemory) checkMemory.Add(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }
    }

}
