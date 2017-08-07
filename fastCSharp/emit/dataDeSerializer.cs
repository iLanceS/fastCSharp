using System;
using System.Collections.Generic;
using fastCSharp.code;
using fastCSharp.threading;
using fastCSharp.reflection;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
#if NOJIT
#else
using System.Reflection.Emit;
#endif

namespace fastCSharp.emit
{
    /// <summary>
    /// 二进制数据反序列化
    /// </summary>
    public unsafe sealed class dataDeSerializer : binaryDeSerializer
    {
        /// <summary>
        /// 基本类型反序列化函数
        /// </summary>
        internal sealed class deSerializeMethod : Attribute { }
        /// <summary>
        /// 基本类型反序列化函数
        /// </summary>
        internal sealed class memberDeSerializeMethod : Attribute { }
        /// <summary>
        /// 基本类型反序列化函数
        /// </summary>
        internal sealed class memberMapDeSerializeMethod : Attribute { }
        /// <summary>
        /// 二进制数据反序列化
        /// </summary>
        internal static class staticTypeDeSerializer
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
                    dynamicMethod = new DynamicMethod("dataDeSerializer", null, new Type[] { typeof(dataDeSerializer), type.MakeByRefType() }, type, true);
                    generator = dynamicMethod.GetILGenerator();
                    isValueType = type.IsValueType;
                }
                /// <summary>
                /// 添加字段
                /// </summary>
                /// <param name="field">字段信息</param>
                public void Push(binarySerializer.fieldInfo field)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldarg_1);
                    if (!isValueType) generator.Emit(OpCodes.Ldind_Ref);
                    generator.Emit(OpCodes.Ldflda, field.Field);
                    MethodInfo method = getMemberDeSerializeMethod(field.Field.FieldType) ?? GetMemberDeSerializer(field.Field.FieldType);
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
                    dynamicMethod = new DynamicMethod("dataMemberMapDeSerializer", null, new Type[] { typeof(memberMap), typeof(dataDeSerializer), type.MakeByRefType() }, type, true);
                    generator = dynamicMethod.GetILGenerator();
                    isValueType = type.IsValueType;
                }
                /// <summary>
                /// 添加字段
                /// </summary>
                /// <param name="field">字段信息</param>
                public void Push(binarySerializer.fieldInfo field)
                {
                    Label end = generator.DefineLabel();
                    generator.memberMapIsMember(OpCodes.Ldarg_0, field.MemberIndex);
                    generator.Emit(OpCodes.Brfalse_S, end);

                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Ldarg_2);
                    if (!isValueType) generator.Emit(OpCodes.Ldind_Ref);
                    generator.Emit(OpCodes.Ldflda, field.Field);
                    MethodInfo method = getMemberMapDeSerializeMethod(field.Field.FieldType) ?? GetMemberDeSerializer(field.Field.FieldType);
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
            /// 未知类型反序列化调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> memberDeSerializers = new interlocked.dictionary<Type,MethodInfo>();
            /// <summary>
            /// 未知类型枚举反序列化委托调用函数信息
            /// </summary>
            /// <param name="type">数组类型</param>
            /// <returns>未知类型反序列化委托调用函数信息</returns>
            public static MethodInfo GetMemberDeSerializer(Type type)
            {
                MethodInfo method;
                if (memberDeSerializers.TryGetValue(type, out method)) return method;
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
                    if (enumType == typeof(uint)) method = enumUIntMethod;
                    else if (enumType == typeof(byte)) method = enumByteMemberMethod;
                    else if (enumType == typeof(ulong)) method = enumULongMethod;
                    else if (enumType == typeof(ushort)) method = enumUShortMemberMethod;
                    else if (enumType == typeof(long)) method = enumLongMethod;
                    else if (enumType == typeof(short)) method = enumShortMemberMethod;
                    else if (enumType == typeof(sbyte)) method = enumSByteMemberMethod;
                    else method = enumIntMethod;
                    method = method.MakeGenericMethod(type);
                }
                else
                {
                    if (type.IsGenericType)
                    {
                        Type genericType = type.GetGenericTypeDefinition();
                        if (genericType == typeof(Dictionary<,>))
                        {
                            method = dictionaryMemberMethod.MakeGenericMethod(type.GetGenericArguments());
                        }
                        else if (genericType == typeof(Nullable<>))
                        {
                            method = nullableMemberDeSerializeMethod.MakeGenericMethod(type.GetGenericArguments());
                        }
                        else if (genericType == typeof(KeyValuePair<,>))
                        {
                            method = keyValuePairDeSerializeMethod.MakeGenericMethod(type.GetGenericArguments());
                        }
                        else if (genericType == typeof(SortedDictionary<,>))
                        {
                            method = sortedDictionaryMemberMethod.MakeGenericMethod(type.GetGenericArguments());
                        }
                        else if (genericType == typeof(SortedList<,>))
                        {
                            method = sortedListMemberMethod.MakeGenericMethod(type.GetGenericArguments());
                        }
                    }
                    if (method == null)
                    {
                        if (typeof(fastCSharp.code.cSharp.dataSerialize.ISerialize).IsAssignableFrom(type))
                        {
                            if (type.IsValueType) method = structISerializeMethod.MakeGenericMethod(type);
                            else method = memberClassISerializeMethod.MakeGenericMethod(type);
                        }
                        else if (type.IsValueType) method = structDeSerializeMethod.MakeGenericMethod(type);
                        else method = memberClassDeSerializeMethod.MakeGenericMethod(type);
                    }
                }
                memberDeSerializers.Set(type, method);
                return method;
            }

            /// <summary>
            /// 真实类型序列化函数集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, Func<dataDeSerializer, object, object>> realDeSerializers = new interlocked.dictionary<Type,Func<dataDeSerializer,object,object>>();
            /// <summary>
            /// 获取真实类型序列化函数
            /// </summary>
            /// <param name="type">数组类型</param>
            /// <returns>真实类型序列化函数</returns>
            public static Func<dataDeSerializer, object, object> GetRealDeSerializer(Type type)
            {
                Func<dataDeSerializer, object, object> method;
                if (realDeSerializers.TryGetValue(type, out method)) return method;
                method = (Func<dataDeSerializer, object, object>)Delegate.CreateDelegate(typeof(Func<dataDeSerializer, object, object>), realTypeObjectMethod.MakeGenericMethod(type));
                realDeSerializers.Set(type, method);
                return method;
            }
        }
        /// <summary>
        /// 二进制数据反序列化
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        internal static class typeDeSerializer<valueType>
        {
            /// <summary>
            /// 二进制数据反序列化委托
            /// </summary>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value">目标数据</param>
            internal delegate void deSerialize(dataDeSerializer deSerializer, ref valueType value);
            /// <summary>
            /// 二进制数据反序列化委托
            /// </summary>
            /// <param name="memberMap">成员位图</param>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value">目标数据</param>
            private delegate void memberMapDeSerialize(memberMap memberMap, dataDeSerializer deSerializer, ref valueType value);
            /// <summary>
            /// 二进制数据序列化类型配置
            /// </summary>
            private static readonly dataSerialize attribute;
            /// <summary>
            /// 反序列化委托
            /// </summary>
            internal static readonly deSerialize DefaultDeSerializer;
            /// <summary>
            /// 固定分组成员序列化
            /// </summary>
            private static readonly deSerialize fixedMemberDeSerializer;
            /// <summary>
            /// 固定分组成员位图序列化
            /// </summary>
            private static readonly memberMapDeSerialize fixedMemberMapDeSerializer;
            /// <summary>
            /// 成员序列化
            /// </summary>
            private static readonly deSerialize memberDeSerializer;
            /// <summary>
            /// 成员位图序列化
            /// </summary>
            private static readonly memberMapDeSerialize memberMapDeSerializer;
            /// <summary>
            /// JSON混合序列化位图
            /// </summary>
            private static readonly memberMap jsonMemberMap;
            /// <summary>
            /// JSON混合序列化成员索引集合
            /// </summary>
            private static readonly int[] jsonMemberIndexs;
            /// <summary>
            /// 固定分组填充字节数
            /// </summary>
            private static readonly int fixedFillSize;
            /// <summary>
            /// 序列化成员数量
            /// </summary>
            private static readonly int memberCountVerify;
            /// <summary>
            /// 是否值类型
            /// </summary>
            private static readonly bool isValueType;
            /// <summary>
            /// 是否支持循环引用处理
            /// </summary>
            internal static readonly bool IsReferenceMember;
            /// <summary>
            /// 对象反序列化
            /// </summary>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value">数据对象</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal static void DeSerialize(dataDeSerializer deSerializer, ref valueType value)
            {
                if (isValueType) StructDeSerialize(deSerializer, ref value);
                else ClassDeSerialize(deSerializer, ref value);
            }
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value">数据对象</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal static void StructDeSerialize(dataDeSerializer deSerializer, ref valueType value)
            {
                if (DefaultDeSerializer == null) MemberDeSerialize(deSerializer, ref value);
                else DefaultDeSerializer(deSerializer, ref value);
            }
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value">数据对象</param>
            //[MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal static void ClassDeSerialize(dataDeSerializer deSerializer, ref valueType value)
            {
                if (deSerializer.checkPoint(ref value))
                    //if (!attribute.IsAttribute || deSerializer.checkPoint(ref value))
                {
                    if (deSerializer.isRealType()) realType(deSerializer, ref value);
                    else classDeSerialize(deSerializer, ref value);
                }
            }
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value">数据对象</param>
            private static void classDeSerialize(dataDeSerializer deSerializer, ref valueType value)
            {
                if (DefaultDeSerializer == null)
                {
                    deSerializer.addPoint(ref value);
                    MemberDeSerialize(deSerializer, ref value);
                }
                else DefaultDeSerializer(deSerializer, ref value);
            }
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value">数据对象</param>
            internal static void MemberDeSerialize(dataDeSerializer deSerializer, ref valueType value)
            {
                if (deSerializer.CheckMemberCount(memberCountVerify))
                {
                    fixedMemberDeSerializer(deSerializer, ref value);
                    deSerializer.Read += fixedFillSize;
                    memberDeSerializer(deSerializer, ref value);
                    if (attribute.IsJson || jsonMemberMap != null) deSerializer.parseJson(ref value);
                }
                else if (attribute.IsMemberMap)
                {
                    memberMap memberMap = deSerializer.GetMemberMap<valueType>();
                    if (memberMap != null)
                    {
                        byte* start = deSerializer.Read;
                        fixedMemberMapDeSerializer(memberMap, deSerializer, ref value);
                        deSerializer.Read += (int)(start - deSerializer.Read) & 3;
                        memberMapDeSerializer(memberMap, deSerializer, ref value);
                        if (attribute.IsJson) deSerializer.parseJson(ref value);
                        else if (jsonMemberMap != null)
                        {
                            foreach (int memberIndex in jsonMemberIndexs)
                            {
                                if (memberMap.IsMember(memberIndex))
                                {
                                    deSerializer.parseJson(ref value);
                                    return;
                                }
                            }
                        }
                    }
                }
                else deSerializer.Error(deSerializeState.MemberMap);
            }
            /// <summary>
            /// 真实类型反序列化
            /// </summary>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value"></param>
            /// <returns></returns>
            //[MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal static void RealType(dataDeSerializer deSerializer, ref valueType value)
            {
                if (isValueType) StructDeSerialize(deSerializer, ref value);
                else classDeSerialize(deSerializer, ref value);
            }
            /// <summary>
            /// 对象反序列化
            /// </summary>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value">数据对象</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal static void BaseDeSerialize<childType>(dataDeSerializer deSerializer, ref childType value) where childType : valueType
            {
                if (value == null) value = fastCSharp.emit.constructor<childType>.New();
                valueType newValue = value;
                classDeSerialize(deSerializer, ref newValue);
            }
            /// <summary>
            /// 找不到构造函数
            /// </summary>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value">数据对象</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private static void noConstructor(dataDeSerializer deSerializer, ref valueType value)
            {
                if (deSerializer.isObjectRealType) deSerializer.Error(deSerializeState.NotNull);
                else realType(deSerializer, ref value);
            }
            /// <summary>
            /// 真实类型
            /// </summary>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value">数据对象</param>
            //[MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private static void realType(dataDeSerializer deSerializer, ref valueType value)
            {
                remoteType remoteType = default(remoteType);
                typeDeSerializer<remoteType>.StructDeSerialize(deSerializer, ref remoteType);
                if (deSerializer.state == deSerializeState.Success)
                {
                    Type type = remoteType.Type;
                    if (value == null || type.IsValueType)
                    {
                        value = (valueType)staticTypeDeSerializer.GetRealDeSerializer(type)(deSerializer, value);
                    }
                    else staticTypeDeSerializer.GetRealDeSerializer(type)(deSerializer, value);
                }
            }
            /// <summary>
            /// 不支持对象转换null
            /// </summary>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value">数据对象</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private static void fromNull(dataDeSerializer deSerializer, ref valueType value)
            {
                deSerializer.checkNull();
                value = default(valueType);
            }
            /// <summary>
            /// 枚举值序列化
            /// </summary>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value">枚举值序列化</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe static void enumByte(dataDeSerializer deSerializer, ref valueType value)
            {
                deSerializer.enumByte(ref value);
            }
            /// <summary>
            /// 枚举值序列化
            /// </summary>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value">枚举值序列化</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe static void enumSByte(dataDeSerializer deSerializer, ref valueType value)
            {
                deSerializer.enumSByte(ref value);
            }
            /// <summary>
            /// 枚举值序列化
            /// </summary>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value">枚举值序列化</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe static void enumShort(dataDeSerializer deSerializer, ref valueType value)
            {
                deSerializer.enumShort(ref value);
            }
            /// <summary>
            /// 枚举值序列化
            /// </summary>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value">枚举值序列化</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe static void enumUShort(dataDeSerializer deSerializer, ref valueType value)
            {
                deSerializer.enumUShort(ref value);
            }
            /// <summary>
            /// 枚举值序列化
            /// </summary>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value">枚举值序列化</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe static void enumInt(dataDeSerializer deSerializer, ref valueType value)
            {
                deSerializer.enumInt(ref value);
            }
            /// <summary>
            /// 枚举值序列化
            /// </summary>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value">枚举值序列化</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe static void enumUInt(dataDeSerializer deSerializer, ref valueType value)
            {
                deSerializer.enumUInt(ref value);
            }
            /// <summary>
            /// 枚举值序列化
            /// </summary>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value">枚举值序列化</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe static void enumLong(dataDeSerializer deSerializer, ref valueType value)
            {
                deSerializer.enumLong(ref value);
            }
            /// <summary>
            /// 枚举值序列化
            /// </summary>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value">枚举值序列化</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe static void enumULong(dataDeSerializer deSerializer, ref valueType value)
            {
                deSerializer.enumULong(ref value);
            }
            static typeDeSerializer()
            {
                Type type = typeof(valueType), attributeType;
                MethodInfo methodInfo = dataDeSerializer.getDeSerializeMethod(type);
                attribute = type.customAttribute<dataSerialize>(out attributeType, true) ?? dataSerialize.Default;
                if (methodInfo != null)
                {
#if NOJIT
                    DefaultDeSerializer = new methodDeSerializer(methodInfo).DeSerialize;
#else
                    DynamicMethod dynamicMethod = new DynamicMethod("dataDeSerializer", typeof(void), new Type[] { typeof(dataDeSerializer), type.MakeByRefType() }, true);
                    dynamicMethod.InitLocals = true;
                    ILGenerator generator = dynamicMethod.GetILGenerator();
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Call, methodInfo);
                    generator.Emit(OpCodes.Ret);
                    DefaultDeSerializer = (deSerialize)dynamicMethod.CreateDelegate(typeof(deSerialize));
#endif
                    IsReferenceMember = false;
                    isValueType = true;
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
                                    if (enumType == typeof(uint)) methodInfo = enumUIntArrayMethod.MakeGenericMethod(elementType);
                                    else if (enumType == typeof(byte)) methodInfo = enumByteArrayMethod.MakeGenericMethod(elementType);
                                    else if (enumType == typeof(ulong)) methodInfo = enumULongArrayMethod.MakeGenericMethod(elementType);
                                    else if (enumType == typeof(ushort)) methodInfo = enumUShortArrayMethod.MakeGenericMethod(elementType);
                                    else if (enumType == typeof(long)) methodInfo = enumLongArrayMethod.MakeGenericMethod(elementType);
                                    else if (enumType == typeof(short)) methodInfo = enumShortArrayMethod.MakeGenericMethod(elementType);
                                    else if (enumType == typeof(sbyte)) methodInfo = enumSByteArrayMethod.MakeGenericMethod(elementType);
                                    else methodInfo = enumIntArrayMethod.MakeGenericMethod(elementType);
                                    IsReferenceMember = false;
                                }
                                else if (elementType.IsGenericType && elementType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                {
                                    methodInfo = nullableArrayMethod.MakeGenericMethod(elementType = elementType.GetGenericArguments()[0]);
                                    IsReferenceMember = dataSerializer.staticTypeSerializer.IsReferenceMember(elementType);
                                }
                                else
                                {
                                    methodInfo = structArrayMethod.MakeGenericMethod(elementType);
                                    IsReferenceMember = dataSerializer.staticTypeSerializer.IsReferenceMember(elementType);
                                }
                            }
                            else
                            {
                                methodInfo = arrayMethod.MakeGenericMethod(elementType);
                                IsReferenceMember = dataSerializer.staticTypeSerializer.IsReferenceMember(elementType);
                            }
                            DefaultDeSerializer = (deSerialize)Delegate.CreateDelegate(typeof(deSerialize), methodInfo);
                            return;
                        }
                    }
                    DefaultDeSerializer = fromNull;
                    IsReferenceMember = false;
                    return;
                }
                if (type.IsEnum)
                {
                    Type enumType = System.Enum.GetUnderlyingType(type);
                    if (enumType == typeof(uint)) DefaultDeSerializer = enumUInt;
                    else if (enumType == typeof(byte)) DefaultDeSerializer = enumByte;
                    else if (enumType == typeof(ulong)) DefaultDeSerializer = enumULong;
                    else if (enumType == typeof(ushort)) DefaultDeSerializer = enumUShort;
                    else if (enumType == typeof(long)) DefaultDeSerializer = enumLong;
                    else if (enumType == typeof(short)) DefaultDeSerializer = enumShort;
                    else if (enumType == typeof(sbyte)) DefaultDeSerializer = enumSByte;
                    else DefaultDeSerializer = enumInt;
                    IsReferenceMember = false;
                    isValueType = true;
                    return;
                }
                if (type.IsPointer)
                {
                    DefaultDeSerializer = fromNull;
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
                        DefaultDeSerializer = (deSerialize)Delegate.CreateDelegate(typeof(deSerialize), subArrayDeSerializeMethod.MakeGenericMethod(type.GetGenericArguments()));
                        IsReferenceMember = dataSerializer.staticTypeSerializer.IsReferenceMember(parameterTypes[0]);
                        isValueType = true;
                        return;
                    }
                    if (genericType == typeof(Dictionary<,>))
                    {
                        DefaultDeSerializer = (deSerialize)Delegate.CreateDelegate(typeof(deSerialize), dictionaryDeSerializeMethod.MakeGenericMethod(type.GetGenericArguments()));
                        IsReferenceMember = dataSerializer.staticTypeSerializer.IsReferenceMember(parameterTypes[0]) || dataSerializer.staticTypeSerializer.IsReferenceMember(parameterTypes[1]);
                        isValueType = true;
                        return;
                    }
                    if (genericType == typeof(nullValue<>))
                    {
                        DefaultDeSerializer = (deSerialize)Delegate.CreateDelegate(typeof(deSerialize), nullableDeSerializeMethod.MakeGenericMethod(type.GetGenericArguments()));
                        IsReferenceMember = dataSerializer.staticTypeSerializer.IsReferenceMember(parameterTypes[0]);
                        isValueType = true;
                        return;
                    }
                    if (genericType == typeof(KeyValuePair<,>))
                    {
                        DefaultDeSerializer = (deSerialize)Delegate.CreateDelegate(typeof(deSerialize), keyValuePairDeSerializeMethod.MakeGenericMethod(type.GetGenericArguments()));
                        IsReferenceMember = dataSerializer.staticTypeSerializer.IsReferenceMember(parameterTypes[0]) || dataSerializer.staticTypeSerializer.IsReferenceMember(parameterTypes[1]);
                        isValueType = true;
                        return;
                    }
                    if (genericType == typeof(SortedDictionary<,>))
                    {
                        DefaultDeSerializer = (deSerialize)Delegate.CreateDelegate(typeof(deSerialize), sortedDictionaryDeSerializeMethod.MakeGenericMethod(type.GetGenericArguments()));
                        IsReferenceMember = dataSerializer.staticTypeSerializer.IsReferenceMember(parameterTypes[0]) || dataSerializer.staticTypeSerializer.IsReferenceMember(parameterTypes[1]);
                        isValueType = true;
                        return;
                    }
                    if (genericType == typeof(SortedList<,>))
                    {
                        DefaultDeSerializer = (deSerialize)Delegate.CreateDelegate(typeof(deSerialize), sortedListDeSerializeMethod.MakeGenericMethod(type.GetGenericArguments()));
                        IsReferenceMember = dataSerializer.staticTypeSerializer.IsReferenceMember(parameterTypes[0]) || dataSerializer.staticTypeSerializer.IsReferenceMember(parameterTypes[1]);
                        isValueType = true;
                        return;
                    }
                }
                if ((methodInfo = dataSerializer.staticTypeSerializer.GetCustom(type, false)) != null)
                {
                    DefaultDeSerializer = (deSerialize)Delegate.CreateDelegate(typeof(deSerialize), methodInfo);
                    IsReferenceMember = attribute.IsReferenceMember;
                    isValueType = true;
                    return;
                }
                if (type.IsAbstract || type.IsInterface || constructor<valueType>.New == null)
                {
                    DefaultDeSerializer = noConstructor;
                    isValueType = IsReferenceMember = true;
                    return;
                }
                IsReferenceMember = attribute.IsReferenceMember;
                foreach (Type interfaceType in type.GetInterfaces())
                {
                    if (interfaceType.IsGenericType)
                    {
                        Type genericType = interfaceType.GetGenericTypeDefinition();
                        if (genericType == typeof(ICollection<>))
                        {
                            Type[] parameters = interfaceType.GetGenericArguments();
                            Type argumentType = parameters[0];
                            parameters[0] = argumentType.MakeArrayType();
                            ConstructorInfo constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, parameters, null);
                            if (constructorInfo != null)
                            {
                                methodInfo = (type.IsValueType ? structCollectionMethod : classCollectionMethod).MakeGenericMethod(type, argumentType);
                                break;
                            }
                            parameters[0] = typeof(IList<>).MakeGenericType(argumentType);
                            constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, parameters, null);
                            if (constructorInfo != null)
                            {
                                methodInfo = (type.IsValueType ? structCollectionMethod : classCollectionMethod).MakeGenericMethod(type, argumentType);
                                break;
                            }
                            parameters[0] = typeof(ICollection<>).MakeGenericType(argumentType);
                            constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, parameters, null);
                            if (constructorInfo != null)
                            {
                                methodInfo = (type.IsValueType ? structCollectionMethod : classCollectionMethod).MakeGenericMethod(type, argumentType);
                                break;
                            }
                            parameters[0] = typeof(IEnumerable<>).MakeGenericType(argumentType);
                            constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, parameters, null);
                            if (constructorInfo != null)
                            {
                                methodInfo = (type.IsValueType ? structCollectionMethod : classCollectionMethod).MakeGenericMethod(type, argumentType);
                                break;
                            }
                        }
                        else if (genericType == typeof(IDictionary<,>))
                        {
                            ConstructorInfo constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { interfaceType }, null);
                            if (constructorInfo != null)
                            {
                                Type[] parameters = interfaceType.GetGenericArguments();
                                methodInfo = (type.IsValueType ? structDictionaryDeSerializeMethod : classDictionaryDeSerializeMethod).MakeGenericMethod(type, parameters[0], parameters[1]);
                                break;
                            }
                        }
                    }
                }
                if (methodInfo != null)
                {
                    DefaultDeSerializer = (deSerialize)Delegate.CreateDelegate(typeof(deSerialize), methodInfo);
                    return;
                }
                if (typeof(fastCSharp.code.cSharp.dataSerialize.ISerialize).IsAssignableFrom(type))
                {
                    methodInfo = (type.IsValueType ? dataDeSerializer.structISerializeMethod : dataDeSerializer.classISerializeMethod).MakeGenericMethod(type);
                    DefaultDeSerializer = (deSerialize)Delegate.CreateDelegate(typeof(deSerialize), methodInfo);
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
                                    DefaultDeSerializer = (deSerialize)Delegate.CreateDelegate(typeof(deSerialize), methodInfo);
                                    return;
                                }
                                break;
                            }
                        }
                    }
                    binarySerializer.fields<binarySerializer.fieldInfo> fields = dataSerializer.staticTypeSerializer.GetFields(memberIndexGroup<valueType>.GetFields(attribute.MemberFilter), out memberCountVerify);
                    fixedFillSize = -fields.FixedSize & 3;

#if NOJIT
                    fixedMemberDeSerializer = new deSerializer(ref fields.FixedFields).DeSerialize;
                    if (attribute.IsMemberMap) fixedMemberMapDeSerializer = new mapDeSerializer(ref fields.FixedFields).DeSerialize;
                    memberDeSerializer = new deSerializer(ref fields.Fields).DeSerialize;
                    if (attribute.IsMemberMap) memberMapDeSerializer = new mapDeSerializer(ref fields.Fields).DeSerialize;
#else
                    staticTypeDeSerializer.memberDynamicMethod fixedDynamicMethod = new staticTypeDeSerializer.memberDynamicMethod(type);
                    staticTypeDeSerializer.memberMapDynamicMethod fixedMemberMapDynamicMethod = attribute.IsMemberMap ? new staticTypeDeSerializer.memberMapDynamicMethod(type) : default(staticTypeDeSerializer.memberMapDynamicMethod);
                    foreach (binarySerializer.fieldInfo member in fields.FixedFields)
                    {
                        fixedDynamicMethod.Push(member);
                        if (attribute.IsMemberMap) fixedMemberMapDynamicMethod.Push(member);
                    }
                    fixedMemberDeSerializer = (deSerialize)fixedDynamicMethod.Create<deSerialize>();
                    if (attribute.IsMemberMap) fixedMemberMapDeSerializer = (memberMapDeSerialize)fixedMemberMapDynamicMethod.Create<memberMapDeSerialize>();

                    staticTypeDeSerializer.memberDynamicMethod dynamicMethod = new staticTypeDeSerializer.memberDynamicMethod(type);
                    staticTypeDeSerializer.memberMapDynamicMethod memberMapDynamicMethod = attribute.IsMemberMap ? new staticTypeDeSerializer.memberMapDynamicMethod(type) : default(staticTypeDeSerializer.memberMapDynamicMethod);
                    foreach (binarySerializer.fieldInfo member in fields.Fields)
                    {
                        dynamicMethod.Push(member);
                        if (attribute.IsMemberMap) memberMapDynamicMethod.Push(member);
                    }
                    memberDeSerializer = (deSerialize)dynamicMethod.Create<deSerialize>();
                    if (attribute.IsMemberMap) memberMapDeSerializer = (memberMapDeSerialize)memberMapDynamicMethod.Create<memberMapDeSerialize>();
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
            /// 反序列化
            /// </summary>
            private sealed class methodDeSerializer
            {
                /// <summary>
                /// 解析函数信息
                /// </summary>
                private MethodInfo method;
                /// <summary>
                /// 反序列化
                /// </summary>
                /// <param name="method"></param>
                public methodDeSerializer(MethodInfo method)
                {
                    this.method = method;
                }
                /// <summary>
                /// 解析委托
                /// </summary>
                /// <param name="parser">反序列化</param>
                /// <param name="value">目标数据</param>
                public void DeSerialize(dataDeSerializer parser, ref valueType value)
                {
                    object[] parameters = new object[] { value };
                    method.Invoke(parser, parameters);
                    value = (valueType)parameters[0];
                }
            }
            /// <summary>
            /// 反序列化
            /// </summary>
            private sealed class deSerializer
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
                    public MethodInfo DeSerializeMethod;
                    /// <summary>
                    /// 设置字段信息
                    /// </summary>
                    /// <param name="field"></param>
                    public void Set(binarySerializer.fieldInfo field)
                    {
                        Field = field.Field;
                        DeSerializeMethod = getMemberDeSerializeMethod(Field.FieldType) ?? staticTypeDeSerializer.GetMemberDeSerializer(Field.FieldType);
                    }
                }
                /// <summary>
                /// 字段集合
                /// </summary>
                private field[] fields;
                /// <summary>
                /// 反序列化
                /// </summary>
                /// <param name="fields"></param>
                public deSerializer(ref subArray<binarySerializer.fieldInfo> fields)
                {
                    this.fields = new field[fields.length];
                    int index = 0;
                    foreach (binarySerializer.fieldInfo field in fields) this.fields[index++].Set(field);
                }
                /// <summary>
                /// 反序列化
                /// </summary>
                /// <param name="deSerializer"></param>
                /// <param name="value"></param>
                public void DeSerialize(dataDeSerializer deSerializer, ref valueType value)
                {
                    object[] parameters = new object[1];
                    object objectValue = value;
                    foreach (field field in fields)
                    {
                        parameters[0] = field.Field.GetValue(objectValue);
                        field.DeSerializeMethod.Invoke(deSerializer, parameters);
                        field.Field.SetValue(objectValue, parameters[0]);
                    }
                    value = (valueType)objectValue;
                }
            }
            /// <summary>
            /// 反序列化
            /// </summary>
            private sealed class mapDeSerializer
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
                    public MethodInfo DeSerializeMethod;
                    /// <summary>
                    /// 成员编号
                    /// </summary>
                    public int MemberIndex;
                    /// <summary>
                    /// 设置字段信息
                    /// </summary>
                    /// <param name="field"></param>
                    public void Set(binarySerializer.fieldInfo field)
                    {
                        Field = field.Field;
                        MemberIndex = field.MemberIndex;
                        DeSerializeMethod = getMemberMapDeSerializeMethod(Field.FieldType) ?? staticTypeDeSerializer.GetMemberDeSerializer(Field.FieldType);
                    }
                }
                /// <summary>
                /// 字段集合
                /// </summary>
                private field[] fields;
                /// <summary>
                /// 反序列化
                /// </summary>
                /// <param name="fields"></param>
                public mapDeSerializer(ref subArray<binarySerializer.fieldInfo> fields)
                {
                    this.fields = new field[fields.length];
                    int index = 0;
                    foreach (binarySerializer.fieldInfo field in fields) this.fields[index++].Set(field);
                }
                /// <summary>
                /// 反序列化
                /// </summary>
                /// <param name="memberMap"></param>
                /// <param name="deSerializer"></param>
                /// <param name="value"></param>
                public void DeSerialize(memberMap memberMap, dataDeSerializer deSerializer, ref valueType value)
                {
                    object[] parameters = null;
                    object objectValue = value;
                    foreach (field field in fields)
                    {
                        if (memberMap.IsMember(field.MemberIndex))
                        {
                            if (parameters == null) parameters = new object[1];
                            parameters[0] = field.Field.GetValue(objectValue);
                            field.DeSerializeMethod.Invoke(deSerializer, parameters);
                            field.Field.SetValue(objectValue, parameters[0]);
                        }
                    }
                    value = (valueType)objectValue;
                }
            }
#endif
        }

        /// <summary>
        /// 历史对象指针位置
        /// </summary>
        private Dictionary<int, object> points;
        /// <summary>
        /// 是否检测相同的引用成员
        /// </summary>
        private bool isReferenceMember;
        /// <summary>
        /// 是否检测引用类型对象的真实类型
        /// </summary>
        private bool isObjectRealType;
        /// <summary>
        /// 是否检测数组引用
        /// </summary>
        private bool isReferenceArray;
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        private deSerializeState deSerialize<valueType>(byte[] data, byte* start, byte* end, ref valueType value, config config)
        {
            Config = config;
            Buffer = data;
            this.start = start;
            Read = start + sizeof(int);
            this.end = end;
            if ((*start & dataSerializer.config.MemberMapValue) == 0) isMemberMap = false;
            else
            {
                isMemberMap = true;
                MemberMap = config.MemberMap;
            }
            isObjectRealType = (*start & dataSerializer.config.ObjectRealTypeValue) != 0;
            isReferenceMember = typeDeSerializer<valueType>.IsReferenceMember;
            if (points == null && isReferenceMember) points = dictionary.CreateInt<object>();
            isReferenceArray = true;
            state = deSerializeState.Success;
            typeDeSerializer<valueType>.DeSerialize(this, ref value);
            checkState();
            return config.State = state;
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        private deSerializeState codeDeSerialize<valueType>(byte[] data, byte* start, byte* end, ref valueType value, config config) where valueType : fastCSharp.code.cSharp.dataSerialize.ISerialize
        {
            Config = config;
            Buffer = data;
            this.start = start;
            Read = start + sizeof(int);
            this.end = end;
            if ((*start & dataSerializer.config.MemberMapValue) == 0) isMemberMap = false;
            else
            {
                isMemberMap = true;
                MemberMap = config.MemberMap;
            }
            isObjectRealType = (*start & dataSerializer.config.ObjectRealTypeValue) != 0;
            isReferenceMember = typeDeSerializer<valueType>.IsReferenceMember;
            if (points == null && isReferenceMember) points = dictionary.CreateInt<object>();
            if (value == null) value = fastCSharp.emit.constructor<valueType>.New();
            isReferenceArray = true;
            state = deSerializeState.Success;
            value.DeSerialize(this);
            checkState();
            return config.State = state;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private new void free()
        {
            base.free();
            if (points != null) points.Clear();
            typePool<dataDeSerializer>.PushNotNull(this);
        }
        /// <summary>
        /// 获取历史对象
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private bool checkPoint<valueType>(ref valueType value)
        {
            if (isReferenceMember && *(int*)Read < 0)
            {
                object pointValue;
                if (points.TryGetValue(*(int*)Read, out pointValue))
                {
                    value = (valueType)pointValue;
                    Read += sizeof(int);
                    return false;
                }
                if (*(int*)Read != fastCSharp.emit.dataSerializer.RealTypeValue)
                {
                    Error(deSerializeState.NoPoint);
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 添加历史对象
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="value"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void addPoint<valueType>(ref valueType value)
        {
            if (value == null) value = fastCSharp.emit.constructor<valueType>.New();
            if (isReferenceMember) points.Add((int)(start - Read), value);
        }
        /// <summary>
        /// 添加历史对象
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="value"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void AddPoint<valueType>(valueType value)
        {
            if (isReferenceMember) points.Add((int)(start - Read), value);
        }
        /// <summary>
        /// 是否真实类型处理
        /// </summary>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private bool isRealType()
        {
            if (isObjectRealType && *(int*)Read == fastCSharp.emit.dataSerializer.RealTypeValue)
            {
                Read += sizeof(int);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 判断成员索引是否有效
        /// </summary>
        /// <param name="memberIndex"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool IsMemberMap(int memberIndex)
        {
            return MemberMap.IsMember(memberIndex);
        }
        /// <summary>
        /// 创建数组
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="array"></param>
        /// <param name="length"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void createArray<valueType>(ref valueType[] array, int length)
        {
            array = new valueType[length];
            if (isReferenceArray)
            {
                if (isReferenceMember) points.Add((int)(start - Read), array);
            }
            else isReferenceArray = true;
        }
        /// <summary>
        /// 数组反序列化
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="value"></param>
        /// <returns>数组长度</returns>
        private int deSerializeArray<valueType>(ref valueType[] value)
        {
            if (isReferenceArray && !checkPoint(ref value)) return 0;
            if (*(int*)Read != 0) return *(int*)Read;
            isReferenceArray = true;
            value = nullValue<valueType>.Array;
            Read += sizeof(int);
            return 0;
        }
        /// <summary>
        /// 逻辑值反序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        [deSerializeMethod]
        private void deSerialize(ref bool[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                if ((((length + (31 + 32)) >> 5) << 2) <= (int)(end - Read))
                {
                    createArray(ref value, length);
                    Read = DeSerialize(Read + sizeof(int), value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 逻辑值反序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref bool[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerialize(ref value);
        }
        /// <summary>
        /// 逻辑值反序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        [deSerializeMethod]
        private void deSerialize(ref bool?[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                if ((((length + (31 + 32)) >> 5) << 2) <= (int)(end - Read))
                {
                    createArray(ref value, length >> 1);
                    Read = DeSerialize(Read + sizeof(int), value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 逻辑值反序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref bool?[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerialize(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [deSerializeMethod]
        private void deSerialize(ref byte[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                if (((length + (3 + sizeof(int))) & (int.MaxValue - 3)) <= (int)(end - Read))
                {
                    createArray(ref value, length);
                    Read = DeSerialize(Read + sizeof(int), value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref byte[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerialize(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [deSerializeMethod]
        private void deSerialize(ref byte?[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                if ((((length + (31 + 32)) >> 5) << 2) <= (int)(end - Read))
                {
                    createArray(ref value, length);
                    Read = DeSerialize(Read + sizeof(int), value);
                    if (Read <= end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref byte?[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerialize(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [deSerializeMethod]
        private void deSerialize(ref sbyte[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                if (((length + (3 + sizeof(int))) & (int.MaxValue - 3)) <= (int)(end - Read))
                {
                    createArray(ref value, length);
                    Read = DeSerialize(Read + sizeof(int), value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref sbyte[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerialize(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [deSerializeMethod]
        private void deSerialize(ref sbyte?[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                if ((((length + (31 + 32)) >> 5) << 2) <= (int)(end - Read))
                {
                    createArray(ref value, length);
                    Read = DeSerialize((sbyte*)Read + sizeof(int), value);
                    if (Read <= end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref sbyte?[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerialize(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [deSerializeMethod]
        private void deSerialize(ref short[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                if ((((length * sizeof(short)) + (3 + sizeof(int))) & (int.MaxValue - 3)) <= (int)(end - Read))
                {
                    createArray(ref value, length);
                    Read = DeSerialize(Read + sizeof(int), value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref short[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerialize(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [deSerializeMethod]
        private void deSerialize(ref short?[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                if ((((length + (31 + 32)) >> 5) << 2) <= (int)(end - Read))
                {
                    createArray(ref value, length);
                    Read = DeSerialize(Read + sizeof(int), value);
                    if (Read <= end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref short?[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerialize(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [deSerializeMethod]
        private void deSerialize(ref ushort[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                if (((length * sizeof(ushort) + (3 + sizeof(int))) & (int.MaxValue - 3)) <= (int)(end - Read))
                {
                    createArray(ref value, length);
                    Read = DeSerialize(Read + sizeof(int), value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref ushort[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerialize(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [deSerializeMethod]
        private void deSerialize(ref ushort?[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                if ((((length + (31 + 32)) >> 5) << 2) <= (int)(end - Read))
                {
                    createArray(ref value, length);
                    Read = DeSerialize(Read + sizeof(int), value);
                    if (Read <= end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref ushort?[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerialize(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [deSerializeMethod]
        private void deSerialize(ref int[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                if ((length + 1) * sizeof(int) <= (int)(end - Read))
                {
                    createArray(ref value, length);
                    Read = DeSerialize(Read + sizeof(int), value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref int[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerialize(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [deSerializeMethod]
        private void deSerialize(ref int?[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                if ((((length + (31 + 32)) >> 5) << 2) <= (int)(end - Read))
                {
                    createArray(ref value, length);
                    Read = DeSerialize(Read + sizeof(int), value);
                    if (Read <= end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref int?[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerialize(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [deSerializeMethod]
        private void deSerialize(ref uint[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                if ((length + 1) * sizeof(int) <= (int)(end - Read))
                {
                    createArray(ref value, length);
                    Read = DeSerialize(Read + sizeof(int), value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref uint[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerialize(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [deSerializeMethod]
        private void deSerialize(ref uint?[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                if ((((length + (31 + 32)) >> 5) << 2) <= (int)(end - Read))
                {
                    createArray(ref value, length);
                    Read = DeSerialize(Read + sizeof(int), value);
                    if (Read <= end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref uint?[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerialize(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [deSerializeMethod]
        private void deSerialize(ref long[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                if (length * sizeof(long) + sizeof(int) <= (int)(end - Read))
                {
                    createArray(ref value, length);
                    Read = DeSerialize(Read + sizeof(int), value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref long[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerialize(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [deSerializeMethod]
        private void deSerialize(ref long?[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                if ((((length + (31 + 32)) >> 5) << 2) <= (int)(end - Read))
                {
                    createArray(ref value, length);
                    Read = DeSerialize(Read + sizeof(int), value);
                    if (Read <= end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref long?[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerialize(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [deSerializeMethod]
        private void deSerialize(ref ulong[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                if (length * sizeof(ulong) + sizeof(int) <= (int)(end - Read))
                {
                    createArray(ref value, length);
                    Read = DeSerialize(Read + sizeof(int), value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref ulong[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerialize(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [deSerializeMethod]
        private void deSerialize(ref ulong?[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                if ((((length + (31 + 32)) >> 5) << 2) <= (int)(end - Read))
                {
                    createArray(ref value, length);
                    Read = DeSerialize(Read + sizeof(int), value);
                    if (Read <= end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref ulong?[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerialize(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [deSerializeMethod]
        private void deSerialize(ref float[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                if (length * sizeof(float) + sizeof(int) <= (int)(end - Read))
                {
                    createArray(ref value, length);
                    Read = DeSerialize(Read + sizeof(int), value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref float[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerialize(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [deSerializeMethod]
        private void deSerialize(ref float?[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                if ((((length + (31 + 32)) >> 5) << 2) <= (int)(end - Read))
                {
                    createArray(ref value, length);
                    Read = DeSerialize(Read + sizeof(int), value);
                    if (Read <= end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref float?[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerialize(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [deSerializeMethod]
        private void deSerialize(ref double[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                if (length * sizeof(double) + sizeof(int) <= (int)(end - Read))
                {
                    createArray(ref value, length);
                    Read = DeSerialize(Read + sizeof(int), value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref double[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerialize(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [deSerializeMethod]
        private void deSerialize(ref double?[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                if ((((length + (31 + 32)) >> 5) << 2) <= (int)(end - Read))
                {
                    createArray(ref value, length);
                    Read = DeSerialize(Read + sizeof(int), value);
                    if (Read <= end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref double?[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerialize(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [deSerializeMethod]
        private void deSerialize(ref decimal[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                if (length * sizeof(decimal) + sizeof(int) <= (int)(end - Read))
                {
                    createArray(ref value, length);
                    Read = DeSerialize(Read + sizeof(int), value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref decimal[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerialize(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [deSerializeMethod]
        private void deSerialize(ref decimal?[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                if ((((length + (31 + 32)) >> 5) << 2) <= (int)(end - Read))
                {
                    createArray(ref value, length);
                    Read = DeSerialize(Read + sizeof(int), value);
                    if (Read <= end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref decimal?[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerialize(ref value);
        }
        /// <summary>
        /// 字符反序列化
        /// </summary>
        /// <param name="value">字符</param>
        [deSerializeMethod]
        private void deSerialize(ref char[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                if (((length * sizeof(char) + (3 + sizeof(int))) & (int.MaxValue - 3)) <= (int)(end - Read))
                {
                    createArray(ref value, length);
                    Read = DeSerialize(Read + sizeof(int), value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 字符反序列化
        /// </summary>
        /// <param name="value">字符</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref char[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerialize(ref value);
        }
        /// <summary>
        /// 字符反序列化
        /// </summary>
        /// <param name="value">字符</param>
        [deSerializeMethod]
        private void deSerialize(ref char?[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                if ((((length + (31 + 32)) >> 5) << 2) <= (int)(end - Read))
                {
                    createArray(ref value, length);
                    Read = DeSerialize(Read + sizeof(int), value);
                    if (Read <= end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 字符反序列化
        /// </summary>
        /// <param name="value">字符</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref char?[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerialize(ref value);
        }
        /// <summary>
        /// 时间反序列化
        /// </summary>
        /// <param name="value">时间</param>
        [deSerializeMethod]
        private void deSerialize(ref DateTime[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                if (length * sizeof(DateTime) + sizeof(int) <= (int)(end - Read))
                {
                    createArray(ref value, length);
                    Read = DeSerialize(Read + sizeof(int), value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 时间反序列化
        /// </summary>
        /// <param name="value">时间</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref DateTime[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerialize(ref value);
        }
        /// <summary>
        /// 时间反序列化
        /// </summary>
        /// <param name="value">时间</param>
        [deSerializeMethod]
        private void deSerialize(ref DateTime?[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                if ((((length + (31 + 32)) >> 5) << 2) <= (int)(end - Read))
                {
                    createArray(ref value, length);
                    Read = DeSerialize(Read + sizeof(int), value);
                    if (Read <= end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 时间反序列化
        /// </summary>
        /// <param name="value">时间</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref DateTime?[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerialize(ref value);
        }
        /// <summary>
        /// Guid反序列化
        /// </summary>
        /// <param name="value">Guid</param>
        [deSerializeMethod]
        private void deSerialize(ref Guid[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                if (length * sizeof(Guid) + sizeof(int) <= (int)(end - Read))
                {
                    createArray(ref value, length);
                    Read = DeSerialize(Read + sizeof(int), value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// Guid反序列化
        /// </summary>
        /// <param name="value">Guid</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref Guid[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerialize(ref value);
        }
        /// <summary>
        /// Guid反序列化
        /// </summary>
        /// <param name="value">Guid</param>
        [deSerializeMethod]
        private void deSerialize(ref Guid?[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                if ((((length + (31 + 32)) >> 5) << 2) <= (int)(end - Read))
                {
                    createArray(ref value, length);
                    Read = DeSerialize(Read + sizeof(int), value);
                    if (Read <= end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// Guid反序列化
        /// </summary>
        /// <param name="value">Guid</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref Guid?[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerialize(ref value);
        }
        /// <summary>
        /// 字符串反序列化
        /// </summary>
        /// <param name="value">字符串</param>
        [deSerializeMethod]
        private void deSerialize(ref string value)
        {
            if (checkPoint(ref value))
            {
                int length = *(int*)Read;
                if ((length & 1) == 0)
                {
                    if (length != 0)
                    {
                        int dataLength = (length + (3 + sizeof(int))) & (int.MaxValue - 3);
                        if (dataLength <= (int)(end - Read))
                        {
                            value = new string((char*)(Read + sizeof(int)), 0, length >> 1);
                            if (isReferenceMember) points.Add((int)(start - Read), value);
                            Read += dataLength;
                        }
                        else Error(deSerializeState.IndexOutOfRange);
                    }
                    else
                    {
                        value = string.Empty;
                        Read += sizeof(int);
                    }
                }
                else
                {
                    int dataLength = ((length >>= 1) + (3 + sizeof(int))) & (int.MaxValue - 3);
                    if (dataLength <= (int)(end - Read))
                    {
                        value = fastCSharp.String.FastAllocateString(length);
                        if (isReferenceMember) points.Add((int)(start - Read), value);
                        fixed (char* valueFixed = value)
                        {
                            char* write = valueFixed;
                            byte* readStart = Read + sizeof(int), readEnd = readStart + length;
                            do
                            {
                                *write++ = (char)*readStart++;
                            }
                            while (readStart != readEnd);
                        }
                        Read += dataLength;
                    }
                    else Error(deSerializeState.IndexOutOfRange);
                }
            }
        }
        /// <summary>
        /// 字符串反序列化
        /// </summary>
        /// <param name="value">字符串</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref string value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerialize(ref value);
        }
        /// <summary>
        /// 字符串反序列化
        /// </summary>
        /// <param name="value">字符串</param>
        [deSerializeMethod]
        private void deSerialize(ref string[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                int mapLength = ((length + (31 + 32)) >> 5) << 2;
                if (mapLength <= (int)(end - Read))
                {
                    createArray(ref value, length);
                    arrayMap arrayMap = new arrayMap(Read + sizeof(int));
                    Read += mapLength;
                    for (int index = 0; index != value.Length; ++index)
                    {
                        if (arrayMap.Next() == 0) value[index] = null;
                        else
                        {
                            deSerialize(ref value[index]);
                            if (state != deSerializeState.Success) return;
                        }
                    }
                    if (Read <= end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 字符串反序列化
        /// </summary>
        /// <param name="value">字符串</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref string[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerialize(ref value);
        }

        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void structDeSerialize<valueType>(ref valueType value) where valueType : struct
        {
            typeDeSerializer<valueType>.StructDeSerialize(this, ref value);
        }
        /// <summary>
        /// 对象序列化函数信息
        /// </summary>
        private static readonly MethodInfo structDeSerializeMethod = typeof(dataDeSerializer).GetMethod("structDeSerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 字典序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void keyValuePairDeSerialize<keyType, valueType>(ref KeyValuePair<keyType, valueType> value)
        {
            keyValue<keyType, valueType> keyValue = default(keyValue<keyType, valueType>);
            typeDeSerializer<keyValue<keyType, valueType>>.MemberDeSerialize(this, ref keyValue);
            value = new KeyValuePair<keyType,valueType>(keyValue.Key, keyValue.Value);
        }
        /// <summary>
        /// 字典序列化函数信息
        /// </summary>
        private static readonly MethodInfo keyValuePairDeSerializeMethod = typeof(dataDeSerializer).GetMethod("keyValuePairDeSerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 字典序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        private void dictionaryArrayDeSerialize<keyType, valueType>(IDictionary<keyType, valueType> value)
        {
            if (isReferenceMember) points.Add((int)(start - Read), value);
            keyType[] keys = null;
            isReferenceArray = false;
            typeDeSerializer<keyType[]>.DefaultDeSerializer(this, ref keys);
            if (state == deSerializeState.Success)
            {
                valueType[] values = null;
                isReferenceArray = false;
                typeDeSerializer<valueType[]>.DefaultDeSerializer(this, ref values);
                if (state == deSerializeState.Success)
                {
                    int index = 0;
                    foreach (valueType nextValue in values) value.Add(keys[index++], nextValue);
                }
            }
        }
        /// <summary>
        /// 字典序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void dictionaryDeSerialize<keyType, valueType>(ref Dictionary<keyType, valueType> value)
        {
            dictionaryArrayDeSerialize(value = dictionary.CreateAny<keyType, valueType>());
        }
        /// <summary>
        /// 字典序列化函数信息
        /// </summary>
        private static readonly MethodInfo dictionaryDeSerializeMethod = typeof(dataDeSerializer).GetMethod("dictionaryDeSerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 字典序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void dictionaryMember<keyType, valueType>(ref Dictionary<keyType, valueType> value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else dictionaryDeSerialize(ref value);
        }
        /// <summary>
        /// 字典序列化函数信息
        /// </summary>
        private static readonly MethodInfo dictionaryMemberMethod = typeof(dataDeSerializer).GetMethod("dictionaryMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 字典序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void sortedDictionaryDeSerialize<keyType, valueType>(ref SortedDictionary<keyType, valueType> value)
        {
            dictionaryArrayDeSerialize(value = new SortedDictionary<keyType, valueType>());
        }
        /// <summary>
        /// 字典序列化函数信息
        /// </summary>
        private static readonly MethodInfo sortedDictionaryDeSerializeMethod = typeof(dataDeSerializer).GetMethod("sortedDictionaryDeSerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 字典序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void sortedDictionaryMember<keyType, valueType>(ref SortedDictionary<keyType, valueType> value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else sortedDictionaryDeSerialize(ref value);
        }
        /// <summary>
        /// 字典序列化函数信息
        /// </summary>
        private static readonly MethodInfo sortedDictionaryMemberMethod = typeof(dataDeSerializer).GetMethod("sortedDictionaryMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 字典序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void sortedListDeSerialize<keyType, valueType>(ref SortedList<keyType, valueType> value)
        {
            dictionaryArrayDeSerialize(value = new SortedList<keyType, valueType>());
        }
        /// <summary>
        /// 字典序列化函数信息
        /// </summary>
        private static readonly MethodInfo sortedListDeSerializeMethod = typeof(dataDeSerializer).GetMethod("sortedListDeSerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 字典序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void sortedListMember<keyType, valueType>(ref SortedList<keyType, valueType> value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else sortedListDeSerialize(ref value);
        }
        /// <summary>
        /// 字典序列化函数信息
        /// </summary>
        private static readonly MethodInfo sortedListMemberMethod = typeof(dataDeSerializer).GetMethod("sortedListMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void nullableDeSerialize<valueType>(ref Nullable<valueType> value) where valueType : struct
        {
            valueType newValue = value.HasValue ? value.Value : default(valueType);
            typeDeSerializer<valueType>.StructDeSerialize(this, ref newValue);
            value = newValue;
        }
        /// <summary>
        /// 对象序列化函数信息
        /// </summary>
        private static readonly MethodInfo nullableDeSerializeMethod = typeof(dataDeSerializer).GetMethod("nullableDeSerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组对象序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void subArrayDeSerialize<valueType>(ref subArray<valueType> value)
        {
            valueType[] array = null;
            isReferenceArray = false;
            typeDeSerializer<valueType[]>.DefaultDeSerializer(this, ref array);
            value.UnsafeSet(array, 0, array.Length);
        }
        /// <summary>
        /// 数组对象序列化函数信息
        /// </summary>
        private static readonly MethodInfo subArrayDeSerializeMethod = typeof(dataDeSerializer).GetMethod("subArrayDeSerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void nullableMemberDeSerialize<valueType>(ref Nullable<valueType> value) where valueType : struct
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else nullableDeSerialize(ref value);
        }
        /// <summary>
        /// 对象序列化函数信息
        /// </summary>
        private static readonly MethodInfo nullableMemberDeSerializeMethod = typeof(dataDeSerializer).GetMethod("nullableMemberDeSerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 基类反序列化
        /// </summary>
        /// <param name="deSerializer">二进制数据反序列化</param>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static void baseSerialize<valueType, childType>(dataDeSerializer deSerializer, ref childType value) where childType : valueType
        {
            typeDeSerializer<valueType>.BaseDeSerialize(deSerializer, ref value);
        }
        /// <summary>
        /// 基类反序列化函数信息
        /// </summary>
        private static readonly MethodInfo baseSerializeMethod = typeof(dataDeSerializer).GetMethod("baseSerialize", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 真实类型反序列化
        /// </summary>
        /// <param name="deSerializer">二进制数据反序列化</param>
        /// <param name="objectValue"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static object realTypeObject<valueType>(dataDeSerializer deSerializer, object objectValue)
        {
            valueType value = (valueType)objectValue;
            typeDeSerializer<valueType>.RealType(deSerializer, ref value);
            return value;
        }
        /// <summary>
        /// 基类反序列化函数信息
        /// </summary>
        private static readonly MethodInfo realTypeObjectMethod = typeof(dataDeSerializer).GetMethod("realTypeObject", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 集合构造函数解析
        /// </summary>
        /// <param name="value">目标数据</param>
        private void collection<valueType, argumentType>(ref valueType value) where valueType : ICollection<argumentType>
        {
            argumentType[] values = null;
            isReferenceArray = false;
            typeDeSerializer<argumentType[]>.DefaultDeSerializer(this, ref values);
            if (state == deSerializeState.Success)
            {
                foreach (argumentType nextValue in values) value.Add(nextValue);
            }
        }
        /// <summary>
        /// 集合构造函数解析
        /// </summary>
        /// <param name="value">目标数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void structCollection<valueType, argumentType>(ref valueType value) where valueType : ICollection<argumentType>
        {
            value = fastCSharp.emit.constructor<valueType>.New();
            collection<valueType, argumentType>(ref value);
        }
        /// <summary>
        /// 集合反序列化函数信息
        /// </summary>
        private static readonly MethodInfo structCollectionMethod = typeof(dataDeSerializer).GetMethod("structCollection", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 集合构造函数解析
        /// </summary>
        /// <param name="value">目标数据</param>
        //[MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void classCollection<valueType, argumentType>(ref valueType value) where valueType : ICollection<argumentType>
        {
            if (checkPoint(ref value))
            {
                value = fastCSharp.emit.constructor<valueType>.New();
                if (isReferenceMember) points.Add((int)(start - Read), value);
                collection<valueType, argumentType>(ref value);
            }
        }
        /// <summary>
        /// 集合反序列化函数信息
        /// </summary>
        private static readonly MethodInfo classCollectionMethod = typeof(dataDeSerializer).GetMethod("classCollection", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 集合构造函数解析
        /// </summary>
        /// <param name="value">目标数据</param>
        private void dictionaryConstructorDeSerialize<dictionaryType, keyType, valueType>(ref dictionaryType value) where dictionaryType : IDictionary<keyType, valueType>
        {
            keyType[] keys = null;
            isReferenceArray = false;
            typeDeSerializer<keyType[]>.DefaultDeSerializer(this, ref keys);
            if (state == deSerializeState.Success)
            {
                valueType[] values = null;
                isReferenceArray = false;
                typeDeSerializer<valueType[]>.DefaultDeSerializer(this, ref values);
                if (state == deSerializeState.Success)
                {
                    int index = 0;
                    foreach (valueType nextValue in values) value.Add(keys[index++], nextValue);
                }
            }
        }
        /// <summary>
        /// 集合构造函数解析
        /// </summary>
        /// <param name="value">目标数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void structDictionaryDeSerialize<dictionaryType, keyType, valueType>(ref dictionaryType value) where dictionaryType : IDictionary<keyType, valueType>
        {
            value = fastCSharp.emit.constructor<dictionaryType>.New();
            dictionaryConstructorDeSerialize<dictionaryType, keyType, valueType>(ref value);
        }
        /// <summary>
        /// 集合反序列化函数信息
        /// </summary>
        private static readonly MethodInfo structDictionaryDeSerializeMethod = typeof(dataDeSerializer).GetMethod("structDictionaryDeSerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 集合构造函数解析
        /// </summary>
        /// <param name="value">目标数据</param>
        //[MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void classDictionaryDeSerialize<dictionaryType, keyType, valueType>(ref dictionaryType value) where dictionaryType : IDictionary<keyType, valueType>
        {
            if (checkPoint(ref value))
            {
                value = fastCSharp.emit.constructor<dictionaryType>.New();
                if (isReferenceMember) points.Add((int)(start - Read), value);
                dictionaryConstructorDeSerialize<dictionaryType, keyType, valueType>(ref value);
            }
        }
        /// <summary>
        /// 集合反序列化函数信息
        /// </summary>
        private static readonly MethodInfo classDictionaryDeSerializeMethod = typeof(dataDeSerializer).GetMethod("classDictionaryDeSerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumByteMemberMethod = typeof(dataDeSerializer).GetMethod("enumByteMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumSByteMemberMethod = typeof(dataDeSerializer).GetMethod("enumSByteMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumShortMemberMethod = typeof(dataDeSerializer).GetMethod("enumShortMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumUShortMemberMethod = typeof(dataDeSerializer).GetMethod("enumUShortMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumIntMethod = typeof(dataDeSerializer).GetMethod("enumInt", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumUIntMethod = typeof(dataDeSerializer).GetMethod("enumUInt", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumLongMethod = typeof(dataDeSerializer).GetMethod("enumLong", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumULongMethod = typeof(dataDeSerializer).GetMethod("enumULong", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumByteArray<valueType>(ref valueType[] array)
        {
            int length = deSerializeArray(ref array);
            if (length != 0)
            {
                int dataLength = (length + (3 + sizeof(int))) & (int.MaxValue - 3);
                if (dataLength <= (int)(end - Read))
                {
                    createArray(ref array, length);
                    byte* data = Read + sizeof(int);
                    for (int index = 0; index != array.Length; array[index++] = pub.enumCast<valueType, byte>.FromInt(*data++)) ;
                    Read += dataLength;
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumByteArrayMethod = typeof(dataDeSerializer).GetMethod("enumByteArray", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void enumByteArrayMember<valueType>(ref valueType[] array)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                array = null;
            }
            else enumByteArray(ref array);
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumByteArrayMemberMethod = typeof(dataDeSerializer).GetMethod("enumByteArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumSByteArray<valueType>(ref valueType[] array)
        {
            int length = deSerializeArray(ref array);
            if (length != 0)
            {
                int dataLength = (length + (3 + sizeof(int))) & (int.MaxValue - 3);
                if (dataLength <= (int)(end - Read))
                {
                    createArray(ref array, length);
                    byte* data = Read + sizeof(int);
                    for (int index = 0; index != array.Length; array[index++] = pub.enumCast<valueType, sbyte>.FromInt((sbyte)*data++)) ;
                    Read += dataLength;
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumSByteArrayMethod = typeof(dataDeSerializer).GetMethod("enumSByteArray", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void enumSByteArrayMember<valueType>(ref valueType[] array)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                array = null;
            }
            else enumSByteArray(ref array);
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumSByteArrayMemberMethod = typeof(dataDeSerializer).GetMethod("enumSByteArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumShortArray<valueType>(ref valueType[] array)
        {
            int length = deSerializeArray(ref array);
            if (length != 0)
            {
                int dataLength = ((length << 1) + (3 + sizeof(int))) & (int.MaxValue - 3);
                if (dataLength <= (int)(end - Read))
                {
                    createArray(ref array, length);
                    short* data = (short*)(Read + sizeof(int));
                    for (int index = 0; index != array.Length; array[index++] = pub.enumCast<valueType, short>.FromInt(*data++)) ;
                    Read += dataLength;
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumShortArrayMethod = typeof(dataDeSerializer).GetMethod("enumShortArray", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void enumShortArrayMember<valueType>(ref valueType[] array)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                array = null;
            }
            else enumShortArray(ref array);
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumShortArrayMemberMethod = typeof(dataDeSerializer).GetMethod("enumShortArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumUShortArray<valueType>(ref valueType[] array)
        {
            int length = deSerializeArray(ref array);
            if (length != 0)
            {
                int dataLength = ((length << 1) + (3 + sizeof(int))) & (int.MaxValue - 3);
                if (dataLength <= (int)(end - Read))
                {
                    createArray(ref array, length);
                    ushort* data = (ushort*)(Read + sizeof(int));
                    for (int index = 0; index != array.Length; array[index++] = pub.enumCast<valueType, ushort>.FromInt(*data++)) ;
                    Read += dataLength;
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumUShortArrayMethod = typeof(dataDeSerializer).GetMethod("enumUShortArray", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void enumUShortArrayMember<valueType>(ref valueType[] array)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                array = null;
            }
            else enumUShortArray(ref array);
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumUShortArrayMemberMethod = typeof(dataDeSerializer).GetMethod("enumUShortArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumIntArray<valueType>(ref valueType[] array)
        {
            int length = deSerializeArray(ref array);
            if (length != 0)
            {
                if ((length + 1) * sizeof(int) <= (int)(end - Read))
                {
                    createArray(ref array, length);
                    Read += sizeof(int);
                    for (int index = 0; index != array.Length; Read += sizeof(int)) array[index++] = pub.enumCast<valueType, int>.FromInt(*(int*)Read);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumIntArrayMethod = typeof(dataDeSerializer).GetMethod("enumIntArray", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void enumIntArrayMember<valueType>(ref valueType[] array)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                array = null;
            }
            else enumIntArray(ref array);
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumIntArrayMemberMethod = typeof(dataDeSerializer).GetMethod("enumIntArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumUIntArray<valueType>(ref valueType[] array)
        {
            int length = deSerializeArray(ref array);
            if (length != 0)
            {
                if ((length + 1) * sizeof(int) <= (int)(end - Read))
                {
                    createArray(ref array, length);
                    Read += sizeof(int);
                    for (int index = 0; index != array.Length; Read += sizeof(uint)) array[index++] = pub.enumCast<valueType, uint>.FromInt(*(uint*)Read);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumUIntArrayMethod = typeof(dataDeSerializer).GetMethod("enumUIntArray", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void enumUIntArrayMember<valueType>(ref valueType[] array)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                array = null;
            }
            else enumUIntArray(ref array);
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumUIntArrayMemberMethod = typeof(dataDeSerializer).GetMethod("enumUIntArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumLongArray<valueType>(ref valueType[] array)
        {
            int length = deSerializeArray(ref array);
            if (length != 0)
            {
                if (length * sizeof(long) + sizeof(int) <= (int)(end - Read))
                {
                    createArray(ref array, length);
                    Read += sizeof(int);
                    for (int index = 0; index != array.Length; Read += sizeof(long)) array[index++] = pub.enumCast<valueType, long>.FromInt(*(long*)Read);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumLongArrayMethod = typeof(dataDeSerializer).GetMethod("enumLongArray", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void enumLongArrayMember<valueType>(ref valueType[] array)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                array = null;
            }
            else enumLongArray(ref array);
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumLongArrayMemberMethod = typeof(dataDeSerializer).GetMethod("enumLongArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumULongArray<valueType>(ref valueType[] array)
        {
            int length = deSerializeArray(ref array);
            if (length != 0)
            {
                if (length * sizeof(ulong) + sizeof(int) <= (int)(end - Read))
                {
                    createArray(ref array, length);
                    Read += sizeof(int);
                    for (int index = 0; index != array.Length; Read += sizeof(ulong)) array[index++] = pub.enumCast<valueType, ulong>.FromInt(*(ulong*)Read);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumULongArrayMethod = typeof(dataDeSerializer).GetMethod("enumULongArray", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void enumULongArrayMember<valueType>(ref valueType[] array)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                array = null;
            }
            else enumULongArray(ref array);
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumULongArrayMemberMethod = typeof(dataDeSerializer).GetMethod("enumULongArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        private void nullableArray<valueType>(ref Nullable<valueType>[] array) where valueType : struct
        {
            int length = deSerializeArray(ref array);
            if (length != 0)
            {
                int mapLength = ((length + (31 + 32)) >> 5) << 2;
                if (mapLength <= (int)(end - Read))
                {
                    createArray(ref array, length);
                    arrayMap arrayMap = new arrayMap(Read + sizeof(int));
                    Read += mapLength;
                    for (int index = 0; index != array.Length; ++index)
                    {
                        if (arrayMap.Next() == 0) array[index] = null;
                        else
                        {
                            valueType value = default(valueType);
                            typeDeSerializer<valueType>.StructDeSerialize(this, ref value);
                            if (state != deSerializeState.Success) return;
                            array[index] = value;
                        }
                    }
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo nullableArrayMethod = typeof(dataDeSerializer).GetMethod("nullableArray", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void nullableArrayMember<valueType>(ref Nullable<valueType>[] array) where valueType : struct
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                array = null;
            }
            else nullableArray(ref array);
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo nullableArrayMemberMethod = typeof(dataDeSerializer).GetMethod("nullableArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        private void structArray<valueType>(ref valueType[] array)
        {
            int length = deSerializeArray(ref array);
            if (length != 0)
            {
                if ((length + 1) * sizeof(int) <= (int)(end - Read))
                {
                    createArray(ref array, length);
                    Read += sizeof(int);
                    for (int index = 0; index != array.Length; ++index)
                    {
                        typeDeSerializer<valueType>.StructDeSerialize(this, ref array[index]);
                        if (state != deSerializeState.Success) return;
                    }
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo structArrayMethod = typeof(dataDeSerializer).GetMethod("structArray", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void structArrayMember<valueType>(ref valueType[] array)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                array = null;
            }
            else structArray(ref array);
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo structArrayMemberMethod = typeof(dataDeSerializer).GetMethod("structArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        private void array<valueType>(ref valueType[] array) where valueType : class
        {
            int length = deSerializeArray(ref array);
            if (length != 0)
            {
                int mapLength = ((length + (31 + 32)) >> 5) << 2;
                if (mapLength <= (int)(end - Read))
                {
                    createArray(ref array, length);
                    arrayMap arrayMap = new arrayMap(Read + sizeof(int));
                    Read += mapLength;
                    for (int index = 0; index != array.Length; ++index)
                    {
                        if (arrayMap.Next() == 0) array[index] = null;
                        else
                        {
                            typeDeSerializer<valueType>.ClassDeSerialize(this, ref array[index]);
                            if (state != deSerializeState.Success) return;
                        }
                    }
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo arrayMethod = typeof(dataDeSerializer).GetMethod("array", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void arrayMember<valueType>(ref valueType[] array) where valueType : class
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                array = null;
            }
            else this.array(ref array);
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo arrayMemberMethod = typeof(dataDeSerializer).GetMethod("arrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 序列化接口
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="value"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void structISerialize<valueType>(ref valueType value) where valueType : struct, fastCSharp.code.cSharp.dataSerialize.ISerialize
        {
            value.DeSerialize(this);
        }
        /// <summary>
        /// 序列化接口函数信息
        /// </summary>
        private static readonly MethodInfo structISerializeMethod = typeof(dataDeSerializer).GetMethod("structISerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 序列化接口
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="value"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void classISerialize<valueType>(ref valueType value) where valueType : class, fastCSharp.code.cSharp.dataSerialize.ISerialize
        {
            if (value == null) value = fastCSharp.emit.constructor<valueType>.New();
            value.DeSerialize(this);
        }
        /// <summary>
        /// 序列化接口函数信息
        /// </summary>
        private static readonly MethodInfo classISerializeMethod = typeof(dataDeSerializer).GetMethod("classISerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 序列化接口
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="value"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberClassISerialize<valueType>(ref valueType value) where valueType : class, fastCSharp.code.cSharp.dataSerialize.ISerialize
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else
            {
                if (value == null) value = fastCSharp.emit.constructor<valueType>.New();
                value.DeSerialize(this);
            }
        }
        /// <summary>
        /// 序列化接口函数信息
        /// </summary>
        private static readonly MethodInfo memberClassISerializeMethod = typeof(dataDeSerializer).GetMethod("memberClassISerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 引用类型成员反序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool MemberClassDeSerialize<valueType>(ref valueType value) where valueType : class
        {
            memberClassDeSerialize(ref value);
            return state == deSerializeState.Success;
        }
        /// <summary>
        /// 引用类型成员反序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberClassDeSerialize<valueType>(ref valueType value) where valueType : class
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else typeDeSerializer<valueType>.ClassDeSerialize(this, ref value);
        }
        /// <summary>
        /// 序列化接口函数信息
        /// </summary>
        private static readonly MethodInfo memberClassDeSerializeMethod = typeof(dataDeSerializer).GetMethod("memberClassDeSerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 未知类型反序列化
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="value"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool MemberStructDeSerialize<valueType>(ref valueType value)
        {
            typeDeSerializer<valueType>.StructDeSerialize(this, ref value);
            return state == deSerializeState.Success;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="data"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static valueType DeSerialize<valueType>(byte[] data, config config = null)
        {
            if (data != null)
            {
                fixed (byte* dataFixed = data) return deSerialize<valueType>(data, dataFixed, data.Length, config);
            }
            if (config != null) config.State = deSerializeState.UnknownData;
            return default(valueType);
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="data"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static valueType DeSerialize<valueType>(ref subArray<byte> data, config config = null)
        {
            if (data.length != 0)
            {
                fixed (byte* dataFixed = data.array) return deSerialize<valueType>(data.array, dataFixed + data.startIndex, data.length, config);
            }
            if (config != null) config.State = deSerializeState.UnknownData;
            return default(valueType);
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static valueType DeSerialize<valueType>(unmanagedStream data, int startIndex = 0, config config = null)
        {
            if (data != null && startIndex >= 0)
            {
                return deSerialize<valueType>(null, data.data.Byte + startIndex, data.Length - startIndex, config);
            }
            if (config != null) config.State = deSerializeState.UnknownData;
            return default(valueType);
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="data"></param>
        /// <param name="size"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType DeSerialize<valueType>(byte* data, int size, config config = null)
        {
            return deSerialize<valueType>(null, data, size, config);
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="buffer"></param>
        /// <param name="data"></param>
        /// <param name="size"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        private static valueType deSerialize<valueType>(byte[] buffer, byte* data, int size, config config)
        {
            if (config == null) config = defaultConfig;
            int length = size - sizeof(int);
            if (length >= 0 && data != null)
            {
                if (config.IsFullData)
                {
                    if ((size & 3) == 0)
                    {
                        if (length != 0)
                        {
                            byte* end = data + length;
                            if (*(int*)end == length)
                            {
                                if ((*(uint*)data & binarySerializer.config.HeaderMapAndValue) == binarySerializer.config.HeaderMapValue)
                                {
                                    valueType value = default(valueType);
                                    dataDeSerializer deSerializer = typePool<dataDeSerializer>.Pop() ?? new dataDeSerializer();
                                    try
                                    {
                                        return deSerializer.deSerialize<valueType>(buffer, data, end, ref value, config) == deSerializeState.Success ? value : default(valueType);
                                    }
                                    finally { deSerializer.free(); }
                                }
                                config.State = deSerializeState.HeaderError;
                                return default(valueType);
                            }
                            config.State = deSerializeState.EndVerify;
                            return default(valueType);
                        }
                        if (*(int*)data == dataSerializer.NullValue)
                        {
                            config.State = deSerializeState.Success;
                            return default(valueType);
                        }
                    }
                }
                else
                {
                    if ((*(uint*)data & binarySerializer.config.HeaderMapAndValue) == binarySerializer.config.HeaderMapValue)
                    {
                        valueType value = default(valueType);
                        dataDeSerializer deSerializer = typePool<dataDeSerializer>.Pop() ?? new dataDeSerializer();
                        try
                        {
                            return deSerializer.deSerialize<valueType>(buffer, data, data + length, ref value, config) == deSerializeState.Success ? value : default(valueType);
                        }
                        finally { deSerializer.free(); }
                    }
                    if (*(int*)data == dataSerializer.NullValue)
                    {
                        config.State = deSerializeState.Success;
                        config.DataLength = sizeof(int);
                        return default(valueType);
                    }
                    config.State = deSerializeState.HeaderError;
                    return default(valueType);
                }
            }
            config.State = deSerializeState.UnknownData;
            return default(valueType);
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="data"></param>
        /// <param name="value"></param>
        /// <param name="config"></param>
        /// <returns>是否成功</returns>
        public static bool DeSerialize<valueType>(byte[] data, ref valueType value, config config = null)
        {
            if (data != null)
            {
                fixed (byte* dataFixed = data) return deSerialize<valueType>(data, dataFixed, data.Length, ref value, config);
            }
            if (config != null) config.State = deSerializeState.UnknownData;
            return false;
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="data"></param>
        /// <param name="value"></param>
        /// <param name="config"></param>
        /// <returns>是否成功</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static bool DeSerialize<valueType>(subArray<byte> data, ref valueType value, config config = null)
        {
            return DeSerialize(ref data, ref value, config);
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="data"></param>
        /// <param name="value"></param>
        /// <param name="config"></param>
        /// <returns>是否成功</returns>
        public static bool DeSerialize<valueType>(ref subArray<byte> data, ref valueType value, config config = null)
        {
            if (data.length != 0)
            {
                fixed (byte* dataFixed = data.array) return deSerialize<valueType>(data.array, dataFixed + data.startIndex, data.length, ref value, config);
            }
            if (config != null) config.State = deSerializeState.UnknownData;
            return false;
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="value"></param>
        /// <param name="config"></param>
        /// <returns>是否成功</returns>
        public static bool DeSerialize<valueType>(unmanagedStream data, ref valueType value, int startIndex = 0, config config = null)
        {
            if (data != null && startIndex >= 0)
            {
                return deSerialize<valueType>(null, data.data.Byte + startIndex, data.Length - startIndex, ref value, config);
            }
            if (config != null) config.State = deSerializeState.UnknownData;
            return false;
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="data"></param>
        /// <param name="size"></param>
        /// <param name="value"></param>
        /// <param name="config"></param>
        /// <returns>是否成功</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static bool DeSerialize<valueType>(byte* data, int size, ref valueType value, config config = null)
        {
            return deSerialize(null, data, size, ref value, config);
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="buffer"></param>
        /// <param name="data"></param>
        /// <param name="size"></param>
        /// <param name="value"></param>
        /// <param name="config"></param>
        /// <returns>是否成功</returns>
        private static bool deSerialize<valueType>(byte[] buffer, byte* data, int size, ref valueType value, config config)
        {
            if (config == null) config = defaultConfig;
            int length = size - sizeof(int);
            if (length >= 0)
            {
                if (config.IsFullData)
                {
                    if ((size & 3) == 0)
                    {
                        if (length != 0)
                        {
                            byte* end = data + length;
                            if (*(int*)end == length)
                            {
                                if ((*(uint*)data & binarySerializer.config.HeaderMapAndValue) == binarySerializer.config.HeaderMapValue)
                                {
                                    dataDeSerializer deSerializer = typePool<dataDeSerializer>.Pop() ?? new dataDeSerializer();
                                    try
                                    {
                                        return deSerializer.deSerialize<valueType>(buffer, data, end, ref value, config) == deSerializeState.Success;
                                    }
                                    finally { deSerializer.free(); }
                                }
                                config.State = deSerializeState.HeaderError;
                                return false;
                            }
                            config.State = deSerializeState.EndVerify;
                            return false;
                        }
                        if (*(int*)data == dataSerializer.NullValue)
                        {
                            config.State = deSerializeState.Success;
                            value = default(valueType);
                            return true;
                        }
                    }
                }
                else
                {
                    if ((*(uint*)data & binarySerializer.config.HeaderMapAndValue) == binarySerializer.config.HeaderMapValue)
                    {
                        dataDeSerializer deSerializer = typePool<dataDeSerializer>.Pop() ?? new dataDeSerializer();
                        try
                        {
                            return deSerializer.deSerialize<valueType>(buffer, data, data + length, ref value, config) == deSerializeState.Success;
                        }
                        finally { deSerializer.free(); }
                    }
                    if (*(int*)data == dataSerializer.NullValue)
                    {
                        config.State = deSerializeState.Success;
                        config.DataLength = sizeof(int);
                        value = default(valueType);
                        return true;
                    }
                    config.State = deSerializeState.HeaderError;
                    return false;
                }
            }
            config.State = deSerializeState.UnknownData;
            return false;
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="data"></param>
        /// <param name="value"></param>
        /// <param name="config"></param>
        /// <returns>是否成功</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static bool CodeDeSerialize<valueType>(byte[] data, ref valueType value, config config = null) where valueType : fastCSharp.code.cSharp.dataSerialize.ISerialize
        {
            return CodeDeSerialize<valueType>(subArray<byte>.Unsafe(data, 0, data.Length), ref value, config);
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="data"></param>
        /// <param name="value"></param>
        /// <param name="config"></param>
        /// <returns>是否成功</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static bool CodeDeSerialize<valueType>(subArray<byte> data, ref valueType value, config config = null) where valueType : fastCSharp.code.cSharp.dataSerialize.ISerialize
        {
            return CodeDeSerialize(ref data, ref value, config);
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="data"></param>
        /// <param name="value"></param>
        /// <param name="config"></param>
        /// <returns>是否成功</returns>
        public static bool CodeDeSerialize<valueType>(ref subArray<byte> data, ref valueType value, config config = null) where valueType : fastCSharp.code.cSharp.dataSerialize.ISerialize
        {
            if (config == null) config = defaultConfig;
            int length = data.length - sizeof(int);
            if (length >= 0)
            {
                if (config.IsFullData)
                {
                    if ((data.length & 3) == 0)
                    {
                        fixed (byte* dataFixed = data.array)
                        {
                            byte* start = dataFixed + data.startIndex;
                            if (length != 0)
                            {
                                byte* end = start + length;
                                if (*(int*)end == length)
                                {
                                    if ((*(uint*)start & binarySerializer.config.HeaderMapAndValue) == binarySerializer.config.HeaderMapValue)
                                    {
                                        dataDeSerializer deSerializer = typePool<dataDeSerializer>.Pop() ?? new dataDeSerializer();
                                        try
                                        {
                                            return deSerializer.codeDeSerialize<valueType>(data.array, start, end, ref value, config) == deSerializeState.Success;
                                        }
                                        finally { deSerializer.free(); }
                                    }
                                    config.State = deSerializeState.HeaderError;
                                    return false;
                                }
                                config.State = deSerializeState.EndVerify;
                                return false;
                            }
                            if (*(int*)start == dataSerializer.NullValue)
                            {
                                config.State = deSerializeState.Success;
                                value = default(valueType);
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    fixed (byte* dataFixed = data.array)
                    {
                        byte* start = dataFixed + data.startIndex;
                        if ((*(uint*)start & binarySerializer.config.HeaderMapAndValue) == binarySerializer.config.HeaderMapValue)
                        {
                            dataDeSerializer deSerializer = typePool<dataDeSerializer>.Pop() ?? new dataDeSerializer();
                            try
                            {
                                return deSerializer.codeDeSerialize<valueType>(data.array, start, start + length, ref value, config) == deSerializeState.Success;
                            }
                            finally { deSerializer.free(); }
                        }
                        if (*(int*)start == dataSerializer.NullValue)
                        {
                            config.State = deSerializeState.Success;
                            config.DataLength = sizeof(int);
                            value = default(valueType);
                            return true;
                        }
                        config.State = deSerializeState.HeaderError;
                        return false;
                    }
                }
            }
            config.State = deSerializeState.UnknownData;
            return false;
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="data"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        private static object deSerializeType<valueType>(subArray<byte> data, config config)
        {
            return DeSerialize<valueType>(ref data, config);
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        private static object DeSerializeType<valueType>(Type type, subArray<byte> data, config config = null)
        {
            if (type == null) log.Error.Throw(log.exceptionType.Null);
            Func<subArray<byte>, config, object> parse;
            if (!deSerializeTypes.TryGetValue(type, out parse))
            {
                parse = (Func<subArray<byte>, config, object>)Delegate.CreateDelegate(typeof(Func<subArray<byte>, config, object>), deSerializeTypeMethod.MakeGenericMethod(type));
                deSerializeTypes.Set(type, parse);
            }
            return parse(data, config);
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        private static readonly interlocked.dictionary<Type, Func<subArray<byte>, config, object>> deSerializeTypes = new interlocked.dictionary<Type,Func<subArray<byte>,config,object>>();
        /// <summary>
        /// 反序列化函数信息
        /// </summary>
        private static readonly MethodInfo deSerializeTypeMethod = typeof(dataDeSerializer).GetMethod("deSerializeType", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(subArray<byte>), typeof(config) }, null);
        /// <summary>
        /// 基本类型反序列化函数
        /// </summary>
        private static readonly Dictionary<Type, MethodInfo> deSerializeMethods;
        /// <summary>
        /// 获取基本类型反序列化函数
        /// </summary>
        /// <param name="type">基本类型</param>
        /// <returns>反序列化函数</returns>
        private static MethodInfo getDeSerializeMethod(Type type)
        {
            MethodInfo method;
            if (deSerializeMethods.TryGetValue(type, out method))
            {
                deSerializeMethods.Remove(type);
                return method;
            }
            return null;
        }
        /// <summary>
        /// 基本类型转换函数
        /// </summary>
        private static readonly Dictionary<Type, MethodInfo> memberDeSerializeMethods;
        /// <summary>
        /// 获取基本类型转换函数
        /// </summary>
        /// <param name="type">基本类型</param>
        /// <returns>转换函数</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static MethodInfo getMemberDeSerializeMethod(Type type)
        {
            MethodInfo method;
            return memberDeSerializeMethods.TryGetValue(type, out method) ? method : null;
        }
        /// <summary>
        /// 基本类型转换函数
        /// </summary>
        private static readonly Dictionary<Type, MethodInfo> memberMapDeSerializeMethods;
        /// <summary>
        /// 获取基本类型转换函数
        /// </summary>
        /// <param name="type">基本类型</param>
        /// <returns>转换函数</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static MethodInfo getMemberMapDeSerializeMethod(Type type)
        {
            MethodInfo method;
            return memberMapDeSerializeMethods.TryGetValue(type, out method) ? method : null;
        }
        static dataDeSerializer()
        {
            deSerializeMethods = dictionary.CreateOnly<Type, MethodInfo>();
            memberDeSerializeMethods = dictionary.CreateOnly<Type, MethodInfo>();
            memberMapDeSerializeMethods = dictionary.CreateOnly<Type, MethodInfo>();
            foreach (MethodInfo method in typeof(dataDeSerializer).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                Type parameterType = null;
                if (method.customAttribute<deSerializeMethod>() != null)
                {
                    deSerializeMethods.Add(parameterType = method.GetParameters()[0].ParameterType.GetElementType(), method);
                }
                if (method.customAttribute<memberDeSerializeMethod>() != null)
                {
                    if (parameterType == null) parameterType = method.GetParameters()[0].ParameterType.GetElementType();
                    memberDeSerializeMethods.Add(parameterType, method);
                }
                if (method.customAttribute<memberMapDeSerializeMethod>() != null)
                {
                    memberMapDeSerializeMethods.Add(parameterType ?? method.GetParameters()[0].ParameterType.GetElementType(), method);
                }
            }
        }
    }
}
