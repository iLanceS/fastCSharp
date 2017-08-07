using System;
#pragma warning disable 649

namespace fastCSharp.code.cSharp.template
{
    class sqlModel : pub
    {
        #region NOTE
        private const int SqlStreamCountTypeNumber = 0;
        private const int CounterTimeout = 0;
        #endregion NOTE

        #region PART CLASS
        #region IF Attribute.IsDefaultSerialize
        [fastCSharp.emit.jsonSerialize(IsAllMember = true)]
        [fastCSharp.emit.jsonParse(IsAllMember = true)]
        [fastCSharp.emit.dataSerialize(IsReferenceMember = false/*NOT:IsDefaultSerializeIsMemberMap*/, IsMemberMap = false/*NOT:IsDefaultSerializeIsMemberMap*/)]
        #endregion IF Attribute.IsDefaultSerialize
        /*NOTE*/
        public partial class /*NOTE*/@TypeNameDefinition
        {
            #region IF IsManyPrimaryKey
            /// <summary>
            /// 关键字
            /// </summary>
            [fastCSharp.code.ignore]
            public struct primaryKey : IEquatable<primaryKey>/*IF:Attribute.IsComparable*/, IComparable<primaryKey>/*IF:Attribute.IsComparable*/
            {
                #region LOOP PrimaryKeys
                #region IF XmlDocument
                /// <summary>
                /// @XmlDocument
                /// </summary>
                #endregion IF XmlDocument
                public @MemberType.FullName @MemberName;
                #endregion LOOP PrimaryKeys
                /// <summary>
                /// 关键字比较
                /// </summary>
                /// <param name="other">关键字</param>
                /// <returns>是否相等</returns>
                public bool Equals(primaryKey other)
                {
                    #region NOTE
                    MemberName = null;
                    #endregion NOTE
                    return /*PUSH:PrimaryKey0*/@MemberName/**/.Equals(other.@MemberName)/*PUSH:PrimaryKey0*//*LOOP:NextPrimaryKeys*/ && @MemberName/**/.Equals(other.@MemberName)/*LOOP:NextPrimaryKeys*/;
                }
                /// <summary>
                /// 哈希编码
                /// </summary>
                /// <returns></returns>
                public override int GetHashCode()
                {
                    return /*PUSH:PrimaryKey0*/@MemberName/*PUSH:PrimaryKey0*/.GetHashCode()/*LOOP:NextPrimaryKeys*/ ^ @MemberName/**/.GetHashCode()/*LOOP:NextPrimaryKeys*/;
                }
                /// <summary>
                /// 关键字比较
                /// </summary>
                /// <param name="obj"></param>
                /// <returns></returns>
                public override bool Equals(object obj)
                {
                    return Equals((primaryKey)obj);
                }
                #region IF Attribute.IsComparable
                /// <summary>
                /// 关键字比较
                /// </summary>
                /// <param name="other"></param>
                /// <returns></returns>
                public int CompareTo(primaryKey other)
                {
                    int _value_ = /*PUSH:PrimaryKey0*/@MemberName/**/.CompareTo(other.@MemberName)/*PUSH:PrimaryKey0*/;
                    #region LOOP NextPrimaryKeys
                    if (_value_ == 0)
                    {
                        _value_ = @MemberName/**/.CompareTo(other.@MemberName);
                    #endregion LOOP NextPrimaryKeys
                        #region LOOP NextPrimaryKeys
                    }
                        #endregion LOOP NextPrimaryKeys
                    return _value_;
                }
                #endregion IF Attribute.IsComparable
            }
            #endregion IF IsManyPrimaryKey

            /// <summary>
            /// 数据库表格模型
            /// </summary>
            /// <typeparam name="tableType">表格映射类型</typeparam>
            #region IF IsMemberCache
            /// <typeparam name="memberCacheType">成员绑定缓存类型</typeparam>
            #endregion IF IsMemberCache
            public abstract class sqlModel<tableType/*IF:IsMemberCache*/, memberCacheType/*IF:IsMemberCache*/> : @type.FullName
                where tableType : sqlModel<tableType/*IF:IsMemberCache*/, memberCacheType/*IF:IsMemberCache*/>
                #region IF IsMemberCache
                where memberCacheType : /*AT:MemberCacheBaseType*//*IF:CacheCounterType=CreateIdentityCounterMemberQueue*/fastCSharp.sql.cache.whole.memberCacheCounter<tableType, memberCacheType>/*IF:CacheCounterType=CreateIdentityCounterMemberQueue*/
                #endregion IF IsMemberCache
            {
                /// <summary>
                /// SQL表格操作工具
                /// </summary>
                protected static readonly fastCSharp.emit.sqlTable<tableType, @type.FullName/*IF:PrimaryKeys.Count*/, @PrimaryKeyType/*IF:PrimaryKeys.Count*/> @SqlTableName = fastCSharp.emit.sqlTable<tableType, @type.FullName/*IF:PrimaryKeys.Count*/, @PrimaryKeyType/*IF:PrimaryKeys.Count*/>.Get();
                private static bool isSqlLoaded;
                /// <summary>
                /// 等待数据初始化完成
                /// </summary>
                public static void WaitSqlLoaded()
                {
                    if (!isSqlLoaded)
                    {
                        @SqlTableName/**/.LoadWait.Wait();
                        isSqlLoaded = true;
                    }
                }
                #region IF IsEventCacheLoaded
                private static bool isEventCacheLoaded;
                #region IF IsCreateEventCache
                private static readonly fastCSharp.threading.waitHandle eventCacheLoadWait = new fastCSharp.threading.waitHandle(false);
                #endregion IF IsCreateEventCache
                /// <summary>
                /// 等待数据事件缓存数据初始化完成
                /// </summary>
                public static void WaitEventCacheLoaded()
                {
                    if (!isEventCacheLoaded)
                    {
                        #region IF IsCreateEventCache
                        eventCacheLoadWait.Wait();
                        #endregion IF IsCreateEventCache
                        #region NOT IsCreateEventCache
                        if (@IdentityArrayCacheName == null) fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.Null);
                        #endregion NOT IsCreateEventCache
                        isEventCacheLoaded = true;
                    }
                }
                #endregion IF IsEventCacheLoaded
                #region IF IsSqlLoaded
                /// <summary>
                /// 数据加载完成
                /// </summary>
                #region IF IsSqlCacheLoaded
                /// <param name="onInserted">添加记录事件</param>
                /// <param name="onUpdated">更新记录事件</param>
                /// <param name="onDeleted">删除记录事件</param>
                #region Attribute.LogTcpCallService
                /// <param name="isMemberMap">是否支持成员位图</param>
                #endregion Attribute.LogTcpCallService
                #endregion IF IsSqlCacheLoaded
                protected static void sqlLoaded(/*IF:IsSqlCacheLoaded*/Action<tableType> onInserted = null, Action</*IF:Attribute.IsLoadedCache*/tableType, /*IF:Attribute.IsLoadedCache*/tableType, tableType, fastCSharp.code.memberMap<@type.FullName>> onUpdated = null, Action<tableType> onDeleted = null/*IF:Attribute.LogTcpCallService*/, bool isMemberMap = true/*IF:Attribute.LogTcpCallService*//*IF:IsSqlCacheLoaded*/)
                {
                    #region IF IsSqlCacheLoaded
                    #region IF Attribute.LogTcpCallService
                    #region IF SqlStreamMembers.Length
                    @SqlStreamName = @IdentityArrayCacheName/**/.GetLogStream(@SqlStreamCountName, isMemberMap);
                    #endregion IF SqlStreamMembers.Length
                    #region NOT SqlStreamMembers.Length
                    @SqlStreamName = @IdentityArrayCacheName/**/.GetLogStream(null, isMemberMap);
                    #endregion NOT SqlStreamMembers.Length
                    #endregion IF Attribute.LogTcpCallService
                    #region IF Attribute.IsLoadedCache
                    @IdentityArrayCacheName/**/.Loaded(onInserted, onUpdated, onDeleted/*IF:SqlStreamTypeCount*/, false/*IF:SqlStreamTypeCount*/);
                    #endregion IF Attribute.IsLoadedCache
                    #region NOT Attribute.IsLoadedCache
                    @SqlTableName/**/.Loaded(onInserted, /*NOTE*/(Action<tableType, tableType, fastCSharp.code.memberMap<@type.FullName>>)(object)/*NOTE*/onUpdated, onDeleted/*IF:SqlStreamTypeCount*/, false/*IF:SqlStreamTypeCount*/);
                    #endregion NOT Attribute.IsLoadedCache
                    #endregion IF IsSqlCacheLoaded
                    #region IF SqlStreamTypeCount
                    fastCSharp.emit.sqlTable.sqlStreamCountLoaderType.Add(typeof(@type.FullName), @SqlTableName/**/.TableNumber, sqlStreamLoad._LoadCount_/*LOOP:SqlStreamCountTypes*/, new fastCSharp.emit.sqlTable.sqlStreamCountLoaderType(typeof(@SqlStreamCountType.FullName), @SqlStreamCountTypeNumber)/*LOOP:SqlStreamCountTypes*/);
                    #endregion IF SqlStreamTypeCount
                }
                #endregion IF IsSqlLoaded
                #region IF CacheType=IdentityArray
                /// <summary>
                /// SQL默认缓存
                /// </summary>
                protected static readonly fastCSharp.sql.cache.whole.events.identityArray<tableType, @type.FullName, tableType> @IdentityArrayCacheName = @SqlTableName == null ? null : new fastCSharp.sql.cache.whole.events.identityArray<tableType, @type.FullName, tableType>(@SqlTableName, null);
                #endregion IF CacheType=IdentityArray
                #region IF CacheType=IdentityTree
                /// <summary>
                /// SQL默认缓存
                /// </summary>
                protected static readonly fastCSharp.sql.cache.whole.events.identityTree<tableType, @type.FullName, tableType> @IdentityTreeCacheName = @SqlTableName == null ? null : new fastCSharp.sql.cache.whole.events.identityTree<tableType, @type.FullName, tableType>(@SqlTableName, null);
                #endregion IF CacheType=IdentityTree
                #region IF CacheType=PrimaryKey
                /// <summary>
                /// SQL默认缓存
                /// </summary>
                protected static readonly fastCSharp.sql.cache.whole.events.primaryKey<tableType, @type.FullName, tableType, @PrimaryKeyType> @PrimaryKeyCacheName = @SqlTableName == null ? null : new fastCSharp.sql.cache.whole.events.primaryKey<tableType, @type.FullName, tableType, @PrimaryKeyType>(@SqlTableName, null);
                #endregion IF CacheType=PrimaryKey

                #region IF CacheType=CreateIdentityArray
                #region IF IsMemberCache
                /// <summary>
                /// SQL默认缓存
                /// </summary>
                protected static fastCSharp.sql.cache.whole.events.identityArray<tableType, @type.FullName, memberCacheType> @CreateIdentityArrayMemberCacheName;
                /// <summary>
                /// 创建SQL默认缓存
                /// </summary>
                /// <typeparam name="memberCacheType"></typeparam>
                /// <param name="memberCache">成员缓存</param>
                /// <param name="group">数据分组</param>
                /// <param name="baseIdentity">基础ID</param>
                /// <param name="isReset">是否初始化事件与数据</param>
                /// <returns></returns>
                protected static fastCSharp.sql.cache.whole.events.identityArray<tableType, @type.FullName, memberCacheType> createCache(System.Linq.Expressions.Expression<Func<tableType, memberCacheType>> memberCache, int group = 0, int baseIdentity = 0, bool isReset = true)
                {
                    if (@SqlTableName == null) return null;
                    @CreateIdentityArrayMemberCacheName = new fastCSharp.sql.cache.whole.events.identityArray<tableType, @type.FullName, memberCacheType>(@SqlTableName, memberCache, group, baseIdentity, isReset);
                    eventCacheLoadWait.Set();
                    #region IF NowTimeMembers.Length
                    loadNowTime(@CreateIdentityArrayMemberCacheName/**/.Values);
                    #endregion IF NowTimeMembers.Length
                    #region IF CounterMembers.Length
                    createCounter(@CreateIdentityArrayMemberCacheName);
                    #endregion IF CounterMembers.Length
                    return @CreateIdentityArrayMemberCacheName;
                }
                #endregion IF IsMemberCache
                #region NOT IsMemberCache
                /// <summary>
                /// SQL默认缓存
                /// </summary>
                protected static fastCSharp.sql.cache.whole.events.identityArray<tableType, @type.FullName, tableType> @CreateIdentityArrayCacheName;
                /// <summary>
                /// 创建SQL默认缓存
                /// </summary>
                /// <param name="baseIdentity">基础ID</param>
                /// <param name="group">数据分组</param>
                /// <param name="isReset">是否初始化事件与数据</param>
                /// <returns></returns>
                protected static fastCSharp.sql.cache.whole.events.identityArray<tableType, @type.FullName, tableType> createCache(int group = 0, int baseIdentity = 0, bool isReset = true)
                {
                    if (@SqlTableName == null) return null;
                    @CreateIdentityArrayCacheName = new fastCSharp.sql.cache.whole.events.identityArray<tableType, @type.FullName, tableType>(@SqlTableName, null, group, baseIdentity, isReset);
                    eventCacheLoadWait.Set();
                    #region IF NowTimeMembers.Length
                    loadNowTime(@CreateIdentityArrayCacheName/**/.Values);
                    #endregion IF NowTimeMembers.Length
                    return @CreateIdentityArrayCacheName;
                }
                #endregion NOT IsMemberCache
                #endregion IF CacheType=CreateIdentityArray
                #region IF CacheType=CreateIdentityArrayWhere
                #region IF IsMemberCache
                /// <summary>
                /// 创建SQL默认缓存
                /// </summary>
                /// <param name="memberCache">成员缓存</param>
                /// <param name="group">数据分组</param>
                /// <param name="baseIdentity">基础ID</param>
                /// <param name="isReset">是否初始化事件与数据</param>
                /// <returns></returns>
                protected static fastCSharp.sql.cache.whole.events.identityArrayWhere<tableType, @type.FullName, memberCacheType> createCache(System.Linq.Expressions.Expression<Func<tableType, memberCacheType>> memberCache, Func<tableType, bool> isValue, int group = 0, int baseIdentity = 0)
                {
                    if (@SqlTableName == null) return null;
                    return new fastCSharp.sql.cache.whole.events.identityArrayWhere<tableType, @type.FullName, memberCacheType>(@SqlTableName, memberCache, isValue, group, baseIdentity);
                }
                #endregion IF IsMemberCache
                #region NOT IsMemberCache
                /// <summary>
                /// 创建SQL默认缓存
                /// </summary>
                /// <param name="isValue">数据匹配器,必须保证更新数据的匹配一致性</param>
                /// <param name="baseIdentity">基础ID</param>
                /// <param name="group">数据分组</param>
                /// <returns></returns>
                protected static fastCSharp.sql.cache.whole.events.identityArrayWhere<tableType, @type.FullName, tableType> createCache(Func<tableType, bool> isValue, int group = 0, int baseIdentity = 0)
                {
                    if (@SqlTableName == null) return null;
                    return new fastCSharp.sql.cache.whole.events.identityArrayWhere<tableType, @type.FullName, tableType>(@SqlTableName, null, isValue, group, baseIdentity);
                }
                #endregion NOT IsMemberCache
                #endregion IF CacheType=CreateIdentityArrayWhere
                #region IF CacheType=CreateIdentityTree
                #region IF IsMemberCache
                /// <summary>
                /// SQL默认缓存
                /// </summary>
                protected static fastCSharp.sql.cache.whole.events.identityTree<tableType, @type.FullName, memberCacheType> @CreateIdentityTreeMemberCacheName;
                /// <summary>
                /// 创建SQL默认缓存
                /// </summary>
                /// <param name="memberCache">成员缓存</param>
                /// <param name="group">数据分组</param>
                /// <param name="baseIdentity">基础ID</param>
                /// <returns></returns>
                protected static fastCSharp.sql.cache.whole.events.identityTree<tableType, @type.FullName, memberCacheType> createCache(System.Linq.Expressions.Expression<Func<tableType, memberCacheType>> memberCache, int group = 0, int baseIdentity = 0)
                {
                    if (@SqlTableName == null) return null;
                    @CreateIdentityTreeMemberCacheName = new fastCSharp.sql.cache.whole.events.identityTree<tableType, @type.FullName, memberCacheType>(@SqlTableName, memberCache, group, baseIdentity);
                    eventCacheLoadWait.Set();
                    #region IF NowTimeMembers.Length
                    loadNowTime(@CreateIdentityTreeMemberCacheName/**/.Values);
                    #endregion IF NowTimeMembers.Length
                    #region IF CounterMembers.Length
                    createCounter(@CreateIdentityTreeMemberCacheName);
                    #endregion IF CounterMembers.Length
                    return @CreateIdentityTreeMemberCacheName;
                }
                #endregion IF IsMemberCache
                #region NOT IsMemberCache
                /// <summary>
                /// SQL默认缓存
                /// </summary>
                protected static fastCSharp.sql.cache.whole.events.identityTree<tableType, @type.FullName, tableType> @CreateIdentityTreeCacheName;
                /// <summary>
                /// 创建SQL默认缓存
                /// </summary>
                /// <param name="group">数据分组</param>
                /// <param name="baseIdentity">基础ID</param>
                /// <returns></returns>
                protected static fastCSharp.sql.cache.whole.events.identityTree<tableType, @type.FullName, tableType> createCache(int group = 0, int baseIdentity = 0)
                {
                    if (@SqlTableName == null) return null;
                    @CreateIdentityTreeCacheName = new fastCSharp.sql.cache.whole.events.identityTree<tableType, @type.FullName, tableType>(@SqlTableName, null, group, baseIdentity);
                    eventCacheLoadWait.Set();
                    #region IF NowTimeMembers.Length
                    loadNowTime(@CreateIdentityTreeCacheName/**/.Values);
                    #endregion IF NowTimeMembers.Length
                    return @CreateIdentityTreeCacheName;
                }
                #endregion NOT IsMemberCache
                #endregion IF CacheType=CreateIdentityTree
                #region IF CacheType=CreatePrimaryKey
                #region IF IsMemberCache
                /// <summary>
                /// SQL默认缓存
                /// </summary>
                protected static fastCSharp.sql.cache.whole.events.primaryKey<tableType, @type.FullName, memberCacheType, @PrimaryKeyType> @CreatePrimaryKeyMemberCacheName;
                /// <summary>
                /// 创建SQL默认缓存
                /// </summary>
                /// <typeparam name="memberCacheType"></typeparam>
                /// <param name="memberCache">成员缓存</param>
                /// <param name="group">数据分组</param>
                /// <returns></returns>
                protected static fastCSharp.sql.cache.whole.events.primaryKey<tableType, @type.FullName, memberCacheType, @PrimaryKeyType> createCache(System.Linq.Expressions.Expression<Func<tableType, memberCacheType>> memberCache, int group = 0)
                {
                    if (@SqlTableName == null) return null;
                    @CreatePrimaryKeyMemberCacheName = new fastCSharp.sql.cache.whole.events.primaryKey<tableType, @type.FullName, memberCacheType, @PrimaryKeyType>(@SqlTableName, memberCache, group);
                    eventCacheLoadWait.Set();
                    #region IF NowTimeMembers.Length
                    loadNowTime(@CreatePrimaryKeyMemberCacheName/**/.Values);
                    #endregion IF NowTimeMembers.Length
                    return @CreatePrimaryKeyMemberCacheName;
                }
                #endregion IF IsMemberCache
                #region NOT IsMemberCache
                /// <summary>
                /// SQL默认缓存
                /// </summary>
                protected static fastCSharp.sql.cache.whole.events.primaryKey<tableType, @type.FullName, tableType, @PrimaryKeyType> @CreatePrimaryKeyCacheName;
                /// <summary>
                /// 创建SQL默认缓存
                /// </summary>
                /// <param name="group">数据分组</param>
                /// <returns></returns>
                protected static fastCSharp.sql.cache.whole.events.primaryKey<tableType, @type.FullName, tableType, @PrimaryKeyType> createCache(int group = 0)
                {
                    if (@SqlTableName == null) return null;
                    @CreatePrimaryKeyCacheName = new fastCSharp.sql.cache.whole.events.primaryKey<tableType, @type.FullName, tableType, @PrimaryKeyType>(@SqlTableName, null, group);
                    eventCacheLoadWait.Set();
                    #region IF NowTimeMembers.Length
                    loadNowTime(@CreatePrimaryKeyCacheName/**/.Values);
                    #endregion IF NowTimeMembers.Length
                    return @CreatePrimaryKeyCacheName;
                }
                #endregion NOT IsMemberCache
                #endregion IF CacheType=CreatePrimaryKey
                #region IF CacheType=CreateMemberKey
                #region IF IsMemberCache
                /// <summary>
                /// SQL默认缓存
                /// </summary>
                protected static fastCSharp.sql.cache.whole.events.cache<tableType, @type.FullName, memberCacheType> @CreateMemberKeyMemberCacheName;
                /// <summary>
                /// 创建SQL默认缓存
                /// </summary>
                /// <typeparam name="targetType"></typeparam>
                /// <typeparam name="targetModelType"></typeparam>
                /// <typeparam name="targetMemberCacheType"></typeparam>
                /// <typeparam name="keyType"></typeparam>
                /// <typeparam name="memberKeyType"></typeparam>
                /// <param name="targetCache">目标缓存</param>
                /// <param name="memberCache">成员缓存</param>
                /// <param name="getKey">键值获取器</param>
                /// <param name="getMemberKey">成员缓存键值获取器</param>
                /// <param name="member">缓存成员</param>
                /// <param name="group">数据分组</param>
                /// <returns></returns>
                protected static fastCSharp.sql.cache.whole.events.memberKey<tableType, @type.FullName, memberCacheType, keyType, memberKeyType, targetMemberCacheType> createCache<targetType, targetModelType, targetMemberCacheType, keyType, memberKeyType>(fastCSharp.sql.cache.whole.events.key<targetType, targetModelType, targetMemberCacheType, keyType> targetCache, System.Linq.Expressions.Expression<Func<tableType, memberCacheType>> memberCache, Func<@type.FullName, keyType> getKey, Func<@type.FullName, memberKeyType> getMemberKey, System.Linq.Expressions.Expression<Func<targetMemberCacheType, keyValue<System.Collections.Generic.Dictionary<fastCSharp.sql.randomKey<memberKeyType>, tableType>, int>>> member, int group = 0)
                    where keyType : struct, IEquatable<keyType>
                    where memberKeyType : struct, IEquatable<memberKeyType>
                    where targetType : class, targetModelType
                    where targetModelType : class
                    where targetMemberCacheType : class
                {
                    if (@SqlTableName == null) return null;
                    fastCSharp.sql.cache.whole.events.memberKey<tableType, @type.FullName, memberCacheType, keyType, memberKeyType, targetMemberCacheType> cache = new fastCSharp.sql.cache.whole.events.memberKey<tableType, @type.FullName, memberCacheType, keyType, memberKeyType, targetMemberCacheType>(@SqlTableName, memberCache, getKey, getMemberKey, targetCache.GetMemberCacheByKey, member, targetCache.GetAllMemberCache, group);
                    @CreateMemberKeyMemberCacheName = cache;
                    eventCacheLoadWait.Set();
                    #region IF NowTimeMembers.Length
                    loadNowTime(cache.Values);
                    #endregion IF NowTimeMembers.Length
                    return cache;
                }
                #endregion IF IsMemberCache
                #region NOT IsMemberCache
                /// <summary>
                /// SQL默认缓存
                /// </summary>
                protected static fastCSharp.sql.cache.whole.events.cache<tableType, @type.FullName, tableType> @CreateMemberKeyCacheName;
                /// <summary>
                /// 创建SQL默认缓存
                /// </summary>
                /// <typeparam name="targetType"></typeparam>
                /// <typeparam name="targetModelType"></typeparam>
                /// <typeparam name="targetMemberCacheType"></typeparam>
                /// <typeparam name="keyType"></typeparam>
                /// <typeparam name="memberKeyType"></typeparam>
                /// <param name="targetCache">目标缓存</param>
                /// <param name="memberCache">成员缓存</param>
                /// <param name="getKey">键值获取器</param>
                /// <param name="getMemberKey">成员缓存键值获取器</param>
                /// <param name="member">缓存成员</param>
                /// <param name="group">数据分组</param>
                /// <returns></returns>
                protected static fastCSharp.sql.cache.whole.events.memberKey<tableType, @type.FullName, tableType, keyType, memberKeyType, targetMemberCacheType> createCache<targetType, targetModelType, targetMemberCacheType, keyType, memberKeyType>(fastCSharp.sql.cache.whole.events.key<targetType, targetModelType, targetMemberCacheType, keyType> targetCache, Func<@type.FullName, keyType> getKey, Func<@type.FullName, memberKeyType> getMemberKey, System.Linq.Expressions.Expression<Func<targetMemberCacheType, keyValue<System.Collections.Generic.Dictionary<fastCSharp.sql.randomKey<memberKeyType>, tableType>, int>>> member, int group = 0)
                    where keyType : struct, IEquatable<keyType>
                    where memberKeyType : struct, IEquatable<memberKeyType>
                    where targetType : class, targetModelType
                    where targetModelType : class
                    where targetMemberCacheType : class
                {
                    if (@SqlTableName == null) return null;
                    fastCSharp.sql.cache.whole.events.memberKey<tableType, @type.FullName, tableType, keyType, memberKeyType, targetMemberCacheType> cache = new fastCSharp.sql.cache.whole.events.memberKey<tableType, @type.FullName, tableType, keyType, memberKeyType, targetMemberCacheType>(@SqlTableName, null, getKey, getMemberKey, targetCache.GetMemberCacheByKey, member, targetCache.GetAllMemberCache, group);
                    @CreateMemberKeyCacheName = cache;
                    eventCacheLoadWait.Set();
                    #region IF NowTimeMembers.Length
                    loadNowTime(cache.Values);
                    #endregion IF NowTimeMembers.Length
                    return cache;
                }
                #endregion NOT IsMemberCache
                #endregion IF CacheType=CreateMemberKey

                #region IF CacheCounterType=IdentityCounter
                /// <summary>
                /// SQL默认计数缓存
                /// </summary>
                #region IF IsIdentity64
                protected static readonly fastCSharp.sql.cache.part.events.identityCounter<tableType, @type.FullName> @IdentityCounterCacheName = @SqlTableName == null ? null : new fastCSharp.sql.cache.part.events.identityCounter<tableType, @type.FullName>(@SqlTableName, 1);
                #endregion IF IsIdentity64
                #region NOT IsIdentity64
                protected static readonly fastCSharp.sql.cache.part.events.identityCounter32<tableType, @type.FullName> @IdentityCounter32CacheName = @SqlTableName == null ? null : new fastCSharp.sql.cache.part.events.identityCounter32<tableType, @type.FullName>(@SqlTableName, 1);
                #endregion NOT IsIdentity64
                #endregion IF CacheCounterType=IdentityCounter
                #region IF CacheCounterType=PrimaryKeyCounter
                /// <summary>
                /// SQL默认计数缓存
                /// </summary>
                protected static readonly fastCSharp.sql.cache.part.events.primaryKeyCounter<tableType, @type.FullName, @PrimaryKeyType> @PrimaryKeyCounterCacheName = @SqlTableName == null ? null : new fastCSharp.sql.cache.part.events.primaryKeyCounter<tableType, @type.FullName, @PrimaryKeyType>(@SqlTableName, 1);
                #endregion IF CacheCounterType=PrimaryKeyCounter

                #region IF CacheCounterType=CreateIdentityCounterMemberQueue
                /// <summary>
                /// SQL默认计数缓存
                /// </summary>
                protected static fastCSharp.sql.cache.part.events.memberIdentityCounter32<tableType, @type.FullName, memberCacheType> @CreateIdentityCounterMemberQueueCacheName;
                /// <summary>
                /// 创建SQL默认先进先出优先队列缓存
                /// </summary>
                /// <param name="cache">自增ID整表缓存</param>
                /// <param name="group">数据分组</param>
                /// <param name="maxCount">缓存默认最大容器大小</param>
                /// <returns></returns>
                protected static fastCSharp.sql.cache.part.memberQueue<tableType, @type.FullName, memberCacheType, int> createCounterMemberQueue(fastCSharp.sql.cache.whole.events.identityMemberMap<tableType, @type.FullName, memberCacheType> cache, int group = 1, int maxCount = 0)
                {
                    @CreateIdentityCounterMemberQueueCacheName = cache.CreateCounter(value => value.Counter, group);
                    return @CreateIdentityCounterMemberQueueCacheName/**/.CreateMemberQueue(value => value.NodeValue, value => value.PreviousNode, value => value.NextNode, maxCount);
                }
                #endregion IF CacheCounterType=CreateIdentityCounterMemberQueue
                #region IF CacheCounterType=CreateIdentityCounterQueue
                /// <summary>
                /// SQL默认计数缓存
                /// </summary>
                #region IF IsIdentity64
                protected static fastCSharp.sql.cache.part.events.identityCounter<tableType, @type.FullName> @CreateIdentityCounterQueueCacheName;
                /// <summary>
                /// 创建SQL默认先进先出优先队列缓存
                /// </summary>
                /// <param name="group">数据分组</param>
                /// <param name="maxCount">缓存默认最大容器大小</param>
                /// <returns>数据分组</returns>
                protected static fastCSharp.sql.cache.part.queue<tableType, @type.FullName, long> createIdentityCounterQueue(int group = 1, int maxCount = 0)
                {
                    return new fastCSharp.sql.cache.part.queue<tableType, @type.FullName, long>(@CreateIdentityCounterQueueCacheName = new fastCSharp.sql.cache.part.events.identityCounter<tableType, @type.FullName>(@SqlTableName, group), @SqlTableName/**/.GetByIdentity, maxCount);
                }
                #endregion IF IsIdentity64
                #region NOT IsIdentity64
                protected static fastCSharp.sql.cache.part.events.identityCounter32<tableType, @type.FullName> @CreateIdentityCounter32QueueCacheName;
                /// <summary>
                /// 创建SQL默认先进先出优先队列缓存
                /// </summary>
                /// <param name="group">数据分组</param>
                /// <param name="maxCount">缓存默认最大容器大小</param>
                /// <returns>数据分组</returns>
                protected static fastCSharp.sql.cache.part.queue<tableType, @type.FullName, int> @CreateIdentityCounterQueueMethodName(int group = 1, int maxCount = 0)
                {
                    return new fastCSharp.sql.cache.part.queue<tableType, @type.FullName, int>(@CreateIdentityCounter32QueueCacheName = new fastCSharp.sql.cache.part.events.identityCounter32<tableType, @type.FullName>(@SqlTableName, group), @SqlTableName/**/.GetByIdentity, maxCount);
                }
                #endregion NOT IsIdentity64
                #endregion IF CacheCounterType=CreateIdentityCounterQueue
                #region IF CacheCounterType=CreateIdentityCounterQueueList
                /// <summary>
                /// SQL默认计数缓存
                /// </summary>
                #region IF IsIdentity64
                protected static fastCSharp.sql.cache.part.events.identityCounter<tableType, @type.FullName> @CreateIdentityCounterQueueListCacheName;
                /// <summary>
                /// 创建SQL默认先进先出优先队列缓存
                /// </summary>
                /// <param name="group">数据分组</param>
                /// <param name="maxCount">缓存默认最大容器大小</param>
                /// <returns>数据分组</returns>
                protected static fastCSharp.sql.cache.part.queueList<tableType, @type.FullName, long, keyType> createIdentityCounterQueueList<keyType>(System.Linq.Expressions.Expression<Func<@type.FullName, keyType>> getKey, Func<keyType, System.Linq.Expressions.Expression<Func<@type.FullName, bool>>> getWhere, int group = 0, int maxCount = 0)
                    where keyType : IEquatable<keyType>
                {
                    return new fastCSharp.sql.cache.part.queueList<tableType, @type.FullName, long, keyType>(@CreateIdentityCounterQueueListCacheName = new fastCSharp.sql.cache.part.events.identityCounter<tableType, @type.FullName>(@SqlTableName, group), getKey, getWhere, maxCount);
                }
                #endregion IF IsIdentity64
                #region NOT IsIdentity64
                protected static fastCSharp.sql.cache.part.events.identityCounter32<tableType, @type.FullName> @CreateIdentityCounter32QueueListCacheName;
                /// <summary>
                /// 创建SQL默认先进先出优先队列缓存
                /// </summary>
                /// <param name="group">数据分组</param>
                /// <param name="maxCount">缓存默认最大容器大小</param>
                /// <returns>数据分组</returns>
                protected static fastCSharp.sql.cache.part.queueList<tableType, @type.FullName, int, keyType> @CreateIdentityCounterQueueListMethodName<keyType>(System.Linq.Expressions.Expression<Func<@type.FullName, keyType>> getKey, Func<keyType, System.Linq.Expressions.Expression<Func<@type.FullName, bool>>> getWhere, int group = 0, int maxCount = 0)
                    where keyType : IEquatable<keyType>
                {
                    return new fastCSharp.sql.cache.part.queueList<tableType, @type.FullName, int, keyType>(@CreateIdentityCounter32QueueCacheName = new fastCSharp.sql.cache.part.events.identityCounter32<tableType, @type.FullName>(@SqlTableName, group), getKey, getWhere, maxCount);
                }
                #endregion NOT IsIdentity64
                #endregion IF CacheCounterType=CreateIdentityCounterQueueList
                #region IF CacheCounterType=CreatePrimaryKeyCounterQueue
                /// <summary>
                /// SQL默认计数缓存
                /// </summary>
                protected static fastCSharp.sql.cache.part.events.primaryKeyCounter<tableType, @type.FullName, @PrimaryKeyType> @CreatePrimaryKeyCounterQueueCacheName;
                /// <summary>
                /// 创建SQL默认先进先出优先队列缓存
                /// </summary>
                /// <param name="group">数据分组</param>
                /// <param name="maxCount">缓存默认最大容器大小</param>
                /// <returns>数据分组</returns>
                protected static fastCSharp.sql.cache.part.queue<tableType, @type.FullName, @PrimaryKeyType> createPrimaryKeyCounterQueue(int group = 1, int maxCount = 0)
                {
                    return new fastCSharp.sql.cache.part.queue<tableType, @type.FullName, @PrimaryKeyType>(@CreatePrimaryKeyCounterQueueCacheName = new fastCSharp.sql.cache.part.events.primaryKeyCounter<tableType, @type.FullName, @PrimaryKeyType>(@SqlTableName, group), @SqlTableName/**/.GetByPrimaryKey, maxCount);
                }
                #endregion IF CacheCounterType=CreatePrimaryKeyCounterQueue
                #region IF CacheCounterType=CreatePrimaryKeyCounterQueueList
                /// <summary>
                /// SQL默认计数缓存
                /// </summary>
                protected static fastCSharp.sql.cache.part.events.primaryKeyCounter<tableType, @type.FullName, @PrimaryKeyType> @CreatePrimaryKeyCounterQueueListCacheName;
                /// <summary>
                /// 创建SQL默认先进先出优先队列缓存
                /// </summary>
                /// <param name="getKey">缓存关键字获取器</param>
                /// <param name="getWhere">条件表达式获取器</param>
                /// <param name="group">数据分组</param>
                /// <param name="maxCount">缓存默认最大容器大小</param>
                /// <returns>数据分组</returns>
                protected static fastCSharp.sql.cache.part.queueList<tableType, @type.FullName, @PrimaryKeyType, keyType> createPrimaryKeyCounterQueueList<keyType>(System.Linq.Expressions.Expression<Func<@type.FullName, keyType>> getKey, Func<keyType, System.Linq.Expressions.Expression<Func<@type.FullName, bool>>> getWhere, int group = 1, int maxCount = 0)
                    where keyType : IEquatable<keyType>
                {
                    return new fastCSharp.sql.cache.part.queueList<tableType, @type.FullName, @PrimaryKeyType, keyType>(@CreatePrimaryKeyCounterQueueListCacheName = new fastCSharp.sql.cache.part.events.primaryKeyCounter<tableType, @type.FullName, @PrimaryKeyType>(@SqlTableName, group), getKey, getWhere, maxCount);
                }
                #endregion IF CacheCounterType=CreatePrimaryKeyCounterQueueList
                #region IF CacheCounterType=CreatePrimaryKeyCounterQueueDictionary
                /// <summary>
                /// SQL默认计数缓存
                /// </summary>
                protected static fastCSharp.sql.cache.part.events.primaryKeyCounter<tableType, @type.FullName, @PrimaryKeyType> @CreatePrimaryKeyCounterQueueDictionaryCacheName;
                /// <summary>
                /// 创建SQL默认先进先出优先队列缓存
                /// </summary>
                /// <param name="getKey">缓存关键字获取器</param>
                /// <param name="getWhere">条件表达式获取器</param>
                /// <param name="getDictionaryKey">缓存字典关键字获取器</param>
                /// <param name="group">数据分组</param>
                /// <param name="maxCount">缓存默认最大容器大小</param>
                /// <returns>数据分组</returns>
                protected static fastCSharp.sql.cache.part.queueDictionary<tableType, @type.FullName, @PrimaryKeyType, keyType, dictionaryKeyType> createPrimaryKeyCounterQueueDictionary<keyType, dictionaryKeyType>(System.Linq.Expressions.Expression<Func<@type.FullName, keyType>> getKey, Func<keyType, System.Linq.Expressions.Expression<Func<@type.FullName, bool>>> getWhere, Func<tableType, dictionaryKeyType> getDictionaryKey, int group = 1, int maxCount = 0)
                    where keyType : IEquatable<keyType>
                    where dictionaryKeyType : IEquatable<dictionaryKeyType>
                {
                    return new fastCSharp.sql.cache.part.queueDictionary<tableType, @type.FullName, @PrimaryKeyType, keyType, dictionaryKeyType>(@CreatePrimaryKeyCounterQueueDictionaryCacheName = new fastCSharp.sql.cache.part.events.primaryKeyCounter<tableType, @type.FullName, @PrimaryKeyType>(@SqlTableName, group), getKey, getWhere, getDictionaryKey, maxCount);
                }
                #endregion IF CacheCounterType=CreatePrimaryKeyCounterQueueDictionary

                #region IF IndexMembers.Length
                /// <summary>
                /// 成员索引定义
                /// </summary>
                protected static class memberIndexs
                {
                    #region LOOP IndexMembers
                    #region PUSH Member
                    #region IF XmlDocument
                    /// <summary>
                    /// @XmlDocument (成员索引)
                    /// </summary>
                    #endregion IF XmlDocument
                    public static readonly fastCSharp.code.memberMap.memberIndex @MemberName = fastCSharp.code.memberMap<@type.FullName>.CreateMemberIndex(value => value.@MemberName);
                    #endregion PUSH Member
                    #endregion LOOP IndexMembers
                }
                #endregion IF IndexMembers.Length
                #region IF Attribute.LogTcpCallService
                /// <summary>
                /// 日志
                /// </summary>
                protected static fastCSharp.sql.logStream<tableType, @type.FullName> @SqlStreamName;
                #region IF Attribute.IsLogClientQueue
                /// <summary>
                /// 日志处理
                /// </summary>
                /// <param name="onLog"></param>
                [fastCSharp.code.cSharp.tcpMethod(IsClientCallbackTask = false, IsClientAsynchronous = true, IsClientSynchronous = false, IsKeepCallback = true, Service = "@Attribute.LogTcpCallService")]
                protected static void onSqlLog(Func<fastCSharp.net.returnValue<fastCSharp.sql.logStream<tableType, @type.FullName>.data>, bool> onLog)
                {
                    @SqlStreamName/**/.OnLog(onLog);
                }
                #endregion IF Attribute.IsLogClientQueue
                #region NOT Attribute.IsLogClientQueue
                /// <summary>
                /// 获取日志流缓存数据
                /// </summary>
                /// <returns>日志流缓存数据</returns>
                [fastCSharp.code.cSharp.tcpMethod(IsClientCallbackTask = false, IsClientAsynchronous = true, IsClientSynchronous = false, Service = "@Attribute.LogTcpCallService")]
                protected static fastCSharp.sql.logStream<tableType, @type.FullName>.cacheIdentity getSqlCache()
                {
                    return @SqlStreamName == null ? new fastCSharp.sql.logStream<tableType, @type.FullName>.cacheIdentity() : @SqlStreamName/**/.Cache;
                }
                /// <summary>
                /// 日志处理
                /// </summary>
                /// <param name="ticks">时钟周期标识</param>
                /// <param name="identity">日志编号</param>
                /// <param name="onLog"></param>
                [fastCSharp.code.cSharp.tcpMethod(IsClientCallbackTask = false, IsClientAsynchronous = true, IsClientSynchronous = false, IsKeepCallback = true, Service = "@Attribute.LogTcpCallService")]
                protected static void onSqlLog(long ticks, int identity, Func<fastCSharp.net.returnValue<fastCSharp.sql.logStream<tableType, @type.FullName>.data>, bool> onLog)
                {
                    @SqlStreamName/**/.OnLog(ticks, identity, onLog);
                }
                #region IF Identity
                #region PUSH Identity
                /// <summary>
                /// 获取数据
                /// </summary>
                /// <param name="@MemberName">@XmlDocument</param>
                /// <returns></returns>
                [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false, Service = "@Attribute.LogTcpCallService")]
                protected static tableType getSqlCache(int @MemberName)
                {
                    return @IdentityArrayCacheName[@MemberName];
                }
                #endregion PUSH Identity
                #endregion IF Identity
                #region NOT Identity
                #region NOT CacheType=CreateMemberKey
                /// <summary>
                /// 获取数据
                /// </summary>
                /// <param name="key">关键字</param>
                /// <returns></returns>
                [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false, Service = "@Attribute.LogTcpCallService")]
                protected static tableType getSqlCache(@PrimaryKeyType key)
                {
                    return @PrimaryKeyCacheName[key];
                }
                #endregion NOT CacheType=CreateMemberKey
                #endregion NOT Identity
                #endregion NOT Attribute.IsLogClientQueue
                #region IF SqlStreamMembers.Length
                /// <summary>
                /// 成员加载计数器
                /// </summary>
                protected static readonly fastCSharp.sql.logStream.memberCount @SqlStreamCountName = new fastCSharp.sql.logStream.memberCount(-1/*LOOP:SqlStreamMembers*//*IF:IsSqlStreamCount*//*PUSH:Member*/, @MemberIndex/*PUSH:Member*//*IF:IsSqlStreamCount*//*LOOP:SqlStreamMembers*/);
                /// <summary>
                /// 计算字段日志流
                /// </summary>
                public struct sqlStreamLoad
                {
                    /// <summary>
                    /// 数据对象
                    /// </summary>
                    internal sqlModel<tableType/*IF:IsMemberCache*/, memberCacheType/*IF:IsMemberCache*/> _value_;
                    #region LOOP SqlStreamMembers
                    #region PUSH Member
                    private static readonly fastCSharp.code.memberMap<@type.FullName> @MemberMapName = fastCSharp.sql.logStream.CreateMemberMap(@SqlTableName, value => value.@MemberName);
                    #region IF XmlDocument
                    /// <summary>
                    /// @XmlDocument (更新日志流)
                    /// </summary>
                    /// <param name="value"></param>
                    #endregion IF XmlDocument
                    public void @MemberName(@MemberType.FullName value)
                    {
                        #region IF MemberType.IsIEquatable
                        if (!value.Equals(_value_.@MemberName))
                        #endregion IF MemberType.IsIEquatable
                        {
                            _value_.@MemberName = value;
                            @MemberName();
                        }
                    }
                    #region IF XmlDocument
                    /// <summary>
                    /// @XmlDocument (更新日志流)
                    /// </summary>
                    #endregion IF XmlDocument
                    public void @MemberName()
                    {
                        if (@SqlStreamName != null) @SqlStreamName/**/.Update((tableType)_value_, @MemberMapName);
                    }
                    #region IF IsSqlStreamCount
                    #region NOT SqlStreamCountType
                    /// <summary>
                    /// @XmlDocument 计算初始化完毕
                    /// </summary>
                    public static void @MemberLoadedMethodName() { @SqlStreamCountName/**/.Load(@MemberIndex); }
                    #endregion NOT SqlStreamCountType
                    #endregion IF IsSqlStreamCount
                    #endregion PUSH Member
                    #endregion LOOP SqlStreamMembers
                    #region IF SqlStreamTypeCount
                    /// <summary>
                    /// 根据日志流计数完成类型初始化完毕
                    /// </summary>
                    /// <param name="type"></param>
                    internal static void _LoadCount_(fastCSharp.emit.sqlTable.sqlStreamCountLoaderType type)
                    {
                        #region LOOP SqlStreamMembers
                        #region PUSH Member
                        #region IF SqlStreamCountType
                        if (type.Equals(typeof(@SqlStreamCountType.FullName), @SqlStreamCountTypeNumber)) @SqlStreamCountName/**/.Load(@MemberIndex);
                        #endregion IF SqlStreamCountType
                        #endregion PUSH Member
                        #endregion LOOP SqlStreamMembers
                    }
                    #endregion IF SqlStreamTypeCount
                }
                /// <summary>
                /// 计算字段日志流
                /// </summary>
                [fastCSharp.code.ignore]
                public sqlStreamLoad @SqlStreamLoadName
                {
                    get { return new sqlStreamLoad { _value_ = this }; }
                }
                #endregion IF SqlStreamMembers.Length
                #endregion IF Attribute.LogTcpCallService
                #region IF NowTimeMembers.Length
                #region LOOP NowTimeMembers
                #region IF Member.XmlDocument
                /// <summary>
                /// @Member.XmlDocument 当前时间
                /// </summary>
                #endregion IF Member.XmlDocument
                protected static readonly fastCSharp.sql.nowTime @NowTimeMemberName = @SqlTableName == null ? null : new fastCSharp.sql.nowTime();
                #endregion LOOP NowTimeMembers
                /// <summary>
                /// 初始化当前时间
                /// </summary>
                /// <param name="values">缓存数据</param>
                protected static void loadNowTime(System.Collections.Generic.IEnumerable<tableType> values)
                {
                    foreach (tableType value in values)
                    {
                        #region LOOP NowTimeMembers
                        @NowTimeMemberName/**/.SetMaxTime(/*NOTE*/(DateTime)(object)/*NOTE*//*PUSH:Member*/value.@MemberName/*PUSH:Member*/);
                        #endregion LOOP NowTimeMembers
                    }
                    #region LOOP NowTimeMembers
                    @NowTimeMemberName/**/.SetMaxTime();
                    #endregion LOOP NowTimeMembers
                }
                #region IF IsLoadNowTimeCache
                static sqlModel()
                {
                    if (@IdentityArrayCacheName != null) loadNowTime(@IdentityArrayCacheName/**/.Values);
                }
                #endregion IF IsLoadNowTimeCache
                #endregion IF NowTimeMembers.Length
                #region IF CounterMembers.Length
                /// <summary>
                /// 计数成员
                /// </summary>
                protected static class sqlCounter
                {
                    #region LOOP CounterMembers
                    #region PUSH Member
                    #region IF XmlDocument
                    /// <summary>
                    /// @XmlDocument 当前时间
                    /// </summary>
                    #endregion IF XmlDocument
                    public static fastCSharp.sql.cache.whole.events.counterMember @MemberName = fastCSharp.sql.cache.whole.events.counterMember.Null;
                    #endregion PUSH Member
                    #endregion LOOP CounterMembers
                }
                #region LOOP CounterMembers
                #region PUSH Member
                #region IF Attribute.CounterReadServiceName
                /// <summary>
                /// 获取 @XmlDocument
                /// </summary>
                #region PUSH Identity
                /// <param name="@MemberName">@XmlDocument</param>
                #endregion PUSH Identity
                /// <returns>@XmlDocument</returns>
                [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false, Service = "@Attribute.CounterReadServiceName")]
                protected static int @GetCountMethodName(/*PUSH:Identity*/@MemberType.FullName @MemberName/*PUSH:Identity*/)
                {
                    return sqlCounter.@MemberName/**/.Get(/*NOTE*/(int)(object)/*NOTE*//*PUSH:Identity*/@MemberName/*PUSH:Identity*/);
                }
                #region IF XmlDocument
                /// <summary>
                /// @XmlDocument 总计数
                /// </summary>
                #endregion IF XmlDocument
                [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false, Service = "@Attribute.CounterReadServiceName")]
                protected static long @TotalCountMemberName
                {
                    get { return sqlCounter.@MemberName/**/.TotalCount; }
                }
                #endregion IF Attribute.CounterReadServiceName
                #region IF Attribute.CounterWriteServiceName
                /// <summary>
                /// @XmlDocument 增加计数
                /// </summary>
                #region PUSH Identity
                /// <param name="@MemberName">@XmlDocument</param>
                #endregion PUSH Identity
                [fastCSharp.code.cSharp.tcpMethod(IsClientCallbackTask = false, IsClientAsynchronous = true, IsClientSynchronous = false, Service = "@Attribute.CounterWriteServiceName")]
                protected static void @IncCountMethodName(/*PUSH:Identity*/@MemberType.FullName @MemberName/*PUSH:Identity*/)
                {
                    sqlCounter.@MemberName/**/.Inc(/*NOTE*/(int)(object)/*NOTE*//*PUSH:Identity*/@MemberName/*PUSH:Identity*/);
                }
                #endregion IF Attribute.CounterWriteServiceName
                #endregion PUSH Member
                #endregion LOOP CounterMembers
                /// <summary>
                /// 初始化计数成员
                /// </summary>
                /// <param name="cache"></param>
                protected static void createCounter(fastCSharp.sql.cache.whole.events.identityCache<tableType, @type.FullName, memberCacheType> cache)
                {
                    #region LOOP CounterMembers
                    #region PUSH Member
                    sqlCounter.@MemberName = new fastCSharp.sql.cache.whole.events.counterMember<tableType, @type.FullName, memberCacheType>(cache, value => /*NOTE*/(int)(object)/*NOTE*/value.@MemberName, /*PUSH:Attribute*/@CounterTimeout/*PUSH:Attribute*/);
                    #endregion PUSH Member
                    #endregion LOOP CounterMembers
                }
                #endregion IF CounterMembers.Length
                #region LOOP WebPaths
                #region IF MemberType.XmlDocument
                /// <summary>
                /// @MemberType.XmlDocument
                /// </summary>
                #endregion IF MemberType.XmlDocument
                [fastCSharp.code.ignore]
                public @MemberType.FullName @PathMemberName
                {
                    get { return new @MemberType.FullName { /*LOOP:Members*/@MemberName = @MemberName/*IF:MemberIndex*/, /*IF:MemberIndex*//*LOOP:Members*//*NOTE*/ PropertyName = null/*NOTE*/ }; }
                }
                #endregion LOOP WebPaths
            }
        }
        #endregion PART CLASS
    }
    /// <summary>
    /// CSharp模板公用模糊类型
    /// </summary>
    internal partial class pub : System.IEquatable<pub>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(pub other) { return false; }
        /// <summary>
        /// 日志流计数完成类型
        /// </summary>
        public class SqlStreamCountType : pub
        {
        }
    }
}
