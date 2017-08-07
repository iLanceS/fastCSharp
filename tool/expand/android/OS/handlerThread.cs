using System;
using Android.OS;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.OS.HandlerThread À©Õ¹
    /// </summary>
    public static class handlerThread
    {
        /// <summary>
        /// Looper
        /// </summary>
        /// <param name="thread"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Looper getLooper(this HandlerThread thread)
        {
            return thread.Looper;
        }
        /// <summary>
        /// IsAlive
        /// </summary>
        /// <param name="thread"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool isAlive(this HandlerThread thread)
        {
            return thread.IsAlive;
        }
        /// <summary>
        /// Start
        /// </summary>
        /// <param name="thread"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void start(this HandlerThread thread)
        {
            thread.Start();
        }
    }
}