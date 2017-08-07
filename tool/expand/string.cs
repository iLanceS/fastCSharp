using System;
using System.Text;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 字符串相关操作
    /// </summary>
    public unsafe static class stringExpand
    {
        /// <summary>
        /// 字符查找
        /// </summary>
        /// <param name="start">起始位置</param>
        /// <param name="end">结束位置</param>
        /// <param name="value">查找值</param>
        /// <returns>字符位置,失败为null</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static char* Find(char* start, char* end, char value)
        {
            return start != null && end > start ? unsafer.String.Find(start, end, value) : null;
        }
        /// <summary>
        /// 字符查找
        /// </summary>
        /// <param name="start">起始位置</param>
        /// <param name="end">结束位置</param>
        /// <param name="valueMap">字符集合</param>
        /// <returns>字符位置,失败为null</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static char* Find(char* start, char* end, fastCSharp.String.asciiMap valueMap)
        {
            return start != null && end > start ? unsafer.String.Find(start, end, valueMap) : null;
        }
        /// <summary>
        /// 查找字符数量
        /// </summary>
        /// <param name="chars">字符串</param>
        /// <param name="value">字符</param>
        /// <returns>字符数量</returns>
        public unsafe static int count(this string chars, char value)
        {
            if (!string.IsNullOrEmpty(chars))
            {
                int count = 0;
                fixed (char* charFixed = chars)
                {
                    for (char* start = charFixed, end = charFixed + chars.Length; start != end; ++start)
                    {
                        if (*start == value) ++count;
                    }
                }
                return count;
            }
            return 0;
        }
        /// <summary>
        /// 分割字符串并返回数值集合(不检查数字合法性)
        /// </summary>
        /// <param name="ints">原字符串</param>
        /// <param name="split">分割符</param>
        /// <returns>数值集合</returns>
        public unsafe static subArray<int> splitIntNoCheck(this string ints, char split)
        {
            subArray<int> values = new subArray<int>();
            if (ints != null && ints.Length != 0)
            {
                fixed (char* intPoint = ints)
                {
                    int intValue = 0;
                    for (char* next = intPoint, end = intPoint + ints.Length; next != end; ++next)
                    {
                        if (*next == split)
                        {
                            values.Add(intValue);
                            intValue = 0;
                        }
                        else
                        {
                            intValue *= 10;
                            intValue += *next;
                            intValue -= '0';
                        }
                    }
                    values.Add(intValue);
                }
            }
            return values;
        }
        /// <summary>
        /// 获取字符串原始字节流
        /// </summary>
        /// <param name="value">字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns>字节流</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static byte[] getBytes(this string value, Encoding encoding)
        {
            return encoding != Encoding.ASCII ? encoding.GetBytes(value) : value.getBytes();
        }
        /// <summary>
        /// 字节流转字符串
        /// </summary>
        /// <param name="data">字节流</param>
        /// <param name="index">起始位置</param>
        /// <param name="length">字节数量</param>
        /// <returns>字符串</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static string deSerialize(this byte[] data, int index, int length)
        {
            return toString(data, index, length, Encoding.ASCII);
        }
        /// <summary>
        /// 字节流转字符串
        /// </summary>
        /// <param name="data">字节流</param>
        /// <param name="index">起始位置</param>
        /// <param name="length">字节数量</param>
        /// <param name="encoding">编码,检测失败为本地编码</param>
        /// <returns>字符串</returns>
        public unsafe static string toString(this byte[] data, int index, int length, Encoding encoding)
        {
            if (encoding != Encoding.ASCII) return encoding.GetString(data, index, length);
            array.range range = new array.range(data.length(), index, length);
            if (range.GetCount == length)
            {
                if (range.GetCount != 0)
                {
                    fixed (byte* dataFixed = data) return fastCSharp.String.UnsafeDeSerialize(dataFixed + range.SkipCount, -range.GetCount);
                }
                return string.Empty;
            }
            log.Error.Throw(log.exceptionType.IndexOutOfRange);
            return null;
        }
        /// <summary>
        /// 大写转小写
        /// </summary>
        /// <param name="value">大写字符串</param>
        /// <returns>小写字节流</returns>
        public unsafe static byte[] getLowerBytes(this string value)
        {
            if (value == null) return null;
            if (value.Length == 0) return nullValue<byte>.Array;
            byte[] newValue = new byte[value.Length];
            fixed (char* valueFixed = value)
            fixed (byte* newValueFixed = newValue)
            {
                unsafer.String.ToLower(valueFixed, valueFixed + value.Length, newValueFixed);
            }
            return newValue;
        }
        /// <summary>
        /// 大写转小写
        /// </summary>
        /// <param name="start">起始位置</param>
        /// <param name="end">结束位置</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static void ToLower(char* start, char* end)
        {
            if (start != null && end > start) unsafer.String.ToLower(start, end);
        }
        /// <summary>
        /// 小写转大写
        /// </summary>
        /// <param name="value">小写字符串</param>
        /// <returns>大写字符串(原引用)</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static string toUpper(this string value)
        {
            if (value != null)
            {
                fixed (char* valueFixed = value) unsafer.String.ToUpper(valueFixed, valueFixed + value.Length);
            }
            return value;
        }
        /// <summary>
        /// 小写转大写
        /// </summary>
        /// <param name="start">起始位置</param>
        /// <param name="end">结束位置</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static void ToUpper(char* start, char* end)
        {
            if (start != null && end > start) unsafer.String.ToUpper(start, end);
        }
        /// <summary>
        /// 比较字符串
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>比较结果,负数left小于right,正数left大于right</returns>
        public static unsafe int cmp(this string left, string right)
        {
            if (left != null && right != null)
            {
                int length = left.Length <= right.Length ? left.Length : right.Length;
                for (int index = 0, endIndex = Math.Min(length, 4); index != endIndex; ++index)
                {
                    int value = left[index] - right[index];
                    if (value != 0) return value;
                }
                if ((length -= 4) > 0)
                {
                    fixed (char* leftFixed = left, rightFixed = right)
                    {
                        int value = Cmp(leftFixed + 4, rightFixed + 4, length);
                        if (value != 0) return value;
                    }
                }
                return left.Length - right.Length;
            }
            if (left == right) return 0;
            return left != null ? 1 : -1;
        }
        /// <summary>
        /// 比较字符串
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="length"></param>
        /// <returns>比较结果,负数left小于right,正数left大于right</returns>
        private static unsafe int Cmp(char* left, char* right, int length)
        {
            while (length >= 8)
            {
                if (((*(uint*)left ^ *(uint*)right) | (*(uint*)(left + 4) ^ *(uint*)(right + 4))
                    | (*(uint*)(left + 8) ^ *(uint*)(right + 8)) | (*(uint*)(left + 12) ^ *(uint*)(right + 12))) != 0)
                {
                    if (((*(uint*)left ^ *(uint*)right) | (*(uint*)(left + 4) ^ *(uint*)(right + 4))) == 0)
                    {
                        left += 8;
                        right += 8;
                    }
                    if (*(uint*)left == *(uint*)right)
                    {
                        left += 4;
                        right += 4;
                    }
                    int value = (int)*(ushort*)left - *(ushort*)right;
                    return value == 0 ? (int)*(ushort*)(left += 2) - *(ushort*)(right += 2) : value;
                }
                length -= 8;
                left += 16;
                right += 16;
            }
            if ((length & 4) != 0)
            {
                if (((*(uint*)left ^ *(uint*)right) | (*(uint*)(left + 4) ^ *(uint*)(right + 4))) != 0)
                {
                    if ((*(uint*)left ^ *(uint*)right) == 0)
                    {
                        left += 4;
                        right += 4;
                    }
                    int value = (int)*(ushort*)left - *(ushort*)right;
                    return value == 0 ? (int)*(ushort*)(left += 2) - *(ushort*)(right += 2) : value;
                }
                left += 8;
                right += 8;
            }
            if ((length & 2) != 0)
            {
                int code = (int)*(ushort*)left - *(ushort*)right;
                if (code != 0) return code;
                code = (int)*(ushort*)(left + 2) - *(ushort*)(right + 2);
                if (code != 0) return code;
                left += 4;
                right += 4;
            }
            return (length & 1) == 0 ? 0 : ((int)*(ushort*)left - *(ushort*)right);
        }
        /// <summary>
        /// 字典顺序比较
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static int OrdinalComparison(string left, string right)
        {
            return string.CompareOrdinal(left, right);
        }
        /// <summary>
        /// 字典顺序比较委托
        /// </summary>
        public static readonly Comparison<string> OrdinalComparisonHandle = OrdinalComparison;
        /// <summary>
        /// 去除前后空格
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns>是否有效数据</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static bool Trim(ref string value)
        {
            return value != null && (value = value.Trim()).Length != 0;
        }
        /// <summary>
        /// 取左侧显示字符串
        /// </summary>
        /// <param name="value">字符串</param>
        /// <param name="count">显示字符串数量</param>
        /// <returns>左侧显示字符串</returns>
        public unsafe static string getLeft(this string value, int count)
        {
            if (count <= 0) return string.Empty;
            if (value.length() << 1 > count)
            {
                fixed (char* valueFixed = value)
                {
                    char* start = valueFixed;
                    for (char* end = valueFixed + value.Length; start != end; ++start)
                    {
                        if (*start > 0xff)
                        {
                            if ((count -= 2) <= 0)
                            {
                                if (count == 0) ++start;
                                break;
                            }
                        }
                        else if (--count == 0)
                        {
                            ++start;
                            break;
                        }
                    }
                    count = (int)(start - valueFixed);
                }
                return count != value.Length ? value.Substring(0, count) : value;
            }
            return value;
        }
        /// <summary>
        /// 全角转半角
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns>转换后的字符串</returns>
        public unsafe static string toHalf(this string value)
        {
            if (value != null)
            {
                fixed (char* valueFixed = value)
                {
                    for (char* start = valueFixed, end = valueFixed + value.Length; start != end; ++start)
                    {
                        int code = *start;
                        if ((uint)(code - 0xff01) <= 0xff5e - 0xff01) *start = (char)(code - 0xfee0);
                        else
                        {
                            switch (code & 7)
                            {
                                case 0x2019 & 7:
                                case 0x2018 & 7:
                                    if ((uint)(code - 0x2018) < 2) *start = '\'';
                                    break;
                                case 0x201c & 7:
                                case 0x201d & 7:
                                    if ((uint)(code - 0x201c) < 2) *start = '"';
                                    break;
                                case 0x3002 & 7:
                                    if (code == 0x3002) *start = '.';
                                    break;
                                case 0xb7 & 7:
                                    if (code == 0xb7) *start = '@';
                                    break;
                                //default:
                                //    if ((uint)(code - 0x2160) < 9) *start = (char)(code - 0x212f);
                                //    break;
                            }
                        }
                    }
                }
            }
            return value;
        }
        /// <summary>
        /// 正则转义字符集合
        /// </summary>
        public const string RegexEscape = @"^[-]{}()\|/?*+.$";
        /// <summary>
        /// 正则转义字符位图
        /// </summary>
        private static readonly fastCSharp.String.asciiMap regexEscapeMap = new fastCSharp.String.asciiMap(RegexEscape, true);
        /// <summary>
        /// 将字符串转换成正则代达式
        /// </summary>
        /// <param name="value">待转换成与正则表达式的字符串</param>
        /// <returns>转换后的正则表达式</returns>
        public unsafe static string toRegex(this string value)
        {
            if (value == null || value.Length == 0)
            {
                fixed (char* valueFixed = value)
                {
                    char* end = valueFixed + value.Length;
                    int count = unsafer.String.AsciiCount(valueFixed, end, regexEscapeMap.Map);
                    if (count != 0)
                    {
                        fixedMap map = new fixedMap(regexEscapeMap.Map);
                        value = fastCSharp.String.FastAllocateString(count += value.Length);
                        fixed (char* writeFixed = value)
                        {
                            for (char* start = valueFixed, write = writeFixed; start != end; *write++ = *start++)
                            {
                                if ((*start & 0xff80) == 0 && map.Get(*start)) *write++ = '\\';
                            }
                        }
                    }
                }
            }
            return value;
        }
        ///// <summary>
        ///// 格式化逗号分隔值
        ///// </summary>
        ///// <param name="value">原始值</param>
        ///// <returns>格式化后的逗号分隔值</returns>
        //public static string toCsv(this string value)
        //{
        //    return value == null ? string.Empty
        //        : (value.IndexOf(',') == -1 ? value : (@"""" + value.Replace(@"""", @"""""") + @""""));
        //}
        /// <summary>
        /// 汉语拼音转英文字母
        /// </summary>
        /// <param name="value">汉语拼音</param>
        /// <returns>英文字母</returns>
        public static unsafe string pinyinToLetter(this string value)
        {
            if (value != null)
            {
                fixed (char* valueFixed = value)
                {
                    for (char* start = valueFixed, end = valueFixed + value.Length, pinyinFixed = pinyins.Char; start != end; ++start)
                    {
                        if ((uint)(*start - 224) <= (476 - 224)) *start = pinyinFixed[*start - 224];
                    }
                }
            }
            return value;
        }
        /// <summary>
        /// 汉语拼音
        /// </summary>
        private static readonly pointer.reference pinyins;
        static stringExpand()
        {
            pinyins = unmanaged.GetStatic((476 - 224 + 1) * sizeof(char), true).Reference;
            char* pinyinData = pinyins.Char;
            pinyinData['ā' - 224] = 'a';
            pinyinData['á' - 224] = 'a';
            pinyinData['ǎ' - 224] = 'a';
            pinyinData['à' - 224] = 'a';
            pinyinData['ē' - 224] = 'e';
            pinyinData['é' - 224] = 'e';
            pinyinData['ě' - 224] = 'e';
            pinyinData['è' - 224] = 'e';
            pinyinData['ī' - 224] = 'i';
            pinyinData['í' - 224] = 'i';
            pinyinData['ǐ' - 224] = 'i';
            pinyinData['ì' - 224] = 'i';
            pinyinData['ō' - 224] = 'o';
            pinyinData['ó' - 224] = 'o';
            pinyinData['ǒ' - 224] = 'o';
            pinyinData['ò' - 224] = 'o';
            pinyinData['ū' - 224] = 'u';
            pinyinData['ú' - 224] = 'u';
            pinyinData['ǔ' - 224] = 'u';
            pinyinData['ù' - 224] = 'u';
            pinyinData['ǘ' - 224] = 'v';
            pinyinData['ǚ' - 224] = 'v';
            pinyinData['ǜ' - 224] = 'v';
        }
    }
}
