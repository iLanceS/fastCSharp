using System;
using System.Linq.Expressions;
using fastCSharp.code.cSharp;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql
{
    /// <summary>
    /// 查询信息
    /// </summary>
    /// <typeparam name="valueType">对象类型</typeparam>
    public sealed class selectQuery<valueType>
    {
        /// <summary>
        /// 跳过记录数量
        /// </summary>
        public int SkipCount;
        /// <summary>
        /// 获取记录数量,0表示不限
        /// </summary>
        public int GetCount;
        /// <summary>
        /// 查询条件表达式
        /// </summary>
        public Expression<Func<valueType, bool>> Where;
        /// <summary>
        /// 排序表达式集合,false为升序,true为降序
        /// </summary>
        public keyValue<LambdaExpression, bool>[] Orders;
        /// <summary>
        /// 排序字符串集合,false为升序,true为降序
        /// </summary>
        internal keyValue<sqlModel.fieldInfo, bool>[] SqlFieldOrders;
        /// <summary>
        /// 查询条件表达式隐式转换为查询信息
        /// </summary>
        /// <param name="expression">查询条件表达式</param>
        /// <returns>查询信息</returns>
        public static implicit operator selectQuery<valueType>(Expression<Func<valueType, bool>> expression)
        {
            return expression == null ? null : new selectQuery<valueType> { Where = expression };
        }
        /// <summary>
        /// 是否已经创建查询索引
        /// </summary>
        private bool isCreatedIndex;
        /// <summary>
        /// 查询条件
        /// </summary>
        /// <param name="sqlTable">数据库表格操作工具</param>
        /// <param name="sqlStream">SQL表达式流</param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe bool WriteWhere(fastCSharp.emit.sqlTable.sqlToolBase sqlTable, charStream sqlStream)
        {
            return WriteWhere(sqlTable, sqlStream, Where, ref isCreatedIndex);
        }
        /// <summary>
        /// 查询条件
        /// </summary>
        /// <param name="sqlTable">数据库表格操作工具</param>
        /// <param name="sqlStream">SQL表达式流</param>
        /// <returns></returns>
        internal unsafe bool WriteWhereOnly(fastCSharp.emit.sqlTable.sqlToolBase sqlTable, charStream sqlStream)
        {
            if (Where == null) return true;
            int length = sqlStream.Length;
            bool logicConstantWhere = false;
            keyValue<string, string> name = sqlTable.Client.GetWhere(Where, sqlStream, ref logicConstantWhere);
            if (length == sqlStream.Length) return logicConstantWhere;
            if (name.Key != null)
            {
                isCreatedIndex = true;
                sqlTable.CreateIndex(name.Key, name.Value);
            }
            return true;
        }
        /// <summary>
        /// 排序字符串
        /// </summary>
        /// <param name="sqlTable">数据库表格操作工具</param>
        /// <param name="sqlStream">SQL表达式流</param>
        internal void WriteOrder(fastCSharp.emit.sqlTable.sqlToolBase sqlTable, charStream sqlStream)
        {
            if (Orders != null)
            {
                int isNext = 0;
                sqlStream.SimpleWriteNotNull(" order by ");
                foreach (keyValue<LambdaExpression, bool> order in Orders)
                {
                    if (isNext == 0) isNext = 1;
                    else sqlStream.Write(',');
                    keyValue<string, string> name = sqlTable.Client.GetSql(order.Key, sqlStream);
                    if (order.Value) sqlStream.SimpleWriteNotNull(" desc");
                    if (!isCreatedIndex && name.Key != null)
                    {
                        isCreatedIndex = true;
                        sqlTable.CreateIndex(name.Key, name.Value);
                    }
                }
            }
            else if (SqlFieldOrders != null)
            {
                int isNext = 0;
                sqlStream.SimpleWriteNotNull(" order by ");
                foreach (keyValue<sqlModel.fieldInfo, bool> order in SqlFieldOrders)
                {
                    if (isNext == 0) isNext = 1;
                    else sqlStream.Write(',');
                    sqlStream.SimpleWriteNotNull(order.Key.SqlFieldName);
                    if (order.Value) sqlStream.SimpleWriteNotNull(" desc");
                }
            }
        }
        /// <summary>
        /// 查询条件
        /// </summary>
        /// <param name="sqlTable">数据库表格操作工具</param>
        /// <param name="sqlStream">SQL表达式流</param>
        /// <param name="expression">查询条件表达式</param>
        /// <param name="isCreatedIndex">是否已经创建查询索引</param>
        /// <returns></returns>
        internal static unsafe bool WriteWhere(fastCSharp.emit.sqlTable.sqlToolBase sqlTable, charStream sqlStream, Expression<Func<valueType, bool>> expression, ref bool isCreatedIndex)
        {
            if (expression == null) return true;
            sqlStream.PrepLength(6);
            sqlStream.UnsafeAddLength(6);
            int length = sqlStream.Length;
            bool logicConstantWhere = false;
            keyValue<string, string> name = sqlTable.Client.GetWhere(expression, sqlStream, ref logicConstantWhere);
            if (length == sqlStream.Length)
            {
                sqlStream.UnsafeFreeLength(6);
                return logicConstantWhere;
            }
            if (name.Key != null)
            {
                byte* where = (byte*)(sqlStream.Char + length);
                *(uint*)(where - sizeof(uint)) = 'e' + (' ' << 16);
                *(uint*)(where - sizeof(uint) * 2) = 'e' + ('r' << 16);
                *(uint*)(where - sizeof(uint) * 3) = 'w' + ('h' << 16);
                isCreatedIndex = true;
                sqlTable.CreateIndex(name.Key, name.Value);
            }
            return true;
        }
    }
}
