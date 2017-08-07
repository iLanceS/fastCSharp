using System;

namespace fastCSharp.document
{
    /// <summary>
    /// 网站生成配置
    /// </summary>
    internal sealed class webConfig : fastCSharp.code.webConfig
    {
        /// <summary>
        /// 默认主域名
        /// </summary>
        public override string MainDomain
        {
            get { return "127.0.0.1:8104"; }
        }
        /// <summary>
        /// 轮询网站域名
        /// </summary>
        public override string PollDomain
        {
            get { return "127.0.0.1:8120"; }
        }
    }
}
