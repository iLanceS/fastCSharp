using System;

namespace fastCSharp.document
{
    /// <summary>
    /// 首页视图
    /// </summary>
    [fastCSharp.code.cSharp.webView(IsPool = true, IsReferer = false, IsPage = false)]
    internal partial class index: fastCSharp.code.cSharp.webView.view<index>
    {
        /// <summary>
        /// 环境参数
        /// </summary>
        public environment Environment
        {
            get { return environment.Default; }
        }
        /// <summary>
        /// HTML 模板值前缀
        /// </summary>
        public string At = "=@";
    }
    /// <summary>
    /// 主页重定向
    /// </summary>
    [fastCSharp.code.cSharp.webCall(IsPool = true)]
    internal sealed class Index : fastCSharp.code.cSharp.webCall.call<Index>
    {
        /// <summary>
        /// 主页重定向
        /// </summary>
        [fastCSharp.code.cSharp.webCall(FullName = "")]
        public void Load()
        {
            location("/index.html");
        }
    }
}
