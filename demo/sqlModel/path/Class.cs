using System;
using fastCSharp;
using fastCSharp.code.cSharp;

namespace fastCSharp.demo.sqlModel.path
{
    /// <summary>
    /// 班级 URL
    /// </summary>
    [fastCSharp.code.cSharp.webPath(Type = typeof(sqlModel.Class))]
    public struct Class
    {
        /// <summary>
        /// 班级标识
        /// </summary>
        public int Id;
        /// <summary>
        /// 查询字符串
        /// </summary>
        [fastCSharp.code.ignore]
        public string Query
        {
            get { return "ClassId=" + Id.toString(); }
        }

        /// <summary>
        /// 班级首页
        /// </summary>
        public webView.hashUrl Index
        {
            get
            {
                return new webView.hashUrl { Path = "/Class.html", Query = Query };
            }
        }
    }
}
