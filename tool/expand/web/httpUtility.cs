using System;

namespace fastCSharp.web
{
    /// <summary>
    /// HTTP工具
    /// </summary>
    public static class httpUtility
    {
        /// <summary>
        /// URL解码
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static unsafe string UrlDecode(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                fixed (char* urlFixed = url)
                {
                    char* end = urlFixed + url.Length, errorEnd = end;
                    int length = 0, isSpace = 0;
                    for (char* start = urlFixed; start != end; ++start)
                    {
                        if (*start == '+' || *start == 0) isSpace = 1;
                        else if (*start == '%')
                        {
                            if (++start == end)
                            {
                                --errorEnd;
                                break;
                            }
                            if (*start == 'u')
                            {
                                if ((start += 4) >= end)
                                {
                                    errorEnd = start - 5;
                                    break;
                                }
                                length += 5;
                            }
                            else
                            {
                                if (++start == end)
                                {
                                    errorEnd -= 2;
                                    break;
                                }
                                length += 2;
                            }
                        }
                    }
                    if ((length | isSpace) != 0)
                    {
                        string value = fastCSharp.String.FastAllocateString(url.Length - length);
                        fixed (char* valueFixed = value)
                        {
                            char* write = valueFixed;
                            for (char* start = urlFixed; start != errorEnd; )
                            {
                                if (*start == '+' || *start == 0)
                                {
                                    *write++ = ' ';
                                    ++start;
                                }
                                else if (*start == '%')
                                {
                                    if (*++start == 'u')
                                    {
                                        uint code = (number.ParseHex(*(start + 1)) << 12) | (number.ParseHex(*(start + 2)) << 8) | (number.ParseHex(*(start + 3)) << 4) | number.ParseHex(*(start + 4));
                                        start += 5;
                                        *write++ = code != 0 ? (char)code : ' ';
                                    }
                                    else
                                    {
                                        uint code = (number.ParseHex(*start) << 4) | number.ParseHex(*(start + 1));
                                        start += 2;
                                        *write++ = code != 0 ? (char)code : ' ';

                                    }
                                }
                                else *write++ = *start++;
                            }
                            while (errorEnd != end) *write++ = *errorEnd++;
                        }
                        return value;
                    }
                }
            }
            return url;
        }
        /// <summary>
        /// URL编码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static unsafe string UrlEncode(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                fixedMap map = urlMap;
                fixed (char* valueFixed = value)
                {
                    char* start = valueFixed, end = valueFixed + value.Length;
                    int length = 0, isSpace = 0;
                    do
                    {
                        if (*((byte*)start + 1) == 0)
                        {
                            if (!map.Get(*(byte*)start))
                            {
                                if (*start == ' ') isSpace = 1;
                                else length += 2;
                            }
                        }
                        else length += 5;
                    }
                    while (++start != end);
                    if ((length | isSpace) != 0)
                    {
                        string url = fastCSharp.String.FastAllocateString(value.Length + length);
                        fixed (char* urlFixed = url)
                        {
                            char* write = urlFixed;
                            start = valueFixed;
                            do
                            {
                                if (*((byte*)start + 1) == 0)
                                {
                                    if (map.Get(*(byte*)start)) *write++ = *start;
                                    else if (*start == ' ') *write++ = '+';
                                    else
                                    {
                                        *write = '%';
                                        int code = *((byte*)start) >> 4;
                                        *(write + 1) = (char)(code + (code < 10 ? '0' : ('0' + 'A' - '9' - 1)));
                                        code = *((byte*)start) & 0xf;
                                        *(write + 2) = (char)(code + (code < 10 ? '0' : ('0' + 'A' - '9' - 1)));
                                        write += 3;
                                    }
                                }
                                else
                                {
                                    int code = *((byte*)start + 1) >> 4;
                                    *(int*)write = '%' + ('u' << 16);
                                    *(write + 2) = (char)(code + (code < 10 ? '0' : ('0' + 'A' - '9' - 1)));
                                    code = *((byte*)start + 1) & 0xf;
                                    *(write + 3) = (char)(code + (code < 10 ? '0' : ('0' + 'A' - '9' - 1)));
                                    code = *((byte*)start) >> 4;
                                    *(write + 4) = (char)(code + (code < 10 ? '0' : ('0' + 'A' - '9' - 1)));
                                    code = *((byte*)start) & 0xf;
                                    *(write + 5) = (char)(code + (code < 10 ? '0' : ('0' + 'A' - '9' - 1)));
                                    write += 6;
                                }
                            }
                            while (++start != end);
                        }
                        return url;
                    }
                }
            }
            return value;
        }
        /// <summary>
        /// URL编码字符位图
        /// </summary>
        private static readonly fixedMap urlMap;
        unsafe static httpUtility()
        {
            urlMap = new fixedMap(unmanaged.GetStatic(256 >> 3, true));
            urlMap.Set('0', 10);
            urlMap.Set('A', 26);
            urlMap.Set('a', 26);
            urlMap.Set('(');
            urlMap.Set(')');
            urlMap.Set('*');
            urlMap.Set('-');
            urlMap.Set('.');
            urlMap.Set('!');
            urlMap.Set('_');
        }
    }
}
