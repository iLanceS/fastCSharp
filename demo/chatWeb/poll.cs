using System;
using fastCSharp.threading;
using fastCSharp.code.cSharp;

namespace fastCSharp.demo.chatWeb
{
    /// <summary>
    /// 长轮询
    /// </summary>
    [fastCSharp.code.cSharp.webView(IsAjax = true, IsPage = false, IsPool = true)]
    internal sealed partial class poll : webView.view<poll>
    {
        /// <summary>
        /// 长轮询消息
        /// </summary>
        public class message
        {
            /// <summary>
            /// 空消息
            /// </summary>
            public static readonly message Null = new message();
            /// <summary>
            /// 用户信息集合版本
            /// </summary>
            public int UserVersion;
            /// <summary>
            /// 用户信息集合
            /// </summary>
            public string[] Users;
            /// <summary>
            /// 信息集合
            /// </summary>
            public data.message[] Messages;
        }
        /// <summary>
        /// 客户端查询参数
        /// </summary>
        private struct clientQuery
        {
            /// <summary>
            /// 用户版本信息
            /// </summary>
            public int UserVersion;
        }
        /// <summary>
        /// 长轮询消息
        /// </summary>
        public message Message;
        /// <summary>
        /// 加载视图数据
        /// </summary>
        /// <param name="verify">登陆用户</param>
        /// <param name="query">客户端查询参数</param>
        /// <returns></returns>
        private bool loadView(string verify, clientQuery query)
        {
            JsContentType();
            setAsynchronous();
            Message = chatWeb.ajax.user.GetMessage(verify, query.UserVersion, message =>
            {
                Message = message;
                callback();
            });
            if (Message != null)
            {
                if (Message == message.Null) Message = null;
                callback();
            }
            return true;
        }
    }
}
