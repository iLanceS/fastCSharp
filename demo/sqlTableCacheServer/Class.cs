using System;

namespace fastCSharp.demo.sqlTableCacheServer
{
    /// <summary>
    /// 班级表格定义
    /// </summary>
    [fastCSharp.code.cSharp.tcpCall(Service = pub.DataReaderServer)]
    [fastCSharp.emit.sqlTable(ConnectionName = pub.SqlConnection, IsLoadIdentity = false)]
    public partial class Class : fastCSharp.demo.sqlModel.Class.sqlModel<Class, Class.memberCache>
    {
        /// <summary>
        /// 远程对象扩展
        /// </summary>
        public struct remote
        {
            /// <summary>
            /// 班级信息
            /// </summary>
            internal Class Value;
            /// <summary>
            /// 学生集合
            /// </summary>
            public Student[] Students
            {
                get
                {
#if NotFastCSharpCode
                    fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.NotFastCSharpCode);
                    return null;
#else
                    return clientCache.Student.Get(tcpCall.Class.GetStudentIds(Value.Id));
#endif
                }
            }
        }
        /// <summary>
        /// 远程对象扩展
        /// </summary>
        public remote Remote
        {
            get { return new remote { Value = this }; }
        }
        /// <summary>
        /// 扩展缓存数据
        /// </summary>
        public sealed class memberCache : fastCSharp.sql.cache.whole.memberCache<Class>
        {
            /// <summary>
            /// 学生列表
            /// </summary>
            internal list<Student> Students;
        }
        /// <summary>
        /// 扩展缓存数据
        /// </summary>
        internal memberCache MemberCache;

        /// <summary>
        /// 获取班级信息
        /// </summary>
        /// <param name="id">班级标识</param>
        /// <returns>班级</returns>
        [fastCSharp.code.cSharp.tcpMethod]
        internal static Class Get(int id)
        {
            return Cache[id];
        }
        /// <summary>
        /// 获取班级标识集合
        /// </summary>
        /// <returns>班级标识集合</returns>
        [fastCSharp.code.cSharp.tcpMethod]
        private static int[] getIds()
        {
            return Cache.Values.getArray(value => value.Id);
        }
        /// <summary>
        /// 获取学生标识集合
        /// </summary>
        /// <param name="id">班级标识</param>
        /// <returns>学生标识集合</returns>
        [fastCSharp.code.cSharp.tcpMethod]
        internal static int[] GetStudentIds(int id)
        {
            Student.WaitSqlLoaded();
            return Cache[id].MemberCache.Students.getArray(value => value.Id);
        }

        /// <summary>
        /// 数据缓存
        /// </summary>
        internal static readonly fastCSharp.sql.cache.whole.events.identityArray<Class, sqlModel.Class, memberCache> Cache = createCache(value => value.MemberCache);
        /// <summary>
        /// 数据缓存初始化
        /// </summary>
        static Class()
        {
            if (sqlTable != null) sqlLoaded();
        }
        /// <summary>
        /// 初始化测试数据
        /// </summary>
        internal static void Initialize()
        {
            sqlTable.Insert(new Class { Name = "软件 1 班", Discipline = demo.sqlModel.member.discipline.软件工程, DateRange = new demo.sqlModel.member.dateRange { Start = new DateTime(2012, 9, 1), End = new DateTime(2015, 7, 1) } });
            sqlTable.Insert(new Class { Name = "软件 2 班", Discipline = demo.sqlModel.member.discipline.软件工程, DateRange = new demo.sqlModel.member.dateRange { Start = new DateTime(2013, 9, 1), End = new DateTime(2016, 7, 1) } });
        }
    }
}
