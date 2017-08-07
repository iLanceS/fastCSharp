using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace fastCSharp
{
    /// <summary>
    /// 内存字符流
    /// </summary>
    public sealed unsafe class charStream : unmanagedStreamBase
    {
        /// <summary>
        /// 数据
        /// </summary>
        public char* Char
        {
            get { return data.Char; }
        }
        /// <summary>
        /// 当前写入位置
        /// </summary>
        public char* CurrentChar
        {
            get { return (char*)(data.Byte + length); }
        }
        /// <summary>
        /// 当前数据长度
        /// </summary>
        public int Length
        {
            get { return length >> 1; }
        }
        /// <summary>
        /// 内存数据流
        /// </summary>
        /// <param name="length">容器初始尺寸</param>
        public charStream(int length = DefaultLength) : base(length << 1) { }
        /// <summary>
        /// 非托管内存数据流
        /// </summary>
        /// <param name="data">无需释放的数据</param>
        /// <param name="length">容器初始尺寸</param>
        public charStream(char* data, int length) : base((byte*)data, length << 1) { }
        /// <summary>
        /// 内存数据流转换
        /// </summary>
        /// <param name="stream">内存数据流</param>
        internal charStream(unmanagedStreamBase stream) : base(stream) { }
        /// <summary>
        /// 预增数据流长度
        /// </summary>
        /// <param name="length">增加长度</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void PrepLength(int length)
        {
            prepLength(length << 1);
        }
        /// <summary>
        /// 预增数据流长度
        /// </summary>
        /// <param name="length">增加长度</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal char* GetPrepLengthCurrent(int length)
        {
            prepLength(length << 1);
            return (char*)(data.Byte + this.length);
        }
        /// <summary>
        /// 重置当前数据长度
        /// </summary>
        /// <param name="length">当前数据长度</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public new void SetLength(int length)
        {
            base.SetLength(length << 1);
        }
        /// <summary>
        /// 增加数据流长度
        /// </summary>
        /// <param name="length">增加长度</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void UnsafeAddLength(int length)
        {
            this.length += length << 1;
        }
        /// <summary>
        /// 减少数据流长度
        /// </summary>
        /// <param name="length">减少长度</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void UnsafeFreeLength(int length)
        {
            this.length -= length << 1;
        }
        /// <summary>
        /// 设置数据流长度
        /// </summary>
        /// <param name="length">数据流长度</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void UnsafeSetLength(int length)
        {
            this.length = length << 1;
        }
        /// <summary>
        /// 写字符串
        /// </summary>
        /// <param name="value">字符串</param>
        public unsafe void WriteNotNull(string value)
        {
            int length = value.Length << 1;
            prepLength(length);
            fastCSharp.unsafer.String.Copy(value, data.Byte + this.length);
            this.length += length;
        }
        /// <summary>
        /// 写字符串
        /// </summary>
        /// <param name="value">字符串</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe void SimpleWriteNotNull(string value)
        {
            fixed (char* valueFixed = value) SimpleWriteNotNull(valueFixed, value.Length);
        }
        /// <summary>
        /// 写字符串
        /// </summary>
        /// <param name="start">字符串起始位置</param>
        /// <param name="count">写入字符数</param>
        internal unsafe void SimpleWriteNotNull(char* start, int count)
        {
            int length = count << 1;
            prepLength(length + 6);
            fastCSharp.unsafer.memory.UnsafeSimpleCopy(start, (char*)(data.Byte + this.length), count);
            this.length += length;
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void UnsafeWrite(char value)
        {
            *(char*)CurrentData = value;
            length += sizeof(char);
        }
        /// <summary>
        /// 写字符串
        /// </summary>
        /// <param name="value">字符串</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe void UnsafeWrite(string value)
        {
            fastCSharp.unsafer.String.Copy(value, data.Byte + length);
            length += value.Length << 1;
        }
        /// <summary>
        /// 写字符串(无需预增数据流)
        /// </summary>
        /// <param name="value">字符串,长度必须>0</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe void UnsafeSimpleWrite(string value)
        {
            fastCSharp.unsafer.String.UnsafeSimpleCopy(value, data.Byte + this.length);
            this.length += value.Length << 1;
        }
        /// <summary>
        /// 写字符串
        /// </summary>
        /// <param name="start">字符串起始位置,不能为null</param>
        /// <param name="count">写入字符数，必须>=0</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe void UnsafeWrite(char* start, int count)
        {
            fastCSharp.unsafer.memory.Copy(start, CurrentData, count <<= 1);
            length += count;
        }
        /// <summary>
        /// 写字符串
        /// </summary>
        /// <param name="start">字符串起始位置,不能为null</param>
        /// <param name="count">写入字符数，必须>0</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe void UnsafeSimpleWrite(char* start, int count)
        {
            fastCSharp.unsafer.memory.UnsafeSimpleCopy(start, (char*)(data.Byte + length), count);
            length += count << 1;
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="stream">数据,不能为null</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void UnsafeWrite(charStream stream)
        {
            fastCSharp.unsafer.memory.Copy(stream.CurrentData, CurrentData, stream.length);
            length += stream.length;
        }
    }
}
