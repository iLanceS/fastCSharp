using System;
using fastCSharp.reflection;
using System.Collections.Generic;
using System.Reflection;
using fastCSharp.emit;
using fastCSharp.threading;
using System.Runtime.CompilerServices;

namespace fastCSharp.code.cSharp
{
    /// <summary>
    /// 数据库表格模型配置
    /// </summary>
    public class sqlModel : dataModel
    {
        /// <summary>
        /// 日志队列TCP调用名称
        /// </summary>
        public string LogTcpCallService;
        /// <summary>
        /// 删除列集合
        /// </summary>
        public string DeleteColumnNames;
        /// <summary>
        /// 是否生成默认序列化配置
        /// </summary>
        public bool IsDefaultSerialize = true;
        /// <summary>
        /// 默认二进制序列化是否序列化成员位图
        /// </summary>
        public bool IsDefaultSerializeIsMemberMap;
        /// <summary>
        /// 日志序列化是否需要支持成员位图
        /// </summary>
        public bool IsLogMemberMap = true;
        /// <summary>
        /// 日志流是否客户端队列模式
        /// </summary>
        public bool IsLogClientQueue;
        /// <summary>
        /// 是否绑定成员缓存类型
        /// </summary>
        public bool IsMemberCache;
        /// <summary>
        /// 默认缓存类型
        /// </summary>
        public enum cacheType : byte
        {
            /// <summary>
            /// 未知/自定义缓存类型
            /// </summary>
            Unknown,
            /// <summary>
            /// 将 ID 作为数组索引的缓存， fastCSharp.sql.cache.whole.events.identityArray&lt;valueType, modelType, valueType&gt;
            /// </summary>
            IdentityArray,
            /// <summary>
            /// 将 ID 作为数组索引并且支持分页查询的缓存， fastCSharp.sql.cache.whole.events.identityTree&lt;valueType, modelType, valueType&gt;
            /// </summary>
            IdentityTree,
            /// <summary>
            /// K-V 缓存， fastCSharp.sql.cache.whole.events.identityTree&lt;valueType, modelType, valueType, keyType&gt;
            /// </summary>
            PrimaryKey,
            /// <summary>
            /// 将 ID 作为数组索引的缓存， fastCSharp.sql.cache.whole.events.identityArray&lt;valueType, modelType, valueType&gt;
            /// </summary>
            CreateIdentityArray,
            /// <summary>
            /// 将 ID 作为数组索引的缓存， fastCSharp.sql.cache.whole.events.identityArrayWhere&lt;valueType, modelType, valueType&gt;
            /// </summary>
            CreateIdentityArrayWhere,
            /// <summary>
            /// 将 ID 作为数组索引并且支持分页查询的缓存， fastCSharp.sql.cache.whole.events.identityTree&lt;valueType, modelType, valueType&gt;
            /// </summary>
            CreateIdentityTree,
            /// <summary>
            /// K-V 缓存， fastCSharp.sql.cache.whole.events.identityTree&lt;valueType, modelType, valueType, keyType&gt;
            /// </summary>
            CreatePrimaryKey,
            /// <summary>
            /// 成员绑定缓存， fastCSharp.sql.cache.whole.events.memberKey&lt;valueType, modelType, memberCacheType, keyType, memberKeyType, targetType&gt;
            /// </summary>
            CreateMemberKey,
        }
        /// <summary>
        /// 默认缓存类型
        /// </summary>
        public cacheType CacheType;
        /// <summary>
        /// 计数缓存类型
        /// </summary>
        public enum cacheCounterType : byte
        {
            /// <summary>
            /// 未知/自定义缓存类型
            /// </summary>
            Unknown,
            /// <summary>
            /// ID 计数缓存，fastCSharp.sql.cache.part.events.identityCounter&lt;valueType, modelType&gt;
            /// </summary>
            IdentityCounter,
            /// <summary>
            /// 关键字计数缓存，fastCSharp.sql.cache.part.events.primaryKeyCounter&lt;valueType, modelType, primaryKey&gt;
            /// </summary>
            PrimaryKeyCounter,
            /// <summary>
            /// 先进先出优先队列缓存，fastCSharp.sql.cache.part.memberQueue&lt;valueType, modelType, memberCacheType, int&gt;
            /// </summary>
            CreateIdentityCounterMemberQueue,
            /// <summary>
            /// 先进先出优先队列缓存，fastCSharp.sql.cache.part.queue&lt;valueType, modelType, int&gt;
            /// </summary>
            CreateIdentityCounterQueue,
            /// <summary>
            /// 先进先出优先队列缓存，fastCSharp.sql.cache.part.queueList&lt;valueType, modelType, counterKeyType, keyType&gt;
            /// </summary>
            CreateIdentityCounterQueueList,
            /// <summary>
            /// 先进先出优先队列缓存，fastCSharp.sql.cache.part.queue&lt;valueType, modelType, primaryKey&gt;
            /// </summary>
            CreatePrimaryKeyCounterQueue,
            /// <summary>
            /// 先进先出优先队列缓存，fastCSharp.sql.cache.part.queueList&lt;valueType, modelType, primaryKey, keyType&gt;
            /// </summary>
            CreatePrimaryKeyCounterQueueList,
            /// <summary>
            /// 先进先出优先队列缓存，fastCSharp.sql.cache.part.queueDictionary&lt;valueType, modelType, primaryKey, keyType, dictionaryKeyType&gt;
            /// </summary>
            CreatePrimaryKeyCounterQueueDictionary
        }
        /// <summary>
        /// 默认计数缓存类型
        /// </summary>
        public cacheCounterType CacheCounterType;
        /// <summary>
        /// 是否默认加载缓存事件
        /// </summary>
        public bool IsLoadedCache = true;
        /// <summary>
        /// 默认空属性
        /// </summary>
        internal static readonly sqlModel Default = new sqlModel();
        /// <summary>
        /// 空自增列成员索引
        /// </summary>
        internal const int NullIdentityMemberIndex = -1;

        /// <summary>
        /// 字段信息
        /// </summary>
        public class fieldInfo
        {
            /// <summary>
            /// 字段信息
            /// </summary>
            public FieldInfo Field { get; private set; }
            /// <summary>
            /// 可空类型数据库数据类型
            /// </summary>
            internal Type NullableDataType;
            /// <summary>
            /// 数据库数据类型
            /// </summary>
            internal Type DataType;
            /// <summary>
            /// 数据库成员信息
            /// </summary>
            public dataMember DataMember { get; private set; }
            /// <summary>
            /// 数据读取函数
            /// </summary>
            internal MethodInfo DataReaderMethod;
            /// <summary>
            /// 数据转换SQL字符串函数信息
            /// </summary>
            internal MethodInfo ToSqlMethod;
            /// <summary>
            /// 数据转换SQL字符串之前的类型转换函数信息
            /// </summary>
            internal MethodInfo ToSqlCastMethod;
            /// <summary>
            /// 数据转换成对象之前的类型转换函数信息
            /// </summary>
            internal MethodInfo ToModelCastMethod;
            /// <summary>
            /// 成员位图索引
            /// </summary>
            internal int MemberMapIndex;
            /// <summary>
            /// 是否数据列
            /// </summary>
            internal bool IsSqlColumn;
            /// <summary>
            /// 是否默认JSON
            /// </summary>
            internal bool IsUnknownJson;
            /// <summary>
            /// 是否需要验证
            /// </summary>
            internal bool IsVerify
            {
                get
                {
                    if (IsSqlColumn)
                    {
                        bool isVerify;
                        if (verifyTypes.TryGetValue(DataType, out isVerify)) return isVerify;
                        isVerify = typeof(sqlColumn<>.verify).MakeGenericType(DataType).GetField("verifyer", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) != null
                            || typeof(sqlColumn<>).MakeGenericType(DataType).GetField("custom", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) != null;
                        verifyTypes.Set(DataType, isVerify);
                        return isVerify;
                    }
                    if (!DataMember.IsDefaultMember)
                    {
                        if (DataType == typeof(string)) return DataMember.MaxStringLength > 0;
                        return DataType.IsClass && !DataMember.IsNull;
                    }
                    return false;
                }
            }
            /// <summary>
            /// SQL名称
            /// </summary>
            internal string SqlFieldName;
            /// <summary>
            /// 获取数据列名称
            /// </summary>
            internal Func<string, string> getSqlColumnName;
            /// <summary>
            /// 获取数据列名称
            /// </summary>
            /// <returns></returns>
            public string GetSqlColumnName()
            {
                if (getSqlColumnName == null) getSqlColumnName = sqlColumn.insertDynamicMethod.GetColumnNames(Field.FieldType);
                return getSqlColumnName(Field.Name);
            }
            /// <summary>
            /// 字段信息
            /// </summary>
            /// <param name="field">字段信息</param>
            /// <param name="attribute">数据库成员信息</param>
            internal fieldInfo(fieldIndex field, dataMember attribute)
            {
                Field = field.Member;
                MemberMapIndex = field.MemberIndex;
                DataMember = dataMember.FormatSql(attribute, Field.FieldType, ref IsSqlColumn);
                if ((NullableDataType = DataMember.DataType) == null) NullableDataType = Field.FieldType;
                if ((DataReaderMethod = fastCSharp.emit.pub.GetDataReaderMethod(DataType = NullableDataType.nullableType() ?? NullableDataType)) == null)
                {
                    if (IsSqlColumn)
                    {
                        if (isSqlColumn(DataType)) return;
                        IsSqlColumn = false;
                    }
#if NOJIT
                    DataType = NullableDataType = typeof(string);
                    DataReaderMethod = fastCSharp.emit.pub.GetDataReaderMethodInfo;
#else
                    DataReaderMethod = fastCSharp.emit.pub.GetDataReaderMethod(DataType = NullableDataType = typeof(string));
#endif
                    IsUnknownJson = true;
                    ToSqlCastMethod = fastCSharp.emit.jsonSerializer.SqlToJsonMethod.MakeGenericMethod(Field.FieldType);
                    ToModelCastMethod = fastCSharp.emit.jsonParser.SqlParseMethod.MakeGenericMethod(Field.FieldType);
                }
                else
                {
                    ToSqlCastMethod = fastCSharp.emit.pub.GetCastMethod(Field.FieldType, DataType);
                    ToModelCastMethod = fastCSharp.emit.pub.GetCastMethod(DataType, Field.FieldType);
                }
                SqlFieldName = new fastCSharp.stateSearcher.asciiSearcher(keywords).Search(Field.Name.ToLower()) < 0 ? Field.Name : toSqlName(Field.Name);
                ToSqlMethod = fastCSharp.emit.pub.GetSqlConverterMethod(DataType);
            }
            /// <summary>  
            /// 数据列验证类型集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, bool> verifyTypes = new interlocked.dictionary<Type,bool>();
            /// <summary>
            /// 是否有效数据列
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            private static bool isSqlColumn(Type type)
            {
                bool isType;
                if (sqlColumnTypes.TryGetValue(type, out isType)) return isType;
                isType = typeof(sqlColumn<>.set).MakeGenericType(type).GetField("defaultSetter", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) != null
                    || typeof(sqlColumn<>).MakeGenericType(type).GetField("custom", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) != null;
                sqlColumnTypes.Set(type, isType);
                return isType;
            }
            /// <summary>
            /// 数据列类型集合
            /// </summary>
            private static readonly interlocked.dictionary<Type, bool> sqlColumnTypes = new interlocked.dictionary<Type, bool>();
        }
        /// <summary>
        /// 获取字段成员集合
        /// </summary>
        /// <param name="fields"></param>
        /// <returns>字段成员集合</returns>
        internal static subArray<fieldInfo> GetFields(fieldIndex[] fields)
        {
            subArray<fieldInfo> values = new subArray<fieldInfo>(fields.Length);
            foreach (fieldIndex field in fields)
            {
                Type type = field.Member.FieldType;
                if (!type.IsPointer && (!type.IsArray || type.GetArrayRank() == 1) && !field.IsIgnore)
                {
                    dataMember attribute = field.GetAttribute<dataMember>(true, true);
                    if (attribute == null || attribute.IsSetup) values.Add(new fieldInfo(field, attribute));
                }
            }
            return values;
        }
        /// <summary>
        /// 获取关键字集合
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        internal static subArray<fieldInfo> GetPrimaryKeys(fieldInfo[] fields)
        {
            return fields.getFind(value => value.DataMember.PrimaryKeyIndex != 0)
                .Sort((left, right) =>
                {
                    int value = left.DataMember.PrimaryKeyIndex - right.DataMember.PrimaryKeyIndex;
                    return value == 0 ? left.Field.Name.CompareTo(right.Field.Name) : value;
                });
        }
        /// <summary>
        /// 获取自增标识
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static fieldInfo GetIdentity(fieldInfo[] fields)
        {
            fieldInfo identity = null;
            int isCase = 0;
            foreach (fieldInfo field in fields)
            {
                if (field.DataMember.IsIdentity) return field;
                if (isCase == 0 && field.Field.Name == fastCSharp.config.sql.Default.DefaultIdentityName)
                {
                    identity = field;
                    isCase = 1;
                }
                else if (identity == null && field.Field.Name.ToLower() == fastCSharp.config.sql.Default.DefaultIdentityName) identity = field;
            }
            return identity;
        }

        /// <summary>
        /// 获取数据库成员信息集合 
        /// </summary>
        /// <param name="type">数据库绑定类型</param>
        /// <param name="database">数据库配置</param>
        /// <returns>数据库成员信息集合</returns>
        internal static keyValue<memberIndex, dataMember>[] GetMemberIndexs<attributeType>(Type type, attributeType database)
             where attributeType : memberFilter
        {//showjim
            return GetMembers(memberIndexGroup.Get(type).Find<dataMember>(database));
        }
        /// <summary>
        /// 获取数据库成员信息集合
        /// </summary>
        /// <typeparam name="memberType">成员类型</typeparam>
        /// <param name="members">成员集合</param>
        /// <returns>数据库成员信息集合</returns>
        public static keyValue<memberType, dataMember>[] GetMembers<memberType>(memberType[] members) where memberType : memberIndex
        {
            return members.getFind(value => value.CanSet && value.CanGet)
                .GetArray(value => new keyValue<memberType, dataMember>(value, dataMember.Get(value)));
        }
        /// <summary>
        /// SQL关键字
        /// </summary>
        private static readonly pointer.reference keywords = fastCSharp.stateSearcher.asciiSearcher.Create(new string[] { "add", "all", "alter", "and", "any", "as", "asc", "authorization", "backup", "begin", "between", "break", "browse", "bulk", "by", "cascade", "case", "check", "checkpoint", "close", "clustered", "coalesce", "collate", "column", "commit", "compute", "constraint", "contains", "containstable", "continue", "convert", "create", "cross", "current", "current_date", "current_time", "current_timestamp", "current_user", "cursor", "database", "dbcc", "deallocate", "declare", "default", "delete", "deny", "desc", "disk", "distinct", "distributed", "double", "drop", "dump", "else", "end", "errlvl", "escape", "except", "exec", "execute", "exists", "exit", "external", "fetch", "file", "fillfactor", "for", "foreign", "freetext", "freetexttable", "full", "function", "goto", "grant", "group", "having", "holdlock", "identity", "identity_insert", "identitycol", "if", "in", "index", "inner", "insert", "intersect", "into", "is", "join", "key", "kill", "left", "like", "lineno", "load", "merge", "national", "nocheck", "nonclustered", "not", "null", "nullif", "of", "off", "offsets", "on", "open", "opendatasource", "openquery", "openrowset", "openxml", "option", "or", "order", "outer", "over", "percent", "pivot", "plan", "precision", "primary", "print", "proc", "procedure", "public", "raiserror", "read", "readtext", "reconfigure", "references", "replication", "restore", "restrict", "return", "revert", "revoke", "right", "rollback", "rowcount", "rowguidcol", "rule", "save", "schema", "securityaudit", "select", "semantickeyphrasetable", "semanticsimilaritydetailstable", "semanticsimilaritytable", "session_user", "set", "setuser", "shutdown", "some", "statistics", "system_user", "table", "tablesample", "textsize", "then", "top", "tran", "transaction", "trigger", "truncate", "try_convert", "tsequal", "union", "unique", "unpivot", "update", "updatetext", "use", "user", "values", "varying", "view", "waitfor", "when", "where", "while", "with", "within", "writetext" }, true).Reference;
        /// <summary>
        /// SQL名称关键字处理
        /// </summary>
        /// <param name="name"></param>
        /// <returns>SQL名称</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static string ToSqlName(string name)
        {
            return new fastCSharp.stateSearcher.asciiSearcher(keywords).Search(name.ToLower()) < 0 ? name : toSqlName(name);
        }
        /// <summary>
        /// SQL名称关键字处理
        /// </summary>
        /// <param name="name"></param>
        /// <returns>SQL名称</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static string toSqlName(string name)
        {
            return name + "_k";
        }
    }
}
