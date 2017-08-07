using System;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Lang.String À©Õ¹
    /// </summary>
    public static partial class stringExpand
    {
        /// <summary>
        /// Java.Lang.Class
        /// </summary>
        public static readonly Java.Lang.Class Class = new Java.Lang.String((string)null).Class;
        /// <summary>
        /// Split
        /// </summary>
        /// <param name="value"></param>
        /// <param name="regex"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string[] split(this Java.Lang.String value, string regex, int limit)
        {
            return value.Split(regex, limit);
        }
        /// <summary>
        /// Split
        /// </summary>
        /// <param name="value"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string[] split(this Java.Lang.String value, string regex)
        {
            return value.Split(regex);
        }
    }
}