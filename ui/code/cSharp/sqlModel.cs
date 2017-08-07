using System;
using fastCSharp.emit;
using fastCSharp.reflection;
using System.Collections.Generic;
using System.Reflection;

namespace fastCSharp.code
{
    /// <summary>
    /// 数据库表格模型
    /// </summary>
    internal partial class sqlModel
    {
        /// <summary>
        /// 数据库表格模型码生成
        /// </summary
        [auto(Name = "数据库表格模型", DependType = typeof(coder.cSharper), IsAuto = true)]
        internal sealed partial class cSharp : dataModel.cSharp<fastCSharp.code.cSharp.sqlModel>
        {
            /// <summary>
            /// 日志流字段
            /// </summary>
            private struct sqlStreamMember
            {
                /// <summary>
                /// 成员信息
                /// </summary>
                public memberInfo Member;
                /// <summary>
                /// 数据库成员信息
                /// </summary>
                public fastCSharp.emit.dataMember Attribute;
                /// <summary>
                /// 成员位图名称
                /// </summary>
                public string MemberMapName
                {
                    get { return "_m" + Member.MemberIndex.toString(); }
                }
                /// <summary>
                /// 成员加载函数名称
                /// </summary>
                public string MemberLoadedMethodName
                {
                    get { return Member.MemberName + "Loaded"; }
                }
                /// <summary>
                /// 是否生成日志流计数
                /// </summary>
                public bool IsSqlStreamCount
                {
                    get { return Attribute.SqlStreamCountType != null || Attribute.IsSqlStreamCount; }
                }
                /// <summary>
                /// 日志流计数完成类型
                /// </summary>
                public memberType SqlStreamCountType
                {
                    get { return Attribute.SqlStreamCountType == null ? null : (memberType)Attribute.SqlStreamCountType; }
                }
                /// <summary>
                /// 日志流计数完成类型表格编号
                /// </summary>
                public int SqlStreamCountTypeNumber
                {
                    get { return Attribute.SqlStreamCountTypeNumber; }
                }
                /// <summary>
                /// 计数总计 TCP 调用属性名称
                /// </summary>
                public string TotalCountMemberName
                {
                    get { return Member.MemberName + "Total"; }
                }
                /// <summary>
                /// 计数 TCP 调用函数名称
                /// </summary>
                public string GetCountMethodName
                {
                    get { return "get" + Member.MemberName; }
                }
                /// <summary>
                /// 增加计数 TCP 调用函数名称
                /// </summary>
                public string IncCountMethodName
                {
                    get { return "inc" + Member.MemberName; }
                }
            }
            /// <summary>
            /// 日志流字段
            /// </summary>
            private sqlStreamMember[] SqlStreamMembers;
            /// <summary>
            /// 日志流计数完成类型数量
            /// </summary>
            public int SqlStreamTypeCount;
            /// <summary>
            /// 日志流计数完成类型
            /// </summary>
            private struct sqlStreamCountType
            {
                /// <summary>
                /// 数据模型类型
                /// </summary>
                public memberType SqlStreamCountType;
                /// <summary>
                /// 表格编号，主要使用枚举识别同一数据模型下的不同表格
                /// </summary>
                public int SqlStreamCountTypeNumber;
            }
            /// <summary>
            /// 日志流计数完成类型集合
            /// </summary>
            private sqlStreamCountType[] SqlStreamCountTypes
            {
                get
                {
                    if (SqlStreamMembers != null)
                    {
                        HashSet<fastCSharp.emit.sqlTable.sqlStreamCountLoaderType> types = new HashSet<fastCSharp.emit.sqlTable.sqlStreamCountLoaderType>();
                        foreach (sqlStreamMember member in SqlStreamMembers)
                        {
                            if (member.Attribute.SqlStreamCountType != null) types.Add(new fastCSharp.emit.sqlTable.sqlStreamCountLoaderType(member.Attribute.SqlStreamCountType, member.Attribute.SqlStreamCountTypeNumber));
                        }
                        if (types.Count != 0) return types.getArray(value => new sqlStreamCountType { SqlStreamCountType = value.Type, SqlStreamCountTypeNumber = value.TableNumber });
                    }
                    return null;
                }
            }
            /// <summary>
            /// 索引成员
            /// </summary>
            private memberInfo[] IndexMembers;
            /// <summary>
            /// 当前时间成员
            /// </summary>
            private struct nowTimeMember
            {
                /// <summary>
                /// 成员信息
                /// </summary>
                public memberInfo Member;
                /// <summary>
                /// 当前时间成员名称
                /// </summary>
                public string NowTimeMemberName
                {
                    get
                    {
                        return Member.MemberName + "NowTime";
                    }
                }
            }
            /// <summary>
            /// 当前时间生成成员
            /// </summary>
            private nowTimeMember[] NowTimeMembers;
            /// <summary>
            /// 计数字段集合
            /// </summary>
            private sqlStreamMember[] CounterMembers;
            /// <summary>
            /// SQL表格操作工具 字段名称
            /// </summary>
            public string SqlTableName
            {
                get { return fastCSharp.emit.sqlTable.SqlTableName; }
            }
            /// <summary>
            /// SQL表格操作日志 字段名称
            /// </summary>
            public string SqlStreamName
            {
                get { return fastCSharp.emit.sqlTable.SqlStreamName; }
            }
            /// <summary>
            /// SQL表格计算列日志 字段名称
            /// </summary>
            public string SqlStreamLoadName
            {
                get { return fastCSharp.emit.sqlTable.SqlStreamLoadName; }
            }
            /// <summary>
            /// SQL表格计算列计数 字段名称
            /// </summary>
            public string SqlStreamCountName
            {
                get { return fastCSharp.emit.sqlTable.SqlStreamCountName; }
            }
            /// <summary>
            /// SQL表格默认缓存 字段名称
            /// </summary>
            public string IdentityArrayCacheName
            {
                get { return fastCSharp.emit.sqlTable.SqlCacheName; }
            }
            /// <summary>
            /// SQL表格默认缓存 字段名称
            /// </summary>
            public string IdentityTreeCacheName
            {
                get { return fastCSharp.emit.sqlTable.SqlCacheName; }
            }
            /// <summary>
            /// SQL表格默认缓存 字段名称
            /// </summary>
            public string PrimaryKeyCacheName
            {
                get { return fastCSharp.emit.sqlTable.SqlCacheName; }
            }
            /// <summary>
            /// SQL表格默认缓存 字段名称
            /// </summary>
            public string CreateIdentityArrayCacheName
            {
                get { return fastCSharp.emit.sqlTable.SqlCacheName; }
            }
            /// <summary>
            /// SQL表格默认缓存 字段名称
            /// </summary>
            public string CreateIdentityArrayMemberCacheName
            {
                get { return fastCSharp.emit.sqlTable.SqlCacheName; }
            }
            /// <summary>
            /// SQL表格默认缓存 字段名称
            /// </summary>
            public string CreateIdentityTreeCacheName
            {
                get { return fastCSharp.emit.sqlTable.SqlCacheName; }
            }
            /// <summary>
            /// SQL表格默认缓存 字段名称
            /// </summary>
            public string CreateIdentityTreeMemberCacheName
            {
                get { return fastCSharp.emit.sqlTable.SqlCacheName; }
            }
            /// <summary>
            /// SQL表格默认缓存 字段名称
            /// </summary>
            public string CreatePrimaryKeyCacheName
            {
                get { return fastCSharp.emit.sqlTable.SqlCacheName; }
            }
            /// <summary>
            /// SQL表格默认缓存 字段名称
            /// </summary>
            public string CreatePrimaryKeyMemberCacheName
            {
                get { return fastCSharp.emit.sqlTable.SqlCacheName; }
            }
            /// <summary>
            /// SQL表格默认缓存 字段名称
            /// </summary>
            public string CreateMemberKeyCacheName
            {
                get { return fastCSharp.emit.sqlTable.SqlCacheName; }
            }
            /// <summary>
            /// SQL表格默认缓存 字段名称
            /// </summary>
            public string CreateMemberKeyMemberCacheName
            {
                get { return fastCSharp.emit.sqlTable.SqlCacheName; }
            }
            /// <summary>
            /// 默认缓存类型
            /// </summary>
            public code.cSharp.sqlModel.cacheType CacheType
            {
                get
                {
                    switch (Attribute.CacheType)
                    {
                        case code.cSharp.sqlModel.cacheType.IdentityArray:
                        case code.cSharp.sqlModel.cacheType.IdentityTree:
                        case code.cSharp.sqlModel.cacheType.CreateIdentityArray:
                        case code.cSharp.sqlModel.cacheType.CreateIdentityTree:
                        case code.cSharp.sqlModel.cacheType.CreateIdentityArrayWhere:
                            if (Identity != null) return Attribute.CacheType;
                            break;
                        case code.cSharp.sqlModel.cacheType.PrimaryKey:
                        case code.cSharp.sqlModel.cacheType.CreatePrimaryKey:
                            if (PrimaryKeys.Count != 0) return Attribute.CacheType;
                            break;
                        case code.cSharp.sqlModel.cacheType.CreateMemberKey:
                            if (Identity != null || PrimaryKeys.Count != 0) return Attribute.CacheType;
                            break;
                    }
                    return code.cSharp.sqlModel.cacheType.Unknown;
                }
            }
            /// <summary>
            /// 是否 64b 自增标识
            /// </summary>
            public bool IsIdentity64
            {
                get
                {
                    Type type = Identity.Type;
                    return type == typeof(long) || type == typeof(ulong);
                }
            }
            /// <summary>
            /// 是否自动初始化当前时间
            /// </summary>
            public bool IsLoadNowTimeCache
            {
                get
                {
                    switch (CacheType)
                    {
                        case code.cSharp.sqlModel.cacheType.IdentityArray:
                        case code.cSharp.sqlModel.cacheType.IdentityTree:
                        case code.cSharp.sqlModel.cacheType.PrimaryKey:
                            return true;
                    }
                    return false;
                }
            }
            /// <summary>
            /// 是否生成数据加载完成代码
            /// </summary>
            public bool IsSqlCacheLoaded
            {
                get
                {
                    switch (CacheType)
                    {
                        case code.cSharp.sqlModel.cacheType.IdentityArray:
                        case code.cSharp.sqlModel.cacheType.IdentityTree:
                        case code.cSharp.sqlModel.cacheType.PrimaryKey:
                        case code.cSharp.sqlModel.cacheType.CreateIdentityArray:
                        case code.cSharp.sqlModel.cacheType.CreateIdentityTree:
                        case code.cSharp.sqlModel.cacheType.CreatePrimaryKey:
                        case code.cSharp.sqlModel.cacheType.CreateMemberKey:
                            return true;
                    }
                    return false;
                }
            }
            /// <summary>
            /// 是否生成数据加载完成代码
            /// </summary>
            public bool IsSqlLoaded
            {
                get
                {
                    return SqlStreamTypeCount != 0 || IsSqlCacheLoaded;
                }
            }
            /// <summary>
            /// 计数缓存类型
            /// </summary>
            public code.cSharp.sqlModel.cacheCounterType CacheCounterType
            {
                get
                {
                    switch (Attribute.CacheCounterType)
                    {
                        case code.cSharp.sqlModel.cacheCounterType.IdentityCounter:
                        case code.cSharp.sqlModel.cacheCounterType.CreateIdentityCounterQueue:
                        case code.cSharp.sqlModel.cacheCounterType.CreateIdentityCounterQueueList:
                            if (Identity != null) return Attribute.CacheCounterType;
                            break;
                        case code.cSharp.sqlModel.cacheCounterType.CreateIdentityCounterMemberQueue:
                            if (Identity != null && !IsIdentity64)
                            {
                                switch (CacheType)
                                {
                                    case code.cSharp.sqlModel.cacheType.CreateIdentityArray:
                                    case code.cSharp.sqlModel.cacheType.CreateIdentityTree:
                                        return Attribute.CacheCounterType;
                                }
                            }
                            break;
                        case code.cSharp.sqlModel.cacheCounterType.PrimaryKeyCounter:
                        case code.cSharp.sqlModel.cacheCounterType.CreatePrimaryKeyCounterQueue:
                        case code.cSharp.sqlModel.cacheCounterType.CreatePrimaryKeyCounterQueueList:
                        case code.cSharp.sqlModel.cacheCounterType.CreatePrimaryKeyCounterQueueDictionary:
                            if (PrimaryKeys.Count != 0) return Attribute.CacheCounterType;
                            break;
                    }
                    return code.cSharp.sqlModel.cacheCounterType.Unknown;
                }
            }
            /// <summary>
            /// 是否绑定成员缓存类型
            /// </summary>
            public bool IsMemberCache
            {
                get { return Attribute.IsMemberCache || CounterMembers.Length != 0 || CacheCounterType == code.cSharp.sqlModel.cacheCounterType.CreateIdentityCounterMemberQueue; }
            }
            /// <summary>
            /// 成员绑定类型约束基类
            /// </summary>
            public string MemberCacheBaseType
            {
                get
                {
                    if (CacheCounterType == code.cSharp.sqlModel.cacheCounterType.CreateIdentityCounterMemberQueue) return null;
                    return "class";
                }
            }
            /// <summary>
            /// SQL表格默认计数缓存 字段名称
            /// </summary>
            public string IdentityCounterCacheName
            {
                get { return fastCSharp.emit.sqlTable.SqlCacheCounterName; }
            }
            /// <summary>
            /// SQL表格默认计数缓存 字段名称
            /// </summary>
            public string IdentityCounter32CacheName
            {
                get { return fastCSharp.emit.sqlTable.SqlCacheCounterName; }
            }
            /// <summary>
            /// SQL表格默认计数缓存 字段名称
            /// </summary>
            public string PrimaryKeyCounterCacheName
            {
                get { return fastCSharp.emit.sqlTable.SqlCacheCounterName; }
            }
            /// <summary>
            /// SQL表格默认计数缓存 字段名称
            /// </summary>
            public string CreateIdentityCounterMemberQueueCacheName
            {
                get { return fastCSharp.emit.sqlTable.SqlCacheCounterName; }
            }
            /// <summary>
            /// SQL表格默认计数缓存 字段名称
            /// </summary>
            public string CreateIdentityCounterQueueCacheName
            {
                get { return fastCSharp.emit.sqlTable.SqlCacheCounterName; }
            }
            /// <summary>
            /// SQL表格默认计数缓存 字段名称
            /// </summary>
            public string CreateIdentityCounter32QueueCacheName
            {
                get { return fastCSharp.emit.sqlTable.SqlCacheCounterName; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string CreateIdentityCounterQueueMethodName
            {
                get { return "createIdentityCounterQueue"; }
            }
            /// <summary>
            /// SQL表格默认计数缓存 字段名称
            /// </summary>
            public string CreateIdentityCounterQueueListCacheName
            {
                get { return fastCSharp.emit.sqlTable.SqlCacheCounterName; }
            }
            /// <summary>
            /// SQL表格默认计数缓存 字段名称
            /// </summary>
            public string CreateIdentityCounter32QueueListCacheName
            {
                get { return fastCSharp.emit.sqlTable.SqlCacheCounterName; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string CreateIdentityCounterQueueListMethodName
            {
                get { return "createIdentityCounterQueueList"; }
            }
            /// <summary>
            /// SQL表格默认计数缓存 字段名称
            /// </summary>
            public string CreatePrimaryKeyCounterQueueCacheName
            {
                get { return fastCSharp.emit.sqlTable.SqlCacheCounterName; }
            }
            /// <summary>
            /// SQL表格默认计数缓存 字段名称
            /// </summary>
            public string CreatePrimaryKeyCounterQueueListCacheName
            {
                get { return fastCSharp.emit.sqlTable.SqlCacheCounterName; }
            }
            /// <summary>
            /// SQL表格默认计数缓存 字段名称
            /// </summary>
            public string CreatePrimaryKeyCounterQueueDictionaryCacheName
            {
                get { return fastCSharp.emit.sqlTable.SqlCacheCounterName; }
            }
            /// <summary>
            /// 默认二进制序列化是否序列化成员位图
            /// </summary>
            public bool IsDefaultSerializeIsMemberMap
            {
                get
                {
                    return (Attribute.LogTcpCallService != null && Attribute.IsLogMemberMap) || Attribute.IsDefaultSerializeIsMemberMap;
                }
            }
            /// <summary>
            /// 是否等待数据事件缓存数据初始化完成
            /// </summary>
            public bool IsEventCacheLoaded
            {
                get
                {
                    switch (CacheType)
                    {
                        case code.cSharp.sqlModel.cacheType.IdentityArray:
                        case code.cSharp.sqlModel.cacheType.IdentityTree:
                        case code.cSharp.sqlModel.cacheType.PrimaryKey:
                            return true;
                    }
                    return IsCreateEventCache;
                }
            }
            /// <summary>
            /// 是否等待创建数据事件缓存数据初始化完成
            /// </summary>
            public bool IsCreateEventCache
            {
                get
                {
                    switch (CacheType)
                    {
                        case code.cSharp.sqlModel.cacheType.CreateIdentityArray:
                        case code.cSharp.sqlModel.cacheType.CreateIdentityTree:
                        case code.cSharp.sqlModel.cacheType.CreatePrimaryKey:
                        case code.cSharp.sqlModel.cacheType.CreateMemberKey:
                            return true;
                    }
                    return false;
                }
            }
            /// <summary>
            /// WEB Path 类型
            /// </summary>
            private struct webPathType
            {
                /// <summary>
                /// WEB Path 类型
                /// </summary>
                public memberType MemberType;
                /// <summary>
                /// WEB Path 配置
                /// </summary>
                public fastCSharp.code.cSharp.webPath Attribute;
                /// <summary>
                /// 服务端生成属性名称
                /// </summary>
                public string PathMemberName
                {
                    get { return Attribute.MemberName; }
                }
                /// <summary>
                /// WEB Path 关联成员集合
                /// </summary>
                public memberInfo[] Members;
                /// <summary>
                /// 
                /// </summary>
                /// <param name="members"></param>
                /// <returns></returns>
                public memberInfo[] CheckMembers(sqlStreamMember[] members)
                {
                    int count = 0;
                    foreach (sqlStreamMember member in members)
                    {
                        foreach (memberInfo field in Members)
                        {
                            if (field.Type == member.Member.Type && field.MemberName == member.Member.MemberName)
                            {
                                ++count;
                                break;
                            }
                        }
                    }
                    if (count == Members.Length) return Members;
                    if (count == 0) return null;
                    memberInfo[] fields = new memberInfo[count];
                    foreach (sqlStreamMember member in members)
                    {
                        foreach (memberInfo field in Members)
                        {
                            if (field.Type == member.Member.Type && field.MemberName == member.Member.MemberName)
                            {
                                int memberIndex = fields.Length - count;
                                fields[--count] = new memberInfo((FieldInfo)field.Member, memberIndex);
                                break;
                            }
                        }
                    }
                    return fields;
                }
            }
            /// <summary>
            /// WEB Path 类型集合
            /// </summary>
            private subArray<webPathType> WebPaths;
            /// <summary>
            /// WEB Path 类型集合
            /// </summary>
            private Dictionary<Type, list<webPathType>> webPathTypes;
            /// <summary>
            /// 安装下一个类型
            /// </summary>
            protected override void nextCreate()
            {
                sqlStreamMember[] members = code.memberInfo.GetMembers(type, Attribute.MemberFilter)
                        .getArray(value => new sqlStreamMember { Member = value, Attribute = value.GetAttribute<fastCSharp.emit.dataMember>(Attribute.IsBaseTypeAttribute, Attribute.IsInheritAttribute) });
                WebPaths.Empty();
                list<webPathType> types;
                if (webPathTypes == null)
                {
                    webPathTypes = new Dictionary<Type, list<webPathType>>();
                    foreach (Type type in AutoParameter.Types)
                    {
                        fastCSharp.code.cSharp.webPath webPath = type.customAttribute<fastCSharp.code.cSharp.webPath>(false);
                        if (webPath != null && webPath.Type != null && webPath.MemberName != null)
                        {
                            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
                            if (fields.Length != 0)
                            {
                                if (!webPathTypes.TryGetValue(webPath.Type, out types)) webPathTypes.Add(webPath.Type, types = new list<webPathType>());
                                int memberIndex = fields.Length;
                                types.Add(new webPathType { MemberType = type, Attribute = webPath, Members = fields.getArray(value => new memberInfo(value, --memberIndex)) });
                            }
                        }
                    }
                }
                if (webPathTypes.TryGetValue(type, out types))
                {
                    foreach (webPathType webPath in types)
                    {
                        memberInfo[] fields = webPath.CheckMembers(members);
                        if (fields != null) WebPaths.Add(new webPathType { MemberType = webPath.MemberType, Attribute = webPath.Attribute, Members = fields });
                    }
                }
                CounterMembers = members.getFindArray(value => value.Attribute != null && value.Attribute.CounterTimeout > 0 && value.Member.Type == typeof(int));
                if (Attribute.LogTcpCallService == null) SqlStreamMembers = null;
                else
                {
                    SqlStreamMembers = members.getFindArray(value => value.Attribute != null && (value.Attribute.IsSqlStream || value.Attribute.SqlStreamCountType != null));
                    if (!Attribute.IsDefaultSerialize && Attribute.IsLogMemberMap)
                    {
                        fastCSharp.emit.dataSerialize dataSerialize = type.Type.customAttribute<fastCSharp.emit.dataSerialize>(true);
                        if (dataSerialize != null && !dataSerialize.IsMemberMap) error.Message("数据库日志流处理类型 " + type.FullName + " 序列化不支持成员位图");
                    }
                }
                SqlStreamTypeCount = SqlStreamMembers.count(value => value.Attribute.SqlStreamCountType != null);
                subArray<sqlStreamMember> strings = members.getFind(value => value.Member.Type == typeof(string) && (value.Attribute == null || (value.Attribute.MaxStringLength == 0 && !value.Attribute.IsIgnoreMaxStringLength)));
                if (strings.Count != 0)
                {
                    error.Message(type.FullName + " 字符串字段缺少最大长度限制 " + strings.joinString(',', value => value.Member.MemberName));
                }
                IndexMembers = members.getFind(value => value.Attribute != null && value.Attribute.IsMemberIndex).getArray(value => value.Member);
                NowTimeMembers = members.getFind(value => value.Member.MemberType.IsDateTime && value.Attribute != null && value.Attribute.IsNowTime).getArray(value => new nowTimeMember { Member = value.Member });
                base.nextCreate();
            }
        }
    }
}