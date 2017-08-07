using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace fastCSharp.reflection
{
    /// <summary>
    /// 类型扩展操作
    /// </summary>
    public static class type
    {
        /// <summary>
        /// 类型名称泛型分隔符
        /// </summary>
        public const char GenericSplit = '`';
        /// <summary>
        /// 类型名称集合
        /// </summary>
        private static readonly Dictionary<Type, string> typeNames;
        /// <summary>
        /// 获取基本类型简称
        /// </summary>
        /// <param name="type">基本类型</param>
        /// <returns>基本类型简称,失败返回null</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static string getTypeName(this Type type)
        {
            string name;
            return typeNames.TryGetValue(type, out name) ? name : null;
        }
        /// <summary>
        /// 类型名称生成器
        /// </summary>
        internal struct nameBuilder
        {
            /// <summary>
            /// 名称缓存
            /// </summary>
            public charStream NameStream;
            /// <summary>
            /// 是否XML注释文档名称
            /// </summary>
            public bool IsXml;
            /// <summary>
            /// 获取XML注释文档名称
            /// </summary>
            /// <param name="type"></param>
            public void Xml(Type type)
            {
                if (type.IsArray) array(type, true);
                else if (type.IsGenericType) genericFullName(type);
                else
                {
                    Type reflectedType = type.ReflectedType;
                    if (reflectedType == null)
                    {
                        NameStream.SimpleWriteNotNull(type.Namespace);
                        NameStream.Write('.');
                        NameStream.SimpleWriteNotNull(type.Name);
                    }
                    else this.reflectedType(type, reflectedType);
                }
            }
            /// <summary>
            /// 获取类型名称
            /// </summary>
            /// <param name="type">类型</param>
            /// <returns>类型名称</returns>
            public unsafe string GetFullName(Type type)
            {
                if (type.IsArray)
                {
                    pointer buffer = fastCSharp.unmanagedPool.TinyBuffers.Get();
                    try
                    {
                        using (NameStream = new charStream(buffer.Char, fastCSharp.unmanagedPool.TinyBuffers.Size >> 1))
                        {
                            array(type, true);
                            return NameStream.ToString();
                        }
                    }
                    finally { fastCSharp.unmanagedPool.TinyBuffers.Push(ref buffer); }
                }
                if (type.IsGenericType)
                {
                    pointer buffer = fastCSharp.unmanagedPool.TinyBuffers.Get();
                    try
                    {
                        using (NameStream = new charStream(buffer.Char, fastCSharp.unmanagedPool.TinyBuffers.Size >> 1))
                        {
                            genericFullName(type);
                            return NameStream.ToString();
                        }
                    }
                    finally { fastCSharp.unmanagedPool.TinyBuffers.Push(ref buffer); }
                }
                Type reflectedType = type.ReflectedType;
                if (reflectedType == null) return type.Namespace + "." + type.Name;
                else
                {
                    pointer buffer = fastCSharp.unmanagedPool.TinyBuffers.Get();
                    try
                    {
                        using (NameStream = new charStream(buffer.Char, fastCSharp.unmanagedPool.TinyBuffers.Size >> 1))
                        {
                            this.reflectedType(type, reflectedType);
                            return NameStream.ToString();
                        }
                    }
                    finally { fastCSharp.unmanagedPool.TinyBuffers.Push(ref buffer); }
                }
            }
            /// <summary>
            /// 获取类型名称
            /// </summary>
            /// <param name="type">类型</param>
            /// <returns>类型名称</returns>
            public unsafe string GetName(Type type)
            {
                if (type.IsArray)
                {
                    pointer buffer = fastCSharp.unmanagedPool.TinyBuffers.Get();
                    try
                    {
                        using (NameStream = new charStream(buffer.Char, fastCSharp.unmanagedPool.TinyBuffers.Size >> 1))
                        {
                            array(type, false);
                            return NameStream.ToString();
                        }
                    }
                    finally { fastCSharp.unmanagedPool.TinyBuffers.Push(ref buffer); }
                }
                if (type.IsGenericType)
                {
                    pointer buffer = fastCSharp.unmanagedPool.TinyBuffers.Get();
                    try
                    {
                        using (NameStream = new charStream(buffer.Char, fastCSharp.unmanagedPool.TinyBuffers.Size >> 1))
                        {
                            genericName(type);
                            return NameStream.ToString();
                        }
                    }
                    finally { fastCSharp.unmanagedPool.TinyBuffers.Push(ref buffer); }
                }
                return type.Name;
            }
            /// <summary>
            /// 任意类型处理
            /// </summary>
            /// <param name="type">类型</param>
            private void getFullName(Type type)
            {
                string value;
                if (!IsXml && typeNames.TryGetValue(type, out value)) NameStream.WriteNotNull(value);
                else if (type.IsGenericParameter) NameStream.SimpleWriteNotNull(type.Name);
                else if (type.IsArray) array(type, true);
                else if (type.IsGenericType) genericFullName(type);
                else
                {
                    Type reflectedType = type.ReflectedType;
                    if (reflectedType == null)
                    {
                        NameStream.WriteNotNull(type.Namespace);
                        NameStream.Write('.');
                        NameStream.SimpleWriteNotNull(type.Name);
                    }
                    else this.reflectedType(type, reflectedType);
                }
            }
            /// <summary>
            /// 任意类型处理
            /// </summary>
            /// <param name="type">类型</param>
            private void getNameNoArray(Type type)
            {
                string value;
                if (!IsXml && typeNames.TryGetValue(type, out value)) NameStream.WriteNotNull(value);
                else if (type.IsGenericParameter) NameStream.SimpleWriteNotNull(type.Name);
                else if (type.IsGenericType) genericName(type);
                else NameStream.SimpleWriteNotNull(type.Name);
            }
            /// <summary>
            /// 数组处理
            /// </summary>
            /// <param name="type">类型</param>
            /// <param name="isFullName">是否全称</param>
            private unsafe void array(Type type, bool isFullName)
            {
                pointer buffer = fastCSharp.unmanagedPool.TinyBuffers.Get();
                try
                {
                    int* currentRank = buffer.Int, endRank = (int*)(buffer.Byte + fastCSharp.unmanagedPool.TinyBuffers.Size);
                    do
                    {
                        if (currentRank == endRank) log.Error.Throw(log.exceptionType.IndexOutOfRange);
                        *currentRank++ = type.GetArrayRank();
                    }
                    while ((type = type.GetElementType()).IsArray);
                    if (isFullName) getFullName(type);
                    else getNameNoArray(type);
                    while (currentRank != buffer.Int)
                    {
                        NameStream.Write('[');
                        int rank = *--currentRank;
                        if (--rank != 0) number.ToString(rank, NameStream);
                        NameStream.Write(']');
                    }
                }
                finally { fastCSharp.unmanagedPool.TinyBuffers.Push(ref buffer); }
            }
            /// <summary>
            /// 泛型处理
            /// </summary>
            /// <param name="type">类型</param>
            private void genericFullName(Type type)
            {
                Type reflectedType = type.ReflectedType;
                if (reflectedType == null)
                {
                    if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        getFullName(type.GetGenericArguments()[0]);
                        NameStream.Write('?');
                        return;
                    }
                    string name = type.Name;
                    NameStream.WriteNotNull(type.Namespace);
                    NameStream.Write('.');
                    NameStream.Write(subString.Unsafe(name, 0, name.IndexOf(GenericSplit)));
                    genericParameter(type);
                    return;
                }
                subArray<Type> reflectedTypeList = default(subArray<Type>);
                do
                {
                    reflectedTypeList.Add(reflectedType);
                    reflectedType = reflectedType.ReflectedType;
                }
                while (reflectedType != null);
                Type[] reflectedTypeArray = reflectedTypeList.array;
                int reflectedTypeIndex = reflectedTypeList.length - 1;
                reflectedType = reflectedTypeArray[reflectedTypeIndex];
                NameStream.WriteNotNull(reflectedType.Namespace);
                Type[] parameterTypes = type.GetGenericArguments();
                int parameterIndex = 0;
                do
                {
                    NameStream.Write('.');
                    if (reflectedType.IsGenericType)
                    {
                        string name = reflectedType.Name;
                        int splitIndex = name.IndexOf(GenericSplit);
                        if (splitIndex != -1)
                        {
                            NameStream.Write(name, 0, splitIndex);
                            int parameterCount = reflectedType.GetGenericArguments().Length;
                            genericParameter(parameterTypes, parameterIndex, parameterCount);
                            parameterIndex = parameterCount;
                        }
                        else NameStream.WriteNotNull(name);
                    }
                    else NameStream.SimpleWriteNotNull(reflectedType.Name);
                    if (reflectedTypeIndex == 0)
                    {
                        reflectedType = type;
                        type = null;
                    }
                    else reflectedType = reflectedTypeArray[--reflectedTypeIndex];
                }
                while (reflectedType != null);
            }
            /// <summary>
            /// 泛型处理
            /// </summary>
            /// <param name="type">类型</param>
            private void genericName(Type type)
            {
                string name = type.Name;
                int splitIndex = name.IndexOf(GenericSplit);
                Type reflectedType = type.ReflectedType;
                if (reflectedType == null)
                {
                    NameStream.Write(name, 0, splitIndex);
                    genericParameter(type);
                    return;
                }
                if (splitIndex == -1)
                {
                    NameStream.WriteNotNull(name);
                    return;
                }
                Type[] parameterTypes = type.GetGenericArguments();
                int parameterIndex = 0;
                do
                {
                    if (reflectedType.IsGenericType)
                    {
                        int parameterCount = reflectedType.GetGenericArguments().Length;
                        if (parameterCount != parameterTypes.Length)
                        {
                            parameterIndex = parameterCount;
                            break;
                        }
                    }
                    reflectedType = reflectedType.ReflectedType;
                }
                while (reflectedType != null);
                NameStream.Write(name, 0, splitIndex);
                genericParameter(parameterTypes, parameterIndex, parameterTypes.Length);
            }
            /// <summary>
            /// 泛型参数处理
            /// </summary>
            /// <param name="type">类型</param>
            private void genericParameter(Type type)
            {
                NameStream.Write(IsXml ? '{' : '<');
                int index = 0;
                foreach (Type parameter in type.GetGenericArguments())
                {
                    if (index != 0) NameStream.Write(',');
                    getFullName(parameter);
                    ++index;
                }
                NameStream.Write(IsXml ? '}' : '>');
            }
            /// <summary>
            /// 泛型参数处理
            /// </summary>
            /// <param name="parameterTypes">参数类型集合</param>
            /// <param name="startIndex">起始位置</param>
            /// <param name="endIndex">结束位置</param>
            private void genericParameter(Type[] parameterTypes, int startIndex, int endIndex)
            {
                NameStream.Write(IsXml ? '{' : '<');
                for (getFullName(parameterTypes[startIndex]); ++startIndex != endIndex; getFullName(parameterTypes[startIndex])) NameStream.Write(',');
                NameStream.Write(IsXml ? '}' : '>');
            }
            /// <summary>
            /// 嵌套类型处理
            /// </summary>
            /// <param name="type">类型</param>
            /// <param name="reflectedType">上层类型</param>
            private void reflectedType(Type type, Type reflectedType)
            {
                subArray<Type> reflectedTypeList = default(subArray<Type>);
                do
                {
                    reflectedTypeList.Add(reflectedType);
                    reflectedType = reflectedType.ReflectedType;
                }
                while (reflectedType != null);
                Type[] reflectedTypeArray = reflectedTypeList.array;
                int reflectedTypeIndex = reflectedTypeList.length - 1;
                reflectedType = reflectedTypeArray[reflectedTypeIndex];
                NameStream.WriteNotNull(reflectedType.Namespace);
                do
                {
                    NameStream.Write('.');
                    NameStream.SimpleWriteNotNull(reflectedType.Name);
                    if (reflectedTypeIndex == 0)
                    {
                        reflectedType = type;
                        type = null;
                    }
                    else reflectedType = reflectedTypeArray[--reflectedTypeIndex];
                }
                while (reflectedType != null);
            }
        }
        /// <summary>
        /// 根据类型获取可用名称
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>类型名称</returns>
        public static string fullName(this Type type)
        {
            if (type == null) return null;
            string value;
            if (typeNames.TryGetValue(type, out value)) return value;
            if (type.IsGenericParameter) return type.Name;
            return new nameBuilder().GetFullName(type);
        }
        /// <summary>
        /// 根据类型获取可用名称
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>类型名称</returns>
        public static string name(this Type type)
        {
            if (type == null) return null;
            string value;
            if (typeNames.TryGetValue(type, out value)) return value;
            if (type.IsGenericParameter) return type.Name;
            return new nameBuilder().GetName(type);
        }
        /// <summary>
        /// 根据类型获取可用名称
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>类型名称</returns>
        public static string onlyName(this Type type)
        {
            string value;
            if (typeNames.TryGetValue(type, out value)) return value;
            value = type.Name;
            if (type.IsGenericTypeDefinition)
            {
                int index = value.IndexOf(GenericSplit);
                if (index != -1) value = value.Substring(0, index);
            }
            return value;
        }
        /// <summary>
        /// 判断类型是否可空类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>是否可空类型</returns>
        public static bool isNull(this Type type)
        {
            return type != null && (!type.IsValueType || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }
        /// <summary>
        /// 是否值类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>是否值类型</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static bool isStruct(this Type type)
        {
            return type != null && type.IsValueType && !type.IsEnum;
        }
        /// <summary>
        /// 获取可空类型的值类型
        /// </summary>
        /// <param name="type">可空类型</param>
        /// <returns>值类型,失败返回null</returns>
        public static Type nullableType(this Type type)
        {
            if (type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return type.GetGenericArguments()[0];
            }
            return null;
        }
        /// <summary>
        /// 值类型转换为可空类型
        /// </summary>
        /// <param name="type">值类型</param>
        /// <returns>可空类型,失败返回null</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static Type toNullableType(this Type type)
        {
            if (type != null && type.IsValueType)
            {
                return typeof(Nullable<>).MakeGenericType(type);
            }
            return null;
        }
        /// <summary>
        /// 获取泛型接口类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="interfaceType">泛型接口类型定义</param>
        /// <returns>泛型接口类型,失败返回null</returns>
        public static Type getGenericInterface(this Type type, Type interfaceType)
        {
            foreach (Type nextType in getGenericInterfaces(type, interfaceType)) return nextType;
            return null;
        }
        /// <summary>
        /// 获取泛型接口类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="interfaceType">泛型接口类型定义</param>
        /// <returns>泛型接口类型,失败返回null</returns>
        public static IEnumerable<Type> getGenericInterfaces(this Type type, Type interfaceType)
        {
            if (type != null && interfaceType != null && interfaceType.IsInterface)
            {
                if (type.IsInterface && type.IsGenericType && type.GetGenericTypeDefinition() == interfaceType) yield return type;
                foreach (Type nextType in type.GetInterfaces())
                {
                    if (nextType.IsGenericType && nextType.GetGenericTypeDefinition() == interfaceType) yield return nextType;
                }
            }
        }
        /// <summary>
        /// 根据成员属性获取自定义属性
        /// </summary>
        /// <typeparam name="attributeType">自定义属性类型</typeparam>
        /// <param name="type">类型</param>
        /// <param name="customType">自定义属性申明类型</param>
        /// <param name="isInheritAttribute">是否包含继承属性</param>
        /// <returns>自定义属性</returns>
        public static attributeType customAttribute<attributeType>
            (this Type type, out Type customType, bool isInheritAttribute = false)
            where attributeType : Attribute
        {
            while (type != null && type != typeof(object))
            {
                foreach (attributeType attribute in fastCSharp.code.typeAttribute.GetAttributes<attributeType>(type))
                {
                    if (isInheritAttribute || attribute.GetType() == typeof(attributeType))
                    {
                        customType = type;
                        return (attributeType)attribute;
                    }
                }
                type = type.BaseType;
            }
            customType = null;
            return null;
        }
        /// <summary>
        /// 获取TryParse函数
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>TryParse函数,失败返回null</returns>
        public static MethodInfo getTryParse(this Type type)
        {
            if (type != null)
            {
                MethodInfo tryParse = type.GetMethod("TryParse", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string), type.MakeByRefType() }, null);
                if (tryParse != null && tryParse.ReturnType == typeof(bool)) return tryParse;
            }
            return null;
        }
        /// <summary>
        /// 获取字段信息
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns>字段信息</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static FieldInfo getField(this Type type, string fieldName)
        {
            return type != null ? type.GetField(fieldName, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase) ?? type.GetField(fieldName, BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase) : null;
        }
        /// <summary>
        /// 获取属性信息
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns>属性信息</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static PropertyInfo getProperty(this Type type, string propertyName)
        {
            return type != null ? type.GetProperty(propertyName, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance) ?? type.GetProperty(propertyName, BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance) : null;
        }

        static type()
        {
            #region 初始化 类型名称集合
            typeNames = dictionary.CreateOnly<Type, string>();
            typeNames.Add(typeof(bool), "bool");
            typeNames.Add(typeof(byte), "byte");
            typeNames.Add(typeof(sbyte), "sbyte");
            typeNames.Add(typeof(short), "short");
            typeNames.Add(typeof(ushort), "ushort");
            typeNames.Add(typeof(int), "int");
            typeNames.Add(typeof(uint), "uint");
            typeNames.Add(typeof(long), "long");
            typeNames.Add(typeof(ulong), "ulong");
            typeNames.Add(typeof(float), "float");
            typeNames.Add(typeof(double), "double");
            typeNames.Add(typeof(decimal), "decimal");
            typeNames.Add(typeof(char), "char");
            typeNames.Add(typeof(string), "string");
            typeNames.Add(typeof(object), "object");
            typeNames.Add(typeof(void), "void");
            #endregion
        }
    }
}
