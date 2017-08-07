using System;
using System.Collections.Generic;
using System.Reflection;
using fastCSharp.threading;
using System.Collections.Specialized;
using System.Text;
using System.Data.Common;
using fastCSharp.sql.expression;
using fastCSharp.code;
using fastCSharp.reflection;
using System.Runtime.CompilerServices;
#if NOJIT
#else
using System.Reflection.Emit;
#endif

namespace fastCSharp.emit
{
    /// <summary>
    /// 公共类型
    /// </summary>
    public static partial class pub
    {
        /// <summary>
        /// LGD
        /// </summary>
        internal const int PuzzleValue = 0x10035113;
        /// <summary>
        /// 获取字段成员集合
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <typeparam name="memberAttribute"></typeparam>
        /// <param name="memberFilter"></param>
        /// <param name="isAllMember"></param>
        /// <returns>字段成员集合</returns>
        internal static subArray<FieldInfo> GetFields<valueType, memberAttribute>(memberFilters memberFilter, bool isAllMember)
            where memberAttribute : ignoreMember
        {
            fieldIndex[] fieldIndexs = fastCSharp.code.memberIndexGroup<valueType>.GetFields(memberFilter);
            subArray<FieldInfo> fields = new subArray<FieldInfo>(fieldIndexs.Length);
            foreach (fieldIndex field in fieldIndexs)
            {
                Type type = field.Member.FieldType;
                if (!type.IsPointer && (!type.IsArray || type.GetArrayRank() == 1) && !field.IsIgnore)
                {
                    memberAttribute attribute = field.GetAttribute<memberAttribute>(true, true);
                    if (isAllMember ? (attribute == null || attribute.IsSetup) : (attribute != null && attribute.IsSetup))
                    {
                        fields.Add(field.Member);
                    }
                }
            }
            return fields;
        }
        /// <summary>
        /// 获取属性成员集合
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <typeparam name="memberAttribute"></typeparam>
        /// <param name="memberFilter"></param>
        /// <param name="isAllMember"></param>
        /// <param name="isGet">是否必须可读</param>
        /// <param name="isSet">是否必须可写</param>
        /// <returns>属性成员集合</returns>
        internal static subArray<PropertyInfo> GetProperties<valueType, memberAttribute>(memberFilters memberFilter, bool isAllMember, bool isGet, bool isSet)
            where memberAttribute : ignoreMember
        {
            propertyIndex[] propertyIndexs = fastCSharp.code.memberIndexGroup<valueType>.GetProperties(memberFilter);
            subArray<PropertyInfo> properties = new subArray<PropertyInfo>(propertyIndexs.Length);
            foreach (propertyIndex property in propertyIndexs)
            {
                if ((!isGet || property.CanGet) && (!isSet || property.CanSet))
                {
                    Type type = property.Member.PropertyType;
                    if (!type.IsPointer && (!type.IsArray || type.GetArrayRank() == 1) && !property.IsIgnore)
                    {
                        memberAttribute attribute = property.GetAttribute<memberAttribute>(true, true);
                        if (isAllMember ? (attribute == null || attribute.IsSetup) : (attribute != null && attribute.IsSetup))
                        {
                            properties.Add(property.Member);
                        }
                    }
                }
            }
            return properties;
        }
        /// <summary>
        /// 获取字段成员集合
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="memberFilter"></param>
        /// <returns>字段成员集合</returns>
        internal static keyValue<FieldInfo, int>[] GetFieldIndexs<valueType>(memberFilters memberFilter)
        {
            return fastCSharp.code.memberIndexGroup<valueType>.GetFields(memberFilter)
                .getArray(value => new keyValue<FieldInfo, int>(value.Member, value.MemberIndex));
        }
#if NOJIT
        /// <summary>
        /// 函数调用
        /// </summary>
        public struct methodParameter1
        {
            /// <summary>
            /// 函数信息
            /// </summary>
            private MethodInfo method;
            /// <summary>
            /// 是否静态函数
            /// </summary>
            private bool isStatic;
            /// <summary>
            /// 函数调用
            /// </summary>
            /// <param name="method"></param>
            public methodParameter1(MethodInfo method)
            {
                this.method = method;
                isStatic = method.IsStatic;
            }
            /// <summary>
            /// 函数调用
            /// </summary>
            /// <param name="target"></param>
            /// <param name="parameters"></param>
            /// <returns></returns>
            public object Invoke(object target, object[] parameters)
            {
                if (isStatic) return method.Invoke(null, new object[] { target, parameters[0] });
                return method.Invoke(target, parameters);
            }
        }
#else
        /// <summary>
        /// 默认动态程序集
        /// </summary>
        private static readonly AssemblyName assemblyName = new AssemblyName("fastCSharpDynamicAssembly");
        /// <summary>
        /// 默认动态程序集
        /// </summary>
        private static readonly AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        /// <summary>
        /// 默认动态程序集模块
        /// </summary>
        public static readonly ModuleBuilder ModuleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);
        /// <summary>
        /// int引用参数类型
        /// </summary>
        internal static readonly Type RefIntType = typeof(int).MakeByRefType();

        /// <summary>
        /// 获取名称空间
        /// </summary>
        /// <param name="name"></param>
        /// <param name="seek">前缀字符长度</param>
        /// <param name="endSize">后缀字符长度</param>
        /// <returns></returns>
        public static unsafe char* GetNamePool(string name, int seek, int endSize)
        {
            return namePool.Get(name, seek, endSize);
        }
        /// <summary>
        /// 名称赋值数据信息集合
        /// </summary>
        private static readonly interlocked.dictionary<string, pointer> nameAssignmentPools = new interlocked.dictionary<string, pointer>();
        /// <summary>
        /// 获取名称赋值数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static unsafe char* GetNameAssignmentPool(string name)
        {
            pointer pointer;
            if (nameAssignmentPools.TryGetValue(name, out pointer)) return pointer.Char;
            char* value = namePool.Get(name, 0, 1);
            *(value + name.Length) = '=';
            nameAssignmentPools.Set(name, new pointer { Data = value });
            return value;
        }

        /// <summary>
        /// 创建构造函数委托
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parameterType">参数类型</param>
        /// <returns>构造函数委托</returns>
        public static Delegate CreateConstructor(Type type, Type parameterType)
        {
            DynamicMethod dynamicMethod = new DynamicMethod("constructor", type, new Type[] { parameterType }, type, true);
            ILGenerator generator = dynamicMethod.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Newobj, type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { parameterType }, null));
            generator.Emit(OpCodes.Ret);
            return dynamicMethod.CreateDelegate(typeof(Func<,>).MakeGenericType(parameterType, type));
        }

        /// <summary>
        /// 获取数值转换委托调用函数信息
        /// </summary>
        /// <param name="type">数值类型</param>
        /// <returns>数值转换委托调用函数信息</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static MethodInfo GetNumberToCharStreamMethod(Type type)
        {
            return numberToCharStream.GetToStringMethod(type);
        }
#endif
        /// <summary>
        /// 可空类型是否为空判断函数信息集合
        /// </summary>
        private static readonly interlocked.dictionary<Type, MethodInfo> nullableHasValues = new interlocked.dictionary<Type, MethodInfo>();
        /// <summary>
        /// 获取可空类型是否为空判断函数信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns>可空类型是否为空判断函数信息</returns>
        internal static MethodInfo GetNullableHasValue(Type type)
        {
            MethodInfo method;
            if (nullableHasValues.TryGetValue(type, out method)) return method;
            method = type.GetProperty("HasValue", BindingFlags.Instance | BindingFlags.Public).GetGetMethod();
            nullableHasValues.Set(type, method);
            return method;
        }

        /// <summary>
        /// 可空类型获取数据函数信息集合
        /// </summary>
        private static readonly interlocked.dictionary<Type, MethodInfo> nullableValues = new interlocked.dictionary<Type, MethodInfo>();
        /// <summary>
        /// 获取可空类型获取数据函数信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns>可空类型获取数据函数信息</returns>
        internal static MethodInfo GetNullableValue(Type type)
        {
            MethodInfo method;
            if (nullableValues.TryGetValue(type, out method)) return method;
            method = type.GetProperty("Value", BindingFlags.Instance | BindingFlags.Public).GetGetMethod();
            nullableValues.Set(type, method);
            return method;
        }

        /// <summary>
        /// SQL常量转换函数信息集合
        /// </summary>
        private static readonly interlocked.dictionary<Type, MethodInfo> sqlConverterMethods = new interlocked.dictionary<Type, MethodInfo>();
        /// <summary>
        /// 获取SQL常量转换函数信息
        /// </summary>
        /// <param name="type">数值类型</param>
        /// <returns>SQL常量转换函数信息</returns>
        internal static MethodInfo GetSqlConverterMethod(Type type)
        {
            MethodInfo method;
            if (sqlConverterMethods.TryGetValue(type, out method)) return method;
            method = typeof(constantConverter).GetMethod("convertConstant", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(charStream), type }, null)
                ?? constantConverter.ConvertConstantStringMethod.MakeGenericMethod(type);
            sqlConverterMethods.Set(type, method);
            return method;
        }

        /// <summary>
        /// 转换类型
        /// </summary>
        private struct castType : IEquatable<castType>
        {
            /// <summary>
            /// 原始类型
            /// </summary>
            public Type FromType;
            /// <summary>
            /// 目标类型
            /// </summary>
            public Type ToType;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public bool Equals(castType other)
            {
                return FromType == other.FromType && ToType == other.ToType;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return FromType.GetHashCode() ^ ToType.GetHashCode();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                return Equals((castType)obj);
            }
        }
        /// <summary>
        /// 类型转换函数集合
        /// </summary>
        private static readonly interlocked.dictionary<castType, MethodInfo> castMethods = new interlocked.dictionary<castType, MethodInfo>();
        /// <summary>
        /// 获取类型转换函数
        /// </summary>
        /// <param name="fromType"></param>
        /// <param name="toType"></param>
        /// <returns></returns>
        internal static MethodInfo GetCastMethod(Type fromType, Type toType)
        {
            if (fromType == toType) return null;
#if NOJIT
#else
            if (fromType == typeof(int))
            {
                if (toType == typeof(uint)) return null;
            }
            else if (fromType == typeof(long))
            {
                if (toType == typeof(ulong)) return null;
            }
            else if (fromType == typeof(byte))
            {
                if (toType == typeof(sbyte)) return null;
            }
            else if (fromType == typeof(short))
            {
                if (toType == typeof(ushort)) return null;
            }
#endif
            castType castType = new castType { FromType = fromType, ToType = toType };
            MethodInfo method;
            if (castMethods.TryGetValue(castType, out method)) return method;
#if NOJIT
#else
            if (!toType.IsPrimitive)
#endif
            {
                foreach (MethodInfo methodInfo in toType.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    if (methodInfo.ReturnType == toType && (methodInfo.Name == "op_Implicit" || methodInfo.Name == "op_Explicit") && methodInfo.GetParameters()[0].ParameterType == fromType)
                    {
                        method = methodInfo;
                        break;
                    }
                }
                //Type[] castParameterTypes = new Type[] { fromType };
                //method = toType.GetMethod("op_Implicit", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public, null, castParameterTypes, null)
                //    ?? toType.GetMethod("op_Explicit", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public, null, castParameterTypes, null);
            }
#if NOJIT
            if (method == null)
#else
            if (method == null && !fromType.IsPrimitive)
#endif
            {
                foreach (MethodInfo methodInfo in fromType.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    if (methodInfo.ReturnType == toType && (methodInfo.Name == "op_Implicit" || methodInfo.Name == "op_Explicit") && methodInfo.GetParameters()[0].ParameterType == fromType)
                    {
                        method = methodInfo;
                        break;
                    }
                }
            }
            castMethods.Set(castType, method);
            return method;
        }

        /// <summary>
        /// 枚举值解析
        /// </summary>
        /// <typeparam name="valueType">枚举类型</typeparam>
        /// <typeparam name="intType">枚举值数字类型</typeparam>
        public static class enumCast<valueType, intType>
        {
#if NOJIT
            /// <summary>
            /// 枚举转数字委托
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public static intType ToInt(valueType value)
            {
                return (intType)(object)value;
            }
            /// <summary>
            /// 枚举转数字委托
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public static valueType FromInt(intType value)
            {
                return (valueType)(object)value;
            }
#else
            /// <summary>
            /// 枚举转数字委托
            /// </summary>
            public static readonly Func<valueType, intType> ToInt;
            /// <summary>
            /// 数字转枚举委托
            /// </summary>
            public static readonly Func<intType, valueType> FromInt;

            static enumCast()
            {
                DynamicMethod dynamicMethod = new DynamicMethod("To" + typeof(intType).FullName, typeof(intType), new Type[] { typeof(valueType) }, typeof(valueType), true);
                ILGenerator generator = dynamicMethod.GetILGenerator();
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ret);
                ToInt = (Func<valueType, intType>)dynamicMethod.CreateDelegate(typeof(Func<valueType, intType>));

                dynamicMethod = new DynamicMethod("From" + typeof(intType).FullName, typeof(valueType), new Type[] { typeof(intType) }, typeof(valueType), true);
                generator = dynamicMethod.GetILGenerator();
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ret);
                FromInt = (Func<intType, valueType>)dynamicMethod.CreateDelegate(typeof(Func<intType, valueType>));
            }
#endif
        }
        /// <summary>
        /// 集合构造函数
        /// </summary>
        /// <typeparam name="dictionaryType">集合类型</typeparam>
        /// <typeparam name="keyType">枚举值类型</typeparam>
        /// <typeparam name="valueType">枚举值类型</typeparam>
        internal static class dictionaryConstructor<dictionaryType, keyType, valueType>
        {
#if NOJIT
            /// <summary>
            /// 构造函数
            /// </summary>
            private static readonly ConstructorInfo constructor = typeof(dictionaryType).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(IDictionary<,>).MakeGenericType(typeof(keyType), typeof(valueType)) }, null);
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static dictionaryType Constructor(IDictionary<keyType, valueType> value)
            {
                return (dictionaryType)constructor.Invoke(new object[] { value });
            }
#else
            /// <summary>
            /// 构造函数
            /// </summary>
            public static readonly Func<IDictionary<keyType, valueType>, dictionaryType> Constructor = (Func<IDictionary<keyType, valueType>, dictionaryType>)pub.CreateConstructor(typeof(dictionaryType), typeof(IDictionary<,>).MakeGenericType(typeof(keyType), typeof(valueType)));
#endif
        }
        /// <summary>
        /// 集合构造函数
        /// </summary>
        /// <typeparam name="valueType">集合类型</typeparam>
        /// <typeparam name="argumentType">枚举值类型</typeparam>
        internal static class listConstructor<valueType, argumentType>
        {
#if NOJIT
            /// <summary>
            /// 构造函数
            /// </summary>
            private static readonly ConstructorInfo constructor = typeof(valueType).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(IList<>).MakeGenericType(typeof(argumentType)) }, null);
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static valueType Constructor(IList<argumentType> value)
            {
                return (valueType)constructor.Invoke(new object[] { value });
            }
#else
            /// <summary>
            /// 构造函数
            /// </summary>
            public static readonly Func<IList<argumentType>, valueType> Constructor = (Func<IList<argumentType>, valueType>)pub.CreateConstructor(typeof(valueType), typeof(IList<>).MakeGenericType(typeof(argumentType)));
#endif
        }
        /// <summary>
        /// 集合构造函数
        /// </summary>
        /// <typeparam name="valueType">集合类型</typeparam>
        /// <typeparam name="argumentType">枚举值类型</typeparam>
        internal static class collectionConstructor<valueType, argumentType>
        {
#if NOJIT
            /// <summary>
            /// 构造函数
            /// </summary>
            private static readonly ConstructorInfo constructor = typeof(valueType).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(ICollection<>).MakeGenericType(typeof(argumentType)) }, null);
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static valueType Constructor(ICollection<argumentType> value)
            {
                return (valueType)constructor.Invoke(new object[] { value });
            }
#else
            /// <summary>
            /// 构造函数
            /// </summary>
            public static readonly Func<ICollection<argumentType>, valueType> Constructor = (Func<ICollection<argumentType>, valueType>)pub.CreateConstructor(typeof(valueType), typeof(ICollection<>).MakeGenericType(typeof(argumentType)));
#endif
        }
        /// <summary>
        /// 集合构造函数
        /// </summary>
        /// <typeparam name="valueType">集合类型</typeparam>
        /// <typeparam name="argumentType">枚举值类型</typeparam>
        public static class enumerableConstructor<valueType, argumentType>
        {
#if NOJIT
            /// <summary>
            /// 构造函数
            /// </summary>
            private static readonly ConstructorInfo constructor = typeof(valueType).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(IEnumerable<>).MakeGenericType(typeof(argumentType)) }, null);
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static valueType Constructor(IEnumerable<argumentType> value)
            {
                return (valueType)constructor.Invoke(new object[] { value });
            }
#else
            /// <summary>
            /// 构造函数
            /// </summary>
            public static readonly Func<IEnumerable<argumentType>, valueType> Constructor = (Func<IEnumerable<argumentType>, valueType>)pub.CreateConstructor(typeof(valueType), typeof(IEnumerable<>).MakeGenericType(typeof(argumentType)));
#endif
        }
        /// <summary>
        /// 集合构造函数
        /// </summary>
        /// <typeparam name="valueType">集合类型</typeparam>
        /// <typeparam name="argumentType">枚举值类型</typeparam>
        internal static class arrayConstructor<valueType, argumentType>
        {
#if NOJIT
            /// <summary>
            /// 构造函数
            /// </summary>
            private static readonly ConstructorInfo constructor = typeof(valueType).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(argumentType).MakeArrayType() }, null);
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static valueType Constructor(argumentType[] value)
            {
                return (valueType)constructor.Invoke(new object[] { value });
            }
#else
            /// <summary>
            /// 构造函数
            /// </summary>
            public static readonly Func<argumentType[], valueType> Constructor = (Func<argumentType[], valueType>)CreateConstructor(typeof(valueType), typeof(argumentType).MakeArrayType());
#endif
        }

#if NOJIT
#else
        /// <summary>
        /// 判断数据是否为空
        /// </summary>
        internal static readonly MethodInfo DataReaderIsDBNullMethod = typeof(DbDataReader).GetMethod("IsDBNull", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(int) }, null);
#endif
        /// <summary>
        /// 基本类型设置函数
        /// </summary>
        private static readonly Dictionary<Type, MethodInfo> dataReaderMethods;
        /// <summary>
        /// 获取基本类型设置函数
        /// </summary>
        /// <param name="type">基本类型</param>
        /// <returns>设置函数</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static MethodInfo GetDataReaderMethod(Type type)
        {
            MethodInfo method;
            return dataReaderMethods.TryGetValue(type, out method) ? method : null;
        }
#if NOJIT
        /// <summary>
        /// 获取基本类型设置函数信息
        /// </summary>
        internal static readonly MethodInfo GetDataReaderMethodInfo;

        /// <summary>
        /// 创建获取字段委托
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <typeparam name="fieldType"></typeparam>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static Func<valueType, fieldType> GetField<valueType, fieldType>(string fieldName)
        {
            FieldInfo field = typeof(valueType).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (field == null) log.Error.Throw(typeof(valueType).fullName() + " 未找到字段成员 " + fieldName, null, false);
            return new field { Field = field }.Get<valueType, fieldType>;
        }
        /// <summary>
        /// 创建获取字段委托
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <typeparam name="fieldType"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        public static Func<valueType, fieldType> UnsafeGetField<valueType, fieldType>(FieldInfo field)
        {
            return new field { Field = field }.Get<valueType, fieldType>;
        }
        /// <summary>
        /// 创建设置字段委托
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <typeparam name="fieldType"></typeparam>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static Action<valueType, fieldType> UnsafeSetField<valueType, fieldType>(string fieldName) where valueType : class
        {
            FieldInfo field = typeof(valueType).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (field == null) return null;
            return new field { Field = field }.Set<valueType, fieldType>;
        }
        /// <summary>
        /// 创建设置字段委托
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <typeparam name="fieldType"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        public static Action<valueType, fieldType> UnsafeSetField<valueType, fieldType>(FieldInfo field) where valueType : class
        {
            return new field { Field = field }.Set<valueType, fieldType>;
        }
        /// <summary>
        /// 字段
        /// </summary>
        private sealed class field
        {
            /// <summary>
            /// 字段信息
            /// </summary>
            public FieldInfo Field;
            /// <summary>
            /// 获取字段
            /// </summary>
            /// <typeparam name="valueType"></typeparam>
            /// <typeparam name="fieldType"></typeparam>
            /// <param name="value"></param>
            /// <returns></returns>
            public fieldType Get<valueType, fieldType>(valueType value)
            {
                return (fieldType)Field.GetValue(value);
            }
            /// <summary>
            /// 设置字段
            /// </summary>
            /// <typeparam name="valueType"></typeparam>
            /// <typeparam name="fieldType"></typeparam>
            /// <param name="value"></param>
            /// <param name="fieldValue"></param>
            public void Set<valueType, fieldType>(valueType value, fieldType fieldValue)
            {
                Field.SetValue(value, fieldValue);
            }
        }
        /// <summary>
        /// 创建设置属性委托
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <typeparam name="propertyType"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        public static Action<valueType, propertyType> SetProperty<valueType, propertyType>(PropertyInfo property) where valueType : class
        {
            if (!property.CanWrite || property.ReflectedType != typeof(valueType) || !typeof(propertyType).IsAssignableFrom(property.PropertyType)) log.Error.Throw(log.exceptionType.ErrorOperation);
            return new propertySetter { SetMethod = property.GetSetMethod(true) }.Set<valueType, propertyType>;
        }
        /// <summary>
        /// 属性
        /// </summary>
        private sealed class propertySetter
        {
            /// <summary>
            /// 属性
            /// </summary>
            public MethodInfo SetMethod;
            /// <summary>
            /// 设置字段
            /// </summary>
            /// <typeparam name="valueType"></typeparam>
            /// <typeparam name="propertyType"></typeparam>
            /// <param name="value"></param>
            /// <param name="propertyValue"></param>
            public void Set<valueType, propertyType>(valueType value, propertyType propertyValue)
            {
                SetMethod.Invoke(value, new object[] { propertyValue });
            }
        }
#else
        /// <summary>
        /// 创建获取字段委托
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <typeparam name="fieldType"></typeparam>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static Func<valueType, fieldType> GetField<valueType, fieldType>(string fieldName)
        {
            FieldInfo field = typeof(valueType).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (field == null) log.Error.Throw(typeof(valueType).fullName() + " 未找到字段成员 " + fieldName, null, false);
            return UnsafeGetField<valueType, fieldType>(field);
        }
        /// <summary>
        /// 创建获取字段委托
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <typeparam name="fieldType"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static Func<valueType, fieldType> GetField<valueType, fieldType>(FieldInfo field)
        {
            if (field.ReflectedType != typeof(valueType) || !typeof(fieldType).IsAssignableFrom(field.FieldType)) log.Error.Throw(log.exceptionType.ErrorOperation);
            return UnsafeGetField<valueType, fieldType>(field);
        }
        /// <summary>
        /// 创建获取字段委托
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <typeparam name="fieldType"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        public static Func<valueType, fieldType> UnsafeGetField<valueType, fieldType>(FieldInfo field)
        {
            DynamicMethod dynamicMethod = new DynamicMethod("get_" + field.Name, typeof(fieldType), new Type[] { typeof(valueType) }, typeof(valueType), true);
            ILGenerator generator = dynamicMethod.GetILGenerator();
            if (typeof(valueType).IsValueType) generator.Emit(OpCodes.Ldarga_S, 0);
            else generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, field);
            generator.Emit(OpCodes.Ret);
            return (Func<valueType, fieldType>)dynamicMethod.CreateDelegate(typeof(Func<valueType, fieldType>));
        }
        /// <summary>
        /// 获取字段委托
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <typeparam name="fieldType"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public delegate fieldType getField<valueType, fieldType>(ref valueType value);
        /// <summary>
        /// 创建获取字段委托
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <typeparam name="fieldType"></typeparam>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static getField<valueType, fieldType> GetFieldStruct<valueType, fieldType>(string fieldName) where valueType : struct
        {
            DynamicMethod dynamicMethod = new DynamicMethod("getRef_" + fieldName, typeof(fieldType), new Type[] { typeof(valueType).MakeByRefType() }, typeof(valueType), true);
            ILGenerator generator = dynamicMethod.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, typeof(valueType).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));
            generator.Emit(OpCodes.Ret);
            return (getField<valueType, fieldType>)dynamicMethod.CreateDelegate(typeof(getField<valueType, fieldType>));
        }
        /// <summary>
        /// 创建设置字段委托
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="dynamicMethod"></param>
        /// <param name="field"></param>
        private static void getSetField<valueType>(DynamicMethod dynamicMethod, FieldInfo field)
        {
            ILGenerator generator = dynamicMethod.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Stfld, field);
            generator.Emit(OpCodes.Ret);
        }
        /// <summary>
        /// 创建设置字段委托
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <typeparam name="fieldType"></typeparam>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static Action<valueType, fieldType> UnsafeSetField<valueType, fieldType>(string fieldName) where valueType : class
        {
            FieldInfo field = typeof(valueType).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (field == null) return null;
            DynamicMethod dynamicMethod = new DynamicMethod("set_" + fieldName, null, new Type[] { typeof(valueType), typeof(fieldType) }, typeof(valueType), true);
            getSetField<valueType>(dynamicMethod, field);
            return (Action<valueType, fieldType>)dynamicMethod.CreateDelegate(typeof(Action<valueType, fieldType>));
        }
        /// <summary>
        /// 创建设置字段委托
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <typeparam name="fieldType"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        public static Action<valueType, fieldType> UnsafeSetField<valueType, fieldType>(FieldInfo field) where valueType : class
        {
            //if (field.ReflectedType != typeof(valueType) || !typeof(fieldType).IsAssignableFrom(field.FieldType)) log.Error.Throw(log.exceptionType.ErrorOperation);
            DynamicMethod dynamicMethod = new DynamicMethod("set_" + field.Name, null, new Type[] { typeof(valueType), typeof(fieldType) }, typeof(valueType), true);
            getSetField<valueType>(dynamicMethod, field);
            return (Action<valueType, fieldType>)dynamicMethod.CreateDelegate(typeof(Action<valueType, fieldType>));
        }
        /// <summary>
        /// 设置字段值委托
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <typeparam name="fieldType"></typeparam>
        /// <param name="value"></param>
        /// <param name="fieldValue"></param>
        public delegate void setField<valueType, fieldType>(ref valueType value, fieldType fieldValue);
        /// <summary>
        /// 创建设置字段委托
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <typeparam name="fieldType"></typeparam>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static setField<valueType, fieldType> SetFieldStruct<valueType, fieldType>(string fieldName) where valueType : struct
        {
            DynamicMethod dynamicMethod = new DynamicMethod("set_" + fieldName, null, new Type[] { typeof(valueType).MakeByRefType(), typeof(fieldType) }, typeof(valueType), true);
            getSetField<valueType>(dynamicMethod, typeof(valueType).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));
            return (setField<valueType, fieldType>)dynamicMethod.CreateDelegate(typeof(setField<valueType, fieldType>));
        }

        /// <summary>
        /// 获取静态属性值
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="assembly"></param>
        /// <param name="typeName"></param>
        /// <param name="name"></param>
        /// <param name="nonPublic"></param>
        /// <returns></returns>
        public static Func<valueType> GetStaticProperty<valueType>(Assembly assembly, string typeName, string name, bool nonPublic)
        {
            Type type = assembly.GetType(typeName);
            DynamicMethod dynamicMethod = new DynamicMethod("get_" + name, typeof(valueType), nullValue<Type>.Array, type, true);
            ILGenerator generator = dynamicMethod.GetILGenerator();
            generator.Emit(OpCodes.Call, type.GetProperty(name, BindingFlags.Static | (nonPublic ? BindingFlags.NonPublic : BindingFlags.Public)).GetGetMethod(nonPublic));
            generator.Emit(OpCodes.Ret);
            return (Func<valueType>)dynamicMethod.CreateDelegate(typeof(Func<valueType>));
        }
        /// <summary>
        /// 获取静态属性值
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="assembly"></param>
        /// <param name="typeName"></param>
        /// <param name="name"></param>
        /// <param name="nonPublic"></param>
        /// <returns></returns>
        public static Func<object, valueType> GetProperty<valueType>(Assembly assembly, string typeName, string name, bool nonPublic)
        {
            Type type = assembly.GetType(typeName);
            DynamicMethod dynamicMethod = new DynamicMethod("get_" + name, typeof(valueType), new Type[] { typeof(object) }, type, true);
            ILGenerator generator = dynamicMethod.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            MethodInfo method = type.GetProperty(name, BindingFlags.Instance | (nonPublic ? BindingFlags.NonPublic : BindingFlags.Public)).GetGetMethod(nonPublic);
            generator.Emit(method.IsFinal || !method.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, method);
            generator.Emit(OpCodes.Ret);
            return (Func<object, valueType>)dynamicMethod.CreateDelegate(typeof(Func<object, valueType>));
        }
        /// <summary>
        /// 创建设置属性委托
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <typeparam name="propertyType"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        public static Action<valueType, propertyType> SetProperty<valueType, propertyType>(PropertyInfo property) where valueType : class
        {
            if (!property.CanWrite || property.ReflectedType != typeof(valueType) || !typeof(propertyType).IsAssignableFrom(property.PropertyType)) log.Error.Throw(log.exceptionType.ErrorOperation);
            MethodInfo method = property.GetSetMethod(true);
            DynamicMethod dynamicMethod = new DynamicMethod("set_" + property.Name, null, new Type[] { typeof(valueType), typeof(propertyType) }, typeof(valueType), true);
            ILGenerator generator = dynamicMethod.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(method.IsFinal || !method.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, method);
            generator.Emit(OpCodes.Ret); 
            return (Action<valueType, propertyType>)dynamicMethod.CreateDelegate(typeof(Action<valueType, propertyType>));
        }

        /// <summary>
        /// 创建函数委托
        /// </summary>
        /// <typeparam name="valueType1"></typeparam>
        /// <typeparam name="valueType2"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        public static Action<valueType1, valueType2> GetAction<valueType1, valueType2>(MethodInfo method)
        {
            DynamicMethod dynamicMethod = new DynamicMethod(method.Name, null, new Type[] { typeof(valueType1), typeof(valueType2) }, method.DeclaringType, true);
            ILGenerator generator = dynamicMethod.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(method.IsFinal || !method.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, method);
            generator.Emit(OpCodes.Ret);
            return (Action<valueType1, valueType2>)dynamicMethod.CreateDelegate(typeof(Action<valueType1, valueType2>));
        }
        /// <summary>
        /// 创建函数委托
        /// </summary>
        /// <typeparam name="valueType1"></typeparam>
        /// <typeparam name="valueType2"></typeparam>
        /// <typeparam name="returnType"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        public static Func<valueType1, valueType2, returnType> GetStaticFunc<valueType1, valueType2, returnType>(MethodInfo method)
        {
            DynamicMethod dynamicMethod = new DynamicMethod(method.Name, typeof(returnType), new Type[] { typeof(valueType1), typeof(valueType2) }, method.DeclaringType, true);
            ILGenerator generator = dynamicMethod.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(method.IsFinal || !method.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, method);
            generator.Emit(OpCodes.Ret);
            return (Func<valueType1, valueType2, returnType>)dynamicMethod.CreateDelegate(typeof(Func<valueType1, valueType2, returnType>));
        }

        /// <summary>
        /// 可空类型构造函数
        /// </summary>
        internal static readonly Dictionary<Type, ConstructorInfo> NullableConstructors;
#endif
        static pub()
        {
            dataReaderMethods = dictionary.CreateOnly<Type, MethodInfo>();
#if NOJIT
            GetDataReaderMethodInfo = typeof(pub).GetMethod("GetDataReaderMethod", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(Type) }, null);
            dataReaderMethods.Add(typeof(bool), GetDataReaderMethodInfo);
            dataReaderMethods.Add(typeof(byte), GetDataReaderMethodInfo);
            dataReaderMethods.Add(typeof(char), GetDataReaderMethodInfo);
            dataReaderMethods.Add(typeof(DateTime), GetDataReaderMethodInfo);
            dataReaderMethods.Add(typeof(decimal), GetDataReaderMethodInfo);
            dataReaderMethods.Add(typeof(double), GetDataReaderMethodInfo);
            dataReaderMethods.Add(typeof(float), GetDataReaderMethodInfo);
            dataReaderMethods.Add(typeof(Guid), GetDataReaderMethodInfo);
            dataReaderMethods.Add(typeof(short), GetDataReaderMethodInfo);
            dataReaderMethods.Add(typeof(int), GetDataReaderMethodInfo);
            dataReaderMethods.Add(typeof(long), GetDataReaderMethodInfo);
            dataReaderMethods.Add(typeof(string), GetDataReaderMethodInfo);

#else
            Type[] intType = new Type[] { typeof(int) };
            dataReaderMethods.Add(typeof(bool), typeof(DbDataReader).GetMethod("GetBoolean", BindingFlags.Public | BindingFlags.Instance, null, intType, null));
            dataReaderMethods.Add(typeof(byte), typeof(DbDataReader).GetMethod("GetByte", BindingFlags.Public | BindingFlags.Instance, null, intType, null));
            dataReaderMethods.Add(typeof(char), typeof(DbDataReader).GetMethod("GetChar", BindingFlags.Public | BindingFlags.Instance, null, intType, null));
            dataReaderMethods.Add(typeof(DateTime), typeof(DbDataReader).GetMethod("GetDateTime", BindingFlags.Public | BindingFlags.Instance, null, intType, null));
            dataReaderMethods.Add(typeof(decimal), typeof(DbDataReader).GetMethod("GetDecimal", BindingFlags.Public | BindingFlags.Instance, null, intType, null));
            dataReaderMethods.Add(typeof(double), typeof(DbDataReader).GetMethod("GetDouble", BindingFlags.Public | BindingFlags.Instance, null, intType, null));
            dataReaderMethods.Add(typeof(float), typeof(DbDataReader).GetMethod("GetFloat", BindingFlags.Public | BindingFlags.Instance, null, intType, null));
            dataReaderMethods.Add(typeof(Guid), typeof(DbDataReader).GetMethod("GetGuid", BindingFlags.Public | BindingFlags.Instance, null, intType, null));
            dataReaderMethods.Add(typeof(short), typeof(DbDataReader).GetMethod("GetInt16", BindingFlags.Public | BindingFlags.Instance, null, intType, null));
            dataReaderMethods.Add(typeof(int), typeof(DbDataReader).GetMethod("GetInt32", BindingFlags.Public | BindingFlags.Instance, null, intType, null));
            dataReaderMethods.Add(typeof(long), typeof(DbDataReader).GetMethod("GetInt64", BindingFlags.Public | BindingFlags.Instance, null, intType, null));
            dataReaderMethods.Add(typeof(string), typeof(DbDataReader).GetMethod("GetString", BindingFlags.Public | BindingFlags.Instance, null, intType, null));
            
            NullableConstructors = dictionary.CreateOnly<Type, ConstructorInfo>();
            NullableConstructors.Add(typeof(bool), typeof(bool?).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(bool) }, null));
            NullableConstructors.Add(typeof(byte), typeof(byte?).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(byte) }, null));
            NullableConstructors.Add(typeof(char), typeof(char?).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(char) }, null));
            NullableConstructors.Add(typeof(DateTime), typeof(DateTime?).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(DateTime) }, null));
            NullableConstructors.Add(typeof(decimal), typeof(decimal?).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(decimal) }, null));
            NullableConstructors.Add(typeof(double), typeof(double?).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(double) }, null));
            NullableConstructors.Add(typeof(float), typeof(float?).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(float) }, null));
            NullableConstructors.Add(typeof(Guid), typeof(Guid?).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(Guid) }, null));
            NullableConstructors.Add(typeof(short), typeof(short?).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(short) }, null));
            NullableConstructors.Add(typeof(int), typeof(int?).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(int) }, null));
            NullableConstructors.Add(typeof(long), typeof(long?).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(long) }, null));
#endif
        }
    }
}
