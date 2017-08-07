using System;
using Java.Util.Concurrent;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Util.Concurrent.ThreadPoolExecutor 扩展
    /// </summary>
    public static class threadPoolExecutor
    {
        /// <summary>
        /// 不支持
        /// </summary>
        /// <param name="executor"></param>
        /// <param name="count"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setCorePoolSize(this ThreadPoolExecutor executor, int count) { }
    }
}