using System;
using System.Collections;
using System.Collections.Generic;

namespace fastCSharp.android
{
    /// <summary>
    /// IList 扩展
    /// </summary>
    public static class iList
    {
        /// <summary>
        /// 根据索引获取数据
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static valueType get<valueType>(this IList<valueType> list, int index)
        {
            return list[index];
        }
        /// <summary>
        /// 根据索引获取数据
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static object Get(this IList list, int index)
        {
            return list[index];
        }
        /// <summary>
        /// Clear
        /// </summary>
        /// <param name="list"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void clear(this IList list)
        {
            list.Clear();
        }
        /// <summary>
        /// Add
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int add(this IList list, object value)
        {
            return list.Add(value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="other"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void addAll(this IList list, IList other)
        {
            foreach (object value in other) list.Add(value);
        }
        /// <summary>
        /// Count
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int Size(this IList value)
        {
            return value.Count;
        }
    }
}