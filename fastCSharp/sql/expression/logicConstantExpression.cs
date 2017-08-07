using System;
using fastCSharp;

namespace fastCSharp.sql.expression
{
    /// <summary>
    /// 逻辑常量表达式
    /// </summary>
    internal class logicConstantExpression : Expression
    {
        /// <summary>
        /// 逻辑值
        /// </summary>
        public bool Value { get; private set; }
        /// <summary>
        /// 逻辑常量表达式
        /// </summary>
        private logicConstantExpression()
        {
            NodeType = ExpressionType.LogicConstant;
            IsSimple = IsConstant = true;
        }
        /// <summary>
        /// 添加到表达式池
        /// </summary>
        internal override void pushPool()
        {
            typePool<logicConstantExpression>.PushNotNull(this);
        }
        /// <summary>
        /// 获取逻辑常量表达式
        /// </summary>
        /// <param name="value">逻辑值</param>
        /// <returns>逻辑常量表达式</returns>
        internal static logicConstantExpression Get(bool value)
        {
            logicConstantExpression expression = typePool<logicConstantExpression>.Pop() ?? new logicConstantExpression();
            expression.Value = value;
            return expression;
        }
    }
}