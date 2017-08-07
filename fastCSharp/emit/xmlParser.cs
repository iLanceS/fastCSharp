using System;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Collections.Generic;
using fastCSharp.reflection;
using fastCSharp.threading;
using fastCSharp.code;
using System.Threading;
#if NOJIT
#else
using System.Reflection.Emit;
#endif

namespace fastCSharp.emit
{
    /// <summary>
    /// XML解析器
    /// </summary>
    public unsafe sealed class xmlParser
    {
        /// <summary>
        /// 解析状态
        /// </summary>
        public enum parseState : byte
        {
            /// <summary>
            /// 成功
            /// </summary>
            Success,
            /// <summary>
            /// 成员位图类型错误
            /// </summary>
            MemberMap,
            /// <summary>
            /// XML字符串参数为空
            /// </summary>
            NullXml,
            /// <summary>
            /// xml头部解析错误
            /// </summary>
            HeaderError,
            /// <summary>
            /// 没有找到根节点结束标签
            /// </summary>
            NotFoundBootNodeEnd,
            /// <summary>
            /// 没有找到根节点开始标签
            /// </summary>
            NotFoundBootNodeStart,
            /// <summary>
            /// 没有找到名称标签开始符号
            /// </summary>
            NotFoundTagStart,
            /// <summary>
            /// 没有找到匹配的结束标签
            /// </summary>
            NotFoundTagEnd,
            /// <summary>
            /// 属性名称解析失败
            /// </summary>
            NotFoundAttributeName,
            /// <summary>
            /// 属性值解析失败
            /// </summary>
            NotFoundAttributeValue,
            /// <summary>
            /// 没有找到预期数据
            /// </summary>
            NotFoundValue,
            /// <summary>
            /// 没有找到预期数据结束
            /// </summary>
            NotFoundValueEnd,
            /// <summary>
            /// 没有找到预期的CDATA开始
            /// </summary>
            NotFoundCDATAStart,
            /// <summary>
            /// 注释错误
            /// </summary>
            NoteError,
            /// <summary>
            /// 非正常意外结束
            /// </summary>
            CrashEnd,
            /// <summary>
            /// 不支持直接解析 基元类型/可空类型/数组/枚举/指针/字典
            /// </summary>
            NotSupport,
            /// <summary>
            /// 找不到构造函数
            /// </summary>
            NoConstructor,
            /// <summary>
            /// 字符解码失败
            /// </summary>
            DecodeError,
            /// <summary>
            /// 逻辑值解析错误
            /// </summary>
            NotBool,
            /// <summary>
            /// 非数字解析错误
            /// </summary>
            NotNumber,
            /// <summary>
            /// 16进制数字解析错误
            /// </summary>
            NotHex,
            /// <summary>
            /// 字符解析错误
            /// </summary>
            NotChar,
            /// <summary>
            /// 时间解析错误
            /// </summary>
            NotDateTime,
            /// <summary>
            /// Guid解析错误
            /// </summary>
            NotGuid,
            /// <summary>
            /// 数组节点解析错误
            /// </summary>
            NotArrayItem,
            /// <summary>
            /// 没有找到匹配的枚举值
            /// </summary>
            NoFoundEnumValue,
            /// <summary>
            /// 非枚举字符
            /// </summary>
            NotEnumChar,
            /// <summary>
            /// 未知名称节点自定义解析失败
            /// </summary>
            UnknownNameError,
        }
        /// <summary>
        /// 配置参数
        /// </summary>
        public sealed class config
        {
            /// <summary>
            /// 自定义构造函数
            /// </summary>
            public Func<Type, object> Constructor;
            /// <summary>
            /// 成员位图
            /// </summary>
            public memberMap MemberMap;
            /// <summary>
            /// 根节点名称(不能包含非法字符)
            /// </summary>
            public string BootNodeName = "xml";
            /// <summary>
            /// 集合子节点名称(不能包含非法字符)
            /// </summary>
            public string ItemName = "item";
            /// <summary>
            /// 成员选择
            /// </summary>
            public fastCSharp.code.memberFilters MemberFilter = code.memberFilters.Instance;
            /// <summary>
            /// 解析状态
            /// </summary>
            public parseState State { get; internal set; }
            /// <summary>
            /// 是否保存属性索引
            /// </summary>
            public bool IsAttribute;
            /// <summary>
            /// 错误时是否输出JSON字符串
            /// </summary>
            public bool IsOutputError = true;
            /// <summary>
            /// 是否临时字符串(可修改)
            /// </summary>
            public bool IsTempString;
            /// <summary>
            /// 是否强制匹配枚举值
            /// </summary>
            public bool IsMatchEnum;
        }
        /// <summary>
        /// 枚举状态查找器
        /// </summary>
        internal struct stateSearcher
        {
            /// <summary>
            /// XML解析器
            /// </summary>
            private xmlParser parser;
            /// <summary>
            /// 状态集合
            /// </summary>
            private byte* state;
            /// <summary>
            /// ASCII字符查找表
            /// </summary>
            private byte* charsAscii;
            /// <summary>
            /// 特殊字符串查找表
            /// </summary>
            private byte* charStart;
            /// <summary>
            /// 特殊字符串查找表结束位置
            /// </summary>
            private byte* charEnd;
            /// <summary>
            /// 当前状态
            /// </summary>
            private byte* currentState;
            /// <summary>
            /// 特殊字符起始值
            /// </summary>
            private int charIndex;
            /// <summary>
            /// 查询矩阵单位尺寸类型
            /// </summary>
            private byte tableType;
            /// <summary>
            /// 名称查找器
            /// </summary>
            /// <param name="parser">XML解析器</param>
            /// <param name="data">数据起始位置</param>
            internal stateSearcher(xmlParser parser, pointer.reference data)
            {
                this.parser = parser;
                if (data.Data == null)
                {
                    state = charsAscii = charStart = charEnd = currentState = null;
                    charIndex = 0;
                    tableType = 0;
                }
                else
                {
                    int stateCount = *data.Int;
                    currentState = state = data.Byte + sizeof(int);
                    charsAscii = state + stateCount * 3 * sizeof(int);
                    charStart = charsAscii + 128 * sizeof(ushort);
                    charIndex = *(ushort*)charStart;
                    charStart += sizeof(ushort) * 2;
                    charEnd = charStart + *(ushort*)(charStart - sizeof(ushort)) * sizeof(ushort);
                    if (stateCount < 256) tableType = 0;
                    else if (stateCount < 65536) tableType = 1;
                    else tableType = 2;
                }
            }
            /// <summary>
            /// 根据字符串查找目标索引
            /// </summary>
            /// <returns>目标索引,null返回-1</returns>
            internal int SearchEnum()
            {
                if (state != null)
                {
                    if (parser.isCData == 0)
                    {
                        currentState = state;
                        int index = searchEnum();
                        if (parser.state == parseState.Success)
                        {
                            parser.searchValueEnd();
                            return index;
                        }
                    }
                    parser.searchCDataValue();
                    if (parser.state == parseState.Success)
                    {
                        currentState = state;
                        return searchCDataEnum();
                    }
                }
                return -1;
            }
            /// <summary>
            /// 根据字符串查找目标索引
            /// </summary>
            /// <returns>目标索引,null返回-1</returns>
            internal int searchEnum()
            {
                do
                {
                    char* prefix = (char*)(currentState + *(int*)currentState);
                    if (*prefix != 0)
                    {
                        if (parser.nextEnumChar() != *prefix) return -1;
                        while (*++prefix != 0)
                        {
                            if (parser.nextEnumChar() != *prefix) return -1;
                        }
                    }
                    char value = parser.nextEnumChar();
                    if (value == 0) return parser.state == parseState.Success ? *(int*)(currentState + sizeof(int) * 2) : -1;
                    if (*(int*)(currentState + sizeof(int)) == 0) return -1;
                    int index = value < 128 ? (int)*(ushort*)(charsAscii + (value << 1)) : getCharIndex(value);
                    byte* table = currentState + *(int*)(currentState + sizeof(int));
                    if (tableType == 0)
                    {
                        if ((index = *(table + index)) == 0) return -1;
                        currentState = state + index * 3 * sizeof(int);
                    }
                    else if (tableType == 1)
                    {
                        if ((index = (int)*(ushort*)(table + index * sizeof(ushort))) == 0) return -1;
                        currentState = state + index * 3 * sizeof(int);
                    }
                    else
                    {
                        if ((index = *(int*)(table + index * sizeof(int))) == 0) return -1;
                        currentState = state + index;
                    }
                }
                while (true);
            }
            /// <summary>
            /// 根据字符串查找目标索引
            /// </summary>
            /// <returns>目标索引,null返回-1</returns>
            internal int searchCDataEnum()
            {
                do
                {
                    char* prefix = (char*)(currentState + *(int*)currentState);
                    if (*prefix != 0)
                    {
                        if (parser.nextCDataEnumChar() != *prefix) return -1;
                        while (*++prefix != 0)
                        {
                            if (parser.nextCDataEnumChar() != *prefix) return -1;
                        }
                    }
                    char value = parser.nextCDataEnumChar();
                    if (value == 0) return parser.state == parseState.Success ? *(int*)(currentState + sizeof(int) * 2) : -1;
                    if (*(int*)(currentState + sizeof(int)) == 0) return -1;
                    int index = value < 128 ? (int)*(ushort*)(charsAscii + (value << 1)) : getCharIndex(value);
                    byte* table = currentState + *(int*)(currentState + sizeof(int));
                    if (tableType == 0)
                    {
                        if ((index = *(table + index)) == 0) return -1;
                        currentState = state + index * 3 * sizeof(int);
                    }
                    else if (tableType == 1)
                    {
                        if ((index = (int)*(ushort*)(table + index * sizeof(ushort))) == 0) return -1;
                        currentState = state + index * 3 * sizeof(int);
                    }
                    else
                    {
                        if ((index = *(int*)(table + index * sizeof(int))) == 0) return -1;
                        currentState = state + index;
                    }
                }
                while (true);
            }
            /// <summary>
            /// 根据枚举字符串查找目标索引
            /// </summary>
            /// <returns>目标索引,null返回-1</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal int SearchFlagEnum()
            {
                if (parser.isCData == 0)
                {
                    currentState = state;
                    return searchFlagEnum();
                }
                parser.searchCDataValue();
                if (parser.state == parseState.Success)
                {
                    currentState = state;
                    return searchCDataFlagEnum();
                }
                return -1;
            }
            /// <summary>
            /// 根据枚举字符串查找目标索引
            /// </summary>
            /// <returns>目标索引,null返回-1</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal int NextFlagEnum()
            {
                currentState = state;
                return parser.isCData == 0 ? searchFlagEnum() : searchCDataFlagEnum();
            }
            /// <summary>
            /// 根据枚举字符串查找目标索引
            /// </summary>
            /// <returns>目标索引,null返回-1</returns>
            private int searchFlagEnum()
            {
                do
                {
                    char* prefix = (char*)(currentState + *(int*)currentState);
                    if (*prefix != 0)
                    {
                        if (parser.nextEnumChar() != *prefix) return -1;
                        while (*++prefix != 0)
                        {
                            if (parser.nextEnumChar() != *prefix) return -1;
                        }
                    }
                    char value = parser.nextEnumChar();
                    if (value == 0 || value == ',') return parser.state == parseState.Success ? *(int*)(currentState + sizeof(int) * 2) : -1;
                    if (*(int*)(currentState + sizeof(int)) == 0) return -1;
                    int index = value < 128 ? (int)*(ushort*)(charsAscii + (value << 1)) : getCharIndex(value);
                    byte* table = currentState + *(int*)(currentState + sizeof(int));
                    if (tableType == 0)
                    {
                        if ((index = *(table + index)) == 0) return -1;
                        currentState = state + index * 3 * sizeof(int);
                    }
                    else if (tableType == 1)
                    {
                        if ((index = (int)*(ushort*)(table + index * sizeof(ushort))) == 0) return -1;
                        currentState = state + index * 3 * sizeof(int);
                    }
                    else
                    {
                        if ((index = *(int*)(table + index * sizeof(int))) == 0) return -1;
                        currentState = state + index;
                    }
                }
                while (true);
            }
            /// <summary>
            /// 根据枚举字符串查找目标索引
            /// </summary>
            /// <returns>目标索引,null返回-1</returns>
            private int searchCDataFlagEnum()
            {
                do
                {
                    char* prefix = (char*)(currentState + *(int*)currentState);
                    if (*prefix != 0)
                    {
                        if (parser.nextCDataEnumChar() != *prefix) return -1;
                        while (*++prefix != 0)
                        {
                            if (parser.nextCDataEnumChar() != *prefix) return -1;
                        }
                    }
                    char value = parser.nextCDataEnumChar();
                    if (value == 0 || value == ',') return parser.state == parseState.Success ? *(int*)(currentState + sizeof(int) * 2) : -1;
                    if (*(int*)(currentState + sizeof(int)) == 0) return -1;
                    int index = value < 128 ? (int)*(ushort*)(charsAscii + (value << 1)) : getCharIndex(value);
                    byte* table = currentState + *(int*)(currentState + sizeof(int));
                    if (tableType == 0)
                    {
                        if ((index = *(table + index)) == 0) return -1;
                        currentState = state + index * 3 * sizeof(int);
                    }
                    else if (tableType == 1)
                    {
                        if ((index = (int)*(ushort*)(table + index * sizeof(ushort))) == 0) return -1;
                        currentState = state + index * 3 * sizeof(int);
                    }
                    else
                    {
                        if ((index = *(int*)(table + index * sizeof(int))) == 0) return -1;
                        currentState = state + index;
                    }
                }
                while (true);
            }
            /// <summary>
            /// 获取特殊字符索引值
            /// </summary>
            /// <param name="value">特殊字符</param>
            /// <returns>索引值,匹配失败返回0</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private int getCharIndex(char value)
            {
                char* current = fastCSharp.stateSearcher.charsSearcher.GetCharIndex((char*)charStart, (char*)charEnd, value);
                return current == null ? 0 : (charIndex + (int)(current - (char*)charStart));
            }
        }
        /// <summary>
        /// 类型解析器静态信息
        /// </summary>
        internal static class typeParser
        {
            /// <summary>
            /// 获取属性成员集合
            /// </summary>
            /// <param name="properties"></param>
            /// <param name="typeAttribute">类型配置</param>
            /// <returns>属性成员集合</returns>
            public static subArray<xmlSerializer.staticTypeToXmler.propertyInfo> GetProperties(propertyIndex[] properties, xmlSerialize typeAttribute)
            {
                subArray<xmlSerializer.staticTypeToXmler.propertyInfo> values = new subArray<xmlSerializer.staticTypeToXmler.propertyInfo>(properties.Length);
                foreach (propertyIndex property in properties)
                {
                    if (property.Member.CanWrite)
                    {
                        Type type = property.Member.PropertyType;
                        if (!type.IsPointer && (!type.IsArray || type.GetArrayRank() == 1) && !property.IsIgnore)
                        {
                            xmlSerialize.member attribute = property.GetAttribute<xmlSerialize.member>(true, true);
                            if (typeAttribute.IsAllMember ? (attribute == null || attribute.IsSetup) : (attribute != null && attribute.IsSetup))
                            {
                                MethodInfo method = property.Member.GetSetMethod(true);
                                if (method != null && method.GetParameters().Length == 1)
                                {
                                    values.Add(new xmlSerializer.staticTypeToXmler.propertyInfo { Property = property, Method = method, Attribute = attribute });
                                }
                            }
                        }
                    }
                }
                return values;
            }
#if NOJIT
#else
            /// <summary>
            /// 创建解析委托函数
            /// </summary>
            /// <param name="type"></param>
            /// <param name="field"></param>
            /// <returns>解析委托函数</returns>
            public static DynamicMethod CreateDynamicMethod(Type type, FieldInfo field)
            {
                DynamicMethod dynamicMethod = new DynamicMethod("xmlParser" + field.Name, null, new Type[] { typeof(xmlParser), type.MakeByRefType() }, type, true);
                ILGenerator generator = dynamicMethod.GetILGenerator();
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldarg_1);
                if (!type.IsValueType) generator.Emit(OpCodes.Ldind_Ref);
                generator.Emit(OpCodes.Ldflda, field);
                MethodInfo methodInfo = GetMemberMethodInfo(field.FieldType);
                generator.Emit(methodInfo.IsFinal || !methodInfo.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, methodInfo);
                generator.Emit(OpCodes.Ret);
                return dynamicMethod;
            }
            /// <summary>
            /// 创建解析委托函数
            /// </summary>
            /// <param name="type"></param>
            /// <param name="property"></param>
            /// <param name="propertyMethod"></param>
            /// <returns>解析委托函数</returns>
            public static DynamicMethod CreateDynamicMethod(Type type, PropertyInfo property, MethodInfo propertyMethod)
            {
                DynamicMethod dynamicMethod = new DynamicMethod("xmlParser" + property.Name, null, new Type[] { typeof(xmlParser), type.MakeByRefType() }, type, true);
                ILGenerator generator = dynamicMethod.GetILGenerator();
                Type memberType = property.PropertyType;
                LocalBuilder loadMember = generator.DeclareLocal(memberType);
                MethodInfo methodInfo = GetMemberMethodInfo(memberType);
                if (!memberType.IsValueType)
                {
                    generator.Emit(OpCodes.Ldloca_S, loadMember);
                    generator.Emit(OpCodes.Initobj, memberType);
                }
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldloca_S, loadMember);
                generator.Emit(methodInfo.IsFinal || !methodInfo.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, methodInfo);

                generator.Emit(OpCodes.Ldarg_1);
                if (!type.IsValueType) generator.Emit(OpCodes.Ldind_Ref);
                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(propertyMethod.IsFinal || !propertyMethod.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, propertyMethod);
                generator.Emit(OpCodes.Ret);
                return dynamicMethod;
            }
#endif
            /// <summary>
            /// 获取成员转换函数信息
            /// </summary>
            /// <param name="type">成员类型</param>
            /// <returns>成员转换函数信息</returns>
            internal static MethodInfo GetMemberMethodInfo(Type type)
            {
                MethodInfo methodInfo = xmlParser.getParseMethod(type);
                if (methodInfo != null) return methodInfo;
                if (type.IsArray) return typeParser.GetArrayParser(type.GetElementType());
                if (type.IsEnum) return typeParser.GetEnumParser(type);
                if (type.IsGenericType)
                {
                    Type genericType = type.GetGenericTypeDefinition();
                    if (genericType == typeof(Nullable<>))
                    {
                        Type[] parameterTypes = type.GetGenericArguments();
                        return (parameterTypes[0].IsEnum ? nullableEnumParseMethod : nullableParseMethod).MakeGenericMethod(parameterTypes);
                    }
                    if (genericType == typeof(KeyValuePair<,>)) return keyValuePairParseMethod.MakeGenericMethod(type.GetGenericArguments());
                }
                if ((methodInfo = typeParser.GetCustomParser(type)) != null) return methodInfo;
                //if (type.IsAbstract || type.IsInterface) return typeParser.GetNoConstructorParser(type);
                if ((methodInfo = typeParser.GetIEnumerableConstructorParser(type)) != null) return methodInfo;
                if (type.IsValueType) return typeParser.GetValueTypeParser(type);
                return typeParser.GetTypeParser(type);
            }

            /// <summary>
            /// 枚举解析调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> enumParsers = new interlocked.dictionary<Type, MethodInfo>();
            /// <summary>
            /// 获取枚举解析调用函数信息
            /// </summary>
            /// <param name="type">枚举类型</param>
            /// <returns>枚举解析调用函数信息</returns>
            public static MethodInfo GetEnumParser(Type type)
            {
                MethodInfo method;
                if (enumParsers.TryGetValue(type, out method)) return method;
                Type enumType = System.Enum.GetUnderlyingType(type);
                if (fastCSharp.code.typeAttribute.GetAttribute<FlagsAttribute>(type, false, false) == null)
                {
                    if (enumType == typeof(uint)) method = enumUIntMethod.MakeGenericMethod(type);
                    else if (enumType == typeof(byte)) method = enumByteMethod.MakeGenericMethod(type);
                    else if (enumType == typeof(ulong)) method = enumULongMethod.MakeGenericMethod(type);
                    else if (enumType == typeof(ushort)) method = enumUShortMethod.MakeGenericMethod(type);
                    else if (enumType == typeof(long)) method = enumLongMethod.MakeGenericMethod(type);
                    else if (enumType == typeof(short)) method = enumShortMethod.MakeGenericMethod(type);
                    else if (enumType == typeof(sbyte)) method = enumSByteMethod.MakeGenericMethod(type);
                    else method = enumIntMethod.MakeGenericMethod(type);
                }
                else
                {
                    if (enumType == typeof(uint)) method = enumUIntFlagsMethod.MakeGenericMethod(type);
                    else if (enumType == typeof(byte)) method = enumByteFlagsMethod.MakeGenericMethod(type);
                    else if (enumType == typeof(ulong)) method = enumULongFlagsMethod.MakeGenericMethod(type);
                    else if (enumType == typeof(ushort)) method = enumUShortFlagsMethod.MakeGenericMethod(type);
                    else if (enumType == typeof(long)) method = enumLongFlagsMethod.MakeGenericMethod(type);
                    else if (enumType == typeof(short)) method = enumShortFlagsMethod.MakeGenericMethod(type);
                    else if (enumType == typeof(sbyte)) method = enumSByteFlagsMethod.MakeGenericMethod(type);
                    else method = enumIntFlagsMethod.MakeGenericMethod(type);
                }
                enumParsers.Set(type, method);
                return method;
            }
            /// <summary>
            /// 值类型解析调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> valueTypeParsers = new interlocked.dictionary<Type, MethodInfo>();
            /// <summary>
            /// 获取值类型解析调用函数信息
            /// </summary>
            /// <param name="type">数据类型</param>
            /// <returns>值类型解析调用函数信息</returns>
            public static MethodInfo GetValueTypeParser(Type type)
            {
                MethodInfo method;
                if (valueTypeParsers.TryGetValue(type, out method)) return method;
                Type nullType = type.nullableType();
                if (nullType == null) method = structParseMethod.MakeGenericMethod(type);
                else method = nullableParseMethod.MakeGenericMethod(nullType);
                valueTypeParsers.Set(type, method);
                return method;
            }
            /// <summary>
            /// 引用类型解析调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> typeParsers = new interlocked.dictionary<Type, MethodInfo>();
            /// <summary>
            /// 获取引用类型解析调用函数信息
            /// </summary>
            /// <param name="type">数据类型</param>
            /// <returns>引用类型解析调用函数信息</returns>
            public static MethodInfo GetTypeParser(Type type)
            {
                MethodInfo method;
                if (typeParsers.TryGetValue(type, out method)) return method;
                //if (type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, nullValue<Type>.Array, null) == null) method = noConstructorMethod.MakeGenericMethod(type);
                //else
                    method = typeParseMethod.MakeGenericMethod(type);
                typeParsers.Set(type, method);
                return method;
            }
            ///// <summary>
            ///// 缺少构造函数解析调用函数信息集合
            ///// </summary>
            //private static readonly interlocked.dictionary<Type, MethodInfo> noConstructorParsers = new interlocked.dictionary<Type, MethodInfo>();
            ///// <summary>
            ///// 获取缺少构造函数委托调用函数信息
            ///// </summary>
            ///// <param name="type">数据类型</param>
            ///// <returns>缺少构造函数委托调用函数信息</returns>
            //public static MethodInfo GetNoConstructorParser(Type type)
            //{
            //    MethodInfo method;
            //    if (noConstructorParsers.TryGetValue(type, out method)) return method;
            //    method = noConstructorMethod.MakeGenericMethod(type);
            //    noConstructorParsers.Set(type, method);
            //    return method;
            //}
            /// <summary>
            /// 获取枚举构造调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> enumerableConstructorParsers = new interlocked.dictionary<Type, MethodInfo>();
            /// <summary>
            /// 获取枚举构造调用函数信息
            /// </summary>
            /// <param name="type">集合类型</param>
            /// <returns>枚举构造调用函数信息</returns>
            public static MethodInfo GetIEnumerableConstructorParser(Type type)
            {
                MethodInfo method;
                if (enumerableConstructorParsers.TryGetValue(type, out method)) return method;
                foreach (Type interfaceType in type.GetInterfaces())
                {
                    if (interfaceType.IsGenericType)
                    {
                        Type genericType = interfaceType.GetGenericTypeDefinition();
                        if (genericType == typeof(IEnumerable<>))
                        {
                            Type[] parameters = interfaceType.GetGenericArguments();
                            Type argumentType = parameters[0];
                            parameters[0] = typeof(IList<>).MakeGenericType(argumentType);
                            ConstructorInfo constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, parameters, null);
                            if (constructorInfo != null)
                            {
                                method = listConstructorMethod.MakeGenericMethod(type, argumentType);
                                break;
                            }
                            parameters[0] = typeof(ICollection<>).MakeGenericType(argumentType);
                            constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, parameters, null);
                            if (constructorInfo != null)
                            {
                                method = collectionConstructorMethod.MakeGenericMethod(type, argumentType);
                                break;
                            }
                            parameters[0] = typeof(IEnumerable<>).MakeGenericType(argumentType);
                            constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, parameters, null);
                            if (constructorInfo != null)
                            {
                                method = enumerableConstructorMethod.MakeGenericMethod(type, argumentType);
                                break;
                            }
                            parameters[0] = argumentType.MakeArrayType();
                            constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, parameters, null);
                            if (constructorInfo != null)
                            {
                                method = arrayConstructorMethod.MakeGenericMethod(type, argumentType);
                                break;
                            }
                        }
                        else if (genericType == typeof(IDictionary<,>))
                        {
                            ConstructorInfo constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { interfaceType }, null);
                            if (constructorInfo != null)
                            {
                                Type[] parameters = interfaceType.GetGenericArguments();
                                method = dictionaryConstructorMethod.MakeGenericMethod(type, parameters[0], parameters[1]);
                                break;
                            }
                        }
                    }
                }
                enumerableConstructorParsers.Set(type, method);
                return method;
            }
            /// <summary>
            /// 数组解析调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> arrayParsers = new interlocked.dictionary<Type, MethodInfo>();
            /// <summary>
            /// 获取数组解析委托调用函数信息
            /// </summary>
            /// <param name="type">数组类型</param>
            /// <returns>数组解析委托调用函数信息</returns>
            public static MethodInfo GetArrayParser(Type type)
            {
                MethodInfo method;
                if (arrayParsers.TryGetValue(type, out method)) return method;
                arrayParsers.Set(type, method = arrayMethod.MakeGenericMethod(type));
                return method;
            }
            /// <summary>
            /// 自定义解析调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> customParsers = new interlocked.dictionary<Type, MethodInfo>();
            /// <summary>
            /// 自定义解析委托调用函数信息
            /// </summary>
            /// <param name="type">数据类型</param>
            /// <returns>自定义解析委托调用函数信息</returns>
            public static MethodInfo GetCustomParser(Type type)
            {
                MethodInfo method;
                if (customParsers.TryGetValue(type, out method)) return method;
                Type refType = type.MakeByRefType();
                foreach (fastCSharp.code.attributeMethod methodInfo in fastCSharp.code.attributeMethod.GetStatic(type))
                {
                    if (methodInfo.Method.ReturnType == typeof(void))
                    {
                        ParameterInfo[] parameters = methodInfo.Method.GetParameters();
                        if (parameters.Length == 2 && parameters[0].ParameterType == typeof(xmlParser) && parameters[1].ParameterType == refType)
                        {
                            if (methodInfo.GetAttribute<xmlSerialize.custom>(true) != null)
                            {
                                method = methodInfo.Method;
                                break;
                            }
                        }
                    }
                }
                customParsers.Set(type, method);
                return method;
            }

            /// <summary>
            /// 泛型定义类型成员名称查找数据
            /// </summary>
            private static readonly interlocked.dictionary<Type, pointer.size> genericDefinitionMemberSearchers = new interlocked.dictionary<Type, pointer.size>();
            /// <summary>
            /// 泛型定义类型成员名称查找数据创建锁
            /// </summary>
            private static readonly object genericDefinitionMemberSearcherCreateLock = new object();
            /// <summary>
            /// 获取泛型定义成员名称查找数据
            /// </summary>
            /// <param name="type">泛型定义类型</param>
            /// <param name="names">成员名称集合</param>
            /// <returns>泛型定义成员名称查找数据</returns>
            internal static pointer.size GetGenericDefinitionMemberSearcher(Type type, string[] names)
            {
                pointer.size data;
                if (genericDefinitionMemberSearchers.TryGetValue(type = type.GetGenericTypeDefinition(), out data)) return data;
                Monitor.Enter(genericDefinitionMemberSearcherCreateLock);
                if (genericDefinitionMemberSearchers.TryGetValue(type, out data))
                {
                    Monitor.Exit(genericDefinitionMemberSearcherCreateLock);
                    return data;
                }
                try
                {
                    genericDefinitionMemberSearchers.Set(type, data = fastCSharp.stateSearcher.charsSearcher.Create(names, true));
                }
                finally { Monitor.Exit(genericDefinitionMemberSearcherCreateLock); }
                return data;
            }
        }
        /// <summary>
        /// 类型解析器
        /// </summary>
        /// <typeparam name="valueType">目标类型</typeparam>
        internal static class typeParser<valueType>
        {
            /// <summary>
            /// 枚举值解析
            /// </summary>
            internal class enumBase : enumBase<valueType>
            {
                /// <summary>
                /// 枚举值解析
                /// </summary>
                /// <param name="parser">XML解析器</param>
                /// <param name="value">目标数据</param>
                protected static void parse(xmlParser parser, ref valueType value)
                {
                    int index = new stateSearcher(parser, enumSearcher).SearchEnum();
                    if (index != -1) value = enumValues[index];
                    else if (parser.Config.IsMatchEnum) parser.state = parseState.NoFoundEnumValue;
                }
                /// <summary>
                /// 枚举值解析
                /// </summary>
                /// <param name="parser">XML解析器</param>
                /// <param name="value"></param>
                /// <param name="searcher">名称查找</param>
                /// <param name="index">第一个枚举索引</param>
                /// <param name="nextIndex">第二个枚举索引</param>
                protected static void getIndex(xmlParser parser, ref valueType value, ref stateSearcher searcher, out int index, ref int nextIndex)
                {
                    if ((index = searcher.SearchFlagEnum()) == -1)
                    {
                        if (parser.Config.IsMatchEnum)
                        {
                            parser.state = parseState.NoFoundEnumValue;
                            return;
                        }
                        else
                        {
                            do
                            {
                                if (parser.isNextFlagEnum() == 0) return;
                            }
                            while ((index = searcher.SearchFlagEnum()) == -1);
                        }
                    }
                    do
                    {
                        if (parser.isNextFlagEnum() == 0)
                        {
                            value = enumValues[index];
                            return;
                        }
                        if ((nextIndex = searcher.SearchFlagEnum()) != -1) break;
                    }
                    while (true);
                }
            }
            /// <summary>
            /// 枚举值解析
            /// </summary>
            internal sealed class enumByte : enumBase
            {
                /// <summary>
                /// 枚举值集合
                /// </summary>
                private static readonly pointer.reference enumInts;
                /// <summary>
                /// 枚举值解析
                /// </summary>
                /// <param name="parser">XML解析器</param>
                /// <param name="value">目标数据</param>
                public static void Parse(xmlParser parser, ref valueType value)
                {
                    if (parser.isEnumNumber())
                    {
                        byte intValue = 0;
                        parser.parseNumber(ref intValue);
                        value = pub.enumCast<valueType, byte>.FromInt(intValue);
                    }
                    else if (parser.state == parseState.Success) parse(parser, ref value);
                }
                /// <summary>
                /// 枚举值解析
                /// </summary>
                /// <param name="parser">XML解析器</param>
                /// <param name="value">目标数据</param>
                public static void ParseFlags(xmlParser parser, ref valueType value)
                {
                    if (parser.isEnumNumber())
                    {
                        byte intValue = 0;
                        parser.parseNumber(ref intValue);
                        value = pub.enumCast<valueType, byte>.FromInt(intValue);
                    }
                    else if (parser.state == parseState.Success)
                    {
                        if (enumSearcher.Data == null)
                        {
                            if (parser.Config.IsMatchEnum) parser.state = parseState.NoFoundEnumValue;
                            else parser.ignoreSearchValue();
                        }
                        else
                        {
                            int index, nextIndex = -1;
                            stateSearcher searcher = new stateSearcher(parser, enumSearcher);
                            getIndex(parser, ref value, ref searcher, out index, ref nextIndex);
                            if (nextIndex != -1)
                            {
                                byte intValue = enumInts.Byte[index];
                                intValue |= enumInts.Byte[nextIndex];
                                while (parser.isNextFlagEnum() != 0)
                                {
                                    if ((index = searcher.NextFlagEnum()) != -1) intValue |= enumInts.Byte[index];
                                }
                                value = pub.enumCast<valueType, byte>.FromInt(intValue);
                            }
                        }
                    }
                }
                static enumByte()
                {
                    enumInts = unmanaged.GetStatic(enumValues.Length * sizeof(byte), false).Reference;
                    byte* data = enumInts.Byte;
                    foreach (valueType value in enumValues) *(byte*)data++ = pub.enumCast<valueType, byte>.ToInt(value);
                }
            }
            /// <summary>
            /// 枚举值解析
            /// </summary>
            internal sealed class enumSByte : enumBase
            {
                /// <summary>
                /// 枚举值集合
                /// </summary>
                private static readonly pointer.reference enumInts;
                /// <summary>
                /// 枚举值解析
                /// </summary>
                /// <param name="parser">XML解析器</param>
                /// <param name="value">目标数据</param>
                public static void Parse(xmlParser parser, ref valueType value)
                {
                    if (parser.isEnumNumberFlag())
                    {
                        sbyte intValue = 0;
                        parser.parseNumber(ref intValue);
                        value = pub.enumCast<valueType, sbyte>.FromInt(intValue);
                    }
                    else if (parser.state == parseState.Success) parse(parser, ref value);
                }
                /// <summary>
                /// 枚举值解析
                /// </summary>
                /// <param name="parser">XML解析器</param>
                /// <param name="value">目标数据</param>
                public static void ParseFlags(xmlParser parser, ref valueType value)
                {
                    if (parser.isEnumNumberFlag())
                    {
                        sbyte intValue = 0;
                        parser.parseNumber(ref intValue);
                        value = pub.enumCast<valueType, sbyte>.FromInt(intValue);
                    }
                    else if (parser.state == parseState.Success)
                    {
                        if (enumSearcher.Data == null)
                        {
                            if (parser.Config.IsMatchEnum) parser.state = parseState.NoFoundEnumValue;
                            else parser.ignoreSearchValue();
                        }
                        else
                        {
                            int index, nextIndex = -1;
                            stateSearcher searcher = new stateSearcher(parser, enumSearcher);
                            getIndex(parser, ref value, ref searcher, out index, ref nextIndex);
                            if (nextIndex != -1)
                            {
                                byte intValue = (byte)enumInts.SByte[index];
                                intValue |= (byte)enumInts.SByte[nextIndex];
                                while (parser.isNextFlagEnum() != 0)
                                {
                                    if ((index = searcher.NextFlagEnum()) != -1) intValue |= (byte)enumInts.SByte[index];
                                }
                                value = pub.enumCast<valueType, sbyte>.FromInt((sbyte)intValue);
                            }
                        }
                    }
                }
                static enumSByte()
                {
                    enumInts = unmanaged.GetStatic(enumValues.Length * sizeof(sbyte), false).Reference;
                    sbyte* data = enumInts.SByte;
                    foreach (valueType value in enumValues) *(sbyte*)data++ = pub.enumCast<valueType, sbyte>.ToInt(value);
                }
            }
            /// <summary>
            /// 枚举值解析
            /// </summary>
            internal sealed class enumShort : enumBase
            {
                /// <summary>
                /// 枚举值集合
                /// </summary>
                private static readonly pointer.reference enumInts;
                /// <summary>
                /// 枚举值解析
                /// </summary>
                /// <param name="parser">XML解析器</param>
                /// <param name="value">目标数据</param>
                public static void Parse(xmlParser parser, ref valueType value)
                {
                    if (parser.isEnumNumberFlag())
                    {
                        short intValue = 0;
                        parser.parseNumber(ref intValue);
                        value = pub.enumCast<valueType, short>.FromInt(intValue);
                    }
                    else if (parser.state == parseState.Success) parse(parser, ref value);
                }
                /// <summary>
                /// 枚举值解析
                /// </summary>
                /// <param name="parser">XML解析器</param>
                /// <param name="value">目标数据</param>
                public static void ParseFlags(xmlParser parser, ref valueType value)
                {
                    if (parser.isEnumNumberFlag())
                    {
                        short intValue = 0;
                        parser.parseNumber(ref intValue);
                        value = pub.enumCast<valueType, short>.FromInt(intValue);
                    }
                    else if (parser.state == parseState.Success)
                    {
                        if (enumSearcher.Data == null)
                        {
                            if (parser.Config.IsMatchEnum) parser.state = parseState.NoFoundEnumValue;
                            else parser.ignoreSearchValue();
                        }
                        else
                        {
                            int index, nextIndex = -1;
                            stateSearcher searcher = new stateSearcher(parser, enumSearcher);
                            getIndex(parser, ref value, ref searcher, out index, ref nextIndex);
                            if (nextIndex != -1)
                            {
                                ushort intValue = (ushort)enumInts.Short[index];
                                intValue |= (ushort)enumInts.Short[nextIndex];
                                while (parser.isNextFlagEnum() != 0)
                                {
                                    if ((index = searcher.NextFlagEnum()) != -1) intValue |= (ushort)enumInts.Short[index];
                                }
                                value = pub.enumCast<valueType, short>.FromInt((short)intValue);
                            }
                        }
                    }
                }
                static enumShort()
                {
                    enumInts = unmanaged.GetStatic(enumValues.Length * sizeof(short), false).Reference;
                    short* data = enumInts.Short;
                    foreach (valueType value in enumValues) *(short*)data++ = pub.enumCast<valueType, short>.ToInt(value);
                }
            }
            /// <summary>
            /// 枚举值解析
            /// </summary>
            internal sealed class enumUShort : enumBase
            {
                /// <summary>
                /// 枚举值集合
                /// </summary>
                private static readonly pointer.reference enumInts;
                /// <summary>
                /// 枚举值解析
                /// </summary>
                /// <param name="parser">XML解析器</param>
                /// <param name="value">目标数据</param>
                public static void Parse(xmlParser parser, ref valueType value)
                {
                    if (parser.isEnumNumber())
                    {
                        ushort intValue = 0;
                        parser.parseNumber(ref intValue);
                        value = pub.enumCast<valueType, ushort>.FromInt(intValue);
                    }
                    else if (parser.state == parseState.Success) parse(parser, ref value);
                }
                /// <summary>
                /// 枚举值解析
                /// </summary>
                /// <param name="parser">XML解析器</param>
                /// <param name="value">目标数据</param>
                public static void ParseFlags(xmlParser parser, ref valueType value)
                {
                    if (parser.isEnumNumber())
                    {
                        ushort intValue = 0;
                        parser.parseNumber(ref intValue);
                        value = pub.enumCast<valueType, ushort>.FromInt(intValue);
                    }
                    else if (parser.state == parseState.Success)
                    {
                        if (enumSearcher.Data == null)
                        {
                            if (parser.Config.IsMatchEnum) parser.state = parseState.NoFoundEnumValue;
                            else parser.ignoreSearchValue();
                        }
                        else
                        {
                            int index, nextIndex = -1;
                            stateSearcher searcher = new stateSearcher(parser, enumSearcher);
                            getIndex(parser, ref value, ref searcher, out index, ref nextIndex);
                            if (nextIndex != -1)
                            {
                                ushort intValue = enumInts.UShort[index];
                                intValue |= enumInts.UShort[nextIndex];
                                while (parser.isNextFlagEnum() != 0)
                                {
                                    if ((index = searcher.NextFlagEnum()) != -1) intValue |= enumInts.UShort[index];
                                }
                                value = pub.enumCast<valueType, ushort>.FromInt(intValue);
                            }
                        }
                    }
                }
                static enumUShort()
                {
                    enumInts = unmanaged.GetStatic(enumValues.Length * sizeof(ushort), false).Reference;
                    ushort* data = enumInts.UShort;
                    foreach (valueType value in enumValues) *(ushort*)data++ = pub.enumCast<valueType, ushort>.ToInt(value);
                }
            }
            /// <summary>
            /// 枚举值解析
            /// </summary>
            internal sealed class enumInt : enumBase
            {
                /// <summary>
                /// 枚举值集合
                /// </summary>
                private static readonly pointer.reference enumInts;
                /// <summary>
                /// 枚举值解析
                /// </summary>
                /// <param name="parser">XML解析器</param>
                /// <param name="value">目标数据</param>
                public static void Parse(xmlParser parser, ref valueType value)
                {
                    if (parser.isEnumNumberFlag())
                    {
                        int intValue = 0;
                        parser.parseNumber(ref intValue);
                        value = pub.enumCast<valueType, int>.FromInt(intValue);
                    }
                    else if (parser.state == parseState.Success) parse(parser, ref value);
                }
                /// <summary>
                /// 枚举值解析
                /// </summary>
                /// <param name="parser">XML解析器</param>
                /// <param name="value">目标数据</param>
                public static void ParseFlags(xmlParser parser, ref valueType value)
                {
                    if (parser.isEnumNumberFlag())
                    {
                        int intValue = 0;
                        parser.parseNumber(ref intValue);
                        value = pub.enumCast<valueType, int>.FromInt(intValue);
                    }
                    else if (parser.state == parseState.Success)
                    {
                        if (enumSearcher.Data == null)
                        {
                            if (parser.Config.IsMatchEnum) parser.state = parseState.NoFoundEnumValue;
                            else parser.ignoreSearchValue();
                        }
                        else
                        {
                            int index, nextIndex = -1;
                            stateSearcher searcher = new stateSearcher(parser, enumSearcher);
                            getIndex(parser, ref value, ref searcher, out index, ref nextIndex);
                            if (nextIndex != -1)
                            {
                                int intValue = enumInts.Int[index];
                                intValue |= enumInts.Int[nextIndex];
                                while (parser.isNextFlagEnum() != 0)
                                {
                                    if ((index = searcher.NextFlagEnum()) != -1) intValue |= enumInts.Int[index];
                                }
                                value = pub.enumCast<valueType, int>.FromInt(intValue);
                            }
                        }
                    }
                }
                static enumInt()
                {
                    enumInts = unmanaged.GetStatic(enumValues.Length * sizeof(int), false).Reference;
                    int* data = enumInts.Int;
                    foreach (valueType value in enumValues) *(int*)data++ = pub.enumCast<valueType, int>.ToInt(value);
                }
            }
            /// <summary>
            /// 枚举值解析
            /// </summary>
            internal sealed class enumUInt : enumBase
            {
                /// <summary>
                /// 枚举值集合
                /// </summary>
                private static readonly pointer.reference enumInts;
                /// <summary>
                /// 枚举值解析
                /// </summary>
                /// <param name="parser">XML解析器</param>
                /// <param name="value">目标数据</param>
                public static void Parse(xmlParser parser, ref valueType value)
                {
                    if (parser.isEnumNumber())
                    {
                        uint intValue = 0;
                        parser.parseNumber(ref intValue);
                        value = pub.enumCast<valueType, uint>.FromInt(intValue);
                    }
                    else if (parser.state == parseState.Success) parse(parser, ref value);
                }
                /// <summary>
                /// 枚举值解析
                /// </summary>
                /// <param name="parser">XML解析器</param>
                /// <param name="value">目标数据</param>
                public static void ParseFlags(xmlParser parser, ref valueType value)
                {
                    if (parser.isEnumNumber())
                    {
                        uint intValue = 0;
                        parser.parseNumber(ref intValue);
                        value = pub.enumCast<valueType, uint>.FromInt(intValue);
                    }
                    else if (parser.state == parseState.Success)
                    {
                        if (enumSearcher.Data == null)
                        {
                            if (parser.Config.IsMatchEnum) parser.state = parseState.NoFoundEnumValue;
                            else parser.ignoreSearchValue();
                        }
                        else
                        {
                            int index, nextIndex = -1;
                            stateSearcher searcher = new stateSearcher(parser, enumSearcher);
                            getIndex(parser, ref value, ref searcher, out index, ref nextIndex);
                            if (nextIndex != -1)
                            {
                                uint intValue = enumInts.UInt[index];
                                intValue |= enumInts.UInt[nextIndex];
                                while (parser.isNextFlagEnum() != 0)
                                {
                                    if ((index = searcher.NextFlagEnum()) != -1) intValue |= enumInts.UInt[index];
                                }
                                value = pub.enumCast<valueType, uint>.FromInt(intValue);
                            }
                        }
                    }
                }
                static enumUInt()
                {
                    enumInts = unmanaged.GetStatic(enumValues.Length * sizeof(uint), false).Reference;
                    uint* data = enumInts.UInt;
                    foreach (valueType value in enumValues) *(uint*)data++ = pub.enumCast<valueType, uint>.ToInt(value);
                }
            }
            /// <summary>
            /// 枚举值解析
            /// </summary>
            internal sealed class enumLong : enumBase
            {
                /// <summary>
                /// 枚举值集合
                /// </summary>
                private static readonly pointer.reference enumInts;
                /// <summary>
                /// 枚举值解析
                /// </summary>
                /// <param name="parser">XML解析器</param>
                /// <param name="value">目标数据</param>
                public static void Parse(xmlParser parser, ref valueType value)
                {
                    if (parser.isEnumNumberFlag())
                    {
                        long intValue = 0;
                        parser.parseNumber(ref intValue);
                        value = pub.enumCast<valueType, long>.FromInt(intValue);
                    }
                    else if (parser.state == parseState.Success) parse(parser, ref value);
                }
                /// <summary>
                /// 枚举值解析
                /// </summary>
                /// <param name="parser">XML解析器</param>
                /// <param name="value">目标数据</param>
                public static void ParseFlags(xmlParser parser, ref valueType value)
                {
                    if (parser.isEnumNumberFlag())
                    {
                        long intValue = 0;
                        parser.parseNumber(ref intValue);
                        value = pub.enumCast<valueType, long>.FromInt(intValue);
                    }
                    else if (parser.state == parseState.Success)
                    {
                        if (enumSearcher.Data == null)
                        {
                            if (parser.Config.IsMatchEnum) parser.state = parseState.NoFoundEnumValue;
                            else parser.ignoreSearchValue();
                        }
                        else
                        {
                            int index, nextIndex = -1;
                            stateSearcher searcher = new stateSearcher(parser, enumSearcher);
                            getIndex(parser, ref value, ref searcher, out index, ref nextIndex);
                            if (nextIndex != -1)
                            {
                                long intValue = enumInts.Long[index];
                                intValue |= enumInts.Long[nextIndex];
                                while (parser.isNextFlagEnum() != 0)
                                {
                                    if ((index = searcher.NextFlagEnum()) != -1) intValue |= enumInts.Long[index];
                                }
                                value = pub.enumCast<valueType, long>.FromInt(intValue);
                            }
                        }
                    }
                }
                static enumLong()
                {
                    enumInts = unmanaged.GetStatic(enumValues.Length * sizeof(long), false).Reference;
                    long* data = enumInts.Long;
                    foreach (valueType value in enumValues) *(long*)data++ = pub.enumCast<valueType, long>.ToInt(value);
                }
            }
            /// <summary>
            /// 枚举值解析
            /// </summary>
            internal sealed class enumULong : enumBase
            {
                /// <summary>
                /// 枚举值集合
                /// </summary>
                private static readonly pointer.reference enumInts;
                /// <summary>
                /// 枚举值解析
                /// </summary>
                /// <param name="parser">XML解析器</param>
                /// <param name="value">目标数据</param>
                public static void Parse(xmlParser parser, ref valueType value)
                {
                    if (parser.isEnumNumber())
                    {
                        ulong intValue = 0;
                        parser.parseNumber(ref intValue);
                        value = pub.enumCast<valueType, ulong>.FromInt(intValue);
                    }
                    else if (parser.state == parseState.Success) parse(parser, ref value);
                }
                /// <summary>
                /// 枚举值解析
                /// </summary>
                /// <param name="parser">XML解析器</param>
                /// <param name="value">目标数据</param>
                public static void ParseFlags(xmlParser parser, ref valueType value)
                {
                    if (parser.isEnumNumber())
                    {
                        ulong intValue = 0;
                        parser.parseNumber(ref intValue);
                        value = pub.enumCast<valueType, ulong>.FromInt(intValue);
                    }
                    else if (parser.state == parseState.Success)
                    {
                        if (enumSearcher.Data == null)
                        {
                            if (parser.Config.IsMatchEnum) parser.state = parseState.NoFoundEnumValue;
                            else parser.ignoreSearchValue();
                        }
                        else
                        {
                            int index, nextIndex = -1;
                            stateSearcher searcher = new stateSearcher(parser, enumSearcher);
                            getIndex(parser, ref value, ref searcher, out index, ref nextIndex);
                            if (nextIndex != -1)
                            {
                                ulong intValue = enumInts.ULong[index];
                                intValue |= enumInts.ULong[nextIndex];
                                while (parser.isNextFlagEnum() != 0)
                                {
                                    if ((index = searcher.NextFlagEnum()) != -1) intValue |= enumInts.ULong[index];
                                }
                                value = pub.enumCast<valueType, ulong>.FromInt(intValue);
                            }
                        }
                    }
                }
                static enumULong()
                {
                    enumInts = unmanaged.GetStatic(enumValues.Length * sizeof(ulong), false).Reference;
                    ulong* data = enumInts.ULong;
                    foreach (valueType value in enumValues) *(ulong*)data++ = pub.enumCast<valueType, ulong>.ToInt(value);
                }
            }
            /// <summary>
            /// 成员解析器过滤
            /// </summary>
            private struct tryParseFilter
            {
                /// <summary>
                /// 成员解析器
                /// </summary>
                public tryParse TryParse;
                /// <summary>
                /// 集合子节点名称
                /// </summary>
                public string ItemName;
                /// <summary>
                /// 成员位图索引
                /// </summary>
                public int MemberMapIndex;
                /// <summary>
                /// 成员选择
                /// </summary>
                public fastCSharp.code.memberFilters Filter;
#if NOJIT
                /// <summary>
                /// 成员解析器
                /// </summary>
                /// <param name="parser">XML解析器</param>
                /// <param name="value">目标数据</param>
                /// <param name="parameters"></param>
                /// <returns>是否存在下一个数据</returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public int Call(xmlParser parser, ref valueType value, ref object[] parameters)
#else
                /// <summary>
                /// 成员解析器
                /// </summary>
                /// <param name="parser">XML解析器</param>
                /// <param name="value">目标数据</param>
                /// <returns>是否存在下一个数据</returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public int Call(xmlParser parser, ref valueType value)
#endif
                {
                    parser.itemName = ItemName;
                    if ((parser.Config.MemberFilter & Filter) == Filter)
                    {
#if NOJIT
                        parser.ReflectionParameter = parameters ?? (parameters = new object[1]);
#endif
                        TryParse(parser, ref value);
                        return parser.state == parseState.Success ? 1 : 0;
                    }
                    return parser.ignoreValue();
                }
#if NOJIT
                /// <summary>
                /// 成员解析器
                /// </summary>
                /// <param name="parser">XML解析器</param>
                /// <param name="memberMap">成员位图</param>
                /// <param name="value">目标数据</param>
                /// <param name="parameters"></param>
                /// <returns>是否存在下一个数据</returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public int Call(xmlParser parser, memberMap memberMap, ref valueType value, ref object[] parameters)
#else
                /// <summary>
                /// 成员解析器
                /// </summary>
                /// <param name="parser">XML解析器</param>
                /// <param name="memberMap">成员位图</param>
                /// <param name="value">目标数据</param>
                /// <returns>是否存在下一个数据</returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public int Call(xmlParser parser, memberMap memberMap, ref valueType value)
#endif
                {
                    parser.itemName = ItemName;
                    if ((parser.Config.MemberFilter & Filter) == Filter)
                    {
#if NOJIT
                        parser.ReflectionParameter = parameters ?? (parameters = new object[1]);
#endif
                        TryParse(parser, ref value);
                        if (parser.state == parseState.Success)
                        {
                            memberMap.SetMember(MemberMapIndex);
                            return 1;
                        }
                        return 0;
                    }
                    return parser.ignoreValue();
                }
            }
            /// <summary>
            /// 解析委托
            /// </summary>
            /// <param name="parser">XML解析器</param>
            /// <param name="value">目标数据</param>
            internal delegate void tryParse(xmlParser parser, ref valueType value);
            /// <summary>
            /// 未知名称解析委托
            /// </summary>
            /// <param name="parser">XML解析器</param>
            /// <param name="value">目标数据</param>
            /// <param name="name">节点名称</param>
            internal delegate bool unknownParse(xmlParser parser, ref valueType value, ref pointer.size name);
            /// <summary>
            /// 解析委托
            /// </summary>
            internal static readonly tryParse DefaultParser;
            /// <summary>
            /// 成员解析器集合
            /// </summary>
            private static readonly tryParseFilter[] memberParsers;
            /// <summary>
            /// 成员名称查找数据
            /// </summary>
            private static readonly pointer.reference memberSearcher;
            /// <summary>
            /// 默认顺序成员名称数据
            /// </summary>
            private static readonly pointer.reference memberNames;
            /// <summary>
            /// 未知名称节点处理
            /// </summary>
            private static readonly unknownParse onUnknownName;
            /// <summary>
            /// XML解析类型配置
            /// </summary>
            private static readonly xmlSerialize attribute;
            /// <summary>
            /// 是否值类型
            /// </summary>
            private static readonly bool isValueType;
            /// <summary>
            /// 是否匿名类型
            /// </summary>
            private static readonly bool isAnonymousType;

            /// <summary>
            /// 引用类型对象解析
            /// </summary>
            /// <param name="parser">XML解析器</param>
            /// <param name="value">目标数据</param>
            internal static void Parse(xmlParser parser, ref valueType value)
            {
                if (DefaultParser == null)
                {
                    if (isValueType) ParseMembers(parser, ref value);
                    else parseClass(parser, ref value);
                }
                else DefaultParser(parser, ref value);
            }
            /// <summary>
            /// 值类型对象解析
            /// </summary>
            /// <param name="parser">XML解析器</param>
            /// <param name="value">目标数据</param>
            internal static void ParseStruct(xmlParser parser, ref valueType value)
            {
                if (DefaultParser == null) ParseMembers(parser, ref value);
                else DefaultParser(parser, ref value);
            }
            /// <summary>
            /// 引用类型对象解析
            /// </summary>
            /// <param name="parser">XML解析器</param>
            /// <param name="value">目标数据</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal static void ParseClass(xmlParser parser, ref valueType value)
            {
                if (DefaultParser == null) parseClass(parser, ref value);
                else DefaultParser(parser, ref value);
            }
            /// <summary>
            /// 引用类型对象解析
            /// </summary>
            /// <param name="parser">XML解析器</param>
            /// <param name="value">目标数据</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal static void parseClass(xmlParser parser, ref valueType value)
            {
                if (value == null)
                {
                    Func<valueType> constructor = fastCSharp.emit.constructor<valueType>.New;
                    if (constructor == null)
                    {
                        parser.checkNoConstructor(ref value, isAnonymousType);
                        if (value == null) return;
                    }
                    else value = constructor();
                }
                else if (isAnonymousType) parser.setAnonymousType(value);
                ParseMembers(parser, ref value);
            }
            /// <summary>
            /// 数据成员解析
            /// </summary>
            /// <param name="parser">XML解析器</param>
            /// <param name="value">目标数据</param>
            internal static void ParseMembers(xmlParser parser, ref valueType value)
            {
#if NOJIT
                object[] parameters = null;
#endif
                byte* names = memberNames.Byte;
                config config = parser.Config;
                memberMap memberMap = config.MemberMap;
                int index = 0;
                if (memberMap == null)
                {
                    while (parser.isName(names, ref index))
                    {
                        if (index == -1) return;
#if NOJIT
                        memberParsers[index].Call(parser, ref value, ref parameters);
#else
                        memberParsers[index].Call(parser, ref value);
#endif
                        if (parser.state != parseState.Success) return;
                        if (parser.isNameEnd(names) == 0)
                        {
                            if (parser.checkNameEnd((char*)(names + (sizeof(short) + sizeof(char))), (*(short*)names >> 1) - 2) == 0) return;
                            break;
                        }
                        ++index;
                        names += *(short*)names + sizeof(short);
                    }
                    fastCSharp.stateSearcher.charsSearcher searcher = new fastCSharp.stateSearcher.charsSearcher(memberSearcher);
                    pointer.size name = new pointer.size();
                    byte isTagEnd = 0;
                    if (onUnknownName == null)
                    {
                        do
                        {
                            if ((name.data = parser.getName(ref name.sizeValue, ref isTagEnd)) == null) return;
                            if (isTagEnd == 0)
                            {
                                if ((index = searcher.UnsafeSearch(name.Char, name.sizeValue)) == -1)
                                {
                                    if (parser.ignoreValue() == 0) return;
                                }
#if NOJIT
                                else if (memberParsers[index].Call(parser, ref value, ref parameters) == 0) return;
#else
                                else if (memberParsers[index].Call(parser, ref value) == 0) return;
#endif
                                if (parser.checkNameEnd(name.Char, name.sizeValue) == 0) return;
                            }
                        }
                        while (true);
                    }
                    else
                    {
                        do
                        {
                            if ((name.data = parser.getName(ref name.sizeValue, ref isTagEnd)) == null) return;
                            if (isTagEnd == 0)
                            {
                                if ((index = searcher.UnsafeSearch(name.Char, name.sizeValue)) == -1)
                                {
                                    name.sizeValue <<= 1;
                                    if (onUnknownName(parser, ref value, ref name))
                                    {
                                        if (parser.state != parseState.Success) return;
                                    }
                                    else
                                    {
                                        if (parser.state == parseState.Success) parser.state = parseState.UnknownNameError;
                                        return;
                                    }
                                }
#if NOJIT
                                else if (memberParsers[index].Call(parser, ref value, ref parameters) == 0) return;
#else
                                else if (memberParsers[index].Call(parser, ref value) == 0) return;
#endif
                                if (parser.checkNameEnd(name.Char, name.sizeValue) == 0) return;
                            }
                        }
                        while (true);
                    }
                }
                else if (memberMap.Type == memberMap<valueType>.TypeInfo)
                {
                    memberMap.Empty();
                    config.MemberMap = null;
                    while (parser.isName(names, ref index))
                    {
                        if (index == -1) return;
#if NOJIT
                        memberParsers[index].Call(parser, memberMap, ref value, ref parameters);
#else
                        memberParsers[index].Call(parser, memberMap, ref value);
#endif
                        if (parser.state != parseState.Success) return;
                        if (parser.isNameEnd(names) == 0)
                        {
                            if (parser.checkNameEnd((char*)(names + (sizeof(short) + sizeof(char))), (*(short*)names >> 1) - 2) == 0) return;
                            break;
                        }
                        ++index;
                        names += *(short*)names + sizeof(short);
                    }
                    fastCSharp.stateSearcher.charsSearcher searcher = new fastCSharp.stateSearcher.charsSearcher(memberSearcher);
                    pointer.size name = new pointer.size();
                    byte isTagEnd = 0;
                    try
                    {
                        if (onUnknownName == null)
                        {
                            do
                            {
                                if ((name.data = parser.getName(ref name.sizeValue, ref isTagEnd)) == null) return;
                                if (isTagEnd == 0)
                                {
                                    if ((index = searcher.UnsafeSearch(name.Char, name.sizeValue)) == -1)
                                    {
                                        if (parser.ignoreValue() == 0) return;
                                    }
#if NOJIT
                                    else if (memberParsers[index].Call(parser, memberMap, ref value, ref parameters) == 0) return;
#else
                                    else if (memberParsers[index].Call(parser, memberMap, ref value) == 0) return;
#endif
                                    if (parser.checkNameEnd(name.Char, name.sizeValue) == 0) return;
                                }
                            }
                            while (true);
                        }
                        else
                        {
                            do
                            {
                                if ((name.data = parser.getName(ref name.sizeValue, ref isTagEnd)) == null) return;
                                if (isTagEnd == 0)
                                {
                                    if ((index = searcher.UnsafeSearch(name.Char, name.sizeValue)) == -1)
                                    {
                                        name.sizeValue <<= 1;
                                        if (onUnknownName(parser, ref value, ref name))
                                        {
                                            if (parser.state != parseState.Success) return;
                                        }
                                        else
                                        {
                                            if (parser.state == parseState.Success) parser.state = parseState.UnknownNameError;
                                            return;
                                        }
                                    }
#if NOJIT
                                    else if (memberParsers[index].Call(parser, memberMap, ref value, ref parameters) == 0) return;
#else
                                    else if (memberParsers[index].Call(parser, memberMap, ref value) == 0) return;
#endif
                                    if (parser.checkNameEnd(name.Char, name.sizeValue) == 0) return;
                                }
                            }
                            while (true);
                        }
                    }
                    finally { config.MemberMap = memberMap; }
                }
                else parser.state = parseState.MemberMap;
            }
            /// <summary>
            /// 包装处理
            /// </summary>
            /// <param name="parser"></param>
            /// <param name="value"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private static void unbox(xmlParser parser, ref valueType value)
            {
                if (parser.isValue() != 0)
                {
#if NOJIT
                    parser.ReflectionParameter = new object[1];
#endif
                    memberParsers[0].TryParse(parser, ref value);
                }
            }
            /// <summary>
            /// 数组解析
            /// </summary>
            /// <param name="parser">XML解析器</param>
            /// <param name="values">目标数据</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal static void Array(xmlParser parser, ref valueType[] values)
            {
                int count = ArrayIndex(parser, ref values);
                if (count != -1 && count != values.Length) System.Array.Resize(ref values, count);
            }
            /// <summary>
            /// 数组解析
            /// </summary>
            /// <param name="parser">XML解析器</param>
            /// <param name="values">目标数据</param>
            /// <returns>数据数量,-1表示失败</returns>
            internal static int ArrayIndex(xmlParser parser, ref valueType[] values)
            {
                if (values == null) values = nullValue<valueType>.Array;
                string arrayItemName = parser.ItemName;
                pointer.size name = new pointer.size();
                int index = 0;
                byte isTagEnd = 0;
                fixed (char* itemFixed = arrayItemName)
                {
                    do
                    {
                        if ((name.data = parser.getName(ref name.sizeValue, ref isTagEnd)) == null) break;
                        if (isTagEnd == 0)
                        {
                            if (arrayItemName.Length != name.sizeValue || !fastCSharp.unsafer.memory.SimpleEqual((byte*)itemFixed, name.Byte, name.sizeValue << 1))
                            {
                                parser.state = parseState.NotArrayItem;
                                return -1;
                            }
                            if (index == values.Length)
                            {
                                valueType value = default(valueType);
                                if (parser.isArrayItem(itemFixed, arrayItemName.Length) != 0)
                                {
                                    Parse(parser, ref value);
                                    if (parser.state != parseState.Success) return -1;
                                    if (parser.checkNameEnd(itemFixed, name.sizeValue) == 0) break;
                                }
                                valueType[] newValues = new valueType[index == 0 ? sizeof(int) : (index << 1)];
                                values.CopyTo(newValues, 0);
                                newValues[index++] = value;
                                values = newValues;
                            }
                            else
                            {
                                if (parser.isArrayItem(itemFixed, arrayItemName.Length) != 0)
                                {
                                    Parse(parser, ref values[index]);
                                    if (parser.state != parseState.Success) return -1;
                                    if (parser.checkNameEnd(itemFixed, name.sizeValue) == 0) break;
                                }
                                ++index;
                            }
                        }
                        else
                        {
                            if (index == values.Length)
                            {
                                valueType[] newValues = new valueType[index == 0 ? sizeof(int) : (index << 1)];
                                values.CopyTo(newValues, 0);
                                values = newValues;
                            }
                            values[index++] = default(valueType);
                        }
                    }
                    while (true);
                }
                return parser.state == parseState.Success ? index : -1;
            }
            ///// <summary>
            ///// 找不到构造函数
            ///// </summary>
            ///// <param name="parser">XML解析器</param>
            ///// <param name="value">目标数据</param>
            //[MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            //private static void noConstructor(xmlParser parser, ref valueType value)
            //{
            //    if (parser.checkNoConstructor(ref value)) ParseClass(parser, ref value);
            //}
            /// <summary>
            /// 不支持基元类型解析
            /// </summary>
            /// <param name="parser">XML解析器</param>
            /// <param name="value">目标数据</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private static void notSupport(xmlParser parser, ref valueType value)
            {
                parser.state = parseState.NotSupport;
            }
            static typeParser()
            {
                Type type = typeof(valueType);
                MethodInfo methodInfo = xmlParser.getParseMethod(type);
                if (methodInfo != null)
                {
#if NOJIT
                    DefaultParser = new methodParser(methodInfo).Parse;
#else
                    DynamicMethod dynamicMethod = new DynamicMethod("xmlParser", typeof(void), new Type[] { typeof(xmlParser), type.MakeByRefType() }, true);
                    dynamicMethod.InitLocals = true;
                    ILGenerator generator = dynamicMethod.GetILGenerator();
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Call, methodInfo);
                    generator.Emit(OpCodes.Ret);
                    DefaultParser = (tryParse)dynamicMethod.CreateDelegate(typeof(tryParse));
#endif
                    return;
                }
                if (type.IsArray)
                {
                    if (type.GetArrayRank() == 1) DefaultParser = (tryParse)Delegate.CreateDelegate(typeof(tryParse), typeParser.GetArrayParser(type.GetElementType()));
                    else DefaultParser = notSupport;
                    return;
                }
                if (type.IsEnum)
                {
                    Type enumType = System.Enum.GetUnderlyingType(type);
                    if (fastCSharp.code.typeAttribute.GetAttribute<FlagsAttribute>(type, false, false) == null)
                    {
                        if (enumType == typeof(uint)) DefaultParser = enumUInt.Parse;
                        else if (enumType == typeof(byte)) DefaultParser = enumByte.Parse;
                        else if (enumType == typeof(ulong)) DefaultParser = enumULong.Parse;
                        else if (enumType == typeof(ushort)) DefaultParser = enumUShort.Parse;
                        else if (enumType == typeof(long)) DefaultParser = enumLong.Parse;
                        else if (enumType == typeof(short)) DefaultParser = enumShort.Parse;
                        else if (enumType == typeof(sbyte)) DefaultParser = enumSByte.Parse;
                        else DefaultParser = enumInt.Parse;
                    }
                    else
                    {
                        if (enumType == typeof(uint)) DefaultParser = enumUInt.ParseFlags;
                        else if (enumType == typeof(byte)) DefaultParser = enumByte.ParseFlags;
                        else if (enumType == typeof(ulong)) DefaultParser = enumULong.ParseFlags;
                        else if (enumType == typeof(ushort)) DefaultParser = enumUShort.ParseFlags;
                        else if (enumType == typeof(long)) DefaultParser = enumLong.ParseFlags;
                        else if (enumType == typeof(short)) DefaultParser = enumShort.ParseFlags;
                        else if (enumType == typeof(sbyte)) DefaultParser = enumSByte.ParseFlags;
                        else DefaultParser = enumInt.ParseFlags;
                    }
                    return;
                }
                if (type.IsPointer)
                {
                    DefaultParser = notSupport;
                    return;
                }
                if (type.IsGenericType)
                {
                    Type genericType = type.GetGenericTypeDefinition();
                    if (genericType == typeof(Nullable<>))
                    {
                        Type[] parameterTypes = type.GetGenericArguments();
                        DefaultParser = (tryParse)Delegate.CreateDelegate(typeof(tryParse), (parameterTypes[0].IsEnum ? nullableEnumParseMethod : nullableParseMethod).MakeGenericMethod(parameterTypes));
                        return;
                    }
                    if (genericType == typeof(KeyValuePair<,>))
                    {
                        DefaultParser = (tryParse)Delegate.CreateDelegate(typeof(tryParse), keyValuePairParseMethod.MakeGenericMethod(type.GetGenericArguments()));
                        isValueType = true;
                        return;
                    }
                }
                if ((methodInfo = typeParser.GetCustomParser(type)) != null)
                {
                    DefaultParser = (tryParse)Delegate.CreateDelegate(typeof(tryParse), methodInfo);
                }
                else
                {
                    Type attributeType;
                    attribute = type.customAttribute<xmlSerialize>(out attributeType, true) ?? xmlSerialize.AllMember;
                    if ((methodInfo = typeParser.GetIEnumerableConstructorParser(type)) != null)
                    {
                        DefaultParser = (tryParse)Delegate.CreateDelegate(typeof(tryParse), methodInfo);
                    }
                    else
                    {
                        if (type.IsValueType) isValueType = true;
                        else if (attribute != xmlSerialize.AllMember && attributeType != type)
                        {
                            for (Type baseType = type.BaseType; baseType != typeof(object); baseType = baseType.BaseType)
                            {
                                xmlSerialize baseAttribute = fastCSharp.code.typeAttribute.GetAttribute<xmlSerialize>(baseType, false, true);
                                if (baseAttribute != null)
                                {
                                    if (baseAttribute.IsBaseType)
                                    {
                                        methodInfo = baseParseMethod.MakeGenericMethod(baseType, type);
                                        DefaultParser = (tryParse)Delegate.CreateDelegate(typeof(tryParse), methodInfo);
                                        return;
                                    }
                                    break;
                                }
                            }
                        }
                        Type refType = type.MakeByRefType();
                        foreach (fastCSharp.code.attributeMethod attributeMethod in fastCSharp.code.attributeMethod.GetStatic(type))
                        {
                            if (attributeMethod.Method.ReturnType == typeof(bool))
                            {
                                ParameterInfo[] parameters = attributeMethod.Method.GetParameters();
                                if (parameters.Length == 3 && parameters[0].ParameterType == typeof(xmlParser) && parameters[1].ParameterType == refType && parameters[2].ParameterType == jsonParser.staticTypeParser.PointerSizeRefType)
                                {
                                    if (attributeMethod.GetAttribute<xmlSerialize.custom>(true) != null)
                                    {
                                        onUnknownName = (unknownParse)Delegate.CreateDelegate(typeof(unknownParse), attributeMethod.Method);
                                        break;
                                    }
                                }
                            }
                        }
                        fieldIndex defaultMember = null;
                        subArray<keyValue<fieldIndex, xmlSerialize.member>> fields = xmlSerializer.staticTypeToXmler.GetFields(memberIndexGroup<valueType>.GetFields(attribute.MemberFilter), attribute);
                        subArray<xmlSerializer.staticTypeToXmler.propertyInfo> properties = typeParser.GetProperties(memberIndexGroup<valueType>.GetProperties(attribute.MemberFilter), attribute);
                        bool isBox = false;
                        if (type.IsValueType && fields.length + properties.length == 1)
                        {
                            boxSerialize boxSerialize = fastCSharp.code.typeAttribute.GetAttribute<boxSerialize>(type, true, true);
                            if (boxSerialize != null && boxSerialize.IsXml)
                            {
                                isBox = true;
                                defaultMember = null;
                            }
                        }
                        tryParseFilter[] parsers = new tryParseFilter[fields.length + properties.length + (defaultMember == null ? 0 : 1)];
                        //memberMap.type memberMapType = memberMap<valueType>.TypeInfo;
                        string[] names = isBox ? null : new string[parsers.Length];
                        int index = 0, nameLength = 0, maxNameLength = 0;
                        foreach (keyValue<fieldIndex, xmlSerialize.member> member in fields)
                        {
                            tryParseFilter tryParse = parsers[index] = new tryParseFilter
                            {
#if NOJIT
                                TryParse = new fieldParser(member.Key.Member).Parser(),
#else
                                TryParse = (tryParse)typeParser.CreateDynamicMethod(type, member.Key.Member).CreateDelegate(typeof(tryParse)),
#endif
                                ItemName = member.Value == null ? null : member.Value.ItemName,
                                MemberMapIndex = member.Key.MemberIndex,
                                Filter = member.Key.Member.IsPublic ? code.memberFilters.PublicInstanceField : code.memberFilters.NonPublicInstanceField
                            };
                            if (!isBox)
                            {
                                string name = member.Key.AnonymousName;
                                if (name.Length > maxNameLength) maxNameLength = name.Length;
                                nameLength += (names[index++] = name).Length;
                                if (member.Key == defaultMember)
                                {
                                    parsers[parsers.Length - 1] = tryParse;
                                    names[parsers.Length - 1] = string.Empty;
                                }
                            }
                        }
                        foreach (xmlSerializer.staticTypeToXmler.propertyInfo member in properties)
                        {
                            parsers[index] = new tryParseFilter
                            {
#if NOJIT
                                TryParse = new propertyParser(member.Property.Member).Parser(),
#else
                                TryParse = (tryParse)typeParser.CreateDynamicMethod(type, member.Property.Member, member.Method).CreateDelegate(typeof(tryParse)),
#endif
                                ItemName = member.Attribute == null ? null : member.Attribute.ItemName,
                                MemberMapIndex = member.Property.MemberIndex,
                                Filter = member.Method.IsPublic ? code.memberFilters.PublicInstanceProperty : code.memberFilters.NonPublicInstanceProperty
                            };
                            if (!isBox)
                            {
                                if (member.Property.Member.Name.Length > maxNameLength) maxNameLength = member.Property.Member.Name.Length;
                                nameLength += (names[index++] = member.Property.Member.Name).Length;
                            }
                        }
                        memberParsers = parsers;
                        if (isBox) DefaultParser = unbox;
                        else
                        {
                            if (type.Name[0] == '<') isAnonymousType = true;
                            if (maxNameLength > (short.MaxValue >> 1) - 2 || nameLength == 0) memberNames = unmanaged.NullByte8;
                            else
                            {
                                memberNames = unmanaged.GetStatic((nameLength + (names.Length - (defaultMember == null ? 0 : 1)) * 3 + 1) << 1, false).Reference;
                                byte* write = memberNames.Byte;
                                foreach (string name in names)
                                {
                                    if (name.Length != 0)
                                    {
                                        *(short*)write = (short)((name.Length + 2) * sizeof(char));
                                        *(char*)(write + sizeof(short)) = '<';
                                        fixed (char* nameFixed = name) fastCSharp.unsafer.memory.SimpleCopy(nameFixed, (char*)(write + (sizeof(short) + sizeof(char))), name.Length);
                                        *(char*)(write += (sizeof(short) + sizeof(char)) + (name.Length << 1)) = '>';
                                        write += sizeof(char);
                                    }
                                }
                                *(short*)write = 0;
                            }
                            if (type.IsGenericType) memberSearcher = typeParser.GetGenericDefinitionMemberSearcher(type, names).Reference;
                            else memberSearcher = fastCSharp.stateSearcher.charsSearcher.Create(names, true).Reference;
                        }
                    }
                }
            }
#if NOJIT
            /// <summary>
            /// XML解析
            /// </summary>
            private sealed class methodParser
            {
                /// <summary>
                /// 解析函数信息
                /// </summary>
                private MethodInfo method;
                /// <summary>
                /// XML解析
                /// </summary>
                /// <param name="method"></param>
                public methodParser(MethodInfo method)
                {
                    this.method = method;
                }
                /// <summary>
                /// 解析委托
                /// </summary>
                /// <param name="parser">XML解析器</param>
                /// <param name="value">目标数据</param>
                public void Parse(xmlParser parser, ref valueType value)
                {
                    object[] parameters = new object[] { value };
                    method.Invoke(parser, parameters);
                    value = (valueType)parameters[0];
                }
            }
            /// <summary>
            /// 字段解析（反射模式）
            /// </summary>
            private sealed class fieldParser
            {
                /// <summary>
                /// 字段信息
                /// </summary>
                private FieldInfo field;
                /// <summary>
                /// 解析函数信息
                /// </summary>
                private pub.methodParameter1 method;
                /// <summary>
                /// 字段解析
                /// </summary>
                /// <param name="field"></param>
                public fieldParser(FieldInfo field)
                {
                    this.field = field;
                    method = new pub.methodParameter1(typeParser.GetMemberMethodInfo(field.FieldType));
                }
                /// <summary>
                /// 获取解析委托
                /// </summary>
                /// <returns></returns>
                public tryParse Parser()
                {
                    return typeof(valueType).IsValueType ? (tryParse)parseValue : parse;
                }
                /// <summary>
                /// 解析委托
                /// </summary>
                /// <param name="parser">XML解析器</param>
                /// <param name="value">目标数据</param>
                private void parse(xmlParser parser, ref valueType value)
                {
                    object[] parameters = parser.ReflectionParameter;
                    parameters[0] = field.GetValue(value);
                    method.Invoke(parser, parameters);
                    field.SetValue(value, parameters[0]);
                }
                /// <summary>
                /// 解析委托
                /// </summary>
                /// <param name="parser">XML解析器</param>
                /// <param name="value">目标数据</param>
                private void parseValue(xmlParser parser, ref valueType value)
                {
                    object[] parameters = parser.ReflectionParameter;
                    object objectValue = value;
                    parameters[0] = field.GetValue(objectValue);
                    method.Invoke(parser, parameters);
                    field.SetValue(objectValue, parameters[0]);
                    value = (valueType)objectValue;
                }
            }
            /// <summary>
            /// 属性解析（反射模式）
            /// </summary>
            private sealed class propertyParser
            {
                /// <summary>
                /// 获取函数信息
                /// </summary>
                private MethodInfo getMethod;
                /// <summary>
                /// 设置函数信息
                /// </summary>
                private MethodInfo setMethod;
                /// <summary>
                /// 解析函数信息
                /// </summary>
                private pub.methodParameter1 method;
                /// <summary>
                /// 属性解析
                /// </summary>
                /// <param name="property"></param>
                public propertyParser(PropertyInfo property)
                {
                    if (property.CanRead) getMethod = property.GetGetMethod(true);
                    setMethod = property.GetSetMethod(true);
                    method = new pub.methodParameter1(typeParser.GetMemberMethodInfo(property.PropertyType));
                }
                /// <summary>
                /// 获取解析委托
                /// </summary>
                /// <returns></returns>
                public tryParse Parser()
                {
                    return typeof(valueType).IsValueType ? (tryParse)parseValue : parse;
                }
                /// <summary>
                /// 解析委托
                /// </summary>
                /// <param name="parser">XML解析器</param>
                /// <param name="value">目标数据</param>
                private void parse(xmlParser parser, ref valueType value)
                {
                    object[] parameters = parser.ReflectionParameter;
                    parameters[0] = getMethod == null ? null : getMethod.Invoke(value, null);
                    method.Invoke(parser, parameters);
                    setMethod.Invoke(value, parameters);
                }
                /// <summary>
                /// 解析委托
                /// </summary>
                /// <param name="parser">XML解析器</param>
                /// <param name="value">目标数据</param>
                private void parseValue(xmlParser parser, ref valueType value)
                {
                    object[] parameters = parser.ReflectionParameter;
                    object objectValue = value;
                    parameters[0] = getMethod == null ? null : getMethod.Invoke(objectValue, null);
                    method.Invoke(parser, parameters);
                    setMethod.Invoke(objectValue, parameters);
                    value = (valueType)objectValue;
                }
            }
#endif
        }
        /// <summary>
        /// 解析类型
        /// </summary>
        private sealed class parseMethod : Attribute { }
        /// <summary>
        /// 属性索引位置
        /// </summary>
        internal struct attributeIndex
        {
            /// <summary>
            /// 起始位置索引
            /// </summary>
            public int StartIndex;
            /// <summary>
            /// 字符长度
            /// </summary>
            public int Length;
            /// <summary>
            /// 属性索引位置
            /// </summary>
            /// <param name="startIndex"></param>
            /// <param name="endIndex"></param>
            public attributeIndex(int startIndex, int endIndex)
            {
                StartIndex = startIndex;
                Length = endIndex - startIndex;
            }
        }
        /// <summary>
        /// XML字符串
        /// </summary>
        private string xml;
        /// <summary>
        /// 配置参数
        /// </summary>
        internal config Config;
        /// <summary>
        /// 匿名类型数据
        /// </summary>
        private subArray<keyValue<Type, object>> anonymousTypes;
        /// <summary>
        /// 字符状态位查询表格
        /// </summary>
        private readonly byte* bits = Bits.Byte;
        /// <summary>
        /// 集合子节点名称
        /// </summary>
        private string itemName;
        /// <summary>
        /// 集合子节点名称
        /// </summary>
        internal string ItemName
        {
            get
            {
                if (itemName == null) return Config.ItemName ?? "item";
                string value = itemName;
                itemName = null;
                return value;
            }
        }
        /// <summary>
        /// 字符解码器
        /// </summary>
        private fastCSharp.stateSearcher.asciiSearcher decoder = new fastCSharp.stateSearcher.asciiSearcher(decodeChars);
        /// <summary>
        /// 属性
        /// </summary>
        private subArray<keyValue<attributeIndex, attributeIndex>> attributes;
        /// <summary>
        /// XML字符串起始位置
        /// </summary>
        private char* xmlFixed;
        /// <summary>
        /// 当前解析位置
        /// </summary>
        private char* current;
        /// <summary>
        /// 解析结束位置
        /// </summary>
        private char* end;
        /// <summary>
        /// 当前数据起始位置
        /// </summary>
        private char* valueStart;
        /// <summary>
        /// 当前数据长度
        /// </summary>
        private int valueSize;
        /// <summary>
        /// 属性名称起始位置
        /// </summary>
        private int attributeNameStartIndex;
        /// <summary>
        /// 属性名称结束位置
        /// </summary>
        private int attributeNameEndIndex;
        /// <summary>
        /// 数字符号
        /// </summary>
        private char sign;
        /// <summary>
        /// 当前数据是否CDATA
        /// </summary>
        private byte isCData;
        /// <summary>
        /// 名称解析节点是否结束
        /// </summary>
        private byte isTagEnd;
        /// <summary>
        /// 解析状态
        /// </summary>
        private parseState state;
#if NOJIT
        /// <summary>
        /// 反射模式参数
        /// </summary>
        private object[] ReflectionParameter;
#endif
        /// <summary>
        /// XML解析
        /// </summary>
        /// <typeparam name="valueType">目标类型</typeparam>
        /// <param name="xml">XML字符串</param>
        /// <param name="value">目标数据</param>
        /// <param name="config">配置参数</param>
        /// <returns>解析状态</returns>
        private parseState parse<valueType>(ref subString xml, ref valueType value, config config)
        {
            fixed (char* xmlFixed = (this.xml = xml.value))
            {
                string bootName = config.BootNodeName;
                current = (this.xmlFixed = xmlFixed) + xml.StartIndex;
                end = current + xml.Length;
                this.Config = config;
                fixed (char* bootNameFixed = bootName)
                {
                NEXTEND:
                    while (end != current)
                    {
                        if (((bits[*(byte*)--end] & spaceBit) | *(((byte*)end) + 1)) != 0)
                        {
                            if (*end == '>')
                            {
                                if (*(end - 1) == '-')
                                {
                                    if (*(end - 2) == '-' && (end -= 2 + 3) > current)
                                    {
                                        do
                                        {
                                        NOTE:
                                            if (*--end == '<')
                                            {
                                                if (*(end + 1) == '!' && *(int*)(end + 2) == '-' + ('-' << 16)) goto NEXTEND;
                                                if ((end -= 3) <= current) break;
                                                goto NOTE;
                                            }
                                        }
                                        while (end != current);
                                    }
                                    state = parseState.NoteError;
                                }
                                else if ((end -= (2 + bootName.Length)) > current && *(int*)end == ('<' + ('/' << 16))
                                    && fastCSharp.unsafer.memory.SimpleEqual((byte*)bootNameFixed, (byte*)(end + 2), bootName.Length << 1))
                                {
                                    goto START;
                                }
                            }
                            state = parseState.NotFoundBootNodeEnd;
                            goto ERROR;
                        }
                    }
                START:
                    state = parseState.Success;
                    space();
                    if (state == parseState.Success)
                    {
                        if (*(int*)current == ('<' + ('?' << 16)))
                        {
                            current += 3;
                            do
                            {
                                if (*current == '>')
                                {
                                    if (current <= end)
                                    {
                                        if (*(current - 1) == '?')
                                        {
                                            ++current;
                                            break;
                                        }
                                        else state = parseState.HeaderError;
                                    }
                                    else state = parseState.CrashEnd;
                                    goto ERROR;
                                }
                                ++current;
                            }
                            while (true);
                            space();
                            if (state != parseState.Success) goto ERROR;
                        }
                        if (*current == '<' && fastCSharp.unsafer.memory.SimpleEqual((byte*)bootNameFixed, (byte*)(++current), bootName.Length << 1))
                        {
                            if (((bits[*(byte*)(current += bootName.Length)] & spaceBit) | *(((byte*)current) + 1)) != 0)
                            {
                                if (*current == '>')
                                {
                                    attributes.Empty();
                                    ++current;
                                    goto PARSE;
                                }
                            }
                            else
                            {
                                ++current;
                                attribute();
                                if (state == parseState.Success) goto PARSE;
                            }
                        }
                    }
                    else state = parseState.NotFoundBootNodeStart;
                    goto ERROR;
                PARSE:
                    typeParser<valueType>.Parse(this, ref value);
                    if (state == parseState.Success)
                    {
                        space();
                        if (state == parseState.Success)
                        {
                            if (current == end) return Config.State = parseState.Success;
                            state = parseState.CrashEnd;
                        }
                    }
                ERROR:
                    if (Config.IsOutputError)
                    {
                        fastCSharp.log.Default.Add(state.ToString() + "[" + ((int)(current - xmlFixed) - xml.StartIndex).toString() + @"]
" + xml, new System.Diagnostics.StackFrame(), false);
                    }
                    return Config.State = state;
                }
            }
        }
        /// <summary>
        /// XML字符串解析
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private bool parseEncodeString(ref subString xml)
        {
            fixed (char* xmlFixed = xml.value)
            {
                valueStart = current = xmlFixed + xml.StartIndex;
                end = current + xml.Length;
                int leftSize = valueSize = 0;
                while (current != end)
                {
                    if (*current == '&')
                    {
                        leftSize = (int)(current - valueStart);
                        break;
                    }
                    ++current;
                }
                while (current != end)
                {
                    if (*current == '&')
                    {
                        do
                        {
                            ++valueSize;
                            if (*++current == ';') break;
                            if (*current == '<') return false;
                        }
                        while (true);
                    }
                    ++current;
                }
                if (valueSize != 0)
                {
                    string decodeValue = fastCSharp.String.FastAllocateString(xml.Length - valueSize);
                    fixed (char* valueFixed = decodeValue)
                    {
                        fastCSharp.unsafer.memory.Copy(valueStart, valueFixed, leftSize);
                        valueStart += leftSize;
                        decodeString(valueFixed + leftSize, valueFixed + decodeValue.Length);
                    }
                    if (state != parseState.Success) return false;
                    xml.UnsafeSet(decodeValue, 0, decodeValue.Length);
                }
                return true;
            }
        }
        /// <summary>
        /// XML字符串解析
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private bool parseTempString(ref subString xml)
        {
            fixed (char* xmlFixed = xml.value)
            {
                valueStart = current = xmlFixed + xml.StartIndex;
                end = current + xml.Length;
                int leftSize = valueSize = 0;
                while (current != end)
                {
                    if (*current == '&')
                    {
                        leftSize = (int)(current - valueStart);
                        break;
                    }
                    ++current;
                }
                while (current != end)
                {
                    if (*current == '&')
                    {
                        do
                        {
                            ++valueSize;
                            if (*++current == ';') break;
                            if (*current == '<') return false;
                        }
                        while (true);
                    }
                    ++current;
                }
                if (valueSize != 0)
                {
                    decodeString(valueStart += leftSize, valueStart + ((valueSize = xml.Length - valueSize) - leftSize));
                    if (state != parseState.Success) return false;
                    xml.UnsafeSetLength(valueSize);
                }
                return true;
            }
        }
        /// <summary>
        /// 释放XML解析器
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void free()
        {
            xml = null;
            Config = null;
            itemName = null;
            anonymousTypes.Null();
#if NOJIT
            ReflectionParameter = null;
#endif
            typePool<xmlParser>.PushNotNull(this);
        }
        /// <summary>
        /// 空格过滤
        /// </summary>
        private void space()
        {
        START:
            while (((bits[*(byte*)current] & spaceBit) | *(((byte*)current) + 1)) == 0) ++current;
            if (*(long*)current == '<' + ('!' << 16) + ((long)'-' << 32) + ((long)'-' << 48))
            {
                current += 6;
                do
                {
                    if (*current == '>')
                    {
                        if (current > end)
                        {
                            state = parseState.CrashEnd;
                            return;
                        }
                        if (*(int*)(current - 2) == '-' + ('-' << 16))
                        {
                            ++current;
                            goto START;
                        }
                        current += 3;
                    }
                    else ++current;
                }
                while (true);
            }
        }
        /// <summary>
        /// 空格过滤
        /// </summary>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private char* endSpace()
        {
            char* end = current;
            while (((bits[*(byte*)--end] & spaceBit) | *(((byte*)end) + 1)) == 0) ;
            return end + 1;
        }
        /// <summary>
        /// 获取节点名称
        /// </summary>
        /// <param name="nameSize">节点名称长度</param>
        /// <param name="isTagEnd">名称解析节点是否结束</param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private char* getName(ref int nameSize, ref byte isTagEnd)
        {
            char* nameStart = getName(ref nameSize);
            isTagEnd = this.isTagEnd;
            return nameStart;
        }
        /// <summary>
        /// 获取节点名称
        /// </summary>
        /// <param name="nameSize">节点名称长度</param>
        /// <returns></returns>
        private char* getName(ref int nameSize)
        {
            space();
            if (state != parseState.Success) return null;
            if (*current == '<')
            {
                char code = *(current + 1);
                if (((bits[code & 0xff] & targetStartCheckBit) | (code & 0xff00)) == 0)
                {
                    if (code == '/') return null;
                }
                else return getNameOnly(ref nameSize);
            }
            state = parseState.NotFoundTagStart;
            return null;
        }
        /// <summary>
        /// 获取节点名称
        /// </summary>
        /// <param name="nameSize">节点名称长度</param>
        /// <returns></returns>
        private char* getNameOnly(ref int nameSize)
        {
            char* nameStart = ++current;
            do
            {
                if (((bits[*(byte*)++current] & spaceBit) | *(((byte*)current) + 1)) != 0)
                {
                    if (*current == '>')
                    {
                        if (current < end)
                        {
                            isTagEnd = 0;
                            attributes.Empty();
                            nameSize = (int)(current++ - nameStart);
                            return nameStart;
                        }
                        state = parseState.CrashEnd;
                        return null;
                    }
                    if (*current == '/')
                    {
                        if (*(current + 1) == '>')
                        {
                            nameSize = (int)(current - nameStart);
                            isTagEnd = 1;
                            attributes.Empty();
                            current += 2;
                            return nameStart;
                        }
                        state = parseState.NotFoundTagStart;
                        return null;
                    }
                }
                else
                {
                    nameSize = (int)(current++ - nameStart);
                    attribute();
                    if (state == parseState.Success)
                    {
                        isTagEnd = 0;
                        return nameStart;
                    }
                    return null;
                }
            }
            while (true);
        }
        /// <summary>
        /// 属性解析
        /// </summary>
        private void attribute()
        {
            attributes.Empty();
            while (((bits[*(byte*)current] & spaceBit) | *(((byte*)current) + 1)) == 0) ++current;
            if (*current == '>')
            {
                if (current++ > end) state = parseState.CrashEnd;
                return;
            }
            if (*current == '/')
            {
                if (*(current + 1) == '>')
                {
                    isTagEnd = 1;
                    current += 2;
                }
                else state = parseState.NotFoundTagStart;
                return;
            }
            attributeName();
        }
        /// <summary>
        /// 属性名称解析
        /// </summary>
        private void attributeName()
        {
        NAME:
            attributeNameStartIndex = (int)(current - xmlFixed);
            do
            {
                if (((bits[*(byte*)++current] & attributeNameSearchBit) | *(((byte*)current) + 1)) == 0)
                {
                    switch (*current & 7)
                    {
                        case '\t' & 7:
                        case ' ' & 7:
                        case '\n' & 7:
                        SPACE:
                            attributeNameEndIndex = (int)(current - xmlFixed);
                            while (((bits[*(byte*)++current] & spaceBit) | *(((byte*)current) + 1)) == 0) ;
                            if (*current == '=')
                            {
                                if (attributeValue() == 0) return;
                                goto NAME;
                            }
                            break;
                        case '=' & 7:
                            if (*current == '=')
                            {
                                attributeNameEndIndex = (int)(current - xmlFixed);
                                if (attributeValue() == 0) return;
                                goto NAME;
                            }
                            goto SPACE;
                    }
                    state = parseState.NotFoundAttributeName;
                    return;
                }
            }
            while (true);
        }
        /// <summary>
        /// 属性值解析
        /// </summary>
        /// <returns></returns>
        private int attributeValue()
        {
            while (((bits[*(byte*)++current] & spaceBit) | *(((byte*)current) + 1)) == 0) ;
            if (*current == '"')
            {
                int valueStartIndex = (int)(++current - xmlFixed);
                do
                {
                    if (*current == '"')
                    {
                        if (Config.IsAttribute)
                        {
                            attributes.Add(new keyValue<attributeIndex, attributeIndex>(new attributeIndex(attributeNameStartIndex, attributeNameEndIndex), new attributeIndex(valueStartIndex, (int)(current - xmlFixed))));
                        }
                        while (((bits[*(byte*)++current] & spaceBit) | *(((byte*)current) + 1)) == 0) ;
                        if (*current == '>')
                        {
                            if (current++ > end) state = parseState.CrashEnd;
                            return 0;
                        }
                        if (*current == '/')
                        {
                            if (*(current + 1) == '>')
                            {
                                isTagEnd = 1;
                                current += 2;
                            }
                            else state = parseState.NotFoundTagStart;
                            return 0;
                        }
                        return 1;
                    }
                    if (*current == '<' && current >= end)
                    {
                        state = parseState.NotFoundAttributeValue;
                        return 0;
                    }
                    ++current;
                }
                while (true);
            }
            state = parseState.NotFoundAttributeValue;
            return 0;
        }
        /// <summary>
        /// 判断否存存在数据
        /// </summary>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private int isValue()
        {
            space();
            return state == parseState.Success ? *(int*)current ^ ('<' + ('/' << 16)) : 0;
        }
        /// <summary>
        /// 节点名称结束检测
        /// </summary>
        /// <param name="nameStart"></param>
        /// <param name="nameSize"></param>
        /// <returns></returns>
        private int checkNameEnd(char* nameStart, int nameSize)
        {
            space();
            if (state == parseState.Success)
            {
                if (*(int*)current == '<' + ('/' << 16) && *(current + (2 + nameSize)) == '>' && fastCSharp.unsafer.memory.SimpleEqual((byte*)(current + 2), (byte*)nameStart, nameSize << 1) && current != end)
                {
                    current += nameSize + 3;
                    return 1;
                }
                state = parseState.NotFoundTagEnd;
            }
            return 0;
        }
        /// <summary>
        /// 是否匹配默认顺序名称
        /// </summary>
        /// <param name="names"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool isName(byte* names, ref int index)
        {
            int length = *(short*)names;
            if (length == 0)
            {
                index = -1;
                return true;
            }
            else if (fastCSharp.unsafer.memory.SimpleEqual((byte*)current, names += sizeof(short), length))
            {
                current = (char*)((byte*)current + length);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 是否匹配默认顺序名称
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        private byte isNameEnd(byte* names)
        {
            if (*(int*)current == '<' + ('/' << 16))
            {
                int length = *(short*)names - sizeof(char);
                if (fastCSharp.unsafer.memory.SimpleEqual((byte*)current + sizeof(int), names + (sizeof(short) + sizeof(char)), length) && current != end)
                {
                    current = (char*)((byte*)current + (length + sizeof(int)));
                    return 1;
                }
            }
            return 0;
        }
        /// <summary>
        /// 设置未知节点名称
        /// </summary>
        /// <param name="nameStart"></param>
        /// <param name="nameSize"></param>
        private void setName(char* nameStart, int nameSize)
        {
            valueStart = nameStart;
            valueSize = nameSize;
        }
        /// <summary>
        /// 是否存在数组数据
        /// </summary>
        /// <param name="nameStart"></param>
        /// <param name="nameSize"></param>
        /// <returns></returns>
        private int isArrayItem(char* nameStart, int nameSize)
        {
            if (*(int*)current == '<' + ('/' << 16) && *(current + (2 + nameSize)) == '>' && fastCSharp.unsafer.memory.SimpleEqual((byte*)(current + 2), (byte*)nameStart, nameSize << 1) && current != end)
            {
                current += nameSize + 3;
                return 0;
            }
            return 1;
        }
        /// <summary>
        /// 忽略数据
        /// </summary>
        /// <returns>是否成功</returns>
        private int ignoreValue()
        {
            char* nameStart;
            int nameSize = 0;
        START:
            space();
            if (state != parseState.Success) return 0;
            if (*current == '<')
            {
                char code = *(current + 1);
                if (((bits[code & 0xff] & targetStartCheckBit) | (code & 0xff00)) == 0)
                {
                    if (code == '/') return 1;
                    if (code == '!')
                    {
                        if (((*(int*)(current + 2) ^ ('[' + ('C' << 16))) | (*(int*)(current + 4) ^ ('D' + ('A' << 16))) | (*(int*)(current + 6) ^ ('T' + ('A' << 16))) | (*(short*)(current + 8) ^ '[')) == 0)
                        {
                            current += 11;
                            do
                            {
                                if (*current == '>')
                                {
                                    if (*(int*)(current - 2) == (']' + (']' << 16)))
                                    {
                                        ++current;
                                        return 1;
                                    }
                                    else if (current < end) current += 3;
                                    else
                                    {
                                        state = parseState.CrashEnd;
                                        return 0;
                                    }
                                }
                                else ++current;
                            }
                            while (true);
                        }
                    }
                    state = parseState.NotFoundTagStart;
                    return 0;
                }
                if ((nameStart = getNameOnly(ref nameSize)) == null) return 0;
                if (isTagEnd == 0 && (ignoreValue() == 0 || checkNameEnd(nameStart, nameSize) == 0)) return 0;
                goto START;
            }
            while (*++current != '<') ;
            return 1;
        }
        /// <summary>
        /// 忽略数据
        /// </summary>
        /// <returns></returns>
        public bool IgnoreValue()
        {
            return ignoreValue() != 0;
        }
        /// <summary>
        /// 获取文本数据
        /// </summary>
        private void getValue()
        {
            space();
            if (state != parseState.Success) return;
            if (*current == '<')
            {
                switch (*(current + 1))
                {
                    case '/':
                        valueStart = current;
                        isCData = 1;
                        valueSize = 0;
                        return;
                    case '!':
                        if (((*(int*)(current + 2) ^ ('[' + ('C' << 16))) | (*(int*)(current + 4) ^ ('D' + ('A' << 16))) | (*(int*)(current + 6) ^ ('T' + ('A' << 16))) | (*(short*)(current + 8) ^ '[')) == 0)
                        {
                            valueStart = current + 9;
                            current += 11;
                            do
                            {
                                if (*current == '>')
                                {
                                    char* cDataValueEnd = current - 2;
                                    if (*(int*)cDataValueEnd == (']' + (']' << 16)))
                                    {
                                        ++current;
                                        valueSize = (int)(cDataValueEnd - valueStart);
                                        isCData = 1;
                                        return;
                                    }
                                    else if (current < end) current += 3;
                                    else
                                    {
                                        state = parseState.CrashEnd;
                                        return;
                                    }
                                }
                                else ++current;
                            }
                            while (true);
                        }
                        break;
                }
                state = parseState.NotFoundValue;
                return;
            }
            valueStart = current;
            while (*++current != '<') ;
            valueSize = (int)(endSpace() - valueStart);
            isCData = 0;
            return;
        }
        /// <summary>
        /// 数据结束处理
        /// </summary>
        private void getValueEnd()
        {
            if (isCData != 0)
            {
                space();
                if (*current != '<') state = parseState.NotFoundValueEnd;
            }
        }
        /// <summary>
        /// 查找数据起始位置
        /// </summary>
        private void searchValue()
        {
            space();
            if (state == parseState.Success)
            {
                if (*current == '<')
                {
                    switch (*(current + 1))
                    {
                        case '/':
                            isCData = 0;
                            return;
                        case '!':
                            if (((*(int*)(current + 2) ^ ('[' + ('C' << 16))) | (*(int*)(current + 4) ^ ('D' + ('A' << 16))) | (*(int*)(current + 6) ^ ('T' + ('A' << 16))) | (*(short*)(current + 8) ^ '[')) == 0)
                            {
                                current += 9;
                                isCData = 1;
                                return;
                            }
                            break;
                    }
                    state = parseState.NotFoundValue;
                    isCData = 2;
                }
                else isCData = 0;
            }
            else isCData = 2;
        }
        /// <summary>
        /// 数据结束处理
        /// </summary>
        private void searchValueEnd()
        {
            switch (isCData)
            {
                case 0:
            SPACE:
                    space();
                    if (*current == '<' || state != parseState.Success) return;
                    break;
                case 1:
                    if (((*(int*)current ^ (']' + (']' << 16))) | (*(short*)(current + 2) ^ '>')) == 0)
                    {
                        current += 3;
                        goto SPACE;
                    }
                    break;
            }
            state = parseState.NotFoundValueEnd;
        }
        /// <summary>
        /// 忽略数据
        /// </summary>
        private void ignoreSearchValue()
        {
            if (isCData == 0)
            {
                while (*current != '<') ++current;
            }
            else
            {
                current += 2;
                do
                {
                    if (*current == '>')
                    {
                        if (*(int*)(current - 2) == (']' + (']' << 16)))
                        {
                            ++current;
                            space();
                            if (*current != '<') state = parseState.NotFoundValueEnd;
                            return;
                        }
                        else if (current < end) current += 3;
                        else
                        {
                            state = parseState.CrashEnd;
                            return;
                        }
                    }
                    else ++current;
                }
                while (true);
            }
        }
        /// <summary>
        /// 查找CDATA数据结束位置
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void searchCData2()
        {
            if (((*(int*)(current + 2) ^ ('[' + ('C' << 16))) | (*(int*)(current + 4) ^ ('D' + ('A' << 16))) | (*(int*)(current + 6) ^ ('T' + ('A' << 16))) | (*(short*)(current + 8) ^ '[')) == 0)
            {
                current += 9;
                searchCDataValue();
            }
            else state = parseState.NotFoundCDATAStart;
        }
        /// <summary>
        /// 查找CDATA数据结束位置
        /// </summary>
        private void searchCDataValue()
        {
            valueStart = current;
            current += 2;
            do
            {
                if (*current == '>')
                {
                    char* valueEnd = current - 2;
                    if (*(int*)valueEnd == (']' + (']' << 16)))
                    {
                        ++current;
                        valueSize = (int)(valueEnd - valueStart);
                        return;
                    }
                    else if (current < end) current += 3;
                    else
                    {
                        state = parseState.CrashEnd;
                        return;
                    }
                }
                else ++current;
            }
            while (true);
        }
        /// <summary>
        /// 查找枚举数字
        /// </summary>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private bool isEnumNumber()
        {
            searchValue();
            return (uint)(*current - '0') < 10;
        }
        /// <summary>
        /// 查找枚举数字
        /// </summary>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private bool isEnumNumberFlag()
        {
            searchValue();
            uint number = (uint)(*current - '0');
            return number < 10 || (int)number == '-' - '0';
        }
        /// <summary>
        /// 读取下一个枚举字符
        /// </summary>
        /// <returns>枚举字符,结束或者错误返回0</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private char nextEnumChar()
        {
            if (((bits[*(byte*)current] & spaceBit) | *(((byte*)current) + 1)) != 0)
            {
                if (*current == '<') return (char)0;
                if (*current == '&')
                {
                    char value = (char)0;
                    decodeChar(ref value);
                    return value;
                }
                return *current++;
            }
            state = parseState.NotEnumChar;
            return (char)0;
        }
        /// <summary>
        /// 读取下一个枚举字符
        /// </summary>
        /// <returns>枚举字符,结束或者错误返回0</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private char nextCDataEnumChar()
        {
            if (valueSize == 0) return (char)0;
            --valueSize;
            return *valueStart++;
        }
        /// <summary>
        /// 枚举值是否结束
        /// </summary>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private int isNextFlagEnum()
        {
            if (state == parseState.Success)
            {
                if (isCData == 0)
                {
                START:
                    switch ((*current >> 3) & 3)
                    {
                        case (' ' >> 3) & 3:
                        case (',' >> 3) & 3:
                            if (*current == ',' || *current == ' ')
                            {
                                ++current;
                                goto START;
                            }
                            return 1;
                        case ('<' >> 3) & 3:
                            return *current - '<';
                        default:
                            return 1;
                    }
                }
                else
                {
                    while (valueSize != 0)
                    {
                        if (*valueStart == ',' || *valueStart == ' ')
                        {
                            --valueSize;
                            ++valueStart;
                            continue;
                        }
                        return 1;
                    }
                }
            }
            return 0;
        }
        /// <summary>
        /// 逻辑值解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref bool value)
        {
            searchValue();
            if (isCData != 2)
            {
                if ((*current | 0x20) == 'f')
                {
                    if ((*(long*)(current + 1) | 0x20002000200020L) == ('a' + ('l' << 16) + ((long)'s' << 32) + ((long)'e' << 48)))
                    {
                        current += 5;
                        value = false;
                        searchValueEnd();
                        return;
                    }
                }
                else
                {
                    if ((*(long*)current | 0x20002000200020L) == ('t' + ('r' << 16) + ((long)'u' << 32) + ((long)'e' << 48)))
                    {
                        current += 4;
                        value = true;
                        searchValueEnd();
                        return;
                    }
                    if ((uint)(*current - '0') < 2)
                    {
                        value = *current++ != '0';
                        searchValueEnd();
                        return;
                    }
                }
                state = parseState.NotBool;
            }
        }
        /// <summary>
        /// 逻辑值解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref bool? value)
        {
            searchValue();
            if (isCData != 2)
            {
                if ((*current | 0x20) == 'f')
                {
                    if ((*(long*)(current + 1) | 0x0020002000200020L) == ('a' + ('l' << 16) + ((long)'s' << 32) + ((long)'e' << 48)))
                    {
                        current += 5;
                        value = false;
                        searchValueEnd();
                        return;
                    }
                }
                else
                {
                    if ((*(long*)(current) | 0x0020002000200020L) == ('t' + ('r' << 16) + ((long)'u' << 32) + ((long)'e' << 48)))
                    {
                        current += 4;
                        value = true;
                        searchValueEnd();
                        return;
                    }
                    if ((uint)(*current - '0') < 2)
                    {
                        value = *current++ != '0';
                        searchValueEnd();
                        return;
                    }
                    if (*current == '<')
                    {
                        value = null;
                        return;
                    }
                }
                state = parseState.NotBool;
            }
        }
        /// <summary>
        /// 解析10进制数字
        /// </summary>
        /// <param name="value">第一位数字</param>
        /// <returns>数字</returns>
        private uint parseUInt32(uint value)
        {
            uint number;
            while ((number = (uint)(*current - '0')) < 10)
            {
                value *= 10;
                ++current;
                value += (byte)number;
            }
            return value;
        }
        /// <summary>
        /// 解析16进制数字
        /// </summary>
        /// <param name="value">数值</param>
        private void parseHex32(ref uint value)
        {
            uint isValue = 0;
            do
            {
                uint number = (uint)(*current - '0');
                if (number > 9)
                {
                    if ((number = (number - ('A' - '0')) & 0xffdfU) > 5)
                    {
                        if (isValue == 0) state = parseState.NotHex;
                        return;
                    }
                    number += 10;
                }
                ++current;
                value <<= 4;
                isValue = 1;
                value += number;
            }
            while (true);
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void parse(ref byte value)
        {
            searchValue();
            parseNumber(ref value);
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        private void parseNumber(ref byte value)
        {
            if (isCData != 2)
            {
                uint number = (uint)(*current - '0');
                if (number < 10)
                {
                    ++current;
                    if (number == 0)
                    {
                        if (*current == 'x')
                        {
                            ++current;
                            parseHex32(ref number);
                            if (state != parseState.Success) return;
                            value = (byte)number;
                        }
                        else value = 0;
                    }
                    else value = (byte)parseUInt32(number);
                    searchValueEnd();
                    return;
                }
                state = parseState.NotNumber;
            }
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref byte? value)
        {
            searchValue();
            if (isCData != 2)
            {
                uint number = (uint)(*current - '0');
                if (number < 10)
                {
                    ++current;
                    if (number == 0)
                    {
                        if (*current == 'x')
                        {
                            ++current;
                            parseHex32(ref number);
                            if (state != parseState.Success) return;
                            value = (byte)number;
                        }
                        else value = 0;
                    }
                    else value = (byte)parseUInt32(number);
                    searchValueEnd();
                    return;
                }
                if (number == '<' - '0')
                {
                    value = null;
                    searchValueEnd();
                    return;
                }
                state = parseState.NotNumber;
            }
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void parse(ref sbyte value)
        {
            searchValue();
            parseNumber(ref value);
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        private void parseNumber(ref sbyte value)
        {
            if (isCData != 2)
            {
                if ((sign = *current) == '-') ++current;
                uint number = (uint)(*current - '0');
                if (number < 10)
                {
                    ++current;
                    if (number == 0)
                    {
                        if (*current == 'x')
                        {
                            ++current;
                            parseHex32(ref number);
                            if (state != parseState.Success) return;
                            value = sign == '-' ? (sbyte)-(int)number : (sbyte)(byte)number;
                        }
                        else value = 0;
                    }
                    else value = sign == '-' ? (sbyte)-(int)parseUInt32(number) : (sbyte)(byte)parseUInt32(number);
                    searchValueEnd();
                    return;
                }
                state = parseState.NotNumber;
            }
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref sbyte? value)
        {
            searchValue();
            if (isCData != 2)
            {
                if ((sign = *current) == '-') ++current;
                uint number = (uint)(*current - '0');
                if (number < 10)
                {
                    ++current;
                    if (number == 0)
                    {
                        if (*current == 'x')
                        {
                            ++current;
                            parseHex32(ref number);
                            if (state != parseState.Success) return;
                            value = sign == '-' ? (sbyte)-(int)number : (sbyte)(byte)number;
                        }
                        else value = 0;
                    }
                    else value = sign == '-' ? (sbyte)-(int)parseUInt32(number) : (sbyte)(byte)parseUInt32(number);
                    searchValueEnd();
                    return;
                }
                if (sign == '<')
                {
                    value = null;
                    searchValueEnd();
                    return;
                }
                state = parseState.NotNumber;
            }
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void parse(ref ushort value)
        {
            searchValue();
            parseNumber(ref value);
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        private void parseNumber(ref ushort value)
        {
            if (isCData != 2)
            {
                uint number = (uint)(*current - '0');
                if (number < 10)
                {
                    ++current;
                    if (number == 0)
                    {
                        if (*current == 'x')
                        {
                            ++current;
                            parseHex32(ref number);
                            if (state != parseState.Success) return;
                            value = (ushort)number;
                        }
                        else value = 0;
                    }
                    else value = (ushort)parseUInt32(number);
                    searchValueEnd();
                    return;
                }
                state = parseState.NotNumber;
            }
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref ushort? value)
        {
            searchValue();
            if (isCData != 2)
            {
                uint number = (uint)(*current - '0');
                if (number < 10)
                {
                    ++current;
                    if (number == 0)
                    {
                        if (*current == 'x')
                        {
                            ++current;
                            parseHex32(ref number);
                            if (state != parseState.Success) return;
                            value = (ushort)number;
                        }
                        else value = 0;
                    }
                    else value = (ushort)parseUInt32(number);
                    searchValueEnd();
                    return;
                }
                if (number == '<' - '0')
                {
                    value = null;
                    searchValueEnd();
                    return;
                }
                state = parseState.NotNumber;
            }
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void parse(ref short value)
        {
            searchValue();
            parseNumber(ref value);
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        private void parseNumber(ref short value)
        {
            if (isCData != 2)
            {
                if ((sign = *current) == '-') ++current;
                uint number = (uint)(*current - '0');
                if (number < 10)
                {
                    ++current;
                    if (number == 0)
                    {
                        if (*current == 'x')
                        {
                            ++current;
                            parseHex32(ref number);
                            if (state != parseState.Success) return;
                            value = sign == '-' ? (short)-(int)number : (short)(ushort)number;
                        }
                        else value = 0;
                    }
                    else value = sign == '-' ? (short)-(int)parseUInt32(number) : (short)(ushort)parseUInt32(number);
                    searchValueEnd();
                    return;
                }
                state = parseState.NotNumber;
            }
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref short? value)
        {
            searchValue();
            if (isCData != 2)
            {
                if ((sign = *current) == '-') ++current;
                uint number = (uint)(*current - '0');
                if (number < 10)
                {
                    ++current;
                    if (number == 0)
                    {
                        if (*current == 'x')
                        {
                            ++current;
                            parseHex32(ref number);
                            if (state != parseState.Success) return;
                            value = sign == '-' ? (short)-(int)number : (short)(ushort)number;
                        }
                        else value = 0;
                    }
                    else value = sign == '-' ? (short)-(int)parseUInt32(number) : (short)(ushort)parseUInt32(number);
                    searchValueEnd();
                    return;
                }
                if (sign == '<')
                {
                    value = null;
                    searchValueEnd();
                    return;
                }
                state = parseState.NotNumber;
            }
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void parse(ref uint value)
        {
            searchValue();
            parseNumber(ref value);
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool UnsafeParse(ref uint value)
        {
            parse(ref value);
            return state == parseState.Success;
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        private void parseNumber(ref uint value)
        {
            if (isCData != 2)
            {
                uint number = (uint)(*current - '0');
                if (number < 10)
                {
                    ++current;
                    if (number == 0)
                    {
                        if (*current == 'x')
                        {
                            ++current;
                            parseHex32(ref number);
                            if (state != parseState.Success) return;
                            value = number;
                        }
                        else value = 0;
                    }
                    else value = parseUInt32(number);
                    searchValueEnd();
                    return;
                }
                state = parseState.NotNumber;
            }
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref uint? value)
        {
            searchValue();
            if (isCData != 2)
            {
                uint number = (uint)(*current - '0');
                if (number < 10)
                {
                    ++current;
                    if (number == 0)
                    {
                        if (*current == 'x')
                        {
                            ++current;
                            parseHex32(ref number);
                            if (state != parseState.Success) return;
                            value = number;
                        }
                        else value = 0;
                    }
                    else value = parseUInt32(number);
                    searchValueEnd();
                    return;
                }
                if (number == '<' - '0')
                {
                    value = null;
                    searchValueEnd();
                    return;
                }
                state = parseState.NotNumber;
            }
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool UnsafeParse(ref uint? value)
        {
            parse(ref value);
            return state == parseState.Success;
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void parse(ref int value)
        {
            searchValue();
            parseNumber(ref value);
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        private void parseNumber(ref int value)
        {
            if (isCData != 2)
            {
                if ((sign = *current) == '-') ++current;
                uint number = (uint)(*current - '0');
                if (number < 10)
                {
                    ++current;
                    if (number == 0)
                    {
                        if (*current == 'x')
                        {
                            ++current;
                            parseHex32(ref number);
                            if (state != parseState.Success) return;
                            value = sign == '-' ? -(int)number : (int)number;
                        }
                        else value = 0;
                    }
                    else value = sign == '-' ? -(int)parseUInt32(number) : (int)parseUInt32(number);
                    searchValueEnd();
                    return;
                }
                state = parseState.NotNumber;
            }
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref int? value)
        {
            searchValue();
            if (isCData != 2)
            {
                if ((sign = *current) == '-') ++current;
                uint number = (uint)(*current - '0');
                if (number < 10)
                {
                    ++current;
                    if (number == 0)
                    {
                        if (*current == 'x')
                        {
                            ++current;
                            parseHex32(ref number);
                            if (state != parseState.Success) return;
                            value = sign == '-' ? -(int)number : (int)number;
                        }
                        else value = 0;
                    }
                    else value = sign == '-' ? -(int)parseUInt32(number) : (int)parseUInt32(number);
                    searchValueEnd();
                    return;
                }
                if (sign == '<')
                {
                    value = null;
                    searchValueEnd();
                    return;
                }
                state = parseState.NotNumber;
            }
        }
        /// <summary>
        /// 解析10进制数字
        /// </summary>
        /// <param name="value">第一位数字</param>
        /// <returns>数字</returns>
        private ulong parseUInt64(uint value)
        {
            ulong number = value;
            while ((value = (uint)(*current - '0')) < 10)
            {
                number *= 10;
                ++current;
                number += value;
            }
            return number;
        }
        /// <summary>
        /// 解析16进制数字
        /// </summary>
        private ulong parseHex64()
        {
            ulong number = 0;
            uint isValue = 0, value;
            do
            {
                if ((value = (uint)(*current - '0')) > 9)
                {
                    if ((value = (value - ('A' - '0')) & 0xffdfU) > 5)
                    {
                        if (isValue == 0) state = parseState.NotHex;
                        return 0;
                    }
                    value += 10;
                }
                ++current;
                number <<= 4;
                isValue = 1;
                number += value;
            }
            while (true);
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void parse(ref ulong value)
        {
            searchValue();
            parseNumber(ref value);
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        private void parseNumber(ref ulong value)
        {
            if (isCData != 2)
            {
                uint number = (uint)(*current - '0');
                if (number < 10)
                {
                    ++current;
                    if (number == 0)
                    {
                        if (*current == 'x')
                        {
                            ++current;
                            value = parseHex64();
                            if (state != parseState.Success) return;
                        }
                        else value = 0;
                    }
                    else value = parseUInt64(number);
                    searchValueEnd();
                    return;
                }
                state = parseState.NotNumber;
            }
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref ulong? value)
        {
            searchValue();
            if (isCData != 2)
            {
                uint number = (uint)(*current - '0');
                if (number < 10)
                {
                    ++current;
                    if (number == 0)
                    {
                        if (*current == 'x')
                        {
                            ++current;
                            value = parseHex64();
                            if (state != parseState.Success) return;
                        }
                        else value = 0;
                    }
                    else value = parseUInt64(number);
                    searchValueEnd();
                    return;
                }
                if (number == '<' - '0')
                {
                    value = null;
                    searchValueEnd();
                    return;
                }
                state = parseState.NotNumber;
            }
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void parse(ref long value)
        {
            searchValue();
            parseNumber(ref value);
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        private void parseNumber(ref long value)
        {
            if (isCData != 2)
            {
                if ((sign = *current) == '-') ++current;
                uint number = (uint)(*current - '0');
                if (number < 10)
                {
                    ++current;
                    if (number == 0)
                    {
                        if (*current == 'x')
                        {
                            ++current;
                            value = sign == '-' ? -(long)parseHex64() : (long)parseHex64();
                            if (state != parseState.Success) return;
                        }
                        else value = 0;
                    }
                    else value = sign == '-' ? -(long)parseUInt64(number) : (long)parseUInt64(number);
                    searchValueEnd();
                    return;
                }
                state = parseState.NotNumber;
            }
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref long? value)
        {
            searchValue();
            if (isCData != 2)
            {
                if ((sign = *current) == '-') ++current;
                uint number = (uint)(*current - '0');
                if (number < 10)
                {
                    ++current;
                    if (number == 0)
                    {
                        if (*current == 'x')
                        {
                            ++current;
                            value = sign == '-' ? -(long)parseHex64() : (long)parseHex64();
                            if (state != parseState.Success) return;
                        }
                        else value = 0;
                    }
                    else value = sign == '-' ? -(long)parseUInt64(number) : (long)parseUInt64(number);
                    searchValueEnd();
                    return;
                }
                if (sign == '<')
                {
                    value = null;
                    searchValueEnd();
                    return;
                }
                state = parseState.NotNumber;
            }
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref float value)
        {
            getValue();
            if (state == parseState.Success)
            {
                if (valueSize != 0)
                {
                    if (valueSize == 3 && *valueStart == 'N' && *(int*)(valueStart + 1) == 'a' + ('N' << 16))
                    {
                        value = float.NaN;
                        return;
                    }
                    string number = new string(valueStart, 0, valueSize);
                    if (float.TryParse(number, out value))
                    {
                        getValueEnd();
                        return;
                    }
                }
                state = parseState.NotNumber;
            }
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref float? value)
        {
            getValue();
            if (state == parseState.Success)
            {
                if (valueSize == 0)
                {
                    value = null;
                    getValueEnd();
                }
                else
                {
                    if (valueSize == 3 && *valueStart == 'N' && *(int*)(valueStart + 1) == 'a' + ('N' << 16))
                    {
                        value = float.NaN;
                        return;
                    }
                    string numberString = new string(valueStart, 0, valueSize);
                    float number;
                    if (float.TryParse(numberString, out number))
                    {
                        value = number;
                        getValueEnd();
                    }
                    else state = parseState.NotNumber;
                }
            }
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref double value)
        {
            getValue();
            if (state == parseState.Success)
            {
                if (valueSize != 0)
                {
                    if (valueSize == 3 && *valueStart == 'N' && *(int*)(valueStart + 1) == 'a' + ('N' << 16))
                    {
                        value = double.NaN;
                        return;
                    }
                    string number = new string(valueStart, 0, valueSize);
                    if (double.TryParse(number, out value))
                    {
                        getValueEnd();
                        return;
                    }
                }
                state = parseState.NotNumber;
            }
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref double? value)
        {
            getValue();
            if (state == parseState.Success)
            {
                if (valueSize == 0)
                {
                    value = null;
                    getValueEnd();
                }
                else
                {
                    if (valueSize == 3 && *valueStart == 'N' && *(int*)(valueStart + 1) == 'a' + ('N' << 16))
                    {
                        value = double.NaN;
                        return;
                    }
                    string numberString = new string(valueStart, 0, valueSize);
                    double number;
                    if (double.TryParse(numberString, out number))
                    {
                        value = number;
                        getValueEnd();
                    }
                    else state = parseState.NotNumber;
                }
            }
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref decimal value)
        {
            getValue();
            if (state == parseState.Success)
            {
                if (valueSize != 0)
                {
                    string number = new string(valueStart, 0, valueSize);
                    if (decimal.TryParse(number, out value))
                    {
                        getValueEnd();
                        return;
                    }
                }
                state = parseState.NotNumber;
            }
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref decimal? value)
        {
            getValue();
            if (state == parseState.Success)
            {
                if (valueSize == 0)
                {
                    value = null;
                    getValueEnd();
                }
                else
                {
                    string numberString = new string(valueStart, 0, valueSize);
                    decimal number;
                    if (decimal.TryParse(numberString, out number))
                    {
                        value = number;
                        getValueEnd();
                    }
                    else state = parseState.NotNumber;
                }
            }
        }
        /// <summary>
        /// 字符解码
        /// </summary>
        /// <param name="value"></param>
        private void decodeChar(ref char value)
        {
            if (*++valueStart == '#')
            {
                uint code = (uint)(*++valueStart - '0');
                if (code < 10)
                {
                    do
                    {
                        uint number = (uint)(*++valueStart - '0');
                        if (number < 10) code = code * 10 + number;
                        else
                        {
                            if (number == ';' - '0')
                            {
                                ++valueStart;
                                value = (char)code;
                            }
                            else state = parseState.DecodeError;
                            return;
                        }
                    }
                    while (true);
                }
            }
            else
            {
                int code = decoder.UnsafeSearch(ref valueStart);
                if (code > 0)
                {
                    value = (char)code;
                    return;
                }
            }
            state = parseState.DecodeError;
        }
        /// <summary>
        /// 字符解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref char value)
        {
            getValue();
            if (valueSize == 1)
            {
                value = *valueStart;
                getValueEnd();
                return;
            }
            if ((isCData | (*valueStart ^ '&')) == 0)
            {
                decodeChar(ref value);
                if (state == parseState.Success) getValueEnd();
                return;
            }
            state = parseState.NotChar;
        }
        /// <summary>
        /// 字符解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref char? value)
        {
            getValue();
            if (state == parseState.Success)
            {
                switch (valueSize)
                {
                    case 0:
                        value = null;
                        getValueEnd();
                        return;
                    case 1:
                        value = *valueStart;
                        getValueEnd();
                        return;
                    default:
                        if ((isCData | (*valueStart ^ '&')) == 0)
                        {
                            char charValue = (char)0;
                            decodeChar(ref charValue);
                            if (state == parseState.Success)
                            {
                                value = charValue;
                                getValueEnd();
                            }
                            return;
                        }
                        break;
                }
                state = parseState.NotChar;
            }
        }
        /// <summary>
        /// 时间解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref DateTime value)
        {
            getValue();
            if (state == parseState.Success)
            {
                if (valueSize != 0)
                {
                    string number = new string(valueStart, 0, valueSize);
                    if (DateTime.TryParse(number, out value))
                    {
                        getValueEnd();
                        return;
                    }
                }
                state = parseState.NotDateTime;
            }
        }
        /// <summary>
        /// 时间解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref DateTime? value)
        {
            getValue();
            if (state == parseState.Success)
            {
                if (valueSize == 0)
                {
                    value = null;
                    getValueEnd();
                }
                else
                {
                    string numberString = new string(valueStart, 0, valueSize);
                    DateTime number;
                    if (DateTime.TryParse(numberString, out number))
                    {
                        value = number;
                        getValueEnd();
                    }
                    else state = parseState.NotDateTime;
                }
            }
        }
        /// <summary>
        /// 解析16进制字符
        /// </summary>
        /// <returns>字符</returns>
        private uint parseHex4()
        {
            uint code = (uint)(*valueStart++ - '0'), number = (uint)(*valueStart++ - '0');
            if (code > 9) code = ((code - ('A' - '0')) & 0xffdfU) + 10;
            if (number > 9) number = ((number - ('A' - '0')) & 0xffdfU) + 10;
            code <<= 12;
            code += (number << 8);
            if ((number = (uint)(*valueStart++ - '0')) > 9) number = ((number - ('A' - '0')) & 0xffdfU) + 10;
            code += (number << 4);
            number = (uint)(*valueStart++ - '0');
            return code + (number > 9 ? (((number - ('A' - '0')) & 0xffdfU) + 10) : number);
        }
        /// <summary>
        /// 解析16进制字符
        /// </summary>
        /// <returns>字符</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private uint parseHex2()
        {
            uint code = (uint)(*valueStart++ - '0'), number = (uint)(*valueStart++ - '0');
            if (code > 9) code = ((code - ('A' - '0')) & 0xffdfU) + 10;
            return (number > 9 ? (((number - ('A' - '0')) & 0xffdfU) + 10) : number) + (code << 4);
        }
        /// <summary>
        /// Guid解析
        /// </summary>
        /// <param name="value">数据</param>
        private void parse(ref guid value)
        {
            value.Byte3 = (byte)parseHex2();
            value.Byte2 = (byte)parseHex2();
            value.Byte1 = (byte)parseHex2();
            value.Byte0 = (byte)parseHex2();
            if (*valueStart++ != '-')
            {
                state = parseState.NotGuid;
                return;
            }
            value.Byte45 = (ushort)parseHex4();
            if (*valueStart++ != '-')
            {
                state = parseState.NotGuid;
                return;
            }
            value.Byte67 = (ushort)parseHex4();
            if (*valueStart++ != '-')
            {
                state = parseState.NotGuid;
                return;
            }
            value.Byte8 = (byte)parseHex2();
            value.Byte9 = (byte)parseHex2();
            if (*valueStart++ != '-')
            {
                state = parseState.NotGuid;
                return;
            }
            value.Byte10 = (byte)parseHex2();
            value.Byte11 = (byte)parseHex2();
            value.Byte12 = (byte)parseHex2();
            value.Byte13 = (byte)parseHex2();
            value.Byte14 = (byte)parseHex2();
            value.Byte15 = (byte)parseHex2();
        }
        /// <summary>
        /// Guid解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref Guid value)
        {
            getValue();
            if (state == parseState.Success)
            {
                if (valueSize == 36)
                {
                    guid guid = new guid();
                    parse(ref guid);
                    value = guid.Value;
                    getValueEnd();
                }
                else state = parseState.NotGuid;
            }
        }
        /// <summary>
        /// Guid解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref Guid? value)
        {
            getValue();
            if (state == parseState.Success)
            {
                if (valueSize == 36)
                {
                    guid guid = new guid();
                    parse(ref guid);
                    value = guid.Value;
                    getValueEnd();
                }
                else if (valueSize == 0)
                {
                    value = null;
                    getValueEnd();
                }
                else state = parseState.NotGuid;
            }
        }
        /// <summary>
        /// 字符串解码
        /// </summary>
        /// <param name="write"></param>
        /// <param name="writeEnd"></param>
        private void decodeString(char* write, char* writeEnd)
        {
            char decodeValue = (char)0;
            do
            {
                if (*valueStart == '&')
                {
                    decodeChar(ref decodeValue);
                    if (state != parseState.Success) return;
                    *write = decodeValue;
                }
                else *write = *valueStart++;
            }
            while (++write != writeEnd);
        }
        /// <summary>
        /// 字符串解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref string value)
        {
            space();
            if (state != parseState.Success) return;
            if (*current == '<')
            {
                if (*(current + 1) == '!')
                {
                    searchCData2();
                    if (state == parseState.Success) value = new string(valueStart, 0, valueSize);
                }
                else value = string.Empty;
            }
            else
            {
                valueStart = current;
                valueSize = 0;
                do
                {
                    if (*current == '<')
                    {
                        int length = (int)(endSpace() - valueStart);
                        if (valueSize == 0) value = new string(valueStart, 0, length);
                        else
                        {
                            fixed (char* valueFixed = value = fastCSharp.String.FastAllocateString(length - valueSize))
                            {
                                decodeString(valueFixed, valueFixed + value.Length);
                            }
                        }
                        return;
                    }
                    if (*current == '&')
                    {
                        do
                        {
                            ++valueSize;
                            if (*++current == ';') break;
                            if (*current == '<')
                            {
                                state = parseState.DecodeError;
                                return;
                            }
                        }
                        while (true);
                    }
                    ++current;
                }
                while (true);
            }
        }
        /// <summary>
        /// 字符串解析
        /// </summary>
        /// <param name="value">数据</param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool UnsafeParse(ref string value)
        {
            parse(ref value);
            return state == parseState.Success;
        }
        /// <summary>
        /// 字符串解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref subString value)
        {
            space();
            if (state != parseState.Success) return;
            if (*current == '<')
            {
                if (*(current + 1) == '!')
                {
                    searchCData2();
                    if (state == parseState.Success) value.UnsafeSet(xml, (int)(valueStart - xmlFixed), valueSize);
                }
                else value.UnsafeSet(string.Empty, 0, 0);
            }
            else
            {
                valueStart = current;
                valueSize = 0;
                do
                {
                    if (*current == '<')
                    {
                        int length = (int)(endSpace() - valueStart);
                        if (valueSize == 0) value.UnsafeSet(xml, (int)(valueStart - xmlFixed), length);
                        else if (Config.IsTempString)
                        {
                            value.UnsafeSet(xml, (int)(valueStart - xmlFixed), length - valueSize);
                            while (*valueStart != '&') ++valueStart;
                            decodeString(valueStart, xmlFixed + value.StartIndex + value.Length);
                        }
                        else
                        {
                            string decodeValue = fastCSharp.String.FastAllocateString(length - valueSize);
                            fixed (char* valueFixed = decodeValue) decodeString(valueFixed, valueFixed + decodeValue.Length);
                            value.UnsafeSet(decodeValue, 0, decodeValue.Length);
                        }
                        return;
                    }
                    if (*current == '&')
                    {
                        do
                        {
                            ++valueSize;
                            if (*++current == ';') break;
                            if (*current == '<')
                            {
                                state = parseState.DecodeError;
                                return;
                            }
                        }
                        while (true);
                    }
                    ++current;
                }
                while (true);
            }
        }
        /// <summary>
        /// XML节点解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref xmlNode value)
        {
            space();
            if (state != parseState.Success) return;
            if (*current == '<')
            {
                char code = *(current + 1);
                if (((bits[code & 0xff] & targetStartCheckBit) | (code & 0xff00)) == 0)
                {
                    if (code == '/')
                    {
                        value.SetString(string.Empty);
                        return;
                    }
                    if (code == '!')
                    {
                        searchCData2();
                        if (state == parseState.Success) value.SetString(xml, (int)(valueStart - xmlFixed), valueSize);
                        return;
                    }
                    state = parseState.NotFoundTagStart;
                    return;
                }
                char* nameStart;
                subArray<keyValue<subString, xmlNode>> nodes = default(subArray<keyValue<subString, xmlNode>>);
                keyValue<xmlParser.attributeIndex, xmlParser.attributeIndex>[] attributes;
                int nameSize = 0;
                do
                {
                    nameStart = getName(ref nameSize);
                    if (state != parseState.Success) return;
                    if (nameStart == null)
                    {
                        value.SetNode(ref nodes);
                        return;
                    }
                    nodes.PrepLength(1);
                    nodes.array[nodes.length].Key.UnsafeSet(xml, (int)(nameStart - xmlFixed), nameSize);
                    attributes = Config.IsAttribute && this.attributes.length != 0 ? this.attributes.GetArray() : null;
                    if (isTagEnd == 0)
                    {
                        parse(ref nodes.array[nodes.length].Value);
                        if (state != parseState.Success || checkNameEnd(nameStart, nameSize) == 0) return;
                    }
                    if (attributes != null) nodes.array[nodes.length].Value.SetAttribute(xml, attributes);
                    nodes.UnsafeSetLength(nodes.length + 1);
                }
                while (true);
            }
            else
            {
                valueStart = current;
                value.Type = xmlNode.type.String;
                do
                {
                    if (*current == '<')
                    {
                        value.String.UnsafeSet(xml, (int)(valueStart - xmlFixed), (int)(endSpace() - valueStart));
                        if (Config.IsTempString && value.Type == xmlNode.type.EncodeString) value.Type = xmlNode.type.TempString;
                        return;
                    }
                    if (*current == '&')
                    {
                        value.Type = xmlNode.type.EncodeString;
                        while (*++current != ';')
                        {
                            if (*current == '<')
                            {
                                state = parseState.DecodeError;
                                return;
                            }
                        }
                    }
                    ++current;
                }
                while (true);
            }
        }
        /// <summary>
        /// 对象解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref object value)
        {
            ignoreValue();
        }
        /// <summary>
        /// 设置匿名类型数据
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="value"></param>
        private void setAnonymousType<valueType>(valueType value)
        {
            foreach (keyValue<Type, object> type in anonymousTypes)
            {
                if (type.Key == typeof(valueType)) return;
            }
            anonymousTypes.Add(new keyValue<Type, object>(typeof(valueType), memberCopyer<valueType>.MemberwiseClone(value)));
        }

        /// <summary>
        /// 值类型对象解析
        /// </summary>
        /// <param name="value">目标数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void structParse<valueType>(ref valueType value) where valueType : struct
        {
            typeParser<valueType>.ParseStruct(this, ref value);
        }
        /// <summary>
        /// 集合构造解析函数信息
        /// </summary>
        private static readonly MethodInfo structParseMethod = typeof(xmlParser).GetMethod("structParse", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 值类型对象解析
        /// </summary>
        /// <param name="value">目标数据</param>
        private void nullableParse<valueType>(ref Nullable<valueType> value) where valueType : struct
        {
            valueType newValue = value.HasValue ? value.Value : default(valueType);
            typeParser<valueType>.Parse(this, ref newValue);
            value = newValue;
        }
        /// <summary>
        /// 集合构造解析函数信息
        /// </summary>
        private static readonly MethodInfo nullableParseMethod = typeof(xmlParser).GetMethod("nullableParse", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 值类型对象解析
        /// </summary>
        /// <param name="value">目标数据</param>
        private void nullableEnumParse<valueType>(ref Nullable<valueType> value) where valueType : struct
        {
            space();
            if (state == parseState.Success)
            {
                if (*current == '<')
                {
                    if (((*(int*)(current + 1) ^ ('!' + ('[' << 16))) | (*(int*)(current + 3) ^ ('C' + ('D' << 16))) | (*(int*)(current + 5) ^ ('A' + ('T' << 16))) | (*(int*)(current + 7) ^ ('A' + ('[' << 16)))) == 0)
                    {
                        if (((*(int*)(current + 9) ^ (']' + (']' << 16))) | (*(short*)(current + 11) ^ '>')) == 0)
                        {
                            current += 12;
                            value = null;
                            return;
                        }
                    }
                    else
                    {
                        value = null;
                        return;
                    }
                }
                valueType newValue = value.HasValue ? value.Value : default(valueType);
                typeParser<valueType>.DefaultParser(this, ref newValue);
                value = newValue;
            }
        }
        /// <summary>
        /// 集合构造解析函数信息
        /// </summary>
        private static readonly MethodInfo nullableEnumParseMethod = typeof(xmlParser).GetMethod("nullableEnumParse", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 值类型对象解析
        /// </summary>
        /// <param name="value">目标数据</param>
        private void keyValuePairParse<keyType, valueType>(ref KeyValuePair<keyType, valueType> value)
        {
            keyValue<keyType, valueType> keyValue = new keyValue<keyType, valueType>(value.Key, value.Value);
            typeParser<keyValue<keyType, valueType>>.ParseMembers(this, ref keyValue);
            value = new KeyValuePair<keyType, valueType>(keyValue.Key, keyValue.Value);
        }
        /// <summary>
        /// 值类型对象解析函数信息
        /// </summary>
        private static readonly MethodInfo keyValuePairParseMethod = typeof(xmlParser).GetMethod("keyValuePairParse", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 引用类型对象解析
        /// </summary>
        /// <param name="value">目标数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void typeParse<valueType>(ref valueType value)
        {
            typeParser<valueType>.ParseClass(this, ref value);
        }
        /// <summary>
        /// 引用类型对象解析函数信息
        /// </summary>
        private static readonly MethodInfo typeParseMethod = typeof(xmlParser).GetMethod("typeParse", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 找不到构造函数
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="value">目标数据</param>
        /// <param name="isAnonymousType"></param>
        private void checkNoConstructor<valueType>(ref valueType value, bool isAnonymousType)
        {
            Func<Type, object> constructor = Config.Constructor;
            if (constructor == null)
            {
                if (isAnonymousType)
                {
                    foreach (keyValue<Type, object> type in anonymousTypes)
                    {
                        if (type.Key == typeof(valueType))
                        {
                            value = memberCopyer<valueType>.MemberwiseClone((valueType)type.Value);
                            return;
                        }
                    }
                }
                ignoreValue();
                return;
            }
            object newValue = constructor(typeof(valueType));
            if (newValue == null)
            {
                ignoreValue();
                return;
            }
            value = (valueType)newValue;
        }
        ///// <summary>
        ///// 找不到构造函数
        ///// </summary>
        ///// <typeparam name="valueType"></typeparam>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //private bool checkNoConstructor<valueType>(ref valueType value)
        //{
        //    if (value == null)
        //    {
        //        Func<Type, object> constructor = Config.Constructor;
        //        if (constructor == null)
        //        {
        //            state = parseState.NoConstructor;
        //            return false;
        //        }
        //        object newValue = constructor(typeof(valueType));
        //        if (newValue == null)
        //        {
        //            state = parseState.NoConstructor;
        //            return false;
        //        }
        //        value = (valueType)newValue;
        //    }
        //    return true;
        //}
        ///// <summary>
        ///// 找不到构造函数
        ///// </summary>
        ///// <param name="value">目标数据</param>
        //private void noConstructor<valueType>(ref valueType value)
        //{
        //    if (checkNoConstructor(ref value)) typeParser<valueType>.ParseClass(this, ref value);
        //}
        ///// <summary>
        ///// 找不到构造函数解析函数信息
        ///// </summary>
        //private static readonly MethodInfo noConstructorMethod = typeof(xmlParser).GetMethod("noConstructor", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组解析
        /// </summary>
        /// <param name="values">目标数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void array<valueType>(ref valueType[] values)
        {
            typeParser<valueType>.Array(this, ref values);
        }
        /// <summary>
        /// 数组解析函数信息
        /// </summary>
        private static readonly MethodInfo arrayMethod = typeof(xmlParser).GetMethod("array", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 基类转换
        /// </summary>
        /// <param name="value">目标数据</param>
        private void baseParse<valueType, childType>(ref childType value) where childType : valueType
        {
            if (value == null)
            {
                Func<childType> constructor = fastCSharp.emit.constructor<childType>.New;
                if (constructor == null)
                {
                    checkNoConstructor(ref value, false);
                    if (value == null) return;
                }
                else value = constructor();
                valueType newValue = value;
                typeParser<valueType>.ParseMembers(this, ref newValue);
            }
            else
            {
                valueType newValue = value;
                typeParser<valueType>.ParseClass(this, ref newValue);
            }
        }
        /// <summary>
        /// 基类转换函数信息
        /// </summary>
        private static readonly MethodInfo baseParseMethod = typeof(xmlParser).GetMethod("baseParse", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举值解析
        /// </summary>
        /// <param name="value">目标数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void enumByte<valueType>(ref valueType value)
        {
            typeParser<valueType>.enumByte.Parse(this, ref value);
        }
        /// <summary>
        /// 枚举值解析
        /// </summary>
        /// <param name="value">目标数据</param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool UnsafeEnumByte<valueType>(ref valueType value)
        {
            typeParser<valueType>.enumByte.Parse(this, ref value);
            return state == parseState.Success;
        }
        /// <summary>
        /// 枚举值解析函数信息
        /// </summary>
        private static readonly MethodInfo enumByteMethod = typeof(xmlParser).GetMethod("enumByte", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举值解析
        /// </summary>
        /// <param name="value">目标数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void enumSByte<valueType>(ref valueType value)
        {
            typeParser<valueType>.enumSByte.Parse(this, ref value);
        }
        /// <summary>
        /// 枚举值解析函数信息
        /// </summary>
        private static readonly MethodInfo enumSByteMethod = typeof(xmlParser).GetMethod("enumSByte", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举值解析
        /// </summary>
        /// <param name="value">目标数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void enumShort<valueType>(ref valueType value)
        {
            typeParser<valueType>.enumShort.Parse(this, ref value);
        }
        /// <summary>
        /// 枚举值解析函数信息
        /// </summary>
        private static readonly MethodInfo enumShortMethod = typeof(xmlParser).GetMethod("enumShort", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举值解析
        /// </summary>
        /// <param name="value">目标数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void enumUShort<valueType>(ref valueType value)
        {
            typeParser<valueType>.enumUShort.Parse(this, ref value);
        }
        /// <summary>
        /// 枚举值解析函数信息
        /// </summary>
        private static readonly MethodInfo enumUShortMethod = typeof(xmlParser).GetMethod("enumUShort", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举值解析
        /// </summary>
        /// <param name="value">目标数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void enumInt<valueType>(ref valueType value)
        {
            typeParser<valueType>.enumInt.Parse(this, ref value);
        }
        /// <summary>
        /// 枚举值解析函数信息
        /// </summary>
        private static readonly MethodInfo enumIntMethod = typeof(xmlParser).GetMethod("enumInt", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举值解析
        /// </summary>
        /// <param name="value">目标数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void enumUInt<valueType>(ref valueType value)
        {
            typeParser<valueType>.enumUInt.Parse(this, ref value);
        }
        /// <summary>
        /// 枚举值解析函数信息
        /// </summary>
        private static readonly MethodInfo enumUIntMethod = typeof(xmlParser).GetMethod("enumUInt", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举值解析
        /// </summary>
        /// <param name="value">目标数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void enumLong<valueType>(ref valueType value)
        {
            typeParser<valueType>.enumLong.Parse(this, ref value);
        }
        /// <summary>
        /// 枚举值解析函数信息
        /// </summary>
        private static readonly MethodInfo enumLongMethod = typeof(xmlParser).GetMethod("enumLong", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举值解析
        /// </summary>
        /// <param name="value">目标数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void enumULong<valueType>(ref valueType value)
        {
            typeParser<valueType>.enumULong.Parse(this, ref value);
        }
        /// <summary>
        /// 枚举值解析函数信息
        /// </summary>
        private static readonly MethodInfo enumULongMethod = typeof(xmlParser).GetMethod("enumULong", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举值解析
        /// </summary>
        /// <param name="value">目标数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void enumByteFlags<valueType>(ref valueType value)
        {
            typeParser<valueType>.enumByte.ParseFlags(this, ref value);
        }
        /// <summary>
        /// 枚举值解析函数信息
        /// </summary>
        private static readonly MethodInfo enumByteFlagsMethod = typeof(xmlParser).GetMethod("enumByteFlags", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举值解析
        /// </summary>
        /// <param name="value">目标数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void enumSByteFlags<valueType>(ref valueType value)
        {
            typeParser<valueType>.enumSByte.ParseFlags(this, ref value);
        }
        /// <summary>
        /// 枚举值解析函数信息
        /// </summary>
        private static readonly MethodInfo enumSByteFlagsMethod = typeof(xmlParser).GetMethod("enumSByteFlags", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举值解析
        /// </summary>
        /// <param name="value">目标数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void enumShortFlags<valueType>(ref valueType value)
        {
            typeParser<valueType>.enumShort.ParseFlags(this, ref value);
        }
        /// <summary>
        /// 枚举值解析函数信息
        /// </summary>
        private static readonly MethodInfo enumShortFlagsMethod = typeof(xmlParser).GetMethod("enumShortFlags", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举值解析
        /// </summary>
        /// <param name="value">目标数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void enumUShortFlags<valueType>(ref valueType value)
        {
            typeParser<valueType>.enumUShort.ParseFlags(this, ref value);
        }
        /// <summary>
        /// 枚举值解析函数信息
        /// </summary>
        private static readonly MethodInfo enumUShortFlagsMethod = typeof(xmlParser).GetMethod("enumUShortFlags", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举值解析
        /// </summary>
        /// <param name="value">目标数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void enumIntFlags<valueType>(ref valueType value)
        {
            typeParser<valueType>.enumInt.ParseFlags(this, ref value);
        }
        /// <summary>
        /// 枚举值解析函数信息
        /// </summary>
        private static readonly MethodInfo enumIntFlagsMethod = typeof(xmlParser).GetMethod("enumIntFlags", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举值解析
        /// </summary>
        /// <param name="value">目标数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void enumUIntFlags<valueType>(ref valueType value)
        {
            typeParser<valueType>.enumUInt.ParseFlags(this, ref value);
        }
        /// <summary>
        /// 枚举值解析函数信息
        /// </summary>
        private static readonly MethodInfo enumUIntFlagsMethod = typeof(xmlParser).GetMethod("enumUIntFlags", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举值解析
        /// </summary>
        /// <param name="value">目标数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void enumLongFlags<valueType>(ref valueType value)
        {
            typeParser<valueType>.enumLong.ParseFlags(this, ref value);
        }
        /// <summary>
        /// 枚举值解析函数信息
        /// </summary>
        private static readonly MethodInfo enumLongFlagsMethod = typeof(xmlParser).GetMethod("enumLongFlags", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举值解析
        /// </summary>
        /// <param name="value">目标数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void enumULongFlags<valueType>(ref valueType value)
        {
            typeParser<valueType>.enumULong.ParseFlags(this, ref value);
        }
        /// <summary>
        /// 枚举值解析函数信息
        /// </summary>
        private static readonly MethodInfo enumULongFlagsMethod = typeof(xmlParser).GetMethod("enumULongFlags", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 集合构造函数解析
        /// </summary>
        /// <param name="value">目标数据</param>
        private void dictionaryConstructor<dictionaryType, keyType, valueType>(ref dictionaryType value)
        {
            KeyValuePair<keyType, valueType>[] values = null;
            int count = typeParser<KeyValuePair<keyType, valueType>>.ArrayIndex(this, ref values);
            if (count == -1) value = default(dictionaryType);
            else
            {
                Dictionary<keyType, valueType> dictionary = fastCSharp.dictionary.CreateAny<keyType, valueType>(count);
                if (count != 0)
                {
                    foreach (KeyValuePair<keyType, valueType> keyValue in values)
                    {
                        dictionary.Add(keyValue.Key, keyValue.Value);
                        if (--count == 0) break;
                    }
                }
                value = pub.dictionaryConstructor<dictionaryType, keyType, valueType>.Constructor(dictionary);
            }
        }
        /// <summary>
        /// 集合构造解析函数信息
        /// </summary>
        private static readonly MethodInfo dictionaryConstructorMethod = typeof(xmlParser).GetMethod("dictionaryConstructor", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 集合构造函数解析
        /// </summary>
        /// <param name="value">目标数据</param>
        private void listConstructor<valueType, argumentType>(ref valueType value)
        {
            argumentType[] values = null;
            int count = typeParser<argumentType>.ArrayIndex(this, ref values);
            if (count == -1) value = default(valueType);
            else value = pub.listConstructor<valueType, argumentType>.Constructor(subArray<argumentType>.Unsafe(values, 0, count));
        }
        /// <summary>
        /// 集合构造解析函数信息
        /// </summary>
        private static readonly MethodInfo listConstructorMethod = typeof(xmlParser).GetMethod("listConstructor", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 集合构造函数解析
        /// </summary>
        /// <param name="value">目标数据</param>
        private void collectionConstructor<valueType, argumentType>(ref valueType value)
        {
            argumentType[] values = null;
            int count = typeParser<argumentType>.ArrayIndex(this, ref values);
            if (count == -1) value = default(valueType);
            else value = pub.collectionConstructor<valueType, argumentType>.Constructor(subArray<argumentType>.Unsafe(values, 0, count));
        }
        /// <summary>
        /// 集合构造解析函数信息
        /// </summary>
        private static readonly MethodInfo collectionConstructorMethod = typeof(xmlParser).GetMethod("collectionConstructor", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 集合构造函数解析
        /// </summary>
        /// <param name="value">目标数据</param>
        private void enumerableConstructor<valueType, argumentType>(ref valueType value)
        {
            argumentType[] values = null;
            int count = typeParser<argumentType>.ArrayIndex(this, ref values);
            if (count == -1) value = default(valueType);
            else value = pub.enumerableConstructor<valueType, argumentType>.Constructor(subArray<argumentType>.Unsafe(values, 0, count));
        }
        /// <summary>
        /// 集合构造解析函数信息
        /// </summary>
        private static readonly MethodInfo enumerableConstructorMethod = typeof(xmlParser).GetMethod("enumerableConstructor", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 集合构造函数解析
        /// </summary>
        /// <param name="value">目标数据</param>
        private void arrayConstructor<valueType, argumentType>(ref valueType value)
        {
            argumentType[] values = null;
            typeParser<argumentType>.Array(this, ref values);
            if (state == parseState.Success)
            {
                if (values == null) value = default(valueType);
                else value = pub.arrayConstructor<valueType, argumentType>.Constructor(values);
            }
        }
        /// <summary>
        /// 数组构造解析函数信息
        /// </summary>
        private static readonly MethodInfo arrayConstructorMethod = typeof(xmlParser).GetMethod("arrayConstructor", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// 公共默认配置参数
        /// </summary>
        private static readonly config defaultConfig = new config();
        /// <summary>
        /// XML解析
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="xml">XML字符串</param>
        /// <param name="config">配置参数</param>
        /// <returns>目标数据</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType Parse<valueType>(subString xml, config config = null)
        {
            return Parse<valueType>(ref xml, config);
        }
        /// <summary>
        /// XML解析
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="xml">XML字符串</param>
        /// <param name="config">配置参数</param>
        /// <returns>目标数据</returns>
        public static valueType Parse<valueType>(ref subString xml, config config = null)
        {
            if (xml.Length == 0)
            {
                if (config != null) config.State = parseState.NullXml;
                return default(valueType);
            }
            valueType value = default(valueType);
            xmlParser parser = typePool<xmlParser>.Pop() ?? new xmlParser();
            try
            {
                return parser.parse<valueType>(ref xml, ref value, config ?? defaultConfig) == parseState.Success ? value : default(valueType);
            }
            finally { parser.free(); }
        }
        /// <summary>
        /// XML解析
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="xml">XML字符串</param>
        /// <param name="value">目标数据</param>
        /// <param name="config">配置参数</param>
        /// <returns>是否解析成功</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static bool Parse<valueType>(subString xml, ref valueType value, config config = null)
        {
            return Parse(ref xml, ref value, config);
        }
        /// <summary>
        /// XML解析
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="xml">XML字符串</param>
        /// <param name="value">目标数据</param>
        /// <param name="config">配置参数</param>
        /// <returns>是否解析成功</returns>
        public static bool Parse<valueType>(ref subString xml, ref valueType value, config config = null)
        {
            if (xml.Length == 0)
            {
                if (config != null) config.State = parseState.NullXml;
                return false;
            }
            xmlParser parser = typePool<xmlParser>.Pop() ?? new xmlParser();
            try
            {
                return parser.parse<valueType>(ref xml, ref value, config ?? defaultConfig) == parseState.Success;
            }
            finally { parser.free(); }
        }
        /// <summary>
        /// XML字符串解析
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static bool ParseEncodeString(ref subString xml)
        {
            xmlParser parser = typePool<xmlParser>.Pop() ?? new xmlParser();
            try
            {
                return parser.parseEncodeString(ref xml);
            }
            finally { parser.free(); }
        }
        /// <summary>
        /// XML字符串解析
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static bool ParseTempString(ref subString xml)
        {
            xmlParser parser = typePool<xmlParser>.Pop() ?? new xmlParser();
            try
            {
                return parser.parseTempString(ref xml);
            }
            finally { parser.free(); }
        }

        /// <summary>
        /// 字符Decode转码
        /// </summary>
        private static readonly pointer.reference decodeChars;
        /// <summary>
        /// 基本类型解析函数
        /// </summary>
        private static readonly Dictionary<Type, MethodInfo> parseMethods;
        /// <summary>
        /// 获取基本类型解析函数
        /// </summary>
        /// <param name="type">基本类型</param>
        /// <returns>解析函数</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static MethodInfo getParseMethod(Type type)
        {
            MethodInfo method;
            return parseMethods.TryGetValue(type, out method) ? method : null;
        }
        /// <summary>
        /// XML解析空格[ ,\t,\r,\n]
        /// </summary>
        private const byte spaceBit = 128;
        /// <summary>
        /// XML解析名称检测
        /// </summary>
        private const byte targetStartCheckBit = 64;
        /// <summary>
        /// XML解析属性名称查找
        /// </summary>
        private const byte attributeNameSearchBit = 32;
        /// <summary>
        /// XML序列化转换字符[ ,\t,\r,\n,&amp;,>,&lt;]
        /// </summary>
        internal const byte EncodeSpaceBit = 8;
        /// <summary>
        /// XML序列化转换字符[&amp;,>,&lt;]
        /// </summary>
        internal const byte EncodeBit = 4;
        /// <summary>
        /// 字符状态位
        /// </summary>
        internal static readonly pointer.reference Bits;
        static xmlParser()
        {
            byte* bits = (Bits = unmanaged.GetStatic(256, false).Reference).Byte;
            fastCSharp.unsafer.memory.Fill(bits, ulong.MaxValue, 256 >> 3);
            bits['\t'] &= (spaceBit | targetStartCheckBit | attributeNameSearchBit) ^ 255;
            bits['\r'] &= (spaceBit | targetStartCheckBit | attributeNameSearchBit) ^ 255;
            bits['\n'] &= (spaceBit | targetStartCheckBit | attributeNameSearchBit) ^ 255;
            bits[' '] &= (spaceBit | targetStartCheckBit | attributeNameSearchBit) ^ 255;
            bits['/'] &= (targetStartCheckBit | attributeNameSearchBit) ^ 255;
            bits['!'] &= targetStartCheckBit ^ 255;
            bits['<'] &= targetStartCheckBit ^ 255;
            bits['>'] &= (targetStartCheckBit | attributeNameSearchBit) ^ 255;
            bits['='] &= attributeNameSearchBit ^ 255;

            parseMethods = fastCSharp.dictionary.CreateOnly<Type, MethodInfo>();
            foreach (MethodInfo method in typeof(xmlParser).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (method.customAttribute<parseMethod>() != null)
                {
                    parseMethods.Add(method.GetParameters()[0].ParameterType.GetElementType(), method);
                }
            }

            keyValue<string, int>[] chars = new keyValue<string, int>[]
            {
                new keyValue<string, int>("AElig;", 198)
                , new keyValue<string, int>("Aacute;", 193)
                , new keyValue<string, int>("Acirc;", 194)
                , new keyValue<string, int>("Agrave;", 192)
                , new keyValue<string, int>("Alpha;", 913)
                , new keyValue<string, int>("Aring;", 197)
                , new keyValue<string, int>("Atilde;", 195)
                , new keyValue<string, int>("Auml;", 196)
                , new keyValue<string, int>("Beta;", 914)
                , new keyValue<string, int>("Ccedil;", 199)
                , new keyValue<string, int>("Chi;", 935)
                , new keyValue<string, int>("Dagger;", 8225)
                , new keyValue<string, int>("Delta;", 916)
                , new keyValue<string, int>("ETH;", 208)
                , new keyValue<string, int>("Eacute;", 201)
                , new keyValue<string, int>("Ecirc;", 202)
                , new keyValue<string, int>("Egrave;", 200)
                , new keyValue<string, int>("Epsilon;", 917)
                , new keyValue<string, int>("Eta;", 919)
                , new keyValue<string, int>("Euml;", 203)
                , new keyValue<string, int>("Gamma;", 915)
                , new keyValue<string, int>("Iacute;", 205)
                , new keyValue<string, int>("Icirc;", 206)
                , new keyValue<string, int>("Igrave;", 204)
                , new keyValue<string, int>("Iota;", 921)
                , new keyValue<string, int>("Iuml;", 207)
                , new keyValue<string, int>("Kappa;", 922)
                , new keyValue<string, int>("Lambda;", 923)
                , new keyValue<string, int>("Mu;", 924)
                , new keyValue<string, int>("Ntilde;", 209)
                , new keyValue<string, int>("Nu;", 925)
                , new keyValue<string, int>("OElig;", 338)
                , new keyValue<string, int>("Oacute;", 211)
                , new keyValue<string, int>("Ocirc;", 212)
                , new keyValue<string, int>("Ograve;", 210)
                , new keyValue<string, int>("Omega;", 937)
                , new keyValue<string, int>("Omicron;", 927)
                , new keyValue<string, int>("Oslash;", 216)
                , new keyValue<string, int>("Otilde;", 213)
                , new keyValue<string, int>("Ouml;", 214)
                , new keyValue<string, int>("Phi;", 934)
                , new keyValue<string, int>("Pi;", 928)
                , new keyValue<string, int>("Prime;", 8243)
                , new keyValue<string, int>("Psi;", 936)
                , new keyValue<string, int>("Rho;", 929)
                , new keyValue<string, int>("Scaron;", 352)
                , new keyValue<string, int>("Sigma;", 931)
                , new keyValue<string, int>("THORN;", 222)
                , new keyValue<string, int>("Tau;", 932)
                , new keyValue<string, int>("Theta;", 920)
                , new keyValue<string, int>("Uacute;", 218)
                , new keyValue<string, int>("Ucirc;", 219)
                , new keyValue<string, int>("Ugrave;", 217)
                , new keyValue<string, int>("Upsilon;", 933)
                , new keyValue<string, int>("Uuml;", 220)
                , new keyValue<string, int>("Xi;", 926)
                , new keyValue<string, int>("Yacute;", 221)
                , new keyValue<string, int>("Yuml;", 376)
                , new keyValue<string, int>("Zeta;", 918)
                , new keyValue<string, int>("aacute;", 225)
                , new keyValue<string, int>("acirc;", 226)
                , new keyValue<string, int>("acute;", 180)
                , new keyValue<string, int>("aelig;", 230)
                , new keyValue<string, int>("agrave;", 224)
                , new keyValue<string, int>("alefsym;", 8501)
                , new keyValue<string, int>("alpha;", 945)
                , new keyValue<string, int>("amp;", 38)
                , new keyValue<string, int>("and;", 8743)
                , new keyValue<string, int>("ang;", 8736)
                , new keyValue<string, int>("aring;", 229)
                , new keyValue<string, int>("asymp;", 8776)
                , new keyValue<string, int>("atilde;", 227)
                , new keyValue<string, int>("auml;", 228)
                , new keyValue<string, int>("bdquo;", 8222)
                , new keyValue<string, int>("beta;", 946)
                , new keyValue<string, int>("brvbar;", 166)
                , new keyValue<string, int>("bull;", 8226)
                , new keyValue<string, int>("cap;", 8745)
                , new keyValue<string, int>("ccedil;", 231)
                , new keyValue<string, int>("cedil;", 184)
                , new keyValue<string, int>("cent;", 162)
                , new keyValue<string, int>("chi;", 967)
                , new keyValue<string, int>("circ;", 710)
                , new keyValue<string, int>("clubs;", 9827)
                , new keyValue<string, int>("cong;", 8773)
                , new keyValue<string, int>("copy;", 169)
                , new keyValue<string, int>("crarr;", 8629)
                , new keyValue<string, int>("cup;", 8746)
                , new keyValue<string, int>("curren;", 164)
                , new keyValue<string, int>("dArr;", 8659)
                , new keyValue<string, int>("dagger;", 8224)
                , new keyValue<string, int>("darr;", 8595)
                , new keyValue<string, int>("deg;", 176)
                , new keyValue<string, int>("delta;", 948)
                , new keyValue<string, int>("diams;", 9830)
                , new keyValue<string, int>("divide;", 247)
                , new keyValue<string, int>("eacute;", 233)
                , new keyValue<string, int>("ecirc;", 234)
                , new keyValue<string, int>("egrave;", 232)
                , new keyValue<string, int>("empty;", 8709)
                , new keyValue<string, int>("emsp;", 8195)
                , new keyValue<string, int>("ensp;", 8194)
                , new keyValue<string, int>("epsilon;", 949)
                , new keyValue<string, int>("equiv;", 8801)
                , new keyValue<string, int>("eta;", 951)
                , new keyValue<string, int>("eth;", 240)
                , new keyValue<string, int>("euml;", 235)
                , new keyValue<string, int>("euro;", 8364)
                , new keyValue<string, int>("exist;", 8707)
                , new keyValue<string, int>("fnof;", 402)
                , new keyValue<string, int>("forall;", 8704)
                , new keyValue<string, int>("frac12;", 189)
                , new keyValue<string, int>("frac14;", 188)
                , new keyValue<string, int>("frac34;", 190)
                , new keyValue<string, int>("frasl;", 8260)
                , new keyValue<string, int>("gamma;", 947)
                , new keyValue<string, int>("ge;", 8805)
                , new keyValue<string, int>("gt;", 62)
                , new keyValue<string, int>("hArr;", 8660)
                , new keyValue<string, int>("harr;", 8596)
                , new keyValue<string, int>("hearts;", 9829)
                , new keyValue<string, int>("hellip;", 8230)
                , new keyValue<string, int>("iacute;", 237)
                , new keyValue<string, int>("icirc;", 238)
                , new keyValue<string, int>("iexcl;", 161)
                , new keyValue<string, int>("igrave;", 236)
                , new keyValue<string, int>("image;", 8465)
                , new keyValue<string, int>("infin;", 8734)
                , new keyValue<string, int>("int;", 8747)
                , new keyValue<string, int>("iota;", 953)
                , new keyValue<string, int>("iquest;", 191)
                , new keyValue<string, int>("isin;", 8712)
                , new keyValue<string, int>("iuml;", 239)
                , new keyValue<string, int>("kappa;", 954)
                , new keyValue<string, int>("lArr;", 8656)
                , new keyValue<string, int>("lambda;", 955)
                , new keyValue<string, int>("lang;", 9001)
                , new keyValue<string, int>("laquo;", 171)
                , new keyValue<string, int>("larr;", 8592)
                , new keyValue<string, int>("lceil;", 8968)
                , new keyValue<string, int>("ldquo;", 8220)
                , new keyValue<string, int>("le;", 8804)
                , new keyValue<string, int>("lfloor;", 8970)
                , new keyValue<string, int>("lowast;", 8727)
                , new keyValue<string, int>("loz;", 9674)
                , new keyValue<string, int>("lrm;", 8206)
                , new keyValue<string, int>("lsaquo;", 8249)
                , new keyValue<string, int>("lsquo;", 8216)
                , new keyValue<string, int>("lt;", 60)
                , new keyValue<string, int>("macr;", 175)
                , new keyValue<string, int>("mdash;", 8212)
                , new keyValue<string, int>("micro;", 181)
                , new keyValue<string, int>("middot;", 183)
                , new keyValue<string, int>("minus;", 8722)
                , new keyValue<string, int>("mu;", 956)
                , new keyValue<string, int>("nabla;", 8711)
                , new keyValue<string, int>("nbsp;", 160)
                , new keyValue<string, int>("ndash;", 8211)
                , new keyValue<string, int>("ne;", 8800)
                , new keyValue<string, int>("ni;", 8715)
                , new keyValue<string, int>("not;", 172)
                , new keyValue<string, int>("notin;", 8713)
                , new keyValue<string, int>("nsub;", 8836)
                , new keyValue<string, int>("ntilde;", 241)
                , new keyValue<string, int>("nu;", 957)
                , new keyValue<string, int>("oacute;", 243)
                , new keyValue<string, int>("ocirc;", 244)
                , new keyValue<string, int>("oelig;", 339)
                , new keyValue<string, int>("ograve;", 242)
                , new keyValue<string, int>("oline;", 8254)
                , new keyValue<string, int>("omega;", 969)
                , new keyValue<string, int>("omicron;", 959)
                , new keyValue<string, int>("oplus;", 8853)
                , new keyValue<string, int>("or;", 8744)
                , new keyValue<string, int>("ordf;", 170)
                , new keyValue<string, int>("ordm;", 186)
                , new keyValue<string, int>("oslash;", 248)
                , new keyValue<string, int>("otilde;", 245)
                , new keyValue<string, int>("otimes;", 8855)
                , new keyValue<string, int>("ouml;", 246)
                , new keyValue<string, int>("para;", 182)
                , new keyValue<string, int>("part;", 8706)
                , new keyValue<string, int>("permil;", 8240)
                , new keyValue<string, int>("perp;", 8869)
                , new keyValue<string, int>("phi;", 966)
                , new keyValue<string, int>("pi;", 960)
                , new keyValue<string, int>("piv;", 982)
                , new keyValue<string, int>("plusmn;", 177)
                , new keyValue<string, int>("pound;", 163)
                , new keyValue<string, int>("prime;", 8242)
                , new keyValue<string, int>("prod;", 8719)
                , new keyValue<string, int>("prop;", 8733)
                , new keyValue<string, int>("psi;", 968)
                , new keyValue<string, int>("quot;", 34)
                , new keyValue<string, int>("rArr;", 8658)
                , new keyValue<string, int>("radic;", 8730)
                , new keyValue<string, int>("rang;", 9002)
                , new keyValue<string, int>("raquo;", 187)
                , new keyValue<string, int>("rarr;", 8594)
                , new keyValue<string, int>("rceil;", 8969)
                , new keyValue<string, int>("rdquo;", 8221)
                , new keyValue<string, int>("real;", 8476)
                , new keyValue<string, int>("reg;", 174)
                , new keyValue<string, int>("rfloor;", 8971)
                , new keyValue<string, int>("rho;", 961)
                , new keyValue<string, int>("rlm;", 8207)
                , new keyValue<string, int>("rsaquo;", 8250)
                , new keyValue<string, int>("rsquo;", 8217)
                , new keyValue<string, int>("sbquo;", 8218)
                , new keyValue<string, int>("scaron;", 353)
                , new keyValue<string, int>("sdot;", 8901)
                , new keyValue<string, int>("sect;", 167)
                , new keyValue<string, int>("shy;", 173)
                , new keyValue<string, int>("sigma;", 963)
                , new keyValue<string, int>("sigmaf;", 962)
                , new keyValue<string, int>("sim;", 8764)
                , new keyValue<string, int>("spades;", 9824)
                , new keyValue<string, int>("sub;", 8834)
                , new keyValue<string, int>("sube;", 8838)
                , new keyValue<string, int>("sum;", 8721)
                , new keyValue<string, int>("sup1;", 185)
                , new keyValue<string, int>("sup2;", 178)
                , new keyValue<string, int>("sup3;", 179)
                , new keyValue<string, int>("sup;", 8835)
                , new keyValue<string, int>("supe;", 8839)
                , new keyValue<string, int>("szlig;", 223)
                , new keyValue<string, int>("tau;", 964)
                , new keyValue<string, int>("there4;", 8756)
                , new keyValue<string, int>("theta;", 952)
                , new keyValue<string, int>("thetasym;", 977)
                , new keyValue<string, int>("thinsp;", 8201)
                , new keyValue<string, int>("thorn;", 254)
                , new keyValue<string, int>("tilde;", 732)
                , new keyValue<string, int>("times;", 215)
                , new keyValue<string, int>("trade;", 8482)
                , new keyValue<string, int>("uArr;", 8657)
                , new keyValue<string, int>("uacute;", 250)
                , new keyValue<string, int>("uarr;", 8593)
                , new keyValue<string, int>("ucirc;", 251)
                , new keyValue<string, int>("ugrave;", 249)
                , new keyValue<string, int>("uml;", 168)
                , new keyValue<string, int>("upsih;", 978)
                , new keyValue<string, int>("upsilon;", 965)
                , new keyValue<string, int>("uuml;", 252)
                , new keyValue<string, int>("weierp;", 8472)
                , new keyValue<string, int>("xi;", 958)
                , new keyValue<string, int>("yacute;", 253)
                , new keyValue<string, int>("yen;", 165)
                , new keyValue<string, int>("yuml;", 255)
                , new keyValue<string, int>("zeta;", 950)
                , new keyValue<string, int>("zwj;", 8205)
                , new keyValue<string, int>("zwnj;", 8204)
            };
            decodeChars = new fastCSharp.stateSearcher.asciiSearcher.stringBuilder(chars, true).Data.Reference;
        }
    }
}
