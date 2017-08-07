using System;
using System.Collections.Generic;

namespace fastCSharp.android
{
    /// <summary>
    /// System.Collections.Generic.HashSet À©Õ¹
    /// </summary>
    public static  class hashSet
    {
        /// <summary>
        /// Add
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="hash"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool add<valueType>(this HashSet<valueType> hash, valueType value)
        {
            return hash.Add(value);
        }
    }
}