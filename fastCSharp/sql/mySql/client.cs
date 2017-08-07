#if NOMYSQL
#else
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using System.Linq.Expressions;
using System.Collections.Generic;
using fastCSharp.code;
using MySql.Data.MySqlClient;
using fastCSharp.code.cSharp;
using fastCSharp.emit;
using System.Threading;

namespace fastCSharp.sql.mySql
{
    /// <summary>
    /// MySql客户端
    /// </summary>
    public sealed class client : sql.client
    {
        //grant usage on *.* to xxx_user@127.0.0.1 identified by 'xxx_pwd' with grant option;
        //flush privileges;
        //create database xxx;
        //grant all privileges on xxx.* to xxx_user@127.0.0.1 identified by 'xxx_pwd';
        //flush privileges;
        /// <summary>
        /// MySql客户端
        /// </summary>
        /// <param name="connection">SQL连接信息</param>
        public client(connection connection) : base(connection) { }
        /// <summary>
        /// 根据SQL连接类型获取SQL连接
        /// </summary>
        /// <param name="isAsynchronous">是否异步连接(不支持)</param>
        /// <returns>SQL连接</returns>
        internal override DbConnection GetConnection(bool isAsynchronous)
        {
            return ConnectionPool.Pop() ?? open(new MySqlConnection(this.Connection.Connection));
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
            DbCommand command = new MySqlCommand(sql, (MySqlConnection)connection);
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
            return new MySqlDataAdapter((MySqlCommand)command);
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
                        DataTable table = GetDataTable(GetCommand(connection, "select * from `" + tableName + "`"), ref isExecute);
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
            get { return false; }
        }
        /// <summary>
        /// 导入数据集合(不支持)
        /// </summary>
        /// <param name="connection">SQL连接</param>
        /// <param name="data">数据集合</param>
        /// <param name="batchSize">批处理数量</param>
        /// <param name="timeout">超时秒数</param>
        /// <returns>成功导入数量</returns>
        internal override int Import(DbConnection connection, DataTable data, int batchSize, int timeout)
        {
            log.Error.Add("mysql 不支持批量导入操作", new System.Diagnostics.StackFrame(), true);
            return 0;
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
            using (DbCommand command = GetCommand(connection, "show tables;"))
            using (DbDataReader reader = command.ExecuteReader(CommandBehavior.Default))
            {
                while (reader.Read())
                {
                    if (tableName.equalCase((string)reader[0]))
                    {
                        isExecute = 1;
                        return true;
                    }
                }
                isExecute = 1;
            }
            return false;
        }
        /// <summary>
        /// 写入列信息
        /// </summary>
        /// <param name="sqlStream">SQL语句流</param>
        /// <param name="column">列信息</param>
        private static void appendColumn(charStream sqlStream, column column)
        {
            sqlStream.Write('`');
            sqlStream.WriteNotNull(column.Name);
            sqlStream.WriteNotNull("` ");
            if (column.DbType == SqlDbType.Text || column.DbType == SqlDbType.NText)
            {
                if (column.Size <= 65535) sqlStream.WriteNotNull("TEXT");
                else if (column.Size <= 16777215) sqlStream.WriteNotNull("MEDIUMTEXT");
                else sqlStream.WriteNotNull("LONGTEXT");
                sqlStream.WriteNotNull(column.DbType == SqlDbType.NText ? " UNICODE" : " ASCII");
            }
            else
            {
                sqlStream.WriteNotNull(column.DbType.getSqlTypeName());
                if (column.DbType.isStringType())
                {
                    if (column.Size != int.MaxValue)
                    {
                        sqlStream.Write('(');
                        sqlStream.WriteNotNull(column.Size.toString());
                        sqlStream.Write(')');
                    }
                    sqlStream.WriteNotNull(column.DbType == SqlDbType.NChar || column.DbType == SqlDbType.NVarChar ? " UNICODE" : " ASCII");
                }
            }
            if (column.DefaultValue != null)
            {
                sqlStream.WriteNotNull(" default ");
                sqlStream.WriteNotNull(column.DefaultValue);
            }
            if (!column.IsNull) sqlStream.WriteNotNull(" not null");
            if (!string.IsNullOrEmpty(column.Remark))
            {
                sqlStream.WriteNotNull(" comment '");
                expression.constantConverter.Default[typeof(string)](sqlStream, column.Remark);
                sqlStream.Write('\'');
            }
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
                    sqlStream.WriteNotNull("create table`");
                    sqlStream.WriteNotNull(tableName);
                    sqlStream.WriteNotNull("`(");
                    bool isNext = false;
                    foreach (column column in table.Columns.Columns)
                    {
                        if (isNext) sqlStream.Write(',');
                        appendColumn(sqlStream, column);
                        isNext = true;
                    }
                    columnCollection primaryKey = table.PrimaryKey;
                    if (primaryKey != null && primaryKey.Columns.length() != 0)
                    {
                        isNext = false;
                        sqlStream.WriteNotNull(",primary key(");
                        foreach (column column in primaryKey.Columns)
                        {
                            if (isNext) sqlStream.Write(',');
                            sqlStream.WriteNotNull(column.Name);
                            isNext = true;
                        }
                        sqlStream.Write(')');
                    }
                    if (table.Indexs != null)
                    {
                        foreach (columnCollection columns in table.Indexs)
                        {
                            if (columns != null && columns.Columns.length() != 0)
                            {
                                if (columns.Type == columnCollection.type.UniqueIndex) sqlStream.WriteNotNull(@"unique index ");
                                else sqlStream.WriteNotNull(@"
index ");
                                AppendIndexName(sqlStream, tableName, columns);
                                sqlStream.Write('(');
                                isNext = false;
                                foreach (column column in columns.Columns)
                                {
                                    if (isNext) sqlStream.Write(',');
                                    sqlStream.Write('`');
                                    sqlStream.WriteNotNull(column.Name);
                                    sqlStream.Write('`');
                                    isNext = true;
                                }
                                sqlStream.Write(')');
                            }
                        }
                    }
                    sqlStream.WriteNotNull(");");
                    sql = sqlStream.ToString();
                }
            }
            finally { fastCSharp.sql.client.SqlBuffers.Push(ref buffer); }
            return executeNonQuery(connection, sql) != ExecuteNonQueryError;
        }
        /// <summary>
        /// 最大字符串长度(最大65532字节)
        /// </summary>
        private const int maxStringSize = 65535;
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
                    if (size <= 0) size = int.MaxValue;
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
            return executeNonQuery(connection, "drop table `" + tableName + "`;") != ExecuteNonQueryError;
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
create index`");
                    AppendIndexName(sqlStream, tableName, columnCollection);
                    sqlStream.WriteNotNull("`on`");
                    sqlStream.WriteNotNull(tableName);
                    sqlStream.WriteNotNull("`(");
                    bool isNext = false;
                    foreach (column column in columnCollection.Columns)
                    {
                        if (isNext) sqlStream.Write(',');
                        sqlStream.Write('`');
                        sqlStream.WriteNotNull(column.Name);
                        sqlStream.Write('`');
                        isNext = true;
                    }
                    sqlStream.WriteNotNull(");");
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
alter table `");
                        sqlStream.WriteNotNull(tableName);
                        sqlStream.WriteNotNull(@"` add ");
                        if (!column.IsNull && column.DefaultValue == null)
                        {
                            column.DefaultValue = column.DbType.getDefaultValue();
                            if (column.DefaultValue == null) column.IsNull = true;
                        }
                        appendColumn(sqlStream, column);
                        sqlStream.Write(';');
                        if (column.UpdateValue != null) isUpdateValue = true;
                    }
                    if (isUpdateValue)
                    {
                        sqlStream.WriteNotNull(@"
update `");
                        sqlStream.WriteNotNull(tableName);
                        sqlStream.WriteNotNull("` set ");
                        foreach (column column in columnCollection.Columns)
                        {
                            if (column.UpdateValue != null)
                            {
                                if (!isUpdateValue) sqlStream.Write(',');
                                sqlStream.WriteNotNull(column.Name);
                                sqlStream.Write('=');
                                sqlStream.WriteNotNull(column.UpdateValue);
                                isUpdateValue = false;
                            }
                        }
                        sqlStream.Write(';');
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
alter table `");
                        sqlStream.WriteNotNull(tableName);
                        sqlStream.WriteNotNull(@"` drop column ");
                        sqlStream.Write('`');
                        sqlStream.WriteNotNull(column.Name);
                        sqlStream.WriteNotNull("`;");
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
            using (DbCommand command = GetCommand(connection, "show tables;"))
            using (DbDataReader reader = command.ExecuteReader(CommandBehavior.Default))
            {
                while (reader.Read()) value.Add((string)reader[0]);
            }
            return value;
        }
        /// <summary>
        /// 索引列
        /// </summary>
        private sealed class indexColumn
        {
            /// <summary>
            /// 数据列
            /// </summary>
            public column Column;
            /// <summary>
            /// 是否不允许重复
            /// </summary>
            public columnCollection.type Type;
            /// <summary>
            /// 列序号
            /// </summary>
            public int Index;
            /// <summary>
            /// 是否可空
            /// </summary>
            public bool IsNull;
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
            if (isTable(connection, tableName, ref isExecute))
            {
                isExecute = 0;
                using (DbCommand command = GetCommand(connection, @"describe `" + tableName + @"`;
show index from `" + tableName + @"`;"))
                using (DbDataReader reader = command.ExecuteReader(CommandBehavior.Default))
                {
                    column identity = null;
                    Dictionary<hashString, column> columns = dictionary.CreateHashString<column>();
                    subArray<column> primaryKeys = default(subArray<column>);
                    Dictionary<hashString, list<indexColumn>> indexs = null;
                    while (reader.Read())
                    {
                        string key = (string)reader["Key"];
                        object defaultValue = reader["Default"];
                        column column = new column
                        {
                            Name = (string)reader["Field"],
                            DefaultValue = defaultValue == DBNull.Value ? null : (string)defaultValue,
                            IsNull = (string)reader["Null"] == "YES",
                        };
                        column.DbType = sqlDbType.FormatDbType((string)reader["Type"], out column.Size);
                        columns.Add(column.Name, column);
                        if (key == "PRI") primaryKeys.Add(column);
                    }
                    if (reader.NextResult())
                    {
                        indexs = dictionary.CreateHashString<list<indexColumn>>();
                        list<indexColumn> indexColumns;
                        while (reader.Read())
                        {
                            string name = (string)reader["Key_name"];
                            indexColumn indexColumn = new indexColumn
                            {
                                Column = columns[(string)reader["Column_name"]],
                                Index = (int)(long)reader["Seq_in_index"],
                                IsNull = (string)reader["Null"] == "YES"
                            };
                            hashString nameKey = name;
                            if (!indexs.TryGetValue(nameKey, out indexColumns))
                            {
                                indexs.Add(nameKey, indexColumns = new list<indexColumn>());
                                indexColumns.Add(indexColumn);
                                indexColumn.Type = (long)reader["Non_unique"] == 0 ? columnCollection.type.UniqueIndex : columnCollection.type.Index;
                            }
                            else indexColumns.Add(indexColumn);
                        }
                    }
                    isExecute = 1;
                    return new table
                    {
                        Columns = new columnCollection
                        {
                            Name = tableName,
                            Columns = columns.Values.getArray(),
                            Type = sql.columnCollection.type.None
                        },
                        Identity = identity,
                        PrimaryKey = primaryKeys.Count == 0 ? null : new columnCollection { Type = columnCollection.type.PrimaryKey, Columns = primaryKeys.ToArray() },
                        Indexs = indexs.getArray(index => new columnCollection
                        {
                            Name = index.Key.ToString(),
                            Type = index.Value[0].Type,
                            Columns = index.Value.sort((left, right) => left.Index - right.Index).getArray(column => column.Column)
                        })
                    };
                }
            }
            return null;
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        private sealed unsafe class importer<valueType, modelType> : IDisposable
            where valueType : class, modelType
            where modelType : class
        {
            /// <summary>
            /// SQL字符流
            /// </summary>
            private charStream sqlStream = new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1);
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
            /// 删除数据
            /// </summary>
            private Action insertHandle;
            /// <summary>
            /// 是否事物处理
            /// </summary>
            private bool isTransaction;
            /// <summary>
            /// 获取数据
            /// </summary>
            private importer()
            {
                insertHandle = insert;
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                pub.Dispose(ref sqlStream);
            }
            /// <summary>
            /// 删除数据
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
                fastCSharp.threading.task.Tiny.Add(insertHandle);
            }
            /// <summary>
            /// 删除数据
            /// </summary>
            private void insert()
            {
                subArray<valueType> valueArray = new subArray<valueType>(values.length);
                DbConnection connection = client.GetConnection(false);
                if (connection != null)
                {
                    string sql = null;
                    try
                    {
                        using (sqlStream)
                        {
                            foreach (valueType value in values)
                            {
                                if (client.insert(connection, sqlTool, value, fastCSharp.code.memberMap<modelType>.Default, sqlStream, ref sql))
                                {
                                    valueArray.Add(value);
                                }
                            }
                        }
                        if (valueArray.length == values.length) client.ConnectionPool.Push(ref connection);
                    }
                    catch (Exception error)
                    {
                        fastCSharp.log.Error.Add(error, sql, false);
                    }
                    if (connection != null) connection.Dispose();
                }
                else valueArray.Null();
                push(valueArray);
            }
            /// <summary>
            /// 连接池处理
            /// </summary>
            private void push(subArray<valueType> values)
            {
                Action<subArray<valueType>> onInserted = this.onInserted;
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
                    if (values.Count == 0)
                    {
                        if (isTransaction) fastCSharp.domainUnload.TransactionEnd();
                        onInserted(default(subArray<valueType>));
                    }
                    else
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
                sqlStream.UnsafeReset(buffer.Byte, SqlBufferSize << 1);
                if (sqlModel<modelType>.Identity != null)
                {
                    sqlStream.WriteNotNull("insert into`");
                    sqlStream.WriteNotNull(sqlTool.TableName);
                    sqlStream.WriteNotNull("`(");
                    sqlModel<modelType>.insert.GetColumnNames(sqlStream, memberMap);
                    sqlStream.WriteNotNull(")values(");
                    sqlModel<modelType>.insert.Insert(sqlStream, memberMap, value, Converter);
                    sqlStream.WriteNotNull(");");
                }
                else
                {
                    sqlStream.WriteNotNull("insert into`");
                    sqlStream.WriteNotNull(sqlTool.TableName);
                    sqlStream.WriteNotNull("`(");
                    sqlModel<modelType>.insert.GetColumnNames(sqlStream, memberMap);
                    sqlStream.WriteNotNull(")values(");
                    sqlModel<modelType>.insert.Insert(sqlStream, memberMap, value, Converter);
                    sqlStream.WriteNotNull(");");
                }
                return sqlStream.ToString();
            }
            finally { fastCSharp.sql.client.SqlBuffers.Push(ref buffer); }
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="connection"></param>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待插入数据</param>
        /// <param name="memberMap">目标成员位图</param>
        /// <param name="sqlStream"></param>
        /// <param name="sql"></param>
        /// <returns>是否成功</returns>
        private unsafe bool insert<valueType, modelType>
            (DbConnection connection, fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap, charStream sqlStream, ref string sql)
            where valueType : class, modelType
            where modelType : class
        {
            if (sqlModel<modelType>.Identity != null)
            {
                long identity = sqlTool.NextIdentity;
                sqlModel<modelType>.SetIdentity(value, identity);
                return executeNonQuery(connection, sql = insert(sqlTool, value, memberMap, sqlStream)) != ExecuteNonQueryError
                    && set<valueType, modelType>(connection, sql = getByIdentity(sqlTool, value, fastCSharp.emit.sqlModel<modelType>.MemberMap, sqlStream), value, fastCSharp.emit.sqlModel<modelType>.MemberMap);
            }
            if (sqlModel<modelType>.PrimaryKeys.Length != 0)
            {
                return executeNonQuery(connection, sql = insert(sqlTool, value, memberMap, sqlStream)) != ExecuteNonQueryError
                    && set<valueType, modelType>(connection, sql = getByPrimaryKey(sqlTool, value, fastCSharp.emit.sqlModel<modelType>.MemberMap, sqlStream), value, fastCSharp.emit.sqlModel<modelType>.MemberMap);
            }
            return ExecuteNonQuery(sql = insert(sqlTool, value, memberMap, sqlStream)) > 0;
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
        protected override unsafe bool insert<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap)
        {
            DbConnection connection = GetConnection(false);
            if (connection != null)
            {
                try
                {
                    string sql = null;
                    using (charStream sqlStream = new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1))
                    {
                        if (insert(connection, sqlTool, value, memberMap, sqlStream, ref sql))
                        {
                            pushConnection(ref connection, false);
                            if (sqlTool.IsLockWrite) sqlTool.CallOnInsertedLock(value);
                            return true;
                        }
                    }
                }
                finally
                {
                    if (connection != null) connection.Dispose();
                }
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
            private charStream sqlStream = new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1);
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
            private valueType value;
            /// <summary>
            /// 
            /// </summary>
            private fastCSharp.code.memberMap<modelType> memberMap;
            /// <summary>
            /// 获取数据回调
            /// </summary>
            private Action<valueType> onInserted;
            /// <summary>
            /// 删除数据
            /// </summary>
            private Action insertHandle;
            /// <summary>
            /// 是否事物处理
            /// </summary>
            private bool isTransaction;
            /// <summary>
            /// 获取数据
            /// </summary>
            private inserter()
            {
                insertHandle = insert;
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                pub.Dispose(ref sqlStream);
            }
            /// <summary>
            /// 删除数据
            /// </summary>
            /// <param name="client">SQL客户端操作</param>
            /// <param name="sqlTool">SQL操作工具</param>
            /// <param name="value">目标对象</param>
            /// <param name="memberMap"></param>
            /// <param name="onInserted">删除数据回调</param>
            /// <param name="isTransaction"></param>
            public void Insert(client client, fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap, ref Action<valueType> onInserted, bool isTransaction)
            {
                this.onInserted = onInserted;
                this.client = client;
                this.sqlTool = sqlTool;
                this.value = value;
                this.memberMap = memberMap;
                this.isTransaction = isTransaction;
                onInserted = null;
                fastCSharp.threading.task.Tiny.Add(insertHandle);
            }
            /// <summary>
            /// 删除数据
            /// </summary>
            private void insert()
            {
                DbConnection connection = client.GetConnection(false);
                if (connection != null)
                {
                    string sql = null;
                    try
                    {
                        using (sqlStream)
                        {
                            if (client.insert(connection, sqlTool, value, memberMap, sqlStream, ref sql))
                            {
                                client.ConnectionPool.Push(ref connection);
                            }
                            else value = null;
                        }
                    }
                    catch (Exception error)
                    {
                        value = null;
                        fastCSharp.log.Error.Add(error, sql, false);
                    }
                    if (connection != null) connection.Dispose();
                }
                else value = null;
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
                    client = null;
                    memberMap = null;
                    this.sqlTool = null;
                    this.onInserted = null;
                    this.value = null;
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
            if (inserter != null) inserter.Insert(this, sqlTool, value, memberMap, ref onInserted, isTransaction);
            else if (isTransaction) fastCSharp.domainUnload.TransactionEnd();
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待更新数据</param>
        /// <param name="memberMap">目标成员位图</param>
        /// <param name="sqlStream"></param>
        /// <returns>SQL字符串</returns>
        private unsafe string update<valueType, modelType>
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
                    sqlStream.WriteNotNull(" update `");
                    sqlStream.WriteNotNull(sqlTool.TableName);
                    sqlStream.WriteNotNull("` set ");
                    sqlModel<modelType>.update.Update(sqlStream, memberMap, value, Converter);
                    sqlStream.WriteNotNull(" where ");
                    sqlModel<modelType>.primaryKeyWhere.Where(sqlStream, value, Converter);
                    sqlStream.Write(';');
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
            DbConnection connection = GetConnection(false);
            if (connection != null)
            {
                using (fastCSharp.code.memberMap<modelType> selectMemberMap = sqlTool.GetSelectMemberMap(memberMap))
                using (charStream sqlStream = new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1))
                {
                    try
                    {
                        string sql = getByPrimaryKey(sqlTool, value, selectMemberMap, sqlStream);
                        valueType oldValue = fastCSharp.emit.constructor<valueType>.New();
                        if (set<valueType, modelType>(connection, sql, oldValue, selectMemberMap))
                        {
                            if (executeNonQuery(connection, update(sqlTool, value, memberMap, sqlStream)) != ExecuteNonQueryError
                                && set<valueType, modelType>(connection, sql, value, selectMemberMap))
                            {
                                pushConnection(ref connection, false);
                                sqlTool.CallOnUpdatedLock(value, oldValue, memberMap);
                                return oldValue;
                            }
                        }
                    }
                    finally
                    {
                        if (connection != null) connection.Dispose();
                    }
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
        /// <param name="sqlStream"></param>
        /// <returns>SQL语句</returns>
        private unsafe string update<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool
            , valueType value, ref fastCSharp.emit.sqlTable.updateExpression sqlExpression, charStream sqlStream)
            where valueType : class, modelType
            where modelType : class
        {
            pointer buffer = fastCSharp.sql.client.SqlBuffers.Get();
            try
            {
                using (sqlStream)
                {
                    sqlStream.UnsafeReset(buffer.Byte, SqlBufferSize << 1);
                    sqlStream.WriteNotNull(" update `");
                    sqlStream.WriteNotNull(sqlTool.TableName);
                    sqlStream.WriteNotNull("` set ");
                    sqlExpression.Update(sqlStream);
                    sqlStream.WriteNotNull(" where ");
                    sqlModel<modelType>.primaryKeyWhere.Where(sqlStream, value, Converter);
                    sqlStream.Write(';');
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
            DbConnection connection = GetConnection(false);
            if (connection != null)
            {
                using (fastCSharp.code.memberMap<modelType> selectMemberMap = sqlTool.GetSelectMemberMap(memberMap))
                using (charStream sqlStream = new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1))
                {
                    try
                    {
                        string sql = getByPrimaryKey(sqlTool, value, selectMemberMap, sqlStream);
                        valueType oldValue = fastCSharp.emit.constructor<valueType>.New();
                        if (set<valueType, modelType>(connection, sql, oldValue, selectMemberMap))
                        {
                            if (executeNonQuery(connection, update(sqlTool, value, ref sqlExpression, sqlStream)) != ExecuteNonQueryError)
                            {
                                if (set<valueType, modelType>(connection, sql, value, selectMemberMap))
                                {
                                    pushConnection(ref connection, false);
                                    sqlTool.CallOnUpdatedLock(value, oldValue, memberMap);
                                    return oldValue;
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (connection != null) connection.Dispose();
                    }
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
                        updater.Update(this, sqlTool, update(sqlTool, value, updater.MemberMap, updater.SqlStream), value, updateMemberMap, ref onUpdated, ref isTransaction, false);
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
                            updater.Update(this, sqlTool, update(sqlTool, value, ref updateExpression, updater.SqlStream), value, updateMemberMap, ref onUpdated, ref isTransaction, false);
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
        /// <param name="memberMap">目标成员位图</param>
        /// <param name="sqlStream"></param>
        /// <returns>SQL字符串</returns>
        private unsafe string updateByIdentity<valueType, modelType>
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
                    sqlStream.WriteNotNull(" update `");
                    sqlStream.WriteNotNull(sqlTool.TableName);
                    sqlStream.WriteNotNull("` set ");
                    sqlModel<modelType>.update.Update(sqlStream, memberMap, value, Converter);
                    sqlStream.WriteNotNull(" where ");
                    sqlStream.WriteNotNull(sqlModel<modelType>.Identity.Field.Name);
                    sqlStream.Write('=');
                    fastCSharp.number.ToString(sqlModel<modelType>.GetIdentity(value), sqlStream);
                    sqlStream.Write(';');
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
                pointer buffer = fastCSharp.sql.client.SqlBuffers.Get();
                try
                {
                    using (charStream sqlStream = new charStream(buffer.Char, fastCSharp.sql.client.SqlBufferSize))
                    {
                        sqlStream.WriteNotNull("select ");
                        sqlModel<modelType>.insert.GetColumnNames(sqlStream, selectMemberMap);
                        sqlStream.WriteNotNull(" from `");
                        sqlStream.WriteNotNull(sqlTool.TableName);
                        sqlStream.WriteNotNull("` where ");
                        sqlStream.WriteNotNull(sqlModel<modelType>.Identity.Field.Name);
                        sqlStream.Write('=');
                        fastCSharp.number.ToString(sqlModel<modelType>.GetIdentity(value), sqlStream);
                        sqlStream.WriteNotNull(" limit 0,1;");
                        string sql = sqlStream.ToString();
                        valueType oldValue = fastCSharp.emit.constructor<valueType>.New();
                        DbConnection connection = GetConnection(false);
                        if(connection != null)
                        {
                            try
                            {
                                if (set<valueType, modelType>(connection, sql, oldValue, selectMemberMap))
                                {
                                    sqlStream.Clear();
                                    sqlStream.WriteNotNull(" update `");
                                    sqlStream.WriteNotNull(sqlTool.TableName);
                                    sqlStream.WriteNotNull("` set ");
                                    sqlModel<modelType>.update.Update(sqlStream, memberMap, value, Converter);
                                    sqlStream.WriteNotNull(" where ");
                                    sqlStream.WriteNotNull(sqlModel<modelType>.Identity.Field.Name);
                                    sqlStream.Write('=');
                                    fastCSharp.number.ToString(sqlModel<modelType>.GetIdentity(value), sqlStream);
                                    sqlStream.Write(';');
                                    if (executeNonQuery(connection, sqlStream.ToString()) != ExecuteNonQueryError && set<valueType, modelType>(connection, sql, value, selectMemberMap))
                                    {
                                        pushConnection(ref connection, false);
                                        sqlTool.CallOnUpdatedLock(value, oldValue, memberMap);
                                        return oldValue;
                                    }
                                }
                            }
                            finally
                            {
                                if (connection != null) connection.Dispose();
                            }
                        }
                    }
                }
                finally { fastCSharp.sql.client.SqlBuffers.Push(ref buffer); }
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
        /// <param name="sqlStream"></param>
        /// <returns>SQL语句</returns>
        private unsafe string updateByIdentity<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool
            , valueType value, ref fastCSharp.emit.sqlTable.updateExpression sqlExpression, charStream sqlStream)
            where valueType : class, modelType
            where modelType : class
        {
            pointer buffer = fastCSharp.sql.client.SqlBuffers.Get();
            try
            {
                using (sqlStream)
                {
                    sqlStream.UnsafeReset(buffer.Byte, SqlBufferSize << 1);
                    sqlStream.WriteNotNull(" update `");
                    sqlStream.WriteNotNull(sqlTool.TableName);
                    sqlStream.WriteNotNull("` set ");
                    sqlExpression.Update(sqlStream);
                    sqlStream.WriteNotNull(" where ");
                    sqlStream.WriteNotNull(sqlModel<modelType>.Identity.Field.Name);
                    sqlStream.Write('=');
                    fastCSharp.number.ToString(sqlModel<modelType>.GetIdentity(value), sqlStream);
                    sqlStream.Write(';');
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
            DbConnection connection = GetConnection(false);
            if (connection != null)
            {
                try
                {
                    using (fastCSharp.code.memberMap<modelType> selectMemberMap = sqlTool.GetSelectMemberMap(memberMap))
                    using (charStream sqlStream = new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1))
                    {
                        string sql = getByIdentity(sqlTool, value, selectMemberMap, sqlStream);
                        valueType oldValue = fastCSharp.emit.constructor<valueType>.New();
                        if (set<valueType, modelType>(connection, sql, oldValue, selectMemberMap)
                            && executeNonQuery(connection, updateByIdentity(sqlTool, value, ref sqlExpression, sqlStream)) != ExecuteNonQueryError
                            && set<valueType, modelType>(connection, sql, value, selectMemberMap))
                        {
                            pushConnection(ref connection, false);
                            sqlTool.CallOnUpdatedLock(value, oldValue, memberMap);
                            return oldValue;
                        }
                    }
                }
                finally
                {
                    if (connection != null) connection.Dispose();
                }
            }
            return null;
        }
        /// <summary>
        /// 更新数据
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
            /// 更新数据
            /// </summary>
            private Action updateHandle;
            /// <summary>
            /// 是否事物处理
            /// </summary>
            private bool isTransaction;
            /// <summary>
            /// 是否自增值
            /// </summary>
            private bool isIdentity;
            /// <summary>
            /// 获取数据
            /// </summary>
            private updater()
            {
                updateHandle = update;
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
            /// <param name="isIdentity"></param>
            public void Update(client client, fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, string sql, valueType value, fastCSharp.code.memberMap<modelType> updateMemberMap, ref Action<valueType, valueType, fastCSharp.code.memberMap> onUpdated, ref bool isTransaction, bool isIdentity)
            {
                this.onUpdated = onUpdated;
                this.client = client;
                this.sqlTool = sqlTool;
                this.sql = sql;
                this.value = value;
                this.updateMemberMap = updateMemberMap;
                this.isTransaction = isTransaction;
                this.isIdentity = isIdentity;
                onUpdated = null;
                isTransaction = false;
                fastCSharp.threading.task.Tiny.Add(updateHandle);
            }
            /// <summary>
            /// 获取数据回调
            /// </summary>
            private void update()
            {
                valueType oldValue = null;
                DbConnection connection = client.GetConnection(false);
                if (connection != null)
                {
                    string sql = null;
                    try
                    {
                        using (SqlStream)
                        {
                            string select = sql = isIdentity ? client.getByIdentity(sqlTool, value, MemberMap, SqlStream) : client.getByPrimaryKey(sqlTool, value, MemberMap, SqlStream);
                            if (client.set<valueType, modelType>(connection, sql, oldValue, MemberMap)
                                && client.executeNonQuery(connection, sql = this.sql) != ExecuteNonQueryError
                                && client.set<valueType, modelType>(connection, sql = select, value, MemberMap))
                            {
                                client.ConnectionPool.Push(ref connection);
                            }
                            else value = null;
                        }
                    }
                    catch (Exception error)
                    {
                        value = null;
                        fastCSharp.log.Error.Add(error, sql, false);
                    }
                    if (connection != null) connection.Dispose();
                }
                else value = null;
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
                    client = null;
                    sql = null;
                    this.sqlTool = null;
                    this.onUpdated = null;
                    this.value = null;
                    this.updateMemberMap = null;
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
                        updater.Update(this, sqlTool, updateByIdentity(sqlTool, value, updater.MemberMap, updater.SqlStream), value, updateMemberMap, ref onUpdated, ref isTransaction, true);
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
                            updater.Update(this, sqlTool, updateByIdentity(sqlTool, value, ref updateExpression, updater.SqlStream), value, updateMemberMap, ref onUpdated, ref isTransaction, true);
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
        private unsafe string delete<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, charStream sqlStream)
            where valueType : class, modelType
            where modelType : class
        {
            pointer buffer = fastCSharp.sql.client.SqlBuffers.Get();
            try
            {
                sqlStream.UnsafeReset(buffer.Byte, SqlBufferSize << 1);
                sqlStream.WriteNotNull("delete from `");
                sqlStream.WriteNotNull(sqlTool.TableName);
                sqlStream.WriteNotNull("' where ");
                sqlModel<modelType>.primaryKeyWhere.Where(sqlStream, value, Converter);
                sqlStream.Write(';');
                return sqlStream.ToString();
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
            DbConnection connection = GetConnection(false);
            if (connection != null)
            {
                try
                {
                    using (charStream sqlStream = new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1))
                    {
                        if (set<valueType, modelType>(connection, getByPrimaryKey(sqlTool, value, sqlTool.SelectMemberMap, sqlStream), value, sqlTool.SelectMemberMap)
                            && executeNonQuery(connection, delete(sqlTool, value, sqlStream)) != ExecuteNonQueryError)
                        {
                            pushConnection(ref connection, false);
                            sqlTool.CallOnDeletedLock(value);
                            return true;
                        }
                    }
                }
                finally
                {
                    if (connection != null) connection.Dispose();
                }
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
            if (deleter != null) deleter.Delete(this, sqlTool, value, ref onDeleted, isTransaction, false);
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
        private unsafe string deleteByIdentity<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, charStream sqlStream)
            where valueType : class, modelType
            where modelType : class
        {
            pointer buffer = fastCSharp.sql.client.SqlBuffers.Get();
            try
            {
                sqlStream.UnsafeReset(buffer.Byte, SqlBufferSize << 1);
                sqlStream.WriteNotNull("delete from `");
                sqlStream.WriteNotNull(sqlTool.TableName);
                sqlStream.WriteNotNull("' where ");
                sqlStream.WriteNotNull(sqlModel<modelType>.Identity.Field.Name);
                sqlStream.Write('=');
                fastCSharp.number.ToString(sqlModel<modelType>.GetIdentity(value), sqlStream);
                sqlStream.Write(';');
                return sqlStream.ToString();
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
            DbConnection connection = GetConnection(false);
            if (connection != null)
            {
                try
                {
                    using (charStream sqlStream = new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1))
                    {
                        if (set<valueType, modelType>(connection, getByIdentity(sqlTool, value, sqlTool.SelectMemberMap, sqlStream), value, sqlTool.SelectMemberMap)
                            && executeNonQuery(connection, deleteByIdentity(sqlTool, value, sqlStream)) != ExecuteNonQueryError)
                        {
                            pushConnection(ref connection, false);
                            sqlTool.CallOnDeletedLock(value);
                            return true;
                        }
                    }
                }
                finally
                {
                    if (connection != null) connection.Dispose();
                }
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
            private charStream sqlStream = new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1);
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
            private valueType value;
            /// <summary>
            /// 获取数据回调
            /// </summary>
            private Action<valueType> onDeleted;
            /// <summary>
            /// 删除数据
            /// </summary>
            private Action deleteHandle;
            /// <summary>
            /// 是否事物处理
            /// </summary>
            private bool isTransaction;
            /// <summary>
            /// 是否自增值
            /// </summary>
            private bool isIdentity;
            /// <summary>
            /// 获取数据
            /// </summary>
            private deleter()
            {
                deleteHandle = delete;
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                pub.Dispose(ref sqlStream);
            }
            /// <summary>
            /// 删除数据
            /// </summary>
            /// <param name="client">SQL客户端操作</param>
            /// <param name="sqlTool">SQL操作工具</param>
            /// <param name="value">目标对象</param>
            /// <param name="onDeleted">删除数据回调</param>
            /// <param name="isTransaction"></param>
            /// <param name="isIdentity"></param>
            public void Delete(client client, fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, ref Action<valueType> onDeleted, bool isTransaction, bool isIdentity)
            {
                this.onDeleted = onDeleted;
                this.client = client;
                this.sqlTool = sqlTool;
                this.value = value;
                this.isTransaction = isTransaction;
                this.isIdentity = isIdentity;
                onDeleted = null;
                fastCSharp.threading.task.Tiny.Add(deleteHandle);
            }
            /// <summary>
            /// 删除数据
            /// </summary>
            private void delete()
            {
                DbConnection connection = client.GetConnection(false);
                if (connection != null)
                {
                    string sql = null;
                    try
                    {
                        using (sqlStream)
                        {
                            sql = isIdentity ? client.getByIdentity(sqlTool, value, sqlTool.SelectMemberMap, sqlStream) : client.getByPrimaryKey(sqlTool, value, sqlTool.SelectMemberMap, sqlStream);
                            if (client.set<valueType, modelType>(connection, sql, value, sqlTool.SelectMemberMap)
                                && client.executeNonQuery(connection, sql = isIdentity ? client.deleteByIdentity(sqlTool, value, sqlStream) : client.delete(sqlTool, value, sqlStream)) != ExecuteNonQueryError)
                            {
                                client.ConnectionPool.Push(ref connection);
                            }
                            else value = null;
                        }
                    }
                    catch (Exception error)
                    {
                        value = null;
                        fastCSharp.log.Error.Add(error, sql, false);
                    }
                    if (connection != null) connection.Dispose();
                }
                else value = null;
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
                    client = null;
                    this.sqlTool = null;
                    this.onDeleted = null;
                    this.value = null;
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
            if (deleter != null) deleter.Delete(this, sqlTool, value, ref onDeleted, isTransaction, true);
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
                    if (!sqlExpression.IsLogicConstantExpression) return converter.Convert(sqlExpression).Value ?? string.Empty;
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
                    sqlStream.WriteNotNull("select count(*)from`");
                    sqlStream.WriteNotNull(sqlTool.TableName);
                    sqlStream.WriteNotNull("`");
                    bool isCreatedIndex = false;
                    if (selectQuery<modelType>.WriteWhere(sqlTool, sqlStream, expression, ref isCreatedIndex))
                    {
                        sqlStream.Write(';');
                        return sqlStream.ToString();
                    }
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
        public unsafe override int Count<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, Expression<Func<modelType, bool>> expression)
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
        /// <param name="memberName">成员名称</param>
        /// <param name="sqlStream"></param>
        /// <returns></returns>
        private unsafe string getValue<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, selectQuery<modelType> query, string memberName, charStream sqlStream)
            where valueType : class, modelType
            where modelType : class
        {
            pointer buffer = fastCSharp.sql.client.SqlBuffers.Get();
            try
            {
                using (sqlStream)
                {
                    sqlStream.UnsafeReset(buffer.Byte, SqlBufferSize << 1);
                    sqlStream.WriteNotNull("select ");
                    sqlStream.WriteNotNull(memberName);
                    sqlStream.WriteNotNull(" from`");
                    sqlStream.WriteNotNull(sqlTool.TableName);
                    sqlStream.WriteNotNull("` ");
                    if (query == null) return sqlStream.ToString();
                    if (query.WriteWhere(sqlTool, sqlStream))
                    {
                        query.WriteOrder(sqlTool, sqlStream);
                        sqlStream.WriteNotNull(" limit ");
                        number.ToString(query.SkipCount, sqlStream);
                        sqlStream.WriteNotNull(",1;");
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
        /// <param name="memberName">成员名称</param>
        /// <param name="errorValue">错误值</param>
        /// <returns>对象集合</returns>
        internal override unsafe returnType GetValue<valueType, modelType, returnType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, selectQuery<modelType> query, string memberName, returnType errorValue)
        {
            string sql = getValue(sqlTool, query, memberName, new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1));
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
            /// 获取数据
            /// </summary>
            private Action getHandle;
            /// <summary>
            /// 获取数据
            /// </summary>
            private getter()
            {
                getHandle = get;
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
                fastCSharp.threading.task.Tiny.Add(getHandle);
            }
            /// <summary>
            /// 获取数据
            /// </summary>
            private void get()
            {
                DbConnection connection = client.GetConnection(false);
                if (connection != null)
                {
                    byte isExecute = 0;
                    try
                    {
                        using (DbCommand command = client.GetCommand(connection, sql))
                        {
                            object value = command.ExecuteScalar();
                            isExecute = 1;
                            if (value != null && value != DBNull.Value) this.value = (returnType)value;
                        }
                    }
                    catch (Exception error)
                    {
                        fastCSharp.log.Error.Add(error, sql, false);
                    }
                    finally
                    {
                        if (isExecute == 0) connection.Dispose();
                        else client.ConnectionPool.Push(ref connection);
                    }
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
                    client = null;
                    sql = null;
                    this.onGet = null;
                    this.value = default(returnType);
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
        /// <param name="sqlStream"></param>
        /// <returns></returns>
        private unsafe string select<valueType, modelType>
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
                    sqlStream.WriteNotNull("select ");
                    sqlModel<modelType>.GetNames(sqlStream, memberMap);
                    sqlStream.WriteNotNull(" from `");
                    sqlStream.WriteNotNull(sqlTool.TableName);
                    sqlStream.Write('`');
                    if (query == null)
                    {
                        sqlStream.Write(';');
                        return sqlStream.ToString();
                    }
                    sqlStream.Write(' ');
                    if (query.WriteWhere(sqlTool, sqlStream))
                    {
                        query.WriteOrder(sqlTool, sqlStream);
                        if ((query.GetCount | query.SkipCount) != 0)
                        {
                            sqlStream.WriteNotNull(" limit ");
                            number.ToString(query.SkipCount, sqlStream);
                            sqlStream.Write(',');
                            number.ToString(query.GetCount, sqlStream);
                        }
                        sqlStream.Write(';');
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
        protected unsafe override IEnumerable<valueType> selectPushMemberMap<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, selectQuery<modelType> query, fastCSharp.code.memberMap<modelType> memberMap)
        {
            string sql = select(sqlTool, query, memberMap, new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1));
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
        /// <param name="onGet"></param>
        public override void Select<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, selectQuery<modelType> query, fastCSharp.code.memberMap<modelType> memberMap, Action<subArray<valueType>> onGet)
        {
            if (onGet == null) log.Error.Throw(log.exceptionType.Null);
            try
            {
                selector<valueType, modelType> selector = selector<valueType, modelType>.Get();
                if (selector != null)
                {
                    selector.MemberMap.CopyFrom(fastCSharp.emit.sqlModel<modelType>.MemberMap);
                    if (memberMap != null && !memberMap.IsDefault) selector.MemberMap.And(memberMap);
                    string sql = select(sqlTool, query, selector.MemberMap, selector.SqlStream);
                    if (sql != null) selector.Get(this, sql, 0, ref onGet);
                    else typePool<selector<valueType, modelType>>.PushNotNull(selector);
                }
            }
            finally
            {
                if (onGet != null) onGet(default(subArray<valueType>));
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
                sqlStream.UnsafeReset(buffer.Byte, SqlBufferSize << 1);
                sqlStream.WriteNotNull("select ");
                sqlModel<modelType>.GetNames(sqlStream, memberMap);
                sqlStream.WriteNotNull(" from `");
                sqlStream.WriteNotNull(sqlTool.TableName);
                sqlStream.WriteNotNull("` where ");
                sqlStream.WriteNotNull(sqlModel<modelType>.Identity.Field.Name);
                sqlStream.Write('=');
                fastCSharp.number.ToString(sqlModel<modelType>.GetIdentity(value), sqlStream);
                sqlStream.WriteNotNull(" limit 0,1;");
                return sqlStream.ToString();
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
                using (charStream sqlStream = new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1))
                {
                    if (set<valueType, modelType>(getByIdentity(sqlTool, value, selectMemberMap, sqlStream), value, selectMemberMap)) return value;
                }
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
                    using (getter.SqlStream) getter.Get(this, getByIdentity(sqlTool, value, getter.MemberMap, getter.SqlStream), value, ref onGet);
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
        /// <param name="memberMap">成员位图</param>
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
                sqlStream.UnsafeReset(buffer.Byte, SqlBufferSize << 1);
                sqlStream.WriteNotNull("select ");
                sqlModel<modelType>.GetNames(sqlStream, memberMap);
                sqlStream.WriteNotNull(" from `");
                sqlStream.WriteNotNull(sqlTool.TableName);
                sqlStream.WriteNotNull("` where ");
                sqlModel<modelType>.primaryKeyWhere.Where(sqlStream, value, Converter);
                sqlStream.WriteNotNull(" limit 0,1;");
                return sqlStream.ToString();
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
                using (charStream sqlStream = new charStream((char*)fastCSharp.emit.pub.PuzzleValue, 1))
                {
                    if (set<valueType, modelType>(getByPrimaryKey(sqlTool, value, selectMemberMap, sqlStream), value, selectMemberMap)) return value;
                }
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
            /// 获取数据
            /// </summary>
            private Action getHandle;
            /// <summary>
            /// 获取数据
            /// </summary>
            private getter()
            {
                getHandle = get;
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
                fastCSharp.threading.task.Tiny.Add(getHandle);
            }
            /// <summary>
            /// 获取数据
            /// </summary>
            private void get()
            {
                DbConnection connection = client.GetConnection(false);
                if (connection != null)
                {
                    try
                    {
                        if (client.set<valueType, modelType>(sql, value, MemberMap))
                        {
                            client.ConnectionPool.Push(ref connection);
                        }
                        else value = null;
                    }
                    catch (Exception error)
                    {
                        value = null;
                        fastCSharp.log.Error.Add(error, sql, false);
                    }
                    if (connection != null) connection.Dispose();
                }
                else value = null;
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
                    client = null;
                    sql = null;
                    this.onGet = null;
                    this.value = null;
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
                    using (getter.SqlStream) getter.Get(this, getByPrimaryKey(sqlTool, value, getter.MemberMap, getter.SqlStream), value, ref onGet);
                }
            }
            finally
            {
                if (onGet != null) onGet(null);
            }
        }
    }
}
#endif