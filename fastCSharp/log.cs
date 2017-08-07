using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.IO;
using fastCSharp.threading;
using fastCSharp.io;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 日志处理
    /// </summary>
    public sealed class log : IDisposable
    {
        /// <summary>
        /// 异常类型
        /// </summary>
        public enum exceptionType
        {
            /// <summary>
            /// 没有异常(不输出)
            /// </summary>
            None,
            /// <summary>
            /// fastCSharp项目编译未启用代码生成
            /// </summary>
            NotFastCSharpCode,
            /// <summary>
            /// 关键值为空
            /// </summary>
            Null,
            /// <summary>
            /// 索引超出范围
            /// </summary>
            IndexOutOfRange,
            /// <summary>
            /// 操作不可用
            /// </summary>
            ErrorOperation,
            /// <summary>
            /// 未知错误
            /// </summary>
            Unknown
        }
        /// <summary>
        /// 缓存类型
        /// </summary>
        public enum cacheType
        {
            /// <summary>
            /// 不缓存
            /// </summary>
            None,
            /// <summary>
            /// 先进先出
            /// </summary>
            Queue,
            /// <summary>
            /// 最后一次
            /// </summary>
            Last,
        }
        /// <summary>
        /// 日志信息
        /// </summary>
        private sealed class debug : queue<debug>.node
        {
            /// <summary>
            /// 调用堆栈
            /// </summary>
            public StackTrace StackTrace;
            /// <summary>
            /// 调用堆栈帧
            /// </summary>
            public StackFrame StackFrame;
            /// <summary>
            /// 提示信息
            /// </summary>
            public string Message;
            /// <summary>
            /// 错误异常
            /// </summary>
            public Exception Exception;
            /// <summary>
            /// 异常类型
            /// </summary>
            public exceptionType Type;
            /// <summary>
            /// 字符串
            /// </summary>
            public string toString;
            /// <summary>
            /// 字符串
            /// </summary>
            /// <returns>字符串</returns>
            public override string ToString()
            {
                if (toString == null)
                {
                    string stackFrameMethodTypeName = null, stackFrameMethodString = null, stackFrameFile = null, stackFrameLine = null, stackFrameColumn = null;
                    if (StackFrame != null)
                    {
                        MethodBase stackFrameMethod = StackFrame.GetMethod();
                        if (stackFrameMethod == null)
                        {
                            StackFrame = null;
                            if (StackTrace == null) StackTrace = new StackTrace(true);
                        }
                        else
                        {
                            stackFrameMethodTypeName = stackFrameMethod.ReflectedType.FullName;
                            stackFrameMethodString = stackFrameMethod.ToString();
                            stackFrameFile = StackFrame.GetFileName();
                            if (stackFrameFile != null)
                            {
                                stackFrameLine = StackFrame.GetFileLineNumber().toString();
                                stackFrameColumn = StackFrame.GetFileColumnNumber().toString();
                            }
                        }
                    }
                    string stackTrace = StackTrace == null ? null : StackTrace.ToString();
                    string exception = Exception == null ? null : Exception.ToString();
                    Monitor.Enter(toStringStreamLock);
                    try
                    {
                        if (Message != null)
                        {
                            toStringStream.SimpleWriteNotNull(@"
附加信息 : ");
                            toStringStream.WriteNotNull(Message);
                        }
                        if (StackFrame != null)
                        {
                            toStringStream.Write(@"
堆栈帧信息 : ");
                            toStringStream.WriteNotNull(stackFrameMethodTypeName);
                            toStringStream.SimpleWriteNotNull(" + ");
                            toStringStream.WriteNotNull(stackFrameMethodString);
                            if (stackFrameFile != null)
                            {
                                toStringStream.SimpleWriteNotNull(" in ");
                                toStringStream.WriteNotNull(stackFrameFile);
                                toStringStream.SimpleWriteNotNull(" line ");
                                toStringStream.SimpleWriteNotNull(stackFrameLine);
                                toStringStream.SimpleWriteNotNull(" col ");
                                toStringStream.SimpleWriteNotNull(stackFrameColumn);
                            }
                        }
                        if (stackTrace != null)
                        {
                            toStringStream.SimpleWriteNotNull(@"
堆栈信息 : ");
                            toStringStream.WriteNotNull(stackTrace);
                        }
                        if (exception != null)
                        {
                            toStringStream.SimpleWriteNotNull(@"
异常信息 : ");
                            toStringStream.WriteNotNull(exception);
                        }
                        if (Type != exceptionType.None)
                        {
                            toStringStream.SimpleWriteNotNull("异常类型 : ");
                            toStringStream.SimpleWriteNotNull(Type.ToString());
                        }
                        toString = toStringStream.ToString();
                    }
                    finally
                    {
                        toStringStream.Clear();
                        Monitor.Exit(toStringStreamLock);
                    }
                }
                return toString;
            }
        }
        /// <summary>
        /// 异常错误信息前缀
        /// </summary>
        public const string ExceptionPrefix = pub.fastCSharp + " Exception : ";
        /// <summary>
        /// 日志文件前缀
        /// </summary>
        public const string DefaultFilePrefix = "log_";
        /// <summary>
        /// 字符串转换流
        /// </summary>
        private static readonly charStream toStringStream = new charStream();
        /// <summary>
        /// 字符串转换流访问锁
        /// </summary>
        private static readonly object toStringStreamLock = new object();

        /// <summary>
        /// 日志文件流
        /// </summary>
        private fileStreamWriter fileStream;
        /// <summary>
        /// 日志文件流访问锁
        /// </summary>
        private readonly object fileLock = new object();
        /// <summary>
        /// 日志文件名
        /// </summary>
        private string fileName;
        /// <summary>
        /// 日志文件名
        /// </summary>
        public string FileName
        {
            get
            {
                if (isFieStream)
                {
                    string fileName = null;
                    Monitor.Enter(fileLock);
                    if (fileStream != null) fileName = fileStream.FileName;
                    Monitor.Exit(fileLock);
                    return fileName;
                }
                return null;
            }
        }
        /// <summary>
        /// 日志队列
        /// </summary>
        private readonly queue<debug>.concurrent.singleDequeueNode queue;
        /// <summary>
        /// 日志缓存队列
        /// </summary>
        private readonly fifoPriorityQueue<hashString, bool> cache = new fifoPriorityQueue<hashString, bool>();
        /// <summary>
        /// 最后一次输出缓存
        /// </summary>
        private hashString lastCache;
        /// <summary>
        /// 最大缓存数量
        /// </summary>
        private readonly int maxCacheCount;
        /// <summary>
        /// 日志缓存访问锁
        /// </summary>
        private readonly object cacheLock = new object();
        /// <summary>
        /// 是否已经释放资源
        /// </summary>
        private int isDisposed;
        /// <summary>
        /// 最大字节长度(小于等于0表示不限)
        /// </summary>
        public int MaxSize = fastCSharp.config.appSetting.MaxLogSize;
        /// <summary>
        /// 是否文件流模式
        /// </summary>
        private bool isFieStream;
        /// <summary>
        /// 日志处理
        /// </summary>
        /// <param name="fileName">日志文件</param>
        /// <param name="maxCacheCount">最大缓存数量</param>
        public log(string fileName, int maxCacheCount)
        {
            queue = new queue<debug>.concurrent.singleDequeueNode(new debug(), output, onError);
            this.maxCacheCount = maxCacheCount <= 0 ? 1 : maxCacheCount;
            if ((this.fileName = fileName) != null)
            {
                open();
                if (isFieStream) fastCSharp.domainUnload.AddLast(this, domainUnload.unloadType.LogDispose);
            }
        }
        /// <summary>
        /// 打开日志文件
        /// </summary>
        private void open()
        {
#if XAMARIN
            try
            {
                fileStream = new fileStreamWriter(fileName, FileMode.Create, FileShare.Read, FileOptions.None, false, fastCSharp.config.appSetting.Encoding);
                isFieStream = true;
                return;
            }
            catch (Exception error)
            {
                Console.WriteLine(error.ToString());
            }
#else
            try
            {
                fileStream = new fileStreamWriter(fileName, FileMode.OpenOrCreate, FileShare.Read, FileOptions.None, false, fastCSharp.config.appSetting.Encoding);
                isFieStream = true;
                return;
            }
            catch (Exception error)
            {
                Console.WriteLine(error.ToString());
            }
            try
            {
                if (File.Exists(fileName))
                {
                    fileStream = new fileStreamWriter(io.file.MoveBakFileName(fileName), FileMode.OpenOrCreate, FileShare.Read, FileOptions.None, false, fastCSharp.config.appSetting.Encoding);
                    isFieStream = true;
                    return;
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error.ToString());
            }
#endif
            isFieStream = false;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.Increment(ref isDisposed) == 1)
            {
                fastCSharp.domainUnload.RemoveLast(this, domainUnload.unloadType.LogDispose, false);
                if (isFieStream)
                {
                    Monitor.Enter(fileLock);
                    try
                    {
#if XAMARIN
                        pub.Dispose(ref fileStream);
#else
                        if (fileStream != null)
                        {
                            string fileName = fileStream.FileName;
                            pub.Dispose(ref fileStream);
                            io.file.MoveBak(fileName);
                        }
#endif
                    }
                    finally { Monitor.Exit(fileLock); }
                }
            }
        }
        /// <summary>
        /// 队列错误处理
        /// </summary>
        /// <param name="error"></param>
        private void onError(Exception error)
        {
            Console.WriteLine(error.ToString());
        }
        /// <summary>
        /// 日志信息写文件
        /// </summary>
        /// <param name="value">日志信息</param>
        private void output(debug value)
        {
            if (isDisposed == 0)
            {
                if (isFieStream)
                {
                    memoryPool.pushSubArray data = fileStreamWriter.GetBytes(@"
" + date.nowTime.Now.toString() + " : " + value.ToString() + @"
", fastCSharp.config.appSetting.Encoding);
                    Monitor.Enter(fileLock);
                    try
                    {
                        if (fileStream != null)
                        {
                            if (fileStream.UnsafeWrite(ref data) >= MaxSize && MaxSize > 0) moveBak();
                            return;
                        }
                    }
                    finally { Monitor.Exit(fileLock); }
                }
                Console.WriteLine(@"
" + date.nowTime.Now.toString() + " : " + value.ToString());
            }
        }
        /// <summary>
        /// 日志信息写文件
        /// </summary>
        /// <param name="value">日志信息</param>
        private void realOutput(debug value)
        {
            if (isDisposed == 0)
            {
                if (isFieStream)
                {
                    memoryPool.pushSubArray data = fileStreamWriter.GetBytes(@"
" + date.nowTime.Now.toString() + " : " + value.ToString() + @"
", fastCSharp.config.appSetting.Encoding);
                    Monitor.Enter(fileLock);
                    try
                    {
                        if (fileStream != null)
                        {
                            if (fileStream.UnsafeWrite(ref data) >= MaxSize && MaxSize > 0) moveBak();
                            else fileStream.Flush(true);
                            return;
                        }
                    }
                    finally { Monitor.Exit(fileLock); }
                }
                Console.WriteLine(@"
" + date.nowTime.Now.toString() + " : " + value.ToString());
            }
        }
        /// <summary>
        /// 检测缓存是否存在
        /// </summary>
        /// <param name="value">日志信息</param>
        /// <param name="isQueue">是否缓存队列</param>
        /// <returns>是否继续输出日志</returns>
        private bool checkCache(debug value, bool isQueue)
        {
            hashString key = value.ToString();
            if (isQueue)
            {
                Monitor.Enter(cacheLock);
                try
                {
                    if (cache.Get(key, false)) return false;
                    cache.Set(key, true);
                    if (cache.Count > maxCacheCount) cache.UnsafePopValue();
                }
                finally { Monitor.Exit(cacheLock); }
                return true;
            }
            if (key.Equals(lastCache)) return false;
            lastCache = key;
            return true;
        }
        /// <summary>
        /// 移动日志文件
        /// </summary>
        /// <returns>新的日志文件名称</returns>
        public string MoveBak()
        {
            if (isFieStream)
            {
                Monitor.Enter(fileLock);
                try
                {
                    if (fileStream != null) return moveBak();
                }
                finally { Monitor.Exit(fileLock); }
            }
            return null;
        }
        /// <summary>
        /// 移动日志文件
        /// </summary>
        /// <returns>新的日志文件名称</returns>
        private string moveBak()
        {
            string fileName = fileStream.FileName;
            pub.Dispose(ref fileStream);
            try
            {
                return io.file.MoveBak(fileName);
            }
            finally
            {
                open();
                if (!isFieStream) fastCSharp.threading.threadPool.TinyPool.FastStart(this, thread.callType.LogDispose);
            }
        }
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="error">错误异常</param>
        /// <param name="message">提示信息</param>
        /// <param name="isCache">是否缓存</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Add(Exception error, string message = null, bool isCache = true)
        {
            Add(error, message, isCache ? cacheType.Queue : cacheType.None);
        }
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="error">错误异常</param>
        /// <param name="message">提示信息</param>
        /// <param name="cache">缓存类型</param>
        public void Add(Exception error, string message, cacheType cache)
        {
            if (error != null && error.Message.StartsWith(ExceptionPrefix, StringComparison.Ordinal)) error = null;
            if (error == null)
            {
                if (message != null) Add(message, null, cache);
            }
            else
            {
                debug value = new debug
                {
                    Exception = error,
                    Message = message
                };
                if (cache == cacheType.None || checkCache(value, cache == cacheType.Queue)) queue.EnqueueNotNull(value);
            }
        }
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <param name="stackFrame">堆栈帧函数信息</param>
        /// <param name="isCache">是否缓存</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Add(string message, StackFrame stackFrame = null, bool isCache = false)
        {
            Add(message, stackFrame, isCache ? cacheType.Queue : cacheType.None);
        }
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <param name="stackFrame">堆栈帧函数信息</param>
        /// <param name="cache">缓存类型</param>
        public void Add(string message, StackFrame stackFrame, cacheType cache)
        {
            debug value = new debug
            {
                StackTrace = stackFrame == null ? new StackTrace(true) : null,
                StackFrame = stackFrame,
                Message = message
            };
            if (cache == cacheType.None || checkCache(value, cache == cacheType.Queue)) queue.EnqueueNotNull(value);
        }
        /// <summary>
        /// 实时添加日志
        /// </summary>
        /// <param name="error">错误异常</param>
        /// <param name="message">提示信息</param>
        /// <param name="isCache">是否缓存</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Real(Exception error, string message = null, bool isCache = true)
        {
            Real(error, message, isCache ? cacheType.Queue : cacheType.None);
        }
        /// <summary>
        /// 实时添加日志
        /// </summary>
        /// <param name="error">错误异常</param>
        /// <param name="message">提示信息</param>
        /// <param name="cache">缓存类型</param>
        public void Real(Exception error, string message, cacheType cache)
        {
            if (error != null && error.Message.StartsWith(ExceptionPrefix, StringComparison.Ordinal)) error = null;
            if (error == null)
            {
                if (message != null) Real(message, null, cache);
            }
            else
            {
                debug value = new debug
                {
                    Exception = error,
                    Message = message
                };
                if (cache == cacheType.None || checkCache(value, cache == cacheType.Queue)) realOutput(value);
            }
        }
        /// <summary>
        /// 实时添加日志
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <param name="stackFrame">堆栈帧函数信息</param>
        /// <param name="isCache">是否缓存</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Real(string message, StackFrame stackFrame = null, bool isCache = false)
        {
            Real(message, stackFrame, isCache ? cacheType.Queue : cacheType.None);
        }
        /// <summary>
        /// 实时添加日志
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <param name="stackFrame">堆栈帧函数信息</param>
        /// <param name="cache">缓存类型</param>
        public void Real(string message, StackFrame stackFrame, cacheType cache)
        {
            debug value = new debug
            {
                StackTrace = stackFrame == null ? new StackTrace(true) : null,
                StackFrame = stackFrame,
                Message = message
            };
            if (cache == cacheType.None || checkCache(value, cache == cacheType.Queue)) realOutput(value);
        }
        /// <summary>
        /// 添加日志并抛出异常
        /// </summary>
        /// <param name="error">异常类型</param>
        public void Throw(exceptionType error)
        {
            debug value = new debug
            {
                StackTrace = new StackTrace(true),
                Type = error
            };
            if (checkCache(value, true)) queue.EnqueueNotNull(value);
            throw new Exception(ExceptionPrefix + value.ToString());
        }
        /// <summary>
        /// 添加日志并抛出异常
        /// </summary>
        /// <param name="error">错误异常</param>
        /// <param name="message">提示信息</param>
        /// <param name="isCache">是否缓存</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Throw(Exception error, string message = null, bool isCache = true)
        {
            Throw(error, message, isCache ? cacheType.Queue : cacheType.None);
        }
        /// <summary>
        /// 添加日志并抛出异常
        /// </summary>
        /// <param name="error">错误异常</param>
        /// <param name="message">提示信息</param>
        /// <param name="cache">缓存类型</param>
        public void Throw(Exception error, string message, cacheType cache)
        {
            if (error != null && error.Message.StartsWith(ExceptionPrefix, StringComparison.Ordinal)) error = null;
            if (error == null)
            {
                if (message != null) Throw(message, null, cache);
            }
            else
            {
                debug value = new debug
                {
                    Exception = error,
                    Message = message
                };
                if (cache == cacheType.None || checkCache(value, cache == cacheType.Queue)) queue.EnqueueNotNull(value);
                throw error != null ? new Exception(ExceptionPrefix + message, error) : new Exception(ExceptionPrefix + message);
            }
        }
        /// <summary>
        /// 添加日志并抛出异常
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <param name="stackFrame">堆栈帧函数信息</param>
        /// <param name="isCache">是否缓存</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Throw(string message, StackFrame stackFrame = null, bool isCache = false)
        {
            Throw(message, stackFrame, isCache ? cacheType.Queue : cacheType.None);
        }
        /// <summary>
        /// 添加日志并抛出异常
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <param name="stackFrame">堆栈帧函数信息</param>
        /// <param name="cache">缓存类型</param>
        public void Throw(string message, StackFrame stackFrame, cacheType cache)
        {
            debug value = new debug
            {
                StackTrace = stackFrame == null ? new StackTrace(true) : null,
                StackFrame = stackFrame,
                Message = message
            };
            if (cache == cacheType.None || checkCache(value, cache == cacheType.Queue)) queue.EnqueueNotNull(value);
            throw new Exception(ExceptionPrefix + message);
        }
        /// <summary>
        /// 实时添加日志并抛出异常
        /// </summary>
        /// <param name="error">异常类型</param>
        public void ThrowReal(exceptionType error)
        {
            debug value = new debug
            {
                StackTrace = new StackTrace(true),
                Type = error
            };
            if (checkCache(value, true)) realOutput(value);
            throw new Exception(ExceptionPrefix + value.ToString());
        }
        /// <summary>
        /// 实时添加日志并抛出异常
        /// </summary>
        /// <param name="error">错误异常</param>
        /// <param name="message">提示信息</param>
        /// <param name="isCache">是否缓存</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void ThrowReal(Exception error, string message = null, bool isCache = true)
        {
            ThrowReal(error, message, isCache ? cacheType.Queue : cacheType.None);
        }
        /// <summary>
        /// 实时添加日志并抛出异常
        /// </summary>
        /// <param name="error">错误异常</param>
        /// <param name="message">提示信息</param>
        /// <param name="cache">缓存类型</param>
        public void ThrowReal(Exception error, string message, cacheType cache)
        {
            if (error != null && error.Message.StartsWith(ExceptionPrefix, StringComparison.Ordinal)) error = null;
            if (error == null)
            {
                if (message != null) ThrowReal(message, null, cache);
            }
            else
            {
                debug value = new debug
                {
                    Exception = error,
                    Message = message
                };
                if (cache == cacheType.None || checkCache(value, cache == cacheType.Queue)) realOutput(value);
                throw error != null ? new Exception(ExceptionPrefix + message, error) : new Exception(ExceptionPrefix + message);
            }
        }
        /// <summary>
        /// 实时添加日志并抛出异常
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <param name="stackFrame">堆栈帧函数信息</param>
        /// <param name="isCache">是否缓存</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void ThrowReal(string message, StackFrame stackFrame = null, bool isCache = false)
        {
            ThrowReal(message, stackFrame, isCache ? cacheType.Queue : cacheType.None);
        }
        /// <summary>
        /// 实时添加日志并抛出异常
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <param name="stackFrame">堆栈帧函数信息</param>
        /// <param name="cache">缓存类型</param>
        public void ThrowReal(string message, StackFrame stackFrame, cacheType cache)
        {
            debug value = new debug
            {
                StackTrace = stackFrame == null ? new StackTrace(true) : null,
                StackFrame = stackFrame,
                Message = message
            };
            if (cache == cacheType.None || checkCache(value, cache == cacheType.Queue)) realOutput(value);
            throw new Exception(ExceptionPrefix + message);
        }
        /// <summary>
        /// 信息日志，一般用于辅助定位BUG
        /// </summary>
        public static readonly log Default;
        /// <summary>
        /// 重要错误日志，说明很可能存在BUG
        /// </summary>
        public static readonly log Error;
        static log()
        {
            Default = new log(config.appSetting.IsLogConsole ? null : config.appSetting.LogPath + DefaultFilePrefix + "default.txt", config.appSetting.MaxLogCacheCount);
            Error = config.appSetting.IsErrorLog ? new log(config.appSetting.IsLogConsole ? null : config.appSetting.LogPath + DefaultFilePrefix + "error.txt", config.appSetting.MaxLogCacheCount) : Default;
#if DEBUGPOOL
            if (config.appSetting.IsPoolDebug) Default.Add("对象池采用纠错模式", new System.Diagnostics.StackFrame(), cacheType.None);
#endif
            if (fastCSharp.config.appSetting.IsCheckMemory) checkMemory.Add(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }
    }
}
