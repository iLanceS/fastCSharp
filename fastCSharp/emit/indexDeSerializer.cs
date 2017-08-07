using System;
using System.Collections.Generic;
using fastCSharp.code;
using fastCSharp.threading;
using fastCSharp.reflection;
using System.Reflection;
using System.Runtime.CompilerServices;
#if NOJIT
#else
using System.Reflection.Emit;
#endif

namespace fastCSharp.emit
{
    /// <summary>
    /// 二进制数据反序列化
    /// </summary>
    public unsafe sealed class indexDeSerializer : binaryDeSerializer
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
        internal static class typeDeSerializer
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
                    dynamicMethod = new DynamicMethod("indexDeSerializer", null, new Type[] { typeof(indexDeSerializer), type.MakeByRefType() }, type, true);
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
                /// 返回结束反序列化
                /// </summary>
                private Label returnLabel;
                /// <summary>
                /// 动态函数
                /// </summary>
                /// <param name="type"></param>
                public memberMapDynamicMethod(Type type)
                {
                    dynamicMethod = new DynamicMethod("indexMemberMapDeSerializer", null, new Type[] { typeof(memberMap), typeof(indexDeSerializer), type.MakeByRefType() }, type, true);
                    generator = dynamicMethod.GetILGenerator();
                    returnLabel = generator.DefineLabel();
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
                    generator.int32(field.MemberIndex);
                    generator.Emit(OpCodes.Call, isMemberIndexMethod);
                    generator.Emit(OpCodes.Brfalse, returnLabel);

                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Ldarg_2);
                    if (!isValueType) generator.Emit(OpCodes.Ldind_Ref);
                    generator.Emit(OpCodes.Ldflda, field.Field);
                    MethodInfo method = getMemberMapDeSerializeMethod(field.Field.FieldType) ?? GetMemberMapDeSerializer(field.Field.FieldType);
                    generator.Emit(method.IsFinal || !method.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, method);

                    generator.MarkLabel(end);
                }
                /// <summary>
                /// 创建成员转换委托
                /// </summary>
                /// <returns>成员转换委托</returns>
                public Delegate Create<delegateType>()
                {
                    generator.MarkLabel(returnLabel);
                    generator.Emit(OpCodes.Ret);
                    return dynamicMethod.CreateDelegate(typeof(delegateType));
                }
            }
#endif
            /// <summary>
            /// 未知类型反序列化调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> memberDeSerializers = new interlocked.dictionary<Type, MethodInfo>();
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
                            method = memberKeyValuePairDeSerializeMethod.MakeGenericMethod(type.GetGenericArguments());
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
                        if (type.IsValueType) method = structDeSerializeMethod.MakeGenericMethod(type);
                        else method = classDeSerializeMethod.MakeGenericMethod(type);
                    }
                }
                memberDeSerializers.Set(type, method);
                return method;
            }

            /// <summary>
            /// 未知类型反序列化调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> memberMapDeSerializers = new interlocked.dictionary<Type, MethodInfo>();
            /// <summary>
            /// 未知类型枚举反序列化委托调用函数信息
            /// </summary>
            /// <param name="type">数组类型</param>
            /// <returns>未知类型反序列化委托调用函数信息</returns>
            public static MethodInfo GetMemberMapDeSerializer(Type type)
            {
                MethodInfo method;
                if (memberMapDeSerializers.TryGetValue(type, out method)) return method;
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
                    if (enumType == typeof(uint)) method = enumUIntMethod;
                    else if (enumType == typeof(byte)) method = enumByteMemberMapMethod;
                    else if (enumType == typeof(ulong)) method = enumULongMethod;
                    else if (enumType == typeof(ushort)) method = enumUShortMemberMapMethod;
                    else if (enumType == typeof(long)) method = enumLongMethod;
                    else if (enumType == typeof(short)) method = enumShortMemberMapMethod;
                    else if (enumType == typeof(sbyte)) method = enumSByteMemberMapMethod;
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
                            method = dictionaryMemberMapMethod.MakeGenericMethod(type.GetGenericArguments());
                        }
                        else if (genericType == typeof(Nullable<>))
                        {
                            method = nullableMemberMapDeSerializeMethod.MakeGenericMethod(type.GetGenericArguments());
                        }
                        else if (genericType == typeof(KeyValuePair<,>))
                        {
                            method = memberKeyValuePairDeSerializeMethod.MakeGenericMethod(type.GetGenericArguments());
                        }
                        else if (genericType == typeof(SortedDictionary<,>))
                        {
                            method = sortedDictionaryMemberMapMethod.MakeGenericMethod(type.GetGenericArguments());
                        }
                        else if (genericType == typeof(SortedList<,>))
                        {
                            method = sortedListMemberMapMethod.MakeGenericMethod(type.GetGenericArguments());
                        }
                    }
                    if (method == null)
                    {
                        if (type.IsValueType) method = structDeSerializeMethod.MakeGenericMethod(type);
                        else method = memberMapClassDeSerializeMethod.MakeGenericMethod(type);
                    }
                }
                memberMapDeSerializers.Set(type, method);
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
            internal delegate void deSerialize(indexDeSerializer deSerializer, ref valueType value);
            /// <summary>
            /// 二进制数据反序列化委托
            /// </summary>
            /// <param name="memberMap">成员位图</param>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value">目标数据</param>
            private delegate void memberMapDeSerialize(memberMap memberMap, indexDeSerializer deSerializer, ref valueType value);
            /// <summary>
            /// 二进制数据序列化类型配置
            /// </summary>
            private static readonly indexSerialize attribute;
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
            /// 序列化成员数量
            /// </summary>
            private static readonly int memberCountVerify;
            ///// <summary>
            ///// 固定分组字节数
            ///// </summary>
            //private static readonly int fixedSize;
            /// <summary>
            /// 固定分组填充字节数
            /// </summary>
            private static readonly int fixedFillSize;
            ///// <summary>
            ///// 成员位图模式固定分组字节数
            ///// </summary>
            //private static readonly int memberMapFixedSize;
            ///// <summary>
            ///// 是否值类型
            ///// </summary>
            //private static readonly bool isValueType;

            /// <summary>
            /// 对象反序列化
            /// </summary>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value">数据对象</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal static void DeSerialize(indexDeSerializer deSerializer, ref valueType value)
            {
                if (DefaultDeSerializer == null)
                {
                    if (value == null) value = fastCSharp.emit.constructor<valueType>.New();
                    MemberDeSerialize(deSerializer, ref value);
                }
                else DefaultDeSerializer(deSerializer, ref value);
            }
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value">数据对象</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal static void StructDeSerialize(indexDeSerializer deSerializer, ref valueType value)
            {
                if (DefaultDeSerializer == null) MemberDeSerialize(deSerializer, ref value);
                else DefaultDeSerializer(deSerializer, ref value);
            }
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value">数据对象</param>
            internal static void MemberDeSerialize(indexDeSerializer deSerializer, ref valueType value)
            {
                if (deSerializer.CheckMemberCount(memberCountVerify))
                {
                    fixedMemberDeSerializer(deSerializer, ref value);
                    deSerializer.Read += fixedFillSize;
                    memberDeSerializer(deSerializer, ref value);
                    if (attribute.IsJson || jsonMemberMap != null) deSerializer.parseJson(ref value);
                }
                else
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
            }
            /// <summary>
            /// 对象反序列化
            /// </summary>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value">数据对象</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal static void BaseSerialize<childType>(indexDeSerializer deSerializer, ref childType value) where childType : valueType
            {
                if (value == null) value = fastCSharp.emit.constructor<childType>.New();
                valueType newValue = value;
                StructDeSerialize(deSerializer, ref newValue);
            }
            /// <summary>
            /// 不支持对象转换null
            /// </summary>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value">数据对象</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private static void fromNull(indexDeSerializer deSerializer, ref valueType value)
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
            private unsafe static void enumByte(indexDeSerializer deSerializer, ref valueType value)
            {
                deSerializer.enumByte(ref value);
            }
            /// <summary>
            /// 枚举值序列化
            /// </summary>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value">枚举值序列化</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe static void enumSByte(indexDeSerializer deSerializer, ref valueType value)
            {
                deSerializer.enumSByte(ref value);
            }
            /// <summary>
            /// 枚举值序列化
            /// </summary>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value">枚举值序列化</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe static void enumShort(indexDeSerializer deSerializer, ref valueType value)
            {
                deSerializer.enumShort(ref value);
            }
            /// <summary>
            /// 枚举值序列化
            /// </summary>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value">枚举值序列化</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe static void enumUShort(indexDeSerializer deSerializer, ref valueType value)
            {
                deSerializer.enumUShort(ref value);
            }
            /// <summary>
            /// 枚举值序列化
            /// </summary>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value">枚举值序列化</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe static void enumInt(indexDeSerializer deSerializer, ref valueType value)
            {
                deSerializer.enumInt(ref value);
            }
            /// <summary>
            /// 枚举值序列化
            /// </summary>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value">枚举值序列化</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe static void enumUInt(indexDeSerializer deSerializer, ref valueType value)
            {
                deSerializer.enumUInt(ref value);
            }
            /// <summary>
            /// 枚举值序列化
            /// </summary>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value">枚举值序列化</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe static void enumLong(indexDeSerializer deSerializer, ref valueType value)
            {
                deSerializer.enumLong(ref value);
            }
            /// <summary>
            /// 枚举值序列化
            /// </summary>
            /// <param name="deSerializer">二进制数据反序列化</param>
            /// <param name="value">枚举值序列化</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe static void enumULong(indexDeSerializer deSerializer, ref valueType value)
            {
                deSerializer.enumULong(ref value);
            }
            static typeDeSerializer()
            {
                Type type = typeof(valueType), attributeType;
                MethodInfo methodInfo = indexDeSerializer.getDeSerializeMethod(type);
                attribute = type.customAttribute<indexSerialize>(out attributeType, true) ?? indexSerialize.Default;
                if (methodInfo != null)
                {
#if NOJIT
                    DefaultDeSerializer = new methodDeSerializer(methodInfo).DeSerialize;
#else
                    DynamicMethod dynamicMethod = new DynamicMethod("indexDeSerializer", typeof(void), new Type[] { typeof(indexDeSerializer), type.MakeByRefType() }, true);
                    dynamicMethod.InitLocals = true;
                    ILGenerator generator = dynamicMethod.GetILGenerator();
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Call, methodInfo);
                    generator.Emit(OpCodes.Ret);
                    DefaultDeSerializer = (deSerialize)dynamicMethod.CreateDelegate(typeof(deSerialize));
#endif
                    //isValueType = true;
                    return;
                }
                if (type.IsArray)
                {
                    //isValueType = true;
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
                                }
                                else if (elementType.IsGenericType && elementType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                {
                                    methodInfo = nullableArrayMethod.MakeGenericMethod(elementType.GetGenericArguments());
                                }
                                else methodInfo = structArrayMethod.MakeGenericMethod(elementType);
                            }
                            else methodInfo = arrayMethod.MakeGenericMethod(elementType);
                            DefaultDeSerializer = (deSerialize)Delegate.CreateDelegate(typeof(deSerialize), methodInfo);
                            return;
                        }
                    }
                    DefaultDeSerializer = fromNull;
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
                    //isValueType = true;
                    return;
                }
                if (type.IsPointer || type.IsAbstract || type.IsInterface)
                {
                    DefaultDeSerializer = fromNull;
                    return;
                }
                if (type.IsGenericType)
                {
                    Type genericType = type.GetGenericTypeDefinition();
                    if (genericType == typeof(subArray<>))
                    {
                        DefaultDeSerializer = (deSerialize)Delegate.CreateDelegate(typeof(deSerialize), subArrayDeSerializeMethod.MakeGenericMethod(type.GetGenericArguments()));
                        //isValueType = true;
                        return;
                    }
                    if (genericType == typeof(Dictionary<,>))
                    {
                        DefaultDeSerializer = (deSerialize)Delegate.CreateDelegate(typeof(deSerialize), dictionaryDeSerializeMethod.MakeGenericMethod(type.GetGenericArguments()));
                        //isValueType = true;
                        return;
                    }
                    if (genericType == typeof(nullValue<>))
                    {
                        DefaultDeSerializer = (deSerialize)Delegate.CreateDelegate(typeof(deSerialize), nullableDeSerializeMethod.MakeGenericMethod(type.GetGenericArguments()));
                        //isValueType = true;
                        return;
                    }
                    if (genericType == typeof(KeyValuePair<,>))
                    {
                        DefaultDeSerializer = (deSerialize)Delegate.CreateDelegate(typeof(deSerialize), keyValuePairDeSerializeMethod.MakeGenericMethod(type.GetGenericArguments()));
                        //isValueType = true;
                        return;
                    }
                    if (genericType == typeof(SortedDictionary<,>))
                    {
                        DefaultDeSerializer = (deSerialize)Delegate.CreateDelegate(typeof(deSerialize), sortedDictionaryDeSerializeMethod.MakeGenericMethod(type.GetGenericArguments()));
                        //isValueType = true;
                        return;
                    }
                    if (genericType == typeof(SortedList<,>))
                    {
                        DefaultDeSerializer = (deSerialize)Delegate.CreateDelegate(typeof(deSerialize), sortedListDeSerializeMethod.MakeGenericMethod(type.GetGenericArguments()));
                        //isValueType = true;
                        return;
                    }
                }
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
                                methodInfo = arrayConstructorMethod.MakeGenericMethod(type, argumentType);
                                break;
                            }
                            parameters[0] = typeof(IList<>).MakeGenericType(argumentType);
                            constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, parameters, null);
                            if (constructorInfo != null)
                            {
                                methodInfo = listConstructorMethod.MakeGenericMethod(type, argumentType);
                                break;
                            }
                            parameters[0] = typeof(ICollection<>).MakeGenericType(argumentType);
                            constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, parameters, null);
                            if (constructorInfo != null)
                            {
                                methodInfo = collectionConstructorMethod.MakeGenericMethod(type, argumentType);
                                break;
                            }
                            parameters[0] = typeof(IEnumerable<>).MakeGenericType(argumentType);
                            constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, parameters, null);
                            if (constructorInfo != null)
                            {
                                methodInfo = enumerableConstructorMethod.MakeGenericMethod(type, argumentType);
                                break;
                            }
                        }
                        else if (genericType == typeof(IDictionary<,>))
                        {
                            ConstructorInfo constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { interfaceType }, null);
                            if (constructorInfo != null)
                            {
                                Type[] parameters = interfaceType.GetGenericArguments();
                                methodInfo = dictionaryConstructorMethod.MakeGenericMethod(type, parameters[0], parameters[1]);
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
                if (constructor<valueType>.New == null) DefaultDeSerializer = fromNull;
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
                                    methodInfo = baseSerializeMethod.MakeGenericMethod(baseType, type);
                                    DefaultDeSerializer = (deSerialize)Delegate.CreateDelegate(typeof(deSerialize), methodInfo);
                                    return;
                                }
                                break;
                            }
                        }
                    }
                    int memberMapFixedSize;
                    binarySerializer.fields<indexSerializer.fieldInfo> fields = indexSerializer.staticTypeSerializer.GetFields(memberIndexGroup<valueType>.GetFields(attribute.MemberFilter), out memberCountVerify, out memberMapFixedSize);
                    fixedFillSize = -fields.FixedSize & 3;
                    //fixedSize = (fields.FixedSize + (sizeof(int) + 3)) & (int.MaxValue - 3);
#if NOJIT
                    fixedMemberDeSerializer = new deSerializer(ref fields.FixedFields).DeSerialize;
                    fixedMemberMapDeSerializer = new mapDeSerializer(ref fields.FixedFields).DeSerialize;
                    memberDeSerializer = new deSerializer(ref fields.Fields).DeSerialize;
                    memberMapDeSerializer = new mapDeSerializer(ref fields.Fields).DeSerialize;
#else
                    typeDeSerializer.memberDynamicMethod fixedDynamicMethod = new typeDeSerializer.memberDynamicMethod(type);
                    typeDeSerializer.memberMapDynamicMethod fixedMemberMapDynamicMethod = new typeDeSerializer.memberMapDynamicMethod(type);
                    foreach (indexSerializer.fieldInfo member in fields.FixedFields)
                    {
                        fixedDynamicMethod.Push(member);
                        fixedMemberMapDynamicMethod.Push(member);
                    }
                    fixedMemberDeSerializer = (deSerialize)fixedDynamicMethod.Create<deSerialize>();
                    fixedMemberMapDeSerializer = (memberMapDeSerialize)fixedMemberMapDynamicMethod.Create<memberMapDeSerialize>();

                    typeDeSerializer.memberDynamicMethod dynamicMethod = new typeDeSerializer.memberDynamicMethod(type);
                    typeDeSerializer.memberMapDynamicMethod memberMapDynamicMethod = new typeDeSerializer.memberMapDynamicMethod(type);
                    foreach (indexSerializer.fieldInfo member in fields.Fields)
                    {
                        dynamicMethod.Push(member);
                        memberMapDynamicMethod.Push(member);
                    }
                    memberDeSerializer = (deSerialize)dynamicMethod.Create<deSerialize>();
                    memberMapDeSerializer = (memberMapDeSerialize)memberMapDynamicMethod.Create<memberMapDeSerialize>();
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
                public void DeSerialize(indexDeSerializer parser, ref valueType value)
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
                    public void Set(indexSerializer.fieldInfo field)
                    {
                        Field = field.Field;
                        DeSerializeMethod = getMemberDeSerializeMethod(Field.FieldType) ?? typeDeSerializer.GetMemberDeSerializer(Field.FieldType);
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
                public deSerializer(ref subArray<indexSerializer.fieldInfo> fields)
                {
                    this.fields = new field[fields.length];
                    int index = 0;
                    foreach (indexSerializer.fieldInfo field in fields) this.fields[index++].Set(field);
                }
                /// <summary>
                /// 反序列化
                /// </summary>
                /// <param name="deSerializer"></param>
                /// <param name="value"></param>
                public void DeSerialize(indexDeSerializer deSerializer, ref valueType value)
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
                    public void Set(indexSerializer.fieldInfo field)
                    {
                        Field = field.Field;
                        MemberIndex = field.MemberIndex;
                        DeSerializeMethod = getMemberMapDeSerializeMethod(Field.FieldType) ?? typeDeSerializer.GetMemberMapDeSerializer(Field.FieldType);
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
                public mapDeSerializer(ref subArray<indexSerializer.fieldInfo> fields)
                {
                    this.fields = new field[fields.length];
                    int index = 0;
                    foreach (indexSerializer.fieldInfo field in fields) this.fields[index++].Set(field);
                }
                /// <summary>
                /// 反序列化
                /// </summary>
                /// <param name="memberMap"></param>
                /// <param name="deSerializer"></param>
                /// <param name="value"></param>
                public void DeSerialize(memberMap memberMap, indexDeSerializer deSerializer, ref valueType value)
                {
                    object[] parameters = null;
                    object objectValue = value;
                    foreach (field field in fields)
                    {
                        if (memberMap.IsMember(field.MemberIndex))
                        {
                            if (deSerializer.isMemberIndex(field.MemberIndex))
                            {
                                if (parameters == null) parameters = new object[1];
                                parameters[0] = field.Field.GetValue(objectValue);
                                field.DeSerializeMethod.Invoke(deSerializer, parameters);
                                field.Field.SetValue(objectValue, parameters[0]);
                            }
                            else return;
                        }
                    }
                    value = (valueType)objectValue;
                }
            }
#endif
        }
        /// <summary>
        /// 当前数据字节数
        /// </summary>
        private int currentSize;
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        private deSerializeState deSerialize<valueType>(byte* start, byte* end, ref valueType value, config config)
        {
            Config = config;
            this.start = start;
            Read = start + sizeof(int);
            this.end = end;
            isMemberMap = (*start & dataSerializer.config.MemberMapValue) != 0;
            if (isMemberMap) MemberMap = config.MemberMap;
            state = deSerializeState.Success;
            typeDeSerializer<valueType>.DeSerialize(this, ref value);
            checkState();
            return config.State = state;
        }
        /// <summary>
        /// 检测是否匹配成员索引
        /// </summary>
        /// <param name="memberIndex"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private bool isMemberIndex(int memberIndex)
        {
            if ((*(uint*)Read & int.MaxValue) == memberIndex)
            {
                Read += sizeof(int);
                return true;
            }
            Error(deSerializeState.MemberIndex);
            return false;
        }
        /// <summary>
        /// 检测是否匹配成员索引函数信息
        /// </summary>
        private static readonly MethodInfo isMemberIndexMethod = typeof(indexDeSerializer).GetMethod("isMemberIndex", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组反序列化
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="value"></param>
        /// <returns>数组长度</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private int deSerializeArray<valueType>(ref valueType[] value)
        {
            if (*(int*)Read != 0) return *(int*)Read;
            value = nullValue<valueType>.Array;
            Read += sizeof(int);
            return 0;
        }
        /// <summary>
        /// 反序列化数组
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="value"></param>
        /// <returns>当前数据字节长度</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private int deSerializeArrayMember<valueType>(ref valueType[] value)
        {
            if ((currentSize = *(int*)Read) == 0)
            {
                value = nullValue<valueType>.Array;
                Read += sizeof(int);
                return 0;
            }
            return *(int*)(Read + sizeof(int));
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
                    value = new bool[length];
                    Read = DeSerialize(Read + sizeof(int), value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 逻辑值反序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        private void deSerializeBoolArray(ref bool[] value)
        {
            int length = deSerializeArrayMember(ref value);
            if (length != 0)
            {
                if ((((length + (31 + 64)) >> 5) << 2) == currentSize && (uint)currentSize <= (uint)(int)(end - Read))
                {
                    value = new bool[length];
                    Read = DeSerialize(Read + sizeof(int) * 2, value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 逻辑值反序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref bool[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerializeBoolArray(ref value);
        }
        /// <summary>
        /// 逻辑值反序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref bool[] value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) deSerializeBoolArray(ref value);
            else value = null;
        }
        /// <summary>
        /// 逻辑值反序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref bool? value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0)
            {
                value = *(bool*)Read;
                Read += sizeof(int);
            }
            else value = null;
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
                    value = new bool?[length >> 1];
                    Read = DeSerialize(Read + sizeof(int), value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 逻辑值反序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        private void deSerializeBoolArray(ref bool?[] value)
        {
            int length = deSerializeArrayMember(ref value);
            if (length != 0)
            {
                if ((((length + (31 + 64)) >> 5) << 2) == currentSize && (uint)currentSize <= (uint)(int)(end - Read))
                {
                    value = new bool?[length >> 1];
                    Read = DeSerialize(Read + sizeof(int) * 2, value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 逻辑值反序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref bool?[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerializeBoolArray(ref value);
        }
        /// <summary>
        /// 逻辑值反序列化
        /// </summary>
        /// <param name="value">逻辑值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref bool?[] value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) deSerializeBoolArray(ref value);
            else value = null;
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
                    value = new byte[length];
                    Read = DeSerialize(Read + sizeof(int), value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void deSerializeByteArray(ref byte[] value)
        {
            int length = deSerializeArrayMember(ref value);
            if (length != 0)
            {
                if (((length + (3 + sizeof(int) * 2)) & (int.MaxValue - 3)) == currentSize && (uint)currentSize <= (uint)(int)(end - Read))
                {
                    value = new byte[length];
                    Read = DeSerialize(Read + sizeof(int) * 2, value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref byte[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerializeByteArray(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref byte[] value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) deSerializeByteArray(ref value);
            else value = null;
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref subArray<byte> value)
        {
            if ((currentSize = *(int*)Read) == 0)
            {
                value.UnsafeSetLength(0);
                Read += sizeof(int);
            }
            else
            {
                int length = *(int*)(Read + sizeof(int));
                if (length != 0)
                {
                    if (((length + (3 + sizeof(int) * 2)) & (int.MaxValue - 3)) == currentSize && (uint)currentSize <= (uint)(int)(end - Read))
                    {
                        byte[] array = new byte[length];
                        Read = DeSerialize(Read + sizeof(int) * 2, array);
                        value.UnsafeSet(array, 0, length);
                    }
                    else Error(deSerializeState.IndexOutOfRange);
                }
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref byte? value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0)
            {
                value = *(byte*)Read;
                Read += sizeof(int);
            }
            else value = null;
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
                    value = new byte?[length];
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
        private void deSerializeByteArray(ref byte?[] value)
        {
            int length = deSerializeArrayMember(ref value);
            if (length != 0)
            {
                byte* end = Read + currentSize;
                if ((((length + (31 + 64)) >> 5) << 2) <= currentSize && end <= this.end)
                {
                    value = new byte?[length];
                    Read = DeSerialize(Read + sizeof(int) * 2, value);
                    if (Read == end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref byte?[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerializeByteArray(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref byte?[] value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) deSerializeByteArray(ref value);
            else value = null;
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
                    value = new sbyte[length];
                    Read = DeSerialize(Read + sizeof(int), value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void deSerializeSByteArray(ref sbyte[] value)
        {
            int length = deSerializeArrayMember(ref value);
            if (length != 0)
            {
                if (((length + (3 + sizeof(int) * 2)) & (int.MaxValue - 3)) == currentSize && (uint)currentSize <= (uint)(int)(end - Read))
                {
                    value = new sbyte[length];
                    Read = DeSerialize(Read + sizeof(int) * 2, value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref sbyte[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerializeSByteArray(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref sbyte[] value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) deSerializeSByteArray(ref value);
            else value = null;
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref sbyte? value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0)
            {
                value = (sbyte)*(int*)Read;
                Read += sizeof(int);
            }
            else value = null;
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
                    value = new sbyte?[length];
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
        private void deSerializeSByteArray(ref sbyte?[] value)
        {
            int length = deSerializeArrayMember(ref value);
            if (length != 0)
            {
                byte* end = Read + currentSize;
                if ((((length + (31 + 64)) >> 5) << 2) <= currentSize && end <= this.end)
                {
                    value = new sbyte?[length];
                    Read = DeSerialize((sbyte*)Read + sizeof(int) * 2, value);
                    if (Read == end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref sbyte?[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerializeSByteArray(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref sbyte?[] value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) deSerializeSByteArray(ref value);
            else value = null;
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
                    value = new short[length];
                    Read = DeSerialize(Read + sizeof(int), value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void deSerializeShortArray(ref short[] value)
        {
            int length = deSerializeArrayMember(ref value);
            if (length != 0)
            {
                if ((((length * sizeof(short)) + (3 + sizeof(int) * 2)) & (int.MaxValue - 3)) == currentSize && (uint)currentSize <= (uint)(int)(end - Read))
                {
                    value = new short[length];
                    Read = DeSerialize(Read + sizeof(int) * 2, value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref short[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerializeShortArray(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref short[] value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) deSerializeShortArray(ref value);
            else value = null;
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref short? value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0)
            {
                value = (short)*(int*)Read;
                Read += sizeof(int);
            }
            else value = null;
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
                    value = new short?[length];
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
        private void deSerializeShortArray(ref short?[] value)
        {
            int length = deSerializeArrayMember(ref value);
            if (length != 0)
            {
                byte* end = Read + currentSize;
                if ((((length + (31 + 64)) >> 5) << 2) <= currentSize && end <= this.end)
                {
                    value = new short?[length];
                    Read = DeSerialize(Read + sizeof(int) * 2, value);
                    if (Read == end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref short?[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerializeShortArray(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref short?[] value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) deSerializeShortArray(ref value);
            else value = null;
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
                    value = new ushort[length];
                    Read = DeSerialize(Read + sizeof(int), value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void deSerializeUShortArray(ref ushort[] value)
        {
            int length = deSerializeArrayMember(ref value);
            if (length != 0)
            {
                if ((((length * sizeof(short)) + (3 + sizeof(int) * 2)) & (int.MaxValue - 3)) == currentSize && (uint)currentSize <= (uint)(int)(end - Read))
                {
                    value = new ushort[length];
                    Read = DeSerialize(Read + sizeof(int) * 2, value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref ushort[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerializeUShortArray(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref ushort[] value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) deSerializeUShortArray(ref value);
            else value = null;
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref ushort? value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0)
            {
                value = *(ushort*)Read;
                Read += sizeof(int);
            }
            else value = null;
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
                    value = new ushort?[length];
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
        private void deSerializeUShortArray(ref ushort?[] value)
        {
            int length = deSerializeArrayMember(ref value);
            if (length != 0)
            {
                byte* end = Read + currentSize;
                if ((((length + (31 + 64)) >> 5) << 2) <= currentSize && end <= this.end)
                {
                    value = new ushort?[length];
                    Read = DeSerialize(Read + sizeof(int) * 2, value);
                    if (Read == end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref ushort?[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerializeUShortArray(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref ushort?[] value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) deSerializeUShortArray(ref value);
            else value = null;
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
                    value = new int[length];
                    Read = DeSerialize(Read + sizeof(int), value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void deSerializeIntArray(ref int[] value)
        {
            int length = deSerializeArrayMember(ref value);
            if (length != 0)
            {
                if ((length + 2) * sizeof(int) == currentSize && (uint)currentSize <= (uint)(int)(end - Read))
                {
                    value = new int[length];
                    Read = DeSerialize(Read + sizeof(int) * 2, value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref int[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerializeIntArray(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref int[] value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) deSerializeIntArray(ref value);
            else value = null;
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref int? value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0)
            {
                value = *(int*)Read;
                Read += sizeof(int);
            }
            else value = null;
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
                    value = new int?[length];
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
        private void deSerializeIntArray(ref int?[] value)
        {
            int length = deSerializeArrayMember(ref value);
            if (length != 0)
            {
                byte* end = Read + currentSize;
                if ((((length + (31 + 64)) >> 5) << 2) <= currentSize && end <= this.end)
                {
                    value = new int?[length];
                    Read = DeSerialize(Read + sizeof(int) * 2, value);
                    if (Read == end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref int?[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerializeIntArray(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref int?[] value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) deSerializeIntArray(ref value);
            else value = null;
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
                    value = new uint[length];
                    Read = DeSerialize(Read + sizeof(int), value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void deSerializeUIntArray(ref uint[] value)
        {
            int length = deSerializeArrayMember(ref value);
            if (length != 0)
            {
                if ((length + 2) * sizeof(int) == currentSize && (uint)currentSize <= (uint)(int)(end - Read))
                {
                    value = new uint[length];
                    Read = DeSerialize(Read + sizeof(int) * 2, value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref uint[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerializeUIntArray(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref uint[] value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) deSerializeUIntArray(ref value);
            else value = null;
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref uint? value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0)
            {
                value = *(uint*)Read;
                Read += sizeof(int);
            }
            else value = null;
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
                    value = new uint?[length];
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
        private void deSerializeUIntArray(ref uint?[] value)
        {
            int length = deSerializeArrayMember(ref value);
            if (length != 0)
            {
                byte* end = Read + currentSize;
                if ((((length + (31 + 64)) >> 5) << 2) <= currentSize && end <= this.end)
                {
                    value = new uint?[length];
                    Read = DeSerialize(Read + sizeof(int) * 2, value);
                    if (Read == end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref uint?[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerializeUIntArray(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref uint?[] value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) deSerializeUIntArray(ref value);
            else value = null;
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
                    value = new long[length];
                    Read = DeSerialize(Read + sizeof(int), value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void deSerializeLongArray(ref long[] value)
        {
            int length = deSerializeArrayMember(ref value);
            if (length != 0)
            {
                if ((length * sizeof(long) + sizeof(int) * 2) == currentSize && (uint)currentSize <= (uint)(int)(end - Read))
                {
                    value = new long[length];
                    Read = DeSerialize(Read + sizeof(int) * 2, value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref long[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerializeLongArray(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref long[] value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) deSerializeLongArray(ref value);
            else value = null;
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref long? value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0)
            {
                value = *(long*)Read;
                Read += sizeof(long);
            }
            else value = null;
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
                    value = new long?[length];
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
        private void deSerializeLongArray(ref long?[] value)
        {
            int length = deSerializeArrayMember(ref value);
            if (length != 0)
            {
                byte* end = Read + currentSize;
                if ((((length + (31 + 64)) >> 5) << 2) <= currentSize && end <= this.end)
                {
                    value = new long?[length];
                    Read = DeSerialize(Read + sizeof(int) * 2, value);
                    if (Read == end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref long?[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerializeLongArray(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref long?[] value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) deSerializeLongArray(ref value);
            else value = null;
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
                    value = new ulong[length];
                    Read = DeSerialize(Read + sizeof(int), value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void deSerializeULongArray(ref ulong[] value)
        {
            int length = deSerializeArrayMember(ref value);
            if (length != 0)
            {
                if ((length * sizeof(ulong) + sizeof(int) * 2) == currentSize && (uint)currentSize <= (uint)(int)(end - Read))
                {
                    value = new ulong[length];
                    Read = DeSerialize(Read + sizeof(int) * 2, value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref ulong[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerializeULongArray(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref ulong[] value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) deSerializeULongArray(ref value);
            else value = null;
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref ulong? value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0)
            {
                value = *(ulong*)Read;
                Read += sizeof(ulong);
            }
            else value = null;
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
                    value = new ulong?[length];
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
        private void deSerializeULongArray(ref ulong?[] value)
        {
            int length = deSerializeArrayMember(ref value);
            if (length != 0)
            {
                byte* end = Read + currentSize;
                if ((((length + (31 + 64)) >> 5) << 2) <= currentSize && end <= this.end)
                {
                    value = new ulong?[length];
                    Read = DeSerialize(Read + sizeof(int) * 2, value);
                    if (Read == end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref ulong?[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerializeULongArray(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref ulong?[] value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) deSerializeULongArray(ref value);
            else value = null;
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
                    value = new float[length];
                    Read = DeSerialize(Read + sizeof(int), value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void deSerializeFloatArray(ref float[] value)
        {
            int length = deSerializeArrayMember(ref value);
            if (length != 0)
            {
                if ((length * sizeof(float) + sizeof(int) * 2) == currentSize && (uint)currentSize <= (uint)(int)(end - Read))
                {
                    value = new float[length];
                    Read = DeSerialize(Read + sizeof(int) * 2, value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref float[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerializeFloatArray(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref float[] value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) deSerializeFloatArray(ref value);
            else value = null;
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref float? value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0)
            {
                value = *(float*)Read;
                Read += sizeof(float);
            }
            else value = null;
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
                    value = new float?[length];
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
        private void deSerializeFloatArray(ref float?[] value)
        {
            int length = deSerializeArrayMember(ref value);
            if (length != 0)
            {
                byte* end = Read + currentSize;
                if ((((length + (31 + 64)) >> 5) << 2) <= currentSize && end <= this.end)
                {
                    value = new float?[length];
                    Read = DeSerialize(Read + sizeof(int) * 2, value);
                    if (Read == end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref float?[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerializeFloatArray(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref float?[] value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) deSerializeFloatArray(ref value);
            else value = null;
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
                    value = new double[length];
                    Read = DeSerialize(Read + sizeof(int), value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void deSerializeDoubleArray(ref double[] value)
        {
            int length = deSerializeArrayMember(ref value);
            if (length != 0)
            {
                if ((length * sizeof(double) + sizeof(int) * 2) == currentSize && (uint)currentSize <= (uint)(int)(end - Read))
                {
                    value = new double[length];
                    Read = DeSerialize(Read + sizeof(int) * 2, value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref double[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerializeDoubleArray(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref double[] value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) deSerializeDoubleArray(ref value);
            else value = null;
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref double? value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0)
            {
                value = *(double*)Read;
                Read += sizeof(double);
            }
            else value = null;
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
                    value = new double?[length];
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
        private void deSerializeDoubleArray(ref double?[] value)
        {
            int length = deSerializeArrayMember(ref value);
            if (length != 0)
            {
                byte* end = Read + currentSize;
                if ((((length + (31 + 64)) >> 5) << 2) <= currentSize && end <= this.end)
                {
                    value = new double?[length];
                    Read = DeSerialize(Read + sizeof(int) * 2, value);
                    if (Read == end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref double?[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerializeDoubleArray(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref double?[] value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) deSerializeDoubleArray(ref value);
            else value = null;
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
                    value = new decimal[length];
                    Read = DeSerialize(Read + sizeof(int), value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void deSerializeDecimalArray(ref decimal[] value)
        {
            int length = deSerializeArrayMember(ref value);
            if (length != 0)
            {
                if ((length * sizeof(decimal) + sizeof(int) * 2) == currentSize && (uint)currentSize <= (uint)(int)(end - Read))
                {
                    value = new decimal[length];
                    Read = DeSerialize(Read + sizeof(int) * 2, value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref decimal[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerializeDecimalArray(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref decimal[] value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) deSerializeDecimalArray(ref value);
            else value = null;
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref decimal? value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0)
            {
                value = *(decimal*)Read;
                Read += sizeof(decimal);
            }
            else value = null;
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
                    value = new decimal?[length];
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
        private void deSerializeDecimalArray(ref decimal?[] value)
        {
            int length = deSerializeArrayMember(ref value);
            if (length != 0)
            {
                byte* end = Read + currentSize;
                if ((((length + (31 + 64)) >> 5) << 2) <= currentSize && end <= this.end)
                {
                    value = new decimal?[length];
                    Read = DeSerialize(Read + sizeof(int) * 2, value);
                    if (Read == end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref decimal?[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerializeDecimalArray(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref decimal?[] value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) deSerializeDecimalArray(ref value);
            else value = null;
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [deSerializeMethod]
        private void deSerialize(ref char[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                if (((length * sizeof(char) + (3 + sizeof(int))) & (int.MaxValue - 3)) <= (int)(end - Read))
                {
                    value = new char[length];
                    Read = DeSerialize(Read + sizeof(int), value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        private void deSerializeCharArray(ref char[] value)
        {
            int length = deSerializeArrayMember(ref value);
            if (length != 0)
            {
                if ((((length * sizeof(char)) + (3 + sizeof(int) * 2)) & (int.MaxValue - 3)) == currentSize && (uint)currentSize <= (uint)(int)(end - Read))
                {
                    value = new char[length];
                    Read = DeSerialize(Read + sizeof(int) * 2, value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref char[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerializeCharArray(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref char[] value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) deSerializeCharArray(ref value);
            else value = null;
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref char? value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0)
            {
                value = *(char*)Read;
                Read += sizeof(int);
            }
            else value = null;
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [deSerializeMethod]
        private void deSerialize(ref char?[] value)
        {
            int length = deSerializeArray(ref value);
            if (length != 0)
            {
                if ((((length + (31 + 32)) >> 5) << 2) <= (int)(end - Read))
                {
                    value = new char?[length];
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
        private void deSerializeCharArray(ref char?[] value)
        {
            int length = deSerializeArrayMember(ref value);
            if (length != 0)
            {
                byte* end = Read + currentSize;
                if ((((length + (31 + 64)) >> 5) << 2) <= currentSize && end <= this.end)
                {
                    value = new char?[length];
                    Read = DeSerialize(Read + sizeof(int) * 2, value);
                    if (Read == end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref char?[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerializeCharArray(ref value);
        }
        /// <summary>
        /// 数值反序列化
        /// </summary>
        /// <param name="value">数值</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref char?[] value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) deSerializeCharArray(ref value);
            else value = null;
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
                    value = new DateTime[length];
                    Read = DeSerialize(Read + sizeof(int), value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 时间反序列化
        /// </summary>
        /// <param name="value">时间</param>
        private void deSerializeDateTimeArray(ref DateTime[] value)
        {
            int length = deSerializeArrayMember(ref value);
            if (length != 0)
            {
                if ((length * sizeof(DateTime) + sizeof(int) * 2) == currentSize && (uint)currentSize <= (uint)(int)(end - Read))
                {
                    value = new DateTime[length];
                    Read = DeSerialize(Read + sizeof(int) * 2, value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 时间反序列化
        /// </summary>
        /// <param name="value">时间</param>
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref DateTime[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerializeDateTimeArray(ref value);
        }
        /// <summary>
        /// 时间反序列化
        /// </summary>
        /// <param name="value">时间</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref DateTime[] value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) deSerializeDateTimeArray(ref value);
            else value = null;
        }
        /// <summary>
        /// 时间反序列化
        /// </summary>
        /// <param name="value">时间</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref DateTime? value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0)
            {
                value = *(DateTime*)Read;
                Read += sizeof(DateTime);
            }
            else value = null;
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
                    value = new DateTime?[length];
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
        private void deSerializeDateTimeArray(ref DateTime?[] value)
        {
            int length = deSerializeArrayMember(ref value);
            if (length != 0)
            {
                byte* end = Read + currentSize;
                if ((((length + (31 + 64)) >> 5) << 2) <= currentSize && end <= this.end)
                {
                    value = new DateTime?[length];
                    Read = DeSerialize(Read + sizeof(int) * 2, value);
                    if (Read == end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 时间反序列化
        /// </summary>
        /// <param name="value">时间</param>
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref DateTime?[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerializeDateTimeArray(ref value);
        }
        /// <summary>
        /// 时间反序列化
        /// </summary>
        /// <param name="value">时间</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref DateTime?[] value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) deSerializeDateTimeArray(ref value);
            else value = null;
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
                    value = new Guid[length];
                    Read = DeSerialize(Read + sizeof(int), value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// Guid反序列化
        /// </summary>
        /// <param name="value">Guid</param>
        private void deSerializeGuidArray(ref Guid[] value)
        {
            int length = deSerializeArrayMember(ref value);
            if (length != 0)
            {
                if ((length * sizeof(Guid) + sizeof(int) * 2) == currentSize && (uint)currentSize <= (uint)(int)(end - Read))
                {
                    value = new Guid[length];
                    Read = DeSerialize(Read + sizeof(int) * 2, value);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// Guid反序列化
        /// </summary>
        /// <param name="value">Guid</param>
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref Guid[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerializeGuidArray(ref value);
        }
        /// <summary>
        /// Guid反序列化
        /// </summary>
        /// <param name="value">Guid</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref Guid[] value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) deSerializeGuidArray(ref value);
            else value = null;
        }
        /// <summary>
        /// Guid反序列化
        /// </summary>
        /// <param name="value">Guid</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref Guid? value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0)
            {
                value = *(Guid*)Read;
                Read += sizeof(Guid);
            }
            else value = null;
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
                    value = new Guid?[length];
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
        private void deSerializeGuidArray(ref Guid?[] value)
        {
            int length = deSerializeArrayMember(ref value);
            if (length != 0)
            {
                byte* end = Read + currentSize;
                if ((((length + (31 + 64)) >> 5) << 2) <= currentSize && end <= this.end)
                {
                    value = new Guid?[length];
                    Read = DeSerialize(Read + sizeof(int) * 2, value);
                    if (Read == end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// Guid反序列化
        /// </summary>
        /// <param name="value">Guid</param>
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref Guid?[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerializeGuidArray(ref value);
        }
        /// <summary>
        /// Guid反序列化
        /// </summary>
        /// <param name="value">Guid</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref Guid?[] value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) deSerializeGuidArray(ref value);
            else value = null;
        }
        /// <summary>
        /// 字符串反序列化
        /// </summary>
        /// <param name="value">字符串</param>
        [deSerializeMethod]
        private void deSerialize(ref string value)
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
        /// <summary>
        /// 字符串反序列化
        /// </summary>
        /// <param name="value">字符串</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void deSerializeString(ref string value)
        {
            if ((currentSize = *(int*)Read) == 0)
            {
                value = string.Empty;
                Read += sizeof(int);
                return;
            }
            byte* end = Read + currentSize;
            if ((Read = DeSerialize(Read + sizeof(int), this.end, ref value)) != end) Error(deSerializeState.IndexOutOfRange);
        }
        /// <summary>
        /// 字符串反序列化
        /// </summary>
        /// <param name="value">字符串</param>
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref string value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerializeString(ref value);
        }
        /// <summary>
        /// 字符串反序列化
        /// </summary>
        /// <param name="value">字符串</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref string value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) deSerializeString(ref value);
            else value = null;
        }
        /// <summary>
        /// 字符串反序列化
        /// </summary>
        /// <param name="value">字符串</param>
        [memberDeSerializeMethod]
        [memberMapDeSerializeMethod]
        private void memberDeSerialize(ref subString value)
        {
            if ((currentSize = *(int*)Read) == 0)
            {
                value = string.Empty;
                Read += sizeof(int);
                return;
            }
            byte* end = Read + currentSize;
            string stringValue = null;
            if ((Read = DeSerialize(Read + sizeof(int), this.end, ref stringValue)) == end) value.UnsafeSet(stringValue, 0, stringValue.Length);
            else Error(deSerializeState.IndexOutOfRange);
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
                    value = new string[length];
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
        private void deSerializeStringArray(ref string[] value)
        {
            int length = deSerializeArrayMember(ref value);
            if (length != 0)
            {
                byte* end = Read + currentSize;
                int mapLength = ((length + (31 + 64)) >> 5) << 2;
                if (mapLength <= currentSize && end <= this.end)
                {
                    value = new string[length];
                    arrayMap arrayMap = new arrayMap(Read + sizeof(int) * 2);
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
                    if (Read == end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 字符串反序列化
        /// </summary>
        /// <param name="value">字符串</param>
        [memberDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberDeSerialize(ref string[] value)
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else deSerializeStringArray(ref value);
        }
        /// <summary>
        /// 字符串反序列化
        /// </summary>
        /// <param name="value">字符串</param>
        [memberMapDeSerializeMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapDeSerialize(ref string[] value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) deSerializeStringArray(ref value);
            else value = null;
        }

        /// <summary>
        /// 基类反序列化
        /// </summary>
        /// <param name="deSerializer">二进制数据反序列化</param>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static void baseSerialize<valueType, childType>(indexDeSerializer deSerializer, ref childType value) where childType : valueType
        {
            typeDeSerializer<valueType>.BaseSerialize(deSerializer, ref value);
        }
        /// <summary>
        /// 基类反序列化函数信息
        /// </summary>
        private static readonly MethodInfo baseSerializeMethod = typeof(indexDeSerializer).GetMethod("baseSerialize", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 对象反序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        private void structDeSerialize<valueType>(ref valueType value)
        {
            byte* end = Read + *(int*)Read;
            if (end <= this.end)
            {
                Read += sizeof(int);
                typeDeSerializer<valueType>.DeSerialize(this, ref value);
                if (Read == end) return;
            }
            Error(deSerializeState.IndexOutOfRange);
        }
        /// <summary>
        /// 对象反序列化函数信息
        /// </summary>
        private static readonly MethodInfo structDeSerializeMethod = typeof(indexDeSerializer).GetMethod("structDeSerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 对象反序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void classDeSerialize<valueType>(ref valueType value) where valueType : class
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else structDeSerialize(ref value);
        }
        /// <summary>
        /// 对象反序列化函数信息
        /// </summary>
        private static readonly MethodInfo classDeSerializeMethod = typeof(indexDeSerializer).GetMethod("classDeSerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 对象反序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberMapClassDeSerialize<valueType>(ref valueType value) where valueType : class
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) structDeSerialize(ref value);
            else value = null;
        }
        /// <summary>
        /// 对象反序列化函数信息
        /// </summary>
        private static readonly MethodInfo memberMapClassDeSerializeMethod = typeof(indexDeSerializer).GetMethod("memberMapClassDeSerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        private void structArrayType<valueType>(ref valueType[] array)
        {
            int length = deSerializeArray(ref array);
            if (length != 0)
            {
                if ((length + 1) * sizeof(int) <= (int)(end - Read))
                {
                    array = new valueType[length];
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
        private static readonly MethodInfo structArrayMethod = typeof(indexDeSerializer).GetMethod("structArrayType", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        private void structArray<valueType>(ref valueType[] array)
        {
            int length = deSerializeArrayMember(ref array);
            if (length != 0)
            {
                byte* end = Read + currentSize;
                if ((length + 2) * sizeof(int) <= currentSize && end <= this.end)
                {
                    array = new valueType[length];
                    Read += sizeof(int) * 2;
                    for (int index = 0; index != array.Length; ++index)
                    {
                        typeDeSerializer<valueType>.StructDeSerialize(this, ref array[index]);
                        if (state != deSerializeState.Success) return;
                    }
                    if (Read == end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
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
        private static readonly MethodInfo structArrayMemberMethod = typeof(indexDeSerializer).GetMethod("structArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void structArrayMemberMap<valueType>(ref valueType[] array)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) structArray(ref array);
            else array = null;
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo structArrayMemberMapMethod = typeof(indexDeSerializer).GetMethod("structArrayMemberMap", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        private void nullableArrayType<valueType>(ref Nullable<valueType>[] array) where valueType : struct
        {
            int length = deSerializeArray(ref array);
            if (length != 0)
            {
                int mapLength = ((length + (31 + 32)) >> 5) << 2;
                if (mapLength <= (int)(end - Read))
                {
                    array = new Nullable<valueType>[length];
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
        private static readonly MethodInfo nullableArrayMethod = typeof(indexDeSerializer).GetMethod("nullableArrayType", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        private void nullableArray<valueType>(ref Nullable<valueType>[] array) where valueType : struct
        {
            int length = deSerializeArrayMember(ref array);
            if (length != 0)
            {
                byte* end = Read + currentSize;
                int mapLength = ((length + (31 + 64)) >> 5) << 2;
                if (mapLength <= currentSize && end <= this.end)
                {
                    array = new Nullable<valueType>[length];
                    arrayMap arrayMap = new arrayMap(Read + sizeof(int) * 2);
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
                    if (Read == end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
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
        private static readonly MethodInfo nullableArrayMemberMethod = typeof(indexDeSerializer).GetMethod("nullableArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void nullableArrayMemberMap<valueType>(ref Nullable<valueType>[] array) where valueType : struct
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) nullableArray(ref array);
            else array = null;
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo nullableArrayMemberMapMethod = typeof(indexDeSerializer).GetMethod("nullableArrayMemberMap", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        private void arrayType<valueType>(ref valueType[] array) where valueType : class
        {
            int length = deSerializeArray(ref array);
            if (length != 0)
            {
                int mapLength = ((length + (31 + 32)) >> 5) << 2;
                if (mapLength <= (int)(end - Read))
                {
                    array = new valueType[length];
                    arrayMap arrayMap = new arrayMap(Read + sizeof(int));
                    Read += mapLength;
                    for (int index = 0; index != array.Length; ++index)
                    {
                        if (arrayMap.Next() == 0) array[index] = null;
                        else
                        {
                            typeDeSerializer<valueType>.DeSerialize(this, ref array[index]);
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
        private static readonly MethodInfo arrayMethod = typeof(indexDeSerializer).GetMethod("arrayType", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        private void array<valueType>(ref valueType[] array) where valueType : class
        {
            int length = deSerializeArrayMember(ref array);
            if (length != 0)
            {
                byte* end = Read + currentSize;
                int mapLength = (((length + (31 + 64)) >> 5) << 2);
                if (mapLength <= currentSize && end <= this.end)
                {
                    array = new valueType[length];
                    arrayMap arrayMap = new arrayMap(Read + sizeof(int) * 2);
                    Read += mapLength;
                    for (int index = 0; index != array.Length; ++index)
                    {
                        if (arrayMap.Next() == 0) array[index] = null;
                        else
                        {
                            typeDeSerializer<valueType>.DeSerialize(this, ref array[index]);
                            if (state != deSerializeState.Success) return;
                        }
                    }
                    if (Read == end) return;
                }
                Error(deSerializeState.IndexOutOfRange);
            }
        }
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
        private static readonly MethodInfo arrayMemberMethod = typeof(indexDeSerializer).GetMethod("arrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void arrayMemberMap<valueType>(ref valueType[] array) where valueType : class
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) this.array(ref array);
            else array = null;
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo arrayMemberMapMethod = typeof(indexDeSerializer).GetMethod("arrayMemberMap", BindingFlags.Instance | BindingFlags.NonPublic);
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
        private static readonly MethodInfo nullableDeSerializeMethod = typeof(indexDeSerializer).GetMethod("nullableDeSerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        private void nullableMemberDeSerialize<valueType>(ref Nullable<valueType> value) where valueType : struct
        {
            if (*(int*)Read == fastCSharp.emit.binarySerializer.NullValue)
            {
                Read += sizeof(int);
                value = null;
            }
            else
            {
                valueType newValue = value.HasValue ? value.Value : default(valueType);
                structDeSerialize(ref newValue);
                value = newValue;
            }
        }
        /// <summary>
        /// 对象序列化函数信息
        /// </summary>
        private static readonly MethodInfo nullableMemberDeSerializeMethod = typeof(indexDeSerializer).GetMethod("nullableMemberDeSerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void nullableMemberMapDeSerialize<valueType>(ref Nullable<valueType> value) where valueType : struct
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0)
            {
                valueType newValue = value.HasValue ? value.Value : default(valueType);
                structDeSerialize(ref newValue);
                value = newValue;
            }
            else value = null;
        }
        /// <summary>
        /// 对象序列化函数信息
        /// </summary>
        private static readonly MethodInfo nullableMemberMapDeSerializeMethod = typeof(indexDeSerializer).GetMethod("nullableMemberMapDeSerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组对象序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void subArrayDeSerialize<valueType>(ref subArray<valueType> value)
        {
            valueType[] array = null;
            typeDeSerializer<valueType[]>.DefaultDeSerializer(this, ref array);
            value.UnsafeSet(array, 0, array.Length);
        }
        /// <summary>
        /// 数组对象序列化函数信息
        /// </summary>
        private static readonly MethodInfo subArrayDeSerializeMethod = typeof(indexDeSerializer).GetMethod("subArrayDeSerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 字典序列化
        /// </summary>
        /// <param name="deSerializer"></param>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static void keyValuePairDeSerialize<keyType, valueType>(indexDeSerializer deSerializer, ref KeyValuePair<keyType, valueType> value)
        {
            keyValue<keyType, valueType> keyValue = new keyValue<keyType, valueType>(value.Key, value.Value);
            typeDeSerializer<keyValue<keyType, valueType>>.MemberDeSerialize(deSerializer, ref keyValue);
            value = new KeyValuePair<keyType, valueType>(keyValue.Key, keyValue.Value);
        }
        /// <summary>
        /// 字典序列化函数信息
        /// </summary>
        private static readonly MethodInfo keyValuePairDeSerializeMethod = typeof(indexDeSerializer).GetMethod("keyValuePairDeSerialize", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 字典序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void memberKeyValuePairDeSerialize<keyType, valueType>(ref KeyValuePair<keyType, valueType> value)
        {
            keyValue<keyType, valueType> keyValue = new keyValue<keyType,valueType>(value.Key, value.Value);
            structDeSerialize(ref keyValue);
            value = new KeyValuePair<keyType, valueType>(keyValue.Key, keyValue.Value);
        }
        /// <summary>
        /// 字典序列化函数信息
        /// </summary>
        private static readonly MethodInfo memberKeyValuePairDeSerializeMethod = typeof(indexDeSerializer).GetMethod("memberKeyValuePairDeSerialize", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 字典序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        private void dictionaryArrayDeSerialize<keyType, valueType>(IDictionary<keyType, valueType> value)
        {
            keyType[] keys = null;
            typeDeSerializer<keyType[]>.DefaultDeSerializer(this, ref keys);
            if (state == deSerializeState.Success)
            {
                valueType[] values = null;
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
        private void dictionaryArrayMember<keyType, valueType>(IDictionary<keyType, valueType> value)
        {
            byte* end = Read + *(int*)Read;
            if (end <= this.end)
            {
                Read += sizeof(int);
                keyType[] keys = null;
                typeDeSerializer<keyType[]>.DefaultDeSerializer(this, ref keys);
                if (state != deSerializeState.Success) return;
                valueType[] values = null;
                typeDeSerializer<valueType[]>.DefaultDeSerializer(this, ref values);
                if (state != deSerializeState.Success) return;
                if (Read == end)
                {
                    int index = 0;
                    foreach (valueType nextValue in values) value.Add(keys[index++], nextValue);
                    return;
                }
            }
            Error(deSerializeState.IndexOutOfRange);
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
        private static readonly MethodInfo dictionaryDeSerializeMethod = typeof(indexDeSerializer).GetMethod("dictionaryDeSerialize", BindingFlags.Instance | BindingFlags.NonPublic);
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
            else dictionaryArrayMember(value = dictionary.CreateAny<keyType, valueType>());
        }
        /// <summary>
        /// 字典序列化函数信息
        /// </summary>
        private static readonly MethodInfo dictionaryMemberMethod = typeof(indexDeSerializer).GetMethod("dictionaryMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 字典序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void dictionaryMemberMap<keyType, valueType>(ref Dictionary<keyType, valueType> value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) dictionaryArrayMember(value = dictionary.CreateAny<keyType, valueType>());
            else value = null;
        }
        /// <summary>
        /// 字典序列化函数信息
        /// </summary>
        private static readonly MethodInfo dictionaryMemberMapMethod = typeof(indexDeSerializer).GetMethod("dictionaryMemberMap", BindingFlags.Instance | BindingFlags.NonPublic);
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
        private static readonly MethodInfo sortedDictionaryDeSerializeMethod = typeof(indexDeSerializer).GetMethod("sortedDictionaryDeSerialize", BindingFlags.Instance | BindingFlags.NonPublic);
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
            else dictionaryArrayMember(value = new SortedDictionary<keyType, valueType>());
        }
        /// <summary>
        /// 字典序列化函数信息
        /// </summary>
        private static readonly MethodInfo sortedDictionaryMemberMethod = typeof(indexDeSerializer).GetMethod("sortedDictionaryMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 字典序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void sortedDictionaryMemberMap<keyType, valueType>(ref SortedDictionary<keyType, valueType> value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) dictionaryArrayMember(value = new SortedDictionary<keyType, valueType>());
            else value = null;
        }
        /// <summary>
        /// 字典序列化函数信息
        /// </summary>
        private static readonly MethodInfo sortedDictionaryMemberMapMethod = typeof(indexDeSerializer).GetMethod("sortedDictionaryMemberMap", BindingFlags.Instance | BindingFlags.NonPublic);
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
        private static readonly MethodInfo sortedListDeSerializeMethod = typeof(indexDeSerializer).GetMethod("sortedListDeSerialize", BindingFlags.Instance | BindingFlags.NonPublic);
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
            else dictionaryArrayMember(value = new SortedList<keyType, valueType>());
        }
        /// <summary>
        /// 字典序列化函数信息
        /// </summary>
        private static readonly MethodInfo sortedListMemberMethod = typeof(indexDeSerializer).GetMethod("sortedListMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 字典序列化
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void sortedListMemberMap<keyType, valueType>(ref SortedList<keyType, valueType> value)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) dictionaryArrayMember(value = new SortedList<keyType, valueType>());
            else value = null;
        }
        /// <summary>
        /// 字典序列化函数信息
        /// </summary>
        private static readonly MethodInfo sortedListMemberMapMethod = typeof(indexDeSerializer).GetMethod("sortedListMemberMap", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 集合构造函数反序列化
        /// </summary>
        /// <param name="value">目标数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void arrayConstructor<valueType, argumentType>(ref valueType value)
        {
            argumentType[] values = null;
            typeDeSerializer<argumentType[]>.DefaultDeSerializer(this, ref values);
            if (state == deSerializeState.Success)
            {
                value = pub.arrayConstructor<valueType, argumentType>.Constructor(values);
            }
        }
        /// <summary>
        /// 数组构造反序列化函数信息
        /// </summary>
        private static readonly MethodInfo arrayConstructorMethod = typeof(indexDeSerializer).GetMethod("arrayConstructor", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 集合构造函数反序列化
        /// </summary>
        /// <param name="value">目标数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void listConstructor<valueType, argumentType>(ref valueType value)
        {
            argumentType[] values = null;
            typeDeSerializer<argumentType[]>.DefaultDeSerializer(this, ref values);
            if (state == deSerializeState.Success)
            {
                value = pub.listConstructor<valueType, argumentType>.Constructor(values);
            }
        }
        /// <summary>
        /// 数组构造反序列化函数信息
        /// </summary>
        private static readonly MethodInfo listConstructorMethod = typeof(indexDeSerializer).GetMethod("listConstructor", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 集合构造函数反序列化
        /// </summary>
        /// <param name="value">目标数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void collectionConstructor<valueType, argumentType>(ref valueType value)
        {
            argumentType[] values = null;
            typeDeSerializer<argumentType[]>.DefaultDeSerializer(this, ref values);
            if (state == deSerializeState.Success)
            {
                value = pub.listConstructor<valueType, argumentType>.Constructor(values);
            }
        }
        /// <summary>
        /// 数组构造反序列化函数信息
        /// </summary>
        private static readonly MethodInfo collectionConstructorMethod = typeof(indexDeSerializer).GetMethod("collectionConstructor", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 集合构造函数反序列化
        /// </summary>
        /// <param name="value">目标数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void enumerableConstructor<valueType, argumentType>(ref valueType value)
        {
            argumentType[] values = null;
            typeDeSerializer<argumentType[]>.DefaultDeSerializer(this, ref values);
            if (state == deSerializeState.Success)
            {
                value = pub.listConstructor<valueType, argumentType>.Constructor(values);
            }
        }
        /// <summary>
        /// 数组构造反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumerableConstructorMethod = typeof(indexDeSerializer).GetMethod("enumerableConstructor", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 集合构造函数反序列化
        /// </summary>
        /// <param name="dictionary">目标数据</param>
        private void dictionaryConstructor<dictionaryType, keyType, valueType, argumentType>(ref dictionaryType dictionary) where dictionaryType : IDictionary<keyType, valueType>
        {
            keyType[] keys = null;
            typeDeSerializer<keyType[]>.DefaultDeSerializer(this, ref keys);
            if (state == deSerializeState.Success)
            {
                valueType[] values = null;
                typeDeSerializer<valueType[]>.DefaultDeSerializer(this, ref values);
                if (state == deSerializeState.Success)
                {
                    int index = 0;
                    dictionary = fastCSharp.emit.constructor<dictionaryType>.New();
                    foreach (valueType value in values) dictionary.Add(keys[index++], value);
                }
            }
        }
        /// <summary>
        /// 数组构造反序列化函数信息
        /// </summary>
        private static readonly MethodInfo dictionaryConstructorMethod = typeof(indexDeSerializer).GetMethod("dictionaryConstructor", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举转换函数信息
        /// </summary>
        private static readonly MethodInfo enumByteMemberMethod = typeof(indexDeSerializer).GetMethod("enumByteMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举转换函数信息
        /// </summary>
        private static readonly MethodInfo enumSByteMemberMethod = typeof(indexDeSerializer).GetMethod("enumSByteMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举转换函数信息
        /// </summary>
        private static readonly MethodInfo enumShortMemberMethod = typeof(indexDeSerializer).GetMethod("enumShortMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举转换函数信息
        /// </summary>
        private static readonly MethodInfo enumUShortMemberMethod = typeof(indexDeSerializer).GetMethod("enumUShortMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举转换函数信息
        /// </summary>
        private static readonly MethodInfo enumByteMemberMapMethod = typeof(indexDeSerializer).GetMethod("enumByte", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举转换函数信息
        /// </summary>
        private static readonly MethodInfo enumSByteMemberMapMethod = typeof(indexDeSerializer).GetMethod("enumSByte", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举转换函数信息
        /// </summary>
        private static readonly MethodInfo enumShortMemberMapMethod = typeof(indexDeSerializer).GetMethod("enumShort", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举转换函数信息
        /// </summary>
        private static readonly MethodInfo enumUShortMemberMapMethod = typeof(indexDeSerializer).GetMethod("enumUShort", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举转换函数信息
        /// </summary>
        private static readonly MethodInfo enumIntMethod = typeof(indexDeSerializer).GetMethod("enumInt", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举转换函数信息
        /// </summary>
        private static readonly MethodInfo enumUIntMethod = typeof(indexDeSerializer).GetMethod("enumUInt", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举转换函数信息
        /// </summary>
        private static readonly MethodInfo enumLongMethod = typeof(indexDeSerializer).GetMethod("enumLong", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举转换函数信息
        /// </summary>
        private static readonly MethodInfo enumULongMethod = typeof(indexDeSerializer).GetMethod("enumULong", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumByteArrayType<valueType>(ref valueType[] array)
        {
            int length = deSerializeArray(ref array);
            if (length != 0)
            {
                int dataLength = (length + (3 + sizeof(int))) & (int.MaxValue - 3);
                if (dataLength <= (int)(end - Read))
                {
                    array = new valueType[length];
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
        private static readonly MethodInfo enumByteArrayMethod = typeof(indexDeSerializer).GetMethod("enumByteArrayType", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumByteArray<valueType>(ref valueType[] array)
        {
            int length = deSerializeArrayMember(ref array);
            if (length != 0)
            {
                byte* end = Read + currentSize;
                if (((length + (3 + sizeof(int) * 2)) & (int.MaxValue - 3)) == currentSize && end <= this.end)
                {
                    array = new valueType[length];
                    byte* data = Read + sizeof(int) * 2;
                    for (int index = 0; index != array.Length; array[index++] = pub.enumCast<valueType, byte>.FromInt(*data++)) ;
                    Read = end;
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
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
        private static readonly MethodInfo enumByteArrayMemberMethod = typeof(indexDeSerializer).GetMethod("enumByteArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void enumByteArrayMemberMap<valueType>(ref valueType[] array)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) enumByteArray(ref array);
            else array = null;
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumByteArrayMemberMapMethod = typeof(indexDeSerializer).GetMethod("enumByteArrayMemberMap", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumSByteArrayType<valueType>(ref valueType[] array)
        {
            int length = deSerializeArray(ref array);
            if (length != 0)
            {
                int dataLength = (length + (3 + sizeof(int))) & (int.MaxValue - 3);
                if (dataLength <= (int)(end - Read))
                {
                    array = new valueType[length];
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
        private static readonly MethodInfo enumSByteArrayMethod = typeof(indexDeSerializer).GetMethod("enumSByteArrayType", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumSByteArray<valueType>(ref valueType[] array)
        {
            int length = deSerializeArrayMember(ref array);
            if (length != 0)
            {
                byte* end = Read + currentSize;
                if (((length + (3 + sizeof(int) * 2)) & (int.MaxValue - 3)) == currentSize && end <= this.end)
                {
                    array = new valueType[length];
                    byte* data = Read + sizeof(int) * 2;
                    for (int index = 0; index != array.Length; array[index++] = pub.enumCast<valueType, sbyte>.FromInt((sbyte)*data++)) ;
                    Read = end;
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
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
        private static readonly MethodInfo enumSByteArrayMemberMethod = typeof(indexDeSerializer).GetMethod("enumSByteArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void enumSByteArrayMemberMap<valueType>(ref valueType[] array)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) enumSByteArray(ref array);
            else array = null;
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumSByteArrayMemberMapMethod = typeof(indexDeSerializer).GetMethod("enumSByteArrayMemberMap", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumShortArrayType<valueType>(ref valueType[] array)
        {
            int length = deSerializeArray(ref array);
            if (length != 0)
            {
                int dataLength = ((length << 1) + (3 + sizeof(int))) & (int.MaxValue - 3);
                if (dataLength <= (int)(end - Read))
                {
                    array = new valueType[length];
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
        private static readonly MethodInfo enumShortArrayMethod = typeof(indexDeSerializer).GetMethod("enumShortArrayType", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumShortArray<valueType>(ref valueType[] array)
        {
            int length = deSerializeArrayMember(ref array);
            if (length != 0)
            {
                byte* end = Read + currentSize;
                if ((((length << 1) + (3 + sizeof(int) * 2)) & (int.MaxValue - 3)) == currentSize && end <= this.end)
                {
                    array = new valueType[length];
                    short* data = (short*)(Read + sizeof(int) * 2);
                    for (int index = 0; index != array.Length; array[index++] = pub.enumCast<valueType, short>.FromInt(*data++)) ;
                    Read = end;
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
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
        private static readonly MethodInfo enumShortArrayMemberMethod = typeof(indexDeSerializer).GetMethod("enumShortArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void enumShortArrayMemberMap<valueType>(ref valueType[] array)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) enumShortArray(ref array);
            else array = null;
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumShortArrayMemberMapMethod = typeof(indexDeSerializer).GetMethod("enumShortArrayMemberMap", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumUShortArrayType<valueType>(ref valueType[] array)
        {
            int length = deSerializeArray(ref array);
            if (length != 0)
            {
                int dataLength = ((length << 1) + (3 + sizeof(int))) & (int.MaxValue - 3);
                if (dataLength <= (int)(end - Read))
                {
                    array = new valueType[length];
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
        private static readonly MethodInfo enumUShortArrayMethod = typeof(indexDeSerializer).GetMethod("enumUShortArrayType", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumUShortArray<valueType>(ref valueType[] array)
        {
            int length = deSerializeArrayMember(ref array);
            if (length != 0)
            {
                byte* end = Read + currentSize;
                if ((((length << 1) + (3 + sizeof(int) * 2)) & (int.MaxValue - 3)) == currentSize && end <= this.end)
                {
                    array = new valueType[length];
                    ushort* data = (ushort*)(Read + sizeof(int) * 2);
                    for (int index = 0; index != array.Length; array[index++] = pub.enumCast<valueType, ushort>.FromInt(*data++)) ;
                    Read = end;
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
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
        private static readonly MethodInfo enumUShortArrayMemberMethod = typeof(indexDeSerializer).GetMethod("enumUShortArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void enumUShortArrayMemberMap<valueType>(ref valueType[] array)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) enumUShortArray(ref array);
            else array = null;
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumUShortArrayMemberMapMethod = typeof(indexDeSerializer).GetMethod("enumUShortArrayMemberMap", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumIntArrayType<valueType>(ref valueType[] array)
        {
            int length = deSerializeArray(ref array);
            if (length != 0)
            {
                if ((length + 1) * sizeof(int) <= (int)(end - Read))
                {
                    array = new valueType[length];
                    Read += sizeof(int);
                    for (int index = 0; index != array.Length; Read += sizeof(int)) array[index++] = pub.enumCast<valueType, int>.FromInt(*(int*)Read);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumIntArrayMethod = typeof(indexDeSerializer).GetMethod("enumIntArrayType", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumIntArray<valueType>(ref valueType[] array)
        {
            int length = deSerializeArrayMember(ref array);
            if (length != 0)
            {
                byte* end = Read + currentSize;
                if ((length + 2) * sizeof(int) == currentSize && end <= this.end)
                {
                    array = new valueType[length];
                    Read += sizeof(int) * 2;
                    for (int index = 0; index != array.Length; Read += sizeof(int)) array[index++] = pub.enumCast<valueType, int>.FromInt(*(int*)Read);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
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
        private static readonly MethodInfo enumIntArrayMemberMethod = typeof(indexDeSerializer).GetMethod("enumIntArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void enumIntArrayMemberMap<valueType>(ref valueType[] array)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) enumIntArray(ref array);
            else array = null;
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumIntArrayMemberMapMethod = typeof(indexDeSerializer).GetMethod("enumIntArrayMemberMap", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumUIntArrayType<valueType>(ref valueType[] array)
        {
            int length = deSerializeArray(ref array);
            if (length != 0)
            {
                if ((length + 1) * sizeof(int) <= (int)(end - Read))
                {
                    array = new valueType[length];
                    Read += sizeof(int);
                    for (int index = 0; index != array.Length; Read += sizeof(uint)) array[index++] = pub.enumCast<valueType, uint>.FromInt(*(uint*)Read);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumUIntArrayMethod = typeof(indexDeSerializer).GetMethod("enumUIntArrayType", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumUIntArray<valueType>(ref valueType[] array)
        {
            int length = deSerializeArrayMember(ref array);
            if (length != 0)
            {
                byte* end = Read + currentSize;
                if ((length + 2) * sizeof(int) == currentSize && end <= this.end)
                {
                    array = new valueType[length];
                    Read += sizeof(int) * 2;
                    for (int index = 0; index != array.Length; Read += sizeof(uint)) array[index++] = pub.enumCast<valueType, uint>.FromInt(*(uint*)Read);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
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
        private static readonly MethodInfo enumUIntArrayMemberMethod = typeof(indexDeSerializer).GetMethod("enumUIntArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void enumUIntArrayMemberMap<valueType>(ref valueType[] array)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) enumUIntArray(ref array);
            else array = null;
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumUIntArrayMemberMapMethod = typeof(indexDeSerializer).GetMethod("enumUIntArrayMemberMap", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumLongArrayType<valueType>(ref valueType[] array)
        {
            int length = deSerializeArray(ref array);
            if (length != 0)
            {
                if (length * sizeof(long) + sizeof(int) <= (int)(end - Read))
                {
                    array = new valueType[length];
                    Read += sizeof(int);
                    for (int index = 0; index != array.Length; Read += sizeof(long)) array[index++] = pub.enumCast<valueType, long>.FromInt(*(long*)Read);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumLongArrayMethod = typeof(indexDeSerializer).GetMethod("enumLongArrayType", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumLongArray<valueType>(ref valueType[] array)
        {
            int length = deSerializeArrayMember(ref array);
            if (length != 0)
            {
                byte* end = Read + currentSize;
                if (length * sizeof(long) + sizeof(int) * 2 == currentSize && end <= this.end)
                {
                    array = new valueType[length];
                    Read += sizeof(int) * 2;
                    for (int index = 0; index != array.Length; Read += sizeof(long)) array[index++] = pub.enumCast<valueType, long>.FromInt(*(long*)Read);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
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
        private static readonly MethodInfo enumLongArrayMemberMethod = typeof(indexDeSerializer).GetMethod("enumLongArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void enumLongArrayMemberMap<valueType>(ref valueType[] array)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) enumLongArray(ref array);
            else array = null;
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumLongArrayMemberMapMethod = typeof(indexDeSerializer).GetMethod("enumLongArrayMemberMap", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumULongArrayType<valueType>(ref valueType[] array)
        {
            int length = deSerializeArray(ref array);
            if (length != 0)
            {
                if (length * sizeof(ulong) + sizeof(int) <= (int)(end - Read))
                {
                    array = new valueType[length];
                    Read += sizeof(int);
                    for (int index = 0; index != array.Length; Read += sizeof(ulong)) array[index++] = pub.enumCast<valueType, ulong>.FromInt(*(ulong*)Read);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumULongArrayMethod = typeof(indexDeSerializer).GetMethod("enumULongArrayType", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        private unsafe void enumULongArray<valueType>(ref valueType[] array)
        {
            int length = deSerializeArrayMember(ref array);
            if (length != 0)
            {
                byte* end = Read + currentSize;
                if (length * sizeof(ulong) + sizeof(int) * 2 == currentSize && end <= this.end)
                {
                    array = new valueType[length];
                    Read += sizeof(int) * 2;
                    for (int index = 0; index != array.Length; Read += sizeof(ulong)) array[index++] = pub.enumCast<valueType, ulong>.FromInt(*(ulong*)Read);
                }
                else Error(deSerializeState.IndexOutOfRange);
            }
        }
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
        private static readonly MethodInfo enumULongArrayMemberMethod = typeof(indexDeSerializer).GetMethod("enumULongArrayMember", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举数组序列化
        /// </summary>
        /// <param name="array">枚举数组序列化</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void enumULongArrayMemberMap<valueType>(ref valueType[] array)
        {
            if ((*(uint*)(Read - sizeof(int)) & 0x80000000U) == 0) enumULongArray(ref array);
            else array = null;
        }
        /// <summary>
        /// 数组反序列化函数信息
        /// </summary>
        private static readonly MethodInfo enumULongArrayMemberMapMethod = typeof(indexDeSerializer).GetMethod("enumULongArrayMemberMap", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="data"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType DeSerialize<valueType>(byte[] data, config config = null)
        {
            if (data != null)
            {
                fixed (byte* dataFixed = data) return DeSerialize<valueType>(dataFixed, data.Length, config);
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
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType DeSerialize<valueType>(ref subArray<byte> data, config config = null)
        {
            if (data.length != 0)
            {
                fixed (byte* dataFixed = data.array) return DeSerialize<valueType>(dataFixed + data.startIndex, data.length, config);
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
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType DeSerialize<valueType>(unmanagedStream data, int startIndex = 0, config config = null)
        {
            if (data != null && startIndex >= 0)
            {
                return DeSerialize<valueType>(data.data.Byte + startIndex, data.Length - startIndex, config);
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
        public static valueType DeSerialize<valueType>(byte* data, int size, config config = null)
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
                                    indexDeSerializer deSerializer = typePool<indexDeSerializer>.Pop() ?? new indexDeSerializer();
                                    try
                                    {
                                        return deSerializer.deSerialize<valueType>(data, end, ref value, config) == deSerializeState.Success ? value : default(valueType);
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
                        indexDeSerializer deSerializer = typePool<indexDeSerializer>.Pop() ?? new indexDeSerializer();
                        try
                        {
                            return deSerializer.deSerialize<valueType>(data, data + length, ref value, config) == deSerializeState.Success ? value : default(valueType);
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
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static bool DeSerialize<valueType>(byte[] data, ref valueType value, config config = null)
        {
            if (data != null)
            {
                fixed (byte* dataFixed = data) return DeSerialize<valueType>(dataFixed, data.Length, ref value, config);
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
        public static bool DeSerialize<valueType>(ref subArray<byte> data, ref valueType value, config config = null)
        {
            if (data.length != 0)
            {
                fixed (byte* dataFixed = data.array) return DeSerialize<valueType>(dataFixed + data.startIndex, data.length, ref value, config);
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
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static bool DeSerialize<valueType>(unmanagedStream data, ref valueType value, int startIndex = 0, config config = null)
        {
            if (data != null && startIndex >= 0)
            {
                return DeSerialize<valueType>(data.data.Byte + startIndex, data.Length - startIndex, ref value, config);
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
        public static bool DeSerialize<valueType>(byte* data, int size, ref valueType value, config config = null)
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
                                    indexDeSerializer deSerializer = typePool<indexDeSerializer>.Pop() ?? new indexDeSerializer();
                                    try
                                    {
                                        return deSerializer.deSerialize<valueType>(data, end, ref value, config) == deSerializeState.Success;
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
                        indexDeSerializer deSerializer = typePool<indexDeSerializer>.Pop() ?? new indexDeSerializer();
                        try
                        {
                            return deSerializer.deSerialize<valueType>(data, data + length, ref value, config) == deSerializeState.Success;
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
        static indexDeSerializer()
        {
            deSerializeMethods = dictionary.CreateOnly<Type, MethodInfo>();
            memberDeSerializeMethods = dictionary.CreateOnly<Type, MethodInfo>();
            memberMapDeSerializeMethods = dictionary.CreateOnly<Type, MethodInfo>();
            foreach (MethodInfo method in typeof(indexDeSerializer).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                Type parameterType = null;
                if (method.customAttribute<deSerializeMethod>() != null)
                {
                    deSerializeMethods.Add(parameterType = method.GetParameters()[0].ParameterType.GetElementType(), method);
                }
                if (method.customAttribute<memberDeSerializeMethod>() != null)
                {
                    if (parameterType == null) parameterType = method.GetParameters()[0].ParameterType.GetElementType();
                    try
                    {
                        memberDeSerializeMethods.Add(parameterType, method);
                    }
                    catch
                    {
                        log.Error.Real(parameterType.fullName(), new System.Diagnostics.StackFrame(), false);
                    }
                }
                if (method.customAttribute<memberMapDeSerializeMethod>() != null)
                {
                    memberMapDeSerializeMethods.Add(parameterType ?? method.GetParameters()[0].ParameterType.GetElementType(), method);
                }
            }
        }
    }
}
