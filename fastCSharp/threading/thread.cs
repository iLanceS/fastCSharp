using System;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace fastCSharp.threading
{
    /// <summary>
    /// 线程池线程
    /// </summary>
    internal sealed class thread
    {
        /// <summary>
        /// 调用类型
        /// </summary>
        public enum callType : byte
        {
            None,
            /// <summary>
            /// 委托回调
            /// </summary>
            Action,
            /// <summary>
            /// 检测SQL链接
            /// </summary>
            CheckSqlConnection,
            /// <summary>
            /// 检测SQL链接
            /// </summary>
            CheckSqlConnectionArray,
            /// <summary>
            /// 新建文件监视超时检测处理
            /// </summary>
            CreateFlieTimeoutWatcherCheckTimeout,
            /// <summary>
            /// 删除应用程序卸载处理
            /// </summary>
            DomainUnloadRemoveLast,
            /// <summary>
            /// 删除应用程序卸载处理
            /// </summary>
            DomainUnloadRemoveLastRun,
            /// <summary>
            /// 文件分块写入流读取
            /// </summary>
            FileBlockStreamReader,
            /// <summary>
            /// 文件分块写入流等待缓存写入
            /// </summary>
            FileBlockStreamReaderWait,
            /// <summary>
            /// 文件流写入器写入文件数据
            /// </summary>
            FileStreamWriteFile,
            /// <summary>
            /// 文件流写入器刷新检测
            /// </summary>
            FileStreamWriterCheckFlush,
            /// <summary>
            /// HTTP套接字数据接收器写文件
            /// </summary>
            HttpSocketBoundaryIdentityReceiverWriteFile,
            /// <summary>
            /// HTTP服务器文件监视超时处理
            /// </summary>
            HttpServersFileWatcherTimeout,
            /// <summary>
            /// HTTP服务器保存域名服务器参数集合
            /// </summary>
            HttpServersSave,
            /// <summary>
            /// HTTP会话标识超时检测
            /// </summary>
            HttpSessionRefreshTimeout,
            /// <summary>
            /// 释放日志
            /// </summary>
            LogDispose,
            /// <summary>
            /// 日志流回调
            /// </summary>
            LogStreamCallback,
            /// <summary>
            /// 释放内存数据库物理层
            /// </summary>
            MemoryDatabasePhysicalDispose,
            /// <summary>
            /// 内存数据库物理层文件读取器读取文件
            /// </summary>
            MemoryDatabasePhysicalFileReader,
            /// <summary>
            /// 内存数据库表格操作工具(远程)数据加载
            /// </summary>
            MemoryDatabaseTableRemoteTableLoadEnd,
            /// <summary>
            /// 内存数据库表格操作工具(远程)数据加载
            /// </summary>
            MemoryDatabaseTableRemoteTableLoaded,
            /// <summary>
            /// 守护进程客户端调用
            /// </summary>
            ProcessCopyClient,
            /// <summary>
            /// 执行任务
            /// </summary>
            Run,
            /// <summary>
            /// 单线程读取队列节点
            /// </summary>
            SingleDequeueNode,
            /// <summary>
            /// SQL客户端添加数据
            /// </summary>
            SqlClientInserter,
            /// <summary>
            /// SQL客户端获取数据
            /// </summary>
            SqlClientSelector,
            /// <summary>
            /// 任务队列执行任务
            /// </summary>
            TaskQueueRun,
            /// <summary>
            /// 任务处理执行任务
            /// </summary>
            TaskRun,
            /// <summary>
            /// TCP客户端命令接收数据回调处理
            /// </summary>
            TcpCommandClientCommandOnRecieveData,
            /// <summary>
            /// TCP调用客户端路由创建客户端
            /// </summary>
            TcpCommandClientRouterClientCreator,
            /// <summary>
            /// TCP客户端命令流处理套接字创建命令输入数据并执行
            /// </summary>
            TcpCommandClientSocketBuildCommand,
            /// <summary>
            /// TCP客户端命令流处理套接字连接检测
            /// </summary>
            TcpCommandClientSocketCheck,
            /// <summary>
            /// 创建TCP客户端命令流处理套接字
            /// </summary>
            TcpCommandClientSocketCreator,
            /// <summary>
            /// 释放TCP客户端命令流处理套接字
            /// </summary>
            TcpCommandClientSocketDispose,
            /// <summary>
            /// TCP客户端命令接收数据
            /// </summary>
            TcpCommandClientSocketReceiver,
            /// <summary>
            /// TCP客户端命令流处理套接字接收会话标识
            /// </summary>
            TcpCommandClientSocketReceiveNextIdentity,
            /// <summary>
            /// TCP客户端命令流处理套接字同步接收服务器端数据
            /// </summary>
            TcpCommandClientSocketSynchronousReceive,
            /// <summary>
            /// TCP调用负载均衡服务端检测任务
            /// </summary>
            TcpCommandLoadBalancingServerCheck,
            /// <summary>
            /// TCP调用负载均衡服务添加TCP调用服务端
            /// </summary>
            TcpCommandLoadBalancingServerInfo,
            /// <summary>
            /// TCP调用服务添加负载均衡服务
            /// </summary>
            TcpCommandServerLoadBalancing,
            /// <summary>
            /// TCP调用服务端负载均衡联通测试
            /// </summary>
            TcpCommandServerLoadBalancingCheck,
            /// <summary>
            /// TCP服务器端调用
            /// </summary>
            TcpCommandServerSocketCall,
            /// <summary>
            /// TCP调用套接字同步接收命令
            /// </summary>
            TcpCommandServerSocketReceiveCommand,
            /// <summary>
            /// 调用套接字同步创建输出数据并执行
            /// </summary>
            TcpCommandServerSocketBuildOutput,
            /// <summary>
            /// 启动TCP注册服务客户端
            /// </summary>
            TcpRegisterClientStart,
            /// <summary>
            /// 保存TCP服务信息集合到缓存文件
            /// </summary>
            TcpRegisterSaveCacheFile,
            /// <summary>
            /// TCP调用服务获取客户端请求
            /// </summary>
            TcpServerGetSocket,
            /// <summary>
            /// TCP调用服务获取客户端请求
            /// </summary>
            TcpServerGetSocketMany,
            /// <summary>
            /// 超时队列检测
            /// </summary>
            TimeoutQueueCheck,
            /// <summary>
            /// 超时队列检测
            /// </summary>
            NetTimeoutQueueCheck,
            /// <summary>
            /// 定时任务执行任务
            /// </summary>
            TimerTaskRun,
            /// <summary>
            /// 定时任务执行任务
            /// </summary>
            TimerCancelTaskRun
        }
        /// <summary>
        /// 错误处理调用类型
        /// </summary>
        public enum errorType : byte
        {
            None,
            /// <summary>
            /// 委托回调
            /// </summary>
            Action,
        }
        /// <summary>
        /// 调用信息
        /// </summary>
        public struct call
        {
            /// <summary>
            /// 任务委托
            /// </summary>
            public object Value;
            /// <summary>
            /// 调用类型
            /// </summary>
            public callType Type;
            /// <summary>
            /// 调用信息
            /// </summary>
            /// <param name="value"></param>
            /// <param name="type"></param>
            public void Set(object value, callType type)
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
                //Console.WriteLine(Type);
                switch (Type)
                {
                    case callType.Action: new unionType { Value = Value }.Action(); return;
                    case callType.CheckSqlConnection: fastCSharp.sql.connection.CheckConnection(new unionType { Value = Value }.Assembly); return;
                    case callType.CheckSqlConnectionArray: fastCSharp.sql.connection.CheckConnection(new unionType { Value = Value }.AssemblyArray); return;
                    case callType.CreateFlieTimeoutWatcherCheckTimeout: new unionType { Value = Value }.CreateFlieTimeoutWatcher.CheckTimeout(); return;
                    case callType.DomainUnloadRemoveLast: fastCSharp.domainUnload.RemoveLast((fastCSharp.domainUnload.unload)Value); return;
                    case callType.DomainUnloadRemoveLastRun: fastCSharp.domainUnload.RemoveLastRun((fastCSharp.domainUnload.unload)Value); return;
                    case callType.FileBlockStreamReader: new unionType { Value = Value }.FileBlockStreamReader.Read(); return;
                    case callType.FileBlockStreamReaderWait: new unionType { Value = Value }.FileBlockStreamReader.Wait(); return;
                    case callType.FileStreamWriteFile: new unionType { Value = Value }.FileStreamWriter.WriteFile(); return;
                    case callType.FileStreamWriterCheckFlush: new unionType { Value = Value }.FileStreamWriter.CheckFlush(); return;
                    case callType.LogDispose: new unionType { Value = Value }.Log.Dispose(); return;
                    case callType.LogStreamCallback: new unionType { Value = Value }.LogStream.Callback(); return;
                    case callType.HttpSocketBoundaryIdentityReceiverWriteFile: new unionType { Value = Value }.HttpSocketBoundaryIdentityReceiver.WriteFile(); return;
                    case callType.HttpServersFileWatcherTimeout: new unionType { Value = Value }.HttpServers.FileWatcherTimeout(); return;
                    case callType.HttpServersSave: new unionType { Value = Value }.HttpServers.Save(); return;
                    case callType.HttpSessionRefreshTimeout: new unionType { Value = Value }.HttpSession.RefreshTimeout(); return;
                    case callType.MemoryDatabasePhysicalDispose: new unionType { Value = Value }.MemoryDatabasePhysical.Dispose(); return;
                    case callType.MemoryDatabasePhysicalFileReader: new unionType { Value = Value }.MemoryDatabasePhysicalFileReader.ReadThread(); return;
                    case callType.MemoryDatabaseTableRemoteTableLoadEnd: new unionType { Value = Value }.MemoryDatabaseTableRemoteTableLoader.End(); return;
                    case callType.MemoryDatabaseTableRemoteTableLoaded: new unionType { Value = Value }.MemoryDatabaseTableRemoteTableLoader.Loaded(); return;
                    case callType.ProcessCopyClient: fastCSharp.diagnostics.processCopyServer.CallGuard(); return;
                    case callType.Run: new unionType { Value = Value }.Run.Run(); return;
                    case callType.SingleDequeueNode: new unionType { Value = Value }.SingleDequeueNode.Thread(); return;
                    case callType.SqlClientInserter: new unionType { Value = Value }.SqlClientInserter.Insert(); return;
                    case callType.SqlClientSelector: new unionType { Value = Value }.SqlClientSelector.GetData(); return;
                    case callType.TaskQueueRun: new unionType { Value = Value }.TaskQueue.Run(); return;
                    case callType.TaskRun: new unionType { Value = Value }.Task.Run(); return;
                    case callType.TcpCommandClientCommandOnRecieveData: new unionType { Value = Value }.TcpCommandClientCommand.OnRecieveData(); return;
                    case callType.TcpCommandClientRouterClientCreator: new unionType { Value = Value }.TcpCommandClientRouterClientCreator.CreateThread(); return;
                    case callType.TcpCommandClientSocketBuildCommand: new unionType { Value = Value }.TcpCommandClientSocket.BuildCommand(); return;
                    case callType.TcpCommandClientSocketCheck: new unionType { Value = Value }.TcpCommandClientSocket.Check(); return;
                    case callType.TcpCommandClientSocketCreator: new unionType { Value = Value }.TcpCommandClientSocketCreator.Create(); return;
                    case callType.TcpCommandClientSocketDispose: new unionType { Value = Value }.TcpCommandClientSocket.Dispose(); return;
                    case callType.TcpCommandClientSocketReceiver: new unionType { Value = Value }.TcpCommandClientSocketReceiver.Receive(); return;
                    case callType.TcpCommandClientSocketReceiveNextIdentity: new unionType { Value = Value }.TcpCommandClientSocket.ReceiveNextIdentity(); return;
                    case callType.TcpCommandClientSocketSynchronousReceive: new unionType { Value = Value }.TcpCommandClientSocket.SynchronousReceive(); return;
                    case callType.TcpCommandLoadBalancingServerCheck: new unionType { Value = Value }.TcpCommandLoadBalancingServer.Check(); return;
                    case callType.TcpCommandLoadBalancingServerInfo: new unionType { Value = Value }.TcpCommandLoadBalancingServerInfo.NewServer(); return;
                    case callType.TcpCommandServerSocketCall: new unionType { Value = Value }.TcpCommandServerSocketCall.Call(); return;
                    case callType.TcpCommandServerLoadBalancing: new unionType { Value = Value }.TcpCommandServer.LoadBalancing(); return;
                    case callType.TcpCommandServerLoadBalancingCheck: new unionType { Value = Value }.TcpCommandServer.LoadBalancingCheck(); return;
                    case callType.TcpCommandServerSocketReceiveCommand: new unionType { Value = Value }.TcpCommandServerSocket.ReceiveCommand(); return;
                    case callType.TcpCommandServerSocketBuildOutput: new unionType { Value = Value }.TcpCommandServerSocket.BuildOutput(); return;
                    case callType.TcpRegisterClientStart: new unionType { Value = Value }.TcpRegisterClient.Start(); return;
                    case callType.TcpRegisterSaveCacheFile: new unionType { Value = Value }.TcpRegister.SaveCacheFile(); return;
                    case callType.TcpServerGetSocket: new unionType { Value = Value }.TcpServer.GetSocket(); return;
                    case callType.TcpServerGetSocketMany: new unionType { Value = Value }.TcpServer.GetSocketMany(); return;
                    case callType.TimeoutQueueCheck: new unionType { Value = Value }.TimeoutQueue.Check(); return;
                    case callType.NetTimeoutQueueCheck: new unionType { Value = Value }.NetTimeoutQueue.Check(); return;
                    case callType.TimerTaskRun: new unionType { Value = Value }.TimerTask.Run(); return;
                    case callType.TimerCancelTaskRun: new unionType { Value = Value }.TimerCancelTask.Run(); return;
                }
            }
        }
        /// <summary>
        /// 线程池线程集合
        /// </summary>
        private static readonly indexPool<Thread> threads = new indexPool<Thread>();
        /// <summary>
        /// 线程池线程默认堆栈帧数
        /// </summary>
        internal static int DefaultFrameCount;
        /// <summary>
        /// 活动的线程池线程集合
        /// </summary>
        public static subArray<Thread> Threads
        {
            get
            {
                return threads.GetArray();
            }
        }
        /// <summary>
        /// 线程池
        /// </summary>
        private readonly threadPool threadPool;
        /// <summary>
        /// 线程句柄
        /// </summary>
        private readonly Thread threadHandle;
        /// <summary>
        /// 等待事件
        /// </summary>
        private autoWaitHandle waitHandle;
        /// <summary>
        /// 线程ID
        /// </summary>
        public int ManagedThreadId
        {
            get { return threadHandle.ManagedThreadId; }
        }
        /// <summary>
        /// 任务委托
        /// </summary>
        private call task;
        /// <summary>
        /// 应用程序退出处理
        /// </summary>
        private domainUnload.unload domainUnload;
        /// <summary>
        /// 应用程序退出处理
        /// </summary>
        private object onError;
        /// <summary>
        /// 线程索引位置
        /// </summary>
        private int threadIndex;
        /// <summary>
        /// 应用程序退出处理调用类型
        /// </summary>
        private errorType onErrorType;
        /// <summary>
        /// 线程池线程
        /// </summary>
        /// <param name="threadPool">线程池</param>
        /// <param name="stackSize">堆栈大小</param>
        /// <param name="task">任务委托</param>
        /// <param name="taskType">任务委托调用类型</param>
        internal thread(threadPool threadPool, int stackSize, object task, callType taskType)
        {
            this.task.Set(task, taskType);
            this.domainUnload.Type = fastCSharp.domainUnload.unloadType.None;
            this.onErrorType = errorType.None;
            waitHandle = new autoWaitHandle(false);
            this.threadPool = threadPool;
            threadHandle = new Thread(run, stackSize);
            threadHandle.IsBackground = true;
            threadIndex = threads.Push(threadHandle);
            threadHandle.Start();
        }
        /// <summary>
        /// 线程池线程
        /// </summary>
        /// <param name="threadPool">线程池</param>
        /// <param name="stackSize">堆栈大小</param>
        /// <param name="task">任务委托</param>
        /// <param name="onError">应用程序退出处理</param>
        /// <param name="taskType">任务委托调用类型</param>
        /// <param name="onErrorType">应用程序退出处理调用类型</param>
        internal thread(threadPool threadPool, int stackSize, object task, object onError, callType taskType, errorType onErrorType)
        {
            this.task.Set(task, taskType);
            this.onError = onError;
            this.domainUnload.Type = fastCSharp.domainUnload.unloadType.None;
            this.onErrorType = onErrorType;
            waitHandle = new autoWaitHandle(false);
            this.threadPool = threadPool;
            threadHandle = new Thread(run, stackSize);
            threadHandle.IsBackground = true;
            threadIndex = threads.Push(threadHandle);
            threadHandle.Start();
        }
        /// <summary>
        /// 线程池线程
        /// </summary>
        /// <param name="threadPool">线程池</param>
        /// <param name="stackSize">堆栈大小</param>
        /// <param name="task">任务委托</param>
        /// <param name="domainUnload">应用程序退出处理</param>
        /// <param name="onError">应用程序退出处理</param>
        /// <param name="taskType">任务委托调用类型</param>
        /// <param name="domainUnloadType">应用程序退出处理调用类型</param>
        /// <param name="onErrorType">应用程序退出处理调用类型</param>
        internal thread(threadPool threadPool, int stackSize, object task, object onError, object domainUnload, callType taskType, errorType onErrorType, domainUnload.unloadType domainUnloadType)
        {
            this.task.Set(task, taskType);
            this.domainUnload.Set(domainUnload, domainUnloadType);
            this.onError = onError;
            this.onErrorType = onErrorType;
            waitHandle = new autoWaitHandle(false);
            this.threadPool = threadPool;
            threadHandle = new Thread(run, stackSize);
            threadHandle.IsBackground = true;
            threadIndex = threads.Push(threadHandle);
            threadHandle.Start();
        }
        /// <summary>
        /// 运行线程
        /// </summary>
        private void run()
        {
#pragma warning disable 618
#pragma warning disable 612
            if (DefaultFrameCount == 0) DefaultFrameCount = new System.Diagnostics.StackTrace(threadHandle, false).FrameCount;
#pragma warning restore 612
#pragma warning restore 618
            do
            {
                try
                {
                    do
                    {
                        if (domainUnload.Type == fastCSharp.domainUnload.unloadType.None) task.Call();
                        else
                        {
                            fastCSharp.domainUnload.Add(ref domainUnload);
                            task.Call();
                            domainUnload.Type = fastCSharp.domainUnload.unloadType.None;
                            fastCSharp.domainUnload.Remove(ref domainUnload);
                        }
                        task.Value = null;
                        onError = null;
                        threadPool.Push(this);
                        waitHandle.Wait();
                    }
                    while (task.Type != callType.None);
                    threads.UnsafeFree(threadIndex);
                    return;
                }
                catch (Exception error)
                {
                    try
                    {
                        switch (onErrorType)
                        {
                            case errorType.Action: new unionType { Value = onError }.ActionException(error); break;
                            default: log.Error.Add(error, null, false); break;
                        }
                    }
                    catch (Exception error1)
                    {
                        log.Error.Add(error1, null, false);
                    }
                }
                finally
                {
                    task.Value = null;
                    onError = null;
                    if (domainUnload.Type != fastCSharp.domainUnload.unloadType.None) fastCSharp.domainUnload.Remove(ref domainUnload);
                }
                threadPool.Push(this);
                waitHandle.Wait();
            }
            while (task.Type != callType.None);
            //do
            //{
            //    if (domainUnload.Type != fastCSharp.domainUnload.unloadType.None) fastCSharp.domainUnload.Add(ref domainUnload);
            //    try
            //    {
            //        task.Call();
            //    }
            //    catch (Exception error)
            //    {
            //        try
            //        {
            //            switch (onErrorType)
            //            {
            //                case errorType.Action: new unionType { Value = onError }.ExceptionAction(error); break;
            //                default: log.Error.Add(error, null, false); break;
            //            }
            //        }
            //        catch (Exception error1)
            //        {
            //            log.Error.Add(error1, null, false);
            //        }
            //    }
            //    finally
            //    {
            //        task.Value = null;
            //        onError = null;
            //        if (domainUnload.Type != fastCSharp.domainUnload.unloadType.None) fastCSharp.domainUnload.Remove(ref domainUnload);
            //    }
            //    threadPool.Push(this);
            //    waitHandle.Wait();
            //}
            //while (task.Type != callType.None);
            threads.UnsafeFree(threadIndex);
        }
        /// <summary>
        /// 结束线程
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void Stop()
        {
            task.Type = callType.None;
            waitHandle.Set();
        }
        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="task">任务委托</param>
        /// <param name="taskType">任务委托调用类型</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void RunTask(object task, callType taskType)
        {
            this.domainUnload.Type = fastCSharp.domainUnload.unloadType.None;
            this.onErrorType = errorType.None;
            this.task.Set(task, taskType);
            waitHandle.Set();
        }
        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="task">任务委托</param>
        /// <param name="onError">应用程序退出处理</param>
        /// <param name="taskType">任务委托调用类型</param>
        /// <param name="onErrorType">应用程序退出处理调用类型</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void RunTask(object task, object onError, callType taskType, errorType onErrorType)
        {
            this.onError = onError;
            this.domainUnload.Type = fastCSharp.domainUnload.unloadType.None;
            this.onErrorType = onErrorType;
            this.task.Set(task, taskType);
            waitHandle.Set();
        }
        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="task">任务委托</param>
        /// <param name="domainUnload">应用程序退出处理</param>
        /// <param name="onError">应用程序退出处理</param>
        /// <param name="taskType">任务委托调用类型</param>
        /// <param name="domainUnloadType">应用程序退出处理调用类型</param>
        /// <param name="onErrorType">应用程序退出处理调用类型</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void RunTask(object task, object onError, object domainUnload, callType taskType, errorType onErrorType, domainUnload.unloadType domainUnloadType)
        {
            this.domainUnload.Set(domainUnload, domainUnloadType);
            this.onError = onError;
            this.onErrorType = onErrorType;
            this.task.Set(task, taskType);
            waitHandle.Set();
        }
    }
}
