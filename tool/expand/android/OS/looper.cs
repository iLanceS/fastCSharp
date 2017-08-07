using System;
using Android.OS;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.OS.Looper À©Õ¹
    /// </summary>
    public static class looper
    {
        /// <summary>
        /// Thread
        /// </summary>
        /// <param name="looper"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.Lang.Thread getThread(this Looper looper)
        {
            return looper.Thread;
        }
    }
}