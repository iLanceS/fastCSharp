using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using fastCSharp.reflection;
using fastCSharp.sql.expression;

namespace fastCSharp.sql.mySql
{
    /// <summary>
    /// 委托关联表达式转SQL表达式
    /// </summary>
    internal class converter : msSql.converter
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
    }
}
