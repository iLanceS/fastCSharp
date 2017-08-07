using System;

namespace fastCSharp.android
{
    /// <summary>
    /// WeakReference À©Õ¹
    /// </summary>
    public static class weakReference
    {
        /// <summary>
        /// Target
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static object get(this WeakReference reference)
        {
            return reference.Target;
        }
    }
}