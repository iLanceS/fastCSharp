using System;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 数组子串
    /// </summary>
    public struct subArray<valueType> : IList<valueType>
    {
        /// <summary>
        /// 原数组
        /// </summary>
        internal valueType[] array;
        /// <summary>
        /// 原数组
        /// </summary>
        public valueType[] UnsafeArray
        {
            get { return array; }
        }
        /// <summary>
        /// 设置或获取值
        /// </summary>
        /// <param name="index">位置</param>
        /// <returns>数据值</returns>
        public valueType this[int index]
        {
            get
            {
                if ((uint)index < (uint)length) return array[startIndex + index];
                log.Error.Throw(log.exceptionType.IndexOutOfRange);
                return default(valueType);
            }
            set
            {
                if ((uint)index < (uint)length) array[startIndex + index] = value;
                else log.Error.Throw(log.exceptionType.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 原数组中的起始位置
        /// </summary>
        internal int startIndex;
        /// <summary>
        /// 原数组中的起始位置
        /// </summary>
        public int StartIndex
        {
            get { return startIndex; }
        }
        /// <summary>
        /// 长度
        /// </summary>
        internal int length;
        /// <summary>
        /// 长度
        /// </summary>
        public int Count
        {
            get
            {
                return length;
            }
        }
        /// <summary>
        /// 只读
        /// </summary>
        public bool IsReadOnly { get { return true; } }
        /// <summary>
        /// 数据结束位置
        /// </summary>
        internal int EndIndex
        {
            get
            {
                return startIndex + length;
            }
        }
        /// <summary>
        /// 最后空闲长度
        /// </summary>
        internal int FreeLength
        {
            get
            {
                return array == null ? 0 : (array.Length - startIndex - length);
            }
        }
        /// <summary>
        /// 数组子串
        /// </summary>
        /// <param name="size">容器大小</param>
        public subArray(int size)
        {
            array = size > 0 ? new valueType[size] : null;
            startIndex = length = 0;
        }
        /// <summary>
        /// 数组子串
        /// </summary>
        /// <param name="value">数组</param>
        public subArray(valueType[] value)
        {
            array = value;
            startIndex = 0;
            length = value == null ? 0 : value.Length;
        }
        /// <summary>
        /// 数组子串
        /// </summary>
        /// <param name="value">数组</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="length">长度</param>
        internal subArray(valueType[] value, int startIndex, int length)
        {
            array = value;
            this.startIndex = startIndex;
            this.length = length;
        }
        /// <summary>
        /// 强制类型转换
        /// </summary>
        /// <param name="value">单向动态数组</param>
        /// <returns>数组数据</returns>
        public static explicit operator valueType[](subArray<valueType> value)
        {
            return value.ToArray();
        }
        /// <summary>
        /// 强制类型转换
        /// </summary>
        /// <param name="value">数组数据</param>
        /// <returns>单向动态数组</returns>
        public static explicit operator subArray<valueType>(valueType[] value)
        {
            return new subArray<valueType>(value);
        }
        /// <summary>
        /// 枚举器
        /// </summary>
        /// <returns>枚举器</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        IEnumerator<valueType> IEnumerable<valueType>.GetEnumerator()
        {
            if (length != 0) return new iEnumerator<valueType>.array(this);
            return iEnumerator<valueType>.Empty;
        }
        /// <summary>
        /// 枚举器
        /// </summary>
        /// <returns>枚举器</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator()
        {
            if (length != 0) return new iEnumerator<valueType>.array(this);
            return iEnumerator<valueType>.Empty;
        }
        /// <summary>
        /// 反转枚举
        /// </summary>
        /// <returns></returns>
        public IEnumerable<valueType> ReverseEnumerable()
        {
            for (int endIndex = startIndex + length; endIndex > startIndex; ) yield return array[--endIndex];
        }
        /// <summary>
        /// 转换数组
        /// </summary>
        /// <returns>数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType[] ToArray()
        {
            if (length == 0) return nullValue<valueType>.Array;
            return length == array.Length ? array : getArray();
        }
        /// <summary>
        /// 转换数组
        /// </summary>
        /// <returns>数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType[] GetArray()
        {
            return length != 0 ? getArray() : nullValue<valueType>.Array;
        }
        /// <summary>
        /// 转换数组
        /// </summary>
        /// <returns>数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private valueType[] getArray()
        {
            valueType[] newArray = new valueType[length];
            System.Array.Copy(array, startIndex, newArray, 0, length);
            return newArray;
        }
        /// <summary>
        /// 反转数组
        /// </summary>
        /// <returns></returns>
        public subArray<valueType> Reverse()
        {
            for (int index = startIndex, endIndex = index + length, middle = index + (length >> 1); index != middle;)
            {
                valueType value = array[index];
                array[index++] = array[--endIndex];
                array[endIndex] = value;
            }
            return this;
        }
        /// <summary>
        /// 获取反转数组
        /// </summary>
        /// <returns></returns>
        public valueType[] GetReverse()
        {
            if (length == 0) return nullValue<valueType>.Array;
            valueType[] newArray = new valueType[length];
            if (startIndex == 0)
            {
                int index = length;
                foreach(valueType value in array)
                {
                    newArray[--index] = value;
                    if (index == 0) break;
                }
            }
            else
            {
                int index = length, copyIndex = startIndex;
                do
                {
                    newArray[--index] = array[copyIndex++];
                }
                while (index != 0);
            }
            return newArray;
        }
        /// <summary>
        /// 获取反转数组
        /// </summary>
        /// <typeparam name="arrayType"></typeparam>
        /// <param name="getValue">数组转换</param>
        /// <returns></returns>
        public arrayType[] GetReverse<arrayType>(Func<valueType, arrayType> getValue)
        {
            if (length == 0) return nullValue<arrayType>.Array;
            arrayType[] newArray = new arrayType[length];
            if (startIndex == 0)
            {
                int index = length;
                foreach (valueType value in array)
                {
                    newArray[--index] = getValue(value);
                    if (index == 0) break;
                }
            }
            else
            {
                int index = length, copyIndex = startIndex;
                do
                {
                    newArray[--index] = getValue(array[copyIndex++]);
                }
                while (index != 0);
            }
            return newArray;
        }
        /// <summary>
        /// 置空并释放数组
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Null()
        {
            array = null;
            startIndex = length = 0;
        }
        /// <summary>
        /// 重置数据
        /// </summary>
        /// <param name="value">数组,不能为null</param>
        /// <param name="startIndex">起始位置,必须合法</param>
        /// <param name="length">长度,必须合法</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void UnsafeSet(valueType[] value, int startIndex, int length)
        {
            array = value;
            this.startIndex = startIndex;
            this.length = length;
        }
        /// <summary>
        /// 重置数据
        /// </summary>
        /// <param name="startIndex">起始位置,必须合法</param>
        /// <param name="length">长度,必须合法</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void UnsafeSet(int startIndex, int length)
        {
            this.startIndex = startIndex;
            this.length = length;
        }
        /// <summary>
        /// 设置数据长度
        /// </summary>
        /// <param name="length">长度,必须合法</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void UnsafeSetLength(int length)
        {
            this.length = length;
        }
        /// <summary>
        /// 设置数据容器长度
        /// </summary>
        /// <param name="count">数据长度</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void setLength(int count)
        {
            valueType[] newArray = dynamicArray<valueType>.GetNewArray(count);
            System.Array.Copy(array, startIndex, newArray, startIndex, length);
            array = newArray;
        }
        /// <summary>
        /// 长度设为0
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Empty()
        {
            startIndex = length = 0;
        }
        /// <summary>
        /// 清除所有数据
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Clear()
        {
            if (array != null)
            {
                if (dynamicArray<valueType>.IsClearArray) System.Array.Clear(array, 0, array.Length);
                Empty();
            }
        }
        /// <summary>
        /// 获取匹配数量
        /// </summary>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>获取匹配数量</returns>
        public int GetCount(Func<valueType, bool> isValue)
        {
            if (isValue == null) log.Error.Throw(log.exceptionType.Null);
            if (length != 0)
            {
                int count = 0;
                if (startIndex == 0)
                {
                    int index = length;
                    foreach (valueType value in array)
                    {
                        if (isValue(value)) ++count;
                        if (--index == 0) break;
                    }
                }
                else
                {
                    int index = startIndex, endIndex = startIndex + length;
                    do
                    {
                        if (isValue(array[index])) ++count;
                    }
                    while (++index != endIndex);
                }
                return count;
            }
            return 0;
        }
        /// <summary>
        /// 判断是否存在数据
        /// </summary>
        /// <param name="value">匹配数据</param>
        /// <returns>是否存在数据</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool Contains(valueType value)
        {
            return IndexOf(value) != -1;
        }
        /// <summary>
        /// 获取匹配数据位置
        /// </summary>
        /// <param name="value">匹配数据</param>
        /// <returns>匹配位置,失败为-1</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public int IndexOf(valueType value)
        {
            if (length != 0)
            {
                int index = System.Array.IndexOf<valueType>(array, value, startIndex, length);
                if (index >= 0) return index - startIndex;
            }
            return -1;
        }
        /// <summary>
        /// 判断是否存在匹配
        /// </summary>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>是否存在匹配</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool Any(Func<valueType, bool> isValue)
        {
            if (isValue == null) log.Error.Throw(log.exceptionType.Null);
            return indexOf(isValue) != -1;
        }
        /// <summary>
        /// 获取获取数组中的匹配位置
        /// </summary>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>数组中的匹配位置,失败为-1</returns>
        private int indexOf(Func<valueType, bool> isValue)
        {
            if (length != 0)
            {
                if (startIndex == 0)
                {
                    int index = 0;
                    foreach (valueType value in array)
                    {
                        if (isValue(value)) return index;
                        if (++index == length) break;
                    }
                }
                else
                {
                    int index = startIndex, endIndex = startIndex + length;
                    do
                    {
                        if (isValue(array[index])) return index;
                    }
                    while (++index != endIndex);
                }
            }
            return -1;
        }
        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="value">数据</param>
        /// <returns>是否存在移除数据</returns>
        public bool Remove(valueType value)
        {
            int index = IndexOf(value);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="index">数据位置</param>
        /// <returns>被移除数据</returns>
        public void RemoveAt(int index)
        {
            if ((uint)index < (uint)length)
            {
                fastCSharp.unsafer.array.Move(array, startIndex + index + 1, startIndex + index, --length - index);
                array[startIndex + length] = default(valueType);
            }
            else log.Error.Throw(log.exceptionType.IndexOutOfRange);
        }
        /// <summary>
        /// 移除数据并使用最后一个数据移动到当前位置
        /// </summary>
        /// <param name="index">数据位置</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void RemoveAtEnd(int index)
        {
            if ((uint)index < (uint)length) array[startIndex + index] = array[startIndex + --length];
            else log.Error.Throw(log.exceptionType.IndexOutOfRange);
        }
        /// <summary>
        /// 移除所有后端匹配值
        /// </summary>
        /// <param name="isValue">数据匹配器</param>
        private void removeEnd(Func<valueType, bool> isValue)
        {
            for (int index = startIndex + length; index != startIndex; --length)
            {
                if (!isValue(array[--index])) return;
            }
        }
        /// <summary>
        /// 移除匹配值
        /// </summary>
        /// <param name="isValue">数据匹配器</param>
        /// <returns></returns>
        public subArray<valueType> Remove(Func<valueType, bool> isValue)
        {
            if (isValue == null) log.Error.Throw(log.exceptionType.Null);
            if (length != 0)
            {
                removeEnd(isValue);
                int index = indexOf(isValue);
                if (index != -1)
                {
                    for (int start = index, endIndex = startIndex + length; ++start != endIndex; )
                    {
                        if (!isValue(array[start])) array[index++] = array[start];
                    }
                    length = index - startIndex;
                }
            }
            return this;
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="index">插入位置</param>
        /// <param name="value">数据</param>
        public void Insert(int index, valueType value)
        {
            if ((uint)index <= (uint)length)
            {
                if (index != length)
                {
                    if (startIndex + length != array.Length)
                    {
                        fastCSharp.unsafer.array.Move(array, startIndex + index, startIndex + index + 1, length - index);
                        array[startIndex + index] = value;
                        ++length;
                    }
                    else
                    {
                        valueType[] values = dynamicArray<valueType>.GetNewArray(array.Length << 1);
                        System.Array.Copy(array, startIndex, values, startIndex, index);
                        values[startIndex + index] = value;
                        System.Array.Copy(array, startIndex + index, values, startIndex + index + 1, length++ - index);
                        array = values;
                    }
                }
                else Add(value);
            }
            else log.Error.Throw(log.exceptionType.IndexOutOfRange);
        }
        /// <summary>
        /// 获取第一个匹配值
        /// </summary>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>匹配值,失败为 default(valueType)</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType FirstOrDefault(Func<valueType, bool> isValue)
        {
            if (isValue == null) log.Error.Throw(log.exceptionType.Null);
            int index = indexOf(isValue);
            return index != -1 ? array[index] : default(valueType);
        }
        /// <summary>
        /// 获取最后一个值
        /// </summary>
        /// <returns>最后一个值,失败为default(valueType)</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType LastOrDefault()
        {
            return length != 0 ? array[startIndex + length - 1] : default(valueType);
        }
        /// <summary>
        /// 复制数据
        /// </summary>
        /// <param name="values">目标数据</param>
        /// <param name="index">目标位置</param>
        public void CopyTo(valueType[] values, int index)
        {
            if (values != null && index >= 0 && length + index <= values.Length)
            {
                if (length != 0) System.Array.Copy(array, startIndex, values, index, length);
            }
            else log.Error.Throw(log.exceptionType.IndexOutOfRange);
        }
        /// <summary>
        /// 转换成子集合(不清除数组)
        /// </summary>
        /// <param name="index">起始位置</param>
        /// <param name="count">数量,小于0表示所有</param>
        /// <returns>子集合</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public subArray<valueType> Sub(int index, int count)
        {
            array.range range = new array.range(length, index, count < 0 ? length - index : count);
            if (range.GetCount > 0)
            {
                startIndex += range.SkipCount;
                length = range.GetCount;
            }
            else startIndex = length = 0;
            return this;
        }
        /// <summary>
        /// 转换成子集合(不清除数组)
        /// </summary>
        /// <param name="index">起始位置</param>
        /// <param name="count">数量,小于0表示所有</param>
        /// <returns>子集合</returns>
        public subArray<valueType> GetSub(int index, int count)
        {
            array.range range = new array.range(length, index, count < 0 ? length - index : count);
            if (range.GetCount > 0)
            {
                return subArray<valueType>.Unsafe(array, startIndex + range.SkipCount, range.GetCount);
            }
            return default(subArray<valueType>);
        }
        /// <summary>
        /// 获取分页字段数组
        /// </summary>
        /// <param name="pageSize">分页尺寸</param>
        /// <param name="currentPage">页号</param>
        /// <returns>分页字段数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public subArray<valueType> Page(int pageSize, int currentPage)
        {
            array.page page = new array.page(length, pageSize, currentPage);
            return subArray<valueType>.Unsafe(array, startIndex + page.SkipCount, page.CurrentPageSize);
        }
        /// <summary>
        /// 获取分页字段数组
        /// </summary>
        /// <param name="pageSize">分页尺寸</param>
        /// <param name="currentPage">页号</param>
        /// <param name="count"></param>
        /// <returns>分页字段数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public subArray<valueType> Page(int pageSize, int currentPage, out int count)
        {
            array.page page = new array.page(count = length, pageSize, currentPage);
            return subArray<valueType>.Unsafe(array, startIndex + page.SkipCount, page.CurrentPageSize);
        }
        /// <summary>
        /// 获取分页字段数组
        /// </summary>
        /// <typeparam name="arrayType"></typeparam>
        /// <param name="getValue">数组转换</param>
        /// <param name="pageSize">分页尺寸</param>
        /// <param name="currentPage">页号</param>
        /// <param name="count"></param>
        /// <returns>分页字段数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public arrayType[] GetPage<arrayType>(Func<valueType, arrayType> getValue, int pageSize, int currentPage, out int count)
        {
            if (getValue == null) fastCSharp.log.Default.Throw(fastCSharp.log.exceptionType.Null);
            array.page page = new array.page(count = length, pageSize, currentPage);
            if (page.CurrentPageSize != 0)
            {
                arrayType[] values = new arrayType[page.CurrentPageSize];
                int readIndex = startIndex + page.SkipCount, endIndex = readIndex + page.CurrentPageSize, writeIndex = 0;
                do
                {
                    values[writeIndex++] = getValue(array[readIndex]);
                }
                while (++readIndex != endIndex);
                return values;
            }
            return nullValue<arrayType>.Array;
        }
        /// <summary>
        /// 获取分页字段数组
        /// </summary>
        /// <param name="pageSize">分页尺寸</param>
        /// <param name="currentPage">页号</param>
        /// <returns>分页字段数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public subArray<valueType> PageDesc(int pageSize, int currentPage)
        {
            array.page page = new array.page(length, pageSize, currentPage);
            return subArray<valueType>.Unsafe(array, startIndex + page.DescSkipCount, page.CurrentPageSize).Reverse();
        }
        /// <summary>
        /// 获取分页字段数组
        /// </summary>
        /// <param name="pageSize">分页尺寸</param>
        /// <param name="currentPage">页号</param>
        /// <param name="count"></param>
        /// <returns>分页字段数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public subArray<valueType> PageDesc(int pageSize, int currentPage, out int count)
        {
            array.page page = new array.page(count = length, pageSize, currentPage);
            return subArray<valueType>.Unsafe(array, startIndex + page.DescSkipCount, page.CurrentPageSize).Reverse();
        }
        /// <summary>
        /// 获取分页字段数组
        /// </summary>
        /// <param name="pageSize">分页尺寸</param>
        /// <param name="currentPage">页号</param>
        /// <returns>分页字段数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType[] GetPageDesc(int pageSize, int currentPage)
        {
            array.page page = new array.page(length, pageSize, currentPage);
            return subArray<valueType>.Unsafe(array, startIndex + page.DescSkipCount, page.CurrentPageSize).GetReverse();
        }
        /// <summary>
        /// 获取分页字段数组
        /// </summary>
        /// <param name="pageSize">分页尺寸</param>
        /// <param name="currentPage">页号</param>
        /// <param name="count"></param>
        /// <returns>分页字段数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType[] GetPageDesc(int pageSize, int currentPage, out int count)
        {
            array.page page = new array.page(count = length, pageSize, currentPage);
            return subArray<valueType>.Unsafe(array, startIndex + page.DescSkipCount, page.CurrentPageSize).GetReverse();
        }
        /// <summary>
        /// 获取分页字段数组
        /// </summary>
        /// <typeparam name="arrayType"></typeparam>
        /// <param name="getValue">数组转换</param>
        /// <param name="pageSize">分页尺寸</param>
        /// <param name="currentPage">页号</param>
        /// <param name="count"></param>
        /// <returns>分页字段数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public arrayType[] GetPageDesc<arrayType>(Func<valueType, arrayType> getValue, int pageSize, int currentPage, out int count)
        {
            if (getValue == null) fastCSharp.log.Default.Throw(fastCSharp.log.exceptionType.Null);
            array.page page = new array.page(count = length, pageSize, currentPage);
            if (page.CurrentPageSize != 0)
            {
                arrayType[] values = new arrayType[page.CurrentPageSize];
                int startIndex = this.startIndex + page.DescSkipCount, readIndex = startIndex + page.CurrentPageSize, writeIndex = 0;
                do
                {
                    values[writeIndex++] = getValue(array[--readIndex]);
                }
                while (readIndex != startIndex);
                return values;
            }
            return nullValue<arrayType>.Array;
        }
        /// <summary>
        /// 增加数据长度
        /// </summary>
        /// <param name="length">数据长度</param>
        private void addToLength(int length)
        {
            if (array == null) array = new valueType[length < sizeof(int) ? sizeof(int) : length];
            else if (length > array.Length) setLength(length);
        }
        /// <summary>
        /// 预增长度
        /// </summary>
        /// <param name="length"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void PrepLength(int length)
        {
            if (array == null) array = new valueType[length < sizeof(int) ? sizeof(int) : length];
            else if ((length += this.length) > array.Length) setLength(Math.Max(length, array.Length << 1));
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value">数据</param>
        public void Add(valueType value)
        {
            if (array == null)
            {
                array = new valueType[sizeof(int)];
                array[0] = value;
                length = 1;
            }
            else
            {
                int index = startIndex + length;
                if (index == array.Length)
                {
                    if (index == 0) array = new valueType[sizeof(int)];
                    else setLength(index << 1);
                }
                array[index] = value;
                ++length;
            }
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void UnsafeAdd(valueType value)
        {
            array[startIndex + length++] = value;
        }
        /// <summary>
        /// 添加数据集合
        /// </summary>
        /// <param name="values">数据集合</param>
        public void Add(ICollection<valueType> values)
        {
            int count = values.count();
            if (count != 0)
            {
                int index = startIndex + length;
                addToLength(index + count);
                foreach (valueType value in values) array[index++] = value;
                length += count;
            }
        }
        /// <summary>
        /// 添加数据集合
        /// </summary>
        /// <param name="values">数据集合</param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public subArray<valueType> Add(valueType[] values)
        {
            if (values != null && values.Length != 0) add(values, 0, values.Length);
            return this;
        }
        /// <summary>
        /// 添加数据集合
        /// </summary>
        /// <param name="values">数据集合</param>
        /// <param name="index">起始位置</param>
        /// <param name="count">数量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Add(valueType[] values, int index, int count)
        {
            array.range range = new array.range(values.length(), index, count);
            if ((count = range.GetCount) != 0) add(values, range.SkipCount, count);
        }
        /// <summary>
        /// 添加数据集合
        /// </summary>
        /// <param name="values">数据集合</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Add(subArray<valueType> values)
        {
            Add(ref values);
        }
        /// <summary>
        /// 添加数据集合
        /// </summary>
        /// <param name="values">数据集合</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Add(ref subArray<valueType> values)
        {
            if (values.length != 0) add(values.array, values.startIndex, values.length);
        }
        /// <summary>
        /// 添加数据集合
        /// </summary>
        /// <param name="values">数据集合</param>
        /// <param name="index">起始位置</param>
        /// <param name="count">数量</param>
        private void add(valueType[] values, int index, int count)
        {
            int newLength = startIndex + length + count;
            addToLength(newLength);
            System.Array.Copy(values, index, array, startIndex + length, count);
            length += count;
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void UnsafeAddExpand(valueType value)
        {
            array[--startIndex] = value;
            ++length;
        }
        /// <summary>
        /// 弹出数据
        /// </summary>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType Pop()
        {
            if (length == 0) log.Error.Throw(log.exceptionType.IndexOutOfRange);
            return array[startIndex + --length];
        }
        /// <summary>
        /// 弹出数据
        /// </summary>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType UnsafePop()
        {
            return array[startIndex + --length];
        }
        /// <summary>
        /// 弹出数据
        /// </summary>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal valueType UnsafePopReset()
        {
            int index = startIndex + --length;
            valueType value = array[index];
            array[index] = default(valueType);
            return value;
        }
        /// <summary>
        /// 转换数组
        /// </summary>
        /// <typeparam name="arrayType">数组类型</typeparam>
        /// <param name="getValue">数据获取委托</param>
        /// <returns>数组</returns>
        public arrayType[] GetArray<arrayType>(Func<valueType, arrayType> getValue)
        {
            //if (array != null)
            //{
                if (length != 0)
                {
                    if (getValue == null) log.Error.Throw(log.exceptionType.Null);
                    arrayType[] newArray = new arrayType[length];
                    for (int count = 0, index = startIndex, endIndex = startIndex + length; index != endIndex; ++index)
                    {
                        newArray[count++] = getValue(array[index]);
                    }
                    return newArray;
                }
                return nullValue<arrayType>.Array;
            //}
            //return null;
        }
        /// <summary>
        /// 获取匹配值集合
        /// </summary>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>匹配值集合</returns>
        public unsafe valueType[] GetFindArray(Func<valueType, bool> isValue)
        {
            if (isValue == null) log.Error.Throw(log.exceptionType.Null);
            if (this.length != 0)
            {
                int length = ((this.length + 31) >> 5) << 2;
                memoryPool pool = fastCSharp.memoryPool.GetDefaultPool(length);
                byte[] data = pool.Get(length);
                try
                {
                    fixed (byte* dataFixed = data)
                    {
                        System.Array.Clear(data, 0, length);
                        return getFindArray(isValue, new fixedMap(dataFixed));
                    }
                }
                finally { pool.PushNotNull(data); }
            }
            return nullValue<valueType>.Array;
        }
        /// <summary>
        /// 获取匹配值集合
        /// </summary>
        /// <param name="isValue">数据匹配器</param>
        /// <param name="map">匹配结果位图</param>
        /// <returns>匹配值集合</returns>
        private valueType[] getFindArray(Func<valueType, bool> isValue, fixedMap map)
        {
            if (startIndex == 0)
            {
                int count = 0, index = 0;
                foreach (valueType value in array)
                {
                    if (isValue(value))
                    {
                        ++count;
                        map.Set(index);
                    }
                    if (++index == length) break;
                }
                if (count != 0)
                {
                    valueType[] values = new valueType[count];
                    for (index = length; count != 0; values[--count] = array[index])
                    {
                        while (!map.Get(--index)) ;
                    }
                    return values;
                }
            }
            else
            {
                int count = 0, index = startIndex, endIndex = startIndex + length;
                do
                {
                    if (isValue(array[index]))
                    {
                        ++count;
                        map.Set(index - startIndex);
                    }
                }
                while (++index != endIndex);
                if (count != 0)
                {
                    valueType[] values = new valueType[count];
                    for (index = length; count != 0; values[--count] = array[startIndex + index])
                    {
                        while (!map.Get(--index)) ;
                    }
                    return values;
                }
            }
            return nullValue<valueType>.Array;
        }
        /// <summary>
        /// 转换为单向动态数组
        /// </summary>
        /// <returns>单向动态数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public list<valueType> ToList()
        {
            return length != 0 ? new list<valueType>(ref this, true) : null;
        }
        /// <summary>
        /// 转换为单向动态数组
        /// </summary>
        /// <returns>单向动态数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public collection<valueType> ToCollection()
        {
            return length != 0 ? new collection<valueType>(ref this, true) : null;
        }
        /// <summary>
        /// 连接数组
        /// </summary>
        /// <typeparam name="arrayType">目标数组类型</typeparam>
        /// <param name="getValue">数据获取器</param>
        /// <returns>目标数组</returns>
        public subArray<arrayType> Concat<arrayType>(Func<valueType, subArray<arrayType>> getValue)
        {
            if (getValue == null) log.Error.Throw(log.exceptionType.Null);
            if (length != 0)
            {
                subArray<arrayType> values = new subArray<arrayType>();
                if (startIndex == 0)
                {
                    int index = length;
                    foreach (valueType value in array)
                    {
                        values.Add(getValue(value));
                        if (--index == 0) break;
                    }
                }
                else
                {
                    int index = startIndex, endIndex = startIndex + length;
                    do
                    {
                        values.Add(getValue(array[index]));
                    }
                    while (++index != endIndex);
                }
                return values;
            }
            return default(subArray<arrayType>);
        }
        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="comparer">比较器</param>
        /// <returns>排序后的数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public subArray<valueType> Sort(Func<valueType, valueType, int> comparer)
        {
            algorithm.quickSort.Sort(array, comparer, startIndex, length);
            return this;
        }
        /// <summary>
        /// 分割数组
        /// </summary>
        /// <param name="count">子数组长度</param>
        /// <returns>分割后的数组集合</returns>
        public subArray<valueType>[] Split(int count)
        {
            return this.length != 0 && count > 0 ? array.split(0, this.length, count) : nullValue<subArray<valueType>>.Array;
        }
        /// <summary>
        /// 数组子串(非安全,请自行确保数据可靠性)
        /// </summary>
        /// <param name="value">数组,不能为null</param>
        /// <param name="startIndex">起始位置,必须合法</param>
        /// <param name="length">长度,必须合法</param>
        /// <returns>数组子串</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray<valueType> Unsafe(valueType[] value, int startIndex, int length)
        {
            return new subArray<valueType> { array = value, startIndex = startIndex, length = length };
        }
        /// <summary>
        /// 数组子串
        /// </summary>
        /// <param name="value">数组</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="length">长度</param>
        public static subArray<valueType> Create(valueType[] value, int startIndex, int length)
        {
            if (value == null) fastCSharp.log.Error.Throw(log.exceptionType.Null);
            array.range range = new array.range(value.Length, startIndex, length);
            if (range.GetCount != length) fastCSharp.log.Error.Throw(log.exceptionType.IndexOutOfRange);
            return range.GetCount == 0 ? new subArray<valueType>(nullValue<valueType>.Array, 0, 0) : new subArray<valueType>(value, range.SkipCount, range.GetCount);
        }
    }
}
