using System;
/*Type:double,doubleSortIndex;float,floatSortIndex*/
/*Compare:,>,<;Desc,<,>*/

namespace fastCSharp
{
    /// <summary>
    /// 数组扩展操作
    /// </summary>
    public static partial class arrayExtension
    {
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <returns>排序后的数组</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static /*Type[0]*/double/*Type[0]*/[] sort/*Compare[0]*//*Compare[0]*/(this /*Type[0]*/double/*Type[0]*/[] array)
        {
            if (array != null)
            {
                fastCSharp.algorithm.quickSort.Sort/*Compare[0]*//*Compare[0]*/(array);
                return array;
            }
            return nullValue</*Type[0]*/double/*Type[0]*/>.Array;
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <returns>排序后的新数组</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static /*Type[0]*/double/*Type[0]*/[] getSort/*Compare[0]*//*Compare[0]*/(this /*Type[0]*/double/*Type[0]*/[] array)
        {
            if (array != null)
            {
                return fastCSharp.algorithm.quickSort.GetSort/*Compare[0]*//*Compare[0]*/(array);
            }
            return nullValue</*Type[0]*/double/*Type[0]*/>.Array;
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组</param>
        /// <param name="getKey">排序键</param>
        /// <returns>排序后的数组</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType[] getSort/*Compare[0]*//*Compare[0]*/<valueType>
            (this valueType[] array, Func<valueType, /*Type[0]*/double/*Type[0]*/> getKey)
        {
            if (getKey == null) log.Error.Throw(log.exceptionType.Null);
            if (array != null)
            {
                if (array.Length > 1) return fastCSharp.algorithm.quickSort.GetSort/*Compare[0]*//*Compare[0]*/(array, getKey);
                if (array.Length != 0) return new valueType[] { array[0] };
            }
            return nullValue<valueType>.Array;
        }
    }
    
    /// <summary>
    /// 数组子串扩展
    /// </summary>
    public static partial class subArrayExtension
    {
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <returns>排序后的数组</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray</*Type[0]*/double/*Type[0]*/> sort/*Compare[0]*//*Compare[0]*/(this subArray</*Type[0]*/double/*Type[0]*/> array)
        {
            if (array.Count > 1) fastCSharp.algorithm.quickSort.Sort/*Compare[0]*//*Compare[0]*/(array.UnsafeArray, array.StartIndex, array.Count);
            return array;
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <returns>排序后的新数组</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static /*Type[0]*/double/*Type[0]*/[] getSort/*Compare[0]*//*Compare[0]*/(this subArray</*Type[0]*/double/*Type[0]*/> array)
        {
            return fastCSharp.algorithm.quickSort.GetSort/*Compare[0]*//*Compare[0]*/(array.UnsafeArray, array.StartIndex, array.Count);
        }
        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组</param>
        /// <param name="getKey">排序键</param>
        /// <returns>排序后的数组</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType[] getSort/*Compare[0]*//*Compare[0]*/<valueType>(this subArray<valueType> array, Func<valueType, /*Type[0]*/double/*Type[0]*/> getKey)
        {
            if (array.Count > 1) return fastCSharp.algorithm.quickSort.GetSort/*Compare[0]*//*Compare[0]*/(array.UnsafeArray, getKey, array.StartIndex, array.Count);
            return array.Count == 0 ? nullValue<valueType>.Array : new valueType[] { array.UnsafeArray[array.StartIndex] };
        }
    }
}