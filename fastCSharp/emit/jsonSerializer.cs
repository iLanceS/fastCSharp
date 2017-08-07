using System;
using System.Collections.Generic;
using System.Reflection;
using fastCSharp.reflection;
using fastCSharp.threading;
using fastCSharp.code;
using System.Runtime.CompilerServices;
#if NOJIT
#else
using System.Reflection.Emit;
#endif

namespace fastCSharp.emit
{
    /// <summary>
    /// JSON序列化
    /// </summary>
    public unsafe sealed class jsonSerializer : stringSerializer
    {
        /// <summary>
        /// 警告提示状态
        /// </summary>
        public enum warning : byte
        {
            /// <summary>
            /// 正常
            /// </summary>
            None,
            /// <summary>
            /// 缺少循环引用设置函数名称
            /// </summary>
            LessSetLoop,
            /// <summary>
            /// 缺少循环引用获取函数名称
            /// </summary>
            LessGetLoop,
            /// <summary>
            /// 成员位图类型不匹配
            /// </summary>
            MemberMap,
        }
        /// <summary>
        /// 配置参数
        /// </summary>
        public sealed class config
        {
            /// <summary>
            /// 循环引用设置函数名称
            /// </summary>
            public string SetLoopObject;
            /// <summary>
            /// 循环引用获取函数名称
            /// </summary>
            public string GetLoopObject;
            /// <summary>
            /// 循环引用检测深度,0表示实时检测
            /// </summary>
            public int CheckLoopDepth;
            /// <summary>
            /// 成员位图
            /// </summary>
            public memberMap MemberMap;
            /// <summary>
            /// 警告提示状态
            /// </summary>
            public warning Warning { get; internal set; }
            /// <summary>
            /// 最小时间是否输出为null
            /// </summary>
            public bool IsDateTimeMinNull = true;
            /// <summary>
            /// 时间是否转换成字符串
            /// </summary>
            public bool IsDateTimeToString;
            /// <summary>
            /// 第三方格式 /Date(xxx)/
            /// </summary>
            public bool IsDateTimeOther;
            /// <summary>
            /// 是否将object转换成真实类型输出
            /// </summary>
            public bool IsObject;
            /// <summary>
            /// Dictionary[string,]是否转换成对象输出
            /// </summary>
            public bool IsStringDictionaryToObject = true;
            /// <summary>
            /// Dictionary是否转换成对象模式输出
            /// </summary>
            public bool IsDictionaryToObject;
            /// <summary>
            /// 是否输出客户端视图绑定类型
            /// </summary>
            public bool IsViewClientType;
            /// <summary>
            /// 超出最大有效精度的long/ulong是否转换成字符串
            /// </summary>
            public bool IsMaxNumberToString;
            /// <summary>
            /// 成员位图类型不匹配是否输出错误信息
            /// </summary>
            public bool IsMemberMapErrorLog = true;
            /// <summary>
            /// 成员位图类型不匹配时是否使用默认输出
            /// </summary>
            public bool IsMemberMapErrorToDefault = true;
        }
        /// <summary>
        /// 基本转换类型
        /// </summary>
        private sealed class toJsonMethod : Attribute { }
        /// <summary>
        /// 对象转换JSON字符串静态信息
        /// </summary>
        private static class typeToJsoner
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
                /// 是否第一个字段
                /// </summary>
                private byte isFirstMember;
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
                    dynamicMethod = new DynamicMethod("jsonSerializer", null, new Type[] { typeof(jsonSerializer), type }, type, true);
                    generator = dynamicMethod.GetILGenerator();
                    generator.DeclareLocal(typeof(charStream));

                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldfld, charStreamField);
                    generator.Emit(OpCodes.Stloc_0);

                    isFirstMember = 1;
                    isValueType = type.IsValueType;
                }
                /// <summary>
                /// 添加成员
                /// </summary>
                /// <param name="name">成员名称</param>
                private void push(string name)
                {
                    if (isFirstMember == 0) generator.charStreamSimpleWriteNotNull(OpCodes.Ldloc_0, GetNamePool(name), name.Length + 4);
                    else
                    {
                        generator.charStreamSimpleWriteNotNull(OpCodes.Ldloc_0, GetNamePool(name) + 1, name.Length + 3);
                        isFirstMember = 0;
                    }
                    generator.Emit(OpCodes.Ldarg_0);
                    if (isValueType) generator.Emit(OpCodes.Ldarga_S, 1);
                    else generator.Emit(OpCodes.Ldarg_1);
                }
                /// <summary>
                /// 添加字段
                /// </summary>
                /// <param name="field">字段信息</param>
                public void Push(fieldIndex field)
                {
                    push(field.AnonymousName);
                    generator.Emit(OpCodes.Ldfld, field.Member);
                    MethodInfo method = GetMemberMethodInfo(field.Member.FieldType);
                    generator.Emit(method.IsFinal || !method.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, method);
                }
                /// <summary>
                /// 添加属性
                /// </summary>
                /// <param name="property">属性信息</param>
                /// <param name="method">函数信息</param>
                public void Push(propertyIndex property, MethodInfo method)
                {
                    push(property.Member.Name);
                    generator.Emit(method.IsFinal || !method.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, method);
                    method = GetMemberMethodInfo(property.Member.PropertyType);
                    generator.Emit(method.IsFinal || !method.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, method);
                }
                /// <summary>
                /// 添加字段
                /// </summary>
                /// <param name="field">字段信息</param>
                public void PushBox(fieldIndex field)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    if (isValueType) generator.Emit(OpCodes.Ldarga_S, 1);
                    else generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Ldfld, field.Member);
                    MethodInfo method = GetMemberMethodInfo(field.Member.FieldType);
                    generator.Emit(method.IsFinal || !method.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, method);
                }
                /// <summary>
                /// 添加属性
                /// </summary>
                /// <param name="property">属性信息</param>
                /// <param name="method">函数信息</param>
                public void PushBox(propertyIndex property, MethodInfo method)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    if (isValueType) generator.Emit(OpCodes.Ldarga_S, 1);
                    else generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(method.IsFinal || !method.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, method);
                    method = GetMemberMethodInfo(property.Member.PropertyType);
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
                    dynamicMethod = new DynamicMethod("jsonMemberMapSerializer", null, new Type[] { typeof(memberMap), typeof(jsonSerializer), type, typeof(charStream) }, type, true);
                    generator = dynamicMethod.GetILGenerator();

                    generator.DeclareLocal(typeof(int));
                    generator.Emit(OpCodes.Ldc_I4_0);
                    generator.Emit(OpCodes.Stloc_0);

                    isValueType = type.IsValueType;
                }
                /// <summary>
                /// 添加成员
                /// </summary>
                /// <param name="name">成员名称</param>
                /// <param name="memberIndex"></param>
                /// <param name="end"></param>
                private void push(string name, int memberIndex, Label end)
                {
                    Label next = generator.DefineLabel(), value = generator.DefineLabel();
                    generator.memberMapIsMember(OpCodes.Ldarg_0, memberIndex);
                    generator.Emit(OpCodes.Brfalse_S, end);

                    generator.Emit(OpCodes.Ldloc_0);
                    generator.Emit(OpCodes.Brtrue_S, next);

                    char* nameChar = GetNamePool(name);
                    generator.Emit(OpCodes.Ldc_I4_1);
                    generator.Emit(OpCodes.Stloc_0);
                    generator.charStreamSimpleWriteNotNull(OpCodes.Ldarg_3, nameChar + 1, name.Length + 3);
                    generator.Emit(OpCodes.Br_S, value);

                    generator.MarkLabel(next);
                    generator.charStreamSimpleWriteNotNull(OpCodes.Ldarg_3, nameChar, name.Length + 4);

                    generator.MarkLabel(value);
                    generator.Emit(OpCodes.Ldarg_1);
                    if (isValueType) generator.Emit(OpCodes.Ldarga_S, 2);
                    else generator.Emit(OpCodes.Ldarg_2);
                }
                /// <summary>
                /// 添加字段
                /// </summary>
                /// <param name="field">字段信息</param>
                public void Push(fieldIndex field)
                {
                    Label end = generator.DefineLabel();
                    push(field.AnonymousName, field.MemberIndex, end);
                    generator.Emit(OpCodes.Ldfld, field.Member);
                    MethodInfo method = GetMemberMethodInfo(field.Member.FieldType);
                    generator.Emit(method.IsFinal || !method.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, method);
                    generator.MarkLabel(end);
                }
                /// <summary>
                /// 添加属性
                /// </summary>
                /// <param name="property">属性信息</param>
                /// <param name="method">函数信息</param>
                public void Push(propertyIndex property, MethodInfo method)
                {
                    Label end = generator.DefineLabel();
                    push(property.Member.Name, property.MemberIndex, end);
                    generator.Emit(method.IsFinal || !method.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, method);
                    method = GetMemberMethodInfo(property.Member.PropertyType);
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
            /// <summary>
            /// 名称数据信息集合
            /// </summary>
            private static readonly interlocked.dictionary<string, pointer> namePools = new interlocked.dictionary<string, pointer>();
            /// <summary>
            /// 获取名称数据
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public static char* GetNamePool(string name)
            {
                pointer pointer;
                if (namePools.TryGetValue(name, out pointer)) return pointer.Char;
                char* value = namePool.Get(name, 2, 2);
                *(int*)value = ',' + ((int)fastCSharp.web.ajax.Quote << 16);
                *(int*)(value + (2 + name.Length)) = fastCSharp.web.ajax.Quote + (':' << 16);
                namePools.Set(name, new pointer { Data = value });
                return value;
            }
#endif
            /// <summary>
            /// 获取字段成员集合
            /// </summary>
            /// <param name="fields"></param>
            /// <param name="typeAttribute">类型配置</param>
            /// <returns>字段成员集合</returns>
            public static subArray<fieldIndex> GetFields(fieldIndex[] fields, jsonSerialize typeAttribute)
            {
                subArray<fieldIndex> values = new subArray<fieldIndex>(fields.Length);
                foreach (fieldIndex field in fields)
                {
                    Type type = field.Member.FieldType;
                    if (!type.IsPointer && (!type.IsArray || type.GetArrayRank() == 1) && !field.IsIgnore)
                    {
                        jsonSerialize.member attribute = field.GetAttribute<jsonSerialize.member>(true, true);
                        if (typeAttribute.IsAllMember ? (attribute == null || attribute.IsSetup) : (attribute != null && attribute.IsSetup)) values.Add(field);
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
            public static subArray<keyValue<propertyIndex, MethodInfo>> GetProperties(propertyIndex[] properties, jsonSerialize typeAttribute)
            {
                subArray<keyValue<propertyIndex, MethodInfo>> values = new subArray<keyValue<propertyIndex, MethodInfo>>(properties.Length);
                foreach (propertyIndex property in properties)
                {
                    if (property.Member.CanRead)
                    {
                        Type type = property.Member.PropertyType;
                        if (!type.IsPointer && (!type.IsArray || type.GetArrayRank() == 1) && !property.IsIgnore)
                        {
                            jsonSerialize.member attribute = property.GetAttribute<jsonSerialize.member>(true, true);
                            if (typeAttribute.IsAllMember ? (attribute == null || attribute.IsSetup) : (attribute != null && attribute.IsSetup))
                            {
                                MethodInfo method = property.Member.GetGetMethod(true);
                                if (method != null && method.GetParameters().Length == 0) values.Add(new keyValue<propertyIndex, MethodInfo>(property, method));
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
            public static subArray<memberIndex> GetMembers(fieldIndex[] fieldIndexs, propertyIndex[] properties, jsonSerialize typeAttribute)
            {
                subArray<memberIndex> members = new subArray<memberIndex>(fieldIndexs.Length + properties.Length);
                foreach (fieldIndex field in fieldIndexs)
                {
                    Type type = field.Member.FieldType;
                    if (!type.IsPointer && (!type.IsArray || type.GetArrayRank() == 1) && !field.IsIgnore)
                    {
                        jsonSerialize.member attribute = field.GetAttribute<jsonSerialize.member>(true, true);
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
                            jsonSerialize.member attribute = property.GetAttribute<jsonSerialize.member>(true, true);
                            if (typeAttribute.IsAllMember ? (attribute == null || attribute.IsSetup) : (attribute != null && attribute.IsSetup))
                            {
                                MethodInfo method = property.Member.GetGetMethod(true);
                                if (method != null && method.GetParameters().Length == 0) members.Add(property);
                            }
                        }
                    }
                }
                return members;
            }
            /// <summary>
            /// 获取成员转换函数信息
            /// </summary>
            /// <param name="type">成员类型</param>
            /// <returns>成员转换函数信息</returns>
            internal static MethodInfo GetMemberMethodInfo(Type type)
            {
                MethodInfo methodInfo = jsonSerializer.getToJsonMethod(type);
                if (methodInfo != null) return methodInfo;
                if (type.IsArray) return GetArrayToJsoner(type.GetElementType());
                if (type.IsEnum) return GetEnumToJsoner(type);
                if (type.IsGenericType)
                {
                    Type genericType = type.GetGenericTypeDefinition();
                    if (genericType == typeof(Dictionary<,>)) return GetDictionaryToJsoner(type);
                    if (genericType == typeof(Nullable<>)) return GetNullableToJsoner(type);
                    if (genericType == typeof(KeyValuePair<,>)) return GetKeyValuePairToJsoner(type);
                }
                return GetCustomToJsoner(type) ?? GetIEnumerableToJsoner(type) ?? GetTypeToJsoner(type);
            }

            /// <summary>
            /// object转换调用委托信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, Action<jsonSerializer, object>> objectToJsoners = new interlocked.dictionary<Type, Action<jsonSerializer, object>>();
            /// <summary>
            /// 获取object转换调用委托信息
            /// </summary>
            /// <param name="type">真实类型</param>
            /// <returns>object转换调用委托信息</returns>
            public static Action<jsonSerializer, object> GetObjectToJsoner(Type type)
            {
                Action<jsonSerializer, object> method;
                if (objectToJsoners.TryGetValue(type, out method)) return method;
                method = (Action<jsonSerializer, object>)Delegate.CreateDelegate(typeof(Action<jsonSerializer, object>), toJsonObjectMethod.MakeGenericMethod(type));
                objectToJsoners.Set(type, method);
                return method;
            }
            /// <summary>
            /// 数组转换调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> arrayToJsoners = new interlocked.dictionary<Type, MethodInfo>();
            /// <summary>
            /// 获取数组转换委托调用函数信息
            /// </summary>
            /// <param name="type">数组类型</param>
            /// <returns>数组转换委托调用函数信息</returns>
            public static MethodInfo GetArrayToJsoner(Type type)
            {
                MethodInfo method;
                if (arrayToJsoners.TryGetValue(type, out method)) return method;
                arrayToJsoners.Set(type, method = arrayMethod.MakeGenericMethod(type));
                return method;
            }
            /// <summary>
            /// 枚举集合转换调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> enumerableToJsoners = new interlocked.dictionary<Type, MethodInfo>();
            /// <summary>
            /// 获取枚举集合转换委托调用函数信息
            /// </summary>
            /// <param name="type">枚举类型</param>
            /// <returns>枚举集合转换委托调用函数信息</returns>
            public static MethodInfo GetIEnumerableToJsoner(Type type)
            {
                MethodInfo method;
                if (enumerableToJsoners.TryGetValue(type, out method)) return method;
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
                                method = (type.IsValueType ? structEnumerableMethod : enumerableMethod).MakeGenericMethod(type, argumentType);
                                break;
                            }
                            parameters[0] = typeof(ICollection<>).MakeGenericType(argumentType);
                            constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, parameters, null);
                            if (constructorInfo != null)
                            {
                                method = (type.IsValueType ? structEnumerableMethod : enumerableMethod).MakeGenericMethod(type, argumentType);
                                break;
                            }
                            parameters[0] = typeof(IEnumerable<>).MakeGenericType(argumentType);
                            constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, parameters, null);
                            if (constructorInfo != null)
                            {
                                method = (type.IsValueType ? structEnumerableMethod : enumerableMethod).MakeGenericMethod(type, argumentType);
                                break;
                            }
                            parameters[0] = argumentType.MakeArrayType();
                            constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, parameters, null);
                            if (constructorInfo != null)
                            {
                                method = (type.IsValueType ? structEnumerableMethod : enumerableMethod).MakeGenericMethod(type, argumentType);
                                break;
                            }
                        }
                        else if (genericType == typeof(IDictionary<,>))
                        {
                            ConstructorInfo constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { interfaceType }, null);
                            if (constructorInfo != null)
                            {
                                method = (type.IsValueType ? structEnumerableMethod : enumerableMethod).MakeGenericMethod(type, typeof(KeyValuePair<,>).MakeGenericType(interfaceType.GetGenericArguments()));
                                break;
                            }
                        }
                    }
                }
                enumerableToJsoners.Set(type, method);
                return method;
            }
            /// <summary>
            /// 字典转换调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> dictionaryToJsoners = new interlocked.dictionary<Type, MethodInfo>();
            /// <summary>
            /// 获取字典转换委托调用函数信息
            /// </summary>
            /// <param name="type">枚举类型</param>
            /// <returns>字典转换委托调用函数信息</returns>
            public static MethodInfo GetDictionaryToJsoner(Type type)
            {
                MethodInfo method;
                if (dictionaryToJsoners.TryGetValue(type, out method)) return method;
                Type[] types = type.GetGenericArguments();
                if (types[0] == typeof(string)) method = stringDictionaryMethod.MakeGenericMethod(types[1]);
                else method = dictionaryMethod.MakeGenericMethod(types);
                dictionaryToJsoners.Set(type, method);
                return method;
            }
            /// <summary>
            /// 可空类型转换调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> nullableToJsons = new interlocked.dictionary<Type, MethodInfo>();
            /// <summary>
            /// 获取可空类型转换委托调用函数信息
            /// </summary>
            /// <param name="type">枚举类型</param>
            /// <returns>可空类型转换委托调用函数信息</returns>
            public static MethodInfo GetNullableToJsoner(Type type)
            {
                MethodInfo method;
                if (nullableToJsons.TryGetValue(type, out method)) return method;
                nullableToJsons.Set(type, method = nullableToJsonMethod.MakeGenericMethod(type.GetGenericArguments()));
                return method;
            }
            /// <summary>
            /// 键值对转换调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> keyValuePairToJsons = new interlocked.dictionary<Type, MethodInfo>();
            /// <summary>
            /// 获取键值对转换委托调用函数信息
            /// </summary>
            /// <param name="type">枚举类型</param>
            /// <returns>键值对转换委托调用函数信息</returns>
            public static MethodInfo GetKeyValuePairToJsoner(Type type)
            {
                MethodInfo method;
                if (keyValuePairToJsons.TryGetValue(type, out method)) return method;
                keyValuePairToJsons.Set(type, method = keyValuePairToJsonMethod.MakeGenericMethod(type.GetGenericArguments()));
                return method;
            }
            /// <summary>
            /// 枚举转换调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> enumToJsoners = new interlocked.dictionary<Type, MethodInfo>();
            /// <summary>
            /// 获取枚举转换委托调用函数信息
            /// </summary>
            /// <param name="type">数组类型</param>
            /// <returns>枚举转换委托调用函数信息</returns>
            public static MethodInfo GetEnumToJsoner(Type type)
            {
                MethodInfo method;
                if (enumToJsoners.TryGetValue(type, out method)) return method;
                enumToJsoners.Set(type, method = enumToStringMethod.MakeGenericMethod(type));
                return method;
            }
            /// <summary>
            /// 未知类型转换调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> typeToJsoners = new interlocked.dictionary<Type, MethodInfo>();
            /// <summary>
            /// 未知类型枚举转换委托调用函数信息
            /// </summary>
            /// <param name="type">数组类型</param>
            /// <returns>未知类型转换委托调用函数信息</returns>
            public static MethodInfo GetTypeToJsoner(Type type)
            {
                MethodInfo method;
                if (typeToJsoners.TryGetValue(type, out method)) return method;
                typeToJsoners.Set(type, method = type.IsValueType ? structToJsonMethod.MakeGenericMethod(type) : classToJsonMethod.MakeGenericMethod(type));
                return method;
            }
            /// <summary>
            /// 自定义转换调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> customToJsoners = new interlocked.dictionary<Type, MethodInfo>();
            /// <summary>
            /// 自定义枚举转换委托调用函数信息
            /// </summary>
            /// <param name="type">数组类型</param>
            /// <returns>自定义转换委托调用函数信息</returns>
            public static MethodInfo GetCustomToJsoner(Type type)
            {
                MethodInfo method;
                if (customToJsoners.TryGetValue(type, out method)) return method;
                foreach (fastCSharp.code.attributeMethod methodInfo in fastCSharp.code.attributeMethod.GetStatic(type))
                {
                    if (methodInfo.Method.ReturnType == typeof(void))
                    {
                        ParameterInfo[] parameters = methodInfo.Method.GetParameters();
                        if (parameters.Length == 2 && parameters[0].ParameterType == typeof(jsonSerializer) && parameters[1].ParameterType == type)
                        {
                            if (methodInfo.GetAttribute<jsonSerialize.custom>(true) != null)
                            {
                                method = methodInfo.Method;
                                break;
                            }
                        }
                    }
                }
                customToJsoners.Set(type, method);
                return method;
            }
        }
        /// <summary>
        /// 对象转换JSON字符串
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        internal static class typeToJsoner<valueType>
        {
            /// <summary>
            /// 成员转换
            /// </summary>
            private static readonly Action<jsonSerializer, valueType> memberToJsoner;
            /// <summary>
            /// 成员转换
            /// </summary>
            private static readonly Action<memberMap, jsonSerializer, valueType, charStream> memberMapToJsoner;
            /// <summary>
            /// 转换委托
            /// </summary>
            private static readonly Action<jsonSerializer, valueType> defaultToJsoner;
            /// <summary>
            /// JSON序列化类型配置
            /// </summary>
            private static readonly jsonSerialize attribute;
            /// <summary>
            /// 客户端视图类型名称
            /// </summary>
            private static readonly string viewClientTypeName;
            /// <summary>
            /// 是否值类型
            /// </summary>
            private static readonly bool isValueType;
            /// <summary>
            /// 对象转换JSON字符串
            /// </summary>
            /// <param name="toJsoner">对象转换JSON字符串</param>
            /// <param name="value">数据对象</param>
            internal static void ToJson(jsonSerializer toJsoner, valueType value)
            {
                if (isValueType) StructToJson(toJsoner, value);
                else toJson(toJsoner, value);
            }
            /// <summary>
            /// 对象转换JSON字符串
            /// </summary>
            /// <param name="toJsoner">对象转换JSON字符串</param>
            /// <param name="value">数据对象</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal static void StructToJson(jsonSerializer toJsoner, valueType value)
            {
                if (defaultToJsoner == null) MemberToJson(toJsoner, value);
                else defaultToJsoner(toJsoner, value);
            }
            /// <summary>
            /// 引用类型对象转换JSON字符串
            /// </summary>
            /// <param name="toJsoner">对象转换JSON字符串</param>
            /// <param name="value">数据对象</param>
            internal static void ClassToJson(jsonSerializer toJsoner, valueType value)
            {
                if (defaultToJsoner == null)
                {
                    if (toJsoner.push(value))
                    {
                        MemberToJson(toJsoner, value);
                        toJsoner.pop();
                    }
                }
                else defaultToJsoner(toJsoner, value);
            }
            /// <summary>
            /// 引用类型对象转换JSON字符串
            /// </summary>
            /// <param name="toJsoner">对象转换JSON字符串</param>
            /// <param name="value">数据对象</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private static void toJson(jsonSerializer toJsoner, valueType value)
            {
                if (value == null) fastCSharp.web.ajax.WriteNull(toJsoner.CharStream);
                else ClassToJson(toJsoner, value);
            }
            /// <summary>
            /// 值类型对象转换JSON字符串
            /// </summary>
            /// <param name="toJsoner">对象转换JSON字符串</param>
            /// <param name="value">数据对象</param>
            internal static void MemberToJson(jsonSerializer toJsoner, valueType value)
            {
                charStream jsonStream = toJsoner.CharStream;
                config config = toJsoner.Config;
                byte isView;
                if (viewClientTypeName != null && config.IsViewClientType)
                {
                    jsonStream.SimpleWriteNotNull(viewClientTypeName);
                    isView = 1;
                }
                else
                {
                    jsonStream.PrepLength(2);
                    jsonStream.UnsafeWrite('{');
                    isView = 0;
                }
                memberMap memberMap = config.MemberMap;
                if (memberMap == null) memberToJsoner(toJsoner, value);
                else if (memberMap.Type == memberMap<valueType>.TypeInfo)
                {
                    config.MemberMap = null;
                    try
                    {
                        memberMapToJsoner(memberMap, toJsoner, value, jsonStream);
                    }
                    finally { config.MemberMap = memberMap; }
                }
                else
                {
                    config.Warning = warning.MemberMap;
                    if (config.IsMemberMapErrorLog) log.Error.Add("Json序列化成员位图类型匹配失败", null, true);
                    if (config.IsMemberMapErrorToDefault) memberToJsoner(toJsoner, value);
                }
                if (isView == 0) jsonStream.Write('}');
                else
                {
                    *(int*)jsonStream.GetPrepLengthCurrent(2) = '}' + (')' << 16);
                    jsonStream.UnsafeAddLength(2);
                }
            }
            /// <summary>
            /// 数组转换
            /// </summary>
            /// <param name="toJsoner">对象转换JSON字符串</param>
            /// <param name="array">数组对象</param>
            internal static void Array(jsonSerializer toJsoner, valueType[] array)
            {
                charStream jsonStream = toJsoner.CharStream;
                jsonStream.Write('[');
                byte isFirst = 1;
                if (isValueType)
                {
                    foreach (valueType value in array)
                    {
                        if (isFirst == 0) jsonStream.Write(',');
                        StructToJson(toJsoner, value);
                        isFirst = 0;
                    }
                }
                else
                {
                    foreach (valueType value in array)
                    {
                        if (isFirst == 0) jsonStream.Write(',');
                        toJson(toJsoner, value);
                        isFirst = 0;
                    }
                }
                jsonStream.Write(']');
            }
            /// <summary>
            /// 枚举集合转换
            /// </summary>
            /// <param name="toJsoner">对象转换JSON字符串</param>
            /// <param name="values">枚举集合</param>
            internal static void Enumerable(jsonSerializer toJsoner, IEnumerable<valueType> values)
            {
                charStream jsonStream = toJsoner.CharStream;
                jsonStream.Write('[');
                byte isFirst = 1;
                if (isValueType)
                {
                    foreach (valueType value in values)
                    {
                        if (isFirst == 0) jsonStream.Write(',');
                        StructToJson(toJsoner, value);
                        isFirst = 0;
                    }
                }
                else
                {
                    foreach (valueType value in values)
                    {
                        if (isFirst == 0) jsonStream.Write(',');
                        toJson(toJsoner, value);
                        isFirst = 0;
                    }
                }
                jsonStream.Write(']');
            }
            /// <summary>
            /// 字典转换
            /// </summary>
            /// <param name="toJsoner">对象转换JSON字符串</param>
            /// <param name="dictionary">数据对象</param>
            internal static void Dictionary<dictionaryValueType>(jsonSerializer toJsoner, Dictionary<valueType, dictionaryValueType> dictionary)
            {
                charStream jsonStream = toJsoner.CharStream;
                byte isFirst = 1;
                if (toJsoner.Config.IsDictionaryToObject)
                {
                    jsonStream.Write('{');
                    foreach (KeyValuePair<valueType, dictionaryValueType> value in dictionary)
                    {
                        if (isFirst == 0) jsonStream.Write(',');
                        typeToJsoner<valueType>.ToJson(toJsoner, value.Key);
                        jsonStream.Write(':');
                        typeToJsoner<dictionaryValueType>.ToJson(toJsoner, value.Value);
                        isFirst = 0;
                    }
                    jsonStream.Write('}');
                }
                else
                {
                    jsonStream.Write('[');
                    foreach (KeyValuePair<valueType, dictionaryValueType> value in dictionary)
                    {
                        if (isFirst == 0) jsonStream.Write(',');
                        KeyValuePair(toJsoner, value);
                        isFirst = 0;
                    }
                    jsonStream.Write(']');
                }
            }
            /// <summary>
            /// 字典转换
            /// </summary>
            /// <param name="toJsoner">对象转换JSON字符串</param>
            /// <param name="dictionary">字典</param>
            internal static void StringDictionary(jsonSerializer toJsoner, Dictionary<string, valueType> dictionary)
            {
                charStream jsonStream = toJsoner.CharStream;
                jsonStream.Write('{');
                byte isFirst = 1;
                if (isValueType)
                {
                    foreach (KeyValuePair<string, valueType> value in dictionary)
                    {
                        if (isFirst == 0) jsonStream.Write(',');
                        fastCSharp.web.ajax.ToString(value.Key, jsonStream);
                        jsonStream.Write(':');
                        StructToJson(toJsoner, value.Value);
                        isFirst = 0;
                    }
                }
                else
                {
                    foreach (KeyValuePair<string, valueType> value in dictionary)
                    {
                        if (isFirst == 0) jsonStream.Write(',');
                        fastCSharp.web.ajax.ToString(value.Key, jsonStream);
                        jsonStream.Write(':');
                        toJson(toJsoner, value.Value);
                        isFirst = 0;
                    }
                }
                jsonStream.Write('}');
            }
            /// <summary>
            /// 字典转换
            /// </summary>
            /// <param name="toJsoner">对象转换JSON字符串</param>
            /// <param name="value">数据对象</param>
            internal static void KeyValuePair<dictionaryValueType>(jsonSerializer toJsoner, KeyValuePair<valueType, dictionaryValueType> value)
            {
                charStream jsonStream = toJsoner.CharStream;
                byte* data = (byte*)jsonStream.GetPrepLengthCurrent(21);
                *(int*)data = '{' + (fastCSharp.web.ajax.Quote << 16);
                *(int*)(data + sizeof(char) * 2) = 'K' + ('e' << 16);
                *(int*)(data + sizeof(char) * 4) = 'y' + (fastCSharp.web.ajax.Quote << 16);
                *(char*)(data + sizeof(char) * 6) = ':';
                jsonStream.UnsafeAddLength(7);
                typeToJsoner<valueType>.ToJson(toJsoner, value.Key);
                data = (byte*)jsonStream.GetPrepLengthCurrent(12);
                *(int*)data = ',' + (fastCSharp.web.ajax.Quote << 16);
                *(int*)(data + sizeof(char) * 2) = 'V' + ('a' << 16);
                *(int*)(data + sizeof(char) * 4) = 'l' + ('u' << 16);
                *(int*)(data + sizeof(char) * 6) = 'e' + (fastCSharp.web.ajax.Quote << 16);
                *(char*)(data + sizeof(char) * 8) = ':';
                jsonStream.UnsafeAddLength(9);
                typeToJsoner<dictionaryValueType>.ToJson(toJsoner, value.Value);
                jsonStream.Write('}');
            }
            /// <summary>
            /// 不支持多维数组
            /// </summary>
            /// <param name="toJsoner">对象转换JSON字符串</param>
            /// <param name="value">数据对象</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private static void arrayManyRank(jsonSerializer toJsoner, valueType value)
            {
                fastCSharp.web.ajax.WriteArray(toJsoner.CharStream);
            }
            /// <summary>
            /// 不支持对象转换null
            /// </summary>
            /// <param name="toJsoner">对象转换JSON字符串</param>
            /// <param name="value">数据对象</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private static void toNull(jsonSerializer toJsoner, valueType value)
            {
                fastCSharp.web.ajax.WriteNull(toJsoner.CharStream);
            }
            /// <summary>
            /// 枚举转换字符串
            /// </summary>
            /// <param name="toJsoner">对象转换JSON字符串</param>
            /// <param name="value">数据对象</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private static void enumToString(jsonSerializer toJsoner, valueType value)
            {
                toJsoner.enumToString(value);
            }
            /// <summary>
            /// 获取字段成员集合
            /// </summary>
            /// <returns>字段成员集合</returns>
            public static subArray<memberIndex> GetMembers()
            {
                if (memberToJsoner == null) return default(subArray<memberIndex>);
                return typeToJsoner.GetMembers(memberIndexGroup<valueType>.GetFields(attribute.MemberFilter), memberIndexGroup<valueType>.GetProperties(attribute.MemberFilter), attribute);
            }
            static typeToJsoner()
            {
                Type type = typeof(valueType);
                MethodInfo methodInfo = jsonSerializer.getToJsonMethod(type);
                if (methodInfo != null)
                {
#if NOJIT
                    defaultToJsoner = new methodSerializer(methodInfo).Serialize;
#else
                    DynamicMethod dynamicMethod = new DynamicMethod("toJsoner", typeof(void), new Type[] { typeof(jsonSerializer), type }, true);
                    dynamicMethod.InitLocals = true;
                    ILGenerator generator = dynamicMethod.GetILGenerator();
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Call, methodInfo);
                    generator.Emit(OpCodes.Ret);
                    defaultToJsoner = (Action<jsonSerializer, valueType>)dynamicMethod.CreateDelegate(typeof(Action<jsonSerializer, valueType>));
#endif
                    isValueType = true;
                    return;
                }
                if (type.IsArray)
                {
                    if (type.GetArrayRank() == 1) defaultToJsoner = (Action<jsonSerializer, valueType>)Delegate.CreateDelegate(typeof(Action<jsonSerializer, valueType>), typeToJsoner.GetArrayToJsoner(type.GetElementType()));
                    else defaultToJsoner = arrayManyRank;
                    isValueType = true;
                    return;
                }
                if (type.IsEnum)
                {
                    defaultToJsoner = enumToString;
                    isValueType = true;
                    return;
                }
                if (type.IsPointer)
                {
                    defaultToJsoner = toNull;
                    isValueType = true;
                    return;
                }
                if (type.IsGenericType)
                {
                    Type genericType = type.GetGenericTypeDefinition();
                    if (genericType == typeof(Dictionary<,>))
                    {
                        defaultToJsoner = (Action<jsonSerializer, valueType>)Delegate.CreateDelegate(typeof(Action<jsonSerializer, valueType>), typeToJsoner.GetDictionaryToJsoner(type));
                        isValueType = true;
                        return;
                    }
                    if (genericType == typeof(Nullable<>))
                    {
                        defaultToJsoner = (Action<jsonSerializer, valueType>)Delegate.CreateDelegate(typeof(Action<jsonSerializer, valueType>), typeToJsoner.GetNullableToJsoner(type));
                        isValueType = true;
                        return;
                    }
                    if (genericType == typeof(KeyValuePair<,>))
                    {
                        defaultToJsoner = (Action<jsonSerializer, valueType>)Delegate.CreateDelegate(typeof(Action<jsonSerializer, valueType>), typeToJsoner.GetKeyValuePairToJsoner(type));
                        isValueType = true;
                        return;
                    }
                }
                if ((methodInfo = typeToJsoner.GetCustomToJsoner(type)) != null
                    || (methodInfo = typeToJsoner.GetIEnumerableToJsoner(type)) != null)
                {
                    defaultToJsoner = (Action<jsonSerializer, valueType>)Delegate.CreateDelegate(typeof(Action<jsonSerializer, valueType>), methodInfo);
                    isValueType = true;
                }
                else
                {
                    Type attributeType;
                    attribute = type.customAttribute<jsonSerialize>(out attributeType, true) ?? (type.Name[0] == '<' ? jsonSerialize.AnonymousTypeMember : jsonSerialize.AllMember);
                    if (type.IsValueType) isValueType = true;
                    else if (attribute != jsonSerialize.AllMember && attributeType != type)
                    {
                        for (Type baseType = type.BaseType; baseType != typeof(object); baseType = baseType.BaseType)
                        {
                            jsonSerialize baseAttribute = fastCSharp.code.typeAttribute.GetAttribute<jsonSerialize>(baseType, false, true);
                            if (baseAttribute != null)
                            {
                                if (baseAttribute.IsBaseType)
                                {
                                    methodInfo = baseToJsonMethod.MakeGenericMethod(baseType, type);
                                    defaultToJsoner = (Action<jsonSerializer, valueType>)Delegate.CreateDelegate(typeof(Action<jsonSerializer, valueType>), methodInfo);
                                    return;
                                }
                                break;
                            }
                        }
                    }
                    subArray<fieldIndex> fields = typeToJsoner.GetFields(memberIndexGroup<valueType>.GetFields(attribute.MemberFilter), attribute);
                    subArray<keyValue<propertyIndex, MethodInfo>> properties = typeToJsoner.GetProperties(memberIndexGroup<valueType>.GetProperties(attribute.MemberFilter), attribute);
                    bool isBox = false;
                    if (type.IsValueType && fields.length + properties.length == 1)
                    {
                        boxSerialize boxSerialize = fastCSharp.code.typeAttribute.GetAttribute<boxSerialize>(type, true, true);
                        if (boxSerialize != null && boxSerialize.IsJson) isBox = true;
                    }
                    fastCSharp.code.cSharp.webView.clientType clientType = isBox ? null : fastCSharp.code.typeAttribute.GetAttribute<fastCSharp.code.cSharp.webView.clientType>(type, true, true);
                    if (clientType != null)
                    {
                        if (clientType.MemberName == null) viewClientTypeName = "new " + clientType.GetClientName(type) + "({";
                        else viewClientTypeName = clientType.GetClientName(type) + ".Get({";
                    }
#if NOJIT
                    if (isBox) defaultToJsoner = memberToJsoner = new serializer(ref fields, ref properties).SerializeBox;
                    else
                    {
                        memberToJsoner = new serializer(ref fields, ref properties).Serialize;
                        memberMapToJsoner = new memberMapSerializer(ref fields, ref properties).Serialize;
                    } 
#else
                    typeToJsoner.memberDynamicMethod dynamicMethod = new typeToJsoner.memberDynamicMethod(type);
                    typeToJsoner.memberMapDynamicMethod memberMapDynamicMethod = isBox ? default(typeToJsoner.memberMapDynamicMethod) : new typeToJsoner.memberMapDynamicMethod(type);
                    foreach (fieldIndex member in fields)
                    {
                        if (isBox) dynamicMethod.PushBox(member);
                        else
                        {
                            dynamicMethod.Push(member);
                            memberMapDynamicMethod.Push(member);
                        }
                    }
                    foreach (keyValue<propertyIndex, MethodInfo> member in properties)
                    {
                        if (isBox) dynamicMethod.PushBox(member.Key, member.Value);
                        else
                        {
                            dynamicMethod.Push(member.Key, member.Value);
                            memberMapDynamicMethod.Push(member.Key, member.Value);
                        }
                    }
                    memberToJsoner = (Action<jsonSerializer, valueType>)dynamicMethod.Create<Action<jsonSerializer, valueType>>();
                    if (isBox) defaultToJsoner = memberToJsoner;
                    else memberMapToJsoner = (Action<memberMap, jsonSerializer, valueType, charStream>)memberMapDynamicMethod.Create<Action<memberMap, jsonSerializer, valueType, charStream>>();
#endif
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
                public void Serialize(jsonSerializer serializer, valueType value)
                {
                    method.Invoke(serializer, new object[] { value });
                }
            }
            /// <summary>
            /// 序列化（反射模式）
            /// </summary>
            private class serializer
            {
                /// <summary>
                /// 字段信息
                /// </summary>
                protected struct field
                {
                    /// <summary>
                    /// 字段信息
                    /// </summary>
                    public FieldInfo Field;
                    /// <summary>
                    /// 字段输出名称
                    /// </summary>
                    public string Name;
                    /// <summary>
                    /// 序列化函数
                    /// </summary>
                    public pub.methodParameter1 SerializeMethod;
                    /// <summary>
                    /// 成员位图编号
                    /// </summary>
                    public int MemberIndex;
                    /// <summary>
                    /// 设置字段信息
                    /// </summary>
                    /// <param name="field"></param>
                    public void Set(fieldIndex field)
                    {
                        Field = field.Member;
                        Name = field.AnonymousName;
                        MemberIndex = field.MemberIndex;
                        SerializeMethod = new pub.methodParameter1(typeToJsoner.GetMemberMethodInfo(Field.FieldType));
                    }
                }
                /// <summary>
                /// 属性信息
                /// </summary>
                protected struct property
                {
                    /// <summary>
                    /// 属性信息
                    /// </summary>
                    public PropertyInfo Property;
                    /// <summary>
                    /// 获取函数
                    /// </summary>
                    public MethodInfo GetMethod;
                    /// <summary>
                    /// 序列化函数
                    /// </summary>
                    public pub.methodParameter1 SerializeMethod;
                    /// <summary>
                    /// 成员位图编号
                    /// </summary>
                    public int MemberIndex;
                    /// <summary>
                    /// 设置属性信息
                    /// </summary>
                    /// <param name="property"></param>
                    public void Set(keyValue<propertyIndex, MethodInfo> property)
                    {
                        Property = property.Key.Member;
                        GetMethod = Property.GetGetMethod(true);
                        MemberIndex = property.Key.MemberIndex;
                        SerializeMethod = new pub.methodParameter1(typeToJsoner.GetMemberMethodInfo(Property.PropertyType));
                    }
                }
                /// <summary>
                /// 字段集合
                /// </summary>
                protected field[] fields;
                /// <summary>
                /// 属性集合
                /// </summary>
                protected property[] properties;
                /// <summary>
                /// 成员序列化
                /// </summary>
                /// <param name="fields"></param>
                /// <param name="properties"></param>
                public serializer(ref subArray<fieldIndex> fields, ref subArray<keyValue<propertyIndex, MethodInfo>> properties)
                {
                    this.fields = new field[fields.Count];
                    int index = 0;
                    foreach (fieldIndex field in fields) this.fields[index++].Set(field);
                    this.properties = new property[properties.Count];
                    index = 0;
                    foreach (keyValue<propertyIndex, MethodInfo> property in properties) this.properties[index++].Set(property);
                }
                /// <summary>
                /// 字段序列化
                /// </summary>
                /// <param name="serializer"></param>
                /// <param name="value"></param>
                /// <param name="field"></param>
                /// <param name="parameters"></param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                protected static void serialize(jsonSerializer serializer, object value, field field, ref object[] parameters)
                {
                    if (parameters == null) parameters = new object[1];
                    parameters[0] = field.Field.GetValue(value);
                    charStream charStream = serializer.CharStream;
                    charStream.PrepLength(field.Name.Length + 5);
                    charStream.UnsafeWrite(fastCSharp.web.ajax.Quote);
                    charStream.UnsafeSimpleWrite(field.Name);
                    charStream.UnsafeWrite(fastCSharp.web.ajax.Quote);
                    charStream.UnsafeWrite(':');
                    field.SerializeMethod.Invoke(serializer, parameters);
                }
                /// <summary>
                /// 属性序列化
                /// </summary>
                /// <param name="serializer"></param>
                /// <param name="value"></param>
                /// <param name="property"></param>
                /// <param name="parameters"></param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                protected static void serialize(jsonSerializer serializer, object value, property property, ref object[] parameters)
                {
                    if (parameters == null) parameters = new object[1];
                    PropertyInfo propertyInfo = property.Property;
                    parameters[0] = property.GetMethod.Invoke(value, null);
                    charStream charStream = serializer.CharStream;
                    charStream.PrepLength(propertyInfo.Name.Length + 5);
                    charStream.UnsafeWrite(fastCSharp.web.ajax.Quote);
                    charStream.UnsafeSimpleWrite(propertyInfo.Name);
                    charStream.UnsafeWrite(fastCSharp.web.ajax.Quote);
                    charStream.UnsafeWrite(':');
                    property.SerializeMethod.Invoke(serializer, parameters);
                }
                /// <summary>
                /// 序列化
                /// </summary>
                /// <param name="serializer"></param>
                /// <param name="value"></param>
                public void Serialize(jsonSerializer serializer, valueType value)
                {
                    object[] parameters = null;
                    object objectValue = value;
                    charStream charStream = serializer.CharStream;
                    byte isNext = 0;
                    foreach (field field in fields)
                    {
                        if (isNext == 0) isNext = 1;
                        else charStream.Write(',');
                        serialize(serializer, objectValue, field, ref parameters);
                    }
                    foreach (property property in properties)
                    {
                        if (isNext == 0) isNext = 1;
                        else charStream.Write(',');
                        serialize(serializer, objectValue, property, ref parameters);
                    }
                }
                /// <summary>
                /// 序列化
                /// </summary>
                /// <param name="serializer"></param>
                /// <param name="value"></param>
                public void SerializeBox(jsonSerializer serializer, valueType value)
                {
                    if (fields.Length == 0)
                    {
                        property property = properties[0];
                        property.SerializeMethod.Invoke(serializer, new object[] { property.GetMethod.Invoke(value, null) });
                    }
                    else
                    {
                        field field = fields[0];
                        field.SerializeMethod.Invoke(serializer, new object[] { field.Field.GetValue(value) });
                    }
                }
            }
            /// <summary>
            /// 成员序列化（反射模式）
            /// </summary>
            private sealed class memberMapSerializer : serializer
            {
                /// <summary>
                /// 成员序列化
                /// </summary>
                /// <param name="fields"></param>
                /// <param name="properties"></param>
                public memberMapSerializer(ref subArray<fieldIndex> fields, ref subArray<keyValue<propertyIndex, MethodInfo>> properties) : base(ref fields, ref properties) { }
                /// <summary>
                /// 序列化
                /// </summary>
                /// <param name="memberMap"></param>
                /// <param name="serializer"></param>
                /// <param name="value"></param>
                /// <param name="charStream"></param>
                public void Serialize(memberMap memberMap, jsonSerializer serializer, valueType value, charStream charStream)
                {
                    object[] parameters = null;
                    object objectValue = value;
                    byte isNext = 0;
                    foreach (field field in fields)
                    {
                        if (memberMap.IsMember(field.MemberIndex))
                        {
                            if (isNext == 0) isNext = 1;
                            else charStream.Write(',');
                            serialize(serializer, objectValue, field, ref parameters);
                        }
                    }
                    foreach (property property in properties)
                    {
                        if (memberMap.IsMember(property.MemberIndex))
                        {
                            if (isNext == 0) isNext = 1;
                            else charStream.Write(',');
                            serialize(serializer, objectValue, property, ref parameters);
                        }
                    }
                }
            }
#endif
        }
        /// <summary>
        /// 配置参数
        /// </summary>
        internal config Config;
        /// <summary>
        /// 配置参数
        /// </summary>
        public config UnsafeConfig
        {
            get { return Config; }
        }
        /// <summary>
        /// 对象编号
        /// </summary>
        private Dictionary<objectReference, string> objectIndexs;
        /// <summary>
        /// 是否调用循环引用处理函数
        /// </summary>
        private bool isLoopObject;
        /// <summary>
        /// 对象转换JSON字符串
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="value">数据对象</param>
        /// <param name="config">配置参数</param>
        /// <returns>Json字符串</returns>
        private string toJson<valueType>(valueType value, config config)
        {
            Config = config;
            pointer buffer = fastCSharp.unmanagedPool.StreamBuffers.Get();
            try
            {
                CharStream.UnsafeReset((byte*)buffer.Char, fastCSharp.unmanagedPool.StreamBuffers.Size);
                using (CharStream)
                {
                    toJson(value);
                    return fastCSharp.web.ajax.FormatJavascript(CharStream);
                }
            }
            finally
            {
                fastCSharp.unmanagedPool.StreamBuffers.Push(ref buffer);
            }
        }
        /// <summary>
        /// 对象转换JSON字符串
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="value">数据对象</param>
        /// <param name="jsonStream">Json输出缓冲区</param>
        /// <param name="config">配置参数</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toJson<valueType>(valueType value, charStream jsonStream, config config)
        {
            Config = config;
            CharStream.From(jsonStream);
            try
            {
                toJson(value);
            }
            finally { jsonStream.From(CharStream); }
        }
        /// <summary>
        /// 对象转换JSON字符串
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="value">数据对象</param>
        private void toJson<valueType>(valueType value)
        {
            if (Config.GetLoopObject == null || Config.SetLoopObject == null)
            {
                if (Config.GetLoopObject != null) Config.Warning = warning.LessSetLoop;
                else if (Config.SetLoopObject != null) Config.Warning = warning.LessGetLoop;
                else Config.Warning = warning.None;
                isLoopObject = false;
                if (Config.CheckLoopDepth <= 0)
                {
                    checkLoopDepth = 0;
                    if (forefather == null) forefather = new object[sizeof(int)];
                }
                else checkLoopDepth = Config.CheckLoopDepth;
            }
            else
            {
                isLoopObject = true;
                if (objectIndexs == null) objectIndexs = dictionary<objectReference>.Create<string>();
                checkLoopDepth = Config.CheckLoopDepth <= 0 ? fastCSharp.config.appSetting.SerializeDepth : Config.CheckLoopDepth;
            }
            typeToJsoner<valueType>.ToJson(this, value);
        }
        /// <summary>
        /// 对象转换JSON字符串
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="value"></param>
        /// <param name="config"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void UnsafeToJson<valueType>(valueType value, config config)
        {
            if (value == null) fastCSharp.web.ajax.WriteNull(CharStream);
            else
            {
                Config = config ?? defaultConfig;
                toJson(value);
            }
        }
        /// <summary>
        /// 对象转换JSON字符串
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="value"></param>
        /// <param name="config"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void UnsafeToJsonNotNull<valueType>(valueType value, config config)
        {
            Config = config ?? defaultConfig;
            toJson(value);
        }
        /// <summary>
        /// 对象转换JSON字符串
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="value"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void UnsafeToJson<valueType>(valueType value)
        {
            if (value == null) fastCSharp.web.ajax.WriteNull(CharStream);
            else typeToJsoner<valueType>.ToJson(this, value);
        }
        /// <summary>
        /// 对象转换JSON字符串
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="value"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void UnsafeToJsonNotNull<valueType>(valueType value)
        {
            typeToJsoner<valueType>.ToJson(this, value);
        }
        /// <summary>
        /// 写入对象名称
        /// </summary>
        /// <param name="name"></param>
        public void UnsafeWriteFirstName(string name)
        {
            CharStream.PrepLength(name.Length + (5 + 1));
            CharStream.UnsafeWrite('{');
            CharStream.UnsafeWrite(fastCSharp.web.ajax.Quote);
            CharStream.UnsafeSimpleWrite(name);
            CharStream.UnsafeWrite(fastCSharp.web.ajax.Quote);
            CharStream.UnsafeWrite(':');
        }
        /// <summary>
        /// 写入对象名称
        /// </summary>
        /// <param name="name"></param>
        public void UnsafeWriteNextName(string name)
        {
            CharStream.PrepLength(name.Length + (5 + 1));
            CharStream.UnsafeWrite(',');
            CharStream.UnsafeWrite(fastCSharp.web.ajax.Quote);
            CharStream.UnsafeSimpleWrite(name);
            CharStream.UnsafeWrite(fastCSharp.web.ajax.Quote);
            CharStream.UnsafeWrite(':');
        }
        /// <summary>
        /// 进入对象节点
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        /// <param name="value">数据对象</param>
        /// <returns>是否继续处理对象</returns>
        private bool push<valueType>(valueType value)
        {
            if (checkLoopDepth == 0)
            {
                if (forefatherCount != 0)
                {
                    int count = forefatherCount;
                    object objectValue = value;
                    foreach (object arrayValue in forefather)
                    {
                        if (arrayValue == objectValue)
                        {
                            fastCSharp.web.ajax.WriteObject(CharStream);
                            return false;
                        }
                        if (--count == 0) break;
                    }
                }
                if (forefatherCount == forefather.Length)
                {
                    object[] newValues = new object[forefatherCount << 1];
                    forefather.CopyTo(newValues, 0);
                    forefather = newValues;
                }
                forefather[forefatherCount++] = value;
            }
            else
            {
                if (--checkLoopDepth == 0) fastCSharp.log.Default.Throw(log.exceptionType.IndexOutOfRange);
                if (isLoopObject)
                {
                    string index;
                    if (objectIndexs.TryGetValue(new objectReference { Value = value }, out index))
                    {
                        CharStream.PrepLength(Config.GetLoopObject.Length + index.Length + (2 + 2));
                        CharStream.UnsafeSimpleWrite(Config.GetLoopObject);
                        CharStream.UnsafeWrite('(');
                        CharStream.UnsafeSimpleWrite(index);
                        CharStream.UnsafeWrite(')');
                        return false;
                    }
                    objectIndexs.Add(new objectReference { Value = value }, index = objectIndexs.Count.toString());
                    CharStream.PrepLength(Config.SetLoopObject.Length + index.Length + (2 + 2));
                    CharStream.UnsafeSimpleWrite(Config.SetLoopObject);
                    CharStream.UnsafeWrite('(');
                    CharStream.UnsafeSimpleWrite(index);
                    CharStream.UnsafeWrite(',');
                }
            }
            return true;
        }
        /// <summary>
        /// 进入对象节点
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        /// <param name="value">数据对象</param>
        /// <returns>是否继续处理对象</returns>
        private bool pushArray<valueType>(valueType value)
        {
            if (checkLoopDepth == 0)
            {
                if (forefatherCount != 0)
                {
                    int count = forefatherCount;
                    object objectValue = value;
                    foreach (object arrayValue in forefather)
                    {
                        if (arrayValue == objectValue)
                        {
                            fastCSharp.web.ajax.WriteObject(CharStream);
                            return false;
                        }
                        if (--count == 0) break;
                    }
                }
                if (forefatherCount == forefather.Length)
                {
                    object[] newValues = new object[forefatherCount << 1];
                    forefather.CopyTo(newValues, 0);
                    forefather = newValues;
                }
                forefather[forefatherCount++] = value;
            }
            else
            {
                if (--checkLoopDepth == 0) fastCSharp.log.Default.Throw(log.exceptionType.IndexOutOfRange);
                if (isLoopObject)
                {
                    string index;
                    if (objectIndexs.TryGetValue(new objectReference { Value = value }, out index))
                    {
                        CharStream.PrepLength(Config.GetLoopObject.Length + index.Length + (5 + 3));
                        CharStream.UnsafeSimpleWrite(Config.GetLoopObject);
                        CharStream.UnsafeWrite('(');
                        CharStream.UnsafeSimpleWrite(index);
                        CharStream.UnsafeSimpleWrite(",[])");
                        return false;
                    }
                    objectIndexs.Add(new objectReference { Value = value }, index = objectIndexs.Count.toString());
                    CharStream.PrepLength(Config.SetLoopObject.Length + index.Length + (2 + 2));
                    CharStream.UnsafeSimpleWrite(Config.SetLoopObject);
                    CharStream.UnsafeWrite('(');
                    CharStream.UnsafeSimpleWrite(index);
                    CharStream.UnsafeWrite(',');
                }
            }
            return true;
        }
        /// <summary>
        /// 退出对象节点
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void pop()
        {
            if (checkLoopDepth == 0) forefather[--forefatherCount] = null;
            else
            {
                ++checkLoopDepth;
                if (isLoopObject) CharStream.Write(')');
            }
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        private void free()
        {
            Config = null;
            if (objectIndexs != null) objectIndexs.Clear();
            if (forefatherCount != 0)
            {
                Array.Clear(forefather, 0, forefatherCount);
                forefatherCount = 0;
            }
            typePool<jsonSerializer>.PushNotNull(this);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void UnsafeFree()
        {
            free();
        }
        /// <summary>
        /// 逻辑值转换
        /// </summary>
        /// <param name="value">逻辑值</param>
        [toJsonMethod]
        private void toJson(bool value)
        {
            if (value)
            {
                *(long*)CharStream.GetPrepLengthCurrent(4) = 't' + ('r' << 16) + ((long)'u' << 32) + ((long)'e' << 48);
                CharStream.UnsafeAddLength(4);
            }
            else
            {
                char* chars = CharStream.GetPrepLengthCurrent(5);
                *(long*)chars = 'f' + ('a' << 16) + ((long)'l' << 32) + ((long)'s' << 48);
                *(char*)(chars + 4) = 'e';
                CharStream.UnsafeAddLength(5);
            }
        }
        /// <summary>
        /// 逻辑值转换
        /// </summary>
        /// <param name="value">逻辑值</param>
        [toJsonMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toJson(bool? value)
        {
            if (value.HasValue) toJson((bool)value);
            else fastCSharp.web.ajax.WriteNull(CharStream);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toJsonMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toJson(byte value)
        {
            fastCSharp.web.ajax.ToString(value, CharStream);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toJsonMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toJson(byte? value)
        {
            if (value.HasValue) fastCSharp.web.ajax.ToString((byte)value, CharStream);
            else fastCSharp.web.ajax.WriteNull(CharStream);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toJsonMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toJson(sbyte value)
        {
            fastCSharp.web.ajax.ToString(value, CharStream);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toJsonMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toJson(sbyte? value)
        {
            if (value.HasValue) fastCSharp.web.ajax.ToString((sbyte)value, CharStream);
            else fastCSharp.web.ajax.WriteNull(CharStream);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toJsonMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toJson(short value)
        {
            fastCSharp.web.ajax.ToString(value, CharStream);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toJsonMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toJson(short? value)
        {
            if (value.HasValue) fastCSharp.web.ajax.ToString((short)value, CharStream);
            else fastCSharp.web.ajax.WriteNull(CharStream);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toJsonMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toJson(ushort value)
        {
            fastCSharp.web.ajax.ToString(value, CharStream);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toJsonMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toJson(ushort? value)
        {
            if (value.HasValue) fastCSharp.web.ajax.ToString((ushort)value, CharStream);
            else fastCSharp.web.ajax.WriteNull(CharStream);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toJsonMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toJson(int value)
        {
            fastCSharp.web.ajax.ToString(value, CharStream);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toJsonMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toJson(int? value)
        {
            if (value.HasValue) fastCSharp.web.ajax.ToString((int)value, CharStream);
            else fastCSharp.web.ajax.WriteNull(CharStream);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toJsonMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toJson(uint value)
        {
            fastCSharp.web.ajax.ToString(value, CharStream);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toJsonMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toJson(uint? value)
        {
            if (value.HasValue) fastCSharp.web.ajax.ToString((uint)value, CharStream);
            else fastCSharp.web.ajax.WriteNull(CharStream);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toJsonMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toJson(long value)
        {
            fastCSharp.web.ajax.ToString(value, CharStream, Config.IsMaxNumberToString);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toJsonMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toJson(long? value)
        {
            if (value.HasValue) fastCSharp.web.ajax.ToString((long)value, CharStream, Config.IsMaxNumberToString);
            else fastCSharp.web.ajax.WriteNull(CharStream);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toJsonMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toJson(ulong value)
        {
            fastCSharp.web.ajax.ToString(value, CharStream, Config.IsMaxNumberToString);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toJsonMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toJson(ulong? value)
        {
            if (value.HasValue) fastCSharp.web.ajax.ToString((ulong)value, CharStream, Config.IsMaxNumberToString);
            else fastCSharp.web.ajax.WriteNull(CharStream);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toJsonMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toJson(float value)
        {
            if (float.IsNaN(value)) fastCSharp.web.ajax.WriteNaN(CharStream);
            else CharStream.SimpleWriteNotNull(value.ToString());
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toJsonMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toJson(float? value)
        {
            if (value.HasValue) toJson(value.Value);
            else fastCSharp.web.ajax.WriteNull(CharStream);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toJsonMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toJson(double value)
        {
            if (double.IsNaN(value)) fastCSharp.web.ajax.WriteNaN(CharStream);
            else CharStream.SimpleWriteNotNull(value.ToString());
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toJsonMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toJson(double? value)
        {
            if (value.HasValue) toJson(value.Value);
            else fastCSharp.web.ajax.WriteNull(CharStream);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toJsonMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toJson(decimal value)
        {
            CharStream.SimpleWriteNotNull(value.ToString());
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toJsonMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toJson(decimal? value)
        {
            if (value.HasValue) CharStream.SimpleWriteNotNull(((decimal)value).ToString());
            else fastCSharp.web.ajax.WriteNull(CharStream);
        }
        /// <summary>
        /// 字符转换
        /// </summary>
        /// <param name="value">字符</param>
        [toJsonMethod]
        private void toJson(char value)
        {
            byte* data = (byte*)CharStream.GetPrepLengthCurrent(3);
            *(char*)data = fastCSharp.web.ajax.Quote;
            *(char*)(data + sizeof(char)) = value == fastCSharp.web.ajax.Quote ? ' ' : value;
            *(char*)(data + sizeof(char) * 2) = fastCSharp.web.ajax.Quote;
            CharStream.UnsafeAddLength(3);
        }
        /// <summary>
        /// 字符转换
        /// </summary>
        /// <param name="value">字符</param>
        [toJsonMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toJson(char? value)
        {
            if (value.HasValue) toJson((char)value);
            else fastCSharp.web.ajax.WriteNull(CharStream);
        }
        /// <summary>
        /// 时间转换
        /// </summary>
        /// <param name="value">时间</param>
        [toJsonMethod]
        private void toJson(DateTime value)
        {
            if (value == DateTime.MinValue && Config.IsDateTimeMinNull)
            {
                fastCSharp.web.ajax.WriteNull(CharStream);
                return;
            }
            if (Config.IsDateTimeToString)
            {
                if (Config.IsDateTimeOther) fastCSharp.web.ajax.ToStringOther(value, CharStream);
                else
                {
                    CharStream.PrepLength(fastCSharp.date.SqlMillisecondSize + 2);
                    CharStream.UnsafeWrite(fastCSharp.web.ajax.Quote);
                    fastCSharp.date.ToSqlMillisecond(value, CharStream);
                    CharStream.UnsafeWrite(fastCSharp.web.ajax.Quote);
                }
            }
            else fastCSharp.web.ajax.ToStringNotNull(value, CharStream);
        }
        /// <summary>
        /// 时间转换
        /// </summary>
        /// <param name="value">时间</param>
        [toJsonMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toJson(DateTime? value)
        {
            if (value.HasValue) toJson((DateTime)value);
            else fastCSharp.web.ajax.WriteNull(CharStream);
        }
        /// <summary>
        /// Guid转换
        /// </summary>
        /// <param name="value">Guid</param>
        [toJsonMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toJson(Guid value)
        {
            fastCSharp.web.ajax.ToString(ref value, CharStream);
        }
        /// <summary>
        /// Guid转换
        /// </summary>
        /// <param name="value">Guid</param>
        [toJsonMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toJson(Guid? value)
        {
            if (value.HasValue) toJson((Guid)value);
            else fastCSharp.web.ajax.WriteNull(CharStream);
        }
        /// <summary>
        /// 字符串转换
        /// </summary>
        /// <param name="value">字符串</param>
        [toJsonMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toJson(string value)
        {
            if (value == null) fastCSharp.web.ajax.WriteNull(CharStream);
            else
            {
                fixed (char* valueFixed = value) toJson(valueFixed, value.Length);
            }
        }
        /// <summary>
        /// 字符串转换
        /// </summary>
        /// <param name="value">字符串</param>
        [toJsonMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toJson(subString value)
        {
            if (value.value == null) fastCSharp.web.ajax.WriteNull(CharStream);
            else
            {
                fixed (char* valueFixed = value.value) toJson(valueFixed + value.StartIndex, value.Length);
            }
        }
        /// <summary>
        /// 字符串转换
        /// </summary>
        /// <param name="value">JSON节点</param>
        [toJsonMethod]
        private void toJson(jsonNode value)
        {
            switch (value.Type)
            {
                case jsonNode.type.Null:
                    fastCSharp.web.ajax.WriteNull(CharStream);
                    return;
                case jsonNode.type.Dictionary:
                    subArray<keyValue<jsonNode, jsonNode>> dictionary = value.Dictionary;
                    CharStream.Write('{');
                    if (dictionary.length != 0)
                    {
                        int count = dictionary.length;
                        foreach (keyValue<jsonNode, jsonNode> keyValue in dictionary.array)
                        {
                            if (count != dictionary.length) CharStream.Write(',');
                            toJson(keyValue.Key);
                            CharStream.Write(':');
                            toJson(keyValue.Value);
                            if (--count == 0) break;
                        }
                    }
                    CharStream.Write('}');
                    return;
                case jsonNode.type.List:
                    subArray<jsonNode> list = value.UnsafeList;
                     CharStream.Write('[');
                     if (list.length != 0)
                    {
                        int count = list.length;
                        foreach (jsonNode node in list.array)
                        {
                            if (count != list.length) CharStream.Write(',');
                            toJson(node);
                            if (--count == 0) break;
                        }
                    }
                    CharStream.Write(']');
                    return;
                case jsonNode.type.String:
                    subString subString = value.String;
                    fixed (char* valueFixed = subString.value) toJson(valueFixed + subString.StartIndex, subString.Length);
                    return;
                case jsonNode.type.QuoteString:
                    CharStream.PrepLength(value.String.Length + 2);
                    CharStream.UnsafeWrite((char)value.Int64);
                    CharStream.Write(ref value.String);
                    CharStream.UnsafeWrite((char)value.Int64);
                    return;
                case jsonNode.type.NumberString:
                    if ((int)value.Int64 == 0) CharStream.Write(ref value.String);
                    else
                    {
                        CharStream.PrepLength(value.String.Length + 2);
                        CharStream.UnsafeWrite((char)value.Int64);
                        CharStream.Write(ref value.String);
                        CharStream.UnsafeWrite((char)value.Int64);
                    }
                    return;
                case jsonNode.type.Bool:
                    toJson((byte)value.Int64 != 0);
                    return;
                case jsonNode.type.DateTimeTick:
                    toJson(new DateTime(value.Int64));
                    return;
                case jsonNode.type.NaN:
                    fastCSharp.web.ajax.WriteNaN(CharStream);
                    return;
                default:
                    log.Error.Add("未知节点类型 " + value.Type.ToString(), new System.Diagnostics.StackFrame(), true);
                    return;
            }
        }
        /// <summary>
        /// 字符串转换
        /// </summary>
        /// <param name="value">字符串</param>
        [toJsonMethod]
        private void toJson(object value)
        {
            if (value == null) fastCSharp.web.ajax.WriteNull(CharStream);
            else if (Config.IsObject)
            {
                Type type = value.GetType();
                if (type == typeof(object)) fastCSharp.web.ajax.WriteObject(CharStream);
                else typeToJsoner.GetObjectToJsoner(type)(this, value);
            }
            else fastCSharp.web.ajax.WriteObject(CharStream);
        }
        /// <summary>
        /// 字符串转换
        /// </summary>
        /// <param name="start">起始位置</param>
        /// <param name="length">字符串长度</param>
        private void toJson(char* start, int length)
        {
            char* data = CharStream.GetPrepLengthCurrent(length + 2);
            *data = fastCSharp.web.ajax.Quote;
            for (char* end = start + length; start != end; ++start) *++data = *start == fastCSharp.web.ajax.Quote ? ' ' : *start;
            *(data + 1) = fastCSharp.web.ajax.Quote;
            CharStream.UnsafeAddLength(length + 2);
        }
        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="type">类型</param>
        [toJsonMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toJson(Type type)
        {
            if (type == null) fastCSharp.web.ajax.WriteNull(CharStream);
            else typeToJsoner<remoteType>.ToJson(this, new remoteType(type));
        }

        /// <summary>
        /// 值类型对象转换JSON字符串
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void nullableToJson<valueType>(Nullable<valueType> value) where valueType : struct
        {
            if (value.HasValue) typeToJsoner<valueType>.StructToJson(this, value.Value);
            else fastCSharp.web.ajax.WriteNull(CharStream);
        }
        /// <summary>
        /// 值类型对象转换函数信息
        /// </summary>
        private static readonly MethodInfo nullableToJsonMethod = typeof(jsonSerializer).GetMethod("nullableToJson", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 字典转换JSON字符串
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void keyValuePairToJson<keyValue, valueType>(KeyValuePair<keyValue, valueType> value)
        {
            typeToJsoner<keyValue>.KeyValuePair(this, value);
        }
        /// <summary>
        /// 字典转换函数信息
        /// </summary>
        private static readonly MethodInfo keyValuePairToJsonMethod = typeof(jsonSerializer).GetMethod("keyValuePairToJson", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 引用类型对象转换JSON字符串
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void classToJson<valueType>(valueType value)
        {
            if (value == null) fastCSharp.web.ajax.WriteNull(CharStream);
            else typeToJsoner<valueType>.ClassToJson(this, value);
        }
        /// <summary>
        /// 字典转换函数信息
        /// </summary>
        private static readonly MethodInfo classToJsonMethod = typeof(jsonSerializer).GetMethod("classToJson", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 值类型对象转换JSON字符串
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void structToJson<valueType>(valueType value)
        {
            typeToJsoner<valueType>.StructToJson(this, value);
        }
        /// <summary>
        /// 字典转换函数信息
        /// </summary>
        private static readonly MethodInfo structToJsonMethod = typeof(jsonSerializer).GetMethod("structToJson", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 字符串转换
        /// </summary>
        /// <param name="value">数据对象</param>
        private void enumToString<valueType>(valueType value)
        {
            string stringValue = value.ToString();
            char charValue = stringValue[0];
            if ((uint)(charValue - '1') < 9 || charValue == '-') CharStream.SimpleWriteNotNull(stringValue);
            else fastCSharp.web.ajax.ToString(stringValue, CharStream);
        }
        /// <summary>
        /// 字典转换函数信息
        /// </summary>
        private static readonly MethodInfo enumToStringMethod = typeof(jsonSerializer).GetMethod("enumToString", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// object转换JSON字符串
        /// </summary>
        /// <param name="toJsoner">对象转换JSON字符串</param>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static void toJsonObject<valueType>(jsonSerializer toJsoner, object value)
        {
            typeToJsoner<valueType>.ToJson(toJsoner, (valueType)value);
        }
        /// <summary>
        /// 字典转换函数信息
        /// </summary>
        private static readonly MethodInfo toJsonObjectMethod = typeof(jsonSerializer).GetMethod("toJsonObject", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        private void array<valueType>(valueType[] array)
        {
            if (array == null) fastCSharp.web.ajax.WriteNull(CharStream);
            else if (push(array))
            {
                typeToJsoner<valueType>.Array(this, array);
                pop();
            }
        }
        /// <summary>
        /// 字典转换函数信息
        /// </summary>
        private static readonly MethodInfo arrayMethod = typeof(jsonSerializer).GetMethod("array", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举集合转换
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void structEnumerable<valueType, elementType>(valueType value) where valueType : IEnumerable<elementType>
        {
            typeToJsoner<elementType>.Enumerable(this, value);
        }
        /// <summary>
        /// 字典转换函数信息
        /// </summary>
        private static readonly MethodInfo structEnumerableMethod = typeof(jsonSerializer).GetMethod("structEnumerable", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举集合转换
        /// </summary>
        /// <param name="value">枚举集合</param>
        private void enumerable<valueType, elementType>(valueType value) where valueType : IEnumerable<elementType>
        {
            if (value == null) fastCSharp.web.ajax.WriteNull(CharStream);
            else if (pushArray(value))
            {
                typeToJsoner<elementType>.Enumerable(this, value);
                pop();
            }
        }
        /// <summary>
        /// 字典转换函数信息
        /// </summary>
        private static readonly MethodInfo enumerableMethod = typeof(jsonSerializer).GetMethod("enumerable", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 字典转换
        /// </summary>
        /// <param name="dictionary">数据对象</param>
        private void dictionary<valueType, dictionaryValueType>(Dictionary<valueType, dictionaryValueType> dictionary)
        {
            if (dictionary == null) fastCSharp.web.ajax.WriteNull(CharStream);
            else if (push(dictionary))
            {
                typeToJsoner<valueType>.Dictionary(this, dictionary);
                pop();
            }
        }
        /// <summary>
        /// 字典转换函数信息
        /// </summary>
        private static readonly MethodInfo dictionaryMethod = typeof(jsonSerializer).GetMethod("dictionary", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 字典转换
        /// </summary>
        /// <param name="dictionary">字典</param>
        private void stringDictionary<valueType>(Dictionary<string, valueType> dictionary)
        {
            if (dictionary == null) fastCSharp.web.ajax.WriteNull(CharStream);
            else if (push(dictionary))
            {
                if (Config.IsStringDictionaryToObject) typeToJsoner<valueType>.StringDictionary(this, dictionary);
                else typeToJsoner<string>.Dictionary<valueType>(this, dictionary);
                pop();
            }
        }
        /// <summary>
        /// 字符串字典转换函数信息
        /// </summary>
        private static readonly MethodInfo stringDictionaryMethod = typeof(jsonSerializer).GetMethod("stringDictionary", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 基类转换
        /// </summary>
        /// <param name="toJsoner">对象转换JSON字符串</param>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static void baseToJson<valueType, childType>(jsonSerializer toJsoner, childType value) where childType : valueType
        {
            typeToJsoner<valueType>.ClassToJson(toJsoner, value);
        }
        /// <summary>
        /// 基类转换函数信息
        /// </summary>
        private static readonly MethodInfo baseToJsonMethod = typeof(jsonSerializer).GetMethod("baseToJson", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="value"></param>
        /// <param name="jsonStream"></param>
        /// <param name="stream"></param>
        /// <param name="config"></param>
        internal static void Serialize<valueType>(valueType value, charStream jsonStream, unmanagedStream stream, config config)
        {
            ToJson(value, jsonStream, config);
            stream.PrepLength(sizeof(int) + (jsonStream.Length << 1));
            stream.UnsafeAddLength(sizeof(int));
            int index = stream.Length;
            web.ajax.FormatJavascript(jsonStream, stream);
            int length = stream.Length - index;
            *(int*)(stream.data.Byte + index - sizeof(int)) = length;
            if ((length & 2) != 0) stream.Write(' ');
        }

        /// <summary>
        /// 公共默认配置参数
        /// </summary>
        private static readonly config defaultConfig = new config { CheckLoopDepth = fastCSharp.config.appSetting.SerializeDepth };
        /// <summary>
        /// 对象转换JSON字符串
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="value">数据对象</param>
        /// <param name="jsonStream">Json输出缓冲区</param>
        /// <param name="config">配置参数</param>
        public static void ToJson<valueType>(valueType value, charStream jsonStream, config config = null)
        {
            if (jsonStream == null) log.Default.Throw(log.exceptionType.Null);
            jsonSerializer toJsoner = typePool<jsonSerializer>.Pop() ?? new jsonSerializer();
            try
            {
                toJsoner.toJson<valueType>(value, jsonStream, config ?? defaultConfig);
            }
            finally { toJsoner.free(); }
        }

        /// <summary>
        /// 对象转换JSON字符串
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="value">数据对象</param>
        /// <param name="config">配置参数</param>
        /// <returns>Json字符串</returns>
        public static string ToJson<valueType>(valueType value, config config = null)
        {
            jsonSerializer toJsoner = typePool<jsonSerializer>.Pop() ?? new jsonSerializer();
            try
            {
                return toJsoner.toJson<valueType>(value, config ?? defaultConfig);
            }
            finally { toJsoner.free(); }
        }
        /// <summary>
        /// 对象转换JSON字符串
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="value">数据对象</param>
        /// <returns>Json字符串</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static string sqlToJson<valueType>(valueType value)
        {
            return value == null ? string.Empty : ToJson(value);
        }
        /// <summary>
        /// 对象转换JSON字符串函数信息
        /// </summary>
        internal static readonly MethodInfo SqlToJsonMethod = typeof(fastCSharp.emit.jsonSerializer).GetMethod("sqlToJson", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 未知类型对象转换JSON字符串
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="value">数据对象</param>
        /// <param name="config">配置参数</param>
        /// <returns>Json字符串</returns>
        private static string objectToJson<valueType>(object value, config config)
        {
            jsonSerializer toJsoner = typePool<jsonSerializer>.Pop() ?? new jsonSerializer();
            try
            {
                return toJsoner.toJson<valueType>((valueType)value, config ?? defaultConfig);
            }
            finally { toJsoner.free(); }
        }
        /// <summary>
        /// 未知类型对象转换JSON字符串
        /// </summary>
        /// <param name="value">数据对象</param>
        /// <param name="config">配置参数</param>
        /// <returns>Json字符串</returns>
        public static string ObjectToJson(object value, config config = null)
        {
            if (value == null) return fastCSharp.web.ajax.Null;
            Type type = value.GetType();
            Func<object, config, string> toJson;
            if (!objectToJsons.TryGetValue(type, out toJson))
            {
                objectToJsons.Set(type, toJson = (Func<object, config, string>)Delegate.CreateDelegate(typeof(Func<object, config, string>), objectToJsonMethod.MakeGenericMethod(type)));
            }
            return toJson(value, config);
        }
        /// <summary>
        /// 未知类型对象转换JSON字符串
        /// </summary>
        private static readonly interlocked.dictionary<Type, Func<object, config, string>> objectToJsons = new interlocked.dictionary<Type, Func<object, config, string>>();
        /// <summary>
        /// 未知类型对象转换JSON字符串函数信息
        /// </summary>
        private static readonly MethodInfo objectToJsonMethod = typeof(jsonSerializer).GetMethod("objectToJson", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(object), typeof(config) }, null);
        /// <summary>
        /// 基本类型转换函数
        /// </summary>
        private static readonly Dictionary<Type, MethodInfo> toJsonMethods;
        /// <summary>
        /// 获取基本类型转换函数
        /// </summary>
        /// <param name="type">基本类型</param>
        /// <returns>转换函数</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static MethodInfo getToJsonMethod(Type type)
        {
            MethodInfo method;
            return toJsonMethods.TryGetValue(type, out method) ? method : null;
        }
        static jsonSerializer()
        {
            toJsonMethods = fastCSharp.dictionary.CreateOnly<Type, MethodInfo>();
            foreach (MethodInfo method in typeof(jsonSerializer).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (method.customAttribute<toJsonMethod>() != null)
                {
                    toJsonMethods.Add(method.GetParameters()[0].ParameterType, method);
                }
            }
        }
    }
}
