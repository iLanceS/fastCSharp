using System;
using System.Collections;
using fastCSharp.sql.expression;

namespace fastCSharp.sql.msSql
{
    /// <summary>
    /// 表达式转换
    /// </summary>
    internal class expressionConverter : fastCSharp.sql.expression.expressionConverter
    {
         /// <summary>
        /// 表达式转换
        /// </summary>
        /// <param name="constantConverter">常量转换</param>
        protected expressionConverter(constantConverter constantConverter)
            : base(constantConverter)
        {
            converters[(int)ExpressionType.Constant] = convertConstant;
            converters[(int)ExpressionType.InSet] = convertInSet;
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        private void convertConstant(sql.expression.converter converter, Expression expression)
        {
            object value = ((ConstantExpression)expression).Value;
            if (value != null)
            {
                Action<charStream, object> toString = constantConverter[value.GetType()];
                if (toString != null) toString(converter.Stream, value);
                else stringConverter(converter.Stream, value.ToString());
            }
            else fastCSharp.web.ajax.WriteNull(converter.Stream);
        }

        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        private void convertInSet(sql.expression.converter converter, Expression expression)
        {
            charStream stream = converter.Stream;
            BinaryExpression binaryExpression = (BinaryExpression)expression;
            Expression left = binaryExpression.Left;
            converters[(int)left.NodeType](converter, left);
            stream.SimpleWriteNotNull(" In(");
            Action<charStream, object> toString = null;
            int index = -1;
            foreach (object value in (IEnumerable)((ConstantExpression)binaryExpression.Right).Value)
            {
                if (++index == 0) toString = constantConverter[value.GetType()];
                else stream.Write(',');
                if (toString == null) stringConverter(stream, value.ToString());
                else toString(stream, value);
            }
            stream.Write(')');
        }
        /// <summary>
        /// 表达式转换
        /// </summary>
        internal static readonly expressionConverter Default = new expressionConverter(Enum<fastCSharp.sql.type, fastCSharp.sql.typeInfo>.Array((int)(byte)type.Sql2000).Converter);
    }
}
