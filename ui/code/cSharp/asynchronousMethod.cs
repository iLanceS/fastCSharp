using System;
using fastCSharp.threading;
using fastCSharp.reflection;

namespace fastCSharp.code
{
    /// <summary>
    /// 方法信息
    /// </summary>
    internal abstract class asynchronousMethod
    {
        /// <summary>
        /// 输入参数名称
        /// </summary>
        internal const string InputParameterName = "_Input_";
        /// <summary>
        /// 方法信息
        /// </summary>
        public code.methodInfo Method;
        /// <summary>
        /// 属性或者字段信息
        /// </summary>
        public memberIndex MemberIndex;
        /// <summary>
        /// 成员名称
        /// </summary>
        internal string MemberFullName
        {
            get
            {
                if (MemberIndex == null) return Method.Method.fullName();
                return MemberIndex.Member.DeclaringType.fullName() + "." + Method.MethodName;
            }
        }
        /// <summary>
        /// 属性或者字段名称
        /// </summary>
        public string PropertyName
        {
            get { return MemberIndex.Member.Name; }
        }
        /// <summary>
        /// 属性或者字段名称
        /// </summary>
        public string StaticPropertyName
        {
            get { return PropertyName; }
        }
        /// <summary>
        /// 方法参数
        /// </summary>
        internal parameterInfo[] methodParameters;
        /// <summary>
        /// 方法参数
        /// </summary>
        public parameterInfo[] MethodParameters
        {
            get
            {
                checkAsynchronousReturn();
                return methodParameters;
            }
        }
        /// <summary>
        /// 第一个方法参数
        /// </summary>
        public parameterInfo FristParameter
        {
            get { return MethodParameters[0]; }
        }
        /// <summary>
        /// 返回值参数名称
        /// </summary>
        public string ReturnName
        {
            get { return fastCSharp.net.returnValue.ReturnParameterName; }
        }
        /// <summary>
        /// 返回值类型
        /// </summary>
        public memberType methodReturnType { get; private set; }
        /// <summary>
        /// 返回值类型
        /// </summary>
        public memberType MethodReturnType
        {
            get
            {
                checkAsynchronousReturn();
                return methodReturnType;
            }
        }
        /// <summary>
        /// 函数泛型返回值类型
        /// </summary>
        private memberType genericReturnType;
        /// <summary>
        /// 函数泛型返回值类型
        /// </summary>
        public memberType GenericReturnType
        {
            get
            {
                if (genericReturnType == null)
                {
                    genericReturnType = MethodReturnType;
                    foreach (memberType type in Method.GenericParameters)
                    {
                        if (type.Type == genericReturnType.Type)
                        {
                            genericReturnType = typeof(object);
                            break;
                        }
                    }
                }
                return genericReturnType;
            }
        }
        /// <summary>
        /// 是否函数泛型返回值
        /// </summary>
        public bool IsGenericReturn
        {
            get { return MethodReturnType.Type != GenericReturnType.Type; }
        }
        /// <summary>
        /// 是否有返回值
        /// </summary>
        public bool MethodIsReturn
        {
            get
            {
                checkAsynchronousReturn();
                return methodReturnType.Type != typeof(void);
            }
        }
        /// <summary>
        /// 是否异步回调方法
        /// </summary>
        protected bool isAsynchronousCallback;
        /// <summary>
        /// 是否异步回调方法
        /// </summary>
        public bool IsAsynchronousCallback
        {
            get
            {
                checkAsynchronousReturn();
                return isAsynchronousCallback;
            }
        }
        /// <summary>
        /// 异步回调是否检测成功状态
        /// </summary>
        protected virtual bool isAsynchronousFunc { get { return true; } }
        /// <summary>
        /// 检测异步回调方法
        /// </summary>
        protected virtual void checkAsynchronousReturn()
        {
            if (methodParameters == null)
            {
                methodParameters = Method.Parameters;
                methodReturnType = Method.ReturnType;
                if (Method.ReturnType.Type == typeof(void) && methodParameters.Length != 0)
                {
                    Type type = methodParameters[methodParameters.Length - 1].ParameterType.Type;
                    if (type.IsGenericType)
                    {
                        Type parameterType = null;
                        if (isAsynchronousFunc && type.GetGenericTypeDefinition() == typeof(Func<,>))
                        {
                            Type[] types = type.GetGenericArguments();
                            if (types[1] == typeof(bool)) parameterType = types[0];
                        }
                        else if (type.GetGenericTypeDefinition() == typeof(Action<>)) parameterType = type.GetGenericArguments()[0];
                        if (parameterType != null)
                        {
                            if (parameterType == typeof(fastCSharp.net.returnValue))
                            {
                                isAsynchronousCallback = true;
                            }
                            else if (parameterType.IsGenericType && parameterType.GetGenericTypeDefinition() == typeof(fastCSharp.net.returnValue<>))
                            {
                                methodReturnType = parameterType.GetGenericArguments()[0];
                                isAsynchronousCallback = true;
                            }
                            if (isAsynchronousCallback)
                            {
                                methodParameters = parameterInfo.Get(methodParameters.getSub(0, methodParameters.Length - 1));
                            }
                        }
                    }
                }
            }
        }
    }
}
