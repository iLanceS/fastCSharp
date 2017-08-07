using System;

namespace fastCSharp.demo.chatWeb
{
    /// <summary>
    /// 主页重定向
    /// </summary>
    [fastCSharp.code.cSharp.webCall(IsPool = true)]
    internal sealed class index : fastCSharp.code.cSharp.webCall.call<index>
    {
        /// <summary>
        /// 主页重定向
        /// </summary>
        [fastCSharp.code.cSharp.webCall(FullName = "")]
        public unsafe void load()
        {
            location("/chat.html");
        }
    }
}
