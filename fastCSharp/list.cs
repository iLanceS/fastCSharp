using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 单向动态数组
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    public class list<valueType> : dynamicArray<valueType>, IList<valueType>
    {
        /// <summary>
        /// 数据数量
        /// </summary>
        internal int length;
        /// <summary>
        /// 数据数量
        /// </summary>
        public int Count
        {
            get { return length; }
        }
        /// <summary>
        /// 最后空闲长度
        /// </summary>
        internal int FreeLength
        {
            get
            {
                return array.Length - length;
            }
        }
        /// <summary>
        /// 数据数量
        /// </summary>
        protected override int ValueCount { get { return length; } }
        /// <summary>
        /// 设置或获取值
        /// </summary>
        /// <param name="index">位置</param>
        /// <returns>数据值</returns>
        public valueType this[int index]
        {
            get
            {
                if ((uint)index < (uint)length) return array[index];
                log.Error.Throw(log.exceptionType.IndexOutOfRange);
                return default(valueType);
            }
            set
            {
                if ((uint)index < (uint)length) array[index] = value;
                else log.Error.Throw(log.exceptionType.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 根据位置设置数据
        /// </summary>
        /// <param name="index">数据位置</param>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Set(int index, valueType value)
        {
            if (index < 0) log.Error.Throw(log.exceptionType.IndexOutOfRange);
            if (index >= length) addToLength(index + 1);
            array[index] = value;
        }
        /// <summary>
        /// 单向动态数据
        /// </summary>
        public list() { array = nullValue<valueType>.Array; }
        /// <summary>
        /// 单向动态数据
        /// </summary>
        /// <param name="count">数组容器尺寸</param>
        public list(int count)
        {
            array = count > 0 ? new valueType[count] : nullValue<valueType>.Array;
        }
        /// <summary>
        /// 单向动态数据
        /// </summary>
        /// <param name="values">数据集合</param>
        public list(ICollection<valueType> values) : this(values == null ? null : values.getArray(), true) { }
        /// <summary>
        /// 单向动态数据
        /// </summary>
        /// <param name="values">数据数组</param>
        /// <param name="isUnsafe">true表示使用原数组,false表示需要复制数组</param>
        public list(valueType[] values, bool isUnsafe = false)
        {
            if ((length = values.length()) == 0) array = nullValue<valueType>.Array;
            else if (isUnsafe) array = values;
            else Array.Copy(values, 0, array = GetNewArray(length), 0, length);
        }
        /// <summary>
        /// 单向动态数据
        /// </summary>
        /// <param name="values">数据数组</param>
        /// <param name="index">起始位置</param>
        /// <param name="count">数据数量</param>
        /// <param name="isUnsafe">true表示可以使用原数组,false表示需要复制数组</param>
        public list(valueType[] values, int index, int count, bool isUnsafe = false)
        {
            int length = values.length();
            array.range range = new array.range(length, index, count);
            this.length = range.GetCount;
            if (isUnsafe)
            {
                if (this.length == 0) array = values ?? nullValue<valueType>.Array;
                else
                {
                    if ((index = range.SkipCount) == 0) array = values;
                    else Array.Copy(values, index, array = GetNewArray(count), 0, this.length);
                }
            }
            else if (this.length == 0) array = nullValue<valueType>.Array;
            else Array.Copy(values, range.SkipCount, array = GetNewArray(count), 0, this.length);
        }
        /// <summary>
        /// 单向动态数据
        /// </summary>
        /// <param name="values">数据数组</param>
        /// <param name="isUnsafe">true表示可以使用原数组,false表示需要复制数组</param>
        public list(ref subArray<valueType> values, bool isUnsafe = false)
        {
            length = values.length;
            if (isUnsafe)
            {
                if (length == 0)
                {
                    array = values.array ?? nullValue<valueType>.Array; ;
                }
                else
                {
                    if (values.startIndex == 0) array = values.array;
                    else Array.Copy(values.array, values.startIndex, array = GetNewArray(length), 0, length);
                }
            }
            else if (this.length == 0) array = nullValue<valueType>.Array;
            else Array.Copy(values.array, values.startIndex, array = GetNewArray(length), 0, length);
        }
        /// <summary>
        /// 强制类型转换
        /// </summary>
        /// <param name="value">单向动态数组</param>
        /// <returns>数组数据</returns>
        public static explicit operator valueType[](list<valueType> value)
        {
            return value != null ? value.ToArray() : null;
        }
        /// <summary>
        /// 强制类型转换
        /// </summary>
        /// <param name="value">数组数据</param>
        /// <returns>单向动态数组</returns>
        public static explicit operator list<valueType>(valueType[] value)
        {
            return value != null ? new list<valueType>(value, true) : null;
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
        /// 获取值
        /// </summary>
        /// <param name="index">位置</param>
        /// <returns>数据值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal valueType UnsafeGet(int index)
        {
            return array[index];
        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="index">位置</param>
        /// <param name="value">数据值</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void UnsafeSet(int index, valueType value)
        {
            array[index] = value;
        }
        /// <summary>
        /// 增加数据长度
        /// </summary>
        /// <param name="length">增加数据长度</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void UnsafeAddLength(int length)
        {
            this.length += length;
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void UnsafeAdd(valueType value)
        {
            array[length++] = value;
        }
        /// <summary>
        /// 弹出最后一个数据
        /// </summary>
        /// <returns>最后一个数据</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType UnsafePop()
        {
            return array[--length];
        }
        /// <summary>
        /// 弹出最后一个数据
        /// </summary>
        /// <returns>最后一个数据</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal valueType UnsafePopReset()
        {
            valueType value = array[--length];
            array[length] = default(valueType);
            return value;
        }
        /// <summary>
        /// 添加数据集合
        /// </summary>
        /// <param name="values">数据集合</param>
        /// <param name="index">起始位置</param>
        /// <param name="count">数量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void UnsafeAdd(valueType[] values, int index, int count)
        {
            System.Array.Copy(values, index, array, length, count);
            length += count;
        }
        /// <summary>
        /// 转换成子集合
        /// </summary>
        /// <param name="index">起始位置</param>
        /// <returns>子集合</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public subArray<valueType> UnsafeSub(int index)
        {
            return subArray<valueType>.Unsafe(array, index, length - index);
        }
        /// <summary>
        /// 转换成子集合()
        /// </summary>
        /// <param name="index">起始位置</param>
        /// <param name="count">数量,小于0表示所有</param>
        /// <returns>子集合</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public subArray<valueType> UnsafeSub(int index, int count)
        {
            return subArray<valueType>.Unsafe(array, index, count);
        }
        /// <summary>
        /// 长度设为0
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Empty()
        {
            length = 0;
        }
        /// <summary>
        /// 清除所有数据
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Clear()
        {
            if (array.Length != 0)
            {
                if (IsClearArray) Array.Clear(array, 0, array.Length);
                Empty();
            }
        }
        /// <summary>
        /// 释放数据数组
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Null()
        {
            length = 0;
            array = nullValue<valueType>.Array;
        }
        /// <summary>
        /// 设置数据容器长度
        /// </summary>
        /// <param name="count">数据长度</param>
        private void setLength(int count)
        {
            valueType[] newArray = GetNewArray(count);
            Array.Copy(array, 0, newArray, 0, length);
            array = newArray;
        }
        /// <summary>
        /// 增加数据长度
        /// </summary>
        /// <param name="length">数据长度</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void addToLength(int length)
        {
            if (length > array.Length) setLength(length);
        }
        /// <summary>
        /// 增加数据长度
        /// </summary>
        public void AddLength()
        {
            int newLength = length + 1;
            addToLength(newLength);
            length = newLength;
        }
        /// <summary>
        /// 增加数据长度
        /// </summary>
        /// <param name="count">数据长度</param>
        public void AddLength(int count)
        {
            if (count > 0)
            {
                int newLength = length + count;
                addToLength(newLength);
                length = newLength;
            }
            else if ((count += length) >= 0) length = count;
            else log.Error.Throw(log.exceptionType.IndexOutOfRange);
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value">数据</param>
        public void Add(valueType value)
        {
            if (length != 0)
            {
                if (length == array.Length) setLength(array.Length << 1);
            }
            else if (array.Length == 0) array = new valueType[sizeof(int)];
            array[length] = value;
            ++length;
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value">数据</param>
        public void Add(ref valueType value)
        {
            if (length != 0)
            {
                if (length == array.Length) setLength(array.Length << 1);
            }
            else if (array.Length == 0) array = new valueType[sizeof(int)];
            array[length] = value;
            ++length;
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
                addToLength(length + count);
                foreach (valueType value in values) array[length++] = value;
            }
        }
        /// <summary>
        /// 添加数据集合
        /// </summary>
        /// <param name="values">数据集合</param>
        /// <param name="index">起始位置</param>
        /// <param name="count">数量</param>
        public override void Add(valueType[] values, int index, int count)
        {
            array.range range = new array.range(values.length(), index, count);
            if ((count = range.GetCount) != 0)
            {
                int newLength = length + count;
                addToLength(newLength);
                Array.Copy(values, range.SkipCount, array, length, count);
                length = newLength;
            }
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
                    if (length != array.Length)
                    {
                        fastCSharp.unsafer.array.Move(array, index, index + 1, length - index);
                        array[index] = value;
                        ++length;
                    }
                    else
                    {
                        valueType[] values = GetNewArray(array.Length << 1);
                        Array.Copy(array, 0, values, 0, index);
                        values[index] = value;
                        Array.Copy(array, index, values, index + 1, length++ - index);
                        array = values;
                    }
                }
                else Add(value);
            }
            else log.Error.Throw(log.exceptionType.IndexOutOfRange);
        }
        /// <summary>
        /// 插入数据集合
        /// </summary>
        /// <param name="index">插入位置</param>
        /// <param name="values">数据集合</param>
        public void Insert(int index, IEnumerable<valueType> values)
        {
            if (values != null)
            {
                subArray<valueType> newValues = values.getSubArray();
                Insert(index, newValues.array, 0, newValues.length);
            }
        }
        /// <summary>
        /// 插入数据集合
        /// </summary>
        /// <param name="index">插入位置</param>
        /// <param name="values">数据集合</param>
        public void Insert(int index, ICollection<valueType> values)
        {
            if ((uint)index <= (uint)this.length)
            {
                int count = values.count();
                if (count != 0)
                {
                    int length = this.length + count;
                    if (array.Length != 0)
                    {
                        if (length <= array.Length)
                        {
                            fastCSharp.unsafer.array.Move(array, index, index + count, this.length - index);
                            foreach (valueType value in values) array[index++] = value;
                        }
                        else
                        {
                            valueType[] newValues = GetNewArray(length);
                            Array.Copy(array, 0, newValues, 0, index);
                            foreach (valueType value in values) array[index++] = value;
                            Array.Copy(array, index -= count, newValues, index + count, this.length - index);
                            array = newValues;
                        }
                        this.length = length;
                    }
                    else
                    {
                        array = GetNewArray(length);
                        foreach (valueType value in values) array[this.length++] = value;
                    }
                }
            }
            else log.Error.Throw(log.exceptionType.IndexOutOfRange);
        }
        /// <summary>
        /// 插入数据集合
        /// </summary>
        /// <param name="index">插入位置</param>
        /// <param name="values">数据集合</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Insert(int index, valueType[] values)
        {
            if (values != null) Insert(index, values, 0, values.Length);
        }
        /// <summary>
        /// 插入数据集合
        /// </summary>
        /// <param name="index">插入位置</param>
        /// <param name="values">数据集合</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">插入数量</param>
        public void Insert(int index, valueType[] values, int startIndex, int count)
        {
            if ((uint)index <= (uint)this.length)
            {
                array.range range = new array.range(values.length(), startIndex, count);
                if ((count = range.GetCount) != 0)
                {
                    int length = this.length + count;
                    if (array.Length != 0)
                    {
                        if (length <= array.Length)
                        {
                            fastCSharp.unsafer.array.Move(array, index, index + count, this.length - index);
                            Array.Copy(values, range.SkipCount, array, index, count);
                        }
                        else
                        {
                            valueType[] newValues = GetNewArray(length);
                            Array.Copy(array, 0, newValues, 0, index);
                            Array.Copy(values, startIndex, newValues, index, count);
                            Array.Copy(array, index, newValues, index + count, this.length - index);
                            array = newValues;
                        }
                    }
                    else Array.Copy(values, range.SkipCount, array = GetNewArray(length), 0, length);
                    this.length = length;
                }
            }
            else log.Error.Throw(log.exceptionType.IndexOutOfRange);
        }
        /// <summary>
        /// 插入数据集合
        /// </summary>
        /// <param name="index">插入位置</param>
        /// <param name="values">数据集合</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Insert(int index, ref subArray<valueType> values)
        {
            Insert(index, values.array, values.startIndex, values.length);
        }
        /// <summary>
        /// 插入数据集合
        /// </summary>
        /// <param name="index">插入位置</param>
        /// <param name="value">数据集合</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">插入数量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Insert(int index, list<valueType> value, int startIndex, int count)
        {
            if (value != null) Insert(index, value.array, startIndex, count);
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
                Array.Copy(array, 0, values, index, length);
            }
            else log.Error.Throw(log.exceptionType.IndexOutOfRange);
        }
        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="index">数据位置</param>
        /// <returns>被移除数据</returns>
        public override void RemoveAt(int index)
        {
            if ((uint)index < (uint)length)
            {
                fastCSharp.unsafer.array.Move(array, index + 1, index, --length - index);
                array[length] = default(valueType);
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
            if ((uint)index < (uint)length)
            {
                array[index] = array[--length];
                array[length] = default(valueType);
            }
            else log.Error.Throw(log.exceptionType.IndexOutOfRange);
        }
        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="index">数据位置</param>
        /// <returns>被移除数据</returns>
        public override valueType GetRemoveAt(int index)
        {
            if ((uint)index < (uint)length)
            {
                valueType value = array[index];
                fastCSharp.unsafer.array.Move(array, index + 1, index, --length - index);
                return value;
            }
            log.Error.Throw(log.exceptionType.IndexOutOfRange);
            return default(valueType);
        }
        /// <summary>
        /// 移除数据并使用最后一个数据移动到当前位置
        /// </summary>
        /// <param name="index">数据位置</param>
        /// <returns>被移除数据</returns>
        public valueType GetRemoveAtEnd(int index)
        {
            if ((uint)index < (uint)length)
            {
                valueType value = array[index];
                array[index] = array[--length];
                return value;
            }
            log.Error.Throw(log.exceptionType.IndexOutOfRange);
            return default(valueType);
        }
        /// <summary>
        /// 获取匹配数据位置
        /// </summary>
        /// <param name="value">匹配数据</param>
        /// <returns>匹配位置,失败为-1</returns>
        public override int IndexOf(valueType value)
        {
            return length != 0 ? Array.IndexOf<valueType>(array, value, 0, length) : -1;
        }
        /// <summary>
        /// 获取匹配位置
        /// </summary>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>匹配位置,失败为-1</returns>
        public override int IndexOf(Func<valueType, bool> isValue)
        {
            if (isValue == null) log.Error.Throw(log.exceptionType.Null);
            return indexOf(isValue);
        }
        /// <summary>
        /// 获取获取数组中的匹配位置
        /// </summary>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>数组中的匹配位置,失败为-1</returns>
        protected override int indexOf(Func<valueType, bool> isValue)
        {
            if (length != 0)
            {
                int index = 0;
                foreach (valueType value in array)
                {
                    if (isValue(value)) return index;
                    if (++index == length) break;
                }
            }
            return -1;
        }
        /// <summary>
        /// 移除数据范围
        /// </summary>
        /// <param name="index">起始位置</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void RemoveRange(int index)
        {
            if ((uint)index <= (uint)length) length = index;
            else log.Error.Throw(log.exceptionType.IndexOutOfRange);
        }
        /// <summary>
        /// 移除数据范围
        /// </summary>
        /// <param name="index">起始位置</param>
        /// <param name="count">移除数量</param>
        public void RemoveRange(int index, int count)
        {
            if (index + count <= length && index >= 0 && count >= 0)
            {
                fastCSharp.unsafer.array.Move(array, index + count, index, (length -= count) - index);
            }
            else log.Error.Throw(log.exceptionType.IndexOutOfRange);
        }
        /// <summary>
        /// 弹出最后一个数据
        /// </summary>
        /// <returns>最后一个数据</returns>
        public valueType Pop()
        {
            if (length != 0) return array[--length];
            log.Error.Throw(log.exceptionType.IndexOutOfRange);
            return default(valueType);
        }
        /// <summary>
        /// 弹出最后一个数据
        /// </summary>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最后一个数据,失败返回默认空值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType Pop(valueType nullValue)
        {
            return length != 0 ? array[--length] : nullValue;
        }
        /// <summary>
        /// 移除所有后端匹配值
        /// </summary>
        /// <param name="isValue">数据匹配器</param>
        private void removeEnd(Func<valueType, bool> isValue)
        {
            while (--length >= 0 && isValue(array[length])) ;
            ++length;
        }
        /// <summary>
        /// 移除匹配值
        /// </summary>
        /// <param name="isValue">数据匹配器</param>
        public void Remove(Func<valueType, bool> isValue)
        {
            if (isValue == null) log.Error.Throw(log.exceptionType.Null);
            removeEnd(isValue);
            int index = indexOf(isValue);
            if (index != -1)
            {
                for (int start = index; ++start != length; )
                {
                    if (!isValue(array[start])) array[index++] = array[start];
                }
                length = index;
            }
        }
        /// <summary>
        /// 逆转列表
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Reverse()
        {
            if (length != 0) Array.Reverse(array, 0, length);
        }
        /// <summary>
        /// 转换成前端子段集合
        /// </summary>
        /// <param name="count">数量</param>
        /// <returns>子集合</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public list<valueType> Left(int count)
        {
            if (count >= 0)
            {
                if (count < length) length = count;
                return this;
            }
            return null;
        }
        /// <summary>
        /// 转换成子集合(不清除数组)
        /// </summary>
        /// <param name="index">起始位置</param>
        /// <param name="count">数量,小于0表示所有</param>
        /// <returns>子集合</returns>
        public subArray<valueType> Sub(int index, int count)
        {
            array.range range = new array.range(length, index, count < 0 ? length - index : count);
            if (range.GetCount > 0)
            {
                return subArray<valueType>.Unsafe(array, range.SkipCount, range.GetCount);
            }
            return default(subArray<valueType>);
        }
        /// <summary>
        /// 取子集合
        /// </summary>
        /// <param name="index">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>子集合</returns>
        public valueType[] GetSub(int index, int count)
        {
            array.range range = new array.range(length, index, count);
            if (range.GetCount > 0)
            {
                valueType[] values = new valueType[range.GetCount];
                Array.Copy(array, range.SkipCount, values, 0, range.GetCount);
                return values;
            }
            return nullValue<valueType>.Array;
        }
        /// <summary>
        /// 转换成数组子串
        /// </summary>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public subArray<valueType> ToSubArray()
        {
            return subArray<valueType>.Unsafe(array, 0, length);
        }
        /// <summary>
        /// 获取匹配数量
        /// </summary>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>匹配数量</returns>
        public int GetCount(Func<valueType, bool> isValue)
        {
            int count = 0;
            if (length != 0)
            {
                if (isValue == null) log.Error.Throw(log.exceptionType.Null);
                int index = length;
                foreach (valueType value in array)
                {
                    if (isValue(value)) ++count;
                    if (--index == 0) break;
                }
            }
            return count;
        }
        /// <summary>
        /// 获取匹配值集合
        /// </summary>
        /// <param name="isValue">数据匹配器</param>
        /// <param name="map">匹配结果位图</param>
        /// <returns>匹配值集合</returns>
        protected override valueType[] getFindArray(Func<valueType, bool> isValue, fixedMap map)
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
            return nullValue<valueType>.Array;
        }
        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="comparer">比较器</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Sort(Func<valueType, valueType, int> comparer)
        {
            algorithm.quickSort.Sort(array, comparer, 0, length);
        }
        /// <summary>
        /// 获取数据范围
        /// </summary>
        /// <typeparam name="arrayType">目标数据类型</typeparam>
        /// <param name="index">起始位置</param>
        /// <param name="count">数量</param>
        /// <param name="getValue">数据转换器</param>
        /// <returns>目标数据</returns>
        protected override arrayType[] getRange<arrayType>(int index, int count, Func<valueType, arrayType> getValue)
        {
            arrayType[] values = new arrayType[count];
            for (int length = 0, endIndex = index + count; index != endIndex; ++index) values[length++] = getValue(array[index]);
            return values;
        }
        /// <summary>
        /// 获取数据排序范围(不清除数组)
        /// </summary>
        /// <param name="comparer">比较器</param>
        /// <param name="skipCount">跳过记录数</param>
        /// <param name="getCount">获取记录数</param>
        /// <returns>排序数据</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public subArray<valueType> RangeSort(Func<valueType, valueType, int> comparer, int skipCount, int getCount)
        {
            array.range range = new array.range(length, skipCount, getCount);
            return algorithm.quickSort.RangeSort(array, 0, length, comparer, range.SkipCount, range.GetCount);
        }
        /// <summary>
        /// 获取排序数据分页(不清除数组)
        /// </summary>
        /// <param name="comparer">比较器</param>
        /// <param name="pageSize">分页尺寸</param>
        /// <param name="currentPage">页号</param>
        /// <returns>排序数据</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public subArray<valueType> PageSort(Func<valueType, valueType, int> comparer, int pageSize, int currentPage)
        {
            array.page page = new array.page(length, pageSize, currentPage);
            return algorithm.quickSort.RangeSort(array, 0, length, comparer, page.SkipCount, page.CurrentPageSize);
        }
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <param name="comparer">比较器</param>
        /// <param name="value">最大值</param>
        /// <returns>是否存在最大值</returns>
        public override bool Max(Func<valueType, valueType, int> comparer, out valueType value)
        {
            if (comparer == null) log.Error.Throw(log.exceptionType.Null);
            if (length != 0)
            {
                value = array[0];
                for (int index = 1; index != length; ++index)
                {
                    valueType nextValue = array[index];
                    if (comparer(nextValue, value) > 0) value = nextValue;
                }
                return true;
            }
            value = default(valueType);
            return false;
        }
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="getKey">获取键值</param>
        /// <param name="comparer">比较器</param>
        /// <param name="value">最大值</param>
        /// <returns>是否存在最大值</returns>
        public override bool Max<keyType>
            (Func<valueType, keyType> getKey, Func<keyType, keyType, int> comparer, out valueType value)
        {
            if (getKey == null || comparer == null) log.Error.Throw(log.exceptionType.Null);
            if (length != 0)
            {
                value = array[0];
                if (length != 1)
                {
                    keyType key = getKey(value);
                    for (int index = 1; index != length; ++index)
                    {
                        valueType nextValue = array[index];
                        keyType nextKey = getKey(nextValue);
                        if (comparer(nextKey, key) > 0)
                        {
                            value = nextValue;
                            key = nextKey;
                        }
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
        /// <param name="comparer">比较器</param>
        /// <param name="value">最小值</param>
        /// <returns>是否存在最小值</returns>
        public override bool Min(Func<valueType, valueType, int> comparer, out valueType value)
        {
            if (comparer == null) log.Error.Throw(log.exceptionType.Null);
            if (length != 0)
            {
                value = array[0];
                for (int index = 1; index != length; ++index)
                {
                    valueType nextValue = array[index];
                    if (comparer(nextValue, value) < 0) value = nextValue;
                }
                return true;
            }
            value = default(valueType);
            return false;
        }
        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="getKey">获取键值</param>
        /// <param name="comparer">比较器</param>
        /// <param name="value">最小值</param>
        /// <returns>是否存在最小值</returns>
        public override bool Min<keyType>
            (Func<valueType, keyType> getKey, Func<keyType, keyType, int> comparer, out valueType value)
        {
            if (getKey == null || comparer == null) log.Error.Throw(log.exceptionType.Null);
            if (length != 0)
            {
                value = array[0];
                if (length != 1)
                {
                    keyType key = getKey(value);
                    for (int index = 1; index != length; ++index)
                    {
                        valueType nextValue = array[index];
                        keyType nextKey = getKey(nextValue);
                        if (comparer(nextKey, key) < 0)
                        {
                            value = nextValue;
                            key = nextKey;
                        }
                    }
                }
                return true;
            }
            value = default(valueType);
            return false;
        }
        /// <summary>
        /// 数据分组
        /// </summary>
        /// <typeparam name="keyType">键类型</typeparam>
        /// <param name="getKey">键值获取器</param>
        /// <returns>分组数据</returns>
        public Dictionary<keyType, list<valueType>> Group<keyType>(Func<valueType, keyType> getKey)
            where keyType : IEquatable<keyType>
        {
            if (getKey == null) log.Error.Throw(log.exceptionType.Null);
            if (length != 0)
            {
                Dictionary<keyType, list<valueType>> newValues = dictionary<keyType>.Create<list<valueType>>(length);
                list<valueType> list;
                int count = length;
                foreach (valueType value in array)
                {
                    keyType key = getKey(value);
                    if (!newValues.TryGetValue(key, out list)) newValues[key] = list = new list<valueType>();
                    list.Add(value);
                    if (--count == 0) break;
                }
                return newValues;
            }
            return dictionary<keyType>.Create<list<valueType>>();
        }
        /// <summary>
        /// 数据去重
        /// </summary>
        /// <typeparam name="arrayType">目标数组类型</typeparam>
        /// <param name="getValue">数据获取器</param>
        /// <returns>目标集合</returns>
        public subArray<arrayType> Distinct<arrayType>(Func<valueType, arrayType> getValue)
        {
            if (length != 0)
            {
                arrayType[] newValues = new arrayType[length];
                HashSet<valueType> hash = hashSet<valueType>.Create();
                int count = length, index = 0;
                foreach (valueType value in array)
                {
                    if (!hash.Contains(value))
                    {
                        newValues[index++] = getValue(value);
                        hash.Add(value);
                    }
                    if (--count == 0) break;
                }
                return subArray<arrayType>.Unsafe(newValues, 0, index);
            }
            return default(subArray<arrayType>);
        }
        /// <summary>
        /// 转换数据集合
        /// </summary>
        /// <typeparam name="arrayType">目标数据类型</typeparam>
        /// <param name="getValue">数据转换器</param>
        /// <returns>数据集合</returns>
        public override arrayType[] GetArray<arrayType>(Func<valueType, arrayType> getValue)
        {
            if (length != 0)
            {
                if (getValue == null) log.Error.Throw(log.exceptionType.Null);
                arrayType[] values = new arrayType[length];
                int index = 0;
                foreach (valueType value in array)
                {
                    values[index] = getValue(value);
                    if (++index == length) break;
                }
                return values;
            }
            return nullValue<arrayType>.Array;
        }
        /// <summary>
        /// 转换键值对数组
        /// </summary>
        /// <typeparam name="keyType">键类型</typeparam>
        /// <param name="getKey">键值获取器</param>
        /// <returns>键值对数组</returns>
        public override keyValue<keyType, valueType>[] GetKeyValueArray<keyType>(Func<valueType, keyType> getKey)
        {
            if (length != 0)
            {
                if (getKey == null) log.Error.Throw(log.exceptionType.Null);
                keyValue<keyType, valueType>[] values = new keyValue<keyType, valueType>[length];
                int index = 0;
                foreach (valueType value in array)
                {
                    values[index].Set(getKey(value), value);
                    if (++index == length) break;
                }
                return values;
            }
            return nullValue<keyValue<keyType, valueType>>.Array;
        }
        /// <summary>
        /// 转换双向动态数组
        /// </summary>
        /// <returns>双向动态数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public collection<valueType> ToCollection()
        {
            return new collection<valueType>(this);
        }
        /// <summary>
        /// 转换数组
        /// </summary>
        /// <returns>数组</returns>
        private valueType[] getArray()
        {
            valueType[] values = new valueType[length];
            Array.Copy(array, 0, values, 0, length);
            return values;
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
        public valueType[] ToArray()
        {
            if (length != 0)
            {
                return length != array.Length ? getArray() : array;
            }
            return nullValue<valueType>.Array;
        }
    }
    /// <summary>
    /// 单向动态数组扩展
    /// </summary>
    public static partial class list
    {
        /// <summary>
        /// 长度设为0
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static list<valueType> empty<valueType>(this list<valueType> list)
        {
            if (list != null) list.Empty();
            return list;
        }
        /// <summary>
        /// 清除所有数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static list<valueType> clear<valueType>(this list<valueType> list)
        {
            if (list != null) list.Clear();
            return list;
        }
        /// <summary>
        /// 获取第一个值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <returns>第一个值,失败为default(valueType)</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType firstOrDefault<valueType>(this list<valueType> list)
        {
            return list.count() != 0 ? list.UnsafeGet(0) : default(valueType);
        }
        /// <summary>
        /// 获取最后一个值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <returns>最后一个值,失败为default(valueType)</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType lastOrDefault<valueType>(this list<valueType> list)
        {
            return list.count() != 0 ? list.UnsafeGet(list.length - 1) : default(valueType);
        }
        /// <summary>
        /// 根据位置设置数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="index">数据位置</param>
        /// <param name="value">数据</param>
        /// <returns>单向动态数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static list<valueType> set<valueType>(this list<valueType> list, int index, valueType value)
        {
            if (list == null) list = new list<valueType>(index + 1);
            list.Set(index, value);
            return list;
        }
        /// <summary>
        /// 增加数据长度
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <returns>单向动态数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static list<valueType> addLength<valueType>(this list<valueType> list)
        {
            if (list != null)
            {
                list.AddLength();
                return list;
            }
            return new list<valueType>(sizeof(int));
        }
        /// <summary>
        /// 增加数据长度
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="count">数据长度</param>
        /// <returns>单向动态数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static list<valueType> addLength<valueType>(this list<valueType> list, int count)
        {
            if (list != null)
            {
                list.AddLength(count);
                return list;
            }
            return new list<valueType>(count > sizeof(int) ? count : sizeof(int));
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="value">数据</param>
        /// <returns>单向动态数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static list<valueType> add<valueType>(this list<valueType> list, valueType value)
        {
            if (list == null) list = new list<valueType>();
            list.Add(value);
            return list;
        }
        /// <summary>
        /// 添加数据集合
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="values">数据集合</param>
        /// <returns>单向动态数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static list<valueType> add<valueType>(this list<valueType> list, IEnumerable<valueType> values)
        {
            if (list != null)
            {
                list.Add(values.getSubArray());
                return list;
            }
            return values.getSubArray().ToList();
        }
        /// <summary>
        /// 添加数据集合
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="values">数据集合</param>
        /// <returns>单向动态数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static list<valueType> add<valueType>(this list<valueType> list, ICollection<valueType> values)
        {
            if (list != null)
            {
                list.Add(values);
                return list;
            }
            return values.getList();
        }
        /// <summary>
        /// 添加数据集合
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="values">数据集合</param>
        /// <returns>单向动态数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static list<valueType> add<valueType>(this list<valueType> list, valueType[] values)
        {
            if (list != null)
            {
                list.Add(values);
                return list;
            }
            return new list<valueType>(values);
        }
        /// <summary>
        /// 添加数据集合
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="values">数据集合</param>
        /// <param name="index">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>单向动态数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static list<valueType> add<valueType>(this list<valueType> list, valueType[] values, int index, int count)
        {
            if (list != null)
            {
                list.Add(values, index, count);
                return list;
            }
            return new list<valueType>(values, index, count, false);
        }
        /// <summary>
        /// 添加数据集合
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="value">数据集合</param>
        /// <returns>单向动态数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static list<valueType> add<valueType>(this list<valueType> list, list<valueType> value)
        {
            if (list != null)
            {
                list.Add(value);
                return list;
            }
            return new list<valueType>(value);
        }
        /// <summary>
        /// 添加数据集合
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="value">数据集合</param>
        /// <param name="index">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>单向动态数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static list<valueType> add<valueType>(this list<valueType> list, list<valueType> value, int index, int count)
        {
            if (list != null)
            {
                list.Add(value, index, count);
                return list;
            }
            return value.count() != 0 ? new list<valueType>(value.array, index, count, false) : null;
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="index">插入位置</param>
        /// <param name="value">数据</param>
        /// <returns>单向动态数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static list<valueType> insert<valueType>(this list<valueType> list, int index, valueType value)
        {
            if (list != null)
            {
                list.Insert(index, value);
                return list;
            }
            if (index == 0) return list.add(value);
            log.Error.Throw(log.exceptionType.IndexOutOfRange);
            return null;
        }
        /// <summary>
        /// 插入数据集合
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="index">插入位置</param>
        /// <param name="values">数据集合</param>
        /// <returns>单向动态数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static list<valueType> insert<valueType>(this list<valueType> list, int index, IEnumerable<valueType> values)
        {
            if (list != null)
            {
                list.Insert(index, values);
                return list;
            }
            if (index == 0) return values.getSubArray().ToList();
            log.Error.Throw(log.exceptionType.IndexOutOfRange);
            return null;
        }
        /// <summary>
        /// 插入数据集合
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="index">插入位置</param>
        /// <param name="values">数据集合</param>
        /// <returns>单向动态数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static list<valueType> insert<valueType>(this list<valueType> list, int index, ICollection<valueType> values)
        {
            if (list != null)
            {
                list.Insert(index, values);
                return list;
            }
            if (index == 0) return values.getList();
            log.Error.Throw(log.exceptionType.IndexOutOfRange);
            return null;
        }
        /// <summary>
        /// 插入数据集合
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="index">插入位置</param>
        /// <param name="values">数据集合</param>
        /// <returns>单向动态数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static list<valueType> insert<valueType>(this list<valueType> list, int index, valueType[] values)
        {
            return list.insert(index, values, 0, values.Length);
        }
        /// <summary>
        /// 插入数据集合
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="index">插入位置</param>
        /// <param name="values">数据集合</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">插入数量</param>
        /// <returns>单向动态数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static list<valueType> insert<valueType>(this list<valueType> list, int index, valueType[] values, int startIndex, int count)
        {
            if (list != null)
            {
                list.Insert(index, values, startIndex, count);
                return list;
            }
            if (index == 0) return new list<valueType>(values, startIndex, count, false);
            log.Error.Throw(log.exceptionType.IndexOutOfRange);
            return null;
        }
        /// <summary>
        /// 插入数据集合
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="index">插入位置</param>
        /// <param name="value">数据集合</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">插入数量</param>
        /// <returns>单向动态数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static list<valueType> insert<valueType>(this list<valueType> list, int index, list<valueType> value, int startIndex, int count)
        {
            return value.count() != 0 ? list.insert(index, value.array, startIndex, count) : list;
        }
        /// <summary>
        /// 复制数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="values">目标数据</param>
        /// <param name="index">目标位置</param>
        /// <returns>目标数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType[] copyTo<valueType>(this list<valueType> list, valueType[] values, int index)
        {
            if (list != null) list.CopyTo(values, index);
            return values;
        }
        /// <summary>
        /// 移除数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="index">数据位置</param>
        /// <returns>被移除数据</returns>
        /// <returns>单向动态数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static list<valueType> removeAt<valueType>(this list<valueType> list, int index)
        {
            if (list != null)
            {
                list.RemoveAt(index);
                return list;
            }
            log.Error.Throw(log.exceptionType.IndexOutOfRange);
            return null;
        }
        /// <summary>
        /// 移除数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="value">数据</param>
        /// <returns>是否存在移除数据</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static bool removeFirst<valueType>(this list<valueType> list, valueType value)
        {
            return list != null ? list.Remove(value) : false;
        }
        /// <summary>
        /// 替换数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="value">新数据</param>
        /// <param name="isValue">数据匹配器</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static list<valueType> replaceFirst<valueType>(this list<valueType> list, valueType value, Func<valueType, bool> isValue)
        {
            if (list != null) list.ReplaceFirst(value, isValue);
            return list;
        }
        /// <summary>
        /// 获取匹配数据位置
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="value">匹配数据</param>
        /// <returns>匹配位置,失败为-1</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static int indexOf<valueType>(this list<valueType> list, valueType value)
        {
            return list != null ? list.IndexOf(value) : -1;
        }
        /// <summary>
        /// 获取匹配位置
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>匹配位置,失败为-1</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static int indexOf<valueType>(this list<valueType> list, Func<valueType, bool> isValue)
        {
            return list != null ? list.IndexOf(isValue) : -1;
        }
        /// <summary>
        /// 获取第一个匹配值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>匹配值,失败为 default(valueType)</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType firstOrDefault<valueType>(this list<valueType> list, Func<valueType, bool> isValue)
        {
            return list != null ? list.FirstOrDefault(isValue) : default(valueType);
        }
        /// <summary>
        /// 判断是否存在匹配
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>是否存在匹配</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static bool any<valueType>(this list<valueType> list, Func<valueType, bool> isValue)
        {
            return list != null && list.Any(isValue);
        }
        /// <summary>
        /// 判断是否存在数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="value">匹配数据</param>
        /// <returns>是否存在数据</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static bool contains<valueType>(this list<valueType> list, valueType value)
        {
            return list.indexOf(value) != -1;
        }
        /// <summary>
        /// 移除数据范围
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="index">起始位置</param>
        /// <returns>单向动态数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static list<valueType> removeRange<valueType>(this list<valueType> list, int index)
        {
            if (list != null)
            {
                list.RemoveRange(index);
                return list;
            }
            if (index == 0) return null;
            log.Error.Throw(log.exceptionType.IndexOutOfRange);
            return null;
        }
        /// <summary>
        /// 移除数据范围
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="index">起始位置</param>
        /// <param name="count">移除数量</param>
        /// <returns>单向动态数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static list<valueType> removeRange<valueType>(this list<valueType> list, int index, int count)
        {
            if (list != null)
            {
                list.RemoveRange(index, count);
                return list;
            }
            if ((index | count) == 0) return null;
            log.Error.Throw(log.exceptionType.IndexOutOfRange);
            return null;
        }
        /// <summary>
        /// 移除匹配值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>单向动态数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static list<valueType> remove<valueType>(this list<valueType> list, Func<valueType, bool> isValue)
        {
            if (list != null) list.Remove(isValue);
            return list;
        }
        /// <summary>
        /// 逆转单向动态数组
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <returns>单向动态数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static list<valueType> reverse<valueType>(this list<valueType> list)
        {
            if (list != null) list.Reverse();
            return list;
        }
        /// <summary>
        /// 转换成前端子段集合
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="count">数量</param>
        /// <returns>子集合</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static list<valueType> left<valueType>(this list<valueType> list, int count)
        {
            return list != null ? list.Left(count) : null;
        }
        /// <summary>
        /// 转换成子集合(不清除数组)
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="index">起始位置</param>
        /// <returns>子集合</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray<valueType> sub<valueType>(this list<valueType> list, int index)
        {
            return list.sub(index, -1);
        }
        /// <summary>
        /// 转换成子集合(不清除数组)
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="index">起始位置</param>
        /// <param name="count">数量,小于0表示所有</param>
        /// <returns>子集合</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray<valueType> sub<valueType>(this list<valueType> list, int index, int count)
        {
            return list != null ? list.Sub(index, count) : default(subArray<valueType>);
        }
        /// <summary>
        /// 取子集合
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="index">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>子集合</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType[] getSub<valueType>(this list<valueType> list, int index, int count)
        {
            return list != null ? list.GetSub(index, count) : nullValue<valueType>.Array;
        }
        /// <summary>
        /// 转换成数组子串
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray<valueType> toSubArray<valueType>(this list<valueType> list)
        {
            return list != null ? list.ToSubArray() : default(subArray<valueType>);
        }
        /// <summary>
        /// 获取匹配数量
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>匹配数量</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static int count<valueType>(this list<valueType> list, Func<valueType, bool> isValue)
        {
            return list != null ? list.GetCount(isValue) : 0;
        }
        /// <summary>
        /// 获取匹配值集合
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>匹配值集合</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType[] getFindArray<valueType>(this list<valueType> list, Func<valueType, bool> isValue)
        {
            return list != null ? list.GetFindArray(isValue) : nullValue<valueType>.Array;
        }
        /// <summary>
        /// 遍历foreach
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="method">调用函数</param>
        /// <returns>单向动态数组</returns>
        public static list<valueType> each<valueType>(this list<valueType> list, Action<valueType> method)
        {
            if (list.count() != 0)
            {
                if (method == null) log.Error.Throw(log.exceptionType.Null);
                int count = list.length;
                foreach (valueType value in list.array)
                {
                    method(value);
                    if (--count == 0) break;
                }
            }
            return list;
        }
        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="comparer">比较器</param>
        /// <returns>单向动态数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static list<valueType> sort<valueType>(this list<valueType> list, Func<valueType, valueType, int> comparer)
        {
            if (list != null) list.Sort(comparer);
            return list;
        }
        /// <summary>
        /// 获取数据分页
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <typeparam name="arrayType">目标数据类型</typeparam>
        /// <param name="pageSize">分页尺寸</param>
        /// <param name="currentPage">页号</param>
        /// <param name="getValue">数据转换器</param>
        /// <returns>目标数据</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static arrayType[] getPage<valueType, arrayType>
            (this list<valueType> list, int pageSize, int currentPage, Func<valueType, arrayType> getValue)
        {
            return list != null ? list.GetPage(pageSize, currentPage, getValue) : nullValue<arrayType>.Array;
        }
        /// <summary>
        /// 获取数据排序范围(不清除数组)
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="comparer">比较器</param>
        /// <param name="skipCount">跳过记录数</param>
        /// <param name="getCount">获取记录数</param>
        /// <returns>排序数据</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray<valueType> rangeSort<valueType>
            (this list<valueType> list, Func<valueType, valueType, int> comparer, int skipCount, int getCount)
        {
            return list != null ? list.RangeSort(comparer, skipCount, getCount) : default(subArray<valueType>);
        }
        /// <summary>
        /// 获取排序数据分页(不清除数组)
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="comparer">比较器</param>
        /// <param name="pageSize">分页尺寸</param>
        /// <param name="currentPage">页号</param>
        /// <returns>排序数据</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray<valueType> pageSort<valueType>
            (this list<valueType> list, Func<valueType, valueType, int> comparer, int pageSize, int currentPage)
        {
            return list != null ? list.PageSort(comparer, pageSize, currentPage) : default(subArray<valueType>);
        }
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">数组数据</param>
        /// <param name="comparer">比较器</param>
        /// <param name="value">最大值</param>
        /// <returns>是否存在最大值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static bool max<valueType>(this list<valueType> list, Func<valueType, valueType, int> comparer, out valueType value)
        {
            if (list != null) return list.Max(comparer, out value);
            value = default(valueType);
            return false;
        }
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="list">数组数据</param>
        /// <param name="getKey">获取键值</param>
        /// <param name="comparer">比较器</param>
        /// <param name="value">最大值</param>
        /// <returns>是否存在最大值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static bool max<valueType, keyType>
            (this list<valueType> list, Func<valueType, keyType> getKey, Func<keyType, keyType, int> comparer, out valueType value)
        {
            if (list != null) list.Max(getKey, comparer, out value);
            value = default(valueType);
            return false;
        }
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">数组数据</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最大值,失败返回默认空值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType max<valueType>(this list<valueType> list, valueType nullValue)
            where valueType : IComparable<valueType>
        {
            if (list != null)
            {
                valueType value;
                if (list.Max(iComparable<valueType>.CompareToHandle, out value)) return value;
            }
            return nullValue;
        }
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="list">数组数据</param>
        /// <param name="getKey">获取键值</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最大值,失败返回默认空值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType max<valueType, keyType>(this list<valueType> list, Func<valueType, keyType> getKey, valueType nullValue)
            where keyType : IComparable<keyType>
        {
            return list != null ? list.Max(getKey, iComparable<keyType>.CompareToHandle, nullValue) : nullValue;
        }
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="list">数组数据</param>
        /// <param name="getKey">获取键值</param>
        /// <param name="comparer">比较器</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最大值,失败返回默认空值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType max<valueType, keyType>
            (this list<valueType> list, Func<valueType, keyType> getKey, Func<keyType, keyType, int> comparer, valueType nullValue)
        {
            return list != null ? list.Max(getKey, comparer, nullValue) : nullValue;
        }
        /// <summary>
        /// 获取最大键值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="list">数组数据</param>
        /// <param name="getKey">获取键值</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最大键值,失败返回默认空值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static keyType maxKey<valueType, keyType>(this list<valueType> list, Func<valueType, keyType> getKey, keyType nullValue)
            where keyType : IComparable<keyType>
        {
            return list != null ? list.MaxKey(getKey, iComparable<keyType>.CompareToHandle, nullValue) : nullValue;
        }
        /// <summary>
        /// 获取最大键值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="list">数组数据</param>
        /// <param name="getKey">获取键值</param>
        /// <param name="comparer">比较器</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最大键值,失败返回默认空值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static keyType maxKey<valueType, keyType>
            (this list<valueType> list, Func<valueType, keyType> getKey, Func<keyType, keyType, int> comparer, keyType nullValue)
        {
            return list != null ? list.MaxKey(getKey, comparer, nullValue) : nullValue;
        }
        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">数组数据</param>
        /// <param name="comparer">比较器</param>
        /// <param name="value">最小值</param>
        /// <returns>是否存在最小值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static bool min<valueType>(this list<valueType> list, Func<valueType, valueType, int> comparer, out valueType value)
        {
            if (list != null) return list.Min(comparer, out value);
            value = default(valueType);
            return false;
        }
        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="list">数组数据</param>
        /// <param name="getKey">获取键值</param>
        /// <param name="comparer">比较器</param>
        /// <param name="value">最小值</param>
        /// <returns>是否存在最小值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static bool min<valueType, keyType>
            (this list<valueType> list, Func<valueType, keyType> getKey, Func<keyType, keyType, int> comparer, out valueType value)
        {
            if (list != null) list.Min(getKey, comparer, out value);
            value = default(valueType);
            return false;
        }
        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">数组数据</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最小值,失败返回默认空值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType min<valueType>(this list<valueType> list, valueType nullValue)
            where valueType : IComparable<valueType>
        {
            if (list != null)
            {
                valueType value;
                if (list.Min(iComparable<valueType>.CompareToHandle, out value)) return value;
            }
            return nullValue;
        }
        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="list">数组数据</param>
        /// <param name="getKey">获取键值</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最小值,失败返回默认空值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType min<valueType, keyType>(this list<valueType> list, Func<valueType, keyType> getKey, valueType nullValue)
            where keyType : IComparable<keyType>
        {
            return list != null ? list.Min(getKey, iComparable<keyType>.CompareToHandle, nullValue) : nullValue;
        }
        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="list">数组数据</param>
        /// <param name="getKey">获取键值</param>
        /// <param name="comparer">比较器</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最小值,失败返回默认空值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType min<valueType, keyType>
            (this list<valueType> list, Func<valueType, keyType> getKey, Func<keyType, keyType, int> comparer, valueType nullValue)
        {
            return list != null ? list.Min(getKey, comparer, nullValue) : nullValue;
        }
        /// <summary>
        /// 获取最小键值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="list">数组数据</param>
        /// <param name="getKey">获取键值</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最小键值,失败返回默认空值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static keyType minKey<valueType, keyType>(this list<valueType> list, Func<valueType, keyType> getKey, keyType nullValue)
            where keyType : IComparable<keyType>
        {
            return list != null ? list.MinKey(getKey, iComparable<keyType>.CompareToHandle, nullValue) : nullValue;
        }
        /// <summary>
        /// 获取最小键值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="list">数组数据</param>
        /// <param name="getKey">获取键值</param>
        /// <param name="comparer">比较器</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最小键值,失败返回默认空值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static keyType minKey<valueType, keyType>
            (this list<valueType> list, Func<valueType, keyType> getKey, Func<keyType, keyType, int> comparer, keyType nullValue)
        {
            return list != null ? list.MinKey(getKey, comparer, nullValue) : nullValue;
        }
        /// <summary>
        /// 数据分组
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="getKey">键值获取器</param>
        /// <returns>分组数据</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static Dictionary<keyType, list<valueType>> group<valueType, keyType>(this list<valueType> list, Func<valueType, keyType> getKey)
            where keyType : IEquatable<keyType>
        {
            return list != null ? list.Group(getKey) : null;
        }
        /// <summary>
        /// 数据去重
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="arrayType">目标数组类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="getValue">数据获取器</param>
        /// <returns>目标集合</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray<arrayType> distinct<valueType, arrayType>(this list<valueType> list, Func<valueType, arrayType> getValue)
        {
            return list != null ? list.Distinct(getValue) : default(subArray<arrayType>);
        }
        /// <summary>
        /// 转换数据集合
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="arrayType">目标数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="getValue">数据转换器</param>
        /// <returns>数据集合</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static arrayType[] getArray<valueType, arrayType>(this list<valueType> list, Func<valueType, arrayType> getValue)
        {
            return list != null ? list.GetArray(getValue) : nullValue<arrayType>.Array;
        }
        /// <summary>
        /// 转换键值对数组
        /// </summary>
        /// <typeparam name="keyType">键类型</typeparam>
        /// <typeparam name="valueType">值类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="getKey">键值获取器</param>
        /// <returns>键值对数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static keyValue<keyType, valueType>[] getKeyValueArray<keyType, valueType>(this list<valueType> list, Func<valueType, keyType> getKey)
        {
            return list != null ? list.GetKeyValueArray(getKey) : nullValue<keyValue<keyType, valueType>>.Array;
        }
        /// <summary>
        /// 转换双向动态数组
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <returns>双向动态数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static collection<valueType> toCollection<valueType>(this list<valueType> list)
        {
            return list != null ? list.ToCollection() : null;
        }
        /// <summary>
        /// 连接字符串
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="toString">字符串转换器</param>
        /// <returns>字符串</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static string joinString<valueType>(this list<valueType> list, Func<valueType, string> toString)
        {
            return list != null ? list.JoinString(toString) : null;
        }
        /// <summary>
        /// 连接字符串
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="toString">字符串转换器</param>
        /// <param name="join">连接串</param>
        /// <returns>字符串</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static string joinString<valueType>(this list<valueType> list, string join, Func<valueType, string> toString)
        {
            return list != null ? list.JoinString(join, toString) : null;
        }
        /// <summary>
        /// 连接字符串
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <param name="toString">字符串转换器</param>
        /// <param name="join">连接字符</param>
        /// <returns>字符串</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static string joinString<valueType>(this list<valueType> list, char join, Func<valueType, string> toString)
        {
            return list != null ? list.JoinString(join, toString) : null;
        }
        /// <summary>
        /// 转换数组
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <returns>数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType[] getArray<valueType>(this list<valueType> list)
        {
            return list != null ? list.GetArray() : nullValue<valueType>.Array;
        }
        /// <summary>
        /// 转换数组
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="list">单向动态数组</param>
        /// <returns>数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType[] toArray<valueType>(this list<valueType> list)
        {
            return list != null ? list.ToArray() : nullValue<valueType>.Array;
        }
    }
}
