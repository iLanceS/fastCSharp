using System;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Collections.Generic;
using fastCSharp.code;
using fastCSharp.threading;
using fastCSharp.reflection;
#if NOJIT
#else
using System.Reflection.Emit;
#endif

namespace fastCSharp.emit
{
    /// <summary>
    /// XML序列化
    /// </summary>
    public unsafe sealed class xmlSerializer : stringSerializer
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
            /// XML头部
            /// </summary>
            public string Header = @"<?xml version=""1.0"" encoding=""utf-8""?>";
            /// <summary>
            /// 根节点名称
            /// </summary>
            public string BootNodeName = "xml";
            /// <summary>
            /// 集合子节点名称
            /// </summary>
            public string ItemName = "item";
            /// <summary>
            /// 成员位图
            /// </summary>
            public memberMap MemberMap;
            /// <summary>
            /// 循环引用检测深度,0表示实时检测
            /// </summary>
            public int CheckLoopDepth;
            /// <summary>
            /// 警告提示状态
            /// </summary>
            public warning Warning { get; internal set; }
            /// <summary>
            /// 是否输出空对象
            /// </summary>
            public bool IsOutputNull;
            /// <summary>
            /// 是否输出长度为0的字符串
            /// </summary>
            public bool IsOutputEmptyString = true;
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
        private sealed class toXmlMethod : Attribute { }
        /// <summary>
        /// 对象转换XML字符串静态信息
        /// </summary>
        internal static class staticTypeToXmler
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
                    dynamicMethod = new DynamicMethod("xmlSerializer", null, new Type[] { typeof(xmlSerializer), type }, type, true);
                    generator = dynamicMethod.GetILGenerator();
                    generator.DeclareLocal(typeof(charStream));

                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldfld, charStreamField);
                    generator.Emit(OpCodes.Stloc_0);

                    isValueType = type.IsValueType;
                }
                /// <summary>
                /// 添加成员
                /// </summary>
                /// <param name="name">成员名称</param>
                /// <param name="attribute">XML序列化成员配置</param>
                private void nameStart(string name, xmlSerialize.member attribute)
                {
                    generator.charStreamSimpleWriteNotNull(OpCodes.Ldloc_0, GetNameStartPool(name), name.Length + 2);

                    if (attribute != null && attribute.ItemName != null)
                    { 
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldstr, attribute.ItemName);
                        generator.Emit(OpCodes.Stfld, itemNameField);
                    }
                    generator.Emit(OpCodes.Ldarg_0);
                    if (isValueType) generator.Emit(OpCodes.Ldarga_S, 1);
                    else generator.Emit(OpCodes.Ldarg_1);
                }
                /// <summary>
                /// 添加字段
                /// </summary>
                /// <param name="field">字段信息</param>
                /// <param name="attribute">XML序列化成员配置</param>
                public void Push(fieldIndex field, xmlSerialize.member attribute)
                {
                    Label end = default(Label);
                    MethodInfo isOutputMethod = GetIsOutputMethod(field.Member.FieldType);
                    if (isOutputMethod != null)
                    {
                        generator.Emit(OpCodes.Ldarg_0);
                        if (isValueType) generator.Emit(OpCodes.Ldarga_S, 1);
                        else generator.Emit(OpCodes.Ldarg_1);
                        generator.Emit(OpCodes.Ldfld, field.Member);
                        generator.Emit(isOutputMethod.IsFinal || !isOutputMethod.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, isOutputMethod);
                        generator.Emit(OpCodes.Brfalse_S, end = generator.DefineLabel());
                    }

                    string name = field.AnonymousName;
                    nameStart(name, attribute);

                    generator.Emit(OpCodes.Ldfld, field.Member);
                    MethodInfo method = GetMemberMethodInfo(field.Member.FieldType);
                    generator.Emit(method.IsFinal || !method.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, method);

                    generator.charStreamSimpleWriteNotNull(OpCodes.Ldloc_0, GetNameEndPool(name), name.Length + 3);

                    if (isOutputMethod != null) generator.MarkLabel(end);
                }
                /// <summary>
                /// 添加属性
                /// </summary>
                /// <param name="property">属性信息</param>
                /// <param name="method">函数信息</param>
                /// <param name="attribute">XML序列化成员配置</param>
                public void Push(propertyIndex property, MethodInfo method, xmlSerialize.member attribute)
                {
                    Label end = default(Label);
                    MethodInfo isOutputMethod = GetIsOutputMethod(property.Member.PropertyType);
                    if (isOutputMethod != null)
                    {
                        generator.Emit(OpCodes.Ldarg_0);
                        if (isValueType) generator.Emit(OpCodes.Ldarga_S, 1);
                        else generator.Emit(OpCodes.Ldarg_1);
                        generator.Emit(method.IsFinal || !method.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, method);
                        generator.Emit(isOutputMethod.IsFinal || !isOutputMethod.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, isOutputMethod);
                        generator.Emit(OpCodes.Brfalse_S, end = generator.DefineLabel());
                    }

                    nameStart(property.Member.Name, attribute);

                    generator.Emit(method.IsFinal || !method.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, method);
                    method = GetMemberMethodInfo(property.Member.PropertyType);
                    generator.Emit(method.IsFinal || !method.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, method);

                    generator.charStreamSimpleWriteNotNull(OpCodes.Ldloc_0, GetNameEndPool(property.Member.Name), property.Member.Name.Length + 3);

                    if (isOutputMethod != null) generator.MarkLabel(end);
                }
                /// <summary>
                /// 添加字段
                /// </summary>
                /// <param name="field">字段信息</param>
                public void PushBox(fieldIndex field)
                {
                    Label end = default(Label);
                    MethodInfo isOutputMethod = GetIsOutputMethod(field.Member.FieldType);
                    if (isOutputMethod != null)
                    {
                        generator.Emit(OpCodes.Ldarg_0);
                        if (isValueType) generator.Emit(OpCodes.Ldarga_S, 1);
                        else generator.Emit(OpCodes.Ldarg_1);
                        generator.Emit(OpCodes.Ldfld, field.Member);
                        generator.Emit(isOutputMethod.IsFinal || !isOutputMethod.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, isOutputMethod);
                        generator.Emit(OpCodes.Brfalse_S, end = generator.DefineLabel());
                    }

                    generator.Emit(OpCodes.Ldarg_0);
                    if (isValueType) generator.Emit(OpCodes.Ldarga_S, 1);
                    else generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Ldfld, field.Member);
                    MethodInfo method = GetMemberMethodInfo(field.Member.FieldType);
                    generator.Emit(method.IsFinal || !method.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, method);

                    if (isOutputMethod != null) generator.MarkLabel(end);
                }
                /// <summary>
                /// 添加属性
                /// </summary>
                /// <param name="property">属性信息</param>
                /// <param name="method">函数信息</param>
                public void PushBox(propertyIndex property, MethodInfo method)
                {
                    Label end = default(Label);
                    MethodInfo isOutputMethod = GetIsOutputMethod(property.Member.PropertyType);
                    if (isOutputMethod != null)
                    {
                        generator.Emit(OpCodes.Ldarg_0);
                        if (isValueType) generator.Emit(OpCodes.Ldarga_S, 1);
                        else generator.Emit(OpCodes.Ldarg_1);
                        generator.Emit(method.IsFinal || !method.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, method);
                        generator.Emit(isOutputMethod.IsFinal || !isOutputMethod.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, isOutputMethod);
                        generator.Emit(OpCodes.Brfalse_S, end = generator.DefineLabel());
                    }

                    generator.Emit(OpCodes.Ldarg_0);
                    if (isValueType) generator.Emit(OpCodes.Ldarga_S, 1);
                    else generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(method.IsFinal || !method.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, method);
                    method = GetMemberMethodInfo(property.Member.PropertyType);
                    generator.Emit(method.IsFinal || !method.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, method);

                    if (isOutputMethod != null) generator.MarkLabel(end);
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
                    dynamicMethod = new DynamicMethod("xmlMemberMapSerializer", null, new Type[] { typeof(memberMap), typeof(xmlSerializer), type }, type, true);
                    generator = dynamicMethod.GetILGenerator();
                    generator.DeclareLocal(typeof(charStream));

                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Ldfld, charStreamField);
                    generator.Emit(OpCodes.Stloc_0);

                    isValueType = type.IsValueType;
                }
                /// <summary>
                /// 添加字段
                /// </summary>
                /// <param name="field">字段信息</param>
                /// <param name="attribute">XML序列化成员配置</param>
                public void Push(fieldIndex field, xmlSerialize.member attribute)
                {
                    Label end = generator.DefineLabel();
                    generator.memberMapIsMember(OpCodes.Ldarg_0, field.MemberIndex);
                    generator.Emit(OpCodes.Brfalse_S, end);

                    MethodInfo isOutputMethod = GetIsOutputMethod(field.Member.FieldType);
                    if (isOutputMethod != null)
                    {
                        generator.Emit(OpCodes.Ldarg_1);
                        if (isValueType) generator.Emit(OpCodes.Ldarga_S, 2);
                        else generator.Emit(OpCodes.Ldarg_2);
                        generator.Emit(OpCodes.Ldfld, field.Member);
                        generator.Emit(isOutputMethod.IsFinal || !isOutputMethod.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, isOutputMethod);
                        generator.Emit(OpCodes.Brfalse_S, end);
                    }
                    string name = field.AnonymousName;
                    generator.charStreamSimpleWriteNotNull(OpCodes.Ldloc_0, GetNameStartPool(name), name.Length + 2);

                    if (attribute != null && attribute.ItemName != null)
                    {
                        generator.Emit(OpCodes.Ldarg_1);
                        generator.Emit(OpCodes.Ldstr, attribute.ItemName);
                        generator.Emit(OpCodes.Stfld, itemNameField);
                    }
                    generator.Emit(OpCodes.Ldarg_1);
                    if (isValueType) generator.Emit(OpCodes.Ldarga_S, 2);
                    else generator.Emit(OpCodes.Ldarg_2);
                    generator.Emit(OpCodes.Ldfld, field.Member);
                    MethodInfo method = GetMemberMethodInfo(field.Member.FieldType);
                    generator.Emit(method.IsFinal || !method.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, method);

                    generator.charStreamSimpleWriteNotNull(OpCodes.Ldloc_0, GetNameEndPool(name), name.Length + 3);

                    generator.MarkLabel(end);
                }
                /// <summary>
                /// 添加属性
                /// </summary>
                /// <param name="property">属性信息</param>
                /// <param name="method">函数信息</param>
                /// <param name="attribute">XML序列化成员配置</param>
                public void Push(propertyIndex property, MethodInfo method, xmlSerialize.member attribute)
                {
                    Label end = generator.DefineLabel();
                    generator.memberMapIsMember(OpCodes.Ldarg_0, property.MemberIndex);
                    generator.Emit(OpCodes.Brfalse_S, end);

                    MethodInfo isOutputMethod = GetIsOutputMethod(property.Member.PropertyType);
                    if (isOutputMethod != null)
                    {
                        generator.Emit(OpCodes.Ldarg_1);
                        if (isValueType) generator.Emit(OpCodes.Ldarga_S, 2);
                        else generator.Emit(OpCodes.Ldarg_2);
                        generator.Emit(method.IsFinal || !method.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, method);
                        generator.Emit(isOutputMethod.IsFinal || !isOutputMethod.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, isOutputMethod);
                        generator.Emit(OpCodes.Brfalse_S, end);
                    }
                    generator.charStreamSimpleWriteNotNull(OpCodes.Ldloc_0, GetNameStartPool(property.Member.Name), property.Member.Name.Length + 2);

                    if (attribute != null && attribute.ItemName != null)
                    {
                        generator.Emit(OpCodes.Ldarg_1);
                        generator.Emit(OpCodes.Ldstr, attribute.ItemName);
                        generator.Emit(OpCodes.Stfld, itemNameField);
                    }
                    generator.Emit(OpCodes.Ldarg_1);
                    if (isValueType) generator.Emit(OpCodes.Ldarga_S, 2);
                    else generator.Emit(OpCodes.Ldarg_2);
                    generator.Emit(method.IsFinal || !method.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, method);
                    method = GetMemberMethodInfo(property.Member.PropertyType);
                    generator.Emit(method.IsFinal || !method.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, method);

                    generator.charStreamSimpleWriteNotNull(OpCodes.Ldloc_0, GetNameEndPool(property.Member.Name), property.Member.Name.Length + 3);

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
            /// <param name="fields"></param>
            /// <param name="typeAttribute">类型配置</param>
            /// <returns>字段成员集合</returns>
            public static subArray<keyValue<fieldIndex, xmlSerialize.member>> GetFields(fieldIndex[] fields, xmlSerialize typeAttribute)
            {
                subArray<keyValue<fieldIndex, xmlSerialize.member>> values = new subArray<keyValue<fieldIndex, xmlSerialize.member>>(fields.Length);
                foreach (fieldIndex field in fields)
                {
                    Type type = field.Member.FieldType;
                    if (!type.IsPointer && (!type.IsArray || type.GetArrayRank() == 1) && !field.IsIgnore)
                    {
                        xmlSerialize.member attribute = field.GetAttribute<xmlSerialize.member>(true, true);
                        if (typeAttribute.IsAllMember ? (attribute == null || attribute.IsSetup) : (attribute != null && attribute.IsSetup))
                        {
                            values.Add(new keyValue<fieldIndex, xmlSerialize.member>(field, attribute));
                        }
                    }
                }
                return values;
            }
            /// <summary>
            /// 属性成员信息
            /// </summary>
            public struct propertyInfo
            {
                /// <summary>
                /// 属性索引
                /// </summary>
                public propertyIndex Property;
                /// <summary>
                /// 访问函数
                /// </summary>
                public MethodInfo Method;
                /// <summary>
                /// 自定义属性
                /// </summary>
                public xmlSerialize.member Attribute;
            }
            /// <summary>
            /// 获取属性成员集合
            /// </summary>
            /// <param name="properties">属性成员集合</param>
            /// <param name="typeAttribute">类型配置</param>
            /// <returns>属性成员集合</returns>
            public static subArray<propertyInfo> GetProperties(propertyIndex[] properties, xmlSerialize typeAttribute)
            {
                subArray<propertyInfo> values = new subArray<propertyInfo>(properties.Length);
                foreach (propertyIndex property in properties)
                {
                    if (property.Member.CanRead)
                    {
                        Type type = property.Member.PropertyType;
                        if (!type.IsPointer && (!type.IsArray || type.GetArrayRank() == 1) && !property.IsIgnore)
                        {
                            xmlSerialize.member attribute = property.GetAttribute<xmlSerialize.member>(true, true);
                            if (typeAttribute.IsAllMember ? (attribute == null || attribute.IsSetup) : (attribute != null && attribute.IsSetup))
                            {
                                MethodInfo method = property.Member.GetGetMethod(true);
                                if (method != null && method.GetParameters().Length == 0) values.Add(new propertyInfo { Property = property, Method = method, Attribute = attribute });
                            }
                        }
                    }
                }
                return values;
            }
            /// <summary>
            /// 获取成员转换函数信息
            /// </summary>
            /// <param name="type">成员类型</param>
            /// <returns>成员转换函数信息</returns>
            internal static MethodInfo GetMemberMethodInfo(Type type)
            {
                MethodInfo methodInfo = xmlSerializer.getToXmlMethod(type);
                if (methodInfo != null) return methodInfo;
                if (type.IsArray) return GetArrayToXmler(type.GetElementType());
                if (type.IsEnum) return GetEnumToXmler(type);
                return GetCustomToXmler(type) ?? GetIEnumerableToXmler(type) ?? GetTypeToXmler(type);
            }
            /// <summary>
            /// 获取是否输出对象函数信息
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            internal static MethodInfo GetIsOutputMethod(Type type)
            {
                if (type.IsValueType) return type == typeof(subString) ? isOutputSubStringMethod : GetIsOutputNullable(type);
                return type == typeof(string) ? isOutputStringMethod : isOutputMethod;
            }
            /// <summary>
            /// 是否输出可空对象函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> isOutputNullableMethods = new interlocked.dictionary<Type, MethodInfo>();
            /// <summary>
            /// 获取是否输出可空对象函数信息
            /// </summary>
            /// <param name="type">数组类型</param>
            /// <returns>数组转换委托调用函数信息</returns>
            public static MethodInfo GetIsOutputNullable(Type type)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    MethodInfo method;
                    if (isOutputNullableMethods.TryGetValue(type, out method)) return method;
                    isOutputNullableMethods.Set(type, method = isOutputNullableMethod.MakeGenericMethod(type.GetGenericArguments()));
                    return method;
                }
                return null;
            }

            /// <summary>
            /// 数组转换调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> arrayToXmlers = new interlocked.dictionary<Type, MethodInfo>();
            /// <summary>
            /// 获取数组转换委托调用函数信息
            /// </summary>
            /// <param name="type">数组类型</param>
            /// <returns>数组转换委托调用函数信息</returns>
            public static MethodInfo GetArrayToXmler(Type type)
            {
                MethodInfo method;
                if (arrayToXmlers.TryGetValue(type, out method)) return method;
                arrayToXmlers.Set(type, method = (type.IsValueType ? structArrayMethod : arrayMethod).MakeGenericMethod(type));
                return method;
            }
            /// <summary>
            /// 枚举集合转换调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> enumerableToXmlers = new interlocked.dictionary<Type, MethodInfo>();
            /// <summary>
            /// 获取枚举集合转换委托调用函数信息
            /// </summary>
            /// <param name="type">枚举类型</param>
            /// <returns>枚举集合转换委托调用函数信息</returns>
            public static MethodInfo GetIEnumerableToXmler(Type type)
            {
                MethodInfo method;
                if (enumerableToXmlers.TryGetValue(type, out method)) return method;
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
                                method = (type.IsValueType ? (argumentType.IsValueType ? structStructEnumerableMethod : structClassEnumerableMethod) : (argumentType.IsValueType ? classStructEnumerableMethod : classClassEnumerableMethod)).MakeGenericMethod(type, argumentType);
                                break;
                            }
                            parameters[0] = typeof(ICollection<>).MakeGenericType(argumentType);
                            constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, parameters, null);
                            if (constructorInfo != null)
                            {
                                method = (type.IsValueType ? (argumentType.IsValueType ? structStructEnumerableMethod : structClassEnumerableMethod) : (argumentType.IsValueType ? classStructEnumerableMethod : classClassEnumerableMethod)).MakeGenericMethod(type, argumentType);
                                break;
                            }
                            parameters[0] = typeof(IEnumerable<>).MakeGenericType(argumentType);
                            constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, parameters, null);
                            if (constructorInfo != null)
                            {
                                method = (type.IsValueType ? (argumentType.IsValueType ? structStructEnumerableMethod : structClassEnumerableMethod) : (argumentType.IsValueType ? classStructEnumerableMethod : classClassEnumerableMethod)).MakeGenericMethod(type, argumentType);
                                break;
                            }
                            parameters[0] = argumentType.MakeArrayType();
                            constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, parameters, null);
                            if (constructorInfo != null)
                            {
                                method = (type.IsValueType ? (argumentType.IsValueType ? structStructEnumerableMethod : structClassEnumerableMethod) : (argumentType.IsValueType ? classStructEnumerableMethod : classClassEnumerableMethod)).MakeGenericMethod(type, argumentType);
                                break;
                            }
                        }
                        else if (genericType == typeof(IDictionary<,>))
                        {
                            ConstructorInfo constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { interfaceType }, null);
                            if (constructorInfo != null)
                            {
                                method = (type.IsValueType ? structStructEnumerableMethod : classStructEnumerableMethod).MakeGenericMethod(type, typeof(KeyValuePair<,>).MakeGenericType(interfaceType.GetGenericArguments()));
                                break;
                            }
                        }
                    }
                }
                enumerableToXmlers.Set(type, method);
                return method;
            }
            /// <summary>
            /// 枚举转换调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> enumToXmlers = new interlocked.dictionary<Type, MethodInfo>();
            /// <summary>
            /// 获取枚举转换委托调用函数信息
            /// </summary>
            /// <param name="type">数组类型</param>
            /// <returns>枚举转换委托调用函数信息</returns>
            public static MethodInfo GetEnumToXmler(Type type)
            {
                MethodInfo method;
                if (enumToXmlers.TryGetValue(type, out method)) return method;
                enumToXmlers.Set(type, method = enumToStringMethod.MakeGenericMethod(type));
                return method;
            }
            /// <summary>
            /// 未知类型转换调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> typeToXmlers = new interlocked.dictionary<Type, MethodInfo>();
            /// <summary>
            /// 未知类型枚举转换委托调用函数信息
            /// </summary>
            /// <param name="type">数组类型</param>
            /// <returns>未知类型转换委托调用函数信息</returns>
            public static MethodInfo GetTypeToXmler(Type type)
            {
                MethodInfo method;
                if (typeToXmlers.TryGetValue(type, out method)) return method;
                if (type.IsValueType)
                {
                    if (type.IsGenericType)
                    {
                        Type genericType = type.GetGenericTypeDefinition();
                        if (genericType == typeof(Nullable<>)) method = nullableToXmlMethod.MakeGenericMethod(type.GetGenericArguments());
                    }
                    if (method == null) method = structToXmlMethod.MakeGenericMethod(type);
                }
                else method = classToXmlMethod.MakeGenericMethod(type);
                typeToXmlers.Set(type, method);
                return method;
            }
            /// <summary>
            /// 自定义转换调用函数信息集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, MethodInfo> customToXmlers = new interlocked.dictionary<Type, MethodInfo>();
            /// <summary>
            /// 自定义枚举转换委托调用函数信息
            /// </summary>
            /// <param name="type">数组类型</param>
            /// <returns>自定义转换委托调用函数信息</returns>
            public static MethodInfo GetCustomToXmler(Type type)
            {
                MethodInfo method;
                if (customToXmlers.TryGetValue(type, out method)) return method;
                foreach (fastCSharp.code.attributeMethod methodInfo in fastCSharp.code.attributeMethod.GetStatic(type))
                {
                    if (methodInfo.Method.ReturnType == typeof(void))
                    {
                        ParameterInfo[] parameters = methodInfo.Method.GetParameters();
                        if (parameters.Length == 2 && parameters[0].ParameterType == typeof(xmlSerializer) && parameters[1].ParameterType == type)
                        {
                            if (methodInfo.GetAttribute<xmlSerialize.custom>(true) != null)
                            {
                                method = methodInfo.Method;
                                break;
                            }
                        }
                    }
                }
                customToXmlers.Set(type, method);
                return method;
            }
#if NOJIT
#else
            /// <summary>
            /// 名称数据信息集合
            /// </summary>
            private static readonly interlocked.dictionary<string, pointer> nameStartPools = new interlocked.dictionary<string, pointer>();
            /// <summary>
            /// 获取名称数据
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public static char* GetNameStartPool(string name)
            {
                pointer pointer;
                if (nameStartPools.TryGetValue(name, out pointer)) return pointer.Char;
                char* value = namePool.Get(name, 1, 1);
                *value = '<';
                *(value + (1 + name.Length)) = '>';
                nameStartPools.Set(name, new pointer { Data = value });
                return value;
            }
            /// <summary>
            /// 名称数据信息集合
            /// </summary>
            private static readonly interlocked.dictionary<string, pointer> nameEndPools = new interlocked.dictionary<string, pointer>();
            /// <summary>
            /// 获取名称数据
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public static char* GetNameEndPool(string name)
            {
                pointer pointer;
                if (nameEndPools.TryGetValue(name, out pointer)) return pointer.Char;
                char* value = namePool.Get(name, 2, 1);
                *(int*)value = '<' + ('/' << 16);
                *(value + (2 + name.Length)) = '>';
                nameEndPools.Set(name, new pointer { Data = value });
                return value;
            }
#endif
        }
        /// <summary>
        /// 对象转换XML字符串
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        internal static class typeToXmler<valueType>
        {
            /// <summary>
            /// 成员转换
            /// </summary>
            private static readonly Action<xmlSerializer, valueType> memberToXmler;
            /// <summary>
            /// 成员转换
            /// </summary>
            private static readonly Action<memberMap, xmlSerializer, valueType> memberMapToXmler;
            /// <summary>
            /// 转换委托
            /// </summary>
            private static readonly Action<xmlSerializer, valueType> defaultToXmler;
            /// <summary>
            /// XML序列化类型配置
            /// </summary>
            private static readonly xmlSerialize attribute;
            /// <summary>
            /// 是否值类型
            /// </summary>
            private static readonly bool isValueType;

            /// <summary>
            /// 对象转换XML字符串
            /// </summary>
            /// <param name="toXmler">对象转换XML字符串</param>
            /// <param name="value">数据对象</param>
            internal static void ToXml(xmlSerializer toXmler, valueType value)
            {
                if (isValueType) StructToXml(toXmler, value);
                else if (value != null) ClassToXml(toXmler, value);
            }
            /// <summary>
            /// 对象转换XML字符串
            /// </summary>
            /// <param name="toXmler">对象转换XML字符串</param>
            /// <param name="value">数据对象</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal static void StructToXml(xmlSerializer toXmler, valueType value)
            {
                if (defaultToXmler == null) MemberToXml(toXmler, value);
                else defaultToXmler(toXmler, value);
            }
            /// <summary>
            /// 引用类型对象转换XML字符串
            /// </summary>
            /// <param name="toXmler">对象转换XML字符串</param>
            /// <param name="value">数据对象</param>
            internal static void ClassToXml(xmlSerializer toXmler, valueType value)
            {
                if (defaultToXmler == null)
                {
                    if (toXmler.push(value))
                    {
                        MemberToXml(toXmler, value);
                        toXmler.pop();
                    }
                }
                else defaultToXmler(toXmler, value);
            }
            /// <summary>
            /// 值类型对象转换XML字符串
            /// </summary>
            /// <param name="toXmler">对象转换XML字符串</param>
            /// <param name="value">数据对象</param>
            internal static void MemberToXml(xmlSerializer toXmler, valueType value)
            {
                //charStream xmlStream = toXmler.CharStream;
                config config = toXmler.Config;
                memberMap memberMap = config.MemberMap;
                if (memberMap == null) memberToXmler(toXmler, value);
                else if (memberMap.Type == memberMap<valueType>.TypeInfo)
                {
                    config.MemberMap = null;
                    try
                    {
                        memberMapToXmler(memberMap, toXmler, value);
                    }
                    finally { config.MemberMap = memberMap; }
                }
                else
                {
                    config.Warning = warning.MemberMap;
                    if (config.IsMemberMapErrorLog) log.Error.Add("Xml序列化成员位图类型匹配失败", null, true);
                    if (config.IsMemberMapErrorToDefault) memberToXmler(toXmler, value);
                }
            }
            /// <summary>
            /// 枚举转换字符串
            /// </summary>
            /// <param name="toXmler">对象转换XML字符串</param>
            /// <param name="value">数据对象</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private static void enumToString(xmlSerializer toXmler, valueType value)
            {
                toXmler.toXml(value.ToString());
            }
            /// <summary>
            /// 不支持序列化
            /// </summary>
            /// <param name="toXmler">对象转换XML字符串</param>
            /// <param name="value">数据对象</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private static void ignore(xmlSerializer toXmler, valueType value)
            {
            }
            static typeToXmler()
            {
                Type type = typeof(valueType);
                MethodInfo methodInfo = xmlSerializer.getToXmlMethod(type);
                if (methodInfo != null)
                {
#if NOJIT
                    defaultToXmler = new methodSerializer(methodInfo).Serialize;
#else
                    DynamicMethod dynamicMethod = new DynamicMethod("toXmler", typeof(void), new Type[] { typeof(xmlSerializer), type }, true);
                    dynamicMethod.InitLocals = true;
                    ILGenerator generator = dynamicMethod.GetILGenerator();
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Call, methodInfo);
                    generator.Emit(OpCodes.Ret);
                    defaultToXmler = (Action<xmlSerializer, valueType>)dynamicMethod.CreateDelegate(typeof(Action<xmlSerializer, valueType>));
#endif
                    isValueType = true;
                    return;
                }
                if (type.IsArray)
                {
                    if (type.GetArrayRank() == 1) defaultToXmler = (Action<xmlSerializer, valueType>)Delegate.CreateDelegate(typeof(Action<xmlSerializer, valueType>), staticTypeToXmler.GetArrayToXmler(type.GetElementType()));
                    else defaultToXmler = ignore;
                    isValueType = true;
                    return;
                }
                if (type.IsEnum)
                {
                    defaultToXmler = enumToString;
                    isValueType = true;
                    return;
                }
                if (type.IsPointer)
                {
                    defaultToXmler = ignore;
                    isValueType = true;
                    return;
                }
                if (type.IsGenericType)
                {
                    Type genericType = type.GetGenericTypeDefinition();
                    if (genericType == typeof(Nullable<>))
                    {
                        defaultToXmler = (Action<xmlSerializer, valueType>)Delegate.CreateDelegate(typeof(Action<xmlSerializer, valueType>), nullableToXmlMethod.MakeGenericMethod(type.GetGenericArguments()));
                        isValueType = true;
                        return;
                    }
                }
                if ((methodInfo = staticTypeToXmler.GetCustomToXmler(type)) != null
                    || (methodInfo = staticTypeToXmler.GetIEnumerableToXmler(type)) != null)
                {
                    defaultToXmler = (Action<xmlSerializer, valueType>)Delegate.CreateDelegate(typeof(Action<xmlSerializer, valueType>), methodInfo);
                    isValueType = true;
                }
                else
                {
                    Type attributeType;
                    attribute = type.customAttribute<xmlSerialize>(out attributeType, true) ?? (type.Name[0] == '<' ? xmlSerialize.AnonymousTypeMember : xmlSerialize.Default);
                    if (type.IsValueType) isValueType = true;
                    else if (attribute != xmlSerialize.Default && attributeType != type)
                    {
                        for (Type baseType = type.BaseType; baseType != typeof(object); baseType = baseType.BaseType)
                        {
                            xmlSerialize baseAttribute = fastCSharp.code.typeAttribute.GetAttribute<xmlSerialize>(baseType, false, true);
                            if (baseAttribute != null)
                            {
                                if (baseAttribute.IsBaseType)
                                {
                                    methodInfo = baseToXmlMethod.MakeGenericMethod(baseType, type);
                                    defaultToXmler = (Action<xmlSerializer, valueType>)Delegate.CreateDelegate(typeof(Action<xmlSerializer, valueType>), methodInfo);
                                    return;
                                }
                                break;
                            }
                        }
                    }
                    subArray<keyValue<fieldIndex, xmlSerialize.member>> fields = staticTypeToXmler.GetFields(memberIndexGroup<valueType>.GetFields(attribute.MemberFilter), attribute);
                    subArray<staticTypeToXmler.propertyInfo> properties = staticTypeToXmler.GetProperties(memberIndexGroup<valueType>.GetProperties(attribute.MemberFilter), attribute);
                    bool isBox = false;
                    if (type.IsValueType && fields.length + properties.length == 1)
                    {
                        boxSerialize boxSerialize = fastCSharp.code.typeAttribute.GetAttribute<boxSerialize>(type, true, true);
                        if (boxSerialize != null && boxSerialize.IsXml) isBox = true;
                    }
#if NOJIT
                    if (isBox) defaultToXmler = memberToXmler = new serializer(ref fields, ref properties).SerializeBox;
                    else
                    {
                        memberToXmler = new serializer(ref fields, ref properties).Serialize;
                        memberMapToXmler = new memberMapSerializer(ref fields, ref properties).Serialize;
                    }
#else
                    staticTypeToXmler.memberDynamicMethod dynamicMethod = new staticTypeToXmler.memberDynamicMethod(type);
                    staticTypeToXmler.memberMapDynamicMethod memberMapDynamicMethod = isBox ? default(staticTypeToXmler.memberMapDynamicMethod) : new staticTypeToXmler.memberMapDynamicMethod(type);
                    foreach (keyValue<fieldIndex, xmlSerialize.member> member in fields)
                    {
                        if (isBox) dynamicMethod.PushBox(member.Key);
                        else
                        {
                            dynamicMethod.Push(member.Key, member.Value);
                            memberMapDynamicMethod.Push(member.Key, member.Value);
                        }
                    }
                    foreach (staticTypeToXmler.propertyInfo member in properties)
                    {
                        if (isBox) dynamicMethod.PushBox(member.Property, member.Method);
                        else
                        {
                            dynamicMethod.Push(member.Property, member.Method, member.Attribute);
                            memberMapDynamicMethod.Push(member.Property, member.Method, member.Attribute);
                        }
                    }
                    memberToXmler = (Action<xmlSerializer, valueType>)dynamicMethod.Create<Action<xmlSerializer, valueType>>();
                    if (isBox) defaultToXmler = memberToXmler;
                    else memberMapToXmler = (Action<memberMap, xmlSerializer, valueType>)memberMapDynamicMethod.Create<Action<memberMap, xmlSerializer, valueType>>();
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
                public void Serialize(xmlSerializer serializer, valueType value)
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
                    /// 集合子节点名称
                    /// </summary>
                    public string ItemName;
                    /// <summary>
                    /// 是否输出判断函数
                    /// </summary>
                    public MethodInfo IsOutputMethod;
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
                    public void Set(keyValue<fieldIndex, xmlSerialize.member> field)
                    {
                        Field = field.Key.Member;
                        Name = field.Key.AnonymousName;
                        if (field.Value != null) ItemName = field.Value.ItemName;
                        MemberIndex = field.Key.MemberIndex;
                        IsOutputMethod = staticTypeToXmler.GetIsOutputMethod(Field.FieldType);
                        SerializeMethod = new pub.methodParameter1(staticTypeToXmler.GetMemberMethodInfo(Field.FieldType));
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
                    /// 集合子节点名称
                    /// </summary>
                    public string ItemName;
                    /// <summary>
                    /// 是否输出判断函数
                    /// </summary>
                    public MethodInfo IsOutputMethod;
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
                    public void Set(staticTypeToXmler.propertyInfo property)
                    {
                        Property = property.Property.Member;
                        GetMethod = Property.GetGetMethod(true);
                        if (property.Attribute != null) ItemName = property.Attribute.ItemName;
                        MemberIndex = property.Property.MemberIndex;
                        IsOutputMethod = staticTypeToXmler.GetIsOutputMethod(Property.PropertyType);
                        SerializeMethod = new pub.methodParameter1(staticTypeToXmler.GetMemberMethodInfo(Property.PropertyType));
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
                public serializer(ref subArray<keyValue<fieldIndex, xmlSerialize.member>> fields, ref subArray<staticTypeToXmler.propertyInfo> properties)
                {
                    this.fields = new field[fields.Count];
                    int index = 0;
                    foreach (keyValue<fieldIndex, xmlSerialize.member> field in fields) this.fields[index++].Set(field);
                    this.properties = new property[properties.Count];
                    index = 0;
                    foreach (staticTypeToXmler.propertyInfo property in properties) this.properties[index++].Set(property);
                }
                /// <summary>
                /// 字段序列化
                /// </summary>
                /// <param name="serializer"></param>
                /// <param name="value"></param>
                /// <param name="field"></param>
                /// <param name="parameters"></param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                protected static void serialize(xmlSerializer serializer, object value, field field, ref object[] parameters)
                {
                    if (parameters == null) parameters = new object[1];
                    parameters[0] = field.Field.GetValue(value);
                    if (field.IsOutputMethod == null || (bool)field.IsOutputMethod.Invoke(serializer, parameters))
                    {
                        charStream charStream = serializer.CharStream;
                        charStream.PrepLength(field.Name.Length + 2);
                        charStream.UnsafeWrite('<');
                        charStream.UnsafeSimpleWrite(field.Name);
                        charStream.UnsafeWrite('>');
                        if (field.ItemName != null) serializer.itemName = field.ItemName;
                        field.SerializeMethod.Invoke(serializer, parameters);
                        charStream.PrepLength(field.Name.Length + 3);
                        charStream.UnsafeWrite('<');
                        charStream.UnsafeWrite('/');
                        charStream.UnsafeSimpleWrite(field.Name);
                        charStream.UnsafeWrite('>');
                    }
                }
                /// <summary>
                /// 属性序列化
                /// </summary>
                /// <param name="serializer"></param>
                /// <param name="value"></param>
                /// <param name="property"></param>
                /// <param name="parameters"></param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                protected static void serialize(xmlSerializer serializer, object value, property property, ref object[] parameters)
                {
                    if (parameters == null) parameters = new object[1];
                    parameters[0] = property.GetMethod.Invoke(value, null);
                    if (property.IsOutputMethod == null || (bool)property.IsOutputMethod.Invoke(serializer, parameters))
                    {
                        PropertyInfo propertyInfo = property.Property;
                        charStream charStream = serializer.CharStream;
                        charStream.PrepLength(propertyInfo.Name.Length + 2);
                        charStream.UnsafeWrite('<');
                        charStream.UnsafeSimpleWrite(propertyInfo.Name);
                        charStream.UnsafeWrite('>');
                        if (property.ItemName != null) serializer.itemName = property.ItemName;
                        property.SerializeMethod.Invoke(serializer, parameters);
                        charStream.PrepLength(propertyInfo.Name.Length + 3);
                        charStream.UnsafeWrite('<');
                        charStream.UnsafeWrite('/');
                        charStream.UnsafeSimpleWrite(propertyInfo.Name);
                        charStream.UnsafeWrite('>');
                    }
                }
                /// <summary>
                /// 序列化
                /// </summary>
                /// <param name="serializer"></param>
                /// <param name="value"></param>
                public void Serialize(xmlSerializer serializer, valueType value)
                {
                    object[] parameters = null;
                    object objectValue = value;
                    foreach (field field in fields) serialize(serializer, objectValue, field, ref parameters);
                    foreach (property property in properties) serialize(serializer, objectValue, property, ref parameters);
                }
                /// <summary>
                /// 序列化
                /// </summary>
                /// <param name="serializer"></param>
                /// <param name="value"></param>
                public void SerializeBox(xmlSerializer serializer, valueType value)
                {
                    if (fields.Length == 0)
                    {
                        property property = properties[0];
                        object[] parameters = new object[] { property.GetMethod.Invoke(value, null) };
                        if (property.IsOutputMethod == null || (bool)property.IsOutputMethod.Invoke(serializer, parameters))
                        {
                            if (property.ItemName != null) serializer.itemName = property.ItemName;
                            property.SerializeMethod.Invoke(serializer, parameters);
                        }
                    }
                    else
                    {
                        field field = fields[0];
                        object[] parameters = new object[] { field.Field.GetValue(value) };
                        if (field.IsOutputMethod == null || (bool)field.IsOutputMethod.Invoke(serializer, parameters))
                        {
                            if (field.ItemName != null) serializer.itemName = field.ItemName;
                            field.SerializeMethod.Invoke(serializer, parameters);
                        }
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
                public memberMapSerializer(ref subArray<keyValue<fieldIndex, xmlSerialize.member>> fields, ref subArray<staticTypeToXmler.propertyInfo> properties) : base(ref fields, ref properties) { }
                /// <summary>
                /// 序列化
                /// </summary>
                /// <param name="memberMap"></param>
                /// <param name="serializer"></param>
                /// <param name="value"></param>
                public void Serialize(memberMap memberMap, xmlSerializer serializer, valueType value)
                {
                    object[] parameters = null;
                    object objectValue = value;
                    foreach (field field in fields)
                    {
                        if (memberMap.IsMember(field.MemberIndex)) serialize(serializer, objectValue, field, ref parameters);
                    }
                    foreach (property property in properties)
                    {
                        if (memberMap.IsMember(property.MemberIndex)) serialize(serializer, objectValue, property, ref parameters);
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
        /// 字符状态位查询表格
        /// </summary>
        private readonly byte* bits = xmlParser.Bits.Byte;
        /// <summary>
        /// 集合子节点名称
        /// </summary>
        private string itemName;
        /// <summary>
        /// 集合子节点名称字段
        /// </summary>
        private static readonly FieldInfo itemNameField = typeof(xmlSerializer).GetField("itemName", BindingFlags.Instance | BindingFlags.NonPublic);
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
        /// 配置参数
        /// </summary>
        public config UnsafeConfig
        {
            get { return Config; }
        }
        /// <summary>
        /// 对象转换XML字符串
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="value">数据对象</param>
        /// <param name="config">配置参数</param>
        /// <returns>Xml字符串</returns>
        private string toXml<valueType>(valueType value, config config)
        {
            Config = config;
            pointer buffer = fastCSharp.unmanagedPool.StreamBuffers.Get();
            try
            {
                CharStream.UnsafeReset((byte*)buffer.Char, fastCSharp.unmanagedPool.StreamBuffers.Size);
                using (CharStream)
                {
                    toXml(value);
                    return CharStream.ToString();
                }
            }
            finally
            {
                fastCSharp.unmanagedPool.StreamBuffers.Push(ref buffer);
            }
        }
        /// <summary>
        /// 对象转换XML字符串
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="value">数据对象</param>
        /// <param name="xmlStream">Xml输出缓冲区</param>
        /// <param name="config">配置参数</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toXml<valueType>(valueType value, charStream xmlStream, config config)
        {
            Config = config;
            CharStream.From(xmlStream);
            try
            {
                toXml(value);
            }
            finally { xmlStream.From(CharStream); }
        }
        /// <summary>
        /// 对象转换XML字符串
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="value">数据对象</param>
        private void toXml<valueType>(valueType value)
        {
            if (Config.CheckLoopDepth <= 0)
            {
                checkLoopDepth = 0;
                if (forefather == null) forefather = new object[sizeof(int)];
            }
            else checkLoopDepth = Config.CheckLoopDepth;
            CharStream.Write(Config.Header);
            fixed (char* nameFixed = Config.BootNodeName)
            {
                nameStart(nameFixed, Config.BootNodeName.Length);
                typeToXmler<valueType>.ToXml(this, value);
                nameEnd(nameFixed, Config.BootNodeName.Length);
            }
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        private void free()
        {
            Config = null;
            itemName = null;
            typePool<xmlSerializer>.PushNotNull(this);
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
                        if (arrayValue == objectValue) return false;
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
            else if (--checkLoopDepth == 0) fastCSharp.log.Default.Throw(log.exceptionType.IndexOutOfRange);
            return true;
        }
        /// <summary>
        /// 退出对象节点
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void pop()
        {
            if (checkLoopDepth == 0) forefather[--forefatherCount] = null;
            else ++checkLoopDepth;
        }
        /// <summary>
        /// 标签开始
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        private void nameStart(char* start, int length)
        {
            char* data = CharStream.GetPrepLengthCurrent(length + (2 + 2));
            *data = '<';
            fastCSharp.unsafer.memory.UnsafeSimpleCopy(start, ++data, length);
            *(data + length) = '>';
            CharStream.UnsafeAddLength(length + 2);
        }
        /// <summary>
        /// 标签结束
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        private void nameEnd(char* start, int length)
        {
            char* data = CharStream.GetPrepLengthCurrent(length + (3 + 2));
            *(int*)data = '<' + ('/' << 16);
            fastCSharp.unsafer.memory.UnsafeSimpleCopy(start, data + 2, length);
            *(data + (length + 2)) = '>';
            CharStream.UnsafeAddLength(length + 3);
        }
        /// <summary>
        /// 逻辑值转换
        /// </summary>
        /// <param name="value">逻辑值</param>
        [toXmlMethod]
        private void toXml(bool value)
        {
            if (value)
            {
                *(long*)CharStream.GetPrepLengthCurrent(4) = 'T' + ('r' << 16) + ((long)'u' << 32) + ((long)'e' << 48);
                CharStream.UnsafeAddLength(4);
            }
            else
            {
                byte* chars = (byte*)CharStream.GetPrepLengthCurrent(5);
                *(long*)chars = 'F' + ('a' << 16) + ((long)'l' << 32) + ((long)'s' << 48);
                *(char*)(chars + sizeof(long)) = 'e';
                CharStream.UnsafeAddLength(5);
            }
        }
        /// <summary>
        /// 逻辑值转换
        /// </summary>
        /// <param name="value">逻辑值</param>
        [toXmlMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toXml(bool? value)
        {
            if (value.HasValue) toXml((bool)value);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toXmlMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toXml(byte value)
        {
            fastCSharp.number.ToString(value, CharStream);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toXmlMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toXml(byte? value)
        {
            if (value.HasValue) fastCSharp.number.ToString((byte)value, CharStream);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toXmlMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toXml(sbyte value)
        {
            fastCSharp.number.ToString(value, CharStream);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toXmlMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toXml(sbyte? value)
        {
            if (value.HasValue) fastCSharp.number.ToString((sbyte)value, CharStream);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toXmlMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toXml(short value)
        {
            fastCSharp.number.ToString(value, CharStream);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toXmlMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toXml(short? value)
        {
            if (value.HasValue) fastCSharp.number.ToString((short)value, CharStream);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toXmlMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toXml(ushort value)
        {
            fastCSharp.number.ToString(value, CharStream);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toXmlMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toXml(ushort? value)
        {
            if (value.HasValue) fastCSharp.number.ToString((ushort)value, CharStream);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toXmlMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toXml(int value)
        {
            fastCSharp.number.ToString(value, CharStream);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toXmlMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toXml(int? value)
        {
            if (value.HasValue) fastCSharp.number.ToString((int)value, CharStream);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toXmlMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toXml(uint value)
        {
            fastCSharp.number.ToString(value, CharStream);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toXmlMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toXml(uint? value)
        {
            if (value.HasValue) fastCSharp.number.ToString((uint)value, CharStream);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toXmlMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toXml(long value)
        {
            fastCSharp.number.ToString(value, CharStream);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toXmlMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toXml(long? value)
        {
            if (value.HasValue) toXml((long)value);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toXmlMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toXml(ulong value)
        {
            fastCSharp.number.ToString(value, CharStream);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toXmlMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toXml(ulong? value)
        {
            if (value.HasValue) toXml((ulong)value);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toXmlMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toXml(float value)
        {
            if (float.IsNaN(value)) fastCSharp.web.ajax.WriteNaN(CharStream);
            else CharStream.SimpleWriteNotNull(value.ToString());
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toXmlMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toXml(float? value)
        {
            if (value.HasValue) toXml(value.Value);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toXmlMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toXml(double value)
        {
            if (double.IsNaN(value)) fastCSharp.web.ajax.WriteNaN(CharStream);
            else CharStream.SimpleWriteNotNull(value.ToString());
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toXmlMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toXml(double? value)
        {
            if (value.HasValue) toXml(value.Value);
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toXmlMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toXml(decimal value)
        {
            CharStream.SimpleWriteNotNull(value.ToString());
        }
        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="value">数字</param>
        [toXmlMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toXml(decimal? value)
        {
            if (value.HasValue) toXml(value.Value);
        }
        /// <summary>
        /// 字符转换
        /// </summary>
        /// <param name="value">字符</param>
        [toXmlMethod]
        private void toXml(char value)
        {
            if (((bits[(byte)value] & xmlParser.EncodeSpaceBit) | (value & 0xff00)) == 0)
            {
                switch ((byte)value)
                {
                    case (byte)'\t':
                        *(long*)CharStream.GetPrepLengthCurrent(4) = '&' + ('#' << 16) + ((long)'9' << 32) + ((long)';' << 48);
                        CharStream.UnsafeAddLength(4);
                        return;
                    case (byte)'\n':
                        byte* data10 = (byte*)CharStream.GetPrepLengthCurrent(5);
                        *(long*)data10 = '&' + ('#' << 16) + ((long)'1' << 32) + ((long)'0' << 48);
                        *(char*)(data10 + sizeof(long)) = ';';
                        CharStream.UnsafeAddLength(5);
                        return;
                    case (byte)'\r':
                        byte* data13 = (byte*)CharStream.GetPrepLengthCurrent(5);
                        *(long*)data13 = '&' + ('#' << 16) + ((long)'1' << 32) + ((long)'3' << 48);
                        *(char*)(data13 + sizeof(long)) = ';';
                        CharStream.UnsafeAddLength(5);
                        return;
                    case (byte)' ':
                        byte* data32 = (byte*)CharStream.GetPrepLengthCurrent(5);
                        *(long*)data32 = '&' + ('#' << 16) + ((long)'3' << 32) + ((long)'2' << 48);
                        *(char*)(data32 + sizeof(long)) = ';';
                        CharStream.UnsafeAddLength(5);
                        return;
                    case (byte)'&':
                        byte* data38 = (byte*)CharStream.GetPrepLengthCurrent(5);
                        *(long*)data38 = '&' + ('#' << 16) + ((long)'3' << 32) + ((long)'8' << 48);
                        *(char*)(data38 + sizeof(long)) = ';';
                        CharStream.UnsafeAddLength(5);
                        return;
                    case (byte)'<':
                        *(long*)CharStream.GetPrepLengthCurrent(4) = '&' + ('l' << 16) + ((long)'t' << 32) + ((long)';' << 48);
                        CharStream.UnsafeAddLength(4);
                        return;
                    case (byte)'>':
                        *(long*)CharStream.GetPrepLengthCurrent(4) = '&' + ('g' << 16) + ((long)'t' << 32) + ((long)';' << 48);
                        CharStream.UnsafeAddLength(4);
                        return;
                }
            }
            CharStream.Write(value);
        }
        /// <summary>
        /// 字符转换
        /// </summary>
        /// <param name="value">字符</param>
        [toXmlMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toXml(char? value)
        {
            if (value.HasValue) toXml((char)value);
        }
        /// <summary>
        /// 时间转换
        /// </summary>
        /// <param name="value">时间</param>
        [toXmlMethod]
        private void toXml(DateTime value)
        {
            CharStream.PrepLength(fastCSharp.date.SqlMillisecondSize);
            date.ToSqlMillisecond(value, CharStream);
        }
        /// <summary>
        /// 时间转换
        /// </summary>
        /// <param name="value">时间</param>
        [toXmlMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toXml(DateTime? value)
        {
            if (value.HasValue) toXml((DateTime)value);
        }
        /// <summary>
        /// Guid转换
        /// </summary>
        /// <param name="value">Guid</param>
        [toXmlMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toXml(Guid value)
        {
            new guid { Value = value }.ToString(CharStream.GetPrepLengthCurrent(36));
            CharStream.UnsafeAddLength(36);
        }
        /// <summary>
        /// Guid转换
        /// </summary>
        /// <param name="value">Guid</param>
        [toXmlMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toXml(Guid? value)
        {
            if (value.HasValue) toXml((Guid)value);
        }
        /// <summary>
        /// 字符串转换
        /// </summary>
        /// <param name="value">字符串</param>
        [toXmlMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toXml(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                fixed (char* valueFixed = value) toXml(valueFixed, value.Length);
            }
        }
        /// <summary>
        /// 字符串转换
        /// </summary>
        /// <param name="value">字符串</param>
        [toXmlMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void toXml(subString value)
        {
            if (value.Length != 0)
            {
                fixed (char* valueFixed = value.value) toXml(valueFixed + value.StartIndex, value.Length);
            }
        }
        /// <summary>
        /// 计算编码增加长度
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private int encodeSpaceSize(char value)
        {
            if (((bits[(byte)value] & xmlParser.EncodeSpaceBit) | (value & 0xff00)) == 0)
            {
                switch (value & 7)
                {
                    case '&' & 7:  //26 00100110
                    //case '>' & 7://3e 00111110
                        return 4 - ((value >> 4) & 1);
                    case '\n' & 7:
                    case '\r' & 7:
                    case ' ' & 7:
                        return 4;
                    case '\t' & 7:
                    case '<' & 7:
                        return 3;
                }
            }
            return 0;
        }
        /// <summary>
        /// 字符转换
        /// </summary>
        /// <param name="data"></param>
        /// <param name="value">字符</param>
        private void encodeSpace(ref byte* data, char value)
        {
            if (((bits[(byte)value] & xmlParser.EncodeSpaceBit) | (value & 0xff00)) == 0)
            {
                switch ((byte)value)
                {
                    case (byte)'\t':
                        *(long*)data = '&' + ('#' << 16) + ((long)'9' << 32) + ((long)';' << 48);
                        data += sizeof(long);
                        return;
                    case (byte)'\n':
                        *(long*)data = '&' + ('#' << 16) + ((long)'1' << 32) + ((long)'0' << 48);
                        *(char*)(data + sizeof(long)) = ';';
                        data += sizeof(long) + sizeof(char);
                        return;
                    case (byte)'\r':
                        *(long*)data = '&' + ('#' << 16) + ((long)'1' << 32) + ((long)'3' << 48);
                        *(char*)(data + sizeof(long)) = ';';
                        data += sizeof(long) + sizeof(char);
                        return;
                    case (byte)' ':
                        *(long*)data = '&' + ('#' << 16) + ((long)'3' << 32) + ((long)'2' << 48);
                        *(char*)(data + sizeof(long)) = ';';
                        data += sizeof(long) + sizeof(char);
                        return;
                    case (byte)'&':
                        *(long*)data = '&' + ('#' << 16) + ((long)'3' << 32) + ((long)'8' << 48);
                        *(char*)(data + sizeof(long)) = ';';
                        data += sizeof(long) + sizeof(char);
                        return;
                    case (byte)'<':
                        *(long*)data = '&' + ('l' << 16) + ((long)'t' << 32) + ((long)';' << 48);
                        data += sizeof(long);
                        return;
                    case (byte)'>':
                        *(long*)data = '&' + ('g' << 16) + ((long)'t' << 32) + ((long)';' << 48);
                        data += sizeof(long);
                        return;
                }
            }
            *(char*)data = value;
            data += sizeof(char);
        }
        /// <summary>
        /// 字符串转换
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        private void toXml(char* start, int length)
        {
            switch (length)
            {
                case 1:
                    toXml(*start);
                    return;
                case 2:
                    toXml(*start);
                    toXml(*(start + 1));
                    return;
            }
            char* end = start + (length - 1);
            int addLength = 0;
            if (length > 8)
            {
                int isCData = *end == '>' && *(int*)(end - 2) == ']' + (']' << 16) ? 0 : 1;
                for (char* code = start + 2; code != end; ++code)
                {
                    if (((bits[*(byte*)code] & xmlParser.EncodeBit) | *(((byte*)code) + 1)) == 0)
                    {
                        switch ((*(byte*)code >> 1) & 7)
                        {
                            case ('&' >> 1) & 7:
                                addLength += 4;
                                break;
                            case ('<' >> 1) & 7:
                                addLength += 3;
                                break;
                            case ('>' >> 1) & 7:
                                addLength += 3;
                                if (*(int*)(code - 2) == ']' + (']' << 16)) isCData = 0;
                                break;
                        }
                    }
                }
                if (isCData == 0)
                {
                    byte* code = (byte*)(start + 1);
                    if (((bits[*(byte*)code] & xmlParser.EncodeBit) | *(((byte*)code) + 1)) == 0)
                    {
                        //& 26 00100110
                        //< 3c 00111100
                        //> 3e 00111110
                        addLength += 4 - ((*(byte*)code >> 4) & 1);
                    }
                }
                else
                {
                    if (addLength == 0)
                    {
                        addLength += encodeSpaceSize(*start) + encodeSpaceSize(*(start + 1)) + encodeSpaceSize(*end);
                    }
                    if (addLength == 0) CharStream.Write(start, length);
                    else
                    {
                        byte* write = (byte*)CharStream.GetPrepLengthCurrent(length + 13);
                        *(long*)write = '<' + ('!' << 16) + ((long)'[' << 32) + ((long)'C' << 48);
                        *(long*)(write + sizeof(long)) = 'D' + ('A' << 16) + ((long)'T' << 32) + ((long)'A' << 48);
                        *(char*)(write + sizeof(long) * 2) = '[';
                        fastCSharp.unsafer.memory.Copy(start, write + (sizeof(long) * 2 + sizeof(char)), length << 1);
                        *(long*)(write + (sizeof(long) * 2 + sizeof(char)) + (length << 1)) = ']' + (']' << 16) + ((long)'>' << 32);
                        CharStream.UnsafeAddLength(length + 12);
                    }
                    return;
                }
            }
            else
            {
                for (char* code = start + 1; code != end; ++code)
                {
                    if (((bits[*(byte*)code] & xmlParser.EncodeBit) | *(((byte*)code) + 1)) == 0)
                    {
                        //& 26 00100110
                        //< 3c 00111100
                        //> 3e 00111110
                        addLength += 4 - ((*(byte*)code >> 4) & 1);
                    }
                }
            }
            if ((addLength += encodeSpaceSize(*start) + encodeSpaceSize(*end)) == 0) CharStream.Write(start, length);
            else
            {
                byte* write = (byte*)CharStream.GetPrepLengthCurrent(length + addLength);
                encodeSpace(ref write, *start++);
                do
                {
                    if (((bits[*(byte*)start] & xmlParser.EncodeBit) | *(((byte*)start) + 1)) == 0)
                    {
                        switch ((*(byte*)start >> 1) & 7)
                        {
                            case ('&' >> 1) & 7:
                                *(long*)write = '&' + ('#' << 16) + ((long)'3' << 32) + ((long)'8' << 48);
                                *(char*)(write + sizeof(long)) = ';';
                                write += sizeof(long) + sizeof(char);
                                break;
                            case ('<' >> 1) & 7:
                                *(long*)write = '&' + ('l' << 16) + ((long)'t' << 32) + ((long)';' << 48);
                                write += sizeof(long);
                                break;
                            case ('>' >> 1) & 7:
                                *(long*)write = '&' + ('g' << 16) + ((long)'t' << 32) + ((long)';' << 48);
                                write += sizeof(long);
                                break;
                        }
                    }
                    else
                    {
                        *(char*)write = *start;
                        write += sizeof(char);
                    }
                }
                while (++start != end);
                encodeSpace(ref write, *start);
                CharStream.UnsafeAddLength(length + addLength);
            }
        }
        /// <summary>
        /// 字符串转换
        /// </summary>
        /// <param name="value">XML节点</param>
        [toXmlMethod]
        private void toXml(xmlNode value)
        {
            switch (value.Type)
            {
                case xmlNode.type.String:
                    toXml(value.String);
                    return;
                case xmlNode.type.EncodeString:
                    CharStream.Write(value.String);
                    return;
                case xmlNode.type.TempString:
                    CharStream.Write(value.String);
                    return;
                case xmlNode.type.ErrorString:
                    return;
                case xmlNode.type.Node:
                    foreach (keyValue<subString, xmlNode> node in value.Nodes)
                    {
                        fixed (char* nameFixed = node.Key.value)
                        {
                            nameStart(nameFixed + node.Key.StartIndex, node.Key.Length);
                            toXml(node.Value);
                            nameEnd(nameFixed + node.Key.StartIndex, node.Key.Length);
                        }
                    }
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
        [toXmlMethod]
        private void toXml(object value)
        {
        }
        /// <summary>
        /// 是否输出对象
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private bool isOutput(object value)
        {
            return value != null || Config.IsOutputNull;
        }
        /// <summary>
        /// 是否输出对象函数信息
        /// </summary>
        private static readonly MethodInfo isOutputMethod = typeof(xmlSerializer).GetMethod("isOutput", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 是否输出字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private bool isOutputString(string value)
        {
            if (value == null) return Config.IsOutputNull && Config.IsOutputEmptyString;
            return value.Length != 0 || Config.IsOutputEmptyString;
        }
        /// <summary>
        /// 是否输出字符串函数信息
        /// </summary>
        private static readonly MethodInfo isOutputStringMethod = typeof(xmlSerializer).GetMethod("isOutputString", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 是否输出字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private bool isOutputSubString(subString value)
        {
            return value.Length != 0 || Config.IsOutputEmptyString;
        }
        /// <summary>
        /// 是否输出字符串函数信息
        /// </summary>
        private static readonly MethodInfo isOutputSubStringMethod = typeof(xmlSerializer).GetMethod("isOutputSubString", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 是否输出可空对象
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private bool isOutputNullable<valueType>(Nullable<valueType> value) where valueType : struct
        {
            return value.HasValue || Config.IsOutputNull;
        }
        /// <summary>
        /// 是否输出可空对象函数信息
        /// </summary>
        private static readonly MethodInfo isOutputNullableMethod = typeof(xmlSerializer).GetMethod("isOutputNullable", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// 值类型对象转换XML字符串
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void nullableToXml<valueType>(Nullable<valueType> value) where valueType : struct
        {
            if (value.HasValue) typeToXmler<valueType>.StructToXml(this, value.Value);
        }
        /// <summary>
        /// 值类型对象转换函数信息
        /// </summary>
        private static readonly MethodInfo nullableToXmlMethod = typeof(xmlSerializer).GetMethod("nullableToXml", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        private void structArray<valueType>(valueType[] array)
        {
            if (array != null && push(array))
            {
                //charStream xmlStream = CharStream;
                string itemName = ItemName;
                fixed (char* itemNameFixed = itemName)
                {
                    int itemNameLength = itemName.Length;
                    foreach (valueType value in array)
                    {
                        nameStart(itemNameFixed, itemNameLength);
                        typeToXmler<valueType>.StructToXml(this, value);
                        nameEnd(itemNameFixed, itemNameLength);
                    }
                }
                pop();
            }
        }
        /// <summary>
        /// 字典转换函数信息
        /// </summary>
        private static readonly MethodInfo structArrayMethod = typeof(xmlSerializer).GetMethod("structArray", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <param name="array">数组对象</param>
        private void array<valueType>(valueType[] array)
        {
            if (array != null && push(array))
            {
                //charStream xmlStream = CharStream;
                string itemName = ItemName;
                fixed (char* itemNameFixed = itemName)
                {
                    int itemNameLength = itemName.Length;
                    foreach (valueType value in array)
                    {
                        nameStart(itemNameFixed, itemNameLength);
                        if (value != null)
                        {
                            int length = CharStream.Length;
                            typeToXmler<valueType>.ClassToXml(this, value);
                            if (length == CharStream.Length) CharStream.Write(' ');
                        }
                        nameEnd(itemNameFixed, itemNameLength);
                    }
                }
                pop();
            }
        }
        /// <summary>
        /// 字典转换函数信息
        /// </summary>
        private static readonly MethodInfo arrayMethod = typeof(xmlSerializer).GetMethod("array", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举集合转换
        /// </summary>
        /// <param name="values">枚举集合</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void structStructEnumerable<valueType, elementType>(valueType values) where valueType : IEnumerable<elementType>
        {
            //charStream xmlStream = CharStream;
            string itemName = ItemName;
            fixed (char* itemNameFixed = itemName)
            {
                int itemNameLength = itemName.Length;
                foreach (elementType value in values)
                {
                    nameStart(itemNameFixed, itemNameLength);
                    typeToXmler<elementType>.StructToXml(this, value);
                    nameEnd(itemNameFixed, itemNameLength);
                }
            }
        }
        /// <summary>
        /// 字典转换函数信息
        /// </summary>
        private static readonly MethodInfo structStructEnumerableMethod = typeof(xmlSerializer).GetMethod("structStructEnumerable", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举集合转换
        /// </summary>
        /// <param name="values">枚举集合</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void structClassEnumerable<valueType, elementType>(valueType values) where valueType : IEnumerable<elementType>
        {
            //charStream xmlStream = CharStream;
            string itemName = ItemName;
            fixed (char* itemNameFixed = itemName)
            {
                int itemNameLength = itemName.Length;
                foreach (elementType value in values)
                {
                    nameStart(itemNameFixed, itemNameLength);
                    if (value != null)
                    {
                        int length = CharStream.Length;
                        typeToXmler<elementType>.ClassToXml(this, value);
                        if (length == CharStream.Length) CharStream.Write(' ');
                    }
                    nameEnd(itemNameFixed, itemNameLength);
                }
            }
        }
        /// <summary>
        /// 字典转换函数信息
        /// </summary>
        private static readonly MethodInfo structClassEnumerableMethod = typeof(xmlSerializer).GetMethod("structClassEnumerable", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举集合转换
        /// </summary>
        /// <param name="value">枚举集合</param>
        private void classStructEnumerable<valueType, elementType>(valueType value) where valueType : IEnumerable<elementType>
        {
            if (value != null && push(value))
            {
                structStructEnumerable<valueType, elementType>(value);
                pop();
            }
        }
        /// <summary>
        /// 字典转换函数信息
        /// </summary>
        private static readonly MethodInfo classStructEnumerableMethod = typeof(xmlSerializer).GetMethod("classStructEnumerable", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 枚举集合转换
        /// </summary>
        /// <param name="value">枚举集合</param>
        private void classClassEnumerable<valueType, elementType>(valueType value) where valueType : IEnumerable<elementType>
        {
            if (value != null && push(value))
            {
                structClassEnumerable<valueType, elementType>(value);
                pop();
            }
        }
        /// <summary>
        /// 字典转换函数信息
        /// </summary>
        private static readonly MethodInfo classClassEnumerableMethod = typeof(xmlSerializer).GetMethod("classClassEnumerable", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 引用类型对象转换XML字符串
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void classToXml<valueType>(valueType value)
        {
            if (value != null) typeToXmler<valueType>.ClassToXml(this, value);
        }
        /// <summary>
        /// 字典转换函数信息
        /// </summary>
        private static readonly MethodInfo classToXmlMethod = typeof(xmlSerializer).GetMethod("classToXml", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 值类型对象转换XML字符串
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void structToXml<valueType>(valueType value)
        {
            typeToXmler<valueType>.StructToXml(this, value);
        }
        /// <summary>
        /// 字典转换函数信息
        /// </summary>
        private static readonly MethodInfo structToXmlMethod = typeof(xmlSerializer).GetMethod("structToXml", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 值类型对象转换XML字符串
        /// </summary>
        /// <param name="toXmler">对象转换XML字符串</param>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static void nullableMemberToXml<valueType>(xmlSerializer toXmler, Nullable<valueType> value) where valueType : struct
        {
            if (value.HasValue) typeToXmler<valueType>.StructToXml(toXmler, value.Value);
        }
        /// <summary>
        /// 字典转换函数信息
        /// </summary>
        private static readonly MethodInfo nullableMemberToXmlMethod = typeof(xmlSerializer).GetMethod("nullableMemberToXml", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 字符串转换
        /// </summary>
        /// <param name="value">数据对象</param>
        private void enumToString<valueType>(valueType value)
        {
            toXml(value.ToString());
        }
        /// <summary>
        /// 字典转换函数信息
        /// </summary>
        private static readonly MethodInfo enumToStringMethod = typeof(xmlSerializer).GetMethod("enumToString", BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// 基类转换
        /// </summary>
        /// <param name="toXmler">对象转换XML字符串</param>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static void baseToXml<valueType, childType>(xmlSerializer toXmler, childType value) where childType : valueType
        {
            typeToXmler<valueType>.ClassToXml(toXmler, value);
        }
        /// <summary>
        /// 基类转换函数信息
        /// </summary>
        private static readonly MethodInfo baseToXmlMethod = typeof(xmlSerializer).GetMethod("baseToXml", BindingFlags.Static | BindingFlags.NonPublic);

        /// <summary>
        /// 公共默认配置参数
        /// </summary>
        private static readonly config defaultConfig = new config { CheckLoopDepth = fastCSharp.config.appSetting.SerializeDepth };
        /// <summary>
        /// 对象转换XML字符串
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="value">数据对象</param>
        /// <param name="xmlStream">Xml输出缓冲区</param>
        /// <param name="config">配置参数</param>
        public static void ToXml<valueType>(valueType value, charStream xmlStream, config config = null)
        {
            if (xmlStream == null) log.Default.Throw(log.exceptionType.Null);
            xmlSerializer toXmler = typePool<xmlSerializer>.Pop() ?? new xmlSerializer();
            try
            {
                toXmler.toXml<valueType>(value, xmlStream, config ?? defaultConfig);
            }
            finally { toXmler.free(); }
        }
        /// <summary>
        /// 对象转换XML字符串
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="value">数据对象</param>
        /// <param name="config">配置参数</param>
        /// <returns>Xml字符串</returns>
        public static string ToXml<valueType>(valueType value, config config = null)
        {
            xmlSerializer toXmler = typePool<xmlSerializer>.Pop() ?? new xmlSerializer();
            try
            {
                return toXmler.toXml<valueType>(value, config ?? defaultConfig);
            }
            finally { toXmler.free(); }
        }
        /// <summary>
        /// 未知类型对象转换XML字符串
        /// </summary>
        /// <typeparam name="valueType">目标数据类型</typeparam>
        /// <param name="value">数据对象</param>
        /// <param name="config">配置参数</param>
        /// <returns>Xml字符串</returns>
        private static string objectToXml<valueType>(object value, config config)
        {
            xmlSerializer toXmler = typePool<xmlSerializer>.Pop() ?? new xmlSerializer();
            try
            {
                return toXmler.toXml<valueType>((valueType)value, config ?? defaultConfig);
            }
            finally { toXmler.free(); }
        }
        /// <summary>
        /// 未知类型对象转换XML字符串
        /// </summary>
        /// <param name="value">数据对象</param>
        /// <param name="config">配置参数</param>
        /// <returns>Xml字符串</returns>
        public static string ObjectToXml(object value, config config = null)
        {
            if (value == null) return "<xml></xml>";
            Type type = value.GetType();
            Func<object, config, string> toXml;
            if (!objectToXmls.TryGetValue(type, out toXml))
            {
                objectToXmls.Set(type, toXml = (Func<object, config, string>)Delegate.CreateDelegate(typeof(Func<object, config, string>), objectToXmlMethod.MakeGenericMethod(type)));
            }
            return toXml(value, config);
        }
        /// <summary>
        /// 未知类型对象转换XML字符串
        /// </summary>
        private static readonly interlocked.dictionary<Type, Func<object, config, string>> objectToXmls = new interlocked.dictionary<Type, Func<object, config, string>>();
        /// <summary>
        /// 未知类型对象转换XML字符串函数信息
        /// </summary>
        private static readonly MethodInfo objectToXmlMethod = typeof(xmlSerializer).GetMethod("objectToXml", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(object), typeof(config) }, null);
        /// <summary>
        /// 基本类型转换函数
        /// </summary>
        private static readonly Dictionary<Type, MethodInfo> toXmlMethods;
        /// <summary>
        /// 获取基本类型转换函数
        /// </summary>
        /// <param name="type">基本类型</param>
        /// <returns>转换函数</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static MethodInfo getToXmlMethod(Type type)
        {
            MethodInfo method;
            return toXmlMethods.TryGetValue(type, out method) ? method : null;
        }
        static xmlSerializer()
        {
            byte* bits = xmlParser.Bits.Byte;
            bits['\t'] &= xmlParser.EncodeSpaceBit ^ 255;
            bits['\r'] &= xmlParser.EncodeSpaceBit ^ 255;
            bits['\n'] &= xmlParser.EncodeSpaceBit ^ 255;
            bits[' '] &= xmlParser.EncodeSpaceBit ^ 255;
            bits['&'] &= (xmlParser.EncodeSpaceBit | xmlParser.EncodeBit) ^ 255;
            bits['<'] &= (xmlParser.EncodeSpaceBit | xmlParser.EncodeBit) ^ 255;
            bits['>'] &= (xmlParser.EncodeSpaceBit | xmlParser.EncodeBit) ^ 255;

            toXmlMethods = fastCSharp.dictionary.CreateOnly<Type, MethodInfo>();
            foreach (MethodInfo method in typeof(xmlSerializer).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (method.customAttribute<toXmlMethod>() != null)
                {
                    toXmlMethods.Add(method.GetParameters()[0].ParameterType, method);
                }
            }
        }

    }
}
