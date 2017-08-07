using System;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 数值相关扩展操作
    /// </summary>
    public unsafe static class numberExpand
    {
        /// <summary>
        /// 2^n相关32位deBruijn序列集合
        /// </summary>
        private static readonly pointer.reference deBruijn32;
        ///// <summary>
        ///// 2^n相关64位deBruijn序列集合
        ///// </summary>
        //private static byte[] deBruijn64;
        ///// <summary>
        ///// 2^n相关64位deBruijn序列
        ///// </summary>
        //private const ulong deBruijn64Number = 0x0218a392cd3d5dbfUL;
        /// <summary>
        /// 获取最后二进制0位的长度
        /// </summary>
        /// <param name="value">数据</param>
        /// <returns>最后二进制0位的长度</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static int endBits(this uint value)
        {
            return value != 0 ? deBruijn32.Byte[((value & (0U - value)) * fastCSharp.number.DeBruijn32Number) >> 27] : 0;
        }
        /// <summary>
        /// 获取最后二进制0位的长度
        /// </summary>
        /// <param name="value">数据</param>
        /// <returns>最后二进制0位的长度</returns>
        public static int endBits(this ulong value)
        {
            return (value & 0xffffffff00000000UL) == 0
                ? (value != 0 ? endBits((uint)(value >> 32)) + 32 : 0)
                : endBits((uint)value);
            //return value != 0 ? DeBruijn64[((value & (0UL - value)) * DeBruijn64Number) >> 58] : 0;
        }
        /// <summary>
        /// 获取2的幂次方的位数
        /// </summary>
        /// <param name="value">2的幂次方值</param>
        /// <returns>位数,非幂次方返回-1</returns>
        public static int power2Bits(this ulong value)
        {
            int bits = (value & 0xffffffff00000000UL) == 0
                ? deBruijn32.Byte[((uint)value * fastCSharp.number.DeBruijn32Number) >> 27]
                : (deBruijn32.Byte[((uint)(value >> 32) * fastCSharp.number.DeBruijn32Number) >> 27] + 32);
            return 1UL << bits == value ? bits : -1;
        }
        /// <summary>
        /// 获取2的幂次方的位数
        /// </summary>
        /// <param name="value">2的幂次方值</param>
        /// <returns>位数,非幂次方返回-1</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static int power2Bits(this uint value)
        {
            int bits = deBruijn32.Byte[(value * fastCSharp.number.DeBruijn32Number) >> 27];
            return 1U << bits == value ? bits : -1;
        }
        /// <summary>
        /// 获取二进制1位的个数
        /// </summary>
        /// <param name="value">数据</param>
        /// <returns>二进制1位的个数</returns>
        public static int bitCount(this ulong value)
        {
            value -= ((value >> 1) & 0x5555555555555555UL);//2:2
            value = (value & 0x3333333333333333UL) + ((value >> 2) & 0x3333333333333333UL);//4:4
            value += value >> 4;
            value &= 0x0f0f0f0f0f0f0f0fUL;//8:8
            //return (int)(value % 255);
            value += (value >> 8);
            value += (value >> 16);
            return (byte)(value + (value >> 32));
        }
        /// <summary>
        /// 判断一个数是否2的幂次方
        /// </summary>
        /// <returns>是否2的幂次方</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static bool isPower2(this ulong value)
        {
            return value != 0 && (value & (value - 1)) == 0;
        }
        /// <summary>
        /// 判断一个数是否2的幂次方
        /// </summary>
        /// <returns>是否2的幂次方</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static bool isPower2(this uint value)
        {
            return value != 0 && (value & (value - 1)) == 0;
        }
        /// <summary>
        /// 二进制反转
        /// </summary>
        /// <param name="value">整数</param>
        /// <returns>反转后的数</returns>
        public static uint reverse(this uint value)
        {
            value = ((value << 1) & 0xaaaaaaaaU) | ((value >> 1) & 0x55555555U);
            value = ((value << 2) & 0xccccccccU) | ((value >> 2) & 0x33333333U);
            value = ((value << 4) & 0xf0f0f0f0U) | ((value >> 4) & 0xf0f0f0fU);
            value = ((value << 8) & 0xff00ff00U) | ((value >> 8) & 0xff00ffU);
            return (value << 16) | (value >> 16);
        }
        /// <summary>
        /// 求最大公约数
        /// </summary>
        /// <param name="leftValue"></param>
        /// <param name="rightValue"></param>
        /// <returns>最大公约数</returns>
        public static ulong gcd(this ulong leftValue, ulong rightValue)
        {
            if (leftValue == 0 || rightValue == 0) log.Error.Throw(log.exceptionType.Null);
            for (rightValue %= leftValue; rightValue != 0; rightValue %= leftValue)
            {
                if ((leftValue %= rightValue) == 0) return rightValue;
            }
            return leftValue;
            //int leftShift = leftValue == 0 ? 0 : power2_64bits[(leftValue ^ (leftValue & (leftValue - 1))) % 67], rightShift = rightValue == 0 ? 0 : power2_64bits[(rightValue ^ (rightValue & (rightValue - 1))) % 67];
            //if (leftShift == 0)
            //{
            //    if (rightShift != 0) rightValue >>= rightShift;
            //}
            //else if (rightShift == 0)
            //{
            //    leftValue >>= leftShift;
            //    leftShift = 0;
            //}
            //else
            //{
            //    leftValue >>= leftShift;
            //    rightValue >>= rightShift;
            //    if (leftShift > rightShift) leftShift = rightShift;
            //}
            //if (leftValue < rightValue) rightValue -= leftValue;
            //else leftValue -= (rightValue = leftValue - rightValue);
            //while (rightValue != 0)
            //{
            //    if (leftValue < (rightValue >>= power2_64bits[(rightValue ^ (rightValue & (rightValue - 1))) % 67])) rightValue -= leftValue;
            //    else leftValue -= (rightValue = leftValue - rightValue);
            //}
            //leftValue <<= leftShift;
        }
        /// <summary>
        /// 求最大公约数
        /// </summary>
        /// <param name="leftValue"></param>
        /// <param name="rightValue"></param>
        /// <returns>最大公约数</returns>
        public static uint gcd(this uint leftValue, uint rightValue)
        {
            if (leftValue == 0 || rightValue == 0) log.Error.Throw(log.exceptionType.Null);
            for (rightValue %= leftValue; rightValue != 0; rightValue %= leftValue)
            {
                if ((leftValue %= rightValue) == 0) return rightValue;
            }
            return leftValue;
        }
        /// <summary>
        /// 乘方取余
        /// </summary>
        /// <param name="value">基数</param>
        /// <param name="power">幂</param>
        /// <param name="mod">模</param>
        /// <returns>余数</returns>
        public static uint powerMod(uint value, ulong power, uint mod)
        {
            if (mod == 0) log.Error.Throw(log.exceptionType.Null);
            ulong value64 = (ulong)value, mod64 = (ulong)mod, modValue = 1;
            if ((power & 1) != 0) modValue = value % mod;
            for (power >>= 1; power != 0; power >>= 1)
            {
                value64 = (value64 * value64) % mod64;
                if ((power & 1) != 0) modValue = (modValue * value64) % mod64;
            }
            return (uint)modValue;
        }

        /// <summary>
        /// 求平方根
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="mod">余数</param>
        /// <returns>平方根</returns>
        public static uint sqrt32(this ulong value, out ulong mod)
        {
            uint sqrtValue = 0, highValue = (uint)(value >> 32);
            if (highValue >= 0x40000000)
            {
                sqrtValue = 0x8000;
                highValue -= 0x40000000;
            }
            uint modHigh = (sqrtValue << 15) + 0x10000000;
            if (highValue >= modHigh)
            {
                sqrtValue |= 0x4000;
                highValue -= modHigh;
            }
            modHigh = (sqrtValue << 14) + 0x4000000;
            if (highValue >= modHigh)
            {
                sqrtValue |= 0x2000;
                highValue -= modHigh;
            }
            modHigh = (sqrtValue << 13) + 0x1000000;
            if (highValue >= modHigh)
            {
                sqrtValue |= 0x1000;
                highValue -= modHigh;
            }
            modHigh = (sqrtValue << 12) + 0x400000;
            if (highValue >= modHigh)
            {
                sqrtValue |= 0x800;
                highValue -= modHigh;
            }
            modHigh = (sqrtValue << 11) + 0x100000;
            if (highValue >= modHigh)
            {
                sqrtValue |= 0x400;
                highValue -= modHigh;
            }
            modHigh = (sqrtValue << 10) + 0x40000;
            if (highValue >= modHigh)
            {
                sqrtValue |= 0x200;
                highValue -= modHigh;
            }
            modHigh = (sqrtValue << 9) + 0x10000;
            if (highValue >= modHigh)
            {
                sqrtValue |= 0x100;
                highValue -= modHigh;
            }
            modHigh = (sqrtValue << 8) + 0x4000;
            if (highValue >= modHigh)
            {
                sqrtValue |= 0x80;
                highValue -= modHigh;
            }
            modHigh = (sqrtValue << 7) + 0x1000;
            if (highValue >= modHigh)
            {
                sqrtValue |= 0x40;
                highValue -= modHigh;
            }
            modHigh = (sqrtValue << 6) + 0x400;
            if (highValue >= modHigh)
            {
                sqrtValue |= 0x20;
                highValue -= modHigh;
            }
            modHigh = (sqrtValue << 5) + 0x100;
            if (highValue >= modHigh)
            {
                sqrtValue |= 0x10;
                highValue -= modHigh;
            }
            modHigh = (sqrtValue << 4) + 0x40;
            if (highValue >= modHigh)
            {
                sqrtValue |= 0x8;
                highValue -= modHigh;
            }
            modHigh = (sqrtValue << 3) + 0x10;
            if (highValue >= modHigh)
            {
                sqrtValue |= 0x4;
                highValue -= modHigh;
            }
            modHigh = (sqrtValue << 2) + 0x4;
            if (highValue >= modHigh)
            {
                sqrtValue |= 0x2;
                highValue -= modHigh;
            }
            modHigh = (sqrtValue << 1) + 0x1;
            if (highValue >= modHigh)
            {
                sqrtValue++;
                highValue -= modHigh;
            }
            sqrtValue <<= 8;
            if ((highValue & 0x10000) == 0)
            {
                highValue = (highValue << 16) | (((uint)value) >> 16);
                modHigh = (sqrtValue << 8) + 0x4000;
                if (highValue >= modHigh)
                {
                    sqrtValue |= 0x80;
                    highValue -= modHigh;
                }
            }
            else
            {
                highValue = ((highValue << 16) | (((uint)value) >> 16)) - ((sqrtValue << 8) + 0x4000);
                sqrtValue |= 0x80;
            }
            modHigh = (sqrtValue << 7) + 0x1000;
            if (highValue >= modHigh)
            {
                sqrtValue |= 0x40;
                highValue -= modHigh;
            }
            modHigh = (sqrtValue << 6) + 0x400;
            if (highValue >= modHigh)
            {
                sqrtValue |= 0x20;
                highValue -= modHigh;
            }
            modHigh = (sqrtValue << 5) + 0x100;
            if (highValue >= modHigh)
            {
                sqrtValue |= 0x10;
                highValue -= modHigh;
            }
            modHigh = (sqrtValue << 4) + 0x40;
            if (highValue >= modHigh)
            {
                sqrtValue |= 0x8;
                highValue -= modHigh;
            }
            modHigh = (sqrtValue << 3) + 0x10;
            if (highValue >= modHigh)
            {
                sqrtValue |= 0x4;
                highValue -= modHigh;
            }
            modHigh = (sqrtValue << 2) + 0x4;
            if (highValue >= modHigh)
            {
                sqrtValue |= 0x2;
                highValue -= modHigh;
            }
            modHigh = (sqrtValue << 1) + 0x1;
            if (highValue >= modHigh)
            {
                sqrtValue++;
                highValue -= modHigh;
            }
            sqrtValue <<= 4;
            if ((highValue & 0x1000000) == 0)
            {
                highValue = (highValue << 8) | ((byte)(value >> 8));
                modHigh = (sqrtValue << 4) + 0x40;
                if (highValue >= modHigh)
                {
                    sqrtValue |= 0x8;
                    highValue -= modHigh;
                }
            }
            else
            {
                highValue = ((highValue << 8) | ((byte)(value >> 8))) - ((sqrtValue << 4) + 0x40);
                sqrtValue |= 0x8;
            }
            modHigh = (sqrtValue << 3) + 0x10;
            if (highValue >= modHigh)
            {
                sqrtValue |= 0x4;
                highValue -= modHigh;
            }
            modHigh = (sqrtValue << 2) + 0x4;
            if (highValue >= modHigh)
            {
                sqrtValue |= 0x2;
                highValue -= modHigh;
            }
            modHigh = (sqrtValue << 1) + 0x1;
            if (highValue >= modHigh)
            {
                sqrtValue++;
                highValue -= modHigh;
            }
            sqrtValue <<= 2;
            if ((highValue & 0x10000000) == 0)
            {
                highValue = (highValue << 4) | (uint)(((byte)value) >> 4);
                modHigh = (sqrtValue << 2) + 0x4;
                if (highValue >= modHigh)
                {
                    sqrtValue |= 0x2;
                    highValue -= modHigh;
                }
            }
            else
            {
                highValue = ((highValue << 4) | (uint)(((byte)value) >> 4)) - ((sqrtValue << 2) + 0x4);
                sqrtValue |= 0x2;
            }
            modHigh = (sqrtValue << 1) + 0x1;
            if (highValue >= modHigh)
            {
                sqrtValue++;
                highValue -= modHigh;
            }
            //
            sqrtValue <<= 1;
            if ((highValue & 0x40000000) == 0)
            {
                highValue = (highValue << 2) | (uint)((((byte)value) >> 2) & 0x3);
                modHigh = (sqrtValue << 1) + 0x1;
                if (highValue >= modHigh)
                {
                    sqrtValue++;
                    highValue -= modHigh;
                }
            }
            else
            {
                highValue = ((highValue << 2) | (uint)((((byte)value) >> 2) & 0x3)) - ((sqrtValue << 1) + 0x1);
                sqrtValue++;
            }
            sqrtValue <<= 1;
            mod = ((ulong)highValue << 2) + (value & 0x3);
            value = (((ulong)sqrtValue) << 1) + 1;
            if (mod > value)
            {
                sqrtValue++;
                mod -= value;
            }
            //sqrtValue <<= 2;
            //mod = ((ulong)highValue << 4) + (value & 0xf);
            //value = ((ulong)sqrtValue << 2) + 0x4;
            //if (mod >= value)
            //{
            //    sqrtValue |= 0x2;
            //    mod -= value;
            //}
            //value = (((ulong)sqrtValue) << 1) + 1;
            //if (mod >= value)
            //{
            //    sqrtValue++;
            //    mod -= value;
            //}
            return sqrtValue;
        }
        //public static uint sqrt(this uint value)
        //{
        //    uint nextValue = value | (value >> 16);
        //    nextValue |= (nextValue >> 8);
        //    nextValue |= (nextValue >> 4);
        //    nextValue |= (nextValue >> 2);
        //    nextValue |= (nextValue >> 1);
        //    int bits = nextValue == uint.MaxValue ? 15 : (((power2_64bits[(nextValue + 1) % 67] + 1) >> 1) - 1);
        //    uint rightValue = value >> bits;
        //    nextValue = 1U << bits;//1:2
        //    nextValue = value / (rightValue = (rightValue + nextValue) >> 1);//4:5
        //    if (nextValue < rightValue) nextValue = value / (rightValue = (rightValue + nextValue) >> 1);//40:41
        //    if (nextValue < rightValue) nextValue = value / (rightValue = (rightValue + nextValue) >> 1);//2380:2381
        //    if (nextValue < rightValue) nextValue = value / (rightValue = (rightValue + nextValue) >> 1);//11333560:11333561
        //    return nextValue < rightValue ? nextValue : rightValue;
        //}
        //public static uint sqrt(ulong value)
        //{
        //    ulong nextValue = value | (value >> 32);
        //    nextValue |= (nextValue >> 16);
        //    nextValue |= (nextValue >> 8);
        //    nextValue |= (nextValue >> 4);
        //    nextValue |= (nextValue >> 2);
        //    nextValue |= (nextValue >> 1);
        //    int bits = nextValue == uint.MaxValue ? 31 : (((power2_64bits[(nextValue + 1) % 67] + 1) >> 1) - 1);
        //    if (bits >= 30)
        //    {
        //        ulong rightValue = value >> bits;
        //        nextValue = 1UL << bits;//1:2
        //        nextValue = value / (rightValue = (rightValue + nextValue) >> 1);//4:5
        //        if (nextValue < rightValue) nextValue = value / (rightValue = (rightValue + nextValue) >> 1);//40:41
        //        if (nextValue < rightValue) nextValue = value / (rightValue = (rightValue + nextValue) >> 1);//2380:2381
        //        if (nextValue < rightValue) nextValue = value / (rightValue = (rightValue + nextValue) >> 1);//11333560:11333561
        //        if (nextValue < rightValue) nextValue = value / (rightValue = (rightValue + nextValue) >> 1);
        //        if (nextValue > rightValue) nextValue = rightValue;
        //    }
        //    else
        //    {
        //        uint rightValue = (uint)(value >> bits), leftValue = 1U << bits;
        //        leftValue = (uint)(value / (rightValue = (rightValue + leftValue) >> 1));//4:5
        //        if (leftValue < rightValue) leftValue = (uint)(value / (rightValue = (rightValue + leftValue) >> 1));//40:41
        //        if (leftValue < rightValue) leftValue = (uint)(value / (rightValue = (rightValue + leftValue) >> 1));//2380:2381
        //        if (leftValue < rightValue) leftValue = (uint)(value / (rightValue = (rightValue + leftValue) >> 1));//11333560:11333561
        //        if (leftValue < rightValue) leftValue = (uint)(value / (rightValue = (rightValue + leftValue) >> 1));
        //        nextValue = leftValue < rightValue ? leftValue : rightValue;
        //    }
        //    return (uint)nextValue;
        //}
        /// <summary>
        /// 求平方根余数
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>余数(32bits)+平方根(32bits)</returns>
        public static ulong mSqrt(this ulong value)
        {
            if (value >= 2)
            {
                int bits = (value.bits() - 1) & 62;
                ulong result = 1, maxResult = 1UL << ((bits >> 1) + 1), subValue;
                for (value -= 1UL << bits, bits -= 2; bits >= 0 && value > maxResult; bits -= 2)
                {
                    result = ((result << 2) + 1) << bits;
                    subValue = (((value >> 1) - (result >> 1)) | ((((subValue = result - value - 1) | (subValue >> 1)) & long.MaxValue) - 1)) >> 63;
                    value -= (subValue - 1) & result;
                    result = (((result >> bits) - 1) >> 1) + (subValue ^ 1);
                }
                value = (value << 32) + (result << ((bits >> 1) + 1));
            }
            return value;
        }
        /// <summary>
        /// 十六进制字符串转字节数组
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public unsafe static byte[] parseHex(this string hex)
        {
            if (hex == null) return null;
            if (hex.Length == 0) return nullValue<byte>.Array;
            byte[] data = new byte[(hex.Length + 1) >> 1];
            fixed (char* hexFixed = hex)
            fixed (byte* dataFixed = data)
            {
                char* read = hexFixed;
                byte* write = dataFixed, end = dataFixed + data.Length;
                if ((hex.Length & 1) != 0) *write++ = (byte)ParseHex(*read++);
                while (write != end)
                {
                    *write++ = (byte)((ParseHex(*read) << 4) | ParseHex(*(read + 1)));
                    read += 2;
                }
            }
            return data;
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
        internal static uint ParseHex(uint hex)
        {
            uint value = hex - '0';
            return value >= 10 ? (hex & 0xdf) - ('0' + 'A' - '9' - 1) : value;
        }

        static numberExpand()
        {
            deBruijn32 = fastCSharp.number.GetDeBruijn32().Reference;
        }
    }
}
