using System;
using System.IO;
using System.Text;
using fastCSharp.code.cSharp;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 非托管内存数据流
    /// </summary>
    public unsafe abstract class unmanagedStreamBase : IDisposable
    {
        /// <summary>
        /// 默认容器初始尺寸
        /// </summary>
        public const int DefaultLength = 256;
        /// <summary>
        /// 数据指针
        /// </summary>
        internal pointer.size data;
        /// <summary>
        /// 数据
        /// </summary>
        public byte* Data
        {
            get { return data.Byte; }
        }
        /// <summary>
        /// 当前写入位置
        /// </summary>
        public byte* CurrentData
        {
            get { return data.Byte + length; }
        }
        /// <summary>
        /// 当前数据长度
        /// </summary>
        protected int length;
        /// <summary>
        /// 是否非托管内存数据
        /// </summary>
        internal bool IsUnmanaged { get; private set; }
        /// <summary>
        /// 非托管内存数据流
        /// </summary>
        /// <param name="length">容器初始尺寸</param>
        protected unmanagedStreamBase(int length)
        {
            data = unmanaged.Get(length > 0 ? length : DefaultLength, false);
            IsUnmanaged = true;
        }
        /// <summary>
        /// 非托管内存数据流
        /// </summary>
        /// <param name="data">无需释放的数据</param>
        /// <param name="dataLength">容器初始尺寸</param>
        protected unmanagedStreamBase(byte* data, int dataLength)
        {
            if (data == null || dataLength <= 0) log.Error.Throw(log.exceptionType.Null);
            this.data.Set(data, dataLength);
        }
        /// <summary>
        /// 内存数据流转换
        /// </summary>
        /// <param name="stream">内存数据流</param>
        protected internal unmanagedStreamBase(unmanagedStreamBase stream)
        {
            data = stream.data;
            length = stream.length;
            IsUnmanaged = stream.IsUnmanaged;
            stream.IsUnmanaged = false;
        }
        /// <summary>
        /// 释放数据容器
        /// </summary>
        public virtual void Dispose()
        {
            Close();
        }
        /// <summary>
        /// 释放数据容器
        /// </summary>
        public virtual void Close()
        {
            if (IsUnmanaged)
            {
                unmanaged.Free(ref data);
                IsUnmanaged = false;
            }
            length = 0;
            data.Null();
        }
        /// <summary>
        /// 清空数据
        /// </summary>
        public virtual void Clear()
        {
            length = 0;
        }
        /// <summary>
        /// 设置容器尺寸
        /// </summary>
        /// <param name="length">容器尺寸</param>
        protected void setStreamLength(int length)
        {
            if (length < DefaultLength) length = DefaultLength;
            pointer.size newData = unmanaged.Get(length, false);
            fastCSharp.unsafer.memory.Copy(data.data, newData.data, this.length);
            if (IsUnmanaged) unmanaged.Free(ref data);
            data = newData;
            IsUnmanaged = true;
        }
        ///// <summary>
        ///// 设置容器尺寸
        ///// </summary>
        ///// <param name="length">容器尺寸</param>
        //
        //protected void trySetStreamLength(int length)
        //{
        //    if ((length += this.length) > DataLength) setStreamLength(length);
        //}
        /// <summary>
        /// 预增数据流长度
        /// </summary>
        /// <param name="length">增加长度</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void prepLength(int length)
        {
            int newLength = length + this.length;
            if (newLength > data.sizeValue) setStreamLength(Math.Max(newLength, data.sizeValue << 1));
        }
        /// <summary>
        /// 重置当前数据长度
        /// </summary>
        /// <param name="length">当前数据长度</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void SetLength(int length)
        {
            if (length > 0)
            {
                if (length > data.sizeValue) setStreamLength(length);
                this.length = length;
            }
            else if (length == 0) this.length = 0;
            else log.Error.Throw(log.exceptionType.IndexOutOfRange);
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Write(char value)
        {
            prepLength(sizeof(char));
            *(char*)(data.Byte + length) = value;
            length += sizeof(char);
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="stream">数据</param>
        public void Write(unmanagedStreamBase stream)
        {
            if (stream != null)
            {
                prepLength(stream.length);
                fastCSharp.unsafer.memory.Copy(stream.data.data, data.Byte + length, stream.length);
                length += stream.length;
            }
        }
        /// <summary>
        /// 写字符串
        /// </summary>
        /// <param name="value">字符串</param>
        public unsafe void Write(string value)
        {
            if (value != null)
            {
                int length = value.Length << 1;
                prepLength(length);
                fastCSharp.unsafer.String.Copy(value, data.Byte + this.length);
                this.length += length;
            }
        }
        /// <summary>
        /// 写字符串
        /// </summary>
        /// <param name="value">字符串</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Write(subString value)
        {
            Write(ref value);
        }
        /// <summary>
        /// 写字符串
        /// </summary>
        /// <param name="value">字符串</param>
        public unsafe void Write(ref subString value)
        {
            if (value.Length != 0)
            {
                int length = value.Length << 1;
                prepLength(length);
                fixed (char* valueFixed = value.value) fastCSharp.unsafer.memory.Copy(valueFixed + value.StartIndex, data.Byte + this.length, length);
                this.length += length;
            }
        }
        /// <summary>
        /// 写字符串
        /// </summary>
        /// <param name="start">字符串起始位置</param>
        /// <param name="count">写入字符数</param>
        public unsafe void Write(char* start, int count)
        {
            if (start != null)
            {
                int length = count << 1;
                prepLength(length);
                fastCSharp.unsafer.memory.Copy(start, data.Byte + this.length, length);
                this.length += length;
            }
        }
        /// <summary>
        /// 写字符串
        /// </summary>
        /// <param name="value">字符串</param>
        /// <param name="index">起始位置</param>
        /// <param name="count">写入字符数</param>
        public unsafe void Write(string value, int index, int count)
        {
            array.range range = new array.range(value.length(), index, count);
            if (range.GetCount == count)
            {
                prepLength(count <<= 1);
                fixed (char* valueFixed = value)
                {
                    fastCSharp.unsafer.memory.Copy(valueFixed + index, data.Byte + length, count);
                }
                length += count;
            }
            else if (count != 0) log.Error.Throw(log.exceptionType.IndexOutOfRange);
        }
        /// <summary>
        /// 写字符串集合
        /// </summary>
        /// <param name="values">字符串集合</param>
        public unsafe void Write(params string[] values)
        {
            if (values != null)
            {
                int length = 0;
                foreach (string value in values)
                {
                    if (value != null) length += value.Length;
                }
                prepLength(length <<= 1);
                byte* write = data.Byte + this.length;
                foreach (string value in values)
                {
                    if (value != null)
                    {
                        fastCSharp.unsafer.String.Copy(value, write);
                        write += value.Length << 1;
                    }
                }
                this.length += length;
            }
        }
        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <returns>字符串</returns>
        public override unsafe string ToString()
        {
            return new string(data.Char, 0, length >> 1);
        }
        ///// <summary>
        ///// 重置数据
        ///// </summary>
        ///// <param name="data">数据</param>
        ///// <param name="length">数据字节长度</param>
        ///// <returns>原数据</returns>
        //internal virtual byte* GetReset(byte* data, int length)
        //{
        //    byte* value = Data;
        //    DataLength = length;
        //    this.length = 0;
        //    Data = data;
        //    return value;
        //}
        /// <summary>
        /// 重置数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="length">数据字节长度</param>
        public virtual void UnsafeReset(byte* data, int length)
        {
            if (IsUnmanaged)
            {
                unmanaged.Free(ref this.data);
                IsUnmanaged = false;
            }
            this.data.Set(data, length);
            this.length = 0;
        }
        /// <summary>
        /// 内存数据流转换
        /// </summary>
        /// <param name="stream">内存数据流</param>
        internal virtual void From(unmanagedStreamBase stream)
        {
            IsUnmanaged = stream.IsUnmanaged;
            data = stream.data;
            length = stream.length;
            stream.IsUnmanaged = false;
        }
        /// <summary>
        /// 转换成字符流
        /// </summary>
        /// <returns>内存字符流</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal charStream ToCharStream()
        {
            if ((length & 1) != 0) log.Error.Throw(log.exceptionType.IndexOutOfRange);
            data.sizeValue &= (int.MaxValue - 1);
            return new charStream(this);
        }
    }
    /// <summary>
    /// 非托管内存数据流
    /// </summary>
    public unsafe class unmanagedStream : unmanagedStreamBase
    {
        /// <summary>
        /// 原始偏移位置
        /// </summary>
        protected int offset;
        /// <summary>
        /// 相对于原始偏移位置的数据长度
        /// </summary>
        public int OffsetLength
        {
            get { return offset + length; }
        }
        /// <summary>
        /// 当前数据长度
        /// </summary>
        public int Length 
        {
            get { return length; }
        }
        /// <summary>
        /// 非托管内存数据流
        /// </summary>
        /// <param name="length">容器初始尺寸</param>
        public unmanagedStream(int length = DefaultLength) : base(length) { }
        /// <summary>
        /// 非托管内存数据流
        /// </summary>
        /// <param name="data">无需释放的数据</param>
        /// <param name="dataLength">容器初始尺寸</param>
        public unmanagedStream(byte* data, int dataLength) : base(data, dataLength) { }
        /// <summary>
        /// 内存数据流转换
        /// </summary>
        /// <param name="stream">内存数据流</param>
        internal unmanagedStream(unmanagedStreamBase stream) : base(stream) { }
        /// <summary>
        /// 预增数据流长度
        /// </summary>
        /// <param name="length">增加长度</param>
        public virtual void PrepLength(int length)
        {
            prepLength(length);
        }
        /// <summary>
        /// 预增数据流结束
        /// </summary>
        public virtual void PrepLength() { }
        /// <summary>
        /// 增加数据流长度
        /// </summary>
        /// <param name="length">增加长度</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void UnsafeAddLength(int length)
        {
            this.length += length;
        }
        /// <summary>
        /// 设置数据流长度
        /// </summary>
        /// <param name="length">数据流长度</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void UnsafeSetLength(int length)
        {
            this.length = length;
        }
        /// <summary>
        /// 增加数据流长度
        /// </summary>
        /// <param name="length">增加长度</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void UnsafeAddSerializeLength(int length)
        {
            this.length += length + (-length & 3);
            //if (Stream.length > Stream.DataLength) log.Error.ThrowReal(Stream.length.toString() + " > " + Stream.DataLength.toString(), true, false);
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Write(bool value)
        {
            if (length == data.sizeValue) setStreamLength(length << 1);
            data.Byte[length++] = (byte)(value ? 1 : 0);
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Write(byte value)
        {
            if (length == data.sizeValue) setStreamLength(length << 1);
            data.Byte[length++] = value;
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Write(sbyte value)
        {
            if (length == data.sizeValue) setStreamLength(length << 1);
            data.Byte[length++] = (byte)value;
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Write(short value)
        {
            prepLength(sizeof(short));
            *(short*)(data.Byte + length) = value;
            length += sizeof(short);
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Write(ushort value)
        {
            prepLength(sizeof(ushort));
            *(ushort*)(data.Byte + length) = value;
            length += sizeof(ushort);
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe void Write(int value)
        {
            prepLength(sizeof(int));
            *(int*)(data.Byte + length) = value;
            length += sizeof(int);
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe void Write(uint value)
        {
            prepLength(sizeof(uint));
            *(uint*)(data.Byte + length) = value;
            length += sizeof(uint);
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe void Write(long value)
        {
            prepLength(sizeof(long));
            *(long*)(data.Byte + length) = value;
            length += sizeof(long);
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe void Write(ulong value)
        {
            prepLength(sizeof(ulong));
            *(ulong*)(data.Byte + length) = value;
            length += sizeof(ulong);
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe void Write(DateTime value)
        {
            prepLength(sizeof(long));
            *(long*)(data.Byte + length) = value.Ticks;
            length += sizeof(long);
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe void Write(float value)
        {
            prepLength(sizeof(float));
            *(float*)(data.Byte + length) = value;
            length += sizeof(float);
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe void Write(double value)
        {
            prepLength(sizeof(double));
            *(double*)(data.Byte + length) = value;
            length += sizeof(double);
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe void Write(decimal value)
        {
            prepLength(sizeof(decimal));
            *(decimal*)(data.Byte + length) = value;
            length += sizeof(decimal);
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe void Write(Guid value)
        {
            prepLength(sizeof(Guid));
            *(Guid*)(data.Byte + length) = value;
            length += sizeof(Guid);
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="data">数据</param>
        public void Write(byte[] data)
        {
            if (data != null)
            {
                prepLength(data.Length);
                fastCSharp.unsafer.memory.Copy(data, this.data.Byte + length, data.Length);
                length += data.Length;
            }
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="data">数据</param>
        internal void WriteNotNull(byte[] data)
        {
            prepLength(data.Length);
            fastCSharp.unsafer.memory.Copy(data, this.data.Byte + length, data.Length);
            length += data.Length;
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="data">数据</param>
        internal void SimpleWriteNotNull(byte[] data)
        {
            prepLength(data.Length);
            fastCSharp.unsafer.memory.SimpleCopy(data, this.data.Byte + length, data.Length);
            length += data.Length;
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="index">起始位置</param>
        /// <param name="count">写入字节数</param>
        public void Write(byte[] data, int index, int count)
        {
            array.range range = new array.range(data.length(), index, count);
            if (range.GetCount == count)
            {
                prepLength(count);
                fixed (byte* dataFixed = data)
                {
                    fastCSharp.unsafer.memory.Copy(dataFixed + range.SkipCount, this.data.Byte + length, count);
                }
                length += count;
            }
            else if (count != 0) log.Error.Throw(log.exceptionType.IndexOutOfRange);
        }
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="data">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Write(subArray<byte> data)
        {
            Write(ref data);
        }
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="data">数据</param>
        public void Write(ref subArray<byte> data)
        {
            int count = data.length;
            if (count != 0)
            {
                prepLength(count);
                fixed (byte* dataFixed = data.array)
                {
                    fastCSharp.unsafer.memory.Copy(dataFixed + data.startIndex, this.data.Byte + length, count);
                }
                length += count;
            }
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="length">数据长度</param>
        public unsafe void Write(byte* value, int length)
        {
            if (value != null)
            {
                prepLength(length);
                fastCSharp.unsafer.memory.Copy(value, data.Byte + this.length, length);
                this.length += length;
            }
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void UnsafeWrite(bool value)
        {
            data.Byte[length++] = (byte)(value ? 1 : 0);
            //if (length > DataLength) log.Error.ThrowReal(length.toString() + " > " + DataLength.toString(), true, false);
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void UnsafeWrite(byte value)
        {
            data.Byte[length++] = value;
            //if (length > DataLength) log.Error.ThrowReal(length.toString() + " > " + DataLength.toString(), true, false);
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void UnsafeWrite(sbyte value)
        {
            data.Byte[length++] = (byte)value;
            //if (length > DataLength) log.Error.ThrowReal(length.toString() + " > " + DataLength.toString(), true, false);
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void UnsafeWrite(short value)
        {
            *(short*)CurrentData = value;
            length += sizeof(short);
            //if (length > DataLength) log.Error.ThrowReal(length.toString() + " > " + DataLength.toString(), true, false);
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void UnsafeWrite(ushort value)
        {
            *(ushort*)CurrentData = value;
            length += sizeof(ushort);
            //if (length > DataLength) log.Error.ThrowReal(length.toString() + " > " + DataLength.toString(), true, false);
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
            //if (length > DataLength) log.Error.ThrowReal(length.toString() + " > " + DataLength.toString(), true, false);
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void UnsafeWrite(int value)
        {
            *(int*)(data.Byte + length) = value;
            length += sizeof(int);
            //if (length > DataLength) log.Error.ThrowReal(length.toString() + " > " + DataLength.toString(), true, false);
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe void UnsafeWrite(uint value)
        {
            *(uint*)CurrentData = value;
            length += sizeof(uint);
            //if (length > DataLength) log.Error.ThrowReal(length.toString() + " > " + DataLength.toString(), true, false);
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe void UnsafeWrite(long value)
        {
            *(long*)CurrentData = value;
            length += sizeof(long);
            //if (length > DataLength) log.Error.ThrowReal(length.toString() + " > " + DataLength.toString(), true, false);
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe void UnsafeWrite(ulong value)
        {
            *(ulong*)CurrentData = value;
            length += sizeof(ulong);
            //if (length > DataLength) log.Error.ThrowReal(length.toString() + " > " + DataLength.toString(), true, false);
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe void UnsafeWrite(DateTime value)
        {
            *(long*)CurrentData = value.Ticks;
            length += sizeof(long);
            //if (length > DataLength) log.Error.ThrowReal(length.toString() + " > " + DataLength.toString(), true, false);
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe void UnsafeWrite(float value)
        {
            *(float*)CurrentData = value;
            length += sizeof(float);
            //if (length > DataLength) log.Error.ThrowReal(length.toString() + " > " + DataLength.toString(), true, false);
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe void UnsafeWrite(double value)
        {
            *(double*)CurrentData = value;
            length += sizeof(double);
            //if (length > DataLength) log.Error.ThrowReal(length.toString() + " > " + DataLength.toString(), true, false);
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe void UnsafeWrite(decimal value)
        {
            *(decimal*)CurrentData = value;
            length += sizeof(decimal);
            //if (length > DataLength) log.Error.ThrowReal(length.toString() + " > " + DataLength.toString(), true, false);
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="value">数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe void UnsafeWrite(Guid value)
        {
            *(Guid*)CurrentData = value;
            length += sizeof(Guid);
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="data">数据,不能为null</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void UnsafeWrite(byte[] data)
        {
            fastCSharp.unsafer.memory.Copy(data, CurrentData, data.Length);
            length += data.Length;
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="stream">数据,不能为null</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void UnsafeWrite(unmanagedStream stream)
        {
            fastCSharp.unsafer.memory.Copy(stream.data.data, CurrentData, stream.length);
            length += stream.length;
        }
        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <returns>字节数组</returns>
        public unsafe byte[] GetArray()
        {
            if (length == 0) return nullValue<byte>.Array;
            byte[] data = new byte[length];
            fastCSharp.unsafer.memory.Copy(this.data.data, data, length);
            return data;
        }
        ///// <summary>
        ///// 转换成字节数组
        ///// </summary>
        ///// <param name="index">起始位置</param>
        ///// <param name="count">字节数</param>
        ///// <returns>字节数组</returns>
        //public byte[] GetArray(int index, int count)
        //{
        //    array.range range = new array.range(length, index, count);
        //    if (count == range.GetCount)
        //    {
        //        byte[] data = new byte[count];
        //        fastCSharp.unsafer.memory.Copy(Data + index, data, count);
        //        return data;
        //    }
        //    else if (count == 0) return null;
        //    log.Default.Throw(log.exceptionType.IndexOutOfRange);
        //    return null;
        //}
        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <param name="copyIndex">复制起始位置</param>
        /// <returns>字节数组</returns>
        internal unsafe byte[] GetArray(int copyIndex)
        {
            byte[] data = new byte[length];
            fixed (byte* dataFixed = data) fastCSharp.unsafer.memory.Copy(this.data.Byte + copyIndex, dataFixed + copyIndex, length - copyIndex);
            return data;
        }
        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <param name="minSize">复制起始位置</param>
        /// <returns>字节数组</returns>
        public unsafe byte[] GetSizeArray(int minSize)
        {
            byte[] data = new byte[length < minSize ? minSize : length];
            fastCSharp.unsafer.memory.Copy(this.data.data, data, length);
            return data;
        }
        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <param name="copyIndex">复制起始位置</param>
        /// <param name="minSize">复制起始位置</param>
        /// <returns>字节数组</returns>
        internal unsafe byte[] GetSizeArray(int copyIndex, int minSize)
        {
            byte[] data = new byte[length < minSize ? minSize : length];
            fixed (byte* dataFixed = data) fastCSharp.unsafer.memory.Copy(this.data.Byte + copyIndex, dataFixed + copyIndex, length - copyIndex);
            return data;
        }
        ///// <summary>
        ///// 重置数据
        ///// </summary>
        ///// <param name="data">数据</param>
        ///// <param name="length">数据字节长度</param>
        ///// <returns>原数据</returns>
        //internal override byte* GetReset(byte* data, int length)
        //{
        //    data = base.GetReset(data, length);
        //    offset = 0;
        //    return data;
        //}
        /// <summary>
        /// 重置数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="length">数据字节长度</param>
        public override void UnsafeReset(byte* data, int length)
        {
            base.UnsafeReset(data, length);
            offset = 0;
        }
        /// <summary>
        /// 内存数据流转换
        /// </summary>
        /// <param name="stream">内存数据流</param>
        internal override void From(unmanagedStreamBase stream)
        {
            base.From(stream);
            offset = 0;
        }
    }
}