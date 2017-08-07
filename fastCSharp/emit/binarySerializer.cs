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
    public unsafe abstract class binarySerializer : IDisposable
    {
        /// <summary>
        /// 空对象
        /// </summary>
        public const int NullValue = int.MinValue;
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
            /// 当前写入位置
            /// </summary>
            public byte* Write;
            /// <summary>
            /// 数组位图
            /// </summary>
            /// <param name="stream">序列化数据流</param>
            /// <param name="arrayLength">数组长度</param>
            public arrayMap(unmanagedStream stream, int arrayLength)
            {
                int length = ((arrayLength + (31 + 32)) >> 5) << 2;
                Bit = 1U << 31;
                stream.PrepLength(length);
                Write = stream.CurrentData;
                Map = 0;
                *(int*)Write = arrayLength;
                stream.UnsafeAddLength(length);
            }
            /// <summary>
            /// 数组位图
            /// </summary>
            /// <param name="stream">序列化数据流</param>
            /// <param name="arrayLength">数组长度</param>
            /// <param name="prepLength">附加长度</param>
            public arrayMap(unmanagedStream stream, int arrayLength, int prepLength)
            {
                int length = ((arrayLength + (31 + 32)) >> 5) << 2;
                Bit = 1U << 31;
                stream.PrepLength(length + prepLength);
                Write = stream.CurrentData;
                Map = 0;
                *(int*)Write = arrayLength;
                stream.UnsafeAddLength(length);
            }
            /// <summary>
            /// 添加数据
            /// </summary>
            /// <param name="value">是否写位图</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Next(bool value)
            {
                if (value) Map |= Bit;
                if (Bit == 1)
                {
                    *(uint*)(Write += sizeof(int)) = Map;
                    Bit = 1U << 31;
                    Map = 0;
                }
                else Bit >>= 1;
            }
            ///// <summary>
            ///// 添加数据
            ///// </summary>
            ///// <param name="value">是否写位图</param>
            //
            //public void NextNot(bool value)
            //{
            //    if (!value) Map |= Bit;
            //    if (Bit == 1)
            //    {
            //        *(uint*)(Write += sizeof(int)) = Map;
            //        Bit = 1U << 31;
            //        Map = 0;
            //    }
            //    else Bit >>= 1;
            //}
            /// <summary>
            /// 添加数据
            /// </summary>
            /// <param name="value">是否写位图</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Next(bool? value)
            {
                if (value.HasValue)
                {
                    Map |= Bit;
                    if ((bool)value) Map |= (Bit >> 1);
                }
                if (Bit == 2)
                {
                    *(uint*)(Write += sizeof(int)) = Map;
                    Bit = 1U << 31;
                    Map = 0;
                }
                else Bit >>= 2;
            }
            /// <summary>
            /// 位图写入结束
            /// </summary>
            /// <param name="stream">序列化数据流</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void End(unmanagedStream stream)
            {
                if (Bit != 1U << 31) *(uint*)(Write + sizeof(int)) = Map;
                stream.PrepLength();
            }
        }
        /// <summary>
        /// 配置参数
        /// </summary>
        public class config
        {
            /// <summary>
            /// 序列化头部数据
            /// </summary>
            internal const uint HeaderMapValue = 0x51031000U;
            /// <summary>
            /// 序列化头部数据
            /// </summary>
            internal const uint HeaderMapAndValue = 0xffffff00U;
            /// <summary>
            /// 是否序列化成员位图
            /// </summary>
            internal const int MemberMapValue = 1;
            /// <summary>
            /// 成员位图
            /// </summary>
            public memberMap MemberMap;
            /// <summary>
            /// 成员位图类型不匹配是否输出错误信息
            /// </summary>
            public bool IsMemberMapErrorLog = true;
            /// <summary>
            /// 序列化头部数据
            /// </summary>
            internal virtual int HeaderValue
            {
                get
                {
                    int value = (int)HeaderMapValue;
                    if (MemberMap != null) value += MemberMapValue;
                    return value;
                }
            }
        }
        /// <summary>
        /// 字段信息
        /// </summary>
        internal class fieldInfo
        {
            /// <summary>
            /// 字段信息
            /// </summary>
            public FieldInfo Field;
            /// <summary>
            /// 成员索引
            /// </summary>
            public int MemberIndex;
            /// <summary>
            /// 固定分组排序字节数
            /// </summary>
            internal byte FixedSize;
            /// <summary>
            /// 字段信息
            /// </summary>
            /// <param name="field"></param>
            internal fieldInfo(fieldIndex field)
            {
                Field = field.Member;
                MemberIndex = field.MemberIndex;
                if (Field.FieldType.IsEnum) fixedSizes.TryGetValue(Field.FieldType.GetEnumUnderlyingType(), out FixedSize);
                else fixedSizes.TryGetValue(Field.FieldType, out FixedSize);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            /// <returns></returns>
            internal static int FixedSizeSort(fieldInfo left, fieldInfo right)
            {
                return (int)((uint)right.FixedSize & (0U - (uint)right.FixedSize)) - (int)((uint)left.FixedSize & (0U - (uint)left.FixedSize));
            }
            /// <summary>
            /// 固定类型字节数
            /// </summary>
            private static readonly Dictionary<Type, byte> fixedSizes;
            static fieldInfo()
            {
                fixedSizes = dictionary.CreateOnly<Type, byte>();
                fixedSizes.Add(typeof(bool), sizeof(bool));
                fixedSizes.Add(typeof(byte), sizeof(byte));
                fixedSizes.Add(typeof(sbyte), sizeof(sbyte));
                fixedSizes.Add(typeof(short), sizeof(short));
                fixedSizes.Add(typeof(ushort), sizeof(ushort));
                fixedSizes.Add(typeof(int), sizeof(int));
                fixedSizes.Add(typeof(uint), sizeof(uint));
                fixedSizes.Add(typeof(long), sizeof(long));
                fixedSizes.Add(typeof(ulong), sizeof(ulong));
                fixedSizes.Add(typeof(char), sizeof(char));
                fixedSizes.Add(typeof(DateTime), sizeof(long));
                fixedSizes.Add(typeof(float), sizeof(float));
                fixedSizes.Add(typeof(double), sizeof(double));
                fixedSizes.Add(typeof(decimal), sizeof(decimal));
                fixedSizes.Add(typeof(Guid), (byte)sizeof(Guid));
                fixedSizes.Add(typeof(bool?), sizeof(byte));
                fixedSizes.Add(typeof(byte?), sizeof(ushort));
                fixedSizes.Add(typeof(sbyte?), sizeof(ushort));
                fixedSizes.Add(typeof(short?), sizeof(uint));
                fixedSizes.Add(typeof(ushort?), sizeof(uint));
                fixedSizes.Add(typeof(int?), sizeof(int) + sizeof(int));
                fixedSizes.Add(typeof(uint?), sizeof(uint) + sizeof(int));
                fixedSizes.Add(typeof(long?), sizeof(long) + sizeof(int));
                fixedSizes.Add(typeof(ulong?), sizeof(ulong) + sizeof(int));
                fixedSizes.Add(typeof(char?), sizeof(uint));
                fixedSizes.Add(typeof(DateTime?), sizeof(long) + sizeof(int));
                fixedSizes.Add(typeof(float?), sizeof(float) + sizeof(int));
                fixedSizes.Add(typeof(double?), sizeof(double) + sizeof(int));
                fixedSizes.Add(typeof(decimal?), sizeof(decimal) + sizeof(int));
                fixedSizes.Add(typeof(Guid?), (byte)(sizeof(Guid) + sizeof(int)));
            }
        }
        /// <summary>
        /// 字段集合信息
        /// </summary>
        /// <typeparam name="fieldType"></typeparam>
        internal struct fields<fieldType> where fieldType : fieldInfo
        {
            /// <summary>
            /// 固定序列化字段
            /// </summary>
            public subArray<fieldType> FixedFields;
            /// <summary>
            /// 非固定序列化字段
            /// </summary>
            public subArray<fieldType> Fields;
            /// <summary>
            /// JSON混合序列化字段
            /// </summary>
            public subArray<fieldIndex> JsonFields;
            /// <summary>
            /// 固定序列化字段字节数
            /// </summary>
            public int FixedSize;
        }
        /// <summary>
        /// 序列化输出缓冲区
        /// </summary>
        public readonly unmanagedStream Stream = new unmanagedStream((byte*)fastCSharp.emit.pub.PuzzleValue, 1);
        /// <summary>
        /// JSON序列化输出缓冲区
        /// </summary>
        private charStream jsonStream;
        /// <summary>
        /// JSON序列化配置参数
        /// </summary>
        private jsonSerializer.config jsonConfig;
        /// <summary>
        /// JSON序列化成员位图
        /// </summary>
        internal memberMap JsonMemberMap;
        /// <summary>
        /// 序列化输出缓冲区字段信息
        /// </summary>
        internal static readonly FieldInfo StreamField = typeof(binarySerializer).GetField("Stream", BindingFlags.Instance | BindingFlags.Public);
        /// <summary>
        /// 成员位图
        /// </summary>
        internal memberMap MemberMap;
        /// <summary>
        /// 成员位图
        /// </summary>
        internal memberMap CurrentMemberMap;
        /// <summary>
        /// 数据流起始位置
        /// </summary>
        protected int streamStartIndex;
        /// <summary>
        /// 序列化配置参数
        /// </summary>
        protected config binarySerializerConfig;
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            fastCSharp.pub.Dispose(ref JsonMemberMap);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void free()
        {
            MemberMap = CurrentMemberMap = null;
        }
        /// <summary>
        /// 获取JSON序列化输出缓冲区
        /// </summary>
        /// <param name="data"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal charStream ResetJsonStream(void* data, int size)
        {
            if (jsonStream == null) return jsonStream = new charStream((char*)data, size >> 1);
            jsonStream.UnsafeReset((byte*)data, size);
            return jsonStream;
        }
        /// <summary>
        /// 获取JSON序列化配置参数
        /// </summary>
        /// <param name="memberMap"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected jsonSerializer.config getJsonConfig(memberMap memberMap)
        {
            if (jsonConfig == null) jsonConfig = new jsonSerializer.config { CheckLoopDepth = fastCSharp.config.appSetting.SerializeDepth };
            jsonConfig.MemberMap = memberMap;
            return jsonConfig;
        }
        /// <summary>
        /// 获取JSON成员位图
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="memberMap"></param>
        /// <param name="memberIndexs"></param>
        /// <returns></returns>
        protected memberMap getJsonMemberMap<valueType>(memberMap memberMap, int[] memberIndexs)
        {
            int count = 0;
            foreach (int memberIndex in memberIndexs)
            {
                if (memberMap.IsMember(memberIndex))
                {
                    if (count == 0 && JsonMemberMap == null) JsonMemberMap = memberMap<valueType>.NewEmpty();
                    JsonMemberMap.SetMember(memberIndex);
                    ++count;
                }
            }
            return count == 0 ? null : JsonMemberMap;
        }
        /// <summary>
        /// 序列化成员位图
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public memberMap SerializeMemberMap<valueType>()
        {
            if (MemberMap != null)
            {
                CurrentMemberMap = MemberMap;
                MemberMap = null;
                if (CurrentMemberMap.Type == memberMap<valueType>.TypeInfo)
                {
                    CurrentMemberMap.FieldSerialize(Stream);
                    return CurrentMemberMap;
                }
                if (binarySerializerConfig.IsMemberMapErrorLog) log.Error.Add("二进制序列化成员位图类型匹配失败", null, true);
            }
            return null;
        }
        /// <summary>
        /// 逻辑值序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        [indexSerializer.serializeMethod]
        [dataSerializer.serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void serialize(bool value)
        {
            Stream.Write(value ? (int)1 : 0);
        }
        /// <summary>
        /// 逻辑值序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        [indexSerializer.memberSerializeMethod]
        [dataSerializer.memberSerializeMethod]
        [dataSerializer.memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberSerialize(bool value)
        {
            Stream.UnsafeWrite(value);
        }
        /// <summary>
        /// 预增数据流长度并写入长度与数据(4字节对齐)
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="array">数据,不能为null</param>
        public static void Serialize(unmanagedStream stream, bool[] array)
        {
            arrayMap arrayMap = new arrayMap(stream, array.Length);
            foreach (bool value in array) arrayMap.Next(value);
            arrayMap.End(stream);
        }
        /// <summary>
        /// 逻辑值序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        [indexSerializer.serializeMethod]
        [dataSerializer.serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void serialize(bool? value)
        {
            Stream.Write((bool)value ? (int)1 : 0);
        }
        /// <summary>
        /// 逻辑值序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        [indexSerializer.memberSerializeMethod]
        [dataSerializer.memberSerializeMethod]
        [dataSerializer.memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberSerialize(bool? value)
        {
            if (value.HasValue) Stream.UnsafeWrite((bool)value ? (byte)2 : (byte)1);
            else Stream.UnsafeWrite((byte)0);
        }
        /// <summary>
        /// 预增数据流长度并写入长度与数据(4字节对齐)
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="array">数据,不能为null</param>
        internal static unsafe void Serialize(unmanagedStream stream, bool?[] array)
        {
            arrayMap arrayMap = new arrayMap(stream, array.Length << 1);
            foreach (bool? value in array) arrayMap.Next(value);
            arrayMap.End(stream);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.serializeMethod]
        [dataSerializer.serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void serialize(byte value)
        {
            Stream.Write((uint)value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.memberSerializeMethod]
        [dataSerializer.memberSerializeMethod]
        [dataSerializer.memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberSerialize(byte value)
        {
            Stream.UnsafeWrite(value);
        }
        /// <summary>
        /// 预增数据流长度并写入长度与数据(4字节对齐)
        /// </summary>
        /// <param name="stream">数据流</param>
        /// <param name="data">数据,不能为null</param>
        /// <param name="arrayLength">数据数量</param>
        /// <param name="size">单个数据字节数</param>
        public static unsafe void Serialize(unmanagedStream stream, void* data, int arrayLength, int size)
        {
            int dataSize = arrayLength * size, length = (dataSize + (3 + sizeof(int))) & (int.MaxValue - 3);
            stream.PrepLength(length);
            byte* write = stream.CurrentData;
            *(int*)write = arrayLength;
            fastCSharp.unsafer.memory.Copy(data, write + sizeof(int), dataSize);
            stream.UnsafeAddLength(length);
            stream.PrepLength();
        }
        /// <summary>
        /// 预增数据流长度并写入长度与数据(4字节对齐)
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="data">数据,不能为null</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static unsafe void Serialize(unmanagedStream stream, byte[] data)
        {
            fixed (byte* dataFixed = data) Serialize(stream, dataFixed, data.Length, 1);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.serializeMethod]
        [dataSerializer.serializeMethod]
        [dataSerializer.memberSerializeMethod]
        [dataSerializer.memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void serialize(subArray<byte> value)
        {
            if (value.length == 0) Stream.Write(0);
            else Serialize(Stream, ref value);
        }
        /// <summary>
        /// 预增数据流长度并序列化数据(4字节对齐)
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="data">数据,不能为null</param>
        internal static unsafe void Serialize(unmanagedStream stream, ref subArray<byte> data)
        {
            int length = sizeof(int) + ((data.length + 3) & (int.MaxValue - 3));
            stream.PrepLength(length);
            byte* write = stream.CurrentData;
            *(int*)write = data.length;
            fixed (byte* dataFixed = data.array) fastCSharp.unsafer.memory.Copy(dataFixed + data.startIndex, write + sizeof(int), data.length);
            stream.UnsafeAddLength(length);
            stream.PrepLength();
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.serializeMethod]
        [dataSerializer.serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void serialize(byte? value)
        {
            Stream.Write((uint)(byte)value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.memberSerializeMethod]
        [dataSerializer.memberSerializeMethod]
        [dataSerializer.memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberSerialize(byte? value)
        {
            if (value.HasValue) Stream.UnsafeWrite((ushort)(byte)value);
            else Stream.UnsafeWrite(short.MinValue);
        }
        /// <summary>
        /// 序列化可空数组
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="array">数组数据</param>
        internal static void Serialize(unmanagedStream stream, byte?[] array)
        {
            arrayMap arrayMap = new arrayMap(stream, array.Length, array.Length);
            byte* write = stream.CurrentData;
            foreach (byte? value in array)
            {
                if (value.HasValue)
                {
                    arrayMap.Next(true);
                    *write++ = (byte)value;
                }
                else arrayMap.Next(false);
            }
            stream.UnsafeAddLength(((int)(write - stream.CurrentData) + 3) & (int.MaxValue - 3));
            arrayMap.End(stream);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.serializeMethod]
        [dataSerializer.serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void serialize(sbyte value)
        {
            Stream.Write((int)value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.memberSerializeMethod]
        [dataSerializer.memberSerializeMethod]
        [dataSerializer.memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberSerialize(sbyte value)
        {
            Stream.UnsafeWrite(value);
        }
        /// <summary>
        /// 预增数据流长度并写入长度与数据(4字节对齐)
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="data">数据,不能为null</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static unsafe void Serialize(unmanagedStream stream, sbyte[] data)
        {
            fixed (sbyte* dataFixed = data) Serialize(stream, dataFixed, data.Length, 1);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.serializeMethod]
        [dataSerializer.serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void serialize(sbyte? value)
        {
            Stream.Write((int)(sbyte)value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.memberSerializeMethod]
        [dataSerializer.memberSerializeMethod]
        [dataSerializer.memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberSerialize(sbyte? value)
        {
            if (value.HasValue) Stream.UnsafeWrite((ushort)(byte)(sbyte)value);
            else Stream.UnsafeWrite(short.MinValue);
        }
        /// <summary>
        /// 序列化可空数组
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="array">数组数据</param>
        internal static void Serialize(unmanagedStream stream, sbyte?[] array)
        {
            arrayMap arrayMap = new arrayMap(stream, array.Length, array.Length);
            sbyte* write = (sbyte*)stream.CurrentData;
            foreach (sbyte? value in array)
            {
                if (value.HasValue)
                {
                    arrayMap.Next(true);
                    *write++ = (sbyte)value;
                }
                else arrayMap.Next(false);
            }
            stream.UnsafeAddLength(((int)(write - (sbyte*)stream.CurrentData) + 3) & (int.MaxValue - 3));
            arrayMap.End(stream);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.serializeMethod]
        [dataSerializer.serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void serialize(short value)
        {
            Stream.Write((int)value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.memberSerializeMethod]
        [dataSerializer.memberSerializeMethod]
        [dataSerializer.memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberSerialize(short value)
        {
            Stream.UnsafeWrite(value);
        }
        /// <summary>
        /// 预增数据流长度并写入长度与数据(4字节对齐)
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="data">数据,不能为null</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static unsafe void Serialize(unmanagedStream stream, short[] data)
        {
            fixed (short* dataFixed = data) Serialize(stream, dataFixed, data.Length, sizeof(short));
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.serializeMethod]
        [dataSerializer.serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void serialize(short? value)
        {
            Stream.Write((int)(short)value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.memberSerializeMethod]
        [dataSerializer.memberSerializeMethod]
        [dataSerializer.memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberSerialize(short? value)
        {
            if (value.HasValue) Stream.UnsafeWrite((uint)(ushort)(short)value);
            else Stream.UnsafeWrite(NullValue);
        }
        /// <summary>
        /// 序列化可空数组
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="array">数组数据</param>
        internal static void Serialize(unmanagedStream stream, short?[] array)
        {
            arrayMap arrayMap = new arrayMap(stream, array.Length, array.Length * sizeof(short));
            short* write = (short*)stream.CurrentData;
            foreach (short? value in array)
            {
                if (value.HasValue)
                {
                    arrayMap.Next(true);
                    *write++ = (short)value;
                }
                else arrayMap.Next(false);
            }
            stream.UnsafeAddLength(((int)((byte*)write - stream.CurrentData) + 3) & (int.MaxValue - 3));
            arrayMap.End(stream);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.serializeMethod]
        [dataSerializer.serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void serialize(ushort value)
        {
            Stream.Write((uint)value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.memberSerializeMethod]
        [dataSerializer.memberSerializeMethod]
        [dataSerializer.memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberSerialize(ushort value)
        {
            Stream.UnsafeWrite(value);
        }
        /// <summary>
        /// 预增数据流长度并写入长度与数据(4字节对齐)
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="data">数据,不能为null</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static unsafe void Serialize(unmanagedStream stream, ushort[] data)
        {
            fixed (ushort* dataFixed = data) Serialize(stream, dataFixed, data.Length, sizeof(ushort));
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.serializeMethod]
        [dataSerializer.serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void serialize(ushort? value)
        {
            Stream.Write((uint)(ushort)value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.memberSerializeMethod]
        [dataSerializer.memberSerializeMethod]
        [dataSerializer.memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberSerialize(ushort? value)
        {
            if (value.HasValue) Stream.UnsafeWrite((uint)(ushort)value);
            else Stream.UnsafeWrite(NullValue);
        }
        /// <summary>
        /// 序列化可空数组
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="array">数组数据</param>
        internal static void Serialize(unmanagedStream stream, ushort?[] array)
        {
            arrayMap arrayMap = new arrayMap(stream, array.Length, array.Length * sizeof(ushort));
            ushort* write = (ushort*)stream.CurrentData;
            foreach (ushort? value in array)
            {
                if (value.HasValue)
                {
                    arrayMap.Next(true);
                    *write++ = (ushort)value;
                }
                else arrayMap.Next(false);
            }
            stream.UnsafeAddLength(((int)((byte*)write - stream.CurrentData) + 3) & (int.MaxValue - 3));
            arrayMap.End(stream);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.serializeMethod]
        [dataSerializer.serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void serialize(int value)
        {
            Stream.Write(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.memberSerializeMethod]
        [indexSerializer.memberMapSerializeMethod]
        [dataSerializer.memberSerializeMethod]
        [dataSerializer.memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberSerialize(int value)
        {
            Stream.UnsafeWrite(value);
        }
        /// <summary>
        /// 预增数据流长度并写入长度与数据(4字节对齐)
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="data">数据,不能为null</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static unsafe void Serialize(unmanagedStream stream, int[] data)
        {
            fixed (int* dataFixed = data) Serialize(stream, dataFixed, data.Length, sizeof(int));
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.serializeMethod]
        [dataSerializer.serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void serialize(int? value)
        {
            Stream.Write((int)value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.memberSerializeMethod]
        [dataSerializer.memberSerializeMethod]
        [dataSerializer.memberMapSerializeMethod]
        protected void memberSerialize(int? value)
        {
            if (value.HasValue)
            {
                byte* data = Stream.CurrentData;
                *(int*)data = 0;
                *(int*)(data + sizeof(int)) = (int)value;
                Stream.UnsafeAddLength(sizeof(int) + sizeof(int));
            }
            else Stream.UnsafeWrite(NullValue);
        }
        /// <summary>
        /// 序列化可空数组
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="array">数组数据</param>
        internal static void Serialize(unmanagedStream stream, int?[] array)
        {
            arrayMap arrayMap = new arrayMap(stream, array.Length, array.Length * sizeof(int));
            int* write = (int*)stream.CurrentData;
            foreach (int? value in array)
            {
                if (value.HasValue)
                {
                    arrayMap.Next(true);
                    *write++ = (int)value;
                }
                else arrayMap.Next(false);
            }
            stream.UnsafeAddLength((int)((byte*)write - stream.CurrentData));
            arrayMap.End(stream);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.serializeMethod]
        [dataSerializer.serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void serialize(uint value)
        {
            Stream.Write(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.memberSerializeMethod]
        [indexSerializer.memberMapSerializeMethod]
        [dataSerializer.memberSerializeMethod]
        [dataSerializer.memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberSerialize(uint value)
        {
            Stream.UnsafeWrite(value);
        }
        /// <summary>
        /// 预增数据流长度并写入长度与数据(4字节对齐)
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="data">数据,不能为null</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static unsafe void Serialize(unmanagedStream stream, uint[] data)
        {
            fixed (uint* dataFixed = data) Serialize(stream, dataFixed, data.Length, sizeof(uint));
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.serializeMethod]
        [dataSerializer.serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void serialize(uint? value)
        {
            Stream.Write((uint)value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.memberSerializeMethod]
        [dataSerializer.memberSerializeMethod]
        [dataSerializer.memberMapSerializeMethod]
        protected void memberSerialize(uint? value)
        {
            if (value.HasValue)
            {
                byte* data = Stream.CurrentData;
                *(int*)data = 0;
                *(uint*)(data + sizeof(int)) = (uint)value;
                Stream.UnsafeAddLength(sizeof(uint) + sizeof(int));
            }
            else Stream.UnsafeWrite(NullValue);
        }
        /// <summary>
        /// 序列化可空数组
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="array">数组数据</param>
        internal static void Serialize(unmanagedStream stream, uint?[] array)
        {
            arrayMap arrayMap = new arrayMap(stream, array.Length, array.Length * sizeof(uint));
            uint* write = (uint*)stream.CurrentData;
            foreach (uint? value in array)
            {
                if (value.HasValue)
                {
                    arrayMap.Next(true);
                    *write++ = (uint)value;
                }
                else arrayMap.Next(false);
            }
            stream.UnsafeAddLength((int)((byte*)write - stream.CurrentData));
            arrayMap.End(stream);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.serializeMethod]
        [dataSerializer.serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void serialize(long value)
        {
            Stream.Write(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.memberSerializeMethod]
        [indexSerializer.memberMapSerializeMethod]
        [dataSerializer.memberSerializeMethod]
        [dataSerializer.memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberSerialize(long value)
        {
            Stream.UnsafeWrite(value);
        }
        /// <summary>
        /// 预增数据流长度并写入长度与数据(4字节对齐)
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="data">数据,不能为null</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static unsafe void Serialize(unmanagedStream stream, long[] data)
        {
            fixed (long* dataFixed = data) Serialize(stream, dataFixed, data.Length, sizeof(long));
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.serializeMethod]
        [dataSerializer.serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void serialize(long? value)
        {
            Stream.Write((long)value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.memberSerializeMethod]
        [dataSerializer.memberSerializeMethod]
        [dataSerializer.memberMapSerializeMethod]
        protected void memberSerialize(long? value)
        {
            if (value.HasValue)
            {
                byte* data = Stream.CurrentData;
                *(int*)data = 0;
                *(long*)(data + sizeof(int)) = (long)value;
                Stream.UnsafeAddLength(sizeof(long) + sizeof(int));
            }
            else Stream.UnsafeWrite(NullValue);
        }
        /// <summary>
        /// 序列化可空数组
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="array">数组数据</param>
        internal static void Serialize(unmanagedStream stream, long?[] array)
        {
            arrayMap arrayMap = new arrayMap(stream, array.Length, array.Length * sizeof(long));
            long* write = (long*)stream.CurrentData;
            foreach (long? value in array)
            {
                if (value.HasValue)
                {
                    arrayMap.Next(true);
                    *write++ = (long)value;
                }
                else arrayMap.Next(false);
            }
            stream.UnsafeAddLength((int)((byte*)write - stream.CurrentData));
            arrayMap.End(stream);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.serializeMethod]
        [dataSerializer.serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void serialize(ulong value)
        {
            Stream.Write(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.memberSerializeMethod]
        [indexSerializer.memberMapSerializeMethod]
        [dataSerializer.memberSerializeMethod]
        [dataSerializer.memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberSerialize(ulong value)
        {
            Stream.UnsafeWrite(value);
        }
        /// <summary>
        /// 预增数据流长度并写入长度与数据(4字节对齐)
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="data">数据,不能为null</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static unsafe void Serialize(unmanagedStream stream, ulong[] data)
        {
            fixed (ulong* dataFixed = data) Serialize(stream, dataFixed, data.Length, sizeof(ulong));
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.serializeMethod]
        [dataSerializer.serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void serialize(ulong? value)
        {
            Stream.Write((ulong)value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.memberSerializeMethod]
        [dataSerializer.memberSerializeMethod]
        [dataSerializer.memberMapSerializeMethod]
        protected void memberSerialize(ulong? value)
        {
            if (value.HasValue)
            {
                byte* data = Stream.CurrentData;
                *(int*)data = 0;
                *(ulong*)(data + sizeof(int)) = (ulong)value;
                Stream.UnsafeAddLength(sizeof(ulong) + sizeof(int));
            }
            else Stream.UnsafeWrite(NullValue);
        }
        /// <summary>
        /// 序列化可空数组
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="array">数组数据</param>
        internal static void Serialize(unmanagedStream stream, ulong?[] array)
        {
            arrayMap arrayMap = new arrayMap(stream, array.Length, array.Length * sizeof(ulong));
            ulong* write = (ulong*)stream.CurrentData;
            foreach (ulong? value in array)
            {
                if (value.HasValue)
                {
                    arrayMap.Next(true);
                    *write++ = (ulong)value;
                }
                else arrayMap.Next(false);
            }
            stream.UnsafeAddLength((int)((byte*)write - stream.CurrentData));
            arrayMap.End(stream);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.serializeMethod]
        [dataSerializer.serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void serialize(float value)
        {
            Stream.Write(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.memberSerializeMethod]
        [indexSerializer.memberMapSerializeMethod]
        [dataSerializer.memberSerializeMethod]
        [dataSerializer.memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberSerialize(float value)
        {
            Stream.UnsafeWrite(value);
        }
        /// <summary>
        /// 预增数据流长度并写入长度与数据(4字节对齐)
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="data">数据,不能为null</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static unsafe void Serialize(unmanagedStream stream, float[] data)
        {
            fixed (float* dataFixed = data) Serialize(stream, dataFixed, data.Length, sizeof(float));
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.serializeMethod]
        [dataSerializer.serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void serialize(float? value)
        {
            Stream.Write((float)value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.memberSerializeMethod]
        [dataSerializer.memberSerializeMethod]
        [dataSerializer.memberMapSerializeMethod]
        protected void memberSerialize(float? value)
        {
            if (value.HasValue)
            {
                byte* data = Stream.CurrentData;
                *(int*)data = 0;
                *(float*)(data + sizeof(int)) = (float)value;
                Stream.UnsafeAddLength(sizeof(float) + sizeof(int));
            }
            else Stream.UnsafeWrite(NullValue);
        }
        /// <summary>
        /// 序列化可空数组
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="array">数组数据</param>
        internal static void Serialize(unmanagedStream stream, float?[] array)
        {
            arrayMap arrayMap = new arrayMap(stream, array.Length, array.Length * sizeof(float));
            float* write = (float*)stream.CurrentData;
            foreach (float? value in array)
            {
                if (value.HasValue)
                {
                    arrayMap.Next(true);
                    *write++ = (float)value;
                }
                else arrayMap.Next(false);
            }
            stream.UnsafeAddLength((int)((byte*)write - stream.CurrentData));
            arrayMap.End(stream);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.serializeMethod]
        [dataSerializer.serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void serialize(double value)
        {
            Stream.Write(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.memberSerializeMethod]
        [indexSerializer.memberMapSerializeMethod]
        [dataSerializer.memberSerializeMethod]
        [dataSerializer.memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberSerialize(double value)
        {
            Stream.UnsafeWrite(value);
        }
        /// <summary>
        /// 预增数据流长度并写入长度与数据(4字节对齐)
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="data">数据,不能为null</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static unsafe void Serialize(unmanagedStream stream, double[] data)
        {
            fixed (double* dataFixed = data) Serialize(stream, dataFixed, data.Length, sizeof(double));
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.serializeMethod]
        [dataSerializer.serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void serialize(double? value)
        {
            Stream.Write((double)value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.memberSerializeMethod]
        [dataSerializer.memberSerializeMethod]
        [dataSerializer.memberMapSerializeMethod]
        protected void memberSerialize(double? value)
        {
            if (value.HasValue)
            {
                byte* data = Stream.CurrentData;
                *(int*)data = 0;
                *(double*)(data + sizeof(int)) = (double)value;
                Stream.UnsafeAddLength(sizeof(double) + sizeof(int));
            }
            else Stream.UnsafeWrite(NullValue);
        }
        /// <summary>
        /// 序列化可空数组
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="array">数组数据</param>
        internal static void Serialize(unmanagedStream stream, double?[] array)
        {
            arrayMap arrayMap = new arrayMap(stream, array.Length, array.Length * sizeof(double));
            double* write = (double*)stream.CurrentData;
            foreach (double? value in array)
            {
                if (value.HasValue)
                {
                    arrayMap.Next(true);
                    *write++ = (double)value;
                }
                else arrayMap.Next(false);
            }
            stream.UnsafeAddLength((int)((byte*)write - stream.CurrentData));
            arrayMap.End(stream);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.serializeMethod]
        [dataSerializer.serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void serialize(decimal value)
        {
            Stream.Write(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.memberSerializeMethod]
        [indexSerializer.memberMapSerializeMethod]
        [dataSerializer.memberSerializeMethod]
        [dataSerializer.memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberSerialize(decimal value)
        {
            Stream.UnsafeWrite(value);
        }
        /// <summary>
        /// 预增数据流长度并写入长度与数据(4字节对齐)
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="data">数据,不能为null</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static unsafe void Serialize(unmanagedStream stream, decimal[] data)
        {
            fixed (decimal* dataFixed = data) Serialize(stream, dataFixed, data.Length, sizeof(decimal));
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.serializeMethod]
        [dataSerializer.serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void serialize(decimal? value)
        {
            Stream.Write((decimal)value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [indexSerializer.memberSerializeMethod]
        [dataSerializer.memberSerializeMethod]
        [dataSerializer.memberMapSerializeMethod]
        protected void memberSerialize(decimal? value)
        {
            if (value.HasValue)
            {
                byte* data = Stream.CurrentData;
                *(int*)data = 0;
                *(decimal*)(data + sizeof(int)) = (decimal)value;
                Stream.UnsafeAddLength(sizeof(decimal) + sizeof(int));
            }
            else Stream.UnsafeWrite(NullValue);
        }
        /// <summary>
        /// 序列化可空数组
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="array">数组数据</param>
        internal static void Serialize(unmanagedStream stream, decimal?[] array)
        {
            arrayMap arrayMap = new arrayMap(stream, array.Length, array.Length * sizeof(decimal));
            decimal* write = (decimal*)stream.CurrentData;
            foreach (decimal? value in array)
            {
                if (value.HasValue)
                {
                    arrayMap.Next(true);
                    *write++ = (decimal)value;
                }
                else arrayMap.Next(false);
            }
            stream.UnsafeAddLength((int)((byte*)write - stream.CurrentData));
            arrayMap.End(stream);
        }
        /// <summary>
        /// 字符序列化
        /// </summary>
        /// <param name="value">字符</param>
        [indexSerializer.serializeMethod]
        [dataSerializer.serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void serialize(char value)
        {
            Stream.Write((uint)value);
        }
        /// <summary>
        /// 字符序列化
        /// </summary>
        /// <param name="value">字符</param>
        [indexSerializer.memberSerializeMethod]
        [dataSerializer.memberSerializeMethod]
        [dataSerializer.memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberSerialize(char value)
        {
            Stream.UnsafeWrite(value);
        }
        /// <summary>
        /// 预增数据流长度并写入长度与数据(4字节对齐)
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="data">数据,不能为null</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static unsafe void Serialize(unmanagedStream stream, char[] data)
        {
            fixed (char* dataFixed = data) Serialize(stream, dataFixed, data.Length, sizeof(char));
        }
        /// <summary>
        /// 字符序列化
        /// </summary>
        /// <param name="value">字符</param>
        [indexSerializer.serializeMethod]
        [dataSerializer.serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void serialize(char? value)
        {
            Stream.Write((uint)(char)value);
        }
        /// <summary>
        /// 字符序列化
        /// </summary>
        /// <param name="value">字符</param>
        [indexSerializer.memberSerializeMethod]
        [dataSerializer.memberSerializeMethod]
        [dataSerializer.memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberSerialize(char? value)
        {
            if (value.HasValue) Stream.UnsafeWrite((uint)(char)value);
            else Stream.UnsafeWrite(NullValue);
        }
        /// <summary>
        /// 序列化可空数组
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="array">数组数据</param>
        internal static void Serialize(unmanagedStream stream, char?[] array)
        {
            arrayMap arrayMap = new arrayMap(stream, array.Length, array.Length * sizeof(char));
            char* write = (char*)stream.CurrentData;
            foreach (char? value in array)
            {
                if (value.HasValue)
                {
                    arrayMap.Next(true);
                    *write++ = (char)value;
                }
                else arrayMap.Next(false);
            }
            stream.UnsafeAddLength(((int)((byte*)write - stream.CurrentData) + 3) & (int.MaxValue - 3));
            arrayMap.End(stream);
        }
        /// <summary>
        /// 时间序列化
        /// </summary>
        /// <param name="value">时间</param>
        [indexSerializer.serializeMethod]
        [dataSerializer.serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void serialize(DateTime value)
        {
            Stream.Write(value);
        }
        /// <summary>
        /// 时间序列化
        /// </summary>
        /// <param name="value">时间</param>
        [indexSerializer.memberSerializeMethod]
        [indexSerializer.memberMapSerializeMethod]
        [dataSerializer.memberSerializeMethod]
        [dataSerializer.memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberSerialize(DateTime value)
        {
            Stream.UnsafeWrite(value);
        }
        /// <summary>
        /// 预增数据流长度并写入长度与数据(4字节对齐)
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="data">数据,不能为null</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static unsafe void Serialize(unmanagedStream stream, DateTime[] data)
        {
            fixed (DateTime* dataFixed = data) Serialize(stream, dataFixed, data.Length, sizeof(DateTime));
        }
        /// <summary>
        /// 时间序列化
        /// </summary>
        /// <param name="value">时间</param>
        [indexSerializer.serializeMethod]
        [dataSerializer.serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void serialize(DateTime? value)
        {
            Stream.Write((DateTime)value);
        }
        /// <summary>
        /// 时间序列化
        /// </summary>
        /// <param name="value">时间</param>
        [indexSerializer.memberSerializeMethod]
        [dataSerializer.memberSerializeMethod]
        [dataSerializer.memberMapSerializeMethod]
        protected void memberSerialize(DateTime? value)
        {
            if (value.HasValue)
            {
                byte* data = Stream.CurrentData;
                *(int*)data = 0;
                *(DateTime*)(data + sizeof(int)) = (DateTime)value;
                Stream.UnsafeAddLength(sizeof(DateTime) + sizeof(int));
            }
            else Stream.UnsafeWrite(NullValue);
        }
        /// <summary>
        /// 序列化可空数组
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="array">数组数据</param>
        internal static void Serialize(unmanagedStream stream, DateTime?[] array)
        {
            arrayMap arrayMap = new arrayMap(stream, array.Length, array.Length * sizeof(DateTime));
            DateTime* write = (DateTime*)stream.CurrentData;
            foreach (DateTime? value in array)
            {
                if (value.HasValue)
                {
                    arrayMap.Next(true);
                    *write++ = (DateTime)value;
                }
                else arrayMap.Next(false);
            }
            stream.UnsafeAddLength((int)((byte*)write - stream.CurrentData));
            arrayMap.End(stream);
        }
        /// <summary>
        /// Guid序列化
        /// </summary>
        /// <param name="value">Guid</param>
        [indexSerializer.serializeMethod]
        [dataSerializer.serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void serialize(Guid value)
        {
            Stream.Write(value);
        }
        /// <summary>
        /// Guid序列化
        /// </summary>
        /// <param name="value">Guid</param>
        [indexSerializer.memberSerializeMethod]
        [indexSerializer.memberMapSerializeMethod]
        [dataSerializer.memberSerializeMethod]
        [dataSerializer.memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void memberSerialize(Guid value)
        {
            Stream.UnsafeWrite(value);
        }
        /// <summary>
        /// 预增数据流长度并写入长度与数据(4字节对齐)
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="data">数据,不能为null</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static unsafe void Serialize(unmanagedStream stream, Guid[] data)
        {
            fixed (Guid* dataFixed = data) Serialize(stream, dataFixed, data.Length, sizeof(Guid));
        }
        /// <summary>
        /// Guid序列化
        /// </summary>
        /// <param name="value">Guid</param>
        [indexSerializer.serializeMethod]
        [dataSerializer.serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void serialize(Guid? value)
        {
            Stream.Write((Guid)value);
        }
        /// <summary>
        /// Guid序列化
        /// </summary>
        /// <param name="value">Guid</param>
        [indexSerializer.memberSerializeMethod]
        [dataSerializer.memberSerializeMethod]
        [dataSerializer.memberMapSerializeMethod]
        protected void memberSerialize(Guid? value)
        {
            if (value.HasValue)
            {
                byte* data = Stream.CurrentData;
                *(int*)data = 0;
                *(Guid*)(data + sizeof(int)) = (Guid)value;
                Stream.UnsafeAddLength(sizeof(Guid) + sizeof(int));
            }
            else Stream.UnsafeWrite(NullValue);
        }
        /// <summary>
        /// 序列化可空数组
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="array">数组数据</param>
        internal static void Serialize(unmanagedStream stream, Guid?[] array)
        {
            arrayMap arrayMap = new arrayMap(stream, array.Length, array.Length * sizeof(Guid));
            Guid* write = (Guid*)stream.CurrentData;
            foreach (Guid? value in array)
            {
                if (value.HasValue)
                {
                    arrayMap.Next(true);
                    *write++ = (Guid)value;
                }
                else arrayMap.Next(false);
            }
            stream.UnsafeAddLength((int)((byte*)write - stream.CurrentData));
            arrayMap.End(stream);
        }
        /// <summary>
        /// 字符串序列化
        /// </summary>
        /// <param name="valueFixed"></param>
        /// <param name="stream"></param>
        /// <param name="stringLength"></param>
        private static void serialize(char* valueFixed, unmanagedStream stream, int stringLength)
        {
            char* start = valueFixed, end = valueFixed + stringLength;
            do
            {
                if ((*start & 0xff00) != 0)
                {
                    int length = ((stringLength <<= 1) + (3 + sizeof(int))) & (int.MaxValue - 3);
                    stream.PrepLength(length);
                    start = (char*)stream.CurrentData;
                    fastCSharp.unsafer.memory.Copy(valueFixed, (byte*)start + sizeof(int), *(int*)start = stringLength);
                    stream.UnsafeAddLength(length);
                    stream.PrepLength();
                    return;
                }
            }
            while (++start != end);
            {
                int length = (stringLength + (3 + sizeof(int))) & (int.MaxValue - 3);
                stream.PrepLength(length);
                byte* write = stream.CurrentData;
                *(int*)write = (stringLength << 1) + 1;
                write += sizeof(int);
                do
                {
                    *write++ = (byte)*valueFixed++;
                }
                while (valueFixed != end);
                stream.UnsafeAddLength(length);
                stream.PrepLength();
            }
        }
        /// <summary>
        /// 预增数据流长度并序列化字符串(4字节对齐)
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="value">字符串,不能为null,长度不能为0</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static unsafe void Serialize(unmanagedStream stream, string value)
        {
            //if (value.Length != 0)
            //{
                fixed (char* valueFixed = value) serialize(valueFixed, stream, value.Length);
            //}
            //else stream.Write(0);
        }
        /// <summary>
        /// 字符串序列化
        /// </summary>
        /// <param name="value">字符串</param>
        [indexSerializer.serializeMethod]
        [dataSerializer.serializeMethod]
        [dataSerializer.memberSerializeMethod]
        [dataSerializer.memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void serialize(subString value)
        {
            Serialize(Stream, ref value);
        }
        /// <summary>
        /// 预增数据流长度并序列化字符串(4字节对齐)
        /// </summary>
        /// <param name="stream">序列化数据流</param>
        /// <param name="value">字符串,不能为null</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static unsafe void Serialize(unmanagedStream stream, ref subString value)
        {
            if (value.Length == 0) stream.Write(0);
            else
            {
                fixed (char* valueFixed = value.value) serialize(valueFixed + value.StartIndex, stream, value.Length);
            }
        }

        /// <summary>
        /// 枚举值序列化
        /// </summary>
        /// <param name="value">枚举值序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void enumByteMember<valueType>(valueType value)
        {
            Stream.UnsafeWrite(pub.enumCast<valueType, byte>.ToInt(value));
        }
        /// <summary>
        /// 枚举值序列化
        /// </summary>
        /// <param name="value">枚举值序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void enumSByteMember<valueType>(valueType value)
        {
            Stream.UnsafeWrite(pub.enumCast<valueType, sbyte>.ToInt(value));
        }
        /// <summary>
        /// 枚举值序列化
        /// </summary>
        /// <param name="value">枚举值序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void enumShortMember<valueType>(valueType value)
        {
            Stream.UnsafeWrite(pub.enumCast<valueType, short>.ToInt(value));
        }
        /// <summary>
        /// 枚举值序列化
        /// </summary>
        /// <param name="value">枚举值序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void enumUShortMember<valueType>(valueType value)
        {
            Stream.UnsafeWrite(pub.enumCast<valueType, ushort>.ToInt(value));
        }
        /// <summary>
        /// 枚举值序列化
        /// </summary>
        /// <param name="value">枚举值序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void enumIntMember<valueType>(valueType value)
        {
            Stream.UnsafeWrite(pub.enumCast<valueType, int>.ToInt(value));
        }
        /// <summary>
        /// 枚举值序列化
        /// </summary>
        /// <param name="value">枚举值序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void enumUIntMember<valueType>(valueType value)
        {
            Stream.UnsafeWrite(pub.enumCast<valueType, uint>.ToInt(value));
        }
        /// <summary>
        /// 枚举值序列化
        /// </summary>
        /// <param name="value">枚举值序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void enumLongMember<valueType>(valueType value)
        {
            Stream.UnsafeWrite(pub.enumCast<valueType, long>.ToInt(value));
        }
        /// <summary>
        /// 枚举值序列化
        /// </summary>
        /// <param name="value">枚举值序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void enumULongMember<valueType>(valueType value)
        {
            Stream.UnsafeWrite(pub.enumCast<valueType, ulong>.ToInt(value));
        }
    }
}
