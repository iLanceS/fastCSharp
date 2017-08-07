using System;
using fastCSharp.code;
using fastCSharp.threading;
using System.Reflection;
using System.Collections.Generic;
using fastCSharp.reflection;
using System.Runtime.CompilerServices;
using System.Threading;
#if NOJIT
#else
using System.Reflection.Emit;
#endif

namespace fastCSharp.emit
{
    /// <summary>
    /// 二进制数据序列化
    /// </summary>
    public unsafe sealed class dataSerializer : binarySerializer
    {
        /// <summary>
        /// 真实类型
        /// </summary>
        public const int RealTypeValue = NullValue + 1;
        /// <summary>
        /// 配置参数
        /// </summary>
        public new sealed class config : binarySerializer.config
        {
            /// <summary>
            /// 是否检测引用类型对象的真实类型
            /// </summary>
            internal const int ObjectRealTypeValue = 2;
            /// <summary>
            /// 是否序列化成员位图
            /// </summary>
            public bool IsMemberMap;
            /// <summary>
            /// 是否检测引用类型对象的真实类型
            /// </summary>
            public bool IsRealType;
            /// <summary>
            /// 是否随机填充空白数据
            /// </summary>
            public bool IsRandom;
            /// <summary>
            /// 序列化头部数据
            /// </summary>
            internal override int HeaderValue
            {
                get
                {
                    int value = base.HeaderValue;
                    if (IsRealType) value += ObjectRealTypeValue;
                    return value;
                }
            }
        }
        /// <summary>
        /// 基本类型序列化函数
        /// </summary>
        internal sealed class serializeMethod : Attribute { }
        /// <summary>
        /// 基本类型序列化函数
        /// </summary>
        internal sealed class memberSerializeMethod : Attribute { }
        /// <summary>
        /// 基本类型序列化函数
        /// </summary>
        internal sealed class memberMapSerializeMethod : Attribute { }
        /// <summary>
        /// 二进制数据序列化
        /// </summary>
        internal static class staticTypeSerializer
        {
#if NOJIT
#else
            /// <summary>
            /// 动态函数
            /// </summary>
            public struct memberDynamicMethod
            {
                /// <summary>
                /// 动态函数
                /// </summary>
                private DynamicMethod dynamicMethod;
                /// <summary>
                /// 
                /// </summary>
                private ILGenerator generator;
                /// <summary>
                /// 是否值类型
                /// </summary>
                private bool isValueType;
                /// <summary>
                /// 动态函数
                /// </summary>
                /// <param name="type"></param>
                public memberDynamicMethod(Type type)
                {
                    dynamicMethod = new DynamicMethod("dataSerializer", null, new Type[] { typeof(dataSerializer), type }, type, true);
                    generator = dynamicMethod.GetILGenerator();
                    isValueType = type.IsValueType;
                }
                /// <summary>
                /// 添加字段
                /// </summary>
                /// <param name="field">字段信息</param>
                public void Push(fieldInfo field)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    if (isValueType) generator.Emit(OpCodes.Ldarga_S, 1);
                    else generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Ldfld, field.Field);
                    MethodInfo method = dataSerializer.getMemberSerializeMethod(field.Field.FieldType) ?? GetMemberSerializer(field.Field.FieldType);
                    generator.Emit(method.IsFinal || !method.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, method);
                }
                /// <summary>
                /// 创建成员转换委托
                /// </summary>
                /// <returns>成员转换委托</returns>
                public Delegate Create<delegateType>()
                {
                    generator.Emit(OpCodes.Ret);
                    return dynamicMethod.CreateDelegate(typeof(delegateType));
                }
            }
            /// <summary>
            /// 动态函数
            /// </summary>
            public struct memberMapDynamicMethod
            {
                /// <summary>
                /// 动态函数
                /// </summary>
                private DynamicMethod dynamicMethod;
                /// <summary>
                /// 
                /// </summary>
                private ILGenerator generator;
                /// <summary>
                /// 是否值类型
                /// </summary>
                private bool isValueType;
                /// <summary>
                /// 动态函数
                /// </summary>
                /// <param name="type"></param>
                public memberMapDynamicMethod(Type type)
                {
                    dynamicMethod = new DynamicMethod("dataMemberMapSerializer", null, new Type[] { typeof(memberMap), typeof(dataSerializer), type }, type, true);
                    generator = dynamicMethod.GetILGenerator();
                    isValueType = type.IsValueType;
                }
                /// <summary>
                /// 添加字段
                /// </summary>
                /// <param name="field">字段信息</param>
                public void Push(fieldInfo field)
                {
                    Label end = generator.DefineLabel();
                    generator.memberMapIsMember(OpCodes.Ldarg_0, field.MemberIndex);
                    generator.Emit(OpCodes.Brfalse_S, end);

                    generator.Emit(OpCodes.Ldarg_1);
                    if (isValueType) generator.Emit(OpCodes.Ldarga_S, 2);
                    else generator.Emit(OpCodes.Ldarg_2);
                    generator.Emit(OpCodes.Ldfld, field.Field);
                    MethodInfo method = dataSerializer.getMemberMapSerializeMethod(field.Field.FieldType) ?? GetMemberSerializer(field.Field.FieldType);
                    generator.Emit(method.IsFinal || !method.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, method);

                    generator.MarkLabel(end);
                }
                /// <summary>
                /// 创建成员转换委托
                /// </summary>
                /// <returns>成员转换委托</returns>
                public Delegate Create<delegateType>()
                {
                    generator.Emit(OpCodes.Ret);
                    return dynamicMethod.CreateDelegate(typeof(delegateType));
                }
            }
#endif
            /// <summary>
            /// 获取字段成员集合
            /// </summary>
            /// <param name="fieldIndexs"></param>
            /// <param name="memberCountVerify"></param>
            /// <returns>字段成员集合</returns>
            public static fields<fieldInfo> GetFields(fieldIndex[] fieldIndexs, out int memberCountVerify)
            {
                subArray<fieldInfo> fixedFields = new subArray<fieldInfo>(fieldIndexs.Length), fields = new subArray<fieldInfo>();
                subArray<fieldIndex> jsonFields = new subArray<fieldIndex>();
                fields.UnsafeSet(fixedFields.array, fixedFields.array.length(), 0);
                int fixedSize = 0;
                foreach (fieldIndex field in fieldIndexs)
                {
                    Type type = field.Member.FieldType;
                    if (!type.IsPointer && (!type.IsArray || type.GetArrayRank() == 1) && !field.IsIgnore)
                    {
                        dataSerialize.member attribute = field.GetAttribute<dataSerialize.member>(true, true);
                        if (attribute == null || attribute.IsSetup)
                        {
                            if (attribute != null && attribute.IsJson) jsonFields.Add(field);
                            else
                            {
                                fieldInfo value = new fieldInfo(field);
                                if (value.FixedSize == 0) fields.UnsafeAddExpand(value);
                                else
                                {
                                    fixedFields.Add(value);
                                    fixedSize += value.FixedSize;
                                }
                            }
                        }
                    }
                }
                memberCountVerify = fixedFields.length + fields.length + jsonFields.length + 0x40000000;
                return new fields<fieldInfo> { FixedFields = fixedFields.Sort(fieldInfo.FixedSizeSort), Fields = fields, JsonFields = jsonFields, FixedSize = fixedSize };
            }
            /// <summary>
            /// 获取字段成员集合
            /// </summary>
            /// <param name="fieldIndexs"></param>
            /// <returns>字段成员集合</returns>
            public static subArray<memberIndex> GetMembers(fieldIndex[] fieldIndexs)
            {
                subArray<memberIndex> fields = new subArray<memberIndex>(fieldIndexs.Length);
                foreach (fieldIndex field in fieldIndexs)
                {
                    Type type = field.Member.FieldType;
                    if (!type.IsPointer && (!type.IsArray || type.GetArrayRank() == 1) && !field.IsIgnore)
                    {
                        dataSerialize.member attribute = field.GetAttribute<dataSerialize.member>(true, true);
                        if (attribute == null || attribute.IsSetup) fields.Add(field);
                    }
                }
                return fields;
            }
            /// <summary>
            /// 获取自定义序列化函数信息
            /// </summary>
            /// <param name="type"></param>
            /// <param name="isSerializer"></param>
            /// <returns></returns>
            public static MethodInfo GetCustom(Type type, bool isSerializer)
            {
                MethodInfo serializeMethod = null, deSerializeMethod = null;
                Type refType = type.MakeByRefType();
                foreach (fastCSharp.code.attributeMethod method in fastCSharp.code.attributeMethod.GetStatic(type))
                {
                    if (method.Method.ReturnType == typeof(void)
                        && method.GetAttribute<dataSerialize.custom>(true) != null)
                    {
                        ParameterInfo[] parameters = method.Method.GetParameters();
                        if (parameters.Length == 2)
                        {
                            if (parameters[0].ParameterType == typeof(dataSerializer))
                            {
                                if (parameters[1].ParameterType == type)
                                {
                                    if (deSerializeMethod != null) return isSerializer ? method.Method : deSerializeMethod;
                                    serializeMethod = method.Method;
                                }
                            }
                            else if (parameters[0].ParameterType == typeof(dataDeSerializer))
                            {
                                if (parameters[1].ParameterType == refType)
                                {
                                    if (serializeMethod != null) return isSerializer ? serializeMethod : method.Method;
                                    deSerializeMethod = method.Method;
                                }
                            }
                        }
                    }
                }
                return null;
            }

            /// <summary>
            /// 未知类型序列化调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> memberSerializers = new interlocked.dictionary<Type,MethodInfo>();
            /// <summary>
            /// 未知类型枚举序列化委托调用函数信息
            /// </summary>
            /// <param name="type">数组类型</param>
            /// <returns>未知类型序列化委托调用函数信息</returns>
            public static MethodInfo GetMemberSerializer(Type type)
            {
                MethodInfo method;
                if (memberSerializers.TryGetValue(type, out method)) return method;
                if (type.IsArray)
                {
                    Type elementType = type.GetElementType();
                    if (elementType.IsValueType)
                    {
                        if (elementType.IsEnum)
                        {
                            Type enumType = System.Enum.GetUnderlyingType(elementType);
                            if (enumType == typeof(uint)) method = enumUIntArrayMemberMethod;
                            else if (enumType == typeof(byte)) method = enumByteArrayMemberMethod;
                            else if (enumType == typeof(ulong)) method = enumULongArrayMemberMethod;
                            else if (enumType == typeof(ushort)) method = enumUShortArrayMemberMethod;
                            else if (enumType == typeof(long)) method = enumLongArrayMemberMethod;
                            else if (enumType == typeof(short)) method = enumShortArrayMemberMethod;
                            else if (enumType == typeof(sbyte)) method = enumSByteArrayMemberMethod;
                            else method = enumIntArrayMemberMethod;
                            method = method.MakeGenericMethod(elementType);
                        }
                        else if (elementType.IsGenericType && elementType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            method = nullableArrayMemberMethod.MakeGenericMethod(elementType.GetGenericArguments());
                        }
                        else method = structArrayMemberMethod.MakeGenericMethod(elementType);
                    }
                    else method = arrayMemberMethod.MakeGenericMethod(elementType);
                }
                else if (type.IsEnum)
                {
                    Type enumType = System.Enum.GetUnderlyingType(type);
                    if (enumType == typeof(uint)) method = enumUIntMemberMethod;
                    else if (enumType == typeof(byte)) method = enumByteMemberMethod;
                    else if (enumType == typeof(ulong)) method = enumULongMemberMethod;
                    else if (enumType == typeof(ushort)) method = enumUShortMemberMethod;
                    else if (enumType == typeof(long)) method = enumLongMemberMethod;
                    else if (enumType == typeof(short)) method = enumShortMemberMethod;
                    else if (enumType == typeof(sbyte)) method = enumSByteMemberMethod;
                    else method = enumIntMemberMethod;
                    method = method.MakeGenericMethod(type);
                }
                else
                {
                    if (type.IsGenericType)
                    {
                        Type genericType = type.GetGenericTypeDefinition();
                        if (genericType == typeof(Dictionary<,>) || genericType == typeof(SortedDictionary<,>) || genericType == typeof(SortedList<,>))
                        {
                            Type[] parameterTypes = type.GetGenericArguments();
                            method = dictionaryMemberMethod.MakeGenericMethod(type, parameterTypes[0], parameterTypes[1]);
                        }
                        else if (genericType == typeof(Nullable<>))
                        {
                            method = nullableMemberSerializeMethod.MakeGenericMethod(type.GetGenericArguments());
                        }
                        else if (genericType == typeof(KeyValuePair<,>))
                        {
                            method = keyValuePairSerializeMethod.MakeGenericMethod(type.GetGenericArguments());
                        }
                    }
                    if (method == null)
                    {
                        if (typeof(fastCSharp.code.cSharp.dataSerialize.ISerialize).IsAssignableFrom(type))
                        {
                            if (type.IsValueType) method = structISerializeMethod.MakeGenericMethod(type);
                            else method = memberClassISerializeMethod.MakeGenericMethod(type);
                        }
                        else if (type.IsValueType) method = structSerializeMethod.MakeGenericMethod(type);
                        else method = memberClassSerializeMethod.MakeGenericMethod(type);
                    }
                }
                memberSerializers.Set(type, method);
                return method;
            }

            /// <summary>
            /// 真实类型序列化函数集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, Action<dataSerializer, object>> realSerializers = new interlocked.dictionary<Type, Action<dataSerializer, object>>();
            /// <summary>
            /// 获取真实类型序列化函数
            /// </summary>
            /// <param name="type">数组类型</param>
            /// <returns>真实类型序列化函数</returns>
            public static Action<dataSerializer, object> GetRealSerializer(Type type)
            {
                Action<dataSerializer, object> method;
                if (realSerializers.TryGetValue(type, out method)) return method;
                method = (Action<dataSerializer, object>)Delegate.CreateDelegate(typeof(Action<dataSerializer, object>), realTypeObjectMethod.MakeGenericMethod(type));
                realSerializers.Set(type, method);
                return method;
            }

            /// <summary>
            /// 是否支持循环引用处理集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, bool> isReferenceMembers = new interlocked.dictionary<Type, bool>();
            /// <summary>
            /// 是否支持循环引用处理
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            public static bool IsReferenceMember(Type type)
            {
                bool isReferenceMember;
                if (isReferenceMembers.TryGetValue(type, out isReferenceMember)) return isReferenceMember;
                isReferenceMembers.Set(type, isReferenceMember = (bool)isReferenceMemberMethod.MakeGenericMethod(type).Invoke(null, null));
                return isReferenceMember;
            }
            /// <summary>
            /// 是否支持循环引用处理
            /// </summary>
            /// <typeparam name="valueType"></typeparam>
            /// <returns></returns>
            private static bool isReferenceMember<valueType>()
            {
                return typeSerializer<valueType>.IsReferenceMember;
            }
            /// <summary>
            /// 是否支持循环引用处理函数信息
            /// </summary>
            private static readonly MethodInfo isReferenceMemberMethod = typeof(staticTypeSerializer).GetMethod("isReferenceMember", BindingFlags.Static | BindingFlags.NonPublic);
        }
        /// <summary>
        /// 二进制数据序列化
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        internal static class typeSerializer<valueType>
        {
            /// <summary>
            /// 二进制数据序列化类型配置
            /// </summary>
            private static readonly dataSerialize attribute;
            /// <summary>
            /// 序列化委托
            /// </summary>
            internal static readonly Action<dataSerializer, valueType> DefaultSerializer;
            /// <summary>
            /// 固定分组成员序列化
            /// </summary>
            private static readonly Action<dataSerializer, valueType> fixedMemberSerializer;
            /// <summary>
            /// 固定分组成员位图序列化
            /// </summary>
            private static readonly Action<memberMap, dataSerializer, valueType> fixedMemberMapSerializer;
            /// <summary>
            /// 成员序列化
            /// </summary>
            private static readonly Action<dataSerializer, valueType> memberSerializer;
            /// <summary>
            /// 成员位图序列化
            /// </summary>
            private static readonly Action<memberMap, dataSerializer, valueType> memberMapSerializer;
            /// <summary>
            /// JSON混合序列化位图
            /// </summary>
            private static readonly memberMap jsonMemberMap;
            /// <summary>
            /// JSON混合序列化成员索引集合
            /// </summary>
            private static readonly int[] jsonMemberIndexs;
            /// <summary>
            /// 序列化成员数量
            /// </summary>
            private static readonly int memberCountVerify;
            /// <summary>
            /// 固定分组字节数
            /// </summary>
            private static readonly int fixedSize;
            /// <summary>
            /// 固定分组填充字节数
            /// </summary>
            private static readonly int fixedFillSize;
            /// <summary>
            /// 是否值类型
            /// </summary>
            private static readonly bool isValueType;
            /// <summary>
            /// 是否支持循环引用处理
            /// </summary>
            internal static readonly bool IsReferenceMember;
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="serializer">二进制数据序列化</param>
            /// <param name="value">数据对象</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal static void Serialize(dataSerializer serializer, valueType value)
            {
                if (isValueType) StructSerialize(serializer, value);
                else ClassSerialize(serializer, value);
            }
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="serializer">二进制数据序列化</param>
            /// <param name="value">数据对象</param>
            internal static void ClassSerialize(dataSerializer serializer, valueType value)
            {
                if (DefaultSerializer == null)
                {
                    if (serializer.CheckPoint(value))
                    {
                        if (serializer.Config.IsRealType)
                        {
                            Type type = value.GetType();
                            if (type != typeof(valueType))
                            {
                                if (serializer.CheckPoint(value))
                                {
                                    serializer.Stream.Write(fastCSharp.emit.dataSerializer.RealTypeValue);
                                    staticTypeSerializer.GetRealSerializer(type)(serializer, value);
                                }
                                return;
                            }
                        }
                        if (constructor<valueType>.New == null) serializer.Stream.Write(fastCSharp.emit.binarySerializer.NullValue);
                        else MemberSerialize(serializer, value);
                    }
                }
                else DefaultSerializer(serializer, value);
            }
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="serializer">二进制数据序列化</param>
            /// <param name="value">数据对象</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal static void StructSerialize(dataSerializer serializer, valueType value)
            {
                if (DefaultSerializer == null) MemberSerialize(serializer, value);
                else DefaultSerializer(serializer, value);
            }
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="serializer">二进制数据序列化</param>
            /// <param name="value">数据对象</param>
            internal static void MemberSerialize(dataSerializer serializer, valueType value)
            {
                memberMap memberMap = attribute.IsMemberMap ? serializer.SerializeMemberMap<valueType>() : null;
                unmanagedStream stream = serializer.Stream;
                if (memberMap == null)
                {
                    stream.PrepLength(fixedSize);
                    stream.UnsafeWrite(memberCountVerify);
                    fixedMemberSerializer(serializer, value);
                    stream.UnsafeAddLength(fixedFillSize);
                    stream.PrepLength();
                    memberSerializer(serializer, value);
                    if (jsonMemberMap == null)
                    {
                        if (attribute.IsJson) stream.Write(0);
                    }
                    else
                    {
                        pointer buffer = fastCSharp.unmanagedPool.StreamBuffers.Get();
                        try
                        {
                            using (charStream jsonStream = serializer.ResetJsonStream(buffer.Data, fastCSharp.unmanagedPool.StreamBuffers.Size))
                            {
                                jsonSerializer.Serialize(value, jsonStream, stream, serializer.getJsonConfig(jsonMemberMap));
                            }
                        }
                        finally { fastCSharp.unmanagedPool.StreamBuffers.Push(ref buffer); }
                    }
                }
                else
                {
                    stream.PrepLength(fixedSize - sizeof(int));
                    int length = stream.OffsetLength;
                    fixedMemberMapSerializer(memberMap, serializer, value);
                    stream.UnsafeAddLength((length - stream.OffsetLength) & 3);
                    stream.PrepLength();
                    memberMapSerializer(memberMap, serializer, value);
                    if (jsonMemberMap == null || (memberMap = serializer.getJsonMemberMap<valueType>(memberMap, jsonMemberIndexs)) == null)
                    {
                        if (attribute.IsJson) stream.Write(0);
                    }
                    else
                    {
                        pointer buffer = fastCSharp.unmanagedPool.StreamBuffers.Get();
                        try
                        {
                            using (charStream jsonStream = serializer.ResetJsonStream(buffer.Data, fastCSharp.unmanagedPool.StreamBuffers.Size))
                            {
                                jsonSerializer.Serialize(value, jsonStream, stream, serializer.getJsonConfig(memberMap));
                            }
                        }
                        finally { fastCSharp.unmanagedPool.StreamBuffers.Push(ref buffer); }
                    }
                }
            }
            /// <summary>
            /// 真实类型序列化
            /// </summary>
            /// <param name="serializer"></param>
            /// <param name="value"></param>
            internal static void RealTypeObject(dataSerializer serializer, object value)
            {
                if (isValueType)
                {
                    typeSerializer<remoteType>.StructSerialize(serializer, typeof(valueType));
                    StructSerialize(serializer, (valueType)value);
                }
                else
                {
                    if (constructor<valueType>.New == null) serializer.Stream.Write(fastCSharp.emit.binarySerializer.NullValue);
                    else
                    {
                        typeSerializer<remoteType>.StructSerialize(serializer, typeof(valueType));
                        if (DefaultSerializer == null)
                        {
                            if (serializer.CheckPoint(value)) MemberSerialize(serializer, (valueType)value);
                        }
                        else DefaultSerializer(serializer, (valueType)value);
                    }
                }
            }
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="serializer">二进制数据序列化</param>
            /// <param name="value">数据对象</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal static void BaseSerialize<childType>(dataSerializer serializer, childType value) where childType : valueType
            {
                if (serializer.CheckPoint(value)) StructSerialize(serializer, value);
            }
            /// <summary>
            /// 找不到构造函数
            /// </summary>
            /// <param name="serializer">二进制数据序列化</param>
            /// <param name="value">数据对象</param>
            private static void noConstructor(dataSerializer serializer, valueType value)
            {
                if (serializer.CheckPoint(value))
                {
                    if (serializer.Config.IsRealType) serializer.Stream.Write(fastCSharp.emit.binarySerializer.NullValue);
                    else
                    {
                        Type type = value.GetType();
                        if (type == typeof(valueType)) serializer.Stream.Write(fastCSharp.emit.binarySerializer.NullValue);
                        else staticTypeSerializer.GetRealSerializer(type)(serializer, value);
                    }
                }
            }
            /// <summary>
            /// 不支持对象转换null
            /// </summary>
            /// <param name="serializer">二进制数据序列化</param>
            /// <param name="value">数据对象</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private static void toNull(dataSerializer serializer, valueType value)
            {
                serializer.Stream.Write(fastCSharp.emit.binarySerializer.NullValue);
            }
            /// <summary>
            /// 枚举值序列化
            /// </summary>
            /// <param name="serializer">二进制数据序列化</param>
            /// <param name="value">枚举值序列化</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe static void enumByte(dataSerializer serializer, valueType value)
            {
                serializer.Stream.Write((uint)pub.enumCast<valueType, byte>.ToInt(value));
            }
            /// <summary>
            /// 枚举值序列化
            /// </summary>
            /// <param name="serializer">二进制数据序列化</param>
            /// <param name="value">枚举值序列化</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe static void enumSByte(dataSerializer serializer, valueType value)
            {
                serializer.Stream.Write((int)pub.enumCast<valueType, sbyte>.ToInt(value));
            }
            /// <summary>
            /// 枚举值序列化
            /// </summary>
            /// <param name="serializer">二进制数据序列化</param>
            /// <param name="value">枚举值序列化</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe static void enumShort(dataSerializer serializer, valueType value)
            {
                serializer.Stream.Write((int)pub.enumCast<valueType, short>.ToInt(value));
            }
            /// <summary>
            /// 枚举值序列化
            /// </summary>
            /// <param name="serializer">二进制数据序列化</param>
            /// <param name="value">枚举值序列化</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe static void enumUShort(dataSerializer serializer, valueType value)
            {
                serializer.Stream.Write((uint)pub.enumCast<valueType, ushort>.ToInt(value));
            }
            /// <summary>
            /// 枚举值序列化
            /// </summary>
            /// <param name="serializer">二进制数据序列化</param>
            /// <param name="value">枚举值序列化</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe static void enumInt(dataSerializer serializer, valueType value)
            {
                serializer.Stream.Write(pub.enumCast<valueType, int>.ToInt(value));
            }
            /// <summary>
            /// 枚举值序列化
            /// </summary>
            /// <param name="serializer">二进制数据序列化</param>
            /// <param name="value">枚举值序列化</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe static void enumUInt(dataSerializer serializer, valueType value)
            {
                serializer.Stream.Write(pub.enumCast<valueType, uint>.ToInt(value));
            }
            /// <summary>
            /// 枚举值序列化
            /// </summary>
            /// <param name="serializer">二进制数据序列化</param>
            /// <param name="value">枚举值序列化</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe static void enumLong(dataSerializer serializer, valueType value)
            {
                serializer.Stream.Write(pub.enumCast<valueType, long>.ToInt(value));
            }
            /// <summary>
            /// 枚举值序列化
            /// </summary>
            /// <param name="serializer">二进制数据序列化</param>
            /// <param name="value">枚举值序列化</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe static void enumULong(dataSerializer serializer, valueType value)
            {
                serializer.Stream.Write(pub.enumCast<valueType, ulong>.ToInt(value));
            }
            /// <summary>
            /// 获取字段成员集合
            /// </summary>
            /// <returns>字段成员集合</returns>
            public static subArray<memberIndex> GetMembers()
            {
                if (fixedMemberSerializer == null) return default(subArray<memberIndex>);
                return staticTypeSerializer.GetMembers(memberIndexGroup<valueType>.GetFields(attribute.MemberFilter));
            }
            static typeSerializer()
            {
                Type type = typeof(valueType), attributeType;
                MethodInfo methodInfo = dataSerializer.getSerializeMethod(type);
                attribute = type.customAttribute<dataSerialize>(out attributeType, true) ?? dataSerialize.Default;
                if (methodInfo != null)
                {
#if NOJIT
                    DefaultSerializer = new methodSerializer(methodInfo).Serialize;
#else
                    DynamicMethod dynamicMethod = new DynamicMethod("dataSerializer", typeof(void), new Type[] { typeof(dataSerializer), type }, true);
                    dynamicMethod.InitLocals = true;
                    ILGenerator generator = dynamicMethod.GetILGenerator();
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Call, methodInfo);
                    generator.Emit(OpCodes.Ret);
                    DefaultSerializer = (Action<dataSerializer, valueType>)dynamicMethod.CreateDelegate(typeof(Action<dataSerializer, valueType>));
#endif
                    isValueType = true;
                    IsReferenceMember = false;
                    return;
                }
                if (type.IsArray)
                {
                    isValueType = true;
                    if (type.GetArrayRank() == 1)
                    {
                        Type elementType = type.GetElementType();
                        if (!elementType.IsPointer)
                        {
                            if (elementType.IsValueType)
                            {
                                if (elementType.IsEnum)
                                {
                                    Type enumType = System.Enum.GetUnderlyingType(elementType);
                                    if (enumType == typeof(uint)) methodInfo = enumUIntArrayMethod;
                                    else if (enumType == typeof(byte)) methodInfo = enumByteArrayMethod;
                                    else if (enumType == typeof(ulong)) methodInfo = enumULongArrayMethod;
                                    else if (enumType == typeof(ushort)) methodInfo = enumUShortArrayMethod;
                                    else if (enumType == typeof(long)) methodInfo = enumLongArrayMethod;
                                    else if (enumType == typeof(short)) methodInfo = enumShortArrayMethod;
                                    else if (enumType == typeof(sbyte)) methodInfo = enumSByteArrayMethod;
                                    else methodInfo = enumIntArrayMethod;
                                    methodInfo = methodInfo.MakeGenericMethod(elementType);
                                    IsReferenceMember = false;
                                }
                                else if (elementType.IsGenericType && elementType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                {
                                    methodInfo = nullableArrayMethod.MakeGenericMethod(elementType = elementType.GetGenericArguments()[0]);
                                    IsReferenceMember = staticTypeSerializer.IsReferenceMember(elementType);
                                }
                                else
                                {
                                    methodInfo = structArrayMethod.MakeGenericMethod(elementType);
                                    IsReferenceMember = staticTypeSerializer.IsReferenceMember(elementType);
                                }
                            }
                            else
                            {
                                methodInfo = arrayMethod.MakeGenericMethod(elementType);
                                IsReferenceMember = staticTypeSerializer.IsReferenceMember(elementType);
                            }
                            DefaultSerializer = (Action<dataSerializer, valueType>)Delegate.CreateDelegate(typeof(Action<dataSerializer, valueType>), methodInfo);
                            return;
                        }
                    }
                    DefaultSerializer = toNull;
                    IsReferenceMember = false;
                    return;
                }
                if (type.IsEnum)
                {
                    Type enumType = System.Enum.GetUnderlyingType(type);
                    if (enumType == typeof(uint)) DefaultSerializer = enumUInt;
                    else if (enumType == typeof(byte)) DefaultSerializer = enumByte;
                    else if (enumType == typeof(ulong)) DefaultSerializer = enumULong;
                    else if (enumType == typeof(ushort)) DefaultSerializer = enumUShort;
                    else if (enumType == typeof(long)) DefaultSerializer = enumLong;
                    else if (enumType == typeof(short)) DefaultSerializer = enumShort;
                    else if (enumType == typeof(sbyte)) DefaultSerializer = enumSByte;
                    else DefaultSerializer = enumInt;
                    isValueType = true;
                    IsReferenceMember = false;
                    return;
                }
                if (type.IsPointer)
                {
                    DefaultSerializer = toNull;
                    IsReferenceMember = false;
                    isValueType = true;
                    return;
                }

                if (type.IsGenericType)
                {
                    Type genericType = type.GetGenericTypeDefinition();
                    Type[] parameterTypes = type.GetGenericArguments();
                    if (genericType == typeof(subArray<>))
                    {
                        DefaultSerializer = (Action<dataSerializer, valueType>)Delegate.CreateDelegate(typeof(Action<dataSerializer, valueType>), subArraySerializeMethod.MakeGenericMethod(parameterTypes));
                        IsReferenceMember = staticTypeSerializer.IsReferenceMember(parameterTypes[0]);
                        isValueType = true;
                        return;
                    }
                    if (genericType == typeof(Dictionary<,>) || genericType == typeof(SortedDictionary<,>) || genericType == typeof(SortedList<,>))
                    {
                        DefaultSerializer = (Action<dataSerializer, valueType>)Delegate.CreateDelegate(typeof(Action<dataSerializer, valueType>), dictionarySerializeMethod.MakeGenericMethod(type, parameterTypes[0], parameterTypes[1]));
                        IsReferenceMember = staticTypeSerializer.IsReferenceMember(parameterTypes[0]) || staticTypeSerializer.IsReferenceMember(parameterTypes[1]);
                        isValueType = true;
                        return;
                    }
                    if (genericType == typeof(Nullable<>))
                    {
                        DefaultSerializer = (Action<dataSerializer, valueType>)Delegate.CreateDelegate(typeof(Action<dataSerializer, valueType>), nullableSerializeMethod.MakeGenericMethod(parameterTypes));
                        IsReferenceMember = staticTypeSerializer.IsReferenceMember(parameterTypes[0]);
                        isValueType = true;
                        return;
                    }
                    if (genericType == typeof(KeyValuePair<,>))
                    {
                        DefaultSerializer = (Action<dataSerializer, valueType>)Delegate.CreateDelegate(typeof(Action<dataSerializer, valueType>), keyValuePairSerializeMethod.MakeGenericMethod(parameterTypes));
                        IsReferenceMember = staticTypeSerializer.IsReferenceMember(parameterTypes[0]) || staticTypeSerializer.IsReferenceMember(parameterTypes[1]);
                        isValueType = true;
                        return;
                    }
                }
                if ((methodInfo = staticTypeSerializer.GetCustom(type, true)) != null)
                {
                    DefaultSerializer = (Action<dataSerializer, valueType>)Delegate.CreateDelegate(typeof(Action<dataSerializer, valueType>), methodInfo);
                    IsReferenceMember = attribute.IsReferenceMember;
                    isValueType = true;
                    return;
                }
                if (type.IsAbstract || type.IsInterface || constructor<valueType>.New == null)
                {
                    DefaultSerializer = noConstructor;
                    isValueType = IsReferenceMember = true;
                    return;
                }
                ConstructorInfo constructorInfo = null;
                Type argumentType = null;
                IsReferenceMember = attribute.IsReferenceMember;
                foreach (Type interfaceType in type.GetInterfaces())
                {
                    if (interfaceType.IsGenericType)
                    {
                        Type genericType = interfaceType.GetGenericTypeDefinition();
                        if (genericType == typeof(ICollection<>))
                        {
                            Type[] parameterTypes = interfaceType.GetGenericArguments();
                            argumentType = parameterTypes[0];
                            parameterTypes[0] = argumentType.MakeArrayType();
                            constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, parameterTypes, null);
                            if (constructorInfo != null) break;
                            parameterTypes[0] = typeof(IList<>).MakeGenericType(argumentType);
                            constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, parameterTypes, null);
                            if (constructorInfo != null) break;
                            parameterTypes[0] = typeof(ICollection<>).MakeGenericType(argumentType);
                            constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, parameterTypes, null);
                            if (constructorInfo != null) break;
                            parameterTypes[0] = typeof(IEnumerable<>).MakeGenericType(argumentType);
                            constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, parameterTypes, null);
                            if (constructorInfo != null) break;
                        }
                        else if (genericType == typeof(IDictionary<,>))
                        {
                            constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { interfaceType }, null);
                            if (constructorInfo != null)
                            {
                                Type[] parameters = interfaceType.GetGenericArguments();
                                methodInfo = (type.IsValueType ? structDictionaryMethod : classDictionaryMethod).MakeGenericMethod(type, parameters[0], parameters[1]);
                                DefaultSerializer = (Action<dataSerializer, valueType>)Delegate.CreateDelegate(typeof(Action<dataSerializer, valueType>), methodInfo);
                                return;
                            }
                        }
                    }
                }
                if (constructorInfo != null)
                {
                    if (argumentType.IsValueType && argumentType.IsEnum)
                    {
                        Type enumType = System.Enum.GetUnderlyingType(argumentType);
                        if (enumType == typeof(uint)) methodInfo = type.IsValueType ? structEnumUIntCollectionMethod : classEnumUIntCollectionMethod;
                        else if (enumType == typeof(byte)) methodInfo = type.IsValueType ? structEnumByteCollectionMethod : classEnumByteCollectionMethod;
                        else if (enumType == typeof(ulong)) methodInfo = type.IsValueType ? structEnumULongCollectionMethod : classEnumULongCollectionMethod;
                        else if (enumType == typeof(ushort)) methodInfo = type.IsValueType ? structEnumUShortCollectionMethod : classEnumUShortCollectionMethod;
                        else if (enumType == typeof(long)) methodInfo = type.IsValueType ? structEnumLongCollectionMethod : classEnumLongCollectionMethod;
                        else if (enumType == typeof(short)) methodInfo = type.IsValueType ? structEnumShortCollectionMethod : classEnumShortCollectionMethod;
                        else if (enumType == typeof(sbyte)) methodInfo = type.IsValueType ? structEnumSByteCollectionMethod : classEnumSByteCollectionMethod;
                        else methodInfo = type.IsValueType ? structEnumIntCollectionMethod : classEnumIntCollectionMethod;
                        methodInfo = methodInfo.MakeGenericMethod(argumentType, type);
                    }
                    else methodInfo = (type.IsValueType ? structCollectionMethod : classCollectionMethod).MakeGenericMethod(argumentType, type);
                    DefaultSerializer = (Action<dataSerializer, valueType>)Delegate.CreateDelegate(typeof(Action<dataSerializer, valueType>), methodInfo);
                    return;
                }
                if (typeof(fastCSharp.code.cSharp.dataSerialize.ISerialize).IsAssignableFrom(type))
                {
                    methodInfo = type.IsValueType ? dataSerializer.structISerializeMethod : dataSerializer.classISerializeMethod;
                    DefaultSerializer = (Action<dataSerializer, valueType>)Delegate.CreateDelegate(typeof(Action<dataSerializer, valueType>), methodInfo.MakeGenericMethod(type));
                    isValueType = true;
                }
                else
                {
                    if (type.IsValueType) isValueType = true;
                    else if (attribute != dataSerialize.Default && attributeType != type)
                    {
                        for (Type baseType = type.BaseType; baseType != typeof(object); baseType = baseType.BaseType)
                        {
                            dataSerialize baseAttribute = fastCSharp.code.typeAttribute.GetAttribute<dataSerialize>(baseType, false, true);
                            if (baseAttribute != null)
                            {
                                if (baseAttribute.IsBaseType)
                                {
                                    methodInfo = baseSerializeMethod.MakeGenericMethod(baseType, type);
                                    DefaultSerializer = (Action<dataSerializer, valueType>)Delegate.CreateDelegate(typeof(Action<dataSerializer, valueType>), methodInfo);
                                    return;
                                }
                                break;
                            }
                        }
                    }
                    fields<fieldInfo> fields = staticTypeSerializer.GetFields(memberIndexGroup<valueType>.GetFields(attribute.MemberFilter), out memberCountVerify);
                    fixedFillSize = -fields.FixedSize & 3;
                    fixedSize = (fields.FixedSize + (sizeof(int) + 3)) & (int.MaxValue - 3);
#if NOJIT
                    fixedMemberSerializer = new serializer(ref fields.FixedFields).Serialize;
                    if (attribute.IsMemberMap) fixedMemberMapSerializer = new mapSerializer(ref fields.FixedFields).Serialize;
                    memberSerializer = new serializer(ref fields.Fields).Serialize;
                    if (attribute.IsMemberMap) memberMapSerializer = new mapSerializer(ref fields.Fields).Serialize;
#else
                    staticTypeSerializer.memberDynamicMethod fixedDynamicMethod = new staticTypeSerializer.memberDynamicMethod(type);
                    staticTypeSerializer.memberMapDynamicMethod fixedMemberMapDynamicMethod = attribute.IsMemberMap ? new staticTypeSerializer.memberMapDynamicMethod(type) : default(staticTypeSerializer.memberMapDynamicMethod);
                    foreach (fieldInfo member in fields.FixedFields)
                    {
                        fixedDynamicMethod.Push(member);
                        if (attribute.IsMemberMap) fixedMemberMapDynamicMethod.Push(member);
                    }
                    fixedMemberSerializer = (Action<dataSerializer, valueType>)fixedDynamicMethod.Create<Action<dataSerializer, valueType>>();
                    if (attribute.IsMemberMap) fixedMemberMapSerializer = (Action<memberMap, dataSerializer, valueType>)fixedMemberMapDynamicMethod.Create<Action<memberMap, dataSerializer, valueType>>();

                    staticTypeSerializer.memberDynamicMethod dynamicMethod = new staticTypeSerializer.memberDynamicMethod(type);
                    staticTypeSerializer.memberMapDynamicMethod memberMapDynamicMethod = attribute.IsMemberMap ? new staticTypeSerializer.memberMapDynamicMethod(type) : default(staticTypeSerializer.memberMapDynamicMethod);
                    foreach (fieldInfo member in fields.Fields)
                    {
                        dynamicMethod.Push(member);
                        if (attribute.IsMemberMap) memberMapDynamicMethod.Push(member);
                    }
                    memberSerializer = (Action<dataSerializer, valueType>)dynamicMethod.Create<Action<dataSerializer, valueType>>();
                    if (attribute.IsMemberMap) memberMapSerializer = (Action<memberMap, dataSerializer, valueType>)memberMapDynamicMethod.Create<Action<memberMap, dataSerializer, valueType>>();
#endif
                    if (fields.JsonFields.length != 0)
                    {
                        jsonMemberMap = memberMap<valueType>.New();
                        jsonMemberIndexs = new int[fields.JsonFields.length];
                        int index = 0;
                        foreach (fieldIndex field in fields.JsonFields) jsonMemberMap.SetMember(jsonMemberIndexs[index++] = field.MemberIndex);
                    }
                }
            }
#if NOJIT
            /// <summary>
            /// 序列化（反射模式）
            /// </summary>
            private sealed class methodSerializer
            {
                /// <summary>
                /// 序列化函数信息
                /// </summary>
                private MethodInfo method;
                /// <summary>
                /// 序列化
                /// </summary>
                /// <param name="method"></param>
                public methodSerializer(MethodInfo method)
                {
                    this.method = method;
                }
                /// <summary>
                /// 序列化
                /// </summary>
                /// <param name="serializer"></param>
                /// <param name="value"></param>
                public void Serialize(dataSerializer serializer, valueType value)
                {
                    method.Invoke(serializer, new object[] { value });
                }
            }
            /// <summary>
            /// 序列化（反射模式）
            /// </summary>
            private sealed class serializer
            {
                /// <summary>
                /// 字段信息
                /// </summary>
                private struct field
                {
                    /// <summary>
                    /// 字段信息
                    /// </summary>
                    public FieldInfo Field;
                    /// <summary>
                    /// 序列化函数信息
                    /// </summary>
                    public MethodInfo SerializeMethod;
                    /// <summary>
                    /// 设置字段信息
                    /// </summary>
                    /// <param name="field"></param>
                    public void Set(fieldInfo field)
                    {
                        Field = field.Field;
                        SerializeMethod = getMemberSerializeMethod(Field.FieldType) ?? staticTypeSerializer.GetMemberSerializer(Field.FieldType);
                    }
                }
                /// <summary>
                /// 字段集合
                /// </summary>
                private field[] fields;
                /// <summary>
                /// 序列化
                /// </summary>
                /// <param name="fields"></param>
                public serializer(ref subArray<fieldInfo> fields)
                {
                    this.fields = new field[fields.length];
                    int index = 0;
                    foreach (fieldInfo field in fields) this.fields[index++].Set(field);
                }
                /// <summary>
                /// 序列化
                /// </summary>
                /// <param name="serializer"></param>
                /// <param name="value"></param>
                public void Serialize(dataSerializer serializer, valueType value)
                {
                    object[] parameters = new object[1];
                    object objectValue = value;
                    foreach (field field in fields)
                    {
                        parameters[0] = field.Field.GetValue(objectValue);
                        field.SerializeMethod.Invoke(serializer, parameters);
                    }
                }
            }
            /// <summary>
            /// 序列化（反射模式）
            /// </summary>
            private sealed class mapSerializer
            {
                /// <summary>
                /// 字段信息
                /// </summary>
                private struct field
                {
                    /// <summary>
                    /// 字段信息
                    /// </summary>
                    public FieldInfo Field;
                    /// <summary>
                    /// 序列化函数信息
                    /// </summary>
                    public MethodInfo SerializeMethod;
                    /// <summary>
                    /// 成员编号
                    /// </summary>
                    public int MemberIndex;
                    /// <summary>
                    /// 设置字段信息
                    /// </summary>
                    /// <param name="field"></param>
                    public void Set(fieldInfo field)
                    {
                        Field = field.Field;
                        MemberIndex = field.MemberIndex;
                        SerializeMethod = getMemberMapSerializeMethod(Field.FieldType) ?? staticTypeSerializer.GetMemberSerializer(Field.FieldType);
                    }
                }
                /// <summary>
                /// 字段集合
                /// </summary>
                private field[] fields;
                /// <summary>
                /// 序列化
                /// </summary>
                /// <param name="fields"></param>
                public mapSerializer(ref subArray<fieldInfo> fields)
                {
                    this.fields = new field[fields.length];
                    int index = 0;
                    foreach (fieldInfo field in fields) this.fields[index++].Set(field);
                }
                /// <summary>
                /// 序列化
                /// </summary>
                /// <param name="memberMap"></param>
                /// <param name="serializer"></param>
                /// <param name="value"></param>
                public void Serialize(memberMap memberMap, dataSerializer serializer, valueType value)
                {
                    object[] parameters = null;
                    object objectValue = null;
                    foreach (field field in fields)
                    {
                        if (memberMap.IsMember(field.MemberIndex))
                        {
                            if (parameters == null)
                            {
                                parameters = new object[1];
                                objectValue = value;
                            }
                            parameters[0] = field.Field.GetValue(objectValue);
                            field.SerializeMethod.Invoke(serializer, parameters);
                        }
                    }
                }
            }
#endif
        }
        /// <summary>
        /// 历史对象指针位置
        /// </summary>
        private Dictionary<objectReference, int> points;
        /// <summary>
        /// 序列化配置参数
        /// </summary>
        internal config Config;
        /// <summary>
        /// 是否支持循环引用处理
        /// </summary>
        private bool isReferenceMember;
        /// <summary>
        /// 是否检测数组引用
        /// </summary>
        private bool isReferenceArray;
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="value">数据对象</param>
        /// <param name="config">配置参数</param>
        /// <returns>序列化数据</returns>
        private byte[] serialize<valueType>(valueType value, config config)
        {
            binarySerializerConfig = Config = config;
            pointer buffer = fastCSharp.unmanagedPool.StreamBuffers.Get();
            try
            {
                Stream.UnsafeReset(buffer.Byte, fastCSharp.unmanagedPool.StreamBuffers.Size);
                using (Stream)
                {
                    serialize(value);
                    return Stream.GetArray();
                }
            }
            finally { fastCSharp.unmanagedPool.StreamBuffers.Push(ref buffer); }
        }
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="value">数据对象</param>
        /// <param name="stream">序列化输出缓冲区</param>
        /// <param name="config">配置参数</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize<valueType>(valueType value, unmanagedStream stream, config config)
        {
            binarySerializerConfig = Config = config;
            this.Stream.From(stream);
            try
            {
                serialize(value);
            }
            finally { stream.From(this.Stream); }
        }
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="value">数据对象</param>
        private void serialize<valueType>(valueType value)
        {
            isReferenceMember = typeSerializer<valueType>.IsReferenceMember;
            if (points == null && isReferenceMember) points = dictionary<objectReference>.Create<int>();
            isReferenceArray = true;
            MemberMap = Config.MemberMap;
            streamStartIndex = Stream.OffsetLength;
            Stream.Write(Config.HeaderValue);
            typeSerializer<valueType>.Serialize(this, value);
            Stream.Write(Stream.OffsetLength - streamStartIndex);
        }
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="value">数据对象</param>
        /// <param name="config">配置参数</param>
        /// <returns>序列化数据</returns>
        private byte[] codeSerialize<valueType>(valueType value, config config) where valueType : fastCSharp.code.cSharp.dataSerialize.ISerialize
        {
            binarySerializerConfig = Config = config;
            pointer buffer = fastCSharp.unmanagedPool.StreamBuffers.Get();
            try
            {
                Stream.UnsafeReset(buffer.Byte, fastCSharp.unmanagedPool.StreamBuffers.Size);
                using (Stream)
                {
                    codeSerialize(value);
                    return Stream.GetArray();
                }
            }
            finally { fastCSharp.unmanagedPool.StreamBuffers.Push(ref buffer); }
        }
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="value">数据对象</param>
        /// <param name="stream">序列化输出缓冲区</param>
        /// <param name="config">配置参数</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void codeSerialize<valueType>(valueType value, unmanagedStream stream, config config) where valueType : fastCSharp.code.cSharp.dataSerialize.ISerialize
        {
            binarySerializerConfig = Config = config;
            this.Stream.From(stream);
            try
            {
                codeSerialize(value);
            }
            finally { stream.From(this.Stream); }
        }
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="value">数据对象</param>
        private void codeSerialize<valueType>(valueType value) where valueType : fastCSharp.code.cSharp.dataSerialize.ISerialize
        {
            isReferenceMember = typeSerializer<valueType>.IsReferenceMember;
            if (points == null && isReferenceMember) points = dictionary<objectReference>.Create<int>();
            isReferenceArray = true;
            MemberMap = Config.MemberMap;
            streamStartIndex = Stream.OffsetLength;
            Stream.Write(Config.HeaderValue);
            value.Serialize(this);
            Stream.Write(Stream.OffsetLength - streamStartIndex);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private new void free()
        {
            base.free();
            if (points != null) points.Clear();
            typePool<dataSerializer>.PushNotNull(this);
        }
        /// <summary>
        /// 添加历史对象
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool CheckPoint<valueType>(valueType value)
        {
            if (isReferenceMember)
            {
                int point;
                if (points.TryGetValue(new fastCSharp.objectReference { Value = value }, out point))
                {
                    Stream.Write(-point);
                    return false;
                }
                points[new fastCSharp.objectReference { Value = value }] = Stream.OffsetLength - streamStartIndex;
            }
            return true;
        }
        /// <summary>
        /// 添加历史对象
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool checkPoint<valueType>(valueType[] value)
        {
            if (value.Length == 0)
            {
                Stream.Write(0);
                isReferenceArray = true;
                return false;
            }
            if (isReferenceArray) return CheckPoint<valueType[]>(value);
            return isReferenceArray = true;
        }
        /// <summary>
        /// 判断成员索引是否有效
        /// </summary>
        /// <param name="memberIndex"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool IsMemberMap(int memberIndex)
        {
            return CurrentMemberMap.IsMember(memberIndex);
        }
        /// <summary>
        /// 逻辑值序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(bool[] value)
        {
            if (checkPoint(value)) Serialize(Stream, value);
        }
        /// <summary>
        /// 逻辑值序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(bool[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serialize(value);
        }
        /// <summary>
        /// 逻辑值序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(bool?[] value)
        {
            if (checkPoint(value)) Serialize(Stream, value);
        }
        /// <summary>
        /// 逻辑值序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(bool?[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serialize(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(byte[] value)
        {
            if (checkPoint(value)) Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(byte[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serialize(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(byte?[] value)
        {
            if (checkPoint(value)) Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(byte?[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serialize(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(sbyte[] value)
        {
            if (checkPoint(value)) Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(sbyte[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serialize(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(sbyte?[] value)
        {
            if (checkPoint(value)) Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(sbyte?[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serialize(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(short[] value)
        {
            if (checkPoint(value)) Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(short[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serialize(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(short?[] value)
        {
            if (checkPoint(value)) Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(short?[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serialize(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(ushort[] value)
        {
            if (checkPoint(value)) Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(ushort[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serialize(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(ushort?[] value)
        {
            if (checkPoint(value)) Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(ushort?[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serialize(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(int[] value)
        {
            if (checkPoint(value)) Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(int[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serialize(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(int?[] value)
        {
            if (checkPoint(value)) Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(int?[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serialize(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(uint[] value)
        {
            if (checkPoint(value)) Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(uint[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serialize(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(uint?[] value)
        {
            if (checkPoint(value)) Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(uint?[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serialize(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(long[] value)
        {
            if (checkPoint(value)) Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(long[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serialize(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(long?[] value)
        {
            if (checkPoint(value)) Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(long?[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serialize(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(ulong[] value)
        {
            if (checkPoint(value)) Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(ulong[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serialize(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(ulong?[] value)
        {
            if (checkPoint(value)) Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(ulong?[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serialize(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(float[] value)
        {
            if (checkPoint(value)) Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(float[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serialize(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(float?[] value)
        {
            if (checkPoint(value)) Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(float?[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serialize(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(double[] value)
        {
            if (checkPoint(value)) Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(double[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serialize(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(double?[] value)
        {
            if (checkPoint(value)) Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(double?[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serialize(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(decimal[] value)
        {
            if (checkPoint(value)) Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(decimal[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serialize(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(decimal?[] value)
        {
            if (checkPoint(value)) Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(decimal?[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serialize(value);
        }
        /// <summary>
        /// 字符序列化
        /// </summary>
        /// <param name="value">字符</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(char[] value)
        {
            if (checkPoint(value)) Serialize(Stream, value);
        }
        /// <summary>
        /// 字符序列化
        /// </summary>
        /// <param name="value">字符</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(char[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serialize(value);
        }
        /// <summary>
        /// 字符序列化
        /// </summary>
        /// <param name="value">字符</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(char?[] value)
        {
            if (checkPoint(value)) Serialize(Stream, value);
        }
        /// <summary>
        /// 字符序列化
        /// </summary>
        /// <param name="value">字符</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(char?[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serialize(value);
        }
        /// <summary>
        /// 时间序列化
        /// </summary>
        /// <param name="value">时间</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(DateTime[] value)
        {
            if (checkPoint(value)) Serialize(Stream, value);
        }
        /// <summary>
        /// 时间序列化
        /// </summary>
        /// <param name="value">时间</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(DateTime[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serialize(value);
        }
        /// <summary>
        /// 时间序列化
        /// </summary>
        /// <param name="value">时间</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(DateTime?[] value)
        {
            if (checkPoint(value)) Serialize(Stream, value);
        }
        /// <summary>
        /// 时间序列化
        /// </summary>
        /// <param name="value">时间</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(DateTime?[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serialize(value);
        }
        /// <summary>
        /// Guid序列化
        /// </summary>
        /// <param name="value">Guid</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(Guid[] value)
        {
            if (checkPoint(value)) Serialize(Stream, value);
        }
        /// <summary>
        /// Guid序列化
        /// </summary>
        /// <param name="value">Guid</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(Guid[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serialize(value);
        }
        /// <summary>
        /// Guid序列化
        /// </summary>
        /// <param name="value">Guid</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(Guid?[] value)
        {
            if (checkPoint(value)) Serialize(Stream, value);
        }
        /// <summary>
        /// Guid序列化
        /// </summary>
        /// <param name="value">Guid</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(Guid?[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serialize(value);
        }
        /// <summary>
        /// 字符串序列化
        /// </summary>
        /// <param name="value">字符串</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(string value)
        {
            if (value.Length == 0) Stream.Write(0);
            else if (CheckPoint(value)) Serialize(Stream, value);
        }
        /// <summary>
        /// 字符串序列化
        /// </summary>
        /// <param name="value">字符串</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(string value)
        {
            if (value == null) Stream.Write(NullValue);
            else serialize(value);
        }
        /// <summary>
        /// 字符串序列化
        /// </summary>
        /// <param name="array">字符串数组</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(string[] array)
        {
            if (array == null) Stream.Write(NullValue);
            else serialize(array);
        }
        /// <summary>
        /// 字符串序列化
        /// </summary>
        /// <param name="array">字符串数组</param>
        [serializeMethod]
        private void serialize(string[] array)
        {
            if (checkPoint(array))
            {
                arrayMap arrayMap = new arrayMap(Stream, array.Length, array.Length);
                foreach (string value in array) arrayMap.Next(value != null);
                arrayMap.End(Stream);
                foreach (string value in array)
                {
                    if (value != null)
                    {
                        if (value.Length == 0) Stream.Write(0);
                        else if (CheckPoint(value)) Serialize(Stream, value);
                    }
                }
            }
        }

        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void structSerialize<valueType>(valueType value) where valueType : struct
        {
            typeSerializer<valueType>.StructSerialize(this, value);
        }
        /// <summary>
        /// 对象序列化函数信息
        /// </summary>
        private static readonly MethodInfo structSerializeMethod = typeof(dataSerializer).GetMethod("structSerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 字典序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        private void dictionarySerialize<dictionaryType, keyType, valueType>(dictionaryType value) where dictionaryType : IDictionary<keyType, valueType>
        {
            if (CheckPoint(value))
            {
                int index = 0;
                keyType[] keys = new keyType[value.Count];
                valueType[] values = new valueType[keys.Length];
                foreach (KeyValuePair<keyType, valueType> keyValue in value)
                {
                    keys[index] = keyValue.Key;
                    values[index++] = keyValue.Value;
                }
                isReferenceArray = false;
                typeSerializer<keyType[]>.DefaultSerializer(this, keys);
                isReferenceArray = false;
                typeSerializer<valueType[]>.DefaultSerializer(this, values);
            }
        }
        /// <summary>
        /// 字典序列化函数信息
        /// </summary>
        private static readonly MethodInfo dictionarySerializeMethod = typeof(dataSerializer).GetMethod("dictionarySerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 字典序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void dictionaryMember<dictionaryType, keyType, valueType>(dictionaryType value) where dictionaryType : IDictionary<keyType, valueType>
        {
            if (value == null) Stream.Write(NullValue);
            else dictionarySerialize<dictionaryType, keyType, valueType>(value);
        }
        /// <summary>
        /// 字典序列化函数信息
        /// </summary>
        private static readonly MethodInfo dictionaryMemberMethod = typeof(dataSerializer).GetMethod("dictionaryMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 字典序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void keyValuePairSerialize<keyType, valueType>(KeyValuePair<keyType, valueType> value)
        {
            typeSerializer<keyValue<keyType, valueType>>.MemberSerialize(this, new keyValue<keyType, valueType>(value.Key, value.Value));
        }
        /// <summary>
        /// 字典序列化函数信息
        /// </summary>
        private static readonly MethodInfo keyValuePairSerializeMethod = typeof(dataSerializer).GetMethod("keyValuePairSerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void subArraySerialize<valueType>(subArray<valueType> value)
        {
            valueType[] array = value.ToArray();
            isReferenceArray = false;
            typeSerializer<valueType[]>.DefaultSerializer(this, array);
        }
        /// <summary>
        /// 数组序列化函数信息
        /// </summary>
        private static readonly MethodInfo subArraySerializeMethod = typeof(dataSerializer).GetMethod("subArraySerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="serializer">二进制数据序列化</param>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static void nullableSerialize<valueType>(dataSerializer serializer, Nullable<valueType> value) where valueType : struct
        {
            typeSerializer<valueType>.StructSerialize(serializer, value.Value);
        }
        /// <summary>
        /// 对象序列化函数信息
        /// </summary>
        private static readonly MethodInfo nullableSerializeMethod = typeof(dataSerializer).GetMethod("nullableSerialize", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void nullableMemberSerialize<valueType>(Nullable<valueType> value) where valueType : struct
        {
            if (value.HasValue) typeSerializer<valueType>.StructSerialize(this, value.Value);
            else Stream.Write(NullValue);
        }
        /// <summary>
        /// 对象序列化函数信息
        /// </summary>
        private static readonly MethodInfo nullableMemberSerializeMethod = typeof(dataSerializer).GetMethod("nullableMemberSerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="serializer">二进制数据序列化</param>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static void baseSerialize<valueType, childType>(dataSerializer serializer, childType value) where childType : valueType
        {
            typeSerializer<valueType>.BaseSerialize(serializer, value);
        }
        /// <summary>
        /// 对象序列化函数信息
        /// </summary>
        private static readonly MethodInfo baseSerializeMethod = typeof(dataSerializer).GetMethod("baseSerialize", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 真实类型序列化
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="value"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static void realTypeObject<valueType>(dataSerializer serializer, object value)
        {
            typeSerializer<valueType>.RealTypeObject(serializer, value);
        }
        /// <summary>
        /// 真实类型序列化函数信息
        /// </summary>
        private static readonly MethodInfo realTypeObjectMethod = typeof(dataSerializer).GetMethod("realTypeObject", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 集合转换
        /// </summary>
        /// <param name="collection">对象集合</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void structCollection<valueType, collectionType>(collectionType collection) where collectionType : ICollection<valueType>
        {
            isReferenceArray = false;
            typeSerializer<valueType[]>.DefaultSerializer(this, collection.getArray());
        }
        /// <summary>
        /// 集合序列化函数信息
        /// </summary>
        private static readonly MethodInfo structCollectionMethod = typeof(dataSerializer).GetMethod("structCollection", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 集合转换
        /// </summary>
        /// <param name="collection">对象集合</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void classCollection<valueType, collectionType>(collectionType collection) where collectionType : ICollection<valueType>
        {
            if (CheckPoint(collection)) structCollection<valueType, collectionType>(collection);
        }
        /// <summary>
        /// 集合序列化函数信息
        /// </summary>
        private static readonly MethodInfo classCollectionMethod = typeof(dataSerializer).GetMethod("classCollection", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 集合转换
        /// </summary>
        /// <param name="dictionary">对象集合</param>
        private void structDictionary<dictionaryType, keyType, valueType>(dictionaryType dictionary) where dictionaryType : IDictionary<keyType, valueType>
        {
            keyType[] keys = new keyType[dictionary.Count];
            valueType[] values = new valueType[keys.Length];
            int index = 0;
            foreach (KeyValuePair<keyType, valueType> keyValue in dictionary)
            {
                keys[index] = keyValue.Key;
                values[index++] = keyValue.Value;
            }
            isReferenceArray = false;
            typeSerializer<keyType[]>.DefaultSerializer(this, keys);
            isReferenceArray = false;
            typeSerializer<valueType[]>.DefaultSerializer(this, values);
        }
        /// <summary>
        /// 集合序列化函数信息
        /// </summary>
        private static readonly MethodInfo structDictionaryMethod = typeof(dataSerializer).GetMethod("structDictionary", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 集合转换
        /// </summary>
        /// <param name="dictionary">对象集合</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void classDictionary<dictionaryType, keyType, valueType>(dictionaryType dictionary) where dictionaryType : IDictionary<keyType, valueType>
        {
            if (CheckPoint(dictionary)) structDictionary<dictionaryType, keyType, valueType>(dictionary);
        }
        /// <summary>
        /// 集合序列化函数信息
        /// </summary>
        private static readonly MethodInfo classDictionaryMethod = typeof(dataSerializer).GetMethod("classDictionary", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举集合序列化
        /// </summary>
        /// <param name="collection">枚举集合序列化</param>
        private unsafe void structEnumByteCollection<valueType, collectionType>(collectionType collection) where collectionType : ICollection<valueType>
        {
            int count = collection.Count, length = (count + 7) & (int.MaxValue - 3);
            Stream.PrepLength(length);
            byte* write = Stream.CurrentData;
            *(int*)write = count;
            write += sizeof(int);
            foreach (valueType value in collection) *write++ = pub.enumCast<valueType, byte>.ToInt(value);
            Stream.UnsafeAddLength(length);
            Stream.PrepLength();
        }
        /// <summary>
        /// 集合序列化函数信息
        /// </summary>
        private static readonly MethodInfo structEnumByteCollectionMethod = typeof(dataSerializer).GetMethod("structEnumByteCollection", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举集合序列化
        /// </summary>
        /// <param name="collection">枚举集合序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void classEnumByteCollection<valueType, collectionType>(collectionType collection) where collectionType : ICollection<valueType>
        {
            if (CheckPoint(collection)) structEnumByteCollection<valueType, collectionType>(collection);
        }
        /// <summary>
        /// 集合序列化函数信息
        /// </summary>
        private static readonly MethodInfo classEnumByteCollectionMethod = typeof(dataSerializer).GetMethod("classEnumByteCollection", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举集合序列化
        /// </summary>
        /// <param name="collection">枚举集合序列化</param>
        private unsafe void structEnumSByteCollection<valueType, collectionType>(collectionType collection) where collectionType : ICollection<valueType>
        {
            int count = collection.Count, length = (count + 7) & (int.MaxValue - 3);
            Stream.PrepLength(length);
            byte* write = Stream.CurrentData;
            *(int*)write = count;
            write += sizeof(int);
            foreach (valueType value in collection) *(sbyte*)write++ = pub.enumCast<valueType, sbyte>.ToInt(value);
            Stream.UnsafeAddLength(length);
            Stream.PrepLength();
        }
        /// <summary>
        /// 集合序列化函数信息
        /// </summary>
        private static readonly MethodInfo structEnumSByteCollectionMethod = typeof(dataSerializer).GetMethod("structEnumSByteCollection", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举集合序列化
        /// </summary>
        /// <param name="collection">枚举集合序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void classEnumSByteCollection<valueType, collectionType>(collectionType collection) where collectionType : ICollection<valueType>
        {
            if (CheckPoint(collection)) structEnumSByteCollection<valueType, collectionType>(collection);
        }
        /// <summary>
        /// 集合序列化函数信息
        /// </summary>
        private static readonly MethodInfo classEnumSByteCollectionMethod = typeof(dataSerializer).GetMethod("classEnumSByteCollection", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举集合序列化
        /// </summary>
        /// <param name="collection">枚举集合序列化</param>
        private unsafe void structEnumShortCollection<valueType, collectionType>(collectionType collection) where collectionType : ICollection<valueType>
        {
            int count = collection.Count, length = ((count * sizeof(short)) + 7) & (int.MaxValue - 3);
            Stream.PrepLength(length);
            byte* write = Stream.CurrentData;
            *(int*)write = count;
            write += sizeof(int);
            foreach (valueType value in collection)
            {
                *(short*)write = pub.enumCast<valueType, short>.ToInt(value);
                write += sizeof(short);
            }
            Stream.UnsafeAddLength(length);
            Stream.PrepLength();
        }
        /// <summary>
        /// 集合序列化函数信息
        /// </summary>
        private static readonly MethodInfo structEnumShortCollectionMethod = typeof(dataSerializer).GetMethod("structEnumShortCollection", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举集合序列化
        /// </summary>
        /// <param name="collection">枚举集合序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void classEnumShortCollection<valueType, collectionType>(collectionType collection) where collectionType : ICollection<valueType>
        {
            if (CheckPoint(collection)) structEnumShortCollection<valueType, collectionType>(collection);
        }
        /// <summary>
        /// 集合序列化函数信息
        /// </summary>
        private static readonly MethodInfo classEnumShortCollectionMethod = typeof(dataSerializer).GetMethod("classEnumShortCollection", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举集合序列化
        /// </summary>
        /// <param name="collection">枚举集合序列化</param>
        private unsafe void structEnumUShortCollection<valueType, collectionType>(collectionType collection) where collectionType : ICollection<valueType>
        {
            int count = collection.Count, length = ((count * sizeof(ushort)) + 7) & (int.MaxValue - 3);
            Stream.PrepLength(length);
            byte* write = Stream.CurrentData;
            *(int*)write = count;
            write += sizeof(int);
            foreach (valueType value in collection)
            {
                *(ushort*)write = pub.enumCast<valueType, ushort>.ToInt(value);
                write += sizeof(ushort);
            }
            Stream.UnsafeAddLength(length);
            Stream.PrepLength();
        }
        /// <summary>
        /// 集合序列化函数信息
        /// </summary>
        private static readonly MethodInfo structEnumUShortCollectionMethod = typeof(dataSerializer).GetMethod("structEnumUShortCollection", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举集合序列化
        /// </summary>
        /// <param name="collection">枚举集合序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void classEnumUShortCollection<valueType, collectionType>(collectionType collection) where collectionType : ICollection<valueType>
        {
            if (CheckPoint(collection)) structEnumUShortCollection<valueType, collectionType>(collection);
        }
        /// <summary>
        /// 集合序列化函数信息
        /// </summary>
        private static readonly MethodInfo classEnumUShortCollectionMethod = typeof(dataSerializer).GetMethod("classEnumUShortCollection", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举集合序列化
        /// </summary>
        /// <param name="collection">枚举集合序列化</param>
        private unsafe void structEnumIntCollection<valueType, collectionType>(collectionType collection) where collectionType : ICollection<valueType>
        {
            int count = collection.Count, length = (count + 1) * sizeof(int);
            Stream.PrepLength(length);
            byte* write = Stream.CurrentData;
            *(int*)write = count;
            foreach (valueType value in collection) *(int*)(write += sizeof(int)) = pub.enumCast<valueType, int>.ToInt(value);
            Stream.UnsafeAddLength(length);
            Stream.PrepLength();
        }
        /// <summary>
        /// 集合序列化函数信息
        /// </summary>
        private static readonly MethodInfo structEnumIntCollectionMethod = typeof(dataSerializer).GetMethod("structEnumIntCollection", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举集合序列化
        /// </summary>
        /// <param name="collection">枚举集合序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void classEnumIntCollection<valueType, collectionType>(collectionType collection) where collectionType : ICollection<valueType>
        {
            if (CheckPoint(collection)) structEnumIntCollection<valueType, collectionType>(collection);
        }
        /// <summary>
        /// 集合序列化函数信息
        /// </summary>
        private static readonly MethodInfo classEnumIntCollectionMethod = typeof(dataSerializer).GetMethod("classEnumIntCollection", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举集合序列化
        /// </summary>
        /// <param name="collection">枚举集合序列化</param>
        private unsafe void structEnumUIntCollection<valueType, collectionType>(collectionType collection) where collectionType : ICollection<valueType>
        {
            int count = collection.Count, length = (count + 1) * sizeof(uint);
            Stream.PrepLength(length);
            byte* write = Stream.CurrentData;
            *(int*)write = count;
            foreach (valueType value in collection) *(uint*)(write += sizeof(uint)) = pub.enumCast<valueType, uint>.ToInt(value);
            Stream.UnsafeAddLength(length);
            Stream.PrepLength();
        }
        /// <summary>
        /// 集合序列化函数信息
        /// </summary>
        private static readonly MethodInfo structEnumUIntCollectionMethod = typeof(dataSerializer).GetMethod("structEnumUIntCollection", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举集合序列化
        /// </summary>
        /// <param name="collection">枚举集合序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void classEnumUIntCollection<valueType, collectionType>(collectionType collection) where collectionType : ICollection<valueType>
        {
            if (CheckPoint(collection)) structEnumUIntCollection<valueType, collectionType>(collection);
        }
        /// <summary>
        /// 集合序列化函数信息
        /// </summary>
        private static readonly MethodInfo classEnumUIntCollectionMethod = typeof(dataSerializer).GetMethod("classEnumUIntCollection", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举集合序列化
        /// </summary>
        /// <param name="collection">枚举集合序列化</param>
        private unsafe void structEnumLongCollection<valueType, collectionType>(collectionType collection) where collectionType : ICollection<valueType>
        {
            int count = collection.Count, length = count * sizeof(long) + sizeof(int);
            Stream.PrepLength(length);
            byte* write = Stream.CurrentData;
            *(int*)write = count;
            write += sizeof(int);
            foreach (valueType value in collection)
            {
                *(long*)write = pub.enumCast<valueType, long>.ToInt(value);
                write += sizeof(long);
            }
            Stream.UnsafeAddLength(length);
            Stream.PrepLength();
        }
        /// <summary>
        /// 集合序列化函数信息
        /// </summary>
        private static readonly MethodInfo structEnumLongCollectionMethod = typeof(dataSerializer).GetMethod("structEnumLongCollection", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举集合序列化
        /// </summary>
        /// <param name="collection">枚举集合序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void classEnumLongCollection<valueType, collectionType>(collectionType collection) where collectionType : ICollection<valueType>
        {
            if (CheckPoint(collection)) structEnumLongCollection<valueType, collectionType>(collection);
        }
        /// <summary>
        /// 集合序列化函数信息
        /// </summary>
        private static readonly MethodInfo classEnumLongCollectionMethod = typeof(dataSerializer).GetMethod("classEnumLongCollection", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举集合序列化
        /// </summary>
        /// <param name="collection">枚举集合序列化</param>
        private unsafe void structEnumULongCollection<valueType, collectionType>(collectionType collection) where collectionType : ICollection<valueType>
        {
            int count = collection.Count, length = count * sizeof(ulong) + sizeof(int);
            Stream.PrepLength(length);
            byte* write = Stream.CurrentData;
            *(int*)write = count;
            write += sizeof(int);
            foreach (valueType value in collection)
            {
                *(ulong*)write = pub.enumCast<valueType, ulong>.ToInt(value);
                write += sizeof(ulong);
            }
            Stream.UnsafeAddLength(length);
            Stream.PrepLength();
        }
        /// <summary>
        /// 集合序列化函数信息
        /// </summary>
        private static readonly MethodInfo structEnumULongCollectionMethod = typeof(dataSerializer).GetMethod("structEnumULongCollection", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举集合序列化
        /// </summary>
        /// <param name="collection">枚举集合序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void classEnumULongCollection<valueType, collectionType>(collectionType collection) where collectionType : ICollection<valueType>
        {
            if (CheckPoint(collection)) structEnumULongCollection<valueType, collectionType>(collection);
        }
        /// <summary>
        /// 集合序列化函数信息
        /// </summary>
        private static readonly MethodInfo classEnumULongCollectionMethod = typeof(dataSerializer).GetMethod("classEnumULongCollection", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举转换函数信息
        /// </summary>
        private static readonly MethodInfo enumByteMemberMethod = typeof(dataSerializer).GetMethod("enumByteMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举转换函数信息
        /// </summary>
        private static readonly MethodInfo enumSByteMemberMethod = typeof(dataSerializer).GetMethod("enumSByteMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举转换函数信息
        /// </summary>
        private static readonly MethodInfo enumShortMemberMethod = typeof(dataSerializer).GetMethod("enumShortMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举转换函数信息
        /// </summary>
        private static readonly MethodInfo enumUShortMemberMethod = typeof(dataSerializer).GetMethod("enumUShortMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举转换函数信息
        /// </summary>
        private static readonly MethodInfo enumIntMemberMethod = typeof(dataSerializer).GetMethod("enumIntMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举转换函数信息
        /// </summary>
        private static readonly MethodInfo enumUIntMemberMethod = typeof(dataSerializer).GetMethod("enumUIntMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举转换函数信息
        /// </summary>
        private static readonly MethodInfo enumLongMemberMethod = typeof(dataSerializer).GetMethod("enumLongMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举转换函数信息
        /// </summary>
        private static readonly MethodInfo enumULongMemberMethod = typeof(dataSerializer).GetMethod("enumULongMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumByteArray<valueType>(valueType[] array)
        {
            if (checkPoint(array))
            {
                int length = (array.Length + 7) & (int.MaxValue - 3);
                Stream.PrepLength(length);
                byte* write = Stream.CurrentData;
                *(int*)write = array.Length;
                write += sizeof(int);
                foreach (valueType value in array) *write++ = pub.enumCast<valueType, byte>.ToInt(value);
                Stream.UnsafeAddLength(length);
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo enumByteArrayMethod = typeof(dataSerializer).GetMethod("enumByteArray", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void enumByteArrayMember<valueType>(valueType[] array)
        {
            if (array == null) Stream.Write(NullValue);
            else enumByteArray(array);
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo enumByteArrayMemberMethod = typeof(dataSerializer).GetMethod("enumByteArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumSByteArray<valueType>(valueType[] array)
        {
            if (checkPoint(array))
            {
                int length = (array.Length + 7) & (int.MaxValue - 3);
                Stream.PrepLength(length);
                byte* write = Stream.CurrentData;
                *(int*)write = array.Length;
                write += sizeof(int);
                foreach (valueType value in array) *(sbyte*)write++ = pub.enumCast<valueType, sbyte>.ToInt(value);
                Stream.UnsafeAddLength(length);
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo enumSByteArrayMethod = typeof(dataSerializer).GetMethod("enumSByteArray", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void enumSByteArrayMember<valueType>(valueType[] array)
        {
            if (array == null) Stream.Write(NullValue);
            else enumSByteArray(array);
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo enumSByteArrayMemberMethod = typeof(dataSerializer).GetMethod("enumSByteArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumShortArray<valueType>(valueType[] array)
        {
            if (checkPoint(array))
            {
                int length = ((array.Length * sizeof(short)) + 7) & (int.MaxValue - 3);
                Stream.PrepLength(length);
                byte* write = Stream.CurrentData;
                *(int*)write = array.Length;
                write += sizeof(int);
                foreach (valueType value in array)
                {
                    *(short*)write = pub.enumCast<valueType, short>.ToInt(value);
                    write += sizeof(short);
                }
                Stream.UnsafeAddLength(length);
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo enumShortArrayMethod = typeof(dataSerializer).GetMethod("enumShortArray", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void enumShortArrayMember<valueType>(valueType[] array)
        {
            if (array == null) Stream.Write(NullValue);
            else enumShortArray(array);
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo enumShortArrayMemberMethod = typeof(dataSerializer).GetMethod("enumShortArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumUShortArray<valueType>(valueType[] array)
        {
            if (checkPoint(array))
            {
                int length = ((array.Length * sizeof(ushort)) + 7) & (int.MaxValue - 3);
                Stream.PrepLength(length);
                byte* write = Stream.CurrentData;
                *(int*)write = array.Length;
                write += sizeof(int);
                foreach (valueType value in array)
                {
                    *(ushort*)write = pub.enumCast<valueType, ushort>.ToInt(value);
                    write += sizeof(ushort);
                }
                Stream.UnsafeAddLength(length);
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo enumUShortArrayMethod = typeof(dataSerializer).GetMethod("enumUShortArray", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void enumUShortArrayMember<valueType>(valueType[] array)
        {
            if (array == null) Stream.Write(NullValue);
            else enumUShortArray(array);
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo enumUShortArrayMemberMethod = typeof(dataSerializer).GetMethod("enumUShortArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumIntArray<valueType>(valueType[] array)
        {
            if (checkPoint(array))
            {
                int length = (array.Length + 1) * sizeof(int);
                Stream.PrepLength(length);
                byte* write = Stream.CurrentData;
                *(int*)write = array.Length;
                foreach (valueType value in array) *(int*)(write += sizeof(int)) = pub.enumCast<valueType, int>.ToInt(value);
                Stream.UnsafeAddLength(length);
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo enumIntArrayMethod = typeof(dataSerializer).GetMethod("enumIntArray", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void enumIntArrayMember<valueType>(valueType[] array)
        {
            if (array == null) Stream.Write(NullValue);
            else enumIntArray(array);
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo enumIntArrayMemberMethod = typeof(dataSerializer).GetMethod("enumIntArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumUIntArray<valueType>(valueType[] array)
        {
            if (checkPoint(array))
            {
                int length = (array.Length + 1) * sizeof(uint);
                Stream.PrepLength(length);
                byte* write = Stream.CurrentData;
                *(int*)write = array.Length;
                foreach (valueType value in array) *(uint*)(write += sizeof(uint)) = pub.enumCast<valueType, uint>.ToInt(value);
                Stream.UnsafeAddLength(length);
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo enumUIntArrayMethod = typeof(dataSerializer).GetMethod("enumUIntArray", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void enumUIntArrayMember<valueType>(valueType[] array)
        {
            if (array == null) Stream.Write(NullValue);
            else enumUIntArray(array);
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo enumUIntArrayMemberMethod = typeof(dataSerializer).GetMethod("enumUIntArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumLongArray<valueType>(valueType[] array)
        {
            if (checkPoint(array))
            {
                int length = array.Length * sizeof(long) + sizeof(int);
                Stream.PrepLength(length);
                byte* write = Stream.CurrentData;
                *(int*)write = array.Length;
                write += sizeof(int);
                foreach (valueType value in array)
                {
                    *(long*)write = pub.enumCast<valueType, long>.ToInt(value);
                    write += sizeof(long);
                }
                Stream.UnsafeAddLength(length);
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo enumLongArrayMethod = typeof(dataSerializer).GetMethod("enumLongArray", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void enumLongArrayMember<valueType>(valueType[] array)
        {
            if (array == null) Stream.Write(NullValue);
            else enumLongArray(array);
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo enumLongArrayMemberMethod = typeof(dataSerializer).GetMethod("enumLongArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumULongArray<valueType>(valueType[] array)
        {
            if (checkPoint(array))
            {
                int length = array.Length * sizeof(ulong) + sizeof(int);
                Stream.PrepLength(length);
                byte* write = Stream.CurrentData;
                *(int*)write = array.Length;
                write += sizeof(int);
                foreach (valueType value in array)
                {
                    *(ulong*)write = pub.enumCast<valueType, ulong>.ToInt(value);
                    write += sizeof(ulong);
                }
                Stream.UnsafeAddLength(length);
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo enumULongArrayMethod = typeof(dataSerializer).GetMethod("enumULongArray", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void enumULongArrayMember<valueType>(valueType[] array)
        {
            if (array == null) Stream.Write(NullValue);
            else enumULongArray(array);
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo enumULongArrayMemberMethod = typeof(dataSerializer).GetMethod("enumULongArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        private void nullableArray<valueType>(Nullable<valueType>[] array) where valueType : struct
        {
            if (checkPoint(array))
            {
                arrayMap arrayMap = new arrayMap(Stream, array.Length);
                foreach (Nullable<valueType> value in array) arrayMap.Next(value.HasValue);
                arrayMap.End(Stream);

                foreach (Nullable<valueType> value in array)
                {
                    if (value.HasValue) typeSerializer<valueType>.StructSerialize(this, value.Value);
                }
            }
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo nullableArrayMethod = typeof(dataSerializer).GetMethod("nullableArray", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void nullableArrayMember<valueType>(Nullable<valueType>[] array) where valueType : struct
        {
            if (array == null) Stream.Write(NullValue);
            else nullableArray(array);
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo nullableArrayMemberMethod = typeof(dataSerializer).GetMethod("nullableArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        private void structArray<valueType>(valueType[] array)
        {
            if (checkPoint(array))
            {
                Stream.Write(array.Length);
                foreach (valueType value in array) typeSerializer<valueType>.StructSerialize(this, value);
            }
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo structArrayMethod = typeof(dataSerializer).GetMethod("structArray", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void structArrayMember<valueType>(valueType[] array)
        {
            if (array == null) Stream.Write(NullValue);
            else structArray(array);
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo structArrayMemberMethod = typeof(dataSerializer).GetMethod("structArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        private void array<valueType>(valueType[] array) where valueType : class
        {
            if (checkPoint(array))
            {
                arrayMap arrayMap = new arrayMap(Stream, array.Length);
                foreach (valueType value in array) arrayMap.Next(value != null);
                arrayMap.End(Stream);

                foreach (valueType value in array)
                {
                    if (value != null) typeSerializer<valueType>.ClassSerialize(this, value);
                }
            }
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo arrayMethod = typeof(dataSerializer).GetMethod("array", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void arrayMember<valueType>(valueType[] array) where valueType : class
        {
            if (array == null) Stream.Write(NullValue);
            else this.array(array);
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo arrayMemberMethod = typeof(dataSerializer).GetMethod("arrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 序列化接口
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="value"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void structISerialize<valueType>(valueType value) where valueType : struct, fastCSharp.code.cSharp.dataSerialize.ISerialize
        {
            value.Serialize(this);
        }
        /// <summary>
        /// 序列化接口函数信息
        /// </summary>
        private static readonly MethodInfo structISerializeMethod = typeof(dataSerializer).GetMethod("structISerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 序列化接口
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="value"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void classISerialize<valueType>(valueType value) where valueType : class, fastCSharp.code.cSharp.dataSerialize.ISerialize
        {
            value.Serialize(this);
        }
        /// <summary>
        /// 序列化接口函数信息
        /// </summary>
        private static readonly MethodInfo classISerializeMethod = typeof(dataSerializer).GetMethod("classISerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 序列化接口
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="value"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberClassISerialize<valueType>(valueType value) where valueType : class, fastCSharp.code.cSharp.dataSerialize.ISerialize
        {
            if (value == null) Stream.Write(NullValue);
            else value.Serialize(this);
        }
        /// <summary>
        /// 序列化接口函数信息
        /// </summary>
        private static readonly MethodInfo memberClassISerializeMethod = typeof(dataSerializer).GetMethod("memberClassISerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 引用类型成员序列化
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="value"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void MemberClassSerialize<valueType>(valueType value) where valueType : class
        {
            if (value == null) Stream.Write(NullValue);
            else typeSerializer<valueType>.ClassSerialize(this, value);
        }
        /// <summary>
        /// 引用类型成员序列化函数信息
        /// </summary>
        private static readonly MethodInfo memberClassSerializeMethod = typeof(dataSerializer).GetMethod("MemberClassSerialize", BindingFlags.Instance | BindingFlags.Public);
        /// <summary>
        /// 未知类型序列化
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="value"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void MemberNullableSerialize<valueType>(Nullable<valueType> value) where valueType : struct
        {
            typeSerializer<valueType>.StructSerialize(this, value.Value);
        }
        /// <summary>
        /// 未知类型序列化
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="value"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void MemberStructSerialize<valueType>(valueType value) where valueType : struct
        {
            typeSerializer<valueType>.StructSerialize(this, value);
        }

        /// <summary>
        /// 公共默认配置参数
        /// </summary>
        private static readonly config defaultConfig = new config();
        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="value">数据对象</param>
        /// <param name="stream">序列化输出缓冲区</param>
        /// <param name="config">配置参数</param>
        public static void Serialize<valueType>(valueType value, unmanagedStream stream, config config = null)
        {
            if (stream == null) log.Default.Throw(log.exceptionType.Null);
            if (value == null) stream.Write(fastCSharp.emit.binarySerializer.NullValue);
            else
            {
                dataSerializer serializer = typePool<dataSerializer>.Pop() ?? new dataSerializer();
                try
                {
                    serializer.serialize<valueType>(value, stream, config ?? defaultConfig);
                }
                finally { serializer.free(); }
            }
        }
        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="value">数据对象</param>
        /// <param name="config">配置参数</param>
        /// <returns>序列化数据</returns>
        public static byte[] Serialize<valueType>(valueType value, config config = null)
        {
            if (value == null) return BitConverter.GetBytes(fastCSharp.emit.binarySerializer.NullValue);
            dataSerializer serializer = typePool<dataSerializer>.Pop() ?? new dataSerializer();
            try
            {
                return serializer.serialize<valueType>(value, config ?? defaultConfig);
            }
            finally { serializer.free(); }
        }
        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="value">数据对象</param>
        /// <param name="stream">序列化输出缓冲区</param>
        /// <param name="config">配置参数</param>
        public static void CodeSerialize<valueType>(valueType value, unmanagedStream stream, config config = null) where valueType : fastCSharp.code.cSharp.dataSerialize.ISerialize
        {
            if (stream == null) log.Default.Throw(log.exceptionType.Null);
            if (value == null) stream.Write(fastCSharp.emit.binarySerializer.NullValue);
            else
            {
                dataSerializer serializer = typePool<dataSerializer>.Pop() ?? new dataSerializer();
                try
                {
                    serializer.codeSerialize<valueType>(value, stream, config ?? defaultConfig);
                }
                finally { serializer.free(); }
            }
        }
        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="value">数据对象</param>
        /// <param name="config">配置参数</param>
        /// <returns>序列化数据</returns>
        public static byte[] CodeSerialize<valueType>(valueType value, config config = null) where valueType : fastCSharp.code.cSharp.dataSerialize.ISerialize
        {
            if (value == null) return BitConverter.GetBytes(fastCSharp.emit.binarySerializer.NullValue);
            dataSerializer serializer = typePool<dataSerializer>.Pop() ?? new dataSerializer();
            try
            {
                return serializer.codeSerialize<valueType>(value, config ?? defaultConfig);
            }
            finally { serializer.free(); }
        }
        /// <summary>
        /// 未知类型对象序列化
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="value">数据对象</param>
        /// <param name="config">配置参数</param>
        /// <returns>序列化数据</returns>
        private static byte[] objectSerialize<valueType>(object value, config config)
        {
            dataSerializer serializer = typePool<dataSerializer>.Pop() ?? new dataSerializer();
            try
            {
                return serializer.serialize<valueType>((valueType)value, config ?? defaultConfig);
            }
            finally { serializer.free(); }
        }
        /// <summary>
        /// 未知类型对象序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        /// <param name="config">配置参数</param>
        /// <returns>序列化数据</returns>
        public static byte[] ObjectSerialize(object value, config config = null)
        {
            if (value == null) return BitConverter.GetBytes(fastCSharp.emit.binarySerializer.NullValue);
            Type type = value.GetType();
            Func<object, config, byte[]> serializer;
            if (!objectSerializes.TryGetValue(type, out serializer))
            {
                serializer = (Func<object, config, byte[]>)Delegate.CreateDelegate(typeof(Func<object, config, byte[]>), objectSerializeMethod.MakeGenericMethod(type));
                objectSerializes.Set(type, serializer);
            }
            return serializer(value, config);
        }
        /// <summary>
        /// 未知类型对象序列化
        /// </summary>
        private static readonly interlocked.dictionary<Type, Func<object, config, byte[]>> objectSerializes = new interlocked.dictionary<Type, Func<object, config, byte[]>>();
        /// <summary>
        /// 未知类型对象序列化
        /// </summary>
        private static readonly MethodInfo objectSerializeMethod = typeof(dataSerializer).GetMethod("objectSerialize", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(object), typeof(config) }, null);
        ///// <summary>
        ///// 序列化数据流字段信息
        ///// </summary>
        //private static readonly FieldInfo serializeStreamField = typeof(dataSerializer).GetField("Stream", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 基本类型转换函数
        /// </summary>
        private static readonly Dictionary<Type, MethodInfo> serializeMethods;
        /// <summary>
        /// 获取基本类型转换函数
        /// </summary>
        /// <param name="type">基本类型</param>
        /// <returns>转换函数</returns>
        private static MethodInfo getSerializeMethod(Type type)
        {
            MethodInfo method;
            if (serializeMethods.TryGetValue(type, out method))
            {
                serializeMethods.Remove(type);
                return method;
            }
            return null;
        }
        /// <summary>
        /// 基本类型转换函数
        /// </summary>
        private static readonly Dictionary<Type, MethodInfo> memberSerializeMethods;
        /// <summary>
        /// 获取基本类型转换函数
        /// </summary>
        /// <param name="type">基本类型</param>
        /// <returns>转换函数</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static MethodInfo getMemberSerializeMethod(Type type)
        {
            MethodInfo method;
            return memberSerializeMethods.TryGetValue(type, out method) ? method : null;
        }
        /// <summary>
        /// 基本类型转换函数
        /// </summary>
        private static readonly Dictionary<Type, MethodInfo> memberMapSerializeMethods;
        /// <summary>
        /// 获取基本类型转换函数
        /// </summary>
        /// <param name="type">基本类型</param>
        /// <returns>转换函数</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static MethodInfo getMemberMapSerializeMethod(Type type)
        {
            MethodInfo method;
            return memberMapSerializeMethods.TryGetValue(type, out method) ? method : null;
        }
        static dataSerializer()
        {
            serializeMethods = dictionary.CreateOnly<Type, MethodInfo>();
            memberSerializeMethods = dictionary.CreateOnly<Type, MethodInfo>();
            memberMapSerializeMethods = dictionary.CreateOnly<Type, MethodInfo>();
            foreach (MethodInfo method in typeof(dataSerializer).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                Type parameterType = null;
                if (method.customAttribute<serializeMethod>() != null)
                {
                    serializeMethods.Add(parameterType = method.GetParameters()[0].ParameterType, method);
                }
                if (method.customAttribute<memberSerializeMethod>() != null)
                {
                    if (parameterType == null) parameterType = method.GetParameters()[0].ParameterType;
                    memberSerializeMethods.Add(parameterType, method);
                }
                if (method.customAttribute<memberMapSerializeMethod>() != null)
                {
                    memberMapSerializeMethods.Add(parameterType ?? method.GetParameters()[0].ParameterType, method);
                }
            }
        }
    }
}
