using System;
using Java.Util;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Util.IIterator À©Õ¹
    /// </summary>
    public static class iIterator
    {
        /// <summary>
        /// HasNext
        /// </summary>
        /// <param name="iterator"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool hasNext(this IIterator iterator)
        {
            return iterator.HasNext;
        }
        /// <summary>
        /// Next
        /// </summary>
        /// <param name="iterator"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.Lang.Object next(this IIterator iterator)
        {
            return iterator.Next();
        }
    }
}