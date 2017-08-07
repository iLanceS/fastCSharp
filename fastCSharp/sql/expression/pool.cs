using System;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql.expression
{
    /// <summary>
    /// 表达式
    /// </summary>
    internal static class pool
    {
        /// <summary>
        /// 添加到表达式池
        /// </summary>
        /// <param name="expression">表达式</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static void PushPool(fastCSharp.sql.expression.Expression expression)
        {
            if (expression != null) expression.PushPool();
        }
        /// <summary>
        /// 添加到表达式池
        /// </summary>
        /// <param name="expression">表达式</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static void PushPool<expressionType>(ref expressionType expression)
            where expressionType : fastCSharp.sql.expression.Expression
        {
            if (expression != null)
            {
                expression.PushPool();
                expression = null;
            }
        }
    }
}
