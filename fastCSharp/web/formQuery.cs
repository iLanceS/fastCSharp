using System;

namespace fastCSharp.web
{
    /// <summary>
    /// Get或Post查询字符串相关操作
    /// </summary>
    public static class formQuery
    {
        ///// <summary>
        ///// 模拟javascript编码函数escape
        ///// </summary>
        ///// <param name="value">原字符串</param>
        ///// <returns>escape编码后的字符串</returns>
        //public static unsafe string JavascriptEscape(string value)
        //{
        //    if (!string.IsNullOrEmpty(value))
        //    {
        //        fixed (char* valueFixed = value)
        //        {
        //            char* end = valueFixed + value.Length;
        //            int length = 0;
        //            for (char* start = valueFixed; start != end; ++start)
        //            {
        //                if ((*start & 0xff00) == 0)
        //                {
        //                    if ((uint)(*start - '0') >= 10 && (uint)((*start | 0x20) - 'a') >= 26) length += 2;
        //                }
        //                else length += 5;
        //            }
        //            if (length != 0)
        //            {
        //                string newValue = fastCSharp.String.FastAllocateString(length += value.Length);
        //                fixed (char* newValueFixed = newValue)
        //                {
        //                    byte* write = (byte*)newValueFixed;
        //                    for (char* start = valueFixed; start != end; ++start)
        //                    {
        //                        uint charValue = *start;
        //                        if ((charValue & 0xff00) == 0)
        //                        {
        //                            if ((uint)(charValue - '0') < 10 || (uint)((charValue | 0x20) - 'a') < 26)
        //                            {
        //                                *(char*)write = (char)charValue;
        //                                write += sizeof(char);
        //                            }
        //                            else
        //                            {
        //                                uint code = charValue >> 4;
        //                                *(char*)write = '%';
        //                                code += code < 10 ? (uint)'0' : (uint)('0' + 'A' - '9' - 1);
        //                                write += sizeof(char);
        //                                code += (charValue << 16) & 0xf0000;
        //                                *(uint*)write = code + (code < 0xa0000 ? (uint)'0' << 16 : ((uint)('0' + 'A' - '9' - 1) << 16));
        //                                write += sizeof(uint);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            uint code = charValue >> 12;
        //                            *(int*)write = '%' + ('u' << 16);
        //                            code += code < 10 ? (uint)'0' : (uint)('0' + 'A' - '9' - 1);
        //                            write += sizeof(int);
        //                            code += (charValue & 0xf00) << 8;
        //                            *(uint*)write = code + (code < 0xa0000 ? (uint)'0' << 16 : ((uint)('0' + 'A' - '9' - 1) << 16));
        //                            code = (charValue >> 4) & 0xf;
        //                            write += sizeof(uint);
        //                            code += code < 10 ? (uint)'0' : (uint)('0' + 'A' - '9' - 1);
        //                            code += (charValue << 16) & 0xf0000;
        //                            *(uint*)write = code + (code < 0xa0000 ? (uint)'0' << 16 : ((uint)('0' + 'A' - '9' - 1) << 16));
        //                            write += sizeof(uint);
        //                        }
        //                    }
        //                }
        //                return newValue;
        //            }
        //        }
        //    }
        //    return value;
        //}
        ///// <summary>
        ///// 模拟javascript解码函数unescape
        ///// </summary>
        ///// <param name="value">原字符串</param>
        ///// <returns>unescape解码后的字符串</returns>
        //internal static unsafe void JavascriptUnescape(ref subString value)
        //{
        //    if (value.Length != 0)
        //    {
        //        fixed (char* valueFixed = value.value)
        //        {
        //            char* start = valueFixed + value.StartIndex, end = start + value.Length;
        //            while (start != end && *start != '%')
        //            {
        //                if (*start == 0) *start = ' ';
        //                ++start;
        //            }
        //            if (start != end)
        //            {
        //                char* write = start;
        //                NEXT:
        //                if (*++start == 'u')
        //                {
        //                    uint code = (number.ParseHex(*(start + 1)) << 12) | (number.ParseHex(*(start + 2)) << 8) | (number.ParseHex(*(start + 3)) << 4) | number.ParseHex(*(start + 4));
        //                    start += 5;
        //                    *write++ = code != 0 ? (char)code : ' ';
        //                }
        //                else
        //                {
        //                    uint code = (number.ParseHex(*start) << 4) | number.ParseHex(*(start + 1));
        //                    start += 2;
        //                    *write++ = code != 0 ? (char)code : ' ';
        //                }
        //                while (start < end)
        //                {
        //                    if (*start == '%') goto NEXT;
        //                    *write++ = *start == 0 ? ' ' : *start;
        //                    ++start;
        //                }
        //                value.UnsafeSetLength((int)(write - valueFixed) - value.StartIndex);
        //            }
        //        }
        //    }
        //}
        /// <summary>
        /// 模拟javascript解码函数unescape
        /// </summary>
        /// <param name="value">原字符串</param>
        /// <returns>unescape解码后的字符串</returns>
        internal static unsafe subString JavascriptUnescape(ref subArray<byte> value)
        {
            if (value.length != 0)
            {
                string newValue = fastCSharp.String.FastAllocateString(value.length);
                fixed (char* newValueFixed = newValue)
                fixed (byte* valueFixed = value.array)
                {
                    byte* start = valueFixed + value.startIndex, end = start + value.length;
                    char* write = newValueFixed;
                    while (start != end && *start != '%')
                    {
                        *write++ = *start == 0 ? ' ' : (char)*start;
                        ++start;
                    }
                    if (start == end) return subString.Unsafe(newValue, 0, value.length);
                    return subString.Unsafe(newValue, 0, (int)(javascriptUnescape(start, end, write) - newValueFixed));
                }
            }
            return default(subString);
        }
        /// <summary>
        /// 模拟javascript解码函数unescape
        /// </summary>
        /// <param name="value">原字符串</param>
        /// <returns>unescape解码后的字符串</returns>
        internal static unsafe subString JavascriptUnescapeUtf8(ref subArray<byte> value)
        {
            if (value.length != 0)
            {
                fixed (byte* valueFixed = value.array)
                {
                    byte* start = valueFixed + value.startIndex, end = start + value.length, read = start, escape = null;
                    uint escapeCode = 0, unicode = 0;
                    do
                    {
                        if ((*read & 0x80) == 0)
                        {
                            if (*read == '%')
                            {
                                if (escape == null) escape = read;
                                if (*++read == 'u')
                                {
                                    unicode = 1;
                                    break;
                                }
                                uint code = (uint)(*read++ - '0');
                                escapeCode |= code > 9 ? 8 : code;
                            }
                        }
                        else escapeCode = 8;
                    }
                    while (++read < end);
                    if (escape == null) escape = end;
                    if (unicode != 0 || (escapeCode & 8) == 0)
                    {
                        string newValue = fastCSharp.String.FastAllocateString(value.length);
                        fixed (char* newValueFixed = newValue)
                        {
                            char* write = newValueFixed;
                            while (start != escape)
                            {
                                *write++ = *start == 0 ? ' ' : (char)*start;
                                ++start;
                            }
                            if (escape == end) return newValue;
                            return subString.Unsafe(newValue, 0, (int)(javascriptUnescape(start, end, write) - newValueFixed));
                        }
                    }
                    for (read = start; read != escape; ++read)
                    {
                        if (*read == 0) *read = (byte)' ';
                    }
                    if (escape != end)
                    {
                        byte* write = read;
                        NEXT:
                        unicode = (uint)(*++read - '0');
                        escapeCode = (uint)(*++read - '0');
                        if (unicode > 9) unicode = ((unicode - ('A' - '0')) & 0xffdfU) + 10;
                        unicode = (escapeCode > 9 ? (((escapeCode - ('A' - '0')) & 0xffdfU) + 10) : escapeCode) + (unicode << 4);
                        *write++ = unicode == 0 ? (byte)' ' : (byte)unicode;
                        while (++read < end)
                        {
                            if (*read == '%') goto NEXT;
                            if (*read == 0) *write = (byte)' ';
                            ++write;
                        }
                        read = write;
                    }
                    return System.Text.Encoding.UTF8.GetString(value.array, (int)(start - valueFixed), (int)(read - start));
                }
            }
            return default(subString);
        }
        /// <summary>
        /// 模拟javascript解码函数unescape
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="write"></param>
        /// <returns></returns>
        private unsafe static char* javascriptUnescape(byte* start, byte* end, char* write)
        {
            NEXT:
            if (*++start == 'u')
            {
                uint code = (uint)(*++start - '0'), number = (uint)(*++start - '0');
                if (code > 9) code = ((code - ('A' - '0')) & 0xffdfU) + 10;
                if (number > 9) number = ((number - ('A' - '0')) & 0xffdfU) + 10;
                code <<= 12;
                code += (number << 8);
                if ((number = (uint)(*++start - '0')) > 9) number = ((number - ('A' - '0')) & 0xffdfU) + 10;
                code += (number << 4);
                number = (uint)(*++start - '0');
                code += (number > 9 ? (((number - ('A' - '0')) & 0xffdfU) + 10) : number);
                *write++ = code == 0 ? ' ' : (char)code;
            }
            else
            {
                uint code = (uint)(*start - '0'), number = (uint)(*++start - '0');
                if (code > 9) code = ((code - ('A' - '0')) & 0xffdfU) + 10;
                code = (number > 9 ? (((number - ('A' - '0')) & 0xffdfU) + 10) : number) + (code << 4);
                *write++ = code == 0 ? ' ' : (char)code;
            }
            while (++start < end)
            {
                if (*start == '%') goto NEXT;
                *write++ = *start == 0 ? ' ' : (char)*start;
            }
            return write;
        }
    }
}
