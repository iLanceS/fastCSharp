using System;
using fastCSharp;
using fastCSharp.code.cSharp;

namespace fastCSharp.demo.sqlModel.path
{
    /// <summary>
    /// 学生 URL
    /// </summary>
    [fastCSharp.code.cSharp.webPath(Type = typeof(sqlModel.Student))]
    public struct Student
    {
        /// <summary>
        /// 学生标识
        /// </summary>
        public int Id;
        /// <summary>
        /// 查询字符串
        /// </summary>
        [fastCSharp.code.ignore]
        public string Query
        {
            get { return "StudentId=" + Id.toString(); }
        }

        /// <summary>
        /// 班级首页
        /// </summary>
        public webView.hashUrl Index
        {
            get
            {
                return new webView.hashUrl { Path = "/Student.html", Query = Query };
            }
        }
    }
}
