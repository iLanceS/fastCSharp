using System;
using System.IO;
using fastCSharp.io.compression;
using fastCSharp.web;
using fastCSharp.threading;
using System.Threading;
using System.Runtime.CompilerServices;

namespace fastCSharp.net.tcp.http
{
    /// <summary>
    /// HTTP响应
    /// </summary>
    //[fastCSharp.code.cSharp.serialize(IsReferenceMember = false)]
    public sealed partial class response : IDisposable
    {
        /// <summary>
        /// HTTP响应状态
        /// </summary>
        public sealed class stateInfo : Attribute
        {
            /// <summary>
            /// 编号
            /// </summary>
            public int Number;
            /// <summary>
            /// 状态输出文本
            /// </summary>
            public string Text;
            /// <summary>
            /// 写入状态
            /// </summary>
            /// <param name="data"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal unsafe void Write(byte* data)
            {
                fixed (char* textFixed = Text)
                {
                    char* read = textFixed;
                    for (byte* end = data + Text.Length; data != end; *data++ = (byte)*read++) ;
                }
            }
            /// <summary>
            /// 是否错误状态类型
            /// </summary>
            public bool IsError;
        }
        /// <summary>
        /// HTTP状态类型
        /// </summary>
        public enum state: byte
        {
            /// <summary>
            /// 未知状态
            /// </summary>
            Unknown,
            /// <summary>
            /// 允许客户端继续发送数据
            /// </summary>
            [stateInfo(Number = 100, Text = @" 100 Continue
")]
            Continue100,
            /// <summary>
            /// WebSocket握手
            /// </summary>
            [stateInfo(Number = 101, Text = @" 101 Switching Protocols
")]
            WebSocket101,
            /// <summary>
            /// 客户端请求成功
            /// </summary>
            [stateInfo(Number = 200, Text = @" 200 OK
")]
            Ok200,
            /// <summary>
            /// 成功处理了Range头的GET请求
            /// </summary>
            [stateInfo(Number = 206, Text = @" 206 Partial Content
")]
            PartialContent206,
            /// <summary>
            /// 永久重定向
            /// </summary>
            [stateInfo(Number = 301, Text = @" 301 Moved Permanently
")]
            MovedPermanently301,
            /// <summary>
            /// 临时重定向
            /// </summary>
            [stateInfo(Number = 302, Text = @" 302 Found
")]
            Found302,
            /// <summary>
            /// 资源未修改
            /// </summary>
            [stateInfo(Number = 304, Text = @" 304 Not Changed
")]
            NotChanged304,
            /// <summary>
            /// 客户端请求有语法错误，不能被服务器所理解
            /// </summary>
            [stateInfo(IsError = true, Number = 400, Text = @" 400 Bad Request
")]
            BadRequest400,
            /// <summary>
            /// 请求未经授权，这个状态代码必须和WWW-Authenticate报头域一起使用
            /// </summary>
            [stateInfo(IsError = true, Number = 401, Text = @" 401 Unauthorized
")]
            Unauthorized401,
            /// <summary>
            /// 服务器收到请求，但是拒绝提供服务
            /// WWW-Authenticate响应报头域必须被包含在401（未授权的）响应消息中，客户端收到401响应消息时候，并发送Authorization报头域请求服务器对其进行验证时，服务端响应报头就包含该报头域。
            /// eg：WWW-Authenticate:Basic realm="Basic Auth Test!"  可以看出服务器对请求资源采用的是基本验证机制。
            /// </summary>
            [stateInfo(IsError = true, Number = 403, Text = @" 403 Forbidden
")]
            Forbidden403,
            /// <summary>
            /// 请求资源不存在
            /// </summary>
            [stateInfo(IsError = true, Number = 404, Text = @" 404 Not Found
")]
            NotFound404,
            /// <summary>
            /// 不允许使用的方法
            /// </summary>
            [stateInfo(IsError = true, Number = 405, Text = @" 405 Method Not Allowed
")]
            MethodNotAllowed405,
            /// <summary>
            /// Request Timeout
            /// </summary>
            [stateInfo(IsError = true, Number = 408, Text = @" 408 Request Timeout
")]
            RequestTimeout408,
            /// <summary>
            /// Range请求无效
            /// </summary>
            [stateInfo(IsError = true, Number = 416, Text = @" 416 Request Range Not Satisfiable
")]
            RangeNotSatisfiable416,
            /// <summary>
            /// 服务器发生不可预期的错误
            /// </summary>
            [stateInfo(IsError = true, Number = 500, Text = @" 500 Internal Server Error
")]
            ServerError500,
            /// <summary>
            /// 服务器当前不能处理客户端的请求，一段时间后可能恢复正常
            /// </summary>
            [stateInfo(IsError = true, Number = 503, Text = @" 503 Server Unavailable
")]
            ServerUnavailable503,
        }
        /// <summary>
        /// 资源未修改
        /// </summary>
        internal static readonly response NotChanged304 = new response { State = response.state.NotChanged304 };
        /// <summary>
        /// Range请求无效
        /// </summary>
        internal static readonly response RangeNotSatisfiable416 = new response { State = response.state.RangeNotSatisfiable416 };
        /// <summary>
        /// 空页面输出
        /// </summary>
        internal static readonly response Blank = new response
        {
            State = response.state.Ok200,
            CacheControl = response.StaticFileCacheControl,
            LastModified = ("Mon, 20 Apr 1981 08:03:16 GMT").getBytes()
        };
        /// <summary>
        /// 默认内容类型头部
        /// </summary>
        internal static readonly byte[] HtmlContentType = ("text/html; charset=" + fastCSharp.config.appSetting.Encoding.WebName).getBytes();
        /// <summary>
        /// 默认内容类型头部
        /// </summary>
        internal static readonly byte[] JsContentType = ("application/x-javascript; charset=" + fastCSharp.config.appSetting.Encoding.WebName).getBytes();
        /// <summary>
        /// ZIP文件输出类型
        /// </summary>
        private static readonly byte[] zipContentType = fastCSharp.web.contentTypeInfo.GetContentType("zip");
        /// <summary>
        /// 文本文件输出类型
        /// </summary>
        private static readonly byte[] textContentType = fastCSharp.web.contentTypeInfo.GetContentType("txt");
        /// <summary>
        /// FLV文件输出类型
        /// </summary>
        private static readonly byte[] flvContentType = fastCSharp.web.contentTypeInfo.GetContentType("flv");
        /// <summary>
        /// GZIP压缩响应头部
        /// </summary>
        internal static readonly byte[] GZipEncoding = ("gzip").getBytes();
        /// <summary>
        /// 非缓存参数输出
        /// </summary>
        private static readonly byte[] noStoreBytes = ("public, no-store").getBytes();
        /// <summary>
        /// 缓存过期
        /// </summary>
        internal static readonly byte[] ZeroAgeBytes = ("public, max-age=0").getBytes();
        /// <summary>
        /// 缓存控制参数
        /// </summary>
        internal const int StaticFileCacheControlSeconds = 10 * 365 * 24 * 60 * 60;
        /// <summary>
        /// 缓存控制参数
        /// </summary>
        internal static readonly byte[] StaticFileCacheControl = ("public, max-age=" + StaticFileCacheControlSeconds.toString()).getBytes();
        /// <summary>
        /// GZIP压缩响应头部字节尺寸
        /// </summary>
        internal static readonly int GZipSize = header.ContentEncoding.Length + GZipEncoding.Length + 2;
        /// <summary>
        /// Cookie集合
        /// </summary>
        internal list<cookie> Cookies = new list<cookie>();
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public byte[] LastModified;
        /// <summary>
        /// 缓存匹配标识
        /// </summary>
        internal byte[] ETag;
        /// <summary>
        /// 输出内容类型
        /// </summary>
        public byte[] ContentType;
        /// <summary>
        /// 输出内容压缩编码
        /// </summary>
        internal byte[] ContentEncoding;
        /// <summary>
        /// 重定向
        /// </summary>
        internal subArray<byte> Location;
        /// <summary>
        /// 缓存参数
        /// </summary>
        public byte[] CacheControl;
        /// <summary>
        /// 内容描述
        /// </summary>
        public byte[] ContentDisposition;
        /// <summary>
        /// 跨域访问权限
        /// </summary>
        internal bufferIndex AccessControlAllowOrigin;
        /// <summary>
        /// 输出内容
        /// </summary>
        internal subArray<byte> Body;
        /// <summary>
        /// 获取包含HTTP头部的输出内容
        /// </summary>
        internal subArray<byte> HeaderBody
        {
            get
            {
                int size = unsafer.memory.GetInt(Body.array);
                return size == 0 ? default(subArray<byte>) : subArray<byte>.Unsafe(Body.array, Body.startIndex - size, Body.length + size);
            }
        }
        /// <summary>
        /// 输出内容数组
        /// </summary>
        public byte[] BodyData
        {
            get { return Body.array; }
        }
        /// <summary>
        /// 输出缓存流
        /// </summary>
        internal unmanagedStream BodyStream;
        /// <summary>
        /// 输出缓存流
        /// </summary>
        public unmanagedStream UnsafeBodyStream
        {
            get { return BodyStream; }
        }
        /// <summary>
        /// JSON输出流
        /// </summary>
        private charStream jsonStream;
        /// <summary>
        /// 临时缓存区
        /// </summary>
        internal byte[] Buffer;
        /// <summary>
        /// 输出内容重定向文件
        /// </summary>
        private string bodyFile;
        /// <summary>
        /// 输出内容重定向文件
        /// </summary>
        public string BodyFile
        {
            get { return bodyFile; }
            set
            {
                bodyFile = value;
                if (value != null) Body.UnsafeSetLength(0);
            }
        }
        /// <summary>
        /// 输出内容长度
        /// </summary>
        public long BodySize
        {
            get
            {
                return BodyFile == null ? Body.length : fileSize();
            }
        }
        /// <summary>
        /// 文件长度
        /// </summary>
        /// <returns></returns>
        private long fileSize()
        {
            try
            {
                return new FileInfo(BodyFile).Length;
            }
            catch (Exception error)
            {
                log.Default.Add(error, BodyFile, false);
                return 0;
            }
        }
        /// <summary>
        /// HTTP响应状态
        /// </summary>
        public state State;
        /// <summary>
        /// 是否已经释放资源
        /// </summary>
        private int isDisposed;
        /// <summary>
        /// 是否可以覆盖HTTP预留头部
        /// </summary>
        internal bool CanHeader;
        /// <summary>
        /// 输出数据是否一次性(不可重用)
        /// </summary>
        private bool isBodyOnlyOnce;
        /// <summary>
        /// 是否使用HTTP响应池
        /// </summary>
        internal bool IsPool;
        /// <summary>
        /// HTTP响应
        /// </summary>
        private response() { }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.Increment(ref isDisposed) == 1)
            {
                if (IsPool) Interlocked.Decrement(ref poolNewCount);
                else Interlocked.Decrement(ref newCount);
            }
            pub.Dispose(ref BodyStream);
        }
        /// <summary>
        /// 清除数据
        /// </summary>
        internal void Clear()
        {
            if (isBodyOnlyOnce)
            {
                isBodyOnlyOnce = false;
                Body.UnsafeSet(Buffer, 0, 0);
                Buffer = nullValue<byte>.Array;
            }
            else
            {
                byte[] buffer = Buffer, data = Body.array;
                if (buffer.Length > data.Length)
                {
                    Body.UnsafeSet(buffer, 0, 0);
                    Buffer = data;
                }
                else Body.UnsafeSetLength(0);
            }
            State = state.ServerError500;
            CanHeader = false;
            bodyFile = null;
            Location.Null();
            LastModified = CacheControl = ContentType = ContentEncoding = ETag = ContentDisposition = null;
            AccessControlAllowOrigin.Value = 0;
            Cookies.Empty();
            BodyStream.Clear();
            IsPool = false;
        }
        /// <summary>
        /// 设置输出数据
        /// </summary>
        /// <param name="data">输出数据</param>
        /// <param name="isBodyOnlyOnce">输出数据是否一次性(不可重用)</param>
        /// <param name="canHeader">是否可以覆盖HTTP预留头部</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void SetBody(subArray<byte> data, bool isBodyOnlyOnce = true, bool canHeader = false)
        {
            SetBody(ref data, isBodyOnlyOnce, canHeader);
        }
        /// <summary>
        /// 设置输出数据
        /// </summary>
        /// <param name="data">输出数据</param>
        /// <param name="isBodyOnlyOnce">输出数据是否一次性(不可重用)</param>
        /// <param name="canHeader">是否可以覆盖HTTP预留头部</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void SetBody(ref subArray<byte> data, bool isBodyOnlyOnce = true, bool canHeader = false)
        {
            CanHeader = canHeader;
            if (data.length == 0) Body.UnsafeSetLength(0);
            else
            {
                if (!this.isBodyOnlyOnce) Buffer = Body.array;
                Body = data;
                this.isBodyOnlyOnce = isBodyOnlyOnce;
            }
        }
        /// <summary>
        /// 获取JSON序列化输出缓冲区
        /// </summary>
        /// <param name="data"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe charStream ResetJsonStream(void* data, int size)
        {
            if (jsonStream == null) return jsonStream = new charStream((char*)data, size >> 1);
            jsonStream.UnsafeReset((byte*)data, size);
            return jsonStream;
        }
        /// <summary>
        /// 设置非缓存参数输出
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void NoStore()
        {
            if (LastModified == null && CacheControl == null && ETag == null) CacheControl = noStoreBytes;
        }
        ///// <summary>
        ///// 设置缓存过期
        ///// </summary>
        //public void ZeroAge()
        //{
        //    CacheControl = zeroAgeBytes;
        //}
        /// <summary>
        /// 设置缓存匹配标识
        /// </summary>
        /// <param name="eTag">缓存匹配标识</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void SetETag(byte[] eTag)
        {
            ETag = eTag;
            if (CacheControl == null) CacheControl = ZeroAgeBytes;
        }
        /// <summary>
        /// 设置js内容类型
        /// </summary>
        /// <param name="domainServer">域名服务</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void SetJsContentType(domainServer domainServer)
        {
            ContentType = domainServer.JsContentType;
        }
        /// <summary>
        /// 设置zip内容类型
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void SetZipContentType()
        {
            ContentType = zipContentType;
        }
        /// <summary>
        /// 设置文本内容类型
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void SetTextContentType()
        {
            ContentType = textContentType;
        }
        /// <summary>
        /// 设置FLV内容类型
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void SetFlvContentType()
        {
            ContentType = flvContentType;
        }
        /// <summary>
        /// 设置静态永久缓存
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void SetStaticCacheControl()
        {
            CacheControl = StaticFileCacheControl;
        }
        /// <summary>
        /// 获取压缩数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="memoryPool">数据缓冲区内存池</param>
        /// <param name="seek">起始位置</param>
        /// <returns>压缩数据,失败返回null</returns>
        internal static subArray<byte> GetCompress(ref subArray<byte> data, memoryPool memoryPool = null, int seek = 0)
        {
            if (data.length > GZipSize)
            {
                subArray<byte> compressData = stream.GZip.GetCompress(data.array, data.startIndex, data.length, seek, memoryPool);
                if (compressData.length != 0)
                {
                    if (compressData.length + GZipSize < data.length) return compressData;
                    if (memoryPool != null) memoryPool.PushNotNull(compressData.array);
                }
            }
            return default(subArray<byte>);
        }
        /// <summary>
        /// HTTP响应池申请数量
        /// </summary>
        private static int poolNewCount;
        /// <summary>
        /// HTTP响应池申请数量
        /// </summary>
        public static int PoolNewCount
        {
            get { return poolNewCount; }
        }
        /// <summary>
        /// 非HTTP响应池申请数量
        /// </summary>
        private static int newCount;
        /// <summary>
        /// 非HTTP响应池申请数量
        /// </summary>
        public static int NewCount
        {
            get { return newCount; }
        }
        /// <summary>
        /// 获取HTTP响应
        /// </summary>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static response Get()
        {
            response response = typePool<response>.Pop();
            if (response != null)
            {
                response.IsPool = true;
                return response;
            }
            Interlocked.Increment(ref poolNewCount);
            return new response { BodyStream = new unmanagedStream(), IsPool = true, Body = subArray<byte>.Unsafe(nullValue<byte>.Array, 0, 0), Buffer = nullValue<byte>.Array };
        }
        /// <summary>
        /// 获取HTTP响应
        /// </summary>
        /// <returns>HTTP响应</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static response New()
        {
            Interlocked.Increment(ref newCount);
            return new response { Body = subArray<byte>.Unsafe(nullValue<byte>.Array, 0, 0), Buffer = nullValue<byte>.Array };
        }
        /// <summary>
        /// 取消使用HTTP响应池
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void CancelPool()
        {
            if (IsPool)
            {
                IsPool = false;
                Interlocked.Decrement(ref poolNewCount);
                Interlocked.Increment(ref newCount);
            }
        }
        /// <summary>
        /// 获取HTTP响应Cookie
        /// </summary>
        /// <param name="response">HTTP响应</param>
        /// <returns>HTTP响应</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static response GetCookie(response response)
        {
            response value = Get();
            if (response != null)
            {
                //value.CacheControl = response.CacheControl;
                //value.ContentEncoding = response.ContentEncoding;
                //value.ContentType = response.ContentType;
                //value.ETag = response.ETag;
                //value.LastModified = response.LastModified;
                //value.ContentDisposition = response.ContentDisposition;
                //value.AccessControlAllowOrigin.Value = response.AccessControlAllowOrigin.Value;
                //int count = response.Cookies.Count;
                //if (count != 0) value.Cookies.Add(response.Cookies.array, 0, count);
                list<cookie> cookies = Interlocked.Exchange(ref response.Cookies, value.Cookies);
                value.Cookies = cookies;
            }
            return value;
        }
        /// <summary>
        /// 添加到HTTP响应池
        /// </summary>
        /// <param name="response">HTTP响应</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static void Push(ref response response)
        {
            if (response != null)
            {
                response value = response;
                response = null;
                if (value != null && value.IsPool)
                {
                    value.Clear();
                    typePool<response>.PushNotNull(value);
                }
            }
        }
    }
}
