using System;
using System.Collections;
using System.Collections.Generic;

namespace fastCSharp.android
{
    /// <summary>
    /// IEnumerator 扩展
    /// </summary>
    public static class iEnumerator
    {
        /// <summary>
        /// 获取当前数据 Current
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static valueType next<valueType>(this IEnumerator<valueType> value)
        {
            return value.Current;
        }
        /// <summary>
        /// 判断是否存在下一个数据 MoveNext
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool hasNext(this IEnumerator value)
        {
            return value.MoveNext();
        }
        /// <summary>
        /// 获取当前数据 Current
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static object next(this IEnumerator value)
        {
            return value.Current;
        }
    }
}