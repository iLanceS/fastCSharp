using System;
using System.Collections;
using fastCSharp.sql.expression;

namespace fastCSharp.sql.expression
{
    /// <summary>
    /// 表达式转换
    /// </summary>
    internal abstract class expressionConverter
    {
        /// <summary>
        /// 常量转换
        /// </summary>
        protected constantConverter constantConverter;
        /// <summary>
        /// 字符串转换
        /// </summary>
        protected Action<charStream, string> stringConverter;
        /// <summary>
        /// 表达式转换处理集合
        /// </summary>
        protected Action<converter, Expression>[] converters;
        /// <summary>
        /// 获取常量转换处理函数
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <returns>失败返回null</returns>
        public Action<converter, Expression> this[ExpressionType type]
        {
            get
            {
                return converters[(int)type];
            }
        }
        /// <summary>
        /// 表达式转换
        /// </summary>
        /// <param name="constantConverter">常量转换</param>
        protected expressionConverter(constantConverter constantConverter)
        {
            stringConverter = (this.constantConverter = constantConverter)[typeof(string)];
            converters = new Action<converter, Expression>[Enum.GetMaxValue<ExpressionType>(-1) + 1];
            converters[(int)ExpressionType.OrElse] = convertOrElse;
            converters[(int)ExpressionType.AndAlso] = convertAndAlso;
            converters[(int)ExpressionType.Equal] = convertEqual;
            converters[(int)ExpressionType.NotEqual] = convertNotEqual;
            converters[(int)ExpressionType.GreaterThanOrEqual] = convertGreaterThanOrEqual;
            converters[(int)ExpressionType.GreaterThan] = convertGreaterThan;
            converters[(int)ExpressionType.LessThan] = convertLessThan;
            converters[(int)ExpressionType.LessThanOrEqual] = convertLessThanOrEqual;
            converters[(int)ExpressionType.Add] = convertAdd;
            converters[(int)ExpressionType.AddChecked] = convertAdd;
            converters[(int)ExpressionType.Subtract] = convertSubtract;
            converters[(int)ExpressionType.SubtractChecked] = convertSubtract;
            converters[(int)ExpressionType.Multiply] = convertMultiply;
            converters[(int)ExpressionType.MultiplyChecked] = convertMultiply;
            converters[(int)ExpressionType.Divide] = convertDivide;
            converters[(int)ExpressionType.Modulo] = convertModulo;
            converters[(int)ExpressionType.Power] = convertPower;
            converters[(int)ExpressionType.Or] = convertOr;
            converters[(int)ExpressionType.And] = convertAnd;
            converters[(int)ExpressionType.ExclusiveOr] = convertExclusiveOr;
            converters[(int)ExpressionType.LeftShift] = convertLeftShift;
            converters[(int)ExpressionType.RightShift] = convertRightShift;
            converters[(int)ExpressionType.FieldAccess] = convertFieldAccess;
            converters[(int)ExpressionType.PropertyAccess] = convertPropertyAccess;
            converters[(int)ExpressionType.Not] = convertNot;
            converters[(int)ExpressionType.Unbox] = convertUnbox;
            converters[(int)ExpressionType.Negate] = convertNegate;
            converters[(int)ExpressionType.NegateChecked] = convertNegate;
            converters[(int)ExpressionType.UnaryPlus] = convertUnaryPlus;
            converters[(int)ExpressionType.IsTrue] = convertIsTrue;
            converters[(int)ExpressionType.IsFalse] = convertIsFalse;
            converters[(int)ExpressionType.Convert] = convertConvert;
            converters[(int)ExpressionType.ConvertChecked] = convertConvert;
            converters[(int)ExpressionType.Conditional] = convertConditional;
            converters[(int)ExpressionType.Call] = convertCall;
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        private void convertOrElse(converter converter, Expression expression)
        {
            convertBinaryExpression(converter, (BinaryExpression)expression, "or");
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        private void convertAndAlso(converter converter, Expression expression)
        {
            convertBinaryExpression(converter, (BinaryExpression)expression, "and");
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        private void convertEqual(converter converter, Expression expression)
        {
            BinaryExpression binaryExpression = (BinaryExpression)expression;
            if (binaryExpression.Left.IsConstantNull)
            {
                charStream stream = converter.Stream;
                if (binaryExpression.Right.IsSimple)
                {
                    converters[(int)binaryExpression.Right.NodeType](converter, binaryExpression.Right);
                }
                else
                {
                    stream.Write('(');
                    converters[(int)binaryExpression.Right.NodeType](converter, binaryExpression.Right);
                    stream.Write(')');
                }
                stream.SimpleWriteNotNull(" is null");
            }
            else if (binaryExpression.Right.IsConstantNull)
            {
                charStream stream = converter.Stream;
                if (binaryExpression.Left.IsSimple)
                {
                    converters[(int)binaryExpression.Left.NodeType](converter, binaryExpression.Left);
                }
                else
                {
                    stream.Write('(');
                    converters[(int)binaryExpression.Left.NodeType](converter, binaryExpression.Left);
                    stream.Write(')');
                }
                stream.SimpleWriteNotNull(" is null");
            }
            else convertBinaryExpression(converter, (BinaryExpression)expression, '=');
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        private void convertNotEqual(converter converter, Expression expression)
        {
            BinaryExpression binaryExpression = (BinaryExpression)expression;
            if (binaryExpression.Left.IsConstantNull)
            {
                charStream stream = converter.Stream;
                if (binaryExpression.Right.IsSimple)
                {
                    converters[(int)binaryExpression.Right.NodeType](converter, binaryExpression.Right);
                }
                else
                {
                    stream.Write('(');
                    converters[(int)binaryExpression.Right.NodeType](converter, binaryExpression.Right);
                    stream.Write(')');
                }
                stream.SimpleWriteNotNull(" is not null");
            }
            else if (binaryExpression.Right.IsConstantNull)
            {
                charStream stream = converter.Stream;
                if (binaryExpression.Left.IsSimple)
                {
                    converters[(int)binaryExpression.Left.NodeType](converter, binaryExpression.Left);
                }
                else
                {
                    stream.Write('(');
                    converters[(int)binaryExpression.Left.NodeType](converter, binaryExpression.Left);
                    stream.Write(')');
                }
                stream.SimpleWriteNotNull(" is not null");
            }
            else convertNotEqual(converter, (BinaryExpression)expression);
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        protected virtual void convertNotEqual(converter converter, BinaryExpression expression)
        {
            convertBinaryExpression(converter, expression, '<', '>');
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        private void convertGreaterThanOrEqual(converter converter, Expression expression)
        {
            convertBinaryExpression(converter, (BinaryExpression)expression, '>', '=');
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        private void convertGreaterThan(converter converter, Expression expression)
        {
            convertBinaryExpression(converter, (BinaryExpression)expression, '>');
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        private void convertLessThan(converter converter, Expression expression)
        {
            convertBinaryExpression(converter, (BinaryExpression)expression, '<');
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        private void convertLessThanOrEqual(converter converter, Expression expression)
        {
            convertBinaryExpression(converter, (BinaryExpression)expression, '<', '=');
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        private void convertAdd(converter converter, Expression expression)
        {
            convertBinaryExpression(converter, (BinaryExpression)expression, '+');
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        private void convertSubtract(converter converter, Expression expression)
        {
            convertBinaryExpression(converter, (BinaryExpression)expression, '-');
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        private void convertMultiply(converter converter, Expression expression)
        {
            convertBinaryExpression(converter, (BinaryExpression)expression, '*');
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        private void convertDivide(converter converter, Expression expression)
        {
            convertBinaryExpression(converter, (BinaryExpression)expression, '/');
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        private void convertModulo(converter converter, Expression expression)
        {
            convertBinaryExpression(converter, (BinaryExpression)expression, '%');
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        private void convertPower(converter converter, Expression expression)
        {
            fastCSharp.log.Error.Throw(log.exceptionType.ErrorOperation);
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        private void convertOr(converter converter, Expression expression)
        {
            convertBinaryExpression(converter, (BinaryExpression)expression, '|');
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        private void convertAnd(converter converter, Expression expression)
        {
            convertBinaryExpression(converter, (BinaryExpression)expression, '&');
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        private void convertExclusiveOr(converter converter, Expression expression)
        {
            convertBinaryExpression(converter, (BinaryExpression)expression, '^');
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        private void convertLeftShift(converter converter, Expression expression)
        {
            convertBinaryExpression(converter, (BinaryExpression)expression, '<', '<');
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        private void convertRightShift(converter converter, Expression expression)
        {
            convertBinaryExpression(converter, (BinaryExpression)expression, '>', '>');
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="binaryExpression">表达式</param>
        /// <param name="char1">操作字符1</param>
        /// <param name="char2">操作字符2</param>
        protected void convertBinaryExpression(converter converter, BinaryExpression binaryExpression,
            char char1, char char2 = ' ')
        {
            charStream stream = converter.Stream;
            Expression left = binaryExpression.Left, right = binaryExpression.Right;
            if (left.IsSimple)
            {
                converters[(int)left.NodeType](converter, left);
            }
            else
            {
                stream.Write('(');
                converters[(int)left.NodeType](converter, left);
                stream.Write(')');
            }
            stream.Write(char1);
            stream.Write(char2);
            if (right.IsSimple)
            {
                converters[(int)right.NodeType](converter, right);
            }
            else
            {
                stream.Write('(');
                converters[(int)right.NodeType](converter, right);
                stream.Write(')');
            }
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="binaryExpression">表达式</param>
        /// <param name="type">操作字符串</param>
        private void convertBinaryExpression(converter converter, BinaryExpression binaryExpression, string type)
        {
            charStream stream = converter.Stream;
            Expression left = binaryExpression.Left, right = binaryExpression.Right;
            stream.Write('(');
            if (left.IsSimple)
            {
                converters[(int)left.NodeType](converter, left);
                stream.Write('=');
                stream.Write('1');
            }
            else converters[(int)left.NodeType](converter, left);
            stream.Write(')');
            stream.SimpleWriteNotNull(type);
            stream.Write('(');
            if (right.IsSimple)
            {
                converters[(int)right.NodeType](converter, right);
                stream.Write('=');
                stream.Write('1');
            }
            else converters[(int)right.NodeType](converter, right);
            stream.Write(')');
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="memberExpression">表达式</param>
        /// <param name="name">成员名称</param>
        private void convertMemberAccess(converter converter, MemberExpression memberExpression, string name)
        {
            if (memberExpression.Expression.GetType() == typeof(ParameterExpression))
            {
                string sqlName = fastCSharp.code.cSharp.sqlModel.ToSqlName(name);
                if (converter.FirstMemberName == null)
                {
                    converter.FirstMemberName = name;
                    converter.FirstMemberSqlName = sqlName;
                }
                converter.Stream.SimpleWriteNotNull(sqlName);
            }
            else fastCSharp.log.Error.Throw("未知成员表达式类型 " + memberExpression.Expression.GetType().Name, new System.Diagnostics.StackFrame(), true);
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        private void convertFieldAccess(converter converter, Expression expression)
        {
            fieldExpression fieldExpression = (fieldExpression)expression;
            convertMemberAccess(converter, fieldExpression, fieldExpression.FieldInfo.Name);
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        private void convertPropertyAccess(converter converter, Expression expression)
        {
            propertyExpression propertyExpression = (propertyExpression)expression;
            convertMemberAccess(converter, propertyExpression, propertyExpression.PropertyInfo.Name);
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        private void convertNot(converter converter, Expression expression)
        {
            expression = ((UnaryExpression)expression).Expression;
            if (expression.IsSimple)
            {
                converters[(int)expression.NodeType](converter, expression);
                converter.Stream.SimpleWriteNotNull("=0");
            }
            else
            {
                converter.Stream.Write('(');
                converters[(int)expression.NodeType](converter, expression);
                converter.Stream.SimpleWriteNotNull(")=0");
            }
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        private void convertUnbox(converter converter, Expression expression)
        {
            expression = ((UnaryExpression)expression).Expression;
            if (expression.IsSimple)
            {
                converters[(int)expression.NodeType](converter, expression);
            }
            else
            {
                converter.Stream.Write('(');
                converters[(int)expression.NodeType](converter, expression);
                converter.Stream.Write(')');
            }
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        private void convertNegate(converter converter, Expression expression)
        {
            expression = ((UnaryExpression)expression).Expression;
            converter.Stream.SimpleWriteNotNull("-(");
            converters[(int)expression.NodeType](converter, expression);
            converter.Stream.Write(')');
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        private void convertUnaryPlus(converter converter, Expression expression)
        {
            expression = ((UnaryExpression)expression).Expression;
            converter.Stream.SimpleWriteNotNull("+(");
            converters[(int)expression.NodeType](converter, expression);
            converter.Stream.Write(')');
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        private void convertIsTrue(converter converter, Expression expression)
        {
            expression = ((UnaryExpression)expression).Expression;
            if (expression.IsSimple)
            {
                converters[(int)expression.NodeType](converter, expression);
                converter.Stream.SimpleWriteNotNull("=1");
            }
            else
            {
                converter.Stream.Write('(');
                converters[(int)expression.NodeType](converter, expression);
                converter.Stream.SimpleWriteNotNull(")=1");
            }
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        private void convertIsFalse(converter converter, Expression expression)
        {
            expression = ((UnaryExpression)expression).Expression;
            if (expression.IsSimple)
            {
                converters[(int)expression.NodeType](converter, expression);
                converter.Stream.SimpleWriteNotNull("=0");
            }
            else
            {
                converter.Stream.Write('(');
                converters[(int)expression.NodeType](converter, expression);
                converter.Stream.SimpleWriteNotNull(")=0");
            }
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        private void convertConvert(converter converter, Expression expression)
        {
            ConvertExpression convertExpression = (ConvertExpression)expression;
            converter.Stream.SimpleWriteNotNull("cast(");
            converters[(int)convertExpression.Expression.NodeType](converter, convertExpression.Expression);
            converter.Stream.SimpleWriteNotNull(" as ");
            converter.Stream.SimpleWriteNotNull(convertExpression.ConvertType.formCSharpType().ToString());
            converter.Stream.Write(')');
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        private void convertConditional(converter converter, Expression expression)
        {
            charStream stream = converter.Stream;
            ConditionalExpression conditionalExpression = (ConditionalExpression)expression;
            Expression test = conditionalExpression.Test, ifTrue = conditionalExpression.IfTrue, ifFalse = conditionalExpression.IfFalse;
            stream.SimpleWriteNotNull("case when ");
            if (test.IsSimple)
            {
                converters[(int)test.NodeType](converter, test);
                stream.Write('=');
                stream.Write('1');
            }
            else converters[(int)test.NodeType](converter, test);
            stream.SimpleWriteNotNull(" then ");
            if (ifTrue.IsSimple) converters[(int)ifTrue.NodeType](converter, ifTrue);
            else
            {
                stream.Write('(');
                converters[(int)ifTrue.NodeType](converter, ifTrue);
                stream.Write(')');
            }
            stream.SimpleWriteNotNull(" else ");
            if (ifFalse.IsSimple) converters[(int)ifFalse.NodeType](converter, ifFalse);
            else
            {
                stream.Write('(');
                converters[(int)ifFalse.NodeType](converter, ifFalse);
                stream.Write(')');
            }
            stream.SimpleWriteNotNull(" end");
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        private void convertCall(converter converter, Expression expression)
        {
            MethodCallExpression methodCallExpression = (MethodCallExpression)expression;
            charStream stream = converter.Stream;
            stream.SimpleWriteNotNull(methodCallExpression.Method.Name);
            stream.Write('(');
            if (methodCallExpression.Arguments != null)
            {
                bool isNext = false;
                foreach (Expression argumentExpression in methodCallExpression.Arguments)
                {
                    if (isNext) stream.Write(',');
                    converters[(int)argumentExpression.NodeType](converter, argumentExpression);
                    isNext = true;
                }
            }
            stream.Write(')');
        }
    }
}
