using System;
using Java.Text;
using Java.Util;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Text.SimpleDateFormat À©Õ¹
    /// </summary>
    public static class simpleDateFormat
    {
        /// <summary>
        /// Format
        /// </summary>
        /// <param name="format"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string format(this SimpleDateFormat format, Date date)
        {
            return format.Format(date);
        }
    }
}