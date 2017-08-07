using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using fastCSharp.threading;

namespace fastCSharp.io
{
    /// <summary>
    /// 新建文件监视
    /// </summary>
    public sealed class createFlieTimeoutWatcher : IDisposable
    {
        /// <summary>
        /// 监视计数
        /// </summary>
        private struct counter
        {
            /// <summary>
            /// 文件监视器
            /// </summary>
            public FileSystemWatcher Watcher;
            /// <summary>
            /// 监视计数
            /// </summary>
            public int Count;
            /// <summary>
            /// 文件监视器初始化
            /// </summary>
            /// <param name="path">监视路径</param>
            /// <param name="onCreated">新建文件处理</param>
            public void Create(string path, FileSystemEventHandler onCreated)
            {
                Watcher = new FileSystemWatcher(path);
                Watcher.IncludeSubdirectories = false;
                Watcher.EnableRaisingEvents = true;
                Watcher.Created += onCreated;
                Count = 1;
            }
        }
        /// <summary>
        /// 超时处理类型
        /// </summary>
        internal enum timeoutType
        {
            /// <summary>
            /// 
            /// </summary>
            Action,
            /// <summary>
            /// HTTP服务器
            /// </summary>
            HttpServers
        }
        /// <summary>
        /// 超时检测时钟周期
        /// </summary>
        private long timeoutTicks;
        /// <summary>
        /// 超时处理
        /// </summary>
        private object onTimeout;
        /// <summary>
        /// 超时处理类型
        /// </summary>
        private timeoutType onTimeoutType;
        /// <summary>
        /// 新建文件前置过滤
        /// </summary>
        private Func<FileSystemEventArgs, bool> filter;
        /// <summary>
        /// 新建文件处理
        /// </summary>
        private FileSystemEventHandler onCreatedHandle;
        /// <summary>
        /// 文件监视器集合
        /// </summary>
        private Dictionary<hashString, counter> watchers;
        /// <summary>
        /// 超时检测文件集合
        /// </summary>
        private subArray<keyValue<FileInfo, DateTime>> files;
        /// <summary>
        /// 文件监视器集合访问锁
        /// </summary>
        private readonly object watcherLock = new object();
        /// <summary>
        /// 超时检测文件集合访问锁
        /// </summary>
        private readonly object fileLock = new object();
        /// <summary>
        /// 超时检测访问锁
        /// </summary>
        private int checkLock;
        /// <summary>
        /// 是否释放资源
        /// </summary>
        private int isDisposed;
        /// <summary>
        /// 新建文件监视
        /// </summary>
        /// <param name="seconds">超时检测秒数</param>
        /// <param name="onTimeout">超时处理</param>
        /// <param name="filter">新建文件前置过滤</param>
        public createFlieTimeoutWatcher(int seconds, Action onTimeout, Func<FileSystemEventArgs, bool> filter = null)
            : this(seconds, onTimeout, timeoutType.Action, filter)
        {
        }
        /// <summary>
        /// 新建文件监视
        /// </summary>
        /// <param name="seconds">超时检测秒数</param>
        /// <param name="onTimeout">超时处理</param>
        /// <param name="onTimeoutType"></param>
        /// <param name="filter">新建文件前置过滤</param>
        internal createFlieTimeoutWatcher(int seconds, object onTimeout, timeoutType onTimeoutType, Func<FileSystemEventArgs, bool> filter = null)
        {
            if (onTimeout == null) log.Error.Throw(log.exceptionType.Null);
            timeoutTicks = new TimeSpan(0, 0, seconds > 0 ? (seconds + 1) : 2).Ticks;
            this.onTimeout = onTimeout;
            this.onTimeoutType = onTimeoutType;
            this.filter = filter;
            onCreatedHandle = filter == null ? (FileSystemEventHandler)onCreated : onCreatedFilter;
            watchers = dictionary.CreateHashString<counter>();
        }
        /// <summary>
        /// 添加监视路径
        /// </summary>
        /// <param name="path">监视路径</param>
        public void Add(string path)
        {
            if (isDisposed == 0)
            {
                path = fastCSharp.io.file.FileNameToLower(path);
                counter counter;
                hashString pathKey = path;
                Monitor.Enter(watcherLock);
                try
                {
                    if (watchers.TryGetValue(pathKey, out counter))
                    {
                        ++counter.Count;
                        watchers[pathKey] = counter;
                    }
                    else
                    {
                        counter.Create(path, onCreatedHandle);
                        watchers.Add(pathKey, counter);
                    }
                }
                finally { Monitor.Exit(watcherLock); }
            }
        }
        /// <summary>
        /// 删除监视路径
        /// </summary>
        /// <param name="path">监视路径</param>
        public void Remove(string path)
        {
            path = fastCSharp.io.file.FileNameToLower(path);
            counter counter;
            hashString pathKey = path;
            Monitor.Enter(watcherLock);
            try
            {
                if (watchers.TryGetValue(pathKey, out counter))
                {
                    if (--counter.Count == 0) watchers.Remove(pathKey);
                    else watchers[pathKey] = counter;
                }
            }
            finally { Monitor.Exit(watcherLock); }
            if (counter.Count == 0 && counter.Watcher != null) dispose(counter.Watcher);
        }
        /// <summary>
        /// 新建文件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onCreated(object sender, FileSystemEventArgs e)
        {
            try
            {
                onCreated(e);
            }
            catch (Exception error)
            {
                fastCSharp.log.Default.Add(error, null, false);
            }
        }
        /// <summary>
        /// 新建文件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onCreatedFilter(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (filter(e)) onCreated(e);
            }
            catch (Exception error)
            {
                fastCSharp.log.Default.Add(error, null, false);
            }
        }
        /// <summary>
        /// 新建文件处理
        /// </summary>
        /// <param name="e"></param>
        private void onCreated(FileSystemEventArgs e)
        {
            if (isDisposed == 0)
            {
                FileInfo file = new FileInfo(e.FullPath);
                if (file.Exists)
                {
                    DateTime timeout = date.nowTime.Now.AddTicks(timeoutTicks);
                    Monitor.Enter(fileLock);
                    try
                    {
                        files.Add(new keyValue<FileInfo, DateTime>(file, timeout));
                    }
                    finally { Monitor.Exit(fileLock); }
                    if (Interlocked.CompareExchange(ref checkLock, 1, 0) == 0) timerTask.Default.Add(this, thread.callType.CreateFlieTimeoutWatcherCheckTimeout, date.nowTime.Now.AddTicks(timeoutTicks));
                }
            }
        }
        /// <summary>
        /// 超时检测处理
        /// </summary>
        internal void CheckTimeout()
        {
            if (isDisposed == 0)
            {
                DateTime now = date.nowTime.Now;
                int index = 0;
                Monitor.Enter(fileLock);
                int count = files.length;
                keyValue<FileInfo, DateTime>[] fileArray = files.array;
                try
                {
                    while (index != count)
                    {
                        keyValue<FileInfo, DateTime> fileTime = fileArray[index];
                        if (fileTime.Value <= now)
                        {
                            FileInfo file = fileTime.Key;
                            long length = file.Length;
                            file.Refresh();
                            if (file.Exists)
                            {
                                if (length == file.Length)
                                {
                                    try
                                    {
                                        using (FileStream fileStream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                                        {
                                            fileArray[index--] = fileArray[--count];
                                        }
                                    }
                                    catch { }
                                }
                            }
                            else fileArray[index--] = fileArray[--count];
                        }
                        ++index;
                    }
                    files.UnsafeSetLength(count);
                }
                finally
                {
                    Monitor.Exit(fileLock);
                    if (count == 0)
                    {
                        try
                        {
                            if (isDisposed == 0)
                            {
                                switch (onTimeoutType)
                                {
                                    case timeoutType.Action: new unionType { Value = onTimeout }.Action(); break;
                                    case timeoutType.HttpServers: new unionType { Value = onTimeout }.HttpServers.OnFileWatcherTimeout(); break;
                                }
                            }
                        }
                        finally { checkLock = 0; }
                    }
                    else if (isDisposed == 0) timerTask.Default.Add(this, thread.callType.CreateFlieTimeoutWatcherCheckTimeout, date.nowTime.Now.AddTicks(timeoutTicks));
                }
            }
        }
        /// <summary>
        /// 关闭文件监视器
        /// </summary>
        /// <param name="watcher">文件监视器</param>
        private void dispose(FileSystemWatcher watcher)
        {
#if XAMARIN
#else
            using (watcher)
#endif
            {
                watcher.EnableRaisingEvents = false;
                watcher.Created -= onCreatedHandle;
            }
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            isDisposed = 1;
            counter[] counters = nullValue<counter>.Array;
            Monitor.Enter(watcherLock);
            try
            {
                if (watchers.Count != 0)
                {
                    counters = watchers.Values.getArray();
                    watchers.Clear();
                }
            }
            finally { Monitor.Exit(watcherLock); }
            foreach (counter counter in counters) dispose(counter.Watcher);
        }
    }
}
