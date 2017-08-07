using System;
using System.Reflection;
using fastCSharp;

namespace fastCSharp.sql.expression
{
    /// <summary>
    /// 字段表达式
    /// </summary>
    internal class fieldExpression : MemberExpression
    {
        /// <summary>
        /// 字段信息
        /// </summary>
        public FieldInfo FieldInfo { get; private set; }
        /// <summary>
        /// 简单表达式
        /// </summary>
        public override Expression SimpleExpression
        {
            get
            {
                if (Expression.IsConstant)
                {
                    object value = FieldInfo.GetValue(((ConstantExpression)Expression).Value);
                    PushPool();
                    return ConstantExpression.Get(value);
                }
                return this;
            }
        }
        /// <summary>
        /// 字段表达式
        /// </summary>
        private fieldExpression()
        {
            NodeType = ExpressionType.FieldAccess;
        }
        /// <summary>
        /// 添加到表达式池
        /// </summary>
        internal override void pushPool()
        {
            FieldInfo = null;
            clear();
            typePool<fieldExpression>.PushNotNull(this);
        }
        /// <summary>
        /// 获取字段表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="field">字段信息</param>
        /// <returns>字段表达式</returns>
        internal static fieldExpression Get(Expression expression, FieldInfo field)
        {
            fieldExpression fieldExpression = typePool<fieldExpression>.Pop() ?? new fieldExpression();
            fieldExpression.FieldInfo = field;
            fieldExpression.set(expression);
            return fieldExpression;
        }
    }
}
