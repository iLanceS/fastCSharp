using System;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Util.Log À©Õ¹
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// Verbose
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void v(string tag, string format, params object[] args)
        {
            Android.Util.Log.Verbose(tag, format, args);
        }
        /// <summary>
        /// Debug
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void d(string tag, string format, params object[] args)
        {
            Android.Util.Log.Debug(tag, format, args);
        }
        /// <summary>
        /// Info
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void i(string tag, string format, params object[] args)
        {
            Android.Util.Log.Info(tag, format, args);
        }
        /// <summary>
        /// Warn
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void w(string tag, string format, params object[] args)
        {
            Android.Util.Log.Warn(tag, format, args);
        }
        /// <summary>
        /// Error
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void e(string tag, string format, params object[] args)
        {
            Android.Util.Log.Error(tag, format, args);
        }
        /// <summary>
        /// GetStackTraceString
        /// </summary>
        /// <param name="throwable"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getStackTraceString(Java.Lang.Throwable throwable)
        {
            return Android.Util.Log.GetStackTraceString(throwable);
        }
    }
}