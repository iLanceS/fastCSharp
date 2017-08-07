using System;
using fastCSharp.reflection;
using fastCSharp.sql.expression;

namespace fastCSharp.sql.expression
{
    /// <summary>
    /// 委托关联表达式转SQL表达式
    /// </summary>
    internal abstract class converter
    {
        /// <summary>
        /// SQL流
        /// </summary>
        protected charStream stream;
        /// <summary>
        /// SQL流
        /// </summary>
        internal charStream Stream { get { return stream; } }
        /// <summary>
        /// 第一个参数成员名称
        /// </summary>
        internal string FirstMemberName;
        /// <summary>
        /// 第一个参数成员SQL名称
        /// </summary>
        internal string FirstMemberSqlName;
        /// <summary>
        /// 创建SQL
        /// </summary>
        /// <param name="expression">委托关联表达式</param>
        /// <returns>SQL表达式</returns>
        internal unsafe string Create(LambdaExpression expression)
        {
            pointer buffer = fastCSharp.sql.client.SqlBuffers.Get();
            try
            {
                using (stream = new charStream(buffer.Char, fastCSharp.sql.client.SqlBufferSize))
                {
                    create(expression, stream);
                    return stream.ToString();
                }
            }
            finally { fastCSharp.sql.client.SqlBuffers.Push(ref buffer); }
        }
        /// <summary>
        /// 创建SQL
        /// </summary>
        /// <param name="expression">委托关联表达式</param>
        /// <param name="stream">SQL表达式流</param>
        protected abstract void create(LambdaExpression expression, charStream stream);
    }
}
