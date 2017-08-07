using System;
using fastCSharp;
using System.Text;

namespace fastCSharp.openApi
{
    /// <summary>
    /// 微博编码
    /// </summary>
    public struct microblog
    {
        /// <summary>
        /// 微博编码类型
        /// </summary>
        public enum encoding : byte
        {
            /// <summary>
            /// 汉字两字节，英文一字节
            /// </summary>
            WordByte,
            /// <summary>
            /// 汉字三字节，英文一字节
            /// </summary>
            Utf8
        }
        /// <summary>
        /// 微博编码类型
        /// </summary>
        public encoding Encoding;
        /// <summary>
        /// 微博编码长度
        /// </summary>
        public int EncodingSize;
        /// <summary>
        /// URL编码长度,0表示按字符串计算
        /// </summary>
        public int UrlSize;
        /// <summary>
        /// 计算字符串编码长度
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns>编码长度</returns>
        public int Size(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                switch (Encoding)
                {
                    case microblog.encoding.Utf8:
                        return System.Text.Encoding.UTF8.GetByteCount(value);
                    case microblog.encoding.WordByte:
                        return sizeWordByte(value);
                }
            }
            return 0;
        }
        /// <summary>
        /// 计算字符串编码长度
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns>编码长度</returns>
        private unsafe static int sizeWordByte(string value)
        {
            int count = value.Length;
            fixed (char* valueFixed = value)
            {
                for (char* start = valueFixed, end = valueFixed + count; start != end; ++start)
                {
                    if ((*start & 0xff80) != 0) ++count;
                }
            }
            return count;
        }
        /// <summary>
        /// 截取左侧字符串
        /// </summary>
        /// <param name="value">字符串</param>
        /// <param name="size">截取编码长度</param>
        /// <returns>字符串</returns>
        public string Left(string value, int size)
        {
            if (size > 0 && !string.IsNullOrEmpty(value))
            {
                switch (Encoding)
                {
                    case microblog.encoding.Utf8:
                        return leftUtf8(value, size);
                    case microblog.encoding.WordByte:
                        return leftWordByte(value, size);
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// 截取左侧字符串
        /// </summary>
        /// <param name="value">字符串</param>
        /// <param name="size">截取编码长度</param>
        /// <returns>字符串</returns>
        private unsafe string leftUtf8(string value, int size)
        {
            int length = value.Length;
            if (size < (length << 1) + length)
            {
                int count = size;
                fixed (char* valueFixed = value)
                {
                    for (char* start = valueFixed, end = valueFixed + length; start != end; ++start)
                    {
                        if ((*start & 0xff80) == 0) --count;
                        else count -= 3;
                        if (count < 0)
                        {
                            length = (int)(start - valueFixed);
                            break;
                        }
                    }
                    count = System.Text.Encoding.UTF8.GetByteCount(valueFixed, length) - size;
                    if (count > 0)
                    {
                        char* start = valueFixed + length;
                        do
                        {
                            count -= System.Text.Encoding.UTF8.GetByteCount(--start, 1);
                        }
                        while (count > 0);
                        length = (int)(start - valueFixed);
                    }
                }
            }
            return length != value.Length ? value.Substring(0, length) : value;
        }
        /// <summary>
        /// 截取左侧字符串
        /// </summary>
        /// <param name="value">字符串</param>
        /// <param name="size">截取编码长度</param>
        /// <returns>字符串</returns>
        private unsafe string leftWordByte(string value, int size)
        {
            if (size < (value.Length << 1))
            {
                fixed (char* valueFixed = value)
                {
                    for (char* start = valueFixed, end = valueFixed + value.Length; start != end; ++start)
                    {
                        if ((*start & 0xff80) == 0) --size;
                        else size -= 2;
                        if (size < 0) return value.Substring(0, (int)(start - valueFixed));
                    }
                }
            }
            return value;
        }
    }
}
