using System;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 指针(因为指针无法静态初始化)
    /// </summary>
    public unsafe struct pointer : IEquatable<pointer>
    {
        /// <summary>
        /// 带长度的指针
        /// </summary>
        public struct size
        {
            /// <summary>
            /// 指针
            /// </summary>
            internal void* data;
            /// <summary>
            /// 指针
            /// </summary>
            public void* Data
            {
                get { return data; }
            }
            /// <summary>
            /// 指针
            /// </summary>
            public pointer Pointer
            {
                get { return new pointer { Data = data }; }
            }
            /// <summary>
            /// 引用
            /// </summary>
            public reference Reference
            {
                get { return new reference { Data = Data }; }
            }
            /// <summary>
            /// 字节长度
            /// </summary>
            internal int sizeValue;
            /// <summary>
            /// 字节长度
            /// </summary>
            public int Size
            {
                get { return sizeValue; }
            }
            /// <summary>
            /// 字节指针
            /// </summary>
            public byte* Byte
            {
                get { return (byte*)data; }
            }
            /// <summary>
            /// 字符指针
            /// </summary>
            public char* Char
            {
                get { return (char*)data; }
            }
            /// <summary>
            /// 字符长度
            /// </summary>
            public int CharSize
            {
                get { return sizeValue >> 1; }
            }
            /// <summary>
            /// 整形指针
            /// </summary>
            public int* Int
            {
                get { return (int*)data; }
            }
            /// <summary>
            /// 设置指针数据
            /// </summary>
            /// <param name="data"></param>
            /// <param name="size"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Set(void* data, int size)
            {
                this.data = data;
                sizeValue = size;
            }
            /// <summary>
            /// 清空数据
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Null()
            {
                data = null;
                sizeValue = 0;
            }

        }
        /// <summary>
        /// 引用，用于静态变量
        /// </summary>
        public sealed class reference
        {
            /// <summary>
            /// 指针
            /// </summary>
            internal void* Data;
            /// <summary>
            /// 字节指针
            /// </summary>
            public byte* Byte
            {
                get { return (byte*)Data; }
            }
            /// <summary>
            /// 字节指针
            /// </summary>
            public sbyte* SByte
            {
                get { return (sbyte*)Data; }
            }
            /// <summary>
            /// 字符指针
            /// </summary>
            public char* Char
            {
                get { return (char*)Data; }
            }
            /// <summary>
            /// 整形指针
            /// </summary>
            public short* Short
            {
                get { return (short*)Data; }
            }
            /// <summary>
            /// 整形指针
            /// </summary>
            public ushort* UShort
            {
                get { return (ushort*)Data; }
            }
            /// <summary>
            /// 整形指针
            /// </summary>
            public int* Int
            {
                get { return (int*)Data; }
            }
            /// <summary>
            /// 整形指针
            /// </summary>
            public uint* UInt
            {
                get { return (uint*)Data; }
            }
            /// <summary>
            /// 整形指针
            /// </summary>
            public long* Long
            {
                get { return (long*)Data; }
            }
            /// <summary>
            /// 整形指针
            /// </summary>
            public ulong* ULong
            {
                get { return (ulong*)Data; }
            }
            /// <summary>
            /// 浮点指针
            /// </summary>
            public float* Float
            {
                get { return (float*)Data; }
            }
            /// <summary>
            /// 双精度浮点指针
            /// </summary>
            public double* Double
            {
                get { return (double*)Data; }
            }
            /// <summary>
            /// 日期指针
            /// </summary>
            public DateTime* DateTime
            {
                get { return (DateTime*)Data; }
            }
        }
        /// <summary>
        /// 指针
        /// </summary>
        internal void* Data;
        /// <summary>
        /// 指针
        /// </summary>
        /// <param name="data"></param>
        public pointer(void* data)
        {
            Data = data;
        }
        /// <summary>
        /// 引用
        /// </summary>
        public reference Reference
        {
            get { return new reference { Data = Data }; }
        }
        /// <summary>
        /// 字节指针
        /// </summary>
        public byte* Byte
        {
            get { return (byte*)Data; }
        }
        /// <summary>
        /// 字节指针
        /// </summary>
        public sbyte* SByte
        {
            get { return (sbyte*)Data; }
        }
        /// <summary>
        /// 字符指针
        /// </summary>
        public char* Char
        {
            get { return (char*)Data; }
        }
        /// <summary>
        /// 整形指针
        /// </summary>
        public short* Short
        {
            get { return (short*)Data; }
        }
        /// <summary>
        /// 整形指针
        /// </summary>
        public ushort* UShort
        {
            get { return (ushort*)Data; }
        }
        /// <summary>
        /// 整形指针
        /// </summary>
        public int* Int
        {
            get { return (int*)Data; }
        }
        /// <summary>
        /// 整形指针
        /// </summary>
        public uint* UInt
        {
            get { return (uint*)Data; }
        }
        /// <summary>
        /// 整形指针
        /// </summary>
        public long* Long
        {
            get { return (long*)Data; }
        }
        /// <summary>
        /// 整形指针
        /// </summary>
        public ulong* ULong
        {
            get { return (ulong*)Data; }
        }
        /// <summary>
        /// 浮点指针
        /// </summary>
        public float* Float
        {
            get { return (float*)Data; }
        }
        /// <summary>
        /// 双精度浮点指针
        /// </summary>
        public double* Double
        {
            get { return (double*)Data; }
        }
        /// <summary>
        /// 日期指针
        /// </summary>
        public DateTime* DateTime
        {
            get { return (DateTime*)Data; }
        }
        /// <summary>
        /// HASH值
        /// </summary>
        /// <returns>HASH值</returns>
        public override int GetHashCode()
        {
            if (fastCSharp.pub.MemoryBits == 64)
            {
                return (int)((long)Data >> 3) ^ (int)((long)Data >> 35);
            }
            return (int)Data >> 2;
        }
        /// <summary>
        /// 指针比较
        /// </summary>
        /// <param name="obj">待比较指针</param>
        /// <returns>指针是否相等</returns>
        public override bool Equals(object obj)
        {
            return Equals((pointer)obj);
        }
        /// <summary>
        /// 指针比较
        /// </summary>
        /// <param name="other">待比较指针</param>
        /// <returns>指针是否相等</returns>
        public bool Equals(pointer other)
        {
            return Data == other.Data;
        }
    }
}
