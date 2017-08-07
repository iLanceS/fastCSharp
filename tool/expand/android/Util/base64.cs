using System;
using Android.Util;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Util.Base64 À©Õ¹
    /// </summary>
    public static class base64
    {
        /// <summary>
        /// EncodeToString
        /// </summary>
        /// <param name="input"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string encodeToString(byte[] input, Base64Flags flags)
        {
            return Base64.EncodeToString(input, flags);
        }
        /// <summary>
        /// EncodeToString
        /// </summary>
        /// <param name="input"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string encodeToString(byte[] input, int flags)
        {
            return Base64.EncodeToString(input, (Base64Flags)flags);
        }
    }
}