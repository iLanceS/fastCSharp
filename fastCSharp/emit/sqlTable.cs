using System;
using fastCSharp.code;
using fastCSharp.reflection;
using fastCSharp.sql;
using fastCSharp.code.cSharp;
using System.Threading;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;
using fastCSharp.threading;
#if NOJIT
#else
using System.Reflection.Emit;
#endif

namespace fastCSharp.emit
{
    /// <summary>
    /// 数据库表格配置
    /// </summary>
    public class sqlTable : Attribute
    {
        /// <summary>
        /// SQL表格操作工具 字段名称
        /// </summary>
        public const string SqlTableName = "sqlTable";
        /// <summary>
        /// SQL表格默认缓存 字段名称
        /// </summary>
        public const string SqlCacheName = "sqlCache";
        /// <summary>
        /// SQL表格默认计数缓存 字段名称
        /// </summary>
        public const string SqlCacheCounterName = "sqlCacheCounter";
        /// <summary>
        /// SQL表格操作日志 字段名称
        /// </summary>
        public const string SqlStreamName = "sqlStream";
        /// <summary>
        /// SQL表格计算列日志 字段名称
        /// </summary>
        public const string SqlStreamLoadName = "SqlStreamLoad";
        /// <summary>
        /// SQL表格计算列计数 字段名称
        /// </summary>
        public const string SqlStreamCountName = "sqlStreamCount";
        /// <summary>
        /// 链接类型
        /// </summary>
        public string ConnectionName;
        /// <summary>
        /// 链接类型
        /// </summary>
        public virtual string ConnectionType
        {
            get { return ConnectionName; }
        }
        /// <summary>
        /// 表格名称
        /// </summary>
        public string TableName;
        /// <summary>
        /// 自增ID起始值
        /// </summary>
        public int BaseIdentity = 1;
        /// <summary>
        /// 表格编号，主要使用枚举识别同一数据模型下的不同表格
        /// </summary>
        public int TableNumber;
        /// <summary>
        /// 是否自动获取自增标识
        /// </summary>
        public bool IsLoadIdentity = true;
        /// <summary>
        /// 是否设置自增标识
        /// </summary>
        public bool IsSetIdentity = true;
        /// <summary>
        /// 写操作是否加锁
        /// </summary>
        public bool IsLockWrite = true;
        /// <summary>
        /// 获取表格名称
        /// </summary>
        /// <param name="type">表格绑定类型</param>
        /// <returns>表格名称</returns>
        internal unsafe string GetTableName(Type type)
        {
            if (TableName != null) return TableName;
            string name = null;
            if (fastCSharp.config.sql.Default.TableNamePrefixs.Length != 0)
            {
                name = type.fullName();
                foreach (string perfix in fastCSharp.config.sql.Default.TableNamePrefixs)
                {
                    if (name.Length > perfix.Length && name.StartsWith(perfix, StringComparison.Ordinal) && name[perfix.Length] == '.')
                    {
                        return name.Substring(perfix.Length + 1);
                    }
                }
            }
            int depth = fastCSharp.config.sql.Default.TableNameDepth;
            if (depth <= 0) return type.Name;
            if (name == null) name = type.fullName();
            fixed (char* nameFixed = name)
            {
                char* start = nameFixed, end = nameFixed + name.Length;
                do
                {
                    while (start != end && *start != '.') ++start;
                    if (start == end) return type.Name;
                    ++start;
                }
                while (--depth != 0);
                int index = (int)(start - nameFixed);
                while (start != end)
                {
                    if (*start == '.') *start = '_';
                    ++start;
                }
                return name.Substring(index);
            }
        }
        /// <summary>
        /// SQL数据验证接口
        /// </summary>
        public interface ISqlVerify
        {
            /// <summary>
            /// 是否通过SQL数据验证
            /// </summary>
            /// <returns></returns>
            bool IsSqlVeify();
        }
        /// <summary>
        /// 未知类型转换SQL字符串
        /// </summary>
        public interface ISqlString
        {
            /// <summary>
            /// 转换字符串
            /// </summary>
            /// <returns></returns>
            string ToSqlString();
        }
        /// <summary>
        /// 取消确认
        /// </summary>
        public sealed class cancel
        {
            /// <summary>
            /// 是否取消
            /// </summary>
            private bool isCancel;
            /// <summary>
            /// 是否取消
            /// </summary>
            public bool IsCancel
            {
                get { return isCancel; }
                set { if (value) isCancel = true; }
            }
        }
        /// <summary>
        /// 更新数据表达式
        /// </summary>
        public struct updateExpression
        {
            /// <summary>
            /// SQL表达式集合
            /// </summary>
            private subArray<keyValue<string, string>> values;
            /// <summary>
            /// SQL表达式数量
            /// </summary>
            public int Count
            {
                get { return values.length; }
            }
            /// <summary>
            /// 添加SQL表达式
            /// </summary>
            /// <param name="value"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void Add(keyValue<string, string> value)
            {
                values.Add(value);
            }
            /// <summary>
            /// 更新数据成员位图
            /// </summary>
            /// <typeparam name="modelType"></typeparam>
            /// <returns></returns>
            internal fastCSharp.code.memberMap<modelType> CreateMemberMap<modelType>()
            {
                fastCSharp.code.memberMap<modelType> memberMap = fastCSharp.code.memberMap<modelType>.New();
                foreach (keyValue<string, string> value in values.array)
                {
                    if (value.Key == null) break;
                    if (!memberMap.SetMember(value.Key))
                    {
                        memberMap.Dispose();
                        log.Error.Throw(typeof(modelType).fullName() + " 找不到SQL字段 " + value.Key, new System.Diagnostics.StackFrame(), true);
                    }
                }
                return memberMap;
            }
            /// <summary>
            /// 数据更新SQL流
            /// </summary>
            /// <param name="sqlStream"></param>
            internal void Update(charStream sqlStream)
            {
                int isNext = 0;
                foreach (keyValue<string, string> value in values.array)
                {
                    if (value.Key == null) break;
                    if (isNext == 0) isNext = 1;
                    else sqlStream.Write(',');
                    sqlStream.SimpleWriteNotNull(value.Key);
                    sqlStream.Write('=');
                    sqlStream.WriteNotNull(value.Value);
                }
            }
        }
        /// <summary>
        /// 日志流数据完成类型加载
        /// </summary>
        public struct sqlStreamCountLoaderType : IEquatable<sqlStreamCountLoaderType>
        {
            /// <summary>
            /// 数据模型类型
            /// </summary>
            public Type Type;
            /// <summary>
            /// 表格编号，主要使用枚举识别同一数据模型下的不同表格
            /// </summary>
            public int TableNumber;
            /// <summary>
            /// 日志流数据完成类型加载
            /// </summary>
            /// <param name="type">数据模型类型</param>
            /// <param name="tableNumber">表格编号</param>
            public sqlStreamCountLoaderType(Type type, int tableNumber)
            {
                Type = type;
                TableNumber = tableNumber;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="type"></param>
            /// <param name="tableNumber"></param>
            /// <returns></returns>
            public bool Equals(Type type, int tableNumber)
            {
                return Type == type && tableNumber == TableNumber;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public bool Equals(sqlStreamCountLoaderType other)
            {
                return Type == other.Type && TableNumber == other.TableNumber;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                return Equals((sqlStreamCountLoaderType)obj);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return Type.GetHashCode() ^ TableNumber;
            }

            /// <summary>
            /// 已经加载的数据完成类型集合
            /// </summary>
            private static readonly HashSet<sqlStreamCountLoaderType> loadedTypes = hashSet<sqlStreamCountLoaderType>.Create();
            /// <summary>
            /// 数据完成类型加载集合
            /// </summary>
            private static readonly Dictionary<sqlStreamCountLoaderType, list<Action<sqlStreamCountLoaderType>>> typeLoaders = dictionary<sqlStreamCountLoaderType>.Create<list<Action<sqlStreamCountLoaderType>>>();
            /// <summary>
            /// 数据完成类型加载集合访问锁
            /// </summary>
            private static readonly object loaderLock = new object();
            /// <summary>
            /// 数据完成类型注册
            /// </summary>
            /// <param name="modelType">数据模型类型</param>
            /// <param name="tableNumber">表格编号</param>
            /// <param name="loader">数据完成操作</param>
            /// <param name="types">待加载类型集合</param>
            public static void Add(Type modelType, int tableNumber, Action<sqlStreamCountLoaderType> loader, params sqlStreamCountLoaderType[] types)
            {
                bool isLoader = false;
                sqlStreamCountLoaderType loaderType = new sqlStreamCountLoaderType(modelType, tableNumber);
                Monitor.Enter(loaderLock);
                try
                {
                    if (loadedTypes.Contains(loaderType)) isLoader = true;
                    else
                    {
                        loadedTypes.Add(loaderType);
                        list<Action<sqlStreamCountLoaderType>> waitLoaders;
                        if (typeLoaders.TryGetValue(loaderType, out waitLoaders))
                        {
                            typeLoaders.Remove(loaderType);
                            foreach (Action<sqlStreamCountLoaderType> waitLoader in waitLoaders) waitLoader(loaderType);
                        }

                        foreach (sqlStreamCountLoaderType type in types.notNull())
                        {
                            if (loadedTypes.Contains(type)) loader(type);
                            else
                            {
                                if (!typeLoaders.TryGetValue(type, out waitLoaders)) typeLoaders.Add(type, waitLoaders = new list<Action<sqlStreamCountLoaderType>>());
                                waitLoaders.Add(loader);
                            }
                        }
                    }
                }
                finally { Monitor.Exit(loaderLock); }
                if (isLoader) fastCSharp.log.Error.Add("数据完成类型注册冲突 " + modelType.fullName()+ "["+ tableNumber.toString() + "]", null, false);
            }
            /// <summary>
            /// 数据完成类型注册
            /// </summary>
            /// <param name="modelType">数据模型类型</param>
            /// <param name="tableNumber">表格编号</param>
            public static void Add(Type modelType, int tableNumber)
            {
                Add(modelType, tableNumber, null);
            }
        }
        /// <summary>
        /// 数据库表格操作工具
        /// </summary>
        public abstract class sqlToolBase : IDisposable
        {
            /// <summary>
            /// 自增ID生成器
            /// </summary>
            protected long identity64;
            /// <summary>
            /// 当前自增ID
            /// </summary>
            public long Identity64
            {
                get { return identity64; }
                set
                {
                    if (sqlTable.IsSetIdentity)
                    {
                        do
                        {
                            long identity = identity64;
                            if (value <= identity) return;
                            if (Interlocked.CompareExchange(ref identity64, value, identity) == identity) return;
                            log.Default.Add("ID "+ identity.toString() + " 被修改", new System.Diagnostics.StackFrame(), false);
                        }
                        while (true);
                    }
                    if (!sqlTable.IsLoadIdentity) identity64 = Math.Max(value, BaseIdentity - 1);
                }
            }
            /// <summary>
            /// 自增ID
            /// </summary>
            internal long NextIdentity
            {
                get { return Interlocked.Increment(ref identity64); }
            }
            /// <summary>
            /// 数据库表格配置
            /// </summary>
            private sqlTable sqlTable;
            /// <summary>
            /// 自增ID起始值
            /// </summary>
            public int BaseIdentity
            {
                get { return sqlTable.BaseIdentity; }
            }
            /// <summary>
            /// 表格编号
            /// </summary>
            public int TableNumber
            {
                get { return sqlTable.TableNumber; }
            }
            /// <summary>
            /// 是否设置自增标识
            /// </summary>
            public bool IsSetIdentity
            {
                get { return sqlTable.IsSetIdentity; }
            }
            /// <summary>
            /// SQL操作客户端
            /// </summary>
            public client Client { get; private set; }
            /// <summary>
            /// 表格名称
            /// </summary>
            internal string TableName { get; private set; }
            /// <summary>
            /// SQL访问锁
            /// </summary>
            public readonly object Lock = new object();
            /// <summary>
            /// 待创建一级索引的成员名称集合
            /// </summary>
            protected fastCSharp.stateSearcher.ascii<string> noIndexMemberNames;
            /// <summary>
            /// 缓存加载等待
            /// </summary>
            public waitHandle LoadWait;
            /// <summary>
            /// 数据库表格是否加载成功
            /// </summary>
            protected bool isTable;
            /// <summary>
            /// 数据库表格是否加载成功
            /// </summary>
            public bool IsTable
            {
                get { return isTable; }
            }
            /// <summary>
            /// 是否锁定同步更新操作
            /// </summary>
            internal bool IsLockWrite
            {
                get { return sqlTable.IsLockWrite; }
            }
            /// <summary>
            /// 成员名称是否忽略大小写
            /// </summary>
            protected bool ignoreCase;
            /// <summary>
            /// 数据库表格操作工具
            /// </summary>
            /// <param name="sqlTable">数据库表格配置</param>
            /// <param name="client">SQL操作客户端</param>
            /// <param name="tableName">表格名称</param>
            protected sqlToolBase(sqlTable sqlTable, client client, string tableName)
            {
                this.sqlTable = sqlTable;
                this.Client = client;
                TableName = tableName;
                ignoreCase = Enum<fastCSharp.sql.type, fastCSharp.sql.typeInfo>.Array((byte)client.Connection.Type).IgnoreCase;
                LoadWait = new waitHandle(false);
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public virtual unsafe void Dispose()
            {
                fastCSharp.pub.Dispose(ref noIndexMemberNames);
            }
            /// <summary>
            /// 创建索引
            /// </summary>
            /// <param name="name">列名称</param>
            /// <param name="sqlName">SQL名称</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void CreateIndex(string name, string sqlName)
            {
                createIndex(name, sqlName, false);
            }
            /// <summary>
            /// 创建索引
            /// </summary>
            /// <param name="name">列名称</param>
            /// <param name="sqlName">数据库列名称</param>
            /// <param name="isUnique">是否唯一值</param>
            protected void createIndex(string name, string sqlName, bool isUnique)
            {
                if (ignoreCase) name = name.ToLower();
                if (noIndexMemberNames.ContainsKey(name))
                {
                    bool isIndex = false;
                    Exception exception = null;
                    Monitor.Enter(Lock);
                    try
                    {
                        if (noIndexMemberNames.Remove(name))
                        {
                            isIndex = true;
                            if (Client.CreateIndex(TableName, new columnCollection
                            {
                                Columns = new column[] { new column { Name = name } },
                                Type = isUnique ? columnCollection.type.UniqueIndex : columnCollection.type.Index
                            }))
                            {
                                return;
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        exception = error;
                    }
                    finally { Monitor.Exit(Lock); }
                    if (isIndex) log.Error.Add(exception, "索引 " + TableName + "." + name + " 创建失败", false);
                }
            }
            /// <summary>
            /// 字符串验证
            /// </summary>
            /// <param name="memberName">成员名称</param>
            /// <param name="value">成员值</param>
            /// <param name="length">最大长度</param>
            /// <param name="isAscii">是否ASCII</param>
            /// <param name="isNull">是否可以为null</param>
            /// <returns>字符串是否通过默认验证</returns>
            internal unsafe bool StringVerify(string memberName, string value, int length, bool isAscii, bool isNull)
            {
                if (!isNull && value == null)
                {
                    NullVerify(memberName);
                    return false;
                }
                if (length != 0)
                {
                    if (isAscii)
                    {
                        int nextLength = length - value.Length;
                        if (nextLength >= 0 && value.length() > (length >> 1))
                        {
                            fixed (char* valueFixed = value)
                            {
                                for (char* start = valueFixed, end = valueFixed + value.Length; start != end; ++start)
                                {
                                    if ((*start & 0xff00) != 0 && --nextLength < 0) break;
                                }
                            }
                        }
                        if (nextLength < 0)
                        {
                            log.Error.Add(TableName + "." + memberName + " 超长 " + length.toString(), null, true);
                            return false;
                        }
                    }
                    else
                    {
                        if (value.length() > length)
                        {
                            log.Error.Add(TableName + "." + memberName + " 超长 " + value.Length.toString() + " > " + length.toString(), null, false);
                            return false;
                        }
                    }
                }
                return true;
            }
            /// <summary>
            /// 成员值不能为null
            /// </summary>
            /// <param name="memberName">成员名称</param>
            internal void NullVerify(string memberName)
            {
                log.Error.Add(TableName + "." + memberName + " 不能为null", null, true);
            }
#if NOJIT
#else
            /// <summary>
            /// 数据库字符串验证函数
            /// </summary>
            internal static readonly MethodInfo StringVerifyMethod = typeof(fastCSharp.emit.sqlTable.sqlToolBase).GetMethod("StringVerify", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(string), typeof(string), typeof(int), typeof(bool), typeof(bool) }, null);
            /// <summary>
            /// 数据库字段空值验证
            /// </summary>
            internal static readonly MethodInfo NullVerifyMethod = typeof(fastCSharp.emit.sqlTable.sqlToolBase).GetMethod("NullVerify", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(string) }, null);
#endif
        }
        /// <summary>
        /// 数据库表格操作工具
        /// </summary>
        /// <typeparam name="modelType">模型类型</typeparam>
        public abstract class sqlTool<modelType> : sqlToolBase
            where modelType : class
        {
            /// <summary>
            /// 更新查询SQL数据成员
            /// </summary>
            protected fastCSharp.code.memberMap<modelType> selectMemberMap = fastCSharp.code.memberMap<modelType>.New();
            /// <summary>
            /// 更新查询SQL数据成员
            /// </summary>
            internal fastCSharp.code.memberMap<modelType> SelectMemberMap { get { return selectMemberMap; } }
            ///// <summary>
            ///// 自增字段名称
            ///// </summary>
            //public string IdentityName
            //{
            //    get
            //    {
            //        fastCSharp.code.cSharp.sqlModel.fieldInfo identity = sqlModel<modelType>.Identity;
            //        return identity != null ? identity.Field.Name : null;
            //    }
            //}
            /// <summary>
            /// 数据库表格操作工具
            /// </summary>
            /// <param name="sqlTable">数据库表格配置</param>
            /// <param name="client">SQL操作客户端</param>
            /// <param name="tableName">表格名称</param>
            protected sqlTool(sqlTable sqlTable, client client, string tableName)
                : base(sqlTable, client, tableName)
            {
            }
            /// <summary>
            /// 获取成员位图
            /// </summary>
            /// <param name="memberMap">成员位图</param>
            /// <returns>成员位图</returns>
            internal fastCSharp.code.memberMap<modelType> GetMemberMapClearIdentity(fastCSharp.code.memberMap<modelType> memberMap)
            {
                fastCSharp.code.memberMap<modelType> value = fastCSharp.emit.sqlModel<modelType>.CopyMemberMap;
                if (memberMap != null && !memberMap.IsDefault) value.And(memberMap);
                if (sqlModel<modelType>.Identity != null) value.ClearMember(sqlModel<modelType>.Identity.MemberMapIndex);
                return value;
            }
            /// <summary>
            /// 获取更新查询SQL数据成员
            /// </summary>
            /// <param name="memberMap">查询SQL数据成员</param>
            /// <returns>更新查询SQL数据成员</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal fastCSharp.code.memberMap<modelType> GetSelectMemberMap(fastCSharp.code.memberMap<modelType> memberMap)
            {
                fastCSharp.code.memberMap<modelType> value = selectMemberMap.Copy();
                value.Or(memberMap);
                return value;
            }
            /// <summary>
            /// 获取更新查询SQL数据成员
            /// </summary>
            /// <param name="memberMap">查询SQL数据成员</param>
            /// <param name="selectMemberMap"></param>
            /// <returns>更新查询SQL数据成员</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void GetSelectMemberMap(fastCSharp.code.memberMap<modelType> memberMap, fastCSharp.code.memberMap<modelType> selectMemberMap)
            {
                selectMemberMap.CopyFrom(this.selectMemberMap);
                selectMemberMap.Or(memberMap);
            }
            /// <summary>
            /// 设置更新查询SQL数据成员
            /// </summary>
            /// <param name="member">字段表达式</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void SetSelectMember<returnType>(Expression<Func<modelType, returnType>> member)
            {
                selectMemberMap.SetMember(member);
            }
            /// <summary>
            /// 设置更新查询SQL数据成员
            /// </summary>
            /// <param name="member">字段表达式</param>
            /// <param name="memberMap"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void SetSelectMember<returnType>(Expression<Func<modelType, returnType>> member, memberMap memberMap)
            {
                selectMemberMap.SetMember(member);
                memberMap.SetMember(member);
            }
            /// <summary>
            /// 数据更新SQL表达式
            /// </summary>
            /// <typeparam name="returnType">表达式类型</typeparam>
            /// <param name="expression">成员表达式</param>
            /// <returns>数据更新SQL表达式</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public updateExpression UpdateExpression<returnType>(Expression<Func<modelType, returnType>> expression)
            {
                updateExpression value = new updateExpression();
                AddUpdateExpression(ref value, expression);
                return value;
            }

            /// <summary>
            /// 数据更新SQL表达式
            /// </summary>
            /// <typeparam name="returnType">表达式类型</typeparam>
            /// <param name="expression">成员表达式</param>
            /// <param name="updateValue">成员值</param>
            /// <returns>数据更新SQL表达式</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public updateExpression UpdateExpression<returnType>(Expression<Func<modelType, returnType>> expression, returnType updateValue)
            {
                updateExpression value = new updateExpression();
                AddUpdateExpression(ref value, expression, updateValue);
                return value;
            }
            /// <summary>
            /// 数据更新SQL表达式
            /// </summary>
            /// <typeparam name="returnType"></typeparam>
            /// <param name="value"></param>
            /// <param name="expression"></param>
            public void AddUpdateExpression<returnType>(ref updateExpression value, Expression<Func<modelType, returnType>> expression)
            {
                if (expression != null)
                {
                    keyValue<string, string> sql = Client.GetSql(expression);
                    if (sql.Key == null) log.Error.Throw(log.exceptionType.Null);
                    value.Add(sql);
                }
            }
            /// <summary>
            /// 数据更新SQL表达式
            /// </summary>
            /// <typeparam name="returnType">更新字段类型</typeparam>
            /// <param name="value">更新数据表达式</param>
            /// <param name="expression">字段表达式</param>
            /// <param name="updateValue">更新字段值</param>
            public void AddUpdateExpression<returnType>(ref updateExpression value, Expression<Func<modelType, returnType>> expression, returnType updateValue)
            {
                if (expression != null)
                {
                    keyValue<string, string> sql = Client.GetSql(expression);
                    if (sql.Key == null) log.Error.Throw(log.exceptionType.Null);
                    value.Add(new keyValue<string, string>(sql.Key, Client.ToString(updateValue)));
                }
            }
            /// <summary>
            /// 数据更新SQL表达式
            /// </summary>
            /// <param name="value"></param>
            /// <param name="expression"></param>
            public void AddUpdateExpression(ref updateExpression value, Expression<Action<modelType>> expression)
            {
                if (expression != null)
                {
                    keyValue<string, string> sql = Client.GetSql(expression);
                    if (sql.Key == null) log.Error.Throw(log.exceptionType.Null);
                    value.Add(sql);
                }
            }
            /// <summary>
            /// 数据更新SQL表达式
            /// </summary>
            /// <typeparam name="returnType">表达式类型</typeparam>
            /// <param name="expressions">成员表达式集合</param>
            /// <returns>数据更新SQL表达式</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public updateExpression UpdateExpression<returnType>(params Expression<Func<modelType, returnType>>[] expressions)
            {
                updateExpression value = new updateExpression();
                AddUpdateExpression(ref value, expressions);
                return value;
            }
            /// <summary>
            /// 数据更新SQL表达式
            /// </summary>
            /// <typeparam name="returnType"></typeparam>
            /// <param name="value"></param>
            /// <param name="expressions"></param>
            public void AddUpdateExpression<returnType>(ref updateExpression value, params Expression<Func<modelType, returnType>>[] expressions)
            {
                foreach (Expression<Func<modelType, returnType>> expression in expressions) AddUpdateExpression(ref value, expression);
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public override unsafe void Dispose()
            {
                base.Dispose();
                fastCSharp.pub.Dispose(ref selectMemberMap);
            }
        }
        /// <summary>
        /// 数据库表格操作工具
        /// </summary>
        /// <typeparam name="valueType">表格类型</typeparam>
        /// <typeparam name="modelType">模型类型</typeparam>
        public abstract class sqlTool<valueType, modelType> : sqlTool<modelType>
            where valueType : class, modelType
            where modelType : class
        {
            /// <summary>
            /// 数据库表格操作工具
            /// </summary>
            /// <param name="sqlTable">数据库表格配置</param>
            /// <param name="client">SQL操作客户端</param>
            /// <param name="tableName">表格名称</param>
            protected sqlTool(sqlTable sqlTable, client client, string tableName)
                : base(sqlTable, client, tableName)
            {
                fastCSharp.sql.connection.WaitCheckConnection(typeof(valueType));
                try
                {
                    table table = client.GetTable(tableName);
                    if (table == null)
                    {
                        //Type sqlModelType = fastCSharp.code.cSharp.dataModel.GetModelType<fastCSharp.code.cSharp.sqlModel>(typeof(valueType)) ?? typeof(valueType);
                        table memberTable = sqlModel<modelType>.GetTable(typeof(valueType), sqlTable);
                        client.ToSqlColumn(memberTable);
                        if (client.CreateTable(memberTable)) table = memberTable;
                    }
                    string[] names = ignoreCase ? table.Columns.Columns.getArray(value => value.Name.ToLower()) : table.Columns.Columns.getArray(value => value.Name);
                    noIndexMemberNames = new stateSearcher.ascii<string>(names, names, false);
                    if (table.Indexs != null)
                    {
                        foreach (columnCollection column in table.Indexs) noIndexMemberNames.Remove(ignoreCase ? column.Columns[0].Name.ToLower() : column.Columns[0].Name);
                    }
                    if (table.PrimaryKey != null) noIndexMemberNames.Remove(ignoreCase ? table.PrimaryKey.Columns[0].Name.ToLower() : table.PrimaryKey.Columns[0].Name);
                    isTable = true;
                }
                catch (Exception error)
                {
                    log.Error.Add(error, tableName, false);
                }
                if (IsTable)
                {
                    fastCSharp.code.cSharp.sqlModel.fieldInfo identity = sqlModel<modelType>.Identity;
                    if (identity != null)
                    {
                        if (client.IsIndex) createIndex(identity.Field.Name, identity.SqlFieldName, true);
                        selectMemberMap.SetMember(identity.Field.Name);
                        if (sqlTable.IsLoadIdentity)
                        {
                            IConvertible identityConvertible = client.GetValue<valueType, modelType, IConvertible>(this, new selectQuery<modelType> { SqlFieldOrders = new keyValue<fastCSharp.code.cSharp.sqlModel.fieldInfo, bool>[] { new keyValue<fastCSharp.code.cSharp.sqlModel.fieldInfo, bool>(identity, true) } }, identity.SqlFieldName, null);
                            identity64 = identityConvertible == null ? sqlTable.BaseIdentity - 1 : identityConvertible.ToInt64(null);
                        }
                    }
                    foreach (fastCSharp.code.cSharp.sqlModel.fieldInfo field in sqlModel<modelType>.PrimaryKeys)
                    {
                        selectMemberMap.SetMember(field.MemberMapIndex);
                    }
                }
            }
            /// <summary>
            /// 添加数据之前的验证事件
            /// </summary>
            public event Action<valueType, cancel> OnInsert;
            /// <summary>
            /// 添加数据之前的验证事件
            /// </summary>
            /// <param name="value">待插入数据</param>
            /// <returns>是否可插入数据库</returns>
            internal bool CallOnInsert(valueType value)
            {
                if (OnInsert != null)
                {
                    cancel cancel = new cancel();
                    OnInsert(value, cancel);
                    return !cancel.IsCancel;
                }
                return true;
            }
            /// <summary>
            /// 添加数据之前的验证事件
            /// </summary>
            /// <param name="values">待插入数据集合</param>
            /// <returns>是否可插入数据库</returns>
            internal bool CallOnInsert(ref subArray<valueType> values)
            {
                if (OnInsert != null)
                {
                    cancel cancel = new cancel();
                    foreach (valueType value in values)
                    {
                        OnInsert(value, cancel);
                        if (cancel.IsCancel) return false;
                    }
                }
                return true;
            }
            /// <summary>
            /// 添加数据之前的验证事件
            /// </summary>
            public event Action<valueType, cancel> OnInsertLock;
            /// <summary>
            /// 添加数据之前的验证事件
            /// </summary>
            /// <param name="value">待插入数据</param>
            /// <returns>是否可插入数据库</returns>
            internal bool CallOnInsertLock(valueType value)
            {
                if (OnInsertLock != null)
                {
                    cancel cancel = new cancel();
                    OnInsertLock(value, cancel);
                    return !cancel.IsCancel;
                }
                return true;
            }
            /// <summary>
            /// 添加数据之前的验证事件
            /// </summary>
            /// <param name="values">待插入数据集合</param>
            /// <returns>是否可插入数据库</returns>
            internal bool CallOnInsertLock(ref subArray<valueType> values)
            {
                if (OnInsertLock != null)
                {
                    cancel cancel = new cancel();
                    foreach (valueType value in values)
                    {
                        OnInsertLock(value, cancel);
                        if (cancel.IsCancel) return false;
                    }
                }
                return true;
            }
            /// <summary>
            /// 添加数据之后的处理事件
            /// </summary>
            public event Action<valueType> OnInsertedLock;
            /// <summary>
            /// 添加数据之后的处理事件
            /// </summary>
            /// <param name="value">被插入的数据</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void CallOnInsertedLock(valueType value)
            {
                if (OnInsertedLock != null) OnInsertedLock(value);
            }
            /// <summary>
            /// 添加数据之后的处理事件
            /// </summary>
            public event Action<valueType> OnInserted;
            /// <summary>
            /// 添加数据之后的处理事件
            /// </summary>
            /// <param name="value">被插入的数据</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void CallOnInserted(valueType value)
            {
                if (OnInserted != null) OnInserted(value);
            }
            /// <summary>
            /// 更新数据之前的验证事件
            /// </summary>
            public event Action<valueType, fastCSharp.code.memberMap<modelType>, cancel> OnUpdate;
            /// <summary>
            /// 更新数据之前的验证事件
            /// </summary>
            /// <param name="value">待更新数据</param>
            /// <param name="memberMap">更新成员位图</param>
            /// <returns>是否可更新数据库</returns>
            internal bool CallOnUpdate(valueType value, fastCSharp.code.memberMap<modelType> memberMap)
            {
                if (OnUpdate != null)
                {
                    cancel cancel = new cancel();
                    OnUpdate(value, memberMap, cancel);
                    return !cancel.IsCancel;
                }
                return true;
            }
            /// <summary>
            /// 更新数据之前的验证事件
            /// </summary>
            public event Action<valueType, fastCSharp.code.memberMap<modelType>, cancel> OnUpdateLock;
            /// <summary>
            /// 更新数据之前的验证事件
            /// </summary>
            /// <param name="value">待更新数据</param>
            /// <param name="memberMap">更新成员位图</param>
            /// <returns>是否可更新数据库</returns>
            internal bool CallOnUpdateLock(valueType value, fastCSharp.code.memberMap<modelType> memberMap)
            {
                if (OnUpdateLock != null)
                {
                    cancel cancel = new cancel();
                    OnUpdateLock(value, memberMap, cancel);
                    return !cancel.IsCancel;
                }
                return true;
            }
            /// <summary>
            /// 更新数据之后的处理事件
            /// </summary>
            public event Action<valueType, valueType, fastCSharp.code.memberMap<modelType>> OnUpdatedLock;
            /// <summary>
            /// 更新数据之后的处理事件
            /// </summary>
            /// <param name="value">更新后的数据</param>
            /// <param name="oldValue">更新前的数据</param>
            /// <param name="memberMap">更新成员位图</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void CallOnUpdatedLock(valueType value, valueType oldValue, fastCSharp.code.memberMap<modelType> memberMap)
            {
                if (OnUpdatedLock != null) OnUpdatedLock(value, oldValue, memberMap);
            }
            /// <summary>
            /// 更新数据之后的处理事件
            /// </summary>
            public event Action<valueType, valueType, fastCSharp.code.memberMap<modelType>> OnUpdated;
            /// <summary>
            /// 更新数据之后的处理事件
            /// </summary>
            /// <param name="value">更新后的数据</param>
            /// <param name="oldValue">更新前的数据</param>
            /// <param name="memberMap">更新成员位图</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void CallOnUpdated(valueType value, valueType oldValue, fastCSharp.code.memberMap<modelType> memberMap)
            {
                if (OnUpdated != null) OnUpdated(value, oldValue, memberMap);
            }
            /// <summary>
            /// 删除数据之前的验证事件
            /// </summary>
            public event Action<valueType, cancel> OnDelete;
            /// <summary>
            /// 删除数据之前的验证事件
            /// </summary>
            /// <param name="value">待删除数据</param>
            /// <returns>是否可删除数据</returns>
            internal bool CallOnDelete(valueType value)
            {
                if (OnDelete != null)
                {
                    cancel cancel = new cancel();
                    OnDelete(value, cancel);
                    return !cancel.IsCancel;
                }
                return true;
            }
            /// <summary>
            /// 同步模式删除数据之前的验证事件
            /// </summary>
            public event Action<valueType, cancel> OnDeleteLock;
            /// <summary>
            /// 删除数据之前的验证事件
            /// </summary>
            /// <param name="value">待删除数据</param>
            /// <returns>是否可删除数据</returns>
            internal bool CallOnDeleteLock(valueType value)
            {
                if (OnDeleteLock != null)
                {
                    cancel cancel = new cancel();
                    OnDeleteLock(value, cancel);
                    return !cancel.IsCancel;
                }
                return true;
            }
            /// <summary>
            /// 删除数据之后的处理事件
            /// </summary>
            public event Action<valueType> OnDeletedLock;
            /// <summary>
            /// 删除数据之后的处理事件
            /// </summary>
            /// <param name="value">被删除的数据</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void CallOnDeletedLock(valueType value)
            {
                if (OnDeletedLock != null) OnDeletedLock(value);
            }
            /// <summary>
            /// 删除数据之后的处理事件
            /// </summary>
            public event Action<valueType> OnDeleted;
            /// <summary>
            /// 删除数据之后的处理事件
            /// </summary>
            /// <param name="value">被删除的数据</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void CallOnDeleted(valueType value)
            {
                if (OnDeleted != null) OnDeleted(value);
            }
            /// <summary>
            /// 添加数据是否启用应用程序事务
            /// </summary>
            internal bool IsInsertTransaction
            {
                get
                {
                    return OnInsertedLock != null || OnInserted != null;
                }
            }
            /// <summary>
            /// 添加数据是否启用应用程序事务
            /// </summary>
            internal bool IsUpdateTransaction
            {
                get
                {
                    return OnUpdatedLock != null || OnUpdated != null;
                }
            }
            /// <summary>
            /// 删除数据是否启用应用程序事务
            /// </summary>
            internal bool IsDeleteTransaction
            {
                get
                {
                    return OnDeletedLock != null || OnDeleted != null;
                }
            }
            /// <summary>
            /// 缓存数据加载完成
            /// </summary>
            /// <param name="onInserted">添加记录事件</param>
            /// <param name="onUpdated">更新记录事件</param>
            /// <param name="onDeleted">删除记录事件</param>
            /// <param name="isSqlStreamTypeCount">是否日志流计数完成类型注册</param>
            public void Loaded(Action<valueType> onInserted = null, Action<valueType, valueType, fastCSharp.code.memberMap<modelType>> onUpdated = null, Action<valueType> onDeleted = null, bool isSqlStreamTypeCount = true)
            {
                if (onInserted != null) OnInsertedLock += onInserted;
                if (onUpdated != null) OnUpdatedLock += onUpdated;
                if (onDeleted != null) OnDeletedLock += onDeleted;
                LoadWait.Set();
                if (isSqlStreamTypeCount) sqlStreamCountLoaderType.Add(typeof(modelType), TableNumber);
            }
            /// <summary>
            /// 数据集合转DataTable
            /// </summary>
            /// <param name="values">数据集合</param>
            /// <returns>数据集合</returns>
            internal DataTable GetDataTable(ref subArray<valueType> values)
            {
                DataTable dataTable = new DataTable(TableName);
                foreach (keyValue<string, Type> column in sqlModel<modelType>.toArray.DataColumns) dataTable.Columns.Add(new DataColumn(column.Key, column.Value));
                foreach (valueType value in values)
                {
                    object[] memberValues = new object[dataTable.Columns.Count];
                    int index = 0;
                    sqlModel<modelType>.toArray.ToArray(value, memberValues, ref index);
                    dataTable.Rows.Add(memberValues);
                }
                return dataTable;
            }
            /// <summary>
            /// 查询数据集合
            /// </summary>
            /// <param name="expression">查询条件表达式</param>
            /// <param name="memberMap">成员位图</param>
            /// <returns>数据集合</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public IEnumerable<valueType> Where(Expression<Func<modelType, bool>> expression = null, fastCSharp.code.memberMap<modelType> memberMap = null)
            {
                return Client.Select(this, (selectQuery<modelType>)expression, memberMap);
            }
            /// <summary>
            /// 查询数据集合
            /// </summary>
            /// <param name="query">查询信息</param>
            /// <param name="memberMap">成员位图</param>
            /// <returns>数据集合</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public IEnumerable<valueType> Where(selectQuery<modelType> query, fastCSharp.code.memberMap<modelType> memberMap = null)
            {
                return Client.Select(this, query, memberMap);
            }
            /// <summary>
            /// 查询数据集合
            /// </summary>
            /// <param name="expression">查询条件表达式</param>
            /// <returns>数据集合,失败返回-1</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public int Count(Expression<Func<modelType, bool>> expression = null)
            {
                return Client.Count(this, expression);
            }
            /// <summary>
            /// 将数据添加到数据库
            /// </summary>
            /// <param name="value">待添加数据</param>
            /// <param name="isIgnoreTransaction">是否忽略应用程序事务（不是数据库事务）</param>
            /// <param name="memberMap">需要生成 SQL 语句的字段成员位图</param>
            /// <returns>添加是否成功</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public bool Insert(valueType value, bool isIgnoreTransaction = false, fastCSharp.code.memberMap<modelType> memberMap = null)
            {
                return Client.Insert(this, value, memberMap, isIgnoreTransaction);
            }
            /// <summary>
            /// 批量导入数据到数据库，对于不支持批量导入的数据库将循环调用添加数据
            /// </summary>
            /// <param name="values">待添加数据集合</param>
            /// <param name="isIgnoreTransaction">是否忽略应用程序事务（不是数据库事务）</param>
            /// <returns>成功导入数据数量</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public int Insert(valueType[] values, bool isIgnoreTransaction = false)
            {
                return Client.Insert(this, values, isIgnoreTransaction).length;
            }
            /// <summary>
            /// 设置自增标识
            /// </summary>
            /// <param name="values"></param>
            internal void SetIdentity(ref subArray<valueType> values)
            {
                if (sqlModel<modelType>.Identity != null)
                {
                    if (IsSetIdentity)
                    {
                        Action<valueType, long> identitySetter = sqlModel<modelType>.SetIdentity;
                        foreach (valueType value in values) identitySetter(value, NextIdentity);
                    }
                    else
                    {
                        Func<valueType, long> identityGetter = sqlModel<modelType>.GetIdentity;
                        long maxIdentity = 0;
                        foreach (valueType value in values)
                        {
                            long identity = identityGetter(value);
                            if (identity > maxIdentity) maxIdentity = identity;
                        }
                        if (maxIdentity > identity64) identity64 = maxIdentity;
                    }
                }
            }
            /// <summary>
            /// 添加到数据库
            /// </summary>
            /// <param name="values">数据集合</param>
            /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
            /// <returns>添加是否成功</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public int Insert(subArray<valueType> values, bool isIgnoreTransaction = false)
            {
                return Client.Insert(this, ref values, isIgnoreTransaction).length;
            }
            /// <summary>
            /// 添加到数据库
            /// </summary>
            /// <param name="values">数据集合</param>
            /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
            /// <returns>添加是否成功</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public int Insert(ref subArray<valueType> values, bool isIgnoreTransaction = false)
            {
                return Client.Insert(this, ref values, isIgnoreTransaction).length;
            }
            /// <summary>
            /// 根据自增id获取数据对象
            /// </summary>
            /// <param name="value">关键字数据对象</param>
            /// <param name="memberMap">成员位图</param>
            /// <returns>数据对象</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public valueType GetByIdentity(valueType value, fastCSharp.code.memberMap<modelType> memberMap = null)
            {
                if (sqlModel<modelType>.Identity == null) log.Error.Throw(log.exceptionType.ErrorOperation);
                return Client.GetByIdentity(this, value, memberMap);
            }
            /// <summary>
            /// 获取数据库记录
            /// </summary>
            /// <param name="identity">自增id</param>
            /// <param name="memberMap">成员位图</param>
            /// <returns>数据对象</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public valueType GetByIdentity(long identity, fastCSharp.code.memberMap<modelType> memberMap = null)
            {
                valueType value = fastCSharp.emit.constructor<valueType>.New();
                sqlModel<modelType>.SetIdentity(value, identity);
                return GetByIdentity(value, memberMap);
            }
            /// <summary>
            /// 获取数据库记录
            /// </summary>
            /// <param name="identity">自增id</param>
            /// <param name="memberMap">成员位图</param>
            /// <returns>数据对象</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public valueType GetByIdentity(int identity, fastCSharp.code.memberMap<modelType> memberMap = null)
            {
                return GetByIdentity((long)identity, memberMap);
            }
            /// <summary>
            /// 根据关键字获取数据对象
            /// </summary>
            /// <param name="value">关键字数据对象</param>
            /// <param name="memberMap">成员位图</param>
            /// <returns>数据对象</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public valueType GetByPrimaryKey(valueType value, fastCSharp.code.memberMap<modelType> memberMap = null)
            {
                if (sqlModel<modelType>.PrimaryKeys.Length == 0) log.Error.Throw(log.exceptionType.ErrorOperation);
                return Client.GetByPrimaryKey(this, value, memberMap);
            }
            /// <summary>
            /// 修改数据库记录
            /// </summary>
            /// <param name="value">待修改数据</param>
            /// <param name="memberMap">需要修改的字段成员位图</param>
            /// <param name="isIgnoreTransaction">是否忽略应用程序事务（不是数据库事务）</param>
            /// <returns>是否修改成功</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public bool UpdateByIdentity(valueType value, fastCSharp.code.memberMap<modelType> memberMap, bool isIgnoreTransaction = false)
            {
                if (sqlModel<modelType>.Identity == null) log.Error.Throw(log.exceptionType.ErrorOperation);
                return Client.UpdateByIdentity(this, value, memberMap, null, isIgnoreTransaction);
            }
            /// <summary>
            /// 修改数据库记录
            /// </summary>
            /// <param name="value">待修改数据</param>
            /// <param name="memberMap">需要修改的字段成员位图</param>
            /// <param name="isIgnoreTransaction">是否忽略应用程序事务（不是数据库事务）</param>
            /// <returns>是否修改成功</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public bool UpdateByPrimaryKey(valueType value, fastCSharp.code.memberMap<modelType> memberMap, bool isIgnoreTransaction = false)
            {
                if (sqlModel<modelType>.PrimaryKeys.Length == 0) log.Error.Throw(log.exceptionType.ErrorOperation);
                return Client.Update(this, value, memberMap, isIgnoreTransaction);
            }
            /// <summary>
            /// 删除数据库记录
            /// </summary>
            /// <param name="value">待删除数据</param>
            /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
            /// <returns>是否成功</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public bool DeleteByIdentity(valueType value, bool isIgnoreTransaction = false)
            {
                if (sqlModel<modelType>.Identity == null) log.Error.Throw(log.exceptionType.ErrorOperation);
                return Client.DeleteByIdentity(this, value, false, isIgnoreTransaction);
            }
            /// <summary>
            /// 删除数据库记录
            /// </summary>
            /// <param name="identity"></param>
            /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
            /// <returns>是否成功</returns>
            public bool DeleteByIdentity(long identity, bool isIgnoreTransaction = false)
            {
                if (sqlModel<modelType>.Identity == null) log.Error.Throw(log.exceptionType.ErrorOperation);
                valueType value = fastCSharp.emit.constructor<valueType>.New();
                sqlModel<modelType>.SetIdentity(value, identity);
                return Client.DeleteByIdentity(this, value, false, isIgnoreTransaction);
            }
            /// <summary>
            /// 删除数据库记录
            /// </summary>
            /// <param name="identity"></param>
            /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
            /// <returns>是否成功</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public bool DeleteByIdentity(int identity, bool isIgnoreTransaction = false)
            {
                return DeleteByIdentity((long)identity, isIgnoreTransaction);
            }
            /// <summary>
            /// 删除数据库记录
            /// </summary>
            /// <param name="value">待删除数据</param>
            /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
            /// <returns>是否成功</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public bool DeleteByPrimaryKey(valueType value, bool isIgnoreTransaction = false)
            {
                if (sqlModel<modelType>.PrimaryKeys.Length == 0) log.Error.Throw(log.exceptionType.ErrorOperation);
                return Client.Delete(this, value, isIgnoreTransaction);
            }
            /// <summary>
            /// 修改数据库记录
            /// </summary>
            /// <param name="identity">自增id</param>
            /// <param name="updateExpression">SQL表达式</param>
            /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
            /// <returns>修改后的数据,失败返回null</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public valueType UpdateByIdentity(long identity, updateExpression updateExpression, bool isIgnoreTransaction = false)
            {
                return UpdateByIdentity(identity, ref updateExpression, isIgnoreTransaction);
            }
            /// <summary>
            /// 修改数据库记录
            /// </summary>
            /// <param name="identity">自增id</param>
            /// <param name="updateExpression">SQL表达式</param>
            /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
            /// <returns>修改后的数据,失败返回null</returns>
            public valueType UpdateByIdentity(long identity, ref updateExpression updateExpression, bool isIgnoreTransaction = false)
            {
                if (updateExpression.Count != 0)
                {
                    valueType value = fastCSharp.emit.constructor<valueType>.New();
                    sqlModel<modelType>.SetIdentity(value, identity);
                    if (Client.UpdateByIdentity(this, value, ref updateExpression, null, isIgnoreTransaction)) return value;
                }
                return null;
            }
            /// <summary>
            /// 修改数据库记录
            /// </summary>
            /// <param name="identity">自增id</param>
            /// <param name="updateExpression">SQL表达式</param>
            /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
            /// <returns>修改后的数据,失败返回null</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public valueType UpdateByIdentity(int identity, updateExpression updateExpression, bool isIgnoreTransaction = false)
            {
                return UpdateByIdentity((long)identity, ref updateExpression, isIgnoreTransaction);
            }
            /// <summary>
            /// 修改数据库记录
            /// </summary>
            /// <param name="identity">自增id</param>
            /// <param name="updateExpression">SQL表达式</param>
            /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
            /// <returns>修改后的数据,失败返回null</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public valueType UpdateByIdentity(int identity, ref updateExpression updateExpression, bool isIgnoreTransaction = false)
            {
                return UpdateByIdentity((long)identity, ref updateExpression, isIgnoreTransaction);
            }
            /// <summary>
            /// 修改数据库记录
            /// </summary>
            /// <param name="identity">自增id</param>
            /// <param name="expression">SQL表达式</param>
            /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
            /// <returns>修改后的数据,失败返回null</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public valueType UpdateByIdentity<returnType>(long identity, Expression<Func<modelType, returnType>> expression, bool isIgnoreTransaction = false)
            {
                return expression != null ? UpdateByIdentity(identity, UpdateExpression(expression), isIgnoreTransaction) : null;
            }
            /// <summary>
            /// 修改数据库记录
            /// </summary>
            /// <param name="identity">自增id</param>
            /// <param name="expression">字段表达式</param>
            /// <param name="returnValue">更新字段值</param>
            /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
            /// <returns>修改后的数据,失败返回null</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public valueType UpdateByIdentity<returnType>(long identity, Expression<Func<modelType, returnType>> expression, returnType returnValue, bool isIgnoreTransaction = false)
            {
                return expression != null ? UpdateByIdentity(identity, UpdateExpression(expression, returnValue), isIgnoreTransaction) : null;
            }
            /// <summary>
            /// 修改数据库记录
            /// </summary>
            /// <param name="identity">自增id</param>
            /// <param name="expression">SQL表达式</param>
            /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
            /// <returns>修改后的数据,失败返回null</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public valueType UpdateByIdentity<returnType>(int identity, Expression<Func<modelType, returnType>> expression, bool isIgnoreTransaction = false)
            {
                return UpdateByIdentity((long)identity, expression, isIgnoreTransaction);
            }
            /// <summary>
            /// 修改数据库记录
            /// </summary>
            /// <param name="identity">自增id</param>
            /// <param name="expression">字段表达式</param>
            /// <param name="returnValue">更新字段值</param>
            /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
            /// <returns>修改后的数据,失败返回null</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public valueType UpdateByIdentity<returnType>(int identity, Expression<Func<modelType, returnType>> expression, returnType returnValue, bool isIgnoreTransaction = false)
            {
                return UpdateByIdentity((long)identity, expression, returnValue, isIgnoreTransaction);
            }
            /// <summary>
            /// 修改数据库记录
            /// </summary>
            /// <param name="identity">自增id</param>
            /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
            /// <param name="expressions">SQL表达式</param>
            /// <returns>修改后的数据,失败返回null</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public valueType UpdateByIdentity<returnType>(long identity, bool isIgnoreTransaction, params Expression<Func<modelType, returnType>>[] expressions)
            {
                return expressions.Length != 0 ? UpdateByIdentity(identity, UpdateExpression(expressions), isIgnoreTransaction) : null;
            }
            /// <summary>
            /// 修改数据库记录
            /// </summary>
            /// <param name="identity">自增id</param>
            /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
            /// <param name="expressions">SQL表达式</param>
            /// <returns>修改后的数据,失败返回null</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public valueType UpdateByIdentity<returnType>(int identity, bool isIgnoreTransaction, params Expression<Func<modelType, returnType>>[] expressions)
            {
                return UpdateByIdentity((long)identity, isIgnoreTransaction, expressions);
            }
            /// <summary>
            /// 修改数据库记录
            /// </summary>
            /// <param name="value">待更新数据</param>
            /// <param name="updateExpression">SQL表达式</param>
            /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
            /// <returns>修改后的数据,失败返回null</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public valueType UpdateByPrimaryKey(valueType value, updateExpression updateExpression, bool isIgnoreTransaction = false)
            {
                return UpdateByPrimaryKey(value, ref updateExpression, isIgnoreTransaction);
            }
            /// <summary>
            /// 修改数据库记录
            /// </summary>
            /// <param name="value">待更新数据</param>
            /// <param name="updateExpression">SQL表达式</param>
            /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
            /// <returns>修改后的数据,失败返回null</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public valueType UpdateByPrimaryKey(valueType value, ref updateExpression updateExpression, bool isIgnoreTransaction = false)
            {
                if (updateExpression.Count != 0)
                {
                    if (Client.Update(this, value, ref updateExpression, isIgnoreTransaction)) return value;
                }
                return null;
            }
            /// <summary>
            /// 修改数据库记录
            /// </summary>
            /// <param name="value">目标数据</param>
            /// <param name="expression">SQL表达式</param>
            /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
            /// <returns>修改后的数据,失败返回null</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public valueType UpdateByPrimaryKey<returnType>(valueType value, Expression<Func<modelType, returnType>> expression, bool isIgnoreTransaction = false)
            {
                return expression != null ? UpdateByPrimaryKey(value, UpdateExpression(expression), isIgnoreTransaction) : null;
            }
            /// <summary>
            /// 修改数据库记录
            /// </summary>
            /// <param name="value">目标数据</param>
            /// <param name="expression">名称表达式</param>
            /// <param name="returnValue">设置值</param>
            /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
            /// <returns>修改后的数据,失败返回null</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public valueType UpdateByPrimaryKey<returnType>(valueType value, Expression<Func<modelType, returnType>> expression, returnType returnValue, bool isIgnoreTransaction = false)
            {
                return expression != null ? UpdateByPrimaryKey(value, UpdateExpression(expression, returnValue), isIgnoreTransaction) : null;
            }
            /// <summary>
            /// 修改数据库记录
            /// </summary>
            /// <param name="value">目标数据</param>
            /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
            /// <param name="expressions">SQL表达式</param>
            /// <returns>修改后的数据,失败返回null</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public valueType UpdateByPrimaryKey<returnType>(valueType value, bool isIgnoreTransaction, params Expression<Func<modelType, returnType>>[] expressions)
            {
                return expressions.Length != 0 ? UpdateByPrimaryKey(value, UpdateExpression(expressions), isIgnoreTransaction) : null;
            }
        }
        /// <summary>
        /// JSON操作客户端
        /// </summary>
        public sealed class jsonTool : IDisposable
        {
            /// <summary>
            /// JSON解析配置参数
            /// </summary>
            private static readonly jsonParser.config jsonConfig = new jsonParser.config { IsTempString = true };
            ///// <summary>
            ///// JSON解析配置参数访问锁
            ///// </summary>
            //private static readonly object jsonConfigLock = new object();
            /// <summary>
            /// 数据库表格操作工具
            /// </summary>
            private interface ISqlTable
            {
                /// <summary>
                /// 字段名称集合
                /// </summary>
                string[] FieldNames { get; }
                /// <summary>
                /// 自增字段名称
                /// </summary>
                string IdentityName { get; }
                /// <summary>
                /// 关键字名称集合
                /// </summary>
                string[] PrimaryKeyNames { get; }
                /// <summary>
                /// 根据JSON查询数据
                /// </summary>
                /// <param name="json"></param>
                /// <returns>查询数据的JSON字符串</returns>
                keyValue<string, string>[] Query(string json);
                /// <summary>
                /// 根据JSON更新数据
                /// </summary>
                /// <param name="json"></param>
                /// <param name="values"></param>
                /// <returns>更新是否成功</returns>
                bool Update(string json, keyValue<string, string>[] values);
                /// <summary>
                /// 根据JSON更新数据
                /// </summary>
                /// <param name="json"></param>
                /// <returns>更新是否成功</returns>
                bool Delete(string json);
            }
            /// <summary>
            /// 数据库表格操作工具
            /// </summary>
            /// <typeparam name="valueType"></typeparam>
            /// <typeparam name="modelType"></typeparam>
            private sealed class sqlTable<valueType, modelType> : ISqlTable
                where valueType : class, modelType
                where modelType : class
            {
                /// <summary>
                /// JSON解析成员位图
                /// </summary>
                private static fastCSharp.code.memberMap<modelType> jsonMemberMap = fastCSharp.code.memberMap<modelType>.New();
                /// <summary>
                /// JSON解析成员位图参数访问锁
                /// </summary>
                private static readonly object jsonMemberMapLock = new object();
                /// <summary>
                /// 成员名称索引查找数据
                /// </summary>
                private static readonly pointer.reference searcher = fastCSharp.stateSearcher.charsSearcher.Create(sqlModel<modelType>.Fields.getArray(value => value.Field.Name), true).Reference;
                /// <summary>
                /// 数据库表格操作工具
                /// </summary>
                private sqlTool<valueType, modelType> sqlTool;
                /// <summary>
                /// 字段名称集合
                /// </summary>
                public string[] FieldNames
                {
                    get { return sqlModel<modelType>.Fields.getArray(value => value.Field.Name); }
                }
                /// <summary>
                /// 自增字段名称
                /// </summary>
                public string IdentityName
                {
                    get { return sqlModel<modelType>.Identity == null ? null : sqlModel<modelType>.Identity.Field.Name; }
                }
                /// <summary>
                /// 关键字名称集合
                /// </summary>
                public string[] PrimaryKeyNames
                {
                    get { return sqlModel<modelType>.PrimaryKeys.getArray(value => value.Field.Name); }
                }
                /// <summary>
                /// 数据库表格操作工具
                /// </summary>
                /// <param name="sqlTool">数据库表格操作工具</param>
                private sqlTable(sqlTool<valueType, modelType> sqlTool)
                {
                    this.sqlTool = sqlTool;
                }
                /// <summary>
                /// JSON解析
                /// </summary>
                /// <param name="value"></param>
                /// <param name="json"></param>
                /// <returns></returns>
                private bool parseJson(modelType value, string json)
                {
                    Monitor.Enter(jsonConfig);
                    try
                    {
                        jsonConfig.MemberMap = jsonMemberMap;
                        return jsonParser.Parse<modelType>(json, ref value, jsonConfig);
                    }
                    finally { Monitor.Exit(jsonConfig); }
                }
                /// <summary>
                /// 根据JSON查询数据
                /// </summary>
                /// <param name="json"></param>
                /// <returns>查询数据的JSON字符串</returns>
                public keyValue<string, string>[] Query(string json)
                {
                    valueType value = fastCSharp.emit.constructor<valueType>.New();
                    Monitor.Enter(jsonMemberMapLock);
                    try
                    {
                        if (parseJson(value, json))
                        {
                            if (sqlModel<modelType>.Identity != null && jsonMemberMap.IsMember(sqlModel<modelType>.Identity.MemberMapIndex))
                            {
                                value = sqlTool.GetByIdentity(value);
                            }
                            else if (sqlModel<modelType>.PrimaryKeys.Length != 0) value = sqlTool.GetByPrimaryKey(value);
                            else value = null;
                        }
                        else value = null;
                    }
                    finally
                    {
                        jsonMemberMap.Clear();
                        Monitor.Exit(jsonMemberMapLock);
                    }
                    if (value != null)
                    {
                        return sqlModel<modelType>.Fields.getArray(field => new keyValue<string, string>(field.Field.Name, jsonSerializer.ObjectToJson(field.Field.GetValue(value))));
                    }
                    return null;
                }
                /// <summary>
                /// 根据JSON更新数据
                /// </summary>
                /// <param name="json"></param>
                /// <param name="values"></param>
                /// <returns>更新是否成功</returns>
                public bool Update(string json, keyValue<string, string>[] values)
                {
                    valueType value = fastCSharp.emit.constructor<valueType>.New();
                    int isUpdate = 0;
                    Monitor.Enter(jsonMemberMapLock);
                    try
                    {
                        if (parseJson(value, json))
                        {
                            if (sqlModel<modelType>.Identity != null && jsonMemberMap.IsMember(sqlModel<modelType>.Identity.MemberMapIndex)) isUpdate = 1;
                            else if (sqlModel<modelType>.PrimaryKeys.Length != 0) isUpdate = 2;
                            if (isUpdate != 0)
                            {
                                fastCSharp.stateSearcher.charsSearcher nameSearcher = new fastCSharp.stateSearcher.charsSearcher(searcher);
                                foreach (keyValue<string, string> nameValue in values)
                                {
                                    int index = nameSearcher.Search(nameValue.Key);
                                    if (index != -1)
                                    {
                                        fastCSharp.code.cSharp.sqlModel.fieldInfo field = sqlModel<modelType>.Fields[index];
                                        field.Field.SetValue(value, jsonParser.ParseType(field.Field.FieldType, nameValue.Value));
                                        jsonMemberMap.SetMember(field.MemberMapIndex);
                                    }
                                }
                                return isUpdate == 1 ? sqlTool.UpdateByIdentity(value, jsonMemberMap) : sqlTool.UpdateByPrimaryKey(value, jsonMemberMap);
                            }
                        }
                    }
                    finally
                    {
                        jsonMemberMap.Clear();
                        Monitor.Exit(jsonMemberMapLock);
                    }
                    return false;
                }
                /// <summary>
                /// 根据JSON更新数据
                /// </summary>
                /// <param name="json"></param>
                /// <returns>更新是否成功</returns>
                public bool Delete(string json)
                {
                    valueType value = fastCSharp.emit.constructor<valueType>.New();
                    bool isIdentity = false, isPrimaryKey = false;
                    Monitor.Enter(jsonMemberMapLock);
                    try
                    {
                        if (parseJson(value, json))
                        {
                            if (sqlModel<modelType>.Identity != null && jsonMemberMap.IsMember(sqlModel<modelType>.Identity.MemberMapIndex))
                            {
                                isIdentity = true;
                            }
                            else if (sqlModel<modelType>.PrimaryKeys.Length != 0) isPrimaryKey = true;
                        }
                    }
                    finally
                    {
                        jsonMemberMap.Clear();
                        Monitor.Exit(jsonMemberMapLock);
                    }
                    if (isIdentity) return sqlTool.DeleteByIdentity(value);
                    if (isPrimaryKey) return sqlTool.DeleteByPrimaryKey(value);
                    return false;
                }
                /// <summary>
                /// 获取数据库表格操作工具
                /// </summary>
                /// <returns>数据库表格操作工具</returns>
                private static ISqlTable get()
                {
                    if (sqlModel<modelType>.Identity != null || sqlModel<modelType>.PrimaryKeys.Length != 0)
                    {
                        FieldInfo field = typeof(valueType).GetField(fastCSharp.emit.sqlTable.SqlTableName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                        if (field != null)
                        {
                            sqlTool<valueType, modelType> sqlTable = field.GetValue(null) as sqlTool<valueType, modelType>;
                            if (sqlTable != null) return new sqlTable<valueType, modelType>(sqlTable);
                        }
                    }
                    return null;
                }
            }
            /// <summary>
            /// 类型名称索引查找数据
            /// </summary>
            private pointer.size searcher;
            /// <summary>
            /// 数据库表格操作工具集合
            /// </summary>
            private ISqlTable[] sqlTables;
            /// <summary>
            /// 类型名称集合
            /// </summary>
            public string[] Names { get; private set; }
            /// <summary>
            /// JSON操作客户端
            /// </summary>
            /// <param name="assembly">数据库表格相关程序集</param>
            public jsonTool(Assembly assembly)
            {
                if (assembly == null) log.Error.Throw(log.exceptionType.Null);
                subArray<keyValue<string, ISqlTable>> sqlTables = default(subArray<keyValue<string, ISqlTable>>);
                foreach (Type type in assembly.GetTypes())
                {
                    sqlTable attribute = fastCSharp.code.typeAttribute.GetAttribute<sqlTable>(type, false, true);
                    if (attribute != null && Array.IndexOf(fastCSharp.config.sql.Default.CheckConnection, attribute.ConnectionType) != -1)
                    {
                        Type modelType;
                        fastCSharp.code.cSharp.sqlModel sqlModel = fastCSharp.code.cSharp.dataModel.GetModelType<fastCSharp.code.cSharp.sqlModel>(type, out modelType);
                        object sqlTable = typeof(sqlTable<,>).MakeGenericType(type, modelType ?? type)
                            .GetMethod("get", BindingFlags.Static | BindingFlags.NonPublic)
                            .Invoke(null, null);
                        if (sqlTable != null) sqlTables.Add(new keyValue<string, ISqlTable>(type.fullName(), (ISqlTable)sqlTable));
                    }
                }
                if (sqlTables.length != 0)
                {
                    sqlTables.Sort((left, right) => left.Key.CompareTo(right.Key));
                    this.sqlTables = sqlTables.GetArray(value => value.Value);
                    searcher = fastCSharp.stateSearcher.charsSearcher.Create(Names = sqlTables.GetArray(value => value.Key), false);
                }
            }
            /// <summary>
            /// 获取数据库表格操作工具
            /// </summary>
            /// <param name="type"></param>
            /// <returns>数据库表格操作工具</returns>
            private ISqlTable get(string type)
            {
                int index = new fastCSharp.stateSearcher.charsSearcher(ref searcher).Search(type);
                return index >= 0 ? sqlTables[index] : null;
            }
            /// <summary>
            /// 获取字段名称集合
            /// </summary>
            /// <param name="type"></param>
            /// <param name="identityName">自增字段名称</param>
            /// <param name="primaryKeyNames">关键字名称集合</param>
            /// <returns>字段名称集合</returns>
            public string[] GetFields(string type, ref string identityName, ref string[] primaryKeyNames)
            {
                ISqlTable sqlTable = get(type);
                if (sqlTable != null)
                {
                    identityName = sqlTable.IdentityName;
                    primaryKeyNames = sqlTable.PrimaryKeyNames;
                    return sqlTable.FieldNames;
                }
                return null;
            }
            /// <summary>
            /// 根据JSON查询数据
            /// </summary>
            /// <param name="type"></param>
            /// <param name="json"></param>
            /// <returns>查询数据的JSON字符串</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public keyValue<string, string>[] Query(string type, string json)
            {
                ISqlTable sqlTable = get(type);
                return sqlTable != null ? sqlTable.Query(json) : null;
            }
            /// <summary>
            /// 根据JSON更新数据
            /// </summary>
            /// <param name="type"></param>
            /// <param name="json"></param>
            /// <param name="values"></param>
            /// <returns>更新是否成功</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public bool Update(string type, string json, keyValue<string, string>[] values)
            {
                ISqlTable sqlTable = get(type);
                return sqlTable != null && sqlTable.Update(json, values);
            }
            /// <summary>
            /// 根据JSON更新数据
            /// </summary>
            /// <param name="type"></param>
            /// <param name="json"></param>
            /// <returns>更新是否成功</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public bool Delete(string type, string json)
            {
                ISqlTable sqlTable = get(type);
                return sqlTable != null && sqlTable.Delete(json);
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                unmanaged.Free(ref searcher);
            }
        }
    }
    /// <summary>
    /// 数据库表格操作工具
    /// </summary>
    /// <typeparam name="valueType">表格类型</typeparam>
    /// <typeparam name="modelType">模型类型</typeparam>
    public class sqlTable<valueType, modelType> : sqlTable.sqlTool<valueType, modelType>
        where valueType : class, modelType
        where modelType : class
    {
        /// <summary>
        /// 数据库表格操作工具
        /// </summary>
        /// <param name="sqlTable">数据库表格配置</param>
        /// <param name="client">SQL操作客户端</param>
        /// <param name="tableName">表格名称</param>
        protected sqlTable(sqlTable sqlTable, client client, string tableName)
            : base(sqlTable, client, tableName)
        {
        }
        /// <summary>
        /// 获取数据库表格操作工具
        /// </summary>
        /// <returns>数据库表格操作工具</returns>
        public static sqlTable<valueType, modelType> Get()
        {
            Type type = typeof(valueType);
            sqlTable sqlTable = fastCSharp.code.typeAttribute.GetAttribute<sqlTable>(type, false, true);
            if (sqlTable != null && Array.IndexOf(fastCSharp.config.sql.Default.CheckConnection, sqlTable.ConnectionType) != -1)
            {
                return new sqlTable<valueType, modelType>(sqlTable, connection.GetConnection(sqlTable.ConnectionType).Client, sqlTable.GetTableName(type));
            }
            return null;
        }
    }
    
    /// <summary>
    /// 数据库表格操作工具
    /// </summary>
    /// <typeparam name="modelType">模型类型</typeparam>
    public sealed class sqlModelTable<modelType> : sqlTable<modelType, modelType>
        where modelType : class
    {
        /// <summary>
        /// 数据库表格操作工具
        /// </summary>
        /// <param name="sqlTable">数据库表格配置</param>
        /// <param name="client">SQL操作客户端</param>
        /// <param name="tableName">表格名称</param>
        private sqlModelTable(sqlTable sqlTable, client client, string tableName)
            : base(sqlTable, client, tableName)
        {
        }
        /// <summary>
        /// 获取数据库表格操作工具
        /// </summary>
        /// <returns>数据库表格操作工具</returns>
        public new static sqlModelTable<modelType> Get()
        {
            Type type = typeof(modelType);
            sqlTable sqlTable = fastCSharp.code.typeAttribute.GetAttribute<sqlTable>(type, false, true);
            if (sqlTable != null && Array.IndexOf(fastCSharp.config.sql.Default.CheckConnection, sqlTable.ConnectionType) != -1)
            {
                return new sqlModelTable<modelType>(sqlTable, connection.GetConnection(sqlTable.ConnectionType).Client, sqlTable.GetTableName(type));
            }
            return null;
        }
    }
    /// <summary>
    /// 数据库表格操作工具
    /// </summary>
    /// <typeparam name="valueType">表格类型</typeparam>
    /// <typeparam name="modelType">模型类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    public class sqlTable<valueType, modelType, keyType> : sqlTable.sqlTool<valueType, modelType>
        where valueType : class, modelType
        where modelType : class
        where keyType : IEquatable<keyType>
    {
        /// <summary>
        /// 设置关键字
        /// </summary>
        private Action<modelType, keyType> setPrimaryKey;
        /// <summary>
        /// 获取关键字
        /// </summary>
        public Func<modelType, keyType> GetPrimaryKey { get; private set; }
        /// <summary>
        /// 数据库表格操作工具
        /// </summary>
        /// <param name="sqlTable">数据库表格配置</param>
        /// <param name="client">SQL操作客户端</param>
        /// <param name="tableName">表格名称</param>
        protected sqlTable(sqlTable sqlTable, client client, string tableName)
            : base(sqlTable, client, tableName)
        {
            FieldInfo[] primaryKeys = sqlModel<modelType>.PrimaryKeys.getArray(value => value.Field);
            GetPrimaryKey = databaseModel<modelType>.GetPrimaryKeyGetter<keyType>("GetSqlPrimaryKey", primaryKeys);
            setPrimaryKey = databaseModel<modelType>.GetPrimaryKeySetter<keyType>("SetSqlPrimaryKey", primaryKeys);
        }
        /// <summary>
        /// 获取数据库表格操作工具
        /// </summary>
        /// <returns>数据库表格操作工具</returns>
        public static sqlTable<valueType, modelType, keyType> Get()
        {
            Type type = typeof(valueType);
            sqlTable sqlTable = fastCSharp.code.typeAttribute.GetAttribute<sqlTable>(type, false, true);
            if (sqlTable != null && Array.IndexOf(fastCSharp.config.sql.Default.CheckConnection, sqlTable.ConnectionType) != -1)
            {
                return new sqlTable<valueType, modelType, keyType>(sqlTable, connection.GetConnection(sqlTable.ConnectionType).Client, sqlTable.GetTableName(type));
            }
            return null;
        }
        /// <summary>
        /// 根据关键字获取数据对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="memberMap">成员位图</param>
        /// <returns>数据对象</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType GetByPrimaryKey(keyType key, fastCSharp.code.memberMap<modelType> memberMap = null)
        {
            valueType value = fastCSharp.emit.constructor<valueType>.New();
            setPrimaryKey(value, key);
            return GetByPrimaryKey(value, memberMap);
        }
        /// <summary>
        /// 修改数据库记录
        /// </summary>
        /// <param name="key">SQL关键字</param>
        /// <param name="updateExpression">SQL表达式</param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
        /// <returns>修改后的数据,失败返回null</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType Update(keyType key, sqlTable.updateExpression updateExpression, bool isIgnoreTransaction = false)
        {
            return Update(key, ref updateExpression, isIgnoreTransaction);
        }
        /// <summary>
        /// 修改数据库记录
        /// </summary>
        /// <param name="key">SQL关键字</param>
        /// <param name="updateExpression">SQL表达式</param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
        /// <returns>修改后的数据,失败返回null</returns>
        public valueType Update(keyType key, ref sqlTable.updateExpression updateExpression, bool isIgnoreTransaction = false)
        {
            if (updateExpression.Count != 0)
            {
                valueType value = fastCSharp.emit.constructor<valueType>.New();
                setPrimaryKey(value, key);
                return UpdateByPrimaryKey(value, ref updateExpression, isIgnoreTransaction);
            }
            return null;
        }
        /// <summary>
        /// 修改数据库记录
        /// </summary>
        /// <param name="key">SQL关键字</param>
        /// <param name="expression">SQL表达式</param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
        /// <returns>修改后的数据,失败返回null</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType Update<returnType>(keyType key, Expression<Func<modelType, returnType>> expression, bool isIgnoreTransaction = false)
        {
            return expression != null ? Update(key, UpdateExpression(expression), isIgnoreTransaction) : null;
        }
        /// <summary>
        /// 修改数据库记录
        /// </summary>
        /// <param name="key">SQL关键字</param>
        /// <param name="expression">字段表达式</param>
        /// <param name="returnValue">更新数据字段值</param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
        /// <returns>修改后的数据,失败返回null</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType Update<returnType>(keyType key, Expression<Func<modelType, returnType>> expression, returnType returnValue, bool isIgnoreTransaction = false)
        {
            return expression != null ? Update(key, UpdateExpression(expression, returnValue), isIgnoreTransaction) : null;
        }
        /// <summary>
        /// 修改数据库记录
        /// </summary>
        /// <param name="key">SQL关键字</param>
        /// <param name="expressions">SQL表达式</param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
        /// <returns>修改后的数据,失败返回null</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType Update<returnType>(keyType key, bool isIgnoreTransaction = false, params Expression<Func<modelType, returnType>>[] expressions)
        {
            return expressions.Length != 0 ? Update(key, UpdateExpression(expressions), isIgnoreTransaction) : null;
        }
        /// <summary>
        /// 修改数据库记录
        /// </summary>
        /// <param name="key">SQL关键字</param>
        /// <param name="isIgnoreTransaction">是否忽略应用程序事务</param>
        /// <returns>修改后的数据,失败返回null</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool Delete(keyType key, bool isIgnoreTransaction = false)
        {
            valueType value = fastCSharp.emit.constructor<valueType>.New();
            setPrimaryKey(value, key);
            return DeleteByPrimaryKey(value, isIgnoreTransaction);
        }
    }
    /// <summary>
    /// 数据库表格操作工具
    /// </summary>
    /// <typeparam name="modelType">模型类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    public sealed class sqlModelTable<modelType, keyType> : sqlTable<modelType, modelType, keyType>
        where modelType : class
        where keyType : IEquatable<keyType>
    {
        /// <summary>
        /// 数据库表格操作工具
        /// </summary>
        /// <param name="sqlTable">数据库表格配置</param>
        /// <param name="client">SQL操作客户端</param>
        /// <param name="tableName">表格名称</param>
        private sqlModelTable(sqlTable sqlTable, client client, string tableName)
            : base(sqlTable, client, tableName)
        {
        }
        /// <summary>
        /// 获取数据库表格操作工具
        /// </summary>
        /// <returns>数据库表格操作工具</returns>
        public new static sqlModelTable<modelType, keyType> Get()
        {
            Type type = typeof(modelType);
            sqlTable sqlTable = fastCSharp.code.typeAttribute.GetAttribute<sqlTable>(type, false, true);
            if (sqlTable != null && Array.IndexOf(fastCSharp.config.sql.Default.CheckConnection, sqlTable.ConnectionType) != -1)
            {
                return new sqlModelTable<modelType, keyType>(sqlTable, connection.GetConnection(sqlTable.ConnectionType).Client, sqlTable.GetTableName(type));
            }
            return null;
        }
    }
}
