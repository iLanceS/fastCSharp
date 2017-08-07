using System;
using fastCSharp.code;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace fastCSharp.emit
{
    /// <summary>
    /// 二进制数据序列化
    /// </summary>
    public unsafe abstract class binaryDeSerializer
    {
        /// <summary>
        /// 反序列化状态
        /// </summary>
        public enum deSerializeState : byte
        {
            /// <summary>
            /// 成功
            /// </summary>
            Success,
            /// <summary>
            /// 数据不可识别
            /// </summary>
            UnknownData,
            /// <summary>
            /// 成员位图检测失败
            /// </summary>
            MemberMap,
            /// <summary>
            /// 成员位图类型错误
            /// </summary>
            MemberMapType,
            /// <summary>
            /// 成员位图数量验证失败
            /// </summary>
            MemberMapVerify,
            /// <summary>
            /// 头部数据不匹配
            /// </summary>
            HeaderError,
            /// <summary>
            /// 结束验证错误
            /// </summary>
            EndVerify,
            /// <summary>
            /// 数据完整检测失败
            /// </summary>
            FullDataError,
            /// <summary>
            /// 没有命中历史对象
            /// </summary>
            NoPoint,
            /// <summary>
            /// 数据长度不足
            /// </summary>
            IndexOutOfRange,
            /// <summary>
            /// 不支持对象null解析检测失败
            /// </summary>
            NotNull,
            /// <summary>
            /// 成员索引检测失败
            /// </summary>
            MemberIndex,
            /// <summary>
            /// JSON反序列化失败
            /// </summary>
            JsonError,
            /// <summary>
            /// 自定义序列化失败
            /// </summary>
            Custom,
        }
        /// <summary>
        /// 数组位图
        /// </summary>
        internal struct arrayMap
        {
            /// <summary>
            /// 当前位
            /// </summary>
            public uint Bit;
            /// <summary>
            /// 当前位图
            /// </summary>
            public uint Map;
            /// <summary>
            /// 当前读取位置
            /// </summary>
            public byte* Read;
            /// <summary>
            /// 数组位图
            /// </summary>
            /// <param name="read">当前读取位置</param>
            public arrayMap(byte* read)
            {
                Read = read;
                Bit = 1;
                Map = 0;
            }
            /// <summary>
            /// 数组位图
            /// </summary>
            /// <param name="read">当前读取位置</param>
            /// <param name="bit">当前位</param>
            public arrayMap(byte* read, uint bit)
            {
                Read = read;
                Bit = bit;
                Map = 0;
            }
            /// <summary>
            /// 获取位图数据
            /// </summary>
            /// <returns></returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public uint Next()
            {
                if (Bit == 1)
                {
                    Map = *(uint*)Read;
                    Bit = 1U << 31;
                    Read += sizeof(uint);
                }
                else Bit >>= 1;
                return Map & Bit;
            }
            /// <summary>
            /// 获取位图数据
            /// </summary>
            /// <returns></returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public bool? NextBool()
            {
                if (Bit == 2)
                {
                    Map = *(uint*)Read;
                    Bit = 1U << 31;
                    Read += sizeof(uint);
                }
                else Bit >>= 2;
                if ((Map & Bit) == 0) return null;
                return (Map & (Bit >> 1)) != 0;
            }
        }
        /// <summary>
        /// 配置参数
        /// </summary>
        public sealed class config
        {
            /// <summary>
            /// 成员位图
            /// </summary>
            public memberMap MemberMap;
            /// <summary>
            /// 反序列化状态
            /// </summary>
            public deSerializeState State;
            /// <summary>
            /// 数据长度
            /// </summary>
            public int DataLength { get; internal set; }
            /// <summary>
            /// 数据是否完整
            /// </summary>
            public bool IsFullData = true;
            /// <summary>
            /// 是否抛出错误异常
            /// </summary>
            public bool IsThrowError;
            /// <summary>
            /// 是否输出错误日志
            /// </summary>
            public bool IsLogError = true;
            /// <summary>
            /// 是否自动释放成员位图
            /// </summary>
            internal bool IsDisposeMemberMap;
        }
        /// <summary>
        /// 反序列化配置参数
        /// </summary>
        internal config Config;
        /// <summary>
        /// 成员位图
        /// </summary>
        internal memberMap MemberMap;
        /// <summary>
        /// 数据字节数组
        /// </summary>
        public byte[] Buffer { get; protected set; }
        /// <summary>
        /// 序列化数据起始位置
        /// </summary>
        protected byte* start;
        /// <summary>
        /// 序列化数据结束位置
        /// </summary>
        protected byte* end;
        /// <summary>
        /// 当前读取数据位置
        /// </summary>
        public byte* Read;
        /// <summary>
        /// 反序列化状态
        /// </summary>
        protected deSerializeState state;
        /// <summary>
        /// 是否序列化成员位图
        /// </summary>
        protected bool isMemberMap;
        /// <summary>
        /// 检测反序列化状态
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void checkState()
        {
            if (state == deSerializeState.Success)
            {
                if (Config.IsFullData)
                {
                    if (Read != end) Error(deSerializeState.FullDataError);
                }
                else if (Read <= end)
                {
                    int length = *(int*)Read;
                    if (length == Read - start) Config.DataLength = length + sizeof(int);
                    Error(deSerializeState.EndVerify);
                }
                else Error(deSerializeState.EndVerify);
            }
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void free()
        {
            if (Config.IsDisposeMemberMap)
            {
                if (MemberMap != null)
                {
                    MemberMap.Dispose();
                    MemberMap = null;
                }
            }
            else MemberMap = null;
        }
        /// <summary>
        /// 设置错误状态
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Error(deSerializeState state)
        {
            this.state = state;
            if (Config.IsLogError) fastCSharp.log.Error.Add(state.ToString(), null, false);
            if (Config.IsThrowError) throw new Exception(state.ToString());
        }
        /// <summary>
        /// 自定义序列化重置当前读取数据位置
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool VerifyRead(int size)
        {
            if ((Read += size) <= end) return true;
            Error(deSerializeState.IndexOutOfRange);
            return false;
        }
        /// <summary>
        /// 获取一个整数值
        /// </summary>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal int ReadInt()
        {
            int value = *(int*)Read;
            Read += sizeof(int);
            return value;
        }
        /// <summary>
        /// JSON反序列化
        /// </summary>
        /// <param name="value"></param>
        protected void parseJson<valueType>(ref valueType value)
        {
            int size = *(int*)Read;
            if (size == 0)
            {
                Read += sizeof(int);
                return;
            }
            if (size > 0 && (size & 1) == 0)
            {
                byte* start = Read;
                if ((Read += (size + (2 + sizeof(int))) & (int.MaxValue - 3)) <= end)
                {
                    if (!jsonParser.UnsafeParse((char*)start, size >> 1, ref value)) Error(deSerializeState.JsonError);
                    return;
                }
            }
            Error(deSerializeState.IndexOutOfRange);
        }
        /// <summary>
        /// 不支持对象null解析检测
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void checkNull()
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue) Read += sizeof(int);
            else Error(deSerializeState.NotNull);
        }
        /// <summary>
        /// 对象null值检测
        /// </summary>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal int CheckNull()
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                return 0;
            }
            return 1;
        }
        /// <summary>
        /// 检测成员数量
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool CheckMemberCount(int count)
        {
            if (*(int*)Read == count)
            {
                Read += sizeof(int);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 成员位图反序列化
        /// </summary>
        /// <param name="memberCount"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal ulong DeSerializeMemberMap(int memberCount)
        {
            if (*(int*)Read == memberCount)
            {
                ulong value = *(ulong*)(Read + sizeof(int));
                Read += sizeof(int) + sizeof(ulong);
                return value;
            }
            Error(deSerializeState.MemberMapVerify);
            return 0;
        }
        /// <summary>
        /// 成员位图反序列化
        /// </summary>
        /// <param name="map"></param>
        /// <param name="fieldCount"></param>
        /// <param name="size"></param>
        internal void DeSerializeFieldMemberMap(byte* map, int fieldCount, int size)
        {
            if (*(int*)Read == fieldCount)
            {
                if (size <= (int)(end - (Read += sizeof(int))))
                {
                    for (byte* mapEnd = map + (size & (int.MaxValue - sizeof(ulong) + 1)); map != mapEnd; map += sizeof(ulong), Read += sizeof(ulong)) *(ulong*)map = *(ulong*)Read;
                    if ((size & sizeof(int)) != 0)
                    {
                        *(uint*)map = *(uint*)Read;
                        Read += sizeof(uint);
                    }
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
            else Error(deSerializeState.MemberMapVerify);
        }
        /// <summary>
        /// 检测成员位图
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <returns></returns>
        public memberMap GetMemberMap<valueType>()
        {
            if ((*(uint*)Read & 0xc0000000U) == 0)
            {
                if (MemberMap == null)
                {
                    MemberMap = memberMap<valueType>.New();
                    if (*Read == 0)
                    {
                        Read += sizeof(int);
                        return MemberMap;
                    }
                }
                else
                {
                    if (MemberMap.Type != memberMap<valueType>.TypeInfo)
                    {
                        Error(deSerializeState.MemberMapType);
                        return null;
                    }
                    if (*Read == 0)
                    {
                        MemberMap.Clear();
                        Read += sizeof(int);
                        return MemberMap;
                    }
                }
                MemberMap.FieldDeSerialize(this);
                return state == deSerializeState.Success ? MemberMap : null;
            }
            Error(deSerializeState.MemberMap);
            return null;
        }
        /// <summary>
        /// 逻辑值反序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        [indexDeSerializer.memberMapDeSerializeMethod]
        [indexDeSerializer.deSerializeMethod]
        [dataDeSerializer.deSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void deSerialize(ref bool value)
        {
            value = *(bool*)Read;
            Read += sizeof(int);
        }
        /// <summary>
        /// 逻辑值反序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        [indexDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberDeSerialize(ref bool value)
        {
            value = *(bool*)Read++;
        }
        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="data">起始位置</param>
        /// <param name="value">目标数据</param>
        /// <returns>结束位置</returns>
        internal static byte* DeSerialize(byte* data, bool[] value)
        {
            arrayMap arrayMap = new arrayMap(data);
            for (int index = 0; index != value.Length; ++index) value[index] = arrayMap.Next() != 0;
            return arrayMap.Read;
        }
        /// <summary>
        /// 逻辑值反序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        [indexDeSerializer.deSerializeMethod]
        [dataDeSerializer.deSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void deSerialize(ref bool? value)
        {
            value = *(bool*)Read;
            Read += sizeof(int);
        }
        /// <summary>
        /// 逻辑值反序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        [indexDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberDeSerialize(ref bool? value)
        {
            if (*Read == 0) value = null;
            else value = *Read == 2;
            ++Read;
        }
        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="data">起始位置</param>
        /// <param name="value">目标数据</param>
        /// <returns>结束位置</returns>
        internal static byte* DeSerialize(byte* data, bool?[] value)
        {
            arrayMap arrayMap = new arrayMap(data, 2);
            for (int index = 0; index != value.Length; ++index) value[index] = arrayMap.NextBool();
            return arrayMap.Read;
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.memberMapDeSerializeMethod]
        [indexDeSerializer.deSerializeMethod]
        [dataDeSerializer.deSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void deSerialize(ref byte value)
        {
            value = *(byte*)Read;
            Read += sizeof(int);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberDeSerialize(ref byte value)
        {
            value = *(byte*)Read++;
        }
        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="data">起始位置</param>
        /// <param name="value">目标数据</param>
        /// <returns>结束位置</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static byte* DeSerialize(byte* data, byte[] value)
        {
            fastCSharp.unsafer.memory.Copy(data, value, value.Length);
            return data + ((value.Length + 3) & (int.MaxValue - 3));
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.deSerializeMethod]
        [dataDeSerializer.deSerializeMethod]
        [dataDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberMapDeSerializeMethod]
        protected void deSerialize(ref subArray<byte> value)
        {
            int length = *(int*)Read;
            if (length == 0)
            {
                value.UnsafeSetLength(0);
                Read += sizeof(int);
            }
            else
            {
                if (((length + (3 + sizeof(int))) & (int.MaxValue - 3)) <= (int)(end - Read))
                {
                    byte[] array = new byte[length];
                    Read = DeSerialize(Read + sizeof(int), array);
                    value.UnsafeSet(array, 0, length);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void DeSerialize(ref subArray<byte> value)
        {
            byte* read = DeSerialize(Read, end, Buffer, ref value);
            if (read == null) Error(emit.binaryDeSerializer.deSerializeState.IndexOutOfRange);
            else Read = read;
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="read"></param>
        /// <param name="end"></param>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        internal static byte* DeSerialize(byte* read, byte* end, byte[] buffer, ref subArray<byte> value)
        {
            int length = *(int*)read;
            if (length > 0)
            {
                byte* start = read;
                if ((read += (length + (3 + sizeof(int))) & (int.MaxValue - 3)) <= end)
                {
                    fixed (byte* bufferFixed = buffer)
                    {
                        value.UnsafeSet(buffer, (int)(start - bufferFixed) + sizeof(int), length);
                        return read;
                    }
                }
            }
            else if (length == 0)
            {
                value.UnsafeSet(nullValue<byte>.Array, 0, 0);
                return read + sizeof(int);
            }
            else if (length == fastCSharp.emit.binarySerializer.NullValue)
            {
                value.Null();
                return read + sizeof(int);
            }
            return null;
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.deSerializeMethod]
        [dataDeSerializer.deSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void deSerialize(ref byte? value)
        {
            value = *(byte*)Read;
            Read += sizeof(int);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberDeSerialize(ref byte? value)
        {
            if (*(Read + sizeof(byte)) == 0) value = *(byte*)Read;
            else value = null;
            Read += sizeof(ushort);
        }
        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="data">起始位置</param>
        /// <param name="value">目标数据</param>
        /// <returns>结束位置</returns>
        internal static byte* DeSerialize(byte* data, byte?[] value)
        {
            arrayMap arrayMap = new arrayMap(data);
            byte* start = (data += ((value.Length + 31) >> 5) << 2);
            for (int index = 0; index != value.Length; ++index)
            {
                if (arrayMap.Next() == 0) value[index] = null;
                else value[index] = *data++;
            }
            return data + ((int)(start - data) & 3);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.memberMapDeSerializeMethod]
        [indexDeSerializer.deSerializeMethod]
        [dataDeSerializer.deSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void deSerialize(ref sbyte value)
        {
            value = (sbyte)*(int*)Read;
            Read += sizeof(int);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberDeSerialize(ref sbyte value)
        {
            value = *(sbyte*)Read++;
        }
        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="data">起始位置</param>
        /// <param name="value">目标数据</param>
        /// <returns>结束位置</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static byte* DeSerialize(byte* data, sbyte[] value)
        {
            fixed (sbyte* valueFixed = value) fastCSharp.unsafer.memory.Copy(data, valueFixed, value.Length);
            return data + ((value.Length + 3) & (int.MaxValue - 3));
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.deSerializeMethod]
        [dataDeSerializer.deSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void deSerialize(ref sbyte? value)
        {
            value = (sbyte)*(int*)Read;
            Read += sizeof(int);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberDeSerialize(ref sbyte? value)
        {
            if (*(Read + sizeof(byte)) == 0) value = *(sbyte*)Read;
            else value = null;
            Read += sizeof(ushort);
        }
        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="data">起始位置</param>
        /// <param name="value">目标数据</param>
        /// <returns>结束位置</returns>
        internal static byte* DeSerialize(sbyte* data, sbyte?[] value)
        {
            arrayMap arrayMap = new arrayMap((byte*)data);
            sbyte* start = (data += ((value.Length + 31) >> 5) << 2);
            for (int index = 0; index != value.Length; ++index)
            {
                if (arrayMap.Next() == 0) value[index] = null;
                else value[index] = *data++;
            }
            return (byte*)(data + ((int)(start - data) & 3));
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.memberMapDeSerializeMethod]
        [indexDeSerializer.deSerializeMethod]
        [dataDeSerializer.deSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void deSerialize(ref short value)
        {
            value = (short)*(int*)Read;
            Read += sizeof(int);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberDeSerialize(ref short value)
        {
            value = *(short*)Read;
            Read += sizeof(short);
        }
        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="data">起始位置</param>
        /// <param name="value">目标数据</param>
        /// <returns>结束位置</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static byte* DeSerialize(byte* data, short[] value)
        {
            int length = value.Length * sizeof(short);
            fixed (short* valueFixed = value) fastCSharp.unsafer.memory.Copy(data, valueFixed, length);
            return data + ((length + 3) & (int.MaxValue - 3));
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.deSerializeMethod]
        [dataDeSerializer.deSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void deSerialize(ref short? value)
        {
            value = (short)*(int*)Read;
            Read += sizeof(int);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberDeSerialize(ref short? value)
        {
            if (*(ushort*)(Read + sizeof(ushort)) == 0) value = *(short*)Read;
            else value = null;
            Read += sizeof(int);
        }
        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="data">起始位置</param>
        /// <param name="value">目标数据</param>
        /// <returns>结束位置</returns>
        internal static byte* DeSerialize(byte* data, short?[] value)
        {
            arrayMap arrayMap = new arrayMap(data);
            short* read = (short*)(data + (((value.Length + 31) >> 5) << 2)), start = read;
            for (int index = 0; index != value.Length; ++index)
            {
                if (arrayMap.Next() == 0) value[index] = null;
                else value[index] = *read++;
            }
            return ((int)((byte*)read - (byte*)start) & 2) == 0 ? (byte*)read : (byte*)(read + 1);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.memberMapDeSerializeMethod]
        [indexDeSerializer.deSerializeMethod]
        [dataDeSerializer.deSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void deSerialize(ref ushort value)
        {
            value = *(ushort*)Read;
            Read += sizeof(int);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberDeSerialize(ref ushort value)
        {
            value = *(ushort*)Read;
            Read += sizeof(ushort);
        }
        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="data">起始位置</param>
        /// <param name="value">目标数据</param>
        /// <returns>结束位置</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static byte* DeSerialize(byte* data, ushort[] value)
        {
            int length = value.Length * sizeof(ushort);
            fixed (ushort* valueFixed = value) fastCSharp.unsafer.memory.Copy(data, valueFixed, length);
            return data + ((length + 3) & (int.MaxValue - 3));
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.deSerializeMethod]
        [dataDeSerializer.deSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void deSerialize(ref ushort? value)
        {
            value = *(ushort*)Read;
            Read += sizeof(int);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberDeSerialize(ref ushort? value)
        {
            if (*(ushort*)(Read + sizeof(ushort)) == 0) value = *(ushort*)Read;
            else value = null;
            Read += sizeof(int);
        }
        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="data">起始位置</param>
        /// <param name="value">目标数据</param>
        /// <returns>结束位置</returns>
        internal static byte* DeSerialize(byte* data, ushort?[] value)
        {
            arrayMap arrayMap = new arrayMap(data);
            ushort* read = (ushort*)(data + (((value.Length + 31) >> 5) << 2)), start = read;
            for (int index = 0; index != value.Length; ++index)
            {
                if (arrayMap.Next() == 0) value[index] = null;
                else value[index] = *read++;
            }
            return ((int)((byte*)read - (byte*)start) & 2) == 0 ? (byte*)read : (byte*)(read + 1);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.deSerializeMethod]
        [indexDeSerializer.memberDeSerializeMethod]
        [indexDeSerializer.memberMapDeSerializeMethod]
        [dataDeSerializer.deSerializeMethod]
        [dataDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void deSerialize(ref int value)
        {
            value = *(int*)Read;
            Read += sizeof(int);
        }
        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="data">起始位置</param>
        /// <param name="value">目标数据</param>
        /// <returns>结束位置</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static byte* DeSerialize(byte* data, int[] value)
        {
            int length = value.Length * sizeof(int);
            fixed (int* valueFixed = value) fastCSharp.unsafer.memory.Copy(data, valueFixed, length);
            return data + length;
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.deSerializeMethod]
        [dataDeSerializer.deSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void deSerialize(ref int? value)
        {
            value = *(int*)Read;
            Read += sizeof(int);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberDeSerialize(ref int? value)
        {
            if (*(int*)Read == 0)
            {
                value = *(int*)(Read += sizeof(int));
                Read += sizeof(int);
            }
            else
            {
                Read += sizeof(int);
                value = null;
            }
        }
        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="data">起始位置</param>
        /// <param name="value">目标数据</param>
        /// <returns>结束位置</returns>
        internal static byte* DeSerialize(byte* data, int?[] value)
        {
            arrayMap arrayMap = new arrayMap(data);
            int* read = (int*)(data + (((value.Length + 31) >> 5) << 2));
            for (int index = 0; index != value.Length; ++index)
            {
                if (arrayMap.Next() == 0) value[index] = null;
                else value[index] = *read++;
            }
            return (byte*)read;
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.deSerializeMethod]
        [indexDeSerializer.memberDeSerializeMethod]
        [indexDeSerializer.memberMapDeSerializeMethod]
        [dataDeSerializer.deSerializeMethod]
        [dataDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void deSerialize(ref uint value)
        {
            value = *(uint*)Read;
            Read += sizeof(int);
        }
        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="data">起始位置</param>
        /// <param name="value">目标数据</param>
        /// <returns>结束位置</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static byte* DeSerialize(byte* data, uint[] value)
        {
            int length = value.Length * sizeof(uint);
            fixed (uint* valueFixed = value) fastCSharp.unsafer.memory.Copy(data, valueFixed, length);
            return data + length;
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.deSerializeMethod]
        [dataDeSerializer.deSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void deSerialize(ref uint? value)
        {
            value = *(uint*)Read;
            Read += sizeof(int);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberDeSerialize(ref uint? value)
        {
            if (*(int*)Read == 0)
            {
                value = *(uint*)(Read += sizeof(int));
                Read += sizeof(uint);
            }
            else
            {
                Read += sizeof(int);
                value = null;
            }
        }
        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="data">起始位置</param>
        /// <param name="value">目标数据</param>
        /// <returns>结束位置</returns>
        internal static byte* DeSerialize(byte* data, uint?[] value)
        {
            arrayMap arrayMap = new arrayMap(data);
            uint* read = (uint*)(data + (((value.Length + 31) >> 5) << 2));
            for (int index = 0; index != value.Length; ++index)
            {
                if (arrayMap.Next() == 0) value[index] = null;
                else value[index] = *read++;
            }
            return (byte*)read;
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.deSerializeMethod]
        [indexDeSerializer.memberDeSerializeMethod]
        [indexDeSerializer.memberMapDeSerializeMethod]
        [dataDeSerializer.deSerializeMethod]
        [dataDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void deSerialize(ref long value)
        {
            value = *(long*)Read;
            Read += sizeof(long);
        }
        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="data">起始位置</param>
        /// <param name="value">目标数据</param>
        /// <returns>结束位置</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static byte* DeSerialize(byte* data, long[] value)
        {
            int length = value.Length * sizeof(long);
            fixed (long* valueFixed = value) fastCSharp.unsafer.memory.Copy(data, valueFixed, length);
            return data + length;
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.deSerializeMethod]
        [dataDeSerializer.deSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void deSerialize(ref long? value)
        {
            value = *(long*)Read;
            Read += sizeof(long);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberDeSerialize(ref long? value)
        {
            if (*(int*)Read == 0)
            {
                value = *(long*)(Read += sizeof(int));
                Read += sizeof(long);
            }
            else
            {
                Read += sizeof(int);
                value = null;
            }
        }
        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="data">起始位置</param>
        /// <param name="value">目标数据</param>
        /// <returns>结束位置</returns>
        internal static byte* DeSerialize(byte* data, long?[] value)
        {
            arrayMap arrayMap = new arrayMap(data);
            long* read = (long*)(data + (((value.Length + 31) >> 5) << 2));
            for (int index = 0; index != value.Length; ++index)
            {
                if (arrayMap.Next() == 0) value[index] = null;
                else value[index] = *read++;
            }
            return (byte*)read;
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.deSerializeMethod]
        [indexDeSerializer.memberDeSerializeMethod]
        [indexDeSerializer.memberMapDeSerializeMethod]
        [dataDeSerializer.deSerializeMethod]
        [dataDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void deSerialize(ref ulong value)
        {
            value = *(ulong*)Read;
            Read += sizeof(ulong);
        }
        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="data">起始位置</param>
        /// <param name="value">目标数据</param>
        /// <returns>结束位置</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static byte* DeSerialize(byte* data, ulong[] value)
        {
            int length = value.Length * sizeof(ulong);
            fixed (ulong* valueFixed = value) fastCSharp.unsafer.memory.Copy(data, valueFixed, length);
            return data + length;
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.deSerializeMethod]
        [dataDeSerializer.deSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void deSerialize(ref ulong? value)
        {
            value = *(ulong*)Read;
            Read += sizeof(ulong);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberDeSerialize(ref ulong? value)
        {
            if (*(int*)Read == 0)
            {
                value = *(ulong*)(Read += sizeof(int));
                Read += sizeof(ulong);
            }
            else
            {
                Read += sizeof(int);
                value = null;
            }
        }
        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="data">起始位置</param>
        /// <param name="value">目标数据</param>
        /// <returns>结束位置</returns>
        internal static byte* DeSerialize(byte* data, ulong?[] value)
        {
            arrayMap arrayMap = new arrayMap(data);
            ulong* read = (ulong*)(data + (((value.Length + 31) >> 5) << 2));
            for (int index = 0; index != value.Length; ++index)
            {
                if (arrayMap.Next() == 0) value[index] = null;
                else value[index] = *read++;
            }
            return (byte*)read;
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.deSerializeMethod]
        [indexDeSerializer.memberDeSerializeMethod]
        [indexDeSerializer.memberMapDeSerializeMethod]
        [dataDeSerializer.deSerializeMethod]
        [dataDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void deSerialize(ref float value)
        {
            value = *(float*)Read;
            Read += sizeof(float);
        }
        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="data">起始位置</param>
        /// <param name="value">目标数据</param>
        /// <returns>结束位置</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static byte* DeSerialize(byte* data, float[] value)
        {
            int length = value.Length * sizeof(float);
            fixed (float* valueFixed = value) fastCSharp.unsafer.memory.Copy(data, valueFixed, length);
            return data + length;
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.deSerializeMethod]
        [dataDeSerializer.deSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void deSerialize(ref float? value)
        {
            value = *(float*)Read;
            Read += sizeof(float);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberDeSerialize(ref float? value)
        {
            if (*(int*)Read == 0)
            {
                value = *(float*)(Read += sizeof(int));
                Read += sizeof(float);
            }
            else
            {
                Read += sizeof(int);
                value = null;
            }
        }
        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="data">起始位置</param>
        /// <param name="value">目标数据</param>
        /// <returns>结束位置</returns>
        internal static byte* DeSerialize(byte* data, float?[] value)
        {
            arrayMap arrayMap = new arrayMap(data);
            float* read = (float*)(data + (((value.Length + 31) >> 5) << 2));
            for (int index = 0; index != value.Length; ++index)
            {
                if (arrayMap.Next() == 0) value[index] = null;
                else value[index] = *read++;
            }
            return (byte*)read;
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.deSerializeMethod]
        [indexDeSerializer.memberDeSerializeMethod]
        [indexDeSerializer.memberMapDeSerializeMethod]
        [dataDeSerializer.deSerializeMethod]
        [dataDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void deSerialize(ref double value)
        {
            value = *(double*)Read;
            Read += sizeof(double);
        }
        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="data">起始位置</param>
        /// <param name="value">目标数据</param>
        /// <returns>结束位置</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static byte* DeSerialize(byte* data, double[] value)
        {
            int length = value.Length * sizeof(double);
            fixed (double* valueFixed = value) fastCSharp.unsafer.memory.Copy(data, valueFixed, length);
            return data + length;
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.deSerializeMethod]
        [dataDeSerializer.deSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void deSerialize(ref double? value)
        {
            value = *(double*)Read;
            Read += sizeof(double);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberDeSerialize(ref double? value)
        {
            if (*(int*)Read == 0)
            {
                value = *(double*)(Read += sizeof(int));
                Read += sizeof(double);
            }
            else
            {
                Read += sizeof(int);
                value = null;
            }
        }
        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="data">起始位置</param>
        /// <param name="value">目标数据</param>
        /// <returns>结束位置</returns>
        internal static byte* DeSerialize(byte* data, double?[] value)
        {
            arrayMap arrayMap = new arrayMap(data);
            double* read = (double*)(data + (((value.Length + 31) >> 5) << 2));
            for (int index = 0; index != value.Length; ++index)
            {
                if (arrayMap.Next() == 0) value[index] = null;
                else value[index] = *read++;
            }
            return (byte*)read;
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.deSerializeMethod]
        [indexDeSerializer.memberDeSerializeMethod]
        [indexDeSerializer.memberMapDeSerializeMethod]
        [dataDeSerializer.deSerializeMethod]
        [dataDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void deSerialize(ref decimal value)
        {
            value = *(decimal*)Read;
            Read += sizeof(decimal);
        }
        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="data">起始位置</param>
        /// <param name="value">目标数据</param>
        /// <returns>结束位置</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static byte* DeSerialize(byte* data, decimal[] value)
        {
            int length = value.Length * sizeof(decimal);
            fixed (decimal* valueFixed = value) fastCSharp.unsafer.memory.Copy(data, valueFixed, length);
            return data + length;
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.deSerializeMethod]
        [dataDeSerializer.deSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void deSerialize(ref decimal? value)
        {
            value = *(decimal*)Read;
            Read += sizeof(decimal);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberDeSerialize(ref decimal? value)
        {
            if (*(int*)Read == 0)
            {
                value = *(decimal*)(Read += sizeof(int));
                Read += sizeof(decimal);
            }
            else
            {
                Read += sizeof(int);
                value = null;
            }
        }
        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="data">起始位置</param>
        /// <param name="value">目标数据</param>
        /// <returns>结束位置</returns>
        internal static byte* DeSerialize(byte* data, decimal?[] value)
        {
            arrayMap arrayMap = new arrayMap(data);
            decimal* read = (decimal*)(data + (((value.Length + 31) >> 5) << 2));
            for (int index = 0; index != value.Length; ++index)
            {
                if (arrayMap.Next() == 0) value[index] = null;
                else value[index] = *read++;
            }
            return (byte*)read;
        }
        /// <summary>
        /// 字符反序列化
        /// </summary>
        /// <param name="value">字符</param>
        [indexDeSerializer.memberMapDeSerializeMethod]
        [indexDeSerializer.deSerializeMethod]
        [dataDeSerializer.deSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void deSerialize(ref char value)
        {
            value = *(char*)Read;
            Read += sizeof(int);
        }
        /// <summary>
        /// 字符反序列化
        /// </summary>
        /// <param name="value">字符</param>
        [indexDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberDeSerialize(ref char value)
        {
            value = *(char*)Read;
            Read += sizeof(char);
        }
        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="data">起始位置</param>
        /// <param name="value">目标数据</param>
        /// <returns>结束位置</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static byte* DeSerialize(byte* data, char[] value)
        {
            int length = value.Length * sizeof(char);
            fixed (char* valueFixed = value) fastCSharp.unsafer.memory.Copy(data, valueFixed, length);
            return data + ((length + 3) & (int.MaxValue - 3));
        }
        /// <summary>
        /// 字符反序列化
        /// </summary>
        /// <param name="value">字符</param>
        [indexDeSerializer.deSerializeMethod]
        [dataDeSerializer.deSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void deSerialize(ref char? value)
        {
            value = *(char*)Read;
            Read += sizeof(int);
        }
        /// <summary>
        /// 字符反序列化
        /// </summary>
        /// <param name="value">字符</param>
        [indexDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberDeSerialize(ref char? value)
        {
            if (*(ushort*)(Read + sizeof(char)) == 0) value = *(char*)Read;
            else value = null;
            Read += sizeof(int);
        }
        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="data">起始位置</param>
        /// <param name="value">目标数据</param>
        /// <returns>结束位置</returns>
        internal static byte* DeSerialize(byte* data, char?[] value)
        {
            arrayMap arrayMap = new arrayMap(data);
            char* read = (char*)(data + (((value.Length + 31) >> 5) << 2)), start = read;
            for (int index = 0; index != value.Length; ++index)
            {
                if (arrayMap.Next() == 0) value[index] = null;
                else value[index] = *read++;
            }
            return ((int)((byte*)read - (byte*)start) & 2) == 0 ? (byte*)read : (byte*)(read + 1);
        }
        /// <summary>
        /// 时间反序列化
        /// </summary>
        /// <param name="value">时间</param>
        [indexDeSerializer.deSerializeMethod]
        [indexDeSerializer.memberDeSerializeMethod]
        [indexDeSerializer.memberMapDeSerializeMethod]
        [dataDeSerializer.deSerializeMethod]
        [dataDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void deSerialize(ref DateTime value)
        {
            value = *(DateTime*)Read;
            Read += sizeof(DateTime);
        }
        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="data">起始位置</param>
        /// <param name="value">目标数据</param>
        /// <returns>结束位置</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static byte* DeSerialize(byte* data, DateTime[] value)
        {
            int length = value.Length * sizeof(DateTime);
            fixed (DateTime* valueFixed = value) fastCSharp.unsafer.memory.Copy(data, valueFixed, length);
            return data + length;
        }
        /// <summary>
        /// 时间反序列化
        /// </summary>
        /// <param name="value">时间</param>
        [indexDeSerializer.deSerializeMethod]
        [dataDeSerializer.deSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void deSerialize(ref DateTime? value)
        {
            value = *(DateTime*)Read;
            Read += sizeof(DateTime);
        }
        /// <summary>
        /// 时间反序列化
        /// </summary>
        /// <param name="value">时间</param>
        [indexDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberDeSerialize(ref DateTime? value)
        {
            if (*(int*)Read == 0)
            {
                value = *(DateTime*)(Read += sizeof(int));
                Read += sizeof(DateTime);
            }
            else
            {
                Read += sizeof(int);
                value = null;
            }
        }
        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="data">起始位置</param>
        /// <param name="value">目标数据</param>
        /// <returns>结束位置</returns>
        internal static byte* DeSerialize(byte* data, DateTime?[] value)
        {
            arrayMap arrayMap = new arrayMap(data);
            DateTime* read = (DateTime*)(data + (((value.Length + 31) >> 5) << 2));
            for (int index = 0; index != value.Length; ++index)
            {
                if (arrayMap.Next() == 0) value[index] = null;
                else value[index] = *read++;
            }
            return (byte*)read;
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexDeSerializer.deSerializeMethod]
        [indexDeSerializer.memberDeSerializeMethod]
        [indexDeSerializer.memberMapDeSerializeMethod]
        [dataDeSerializer.deSerializeMethod]
        [dataDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void deSerialize(ref Guid value)
        {
            value = *(Guid*)Read;
            Read += sizeof(Guid);
        }
        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="data">起始位置</param>
        /// <param name="value">目标数据</param>
        /// <returns>结束位置</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static byte* DeSerialize(byte* data, Guid[] value)
        {
            int length = value.Length * sizeof(Guid);
            fixed (Guid* valueFixed = value) fastCSharp.unsafer.memory.Copy(data, valueFixed, length);
            return data + length;
        }
        /// <summary>
        /// Guid反序列化
        /// </summary>
        /// <param name="value">Guid</param>
        [indexDeSerializer.deSerializeMethod]
        [dataDeSerializer.deSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void deSerialize(ref Guid? value)
        {
            value = *(Guid*)Read;
            Read += sizeof(Guid);
        }
        /// <summary>
        /// Guid反序列化
        /// </summary>
        /// <param name="value">Guid</param>
        [indexDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberDeSerialize(ref Guid? value)
        {
            if (*(int*)Read == 0)
            {
                value = *(Guid*)(Read += sizeof(int));
                Read += sizeof(Guid);
            }
            else
            {
                Read += sizeof(int);
                value = null;
            }
        }
        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="data">起始位置</param>
        /// <param name="value">目标数据</param>
        /// <returns>结束位置</returns>
        internal static byte* DeSerialize(byte* data, Guid?[] value)
        {
            arrayMap arrayMap = new arrayMap(data);
            Guid* read = (Guid*)(data + (((value.Length + 31) >> 5) << 2));
            for (int index = 0; index != value.Length; ++index)
            {
                if (arrayMap.Next() == 0) value[index] = null;
                else value[index] = *read++;
            }
            return (byte*)read;
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="value"></param>
        [indexDeSerializer.deSerializeMethod]
        [dataDeSerializer.deSerializeMethod]
        [dataDeSerializer.memberDeSerializeMethod]
        [dataDeSerializer.memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void deSerialize(ref subString value)
        {
            string stringValue = null;
            if ((Read = DeSerialize(Read, end, ref stringValue)) == null) Error(deSerializeState.IndexOutOfRange);
            else value.UnsafeSet(stringValue, 0, stringValue.Length);
        }
        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="data">起始位置</param>
        /// <param name="end">结束位置</param>
        /// <param name="value">目标数据</param>
        /// <returns>结束位置,失败返回null</returns>
        internal static byte* DeSerialize(byte* data, byte* end, ref string value)
        {
            int length = *(int*)data;
            if ((length & 1) == 0)
            {
                if (length != 0)
                {
                    int dataLength = (length + (3 + sizeof(int))) & (int.MaxValue - 3);
                    if (dataLength <= end - data)
                    {
                        value = new string((char*)(data + sizeof(int)), 0, length >> 1);
                        return data + dataLength;
                    }
                }
                else
                {
                    value = string.Empty;
                    return data + sizeof(int);
                }
            }
            else
            {
                int dataLength = ((length >>= 1) + (3 + sizeof(int))) & (int.MaxValue - 3);
                if (dataLength <= end - data)
                {
                    fixed (char* valueFixed = (value = fastCSharp.String.FastAllocateString(length)))
                    {
                        byte* start = data + sizeof(int);
                        char* write = valueFixed;
                        end = start + length;
                        do
                        {
                            *write++ = (char)*start++;
                        }
                        while (start != end);
                    }
                    return data + dataLength;
                }
            }
            return null;
        }

        /// <summary>
        /// 枚举值序列化
        /// </summary>
        /// <param name="value">枚举值序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected unsafe void enumByte<valueType>(ref valueType value)
        {
            value = pub.enumCast<valueType, byte>.FromInt(*Read);
            Read += sizeof(int);
        }
        /// <summary>
        /// 枚举值序列化
        /// </summary>
        /// <param name="value">枚举值序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected unsafe void enumByteMember<valueType>(ref valueType value)
        {
            value = pub.enumCast<valueType, byte>.FromInt(*Read++);
        }
        /// <summary>
        /// 枚举值序列化
        /// </summary>
        /// <param name="value">枚举值序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected unsafe void enumSByte<valueType>(ref valueType value)
        {
            value = pub.enumCast<valueType, sbyte>.FromInt((sbyte)*(int*)Read);
            Read += sizeof(int);
        }
        /// <summary>
        /// 枚举值序列化
        /// </summary>
        /// <param name="value">枚举值序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected unsafe void enumSByteMember<valueType>(ref valueType value)
        {
            value = pub.enumCast<valueType, sbyte>.FromInt(*(sbyte*)Read++);
        }
        /// <summary>
        /// 枚举值序列化
        /// </summary>
        /// <param name="value">枚举值序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected unsafe void enumShort<valueType>(ref valueType value)
        {
            value = pub.enumCast<valueType, short>.FromInt((short)*(int*)Read);
            Read += sizeof(int);
        }
        /// <summary>
        /// 枚举值序列化
        /// </summary>
        /// <param name="value">枚举值序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected unsafe void enumShortMember<valueType>(ref valueType value)
        {
            value = pub.enumCast<valueType, short>.FromInt(*(short*)Read);
            Read += sizeof(short);
        }
        /// <summary>
        /// 枚举值序列化
        /// </summary>
        /// <param name="value">枚举值序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected unsafe void enumUShort<valueType>(ref valueType value)
        {
            value = pub.enumCast<valueType, ushort>.FromInt(*(ushort*)Read);
            Read += sizeof(int);
        }
        /// <summary>
        /// 枚举值序列化
        /// </summary>
        /// <param name="value">枚举值序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected unsafe void enumUShortMember<valueType>(ref valueType value)
        {
            value = pub.enumCast<valueType, ushort>.FromInt(*(ushort*)Read);
            Read += sizeof(ushort);
        }
        /// <summary>
        /// 枚举值序列化
        /// </summary>
        /// <param name="value">枚举值序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected unsafe void enumInt<valueType>(ref valueType value)
        {
            value = pub.enumCast<valueType, int>.FromInt(*(int*)Read);
            Read += sizeof(int);
        }
        /// <summary>
        /// 枚举值序列化
        /// </summary>
        /// <param name="value">枚举值序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected unsafe void enumUInt<valueType>(ref valueType value)
        {
            value = pub.enumCast<valueType, uint>.FromInt(*(uint*)Read);
            Read += sizeof(uint);
        }
        /// <summary>
        /// 枚举值序列化
        /// </summary>
        /// <param name="value">枚举值序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected unsafe void enumLong<valueType>(ref valueType value)
        {
            value = pub.enumCast<valueType, long>.FromInt(*(long*)Read);
            Read += sizeof(long);
        }
        /// <summary>
        /// 枚举值序列化
        /// </summary>
        /// <param name="value">枚举值序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected unsafe void enumULong<valueType>(ref valueType value)
        {
            value = pub.enumCast<valueType, ulong>.FromInt(*(ulong*)Read);
            Read += sizeof(ulong);
        }
        /// <summary>
        /// 公共默认配置参数
        /// </summary>
        protected static readonly config defaultConfig = new config { IsDisposeMemberMap = true };
    }
}
