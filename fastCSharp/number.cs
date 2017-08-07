using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace fastCSharp
{
    /// <summary>
    /// 数值相关扩展操作
    /// </summary>
    public unsafe static class number
    {
        #region 转字符串(用于代码生成)
        /// <summary>
        /// 16位除以10转乘法的乘数
        /// </summary>
        public const uint Div10_16Mul = ((1 << 19) + 9) / 10;
        /// <summary>
        /// 16位除以10转乘法的位移
        /// </summary>
        public const int Div10_16Shift = 19;
        //public const int Div100_16Mul = ((1 << 22) + 99) / 100;
        //public const int Div100_16Shift = 22;
        /// <summary>
        /// 32位除以10000转乘法的乘数
        /// </summary>
        public const ulong Div10000Mul = ((1L << 45) + 9999) / 10000;
        /// <summary>
        /// 32位除以10000转乘法的位移
        /// </summary>
        public const int Div10000Shift = 45;
        /// <summary>
        /// 32位除以100000000转乘法的乘数
        /// </summary>
        public const ulong Div100000000Mul = ((1L << 58) + 99999999) / 100000000;
        /// <summary>
        /// 32位除以100000000转乘法的位移
        /// </summary>
        public const int Div100000000Shift = 58;
        /// <summary>
        /// 获取除法转乘法的乘数与位移
        /// </summary>
        /// <param name="value">除数</param>
        /// <returns>乘数与位移</returns>
        public static keyValue<uint, int> divShift(ushort value)
        {
            uint divMul = uint.MaxValue / value;
            int shift = divMul.bits() - 16;
            divMul >>= shift;
            shift ^= 31;
            return new keyValue<uint, int>(++divMul, ++shift);
        }
        /// <summary>
        /// 获取除法转乘法的乘数与位移
        /// </summary>
        /// <param name="value">除数</param>
        /// <returns>乘数与位移</returns>
        public static keyValue<ulong, int> divShift(uint value)
        {
            ulong divMul = ulong.MaxValue / value;
            int shift = ((uint)(divMul >> 32)).bits();
            divMul >>= shift;
            shift ^= 63;
            return new keyValue<ulong, int>(++divMul, ++shift);
        }
        /// <summary>
        /// 数值转字符串
        /// </summary>
        /// <param name="value">数值</param>
        /// <returns>字符串</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static string toString(this byte value)
        {
            long chars;
            return new string((char*)&chars, 0, toString(value, (char*)&chars));
        }
        /// <summary>
        /// 数值转字符串
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="charStream">字符流</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe static void ToString(byte value, charStream charStream)
        {
            charStream.PrepLength(3);
            charStream.UnsafeAddLength(toString(value, charStream.CurrentChar));
        }
        /// <summary>
        /// 数值转字符串
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="chars">字符串</param>
        /// <returns>字符串长度</returns>
        private unsafe static int toString(byte value, char* chars)
        {
            if (value >= 100)
            {
                int value10 = (value * (int)Div10_16Mul) >> Div10_16Shift;
                chars[2] = (char)((value - value10 * 10) + '0');
                int value100 = (value10 * (int)Div10_16Mul) >> Div10_16Shift;
                *(int*)chars = ((value10 - value100 * 10) << 16) | value100 | 0x300030;
                return 3;
            }
            if (value >= 10)
            {
                int value10 = (value * (int)Div10_16Mul) >> Div10_16Shift;
                *(int*)chars = ((value - value10 * 10) << 16) | value10 | 0x300030;
                return 2;
            }
            *chars = (char)(value + '0');
            return 1;
        }
        /// <summary>
        /// 数值转字符串
        /// </summary>
        /// <param name="value">数值</param>
        /// <returns>字符串</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static string toString(this sbyte value)
        {
            long chars;
            return new string((char*)&chars, 0, toString(value, (char*)&chars));
        }
        /// <summary>
        /// 数值转字符串
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="charStream">字符流</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe static void ToString(sbyte value, charStream charStream)
        {
            charStream.PrepLength(4);
            charStream.UnsafeAddLength(toString(value, charStream.CurrentChar));
        }
        /// <summary>
        /// 数值转字符串
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="chars">字符串</param>
        /// <returns>字符串长度</returns>
        private unsafe static int toString(sbyte value, char* chars)
        {
            if (value >= 0)
            {
                if (value >= 100)
                {
                    value -= 100;
                    *chars = '1';
                    int value10 = (value * (int)Div10_16Mul) >> Div10_16Shift;
                    *(int*)(chars + 1) = ((value - value10 * 10) << 16) | value10 | 0x300030;
                    return 3;
                }
                else if (value >= 10)
                {
                    int value10 = (value * (int)Div10_16Mul) >> Div10_16Shift;
                    *(int*)chars = ((value - value10 * 10) << 16) | value10 | 0x300030;
                    return 2;
                }
                *chars = (char)(value + '0');
                return 1;
            }
            int value32 = -value;
            if (value32 >= 100)
            {
                value32 -= 100;
                *(int*)chars = '-' + ('1' << 16);
                int value10 = (value32 * (int)Div10_16Mul) >> Div10_16Shift;
                *(int*)(chars + 2) = ((value32 - value10 * 10) << 16) | value10 | 0x300030;
                return 4;
            }
            if (value32 >= 10)
            {
                *chars = '-';
                int value10 = (value32 * (int)Div10_16Mul) >> Div10_16Shift;
                *(int*)(chars + 1) = ((value32 - value10 * 10) << 16) | value10 | 0x300030;
                return 3;
            }
            *(int*)chars = '-' + ((value32 + '0') << 16);
            return 2;
        }
        /// <summary>
        /// 数值转字符串
        /// </summary>
        /// <param name="value">数值</param>
        /// <returns>字符串</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static string toString(this ushort value)
        {
            char* chars = stackalloc char[5];
            return new string(chars, 0, toString(value, chars));
        }
        /// <summary>
        /// 数值转字符串
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="charStream">字符流</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe static void ToString(ushort value, charStream charStream)
        {
            charStream.PrepLength(5);
            charStream.UnsafeAddLength(toString(value, charStream.CurrentChar));
        }
        /// <summary>
        /// 数值转字符串
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="chars">字符串</param>
        /// <returns>字符串长度</returns>
        private unsafe static int toString(ushort value, char* chars)
        {
            if (value >= 10000)
            {
                int value10 = (int)((uint)(value * Div10_16Mul) >> Div10_16Shift);
                int value100 = (value10 * (int)Div10_16Mul) >> Div10_16Shift;
                *(int*)(chars + 3) = ((value - value10 * 10) << 16) | (value10 - value100 * 10) | 0x300030;
                value10 = (value100 * (int)Div10_16Mul) >> Div10_16Shift;
                value = (ushort)((value10 * Div10_16Mul) >> Div10_16Shift);
                *(int*)(chars + 1) = ((value100 - value10 * 10) << 16) | (value10 - value * 10) | 0x300030;
                *chars = (char)(value + '0');
                return 5;
            }
            if (value >= 100)
            {
                int value10 = (value * (int)Div10_16Mul) >> Div10_16Shift;
                if (value >= 1000)
                {
                    int value100 = (value10 * (int)Div10_16Mul) >> Div10_16Shift;
                    *(int*)(chars + 2) = ((value - value10 * 10) << 16) | (value10 - value100 * 10) | 0x300030;
                    value10 = (value100 * (int)Div10_16Mul) >> Div10_16Shift;
                    *(int*)chars = ((value100 - value10 * 10) << 16) | value10 | 0x300030;
                    return 4;
                }
                else
                {
                    chars[2] = (char)((value - value10 * 10) + '0');
                    int value100 = (value10 * (int)Div10_16Mul) >> Div10_16Shift;
                    *(int*)chars = ((value10 - value100 * 10) << 16) | value100 | 0x300030;
                    return 3;
                }
            }
            if (value >= 10)
            {
                int value10 = (value * (int)Div10_16Mul) >> Div10_16Shift;
                *(int*)chars = ((value - value10 * 10) << 16) | value10 | 0x300030;
                return 2;
            }
            *chars = (char)(value + '0');
            return 1;
        }
        /// <summary>
        /// 数值转字符串
        /// </summary>
        /// <param name="value">数值</param>
        /// <returns>字符串</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static string toString(this short value)
        {
            char* chars = stackalloc char[6];
            return new string(chars, 0, toString(value, chars));
        }
        /// <summary>
        /// 数值转字符串
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="charStream">字符流</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe static void ToString(short value, charStream charStream)
        {
            charStream.PrepLength(6);
            charStream.UnsafeAddLength(toString(value, charStream.CurrentChar));
        }
        /// <summary>
        /// 数值转字符串
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="chars">字符串</param>
        /// <returns>字符串长度</returns>
        private unsafe static int toString(short value, char* chars)
        {
            if (value >= 0) return toString((ushort)value, chars);
            int value32 = -value;
            if (value32 >= 10000)
            {
                int value10 = (int)((uint)(value32 * Div10_16Mul) >> Div10_16Shift);
                int value100 = (value10 * (int)Div10_16Mul) >> Div10_16Shift;
                *(int*)(chars + 4) = ((value32 - value10 * 10) << 16) | (value10 - value100 * 10) | 0x300030;
                value10 = (value100 * (int)Div10_16Mul) >> Div10_16Shift;
                value32 = (value10 * (int)Div10_16Mul) >> Div10_16Shift;
                *(int*)(chars + 2) = ((value100 - value10 * 10) << 16) | (value10 - value32 * 10) | 0x300030;
                *(int*)chars = '-' + ((value32 + '0') << 16);
                return 6;
            }
            if (value32 >= 100)
            {
                if (value32 >= 1000)
                {
                    *chars = '-';
                    int value10 = (value32 * (int)Div10_16Mul) >> Div10_16Shift;
                    int value100 = (value10 * (int)Div10_16Mul) >> Div10_16Shift;
                    *(int*)(chars + 3) = ((value32 - value10 * 10) << 16) | (value10 - value100 * 10) | 0x300030;
                    value10 = (value100 * (int)Div10_16Mul) >> Div10_16Shift;
                    *(int*)(chars + 1) = ((value100 - value10 * 10) << 16) | value10 | 0x300030;
                    return 5;
                }
                else
                {
                    int value10 = (value32 * (int)Div10_16Mul) >> Div10_16Shift;
                    int value100 = (value10 * (int)Div10_16Mul) >> Div10_16Shift;
                    *(int*)(chars + 2) = ((value32 - value10 * 10) << 16) | (value10 - value100 * 10) | 0x300030;
                    *(int*)chars = '-' + ((value100 + '0') << 16);
                    return 4;
                }
            }
            if (value32 >= 10)
            {
                *chars = '-';
                int value10 = (value32 * (int)Div10_16Mul) >> Div10_16Shift;
                *(int*)(chars + 1) = ((value32 - value10 * 10) << 16) | value10 | 0x300030;
                return 3;
            }
            *(int*)chars = '-' + ((value32 + '0') << 16);
            return 2;
        }
        /// <summary>
        /// 数值转字符串
        /// </summary>
        /// <param name="value">数值</param>
        /// <returns>字符串</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static string toString(this uint value)
        {
            char* chars = stackalloc char[10];
            keyValue<int, int> index = toString(value, chars);
            return new string(chars + index.Key, 0, index.Value);
        }
        /// <summary>
        /// 数值转字符串
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="charStream">字符流</param>
        internal unsafe static void ToString(uint value, charStream charStream)
        {
            char* chars = charStream.GetPrepLengthCurrent(10 + 3);
            keyValue<int, int> index = toString(value, chars);
            charStream.UnsafeSimpleWrite(chars + index.Key, index.Value);
        }
        /// <summary>
        /// 数值转字符串
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="chars">字符串</param>
        /// <returns>起始位置+字符串长度</returns>
        private unsafe static keyValue<int, int> toString(this uint value, char* chars)
        {
            if (value >= 100000000)
            {
                uint value100000000 = (uint)((value * (ulong)Div100000000Mul) >> Div100000000Shift);
                value -= value100000000 * 100000000;
                uint value10000 = (uint)((value * Div10000Mul) >> Div10000Shift);
                uIntToString(value10000, chars + 2);
                uIntToString(value - value10000 * 10000, chars + 6);
                if (value100000000 >= 10)
                {
                    value10000 = (value100000000 * Div10_16Mul) >> Div10_16Shift;
                    *(uint*)chars = ((value100000000 - value10000 * 10) << 16) | value10000 | 0x300030U;
                    return new keyValue<int, int>(0, 10);
                }
                *(chars + 1) = (char)(value100000000 + '0');
                return new keyValue<int, int>(1, 9);
            }
            return new keyValue<int, int>(0, toString99999999(value, chars));
        }
        /// <summary>
        /// 小于100000000的正整数转字符串
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="chars">字符串</param>
        /// <returns>字符串长度</returns>
        private unsafe static int toString99999999(uint value, char* chars)
        {
            if (value >= 10000)
            {
                uint value10000 = (uint)((value * Div10000Mul) >> Div10000Shift);
                if (value10000 >= 100)
                {
                    uint value10 = (value10000 * Div10_16Mul) >> Div10_16Shift;
                    if (value10000 >= 1000)
                    {
                        uIntToString(value - value10000 * 10000, chars + 4);
                        value = (value10 * Div10_16Mul) >> Div10_16Shift;
                        *(uint*)(chars + 2) = ((value10000 - value10 * 10) << 16) | (value10 - value * 10) | 0x300030U;
                        value10 = (value * Div10_16Mul) >> Div10_16Shift;
                        *(uint*)chars = ((value - value10 * 10) << 16) | value10 | 0x300030U;
                        return 8;
                    }
                    else
                    {
                        uIntToString(value - value10000 * 10000, chars + 3);
                        chars[2] = (char)((value10000 - value10 * 10) + '0');
                        value = (value10 * Div10_16Mul) >> Div10_16Shift;
                        *(uint*)chars = ((value10 - value * 10) << 16) | value | 0x300030U;
                        return 7;
                    }
                }
                if (value10000 >= 10)
                {
                    uIntToString(value - value10000 * 10000, chars + 2);
                    value = (value10000 * Div10_16Mul) >> Div10_16Shift;
                    *(uint*)chars = ((value10000 - value * 10) << 16) | value | 0x300030U;
                    return 6;
                }
                uIntToString(value - value10000 * 10000, chars + 1);
                chars[0] = (char)(value10000 + '0');
                return 5;
            }
            if (value >= 100)
            {
                uint value10 = (value * Div10_16Mul) >> Div10_16Shift;
                if (value >= 1000)
                {
                    uint value100 = (value10 * Div10_16Mul) >> Div10_16Shift;
                    *(uint*)(chars + 2) = ((value - value10 * 10) << 16) | (value10 - value100 * 10) | 0x300030U;
                    value10 = (value100 * Div10_16Mul) >> Div10_16Shift;
                    *(uint*)chars = ((value100 - value10 * 10) << 16) | value10 | 0x300030U;
                    return 4;
                }
                else
                {
                    chars[2] = (char)((value - value10 * 10) + '0');
                    uint value100 = (value10 * Div10_16Mul) >> Div10_16Shift;
                    *(uint*)chars = ((value10 - value100 * 10) << 16) | value100 | 0x300030U;
                    return 3;
                }
            }
            if (value >= 10)
            {
                uint value10 = (value * Div10_16Mul) >> Div10_16Shift;
                *(uint*)chars = ((value - value10 * 10) << 16) | value10 | 0x300030U;
                return 2;
            }
            *chars = (char)(value + '0');
            return 1;
        }
        /// <summary>
        /// 数值转字符串
        /// </summary>
        /// <param name="value">数值</param>
        /// <returns>字符串</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static string toString(this int value)
        {
            char* chars = stackalloc char[12];
            keyValue<int, int> index = toString(value, chars);
            return new string(chars + index.Key, 0, index.Value);
        }
        /// <summary>
        /// 数值转字符串
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="charStream">字符流</param>
        internal unsafe static void ToString(int value, charStream charStream)
        {
            char* chars = charStream.GetPrepLengthCurrent(12 + 3);
            keyValue<int, int> index = toString(value, chars);
            charStream.UnsafeSimpleWrite(chars + index.Key, index.Value);
        }
        /// <summary>
        /// 数值转字符串
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="chars">字符串</param>
        /// <returns>起始位置+字符串长度</returns>
        private unsafe static keyValue<int, int> toString(int value, char* chars)
        {
            if (value >= 0) return toString((uint)value, chars);
            uint value32 = (uint)-value;
            if (value32 >= 100000000)
            {
                uint value100000000 = (uint)((value32 * (ulong)Div100000000Mul) >> Div100000000Shift);
                value32 -= value100000000 * 100000000;
                uint value10000 = (uint)((value32 * Div10000Mul) >> Div10000Shift);
                uIntToString(value10000, chars + 4);
                uIntToString(value32 - value10000 * 10000, chars + 8);
                if (value100000000 >= 10)
                {
                    value10000 = (value100000000 * Div10_16Mul) >> Div10_16Shift;
                    *(uint*)(chars + 2) = ((value100000000 - value10000 * 10) << 16) | value10000 | 0x300030U;
                    *(chars + 1) = '-';
                    return new keyValue<int, int>(1, 11);
                }
                *(uint*)(chars + 2) = '-' + ((value100000000 + '0') << 16);
                return new keyValue<int, int>(2, 10);
            }
            return toString_99999999(value32, chars);
        }
        /// <summary>
        /// 绝对值小于100000000的负整数转字符串
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="chars">字符串</param>
        /// <returns>起始位置+字符串长度</returns>
        private unsafe static keyValue<int, int> toString_99999999(uint value, char* chars)
        {
            if (value >= 10000)
            {
                uint value10000 = (uint)((value * Div10000Mul) >> Div10000Shift);
                if (value10000 >= 100)
                {
                    uint value10 = (value10000 * Div10_16Mul) >> Div10_16Shift;
                    if (value10000 >= 1000)
                    {
                        uIntToString(value - value10000 * 10000, chars + 6);
                        value = (value10 * Div10_16Mul) >> Div10_16Shift;
                        *(uint*)(chars + 4) = ((value10000 - value10 * 10) << 16) | (value10 - value * 10) | 0x300030U;
                        value10 = (value * Div10_16Mul) >> Div10_16Shift;
                        *(uint*)(chars + 2) = ((value - value10 * 10) << 16) | value10 | 0x300030U;
                        *(chars + 1) = '-';
                        return new keyValue<int, int>(1, 9);
                    }
                    else
                    {
                        uIntToString(value - value10000 * 10000, chars + 4);
                        value = (value10 * Div10_16Mul) >> Div10_16Shift;
                        *(uint*)(chars + 2) = ((value10000 - value10 * 10) << 16) | (value10 - value * 10) | 0x300030U;
                        *(uint*)chars = '-' + ((value + '0') << 16);
                        return new keyValue<int, int>(0, 8);
                    }
                }
                if (value10000 >= 10)
                {
                    uIntToString(value - value10000 * 10000, chars + 4);
                    value = (value10000 * Div10_16Mul) >> Div10_16Shift;
                    *(uint*)(chars + 2) = ((value10000 - value * 10) << 16) | value | 0x300030U;
                    *(chars + 1) = '-';
                    return new keyValue<int, int>(1, 7);
                }
                uIntToString(value - value10000 * 10000, chars + 2);
                *(uint*)chars = '-' + ((value10000 + '0') << 16);
                return new keyValue<int, int>(0, 6);
            }
            if (value >= 100)
            {
                if (value >= 1000)
                {
                    uint value10 = (value * Div10_16Mul) >> Div10_16Shift;
                    uint value100 = (value10 * Div10_16Mul) >> Div10_16Shift;
                    *(uint*)(chars + 4) = ((value - value10 * 10) << 16) | (value10 - value100 * 10) | 0x300030U;
                    value10 = (value100 * Div10_16Mul) >> Div10_16Shift;
                    *(uint*)(chars + 2) = ((value100 - value10 * 10) << 16) | value10 | 0x300030U;
                    *(chars + 1) = '-';
                    return new keyValue<int, int>(1, 5);
                }
                else
                {
                    uint value10 = (value * Div10_16Mul) >> Div10_16Shift;
                    uint value100 = (value10 * Div10_16Mul) >> Div10_16Shift;
                    *(uint*)(chars + 2) = ((value - value10 * 10) << 16) | (value10 - value100 * 10) | 0x300030U;
                    *(uint*)chars = '-' + ((value100 + '0') << 16);
                    return new keyValue<int, int>(0, 4);
                }
            }
            if (value >= 10)
            {
                *chars = '-';
                uint value10 = (value * Div10_16Mul) >> Div10_16Shift;
                *(uint*)(chars + 1) = ((value - value10 * 10) << 16) | value10 | 0x300030U;
                return new keyValue<int, int>(0, 3);
            }
            *(uint*)chars = '-' + ((value + '0') << 16);
            return new keyValue<int, int>(0, 2);
        }
        /// <summary>
        /// 数值转字符串
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="chars">字符串</param>
        private unsafe static void uIntToString(uint value, char* chars)
        {
            uint value10 = (value * Div10_16Mul) >> Div10_16Shift;
            uint value100 = (value10 * Div10_16Mul) >> Div10_16Shift;
            *(uint*)(chars + 2) = ((value - value10 * 10) << 16) | (value10 - value100 * 10) | 0x300030U;
            value10 = (value100 * Div10_16Mul) >> Div10_16Shift;
            *(uint*)chars = ((value100 - value10 * 10) << 16) | value10 | 0x300030U;
        }
        /// <summary>
        /// 数值转字符串
        /// </summary>
        /// <param name="value">数值</param>
        /// <returns>字符串</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static string toString(this ulong value)
        {
            char* chars = stackalloc char[20];
            keyValue<int, int> index = toString(value, chars);
            return new string(chars + index.Key, 0, index.Value);
        }
        /// <summary>
        /// 数值转字符串
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="charStream">字符流</param>
        internal unsafe static void ToString(ulong value, charStream charStream)
        {
            charStream.PrepLength(20 + 3);
            UnsafeToString(value, charStream);
        }
        /// <summary>
        /// 数值转字符串
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="charStream">字符流</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe static void UnsafeToString(ulong value, charStream charStream)
        {
            char* chars = charStream.CurrentChar;
            keyValue<int, int> index = toString(value, chars);
            charStream.UnsafeSimpleWrite(chars + index.Key, index.Value);
        }
        /// <summary>
        /// 数值转字符串
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="chars">字符串</param>
        /// <returns>起始位置+字符串长度</returns>
        internal unsafe static keyValue<int, int> toString(ulong value, char* chars)
        {
            if (value >= 10000000000000000L)
            {
                ulong value100000000 = value / 100000000;
                value -= value100000000 * 100000000;
                uint value10000 = (uint)((value * Div10000Mul) >> Div10000Shift);
                uIntToString(value10000, chars + 12);
                uIntToString((uint)value - value10000 * 10000U, chars + 16);
                value = value100000000 / 100000000;
                value100000000 -= value * 100000000;
                value10000 = (uint)((value100000000 * Div10000Mul) >> Div10000Shift);
                uIntToString(value10000, chars + 4);
                uIntToString((uint)value100000000 - value10000 * 10000U, chars + 8);
                uint value32 = (uint)value;
                if (value32 >= 100)
                {
                    value10000 = (value32 * Div10_16Mul) >> Div10_16Shift;
                    uint value100 = (value10000 * Div10_16Mul) >> Div10_16Shift;
                    *(uint*)(chars + 2) = ((value32 - value10000 * 10) << 16) | (value10000 - value100 * 10) | 0x300030U;
                    if (value32 >= 1000)
                    {
                        value10000 = (value100 * Div10_16Mul) >> Div10_16Shift;
                        *(uint*)chars = ((value100 - value10000 * 10) << 16) | value10000 | 0x300030U;
                        return new keyValue<int, int>(0, 20);
                    }
                    *(chars + 1) = (char)(value100 + '0');
                    return new keyValue<int, int>(1, 19);
                }
                if (value32 >= 10)
                {
                    value10000 = (value32 * Div10_16Mul) >> Div10_16Shift;
                    *(uint*)(chars + 2) = ((value32 - value10000 * 10) << 16) | value10000 | 0x300030U;
                    return new keyValue<int, int>(2, 18);
                }
                *(chars + 3) = (char)(value32 + '0');
                return new keyValue<int, int>(3, 17);
            }
            if (value >= 100000000)
            {
                ulong value100000000 = value / 100000000;
                value -= value100000000 * 100000000;
                uint value10000 = (uint)((value * Div10000Mul) >> Div10000Shift);
                uIntToString(value10000, chars + 8);
                uIntToString((uint)value - value10000 * 10000U, chars + 12);
                uint value32 = (uint)value100000000;
                if (value32 >= 10000)
                {
                    value10000 = (uint)((value100000000 * Div10000Mul) >> Div10000Shift);
                    uIntToString(value32 - value10000 * 10000, chars + 4);
                    if (value10000 >= 100)
                    {
                        value32 = (value10000 * Div10_16Mul) >> Div10_16Shift;
                        if (value10000 >= 1000)
                        {
                            uint value100 = (value32 * Div10_16Mul) >> Div10_16Shift;
                            *(uint*)(chars + 2) = ((value10000 - value32 * 10) << 16) | (value32 - value100 * 10) | 0x300030U;
                            value32 = (value100 * Div10_16Mul) >> Div10_16Shift;
                            *(uint*)chars = ((value100 - value32 * 10) << 16) | value32 | 0x300030U;
                            return new keyValue<int, int>(0, 16);
                        }
                        else
                        {
                            chars[3] = (char)((value10000 - value32 * 10) + '0');
                            uint value100 = (value32 * Div10_16Mul) >> Div10_16Shift;
                            *(uint*)(chars + 1) = ((value32 - value100 * 10) << 16) | value100 | 0x300030U;
                            return new keyValue<int, int>(1, 15);
                        }
                    }
                    if (value10000 >= 10)
                    {
                        value32 = (value10000 * Div10_16Mul) >> Div10_16Shift;
                        *(uint*)(chars + 2) = ((value10000 - value32 * 10) << 16) | value32 | 0x300030U;
                        return new keyValue<int, int>(2, 14);
                    }
                    *(chars + 3) = (char)(value10000 + '0');
                    return new keyValue<int, int>(3, 13);
                }
                if (value32 >= 100)
                {
                    value10000 = (value32 * Div10_16Mul) >> Div10_16Shift;
                    if (value32 >= 1000)
                    {
                        uint value100 = (value10000 * Div10_16Mul) >> Div10_16Shift;
                        *(uint*)(chars + 6) = ((value32 - value10000 * 10) << 16) | (value10000 - value100 * 10) | 0x300030U;
                        value10000 = (value100 * Div10_16Mul) >> Div10_16Shift;
                        *(uint*)(chars + 4) = ((value100 - value10000 * 10) << 16) | value10000 | 0x300030U;
                        return new keyValue<int, int>(4, 12);
                    }
                    else
                    {
                        chars[7] = (char)((value32 - value10000 * 10) + '0');
                        uint value100 = (value10000 * Div10_16Mul) >> Div10_16Shift;
                        *(uint*)(chars + 5) = ((value10000 - value100 * 10) << 16) | value100 | 0x300030U;
                        return new keyValue<int, int>(5, 11);
                    }
                }
                if (value32 >= 10)
                {
                    value10000 = (value32 * Div10_16Mul) >> Div10_16Shift;
                    *(uint*)(chars + 6) = ((value32 - value10000 * 10) << 16) | value10000 | 0x300030U;
                    return new keyValue<int, int>(6, 10);
                }
                *(chars + 7) = (char)(value32 + '0');
                return new keyValue<int, int>(7, 9);
            }
            return new keyValue<int, int>(0, toString99999999((uint)value, chars));
        }
        /// <summary>
        /// 数值转字符串
        /// </summary>
        /// <param name="value">数值</param>
        /// <returns>字符串</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static string toString(this long value)
        {
            char* chars = stackalloc char[22];
            keyValue<int, int> index = toString(value, chars);
            return new string(chars + index.Key, 0, index.Value);
        }
        /// <summary>
        /// 数值转字符串
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="charStream">字符流</param>
        internal unsafe static void ToString(long value, charStream charStream)
        {
            charStream.PrepLength(22 + 3);
            UnsafeToString(value, charStream);
        }
        /// <summary>
        /// 数值转字符串
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="charStream">字符流</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe static void UnsafeToString(long value, charStream charStream)
        {
            char* chars = charStream.CurrentChar;
            keyValue<int, int> index = toString(value, chars);
            charStream.UnsafeSimpleWrite(chars + index.Key, index.Value);
        }
        /// <summary>
        /// 数值转字符串
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="chars">字符串</param>
        /// <returns>起始位置+字符串长度</returns>
        private unsafe static keyValue<int, int> toString(long value, char* chars)
        {
            if (value >= 0) return toString((ulong)value, chars);
            ulong value64 = (ulong)-value;
            if (value64 >= 10000000000000000L)
            {
                ulong value100000000 = value64 / 100000000;
                value64 -= value100000000 * 100000000;
                uint value10000 = (uint)((value64 * Div10000Mul) >> Div10000Shift);
                uIntToString(value10000, chars + 14);
                uIntToString((uint)value64 - value10000 * 10000U, chars + 18);
                value64 = value100000000 / 100000000;
                value100000000 -= value64 * 100000000;
                value10000 = (uint)((value100000000 * Div10000Mul) >> Div10000Shift);
                uIntToString(value10000, chars + 6);
                uIntToString((uint)value100000000 - value10000 * 10000U, chars + 10);
                uint value32 = (uint)value64;
                if (value32 >= 100)
                {
                    value10000 = (value32 * Div10_16Mul) >> Div10_16Shift;
                    uint value100 = (value10000 * Div10_16Mul) >> Div10_16Shift;
                    *(uint*)(chars + 4) = ((value32 - value10000 * 10) << 16) | (value10000 - value100 * 10) | 0x300030U;
                    if (value32 >= 1000)
                    {
                        value10000 = (value100 * Div10_16Mul) >> Div10_16Shift;
                        *(uint*)(chars + 2) = ((value100 - value10000 * 10) << 16) | value10000 | 0x300030U;
                        *(chars + 1) = '-';
                        return new keyValue<int, int>(1, 21);
                    }
                    *(uint*)(chars + 2) = '-' + ((value100 + '0') << 16);
                    return new keyValue<int, int>(2, 20);
                }
                if (value32 >= 10)
                {
                    value10000 = (value32 * Div10_16Mul) >> Div10_16Shift;
                    *(uint*)(chars + 4) = ((value32 - value10000 * 10) << 16) | value10000 | 0x300030U;
                    *(chars + 3) = '-';
                    return new keyValue<int, int>(3, 19);
                }
                *(uint*)(chars + 4) = '-' + ((value32 + '0') << 16);
                return new keyValue<int, int>(4, 18);
            }
            if (value64 >= 100000000)
            {
                ulong value100000000 = value64 / 100000000;
                value64 -= value100000000 * 100000000;
                uint value10000 = (uint)((value64 * Div10000Mul) >> Div10000Shift);
                uIntToString(value10000, chars + 10);
                uIntToString((uint)value64 - value10000 * 10000U, chars + 14);
                uint value32 = (uint)value100000000;
                if (value32 >= 10000)
                {
                    value10000 = (uint)((value100000000 * Div10000Mul) >> Div10000Shift);
                    uIntToString(value32 - value10000 * 10000, chars + 6);
                    if (value10000 >= 100)
                    {
                        value32 = (value10000 * Div10_16Mul) >> Div10_16Shift;
                        uint value100 = (value32 * Div10_16Mul) >> Div10_16Shift;
                        *(uint*)(chars + 4) = ((value10000 - value32 * 10) << 16) | (value32 - value100 * 10) | 0x300030U;
                        if (value10000 >= 1000)
                        {
                            value32 = (value100 * Div10_16Mul) >> Div10_16Shift;
                            *(uint*)(chars + 2) = ((value100 - value32 * 10) << 16) | value32 | 0x300030U;
                            *(chars + 1) = '-';
                            return new keyValue<int, int>(1, 17);
                        }
                        *(uint*)(chars + 2) = '-' + ((value100 + '0') << 16);
                        return new keyValue<int, int>(2, 16);
                    }
                    if (value10000 >= 10)
                    {
                        value32 = (value10000 * Div10_16Mul) >> Div10_16Shift;
                        *(uint*)(chars + 4) = ((value10000 - value32 * 10) << 16) | value32 | 0x300030U;
                        *(chars + 3) = '-';
                        return new keyValue<int, int>(3, 15);
                    }
                    *(uint*)(chars + 4) = '-' + ((value10000 + '0') << 16);
                    return new keyValue<int, int>(4, 14);
                }
                if (value32 >= 100)
                {
                    value10000 = (value32 * Div10_16Mul) >> Div10_16Shift;
                    uint value100 = (value10000 * Div10_16Mul) >> Div10_16Shift;
                    *(uint*)(chars + 8) = ((value32 - value10000 * 10) << 16) | (value10000 - value100 * 10) | 0x300030U;
                    if (value32 >= 1000)
                    {
                        value10000 = (value100 * Div10_16Mul) >> Div10_16Shift;
                        *(uint*)(chars + 6) = ((value100 - value10000 * 10) << 16) | value10000 | 0x300030U;
                        *(chars + 5) = '-';
                        return new keyValue<int, int>(5, 13);
                    }
                    *(uint*)(chars + 6) = '-' + ((value100 + '0') << 16);
                    return new keyValue<int, int>(6, 12);
                }
                if (value32 >= 10)
                {
                    value10000 = (value32 * Div10_16Mul) >> Div10_16Shift;
                    *(uint*)(chars + 8) = ((value32 - value10000 * 10) << 16) | value10000 | 0x300030U;
                    *(chars + 7) = '-';
                    return new keyValue<int, int>(7, 11);
                }
                *(uint*)(chars + 8) = '-' + ((value32 + '0') << 16);
                return new keyValue<int, int>(8, 10);
            }
            return toString_99999999((uint)value64, chars);
        }
        /// <summary>
        /// 双精度浮点数转字符串(用于代码生成)
        /// </summary>
        /// <param name="value">浮点数</param>
        /// <returns>字符串</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static string toString(this double value)
        {
            return value.ToString();
        }
        /// <summary>
        /// 双精度浮点数转字符串(用于代码生成)
        /// </summary>
        /// <param name="value">浮点数</param>
        /// <returns>字符串</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static string toString(this float value)
        {
            return value.ToString();
        }
        #endregion

        ///// <summary>
        ///// 字节转十六进制字符串
        ///// </summary>
        //internal static pointer ByteToHexChars;
        /// <summary>
        /// 2^n相关32位deBruijn序列集合
        /// </summary>
        private static readonly pointer.reference deBruijn32;
        /// <summary>
        /// 2^n相关32位deBruijn序列
        /// </summary>
        public const uint DeBruijn32Number = 0x04653adfU;
        /// <summary>
        /// 获取2^n相关32位deBruijn序列集合
        /// </summary>
        /// <returns>2^n相关32位deBruijn序列集合</returns>
        public static pointer GetDeBruijn32()
        {
            pointer deBruijn32 = unmanaged.GetStatic(32, true);
            byte* deBruijn32Data = deBruijn32.Byte;
            for (byte bit = 0; bit != 32; ++bit) deBruijn32Data[((1U << bit) * DeBruijn32Number) >> 27] = bit;
            //deBruijn64 = new byte[64];
            //for (byte bit = 0; bit != 64; ++bit) deBruijn64[((1UL << bit) * deBruijn64Number) >> 58] = bit;
            return deBruijn32;
        }
        /// <summary>
        /// 获取有效位长度
        /// </summary>
        /// <param name="value">数据</param>
        /// <returns>有效位长度</returns>
        public static int bits(this uint value)
        {
            if ((value & 0x80000000U) == 0)
            {
                uint code = value;
                code |= code >> 16;
                code |= code >> 8;
                code |= code >> 4;
                code |= code >> 2;
                code |= code >> 1;
                return deBruijn32.Byte[((++code) * DeBruijn32Number) >> 27];
            }
            return 32;
        }
        /// <summary>
        /// 获取有效位长度
        /// </summary>
        /// <param name="value">数据</param>
        /// <returns>有效位长度</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static int bits(this ulong value)
        {
            return (value & 0xffffffff00000000UL) == 0 ? bits((uint)value) : (bits((uint)(value >> 32)) + 32);
            //if ((value & 0x8000000000000000UL) == 0)
            //{
            //    ulong code = value;
            //    code |= code >> 32;
            //    code |= code >> 16;
            //    code |= code >> 8;
            //    code |= code >> 4;
            //    code |= code >> 2;
            //    code |= code >> 1;
            //    return DeBruijn64[((++code) * DeBruijn64Number) >> 58];
            //}
            //else return 32;
        }
        /// <summary>
        /// 获取二进制1位的个数
        /// </summary>
        /// <param name="value">数据</param>
        /// <returns>二进制1位的个数</returns>
        public static int bitCount(this uint value)
        {
            //return bitCounts[(byte)value] + bitCounts[(byte)(value >> 8)] + bitCounts[(byte)(value >> 16)] + bitCounts[value >> 24];

            //value = (value & 0x49249249) + ((value >> 1) & 0x49249249) + ((value >> 2) & 0x49249249);
            //value = (value & 0xc71c71c7) + ((value >> 3) & 0xc71c71c7);
            //uint div = (uint)(((ulong)value * (((1UL << 37) + 62) / 63)) >> 37);
            //return (int)(value - (div << 6) + div);

            //value = (value & 0x49249249) + ((value >> 1) & 0x49249249) + ((value >> 2) & 0x49249249);
            //value = (value & 0x71c71c7) + ((value >> 3) & 0x71c71c7) + (value >> 30);
            //uint nextValue = (uint)((value * 0x41041042UL) >> 36);
            //return (int)(value - (nextValue << 6) + nextValue);

            value -= ((value >> 1) & 0x55555555U);//2:2
            value = (value & 0x33333333U) + ((value >> 2) & 0x33333333U);//4:4
            value += value >> 4;
            value &= 0x0f0f0f0f;//8:8

            //uint div = (uint)(((ulong)value * (((1UL << 39) + 254) / 255)) >> 39);
            //return (int)(value - (div << 8) + div);
            value += (value >> 8);
            return (byte)(value + (value >> 16));
        }
        /// <summary>
        /// 转十六进制字符串
        /// </summary>
        /// <param name="value">数字值</param>
        /// <returns>十六进制字符串</returns>
        public unsafe static string toHex(this ushort value)
        {
            string hexs = fastCSharp.String.FastAllocateString(4);
            uint value32 = value;
            fixed (char* chars = hexs)
            {
                *chars = (char)ToHex(value32 >> 12);
                *(chars + 1) = (char)ToHex((value32 >> 8) & 15);
                *(chars + 2) = (char)ToHex((value32 >> 4) & 15);
                *(chars + 3) = (char)ToHex(value32 & 15);
            }
            return hexs;
        }
        /// <summary>
        /// 数字值转换为十六进制字符串
        /// </summary>
        /// <param name="value">数字值</param>
        /// <param name="hexs">十六进制字符串</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static void UnsafeToHex(this uint value, char* hexs)
        {
            if (hexs != null) uIntToHex(value, hexs);
        }
        /// <summary>
        /// 数字值转换为十六进制字符串
        /// </summary>
        /// <param name="value">数字值</param>
        /// <param name="hexs">十六进制字符串</param>
        private unsafe static void uIntToHex(uint value, char* hexs)
        {
            *hexs = (char)ToHex(value >> 28);
            *(hexs + 1) = (char)ToHex((value >> 24) & 15);
            *(hexs + 2) = (char)ToHex((value >> 20) & 15);
            *(hexs + 3) = (char)ToHex((value >> 16) & 15);
            *(hexs + 4) = (char)ToHex((value >> 12) & 15);
            *(hexs + 5) = (char)ToHex((value >> 8) & 15);
            *(hexs + 6) = (char)ToHex((value >> 4) & 15);
            *(hexs + 7) = (char)ToHex(value & 15);
        }
        /// <summary>
        /// 转换8位十六进制字符串
        /// </summary>
        /// <param name="value">数字值</param>
        /// <returns>8位十六进制字符串</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static string toHex8(this uint value)
        {
            string hexs = fastCSharp.String.FastAllocateString(8);
            fixed (char* hexFixed = hexs) uIntToHex(value, hexFixed);
            return hexs;
        }
        /// <summary>
        /// 转换16位十六进制字符串
        /// </summary>
        /// <param name="value">数字值</param>
        /// <returns>16位十六进制字符串</returns>
        public unsafe static string toHex16(this ulong value)
        {
            string hexs = fastCSharp.String.FastAllocateString(16);
            fixed (char* hexFixed = hexs)
            {
                uIntToHex((uint)value, hexFixed + 8);
                uIntToHex((uint)(value >> 32), hexFixed);
            }
            return hexs;
        }
        /// <summary>
        /// 16进制字符串转换成整数
        /// </summary>
        /// <param name="start">起始位置</param>
        /// <param name="end">结束位置</param>
        /// <returns>整数</returns>
        internal static unsafe uint UnsafeParseHex(char* start, char* end)
        {
            uint value = ParseHex(*start);
            while (++start != end)
            {
                value <<= 4;
                value |= ParseHex(*start);
            }
            return value;
        }
        /// <summary>
        /// 16进制字符串转换成整数
        /// </summary>
        /// <param name="start">起始位置</param>
        /// <returns>整数</returns>
        public static unsafe uint UnsafeParseHex32(char* start)
        {
            return (ParseHex(*start) << 28)
                | (ParseHex(*(start + 1)) << 24)
                | (ParseHex(*(start + 2)) << 20)
                | (ParseHex(*(start + 3)) << 16)
                | (ParseHex(*(start + 4)) << 12)
                | (ParseHex(*(start + 5)) << 8)
                | (ParseHex(*(start + 6)) << 4)
                | ParseHex(*(start + 7));
        }
        /// <summary>
        /// 16位十六进制转64二进制位整数(无格式检测)
        /// </summary>
        /// <param name="value">十六进制字符串</param>
        /// <returns>64二进制位整数</returns>
        public unsafe static ulong parseHex16NoCheck(this string value)
        {
            if (value != null && value.Length == 16)
            {
                fixed (char* hexs = value) return UnsafeParseHex64(hexs, hexs + 16);
            }
            return 0;
        }
        /// <summary>
        /// 16进制字符串转换成64位整数
        /// </summary>
        /// <param name="start">起始位置</param>
        /// <param name="end">结束位置</param>
        /// <returns>64位整数</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static unsafe ulong UnsafeParseHex64(char* start, char* end)
        {
            int length = (int)(end - start);
            switch (length)
            {
                case 8: return UnsafeParseHex32(start);
                case 16: return ((ulong)UnsafeParseHex32(start) << 32) + UnsafeParseHex32(start + 8);
                default:
                    if (length < 8) return UnsafeParseHex(start, end);
                    char* lowStart = end - 8;
                    return ((ulong)UnsafeParseHex(start, lowStart) << 32) + UnsafeParseHex32(lowStart);
            }
        }
        /// <summary>
        /// 8位十六进制转32二进制位整数(无格式检测)
        /// </summary>
        /// <param name="value">十六进制字符串</param>
        /// <returns>32二进制位整数</returns>
        public unsafe static uint parseHex8NoCheck(this string value)
        {
            if (value != null && value.Length == 8)
            {
                fixed (char* hexs = value) return UnsafeParseHex32(hexs);
            }
            return 0;
        }
        /// <summary>
        /// 10进制字符串转换成整数
        /// </summary>
        /// <param name="start">起始位置</param>
        /// <param name="end">结束位置</param>
        /// <returns>整数</returns>
        internal static unsafe uint Parse(char* start, char* end)
        {
            uint value = (uint)(*start - '0');
            while (++start != end)
            {
                value *= 10;
                value += (uint)(*start - '0');
            }
            return value;
        }
        /// <summary>
        /// 10进制字符串转换成64位整数
        /// </summary>
        /// <param name="start">起始位置</param>
        /// <param name="end">结束位置</param>
        /// <returns>64位整数</returns>
        internal static unsafe ulong Parse64(char* start, char* end)
        {
            int length = (int)(end - start);
            if (length <= 9) return Parse(start, end);
            char* lowStart = end - 9;
            if (length <= 18) return ((ulong)Parse(start, lowStart) * 1000000000UL) + Parse(lowStart, end);
            char* highStart = lowStart - 9;
            return ((ulong)Parse(start, highStart) * 1000000000000000000UL) + ((ulong)Parse(highStart, lowStart) * 1000000000UL) + Parse(lowStart, end);
        }
        /// <summary>
        /// 16进制字符串转换成整数
        /// </summary>
        /// <param name="start">起始位置</param>
        /// <returns>整数</returns>
        internal static unsafe uint ParseHex32(byte* start)
        {
            return (ParseHex(*start) << 28)
                | (ParseHex(*(start + 1)) << 24)
                | (ParseHex(*(start + 2)) << 20)
                | (ParseHex(*(start + 3)) << 16)
                | (ParseHex(*(start + 4)) << 12)
                | (ParseHex(*(start + 5)) << 8)
                | (ParseHex(*(start + 6)) << 4)
                | ParseHex(*(start + 7));
        }
        /// <summary>
        /// 数字值转换为十六进制字符串
        /// </summary>
        /// <param name="value">数字值</param>
        /// <param name="hexs">十六进制字符串</param>
        internal unsafe static void ToHex(uint value, byte* hexs)
        {
            *hexs = (byte)ToHex(value >> 28);
            *(hexs + 1) = (byte)ToHex((value >> 24) & 15);
            *(hexs + 2) = (byte)ToHex((value >> 20) & 15);
            *(hexs + 3) = (byte)ToHex((value >> 16) & 15);
            *(hexs + 4) = (byte)ToHex((value >> 12) & 15);
            *(hexs + 5) = (byte)ToHex((value >> 8) & 15);
            *(hexs + 6) = (byte)ToHex((value >> 4) & 15);
            *(hexs + 7) = (byte)ToHex(value & 15);
        }
        /// <summary>
        /// 转换16位十六进制字符串
        /// </summary>
        /// <param name="value">数字值</param>
        /// <param name="hexs">16位十六进制字符串</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static void UnsafeToHex16(this ulong value, byte* hexs)
        {
            ToHex((uint)value, hexs + 8);
            ToHex((uint)(value >> 32), hexs);
        }
        /// <summary>
        /// 数字值转换为十六进制字符串
        /// </summary>
        /// <param name="value">数字值</param>
        /// <param name="hexs">十六进制字符串</param>
        internal unsafe static void ToHex(uint value, char* hexs)
        {
            *hexs = (char)ToHex(value >> 28);
            *(hexs + 1) = (char)ToHex((value >> 24) & 15);
            *(hexs + 2) = (char)ToHex((value >> 20) & 15);
            *(hexs + 4) = (char)ToHex((value >> 12) & 15);
            *(hexs + 3) = (char)ToHex((value >> 16) & 15);
            *(hexs + 5) = (char)ToHex((value >> 8) & 15);
            *(hexs + 6) = (char)ToHex((value >> 4) & 15);
            *(hexs + 7) = (char)ToHex(value & 15);
        }
        /// <summary>
        /// 转换16位十六进制字符串
        /// </summary>
        /// <param name="value">数字值</param>
        /// <param name="hexs">16位十六进制字符串</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static void UnsafeToHex16(this ulong value, char* hexs)
        {
            ToHex((uint)value, hexs + 8);
            ToHex((uint)(value >> 32), hexs);
        }
        /// <summary>
        /// 数字值转换为十六进制字符串
        /// </summary>
        /// <param name="value">数字值</param>
        /// <param name="hexs">十六进制字符串</param>
        /// <returns>0表示相等</returns>
        internal unsafe static uint CheckHex(uint value, char* hexs)
        {
            return ((uint)*hexs ^ ToHex(value >> 28))
                | ((uint)*(hexs + 1) ^ ToHex((value >> 24) & 15))
                | ((uint)*(hexs + 2) ^ ToHex((value >> 20) & 15))
                | ((uint)*(hexs + 3) ^ ToHex((value >> 16) & 15))
                | ((uint)*(hexs + 4) ^ ToHex((value >> 12) & 15))
                | ((uint)*(hexs + 5) ^ ToHex((value >> 8) & 15))
                | ((uint)*(hexs + 6) ^ ToHex((value >> 4) & 15))
                | ((uint)*(hexs + 7) ^ ToHex(value & 15));
        }
        /// <summary>
        /// 转换16位十六进制字符串
        /// </summary>
        /// <param name="value">数字值</param>
        /// <param name="hexs">16位十六进制字符串</param>
        /// <returns>0表示相等</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe static uint UnsafeCheckHex(this ulong value, char* hexs)
        {
            return CheckHex((uint)value, hexs + 8) == 0 ? CheckHex((uint)(value >> 32), hexs) : 1;
        }
        /// <summary>
        /// 半字节转十六进制字符
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static uint ToHex(uint data)
        {
            return data < 10 ? data + '0' : (data + ('0' + 'A' - '9' - 1));
        }
        /// <summary>
        /// 十六进制字符转字节
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static uint ParseHex(uint hex)
        {
            uint value = hex - '0';
            return value >= 10 ? (hex & 0xdf) - ('0' + 'A' - '9' - 1) : value;
        }
        /// <summary>
        /// 求平方根
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="mod">余数</param>
        /// <returns>平方根</returns>
        public static uint sqrt(this uint value, out uint mod)
        {
            uint sqrtValue = 0;
            if ((mod = value) >= 0x40000000)
            {
                sqrtValue = 0x8000;
                mod -= 0x40000000;
            }
            value = (sqrtValue << 15) + 0x10000000;
            if (mod >= value)
            {
                sqrtValue |= 0x4000;
                mod -= value;
            }
            value = (sqrtValue << 14) + 0x4000000;
            if (mod >= value)
            {
                sqrtValue |= 0x2000;
                mod -= value;
            }
            value = (sqrtValue << 13) + 0x1000000;
            if (mod >= value)
            {
                sqrtValue |= 0x1000;
                mod -= value;
            }
            value = (sqrtValue << 12) + 0x400000;
            if (mod >= value)
            {
                sqrtValue |= 0x800;
                mod -= value;
            }
            value = (sqrtValue << 11) + 0x100000;
            if (mod >= value)
            {
                sqrtValue |= 0x400;
                mod -= value;
            }
            value = (sqrtValue << 10) + 0x40000;
            if (mod >= value)
            {
                sqrtValue |= 0x200;
                mod -= value;
            }
            value = (sqrtValue << 9) + 0x10000;
            if (mod >= value)
            {
                sqrtValue |= 0x100;
                mod -= value;
            }
            value = (sqrtValue << 8) + 0x4000;
            if (mod >= value)
            {
                sqrtValue |= 0x80;
                mod -= value;
            }
            value = (sqrtValue << 7) + 0x1000;
            if (mod >= value)
            {
                sqrtValue |= 0x40;
                mod -= value;
            }
            value = (sqrtValue << 6) + 0x400;
            if (mod >= value)
            {
                sqrtValue |= 0x20;
                mod -= value;
            }
            value = (sqrtValue << 5) + 0x100;
            if (mod >= value)
            {
                sqrtValue |= 0x10;
                mod -= value;
            }
            value = (sqrtValue << 4) + 0x40;
            if (mod >= value)
            {
                sqrtValue |= 0x8;
                mod -= value;
            }
            value = (sqrtValue << 3) + 0x10;
            if (mod >= value)
            {
                sqrtValue |= 0x4;
                mod -= value;
            }
            value = (sqrtValue << 2) + 0x4;
            if (mod >= value)
            {
                sqrtValue |= 0x2;
                mod -= value;
            }
            value = (sqrtValue << 1) + 0x1;
            if (mod >= value)
            {
                sqrtValue++;
                mod -= value;
            }
            return sqrtValue;
        }
        static number()
        {
            deBruijn32 = GetDeBruijn32().Reference;

            //ByteToHexChars = unmanaged.Get(256 * sizeof(int), false);
            //for (int value = 256; value != 0; )
            //{
            //    --value;
            //    int high = value >> 4, low = value & 0xf;
            //    ByteToHexChars.Int[value] = (high < 10 ? high : (high + ('A' - '9' - 1))) + ((low < 10 ? low : (low + ('A' - '9' - 1))) << 16) + 0x300030;
            //}
        }
    }
}
