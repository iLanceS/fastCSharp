using System;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Lang.Exception À©Õ¹
    /// </summary>
    public static class exception
    {
        /// <summary>
        /// GetStackTrace
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.Lang.StackTraceElement[] getStackTrace(this Java.Lang.Exception exception)
        {
            return exception.GetStackTrace();
        }
        /// <summary>
        /// Message
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getMessage(this Java.Lang.Exception exception)
        {
            return exception.Message;
        }
    }
}