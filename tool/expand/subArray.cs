using System;
using fastCSharp.algorithm;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 数组子串扩展操作
    /// </summary>
    public static partial class subArrayExtension
    {
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <returns>排序后的数组子串</returns>
        public static subArray<ulong> sort(this subArray<ulong> array)
        {
            if (array.Count > 1)
            {
                if (array.Count >= fastCSharp.pub.RadixSortSize64) algorithm.radixSort.Sort(array.UnsafeArray, array.StartIndex, array.Count);
                else algorithm.quickSort.Sort(array.UnsafeArray, array.StartIndex, array.Count);
            }
            return array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        public static subArray<ulong> sort(this subArray<ulong> array, int startIndex, int count)
        {
            if (array.Count != 0)
            {
                array.range range = new array.range(array.Count, startIndex, count);
                if (range.GetCount >= fastCSharp.pub.RadixSortSize64) algorithm.radixSort.Sort(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
                else if (range.GetCount > 1) algorithm.quickSort.Sort(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
                return subArray<ulong>.Unsafe(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
            }
            return array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <returns>排序后的数组</returns>
        public static subArray<ulong> sortDesc(this subArray<ulong> array)
        {
            if (array.Count > 1)
            {
                if (array.Count >= fastCSharp.pub.RadixSortSize64) algorithm.radixSort.SortDesc(array.UnsafeArray, array.StartIndex, array.Count);
                else algorithm.quickSort.SortDesc(array.UnsafeArray, array.StartIndex, array.Count);
            }
            return array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        public static subArray<ulong> sortDesc(this subArray<ulong> array, int startIndex, int count)
        {
            if (array.Count != 0)
            {
                array.range range = new array.range(array.Count, startIndex, count);
                if (range.GetCount >= fastCSharp.pub.RadixSortSize64) algorithm.radixSort.SortDesc(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
                else if (range.GetCount > 1) algorithm.quickSort.SortDesc(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
                return subArray<ulong>.Unsafe(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
            }
            return array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <returns>排序后的新数组</returns>
        public static ulong[] getSort(this subArray<ulong> array)
        {
            if (array.Count > 1)
            {
                if (array.Count >= fastCSharp.pub.RadixSortSize64) return algorithm.radixSort.GetSort(array.UnsafeArray, array.StartIndex, array.Count);
                return algorithm.quickSort.GetSort(array.UnsafeArray, array.StartIndex, array.Count);
            }
            return array.GetArray();
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        public static ulong[] getSort(this subArray<ulong> array, int startIndex, int count)
        {
            if (array.Count != 0)
            {
                array.range range = new array.range(array.Count, startIndex, count);
                if (range.GetCount >= fastCSharp.pub.RadixSortSize64) return algorithm.radixSort.GetSort(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
                return algorithm.quickSort.GetSort(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
            }
            return nullValue<ulong>.Array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <returns>排序后的新数组</returns>
        public static ulong[] getSortDesc(this subArray<ulong> array)
        {
            if (array.Count > 1)
            {
                if (array.Count >= fastCSharp.pub.RadixSortSize64) return algorithm.radixSort.GetSortDesc(array.UnsafeArray, array.StartIndex, array.Count);
                return algorithm.quickSort.GetSortDesc(array.UnsafeArray, array.StartIndex, array.Count);
            }
            return array.GetArray();
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        public static ulong[] getSortDesc(this subArray<ulong> array, int startIndex, int count)
        {
            if (array.Count != 0)
            {
                array.range range = new array.range(array.Count, startIndex, count);
                if (range.GetCount >= fastCSharp.pub.RadixSortSize64) return algorithm.radixSort.GetSortDesc(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
                return algorithm.quickSort.GetSortDesc(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
            }
            return nullValue<ulong>.Array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组子串</param>
        /// <param name="getKey">排序键</param>
        /// <returns>排序后的数组</returns>
        public static unsafe valueType[] getSort<valueType>(this subArray<valueType> array, Func<valueType, ulong> getKey)
        {
            if (getKey == null) log.Error.Throw(log.exceptionType.Null);
            if (array.Count > 1)
            {
                if (array.Count >= fastCSharp.pub.RadixSortSize64)
                {
                    ulongSortIndex[] indexs = new ulongSortIndex[array.Count << 1];
                    fixed (ulongSortIndex* indexFixed = indexs)
                    {
                        ulongSortIndex.Create(indexFixed, array.UnsafeArray, getKey, array.StartIndex, array.Count);
                        algorithm.radixSort.Sort(indexFixed, indexFixed + array.Count, array.Count);
                        return ulongSortIndex.Create(indexFixed, array.UnsafeArray, array.Count);
                    }
                }
                return algorithm.quickSort.GetSort(array.UnsafeArray, getKey, array.StartIndex, array.Count);
            }
            return array.GetArray();
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组子串</param>
        /// <param name="getKey">排序键</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        public static unsafe valueType[] getSort<valueType>(this subArray<valueType> array, Func<valueType, ulong> getKey, int startIndex, int count)
        {
            if (getKey == null) log.Error.Throw(log.exceptionType.Null);
            if (array.Count != 0)
            {
                array.range range = new array.range(array.Count, startIndex, count);
                if (range.GetCount >= fastCSharp.pub.RadixSortSize64)
                {
                    ulongSortIndex[] indexs = new ulongSortIndex[range.GetCount << 1];
                    fixed (ulongSortIndex* indexFixed = indexs)
                    {
                        ulongSortIndex.Create(indexFixed, array.UnsafeArray, getKey, array.StartIndex + range.SkipCount, range.GetCount);
                        algorithm.radixSort.Sort(indexFixed, indexFixed + range.GetCount, range.GetCount);
                        return ulongSortIndex.Create(indexFixed, array.UnsafeArray, range.GetCount);
                    }
                }
                if (range.GetCount > 1) return algorithm.quickSort.GetSort(array.UnsafeArray, getKey, array.StartIndex + range.SkipCount, range.GetCount);
                if (range.GetCount != 0) return new valueType[] { array.UnsafeArray[array.StartIndex + range.SkipCount] };
            }
            return nullValue<valueType>.Array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组子串</param>
        /// <param name="getKey">排序键</param>
        /// <returns>排序后的数组</returns>
        public static unsafe valueType[] getSortDesc<valueType>(this subArray<valueType> array, Func<valueType, ulong> getKey)
        {
            if (getKey == null) log.Error.Throw(log.exceptionType.Null);
            if (array.Count > 1)
            {
                if (array.Count >= fastCSharp.pub.RadixSortSize64)
                {
                    ulongSortIndex[] indexs = new ulongSortIndex[array.Count << 1];
                    fixed (ulongSortIndex* indexFixed = indexs)
                    {
                        ulongSortIndex.Create(indexFixed, array.UnsafeArray, getKey, array.StartIndex, array.Count);
                        algorithm.radixSort.SortDesc(indexFixed, indexFixed + array.Count, array.Count);
                        return ulongSortIndex.Create(indexFixed, array.UnsafeArray, array.Count);
                    }
                }
                return algorithm.quickSort.GetSortDesc(array.UnsafeArray, getKey, array.StartIndex, array.Count);
            }
            return array.GetArray();
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组子串</param>
        /// <param name="getKey">排序键</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        public static unsafe valueType[] getSortDesc<valueType>(this subArray<valueType> array, Func<valueType, ulong> getKey, int startIndex, int count)
        {
            if (getKey == null) log.Error.Throw(log.exceptionType.Null);
            if (array.Count != 0)
            {
                array.range range = new array.range(array.Count, startIndex, count);
                if (range.GetCount >= fastCSharp.pub.RadixSortSize64)
                {
                    ulongSortIndex[] indexs = new ulongSortIndex[range.GetCount << 1];
                    fixed (ulongSortIndex* indexFixed = indexs)
                    {
                        ulongSortIndex.Create(indexFixed, array.UnsafeArray, getKey, array.StartIndex + range.SkipCount, range.GetCount);
                        algorithm.radixSort.SortDesc(indexFixed, indexFixed + range.GetCount, range.GetCount);
                        return ulongSortIndex.Create(indexFixed, array.UnsafeArray, range.GetCount);
                    }
                }
                if (range.GetCount > 1) return algorithm.quickSort.GetSortDesc(array.UnsafeArray, getKey, array.StartIndex + range.SkipCount, range.GetCount);
                if (range.GetCount != 0) return new valueType[] { array.UnsafeArray[array.StartIndex + range.SkipCount] };
            }
            return nullValue<valueType>.Array;
        }

        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <returns>排序后的数组子串</returns>
        public static subArray<long> sort(this subArray<long> array)
        {
            if (array.Count > 1)
            {
                if (array.Count >= fastCSharp.pub.RadixSortSize64) algorithm.radixSort.Sort(array.UnsafeArray, array.StartIndex, array.Count);
                else algorithm.quickSort.Sort(array.UnsafeArray, array.StartIndex, array.Count);
            }
            return array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        public static subArray<long> sort(this subArray<long> array, int startIndex, int count)
        {
            if (array.Count != 0)
            {
                array.range range = new array.range(array.Count, startIndex, count);
                if (range.GetCount >= fastCSharp.pub.RadixSortSize64) algorithm.radixSort.Sort(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
                else if (range.GetCount > 1) algorithm.quickSort.Sort(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
                return subArray<long>.Unsafe(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
            }
            return array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <returns>排序后的数组</returns>
        public static subArray<long> sortDesc(this subArray<long> array)
        {
            if (array.Count > 1)
            {
                if (array.Count >= fastCSharp.pub.RadixSortSize64) algorithm.radixSort.SortDesc(array.UnsafeArray, array.StartIndex, array.Count);
                else algorithm.quickSort.SortDesc(array.UnsafeArray, array.StartIndex, array.Count);
            }
            return array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        public static subArray<long> sortDesc(this subArray<long> array, int startIndex, int count)
        {
            if (array.Count != 0)
            {
                array.range range = new array.range(array.Count, startIndex, count);
                if (range.GetCount >= fastCSharp.pub.RadixSortSize64) algorithm.radixSort.SortDesc(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
                else if (range.GetCount > 1) algorithm.quickSort.SortDesc(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
                return subArray<long>.Unsafe(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
            }
            return array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <returns>排序后的新数组</returns>
        public static long[] getSort(this subArray<long> array)
        {
            if (array.Count > 1)
            {
                if (array.Count >= fastCSharp.pub.RadixSortSize64) return algorithm.radixSort.GetSort(array.UnsafeArray, array.StartIndex, array.Count);
                return algorithm.quickSort.GetSort(array.UnsafeArray, array.StartIndex, array.Count);
            }
            return array.GetArray();
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        public static long[] getSort(this subArray<long> array, int startIndex, int count)
        {
            if (array.Count != 0)
            {
                array.range range = new array.range(array.Count, startIndex, count);
                if (range.GetCount >= fastCSharp.pub.RadixSortSize64) return algorithm.radixSort.GetSort(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
                return algorithm.quickSort.GetSort(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
            }
            return nullValue<long>.Array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <returns>排序后的新数组</returns>
        public static long[] getSortDesc(this subArray<long> array)
        {
            if (array.Count > 1)
            {
                if (array.Count >= fastCSharp.pub.RadixSortSize64) return algorithm.radixSort.GetSortDesc(array.UnsafeArray, array.StartIndex, array.Count);
                return algorithm.quickSort.GetSortDesc(array.UnsafeArray, array.StartIndex, array.Count);
            }
            return array.GetArray();
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        public static long[] getSortDesc(this subArray<long> array, int startIndex, int count)
        {
            if (array.Count != 0)
            {
                array.range range = new array.range(array.Count, startIndex, count);
                if (range.GetCount >= fastCSharp.pub.RadixSortSize64) return algorithm.radixSort.GetSortDesc(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
                return algorithm.quickSort.GetSortDesc(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
            }
            return nullValue<long>.Array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组子串</param>
        /// <param name="getKey">排序键</param>
        /// <returns>排序后的数组</returns>
        public static unsafe valueType[] getSort<valueType>(this subArray<valueType> array, Func<valueType, long> getKey)
        {
            if (getKey == null) log.Error.Throw(log.exceptionType.Null);
            if (array.Count > 1)
            {
                if (array.Count >= fastCSharp.pub.RadixSortSize64)
                {
                    longSortIndex[] indexs = new longSortIndex[array.Count << 1];
                    fixed (longSortIndex* indexFixed = indexs)
                    {
                        longSortIndex.Create(indexFixed, array.UnsafeArray, getKey, array.StartIndex, array.Count);
                        algorithm.radixSort.Sort(indexFixed, (ulongSortIndex*)(indexFixed + array.Count), array.Count);
                        return longSortIndex.Create(indexFixed, array.UnsafeArray, array.Count);
                    }
                }
                return algorithm.quickSort.GetSort(array.UnsafeArray, getKey, array.StartIndex, array.Count);
            }
            return array.GetArray();
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组子串</param>
        /// <param name="getKey">排序键</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        public static unsafe valueType[] getSort<valueType>(this subArray<valueType> array, Func<valueType, long> getKey, int startIndex, int count)
        {
            if (getKey == null) log.Error.Throw(log.exceptionType.Null);
            if (array.Count != 0)
            {
                array.range range = new array.range(array.Count, startIndex, count);
                if (range.GetCount >= fastCSharp.pub.RadixSortSize64)
                {
                    longSortIndex[] indexs = new longSortIndex[range.GetCount << 1];
                    fixed (longSortIndex* indexFixed = indexs)
                    {
                        longSortIndex.Create(indexFixed, array.UnsafeArray, getKey, array.StartIndex + range.SkipCount, range.GetCount);
                        algorithm.radixSort.Sort(indexFixed, (ulongSortIndex*)(indexFixed + range.GetCount), range.GetCount);
                        return longSortIndex.Create(indexFixed, array.UnsafeArray, range.GetCount);
                    }
                }
                if (range.GetCount > 1) return algorithm.quickSort.GetSort(array.UnsafeArray, getKey, array.StartIndex + range.SkipCount, range.GetCount);
                if (range.GetCount != 0) return new valueType[] { array.UnsafeArray[array.StartIndex + range.SkipCount] };
            }
            return nullValue<valueType>.Array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组子串</param>
        /// <param name="getKey">排序键</param>
        /// <returns>排序后的数组</returns>
        public static unsafe valueType[] getSortDesc<valueType>(this subArray<valueType> array, Func<valueType, long> getKey)
        {
            if (getKey == null) log.Error.Throw(log.exceptionType.Null);
            if (array.Count > 1)
            {
                if (array.Count >= fastCSharp.pub.RadixSortSize64)
                {
                    longSortIndex[] indexs = new longSortIndex[array.Count << 1];
                    fixed (longSortIndex* indexFixed = indexs)
                    {
                        longSortIndex.Create(indexFixed, array.UnsafeArray, getKey, array.StartIndex, array.Count);
                        algorithm.radixSort.SortDesc(indexFixed, (ulongSortIndex*)(indexFixed + array.Count), array.Count);
                        return longSortIndex.Create(indexFixed, array.UnsafeArray, array.Count);
                    }
                }
                return algorithm.quickSort.GetSortDesc(array.UnsafeArray, getKey, array.StartIndex, array.Count);
            }
            return array.GetArray();
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组子串</param>
        /// <param name="getKey">排序键</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        public static unsafe valueType[] getSortDesc<valueType>(this subArray<valueType> array, Func<valueType, long> getKey, int startIndex, int count)
        {
            if (getKey == null) log.Error.Throw(log.exceptionType.Null);
            if (array.Count != 0)
            {
                array.range range = new array.range(array.Count, startIndex, count);
                if (range.GetCount >= fastCSharp.pub.RadixSortSize64)
                {
                    longSortIndex[] indexs = new longSortIndex[range.GetCount << 1];
                    fixed (longSortIndex* indexFixed = indexs)
                    {
                        longSortIndex.Create(indexFixed, array.UnsafeArray, getKey, array.StartIndex + range.SkipCount, range.GetCount);
                        algorithm.radixSort.SortDesc(indexFixed, (ulongSortIndex*)(indexFixed + range.GetCount), range.GetCount);
                        return longSortIndex.Create(indexFixed, array.UnsafeArray, range.GetCount);
                    }
                }
                if (range.GetCount > 1) return algorithm.quickSort.GetSortDesc(array.UnsafeArray, getKey, array.StartIndex + range.SkipCount, range.GetCount);
                if (range.GetCount != 0) return new valueType[] { array.UnsafeArray[array.StartIndex + range.SkipCount] };
            }
            return nullValue<valueType>.Array;
        }

        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <returns>排序后的数组子串</returns>
        public static subArray<uint> sort(this subArray<uint> array)
        {
            if (array.Count > 1)
            {
                if (array.Count >= fastCSharp.pub.RadixSortSize64) algorithm.radixSort.Sort(array.UnsafeArray, array.StartIndex, array.Count);
                else algorithm.quickSort.Sort(array.UnsafeArray, array.StartIndex, array.Count);
            }
            return array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        public static subArray<uint> sort(this subArray<uint> array, int startIndex, int count)
        {
            if (array.Count != 0)
            {
                array.range range = new array.range(array.Count, startIndex, count);
                if (range.GetCount >= fastCSharp.pub.RadixSortSize64) algorithm.radixSort.Sort(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
                else if (range.GetCount > 1) algorithm.quickSort.Sort(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
                return subArray<uint>.Unsafe(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
            }
            return array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <returns>排序后的数组</returns>
        public static subArray<uint> sortDesc(this subArray<uint> array)
        {
            if (array.Count > 1)
            {
                if (array.Count >= fastCSharp.pub.RadixSortSize64) algorithm.radixSort.SortDesc(array.UnsafeArray, array.StartIndex, array.Count);
                else algorithm.quickSort.SortDesc(array.UnsafeArray, array.StartIndex, array.Count);
            }
            return array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        public static subArray<uint> sortDesc(this subArray<uint> array, int startIndex, int count)
        {
            if (array.Count != 0)
            {
                array.range range = new array.range(array.Count, startIndex, count);
                if (range.GetCount >= fastCSharp.pub.RadixSortSize64) algorithm.radixSort.SortDesc(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
                else if (range.GetCount > 1) algorithm.quickSort.SortDesc(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
                return subArray<uint>.Unsafe(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
            }
            return array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <returns>排序后的新数组</returns>
        public static uint[] getSort(this subArray<uint> array)
        {
            if (array.Count > 1)
            {
                if (array.Count >= fastCSharp.pub.RadixSortSize64) return algorithm.radixSort.GetSort(array.UnsafeArray, array.StartIndex, array.Count);
                return algorithm.quickSort.GetSort(array.UnsafeArray, array.StartIndex, array.Count);
            }
            return array.GetArray();
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        public static uint[] getSort(this subArray<uint> array, int startIndex, int count)
        {
            if (array.Count != 0)
            {
                array.range range = new array.range(array.Count, startIndex, count);
                if (range.GetCount >= fastCSharp.pub.RadixSortSize64) return algorithm.radixSort.GetSort(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
                return algorithm.quickSort.GetSort(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
            }
            return nullValue<uint>.Array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <returns>排序后的新数组</returns>
        public static uint[] getSortDesc(this subArray<uint> array)
        {
            if (array.Count > 1)
            {
                if (array.Count >= fastCSharp.pub.RadixSortSize64) return algorithm.radixSort.GetSortDesc(array.UnsafeArray, array.StartIndex, array.Count);
                return algorithm.quickSort.GetSortDesc(array.UnsafeArray, array.StartIndex, array.Count);
            }
            return array.GetArray();
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        public static uint[] getSortDesc(this subArray<uint> array, int startIndex, int count)
        {
            if (array.Count != 0)
            {
                array.range range = new array.range(array.Count, startIndex, count);
                if (range.GetCount >= fastCSharp.pub.RadixSortSize64) return algorithm.radixSort.GetSortDesc(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
                return algorithm.quickSort.GetSortDesc(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
            }
            return nullValue<uint>.Array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组子串</param>
        /// <param name="getKey">排序键</param>
        /// <returns>排序后的数组</returns>
        public static unsafe valueType[] getSort<valueType>(this subArray<valueType> array, Func<valueType, uint> getKey)
        {
            if (getKey == null) log.Error.Throw(log.exceptionType.Null);
            if (array.Count > 1)
            {
                if (array.Count >= fastCSharp.pub.RadixSortSize64)
                {
                    uintSortIndex[] indexs = new uintSortIndex[array.Count << 1];
                    fixed (uintSortIndex* indexFixed = indexs)
                    {
                        uintSortIndex.Create(indexFixed, array.UnsafeArray, getKey, array.StartIndex, array.Count);
                        algorithm.radixSort.Sort(indexFixed, indexFixed + array.Count, array.Count);
                        return uintSortIndex.Create(indexFixed, array.UnsafeArray, array.Count);
                    }
                }
                return algorithm.quickSort.GetSort(array.UnsafeArray, getKey, array.StartIndex, array.Count);
            }
            return array.GetArray();
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组子串</param>
        /// <param name="getKey">排序键</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        public static unsafe valueType[] getSort<valueType>(this subArray<valueType> array, Func<valueType, uint> getKey, int startIndex, int count)
        {
            if (getKey == null) log.Error.Throw(log.exceptionType.Null);
            if (array.Count != 0)
            {
                array.range range = new array.range(array.Count, startIndex, count);
                if (range.GetCount >= fastCSharp.pub.RadixSortSize64)
                {
                    uintSortIndex[] indexs = new uintSortIndex[range.GetCount << 1];
                    fixed (uintSortIndex* indexFixed = indexs)
                    {
                        uintSortIndex.Create(indexFixed, array.UnsafeArray, getKey, array.StartIndex + range.SkipCount, range.GetCount);
                        algorithm.radixSort.Sort(indexFixed, indexFixed + range.GetCount, range.GetCount);
                        return uintSortIndex.Create(indexFixed, array.UnsafeArray, range.GetCount);
                    }
                }
                if (range.GetCount > 1) return algorithm.quickSort.GetSort(array.UnsafeArray, getKey, array.StartIndex + range.SkipCount, range.GetCount);
                if (range.GetCount != 0) return new valueType[] { array.UnsafeArray[array.StartIndex + range.SkipCount] };
            }
            return nullValue<valueType>.Array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组子串</param>
        /// <param name="getKey">排序键</param>
        /// <returns>排序后的数组</returns>
        public static unsafe valueType[] getSortDesc<valueType>(this subArray<valueType> array, Func<valueType, uint> getKey)
        {
            if (getKey == null) log.Error.Throw(log.exceptionType.Null);
            if (array.Count > 1)
            {
                if (array.Count >= fastCSharp.pub.RadixSortSize64)
                {
                    uintSortIndex[] indexs = new uintSortIndex[array.Count << 1];
                    fixed (uintSortIndex* indexFixed = indexs)
                    {
                        uintSortIndex.Create(indexFixed, array.UnsafeArray, getKey, array.StartIndex, array.Count);
                        algorithm.radixSort.SortDesc(indexFixed, indexFixed + array.Count, array.Count);
                        return uintSortIndex.Create(indexFixed, array.UnsafeArray, array.Count);
                    }
                }
                return algorithm.quickSort.GetSortDesc(array.UnsafeArray, getKey, array.StartIndex, array.Count);
            }
            return array.GetArray();
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组子串</param>
        /// <param name="getKey">排序键</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        public static unsafe valueType[] getSortDesc<valueType>(this subArray<valueType> array, Func<valueType, uint> getKey, int startIndex, int count)
        {
            if (getKey == null) log.Error.Throw(log.exceptionType.Null);
            if (array.Count != 0)
            {
                array.range range = new array.range(array.Count, startIndex, count);
                if (range.GetCount >= fastCSharp.pub.RadixSortSize64)
                {
                    uintSortIndex[] indexs = new uintSortIndex[range.GetCount << 1];
                    fixed (uintSortIndex* indexFixed = indexs)
                    {
                        uintSortIndex.Create(indexFixed, array.UnsafeArray, getKey, array.StartIndex + range.SkipCount, range.GetCount);
                        algorithm.radixSort.SortDesc(indexFixed, indexFixed + range.GetCount, range.GetCount);
                        return uintSortIndex.Create(indexFixed, array.UnsafeArray, range.GetCount);
                    }
                }
                if (range.GetCount > 1) return algorithm.quickSort.GetSortDesc(array.UnsafeArray, getKey, array.StartIndex + range.SkipCount, range.GetCount);
                if (range.GetCount != 0) return new valueType[] { array.UnsafeArray[array.StartIndex + range.SkipCount] };
            }
            return nullValue<valueType>.Array;
        }

        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <returns>排序后的数组子串</returns>
        public static subArray<int> sort(this subArray<int> array)
        {
            if (array.Count > 1)
            {
                if (array.Count >= fastCSharp.pub.RadixSortSize64) algorithm.radixSort.Sort(array.UnsafeArray, array.StartIndex, array.Count);
                else algorithm.quickSort.Sort(array.UnsafeArray, array.StartIndex, array.Count);
            }
            return array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        public static subArray<int> sort(this subArray<int> array, int startIndex, int count)
        {
            if (array.Count != 0)
            {
                array.range range = new array.range(array.Count, startIndex, count);
                if (range.GetCount >= fastCSharp.pub.RadixSortSize64) algorithm.radixSort.Sort(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
                else if (range.GetCount > 1) algorithm.quickSort.Sort(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
                return subArray<int>.Unsafe(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
            }
            return array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <returns>排序后的数组</returns>
        public static subArray<int> sortDesc(this subArray<int> array)
        {
            if (array.Count > 1)
            {
                if (array.Count >= fastCSharp.pub.RadixSortSize64) algorithm.radixSort.SortDesc(array.UnsafeArray, array.StartIndex, array.Count);
                else algorithm.quickSort.SortDesc(array.UnsafeArray, array.StartIndex, array.Count);
            }
            return array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        public static subArray<int> sortDesc(this subArray<int> array, int startIndex, int count)
        {
            if (array.Count != 0)
            {
                array.range range = new array.range(array.Count, startIndex, count);
                if (range.GetCount >= fastCSharp.pub.RadixSortSize64) algorithm.radixSort.SortDesc(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
                else if (range.GetCount > 1) algorithm.quickSort.SortDesc(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
                return subArray<int>.Unsafe(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
            }
            return array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <returns>排序后的新数组</returns>
        public static int[] getSort(this subArray<int> array)
        {
            if (array.Count > 1)
            {
                if (array.Count >= fastCSharp.pub.RadixSortSize64) return algorithm.radixSort.GetSort(array.UnsafeArray, array.StartIndex, array.Count);
                return algorithm.quickSort.GetSort(array.UnsafeArray, array.StartIndex, array.Count);
            }
            return array.GetArray();
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        public static int[] getSort(this subArray<int> array, int startIndex, int count)
        {
            if (array.Count != 0)
            {
                array.range range = new array.range(array.Count, startIndex, count);
                if (range.GetCount >= fastCSharp.pub.RadixSortSize64) return algorithm.radixSort.GetSort(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
                return algorithm.quickSort.GetSort(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
            }
            return nullValue<int>.Array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <returns>排序后的新数组</returns>
        public static int[] getSortDesc(this subArray<int> array)
        {
            if (array.Count > 1)
            {
                if (array.Count >= fastCSharp.pub.RadixSortSize64) return algorithm.radixSort.GetSortDesc(array.UnsafeArray, array.StartIndex, array.Count);
                return algorithm.quickSort.GetSortDesc(array.UnsafeArray, array.StartIndex, array.Count);
            }
            return array.GetArray();
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        public static int[] getSortDesc(this subArray<int> array, int startIndex, int count)
        {
            if (array.Count != 0)
            {
                array.range range = new array.range(array.Count, startIndex, count);
                if (range.GetCount >= fastCSharp.pub.RadixSortSize64) return algorithm.radixSort.GetSortDesc(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
                return algorithm.quickSort.GetSortDesc(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
            }
            return nullValue<int>.Array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组子串</param>
        /// <param name="getKey">排序键</param>
        /// <returns>排序后的数组</returns>
        public static unsafe valueType[] getSort<valueType>(this subArray<valueType> array, Func<valueType, int> getKey)
        {
            if (getKey == null) log.Error.Throw(log.exceptionType.Null);
            if (array.Count > 1)
            {
                if (array.Count >= fastCSharp.pub.RadixSortSize64)
                {
                    intSortIndex[] indexs = new intSortIndex[array.Count << 1];
                    fixed (intSortIndex* indexFixed = indexs)
                    {
                        intSortIndex.Create(indexFixed, array.UnsafeArray, getKey, array.StartIndex, array.Count);
                        algorithm.radixSort.Sort(indexFixed, (uintSortIndex*)(indexFixed + array.Count), array.Count);
                        return intSortIndex.Create(indexFixed, array.UnsafeArray, array.Count);
                    }
                }
                return algorithm.quickSort.GetSort(array.UnsafeArray, getKey, array.StartIndex, array.Count);
            }
            return array.GetArray();
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组子串</param>
        /// <param name="getKey">排序键</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        public static unsafe valueType[] getSort<valueType>(this subArray<valueType> array, Func<valueType, int> getKey, int startIndex, int count)
        {
            if (getKey == null) log.Error.Throw(log.exceptionType.Null);
            if (array.Count != 0)
            {
                array.range range = new array.range(array.Count, startIndex, count);
                if (range.GetCount >= fastCSharp.pub.RadixSortSize64)
                {
                    intSortIndex[] indexs = new intSortIndex[range.GetCount << 1];
                    fixed (intSortIndex* indexFixed = indexs)
                    {
                        intSortIndex.Create(indexFixed, array.UnsafeArray, getKey, array.StartIndex + range.SkipCount, range.GetCount);
                        algorithm.radixSort.Sort(indexFixed, (uintSortIndex*)(indexFixed + range.GetCount), range.GetCount);
                        return intSortIndex.Create(indexFixed, array.UnsafeArray, range.GetCount);
                    }
                }
                if (range.GetCount > 1) return algorithm.quickSort.GetSort(array.UnsafeArray, getKey, array.StartIndex + range.SkipCount, range.GetCount);
                if (range.GetCount != 0) return new valueType[] { array.UnsafeArray[array.StartIndex + range.SkipCount] };
            }
            return nullValue<valueType>.Array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组子串</param>
        /// <param name="getKey">排序键</param>
        /// <returns>排序后的数组</returns>
        public static unsafe valueType[] getSortDesc<valueType>(this subArray<valueType> array, Func<valueType, int> getKey)
        {
            if (getKey == null) log.Error.Throw(log.exceptionType.Null);
            if (array.Count > 1)
            {
                if (array.Count >= fastCSharp.pub.RadixSortSize64)
                {
                    intSortIndex[] indexs = new intSortIndex[array.Count << 1];
                    fixed (intSortIndex* indexFixed = indexs)
                    {
                        intSortIndex.Create(indexFixed, array.UnsafeArray, getKey, array.StartIndex, array.Count);
                        algorithm.radixSort.SortDesc(indexFixed, (uintSortIndex*)(indexFixed + array.Count), array.Count);
                        return intSortIndex.Create(indexFixed, array.UnsafeArray, array.Count);
                    }
                }
                return algorithm.quickSort.GetSortDesc(array.UnsafeArray, getKey, array.StartIndex, array.Count);
            }
            return array.GetArray();
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组子串</param>
        /// <param name="getKey">排序键</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        public static unsafe valueType[] getSortDesc<valueType>(this subArray<valueType> array, Func<valueType, int> getKey, int startIndex, int count)
        {
            if (getKey == null) log.Error.Throw(log.exceptionType.Null);
            if (array.Count != 0)
            {
                array.range range = new array.range(array.Count, startIndex, count);
                if (range.GetCount >= fastCSharp.pub.RadixSortSize64)
                {
                    intSortIndex[] indexs = new intSortIndex[range.GetCount << 1];
                    fixed (intSortIndex* indexFixed = indexs)
                    {
                        intSortIndex.Create(indexFixed, array.UnsafeArray, getKey, array.StartIndex + range.SkipCount, range.GetCount);
                        algorithm.radixSort.SortDesc(indexFixed, (uintSortIndex*)(indexFixed + range.GetCount), range.GetCount);
                        return intSortIndex.Create(indexFixed, array.UnsafeArray, range.GetCount);
                    }
                }
                if (range.GetCount > 1) return algorithm.quickSort.GetSortDesc(array.UnsafeArray, getKey, array.StartIndex + range.SkipCount, range.GetCount);
                if (range.GetCount != 0) return new valueType[] { array.UnsafeArray[array.StartIndex + range.SkipCount] };
            }
            return nullValue<valueType>.Array;
        }

        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <returns>排序后的数组子串</returns>
        public static subArray<DateTime> sort(this subArray<DateTime> array)
        {
            if (array.Count > 1)
            {
                if (array.Count >= fastCSharp.pub.RadixSortSize64) algorithm.radixSort.Sort(array.UnsafeArray, array.StartIndex, array.Count);
                else algorithm.quickSort.Sort(array.UnsafeArray, array.StartIndex, array.Count);
            }
            return array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        public static subArray<DateTime> sort(this subArray<DateTime> array, int startIndex, int count)
        {
            if (array.Count != 0)
            {
                array.range range = new array.range(array.Count, startIndex, count);
                if (range.GetCount >= fastCSharp.pub.RadixSortSize64) algorithm.radixSort.Sort(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
                else if (range.GetCount > 1) algorithm.quickSort.Sort(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
                return subArray<DateTime>.Unsafe(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
            }
            return array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <returns>排序后的数组</returns>
        public static subArray<DateTime> sortDesc(this subArray<DateTime> array)
        {
            if (array.Count > 1)
            {
                if (array.Count >= fastCSharp.pub.RadixSortSize64) algorithm.radixSort.SortDesc(array.UnsafeArray, array.StartIndex, array.Count);
                else algorithm.quickSort.SortDesc(array.UnsafeArray, array.StartIndex, array.Count);
            }
            return array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        public static subArray<DateTime> sortDesc(this subArray<DateTime> array, int startIndex, int count)
        {
            if (array.Count != 0)
            {
                array.range range = new array.range(array.Count, startIndex, count);
                if (range.GetCount >= fastCSharp.pub.RadixSortSize64) algorithm.radixSort.SortDesc(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
                else if (range.GetCount > 1) algorithm.quickSort.SortDesc(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
                return subArray<DateTime>.Unsafe(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
            }
            return array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <returns>排序后的新数组</returns>
        public static DateTime[] getSort(this subArray<DateTime> array)
        {
            if (array.Count > 1)
            {
                if (array.Count >= fastCSharp.pub.RadixSortSize64) return algorithm.radixSort.GetSort(array.UnsafeArray, array.StartIndex, array.Count);
                return algorithm.quickSort.GetSort(array.UnsafeArray, array.StartIndex, array.Count);
            }
            return array.GetArray();
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        public static DateTime[] getSort(this subArray<DateTime> array, int startIndex, int count)
        {
            if (array.Count != 0)
            {
                array.range range = new array.range(array.Count, startIndex, count);
                if (range.GetCount >= fastCSharp.pub.RadixSortSize64) return algorithm.radixSort.GetSort(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
                return algorithm.quickSort.GetSort(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
            }
            return nullValue<DateTime>.Array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <returns>排序后的新数组</returns>
        public static DateTime[] getSortDesc(this subArray<DateTime> array)
        {
            if (array.Count > 1)
            {
                if (array.Count >= fastCSharp.pub.RadixSortSize64) return algorithm.radixSort.GetSortDesc(array.UnsafeArray, array.StartIndex, array.Count);
                return algorithm.quickSort.GetSortDesc(array.UnsafeArray, array.StartIndex, array.Count);
            }
            return array.GetArray();
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <param name="array">待排序数组子串</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        public static DateTime[] getSortDesc(this subArray<DateTime> array, int startIndex, int count)
        {
            if (array.Count != 0)
            {
                array.range range = new array.range(array.Count, startIndex, count);
                if (range.GetCount >= fastCSharp.pub.RadixSortSize64) return algorithm.radixSort.GetSortDesc(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
                return algorithm.quickSort.GetSortDesc(array.UnsafeArray, array.StartIndex + range.SkipCount, range.GetCount);
            }
            return nullValue<DateTime>.Array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组子串</param>
        /// <param name="getKey">排序键</param>
        /// <returns>排序后的数组</returns>
        public static unsafe valueType[] getSort<valueType>(this subArray<valueType> array, Func<valueType, DateTime> getKey)
        {
            if (getKey == null) log.Error.Throw(log.exceptionType.Null);
            if (array.Count > 1)
            {
                if (array.Count >= fastCSharp.pub.RadixSortSize64)
                {
                    longSortIndex[] indexs = new longSortIndex[array.Count << 1];
                    fixed (longSortIndex* indexFixed = indexs)
                    {
                        dateTimeSortIndex.Create(indexFixed, array.UnsafeArray, getKey, array.StartIndex, array.Count);
                        algorithm.radixSort.Sort(indexFixed, (ulongSortIndex*)(indexFixed + array.Count), array.Count);
                        return longSortIndex.Create(indexFixed, array.UnsafeArray, array.Count);
                    }
                }
                return algorithm.quickSort.GetSort(array.UnsafeArray, getKey, array.StartIndex, array.Count);
            }
            return array.GetArray();
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组子串</param>
        /// <param name="getKey">排序键</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        public static unsafe valueType[] getSort<valueType>(this subArray<valueType> array, Func<valueType, DateTime> getKey, int startIndex, int count)
        {
            if (getKey == null) log.Error.Throw(log.exceptionType.Null);
            if (array.Count != 0)
            {
                array.range range = new array.range(array.Count, startIndex, count);
                if (range.GetCount >= fastCSharp.pub.RadixSortSize64)
                {
                    longSortIndex[] indexs = new longSortIndex[range.GetCount << 1];
                    fixed (longSortIndex* indexFixed = indexs)
                    {
                        dateTimeSortIndex.Create(indexFixed, array.UnsafeArray, getKey, array.StartIndex + range.SkipCount, range.GetCount);
                        algorithm.radixSort.Sort(indexFixed, (ulongSortIndex*)(indexFixed + range.GetCount), range.GetCount);
                        return longSortIndex.Create(indexFixed, array.UnsafeArray, range.GetCount);
                    }
                }
                if (range.GetCount > 1) return algorithm.quickSort.GetSort(array.UnsafeArray, getKey, array.StartIndex + range.SkipCount, range.GetCount);
                if (range.GetCount != 0) return new valueType[] { array.UnsafeArray[array.StartIndex + range.SkipCount] };
            }
            return nullValue<valueType>.Array;
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组子串</param>
        /// <param name="getKey">排序键</param>
        /// <returns>排序后的数组</returns>
        public static unsafe valueType[] getSortDesc<valueType>(this subArray<valueType> array, Func<valueType, DateTime> getKey)
        {
            if (getKey == null) log.Error.Throw(log.exceptionType.Null);
            if (array.Count > 1)
            {
                if (array.Count >= fastCSharp.pub.RadixSortSize64)
                {
                    longSortIndex[] indexs = new longSortIndex[array.Count << 1];
                    fixed (longSortIndex* indexFixed = indexs)
                    {
                        dateTimeSortIndex.Create(indexFixed, array.UnsafeArray, getKey, array.StartIndex, array.Count);
                        algorithm.radixSort.SortDesc(indexFixed, (ulongSortIndex*)(indexFixed + array.Count), array.Count);
                        return longSortIndex.Create(indexFixed, array.UnsafeArray, array.Count);
                    }
                }
                return algorithm.quickSort.GetSortDesc(array.UnsafeArray, getKey, array.StartIndex, array.Count);
            }
            return array.GetArray();
        }
        /// <summary>
        /// 数组子串排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组子串</param>
        /// <param name="getKey">排序键</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>排序后的数组</returns>
        public static unsafe valueType[] getSortDesc<valueType>(this subArray<valueType> array, Func<valueType, DateTime> getKey, int startIndex, int count)
        {
            if (getKey == null) log.Error.Throw(log.exceptionType.Null);
            if (array.Count != 0)
            {
                array.range range = new array.range(array.Count, startIndex, count);
                if (range.GetCount >= fastCSharp.pub.RadixSortSize64)
                {
                    longSortIndex[] indexs = new longSortIndex[range.GetCount << 1];
                    fixed (longSortIndex* indexFixed = indexs)
                    {
                        dateTimeSortIndex.Create(indexFixed, array.UnsafeArray, getKey, array.StartIndex + range.SkipCount, range.GetCount);
                        algorithm.radixSort.SortDesc(indexFixed, (ulongSortIndex*)(indexFixed + range.GetCount), range.GetCount);
                        return longSortIndex.Create(indexFixed, array.UnsafeArray, range.GetCount);
                    }
                }
                if (range.GetCount > 1) return algorithm.quickSort.GetSortDesc(array.UnsafeArray, getKey, array.StartIndex + range.SkipCount, range.GetCount);
                if (range.GetCount != 0) return new valueType[] { array.UnsafeArray[array.StartIndex + range.SkipCount] };
            }
            return nullValue<valueType>.Array;
        }
    }
}
