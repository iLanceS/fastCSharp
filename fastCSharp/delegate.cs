using System;

namespace fastCSharp
{
    /// <summary>
    /// 字符串转换委托
    /// </summary>
    /// <typeparam name="returnType">目标类型</typeparam>
    /// <param name="stringValue">字符串</param>
    /// <param name="value">目标对象</param>
    /// <returns>是否转换成功</returns>
    public delegate bool tryParse<returnType>(string stringValue, out returnType value);
    /// <summary>
    /// 入池函数调用委托
    /// </summary>
    /// <typeparam name="parameterType">输入参数类型</typeparam>
    /// <param name="parameter">输入参数</param>
    public delegate void pushPool<parameterType>(ref parameterType parameter);
}
