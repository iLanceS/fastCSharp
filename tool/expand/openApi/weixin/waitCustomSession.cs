using System;

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// 未接入会话列表
    /// </summary>
    public sealed class waitCustomSession : isValue
    {
        /// <summary>
        /// 未接入会话数量
        /// </summary>
        public int count;
        /// <summary>
        /// 未接入会话
        /// </summary>
        public struct waitSession
        {
            /// <summary>
            /// 用户来访时间，UNIX时间戳
            /// </summary>
            public long createtime;
            /// <summary>
            /// 指定接待的客服，为空表示未指定客服
            /// </summary>
            public string kf_account;
            /// <summary>
            /// 客户openid
            /// </summary>
            public string openid;
        }
        /// <summary>
        /// 未接入会话列表
        /// </summary>
        public waitSession[] waitcaselist;
    }
}
