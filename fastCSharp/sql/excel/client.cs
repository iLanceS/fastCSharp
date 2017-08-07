using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data;
using System.Linq.Expressions;
using fastCSharp.code;
using fastCSharp.code.cSharp;
using fastCSharp.emit;
using System.Runtime.CompilerServices;
using System.Threading;

namespace fastCSharp.sql.excel
{
    /// <summary>
    /// Excel客户端(不做持久化设计,仅用于数据导入导出)
    /// </summary>
    public sealed class client : sql.client
    {
        /// <summary>
        /// 表格名称
        /// </summary>
        private const string schemaTableName = "Table_Name";
        /// <summary>
        /// Excel客户端
        /// </summary>
        /// <param name="connection">SQL连接信息</param>
        public client(sql.connection connection) : base(connection) { }
        /// <summary>
        /// 根据SQL连接类型获取SQL连接
        /// </summary>
        /// <param name="isAsynchronous">是否异步连接(不支持)</param>
        /// <returns>SQL连接</returns>
        internal override DbConnection GetConnection(bool isAsynchronous)
        {
            return ConnectionPool.Pop() ?? open(new OleDbConnection(this.Connection.Connection));
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
            DbCommand command = new OleDbCommand(sql, (OleDbConnection)connection);
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
            return new OleDbDataAdapter((OleDbCommand)command);
        }
        /// <summary>
        /// 获取数据表格
        /// </summary>
        /// <param name="tableName">表格名称</param>
        /// <param name="isName">是否处理表格名称</param>
        /// <returns>数据表格</returns>
        private DataTable getDataTable(string tableName, bool isName)
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
                            if (isName) table.TableName = tableName[tableName.Length - 1] == '$' ? tableName.Substring(0, tableName.Length - 1) : tableName;
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
        /// 获取数据表格
        /// </summary>
        /// <param name="tableName">表格名称</param>
        /// <returns>数据表格</returns>
        public override DataTable GetDataTable(string tableName)
        {
            return getDataTable(tableName, true);
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
        /// <returns>成功导入数量</returns>
        private int import(DbConnection connection, DataTable data)
        {
            using (OleDbDataAdapter adapter = new OleDbDataAdapter("select * from [" + data.TableName + "]", (OleDbConnection)connection))
            using (OleDbCommandBuilder command = new OleDbCommandBuilder(adapter))
            using (DataSet dataSet = new DataSet())
            {
                dataSet.Tables.Add(data);
                adapter.Update(dataSet, data.TableName);
                return data.Rows.Count;
            }
        }
        /// <summary>
        /// 导入数据集合
        /// </summary>
        /// <param name="connection">SQL连接</param>
        /// <param name="data">数据集合</param>
        /// <param name="batchSize">批处理数量,不支持</param>
        /// <param name="timeout">超时秒数,不支持</param>
        /// <returns>成功导入数量</returns>
        internal override int Import(DbConnection connection, DataTable data, int batchSize, int timeout)
        {
            return import(connection, data);
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
            using (DataTable table = ((OleDbConnection)connection).GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, null))
            {
                isExecute = 1;
                foreach (DataRow row in table.Rows)
                {
                    if (row[schemaTableName].ToString() == tableName) return true;
                }
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
            string name = table.Columns.Name;
            if (connection != null && name != null && name.Length != 0 && table.Columns != null && table.Columns.Columns.Length != 0)
            {
                pointer buffer = fastCSharp.sql.client.SqlBuffers.Get();
                try
                {
                    using (charStream sqlStream = new charStream(buffer.Char, fastCSharp.sql.client.SqlBufferSize))
                    {
                        sqlStream.SimpleWriteNotNull("create table ");
                        sqlStream.SimpleWriteNotNull(name);
                        sqlStream.SimpleWriteNotNull(" (");
                        bool isNext = false;
                        foreach (column column in table.Columns.Columns)
                        {
                            if (isNext) sqlStream.Write(',');
                            sqlStream.SimpleWriteNotNull(column.SqlName);
                            sqlStream.Write(' ');
                            sqlStream.Write(column.DbType.getSqlTypeName());
                            isNext = true;
                        }
                        sqlStream.Write(')');
                        return executeNonQuery(connection, sqlStream.ToString()) >= 0;
                    }
                }
                finally { fastCSharp.sql.client.SqlBuffers.Push(ref buffer); }
            }
            return false;
        }
        /// <summary>
        /// 成员信息转换为数据列
        /// </summary>
        /// <param name="type">成员类型</param>
        /// <param name="sqlMember">SQL成员信息</param>
        /// <returns>数据列</returns>
        internal override column getColumn(Type type, dataMember sqlMember)
        {
            SqlDbType sqlType = SqlDbType.NVarChar;
            int size = sqlMember.MaxStringLength;
            memberType memberType = sqlMember.DataType != null ? sqlMember.DataType : type;
            if (memberType.IsString)
            {
                if (size > 0) sqlType = SqlDbType.NVarChar;
                else
                {
                    sqlType = SqlDbType.NText;
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
        /// 是否支持删除表格
        /// </summary>
        internal override bool IsDropTable
        {
            get { return false; }
        }
        /// <summary>
        /// 删除表格(不支持)
        /// </summary>
        /// <param name="connection">SQL连接</param>
        /// <param name="tableName">表格名称</param>
        protected override bool dropTable(DbConnection connection, string tableName)
        {
            fastCSharp.log.Error.Throw(log.exceptionType.ErrorOperation);
            return false;
        }
        /// <summary>
        /// 是否支持索引
        /// </summary>
        internal override bool IsIndex
        {
            get { return false; }
        }
        /// <summary>
        /// 创建索引(不支持)
        /// </summary>
        /// <param name="connection">SQL连接</param>
        /// <param name="tableName">表格名称</param>
        /// <param name="columnCollection">索引列集合</param>
        internal override bool createIndex(DbConnection connection, string tableName, columnCollection columnCollection)
        {
            log.Error.Add("Excel 表格 " + tableName + " 不支持索引", new System.Diagnostics.StackFrame(), false);
            return false;
        }
        /// <summary>
        /// 是否支持新增列
        /// </summary>
        internal override bool IsAddField
        {
            get { return false; }
        }
        /// <summary>
        /// 新增列集合(不支持)
        /// </summary>
        /// <param name="connection">SQL连接</param>
        /// <param name="columnCollection">新增列集合</param>
        internal unsafe override bool addFields(DbConnection connection, columnCollection columnCollection)
        {
            fastCSharp.log.Error.Throw(log.exceptionType.ErrorOperation);
            return false;
        }
        /// <summary>
        /// 删除列集合(不支持)
        /// </summary>
        /// <param name="connection">SQL连接</param>
        /// <param name="columnCollection">删除列集合</param>
        internal unsafe override bool deleteFields(DbConnection connection, columnCollection columnCollection)
        {
            fastCSharp.log.Error.Throw(log.exceptionType.ErrorOperation);
            return false;
        }
        /// <summary>
        /// 获取表格名称集合
        /// </summary>
        /// <param name="connection">SQL连接</param>
        /// <returns>表格名称集合</returns>
        protected override subArray<string> getTableNames(DbConnection connection)
        {
            using (DataTable table = ((OleDbConnection)connection).GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, null))
            {
                DataRowCollection rows = table.Rows;
                subArray<string> names = new subArray<string>(rows.Count);
                foreach (DataRow row in rows) names.UnsafeAdd(row[schemaTableName].ToString());
                return names;
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
            using (DbCommand command = GetCommand(connection, "select top 1 * from [" + tableName + "]"))
            {
                using (DataSet dataSet = GetDataSet(command, ref isExecute))
                {
                    DataTable table = dataSet.Tables[0];
                    column identity = null;
                    subArray<column> columns = new subArray<column>(table.Columns.Count);
                    foreach (DataColumn dataColumn in table.Columns)
                    {
                        column column = new column
                        {
                            Name = dataColumn.ColumnName,
                            DbType = fastCSharp.sql.sqlDbType.formCSharpType(dataColumn.DataType),
                            Size = dataColumn.MaxLength,
                            DefaultValue = dataColumn.DefaultValue == null ? null : dataColumn.DefaultValue.ToString(),
                            IsNull = dataColumn.AllowDBNull,
                        };
                        if (dataColumn.AutoIncrement) identity = column;
                        columns.UnsafeAdd(column);
                    }
                    return new table
                    {
                        Columns = new columnCollection
                        {
                            Name = tableName,
                            Columns = columns.array,
                            Type = sql.columnCollection.type.None
                        },
                        Identity = identity
                    };
                }
            }
        }
        /// <summary>
        /// 导入数据
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
                        if (client.import(connection, sqlTool.GetDataTable(ref values)) != 0)
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
                //fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool = this.sqlTool;
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
                    if (isTransaction) fastCSharp.domainUnload.TransactionEnd();
                    onInserted(values);
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
        /// <returns>是否成功</returns>
        protected override bool insert<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap)
        {
            DbConnection connection = GetConnection(false);
            if (connection != null)
            {
                byte isExecute = 0;
                try
                {
                    subArray<valueType> values = subArray<valueType>.Unsafe(new valueType[] { value }, 0, 1);
                    import(connection, sqlTool.GetDataTable(ref values));
                    return true;
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
                finally
                {
                    if (isExecute == 0) connection.Dispose();
                    else pushConnection(ref connection, false);
                }
            }
            return false;
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        private sealed unsafe class inserter<valueType, modelType> : inserter
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
            private valueType value;
            /// <summary>
            /// 获取数据回调
            /// </summary>
            private Action<valueType> onInserted;
            /// <summary>
            /// 是否事物处理
            /// </summary>
            private bool isTransaction;
            /// <summary>
            /// 添加数据
            /// </summary>
            /// <param name="client">SQL客户端操作</param>
            /// <param name="sqlTool">SQL操作工具</param>
            /// <param name="value">目标对象</param>
            /// <param name="onInserted">删除数据回调</param>
            /// <param name="isTransaction"></param>
            public void Insert(client client, fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, ref Action<valueType> onInserted, bool isTransaction)
            {
                this.onInserted = onInserted;
                this.client = client;
                this.sqlTool = sqlTool;
                this.value = value;
                this.isTransaction = isTransaction;
                onInserted = null;
                fastCSharp.threading.task.Tiny.Add(this, threading.thread.callType.SqlClientInserter);
            }
            /// <summary>
            /// 添加数据
            /// </summary>
            internal override void Insert()
            {
                DbConnection connection = client.GetConnection(false);
                if (connection != null)
                {
                    try
                    {
                        subArray<valueType> values = subArray<valueType>.Unsafe(new valueType[] { value }, 0, 1);
                        if (client.import(connection, sqlTool.GetDataTable(ref values)) != 0)
                        {
                            client.ConnectionPool.Push(ref connection);
                        }
                        else value = null;
                    }
                    catch (Exception error)
                    {
                        value = null;
                        fastCSharp.log.Error.Add(error, sqlTool.TableName, false);
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
                bool isTransaction = this.isTransaction;
                try
                {
                    client = null;
                    this.sqlTool = null;
                    this.onInserted = null;
                    this.value = null;
                    typePool<inserter<valueType, modelType>>.PushNotNull(this);
                }
                finally
                {
                    if (isTransaction) fastCSharp.domainUnload.TransactionEnd();
                    onInserted(value);
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
            if (inserter != null) inserter.Insert(this, sqlTool, value, ref onInserted, isTransaction);
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
        /// <returns>更新前的数据对象,null表示失败</returns>
        protected override valueType update<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap)
        {
            log.Error.Throw(log.exceptionType.ErrorOperation);
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
            using (memberMap) onUpdated(null, null, null);
            log.Error.Throw(log.exceptionType.ErrorOperation);
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
        protected override valueType update<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool
            , valueType value, ref fastCSharp.emit.sqlTable.updateExpression sqlExpression, fastCSharp.code.memberMap<modelType> memberMap)
        {
            log.Error.Throw(log.exceptionType.ErrorOperation);
            return null;
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
            onUpdated(null, null, null);
            log.Error.Throw(log.exceptionType.ErrorOperation);
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
        protected override valueType updateByIdentity<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap)
        {
            log.Error.Throw(log.exceptionType.ErrorOperation);
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
        public override void UpdateByIdentity<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap, bool isIgnoreTransaction, Action<valueType, valueType, fastCSharp.code.memberMap> onUpdated)
        {
            using (memberMap) onUpdated(null, null, null);
            log.Error.Throw(log.exceptionType.ErrorOperation);
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
        protected override valueType updateByIdentity<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool
            , valueType value, ref fastCSharp.emit.sqlTable.updateExpression sqlExpression, fastCSharp.code.memberMap<modelType> memberMap)
        {
            log.Error.Throw(log.exceptionType.ErrorOperation);
            return null;
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
            onUpdated(null, null, null);
            log.Error.Throw(log.exceptionType.ErrorOperation);
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待删除数据</param>
        /// <returns>是否成功</returns>
        protected override bool delete<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value)
        {
            log.Error.Throw(log.exceptionType.ErrorOperation);
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
            log.Error.Throw(log.exceptionType.ErrorOperation);
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待删除数据</param>
        /// <returns>是否成功</returns>
        protected override bool deleteByIdentity<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value)
        {
            log.Error.Throw(log.exceptionType.ErrorOperation);
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
        protected override void deleteByIdentity<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, bool isIgnoreTransaction, ref Action<valueType> onDeleted)
        {
            log.Error.Throw(log.exceptionType.ErrorOperation);
        }
        /// <summary>
        /// 委托关联表达式转SQL表达式
        /// </summary>
        /// <param name="expression">委托关联表达式</param>
        /// <param name="logicConstantWhere">逻辑常量值</param>
        /// <returns>SQL表达式(null表示常量条件)</returns>
        public override string GetWhere(LambdaExpression expression, ref bool logicConstantWhere)
        {
            log.Error.Throw(log.exceptionType.ErrorOperation);
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
            log.Error.Throw(log.exceptionType.ErrorOperation);
            return default(keyValue<string, string>);
        }
        /// <summary>
        /// 委托关联表达式转SQL表达式
        /// </summary>
        /// <param name="expression">委托关联表达式</param>
        /// <returns>参数成员名称+SQL表达式</returns>
        public override keyValue<string, string> GetSql(LambdaExpression expression)
        {
            log.Error.Throw(log.exceptionType.ErrorOperation);
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
            log.Error.Throw(log.exceptionType.ErrorOperation);
            return default(keyValue<string, string>);
        }
        /// <summary>
        /// 获取记录数
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="expression">查询表达式</param>
        /// <returns>记录数,失败返回-1</returns>
        public override int Count<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, Expression<Func<modelType, bool>> expression)
        {
            log.Error.Throw(log.exceptionType.ErrorOperation);
            return 0;
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
            onGet(-1);
            log.Error.Throw(log.exceptionType.ErrorOperation);
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
        internal override returnType GetValue<valueType, modelType, returnType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, selectQuery<modelType> query, string memberSqlName, returnType errorValue)
        {
            log.Error.Throw(log.exceptionType.ErrorOperation);
            return errorValue;
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
            onGet(errorValue);
            log.Error.Throw(log.exceptionType.ErrorOperation);
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
            memberMap.Dispose();
            log.Error.Throw(log.exceptionType.ErrorOperation);
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
        /// <param name="onGet"></param>
        public override void Select<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, selectQuery<modelType> query, fastCSharp.code.memberMap<modelType> memberMap, Action<subArray<valueType>> onGet)
        {
            onGet(default(subArray<valueType>));
            log.Error.Throw(log.exceptionType.ErrorOperation);
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
        public override valueType GetByIdentity<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap)
        {
            log.Error.Throw(log.exceptionType.ErrorOperation);
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
            onGet(null);
            log.Error.Throw(log.exceptionType.ErrorOperation);
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
        public override valueType GetByPrimaryKey<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap)
        {
            log.Error.Throw(log.exceptionType.ErrorOperation);
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
        public override void GetByPrimaryKey<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap, Action<valueType> onGet)
        {
            onGet(null);
            log.Error.Throw(log.exceptionType.ErrorOperation);
        }
    }
}