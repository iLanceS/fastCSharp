using System;

namespace fastCSharp.openApi.qq
{
    /// <summary>
    /// 微博
    /// </summary>
    public sealed class microblogReturn : errorCode
    {
        /// <summary>
        /// 微博ID
        /// </summary>
        public struct microblogId
        {
            /// <summary>
            /// 微博消息的ID，用来唯一标识一条微博消息
            /// </summary>
            public string id;
            /// <summary>
            /// 微博消息的发表时间
            /// </summary>
            public long time;
        }
        /// <summary>
        /// 微博ID
        /// </summary>
        public microblogId data;
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="value">微博信息</param>
        /// <returns>微博ID</returns>
        public static implicit operator string(microblogReturn value) { return value != null && value.IsValue ? value.data.id : null; }
    }
}
