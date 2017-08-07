using System;

namespace fastCSharp.demo.loadBalancingTcpCommandWeb
{
    /// <summary>
    /// WEB服务器
    /// </summary>
    public partial class webServer
    {
        /// <summary>
        /// 是否输出日期
        /// </summary>
        protected override bool isResponseDate { get { return false; } }
        /// <summary>
        /// 是否输出服务器信息
        /// </summary>
        protected override bool isResponseServer { get { return false; } }
        /// <summary>
        /// 是否输出缓存参数
        /// </summary>
        protected override bool isResponseCacheControl { get { return false; } }
        /// <summary>
        /// 输出内容类型
        /// </summary>
        protected override bool isResponseContentType { get { return false; } }
    }
}
