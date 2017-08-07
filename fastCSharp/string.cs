using System;
using System.Collections.Generic;
using System.Text;
using fastCSharp.io;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 字符串相关操作
    /// </summary>
    public unsafe static class String
    {
        /// <summary>
        /// 申请字符串空间
        /// </summary>
        public static readonly Func<int, string> FastAllocateString = (Func<int, string>)Delegate.CreateDelegate(typeof(Func<int, string>), typeof(string).GetMethod("FastAllocateString", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(int) }, null));

        #region ASCII位图
        /// <summary>
        /// ASCII位图
        /// </summary>
        public sealed class asciiMap : IDisposable
        {
            /// <summary>
            /// 位图字节长度
            /// </summary>
            public const int mapBytes = 128 >> 3;
            /// <summary>
            /// 位图内存池
            /// </summary>
            fastCSharp.code.memberMap.pool mapPool = fastCSharp.code.memberMap.pool.GetPool(mapBytes);
            /// <summary>
            /// 非安全访问ASCII位图
            /// </summary>
            public fixedMap.unsafer Unsafer
            {
                get { return new fixedMap.unsafer(map); }
            }
            /// <summary>
            /// 位图
            /// </summary>
            private byte* map;
            /// <summary>
            /// 位图
            /// </summary>
            public byte* Map { get { return map; } }
            /// <summary>
            /// 初始化ASCII位图
            /// </summary>
            /// <param name="value">初始值集合</param>
            /// <param name="isUnsafe">初始值是否安全</param>
            public asciiMap(string value, bool isUnsafe = true)
            {
                map = mapPool.GetClear();
                if (isUnsafe && value.Length != 0)
                {
                    fixed (char* valueFixed = value)
                    {
                        for (char* start = valueFixed, end = valueFixed + value.Length; start != end; ++start)
                        {
                            map[*start >> 3] |= (byte)(1 << (*start & 7));
                        }
                    }
                }
                else Set(value);
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                mapPool.Push(map);
                map = null;
            }
            /// <summary>
            /// 设置占位
            /// </summary>
            /// <param name="value">位值</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Set(char value)
            {
                if (map == null) log.Error.Throw(log.exceptionType.IndexOutOfRange);
                if ((value & 0xff80) == 0) map[value >> 3] |= (byte)(1 << (value & 7));
            }
            /// <summary>
            /// 设置占位
            /// </summary>
            /// <param name="value">位值</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Set(byte value)
            {
                if (map == null) log.Error.Throw(log.exceptionType.IndexOutOfRange);
                if ((value & 0x80) == 0) map[value >> 3] |= (byte)(1 << (value & 7));
            }
            /// <summary>
            /// 获取占位状态
            /// </summary>
            /// <param name="value">位值</param>
            /// <returns>是否占位</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public bool Get(char value)
            {
                if (map == null) log.Error.Throw(log.exceptionType.IndexOutOfRange);
                return (value & 0xff80) == 0 && (map[value >> 3] & (byte)(1 << (value & 7))) != 0;
            }
            /// <summary>
            /// 获取占位状态
            /// </summary>
            /// <param name="value">位值</param>
            /// <returns>是否占位</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public bool Get(int value)
            {
                if (map == null) log.Error.Throw(log.exceptionType.IndexOutOfRange);
                return ((uint)value & 0xffffff80) == 0 && (map[value >> 3] & (byte)(1 << (value & 7))) != 0;
            }
            /// <summary>
            /// 设置占位
            /// </summary>
            /// <param name="value">值集合</param>
            public unsafe void Set(string value)
            {
                if (map == null) log.Error.Throw(log.exceptionType.IndexOutOfRange);
                if (value != null)
                {
                    fixed (char* valueFixed = value)
                    {
                        for (char* start = valueFixed, end = valueFixed + value.Length; start != end; ++start)
                        {
                            if ((*start & 0xff80) == 0) map[*start >> 3] |= (byte)(1 << (*start & 7));
                        }
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// 获取字符串长度
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns>null为0</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static int length(this string value)
        {
            return value != null ? value.Length : 0;
        }
        /// <summary>
        /// 连接字符串集合
        /// </summary>
        /// <param name="values">字符串集合</param>
        /// <param name="join">字符连接</param>
        /// <returns>连接后的字符串</returns>
        public unsafe static string joinString(this subArray<string> values, char join)
        {
            if (values.array != null)
            {
                if (values.length > 1)
                {
                    string[] array = values.array;
                    int length = 0;
                    for (int index = values.startIndex, endIndex = index + values.length; index != endIndex; ++index)
                    {
                        string value = array[index];
                        if (value != null) length += value.Length;
                    }
                    string newValue = fastCSharp.String.FastAllocateString(length + values.length - 1);
                    fixed (char* valueFixed = newValue)
                    {
                        char* write = valueFixed;
                        for (int index = values.startIndex, endIndex = index + values.length; index != endIndex; ++index)
                        {
                            string value = array[index];
                            if (value != null)
                            {
                                unsafer.String.Copy(value, write);
                                write += value.Length;
                            }
                            *write++ = join;
                        }
                    }
                    return newValue;
                }
                else if (values.length == 1) return values.array[values.startIndex] ?? string.Empty;
                return string.Empty;
            }
            return null;
        }
        /// <summary>
        /// 连接字符串集合
        /// </summary>
        /// <param name="values">字符串集合</param>
        /// <param name="join">字符连接</param>
        /// <returns>连接后的字符串</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static string joinString(this IEnumerable<string> values, string join)
        {
            return values != null ? string.Join(join, values.getSubArray().ToArray()) : null;
        }
        /// <summary>
        /// 字符替换
        /// </summary>
        /// <param name="value">字符串</param>
        /// <param name="oldChar">原字符</param>
        /// <param name="newChar">目标字符</param>
        /// <returns>字符串</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static string replace(this string value, char oldChar, char newChar)
        {
            return value != null && value.Length != 0 ? unsafer.String.Replace(value, oldChar, newChar) : value;
        }
        /// <summary>
        /// 字符替换
        /// </summary>
        /// <param name="value">字符串,不能为null</param>
        /// <param name="oldChar">原字符</param>
        /// <param name="newChar">目标字符</param>
        /// <returns>字符串</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static string replaceNotNull(this string value, char oldChar, char newChar)
        {
            return value.Length != 0 ? unsafer.String.Replace(value, oldChar, newChar) : value;
        }
        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="value">原字符串</param>
        /// <param name="split">分割符</param>
        /// <returns>字符子串集合</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray<subString> split(this string value, char split)
        {
            return value.split(0, value.length(), split);
        }
        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="value">原字符串</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="length">分割字符串长度</param>
        /// <param name="split">分割符</param>
        /// <returns>字符子串集合</returns>
        public unsafe static subArray<subString> split(this string value, int startIndex, int length, char split)
        {
            array.range range = new array.range(value.length(), startIndex, length);
            if (range.GetCount != length) fastCSharp.log.Error.Throw(log.exceptionType.IndexOutOfRange);
            subArray<subString> values = default(subArray<subString>);
            if (value != null)
            {
                fixed (char* valueFixed = value)
                {
                    char* last = valueFixed + range.SkipCount, end = last + range.GetCount;
                    for (char* start = last; start != end; )
                    {
                        if (*start == split)
                        {
                            values.Add(subString.Unsafe(value, (int)(last - valueFixed), (int)(start - last)));
                            last = ++start;
                        }
                        else ++start;
                    }
                    values.Add(subString.Unsafe(value, (int)(last - valueFixed), (int)(end - last)));
                }
            }
            return values;
        }
        /// <summary>
        /// 获取字符串原始字节流
        /// </summary>
        /// <param name="value">字符串,不能为null</param>
        /// <returns>原始字节流</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static byte[] serializeNotNull(this string value)
        {
            return value.Length != 0 ? unsafer.String.Serialize(value) : nullValue<byte>.Array;
        }
        /// <summary>
        /// 获取Ascii字符串原始字节流
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns>字节流</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static byte[] getBytes(this string value)
        {
            if (value != null)
            {
                fixed (char* valueFixed = value) return unsafer.String.GetBytes(valueFixed, value.Length);
            }
            return null;
        }
        /// <summary>
        /// 根据原始字节流生成字符串
        /// </summary>
        /// <param name="data">原始字节流</param>
        /// <returns>字符串</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static string deSerialize(this byte[] data)
        {
            if (data != null)
            {
                if (data.Length != 0)
                {
                    fixed (byte* dataFixed = data) return UnsafeDeSerialize(dataFixed, -data.Length);
                }
                return string.Empty;
            }
            return null;
        }
        /// <summary>
        /// 根据原始字节流生成字符串
        /// </summary>
        /// <param name="data">原始字节流</param>
        /// <returns>字符串</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static string deSerialize(this subArray<byte> data)
        {
            if (data.array != null)
            {
                if (data.length != 0)
                {
                    fixed (byte* dataFixed = data.array) return UnsafeDeSerialize(dataFixed + data.startIndex, -data.length);
                }
                return string.Empty;
            }
            return null;
        }
        /// <summary>
        /// 根据原始字节流生成字符串
        /// </summary>
        /// <param name="data">原始字节流</param>
        /// <param name="length">字符串长度</param>
        /// <returns>字符串</returns>
        public unsafe static string UnsafeDeSerialize(byte* data, int length)
        {
            if (length >= 0)
            {
                return length != 0 ? new string((char*)data, 0, length >> 1) : string.Empty;
            }
            else
            {
                string value = fastCSharp.String.FastAllocateString(length = -length);
                fixed (char* valueFixed = value)
                {
                    char* start = valueFixed;
                    for (byte* end = data + length; data != end; *start++ = (char)*data++) ;
                }
                return value;
            }
        }
        /// <summary>
        /// 大写转小写
        /// </summary>
        /// <param name="value">大写字符串</param>
        /// <returns>小写字符串(原引用)</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static string toLower(this string value)
        {
            if (value != null)
            {
                fixed (char* valueFixed = value) unsafer.String.ToLower(valueFixed, valueFixed + value.Length);
            }
            return value;
        }
        /// <summary>
        /// 大写转小写
        /// </summary>
        /// <param name="value">大写字符串</param>
        /// <returns>小写字符串(原引用)</returns>
        public unsafe static subString toLower(this subString value)
        {
            if (value.Length != 0)
            {
                fixed (char* valueFixed = value.value)
                {
                    char* start = valueFixed + value.StartIndex;
                    unsafer.String.ToLower(start, start + value.Length);
                }
            }
            return value;
        }
        /// <summary>
        /// 比较字符串(忽略大小写)
        /// </summary>
        /// <param name="left">不能为null</param>
        /// <param name="right">不能为null</param>
        /// <returns>是否相等</returns>
        public unsafe static bool equalCase(this string left, string right)
        {
            if (left != null)
            {
                if (right != null)
                {
                    if (left.Length == right.Length)
                    {
                        fixed (char* leftFixed = left, rightFixed = right) return unsafer.String.EqualCase(leftFixed, rightFixed, left.Length);
                    }
                }
                return false;
            }
            return right == null;
        }
        /// <summary>
        /// 比较字符串(忽略大小写)
        /// </summary>
        /// <param name="left">不能为null</param>
        /// <param name="right">不能为null</param>
        /// <param name="count">字符数量,大于等于0</param>
        /// <returns>是否相等</returns>
        public unsafe static bool equalCase(this string left, string right, int count)
        {
            if (left != null)
            {
                if (right != null)
                {
                    int leftLength = left.Length, rightLength = right.Length;
                    if (leftLength > count) leftLength = count;
                    if (rightLength > count) rightLength = count;
                    if (leftLength == rightLength && count >= 0)
                    {
                        fixed (char* leftFixed = left, rightFixed = right) return unsafer.String.EqualCase(leftFixed, rightFixed, count);
                    }
                }
                return false;
            }
            return right == null;
        }
        /// <summary>
        /// 子串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public unsafe static subString substring(this string value, int startIndex, int length)
        {
            return new subString(value, startIndex, length);
        }
        /// <summary>
        /// 子串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public unsafe static subString substring(this string value, int startIndex)
        {
            return new subString(value, startIndex);
        }

        /// <summary>
        /// base64编码查询表
        /// </summary>
        internal static readonly pointer.reference Base64;
        static String()
        {
            Base64 = unmanaged.GetStatic(64, true).Reference;
            byte* base64 = Base64.Byte;
            foreach (char code in "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/") *base64++ = (byte)code;

        }
    }
}
