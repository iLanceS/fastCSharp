using System;
using Java.Util.Concurrent.Atomic;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Util.Concurrent.Atomic.AtomicInteger À©Õ¹
    /// </summary>
    public static class atomicInteger
    {
        /// <summary>
        /// Get
        /// </summary>
        /// <param name="atomicInteger"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int get(this AtomicInteger atomicInteger)
        {
            return atomicInteger.Get();
        }
        /// <summary>
        /// AddAndGet
        /// </summary>
        /// <param name="atomicInteger"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int addAndGet(this AtomicInteger atomicInteger, int delta)
        {
            return atomicInteger.AddAndGet(delta);
        }
        /// <summary>
        /// Set
        /// </summary>
        /// <param name="atomicInteger"></param>
        /// <param name="newValue"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void set(this AtomicInteger atomicInteger, int newValue)
        {
            atomicInteger.Set(newValue);
        }
    }
}