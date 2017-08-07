using System;
using System.Collections.Generic;

namespace fastCSharp.android
{
    /// <summary>
    /// 字典扩展
    /// </summary>
    public static class dictionary
    {
        /// <summary>
        /// 获取数据 TryGetValue
        /// </summary>
        /// <typeparam name="keyType"></typeparam>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static valueType get<keyType, valueType>(this Dictionary<keyType, valueType> dictionary, keyType key)
        {
            valueType value;
            return dictionary.TryGetValue(key, out value) ? value : default(valueType);
        }
        /// <summary>
        /// 设置数据 [key] = value
        /// </summary>
        /// <typeparam name="keyType"></typeparam>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void put<keyType, valueType>(this Dictionary<keyType, valueType> dictionary, keyType key, valueType value)
        {
            dictionary[key] = value;
        }
        /// <summary>
        /// Keys
        /// </summary>
        /// <typeparam name="keyType"></typeparam>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Dictionary<keyType, valueType>.KeyCollection keySet<keyType, valueType>(this Dictionary<keyType, valueType> dictionary)
        {
            return dictionary.Keys;
        }
    }
}