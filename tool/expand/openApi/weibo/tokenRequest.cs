using System;

namespace fastCSharp.openApi.weibo
{
    /// <summary>
    /// 访问令牌请求
    /// </summary>
    internal partial class tokenRequest : config
    {
#pragma warning disable 414
        /// <summary>
        /// 请求的类型
        /// </summary>
        private string grant_type = "authorization_code";
#pragma warning restore 414
        /// <summary>
        /// authorization_code
        /// </summary>
        internal string code;
    }
}
