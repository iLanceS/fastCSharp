using System;
using System.Collections.Generic;
using fastCSharp.reflection;
using System.Reflection;
using fastCSharp.threading;

namespace fastCSharp.code.cSharp
{
    /// <summary>
    /// 序列化代码生成自定义属性
    /// </summary>
    public sealed partial class indexSerialize : serializeBase
    {
        /// <summary>
        /// 默认空属性
        /// </summary>
        public static readonly fastCSharp.code.cSharp.indexSerialize SerializeAttribute = new fastCSharp.code.cSharp.indexSerialize { Filter = code.memberFilters.PublicInstance, IsReferenceMember = false };
        /// <summary>
        /// 序列化接口
        /// </summary>
        public interface ISerialize
        {
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="stream">数据流</param>
            void IndexSerialize(unmanagedStream stream);
            /// <summary>
            /// 反序列化
            /// </summary>
            /// <param name="data">序列化数据</param>
            /// <param name="endIndex">数据结束位置</param>
            /// <param name="memberMap">成员位图类型</param>
            /// <param name="memberIndexs">自定义成员位图索引</param>
            bool DeIndexSerialize(subArray<byte> data, out int endIndex, out IMemberMap memberMap, int[] memberIndexs);
        }
        /// <summary>
        /// 序列化接口
        /// </summary>
        /// <typeparam name="memberType">成员位图类型</typeparam>
        public interface ISerialize<memberType> : ISerialize
            where memberType : IMemberMap<memberType>
        {
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="stream">数据流</param>
            /// <param name="memberMap">成员位图接口</param>
            void IndexSerialize(unmanagedStream stream, memberType memberMap);
            /// <summary>
            /// 反序列化
            /// </summary>
            /// <param name="data">序列化数据</param>
            /// <param name="startIndex">起始位置</param>
            /// <param name="endIndex">结束位置</param>
            /// <param name="memberMap">成员位图类型</param>
            /// <param name="memberIndexs">自定义成员位图索引</param>
            bool DeIndexSerialize(byte[] data, int startIndex, out int endIndex, out memberType memberMap, int[] memberIndexs);
        }
        /// <summary>
        /// 对象序列化
        /// </summary>
        public new abstract unsafe class serializer : serialize.unmanagedStreamSerializer
        {
            /// <summary>
            /// 序列化
            /// </summary>
            /// <param name="isReferenceMember">是否检测相同的引用成员</param>
            /// <param name="stream">序列化流</param>
            /// <param name="memberFilter">成员选择</param>
            protected serializer(bool isReferenceMember, unmanagedStream stream, code.memberFilters memberFilter)
                :base(isReferenceMember, stream, memberFilter)
            {
            }
            /// <summary>
            /// 序列化
            /// </summary>
            /// <param name="parentSerializer">序列化</param>
            /// <param name="isReferenceMember">是否检测相同的引用成员</param>
            protected serializer(serializer parentSerializer, bool isReferenceMember)
                : base(parentSerializer, isReferenceMember)
            {
            }
            /// <summary>
            /// 序列化版本号与成员位图
            /// </summary>
            /// <param name="version">版本号</param>
            protected internal override void versionMemerMap(int version) { }
            /// <summary>
            /// 序列化结束
            /// </summary>
            public void Finally()
            {
                int offset = (dataStream.OffsetLength - streamStartIndex) & 3;
                if (offset == 0) dataStream.Write(fastCSharp.emit.binarySerializer.NullValue);
                else
                {
                    dataStream.PrepLength((offset = sizeof(int) - offset) + sizeof(int));
                    dataStream.Unsafer.Write(fastCSharp.emit.binarySerializer.NullValue);
                    dataStream.Unsafer.AddLength(offset);
                    dataStream.PrepLength();
                }
            }
            /// <summary>
            /// 字符串序列化
            /// </summary>
            /// <param name="value">字符串</param>
            protected void indexSerializeString(string value)
            {
                if (value == null) dataStream.Unsafer.Write(fastCSharp.emit.binarySerializer.NullValue);
                else if (value.Length == 0) dataStream.Write((long)(sizeof(int) * 2));
                else if (!isReferenceMember || checkPointNotNull(value))
                {
                    int index = dataStream.Length;
                    dataStream.Unsafer.AddLength(sizeof(int));
                    fastCSharp.emit.dataSerializer.Serialize(dataStream, value);
                    *(int*)(dataStream.Data + index) = dataStream.Length - index;
                }
            }
            /// <summary>
            /// 字节数组序列化
            /// </summary>
            /// <param name="value">字节数组</param>
            protected void byteArray(byte[] value)
            {
                if (value == null) dataStream.Unsafer.Write(fastCSharp.emit.binarySerializer.NullValue);
                else if (!isReferenceMember || value.Length == 0 || checkPointNotNull(value))
                {
                    int index = dataStream.Length;
                    dataStream.Unsafer.AddLength(sizeof(int));
                    fastCSharp.emit.binarySerializer.Serialize(dataStream, value);
                    *(int*)(dataStream.Data + index) = dataStream.Length - index;
                }
            }
            /// <summary>
            /// 序列化接口
            /// </summary>
            /// <typeparam name="valueType">数据类型</typeparam>
            /// <param name="value">序列化接口数据</param>
            protected void indexSerialize<valueType>(valueType value) where valueType : serialize.ISerialize
            {
                if (value == null) dataStream.Unsafer.Write(fastCSharp.emit.binarySerializer.NullValue);
                else if (!isReferenceMember || checkPoint(value)) indexSerializeNoPoint(value);
            }
            /// <summary>
            /// 序列化接口
            /// </summary>
            /// <typeparam name="valueType">数据类型</typeparam>
            /// <param name="value">序列化接口数据</param>
            protected void indexSerializeNoPoint<valueType>(valueType value) where valueType : serialize.ISerialize
            {
                int index = dataStream.Length;
                dataStream.Unsafer.AddLength(sizeof(int));
                Type type = value.GetType();
                if (type == typeof(valueType)) dataStream.Write(fastCSharp.emit.binarySerializer.NullValue);
                else new fastCSharp.code.remoteType(type).Serialize(this);
                value.Serialize(this);
                *(int*)(dataStream.Data + index) = dataStream.Length - index;
            }
            /// <summary>
            /// 序列化接口
            /// </summary>
            /// <typeparam name="valueType">数据类型</typeparam>
            /// <param name="value">序列化接口数据</param>
            protected void indexSerializeNotNull<valueType>(valueType value) where valueType : serialize.ISerialize
            {
                int index = dataStream.Length;
                dataStream.Unsafer.AddLength(sizeof(int));
                value.Serialize(this);
                *(int*)(dataStream.Data + index) = dataStream.Length - index;
            }
            /// <summary>
            /// 未知类型数据序列化
            /// </summary>
            /// <typeparam name="valueType">对象类型</typeparam>
            /// <param name="value">未知类型数据</param>
            protected void indexUnknownNull<valueType>(Nullable<valueType> value) where valueType : struct
            {
                if (value == null) dataStream.Unsafer.Write(fastCSharp.emit.binarySerializer.NullValue);
                else
                {
                    if (!isReferenceMember || checkPointNotNull(value))
                    {
                        int index = dataStream.Length;
                        dataStream.Unsafer.AddLength(sizeof(int));
                        unknownNotNull(value.Value);
                        *(int*)(dataStream.Data + index) = dataStream.Length - index;
                    }
                }
            }
            /// <summary>
            /// 未知类型数据序列化
            /// </summary>
            /// <typeparam name="valueType">对象类型</typeparam>
            /// <param name="value">未知类型数据</param>
            protected void indexUnknown<valueType>(valueType value)
            {
                if (!isReferenceMember || checkPointNotNull(value))
                {
                    int index = dataStream.Length;
                    dataStream.Unsafer.AddLength(sizeof(int));
                    unknownNoPoint(value);
                    *(int*)(dataStream.Data + index) = dataStream.Length - index;
                }
            }
            /// <summary>
            /// 未知值类型数据序列化
            /// </summary>
            /// <typeparam name="valueType">对象类型</typeparam>
            /// <param name="value">值类型数据</param>
            protected void indexUnknownNotNull<valueType>(valueType value)
            {
                int index = dataStream.Length;
                dataStream.Unsafer.AddLength(sizeof(int));
                unknownNotNull(value);
                *(int*)(dataStream.Data + index) = dataStream.Length - index;
            }
        }
        /// <summary>
        /// 对象反序列化
        /// </summary>
        public unsafe abstract class deSerializer<memberType> : serialize.dataDeSerializer
            where memberType : IMemberMap<memberType>
        {
            /// <summary>
            /// 数据结束位置
            /// </summary>
            private byte* readEnd;
            /// <summary>
            /// 成员数据位置集合
            /// </summary>
            protected internal int* members;
            /// <summary>
            /// 成员位图接口
            /// </summary>
            public memberType MemberMap;
            /// <summary>
            /// 对象反序列化
            /// </summary>
            /// <param name="isReferenceMember">是否检测相同的引用成员</param>
            /// <param name="dataStart">序列化数据起始位置</param>
            /// <param name="dataEnd">序列化数据结束位置</param>
            protected deSerializer(bool isReferenceMember, byte* dataStart, byte* dataEnd)
                : base(isReferenceMember, dataStart, dataEnd)
            {
            }
            /// <summary>
            /// 设置成员数据位置
            /// </summary>
            /// <param name="memberCount">成员数量</param>
            /// <param name="memberIndexs">自定义成员位图索引</param>
            /// <returns>是否成功</returns>
            protected internal unsafe bool setMemberIndex(int memberCount, int[] memberIndexs)
            {
                this.readEnd = Read;
                if (memberIndexs == null)
                {
                    while (*(int*)readEnd != fastCSharp.emit.binarySerializer.NullValue)
                    {
                        int memberIndex = *(int*)readEnd, length = *(int*)(readEnd += sizeof(int));
                        if ((uint)memberIndex >= memberCount) return false;
                        members[memberIndex] = (int)(readEnd - dataStart);
                        MemberMap.SetMember(memberIndex);
                        if (length <= 0) readEnd += sizeof(int);
                        else readEnd += length;
                        if (readEnd > dataEnd) return false;
                    }
                }
                else
                {
                    fixed (int* memberIndexFixed = memberIndexs)
                    {
                        while (*(int*)readEnd != fastCSharp.emit.binarySerializer.NullValue)
                        {
                            int memberIndex = *(int*)readEnd, length = *(int*)(readEnd += sizeof(int));
                            if ((uint)memberIndex >= memberIndexs.Length) return false;
                            if ((memberIndex = memberIndexFixed[memberIndex]) != fastCSharp.emit.binarySerializer.NullValue)
                            {
                                members[memberIndex] = (int)(readEnd - dataStart);
                                MemberMap.SetMember(memberIndex);
                            }
                            if (length <= 0) readEnd += sizeof(int);
                            else readEnd += length;
                            if (readEnd > dataEnd) return false;
                        }
                    }
                }
                return true;
            }
            /// <summary>
            /// 设置读取数据结束位置
            /// </summary>
            protected internal void setReadEnd()
            {
                int offset = (int)(readEnd - dataStart) & 3;
                Read = offset == 0 ? readEnd + sizeof(int) : (readEnd + (sizeof(int) * 2 - offset));
            }
            /// <summary>
            /// 反序列化字符串
            /// </summary>
            /// <param name="memberIndex">成员索引</param>
            /// <returns>字符串</returns>
            protected internal string getString(int memberIndex)
            {
                byte* read = dataStart + members[memberIndex];
                int length = *(int*)read;
                if (length == fastCSharp.emit.binarySerializer.NullValue) return null;
                if (length < 0) return (string)points[length];
                int point = (int)(read - dataStart), stringLength = *(int*)(read += sizeof(int));
                if ((stringLength & 1) == 0)
                {
                    if (stringLength != 0)
                    {
                        if ((length -= sizeof(int) * 2) < stringLength) fastCSharp.log.Default.Throw("数据不足 " + length.toString() + " < " + stringLength.toString(), true, false);
                        string value = new string((char*)(read + sizeof(int)), 0, stringLength >> 1);
                        if (isReferenceMember) points.Add(-point, value);
                        return value;
                    }
                }
                else if (stringLength > 1)
                {
                    if ((length -= sizeof(int) * 2) < (stringLength >>= 1)) fastCSharp.log.Default.Throw("数据不足 " + length.toString() + " < " + stringLength.toString(), true, false);
                    string value = fastCSharp.String.FastAllocateString(stringLength);
                    fixed (char* valueFixed = value)
                    {
                        char* start = valueFixed;
                        for (byte* end = (read += sizeof(int)) + stringLength; read != end; *start++ = (char)*read++) ;
                    }
                    if (isReferenceMember) points.Add(-point, value);
                    return value;
                }
                return string.Empty;
            }
            /// <summary>
            /// 反序列化字节数组
            /// </summary>
            /// <param name="memberIndex">成员索引</param>
            /// <returns>字节数组</returns>
            protected byte[] byteArray(int memberIndex)
            {
                byte* read = dataStart + members[memberIndex];
                int length = *(int*)read;
                if (length == fastCSharp.emit.binarySerializer.NullValue) return null;
                if (length < 0) return (byte[])points[length];
                int point = (int)(read - dataStart), arrayLength = *(int*)(read += sizeof(int));
                if ((length -= sizeof(int) * 2) < arrayLength) fastCSharp.log.Default.Throw("数据不足 " + length.toString() + " < " + arrayLength.toString(), true, false);
                byte[] array = new byte[length];
                if (isReferenceMember && length != 0) points.Add(-point, array);
                unsafer.memory.Copy(read + sizeof(int), array, arrayLength);
                return array;
            }
            /// <summary>
            /// 序列化接口数据
            /// </summary>
            /// <typeparam name="valueType">数据类型</typeparam>
            /// <param name="memberIndex">成员索引</param>
            /// <param name="newValue">获取新数据委托</param>
            /// <returns>序列化接口数据</returns>
            protected valueType iSerialize<valueType>(int memberIndex, Func<valueType> newValue) where valueType : serialize.ISerialize
            {
                int length = *(int*)(Read = dataStart + members[memberIndex]);
                if (length == fastCSharp.emit.binarySerializer.NullValue) return default(valueType);
                if (length <= 0) return (valueType)points[length];
                int point = (int)(Read - dataStart);
                valueType value;
                if (*(int*)(Read += sizeof(int)) == fastCSharp.emit.binarySerializer.NullValue)
                {
                    Read += sizeof(int);
                    value = newValue != null ? newValue() : fastCSharp.emit.constructor<valueType>.New();
                }
                else
                {
                    fastCSharp.code.remoteType remoteType = new fastCSharp.code.remoteType();
                    if (remoteType.DeSerialize(this))
                    {
                        Type type = remoteType.Type;
                        if (!type.isInherit(typeof(valueType))) log.Default.Throw(type.fullName() + " 不继承 " + typeof(valueType).fullName() + " ,无法反序列化", false, false);
                        value = (valueType)fastCSharp.emit.constructor.Get(type);
                    }
                    else
                    {
                        log.Default.Throw("remoteType 反序列化失败", true, false);
                        value = default(valueType);
                    }
                }
                if (isReferenceMember) points.Add(-point, value);
                value.DeSerialize(this);
                return value;
            }
            /// <summary>
            /// 序列化接口数组
            /// </summary>
            /// <typeparam name="valueType">数据类型</typeparam>
            /// <param name="memberIndex">成员索引</param>
            /// <returns>序列化接口数据</returns>
            protected valueType iSerializeNotNull<valueType>(int memberIndex) where valueType : serialize.ISerialize
            {
                valueType value = default(valueType);
                Read = dataStart + (members[memberIndex] + sizeof(int));
                value.DeSerialize(this);
                return value;
            }
            /// <summary>
            /// 反序列化值类型数据
            /// </summary>
            /// <typeparam name="valueType">数据类型</typeparam>
            /// <param name="memberIndex">成员索引</param>
            /// <param name="array">值类型数据</param>
            protected Nullable<valueType> unknownNull<valueType>(int memberIndex) where valueType : struct
            {
                Read = dataStart + (members[memberIndex] + sizeof(int));
                return unknownNotNull<valueType>();
            }
            /// <summary>
            /// 反序列化未知类型数据
            /// </summary>
            /// <param name="memberIndex">成员索引</param>
            /// <returns>数组数据</returns>
            protected valueType unknown<valueType>(int memberIndex)
            {
                int length = *(int*)(Read = dataStart + members[memberIndex]);
                if (length == fastCSharp.emit.binarySerializer.NullValue) return default(valueType);
                if (length <= 0) return (valueType)points[length];
                int point = (int)(Read - dataStart);
                if (*(int*)(Read += sizeof(int)) == fastCSharp.emit.binarySerializer.NullValue)
                {
                    Read += sizeof(int);
                    return serialize.deSerialize<valueType>.GetVersionMemerMap(this, point);
                }
                fastCSharp.code.remoteType remoteType = new fastCSharp.code.remoteType();
                if (remoteType.DeSerialize(this))
                {
                    Type type = remoteType.Type;
                    if (typeof(valueType) != typeof(object) && !type.isInherit(typeof(valueType))) log.Default.Throw(type.fullName() + " 不继承 " + typeof(valueType).fullName() + " ,无法反序列化", false, false);
                    return (valueType)serializeBase.unknownValue.GetValue(typeof(valueType), type, ((Func<serialize.dataDeSerializer, object>)Delegate.CreateDelegate(typeof(Func<serialize.dataDeSerializer, object>), unknownNotNullMethod.MakeGenericMethod(serializeBase.unknownValue.GetGenericType(type))))(this));
                }
                log.Default.Throw("remoteType 反序列化失败", true, false);
                return default(valueType);
            }
            /// <summary>
            /// 反序列化值类型数据
            /// </summary>
            /// <param name="memberIndex">成员索引</param>
            /// <typeparam name="valueType">数据类型</typeparam>
            protected valueType unknownNotNull<valueType>(int memberIndex)
            {
                Read = dataStart + (members[memberIndex] + sizeof(int));
                return serialize.deSerialize<valueType>.GetVersionMemerMap(this, (int)(Read - dataStart));
            }
        }
        /// <summary>
        /// 对象序列化器(反射模式)
        /// </summary>
        internal sealed class reflectionDataSerializer : serializer
        {
            /// <summary>
            /// 对象序列化器
            /// </summary>
            /// <param name="isReferenceMember">是否检测相同的引用成员</param>
            /// <param name="stream">序列化流</param>
            /// <param name="memberFilter">成员选择</param>
            public reflectionDataSerializer(bool isReferenceMember, unmanagedStream stream, code.memberFilters memberFilter)
                : base(isReferenceMember, stream, memberFilter)
            {
            }
            /// <summary>
            /// 序列化版本号与成员位图
            /// </summary>
            /// <param name="version">版本号</param>
            protected internal override void versionMemerMap(int version)
            {
            }
        }
        /// <summary>
        /// 对象序列化(反射模式)
        /// </summary>
        public static class dataSerialize
        {
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <typeparam name="valueType">对象类型</typeparam>
            /// <param name="stream">内存数据流</param>
            /// <param name="value">数据对象</param>
            /// <param name="filter">成员选择,默认为公共字段成员</param>
            /// <param name="memberMap">成员位图</param>
            public static void Get<valueType>(unmanagedStream stream, valueType value
                    , code.memberFilters filter = code.memberFilters.InstanceField, memberMap<valueType> memberMap = default(memberMap<valueType>))
            {
                dataSerialize<valueType>.Get(stream, value, filter, memberMap);
            }
            /// <summary>
            /// 序列化接口
            /// </summary>
            /// <typeparam name="valueType">数据类型</typeparam>
            /// <param name="stream">内存数据流</param>
            /// <param name="value">序列化接口数据</param>
            private static void getISerializeType<valueType>(unmanagedStream stream, valueType value) where valueType : ISerialize
            {
                value.IndexSerialize(stream);
            }
            /// <summary>
            /// 可空类型
            /// </summary>
            /// <typeparam name="valueType">数据类型</typeparam>
            /// <param name="serializer">反序列化器</param>
            /// <returns>可空类型数据</returns>
            private static void getNullType<valueType>(unmanagedStream stream, Nullable<valueType> value) where valueType : struct
            {
                Get<valueType>(stream, value.Value, code.memberFilters.InstanceField, default(memberMap<valueType>));
            }
        }
        /// <summary>
        /// 对象序列化(反射模式)
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        internal sealed class dataSerialize<valueType> : serialize.dataSerialize<valueType, indexSerialize>
        {
            /// <summary>
            /// 对象反序列化
            /// </summary>
            private static readonly Action<unmanagedStream, valueType> indexSerializeGetter;
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="stream">内存数据流</param>
            /// <param name="value">数据对象</param>
            /// <param name="filter">成员选择</param>
            /// <param name="memberMap">成员位图</param>
            /// <returns>序列化数据</returns>
            public static void Get(unmanagedStream stream, valueType value, code.memberFilters filter, memberMap<valueType> memberMap)
            {
                if ((!isStruct || nullType != null) && value == null) stream.Write(fastCSharp.emit.binarySerializer.NullValue);
                else if (indexSerializeGetter == null)
                {
                    if (getter == null)
                    {
                        reflectionDataSerializer serializer = new reflectionDataSerializer(serializeAttribute.IsReferenceMember, stream, filter);
                        if (!isStruct && serializeAttribute.IsReferenceMember) serializer.points[new objectReference { Value = value }] = 0;
                        getMember(serializer, value, memberMap);
                        serializer.Finally();
                    }
                    else
                    {
                        serialize.reflectionDataSerializer serializer = new serialize.reflectionDataSerializer(serializeAttribute.IsReferenceMember, stream, memberMap, filter);
                        if (!isStruct) serializer.checkPointReferenceMember(value);
                        getter(serializer, value);
                        serializer.Finally();
                    }
                }
                else indexSerializeGetter(stream, value);
            }
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="serializer">对象序列化器</param>
            /// <param name="value">数据对象</param>
            /// <param name="memberMap">成员位图</param>
            private unsafe static void getMember(reflectionDataSerializer serializer, valueType value, memberMap<valueType> memberMap)
            {
                subArray<keyValue<code.memberInfo, object>> memberValues = memberGroup.GetMemberValue(value, serializer.memberFilter, memberMap);
                int memberValueCount = memberValues.Count;
                if (memberValueCount != 0)
                {
                    unmanagedStream dataStream = serializer.dataStream;
                    foreach (keyValue<code.memberInfo, object> memberValue in memberValues.Array)
                    {
                        int memberIndex = memberValue.Key.MemberIndex;
                        Func<object, object> converter = converters[memberIndex];
                        object objectValue = converter == null ? memberValue.Value : converter(memberValue.Value);
                        dataStream.PrepLength(sizeof(int) + sizeof(int) + sizeof(Guid));
                        dataStream.Unsafer.Write(memberIndex);
                        if (isMemberSerializeMap.Get(memberIndex))
                        {
                            if (objectValue == null) dataStream.Unsafer.Write(sizeof(int));
                            else
                            {
                                byte* start = dataStream.CurrentData;
                                serializer.write = start + sizeof(int);
                                memberGetters[memberIndex](serializer, objectValue);
                                int length = (int)(serializer.write - start);
                                *(int*)start = length;
                                dataStream.Unsafer.AddLength(length);
                            }
                        }
                        else if (objectValue == null) dataStream.Unsafer.Write(fastCSharp.emit.binarySerializer.NullValue);
                        else if (serializer.checkPointReferenceMember(memberValue.Value))
                        {
                            int index = dataStream.Length;
                            dataStream.Unsafer.AddLength(sizeof(int));
                            memberGetters[memberIndex](serializer, objectValue);
                            *(int*)(dataStream.Data + index) = dataStream.Length - index;
                        }
                        dataStream.PrepLength();
                        if (--memberValueCount == 0) break;
                    }
                }
            }
            static dataSerialize()
            {
                if (!serialize.dataSerialize<valueType, indexSerialize>.isUnknownValue)
                {
                    if (isIndexSerialize)
                    {
                        indexSerializeGetter = (Action<unmanagedStream, valueType>)Delegate.CreateDelegate(typeof(Action<unmanagedStream, valueType>), typeof(dataSerialize).GetMethod("getISerializeType", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(typeof(valueType)));
                        return;
                    }
                    if (isStruct && nullType != null && !((memberType)typeof(valueType)).IsMemberSerialize)
                    {
                        indexSerializeGetter = (Action<unmanagedStream, valueType>)Delegate.CreateDelegate(typeof(Action<unmanagedStream, valueType>), typeof(deSerialize).GetMethod("getNullType", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(nullType));
                    }
                }
            }
        }
        /// <summary>
        /// 对象反序列化器(反射模式)
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        internal unsafe class reflectionDeSerializer<valueType> : deSerializer<memberMap<valueType>>
        {
            /// <summary>
            /// 对象反序列化器
            /// </summary>
            /// <param name="dataStart">序列化数据起始位置</param>
            /// <param name="dataEnd">序列化数据结束位置</param>
            public reflectionDeSerializer(byte* dataStart, byte* dataEnd)
                : base(serializer<valueType, serialize>.serializeAttribute.IsReferenceMember, dataStart, dataEnd)
            {
            }
        }
        /// <summary>
        /// 对象反序列化(反射模式)
        /// </summary>
        public static class deSerialize
        {
            /// <summary>
            /// 序列化输入
            /// </summary>
            internal struct serializeInput
            {
                /// <summary>
                /// 序列化数据
                /// </summary>
                public subArray<byte> Data;
                /// <summary>
                /// 自定义成员位图索引
                /// </summary>
                public int[] MemberIndexs;
            }
            /// <summary>
            /// 对象反序列化
            /// </summary>
            /// <typeparam name="valueType">对象类型</typeparam>
            /// <param name="value">目标对象</param>
            /// <param name="data">序列化数据</param>
            /// <param name="endIndex">数据结束位置</param>
            /// <param name="memberMap">成员位图</param>
            /// <param name="memberIndexs">自定义成员位图索引</param>
            /// <returns>是否成功</returns>
            public static bool Get<valueType>(ref valueType value, subArray<byte> data, out int endIndex, out memberMap<valueType> memberMap, int[] memberIndexs)
            {
                return deSerialize<valueType>.Get(ref value, data, out endIndex, out memberMap, memberIndexs);
            }
            /// <summary>
            /// 序列化接口
            /// </summary>
            /// <typeparam name="valueType">数据类型</typeparam>
            /// <param name="input">输入数据</param>
            /// <returns>输入数据</returns>
            private static deSerialize<valueType>.serializeOutput getISerializeType<valueType>(serializeInput input) where valueType : ISerialize
            {
                deSerialize<valueType>.serializeOutput output = new deSerialize<valueType>.serializeOutput { Value = fastCSharp.emit.constructor<valueType>.New() };
                output.Value.DeIndexSerialize(input.Data, out output.EndIndex, out output.MemberMap, input.MemberIndexs);
                return output;
            }
            /// <summary>
            /// 可空类型
            /// </summary>
            /// <typeparam name="valueType">数据类型</typeparam>
            /// <param name="serializer">反序列化器</param>
            /// <returns>可空类型数据</returns>
            private static deSerialize<Nullable<valueType>>.serializeOutput getNullType<valueType>(serializeInput input) where valueType : struct
            {
                deSerialize<Nullable<valueType>>.serializeOutput output = new deSerialize<Nullable<valueType>>.serializeOutput { };
                valueType value = default(valueType);
                memberMap<valueType> memberMap;
                if (Get<valueType>(ref value, input.Data, out output.EndIndex, out memberMap, input.MemberIndexs))
                {
                    output.Value = value;
                    output.MemberMap = memberMap;
                }
                return output;
            }
        }
        /// <summary>
        /// 对象反序列化(反射模式)
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        internal sealed class deSerialize<valueType> : serialize.dataDeSerialize<valueType, indexSerialize>
        {
            /// <summary>
            /// 序列化输出
            /// </summary>
            public struct serializeOutput
            {
                /// <summary>
                /// 序列化对象
                /// </summary>
                public valueType Value;
                /// <summary>
                /// 成员位图
                /// </summary>
                public IMemberMap MemberMap;
                /// <summary>
                /// 数据结束位置
                /// </summary>
                public int EndIndex;
            }
            /// <summary>
            /// 对象反序列化
            /// </summary>
            private static readonly Func<deSerialize.serializeInput, serializeOutput> indexSerializeGetter;
            /// <summary>
            /// 对象反序列化
            /// </summary>
            /// <param name="value">数据对象</param>
            /// <param name="data">序列化数据</param>
            /// <param name="endIndex">数据结束位置</param>
            /// <param name="memberMap">成员位图</param>
            /// <param name="memberIndexs">自定义成员位图索引</param>
            /// <returns>是否成功</returns>
            public unsafe static bool Get(ref valueType value, subArray<byte> data, out int endIndex, out memberMap<valueType> memberMap, int[] memberIndexs)
            {
                fixed (byte* dataFixed = data.Array)
                {
                    byte* dataStart = dataFixed + data.StartIndex;
                    if (*(int*)dataStart == fastCSharp.emit.binarySerializer.NullValue)
                    {
                        value = default(valueType);
                        endIndex = data.StartIndex + sizeof(int);
                        memberMap = default(memberMap<valueType>);
                    }
                    else
                    {
                        if (indexSerializeGetter == null)
                        {
                            if (getter == null)
                            {
                                reflectionDeSerializer<valueType> deSerializer = new reflectionDeSerializer<valueType>(dataStart, dataStart + data.Count);
                                if (!isStruct && deSerializer.isReferenceMember) deSerializer.points.Add(-sizeof(int), value);
                                getMember(deSerializer, memberIndexs, ref value);
                                memberMap = deSerializer.MemberMap;
                                deSerializer.setReadEnd();
                                endIndex = (int)(deSerializer.Read - dataFixed);
                            }
                            else
                            {
                                serialize.reflectionDeSerializer<valueType> deSerializer = new serialize.reflectionDeSerializer<valueType>(dataStart, dataStart + data.Count);
                                value = getter(deSerializer);
                                memberMap = deSerializer.MemberMap;
                                //(memberMap = default(memberMap<valueType>)).CopyFrom(deSerializer.MemberMap);
                                if (deSerializer.checkEnd())
                                {
                                    endIndex = (int)(deSerializer.Read - dataFixed);
                                    return true;
                                }
                                endIndex = -1;
                                return false;
                            }
                        }
                        else
                        {
                            serializeOutput output = indexSerializeGetter(new deSerialize.serializeInput { Data = data, MemberIndexs = memberIndexs });
                            value = output.Value;
                            memberMap = default(memberMap<valueType>);
                            endIndex = output.EndIndex;
                            if (output.MemberMap != null)
                            {
                                memberMap.CopyFrom(output.MemberMap);
                                output.MemberMap.PushPool();
                            }
                        }
                    }
                }
                return true;
            }
            /// <summary>
            /// 对象反序列化
            /// </summary>
            /// <param name="deSerializer">对象反序列化器</param>
            /// <param name="memberIndexs">自定义成员位图索引</param>
            /// <param name="value">数据对象</param>
            private unsafe static void getMember(reflectionDeSerializer<valueType> deSerializer, int[] memberIndexs, ref valueType value)
            {
                int* members = stackalloc int[memberCount];
                deSerializer.members = members;
                if (deSerializer.setMemberIndex(memberCount, memberIndexs))
                {
                    object[] values = new object[memberCount];
                    byte* isValueFixed = stackalloc byte[memberMapSize];
                    fixedMap isValueMap = new fixedMap(isValueFixed, memberMapSize);
                    memberMap<valueType> memberMap = deSerializer.MemberMap;
                    object reference;
                    for (int* memberIndexStart = memberSort.Int, memberIndexEnd = memberIndexStart + memberGroup.Count; memberIndexStart != memberIndexEnd; ++memberIndexStart)
                    {
                        int memberIndex = *memberIndexStart;
                        if (memberMap.IsMember(memberIndex))
                        {
                            byte* read = deSerializer.dataStart + members[memberIndex];
                            if (isMemberSerializeMap.Get(memberIndex))
                            {
                                if (*(int*)read == sizeof(int)) values[memberIndex] = defaultValues[memberIndex];
                                else
                                {
                                    deSerializer.Read = read + sizeof(int);
                                    Func<object, object> converter = converters[memberIndex];
                                    reference = memberGetters[memberIndex](deSerializer);
                                    values[memberIndex] = converter == null ? reference : converter(reference);
                                }
                            }
                            else
                            {
                                int length = *(int*)read;
                                if (length == fastCSharp.emit.binarySerializer.NullValue) values[memberIndex] = defaultValues[memberIndex];
                                else
                                {
                                    Func<object, object> converter = converters[memberIndex];
                                    if (length <= 0) reference = deSerializer.points[length];
                                    else
                                    {
                                        deSerializer.Read = read + sizeof(int);
                                        reference = memberGetters[memberIndex](deSerializer);
                                    }
                                    values[memberIndex] = converter == null ? reference : converter(reference);
                                }
                            }
                            isValueMap.Set(memberIndex);
                        }
                    }
                    if (isStruct) value = memberGroup.SetMemberValue(value, values, isValueMap);
                    else memberGroup.SetMember(value, values, isValueMap);
                }
                else value = default(valueType);
            }
            static deSerialize()
            {
                if (isIndexSerialize)
                {
                    indexSerializeGetter = (Func<deSerialize.serializeInput, serializeOutput>)Delegate.CreateDelegate(typeof(Func<deSerialize.serializeInput, serializeOutput>), typeof(deSerialize).GetMethod("getISerializeType", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(typeof(valueType)));
                    return;
                }
                if (isStruct && nullType != null && !((memberType)typeof(valueType)).IsMemberSerialize)
                {
                    indexSerializeGetter = (Func<deSerialize.serializeInput, serializeOutput>)Delegate.CreateDelegate(typeof(Func<deSerialize.serializeInput, serializeOutput>), typeof(deSerialize).GetMethod("getNullType", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(nullType));
                }
            }
        }
    }
}
