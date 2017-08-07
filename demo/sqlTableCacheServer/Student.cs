using System;

namespace fastCSharp.demo.sqlTableCacheServer
{
    /// <summary>
    /// 学生表格定义
    /// </summary>
    [fastCSharp.code.cSharp.tcpCall(Service = pub.DataReaderServer)]
    [fastCSharp.emit.sqlTable(ConnectionName = pub.SqlConnection, IsLoadIdentity = false)]
    public partial class Student : fastCSharp.demo.sqlModel.Student.sqlModel<Student>
    {
        /// <summary>
        /// 远程对象扩展
        /// </summary>
        public struct remote
        {
            /// <summary>
            /// 学生信息
            /// </summary>
            internal Student Value;
            /// <summary>
            /// 获取当前班级
            /// </summary>
            public Class Class
            {
                get { return clientCache.Class[Value.ClassId]; }
            }
            /// <summary>
            /// 加入班级时间信息
            /// </summary>
            public struct classDate
            {
                /// <summary>
                /// 加入班级时间信息
                /// </summary>
                public sqlModel.member.classDate ClassDate;
                /// <summary>
                /// 班级信息
                /// </summary>
                public Class Class
                {
                    get { return clientCache.Class[ClassDate.ClassId]; }
                }
            }
            /// <summary>
            /// 按加入时间排序的班级集合
            /// </summary>
            public classDate[] Classes
            {
                get
                {
                    return Value.Classes.getArray(value => new classDate { ClassDate = value });
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
        /// 获取学生信息
        /// </summary>
        /// <param name="id">学生标识</param>
        /// <returns>学生</returns>
        [fastCSharp.code.cSharp.tcpMethod]
        internal static Student Get(int id)
        {
            return sqlCache[id];
        }

        /// <summary>
        /// 添加学生处理
        /// </summary>
        /// <param name="value"></param>
        private static void onInserted(Student value)
        {
            Class Class = Class.Cache[value.ClassId];
            if (Class != null) Class.SqlStreamLoad.StudentCount(Class.StudentCount + 1);
        }
        /// <summary>
        /// 修改学生处理
        /// </summary>
        /// <param name="cacheValue">当前缓存对象</param>
        /// <param name="value">修改之后的对象数据</param>
        /// <param name="oldValue">修改之前的对象数据</param>
        /// <param name="memberMap">被修改成员位图</param>
        private static void onUpdated(Student cacheValue, Student value, Student oldValue, fastCSharp.code.memberMap<sqlModel.Student> memberMap)
        {
            if (memberIndexs.Classes.IsMember(memberMap))
            {
                Class Class = Class.Cache[oldValue.ClassId];
                if (Class != null) Class.SqlStreamLoad.StudentCount(Class.StudentCount - 1);

                onInserted(cacheValue);
            }
        }

        /// <summary>
        /// 数据缓存初始化
        /// </summary>
        static Student()
        {
            if (sqlTable != null)
            {
                sqlCache.CreateMemberList(Class.Cache, value => value.ClassId, value => value.Students, true);

                foreach (Student value in sqlCache.Values) ++Class.Cache[value.ClassId].StudentCount;

                sqlLoaded(onInserted, onUpdated);

                if (sqlCache.Count == 0)
                {
                    Class.Initialize();
                    Initialize();
                }
            }
        }
        /// <summary>
        /// 初始化测试数据
        /// </summary>
        internal static void Initialize()
        {
            Student[] students = new Student[] {
                new Student { Name = "张三", Email = "zhangsan@fastCSharp.com", Password = "zhangsan", Birthday = new DateTime(1994, 3, 3), Gender = demo.sqlModel.member.gender.男 },
                new Student { Name = "李四", Email = "lisi@fastCSharp.com", Password = "lisi", Birthday = new DateTime(1994, 4, 4), Gender = demo.sqlModel.member.gender.女 },
                new Student { Name = "王五", Email = "wangwu@fastCSharp.com", Password = "wangwu", Birthday = new DateTime(1995, 5, 5), Gender = demo.sqlModel.member.gender.男 },
                new Student { Name = "赵六", Email = "zhaoliu@fastCSharp.com", Password = "zhaoliu", Birthday = new DateTime(1995, 4, 4), Gender = demo.sqlModel.member.gender.女 },
            };
            int index = 0;
            foreach (Class value in Class.Cache.Values)
            {
                Student student = students[index++];
                student.Classes = new demo.sqlModel.member.classDate[] { new demo.sqlModel.member.classDate { ClassId = value.Id, Date = value.DateRange.Start } };
                sqlTable.Insert(student);

                student = students[index++];
                student.Classes = new demo.sqlModel.member.classDate[] { new demo.sqlModel.member.classDate { ClassId = value.Id, Date = value.DateRange.Start } };
                sqlTable.Insert(student);
            }
        }
    }
}
