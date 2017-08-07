using System;
using System.Reflection;
using fastCSharp.reflection;

namespace fastCSharp.code
{
    /// <summary>
    /// 测试用例
    /// </summary>
    [auto(Name = "测试用例", IsTemplate = false)]
    internal sealed class test : ignoreMember, IAuto
    {
        /// <summary>
        /// 安装入口
        /// </summary>
        /// <param name="parameter">安装参数</param>
        /// <returns>是否安装成功</returns>
        public bool Run(auto.parameter parameter)
        {
            if (parameter != null)
            {
                //bool isTest = true;
                foreach (Type type in parameter.Types)
                {
                    if (!type.IsGenericType && !type.IsInterface && !type.IsEnum)
                    {
                        foreach (code.methodInfo methodInfo in code.methodInfo.GetMethods<testCase>(type, memberFilters.Static, false, true, false, false))
                        {
                            MethodInfo method = methodInfo.Method;
                            if (method.IsGenericMethod)
                            {
                                //isTest = false;
                                error.Message("测试用例不能是泛型函数 " + method.fullName());
                            }
                            else
                            {
                                Type returnType = method.ReturnType;
                                if ((returnType == typeof(bool) || returnType == typeof(void)) && method.GetParameters().Length == 0)
                                {
                                    try
                                    {
                                        object returnValue = method.Invoke(null, null);
                                        if (method.ReturnType == typeof(bool) && !(bool)returnValue)
                                        {
                                            //isTest = false;
                                            error.Message("测试用例调用失败 " + method.fullName());
                                        }
                                    }
                                    catch (Exception error)
                                    {
                                        code.error.Message(error.ToString());
                                    }
                                }
                            }
                        }
                    }
                }
                //return isTest;
            }
            //return false;
            return true;
        }
    }
}
