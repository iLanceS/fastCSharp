using System;

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// OpenID列表群发消息
    /// </summary>
    public sealed class openIdMessage : bulkMessageBase
    {
        /// <summary>
        /// 接收者，一串OpenID列表，OpenID最少2个，最多10000个
        /// </summary>
        public string[] touser;
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="toJsoner"></param>
        /// <param name="value"></param>
        [fastCSharp.emit.jsonSerialize.custom]
        private static void toJson(fastCSharp.emit.jsonSerializer toJsoner, openIdMessage value)
        {
            toJsoner.UnsafeWriteFirstName("touser");
            toJsoner.UnsafeToJson(value.touser);
            value.toJson(toJsoner);
        }
    }
}
