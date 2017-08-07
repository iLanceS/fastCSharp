using System;
using fastCSharp.code.cSharp;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Threading;

namespace fastCSharp
{
    /// <summary>
    /// 会话标识
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct sessionId : IEquatable<sessionId>
    {
        /// <summary>
        /// 低32位
        /// </summary>
        [FieldOffset(0)]
        private uint bit0;
        /// <summary>
        /// 32-64位
        /// </summary>
        [FieldOffset(sizeof(uint))]
        private uint bit32;
        /// <summary>
        /// 64-96位
        /// </summary>
        [FieldOffset(sizeof(ulong))]
        private uint bit64;
        /// <summary>
        /// 96-128位
        /// </summary>
        [FieldOffset(sizeof(ulong) + sizeof(uint))]
        private uint bit96;
        /// <summary>
        /// 128-160位
        /// </summary>
        [FieldOffset(sizeof(ulong) * 2)]
        private uint bit128;
        /// <summary>
        /// 160-192位
        /// </summary>
        [FieldOffset(sizeof(ulong) * 2 + sizeof(uint))]
        private uint bit160;
        /// <summary>
        /// 192-224位
        /// </summary>
        [FieldOffset(sizeof(ulong) * 3)]
        private uint bit192;
        /// <summary>
        /// 高32位
        /// </summary>
        [FieldOffset(sizeof(ulong) * 3 + sizeof(uint))]
        private uint bit224;
        /// <summary>
        /// 服务器时间戳
        /// </summary>
        [FieldOffset(0)]
        internal ulong Ticks;
        /// <summary>
        /// 服务器自增标识
        /// </summary>
        [FieldOffset(sizeof(ulong))]
        internal ulong Identity;
        /// <summary>
        /// 索引
        /// </summary>
        [FieldOffset(sizeof(ulong))]
        internal int Index;
        /// <summary>
        /// 索引标识
        /// </summary>
        [FieldOffset(sizeof(ulong) + sizeof(uint))]
        internal uint IndexIdentity;
        /// <summary>
        /// 随机数低64位
        /// </summary>
        [FieldOffset(sizeof(ulong) * 2)]
        internal ulong Low;
        /// <summary>
        /// 随机数高64位
        /// </summary>
        [FieldOffset(sizeof(ulong) * 3)]
        internal ulong High;
        /// <summary>
        /// 是否为空
        /// </summary>
        public bool IsNull
        {
            get { return Low == 0; }
        }
        /// <summary>
        /// 空值判断参数
        /// </summary>
        internal ulong CookieValue
        {
            get { return Ticks | Low; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(sessionId other)
        {
            return ((Low ^ other.Low) | (High ^ other.High)) == 0 && ((Identity ^ other.Identity) | (Ticks ^ other.Ticks)) == 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns>相等返回0</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal ulong Equals(ref sessionId other)
        {
            return (Low ^ other.Low) | (High ^ other.High) | (Identity ^ other.Identity) | (Ticks ^ other.Ticks);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            ulong value = Low ^ High ^ Ticks ^ Identity;
            return (int)((uint)value ^ (uint)(value >> 32));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return Equals((sessionId)obj);
        }
        /// <summary>
        /// 随机数高位
        /// </summary>
        private static ulong highRandom = fastCSharp.random.Default.SecureNextULong();
        /// <summary>
        /// 随机数自增标识
        /// </summary>
        private static long identityRandom = (long)fastCSharp.random.Default.SecureNextULong();
        /// <summary>
        /// 清空数据
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Null()
        {
            Ticks = Low = 0;
        }
        /// <summary>
        /// 重置会话标识
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe void New()
        {
            Low = fastCSharp.random.Default.SecureNextULongNotZero();
            Ticks = (ulong)pub.StartTime.Ticks;
            highRandom ^= Low;
            Identity = (ulong)Interlocked.Increment(ref identityRandom);
            High = (highRandom << 11) | (highRandom >> 53);
        }
        /// <summary>
        /// 重置会话标识
        /// </summary>
        internal void NewNoIndex()
        {
            Low = fastCSharp.random.Default.SecureNextULongNotZero();
            Ticks = (ulong)pub.StartTime.Ticks;
            highRandom ^= Low;
            High = (highRandom << 11) | (highRandom >> 53);
        }
        /// <summary>
        /// Cookie解析
        /// </summary>
        /// <param name="data"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe void FromCookie(ref subArray<byte> data)
        {
            if (data.length == 64)
            {
                fixed (byte* dataFixed = data.array)
                {
                    byte* start = dataFixed + data.startIndex;
                    bit32 = number.ParseHex32(start);
                    bit0 = number.ParseHex32(start + 8);

                    bit96 = number.ParseHex32(start + 16);
                    bit64 = number.ParseHex32(start + 24);

                    bit160 = number.ParseHex32(start + 32);
                    bit128 = number.ParseHex32(start + 40);

                    bit224 = number.ParseHex32(start + 48);
                    bit192 = number.ParseHex32(start + 56);
                }
                return;
            }
            Low = 1;
            Ticks = 0;
        }
        /// <summary>
        /// 转换成16进制字符串
        /// </summary>
        /// <returns></returns>
        internal unsafe byte[] ToCookie()
        {
            byte[] data = new byte[64];
            fixed (byte* dataFixed = data)
            {
                number.UnsafeToHex16(Ticks, dataFixed);
                number.UnsafeToHex16(Identity, dataFixed + 16);
                number.UnsafeToHex16(Low, dataFixed + 32);
                number.UnsafeToHex16(High, dataFixed + 48);
            }
            return data;
        }
        /// <summary>
        /// 转换成16进制字符串
        /// </summary>
        /// <returns></returns>
        public unsafe string ToHexString()
        {
            string hex = fastCSharp.String.FastAllocateString(64);
            fixed (char* hexFixed = hex)
            {
                number.UnsafeToHex16(Ticks, hexFixed);
                number.UnsafeToHex16(Identity, hexFixed + 16);
                number.UnsafeToHex16(Low, hexFixed + 32);
                number.UnsafeToHex16(High, hexFixed + 48);
            }
            return hex;
        }
        /// <summary>
        /// 判断是否匹配16进制字符串
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public unsafe bool CheckHex(string hex)
        {
            if (hex.Length == 64)
            {
                fixed (char* hexFixed = hex)
                {
                    if (number.UnsafeCheckHex(High, hexFixed + 48) == 0 && (number.UnsafeCheckHex(Low, hexFixed + 32) | number.UnsafeCheckHex(Identity, hexFixed + 16) | number.UnsafeCheckHex(Ticks, hexFixed)) == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="serializer">对象序列化器</param>
        /// <param name="value"></param>
        [fastCSharp.emit.dataSerialize.custom]
        private unsafe static void serialize(fastCSharp.emit.dataSerializer serializer, sessionId value)
        {
            unmanagedStream stream = serializer.Stream;
            stream.PrepLength(sizeof(ulong) * 4);
            byte* write = stream.CurrentData;
            *(ulong*)write = value.Ticks;
            *(ulong*)(write + sizeof(ulong)) = value.Identity;
            *(ulong*)(write + sizeof(ulong) * 2) = value.Low;
            *(ulong*)(write + sizeof(ulong) * 3) = value.High;
            stream.UnsafeAddLength(sizeof(ulong) * 4);
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="deSerializer">对象反序列化器</param>
        /// <param name="value"></param>
        [fastCSharp.emit.dataSerialize.custom]
        private unsafe static void deSerialize(fastCSharp.emit.dataDeSerializer deSerializer, ref sessionId value)
        {
            if (deSerializer.VerifyRead(sizeof(ulong) * 4))
            {
                byte* dataStart = deSerializer.Read;
                value.Ticks = *(ulong*)(dataStart - sizeof(ulong) * 4);
                value.Identity = *(ulong*)(dataStart - sizeof(ulong) * 3);
                value.Low = *(ulong*)(dataStart - sizeof(ulong) * 2);
                value.High = *(ulong*)(dataStart - sizeof(ulong));
            }
        }
    }
}
