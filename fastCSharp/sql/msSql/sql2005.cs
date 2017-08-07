using System;
using System.Collections.Generic;
using fastCSharp.code.cSharp;
using System.Linq.Expressions;
using fastCSharp.emit;

namespace fastCSharp.sql.msSql
{
    /// <summary>
    /// SQL Server2005/2008/2012客户端
    /// </summary>
    public class sql2005 : sql2000
    {
        /// <summary>
        /// 排序名称
        /// </summary>
        private const string orderOverName = "_ROW_";
        /// <summary>
        /// SQL Server2005客户端
        /// </summary>
        /// <param name="connection">SQL连接信息</param>
        public sql2005(connection connection) : base(connection) { }
        /// <summary>
        /// 最大字符串长度
        /// </summary>
        protected override string maxString
        {
            get { return "max"; }
        }
        /// <summary>
        /// 获取表格名称的SQL语句
        /// </summary>
        protected override string GetTableNameSql
        {
            get
            {
                return "select name from sysobjects where objectproperty(id,'IsUserTable')=1";
            }
        }
        /// <summary>
        /// 根据表格名称获取表格信息的SQL语句
        /// </summary>
        /// <param name="tableName">表格名称</param>
        /// <returns>表格信息的SQL语句</returns>
        protected override string GetTableSql(string tableName)
        {
            return @"declare @id int
set @id=object_id(N'[dbo].[" + tableName + @"]')
if(select top 1 id from sysobjects where id=@id and objectproperty(id,N'IsUserTable')=1)is not null begin
 select columnproperty(id,name,'IsIdentity')as isidentity,id,xusertype,name,length,isnullable,colid,isnull((select top 1 text from syscomments where id=syscolumns.cdefault and colid=1),'')as defaultValue
  ,isnull((select value from ::fn_listextendedproperty(null,'user','dbo','table','" + tableName + @"','column',syscolumns.name)as property where property.name='MS_Description'),'')as remark
  from syscolumns where id=@id order by colid
 if @@rowcount<>0 begin
  select a.indid,a.colid,b.name,(case when b.status=2 then 'UQ' else(select top 1 xtype from sysobjects where name=b.name)end)as type from sysindexkeys a left join sysindexes b on a.id=b.id and a.indid=b.indid where a.id=@id order by a.indid,a.keyno
 end
end";
            //备注
            //"select top 1 value from ::fn_listextendedproperty(null,'user','dbo','table','" + tableName + "','column','" + reader["name"].ToString() + "')as property where property.name='MS_Description'"
        }
        /// <summary>
        /// 查询对象集合
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="query">查询信息</param>
        /// <param name="memberMap">成员位图</param>
        /// <returns>对象集合</returns>
        protected override IEnumerable<valueType> selectPushMemberMap<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, selectQuery<modelType> query, fastCSharp.code.memberMap<modelType> memberMap)
        {
            if (query != null && query.SkipCount != 0 && query.Orders.length() != 0)
            {
                if (sqlModel<modelType>.PrimaryKeys.Length == 1)
                {
                    return selectKeysPushMemberMap(sqlTool, query, sqlModel<modelType>.PrimaryKeys[0].SqlFieldName, memberMap);
                }
                if (sqlModel<modelType>.Identity != null)
                {
                    return selectKeysPushMemberMap(sqlTool, query, sqlModel<modelType>.Identity.SqlFieldName, memberMap);
                }
                return selectRowsPushMemberMap(sqlTool, query, memberMap);
            }
            return selectNoOrderPushMemberMap(sqlTool, query, memberMap);
        }
        /// <summary>
        /// 查询对象集合
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="query">查询信息</param>
        /// <param name="memberMap">成员位图</param>
        /// <param name="onGet"></param>
        public override void Select<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, selectQuery<modelType> query, fastCSharp.code.memberMap<modelType> memberMap, Action<subArray<valueType>> onGet)
        {
            if (onGet == null) log.Error.Throw(log.exceptionType.Null);
            subArray<valueType> values = default(subArray<valueType>);
            try
            {
                selector<valueType, modelType> selector = selector<valueType, modelType>.Get();
                if (selector != null)
                {
                    selector.MemberMap.CopyFrom(fastCSharp.emit.sqlModel<modelType>.MemberMap);
                    if (memberMap != null && !memberMap.IsDefault) selector.MemberMap.And(memberMap);
                    if (query != null && query.SkipCount != 0 && query.Orders.length() != 0)
                    {
                        if (sqlModel<modelType>.PrimaryKeys.Length == 1)
                        {
                            string sql = selectKeys(sqlTool, query, sqlModel<modelType>.PrimaryKeys[0].SqlFieldName, selector.MemberMap, selector.SqlStream);
                            if (sql != null)
                            {
                                selector.Get(this, sql, 0, ref onGet);
                                return;
                            }
                            values.array = nullValue<valueType>.Array;
                        }
                        else if (sqlModel<modelType>.Identity != null)
                        {
                            string sql = selectKeys(sqlTool, query, sqlModel<modelType>.Identity.SqlFieldName, selector.MemberMap, selector.SqlStream);
                            if (sql != null)
                            {
                                selector.Get(this, sql, 0, ref onGet);
                                return;
                            }
                            values.array = nullValue<valueType>.Array;
                        }
                        else
                        {
                            string sql = selectRows(sqlTool, query, selector.MemberMap, selector.SqlStream);
                            if (sql != null)
                            {
                                selector.Get(this, sql, 0, ref onGet);
                                return;
                            }
                            values.array = nullValue<valueType>.Array;
                        }
                    }
                    if (values.array == null)
                    {
                        string sql = selectNoOrder(sqlTool, query, selector.MemberMap, selector.SqlStream);
                        if (sql != null)
                        {
                            selector.Get(this, sql, query == null ? 0 : query.SkipCount, ref onGet);
                            return;
                        }
                        values.array = nullValue<valueType>.Array;
                    }
                    typePool<selector<valueType, modelType>>.PushNotNull(selector);
                }
            }
            finally
            {
                if (onGet != null) onGet(values);
            }
        }
        /// <summary>
        /// 查询对象集合
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        /// <typeparam name="modelType">成员位图类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="query">查询信息</param>
        /// <param name="keyName">关键之名称</param>
        /// <param name="memberMap">成员位图</param>
        /// <param name="sqlStream"></param>
        /// <returns></returns>
        private unsafe string selectKeys<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, selectQuery<modelType> query, string keyName, fastCSharp.code.memberMap<modelType> memberMap, charStream sqlStream)
            where valueType : class, modelType
            where modelType : class
        {
            pointer buffer = fastCSharp.sql.client.SqlBuffers.Get();
            try
            {
                using (sqlStream)
                {
                    sqlStream.UnsafeReset(buffer.Byte, SqlBufferSize << 1);
                    sqlStream.SimpleWriteNotNull("select ");
                    sqlModel<modelType>.GetNames(sqlStream, memberMap);
                    sqlStream.SimpleWriteNotNull(" from[");
                    sqlStream.SimpleWriteNotNull(sqlTool.TableName);
                    sqlStream.WriteNotNull("]with(nolock)where ");
                    sqlStream.SimpleWriteNotNull(keyName);
                    sqlStream.SimpleWriteNotNull(" in(select ");
                    sqlStream.SimpleWriteNotNull(keyName);
                    sqlStream.SimpleWriteNotNull(" from(select ");
                    sqlStream.SimpleWriteNotNull(keyName);
                    sqlStream.WriteNotNull(",row_number()over(");
                    int startIndex = sqlStream.Length;
                    query.WriteOrder(sqlTool, sqlStream);
                    int count = sqlStream.Length - startIndex;
                    sqlStream.SimpleWriteNotNull(")as ");
                    sqlStream.SimpleWriteNotNull(orderOverName);
                    sqlStream.SimpleWriteNotNull(" from[");
                    sqlStream.SimpleWriteNotNull(sqlTool.TableName);
                    sqlStream.SimpleWriteNotNull("]with(nolock)");
                    if (query.WriteWhereOnly(sqlTool, sqlStream))
                    {
                        sqlStream.SimpleWriteNotNull(")as T where ");
                        sqlStream.SimpleWriteNotNull(orderOverName);
                        sqlStream.SimpleWriteNotNull(" between ");
                        number.ToString(query.SkipCount, sqlStream);
                        sqlStream.SimpleWriteNotNull(" and ");
                        number.ToString(query.SkipCount + query.GetCount - 1, sqlStream);
                        sqlStream.Write(')');
                        if (count != 0) sqlStream.Write(sqlStream.Char + startIndex, count);
                        return sqlStream.ToString();
                    }
                }
            }
            finally { fastCSharp.sql.client.SqlBuffers.Push(ref buffer); }
            return null;
        }
        /// <summary>
        /// 查询对象集合
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        /// <typeparam name="modelType">成员位图类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="query">查询信息</param>
        /// <param name="keyName">关键之名称</param>
        /// <param name="memberMap">成员位图</param>
        /// <returns>对象集合</returns>
        private unsafe IEnumerable<valueType> selectKeysPushMemberMap<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, selectQuery<modelType> query, string keyName, fastCSharp.code.memberMap<modelType> memberMap)
            where valueType : class, modelType
            where modelType : class
        {
            string sql = selectKeys(sqlTool, query, keyName, memberMap, new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1));
            return sql != null ? selectPushMemberMap<valueType, modelType>(sql, 0, memberMap) : nullValue<valueType>.Array;
        }
        /// <summary>
        /// 查询对象集合
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="query">查询信息</param>
        /// <param name="memberMap">成员位图</param>
        /// <param name="sqlStream"></param>
        /// <returns></returns>
        private unsafe string selectRows<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, selectQuery<modelType> query, fastCSharp.code.memberMap<modelType> memberMap, charStream sqlStream)
            where valueType : class,  modelType
            where modelType : class
        {
            pointer buffer = fastCSharp.sql.client.SqlBuffers.Get();
            try
            {
                using (sqlStream)
                {
                    sqlStream.UnsafeReset(buffer.Byte, SqlBufferSize << 1);
                    sqlStream.WriteNotNull("select * from(select ");
                    sqlModel<modelType>.GetNames(sqlStream, memberMap);
                    sqlStream.WriteNotNull(",row_number()over(");
                    query.WriteOrder(sqlTool, sqlStream);
                    sqlStream.SimpleWriteNotNull(")as ");
                    sqlStream.SimpleWriteNotNull(orderOverName);
                    sqlStream.SimpleWriteNotNull(" from[");
                    sqlStream.SimpleWriteNotNull(sqlTool.TableName);
                    sqlStream.SimpleWriteNotNull("]with(nolock)");
                    if (query.WriteWhereOnly(sqlTool, sqlStream))
                    {
                        sqlStream.SimpleWriteNotNull(")as T where ");
                        sqlStream.SimpleWriteNotNull(orderOverName);
                        sqlStream.SimpleWriteNotNull(" between ");
                        number.ToString(query.SkipCount, sqlStream);
                        sqlStream.SimpleWriteNotNull(" and ");
                        number.ToString(query.SkipCount + query.GetCount - 1, sqlStream);
                        return sqlStream.ToString();
                    }
                }
            }
            finally { fastCSharp.sql.client.SqlBuffers.Push(ref buffer); }
            return null;
        }
        /// <summary>
        /// 查询对象集合
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="query">查询信息</param>
        /// <param name="memberMap">成员位图</param>
        /// <returns>对象集合</returns>
        private unsafe IEnumerable<valueType> selectRowsPushMemberMap<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, selectQuery<modelType> query, fastCSharp.code.memberMap<modelType> memberMap)
            where valueType : class,  modelType
            where modelType : class
        {
            string sql = selectRows(sqlTool, query, memberMap, new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1));
            return sql != null ? selectPushMemberMap<valueType, modelType>(sql, 0, memberMap) : nullValue<valueType>.Array;
        }
    }
}
