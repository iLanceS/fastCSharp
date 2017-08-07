using System;
using fastCSharp.algorithm;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 数组扩展操作
    /// </summary>
    public static partial class arrayExtension
    {
        /// <summary>
        /// 求和
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static long sum(this int[] array)
        {
            if (array != null)
            {
                long sum = 0;
                foreach (int value in array) sum += value;
                return sum;
            }
            return 0;
        }

        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <returns>排序后的数组</returns>
        public static ulong[] sort(this ulong[] array)
        {
            if (array != null)
            {
                if (array.Length >= fastCSharp.pub.RadixSortSize64) algorithm.radixSort.Sort(array, 0, array.Length);
                else algorithm.quickSort.Sort(array);
                return array;
            }
            return nullValue<ulong>.Array;
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray<ulong> sort(this ulong[] array, int startIndex, int count)
        {
            return subArray<ulong>.Create(array, startIndex, count).sort();
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <returns>排序后的数组</returns>
        public static ulong[] sortDesc(this ulong[] array)
        {
            if (array != null)
            {
                if (array.Length >= fastCSharp.pub.RadixSortSize64) algorithm.radixSort.SortDesc(array, 0, array.Length);
                else algorithm.quickSort.SortDesc(array);
                return array;
            }
            return nullValue<ulong>.Array;
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray<ulong> sortDesc(this ulong[] array, int startIndex, int count)
        {
            return subArray<ulong>.Create(array, startIndex, count).sortDesc();
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <returns>排序后的新数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static ulong[] getSort(this ulong[] array)
        {
            return new subArray<ulong>(array).getSort();
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static ulong[] getSort(this ulong[] array, int startIndex, int count)
        {
            return subArray<ulong>.Create(array, startIndex, count).getSort();
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <returns>排序后的新数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static ulong[] getSortDesc(this ulong[] array)
        {
            return new subArray<ulong>(array).getSortDesc();
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static ulong[] getSortDesc(this ulong[] array, int startIndex, int count)
        {
            return subArray<ulong>.Create(array, startIndex, count).getSortDesc();
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组</param>
        /// <param name="getKey">排序键</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static unsafe valueType[] getSort<valueType>(this valueType[] array, Func<valueType, ulong> getKey)
        {
            return new subArray<valueType>(array).getSort(getKey);
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组</param>
        /// <param name="getKey">排序键</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static unsafe valueType[] getSort<valueType>(this valueType[] array, Func<valueType, ulong> getKey, int startIndex, int count)
        {
            return subArray<valueType>.Create(array, startIndex, count).getSort(getKey);
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组</param>
        /// <param name="getKey">排序键</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static unsafe valueType[] getSortDesc<valueType>(this valueType[] array, Func<valueType, ulong> getKey)
        {
            return new subArray<valueType>(array).getSortDesc(getKey);
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组</param>
        /// <param name="getKey">排序键</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static unsafe valueType[] getSortDesc<valueType>(this valueType[] array, Func<valueType, ulong> getKey, int startIndex, int count)
        {
            return subArray<valueType>.Create(array, startIndex, count).getSortDesc(getKey);
        }

        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <returns>排序后的数组</returns>
        public static long[] sort(this long[] array)
        {
            if (array != null)
            {
                if (array.Length >= fastCSharp.pub.RadixSortSize64) algorithm.radixSort.Sort(array, 0, array.Length);
                else algorithm.quickSort.Sort(array);
                return array;
            }
            return nullValue<long>.Array;
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray<long> sort(this long[] array, int startIndex, int count)
        {
            return subArray<long>.Create(array, startIndex, count).sort();
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <returns>排序后的数组</returns>
        public static long[] sortDesc(this long[] array)
        {
            if (array != null)
            {
                if (array.Length >= fastCSharp.pub.RadixSortSize64) algorithm.radixSort.SortDesc(array, 0, array.Length);
                else algorithm.quickSort.SortDesc(array);
                return array;
            }
            return nullValue<long>.Array;
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray<long> sortDesc(this long[] array, int startIndex, int count)
        {
            return subArray<long>.Create(array, startIndex, count).sortDesc();
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <returns>排序后的新数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static long[] getSort(this long[] array)
        {
            return new subArray<long>(array).getSort();
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static long[] getSort(this long[] array, int startIndex, int count)
        {
            return subArray<long>.Create(array, startIndex, count).getSort();
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <returns>排序后的新数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static long[] getSortDesc(this long[] array)
        {
            return new subArray<long>(array).getSortDesc();
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static long[] getSortDesc(this long[] array, int startIndex, int count)
        {
            return subArray<long>.Create(array, startIndex, count).getSortDesc();
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组</param>
        /// <param name="getKey">排序键</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static unsafe valueType[] getSort<valueType>(this valueType[] array, Func<valueType, long> getKey)
        {
            return new subArray<valueType>(array).getSort(getKey);
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组</param>
        /// <param name="getKey">排序键</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static unsafe valueType[] getSort<valueType>(this valueType[] array, Func<valueType, long> getKey, int startIndex, int count)
        {
            return subArray<valueType>.Create(array, startIndex, count).getSort(getKey);
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组</param>
        /// <param name="getKey">排序键</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static unsafe valueType[] getSortDesc<valueType>(this valueType[] array, Func<valueType, long> getKey)
        {
            return new subArray<valueType>(array).getSortDesc(getKey);
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组</param>
        /// <param name="getKey">排序键</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static unsafe valueType[] getSortDesc<valueType>(this valueType[] array, Func<valueType, long> getKey, int startIndex, int count)
        {
            return subArray<valueType>.Create(array, startIndex, count).getSortDesc(getKey);
        }

        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <returns>排序后的数组</returns>
        public static uint[] sort(this uint[] array)
        {
            if (array != null)
            {
                if (array.Length >= fastCSharp.pub.RadixSortSize64) algorithm.radixSort.Sort(array, 0, array.Length);
                else algorithm.quickSort.Sort(array);
                return array;
            }
            return nullValue<uint>.Array;
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray<uint> sort(this uint[] array, int startIndex, int count)
        {
            return subArray<uint>.Create(array, startIndex, count).sort();
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <returns>排序后的数组</returns>
        public static uint[] sortDesc(this uint[] array)
        {
            if (array != null)
            {
                if (array.Length >= fastCSharp.pub.RadixSortSize64) algorithm.radixSort.SortDesc(array, 0, array.Length);
                else algorithm.quickSort.SortDesc(array);
                return array;
            }
            return nullValue<uint>.Array;
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray<uint> sortDesc(this uint[] array, int startIndex, int count)
        {
            return subArray<uint>.Create(array, startIndex, count).sortDesc();
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <returns>排序后的新数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static uint[] getSort(this uint[] array)
        {
            return new subArray<uint>(array).getSort();
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static uint[] getSort(this uint[] array, int startIndex, int count)
        {
            return subArray<uint>.Create(array, startIndex, count).getSort();
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <returns>排序后的新数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static uint[] getSortDesc(this uint[] array)
        {
            return new subArray<uint>(array).getSortDesc();
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static uint[] getSortDesc(this uint[] array, int startIndex, int count)
        {
            return subArray<uint>.Create(array, startIndex, count).getSortDesc();
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组</param>
        /// <param name="getKey">排序键</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static unsafe valueType[] getSort<valueType>(this valueType[] array, Func<valueType, uint> getKey)
        {
            return new subArray<valueType>(array).getSort(getKey);
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组</param>
        /// <param name="getKey">排序键</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static unsafe valueType[] getSort<valueType>(this valueType[] array, Func<valueType, uint> getKey, int startIndex, int count)
        {
            return subArray<valueType>.Create(array, startIndex, count).getSort(getKey);
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组</param>
        /// <param name="getKey">排序键</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static unsafe valueType[] getSortDesc<valueType>(this valueType[] array, Func<valueType, uint> getKey)
        {
            return new subArray<valueType>(array).getSortDesc(getKey);
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组</param>
        /// <param name="getKey">排序键</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static unsafe valueType[] getSortDesc<valueType>(this valueType[] array, Func<valueType, uint> getKey, int startIndex, int count)
        {
            return subArray<valueType>.Create(array, startIndex, count).getSortDesc(getKey);
        }

        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <returns>排序后的数组</returns>
        public static int[] sort(this int[] array)
        {
            if (array != null)
            {
                if (array.Length >= fastCSharp.pub.RadixSortSize64) algorithm.radixSort.Sort(array, 0, array.Length);
                else algorithm.quickSort.Sort(array);
                return array;
            }
            return nullValue<int>.Array;
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray<int> sort(this int[] array, int startIndex, int count)
        {
            return subArray<int>.Create(array, startIndex, count).sort();
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <returns>排序后的数组</returns>
        public static int[] sortDesc(this int[] array)
        {
            if (array != null)
            {
                if (array.Length >= fastCSharp.pub.RadixSortSize64) algorithm.radixSort.SortDesc(array, 0, array.Length);
                else algorithm.quickSort.SortDesc(array);
                return array;
            }
            return nullValue<int>.Array;
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray<int> sortDesc(this int[] array, int startIndex, int count)
        {
            return subArray<int>.Create(array, startIndex, count).sortDesc();
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <returns>排序后的新数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static int[] getSort(this int[] array)
        {
            return new subArray<int>(array).getSort();
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static int[] getSort(this int[] array, int startIndex, int count)
        {
            return subArray<int>.Create(array, startIndex, count).getSort();
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <returns>排序后的新数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static int[] getSortDesc(this int[] array)
        {
            return new subArray<int>(array).getSortDesc();
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static int[] getSortDesc(this int[] array, int startIndex, int count)
        {
            return subArray<int>.Create(array, startIndex, count).getSortDesc();
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组</param>
        /// <param name="getKey">排序键</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static unsafe valueType[] getSort<valueType>(this valueType[] array, Func<valueType, int> getKey)
        {
            return new subArray<valueType>(array).getSort(getKey);
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组</param>
        /// <param name="getKey">排序键</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static unsafe valueType[] getSort<valueType>(this valueType[] array, Func<valueType, int> getKey, int startIndex, int count)
        {
            return subArray<valueType>.Create(array, startIndex, count).getSort(getKey);
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组</param>
        /// <param name="getKey">排序键</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static unsafe valueType[] getSortDesc<valueType>(this valueType[] array, Func<valueType, int> getKey)
        {
            return new subArray<valueType>(array).getSortDesc(getKey);
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组</param>
        /// <param name="getKey">排序键</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static unsafe valueType[] getSortDesc<valueType>(this valueType[] array, Func<valueType, int> getKey, int startIndex, int count)
        {
            return subArray<valueType>.Create(array, startIndex, count).getSortDesc(getKey);
        }


        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <returns>排序后的数组</returns>
        public static DateTime[] sort(this DateTime[] array)
        {
            if (array != null)
            {
                if (array.Length >= fastCSharp.pub.RadixSortSize64) algorithm.radixSort.Sort(array, 0, array.Length);
                else algorithm.quickSort.Sort(array);
                return array;
            }
            return nullValue<DateTime>.Array;
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray<DateTime> sort(this DateTime[] array, int startIndex, int count)
        {
            return subArray<DateTime>.Create(array, startIndex, count).sort();
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <returns>排序后的数组</returns>
        public static DateTime[] sortDesc(this DateTime[] array)
        {
            if (array != null)
            {
                if (array.Length >= fastCSharp.pub.RadixSortSize64) algorithm.radixSort.SortDesc(array, 0, array.Length);
                else algorithm.quickSort.SortDesc(array);
                return array;
            }
            return nullValue<DateTime>.Array;
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray<DateTime> sortDesc(this DateTime[] array, int startIndex, int count)
        {
            return subArray<DateTime>.Create(array, startIndex, count).sortDesc();
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <returns>排序后的新数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static DateTime[] getSort(this DateTime[] array)
        {
            return new subArray<DateTime>(array).getSort();
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static DateTime[] getSort(this DateTime[] array, int startIndex, int count)
        {
            return subArray<DateTime>.Create(array, startIndex, count).getSort();
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <returns>排序后的新数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static DateTime[] getSortDesc(this DateTime[] array)
        {
            return new subArray<DateTime>(array).getSortDesc();
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static DateTime[] getSortDesc(this DateTime[] array, int startIndex, int count)
        {
            return subArray<DateTime>.Create(array, startIndex, count).getSortDesc();
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组</param>
        /// <param name="getKey">排序键</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static unsafe valueType[] getSort<valueType>(this valueType[] array, Func<valueType, DateTime> getKey)
        {
            return new subArray<valueType>(array).getSort(getKey);
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组</param>
        /// <param name="getKey">排序键</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static unsafe valueType[] getSort<valueType>(this valueType[] array, Func<valueType, DateTime> getKey, int startIndex, int count)
        {
            return subArray<valueType>.Create(array, startIndex, count).getSort(getKey);
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组</param>
        /// <param name="getKey">排序键</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static unsafe valueType[] getSortDesc<valueType>(this valueType[] array, Func<valueType, DateTime> getKey)
        {
            return new subArray<valueType>(array).getSortDesc(getKey);
        }
        /// <summary>
        /// 数组排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组</param>
        /// <param name="getKey">排序键</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static unsafe valueType[] getSortDesc<valueType>(this valueType[] array, Func<valueType, DateTime> getKey, int startIndex, int count)
        {
            return subArray<valueType>.Create(array, startIndex, count).getSortDesc(getKey);
        }
    }
}
