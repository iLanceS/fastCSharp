using System;
using Android.Webkit;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Webkit.ConsoleMessage À©Õ¹
    /// </summary>
    public static class consoleMessage
    {
        /// <summary>
        /// Message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string message(this ConsoleMessage message)
        {
            return message.Message();
        }
        /// <summary>
        /// LineNumber
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int lineNumber(this ConsoleMessage message)
        {
            return message.LineNumber();
        }
        /// <summary>
        /// SourceId
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string sourceId(this ConsoleMessage message)
        {
            return message.SourceId();
        }
    }
}