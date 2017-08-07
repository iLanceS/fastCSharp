using System;
using System.Collections.Generic;
using System.Reflection;
using fastCSharp.reflection;
using System.Runtime.CompilerServices;

namespace fastCSharp.code
{
    /// <summary>
    /// 成员方法
    /// </summary>
    public sealed class methodInfo : memberInfo
    {
        /// <summary>
        /// 成员方法信息
        /// </summary>
        public MethodInfo Method { get; private set; }
        /// <summary>
        /// 自定义方法相关成员信息
        /// </summary>
        private MemberInfo customMember;
        /// <summary>
        /// 自定义方法是否取值，否则为设置值
        /// </summary>
        public bool IsGetMember { get; private set; }
        /// <summary>
        /// 自定义方法属性输入参数
        /// </summary>
        public parameterInfo[] PropertyParameters { get; private set; }
        /// <summary>
        /// 自定义方法属性返回值参数
        /// </summary>
        public parameterInfo PropertyParameter { get; private set; }
        /// <summary>
        /// 方法名称
        /// </summary>
        public string MethodName
        {
            get { return Method == null ? (IsGetMember ? "get_" : "set_") + customMember.Name : Method.Name; }
        }
        /// <summary>
        /// 方法泛型名称
        /// </summary>
        public string MethodGenericName
        {
            get
            {
                return MethodName + GenericParameterName;
            }
        }
        /// <summary>
        /// 方法泛型名称
        /// </summary>
        public string StaticMethodGenericName
        {
            get { return MethodGenericName; }
        }
        /// <summary>
        /// 方法全称标识
        /// </summary>
        public string MethodKeyFullName
        {
            get
            {
                return (customMember ?? Method).DeclaringType.fullName() + MethodKeyName;
            }
        }
        /// <summary>
        /// 方法标识
        /// </summary>
        public string MethodKeyName
        {
            get
            {
                return "(" + Parameters.joinString(',', value => value.ParameterRef + value.ParameterType.FullName) + ")" + GenericParameterName + MethodName;
            }
        }
        /// <summary>
        /// 返回值类型
        /// </summary>
        public memberType ReturnType { get; private set; }
        /// <summary>
        /// XML文档注释
        /// </summary>
        public override string XmlDocument
        {
            get
            {
                if (xmlDocument == null)
                {
                    if (customMember == null) xmlDocument = Method == null ? string.Empty : code.xmlDocument.Get(Method);
                    else
                    {
                        PropertyInfo property = customMember as PropertyInfo;
                        xmlDocument = property == null ? code.xmlDocument.Get((FieldInfo)customMember) : code.xmlDocument.Get(property);
                    }
                }
                return xmlDocument.Length == 0 ? null : xmlDocument;
            }
        }
        /// <summary>
        /// 返回值XML文档注释
        /// </summary>
        private string returnXmlDocument;
        /// <summary>
        /// 返回值XML文档注释
        /// </summary>
        public string ReturnXmlDocument
        {
            get
            {
                if (returnXmlDocument == null)
                {
                    if (customMember == null) returnXmlDocument = Method == null ? string.Empty : code.xmlDocument.GetReturn(Method);
                    else returnXmlDocument = XmlDocument ?? string.Empty;
                }
                return returnXmlDocument.Length == 0 ? null : returnXmlDocument;
            }
        }
        /// <summary>
        /// 是否有返回值
        /// </summary>
        public bool IsReturn
        {
            get
            {
                return ReturnType.Type != typeof(void);
            }
        }
        /// <summary>
        /// 参数集合
        /// </summary>
        public parameterInfo[] Parameters { get; private set; }
        /// <summary>
        /// 泛型参数类型集合
        /// </summary>
        public memberType[] GenericParameters { get; private set; }
        /// <summary>
        /// 泛型参数拼写
        /// </summary>
        private string genericParameterName;
        /// <summary>
        /// 泛型参数拼写
        /// </summary>
        public string GenericParameterName
        {
            get
            {
                if (genericParameterName == null)
                {
                    memberType[] genericParameters = GenericParameters;
                    genericParameterName = genericParameters.Length == 0 ? string.Empty : ("<" + genericParameters.joinString(',', value => value.FullName) + ">");
                }
                return genericParameterName;
            }
        }
        /// <summary>
        /// 参数集合
        /// </summary>
        public parameterInfo[] OutputParameters { get; private set; }
        /// <summary>
        /// 成员方法
        /// </summary>
        /// <param name="method">成员方法信息</param>
        /// <param name="filter">选择类型</param>
        internal methodInfo(MethodInfo method, memberFilters filter)
            : base(method, filter)
        {
            Method = method;
            ReturnType = method.ReturnType;
            Type[] genericParameters = method.GetGenericArguments();
            Parameters = parameterInfo.Get(method, genericParameters);
            OutputParameters = Parameters.getFindArray(value => value.Parameter.IsOut || value.Parameter.ParameterType.IsByRef);
            GenericParameters = genericParameters.getArray(value => (memberType)value);
        }
        /// <summary>
        /// 成员方法
        /// </summary>
        /// <param name="property">属性信息</param>
        /// <param name="isGet">是否取值</param>
        public methodInfo(PropertyInfo property, bool isGet)
            : this(isGet ? property.GetGetMethod(true) : property.GetSetMethod(true), memberFilters.Instance)
        {
            MemberName = property.Name;
            customMember = property;
            IsGetMember = isGet;
            PropertyParameter = isGet ? null : Parameters[Parameters.Length - 1];
            PropertyParameters = isGet ? Parameters : parameterInfo.Get(Parameters.getSub(0, Parameters.Length - 1));
        }
        /// <summary>
        /// 成员方法
        /// </summary>
        /// <param name="field">字段信息</param>
        /// <param name="isGet">是否取值</param>
        public methodInfo(FieldInfo field, bool isGet)
            : base(field.FieldType, field.Name)
        {
            customMember = field;
            if (IsGetMember = isGet)
            {
                ReturnType = field.FieldType;
                Parameters = nullValue<parameterInfo>.Array;
            }
            else
            {
                ReturnType = typeof(void);
                Parameters = new parameterInfo[] { new parameterInfo("value", field.FieldType) };
            }
            OutputParameters = PropertyParameters = nullValue<parameterInfo>.Array;
            GenericParameters = nullValue<memberType>.Array;
        }
        /// <summary>
        /// 类型成员方法缓存
        /// </summary>
        private static readonly Dictionary<Type, methodInfo[]> methodCache = dictionary.CreateOnly<Type, methodInfo[]>();
        /// <summary>
        /// 获取类型的成员方法集合
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>成员方法集合</returns>
        private static methodInfo[] getMethods(Type type)
        {
            methodInfo[] methods;
            if (!methodCache.TryGetValue(type, out methods))
            {
                int index = 0;
                methodCache[type] = methods = array.concat(
                    type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).getArray(value => new methodInfo(value, memberFilters.PublicStatic)),
                    type.GetMethods(BindingFlags.Public | BindingFlags.Instance).getArray(value => new methodInfo(value, memberFilters.PublicInstance)),
                    type.GetMethods(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy).getArray(value => new methodInfo(value, memberFilters.NonPublicStatic)),
                    type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).getArray(value => new methodInfo(value, memberFilters.NonPublicInstance)))
                    .each(value => value.MemberIndex = index++);
            }
            return methods;
        }
        /// <summary>
        /// 获取匹配成员方法集合
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="filter">选择类型</param>
        /// <param name="isFilter">是否完全匹配选择类型</param>
        /// <returns>匹配的成员方法集合</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray<methodInfo> GetMethods(Type type, memberFilters filter, bool isFilter)
        {
            return getMethods(type).getFind(value => isFilter ? (value.Filter & filter) == filter : ((value.Filter & filter) != 0));
        }
        /// <summary>
        /// 获取匹配成员方法集合
        /// </summary>
        /// <typeparam name="attributeType">自定义属性类型</typeparam>
        /// <param name="type">类型</param>
        /// <param name="methods">成员方法集合</param>
        /// <param name="isAttribute">是否匹配自定义属性类型</param>
        /// <param name="isBaseType">指定是否搜索该成员的继承链以查找这些特性，参考System.Reflection.MemberInfo.GetCustomAttributes(bool inherit)。</param>
        /// <param name="isInheritAttribute">自定义属性类型是否可继承</param>
        /// <returns>匹配成员方法集合</returns>
        private static methodInfo[] getMethods<attributeType>
            (Type type, subArray<methodInfo> methods, bool isAttribute, bool isBaseType, bool isInheritAttribute)
            where attributeType : ignoreMember
        {
            if (isAttribute)
            {
                return methods.ToList().getFindArray(value => value.IsAttribute<attributeType>(isBaseType, isInheritAttribute));
            }
            else
            {
                return methods.ToList().getFindArray(value => value.Method.DeclaringType == type && !value.IsIgnoreAttribute<attributeType>(isBaseType, isInheritAttribute));
            }
        }
        /// <summary>
        /// 获取匹配成员方法集合
        /// </summary>
        /// <typeparam name="attributeType">自定义属性类型</typeparam>
        /// <param name="type">类型</param>
        /// <param name="filter">选择类型</param>
        /// <param name="isFilter">是否完全匹配选择类型</param>
        /// <param name="isAttribute">是否匹配自定义属性类型</param>
        /// <param name="isBaseType">指定是否搜索该成员的继承链以查找这些特性，参考System.Reflection.MemberInfo.GetCustomAttributes(bool inherit)。</param>
        /// <param name="isInheritAttribute">自定义属性类型是否可继承</param>
        /// <returns>匹配成员方法集合</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static methodInfo[] GetMethods<attributeType>(Type type, memberFilters filter, bool isFilter, bool isAttribute, bool isBaseType, bool isInheritAttribute)
            where attributeType : ignoreMember
        {
            return getMethods<attributeType>(type, GetMethods(type, filter, isFilter), isAttribute, isBaseType, isInheritAttribute);
        }
    }
}
