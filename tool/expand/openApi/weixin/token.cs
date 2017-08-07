using System;
#pragma warning disable

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// 访问令牌
    /// </summary>
    internal sealed class token : isValue
    {
        /// <summary>
        /// 获取到的凭证
        /// </summary>
        public string access_token;
        /// <summary>
        /// 凭证有效时间，单位：秒
        /// </summary>
        public int expires_in;
    }
}
