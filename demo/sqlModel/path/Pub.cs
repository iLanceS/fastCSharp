using System;

namespace fastCSharp.demo.sqlModel.path
{
    /// <summary>
    /// 公共全局 URL
    /// </summary>
    [fastCSharp.code.cSharp.webView.clientType(Name = "fastCSharpPath.Pub", MemberName = null)]
    [fastCSharp.code.cSharp.webPath]
    public struct Pub
    {
        /// <summary>
        /// 404
        /// </summary>
        [fastCSharp.code.cSharp.webView.outputAjax(IsIgnoreCurrent = true)]
        public string NotFound404
        {
            get { return "/404.html"; }
        }
        /// <summary>
        /// 班级列表
        /// </summary>
        [fastCSharp.code.cSharp.webView.outputAjax(IsIgnoreCurrent = true)]
        public string ClassList
        {
            get { return "/ClassList.html"; }
        }
    }
}
