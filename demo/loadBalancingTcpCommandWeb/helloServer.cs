using System;

namespace fastCSharp.demo.loadBalancingTcpCommandWeb
{
#if HELLO
    /// <summary>
    /// hello.html文件测试服务
    /// </summary>
    public sealed class helloServer : fastCSharp.net.tcp.http.domainServer.fileServer
    {
        /// <summary>
        /// HTML文件缓存是否预留HTTP头部
        /// </summary>
        protected override bool isCacheHtmlHttpHeader { get { return true; } }
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
#else
#endif
}
