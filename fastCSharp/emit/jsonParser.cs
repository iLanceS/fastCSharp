using System;
using System.Collections.Generic;
using System.Reflection;
using fastCSharp.code;
using fastCSharp.reflection;
using System.Threading;
using fastCSharp.threading;
using System.Runtime.CompilerServices;
#if NOJIT
#else
using System.Reflection.Emit;
#endif

namespace fastCSharp.emit
{
    /// <summary>
    /// Json解析器
    /// </summary>
    public unsafe sealed class jsonParser
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
            /// Json字符串参数为空
            /// </summary>
            NullJson,
            /// <summary>
            /// 解析目标对象参数为空
            /// </summary>
            NullValue,
            /// <summary>
            /// 非正常意外结束
            /// </summary>
            CrashEnd,
            /// <summary>
            /// 未能识别的注释
            /// </summary>
            UnknownNote,
            /// <summary>
            /// /**/注释缺少回合
            /// </summary>
            NoteNotRound,
            /// <summary>
            /// null值解析失败
            /// </summary>
            NotNull,
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
            /// 字符串解析失败
            /// </summary>
            NotString,
            /// <summary>
            /// 字符串被换行截断
            /// </summary>
            StringEnter,
            /// <summary>
            /// 时间解析错误
            /// </summary>
            NotDateTime,
            /// <summary>
            /// Guid解析错误
            /// </summary>
            NotGuid,
            /// <summary>
            /// 不支持多维数组
            /// </summary>
            ArrayManyRank,
            /// <summary>
            /// 数组解析错误
            /// </summary>
            NotArray,
            /// <summary>
            /// 数组数据解析错误
            /// </summary>
            NotArrayValue,
            ///// <summary>
            ///// 不支持指针
            ///// </summary>
            //Pointer,
            ///// <summary>
            ///// 找不到构造函数
            ///// </summary>
            //NoConstructor,
            /// <summary>
            /// 非枚举字符
            /// </summary>
            NotEnumChar,
            /// <summary>
            /// 没有找到匹配的枚举值
            /// </summary>
            NoFoundEnumValue,
            /// <summary>
            /// 对象解析错误
            /// </summary>
            NotObject,
            /// <summary>
            /// 没有找到成员名称
            /// </summary>
            NotFoundName,
            /// <summary>
            /// 没有找到冒号
            /// </summary>
            NotFoundColon,
            /// <summary>
            /// 忽略值解析错误
            /// </summary>
            UnknownValue,
            /// <summary>
            /// 字典解析错误
            /// </summary>
            NotDictionary,
            /// <summary>
            /// 类型解析错误
            /// </summary>
            ErrorType,
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
            /// 解析状态
            /// </summary>
            public parseState State { get; internal set; }
            /// <summary>
            /// 自定义构造函数
            /// </summary>
            public Func<Type, object> Constructor;
            /// <summary>
            /// 成员选择
            /// </summary>
            public fastCSharp.code.memberFilters MemberFilter = code.memberFilters.Instance;
            /// <summary>
            /// 错误时是否输出JSON字符串
            /// </summary>
            public bool IsOutputError = true;
            /// <summary>
            /// 对象解析结束后是否检测最后的空格符
            /// </summary>
            public bool IsEndSpace = true;
            /// <summary>
            /// 是否临时字符串(可修改)
            /// </summary>
            public bool IsTempString;
            /// <summary>
            /// 是否强制匹配枚举值
            /// </summary>
            public bool IsMatchEnum;
            /// <summary>
            /// 清空数据
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void Null()
            {
                MemberMap = null;
                Constructor = null;
            }
        }
        /// <summary>
        /// 名称状态查找器
        /// </summary>
        internal struct stateSearcher
        {
            /// <summary>
            /// Json解析器
            /// </summary>
            private jsonParser parser;
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
            /// <param name="parser">Json解析器</param>
            /// <param name="data">数据起始位置</param>
            internal stateSearcher(jsonParser parser, pointer.reference data)
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
            /// 名称查找器
            /// </summary>
            /// <param name="parser">Json解析器</param>
            /// <param name="data">数据起始位置</param>
            internal void Set(jsonParser parser, pointer.reference data)
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
            /// <summary>
            /// 根据字符串查找目标索引
            /// </summary>
            /// <returns>目标索引,null返回-1</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal int SearchString()
            {
                char value = parser.searchQuote();
                if (parser.State != parseState.Success || state == null) return -1;
                currentState = state;
                return searchString(value);
            }
            /// <summary>
            /// 获取名称索引
            /// </summary>
            /// <param name="isQuote">名称是否带引号</param>
            /// <returns>名称索引,失败返回-1</returns>
            internal int SearchName(out bool isQuote)
            {
                char value = parser.getFirstName();
                if (state == null)
                {
                    isQuote = parser.quote != 0;
                    return -1;
                }
                if (parser.quote != 0)
                {
                    isQuote = true;
                    currentState = state;
                    return searchString(value);
                }
                isQuote = false;
                if (parser.State != parseState.Success) return -1;
                currentState = state;
                do
                {
                    char* prefix = (char*)(currentState + *(int*)currentState);
                    if (*prefix != 0)
                    {
                        if (value != *prefix) return -1;
                        while (*++prefix != 0)
                        {
                            if (parser.getNextName() != *prefix) return -1;
                        }
                        value = parser.getNextName();
                    }
                    if (value == 0) return parser.State == parseState.Success ? *(int*)(currentState + sizeof(int) * 2) : -1;
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
                    value = parser.getNextName();
                }
                while (true);
            }
            /// <summary>
            /// 根据字符串查找目标索引
            /// </summary>
            /// <param name="value">第一个字符</param>
            /// <returns>目标索引,null返回-1</returns>
            internal int searchString(char value)
            {
                do
                {
                    char* prefix = (char*)(currentState + *(int*)currentState);
                    if (*prefix != 0)
                    {
                        if (value != *prefix) return -1;
                        while (*++prefix != 0)
                        {
                            if (parser.nextStringChar() != *prefix) return -1;
                        }
                        value = parser.nextStringChar();
                    }
                    if (value == 0) return parser.State == parseState.Success ? *(int*)(currentState + sizeof(int) * 2) : -1;
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
                    value = parser.nextStringChar();
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
                currentState = state;
                return flagEnum(parser.searchEnumQuote());
            }
            /// <summary>
            /// 根据枚举字符串查找目标索引
            /// </summary>
            /// <returns>目标索引,null返回-1</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal int NextFlagEnum()
            {
                currentState = state;
                return flagEnum(parser.searchNextEnum());
            }
            /// <summary>
            /// 根据枚举字符串查找目标索引
            /// </summary>
            /// <param name="value">当前字符</param>
            /// <returns>目标索引,null返回-1</returns>
            private int flagEnum(char value)
            {
                do
                {
                    char* prefix = (char*)(currentState + *(int*)currentState);
                    if (*prefix != 0)
                    {
                        if (value != *prefix) return -1;
                        while (*++prefix != 0)
                        {
                            if (parser.nextEnumChar() != *prefix) return -1;
                        }
                        value = parser.nextEnumChar();
                    }
                    if (value == 0 || value == ',') return parser.State == parseState.Success ? *(int*)(currentState + sizeof(int) * 2) : -1;
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
                    value = parser.nextEnumChar();
                }
                while (true);
            }
            /// <summary>
            /// 成员名称查找数据
            /// </summary>
            private static readonly interlocked.dictionary<Type, pointer.reference> memberSearchers = new interlocked.dictionary<Type, pointer.reference>();
            /// <summary>
            /// 成员名称查找数据创建锁
            /// </summary>
            private static readonly object memberSearcherLock = new object();
            /// <summary>
            /// 获取成员名称查找数据
            /// </summary>
            /// <param name="type">定义类型</param>
            /// <param name="names">成员名称集合</param>
            /// <returns>成员名称查找数据</returns>
            internal static pointer.reference GetMemberSearcher(Type type, string[] names)
            {
                if (type.IsGenericType) type = type.GetGenericTypeDefinition();
                pointer.reference data;
                if (memberSearchers.TryGetValue(type, out data)) return data;
                Monitor.Enter(memberSearcherLock);
                if (memberSearchers.TryGetValue(type, out data))
                {
                    Monitor.Exit(memberSearcherLock);
                    return data;
                }
                try
                {
                    memberSearchers.Set(type, data = fastCSharp.stateSearcher.charsSearcher.Create(names, true).Reference);
                }
                finally { Monitor.Exit(memberSearcherLock); }
                return data;
            }
        }
        /// <summary>
        /// 类型解析器静态信息
        /// </summary>
        internal static class staticTypeParser
        {
            /// <summary>
            /// 带长度的指针的引用类型
            /// </summary>
            public static readonly Type PointerSizeRefType = typeof(pointer.size).MakeByRefType();
            /// <summary>
            /// JSON节点的引用类型
            /// </summary>
            public static readonly Type JsonNodeRefType = typeof(jsonNode).MakeByRefType();
            /// <summary>
            /// 获取字段成员集合
            /// </summary>
            /// <param name="fields"></param>
            /// <param name="typeAttribute">类型配置</param>
            /// <param name="defaultMember">默认解析字段</param>
            /// <returns>字段成员集合</returns>
            public static subArray<fieldIndex> GetFields(fieldIndex[] fields, jsonParse typeAttribute, ref fieldIndex defaultMember)
            {
                subArray<fieldIndex> values = new subArray<fieldIndex>(fields.Length);
                foreach (fieldIndex field in fields)
                {
                    Type type = field.Member.FieldType;
                    if (!type.IsPointer && (!type.IsArray || type.GetArrayRank() == 1) && !field.IsIgnore)
                    {
                        jsonParse.member attribute = field.GetAttribute<jsonParse.member>(true, true);
                        if (typeAttribute.IsAllMember ? (attribute == null || attribute.IsSetup) : (attribute != null && attribute.IsSetup))
                        {
                            if (attribute != null && attribute.IsDefault) defaultMember = field;
                            values.Add(field);
                        }
                    }
                }
                return values;
            }
            /// <summary>
            /// 获取属性成员集合
            /// </summary>
            /// <param name="properties">属性成员集合</param>
            /// <param name="typeAttribute">类型配置</param>
            /// <returns>属性成员集合</returns>
            public static subArray<keyValue<propertyIndex, MethodInfo>> GetProperties(propertyIndex[] properties, jsonParse typeAttribute)
            {
                subArray<keyValue<propertyIndex, MethodInfo>> values = new subArray<keyValue<propertyIndex, MethodInfo>>(properties.Length);
                foreach (propertyIndex property in properties)
                {
                    if (property.Member.CanWrite)
                    {
                     Type   type = property.Member.PropertyType;
                        if (!type.IsPointer && (!type.IsArray || type.GetArrayRank() == 1) && !property.IsIgnore)
                        {
                            jsonParse.member attribute = property.GetAttribute<jsonParse.member>(true, true);
                            if (typeAttribute.IsAllMember ? (attribute == null || attribute.IsSetup) : (attribute != null && attribute.IsSetup))
                            {
                                MethodInfo method = property.Member.GetSetMethod(true);
                                if (method != null && method.GetParameters().Length == 1)
                                {
                                    values.Add(new keyValue<propertyIndex, MethodInfo>(property, method));
                                }
                            }
                        }
                    }
                }
                return values;
            }
            /// <summary>
            /// 获取字段成员集合
            /// </summary>
            /// <param name="fieldIndexs"></param>
            /// <param name="properties"></param>
            /// <param name="typeAttribute"></param>
            /// <returns>字段成员集合</returns>
            public static subArray<memberIndex> GetMembers(fieldIndex[] fieldIndexs, propertyIndex[] properties, jsonParse typeAttribute)
            {
                subArray<memberIndex> members = new subArray<memberIndex>(fieldIndexs.Length + properties.Length);
                foreach (fieldIndex field in fieldIndexs)
                {
                    Type type = field.Member.FieldType;
                    if (!type.IsPointer && (!type.IsArray || type.GetArrayRank() == 1) && !field.IsIgnore)
                    {
                        jsonParse.member attribute = field.GetAttribute<jsonParse.member>(true, true);
                        if (typeAttribute.IsAllMember ? (attribute == null || attribute.IsSetup) : (attribute != null && attribute.IsSetup)) members.Add(field);
                    }
                }
                foreach (propertyIndex property in properties)
                {
                    if (property.Member.CanRead && property.Member.CanWrite)
                    {
                        Type type = property.Member.PropertyType;
                        if (!type.IsPointer && (!type.IsArray || type.GetArrayRank() == 1) && !property.IsIgnore)
                        {
                            jsonParse.member attribute = property.GetAttribute<jsonParse.member>(true, true);
                            if (typeAttribute.IsAllMember ? (attribute == null || attribute.IsSetup) : (attribute != null && attribute.IsSetup))
                            {
                                MethodInfo method = property.Member.GetSetMethod(true);
                                if (method != null && method.GetParameters().Length == 1) members.Add(property);
                            }
                        }
                    }
                }
                return members;
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
                DynamicMethod dynamicMethod = new DynamicMethod("jsonParser" + field.Name, null, new Type[] { typeof(jsonParser), type.MakeByRefType() }, type, true);
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
                DynamicMethod dynamicMethod = new DynamicMethod("jsonParser" + property.Name, null, new Type[] { typeof(jsonParser), type.MakeByRefType() }, type, true);
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
                MethodInfo methodInfo = jsonParser.getParseMethod(type);
                if (methodInfo != null) return methodInfo;
                if (type.IsArray) return staticTypeParser.GetArrayParser(type.GetElementType());
                if (type.IsEnum) return staticTypeParser.GetEnumParser(type);
                if (type.IsGenericType)
                {
                    Type genericType = type.GetGenericTypeDefinition();
                    if (genericType == typeof(Dictionary<,>)) return staticTypeParser.GetDictionaryParser(type);
                    if (genericType == typeof(Nullable<>)) return staticTypeParser.GetNullableParser(type);
                    if (genericType == typeof(KeyValuePair<,>)) return staticTypeParser.GetKeyValuePairParser(type);
                }
                if ((methodInfo = staticTypeParser.GetCustomParser(type)) != null) return methodInfo;
                //if (type.IsAbstract || type.IsInterface) return typeParser.GetNoConstructorParser(type);
                if ((methodInfo = staticTypeParser.GetIEnumerableConstructorParser(type)) != null) return methodInfo;
                if (type.IsValueType) return staticTypeParser.GetValueTypeParser(type);
                return staticTypeParser.GetTypeParser(type);
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
            //    method = checkNoConstructorMethod.MakeGenericMethod(type);
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
                valueTypeParsers.Set(type, method = structParseMethod.MakeGenericMethod(type));
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
                //if (type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, nullValue<Type>.Array, null) == null) method = checkNoConstructorMethod.MakeGenericMethod(type);
                //else
                    method = typeParseMethod.MakeGenericMethod(type);
                typeParsers.Set(type, method);
                return method;
            }
            /// <summary>
            /// 字典解析调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> dictionaryParsers = new interlocked.dictionary<Type, MethodInfo>();
            /// <summary>
            /// 获取字典解析调用函数信息
            /// </summary>
            /// <param name="type">数据类型</param>
            /// <returns>字典解析调用函数信息</returns>
            public static MethodInfo GetDictionaryParser(Type type)
            {
                MethodInfo method;
                if (dictionaryParsers.TryGetValue(type, out method)) return method;
                method = dictionaryMethod.MakeGenericMethod(type.GetGenericArguments());
                dictionaryParsers.Set(type, method);
                return method;
            }
            /// <summary>
            /// 可空类型解析调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> nullableParsers = new interlocked.dictionary<Type, MethodInfo>();
            /// <summary>
            /// 获取可空类型解析调用函数信息
            /// </summary>
            /// <param name="type">数据类型</param>
            /// <returns>可空类型解析调用函数信息</returns>
            public static MethodInfo GetNullableParser(Type type)
            {
                MethodInfo method;
                if (nullableParsers.TryGetValue(type, out method)) return method;
                Type[] parameterTypes = type.GetGenericArguments();
                method = (parameterTypes[0].IsEnum ? nullableEnumParseMethod : nullableParseMethod).MakeGenericMethod(parameterTypes);
                nullableParsers.Set(type, method);
                return method;
            }
            /// <summary>
            /// 键值对解析调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> keyValuePairParsers = new interlocked.dictionary<Type, MethodInfo>();
            /// <summary>
            /// 获取键值对解析调用函数信息
            /// </summary>
            /// <param name="type">数据类型</param>
            /// <returns>键值对解析调用函数信息</returns>
            public static MethodInfo GetKeyValuePairParser(Type type)
            {
                MethodInfo method;
                if (keyValuePairParsers.TryGetValue(type, out method)) return method;
                keyValuePairParsers.Set(type, method = keyValuePairParseMethod.MakeGenericMethod(type.GetGenericArguments()));
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
                        if (parameters.Length == 2 && parameters[0].ParameterType == typeof(jsonParser) && parameters[1].ParameterType == refType)
                        {
                            if (methodInfo.GetAttribute<jsonParse.custom>(true) != null)
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
                /// <param name="parser">Json解析器</param>
                /// <param name="value">目标数据</param>
                protected static void parse(jsonParser parser, ref valueType value)
                {
                    int index = new stateSearcher(parser, enumSearcher).SearchString();
                    if (index != -1) value = enumValues[index];
                    else if (parser.Config.IsMatchEnum) parser.State = parseState.NoFoundEnumValue;
                    else if (parser.State == parseState.Success) parser.searchStringEnd();
                }
                /// <summary>
                /// 枚举值解析
                /// </summary>
                /// <param name="parser">Json解析器</param>
                /// <param name="value">目标数据</param>
                /// <param name="searcher">名称状态查找器</param>
                /// <param name="index">第一个枚举索引</param>
                /// <param name="nextIndex">第二个枚举索引</param>
                protected static void getIndex(jsonParser parser, ref valueType value, ref stateSearcher searcher, out int index, ref int nextIndex)
                {
                    if ((index = searcher.SearchFlagEnum()) == -1)
                    {
                        if (parser.Config.IsMatchEnum)
                        {
                            parser.State = parseState.NoFoundEnumValue;
                            return;
                        }
                        do
                        {
                            if (parser.State != parseState.Success || parser.quote == 0) return;
                        }
                        while ((index = searcher.NextFlagEnum()) == -1);
                    }
                    do
                    {
                        if (parser.quote == 0)
                        {
                            value = enumValues[index];
                            return;
                        }
                        if ((nextIndex = searcher.NextFlagEnum()) != -1) break;
                        if (parser.State != parseState.Success) return;
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
                /// <param name="parser">Json解析器</param>
                /// <param name="value">目标数据</param>
                public static void Parse(jsonParser parser, ref valueType value)
                {
                    if (parser.isEnumNumber())
                    {
                        byte intValue = 0;
                        parser.Parse(ref intValue);
                        value = pub.enumCast<valueType, byte>.FromInt(intValue);
                    }
                    else if (parser.State == parseState.Success) parse(parser, ref value);
                }
                /// <summary>
                /// 枚举值解析
                /// </summary>
                /// <param name="parser">Json解析器</param>
                /// <param name="value">目标数据</param>
                public static void ParseFlags(jsonParser parser, ref valueType value)
                {
                    if (parser.isEnumNumber())
                    {
                        byte intValue = 0;
                        parser.Parse(ref intValue);
                        value = pub.enumCast<valueType, byte>.FromInt(intValue);
                    }
                    else if (parser.State == parseState.Success)
                    {
                        if (enumSearcher.Data == null)
                        {
                            if (parser.Config.IsMatchEnum) parser.State = parseState.NoFoundEnumValue;
                            else parser.ignore();
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
                                while (parser.quote != 0)
                                {
                                    index = searcher.NextFlagEnum();
                                    if (parser.State != parseState.Success) return;
                                    if (index != -1) intValue |= enumInts.Byte[index];
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
                /// <param name="parser">Json解析器</param>
                /// <param name="value">目标数据</param>
                public static void Parse(jsonParser parser, ref valueType value)
                {
                    if (parser.isEnumNumberFlag())
                    {
                        sbyte intValue = 0;
                        parser.Parse(ref intValue);
                        value = pub.enumCast<valueType, sbyte>.FromInt(intValue);
                    }
                    else if (parser.State == parseState.Success) parse(parser, ref value);
                }
                /// <summary>
                /// 枚举值解析
                /// </summary>
                /// <param name="parser">Json解析器</param>
                /// <param name="value">目标数据</param>
                public static void ParseFlags(jsonParser parser, ref valueType value)
                {
                    if (parser.isEnumNumberFlag())
                    {
                        sbyte intValue = 0;
                        parser.Parse(ref intValue);
                        value = pub.enumCast<valueType, sbyte>.FromInt(intValue);
                    }
                    else if (parser.State == parseState.Success)
                    {
                        if (enumSearcher.Data == null)
                        {
                            if (parser.Config.IsMatchEnum) parser.State = parseState.NoFoundEnumValue;
                            else parser.ignore();
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
                                while (parser.quote != 0)
                                {
                                    index = searcher.NextFlagEnum();
                                    if (parser.State != parseState.Success) return;
                                    if (index != -1) intValue |= (byte)enumInts.SByte[index];
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
                /// <param name="parser">Json解析器</param>
                /// <param name="value">目标数据</param>
                public static void Parse(jsonParser parser, ref valueType value)
                {
                    if (parser.isEnumNumberFlag())
                    {
                        short intValue = 0;
                        parser.Parse(ref intValue);
                        value = pub.enumCast<valueType, short>.FromInt(intValue);
                    }
                    else if (parser.State == parseState.Success) parse(parser, ref value);
                }
                /// <summary>
                /// 枚举值解析
                /// </summary>
                /// <param name="parser">Json解析器</param>
                /// <param name="value">目标数据</param>
                public static void ParseFlags(jsonParser parser, ref valueType value)
                {
                    if (parser.isEnumNumberFlag())
                    {
                        short intValue = 0;
                        parser.Parse(ref intValue);
                        value = pub.enumCast<valueType, short>.FromInt(intValue);
                    }
                    else if (parser.State == parseState.Success)
                    {
                        if (enumSearcher.Data == null)
                        {
                            if (parser.Config.IsMatchEnum) parser.State = parseState.NoFoundEnumValue;
                            else parser.ignore();
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
                                while (parser.quote != 0)
                                {
                                    index = searcher.NextFlagEnum();
                                    if (parser.State != parseState.Success) return;
                                    if (index != -1) intValue |= (ushort)enumInts.Short[index];
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
                /// <param name="parser">Json解析器</param>
                /// <param name="value">目标数据</param>
                public static void Parse(jsonParser parser, ref valueType value)
                {
                    if (parser.isEnumNumber())
                    {
                        ushort intValue = 0;
                        parser.Parse(ref intValue);
                        value = pub.enumCast<valueType, ushort>.FromInt(intValue);
                    }
                    else if (parser.State == parseState.Success) parse(parser, ref value);
                }
                /// <summary>
                /// 枚举值解析
                /// </summary>
                /// <param name="parser">Json解析器</param>
                /// <param name="value">目标数据</param>
                public static void ParseFlags(jsonParser parser, ref valueType value)
                {
                    if (parser.isEnumNumber())
                    {
                        ushort intValue = 0;
                        parser.Parse(ref intValue);
                        value = pub.enumCast<valueType, ushort>.FromInt(intValue);
                    }
                    else if (parser.State == parseState.Success)
                    {
                        if (enumSearcher.Data == null)
                        {
                            if (parser.Config.IsMatchEnum) parser.State = parseState.NoFoundEnumValue;
                            else parser.ignore();
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
                                while (parser.quote != 0)
                                {
                                    index = searcher.NextFlagEnum();
                                    if (parser.State != parseState.Success) return;
                                    if (index != -1) intValue |= enumInts.UShort[index];
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
                /// <param name="parser">Json解析器</param>
                /// <param name="value">目标数据</param>
                public static void Parse(jsonParser parser, ref valueType value)
                {
                    if (parser.isEnumNumberFlag())
                    {
                        int intValue = 0;
                        parser.Parse(ref intValue);
                        value = pub.enumCast<valueType, int>.FromInt(intValue);
                    }
                    else if (parser.State == parseState.Success) parse(parser, ref value);
                }
                /// <summary>
                /// 枚举值解析
                /// </summary>
                /// <param name="parser">Json解析器</param>
                /// <param name="value">目标数据</param>
                public static void ParseFlags(jsonParser parser, ref valueType value)
                {
                    if (parser.isEnumNumberFlag())
                    {
                        int intValue = 0;
                        parser.Parse(ref intValue);
                        value = pub.enumCast<valueType, int>.FromInt(intValue);
                    }
                    else if (parser.State == parseState.Success)
                    {
                        if (enumSearcher.Data == null)
                        {
                            if (parser.Config.IsMatchEnum) parser.State = parseState.NoFoundEnumValue;
                            else parser.ignore();
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
                                while (parser.quote != 0)
                                {
                                    index = searcher.NextFlagEnum();
                                    if (parser.State != parseState.Success) return;
                                    if (index != -1) intValue |= enumInts.Int[index];
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
                /// <param name="parser">Json解析器</param>
                /// <param name="value">目标数据</param>
                public static void Parse(jsonParser parser, ref valueType value)
                {
                    if (parser.isEnumNumber())
                    {
                        uint intValue = 0;
                        parser.Parse(ref intValue);
                        value = pub.enumCast<valueType, uint>.FromInt(intValue);
                    }
                    else if (parser.State == parseState.Success) parse(parser, ref value);
                }
                /// <summary>
                /// 枚举值解析
                /// </summary>
                /// <param name="parser">Json解析器</param>
                /// <param name="value">目标数据</param>
                public static void ParseFlags(jsonParser parser, ref valueType value)
                {
                    if (parser.isEnumNumber())
                    {
                        uint intValue = 0;
                        parser.Parse(ref intValue);
                        value = pub.enumCast<valueType, uint>.FromInt(intValue);
                    }
                    else if (parser.State == parseState.Success)
                    {
                        if (enumSearcher.Data == null)
                        {
                            if (parser.Config.IsMatchEnum) parser.State = parseState.NoFoundEnumValue;
                            else parser.ignore();
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
                                while (parser.quote != 0)
                                {
                                    index = searcher.NextFlagEnum();
                                    if (parser.State != parseState.Success) return;
                                    if (index != -1) intValue |= enumInts.UInt[index];
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
                /// <param name="parser">Json解析器</param>
                /// <param name="value">目标数据</param>
                public static void Parse(jsonParser parser, ref valueType value)
                {
                    if (parser.isEnumNumberFlag())
                    {
                        long intValue = 0;
                        parser.Parse(ref intValue);
                        value = pub.enumCast<valueType, long>.FromInt(intValue);
                    }
                    else if (parser.State == parseState.Success) parse(parser, ref value);
                }
                /// <summary>
                /// 枚举值解析
                /// </summary>
                /// <param name="parser">Json解析器</param>
                /// <param name="value">目标数据</param>
                public static void ParseFlags(jsonParser parser, ref valueType value)
                {
                    if (parser.isEnumNumberFlag())
                    {
                        long intValue = 0;
                        parser.Parse(ref intValue);
                        value = pub.enumCast<valueType, long>.FromInt(intValue);
                    }
                    else if (parser.State == parseState.Success)
                    {
                        if (enumSearcher.Data == null)
                        {
                            if (parser.Config.IsMatchEnum) parser.State = parseState.NoFoundEnumValue;
                            else parser.ignore();
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
                                while (parser.quote != 0)
                                {
                                    index = searcher.NextFlagEnum();
                                    if (parser.State != parseState.Success) return;
                                    if (index != -1) intValue |= enumInts.Long[index];
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
                /// <param name="parser">Json解析器</param>
                /// <param name="value">目标数据</param>
                public static void Parse(jsonParser parser, ref valueType value)
                {
                    if (parser.isEnumNumber())
                    {
                        ulong intValue = 0;
                        parser.Parse(ref intValue);
                        value = pub.enumCast<valueType, ulong>.FromInt(intValue);
                    }
                    else if (parser.State == parseState.Success) parse(parser, ref value);
                }
                /// <summary>
                /// 枚举值解析
                /// </summary>
                /// <param name="parser">Json解析器</param>
                /// <param name="value">目标数据</param>
                public static void ParseFlags(jsonParser parser, ref valueType value)
                {
                    if (parser.isEnumNumber())
                    {
                        ulong intValue = 0;
                        parser.Parse(ref intValue);
                        value = pub.enumCast<valueType, ulong>.FromInt(intValue);
                    }
                    else if (parser.State == parseState.Success)
                    {
                        if (enumSearcher.Data == null)
                        {
                            if (parser.Config.IsMatchEnum) parser.State = parseState.NoFoundEnumValue;
                            else parser.ignore();
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
                                while (parser.quote != 0)
                                {
                                    index = searcher.NextFlagEnum();
                                    if (parser.State != parseState.Success) return;
                                    if (index != -1) intValue |= enumInts.ULong[index];
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
                /// <param name="parser">Json解析器</param>
                /// <param name="value">目标数据</param>
                /// <param name="parameters"></param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Call(jsonParser parser, ref valueType value, ref object[] parameters)
#else
                /// <summary>
                /// 成员解析器
                /// </summary>
                /// <param name="parser">Json解析器</param>
                /// <param name="value">目标数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Call(jsonParser parser, ref valueType value)
#endif
                {
                    if ((parser.Config.MemberFilter & Filter) == Filter)
                    {
#if NOJIT
                        parser.ReflectionParameter = parameters ?? (parameters = new object[1]);
#endif
                        TryParse(parser, ref value);
                    }
                    else parser.ignore();
                }
#if NOJIT
                /// <summary>
                /// 成员解析器
                /// </summary>
                /// <param name="parser">Json解析器</param>
                /// <param name="memberMap">成员位图</param>
                /// <param name="value">目标数据</param>
                /// <param name="parameters"></param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Call(jsonParser parser, memberMap memberMap, ref valueType value, ref object[] parameters)
#else
                /// <summary>
                /// 成员解析器
                /// </summary>
                /// <param name="parser">Json解析器</param>
                /// <param name="memberMap">成员位图</param>
                /// <param name="value">目标数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Call(jsonParser parser, memberMap memberMap, ref valueType value)
#endif
                {
                    if ((parser.Config.MemberFilter & Filter) == Filter)
                    {
#if NOJIT
                        parser.ReflectionParameter = parameters ?? (parameters = new object[1]);
#endif
                        TryParse(parser, ref value);
                        memberMap.SetMember(MemberMapIndex);
                    }
                    else parser.ignore();
                }
            }
            /// <summary>
            /// 解析委托
            /// </summary>
            /// <param name="parser">Json解析器</param>
            /// <param name="value">目标数据</param>
            internal delegate void tryParse(jsonParser parser, ref valueType value);
            /// <summary>
            /// 未知名称处理委托
            /// </summary>
            /// <param name="value"></param>
            /// <param name="name"></param>
            /// <param name="node"></param>
            private delegate void unknownParse(ref valueType value, ref pointer.size name, ref jsonNode node);
            /// <summary>
            /// Json解析类型配置
            /// </summary>
            private static readonly jsonParse attribute;
            /// <summary>
            /// 解析委托
            /// </summary>
            internal static readonly tryParse DefaultParser;
            /// <summary>
            /// 未知名称处理委托
            /// </summary>
            private static readonly unknownParse onUnknownName;
            /// <summary>
            /// 是否值类型
            /// </summary>
            private static readonly bool isValueType;
            /// <summary>
            /// 是否匿名类型
            /// </summary>
            private static readonly bool isAnonymousType;
            /// <summary>
            /// 成员解析器集合
            /// </summary>
            private static readonly tryParseFilter[] memberParsers;
            /// <summary>
            /// 包装处理
            /// </summary>
            private static readonly tryParse unboxParser;
            /// <summary>
            /// 成员名称查找数据
            /// </summary>
            private static readonly pointer.reference memberSearcher;
            /// <summary>
            /// 默认顺序成员名称数据
            /// </summary>
            private static readonly pointer.reference memberNames;
            /// <summary>
            /// 引用类型对象解析
            /// </summary>
            /// <param name="parser">Json解析器</param>
            /// <param name="value">目标数据</param>
            internal static void Parse(jsonParser parser, ref valueType value)
            {
                if (DefaultParser == null)
                {
                    if (isValueType) ParseValue(parser, ref value);
                    else parseClass(parser, ref value);
                }
                else DefaultParser(parser, ref value);
            }
            /// <summary>
            /// 值类型对象解析
            /// </summary>
            /// <param name="parser">Json解析器</param>
            /// <param name="value">目标数据</param>
            internal static void ParseStruct(jsonParser parser, ref valueType value)
            {
                if (DefaultParser == null) ParseValue(parser, ref value);
                else DefaultParser(parser, ref value);
            }
            /// <summary>
            /// 值类型对象解析
            /// </summary>
            /// <param name="parser">Json解析器</param>
            /// <param name="value">目标数据</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal static void ParseValue(jsonParser parser, ref valueType value)
            {
                if (parser.searchObject()) ParseMembers(parser, ref value);
                else value = default(valueType);
            }
            /// <summary>
            /// 引用类型对象解析
            /// </summary>
            /// <param name="parser">Json解析器</param>
            /// <param name="value">目标数据</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal static void ParseClass(jsonParser parser, ref valueType value)
            {
                if (DefaultParser == null) parseClass(parser, ref value);
                else DefaultParser(parser, ref value);
            }
            /// <summary>
            /// 引用类型对象解析
            /// </summary>
            /// <param name="parser">Json解析器</param>
            /// <param name="value">目标数据</param>
            //[MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private static void parseClass(jsonParser parser, ref valueType value)
            {
                if (parser.searchObject())
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
                else
                {
                    if (value != null && isAnonymousType) parser.setAnonymousType(value);
                    value = default(valueType);
                }
            }
            /// <summary>
            /// 数据成员解析
            /// </summary>
            /// <param name="parser">Json解析器</param>
            /// <param name="value">目标数据</param>
            internal static void ParseMembers(jsonParser parser, ref valueType value)
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
                    while ((names = parser.isName(names, ref index)) != null)
                    {
                        if (index == -1) return;
#if NOJIT
                        memberParsers[index].Call(parser, ref value, ref parameters);
#else
                        memberParsers[index].Call(parser, ref value);
#endif
                        if (parser.State != parseState.Success) return;
                        ++index;
                    }
                    if (index == 0 ? parser.isFirstObject() : parser.isNextObject())
                    {
                        stateSearcher searcher = new stateSearcher(parser, memberSearcher);
                        bool isQuote;
                        if (onUnknownName == null)
                        {
                            do
                            {
                                if ((index = searcher.SearchName(out isQuote)) != -1)
                                {
                                    if (parser.searchColon() == 0) return;
#if NOJIT
                                    memberParsers[index].Call(parser, ref value, ref parameters);
#else
                                    memberParsers[index].Call(parser, ref value);
#endif

                                }
                                else
                                {
                                    if (parser.State != parseState.Success) return;
                                    if (isQuote) parser.searchStringEnd();
                                    else parser.searchNameEnd();
                                    if (parser.State != parseState.Success || parser.searchColon() == 0) return;
                                    parser.ignore();
                                }
                            }
                            while (parser.State == parseState.Success && parser.isNextObject());
                        }
                        else
                        {
                            pointer.size name;
                            do
                            {
                                name.data = parser.Current;
                                if ((index = searcher.SearchName(out isQuote)) != -1)
                                {
                                    if (parser.searchColon() == 0) return;
#if NOJIT
                                    memberParsers[index].Call(parser, ref value, ref parameters);
#else
                                    memberParsers[index].Call(parser, ref value);
#endif
                                }
                                else
                                {
                                    if (parser.State != parseState.Success) return;
                                    if (isQuote) parser.searchStringEnd();
                                    else parser.searchNameEnd();
                                    if (parser.State != parseState.Success) return;
                                    name.sizeValue = (int)((byte*)parser.Current - (byte*)name.data);
                                    if (parser.searchColon() == 0) return;
                                    jsonNode node = default(jsonNode);
                                    parser.parse(ref node);
                                    if (parser.State != parseState.Success) return;
                                    onUnknownName(ref value, ref name, ref node);
                                }
                            }
                            while (parser.isNextObject());
                        }
                    }
                }
                else if (memberMap.Type == memberMap<valueType>.TypeInfo)
                {
                    try
                    {
                        memberMap.Empty();
                        config.MemberMap = null;
                        while ((names = parser.isName(names, ref index)) != null)
                        {
                            if (index == -1) return;
#if NOJIT
                            memberParsers[index].Call(parser, memberMap, ref value, ref parameters);
#else
                            memberParsers[index].Call(parser, memberMap, ref value);
#endif
                            if (parser.State != parseState.Success) return;
                            ++index;
                        }
                        if (index == 0 ? parser.isFirstObject() : parser.isNextObject())
                        {
                            stateSearcher searcher = new stateSearcher(parser, memberSearcher);
                            bool isQuote;
                            if (onUnknownName == null)
                            {
                                do
                                {
                                    if ((index = searcher.SearchName(out isQuote)) != -1)
                                    {
                                        if (parser.searchColon() == 0) return;
#if NOJIT
                                        memberParsers[index].Call(parser, memberMap, ref value, ref parameters);
#else
                                        memberParsers[index].Call(parser, memberMap, ref value);
#endif
                                    }
                                    else
                                    {
                                        if (parser.State != parseState.Success) return;
                                        if (isQuote) parser.searchStringEnd();
                                        else parser.searchNameEnd();
                                        if (parser.State != parseState.Success || parser.searchColon() == 0) return;
                                        parser.ignore();
                                    }
                                }
                                while (parser.State == parseState.Success && parser.isNextObject());
                            }
                            else
                            {
                                pointer.size name;
                                do
                                {
                                    name.data = parser.Current;
                                    if ((index = searcher.SearchName(out isQuote)) != -1)
                                    {
                                        if (parser.searchColon() == 0) return;
#if NOJIT
                                        memberParsers[index].Call(parser, memberMap, ref value, ref parameters);
#else
                                        memberParsers[index].Call(parser, memberMap, ref value);
#endif
                                    }
                                    else
                                    {
                                        if (parser.State != parseState.Success) return;
                                        if (isQuote) parser.searchStringEnd();
                                        else parser.searchNameEnd();
                                        if (parser.State != parseState.Success) return;
                                        name.sizeValue = (int)((byte*)parser.Current - (byte*)name.data);
                                        if (parser.searchColon() == 0) return;
                                        jsonNode node = default(jsonNode);
                                        parser.parse(ref node);
                                        if (parser.State != parseState.Success) return;
                                        onUnknownName(ref value, ref name, ref node);
                                    }
                                }
                                while (parser.isNextObject());
                            }
                        }
                    }
                    finally { config.MemberMap = memberMap; }
                }
                else parser.State = parseState.MemberMap;
            }
            /// <summary>
            /// 不支持多维数组
            /// </summary>
            /// <param name="parser">Json解析器</param>
            /// <param name="value">目标数据</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private static void arrayManyRank(jsonParser parser, ref valueType value)
            {
                parser.State = parseState.ArrayManyRank;
            }
            /// <summary>
            /// 数组解析
            /// </summary>
            /// <param name="parser">Json解析器</param>
            /// <param name="values">目标数据</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal static void Array(jsonParser parser, ref valueType[] values)
            {
                int count = ArrayIndex(parser, ref values);
                if (count != -1 && count != values.Length) System.Array.Resize(ref values, count);
            }
            /// <summary>
            /// 数组解析
            /// </summary>
            /// <param name="parser">Json解析器</param>
            /// <param name="values">目标数据</param>
            /// <returns>数据数量,-1表示失败</returns>
            internal static int ArrayIndex(jsonParser parser, ref valueType[] values)
            {
                parser.searchArray(ref values);
                if (parser.State != parseState.Success || values == null) return -1;
                int index = 0;
                if (parser.isFirstArrayValue())
                {
                    do
                    {
                        if (index == values.Length)
                        {
                            valueType value = default(valueType);
                            Parse(parser, ref value);
                            if (parser.State != parseState.Success) return -1;
                            valueType[] newValues = new valueType[index == 0 ? sizeof(int) : (index << 1)];
                            values.CopyTo(newValues, 0);
                            newValues[index++] = value;
                            values = newValues;
                        }
                        else
                        {
                            Parse(parser, ref values[index]);
                            if (parser.State != parseState.Success) return -1;
                            ++index;
                        }
                    }
                    while (parser.isNextArrayValue());
                }
                return parser.State == parseState.Success ? index : -1;
            }
            /// <summary>
            /// 忽略数据
            /// </summary>
            /// <param name="parser">Json解析器</param>
            /// <param name="value">目标数据</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private static void ignore(jsonParser parser, ref valueType value)
            {
                parser.ignore();
            }
            ///// <summary>
            ///// 找不到构造函数
            ///// </summary>
            ///// <param name="parser">Json解析器</param>
            ///// <param name="value">目标数据</param>
            //[MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            //private static void noConstructor(jsonParser parser, ref valueType value)
            //{
            //    parser.checkNoConstructor(ref value);
            //}
            /// <summary>
            /// 获取字段成员集合
            /// </summary>
            /// <returns>字段成员集合</returns>
            public static subArray<memberIndex> GetMembers()
            {
                if (memberParsers == null) return default(subArray<memberIndex>);
                return staticTypeParser.GetMembers(memberIndexGroup<valueType>.GetFields(attribute.MemberFilter), memberIndexGroup<valueType>.GetProperties(attribute.MemberFilter), attribute);
            }
            /// <summary>
            /// 包装处理
            /// </summary>
            /// <param name="parser"></param>
            /// <param name="value"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private static void unbox(jsonParser parser, ref valueType value)
            {
#if NOJIT
                parser.ReflectionParameter = new object[1];
#endif
                unboxParser(parser, ref value);
            }
            static typeParser()
            {
                Type type = typeof(valueType);
                MethodInfo methodInfo = jsonParser.getParseMethod(type);
                if (methodInfo != null)
                {
#if NOJIT
                    DefaultParser = new methodParser(methodInfo).Parse;
#else
                    DynamicMethod dynamicMethod = new DynamicMethod("jsonParser", typeof(void), new Type[] { typeof(jsonParser), type.MakeByRefType() }, true);
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
                    if (type.GetArrayRank() == 1) DefaultParser = (tryParse)Delegate.CreateDelegate(typeof(tryParse), staticTypeParser.GetArrayParser(type.GetElementType()));
                    else DefaultParser = arrayManyRank;
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
                    DefaultParser = ignore;
                    return;
                }
                if (type.IsGenericType)
                {
                    Type genericType = type.GetGenericTypeDefinition();
                    if (genericType == typeof(Dictionary<,>))
                    {
                        DefaultParser = (tryParse)Delegate.CreateDelegate(typeof(tryParse), staticTypeParser.GetDictionaryParser(type));
                        return;
                    }
                    if (genericType == typeof(Nullable<>))
                    {
                        DefaultParser = (tryParse)Delegate.CreateDelegate(typeof(tryParse), staticTypeParser.GetNullableParser(type));
                        return;
                    }
                    if (genericType == typeof(KeyValuePair<,>))
                    {
                        DefaultParser = (tryParse)Delegate.CreateDelegate(typeof(tryParse), staticTypeParser.GetKeyValuePairParser(type));
                        isValueType = true;
                        return;
                    }
                }
                if ((methodInfo = staticTypeParser.GetCustomParser(type)) != null)
                {
                    DefaultParser = (tryParse)Delegate.CreateDelegate(typeof(tryParse), methodInfo);
                }
                else
                {
                    Type attributeType;
                    attribute = type.customAttribute<jsonParse>(out attributeType, true) ?? jsonParse.AllMember;
                    if ((methodInfo = staticTypeParser.GetIEnumerableConstructorParser(type)) != null)
                    {
                        DefaultParser = (tryParse)Delegate.CreateDelegate(typeof(tryParse), methodInfo);
                    }
                    else
                    {
                        if (type.IsValueType) isValueType = true;
                        else if (attribute != jsonParse.AllMember && attributeType != type)
                        {
                            for (Type baseType = type.BaseType; baseType != typeof(object); baseType = baseType.BaseType)
                            {
                                jsonParse baseAttribute = fastCSharp.code.typeAttribute.GetAttribute<jsonParse>(baseType, false, true);
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
                        fieldIndex defaultMember = null;
                        subArray<fieldIndex> fields = staticTypeParser.GetFields(memberIndexGroup<valueType>.GetFields(attribute.MemberFilter), attribute, ref defaultMember);
                        subArray<keyValue<propertyIndex, MethodInfo>> properties = staticTypeParser.GetProperties(memberIndexGroup<valueType>.GetProperties(attribute.MemberFilter), attribute);
                        bool isBox = false;
                        if (type.IsValueType && fields.length + properties.length == 1)
                        {
                            boxSerialize boxSerialize = fastCSharp.code.typeAttribute.GetAttribute<boxSerialize>(type, true, true);
                            if (boxSerialize != null && boxSerialize.IsJson)
                            {
                                isBox = true;
                                defaultMember = null;
                            }
                        }
                        tryParseFilter[] parsers = new tryParseFilter[fields.length + properties.length + (defaultMember == null ? 0 : 1)];
                        //memberMap.type memberMapType = memberMap<valueType>.TypeInfo;
                        string[] names = isBox ? null : new string[parsers.Length];
                        int index = 0, nameLength = 0, maxNameLength = 0;
                        foreach (fieldIndex member in fields)
                        {
                            tryParseFilter tryParse = parsers[index] = new tryParseFilter
                            {
#if NOJIT
                                TryParse = new fieldParser(member.Member).Parser(),
#else
                                TryParse = (tryParse)staticTypeParser.CreateDynamicMethod(type, member.Member).CreateDelegate(typeof(tryParse)),
#endif
                                MemberMapIndex = member.MemberIndex,
                                Filter = member.Member.IsPublic ? code.memberFilters.PublicInstanceField : code.memberFilters.NonPublicInstanceField
                            };
                            if (!isBox)
                            {
                                string name = member.AnonymousName;
                                if (name.Length > maxNameLength) maxNameLength = name.Length;
                                nameLength += (names[index++] = name).Length;
                                if (member == defaultMember)
                                {
                                    parsers[parsers.Length - 1] = tryParse;
                                    names[parsers.Length - 1] = string.Empty;
                                }
                            }
                        }
                        foreach (keyValue<propertyIndex, MethodInfo> member in properties)
                        {
                            parsers[index] = new tryParseFilter
                            {
#if NOJIT
                                TryParse = new propertyParser(member.Key.Member).Parser(),
#else
                                TryParse = (tryParse)staticTypeParser.CreateDynamicMethod(type, member.Key.Member, member.Value).CreateDelegate(typeof(tryParse)),
#endif
                                MemberMapIndex = member.Key.MemberIndex,
                                Filter = member.Value.IsPublic ? code.memberFilters.PublicInstanceProperty : code.memberFilters.NonPublicInstanceProperty
                            };
                            if (!isBox)
                            {
                                if (member.Key.Member.Name.Length > maxNameLength) maxNameLength = member.Key.Member.Name.Length;
                                nameLength += (names[index++] = member.Key.Member.Name).Length;
                            }
                        }
                        if (isBox)
                        {
                            unboxParser = parsers[0].TryParse;
                            DefaultParser = unbox;
                        }
                        else
                        {
                            if (type.Name[0] == '<') isAnonymousType = true;
                            if (maxNameLength > (short.MaxValue >> 1) - 4 || nameLength == 0) memberNames = unmanaged.NullByte8;
                            else
                            {
                                memberNames = unmanaged.GetStatic((nameLength + (names.Length - (defaultMember == null ? 0 : 1)) * 5 + 1) << 1, false).Reference;
                                byte* write = memberNames.Byte;
                                foreach (string name in names)
                                {
                                    if (name.Length != 0)
                                    {
                                        if (write == memberNames.Byte)
                                        {
                                            *(short*)write = (short)((name.Length + 3) * sizeof(char));
                                            *(char*)(write + sizeof(short)) = '"';
                                            write += sizeof(short) + sizeof(char);
                                        }
                                        else
                                        {
                                            *(short*)write = (short)((name.Length + 4) * sizeof(char));
                                            *(int*)(write + sizeof(short)) = ',' + ('"' << 16);
                                            write += sizeof(short) + sizeof(int);
                                        }
                                        fixed (char* nameFixed = name) fastCSharp.unsafer.memory.SimpleCopy(nameFixed, (char*)write, name.Length);
                                        *(int*)(write += name.Length << 1) = '"' + (':' << 16);
                                        write += sizeof(int);
                                    }
                                }
                                *(short*)write = 0;
                            }
                            memberSearcher = stateSearcher.GetMemberSearcher(type, names);
                            memberParsers = parsers;

                            Type refType = type.MakeByRefType();
                            foreach (fastCSharp.code.attributeMethod attributeMethod in fastCSharp.code.attributeMethod.GetStatic(type))
                            {
                                if ((methodInfo = attributeMethod.Method).ReturnType == typeof(void))
                                {
                                    ParameterInfo[] parameters = methodInfo.GetParameters();
                                    if (parameters.Length == 3 && parameters[0].ParameterType == refType && parameters[1].ParameterType == staticTypeParser.PointerSizeRefType && parameters[2].ParameterType == staticTypeParser.JsonNodeRefType)
                                    {
                                        if (attributeMethod.GetAttribute<jsonParse.unknownName>(true) != null)
                                        {
                                            onUnknownName = (unknownParse)Delegate.CreateDelegate(typeof(unknownParse), methodInfo);
                                            break;
                                        }
                                    }
                                }
                            }
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
                public void Parse(jsonParser parser, ref valueType value)
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
                    method = new pub.methodParameter1(staticTypeParser.GetMemberMethodInfo(field.FieldType));
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
                private void parse(jsonParser parser, ref valueType value)
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
                private void parseValue(jsonParser parser, ref valueType value)
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
                    method = new pub.methodParameter1(staticTypeParser.GetMemberMethodInfo(property.PropertyType));
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
                private void parse(jsonParser parser, ref valueType value)
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
                private void parseValue(jsonParser parser, ref valueType value)
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
        /// 配置参数
        /// </summary>
        internal config Config;
        /// <summary>
        /// 匿名类型数据
        /// </summary>
        private subArray<keyValue<Type, object>> anonymousTypes;
        /// <summary>
        /// Json字符串
        /// </summary>
        private string json;
        /// <summary>
        /// 二进制缓冲区
        /// </summary>
        internal byte[] Buffer { get; private set; }
        /// <summary>
        /// 字符状态位查询表格
        /// </summary>
        private readonly byte* bits = Bits.Byte;
        /// <summary>
        /// 转义字符集合
        /// </summary>
        private readonly char* escapeChars = escapeCharData.Char;
        /// <summary>
        /// Json字符串起始位置
        /// </summary>
        private char* jsonFixed;
        /// <summary>
        /// 当前解析位置
        /// </summary>
        internal char* Current;
        /// <summary>
        /// 当前解析字符
        /// </summary>
        public char CurrentChar
        {
            get { return *Current; }
        }
        /// <summary>
        /// 解析结束位置
        /// </summary>
        private char* end;
        /// <summary>
        /// 解析结束位置
        /// </summary>
        internal char* End
        {
            get { return end; }
        }
        /// <summary>
        /// 最后一个字符
        /// </summary>
        private char endChar;
        /// <summary>
        /// 当前字符串引号
        /// </summary>
        private char quote;
        /// <summary>
        /// 解析状态
        /// </summary>
        public parseState State { get; private set; }
        /// <summary>
        /// 是否以空格字符结束
        /// </summary>
        private bool isEndSpace;
        /// <summary>
        /// 是否以10进制数字字符结束
        /// </summary>
        private bool isEndDigital;
        /// <summary>
        /// 是否以16进制数字字符结束
        /// </summary>
        private bool isEndHex;
        /// <summary>
        /// 是否以数字字符结束
        /// </summary>
        private bool isEndNumber;
#if NOJIT
        /// <summary>
        /// 反射模式参数
        /// </summary>
        private object[] ReflectionParameter;
#endif
        /// <summary>
        /// Json解析器
        /// </summary>
        private jsonParser() { }
        /// <summary>
        /// Json解析
        /// </summary>
        /// <typeparam name="valueType">目标类型</typeparam>
        /// <param name="json">Json字符串</param>
        /// <param name="value">目标数据</param>
        /// <param name="config">配置参数</param>
        /// <returns>解析状态</returns>
        private parseState parse<valueType>(ref subString json, ref valueType value, config config)
        {
            fixed (char* jsonFixed = (this.json = json.value))
            {
                Current = (this.jsonFixed = jsonFixed) + json.StartIndex;
                this.Config = config;
                endChar = *((end = Current + json.Length) - 1);
                parseState state = parse(ref value);
                if (state != parseState.Success && config.IsOutputError)
                {
                    fastCSharp.log.Default.Add(state.ToString() + "[" + ((int)(Current - jsonFixed) - json.StartIndex).toString() + @"]
" + json, new System.Diagnostics.StackFrame(), false);
                }
                return state;
            }
        }
        /// <summary>
        /// Json解析
        /// </summary>
        /// <typeparam name="valueType">目标类型</typeparam>
        /// <param name="json">Json字符串</param>
        /// <param name="length">Json长度</param>
        /// <param name="value">目标数据</param>
        /// <param name="config">配置参数</param>
        /// <param name="buffer">二进制缓冲区</param>
        /// <returns>解析状态</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private parseState parse<valueType>(char* json, int length, ref valueType value, config config, byte[] buffer)
        {
            Config = config;
            Buffer = buffer;
            endChar = *((end = (jsonFixed = Current = json) + length) - 1);
            parseState state = parse(ref value);
            if (state != parseState.Success && config.IsOutputError)
            {
                fastCSharp.log.Default.Add(state.ToString() + "[" + ((int)(Current - jsonFixed)).toString() + @"]
" + new string(json, 0, length), new System.Diagnostics.StackFrame(), false);
            }
            return state;
        }
        /// <summary>
        /// Json解析
        /// </summary>
        /// <typeparam name="valueType">目标类型</typeparam>
        /// <param name="value">目标数据</param>
        /// <returns>解析状态</returns>
        private parseState parse<valueType>(ref valueType value)
        {
            if ((endChar & 0xff00) == 0)
            {
                isEndSpace = (bits[(byte)endChar] & spaceBit) == 0;
                isEndDigital = (uint)(endChar - '0') < 10;
                isEndHex = isEndDigital || (uint)((endChar | 0x20) - 'a') < 6;
                isEndNumber = isEndHex || (bits[(byte)endChar] & numberBit) == 0;
            }
            else isEndSpace = isEndDigital = isEndHex = isEndNumber = false;
            State = parseState.Success;
            typeParser<valueType>.Parse(this, ref value);
            if (State == parseState.Success)
            {
                if (Current == end || !Config.IsEndSpace) return Config.State = parseState.Success;
                space();
                if (State == parseState.Success)
                {
                    if (Current == end) return Config.State = parseState.Success;
                    State = parseState.CrashEnd;
                }
            }
            return State;
        }
        /// <summary>
        /// 释放Json解析器
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void free()
        {
            json = null;
            Config = null;
            Buffer = null;
            anonymousTypes.Null();
#if NOJIT
            ReflectionParameter = null;
#endif
            typePool<jsonParser>.PushNotNull(this);
        }
        /// <summary>
        /// 设置错误解析状态
        /// </summary>
        /// <param name="state"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Error(parseState state)
        {
            this.State = state;
        }
        /// <summary>
        /// 扫描空格字符
        /// </summary>
        private void space()
        {
        SPACE:
            if (isEndSpace)
            {
                do
                {
                    if (Current == end) return;
                    if (((bits[*(byte*)Current] & spaceBit) | *(((byte*)Current) + 1)) != 0)
                    {
                        if (*Current == '/') break;
                        return;
                    }
                    ++Current;
                }
                while (true);
            }
            else
            {
                while (((bits[*(byte*)Current] & spaceBit) | *(((byte*)Current) + 1)) == 0) ++Current;
                if (*Current != '/' || Current == end) return;
            }
            if (++Current == end)
            {
                State = parseState.UnknownNote;
                return;
            }
            if (*Current == '/')
            {
                if (endChar == '\n')
                {
                    while (*++Current != '\n') ;
                    ++Current;
                }
                else
                {
                    do
                    {
                        if (++Current == end) return;
                    }
                    while (*Current != '\n');
                }
                goto SPACE;
            }
            if (*Current == '*')
            {
                if (++Current == end)
                {
                    State = parseState.NoteNotRound;
                    return;
                }
                if (endChar == '/')
                {
                    do
                    {
                        if (++Current == end)
                        {
                            State = parseState.NoteNotRound;
                            return;
                        }
                        while (*Current != '/') ++Current;
                        if (*(Current - 1) == '*')
                        {
                            ++Current;
                            goto SPACE;
                        }
                        if (++Current == end)
                        {
                            State = parseState.NoteNotRound;
                            return;
                        }
                    }
                    while (true);
                }
                do
                {
                    while (*Current != '*')
                    {
                        if (++Current == end)
                        {
                            State = parseState.NoteNotRound;
                            return;
                        }
                    }
                    if (++Current == end)
                    {
                        State = parseState.NoteNotRound;
                        return;
                    }
                    if (*Current == '/')
                    {
                        if (++Current == end) return;
                        goto SPACE;
                    }
                }
                while (true);
            }
            State = parseState.UnknownNote;
        }
        /// <summary>
        /// 是否null
        /// </summary>
        /// <returns>是否null</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private bool isNull()
        {
            if (*Current == 'n')
            {
                if (((*(Current + 1) ^ 'u') | (*(int*)(Current + 2) ^ ('l' + ('l' << 16)))) == 0 && (int)((byte*)end - (byte*)Current) >= 4 * sizeof(char))
                {
                    Current += 4;
                    return true;
                }
                State = parseState.NotNull;
            }
            return false;
        }
        /// <summary>
        /// 是否非数字NaN
        /// </summary>
        /// <returns>是否非数字NaN</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private bool isNaN()
        {
            if (*Current == 'N')
            {
                if ((*(int*)(Current + 1) ^ ('a' + ('N' << 16))) == 0 && (int)((byte*)end - (byte*)Current) >= 3 * sizeof(char))
                {
                    Current += 3;
                    return true;
                }
                State = parseState.NotNumber;
            }
            return false;
        }
        /// <summary>
        /// 解析10进制数字
        /// </summary>
        /// <param name="value">第一位数字</param>
        /// <returns>数字</returns>
        private uint parseUInt32(uint value)
        {
            uint number;
            if (isEndDigital)
            {
                do
                {
                    if ((number = (uint)(*Current - '0')) > 9) return value;
                    value *= 10;
                    value += number;
                    if (++Current == end) return value;
                }
                while (true);
            }
            while ((number = (uint)(*Current - '0')) < 10)
            {
                value *= 10;
                ++Current;
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
            uint number = (uint)(*Current - '0');
            if (number > 9)
            {
                if ((number = (number - ('A' - '0')) & 0xffdfU) > 5)
                {
                    State = parseState.NotHex;
                    return;
                }
                number += 10;
            }
            value = number;
            if (++Current == end) return;
            if (isEndHex)
            {
                do
                {
                    if ((number = (uint)(*Current - '0')) > 9)
                    {
                        if ((number = (number - ('A' - '0')) & 0xffdfU) > 5) return;
                        number += 10;
                    }
                    value <<= 4;
                    value += number;
                }
                while (++Current != end);
                return;
            }
            do
            {
                if ((number = (uint)(*Current - '0')) > 9)
                {
                    if ((number = (number - ('A' - '0')) & 0xffdfU) > 5) return;
                    number += 10;
                }
                value <<= 4;
                ++Current;
                value += number;
            }
            while (true);
        }
        /// <summary>
        /// 逻辑值解析
        /// </summary>
        /// <param name="value">数据</param>
        /// <returns>解析状态</returns>
        [parseMethod]
        internal void Parse(ref bool value)
        {
            byte isSpace = 0;
        START:
            switch (*Current & 7)
            {
                case 'f' & 7:
                    if (*(long*)(Current + 1) == 'a' + ('l' << 16) + ((long)'s' << 32) + ((long)'e' << 48)
                        && (int)((byte*)end - (byte*)Current) >= 5 * sizeof(char))
                    {
                        value = false;
                        Current += 5;
                        return;
                    }
                    break;
                case 't' & 7:
                    if (*(long*)(Current) == 't' + ('r' << 16) + ((long)'u' << 32) + ((long)'e' << 48)
                        && (int)((byte*)end - (byte*)Current) >= 4 * sizeof(char))
                    {
                        value = true;
                        Current += 4;
                        return;
                    }
                    break;
                case '0' & 7:
                    if (*Current == '0')
                    {
                        value = false;
                        ++Current;
                        return;
                    }
                    break;
                case '1' & 7:
                    if (*Current == '1')
                    {
                        value = true;
                        ++Current;
                        return;
                    }
                    break;
                case '"' & 7:
                case '\'' & 7:
                    if (*Current == '"' || *Current == '\'')
                    {
                        quote = *Current;
                        if (++Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        if ((uint)(*Current - '0') < 2) value = *(byte*)Current++ != '0';
                        else if (*Current == 'f')
                        {
                            if (*(long*)(Current + 1) == 'a' + ('l' << 16) + ((long)'s' << 32) + ((long)'e' << 48) && (int)((byte*)end - (byte*)Current) >= 5 * sizeof(char))
                            {
                                value = false;
                                Current += 5;
                            }
                            else
                            {
                                State = parseState.NotBool;
                                return;
                            }
                        }
                        else if (*(long*)(Current) == 't' + ('r' << 16) + ((long)'u' << 32) + ((long)'e' << 48) && (int)((byte*)end - (byte*)Current) >= 4 * sizeof(char))
                        {
                            value = true;
                            Current += 4;
                        }
                        else
                        {
                            State = parseState.NotBool;
                            return;
                        }
                        if (Current == end) State = parseState.CrashEnd;
                        else if (*Current == quote) ++Current;
                        else State = parseState.NotBool;
                        return;
                    }
                    break;
                default:
                    if (isSpace == 0)
                    {
                        space();
                        if (State != parseState.Success) return;
                        if (Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        isSpace = 1;
                        goto START;
                    }
                    break;
            }
            State = parseState.NotBool;
        }
        /// <summary>
        /// 逻辑值解析
        /// </summary>
        /// <param name="value">数据</param>
        /// <returns>解析状态</returns>
        [parseMethod]
        private void parse(ref bool? value)
        {
            byte isSpace = 0;
        START:
            switch (*Current & 7)
            {
                case 'f' & 7:
                    if (*Current == 'f')
                    {
                        if (*(long*)(Current + 1) == 'a' + ('l' << 16) + ((long)'s' << 32) + ((long)'e' << 48)
                            && (int)((byte*)end - (byte*)Current) >= 5 * sizeof(char))
                        {
                            value = false;
                            Current += 5;
                            return;
                        }
                    }
                    else if (*(long*)(Current) == 'n' + ('u' << 16) + ((long)'l' << 32) + ((long)'l' << 48)
                        && (int)((byte*)end - (byte*)Current) >= 4 * sizeof(char))
                    {
                        value = null;
                        Current += 4;
                        return;
                    }
                    break;
                case 't' & 7:
                    if (*(long*)(Current) == 't' + ('r' << 16) + ((long)'u' << 32) + ((long)'e' << 48)
                        && (int)((byte*)end - (byte*)Current) >= 4 * sizeof(char))
                    {
                        value = true;
                        Current += 4;
                        return;
                    }
                    break;
                case '0' & 7:
                    if (*Current == '0')
                    {
                        value = false;
                        ++Current;
                        return;
                    }
                    break;
                case '1' & 7:
                    if (*Current == '1')
                    {
                        value = true;
                        ++Current;
                        return;
                    }
                    break;
                case '"' & 7:
                case '\'' & 7:
                    if (*Current == '"' || *Current == '\'')
                    {
                        quote = *Current;
                        if (++Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        if ((uint)(*Current - '0') < 2) value = *(byte*)Current++ != '0';
                        else if (*Current == 'f')
                        {
                            if (*(long*)(Current + 1) == 'a' + ('l' << 16) + ((long)'s' << 32) + ((long)'e' << 48) && (int)((byte*)end - (byte*)Current) >= 5 * sizeof(char))
                            {
                                value = false;
                                Current += 5;
                            }
                            else
                            {
                                State = parseState.NotBool;
                                return;
                            }
                        }
                        else if (*(long*)(Current) == 't' + ('r' << 16) + ((long)'u' << 32) + ((long)'e' << 48) && (int)((byte*)end - (byte*)Current) >= 4 * sizeof(char))
                        {
                            value = true;
                            Current += 4;
                        }
                        else
                        {
                            State = parseState.NotBool;
                            return;
                        }
                        if (Current == end) State = parseState.CrashEnd;
                        else if (*Current == quote) ++Current;
                        else State = parseState.NotBool;
                        return;
                    }
                    break;
                default:
                    if (isSpace == 0)
                    {
                        space();
                        if (State != parseState.Success) return;
                        if (Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        isSpace = 1;
                        goto START;
                    }
                    break;
            }
            State = parseState.NotBool;
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        internal void Parse(ref byte value)
        {
            uint number = (uint)(*Current - '0');
            if (number > 9)
            {
                space();
                if (State != parseState.Success) return;
                if (Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                if ((number = (uint)(*Current - '0')) > 9)
                {
                    if (*Current == '"' || *Current == '\'')
                    {
                        quote = *Current;
                        if (++Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        if ((number = (uint)(*Current - '0')) > 9)
                        {
                            State = parseState.NotNumber;
                            return;
                        }
                        if (++Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        if (number == 0)
                        {
                            if (*Current == quote)
                            {
                                value = 0;
                                ++Current;
                                return;
                            }
                            if (*Current != 'x')
                            {
                                State = parseState.NotNumber;
                                return;
                            }
                            if (++Current == end)
                            {
                                State = parseState.CrashEnd;
                                return;
                            }
                            parseHex32(ref number);
                            value = (byte)number;
                        }
                        else value = (byte)parseUInt32(number);
                        if (State == parseState.Success)
                        {
                            if (Current == end) State = parseState.CrashEnd;
                            else if (*Current == quote) ++Current;
                            else State = parseState.NotNumber;
                        }
                        return;
                    }
                    State = parseState.NotNumber;
                    return;
                }
            }
            if (++Current == end)
            {
                value = (byte)number;
                return;
            }
            if (number == 0)
            {
                if (*Current != 'x')
                {
                    value = 0;
                    return;
                }
                if (++Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                parseHex32(ref number);
                value = (byte)number;
                return;
            }
            value = (byte)parseUInt32(number);
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref byte? value)
        {
            uint number = (uint)(*Current - '0');
            if (number > 9)
            {
                if (isNull())
                {
                    value = null;
                    return;
                }
                space();
                if (State != parseState.Success) return;
                if (Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                if ((number = (uint)(*Current - '0')) > 9)
                {
                    if (isNull())
                    {
                        value = null;
                        return;
                    }
                    if (*Current == '"' || *Current == '\'')
                    {
                        quote = *Current;
                        if (++Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        if ((number = (uint)(*Current - '0')) > 9)
                        {
                            State = parseState.NotNumber;
                            return;
                        }
                        if (++Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        if (number == 0)
                        {
                            if (*Current == quote)
                            {
                                value = 0;
                                ++Current;
                                return;
                            }
                            if (*Current != 'x')
                            {
                                State = parseState.NotNumber;
                                return;
                            }
                            if (++Current == end)
                            {
                                State = parseState.CrashEnd;
                                return;
                            }
                            parseHex32(ref number);
                            value = (byte)number;
                        }
                        else value = (byte)parseUInt32(number);
                        if (State == parseState.Success)
                        {
                            if (Current == end) State = parseState.CrashEnd;
                            else if (*Current == quote) ++Current;
                            else State = parseState.NotNumber;
                        }
                        return;
                    }
                    State = parseState.NotNumber;
                    return;
                }
            }
            if (++Current == end)
            {
                value = (byte)number;
                return;
            }
            if (number == 0)
            {
                if (*Current != 'x')
                {
                    value = 0;
                    return;
                }
                if (++Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                parseHex32(ref number);
                value = (byte)number;
                return;
            }
            value = (byte)parseUInt32(number);
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        internal void Parse(ref sbyte value)
        {
            int sign = 0;
            uint number = (uint)(*Current - '0');
            if (number > 9)
            {
                if (*Current == '-')
                {
                    if (++Current == end)
                    {
                        State = parseState.CrashEnd;
                        return;
                    }
                    if ((number = (uint)(*Current - '0')) > 9)
                    {
                        State = parseState.NotNumber;
                        return;
                    }
                    sign = 1;
                }
                else
                {
                    space();
                    if (State != parseState.Success) return;
                    if (Current == end)
                    {
                        State = parseState.CrashEnd;
                        return;
                    }
                    if ((number = (uint)(*Current - '0')) > 9)
                    {
                        if (*Current == '"' || *Current == '\'')
                        {
                            quote = *Current;
                            if (++Current == end)
                            {
                                State = parseState.CrashEnd;
                                return;
                            }
                            if ((number = (uint)(*Current - '0')) > 9)
                            {
                                if (*Current != '-')
                                {
                                    State = parseState.NotNumber;
                                    return;
                                }
                                if (++Current == end)
                                {
                                    State = parseState.CrashEnd;
                                    return;
                                }
                                if ((number = (uint)(*Current - '0')) > 9)
                                {
                                    State = parseState.NotNumber;
                                    return;
                                }
                                sign = 1;
                            }
                            if (++Current == end)
                            {
                                State = parseState.CrashEnd;
                                return;
                            }
                            if (number == 0)
                            {
                                if (*Current == quote)
                                {
                                    value = 0;
                                    ++Current;
                                    return;
                                }
                                if (*Current != 'x')
                                {
                                    State = parseState.NotNumber;
                                    return;
                                }
                                if (++Current == end)
                                {
                                    State = parseState.CrashEnd;
                                    return;
                                }
                                parseHex32(ref number);
                                value = sign == 0 ? (sbyte)(byte)number : (sbyte)-(int)number;
                            }
                            else value = sign == 0 ? (sbyte)(byte)parseUInt32(number) : (sbyte)-(int)parseUInt32(number);
                            if (State == parseState.Success)
                            {
                                if (Current == end) State = parseState.CrashEnd;
                                else if (*Current == quote) ++Current;
                                else State = parseState.NotNumber;
                            }
                            return;
                        }
                        if (*Current != '-')
                        {
                            State = parseState.NotNumber;
                            return;
                        }
                        if (++Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        if ((number = (uint)(*Current - '0')) > 9)
                        {
                            State = parseState.NotNumber;
                            return;
                        }
                        sign = 1;
                    }
                }
            }
            if (++Current == end)
            {
                value = sign == 0 ? (sbyte)(byte)number : (sbyte)-(int)number;
                return;
            }
            if (number == 0)
            {
                if (*Current != 'x')
                {
                    value = 0;
                    return;
                }
                if (++Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                parseHex32(ref number);
                value = sign == 0 ? (sbyte)(byte)number : (sbyte)-(int)number;
                return;
            }
            value = sign == 0 ? (sbyte)(byte)parseUInt32(number) : (sbyte)-(int)parseUInt32(number);
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref sbyte? value)
        {
            int sign = 0;
            uint number = (uint)(*Current - '0');
            if (number > 9)
            {
                if (isNull())
                {
                    value = null;
                    return;
                }
                if (*Current == '-')
                {
                    if (++Current == end)
                    {
                        State = parseState.CrashEnd;
                        return;
                    }
                    if ((number = (uint)(*Current - '0')) > 9)
                    {
                        State = parseState.NotNumber;
                        return;
                    }
                    sign = 1;
                }
                else
                {
                    space();
                    if (State != parseState.Success) return;
                    if (Current == end)
                    {
                        State = parseState.CrashEnd;
                        return;
                    }
                    if ((number = (uint)(*Current - '0')) > 9)
                    {
                        if (isNull())
                        {
                            value = null;
                            return;
                        }
                        if (*Current == '"' || *Current == '\'')
                        {
                            quote = *Current;
                            if (++Current == end)
                            {
                                State = parseState.CrashEnd;
                                return;
                            }
                            if ((number = (uint)(*Current - '0')) > 9)
                            {
                                if (*Current != '-')
                                {
                                    State = parseState.NotNumber;
                                    return;
                                }
                                if (++Current == end)
                                {
                                    State = parseState.CrashEnd;
                                    return;
                                }
                                if ((number = (uint)(*Current - '0')) > 9)
                                {
                                    State = parseState.NotNumber;
                                    return;
                                }
                                sign = 1;
                            }
                            if (++Current == end)
                            {
                                State = parseState.CrashEnd;
                                return;
                            }
                            if (number == 0)
                            {
                                if (*Current == quote)
                                {
                                    value = 0;
                                    ++Current;
                                    return;
                                }
                                if (*Current != 'x')
                                {
                                    State = parseState.NotNumber;
                                    return;
                                }
                                if (++Current == end)
                                {
                                    State = parseState.CrashEnd;
                                    return;
                                }
                                parseHex32(ref number);
                                value = sign == 0 ? (sbyte)(byte)number : (sbyte)-(int)number;
                            }
                            else value = sign == 0 ? (sbyte)(byte)parseUInt32(number) : (sbyte)-(int)parseUInt32(number);
                            if (State == parseState.Success)
                            {
                                if (Current == end) State = parseState.CrashEnd;
                                else if (*Current == quote) ++Current;
                                else State = parseState.NotNumber;
                            }
                            return;
                        }
                        if (*Current != '-')
                        {
                            State = parseState.NotNumber;
                            return;
                        }
                        if (++Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        if ((number = (uint)(*Current - '0')) > 9)
                        {
                            State = parseState.NotNumber;
                            return;
                        }
                        sign = 1;
                    }
                }
            }
            if (++Current == end)
            {
                value = sign == 0 ? (sbyte)(byte)number : (sbyte)-(int)number;
                return;
            }
            if (number == 0)
            {
                if (*Current != 'x')
                {
                    value = 0;
                    return;
                }
                if (++Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                parseHex32(ref number);
                value = sign == 0 ? (sbyte)(byte)number : (sbyte)-(int)number;
                return;
            }
            value = sign == 0 ? (sbyte)(byte)parseUInt32(number) : (sbyte)-(int)parseUInt32(number);
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        internal void Parse(ref ushort value)
        {
            uint number = (uint)(*Current - '0');
            if (number > 9)
            {
                space();
                if (State != parseState.Success) return;
                if (Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                if ((number = (uint)(*Current - '0')) > 9)
                {
                    if (*Current == '"' || *Current == '\'')
                    {
                        quote = *Current;
                        if (++Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        if ((number = (uint)(*Current - '0')) > 9)
                        {
                            State = parseState.NotNumber;
                            return;
                        }
                        if (++Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        if (number == 0)
                        {
                            if (*Current == quote)
                            {
                                value = 0;
                                ++Current;
                                return;
                            }
                            if (*Current != 'x')
                            {
                                State = parseState.NotNumber;
                                return;
                            }
                            if (++Current == end)
                            {
                                State = parseState.CrashEnd;
                                return;
                            }
                            parseHex32(ref number);
                            value = (ushort)number;
                        }
                        else value = (ushort)parseUInt32(number);
                        if (State == parseState.Success)
                        {
                            if (Current == end) State = parseState.CrashEnd;
                            else if (*Current == quote) ++Current;
                            else State = parseState.NotNumber;
                        }
                        return;
                    }
                    State = parseState.NotNumber;
                    return;
                }
            }
            if (++Current == end)
            {
                value = (ushort)number;
                return;
            }
            if (number == 0)
            {
                if (*Current != 'x')
                {
                    value = 0;
                    return;
                }
                if (++Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                parseHex32(ref number);
                value = (ushort)number;
                return;
            }
            value = (ushort)parseUInt32(number);
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref ushort? value)
        {
            uint number = (uint)(*Current - '0');
            if (number > 9)
            {
                if (isNull())
                {
                    value = null;
                    return;
                }
                space();
                if (State != parseState.Success) return;
                if (Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                if ((number = (uint)(*Current - '0')) > 9)
                {
                    if (isNull())
                    {
                        value = null;
                        return;
                    }
                    if (*Current == '"' || *Current == '\'')
                    {
                        quote = *Current;
                        if (++Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        if ((number = (uint)(*Current - '0')) > 9)
                        {
                            State = parseState.NotNumber;
                            return;
                        }
                        if (++Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        if (number == 0)
                        {
                            if (*Current == quote)
                            {
                                value = 0;
                                ++Current;
                                return;
                            }
                            if (*Current != 'x')
                            {
                                State = parseState.NotNumber;
                                return;
                            }
                            if (++Current == end)
                            {
                                State = parseState.CrashEnd;
                                return;
                            }
                            parseHex32(ref number);
                            value = (ushort)number;
                        }
                        else value = (ushort)parseUInt32(number);
                        if (State == parseState.Success)
                        {
                            if (Current == end) State = parseState.CrashEnd;
                            else if (*Current == quote) ++Current;
                            else State = parseState.NotNumber;
                        }
                        return;
                    }
                    State = parseState.NotNumber;
                    return;
                }
            }
            if (++Current == end)
            {
                value = (ushort)number;
                return;
            }
            if (number == 0)
            {
                if (*Current != 'x')
                {
                    value = 0;
                    return;
                }
                if (++Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                parseHex32(ref number);
                value = (ushort)number;
                return;
            }
            value = (ushort)parseUInt32(number);
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        internal void Parse(ref short value)
        {
            int sign = 0;
            uint number = (uint)(*Current - '0');
            if (number > 9)
            {
                if (*Current == '-')
                {
                    if (++Current == end)
                    {
                        State = parseState.CrashEnd;
                        return;
                    }
                    if ((number = (uint)(*Current - '0')) > 9)
                    {
                        State = parseState.NotNumber;
                        return;
                    }
                    sign = 1;
                }
                else
                {
                    space();
                    if (State != parseState.Success) return;
                    if (Current == end)
                    {
                        State = parseState.CrashEnd;
                        return;
                    }
                    if ((number = (uint)(*Current - '0')) > 9)
                    {
                        if (*Current == '"' || *Current == '\'')
                        {
                            quote = *Current;
                            if (++Current == end)
                            {
                                State = parseState.CrashEnd;
                                return;
                            }
                            if ((number = (uint)(*Current - '0')) > 9)
                            {
                                if (*Current != '-')
                                {
                                    State = parseState.NotNumber;
                                    return;
                                }
                                if (++Current == end)
                                {
                                    State = parseState.CrashEnd;
                                    return;
                                }
                                if ((number = (uint)(*Current - '0')) > 9)
                                {
                                    State = parseState.NotNumber;
                                    return;
                                }
                                sign = 1;
                            }
                            if (++Current == end)
                            {
                                State = parseState.CrashEnd;
                                return;
                            }
                            if (number == 0)
                            {
                                if (*Current == quote)
                                {
                                    value = 0;
                                    ++Current;
                                    return;
                                }
                                if (*Current != 'x')
                                {
                                    State = parseState.NotNumber;
                                    return;
                                }
                                if (++Current == end)
                                {
                                    State = parseState.CrashEnd;
                                    return;
                                }
                                parseHex32(ref number);
                                value = sign == 0 ? (short)(ushort)number : (short)-(int)number;
                            }
                            else value = sign == 0 ? (short)(ushort)parseUInt32(number) : (short)-(int)parseUInt32(number);
                            if (State == parseState.Success)
                            {
                                if (Current == end) State = parseState.CrashEnd;
                                else if (*Current == quote) ++Current;
                                else State = parseState.NotNumber;
                            }
                            return;
                        }
                        if (*Current != '-')
                        {
                            State = parseState.NotNumber;
                            return;
                        }
                        if (++Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        if ((number = (uint)(*Current - '0')) > 9)
                        {
                            State = parseState.NotNumber;
                            return;
                        }
                        sign = 1;
                    }
                }
            }
            if (++Current == end)
            {
                value = sign == 0 ? (short)(ushort)number : (short)-(int)number;
                return;
            }
            if (number == 0)
            {
                if (*Current != 'x')
                {
                    value = 0;
                    return;
                }
                if (++Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                parseHex32(ref number);
                value = sign == 0 ? (short)(ushort)number : (short)-(int)number;
                return;
            }
            value = sign == 0 ? (short)(ushort)parseUInt32(number) : (short)-(int)parseUInt32(number);
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref short? value)
        {
            int sign = 0;
            uint number = (uint)(*Current - '0');
            if (number > 9)
            {
                if (isNull())
                {
                    value = null;
                    return;
                }
                if (*Current == '-')
                {
                    if (++Current == end)
                    {
                        State = parseState.CrashEnd;
                        return;
                    }
                    if ((number = (uint)(*Current - '0')) > 9)
                    {
                        State = parseState.NotNumber;
                        return;
                    }
                    sign = 1;
                }
                else
                {
                    space();
                    if (State != parseState.Success) return;
                    if (Current == end)
                    {
                        State = parseState.CrashEnd;
                        return;
                    }
                    if ((number = (uint)(*Current - '0')) > 9)
                    {
                        if (isNull())
                        {
                            value = null;
                            return;
                        }
                        if (*Current == '"' || *Current == '\'')
                        {
                            quote = *Current;
                            if (++Current == end)
                            {
                                State = parseState.CrashEnd;
                                return;
                            }
                            if ((number = (uint)(*Current - '0')) > 9)
                            {
                                if (*Current != '-')
                                {
                                    State = parseState.NotNumber;
                                    return;
                                }
                                if (++Current == end)
                                {
                                    State = parseState.CrashEnd;
                                    return;
                                }
                                if ((number = (uint)(*Current - '0')) > 9)
                                {
                                    State = parseState.NotNumber;
                                    return;
                                }
                                sign = 1;
                            }
                            if (++Current == end)
                            {
                                State = parseState.CrashEnd;
                                return;
                            }
                            if (number == 0)
                            {
                                if (*Current == quote)
                                {
                                    value = 0;
                                    ++Current;
                                    return;
                                }
                                if (*Current != 'x')
                                {
                                    State = parseState.NotNumber;
                                    return;
                                }
                                if (++Current == end)
                                {
                                    State = parseState.CrashEnd;
                                    return;
                                }
                                parseHex32(ref number);
                                value = sign == 0 ? (short)(ushort)number : (short)-(int)number;
                            }
                            else value = sign == 0 ? (short)(ushort)parseUInt32(number) : (short)-(int)parseUInt32(number);
                            if (State == parseState.Success)
                            {
                                if (Current == end) State = parseState.CrashEnd;
                                else if (*Current == quote) ++Current;
                                else State = parseState.NotNumber;
                            }
                            return;
                        }
                        if (*Current != '-')
                        {
                            State = parseState.NotNumber;
                            return;
                        }
                        if (++Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        if ((number = (uint)(*Current - '0')) > 9)
                        {
                            State = parseState.NotNumber;
                            return;
                        }
                        sign = 1;
                    }
                }
            }
            if (++Current == end)
            {
                value = sign == 0 ? (short)(ushort)number : (short)-(int)number;
                return;
            }
            if (number == 0)
            {
                if (*Current != 'x')
                {
                    value = 0;
                    return;
                }
                if (++Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                parseHex32(ref number);
                value = sign == 0 ? (short)(ushort)number : (short)-(int)number;
                return;
            }
            value = sign == 0 ? (short)(ushort)parseUInt32(number) : (short)-(int)parseUInt32(number);
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        internal void Parse(ref uint value)
        {
            uint number = (uint)(*Current - '0');
            if (number > 9)
            {
                space();
                if (State != parseState.Success) return;
                if (Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                if ((number = (uint)(*Current - '0')) > 9)
                {
                    if (*Current == '"' || *Current == '\'')
                    {
                        quote = *Current;
                        if (++Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        if ((number = (uint)(*Current - '0')) > 9)
                        {
                            State = parseState.NotNumber;
                            return;
                        }
                        if (++Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        if (number == 0)
                        {
                            if (*Current == quote)
                            {
                                value = 0;
                                ++Current;
                                return;
                            }
                            if (*Current != 'x')
                            {
                                State = parseState.NotNumber;
                                return;
                            }
                            if (++Current == end)
                            {
                                State = parseState.CrashEnd;
                                return;
                            }
                            parseHex32(ref number);
                            value = number;
                        }
                        else value = parseUInt32(number);
                        if (State == parseState.Success)
                        {
                            if (Current == end) State = parseState.CrashEnd;
                            else if (*Current == quote) ++Current;
                            else State = parseState.NotNumber;
                        }
                        return;
                    }
                    State = parseState.NotNumber;
                    return;
                }
            }
            if (++Current == end)
            {
                value = number;
                return;
            }
            if (number == 0)
            {
                if (*Current != 'x')
                {
                    value = 0;
                    return;
                }
                if (++Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                parseHex32(ref number);
                value = number;
                return;
            }
            value = parseUInt32(number);
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref uint? value)
        {
            uint number = (uint)(*Current - '0');
            if (number > 9)
            {
                if (isNull())
                {
                    value = null;
                    return;
                }
                space();
                if (State != parseState.Success) return;
                if (Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                if ((number = (uint)(*Current - '0')) > 9)
                {
                    if (isNull())
                    {
                        value = null;
                        return;
                    }
                    if (*Current == '"' || *Current == '\'')
                    {
                        quote = *Current;
                        if (++Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        if ((number = (uint)(*Current - '0')) > 9)
                        {
                            State = parseState.NotNumber;
                            return;
                        }
                        if (++Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        if (number == 0)
                        {
                            if (*Current == quote)
                            {
                                value = 0;
                                ++Current;
                                return;
                            }
                            if (*Current != 'x')
                            {
                                State = parseState.NotNumber;
                                return;
                            }
                            if (++Current == end)
                            {
                                State = parseState.CrashEnd;
                                return;
                            }
                            parseHex32(ref number);
                            value = number;
                        }
                        else value = parseUInt32(number);
                        if (State == parseState.Success)
                        {
                            if (Current == end) State = parseState.CrashEnd;
                            else if (*Current == quote) ++Current;
                            else State = parseState.NotNumber;
                        }
                        return;
                    }
                    State = parseState.NotNumber;
                    return;
                }
            }
            if (++Current == end)
            {
                value = number;
                return;
            }
            if (number == 0)
            {
                if (*Current != 'x')
                {
                    value = 0;
                    return;
                }
                if (++Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                parseHex32(ref number);
                value = number;
                return;
            }
            value = parseUInt32(number);
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        public void Parse(ref int value)
        {
            int sign = 0;
            uint number = (uint)(*Current - '0');
            if (number > 9)
            {
                if (*Current == '-')
                {
                    if (++Current == end)
                    {
                        State = parseState.CrashEnd;
                        return;
                    }
                    if ((number = (uint)(*Current - '0')) > 9)
                    {
                        State = parseState.NotNumber;
                        return;
                    }
                    sign = 1;
                }
                else
                {
                    space();
                    if (State != parseState.Success) return;
                    if (Current == end)
                    {
                        State = parseState.CrashEnd;
                        return;
                    }
                    if ((number = (uint)(*Current - '0')) > 9)
                    {
                        if (*Current == '"' || *Current == '\'')
                        {
                            quote = *Current;
                            if (++Current == end)
                            {
                                State = parseState.CrashEnd;
                                return;
                            }
                            if ((number = (uint)(*Current - '0')) > 9)
                            {
                                if (*Current != '-')
                                {
                                    State = parseState.NotNumber;
                                    return;
                                }
                                if (++Current == end)
                                {
                                    State = parseState.CrashEnd;
                                    return;
                                }
                                if ((number = (uint)(*Current - '0')) > 9)
                                {
                                    State = parseState.NotNumber;
                                    return;
                                }
                                sign = 1;
                            }
                            if (++Current == end)
                            {
                                State = parseState.CrashEnd;
                                return;
                            }
                            if (number == 0)
                            {
                                if (*Current == quote)
                                {
                                    value = 0;
                                    ++Current;
                                    return;
                                }
                                if (*Current != 'x')
                                {
                                    State = parseState.NotNumber;
                                    return;
                                }
                                if (++Current == end)
                                {
                                    State = parseState.CrashEnd;
                                    return;
                                }
                                parseHex32(ref number);
                                value = sign == 0 ? (int)number : -(int)number;
                            }
                            else value = sign == 0 ? (int)parseUInt32(number) : -(int)parseUInt32(number);
                            if (State == parseState.Success)
                            {
                                if (Current == end) State = parseState.CrashEnd;
                                else if (*Current == quote) ++Current;
                                else State = parseState.NotNumber;
                            }
                            return;
                        }
                        if (*Current != '-')
                        {
                            State = parseState.NotNumber;
                            return;
                        }
                        if (++Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        if ((number = (uint)(*Current - '0')) > 9)
                        {
                            State = parseState.NotNumber;
                            return;
                        }
                        sign = 1;
                    }
                }
            }
            if (++Current == end)
            {
                value = sign == 0 ? (int)number : -(int)number;
                return;
            }
            if (number == 0)
            {
                if (*Current != 'x')
                {
                    value = 0;
                    return;
                }
                if (++Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                parseHex32(ref number);
                value = sign == 0 ? (int)number : -(int)number;
                return;
            }
            value = sign == 0 ? (int)parseUInt32(number) : -(int)parseUInt32(number);
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref int? value)
        {
            int sign = 0;
            uint number = (uint)(*Current - '0');
            if (number > 9)
            {
                if (isNull())
                {
                    value = null;
                    return;
                }
                if (*Current == '-')
                {
                    if (++Current == end)
                    {
                        State = parseState.CrashEnd;
                        return;
                    }
                    if ((number = (uint)(*Current - '0')) > 9)
                    {
                        State = parseState.NotNumber;
                        return;
                    }
                    sign = 1;
                }
                else
                {
                    space();
                    if (State != parseState.Success) return;
                    if (Current == end)
                    {
                        State = parseState.CrashEnd;
                        return;
                    }
                    if ((number = (uint)(*Current - '0')) > 9)
                    {
                        if (isNull())
                        {
                            value = null;
                            return;
                        }
                        if (*Current == '"' || *Current == '\'')
                        {
                            quote = *Current;
                            if (++Current == end)
                            {
                                State = parseState.CrashEnd;
                                return;
                            }
                            if ((number = (uint)(*Current - '0')) > 9)
                            {
                                if (*Current != '-')
                                {
                                    State = parseState.NotNumber;
                                    return;
                                }
                                if (++Current == end)
                                {
                                    State = parseState.CrashEnd;
                                    return;
                                }
                                if ((number = (uint)(*Current - '0')) > 9)
                                {
                                    State = parseState.NotNumber;
                                    return;
                                }
                                sign = 1;
                            }
                            if (++Current == end)
                            {
                                State = parseState.CrashEnd;
                                return;
                            }
                            if (number == 0)
                            {
                                if (*Current == quote)
                                {
                                    value = 0;
                                    ++Current;
                                    return;
                                }
                                if (*Current != 'x')
                                {
                                    State = parseState.NotNumber;
                                    return;
                                }
                                if (++Current == end)
                                {
                                    State = parseState.CrashEnd;
                                    return;
                                }
                                parseHex32(ref number);
                                value = sign == 0 ? (int)number : -(int)number;
                            }
                            else value = sign == 0 ? (int)parseUInt32(number) : -(int)parseUInt32(number);
                            if (State == parseState.Success)
                            {
                                if (Current == end) State = parseState.CrashEnd;
                                else if (*Current == quote) ++Current;
                                else State = parseState.NotNumber;
                            }
                            return;
                        }
                        if (*Current != '-')
                        {
                            State = parseState.NotNumber;
                            return;
                        }
                        if (++Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        if ((number = (uint)(*Current - '0')) > 9)
                        {
                            State = parseState.NotNumber;
                            return;
                        }
                        sign = 1;
                    }
                }
            }
            if (++Current == end)
            {
                value = sign == 0 ? (int)number : -(int)number;
                return;
            }
            if (number == 0)
            {
                if (*Current != 'x')
                {
                    value = 0;
                    return;
                }
                if (++Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                parseHex32(ref number);
                value = sign == 0 ? (int)number : -(int)number;
                return;
            }
            value = sign == 0 ? (int)parseUInt32(number) : -(int)parseUInt32(number);
        }
        /// <summary>
        /// 解析10进制数字
        /// </summary>
        /// <param name="value">第一位数字</param>
        /// <returns>数字</returns>
        private ulong parseUInt64(uint value)
        {
            char* end32 = Current + 8;
            if (end32 > end) end32 = end;
            uint number;
            do
            {
                if ((number = (uint)(*Current - '0')) > 9) return value;
                value *= 10;
                value += number;
            }
            while (++Current != end32);
            if (Current == end) return value;
            ulong value64 = value;
            if (isEndDigital)
            {
                do
                {
                    if ((number = (uint)(*Current - '0')) > 9) return value64;
                    value64 *= 10;
                    value64 += number;
                    if (++Current == end) return value64;
                }
                while (true);
            }
            while ((number = (uint)(*Current - '0')) < 10)
            {
                value64 *= 10;
                ++Current;
                value64 += (byte)number;
            }
            return value64;
        }
        /// <summary>
        /// 解析16进制数字
        /// </summary>
        /// <returns>数字</returns>
        private ulong parseHex64()
        {
            uint number = (uint)(*Current - '0');
            if (number > 9)
            {
                if ((number = (number - ('A' - '0')) & 0xffdfU) > 5)
                {
                    State = parseState.NotHex;
                    return 0;
                }
                number += 10;
            }
            if (++Current == end) return number;
            uint high = number;
            char* end32 = Current + 7;
            if (end32 > end) end32 = end;
            do
            {
                if ((number = (uint)(*Current - '0')) > 9)
                {
                    if ((number = (number - ('A' - '0')) & 0xffdfU) > 5) return high;
                    number += 10;
                }
                high <<= 4;
                high += number;
            }
            while (++Current != end32);
            if (Current == end) return high;
            char* start = Current;
            ulong low = number;
            if (isEndHex)
            {
                do
                {
                    if ((number = (uint)(*Current - '0')) > 9)
                    {
                        if ((number = (number - ('A' - '0')) & 0xffdfU) > 5)
                        {
                            return low | (ulong)high << ((int)((byte*)Current - (byte*)start) << 1);
                        }
                        number += 10;
                    }
                    low <<= 4;
                    low += number;
                }
                while (++Current != end);
                return low | (ulong)high << ((int)((byte*)Current - (byte*)start) << 1);
            }
            do
            {
                if ((number = (uint)(*Current - '0')) > 9)
                {
                    if ((number = (number - ('A' - '0')) & 0xffdfU) > 5)
                    {
                        return low | (ulong)high << ((int)((byte*)Current - (byte*)start) << 1);
                    }
                    number += 10;
                }
                low <<= 4;
                ++Current;
                low += number;
            }
            while (true);
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        internal void Parse(ref ulong value)
        {
            uint number = (uint)(*Current - '0');
            if (number > 9)
            {
                space();
                if (State != parseState.Success) return;
                if (Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                if ((number = (uint)(*Current - '0')) > 9)
                {
                    if (*Current == '"' || *Current == '\'')
                    {
                        quote = *Current;
                        if (++Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        if ((number = (uint)(*Current - '0')) > 9)
                        {
                            State = parseState.NotNumber;
                            return;
                        }
                        if (++Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        if (number == 0)
                        {
                            if (*Current == quote)
                            {
                                value = 0;
                                ++Current;
                                return;
                            }
                            if (*Current != 'x')
                            {
                                State = parseState.NotNumber;
                                return;
                            }
                            if (++Current == end)
                            {
                                State = parseState.CrashEnd;
                                return;
                            }
                            value = parseHex64();
                        }
                        else value = parseUInt64(number);
                        if (State == parseState.Success)
                        {
                            if (Current == end) State = parseState.CrashEnd;
                            else if (*Current == quote) ++Current;
                            else State = parseState.NotNumber;
                        }
                        return;
                    }
                    State = parseState.NotNumber;
                    return;
                }
            }
            if (++Current == end)
            {
                value = number;
                return;
            }
            if (number == 0)
            {
                if (*Current != 'x')
                {
                    value = 0;
                    return;
                }
                if (++Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                value = parseHex64();
                return;
            }
            value = parseUInt64(number);
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref ulong? value)
        {
            uint number = (uint)(*Current - '0');
            if (number > 9)
            {
                if (isNull())
                {
                    value = null;
                    return;
                }
                space();
                if (State != parseState.Success) return;
                if (Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                if ((number = (uint)(*Current - '0')) > 9)
                {
                    if (isNull())
                    {
                        value = null;
                        return;
                    }
                    if (*Current == '"' || *Current == '\'')
                    {
                        quote = *Current;
                        if (++Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        if ((number = (uint)(*Current - '0')) > 9)
                        {
                            State = parseState.NotNumber;
                            return;
                        }
                        if (++Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        if (number == 0)
                        {
                            if (*Current == quote)
                            {
                                value = 0;
                                ++Current;
                                return;
                            }
                            if (*Current != 'x')
                            {
                                State = parseState.NotNumber;
                                return;
                            }
                            if (++Current == end)
                            {
                                State = parseState.CrashEnd;
                                return;
                            }
                            value = parseHex64();
                        }
                        else value = parseUInt64(number);
                        if (State == parseState.Success)
                        {
                            if (Current == end) State = parseState.CrashEnd;
                            else if (*Current == quote) ++Current;
                            else State = parseState.NotNumber;
                        }
                        return;
                    }
                    State = parseState.NotNumber;
                    return;
                }
            }
            if (++Current == end)
            {
                value = number;
                return;
            }
            if (number == 0)
            {
                if (*Current != 'x')
                {
                    value = 0;
                    return;
                }
                if (++Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                value = parseHex64();
                return;
            }
            value = parseUInt64(number);
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        internal void Parse(ref long value)
        {
            int sign = 0;
            uint number = (uint)(*Current - '0');
            if (number > 9)
            {
                if (*Current == '-')
                {
                    if (++Current == end)
                    {
                        State = parseState.CrashEnd;
                        return;
                    }
                    if ((number = (uint)(*Current - '0')) > 9)
                    {
                        State = parseState.NotNumber;
                        return;
                    }
                    sign = 1;
                }
                else
                {
                    space();
                    if (State != parseState.Success) return;
                    if (Current == end)
                    {
                        State = parseState.CrashEnd;
                        return;
                    }
                    if ((number = (uint)(*Current - '0')) > 9)
                    {
                        if (*Current == '"' || *Current == '\'')
                        {
                            quote = *Current;
                            if (++Current == end)
                            {
                                State = parseState.CrashEnd;
                                return;
                            }
                            if ((number = (uint)(*Current - '0')) > 9)
                            {
                                if (*Current != '-')
                                {
                                    State = parseState.NotNumber;
                                    return;
                                }
                                if (++Current == end)
                                {
                                    State = parseState.CrashEnd;
                                    return;
                                }
                                if ((number = (uint)(*Current - '0')) > 9)
                                {
                                    State = parseState.NotNumber;
                                    return;
                                }
                                sign = 1;
                            }
                            if (++Current == end)
                            {
                                State = parseState.CrashEnd;
                                return;
                            }
                            if (number == 0)
                            {
                                if (*Current == quote)
                                {
                                    value = 0;
                                    ++Current;
                                    return;
                                }
                                if (*Current != 'x')
                                {
                                    State = parseState.NotNumber;
                                    return;
                                }
                                if (++Current == end)
                                {
                                    State = parseState.CrashEnd;
                                    return;
                                }
                                value = (long)parseHex64();
                            }
                            else value = (long)parseUInt64(number);
                            if (State == parseState.Success)
                            {
                                if (Current == end) State = parseState.CrashEnd;
                                else if (*Current == quote)
                                {
                                    if (sign != 0) value = -value;
                                    ++Current;
                                }
                                else State = parseState.NotNumber;
                            }
                            return;
                        }
                        if (*Current != '-')
                        {
                            State = parseState.NotNumber;
                            return;
                        }
                        if (++Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        if ((number = (uint)(*Current - '0')) > 9)
                        {
                            State = parseState.NotNumber;
                            return;
                        }
                        sign = 1;
                    }
                }
            }
            if (++Current == end)
            {
                value = sign == 0 ? (long)(int)number : -(long)(int)number;
                return;
            }
            if (number == 0)
            {
                if (*Current != 'x')
                {
                    value = 0;
                    return;
                }
                if (++Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                value = (long)parseHex64();
                if (sign != 0) value = -value;
                return;
            }
            value = (long)parseUInt64(number);
            if (sign != 0) value = -value;
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref long? value)
        {
            int sign = 0;
            uint number = (uint)(*Current - '0');
            if (number > 9)
            {
                if (isNull())
                {
                    value = null;
                    return;
                }
                if (*Current == '-')
                {
                    if (++Current == end)
                    {
                        State = parseState.CrashEnd;
                        return;
                    }
                    if ((number = (uint)(*Current - '0')) > 9)
                    {
                        State = parseState.NotNumber;
                        return;
                    }
                    sign = 1;
                }
                else
                {
                    space();
                    if (State != parseState.Success) return;
                    if (Current == end)
                    {
                        State = parseState.CrashEnd;
                        return;
                    }
                    if ((number = (uint)(*Current - '0')) > 9)
                    {
                        if (isNull())
                        {
                            value = null;
                            return;
                        }
                        if (*Current == '"' || *Current == '\'')
                        {
                            quote = *Current;
                            if (++Current == end)
                            {
                                State = parseState.CrashEnd;
                                return;
                            }
                            if ((number = (uint)(*Current - '0')) > 9)
                            {
                                if (*Current != '-')
                                {
                                    State = parseState.NotNumber;
                                    return;
                                }
                                if (++Current == end)
                                {
                                    State = parseState.CrashEnd;
                                    return;
                                }
                                if ((number = (uint)(*Current - '0')) > 9)
                                {
                                    State = parseState.NotNumber;
                                    return;
                                }
                                sign = 1;
                            }
                            if (++Current == end)
                            {
                                State = parseState.CrashEnd;
                                return;
                            }
                            if (number == 0)
                            {
                                if (*Current == quote)
                                {
                                    value = 0;
                                    ++Current;
                                    return;
                                }
                                if (*Current != 'x')
                                {
                                    State = parseState.NotNumber;
                                    return;
                                }
                                if (++Current == end)
                                {
                                    State = parseState.CrashEnd;
                                    return;
                                }
                                value = (long)parseHex64();
                            }
                            else value = (long)parseUInt64(number);
                            if (State == parseState.Success)
                            {
                                if (Current == end) State = parseState.CrashEnd;
                                else if (*Current == quote)
                                {
                                    if (sign != 0) value = -value;
                                    ++Current;
                                }
                                else State = parseState.NotNumber;
                            }
                            return;
                        }
                        if (*Current != '-')
                        {
                            State = parseState.NotNumber;
                            return;
                        }
                        if (++Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        if ((number = (uint)(*Current - '0')) > 9)
                        {
                            State = parseState.NotNumber;
                            return;
                        }
                        sign = 1;
                    }
                }
            }
            if (++Current == end)
            {
                value = sign == 0 ? (long)(int)number : -(long)(int)number;
                return;
            }
            if (number == 0)
            {
                if (*Current != 'x')
                {
                    value = 0;
                    return;
                }
                if (++Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                long hexValue = (long)parseHex64();
                value = sign == 0 ? hexValue : -hexValue;
                return;
            }
            long value64 = (long)parseUInt64(number);
            value = sign == 0 ? value64 : -value64;
        }
        /// <summary>
        /// 查找数字结束位置
        /// </summary>
        /// <returns>数字结束位置,失败返回null</returns>
        private char* searchNumber()
        {
            if (((bits[*(byte*)Current] & numberBit) | *(((byte*)Current) + 1)) != 0)
            {
                space();
                if (State != parseState.Success) return null;
                if (Current == end)
                {
                    State = parseState.CrashEnd;
                    return null;
                }
                if (*Current == '"' || *Current == '\'')
                {
                    quote = *Current;
                    if (++Current == end)
                    {
                        State = parseState.CrashEnd;
                        return null;
                    }
                    char* stringEnd = Current;
                    if (endChar == quote)
                    {
                        while (*stringEnd != quote) ++stringEnd;
                    }
                    else
                    {
                        while (*stringEnd != quote)
                        {
                            if (++stringEnd == end)
                            {
                                State = parseState.CrashEnd;
                                return null;
                            }
                        }
                    }
                    return stringEnd;
                }
                if (((bits[*(byte*)Current] & numberBit) | *(((byte*)Current) + 1)) != 0)
                {
                    if (isNaN()) return jsonFixed;
                    State = parseState.NotNumber;
                    return null;
                }
            }
            char* numberEnd = Current;
            if (isEndNumber)
            {
                while (++numberEnd != end && ((bits[*(byte*)numberEnd] & numberBit) | *(((byte*)numberEnd) + 1)) == 0) ;
            }
            else
            {
                while (((bits[*(byte*)++numberEnd] & numberBit) | *(((byte*)numberEnd) + 1)) == 0) ;
            }
            quote = (char)0;
            return numberEnd;
        }
        /// <summary>
        /// 查找数字结束位置
        /// </summary>
        /// <returns>数字结束位置,失败返回null</returns>
        private char* searchNumberNull()
        {
            if (((bits[*(byte*)Current] & numberBit) | *(((byte*)Current) + 1)) != 0)
            {
                if (isNull()) return jsonFixed;
                space();
                if (State != parseState.Success) return null;
                if (Current == end)
                {
                    State = parseState.CrashEnd;
                    return null;
                }
                if (*Current == '"' || *Current == '\'')
                {
                    quote = *Current;
                    if (++Current == end)
                    {
                        State = parseState.CrashEnd;
                        return null;
                    }
                    char* stringEnd = Current;
                    if (endChar == quote)
                    {
                        while (*stringEnd != quote) ++stringEnd;
                    }
                    else
                    {
                        while (*stringEnd != quote)
                        {
                            if (++stringEnd == end)
                            {
                                State = parseState.CrashEnd;
                                return null;
                            }
                        }
                    }
                    return stringEnd;
                }
                if (((bits[*(byte*)Current] & numberBit) | *(((byte*)Current) + 1)) != 0)
                {
                    if (isNull() || isNaN()) return jsonFixed;
                    State = parseState.NotNumber;
                    return null;
                }
            }
            char* numberEnd = Current;
            if (isEndNumber)
            {
                while (++numberEnd != end && ((bits[*(byte*)numberEnd] & numberBit) | *(((byte*)numberEnd) + 1)) == 0) ;
            }
            else
            {
                while (((bits[*(byte*)++numberEnd] & numberBit) | *(((byte*)numberEnd) + 1)) == 0) ;
            }
            quote = (char)0;
            return numberEnd;
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        internal void Parse(ref float value)
        {
            char* end = searchNumber();
            if (end != null)
            {
                if (end == jsonFixed) value = float.NaN;
                else
                {
                    string number = this.json == null ? new string(Current, 0, (int)(end - Current)) : this.json.Substring((int)(Current - jsonFixed), (int)(end - Current));
                    if (float.TryParse(number, out value))
                    {
                        Current = end;
                        if (quote != 0) ++Current;
                    }
                    else State = parseState.NotNumber;
                }
            }
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref float? value)
        {
            char* end = searchNumberNull();
            if (end != null)
            {
                if (end == jsonFixed)
                {
                    if (*(Current - 1) == 'l') value = null;
                    else value = float.NaN;
                }
                else
                {
                    string number = this.json == null ? new string(Current, 0, (int)(end - Current)) : this.json.Substring((int)(Current - jsonFixed), (int)(end - Current));
                    float parseValue;
                    if (float.TryParse(number, out parseValue))
                    {
                        Current = end;
                        value = parseValue;
                        if (quote != 0) ++Current;
                    }
                    else State = parseState.NotNumber;
                }
            }
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        internal void Parse(ref double value)
        {
            char* end = searchNumber();
            if (end != null)
            {
                if (end == jsonFixed) value = double.NaN;
                else
                {
                    string number = this.json == null ? new string(Current, 0, (int)(end - Current)) : this.json.Substring((int)(Current - jsonFixed), (int)(end - Current));
                    if (double.TryParse(number, out value))
                    {
                        Current = end;
                        if (quote != 0) ++Current;
                    }
                    else State = parseState.NotNumber;
                }
            }
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref double? value)
        {
            char* end = searchNumberNull();
            if (end != null)
            {
                if (end == jsonFixed)
                {
                    if (*(Current - 1) == 'l') value = null;
                    else value = double.NaN;
                }
                else
                {
                    string number = this.json == null ? new string(Current, 0, (int)(end - Current)) : this.json.Substring((int)(Current - jsonFixed), (int)(end - Current));
                    double parseValue;
                    if (double.TryParse(number, out parseValue))
                    {
                        Current = end;
                        value = parseValue;
                        if (quote != 0) ++Current;
                    }
                    else State = parseState.NotNumber;
                }
            }
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        internal void Parse(ref decimal value)
        {
            char* end = searchNumber();
            if (end != null)
            {
                if (end == jsonFixed) State = parseState.NotNumber;
                else
                {
                    string number = this.json == null ? new string(Current, 0, (int)(end - Current)) : this.json.Substring((int)(Current - jsonFixed), (int)(end - Current));
                    if (decimal.TryParse(number, out value))
                    {
                        Current = end;
                        if (quote != 0) ++Current;
                    }
                    else State = parseState.NotNumber;
                }
            }
        }
        /// <summary>
        /// 数字解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref decimal? value)
        {
            char* end = searchNumberNull();
            if (end != null)
            {
                if (end == jsonFixed)
                {
                    if (*(Current - 1) == 'l') value = null;
                    else State = parseState.NotNumber;
                }
                else
                {
                    string number = this.json == null ? new string(Current, 0, (int)(end - Current)) : this.json.Substring((int)(Current - jsonFixed), (int)(end - Current));
                    decimal parseValue;
                    if (decimal.TryParse(number, out parseValue))
                    {
                        Current = end;
                        value = parseValue;
                        if (quote != 0) ++Current;
                    }
                    else State = parseState.NotNumber;
                }
            }
        }
        /// <summary>
        /// 字符解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        internal void Parse(ref char value)
        {
            byte isSpace = 0;
        START:
            if (*Current == '"' || *Current == '\'')
            {
                quote = *Current;
                if (++Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                if (((bits[*(byte*)Current] & escapeSearchBit) | *(((byte*)Current) + 1)) == 0)
                {
                    if (*Current == '\\')
                    {
                        if (++Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        if (*Current == 'u')
                        {
                            if ((int)((byte*)end - (byte*)Current) < 5 * sizeof(char))
                            {
                                State = parseState.NotChar;
                                return;
                            }
                            value = (char)parseHex4();
                        }
                        else if (*Current == 'x')
                        {
                            if ((int)((byte*)end - (byte*)Current) < 3 * sizeof(char))
                            {
                                State = parseState.NotChar;
                                return;
                            }
                            value = (char)parseHex2();
                        }
                        else value = *Current < escapeCharSize ? escapeChars[*Current] : *Current;
                    }
                    else
                    {
                        if (*Current == quote)
                        {
                            State = parseState.NotChar;
                            return;
                        }
                        value = *Current;
                    }
                }
                else value = *Current;
                if (++Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                if (*Current == quote)
                {
                    ++Current;
                    return;
                }
            }
            else if (isSpace == 0)
            {
                space();
                if (State != parseState.Success) return;
                if (Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                isSpace = 1;
                goto START;
            }
            State = parseState.NotChar;
        }
        /// <summary>
        /// 字符解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref char? value)
        {
            byte isSpace = 0;
        START:
            if (*Current == '"' || *Current == '\'')
            {
                quote = *Current;
                if (++Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                if (((bits[*(byte*)Current] & escapeSearchBit) | *(((byte*)Current) + 1)) == 0)
                {
                    if (*Current == '\\')
                    {
                        if (++Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        if (*Current == 'u')
                        {
                            if ((int)((byte*)end - (byte*)Current) < 5 * sizeof(char))
                            {
                                State = parseState.NotChar;
                                return;
                            }
                            value = (char)parseHex4();
                        }
                        else if (*Current == 'x')
                        {
                            if ((int)((byte*)end - (byte*)Current) < 3 * sizeof(char))
                            {
                                State = parseState.NotChar;
                                return;
                            }
                            value = (char)parseHex2();
                        }
                        else value = *Current < escapeCharSize ? escapeChars[*Current] : *Current;
                    }
                    else
                    {
                        if (*Current == quote)
                        {
                            State = parseState.NotChar;
                            return;
                        }
                        value = *Current;
                    }
                }
                else value = *Current;
                if (++Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                if (*Current == quote)
                {
                    ++Current;
                    return;
                }
            }
            else
            {
                if (*(long*)(Current) == 'n' + ('u' << 16) + ((long)'l' << 32) + ((long)'l' << 48) && (int)((byte*)end - (byte*)Current) >= 4 * sizeof(char))
                {
                    value = null;
                    Current += 4;
                    return;
                }
                if (isSpace == 0)
                {
                    space();
                    if (State != parseState.Success) return;
                    if (Current == end)
                    {
                        State = parseState.CrashEnd;
                        return;
                    }
                    isSpace = 1;
                    goto START;
                }
            }
            State = parseState.NotChar;
        }
        /// <summary>
        /// 时间解析 /Date(xxx)/
        /// </summary>
        /// <param name="timeString"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private unsafe static bool parseTime(string timeString, out DateTime value)
        {
            if (timeString.Length > 8)
            {
                fixed (char* timeFixed = timeString)
                {
                    if (*(long*)(timeFixed + 1) == 'D' + ('a' << 16) + ((long)'t' << 32) + ((long)'e' << 48))
                    {
                        char* end = timeFixed + (timeString.Length - 2);
                        if (((*(timeFixed + 5) ^ '(') | (*(int*)end ^ (')' + ('/' << 16)))) == 0)
                        {
                            char* start = timeFixed + 6;
                            bool isSign;
                            if (*start == '-')
                            {
                                if (timeString.Length == 9)
                                {
                                    value = DateTime.MinValue;
                                    return false;
                                }
                                isSign = true;
                                ++start;
                            }
                            else isSign = false;
                            uint code = (uint)(*start - '0');
                            if (code < 10)
                            {
                                long millisecond = code;
                                while (++start != end)
                                {
                                    if ((code = (uint)(*start - '0')) >= 10)
                                    {
                                        value = DateTime.MinValue;
                                        return false;
                                    }
                                    millisecond *= 10;
                                    millisecond += code;
                                }
                                value = fastCSharp.pub.JavascriptLocalMinTime.AddTicks((isSign ? -millisecond : millisecond) * date.MillisecondTicks);
                                return true;
                            }
                        }
                    }
                }
            }
            value = DateTime.MinValue;
            return false;
        }
        /// <summary>
        /// 时间解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        internal void Parse(ref DateTime value)
        {
            byte isSpace = 0;
        START:
            if (*Current == '"' || *Current == '\'')
            {
                string timeString = parseString();
                if (!string.IsNullOrEmpty(timeString))
                {
                    DateTime parseTime;
                    if (timeString[0] == '/')
                    {
                        if(jsonParser.parseTime(timeString, out parseTime))
                        {
                            value = parseTime;
                            return;
                        }
                    }
                    else
                    {
                        if (DateTime.TryParse(timeString, out parseTime))
                        {
                            value = parseTime;
                            return;
                        }
                    }
                }
            }
            else
            {
                if ((int)((byte*)end - (byte*)Current) >= 4 * sizeof(char))
                {
                    if (*(long*)(Current) == 'n' + ('u' << 16) + ((long)'l' << 32) + ((long)'l' << 48))
                    {
                        value = DateTime.MinValue;
                        Current += 4;
                        return;
                    }
                    if (*Current == 'n' && (int)((byte*)end - (byte*)Current) > 9 * sizeof(char) && ((*(long*)(Current + 1) ^ ('e' + ('w' << 16) + ((long)' ' << 32) + ((long)'D' << 48))) | (*(long*)(Current + 5) ^ ('a' + ('t' << 16) + ((long)'e' << 32) + ((long)'(' << 48)))) == 0)
                    {
                        long millisecond = 0;
                        Current += 9;
                        Parse(ref millisecond);
                        if (State != parseState.Success) return;
                        if (Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        if (*Current == ')')
                        {
                            value = fastCSharp.pub.JavascriptLocalMinTime.AddTicks(millisecond * date.MillisecondTicks);
                            ++Current;
                            return;
                        }
                    }
                }
                if (isSpace == 0)
                {
                    space();
                    if (State != parseState.Success) return;
                    if (Current == end)
                    {
                        State = parseState.CrashEnd;
                        return;
                    }
                    isSpace = 1;
                    goto START;
                }
            }
            State = parseState.NotDateTime;
        }
        /// <summary>
        /// 时间解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref DateTime? value)
        {
            byte isSpace = 0;
        START:
            if (*Current == '"' || *Current == '\'')
            {
                string timeString = parseString();
                if (!string.IsNullOrEmpty(timeString))
                {
                    DateTime parseTime;
                    if (timeString[0] == '/')
                    {
                        if (jsonParser.parseTime(timeString, out parseTime))
                        {
                            value = parseTime;
                            return;
                        }
                    }
                    else
                    {
                        if (DateTime.TryParse(timeString, out parseTime))
                        {
                            value = parseTime;
                            return;
                        }
                    }
                }
            }
            else
            {
                if ((int)((byte*)end - (byte*)Current) >= 4 * sizeof(char))
                {
                    if (*(long*)(Current) == 'n' + ('u' << 16) + ((long)'l' << 32) + ((long)'l' << 48))
                    {
                        value = null;
                        Current += 4;
                        return;
                    }
                    if (*Current == 'n' && (int)((byte*)end - (byte*)Current) > 9 * sizeof(char) && ((*(long*)(Current + 1) ^ ('e' + ('w' << 16) + ((long)' ' << 32) + ((long)'D' << 48))) | (*(long*)(Current + 5) ^ ('a' + ('t' << 16) + ((long)'e' << 32) + ((long)'(' << 48)))) == 0)
                    {
                        long millisecond = 0;
                        Current += 9;
                        Parse(ref millisecond);
                        if (State != parseState.Success) return;
                        if (Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        if (*Current == ')')
                        {
                            value = fastCSharp.pub.JavascriptLocalMinTime.AddTicks(millisecond * date.MillisecondTicks);
                            ++Current;
                            return;
                        }
                    }
                }
                if (isSpace == 0)
                {
                    space();
                    if (State != parseState.Success) return;
                    if (Current == end)
                    {
                        State = parseState.CrashEnd;
                        return;
                    }
                    isSpace = 1;
                    goto START;
                }
            }
            State = parseState.NotDateTime;
        }
        /// <summary>
        /// Guid解析
        /// </summary>
        /// <param name="value">数据</param>
        private void parse(ref guid value)
        {
            if ((int)((byte*)end - (byte*)Current) < 38 * sizeof(char))
            {
                State = parseState.CrashEnd;
                return;
            }
            quote = *Current;
            value.Byte3 = (byte)parseHex2();
            value.Byte2 = (byte)parseHex2();
            value.Byte1 = (byte)parseHex2();
            value.Byte0 = (byte)parseHex2();
            if (*++Current != '-')
            {
                State = parseState.NotGuid;
                return;
            }
            value.Byte45 = (ushort)parseHex4();
            if (*++Current != '-')
            {
                State = parseState.NotGuid;
                return;
            }
            value.Byte67 = (ushort)parseHex4();
            if (*++Current != '-')
            {
                State = parseState.NotGuid;
                return;
            }
            value.Byte8 = (byte)parseHex2();
            value.Byte9 = (byte)parseHex2();
            if (*++Current != '-')
            {
                State = parseState.NotGuid;
                return;
            }
            value.Byte10 = (byte)parseHex2();
            value.Byte11 = (byte)parseHex2();
            value.Byte12 = (byte)parseHex2();
            value.Byte13 = (byte)parseHex2();
            value.Byte14 = (byte)parseHex2();
            value.Byte15 = (byte)parseHex2();
            if (*++Current == quote)
            {
                ++Current;
                return;
            }
            State = parseState.NotGuid;
        }
        /// <summary>
        /// Guid解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        internal void Parse(ref Guid value)
        {
            if (*Current == '\'' || *Current == '"')
            {
                guid guid = new guid();
                parse(ref guid);
                value = guid.Value;
                return;
            }
            space();
            if (State != parseState.Success) return;
            if (Current == end)
            {
                State = parseState.CrashEnd;
                return;
            }
            if (*Current == '\'' || *Current == '"')
            {
                guid guid = new guid();
                parse(ref guid);
                value = guid.Value;
                return;
            }
            State = parseState.NotGuid;
        }
        /// <summary>
        /// Guid解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref Guid? value)
        {
            byte isSpace = 0;
        START:
            if (*Current == '"' || *Current == '\'')
            {
                guid guid = new guid();
                parse(ref guid);
                value = guid.Value;
                return;
            }
            if (*(long*)(Current) == 'n' + ('u' << 16) + ((long)'l' << 32) + ((long)'l' << 48) && (int)((byte*)end - (byte*)Current) >= 4 * sizeof(char))
            {
                value = null;
                Current += 4;
                return;
            }
            if (isSpace == 0)
            {
                space();
                if (State != parseState.Success) return;
                if (Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                isSpace = 1;
                goto START;
            }
            State = parseState.NotGuid;
        }
        /// <summary>
        /// 查找字符串中的转义符
        /// </summary>
        private byte searchEscape()
        {
            if (endChar == quote)
            {
                do
                {
                    if (((bits[*(byte*)Current] & escapeSearchBit) | *(((byte*)Current) + 1)) == 0)
                    {
                        if (*Current == quote) return 0;
                        if (*Current == '\\') return 1;
                        if (*Current == '\n')
                        {
                            State = parseState.StringEnter;
                            return 0;
                        }
                    }
                    ++Current;
                }
                while (true);
            }
            do
            {
                if (((bits[*(byte*)Current] & escapeSearchBit) | *(((byte*)Current) + 1)) == 0)
                {
                    if (*Current == quote) return 0;
                    if (*Current == '\\') return 1;
                    if (*Current == '\n')
                    {
                        State = parseState.StringEnter;
                        return 0;
                    }
                }
                if (++Current == end)
                {
                    State = parseState.CrashEnd;
                    return 0;
                }
            }
            while (true);
        }
        /// <summary>
        /// 解析16进制字符
        /// </summary>
        /// <returns>字符</returns>
        private uint parseHex4()
        {
            uint code = (uint)(*++Current - '0'), number = (uint)(*++Current - '0');
            if (code > 9) code = ((code - ('A' - '0')) & 0xffdfU) + 10;
            if (number > 9) number = ((number - ('A' - '0')) & 0xffdfU) + 10;
            code <<= 12;
            code += (number << 8);
            if ((number = (uint)(*++Current - '0')) > 9) number = ((number - ('A' - '0')) & 0xffdfU) + 10;
            code += (number << 4);
            number = (uint)(*++Current - '0');
            return code + (number > 9 ? (((number - ('A' - '0')) & 0xffdfU) + 10) : number);
        }
        /// <summary>
        /// 解析16进制字符
        /// </summary>
        /// <returns>字符</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private uint parseHex2()
        {
            uint code = (uint)(*++Current - '0'), number = (uint)(*++Current - '0');
            if (code > 9) code = ((code - ('A' - '0')) & 0xffdfU) + 10;
            return (number > 9 ? (((number - ('A' - '0')) & 0xffdfU) + 10) : number) + (code << 4);
        }
        /// <summary>
        /// 字符串转义解析
        /// </summary>
        /// <returns>写入结束位置</returns>
        private char* parseEscape()
        {
            char* write = Current;
        NEXT:
            if (*++Current == 'u')
            {
                if ((int)((byte*)end - (byte*)Current) < 5 * sizeof(char))
                {
                    State = parseState.CrashEnd;
                    return null;
                }
                *write++ = (char)parseHex4();
            }
            else if (*Current == 'x')
            {
                if ((int)((byte*)end - (byte*)Current) < 3 * sizeof(char))
                {
                    State = parseState.CrashEnd;
                    return null;
                }
                *write++ = (char)parseHex2();
            }
            else
            {
                if (Current == end)
                {
                    State = parseState.CrashEnd;
                    return null;
                }
                *write++ = *Current < escapeCharSize ? escapeChars[*Current] : *Current;
            }
            if (++Current == end)
            {
                State = parseState.CrashEnd;
                return null;
            }
            if (endChar == quote)
            {
                do
                {
                    if (((bits[*(byte*)Current] & escapeSearchBit) | *(((byte*)Current) + 1)) == 0)
                    {
                        if (*Current == quote)
                        {
                            ++Current;
                            return write;
                        }
                        if (*Current == '\\') goto NEXT;
                        if (*Current == '\n')
                        {
                            State = parseState.StringEnter;
                            return null;
                        }
                    }
                    *write++ = *Current++;
                }
                while (true);
            }
            do
            {
                if (((bits[*(byte*)Current] & escapeSearchBit) | *(((byte*)Current) + 1)) == 0)
                {
                    if (*Current == quote)
                    {
                        ++Current;
                        return write;
                    }
                    if (*Current == '\\') goto NEXT;
                    if (*Current == '\n')
                    {
                        State = parseState.StringEnter;
                        return null;
                    }
                }
                *write++ = *Current++;
                if (Current == end)
                {
                    State = parseState.CrashEnd;
                    return null;
                }
            }
            while (true);
        }
        /// <summary>
        /// 获取转义后的字符串长度
        /// </summary>
        /// <returns>字符串长度</returns>
        private int parseEscapeSize()
        {
            char* start = Current;
            int length = 0;
        NEXT:
            if (*++Current == 'u')
            {
                if ((int)((byte*)end - (byte*)Current) < 5 * sizeof(char))
                {
                    State = parseState.CrashEnd;
                    return 0;
                }
                length += 5;
                Current += 5;
            }
            else if (*Current == 'x')
            {
                if ((int)((byte*)end - (byte*)Current) < 3 * sizeof(char))
                {
                    State = parseState.CrashEnd;
                    return 0;
                }
                length += 3;
                Current += 3;
            }
            else
            {
                if (Current == end)
                {
                    State = parseState.CrashEnd;
                    return 0;
                }
                ++length;
                ++Current;
            }
            if (Current == end)
            {
                State = parseState.CrashEnd;
                return 0;
            }
            if (endChar == quote)
            {
                do
                {
                    if (((bits[*(byte*)Current] & escapeSearchBit) | *(((byte*)Current) + 1)) == 0)
                    {
                        if (*Current == quote)
                        {
                            length = (int)(Current - start) - length;
                            Current = start;
                            return length;
                        }
                        if (*Current == '\\') goto NEXT;
                        if (*Current == '\n')
                        {
                            State = parseState.StringEnter;
                            return 0;
                        }
                    }
                    ++Current;
                }
                while (true);
            }
            do
            {
                if (((bits[*(byte*)Current] & escapeSearchBit) | *(((byte*)Current) + 1)) == 0)
                {
                    if (*Current == quote)
                    {
                        length = (int)(Current - start) - length;
                        Current = start;
                        return length;
                    }
                    if (*Current == '\\') goto NEXT;
                    if (*Current == '\n')
                    {
                        State = parseState.StringEnter;
                        return 0;
                    }
                }
                if (++Current == end)
                {
                    State = parseState.CrashEnd;
                    return 0;
                }
            }
            while (true);
        }
        /// <summary>
        /// 字符串转义解析
        /// </summary>
        /// <param name="write">当前写入位置</param>
        private void parseEscapeUnsafe(char* write)
        {
        NEXT:
            if (*++Current == 'u') *write++ = (char)parseHex4();
            else if (*Current == 'x') *write++ = (char)parseHex2();
            else *write++ = *Current < escapeCharSize ? escapeChars[*Current] : *Current;
            do
            {
                if (*++Current == quote)
                {
                    ++Current;
                    return;
                }
                if (*Current == '\\') goto NEXT;
                *write++ = *Current;
            }
            while (true);
        }
        /// <summary>
        /// 字符串解析
        /// </summary>
        /// <returns>字符串,失败返回null</returns>
        private string parseString()
        {
            quote = *Current;
            if (++Current == end)
            {
                State = parseState.CrashEnd;
                return null;
            }
            char* start = Current;
            if (searchEscape() == 0) return State == parseState.Success ? new string(start, 0, (int)(Current++ - start)) : null;
            if (Config.IsTempString)
            {
                char* writeEnd = parseEscape();
                return writeEnd != null ? new string(start, 0, (int)(writeEnd - start)) : null;
            }
            return parseEscape(start);
        }
        /// <summary>
        /// 字符串解析
        /// </summary>
        /// <param name="start"></param>
        /// <returns>字符串,失败返回null</returns>
        private string parseEscape(char* start)
        {
            int size = parseEscapeSize();
            if (size != 0)
            {
                int left = (int)(Current - start);
                string value = fastCSharp.String.FastAllocateString(left + size);
                fixed (char* valueFixed = value)
                {
                    fastCSharp.unsafer.memory.Copy((void*)start, valueFixed, left << 1);
                    parseEscapeUnsafe(valueFixed + left);
                    return value;
                }
            }
            return null;
        }
        /// <summary>
        /// 查找枚举数字
        /// </summary>
        /// <returns></returns>
        private bool isEnumNumber()
        {
            if ((uint)(*Current - '0') < 10) return true;
            space();
            if (State != parseState.Success) return false;
            if (Current == end)
            {
                State = parseState.CrashEnd;
                return false;
            }
            return (uint)(*Current - '0') < 10;
        }
        /// <summary>
        /// 查找枚举数字
        /// </summary>
        /// <returns></returns>
        private bool isEnumNumberFlag()
        {
            if ((uint)(*Current - '0') < 10 || *Current == '-') return true;
            space();
            if (State != parseState.Success) return false;
            if (Current == end)
            {
                State = parseState.CrashEnd;
                return false;
            }
            return (uint)(*Current - '0') < 10 || *Current == '-';
        }
        /// <summary>
        /// 查找字符串引号并返回第一个字符
        /// </summary>
        /// <returns>第一个字符,0表示null</returns>
        private char searchQuote()
        {
            if (*Current == '\'' || *Current == '"')
            {
                quote = *Current;
                return nextStringChar();
            }
            if (isNull()) return quote = (char)0;
            space();
            if (State != parseState.Success) return (char)0;
            if (++Current == end)
            {
                State = parseState.CrashEnd;
                return (char)0;
            }
            if (*Current == '\'' || *Current == '"')
            {
                quote = *Current;
                return nextStringChar();
            }
            if (isNull()) return quote = (char)0;
            State = parseState.NotString;
            return (char)0;
        }
        /// <summary>
        /// 读取下一个字符
        /// </summary>
        /// <returns>字符,结束或者错误返回0</returns>
        private char nextStringChar()
        {
            if (++Current == end)
            {
                State = parseState.CrashEnd;
                return (char)0;
            }
            if (((bits[*(byte*)Current] & escapeSearchBit) | *(((byte*)Current) + 1)) == 0)
            {
                if (*Current == quote)
                {
                    ++Current;
                    return quote = (char)0;
                }
                if (*Current == '\\')
                {
                    if (*++Current == 'u')
                    {
                        if ((int)((byte*)end - (byte*)Current) < 5 * sizeof(char))
                        {
                            State = parseState.CrashEnd;
                            return (char)0;
                        }
                        return (char)parseHex4();
                    }
                    if (*Current == 'x')
                    {
                        if ((int)((byte*)end - (byte*)Current) < 3 * sizeof(char))
                        {
                            State = parseState.CrashEnd;
                            return (char)0;
                        }
                        return (char)parseHex2();
                    }
                    if (Current == end)
                    {
                        State = parseState.CrashEnd;
                        return (char)0;
                    }
                    return *Current < escapeCharSize ? escapeChars[*Current] : *Current;
                }
                if (*Current == '\n')
                {
                    State = parseState.StringEnter;
                    return (char)0;
                }
            }
            return *Current;
        }
        /// <summary>
        /// 查找字符串直到结束
        /// </summary>
        private void searchStringEnd()
        {
            if (quote != 0 && State == parseState.Success)
            {
                //++Current;
                do
                {
                    if (Current == end)
                    {
                        State = parseState.CrashEnd;
                        return;
                    }
                    if (endChar == quote)
                    {
                        do
                        {
                            if (((bits[*(byte*)Current] & escapeSearchBit) | *(((byte*)Current) + 1)) == 0)
                            {
                                if (*Current == quote)
                                {
                                    ++Current;
                                    return;
                                }
                                if (*Current == '\\') goto NEXT;
                                if (*Current == '\n')
                                {
                                    State = parseState.StringEnter;
                                    return;
                                }
                            }
                            ++Current;
                        }
                        while (true);
                    }
                    do
                    {
                        if (((bits[*(byte*)Current] & escapeSearchBit) | *(((byte*)Current) + 1)) == 0)
                        {
                            if (*Current == quote)
                            {
                                ++Current;
                                return;
                            }
                            if (*Current == '\\') goto NEXT;
                            if (*Current == '\n')
                            {
                                State = parseState.StringEnter;
                                return;
                            }
                        }
                        if (++Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                    }
                    while (true);
                NEXT:
                    if (*++Current == 'u')
                    {
                        if ((int)((byte*)end - (byte*)Current) < 5 * sizeof(char))
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        Current += 5;
                    }
                    else if (*Current == 'x')
                    {
                        if ((int)((byte*)end - (byte*)Current) < 3 * sizeof(char))
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        Current += 3;
                    }
                    else
                    {
                        if (Current == end)
                        {
                            State = parseState.CrashEnd;
                            return;
                        }
                        ++Current;
                    }
                }
                while (true);
            }
        }
        /// <summary>
        /// 字符串解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        internal void Parse(ref string value)
        {
            byte isSpace = 0;
        START:
            if (*Current == '"' || *Current == '\'')
            {
                value = parseString();
                return;
            }
            if (*(long*)(Current) == 'n' + ('u' << 16) + ((long)'l' << 32) + ((long)'l' << 48) && (int)((byte*)end - (byte*)Current) >= 4 * sizeof(char))
            {
                value = null;
                Current += 4;
                return;
            }
            if (isSpace == 0)
            {
                space();
                if (State != parseState.Success) return;
                if (Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                isSpace = 1;
                goto START;
            }
            State = parseState.NotString;
        }
        /// <summary>
        /// 字符串解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        public void Parse(ref subString value)
        {
            byte isSpace = 0;
        START:
            if (*Current == '"' || *Current == '\'')
            {
                quote = *Current;
                if (++Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                char* start = Current;
                if (searchEscape() == 0)
                {
                    if (State == parseState.Success)
                    {
                        if (this.json == null) value = new string(start, 0, (int)(Current++ - start));
                        else value.UnsafeSet(this.json, (int)(start - jsonFixed), (int)(Current++ - start));
                    }
                    return;
                }
                if (Config.IsTempString && this.json != null)
                {
                    char* writeEnd = parseEscape();
                    if (writeEnd != null) value.UnsafeSet(this.json, (int)(start - jsonFixed), (int)(writeEnd - start));
                }
                else
                {
                    string newValue = parseEscape(start);
                    if (newValue != null) value.UnsafeSet(newValue, 0, newValue.Length);
                }
                return;
            }
            if (*(long*)(Current) == 'n' + ('u' << 16) + ((long)'l' << 32) + ((long)'l' << 48) && (int)((byte*)end - (byte*)Current) >= 4 * sizeof(char))
            {
                value.Null();
                Current += 4;
                return;
            }
            if (isSpace == 0)
            {
                space();
                if (State != parseState.Success) return;
                if (Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                isSpace = 1;
                goto START;
            }
            State = parseState.NotString;
        }
        /// <summary>
        /// JSON节点解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref jsonNode value)
        {
            space();
            if (State != parseState.Success) return;
            if (Current == end)
            {
                State = parseState.CrashEnd;
                return;
            }
            switch (*Current & 7)
            {
                case '"' & 7:
                case '\'' & 7:
                    if (*Current == '"' || *Current == '\'')
                    {
                        parseStringNode(ref value);
                        return;
                    }
                    goto NUMBER;
                case '{' & 7:
                    if (*Current == '{')
                    {
                        subArray<keyValue<jsonNode, jsonNode>> dictionary = parseDictionaryNode();
                        if (State == parseState.Success) value.SetDictionary(ref dictionary);
                        return;
                    }
                    if (*Current == '[')
                    {
                        subArray<jsonNode> list = parseListNode();
                        if (State == parseState.Success) value.SetList(ref list);
                        {
                            value.Type = jsonNode.type.List;
                        }
                        return;
                    }
                    goto NUMBER;
                case 't' & 7:
                    if ((int)((byte*)end - (byte*)Current) >= 4 * sizeof(char) && *(long*)(Current) == 't' + ('r' << 16) + ((long)'u' << 32) + ((long)'e' << 48))
                    {
                        Current += 4;
                        value.Int64 = 1;
                        value.Type = jsonNode.type.Bool;
                        return;
                    }
                    goto NUMBER;
                case 'f' & 7:
                    if (*Current == 'f')
                    {
                        if ((int)((byte*)end - (byte*)Current) >= 5 * sizeof(char) && *(long*)(Current + 1) == 'a' + ('l' << 16) + ((long)'s' << 32) + ((long)'e' << 48))
                        {
                            Current += 5;
                            value.Int64 = 0;
                            value.Type = jsonNode.type.Bool;
                            return;
                        }
                        break;
                    }
                    if ((int)((byte*)end - (byte*)Current) >= 4 * sizeof(char))
                    {
                        if (*(long*)(Current) == 'n' + ('u' << 16) + ((long)'l' << 32) + ((long)'l' << 48))
                        {
                            value.Type = jsonNode.type.Null;
                            Current += 4;
                            return;
                        }
                        if ((int)((byte*)end - (byte*)Current) > 9 * sizeof(char) && ((*(long*)(Current + 1) ^ ('e' + ('w' << 16) + ((long)' ' << 32) + ((long)'D' << 48))) | (*(long*)(Current + 5) ^ ('a' + ('t' << 16) + ((long)'e' << 32) + ((long)'(' << 48)))) == 0)
                        {
                            long millisecond = 0;
                            Current += 9;
                            Parse(ref millisecond);
                            if (State != parseState.Success) return;
                            if (Current == end)
                            {
                                State = parseState.CrashEnd;
                                return;
                            }
                            if (*Current == ')')
                            {
                                value.Int64 = fastCSharp.pub.JavascriptLocalMinTime.Ticks + millisecond * date.MillisecondTicks;
                                value.Type = jsonNode.type.DateTimeTick;
                                ++Current;
                                return;
                            }
                            break;
                        }
                    }
                    goto NUMBER;
                default:
                NUMBER:
                    char* numberEnd = searchNumber();
                    if (numberEnd != null)
                    {
                        if (numberEnd == jsonFixed) value.Type = jsonNode.type.NaN;
                        else
                        {
                            if (json == null) value.String = new string(Current, 0, (int)(numberEnd - Current));
                            else value.String.UnsafeSet(this.json, (int)(Current - jsonFixed), (int)(numberEnd - Current));
                            Current = numberEnd;
                            if (quote != 0) ++Current;
                            value.SetNumberString(quote);
                        }
                        return;
                    }
                    break;
            }
            State = parseState.UnknownValue;
        }
        /// <summary>
        /// 解析字符串节点
        /// </summary>
        /// <param name="value"></param>
        private void parseStringNode(ref jsonNode value)
        {
            quote = *Current;
            if (++Current == end)
            {
                State = parseState.CrashEnd;
                return;
            }
            char* start = Current;
            if (searchEscape() == 0)
            {
                if (State == parseState.Success) 
                {
                    if (this.json == null) value.String = new string(start, 0, (int)(Current++ - start));
                    else value.String.UnsafeSet(this.json, (int)(start - jsonFixed), (int)(Current++ - start));
                    value.Type = jsonNode.type.String;
                }
                return;
            }
            if (this.json != null)
            {
                char* escapeStart = Current;
                searchEscapeEnd();
                if (State == parseState.Success)
                {
                    value.String.UnsafeSet(this.json, (int)(start - jsonFixed), (int)(Current - start));
                    value.SetQuoteString((int)(escapeStart - start), quote, Config.IsTempString);
                    ++Current;
                }
            }
            else
            {
                string newValue = parseEscape(start);
                if (newValue != null)
                {
                    value.String.UnsafeSet(newValue, 0, newValue.Length);
                    value.Type = jsonNode.type.String;
                }
            }
        }
        /// <summary>
        /// 查找转义字符串结束位置
        /// </summary>
        private void searchEscapeEnd()
        {
        NEXT:
            if (*++Current == 'u')
            {
                if ((int)((byte*)end - (byte*)Current) < 5 * sizeof(char))
                {
                    State = parseState.CrashEnd;
                    return;
                }
                Current += 5;
            }
            else if (*Current == 'x')
            {
                if ((int)((byte*)end - (byte*)Current) < 3 * sizeof(char))
                {
                    State = parseState.CrashEnd;
                    return;
                }
                Current += 3;
            }
            else
            {
                if (Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                ++Current;
            }
            if (Current == end)
            {
                State = parseState.CrashEnd;
                return;
            }
            if (endChar == quote)
            {
                do
                {
                    if (((bits[*(byte*)Current] & escapeSearchBit) | *(((byte*)Current) + 1)) == 0)
                    {
                        if (*Current == quote) return;
                        if (*Current == '\\') goto NEXT;
                        if (*Current == '\n')
                        {
                            State = parseState.StringEnter;
                            return;
                        }
                    }
                    ++Current;
                }
                while (true);
            }
            do
            {
                if (((bits[*(byte*)Current] & escapeSearchBit) | *(((byte*)Current) + 1)) == 0)
                {
                    if (*Current == quote) return;
                    if (*Current == '\\') goto NEXT;
                    if (*Current == '\n')
                    {
                        State = parseState.StringEnter;
                        return;
                    }
                }
                if (++Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
            }
            while (true);
        }
        /// <summary>
        /// 字符串转义解析
        /// </summary>
        /// <param name="value"></param>
        /// <param name="escapeIndex">未解析字符串起始位置</param>
        /// <param name="quote">字符串引号</param>
        /// <param name="isTempString"></param>
        private void parseQuoteString(ref subString value, int escapeIndex, char quote, int isTempString)
        {
            fixed (char* jsonFixed = value.value)
            {
                char* start = jsonFixed + value.StartIndex;
                end = start + value.Length;
                this.quote = quote;
                Current = start + escapeIndex;
                endChar = *end++;
                if (isTempString == 0)
                {
                    string newValue = parseEscape(start);
                    if (newValue != null)
                    {
                        value.UnsafeSet(newValue, 0, newValue.Length);
                        return;
                    }
                }
                else
                {
                    char* writeEnd = parseEscape();
                    if (writeEnd != null)
                    {
                        value.UnsafeSet((int)(start - jsonFixed), (int)(writeEnd - start));
                        return;
                    }
                }
            }
            value.Null();
        }
        /// <summary>
        /// 解析列表节点
        /// </summary>
        /// <returns></returns>
        private subArray<jsonNode> parseListNode()
        {
            if (++Current == end)
            {
                State = parseState.CrashEnd;
                return default(subArray<jsonNode>);
            }
            subArray<jsonNode> value = default(subArray<jsonNode>);
            if (isFirstArrayValue())
            {
                do
                {
                    jsonNode node = default(jsonNode);
                    parse(ref node);
                    if (State != parseState.Success) return default(subArray<jsonNode>);
                    value.Add(node);
                }
                while (isNextArrayValue());
            }
            return value;
        }
        /// <summary>
        /// 解析字典节点
        /// </summary>
        /// <returns></returns>
        private subArray<keyValue<jsonNode, jsonNode>> parseDictionaryNode()
        {
            if (++Current == end)
            {
                State = parseState.CrashEnd;
                return default(subArray<keyValue<jsonNode, jsonNode>>);
            }
            subArray<keyValue<jsonNode, jsonNode>> value = default(subArray<keyValue<jsonNode, jsonNode>>);
            if (isFirstObject())
            {
                do
                {
                    jsonNode name = default(jsonNode);
                    if (*Current == '"' || *Current == '\'') parseStringNode(ref name);
                    else
                    {
                        char* nameStart = Current;
                        searchNameEnd();
                        if (this.json == null) name.String = new string(nameStart, 0, (int)(Current - nameStart));
                        else name.String.UnsafeSet(this.json, (int)(nameStart - jsonFixed), (int)(Current - nameStart));
                        name.Type = jsonNode.type.String;
                    }
                    if (State != parseState.Success || searchColon() == 0) return default(subArray<keyValue<jsonNode, jsonNode>>);
                    jsonNode node = default(jsonNode);
                    parse(ref node);
                    if (State != parseState.Success) return default(subArray<keyValue<jsonNode, jsonNode>>);
                    value.Add(new keyValue<jsonNode, jsonNode>(name, node));
                }
                while (isNextObject());
            }
            return value;
        }
        /// <summary>
        /// 对象解析
        /// </summary>
        /// <param name="value">数据</param>
        [parseMethod]
        private void parse(ref object value)
        {
            if (isNull())
            {
                value = null;
                return;
            }
            space();
            if (State != parseState.Success) return;
            if (Current == end)
            {
                State = parseState.CrashEnd;
                return;
            }
            if (isNull())
            {
                value = null;
                return;
            }
            ignore();
            if (State == parseState.Success) value = new object();
        }
        /// <summary>
        /// 类型解析
        /// </summary>
        /// <param name="type">类型</param>
        [parseMethod]
        internal void Parse(ref Type type)
        {
            byte isSpace = 0;
        START:
            if (*Current == '{')
            {
                remoteType remoteType = default(remoteType);
                typeParser<remoteType>.Parse(this, ref remoteType);
                if (State != parseState.Success) return;
                if (!remoteType.TryGet(out type)) State = parseState.ErrorType;
                return;
            }
            if (*(long*)Current == 'n' + ('u' << 16) + ((long)'l' << 32) + ((long)'l' << 48) && (int)((byte*)end - (byte*)Current) >= 4 * sizeof(char))
            {
                type = null;
                Current += 4;
                return;
            }
            if (isSpace == 0)
            {
                space();
                if (State != parseState.Success) return;
                if (Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                isSpace = 1;
                goto START;
            }
            State = parseState.ErrorType;
        }
        /// <summary>
        /// 查找枚举引号并返回第一个字符
        /// </summary>
        /// <returns>第一个字符,0表示null</returns>
        private char searchEnumQuote()
        {
            byte isSpace = 0;
        START:
            if (*Current == '"' || *Current == '\'')
            {
                quote = *Current;
                return nextEnumChar();
            }
            if (*(long*)Current == 'n' + ('u' << 16) + ((long)'l' << 32) + ((long)'l' << 48) && (int)((byte*)end - (byte*)Current) >= 4 * sizeof(char))
            {
                Current += 4;
                return quote = (char)0;
            }
            if (isSpace == 0)
            {
                space();
                if (State != parseState.Success) return (char)0;
                if (++Current == end)
                {
                    State = parseState.CrashEnd;
                    return (char)0;
                }
                isSpace = 1;
                goto START;
            }
            State = parseState.NotEnumChar;
            return (char)0;
        }
        /// <summary>
        /// 获取下一个枚举字符
        /// </summary>
        /// <returns>下一个枚举字符,0表示null</returns>
        private char nextEnumChar()
        {
            if (++Current == end)
            {
                State = parseState.CrashEnd;
                return (char)0;
            }
            if (((bits[*(byte*)Current] & escapeSearchBit) | *(((byte*)Current) + 1)) == 0)
            {
                if (*Current == quote)
                {
                    ++Current;
                    return quote = (char)0;
                }
                if (*Current == '\\' || *Current == '\n')
                {
                    State = parseState.NotEnumChar;
                    return (char)0;
                }
            }
            return *Current;
        }
        /// <summary>
        /// 查找下一个枚举字符
        /// </summary>
        /// <returns>下一个枚举字符,0表示null</returns>
        private char searchNextEnum()
        {
            do
            {
                if (((bits[*(byte*)Current] & escapeSearchBit) | *(((byte*)Current) + 1)) == 0)
                {
                    if (*Current == quote)
                    {
                        ++Current;
                        return quote = (char)0;
                    }
                    if (*Current == '\\' || *Current == '\n')
                    {
                        State = parseState.NotEnumChar;
                        return (char)0;
                    }
                }
                else if (*Current == ',')
                {
                    do
                    {
                        if (++Current == end)
                        {
                            State = parseState.CrashEnd;
                            return (char)0;
                        }
                    }
                    while (*Current == ' ');
                    if (*Current == quote)
                    {
                        ++Current;
                        return quote = (char)0;
                    }
                    if (*Current == '\\' || *Current == '\n')
                    {
                        State = parseState.NotEnumChar;
                        return (char)0;
                    }
                    return *Current;
                }
                if (++Current == end)
                {
                    State = parseState.CrashEnd;
                    return (char)0;
                }
            }
            while (true);
        }
        /// <summary>
        /// 查找数组起始位置
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="value">目标数组</param>
        private void searchArray<valueType>(ref valueType[] value)
        {
            byte isSpace = 0;
        START:
            if (*Current == '[')
            {
                ++Current;
                if (value == null) value = nullValue<valueType>.Array;
                return;
            }
            if (*(long*)Current == 'n' + ('u' << 16) + ((long)'l' << 32) + ((long)'l' << 48) && (int)((byte*)end - (byte*)Current) >= 4 * sizeof(char))
            {
                value = null;
                Current += 4;
                return;
            }
            if (isSpace == 0)
            {
                space();
                if (State != parseState.Success) return;
                if (Current != end)
                {
                    isSpace = 1;
                    goto START;
                }
            }
            State = parseState.CrashEnd;
        }
        /// <summary>
        /// 是否存在下一个数组数据
        /// </summary>
        /// <returns>是否存在下一个数组数据</returns>
        private bool isFirstArrayValue()
        {
            if (*Current == ']')
            {
                ++Current;
                return false;
            }
            space();
            if (State != parseState.Success) return false;
            if (Current == end)
            {
                State = parseState.CrashEnd;
                return false;
            }
            if (*Current == ']')
            {
                ++Current;
                return false;
            }
            return true;
        }
        /// <summary>
        /// 是否存在下一个数组数据
        /// </summary>
        /// <returns>是否存在下一个数组数据</returns>
        private bool isNextArrayValue()
        {
            if (*Current == ',')
            {
                ++Current;
                return true;
            }
            if (*Current == ']')
            {
                ++Current;
                return false;
            }
            space();
            if (State != parseState.Success) return false;
            if (Current == end)
            {
                State = parseState.CrashEnd;
                return false;
            }
            if (*Current == ',')
            {
                ++Current;
                return true;
            }
            if (*Current == ']')
            {
                ++Current;
                return false;
            }
            State = parseState.NotArrayValue;
            return false;
        }
        /// <summary>
        /// 查找对象起始位置
        /// </summary>
        /// <returns>是否查找到</returns>
        private bool searchObject()
        {
            byte isSpace = 0;
        START:
            if (*Current == '{')
            {
                ++Current;
                return true;
            }
            if (*(long*)Current == 'n' + ('u' << 16) + ((long)'l' << 32) + ((long)'l' << 48) && (int)((byte*)end - (byte*)Current) >= 4 * sizeof(char))
            {
                Current += 4;
                return false;
            }
            if (isSpace == 0)
            {
                space();
                if (State != parseState.Success) return false;
                if (Current == end)
                {
                    State = parseState.CrashEnd;
                    return false;
                }
                isSpace = 1;
                goto START;
            }
            State = parseState.NotObject;
            return false;
        }
        /// <summary>
        /// 判断是否存在第一个成员
        /// </summary>
        /// <returns>是否存在第一个成员</returns>
        private bool isFirstObject()
        {
            if (((bits[*(byte*)Current] & nameStartBit) | *(((byte*)Current) + 1)) == 0) return true;
            if (*Current == '}')
            {
                ++Current;
                return false;
            }
            space();
            if (State != parseState.Success) return false;
            if (Current == end)
            {
                State = parseState.CrashEnd;
                return false;
            }
            if (((bits[*(byte*)Current] & nameStartBit) | *(((byte*)Current) + 1)) == 0) return true;
            if (*Current == '}')
            {
                ++Current;
                return false;
            }
            State = parseState.NotFoundName;
            return false;
        }
        /// <summary>
        /// 判断是否存在下一个成员
        /// </summary>
        /// <returns>是否存在下一个成员</returns>
        private bool isNextObject()
        {
            byte isSpace = 0;
        START:
            if (*Current == ',')
            {
                if (++Current == end)
                {
                    State = parseState.CrashEnd;
                    return false;
                }
                if (((bits[*(byte*)Current] & nameStartBit) | *(((byte*)Current) + 1)) == 0) return true;
                space();
                if (State != parseState.Success) return false;
                if (Current == end)
                {
                    State = parseState.CrashEnd;
                    return false;
                }
                if (((bits[*(byte*)Current] & nameStartBit) | *(((byte*)Current) + 1)) == 0) return true;
                State = parseState.NotFoundName;
                return false;
            }
            if (*Current == '}')
            {
                ++Current;
                return false;
            }
            if (isSpace == 0)
            {
                space();
                if (State != parseState.Success) return false;
                if (Current == end)
                {
                    State = parseState.CrashEnd;
                    return false;
                }
                isSpace = 1;
                goto START;
            }
            State = parseState.NotObject;
            return false;
        }
        /// <summary>
        /// 查找冒号
        /// </summary>
        /// <returns>是否找到</returns>
        private byte searchColon()
        {
            if (*Current == ':')
            {
                ++Current;
                return 1;
            }
            space();
            if (State != parseState.Success) return 0;
            if (Current == end)
            {
                State = parseState.CrashEnd;
                return 0;
            }
            if (*Current == ':')
            {
                ++Current;
                return 1;
            }
            State = parseState.NotFoundColon;
            return 0;
        }
        /// <summary>
        /// 是否匹配默认顺序名称
        /// </summary>
        /// <param name="names"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private byte* isName(byte* names, ref int index)
        {
            int length = *(short*)names;
            if (length == 0)
            {
                if (*Current == '}')
                {
                    index = -1;
                    ++Current;
                    return names;
                }
            }
            else if (fastCSharp.unsafer.memory.SimpleEqual((byte*)Current, names += sizeof(short), length) && (int)((byte*)end - (byte*)Current) >= length)
            {
                Current = (char*)((byte*)Current + length);
                return names + length;
            }
            return null;
        }
        /// <summary>
        /// 获取成员名称第一个字符
        /// </summary>
        /// <returns>第一个字符,0表示失败</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private char getFirstName()
        {
            if (*Current == '\'' || *Current == '"')
            {
                quote = *Current;
                return nextStringChar();
            }
            quote = (char)0;
            return *Current;
        }
        /// <summary>
        /// 获取成员名称下一个字符
        /// </summary>
        /// <returns>第一个字符,0表示失败</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private char getNextName()
        {
            if (++Current == end)
            {
                State = parseState.CrashEnd;
                return (char)0;
            }
            return ((bits[*(byte*)Current] & nameBit) | *(((byte*)Current) + 1)) == 0 ? *Current : (char)0;
        }
        /// <summary>
        /// 查找名称直到结束
        /// </summary>
        private void searchNameEnd()
        {
            if (State == parseState.Success)
            {
                while (((bits[*(byte*)Current] & nameBit) | *(((byte*)Current) + 1)) == 0)
                {
                    if (++Current == end)
                    {
                        State = parseState.CrashEnd;
                        return;
                    }
                }
                //while (++Current != end && ((bits[*(byte*)Current] & nameBit) | *(((byte*)Current) + 1)) == 0) ;
                //if (Current == end)
                //{
                //    State = parseState.CrashEnd;
                //    return;
                //}
            }
        }
        /// <summary>
        /// 忽略对象
        /// </summary>
        private void ignore()
        {
            space();
            if (State != parseState.Success) return;
            if (Current == end)
            {
                State = parseState.CrashEnd;
                return;
            }
            switch (*Current & 7)
            {
                case '"' & 7:
                case '\'' & 7:
                    if (*Current == '"' || *Current == '\'')
                    {
                        ignoreString();
                        return;
                    }
                    goto NUMBER;
                case '{' & 7:
                    if (*Current == '{')
                    {
                        ignoreObject();
                        return;
                    }
                    if (*Current == '[')
                    {
                        ignoreArray();
                        return;
                    }
                    goto NUMBER;
                case 't' & 7:
                    if ((int)((byte*)end - (byte*)Current) >= 4 * sizeof(char) && *(long*)(Current) == 't' + ('r' << 16) + ((long)'u' << 32) + ((long)'e' << 48))
                    {
                        Current += 4;
                        return;
                    }
                    goto NUMBER;
                case 'f' & 7:
                    if (*Current == 'f')
                    {
                        if ((int)((byte*)end - (byte*)Current) >= 5 * sizeof(char) && *(long*)(Current + 1) == 'a' + ('l' << 16) + ((long)'s' << 32) + ((long)'e' << 48))
                        {
                            Current += 5;
                            return;
                        }
                        break;
                    }
                    if ((int)((byte*)end - (byte*)Current) >= 4 * sizeof(char))
                    {
                        if (*(long*)Current == 'n' + ('u' << 16) + ((long)'l' << 32) + ((long)'l' << 48))
                        {
                            Current += 4;
                            return;
                        }
                        if ((int)((byte*)end - (byte*)Current) > 9 * sizeof(char) && ((*(long*)(Current + 1) ^ ('e' + ('w' << 16) + ((long)' ' << 32) + ((long)'D' << 48))) | (*(long*)(Current + 5) ^ ('a' + ('t' << 16) + ((long)'e' << 32) + ((long)'(' << 48)))) == 0)
                        {
                            Current += 9;
                            ignoreNumber();
                            if (State != parseState.Success) return;
                            if (Current == end)
                            {
                                State = parseState.CrashEnd;
                                return;
                            }
                            if (*Current == ')')
                            {
                                ++Current;
                                return;
                            }
                            break;
                        }
                    }
                    goto NUMBER;
                default:
                NUMBER:
                    ignoreNumber();
                    return;
            }
            State = parseState.UnknownValue;
        }
        /// <summary>
        /// 忽略字符串
        /// </summary>
        private void ignoreString()
        {
            quote = *Current;
            if (++Current == end)
            {
                State = parseState.CrashEnd;
                return;
            }
            //char* start = Current;
            if (searchEscape() == 0)
            {
                if (State == parseState.Success) ++Current;
                return;
            }
        NEXT:
            if (*++Current == 'u')
            {
                if ((int)((byte*)end - (byte*)Current) < 5 * sizeof(char))
                {
                    State = parseState.CrashEnd;
                    return;
                }
                Current += 5;
            }
            else if (*Current == 'x')
            {
                if ((int)((byte*)end - (byte*)Current) < 3 * sizeof(char))
                {
                    State = parseState.CrashEnd;
                    return;
                }
                Current += 3;
            }
            else
            {
                if (Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
                ++Current;
            }
            if (Current == end)
            {
                State = parseState.CrashEnd;
                return;
            }
            if (endChar == quote)
            {
                do
                {
                    if (((bits[*(byte*)Current] & escapeSearchBit) | *(((byte*)Current) + 1)) == 0)
                    {
                        if (*Current == quote)
                        {
                            ++Current;
                            return;
                        }
                        if (*Current == '\\') goto NEXT;
                        if (*Current == '\n')
                        {
                            State = parseState.StringEnter;
                            return;
                        }
                    }
                    ++Current;
                }
                while (true);
            }
            do
            {
                if (((bits[*(byte*)Current] & escapeSearchBit) | *(((byte*)Current) + 1)) == 0)
                {
                    if (*Current == quote)
                    {
                        ++Current;
                        return;
                    }
                    if (*Current == '\\') goto NEXT;
                    if (*Current == '\n')
                    {
                        State = parseState.StringEnter;
                        return;
                    }
                }
                if (++Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
            }
            while (true);
        }
        /// <summary>
        /// 忽略对象
        /// </summary>
        private void ignoreObject()
        {
            if (++Current == end)
            {
                State = parseState.CrashEnd;
                return;
            }
            if (isFirstObject())
            {
                if (*Current == '\'' || *Current == '"') ignoreString();
                else ignoreName();
                if (State != parseState.Success || searchColon() == 0) return;
                ignore();
                while (State == parseState.Success && isNextObject())
                {
                    if (*Current == '\'' || *Current == '"') ignoreString();
                    else ignoreName();
                    if (State != parseState.Success || searchColon() == 0) return;
                    ignore();
                }
            }
        }
        /// <summary>
        /// 忽略成员名称
        /// </summary>
        private void ignoreName()
        {
            do
            {
                if (++Current == end)
                {
                    State = parseState.CrashEnd;
                    return;
                }
            }
            while (((bits[*(byte*)Current] & nameBit) | *(((byte*)Current) + 1)) == 0);
        }
        /// <summary>
        /// 忽略数组
        /// </summary>
        private void ignoreArray()
        {
            if (++Current == end)
            {
                State = parseState.CrashEnd;
                return;
            }
            if (isFirstArrayValue())
            {
                do
                {
                    ignore();
                    if (State != parseState.Success) return;
                }
                while (isNextArrayValue());
            }
        }
        /// <summary>
        /// 忽略数字
        /// </summary>
        private void ignoreNumber()
        {
            if (((bits[*(byte*)Current] & numberBit) | *(((byte*)Current) + 1)) == 0)
            {
                while (++Current != end && ((bits[*(byte*)Current] & numberBit) | *(((byte*)Current) + 1)) == 0) ;
                return;
            }
            State = parseState.NotNumber;
        }
        /// <summary>
        /// 查找字典起始位置
        /// </summary>
        /// <returns>是否查找到</returns>
        private byte searchDictionary()
        {
            byte isSpace = 0;
        START:
            if (*Current == '{')
            {
                ++Current;
                return 1;
            }
            if (*Current == '[')
            {
                ++Current;
                return 2;
            }
            if (*(long*)Current == 'n' + ('u' << 16) + ((long)'l' << 32) + ((long)'l' << 48) && (int)((byte*)end - (byte*)Current) >= 4 * sizeof(char))
            {
                Current += 4;
                return 0;
            }
            if (isSpace == 0)
            {
                space();
                if (State != parseState.Success) return 0;
                if (Current == end)
                {
                    State = parseState.CrashEnd;
                    return 0;
                }
                isSpace = 1;
                goto START;
            }
            State = parseState.NotObject;
            return 0;
        }
        /// <summary>
        /// 对象是否结束
        /// </summary>
        /// <returns>对象是否结束</returns>
        private byte isDictionaryObjectEnd()
        {
            if (*Current == '}')
            {
                ++Current;
                return 1;
            }
            space();
            if (State != parseState.Success) return 1;
            if (Current == end)
            {
                State = parseState.CrashEnd;
                return 1;
            }
            if (*Current == '}')
            {
                ++Current;
                return 1;
            }
            return 0;
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
        /// 转换成字符串解析
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void EndToSubString(ref subString value)
        {
            if (this.json == null) value = new string(Current, 0, (int)(end - Current));
            else value.UnsafeSet(this.json, (int)(Current - jsonFixed), (int)(end - Current));
            Current = end;
        }

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
        private static readonly MethodInfo typeParseMethod = typeof(jsonParser).GetMethod("typeParse", BindingFlags.Instance | BindingFlags.NonPublic);
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
        private static readonly MethodInfo structParseMethod = typeof(jsonParser).GetMethod("structParse", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 值类型对象解析
        /// </summary>
        /// <param name="value">目标数据</param>
        private void nullableParse<valueType>(ref Nullable<valueType> value) where valueType : struct
        {
            if (tryNull()) value = null;
            else if (State == parseState.Success)
            {
                valueType newValue = value.HasValue ? value.Value : default(valueType);
                typeParser<valueType>.Parse(this, ref newValue);
                value = newValue;
            }
        }
        /// <summary>
        /// 集合构造解析函数信息
        /// </summary>
        private static readonly MethodInfo nullableParseMethod = typeof(jsonParser).GetMethod("nullableParse", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 是否null
        /// </summary>
        /// <returns>是否null</returns>
        private bool tryNull()
        {
            if (isNull()) return true;
            space();
            return isNull();
        }
        /// <summary>
        /// 值类型对象解析
        /// </summary>
        /// <param name="value">目标数据</param>
        private void nullableEnumParse<valueType>(ref Nullable<valueType> value) where valueType : struct
        {
            if (tryNull()) value = null;
            else
            {
                valueType newValue = value.HasValue ? value.Value : default(valueType);
                typeParser<valueType>.DefaultParser(this, ref newValue);
                value = newValue;
            }
        }
        /// <summary>
        /// 集合构造解析函数信息
        /// </summary>
        private static readonly MethodInfo nullableEnumParseMethod = typeof(jsonParser).GetMethod("nullableEnumParse", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 值类型对象解析
        /// </summary>
        /// <param name="value">目标数据</param>
        private void keyValuePairParse<keyType, valueType>(ref KeyValuePair<keyType, valueType> value)
        {
            if (searchObject())
            {
                keyValue<keyType, valueType> keyValue = new keyValue<keyType,valueType>(value.Key, value.Value);
                typeParser<keyValue<keyType, valueType>>.ParseMembers(this, ref keyValue);
                value = new KeyValuePair<keyType, valueType>(keyValue.Key, keyValue.Value);
            }
            else value = new KeyValuePair<keyType, valueType>();
        }
        /// <summary>
        /// 值类型对象解析函数信息
        /// </summary>
        private static readonly MethodInfo keyValuePairParseMethod = typeof(jsonParser).GetMethod("keyValuePairParse", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 基类转换
        /// </summary>
        /// <param name="value">目标数据</param>
        private void baseParse<valueType, childType>(ref childType value) where childType : valueType
        {
            if (value == null)
            {
                if (searchObject())
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
        private static readonly MethodInfo baseParseMethod = typeof(jsonParser).GetMethod("baseParse", BindingFlags.Instance | BindingFlags.NonPublic);
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
                --Current;
                ignoreObject();
                return;
            }
            object newValue = constructor(typeof(valueType));
            if (newValue == null)
            {
                --Current;
                ignoreObject();
                return;
            }
            value = (valueType)newValue;
        }
        ///// <summary>
        ///// 找不到构造函数
        ///// </summary>
        ///// <param name="value">目标数据</param>
        //private void checkNoConstructor<valueType>(ref valueType value)
        //{
        //    if (value == null)
        //    {
        //        Func<Type, object> constructor = Config.Constructor;
        //        if (constructor == null)
        //        {
        //            state = parseState.NoConstructor;
        //            return;
        //        }
        //        object newValue = constructor(typeof(valueType));
        //        if (newValue == null)
        //        {
        //            state = parseState.NoConstructor;
        //            return;
        //        }
        //        value = (valueType)newValue;
        //    }
        //    typeParser<valueType>.ParseClassNew(this, ref value);
        //}
        ///// <summary>
        ///// 找不到构造函数解析函数信息
        ///// </summary>
        //private static readonly MethodInfo checkNoConstructorMethod = typeof(jsonParser).GetMethod("checkNoConstructor", BindingFlags.Instance | BindingFlags.NonPublic);
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
        /// 枚举值解析函数信息
        /// </summary>
        private static readonly MethodInfo enumByteMethod = typeof(jsonParser).GetMethod("enumByte", BindingFlags.Instance | BindingFlags.NonPublic);
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
        private static readonly MethodInfo enumSByteMethod = typeof(jsonParser).GetMethod("enumSByte", BindingFlags.Instance | BindingFlags.NonPublic);
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
        private static readonly MethodInfo enumShortMethod = typeof(jsonParser).GetMethod("enumShort", BindingFlags.Instance | BindingFlags.NonPublic);
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
        private static readonly MethodInfo enumUShortMethod = typeof(jsonParser).GetMethod("enumUShort", BindingFlags.Instance | BindingFlags.NonPublic);
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
        private static readonly MethodInfo enumIntMethod = typeof(jsonParser).GetMethod("enumInt", BindingFlags.Instance | BindingFlags.NonPublic);
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
        private static readonly MethodInfo enumUIntMethod = typeof(jsonParser).GetMethod("enumUInt", BindingFlags.Instance | BindingFlags.NonPublic);
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
        private static readonly MethodInfo enumLongMethod = typeof(jsonParser).GetMethod("enumLong", BindingFlags.Instance | BindingFlags.NonPublic);
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
        private static readonly MethodInfo enumULongMethod = typeof(jsonParser).GetMethod("enumULong", BindingFlags.Instance | BindingFlags.NonPublic);
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
        private static readonly MethodInfo enumByteFlagsMethod = typeof(jsonParser).GetMethod("enumByteFlags", BindingFlags.Instance | BindingFlags.NonPublic);
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
        private static readonly MethodInfo enumSByteFlagsMethod = typeof(jsonParser).GetMethod("enumSByteFlags", BindingFlags.Instance | BindingFlags.NonPublic);
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
        private static readonly MethodInfo enumShortFlagsMethod = typeof(jsonParser).GetMethod("enumShortFlags", BindingFlags.Instance | BindingFlags.NonPublic);
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
        private static readonly MethodInfo enumUShortFlagsMethod = typeof(jsonParser).GetMethod("enumUShortFlags", BindingFlags.Instance | BindingFlags.NonPublic);
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
        private static readonly MethodInfo enumIntFlagsMethod = typeof(jsonParser).GetMethod("enumIntFlags", BindingFlags.Instance | BindingFlags.NonPublic);
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
        private static readonly MethodInfo enumUIntFlagsMethod = typeof(jsonParser).GetMethod("enumUIntFlags", BindingFlags.Instance | BindingFlags.NonPublic);
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
        private static readonly MethodInfo enumLongFlagsMethod = typeof(jsonParser).GetMethod("enumLongFlags", BindingFlags.Instance | BindingFlags.NonPublic);
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
        private static readonly MethodInfo enumULongFlagsMethod = typeof(jsonParser).GetMethod("enumULongFlags", BindingFlags.Instance | BindingFlags.NonPublic);
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
        private static readonly MethodInfo arrayMethod = typeof(jsonParser).GetMethod("array", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 字典解析
        /// </summary>
        /// <param name="dictionary">目标数据</param>
        private void dictionary<valueType, dictionaryValueType>(ref Dictionary<valueType, dictionaryValueType> dictionary)
        {
            byte type = searchDictionary();
            if (type == 0) dictionary = null;
            else
            {
                dictionary = fastCSharp.dictionary.CreateAny<valueType, dictionaryValueType>();
                if (type == 1)
                {
                    if (isDictionaryObjectEnd() == 0)
                    {
                        valueType key = default(valueType);
                        dictionaryValueType value = default(dictionaryValueType);
                        do
                        {
                            typeParser<valueType>.Parse(this, ref key);
                            if (State != parseState.Success || searchColon() == 0) return;
                            typeParser<dictionaryValueType>.Parse(this, ref value);
                            if (State != parseState.Success) return;
                            dictionary.Add(key, value);
                        }
                        while (isNextObject());
                    }
                }
                else if (isFirstArrayValue())
                {
                    keyValue<valueType, dictionaryValueType> value = default(keyValue<valueType, dictionaryValueType>);
                    do
                    {
                        typeParser<keyValue<valueType, dictionaryValueType>>.ParseValue(this, ref value);
                        if (State != parseState.Success) return;
                        dictionary.Add(value.Key, value.Value);
                    }
                    while (isNextArrayValue());
                }
            }
        }
        /// <summary>
        /// 字典解析函数信息
        /// </summary>
        private static readonly MethodInfo dictionaryMethod = typeof(jsonParser).GetMethod("dictionary", BindingFlags.Instance | BindingFlags.NonPublic);
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
        private static readonly MethodInfo dictionaryConstructorMethod = typeof(jsonParser).GetMethod("dictionaryConstructor", BindingFlags.Instance | BindingFlags.NonPublic);
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
        private static readonly MethodInfo listConstructorMethod = typeof(jsonParser).GetMethod("listConstructor", BindingFlags.Instance | BindingFlags.NonPublic);
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
        private static readonly MethodInfo collectionConstructorMethod = typeof(jsonParser).GetMethod("collectionConstructor", BindingFlags.Instance | BindingFlags.NonPublic);
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
        private static readonly MethodInfo enumerableConstructorMethod = typeof(jsonParser).GetMethod("enumerableConstructor", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 集合构造函数解析
        /// </summary>
        /// <param name="value">目标数据</param>
        private void arrayConstructor<valueType, argumentType>(ref valueType value)
        {
            argumentType[] values = null;
            typeParser<argumentType>.Array(this, ref values);
            if (State == parseState.Success)
            {
                if (values == null) value = default(valueType);
                else value = pub.arrayConstructor<valueType, argumentType>.Constructor(values);
            }
        }
        /// <summary>
        /// 数组构造解析函数信息
        /// </summary>
        private static readonly MethodInfo arrayConstructorMethod = typeof(jsonParser).GetMethod("arrayConstructor", BindingFlags.Instance | BindingFlags.NonPublic);
        
        /// <summary>
        /// 公共默认配置参数
        /// </summary>
        private static readonly config defaultConfig = new config();
        /// <summary>
        /// Json解析
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="json">Json字符串</param>
        /// <param name="config">配置参数</param>
        /// <returns>目标数据</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType Parse<valueType>(subString json, config config = null)
        {
            return Parse<valueType>(ref json, config);
        }
        /// <summary>
        /// Json解析
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="json">Json字符串</param>
        /// <param name="config">配置参数</param>
        /// <returns>目标数据</returns>
        public static valueType Parse<valueType>(ref subString json, config config = null)
        {
            if (json.Length == 0)
            {
                if (config != null) config.State = parseState.NullJson;
                return default(valueType);
            }
            valueType value = default(valueType);
            jsonParser parser = typePool<jsonParser>.Pop() ?? new jsonParser();
            try
            {
                return parser.parse<valueType>(ref json, ref value, config ?? defaultConfig) == parseState.Success ? value : default(valueType);
            }
            finally { parser.free(); }
        }
        /// <summary>
        /// Json解析
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="jsonString">Json字符串</param>
        /// <returns>目标数据</returns>
        private static valueType sqlParse<valueType>(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString)) return default(valueType);
            valueType value = default(valueType);
            jsonParser parser = typePool<jsonParser>.Pop() ?? new jsonParser();
            subString json = jsonString;
            try
            {
                return parser.parse<valueType>(ref json, ref value, defaultConfig) == parseState.Success ? value : default(valueType);
            }
            finally { parser.free(); }
        }
        /// <summary>
        /// Json解析函数信息
        /// </summary>
        internal static readonly MethodInfo SqlParseMethod = typeof(fastCSharp.emit.jsonParser).GetMethod("sqlParse", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(string) }, null);
        /// <summary>
        /// Json解析
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="json">Json字符串</param>
        /// <param name="value">目标数据</param>
        /// <param name="config">配置参数</param>
        /// <returns>是否解析成功</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static bool Parse<valueType>(subString json, ref valueType value, config config = null)
        {
            return Parse(ref json, ref value, config);
        }
        /// <summary>
        /// Json解析
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="json">Json字符串</param>
        /// <param name="value">目标数据</param>
        /// <param name="config">配置参数</param>
        /// <returns>是否解析成功</returns>
        public static bool Parse<valueType>(ref subString json, ref valueType value, config config = null)
        {
            if (json.Length == 0)
            {
                if (config != null) config.State = parseState.NullJson;
                return false;
            }
            jsonParser parser = typePool<jsonParser>.Pop() ?? new jsonParser();
            try
            {
                return parser.parse<valueType>(ref json, ref value, config ?? defaultConfig) == parseState.Success;
            }
            finally { parser.free(); }
        }
        /// <summary>
        /// Json解析
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="json">Json字符串</param>
        /// <param name="length">Json长度</param>
        /// <param name="value">目标数据</param>
        /// <param name="config">配置参数</param>
        /// <param name="buffer">二进制缓冲区</param>
        /// <returns>是否解析成功</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static bool UnsafeParse<valueType>(char* json, int length, ref valueType value, config config = null, byte[] buffer = null)
        {
            jsonParser parser = typePool<jsonParser>.Pop() ?? new jsonParser();
            try
            {
                return parser.parse<valueType>(json, length, ref value, config ?? defaultConfig, buffer) == parseState.Success;
            }
            finally { parser.free(); }
        }
        /// <summary>
        /// Json解析
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="json">Json字符串</param>
        /// <param name="config">配置参数</param>
        /// <returns>目标数据</returns>
        private static object parseType<valueType>(subString json, config config)
        {
            return Parse<valueType>(ref json, config);
        }
        /// <summary>
        /// Json解析
        /// </summary>
        /// <param name="type">目标数据类型</param>
        /// <param name="json">Json字符串</param>
        /// <param name="config">配置参数</param>
        /// <returns>目标数据</returns>
        public static object ParseType(Type type, subString json, config config = null)
        {
            if (type == null) log.Error.Throw(log.exceptionType.Null);
            Func<subString, config, object> parse;
            if (!parseTypes.TryGetValue(type, out parse))
            {
                parse = (Func<subString, config, object>)Delegate.CreateDelegate(typeof(Func<subString, config, object>), parseTypeMethod.MakeGenericMethod(type));
                parseTypes.Set(type, parse);
            }
            return parse(json, config);
        }
        /// <summary>
        /// Json解析
        /// </summary>
        private static readonly interlocked.dictionary<Type, Func<subString, config, object>> parseTypes = new interlocked.dictionary<Type, Func<subString, config, object>>();
        /// <summary>
        /// Json解析函数信息
        /// </summary>
        private static readonly MethodInfo parseTypeMethod = typeof(jsonParser).GetMethod("parseType", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(subString), typeof(config) }, null);
        /// <summary>
        /// 字符串转义解析
        /// </summary>
        /// <param name="value"></param>
        /// <param name="escapeIndex">未解析字符串起始位置</param>
        /// <param name="quote">字符串引号</param>
        /// <param name="isTempString"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static void ParseQuoteString(ref subString value, int escapeIndex, char quote, int isTempString)
        {
            jsonParser parser = typePool<jsonParser>.Pop() ?? new jsonParser();
            try
            {
                parser.parseQuoteString(ref value, escapeIndex, quote, isTempString);
            }
            finally { parser.free(); }
        }
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
        /// Json解析空格[ ,\t,\r,\n,160]
        /// </summary>
        private const byte spaceBit = 128;
        /// <summary>
        /// Json解析键值开始
        /// </summary>
        private const byte nameStartBit = 64;
        /// <summary>
        /// Json解析键值
        /// </summary>
        private const byte nameBit = 32;
        /// <summary>
        /// Json解析数字
        /// </summary>
        private const byte numberBit = 16;
        /// <summary>
        /// Json解析转义查找
        /// </summary>
        private const byte escapeSearchBit = 8;
        /// <summary>
        /// Javascript转义位[\r,\n,\\,"]
        /// </summary>
        internal const byte EscapeBit = 4;
        /// <summary>
        /// 字符状态位
        /// </summary>
        internal static readonly pointer.reference Bits;
        /// <summary>
        /// 转义字符集合尺寸
        /// </summary>
        private const int escapeCharSize = 128;
        /// <summary>
        /// 转义字符集合
        /// </summary>
        private static readonly pointer.reference escapeCharData;
        static jsonParser()
        {
            pointer[] pointers = unmanaged.GetStatic(false, 256, escapeCharSize * sizeof(char));
            byte* bits = (Bits = pointers[0].Reference).Byte;
            fastCSharp.unsafer.memory.Fill(bits, ulong.MaxValue, 256 >> 3);
            for (char value = '0'; value <= '9'; ++value) bits[value] &= (numberBit | nameBit) ^ 255;
            for (char value = 'A'; value <= 'F'; ++value) bits[value] &= (nameBit | nameStartBit | numberBit) ^ 255;
            for (char value = 'a'; value <= 'f'; ++value) bits[value] &= (nameBit | nameStartBit | numberBit) ^ 255;
            for (char value = 'G'; value <= 'Z'; ++value) bits[value] &= (nameBit | nameStartBit) ^ 255;
            for (char value = 'g'; value <= 'z'; ++value) bits[value] &= (nameBit | nameStartBit) ^ 255;
            bits['\t'] &= spaceBit ^ 255;
            bits['\r'] &= spaceBit ^ 255;
            bits['\n'] &= (spaceBit | escapeSearchBit) ^ 255;
            bits[' '] &= spaceBit ^ 255;
            bits[0xA0] &= spaceBit ^ 255;
            bits['x'] &= numberBit ^ 255;
            bits['+'] &= numberBit ^ 255;
            bits['-'] &= numberBit ^ 255;
            bits['.'] &= numberBit ^ 255;
            bits['_'] &= (nameBit | nameStartBit) ^ 255;
            bits['\''] &= (nameStartBit | escapeSearchBit) ^ 255;
            bits['"'] &= (nameStartBit | escapeSearchBit) ^ 255;
            bits['\\'] &= escapeSearchBit ^ 255;

            escapeCharData = pointers[1].Reference;
            char* escapeCharDataChar = escapeCharData.Char;
            for (int value = 0; value != escapeCharSize; ++value) escapeCharDataChar[value] = (char)value;
            escapeCharDataChar['0'] = ' ';
            escapeCharDataChar['B'] = escapeCharDataChar['b'] = '\b';
            escapeCharDataChar['F'] = escapeCharDataChar['f'] = '\f';
            escapeCharDataChar['N'] = escapeCharDataChar['n'] = '\n';
            escapeCharDataChar['R'] = escapeCharDataChar['r'] = '\r';
            escapeCharDataChar['T'] = escapeCharDataChar['t'] = '\t';
            escapeCharDataChar['V'] = escapeCharDataChar['v'] = '\v';

            parseMethods = fastCSharp.dictionary.CreateOnly<Type, MethodInfo>();
            foreach (MethodInfo method in typeof(jsonParser).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (method.customAttribute<parseMethod>() != null)
                {
                    parseMethods.Add(method.GetParameters()[0].ParameterType.GetElementType(), method);
                }
            }
        }
    }
}
