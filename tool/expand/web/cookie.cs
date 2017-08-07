using System;

namespace fastCSharp.web
{
    /// <summary>
    /// COOKIE处理类
    /// </summary>
    public unsafe static class cookie
    {
        /// <summary>
        /// 需要格式化的cookie名称字符集合
        /// </summary>
        public const string FormatCookieNameChars = ",; -\n\r\t";
        /// <summary>
        /// 需要格式化的cookie名称字符位图
        /// </summary>
        private static readonly String.asciiMap formatCookieNameCharMap = new String.asciiMap(FormatCookieNameChars, true);
        /// <summary>
        /// 最大cookie名称长度
        /// </summary>
        public const int MaxCookieNameLength = 256;
        /// <summary>
        /// 格式化Cookie名称
        /// </summary>
        /// <param name="name">Cookie名称</param>
        /// <returns>格式化后Cookie名称</returns>
        public unsafe static string FormatCookieName(string name)
        {
            if (string.IsNullOrEmpty(name)) log.Error.Throw(log.exceptionType.Null);
            if (name.Length > MaxCookieNameLength) log.Error.Throw(null, "cookie名称超过限定 " + ((uint)name.Length).toString(), false);
            fixed (char* nameFixed = name)
            {
                char* endName = nameFixed + name.Length;
                int count = unsafer.String.AsciiCount(nameFixed, endName, formatCookieNameCharMap.Map);
                if (*nameFixed == '$') ++count;
                if (count != 0)
                {
                    string newName = fastCSharp.String.FastAllocateString(count = name.Length + (count << 1));
                    fixed (char* newNameFixed = newName)
                    {
                        char* nextCookieName = newNameFixed;
                        if (*nameFixed == '$')
                        {
                            nextCookieName += 2;
                            *(uint*)newNameFixed = '%' + ('2' << 16);
                            *nextCookieName = '4';
                        }
                        else *nextCookieName = *nameFixed;
                        char* nextName = nameFixed;
                        String.asciiMap formatMap = formatCookieNameCharMap;
                        while (++nextName != endName)
                        {
                            uint nextValue = (uint)*nextName;
                            if (formatMap.Get((int)nextValue))
                            {
                                *++nextCookieName = '%';
                                *++nextCookieName = (char)numberExpand.ToHex(nextValue >> 4);
                                *++nextCookieName = (char)numberExpand.ToHex(nextValue & 15);
                            }
                            else *++nextCookieName = (char)nextValue;
                        }
                        return newName;
                    }
                }
            }
            return name;
        }
        /// <summary>
        /// 格式化Cookie值
        /// </summary>
        /// <param name="value">Cookie值</param>
        /// <returns>格式化后的Cookie值</returns>
        public static string FormatCookieValue(string value)
        {
            //return value == null ? string.Empty : value.Replace(",", "%2C").Replace(";", "%3B");
            return value == null ? string.Empty : value.Replace(";", "%3B");
        }
    }
}
