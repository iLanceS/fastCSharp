using System;
using System.Collections;
using fastCSharp.sql.expression;

namespace fastCSharp.sql.mySql
{
    /// <summary>
    /// 表达式转换
    /// </summary>
    internal class expressionConverter : fastCSharp.sql.msSql.expressionConverter
    {
        /// <summary>
        /// 表达式转换
        /// </summary>
        /// <param name="constantConverter">常量转换</param>
        private expressionConverter(constantConverter constantConverter)
            : base(constantConverter)
        {
        }
        /// <summary>
        /// 转换表达式
        /// </summary>
        /// <param name="converter">表达式转换器</param>
        /// <param name="expression">表达式</param>
        protected override void convertNotEqual(expression.converter converter, BinaryExpression expression)
        {
            convertBinaryExpression(converter, expression, '!', '=');
        }
        /// <summary>
        /// 表达式转换
        /// </summary>
        internal static new readonly expressionConverter Default = new expressionConverter(Enum<fastCSharp.sql.type, fastCSharp.sql.typeInfo>.Array((byte)type.MySql).Converter);
    }
}
