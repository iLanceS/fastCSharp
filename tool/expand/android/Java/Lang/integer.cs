using System;

namespace fastCSharp.android
{
    /// <summary>
    /// 整数扩展
    /// </summary>
    public static class integer
    {
        /// <summary>
        /// IntValue
        /// </summary>
        /// <param name="integer"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int intValue(this Java.Lang.Integer integer)
        {
            return integer.IntValue();
        }

        /// <summary>
        /// 字符串转整数 int.Parse
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int parseInt(string value)
        {
            return int.Parse(value);
        }
        /// <summary>
        /// ValueOf
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.Lang.Integer valueOf(int value)
        {
            return Java.Lang.Integer.ValueOf(value);
        }
        /// <summary>
        /// ValueOf
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.Lang.Integer valueOf(string value)
        {
            return Java.Lang.Integer.ValueOf(value);
        }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string toString(int value)
        {
            return Java.Lang.Integer.ToString(value);
        }
        /// <summary>
        /// ToHexString
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string toHexString(int value)
        {
            return Java.Lang.Integer.ToHexString(value);
        }
    }
}