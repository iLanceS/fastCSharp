using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using fastCSharp.code;
using fastCSharp.code.cSharp;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading;
using fastCSharp.reflection;
using fastCSharp.threading;
using fastCSharp.emit;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql
{
    /// <summary>
    /// SQL客户端操作
    /// </summary>
    public abstract class client
    {
        /// <summary>
        /// 执行错误返回值
        /// </summary>
        public const int ExecuteNonQueryError = int.MinValue;
        /// <summary>
        /// SQL字符串缓冲区大小
        /// </summary>
        internal const int SqlBufferSize = 1 << 10;
        /// <summary>
        /// SQL字符串缓冲区
        /// </summary>
        internal static readonly unmanagedPool SqlBuffers = unmanagedPool.GetOrCreate(SqlBufferSize << 1);
        /// <summary>
        /// SQL连接信息
        /// </summary>
        internal connection Connection;
        /// <summary>
        /// SQL常量转换处理
        /// </summary>
        internal expression.constantConverter Converter;
        /// <summary>
        /// 字符串转换
        /// </summary>
        protected Action<charStream, string> stringConverter;
        /// <summary>
        /// 同步连接池
        /// </summary>
        internal readonly connectionPool ConnectionPool;
        /// <summary>
        /// 异步连接池
        /// </summary>
        internal connectionPool AsynchronousConnectionPool;
        /// <summary>
        /// SQL客户端操作
        /// </summary>
        /// <param name="connection">SQL连接信息</param>
        protected client(connection connection)
        {
            AsynchronousConnectionPool = ConnectionPool = connectionPool.Get(connection.Type, connection.Connection);
            Connection = connection;
            Converter = Enum<fastCSharp.sql.type, fastCSharp.sql.typeInfo>.Array((byte)Connection.Type).Converter;
            stringConverter = Converter[typeof(string)];
        }
        /// <summary>
        /// 打开连接
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        protected DbConnection open(DbConnection connection)
        {
            Exception openError = null;
            try
            {
                connection.Open();
                return connection;
            }
            catch (Exception error)
            {
                connection.Dispose();
                openError = error;
            }
            fastCSharp.log.Error.Add(openError, null, true);
            return null;
        }
        /// <summary>
        /// 根据SQL连接类型获取SQL连接
        /// </summary>
        /// <param name="isAsynchronous">是否异步连接</param>
        /// <returns>SQL连接</returns>
        internal abstract DbConnection GetConnection(bool isAsynchronous);
        /// <summary>
        /// 连接池处理
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="isAsynchronous"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void pushConnection(ref DbConnection connection, bool isAsynchronous)
        {
            if (isAsynchronous) AsynchronousConnectionPool.Push(ref connection);
            else ConnectionPool.Push(ref connection);
        }
        /// <summary>
        /// 获取SQL命令
        /// </summary>
        /// <param name="connection">SQL连接</param>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="type">SQL命令类型</param>
        /// <returns>SQL命令</returns>
        public abstract DbCommand GetCommand
            (DbConnection connection, string sql, SqlParameter[] parameters = null, CommandType type = CommandType.Text);
        /// <summary>
        /// 获取数据适配器
        /// </summary>
        /// <param name="command">SQL命令</param>
        /// <returns>数据适配器</returns>
        protected abstract DbDataAdapter getAdapter(DbCommand command);
        /// <summary>
        /// 获取数据集并关闭SQL命令
        /// </summary>
        /// <param name="command">SQL命令</param>
        /// <param name="isExecute">SQL连接执行是否成功</param>
        /// <returns>数据集</returns>
        public DataSet GetDataSet(DbCommand command, ref byte isExecute)
        {
            using (command)
            {
                DbDataAdapter adapter = getAdapter(command);
                if (adapter != null)
                {
                    DataSet data = new DataSet();
                    adapter.Fill(data);
                    isExecute = 1;
                    return data;
                }
                return null;
            }
        }
        /// <summary>
        /// 获取数据表格并关闭SQL命令
        /// </summary>
        /// <param name="command">SQL命令</param>
        /// <param name="isExecute">SQL连接执行是否成功</param>
        /// <returns>数据表格</returns>
        public DataTable GetDataTable(DbCommand command, ref byte isExecute)
        {
            using (command)
            {
                DbDataAdapter adapter = getAdapter(command);
                if (adapter != null)
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    isExecute = 1;
                    return table;
                }
                return null;
            }
        }
        /// <summary>
        /// 获取数据表格
        /// </summary>
        /// <param name="tableName">表格名称</param>
        /// <returns>数据表格</returns>
        public abstract DataTable GetDataTable(string tableName);
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="errorValue">错误值</param>
        /// <returns>数据</returns>
        public valueType GetValue<valueType>(string sql, valueType errorValue)
        {
            if (!string.IsNullOrEmpty(sql))
            {
                DbConnection connection = GetConnection(false);
                if (connection != null)
                {
                    byte isExecute = 0;
                    try
                    {
                        using (DbCommand command = GetCommand(connection, sql))
                        {
                            object value = command.ExecuteScalar();
                            isExecute = 1;
                            if (value != null && value != DBNull.Value) return (valueType)value;
                        }
                    }
                    catch (Exception error)
                    {
                        fastCSharp.log.Error.Add(error, sql, false);
                    }
                    finally
                    {
                        if (isExecute == 0) connection.Dispose();
                        else pushConnection(ref connection, false);
                    }
                }
            }
            return errorValue;
        }
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="connection">SQL连接</param>
        /// <param name="sql">SQL语句</param>
        /// <returns>受影响的行数,错误返回ExecuteNonQueryError</returns>
        protected int executeNonQuery(DbConnection connection, string sql)
        {
            try
            {
                using (DbCommand command = GetCommand(connection, sql)) return command.ExecuteNonQuery();
            }
            catch (Exception error)
            {
                fastCSharp.log.Error.Add(error, sql, false);
            }
            return ExecuteNonQueryError;
        }
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>受影响的行数,错误返回ExecuteNonQueryError</returns>
        public int ExecuteNonQuery(string sql)
        {
            if (!string.IsNullOrEmpty(sql))
            {
                DbConnection connection = GetConnection(false);
                if (connection != null)
                {
                    try
                    {
                        int count = executeNonQuery(connection, sql);
                        if (count != ExecuteNonQueryError) pushConnection(ref connection, false);
                        return count;
                    }
                    finally
                    {
                        if (connection != null) connection.Dispose();
                    }
                }
            }
            return ExecuteNonQueryError;
        }
        /// <summary>
        /// 是否支持DataTable导入
        /// </summary>
        protected virtual bool isImport
        {
            get { return false; }
        }
        /// <summary>
        /// 导入数据集合
        /// </summary>
        /// <param name="connection">SQL连接</param>
        /// <param name="data">数据集合</param>
        /// <param name="batchSize">批处理数量</param>
        /// <param name="timeout">超时秒数</param>
        /// <returns>成功导入数量</returns>
        internal abstract int Import(DbConnection connection, DataTable data, int batchSize, int timeout);
        /// <summary>
        /// 导入数据集合
        /// </summary>
        /// <param name="data">数据集合</param>
        /// <param name="batchSize">批处理数量,0表示默认数量</param>
        /// <param name="timeout">超时秒数,0表示不设置超时</param>
        /// <returns>成功导入数量</returns>
        public int Import(DataTable data, int batchSize = 0, int timeout = 0)
        {
            if (data != null && data.Rows.Count != 0)
            {
                DbConnection connection = GetConnection(false);
                if (connection != null)
                {
                    try
                    {
                        int count = Import(connection, data, batchSize, timeout);
                        if (count != 0)
                        {
                            pushConnection(ref connection, false);
                            return count;
                        }
                    }
                    finally
                    {
                        if (connection != null) connection.Dispose();
                    }
                }
            }
            return 0;
        }
        /// <summary>
        /// 判断表格是否存在
        /// </summary>
        /// <param name="connection">SQL连接</param>
        /// <param name="tableName">表格名称</param>
        /// <param name="isExecute">SQL连接执行是否成功</param>
        /// <returns>表格是否存在</returns>
        protected abstract bool isTable(DbConnection connection, string tableName, ref byte isExecute);
        /// <summary>
        /// 判断表格是否存在
        /// </summary>
        /// <param name="tableName">表格名称</param>
        /// <returns>表格是否存在</returns>
        public bool IsTable(string tableName)
        {
            if (!string.IsNullOrEmpty(tableName))
            {
                DbConnection connection = GetConnection(false);
                if (connection != null)
                {
                    byte isExecute = 0;
                    try
                    {
                        return isTable(connection, tableName, ref isExecute);
                    }
                    finally
                    {
                        if (isExecute == 0) connection.Dispose();
                        else pushConnection(ref connection, false);
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 写入索引名称
        /// </summary>
        /// <param name="sqlStream">SQL语句流</param>
        /// <param name="tableName">表格名称</param>
        /// <param name="columnCollection">索引列集合</param>
        internal static void AppendIndexName(charStream sqlStream, string tableName, columnCollection columnCollection)
        {
            if (string.IsNullOrEmpty(columnCollection.Name))
            {
                sqlStream.SimpleWriteNotNull("ix_");
                sqlStream.SimpleWriteNotNull(tableName);
                foreach (column column in columnCollection.Columns)
                {
                    sqlStream.Write('_');
                    sqlStream.SimpleWriteNotNull(column.Name);
                }
            }
            else sqlStream.SimpleWriteNotNull(columnCollection.Name);
        }
        /// <summary>
        /// 创建表格
        /// </summary>
        /// <param name="connection">SQL连接</param>
        /// <param name="table">表格信息</param>
        internal abstract bool createTable(DbConnection connection, table table);
        /// <summary>
        /// 创建表格
        /// </summary>
        /// <param name="table">表格信息</param>
        internal bool CreateTable(table table)
        {
            if (table != null && table.Columns != null && !string.IsNullOrEmpty(table.Columns.Name) && table.Columns.Columns.length() != 0)
            {
                DbConnection connection = GetConnection(false);
                if (connection != null)
                {
                    try
                    {
                        if (createTable(connection, table))
                        {
                            pushConnection(ref connection, false);
                            return true;
                        }
                    }
                    finally
                    {
                        if (connection != null) connection.Dispose();
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 成员信息转换为数据列
        /// </summary>
        /// <param name="type">成员类型</param>
        /// <param name="sqlMember">SQL成员信息</param>
        /// <returns>数据列</returns>
        internal abstract column getColumn(Type type, dataMember sqlMember);
        /// <summary>
        /// 成员信息转换为数据列
        /// </summary>
        /// <param name="name">成员名称</param>
        /// <param name="type">成员类型</param>
        /// <param name="sqlMember">SQL成员信息</param>
        /// <returns>数据列</returns>
        internal column GetColumn(string name, Type type, dataMember sqlMember)
        {
            column column = fastCSharp.code.typeAttribute.GetAttribute<fastCSharp.emit.sqlColumn>(type, false, false) == null ? getColumn(type, sqlMember) : new column { SqlColumnType = type };
            column.Name = name;
            return column;
        }
        /// <summary>
        /// SQL列转换
        /// </summary>
        internal struct sqlColumnBuilder
        {
            /// <summary>
            /// SQL客户端操作
            /// </summary>
            public client Client;
            /// <summary>
            /// 数据列集合
            /// </summary>
            public subArray<column> Columns;
            /// <summary>
            /// SQL列转换
            /// </summary>
            /// <param name="column">数据列</param>
            public void Append(column column)
            {
                if (column.SqlColumnType == null) Columns.Add(column);
                else
                {
                    foreach (column sqlColumn in get(column.SqlColumnType))
                    {
                        column copyColumn = sqlColumn.Clone();
                        copyColumn.Name = column.Name + "_" + copyColumn.Name;
                        Columns.Add(copyColumn);
                    }
                }
            }
            /// <summary>
            /// 添加SQL列类型
            /// </summary>
            /// <param name="type">SQL列类型</param>
            private void append(Type type)
            {
                foreach (keyValue<memberIndex, dataMember> member in fastCSharp.code.cSharp.sqlModel.GetMemberIndexs(type, fastCSharp.code.typeAttribute.GetAttribute<fastCSharp.emit.sqlColumn>(type, false, false)))
                {
                    column column = Client.GetColumn(member.Key.Member.Name, member.Key.Type, member.Value);
                    if (column.SqlColumnType == null) Columns.Add(column);
                    else
                    {
                        foreach (column sqlColumn in getNoLock(column.SqlColumnType))
                        {
                            column copyColumn = sqlColumn.Clone();
                            copyColumn.Name = column.Name + "_" + copyColumn.Name;
                            Columns.Add(copyColumn);
                        }
                    }
                }
            }
            /// <summary>
            /// 获取SQL列转换集合
            /// </summary>
            /// <param name="type">SQL列类型</param>
            /// <returns>SQL列转换集合</returns>
            private column[] getNoLock(Type type)
            {
                column[] columns;
                if (!sqlColumns.TryGetValue(type, out columns))
                {
                    int index = Columns.length;
                    append(type);
                    sqlColumns.Add(type, columns = subArray<column>.Unsafe(Columns.array, index, Columns.length - index).GetArray());
                    Columns.UnsafeSetLength(index);
                }
                return columns;
            }
            /// <summary>
            /// 获取SQL列转换集合
            /// </summary>
            /// <param name="type">SQL列类型</param>
            /// <returns>SQL列转换集合</returns>
            private column[] get(Type type)
            {
                column[] columns;
                Monitor.Enter(sqlColumnLock);
                try
                {
                    columns = getNoLock(type);
                }
                finally { Monitor.Exit(sqlColumnLock); }
                return columns;
            }
            /// <summary>
            /// 获取SQL列转换集合
            /// </summary>
            /// <param name="type">SQL列类型</param>
            /// <param name="client">SQL客户端操作</param>
            /// <returns>SQL列转换集合</returns>
            public static column[] Get(Type type, client client)
            {
                column[] columns;
                Monitor.Enter(sqlColumnLock);
                try
                {
                    if (!sqlColumns.TryGetValue(type, out columns))
                    {
                        sqlColumnBuilder sqlColumn = new sqlColumnBuilder { Client = client };
                        sqlColumn.append(type);
                        sqlColumns.Add(type, columns = sqlColumn.Columns.ToArray());
                    }
                }
                finally { Monitor.Exit(sqlColumnLock); }
                return columns;
            }
            /// <summary>
            /// SQL列转换类型集合
            /// </summary>
            private static Dictionary<Type, column[]> sqlColumns = dictionary.CreateOnly<Type, column[]>();
            /// <summary>
            /// SQL列转换类型集合访问锁
            /// </summary>
            private static readonly object sqlColumnLock = new object();
        }
        /// <summary>
        /// SQL列转换
        /// </summary>
        /// <param name="table">表格信息</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void ToSqlColumn(table table)
        {
            table.Columns.Columns = ToSqlColumn(table.Columns.Columns);
        }
        /// <summary>
        /// SQL列转换
        /// </summary>
        /// <param name="columns">数据列集合</param>
        /// <returns></returns>
        internal column[] ToSqlColumn(column[] columns)
        {
            if (columns.any(column => column.SqlColumnType != null))
            {
                sqlColumnBuilder sqlColumn = new sqlColumnBuilder { Client = this };
                foreach (column column in columns) sqlColumn.Append(column);
                return sqlColumn.Columns.ToArray();
            }
            return columns;
        }
        /// <summary>
        /// 是否支持删除表格
        /// </summary>
        internal virtual bool IsDropTable
        {
            get { return true; }
        }
        /// <summary>
        /// 删除表格
        /// </summary>
        /// <param name="connection">SQL连接</param>
        /// <param name="tableName">表格名称</param>
        protected abstract bool dropTable(DbConnection connection, string tableName);
        /// <summary>
        /// 删除表格
        /// </summary>
        /// <param name="tableName">表格名称</param>
        public bool DropTable(string tableName)
        {
            if (!string.IsNullOrEmpty(tableName))
            {
                DbConnection connection = GetConnection(false);
                if (connection != null)
                {
                    try
                    {
                        if (dropTable(connection, tableName))
                        {
                            pushConnection(ref connection, false);
                            return true;
                        }
                    }
                    finally
                    {
                        if (connection != null) connection.Dispose();
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 是否支持索引
        /// </summary>
        internal virtual bool IsIndex
        {
            get { return true; }
        }
        /// <summary>
        /// 创建索引
        /// </summary>
        /// <param name="connection">SQL连接</param>
        /// <param name="tableName">表格名称</param>
        /// <param name="columnCollection">索引列集合</param>
        internal abstract bool createIndex(DbConnection connection, string tableName, columnCollection columnCollection);
        /// <summary>
        /// 创建索引
        /// </summary>
        /// <param name="tableName">表格名称</param>
        /// <param name="columnCollection">索引列集合</param>
        internal bool CreateIndex(string tableName, columnCollection columnCollection)
        {
            if (!string.IsNullOrEmpty(tableName) && columnCollection != null && columnCollection.Columns.length() != 0)
            {
                DbConnection connection = GetConnection(false);
                if (connection != null)
                {
                    try
                    {
                        if (createIndex(connection, tableName, columnCollection))
                        {
                            pushConnection(ref connection, false);
                            return true;
                        }
                    }
                    finally
                    {
                        if (connection != null) connection.Dispose();
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 是否支持新增列
        /// </summary>
        internal virtual bool IsAddField
        {
            get { return true; }
        }
        /// <summary>
        /// 新增列集合
        /// </summary>
        /// <param name="connection">SQL连接</param>
        /// <param name="columnCollection">新增列集合</param>
        internal abstract bool addFields(DbConnection connection, columnCollection columnCollection);
        /// <summary>
        /// 新增列集合
        /// </summary>
        /// <param name="columnCollection">新增列集合</param>
        internal bool AddFields(columnCollection columnCollection)
        {
            if (columnCollection != null && columnCollection.Columns.length() != 0 && !string.IsNullOrEmpty(columnCollection.Name))
            {
                DbConnection connection = GetConnection(false);
                if (connection != null)
                {
                    try
                    {
                        if (addFields(connection, columnCollection))
                        {
                            pushConnection(ref connection, false);
                            return true;
                        }
                    }
                    finally
                    {
                        if (connection != null) connection.Dispose();
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 删除列集合
        /// </summary>
        /// <param name="connection">SQL连接</param>
        /// <param name="columnCollection">删除列集合</param>
        internal abstract bool deleteFields(DbConnection connection, columnCollection columnCollection);
        /// <summary>
        /// 删除列集合
        /// </summary>
        /// <param name="columnCollection">删除列集合</param>
        internal bool DeleteFields(columnCollection columnCollection)
        {
            if (columnCollection != null && columnCollection.Columns.length() != 0 && !string.IsNullOrEmpty(columnCollection.Name))
            {
                DbConnection connection = GetConnection(false);
                if (connection != null)
                {
                    try
                    {
                        if (deleteFields(connection, columnCollection))
                        {
                            pushConnection(ref connection, false);
                            return true;
                        }
                    }
                    finally
                    {
                        if (connection != null) connection.Dispose();
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 获取表格名称集合
        /// </summary>
        /// <param name="connection">SQL连接</param>
        /// <returns>表格名称集合</returns>
        protected abstract subArray<string> getTableNames(DbConnection connection);
        /// <summary>
        /// 获取表格名称集合
        /// </summary>
        /// <returns>表格名称集合</returns>
        public subArray<string> GetTableNames()
        {
            DbConnection connection = GetConnection(false);
            if (connection != null)
            {
                try
                {
                    subArray<string> names = getTableNames(connection);
                    pushConnection(ref connection, false);
                    return names;
                }
                finally
                {
                    if (connection != null) connection.Dispose();
                }
            }
            return default(subArray<string>);
        }
        /// <summary>
        /// 根据表格名称获取表格信息
        /// </summary>
        /// <param name="connection">SQL连接</param>
        /// <param name="tableName">表格名称</param>
        /// <param name="isExecute">SQL连接执行是否成功</param>
        /// <returns>表格信息</returns>
        internal abstract table getTable(DbConnection connection, string tableName, ref byte isExecute);
        /// <summary>
        /// 根据表格名称获取表格信息
        /// </summary>
        /// <param name="tableName">表格名称</param>
        /// <returns>表格信息</returns>
        internal table GetTable(string tableName)
        {
            if (!string.IsNullOrEmpty(tableName))
            {
                DbConnection connection = GetConnection(false);
                if (connection != null)
                {
                    byte isExecute = 0;
                    try
                    {
                        return getTable(connection, tableName, ref isExecute);
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
        /// 对象转换成SQL字符流
        /// </summary>
        private static readonly charStream toStringStream = new charStream(unmanagedStreamBase.DefaultLength);
        /// <summary>
        /// 对象转换成SQL字符流访问锁
        /// </summary>
        private static readonly object toStringLock = new object();
        /// <summary>
        /// 对象转换成SQL字符串
        /// </summary>
        /// <param name="value">对象</param>
        /// <returns>SQL字符串</returns>
        public virtual string ToString(object value)
        {
            if (value != null)
            {
                string stringValue = null;
                Action<charStream, object> toString = Converter[value.GetType()];
                if (toString == null)
                {
                    fastCSharp.emit.sqlTable.ISqlString sqlString = value as fastCSharp.emit.sqlTable.ISqlString;
                    stringValue = sqlString == null? sqlString.ToSqlString() : fastCSharp.emit.jsonSerializer.ObjectToJson(value);
                    Monitor.Enter(toStringLock);
                    try
                    {
                        toStringStream.UnsafeSetLength(0);
                        stringConverter(toStringStream, stringValue);
                        stringValue = toStringStream.ToString();
                    }
                    finally { Monitor.Exit(toStringLock); }
                }
                else
                {
                    Monitor.Enter(toStringLock);
                    try
                    {
                        toStringStream.UnsafeSetLength(0);
                        toString(toStringStream, value);
                        stringValue = toStringStream.ToString();
                    }
                    finally { Monitor.Exit(toStringLock); }
                }
                return stringValue;
            }
            return "null";
        }
        /// <summary>
        /// 执行SQL语句并更新成员
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="value">目标对象</param>
        /// <param name="oldValue">更新前的目标对象</param>
        /// <param name="memberMap">目标成员位图</param>
        /// <returns>更新是否成功</returns>
        protected bool set<valueType, modelType>(string sql, valueType value, valueType oldValue, fastCSharp.code.memberMap<modelType> memberMap)
            where valueType : class, modelType
            where modelType : class
        {
            DbConnection connection = GetConnection(false);
            if (connection != null)
            {
                byte isExecute = 0;
                try
                {
                    using (DbCommand command = GetCommand(connection, sql))
                    using (DbDataReader reader = command.ExecuteReader(CommandBehavior.Default))
                    {
                        if (reader.Read())
                        {
                            sqlModel<modelType>.set.Set(reader, oldValue, memberMap);
                            if (reader.NextResult() && reader.Read())
                            {
                                sqlModel<modelType>.set.Set(reader, value, memberMap);
                                isExecute = 1;
                                return true;
                            }
                        }
                    }
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, sql, false);
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
        /// 执行SQL语句并更新成员
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="connection">SQL连接</param>
        /// <param name="sql">SQL语句</param>
        /// <param name="value">目标对象</param>
        /// <param name="memberMap">目标成员位图</param>
        /// <returns>更新是否成功</returns>
        protected bool set<valueType, modelType>
            (DbConnection connection, string sql, valueType value, fastCSharp.code.memberMap<modelType> memberMap)
            where valueType : class, modelType
            where modelType : class
        {
            using (DbCommand command = GetCommand(connection, sql))
            {
                try
                {
                    using (DbDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        if (reader.Read())
                        {
                            sqlModel<modelType>.set.Set(reader, value, memberMap);
                            return true;
                        }
                    }
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, sql, false);
                }
            }
            return false;
        }
        /// <summary>
        /// 执行SQL语句并更新成员
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="value">目标对象</param>
        /// <param name="memberMap">目标成员位图</param>
        /// <returns>更新是否成功</returns>
        protected bool set<valueType, modelType>
            (string sql, valueType value, fastCSharp.code.memberMap<modelType> memberMap)
            where valueType : class, modelType
            where modelType : class
        {
            DbConnection connection = GetConnection(false);
            if (connection != null)
            {
                try
                {
                    if (set<valueType, modelType>(connection, sql, value, memberMap))
                    {
                        pushConnection(ref connection, false);
                        return true;
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
        /// 导入数据
        /// </summary>
        internal abstract class inserter
        {
            /// <summary>
            /// 导入数据
            /// </summary>
            internal abstract void Insert();
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">表格模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="values">待插入数据集合</param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
        /// <returns>插入的数据集合,失败返回null</returns>
        public subArray<valueType> Insert<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType[] values, bool isIgnoreTransaction)
            where valueType : class, modelType
            where modelType : class
        {
            subArray<valueType> valueArray = new subArray<valueType>(values);
            return Insert(sqlTool, ref valueArray, isIgnoreTransaction);
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">表格模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="values">待插入数据集合</param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
        /// <returns>插入的数据集合,失败返回null</returns>
        public subArray<valueType> Insert<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, ref subArray<valueType> values, bool isIgnoreTransaction)
            where valueType : class, modelType
            where modelType : class
        {
            switch (values.length)
            {
                case 0: return values;
                case 1:
                    if (Insert(sqlTool, values[0], null, isIgnoreTransaction)) return values;
                    break;
                default:
                    if (isImport)
                    {
                        if (values.GetCount(value => sqlModel<modelType>.verify.Verify(value, fastCSharp.code.memberMap<modelType>.Default, sqlTool)) == values.length && sqlTool.CallOnInsert(ref values))
                        {
                            sqlTool.SetIdentity(ref values);
                            DataTable dataTable = sqlTool.GetDataTable(ref values);
                            if (!isIgnoreTransaction && sqlTool.IsInsertTransaction)
                            {
                                if (fastCSharp.domainUnload.TransactionStart(false))
                                {
                                    try
                                    {
                                        insertLock(sqlTool, ref values, dataTable);
                                        return values;
                                    }
                                    finally { fastCSharp.domainUnload.TransactionEnd(); }
                                }
                            }
                            else
                            {
                                insertLock(sqlTool, ref values, dataTable);
                                return values;
                            }
                        }
                    }
                    else
                    {
                        subArray<valueType> newValues = new subArray<valueType>(values.length);
                        foreach (valueType value in values)
                        {
                            if (Insert(sqlTool, value, null, false)) newValues.UnsafeAdd(value);
                        }
                        return newValues;
                    }
                    break;
            }
            return default(subArray<valueType>);
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">表格模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="values">待插入数据集合</param>
        /// <param name="dataTable">待插入数据集合</param>
        private void insertLock<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, ref subArray<valueType> values, DataTable dataTable)
            where valueType : class, modelType
            where modelType : class
        {
            if (sqlTool.IsLockWrite)
            {
                Monitor.Enter(sqlTool.Lock);
                try
                {
                    if (sqlTool.CallOnInsertLock(ref values)) insert(sqlTool, ref values, dataTable);
                }
                finally { Monitor.Exit(sqlTool.Lock); }
            }
            else insert(sqlTool, ref values, dataTable);
            foreach (valueType value in values) sqlTool.CallOnInserted(value);
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">表格模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="values">待插入数据集合</param>
        /// <param name="dataTable">待插入数据集合</param>
        /// <returns>成功导入数量</returns>
        private int insert<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, ref subArray<valueType> values, DataTable dataTable)
            where valueType : class, modelType
            where modelType : class
        {
            DbConnection connection = GetConnection(false);
            if (connection != null)
            {
                try
                {
                    Import(connection, dataTable, 0, 0);
                    pushConnection(ref connection, false);
                    if (sqlTool.IsLockWrite)
                    {
                        foreach (valueType value in values) sqlTool.CallOnInsertedLock(value);
                        return values.length;
                    }
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
                finally
                {
                    if (connection != null) connection.Dispose();
                }
            }
            return 0;
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
        public void Insert<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType[] values, bool isIgnoreTransaction, Action<subArray<valueType>> onInserted)
            where valueType : class, modelType
            where modelType : class
        {
            Insert(sqlTool, new subArray<valueType>(values), isIgnoreTransaction, onInserted);
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
        public abstract void Insert<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, subArray<valueType> values, bool isIgnoreTransaction, Action<subArray<valueType>> onInserted)
            where valueType : class, modelType
            where modelType : class;
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待插入数据</param>
        /// <param name="memberMap">目标成员位图</param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
        /// <returns>是否成功</returns>
        public bool Insert<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap, bool isIgnoreTransaction)
            where valueType : class, modelType
            where modelType : class
        {
            if (memberMap == null) memberMap = fastCSharp.code.memberMap<modelType>.Default;
            if (sqlModel<modelType>.verify.Verify(value, memberMap, sqlTool) && sqlTool.CallOnInsert(value))
            {
                if (!isIgnoreTransaction && sqlTool.IsInsertTransaction)
                {
                    if (fastCSharp.domainUnload.TransactionStart(false))
                    {
                        try
                        {
                            return insertLock(sqlTool, value, memberMap);
                        }
                        finally { fastCSharp.domainUnload.TransactionEnd(); }
                    }
                }
                else return insertLock(sqlTool, value, memberMap);
            }
            return false;
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
        private bool insertLock<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap)
            where valueType : class, modelType
            where modelType : class
        {
            bool isInsert = false;
            if (sqlTool.IsLockWrite)
            {
                Monitor.Enter(sqlTool.Lock);
                try
                {
                    if (sqlTool.CallOnInsertLock(value)) isInsert = insert(sqlTool, value, memberMap);
                }
                finally { Monitor.Exit(sqlTool.Lock); }
            }
            else isInsert = insert(sqlTool, value, memberMap);
            if (isInsert) sqlTool.CallOnInserted(value);
            return isInsert;
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
        protected abstract bool insert<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap)
            where valueType : class, modelType
            where modelType : class;
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
        public void Insert<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap, bool isIgnoreTransaction, Action<valueType> onInserted)
            where valueType : class, modelType
            where modelType : class
        {
            if (onInserted == null) log.Error.Throw(log.exceptionType.Null);
            try
            {
                if (memberMap == null) memberMap = fastCSharp.code.memberMap<modelType>.Default;
                if (sqlModel<modelType>.verify.Verify(value, memberMap, sqlTool) && sqlTool.CallOnInsert(value))
                {
                    insert(sqlTool, value, memberMap, isIgnoreTransaction, ref onInserted);
                }
            }
            finally
            {
                if (onInserted != null) onInserted(null);
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
        protected abstract void insert<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap, bool isIgnoreTransaction, ref Action<valueType> onInserted)
            where valueType : class, modelType
            where modelType : class;
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待更新数据自增id标识</param>
        /// <param name="updateExpression">待更新数据</param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
        /// <returns>是否成功</returns>
        public bool Update<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value
            , ref fastCSharp.emit.sqlTable.updateExpression updateExpression, bool isIgnoreTransaction)
            where valueType : class, modelType
            where modelType : class
        {
            using (fastCSharp.code.memberMap<modelType> memberMap = updateExpression.CreateMemberMap<modelType>())
            {
                if (!memberMap.IsDefault)
                {
                    if (sqlModel<modelType>.Identity != null) memberMap.ClearMember(sqlModel<modelType>.Identity.MemberMapIndex);
                    if (sqlTool.CallOnUpdate(value, memberMap))
                    {
                        if (!isIgnoreTransaction && sqlTool.IsUpdateTransaction)
                        {
                            if (fastCSharp.domainUnload.TransactionStart(false))
                            {
                                try
                                {
                                    return updateLock(sqlTool, value, ref updateExpression, memberMap);
                                }
                                finally { fastCSharp.domainUnload.TransactionEnd(); }
                            }
                        }
                        else return updateLock(sqlTool, value, ref updateExpression, memberMap);
                    }
                }
            }
            return false;
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
        /// <returns>是否成功</returns>
        private bool updateLock<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool
            , valueType value, ref fastCSharp.emit.sqlTable.updateExpression sqlExpression, fastCSharp.code.memberMap<modelType> memberMap)
            where valueType : class, modelType
            where modelType : class
        {
            valueType oldValue = null;
            if (sqlTool.IsLockWrite)
            {
                Monitor.Enter(sqlTool.Lock);
                try
                {
                    if (sqlTool.CallOnUpdateLock(value, memberMap)) oldValue = update(sqlTool, value, ref sqlExpression, memberMap);
                }
                finally { Monitor.Exit(sqlTool.Lock); }
            }
            else oldValue = update(sqlTool, value, ref sqlExpression, memberMap);
            if (oldValue != null)
            {
                sqlTool.CallOnUpdated(value, oldValue, memberMap);
                return true;
            }
            return false;
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
        protected abstract valueType update<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool
            , valueType value, ref fastCSharp.emit.sqlTable.updateExpression sqlExpression, fastCSharp.code.memberMap<modelType> memberMap)
            where valueType : class, modelType
            where modelType : class;
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
        public abstract void Update<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value
            , ref fastCSharp.emit.sqlTable.updateExpression updateExpression, bool isIgnoreTransaction, Action<valueType, valueType, fastCSharp.code.memberMap> onUpdated)
            where valueType : class, modelType
            where modelType : class;
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待更新数据</param>
        /// <param name="memberMap">目标成员位图</param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
        /// <returns>是否成功</returns>
        public bool Update<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap, bool isIgnoreTransaction)
            where valueType : class, modelType
            where modelType : class
        {
            using (fastCSharp.code.memberMap<modelType> updateMemberMap = sqlTool.GetMemberMapClearIdentity(memberMap))
            {
                if (sqlModel<modelType>.verify.Verify(value, updateMemberMap, sqlTool) && sqlTool.CallOnUpdate(value, updateMemberMap))
                {
                    if (!isIgnoreTransaction && sqlTool.IsUpdateTransaction)
                    {
                        if (fastCSharp.domainUnload.TransactionStart(false))
                        {
                            try
                            {
                                return updateLock(sqlTool, value, updateMemberMap);
                            }
                            finally { fastCSharp.domainUnload.TransactionEnd(); }
                        }
                    }
                    else return updateLock(sqlTool, value, updateMemberMap);
                }
            }
            return false;
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待更新数据</param>
        /// <param name="memberMap">目标成员位图</param>
        /// <returns>是否成功</returns>
        private bool updateLock<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap)
            where valueType : class, modelType
            where modelType : class
        {
            valueType oldValue = null;
            if (sqlTool.IsLockWrite)
            {
                Monitor.Enter(sqlTool.Lock);
                try
                {
                    if (sqlTool.CallOnUpdateLock(value, memberMap)) oldValue = update(sqlTool, value, memberMap);
                }
                finally { Monitor.Exit(sqlTool.Lock); }
            }
            else oldValue = update(sqlTool, value, memberMap);
            if (oldValue != null)
            {
                sqlTool.CallOnUpdated(value, oldValue, memberMap);
                return true;
            }
            return false;
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
        protected abstract valueType update<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap)
            where valueType : class, modelType
            where modelType : class;
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
        public abstract void Update<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap, bool isIgnoreTransaction, Action<valueType, valueType, fastCSharp.code.memberMap> onUpdated)
            where valueType : class, modelType
            where modelType : class;
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待更新数据自增id标识</param>
        /// <param name="updateExpression">待更新数据</param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
        /// <param name="cacheLock">缓存对象，作为访问锁使用</param>
        /// <returns>是否成功</returns>
        public bool UpdateByIdentity<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value
            , ref fastCSharp.emit.sqlTable.updateExpression updateExpression, object cacheLock, bool isIgnoreTransaction)
            where valueType : class, modelType
            where modelType : class
        {
            using (fastCSharp.code.memberMap<modelType> memberMap = updateExpression.CreateMemberMap<modelType>())
            {
                if (!memberMap.IsDefault)
                {
                    memberMap.ClearMember(sqlModel<modelType>.Identity.MemberMapIndex);
                    if (sqlTool.CallOnUpdate(value, memberMap))
                    {
                        if (!isIgnoreTransaction && sqlTool.IsUpdateTransaction)
                        {
                            if (fastCSharp.domainUnload.TransactionStart(false))
                            {
                                try
                                {
                                    return updateByIdentityLock(sqlTool, value, ref updateExpression, memberMap, cacheLock);
                                }
                                finally { fastCSharp.domainUnload.TransactionEnd(); }
                            }
                        }
                        else return updateByIdentityLock(sqlTool, value, ref updateExpression, memberMap, cacheLock);
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待更新数据自增id标识</param>
        /// <param name="updateExpression">待更新数据</param>
        /// <param name="memberMap">目标成员位图</param>
        /// <param name="cacheLock">缓存对象，作为访问锁使用</param>
        /// <returns>是否成功</returns>
        private bool updateByIdentityLock<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool
            , valueType value, ref fastCSharp.emit.sqlTable.updateExpression updateExpression, fastCSharp.code.memberMap<modelType> memberMap, object cacheLock)
            where valueType : class, modelType
            where modelType : class
        {
            valueType oldValue = null;
            if (sqlTool.IsLockWrite)
            {
                if (cacheLock == null) cacheLock = sqlTool.Lock;
                Monitor.Enter(cacheLock);
                try
                {
                    if (sqlTool.CallOnUpdateLock(value, memberMap)) oldValue = updateByIdentity(sqlTool, value, ref updateExpression, memberMap);
                }
                finally { Monitor.Exit(cacheLock); }
            }
            else oldValue = updateByIdentity(sqlTool, value, ref updateExpression, memberMap);
            if (oldValue != null)
            {
                sqlTool.CallOnUpdated(value, oldValue, memberMap);
                return true;
            }
            return false;
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
        protected abstract valueType updateByIdentity<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool
            , valueType value, ref fastCSharp.emit.sqlTable.updateExpression sqlExpression, fastCSharp.code.memberMap<modelType> memberMap)
            where valueType : class, modelType
            where modelType : class;
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
        public abstract void UpdateByIdentity<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value
            , ref fastCSharp.emit.sqlTable.updateExpression updateExpression, bool isIgnoreTransaction, Action<valueType, valueType, fastCSharp.code.memberMap> onUpdated)
            where valueType : class, modelType
            where modelType : class;
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待更新数据</param>
        /// <param name="memberMap">目标成员位图</param>
        /// <param name="cacheLock">缓存对象，作为访问锁使用</param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
        /// <returns>是否成功</returns>
        public bool UpdateByIdentity<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap, object cacheLock, bool isIgnoreTransaction)
            where valueType : class, modelType
            where modelType : class
        {
            using (fastCSharp.code.memberMap<modelType> updateMemberMap = sqlTool.GetMemberMapClearIdentity(memberMap))
            {
                if (sqlModel<modelType>.verify.Verify(value, updateMemberMap, sqlTool) && sqlTool.CallOnUpdate(value, updateMemberMap))
                {
                    if (!isIgnoreTransaction && sqlTool.IsUpdateTransaction)
                    {
                        if (fastCSharp.domainUnload.TransactionStart(false))
                        {
                            try
                            {
                                return updateByIdentityLock(sqlTool, value, updateMemberMap, cacheLock);
                            }
                            finally { fastCSharp.domainUnload.TransactionEnd(); }
                        }
                    }
                    else return updateByIdentityLock(sqlTool, value, updateMemberMap, cacheLock);
                }
            }
            return false;
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待更新数据</param>
        /// <param name="memberMap">目标成员位图</param>
        /// <param name="cacheLock">缓存对象，作为访问锁使用</param>
        /// <returns>是否成功</returns>
        private bool updateByIdentityLock<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap, object cacheLock)
            where valueType : class, modelType
            where modelType : class
        {
            valueType oldValue = null;
            if (sqlTool.IsLockWrite)
            {
                if (cacheLock == null) cacheLock = sqlTool.Lock;
                Monitor.Enter(cacheLock);
                try
                {
                    if (sqlTool.CallOnUpdateLock(value, memberMap)) oldValue = updateByIdentity(sqlTool, value, memberMap);
                }
                finally { Monitor.Exit(cacheLock); }
            }
            else oldValue = updateByIdentity(sqlTool, value, memberMap);
            if (oldValue != null)
            {
                sqlTool.CallOnUpdated(value, oldValue, memberMap);
                return true;
            }
            return false;
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
        protected abstract valueType updateByIdentity<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap)
            where valueType : class, modelType
            where modelType : class;
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
        public abstract void UpdateByIdentity<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap, bool isIgnoreTransaction, Action<valueType, valueType, fastCSharp.code.memberMap> onUpdated)
            where valueType : class, modelType
            where modelType : class;
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待删除数据</param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
        /// <returns>是否成功</returns>
        public bool Delete<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, bool isIgnoreTransaction)
            where valueType : class, modelType
            where modelType : class
        {
            if (sqlTool.CallOnDelete(value))
            {
                if (!isIgnoreTransaction && sqlTool.IsDeleteTransaction)
                {
                    if (fastCSharp.domainUnload.TransactionStart(false))
                    {
                        try
                        {
                            return deleteLock(sqlTool, value);
                        }
                        finally { fastCSharp.domainUnload.TransactionEnd(); }
                    }
                }
                else return deleteLock(sqlTool, value);
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
        /// <returns>是否成功</returns>
        private bool deleteLock<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value)
            where valueType : class, modelType
            where modelType : class
        {
            bool isDelete = false;
            if (sqlTool.IsLockWrite)
            {
                Monitor.Enter(sqlTool.Lock);
                try
                {
                    if (sqlTool.CallOnDeleteLock(value)) isDelete = delete(sqlTool, value);
                }
                finally { Monitor.Exit(sqlTool.Lock); }
            }
            else isDelete = delete(sqlTool, value);
            if (isDelete)
            {
                sqlTool.CallOnDeleted(value);
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
        /// <returns>是否成功</returns>
        protected abstract bool delete<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value)
            where valueType : class, modelType
            where modelType : class;
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待删除数据</param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
        /// <param name="onDeleted">是否成功</param>
        public void Delete<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, bool isIgnoreTransaction, Action<valueType> onDeleted)
            where valueType : class, modelType
            where modelType : class
        {
            if (onDeleted == null) log.Error.Throw(log.exceptionType.Null);
            try
            {
                if (sqlTool.CallOnDelete(value))
                {
                    delete(sqlTool, value, isIgnoreTransaction, ref onDeleted);
                }
            }
            finally
            {
                if (onDeleted != null) onDeleted(null);
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
        protected abstract void delete<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, bool isIgnoreTransaction, ref Action<valueType> onDeleted)
            where valueType : class, modelType
            where modelType : class;
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待删除数据</param>
        /// <param name="isCacheLock">待删除数据是否作为访问锁使用的缓存对象</param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
        /// <returns>是否成功</returns>
        public bool DeleteByIdentity<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, bool isCacheLock, bool isIgnoreTransaction)
            where valueType : class, modelType
            where modelType : class
        {
            if (sqlTool.CallOnDelete(value))
            {
                if (!isIgnoreTransaction && sqlTool.IsDeleteTransaction)
                {
                    if (fastCSharp.domainUnload.TransactionStart(false))
                    {
                        try
                        {
                            return deleteByIdentityLock(sqlTool, value, isCacheLock);
                        }
                        finally { fastCSharp.domainUnload.TransactionEnd(); }
                    }
                }
                else return deleteByIdentityLock(sqlTool, value, isCacheLock);
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
        /// <param name="isCacheLock">待删除数据是否作为访问锁使用的缓存对象</param>
        /// <returns>是否成功</returns>
        private bool deleteByIdentityLock<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, bool isCacheLock)
            where valueType : class, modelType
            where modelType : class
        {
            bool isDelete = false;
            if (sqlTool.IsLockWrite)
            {
                object cacheLock = isCacheLock ? value : sqlTool.Lock;
                Monitor.Enter(cacheLock);
                try
                {
                    if (sqlTool.CallOnDeleteLock(value)) isDelete = deleteByIdentity(sqlTool, value);
                }
                finally { Monitor.Exit(cacheLock); }
            }
            else isDelete = deleteByIdentity(sqlTool, value);
            if (isDelete)
            {
                sqlTool.CallOnDeleted(value);
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
        /// <returns>是否成功</returns>
        protected abstract bool deleteByIdentity<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value)
            where valueType : class, modelType
            where modelType : class;
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">待删除数据</param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
        /// <param name="onDeleted">是否成功</param>
        public void DeleteByIdentity<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, bool isIgnoreTransaction, Action<valueType> onDeleted)
            where valueType : class, modelType
            where modelType : class
        {
            try
            {
                if (sqlTool.CallOnDelete(value))
                {
                    deleteByIdentity(sqlTool, value, isIgnoreTransaction, ref onDeleted);
                }
            }
            finally
            {
                if (onDeleted != null) onDeleted(null);
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
        protected abstract void deleteByIdentity<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, bool isIgnoreTransaction, ref Action<valueType> onDeleted)
            where valueType : class, modelType
            where modelType : class;
        /// <summary>
        /// 委托关联表达式转SQL表达式
        /// </summary>
        /// <param name="expression">委托关联表达式</param>
        /// <param name="logicConstantWhere">逻辑常量值</param>
        /// <returns>SQL表达式(null表示常量条件)</returns>
        public abstract string GetWhere(LambdaExpression expression, ref bool logicConstantWhere);
        /// <summary>
        /// 委托关联表达式转SQL表达式
        /// </summary>
        /// <param name="expression">委托关联表达式</param>
        /// <param name="sqlStream">SQL表达式流</param>
        /// <param name="logicConstantWhere">逻辑常量值</param>
        /// <returns>参数成员名称</returns>
        internal abstract keyValue<string, string> GetWhere(LambdaExpression expression, charStream sqlStream, ref bool logicConstantWhere);
        /// <summary>
        /// 委托关联表达式转SQL表达式
        /// </summary>
        /// <param name="expression">委托关联表达式</param>
        /// <returns>参数成员名称+SQL表达式</returns>
        public abstract keyValue<string, string> GetSql(LambdaExpression expression);
        /// <summary>
        /// 委托关联表达式转SQL表达式
        /// </summary>
        /// <param name="expression">委托关联表达式</param>
        /// <param name="sqlStream">SQL表达式流</param>
        /// <returns>参数成员名称+参数成员SQL 名称</returns>
        internal abstract keyValue<string, string> GetSql(LambdaExpression expression, charStream sqlStream);
        /// <summary>
        /// 委托关联表达式转SQL列
        /// </summary>
        /// <param name="expression">委托关联表达式</param>
        /// <returns>SQL列</returns>
        public object GetSqlColumn(LambdaExpression expression)
        {
            if (expression != null)
            {
                fastCSharp.sql.expression.LambdaExpression sqlExpression = fastCSharp.sql.expression.LambdaExpression.convert(expression);
                try
                {
                    fastCSharp.sql.expression.Expression body = sqlExpression.Body;
                    if (body.IsConstant) return ((fastCSharp.sql.expression.ConstantExpression)body).Value;
                }
                finally { sqlExpression.PushPool(); }
            }
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
        public abstract int Count<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, Expression<Func<modelType, bool>> expression)
            where valueType : class, modelType
            where modelType : class;
        /// <summary>
        /// 获取记录数
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="expression">查询表达式</param>
        /// <param name="onGet">记录数,失败返回-1</param>
        public abstract void Count<valueType, modelType>(fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, Expression<Func<modelType, bool>> expression, Action<int> onGet)
            where valueType : class, modelType
            where modelType : class;
        /// <summary>
       /// 执行SQL语句并返回数据集合
       /// </summary>
       /// <typeparam name="valueType">数据类型</typeparam>
       /// <typeparam name="modelType">模型类型</typeparam>
       /// <param name="sql">SQL语句</param>
       /// <param name="skipCount">跳过记录数</param>
       /// <param name="memberMap">成员位图</param>
       /// <returns>数据集合</returns>
        protected IEnumerable<valueType> selectPushMemberMap<valueType, modelType>(string sql, int skipCount, fastCSharp.code.memberMap<modelType> memberMap)
            where valueType : class, modelType
            where modelType : class
        {
            using (memberMap)
            {
                DbConnection connection = GetConnection(false);
                if (connection != null)
                {
                    byte isExecute = 0;
                    try
                    {
                        using (DbCommand command = GetCommand(connection, sql))
                        {
                            DbDataReader reader = null;
                            try
                            {
                                reader = command.ExecuteReader(CommandBehavior.SingleResult);
                            }
                            catch (Exception error)
                            {
                                fastCSharp.log.Error.Add(error, sql, false);
                            }
                            if (reader != null)
                            {
                                using (reader)
                                {
                                    while (skipCount != 0 && reader.Read()) --skipCount;
                                    if (skipCount == 0)
                                    {
                                        while (reader.Read())
                                        {
                                            valueType value = fastCSharp.emit.constructor<valueType>.New();
                                            sqlModel<modelType>.set.Set(reader, value, memberMap);
                                            yield return value;
                                        }
                                    }
                                    isExecute = 1;
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (isExecute == 0) connection.Dispose();
                        else pushConnection(ref connection, false);
                    }
                }
            }
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        internal abstract class selector
        {
            /// <summary>
            /// 获取数据
            /// </summary>
            internal abstract void GetData();
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <typeparam name="modelType"></typeparam>
        internal sealed unsafe class selector<valueType, modelType> : selector, IDisposable
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
            /// 跳过记录数
            /// </summary>
            private int skipCount;
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
                fastCSharp.threading.task.Tiny.Add(this, thread.callType.SqlClientSelector);
            }
            /// <summary>
            /// 获取数据
            /// </summary>
            internal override void GetData()
            {
                subArray<valueType> values = default(subArray<valueType>);
                DbConnection connection = client.GetConnection(false);
                if (connection != null)
                {
                    try
                    {
                        using (DbCommand command = client.GetCommand(connection, sql))
                        using (DbDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult))
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
                            client.ConnectionPool.Push(ref connection);
                        }
                    }
                    catch (Exception error)
                    {
                        values.Null();
                        fastCSharp.log.Error.Add(error, sql, false);
                    }
                    if (connection != null) connection.Dispose();
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
                    client = null;
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
        internal abstract returnType GetValue<valueType, modelType, returnType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, selectQuery<modelType> query, string memberSqlName, returnType errorValue)
            where valueType : class, modelType
            where modelType : class;
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
        internal abstract void GetValue<valueType, modelType, returnType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, selectQuery<modelType> query, string memberName, returnType errorValue, Action<returnType> onGet)
            where valueType : class, modelType
            where modelType : class;
        /// <summary>
        /// 查询对象集合
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="query">查询信息</param>
        /// <param name="memberMap">成员位图</param>
        /// <returns>对象集合</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public IEnumerable<valueType> Select<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, selectQuery<modelType> query, fastCSharp.code.memberMap<modelType> memberMap)
            where valueType : class, modelType
            where modelType : class
        {
            fastCSharp.code.memberMap<modelType> selectMemberMap = fastCSharp.emit.sqlModel<modelType>.CopyMemberMap;
            if (memberMap != null && !memberMap.IsDefault) selectMemberMap.And(memberMap);
            return selectPushMemberMap(sqlTool, query, selectMemberMap);
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
        protected abstract IEnumerable<valueType> selectPushMemberMap<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, selectQuery<modelType> query, fastCSharp.code.memberMap<modelType> memberMap)
            where valueType : class, modelType
            where modelType : class;
        /// <summary>
        /// 查询对象集合
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="query">查询信息</param>
        /// <param name="memberMap">成员位图</param>
        /// <param name="onGet"></param>
        public abstract void Select<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, selectQuery<modelType> query, fastCSharp.code.memberMap<modelType> memberMap, Action<subArray<valueType>> onGet)
            where valueType : class, modelType
            where modelType : class;
        /// <summary>
        /// 查询对象
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">匹配成员值</param>
        /// <param name="memberMap">成员位图</param>
        /// <returns>对象集合</returns>
        public abstract valueType GetByIdentity<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap)
            where valueType : class, modelType
            where modelType : class;
        /// <summary>
        /// 查询对象
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">匹配成员值</param>
        /// <param name="memberMap">成员位图</param>
        /// <param name="onGet"></param>
        public abstract void GetByIdentity<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap, Action<valueType> onGet)
            where valueType : class, modelType
            where modelType : class;
        /// <summary>
        /// 查询对象
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">匹配成员值</param>
        /// <param name="memberMap">成员位图</param>
        /// <returns>对象集合</returns>
        public abstract valueType GetByPrimaryKey<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap)
            where valueType : class, modelType
            where modelType : class;
        /// <summary>
        /// 查询对象
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        /// <param name="sqlTool">SQL操作工具</param>
        /// <param name="value">匹配成员值</param>
        /// <param name="memberMap">成员位图</param>
        /// <param name="onGet"></param>
        public abstract void GetByPrimaryKey<valueType, modelType>
            (fastCSharp.emit.sqlTable.sqlTool<valueType, modelType> sqlTool, valueType value, fastCSharp.code.memberMap<modelType> memberMap, Action<valueType> onGet)
            where valueType : class, modelType
            where modelType : class;
    }
}
