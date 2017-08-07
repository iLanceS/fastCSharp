using System;
/*Type:ulong;long;uint;int;ushort;short;byte;sbyte;double;float;DateTime*/

namespace fastCSharp
{
    /// <summary>
    /// 数组扩展操作
    /// </summary>
    public static partial class arrayExtension
    {
        /// <summary>
        /// 逆转数组
        /// </summary>
        /// <param name="array">数组数据</param>
        /// <param name="index">起始位置</param>
        /// <param name="length">翻转数据数量</param>
        internal unsafe static void Reverse(/*Type[0]*/ulong/*Type[0]*/[] array, int index, int length)
        {
            fixed (/*Type[0]*/ulong/*Type[0]*/* valueFixed = array)
            {
                for (/*Type[0]*/ulong/*Type[0]*/* start = valueFixed + index, end = start + length; start < --end; ++start)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/ value = *start;
                    *start = *end;
                    *end = value;
                }
            }
        }
        /// <summary>
        /// 逆转数组
        /// </summary>
        /// <param name="array">数组数据</param>
        /// <returns>翻转后的新数组</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static /*Type[0]*/ulong/*Type[0]*/[] reverse(this /*Type[0]*/ulong/*Type[0]*/[] array)
        {
            if (array == null || array.Length == 0) return nullValue</*Type[0]*/ulong/*Type[0]*/>.Array;
            Reverse(array, 0, array.Length);
            return array;
        }
        /// <summary>
        /// 逆转数组
        /// </summary>
        /// <param name="array">数组数据</param>
        /// <param name="index">起始位置</param>
        /// <param name="length">翻转数据数量</param>
        /// <returns>翻转后的新数组</returns>
        internal unsafe static /*Type[0]*/ulong/*Type[0]*/[] GetReverse(/*Type[0]*/ulong/*Type[0]*/[] array, int index, int length)
        {
            /*Type[0]*/
            ulong/*Type[0]*/[] newValues = new /*Type[0]*/ulong/*Type[0]*/[length];
            fixed (/*Type[0]*/ulong/*Type[0]*/* valueFixed = array, newValueFixed = newValues)
            {
                for (/*Type[0]*/ulong/*Type[0]*/* start = valueFixed + index, end = start + length, wirte = newValueFixed + length;
                    start != end;
                    *--wirte = *start++) ;
            }
            return newValues;
        }
        /// <summary>
        /// 逆转数组
        /// </summary>
        /// <param name="array">数组数据</param>
        /// <returns>翻转后的新数组</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static /*Type[0]*/ulong/*Type[0]*/[] getReverse(this /*Type[0]*/ulong/*Type[0]*/[] array)
        {
            if (array == null || array.Length == 0) return nullValue</*Type[0]*/ulong/*Type[0]*/>.Array;
            return GetReverse(array, 0, array.Length);
        }
        /// <summary>
        /// 获取匹配数据位置
        /// </summary>
        /// <param name="valueFixed">数据指针</param>
        /// <param name="length">匹配数据数量</param>
        /// <param name="value">匹配数据</param>
        /// <returns>匹配位置,失败为null</returns>
        internal unsafe static /*Type[0]*/ulong/*Type[0]*/* IndexOf
            (/*Type[0]*/ulong/*Type[0]*/* valueFixed, int length, /*Type[0]*/ulong/*Type[0]*/ value)
        {
            for (/*Type[0]*/ulong/*Type[0]*/* start = valueFixed, end = valueFixed + length; start != end; ++start)
            {
                if (*start == value) return start;
            }
            return null;
        }
        /// <summary>
        /// 获取匹配数据位置
        /// </summary>
        /// <param name="array">数组数据</param>
        /// <param name="value">匹配数据</param>
        /// <returns>匹配位置,失败为-1</returns>
        public unsafe static int indexOf(this /*Type[0]*/ulong/*Type[0]*/[] array, /*Type[0]*/ulong/*Type[0]*/ value)
        {
            if (array != null)
            {
                fixed (/*Type[0]*/ulong/*Type[0]*/* valueFixed = array)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/* valueIndex = IndexOf(valueFixed, array.Length, value);
                    if (valueIndex != null) return (int)(valueIndex - valueFixed);
                }
            }
            return -1;
        }
        /// <summary>
        /// 获取匹配数据位置
        /// </summary>
        /// <param name="valueFixed">数组数据</param>
        /// <param name="length">匹配数据数量</param>
        /// <param name="isValue">匹配数据委托</param>
        /// <returns>匹配位置,失败为null</returns>
        internal unsafe static /*Type[0]*/ulong/*Type[0]*/* IndexOf
            (/*Type[0]*/ulong/*Type[0]*/* valueFixed, int length, Func</*Type[0]*/ulong/*Type[0]*/, bool> isValue)
        {
            for (/*Type[0]*/ulong/*Type[0]*/* start = valueFixed, end = valueFixed + length; start != end; ++start)
            {
                if (isValue(*start)) return start;
            }
            return null;
        }
        /// <summary>
        /// 获取匹配数据位置
        /// </summary>
        /// <param name="array">数据数组</param>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>匹配位置,失败为-1</returns>
        public unsafe static int indexOf(this /*Type[0]*/ulong/*Type[0]*/[] array, Func</*Type[0]*/ulong/*Type[0]*/, bool> isValue)
        {
            if (array != null)
            {
                if (isValue == null) log.Error.Throw(log.exceptionType.Null);
                fixed (/*Type[0]*/ulong/*Type[0]*/* valueFixed = array)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/* valueIndex = IndexOf(valueFixed, array.Length, isValue);
                    if (valueIndex != null) return (int)(valueIndex - valueFixed);
                }
            }
            return -1;
        }
        /// <summary>
        /// 获取第一个匹配值
        /// </summary>
        /// <param name="array">数组数据</param>
        /// <param name="isValue">数据匹配器</param>
        /// <param name="index">起始位置</param>
        /// <returns>第一个匹配值,失败为default(/*Type[0]*/ulong/*Type[0]*/)</returns>
        public unsafe static /*Type[0]*/ulong/*Type[0]*/ firstOrDefault
            (this /*Type[0]*/ulong/*Type[0]*/[] array, Func</*Type[0]*/ulong/*Type[0]*/, bool> isValue, int index)
        {
            if (array != null && (uint)index < (uint)array.Length)
            {
                if (isValue == null) log.Error.Throw(log.exceptionType.Null);
                fixed (/*Type[0]*/ulong/*Type[0]*/* valueFixed = array)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/* valueIndex = IndexOf(valueFixed + index, array.Length - index, isValue);
                    if (valueIndex != null) return *valueIndex;
                }
            }
            return default(/*Type[0]*/ulong/*Type[0]*/);
        }
        /// <summary>
        /// 获取匹配数量
        /// </summary>
        /// <param name="array">数组数据</param>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>匹配数量</returns>
        public unsafe static int count
            (this /*Type[0]*/ulong/*Type[0]*/[] array, Func</*Type[0]*/ulong/*Type[0]*/, bool> isValue)
        {
            if (array != null)
            {
                if (isValue == null) log.Error.Throw(log.exceptionType.Null);
                int value = 0;
                fixed (/*Type[0]*/ulong/*Type[0]*/* valueFixed = array)
                {
                    for (/*Type[0]*/ulong/*Type[0]*/* end = valueFixed + array.Length; end != valueFixed; )
                    {
                        if (isValue(*--end)) ++value;
                    }
                }
                return value;
            }
            return 0;
        }
        /// <summary>
        /// 替换数据
        /// </summary>
        /// <param name="array">数组数据</param>
        /// <param name="value">新值</param>
        /// <param name="isValue">数据匹配器</param>
        public unsafe static /*Type[0]*/ulong/*Type[0]*/[] replaceFirst
            (this /*Type[0]*/ulong/*Type[0]*/[] array, /*Type[0]*/ulong/*Type[0]*/ value, Func</*Type[0]*/ulong/*Type[0]*/, bool> isValue)
        {
            if (array != null)
            {
                if (isValue == null) log.Error.Throw(log.exceptionType.Null);
                fixed (/*Type[0]*/ulong/*Type[0]*/* valueFixed = array)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/* valueIndex = IndexOf(valueFixed, array.Length, isValue);
                    if (valueIndex != null) *valueIndex = value;
                }
                return array;
            }
            return nullValue</*Type[0]*/ulong/*Type[0]*/>.Array;
        }
        /// <summary>
        /// 数据转换
        /// </summary>
        /// <typeparam name="valueType">数组类型</typeparam>
        /// <param name="array">数据数组</param>
        /// <param name="getValue">数据获取器</param>
        /// <returns>目标数组</returns>
        public unsafe static /*Type[0]*/ulong/*Type[0]*/[] getArray<valueType>
            (this valueType[] array, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getValue)
        {
            if (array.length() != 0)
            {
                if (getValue == null) log.Error.Throw(log.exceptionType.Null);
                /*Type[0]*/
                ulong/*Type[0]*/[] newValues = new /*Type[0]*/ulong/*Type[0]*/[array.Length];
                fixed (/*Type[0]*/ulong/*Type[0]*/* newValueFixed = newValues)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/* writeValue = newValueFixed;
                    foreach (valueType value in array) *writeValue++ = getValue(value);
                }
                return newValues;
            }
            return nullValue</*Type[0]*/ulong/*Type[0]*/>.Array;
        }
        /// <summary>
        /// 获取匹配集合
        /// </summary>
        /// <param name="array">数组数据</param>
        /// <param name="index">起始位置</param>
        /// <param name="length">翻转数据数量</param>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>匹配集合</returns>
        internal unsafe static subArray</*Type[0]*/ulong/*Type[0]*/> GetFind
            (this /*Type[0]*/ulong/*Type[0]*/[] array, int index, int length, Func</*Type[0]*/ulong/*Type[0]*/, bool> isValue)
        {
            /*Type[0]*/
            ulong/*Type[0]*/[] newValues = new /*Type[0]*/ulong/*Type[0]*/[length < sizeof(int) ? sizeof(int) : length];
            fixed (/*Type[0]*/ulong/*Type[0]*/* newValueFixed = newValues, valueFixed = array)
            {
                /*Type[0]*/
                ulong/*Type[0]*/* write = newValueFixed;
                for (/*Type[0]*/ulong/*Type[0]*/* start = valueFixed + index, end = valueFixed + length; start != end; ++start)
                {
                    if (isValue(*start)) *write++ = *start;
                }
                return subArray</*Type[0]*/ulong/*Type[0]*/>.Unsafe(newValues, 0, (int)(write - newValueFixed));
            }
        }
        /// <summary>
        /// 获取匹配集合
        /// </summary>
        /// <param name="array">数组数据</param>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>匹配集合</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static subArray</*Type[0]*/ulong/*Type[0]*/> getFind
            (this /*Type[0]*/ulong/*Type[0]*/[] array, Func</*Type[0]*/ulong/*Type[0]*/, bool> isValue)
        {
            if (array.length() != 0)
            {
                if (isValue == null) log.Error.Throw(log.exceptionType.Null);
                return GetFind(array, 0, array.Length, isValue);
            }
            return default(subArray</*Type[0]*/ulong/*Type[0]*/>);
        }
        /// <summary>
        /// 获取匹配数组
        /// </summary>
        /// <param name="array">数组数据</param>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>匹配数组</returns>
        public unsafe static /*Type[0]*/ulong/*Type[0]*/[] getFindArray
            (this /*Type[0]*/ulong/*Type[0]*/[] array, Func</*Type[0]*/ulong/*Type[0]*/, bool> isValue)
        {
            if (isValue == null) log.Error.Throw(log.exceptionType.Null);
            int length = array.length();
            if (length != 0)
            {
                memoryPool pool = fastCSharp.memoryPool.GetDefaultPool(length = ((length + 31) >> 5) << 2);
                byte[] data = pool.Get(length);
                try
                {
                    fixed (byte* dataFixed = data)
                    {
                        Array.Clear(data, 0, length);
                        return GetFindArray(array, 0, array.Length, isValue, new fixedMap(dataFixed));
                    }
                }
                finally { pool.PushNotNull(data); }
            }
            return nullValue</*Type[0]*/ulong/*Type[0]*/>.Array;
        }
        /// <summary>
        /// 获取匹配数组
        /// </summary>
        /// <param name="array">数组数据</param>
        /// <param name="index">起始位置</param>
        /// <param name="count">匹配数据数量</param>
        /// <param name="isValue">数据匹配器</param>
        /// <param name="map">匹配结果位图</param>
        /// <returns>匹配数组</returns>
        internal unsafe static /*Type[0]*/ulong/*Type[0]*/[] GetFindArray
            (/*Type[0]*/ulong/*Type[0]*/[] array, int index, int count, Func</*Type[0]*/ulong/*Type[0]*/, bool> isValue, fixedMap map)
        {
            int length = 0, mapIndex = 0;
            fixed (/*Type[0]*/ulong/*Type[0]*/* valueFixed = array)
            {
                /*Type[0]*/
                ulong/*Type[0]*/* startFixed = valueFixed + index, end = startFixed + count;
                for (/*Type[0]*/ulong/*Type[0]*/* start = startFixed; start != end; ++start, ++mapIndex)
                {
                    if (isValue(*start))
                    {
                        ++length;
                        map.Set(mapIndex);
                    }
                }
                if (length != 0)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/[] newValues = new /*Type[0]*/ulong/*Type[0]*/[length];
                    fixed (/*Type[0]*/ulong/*Type[0]*/* newValueFixed = newValues)
                    {
                        /*Type[0]*/
                        ulong/*Type[0]*/* write = newValueFixed + length;
                        while (mapIndex != 0)
                        {
                            if (map.Get(--mapIndex)) *--write = startFixed[mapIndex];
                        }
                    }
                    return newValues;
                }
            }
            return nullValue</*Type[0]*/ulong/*Type[0]*/>.Array;
        }
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <param name="array">数据数组</param>
        /// <param name="value">最大值</param>
        /// <returns>是否存在最大值</returns>
        public unsafe static bool max(this /*Type[0]*/ulong/*Type[0]*/[] array, out /*Type[0]*/ulong/*Type[0]*/ value)
        {
            if (array.length() != 0)
            {
                fixed (/*Type[0]*/ulong/*Type[0]*/* valueFixed = array)
                {
                    value = *valueFixed;
                    for (/*Type[0]*/ulong/*Type[0]*/* start = valueFixed + 1, end = valueFixed + array.Length; start != end; ++start)
                    {
                        if (*start > value) value = *start;
                    }
                    return true;
                }
            }
            value = /*Type[0]*/ulong/*Type[0]*/.MinValue;
            return false;
        }
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <param name="array">数据数组</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最大值,失败返回默认空值</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static /*Type[0]*/ulong/*Type[0]*/ max(this /*Type[0]*/ulong/*Type[0]*/[] array, /*Type[0]*/ulong/*Type[0]*/ nullValue)
        {
            /*Type[0]*/
            ulong/*Type[0]*/ value;
            return max(array, out value) ? value : nullValue;
        }
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">数据数组</param>
        /// <param name="getKey">数据获取器</param>
        /// <param name="value">最大值</param>
        /// <returns>是否存在最大值</returns>
        public static bool maxKey<valueType>
            (this valueType[] array, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getKey, out /*Type[0]*/ulong/*Type[0]*/ value)
        {
            if (array.length() != 0)
            {
                value = getKey(array[0]);
                foreach (valueType nextValue in array)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/ nextKey = getKey(nextValue);
                    if (nextKey > value) value = nextKey;
                }
                return true;
            }
            value = /*Type[0]*/ulong/*Type[0]*/.MinValue;
            return false;
        }
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">数据数组</param>
        /// <param name="getKey">数据获取器</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最大值,失败返回默认空值</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static /*Type[0]*/ulong/*Type[0]*/ maxKey<valueType>
            (this valueType[] array, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getKey, /*Type[0]*/ulong/*Type[0]*/ nullValue)
        {
            /*Type[0]*/
            ulong/*Type[0]*/ value;
            return maxKey(array, getKey, out value) ? value : nullValue;
        }
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">数据数组</param>
        /// <param name="getKey">数据获取器</param>
        /// <param name="value">最大值</param>
        /// <returns>是否存在最大值</returns>
        public static bool max<valueType>
            (this valueType[] array, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getKey, out valueType value)
        {
            if (array.length() != 0)
            {
                /*Type[0]*/
                ulong/*Type[0]*/ maxKey = getKey(value = array[0]);
                foreach (valueType nextValue in array)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/ nextKey = getKey(nextValue);
                    if (nextKey > maxKey)
                    {
                        maxKey = nextKey;
                        value = nextValue;
                    }
                }
                return true;
            }
            value = default(valueType);
            return false;
        }
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">数据数组</param>
        /// <param name="getKey">数据获取器</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最大值,失败返回默认空值</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType max<valueType>(this valueType[] array, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getKey, valueType nullValue)
        {
            valueType value;
            return max(array, getKey, out value) ? value : nullValue;
        }
        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <param name="array">数据数组</param>
        /// <param name="value">最小值</param>
        /// <returns>是否存在最小值</returns>
        public unsafe static bool min(this /*Type[0]*/ulong/*Type[0]*/[] array, out /*Type[0]*/ulong/*Type[0]*/ value)
        {
            if (array.length() != 0)
            {
                fixed (/*Type[0]*/ulong/*Type[0]*/* valueFixed = array)
                {
                    value = *valueFixed;
                    for (/*Type[0]*/ulong/*Type[0]*/* start = valueFixed + 1, end = valueFixed + array.Length; start != end; ++start)
                    {
                        if (*start < value) value = *start;
                    }
                    return true;
                }
            }
            value = /*Type[0]*/ulong/*Type[0]*/.MaxValue;
            return false;
        }
        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <param name="array">数据数组</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最小值,失败返回默认空值</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static /*Type[0]*/ulong/*Type[0]*/ min(this /*Type[0]*/ulong/*Type[0]*/[] array, /*Type[0]*/ulong/*Type[0]*/ nullValue)
        {
            /*Type[0]*/
            ulong/*Type[0]*/ value;
            return min(array, out value) ? value : nullValue;
        }
        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">数据数组</param>
        /// <param name="getKey">数据获取器</param>
        /// <param name="value">最小值</param>
        /// <returns>是否存在最小值</returns>
        public static bool minKey<valueType>
            (this valueType[] array, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getKey, out /*Type[0]*/ulong/*Type[0]*/ value)
        {
            if (array.length() != 0)
            {
                value = getKey(array[0]);
                foreach (valueType nextValue in array)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/ nextKey = getKey(nextValue);
                    if (nextKey < value) value = nextKey;
                }
                return true;
            }
            value = /*Type[0]*/ulong/*Type[0]*/.MaxValue;
            return false;
        }
        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">数据数组</param>
        /// <param name="getKey">数据获取器</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最小值,失败返回默认空值</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static /*Type[0]*/ulong/*Type[0]*/ minKey<valueType>
            (this valueType[] array, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getKey, /*Type[0]*/ulong/*Type[0]*/ nullValue)
        {
            /*Type[0]*/
            ulong/*Type[0]*/ value;
            return minKey(array, getKey, out value) ? value : nullValue;
        }
        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">数据数组</param>
        /// <param name="getKey">数据获取器</param>
        /// <param name="value">最小值</param>
        /// <returns>是否存在最小值</returns>
        public static bool min<valueType>(this valueType[] array, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getKey, out valueType value)
        {
            if (array.length() != 0)
            {
                value = array[0];
                /*Type[0]*/
                ulong/*Type[0]*/ minKey = getKey(value);
                foreach (valueType nextValue in array)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/ nextKey = getKey(nextValue);
                    if (nextKey < minKey)
                    {
                        minKey = nextKey;
                        value = nextValue;
                    }
                }
                return true;
            }
            value = default(valueType);
            return false;
        }
        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">数据数组</param>
        /// <param name="getKey">数据获取器</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最小值,失败返回默认空值</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType min<valueType>(this valueType[] array, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getKey, valueType nullValue)
        {
            valueType value;
            return min(array, getKey, out value) ? value : nullValue;
        }
    }

    /// <summary>
    /// 数组子串扩展
    /// </summary>
    public static partial class subArrayExtension
    {
        /// <summary>
        /// 逆转数组
        /// </summary>
        /// <param name="array">数组数据</param>
        /// <returns>翻转后的新数组</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static /*Type[0]*/ulong/*Type[0]*/[] getReverse(this subArray</*Type[0]*/ulong/*Type[0]*/> array)
        {
            if (array.Count == 0) return nullValue</*Type[0]*/ulong/*Type[0]*/>.Array;
            return arrayExtension.GetReverse(array.UnsafeArray, array.StartIndex, array.Count);
        }
        /// <summary>
        /// 逆转数组
        /// </summary>
        /// <param name="array">数组数据</param>
        /// <returns>翻转后的新数组</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray</*Type[0]*/ulong/*Type[0]*/> reverse(this subArray</*Type[0]*/ulong/*Type[0]*/> array)
        {
            if (array.Count > 1) arrayExtension.Reverse(array.UnsafeArray, array.StartIndex, array.Count);
            return array;
        }
        /// <summary>
        /// 获取匹配数据位置
        /// </summary>
        /// <param name="array">数组数据</param>
        /// <param name="value">匹配数据</param>
        /// <returns>匹配位置,失败为-1</returns>
        public unsafe static int indexOf(this subArray</*Type[0]*/ulong/*Type[0]*/> array, /*Type[0]*/ulong/*Type[0]*/ value)
        {
            if (array.Count != 0)
            {
                fixed (/*Type[0]*/ulong/*Type[0]*/* valueFixed = array.UnsafeArray)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/* start = valueFixed + array.StartIndex, index = arrayExtension.IndexOf(start, array.Count, value);
                    if (index != null) return (int)(index - start);
                }
            }
            return -1;
        }
        /// <summary>
        /// 获取匹配数据位置
        /// </summary>
        /// <param name="array">数据数组</param>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>匹配位置,失败为-1</returns>
        public unsafe static int indexOf(this subArray</*Type[0]*/ulong/*Type[0]*/> array, Func</*Type[0]*/ulong/*Type[0]*/, bool> isValue)
        {
            if (array.Count != 0)
            {
                if (isValue == null) log.Error.Throw(log.exceptionType.Null);
                fixed (/*Type[0]*/ulong/*Type[0]*/* valueFixed = array.UnsafeArray)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/* start = valueFixed + array.StartIndex, index = arrayExtension.IndexOf(start, array.Count, isValue);
                    if (index != null) return (int)(index - valueFixed);
                }
            }
            return -1;
        }
        /// <summary>
        /// 获取第一个匹配值
        /// </summary>
        /// <param name="array">数组数据</param>
        /// <param name="isValue">数据匹配器</param>
        /// <param name="index">起始位置</param>
        /// <returns>第一个匹配值,失败为default(/*Type[0]*/ulong/*Type[0]*/)</returns>
        public unsafe static /*Type[0]*/ulong/*Type[0]*/ firstOrDefault
            (this subArray</*Type[0]*/ulong/*Type[0]*/> array, Func</*Type[0]*/ulong/*Type[0]*/, bool> isValue, int index)
        {
            if ((uint)index < (uint)array.Count)
            {
                if (isValue == null) log.Error.Throw(log.exceptionType.Null);
                fixed (/*Type[0]*/ulong/*Type[0]*/* valueFixed = array.UnsafeArray)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/* valueIndex = arrayExtension.IndexOf(valueFixed + array.StartIndex + index, array.Count - index, isValue);
                    if (valueIndex != null) return *valueIndex;
                }
            }
            return default(/*Type[0]*/ulong/*Type[0]*/);
        }
        /// <summary>
        /// 获取匹配数量
        /// </summary>
        /// <param name="array">数组数据</param>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>匹配数量</returns>
        public unsafe static int count(this subArray</*Type[0]*/ulong/*Type[0]*/> array, Func</*Type[0]*/ulong/*Type[0]*/, bool> isValue)
        {
            if (array.Count != 0)
            {
                if (isValue == null) log.Error.Throw(log.exceptionType.Null);
                int value = 0;
                fixed (/*Type[0]*/ulong/*Type[0]*/* valueFixed = array.UnsafeArray)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/* start = valueFixed + array.StartIndex, end = start + array.Count;
                    do
                    {
                        if (isValue(*start)) ++value;
                    }
                    while (++start != end);
                }
                return value;
            }
            return 0;
        }
        /// <summary>
        /// 替换数据
        /// </summary>
        /// <param name="array">数组数据</param>
        /// <param name="value">新值</param>
        /// <param name="isValue">数据匹配器</param>
        public unsafe static subArray</*Type[0]*/ulong/*Type[0]*/> replaceFirst
            (this subArray</*Type[0]*/ulong/*Type[0]*/> array, /*Type[0]*/ulong/*Type[0]*/ value, Func</*Type[0]*/ulong/*Type[0]*/, bool> isValue)
        {
            if (array.Count != 0)
            {
                if (isValue == null) log.Error.Throw(log.exceptionType.Null);
                fixed (/*Type[0]*/ulong/*Type[0]*/* valueFixed = array.UnsafeArray)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/* valueIndex = arrayExtension.IndexOf(valueFixed + array.StartIndex, array.Count, isValue);
                    if (valueIndex != null) *valueIndex = value;
                }
            }
            return array;
        }
        /// <summary>
        /// 获取匹配集合
        /// </summary>
        /// <param name="array">数组数据</param>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>匹配集合</returns>
        public unsafe static subArray</*Type[0]*/ulong/*Type[0]*/> find
            (this subArray</*Type[0]*/ulong/*Type[0]*/> array, Func</*Type[0]*/ulong/*Type[0]*/, bool> isValue)
        {
            if (array.Count != 0)
            {
                if (isValue == null) log.Error.Throw(log.exceptionType.Null);
                fixed (/*Type[0]*/ulong/*Type[0]*/* valueFixed = array.UnsafeArray)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/* write = valueFixed + array.StartIndex, start = write, end = write + array.Count;
                    do
                    {
                        if (isValue(*start)) *write++ = *start;
                    }
                    while (++start != end);
                    return subArray</*Type[0]*/ulong/*Type[0]*/>.Unsafe(array.UnsafeArray, array.StartIndex, (int)(write - valueFixed) - array.StartIndex);
                }
            }
            return array;
        }
        /// <summary>
        /// 获取匹配集合
        /// </summary>
        /// <param name="array">数组数据</param>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>匹配集合</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static subArray</*Type[0]*/ulong/*Type[0]*/> getFind
            (this subArray</*Type[0]*/ulong/*Type[0]*/> array, Func</*Type[0]*/ulong/*Type[0]*/, bool> isValue)
        {
            if (array.Count != 0)
            {
                if (isValue == null) log.Error.Throw(log.exceptionType.Null);
                return arrayExtension.GetFind(array.UnsafeArray, array.StartIndex, array.Count, isValue);
            }
            return default(subArray</*Type[0]*/ulong/*Type[0]*/>);
        }
        /// <summary>
        /// 获取匹配数组
        /// </summary>
        /// <param name="array">数组数据</param>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>匹配数组</returns>
        public unsafe static /*Type[0]*/ulong/*Type[0]*/[] getFindArray
            (this subArray</*Type[0]*/ulong/*Type[0]*/> array, Func</*Type[0]*/ulong/*Type[0]*/, bool> isValue)
        {
            if (array.Count != 0)
            {
                if (isValue == null) log.Error.Throw(log.exceptionType.Null);
                int length = ((array.Count + 31) >> 5) << 2;
                memoryPool pool = fastCSharp.memoryPool.GetDefaultPool(length);
                byte[] data = pool.Get(length);
                try
                {
                    fixed (byte* dataFixed = data)
                    {
                        Array.Clear(data, 0, length);
                        return arrayExtension.GetFindArray(array.UnsafeArray, array.StartIndex, array.Count, isValue, new fixedMap(dataFixed));
                    }
                }
                finally { pool.PushNotNull(data); }
            }
            return nullValue</*Type[0]*/ulong/*Type[0]*/>.Array;
        }
        /// <summary>
        /// 数组类型转换
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="subArray">数据数组</param>
        /// <param name="getValue">数据获取器</param>
        /// <returns>目标数组</returns>
        public unsafe static /*Type[0]*/ulong/*Type[0]*/[] getArray<valueType>
            (this subArray<valueType> subArray, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getValue)
        {
            if (subArray.Count != 0)
            {
                if (getValue == null) log.Error.Throw(log.exceptionType.Null);
                valueType[] array = subArray.UnsafeArray;
                /*Type[0]*/
                ulong/*Type[0]*/[] newValues = new /*Type[0]*/ulong/*Type[0]*/[subArray.Count];
                fixed (/*Type[0]*/ulong/*Type[0]*/* newValueFixed = newValues)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/* write = newValueFixed;
                    int index = subArray.StartIndex, endIndex = index + subArray.Count;
                    do
                    {
                        *write++ = getValue(array[index++]);
                    }
                    while (index != endIndex);
                }
                return newValues;
            }
            return nullValue</*Type[0]*/ulong/*Type[0]*/>.Array;
        }
        /// <summary>
        /// 数组类型转换
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">数据数组</param>
        /// <param name="getValue">数据获取器</param>
        /// <returns>目标数组</returns>
        public unsafe static valueType[] getArray<valueType>
            (this subArray</*Type[0]*/ulong/*Type[0]*/> array, Func</*Type[0]*/ulong/*Type[0]*/, valueType> getValue)
        {
            if (array.Count != 0)
            {
                if (getValue == null) log.Error.Throw(log.exceptionType.Null);
                valueType[] newValues = new valueType[array.Count];
                fixed (/*Type[0]*/ulong/*Type[0]*/* arrayFixed = array.UnsafeArray)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/* start = arrayFixed + array.StartIndex, end = start + array.Count;
                    int index = 0;
                    do
                    {
                        newValues[index++] = getValue(*start);
                    }
                    while (++start != end);
                }
                return newValues;
            }
            return nullValue<valueType>.Array;
        }
        /// <summary>
        /// 遍历foreach
        /// </summary>
        /// <param name="array">数据数组</param>
        /// <param name="method">调用函数</param>
        /// <returns>数据数组</returns>
        public unsafe static subArray</*Type[0]*/ulong/*Type[0]*/> each
            (this subArray</*Type[0]*/ulong/*Type[0]*/> array, Action</*Type[0]*/ulong/*Type[0]*/> method)
        {
            if (array.Count != 0)
            {
                if (method == null) log.Error.Throw(log.exceptionType.Null);
                fixed (/*Type[0]*/ulong/*Type[0]*/* valueFixed = array.UnsafeArray)
                {
                    for (/*Type[0]*/ulong/*Type[0]*/* start = valueFixed + array.StartIndex, end = start + array.Count; start != end; method(*start++)) ;
                }
            }
            return array;
        }
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <param name="array">数据数组</param>
        /// <param name="value">最大值</param>
        /// <returns>是否存在最大值</returns>
        public unsafe static bool max(this subArray</*Type[0]*/ulong/*Type[0]*/> array, out /*Type[0]*/ulong/*Type[0]*/ value)
        {
            if (array.Count != 0)
            {
                fixed (/*Type[0]*/ulong/*Type[0]*/* valueFixed = array.UnsafeArray)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/* start = valueFixed + array.StartIndex, end = start + array.Count;
                    for (value = *start; ++start != end; )
                    {
                        if (*start > value) value = *start;
                    }
                    return true;
                }
            }
            value = /*Type[0]*/ulong/*Type[0]*/.MinValue;
            return false;
        }
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <param name="array">数据数组</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最大值,失败返回默认空值</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static /*Type[0]*/ulong/*Type[0]*/ max(this subArray</*Type[0]*/ulong/*Type[0]*/> array, /*Type[0]*/ulong/*Type[0]*/ nullValue)
        {
            /*Type[0]*/
            ulong/*Type[0]*/ value;
            return max(array, out value) ? value : nullValue;
        }
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="subArray">数据数组</param>
        /// <param name="getKey">数据获取器</param>
        /// <param name="value">最大值</param>
        /// <returns>是否存在最大值</returns>
        public static bool maxKey<valueType>
            (this subArray<valueType> subArray, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getKey, out /*Type[0]*/ulong/*Type[0]*/ value)
        {
            if (subArray.Count != 0)
            {
                valueType[] array = subArray.UnsafeArray;
                int index = subArray.StartIndex, endIndex = index + subArray.Count;
                value = getKey(array[index]);
                while (++index != endIndex)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/ nextKey = getKey(array[index]);
                    if (nextKey > value) value = nextKey;
                }
                return true;
            }
            value = /*Type[0]*/ulong/*Type[0]*/.MinValue;
            return false;
        }
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">数据数组</param>
        /// <param name="getKey">数据获取器</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最大值,失败返回默认空值</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static /*Type[0]*/ulong/*Type[0]*/ maxKey<valueType>
            (this subArray<valueType> array, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getKey, /*Type[0]*/ulong/*Type[0]*/ nullValue)
        {
            /*Type[0]*/
            ulong/*Type[0]*/ value;
            return maxKey(array, getKey, out value) ? value : nullValue;
        }
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="subArray">数据数组</param>
        /// <param name="getKey">数据获取器</param>
        /// <param name="value">最大值</param>
        /// <returns>是否存在最大值</returns>
        public static bool max<valueType>
            (this subArray<valueType> subArray, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getKey, out valueType value)
        {
            if (subArray.Count != 0)
            {
                valueType[] array = subArray.UnsafeArray;
                int index = subArray.StartIndex, endIndex = index + subArray.Count;
                /*Type[0]*/
                ulong/*Type[0]*/ maxKey = getKey(value = array[index]);
                while (++index != endIndex)
                {
                    valueType nextValue = array[index];
                    /*Type[0]*/
                    ulong/*Type[0]*/ nextKey = getKey(nextValue);
                    if (nextKey > maxKey)
                    {
                        maxKey = nextKey;
                        value = nextValue;
                    }
                }
                return true;
            }
            value = default(valueType);
            return false;
        }
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">数据数组</param>
        /// <param name="getKey">数据获取器</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最大值,失败返回默认空值</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType max<valueType>(this subArray<valueType> array, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getKey, valueType nullValue)
        {
            valueType value;
            return max(array, getKey, out value) ? value : nullValue;
        }
        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <param name="array">数据数组</param>
        /// <param name="value">最小值</param>
        /// <returns>是否存在最小值</returns>
        public unsafe static bool min(this subArray</*Type[0]*/ulong/*Type[0]*/> array, out /*Type[0]*/ulong/*Type[0]*/ value)
        {
            if (array.Count != 0)
            {
                fixed (/*Type[0]*/ulong/*Type[0]*/* valueFixed = array.UnsafeArray)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/* start = valueFixed + array.StartIndex, end = start + array.Count;
                    for (value = *start; ++start != end; )
                    {
                        if (*start < value) value = *start;
                    }
                    return true;
                }
            }
            value = /*Type[0]*/ulong/*Type[0]*/.MinValue;
            return false;
        }
        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <param name="array">数据数组</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最小值,失败返回默认空值</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static /*Type[0]*/ulong/*Type[0]*/ min(this subArray</*Type[0]*/ulong/*Type[0]*/> array, /*Type[0]*/ulong/*Type[0]*/ nullValue)
        {
            /*Type[0]*/
            ulong/*Type[0]*/ value;
            return min(array, out value) ? value : nullValue;
        }
        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="subArray">数据数组</param>
        /// <param name="getKey">数据获取器</param>
        /// <param name="value">最小值</param>
        /// <returns>是否存在最小值</returns>
        public static bool minKey<valueType>
            (this subArray<valueType> subArray, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getKey, out /*Type[0]*/ulong/*Type[0]*/ value)
        {
            if (subArray.Count != 0)
            {
                valueType[] array = subArray.UnsafeArray;
                int index = subArray.StartIndex, endIndex = index + subArray.Count;
                value = getKey(array[index]);
                while (++index != endIndex)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/ nextKey = getKey(array[index]);
                    if (nextKey < value) value = nextKey;
                }
                return true;
            }
            value = /*Type[0]*/ulong/*Type[0]*/.MinValue;
            return false;
        }
        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">数据数组</param>
        /// <param name="getKey">数据获取器</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最小值,失败返回默认空值</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static /*Type[0]*/ulong/*Type[0]*/ minKey<valueType>
            (this subArray<valueType> array, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getKey, /*Type[0]*/ulong/*Type[0]*/ nullValue)
        {
            /*Type[0]*/
            ulong/*Type[0]*/ value;
            return minKey(array, getKey, out value) ? value : nullValue;
        }
        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="subArray">数据数组</param>
        /// <param name="getKey">数据获取器</param>
        /// <param name="value">最小值</param>
        /// <returns>是否存在最小值</returns>
        public static bool min<valueType>
            (this subArray<valueType> subArray, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getKey, out valueType value)
        {
            if (subArray.Count != 0)
            {
                valueType[] array = subArray.UnsafeArray;
                int index = subArray.StartIndex, endIndex = index + subArray.Count;
                /*Type[0]*/
                ulong/*Type[0]*/ minKey = getKey(value = array[index]);
                while (++index != endIndex)
                {
                    valueType nextValue = array[index];
                    /*Type[0]*/
                    ulong/*Type[0]*/ nextKey = getKey(nextValue);
                    if (nextKey < minKey)
                    {
                        minKey = nextKey;
                        value = nextValue;
                    }
                }
                return true;
            }
            value = default(valueType);
            return false;
        }
        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">数据数组</param>
        /// <param name="getKey">数据获取器</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最小值,失败返回默认空值</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType min<valueType>(this subArray<valueType> array, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getKey, valueType nullValue)
        {
            valueType value;
            return min(array, getKey, out value) ? value : nullValue;
        }
    }
}

namespace fastCSharp.unsafer
{
    /// <summary>
    /// 数组非安全扩展操作(请自行确保数据可靠性)
    /// </summary>
    public static partial class arrayExtension
    {
        /// <summary>
        /// 移动数据块
        /// </summary>
        /// <param name="array">待处理数组</param>
        /// <param name="index">原始数据位置</param>
        /// <param name="writeIndex">目标数据位置</param>
        /// <param name="count">移动数据数量</param>
#if MONO
#else
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
#endif
        public unsafe static void Move(/*Type[0]*/ulong/*Type[0]*/[] array, int index, int writeIndex, int count)
        {
#if MONO
            int endIndex = index + count;
            if (index < writeIndex && endIndex > writeIndex)
            {
                fixed (/*Type[0]*/ulong/*Type[0]*/* valueFixed = array)
                {
                    for (/*Type[0]*/ulong/*Type[0]*/* write = valueFixed + writeIndex + count, start = valueFixed + index, end = valueFixed + endIndex;
                        end != start;
                        *--write = *--end) ;
                }
            }
            else Array.Copy(array, index, array, writeIndex, count);
#else
            fixed (/*Type[0]*/ulong/*Type[0]*/* valueFixed = array) win32.kernel32.RtlMoveMemory(valueFixed + writeIndex, valueFixed + index, count * sizeof(/*Type[0]*/ulong/*Type[0]*/));
#endif
        }
    }
}