using System;
using System.Collections;
using System.Reflection;
using fastCSharp;

namespace fastCSharp.sql.expression
{
    /// <summary>
    /// 函数调用表达式
    /// </summary>
    internal sealed class MethodCallExpression : Expression
    {
        /// <summary>
        /// 动态函数对象表达式
        /// </summary>
        public Expression Instance { get; private set; }
        /// <summary>
        /// 函数信息
        /// </summary>
        public MethodInfo Method { get; private set; }
        /// <summary>
        /// 调用参数
        /// </summary>
        public Expression[] Arguments { get; private set; }
        /// <summary>
        /// 简单表达式
        /// </summary>
        public override Expression SimpleExpression
        {
            get
            {
                if (Method.ReflectedType != typeof(fastCSharp.sql.expressionCall))
                {
                    object value = Method.Invoke(Instance == null ? null : ((ConstantExpression)Instance).Value
                        , Arguments.getArray(argumentExpression => ((ConstantExpression)argumentExpression).Value));
                    PushPool();
                    return ConstantExpression.Get(value);
                }
                if (Method.Name == "In")
                {
                    object values = ((ConstantExpression)Arguments[1]).Value;
                    if (values != null)
                    {
                        int index = 0;
                        object firstValue = null;
                        foreach (object value in (IEnumerable)values)
                        {
                            if (index != 0)
                            {
                                Expression left = Arguments[0], right = Arguments[1];
                                Arguments[0] = Arguments[1] = null;
                                PushPool();
                                return BinaryExpression.Get(ExpressionType.InSet, left, right, null);
                            }
                            firstValue = value;
                            ++index;
                        }
                        if (index != 0)
                        {
                            Expression expression = Arguments[0];
                            Arguments[0] = null;
                            PushPool();
                            return BinaryExpression.Get(ExpressionType.Equal, expression, ConstantExpression.Get(firstValue), null).SimpleExpression;
                        }
                    }
                    PushPool();
                    return logicConstantExpression.Get(false);
                }
                return this;
            }
        }
        /// <summary>
        /// 函数调用表达式
        /// </summary>
        private MethodCallExpression()
        {
            NodeType = ExpressionType.Call;
            Arguments = nullValue<Expression>.Array;
            IsSimple = true;
        }
        /// <summary>
        /// 函数调用表达式
        /// </summary>
        /// <param name="method">函数信息</param>
        /// <param name="instance">动态函数对象表达式</param>
        /// <param name="arguments">调用参数</param>
        private void set(MethodInfo method, Expression instance, Expression[] arguments)
        {
            Method = method;
            if (instance != null) ++(Instance = instance.SimpleExpression).ExpressionCount;
            if (arguments.length() != 0)
            {
                int index = 0;
                foreach (Expression expression in arguments) ++(arguments[index++] = expression.SimpleExpression).ExpressionCount;
                Arguments = arguments;
            }
        }
        /// <summary>
        /// 添加到表达式池
        /// </summary>
        internal override void pushPool()
        {
            Method = null;
            if (Instance != null)
            {
                Instance.PushCountPool();
                Instance = null;
            }
            foreach (Expression expression in Arguments)
            {
                if (expression != null) expression.PushCountPool();
            }
            Arguments = nullValue<Expression>.Array;
            typePool<MethodCallExpression>.PushNotNull(this);
        }
        /// <summary>
        /// 获取函数调用表达式
        /// </summary>
        /// <param name="method">函数信息</param>
        /// <param name="instance">动态函数对象表达式</param>
        /// <param name="arguments">调用参数</param>
        /// <returns>函数调用表达式</returns>
        internal static MethodCallExpression Get(MethodInfo method, Expression instance, Expression[] arguments)
        {
            MethodCallExpression expression = typePool<MethodCallExpression>.Pop() ?? new MethodCallExpression();
            expression.set(method, instance, arguments);
            return expression;
        }
    }
}
