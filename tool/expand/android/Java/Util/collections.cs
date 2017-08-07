using System;
using System.Collections;
using Java.Util;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Util.Collections À©Õ¹
    /// </summary>
    public static class collections
    {
        /// <summary>
        /// SynchronizedList
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static System.Collections.IList synchronizedList(System.Collections.IList list)
        {
            return Collections.SynchronizedList(list);
        }
        /// <summary>
        /// SynchronizedMap
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static IDictionary synchronizedMap(IDictionary dictionary)
        {
            return Collections.SynchronizedMap(dictionary);
        }
    }
}