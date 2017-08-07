using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace fastCSharp.code
{
    /// <summary>
    /// 参数信息
    /// </summary>
    public sealed partial class parameterInfo
    {
        /// <summary>
        /// 定义方法
        /// </summary>
        private MethodInfo method;
        /// <summary>
        /// 参数信息
        /// </summary>
        public ParameterInfo Parameter { get; private set; }
        /// <summary>
        /// 参数索引位置
        /// </summary>
        public int ParameterIndex { get; private set; }
        /// <summary>
        /// 参数类型
        /// </summary>
        public memberType ParameterType { get; private set; }
        /// <summary>
        /// 函数泛型参数类型
        /// </summary>
        public memberType GenericParameterType { get; private set; }
        /// <summary>
        /// 是否函数泛型参数
        /// </summary>
        public bool IsGenericParameter
        {
            get { return ParameterType.Type != GenericParameterType.Type; }
        }
        /// <summary>
        /// XML文档注释
        /// </summary>
        private string xmlDocument;
        /// <summary>
        /// XML文档注释
        /// </summary>
        public string XmlDocument
        {
            get
            {
                if (xmlDocument == null)
                {
                    xmlDocument = Parameter == null ? string.Empty : code.xmlDocument.Get(method, Parameter);
                }
                return xmlDocument.Length == 0 ? null : xmlDocument;
            }
        }
        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParameterName;
        /// <summary>
        /// 参数连接名称，最后一个参数不带逗号
        /// </summary>
        public string ParameterJoinName
        {
            get
            {
                return ParameterName + ParameterJoin;
            }
        }
        /// <summary>
        /// 带引用修饰的参数连接名称，最后一个参数不带逗号
        /// </summary>
        public string ParameterJoinRefName
        {
            get
            {
                return getRefName(ParameterJoinName);
            }
        }
        /// <summary>
        /// 带引用修饰的参数名称
        /// </summary>
        public string ParameterTypeRefName
        {
            get
            {
                return getRefName(ParameterType.FullName);
            }
        }
        /// <summary>
        /// 带引用修饰的参数名称
        /// </summary>
        public string ParameterRefName
        {
            get
            {
                return getRefName(ParameterName);
            }
        }
        /// <summary>
        /// 参数连接逗号，最后一个参数为null
        /// </summary>
        public string ParameterJoin { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string Ref
        {
            get { return "ref "; }
        }
        /// <summary>
        /// 是否引用参数
        /// </summary>
        public bool IsRef;
        /// <summary>
        /// 是否输出参数
        /// </summary>
        public bool IsOut { get; private set; }
        /// <summary>
        /// 是否输出参数
        /// </summary>
        public bool IsRefOrOut
        {
            get { return IsRef || IsOut; }
        }
        /// <summary>
        /// 参数引用前缀
        /// </summary>
        public string ParameterRef
        {
            get
            {
                return getRefName(null);
            }
        }
        /// <summary>
        /// 参数信息
        /// </summary>
        /// <param name="method">函数信息</param>
        /// <param name="genericParameters">方法泛型参数集合</param>
        /// <param name="parameter">参数信息</param>
        /// <param name="index">参数索引位置</param>
        /// <param name="isLast">是否最后一个参数</param>
        private parameterInfo(MethodInfo method, Type[] genericParameters, ParameterInfo parameter, int index, bool isLast)
        {
            this.method = method;
            Parameter = parameter;
            ParameterIndex = index;
            Type parameterType = parameter.ParameterType;
            if (parameterType.IsByRef)
            {
                if (parameter.IsOut) IsOut = true;
                else IsRef = true;
                ParameterType = parameterType.GetElementType();
            }
            else ParameterType = parameterType;
            GenericParameterType = ParameterType.Type.IsGenericParameter && Array.IndexOf(genericParameters, ParameterType) != -1 ? (memberType)typeof(object) : ParameterType;
            ParameterName = Parameter.Name;
            ParameterJoin = isLast ? null : ", ";
        }
        /// <summary>
        /// 参数信息
        /// </summary>
        /// <param name="parameter"></param>
        private parameterInfo(parameterInfo parameter)
        {
            this.method = parameter.method;
            Parameter = parameter.Parameter;
            ParameterIndex = parameter.ParameterIndex;
            ParameterType = parameter.ParameterType;
            GenericParameterType = parameter.GenericParameterType;
            IsOut = parameter.IsOut;
            IsRef = parameter.IsRef;
            ParameterName = parameter.ParameterName;
            ParameterJoin = null;
        }
        /// <summary>
        /// 参数信息
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="type">参数类型</param>
        public parameterInfo(string name, Type type)
        {
            ParameterName = name;
            ParameterType = GenericParameterType = type;
        }
        /// <summary>
        /// 获取带引用修饰的名称
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>带引用修饰的名称</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private string getRefName(string name)
        {
            if (IsOut) return "out " + name;
            if (IsRef) return Ref + name;
            return name;
        }
        /// <summary>
        /// 获取方法参数信息集合
        /// </summary>
        /// <param name="method">方法信息</param>
        /// <param name="genericParameters">方法泛型参数集合</param>
        /// <returns>参数信息集合</returns>
        internal static parameterInfo[] Get(MethodInfo method, Type[] genericParameters)
        {
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.length() != 0)
            {
                int index = 0;
                return parameters.getArray(value => new parameterInfo(method, genericParameters, value, index, ++index == parameters.Length));
            }
            return nullValue<parameterInfo>.Array;
        }
        /// <summary>
        /// 获取方法参数信息集合
        /// </summary>
        /// <param name="parameters">参数信息集合</param>
        /// <returns>参数信息集合</returns>
        public static parameterInfo[] Get(parameterInfo[] parameters)
        {
            if (parameters.length() != 0)
            {
                parameterInfo parameter = parameters[parameters.Length - 1];
                parameters[parameters.Length - 1] = new parameterInfo(parameter);
            }
            return parameters;
        }
    }
}
