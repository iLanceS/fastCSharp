using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace fastCSharp.web
{
    /// <summary>
    /// ajax相关操作
    /// </summary>
    public static class ajax
    {
        /// <summary>
        /// ajax生成串替代字符串,默认输入都必须过滤
        /// </summary>
        public const char Quote = pub.NullChar;
        /// <summary>
        /// ajax生成串替代字符串,默认输入都必须过滤
        /// </summary>
        public const string QuoteString = "\0";
        /// <summary>
        /// 视图类型名称
        /// </summary>
        public const char ViewClientType = '@';
        /// <summary>
        /// 视图类型成员标识
        /// </summary>
        public const char ViewClientMember = '.';
        /// <summary>
        /// ajax时间前缀
        /// </summary>
        public const string DateStart = "new Date(";
        /// <summary>
        /// ajax时间后缀
        /// </summary>
        public const char DateEnd = ')';
        /// <summary>
        /// 第三方ajax时间前缀
        /// </summary>
        public const string OtherDateStart = @"/Date(";
        /// <summary>
        /// ajax数组字符串
        /// </summary>
        public const string Array = "[]";
        /// <summary>
        /// ajax空对象
        /// </summary>
        public const string Object = "{}";
        /// <summary>
        /// ajax空值
        /// </summary>
        public const string Null = "null";
        ///// <summary>
        ///// 客户端格式化ajax串的函数
        ///// </summary>
        //public const string FormatAjax = ".FormatAjax()";
        /// <summary>
        /// 客户端格式化视图数组函数
        /// </summary>
        public const string FormatView = ".FormatView()";
        /// <summary>
        /// 客户端循环引用获取函数名称
        /// </summary>
        public const string SetLoopObject = pub.fastCSharp + ".Pub.SetJsonLoop";
        /// <summary>
        /// 客户端循环引用获取函数名称
        /// </summary>
        public const string GetLoopObject = pub.fastCSharp + ".Pub.GetJsonLoop";
        ///// <summary>
        ///// 客户端视图类型复制函数
        ///// </summary>
        //public static readonly string ViewCopy = pub.fastCSharp + ".Copy";
        /// <summary>
        /// 十六进制前缀"--0x"
        /// </summary>
        private const ulong hexPrefix = '-' + ('-' << 16) + ((long)'0' << 32) + ((long)'x' << 48);
        /// <summary>
        /// 最大值
        /// </summary>
        private const long maxValue = (1L << 52) - 1;
        /// <summary>
        /// 格式化ajax字符串
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="write"></param>
        private unsafe static void formatJavascript(char* start, char* end, char* write)
        {
            byte* bits = fastCSharp.emit.jsonParser.Bits.Byte;
            do
            {
                if (((bits[*(byte*)start] & fastCSharp.emit.jsonParser.EscapeBit) | *(((byte*)start) + 1)) != 0)
                {
                    *write++ = *start == Quote ? '"' : *start;
                }
                else if (*start <= '\r')
                {
                    *(int*)write = *start == '\r' ? '\\' + ('r' << 16) : ('\\' + ('n' << 16));
                    write += 2;
                }
                else
                {
                    *write++ = '\\';
                    *write++ = *start;
                }
            }
            while (++start != end);
        }
        /// <summary>
        /// 格式化ajax字符串
        /// </summary>
        /// <param name="jsStream">js字符流</param>
        /// <returns>格式化后ajax字符串</returns>
        public unsafe static string FormatJavascript(charStream jsStream)
        {
            if (jsStream.Length == 0) return string.Empty;
            int length = formatLength(jsStream);
            string value = fastCSharp.String.FastAllocateString(length + jsStream.Length);
            fixed (char* valueFixed = value)
            {
                char* start = jsStream.Char;
                if (length == 0)
                {
                    char* write = valueFixed, end = start + jsStream.Length;
                    do
                    {
                        *write++ = *start == Quote ? '"' : *start;
                    }
                    while (++start != end);
                }
                else formatJavascript(start, start + jsStream.Length, valueFixed);
            }
            return value;
        }
        /// <summary>
        /// ajax字符串长度
        /// </summary>
        /// <param name="jsStream">js字符流</param>
        /// <returns>ajax字符串长度</returns>
        private unsafe static int formatLength(charStream jsStream)
        {
            byte* bits = fastCSharp.emit.jsonParser.Bits.Byte;
            char* start = jsStream.Char, end = start + jsStream.Length;
            int length = 0;
            do
            {
                if (((bits[*(byte*)start] & fastCSharp.emit.jsonParser.EscapeBit) | *(((byte*)start) + 1)) == 0) ++length;
            }
            while (++start != end);
            return length;
        }
        /// <summary>
        /// 格式化ajax字符串
        /// </summary>
        /// <param name="jsStream">JS字符流</param>
        /// <param name="formatStream">格式化JSON字符流</param>
        internal unsafe static void FormatJavascript(charStream jsStream, unmanagedStream formatStream)
        {
            if (jsStream.Length != 0)
            {
                char* start = jsStream.Char;
                int length = formatLength(jsStream);
                if (length == 0)
                {
                    char* end = start + (length = jsStream.Length);
                    formatStream.PrepLength(length <<= 1);
                    for (char* write = (char*)(formatStream.CurrentData); start != end; ++start) *write++ = *start == Quote ? '"' : *start;
                    formatStream.UnsafeAddLength(length);
                }
                else
                {
                    length += jsStream.Length;
                    formatStream.PrepLength(length <<= 1);
                    formatJavascript(start, start + jsStream.Length, (char*)(formatStream.CurrentData));
                    formatStream.UnsafeAddLength(length);
                }
            }
        }
        /// <summary>
        /// 格式化ajax字符串
        /// </summary>
        /// <param name="jsStream">JS字符流</param>
        /// <param name="formatStream">格式化JSON字符流</param>
        public unsafe static void UnsafeFormatJavascript(charStream jsStream, charStream formatStream)
        {
            if (jsStream.Length != 0)
            {
                char* start = jsStream.Char;
                int length = formatLength(jsStream);
                if (length == 0)
                {
                    char* end = start + (length = jsStream.Length);
                    formatStream.PrepLength(length);
                    for (char* write = formatStream.CurrentChar; start != end; ++start) *write++ = *start == Quote ? '"' : *start;
                    formatStream.UnsafeAddLength(length);
                }
                else
                {
                    formatStream.PrepLength(length += jsStream.Length);
                    formatJavascript(start, start + jsStream.Length, formatStream.CurrentChar);
                    formatStream.UnsafeAddLength(length);
                }
            }
        }
        /// <summary>
        /// 输出空对象
        /// </summary>
        public unsafe static void WriteObject(charStream jsonStream)
        {
            *(int*)jsonStream.GetPrepLengthCurrent(2) = '{' + ('}' << 16);
            jsonStream.UnsafeAddLength(2);
        }
        /// <summary>
        /// 输出空数组
        /// </summary>
        public unsafe static void WriteArray(charStream jsonStream)
        {
            *(int*)jsonStream.GetPrepLengthCurrent(2) = '[' + (']' << 16);
            jsonStream.UnsafeAddLength(2);
        }
        /// <summary>
        /// 输出null值
        /// </summary>
        public unsafe static void WriteNull(charStream jsonStream)
        {
            *(long*)jsonStream.GetPrepLengthCurrent(4) = 'n' + ('u' << 16) + ((long)'l' << 32) + ((long)'l' << 48);
            jsonStream.UnsafeAddLength(4);
        }
        /// <summary>
        /// 输出非数字值
        /// </summary>
        public unsafe static void WriteNaN(charStream jsonStream)
        {
            *(long*)jsonStream.GetPrepLengthCurrent(4) = 'N' + ('a' << 16) + ((long)'N' << 32);
            jsonStream.UnsafeAddLength(3);
        }
        /// <summary>
        /// 数字转换成字符串
        /// </summary>
        /// <param name="value">数字值</param>
        /// <param name="jsonStream">JSON输出流</param>
        /// <param name="isMaxToString">超出最大有效精度是否转换成字符串</param>
        public unsafe static void ToString(long value, charStream jsonStream, bool isMaxToString = true)
        {
            if ((ulong)(value + maxValue) <= (ulong)(maxValue << 1) || !isMaxToString) toString(value, jsonStream);
            else
            {
                jsonStream.PrepLength(24 + 2);
                jsonStream.UnsafeWrite(Quote);
                number.UnsafeToString(value, jsonStream);
                jsonStream.UnsafeWrite(Quote);
            }
        }
        /// <summary>
        /// 数字转换成字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="jsonStream"></param>
        private unsafe static void toString(long value, charStream jsonStream)
        {
            if (value >= 0) toString((ulong)value, jsonStream);
            else
            {
                jsonStream.PrepLength(19);
                jsonStream.UnsafeWrite('-');
                toString((ulong)-value, jsonStream);
            }
        }
        /// <summary>
        /// 数字转换成字符串
        /// </summary>
        /// <param name="value">数字值</param>
        /// <param name="jsonStream">JSON输出流</param>
        /// <param name="isMaxToString">超出最大有效精度是否转换成字符串</param>
        public unsafe static void ToString(ulong value, charStream jsonStream, bool isMaxToString = true)
        {
            if (value <= maxValue || !isMaxToString) toString(value, jsonStream);
            else
            {
                jsonStream.PrepLength(22 + 2);
                jsonStream.UnsafeWrite(Quote);
                number.UnsafeToString(value, jsonStream);
                jsonStream.UnsafeWrite(Quote);
            }
        }
        /// <summary>
        /// 数字转换成字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="jsonStream"></param>
        private unsafe static void toString(ulong value, charStream jsonStream)
        {
            if (value <= uint.MaxValue) ToString((uint)value, jsonStream);
            else
            {
                char* chars = jsonStream.GetPrepLengthCurrent(18), next;
                uint value32 = (uint)(value >> 32);
                *(int*)chars = '0' + ('x' << 16);
                if (value32 >= 0x10000)
                {
                    next = getToHex(value32 >> 16, chars + 2);
                    toHex16(value32 & 0xffff, next);
                    next += 4;
                }
                else next = getToHex(value32, chars + 2);
                toHex16((value32 = (uint)value) >> 16, next);
                toHex16(value32 & 0xffff, next + 4);
                jsonStream.UnsafeAddLength((int)(next - chars) + 8);
            }
        }
        /// <summary>
        /// 数字转换成字符串
        /// </summary>
        /// <param name="value">数字值</param>
        /// <param name="jsonStream">JSON输出流</param>
        public unsafe static void ToString(int value, charStream jsonStream)
        {
            if (value >= 0) ToString((uint)value, jsonStream);
            else
            {
                jsonStream.PrepLength(11);
                jsonStream.UnsafeWrite('-');
                ToString((uint)-value, jsonStream);
            }
        }
        /// <summary>
        /// 数字转换成字符串
        /// </summary>
        /// <param name="value">数字值</param>
        /// <param name="jsonStream">JSON输出流</param>
        public unsafe static void ToString(uint value, charStream jsonStream)
        {
            if (value <= ushort.MaxValue) ToString((ushort)value, jsonStream);
            else
            {
                char* chars = jsonStream.GetPrepLengthCurrent(10);
                *(int*)chars = '0' + ('x' << 16);
                char* next = getToHex(value >> 16, chars + 2);
                toHex16(value & 0xffff, next);
                jsonStream.UnsafeAddLength((int)(next - chars) + 4);
            }
        }
        /// <summary>
        /// 数字转换成16进制字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="chars"></param>
        /// <returns></returns>
        private unsafe static char* getToHex(uint value, char* chars)
        {
            if (value >= 0x100)
            {
                if (value >= 0x1000)
                {
                    toHex16(value, chars);
                    return chars + 4;
                }
                *chars = (char)number.ToHex(value >> 8);
                *(chars + 1) = (char)number.ToHex((value >> 4) & 15);
                *(chars + 2) = (char)number.ToHex(value & 15);
                return chars + 3;
            }
            if (value >= 0x10)
            {
                *chars = (char)number.ToHex(value >> 4);
                *(chars + 1) = (char)number.ToHex(value & 15);
                return chars + 2;
            }
            *chars = (char)number.ToHex(value);
            return chars + 1;
        }
        /// <summary>
        /// 数字转换成字符串
        /// </summary>
        /// <param name="value">数字值</param>
        /// <param name="jsonStream">JSON输出流</param>
        public unsafe static void ToString(short value, charStream jsonStream)
        {
            if (value >= 0) ToString((ushort)value, jsonStream);
            else
            {
                jsonStream.PrepLength(7);
                jsonStream.UnsafeWrite('-');
                ToString((ushort)-value, jsonStream);
            }
        }
        /// <summary>
        /// 数字转换成字符串
        /// </summary>
        /// <param name="value">数字值</param>
        /// <param name="jsonStream">JSON输出流</param>
        public unsafe static void ToString(ushort value, charStream jsonStream)
        {
            char* chars;
            if (value < 10000)
            {
                if (value < 10)
                {
                    jsonStream.Write((char)(value + '0'));
                    return;
                }
                int div10 = (value * (int)fastCSharp.number.Div10_16Mul) >> fastCSharp.number.Div10_16Shift;
                if (div10 < 10)
                {
                    *(chars = jsonStream.GetPrepLengthCurrent(2)) = (char)(div10 + '0');
                    *(chars + 1) = (char)((value - div10 * 10) + '0');
                    jsonStream.UnsafeAddLength(2);
                    return;
                }
                int div100 = (div10 * (int)fastCSharp.number.Div10_16Mul) >> fastCSharp.number.Div10_16Shift;
                if (div100 < 10)
                {
                    *(chars = jsonStream.GetPrepLengthCurrent(3)) = (char)(div100 + '0');
                    *(chars + 1) = (char)((div10 - div100 * 10) + '0');
                    *(chars + 2) = (char)((value - div10 * 10) + '0');
                    jsonStream.UnsafeAddLength(3);
                    return;
                }
                int div1000 = (div100 * (int)fastCSharp.number.Div10_16Mul) >> fastCSharp.number.Div10_16Shift;
                *(chars = jsonStream.GetPrepLengthCurrent(4)) = (char)(div1000 + '0');
                *(chars + 1) = (char)((div100 - div1000 * 10) + '0');
                *(chars + 2) = (char)((div10 - div100 * 10) + '0');
                *(chars + 3) = (char)((value - div10 * 10) + '0');
                jsonStream.UnsafeAddLength(4);
                return;
            }
            *(int*)(chars = jsonStream.GetPrepLengthCurrent(6)) = '0' + ('x' << 16);
            toHex16(value, chars + 2);
            jsonStream.UnsafeAddLength(6);
        }
        /// <summary>
        /// 数字转换成16进制字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="chars"></param>
        private unsafe static void toHex16(uint value, char* chars)
        {
            *chars = (char)number.ToHex(value >> 12);
            *(chars + 1) = (char)number.ToHex((value >> 8) & 15);
            *(chars + 2) = (char)number.ToHex((value >> 4) & 15);
            *(chars + 3) = (char)number.ToHex(value & 15);
        }
        /// <summary>
        /// 数字转换成字符串
        /// </summary>
        /// <param name="value">数字值</param>
        /// <param name="jsonStream">JSON输出流</param>
        public unsafe static void ToString(sbyte value, charStream jsonStream)
        {
            if (value == 0) jsonStream.Write('0');
            else
            {
                if (value < 0)
                {
                    char* chars = jsonStream.GetPrepLengthCurrent(5);
                    uint value32 = (uint)-(int)value;
                    *(int*)chars = '-' + ('0' << 16);
                    *(chars + 2) = 'x';
                    *(chars + 3) = (char)((value32 >> 4) + '0');
                    *(chars + 4) = (char)number.ToHex(value32 & 15);
                    jsonStream.UnsafeAddLength(5);
                }
                else
                {
                    char* chars = jsonStream.GetPrepLengthCurrent(4);
                    uint value32 = (uint)(int)value;
                    *(int*)chars = '0' + ('x' << 16);
                    *(chars + 2) = (char)((value32 >> 4) + '0');
                    *(chars + 3) = (char)number.ToHex(value32 & 15);
                    jsonStream.UnsafeAddLength(4);
                }
            }
        }
        /// <summary>
        /// 数字转换成字符串
        /// </summary>
        /// <param name="value">数字值</param>
        /// <param name="jsonStream">JSON输出流</param>
        public unsafe static void ToString(byte value, charStream jsonStream)
        {
            if (value == 0) jsonStream.Write('0');
            else
            {
                byte* chars = (byte*)jsonStream.GetPrepLengthCurrent(4);
                *(int*)chars = '0' + ('x' << 16);
                *(char*)(chars + sizeof(char) * 2) = (char)number.ToHex((uint)value >> 4);
                *(char*)(chars + sizeof(char) * 3) = (char)number.ToHex((uint)value & 15);
                jsonStream.UnsafeAddLength(4);
            }
        }
        /// <summary>
        /// 数字转换成字符串
        /// </summary>
        /// <param name="value">数字值</param>
        /// <param name="jsonStream">JSON输出流</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static void ToString(bool value, charStream jsonStream)
        {
            jsonStream.Write(value ? '1' : '0');
        }
        /// <summary>
        /// 单精度浮点数转字符串
        /// </summary>
        /// <param name="value">单精度浮点数</param>
        /// <param name="jsonStream">JSON输出流</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static void ToString(float value, charStream jsonStream)
        {
            jsonStream.SimpleWriteNotNull(value.ToString());
        }
        /// <summary>
        /// 双精度浮点数转字符串
        /// </summary>
        /// <param name="value">双精度浮点数</param>
        /// <param name="jsonStream">JSON输出流</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static void ToString(double value, charStream jsonStream)
        {
            jsonStream.SimpleWriteNotNull(value.ToString());
        }
        /// <summary>
        /// 十进制数转字符串
        /// </summary>
        /// <param name="value">十进制数</param>
        /// <param name="jsonStream">JSON输出流</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static void ToString(decimal value, charStream jsonStream)
        {
            jsonStream.SimpleWriteNotNull(value.ToString());
        }
        /// <summary>
        ///  Json转换时间差
        /// </summary>
        public static readonly long JavascriptLocalMinTimeTicks = pub.JavascriptLocalMinTime.Ticks;
        /// <summary>
        /// 时间转字符串
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="jsonStream">JSON输出流</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static unsafe void ToString(DateTime time, charStream jsonStream)
        {
            if (time != DateTime.MinValue) ToStringNotNull(time, jsonStream);
            else WriteNull(jsonStream);
        }
        /// <summary>
        /// 时间转字符串
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="jsonStream">JSON输出流</param>
        public static unsafe void ToStringNotNull(DateTime time, charStream jsonStream)
        {
            jsonStream.PrepLength(DateStart.Length + (19 + 1));
            jsonStream.UnsafeSimpleWrite(DateStart);
            toString((long)(((time.Kind == DateTimeKind.Utc ? time.Ticks + pub.LocalTimeTicks : time.Ticks) - JavascriptLocalMinTimeTicks) / date.MillisecondTicks), jsonStream);
            jsonStream.UnsafeWrite(DateEnd);
        }
        /// <summary>
        /// 时间转字符串 第三方格式 /Date(xxx)/
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="jsonStream">JSON输出流</param>
        public static unsafe void ToStringOther(DateTime time, charStream jsonStream)
        {
            jsonStream.PrepLength(OtherDateStart.Length + (19 + 1 + 4));
            jsonStream.UnsafeWrite(fastCSharp.web.ajax.Quote);
            jsonStream.UnsafeSimpleWrite(OtherDateStart);
            toString((long)(((time.Kind == DateTimeKind.Utc ? time.Ticks + pub.LocalTimeTicks : time.Ticks) - JavascriptLocalMinTimeTicks) / date.MillisecondTicks), jsonStream);
            *(long*)jsonStream.CurrentChar = DateEnd + ('/' << 16) + ((long)fastCSharp.web.ajax.Quote << 32);
            jsonStream.UnsafeAddLength(3);
        }
        /// <summary>
        /// Guid转换成字符串
        /// </summary>
        /// <param name="value">Guid</param>
        /// <param name="jsonStream">JSON输出流</param>
        public unsafe static void ToString(ref Guid value, charStream jsonStream)
        {
            byte* data = (byte*)jsonStream.GetPrepLengthCurrent(38);
            *(char*)data = fastCSharp.web.ajax.Quote;
            new guid { Value = value }.ToString((char*)(data + sizeof(char)));
            *(char*)(data + sizeof(char) * 37) = fastCSharp.web.ajax.Quote;
            jsonStream.UnsafeAddLength(38);
        }
        /// <summary>
        /// 字符
        /// </summary>
        /// <param name="value">字符</param>
        /// <param name="jsonStream">JSON输出流</param>
        public unsafe static void ToString(char value, charStream jsonStream)
        {
            char* chars = jsonStream.GetPrepLengthCurrent(3);
            *chars = Quote;
            *(chars + 1) = value;
            *(chars + 2) = Quote;
            jsonStream.UnsafeAddLength(3);
        }
        /// <summary>
        /// 字符串
        /// </summary>
        /// <param name="value">字符串</param>
        /// <param name="jsonStream">JSON输出流</param>
        public static void ToString(string value, charStream jsonStream)
        {
            jsonStream.PrepLength(value.Length + 2);
            jsonStream.UnsafeWrite(Quote);
            jsonStream.UnsafeWrite(value);
            jsonStream.UnsafeWrite(Quote);
        }
        /// <summary>
        /// 字符串
        /// </summary>
        /// <param name="value">字符串</param>
        /// <param name="jsonStream">JSON输出流</param>
        public static void ToString(ref subString value, charStream jsonStream)
        {
            if (value.value == null) WriteNull(jsonStream);
            else
            {
                jsonStream.PrepLength(value.Length + 2);
                jsonStream.UnsafeWrite(Quote);
                jsonStream.Write(ref value);
                jsonStream.UnsafeWrite(Quote);
            }
        }
        static unsafe ajax()
        {
            byte* bits = fastCSharp.emit.jsonParser.Bits.Byte;
            bits['\n'] &= fastCSharp.emit.jsonParser.EscapeBit ^ 255;
            bits['\r'] &= fastCSharp.emit.jsonParser.EscapeBit ^ 255;
            bits['\\'] &= fastCSharp.emit.jsonParser.EscapeBit ^ 255;
            bits['"'] &= fastCSharp.emit.jsonParser.EscapeBit ^ 255;
        }
    }
}
