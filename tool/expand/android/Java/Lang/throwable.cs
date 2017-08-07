using System;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Lang.Throwable “Ï≥£¿©’π
    /// </summary>
    public static class throwable
    {
        /// <summary>
        /// PrintStackTrace
        /// </summary>
        /// <param name="exception"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void printStackTrace(this Java.Lang.Throwable throwable)
        {
            throwable.PrintStackTrace();
        }
        /// <summary>
        /// PrintStackTrace
        /// </summary>
        /// <param name="throwable"></param>
        /// <param name="writer"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void printStackTrace(this Java.Lang.Throwable throwable, Java.IO.PrintWriter writer)
        {
            throwable.PrintStackTrace(writer);
        }
        /// <summary>
        /// GetStackTrace
        /// </summary>
        /// <param name="throwable"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.Lang.StackTraceElement[] getStackTrace(this Java.Lang.Throwable throwable)
        {
            return throwable.GetStackTrace();
        }
        /// <summary>
        /// SetStackTrace
        /// </summary>
        /// <param name="throwable"></param>
        /// <param name="elements"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setStackTrace(this Java.Lang.Throwable throwable, Java.Lang.StackTraceElement[] elements)
        {
            throwable.SetStackTrace(elements);
        }
    }
}