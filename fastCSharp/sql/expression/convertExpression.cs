using System;
using System.Reflection;
using fastCSharp;
using fastCSharp.threading;

namespace fastCSharp.sql.expression
{
    /// <summary>
    /// 类型转换表达式
    /// </summary>
    internal sealed class ConvertExpression : UnaryExpression
    {
        /// <summary>
        /// 转换目标类型
        /// </summary>
        public Type ConvertType { get; private set; }
        /// <summary>
        /// 简单表达式
        /// </summary>
        public override Expression SimpleExpression
        {
            get
            {
                if (Expression.IsConstant)
                {
                    object[] parameters = typePool<UnaryExpression, object[]>.Default.Pop() ?? new object[1];
                    object value;
                    try
                    {
                        parameters[0] = ((ConstantExpression)Expression).Value;
                        value = (Method ?? convertMethod.MakeGenericMethod(ConvertType)).Invoke(null, parameters);
                    }
                    finally { pushObjectArray(ref parameters); }
                    PushPool();
                    return ConstantExpression.Get(value);
                }
                return this;
            }
        }
        /// <summary>
        /// 添加到表达式池
        /// </summary>
        internal override void pushPool()
        {
            ConvertType = null;
            clear();
            typePool<ConvertExpression>.PushNotNull(this);
        }
        /// <summary>
        /// 获取类型转换表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="convertType">转换目标类型</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>类型转换表达式</returns>
        internal static ConvertExpression Get(Expression expression, Type convertType, MethodInfo method)
        {
            ConvertExpression convertExpression = typePool<ConvertExpression>.Pop() ?? new ConvertExpression();
            convertExpression.ConvertType = convertType;
            convertExpression.set(ExpressionType.Convert, expression, method);
            return convertExpression;
        }
        /// <summary>
        /// 强制类型转换
        /// </summary>
        /// <typeparam name="valueType">目标类型</typeparam>
        /// <param name="value">被转换的数据</param>
        /// <returns>转换后的数据</returns>
        private static valueType convert<valueType>(object value)
        {
            return (valueType)value;
        }
        /// <summary>
        /// 强制类型转换
        /// </summary>
        private static readonly MethodInfo convertMethod = typeof(ConvertExpression).GetMethod("convert", BindingFlags.Static | BindingFlags.NonPublic);
    }
}