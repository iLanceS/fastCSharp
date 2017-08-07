using System;
using System.Reflection;

namespace fastCSharp.sql.expression
{
    /// <summary>
    /// 成员表达式
    /// </summary>
    internal abstract class MemberExpression : Expression
    {
        /// <summary>
        /// 表达式
        /// </summary>
        public Expression Expression { get; private set; }
        /// <summary>
        /// 成员表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        protected void set(Expression expression)
        {
            ++(Expression = expression.SimpleExpression).ExpressionCount;
            IsSimple = true;
            IsConstant = Expression.IsConstant;
        }
        /// <summary>
        /// 清除数据
        /// </summary>
        protected void clear()
        {
            Expression.PushCountPool();
            Expression = null;
        }
    }
}
