using System;
using Java.Util;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Util.Locale À©Õ¹
    /// </summary>
    public static class locale
    {
        /// <summary>
        /// Default
        /// </summary>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Locale getDefault()
        {
            return Locale.Default;
        }
        /// <summary>
        /// Language
        /// </summary>
        /// <param name="locale"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getLanguage(this Locale locale)
        {
            return locale.Language;
        }
    }
}