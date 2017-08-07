using System;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 字符子串
    /// </summary>
    public unsafe struct subString : IEquatable<subString>, IEquatable<string>
    {
        /// <summary>
        /// Trim删除字符位图
        /// </summary>
        private static readonly String.asciiMap trimMap = new String.asciiMap(" \t\r\n", true);
        /// <summary>
        /// 原字符串
        /// </summary>
        internal string value;
        /// <summary>
        /// 原字符串
        /// </summary>
        public string Value
        {
            get { return value; }
        }
        /// <summary>
        /// 原字符串中的起始位置
        /// </summary>
        private int startIndex;
        /// <summary>
        /// 原字符串中的起始位置
        /// </summary>
        public int StartIndex
        {
            get { return startIndex; }
        }
        /// <summary>
        /// 长度
        /// </summary>
        private int length;
        /// <summary>
        /// 长度
        /// </summary>
        public int Length
        {
            get { return length; }
        }
        /// <summary>
        /// 获取字符
        /// </summary>
        /// <param name="index">字符位置</param>
        /// <returns>字符</returns>
        public char this[int index]
        {
            get
            {
                return value[index + startIndex];
            }
        }
        /// <summary>
        /// 字符子串
        /// </summary>
        /// <param name="value">字符串</param>
        public subString(string value)
        {
            this.value = value;
            startIndex = 0;
            length = value.length();
        }
        /// <summary>
        /// 字符子串
        /// </summary>
        /// <param name="value">字符串</param>
        /// <param name="startIndex">起始位置</param>
        public subString(string value, int startIndex)
        {
            if (value == null) fastCSharp.log.Error.Throw(log.exceptionType.Null);
            length = value.Length - (this.startIndex = startIndex);
            if (length < 0 || startIndex < 0) fastCSharp.log.Error.Throw(log.exceptionType.IndexOutOfRange);
            this.value = length != 0 ? value : string.Empty;
        }
        /// <summary>
        /// 字符子串
        /// </summary>
        /// <param name="value">字符串</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="length">长度</param>
        public subString(string value, int startIndex, int length)
        {
            if (value == null) fastCSharp.log.Error.Throw(log.exceptionType.Null);
            array.range range = new array.range(value.Length, startIndex, length);
            if (range.GetCount != length) fastCSharp.log.Error.Throw(log.exceptionType.IndexOutOfRange);
            if (range.GetCount != 0)
            {
                this.value = value;
                this.startIndex = range.SkipCount;
                this.length = range.GetCount;
            }
            else
            {
                this.value = string.Empty;
                this.startIndex = this.length = 0;
            }
        }
        /// <summary>
        /// 字符串隐式转换为子串
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns>字符子串</returns>
        public static implicit operator subString(string value)
        {
            return value != null ? subString.Unsafe(value, 0, value.Length) : default(subString);
        }
        /// <summary>
        /// 字符子串隐式转换为字符串
        /// </summary>
        /// <param name="value">字符子串</param>
        /// <returns>字符串</returns>
        public static implicit operator string(subString value) { return value.ToString(); }
        /// <summary>
        /// 清空数据
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Null()
        {
            value = null;
            startIndex = length = 0;
        }
        /// <summary>
        /// 字符查找
        /// </summary>
        /// <param name="value">查找值</param>
        /// <returns>字符位置,失败返回-1</returns>
        public unsafe int IndexOf(char value)
        {
            if (length != 0)
            {
                fixed (char* valueFixed = this.value)
                {
                    char* start = valueFixed + startIndex, find = unsafer.String.Find(start, start + length, value);
                    if (find != null) return (int)(find - start);
                }
            }
            return -1;
        }
        /// <summary>
        /// 获取子串
        /// </summary>
        /// <param name="startIndex">起始位置</param>
        /// <returns>子串</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public subString Substring(int startIndex)
        {
            return new subString(value, this.startIndex + startIndex, length - startIndex);
        }
        /// <summary>
        /// 获取子串
        /// </summary>
        /// <param name="startIndex">起始位置</param>
        /// <param name="length">长度</param>
        /// <returns>子串</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public subString Substring(int startIndex, int length)
        {
            return new subString(value, this.startIndex + startIndex, length);
        }
        /// <summary>
        /// 删除前后空格
        /// </summary>
        /// <returns>删除前后空格</returns>
        public unsafe subString Trim()
        {
            if (length != 0)
            {
                fixed (char* valueFixed = this.value)
                {
                    char* start = valueFixed + startIndex, end = start + length;
                    start = unsafer.String.findNotAscii(start, end, trimMap.Map);
                    if (start == null) return new subString(string.Empty);
                    end = unsafer.String.findLastNotAscii(start, end, trimMap.Map);
                    if (end == null) return new subString(string.Empty);
                    return Unsafe(value, (int)(start - valueFixed), (int)(end - start));
                }
            }
            return this;
        }
        /// <summary>
        /// 删除前置空格
        /// </summary>
        /// <returns>删除前置空格</returns>
        public unsafe subString TrimStart()
        {
            if (length != 0)
            {
                fixed (char* valueFixed = this.value)
                {
                    char* start = valueFixed + startIndex, end = start + length;
                    start = unsafer.String.findNotAscii(start, end, trimMap.Map);
                    if (start == null) return new subString(string.Empty);
                    return Unsafe(value, (int)(start - valueFixed), (int)(end - start));
                }
            }
            return this;
        }
        /// <summary>
        /// 删除后置空格
        /// </summary>
        /// <returns>删除后置空格</returns>
        public unsafe subString TrimEnd()
        {
            if (length != 0)
            {
                fixed (char* valueFixed = this.value)
                {
                    char* start = valueFixed + startIndex, end = unsafer.String.findLastNotAscii(start, start + length, trimMap.Map);
                    if (end == null) return new subString(string.Empty);
                    return Unsafe(value, (int)(start - valueFixed), (int)(end - start));
                }
            }
            return this;
        }
        /// <summary>
        /// 删除后缀
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public unsafe subString TrimEnd(char value)
        {
            if (length != 0)
            {
                fixed (char* valueFixed = this.value)
                {
                    char* start = valueFixed + startIndex, end = start + length;
                    do
                    {
                        if (*--end != value) return Unsafe(this.value, startIndex, (int)(end - start) + 1);
                    }
                    while (end != start);
                    return new subString(string.Empty);
                }
            }
            return this;
        }
        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="split">分割符</param>
        /// <returns>字符子串集合</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public subArray<subString> Split(char split)
        {
            return String.split(value, startIndex, length, split);
        }
        /// <summary>
        /// 是否以字符串开始
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns>是否以字符串开始</returns>
        public unsafe bool StartsWith(string value)
        {
            if (value == null) fastCSharp.log.Error.Throw(log.exceptionType.Null);
            if (length >= value.Length)
            {
                fixed (char* valueFixed = this.value, cmpFixed = value)
                {
                    return unsafer.memory.Equal(valueFixed + startIndex, cmpFixed, value.Length << 1);
                }
            }
            return false;
        }
        /// <summary>
        /// HASH值
        /// </summary>
        /// <returns>HASH值</returns>
        public unsafe override int GetHashCode()
        {
            if (length == 0) return 0;
            fixed (char* valueFixed = value) return algorithm.hashCode.GetHashCode(valueFixed + startIndex, length << 1);
        }
        /// <summary>
        /// 判断子串是否相等
        /// </summary>
        /// <param name="obj">待比较子串</param>
        /// <returns>子串是否相等</returns>
        public override bool Equals(object obj)
        {
            return Equals((subString)obj);//obj != null & obj.GetType() == typeof(subString) && 
        }
        /// <summary>
        /// 判断子串是否相等
        /// </summary>
        /// <param name="other">待比较子串</param>
        /// <returns>子串是否相等</returns>
        public bool Equals(subString other)
        {
            if (value == null) return other == null;
            if (length == other.length)
            {
                if (value == other.value && startIndex == other.startIndex) return true;
                fixed (char* valueFixed = value, otherFixed = other.value)
                {
                    return unsafer.memory.Equal(valueFixed + startIndex, otherFixed + other.startIndex, length << 1);
                }
            }
            return false;
        }
        /// <summary>
        /// 判断子串是否相等
        /// </summary>
        /// <param name="other">待比较子串</param>
        /// <returns>子串是否相等</returns>
        public bool Equals(string other)
        {
            if (value == null) return other == null;
            if (other != null && length == other.Length)
            {
                if (value == other && startIndex == 0) return true;
                fixed (char* valueFixed = value, otherFixed = other)
                {
                    return unsafer.memory.Equal(valueFixed + startIndex, otherFixed, length << 1);
                }
            }
            return false;
        }
        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <returns>字符串</returns>
        public unsafe override string ToString()
        {
            if (value != null)
            {
                if (startIndex == 0 && length == value.Length) return value;
                fixed (char* valueFixed = value) return new string(valueFixed, startIndex, length);
            }
            return null;
        }
        /// <summary>
        /// 设置数据长度
        /// </summary>
        /// <param name="value">字符串,不能为null</param>
        /// <param name="startIndex">起始位置,必须合法</param>
        /// <param name="length">长度,必须合法</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void UnsafeSet(string value, int startIndex, int length)
        {
            this.value = value;
            this.startIndex = startIndex;
            this.length = length;
        }
        /// <summary>
        /// 设置数据长度
        /// </summary>
        /// <param name="length">长度,必须合法</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void UnsafeSetLength(int length)
        {
            this.length = length;
        }
        /// <summary>
        /// 设置数据长度
        /// </summary>
        /// <param name="startIndex">起始位置,必须合法</param>
        /// <param name="length">长度,必须合法</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void UnsafeSet(int startIndex, int length)
        {
            this.startIndex = startIndex;
            this.length = length;
        }
        /// <summary>
        /// 字符子串(非安全,请自行确保数据可靠性)
        /// </summary>
        /// <param name="value">字符串,不能为null</param>
        /// <param name="startIndex">起始位置,必须合法</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subString Unsafe(string value, int startIndex)
        {
            return new subString { value = value, startIndex = startIndex, length = value.Length - startIndex };
        }
        /// <summary>
        /// 字符子串(非安全,请自行确保数据可靠性)
        /// </summary>
        /// <param name="value">字符串,不能为null</param>
        /// <param name="startIndex">起始位置,必须合法</param>
        /// <param name="length">长度,必须合法</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subString Unsafe(string value, int startIndex, int length)
        {
            return new subString { value = value, startIndex = startIndex, length = length };
        }
        /// <summary>
        /// 字典序比较
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static int CompareOrdinal(ref subString left, ref subString right)
        {
            int length = left.length - right.length, value = string.CompareOrdinal(left.value, left.startIndex, right.value, right.startIndex, length <= 0 ? left.length : right.length);
            return value == 0 ? length : value;
        }
        /// <summary>
        /// 字典序比较
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static int CompareOrdinal(subString left, subString right)
        {
            int length = left.length - right.length, value = string.CompareOrdinal(left.value, left.startIndex, right.value, right.startIndex, length <= 0 ? left.length : right.length);
            return value == 0 ? length : value;
        }
        /// <summary>
        /// 字典序比较
        /// </summary>
        public static readonly Func<subString, subString, int> CompareOrdinalHandle = CompareOrdinal;
        /// <summary>
        /// 获取字符数量
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int Count(char value)
        {
            if (length != 0)
            {
                fixed (char* valueFixed = this.value)
                {
                    char* start = valueFixed + startIndex;
                    return fastCSharp.unsafer.String.Count(start, start + length, value);
                }
            }
            return 0;
        }
    }
}
