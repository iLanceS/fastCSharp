using System;

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// 客服的会话
    /// </summary>
    public struct customTime
    {
        /// <summary>
        /// 客户openid
        /// </summary>
        public string openid;
        /// <summary>
        /// 会话创建时间，UNIX时间戳
        /// </summary>
        public long createtime;
    }
}
