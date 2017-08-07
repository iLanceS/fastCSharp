using System;
using System.Collections.Generic;
using System.Collections;
using fastCSharp.code.cSharp;
using fastCSharp.threading;
using fastCSharp.web;
using System.Text;
using System.Reflection;
using fastCSharp.reflection;
using System.Runtime.CompilerServices;
#if NOJIT
#else
using System.Reflection.Emit;
#endif
namespace fastCSharp.net.tcp.http
{
    /// <summary>
    /// HTTP请求头部
    /// </summary>
    public sealed class requestHeader
    {
        /// <summary>
        /// fastCSharp爬虫标识
        /// </summary>
        public const string fastCSharpSpiderUserAgent = "fastCSharp spider";
        /// <summary>
        /// 最大数据分隔符长度
        /// </summary>
        private const int maxBoundaryLength = 128;
        /// <summary>
        /// 提交数据类型
        /// </summary>
        public enum postType : byte
        {
            /// <summary>
            /// 
            /// </summary>
            None,
            /// <summary>
            /// JSON数据
            /// </summary>
            Json,
            /// <summary>
            /// 表单
            /// </summary>
            Form,
            /// <summary>
            /// 表单数据boundary
            /// </summary>
            FormData,
            /// <summary>
            /// XML数据
            /// </summary>
            Xml,
            /// <summary>
            /// 未知数据流
            /// </summary>
            Data,
        }
        /// <summary>
        /// HTTP头名称唯一哈希
        /// </summary>
        private struct headerName : IEquatable<headerName>
        {
            //string[] keys = new string[] { "host", "content-length", "accept-encoding", "connection", "content-type", "cookie", "referer", "range", "user-agent", "if-modified-since", "x-prowarded-for", "if-none-match", "expect", "upgrade", "origin", "sec-webSocket-key", "sec-webSocket-origin" };
            /// <summary>
            /// HTTP头名称
            /// </summary>
            public subArray<byte> Name;
            /// <summary>
            /// 隐式转换
            /// </summary>
            /// <param name="name">HTTP头名称</param>
            /// <returns>HTTP头名称唯一哈希</returns>
            public static implicit operator headerName(byte[] name) { return new headerName { Name = subArray<byte>.Unsafe(name, 0, name.Length) }; }
            /// <summary>
            /// 隐式转换
            /// </summary>
            /// <param name="name">HTTP头名称</param>
            /// <returns>HTTP头名称唯一哈希</returns>
            public static implicit operator headerName(subArray<byte> name) { return new headerName { Name = name }; }
            /// <summary>
            /// 获取哈希值
            /// </summary>
            /// <returns>哈希值</returns>
            public unsafe override int GetHashCode()
            {
                fixed (byte* nameFixed = Name.array)
                {
                    byte* start = nameFixed + Name.startIndex;
                    return (((*(start + (Name.length >> 1)) | 0x20) >> 2) ^ (*(start + Name.length - 3) << 1)) & ((1 << 5) - 1);
                }
            }
            /// <summary>
            /// 判断是否相等
            /// </summary>
            /// <param name="other">待匹配数据</param>
            /// <returns>是否相等</returns>
            public unsafe bool Equals(headerName other)
            {
                if (Name.length == other.Name.length)
                {
                    fixed (byte* nameFixed = Name.array, otherNameFixed = other.Name.array)
                    {
                        return fastCSharp.unsafer.memory.EqualCase(nameFixed + Name.startIndex, otherNameFixed + other.Name.startIndex, Name.length);
                    }
                }
                return false;
            }
            /// <summary>
            /// 判断是否相等
            /// </summary>
            /// <param name="obj">待匹配数据</param>
            /// <returns>是否相等</returns>
            public override bool Equals(object obj)
            {
                return Equals((headerName)obj);
            }
        }
        /// <summary>
        /// 查询解析器
        /// </summary>
        private unsafe sealed class queryParser
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
                /// 时间解析错误
                /// </summary>
                NotDateTime,
                /// <summary>
                /// Guid解析错误
                /// </summary>
                NotGuid,
                /// <summary>
                /// 未知类型解析错误
                /// </summary>
                Unknown,
            }
            /// <summary>
            /// 解析类型
            /// </summary>
            private sealed class parseType : Attribute { }
            /// <summary>
            /// 名称状态查找器
            /// </summary>
            internal struct stateSearcher
            {
                /// <summary>
                /// 查询解析器
                /// </summary>
                private queryParser parser;
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
                ///// <summary>
                ///// 特殊字符串查找表结束位置
                ///// </summary>
                //private byte* charEnd;
                /// <summary>
                /// 当前状态
                /// </summary>
                private byte* currentState;
                ///// <summary>
                ///// 特殊字符起始值
                ///// </summary>
                //private int charIndex;
                /// <summary>
                /// 查询矩阵单位尺寸类型
                /// </summary>
                private byte tableType;
                /// <summary>
                /// 名称查找器
                /// </summary>
                /// <param name="parser">查询解析器</param>
                /// <param name="data">数据起始位置</param>
                internal stateSearcher(queryParser parser, pointer.reference data)
                {
                    this.parser = parser;
                    if (data.Data == null)
                    {
                        state = charsAscii = charStart = currentState = null;//charEnd = 
                        //charIndex = 0;
                        tableType = 0;
                    }
                    else
                    {
                        int stateCount = *data.Int;
                        currentState = state = data.Byte + sizeof(int);
                        charsAscii = state + stateCount * 3 * sizeof(int);
                        charStart = charsAscii + 128 * sizeof(ushort);
                        //charIndex = *(ushort*)charStart;
                        charStart += sizeof(ushort) * 2;
                        //charEnd = charStart + *(ushort*)(charStart - sizeof(ushort)) * sizeof(ushort);
                        if (stateCount < 256) tableType = 0;
                        else if (stateCount < 65536) tableType = 1;
                        else tableType = 2;
                    }
                }
                /// <summary>
                /// 获取名称索引
                /// </summary>
                /// <returns>名称索引,失败返回-1</returns>
                internal int SearchName()
                {
                    if (state == null) return -1;
                    byte value = parser.getName();
                    if (value == 0) return *(int*)(currentState + sizeof(int) * 2);
                    currentState = state;
                    do
                    {
                        char* prefix = (char*)(currentState + *(int*)currentState);
                        if (*prefix != 0)
                        {
                            if (value != *prefix) return -1;
                            while (*++prefix != 0)
                            {
                                if (parser.getName() != *prefix) return -1;
                            }
                            value = parser.getName();
                        }
                        if (value == 0) return *(int*)(currentState + sizeof(int) * 2);
                        if (*(int*)(currentState + sizeof(int)) == 0 || value >= 128) return -1;
                        int index = (int)*(ushort*)(charsAscii + (value << 1));
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
                        value = parser.getName();
                    }
                    while (true);
                }
            }
            /// <summary>
            /// 类型解析器
            /// </summary>
            /// <typeparam name="valueType">目标类型</typeparam>
            internal static class parser<valueType>
            {
                /// <summary>
                /// 解析委托
                /// </summary>
                /// <param name="parser">查询解析器</param>
                /// <param name="value">目标数据</param>
                internal delegate void tryParse(queryParser parser, ref valueType value);
                /// <summary>
                /// 成员解析器集合
                /// </summary>
                private static readonly tryParse[] memberParsers;
                /// <summary>
                /// 成员名称查找数据
                /// </summary>
                private static readonly pointer.reference memberSearcher;
                /// <summary>
                /// 默认顺序成员名称数据
                /// </summary>
                private static readonly pointer.reference memberNames;
                /// <summary>
                /// 对象解析
                /// </summary>
                /// <param name="parser">查询解析器</param>
                /// <param name="value">目标数据</param>
                internal static void Parse(queryParser parser, ref valueType value)
                {
#if NOJIT
                    object[] parameter = null;
#endif
                    byte* names = memberNames.Byte;
                    int index = 0;
                    while ((names = parser.isName(names, ref index)) != null)
                    {
                        if (index == -1) return;
#if NOJIT
                        parser.ReflectionParameter = parameter ?? (parameter = new object[1]);
#endif
                        memberParsers[index](parser, ref value);
                        ++index;
                    }
                    stateSearcher searcher = new stateSearcher(parser, memberSearcher);
                    do
                    {
                        if ((index = searcher.SearchName()) != -1)
                        {
#if NOJIT
                            parser.ReflectionParameter = parameter ?? (parameter = new object[1]);
#endif
                            memberParsers[index](parser, ref value);
                        }
                    }
                    while (parser.isQuery());
                }
                static parser()
                {
                    Type type = typeof(valueType);
                    fastCSharp.emit.jsonParse attribute = fastCSharp.code.typeAttribute.GetAttribute<fastCSharp.emit.jsonParse>(type, true, true) ?? fastCSharp.emit.jsonParse.AllMember;
                    fastCSharp.code.fieldIndex defaultMember = null;
                    subArray<fastCSharp.code.fieldIndex> fields = fastCSharp.emit.jsonParser.staticTypeParser.GetFields(fastCSharp.code.memberIndexGroup<valueType>.GetFields(attribute.MemberFilter), attribute, ref defaultMember);
                    subArray<keyValue<fastCSharp.code.propertyIndex, MethodInfo>> properties = fastCSharp.emit.jsonParser.staticTypeParser.GetProperties(fastCSharp.code.memberIndexGroup<valueType>.GetProperties(attribute.MemberFilter), attribute);
                    memberParsers = new tryParse[fields.length + properties.length + (defaultMember == null ? 0 : 1)];
                    string[] names = new string[memberParsers.Length];
                    int index = 0, nameLength = 0, maxNameLength = 0;
                    foreach (fastCSharp.code.fieldIndex member in fields)
                    {
#if NOJIT
                        tryParse tryParse = new fieldParser(member.Member).Parser();
#else
                        ILGenerator generator;
                        DynamicMethod dynamicMethod = createDynamicMethod(type, member.Member.Name, member.Member.FieldType, out generator);
                        generator.Emit(OpCodes.Stfld, member.Member);
                        generator.Emit(OpCodes.Ret);
                        tryParse tryParse = (tryParse)dynamicMethod.CreateDelegate(typeof(tryParse));
#endif
                        memberParsers[index] = tryParse;
                        if (member.Member.Name.Length > maxNameLength) maxNameLength = member.Member.Name.Length;
                        nameLength += (names[index++] = member.Member.Name).Length;
                        if (member == defaultMember)
                        {
                            memberParsers[names.Length - 1] = tryParse;
                            names[names.Length - 1] = string.Empty;
                        }
                    }
                    foreach (keyValue<fastCSharp.code.propertyIndex, MethodInfo> member in properties)
                    {
#if NOJIT
                        memberParsers[index] = new propertyParser(member.Key.Member).Parser();
#else
                        ILGenerator generator;
                        DynamicMethod dynamicMethod = createDynamicMethod(type, member.Key.Member.Name, member.Key.Member.PropertyType, out generator);
                        generator.Emit(member.Value.IsFinal || !member.Value.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, member.Value);
                        generator.Emit(OpCodes.Ret);
                        memberParsers[index] = (tryParse)dynamicMethod.CreateDelegate(typeof(tryParse));
#endif
                        if (member.Key.Member.Name.Length > maxNameLength) maxNameLength = member.Key.Member.Name.Length;
                        nameLength += (names[index++] = member.Key.Member.Name).Length;
                    }
                    if (maxNameLength > short.MaxValue || nameLength == 0) memberNames = unmanaged.NullByte8;
                    else
                    {
                        memberNames = unmanaged.GetStatic(nameLength + (names.Length - (defaultMember == null ? 0 : 1)) * sizeof(short) + sizeof(short), false).Reference;
                        byte* write = memberNames.Byte;
                        foreach (string name in names)
                        {
                            if (name.Length != 0)
                            {
                                *(short*)write = (short)name.Length;
                                fixed (char* nameFixed = name) fastCSharp.unsafer.String.WriteBytes(nameFixed, name.Length, write + sizeof(short));
                                write += sizeof(short) + name.Length;
                            }
                        }
                        *(short*)write = 0;
                    }
                    memberSearcher = fastCSharp.emit.jsonParser.stateSearcher.GetMemberSearcher(type, names);
                }
#if NOJIT
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
                    private MethodInfo parseMethod;
                    /// <summary>
                    /// 字段信息
                    /// </summary>
                    /// <param name="field">字段信息</param>
                    public fieldParser(FieldInfo field)
                    {
                        this.field = field;
                        parseMethod = getParseMemberMethod(field.FieldType);
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
                    /// <param name="parser">查询解析器</param>
                    /// <param name="value">目标数据</param>
                    private void parse(queryParser parser, ref valueType value)
                    {
                        object[] parameter = parser.ReflectionParameter;
                        parameter[0] = field.GetValue(value);
                        parseMethod.Invoke(parser, parameter);
                        field.SetValue(value, parameter[0]);
                    }
                    /// <summary>
                    /// 解析委托
                    /// </summary>
                    /// <param name="parser">查询解析器</param>
                    /// <param name="value">目标数据</param>
                    private void parseValue(queryParser parser, ref valueType value)
                    {
                        object[] parameter = parser.ReflectionParameter;
                        object objectValue = value;
                        parameter[0] = field.GetValue(objectValue);
                        parseMethod.Invoke(parser, parameter);
                        field.SetValue(objectValue, parameter[0]);
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
                    private MethodInfo parseMethod;
                    /// <summary>
                    /// 属性信息
                    /// </summary>
                    /// <param name="property">字段信息</param>
                    public propertyParser(PropertyInfo property)
                    {
                        if (property.CanRead) getMethod = property.GetGetMethod(true);
                        setMethod = property.GetSetMethod(true);
                        parseMethod = getParseMemberMethod(property.PropertyType);
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
                    /// <param name="parser">查询解析器</param>
                    /// <param name="value">目标数据</param>
                    private void parse(queryParser parser, ref valueType value)
                    {
                        object[] parameter = parser.ReflectionParameter;
                        parameter[0] = getMethod == null ? null : getMethod.Invoke(value, null);
                        parseMethod.Invoke(parser, parameter);
                        setMethod.Invoke(value, parameter);
                    }
                    /// <summary>
                    /// 解析委托
                    /// </summary>
                    /// <param name="parser">查询解析器</param>
                    /// <param name="value">目标数据</param>
                    private void parseValue(queryParser parser, ref valueType value)
                    {
                        object[] parameter = parser.ReflectionParameter;
                        object objectValue = value;
                        parameter[0] = getMethod == null ? null : getMethod.Invoke(objectValue, null);
                        parseMethod.Invoke(parser, parameter);
                        setMethod.Invoke(objectValue, parameter);
                        value = (valueType)objectValue;
                    }
                }
#endif
            }
            /// <summary>
            /// 获取解析函数信息
            /// </summary>
            /// <param name="memberType"></param>
            /// <returns></returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private static MethodInfo getParseMemberMethod(Type memberType)
            {
                return getParseMethod(memberType) ?? ((memberType.IsEnum ? parseEnumMethod : unknownMethod).MakeGenericMethod(memberType));
            }
#if NOJIT
#else
            /// <summary>
            /// 创建解析委托函数
            /// </summary>
            /// <param name="type"></param>
            /// <param name="name">成员名称</param>
            /// <param name="memberType">成员类型</param>
            /// <param name="generator"></param>
            /// <returns>解析委托函数</returns>
            private static DynamicMethod createDynamicMethod(Type type, string name, Type memberType, out ILGenerator generator)
            {
                DynamicMethod dynamicMethod = new DynamicMethod("queryParser" + name, null, new Type[] { typeof(queryParser), type.MakeByRefType() }, type, true);
                generator = dynamicMethod.GetILGenerator();
                LocalBuilder loadMember = generator.DeclareLocal(memberType);
                generator.DeclareLocal(memberType);
                MethodInfo methodInfo = getParseMemberMethod(memberType);
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
                return dynamicMethod;
            }
#endif
            /// <summary>
            /// 解析状态
            /// </summary>
            private parseState state;
            /// <summary>
            /// HTTP请求头部
            /// </summary>
            private requestHeader requestHeader;
            /// <summary>
            /// 缓冲区起始位置
            /// </summary>
            private byte* bufferFixed;
            /// <summary>
            /// 当前解析位置
            /// </summary>
            private byte* current;
            /// <summary>
            /// 解析结束位置
            /// </summary>
            private byte* end;
            /// <summary>
            /// 当前处理位置
            /// </summary>
            private bufferIndex* queryIndex;
            /// <summary>
            /// 最后处理位置
            /// </summary>
            private bufferIndex* queryEndIndex;
#if NOJIT
            /// <summary>
            /// 反射模式参数
            /// </summary>
            private object[] ReflectionParameter;
#endif

            /// <summary>
            /// 查询解析器
            /// </summary>
            private queryParser() { }
            /// <summary>
            /// 查询解析
            /// </summary>
            /// <typeparam name="valueType">目标类型</typeparam>
            /// <param name="requestHeader">HTTP请求头部</param>
            /// <param name="value">目标数据</param>
            /// <returns>解析状态</returns>
            private parseState parse<valueType>(requestHeader requestHeader, ref valueType value)
            {
                this.requestHeader = requestHeader;
                state = parseState.Success;
                fixed (byte* bufferFixed = requestHeader.Buffer)
                {
                    this.bufferFixed = bufferFixed;
                    queryIndex = (bufferIndex*)(bufferFixed + socketBase.HeaderQueryStartIndex);
                    queryEndIndex = queryIndex + (requestHeader.queryCount << 1);
                    queryIndex -= 2;
                    parser<valueType>.Parse(this, ref value);
                }
                return state;
            }
            /// <summary>
            /// 释放查询解析器
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private void free()
            {
                requestHeader = null;
#if NOJIT
                ReflectionParameter = null;
#endif
                typePool<queryParser>.PushNotNull(this);
            }
            /// <summary>
            /// 解析10进制数字
            /// </summary>
            /// <param name="value">第一位数字</param>
            /// <returns>数字</returns>
            private uint parseUInt32(uint value)
            {
                uint number;
                do
                {
                    if ((number = (uint)(*current - '0')) > 9) return value;
                    value *= 10;
                    value += number;
                    if (++current == end) return value;
                }
                while (true);
            }
            /// <summary>
            /// 解析16进制数字
            /// </summary>
            /// <param name="value">数值</param>
            private void parseHex32(ref uint value)
            {
                uint number = (uint)(*current - '0');
                if (number > 9)
                {
                    if ((number = (number - ('A' - '0')) & 0xffdfU) > 5)
                    {
                        state = parseState.NotHex;
                        return;
                    }
                    number += 10;
                }
                value = number;
                if (++current == end) return;
                do
                {
                    if ((number = (uint)(*current - '0')) > 9)
                    {
                        if ((number = (number - ('A' - '0')) & 0xffdfU) > 5) return;
                        number += 10;
                    }
                    value <<= 4;
                    value += number;
                }
                while (++current != end);
            }
            /// <summary>
            /// 逻辑值解析
            /// </summary>
            /// <param name="value">数据</param>
            /// <returns>解析状态</returns>
            [parseType]
            internal void Parse(ref bool value)
            {
                bufferIndex* indexs = queryIndex + 1;
                switch (indexs->Length)
                {
                    case 0:
                        value = false;
                        return;
                    case 4:
                        current = bufferFixed + indexs->StartIndex;
                        if (*(int*)current == ('t' + ('r' << 8) + ('u' << 16) + ('e' << 24))) value = true;
                        else state = parseState.NotBool;
                        return;
                    case 5:
                        current = bufferFixed + indexs->StartIndex;
                        if ((*current | 0x20) == 'f' && *(int*)(current + 1) == ('a' + ('l' << 8) + ('s' << 16) + ('e' << 24))) value = false;
                        else state = parseState.NotBool;
                        return;
                    default:
                        byte byteValue = (byte)(*(bufferFixed + indexs->StartIndex) - '0');
                        if (byteValue < 10) value = byteValue != 0;
                        else state = parseState.NotBool;
                        return;
                }
            }
            /// <summary>
            /// 数字解析
            /// </summary>
            /// <param name="value">数据</param>
            [parseType]
            internal void Parse(ref byte value)
            {
                bufferIndex* indexs = queryIndex + 1;
                if (indexs->Length == 0)
                {
                    value = 0;
                    return;
                }
                current = bufferFixed + indexs->StartIndex;
                end = current + indexs->Length;
                uint number = (uint)(*current - '0');
                if (number > 9)
                {
                    state = parseState.NotNumber;
                    return;
                }
                if (++current == end)
                {
                    value = (byte)number;
                    return;
                }
                if (number == 0)
                {
                    if (*current != 'x')
                    {
                        value = 0;
                        return;
                    }
                    if (++current == end)
                    {
                        state = parseState.NotNumber;
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
            [parseType]
            internal void Parse(ref sbyte value)
            {
                bufferIndex* indexs = queryIndex + 1;
                if (indexs->Length == 0)
                {
                    value = 0;
                    return;
                }
                current = bufferFixed + indexs->StartIndex;
                end = current + indexs->Length;
                int sign = 0;
                if (*current == '-')
                {
                    if (++current == end)
                    {
                        state = parseState.NotNumber;
                        return;
                    }
                    sign = 1;
                }
                uint number = (uint)(*current - '0');
                if (number > 9)
                {
                    state = parseState.NotNumber;
                    return;
                }
                if (++current == end)
                {
                    value = sign == 0 ? (sbyte)(byte)number : (sbyte)-(int)number;
                    return;
                }
                if (number == 0)
                {
                    if (*current != 'x')
                    {
                        value = 0;
                        return;
                    }
                    if (++current == end)
                    {
                        state = parseState.NotNumber;
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
            [parseType]
            internal void Parse(ref ushort value)
            {
                bufferIndex* indexs = queryIndex + 1;
                if (indexs->Length == 0)
                {
                    value = 0;
                    return;
                }
                current = bufferFixed + indexs->StartIndex;
                end = current + indexs->Length;
                uint number = (uint)(*current - '0');
                if (number > 9)
                {
                    state = parseState.NotNumber;
                    return;
                }
                if (++current == end)
                {
                    value = (ushort)number;
                    return;
                }
                if (number == 0)
                {
                    if (*current != 'x')
                    {
                        value = 0;
                        return;
                    }
                    if (++current == end)
                    {
                        state = parseState.NotNumber;
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
            [parseType]
            internal void Parse(ref short value)
            {
                bufferIndex* indexs = queryIndex + 1;
                if (indexs->Length == 0)
                {
                    value = 0;
                    return;
                }
                current = bufferFixed + indexs->StartIndex;
                end = current + indexs->Length;
                int sign = 0;
                if (*current == '-')
                {
                    if (++current == end)
                    {
                        state = parseState.NotNumber;
                        return;
                    }
                    sign = 1;
                }
                uint number = (uint)(*current - '0');
                if (number > 9)
                {
                    state = parseState.NotNumber;
                    return;
                }
                if (++current == end)
                {
                    value = sign == 0 ? (short)(ushort)number : (short)-(int)number;
                    return;
                }
                if (number == 0)
                {
                    if (*current != 'x')
                    {
                        value = 0;
                        return;
                    }
                    if (++current == end)
                    {
                        state = parseState.NotNumber;
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
            [parseType]
            internal void Parse(ref uint value)
            {
                bufferIndex* indexs = queryIndex + 1;
                if (indexs->Length == 0)
                {
                    value = 0;
                    return;
                }
                current = bufferFixed + indexs->StartIndex;
                end = current + indexs->Length;
                uint number = (uint)(*current - '0');
                if (number > 9)
                {
                    state = parseState.NotNumber;
                    return;
                }
                if (++current == end)
                {
                    value = number;
                    return;
                }
                if (number == 0)
                {
                    if (*current != 'x')
                    {
                        value = 0;
                        return;
                    }
                    if (++current == end)
                    {
                        state = parseState.NotNumber;
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
            [parseType]
            internal void Parse(ref int value)
            {
                bufferIndex* indexs = queryIndex + 1;
                if (indexs->Length == 0)
                {
                    value = 0;
                    return;
                }
                current = bufferFixed + indexs->StartIndex;
                end = current + indexs->Length;
                int sign = 0;
                if (*current == '-')
                {
                    if (++current == end)
                    {
                        state = parseState.NotNumber;
                        return;
                    }
                    sign = 1;
                }
                uint number = (uint)(*current - '0');
                if (number > 9)
                {
                    state = parseState.NotNumber;
                    return;
                }
                if (++current == end)
                {
                    value = sign == 0 ? (int)number : -(int)number;
                    return;
                }
                if (number == 0)
                {
                    if (*current != 'x')
                    {
                        value = 0;
                        return;
                    }
                    if (++current == end)
                    {
                        state = parseState.NotNumber;
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
                byte* end32 = current + 8;
                if (end32 > end) end32 = end;
                uint number;
                do
                {
                    if ((number = (uint)(*current - '0')) > 9) return value;
                    value *= 10;
                    value += number;
                }
                while (++current != end32);
                if (current == end) return value;
                ulong value64 = value;
                do
                {
                    if ((number = (uint)(*current - '0')) > 9) return value64;
                    value64 *= 10;
                    value64 += number;
                    if (++current == end) return value64;
                }
                while (true);
            }
            /// <summary>
            /// 解析16进制数字
            /// </summary>
            /// <returns>数字</returns>
            private ulong parseHex64()
            {
                uint number = (uint)(*current - '0');
                if (number > 9)
                {
                    if ((number = (number - ('A' - '0')) & 0xffdfU) > 5)
                    {
                        state = parseState.NotHex;
                        return 0;
                    }
                    number += 10;
                }
                if (++current == end) return number;
                uint high = number;
                byte* end32 = current + 7;
                if (end32 > end) end32 = end;
                do
                {
                    if ((number = (uint)(*current - '0')) > 9)
                    {
                        if ((number = (number - ('A' - '0')) & 0xffdfU) > 5) return high;
                        number += 10;
                    }
                    high <<= 4;
                    high += number;
                }
                while (++current != end32);
                if (current == end) return high;
                byte* start = current;
                ulong low = number;
                do
                {
                    if ((number = (uint)(*current - '0')) > 9)
                    {
                        if ((number = (number - ('A' - '0')) & 0xffdfU) > 5)
                        {
                            return low | (ulong)high << ((int)((byte*)current - (byte*)start) << 1);
                        }
                        number += 10;
                    }
                    low <<= 4;
                    low += number;
                }
                while (++current != end);
                return low | (ulong)high << ((int)((byte*)current - (byte*)start) << 1);
            }
            /// <summary>
            /// 数字解析
            /// </summary>
            /// <param name="value">数据</param>
            [parseType]
            internal void Parse(ref ulong value)
            {
                bufferIndex* indexs = queryIndex + 1;
                if (indexs->Length == 0)
                {
                    value = 0;
                    return;
                }
                current = bufferFixed + indexs->StartIndex;
                end = current + indexs->Length;
                uint number = (uint)(*current - '0');
                if (number > 9)
                {
                    state = parseState.NotNumber;
                    return;
                }
                if (++current == end)
                {
                    value = number;
                    return;
                }
                if (number == 0)
                {
                    if (*current != 'x')
                    {
                        value = 0;
                        return;
                    }
                    if (++current == end)
                    {
                        state = parseState.NotNumber;
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
            [parseType]
            internal void Parse(ref long value)
            {
                bufferIndex* indexs = queryIndex + 1;
                if (indexs->Length == 0)
                {
                    value = 0;
                    return;
                }
                current = bufferFixed + indexs->StartIndex;
                end = current + indexs->Length;
                int sign = 0;
                if (*current == '-')
                {
                    if (++current == end)
                    {
                        state = parseState.NotNumber;
                        return;
                    }
                    sign = 1;
                }
                uint number = (uint)(*current - '0');
                if (number > 9)
                {
                    state = parseState.NotNumber;
                    return;
                }
                if (++current == end)
                {
                    value = sign == 0 ? (long)(int)number : -(long)(int)number;
                    return;
                }
                if (number == 0)
                {
                    if (*current != 'x')
                    {
                        value = 0;
                        return;
                    }
                    if (++current == end)
                    {
                        state = parseState.NotNumber;
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
            [parseType]
            internal void Parse(ref float value)
            {
                bufferIndex* indexs = queryIndex + 1;
                if (indexs->Length == 0) value = 0;
                else
                {
                    subArray<byte> jsonArray = subArray<byte>.Unsafe(requestHeader.Buffer, indexs->StartIndex, indexs->Length);
                    string number = formQuery.JavascriptUnescape(ref jsonArray);
                    if (!float.TryParse(number, out value)) state = parseState.NotNumber;
                }
            }
            /// <summary>
            /// 数字解析
            /// </summary>
            /// <param name="value">数据</param>
            [parseType]
            internal void Parse(ref double value)
            {
                bufferIndex* indexs = queryIndex + 1;
                if (indexs->Length == 0) value = 0;
                else
                {
                    subArray<byte> jsonArray = subArray<byte>.Unsafe(requestHeader.Buffer, indexs->StartIndex, indexs->Length);
                    string number = formQuery.JavascriptUnescape(ref jsonArray);
                    if (!double.TryParse(number, out value)) state = parseState.NotNumber;
                }
            }
            /// <summary>
            /// 数字解析
            /// </summary>
            /// <param name="value">数据</param>
            [parseType]
            internal void Parse(ref decimal value)
            {
                bufferIndex* indexs = queryIndex + 1;
                if (indexs->Length == 0) value = 0;
                else
                {
                    subArray<byte> jsonArray = subArray<byte>.Unsafe(requestHeader.Buffer, indexs->StartIndex, indexs->Length);
                    string number = formQuery.JavascriptUnescape(ref jsonArray);
                    if (!decimal.TryParse(number, out value)) state = parseState.NotNumber;
                }
            }
            /// <summary>
            /// 时间解析
            /// </summary>
            /// <param name="value">数据</param>
            [parseType]
            internal void Parse(ref DateTime value)
            {
                bufferIndex* indexs = queryIndex + 1;
                if (indexs->Length == 0) value = DateTime.MinValue;
                else
                {
                    subArray<byte> jsonArray = subArray<byte>.Unsafe(requestHeader.Buffer, indexs->StartIndex, indexs->Length);
                    string dateTime = formQuery.JavascriptUnescape(ref jsonArray);
                    if (!DateTime.TryParse(dateTime, out value)) state = parseState.NotDateTime;
                }
            }
            /// <summary>
            /// 解析16进制字符
            /// </summary>
            /// <returns>字符</returns>
            private uint parseHex4()
            {
                uint code = (uint)(*++current - '0'), number = (uint)(*++current - '0');
                if (code > 9) code = ((code - ('A' - '0')) & 0xffdfU) + 10;
                if (number > 9) number = ((number - ('A' - '0')) & 0xffdfU) + 10;
                code <<= 12;
                code += (number << 8);
                if ((number = (uint)(*++current - '0')) > 9) number = ((number - ('A' - '0')) & 0xffdfU) + 10;
                code += (number << 4);
                number = (uint)(*++current - '0');
                return code + (number > 9 ? (((number - ('A' - '0')) & 0xffdfU) + 10) : number);
            }
            /// <summary>
            /// 解析16进制字符
            /// </summary>
            /// <returns>字符</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private uint parseHex2()
            {
                uint code = (uint)(*++current - '0'), number = (uint)(*++current - '0');
                if (code > 9) code = ((code - ('A' - '0')) & 0xffdfU) + 10;
                return (number > 9 ? (((number - ('A' - '0')) & 0xffdfU) + 10) : number) + (code << 4);
            }
            /// <summary>
            /// Guid解析
            /// </summary>
            /// <param name="value">数据</param>
            [parseType]
            internal void Parse(ref Guid value)
            {
                bufferIndex* indexs = queryIndex + 1;
                if (indexs->Length == 0) value = new Guid();
                else if (end - current != 36) state = parseState.NotGuid;
                else
                {
                    current = bufferFixed + indexs->StartIndex;
                    end = current + indexs->Length;
                    guid guid = new guid();
                    guid.Byte3 = (byte)parseHex2();
                    guid.Byte2 = (byte)parseHex2();
                    guid.Byte1 = (byte)parseHex2();
                    guid.Byte0 = (byte)parseHex2();
                    if (*++current != '-')
                    {
                        state = parseState.NotGuid;
                        return;
                    }
                    guid.Byte45 = (ushort)parseHex4();
                    if (*++current != '-')
                    {
                        state = parseState.NotGuid;
                        return;
                    }
                    guid.Byte67 = (ushort)parseHex4();
                    if (*++current != '-')
                    {
                        state = parseState.NotGuid;
                        return;
                    }
                    guid.Byte8 = (byte)parseHex2();
                    guid.Byte9 = (byte)parseHex2();
                    if (*++current != '-')
                    {
                        state = parseState.NotGuid;
                        return;
                    }
                    guid.Byte10 = (byte)parseHex2();
                    guid.Byte11 = (byte)parseHex2();
                    guid.Byte12 = (byte)parseHex2();
                    guid.Byte13 = (byte)parseHex2();
                    guid.Byte14 = (byte)parseHex2();
                    guid.Byte15 = (byte)parseHex2();
                    value = guid.Value;
                }
            }
            /// <summary>
            /// 字符串解析
            /// </summary>
            /// <param name="value">数据</param>
            [parseType]
            internal void Parse(ref string value)
            {
                bufferIndex* indexs = queryIndex + 1;
                if (indexs->Length == 0) value = string.Empty;
                else
                {
                    subArray<byte> jsonArray = subArray<byte>.Unsafe(requestHeader.Buffer, indexs->StartIndex, indexs->Length);
                    value = formQuery.JavascriptUnescapeUtf8(ref jsonArray);
                }
            }
            /// <summary>
            /// 字符串解析
            /// </summary>
            /// <param name="value">数据</param>
            [parseType]
            internal void Parse(ref subString value)
            {
                bufferIndex* indexs = queryIndex + 1;
                if (indexs->Length == 0) value.UnsafeSet(string.Empty, 0, 0);
                else
                {
                    subArray<byte> jsonArray = subArray<byte>.Unsafe(requestHeader.Buffer, indexs->StartIndex, indexs->Length);
                    value = formQuery.JavascriptUnescapeUtf8(ref jsonArray);
                }
            }
            /// <summary>
            /// 未知类型解析
            /// </summary>
            /// <param name="value">目标数据</param>
            private unsafe void parseEnum<valueType>(ref valueType value)
            {
                bufferIndex* indexs = queryIndex + 1;
                if (indexs->Length == 0) value = default(valueType);
                else
                {
                    subArray<byte> jsonArray = subArray<byte>.Unsafe(requestHeader.Buffer, indexs->StartIndex - 1, indexs->Length + 2);
                    subString json = formQuery.JavascriptUnescapeUtf8(ref jsonArray);
                    fixed (char* jsonFixed = json.value) *jsonFixed = *(jsonFixed + json.Length - 1) = '"';
                    if (!fastCSharp.emit.jsonParser.Parse(ref json, ref value)) state = parseState.Unknown;
                }
            }
            /// <summary>
            /// 未知类型解析函数信息
            /// </summary>
            private static readonly MethodInfo parseEnumMethod = typeof(queryParser).GetMethod("parseEnum", BindingFlags.Instance | BindingFlags.NonPublic);
            /// <summary>
            /// 未知类型解析
            /// </summary>
            /// <param name="value">目标数据</param>
            private void unknown<valueType>(ref valueType value)
            {
                bufferIndex* indexs = queryIndex + 1;
                if (indexs->Length == 0) value = default(valueType);
                else
                {
                    subArray<byte> jsonArray = subArray<byte>.Unsafe(requestHeader.Buffer, indexs->StartIndex, indexs->Length);
                    subString json = formQuery.JavascriptUnescapeUtf8(ref jsonArray);
                    if (!fastCSharp.emit.jsonParser.Parse(ref json, ref value)) state = parseState.Unknown;
                }
            }
            /// <summary>
            /// 未知类型解析函数信息
            /// </summary>
            private static readonly MethodInfo unknownMethod = typeof(queryParser).GetMethod("unknown", BindingFlags.Instance | BindingFlags.NonPublic);
            /// <summary>
            /// 是否存在未结束的查询
            /// </summary>
            /// <returns>是否存在未结束的查询</returns>
            private bool isQuery()
            {
                if ((queryIndex += 2) == queryEndIndex) return false;
                current = bufferFixed + queryIndex->StartIndex;
                end = current + queryIndex->Length;
                return true;
            }
            /// <summary>
            /// 获取当前名称字符
            /// </summary>
            /// <returns>当前名称字符,结束返回0</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private byte getName()
            {
                return current == end ? (byte)0 : *current++;
            }
            /// <summary>
            /// 是否匹配默认顺序名称
            /// </summary>
            /// <param name="names"></param>
            /// <param name="index"></param>
            /// <returns></returns>
            private byte* isName(byte* names, ref int index)
            {
                if ((queryIndex += 2) == queryEndIndex)
                {
                    index = -1;
                    return names;
                }
                int length = *(short*)names;
                if (queryIndex->Length == (short)length && fastCSharp.unsafer.memory.SimpleEqual(bufferFixed + queryIndex->StartIndex, names += sizeof(short), length))
                {
                    return names + length;
                }
                current = bufferFixed + queryIndex->StartIndex;
                end = current + queryIndex->Length;
                return null;
            }
            /// <summary>
            /// 查询解析
            /// </summary>
            /// <typeparam name="valueType">目标类型</typeparam>
            /// <param name="requestHeader">HTTP请求头部</param>
            /// <param name="value">目标数据</param>
            /// <returns>是否解析成功</returns>
            public static bool Parse<valueType>(requestHeader requestHeader, ref valueType value)
            {
                if (requestHeader.queryCount != 0)
                {
                    queryParser parser = typePool<queryParser>.Pop() ?? new queryParser();
                    try
                    {
                        return parser.parse<valueType>(requestHeader, ref value) == parseState.Success;
                    }
                    finally { parser.free(); }
                }
                return true;
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
            static queryParser()
            {
                parseMethods = dictionary.CreateOnly<Type, MethodInfo>();
                foreach (MethodInfo method in typeof(queryParser).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
                {
                    if (method.customAttribute<parseType>() != null)
                    {
                        parseMethods.Add(method.GetParameters()[0].ParameterType.GetElementType(), method);
                    }
                }
            }
        }
        /// <summary>
        /// 搜索引擎首字母
        /// </summary>
        private enum searchEngineLetter : byte
        {
            /// <summary>
            /// 未知字符
            /// </summary>
            Unknown,
            /// <summary>
            /// bingbot
            /// </summary>
            b,
            /// <summary>
            /// DotBot
            /// </summary>
            D,
            /// <summary>
            /// Googlebot,GeoHasher
            /// </summary>
            G,
            /// <summary>
            /// ia_archiver
            /// </summary>
            i,
            /// <summary>
            /// Mediapartners-Google,MJ12bot
            /// </summary>
            M,
            /// <summary>
            /// msnbot
            /// </summary>
            m,
            /// <summary>
            /// R6_CommentReader
            /// </summary>
            R,
            /// <summary>
            /// renren share slurp
            /// </summary>
            r,
            /// <summary>
            /// Sogou,SiteBot
            /// </summary>
            S,
            /// <summary>
            /// spider
            /// </summary>
            s,
            /// <summary>
            /// Twiceler
            /// </summary>
            T,
            /// <summary>
            /// Yandex,YoudaoBot,Yahoo! Slurp
            /// </summary>
            Y,
            /// <summary>
            /// ZhihuExternalHit
            /// </summary>
            Z
        }
        /// <summary>
        /// 搜索引擎首字母查询表
        /// </summary>
        private static pointer searchEngineLetterTable;
        /// <summary>
        /// Google请求#!查询名称
        /// </summary>
        private static readonly byte[] googleFragmentName = ("escaped_fragment_=").getBytes();
        ///// <summary>
        ///// HTTP头名称解析委托
        ///// </summary>
        //private static readonly uniqueDictionary<headerName, Action<requestHeader, bufferIndex>> parses;
        /// <summary>
        /// HTTP请求头部缓冲区
        /// </summary>
        internal byte[] Buffer;
        /// <summary>
        /// 结束位置
        /// </summary>
        internal int EndIndex;
        /// <summary>
        /// HTTP请求头部数据
        /// </summary>
        public subArray<byte> Data
        {
            get { return subArray<byte>.Unsafe(Buffer, 0, EndIndex); }
        }
        /// <summary>
        /// 请求URI
        /// </summary>
        internal bufferIndex uri;
        /// <summary>
        /// 请求URI
        /// </summary>
        public subArray<byte> Uri
        {
            get { return subArray<byte>.Unsafe(Buffer, uri.StartIndex, uri.Length); }
        }
        /// <summary>
        /// 请求路径
        /// </summary>
        private bufferIndex path;
        /// <summary>
        /// 请求路径索引位置
        /// </summary>
        /// <param name="startIndex">起始位置</param>
        /// <param name="length">长度</param>
        public void UnsafeSetPath(int startIndex, int length)
        {
            path.Set(startIndex, length);
        }
        /// <summary>
        /// 请求路径索引位置
        /// </summary>
        /// <param name="length">长度</param>
        public void UnsafeSetPathLength(int length)
        {
            path.Length = (short)length;
        }
        /// <summary>
        /// 请求路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal unsafe bool UnsafeSetPath(byte[] path)
        {
            int endIndex = this.path.EndIndex, startIndex = endIndex - path.Length;
            if (startIndex >= 0)
            {
                fixed (byte* bufferFixed = Buffer) fastCSharp.unsafer.memory.SimpleCopy(path, bufferFixed + startIndex, path.Length);
                this.path.Set(startIndex, path.Length);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 请求路径
        /// </summary>
        public subArray<byte> Path
        {
            get { return subArray<byte>.Unsafe(Buffer, path.StartIndex, path.Length); }
        }
        ///// <summary>
        ///// 请求路径是否需要做web视图路径转换
        ///// </summary>
        //public unsafe bool IsViewPath
        //{
        //    get
        //    {
        //        fixed (byte* bufferFixed = Buffer)
        //        {
        //            if (path.Length > 1)
        //            {
        //                byte* start = bufferFixed + path.StartIndex;
        //                if (*start == '/')
        //                {
        //                    start += path.Length;
        //                    do
        //                    {
        //                        if (*--start == '/') return true;
        //                        if (*start == '.') return false;
        //                    }
        //                    while (true);
        //                }
        //            }
        //        }
        //        return false;
        //    }
        //}
#if MONO
#else
        /// <summary>
        /// 小写路径
        /// </summary>
        internal unsafe subArray<byte> LowerPath
        {
            get
            {
                fixed (byte* bufferFixed = Buffer)
                {
                    byte* start = bufferFixed + path.StartIndex;
                    fastCSharp.unsafer.memory.ToLower(start, start + path.Length);
                }
                return subArray<byte>.Unsafe(Buffer, path.StartIndex, path.Length);
            }
        }
#endif
        ///// <summary>
        ///// 查询参数索引集合
        ///// </summary>
        //private list<keyValue<bufferIndex, bufferIndex>> queryIndexs = new list<keyValue<bufferIndex, bufferIndex>>(sizeof(int));
        /// <summary>
        /// 查询参数数量
        /// </summary>
        private int queryCount;
        /// <summary>
        /// 请求域名
        /// </summary>
        internal bufferIndex host;
        /// <summary>
        /// 判断来源页是否合法
        /// </summary>
        private bool? isReferer;
        /// <summary>
        /// 判断来源页是否合法
        /// </summary>
        public bool IsReferer
        {
            get
            {
                if (isReferer == null)
                {
                    if (host.Length != 0)
                    {
                        subArray<byte> domain = default(subArray<byte>);
                        if (referer.Length != 0)
                        {
                            domain = fastCSharp.web.domain.GetMainDomainByUrl(Referer);
                        }
                        else if (origin.Length != 0)
                        {
                            domain = fastCSharp.web.domain.GetMainDomainByUrl(Origin);
                        }
                        if (domain.array != null && domain.equal(fastCSharp.web.domain.GetMainDomain(Host))) isReferer = true;
                    }
                    if (isReferer == null) isReferer = false;
                }
                return (bool)isReferer;
            }
        }
        /// <summary>
        /// 请求域名
        /// </summary>
        public subArray<byte> Host
        {
            get { return subArray<byte>.Unsafe(Buffer, host.StartIndex, host.Length); }
        }
        /// <summary>
        /// 提交数据分隔符
        /// </summary>
        private bufferIndex boundary;
        /// <summary>
        /// 提交数据分隔符
        /// </summary>
        public subArray<byte> Boundary
        {
            get { return subArray<byte>.Unsafe(Buffer, boundary.StartIndex, boundary.Length); }
        }
        /// <summary>
        /// HTTP请求内容类型
        /// </summary>
        private bufferIndex contentType;
        /// <summary>
        /// Cookie
        /// </summary>
        private bufferIndex cookie;
        /// <summary>
        /// 访问来源
        /// </summary>
        private bufferIndex referer;
        /// <summary>
        /// 访问来源
        /// </summary>
        public subArray<byte> Referer
        {
            get { return subArray<byte>.Unsafe(Buffer, referer.StartIndex, referer.Length); }
        }
        /// <summary>
        /// 访问来源
        /// </summary>
        internal bufferIndex origin;
        /// <summary>
        /// 访问来源
        /// </summary>
        public subArray<byte> Origin
        {
            get { return subArray<byte>.Unsafe(Buffer, origin.StartIndex, origin.Length); }
        }
        /// <summary>
        /// 请求范围起始位置
        /// </summary>
        internal long RangeStart { get; private set; }
        /// <summary>
        /// 请求范围结束位置
        /// </summary>
        internal long RangeEnd { get; private set; }
        /// <summary>
        /// 请求范围长度
        /// </summary>
        internal long RangeLength
        {
            get { return RangeEnd - RangeStart + 1; }
        }
        /// <summary>
        /// 是否存在请求范围
        /// </summary>
        public bool IsRange;
        /// <summary>
        /// 请求范围是否错误
        /// </summary>
        internal bool IsRangeError;
        /// <summary>
        /// 是否已经格式化请求范围
        /// </summary>
        internal bool IsFormatRange;
        /// <summary>
        /// 格式化请求范围
        /// </summary>
        /// <param name="contentLength">内容字节长度</param>
        /// <returns>范围是否有效</returns>
        public bool FormatRange(long contentLength)
        {
            IsFormatRange = true;
            if (RangeStart == 0)
            {
                if (RangeEnd >= contentLength - 1 || RangeEnd < 0)
                {
                    RangeStart = RangeEnd = long.MinValue;
                    IsRange = false;
                }
            }
            else if (RangeStart > 0)
            {
                if (RangeStart >= contentLength || (ulong)RangeEnd < (ulong)RangeStart)
                {
                    IsRangeError = true;
                    return false;
                }
                if (RangeEnd >= contentLength || RangeEnd < 0) RangeEnd = contentLength - 1;
            }
            else if (RangeEnd >= 0)
            {
                if (RangeEnd < contentLength) RangeStart = 0;
                else
                {
                    RangeEnd = long.MinValue;
                    IsRange = false;
                }
            }
            return true;
        }
        /// <summary>
        /// 浏览器参数
        /// </summary>
        private bufferIndex userAgent;
        /// <summary>
        /// 浏览器参数
        /// </summary>
        public subArray<byte> UserAgent
        {
            get { return subArray<byte>.Unsafe(Buffer, userAgent.StartIndex, userAgent.Length); }
        }
        /// <summary>
        /// 客户端文档时间标识
        /// </summary>
        internal bufferIndex ifModifiedSince;
        /// <summary>
        /// 客户端文档时间标识
        /// </summary>
        public subArray<byte> IfModifiedSince
        {
            get { return subArray<byte>.Unsafe(Buffer, ifModifiedSince.StartIndex, ifModifiedSince.Length); }
        }
        /// <summary>
        /// 客户端缓存有效标识
        /// </summary>
        private bufferIndex ifNoneMatch;
        /// <summary>
        /// 客户端缓存有效标识
        /// </summary>
        public subArray<byte> IfNoneMatch
        {
            get { return subArray<byte>.Unsafe(Buffer, ifNoneMatch.StartIndex, ifNoneMatch.Length); }
        }
        /// <summary>
        /// 转发信息
        /// </summary>
        private bufferIndex xProwardedFor;
        /// <summary>
        /// AJAX调用函数名称
        /// </summary>
        private bufferIndex ajaxCallName;
        /// <summary>
        /// AJAX调用函数名称
        /// </summary>
        internal subArray<byte> AjaxCallName
        {
            get { return subArray<byte>.Unsafe(Buffer, ajaxCallName.StartIndex, ajaxCallName.Length); }
        }
#if MONO
#else
        /// <summary>
        /// AJAX调用函数名称是否小写
        /// </summary>
        private bool isLowerAjaxCallName;
        /// <summary>
        /// AJAX调用函数名称
        /// </summary>
        internal unsafe subArray<byte> LowerAjaxCallName
        {
            get
            {
                if (!isLowerAjaxCallName)
                {
                    fixed (byte* bufferFixed = Buffer)
                    {
                        byte* start = bufferFixed + ajaxCallName.StartIndex;
                        fastCSharp.unsafer.memory.ToLower(start, start + ajaxCallName.Length);
                    }
                    isLowerAjaxCallName = true;
                }
                return subArray<byte>.Unsafe(Buffer, ajaxCallName.StartIndex, ajaxCallName.Length);
            }
        }
#endif
        /// <summary>
        /// AJAX回调函数名称
        /// </summary>
        private bufferIndex ajaxCallBackName;
        /// <summary>
        /// AJAX回调函数名称
        /// </summary>
        internal subArray<byte> AjaxCallBackName
        {
            get { return subArray<byte>.Unsafe(Buffer, ajaxCallBackName.StartIndex, ajaxCallBackName.Length); }
        }
        /// <summary>
        /// Json字符串
        /// </summary>
        private bufferIndex queryJson;
        /// <summary>
        /// Json字符串
        /// </summary>
        internal subString QueryJson
        {
            get
            {
                if (queryJson.Length != 0)
                {
                    subArray<byte> json = subArray<byte>.Unsafe(Buffer, queryJson.StartIndex, queryJson.Length);
                    return fastCSharp.web.formQuery.JavascriptUnescapeUtf8(ref json);
                }
                return default(subString);
            }
        }
        /// <summary>
        /// XML字符串
        /// </summary>
        private bufferIndex queryXml;
        /// <summary>
        /// XML字符串
        /// </summary>
        internal subString QueryXml
        {
            get
            {
                if (queryXml.Length != 0)
                {
                    subArray<byte> xml = subArray<byte>.Unsafe(Buffer, queryXml.StartIndex, queryXml.Length);
                    return fastCSharp.web.formQuery.JavascriptUnescapeUtf8(ref xml);
                }
                return default(subString);
            }
        }
        /// <summary>
        /// 是否重新加载视图
        /// </summary>
        public bool IsReView { get; private set; }
        /// <summary>
        /// 是否重新加载视图（忽略主列表）
        /// </summary>
        public bool IsMobileReView { get; private set; }
        /// <summary>
        /// 第一次加载页面缓存名称
        /// </summary>
        public bool IsLoadPageCache { get; private set; }
        /// <summary>
        /// HTTP头部名称数据
        /// </summary>
        internal int HeaderCount;
        /// <summary>
        /// 请求内存字节长度,int.MinValue表示未知,-1表示错误
        /// </summary>
        public int ContentLength { get; private set; }
        /// <summary>
        /// 请求内容编码
        /// </summary>
        private Encoding requestEncoding;
        /// <summary>
        /// 请求内容编码
        /// </summary>
        internal Encoding RequestEncoding
        {
            get { return requestEncoding ?? Encoding.UTF8; }
        }
        /// <summary>
        /// 查询模式类型
        /// </summary>
        public fastCSharp.web.http.methodType Method { get; internal set; }
        /// <summary>
        /// 提交数据类型
        /// </summary>
        internal postType PostType { get; private set; }
        /// <summary>
        /// 是否需要保持连接
        /// </summary>
        internal bool IsKeepAlive;
        /// <summary>
        /// 是否100 Continue确认
        /// </summary>
        internal bool Is100Continue;
        /// <summary>
        /// 连接是否升级协议
        /// </summary>
        private byte isConnectionUpgrade;
        /// <summary>
        /// 升级协议是否支持WebSocket
        /// </summary>
        private byte isUpgradeWebSocket;
        /// <summary>
        /// 是否WebSocket连接
        /// </summary>
        internal bool IsWebSocket;
        /// <summary>
        /// WebSocket确认连接值
        /// </summary>
        private bufferIndex secWebSocketKey;
        /// <summary>
        /// WebSocket确认连接值
        /// </summary>
        public subArray<byte> SecWebSocketKey
        {
            get { return subArray<byte>.Unsafe(Buffer, secWebSocketKey.StartIndex, secWebSocketKey.Length); }
        }
        /// <summary>
        /// WebSocket数据
        /// </summary>
        internal subString WebSocketData;
        /// <summary>
        /// 客户端是否支持GZip压缩
        /// </summary>
        internal bool IsGZip { get; set; }
        /// <summary>
        /// HTTP头部是否存在解析错误
        /// </summary>
        internal bool IsHeaderError { get; private set; }
        ///// <summary>
        ///// 是否google搜索引擎
        ///// </summary>
        //public bool IsGoogleQuery
        //{
        //    get { return GoogleQuery.Count != 0; }
        //}
        /// <summary>
        /// URL中是否包含#
        /// </summary>
        private bool? isHash;
        /// <summary>
        /// URL中是否包含#
        /// </summary>
        public bool IsHash
        {
            get
            {
                if (isHash == null) isHash = false;
                return isHash.Value;
            }
        }
        /// <summary>
        /// 是否搜索引擎
        /// </summary>
        private bool? isSearchEngine;
        /// <summary>
        /// 是否搜索引擎
        /// </summary>
        [fastCSharp.code.cSharp.webView.outputAjax(IsIgnoreCurrent = true)]
        public bool IsSearchEngine
        {
            get
            {
                if (isSearchEngine == null) isSearchEngine = userAgent.Length >= 5 && checkSearchEngine();
                return isSearchEngine.Value;
            }
        }
        /// <summary>
        /// 判断是否搜索引擎
        /// </summary>
        /// <returns></returns>
        private unsafe bool checkSearchEngine()
        {
            fixed (byte* bufferFixed = Buffer)
            {
                byte* start = bufferFixed + userAgent.StartIndex, letterTable = (byte*)searchEngineLetterTable.Data, end = start + userAgent.Length - 4;
                do
                {
                    switch (letterTable[*start])
                    {
                        case (byte)searchEngineLetter.b://bingbot
                            if (*(short*)(start + 1) == 'i' + ('n' << 8) && *(int*)(start + 3) == 'g' + ('b' << 8) + ('o' << 16) + ('t' << 24)) return true;
                            break;
                        case (byte)searchEngineLetter.D://DotBot
                            if (*(start + 1) == 'o' && *(int*)(start + 2) == 't' + ('B' << 8) + ('o' << 16) + ('t' << 24)) return true;
                            break;
                        case (byte)searchEngineLetter.G:
                            if (*(start + 1) == 'o')
                            {//Googlebot
                                if (*(int*)(start + 1) == 'o' + ('o' << 8) + ('g' << 16) + ('l' << 24) && *(int*)(start + 5) == 'e' + ('b' << 8) + ('o' << 16) + ('t' << 24)) return true;
                            }
                            else if (*(start + 1) == 'e')
                            {//GeoHasher
                                if (*(int*)(start + 1) == 'e' + ('o' << 8) + ('H' << 16) + ('a' << 24) && *(int*)(start + 5) == 's' + ('h' << 8) + ('e' << 16) + ('r' << 24)) return true;
                            }
                            break;
                        case (byte)searchEngineLetter.i://ia_archiver
                            if (*(short*)(start + 1) == 'a' + ('_' << 8) && *(int*)(start + 3) == 'a' + ('r' << 8) + ('c' << 16) + ('h' << 24) && *(int*)(start + 7) == 'i' + ('v' << 8) + ('e' << 16) + ('r' << 24)) return true;
                            break;
                        case (byte)searchEngineLetter.M:
                            if (*(start + 1) == 'e')
                            {//Mediapartners-Google
                                if (*(short*)(start + 2) == 'd' + ('i' << 8) && *(int*)(start + 4) == 'a' + ('p' << 8) + ('a' << 16) + ('r' << 24)
                                    && *(int*)(start + 8) == 't' + ('n' << 8) + ('e' << 16) + ('r' << 24) && *(int*)(start + 12) == 's' + ('-' << 8) + ('G' << 16) + ('o' << 24)
                                    && *(int*)(start + 16) == 'o' + ('g' << 8) + ('l' << 16) + ('e' << 24)) return true;
                            }
                            else if (*(short*)(start + 1) == 'J' + ('1' << 8))
                            {//MJ12bot
                                if (*(int*)(start + 3) == '2' + ('b' << 8) + ('o' << 16) + ('t' << 24)) return true;
                            }
                            break;
                        case (byte)searchEngineLetter.m://msnbot
                            if (*(start + 1) == 's' && *(int*)(start + 2) == 'n' + ('b' << 8) + ('o' << 16) + ('t' << 24)) return true;
                            break;
                        case (byte)searchEngineLetter.R://R6_CommentReader
                            if (*(int*)start == 'R' + ('6' << 8) + ('_' << 16) + ('C' << 24) && *(int*)(start + 4) == 'o' + ('m' << 8) + ('m' << 16) + ('e' << 24)
                                && *(int*)(start + 8) == 'n' + ('t' << 8) + ('R' << 16) + ('e' << 24) && *(int*)(start + 12) == 'a' + ('d' << 8) + ('e' << 16) + ('r' << 24)) return true;
                            break;
                        case (byte)searchEngineLetter.r://renren share slurp
                            if (*(start + 1) == 'e' && *(int*)(start + 2) == 'n' + ('r' << 8) + ('e' << 16) + ('n' << 24) && *(int*)(start + 6) == ' ' + ('s' << 8) + ('h' << 16) + ('a' << 24)
                                && *(int*)(start + 10) == 'r' + ('e' << 8) + (' ' << 16) + ('s' << 24) && *(int*)(start + 14) == 'l' + ('u' << 8) + ('r' << 16) + ('p' << 24)) return true;
                            break;
                        case (byte)searchEngineLetter.S://
                            if (*(start + 1) == 'p')
                            {//Spider
                                if (*(int*)(start + 2) == 'i' + ('d' << 8) + ('e' << 16) + ('r' << 24)) return true;
                            }
                            else if (*(start + 1) == 'o')
                            {//Sogou
                                if (*(int*)(start + 1) == 'o' + ('g' << 8) + ('o' << 16) + ('u' << 24)) return true;
                            }
                            else if (*(short*)(start + 1) == 'i' + ('t' << 8))
                            {//SiteBot
                                if (*(int*)(start + 3) == 'e' + ('B' << 8) + ('o' << 16) + ('t' << 24)) return true;
                            }
                            break;
                        case (byte)searchEngineLetter.s://spider
                            if (*(start + 1) == 'p' && *(int*)(start + 2) == 'i' + ('d' << 8) + ('e' << 16) + ('r' << 24)) return true;
                            break;
                        case (byte)searchEngineLetter.T://Twiceler
                            if (*(int*)start == 'T' + ('w' << 8) + ('i' << 16) + ('c' << 24) && *(int*)(start + 4) == 'e' + ('l' << 8) + ('e' << 16) + ('r' << 24)) return true;
                            break;
                        case (byte)searchEngineLetter.Y:
                            if (*(start + 1) == 'a')
                            {
                                if (*(short*)(start + 2) == 'h' + ('o' << 8))
                                {//Yahoo! Slurp
                                    if (*(int*)(start + 4) == 'o' + ('!' << 8) + (' ' << 16) + ('S' << 24) && *(int*)(start + 8) == 'l' + ('u' << 8) + ('r' << 16) + ('p' << 24)) return true;
                                }
                                else
                                {//Yandex
                                    if (*(int*)(start + 2) == 'n' + ('d' << 8) + ('e' << 16) + ('x' << 24)) return true;
                                }
                            }
                            else if (*(int*)(start + 1) == 'o' + ('u' << 8) + ('d' << 16) + ('a' << 24))
                            {//YoudaoBot
                                if (*(int*)(start + 5) == 'o' + ('B' << 8) + ('o' << 16) + ('t' << 24)) return true;
                            }
                            break;
                        case (byte)searchEngineLetter.Z://ZhihuExternalHit
                            if (*(int*)start == 'Z' + ('h' << 8) + ('i' << 16) + ('h' << 24) && *(int*)(start + 4) == 'u' + ('E' << 8) + ('x' << 16) + ('t' << 24)
                                 && *(int*)(start + 8) == 'e' + ('r' << 8) + ('n' << 16) + ('a' << 24) && *(int*)(start + 16) == 'l' + ('H' << 8) + ('i' << 16) + ('t' << 24)) return true;
                            break;
                    }
                }
                while (++start != end);
            }
            return false;
        }
        /// <summary>
        /// 是否SSL链接
        /// </summary>
        public bool IsSsl { get; internal set; }
        /// <summary>
        /// HTTP请求头
        /// </summary>
        public unsafe requestHeader()
        {
            Buffer = new byte[socketBase.HeaderQueryStartIndex + socketBase.MaxQueryCount * sizeof(bufferIndex) * 2];
        }
        /// <summary>
        /// HTTP头部解析
        /// </summary>
        /// <param name="headerEndIndex">HTTP头部数据结束位置</param>
        /// <param name="receiveEndIndex">HTTP缓冲区接收数据结束位置</param>
        /// <param name="isParseHeader">是否解析未知名称</param>
        /// <returns>是否成功</returns>
        internal unsafe bool Parse(int headerEndIndex, int receiveEndIndex, bool isParseHeader)
        {
            host.Value = 0;
            EndIndex = headerEndIndex;
            IsGZip = false;
            try
            {
                fixed (byte* bufferFixed = Buffer)
                {
                    if ((Method = fastCSharp.web.http.GetMethod(bufferFixed)) == fastCSharp.web.http.methodType.Unknown) return false;
                    byte* current = bufferFixed, end = bufferFixed + headerEndIndex;
                    for (*end = 32; *current != 32; ++current) ;
                    *end = 13;
                    if (current == end) return false;
                    while (*++current == 32) ;
                    if (current == end) return false;
                    byte* start = current;
                    while (*current != 32 && *current != 13) ++current;
                    uri.Set(start - bufferFixed, current - start);
                    if (uri.Length == 0) return false;
                    while (*current != 13) ++current;

                    byte* headerIndex = bufferFixed + socketBase.HeaderNameStartIndex;
                    HeaderCount = ContentLength = 0;
                    cookie.Value = boundary.Value = contentType.Value = referer.Value = userAgent.Value = ifModifiedSince.Value = ifNoneMatch.Value = xProwardedFor.Value = secWebSocketKey.Value = 0;
                    WebSocketData.Null();
                    requestEncoding = null;
                    isReferer = null;
                    PostType = postType.None;
                    //RangeStart = RangeEnd = long.MinValue;
                    IsFormatRange = Is100Continue = IsHeaderError = IsRange = IsRangeError = IsWebSocket = false;
                    isUpgradeWebSocket = isConnectionUpgrade = 0;
                    while (current != end)
                    {
                        if ((current += 2) >= end) return false;
                        byte* nameStart = current;
                        for (*end = (byte)':'; *current != (byte)':'; ++current) ;
                        int nameSize = (int)(current - nameStart), isParse = 0;
                        //subArray<byte> name = subArray<byte>.Unsafe(Buffer, (int)(start - bufferFixed), (int)(current - start));
                        *end = 13;
                        if (current == end || *++current != ' ') return false;
                        for (start = ++current; *current != 13; ++current) ;
                        //Action<requestHeader, bufferIndex> parseHeaderName = parses.Get(name, null);
                        switch ((((*(nameStart + (nameSize >> 1)) | 0x20) >> 2) ^ (*(nameStart + nameSize - 3) << 1)) & ((1 << 5) - 1))
                        {
                            case (('s' >> 2) ^ ('o' << 1)) & ((1 << 5) - 1):
                                if (((nameSize ^ 4) | ((*(int*)nameStart | 0x20202020) ^ ('h' + ('o' << 8) + ('s' << 16) + ('t' << 24)))) == 0)
                                {
                                    host.Set(start - bufferFixed, current - start);
                                    isParse = 1;
                                }
                                break;
                            case ((('-' | 0x20) >> 2) ^ ('g' << 1)) & ((1 << 5) - 1):
                                if (((nameSize ^ 14) | ((*(int*)nameStart | 0x20202020) ^ ('c' + ('o' << 8) + ('n' << 16) + ('t' << 24)))
                                    | ((*(int*)(nameStart + sizeof(int)) | 0x202020) ^ ('e' + ('n' << 8) + ('t' << 16) + ('-' << 24)))
                                    | ((*(int*)(nameStart + sizeof(int) * 2) | 0x20202020) ^ ('l' + ('e' << 8) + ('n' << 16) + ('g' << 24)))
                                    | ((*(short*)(nameStart + sizeof(int) * 3) | 0x2020) ^ ('t' + ('h' << 8)))) == 0)
                                {
                                    ContentLength = 0;
                                    while (start != current)
                                    {
                                        ContentLength *= 10;
                                        ContentLength += *start++ - '0';
                                    }
                                    isParse = 1;
                                }
                                break;
                            case (('e' >> 2) ^ ('i' << 1)) & ((1 << 5) - 1):
                                if (((nameSize ^ 15) | ((*(int*)nameStart | 0x20202020) ^ ('a' + ('c' << 8) + ('c' << 16) + ('e' << 24)))
                                    | ((*(int*)(nameStart + sizeof(int)) | 0x20002020) ^ ('p' + ('t' << 8) + ('-' << 16) + ('e' << 24)))
                                    | ((*(int*)(nameStart + sizeof(int) * 2) | 0x20202020) ^ ('n' + ('c' << 8) + ('o' << 16) + ('d' << 24)))
                                    | (int)((*(uint*)(nameStart + sizeof(int) * 3) | 0xff202020U) ^ ('i' + ('n' << 8) + ('g' << 16) + 0xff000000U))) == 0)
                                {
                                    if (current - start >= 4)
                                    {
                                        *current = (byte)'g';
                                        while (true)
                                        {
                                            while (*start != 'g') ++start;
                                            if (start != current)
                                            {
                                                if ((*(int*)start | 0x20202020) == ('g' | ('z' << 8) | ('i' << 16) | ('p' << 24)))
                                                {
                                                    IsGZip = true;
                                                    break;
                                                }
                                                else ++start;
                                            }
                                            else break;
                                        }
                                        *current = 13;
                                    }
                                    isParse = 1;
                                }
                                break;
                            case (('c' >> 2) ^ ('i' << 1)) & ((1 << 5) - 1):
                                if (((nameSize ^ 10) | ((*(int*)nameStart | 0x20202020) ^ ('c' + ('o' << 8) + ('n' << 16) + ('n' << 24)))
                                    | ((*(int*)(nameStart + sizeof(int)) | 0x20202020) ^ ('e' + ('c' << 8) + ('t' << 16) + ('i' << 24)))
                                    | ((*(short*)(nameStart + sizeof(int) * 2) | 0x2020) ^ ('o' + ('n' << 8)))) == 0)
                                {
                                    switch ((int)(current - start) - 5)
                                    {
                                        case 5 - 5:
                                            if ((((*(int*)start | 0x20202020) ^ ('c' | ('l' << 8) | ('o' << 16) | ('s' << 24)))
                                                | ((*(start + sizeof(int)) | 0x20) ^ 'e')) == 0)
                                            {
                                                IsKeepAlive = false;
                                            }
                                            break;
                                        case 7 - 5:
                                            if ((((*(int*)start | 0x20202020) ^ ('u' | ('p' << 8) | ('g' << 16) | ('r' << 24)))
                                                | ((*(int*)(start + sizeof(int)) | 0x202020) ^ ('a' | ('d' << 8) | ('e' << 16) | 0xd000000))) == 0)
                                            {
                                                isConnectionUpgrade = 1;
                                            }
                                            break;
                                        case 10 - 5:
                                            if ((((*(int*)start | 0x20202020) ^ ('k' | ('e' << 8) | ('e' << 16) | ('p' << 24)))
                                                | ((*(int*)(start + sizeof(int)) | 0x20202000) ^ ('-' | ('a' << 8) | ('l' << 16) | ('i' << 24)))
                                                | ((*(short*)(start + sizeof(int) * 2) | 0x2020) ^ ('v' | ('e' << 8)))) == 0)
                                            {
                                                IsKeepAlive = true;
                                            }
                                            break;
                                    }
                                    isParse = 1;
                                }
                                break;
                            case (('t' >> 2) ^ ('y' << 1)) & ((1 << 5) - 1):
                                if (((nameSize ^ 12) | ((*(int*)nameStart | 0x20202020) ^ ('c' + ('o' << 8) + ('n' << 16) + ('t' << 24)))
                                    | ((*(int*)(nameStart + sizeof(int)) | 0x202020) ^ ('e' + ('n' << 8) + ('t' << 16) + ('-' << 24)))
                                    | ((*(int*)(nameStart + sizeof(int) * 2) | 0x20202020) ^ ('t' + ('y' << 8) + ('p' << 16) + ('e' << 24)))) == 0)
                                {
                                    contentType.Set(start - bufferFixed, current - start);
                                    isParse = 1;
                                }
                                break;
                            case (('k' >> 2) ^ ('k' << 1)) & ((1 << 5) - 1):
                                if (((nameSize ^ 6) | ((*(int*)nameStart | 0x20202020) ^ ('c' + ('o' << 8) + ('o' << 16) + ('k' << 24)))
                                    | ((*(short*)(nameStart + sizeof(int)) | 0x2020) ^ ('i' + ('e' << 8)))) == 0)
                                {
                                    cookie.Set(start - bufferFixed, current - start);
                                    isParse = 1;
                                }
                                break;
                            case (('e' >> 2) ^ ('r' << 1)) & ((1 << 5) - 1):
                                if (((nameSize ^ 7) | ((*(int*)nameStart | 0x20202020) ^ ('r' + ('e' << 8) + ('f' << 16) + ('e' << 24)))
                                    | (int)((*(uint*)(nameStart + sizeof(int)) | 0xff202020U) ^ ('r' + ('e' << 8) + ('r' << 16) + 0xff000000U))) == 0)
                                {
                                    referer.Set(start - bufferFixed, current - start);
                                    isParse = 1;
                                }
                                break;
                            case (('n' >> 2) ^ ('n' << 1)) & ((1 << 5) - 1):
                                if (((nameSize ^ 5) | ((*(int*)nameStart | 0x20202020) ^ ('r' + ('a' << 8) + ('n' << 16) + ('g' << 24)))
                                    | ((*(nameStart + sizeof(int)) | 0x20) ^ 'e')) == 0)
                                {
                                    if ((int)(current - start) > 6 && ((*(int*)start ^ ('b' + ('y' << 8) + ('t' << 16) + ('e' << 24))) | (*(short*)(start + 4) ^ ('s' + ('=' << 8)))) == 0)
                                    {
                                        parseRange(start, current);
                                    }
                                    isParse = 1;
                                }
                                break;
                            case (('a' >> 2) ^ ('e' << 1)) & ((1 << 5) - 1):
                                if (((nameSize ^ 10) | ((*(int*)nameStart | 0x20202020) ^ ('u' + ('s' << 8) + ('e' << 16) + ('r' << 24)))
                                    | ((*(int*)(nameStart + sizeof(int)) | 0x20202000) ^ ('-' + ('a' << 8) + ('g' << 16) + ('e' << 24)))
                                    | ((*(short*)(nameStart + sizeof(int) * 2) | 0x2020) ^ ('n' + ('t' << 8)))) == 0)
                                {
                                    userAgent.Set(start - bufferFixed, current - start);
                                    isParse = 1;
                                }
                                break;
                            case (('i' >> 2) ^ ('n' << 1)) & ((1 << 5) - 1):
                                if (((nameSize ^ 17) | ((*(int*)nameStart | 0x20002020) ^ ('i' + ('f' << 8) + ('-' << 16) + ('m' << 24)))
                                    | ((*(int*)(nameStart + sizeof(int)) | 0x20202020) ^ ('o' + ('d' << 8) + ('i' << 16) + ('f' << 24)))
                                    | ((*(int*)(nameStart + sizeof(int) * 2) | 0x202020) ^ ('i' + ('e' << 8) + ('d' << 16) + ('-' << 24)))
                                    | ((*(int*)(nameStart + sizeof(int) * 3) | 0x20202020) ^ ('s' + ('i' << 8) + ('n' << 16) + ('c' << 24)))
                                    | ((*(nameStart + sizeof(int) * 4) | 0x20) ^ 'e')) == 0)
                                {
                                    ifModifiedSince.Set(start - bufferFixed, current - start);
                                    isParse = 1;
                                }
                                break;
                            case (('e' >> 2) ^ ('t' << 1)) & ((1 << 5) - 1):
                                if (((nameSize ^ 13) | ((*(int*)nameStart | 0x20002020) ^ ('i' + ('f' << 8) + ('-' << 16) + ('n' << 24)))
                                    | ((*(int*)(nameStart + sizeof(int)) | 0x202020) ^ ('o' + ('n' << 8) + ('e' << 16) + ('-' << 24)))
                                    | ((*(int*)(nameStart + sizeof(int) * 2) | 0x20202020) ^ ('m' + ('a' << 8) + ('t' << 16) + ('c' << 24)))
                                    | ((*(nameStart + sizeof(int) * 3) | 0x20) ^ 'h')) == 0)
                                {
                                    if (*(current - 1) == '"' && (nameSize = (int)(current - start)) >= 2)
                                    {
                                        if (*start == '"') ifNoneMatch.Set((int)(start - bufferFixed) + 1, nameSize - 2);
                                        else if ((*(int*)start & 0xffffff) == ('W' + ('/' << 8) + ('"' << 16)) && nameSize >= 4)
                                        {
                                            ifNoneMatch.Set(((int)(start - bufferFixed) + 3), nameSize - 4);
                                        }
                                    }
                                    isParse = 1;
                                }
                                break;
                            case (('r' >> 2) ^ ('F' << 1)) & ((1 << 5) - 1):
                                if (((nameSize ^ 15) | ((*(int*)nameStart | 0x20200020) ^ ('x' + ('-' << 8) + ('p' << 16) + ('r' << 24)))
                                    | ((*(int*)(nameStart + sizeof(int)) | 0x20202020) ^ ('o' + ('w' << 8) + ('a' << 16) + ('r' << 24)))
                                    | ((*(int*)(nameStart + sizeof(int) * 2) | 0x202020) ^ ('d' + ('e' << 8) + ('d' << 16) + ('-' << 24)))
                                    | (int)((*(uint*)(nameStart + sizeof(int) * 3) | 0xff202020U) ^ ('f' + ('o' << 8) + ('r' << 16) + 0xff000000U))) == 0)
                                {
                                    xProwardedFor.Set(start - bufferFixed, current - start);
                                    isParse = 1;
                                }
                                break;
                            case (('e' >> 2) ^ ('e' << 1)) & ((1 << 5) - 1):
                                if (((nameSize ^ 6) | ((*(int*)nameStart | 0x20202020) ^ ('e' + ('x' << 8) + ('p' << 16) + ('e' << 24)))
                                    | ((*(short*)(nameStart + sizeof(int)) | 0x2020) ^ ('c' + ('t' << 8)))) == 0)
                                {
                                    if ((int)(current - start) == 12)
                                    {
                                        if (((*(int*)start ^ ('1' | ('0' << 8) | ('0' << 16) | ('-' << 24)))
                                            | ((*(int*)(start + sizeof(int)) | 0x20202020) ^ ('c' | ('o' << 8) | ('n' << 16) | ('t' << 24)))
                                            | ((*(int*)(start + sizeof(int) * 2) | 0x20202020) ^ ('i' | ('n' << 8) | ('u' << 16) | ('e' << 24)))) == 0)
                                        {
                                            Is100Continue = true;
                                        }
                                    }
                                    isParse = 1;
                                }
                                break;
                            case (('r' >> 2) ^ ('a' << 1)) & ((1 << 5) - 1):
                                if (((nameSize ^ 7) | ((*(int*)nameStart | 0x20202020) ^ ('u' + ('p' << 8) + ('g' << 16) + ('r' << 24)))
                                    | (int)((*(uint*)(nameStart + sizeof(int)) | 0xff202020U) ^ ('a' + ('d' << 8) + ('e' << 16) + 0xff000000U))) == 0)
                                {
                                    if ((int)(current - start) == 9)
                                    {
                                        if ((((*(int*)start | 0x20202020) ^ ('w' | ('e' << 8) | ('b' << 16) | ('s' << 24)))
                                            | ((*(int*)(start + sizeof(int)) | 0x20202020) ^ ('o' | ('c' << 8) | ('k' << 16) | ('e' << 24)))
                                            | ((*(start + sizeof(int) * 2) | 0x20) ^ 't')) == 0)
                                        {
                                            isUpgradeWebSocket = 1;
                                        }
                                    }
                                    isParse = 1;
                                }
                                break;
                            case (('o' >> 2) ^ ('K' << 1)) & ((1 << 5) - 1):
                                if (((nameSize ^ 17) | ((*(int*)nameStart | 0x202020) ^ ('s' + ('e' << 8) + ('c' << 16) + ('-' << 24)))
                                    | ((*(int*)(nameStart + sizeof(int)) | 0x20202020) ^ ('w' + ('e' << 8) + ('b' << 16) + ('s' << 24)))
                                    | ((*(int*)(nameStart + sizeof(int) * 2) | 0x20202020) ^ ('o' + ('c' << 8) + ('k' << 16) + ('e' << 24)))
                                    | ((*(int*)(nameStart + sizeof(int) * 3) | 0x20200020) ^ ('t' + ('-' << 8) + ('k' << 16) + ('e' << 24)))
                                    | ((*(nameStart + sizeof(int) * 4) | 0x20) ^ 'y')) == 0)
                                {
                                    if ((int)(current - start) <= 32) secWebSocketKey.Set(start - bufferFixed, current - start);
                                    isParse = 1;
                                }
                                break;
                            case (('k' >> 2) ^ ('g' << 1)) & ((1 << 5) - 1):
                                if (((nameSize ^ 20) | ((*(int*)nameStart | 0x202020) ^ ('s' + ('e' << 8) + ('c' << 16) + ('-' << 24)))
                                    | ((*(int*)(nameStart + sizeof(int)) | 0x20202020) ^ ('w' + ('e' << 8) + ('b' << 16) + ('s' << 24)))
                                    | ((*(int*)(nameStart + sizeof(int) * 2) | 0x20202020) ^ ('o' + ('c' << 8) + ('k' << 16) + ('e' << 24)))
                                    | ((*(int*)(nameStart + sizeof(int) * 3) | 0x20200020) ^ ('t' + ('-' << 8) + ('o' << 16) + ('r' << 24)))
                                    | ((*(int*)(nameStart + sizeof(int) * 4) | 0x20202020) ^ ('i' + ('g' << 8) + ('i' << 16) + ('n' << 24)))) == 0)
                                {
                                    origin.Set(start - bufferFixed, current - start);
                                    isParse = 1;
                                }
                                break;
                            case (('g' >> 2) ^ ('g' << 1)) & ((1 << 5) - 1):
                                if (((nameSize ^ 6) | ((*(int*)nameStart | 0x20202020) ^ ('o' + ('r' << 8) + ('i' << 16) + ('g' << 24)))
                                    | ((*(short*)(nameStart + sizeof(int)) | 0x2020) ^ ('i' + ('n' << 8)))) == 0)
                                {
                                    origin.Set(start - bufferFixed, current - start);
                                    isParse = 1;
                                }
                                break;
                        }
                        if (isParse == 0 && isParseHeader)
                        {
                            if (HeaderCount == socketBase.MaxHeaderCount)
                            {
                                IsHeaderError = true;
                                break;
                            }
                            else
                            {
                                (*(bufferIndex*)headerIndex).Set((int)(nameStart - bufferFixed), nameSize);
                                (*(bufferIndex*)(headerIndex + sizeof(bufferIndex))).Set(start - bufferFixed, current - start);
                                ++HeaderCount;
                                headerIndex += sizeof(bufferIndex) * 2;
                            }
                        }
                    }
                    if (host.Length == 0 || ContentLength < 0) return false;
                    if ((isConnectionUpgrade & isUpgradeWebSocket) != 0 && secWebSocketKey.Length != 0)
                    {
                        if (Method != web.http.methodType.GET || ifModifiedSince.Length != 0) return false;
                        IsWebSocket = true;
                    }

                    if (contentType.Length != 0)
                    {
                        start = bufferFixed + contentType.StartIndex;
                        end = start + contentType.Length;
                        current = fastCSharp.unsafer.memory.Find(start, end, (byte)';');
                        switch ((current == null ? contentType.Length : (int)(current - start)) - 8)
                        {
                            case 8 - 8://text/xml; charset=utf-8
                                if ((((*(int*)start | 0x20202020) ^ ('t' | ('e' << 8) | ('x' << 16) | ('t' << 24)))
                                    | ((*(int*)(start + sizeof(int)) | 0x20202000) ^ ('/' | ('x' << 8) | ('m' << 16) | ('l' << 24)))) == 0)
                                {
                                    if (*(start += 8) == ';') parseCharset(start, end);
                                    PostType = postType.Xml;
                                }
                                break;
                            case 9 - 8://text/json; charset=utf-8
                                if ((((*(int*)start | 0x20202020) ^ ('t' | ('e' << 8) | ('x' << 16) | ('t' << 24)))
                                    | ((*(int*)(start + sizeof(int)) | 0x20202000) ^ ('/' | ('j' << 8) | ('s' << 16) | ('o' << 24)))
                                    | ((*(start + sizeof(int) * 2) | 0x20) ^ 'n')) == 0)
                                {
                                    if (*(start += 9) == ';') parseCharset(start, end);
                                    PostType = postType.Json;
                                }
                                break;
                            case 15 - 8://application/xml; charset=utf-8
                                if ((((*(int*)start | 0x20202020) ^ ('a' | ('p' << 8) | ('p' << 16) | ('l' << 24)))
                                    | ((*(int*)(start + sizeof(int)) | 0x20202020) ^ ('i' | ('c' << 8) | ('a' << 16) | ('t' << 24)))
                                    | ((*(int*)(start + sizeof(int) * 2) | 0x00202020) ^ ('i' | ('o' << 8) | ('n' << 16) | ('/' << 24)))
                                    | (int)((*(uint*)(start + sizeof(int) * 3) | 0xff202020U) ^ ('x' | ('m' << 8) | ('l' << 16) | 0xff000000U))) == 0)
                                {
                                    if (*(start += 15) == ';') parseCharset(start, end);
                                    PostType = postType.Xml;
                                }
                                break;
                            case 16 - 8://application/json; charset=utf-8
                                if ((((*(int*)start | 0x20202020) ^ ('a' | ('p' << 8) | ('p' << 16) | ('l' << 24)))
                                    | ((*(int*)(start + sizeof(int)) | 0x20202020) ^ ('i' | ('c' << 8) | ('a' << 16) | ('t' << 24)))
                                    | ((*(int*)(start + sizeof(int) * 2) | 0x00202020) ^ ('i' | ('o' << 8) | ('n' << 16) | ('/' << 24)))
                                    | ((*(int*)(start + sizeof(int) * 3) | 0x20202020) ^ ('j' | ('s' << 8) | ('o' << 16) | ('n' << 24)))) == 0)
                                {
                                    if (*(start += 16) == ';') parseCharset(start, end);
                                    PostType = postType.Json;
                                }
                                break;
                            case 19 - 8://multipart/form-data; boundary=---------------------------xxxxxxxxxxxxx
                                if (contentType.Length > 30)
                                {
                                    if ((((*(int*)start | 0x20202020) ^ ('m' | ('u' << 8) | ('l' << 16) | ('t' << 24)))
                                        | ((*(int*)(start + sizeof(int)) | 0x20202020) ^ ('i' | ('p' << 8) | ('a' << 16) | ('r' << 24)))
                                        | ((*(int*)(start + sizeof(int) * 2) | 0x20200020) ^ ('t' | ('/' << 8) | ('f' << 16) | ('o' << 24)))
                                        | ((*(int*)(start + sizeof(int) * 3) | 0x20002020) ^ ('r' | ('m' << 8) | ('-' << 16) | ('d' << 24)))
                                        | ((*(int*)(start + sizeof(int) * 4) | 0x00202020) ^ ('a' | ('t' << 8) | ('a' << 16) | (';' << 24)))
                                        | ((*(int*)(start + sizeof(int) * 5) | 0x20202000) ^ (' ' | ('b' << 8) | ('o' << 16) | ('u' << 24)))
                                        | ((*(int*)(start + sizeof(int) * 6) | 0x20202020) ^ ('n' | ('d' << 8) | ('a' << 16) | ('r' << 24)))
                                        | ((*(short*)(start += sizeof(int) * 7) | 0x20) ^ ('y' | ('=' << 8)))) == 0)
                                    {
                                        boundary.Set(contentType.StartIndex + sizeof(int) * 7 + 2, contentType.Length - (sizeof(int) * 7 + 2));
                                        if (boundary.Length > maxBoundaryLength) IsHeaderError = true;
                                        PostType = postType.FormData;
                                    }
                                }
                                break;
                            case 33 - 8://application/x-www-form-urlencoded
                                if ((((*(int*)start | 0x20202020) ^ ('a' | ('p' << 8) | ('p' << 16) | ('l' << 24)))
                                    | ((*(int*)(start + sizeof(int)) | 0x20202020) ^ ('i' | ('c' << 8) | ('a' << 16) | ('t' << 24)))
                                    | ((*(int*)(start + sizeof(int) * 2) | 0x00202020) ^ ('i' | ('o' << 8) | ('n' << 16) | ('/' << 24)))
                                    | ((*(int*)(start + sizeof(int) * 3) | 0x20200020) ^ ('x' | ('-' << 8) | ('w' << 16) | ('w' << 24)))
                                    | ((*(int*)(start + sizeof(int) * 4) | 0x20200020) ^ ('w' | ('-' << 8) | ('f' << 16) | ('o' << 24)))
                                    | ((*(int*)(start + sizeof(int) * 5) | 0x20002020) ^ ('r' | ('m' << 8) | ('-' << 16) | ('u' << 24)))
                                    | ((*(int*)(start + sizeof(int) * 6) | 0x20202020) ^ ('r' | ('l' << 8) | ('e' << 16) | ('n' << 24)))
                                    | ((*(int*)(start + sizeof(int) * 7) | 0x20202020) ^ ('c' | ('o' << 8) | ('d' << 16) | ('e' << 24)))
                                    | ((*(start + sizeof(int) * 8) | 0x20) ^ 'd')) == 0)
                                {
                                    PostType = postType.Form;
                                }
                                break;
                        }
                    }

                    if (Method == fastCSharp.web.http.methodType.POST)
                    {
                        if (PostType == postType.None)
                        {
                            if (IsWebSocket) return false;
                            PostType = postType.Data;
                        }
                        if (ContentLength < ((int)(receiveEndIndex - headerEndIndex) - sizeof(int))) return false;
                    }
                    else
                    {
                        if (PostType != postType.None || (!IsKeepAlive && receiveEndIndex != headerEndIndex + sizeof(int)) || ((uint)ContentLength | (uint)(int)boundary.Length) != 0) return false;
                    }
                    if (!IsHeaderError && !IsRangeError)
                    {
                        int isValue = queryCount = 0;
                        ajaxCallName.Value = ajaxCallBackName.Value = queryJson.Value = queryXml.Value = 0;
                        IsReView = IsMobileReView = IsLoadPageCache = false;
#if MONO
#else
                        isLowerAjaxCallName = false;
#endif
                        if (IsWebSocket)
                        {
                            isSearchEngine = isHash = false;
                            start = bufferFixed + uri.StartIndex;
                            end = fastCSharp.unsafer.memory.Find(start, start + uri.Length, (byte)'?');
                            if (end == null) path = uri;
                            else
                            {
                                path.Set(uri.StartIndex, (short)(end - start));
                                bufferIndex* nameIndex = (bufferIndex*)(bufferFixed + socketBase.HeaderQueryStartIndex);
                                current = end;
                                byte endValue = *(end = start + uri.Length);
                                *end = (byte)'&';
                                do
                                {
                                    byte isDefaultQuery = 0;
                                    nameIndex->StartIndex = (short)(++current - bufferFixed);
                                    while (*current != '&' && *current != '=') ++current;
                                    bufferIndex* valueIndex = nameIndex + 1;
                                    nameIndex->Length = (short)((int)(current - bufferFixed) - nameIndex->StartIndex);
                                    if (*current == '=')
                                    {
                                        isValue = 1;
                                        valueIndex->StartIndex = (short)(++current - bufferFixed);
                                        while (*current != '&') ++current;
                                        valueIndex->Length = (short)((int)(current - bufferFixed) - valueIndex->StartIndex);
                                    }
                                    else valueIndex->Value = 0;
                                    if (nameIndex->Length == 1 && valueIndex->Length != 0)
                                    {
                                        switch (*(bufferFixed + nameIndex->StartIndex) - 'a')
                                        {
                                            case fastCSharp.config.web.QueryJsonName - 'a':
                                                queryJson = *valueIndex;
                                                isDefaultQuery = 1;
                                                break;
                                            case fastCSharp.config.web.QueryXmlName - 'a':
                                                queryXml = *valueIndex;
                                                isDefaultQuery = 1;
                                                break;
                                        }
                                    }
                                    if (isDefaultQuery == 0)
                                    {
                                        if (queryCount == socketBase.MaxQueryCount)
                                        {
                                            IsHeaderError = true;
                                            break;
                                        }
                                        else
                                        {
                                            nameIndex += 2;
                                            ++queryCount;
                                        }
                                    }
                                }
                                while (current != end);
                                *end = endValue;
                            }
                        }
                        else
                        {
                            isSearchEngine = isHash = null;
                            start = bufferFixed + uri.StartIndex;
                            end = fastCSharp.unsafer.memory.Find(start, start + uri.Length, (byte)'?');
                            if (end == null)
                            {
                                end = fastCSharp.unsafer.memory.Find(start, start + uri.Length, (byte)'#');
                                if (end != null) isSearchEngine = isHash = true;
                            }
                            else if (*(end + 1) == '_')
                            {
                                fixed (byte* googleFixed = googleFragmentName)
                                {
                                    if (unsafer.memory.Equal(googleFixed, end + 2, googleFragmentName.Length))
                                    {
                                        isSearchEngine = isHash = true;
                                        byte* write = end + 1, urlEnd = start + uri.Length;
                                        current = write + googleFragmentName.Length + 1;
                                        byte endValue = *urlEnd;
                                        *urlEnd = (byte)'%';
                                        do
                                        {
                                            while (*current != '%') *write++ = *current++;
                                            if (current == urlEnd) break;
                                            uint code = (uint)(*++current - '0'), number = (uint)(*++current - '0');
                                            if (code > 9) code = ((code - ('A' - '0')) & 0xffdfU) + 10;
                                            code = (number > 9 ? (((number - ('A' - '0')) & 0xffdfU) + 10) : number) + (code << 4);
                                            *write++ = code == 0 ? (byte)' ' : (byte)code;
                                        }
                                        while (++current < urlEnd);
                                        *urlEnd = endValue;
                                        uri.Length = (short)((int)(write - bufferFixed) - uri.StartIndex);
                                    }
                                }
                            }
                            if (end == null) path = uri;
                            else
                            {
                                path.Set(uri.StartIndex, (short)(end - start));
                                bufferIndex* nameIndex = (bufferIndex*)(bufferFixed + socketBase.HeaderQueryStartIndex);
                                current = end;
                                byte endValue = *(end = start + uri.Length);
                                *end = (byte)'&';
                                if (isHash != null)
                                {
                                    if (*current == '!') ++current;
                                    else if (*current == '%' && *(short*)(current + 1) == '2' + ('1' << 8)) current += 3;
                                }
                                do
                                {
                                    byte isDefaultQuery = 0;
                                    nameIndex->StartIndex = (short)(++current - bufferFixed);
                                    while (*current != '&' && *current != '=') ++current;
                                    bufferIndex* valueIndex = nameIndex + 1;
                                    nameIndex->Length = (short)((int)(current - bufferFixed) - nameIndex->StartIndex);
                                    if (*current == '=')
                                    {
                                        isValue = 1;
                                        valueIndex->StartIndex = (short)(++current - bufferFixed);
                                        while (*current != '&') ++current;
                                        valueIndex->Length = (short)((int)(current - bufferFixed) - valueIndex->StartIndex);
                                    }
                                    else valueIndex->Value = 0;
                                    if (nameIndex->Length == 1 && valueIndex->Length != 0)
                                    {
                                        switch (*(bufferFixed + nameIndex->StartIndex) - 'a')
                                        {
                                            case fastCSharp.config.web.AjaxCallName - 'a':
                                                ajaxCallName = *valueIndex;
                                                isDefaultQuery = 1;
                                                break;
                                            case fastCSharp.config.web.AjaxCallBackName - 'a':
                                                ajaxCallBackName = *valueIndex;
                                                isDefaultQuery = 1;
                                                break;
                                            case fastCSharp.config.web.ReViewName - 'a':
                                                IsReView = true;
                                                isDefaultQuery = 1;
                                                break;
                                            case fastCSharp.config.web.LoadPageCache - 'a':
                                                IsLoadPageCache = true;
                                                isDefaultQuery = 1;
                                                break;
                                            case fastCSharp.config.web.MobileReViewName - 'a':
                                                IsReView = IsMobileReView = true;
                                                isDefaultQuery = 1;
                                                break;
                                            case fastCSharp.config.web.QueryJsonName - 'a':
                                                queryJson = *valueIndex;
                                                isDefaultQuery = 1;
                                                break;
                                            case fastCSharp.config.web.QueryXmlName - 'a':
                                                queryXml = *valueIndex;
                                                isDefaultQuery = 1;
                                                break;
                                            case fastCSharp.config.web.EncodingName - 'a':
                                                if (requestEncoding == null)
                                                {
                                                    start = bufferFixed + valueIndex->StartIndex;
                                                    switch (*(start + 2) & 15)
                                                    {
                                                        case ('2' & 15)://gb2312
                                                            if (*(int*)(start + 2) == ('2' | ('3' << 8) | ('1' << 16) | ('2' << 24))) requestEncoding = encoding.Gb2312;
                                                            break;
                                                        case ('f' & 15)://utf-8
                                                            if ((*(int*)(start + 1) | 0x2020) == ('t' | ('f' << 8) | ('-' << 16) | ('8' << 24))) requestEncoding = Encoding.UTF8;
                                                            break;
                                                        case ('k' & 15)://gbk
                                                            if ((*(int*)(start - 1) | 0x20202000) == ('=' | ('g' << 8) | ('b' << 16) | ('k' << 24))) requestEncoding = encoding.Gbk;
                                                            break;
                                                        case ('g' & 15)://big5
                                                            if ((*(int*)start | 0x00202020) == ('b' | ('i' << 8) | ('g' << 16) | ('5' << 24))) requestEncoding = encoding.Big5;
                                                            break;
                                                        case ('1' & 15)://gb18030
                                                            if (*(int*)(start + 3) == ('8' | ('0' << 8) | ('3' << 16) | ('0' << 24))) requestEncoding = encoding.Gb18030;
                                                            break;
                                                        case ('i' & 15)://unicode
                                                            if ((*(int*)(start + 3) | 0x20202020) == ('c' | ('o' << 8) | ('d' << 16) | ('e' << 24))) requestEncoding = Encoding.Unicode;
                                                            break;
                                                    }
                                                    isDefaultQuery = 1;
                                                }
                                                break;
                                        }
                                    }
                                    if (isDefaultQuery == 0)
                                    {
                                        if (queryCount == socketBase.MaxQueryCount)
                                        {
                                            IsHeaderError = true;
                                            break;
                                        }
                                        else
                                        {
                                            nameIndex += 2;
                                            ++queryCount;
                                        }
                                    }
                                }
                                while (current != end);
                                *end = endValue;
                            }
                        }
                        if (((queryCount ^ 1) | isValue) == 0)
                        {
                            bufferIndex* nameIndex = (bufferIndex*)(bufferFixed + socketBase.HeaderQueryStartIndex);
                            (nameIndex + 1)->Value = nameIndex->Value;
                            nameIndex->StartIndex = 1;
                            nameIndex->Length = 0;
                        }
                    }
                    return true;
                }
            }
            catch (Exception error)
            {
                log.Default.Add(error, null, false);
            }
            return false;
        }
        /// <summary>
        /// 编码解析
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private unsafe void parseCharset(byte* start, byte* end)
        {
            *end = (byte)'c';
            while (*++start != 'c') ;
            if (start != end && (((*(int*)start | 0x20202020) ^ ('c' | ('h' << 8) | ('a' << 16) | ('r' << 24)))
                | ((*(int*)(start + sizeof(int)) | 0x00202020) ^ ('s' | ('e' << 8) | ('t' << 16) | ('=' << 24)))) == 0)
            {
                switch (*(start + 10) & 15)
                {
                    case ('2' & 15)://gb2312
                        if (*(int*)(start + 10) == ('2' | ('3' << 8) | ('1' << 16) | ('2' << 24))) requestEncoding = encoding.Gb2312;
                        break;
                    case ('f' & 15)://utf-8
                        if ((*(int*)(start + 9) | 0x2020) == ('t' | ('f' << 8) | ('-' << 16) | ('8' << 24))) requestEncoding = Encoding.UTF8;
                        break;
                    case ('k' & 15)://gbk
                        if ((*(int*)(start + 7) | 0x20202000) == ('=' | ('g' << 8) | ('b' << 16) | ('k' << 24))) requestEncoding = encoding.Gbk;
                        break;
                    case ('g' & 15)://big5
                        if ((*(int*)(start + 8) | 0x00202020) == ('b' | ('i' << 8) | ('g' << 16) | ('5' << 24))) requestEncoding = encoding.Big5;
                        break;
                    case ('1' & 15)://gb18030
                        if (*(int*)(start + 11) == ('8' | ('0' << 8) | ('3' << 16) | ('0' << 24))) requestEncoding = encoding.Gb18030;
                        break;
                    case ('i' & 15)://unicode
                        if ((*(int*)(start + 11) | 0x20202020) == ('c' | ('o' << 8) | ('d' << 16) | ('e' << 24))) requestEncoding = Encoding.Unicode;
                        break;
                }
            }
            *end = 13;
        }
        /// <summary>
        /// WebSocket重置URL
        /// </summary>
        /// <param name="start">起始位置</param>
        /// <param name="end">结束位置</param>
        /// <returns>是否成功</returns>
        internal unsafe bool SetWebSocketUrl(byte* start, byte* end)
        {
            int length = (int)(end - start);
            if (EndIndex + length <= socketBase.HeaderBufferLength)
            {
                fixed (byte* bufferFixed = Buffer)
                {
                    unsafer.memory.Copy(start, bufferFixed + EndIndex, length);
                    start = bufferFixed + EndIndex;
                    uri.Set(EndIndex, length);
                    byte* current = start;
                    for (*(end = start + length) = (byte)'?'; *current != '?'; ++current) ;
                    queryCount = 0;
                    if (current == end) path = uri;
                    else
                    {
                        path.Set(EndIndex, (int)(current - start));
                        bufferIndex* nameIndex = (bufferIndex*)(bufferFixed + socketBase.HeaderQueryStartIndex);
                        *end = (byte)'&';
                        do
                        {
                            nameIndex->StartIndex = (short)(++current - bufferFixed);
                            while (*current != '&' && *current != '=') ++current;
                            nameIndex->Length = (short)((int)(current - bufferFixed) - nameIndex->StartIndex);
                            if (*current == '=')
                            {
                                bufferIndex* valueIndex = nameIndex + 1;
                                valueIndex->StartIndex = (short)(++current - bufferFixed);
                                while (*current != '&') ++current;
                                valueIndex->Length = (short)((int)(current - bufferFixed) - valueIndex->StartIndex);
                                if (queryCount == socketBase.MaxQueryCount)
                                {
                                    IsHeaderError = true;
                                    break;
                                }
                                else
                                {
                                    nameIndex += 2;
                                    ++queryCount;
                                }
                            }
                            else if (nameIndex->Length == 0)
                            {
                                if (queryCount == socketBase.MaxQueryCount)
                                {
                                    IsHeaderError = true;
                                    break;
                                }
                                else
                                {
                                    (nameIndex + 1)->Null();
                                    ++queryCount;
                                    nameIndex += 2;
                                }
                            }
                        }
                        while (current != end);
                    }
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 判断是否存在Cookie值
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>是否存在Cookie值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe bool IsCookie(byte[] name)
        {
            if (cookie.Length > name.Length)
            {
                fixed (byte* nameFixed = name)
                {
                    return getCookie(nameFixed, name.Length).StartIndex != 0;
                }
            }
            return false;
        }
        /// <summary>
        /// 获取Cookie值
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>值</returns>
        internal unsafe string GetCookieString(byte[] name)
        {
            if (cookie.Length > name.Length)
            {
                fixed (byte* nameFixed = name)
                {
                    bufferIndex index = getCookie(nameFixed, name.Length);
                    if (index.StartIndex != 0)
                    {
                        if (index.Length == 0) return string.Empty;
                        fixed (byte* bufferFixed = Buffer) return String.UnsafeDeSerialize(bufferFixed + index.StartIndex, -index.Length);
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 获取Cookie值
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe void GetCookie(byte[] name, ref subArray<byte> value)
        {
            if (cookie.Length > name.Length)
            {
                fixed (byte* nameFixed = name)
                {
                    bufferIndex index = getCookie(nameFixed, name.Length);
                    if (index.StartIndex != 0) value.UnsafeSet(Buffer, index.StartIndex, index.Length);
                }
            }
        }
        /// <summary>
        /// 获取Cookie值
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>值</returns>
        public unsafe string GetCookie(string name)
        {
            if (cookie.Length > name.Length && name.Length <= unmanagedStreamBase.DefaultLength)
            {
                bufferIndex index;
                fixed (char* nameFixed = name)
                {
                    pointer cookieNameBuffer = unmanagedPool.TinyBuffers.Get();
                    unsafer.String.WriteBytes(nameFixed, name.Length, cookieNameBuffer.Byte);
                    index = getCookie(cookieNameBuffer.Byte, name.Length);
                    unmanagedPool.TinyBuffers.Push(ref cookieNameBuffer);
                }
                if (index.StartIndex != 0)
                {
                    if (index.Length == 0) return string.Empty;
                    fixed (byte* bufferFixed = Buffer) return String.UnsafeDeSerialize(bufferFixed + index.StartIndex, -index.Length);
                }
            }
            return null;
        }
        /// <summary>
        /// 获取Cookie值
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="nameLength">名称长度</param>
        /// <returns>值</returns>
        private unsafe bufferIndex getCookie(byte* name, int nameLength)
        {
            fixed (byte* bufferFixed = Buffer)
            {
                byte* start = bufferFixed + cookie.StartIndex, end = start + cookie.Length, searchEnd = end - nameLength;
                *end = (byte)';';
                do
                {
                    while (*start == ' ') ++start;
                    if (start >= searchEnd) break;
                    if (*(start + nameLength) == '=')
                    {
                        if (unsafer.memory.SimpleEqual(name, start, nameLength))
                        {
                            for (start += nameLength + 1; *start == ' '; ++start) ;
                            int startIndex = (int)(start - bufferFixed);
                            while (*start != ';') ++start;
                            return new bufferIndex { StartIndex = (short)startIndex, Length = (short)((int)(start - bufferFixed) - startIndex) };
                        }
                        start += nameLength + 1;
                    }
                    while (*start != ';') ++start;
                }
                while (++start < searchEnd);
            }
            return default(bufferIndex);
        }
        /// <summary>
        /// 获取查询值
        /// </summary>
        /// <param name="name">查询名称</param>
        /// <returns>查询值</returns>
        private unsafe subArray<byte> getQuery(byte[] name)
        {
            if (queryCount != 0)
            {
                fixed (byte* bufferFixed = Buffer)
                {
                    bufferIndex* start = (bufferIndex*)(bufferFixed + socketBase.HeaderQueryStartIndex), end = start + (queryCount << 1);
                    do
                    {
                        if (start->Length == name.Length
                            && unsafer.memory.SimpleEqual(name, bufferFixed + start->StartIndex, start->Length))
                        {
                            ++start;
                            return subArray<byte>.Unsafe(Buffer, start->StartIndex, start->Length);
                        }
                    }
                    while ((start += 2) != end);
                }
            }
            return default(subArray<byte>);
        }
        /// <summary>
        /// 获取查询整数值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nullValue"></param>
        /// <returns></returns>
        public unsafe int GetQueryInt(byte[] name, int nullValue)
        {
            subArray<byte> value = getQuery(name);
            if (value.length != 0)
            {
                int intValue = 0;
                fixed (byte* bufferFixed = Buffer)
                {
                    byte* start = bufferFixed + value.startIndex, end = start + value.length;
                    for (intValue = *(start) - '0'; ++start != end; intValue += *(start) - '0') intValue *= 10;
                }
                return intValue;
            }
            return nullValue;
        }
        /// <summary>
        /// 查询解析
        /// </summary>
        /// <typeparam name="valueType">目标类型</typeparam>
        /// <param name="value">目标数据</param>
        /// <returns>是否解析成功</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal bool ParseQuery<valueType>(ref valueType value)
        {
            return queryParser.Parse(this, ref value);
        }
        /// <summary>
        /// 请求字节范围解析
        /// </summary>
        /// <param name="start">数据起始位置</param>
        /// <param name="end">数据结束未知</param>
        private unsafe void parseRange(byte* start, byte* end)
        {
            IsRange = true;
            if (*(start += 6) == '-')
            {
                long rangeEnd = 0;
                while (++start != end)
                {
                    rangeEnd *= 10;
                    rangeEnd += *start - '0';
                }
                if (rangeEnd > 0)
                {
                    RangeStart = long.MinValue;
                    RangeEnd = rangeEnd;
                    return;
                }
            }
            else
            {
                long rangeStart = 0;
                do
                {
                    int number = *start - '0';
                    if ((uint)number > 9) break;
                    rangeStart *= 10;
                    ++start;
                    rangeStart += number;
                }
                while (true);
                if (rangeStart >= 0 && *start == '-')
                {
                    if (++start == end)
                    {
                        RangeStart = rangeStart;
                        RangeEnd = long.MinValue;
                        return;
                    }
                    long rangeEnd = *start - '0';
                    while (++start != end)
                    {
                        rangeEnd *= 10;
                        rangeEnd += *start - '0';
                    }
                    if (rangeStart < rangeEnd)
                    {
                        RangeStart = rangeStart;
                        RangeEnd = rangeEnd;
                        return;
                    }
                }
            }
            IsRangeError = true;
        }
        unsafe static requestHeader()
        {
            searchEngineLetterTable = unmanaged.GetStatic(256, true);
            byte* letterTable = (byte*)searchEngineLetterTable.Data;
            letterTable['b'] = (byte)searchEngineLetter.b;
            letterTable['D'] = (byte)searchEngineLetter.D;
            letterTable['G'] = (byte)searchEngineLetter.G;
            letterTable['i'] = (byte)searchEngineLetter.i;
            letterTable['M'] = (byte)searchEngineLetter.M;
            letterTable['m'] = (byte)searchEngineLetter.m;
            letterTable['R'] = (byte)searchEngineLetter.R;
            letterTable['r'] = (byte)searchEngineLetter.r;
            letterTable['S'] = (byte)searchEngineLetter.S;
            letterTable['s'] = (byte)searchEngineLetter.s;
            letterTable['T'] = (byte)searchEngineLetter.T;
            letterTable['Y'] = (byte)searchEngineLetter.Y;
            letterTable['Z'] = (byte)searchEngineLetter.Z;
        }
    }
}
