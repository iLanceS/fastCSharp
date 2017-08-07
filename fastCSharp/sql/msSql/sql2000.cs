using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq.Expressions;
using fastCSharp.code;
using fastCSharp.code.cSharp;
using fastCSharp.emit;
using System.Threading;

namespace fastCSharp.sql.msSql
{
    /// <summary>
    /// SQL Server2000客户端
    /// </summary>
    public class sql2000 : client
    {
        ///// <summary>
        ///// 最大参数数量
        ///// </summary>
        //public const int MaxParameterCount = 2100 - 3;
        /// <summary>
        /// 最大时间值
        /// </summary>
        public static readonly DateTime MaxDateTime = new DateTime(9999, 12, 31, 23, 59, 59, 997);
        /// <summary>
        /// 异步连接字符串
        /// </summary>
        private string asynchronousConnection;
        /// <summary>
        /// SQL Server2000客户端
        /// </summary>
        /// <param name="connection">SQL连接信息</param>
        public sql2000(connection connection) : base(connection)
        {
            AsynchronousConnectionPool = connectionPool.Get(connection.Type, asynchronousConnection = connection.Connection + ";Asynchronous Processing=true");
        }
        /// <summary>
        /// 根据SQL连接类型获取SQL连接
        /// </summary>
        /// <param name="isAsynchronous">是否异步连接</param>
        /// <returns>SQL连接</returns>
        internal override DbConnection GetConnection(bool isAsynchronous)
        {
            if (isAsynchronous) return AsynchronousConnectionPool.Pop() ?? open(new SqlConnection(asynchronousConnection));
            return ConnectionPool.Pop() ?? open(new SqlConnection(this.Connection.Connection));
        }
        /// <summary>
        /// 获取SQL命令
        /// </summary>
        /// <param name="connection">SQL连接</param>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="type">SQL命令类型</param>
        /// <returns>SQL命令</returns>
        public override DbCommand GetCommand
            (DbConnection connection, string sql, SqlParameter[] parameters = null, CommandType type = CommandType.Text)
        {
            DbCommand command = new SqlCommand(sql, (SqlConnection)connection);
            command.CommandType = type;
            if (parameters != null) command.Parameters.AddRange(parameters);
            return command;
        }
        /// <summary>
        /// 获取数据适配器
        /// </summary>
        /// <param name="command">SQL命令</param>
        /// <returns>数据适配器</returns>
        protected override DbDataAdapter getAdapter(DbCommand command)
        {
            return new SqlDataAdapter((SqlCommand)command);
        }
        /// <summary>
        /// 获取数据表格
        /// </summary>
        /// <param name="tableName">表格名称</param>
        /// <returns>数据表格</returns>
        public override DataTable GetDataTable(string tableName)
        {
            if (tableName != null && tableName.Length != 0)
            {
                DbConnection connection = GetConnection(false);
                if (connection != null)
                {
                    byte isExecute = 0;
                    try
                    {
                        DataTable table = GetDataTable(GetCommand(connection, "select * from [" + tableName + "]"), ref isExecute);
                        if (table != null)
                        {
                            table.TableName = tableName;
                            return table;
                        }
                    }
                    catch (Exception error)
                    {
                        fastCSharp.log.Error.Add(error, tableName, false);
                    }
                    finally
                    {
                        if (isExecute == 0) connection.Dispose();
                        else pushConnection(ref connection, false);
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 是否支持DataTable导入
        /// </summary>
        protected override bool isImport
        {
            get { return true; }
        }
        /// <summary>
        /// 导入数据集合
        /// </summary>
        /// <param name="connection">SQL连接</param>
        /// <param name="data">数据集合</param>
        /// <param name="batchSize">批处理数量</param>
        /// <param name="timeout">超时秒数</param>
        /// <returns>成功导入数量</returns>
        internal override int Import(DbConnection connection, DataTable data, int batchSize, int timeout)
        {
            using (SqlBulkCopy copy = new SqlBulkCopy((SqlConnection)connection))
            {
                int count = data.Rows.Count;
                if (batchSize <= 0) batchSize = fastCSharp.config.sql.Default.ImportBatchSize;
                copy.BulkCopyTimeout = timeout == 0 ? (count / batchSize) + 1 : timeout;
                copy.BatchSize = batchSize;
                copy.DestinationTableName = data.TableName;
                foreach (DataColumn column in data.Columns) copy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                copy.WriteToServer(data);
                return count;
            }
        }
        /// <summary>
        /// 判断表格是否存在
        /// </summary>
        /// <param name="connection">SQL连接</param>
        /// <param name="tableName">表格名称</param>
        /// <param name="isExecute">SQL连接执行是否成功</param>
        /// <returns>表格是否存在</returns>
        protected override bool isTable(DbConnection connection, string tableName, ref byte isExecute)
        {
            string sql = "select top 1 id from dbo.sysobjects where id=object_id(N'[" + this.Connection.Owner + "].[" + tableName + "]')and objectproperty(id,N'IsUserTable')=1";
            using (DbCommand command = GetCommand(connection, sql))
            {
                object value = command.ExecuteScalar();
                isExecute = 1;
                if (value != null && value != DBNull.Value) return (int)value != 0;
            }
            return false;
        }
        /// <summary>
        /// 创建表格
        /// </summary>
        /// <param name="connection">SQL连接</param>
        /// <param name="table">表格信息</param>
        internal unsafe override bool createTable(DbConnection connection, table table)
        {
            string tableName = table.Columns.Name, sql;
            pointer buffer = fastCSharp.sql.client.SqlBuffers.Get();
            try
            {
                using (charStream sqlStream = new charStream(buffer.Char, fastCSharp.sql.client.SqlBufferSize))
                {
                    sqlStream.SimpleWriteNotNull("create table[");
                    sqlStream.SimpleWriteNotNull(this.Connection.Owner);
                    sqlStream.SimpleWriteNotNull("].[");
                    sqlStream.SimpleWriteNotNull(tableName);
                    sqlStream.SimpleWriteNotNull("](");
                    bool isTextImage = false, isNext = false;
                    foreach (column column in table.Columns.Columns)
                    {
                        if (isNext) sqlStream.Write(',');
                        appendColumn(sqlStream, column);
                        if (!isTextImage) isTextImage = column.DbType.isTextImageType();
                        isNext = true;
                    }
                    columnCollection primaryKey = table.PrimaryKey;
                    if (primaryKey != null && primaryKey.Columns.length() != 0)
                    {
                        isNext = false;
                        sqlStream.SimpleWriteNotNull(",primary key(");
                        foreach (column column in primaryKey.Columns)
                        {
                            if (isNext) sqlStream.Write(',');
                            sqlStream.SimpleWriteNotNull(column.SqlName);
                            isNext = true;
                        }
                        sqlStream.Write(')');
                    }
                    sqlStream.SimpleWriteNotNull(")on[primary]");
                    if (isTextImage) sqlStream.WriteNotNull(" textimage_on[primary]");
                    foreach (column column in table.Columns.Columns)
                    {
                        if (!string.IsNullOrEmpty(column.Remark))
                        {
                            sqlStream.WriteNotNull(@"
exec dbo.sp_addextendedproperty @name=N'MS_Description',@value=N");
                            stringConverter(sqlStream, column.Remark);
                            sqlStream.WriteNotNull(",@level0type=N'USER',@level0name=N'");
                            sqlStream.SimpleWriteNotNull(this.Connection.Owner);
                            sqlStream.WriteNotNull("',@level1type=N'TABLE',@level1name=N'");
                            sqlStream.SimpleWriteNotNull(tableName);
                            sqlStream.WriteNotNull("', @level2type=N'COLUMN',@level2name=N'");
                            sqlStream.SimpleWriteNotNull(column.SqlName);
                            sqlStream.Write('\'');
                        }
                    }
                    if (table.Indexs != null)
                    {
                        foreach (columnCollection columns in table.Indexs)
                        {
                            if (columns != null && columns.Columns.length() != 0)
                            {
                                sqlStream.SimpleWriteNotNull(@"
create");
                                if (columns.Type == columnCollection.type.UniqueIndex) sqlStream.SimpleWriteNotNull(" unique");
                                sqlStream.SimpleWriteNotNull(" index[");
                                AppendIndexName(sqlStream, tableName, columns);
                                sqlStream.SimpleWriteNotNull("]on[");
                                sqlStream.SimpleWriteNotNull(this.Connection.Owner);
                                sqlStream.SimpleWriteNotNull("].[");
                                sqlStream.SimpleWriteNotNull(tableName);
                                sqlStream.SimpleWriteNotNull("](");
                                isNext = false;
                                foreach (column column in columns.Columns)
                                {
                                    if (isNext) sqlStream.Write(',');
                                    //sqlStream.Write('[');
                                    sqlStream.SimpleWriteNotNull(column.SqlName);
                                    //sqlStream.Write(']');
                                    isNext = true;
                                }
                                sqlStream.SimpleWriteNotNull(")on[primary]");
                            }
                        }
                    }
                    sql = sqlStream.ToString();
                }
            }
            finally { fastCSharp.sql.client.SqlBuffers.Push(ref buffer); }
            return executeNonQuery(connection, sql) != ExecuteNonQueryError;
        }
        /// <summary>
        /// 最大字符串长度
        /// </summary>
        private const int maxStringSize = 4000;
        /// <summary>
        /// 成员信息转换为数据列
        /// </summary>
        /// <param name="type">成员类型</param>
        /// <param name="sqlMember">SQL成员信息</param>
        /// <returns>数据列</returns>
        internal override column getColumn(Type type, dataMember sqlMember)
        {
            SqlDbType sqlType = SqlDbType.NVarChar;
            int size = maxStringSize;
            memberType memberType = sqlMember.DataType != null ? sqlMember.DataType : type;
            if (memberType.IsString)
            {
                if (sqlMember.MaxStringLength > 0 && sqlMember.MaxStringLength <= maxStringSize)
                {
                    if (sqlMember.IsFixedLength) sqlType = sqlMember.IsAscii ? SqlDbType.Char : SqlDbType.NChar;
                    else sqlType = sqlMember.IsAscii ? SqlDbType.VarChar : SqlDbType.NVarChar;
                    size = sqlMember.MaxStringLength <= maxStringSize ? sqlMember.MaxStringLength : maxStringSize;
                }
                else if (!sqlMember.IsFixedLength && sqlMember.MaxStringLength == -1)
                {
                    sqlType = sqlMember.IsAscii ? SqlDbType.VarChar : SqlDbType.NVarChar;
                    size = sqlMember.MaxStringLength <= maxStringSize ? sqlMember.MaxStringLength : maxStringSize;
                }
                else
                {
                    sqlType = sqlMember.IsAscii ? SqlDbType.Text : SqlDbType.NText;
                    size = int.MaxValue;
                }
            }
            else
            {
                sqlType = memberType.Type.formCSharpType();
                size = sqlType.getSize();
            }
            return new column
            {
                DbType = sqlType,
                Size = size,
                IsNull = (sqlMember.IsDefaultMember && !memberType.IsString ? ((memberType)type).IsNull : sqlMember.IsNull),
                DefaultValue = sqlMember.DefaultValue,
                UpdateValue = sqlMember.UpdateValue
            };
        }
        /// <summary>
        /// 删除表格
        /// </summary>
        /// <param name="connection">SQL连接</param>
        /// <param name="tableName">表格名称</param>
        protected override bool dropTable(DbConnection connection, string tableName)
        {
            return executeNonQuery(connection, "drop table[" + this.Connection.Owner + "].[" + tableName + "]") != ExecuteNonQueryError;
        }
        /// <summary>
        /// 最大字符串长度
        /// </summary>
        protected virtual string maxString
        {
            get { return "max"; }
        }
        /// <summary>
        /// 写入列信息
        /// </summary>
        /// <param name="sqlStream">SQL语句流</param>
        /// <param name="column">列信息</param>
        private void appendColumn(charStream sqlStream, column column)
        {
            //sqlStream.Write('[');
            sqlStream.SimpleWriteNotNull(column.SqlName);
            sqlStream.Write(' ');
            //sqlStream.Write(']');
            sqlStream.SimpleWriteNotNull(column.DbType.ToString());
            //if (isIdentity) sqlStream.Write(" identity(1,1)not");
            //else
            //{
                if (column.DbType.isStringType() && column.Size != int.MaxValue)
                {
                    sqlStream.Write('(');
                    sqlStream.SimpleWriteNotNull(column.Size == -1 ? maxString : column.Size.toString());
                    sqlStream.Write(')');
                }
                if (column.DefaultValue != null)
                {
                    sqlStream.SimpleWriteNotNull(" default ");
                    sqlStream.SimpleWriteNotNull(column.DefaultValue);
                }
                if (!column.IsNull) sqlStream.SimpleWriteNotNull(" not");
            //}
                sqlStream.SimpleWriteNotNull(" null");
        }
        /// <summary>
        /// 创建索引
        /// </summary>
        /// <param name="connection">SQL连接</param>
        /// <param name="tableName">表格名称</param>
        /// <param name="columnCollection">索引列集合</param>
        internal unsafe override bool createIndex(DbConnection connection, string tableName, columnCollection columnCollection)
        {
            string sql;
            pointer buffer = fastCSharp.sql.client.SqlBuffers.Get();
            try
            {
                using (charStream sqlStream = new charStream(buffer.Char, fastCSharp.sql.client.SqlBufferSize))
                {
                    sqlStream.WriteNotNull(@"
create index[");
                    AppendIndexName(sqlStream, tableName, columnCollection);
                    sqlStream.SimpleWriteNotNull("]on[");
                    sqlStream.SimpleWriteNotNull(this.Connection.Owner);
                    sqlStream.SimpleWriteNotNull("].[");
                    sqlStream.SimpleWriteNotNull(tableName);
                    sqlStream.SimpleWriteNotNull("](");
                    bool isNext = false;
                    foreach (column column in columnCollection.Columns)
                    {
                        if (isNext) sqlStream.Write(',');
                        //sqlStream.Write('[');
                        sqlStream.SimpleWriteNotNull(column.SqlName);
                        //sqlStream.Write(']');
                        isNext = true;
                    }
                    sqlStream.SimpleWriteNotNull(")on[primary]");
                    sql = sqlStream.ToString();
                }
            }
            finally { fastCSharp.sql.client.SqlBuffers.Push(ref buffer); }
            return executeNonQuery(connection, sql) != ExecuteNonQueryError;
        }
        /// <summary>
        /// 新增列集合
        /// </summary>
        /// <param name="connection">SQL连接</param>
        /// <param name="columnCollection">新增列集合</param>
        internal unsafe override bool addFields(DbConnection connection, columnCollection columnCollection)
        {
            string tableName = columnCollection.Name, sql;
            pointer buffer = fastCSharp.sql.client.SqlBuffers.Get();
            try
            {
                using (charStream sqlStream = new charStream(buffer.Char, fastCSharp.sql.client.SqlBufferSize))
                {
                    bool isUpdateValue = false;
                    foreach (column column in columnCollection.Columns)
                    {
                        sqlStream.WriteNotNull(@"
alter table [");
                        sqlStream.SimpleWriteNotNull(this.Connection.Owner);
                        sqlStream.SimpleWriteNotNull("].[");
                        sqlStream.SimpleWriteNotNull(tableName);
                        sqlStream.SimpleWriteNotNull(@"]add ");
                        if (!column.IsNull && column.DefaultValue == null)
                        {
                            column.DefaultValue = column.DbType.getDefaultValue();
                            if (column.DefaultValue == null) column.IsNull = true;
                        }
                        appendColumn(sqlStream, column);
                        if (column.UpdateValue != null) isUpdateValue = true;
                    }
                    if (isUpdateValue)
                    {
                        sqlStream.SimpleWriteNotNull(@"
update[");
                        sqlStream.SimpleWriteNotNull(tableName);
                        sqlStream.SimpleWriteNotNull("]set ");
                        foreach (column column in columnCollection.Columns)
                        {
                            if (column.UpdateValue != null)
                            {
                                if (!isUpdateValue) sqlStream.Write(',');
                                sqlStream.SimpleWriteNotNull(column.SqlName);
                                sqlStream.Write('=');
                                sqlStream.WriteNotNull(column.UpdateValue);
                                isUpdateValue = false;
                            }
                        }
                        sqlStream.SimpleWriteNotNull(" from[");
                        sqlStream.SimpleWriteNotNull(tableName);
                        sqlStream.SimpleWriteNotNull("]with(nolock)");
                    }
                    sql = sqlStream.ToString();
                }
            }
            finally { fastCSharp.sql.client.SqlBuffers.Push(ref buffer); }
            return executeNonQuery(connection, sql) != ExecuteNonQueryError;
        }
        /// <summary>
        /// 删除列集合
        /// </summary>
        /// <param name="connection">SQL连接</param>
        /// <param name="columnCollection">删除列集合</param>
        internal unsafe override bool deleteFields(DbConnection connection, columnCollection columnCollection)
        {
            string tableName = columnCollection.Name, sql;
            pointer buffer = fastCSharp.sql.client.SqlBuffers.Get();
            try
            {
                using (charStream sqlStream = new charStream(buffer.Char, fastCSharp.sql.client.SqlBufferSize))
                {
                    foreach (column column in columnCollection.Columns)
                    {
                        sqlStream.WriteNotNull(@"
alter table [");
                        sqlStream.SimpleWriteNotNull(this.Connection.Owner);
                        sqlStream.SimpleWriteNotNull("].[");
                        sqlStream.SimpleWriteNotNull(tableName);
                        sqlStream.SimpleWriteNotNull(@"]drop column ");
                        sqlStream.SimpleWriteNotNull(column.SqlName);
                    }
                    sql = sqlStream.ToString();
                }
            }
            finally { fastCSharp.sql.client.SqlBuffers.Push(ref buffer); }
            return executeNonQuery(connection, sql) != ExecuteNonQueryError;
        }
        /// <summary>
        /// 获取表格名称集合
        /// </summary>
        /// <param name="connection">SQL连接</param>
        /// <returns>表格名称集合</returns>
        protected override subArray<string> getTableNames(DbConnection connection)
        {
            subArray<string> value = new subArray<string>();
            using (DbCommand command = GetCommand(connection, GetTableNameSql))
            using (DbDataReader reader = command.ExecuteReader(CommandBehavior.Default))
            {
                while (reader.Read()) value.Add((string)reader[0]);
            }
            return value;
        }
        /// <summary>
        /// 获取表格名称的SQL语句
        /// </summary>
        protected virtual string GetTableNameSql
        {
            get
            {
                return "select name from sysobjects where(status&0xe0000000)=0x60000000 and objectproperty(id,'IsUserTable')=1";
            }
        }
        /// <summary>
        /// 根据表格名称获取表格信息
        /// </summary>
        /// <param name="connection">SQL连接</param>
        /// <param name="tableName">表格名称</param>
        /// <param name="isExecute">SQL连接执行是否成功</param>
        /// <returns>表格信息</returns>
        internal override table getTable(DbConnection connection, string tableName, ref byte isExecute)
        {
            using (DbCommand command = GetCommand(connection, GetTableSql(tableName)))
            using (DbDataReader reader = command.ExecuteReader(CommandBehavior.Default))
            {
                column identity = null;
                Dictionary<short, column> columns = dictionary.CreateShort<column>();
                while (reader.Read())
                {
                    SqlDbType type = sqlDbType.GetType((short)reader["xusertype"]);
                    int size = (int)(short)reader["length"];
                    if (type == SqlDbType.NChar || type == SqlDbType.NVarChar) size >>= 1;
                    else if (type == SqlDbType.Text || type == SqlDbType.NText) size = int.MaxValue;
                    column column = new column
                    {
                        Name = reader["name"].ToString(),
                        DbType = type,
                        Size = size,
                        DefaultValue = formatDefaultValue(reader["defaultValue"]),
                        Remark = reader["remark"].ToString(),
                        //GetColumnRemark(table, connection, name),
                        IsNull = (int)reader["isnullable"] == 1,
                    };
                    columns.Add((short)reader["colid"], column);
                    if ((int)reader["isidentity"] == 1) identity = column;
                }
                subArray<columnCollection> columnCollections = default(subArray<columnCollection>);
                if (reader.NextResult())
                {
                    short indexId = -1;
                    string indexName = null;
                    columnCollection.type columnType = columnCollection.type.Index;
                    subArray<short> columnId = default(subArray<short>);
                    while (reader.Read())
                    {
                        if (indexId != (short)reader["indid"])
                        {
                            if (indexId != -1)
                            {
                                column[] indexs = columnId.GetArray(columnIndex => columns[columnIndex]);
                                columnCollections.Add(new columnCollection
                                {
                                    Type = columnType,
                                    Name = indexName,
                                    Columns = indexs
                                });
                            }
                            columnId.Empty();
                            indexId = (short)reader["indid"];
                            indexName = reader["name"].ToString();
                            string type = reader["type"].ToString();
                            if (type == "PK") columnType = columnCollection.type.PrimaryKey;
                            else if (type == "UQ") columnType = columnCollection.type.UniqueIndex;
                            else columnType = columnCollection.type.Index;
                        }
                        columnId.Add((short)reader["colid"]);
                    }
                    if (indexId != -1)
                    {
                        columnCollections.Add(new columnCollection
                        {
                            Type = columnType,
                            Name = indexName,
                            Columns = columnId.GetArray(columnIndex => columns[columnIndex])
                        });
                    }
                }
                isExecute = 1;
                if (columns.Count != 0)
                {
                    columnCollection primaryKey = columnCollections.FirstOrDefault(columnCollection => columnCollection.Type == columnCollection.type.PrimaryKey);
                    return new table
                    {
                        Columns = new columnCollection
                        {
                            Name = tableName,
                            Columns = columns.Values.getArray(),
                            Type = sql.columnCollection.type.None
                        },
                        Identity = identity,
                        PrimaryKey = primaryKey,
                        Indexs = columnCollections.GetFindArray(columnCollection => columnCollection.Type != columnCollection.type.PrimaryKey)
                    };
                }
                return null;
            }
        }
        /// <summary>
        /// 根据表格名称获取表格信息的SQL语句
        /// </summary>
        /// <param name="tableName">表格名称</param>
        /// <returns>表格信息的SQL语句</returns>
        protected virtual string GetTableSql(string tableName)
        {
            return @"declare @id int
set @id=object_id(N'[dbo].[" + tableName + @"]')
if(select top 1 id from sysobjects where id=@id and objectproperty(id,N'IsUserTable')=1)is not null begin
 select columnproperty(id,name,'IsIdentity')as isidentity,id,xusertype,name,length,isnullable,colid,isnull((select top 1 text from syscomments where id=syscolumns.cdefault and colid=1),'')as defaultValue,isnull((select top 1 cast(value as varchar(256))from sysproperties where id=syscolumns.id and smallid=syscolumns.colid),'')as remark from syscolumns where id=@id order by colid
 if @@rowcount<>0 begin
  select a.indid,a.colid,b.name,(case when b.status=2 then 'UQ' else(select top 1 xtype from sysobjects where name=b.name)end)as type from sysindexkeys a left join sysindexes b on a.id=b.id and a.indid=b.indid where a.id=@id order by a.indid,a.keyno
 end
end";
        }
        /// <summary>
        /// 删除默认值左右括号()
        /// </summary>
        /// <param name="defaultValue">默认值</param>
        /// <returns>默认值</returns>
        protected static string formatDefaultValue(object defaultValue)
        {
            if (defaultValue != null)
            {
                string value = defaultValue.ToString();
                if (value.Length != 0)
                {
                    int valueIndex = 0, index = 0;
                    int[] valueIndexs = new int[value.Length];
                    for (int length = value.Length; index != length; ++index)
                    {
                        if (value[index] == '(') ++valueIndex;
                        else if (value[index] == ')') valueIndexs[--valueIndex] = index;
                    }
                    index = 0;
                    for (int length = value.Length - 1; valueIndexs[index] == length && value[index] == '('; --length) ++index;
                    value = value.Substring(index, value.Length - (index << 1));
                }
                return value;
            }
            return null;
        }
        ///// <summary>
        ///// like转义字符位图
        ///// </summary>
        //private static readonly String.asciiMap likeMap = new String.asciiMap(@"[]*_%", true);
        ///// <summary>
        ///// like转义
        ///// </summary>
        //private struct toLiker
        //{
        //    /// <summary>
        //    /// 源字符串
        //    /// </summary>
        //    public string Value;
        //    /// <summary>
        //    /// 字符串like转义
        //    /// </summary>
        //    /// <param name="map">转义索引位图</param>
        //    /// <returns>转义后的字符串</returns>
        //    public unsafe string Get(fixedMap map)
        //    {
        //        int count = 0;
        //        String.asciiMap likeMap = sql2000.likeMap;
        //        fixed (char* valueFixed = Value)
        //        {
        //            for (char* start = valueFixed, end = valueFixed + Value.Length; start != end; ++start)
        //            {
        //                if (likeMap.Get(*start))
        //                {
        //                    map.Set((int)(start - valueFixed));
        //                    ++count;
        //                }
        //            }
        //            if (count != 0)
        //            {
        //                string newValue = fastCSharp.String.FastAllocateString(Value.Length + (count << 1));
        //                fixed (char* newValueFixed = newValue)
        //                {
        //                    char* write = newValueFixed, read = valueFixed;
        //                    for (int index = 0; index != Value.Length; ++index)
        //                    {
        //                        if (map.Get(index))
        //                        {
        //                            *write++ = '[';
        //                            *write++ = *read++;
        //                            *write++ = ']';
        //                        }
        //                        else *write++ = *read++;
        //                    }
        //                }
        //                return newValue;
        //            }
        //        }
        //        return Value;
        //    }
        //}
        ///// <summary>
        ///// 字符串like转义
        ///// </summary>
        ///// <param name="value">字符串</param>
        ///// <returns>转义后的字符串</returns>
        //public unsafe string ToLike(string value)
        //{
        //    return value.length() != 0 ? fixedMap.GetMap<string>(value.Length, new toLiker { Value = value }.Get) : value;
        //}
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        internal sealed unsafe class importer<valueType, modelType> : inserter
            where valueType : class, modelType
            where modelType : class
        {
            /// <summary>
            /// SQL客户端操作
            /// </summary>
            private client client;
            /// <summary>
            /// SQL操作工具
            /// </summary>
            private fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool;
            /// <summary>
            /// 目标对象
            /// </summary>
            private subArray<valueType> values;
            /// <summary>
            /// 获取数据回调
            /// </summary>
            private Action<subArray<valueType>> onInserted;
            /// <summary>
            /// 是否事物处理
            /// </summary>
            private bool isTransaction;
            /// <summary>
            /// 导入数据
            /// </summary>
            /// <param name="client">SQL客户端操作</param>
            /// <param name="sqlTool">SQL操作工具</param>
            /// <param name="values">目标对象</param>
            /// <param name="onInserted">删除数据回调</param>
            /// <param name="isTransaction"></param>
            public void Insert(client client, fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, ref subArray<valueType> values, ref Action<subArray<valueType>> onInserted, bool isTransaction)
            {
                this.onInserted = onInserted;
                this.client = client;
                this.sqlTool = sqlTool;
                this.values = values;
                this.isTransaction = isTransaction;
                onInserted = null;
                fastCSharp.threading.task.Tiny.Add(this, threading.thread.callType.SqlClientInserter);
            }
            /// <summary>
            /// 导入数据
            /// </summary>
            internal override void Insert()
            {
                DbConnection connection = client.GetConnection(false);
                if (connection != null)
                {
                    try
                    {
                        sqlTool.SetIdentity(ref values);
                        if (client.Import(connection, sqlTool.GetDataTable(ref values), 0, 0) != 0)
                        {
                            client.ConnectionPool.Push(ref connection);
                        }
                        else values.Null();
                    }
                    catch (Exception error)
                    {
                        values.Null();
                        fastCSharp.log.Error.Add(error, sqlTool.TableName, false);
                    }
                    if (connection != null) connection.Dispose();
                }
                else values.Null();
                push();
            }
            /// <summary>
            /// 连接池处理
            /// </summary>
            private void push()
            {
                Action<subArray<valueType>> onInserted = this.onInserted;
                subArray<valueType> values = this.values;
                fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool = this.sqlTool;
                bool isTransaction = this.isTransaction;
                try
                {
                    client = null;
                    this.sqlTool = null;
                    this.onInserted = null;
                    this.values.Null();
                    typePool<importer<valueType, modelType>>.PushNotNull(this);
                }
                finally
                {
                    if (sqlTool.IsLockWrite)
                    {
                        Monitor.Enter(sqlTool.Lock);
                        try
                        {
                            foreach (valueType value in values) sqlTool.CallOnInsertedLock(value);
                        }
                        catch (Exception error)
                        {
                            log.Default.Add(error, null, false);
                        }
                        finally { Monitor.Exit(sqlTool.Lock); }
                    }
                    if (isTransaction) fastCSharp.domainUnload.TransactionEnd();
                    try
                    {
                        foreach (valueType value in values) sqlTool.CallOnInserted(value);
                    }
                    finally { onInserted(values); }
                }
            }
            /// <summary>
            /// 获取数据
            /// </summary>
            /// <returns></returns>
            public static importer<valueType, modelType> Get()
            {
                importer<valueType, modelType> inserter = typePool<importer<valueType, modelType>>.Pop();
                if (inserter == null)
                {
                    try
                    {
                        inserter = new importer<valueType, modelType>();
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, null, false);
                    }
                }
                return inserter;
            }
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="values">待插入数据</param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
        /// <param name="onInserted"></param>
        public override void Insert<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, subArray<valueType> values, bool isIgnoreTransaction, Action<subArray<valueType>> onInserted)
        {
            if (onInserted == null) log.Error.Throw(log.exceptionType.Null);
            if (values.length == 0) onInserted(values);
            else
            {
                try
                {
                    if (values.GetCount(value => sqlModel<modelType>.verify.Verify(value, fastCSharp.code.memberMap<modelType>.Default, sqlTool)) == values.length && sqlTool.CallOnInsert(ref values))
                    {
                        importer<valueType, modelType> inserter;
                        bool isTransaction = false;
                        if (!isIgnoreTransaction && sqlTool.IsDeleteTransaction)
                        {
                            if (fastCSharp.domainUnload.TransactionStart(false))
                            {
                                isTransaction = true;
                                inserter = importer<valueType, modelType>.Get();
                            }
                            else inserter = null;
                        }
                        else inserter = importer<valueType, modelType>.Get();
                        if (inserter != null) inserter.Insert(this, sqlTool, ref values, ref onInserted, isTransaction);
                        else if (isTransaction) fastCSharp.domainUnload.TransactionEnd();
                    }
                }
                finally
                {
                    if (onInserted != null) onInserted(default(subArray<valueType>));
                }
            }
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待插入数据</param>
        /// <param name="memberMap">目标成员位图</param>
        /// <param name="sqlStream"></param>
        /// <returns>是否成功</returns>
        private unsafe string insert<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap, charStream sqlStream)
            where valueType : class, modelType
            where modelType : class
        {
            pointer buffer = fastCSharp.sql.client.SqlBuffers.Get();
            try
            {
                using (sqlStream)
                {
                    sqlStream.UnsafeReset(buffer.Byte, SqlBufferSize << 1);
                    if (sqlModel<modelType>.Identity != null)
                    {
                        long identity;
                        if (sqlTool.IsSetIdentity) sqlModel<modelType>.SetIdentity(value, identity = sqlTool.NextIdentity);
                        else sqlTool.Identity64 = identity = sqlModel<modelType>.GetIdentity(value);
                        sqlStream.SimpleWriteNotNull("insert into[");
                        sqlStream.SimpleWriteNotNull(sqlTool.TableName);
                        sqlStream.SimpleWriteNotNull("](");
                        sqlModel<modelType>.insert.GetColumnNames(sqlStream, memberMap);
                        sqlStream.SimpleWriteNotNull(")values(");
                        sqlModel<modelType>.insert.Insert(sqlStream, memberMap, value, Converter);
                        sqlStream.WriteNotNull(@")
if @@ROWCOUNT<>0 begin
 select top 1 ");
                        sqlModel<modelType>.GetNames(sqlStream, fastCSharp.emit.sqlModel<modelType>.MemberMap);
                        sqlStream.SimpleWriteNotNull(" from[");
                        sqlStream.SimpleWriteNotNull(sqlTool.TableName);
                        sqlStream.WriteNotNull("]with(nolock)where ");
                        sqlStream.SimpleWriteNotNull(sqlModel<modelType>.Identity.SqlFieldName);
                        sqlStream.Write('=');
                        fastCSharp.number.ToString(identity, sqlStream);
                        sqlStream.SimpleWriteNotNull(@"
end");
                    }
                    else
                    {
                        sqlStream.SimpleWriteNotNull("insert into[");
                        sqlStream.SimpleWriteNotNull(sqlTool.TableName);
                        sqlStream.SimpleWriteNotNull("](");
                        sqlModel<modelType>.insert.GetColumnNames(sqlStream, memberMap);
                        sqlStream.SimpleWriteNotNull(")values(");
                        sqlModel<modelType>.insert.Insert(sqlStream, memberMap, value, Converter);
                        if (sqlModel<modelType>.PrimaryKeys.Length != 0)
                        {
                            sqlStream.WriteNotNull(@")
if @@ROWCOUNT<>0 begin
 select top 1 ");
                            sqlModel<modelType>.GetNames(sqlStream, fastCSharp.emit.sqlModel<modelType>.MemberMap);
                            sqlStream.SimpleWriteNotNull(" from[");
                            sqlStream.SimpleWriteNotNull(sqlTool.TableName);
                            sqlStream.WriteNotNull("]with(nolock)where ");
                            sqlModel<modelType>.primaryKeyWhere.Where(sqlStream, value, Converter);
                            sqlStream.SimpleWriteNotNull(@"
end");
                        }
                        else sqlStream.Write(')');
                    }
                    return sqlStream.ToString();
                }
            }
            finally { fastCSharp.sql.client.SqlBuffers.Push(ref buffer); }
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待插入数据</param>
        /// <param name="memberMap">目标成员位图</param>
        /// <returns>是否成功</returns>
        protected unsafe override bool insert<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap)
        {
            bool isInsert = false;
            using (charStream sqlStream = new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1))
            {
                if (sqlModel<modelType>.Identity != null)
                {
                    if (set<valueType, modelType>(insert(sqlTool, value, memberMap, sqlStream), value, fastCSharp.emit.sqlModel<modelType>.MemberMap))
                    {
                        isInsert = true;
                    }
                }
                else
                {
                    if (sqlModel<modelType>.PrimaryKeys.Length != 0)
                    {
                        if (set<valueType, modelType>(insert(sqlTool, value, memberMap, sqlStream), value, fastCSharp.emit.sqlModel<modelType>.MemberMap))
                        {
                            isInsert = true;
                        }
                    }
                    else if (ExecuteNonQuery(insert(sqlTool, value, memberMap, sqlStream)) > 0) isInsert = true;
                }
            }
            if (isInsert)
            {
                if (sqlTool.IsLockWrite) sqlTool.CallOnInsertedLock(value);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        private sealed unsafe class inserter<valueType, modelType> : IDisposable
            where valueType : class, modelType
            where modelType : class
        {
            /// <summary>
            /// SQL字符流
            /// </summary>
            internal charStream SqlStream = new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1);
            /// <summary>
            /// SQL客户端操作
            /// </summary>
            private client client;
            /// <summary>
            /// SQL操作工具
            /// </summary>
            private fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool;
            /// <summary>
            /// SQL语句
            /// </summary>
            private string sql;
            /// <summary>
            /// 目标对象
            /// </summary>
            private valueType value;
            /// <summary>
            /// 获取数据回调
            /// </summary>
            private Action<valueType> onInserted;
            /// <summary>
            /// SQL连接
            /// </summary>
            private DbConnection connection;
            /// <summary>
            /// SQL命令
            /// </summary>
            private SqlCommand command;
            /// <summary>
            /// 获取数据回调
            /// </summary>
            private AsyncCallback callback;
            /// <summary>
            /// 是否事物处理
            /// </summary>
            private bool isTransaction;
            /// <summary>
            /// SQL连接执行是否成功
            /// </summary>
            private byte isExecute;
            /// <summary>
            /// 获取数据
            /// </summary>
            private inserter()
            {
                callback = onInsert;
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                pub.Dispose(ref SqlStream);
            }
            /// <summary>
            /// 删除数据
            /// </summary>
            /// <param name="client">SQL客户端操作</param>
            /// <param name="sqlTool">SQL操作工具</param>
            /// <param name="sql">SQL语句</param>
            /// <param name="value">目标对象</param>
            /// <param name="onInserted">删除数据回调</param>
            /// <param name="isTransaction"></param>
            public void Insert(client client, fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, string sql, valueType value, ref Action<valueType> onInserted, bool isTransaction)
            {
                this.onInserted = onInserted;
                this.client = client;
                this.sqlTool = sqlTool;
                this.sql = sql;
                this.value = value;
                this.isTransaction = isTransaction;
                onInserted = null;
                try
                {
                    if ((connection = client.GetConnection(true)) != null)
                    {
                        (command = (SqlCommand)client.GetCommand(connection, sql)).BeginExecuteReader(callback, this, CommandBehavior.SingleResult);
                        return;
                    }
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, sql, false);
                }
                this.value = null;
                push();
            }
            /// <summary>
            /// 获取数据回调
            /// </summary>
            /// <param name="result"></param>
            private void onInsert(IAsyncResult result)
            {
                try
                {
                    using (SqlDataReader reader = command.EndExecuteReader(result))
                    {
                        if (reader.Read())
                        {
                            isExecute = 1;
                            sqlModel<modelType>.set.Set(reader, value, fastCSharp.emit.sqlModel<modelType>.MemberMap);
                        }
                        else
                        {
                            isExecute = 1;
                            value = null;
                        }
                    }
                }
                catch (Exception error)
                {
                    value = null;
                    fastCSharp.log.Error.Add(error, sql, false);
                }
                push();
            }
            /// <summary>
            /// 连接池处理
            /// </summary>
            private void push()
            {
                Action<valueType> onInserted = this.onInserted;
                valueType value = this.value;
                fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool = this.sqlTool;
                bool isTransaction = this.isTransaction;
                try
                {
                    if (command != null) command.Dispose();
                    if (isExecute == 0)
                    {
                        if (connection != null) connection.Dispose();
                    }
                    else client.AsynchronousConnectionPool.Push(ref connection);
                    client = null;
                    connection = null;
                    command = null;
                    sql = null;
                    this.sqlTool = null;
                    this.onInserted = null;
                    this.value = null;
                    this.isExecute = 0;
                    typePool<inserter<valueType, modelType>>.PushNotNull(this);
                }
                finally
                {
                    if (sqlTool.IsLockWrite)
                    {
                        Monitor.Enter(sqlTool.Lock);
                        try
                        {
                            sqlTool.CallOnInsertedLock(value);
                        }
                        catch (Exception error)
                        {
                            log.Default.Add(error, null, false);
                        }
                        finally { Monitor.Exit(sqlTool.Lock); }
                    }
                    if (isTransaction) fastCSharp.domainUnload.TransactionEnd();
                    try
                    {
                        sqlTool.CallOnInserted(value);
                    }
                    finally { onInserted(value); }
                }
            }
            /// <summary>
            /// 获取数据
            /// </summary>
            /// <returns></returns>
            public static inserter<valueType, modelType> Get()
            {
                inserter<valueType, modelType> inserter = typePool<inserter<valueType, modelType>>.Pop();
                if (inserter == null)
                {
                    try
                    {
                        inserter = new inserter<valueType, modelType>();
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, null, false);
                    }
                }
                return inserter;
            }
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待插入数据</param>
        /// <param name="memberMap">目标成员位图</param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
        /// <param name="onInserted"></param>
        protected override void insert<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap, bool isIgnoreTransaction, ref Action<valueType> onInserted)
        {
            inserter<valueType, modelType> inserter;
            bool isTransaction = false;
            if (!isIgnoreTransaction && sqlTool.IsDeleteTransaction)
            {
                if (fastCSharp.domainUnload.TransactionStart(false))
                {
                    isTransaction = true;
                    inserter = inserter<valueType, modelType>.Get();
                }
                else inserter = null;
            }
            else inserter = inserter<valueType, modelType>.Get();
            if (inserter != null) inserter.Insert(this, sqlTool, insert(sqlTool, value, memberMap, inserter.SqlStream), value, ref onInserted, isTransaction);
            else if (isTransaction) fastCSharp.domainUnload.TransactionEnd();
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待更新数据</param>
        /// <param name="selectMemberMap">目标成员位图</param>
        /// <param name="updateMemberMap">目标成员位图</param>
        /// <param name="sqlStream"></param>
        /// <returns>SQL语句</returns>
        private unsafe string update<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap updateMemberMap, fastCSharp.code.memberMap selectMemberMap, charStream sqlStream)
            where valueType : class, modelType
            where modelType : class
        {
            pointer buffer = fastCSharp.sql.client.SqlBuffers.Get();
            try
            {
                using (sqlStream)
                {
                    sqlStream.UnsafeReset(buffer.Byte, SqlBufferSize << 1);
                    sqlStream.SimpleWriteNotNull("select top 1 ");
                    sqlModel<modelType>.insert.GetColumnNames(sqlStream, selectMemberMap);
                    sqlStream.SimpleWriteNotNull(" from[");
                    sqlStream.SimpleWriteNotNull(sqlTool.TableName);
                    sqlStream.WriteNotNull("]with(nolock)where ");
                    sqlModel<modelType>.primaryKeyWhere.Where(sqlStream, value, Converter);
                    string sql = sqlStream.ToString();
                    sqlStream.WriteNotNull(@"
if @@ROWCOUNT<>0 begin
 update[");
                    sqlStream.SimpleWriteNotNull(sqlTool.TableName);
                    sqlStream.SimpleWriteNotNull("]set ");
                    sqlModel<modelType>.update.Update(sqlStream, updateMemberMap, value, Converter);
                    sqlStream.SimpleWriteNotNull(" from[");
                    sqlStream.SimpleWriteNotNull(sqlTool.TableName);
                    sqlStream.WriteNotNull("]with(nolock)where ");
                    sqlModel<modelType>.primaryKeyWhere.Where(sqlStream, value, Converter);
                    sqlStream.SimpleWriteNotNull(@"
 ");
                    sqlStream.WriteNotNull(sql);
                    sqlStream.SimpleWriteNotNull(@"
end");
                    return sqlStream.ToString();
                }
            }
            finally { fastCSharp.sql.client.SqlBuffers.Push(ref buffer); }
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待更新数据</param>
        /// <param name="memberMap">目标成员位图</param>
        /// <returns>更新前的数据对象,null表示失败</returns>
        protected override unsafe valueType update<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap)
        {
            using (fastCSharp.code.memberMap<modelType> selectMemberMap = sqlTool.GetSelectMemberMap(memberMap))
            {
                string sql = update(sqlTool, value, memberMap, selectMemberMap, new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1));
                valueType oldValue = fastCSharp.emit.constructor<valueType>.New();
                if (set<valueType, modelType>(sql, value, oldValue, selectMemberMap))
                {
                    sqlTool.CallOnUpdatedLock(value, oldValue, memberMap);
                    return oldValue;
                }
            }
            return null;
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待更新数据自增id标识</param>
        /// <param name="sqlExpression">待更新数据</param>
        /// <param name="memberMap">目标成员位图</param>
        /// <param name="sqlStream"></param>
        /// <returns>SQL语句</returns>
        private unsafe string update<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool
            , valueType value, ref fastCSharp.emit.sqlTable.updateExpression sqlExpression, fastCSharp.code.memberMap<modelType> memberMap, charStream sqlStream)
            where valueType : class, modelType
            where modelType : class
        {
            pointer buffer = fastCSharp.sql.client.SqlBuffers.Get();
            try
            {
                using (sqlStream)
                {
                    sqlStream.UnsafeReset(buffer.Byte, SqlBufferSize << 1);
                    sqlStream.SimpleWriteNotNull("select top 1 ");
                    sqlModel<modelType>.GetNames(sqlStream, memberMap);
                    sqlStream.SimpleWriteNotNull(" from[");
                    sqlStream.SimpleWriteNotNull(sqlTool.TableName);
                    sqlStream.WriteNotNull("]with(nolock)where ");
                    sqlModel<modelType>.primaryKeyWhere.Where(sqlStream, value, Converter);
                    string sql = sqlStream.ToString();
                    sqlStream.WriteNotNull(@"
if @@ROWCOUNT<>0 begin
 update[");
                    sqlStream.SimpleWriteNotNull(sqlTool.TableName);
                    sqlStream.SimpleWriteNotNull("]set ");
                    sqlExpression.Update(sqlStream);
                    sqlStream.SimpleWriteNotNull(" from[");
                    sqlStream.SimpleWriteNotNull(sqlTool.TableName);
                    sqlStream.WriteNotNull("]with(nolock)where ");
                    sqlModel<modelType>.primaryKeyWhere.Where(sqlStream, value, Converter);
                    sqlStream.SimpleWriteNotNull(@"
 ");
                    sqlStream.WriteNotNull(sql);
                    sqlStream.SimpleWriteNotNull(@"
end");
                    return sqlStream.ToString();
                }
            }
            finally { fastCSharp.sql.client.SqlBuffers.Push(ref buffer); }
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待更新数据自增id标识</param>
        /// <param name="sqlExpression">待更新数据</param>
        /// <param name="memberMap">目标成员位图</param>
        /// <returns>更新前的数据对象,null表示失败</returns>
        protected override unsafe valueType update<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool
            , valueType value, ref fastCSharp.emit.sqlTable.updateExpression sqlExpression, fastCSharp.code.memberMap<modelType> memberMap)
        {
            using (fastCSharp.code.memberMap<modelType> selectMemberMap = sqlTool.GetSelectMemberMap(memberMap))
            {
                string sql = update(sqlTool, value, ref sqlExpression, selectMemberMap, new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1));
                valueType oldValue = fastCSharp.emit.constructor<valueType>.New();
                if (set<valueType, modelType>(sql, value, oldValue, selectMemberMap))
                {
                    sqlTool.CallOnUpdatedLock(value, oldValue, memberMap);
                    return oldValue;
                }
            }
            return null;
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待更新数据</param>
        /// <param name="memberMap">目标成员位图</param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
        /// <param name="onUpdated">是否成功</param>
        public override void Update<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap, bool isIgnoreTransaction, Action<valueType, valueType, fastCSharp.code.memberMap> onUpdated)
        {
            if (onUpdated == null) log.Error.Throw(log.exceptionType.Null);
            updater<valueType, modelType> updater;
            bool isTransaction = false;
            try
            {
                if (!isIgnoreTransaction && sqlTool.IsDeleteTransaction)
                {
                    if (fastCSharp.domainUnload.TransactionStart(false))
                    {
                        isTransaction = true;
                        updater = updater<valueType, modelType>.Get();
                    }
                    else updater = null;
                }
                else updater = updater<valueType, modelType>.Get();
                if (updater != null)
                {
                    fastCSharp.code.memberMap<modelType> updateMemberMap = sqlTool.GetMemberMapClearIdentity(memberMap);
                    if (sqlModel<modelType>.verify.Verify(value, updateMemberMap, sqlTool) && sqlTool.CallOnUpdate(value, updateMemberMap))
                    {
                        sqlTool.GetSelectMemberMap(updateMemberMap, updater.MemberMap);
                        updater.Update(this, sqlTool, update(sqlTool, value, updateMemberMap, updater.MemberMap, updater.SqlStream), value, updateMemberMap, ref onUpdated, ref isTransaction);
                    }
                }
            }
            finally
            {
                if (isTransaction) fastCSharp.domainUnload.TransactionEnd();
                if (onUpdated != null) onUpdated(null, null, null);
            }
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待更新数据自增id标识</param>
        /// <param name="updateExpression">待更新数据</param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
        /// <param name="onUpdated"></param>
        public override void Update<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value
            , ref fastCSharp.emit.sqlTable.updateExpression updateExpression, bool isIgnoreTransaction, Action<valueType, valueType, fastCSharp.code.memberMap> onUpdated)
        {
            if (onUpdated == null) log.Error.Throw(log.exceptionType.Null);
            updater<valueType, modelType> updater;
            bool isTransaction = false;
            try
            {
                if (!isIgnoreTransaction && sqlTool.IsDeleteTransaction)
                {
                    if (fastCSharp.domainUnload.TransactionStart(false))
                    {
                        isTransaction = true;
                        updater = updater<valueType, modelType>.Get();
                    }
                    else updater = null;
                }
                else updater = updater<valueType, modelType>.Get();
                if (updater != null)
                {
                    fastCSharp.code.memberMap<modelType> updateMemberMap = updateExpression.CreateMemberMap<modelType>();
                    if (!updateMemberMap.IsDefault)
                    {
                        updateMemberMap.ClearMember(sqlModel<modelType>.Identity.MemberMapIndex);
                        if (sqlTool.CallOnUpdate(value, updateMemberMap))
                        {
                            sqlTool.GetSelectMemberMap(updateMemberMap, updater.MemberMap);
                            updater.Update(this, sqlTool, update(sqlTool, value, ref updateExpression, updater.MemberMap, updater.SqlStream), value, updateMemberMap, ref onUpdated, ref isTransaction);
                        }
                    }
                }
            }
            finally
            {
                if (isTransaction) fastCSharp.domainUnload.TransactionEnd();
                if (onUpdated != null) onUpdated(null, null, null);
            }
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待更新数据</param>
        /// <param name="updateMemberMap">目标成员位图</param>
        /// <param name="selectMemberMap">目标成员位图</param>
        /// <param name="sqlStream"></param>
        /// <returns>SQL语句</returns>
        private unsafe string updateByIdentity<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap updateMemberMap, fastCSharp.code.memberMap selectMemberMap, charStream sqlStream)
            where valueType : class, modelType
            where modelType : class
        {
            pointer buffer = fastCSharp.sql.client.SqlBuffers.Get();
            try
            {
                using (sqlStream)
                {
                    sqlStream.UnsafeReset(buffer.Byte, SqlBufferSize << 1);
                    sqlStream.SimpleWriteNotNull("select top 1 ");
                    sqlModel<modelType>.insert.GetColumnNames(sqlStream, selectMemberMap);
                    sqlStream.SimpleWriteNotNull(" from[");
                    sqlStream.SimpleWriteNotNull(sqlTool.TableName);
                    sqlStream.WriteNotNull("]with(nolock)where ");
                    sqlStream.SimpleWriteNotNull(sqlModel<modelType>.Identity.SqlFieldName);
                    sqlStream.Write('=');
                    fastCSharp.number.ToString(sqlModel<modelType>.GetIdentity(value), sqlStream);
                    string sql = sqlStream.ToString();
                    sqlStream.WriteNotNull(@"
if @@ROWCOUNT<>0 begin
 update[");
                    sqlStream.SimpleWriteNotNull(sqlTool.TableName);
                    sqlStream.SimpleWriteNotNull("]set ");
                    sqlModel<modelType>.update.Update(sqlStream, updateMemberMap, value, Converter);
                    sqlStream.SimpleWriteNotNull(" from[");
                    sqlStream.SimpleWriteNotNull(sqlTool.TableName);
                    sqlStream.WriteNotNull("]with(nolock)where ");
                    sqlStream.WriteNotNull(sqlModel<modelType>.Identity.SqlFieldName);
                    sqlStream.Write('=');
                    fastCSharp.number.ToString(sqlModel<modelType>.GetIdentity(value), sqlStream);
                    sqlStream.SimpleWriteNotNull(@"
 ");
                    sqlStream.WriteNotNull(sql);
                    sqlStream.SimpleWriteNotNull(@"
end");
                    return sqlStream.ToString();
                }
            }
            finally { fastCSharp.sql.client.SqlBuffers.Push(ref buffer); }
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待更新数据</param>
        /// <param name="memberMap">目标成员位图</param>
        /// <returns>更新前的数据对象,null表示失败</returns>
        protected override unsafe valueType updateByIdentity<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap)
        {
            using (fastCSharp.code.memberMap<modelType> selectMemberMap = sqlTool.GetSelectMemberMap(memberMap))
            {
                string sql = updateByIdentity(sqlTool, value, memberMap, selectMemberMap, new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1));
                valueType oldValue = fastCSharp.emit.constructor<valueType>.New();
                if (set<valueType, modelType>(sql, value, oldValue, selectMemberMap))
                {
                    sqlTool.CallOnUpdatedLock(value, oldValue, memberMap);
                    return oldValue;
                }
            }
            return null;
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待更新数据自增id标识</param>
        /// <param name="sqlExpression">待更新数据</param>
        /// <param name="memberMap">目标成员位图</param>
        /// <param name="sqlStream"></param>
        /// <returns>SQL语句</returns>
        private unsafe string updateByIdentity<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool
            , valueType value, ref fastCSharp.emit.sqlTable.updateExpression sqlExpression, fastCSharp.code.memberMap<modelType> memberMap, charStream sqlStream)
            where valueType : class, modelType
            where modelType : class
        {
            pointer buffer = fastCSharp.sql.client.SqlBuffers.Get();
            try
            {
                using (sqlStream)
                {
                    sqlStream.UnsafeReset(buffer.Byte, SqlBufferSize << 1);
                    sqlStream.SimpleWriteNotNull("select top 1 ");
                    sqlModel<modelType>.GetNames(sqlStream, memberMap);
                    sqlStream.SimpleWriteNotNull(" from[");
                    sqlStream.SimpleWriteNotNull(sqlTool.TableName);
                    sqlStream.WriteNotNull("]with(nolock)where ");
                    sqlStream.SimpleWriteNotNull(sqlModel<modelType>.Identity.SqlFieldName);
                    sqlStream.Write('=');
                    fastCSharp.number.ToString(sqlModel<modelType>.GetIdentity(value), sqlStream);
                    string sql = sqlStream.ToString();
                    sqlStream.WriteNotNull(@"
if @@ROWCOUNT<>0 begin
 update[");
                    sqlStream.SimpleWriteNotNull(sqlTool.TableName);
                    sqlStream.SimpleWriteNotNull("]set ");
                    sqlExpression.Update(sqlStream);
                    sqlStream.SimpleWriteNotNull(" from[");
                    sqlStream.SimpleWriteNotNull(sqlTool.TableName);
                    sqlStream.WriteNotNull("]with(nolock)where ");
                    sqlStream.SimpleWriteNotNull(sqlModel<modelType>.Identity.SqlFieldName);
                    sqlStream.Write('=');
                    fastCSharp.number.ToString(sqlModel<modelType>.GetIdentity(value), sqlStream);
                    sqlStream.SimpleWriteNotNull(@"
 ");
                    sqlStream.WriteNotNull(sql);
                    sqlStream.SimpleWriteNotNull(@"
end");
                    return sqlStream.ToString();
                }
            }
            finally { fastCSharp.sql.client.SqlBuffers.Push(ref buffer); }
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待更新数据自增id标识</param>
        /// <param name="sqlExpression">待更新数据</param>
        /// <param name="memberMap">目标成员位图</param>
        /// <returns>更新前的数据对象,null表示失败</returns>
        protected override unsafe valueType updateByIdentity<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool
            , valueType value, ref fastCSharp.emit.sqlTable.updateExpression sqlExpression, fastCSharp.code.memberMap<modelType> memberMap)
        {
            using (fastCSharp.code.memberMap<modelType> selectMemberMap = sqlTool.GetSelectMemberMap(memberMap))
            {
                string sql = updateByIdentity(sqlTool, value, ref sqlExpression, selectMemberMap, new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1));
                valueType oldValue = fastCSharp.emit.constructor<valueType>.New();
                if (set<valueType, modelType>(sql, value, oldValue, selectMemberMap))
                {
                    sqlTool.CallOnUpdatedLock(value, oldValue, memberMap);
                    return oldValue;
                }
            }
            return null;
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        private sealed unsafe class updater<valueType, modelType> : IDisposable
            where valueType : class, modelType
            where modelType : class
        {
            /// <summary>
            /// SQL字符流
            /// </summary>
            internal charStream SqlStream = new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1);
            /// <summary>
            /// 目标成员位图
            /// </summary>
            internal fastCSharp.code.memberMap<modelType> MemberMap = fastCSharp.code.memberMap<modelType>.New();
            /// <summary>
            /// 更新数据成员
            /// </summary>
            private fastCSharp.code.memberMap<modelType> updateMemberMap;
            /// <summary>
            /// SQL客户端操作
            /// </summary>
            private client client;
            /// <summary>
            /// SQL操作工具
            /// </summary>
            private fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool;
            /// <summary>
            /// SQL语句
            /// </summary>
            private string sql;
            /// <summary>
            /// 目标对象
            /// </summary>
            private valueType value;
            /// <summary>
            /// 获取数据回调
            /// </summary>
            private Action<valueType, valueType, fastCSharp.code.memberMap> onUpdated;
            /// <summary>
            /// SQL连接
            /// </summary>
            private DbConnection connection;
            /// <summary>
            /// SQL命令
            /// </summary>
            private SqlCommand command;
            /// <summary>
            /// 获取数据回调
            /// </summary>
            private AsyncCallback callback;
            /// <summary>
            /// 是否事物处理
            /// </summary>
            private bool isTransaction;
            /// <summary>
            /// SQL连接执行是否成功
            /// </summary>
            private byte isExecute;
            /// <summary>
            /// 获取数据
            /// </summary>
            private updater()
            {
                callback = onUpdate;
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                pub.Dispose(ref MemberMap);
                pub.Dispose(ref SqlStream);
            }
            /// <summary>
            /// 删除数据
            /// </summary>
            /// <param name="client">SQL客户端操作</param>
            /// <param name="sqlTool">SQL操作工具</param>
            /// <param name="sql">SQL语句</param>
            /// <param name="value">目标对象</param>
            /// <param name="updateMemberMap"></param>
            /// <param name="onUpdated">删除数据回调</param>
            /// <param name="isTransaction"></param>
            public void Update(client client, fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, string sql, valueType value, fastCSharp.code.memberMap<modelType> updateMemberMap, ref Action<valueType, valueType, fastCSharp.code.memberMap> onUpdated, ref bool isTransaction)
            {
                this.onUpdated = onUpdated;
                this.client = client;
                this.sqlTool = sqlTool;
                this.sql = sql;
                this.value = value;
                this.updateMemberMap = updateMemberMap;
                this.isTransaction = isTransaction;
                onUpdated = null;
                isTransaction = false;
                try
                {
                    if ((connection = client.GetConnection(true)) != null)
                    {
                        (command = (SqlCommand)client.GetCommand(connection, sql)).BeginExecuteReader(callback, this, CommandBehavior.Default);
                        return;
                    }
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, sql, false);
                }
                this.value = null;
                push(null);
            }
            /// <summary>
            /// 获取数据回调
            /// </summary>
            /// <param name="result"></param>
            private void onUpdate(IAsyncResult result)
            {
                valueType oldValue = null;
                try
                {
                    using (SqlDataReader reader = command.EndExecuteReader(result))
                    {
                        if (reader.Read())
                        {
                            sqlModel<modelType>.set.Set(reader, oldValue = fastCSharp.emit.constructor<valueType>.New(), MemberMap);
                            if (reader.NextResult() && reader.Read())
                            {
                                isExecute = 1;
                                sqlModel<modelType>.set.Set(reader, value, MemberMap);
                            }
                        }
                        else
                        {
                            isExecute = 1;
                            value = null;
                        }
                    }
                }
                catch (Exception error)
                {
                    value = null;
                    fastCSharp.log.Error.Add(error, sql, false);
                }
                push(oldValue);
            }
            /// <summary>
            /// 连接池处理
            /// </summary>
            private void push(valueType oldValue)
            {
                Action<valueType, valueType, fastCSharp.code.memberMap> onUpdated = this.onUpdated;
                valueType value = this.value;
                fastCSharp.code.memberMap<modelType> updateMemberMap = this.updateMemberMap;
                fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool = this.sqlTool;
                bool isTransaction = this.isTransaction;
                try
                {
                    if (command != null) command.Dispose();
                    if (isExecute == 0)
                    {
                        if (connection != null) connection.Dispose();
                    }
                    else client.AsynchronousConnectionPool.Push(ref connection);
                    client = null;
                    connection = null;
                    command = null;
                    sql = null;
                    this.sqlTool = null;
                    this.onUpdated = null;
                    this.value = null;
                    this.updateMemberMap = null;
                    this.isExecute = 0;
                    typePool<updater<valueType, modelType>>.PushNotNull(this);
                }
                finally
                {
                    using (updateMemberMap)
                    {
                        if (sqlTool.IsLockWrite)
                        {
                            Monitor.Enter(sqlTool.Lock);
                            try
                            {
                                sqlTool.CallOnUpdatedLock(value, oldValue, updateMemberMap);
                            }
                            catch (Exception error)
                            {
                                log.Default.Add(error, null, false);
                            }
                            finally { Monitor.Exit(sqlTool.Lock); }
                        }
                        if (isTransaction) fastCSharp.domainUnload.TransactionEnd();
                        try
                        {
                            sqlTool.CallOnUpdated(value, oldValue, updateMemberMap);
                        }
                        finally { onUpdated(value, oldValue, updateMemberMap); }
                    }
                }
            }
            /// <summary>
            /// 获取数据
            /// </summary>
            /// <returns></returns>
            public static updater<valueType, modelType> Get()
            {
                updater<valueType, modelType> updater = typePool<updater<valueType, modelType>>.Pop();
                if (updater == null)
                {
                    try
                    {
                        updater = new updater<valueType, modelType>();
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, null, false);
                    }
                }
                return updater;
            }
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待更新数据</param>
        /// <param name="memberMap">目标成员位图</param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
        /// <param name="onUpdated">是否成功</param>
        public override void UpdateByIdentity<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap, bool isIgnoreTransaction, Action<valueType, valueType, fastCSharp.code.memberMap> onUpdated)
        {
            if (onUpdated == null) log.Error.Throw(log.exceptionType.Null);
            updater<valueType, modelType> updater;
            bool isTransaction = false;
            try
            {
                if (!isIgnoreTransaction && sqlTool.IsDeleteTransaction)
                {
                    if (fastCSharp.domainUnload.TransactionStart(false))
                    {
                        isTransaction = true;
                        updater = updater<valueType, modelType>.Get();
                    }
                    else updater = null;
                }
                else updater = updater<valueType, modelType>.Get();
                if (updater != null)
                {
                    fastCSharp.code.memberMap<modelType> updateMemberMap = sqlTool.GetMemberMapClearIdentity(memberMap);
                    if (sqlModel<modelType>.verify.Verify(value, updateMemberMap, sqlTool) && sqlTool.CallOnUpdate(value, updateMemberMap))
                    {
                        sqlTool.GetSelectMemberMap(updateMemberMap, updater.MemberMap);
                        updater.Update(this, sqlTool, updateByIdentity(sqlTool, value, updateMemberMap, updater.MemberMap, updater.SqlStream), value, updateMemberMap, ref onUpdated, ref isTransaction);
                    }
                }
            }
            finally
            {
                if (isTransaction) fastCSharp.domainUnload.TransactionEnd();
                if (onUpdated != null) onUpdated(null, null, null);
            }
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待更新数据自增id标识</param>
        /// <param name="updateExpression">待更新数据</param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
        /// <param name="onUpdated"></param>
        public override void UpdateByIdentity<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value
            , ref fastCSharp.emit.sqlTable.updateExpression updateExpression, bool isIgnoreTransaction, Action<valueType, valueType, fastCSharp.code.memberMap> onUpdated)
        {
            if (onUpdated == null) log.Error.Throw(log.exceptionType.Null);
            updater<valueType, modelType> updater;
            bool isTransaction = false;
            try
            {
                if (!isIgnoreTransaction && sqlTool.IsDeleteTransaction)
                {
                    if (fastCSharp.domainUnload.TransactionStart(false))
                    {
                        isTransaction = true;
                        updater = updater<valueType, modelType>.Get();
                    }
                    else updater = null;
                }
                else updater = updater<valueType, modelType>.Get();
                if (updater != null)
                {
                    fastCSharp.code.memberMap<modelType> updateMemberMap = updateExpression.CreateMemberMap<modelType>();
                    if (!updateMemberMap.IsDefault)
                    {
                        updateMemberMap.ClearMember(sqlModel<modelType>.Identity.MemberMapIndex);
                        if (sqlTool.CallOnUpdate(value, updateMemberMap))
                        {
                            sqlTool.GetSelectMemberMap(updateMemberMap, updater.MemberMap);
                            updater.Update(this, sqlTool, updateByIdentity(sqlTool, value, ref updateExpression, updater.MemberMap, updater.SqlStream), value, updateMemberMap, ref onUpdated, ref isTransaction);
                        }
                    }
                }
            }
            finally
            {
                if (isTransaction) fastCSharp.domainUnload.TransactionEnd();
                if (onUpdated != null) onUpdated(null, null, null);
            }
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待删除数据</param>
        /// <param name="sqlStream"></param>
        /// <returns>SQL语句</returns>
        private unsafe string deleteSql<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, charStream sqlStream)
            where valueType : class, modelType
            where modelType : class
        {
            pointer buffer = fastCSharp.sql.client.SqlBuffers.Get();
            try
            {
                using (sqlStream)
                {
                    sqlStream.UnsafeReset(buffer.Byte, SqlBufferSize << 1);
                    sqlStream.SimpleWriteNotNull("select top 1 ");
                    sqlModel<modelType>.GetNames(sqlStream, sqlTool.SelectMemberMap);
                    sqlStream.SimpleWriteNotNull(" from[");
                    sqlStream.SimpleWriteNotNull(sqlTool.TableName);
                    sqlStream.WriteNotNull("]with(nolock)where ");
                    sqlModel<modelType>.primaryKeyWhere.Where(sqlStream, value, Converter);
                    sqlStream.WriteNotNull(@"
if @@ROWCOUNT<>0 begin
 delete[");
                    sqlStream.SimpleWriteNotNull(sqlTool.TableName);
                    sqlStream.SimpleWriteNotNull("]from[");
                    sqlStream.SimpleWriteNotNull(sqlTool.TableName);
                    sqlStream.WriteNotNull("]with(nolock)where ");
                    sqlModel<modelType>.primaryKeyWhere.Where(sqlStream, value, Converter);
                    sqlStream.SimpleWriteNotNull(@"
end");
                    return sqlStream.ToString();
                }
            }
            finally { fastCSharp.sql.client.SqlBuffers.Push(ref buffer); }
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待删除数据</param>
        /// <returns>是否成功</returns>
        protected override unsafe bool delete<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value)
        {
            if (set<valueType, modelType>(deleteSql(sqlTool, value, new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1)), value, sqlTool.SelectMemberMap))
            {
                sqlTool.CallOnDeletedLock(value);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待删除数据</param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
        /// <param name="onDeleted">是否成功</param>
        protected override void delete<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, bool isIgnoreTransaction, ref Action<valueType> onDeleted)
        {
            deleter<valueType, modelType> deleter;
            bool isTransaction = false;
            if (!isIgnoreTransaction && sqlTool.IsDeleteTransaction)
            {
                if (fastCSharp.domainUnload.TransactionStart(false))
                {
                    isTransaction = true;
                    deleter = deleter<valueType, modelType>.Get();
                }
                else deleter = null;
            }
            else deleter = deleter<valueType, modelType>.Get();
            if (deleter != null) deleter.Delete(this, sqlTool, deleteSql(sqlTool, value, deleter.SqlStream), value, ref onDeleted, isTransaction);
            else if (isTransaction) fastCSharp.domainUnload.TransactionEnd();
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待删除数据</param>
        /// <param name="sqlStream"></param>
        /// <returns>SQL语句</returns>
        private unsafe string deleteSqlByIdentity<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, charStream sqlStream)
            where valueType : class, modelType
            where modelType : class
        {
            pointer buffer = fastCSharp.sql.client.SqlBuffers.Get();
            try
            {
                using (sqlStream)
                {
                    sqlStream.UnsafeReset(buffer.Byte, SqlBufferSize << 1);
                    sqlStream.SimpleWriteNotNull("select top 1 ");
                    sqlModel<modelType>.GetNames(sqlStream, sqlTool.SelectMemberMap);
                    sqlStream.SimpleWriteNotNull(" from[");
                    sqlStream.SimpleWriteNotNull(sqlTool.TableName);
                    sqlStream.WriteNotNull("]with(nolock)where ");
                    sqlStream.SimpleWriteNotNull(sqlModel<modelType>.Identity.SqlFieldName);
                    sqlStream.Write('=');
                    fastCSharp.number.ToString(sqlModel<modelType>.GetIdentity(value), sqlStream);
                    sqlStream.WriteNotNull(@"
if @@ROWCOUNT<>0 begin
 delete[");
                    sqlStream.SimpleWriteNotNull(sqlTool.TableName);
                    sqlStream.SimpleWriteNotNull("]from[");
                    sqlStream.SimpleWriteNotNull(sqlTool.TableName);
                    sqlStream.WriteNotNull("]with(nolock)where ");
                    sqlStream.SimpleWriteNotNull(sqlModel<modelType>.Identity.SqlFieldName);
                    sqlStream.Write('=');
                    fastCSharp.number.ToString(sqlModel<modelType>.GetIdentity(value), sqlStream);
                    sqlStream.SimpleWriteNotNull(@"
end");
                    return sqlStream.ToString();
                }
            }
            finally { fastCSharp.sql.client.SqlBuffers.Push(ref buffer); }
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待删除数据</param>
        /// <returns>是否成功</returns>
        protected override unsafe bool deleteByIdentity<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value)
        {
            if (set<valueType, modelType>(deleteSqlByIdentity(sqlTool, value, new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1)), value, sqlTool.SelectMemberMap))
            {
                sqlTool.CallOnDeletedLock(value);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        private sealed unsafe class deleter<valueType, modelType> : IDisposable
            where valueType : class, modelType
            where modelType : class
        {
            /// <summary>
            /// SQL字符流
            /// </summary>
            internal charStream SqlStream = new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1);
            /// <summary>
            /// SQL客户端操作
            /// </summary>
            private client client;
            /// <summary>
            /// SQL操作工具
            /// </summary>
            private fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool;
            /// <summary>
            /// SQL语句
            /// </summary>
            private string sql;
            /// <summary>
            /// 目标对象
            /// </summary>
            private valueType value;
            /// <summary>
            /// 获取数据回调
            /// </summary>
            private Action<valueType> onDeleted;
            /// <summary>
            /// SQL连接
            /// </summary>
            private DbConnection connection;
            /// <summary>
            /// SQL命令
            /// </summary>
            private SqlCommand command;
            /// <summary>
            /// 获取数据回调
            /// </summary>
            private AsyncCallback callback;
            /// <summary>
            /// 是否事物处理
            /// </summary>
            private bool isTransaction;
            /// <summary>
            /// SQL连接执行是否成功
            /// </summary>
            private byte isExecute;
            /// <summary>
            /// 获取数据
            /// </summary>
            private deleter()
            {
                callback = onDelete;
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                pub.Dispose(ref SqlStream);
            }
            /// <summary>
            /// 删除数据
            /// </summary>
            /// <param name="client">SQL客户端操作</param>
            /// <param name="sqlTool">SQL操作工具</param>
            /// <param name="sql">SQL语句</param>
            /// <param name="value">目标对象</param>
            /// <param name="onDeleted">删除数据回调</param>
            /// <param name="isTransaction"></param>
            public void Delete(client client, fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, string sql, valueType value, ref Action<valueType> onDeleted, bool isTransaction)
            {
                this.onDeleted = onDeleted;
                this.client = client;
                this.sqlTool = sqlTool;
                this.sql = sql;
                this.value = value;
                this.isTransaction = isTransaction;
                onDeleted = null;
                try
                {
                    if ((connection = client.GetConnection(true)) != null)
                    {
                        (command = (SqlCommand)client.GetCommand(connection, sql)).BeginExecuteReader(callback, this, CommandBehavior.SingleResult);
                        return;
                    }
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, sql, false);
                }
                this.value = null;
                push();
            }
            /// <summary>
            /// 获取数据回调
            /// </summary>
            /// <param name="result"></param>
            private void onDelete(IAsyncResult result)
            {
                try
                {
                    using (SqlDataReader reader = command.EndExecuteReader(result))
                    {
                        if (reader.Read())
                        {
                            isExecute = 1;
                            sqlModel<modelType>.set.Set(reader, value, sqlTool.SelectMemberMap);
                        }
                        else
                        {
                            isExecute = 1;
                            value = null;
                        }
                    }
                }
                catch (Exception error)
                {
                    value = null;
                    fastCSharp.log.Error.Add(error, sql, false);
                }
                push();
            }
            /// <summary>
            /// 连接池处理
            /// </summary>
            private void push()
            {
                Action<valueType> onDeleted = this.onDeleted;
                valueType value = this.value;
                fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool = this.sqlTool;
                bool isTransaction = this.isTransaction;
                try
                {
                    if (command != null) command.Dispose();
                    if (isExecute == 0)
                    {
                        if (connection != null) connection.Dispose();
                    }
                    else client.AsynchronousConnectionPool.Push(ref connection);
                    client = null;
                    connection = null;
                    command = null;
                    sql = null;
                    this.sqlTool = null;
                    this.onDeleted = null;
                    this.value = null;
                    this.isExecute = 0;
                    typePool<deleter<valueType, modelType>>.PushNotNull(this);
                }
                finally
                {
                    if (sqlTool.IsLockWrite)
                    {
                        Monitor.Enter(sqlTool.Lock);
                        try
                        {
                            sqlTool.CallOnDeletedLock(value);
                        }
                        catch (Exception error)
                        {
                            log.Default.Add(error, null, false);
                        }
                        finally { Monitor.Exit(sqlTool.Lock); }
                    }
                    if (isTransaction) fastCSharp.domainUnload.TransactionEnd();
                    try
                    {
                        sqlTool.CallOnDeleted(value);
                    }
                    finally { onDeleted(value); }
                }
            }
            /// <summary>
            /// 获取数据
            /// </summary>
            /// <returns></returns>
            public static deleter<valueType, modelType> Get()
            {
                deleter<valueType, modelType> deleter = typePool<deleter<valueType, modelType>>.Pop();
                if (deleter == null)
                {
                    try
                    {
                        deleter = new deleter<valueType, modelType>();
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, null, false);
                    }
                }
                return deleter;
            }
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待删除数据</param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
        /// <param name="onDeleted">是否成功</param>
        protected override void deleteByIdentity<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, bool isIgnoreTransaction, ref Action<valueType> onDeleted)
        {
            deleter<valueType, modelType> deleter;
            bool isTransaction = false;
            if (!isIgnoreTransaction && sqlTool.IsDeleteTransaction)
            {
                if (fastCSharp.domainUnload.TransactionStart(false))
                {
                    isTransaction = true;
                    deleter = deleter<valueType, modelType>.Get();
                }
                else deleter = null;
            }
            else deleter = deleter<valueType, modelType>.Get();
            if (deleter != null) deleter.Delete(this, sqlTool, deleteSqlByIdentity(sqlTool, value, deleter.SqlStream), value, ref onDeleted, isTransaction);
            else if (isTransaction) fastCSharp.domainUnload.TransactionEnd();
        }
        /// <summary>
        /// 委托关联表达式转SQL表达式
        /// </summary>
        /// <param name="expression">委托关联表达式</param>
        /// <param name="logicConstantWhere">逻辑常量值</param>
        /// <returns>SQL表达式(null表示常量条件)</returns>
        public override string GetWhere(LambdaExpression expression, ref bool logicConstantWhere)
        {
            if (expression != null)
            {
                fastCSharp.sql.expression.LambdaExpression sqlExpression = fastCSharp.sql.expression.LambdaExpression.convert(expression);
                try
                {
                    if (!sqlExpression.IsLogicConstantExpression) return converter.Convert(sqlExpression).Value;
                    logicConstantWhere = sqlExpression.LogicConstantValue;
                }
                finally { sqlExpression.PushPool(); }
            }
            else logicConstantWhere = true;
            return null;
        }
        /// <summary>
        /// 委托关联表达式转SQL表达式
        /// </summary>
        /// <param name="expression">委托关联表达式</param>
        /// <param name="sqlStream">SQL表达式流</param>
        /// <param name="logicConstantWhere">逻辑常量值</param>
        /// <returns>参数成员名称</returns>
        internal override keyValue<string, string> GetWhere(LambdaExpression expression, charStream sqlStream, ref bool logicConstantWhere)
        {
            if (expression != null)
            {
                fastCSharp.sql.expression.LambdaExpression sqlExpression = fastCSharp.sql.expression.LambdaExpression.convert(expression);
                try
                {
                    if (!sqlExpression.IsLogicConstantExpression) return converter.Convert(sqlExpression, sqlStream);
                    logicConstantWhere = sqlExpression.LogicConstantValue;
                }
                finally { sqlExpression.PushPool(); }
            }
            else logicConstantWhere = true;
            return default(keyValue<string, string>);
        }
        /// <summary>
        /// 委托关联表达式转SQL表达式
        /// </summary>
        /// <param name="expression">委托关联表达式</param>
        /// <returns>参数成员名称+SQL表达式</returns>
        public override keyValue<string, string> GetSql(LambdaExpression expression)
        {
            if (expression != null)
            {
                fastCSharp.sql.expression.LambdaExpression sqlExpression = fastCSharp.sql.expression.LambdaExpression.convert(expression);
                try
                {
                    return converter.Convert(sqlExpression);
                }
                finally { sqlExpression.PushPool(); }
            }
            return default(keyValue<string, string>);
        }
        /// <summary>
        /// 委托关联表达式转SQL表达式
        /// </summary>
        /// <param name="expression">委托关联表达式</param>
        /// <param name="sqlStream">SQL表达式流</param>
        /// <returns>参数成员名称</returns>
        internal override keyValue<string, string> GetSql(LambdaExpression expression, charStream sqlStream)
        {
            if (expression != null)
            {
                fastCSharp.sql.expression.LambdaExpression sqlExpression = fastCSharp.sql.expression.LambdaExpression.convert(expression);
                try
                {
                    return converter.Convert(sqlExpression, sqlStream);
                }
                finally { sqlExpression.PushPool(); }
            }
            return default(keyValue<string, string>);
        }
        /// <summary>
        /// 获取记录数
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="expression">查询表达式</param>
        /// <param name="sqlStream"></param>
        /// <returns></returns>
        private unsafe string count<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, Expression<Func<modelType, bool>> expression, charStream sqlStream)
            where valueType : class, modelType
            where modelType : class
        {
            pointer buffer = fastCSharp.sql.client.SqlBuffers.Get();
            try
            {
                using (sqlStream)
                {
                    sqlStream.UnsafeReset(buffer.Byte, SqlBufferSize << 1);
                    sqlStream.WriteNotNull("select count(*)from[");
                    sqlStream.SimpleWriteNotNull(sqlTool.TableName);
                    sqlStream.SimpleWriteNotNull("]with(nolock)");
                    bool isCreatedIndex = false;
                    if (selectQuery<modelType>.WriteWhere(sqlTool, sqlStream, expression, ref isCreatedIndex)) return sqlStream.ToString();
                }
            }
            finally { fastCSharp.sql.client.SqlBuffers.Push(ref buffer); }
            return null;
        }
        /// <summary>
        /// 获取记录数
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="expression">查询表达式</param>
        /// <returns>记录数,失败返回-1</returns>
        public override unsafe int Count<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, Expression<Func<modelType, bool>> expression)
        {
            string sql = count(sqlTool, expression, new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1));
            return sql != null ? GetValue(sql, -1) : -1;
        }
        /// <summary>
        /// 获取记录数
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="expression">查询表达式</param>
        /// <param name="onGet">记录数,失败返回-1</param>
        public override void Count<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, Expression<Func<modelType, bool>> expression, Action<int> onGet)
        {
            try
            {
                getter<int> getter = getter<int>.Get();
                if (getter != null)
                {
                    string sql = count(sqlTool, expression, getter.SqlStream);
                    if (sql != null) getter.Get(this, sql, -1, ref onGet);
                    else typePool<getter<int>>.PushNotNull(getter);
                }
            }
            finally
            {
                if (onGet != null) onGet(-1);
            }
        }
        /// <summary>
        /// 查询单值数据
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="query">查询信息</param>
        /// <param name="memberSqlName">成员名称</param>
        /// <param name="sqlStream"></param>
        /// <returns></returns>
        private unsafe string getValue<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, selectQuery<modelType> query, string memberSqlName, charStream sqlStream)
            where valueType : class, modelType
            where modelType : class
        {
            pointer buffer = fastCSharp.sql.client.SqlBuffers.Get();
            try
            {
                using (sqlStream)
                {
                    sqlStream.UnsafeReset(buffer.Byte, SqlBufferSize << 1);
                    sqlStream.SimpleWriteNotNull("select top 1 ");
                    sqlStream.SimpleWriteNotNull(memberSqlName);
                    sqlStream.SimpleWriteNotNull(" from[");
                    sqlStream.SimpleWriteNotNull(sqlTool.TableName);
                    sqlStream.SimpleWriteNotNull("]with(nolock)");
                    if (query == null) return sqlStream.ToString();
                    if (query.WriteWhere(sqlTool, sqlStream))
                    {
                        query.WriteOrder(sqlTool, sqlStream);
                        return sqlStream.ToString();
                    }
                }
            }
            finally { fastCSharp.sql.client.SqlBuffers.Push(ref buffer); }
            return null;
        }
        /// <summary>
        /// 查询单值数据
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <typeparam name="returnType">返回值类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="query">查询信息</param>
        /// <param name="memberSqlName">成员名称</param>
        /// <param name="errorValue">错误值</param>
        /// <returns>对象集合</returns>
        internal override unsafe returnType GetValue<valueType, modelType, returnType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, selectQuery<modelType> query, string memberSqlName, returnType errorValue)
        {
            string sql = getValue(sqlTool, query, memberSqlName, new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1));
            return sql != null ? GetValue<returnType>(sql, errorValue) : errorValue;
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="returnType">返回值类型</typeparam>
        private sealed unsafe class getter<returnType> : IDisposable
        {
            /// <summary>
            /// SQL字符流
            /// </summary>
            internal charStream SqlStream = new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1);
            /// <summary>
            /// SQL客户端操作
            /// </summary>
            private client client;
            /// <summary>
            /// SQL语句
            /// </summary>
            private string sql;
            /// <summary>
            /// 返回数据值
            /// </summary>
            private returnType value;
            /// <summary>
            /// 获取数据回调
            /// </summary>
            private Action<returnType> onGet;
            /// <summary>
            /// SQL连接
            /// </summary>
            private DbConnection connection;
            /// <summary>
            /// SQL命令
            /// </summary>
            private SqlCommand command;
            /// <summary>
            /// 获取数据回调
            /// </summary>
            private AsyncCallback callback;
            /// <summary>
            /// SQL连接执行是否成功
            /// </summary>
            private byte isExecute;
            /// <summary>
            /// 获取数据
            /// </summary>
            private getter()
            {
                callback = onRead;
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                pub.Dispose(ref SqlStream);
            }
            /// <summary>
            /// 获取数据
            /// </summary>
            /// <param name="client">SQL客户端操作</param>
            /// <param name="sql">SQL语句</param>
            /// <param name="errorValue">错误值</param>
            /// <param name="onGet">获取数据回调</param>
            public void Get(client client, string sql, returnType errorValue, ref Action<returnType> onGet)
            {
                this.onGet = onGet;
                this.client = client;
                this.sql = sql;
                this.value = errorValue;
                onGet = null;
                try
                {
                    if ((connection = client.GetConnection(true)) != null)
                    {
                        (command = (SqlCommand)client.GetCommand(connection, sql)).BeginExecuteReader(callback, this, CommandBehavior.SingleResult);
                        return;
                    }
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, sql, false);
                }
                push();
            }
            /// <summary>
            /// 获取数据回调
            /// </summary>
            /// <param name="result"></param>
            private void onRead(IAsyncResult result)
            {
                try
                {
                    using (SqlDataReader reader = command.EndExecuteReader(result))
                    {
                        if (reader.Read())
                        {
                            isExecute = 1;
                            object value = reader[0];
                            if (value != null && value != DBNull.Value) this.value = (returnType)value;
                        }
                        else isExecute = 1;
                    }
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, sql, false);
                }
                push();
            }
            /// <summary>
            /// 连接池处理
            /// </summary>
            private void push()
            {
                Action<returnType> onGet = this.onGet;
                returnType value = this.value;
                try
                {
                    if (command != null) command.Dispose();
                    if (isExecute == 0) connection.Dispose();
                    else client.AsynchronousConnectionPool.Push(ref connection);
                    client = null;
                    connection = null;
                    command = null;
                    sql = null;
                    this.onGet = null;
                    this.isExecute = 0;
                    typePool<getter<returnType>>.PushNotNull(this);
                }
                finally
                {
                    onGet(value);
                }
            }
            /// <summary>
            /// 获取数据
            /// </summary>
            /// <returns></returns>
            public static getter<returnType> Get()
            {
                getter<returnType> getter = typePool<getter<returnType>>.Pop();
                if (getter == null)
                {
                    try
                    {
                        getter = new getter<returnType>();
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, null, false);
                    }
                }
                return getter;
            }
        }
        /// <summary>
        /// 查询单值数据
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <typeparam name="returnType">返回值类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="query">查询信息</param>
        /// <param name="memberName">成员名称</param>
        /// <param name="errorValue">错误值</param>
        /// <param name="onGet"></param>
        internal override void GetValue<valueType, modelType, returnType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, selectQuery<modelType> query, string memberName, returnType errorValue, Action<returnType> onGet)
        {
            if (onGet == null) log.Error.Throw(log.exceptionType.Null);
            try
            {
                getter<returnType> getter = getter<returnType>.Get();
                if (getter != null)
                {
                    string sql = getValue(sqlTool, query, memberName, getter.SqlStream);
                    if (sql != null) getter.Get(this, sql, errorValue, ref onGet);
                    else typePool<getter<returnType>>.PushNotNull(getter);
                }
            }
            finally
            {
                if (onGet != null) onGet(errorValue);
            }
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
            }
            return selectNoOrderPushMemberMap(sqlTool, query, memberMap);
        }
        /// <summary>
        /// 查询对象集合
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        /// <typeparam name="modelType">数据模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="query">查询信息</param>
        /// <param name="memberMap">成员位图</param>
        /// <param name="sqlStream"></param>
        protected unsafe string selectNoOrder<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, selectQuery<modelType> query, fastCSharp.code.memberMap<modelType> memberMap, charStream sqlStream)
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
                    if (query != null)
                    {
                        int count = query.SkipCount + query.GetCount;
                        if (count != 0)
                        {
                            sqlStream.SimpleWriteNotNull("top ");
                            number.ToString(count, sqlStream);
                            sqlStream.Write(' ');
                        }
                    }
                    sqlModel<modelType>.GetNames(sqlStream, memberMap);
                    sqlStream.SimpleWriteNotNull(" from [");
                    sqlStream.SimpleWriteNotNull(sqlTool.TableName);
                    sqlStream.SimpleWriteNotNull("]with(nolock)");
                    if (query == null) return sqlStream.ToString();
                    if (query.WriteWhere(sqlTool, sqlStream))
                    {
                        query.WriteOrder(sqlTool, sqlStream);
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
        /// <typeparam name="modelType">数据模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="query">查询信息</param>
        /// <param name="memberMap">成员位图</param>
        /// <returns>对象集合</returns>
        internal unsafe IEnumerable<valueType> selectNoOrderPushMemberMap<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, selectQuery<modelType> query, fastCSharp.code.memberMap<modelType> memberMap)
            where valueType : class, modelType
            where modelType : class
        {
            string sql = selectNoOrder(sqlTool, query, memberMap, new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1));
            return sql != null ? selectPushMemberMap<valueType, modelType>(sql, query == null ? 0 : query.SkipCount, memberMap) : nullValue<valueType>.Array;
        }
        /// <summary>
        /// 查询对象集合
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
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
                    sqlStream.WriteNotNull(" in(select top ");
                    number.ToString(query.GetCount, sqlStream);
                    sqlStream.Write(' ');
                    sqlStream.SimpleWriteNotNull(keyName);
                    sqlStream.SimpleWriteNotNull(" from[");
                    sqlStream.SimpleWriteNotNull(sqlTool.TableName);
                    sqlStream.WriteNotNull("]with(nolock)where ");
                    sqlStream.Write('(');
                    int startIndex = sqlStream.Length;
                    if (query.WriteWhereOnly(sqlTool, sqlStream))
                    {
                        int count = sqlStream.Length - startIndex;
                        if (count == 0) sqlStream.UnsafeFreeLength(1);
                        else sqlStream.Write(")and ");
                        sqlStream.SimpleWriteNotNull(keyName);
                        sqlStream.WriteNotNull(" not in(select top ");
                        number.ToString(query.SkipCount, sqlStream);
                        sqlStream.Write(' ');
                        sqlStream.SimpleWriteNotNull(keyName);
                        sqlStream.SimpleWriteNotNull(" from[");
                        sqlStream.SimpleWriteNotNull(sqlTool.TableName);
                        sqlStream.SimpleWriteNotNull("]with(nolock)");
                        if (count != 0)
                        {
                            sqlStream.SimpleWriteNotNull("where ");
                            sqlStream.Write(sqlStream.Char + startIndex, count);
                        }

                        startIndex = sqlStream.Length;
                        query.WriteOrder(sqlTool, sqlStream);
                        count = sqlStream.Length - startIndex;
                        sqlStream.Write(')');
                        if (count != 0) sqlStream.Write(sqlStream.Char + startIndex, count);
                        sqlStream.Write(')');
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
        /// 获取数据
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <typeparam name="modelType"></typeparam>
        protected sealed unsafe new class selector<valueType, modelType> : IDisposable
            where valueType : class, modelType
            where modelType : class
        {
            /// <summary>
            /// SQL字符流
            /// </summary>
            internal charStream SqlStream = new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1);
            /// <summary>
            /// 目标成员位图
            /// </summary>
            internal fastCSharp.code.memberMap<modelType> MemberMap = fastCSharp.code.memberMap<modelType>.New();
            /// <summary>
            /// SQL客户端操作
            /// </summary>
            private client client;
            /// <summary>
            /// SQL语句
            /// </summary>
            private string sql;
            /// <summary>
            /// 获取数据回调
            /// </summary>
            private Action<subArray<valueType>> onGet;
            /// <summary>
            /// SQL连接
            /// </summary>
            private DbConnection connection;
            /// <summary>
            /// SQL命令
            /// </summary>
            private SqlCommand command;
            /// <summary>
            /// 获取数据回调
            /// </summary>
            private AsyncCallback callback;
            /// <summary>
            /// 跳过记录数
            /// </summary>
            private int skipCount;
            /// <summary>
            /// 获取数据
            /// </summary>
            private selector()
            {
                callback = onRead;
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                pub.Dispose(ref MemberMap);
                pub.Dispose(ref SqlStream);
            }
            /// <summary>
            /// 获取数据
            /// </summary>
            /// <param name="client">SQL客户端操作</param>
            /// <param name="sql">SQL语句</param>
            /// <param name="skipCount">跳过记录数</param>
            /// <param name="onGet">获取数据回调</param>
            public void Get(client client, string sql, int skipCount, ref Action<subArray<valueType>> onGet)
            {
                this.onGet = onGet;
                this.client = client;
                this.sql = sql;
                this.skipCount = skipCount;
                onGet = null;
                try
                {
                    if ((connection = client.GetConnection(true)) != null)
                    {
                        (command = (SqlCommand)client.GetCommand(connection, sql)).BeginExecuteReader(callback, this, CommandBehavior.SingleResult);
                        return;
                    }
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, sql, false);
                }
                push(default(subArray<valueType>));
            }
            /// <summary>
            /// 获取数据回调
            /// </summary>
            /// <param name="result"></param>
            private void onRead(IAsyncResult result)
            {
                subArray<valueType> values = default(subArray<valueType>);
                try
                {
                    using (SqlDataReader reader = command.EndExecuteReader(result))
                    {
                        while (skipCount != 0 && reader.Read()) --skipCount;
                        if (skipCount == 0)
                        {
                            while (reader.Read())
                            {
                                valueType value = fastCSharp.emit.constructor<valueType>.New();
                                sqlModel<modelType>.set.Set(reader, value, MemberMap);
                                values.Add(value);
                            }
                        }
                        if (values.length == 0) values.array = nullValue<valueType>.Array;
                    }
                }
                catch (Exception error)
                {
                    values.Null();
                    fastCSharp.log.Error.Add(error, sql, false);
                }
                push(values);
            }
            /// <summary>
            /// 连接池处理
            /// </summary>
            /// <param name="value"></param>
            private void push(subArray<valueType> value)
            {
                Action<subArray<valueType>> onGet = this.onGet;
                try
                {
                    if (command != null) command.Dispose();
                    if (value.array == null)
                    {
                        if (connection != null) connection.Dispose();
                    }
                    else client.AsynchronousConnectionPool.Push(ref connection);
                    client = null;
                    connection = null;
                    command = null;
                    sql = null;
                    this.onGet = null;
                    typePool<selector<valueType, modelType>>.PushNotNull(this);
                }
                finally
                {
                    onGet(value);
                }
            }
            /// <summary>
            /// 获取数据
            /// </summary>
            /// <returns></returns>
            public static selector<valueType, modelType> Get()
            {
                selector<valueType, modelType> selector = typePool<selector<valueType, modelType>>.Pop();
                if (selector == null)
                {
                    try
                    {
                        selector = new selector<valueType, modelType>();
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, null, false);
                    }
                }
                return selector;
            }
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
        /// 查询对象
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">匹配成员值</param>
        /// <param name="memberMap">成员位图</param>
        /// <param name="sqlStream"></param>
        /// <returns>SQL语句</returns>
        private unsafe string getByIdentity<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap, charStream sqlStream)
            where valueType : class, modelType
            where modelType : class
        {
            pointer buffer = fastCSharp.sql.client.SqlBuffers.Get();
            try
            {
                using (sqlStream)
                {
                    sqlStream.UnsafeReset(buffer.Byte, SqlBufferSize << 1);
                    sqlStream.SimpleWriteNotNull("select top 1 ");
                    sqlModel<modelType>.GetNames(sqlStream, memberMap);
                    sqlStream.SimpleWriteNotNull(" from[");
                    sqlStream.SimpleWriteNotNull(sqlTool.TableName);
                    sqlStream.WriteNotNull("]with(nolock)where ");
                    sqlStream.SimpleWriteNotNull(sqlModel<modelType>.Identity.SqlFieldName);
                    sqlStream.Write('=');
                    fastCSharp.number.ToString(sqlModel<modelType>.GetIdentity(value), sqlStream);
                    return sqlStream.ToString();
                }
            }
            finally { fastCSharp.sql.client.SqlBuffers.Push(ref buffer); }
        }
        /// <summary>
        /// 查询对象
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">匹配成员值</param>
        /// <param name="memberMap">成员位图</param>
        /// <returns>对象集合</returns>
        public override unsafe valueType GetByIdentity<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap)
        {
            using (fastCSharp.code.memberMap<modelType> selectMemberMap = fastCSharp.emit.sqlModel<modelType>.CopyMemberMap)
            {
                if (memberMap != null && !memberMap.IsDefault) selectMemberMap.And(memberMap);
                string sql = getByIdentity(sqlTool, value, selectMemberMap, new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1));
                if (set<valueType, modelType>(sql, value, selectMemberMap)) return value;
            }
            return null;
        }
        /// <summary>
        /// 查询对象
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">匹配成员值</param>
        /// <param name="memberMap">成员位图</param>
        /// <param name="onGet"></param>
        public override void GetByIdentity<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap, Action<valueType> onGet)
        {
            if (onGet == null) log.Error.Throw(log.exceptionType.Null);
            try
            {
                getter<valueType, modelType> getter = getter<valueType, modelType>.Get();
                if (getter != null)
                {
                    getter.MemberMap.CopyFrom(fastCSharp.emit.sqlModel<modelType>.MemberMap);
                    if (memberMap != null && !memberMap.IsDefault) getter.MemberMap.And(memberMap);
                    getter.Get(this, getByIdentity(sqlTool, value, getter.MemberMap, getter.SqlStream), value, ref onGet);
                }
            }
            finally
            {
                if (onGet != null) onGet(null);
            }
        }
        /// <summary>
        /// 查询对象
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">匹配成员值</param>
        /// <param name="memberMap"></param>
        /// <param name="sqlStream"></param>
        /// <returns>SQL语句</returns>
        private unsafe string getByPrimaryKey<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap, charStream sqlStream)
            where valueType : class, modelType
            where modelType : class
        {
            pointer buffer = fastCSharp.sql.client.SqlBuffers.Get();
            try
            {
                using (sqlStream)
                {
                    sqlStream.UnsafeReset(buffer.Byte, SqlBufferSize << 1);
                    sqlStream.SimpleWriteNotNull("select top 1 ");
                    sqlModel<modelType>.GetNames(sqlStream, memberMap);
                    sqlStream.SimpleWriteNotNull(" from[");
                    sqlStream.SimpleWriteNotNull(sqlTool.TableName);
                    sqlStream.WriteNotNull("]with(nolock)where ");
                    sqlModel<modelType>.primaryKeyWhere.Where(sqlStream, value, Converter);
                    return sqlStream.ToString();
                }
            }
            finally { fastCSharp.sql.client.SqlBuffers.Push(ref buffer); }
        }
        /// <summary>
        /// 查询对象
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">匹配成员值</param>
        /// <param name="memberMap">成员位图</param>
        /// <returns>对象集合</returns>
        public override unsafe valueType GetByPrimaryKey<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap)
        {
            using (fastCSharp.code.memberMap<modelType> selectMemberMap = fastCSharp.emit.sqlModel<modelType>.CopyMemberMap)
            {
                if (memberMap != null && !memberMap.IsDefault) selectMemberMap.And(memberMap);
                string sql = getByPrimaryKey(sqlTool, value, selectMemberMap, new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1));
                if (set<valueType, modelType>(sql, value, selectMemberMap)) return value;
            }
            return null;
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        private sealed unsafe class getter<valueType, modelType> : IDisposable
            where valueType : class, modelType
            where modelType : class
        {
            /// <summary>
            /// SQL字符流
            /// </summary>
            internal charStream SqlStream = new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1);
            /// <summary>
            /// 目标成员位图
            /// </summary>
            internal fastCSharp.code.memberMap<modelType> MemberMap = fastCSharp.code.memberMap<modelType>.New();
            /// <summary>
            /// SQL客户端操作
            /// </summary>
            private client client;
            /// <summary>
            /// SQL语句
            /// </summary>
            private string sql;
            /// <summary>
            /// 目标对象
            /// </summary>
            private valueType value;
            /// <summary>
            /// 获取数据回调
            /// </summary>
            private Action<valueType> onGet;
            /// <summary>
            /// SQL连接
            /// </summary>
            private DbConnection connection;
            /// <summary>
            /// SQL命令
            /// </summary>
            private SqlCommand command;
            /// <summary>
            /// 获取数据回调
            /// </summary>
            private AsyncCallback callback;
            /// <summary>
            /// SQL连接执行是否成功
            /// </summary>
            private byte isExecute;
            /// <summary>
            /// 获取数据
            /// </summary>
            private unsafe getter()
            {
                callback = onRead;
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                pub.Dispose(ref MemberMap);
                pub.Dispose(ref SqlStream);
            }
            /// <summary>
            /// 获取数据
            /// </summary>
            /// <param name="client">SQL客户端操作</param>
            /// <param name="sql">SQL语句</param>
            /// <param name="value">目标对象</param>
            /// <param name="onGet">获取数据回调</param>
            public void Get(client client, string sql, valueType value, ref Action<valueType> onGet)
            {
                this.onGet = onGet;
                this.client = client;
                this.sql = sql;
                this.value = value;
                onGet = null;
                try
                {
                    if ((connection = client.GetConnection(true)) != null)
                    {
                        (command = (SqlCommand)client.GetCommand(connection, sql)).BeginExecuteReader(callback, this, CommandBehavior.SingleResult);
                    }
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, sql, false);
                }
                this.value = null;
                push();
            }
            /// <summary>
            /// 获取数据回调
            /// </summary>
            /// <param name="result"></param>
            private void onRead(IAsyncResult result)
            {
                try
                {
                    using (SqlDataReader reader = command.EndExecuteReader(result))
                    {
                        if (reader.Read())
                        {
                            isExecute = 1;
                            sqlModel<modelType>.set.Set(reader, value, MemberMap);
                        }
                        else
                        {
                            isExecute = 1;
                            value = null;
                        }
                    }
                }
                catch (Exception error)
                {
                    value = null;
                    fastCSharp.log.Error.Add(error, sql, false);
                }
                push();
            }
            /// <summary>
            /// 连接池处理
            /// </summary>
            private void push()
            {
                Action<valueType> onGet = this.onGet;
                valueType value = this.value;
                try
                {
                    if (command != null) command.Dispose();
                    if (isExecute == 0)
                    {
                        if (connection != null) connection.Dispose();
                    }
                    else client.AsynchronousConnectionPool.Push(ref connection);
                    client = null;
                    connection = null;
                    command = null;
                    sql = null;
                    this.onGet = null;
                    this.value = null;
                    this.isExecute = 0;
                    typePool<getter<valueType, modelType>>.PushNotNull(this);
                }
                finally
                {
                    onGet(value);
                }
            }
            /// <summary>
            /// 获取数据
            /// </summary>
            /// <returns></returns>
            public static getter<valueType, modelType> Get()
            {
                getter<valueType, modelType> getter = typePool<getter<valueType, modelType>>.Pop();
                if (getter == null)
                {
                    try
                    {
                        getter = new getter<valueType, modelType>();
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, null, false);
                    }
                }
                return getter;
            }
        }
        /// <summary>
        /// 查询对象
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">匹配成员值</param>
        /// <param name="memberMap">成员位图</param>
        /// <param name="onGet"></param>
        public override void GetByPrimaryKey<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap, Action<valueType> onGet)
        {
            if (onGet == null) log.Error.Throw(log.exceptionType.Null);
            try
            {
                getter<valueType, modelType> getter = getter<valueType, modelType>.Get();
                if (getter != null)
                {
                    getter.MemberMap.CopyFrom(fastCSharp.emit.sqlModel<modelType>.MemberMap);
                    if (memberMap != null && !memberMap.IsDefault) getter.MemberMap.And(memberMap);
                    getter.Get(this, getByPrimaryKey(sqlTool, value, getter.MemberMap, getter.SqlStream), value, ref onGet);
                }
            }
            finally
            {
                if (onGet != null) onGet(null);
            }
        }
    }
}
