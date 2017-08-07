using System;
using System.Reflection;
using fastCSharp;
using fastCSharp.threading;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql.expression
{
    /// <summary>
    /// 一元表达式
    /// </summary>
    internal class UnaryExpression : Expression
    {
        /// <summary>
        /// 添加对象数组
        /// </summary>
        /// <param name="value">对象数组</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected static void pushObjectArray(ref object[] value)
        {
            value[0] = null;
            typePool<UnaryExpression, object[]>.Default.Push(ref value);
        }

        /// <summary>
        /// 表达式
        /// </summary>
        public Expression Expression { get; private set; }
        /// <summary>
        /// 运算符重载函数
        /// </summary>
        public MethodInfo Method { get; private set; }
        /// <summary>
        /// 简单表达式
        /// </summary>
        public override Expression SimpleExpression
        {
            get
            {
                if (Expression.IsConstant && Method != null)
                {
                    object[] parameters = typePool<UnaryExpression, object[]>.Default.Pop() ?? new object[1];
                    object value;
                    try
                    {
                        parameters[0] = ((ConstantExpression)Expression).Value;
                        value = Method.Invoke(null, parameters);
                    }
                    finally { pushObjectArray(ref parameters); }
                    PushPool();
                    return ConstantExpression.Get(value);
                }
                Func<UnaryExpression, Expression> getSimple = getSimpleExpressions[(int)NodeType];
                if (getSimple != null) return getSimple(this);
                return this;
            }
        }
        /// <summary>
        /// 一元表达式
        /// </summary>
        /// <param name="type">表达式类型</param>
        /// <param name="expression">表达式</param>
        /// <param name="method">运算符重载函数</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void set(ExpressionType type, Expression expression, MethodInfo method)
        {
            NodeType = type;
            Method = method;
            ++(Expression = expression.SimpleExpression).ExpressionCount;
        }
        /// <summary>
        /// 清除数据
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void clear()
        {
            Method = null;
            Expression.PushCountPool();
            Expression = null;
        }
        /// <summary>
        /// 添加到表达式池
        /// </summary>
        internal override void pushPool()
        {
            clear();
            typePool<UnaryExpression>.PushNotNull(this);
        }
        /// <summary>
        /// 获取一元表达式
        /// </summary>
        /// <param name="type">表达式类型</param>
        /// <param name="expression">表达式</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>一元表达式</returns>
        internal static UnaryExpression Get(ExpressionType type, Expression expression, MethodInfo method)
        {
            UnaryExpression unaryExpression = typePool<UnaryExpression>.Pop() ?? new UnaryExpression();
            unaryExpression.set(type, expression, method);
            return unaryExpression;
        }

        /// <summary>
        /// 获取简单表达式
        /// </summary>
        private static readonly Func<UnaryExpression, Expression>[] getSimpleExpressions;
        /// <summary>
        /// 获取简单表达式
        /// </summary>
        /// <param name="binaryExpression">一元表达式</param>
        /// <returns>简单表达式</returns>
        private static Expression getSimpleNot(UnaryExpression binaryExpression)
        {
            if (binaryExpression.Expression.IsConstant)
            {
                bool value = !(bool)((ConstantExpression)binaryExpression.Expression).Value;
                binaryExpression.PushPool();
                return ConstantExpression.Get(value);
            }
            return binaryExpression;
        }
        /// <summary>
        /// 获取简单表达式
        /// </summary>
        /// <param name="binaryExpression">一元表达式</param>
        /// <returns>简单表达式</returns>
        private static Expression getSimpleIsTrue(UnaryExpression binaryExpression)
        {
            if (binaryExpression.Expression.IsConstant)
            {
                bool value = (bool)((ConstantExpression)binaryExpression.Expression).Value;
                binaryExpression.PushPool();
                return logicConstantExpression.Get(value);
            }
            return binaryExpression;
        }
        /// <summary>
        /// 获取简单表达式
        /// </summary>
        /// <param name="binaryExpression">一元表达式</param>
        /// <returns>简单表达式</returns>
        private static Expression getSimpleIsFalse(UnaryExpression binaryExpression)
        {
            if (binaryExpression.Expression.IsConstant)
            {
                bool value = !(bool)((ConstantExpression)binaryExpression.Expression).Value;
                binaryExpression.PushPool();
                return logicConstantExpression.Get(value);
            }
            return binaryExpression;
        }

        static UnaryExpression()
        {
            getSimpleExpressions = new Func<UnaryExpression, Expression>[fastCSharp.Enum.GetMaxValue<ExpressionType>(-1) + 1];
            getSimpleExpressions[(int)ExpressionType.Not] = getSimpleNot;
            //getSimpleExpressions[(int)expressionType.Negate] = getSimpleNegate;
            //getSimpleExpressions[(int)expressionType.NegateChecked] = getSimpleNegate;
            //getSimpleExpressions[(int)expressionType.UnaryPlus] = getSimpleUnaryPlus;
            getSimpleExpressions[(int)ExpressionType.IsTrue] = getSimpleIsTrue;
            getSimpleExpressions[(int)ExpressionType.IsFalse] = getSimpleIsFalse;
            //getSimpleExpressions[(int)expressionType.Convert] = getSimpleConvert;
            //getSimpleExpressions[(int)expressionType.ConvertChecked] = getSimpleConvert;
        }
    }
}
