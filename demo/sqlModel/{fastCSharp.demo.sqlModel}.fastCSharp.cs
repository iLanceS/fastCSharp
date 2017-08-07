//本文件由程序自动生成,请不要自行修改
using System;
using fastCSharp;

#if NotFastCSharpCode
#else
#pragma warning disable
namespace fastCSharp.demo.sqlModel
{
        [fastCSharp.emit.jsonSerialize(IsAllMember = true)]
        [fastCSharp.emit.jsonParse(IsAllMember = true)]
        [fastCSharp.emit.dataSerialize(IsReferenceMember = false)]
        public partial class Class
        {

            /// <summary>
            /// 数据库表格模型
            /// </summary>
            /// <typeparam name="tableType">表格映射类型</typeparam>
            /// <typeparam name="memberCacheType">成员绑定缓存类型</typeparam>
            public abstract class sqlModel<tableType, memberCacheType> : fastCSharp.demo.sqlModel.Class
                where tableType : sqlModel<tableType, memberCacheType>
                where memberCacheType : class
            {
                /// <summary>
                /// SQL表格操作工具
                /// </summary>
                protected static readonly fastCSharp.emit.sqlTable<tableType, fastCSharp.demo.sqlModel.Class> sqlTable = fastCSharp.emit.sqlTable<tableType, fastCSharp.demo.sqlModel.Class>.Get();
                private static bool isSqlLoaded;
                /// <summary>
                /// 等待数据初始化完成
                /// </summary>
                public static void WaitSqlLoaded()
                {
                    if (!isSqlLoaded)
                    {
                        sqlTable/**/.LoadWait.Wait();
                        isSqlLoaded = true;
                    }
                }
                private static bool isEventCacheLoaded;
                private static readonly fastCSharp.threading.waitHandle eventCacheLoadWait = new fastCSharp.threading.waitHandle(false);
                /// <summary>
                /// 等待数据事件缓存数据初始化完成
                /// </summary>
                public static void WaitEventCacheLoaded()
                {
                    if (!isEventCacheLoaded)
                    {
                        eventCacheLoadWait.Wait();
                        isEventCacheLoaded = true;
                    }
                }
                /// <summary>
                /// 数据加载完成
                /// </summary>
                /// <param name="onInserted">添加记录事件</param>
                /// <param name="onUpdated">更新记录事件</param>
                /// <param name="onDeleted">删除记录事件</param>
                #region Attribute.LogTcpCallService
                /// <param name="isMemberMap">是否支持成员位图</param>
                #endregion Attribute.LogTcpCallService
                protected static void sqlLoaded(Action<tableType> onInserted = null, Action<tableType, tableType, tableType, fastCSharp.code.memberMap<fastCSharp.demo.sqlModel.Class>> onUpdated = null, Action<tableType> onDeleted = null, bool isMemberMap = true)
                {
                    sqlStream = sqlCache/**/.GetLogStream(sqlStreamCount, isMemberMap);
                    sqlCache/**/.Loaded(onInserted, onUpdated, onDeleted, false);
                    fastCSharp.emit.sqlTable.sqlStreamCountLoaderType.Add(typeof(fastCSharp.demo.sqlModel.Class), sqlTable/**/.TableNumber, sqlStreamLoad._LoadCount_, new fastCSharp.emit.sqlTable.sqlStreamCountLoaderType(typeof(fastCSharp.demo.sqlModel.Student), 0));
                }

                /// <summary>
                /// SQL默认缓存
                /// </summary>
                protected static fastCSharp.sql.cache.whole.events.identityArray<tableType, fastCSharp.demo.sqlModel.Class, memberCacheType> sqlCache;
                /// <summary>
                /// 创建SQL默认缓存
                /// </summary>
                /// <typeparam name="memberCacheType"></typeparam>
                /// <param name="memberCache">成员缓存</param>
                /// <param name="group">数据分组</param>
                /// <param name="baseIdentity">基础ID</param>
                /// <param name="isReset">是否初始化事件与数据</param>
                /// <returns></returns>
                protected static fastCSharp.sql.cache.whole.events.identityArray<tableType, fastCSharp.demo.sqlModel.Class, memberCacheType> createCache(System.Linq.Expressions.Expression<Func<tableType, memberCacheType>> memberCache, int group = 0, int baseIdentity = 0, bool isReset = true)
                {
                    if (sqlTable == null) return null;
                    sqlCache = new fastCSharp.sql.cache.whole.events.identityArray<tableType, fastCSharp.demo.sqlModel.Class, memberCacheType>(sqlTable, memberCache, group, baseIdentity, isReset);
                    eventCacheLoadWait.Set();
                    return sqlCache;
                }



                /// <summary>
                /// 日志
                /// </summary>
                protected static fastCSharp.sql.logStream<tableType, fastCSharp.demo.sqlModel.Class> sqlStream;
                /// <summary>
                /// 获取日志流缓存数据
                /// </summary>
                /// <returns>日志流缓存数据</returns>
                [fastCSharp.code.cSharp.tcpMethod(IsClientCallbackTask = false, IsClientAsynchronous = true, IsClientSynchronous = false, Service = "DataLog")]
                protected static fastCSharp.sql.logStream<tableType, fastCSharp.demo.sqlModel.Class>.cacheIdentity getSqlCache()
                {
                    return sqlStream == null ? new fastCSharp.sql.logStream<tableType, fastCSharp.demo.sqlModel.Class>.cacheIdentity() : sqlStream/**/.Cache;
                }
                /// <summary>
                /// 日志处理
                /// </summary>
                /// <param name="ticks">时钟周期标识</param>
                /// <param name="identity">日志编号</param>
                /// <param name="onLog"></param>
                [fastCSharp.code.cSharp.tcpMethod(IsClientCallbackTask = false, IsClientAsynchronous = true, IsClientSynchronous = false, IsKeepCallback = true, Service = "DataLog")]
                protected static void onSqlLog(long ticks, int identity, Func<fastCSharp.net.returnValue<fastCSharp.sql.logStream<tableType, fastCSharp.demo.sqlModel.Class>.data>, bool> onLog)
                {
                    sqlStream/**/.OnLog(ticks, identity, onLog);
                }
                /// <summary>
                /// 获取数据
                /// </summary>
                /// <param name="Id">班级标识（默认自增）</param>
                /// <returns></returns>
                [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false, Service = "DataLog")]
                protected static tableType getSqlCache(int Id)
                {
                    return sqlCache[Id];
                }
                /// <summary>
                /// 成员加载计数器
                /// </summary>
                protected static readonly fastCSharp.sql.logStream.memberCount sqlStreamCount = new fastCSharp.sql.logStream.memberCount(-1, 4);
                /// <summary>
                /// 计算字段日志流
                /// </summary>
                public struct sqlStreamLoad
                {
                    /// <summary>
                    /// 数据对象
                    /// </summary>
                    internal sqlModel<tableType, memberCacheType> _value_;
                    private static readonly fastCSharp.code.memberMap<fastCSharp.demo.sqlModel.Class> _m4 = fastCSharp.sql.logStream.CreateMemberMap(sqlTable, value => value.StudentCount);
                    /// <summary>
                    /// 当前学生数量 (更新日志流)
                    /// </summary>
                    /// <param name="value"></param>
                    public void StudentCount(int value)
                    {
                        if (!value.Equals(_value_.StudentCount))
                        {
                            _value_.StudentCount = value;
                            StudentCount();
                        }
                    }
                    /// <summary>
                    /// 当前学生数量 (更新日志流)
                    /// </summary>
                    public void StudentCount()
                    {
                        if (sqlStream != null) sqlStream/**/.Update((tableType)_value_, _m4);
                    }
                    /// <summary>
                    /// 根据日志流计数完成类型初始化完毕
                    /// </summary>
                    /// <param name="type"></param>
                    internal static void _LoadCount_(fastCSharp.emit.sqlTable.sqlStreamCountLoaderType type)
                    {
                        if (type.Equals(typeof(fastCSharp.demo.sqlModel.Student), 0)) sqlStreamCount/**/.Load(4);
                    }
                }
                /// <summary>
                /// 计算字段日志流
                /// </summary>
                [fastCSharp.code.ignore]
                public sqlStreamLoad SqlStreamLoad
                {
                    get { return new sqlStreamLoad { _value_ = this }; }
                }
                /// <summary>
                /// 班级 URL
                /// </summary>
                [fastCSharp.code.ignore]
                public fastCSharp.demo.sqlModel.path.Class Path
                {
                    get { return new fastCSharp.demo.sqlModel.path.Class { Id = Id }; }
                }
            }
        }
}namespace fastCSharp.demo.sqlModel
{
        [fastCSharp.emit.jsonSerialize(IsAllMember = true)]
        [fastCSharp.emit.jsonParse(IsAllMember = true)]
        [fastCSharp.emit.dataSerialize(IsReferenceMember = false)]
        public partial class Student
        {

            /// <summary>
            /// 数据库表格模型
            /// </summary>
            /// <typeparam name="tableType">表格映射类型</typeparam>
            public abstract class sqlModel<tableType> : fastCSharp.demo.sqlModel.Student
                where tableType : sqlModel<tableType>
            {
                /// <summary>
                /// SQL表格操作工具
                /// </summary>
                protected static readonly fastCSharp.emit.sqlTable<tableType, fastCSharp.demo.sqlModel.Student, string> sqlTable = fastCSharp.emit.sqlTable<tableType, fastCSharp.demo.sqlModel.Student, string>.Get();
                private static bool isSqlLoaded;
                /// <summary>
                /// 等待数据初始化完成
                /// </summary>
                public static void WaitSqlLoaded()
                {
                    if (!isSqlLoaded)
                    {
                        sqlTable/**/.LoadWait.Wait();
                        isSqlLoaded = true;
                    }
                }
                private static bool isEventCacheLoaded;
                /// <summary>
                /// 等待数据事件缓存数据初始化完成
                /// </summary>
                public static void WaitEventCacheLoaded()
                {
                    if (!isEventCacheLoaded)
                    {
                        if (sqlCache == null) fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.Null);
                        isEventCacheLoaded = true;
                    }
                }
                /// <summary>
                /// 数据加载完成
                /// </summary>
                /// <param name="onInserted">添加记录事件</param>
                /// <param name="onUpdated">更新记录事件</param>
                /// <param name="onDeleted">删除记录事件</param>
                #region Attribute.LogTcpCallService
                /// <param name="isMemberMap">是否支持成员位图</param>
                #endregion Attribute.LogTcpCallService
                protected static void sqlLoaded(Action<tableType> onInserted = null, Action<tableType, tableType, tableType, fastCSharp.code.memberMap<fastCSharp.demo.sqlModel.Student>> onUpdated = null, Action<tableType> onDeleted = null, bool isMemberMap = true)
                {
                    sqlStream = sqlCache/**/.GetLogStream(null, isMemberMap);
                    sqlCache/**/.Loaded(onInserted, onUpdated, onDeleted);
                }
                /// <summary>
                /// SQL默认缓存
                /// </summary>
                protected static readonly fastCSharp.sql.cache.whole.events.identityArray<tableType, fastCSharp.demo.sqlModel.Student, tableType> sqlCache = sqlTable == null ? null : new fastCSharp.sql.cache.whole.events.identityArray<tableType, fastCSharp.demo.sqlModel.Student, tableType>(sqlTable, null);




                /// <summary>
                /// 成员索引定义
                /// </summary>
                protected static class memberIndexs
                {
                    /// <summary>
                    /// 按加入时间排序的班级集合（不可识别的字段映射为 JSON 字符串） (成员索引)
                    /// </summary>
                    public static readonly fastCSharp.code.memberMap.memberIndex Classes = fastCSharp.code.memberMap<fastCSharp.demo.sqlModel.Student>.CreateMemberIndex(value => value.Classes);
                }
                /// <summary>
                /// 日志
                /// </summary>
                protected static fastCSharp.sql.logStream<tableType, fastCSharp.demo.sqlModel.Student> sqlStream;
                /// <summary>
                /// 获取日志流缓存数据
                /// </summary>
                /// <returns>日志流缓存数据</returns>
                [fastCSharp.code.cSharp.tcpMethod(IsClientCallbackTask = false, IsClientAsynchronous = true, IsClientSynchronous = false, Service = "DataLog")]
                protected static fastCSharp.sql.logStream<tableType, fastCSharp.demo.sqlModel.Student>.cacheIdentity getSqlCache()
                {
                    return sqlStream == null ? new fastCSharp.sql.logStream<tableType, fastCSharp.demo.sqlModel.Student>.cacheIdentity() : sqlStream/**/.Cache;
                }
                /// <summary>
                /// 日志处理
                /// </summary>
                /// <param name="ticks">时钟周期标识</param>
                /// <param name="identity">日志编号</param>
                /// <param name="onLog"></param>
                [fastCSharp.code.cSharp.tcpMethod(IsClientCallbackTask = false, IsClientAsynchronous = true, IsClientSynchronous = false, IsKeepCallback = true, Service = "DataLog")]
                protected static void onSqlLog(long ticks, int identity, Func<fastCSharp.net.returnValue<fastCSharp.sql.logStream<tableType, fastCSharp.demo.sqlModel.Student>.data>, bool> onLog)
                {
                    sqlStream/**/.OnLog(ticks, identity, onLog);
                }
                /// <summary>
                /// 获取数据
                /// </summary>
                /// <param name="Id">学生标识（默认自增）</param>
                /// <returns></returns>
                [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false, Service = "DataLog")]
                protected static tableType getSqlCache(int Id)
                {
                    return sqlCache[Id];
                }
                /// <summary>
                /// 学生 URL
                /// </summary>
                [fastCSharp.code.ignore]
                public fastCSharp.demo.sqlModel.path.Student Path
                {
                    get { return new fastCSharp.demo.sqlModel.path.Student { Id = Id }; }
                }
            }
        }
}
#endif