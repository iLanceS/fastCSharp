using System;
using System.Collections.Generic;

namespace fastCSharp.android
{
    /// <summary>
    /// List À©Õ¹
    /// </summary>
    public static class list
    {
        /// <summary>
        /// Add
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="list"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void add<valueType>(this List<valueType> list, valueType value)
        {
            list.Add(value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="list"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public static List<valueType> subList<valueType>(this List<valueType> list, int startIndex, int endIndex)
        {
            List<valueType> newList = new List<valueType>(endIndex - startIndex);
            while (startIndex <= endIndex) newList.Add(list[startIndex++]);
            return newList;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void set<valueType>(this List<valueType> list, int index, valueType value)
        {
            list[index] = value;
        }
    }
}