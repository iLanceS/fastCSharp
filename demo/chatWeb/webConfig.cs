using System;

namespace fastCSharp.demo.chatWeb
{
    /// <summary>
    /// 网站生成配置
    /// </summary>
    internal sealed class webConfig : fastCSharp.code.webConfig
    {
        /// <summary>
        /// Session类型
        /// </summary>
        public override Type SessionType
        {
            get { return typeof(string); }
        }
        /// <summary>
        /// 默认主域名
        /// </summary>
        public override string MainDomain
        {
            get { return "127.0.0.1:8103"; }
        }
        /// <summary>
        /// 轮询网站域名
        /// </summary>
        public override string PollDomain
        {
            get { return "127.0.0.1:8112"; }
        }
        /// <summary>
        /// 视图加载失败重定向
        /// </summary>
        public override string NoViewLocation
        {
            get { return "http://127.0.0.1:8103/"; }
        }
    }
}
