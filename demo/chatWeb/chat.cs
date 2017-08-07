using System;
using fastCSharp.code.cSharp;

namespace fastCSharp.demo.chatWeb
{
    /// <summary>
    /// 群聊
    /// </summary>
    [fastCSharp.code.cSharp.webView(IsPage = false, IsPool = true)]
    internal sealed partial class chat : webView.view<chat>
    {
        /// <summary>
        /// 消息集合
        /// </summary>
        public data.message[] Messages;
        /// <summary>
        /// 是否已经登陆状态
        /// </summary>
        public bool IsLogin;
    }
}
