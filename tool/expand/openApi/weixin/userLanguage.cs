using System;

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// 用户语言
    /// </summary>
    public struct userLanguage
    {
        /// <summary>
        /// 用户的标识，对当前公众号唯一
        /// </summary>
        public string openid;
        /// <summary>
        /// 国家地区语言版本
        /// </summary>
        public language lang;
    }
}
