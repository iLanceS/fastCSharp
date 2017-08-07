using System;
using System.Reflection;
using fastCSharp.reflection;
using System.Runtime.CompilerServices;
using fastCSharp.code;
using System.Collections.Generic;
using fastCSharp.threading;
#if NOJIT
#else
using System.Reflection.Emit;
#endif

namespace fastCSharp.emit
{
    /// <summary>
    /// 二进制数据序列化(内存数据库专用)
    /// </summary>
    public unsafe sealed class indexSerializer : binarySerializer
    {
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
        /// 字段信息
        /// </summary>
        internal new class fieldInfo : binarySerializer.fieldInfo
        {
            /// <summary>
            /// 固定分组排序字节数
            /// </summary>
            internal byte MemberMapFixedSize;
            /// <summary>
            /// 字段信息
            /// </summary>
            /// <param name="field"></param>
            internal fieldInfo(fieldIndex field)
                : base(field)
            {
                if (Field.FieldType.IsEnum) memberMapFixedSizes.TryGetValue(Field.FieldType.GetEnumUnderlyingType(), out MemberMapFixedSize);
                else memberMapFixedSizes.TryGetValue(Field.FieldType.nullableType() ?? Field.FieldType, out MemberMapFixedSize);
            }
            /// <summary>
            /// 固定类型字节数
            /// </summary>
            private static readonly Dictionary<Type, byte> memberMapFixedSizes;
            static fieldInfo()
            {
                memberMapFixedSizes = fastCSharp.dictionary.CreateOnly<Type, byte>();
                memberMapFixedSizes.Add(typeof(bool), sizeof(int));
                memberMapFixedSizes.Add(typeof(byte), sizeof(int));
                memberMapFixedSizes.Add(typeof(sbyte), sizeof(int));
                memberMapFixedSizes.Add(typeof(short), sizeof(int));
                memberMapFixedSizes.Add(typeof(ushort), sizeof(int));
                memberMapFixedSizes.Add(typeof(int), sizeof(int));
                memberMapFixedSizes.Add(typeof(uint), sizeof(uint));
                memberMapFixedSizes.Add(typeof(long), sizeof(long));
                memberMapFixedSizes.Add(typeof(ulong), sizeof(ulong));
                memberMapFixedSizes.Add(typeof(char), sizeof(int));
                memberMapFixedSizes.Add(typeof(DateTime), sizeof(long));
                memberMapFixedSizes.Add(typeof(float), sizeof(float));
                memberMapFixedSizes.Add(typeof(double), sizeof(double));
                memberMapFixedSizes.Add(typeof(decimal), sizeof(decimal));
                memberMapFixedSizes.Add(typeof(Guid), (byte)sizeof(Guid));
            }
        }
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
                    dynamicMethod = new DynamicMethod("indexSerializer", null, new Type[] { typeof(indexSerializer), type }, type, true);
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
                    MethodInfo method = indexSerializer.getMemberSerializeMethod(field.Field.FieldType) ?? GetMemberSerializer(field.Field.FieldType);
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
                    dynamicMethod = new DynamicMethod("indexMemberMapSerializer", null, new Type[] { typeof(memberMap), typeof(indexSerializer), type }, type, true);
                    generator = dynamicMethod.GetILGenerator();
                    isValueType = type.IsValueType;
                }
                /// <summary>
                /// 添加字段
                /// </summary>
                /// <param name="field">字段信息</param>
                /// <param name="isFixed"></param>
                public void Push(fieldInfo field, bool isFixed)
                {
                    Label end = generator.DefineLabel();
                    generator.memberMapIsMember(OpCodes.Ldarg_0, field.MemberIndex);
                    generator.Emit(OpCodes.Brfalse_S, end);

                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Ldfld, StreamField);
                    if (isFixed) generator.unmanagedStreamUnsafeWriteInt(field.MemberIndex);
                    else generator.unmanagedStreamWriteInt(field.MemberIndex);
                    
                    generator.Emit(OpCodes.Ldarg_1);
                    if (isValueType) generator.Emit(OpCodes.Ldarga_S, 2);
                    else generator.Emit(OpCodes.Ldarg_2);
                    generator.Emit(OpCodes.Ldfld, field.Field);
                    MethodInfo method = indexSerializer.getMemberMapSerializeMethod(field.Field.FieldType) ?? GetMemberMapSerializer(field.Field.FieldType);
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
            /// <param name="memberMapFixedSize"></param>
            /// <returns>字段成员集合</returns>
            public static fields<fieldInfo> GetFields(fieldIndex[] fieldIndexs, out int memberCountVerify, out int memberMapFixedSize)
            {
                subArray<fieldInfo> fixedFields = new subArray<fieldInfo>(fieldIndexs.Length), fields = new subArray<fieldInfo>();
                subArray<fieldIndex> jsonFields = new subArray<fieldIndex>();
                fields.UnsafeSet(fixedFields.array, fixedFields.array.length(), 0);
                int fixedSize = memberMapFixedSize = 0;
                foreach (fieldIndex field in fieldIndexs)
                {
                    Type type = field.Member.FieldType;
                    if (!type.IsPointer && (!type.IsArray || type.GetArrayRank() == 1) && !field.IsIgnore)
                    {
                        indexSerialize.member attribute = field.GetAttribute<indexSerialize.member>(true, true);
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
                                    memberMapFixedSize += value.MemberMapFixedSize + sizeof(int);
                                }
                            }
                        }
                    }
                }
                memberCountVerify = fixedFields.length + fields.length + 0x40000000;
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
                        indexSerialize.member attribute = field.GetAttribute<indexSerialize.member>(true, true);
                        if (attribute == null || attribute.IsSetup) fields.Add(field);
                    }
                }
                return fields;
            }

            /// <summary>
            /// 未知类型序列化调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> memberSerializers = new interlocked.dictionary<Type, MethodInfo>();
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
                            method = memberKeyValuePairSerializeMethod.MakeGenericMethod(type.GetGenericArguments());
                        }
                    }
                    if (method == null)
                    {
                        if (type.IsValueType) method = structSerializeMethod.MakeGenericMethod(type);
                        else method = classSerializeMethod.MakeGenericMethod(type);
                    }
                }
                memberSerializers.Set(type, method);
                return method;
            }
            /// <summary>
            /// 未知类型序列化调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> memberMapSerializers = new interlocked.dictionary<Type, MethodInfo>();
            /// <summary>
            /// 未知类型枚举序列化委托调用函数信息
            /// </summary>
            /// <param name="type">数组类型</param>
            /// <returns>未知类型序列化委托调用函数信息</returns>
            public static MethodInfo GetMemberMapSerializer(Type type)
            {
                MethodInfo method;
                if (memberMapSerializers.TryGetValue(type, out method)) return method;
                if (type.IsArray)
                {
                    Type elementType = type.GetElementType();
                    if (elementType.IsValueType)
                    {
                        if (elementType.IsEnum)
                        {
                            Type enumType = System.Enum.GetUnderlyingType(elementType);
                            if (enumType == typeof(uint)) method = enumUIntArrayMemberMapMethod;
                            else if (enumType == typeof(byte)) method = enumByteArrayMemberMapMethod;
                            else if (enumType == typeof(ulong)) method = enumULongArrayMemberMapMethod;
                            else if (enumType == typeof(ushort)) method = enumUShortArrayMemberMapMethod;
                            else if (enumType == typeof(long)) method = enumLongArrayMemberMapMethod;
                            else if (enumType == typeof(short)) method = enumShortArrayMemberMapMethod;
                            else if (enumType == typeof(sbyte)) method = enumSByteArrayMemberMapMethod;
                            else method = enumIntArrayMemberMapMethod;
                            method = method.MakeGenericMethod(elementType);
                        }
                        else if (elementType.IsGenericType && elementType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            method = nullableArrayMemberMapMethod.MakeGenericMethod(elementType.GetGenericArguments());
                        }
                        else method = structArrayMemberMapMethod.MakeGenericMethod(elementType);
                    }
                    else method = arrayMemberMapMethod.MakeGenericMethod(elementType);
                }
                else if (type.IsEnum)
                {
                    Type enumType = System.Enum.GetUnderlyingType(type);
                    if (enumType == typeof(uint)) method = enumUIntMemberMethod;
                    else if (enumType == typeof(byte)) method = enumByteMemberMapMethod;
                    else if (enumType == typeof(ulong)) method = enumULongMemberMethod;
                    else if (enumType == typeof(ushort)) method = enumUShortMemberMapMethod;
                    else if (enumType == typeof(long)) method = enumLongMemberMethod;
                    else if (enumType == typeof(short)) method = enumShortMemberMapMethod;
                    else if (enumType == typeof(sbyte)) method = enumSByteMemberMapMethod;
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
                            method = memberMapDictionaryMethod.MakeGenericMethod(type, parameterTypes[0], parameterTypes[1]);
                        }
                        else if (genericType == typeof(Nullable<>))
                        {
                            method = memberMapNullableSerializeMethod.MakeGenericMethod(type.GetGenericArguments());
                        }
                        else if (genericType == typeof(KeyValuePair<,>))
                        {
                            method = memberKeyValuePairSerializeMethod.MakeGenericMethod(type.GetGenericArguments());
                        }
                    }
                    if (method == null)
                    {
                        if (type.IsValueType) method = structSerializeMethod.MakeGenericMethod(type);
                        else method = memberMapClassSerializeMethod.MakeGenericMethod(type);
                    }
                }
                memberMapSerializers.Set(type, method);
                return method;
            }
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
            private static readonly indexSerialize attribute;
            /// <summary>
            /// 序列化委托
            /// </summary>
            internal static readonly Action<indexSerializer, valueType> DefaultSerializer;
            /// <summary>
            /// 固定分组成员序列化
            /// </summary>
            private static readonly Action<indexSerializer, valueType> fixedMemberSerializer;
            /// <summary>
            /// 固定分组成员位图序列化
            /// </summary>
            private static readonly Action<memberMap, indexSerializer, valueType> fixedMemberMapSerializer;
            /// <summary>
            /// 成员序列化
            /// </summary>
            private static readonly Action<indexSerializer, valueType> memberSerializer;
            /// <summary>
            /// 成员位图序列化
            /// </summary>
            private static readonly Action<memberMap, indexSerializer, valueType> memberMapSerializer;
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
            /// 成员位图模式固定分组字节数
            /// </summary>
            private static readonly int memberMapFixedSize;
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="serializer">二进制数据序列化</param>
            /// <param name="value">数据对象</param>
            internal static void Serialize(indexSerializer serializer, valueType value)
            {
                if (DefaultSerializer == null)
                {
                    memberMap memberMap = serializer.SerializeMemberMap<valueType>();
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
                        stream.PrepLength(memberMapFixedSize);
                        fixedMemberMapSerializer(memberMap, serializer, value);
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
                else DefaultSerializer(serializer, value);
            }
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="serializer">二进制数据序列化</param>
            /// <param name="value">数据对象</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal static void BaseSerialize<childType>(indexSerializer serializer, childType value) where childType : valueType
            {
                Serialize(serializer, value);
            }
            /// <summary>
            /// 不支持对象转换null
            /// </summary>
            /// <param name="serializer">二进制数据序列化</param>
            /// <param name="value">数据对象</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private static void toNull(indexSerializer serializer, valueType value)
            {
                serializer.Stream.Write(fastCSharp.emit.binarySerializer.NullValue);
            }
            /// <summary>
            /// 枚举值序列化
            /// </summary>
            /// <param name="serializer">二进制数据序列化</param>
            /// <param name="value">枚举值序列化</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe static void enumByte(indexSerializer serializer, valueType value)
            {
                serializer.Stream.Write((uint)pub.enumCast<valueType, byte>.ToInt(value));
            }
            /// <summary>
            /// 枚举值序列化
            /// </summary>
            /// <param name="serializer">二进制数据序列化</param>
            /// <param name="value">枚举值序列化</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe static void enumSByte(indexSerializer serializer, valueType value)
            {
                serializer.Stream.Write((int)pub.enumCast<valueType, sbyte>.ToInt(value));
            }
            /// <summary>
            /// 枚举值序列化
            /// </summary>
            /// <param name="serializer">二进制数据序列化</param>
            /// <param name="value">枚举值序列化</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe static void enumShort(indexSerializer serializer, valueType value)
            {
                serializer.Stream.Write((int)pub.enumCast<valueType, short>.ToInt(value));
            }
            /// <summary>
            /// 枚举值序列化
            /// </summary>
            /// <param name="serializer">二进制数据序列化</param>
            /// <param name="value">枚举值序列化</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe static void enumUShort(indexSerializer serializer, valueType value)
            {
                serializer.Stream.Write((uint)pub.enumCast<valueType, ushort>.ToInt(value));
            }
            /// <summary>
            /// 枚举值序列化
            /// </summary>
            /// <param name="serializer">二进制数据序列化</param>
            /// <param name="value">枚举值序列化</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe static void enumInt(indexSerializer serializer, valueType value)
            {
                serializer.Stream.Write(pub.enumCast<valueType, int>.ToInt(value));
            }
            /// <summary>
            /// 枚举值序列化
            /// </summary>
            /// <param name="serializer">二进制数据序列化</param>
            /// <param name="value">枚举值序列化</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe static void enumUInt(indexSerializer serializer, valueType value)
            {
                serializer.Stream.Write(pub.enumCast<valueType, uint>.ToInt(value));
            }
            /// <summary>
            /// 枚举值序列化
            /// </summary>
            /// <param name="serializer">二进制数据序列化</param>
            /// <param name="value">枚举值序列化</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe static void enumLong(indexSerializer serializer, valueType value)
            {
                serializer.Stream.Write(pub.enumCast<valueType, long>.ToInt(value));
            }
            /// <summary>
            /// 枚举值序列化
            /// </summary>
            /// <param name="serializer">二进制数据序列化</param>
            /// <param name="value">枚举值序列化</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe static void enumULong(indexSerializer serializer, valueType value)
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
                MethodInfo methodInfo = indexSerializer.getSerializeMethod(type);
                attribute = type.customAttribute<indexSerialize>(out attributeType, true) ?? indexSerialize.Default;
                if (methodInfo != null)
                {
#if NOJIT
                    DefaultSerializer = new methodSerializer(methodInfo).Serialize;
#else
                    DynamicMethod dynamicMethod = new DynamicMethod("indexSerializer", typeof(void), new Type[] { typeof(indexSerializer), type }, true);
                    dynamicMethod.InitLocals = true;
                    ILGenerator generator = dynamicMethod.GetILGenerator();
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Call, methodInfo);
                    generator.Emit(OpCodes.Ret);
                    DefaultSerializer = (Action<indexSerializer, valueType>)dynamicMethod.CreateDelegate(typeof(Action<indexSerializer, valueType>));
#endif
                    return;
                }
                if (type.IsArray)
                {
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
                                    if (enumType == typeof(uint)) methodInfo = enumUIntArrayTypeMethod.MakeGenericMethod(elementType);
                                    else if (enumType == typeof(byte)) methodInfo = enumByteArrayTypeMethod.MakeGenericMethod(elementType);
                                    else if (enumType == typeof(ulong)) methodInfo = enumULongArrayTypeMethod.MakeGenericMethod(elementType);
                                    else if (enumType == typeof(ushort)) methodInfo = enumUShortArrayTypeMethod.MakeGenericMethod(elementType);
                                    else if (enumType == typeof(long)) methodInfo = enumLongArrayTypeMethod.MakeGenericMethod(elementType);
                                    else if (enumType == typeof(short)) methodInfo = enumShortArrayTypeMethod.MakeGenericMethod(elementType);
                                    else if (enumType == typeof(sbyte)) methodInfo = enumSByteArrayTypeMethod.MakeGenericMethod(elementType);
                                    else methodInfo = enumIntArrayTypeMethod.MakeGenericMethod(elementType);
                                }
                                else if (elementType.IsGenericType && elementType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                {
                                    methodInfo = nullableArrayTypeMethod.MakeGenericMethod(elementType.GetGenericArguments());
                                }
                                else methodInfo = structArrayTypeMethod.MakeGenericMethod(elementType);
                            }
                            else methodInfo = arrayTypeMethod.MakeGenericMethod(elementType);
                            DefaultSerializer = (Action<indexSerializer, valueType>)Delegate.CreateDelegate(typeof(Action<indexSerializer, valueType>), methodInfo);
                            return;
                        }
                    }
                    DefaultSerializer = toNull;
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
                    return;
                }
                if (type.IsPointer || type.IsAbstract || type.IsInterface)
                {
                    DefaultSerializer = toNull;
                    return;
                }
                if (type.IsGenericType)
                {
                    Type genericType = type.GetGenericTypeDefinition();
                    if (genericType == typeof(subArray<>))
                    {
                        DefaultSerializer = (Action<indexSerializer, valueType>)Delegate.CreateDelegate(typeof(Action<indexSerializer, valueType>), subArraySerializeMethod.MakeGenericMethod(type.GetGenericArguments()));
                        return;
                    }
                    if (genericType == typeof(Dictionary<,>) || genericType == typeof(SortedDictionary<,>) || genericType == typeof(SortedList<,>))
                    {
                        Type[] parameterTypes = type.GetGenericArguments();
                        DefaultSerializer = (Action<indexSerializer, valueType>)Delegate.CreateDelegate(typeof(Action<indexSerializer, valueType>), dictionarySerializeMethod.MakeGenericMethod(type, parameterTypes[0], parameterTypes[1]));
                        return;
                    }
                    if (genericType == typeof(Nullable<>))
                    {
                        DefaultSerializer = (Action<indexSerializer, valueType>)Delegate.CreateDelegate(typeof(Action<indexSerializer, valueType>), nullableSerializeMethod.MakeGenericMethod(type.GetGenericArguments()));
                        return;
                    }
                    if (genericType == typeof(KeyValuePair<,>))
                    {
                        DefaultSerializer = (Action<indexSerializer, valueType>)Delegate.CreateDelegate(typeof(Action<indexSerializer, valueType>), keyValuePairSerializeMethod.MakeGenericMethod(type.GetGenericArguments()));
                        return;
                    }
                }
                ConstructorInfo constructorInfo = null;
                Type argumentType = null;
                foreach (Type interfaceType in type.GetInterfaces())
                {
                    if (interfaceType.IsGenericType)
                    {
                        Type genericType = interfaceType.GetGenericTypeDefinition();
                        if (genericType == typeof(ICollection<>))
                        {
                            Type[] parameters = interfaceType.GetGenericArguments();
                            argumentType = parameters[0];
                            parameters[0] = argumentType.MakeArrayType();
                            constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, parameters, null);
                            if (constructorInfo != null) break;
                            parameters[0] = typeof(IList<>).MakeGenericType(argumentType);
                            constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, parameters, null);
                            if (constructorInfo != null) break;
                            parameters[0] = typeof(ICollection<>).MakeGenericType(argumentType);
                            constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, parameters, null);
                            if (constructorInfo != null) break;
                            parameters[0] = typeof(IEnumerable<>).MakeGenericType(argumentType);
                            constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, parameters, null);
                            if (constructorInfo != null) break;
                        }
                        else if (genericType == typeof(IDictionary<,>))
                        {
                            constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { interfaceType }, null);
                            if (constructorInfo != null)
                            {
                                Type[] parameters = interfaceType.GetGenericArguments();
                                methodInfo = dictionaryMethod.MakeGenericMethod(type, parameters[0], parameters[1]);
                                DefaultSerializer = (Action<indexSerializer, valueType>)Delegate.CreateDelegate(typeof(Action<indexSerializer, valueType>), methodInfo);
                                return;
                            }
                        }
                    }
                }
                if (constructorInfo != null)
                {
                    methodInfo = collectionMethod.MakeGenericMethod(argumentType, type);
                    DefaultSerializer = (Action<indexSerializer, valueType>)Delegate.CreateDelegate(typeof(Action<indexSerializer, valueType>), methodInfo);
                    return;
                }
                if (constructor<valueType>.New == null) DefaultSerializer = toNull;
                else
                {
                    if (!type.IsValueType && attribute != indexSerialize.Default && attributeType != type)
                    {
                        for (Type baseType = type.BaseType; baseType != typeof(object); baseType = baseType.BaseType)
                        {
                            indexSerialize baseAttribute = fastCSharp.code.typeAttribute.GetAttribute<indexSerialize>(baseType, false, true);
                            if (baseAttribute != null)
                            {
                                if (baseAttribute.IsBaseType)
                                {
                                    DefaultSerializer = (Action<indexSerializer, valueType>)Delegate.CreateDelegate(typeof(Action<indexSerializer, valueType>), baseSerializeMethod.MakeGenericMethod(baseType, type));
                                    return;
                                }
                                break;
                            }
                        }
                    }
                    fields<fieldInfo> fields = staticTypeSerializer.GetFields(memberIndexGroup<valueType>.GetFields(attribute.MemberFilter), out memberCountVerify, out memberMapFixedSize);
                    fixedFillSize = -fields.FixedSize & 3;
                    fixedSize = (fields.FixedSize + (sizeof(int) + 3)) & (int.MaxValue - 3);
#if NOJIT
                    fixedMemberSerializer = new serializer(ref fields.FixedFields).Serialize;
                    fixedMemberMapSerializer = new mapSerializer(ref fields.FixedFields).SerializeFixed;
                    memberSerializer = new serializer(ref fields.Fields).Serialize;
                    memberMapSerializer = new mapSerializer(ref fields.Fields).Serialize;
#else
                    staticTypeSerializer.memberDynamicMethod fixedDynamicMethod = new staticTypeSerializer.memberDynamicMethod(type);
                    staticTypeSerializer.memberMapDynamicMethod fixedMemberMapDynamicMethod = new staticTypeSerializer.memberMapDynamicMethod(type);
                    foreach (fieldInfo member in fields.FixedFields)
                    {
                        fixedDynamicMethod.Push(member);
                        fixedMemberMapDynamicMethod.Push(member, true);
                    }
                    fixedMemberSerializer = (Action<indexSerializer, valueType>)fixedDynamicMethod.Create<Action<indexSerializer, valueType>>();
                    fixedMemberMapSerializer = (Action<memberMap, indexSerializer, valueType>)fixedMemberMapDynamicMethod.Create<Action<memberMap, indexSerializer, valueType>>();

                    staticTypeSerializer.memberDynamicMethod dynamicMethod = new staticTypeSerializer.memberDynamicMethod(type);
                    staticTypeSerializer.memberMapDynamicMethod memberMapDynamicMethod = new staticTypeSerializer.memberMapDynamicMethod(type);
                    foreach (fieldInfo member in fields.Fields)
                    {
                        dynamicMethod.Push(member);
                        memberMapDynamicMethod.Push(member, false);
                    }
                    memberSerializer = (Action<indexSerializer, valueType>)dynamicMethod.Create<Action<indexSerializer, valueType>>();
                    memberMapSerializer = (Action<memberMap, indexSerializer, valueType>)memberMapDynamicMethod.Create<Action<memberMap, indexSerializer, valueType>>();
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
                public void Serialize(indexSerializer serializer, valueType value)
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
                public void Serialize(indexSerializer serializer, valueType value)
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
                        SerializeMethod = getMemberMapSerializeMethod(Field.FieldType) ?? staticTypeSerializer.GetMemberMapSerializer(Field.FieldType);
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
                public void Serialize(memberMap memberMap, indexSerializer serializer, valueType value)
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
                            serializer.Stream.Write(field.MemberIndex);
                            parameters[0] = field.Field.GetValue(objectValue);
                            field.SerializeMethod.Invoke(serializer, parameters);
                        }
                    }
                }
                /// <summary>
                /// 序列化
                /// </summary>
                /// <param name="memberMap"></param>
                /// <param name="serializer"></param>
                /// <param name="value"></param>
                public void SerializeFixed(memberMap memberMap, indexSerializer serializer, valueType value)
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
                            serializer.Stream.UnsafeWrite(field.MemberIndex);
                            parameters[0] = field.Field.GetValue(objectValue);
                            field.SerializeMethod.Invoke(serializer, parameters);
                        }
                    }
                }
            }
#endif
        }
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="value">数据对象</param>
        /// <param name="config">配置参数</param>
        /// <returns>序列化数据</returns>
        private byte[] serialize<valueType>(valueType value, config config)
        {
            binarySerializerConfig = config;
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
            binarySerializerConfig = config;
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
        /// <returns>序列化数据</returns>
        private void serialize<valueType>(valueType value)
        {
            MemberMap = binarySerializerConfig.MemberMap;
            streamStartIndex = Stream.OffsetLength;
            Stream.Write(binarySerializerConfig.HeaderValue);
            typeSerializer<valueType>.Serialize(this, value);
            Stream.Write(Stream.OffsetLength - streamStartIndex);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private new void free()
        {
            base.free();
            typePool<indexSerializer>.PushNotNull(this);
        }
        /// <summary>
        /// 逻辑值序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(bool value)
        {
            Stream.UnsafeWrite(value ? (int)1 : 0);
        }
        /// <summary>
        /// 逻辑值序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(bool[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else Serialize(Stream, value);
        }
        /// <summary>
        /// 逻辑值序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        private void serializeBoolArray(bool[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 逻辑值序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        [memberSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(bool[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serializeBoolArray(value);
        }
        /// <summary>
        /// 逻辑值序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(bool[] value)
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else serializeBoolArray(value);
        }
        /// <summary>
        /// 逻辑值序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(bool? value)
        {
            if (value.HasValue) Stream.UnsafeWrite((bool)value ? 1 : 0);
            else *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
        }
        /// <summary>
        /// 逻辑值序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(bool?[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else Serialize(Stream, value);
        }
        /// <summary>
        /// 逻辑值序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        private void serializeBoolArray(bool?[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 逻辑值序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        [memberSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(bool?[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serializeBoolArray(value);
        }
        /// <summary>
        /// 逻辑值序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(bool?[] value)
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else serializeBoolArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(byte value)
        {
            Stream.UnsafeWrite((uint)value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(byte[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void serializeByteArray(byte[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(byte[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serializeByteArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(byte[] value)
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else serializeByteArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        private void memberSerialize(subArray<byte> value)
        {
            if (value.length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, ref value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(byte? value)
        {
            if (value.HasValue) Stream.UnsafeWrite((uint)(byte)value);
            else *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(byte?[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void serializeByteArray(byte?[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(byte?[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serializeByteArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(byte?[] value)
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else serializeByteArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(sbyte value)
        {
            Stream.UnsafeWrite((int)value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(sbyte[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void serializeSByteArray(sbyte[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(sbyte[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serializeSByteArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(sbyte[] value)
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else serializeSByteArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(sbyte? value)
        {
            if (value.HasValue) Stream.UnsafeWrite((int)(sbyte)value);
            else *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(sbyte?[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void serializeSByteArray(sbyte?[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(sbyte?[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serializeSByteArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(sbyte?[] value)
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else serializeSByteArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(short value)
        {
            Stream.UnsafeWrite((int)value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(short[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void serializeShortArray(short[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(short[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serializeShortArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(short[] value)
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else serializeShortArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(short? value)
        {
            if (value.HasValue) Stream.UnsafeWrite((int)(short)value);
            else *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(short?[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void serializeShortArray(short?[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(short?[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serializeShortArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(short?[] value)
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else serializeShortArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(ushort value)
        {
            Stream.UnsafeWrite((uint)value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(ushort[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void serializeUShortArray(ushort[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(ushort[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serializeUShortArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(ushort[] value)
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else serializeUShortArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(ushort? value)
        {
            if (value.HasValue) Stream.UnsafeWrite((uint)(ushort)value);
            else *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(ushort?[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void serializeUShortArray(ushort?[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(ushort?[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serializeUShortArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(ushort?[] value)
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else serializeUShortArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(int[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void serializeIntArray(int[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(int[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serializeIntArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(int[] value)
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else serializeIntArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(int? value)
        {
            if (value.HasValue) Stream.UnsafeWrite((int)value);
            else *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(int?[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void serializeIntArray(int?[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(int?[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serializeIntArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(int?[] value)
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else serializeIntArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(uint[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void serializeUIntArray(uint[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(uint[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serializeUIntArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(uint[] value)
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else serializeUIntArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(uint? value)
        {
            if (value.HasValue) Stream.UnsafeWrite((uint)value);
            else *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(uint?[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void serializeUIntArray(uint?[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(uint?[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serializeUIntArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(uint?[] value)
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else serializeUIntArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(long[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void serializeLongArray(long[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(long[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serializeLongArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(long[] value)
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else serializeLongArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(long? value)
        {
            if (value.HasValue) Stream.UnsafeWrite((long)value);
            else *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(long?[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void serializeLongArray(long?[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(long?[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serializeLongArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(long?[] value)
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else serializeLongArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(ulong[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void serializeULongArray(ulong[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(ulong[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serializeULongArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(ulong[] value)
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else serializeULongArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(ulong? value)
        {
            if (value.HasValue) Stream.UnsafeWrite((ulong)value);
            else *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(ulong?[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void serializeULongArray(ulong?[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(ulong?[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serializeULongArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(ulong?[] value)
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else serializeULongArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(float[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void serializeFloatArray(float[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(float[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serializeFloatArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(float[] value)
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else serializeFloatArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(float? value)
        {
            if (value.HasValue) Stream.UnsafeWrite((float)value);
            else *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(float?[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void serializeFloatArray(float?[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(float?[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serializeFloatArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(float?[] value)
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else serializeFloatArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(double[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void serializeDoubleArray(double[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(double[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serializeDoubleArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(double[] value)
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else serializeDoubleArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(double? value)
        {
            if (value.HasValue) Stream.UnsafeWrite((double)value);
            else *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(double?[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void serializeDoubleArray(double?[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(double?[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serializeDoubleArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(double?[] value)
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else serializeDoubleArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(decimal[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void serializeDecimalArray(decimal[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(decimal[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serializeDecimalArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(decimal[] value)
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else serializeDecimalArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(decimal? value)
        {
            if (value.HasValue) Stream.UnsafeWrite((decimal)value);
            else *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(decimal?[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else Serialize(Stream, value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void serializeDecimalArray(decimal?[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(decimal?[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serializeDecimalArray(value);
        }
        /// <summary>
        /// 数值序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(decimal?[] value)
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else serializeDecimalArray(value);
        }
        /// <summary>
        /// 字符序列化
        /// </summary>
        /// <param name="value">字符</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(char value)
        {
            Stream.UnsafeWrite((uint)value);
        }
        /// <summary>
        /// 字符序列化
        /// </summary>
        /// <param name="value">字符</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(char[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else Serialize(Stream, value);
        }
        /// <summary>
        /// 字符序列化
        /// </summary>
        /// <param name="value">字符</param>
        private void serializeCharArray(char[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 字符序列化
        /// </summary>
        /// <param name="value">字符</param>
        [memberSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(char[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serializeCharArray(value);
        }
        /// <summary>
        /// 字符序列化
        /// </summary>
        /// <param name="value">字符</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(char[] value)
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else serializeCharArray(value);
        }
        /// <summary>
        /// 字符序列化
        /// </summary>
        /// <param name="value">字符</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(char? value)
        {
            if (value.HasValue) Stream.UnsafeWrite((uint)(char)value);
            else *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
        }
        /// <summary>
        /// 字符序列化
        /// </summary>
        /// <param name="value">字符</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(char?[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else Serialize(Stream, value);
        }
        /// <summary>
        /// 字符序列化
        /// </summary>
        /// <param name="value">字符</param>
        private void serializeCharArray(char?[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 字符序列化
        /// </summary>
        /// <param name="value">字符</param>
        [memberSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(char?[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serializeCharArray(value);
        }
        /// <summary>
        /// 字符序列化
        /// </summary>
        /// <param name="value">字符</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(char?[] value)
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else serializeCharArray(value);
        }
        /// <summary>
        /// 时间序列化
        /// </summary>
        /// <param name="value">时间</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(DateTime[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else Serialize(Stream, value);
        }
        /// <summary>
        /// 时间序列化
        /// </summary>
        /// <param name="value">时间</param>
        private void serializeDateTimeArray(DateTime[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 时间序列化
        /// </summary>
        /// <param name="value">时间</param>
        [memberSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(DateTime[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serializeDateTimeArray(value);
        }
        /// <summary>
        /// 时间序列化
        /// </summary>
        /// <param name="value">时间</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(DateTime[] value)
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else serializeDateTimeArray(value);
        }
        /// <summary>
        /// 时间序列化
        /// </summary>
        /// <param name="value">时间</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(DateTime? value)
        {
            if (value.HasValue) Stream.UnsafeWrite((DateTime)value);
            else *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
        }
        /// <summary>
        /// 时间序列化
        /// </summary>
        /// <param name="value">时间</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(DateTime?[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else Serialize(Stream, value);
        }
        /// <summary>
        /// 时间序列化
        /// </summary>
        /// <param name="value">时间</param>
        private void serializeDateTimeArray(DateTime?[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 时间序列化
        /// </summary>
        /// <param name="value">时间</param>
        [memberSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(DateTime?[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serializeDateTimeArray(value);
        }
        /// <summary>
        /// 时间序列化
        /// </summary>
        /// <param name="value">时间</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(DateTime?[] value)
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else serializeDateTimeArray(value);
        }
        /// <summary>
        /// Guid序列化
        /// </summary>
        /// <param name="value">Guid</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(Guid[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else Serialize(Stream, value);
        }
        /// <summary>
        /// Guid序列化
        /// </summary>
        /// <param name="value">Guid</param>
        private void serializeGuidArray(Guid[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// Guid序列化
        /// </summary>
        /// <param name="value">Guid</param>
        [memberSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(Guid[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serializeGuidArray(value);
        }
        /// <summary>
        /// Guid序列化
        /// </summary>
        /// <param name="value">Guid</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(Guid[] value)
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else serializeGuidArray(value);
        }
        /// <summary>
        /// Guid序列化
        /// </summary>
        /// <param name="value">Guid</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(Guid? value)
        {
            if (value.HasValue) Stream.UnsafeWrite((Guid)value);
            else *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
        }
        /// <summary>
        /// Guid序列化
        /// </summary>
        /// <param name="value">Guid</param>
        [serializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void serialize(Guid?[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else Serialize(Stream, value);
        }
        /// <summary>
        /// Guid序列化
        /// </summary>
        /// <param name="value">Guid</param>
        private void serializeGuidArray(Guid?[] value)
        {
            if (value.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// Guid序列化
        /// </summary>
        /// <param name="value">Guid</param>
        [memberSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(Guid?[] value)
        {
            if (value == null) Stream.Write(NullValue);
            else serializeGuidArray(value);
        }
        /// <summary>
        /// Guid序列化
        /// </summary>
        /// <param name="value">Guid</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(Guid?[] value)
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else serializeGuidArray(value);
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
            else Serialize(Stream, value);
        }
        /// <summary>
        /// 字符串序列化
        /// </summary>
        /// <param name="value">字符串</param>
        private void serializeString(string value)
        {
            if (value.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 字符串序列化
        /// </summary>
        /// <param name="value">字符串</param>
        [memberSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(string value)
        {
            if (value == null) Stream.Write(NullValue);
            else serializeString(value);
        }
        /// <summary>
        /// 字符串序列化
        /// </summary>
        /// <param name="value">字符串</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(string value)
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else serializeString(value);
        }
        /// <summary>
        /// 字符串序列化
        /// </summary>
        /// <param name="value">字符串</param>
        [memberSerializeMethod]
        [memberMapSerializeMethod]
        private void memberSerialize(subString value)
        {
            if (value.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Serialize(Stream, ref value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 字符串序列化
        /// </summary>
        /// <param name="array">字符串数组</param>
        [serializeMethod]
        private void serialize(string[] array)
        {
            if (array.Length == 0) Stream.Write(0);
            else
            {
                arrayMap arrayMap = new arrayMap(Stream, array.Length, array.Length);
                foreach (string value in array) arrayMap.Next(value != null);
                arrayMap.End(Stream);
                foreach (string value in array)
                {
                    if (value != null)
                    {
                        if (value.Length == 0) Stream.Write(0);
                        else Serialize(Stream, value);
                    }
                }
            }
        }
        /// <summary>
        /// 字符串序列化
        /// </summary>
        /// <param name="array">字符串数组</param>
        private void serializeStringArray(string[] array)
        {
            if (array.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));

                arrayMap arrayMap = new arrayMap(Stream, array.Length, array.Length);
                foreach (string value in array) arrayMap.Next(value != null);
                arrayMap.End(Stream);
                foreach (string value in array)
                {
                    if (value != null)
                    {
                        if (value.Length == 0) Stream.Write(0);
                        else Serialize(Stream, value);
                    }
                }

                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
        /// <summary>
        /// 字符串序列化
        /// </summary>
        /// <param name="array">字符串数组</param>
        [memberSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberSerialize(string[] array)
        {
            if (array == null) Stream.Write(NullValue);
            else serializeStringArray(array);
        }
        /// <summary>
        /// 字符串序列化
        /// </summary>
        /// <param name="array">字符串数组</param>
        [memberMapSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapSerialize(string[] array)
        {
            if (array == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else serializeStringArray(array);
        }

        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static void nullableSerialize<valueType>(indexSerializer serializer, Nullable<valueType> value) where valueType : struct
        {
            typeSerializer<valueType>.Serialize(serializer, value.Value);
        }
        /// <summary>
        /// 对象序列化函数信息
        /// </summary>
        private static readonly MethodInfo nullableSerializeMethod = typeof(indexSerializer).GetMethod("nullableSerialize", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        private void structSerialize<valueType>(valueType value)
        {
            int length = Stream.Length;
            Stream.PrepLength(sizeof(int) * 2);
            Stream.UnsafeAddLength(sizeof(int));
            typeSerializer<valueType>.Serialize(this, value);
            *(int*)(Stream.data.Byte + length) = Stream.Length - length;
            Stream.PrepLength();
        }
        /// <summary>
        /// 对象序列化函数信息
        /// </summary>
        private static readonly MethodInfo structSerializeMethod = typeof(indexSerializer).GetMethod("structSerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void nullableMemberSerialize<valueType>(Nullable<valueType> value) where valueType : struct
        {
            if (value.HasValue) structSerialize(value.Value);
            else Stream.Write(NullValue);
        }
        /// <summary>
        /// 对象序列化函数信息
        /// </summary>
        private static readonly MethodInfo nullableMemberSerializeMethod = typeof(indexSerializer).GetMethod("nullableMemberSerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void classSerialize<valueType>(valueType value) where valueType : class
        {
            if (value == null) Stream.Write(NullValue);
            else structSerialize(value);
        }
        /// <summary>
        /// 对象序列化函数信息
        /// </summary>
        private static readonly MethodInfo classSerializeMethod = typeof(indexSerializer).GetMethod("classSerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapClassSerialize<valueType>(valueType value) where valueType : class
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else structSerialize(value);
        }
        /// <summary>
        /// 对象序列化函数信息
        /// </summary>
        private static readonly MethodInfo memberMapClassSerializeMethod = typeof(indexSerializer).GetMethod("memberMapClassSerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapNullableSerialize<valueType>(Nullable<valueType> value) where valueType : struct
        {
            if (value.HasValue)  structSerialize(value.Value);
            else *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
        }
        /// <summary>
        /// 对象序列化函数信息
        /// </summary>
        private static readonly MethodInfo memberMapNullableSerializeMethod = typeof(indexSerializer).GetMethod("memberMapNullableSerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static void keyValuePairSerialize<keyType, valueType>(indexSerializer serializer, KeyValuePair<keyType, valueType> value)
        {
            typeSerializer<keyValue<keyType, valueType>>.Serialize(serializer, new keyValue<keyType, valueType>(value.Key, value.Value));
        }
        /// <summary>
        /// 对象序列化函数信息
        /// </summary>
        private static readonly MethodInfo keyValuePairSerializeMethod = typeof(indexSerializer).GetMethod("keyValuePairSerialize", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberKeyValuePairSerialize<keyType, valueType>(KeyValuePair<keyType, valueType> value)
        {
            structSerialize(new keyValue<keyType, valueType>(value.Key, value.Value));
        }
        /// <summary>
        /// 对象序列化函数信息
        /// </summary>
        private static readonly MethodInfo memberKeyValuePairSerializeMethod = typeof(indexSerializer).GetMethod("memberKeyValuePairSerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 字典序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        private void dictionarySerializeType<dictionaryType, keyType, valueType>(dictionaryType value) where dictionaryType : IDictionary<keyType, valueType>
        {
            int index = 0;
            keyType[] keys = new keyType[value.Count];
            valueType[] values = new valueType[keys.Length];
            foreach (KeyValuePair<keyType, valueType> keyValue in value)
            {
                keys[index] = keyValue.Key;
                values[index++] = keyValue.Value;
            }
            typeSerializer<keyType[]>.DefaultSerializer(this, keys);
            typeSerializer<valueType[]>.DefaultSerializer(this, values);
        }
        /// <summary>
        /// 字典序列化函数信息
        /// </summary>
        private static readonly MethodInfo dictionarySerializeMethod = typeof(indexSerializer).GetMethod("dictionarySerializeType", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 字典序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        private void dictionarySerialize<dictionaryType, keyType, valueType>(dictionaryType value) where dictionaryType : IDictionary<keyType, valueType>
        {
            int length = Stream.Length;
            Stream.PrepLength(sizeof(int) * 2);
            Stream.UnsafeAddLength(sizeof(int));
            dictionarySerializeType<dictionaryType, keyType, valueType>(value);
            *(int*)(Stream.data.Byte + length) = Stream.Length - length;
            Stream.PrepLength();
        }
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
        private static readonly MethodInfo dictionaryMemberMethod = typeof(indexSerializer).GetMethod("dictionaryMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 字典序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDictionary<dictionaryType, keyType, valueType>(dictionaryType value) where dictionaryType : IDictionary<keyType, valueType>
        {
            if (value == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else dictionarySerialize<dictionaryType, keyType, valueType>(value);
        }
        /// <summary>
        /// 字典序列化函数信息
        /// </summary>
        private static readonly MethodInfo memberMapDictionaryMethod = typeof(indexSerializer).GetMethod("memberMapDictionary", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void subArraySerialize<valueType>(subArray<valueType> value)
        {
            valueType[] array = value.ToArray();
            typeSerializer<valueType[]>.DefaultSerializer(this, array);
        }
        /// <summary>
        /// 对象序列化函数信息
        /// </summary>
        private static readonly MethodInfo subArraySerializeMethod = typeof(indexSerializer).GetMethod("subArraySerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="serializer">二进制数据序列化</param>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static void baseSerialize<valueType, childType>(indexSerializer serializer, childType value) where childType : valueType
        {
            typeSerializer<valueType>.BaseSerialize(serializer, value);
        }
        /// <summary>
        /// 对象序列化函数信息
        /// </summary>
        private static readonly MethodInfo baseSerializeMethod = typeof(indexSerializer).GetMethod("baseSerialize", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        private void structArrayType<valueType>(valueType[] array)
        {
            if (array.Length == 0) Stream.Write(0);
            else
            {
                Stream.Write(array.Length);
                foreach (valueType value in array) typeSerializer<valueType>.Serialize(this, value);
            }
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo structArrayTypeMethod = typeof(indexSerializer).GetMethod("structArrayType", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        private void structArray<valueType>(valueType[] array)
        {
            if (array.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));
                Stream.UnsafeWrite(array.Length);
                foreach (valueType value in array) typeSerializer<valueType>.Serialize(this, value);
                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
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
        private static readonly MethodInfo structArrayMemberMethod = typeof(indexSerializer).GetMethod("structArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void structArrayMemberMap<valueType>(valueType[] array)
        {
            if (array == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else structArray(array);
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo structArrayMemberMapMethod = typeof(indexSerializer).GetMethod("structArrayMemberMap", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        private void nullableArrayType<valueType>(Nullable<valueType>[] array) where valueType : struct
        {
            if (array.Length == 0) Stream.Write(0);
            else
            {
                arrayMap arrayMap = new arrayMap(Stream, array.Length);
                foreach (Nullable<valueType> value in array) arrayMap.Next(value.HasValue);
                arrayMap.End(Stream);

                foreach (Nullable<valueType> value in array)
                {
                    if (value.HasValue) typeSerializer<valueType>.Serialize(this, value.Value);
                }
            }
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo nullableArrayTypeMethod = typeof(indexSerializer).GetMethod("nullableArrayType", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        private void nullableArray<valueType>(Nullable<valueType>[] array) where valueType : struct
        {
            if (array.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));

                arrayMap arrayMap = new arrayMap(Stream, array.Length);
                foreach (Nullable<valueType> value in array) arrayMap.Next(value.HasValue);
                arrayMap.End(Stream);

                foreach (Nullable<valueType> value in array)
                {
                    if (value.HasValue) typeSerializer<valueType>.Serialize(this, value.Value);
                }

                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
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
        private static readonly MethodInfo nullableArrayMemberMethod = typeof(indexSerializer).GetMethod("nullableArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void nullableArrayMemberMap<valueType>(Nullable<valueType>[] array) where valueType : struct
        {
            if (array == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else nullableArray(array);
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo nullableArrayMemberMapMethod = typeof(indexSerializer).GetMethod("nullableArrayMemberMap", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        private void arrayType<valueType>(valueType[] array) where valueType : class
        {
            if (array.Length == 0) Stream.Write(0);
            else
            {
                arrayMap arrayMap = new arrayMap(Stream, array.Length);
                foreach (valueType value in array) arrayMap.Next(value != null);
                arrayMap.End(Stream);

                foreach (valueType value in array)
                {
                    if (value != null) typeSerializer<valueType>.Serialize(this, value);
                }
            }
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo arrayTypeMethod = typeof(indexSerializer).GetMethod("arrayType", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        private void array<valueType>(valueType[] array) where valueType : class
        {
            if (array.Length == 0) Stream.Write(0);
            else
            {
                int length = Stream.Length;
                Stream.PrepLength(sizeof(int) * 2);
                Stream.UnsafeAddLength(sizeof(int));

                arrayMap arrayMap = new arrayMap(Stream, array.Length);
                foreach (valueType value in array) arrayMap.Next(value != null);
                arrayMap.End(Stream);

                foreach (valueType value in array)
                {
                    if (value != null) typeSerializer<valueType>.Serialize(this, value);
                }

                *(int*)(Stream.data.Byte + length) = Stream.Length - length;
                Stream.PrepLength();
            }
        }
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
        private static readonly MethodInfo arrayMemberMethod = typeof(indexSerializer).GetMethod("arrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void arrayMemberMap<valueType>(valueType[] array) where valueType : class
        {
            if (array == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else this.array(array);
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo arrayMemberMapMethod = typeof(indexSerializer).GetMethod("arrayMemberMap", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 集合转换
        /// </summary>
        /// <param name="collection">对象集合</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void collection<valueType, collectionType>(collectionType collection) where collectionType : ICollection<valueType>
        {
            typeSerializer<valueType[]>.DefaultSerializer(this, collection.getArray());
        }
        /// <summary>
        /// 集合序列化函数信息
        /// </summary>
        private static readonly MethodInfo collectionMethod = typeof(indexSerializer).GetMethod("collection", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 集合转换
        /// </summary>
        /// <param name="dictionary">对象集合</param>
        private void dictionary<dictionaryType, keyType, valueType>(dictionaryType dictionary) where dictionaryType : IDictionary<keyType, valueType>
        {
            keyType[] keys = new keyType[dictionary.Count];
            valueType[] values = new valueType[keys.Length];
            int index = 0;
            foreach (KeyValuePair<keyType, valueType> keyValue in dictionary)
            {
                keys[index] = keyValue.Key;
                values[index++] = keyValue.Value;
            }
            typeSerializer<keyType[]>.DefaultSerializer(this, keys);
            typeSerializer<valueType[]>.DefaultSerializer(this, values);
        }
        /// <summary>
        /// 集合序列化函数信息
        /// </summary>
        private static readonly MethodInfo dictionaryMethod = typeof(indexSerializer).GetMethod("dictionary", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举转换函数信息
        /// </summary>
        private static readonly MethodInfo enumByteMemberMethod = typeof(indexSerializer).GetMethod("enumByteMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举转换函数信息
        /// </summary>
        private static readonly MethodInfo enumSByteMemberMethod = typeof(indexSerializer).GetMethod("enumSByteMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举转换函数信息
        /// </summary>
        private static readonly MethodInfo enumShortMemberMethod = typeof(indexSerializer).GetMethod("enumShortMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举转换函数信息
        /// </summary>
        private static readonly MethodInfo enumUShortMemberMethod = typeof(indexSerializer).GetMethod("enumUShortMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举转换函数信息
        /// </summary>
        private static readonly MethodInfo enumIntMemberMethod = typeof(indexSerializer).GetMethod("enumIntMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举转换函数信息
        /// </summary>
        private static readonly MethodInfo enumUIntMemberMethod = typeof(indexSerializer).GetMethod("enumUIntMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举转换函数信息
        /// </summary>
        private static readonly MethodInfo enumLongMemberMethod = typeof(indexSerializer).GetMethod("enumLongMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举转换函数信息
        /// </summary>
        private static readonly MethodInfo enumULongMemberMethod = typeof(indexSerializer).GetMethod("enumULongMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举值序列化
        /// </summary>
        /// <param name="value">枚举值序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void enumByteMemberMap<valueType>(valueType value)
        {
            Stream.UnsafeWrite((uint)pub.enumCast<valueType, byte>.ToInt(value));
        }
        /// <summary>
        /// 枚举转换函数信息
        /// </summary>
        private static readonly MethodInfo enumByteMemberMapMethod = typeof(indexSerializer).GetMethod("enumByteMemberMap", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举值序列化
        /// </summary>
        /// <param name="value">枚举值序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void enumSByteMemberMap<valueType>(valueType value)
        {
            Stream.UnsafeWrite((int)pub.enumCast<valueType, sbyte>.ToInt(value));
        }
        /// <summary>
        /// 枚举转换函数信息
        /// </summary>
        private static readonly MethodInfo enumSByteMemberMapMethod = typeof(indexSerializer).GetMethod("enumSByteMemberMap", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举值序列化
        /// </summary>
        /// <param name="value">枚举值序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void enumShortMemberMap<valueType>(valueType value)
        {
            Stream.UnsafeWrite((int)pub.enumCast<valueType, short>.ToInt(value));
        }
        /// <summary>
        /// 枚举转换函数信息
        /// </summary>
        private static readonly MethodInfo enumShortMemberMapMethod = typeof(indexSerializer).GetMethod("enumShortMemberMap", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举值序列化
        /// </summary>
        /// <param name="value">枚举值序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void enumUShortMemberMap<valueType>(valueType value)
        {
            Stream.UnsafeWrite((uint)pub.enumCast<valueType, ushort>.ToInt(value));
        }
        /// <summary>
        /// 枚举转换函数信息
        /// </summary>
        private static readonly MethodInfo enumUShortMemberMapMethod = typeof(indexSerializer).GetMethod("enumUShortMemberMap", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumByteArrayType<valueType>(valueType[] array)
        {
            if (array.Length == 0) Stream.Write(0);
            else
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
        private static readonly MethodInfo enumByteArrayTypeMethod = typeof(indexSerializer).GetMethod("enumByteArrayType", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumByteArray<valueType>(valueType[] array)
        {
            if (array.Length == 0) Stream.Write(0);
            else
            {
                int length = (array.Length + (3 + sizeof(int) * 2)) & (int.MaxValue - 3);
                Stream.PrepLength(length);
                byte* write = Stream.CurrentData + sizeof(int);
                *(int*)write = array.Length;
                write += sizeof(int);
                foreach (valueType value in array) *write++ = pub.enumCast<valueType, byte>.ToInt(value);
                *(int*)Stream.CurrentData = length;
                Stream.UnsafeAddLength(length);
                Stream.PrepLength();
            }
        }
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
        private static readonly MethodInfo enumByteArrayMemberMethod = typeof(indexSerializer).GetMethod("enumByteArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void enumByteArrayMemberMap<valueType>(valueType[] array)
        {
            if (array == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else enumByteArray(array);
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo enumByteArrayMemberMapMethod = typeof(indexSerializer).GetMethod("enumByteArrayMemberMap", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumSByteArrayType<valueType>(valueType[] array)
        {
            if (array.Length == 0) Stream.Write(0);
            else
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
        private static readonly MethodInfo enumSByteArrayTypeMethod = typeof(indexSerializer).GetMethod("enumSByteArrayType", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumSByteArray<valueType>(valueType[] array)
        {
            if (array.Length == 0) Stream.Write(0);
            else
            {
                int length = (array.Length + (3 + sizeof(int) * 2)) & (int.MaxValue - 3);
                Stream.PrepLength(length);
                byte* write = Stream.CurrentData + sizeof(int);
                *(int*)write = array.Length;
                write += sizeof(int);
                foreach (valueType value in array) *(sbyte*)write++ = pub.enumCast<valueType, sbyte>.ToInt(value);
                *(int*)Stream.CurrentData = length;
                Stream.UnsafeAddLength(length);
                Stream.PrepLength();
            }
        }
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
        private static readonly MethodInfo enumSByteArrayMemberMethod = typeof(indexSerializer).GetMethod("enumSByteArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void enumSByteArrayMemberMap<valueType>(valueType[] array)
        {
            if (array == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else enumSByteArray(array);
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo enumSByteArrayMemberMapMethod = typeof(indexSerializer).GetMethod("enumSByteArrayMemberMap", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumShortArrayType<valueType>(valueType[] array)
        {
            if (array.Length == 0) Stream.Write(0);
            else
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
        private static readonly MethodInfo enumShortArrayTypeMethod = typeof(indexSerializer).GetMethod("enumShortArrayType", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumShortArray<valueType>(valueType[] array)
        {
            if (array.Length == 0) Stream.Write(0);
            else
            {
                int length = ((array.Length * sizeof(short)) + (3 + sizeof(int) * 2)) & (int.MaxValue - 3);
                Stream.PrepLength(length);
                byte* write = Stream.CurrentData + sizeof(int);
                *(int*)write = array.Length;
                write += sizeof(int);
                foreach (valueType value in array)
                {
                    *(short*)write = pub.enumCast<valueType, short>.ToInt(value);
                    write += sizeof(short);
                }
                *(int*)Stream.CurrentData = length;
                Stream.UnsafeAddLength(length);
                Stream.PrepLength();
            }
        }
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
        private static readonly MethodInfo enumShortArrayMemberMethod = typeof(indexSerializer).GetMethod("enumShortArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void enumShortArrayMemberMap<valueType>(valueType[] array)
        {
            if (array == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else enumShortArray(array);
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo enumShortArrayMemberMapMethod = typeof(indexSerializer).GetMethod("enumShortArrayMemberMap", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumUShortArrayType<valueType>(valueType[] array)
        {
            if (array.Length == 0) Stream.Write(0);
            else
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
        private static readonly MethodInfo enumUShortArrayTypeMethod = typeof(indexSerializer).GetMethod("enumUShortArrayType", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumUShortArray<valueType>(valueType[] array)
        {
            if (array.Length == 0) Stream.Write(0);
            else
            {
                int length = ((array.Length * sizeof(ushort)) + (3 + sizeof(int) * 2)) & (int.MaxValue - 3);
                Stream.PrepLength(length);
                byte* write = Stream.CurrentData + sizeof(int);
                *(int*)write = array.Length;
                write += sizeof(int);
                foreach (valueType value in array)
                {
                    *(ushort*)write = pub.enumCast<valueType, ushort>.ToInt(value);
                    write += sizeof(ushort);
                }
                *(int*)Stream.CurrentData = length;
                Stream.UnsafeAddLength(length);
                Stream.PrepLength();
            }
        }
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
        private static readonly MethodInfo enumUShortArrayMemberMethod = typeof(indexSerializer).GetMethod("enumUShortArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void enumUShortArrayMemberMap<valueType>(valueType[] array)
        {
            if (array == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else enumUShortArray(array);
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo enumUShortArrayMemberMapMethod = typeof(indexSerializer).GetMethod("enumUShortArrayMemberMap", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumIntArrayType<valueType>(valueType[] array)
        {
            if (array.Length == 0) Stream.Write(0);
            else
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
        private static readonly MethodInfo enumIntArrayTypeMethod = typeof(indexSerializer).GetMethod("enumIntArrayType", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumIntArray<valueType>(valueType[] array)
        {
            if (array.Length == 0) Stream.Write(0);
            else
            {
                int length = (array.Length + 2) * sizeof(int);
                Stream.PrepLength(length);
                byte* write = Stream.CurrentData + sizeof(int);
                *(int*)write = array.Length;
                foreach (valueType value in array) *(int*)(write += sizeof(int)) = pub.enumCast<valueType, int>.ToInt(value);
                *(int*)Stream.CurrentData = length;
                Stream.UnsafeAddLength(length);
                Stream.PrepLength();
            }
        }
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
        private static readonly MethodInfo enumIntArrayMemberMethod = typeof(indexSerializer).GetMethod("enumIntArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void enumIntArrayMemberMap<valueType>(valueType[] array)
        {
            if (array == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else enumIntArray(array);
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo enumIntArrayMemberMapMethod = typeof(indexSerializer).GetMethod("enumIntArrayMemberMap", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumUIntArrayType<valueType>(valueType[] array)
        {
            if (array.Length == 0) Stream.Write(0);
            else
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
        private static readonly MethodInfo enumUIntArrayTypeMethod = typeof(indexSerializer).GetMethod("enumUIntArrayType", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumUIntArray<valueType>(valueType[] array)
        {
            if (array.Length == 0) Stream.Write(0);
            else
            {
                int length = (array.Length + 2) * sizeof(uint);
                Stream.PrepLength(length);
                byte* write = Stream.CurrentData + sizeof(int);
                *(int*)write = array.Length;
                foreach (valueType value in array) *(uint*)(write += sizeof(uint)) = pub.enumCast<valueType, uint>.ToInt(value);
                *(int*)Stream.CurrentData = length;
                Stream.UnsafeAddLength(length);
                Stream.PrepLength();
            }
        }
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
        private static readonly MethodInfo enumUIntArrayMemberMethod = typeof(indexSerializer).GetMethod("enumUIntArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void enumUIntArrayMemberMap<valueType>(valueType[] array)
        {
            if (array == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else enumUIntArray(array);
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo enumUIntArrayMemberMapMethod = typeof(indexSerializer).GetMethod("enumUIntArrayMemberMap", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumLongArrayType<valueType>(valueType[] array)
        {
            if (array.Length == 0) Stream.Write(0);
            else
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
        private static readonly MethodInfo enumLongArrayTypeMethod = typeof(indexSerializer).GetMethod("enumLongArrayType", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumLongArray<valueType>(valueType[] array)
        {
            if (array.Length == 0) Stream.Write(0);
            else
            {
                int length = array.Length * sizeof(long) + sizeof(int) * 2;
                Stream.PrepLength(length);
                byte* write = Stream.CurrentData + sizeof(int);
                *(int*)write = array.Length;
                write += sizeof(int);
                foreach (valueType value in array)
                {
                    *(long*)write = pub.enumCast<valueType, long>.ToInt(value);
                    write += sizeof(long);
                }
                *(int*)Stream.CurrentData = length;
                Stream.UnsafeAddLength(length);
                Stream.PrepLength();
            }
        }
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
        private static readonly MethodInfo enumLongArrayMemberMethod = typeof(indexSerializer).GetMethod("enumLongArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void enumLongArrayMemberMap<valueType>(valueType[] array)
        {
            if (array == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else enumLongArray(array);
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo enumLongArrayMemberMapMethod = typeof(indexSerializer).GetMethod("enumLongArrayMemberMap", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumULongArrayType<valueType>(valueType[] array)
        {
            if (array.Length == 0) Stream.Write(0);
            else
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
        private static readonly MethodInfo enumULongArrayTypeMethod = typeof(indexSerializer).GetMethod("enumULongArrayType", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumULongArray<valueType>(valueType[] array)
        {
            if (array.Length == 0) Stream.Write(0);
            else
            {
                int length = array.Length * sizeof(ulong) + sizeof(int) * 2;
                Stream.PrepLength(length);
                byte* write = Stream.CurrentData + sizeof(int);
                *(int*)write = array.Length;
                write += sizeof(int);
                foreach (valueType value in array)
                {
                    *(ulong*)write = pub.enumCast<valueType, ulong>.ToInt(value);
                    write += sizeof(ulong);
                }
                *(int*)Stream.CurrentData = length;
                Stream.UnsafeAddLength(length);
                Stream.PrepLength();
            }
        }
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
        private static readonly MethodInfo enumULongArrayMemberMethod = typeof(indexSerializer).GetMethod("enumULongArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void enumULongArrayMemberMap<valueType>(valueType[] array)
        {
            if (array == null) *(uint*)(Stream.CurrentData - sizeof(int)) |= 0x80000000U;
            else enumULongArray(array);
        }
        /// <summary>
        /// 数组转换函数信息
        /// </summary>
        private static readonly MethodInfo enumULongArrayMemberMapMethod = typeof(indexSerializer).GetMethod("enumULongArrayMemberMap", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// 公共默认配置参数
        /// </summary>
        private static readonly config defaultConfig = new config { };
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
            indexSerializer serializer = typePool<indexSerializer>.Pop() ?? new indexSerializer();
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
        public static void Serialize<valueType>(valueType value, unmanagedStream stream, config config = null)
        {
            if (stream == null || value == null) log.Default.Throw(log.exceptionType.Null);
            else
            {
                indexSerializer serializer = typePool<indexSerializer>.Pop() ?? new indexSerializer();
                try
                {
                    serializer.serialize<valueType>(value, stream, config ?? defaultConfig);
                }
                finally { serializer.free(); }
            }
        }

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
        static indexSerializer()
        {
            serializeMethods = fastCSharp.dictionary.CreateOnly<Type, MethodInfo>();
            memberSerializeMethods = fastCSharp.dictionary.CreateOnly<Type, MethodInfo>();
            memberMapSerializeMethods = fastCSharp.dictionary.CreateOnly<Type, MethodInfo>();
            foreach (MethodInfo method in typeof(indexSerializer).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
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
