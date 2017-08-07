using System;
using fastCSharp;

namespace fastCSharp.sql.expression
{
    /// <summary>
    /// 条件表达式
    /// </summary>
    internal class ConditionalExpression : Expression
    {
        /// <summary>
        /// 测试条件
        /// </summary>
        public Expression Test { get; private set; }
        /// <summary>
        /// 真表达式
        /// </summary>
        public Expression IfTrue { get; private set; }
        /// <summary>
        /// 假表达式
        /// </summary>
        public Expression IfFalse { get; private set; }
        /// <summary>
        /// 条件表达式
        /// </summary>
        private ConditionalExpression()
        {
            NodeType = ExpressionType.Conditional;
        }
        /// <summary>
        /// 简单表达式
        /// </summary>
        public override Expression SimpleExpression
        {
            get
            {
                if (Test.IsConstant)
                {
                    Expression expression;
                    object value = ((ConstantExpression)Test).Value;
                    if (value != null && (bool)value)
                    {
                        expression = IfTrue;
                        IfTrue = null;
                    }
                    else
                    {
                        expression = IfFalse;
                        IfFalse = null;
                    }
                    PushPool();
                    return expression;
                }
                return this;
            }
        }
        /// <summary>
        /// 添加到表达式池
        /// </summary>
        internal override void pushPool()
        {
            Test.PushCountPool();
            Test = null;
            if (IfTrue != null)
            {
                IfTrue.PushCountPool();
                IfTrue = null;
            }
            if (Test != null)
            {
                IfFalse.PushCountPool();
                IfFalse = null;
            }
            typePool<ConditionalExpression>.PushNotNull(this);
        }
        /// <summary>
        /// 获取条件表达式
        /// </summary>
        /// <param name="test">测试条件</param>
        /// <param name="ifTrue">真表达式</param>
        /// <param name="ifFalse">假表达式</param>
        /// <returns>条件表达式</returns>
        internal static ConditionalExpression Get(Expression test, Expression ifTrue, Expression ifFalse)
        {
            ConditionalExpression expression = typePool<ConditionalExpression>.Pop() ?? new ConditionalExpression();
            ++(expression.Test = test.SimpleExpression).ExpressionCount;
            ++(expression.IfTrue = ifTrue.SimpleExpression).ExpressionCount;
            ++(expression.IfFalse = ifFalse.SimpleExpression).ExpressionCount;
            return expression;
        }
    }
}
