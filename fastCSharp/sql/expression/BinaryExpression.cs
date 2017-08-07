using System;
using System.Reflection;
using fastCSharp;
using fastCSharp.threading;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql.expression
{
    /// <summary>
    /// 二元表达式
    /// </summary>
    internal class BinaryExpression : Expression
    {
        /// <summary>
        /// 添加对象数组
        /// </summary>
        /// <param name="value">对象数组</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static void pushObjectArray2(ref object[] value)
        {
            value[0] = value[1] = null;
            typePool<BinaryExpression, object[]>.Default.Push(ref value);
        }

        /// <summary>
        /// 左表达式
        /// </summary>
        public Expression Left { get; private set; }
        /// <summary>
        /// 右表达式
        /// </summary>
        public Expression Right { get; private set; }
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
                if (Left.IsConstant && Right.IsConstant && Method != null)
                {
                    object[] parameters = typePool<BinaryExpression, object[]>.Default.Pop() ?? new object[2];
                    object value;
                    try
                    {
                        parameters[0] = ((ConstantExpression)Left).Value;
                        parameters[1] = ((ConstantExpression)Right).Value;
                        value = Method.Invoke(null, parameters);
                    }
                    finally { pushObjectArray2(ref parameters); }
                    PushPool();
                    return ConstantExpression.Get(value);
                }
                Func<BinaryExpression, Expression> getSimpleExpression = getSimpleExpressions[(int)NodeType];
                if (getSimpleExpression != null) return getSimpleExpression(this);
                return this;
            }
        }
        /// <summary>
        /// 添加到表达式池
        /// </summary>
        internal override void pushPool()
        {
            Method = null;
            if (Left != null)
            {
                Left.PushCountPool();
                Left = null;
            }
            if (Right != null)
            {
                Right.PushCountPool();
                Right = null;
            }
            typePool<BinaryExpression>.PushNotNull(this);
        }
        /// <summary>
        /// 获取二元表达式
        /// </summary>
        /// <param name="type">表达式类型</param>
        /// <param name="left">左表达式</param>
        /// <param name="right">右表达式</param>
        /// <param name="method">运算符重载函数</param>
        /// <returns>二元表达式</returns>
        internal static BinaryExpression Get(ExpressionType type, Expression left, Expression right, MethodInfo method)
        {
            BinaryExpression expression = typePool<BinaryExpression>.Pop() ?? new BinaryExpression();
            expression.NodeType = type;
            expression.Method = method;
            ++(expression.Left = left.SimpleExpression).ExpressionCount;
            ++(expression.Right = right.SimpleExpression).ExpressionCount;
            return expression;
        }

        /// <summary>
        /// 获取简单表达式
        /// </summary>
        private static readonly Func<BinaryExpression, Expression>[] getSimpleExpressions;
        /// <summary>
        /// 获取简单表达式
        /// </summary>
        /// <param name="binaryExpression">二元表达式</param>
        /// <returns>简单表达式</returns>
        private static Expression getSimpleOrElse(BinaryExpression binaryExpression)
        {
            if (binaryExpression.Left.NodeType == ExpressionType.LogicConstant)
            {
                Expression expression;
                if (((logicConstantExpression)binaryExpression.Left).Value)
                {
                    expression = binaryExpression.Left;
                    binaryExpression.Left = null;
                }
                else
                {
                    expression = binaryExpression.Right;
                    binaryExpression.Right = null;
                }
                --expression.ExpressionCount;
                binaryExpression.PushPool();
                return expression;
            }
            if (binaryExpression.Right.NodeType == ExpressionType.LogicConstant)
            {
                Expression expression;
                if (((logicConstantExpression)binaryExpression.Right).Value)
                {
                    expression = binaryExpression.Right;
                    binaryExpression.Right = null;
                }
                else
                {
                    expression = binaryExpression.Left;
                    binaryExpression.Left = null;
                }
                --expression.ExpressionCount;
                binaryExpression.PushPool();
                return expression;
            }
            return binaryExpression;
        }
        /// <summary>
        /// 获取简单表达式
        /// </summary>
        /// <param name="binaryExpression">二元表达式</param>
        /// <returns>简单表达式</returns>
        private static Expression getSimpleAndAlso(BinaryExpression binaryExpression)
        {
            if (binaryExpression.Left.NodeType == ExpressionType.LogicConstant)
            {
                Expression expression;
                if (((logicConstantExpression)binaryExpression.Left).Value)
                {
                    expression = binaryExpression.Right;
                    binaryExpression.Right = null;
                }
                else
                {
                    expression = binaryExpression.Left;
                    binaryExpression.Left = null;
                }
                --expression.ExpressionCount;
                binaryExpression.PushPool();
                return expression;
            }
            if (binaryExpression.Right.NodeType == ExpressionType.LogicConstant)
            {
                Expression expression;
                if (((logicConstantExpression)binaryExpression.Right).Value)
                {
                    expression = binaryExpression.Left;
                    binaryExpression.Left = null;
                }
                else
                {
                    expression = binaryExpression.Right;
                    binaryExpression.Right = null;
                }
                --expression.ExpressionCount;
                binaryExpression.PushPool();
                return expression;
            }
            return binaryExpression;
        }
        /// <summary>
        /// 获取简单表达式
        /// </summary>
        /// <param name="binaryExpression">二元表达式</param>
        /// <returns>简单表达式</returns>
        private static Expression getSimpleEqual(BinaryExpression binaryExpression)
        {
            if (binaryExpression.Left.IsConstant && binaryExpression.Right.IsConstant)
            {
                object leftValue = ((ConstantExpression)binaryExpression.Left).Value;
                object rightValue = ((ConstantExpression)binaryExpression.Right).Value;
                binaryExpression.PushPool();
                return logicConstantExpression.Get(leftValue == null ? rightValue == null : leftValue.Equals(rightValue));
            }
            return binaryExpression;
        }
        /// <summary>
        /// 获取简单表达式
        /// </summary>
        /// <param name="binaryExpression">二元表达式</param>
        /// <returns>简单表达式</returns>
        private static Expression getSimpleNotEqual(BinaryExpression binaryExpression)
        {
            if (binaryExpression.Left.IsConstant && binaryExpression.Right.IsConstant)
            {
                object leftValue = ((ConstantExpression)binaryExpression.Left).Value;
                object rightValue = ((ConstantExpression)binaryExpression.Right).Value;
                binaryExpression.PushPool();
                return logicConstantExpression.Get(leftValue == null ? rightValue != null : !leftValue.Equals(rightValue));
            }
            return binaryExpression;
        }
        /// <summary>
        /// 获取简单表达式
        /// </summary>
        /// <param name="binaryExpression">二元表达式</param>
        /// <returns>简单表达式</returns>
        private static Expression getSimpleGreaterThanOrEqual(BinaryExpression binaryExpression)
        {
            if (binaryExpression.Left.IsConstant && binaryExpression.Right.IsConstant)
            {
                int value = ((IComparable)((ConstantExpression)binaryExpression.Left).Value)
                    .CompareTo(((ConstantExpression)binaryExpression.Right).Value);
                binaryExpression.PushPool();
                return logicConstantExpression.Get(value >= 0);
            }
            return binaryExpression;
        }
        /// <summary>
        /// 获取简单表达式
        /// </summary>
        /// <param name="binaryExpression">二元表达式</param>
        /// <returns>简单表达式</returns>
        private static Expression getSimpleGreaterThan(BinaryExpression binaryExpression)
        {
            if (binaryExpression.Left.IsConstant && binaryExpression.Right.IsConstant)
            {
                int value = ((IComparable)((ConstantExpression)binaryExpression.Left).Value)
                    .CompareTo(((ConstantExpression)binaryExpression.Right).Value);
                binaryExpression.PushPool();
                return logicConstantExpression.Get(value > 0);
            }
            return binaryExpression;
        }
        /// <summary>
        /// 获取简单表达式
        /// </summary>
        /// <param name="binaryExpression">二元表达式</param>
        /// <returns>简单表达式</returns>
        private static Expression getSimpleLessThan(BinaryExpression binaryExpression)
        {
            if (binaryExpression.Left.IsConstant && binaryExpression.Right.IsConstant)
            {
                int value = ((IComparable)((ConstantExpression)binaryExpression.Left).Value)
                    .CompareTo(((ConstantExpression)binaryExpression.Right).Value);
                binaryExpression.PushPool();
                return logicConstantExpression.Get(value < 0);
            }
            return binaryExpression;
        }
        /// <summary>
        /// 获取简单表达式
        /// </summary>
        /// <param name="binaryExpression">二元表达式</param>
        /// <returns>简单表达式</returns>
        private static Expression getSimpleLessThanOrEqual(BinaryExpression binaryExpression)
        {
            if (binaryExpression.Left.IsConstant && binaryExpression.Right.IsConstant)
            {
                int value = ((IComparable)((ConstantExpression)binaryExpression.Left).Value)
                    .CompareTo(((ConstantExpression)binaryExpression.Right).Value);
                binaryExpression.PushPool();
                return logicConstantExpression.Get(value <= 0);
            }
            return binaryExpression;
        }

        static BinaryExpression()
        {
            getSimpleExpressions = new Func<BinaryExpression, Expression>[fastCSharp.Enum.GetMaxValue<ExpressionType>(-1) + 1];
            getSimpleExpressions[(int)ExpressionType.OrElse] = getSimpleOrElse;
            getSimpleExpressions[(int)ExpressionType.AndAlso] = getSimpleAndAlso;
            getSimpleExpressions[(int)ExpressionType.Equal] = getSimpleEqual;
            getSimpleExpressions[(int)ExpressionType.NotEqual] = getSimpleNotEqual;
            getSimpleExpressions[(int)ExpressionType.GreaterThanOrEqual] = getSimpleGreaterThanOrEqual;
            getSimpleExpressions[(int)ExpressionType.GreaterThan] = getSimpleGreaterThan;
            getSimpleExpressions[(int)ExpressionType.LessThan] = getSimpleLessThan;
            getSimpleExpressions[(int)ExpressionType.LessThanOrEqual] = getSimpleLessThanOrEqual;
            //getSimpleExpressions[(int)expressionType.Add] = getSimpleAdd;
            //getSimpleExpressions[(int)expressionType.AddChecked] = getSimpleAdd;
            //getSimpleExpressions[(int)expressionType.Subtract] = getSimpleSubtract;
            //getSimpleExpressions[(int)expressionType.SubtractChecked] = getSimpleSubtract;
            //getSimpleExpressions[(int)expressionType.Multiply] = getSimpleMultiply;
            //getSimpleExpressions[(int)expressionType.MultiplyChecked] = getSimpleMultiply;
            //getSimpleExpressions[(int)expressionType.Divide] = getSimpleDivide;
            //getSimpleExpressions[(int)expressionType.Modulo] = getSimpleModulo;
            //getSimpleExpressions[(int)expressionType.Power] = getSimplePower;
            //getSimpleExpressions[(int)expressionType.Or] = getSimpleOr;
            //getSimpleExpressions[(int)expressionType.And] = getSimpleAnd;
            //getSimpleExpressions[(int)expressionType.ExclusiveOr] = getSimpleExclusiveOr;
            //getSimpleExpressions[(int)expressionType.LeftShift] = getSimpleLeftShift;
            //getSimpleExpressions[(int)expressionType.RightShift] = getSimpleRightShift;
        }
    }
}
