using System;
using Java.Util;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Util.Calendar À©Õ¹
    /// </summary>
    public static class calendar
    {
        /// <summary>
        /// Instance
        /// </summary>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Calendar getInstance()
        {
            return Calendar.Instance;
        }
        /// <summary>
        /// TimeInMillis
        /// </summary>
        /// <param name="calendar"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setTimeInMillis(this Calendar calendar, long value)
        {
            calendar.TimeInMillis = value;
        }
        /// <summary>
        /// Time
        /// </summary>
        /// <param name="calendar"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Date getTime(this Calendar calendar)
        {
            return calendar.Time;
        }
        /// <summary>
        /// Set
        /// </summary>
        /// <param name="calendar"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void set(this Calendar calendar, CalendarField field, int value)
        {
            calendar.Set(field, value);
        }
        /// <summary>
        /// Set
        /// </summary>
        /// <param name="calendar"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void set(this Calendar calendar, int field, int value)
        {
            calendar.Set((CalendarField)field, value);
        }
        /// <summary>
        /// TimeInMillis
        /// </summary>
        /// <param name="calendar"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static long getTimeInMillis(this Calendar calendar)
        {
            return calendar.TimeInMillis;
        }
    }
}