using System;
using System.Reflection;
using fastCSharp.reflection;
using fastCSharp.code;
using fastCSharp.code.cSharp;
using System.Threading;
using System.Collections.Generic;
using fastCSharp.threading;
using fastCSharp.emit;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql
{
    /// <summary>
    /// 数据库连接信息
    /// </summary>
    public sealed unsafe class connection
    {
        /// <summary>
        /// SQL类型
        /// </summary>
        public fastCSharp.sql.type Type;
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string Connection;
        /// <summary>
        /// 数据库表格所有者
        /// </summary>
        public string Owner = "dbo";
        /// <summary>
        /// SQL客户端
        /// </summary>
        private client client;
        /// <summary>
        /// SQL客户端
        /// </summary>
        public client Client
        {
            get
            {
                if (client == null)
                {
                    client = (client)Enum<fastCSharp.sql.type, fastCSharp.sql.typeInfo>.Array((byte)Type).ClientType
                        .GetConstructor(new Type[] { typeof(connection) }).Invoke(new object[] { this });
                }
                return client;
            }
        }

        /// <summary>
        /// 是否需要检测链接
        /// </summary>
        public static bool IsCheckConnection
        {
            get
            {
                return config.sql.Default.CheckConnection.Length != 0;
            }
        }
        /// <summary>
        /// 连接集合
        /// </summary>
        private static readonly fastCSharp.threading.interlocked.lastDictionary<hashString, connection> connections = new interlocked.lastDictionary<hashString, connection>();
        /// <summary>
        /// 根据连接类型获取连接信息
        /// </summary>
        /// <param name="type">连接类型</param>
        /// <returns>连接信息</returns>
        public static connection GetConnection(string type)
        {
            if (type != null)
            {
                connection value;
                hashString key = type;
                if (!connections.TryGetValue(ref key, out value)) connections.Set(ref key, value = config.pub.LoadConfig(new connection(), type));
                return value;
            }
            return null;
        }
        /// <summary>
        /// 检测链接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void checkConnection(object sender, AssemblyLoadEventArgs args)
        {
            threading.threadPool.TinyPool.FastStart(args.LoadedAssembly, thread.callType.CheckSqlConnection);
        }
        /// <summary>
        /// 程序集名称唯一哈希
        /// </summary>
        private struct assemblyName : IEquatable<assemblyName>
        {
            /// <summary>
            /// 程序集名称
            /// </summary>
            public string Name;
            /// <summary>
            /// 隐式转换
            /// </summary>
            /// <param name="name">程序集名称</param>
            /// <returns>程序集名称唯一哈希</returns>
            public static implicit operator assemblyName(string name) { return new assemblyName { Name = name }; }
            /// <summary>
            /// 获取哈希值
            /// </summary>
            /// <returns>哈希值</returns>
            public override int GetHashCode()
            {
                return (Name[0] >> 3) & 7;
            }
            /// <summary>
            /// 判断是否相等
            /// </summary>
            /// <param name="other">待匹配数据</param>
            /// <returns>是否相等</returns>
            public bool Equals(assemblyName other)
            {
                return Name == other.Name;
            }
            /// <summary>
            /// 判断是否相等
            /// </summary>
            /// <param name="obj">待匹配数据</param>
            /// <returns>是否相等</returns>
            public override bool Equals(object obj)
            {
                return Equals((assemblyName)obj);
            }
        }
        /// <summary>
        /// 检测链接程序集名称集合
        /// </summary>
        private static readonly uniqueHashSet<assemblyName> checkConnectionAssemblyNames = new uniqueHashSet<assemblyName>(fastCSharp.config.sql.Default.IgnoreConnectionFastCSharp ? new assemblyName[] { "mscorlib", "System", "Microsoft", "vshost" } : new assemblyName[] { "mscorlib", fastCSharp.pub.fastCSharp, "System", "Microsoft", "vshost" }, 7);
        /// <summary>
        /// 检测链接程序集名称分隔符位图
        /// </summary>
        private static readonly String.asciiMap checkConnectionAssemblyNameMap;
        /// <summary>
        /// 检测程序集名称集合
        /// </summary>
        private static readonly interlocked.dictionary<hashString, EventWaitHandle> checkAssemblyNames = new interlocked.dictionary<hashString, EventWaitHandle>();
        /// <summary>
        /// 检测链接
        /// </summary>
        /// <param name="assembly">程序集</param>
        internal unsafe static void CheckConnection(Assembly assembly)
        {
            bool isAssembly;
            string assemblyName = assembly.FullName;
            fixed (char* nameFixed = assemblyName)
            {
                char* splitIndex = unsafer.String.FindAscii(nameFixed, nameFixed + assemblyName.Length, checkConnectionAssemblyNameMap.Map);
                isAssembly = splitIndex == null || !checkConnectionAssemblyNames.Contains(assemblyName.Substring(0, (int)(splitIndex - nameFixed)));
            }
            if (isAssembly)
            {
                Type currentType = null;
                try
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (type.IsClass && !type.IsAbstract && !type.IsGenericType && type.IsVisible)
                        {
                            fastCSharp.emit.sqlTable sqlTable = fastCSharp.code.typeAttribute.GetAttribute<fastCSharp.emit.sqlTable>(type, false, true);
                            if (sqlTable != null && Array.IndexOf(fastCSharp.config.sql.Default.CheckConnection, sqlTable.ConnectionType) != -1)
                            {
                                Type modelType;
                                fastCSharp.code.cSharp.sqlModel sqlModel = fastCSharp.code.cSharp.dataModel.GetModelType<fastCSharp.code.cSharp.sqlModel>(type, out modelType);
                                checkSqlTable(currentType = type, modelType ?? type, sqlModel, sqlTable);
                            }
                        }
                    }
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, currentType.fullName(), true);
                }
                finally
                {
                    EventWaitHandle wait = null;
                    if (checkAssemblyNames.Set(assemblyName, null, out wait))
                    {
                        wait.Set();
                        wait.Close();
                    }
                }
            }
        }
        /// <summary>
        /// 等待检测链接
        /// </summary>
        /// <param name="type">表格绑定类型</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static void WaitCheckConnection(Type type)
        {
            if (type != null) WaitCheckConnection(type.Assembly);
        }
        /// <summary>
        /// 等待检测链接
        /// </summary>
        /// <param name="assembly">程序集</param>
        public static void WaitCheckConnection(Assembly assembly)
        {
            if (assembly != null)
            {
                EventWaitHandle wait = null;
                hashString assemblyName = assembly.FullName;
                if (!checkAssemblyNames.TryGetValue(assemblyName, out wait))
                {
                    checkAssemblyNames.Set(assemblyName, wait = new EventWaitHandle(false, EventResetMode.ManualReset));
                }
                if (wait != null) wait.WaitOne();
            }
        }
        /// <summary>
        /// 检测链接
        /// </summary>
        /// <param name="assemblys">程序集集合</param>
        internal static void CheckConnection(Assembly[] assemblys)
        {
            foreach (Assembly assembly in assemblys)
            {
                Thread.Yield();
                CheckConnection(assembly);
            }
        }
        /// <summary>
        /// 检测SQL表格
        /// </summary>
        /// <param name="type">表格绑定类型</param>
        /// <param name="modelType">表格模型类型</param>
        /// <param name="sqlModel">数据库表格模型配置</param>
        /// <param name="sqlTable">数据库表格配置</param>
        private static void checkSqlTable(Type type, Type modelType, fastCSharp.code.cSharp.sqlModel sqlModel, fastCSharp.emit.sqlTable sqlTable)
        {
            client sqlClient = GetConnection(sqlTable.ConnectionType).Client;
            table memberTable = (table)typeof(sqlModel<>).MakeGenericType(modelType).GetMethod("GetTable", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(Type), typeof(fastCSharp.emit.sqlTable) }, null).Invoke(null, new object[] { type, sqlTable });
            sqlClient.ToSqlColumn(memberTable);
            table table = sqlClient.GetTable(memberTable.Columns.Name);
            if (table == null)
            {
                if (!sqlClient.CreateTable(memberTable)) fastCSharp.log.Error.Add("表格 " + memberTable.Columns.Name + " 创建失败", new System.Diagnostics.StackFrame(), false);
            }
            else
            {
                bool ignoreCase = Enum<fastCSharp.sql.type, fastCSharp.sql.typeInfo>.Array((byte)sqlClient.Connection.Type).IgnoreCase;
                if (sqlModel.DeleteColumnNames != null)
                {
                    HashSet<string> deleteNames = sqlModel.DeleteColumnNames.Split(',').getHash(value => ignoreCase ? value.toLower() : value);
                    column[] deleteColumns = table.Columns.Columns.getFindArray(value => deleteNames.Contains(ignoreCase ? value.SqlName.ToLower() : value.SqlName));
                    if (deleteColumns.Length != 0)
                    {
                        table.Columns.Columns = table.Columns.Columns.getFindArray(value => !deleteNames.Contains(ignoreCase ? value.SqlName.ToLower() : value.SqlName));
                        if (!sqlClient.DeleteFields(new columnCollection { Name = memberTable.Columns.Name, Columns = deleteColumns }))
                        {
                            fastCSharp.log.Error.Add("表格 " + memberTable.Columns.Name + " 字段删除失败 : " + deleteColumns.joinString(',', value => value.SqlName), new System.Diagnostics.StackFrame(), false);
                        }
                    }
                }
                string[] names = ignoreCase ? table.Columns.Columns.getArray(value => value.SqlName.ToLower()) : table.Columns.Columns.getArray(value => value.SqlName);
                using (fastCSharp.stateSearcher.ascii<column> sqlColumnNames = new stateSearcher.ascii<column>(names, table.Columns.Columns, false))
                {
                    subArray<column> newColumns;
                    if (ignoreCase) newColumns = memberTable.Columns.Columns.getFind(value => !sqlColumnNames.ContainsKey(value.SqlName.ToLower()));
                    else newColumns = memberTable.Columns.Columns.getFind(value => !sqlColumnNames.ContainsKey(value.SqlName));
                    if (newColumns.length != 0)
                    {
                        if (sqlClient.IsAddField && sqlClient.AddFields(new columnCollection { Name = memberTable.Columns.Name, Columns = newColumns.ToArray() }))
                        {
                            table.Columns.Columns = newColumns.Add(table.Columns.Columns).ToArray();
                        }
                        else
                        {
                            fastCSharp.log.Error.Add("表格 " + memberTable.Columns.Name + " 字段添加失败 : " + newColumns.joinString(',', value => value.SqlName), new System.Diagnostics.StackFrame(), false);
                        }
                    }
                    if (ignoreCase) newColumns = memberTable.Columns.Columns.getFind(value => !value.IsMatch(sqlColumnNames.Get(value.SqlName.ToLower()), ignoreCase));
                    else newColumns = memberTable.Columns.Columns.getFind(value => !value.IsMatch(sqlColumnNames.Get(value.SqlName), ignoreCase));
                    if (newColumns.count() != 0)
                    {
                        fastCSharp.log.Default.Add("表格 " + memberTable.Columns.Name + " 字段类型不匹配 : " + newColumns.joinString(',', value => value.SqlName), new System.Diagnostics.StackFrame(), false);
                    }
                }
            }
        }

        static connection()
        {
            if (config.sql.Default.CheckConnection.Length != 0)
            {
                checkConnectionAssemblyNameMap = new String.asciiMap(".,-", true);
                AppDomain.CurrentDomain.AssemblyLoad += checkConnection;
                threading.threadPool.TinyPool.FastStart(AppDomain.CurrentDomain.GetAssemblies(), thread.callType.CheckSqlConnectionArray);
            }
        }
    }
}
