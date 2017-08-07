using System;
using Java.Util;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Util.LinkedList À©Õ¹
    /// </summary>
    public static class linkedList
    {
        /// <summary>
        /// Poll
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.Lang.Object poll(this LinkedList list)
        {
            return list.Poll();
        }
    }
}