using System;

namespace fastCSharp.openApi.qq
{
    /// <summary>
    /// API调用表单
    /// </summary>
    public abstract class apiForm : appId
    {
        /// <summary>
        /// 访问令牌
        /// </summary>
        internal string access_token;
        /// <summary>
        /// 用户身份的标识
        /// </summary>
        internal string openid;
    }
}
