using System;

namespace fastCSharp.demo.chatWeb.data
{
    /// <summary>
    /// 聊天信息
    /// </summary>
    internal sealed partial class message
    {
        /// <summary>
        /// 发送者
        /// </summary>
        public string User;
        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime Time = date.NowSecond;
        /// <summary>
        /// 发送内容
        /// </summary>
        public string Message;
    }
}
