using System;

namespace fastCSharp.android
{
    /// <summary>
    /// 公共扩展
    /// </summary>
    public static class pub
    {
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string toString(this object value)
        {
            return value.ToString();
        }
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static object clone(this object value)
        {
            return value.Clone();
        }
        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string toString(this Java.Lang.Object value)
        {
            return value.ToString();
        }
        /// <summary>
        /// 相等比较 Equals
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool equals(this Java.Lang.Object left, Java.Lang.Object right)
        {
            return left.Equals(right);
        }
    }
}