using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using fastCSharp.web;
using fastCSharp.net.tcp.http;
using fastCSharp.threading;
using fastCSharp.reflection;
using System.Net;
using System.Threading;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace fastCSharp.code.cSharp
{
    /// <summary>
    /// WEB视图配置
    /// </summary>
    public sealed partial class webView : webPage
    {
        /// <summary>
        /// 查询字段配置
        /// </summary>
        public sealed class query : ignoreMember { }
        /// <summary>
        /// 客户端视图绑定类型
        /// </summary>
        public sealed class clientType : Attribute
        {
            /// <summary>
            /// 默认绑定成员名称
            /// </summary>
            public const string DefaultMemberName = "Id";
            /// <summary>
            /// 默认客户端视图绑定类型
            /// </summary>
            internal static readonly clientType Null = new clientType();
            /// <summary>
            /// 客户端视图绑定类型名称（PrefixName 为 null 时有效）
            /// </summary>
            public string Name;
            /// <summary>
            /// 客户端视图绑定类型前缀
            /// </summary>
            public string PrefixName;
            /// <summary>
            /// 绑定成员名称,默认为Id
            /// </summary>
            public string MemberName = DefaultMemberName;
            /// <summary>
            /// 客户端视图绑定类型名称
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            internal string GetClientName(Type type)
            {
                if (PrefixName == null) return Name;
                return PrefixName + type.Name;
            }
        }
        /// <summary>
        /// Ajax视图输出参数
        /// </summary>
        public sealed class outputAjax : ignoreMember
        {
            /// <summary>
            /// 默认Ajax视图输出参数
            /// </summary>
            internal static readonly outputAjax Null = new outputAjax();
            /// <summary>
            /// 输出绑定名称
            /// </summary>
            public string BindingName;
            /// <summary>
            /// 是否忽略null值输出
            /// </summary>
            public bool IsIgnoreNull;
        }
        /// <summary>
        /// 默认空WEB视图配置
        /// </summary>
        internal static readonly webView Null = new webView();
        /// <summary>
        /// WEB视图接口
        /// </summary>
        public interface IWebView : webPage.IWebPage
        {
            /// <summary>
            /// 最大接收数据字节数
            /// </summary>
            int MaxPostDataSize { get; }
            /// <summary>
            /// 根据HTTP请求表单值获取内存流最大字节数
            /// </summary>
            /// <param name="value">HTTP请求表单值</param>
            /// <returns>内存流最大字节数</returns>
            int MaxMemoryStreamSize(ref fastCSharp.net.tcp.http.requestForm.value value);
        }
        /// <summary>
        /// #!转换URL
        /// </summary>
        public struct hashUrl
        {
            /// <summary>
            /// URL路径
            /// </summary>
            public string Path;
            /// <summary>
            /// URL查询
            /// </summary>
            public string Query;
            /// <summary>
            /// 对象转换成JSON字符串
            /// </summary>
            /// <param name="jsonStream">JSON输出流</param>
            public void ToJson(charStream jsonStream)
            {
                int queryLength = Query.length();
                jsonStream.PrepLength(Path.length() + queryLength + 4);
                jsonStream.UnsafeWrite(fastCSharp.web.ajax.Quote);
                jsonStream.Write(Path);
                if (queryLength != 0)
                {
                    jsonStream.UnsafeWrite('#');
                    jsonStream.UnsafeWrite('!');
                    jsonStream.WriteNotNull(Query);
                }
                jsonStream.UnsafeWrite(fastCSharp.web.ajax.Quote);
            }
            /// <summary>
            /// 转换成?查询字符串
            /// </summary>
            /// <returns></returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public string ToQueryString()
            {
                if (Query == null) return Path;
                return Path + "?" + Query;
            }
            /// <summary>
            /// 转换成?查询字符串
            /// </summary>
            /// <returns></returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public string ToPageString()
            {
                if (Query == null) return Path + "?";
                return Path + "?" + Query + "&";
            }
            /// <summary>
            /// 转换成字符串
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                if (Query == null) return Path;
                return Path + "#!" + Query;
            }
        }
        /// <summary>
        /// 视图错误重定向路径
        /// </summary>
        public struct errorPath
        {
            /// <summary>
            /// 错误重定向路径
            /// </summary>
            public string ErrorPath;
            /// <summary>
            /// 返回路径
            /// </summary>
            public string ReturnPath;
            /// <summary>
            /// 重定向路径
            /// </summary>
            public string LocationPath;
        }
        /// <summary>
        /// WEB页面视图
        /// </summary>
        [fastCSharp.emit.webView.clearMember(IsIgnoreCurrent = true)]
        public abstract class view : webPage.page
        {
            /// <summary>
            /// 服务器端时间
            /// </summary>
            [fastCSharp.code.cSharp.webView.clientType(Name = "fastCSharp.ServerTime", MemberName = null)]
            protected struct time
            {
                /// <summary>
                /// 当前时间
                /// </summary>
                [fastCSharp.code.cSharp.webView.outputAjax]
                public DateTime Now;
            }
            ///// <summary>
            ///// URL查询字符位图
            ///// </summary>
            //private static readonly fastCSharp.String.asciiMap urlQueryMap = new fastCSharp.String.asciiMap(@"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789:/.&=_-!@$%^*()+{}|""<>[]\;',", true);
            /// <summary>
            /// AJAX回调函数开始符
            /// </summary>
            private static readonly byte[] callBackStart = fastCSharp.config.appSetting.Encoding.GetBytes("(");
            /// <summary>
            /// AJAX回调函数结束符
            /// </summary>
            private static readonly byte[] callBackEnd = fastCSharp.config.appSetting.Encoding.GetBytes(")");
            /// <summary>
            /// URL查询符
            /// </summary>
            private static readonly byte[] urlQuery = fastCSharp.config.appSetting.Encoding.GetBytes("?");
            /// <summary>
            /// URL HASH查询符
            /// </summary>
            private static readonly byte[] urlHash = fastCSharp.config.appSetting.Encoding.GetBytes("#!");
            /// <summary>
            /// AJAX调用名称
            /// </summary>
            private static readonly byte[] ajaxWebCallName = fastCSharp.config.web.Default.AjaxWebCallName.getBytes();
            /// <summary>
            /// URL中的#!是否需要转换成?
            /// </summary>
            protected bool isHashQueryUri;
            /// <summary>
            /// 请求视图路径
            /// </summary>
            public string ViewPath;
            /// <summary>
            /// 页面关键字
            /// </summary>
            [fastCSharp.code.ignore]
            public string ViewKeywords { get; protected set; }
            /// <summary>
            /// 页面描述
            /// </summary>
            [fastCSharp.code.ignore]
            public string ViewDescription { get; protected set; }
            /// <summary>
            /// 最大接收数据字节数
            /// </summary>
            public virtual int MaxPostDataSize { get { return fastCSharp.config.http.Default.MaxPostDataSize << 20; } }
            /// <summary>
            /// WEB视图配置
            /// </summary>
            protected virtual webView webView { get { return Null; } }
            /// <summary>
            /// 临时逻辑变量
            /// </summary>
            protected bool _if_;
            /// <summary>
            /// 当前循环索引
            /// </summary>
            protected int _loopIndex_;
            /// <summary>
            /// 当前循环数量
            /// </summary>
            protected int _loopCount_;
            /// <summary>
            /// 是否ajax请求
            /// </summary>
            protected bool isAjax;
            /// <summary>
            /// JSON序列化是否使用默认模式(非视图模式模式)
            /// </summary>
            protected virtual bool isDefaultToJson
            {
                get { return false; }
            }
            /// <summary>
            /// 当前时间
            /// </summary>
            protected time serverTime
            {
                get { return new time { Now = date.Now }; }
            }
#if NOJIT
#else
            /// <summary>
            /// 清除当前请求数据
            /// </summary>
            protected override void clear()
            {
                base.clear();
                isHashQueryUri = isAjax = false;
                ViewKeywords = ViewDescription = ViewPath = null;
            }
#endif
            /// <summary>
            /// HTTP请求头部处理
            /// </summary>
            /// <returns>是否成功</returns>
            protected unsafe virtual bool loadHeader()
            {
                subArray<byte> path = requestHeader.Path;
                if (path.length == ajaxWebCallName.Length)
                {
                    fixed (byte* pathFixed = path.array)
                    {
                        if (unsafer.memory.SimpleEqual(ajaxWebCallName, pathFixed + path.startIndex, path.length)) path = requestHeader.AjaxCallName;
                    }
                }
                if (path.length == 0) ViewPath = string.Empty;
                else
                {
                    fixed (byte* pathFixed = path.array)
                    {
                        ViewPath = String.UnsafeDeSerialize(pathFixed + path.startIndex, -path.length);
                    }
                }
                return true;
            }
            /// <summary>
            /// WEB视图加载
            /// </summary>
            /// <returns>是否成功</returns>
            protected virtual bool loadView()
            {
                return true;
            }
            /// <summary>
            /// 输出HTML片段
            /// </summary>
            /// <param name="url">URL</param>
            protected void response(hashUrl url)
            {
                this.response(url.Path);
                if (!string.IsNullOrEmpty(url.Query))
                {
                    responseUrlQueryHash();
                    this.response(url.Query);
                }
            }
            /// <summary>
            /// 输出URL HASH查询符
            /// </summary>
            private void responseUrlQueryHash()
            {
                if (responseEncoding.CodePage == Encoding.Unicode.CodePage)
                {
                    if (isHashQueryUri) Response.BodyStream.Write((short)'?');
                    else Response.BodyStream.Write((int)('#' + ('!' << 16)));
                }
                else if (responseEncoding.CodePage == fastCSharp.config.appSetting.Encoding.CodePage)
                {
                    Response.BodyStream.SimpleWriteNotNull(isHashQueryUri ? urlQuery : urlHash);
                }
                else
                {
                    if (isHashQueryUri) Response.BodyStream.Write((byte)'?');
                    else Response.BodyStream.Write((short)('#' + ('!' << 8)));
                }
            }
            /// <summary>
            /// 输出HTML片段
            /// </summary>
            /// <param name="html">HTML片段</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected void responseHtml(subString html)
            {
                responseHtml(ref html);
            }
            /// <summary>
            /// 输出HTML片段
            /// </summary>
            /// <param name="html">HTML片段</param>
            protected void responseHtml(ref subString html)
            {
                if (html.Length != 0)
                {
                    if (responseEncoding.CodePage == Encoding.Unicode.CodePage)
                    {
                        fastCSharp.web.html.HtmlEncoder.ToHtml(ref html, Response.BodyStream);
                    }
                    else
                    {
                        fastCSharp.web.html.HtmlEncoder.ToHtml(ref html);
                        this.response(ref html);
                    }
                }
            }
            /// <summary>
            /// 输出HTML片段
            /// </summary>
            /// <param name="html">HTML片段</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected void responseHtml(string html)
            {
                responseHtml((subString)html);
            }
            /// <summary>
            /// 输出HTML片段
            /// </summary>
            /// <param name="url">URL</param>
            protected void responseHtml(ref hashUrl url)
            {
                responseHtml(url.Path);
                if (!string.IsNullOrEmpty(url.Query))
                {
                    responseUrlQueryHash();
                    responseHtml(url.Query);
                }
            }
            /// <summary>
            /// 输出HTML片段
            /// </summary>
            /// <param name="url">URL</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected void responseHtml(hashUrl url)
            {
                responseHtml(ref url);
            }
            /// <summary>
            /// 输出HTML片段
            /// </summary>
            /// <param name="html">HTML片段</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected void responseTextArea(string html)
            {
                responseHtml((subString)html);
            }
            /// <summary>
            /// 输出HTML片段
            /// </summary>
            /// <param name="html">HTML片段</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected void responseTextArea(subString html)
            {
                responseHtml(ref html);
                //if (html.Length != 0)
                //{
                //    if (responseEncoding.CodePage == Encoding.Unicode.CodePage)
                //    {
                //        fastCSharp.web.html.TextAreaEncoder.ToHtml(html, Response.BodyStream);
                //    }
                //    else this.response(fastCSharp.web.html.TextAreaEncoder.ToHtml(html));
                //}
            }
            /// <summary>
            /// 输出HTML片段
            /// </summary>
            /// <param name="url">URL</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected void responseTextArea(hashUrl url)
            {
                responseHtml(ref url);
                //responseTextArea(url.Path);
                //if (url.Query.length() != 0)
                //{
                //    responseUrlQueryHash();
                //    responseTextArea(url.Query);
                //}
            }
            /// <summary>
            /// 缺少关键查询参数时检测搜索引擎重定向
            /// </summary>
            /// <param name="path">搜索引擎重定向URL</param>
            /// <param name="errorPath">视图错误重定向路径</param>
            protected void checkSearchEngine(string path, webView.errorPath errorPath)
            {
                if (requestHeader.IsSearchEngine)
                {
                    if (path != null) location(path, false);
                    else SearchEngineNotFound404();
                }
                else
                {
                    headerLog();
                    AjaxResponse(errorPath);
                }
            }
            /// <summary>
            /// 浏览器参数名称字符串
            /// </summary>
            private const string userAgentName = @"
userAgent : ";
            /// <summary>
            /// 访问来源名称字符串
            /// </summary>
            private const string refererName = @"
referer : ";
            /// <summary>
            /// 疑似搜索引擎日志
            /// </summary>
            private unsafe void headerLog()
            {
                subArray<byte> uri = requestHeader.Uri;
                fixed (byte* uriFixed = uri.array)
                {
                    byte* start = uriFixed + uri.startIndex, end = fastCSharp.unsafer.memory.FindLast(start, start + uri.length, (byte)'&');
                    if (end != null && *(short*)(end + 1) == 't' + ('=' << 8)) uri = new subArray<byte>(uri.array, uri.startIndex, (int)(end - start));
                    subArray<byte> userAgent = requestHeader.UserAgent, referer = requestHeader.Referer;
                    string content = fastCSharp.String.FastAllocateString(uri.length + userAgentName.Length + userAgent.length + refererName.Length + referer.length);
                    fixed (char* contentFixed = content)
                    {
                        char* write = contentFixed;
                        for (end = start + uri.length; start != end; *write++ = (char)*start++) ;
                        fastCSharp.unsafer.String.Copy(userAgentName, write);
                        write += userAgentName.Length;
                        for (start = uriFixed + userAgent.startIndex, end = start + userAgent.length; start != end; *write++ = (char)*start++) ;
                        fastCSharp.unsafer.String.Copy(refererName, write);
                        write += refererName.Length;
                        for (start = uriFixed + referer.startIndex, end = start + referer.length; start != end; *write++ = (char)*start++) ;
                    }
                    log.Default.Add(content, new System.Diagnostics.StackFrame(), true);
                }
            }
            /// <summary>
            /// AJAX回调输出
            /// </summary>
            /// <returns>是否存在回调参数</returns>
            private unsafe bool responseAjaxCallBack()
            {
                if (requestHeader != null)
                {
                    //if (requestHeader.IsWebSocket)
                    //{
                    //subString callBack = Socket.GetWebSocketCallBack(SocketIdentity);
                    //    if (callBack.Length != 0)
                    //    {
                    //        this.response(ref callBack);
                    //        responseCallBackStart();
                    //        return true;
                    //    }
                    //}
                    //else
                    //{
                        subArray<byte> callBack = requestHeader.AjaxCallBackName;
                        if (callBack.length != 0)
                        {
                            this.response(fastCSharp.web.formQuery.JavascriptUnescape(ref callBack));
                            responseCallBackStart();
                            return true;
                        }
                    //}
                }
                return false;
            }
            /// <summary>
            /// 输出AJAX回调函数开始符
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private void responseCallBackStart()
            {
                if (responseEncoding.CodePage == Encoding.Unicode.CodePage)
                {
                    Response.BodyStream.Write((ushort)'(');
                }
                else if (responseEncoding.CodePage == fastCSharp.config.appSetting.Encoding.CodePage)
                {
                    Response.BodyStream.SimpleWriteNotNull(callBackStart);
                }
                else Response.BodyStream.Write((byte)'(');
            }
            /// <summary>
            /// AJAX响应输出
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public unsafe void AjaxResponse()
            {
                if (requestHeader != null)
                {
                    response response = response.Get();
                    Response = response;
                    AjaxResponse(ref response);
                }
            }
            /// <summary>
            /// AJAX响应输出
            /// </summary>
            /// <param name="response">HTTP响应</param>
            public unsafe void AjaxResponse(ref response response)
            {
                try
                {
                    if (Response == response)
                    {
                        response.SetJsContentType(DomainServer);
                        if (responseAjaxCallBack()) responseCallBackEnd();
                        responseEnd(ref response);
                    }
                }
                finally { response.Push(ref response); }
            }
            ///// <summary>
            ///// AJAX响应输出
            ///// </summary>
            ///// <typeparam name="valueType">输出数据类型</typeparam>
            ///// <param name="value">输出数据</param>
            //public unsafe void AjaxResponse<valueType>(valueType value) where valueType : json.IToJson
            //{
            //    AjaxResponse((json.IToJson)value);
            //}
            /// <summary>
            /// AJAX响应输出
            /// </summary>
            /// <typeparam name="valueType">输出数据类型</typeparam>
            /// <param name="value">输出数据</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public unsafe void AjaxResponse<valueType>(ref valueType value) where valueType : struct
            {
                if (requestHeader != null)
                {
                    response response = response.Get();
                    Response = response;
                    AjaxResponse(ref value, ref response);
                }
            }
            /// <summary>
            /// 输出AJAX回调函数结束符
            /// </summary>
            private void responseCallBackEnd()
            {
                if (responseEncoding.CodePage == Encoding.Unicode.CodePage) Response.BodyStream.Write((short)')');
                else if (responseEncoding.CodePage == fastCSharp.config.appSetting.Encoding.CodePage)
                {
                    Response.BodyStream.SimpleWriteNotNull(callBackEnd);
                }
                else Response.BodyStream.Write((byte)')');
            }
            /// <summary>
            /// AJAX响应输出参数
            /// </summary>
            private static readonly fastCSharp.emit.jsonSerializer.config toJsonConfig = new fastCSharp.emit.jsonSerializer.config { IsViewClientType = true, GetLoopObject = fastCSharp.web.ajax.GetLoopObject, SetLoopObject = fastCSharp.web.ajax.SetLoopObject, IsMaxNumberToString = true };
            /// <summary>
            /// AJAX响应输出
            /// </summary>
            /// <typeparam name="valueType">输出数据类型</typeparam>
            /// <param name="response">HTTP响应</param>
            /// <param name="value">输出数据</param>
            public unsafe void AjaxResponse<valueType>(ref valueType value, ref response response) where valueType : struct
            {
                try
                {
                    if (Response == response)
                    {
                        response.SetJsContentType(DomainServer);
                        unmanagedStream bodyStream = response.BodyStream;
                        bool isCallBack = responseAjaxCallBack();
                        if (responseEncoding.CodePage == Encoding.Unicode.CodePage)
                        {
                            charStream jsonStream = bodyStream.ToCharStream();
                            try
                            {
                                fastCSharp.emit.jsonSerializer.ToJson(value, jsonStream, isDefaultToJson ? null : toJsonConfig);
                            }
                            finally { bodyStream.From(jsonStream); }
                        }
                        else
                        {
                            this.response(fastCSharp.emit.jsonSerializer.ToJson(value, isDefaultToJson ? null : toJsonConfig));
                        }
                        if (isCallBack) responseCallBackEnd();
                        responseEnd(ref response);
                    }
                }
                finally { response.Push(ref response); }
            }
            /// <summary>
            /// 重定向
            /// </summary>
            /// <param name="path">重定向地址</param>
            /// <param name="is302">是否临时重定向</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected void location(webView.hashUrl path, bool is302 = true)
            {
                if (requestHeader.IsSearchEngine) base.location(path.ToQueryString(), false);
                else AjaxResponse(new webView.errorPath { LocationPath = path.ToString() });
            }
            /// <summary>
            /// AJAX响应输出
            /// </summary>
            /// <typeparam name="valueType">输出数据类型</typeparam>
            /// <param name="value">输出数据</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public unsafe void AjaxResponse<valueType>(valueType value) where valueType : struct
            {
                if (requestHeader != null)
                {
                    response response = response.Get();
                    Response = response;
                    AjaxResponse(value, ref response);
                }
            }
            /// <summary>
            /// AJAX响应输出
            /// </summary>
            /// <typeparam name="valueType">输出数据类型</typeparam>
            /// <param name="response">HTTP响应输出</param>
            /// <param name="value">输出数据</param>
            public unsafe void AjaxResponse<valueType>(valueType value, ref response response) where valueType : struct
            {
                try
                {
                    if (Response == response)
                    {
                        response.SetJsContentType(DomainServer);
                        unmanagedStream bodyStream = response.BodyStream;
                        bool isCallBack = responseAjaxCallBack();
                        if (responseEncoding.CodePage == Encoding.Unicode.CodePage)
                        {
                            charStream jsonStream = bodyStream.ToCharStream();
                            try
                            {
                                fastCSharp.emit.jsonSerializer.ToJson(value, jsonStream, isDefaultToJson ? null : toJsonConfig);
                            }
                            finally { bodyStream.From(jsonStream); }
                        }
                        else
                        {
                            this.response(fastCSharp.emit.jsonSerializer.ToJson(value, isDefaultToJson ? null : toJsonConfig));
                        }
                        if (isCallBack) responseCallBackEnd();
                        responseEnd(ref response);
                    }
                }
                finally { response.Push(ref response); }
            }
            /// <summary>
            /// 输出JSON字符串
            /// </summary>
            /// <param name="jsonStream">JSON字符流</param>
            /// <param name="response">HTTP响应输出</param>
            /// <returns>是否操作成功</returns>
            protected unsafe bool responseJs(charStream jsonStream, ref response response)
            {
                bool isCallBack = responseAjaxCallBack();
                if (responseEncoding.CodePage == Encoding.Unicode.CodePage)
                {
                    fastCSharp.web.ajax.FormatJavascript(jsonStream, response.BodyStream);
                }
                else
                {
                    this.response(fastCSharp.web.ajax.FormatJavascript(jsonStream));
                }
                if (isCallBack) responseCallBackEnd();
                return responseEnd(ref response);
            }
            /// <summary>
            /// HTTP响应输出处理
            /// </summary>
            /// <returns>是否成功</returns>
            protected virtual bool response()
            {
                log.Error.Add(GetType().FullName, new System.Diagnostics.StackFrame(), true);
                return false;
            }
            /// <summary>
            /// AJAX响应输出处理
            /// </summary>
            /// <param name="js">JS输出流</param>
            protected virtual void ajax(charStream js)
            {
                log.Error.Throw(GetType().FullName, new System.Diagnostics.StackFrame(), true);
            }
            /// <summary>
            /// AJAX视图异步回调输出
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected unsafe void callback()
            {
                response response = cancelAsynchronous();
                callback(ref response);
            }
            /// <summary>
            /// AJAX视图异步回调输出
            /// </summary>
            protected unsafe void callback(ref response response)
            {
                long identity = SocketIdentity;
                pointer buffer = new pointer();
                try
                {
                    if (isAjax)
                    {
                        buffer = fastCSharp.unmanagedPool.StreamBuffers.Get();
                        response.SetJsContentType(DomainServer);
                        using (charStream js = new charStream(buffer.Char, fastCSharp.unmanagedPool.StreamBuffers.Size >> 1))
                        {
                            ajax(js);
                            if (responseJs(js, ref response)) return;
                        }
                    }
                    else
                    {
                        bool isResponse = this.response();
                        if (isResponse && responseEnd(ref response)) return;
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                finally
                {
                    fastCSharp.unmanagedPool.StreamBuffers.Push(ref buffer);
                    response.Push(ref response);
                }
                if (Socket.ResponseError(identity, net.tcp.http.response.state.ServerError500)) PushPool();
            }
            /// <summary>
            /// 根据HTTP请求表单值获取内存流最大字节数
            /// </summary>
            /// <param name="value">HTTP请求表单值</param>
            /// <returns>内存流最大字节数</returns>
            public virtual int MaxMemoryStreamSize(ref fastCSharp.net.tcp.http.requestForm.value value) { return fastCSharp.config.http.Default.MaxMemoryStreamSize << 10; }
            /// <summary>
            /// 根据HTTP请求表单值获取保存文件全称
            /// </summary>
            /// <param name="value">HTTP请求表单值</param>
            /// <returns>文件全称</returns>
            public virtual string GetSaveFileName(ref fastCSharp.net.tcp.http.requestForm.value value) { return null; }
            /// <summary>
            /// 加载HTML数据
            /// </summary>
            /// <param name="fileName">HTML文件</param>
            /// <param name="htmlCount">HTML片段数量验证</param>
            /// <returns>HTML数据,失败返回null</returns>
            protected byte[][] loadHtml(string fileName, int htmlCount)
            {
                if (File.Exists(fileName = WorkPath + fileName))
                {
                    try
                    {
                        treeBuilder treeBuilder = new treeBuilder(File.ReadAllText(fileName, DomainServer.ResponseEncoding), 1);
                        if (treeBuilder.HtmlCount == htmlCount)
                        {
                            return treeBuilder.Htmls.getArray(value => DomainServer.ResponseEncoding.GetBytes(value));
                        }
                        else log.Error.Add("HTML模版文件不匹配 " + fileName, new System.Diagnostics.StackFrame(), log.cacheType.Last);
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, fileName, log.cacheType.Last);
                    }
                }
                else log.Error.Add("没有找到HTML模版文件 " + fileName, new System.Diagnostics.StackFrame(), log.cacheType.Last);
                return null;
            }
        }
        /// <summary>
        /// WEB页面视图
        /// </summary>
        /// <typeparam name="pageType">WEB页面类型</typeparam>
        [fastCSharp.emit.webView.clearMember(IsIgnoreCurrent = true)]
        public abstract class view<pageType> : view where pageType : view<pageType>
        {
            /// <summary>
            /// HTML数据
            /// </summary>
            protected static byte[][] htmls;
            ///// <summary>
            ///// 是否已经加载HTTP请求头部
            ///// </summary>
            //private int isLoadHeader;
            /// <summary>
            /// 当前WEB页面视图
            /// </summary>
            private pageType thisPage;
            /// <summary>
            /// 是否使用对象池
            /// </summary>
            private bool isPool;
            /// <summary>
            /// WEB页面视图
            /// </summary>
            protected view()
            {
                thisPage = (pageType)this;
            }
            /// <summary>
            /// WEB页面回收
            /// </summary>
            internal override void PushPool()
            {
                if (isPool)
                {
                    isPool = false;
                    clear();
#if NOJIT
#else
                    typePool<pageType>.PushNotNull(thisPage);
#endif
                }
            }
            /// <summary>
            /// HTTP请求头部处理
            /// </summary>
            /// <param name="socketIdentity">套接字操作编号</param>
            /// <param name="request">HTTP请求头部</param>
            /// <param name="isPool">是否使用WEB页面池</param>
            /// <returns>是否成功</returns>
            internal override bool LoadHeader(long socketIdentity, fastCSharp.net.tcp.http.requestHeader request, ref bool isPool)
            {
                SocketIdentity = socketIdentity;
                requestHeader = request;
                responseEncoding = DomainServer.ResponseEncoding;//request.IsWebSocket ? Encoding.UTF8 : DomainServer.ResponseEncoding;
                isHashQueryUri = !requestHeader.IsHash && requestHeader.IsSearchEngine;
                try
                {
                    if (loadHeader())
                    {
                        this.isPool = isPool;
                        isPool = false;
                        return true;
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                if (Socket.ResponseError(SocketIdentity, net.tcp.http.response.state.ServerError500) && isPool) PushPool();
                return false;
            }
            /// <summary>
            /// 加载查询参数
            /// </summary>
            /// <param name="form">HTTP请求表单</param>
            /// <returns>是否成功</returns>
            protected virtual bool load(fastCSharp.net.tcp.http.requestForm form)
            {
                this.form = form;
                return loadView();
            }
            /// <summary>
            /// 加载查询参数
            /// </summary>
            /// <param name="form">HTTP请求表单</param>
            /// <param name="isAjax">是否ajax请求</param>
            internal override unsafe void Load(fastCSharp.net.tcp.http.requestForm form, bool isAjax)
            {
                long identity = SocketIdentity;
                fastCSharp.net.tcp.http.socketBase socket = Socket;
                response response = null;
                try
                {
                    Response = (response = response.Get());
                    int asynchronousIdentity = this.asynchronousIdentity;
                    this.isAjax = isAjax;
                    if (load(form))
                    {
                        if (IsAsynchronous || asynchronousIdentity != this.asynchronousIdentity) response = null;
                        else callback(ref response);
                        return;
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                finally 
                {
                    response.Push(ref response);
                }
                if (socket.ResponseError(identity, net.tcp.http.response.state.ServerError500)) PushPool();
            }
#if NOJIT
#else
            /// <summary>
            /// 清除页面信息
            /// </summary>
            protected override void clear()
            {
                fastCSharp.emit.webView.clearMember<pageType>.Cleaner(thisPage);
                base.clear();
            }
#endif
            /// <summary>
            /// 加载HTML数据
            /// </summary>
            /// <param name="fileName">HTML文件</param>
            /// <param name="htmlCount">HTML片段数量验证</param>
            /// <returns></returns>
            protected bool isLoadHtml(string fileName, int htmlCount)
            {
                if (htmls != null) return true;
                if (File.Exists(fileName = WorkPath + fileName))
                {
                    try
                    {
                        treeBuilder treeBuilder = new treeBuilder(File.ReadAllText(fileName, DomainServer.ResponseEncoding), 1);
                        if (treeBuilder.HtmlCount == htmlCount)
                        {
                            htmls = treeBuilder.Htmls.getArray(value => DomainServer.ResponseEncoding.GetBytes(value));
                            return true;
                        }
                        log.Error.Add("HTML模版文件不匹配 " + fileName, new System.Diagnostics.StackFrame(), log.cacheType.Last);
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, fileName, log.cacheType.Last);
                    }
                }
                else log.Error.Add("没有找到HTML模版文件 " + fileName, new System.Diagnostics.StackFrame(), log.cacheType.Last);
                return false;
            }
        }
        ///// <summary>
        ///// WEB页面视图
        ///// </summary>
        //public abstract class view<pageType, queryType> : view<pageType>
        //    where pageType : view<pageType, queryType>
        //{
        //    /// <summary>
        //    /// 请求查询数据
        //    /// </summary>
        //    protected queryType query;
        //    /// <summary>
        //    /// 请求查询数据
        //    /// </summary>
        //    /// <returns>新建请求查询数据</returns>
        //    protected virtual queryType newQuery()
        //    {
        //        return fastCSharp.code.constructor<queryType>.New();
        //    }
        //    /// <summary>
        //    /// 加载查询参数
        //    /// </summary>
        //    /// <param name="request">HTTP请求表单</param>
        //    /// <returns>是否成功</returns>
        //    protected override bool load(fastCSharp.net.tcp.http.requestForm form)
        //    {
        //        this.form = form;
        //        query = newQuery();
        //        if (form != null && form.Json != null)
        //        {
        //            if (form.Json.Length != 0
        //                && !fastCSharp.emit.jsonParser.Parse(form.Json, ref query))
        //            {
        //                return false;
        //            }
        //        }
        //        else
        //        {
        //            if (!requestHeader.ParseQuery(ref query)) return false;
        //            subString queryJson = requestHeader.QueryJson;
        //            if (queryJson.Length != 0
        //                && !fastCSharp.emit.jsonParser.Parse(queryJson, ref query))
        //            {
        //                return false;
        //            }
        //        }
        //        return loadView();
        //    }
        //}
        /// <summary>
        /// HTML模板建树器
        /// </summary>
        public class treeBuilder
        {
            ///// <summary>
            ///// 视图body替换内容
            ///// </summary>
            //public const string ViewBody = @"<div id=""fastCSharpViewOver"" style=""position:fixed;left:0px;top:0px;width:100%;height:100%;z-index:10000;display:block;background-color:#ffffff"">视图数据加载中...</div>";
            /// <summary>
            /// @取值command
            /// </summary>
            private readonly static string atCommand = command.At.ToString();
            /// <summary>
            /// =@取值字符位图尺寸
            /// </summary>
            private const int atMapSize = 128;
            /// <summary>
            /// 图片源
            /// </summary>
            private static readonly Regex imageSrc = new Regex(@" @src=""", RegexOptions.Compiled);
            /// <summary>
            /// =@取值字符位图
            /// </summary>
            private static readonly fixedMap atMap;
            /// <summary>
            /// 模板命令HASH匹配数据容器尺寸
            /// </summary>
            private const int commandUniqueHashDataCount = 16;
            /// <summary>
            /// 模板命令HASH匹配数据尺寸
            /// </summary>
            private const int commandUniqueHashDataSize = 16;
            /// <summary>
            /// 模板命令HASH匹配数据
            /// </summary>
            private static readonly pointer.reference commandUniqueHashData;
            /// <summary>
            /// 客户端命令索引位置
            /// </summary>
            private static readonly int clientCommandIndex;
            /// <summary>
            /// HTML模板树节点
            /// </summary>
            public sealed class node : fastCSharp.code.template<fastCSharp.code.cSharp.webView.treeBuilder.node>.INode, code.treeBuilder<node, tag>.INode
            {
                /// <summary>
                /// 树节点标识
                /// </summary>
                public tag Tag { get; internal set; }
                /// <summary>
                /// 模板命令
                /// </summary>
                public string TemplateCommand
                {
                    get { return Tag.command; }
                }
                /// <summary>
                /// 模板成员名称
                /// </summary>
                public string TemplateMemberName
                {
                    get { return Tag.command.value != null ? Tag.content : null; }
                }
                /// <summary>
                /// 模板成员名称
                /// </summary>
                public string TemplateMemberNameBeforeAt
                {
                    get
                    {
                        string name = TemplateMemberName;
                        int index = name.IndexOf('@');
                        return index == -1 ? name : name.Substring(0, index);
                    }
                }
                /// <summary>
                /// 模板文本代码
                /// </summary>
                public string TemplateCode
                {
                    get { return Tag.command.value == null ? Tag.content : null; }
                }
                /// <summary>
                /// 子节点数量
                /// </summary>
                public int ChildCount
                {
                    get { return childs.length; }
                }
                /// <summary>
                /// 子节点集合
                /// </summary>
                public IEnumerable<node> Childs
                {
                    get { return childs; }
                }
                /// <summary>
                /// 子节点集合
                /// </summary>
                private subArray<node> childs;
                /// <summary>
                /// 设置子节点集合
                /// </summary>
                /// <param name="childs">子节点集合</param>
                [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void SetChilds(node[] childs)
                {
                    if (Tag != null) Tag.IsRound = true;
                    this.childs = new subArray<node>(childs);
                }
                ///// <summary>
                ///// 获取第一个匹配的子孙节点
                ///// </summary>
                ///// <param name="command">模板命令类型</param>
                ///// <param name="content">内容</param>
                ///// <returns>匹配的CSharp代码树节点</returns>
                //public node GetFirstNodeByTag(code.template.command command, string content)
                //{
                //    if (Tag.command == command.ToString() && Tag.content == content) return this;
                //    if (childs != null)
                //    {
                //        foreach (node son in childs)
                //        {
                //            node value = son.GetFirstNodeByTag(command, content);
                //            if (value != null) return value;
                //        }
                //    }
                //    return null;
                //}
            }
            /// <summary>
            /// HTML模板树节点标识
            /// </summary>
            public sealed class tag : IEquatable<tag>
            {
                /// <summary>
                /// 树节点标识类型
                /// </summary>
                internal enum type
                {
                    /// <summary>
                    /// 普通HTML段
                    /// </summary>
                    Html,
                    /// <summary>
                    /// 注释子段
                    /// </summary>
                    Note,
                    /// <summary>
                    /// =@取值代码
                    /// </summary>
                    At,
                }
                /// <summary>
                /// 树节点标识类型
                /// </summary>
                internal type tagType;
                /// <summary>
                /// 标识command
                /// </summary>
                internal subString command;
                /// <summary>
                /// 内容
                /// </summary>
                internal subString content;
                /// <summary>
                /// 是否已经回合
                /// </summary>
                internal bool IsRound;
                /// <summary>
                /// 判断树节点标识是否相等
                /// </summary>
                /// <param name="other">树节点标识</param>
                /// <returns>是否相等</returns>
                public bool Equals(tag other)
                {
                    return other != null && other.tagType == tagType && other.command.Equals(command) && other.content.Equals(content);
                }
                /// <summary>
                /// 转换成输出字符串
                /// </summary>
                /// <returns></returns>
                public override string ToString()
                {
                    if (tagType == type.Html) return "\t" + tagType.ToString() + " " + command.ToString();
                    if (IsRound || command.Equals("At")) return "\t" + tagType.ToString() + " " + command.ToString() + " " + content.ToString();
                    return (subString.Unsafe(command.value, 0, command.StartIndex).Count('\n') + 1).toString() + " : " + tagType.ToString() + " " + command.ToString() + " " + content.ToString();
                }
            }
            /// <summary>
            /// 建树器
            /// </summary>
            private code.treeBuilder<node, tag> tree;
            /// <summary>
            /// HTML片段
            /// </summary>
            private Dictionary<hashString, int> htmls = dictionary.CreateHashString<int>();
            /// <summary>
            /// HTML片段数量
            /// </summary>
            public int HtmlCount
            {
                get { return htmls.Count; }
            }
            /// <summary>
            /// HTML片段
            /// </summary>
            internal string[] Htmls
            {
                get
                {
                    string[] values = new string[htmls.Count];
                    foreach (KeyValuePair<hashString, int> value in htmls) values[value.Value] = value.Key.ToString();
                    return values;
                }
            }
            /// <summary>
            /// 树节点
            /// </summary>
            public node Boot { get; private set; }
            /// <summary>
            /// 是否仅仅生成HTML
            /// </summary>
            private int isOnlyHtml;
            /// <summary>
            /// HTML模板建树器
            /// </summary>
            /// <param name="html">HTML</param>
            /// <param name="isOnlyHtml">是否仅仅解析HTML（不生成AJAX数据）</param>
            public treeBuilder(string html, int isOnlyHtml)
            {
                if ((this.isOnlyHtml = isOnlyHtml) == 0) tree = new code.treeBuilder<node, tag>();
                create(formatHtml(html));
            }
            /// <summary>
            /// 根据HTML获取模板树
            /// </summary>
            /// <param name="html">HTML</param>
            private unsafe void create(string html)
            {
                if (html != null)
                {
                    if (html.Length >= 3)
                    {
                        fixedMap atFixedMap = atMap;
                        byte* commandUniqueHashDataFixed = commandUniqueHashData.Byte;
                        fixed (char* htmlFixed = html)
                        {
                            char* start = htmlFixed, end = htmlFixed + html.Length - 1, current = htmlFixed;
                            char endChar = *end;
                            do
                            {
                                for (*end = '<'; *current != '<'; ++current) ;
                                if (current == end) break;
                                if (*++current == '!' && *(int*)++current == ('-' | ('-' << 16)))
                                {
                                    char* tagStart = current += 2;
                                    for (*end = '>'; *current != '>'; ++current) ;
                                    if (current == end && endChar != '>') break;
                                    if (*(int*)(current -= 2) == ('-' | ('-' << 16)))
                                    {
                                        char* contentStart = tagStart;
                                        for (*current = ':'; *contentStart != ':'; ++contentStart) ;
                                        *current = '-';
                                        int tagLength = (int)(contentStart - tagStart), tagIndex = (((*tagStart >> 1) ^ (tagStart[tagLength >> 2] >> 2)) & ((1 << 4) - 1));
                                        byte* hashData = commandUniqueHashDataFixed + (tagIndex * commandUniqueHashDataSize);
                                        if (*(int*)hashData == tagLength && unsafer.memory.SimpleEqual((byte*)tagStart, hashData + sizeof(int), tagLength << 1))
                                        {
                                            tag tag = new tag { tagType = treeBuilder.tag.type.Note, command = subString.Unsafe(html, (int)(tagStart - htmlFixed), (int)(contentStart - tagStart)), content = contentStart == current ? subString.Unsafe(html, 0, 0) : subString.Unsafe(html, (int)(++contentStart - htmlFixed), (int)(current - contentStart)) };
                                            #region =@取值解析
                                            if (start != (tagStart -= 4))
                                            {
                                                contentStart = start;
                                                *tagStart = '@';
                                                do
                                                {
                                                    while (*++contentStart != '@') ;
                                                    if (contentStart == tagStart) break;
                                                    if (*--contentStart == '=' && *(contentStart + 2) != '[')
                                                    {
                                                        if (start != contentStart)
                                                        {
                                                            appendHtml(subString.Unsafe(html, (int)(start - htmlFixed), (int)(contentStart - start)));
                                                        }
                                                        start = (contentStart += 2);
                                                        if (contentStart == tagStart)
                                                        {
                                                            if (isOnlyHtml == 0) tree.Append(new node { Tag = new tag { tagType = treeBuilder.tag.type.At, command = atCommand, content = subString.Unsafe(html, 0, 0) } }, false);
                                                            break;
                                                        }
                                                        if (*contentStart == '$')
                                                        {
                                                            if (isOnlyHtml == 0) tree.Append(new node { Tag = new tag { tagType = treeBuilder.tag.type.At, command = atCommand, content = subString.Unsafe(html, (int)(start - htmlFixed), 1) } }, false);
                                                            ++contentStart;
                                                        }
                                                        else
                                                        {
                                                            if (*contentStart == '@' || *contentStart == '*')
                                                            {
                                                                if (++contentStart == tagStart)
                                                                {
                                                                    if (isOnlyHtml == 0) tree.Append(new node { Tag = new tag { tagType = treeBuilder.tag.type.At, command = atCommand, content = subString.Unsafe(html, (int)(start - htmlFixed), 1) } }, false);
                                                                    ++start;
                                                                    break;
                                                                }
                                                            }
                                                            while (*contentStart < atMapSize && atFixedMap.Get(*contentStart)) ++contentStart;
                                                            if (*contentStart == '#' && (*(contentStart + 1) < atMapSize && atFixedMap.Get(*(contentStart + 1))))
                                                            {
                                                                for (contentStart += 2; *contentStart < atMapSize && atFixedMap.Get(*contentStart); ++contentStart) ;
                                                            }
                                                            if (isOnlyHtml == 0) tree.Append(new node { Tag = new tag { tagType = treeBuilder.tag.type.At, command = atCommand, content = subString.Unsafe(html, (int)(start - htmlFixed), (int)(contentStart - start)) } }, false);
                                                            if (*contentStart == '$') ++contentStart;
                                                        }
                                                        start = contentStart;
                                                    }
                                                    else contentStart += 2;
                                                }
                                                while (contentStart != tagStart);
                                                *tagStart = '<';
                                                if (start != tagStart)
                                                {
                                                    appendHtml(subString.Unsafe(html, (int)(start - htmlFixed), (int)(tagStart - start)));
                                                }
                                            }
                                            #endregion
                                            if (isOnlyHtml == 0 && !tree.IsRound(tag, tagIndex == clientCommandIndex)) tree.Append(new node { Tag = tag });
                                            start = current + 3;
                                        }
                                    }
                                    current += 3;
                                }
                            }
                            while (current < end);
                            #region 最后=@取值解析
                            if (current <= end)
                            {
                                current = start;
                                *end = '@';
                                do
                                {
                                    while (*current != '@') ++current;
                                    if (current == end) break;
                                    if (*--current == '=')
                                    {
                                        if (start != current)
                                        {
                                            appendHtml(subString.Unsafe(html, (int)(start - htmlFixed), (int)(current - start)));
                                        }
                                        start = (current += 2);
                                        if (current == end)
                                        {
                                            if (isOnlyHtml == 0) tree.Append(new node { Tag = new tag { tagType = treeBuilder.tag.type.At, command = atCommand, content = subString.Unsafe(html, 0, 0) } }, false);
                                            break;
                                        }
                                        if (*current == '@' || *current == '*')
                                        {
                                            if (++current == end)
                                            {
                                                if (isOnlyHtml == 0) tree.Append(new node { Tag = new tag { tagType = treeBuilder.tag.type.At, command = atCommand, content = subString.Unsafe(html, (int)(start - htmlFixed), 1) } }, false);
                                                ++start;
                                                break;
                                            }
                                        }
                                        while (*current < atMapSize && atFixedMap.Get(*current)) ++current;
                                        if (*current == '#' && (*(current + 1) < atMapSize && atFixedMap.Get(*(current + 1))))
                                        {
                                            for (current += 2; *current < atMapSize && atFixedMap.Get(*current); ++current) ;
                                        }
                                        if (isOnlyHtml == 0) tree.Append(new node { Tag = new tag { tagType = treeBuilder.tag.type.At, command = atCommand, content = subString.Unsafe(html, (int)(start - htmlFixed), (int)(current - start)) } }, false);
                                        start = current;
                                    }
                                    else current += 2;
                                }
                                while (current != end);
                                *end = endChar;
                                appendHtml(subString.Unsafe(html, (int)(start - htmlFixed), (int)(end - start) + 1));
                            }
                            #endregion
                            *end = endChar;
                        }
                    }
                    else appendHtml(html);
                    if (isOnlyHtml == 0) (Boot = new node()).SetChilds(tree.End());
                }
            }
            /// <summary>
            /// 添加HTML片段
            /// </summary>
            /// <param name="html">HTML片段</param>
            private void appendHtml(subString html)
            {
                if (isOnlyHtml == 0) tree.Append(new node { Tag = new tag { tagType = treeBuilder.tag.type.Html, content = html } }, false);
                hashString htmlKey = html;
                if (!htmls.ContainsKey(htmlKey)) htmls.Add(htmlKey, htmls.Count);
            }
            /// <summary>
            /// 获取HTML片段索引号
            /// </summary>
            /// <param name="html">HTML片段</param>
            /// <returns>索引号</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public int GetHtmlIndex(string html)
            {
                //if (!htmls.ContainsKey(html)) log.Default.Add("->" + html + "<-", false, false);
                return htmls[html];
            }
            /// <summary>
            /// HTML模板格式化处理
            /// </summary>
            /// <param name="html">HTML模板</param>
            /// <returns>格式化处理后的HTML模板</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private static string formatHtml(string html)
            {
                html = imageSrc.Replace(html, @" src=""");
                //html = html.Replace(ViewBody, string.Empty);
                return html;
            }
            static unsafe treeBuilder()
            {
                pointer[] datas = unmanaged.GetStatic(true, atMapSize, commandUniqueHashDataCount * commandUniqueHashDataSize);
                int dataIndex = 0;
                atMap = new fixedMap(datas[dataIndex++]);
                commandUniqueHashData = datas[dataIndex++].Reference;

                atMap.Set('0', 10);
                atMap.Set('A', 26);
                atMap.Set('a', 26);
                atMap.Set('.');
                atMap.Set('_');

                for (byte* start = commandUniqueHashData.Byte, end = start + commandUniqueHashDataCount * commandUniqueHashDataSize; start != end; start += commandUniqueHashDataSize) *(int*)start = int.MinValue;
                foreach (command command in System.Enum.GetValues(typeof(command)))
                {
                    string commandString = command.ToString();
                    if (sizeof(int) + (commandString.Length << 1) > commandUniqueHashDataSize)
                    {
                        log.Error.Throw(log.exceptionType.IndexOutOfRange);
                    }
                    fixed (char* commandFixed = commandString)
                    {
                        int code = ((*commandFixed >> 1) ^ (commandFixed[commandString.Length >> 2] >> 2)) & ((1 << 4) - 1);
                        if (command == command.Client) clientCommandIndex = code;
                        byte* data = commandUniqueHashData.Byte + (code * commandUniqueHashDataSize);
                        if (*(int*)data != int.MinValue) log.Error.Throw(log.exceptionType.IndexOutOfRange);
                        *(int*)data = commandString.Length;
                        unsafer.memory.Copy(commandFixed, data + sizeof(int), commandString.Length << 1);
                    }
                }                
            }
        }
        /// <summary>
        /// HTML模板命令类型
        /// </summary>
        public enum command
        {
            /// <summary>
            /// 输出绑定的数据
            /// </summary>
            At,
            /// <summary>
            /// 客户端代码段
            /// </summary>
            Client,
            /// <summary>
            /// 子代码段
            /// </summary>
            Value,
            /// <summary>
            /// 循环
            /// </summary>
            Loop,
            /// <summary>
            /// 循环=LOOP
            /// </summary>
            For,
            /// <summary>
            /// 屏蔽代码段输出
            /// </summary>
            Note,
            /// <summary>
            /// 绑定的数据为true非0非null时输出代码
            /// </summary>
            If,
            /// <summary>
            /// 绑定的数据为false或者0或者null时输出代码
            /// </summary>
            Not,
        }
        /// <summary>
        /// WEB 视图调用类型名称，用于替换默认的类型名称。
        /// </summary>
        public string TypeCallName;
        /// <summary>
        /// 如果存在查询参数，则会生成一个 struct 用于整合这些参数，同时生成一个这个类型的字段，这个字段名称默认为 query。
        /// </summary>
        public string QueryName = fastCSharp.config.web.Default.ViewQueryName;
        /// <summary>
        /// 来源路径重写
        /// </summary>
        public string RewritePath;
        /// <summary>
        /// 来源路径重写是否补全扩展名称
        /// </summary>
        public bool IsRewriteHtml = true;
        /// <summary>
        /// 默认为 true 表示支持服务端 HTML 输出，也就是说支持搜索引擎访问，否则对于搜索引擎返回 404。
        /// </summary>
        public bool IsPage = true;
        /// <summary>
        /// 默认为 true 表示支持服务端输出 WEB 视图 JSON 数据，也就是说支持浏览器的正常访问。
        /// </summary>
        public bool IsAjax = true;
        /// <summary>
        /// 默认为 true 表示需要验证 Referer 的来源页主域名是否匹配当前主域名，用于防止跨域攻击。
        /// </summary>
        public bool IsReferer = true;
        /// <summary>
        /// 默认为 false 表示不生成 TypeScript 调用代理，一般只有在嵌入式页面中才需要设置为 true，只有 IsAjax = true 时有效。
        /// </summary>
        public bool IsExportTypeScript;
        /// <summary>
        /// 获取视图加载函数+视图加载函数配置
        /// </summary>
        /// <param name="type">视图类型</param>
        /// <returns>视图加载函数+视图加载函数配置</returns>
        public static keyValue<methodInfo, webView> GetLoadMethod(Type type)
        {
            methodInfo loadMethod = null;
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (method.ReturnType == typeof(bool))
                {
                    webView loadWebView = method.customAttribute<webView>();
                    if (loadWebView != null)
                    {
                        return new keyValue<methodInfo, webView>(new methodInfo(method, code.memberFilters.Instance), loadWebView);
                    }
                    if (loadMethod == null && method.Name == "loadView" && method.GetParameters().Length != 0)
                    {
                        loadMethod = new methodInfo(method, code.memberFilters.Instance);
                    }
                }
            }
            return new keyValue<methodInfo, webView>(loadMethod, loadMethod == null ? null : Null);
        }
    }
}
namespace fastCSharp.code
{
    /// <summary>
    /// 成员类型
    /// </summary>
    public sealed partial class memberType
    {
        /// <summary>
        /// AJAX toString重定向类型集合
        /// </summary>
        private static readonly HashSet<Type> ajaxToStringTypes = new HashSet<Type>(new Type[] { typeof(bool), typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal), typeof(char) });
        /// <summary>
        /// 是否#!URL
        /// </summary>
        public bool IsHashUrl
        {
            get { return Type == typeof(fastCSharp.code.cSharp.webView.hashUrl); }
        }
        /// <summary>
        /// 是否AJAX toString重定向类型
        /// </summary>
        public bool IsAjaxToString
        {
            get { return ajaxToStringTypes.Contains(Type.nullableType() ?? Type); }
        }
        /// <summary>
        /// 客户端视图绑定类型
        /// </summary>
        private cSharp.webView.clientType clientViewType;
        /// <summary>
        /// 客户端视图绑定类型
        /// </summary>
        public string ClientViewTypeName
        {
            get
            {
                if (clientViewType == null)
                {
                    clientViewType = fastCSharp.code.typeAttribute.GetAttribute<cSharp.webView.clientType>(Type, true, true) ?? cSharp.webView.clientType.Null;
                }
                return clientViewType.GetClientName(Type);
            }
        }
        /// <summary>
        /// 客户端视图绑定成员名称
        /// </summary>
        public string ClientViewMemberName
        {
            get
            {
                if (clientViewType == null)
                {
                    clientViewType = fastCSharp.code.typeAttribute.GetAttribute<cSharp.webView.clientType>(Type, true, true) ?? cSharp.webView.clientType.Null;
                }
                return clientViewType.MemberName;
            }
        }
    }
}
