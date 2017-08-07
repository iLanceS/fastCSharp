using System;

namespace fastCSharp.web
{
    /// <summary>
    /// HTTP头部名称数据
    /// </summary>
    public static class header
    {
        /// <summary>
        /// 允许的编码方式
        /// </summary>
        public const string AcceptEncoding = "Accept-Encoding";
        /// <summary>
        /// 允许的编码方式
        /// </summary>
        internal static readonly byte[] AcceptEncodingBytes = AcceptEncoding.getBytes();
        ///// <summary>
        ///// 允许的请求范围方式
        ///// </summary>
        //public const string AcceptRanges = "Accept-Ranges";
        /// <summary>
        /// 跨域权限控制
        /// </summary>
        public const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";
        /// <summary>
        /// 缓存参数
        /// </summary>
        public const string CacheControl = "Cache-Control";
        /// <summary>
        /// 缓存参数
        /// </summary>
        internal static readonly byte[] CacheControlBytes = CacheControl.getBytes();
        /// <summary>
        /// 连接状态
        /// </summary>
        public const string Connection = "Connection";
        /// <summary>
        /// 内容描述名称
        /// </summary>
        public const string ContentDisposition = "Content-Disposition";
        /// <summary>
        /// 连接状态
        /// </summary>
        public static readonly byte[] ConnectionBytes = Connection.getBytes();
        /// <summary>
        /// 压缩编码名称
        /// </summary>
        public const string ContentEncoding = "Content-Encoding";
        /// <summary>
        /// 压缩编码名称
        /// </summary>
        public static readonly byte[] ContentEncodingBytes = ContentEncoding.getBytes();
        /// <summary>
        /// 内容长度名称
        /// </summary>
        public const string ContentLength = "Content-Length";
        /// <summary>
        /// 内容长度名称
        /// </summary>
        public static readonly byte[] ContentLengthBytes = ContentLength.getBytes();
        ///// <summary>
        ///// 内容请求范围名称
        ///// </summary>
        //public const string ContentRange = "Content-Range";
        /// <summary>
        /// 内容类型名称
        /// </summary>
        public const string ContentType = "Content-Type";
        /// <summary>
        /// 内容类型名称
        /// </summary>
        public static readonly byte[] ContentTypeBytes = ContentType.getBytes();
        /// <summary>
        /// Cookie
        /// </summary>
        public const string Cookie = "Cookie";
        /// <summary>
        /// Cookie
        /// </summary>
        internal static readonly byte[] CookieBytes = Cookie.getBytes();
        /// <summary>
        /// 时间
        /// </summary>
        public const string Date = "Date";
        /// <summary>
        /// 缓存匹配标识
        /// </summary>
        public const string ETag = "ETag";
        /// <summary>
        /// 100 Continue确认名称
        /// </summary>
        public const string Expect = "Expect";
        /// <summary>
        /// 100 Continue确认名称
        /// </summary>
        internal static readonly byte[] ExpectBytes = Expect.getBytes();
        /// <summary>
        /// 主机名称
        /// </summary>
        public const string Host = "Host";
        /// <summary>
        /// 主机名称
        /// </summary>
        internal static readonly byte[] HostBytes = Host.getBytes();
        /// <summary>
        /// 文档时间修改标识
        /// </summary>
        public const string IfModifiedSince = "If-Modified-Since";
        /// <summary>
        /// 文档时间修改标识
        /// </summary>
        internal static readonly byte[] IfModifiedSinceBytes = IfModifiedSince.getBytes();
        /// <summary>
        /// 文档匹配标识
        /// </summary>
        public const string IfNoneMatch = "If-None-Match";
        /// <summary>
        /// 文档匹配标识
        /// </summary>
        internal static readonly byte[] IfNoneMatchBytes = IfNoneMatch.getBytes();
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public const string LastModified = "Last-Modified";
        /// <summary>
        /// 重定向
        /// </summary>
        public const string Location = "Location";
        /// <summary>
        /// 重定向
        /// </summary>
        internal static readonly byte[] LocationBytes = Location.getBytes();
        /// <summary>
        /// 来源页面名称
        /// </summary>
        public const string Origin = "Origin";
        /// <summary>
        /// 来源页面名称
        /// </summary>
        internal static readonly byte[] OriginBytes = Origin.getBytes();
        /// <summary>
        /// 请求字节范围
        /// </summary>
        public const string Range = "Range";
        /// <summary>
        /// 请求字节范围
        /// </summary>
        internal static readonly byte[] RangeBytes = Range.getBytes();
        /// <summary>
        /// 来源页面名称
        /// </summary>
        public const string Referer = "Referer";
        /// <summary>
        /// 来源页面名称
        /// </summary>
        internal static readonly byte[] RefererBytes = Referer.getBytes();
        /// <summary>
        /// WebSocket确认连接值
        /// </summary>
        public const string SecWebSocketKey = "Sec-WebSocket-Key";
        /// <summary>
        /// WebSocket确认连接值
        /// </summary>
        internal static readonly byte[] SecWebSocketKeyBytes = SecWebSocketKey.getBytes();
        /// <summary>
        /// WebSocket来源页面名称
        /// </summary>
        public const string SecWebSocketOrigin = "Sec-WebSocket-Origin";
        /// <summary>
        /// WebSocket来源页面名称
        /// </summary>
        internal static readonly byte[] SecWebSocketOriginBytes = SecWebSocketOrigin.getBytes();
        /// <summary>
        /// 输出Cookie
        /// </summary>
        public const string SetCookie = "Set-Cookie";
        /// <summary>
        /// 传输编码
        /// </summary>
        public const string TransferEncoding = "Transfer-Encoding";
        /// <summary>
        /// 传输编码名称
        /// </summary>
        public static readonly byte[] TransferEncodingBytes = TransferEncoding.getBytes();
        /// <summary>
        /// 协议升级支持名称
        /// </summary>
        public const string Upgrade = "Upgrade";
        /// <summary>
        /// 协议升级支持名称
        /// </summary>
        internal static readonly byte[] UpgradeBytes = Upgrade.getBytes();
        /// <summary>
        /// 浏览器参数名称
        /// </summary>
        public const string UserAgent = "User-Agent";
        /// <summary>
        /// 浏览器参数名称
        /// </summary>
        internal static readonly byte[] UserAgentBytes = UserAgent.getBytes();
        /// <summary>
        /// 转发名称
        /// </summary>
        public const string XProwardedFor = "X-Prowarded-For";
        /// <summary>
        /// 转发名称
        /// </summary>
        internal static readonly byte[] XProwardedForBytes = XProwardedFor.getBytes();
    }
}
