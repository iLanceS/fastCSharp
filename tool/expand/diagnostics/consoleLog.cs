using System;
using System.IO;
using System.Threading;
using fastCSharp.threading;
using fastCSharp.io;
using System.Diagnostics;
using fastCSharp.net.tcp;

namespace fastCSharp.diagnostics
{
    /// <summary>
    /// 控制台日志处理
    /// </summary>
    public abstract class consoleLog : IDisposable
    {
        /// <summary>
        /// TCP忽略退出
        /// </summary>
        private sealed class tcpIgnoreGroup
        {
            /// <summary>
            /// 控制台日志处理
            /// </summary>
            public consoleLog Console;
            /// <summary>
            /// TCP调用服务端
            /// </summary>
            public Func<commandServer> GetServer;
            /// <summary>
            /// 当前进程是否正在尝试忽略服务端操作
            /// </summary>
            public Func<bool> IsTryIgnoreServer;
            /// <summary>
            /// TCP忽略事件
            /// </summary>
            public Action OnIgnoreGroup;
            /// <summary>
            /// TCP忽略检测操作
            /// </summary>
            /// <param name="groupId"></param>
            public void IgnoreGroup(int groupId)
            {
                if (groupId == 0 && !IsTryIgnoreServer())
                {
                    try
                    {
                        processCopyServer.Remove();
                    }
                    finally
                    {
                        commandServer server = GetServer();
                        if (server != null) server.StopListen();
                        if (OnIgnoreGroup != null) OnIgnoreGroup();
                        fastCSharp.threading.threadPool.TinyPool.Start(exit);
                    }
                }
            }
            /// <summary>
            /// 程序结束退出
            /// </summary>
            private void exit()
            {
                Console.Dispose();
                Thread.Sleep(1000);
                Environment.Exit(-1);
            }
        }
        /// <summary>
        /// 进程复制重启文件监视
        /// </summary>
        private createFlieTimeoutWatcher fileWatcher;
        /// <summary>
        /// 文件监视是否超时
        /// </summary>
        private int isFileWatcherTimeout;
        /// <summary>
        /// 是否自动设置进程复制重启
        /// </summary>
        protected virtual bool isSetProcessCopy
        {
            get { return true; }
        }
        /// <summary>
        /// 控制台日志处理
        /// </summary>
        protected consoleLog()
        {
            AppDomain.CurrentDomain.UnhandledException += unhandledException;
            domainUnload.Add(Dispose);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            pub.Dispose(ref fileWatcher);
            dispose();
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        protected abstract void dispose();
        /// <summary>
        /// 启动处理
        /// </summary>
        public void Start()
        {
            if (fastCSharp.config.pub.Default.IsDebug) output("警告：Debug模式");
            if (fastCSharp.config.pub.Default.IsService) threadPool.TinyPool.Start(start);
            else
            {
                output("非服务模式启动");
                start();
                if (isSetProcessCopy) setProcessCopy();
            }
        }
        /// <summary>
        /// 启动处理
        /// </summary>
        protected abstract void start();
        /// <summary>
        /// 输出信息
        /// </summary>
        /// <param name="message">输出信息</param>
        protected void output(string message)
        {
            if (!fastCSharp.config.pub.Default.IsService)
            {
                Console.WriteLine(date.Now.toString());
                Console.WriteLine(message);
            }
            log.Default.Add(message, new System.Diagnostics.StackFrame(), false);
        }
        /// <summary>
        /// 输出错误
        /// </summary>
        /// <param name="error">输出错误</param>
        protected void output(Exception error)
        {
            if (!fastCSharp.config.pub.Default.IsService)
            {
                Console.WriteLine(date.Now.toString());
                Console.WriteLine(error.ToString());
            }
            log.Error.Add(error, null, false);
        }
        /// <summary>
        /// 启动进程
        /// </summary>
        /// <param name="process">进程信息</param>
        protected void startProcess(string process)
        {
            string arguments = null;
            int index = process.IndexOf('|');
            if (index != -1)
            {
                arguments = process.Substring(index + 1);
                process = process.Substring(0, index);
            }
            FileInfo file = new FileInfo(process);
            if (file.Exists)
            {
                if (fastCSharp.diagnostics.process.Count(fastCSharp.diagnostics.process.GetProcessName(file)) == 0)
                {
                    fastCSharp.diagnostics.process.StartDirectory(process, arguments);
                    output("启动进程 : " + file.Name);
                }
                else output("进程已存在 : " + file.Name);
            }
            else output("文件不存在 : " + process);
        }
        /// <summary>
        /// 输出异常
        /// </summary>
        /// <param name="errorString">异常信息</param>
        private void outputException(string errorString)
        {
            Console.WriteLine();
            Console.WriteLine(date.NowSecond.toString());
            Console.WriteLine(errorString);
            try
            {
                File.AppendAllText("CRASH " + date.NowSecond.ToString("yyyyMMdd HHmmss") + ".txt", errorString + @"
");
            }
            catch (Exception error)
            {
                Console.WriteLine();
                Console.WriteLine(error.ToString());
            }
        }
        /// <summary>
        /// 异常重启
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="exception"></param>
        private void unhandledException(object sender, UnhandledExceptionEventArgs exception)
        {
            object exceptionObject = exception.ExceptionObject;
            outputException(exceptionObject == null ? exception.ToString() : exceptionObject.ToString());
            try
            {
                fastCSharp.diagnostics.process.ReStart();
            }
            catch (Exception error)
            {
                outputException(error.ToString());
            }
            try
            {
                Dispose();
            }
            catch (Exception error)
            {
                outputException(error.ToString());
            }
        }
        /// <summary>
        /// 设置进程复制重启
        /// </summary>
        private void setProcessCopy()
        {
            if (!fastCSharp.config.pub.Default.IsService && fastCSharp.config.processCopy.Default.WatcherPath != null)
            {
                try
                {
                    fileWatcher = new fastCSharp.io.createFlieTimeoutWatcher(fastCSharp.config.processCopy.Default.CheckTimeoutSeconds, onFileWatcherTimeout, fastCSharp.diagnostics.processCopyServer.DefaultFileWatcherFilter);
                    fileWatcher.Add(fastCSharp.config.processCopy.Default.WatcherPath);
                }
                catch (Exception error)
                {
                    log.Error.Add(error, fastCSharp.config.processCopy.Default.WatcherPath, false);
                }
                threadPool.TinyPool.Start(processCopyServer.Guard);
            }
        }
        /// <summary>
        /// 删除进程复制重启
        /// </summary>
        protected virtual void removeProcessCopy()
        {
            if (isSetProcessCopy) processCopyServer.Remove();
        }
        /// <summary>
        /// 文件监视超时处理
        /// </summary>
        private void onFileWatcherTimeout()
        {
            if (Interlocked.CompareExchange(ref isFileWatcherTimeout, 1, 0) == 0) fileWatcherTimeout();
        }
        /// <summary>
        /// 文件监视超时处理
        /// </summary>
        private void fileWatcherTimeout()
        {
            if (processCopyServer.CopyStart())
            {
                Dispose();
                Environment.Exit(-1);
            }
            else
            {
                timerTask.Default.Add(fileWatcherTimeout, date.NowSecond.AddSeconds(fastCSharp.config.processCopy.Default.CheckTimeoutSeconds));
            }
        }
        /// <summary>
        /// TCP忽略退出
        /// </summary>
        /// <param name="getServer">TCP调用服务端</param>
        /// <param name="isTryIgnoreServer">当前进程是否正在尝试忽略服务端操作</param>
        /// <param name="onIgnoreGroup">TCP忽略事件</param>
        protected void setTcpIgnoreGroup(Func<commandServer> getServer, Func<bool> isTryIgnoreServer, Action onIgnoreGroup = null)
        {
            getServer().OnIgnoreGroup += new tcpIgnoreGroup { Console = this, GetServer = getServer, IsTryIgnoreServer = isTryIgnoreServer, OnIgnoreGroup = onIgnoreGroup }.IgnoreGroup;
        }
        /// <summary>
        /// 启动控制台服务
        /// </summary>
        /// <typeparam name="serverType">服务类型</typeparam>
        /// <param name="server">服务实例</param>
        public static void Start<serverType>(serverType server) where serverType : consoleLog
        {
            server.Start();
            try
            {
                do
                {
                    string command = Console.ReadLine();
                    int parameterIndex = command.IndexOf(' ');
                    switch (parameterIndex == -1 ? command : command.Substring(0, parameterIndex))
                    {
                        case "quit":
                            server.removeProcessCopy();
                            return;
                        case "thread":
                            Console.WriteLine("fastCSharp.threading.threadPool.CheckLog START");
                            try
                            {
                                fastCSharp.threading.threadPool.CheckLog();
                            }
                            catch (Exception error)
                            {
                                Console.WriteLine(error.ToString());
                            }
                            finally
                            {
                                Console.WriteLine("fastCSharp.threading.threadPool.CheckLog FINALLY");
                            }
                            break;
                        case "gc":
                            int count = 0;
                            if (parameterIndex != -1 && int.TryParse(command.Substring(parameterIndex + 1), out count) && count < 0) count = 0;
                            unmanagedPool.ClearPool(count);
                            memoryPool.ClearPool(count);
                            typePool.ClearPool(count);
                            GC.Collect();
                            break;
                    }
                }
                while (true);
            }
            finally { pub.Dispose(ref server); }
        }
    }
}
