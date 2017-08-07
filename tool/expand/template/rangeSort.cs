using System;
/*Type:ulong,ulongRangeSorter,ulongSortIndex,ulongRangeIndexSorter;long,longRangeSorter,longSortIndex,longRangeIndexSorter;uint,uintRangeSorter,uintSortIndex,uintRangeIndexSorter;int,intRangeSorter,intSortIndex,intRangeIndexSorter;double,doubleRangeSorter,doubleSortIndex,doubleRangeIndexSorter;float,floatRangeSorter,floatSortIndex,floatRangeIndexSorter;DateTime,dateTimeRangeSorter,dateTimeSortIndex,dateTimeRangeIndexSorter*/
/*Compare:,>,<;Desc,<,>*/

namespace fastCSharp.algorithm
{
    /// <summary>
    /// 快速排序
    /// </summary>
    internal static partial class quickSort
    {
        /// <summary>
        /// 范围排序器(一般用于获取分页)
        /// </summary>
        internal unsafe struct /*Type[1]*/ulongRangeSorter/*Type[1]*//*Compare[0]*//*Compare[0]*/
        {
            /// <summary>
            /// 跳过数据指针
            /// </summary>
            public /*Type[0]*/ulong/*Type[0]*/* SkipCount;
            /// <summary>
            /// 最后一条记录指针-1
            /// </summary>
            public /*Type[0]*/ulong/*Type[0]*/* GetEndIndex;
            /// <summary>
            /// 范围排序
            /// </summary>
            /// <param name="startIndex">起始指针</param>
            /// <param name="endIndex">结束指针-1</param>
            public void Sort(/*Type[0]*/ulong/*Type[0]*/* startIndex, /*Type[0]*/ulong/*Type[0]*/* endIndex)
            {
                do
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/ leftValue = *startIndex, rightValue = *endIndex;
                    int average = (int)(endIndex - startIndex) >> 1;
                    if (average == 0)
                    {
                        if (leftValue /*Compare[1]*/>/*Compare[1]*/ rightValue)
                        {
                            *startIndex = rightValue;
                            *endIndex = leftValue;
                        }
                        break;
                    }
                    /*Type[0]*/
                    ulong/*Type[0]*/* averageIndex = startIndex + average, leftIndex = startIndex, rightIndex = endIndex;
                    /*Type[0]*/
                    ulong/*Type[0]*/ value = *averageIndex;
                    if (leftValue /*Compare[1]*/>/*Compare[1]*/ value)
                    {
                        if (leftValue /*Compare[1]*/>/*Compare[1]*/ rightValue)
                        {
                            *rightIndex = leftValue;
                            if (value /*Compare[1]*/>/*Compare[1]*/ rightValue) *leftIndex = rightValue;
                            else
                            {
                                *leftIndex = value;
                                *averageIndex = value = rightValue;
                            }
                        }
                        else
                        {
                            *leftIndex = value;
                            *averageIndex = value = leftValue;
                        }
                    }
                    else
                    {
                        if (value /*Compare[1]*/>/*Compare[1]*/ rightValue)
                        {
                            *rightIndex = value;
                            if (leftValue /*Compare[1]*/>/*Compare[1]*/ rightValue)
                            {
                                *leftIndex = rightValue;
                                *averageIndex = value = leftValue;
                            }
                            else *averageIndex = value = rightValue;
                        }
                    }
                    ++leftIndex;
                    --rightIndex;
                    do
                    {
                        while (*leftIndex /*Compare[2]*/</*Compare[2]*/ value) ++leftIndex;
                        while (value /*Compare[2]*/</*Compare[2]*/ *rightIndex) --rightIndex;
                        if (leftIndex < rightIndex)
                        {
                            leftValue = *leftIndex;
                            *leftIndex = *rightIndex;
                            *rightIndex = leftValue;
                        }
                        else
                        {
                            if (leftIndex == rightIndex)
                            {
                                ++leftIndex;
                                --rightIndex;
                            }
                            break;
                        }
                    }
                    while (++leftIndex <= --rightIndex);
                    if (rightIndex - startIndex <= endIndex - leftIndex)
                    {
                        if (startIndex < rightIndex && rightIndex >= SkipCount) Sort(startIndex, rightIndex);
                        if (leftIndex > GetEndIndex) break;
                        startIndex = leftIndex;
                    }
                    else
                    {
                        if (leftIndex < endIndex && leftIndex <= GetEndIndex) Sort(leftIndex, endIndex);
                        if (rightIndex < SkipCount) break;
                        endIndex = rightIndex;
                    }
                }
                while (startIndex < endIndex);
            }
        }
        /// <summary>
        /// 范围排序
        /// </summary>
        /// <param name="values">待排序数组</param>
        /// <param name="skipCount">跳过数据数量</param>
        /// <param name="getCount">排序数据数量</param>
        /// <returns>排序后的数据</returns>
        public unsafe static subArray</*Type[0]*/ulong/*Type[0]*/> RangeSort/*Compare[0]*//*Compare[0]*/
            (/*Type[0]*/ulong/*Type[0]*/[] values, int skipCount, int getCount)
        {
            array.range range = new array.range(values.length(), skipCount, getCount);
            if ((getCount = range.GetCount) != 0)
            {
                fixed (/*Type[0]*/ulong/*Type[0]*/* valueFixed = values)
                {
                    new /*Type[1]*/ulongRangeSorter/*Type[1]*//*Compare[0]*//*Compare[0]*/
                    {
                        SkipCount = valueFixed + range.SkipCount,
                        GetEndIndex = valueFixed + range.EndIndex - 1
                    }.Sort(valueFixed, valueFixed + values.Length - 1);
                }
                return subArray</*Type[0]*/ulong/*Type[0]*/>.Unsafe(values, range.SkipCount, getCount);
            }
            return default(subArray</*Type[0]*/ulong/*Type[0]*/>);
        }
        /// <summary>
        /// 范围排序
        /// </summary>
        /// <param name="values">待排序数组</param>
        /// <param name="skipCount">跳过数据数量</param>
        /// <param name="getCount">排序数据数量</param>
        /// <returns>排序后的新数据</returns>
        public unsafe static subArray</*Type[0]*/ulong/*Type[0]*/> GetRangeSort/*Compare[0]*//*Compare[0]*/
            (/*Type[0]*/ulong/*Type[0]*/[] values, int skipCount, int getCount)
        {
            array.range range = new array.range(values.length(), skipCount, getCount);
            if ((getCount = range.GetCount) != 0)
            {
                /*Type[0]*/
                ulong/*Type[0]*/[] newValues = new /*Type[0]*/ulong/*Type[0]*/[values.Length];
                Buffer.BlockCopy(values, 0, newValues, 0, values.Length * sizeof(/*Type[0]*/ulong/*Type[0]*/));
                fixed (/*Type[0]*/ulong/*Type[0]*/* newValueFixed = newValues, valueFixed = values)
                {
                    new /*Type[1]*/ulongRangeSorter/*Type[1]*//*Compare[0]*//*Compare[0]*/
                    {
                        SkipCount = newValueFixed + range.SkipCount,
                        GetEndIndex = newValueFixed + range.EndIndex - 1
                    }.Sort(newValueFixed, newValueFixed + values.Length - 1);
                }
                return subArray</*Type[0]*/ulong/*Type[0]*/>.Unsafe(newValues, range.SkipCount, getCount);
            }
            return default(subArray</*Type[0]*/ulong/*Type[0]*/>);
        }
        /// <summary>
        /// 范围排序
        /// </summary>
        /// <param name="values">待排序数组</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">排序范围数据数量</param>
        /// <param name="skipCount">跳过数据数量</param>
        /// <param name="getCount">排序数据数量</param>
        /// <returns>排序后的数据</returns>
        public unsafe static subArray</*Type[0]*/ulong/*Type[0]*/> RangeSort/*Compare[0]*//*Compare[0]*/
            (/*Type[0]*/ulong/*Type[0]*/[] values, int startIndex, int count, int skipCount, int getCount)
        {
            array.range range = new array.range(values.length(), startIndex, count);
            if ((count = range.GetCount) != 0)
            {
                array.range getRange = new array.range(count, skipCount, getCount);
                if ((getCount = getRange.GetCount) != 0)
                {
                    skipCount = range.SkipCount + getRange.SkipCount;
                    fixed (/*Type[0]*/ulong/*Type[0]*/* valueFixed = values)
                    {
                        /*Type[0]*/
                        ulong/*Type[0]*/* skip = valueFixed + skipCount, start = valueFixed + range.SkipCount;
                        new /*Type[1]*/ulongRangeSorter/*Type[1]*//*Compare[0]*//*Compare[0]*/
                        {
                            SkipCount = skip,
                            GetEndIndex = skip + getCount - 1
                        }.Sort(start, start + --count);
                    }
                    return subArray</*Type[0]*/ulong/*Type[0]*/>.Unsafe(values, skipCount, getCount);
                }
            }
            return default(subArray</*Type[0]*/ulong/*Type[0]*/>);
        }
        /// <summary>
        /// 范围排序
        /// </summary>
        /// <param name="values">待排序数组</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">排序范围数据数量</param>
        /// <param name="skipCount">跳过数据数量</param>
        /// <param name="getCount">排序数据数量</param>
        /// <returns>排序后的新数据</returns>
        public unsafe static subArray</*Type[0]*/ulong/*Type[0]*/> GetRangeSort/*Compare[0]*//*Compare[0]*/
            (/*Type[0]*/ulong/*Type[0]*/[] values, int startIndex, int count, int skipCount, int getCount)
        {
            array.range range = new array.range(values.length(), startIndex, count);
            if ((count = range.GetCount) != 0)
            {
                array.range getRange = new array.range(count, skipCount, getCount);
                if ((getCount = getRange.GetCount) != 0)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/[] newValues = new /*Type[0]*/ulong/*Type[0]*/[count];
                    Buffer.BlockCopy(values, range.SkipCount * sizeof(/*Type[0]*/ulong/*Type[0]*/), newValues, 0, count * sizeof(/*Type[0]*/ulong/*Type[0]*/));
                    fixed (/*Type[0]*/ulong/*Type[0]*/* newValueFixed = newValues, valueFixed = values)
                    {
                        new /*Type[1]*/ulongRangeSorter/*Type[1]*//*Compare[0]*//*Compare[0]*/
                        {
                            SkipCount = newValueFixed + getRange.SkipCount,
                            GetEndIndex = newValueFixed + getRange.SkipCount + getCount - 1
                        }.Sort(newValueFixed, newValueFixed + count - 1);
                    }
                    return subArray</*Type[0]*/ulong/*Type[0]*/>.Unsafe(newValues, getRange.SkipCount, getCount);
                }
            }
            return default(subArray</*Type[0]*/ulong/*Type[0]*/>);
        }

        /// <summary>
        /// 索引范围排序器
        /// </summary>
        internal unsafe struct /*Type[3]*/ulongRangeIndexSorter/*Type[3]*//*Compare[0]*//*Compare[0]*/
        {
            /// <summary>
            /// 跳过数据指针
            /// </summary>
            public /*Type[2]*/ulongSortIndex/*Type[2]*/* SkipCount;
            /// <summary>
            /// 最后一条记录指针-1
            /// </summary>
            public /*Type[2]*/ulongSortIndex/*Type[2]*/* GetEndIndex;
            /// <summary>
            /// 范围排序
            /// </summary>
            /// <param name="startIndex">起始指针</param>
            /// <param name="endIndex">结束指针-1</param>
            public void Sort(/*Type[2]*/ulongSortIndex/*Type[2]*/* startIndex, /*Type[2]*/ulongSortIndex/*Type[2]*/* endIndex)
            {
                do
                {
                    /*Type[2]*/
                    ulongSortIndex/*Type[2]*/ leftValue = *startIndex, rightValue = *endIndex;
                    int average = (int)(endIndex - startIndex) >> 1;
                    if (average == 0)
                    {
                        if (leftValue.Value /*Compare[1]*/>/*Compare[1]*/ rightValue.Value)
                        {
                            *startIndex = rightValue;
                            *endIndex = leftValue;
                        }
                        break;
                    }
                    /*Type[2]*/
                    ulongSortIndex/*Type[2]*/* averageIndex = startIndex + average, leftIndex = startIndex, rightIndex = endIndex;
                    /*Type[2]*/
                    ulongSortIndex/*Type[2]*/ indexValue = *averageIndex;
                    if (leftValue.Value /*Compare[1]*/>/*Compare[1]*/ indexValue.Value)
                    {
                        if (leftValue.Value /*Compare[1]*/>/*Compare[1]*/ rightValue.Value)
                        {
                            *rightIndex = leftValue;
                            if (indexValue.Value /*Compare[1]*/>/*Compare[1]*/ rightValue.Value) *leftIndex = rightValue;
                            else
                            {
                                *leftIndex = indexValue;
                                *averageIndex = indexValue = rightValue;
                            }
                        }
                        else
                        {
                            *leftIndex = indexValue;
                            *averageIndex = indexValue = leftValue;
                        }
                    }
                    else
                    {
                        if (indexValue.Value /*Compare[1]*/>/*Compare[1]*/ rightValue.Value)
                        {
                            *rightIndex = indexValue;
                            if (leftValue.Value /*Compare[1]*/>/*Compare[1]*/ rightValue.Value)
                            {
                                *leftIndex = rightValue;
                                *averageIndex = indexValue = leftValue;
                            }
                            else *averageIndex = indexValue = rightValue;
                        }
                    }
                    ++leftIndex;
                    --rightIndex;
                    /*Type[0]*/
                    ulong/*Type[0]*/ value = indexValue.Value;
                    do
                    {
                        while ((*leftIndex).Value /*Compare[2]*/</*Compare[2]*/ value) ++leftIndex;
                        while (value /*Compare[2]*/</*Compare[2]*/ (*rightIndex).Value) --rightIndex;
                        if (leftIndex < rightIndex)
                        {
                            leftValue = *leftIndex;
                            *leftIndex = *rightIndex;
                            *rightIndex = leftValue;
                        }
                        else
                        {
                            if (leftIndex == rightIndex)
                            {
                                ++leftIndex;
                                --rightIndex;
                            }
                            break;
                        }
                    }
                    while (++leftIndex <= --rightIndex);
                    if (rightIndex - startIndex <= endIndex - leftIndex)
                    {
                        if (startIndex < rightIndex && rightIndex >= SkipCount) Sort(startIndex, rightIndex);
                        if (leftIndex > GetEndIndex) break;
                        startIndex = leftIndex;
                    }
                    else
                    {
                        if (leftIndex < endIndex && leftIndex <= GetEndIndex) Sort(leftIndex, endIndex);
                        if (rightIndex < SkipCount) break;
                        endIndex = rightIndex;
                    }
                }
                while (startIndex < endIndex);
            }
        }
        /// <summary>
        /// 数组范围排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="values">待排序数组</param>
        /// <param name="getKey">排序键值获取器</param>
        /// <param name="skipCount">跳过数据数量</param>
        /// <param name="getCount">排序数据数量</param>
        /// <returns>排序后的数组</returns>
        public unsafe static valueType[] GetRangeSort/*Compare[0]*//*Compare[0]*/<valueType>
            (valueType[] values, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getKey, int skipCount, int getCount)
        {
            array.range range = new array.range(values.length(), skipCount, getCount);
            if ((getCount = range.GetCount) != 0)
            {
                if (getKey == null) log.Error.Throw(log.exceptionType.Null);
                unmanagedPool pool = fastCSharp.unmanagedPool.GetDefaultPool(values.Length * sizeof(/*Type[2]*/ulongSortIndex/*Type[2]*/));
                pointer.size data = pool.Get(values.Length * sizeof(/*Type[2]*/ulongSortIndex/*Type[2]*/));
                try
                {
                    return getRangeSort/*Compare[0]*//*Compare[0]*/(values, getKey, range.SkipCount, getCount, (/*Type[2]*/ulongSortIndex/*Type[2]*/*)data.Data);
                }
                finally { pool.Push(ref data); }
            }
            return nullValue<valueType>.Array;
        }
        /// <summary>
        /// 数组范围排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="values">待排序数组</param>
        /// <param name="getKey">排序键值获取器</param>
        /// <param name="skipCount">跳过数据数量</param>
        /// <param name="getCount">排序数据数量</param>
        /// <param name="fixedIndex">索引位置</param>
        /// <returns>排序后的数组</returns>
        private unsafe static valueType[] getRangeSort/*Compare[0]*//*Compare[0]*/<valueType>
            (valueType[] values, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getKey, int skipCount, int getCount
            , /*Type[2]*/ulongSortIndex/*Type[2]*/* fixedIndex)
        {
            /*Type[2]*/
            ulongSortIndex/*Type[2]*/* writeIndex = fixedIndex;
            for (int index = 0; index != values.Length; (*writeIndex++).Set(getKey(values[index]), index++)) ;
            return getRangeSort/*Compare[0]*//*Compare[0]*/(values, skipCount, getCount, fixedIndex);
        }
        /// <summary>
        /// 数组范围排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="values">待排序数组</param>
        /// <param name="skipCount">跳过数据数量</param>
        /// <param name="getCount">排序数据数量</param>
        /// <param name="fixedIndex">索引位置</param>
        /// <returns>排序后的数组</returns>
        private unsafe static valueType[] getRangeSort/*Compare[0]*//*Compare[0]*/<valueType>
            (valueType[] values, int skipCount, int getCount, /*Type[2]*/ulongSortIndex/*Type[2]*/* fixedIndex)
        {
            new /*Type[3]*/ulongRangeIndexSorter/*Type[3]*//*Compare[0]*//*Compare[0]*/
            {
                SkipCount = fixedIndex + skipCount,
                GetEndIndex = fixedIndex + skipCount + getCount - 1
            }.Sort(fixedIndex, fixedIndex + values.Length - 1);
            valueType[] newValues = new valueType[getCount];
            /*Type[2]*/
            ulongSortIndex/*Type[2]*/* writeIndex = fixedIndex + skipCount;
            for (int index = 0; index != newValues.Length; ++index) newValues[index] = values[(*writeIndex++).Index];
            return newValues;
        }
        /// <summary>
        /// 数组范围排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="values">待排序数组</param>
        /// <param name="indexs">排序索引</param>
        /// <param name="skipCount">跳过数据数量</param>
        /// <param name="getCount">排序数据数量</param>
        /// <returns>排序后的数组</returns>
        public unsafe static valueType[] GetRangeSort/*Compare[0]*//*Compare[0]*/<valueType>
            (valueType[] values, /*Type[2]*/ulongSortIndex/*Type[2]*/[] indexs, int skipCount, int getCount)
        {
            if (values.length() != indexs.length()) log.Error.Throw(log.exceptionType.IndexOutOfRange);
            array.range range = new array.range(values.length(), skipCount, getCount);
            if ((getCount = range.GetCount) != 0)
            {
                fixed (/*Type[2]*/ulongSortIndex/*Type[2]*/* fixedIndex = indexs) return getRangeSort/*Compare[0]*//*Compare[0]*/(values, skipCount, getCount, fixedIndex);
            }
            return nullValue<valueType>.Array;
        }
        /// <summary>
        /// 数组范围排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="values">待排序数组</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">排序范围数据数量</param>
        /// <param name="getKey">排序键值获取器</param>
        /// <param name="skipCount">跳过数据数量</param>
        /// <param name="getCount">排序数据数量</param>
        /// <returns>排序后的数组</returns>
        public unsafe static valueType[] GetRangeSort/*Compare[0]*//*Compare[0]*/<valueType>
            (valueType[] values, int startIndex, int count, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getKey, int skipCount, int getCount)
        {
            array.range range = new array.range(values.length(), startIndex, count);
            if ((count = range.GetCount) != 0)
            {
                array.range getRange = new array.range(count, skipCount, getCount);
                if ((getCount = getRange.GetCount) != 0)
                {
                    if (getKey == null) log.Error.Throw(log.exceptionType.Null);
                    unmanagedPool pool = fastCSharp.unmanagedPool.GetDefaultPool(count * sizeof(/*Type[2]*/ulongSortIndex/*Type[2]*/));
                    pointer.size data = pool.Get(count * sizeof(/*Type[2]*/ulongSortIndex/*Type[2]*/));
                    try
                    {
                        return getRangeSort/*Compare[0]*//*Compare[0]*/
                            (values, range.SkipCount, count, getKey, getRange.SkipCount, getCount, (/*Type[2]*/ulongSortIndex/*Type[2]*/*)data.Data);
                    }
                    finally { pool.Push(ref data); }
                }
            }
            return nullValue<valueType>.Array;
        }
        /// <summary>
        /// 数组范围排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="values">待排序数组</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">排序范围数据数量</param>
        /// <param name="getKey">排序键值获取器</param>
        /// <param name="skipCount">跳过数据数量</param>
        /// <param name="getCount">排序数据数量</param>
        /// <param name="fixedIndex">索引位置</param>
        /// <returns>排序后的数组</returns>
        private unsafe static valueType[] getRangeSort/*Compare[0]*//*Compare[0]*/<valueType>
            (valueType[] values, int startIndex, int count, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getKey, int skipCount, int getCount
            , /*Type[2]*/ulongSortIndex/*Type[2]*/* fixedIndex)
        {
            /*Type[2]*/
            ulongSortIndex/*Type[2]*/* writeIndex = fixedIndex;
            for (int index = startIndex, endIndex = startIndex + count; index != endIndex; (*writeIndex++).Set(getKey(values[index]), index++)) ;
            new /*Type[3]*/ulongRangeIndexSorter/*Type[3]*//*Compare[0]*//*Compare[0]*/
            {
                SkipCount = fixedIndex + skipCount,
                GetEndIndex = fixedIndex + skipCount + getCount - 1
            }.Sort(fixedIndex, fixedIndex + count - 1);
            valueType[] newValues = new valueType[getCount];
            writeIndex = fixedIndex + skipCount;
            for (int index = 0; index != newValues.Length; ++index) newValues[index] = values[(*writeIndex++).Index];
            return newValues;
        }
    }
}

namespace fastCSharp
{
    /// <summary>
    /// 数组扩展操作
    /// </summary>
    public static partial class arrayExtension
    {
        /// <summary>
        /// 范围排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <param name="skipCount">跳过记录数</param>
        /// <param name="getCount">获取记录数,小于0表示所有</param>
        /// <returns>排序范围数组</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray</*Type[0]*/ulong/*Type[0]*/> rangeSort/*Compare[0]*//*Compare[0]*/
            (this /*Type[0]*/ulong/*Type[0]*/[] array, int skipCount, int getCount)
        {
            return fastCSharp.algorithm.quickSort.RangeSort/*Compare[0]*//*Compare[0]*/(array, skipCount, getCount);
        }
        /// <summary>
        /// 范围排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <param name="skipCount">跳过记录数</param>
        /// <param name="getCount">获取记录数,小于0表示所有</param>
        /// <returns>排序范围数组</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray</*Type[0]*/ulong/*Type[0]*/> getRangeSort/*Compare[0]*//*Compare[0]*/
            (this /*Type[0]*/ulong/*Type[0]*/[] array, int skipCount, int getCount)
        {
            return fastCSharp.algorithm.quickSort.GetRangeSort/*Compare[0]*//*Compare[0]*/(array, skipCount, getCount);
        }
        /// <summary>
        /// 范围排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组</param>
        /// <param name="getKey">排序键</param>
        /// <param name="skipCount">跳过记录数</param>
        /// <param name="getCount">获取记录数,小于0表示所有</param>
        /// <returns>排序范围数组</returns>
        public static valueType[] getRangeSort/*Compare[0]*//*Compare[0]*/<valueType>
            (this valueType[] array, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getKey, int skipCount, int getCount)
        {
            return fastCSharp.algorithm.quickSort.GetRangeSort/*Compare[0]*//*Compare[0]*/(array, getKey, skipCount, getCount);
        }
        /// <summary>
        /// 范围排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">结束位置</param>
        /// <param name="skipCount">跳过记录数</param>
        /// <param name="getCount">获取记录数</param>
        /// <returns>排序范围数组</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray</*Type[0]*/ulong/*Type[0]*/> rangeSort/*Compare[0]*//*Compare[0]*/
            (this /*Type[0]*/ulong/*Type[0]*/[] array, int startIndex, int count, int skipCount, int getCount)
        {
            return fastCSharp.algorithm.quickSort.RangeSort/*Compare[0]*//*Compare[0]*/(array, startIndex, count, skipCount, getCount);
        }
        /// <summary>
        /// 范围排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">结束位置</param>
        /// <param name="skipCount">跳过记录数</param>
        /// <param name="getCount">获取记录数</param>
        /// <returns>排序范围数组</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray</*Type[0]*/ulong/*Type[0]*/> getRangeSort/*Compare[0]*//*Compare[0]*/
            (this /*Type[0]*/ulong/*Type[0]*/[] array, int startIndex, int count, int skipCount, int getCount)
        {
            return fastCSharp.algorithm.quickSort.GetRangeSort/*Compare[0]*//*Compare[0]*/(array, startIndex, count, skipCount, getCount);
        }
        /// <summary>
        /// 范围排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待排序数组</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">结束位置</param>
        /// <param name="getKey">排序键</param>
        /// <param name="skipCount">跳过记录数</param>
        /// <param name="getCount">获取记录数</param>
        /// <returns>排序范围数组</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType[] getRangeSort/*Compare[0]*//*Compare[0]*/<valueType>
            (this valueType[] array, int startIndex, int count, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getKey, int skipCount, int getCount)
        {
            return fastCSharp.algorithm.quickSort.GetRangeSort/*Compare[0]*//*Compare[0]*/(array, startIndex, count, getKey, skipCount, getCount);
        }
        /// <summary>
        /// 分页排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <param name="pageSize">分页尺寸</param>
        /// <param name="currentPage">页号</param>
        /// <returns>分页排序数据</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray</*Type[0]*/ulong/*Type[0]*/> pageSort/*Compare[0]*//*Compare[0]*/
            (this /*Type[0]*/ulong/*Type[0]*/[] array, int pageSize, int currentPage)
        {
            array.page page = new array.page(array.length(), pageSize, currentPage);
            return fastCSharp.algorithm.quickSort.RangeSort/*Compare[0]*//*Compare[0]*/(array, page.SkipCount, page.CurrentPageSize);
        }
        /// <summary>
        /// 分页排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <param name="getKey">获取排序关键字委托</param>
        /// <param name="pageSize">分页尺寸</param>
        /// <param name="currentPage">页号</param>
        /// <returns>分页排序数据</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType[] getPageSort/*Compare[0]*//*Compare[0]*/<valueType>
            (this valueType[] array, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getKey, int pageSize, int currentPage)
        {
            array.page page = new array.page(array.length(), pageSize, currentPage);
            return fastCSharp.algorithm.quickSort.GetRangeSort/*Compare[0]*//*Compare[0]*/(array, getKey, page.SkipCount, page.CurrentPageSize);
        }
        /// <summary>
        /// 分页排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <param name="getKey">获取排序关键字委托</param>
        /// <param name="pageSize">分页尺寸</param>
        /// <param name="currentPage">页号</param>
        /// <param name="count">数据总量</param>
        /// <returns>分页排序数据</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType[] getPageSort/*Compare[0]*//*Compare[0]*/<valueType>
            (this valueType[] array, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getKey, int pageSize, int currentPage, out int count)
        {
            array.page page = new array.page(count = array.length(), pageSize, currentPage);
            return fastCSharp.algorithm.quickSort.GetRangeSort/*Compare[0]*//*Compare[0]*/(array, getKey, page.SkipCount, page.CurrentPageSize);
        }
        /// <summary>
        /// 分页排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <param name="pageSize">分页尺寸</param>
        /// <param name="currentPage">页号</param>
        /// <returns>分页排序数据</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray</*Type[0]*/ulong/*Type[0]*/> getPageSort/*Compare[0]*//*Compare[0]*/
            (this /*Type[0]*/ulong/*Type[0]*/[] array, int pageSize, int currentPage)
        {
            array.page page = new array.page(array.length(), pageSize, currentPage);
            return fastCSharp.algorithm.quickSort.GetRangeSort/*Compare[0]*//*Compare[0]*/(array, page.SkipCount, page.CurrentPageSize);
        }
    }

    /// <summary>
    /// 数组子串扩展
    /// </summary>
    public static partial class subArrayExtension
    {
        /// <summary>
        /// 分页排序
        /// </summary>
        /// <param name="array">待排序数组</param>
        /// <param name="getKey">获取排序关键字委托</param>
        /// <param name="pageSize">分页尺寸</param>
        /// <param name="currentPage">页号</param>
        /// <param name="count">数据总量</param>
        /// <returns>分页排序数据</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType[] getPageSort/*Compare[0]*//*Compare[0]*/<valueType>
            (this subArray<valueType> array, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getKey, int pageSize, int currentPage, out int count)
        {
            array.page page = new array.page(count = array.Count, pageSize, currentPage);
            return fastCSharp.algorithm.quickSort.GetRangeSort/*Compare[0]*//*Compare[0]*/(array.UnsafeArray, 0, count, getKey, page.SkipCount, page.CurrentPageSize);
        }
        /// <summary>
        /// 范围排序
        /// </summary>
        /// <param name="array">数组子串</param>
        /// <param name="skipCount">跳过记录数</param>
        /// <param name="getCount">获取记录数,小于0表示所有</param>
        /// <returns>排序范围数组</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray</*Type[0]*/ulong/*Type[0]*/> rangeSort/*Compare[0]*//*Compare[0]*/
            (this subArray</*Type[0]*/ulong/*Type[0]*/> array, int skipCount, int getCount)
        {
            if (array.Count != 0)
            {
                return fastCSharp.algorithm.quickSort.RangeSort/*Compare[0]*//*Compare[0]*/(array.UnsafeArray, array.StartIndex, array.Count, skipCount, getCount);
            }
            return default(subArray</*Type[0]*/ulong/*Type[0]*/>);
        }
        /// <summary>
        /// 范围排序
        /// </summary>
        /// <param name="array">数组子串</param>
        /// <param name="skipCount">跳过记录数</param>
        /// <param name="getCount">获取记录数,小于0表示所有</param>
        /// <returns>排序范围数组</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray</*Type[0]*/ulong/*Type[0]*/> getRangeSort/*Compare[0]*//*Compare[0]*/
            (this subArray</*Type[0]*/ulong/*Type[0]*/> array, int skipCount, int getCount)
        {
            if (array.Count != 0)
            {
                return fastCSharp.algorithm.quickSort.GetRangeSort/*Compare[0]*//*Compare[0]*/(array.UnsafeArray, array.StartIndex, array.Count, skipCount, getCount);
            }
            return default(subArray</*Type[0]*/ulong/*Type[0]*/>);
        }
        /// <summary>
        /// 范围排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">数组子串</param>
        /// <param name="getKey">排序键</param>
        /// <param name="skipCount">跳过记录数</param>
        /// <param name="getCount">获取记录数,小于0表示所有</param>
        /// <returns>排序范围数组</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType[] getRangeSort/*Compare[0]*//*Compare[0]*/<valueType>
            (this subArray<valueType> array, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getKey, int skipCount, int getCount)
        {
            if (array.Count != 0)
            {
                return fastCSharp.algorithm.quickSort.GetRangeSort/*Compare[0]*//*Compare[0]*/
                    (array.UnsafeArray, array.StartIndex, array.Count, getKey, skipCount, getCount);
            }
            return nullValue<valueType>.Array;
        }
        /// <summary>
        /// 分页排序
        /// </summary>
        /// <param name="array">数组子串</param>
        /// <param name="pageSize">分页尺寸</param>
        /// <param name="currentPage">页号</param>
        /// <returns>分页排序数据</returns>
        public static subArray</*Type[0]*/ulong/*Type[0]*/> pageSort/*Compare[0]*//*Compare[0]*/
            (this subArray</*Type[0]*/ulong/*Type[0]*/> array, int pageSize, int currentPage)
        {
            array.page page = new array.page(array.Count, pageSize, currentPage);
            int count = page.CurrentPageSize;
            if (count != 0)
            {
                return fastCSharp.algorithm.quickSort.RangeSort/*Compare[0]*//*Compare[0]*/(array.UnsafeArray, array.StartIndex, array.Count, page.SkipCount, count);
            }
            return default(subArray</*Type[0]*/ulong/*Type[0]*/>);
        }
        /// <summary>
        /// 分页排序
        /// </summary>
        /// <param name="array">数组子串</param>
        /// <param name="pageSize">分页尺寸</param>
        /// <param name="currentPage">页号</param>
        /// <returns>分页排序数据</returns>
        public static subArray</*Type[0]*/ulong/*Type[0]*/> getPageSort/*Compare[0]*//*Compare[0]*/
            (this subArray</*Type[0]*/ulong/*Type[0]*/> array, int pageSize, int currentPage)
        {
            array.page page = new array.page(array.Count, pageSize, currentPage);
            int getCount = page.CurrentPageSize;
            if (getCount != 0)
            {
                return fastCSharp.algorithm.quickSort.GetRangeSort/*Compare[0]*//*Compare[0]*/(array.UnsafeArray, array.StartIndex, array.Count, page.SkipCount, getCount);
            }
            return default(subArray</*Type[0]*/ulong/*Type[0]*/>);
        }
    }
}