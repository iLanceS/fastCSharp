using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using fastCSharp.reflection;
using fastCSharp.sql.expression;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql.msSql
{
    /// <summary>
    /// 委托关联表达式转SQL表达式
    /// </summary>
    internal class converter : sql.expression.converter
    {
        /// <summary>
        /// 创建SQL
        /// </summary>
        /// <param name="expression">委托关联表达式</param>
        /// <param name="stream">SQL表达式流</param>
        protected override void create(LambdaExpression expression, charStream stream)
        {
            this.stream = stream;
            expressionConverter.Default[expression.Body.NodeType](this, expression.Body);
        }
        /// <summary>
        /// 委托关联表达式转SQL表达式
        /// </summary>
        /// <param name="expression">委托关联表达式</param>
        /// <param name="stream">SQL表达式流</param>
        /// <returns>参数成员名称</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static keyValue<string, string> Convert(LambdaExpression expression, charStream stream)
        {
            converter converter = new converter();
            converter.create(expression, stream);
            return new keyValue<string, string>(converter.FirstMemberName, converter.FirstMemberSqlName);
        }
        /// <summary>
        /// 委托关联表达式转SQL表达式
        /// </summary>
        /// <param name="expression">委托关联表达式</param>
        /// <returns>参数成员名称+SQL表达式</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static keyValue<string, string> Convert(LambdaExpression expression)
        {
            converter converter = new converter();
            string sql = converter.Create(expression);
            return new keyValue<string, string>(converter.FirstMemberName, sql);
        }
    }
}
