using System;

namespace fastCSharp.openApi.weibo
{
    /// <summary>
    /// 访问令牌+用户身份的标识，用于保存
    /// </summary>
    public struct tokenUid
    {
        /// <summary>
        /// 访问令牌
        /// </summary>
        public string Token;
        /// <summary>
        /// 用户身份的标识
        /// </summary>
        public string Uid;
    }
}
