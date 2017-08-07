using System;
using System.Timers;
using fastCSharp.threading;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 日期相关操作
    /// </summary>
    public unsafe static class date
    {
        /// <summary>
        /// 每毫秒计时周期数
        /// </summary>
        public static readonly long MillisecondTicks = new TimeSpan(0, 0, 0, 0, 1).Ticks;
        /// <summary>
        /// 每秒计时周期数
        /// </summary>
        public static readonly long SecondTicks = MillisecondTicks * 1000;
        /// <summary>
        /// 每分钟计时周期数
        /// </summary>
        public static readonly long MinutesTicks = SecondTicks * 60;
        /// <summary>
        /// 一天的计时周期数
        /// </summary>
        public static readonly long DayTiks = 24L * 60L * 60L * SecondTicks;
        /// <summary>
        /// 32位除以60转乘法的乘数
        /// </summary>
        public const ulong Div60_32Mul = ((1L << Div60_32Shift) + 59) / 60;
        /// <summary>
        /// 32位除以60转乘法的位移
        /// </summary>
        public const int Div60_32Shift = 21 + 32;
        /// <summary>
        /// 16位除以60转乘法的乘数
        /// </summary>
        public const uint Div60_16Mul = ((1U << Div60_16Shift) + 59) / 60;
        /// <summary>
        /// 16位除以60转乘法的位移
        /// </summary>
        public const int Div60_16Shift = 21;
        /// <summary>
        /// 时间转换成日期字符串(yyyy/MM/dd)
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="split">分隔符</param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static string toDateString(this DateTime time, char split = '/')
        {
            string timeString = fastCSharp.String.FastAllocateString(10);
            fixed (char* timeFixed = timeString) toString(time, timeFixed, split);
            return timeString;
        }
        /// <summary>
        /// 时间转换成日期字符串(yyyy/MM/dd)
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="chars">时间字符串</param>
        /// <param name="split">分隔符</param>
        private unsafe static void toString(DateTime time, char* chars, char split)
        {
            int data0 = time.Year, data1 = (data0 * (int)number.Div10_16Mul) >> number.Div10_16Shift;
            *(chars + 4) = split;
            int data2 = (data1 * (int)number.Div10_16Mul) >> number.Div10_16Shift;
            *(chars + 7) = split;
            int data3 = (data2 * (int)number.Div10_16Mul) >> number.Div10_16Shift;
            *(int*)(chars + 2) = ((data1 - data2 * 10) + ((data0 - data1 * 10) << 16)) + 0x300030;
            *(int*)chars = (data3 + ((data2 - data3 * 10) << 16)) + 0x300030;
            data0 = time.Month;
            data2 = time.Day;
            data1 = (data0 + 6) >> 4;
            data3 = (data2 * (int)number.Div10_16Mul) >> number.Div10_16Shift;
            *(chars + 5) = (char)(data1 + '0');
            *(chars + 6) = (char)((data0 - data1 * 10) + '0');
            *(int*)(chars + 8) = (data3 + ((data2 - data3 * 10) << 16)) + 0x300030;
        }
        /// <summary>
        /// 时间转换成字符串(yyyy/MM/dd HH:mm:ss)
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="dateSplit">日期分隔符</param>
        /// <returns>时间字符串</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static string toString(this DateTime time, char dateSplit = '/')
        {
            string timeString = fastCSharp.String.FastAllocateString(19);
            fixed (char* timeFixed = timeString)
            {
                toString(time, timeFixed, dateSplit);
                *(timeFixed + 10) = ' ';
                toString((int)((time.Ticks % DayTiks) / (1000 * 10000)), timeFixed + 11);
            }
            return timeString;
        }
        /// <summary>
        /// 时间转换成字符串(HH:mm:ss)
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>时间字符串</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static string toTimeString(this DateTime time)
        {
            string timeString = fastCSharp.String.FastAllocateString(8);
            fixed (char* timeFixed = timeString)
            {
                toString((int)((time.Ticks % DayTiks) / (1000 * 10000)), timeFixed);
            }
            return timeString;
        }
        /// <summary>
        /// 时间转换成字符串(HH:mm:ss)
        /// </summary>
        /// <param name="second">当天的计时秒数</param>
        /// <param name="chars">时间字符串</param>
        private unsafe static void toString(int second, char* chars)
        {
            int minute = (int)(((ulong)second * Div60_32Mul) >> Div60_32Shift);
            int hour = (minute * (int)Div60_16Mul) >> Div60_16Shift;
            second -= minute * 60;
            int high = (hour * (int)number.Div10_16Mul) >> number.Div10_16Shift;
            minute -= hour * 60;
            *chars = (char)(high + '0');
            *(chars + 1) = (char)((hour - high * 10) + '0');
            *(chars + 2) = ':';
            high = (minute * (int)number.Div10_16Mul) >> number.Div10_16Shift;
            *(int*)(chars + 3) = (high + ((minute - high * 10) << 16)) + 0x300030;
            *(chars + 5) = ':';
            high = (second * (int)number.Div10_16Mul) >> number.Div10_16Shift;
            *(chars + 6) = (char)(high + '0');
            *(chars + 7) = (char)((second - high * 10) + '0');
        }
        /// <summary>
        /// 时间转换字符串字节长度
        /// </summary>
        public const int SqlMillisecondSize = 23;
        /// <summary>
        /// 时间转换成字符串(精确到毫秒)
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="charStream">字符流</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe static void ToSqlMillisecond(DateTime time, charStream charStream)
        {
            toSqlMillisecond(time, charStream.CurrentChar);
            charStream.UnsafeAddLength(SqlMillisecondSize);
        }
        /// <summary>
        /// 时间转换成字符串(精确到毫秒)
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="chars">时间字符串</param>
        private unsafe static void toSqlMillisecond(DateTime time, char* chars)
        {
            long dayTiks = time.Ticks % DayTiks;
            toString(time, chars, '/');
            long seconds = dayTiks / (1000 * 10000);
            *(chars + 19) = '.';
            *(chars + 10) = ' ';
            toString((int)seconds, chars + 11);
            int data0 = (int)(((ulong)(dayTiks - seconds * (1000 * 10000)) * number.Div10000Mul) >> number.Div10000Shift);
            int data1 = (data0 * (int)number.Div10_16Mul) >> number.Div10_16Shift;
            int data2 = (data1 * (int)number.Div10_16Mul) >> number.Div10_16Shift;
            *(chars + 22) = (char)(data0 - data1 * 10 + '0');
            *(int*)(chars + 20) = (data2 + ((data1 - data2 * 10) << 16)) + 0x300030;
        }
        /// <summary>
        /// 日期转换成整数表示
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns>整数表示</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static int toInt(this DateTime date)
        {
            return date != default(DateTime) ? (date.Year << 9) + (date.Month << 5) + date.Day : 0;
        }
        /// <summary>
        /// 判断整数日期是否匹配
        /// </summary>
        /// <param name="date"></param>
        /// <param name="intDate"></param>
        /// <returns>0表示相等</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static int equal(this DateTime date, int intDate)
        {
            if (date == default(DateTime)) return intDate;
            return date.Day == (intDate & 31) ? ((date.Year << 4) + date.Month) ^ (intDate >> 5) : 1;
        }
        /// <summary>
        /// 当前日期转换成整数表示
        /// </summary>
        /// <returns>整数表示</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static int ToInt()
        {
            DateTime date = nowTime.Now;
            return (date.Year << 9) + (date.Month << 5) + date.Day;
        }
        /// <summary>
        /// 整数表示转换成日期
        /// </summary>
        /// <param name="dateInt">整数表示</param>
        /// <returns>日期</returns>
        public static DateTime GetDate(int dateInt)
        {
            DateTime date = default(DateTime);
            if (dateInt != 0)
            {
                try
                {
                    int year = dateInt >> 9, month = (dateInt >> 5) & 15, day = dateInt & 31;
                    if (year >= 1900 && year < 10000 && month >= 1 && month <= 12 && day >= 1 && day <= 31)
                    {
                        date = new DateTime(year, month, day);
                    }
                }
                catch { }
            }
            return date;
        }
        /// <summary>
        /// 时间转换成时钟周期
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns>时钟周期</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static ulong toKindTicks(this DateTime date)
        {
            return (ulong)date.Ticks + ((ulong)(int)date.Kind << 0x3e);
        }
        /// <summary>
        /// 时钟周期转换时间
        /// </summary>
        /// <param name="value">时钟周期</param>
        /// <returns>时间</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static DateTime FromKindTicks(ulong value)
        {
            return new DateTime((long)(value & 0x3fffffffffffffffL), (DateTimeKind)(int)(value >> 0x3e));
        }
        /// <summary>
        /// 星期
        /// </summary>
        private static readonly pointer.reference weekData;
        /// <summary>
        /// 月份
        /// </summary>
        private static readonly pointer.reference monthData;
        /// <summary>
        /// 时间转字节流长度
        /// </summary>
        internal const int ToByteLength = 29;
        /// <summary>
        /// 时间字节流缓冲区
        /// </summary>
        internal static readonly memoryPool ByteBuffers = memoryPool.GetOrCreate(ToByteLength);
        /// <summary>
        /// 时间转字节流
        /// </summary>
        /// <param name="date">时间</param>
        /// <returns>字节流</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static byte[] universalToBytes(this DateTime date)
        {
            byte[] data = ByteBuffers.Get();
            fixed (byte* fixedData = data) UniversalToBytes(date, fixedData);
            return data;
        }
        /// <summary>
        /// 时间转字节流
        /// </summary>
        /// <param name="date">时间</param>
        /// <returns>字节流</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe static byte[] universalNewBytes(this DateTime date)
        {
            byte[] data = new byte[ToByteLength];
            fixed (byte* fixedData = data) UniversalToBytes(date, fixedData);
            return data;
        }
        /// <summary>
        /// 时间转字节流
        /// </summary>
        /// <param name="date">时间</param>
        /// <param name="data">写入数据起始位置</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe static void ToBytes(DateTime date, byte* data)
        {
            UniversalToBytes(date.toUniversalTime(), data);
        }
        /// <summary>
        /// 时间转字节流
        /// </summary>
        /// <param name="date">时间</param>
        /// <param name="data">写入数据起始位置</param>
        internal unsafe static void UniversalToBytes(DateTime date, byte* data)
        {
            *(int*)data = weekData.Int[(int)date.DayOfWeek];
            int value = date.Day, value10 = (value * (int)number.Div10_16Mul) >> number.Div10_16Shift;
            *(int*)(data + sizeof(int)) = (' ' + (value10 << 8) + ((value - value10 * 10) << 16) + (' ' << 24)) | 0x303000;
            value = date.Year;
            *(int*)(data + sizeof(int) * 2) = monthData.Int[date.Month - 1];
            value10 = (value * (int)number.Div10_16Mul) >> number.Div10_16Shift;
            int value100 = (value10 * (int)number.Div10_16Mul) >> number.Div10_16Shift;
            int value1000 = (value100 * (int)number.Div10_16Mul) >> number.Div10_16Shift;
            *(int*)(data + sizeof(int) * 3) = (value1000 + ((value100 - value1000 * 10) << 8) + ((value10 - value100 * 10) << 16) + ((value - value10 * 10) << 24)) | 0x30303030;

            value100 = (int)(date.Ticks % DayTiks / (1000 * 10000));
            value1000 = (int)(((ulong)value100 * Div60_32Mul) >> Div60_32Shift);
            value100 -= value1000 * 60;
            value = (value1000 * (int)Div60_16Mul) >> Div60_16Shift;
            value1000 -= value * 60;

            value10 = (value * (int)number.Div10_16Mul) >> number.Div10_16Shift;
            *(int*)(data + sizeof(int) * 4) = (' ' + (value10 << 8) + ((value - value10 * 10) << 16) + (':' << 24)) | 0x303000;
            value10 = (value1000 * (int)number.Div10_16Mul) >> number.Div10_16Shift;
            value = (value100 * (int)number.Div10_16Mul) >> number.Div10_16Shift;
            *(int*)(data + sizeof(int) * 5) = (value10 + ((value1000 - value10 * 10) << 8) + (':' << 16) + (value << 24)) | 0x30003030;
            *(int*)(data + sizeof(int) * 6) = ((value100 - value * 10) + '0') + (' ' << 8) + ('G' << 16) + ('M' << 24);
            *(data + sizeof(int) * 7) = (byte)'T';
        }
        ///// <summary>
        ///// 判断时间是否相等
        ///// </summary>
        ///// <param name="date"></param>
        ///// <param name="dataArray"></param>
        ///// <returns></returns>
        //[MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        //internal unsafe static int ByteEquals(DateTime date, subArray<byte> dataArray)
        //{
        //    return UniversalByteEquals(date.toUniversalTime(), dataArray);
        //}
        /// <summary>
        /// 判断时间是否相等
        /// </summary>
        /// <param name="date"></param>
        /// <param name="dataArray"></param>
        /// <returns></returns>
        internal unsafe static int UniversalByteEquals(DateTime date, subArray<byte> dataArray)
        {
            fixed (byte* dataFixed = dataArray.array)
            {
                byte* data = dataFixed + dataArray.startIndex;
                if (((*(int*)data ^ weekData.Int[(int)date.DayOfWeek]) | (*(data + sizeof(int) * 7) ^ (byte)'T')) != 0) return 1;
                int value = date.Day, value10 = (value * (int)number.Div10_16Mul) >> number.Div10_16Shift;
                if (*(int*)(data + sizeof(int)) != ((' ' + (value10 << 8) + ((value - value10 * 10) << 16) + (' ' << 24)) | 0x303000)) return 1;
                value = date.Year;
                if (*(int*)(data + sizeof(int) * 2) != monthData.Int[date.Month - 1]) return 1;
                value10 = (value * (int)number.Div10_16Mul) >> number.Div10_16Shift;
                int value100 = (value10 * (int)number.Div10_16Mul) >> number.Div10_16Shift;
                int value1000 = (value100 * (int)number.Div10_16Mul) >> number.Div10_16Shift;
                if (*(int*)(data + sizeof(int) * 3) != ((value1000 + ((value100 - value1000 * 10) << 8) + ((value10 - value100 * 10) << 16) + ((value - value10 * 10) << 24)) | 0x30303030)) return 1;


                value100 = (int)(date.Ticks % DayTiks / (1000 * 10000));
                value1000 = (int)(((ulong)value100 * Div60_32Mul) >> Div60_32Shift);
                value100 -= value1000 * 60;
                value = (value1000 * (int)Div60_16Mul) >> Div60_16Shift;
                value1000 -= value * 60;

                value10 = (value * (int)number.Div10_16Mul) >> number.Div10_16Shift;
                if (*(int*)(data + sizeof(int) * 4) != ((' ' + (value10 << 8) + ((value - value10 * 10) << 16) + (':' << 24)) | 0x303000)) return 1;
                value10 = (value1000 * (int)number.Div10_16Mul) >> number.Div10_16Shift;
                value = (value100 * (int)number.Div10_16Mul) >> number.Div10_16Shift;
                return (*(int*)(data + sizeof(int) * 5) ^ ((value10 + ((value1000 - value10 * 10) << 8) + (':' << 16) + (value << 24)) | 0x30003030))
                    | (*(int*)(data + sizeof(int) * 6) ^ ((value100 - value * 10) + '0') + (' ' << 8) + ('G' << 16) + ('M' << 24));
            }
        }
        /// <summary>
        /// 复制时间字节流
        /// </summary>
        /// <param name="dateTime">时间字节流</param>
        /// <returns>时间字节流</returns>
        internal static byte[] CopyBytes(byte[] dateTime)
        {
            if (dateTime == null) return null;
            byte[] data = ByteBuffers.Get(dateTime.Length);
            fixed (byte* dataFixed = data) unsafer.memory.Copy(dateTime, dataFixed, dateTime.Length);
            return data;
        }
        /// <summary>
        /// 精确到秒的时间
        /// </summary>
        internal static class nowTime
        {
            /// <summary>
            /// 精确到秒的时间
            /// </summary>
            public static DateTime Now;
            /// <summary>
            /// 精确到秒的时间
            /// </summary>
            public static DateTime UtcNow;
            /// <summary>
            /// 重置时间
            /// </summary>
            /// <returns></returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public static DateTime Set()
            {
                UtcNow = (Now = DateTime.Now).localToUniversalTime();
                return Now;
            }
            /// <summary>
            /// 重置时间
            /// </summary>
            /// <returns></returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public static DateTime SetUtc()
            {
                return UtcNow = (Now = DateTime.Now).localToUniversalTime();
            }
            /// <summary>
            /// 刷新时间的定时器
            /// </summary>
            public readonly static Timer Timer;
            /// <summary>
            /// 刷新时间
            /// </summary>
            private static void refreshTime(object sender, ElapsedEventArgs e)
            {
                UtcNow = (Now = DateTime.Now).localToUniversalTime();
                Timer.Interval = 1000 - Now.Millisecond;
                Timer.Start();
            }
            static nowTime()
            {
                UtcNow = (Now = DateTime.Now).localToUniversalTime();
                Timer = new Timer(1000);
                Timer.Elapsed += refreshTime;
                Timer.AutoReset = false;
                Timer.Start();
            }
        }
        /// <summary>
        /// 精确到秒的时间
        /// </summary>
        public static DateTime NowSecond
        {
            get { return nowTime.Now; }
        }
        /// <summary>
        /// 精确到秒的时间
        /// </summary>
        public static DateTime UtcNowSecond
        {
            get { return nowTime.UtcNow; }
        }
        /// <summary>
        /// DateTime.Now
        /// </summary>
        public static DateTime Now
        {
            get { return nowTime.Set(); }
        }
        /// <summary>
        /// DateTime.Now
        /// </summary>
        public static DateTime UtcNow
        {
            get { return nowTime.SetUtc(); }
        }
        /// <summary>
        /// 时间更新间隔
        /// </summary>
        internal static int NowTimerInterval
        {
            get { return (int)nowTime.Timer.Interval; }
        }
        static date()
        {
            int dataIndex = 0;
            pointer[] datas = unmanaged.GetStatic(false, 7 * sizeof(int), 12 * sizeof(int));
            weekData = datas[dataIndex++].Reference;
            monthData = datas[dataIndex++].Reference;

            int* write = weekData.Int;
            *write = 'S' + ('u' << 8) + ('n' << 16) + (',' << 24);
            *++write = 'M' + ('o' << 8) + ('n' << 16) + (',' << 24);
            *++write = 'T' + ('u' << 8) + ('e' << 16) + (',' << 24);
            *++write = 'W' + ('e' << 8) + ('d' << 16) + (',' << 24);
            *++write = 'T' + ('h' << 8) + ('u' << 16) + (',' << 24);
            *++write = 'F' + ('r' << 8) + ('i' << 16) + (',' << 24);
            *++write = 'S' + ('a' << 8) + ('t' << 16) + (',' << 24);

            write = monthData.Int;
            *write = 'J' + ('a' << 8) + ('n' << 16) + (' ' << 24);
            *++write = 'F' + ('e' << 8) + ('b' << 16) + (' ' << 24);
            *++write = 'M' + ('a' << 8) + ('r' << 16) + (' ' << 24);
            *++write = 'A' + ('p' << 8) + ('r' << 16) + (' ' << 24);
            *++write = 'M' + ('a' << 8) + ('y' << 16) + (' ' << 24);
            *++write = 'J' + ('u' << 8) + ('n' << 16) + (' ' << 24);
            *++write = 'J' + ('u' << 8) + ('l' << 16) + (' ' << 24);
            *++write = 'A' + ('u' << 8) + ('g' << 16) + (' ' << 24);
            *++write = 'S' + ('e' << 8) + ('p' << 16) + (' ' << 24);
            *++write = 'O' + ('c' << 8) + ('t' << 16) + (' ' << 24);
            *++write = 'N' + ('o' << 8) + ('v' << 16) + (' ' << 24);
            *++write = 'D' + ('e' << 8) + ('c' << 16) + (' ' << 24);
        }
    }
}
