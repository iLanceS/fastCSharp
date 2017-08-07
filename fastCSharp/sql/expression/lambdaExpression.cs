using System;
using fastCSharp;

namespace fastCSharp.sql.expression
{
    /// <summary>
    /// lambda表达式
    /// </summary>
    internal class LambdaExpression : Expression
    {
        /// <summary>
        /// 表达式主体
        /// </summary>
        public Expression Body { get; private set; }
        /// <summary>
        /// 参数
        /// </summary>
        public ParameterExpression[] Parameters { get; private set; }
        /// <summary>
        /// 是否逻辑常量表达式
        /// </summary>
        public bool IsLogicConstantExpression
        {
            get { return Body.NodeType == ExpressionType.LogicConstant; }
        }
        /// <summary>
        /// 逻辑常量值
        /// </summary>
        public bool LogicConstantValue
        {
            get { return ((logicConstantExpression)Body).Value; }
        }
        /// <summary>
        /// 委托关联表达式
        /// </summary>
        protected LambdaExpression()
        {
            NodeType = ExpressionType.Lambda;
        }
        /// <summary>
        /// 清除数据
        /// </summary>
        protected void clear()
        {
            Body.PushCountPool();
            Body = null;
            if (Parameters != null)
            {
                foreach (ParameterExpression parameter in Parameters) parameter.PushCountPool();
                Parameters = null;
            }
        }
        /// <summary>
        /// 委托关联表达式
        /// </summary>
        /// <param name="body">表达式主体</param>
        /// <param name="parameters">参数</param>
        protected void set(Expression body, ParameterExpression[] parameters)
        {
            ++(Body = body.SimpleExpression).ExpressionCount;
            this.Parameters = parameters;
            if (parameters != null)
            {
                foreach (ParameterExpression parameter in parameters) ++parameter.ExpressionCount;
            }
        }
        ///// <summary>
        ///// 根据参数名称获取参数表达式
        ///// </summary>
        ///// <param name="name">参数名称</param>
        ///// <returns>参数表达式</returns>
        //public ParameterExpression GetParameter(string name)
        //{
        //    if (Parameters != null)
        //    {
        //        foreach (ParameterExpression parameter in Parameters)
        //        {
        //            if (parameter.Name == name) return parameter;
        //        }
        //    }
        //    return null;
        //}
        /// <summary>
        /// 添加到表达式池
        /// </summary>
        internal override void pushPool()
        {
            clear();
            typePool<LambdaExpression>.PushNotNull(this);
        }
        /// <summary>
        /// 获取委托关联表达式
        /// </summary>
        /// <param name="body">表达式主体</param>
        /// <param name="parameters">参数</param>
        /// <returns>委托关联表达式</returns>
        internal static LambdaExpression Get(Expression body, ParameterExpression[] parameters)
        {
            LambdaExpression expression = typePool<LambdaExpression>.Pop() ?? new LambdaExpression();
            expression.set(body, parameters);
            return expression;
        }
        /// <summary>
        /// 表达式转换
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns>表达式</returns>
        internal static LambdaExpression convert(System.Linq.Expressions.LambdaExpression expression)
        {
            return LambdaExpression.Get(Expression.convert(expression.Body)
                , expression.Parameters.getArray(value => ParameterExpression.convert(value)));
        }
    }
}
