using System;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Lang.Thread.IUncaughtExceptionHandler À©Õ¹
    /// </summary>
    public static class iUncaughtExceptionHandler
    {
        /// <summary>
        /// UncaughtException
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="thread"></param>
        /// <param name="throwable"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void uncaughtException(this Java.Lang.Thread.IUncaughtExceptionHandler handler, Java.Lang.Thread thread, Java.Lang.Throwable throwable)
        {
            handler.UncaughtException(thread, throwable);
        }
    }
}