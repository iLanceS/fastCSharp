using System;
using System.Collections.Generic;

namespace fastCSharp.android
{
    /// <summary>
    /// System.Collections.Generic.KeyValuePair À©Õ¹
    /// </summary>
    public static class keyValuePair
    {
        /// <summary>
        /// Key
        /// </summary>
        /// <typeparam name="keyType"></typeparam>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static keyType getKey<keyType, valueType>(this KeyValuePair<keyType, valueType> keyValue)
        {
            return keyValue.Key;
        }
        /// <summary>
        /// Value
        /// </summary>
        /// <typeparam name="keyType"></typeparam>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static valueType getValue<keyType, valueType>(this KeyValuePair<keyType, valueType> keyValue)
        {
            return keyValue.Value;
        }
    }
}