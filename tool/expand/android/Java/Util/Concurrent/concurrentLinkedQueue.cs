using System;
using Java.Util.Concurrent;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Util.Concurrent.ConcurrentLinkedQueue À©Õ¹
    /// </summary>
    public static class concurrentLinkedQueue
    {
        /// <summary>
        /// Add
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool add(this ConcurrentLinkedQueue queue, Java.Lang.Object value)
        {
            return queue.Add(value);
        }
        /// <summary>
        /// Clear
        /// </summary>
        /// <param name="queue"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void clear(this ConcurrentLinkedQueue queue)
        {
            queue.Clear();
        }
    }
}