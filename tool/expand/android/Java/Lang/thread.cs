using System;

namespace fastCSharp.android
{
    /// <summary>
    /// 线程扩展
    /// </summary>
    public static class thread
    {
        /// <summary>
        /// 获取当前线程
        /// </summary>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.Lang.Thread currentThread()
        {
            return Java.Lang.Thread.CurrentThread();
        }
        /// <summary>
        /// 启动线程 Start
        /// </summary>
        /// <param name="thread"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void start(this Java.Lang.Thread thread)
        {
            thread.Start();
        }
        /// <summary>
        /// Name
        /// </summary>
        /// <param name="thread"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getName(this Java.Lang.Thread thread)
        {
            return thread.Name;
        }
        /// <summary>
        /// GetStackTrace
        /// </summary>
        /// <param name="thread"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.Lang.StackTraceElement[] getStackTrace(this Java.Lang.Thread thread)
        {
            return thread.GetStackTrace();
        }
        /// <summary>
        /// Id
        /// </summary>
        /// <param name="thread"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static long getId(this Java.Lang.Thread thread)
        {
            return thread.Id;
        }
    }
}