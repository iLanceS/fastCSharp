using System;
using fastCSharp.threading;
using fastCSharp.net.tcp.http;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using fastCSharp.reflection;
using System.Runtime.CompilerServices;

namespace fastCSharp.code.cSharp
{
    /// <summary>
    /// WEB页面
    /// </summary>
    public abstract class webPage : ignoreMember
    {
        /// <summary>
        /// WEB页面
        /// </summary>
        public interface IWebPage
        {
            ///// <summary>
            ///// HTTP套接字接口设置
            ///// </summary>
            //socketBase Socket { set; }
            ///// <summary>
            ///// 域名服务设置
            ///// </summary>
            //domainServer DomainServer { set; }
            /// <summary>
            /// 根据HTTP请求表单值获取保存文件全称
            /// </summary>
            /// <param name="value">HTTP请求表单值</param>
            /// <returns>文件全称</returns>
            string GetSaveFileName(ref requestForm.value value);
            ///// <summary>
            ///// 套接字请求编号
            ///// </summary>
            //long SocketIdentity { get; }
        }
        /// <summary>
        /// WEB类型调用
        /// </summary>
        public abstract class pageBase
        {
            /// <summary>
            /// WebView每秒请求数量
            /// </summary>
            protected static secondCount secondCount = new secondCount(fastCSharp.config.http.Default.CountSeconds);
            /// <summary>
            /// WebView每秒请求数量
            /// </summary>
            public static int[] SecondCount
            {
                get { return secondCount.Counts; }
            }
            /// <summary>
            /// HTTP套接字接口设置
            /// </summary>
            internal socketBase socket;
            /// <summary>
            /// HTTP套接字接口设置
            /// </summary>
            internal socketBase Socket
            {
                get { return socket; }
                set
                {
                    if (socket == null) socket = value;
                    else log.Error.Throw(log.exceptionType.ErrorOperation);
                }
            }
            /// <summary>
            /// 远程终结点
            /// </summary>
            public EndPoint RemoteEndPoint
            {
                get { return socket.RemoteEndPoint; }
            }
            /// <summary>
            /// 域名服务
            /// </summary>
            internal domainServer domainServer;
            /// <summary>
            /// 域名服务
            /// </summary>
            internal domainServer DomainServer
            {
                get { return domainServer; }
                set
                {
                    if (domainServer == null) domainServer = value;
                    else log.Error.Throw(log.exceptionType.ErrorOperation);
                }
            }
            /// <summary>
            /// 域名服务工作文件路径
            /// </summary>
            public string WorkPath { get { return domainServer.WorkPath; } }
            /// <summary>
            /// HTTP请求头部
            /// </summary>
            protected requestHeader requestHeader;
            /// <summary>
            /// 套接字请求编号
            /// </summary>
            public long SocketIdentity { get; internal set; }
            /// <summary>
            /// JSON解析配置参数
            /// </summary>
            protected virtual fastCSharp.emit.jsonParser.config jsonParserConfig { get { return null; } }
            /// <summary>
            /// XML解析配置参数
            /// </summary>
            protected virtual fastCSharp.emit.xmlParser.config xmlParserConfig { get { return null; } }
            /// <summary>
            /// 解析web调用参数
            /// </summary>
            /// <typeparam name="parameterType">web调用参数类型</typeparam>
            /// <param name="parameter">web调用参数</param>
            /// <returns>是否成功</returns>
            protected bool parseParameterQuery<parameterType>(ref parameterType parameter) where parameterType : struct
            {
                if (requestHeader.ParseQuery(ref parameter))
                {
                    subString queryJson = requestHeader.QueryJson;
                    if (queryJson.Length != 0) return fastCSharp.emit.jsonParser.Parse(ref queryJson, ref parameter, jsonParserConfig);
                    subString queryXml = requestHeader.QueryXml;
                    if (queryXml.Length != 0) return fastCSharp.emit.xmlParser.Parse(ref queryXml, ref parameter, xmlParserConfig);
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 解析web调用参数
            /// </summary>
            /// <typeparam name="parameterType">web调用参数类型</typeparam>
            /// <param name="parameter">web调用参数</param>
            /// <returns>是否成功</returns>
            protected bool parseParameterQueryAny<parameterType>(ref parameterType parameter)
            {
                if (requestHeader.ParseQuery(ref parameter))
                {
                    subString queryJson = requestHeader.QueryJson;
                    if (queryJson.Length != 0) return fastCSharp.emit.jsonParser.Parse(ref queryJson, ref parameter, jsonParserConfig);
                    subString queryXml = requestHeader.QueryXml;
                    if (queryXml.Length != 0) return fastCSharp.emit.xmlParser.Parse(ref queryXml, ref parameter, xmlParserConfig);
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// WEB页面
        /// </summary>
        [fastCSharp.emit.webView.clearMember(IsIgnoreCurrent = true)]
        public abstract class page : pageBase, IDisposable
        {
            /// <summary>
            /// 缓存标识处理
            /// </summary>
            public unsafe struct eTag
            {
                /// <summary>
                /// 起始字符
                /// </summary>
                private const uint startChar = ':' + 1;
                /// <summary>
                /// WEB页面
                /// </summary>
                private page page;
                /// <summary>
                /// 客户端缓存有效标识
                /// </summary>
                public subArray<byte> IfNoneMatch
                {
                    get { return page.requestHeader.IfNoneMatch; }
                }
                /// <summary>
                /// 当前数据位置
                /// </summary>
                private byte* Data;
                /// <summary>
                /// 缓存标识长度
                /// </summary>
                private int length;
                /// <summary>
                /// 判断长度是否匹配
                /// </summary>
                public bool IsLength
                {
                    get { return IfNoneMatch.length == length; }
                }
                /// <summary>
                /// 是否处理网站配置版本
                /// </summary>
                private bool isServerVersion;
                /// <summary>
                /// 是否处理页面版本
                /// </summary>
                private bool isPageVersion;
                /// <summary>
                /// 缓存标识处理
                /// </summary>
                /// <param name="page">WEB页面</param>
                public eTag(page page) : this(page, true, true) { }
                /// <summary>
                /// 缓存标识处理
                /// </summary>
                /// <param name="page">WEB页面</param>
                /// <param name="isPageVersion">是否处理页面版本</param>
                /// <param name="isServerVersion">是否处理网站配置版本</param>
                public eTag(page page, bool isPageVersion, bool isServerVersion)
                {
                    this.page = page;
                    this.isServerVersion = isServerVersion;
                    this.isPageVersion = isPageVersion;
                    length = 0;
                    Data = null;
                    if (isServerVersion) AddLength(0);
                    if (isPageVersion) AddLength(0);
                }
                /// <summary>
                /// 添加长度
                /// </summary>
                /// <param name="value">数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void AddLength(DateTime value)
                {
                    length += 12;
                }
                /// <summary>
                /// 添加长度
                /// </summary>
                /// <param name="value">数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void AddLength(ulong value)
                {
                    length += 12;
                }
                /// <summary>
                /// 添加长度
                /// </summary>
                /// <param name="value">数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void AddLength(long value)
                {
                    length += 12;
                }
                /// <summary>
                /// 添加长度
                /// </summary>
                /// <param name="value">数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void AddLength(uint value)
                {
                    length += 6;
                }
                /// <summary>
                /// 添加长度
                /// </summary>
                /// <param name="value">数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void AddLength(int value)
                {
                    length += 6;
                }
                /// <summary>
                /// 添加长度
                /// </summary>
                /// <param name="value">数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void AddLength(ushort value)
                {
                    length += 3;
                }
                /// <summary>
                /// 添加长度
                /// </summary>
                /// <param name="value">数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void AddLength(short value)
                {
                    length += 3;
                }
                /// <summary>
                /// 添加长度
                /// </summary>
                /// <param name="value">数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void AddLength(char value)
                {
                    length += 3;
                }
                /// <summary>
                /// 添加长度
                /// </summary>
                /// <param name="value">数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void AddLength(byte value)
                {
                    length += 2;
                }
                /// <summary>
                /// 添加长度
                /// </summary>
                /// <param name="value">数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void AddLength(sbyte value)
                {
                    length += 2;
                }
                /// <summary>
                /// 添加长度
                /// </summary>
                /// <param name="value">数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void AddLength(bool value)
                {
                    ++length;
                }
                /// <summary>
                /// 添加长度
                /// </summary>
                /// <param name="value">数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void AddLength(string value)
                {
                    if (value != null) length += (value.Length << 1) + value.Length;
                }
                /// <summary>
                /// 设置检测数据
                /// </summary>
                /// <param name="data">检测数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void SetCheckData(byte* data)
                {
                    this.Data = data;
                    if (isServerVersion) Check(page.DomainServer.WebConfig.ETagVersion);
                    if (isPageVersion) Check(page.ETagVersion);
                }
                /// <summary>
                /// 检测数据
                /// </summary>
                /// <param name="value">数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Check(DateTime value)
                {
                    Check((ulong)value.Ticks);
                }
                /// <summary>
                /// 检测数据
                /// </summary>
                /// <param name="value">数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Check(long value)
                {
                    Check((ulong)value);
                }
                /// <summary>
                /// 检测数据
                /// </summary>
                /// <param name="value">数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Check(ulong value)
                {
                    Check((uint)value);
                    Check((uint)(value >> 32));
                }
                /// <summary>
                /// 检测数据
                /// </summary>
                /// <param name="value">数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Check(int value)
                {
                    Check((uint)value);
                }
                /// <summary>
                /// 检测数据
                /// </summary>
                /// <param name="value">数据</param>
                public void Check(uint value)
                {
                    if (Data != null)
                    {
                        if (*Data == (value & 0x3fU) + startChar
                            && Data[1] == ((value >> 6) & 0x3fU) + startChar
                            && Data[2] == ((value >> 12) & 0x3fU) + startChar
                            && Data[3] == ((value >> 18) & 0x3fU) + startChar
                            && Data[4] == ((value >> 24) & 0x3fU) + startChar
                            && Data[5] == (value >> 30) + startChar)
                        {
                            Data += 6;
                        }
                        else Data = null;
                    }
                }
                /// <summary>
                /// 检测数据
                /// </summary>
                /// <param name="value">数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Check(short value)
                {
                    Check((ushort)value);
                }
                /// <summary>
                /// 检测数据
                /// </summary>
                /// <param name="value">数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Check(char value)
                {
                    Check((ushort)value);
                }
                /// <summary>
                /// 检测数据
                /// </summary>
                /// <param name="value">数据</param>
                public void Check(ushort value)
                {
                    if (Data != null)
                    {
                        if (*Data == (value & 0x3fU) + startChar
                            && Data[1] == ((value >> 6) & 0x3fU) + startChar
                            && Data[2] == (value >> 12) + startChar)
                        {
                            Data += 3;
                        }
                        else Data = null;
                    }
                }
                /// <summary>
                /// 检测数据
                /// </summary>
                /// <param name="value">数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Check(sbyte value)
                {
                    Check((byte)value);
                }
                /// <summary>
                /// 检测数据
                /// </summary>
                /// <param name="value">数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Check(byte value)
                {
                    if (Data != null)
                    {
                        if (*Data == (value & 0x3fU) + startChar
                            && Data[1] == (value >> 6) + startChar)
                        {
                            Data += 2;
                        }
                        else Data = null;
                    }
                }
                /// <summary>
                /// 检测数据
                /// </summary>
                /// <param name="value">数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Check(bool value)
                {
                    if (Data != null)
                    {
                        if (*Data == (value ? (byte)'1' : (byte)'0')) ++Data;
                        else Data = null;
                    }
                }
                /// <summary>
                /// 检测数据
                /// </summary>
                /// <param name="value">数据</param>
                public void Check(string value)
                {
                    if (Data != null && value != null)
                    {
                        foreach (char code in value)
                        {
                            Check((ushort)code);
                            if (Data == null) break;
                        }
                    }
                }
                /// <summary>
                /// 检测数据结束
                /// </summary>
                /// <returns>当前数据位置</returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public byte* Check()
                {
                    if (Data != null) page.notChanged304();
                    return Data;
                }
                /// <summary>
                /// 设置数据
                /// </summary>
                /// <returns>缓存数据</returns>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public byte[] Set()
                {
                    byte[] eTag = new byte[length];
                    page.Response.SetETag(eTag);
                    return eTag;
                }
                /// <summary>
                /// 设置数据
                /// </summary>
                /// <param name="data">数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void SetData(byte* data)
                {
                    this.Data = data;
                    if (isServerVersion) Set(page.DomainServer.WebConfig.ETagVersion);
                    if (isPageVersion) Set(page.ETagVersion);
                }
                /// <summary>
                /// 设置数据
                /// </summary>
                /// <param name="value">数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Set(DateTime value)
                {
                    Set((ulong)value.Ticks);
                }
                /// <summary>
                /// 设置数据
                /// </summary>
                /// <param name="value">数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Set(long value)
                {
                    Set((ulong)value);
                }
                /// <summary>
                /// 设置数据
                /// </summary>
                /// <param name="value">数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Set(ulong value)
                {
                    Set((uint)value);
                    Set((uint)(value >> 32));
                }
                /// <summary>
                /// 设置数据
                /// </summary>
                /// <param name="value">数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Set(int value)
                {
                    Set((uint)value);
                }
                /// <summary>
                /// 设置数据
                /// </summary>
                /// <param name="value">数据</param>
                public void Set(uint value)
                {
                    *Data = (byte)((value & 0x3fU) + startChar);
                    Data[1] = (byte)(((value >> 6) & 0x3fU) + startChar);
                    Data[2] = (byte)(((value >> 12) & 0x3fU) + startChar);
                    Data[3] = (byte)(((value >> 18) & 0x3fU) + startChar);
                    Data[4] = (byte)(((value >> 24) & 0x3fU) + startChar);
                    Data[5] = (byte)((value >> 30) + startChar);
                    Data += 6;
                }
                /// <summary>
                /// 设置数据
                /// </summary>
                /// <param name="value">数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Set(short value)
                {
                    Set((ushort)value);
                }
                /// <summary>
                /// 设置数据
                /// </summary>
                /// <param name="value">数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Set(char value)
                {
                    Set((ushort)value);
                }
                /// <summary>
                /// 设置数据
                /// </summary>
                /// <param name="value">数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Set(ushort value)
                {
                    *Data = (byte)((value & 0x3fU) + startChar);
                    Data[1] = (byte)(((value >> 6) & 0x3fU) + startChar);
                    Data[2] = (byte)((value >> 12) + startChar);
                    Data += 3;
                }
                /// <summary>
                /// 设置数据
                /// </summary>
                /// <param name="value">数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Set(sbyte value)
                {
                    Set((byte)value);
                }
                /// <summary>
                /// 设置数据
                /// </summary>
                /// <param name="value">数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Set(byte value)
                {
                    *Data = (byte)((value & 0x3fU) + startChar);
                    Data[1] = (byte)((value >> 6) + startChar);
                    Data += 2;
                }
                /// <summary>
                /// 设置数据
                /// </summary>
                /// <param name="value">数据</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Set(bool value)
                {
                    *Data++ = value ? (byte)'1' : (byte)'0';
                }
                /// <summary>
                /// 设置数据
                /// </summary>
                /// <param name="value">数据</param>
                public void Set(string value)
                {
                    if (value != null)
                    {
                        foreach (char code in value) Set((ushort)code);
                    }
                }
            }
            /// <summary>
            /// 默认重定向路径
            /// </summary>
            private static readonly byte[] locationPath = new byte[] { (byte)'/' };
            /// <summary>
            /// Session名称
            /// </summary>
            private static readonly byte[] sessionName = fastCSharp.config.http.Default.SessionName.getBytes();
            /// <summary>
            /// 输出编码
            /// </summary>
            protected Encoding responseEncoding;
            /// <summary>
            /// HTTP请求表单
            /// </summary>
            protected internal requestForm form;
            /// <summary>
            /// 未知数据流解析转换类型
            /// </summary>
            protected internal virtual requestHeader.postType dataToType { get { return net.tcp.http.requestHeader.postType.Json; } }
            /// <summary>
            /// 会话标识
            /// </summary>
            private sessionId sessionId;
            /// <summary>
            /// HTTP响应输出
            /// </summary>
            public response Response;
            /// <summary>
            /// HTTP响应输出标识(用于终止同步ajax输出)
            /// </summary>
            public int ResponseIdentity { get; private set; }
            /// <summary>
            /// 是否异步调用
            /// </summary>
            public bool IsAsynchronous { get; internal set; }
            /// <summary>
            /// 异步调用标识
            /// </summary>
            protected int asynchronousIdentity;
            /// <summary>
            /// 客户端缓存版本号
            /// </summary>
            public virtual int ETagVersion
            {
                get { return 0; }
            }
            /// <summary>
            /// 是否支持压缩
            /// </summary>
            protected virtual bool isGZip { get { return true; } }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                Response = null;
                requestHeader requestHeader = this.requestHeader;
                this.requestHeader = null;
                socketBase socket = this.socket;
                this.socket = null;
                if (socket != null && requestHeader != null)
                {
                    socket.ResponseError(SocketIdentity, net.tcp.http.response.state.ServerError500);
                }
            }
            /// <summary>
            /// HTTP请求头部处理
            /// </summary>
            /// <param name="socketIdentity">套接字操作编号</param>
            /// <param name="request">HTTP请求头部</param>
            /// <param name="isPool">是否使用WEB页面池</param>
            /// <returns>是否成功</returns>
            internal virtual bool LoadHeader(long socketIdentity, fastCSharp.net.tcp.http.requestHeader request, ref bool isPool) { return false; }
            /// <summary>
            /// 加载查询参数
            /// </summary>
            /// <param name="form">HTTP请求表单</param>
            /// <param name="isAjax">是否ajax请求</param>
            /// <returns>是否成功</returns>
            internal virtual void Load(fastCSharp.net.tcp.http.requestForm form, bool isAjax)
            {
                log.Error.Throw(log.exceptionType.ErrorOperation);
            }
            /// <summary>
            /// 加载AJAX视图
            /// </summary>
            /// <param name="socketIdentity">套接字操作编号</param>
            /// <param name="socket"></param>
            /// <param name="domainServer"></param>
            /// <param name="request">HTTP请求头部</param>
            /// <param name="form">HTTP请求表单</param>
            /// <param name="isPool">是否使用WEB页面池</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void LoadAjaxView(long socketIdentity, socketBase socket, domainServer domainServer, fastCSharp.net.tcp.http.requestHeader request, fastCSharp.net.tcp.http.requestForm form, bool isPool)
            {
                Socket = socket;
                DomainServer = domainServer;
                if (LoadHeader(socketIdentity, request, ref isPool)) Load(form, true);
                else socket.ResponseError(socketIdentity, fastCSharp.net.tcp.http.response.state.ServerError500);
            }
            /// <summary>
            /// HTTP头部请求处理获取HTTP响应
            /// </summary>
            /// <param name="socketIdentity">套接字操作编号</param>
            /// <param name="socket"></param>
            /// <param name="domainServer"></param>
            /// <param name="request">HTTP请求头部</param>
            /// <param name="form">HTTP请求表单</param>
            /// <param name="isPool">是否使用WEB页面池</param>
            /// <returns></returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal fastCSharp.net.tcp.http.response GetLoadResponse(long socketIdentity, socketBase socket, domainServer domainServer, fastCSharp.net.tcp.http.requestHeader request, fastCSharp.net.tcp.http.requestForm form, bool isPool)
            {
                Socket = socket;
                DomainServer = domainServer;
                if (LoadHeader(socketIdentity, request, ref isPool))
                {
                    this.form = form;
                    return Response = fastCSharp.net.tcp.http.response.Get();
                }
                return null;
            }
            /// <summary>
            /// 清除当前请求数据
            /// </summary>
            protected virtual void clear()
            {
                ++ResponseIdentity;
                if (IsAsynchronous)
                {
                    IsAsynchronous = false;
                    ++asynchronousIdentity;
                    fastCSharp.net.tcp.http.response.Push(ref Response);
                }
                else Response = null;
#if NOJIT
#else
                requestHeader = null;
                socket = null;
                domainServer = null;
                form = null;
                sessionId.Null();
#endif
            }
            /// <summary>
            /// 解析web调用参数
            /// </summary>
            /// <typeparam name="parameterType">web调用参数类型</typeparam>
            /// <param name="parameter">web调用参数</param>
            /// <returns>是否成功</returns>
            public bool ParseParameter<parameterType>(ref parameterType parameter) where parameterType : struct
            {
                switch (requestHeader.PostType)
                {
                    case fastCSharp.net.tcp.http.requestHeader.postType.Json:
                        return Socket.ParseForm() && fastCSharp.emit.jsonParser.Parse(form.Text, ref parameter, jsonParserConfig);
                    case fastCSharp.net.tcp.http.requestHeader.postType.Xml:
                        return Socket.ParseForm() && fastCSharp.emit.xmlParser.Parse(form.Text, ref parameter, xmlParserConfig);
                    case fastCSharp.net.tcp.http.requestHeader.postType.Data:
                        return Socket.ParseForm(dataToType) && parseParameter(ref parameter);
                    case fastCSharp.net.tcp.http.requestHeader.postType.Form:
                        return Socket.ParseForm() && parseParameter(ref parameter);
                    case fastCSharp.net.tcp.http.requestHeader.postType.FormData:
                        return parseParameter(ref parameter);
                    default:
                        return parseParameterQuery(ref parameter);
                }
            }
            /// <summary>
            /// 解析web调用参数
            /// </summary>
            /// <typeparam name="parameterType">web调用参数类型</typeparam>
            /// <param name="parameter">web调用参数</param>
            /// <returns>是否成功</returns>
            private bool parseParameter<parameterType>(ref parameterType parameter) where parameterType : struct
            {
                switch (form.TextQueryChar)
                {
                    case fastCSharp.config.web.QueryJsonName:
                        return fastCSharp.emit.jsonParser.Parse(form.Text, ref parameter, jsonParserConfig);
                    case fastCSharp.config.web.QueryXmlName:
                        return fastCSharp.emit.xmlParser.Parse(form.Text, ref parameter, xmlParserConfig);
                    default:
                        return parseParameterQuery(ref parameter);
                }
            }
            /// <summary>
            /// 解析web调用参数
            /// </summary>
            /// <typeparam name="parameterType">web调用参数类型</typeparam>
            /// <param name="parameter">web调用参数</param>
            /// <returns>是否成功</returns>
            public bool ParseParameterAny<parameterType>(ref parameterType parameter)
            {
                switch (requestHeader.PostType)
                {
                    case fastCSharp.net.tcp.http.requestHeader.postType.Json:
                        return Socket.ParseForm() && fastCSharp.emit.jsonParser.Parse(form.Text, ref parameter, jsonParserConfig);
                    case fastCSharp.net.tcp.http.requestHeader.postType.Xml:
                        return Socket.ParseForm() && fastCSharp.emit.xmlParser.Parse(form.Text, ref parameter, xmlParserConfig);
                    case fastCSharp.net.tcp.http.requestHeader.postType.Data:
                        return Socket.ParseForm(dataToType) && parseParameterAny(ref parameter);
                    case fastCSharp.net.tcp.http.requestHeader.postType.Form:
                        return Socket.ParseForm() && parseParameterAny(ref parameter);
                    case fastCSharp.net.tcp.http.requestHeader.postType.FormData:
                        return parseParameterAny(ref parameter);
                    default:
                        return parseParameterQueryAny(ref parameter);
                }
            }
            /// <summary>
            /// 解析web调用参数
            /// </summary>
            /// <typeparam name="parameterType">web调用参数类型</typeparam>
            /// <param name="parameter">web调用参数</param>
            /// <returns>是否成功</returns>
            private bool parseParameterAny<parameterType>(ref parameterType parameter)
            {
                switch (form.TextQueryChar)
                {
                    case fastCSharp.config.web.QueryJsonName:
                        return fastCSharp.emit.jsonParser.Parse(form.Text, ref parameter, jsonParserConfig);
                    case fastCSharp.config.web.QueryXmlName:
                        return fastCSharp.emit.xmlParser.Parse(form.Text, ref parameter, xmlParserConfig);
                    default:
                        return parseParameterQueryAny(ref parameter);
                }
            }
            /// <summary>
            /// 设置为异步调用模式(回调中需要cancelAsynchronous)
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected internal void setAsynchronous()
            {
                IsAsynchronous = true;
            }
            /// <summary>
            /// 取消异步调用模式，获取HTTP响应
            /// </summary>
            /// <returns>HTTP响应</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected internal response cancelAsynchronous()
            {
                if (IsAsynchronous)
                {
                    ++asynchronousIdentity;
                    IsAsynchronous = false;
                    return Response;
                }
                return null;
            }
            /// <summary>
            /// WEB页面回收
            /// </summary>
            internal abstract void PushPool();
            /// <summary>
            /// 重定向
            /// </summary>
            /// <param name="path">重定向地址</param>
            /// <param name="is302">是否临时重定向</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected void location(string path, bool is302 = true)
            {
                location(string.IsNullOrEmpty(path) ? null : path.getBytes(), is302);
            }
            /// <summary>
            /// 重定向
            /// </summary>
            /// <param name="path">重定向地址</param>
            /// <param name="is302">是否临时重定向</param>
            protected void location(byte[] path, bool is302 = true)
            {
                if (requestHeader != null)
                {
                    response response = Response ?? response.Get();
                    Response = null;
                    response.State = is302 ? response.state.Found302 : response.state.MovedPermanently301;
                    if (path == null) path = locationPath;
                    response.Location.UnsafeSet(path, 0, path.Length);
                    if (socket.Response(SocketIdentity, ref response)) PushPool();
                }
            }
            /// <summary>
            /// 资源未修改
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected void notChanged304()
            {
                if (socket.Response(SocketIdentity, fastCSharp.net.tcp.http.response.NotChanged304)) PushPool();
            }
            /// <summary>
            /// 服务器发生不可预期的错误
            /// </summary>
            protected internal void serverError500()
            {
                if (socket.ResponseError(SocketIdentity, net.tcp.http.response.state.ServerError500)) PushPool();
            }
            /// <summary>
            /// 请求资源不存在
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected void NotFound404()
            {
                if (socket.ResponseError(SocketIdentity, net.tcp.http.response.state.NotFound404)) PushPool();
            }
            /// <summary>
            /// 搜索引擎404提示
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected void SearchEngineNotFound404()
            {
                if (socket.ResponseSearchEngine404(SocketIdentity)) PushPool();
            }
            /// <summary>
            /// 任意客户端缓存有效标识处理
            /// </summary>
            /// <returns>是否需要继续加载</returns>
            protected bool anyIfNoneMatch()
            {
                if (requestHeader.IfNoneMatch.length == 0)
                {
                    Response.SetETag(locationPath);
                    return true;
                }
                notChanged304();
                return false;
            }
            /// <summary>
            /// 客户端缓存有效标识匹配处理
            /// </summary>
            /// <param name="eTag">客户端缓存有效标识</param>
            /// <returns>是否需要继续加载</returns>
            protected unsafe bool loadIfNoneMatch(byte[] eTag)
            {
                if (eTag != null)
                {
                    subArray<byte> ifNoneMatch = requestHeader.IfNoneMatch;
                    if (ifNoneMatch.length == eTag.Length)
                    {
                        fixed (byte* eTagFixed = eTag, ifNoneMatchFixed = ifNoneMatch.array)
                        {
                            if (unsafer.memory.SimpleEqual(eTag, ifNoneMatchFixed + ifNoneMatch.startIndex, eTag.Length))
                            {
                                notChanged304();
                                return false;
                            }
                        }
                    }
                    Response.SetETag(eTag);
                }
                return true;
            }
            /// <summary>
            /// 输出数据
            /// </summary>
            /// <param name="data"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected void response(byte[] data)
            {
                Response.BodyStream.Write(data);
            }
            /// <summary>
            /// 输出数据
            /// </summary>
            /// <param name="data"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected void response(subArray<byte> data)
            {
                Response.BodyStream.Write(ref data);
            }
            /// <summary>
            /// 输出数据
            /// </summary>
            /// <param name="data"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected void response(ref subArray<byte> data)
            {
                Response.BodyStream.Write(ref data);
            }
            /// <summary>
            /// 输出HTML片段
            /// </summary>
            /// <param name="html">HTML片段</param>
            protected unsafe void response(char html)
            {
                unmanagedStream bodyStream = Response.BodyStream;
                if (responseEncoding.CodePage == Encoding.Unicode.CodePage) bodyStream.Write(html);
                else
                {
                    int count = responseEncoding.GetByteCount(&html, 1);
                    bodyStream.PrepLength(count);
                    responseEncoding.GetBytes(&html, 1, bodyStream.CurrentData, count);
                    bodyStream.UnsafeAddLength(count);
                }
            }
            /// <summary>
            /// 输出字符串数据
            /// </summary>
            /// <param name="value">字符串</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected void response(subString value)
            {
                response(ref value);
            }
            /// <summary>
            /// 输出字符串数据
            /// </summary>
            /// <param name="value">字符串</param>
            protected unsafe void response(ref subString value)
            {
                if (value.Length != 0)
                {
                    unmanagedStream bodyStream = Response.BodyStream;
                    fixed (char* valueFixed = value.value)
                    {
                        char* valueStart = valueFixed + value.StartIndex;
                        if (responseEncoding.CodePage == Encoding.Unicode.CodePage)
                        {
                            int count = value.Length << 1;
                            bodyStream.PrepLength(count);
                            unsafer.memory.Copy(valueStart, bodyStream.CurrentData, count);
                            bodyStream.UnsafeAddLength(count);
                        }
                        else
                        {
                            int count = responseEncoding.GetByteCount(valueStart, value.Length);
                            bodyStream.PrepLength(count);
                            responseEncoding.GetBytes(valueStart, value.Length, bodyStream.CurrentData, count);
                            bodyStream.UnsafeAddLength(count);
                        }
                    }
                }
            }
            /// <summary>
            /// 输出HTML片段
            /// </summary>
            /// <param name="html">HTML片段</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected void response(string html)
            {
                this.response((subString)html);
            }
            /// <summary>
            /// 输出结束
            /// </summary>
            /// <param name="response">HTTP响应输出</param>
            /// <returns>是否操作成功</returns>
            protected unsafe bool responseEnd(ref response response)
            {
                long identity = SocketIdentity;
                try
                {
                    byte[] buffer = response.Body.array;
                    int length = response.BodyStream.Length;
                    if (requestHeader.IsRange && !requestHeader.FormatRange(length))
                    {
                        if (socket.ResponseError(SocketIdentity, response.state.RangeNotSatisfiable416))
                        {
                            PushPool();
                            return true;
                        }
                    }
                    else
                    {
                        if (buffer.Length >= length)
                        {
                            unsafer.memory.Copy(response.BodyStream.data.data, buffer, length);
                            response.Body.UnsafeSet(0, length);
                        }
                        else
                        {
                            byte[] data = response.BodyStream.GetArray();
                            response.Body.UnsafeSet(data, 0, data.Length);
                            response.Buffer = buffer;
                        }
                        response.State = response.state.Ok200;
                        if (response.ContentType == null) response.ContentType = DomainServer.HtmlContentType;
                        if (requestHeader.IsGZip && isGZip)
                        {
                            if (isGZip)
                            {
                                if (!requestHeader.IsRange)
                                {
                                    subArray<byte> compressData = response.GetCompress(ref response.Body, fastCSharp.memoryPool.StreamBuffers);
                                    if (compressData.array != null)
                                    {
                                        Buffer.BlockCopy(compressData.array, 0, response.Body.array, 0, compressData.length);
                                        response.Body.UnsafeSet(0, compressData.length);
                                        response.ContentEncoding = response.GZipEncoding;
                                        fastCSharp.memoryPool.StreamBuffers.PushNotNull(compressData.array);
                                    }
                                }
                            }
                            else requestHeader.IsGZip = false;
                        }
                        response.NoStore();
                        if (socket.Response(identity, ref response))
                        {
                            PushPool();
                            return true;
                        }
                    }
                }
                finally { response.Push(ref response); }
                return false;
            }
            /// <summary>
            /// 初始化请求会话标识
            /// </summary>
            private void setSessionId()
            {
                if (sessionId.CookieValue == 0)
                {
                    subArray<byte> cookie = default(subArray<byte>);
                    requestHeader.GetCookie(sessionName, ref cookie);
                    sessionId.FromCookie(ref cookie);
                }
            }
            /// <summary>
            /// 获取Session值
            /// </summary>
            /// <typeparam name="valueType">值类型</typeparam>
            /// <returns>Session值</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public valueType GetSession<valueType>() where valueType : class
            {
                ISession session = socket.Session;
                if (session != null)
                {
                    setSessionId();
                    if (sessionId.Ticks != 0)
                    {
#if NOJIT
                        return session.Get(ref sessionId, null) as valueType;
#else
                        ISession<valueType> sessionType = session as ISession<valueType>;
                        return sessionType == null ? session.Get(ref sessionId, null) as valueType : sessionType.Get(ref sessionId, null);
#endif
                    }
                }
                return null;
            }
            /// <summary>
            /// 获取Session值
            /// </summary>
            /// <typeparam name="valueType">值类型</typeparam>
            /// <param name="nullValue">默认空值</param>
            /// <returns>Session值</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public valueType GetSession<valueType>(valueType nullValue) where valueType : struct
            {
                ISession session = socket.Session;
                if (session != null)
                {
                    setSessionId();
                    if (sessionId.Ticks != 0)
                    {
#if NOJIT
                        return (valueType)session.Get(ref sessionId, null);
#else
                        ISession<valueType> sessionType = session as ISession<valueType>;
                        return sessionType == null ? (valueType)session.Get(ref sessionId, null) : sessionType.Get(ref sessionId, nullValue);
#endif
                    }
                }
                return nullValue;
            }
            /// <summary>
            /// 获取Session值
            /// </summary>
            /// <returns>Session值</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private object getSession()
            {
                ISession session = socket.Session;
                if (session != null)
                {
                    setSessionId();
                    if (sessionId.Ticks != 0) return session.Get(ref sessionId, null);
                }
                return null;
            }
            /// <summary>
            /// 设置Session值
            /// </summary>
            /// <param name="value">值</param>
            /// <typeparam name="valueType">值类型</typeparam>
            /// <returns>是否设置成功</returns>
            public bool SetSession<valueType>(valueType value)
            {
                if (Response == null)
                {
                    fastCSharp.log.Error.Add("找不到HTTP响应输出", null, false);
                    return false;
                }
                ISession session = socket.Session;
                if (session != null)
                {
                    setSessionId();
#if NOJIT
                    if (session.Set(ref sessionId, value))
#else
                    ISession<valueType> sessionType = session as ISession<valueType>;
                    if (sessionType == null ? session.Set(ref sessionId, value) : sessionType.Set(ref sessionId, value))
#endif
                    {
                        Response.Cookies.Add(new cookie(sessionName, sessionId.ToCookie(), DateTime.MinValue, requestHeader.Host, locationPath, false, true));
                    }
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 设置Session值
            /// </summary>
            /// <param name="value">值</param>
            /// <returns>是否设置成功</returns>
            public bool SetSession(object value)
            {
                ISession session = socket.Session;
                if (session != null)
                {
                    setSessionId();
                    if (session.Set(ref sessionId, value))
                    {
                        Response.Cookies.Add(new cookie(sessionName, sessionId.ToCookie(), DateTime.MinValue, requestHeader.Host, locationPath, false, true));
                    }
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 删除Session
            /// </summary>
            public void RemoveSession()
            {
                ISession session = socket.Session;
                if (session != null)
                {
                    setSessionId();
                    if (sessionId.Ticks != 0)
                    {
                        session.Remove(ref sessionId);
                        Response.Cookies.Add(new cookie(sessionName, nullValue<byte>.Array, pub.MinTime, requestHeader.Host, locationPath, false, true));
                    }
                }
            }
            /// <summary>
            /// 设置Cookie
            /// </summary>
            /// <param name="cookie">Cookie</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void SetCookie(cookie cookie)
            {
                if (cookie != null && cookie.Name != null) Response.Cookies.Add(cookie);
            }
            /// <summary>
            /// 获取Cookie
            /// </summary>
            /// <param name="name">名称</param>
            /// <returns>值</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public string GetCookie(string name)
            {
                return requestHeader.GetCookie(name);
            }
            /// <summary>
            /// 获取Cookie
            /// </summary>
            /// <param name="name">名称</param>
            /// <returns>值</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public string GetCookie(byte[] name)
            {
                return requestHeader.GetCookieString(name);
            }
            /// <summary>
            /// 判断是否存在Cookie值
            /// </summary>
            /// <param name="name">名称</param>
            /// <returns>是否存在Cookie值</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public bool IsCookie(byte[] name)
            {
                return requestHeader.IsCookie(name);
            }
            ///// <summary>
            ///// 获取查询整数值
            ///// </summary>
            ///// <param name="name"></param>
            ///// <param name="nullValue"></param>
            ///// <returns></returns>
            //public int GetQueryInt(byte[] name, int nullValue)
            //{
            //    return requestHeader.GetQueryInt(name, nullValue);
            //}
            /// <summary>
            /// 设置内容类型
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void JsContentType()
            {
                Response.SetJsContentType(DomainServer);
            }
        }
        /// <summary>
        /// WEB 调用函数名称，用于替换默认的函数名称。
        /// </summary>
        public string MethodName;
        /// <summary>
        /// HTTP Body 接收数据的最大字节数（单位:MB），默认为 4MB。
        /// </summary>
        public int MaxPostDataSize = fastCSharp.config.http.Default.MaxPostDataSize;
        /// <summary>
        /// 接收 HTTP Body 数据内存缓冲区的最大字节数(单位:KB)，默认为 64KB，超出限定则使用文件储存方式。
        /// </summary>
        public int MaxMemoryStreamSize = fastCSharp.config.http.Default.MaxMemoryStreamSize;
        /// <summary>
        /// 默认为 false 表示每次请求都需要 new 一个页面对象；否则在页面对象使用完以后会添加到 WEB 页面对象池中以便重复使用，记得重载 void clear() 处理需要清除的字段数据，或者给需要处理的字段数据添加申明[fastCSharp.emit.webView.clearMember]。
        /// </summary>
        public bool IsPool;
    }
}
