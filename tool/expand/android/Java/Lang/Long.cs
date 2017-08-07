using System;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Lang.Long 扩展
    /// </summary>
    public static class Long
    {
        /// <summary>
        /// LongValue
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static long longValue(this Java.Lang.Long value)
        {
            return value.LongValue();
        }
        /// <summary>
        /// ValueOf
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.Lang.Long valueOf(long value)
        {
            return Java.Lang.Long.ValueOf(value);
        }
        /// <summary>
        /// ValueOf
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.Lang.Long valueOf(string value)
        {
            return Java.Lang.Long.ValueOf(value);
        }
        /// <summary>
        /// 字符串转整数 int.Parse
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static long parseLong(string value)
        {
            return long.Parse(value);
        }
    }
}