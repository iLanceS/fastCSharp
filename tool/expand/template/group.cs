using System;
/*Type:ulong;long;uint;int;DateTime*/

namespace fastCSharp
{
    /// <summary>
    /// 数组扩展操作
    /// </summary>
    public static partial class arrayExtension
    {
        /// <summary>
        /// 数据去重
        /// </summary>
        /// <param name="array">数据数组</param>
        /// <returns>目标数据集合</returns>
        public unsafe static subArray</*Type[0]*/ulong/*Type[0]*/> distinct(this /*Type[0]*/ulong/*Type[0]*/[] array)
        {
            if (array == null) return default(subArray</*Type[0]*/ulong/*Type[0]*/>);
            if (array.Length <= 1) return subArray</*Type[0]*/ulong/*Type[0]*/>.Unsafe(array, 0, array.Length);
            arrayExtension.sort(array, 0, array.Length);
            fixed (/*Type[0]*/ulong/*Type[0]*/* valueFixed = array)
            {
                /*Type[0]*/
                ulong/*Type[0]*/* start = valueFixed + 1, end = valueFixed + array.Length, write = valueFixed;
                do
                {
                    if (*start != *write) *++write = *start;
                }
                while (++start != end);
                return subArray</*Type[0]*/ulong/*Type[0]*/>.Unsafe(array, 0, (int)(write - valueFixed) + 1);
            }
        }
        /// <summary>
        /// 数据去重
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">数据数组</param>
        /// <param name="getValue">数据获取器</param>
        /// <returns>目标数据集合</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType[] distinct<valueType>
            (this /*Type[0]*/ulong/*Type[0]*/[] array, Func</*Type[0]*/ulong/*Type[0]*/, valueType> getValue)
        {
            return new subArray</*Type[0]*/ulong/*Type[0]*/>(array).distinct(getValue);
        }
        /// <summary>
        /// 数据去重
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">数据数组</param>
        /// <param name="getValue">数据获取器</param>
        /// <returns>目标数据集合</returns>
        public static subArray</*Type[0]*/ulong/*Type[0]*/> distinct<valueType>
            (this valueType[] array, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getValue)
        {
            if (getValue == null) log.Error.Throw(log.exceptionType.Null);
            /*Type[0]*/
            ulong/*Type[0]*/[] newValues = array.getArray(getValue);
            arrayExtension.sort(newValues, 0, newValues.Length);
            return newValues.distinct();
        }
        /// <summary>
        /// 求交集
        /// </summary>
        /// <param name="left">左侧数据</param>
        /// <param name="right">右侧数据</param>
        /// <returns>数据交集</returns>
        public unsafe static subArray</*Type[0]*/ulong/*Type[0]*/> intersect(this /*Type[0]*/ulong/*Type[0]*/[] left, /*Type[0]*/ulong/*Type[0]*/[] right)
        {
            int leftLength = left.length(), rightLength = right.length();
            if (leftLength != 0 && rightLength != 0)
            {
                /*Type[0]*/
                ulong/*Type[0]*/[] min = leftLength <= rightLength ? left : right, values = new /*Type[0]*/ulong/*Type[0]*/[min.Length];
                fixed (/*Type[0]*/ulong/*Type[0]*/* valueFixed = values)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/* write = valueFixed;
                    staticHashSet</*Type[0]*/ulong/*Type[0]*/> hash = new staticHashSet</*Type[0]*/ulong/*Type[0]*/>(min);
                    foreach (/*Type[0]*/ulong/*Type[0]*/ value in leftLength <= rightLength ? right : left)
                    {
                        if (hash.Contains(value)) *write++ = value;
                    }
                    return subArray</*Type[0]*/ulong/*Type[0]*/>.Unsafe(values, 0, (int)(write - valueFixed));
                }
            }
            return default(subArray</*Type[0]*/ulong/*Type[0]*/>);
        }
        /// <summary>
        /// 数据排序分组数量
        /// </summary>
        /// <param name="array">数据数组</param>
        /// <returns>分组数量</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static keyValue</*Type[0]*/ulong/*Type[0]*/, int>[] sortGroupCount(this /*Type[0]*/ulong/*Type[0]*/[] array)
        {
            return new subArray</*Type[0]*/ulong/*Type[0]*/>(array).sortGroupCount();
        }
        /// <summary>
        /// 数据排序分组
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">数据数组</param>
        /// <param name="getValue">数据获取器</param>
        /// <returns>目标数据集合</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray<subArray<valueType>> sortGroup<valueType>
            (this valueType[] array, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getValue)
        {
            return new subArray<valueType>(array).sortGroup(getValue);
        }
        /// <summary>
        /// 数据排序分组数量
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">数据数组</param>
        /// <param name="getValue">数据获取器</param>
        /// <returns>数据排序分组数量</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static int sortGroupCount<valueType>
            (this valueType[] array, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getValue)
        {
            return new subArray<valueType>(array).sortGroupCount(getValue);
        }
    }
    
    /// <summary>
    /// 数组子串扩展操作
    /// </summary>
    public static partial class subArrayExtension
    {
        /// <summary>
        /// 数据去重
        /// </summary>
        /// <param name="array">数据数组</param>
        /// <returns>目标数据集合</returns>
        public unsafe static subArray</*Type[0]*/ulong/*Type[0]*/> distinct(this subArray</*Type[0]*/ulong/*Type[0]*/> array)
        {
            if (array.Count > 1)
            {
                arrayExtension.sort(array.UnsafeArray, array.StartIndex, array.Count);
                fixed (/*Type[0]*/ulong/*Type[0]*/* valueFixed = array.UnsafeArray)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/* write = valueFixed + array.StartIndex, start = write + 1, end = write + array.Count;
                    do
                    {
                        if (*start != *write) *++write = *start;
                    }
                    while (++start != end);
                    return subArray</*Type[0]*/ulong/*Type[0]*/>.Unsafe(array.UnsafeArray, array.StartIndex, (int)(write - valueFixed) + 1 - array.StartIndex);
                }
            }
            return array;
        }
        /// <summary>
        /// 数据去重
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">数据数组</param>
        /// <param name="getValue">数据获取器</param>
        /// <returns>目标数据集合</returns>
        public unsafe static valueType[] distinct<valueType>
            (this subArray</*Type[0]*/ulong/*Type[0]*/> array, Func</*Type[0]*/ulong/*Type[0]*/, valueType> getValue)
        {
            if (array.Count != 0)
            {
                if (getValue == null) log.Error.Throw(log.exceptionType.Null);
                arrayExtension.sort(array.UnsafeArray, array.StartIndex, array.Count);
                fixed (/*Type[0]*/ulong/*Type[0]*/* valueFixed = array.UnsafeArray)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/* start = valueFixed + array.StartIndex, end = start + array.Count;
                    /*Type[0]*/
                    ulong/*Type[0]*/ value = *start;
                    int count = 1;
                    while (++start != end)
                    {
                        if (*start != value)
                        {
                            ++count;
                            value = *start;
                        }
                    }
                    valueType[] values = new valueType[count];
                    values[0] = getValue(value = *(start = valueFixed + array.StartIndex));
                    count = 1;
                    while (++start != end)
                    {
                        if (*start != value) values[count++] = getValue(value = *start);
                    }
                    return values;
                }
            }
            return nullValue<valueType>.Array;
        }
        /// <summary>
        /// 数据去重
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">数据数组</param>
        /// <param name="getValue">数据获取器</param>
        /// <returns>目标数据集合</returns>
        public unsafe static subArray</*Type[0]*/ulong/*Type[0]*/> distinct<valueType>
            (this subArray<valueType> array, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getValue)
        {
            if (array.Count != 0)
            {
                if (getValue == null) log.Error.Throw(log.exceptionType.Null);
                /*Type[0]*/
                ulong/*Type[0]*/[] newValues = array.getArray(getValue);
                arrayExtension.sort(newValues, 0, newValues.Length);
                return newValues.distinct();
            }
            return default(subArray</*Type[0]*/ulong/*Type[0]*/>);
        }
        /// <summary>
        /// 数据排序分组数量
        /// </summary>
        /// <param name="array">数据数组</param>
        /// <returns>分组数量</returns>
        public unsafe static keyValue</*Type[0]*/ulong/*Type[0]*/, int>[] sortGroupCount(this subArray</*Type[0]*/ulong/*Type[0]*/> array)
        {
            if (array.Count != 0)
            {
                arrayExtension.sort(array.UnsafeArray, array.StartIndex, array.Count);
                fixed (/*Type[0]*/ulong/*Type[0]*/* valueFixed = array.UnsafeArray)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/* start = valueFixed + array.StartIndex, lastStart = start, end = start + array.Count;
                    /*Type[0]*/
                    ulong/*Type[0]*/ value = *start;
                    int count = 1;
                    while (++start != end)
                    {
                        if (*start != value)
                        {
                            ++count;
                            value = *start;
                        }
                    }
                    keyValue</*Type[0]*/ulong/*Type[0]*/, int>[] values = new keyValue</*Type[0]*/ulong/*Type[0]*/, int>[count];
                    value = *(start = lastStart);
                    count = 0;
                    while (++start != end)
                    {
                        if (*start != value)
                        {
                            values[count++].Set(value, (int)(start - lastStart));
                            value = *start;
                            lastStart = start;
                        }
                    }
                    values[count].Set(value, (int)(start - lastStart));
                    return values;
                }
            }
            return nullValue<keyValue</*Type[0]*/ulong/*Type[0]*/, int>>.Array;
        }
        /// <summary>
        /// 数据排序分组
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">数据数组</param>
        /// <param name="getValue">数据获取器</param>
        /// <returns>目标数据集合</returns>
        public unsafe static subArray<subArray<valueType>> sortGroup<valueType>
            (this subArray<valueType> array, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getValue)
        {
            if (array.Count != 0)
            {
                if (getValue == null) log.Error.Throw(log.exceptionType.Null);
                valueType[] sortArray = arrayExtension.getSort(array.UnsafeArray, getValue, array.StartIndex, array.Count);
                subArray<valueType>[] values = new subArray<valueType>[sortArray.Length];
                /*Type[0]*/
                ulong/*Type[0]*/ key = getValue(sortArray[0]);
                int startIndex = 0, valueIndex = 0;
                for (int index = 1; index != sortArray.Length; ++index)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/ nextKey = getValue(sortArray[index]);
                    if (key != nextKey)
                    {
                        values[valueIndex++].UnsafeSet(sortArray, startIndex, index - startIndex);
                        key = nextKey;
                        startIndex = index;
                    }
                }
                values[valueIndex++].UnsafeSet(sortArray, startIndex, sortArray.Length - startIndex);
                return subArray<subArray<valueType>>.Unsafe(values, 0, valueIndex);
            }
            return default(subArray<subArray<valueType>>);
        }
        /// <summary>
        /// 数据排序分组数量
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">数据数组</param>
        /// <param name="getValue">数据获取器</param>
        /// <returns>目标数据集合数量</returns>
        public unsafe static int sortGroupCount<valueType>
            (this subArray<valueType> array, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getValue)
        {
            if (array.Count != 0)
            {
                if (getValue == null) log.Error.Throw(log.exceptionType.Null);
                valueType[] sortArray = arrayExtension.getSort(array.UnsafeArray, getValue, array.StartIndex, array.Count);
                /*Type[0]*/
                ulong/*Type[0]*/ key = getValue(sortArray[0]);
                int count = 0;
                for (int index = 1; index != sortArray.Length; ++index)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/ nextKey = getValue(sortArray[index]);
                    if (key != nextKey)
                    {
                        ++count;
                        key = nextKey;
                    }
                }
                return count + 1;
            }
            return 0;
        }
    }
}
