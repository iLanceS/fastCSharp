using System;
using Android.Text.Format;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Text.Format.Time À©Õ¹
    /// </summary>
    public static class time
    {
        /// <summary>
        /// Set
        /// </summary>
        /// <param name="time"></param>
        /// <param name="millis"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void set(this Time time, long millis)
        {
            time.Set(millis);
        }
        /// <summary>
        /// Format
        /// </summary>
        /// <param name="time"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string format(this Time time, string format)
        {
            return time.Format(format);
        }
    }
}