using System;
using System.Reflection;
using fastCSharp;

namespace fastCSharp.sql.expression
{
    /// <summary>
    /// 属性表达式
    /// </summary>
    internal class propertyExpression : MemberExpression
    {
        /// <summary>
        /// 属性信息
        /// </summary>
        public PropertyInfo PropertyInfo { get; private set; }
        /// <summary>
        /// 简单表达式
        /// </summary>
        public override Expression SimpleExpression
        {
            get
            {
                if (Expression.IsConstant)
                {
                    object value = PropertyInfo.GetValue(((ConstantExpression)Expression).Value, null);
                    PushPool();
                    return ConstantExpression.Get(value);
                }
                return this;
            }
        }
        /// <summary>
        /// 属性表达式
        /// </summary>
        private propertyExpression()
        {
            NodeType = ExpressionType.PropertyAccess;
        }
        /// <summary>
        /// 添加到表达式池
        /// </summary>
        internal override void pushPool()
        {
            PropertyInfo = null;
            clear();
            typePool<propertyExpression>.PushNotNull(this);
        }
        /// <summary>
        /// 获取属性表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="property">属性信息</param>
        /// <returns>属性表达式</returns>
        internal static propertyExpression Get(Expression expression, PropertyInfo property)
        {
            propertyExpression propertyExpression = typePool<propertyExpression>.Pop() ?? new propertyExpression();
            propertyExpression.PropertyInfo = property;
            propertyExpression.set(expression);
            return propertyExpression;
        }
    }
}