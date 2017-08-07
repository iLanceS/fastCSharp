using System;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Text.TextUtils À©Õ¹
    /// </summary>
    public static class textUtils
    {
        /// <summary>
        /// IsEmpty
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool isEmpty(string value)
        {
            return Android.Text.TextUtils.IsEmpty(value);
        }
    }
}