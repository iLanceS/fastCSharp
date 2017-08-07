using System;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Lang.StackTraceElement À©Õ¹
    /// </summary>
    public static class stackTraceElement
    {
        /// <summary>
        /// FileName
        /// </summary>
        /// <param name="stackTrace"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getFileName(this Java.Lang.StackTraceElement stackTrace)
        {
            return stackTrace.FileName;
        }
        /// <summary>
        /// LineNumber
        /// </summary>
        /// <param name="stackTrace"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int getLineNumber(this Java.Lang.StackTraceElement stackTrace)
        {
            return stackTrace.LineNumber;
        }
        /// <summary>
        /// IsNativeMethod
        /// </summary>
        /// <param name="stackTrace"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool isNativeMethod(this Java.Lang.StackTraceElement stackTrace)
        {
            return stackTrace.IsNativeMethod;
        }
        /// <summary>
        /// ClassName
        /// </summary>
        /// <param name="stackTrace"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getClassName(this Java.Lang.StackTraceElement stackTrace)
        {
            return stackTrace.ClassName;
        }
    }
}