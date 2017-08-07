using System;
using fastCSharp;

namespace fastCSharp.sql.expression
{
    /// <summary>
    /// 参数表达式
    /// </summary>
    internal class ParameterExpression : Expression
    {
        /// <summary>
        /// 参数类型
        /// </summary>
        public Type Type { get; private set; }
        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 参数表达式
        /// </summary>
        private ParameterExpression()
        {
            NodeType = ExpressionType.Parameter;
        }
        /// <summary>
        /// 添加到表达式池
        /// </summary>
        internal override void pushPool()
        {
            Type = null;
            Name = null;
            typePool<ParameterExpression>.PushNotNull(this);
        }
        /// <summary>
        /// 获取参数表达式
        /// </summary>
        /// <param name="type">参数类型</param>
        /// <param name="name">参数名称</param>
        /// <returns>参数表达式</returns>
        internal static ParameterExpression Get(Type type, string name)
        {
            ParameterExpression expression = typePool<ParameterExpression>.Pop() ?? new ParameterExpression();
            expression.Type = type;
            expression.Name = name;
            return expression;
        }
        /// <summary>
        /// 表达式转换
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns>表达式</returns>
        internal static ParameterExpression convert(System.Linq.Expressions.ParameterExpression expression)
        {
            return ParameterExpression.Get(expression.Type, expression.Name);
        }
    }
}
