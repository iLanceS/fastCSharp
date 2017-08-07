using System;

namespace fastCSharp.demo.sqlTableWeb
{
    /// <summary>
    /// 学生页面
    /// </summary>
    [fastCSharp.code.cSharp.webView(IsPool = true)]
    partial class Student : View<Student>
    {
        /// <summary>
        /// 学生信息
        /// </summary>
        [fastCSharp.emit.webView.clearMember]
        sqlTableCacheServer.Student StudentInfo;
        /// <summary>
        /// 学生页面
        /// </summary>
        /// <param name="StudentId">学生标识</param>
        /// <returns>是否成功</returns>
        private bool loadView(int StudentId)
        {
            StudentInfo = fastCSharp.demo.sqlTableCacheServer.clientCache.Student[StudentId];
            if (StudentInfo != null) return true;
            AjaxResponse(webConfig.NotFound404);
            return false;
        }
    }
}
