using System;
using fastCSharp;

namespace fastCSharp.sql.expression
{
    /// <summary>
    /// 常量表达式
    /// </summary>
    internal class ConstantExpression : Expression
    {
        /// <summary>
        /// 数据
        /// </summary>
        public object Value { get; private set; }
        /// <summary>
        /// 常量表达式
        /// </summary>
        private ConstantExpression()
        {
            NodeType = ExpressionType.Constant;
            IsSimple = IsConstant = true;
        }
        /// <summary>
        /// 添加到表达式池
        /// </summary>
        internal override void pushPool()
        {
            Value = null;
            typePool<ConstantExpression>.PushNotNull(this);
        }
        /// <summary>
        /// 获取常量表达式
        /// </summary>
        /// <param name="value">数据</param>
        /// <returns>常量表达式</returns>
        internal static ConstantExpression Get(object value)
        {
            ConstantExpression expression = typePool<ConstantExpression>.Pop() ?? new ConstantExpression();
            expression.Value = value;
            return expression;
        }
    }
}
