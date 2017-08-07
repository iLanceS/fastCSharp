using System;
using fastCSharp.net.tcp.http;
using System.Collections.Generic;
using fastCSharp.threading;
using System.Runtime.CompilerServices;
using System.Threading;

namespace fastCSharp.code.cSharp
{
    /// <summary>
    /// AJAX调用配置
    /// </summary>
    public sealed partial class ajax : webPage
    {
        /// <summary>
        /// 公用错误处理函数名称
        /// </summary>
        public const string PubErrorCallName = "pub.Error";
        /// <summary>
        /// AJAX调用
        /// </summary>
        /// <typeparam name="ajaxType">AJAX调用类型</typeparam>
        [fastCSharp.emit.webView.clearMember(IsIgnoreCurrent = true)]
        public abstract class call<ajaxType> : webView.view<ajaxType>, webView.IWebView where ajaxType : call<ajaxType>
        {
            /// <summary>
            /// WEB视图加载
            /// </summary>
            /// <returns>是否成功</returns>
            protected override bool loadView()
            {
                return false;
            }
        }
        /// <summary>
        /// AJAX调用信息
        /// </summary>
        public sealed class call
        {
            /// <summary>
            /// AJAX调用
            /// </summary>
            public int CallIndex;
            /// <summary>
            /// 最大接收数据字节数
            /// </summary>
            public int MaxPostDataSize;
            /// <summary>
            /// 内存流最大字节数
            /// </summary>
            public int MaxMemoryStreamSize;
            /// <summary>
            /// 是否视图页面
            /// </summary>
            public bool IsViewPage;
            /// <summary>
            /// 是否只接受POST请求
            /// </summary>
            public bool IsPost;
            /// <summary>
            /// 判断来源页是否合法
            /// </summary>
            public bool IsReferer;
            /// <summary>
            /// AJAX调用信息
            /// </summary>
            /// <param name="callIndex">AJAX调用索引</param>
            /// <param name="maxPostDataSize">最大接收数据字节数</param>
            /// <param name="maxMemoryStreamSize">内存流最大字节数</param>
            /// <param name="isViewPage">是否视图页面</param>
            /// <param name="isPost">是否只接受POST请求</param>
            /// <param name="isReferer">是否验证请求来源地址</param>
            public call(int callIndex, int maxPostDataSize, int maxMemoryStreamSize, bool isViewPage, bool isPost = true, bool isReferer = true)
            {
                CallIndex = callIndex;
                MaxPostDataSize = maxPostDataSize;
                MaxMemoryStreamSize = maxMemoryStreamSize;
                IsViewPage = isViewPage;
                IsPost = isPost;
                IsReferer = isReferer;
            }
        }
        /// <summary>
        /// 表单加载
        /// </summary>
        public class loader : requestForm.ILoadForm
        {
            /// <summary>
            /// HTTP套接字接口
            /// </summary>
            private socketBase socket;
            /// <summary>
            /// 域名服务
            /// </summary>
            private domainServer domainServer;
            /// <summary>
            /// HTTP请求头
            /// </summary>
            private requestHeader request;
            /// <summary>
            /// 套接字请求编号
            /// </summary>
            public long SocketIdentity { get; private set; }
            /// <summary>
            /// web调用
            /// </summary>
            private webCall.call webCall;
            /// <summary>
            /// AJAX调用信息
            /// </summary>
            private call call;
            /// <summary>
            /// 表单加载
            /// </summary>
            internal loader() { }
            /// <summary>
            /// HTTP请求表单
            /// </summary>
            private requestForm form;
            /// <summary>
            /// 表单加载回收
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private void push()
            {
                webCall = null;
                socket = null;
                domainServer = null;
                request = null;
                form = null;
                call = null;
                typePool<loader>.PushNotNull(this);
            }
            /// <summary>
            /// 表单回调处理
            /// </summary>
            /// <param name="form">HTTP请求表单</param>
            public void OnGetForm(requestForm form)
            {
                if (form == null) push();
                else
                {
                    SocketIdentity = form.Identity;
                    Load(form);
                }
            }
            /// <summary>
            /// WEB视图表单加载
            /// </summary>
            /// <param name="form">HTTP请求表单</param>
            internal void Load(requestForm form)
            {
                long identity = SocketIdentity;
                socketBase socket = this.socket;
                try
                {
                    this.form = form;
                    webCall.CallAjax(call.CallIndex, this);
                    return;
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                finally { push(); }
                socket.ResponseError(identity, response.state.ServerError500);
            }
            /// <summary>
            /// 根据HTTP请求表单值获取内存流最大字节数
            /// </summary>
            /// <param name="value">HTTP请求表单值</param>
            /// <returns>内存流最大字节数</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public int MaxMemoryStreamSize(ref fastCSharp.net.tcp.http.requestForm.value value)
            {
                return call.MaxMemoryStreamSize;
            }
            /// <summary>
            /// 根据HTTP请求表单值获取保存文件全称
            /// </summary>
            /// <param name="value">HTTP请求表单值</param>
            /// <returns>文件全称</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public string GetSaveFileName(ref fastCSharp.net.tcp.http.requestForm.value value)
            {
                return null;
            }
            /// <summary>
            /// AJAX数据加载
            /// </summary>
            /// <typeparam name="ajaxType">AJAX调用类型</typeparam>
            /// <param name="ajax">AJAX调用</param>
            /// <param name="isPool">是否使用WEB页面池</param>
            /// <returns>HTTP响应,失败返回null</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public response Load<ajaxType>(ajaxType ajax, bool isPool)
                where ajaxType : webView.view, webView.IWebView
            {
                return ajax.GetLoadResponse(SocketIdentity, socket, domainServer, request, form, isPool);
            }
            /// <summary>
            /// AJAX数据加载
            /// </summary>
            /// <typeparam name="ajaxType">AJAX调用类型</typeparam>
            /// <typeparam name="valueType">调用参数类型</typeparam>
            /// <param name="ajax">AJAX调用</param>
            /// <param name="parameter">参数值</param>
            /// <param name="isPool">是否使用WEB页面池</param>
            /// <returns>HTTP响应,失败返回null</returns>
            public response Load<ajaxType, valueType>(ajaxType ajax, ref valueType parameter, bool isPool)
                where ajaxType : webView.view, webView.IWebView
                where valueType : struct
            {
                socketBase socket = this.socket;
                try
                {
                    ajax.Socket = socket;
                    ajax.DomainServer = domainServer;
                    if (ajax.LoadHeader(SocketIdentity, request, ref isPool))
                    {
                        ajax.form = form;
                        if (ajax.ParseParameter(ref parameter)) return ajax.Response = response.Get();
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                if (socket.ResponseError(SocketIdentity, response.state.ServerError500) && isPool) typePool<ajaxType>.PushNotNull(ajax);
                return null;
            }
            /// <summary>
            /// 加载WEB视图
            /// </summary>
            /// <typeparam name="valueType">WEB视图类型</typeparam>
            /// <param name="view">WEB视图</param>
            /// <param name="isPool">是否使用WEB视图池</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void LoadView<valueType>(valueType view, bool isPool)
                where valueType : webView.view, webView.IWebView
            {
                view.LoadAjaxView(SocketIdentity, socket, domainServer, request, form, isPool);
            }
            /// <summary>
            /// 表单加载
            /// </summary>
            /// <param name="webCall">web调用</param>
            /// <param name="socket">HTTP套接字接口</param>
            /// <param name="domainServer">域名服务</param>
            /// <param name="socketIdentity">套接字操作编号</param>
            /// <param name="request">HTTP请求头</param>
            /// <param name="call">AJAX调用信息</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void Set(webCall.call webCall, socketBase socket, domainServer domainServer, long socketIdentity, requestHeader request, call call)
            {
                this.webCall = webCall;
                this.socket = socket;
                this.domainServer = domainServer;
                this.SocketIdentity = socketIdentity;
                this.request = request;
                this.call = call;
            }
        }
        /// <summary>
        /// 公用AJAX调用
        /// </summary>
        [fastCSharp.emit.webView.clearMember(IsIgnoreCurrent = true)]
        private sealed class pub : call<pub>
        {
            /// <summary>
            /// 公用错误处理参数
            /// </summary>
            public struct errorParameter
            {
                /// <summary>
                /// 错误信息
                /// </summary>
#pragma warning disable
                public string error;
#pragma warning restore
            }
            /// <summary>
            /// 错误信息队列
            /// </summary>
            private static readonly fifoPriorityQueue<hashString, string> errorQueue = new fifoPriorityQueue<hashString, string>();
            /// <summary>
            /// 错误信息队列访问锁
            /// </summary>
            private static readonly object errorQueueLock = new object();
            /// <summary>
            /// 公用错误处理函数
            /// </summary>
            /// <param name="error">错误信息</param>
            public void Error(string error)
            {
                if (!string.IsNullOrEmpty(error))
                {
                    bool isLog = false;
                    if (error.Length <= config.web.Default.PubErrorMaxSize)
                    {
                        hashString errorHash = error;
                        Monitor.Enter(errorQueueLock);
                        try
                        {
                            if (errorQueue.Set(errorHash, error) == null)
                            {
                                isLog = true;
                                if (errorQueue.Count > config.web.Default.PubErrorMaxCacheCount) errorQueue.UnsafePopValue();
                            }
                        }
                        finally { Monitor.Exit(errorQueueLock); }
                    }
                    else isLog = true;
                    if (isLog) fastCSharp.log.Default.Add(error, new System.Diagnostics.StackFrame(), false);
                }
            }
            static pub()
            {
                if (fastCSharp.config.appSetting.IsCheckMemory) checkMemory.Add(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            }
        }
        /// <summary>
        /// AJAX调用加载
        /// </summary>
        /// <typeparam name="loaderType">AJAX调用加载类型</typeparam>
        [fastCSharp.emit.webView.clearMember(IsIgnoreCurrent = true)]
        public abstract class loader<loaderType> : webCall.call<loaderType> where loaderType : loader<loaderType>
        {
            /// <summary>
            /// AJAX调用
            /// </summary>
            /// <param name="methods">AJAX函数调用集合</param>
            protected void load(fastCSharp.stateSearcher.ascii<call> methods)
            {
                try
                {
                    long identity = SocketIdentity;
#if MONO
                    call call = methods.Get(requestHeader.AjaxCallName);
#else
                    call call = methods.Get(DomainServer.WebConfig.IgnoreCase ? requestHeader.LowerAjaxCallName : requestHeader.AjaxCallName);
#endif
                    if (call == null)
                    {
#if MONO
                        byte[] path = DomainServer.GetViewRewrite(requestHeader.AjaxCallName);
#else
                        byte[] path = DomainServer.GetViewRewrite(DomainServer.WebConfig.IgnoreCase ? requestHeader.LowerAjaxCallName : requestHeader.AjaxCallName);
#endif
                        if (path != null) call = methods.Get(path);
                    }
                    if (call != null && (requestHeader.Method == web.http.methodType.POST || !call.IsPost)// || requestHeader.IsWebSocket
                        && (!call.IsReferer || !fastCSharp.config.web.Default.IsAjaxReferer || fastCSharp.config.pub.Default.IsDebug || requestHeader.IsReferer))
                    {
                        if (requestHeader.ContentLength <= call.MaxPostDataSize)
                        {
                            if (call.IsViewPage) secondCount.Add();
                            loader loadForm = typePool<loader>.Pop() ?? new loader();
                            loadForm.Set(this, Socket, DomainServer, identity, requestHeader, call);
                            if (requestHeader.Method == web.http.methodType.POST) Socket.GetForm(identity, loadForm);
                            else loadForm.Load((requestForm)null);
                        }
                        else Socket.ResponseError(identity, net.tcp.http.response.state.ServerError500);
                        return;
                    }
                    Socket.ResponseError(identity, net.tcp.http.response.state.NotFound404);
                }
                finally { PushPool(); }
            }
            /// <summary>
            /// 公用错误处理函数
            /// </summary>
            /// <param name="loader">表单加载</param>
            protected static void pubError(fastCSharp.code.cSharp.ajax.loader loader)
            {
                pub view = fastCSharp.typePool<pub>.Pop() ?? new pub();
                pub.errorParameter parameter = new pub.errorParameter();
                response response = loader.Load(view, ref parameter, true);
                if (response != null)
                {
                    try
                    {
                        view.Error(parameter.error);
                    }
                    finally { view.AjaxResponse(ref response); }
                }
            }
        }
        /// <summary>
        /// AJAX异步回调
        /// </summary>
        /// <typeparam name="callbackType">异步回调类型</typeparam>
        /// <typeparam name="ajaxType">AJAX类型</typeparam>
        public abstract class callback<callbackType, ajaxType>
            where callbackType : callback<callbackType, ajaxType>
            where ajaxType : webView.view<ajaxType>
        {
            /// <summary>
            /// 当前AJAX异步回调
            /// </summary>
            private callbackType thisCallback;
            /// <summary>
            /// AJAX回调对象
            /// </summary>
            private ajaxType ajax;
            /// <summary>
            /// HTTP响应
            /// </summary>
            private response response;
            /// <summary>
            /// AJAX回调处理
            /// </summary>
            private Action<fastCSharp.net.returnValue> onReturnHandle;
            /// <summary>
            /// AJAX异步回调
            /// </summary>
            protected callback()
            {
                thisCallback = (callbackType)this;
                onReturnHandle = onReturn;
            }
            /// <summary>
            /// AJAX回调处理
            /// </summary>
            /// <param name="value">回调值</param>
            private void onReturn(fastCSharp.net.returnValue value)
            {
                try
                {
                    if (value.Type == fastCSharp.net.returnValue.type.Success)
                    {
                        ajax.cancelAsynchronous();
                        ajax.AjaxResponse(ref response);
                    }
                    else ajax.serverError500();
                }
                finally
                {
                    ajax = null;
                    response = null;
                    typePool<callbackType>.PushNotNull(thisCallback);
                }
            }
            /// <summary>
            /// 获取AJAX回调处理
            /// </summary>
            /// <param name="ajax">AJAX回调对象</param>
            /// <param name="response">HTTP响应</param>
            /// <returns>AJAX回调处理</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public Action<fastCSharp.net.returnValue> Get(ajaxType ajax, response response)
            {
                this.ajax = ajax;
                this.response = response;
                ajax.setAsynchronous();
                return onReturnHandle;
            }
        }
        /// <summary>
        /// AJAX异步回调
        /// </summary>
        /// <typeparam name="callbackType">异步回调类型</typeparam>
        /// <typeparam name="ajaxType">AJAX类型</typeparam>
        /// <typeparam name="returnType">返回值类型</typeparam>
        public abstract class callback<callbackType, ajaxType, returnType>
            where callbackType : callback<callbackType, ajaxType, returnType>
            where ajaxType : webView.view
        {
            /// <summary>
            /// 当前AJAX异步回调
            /// </summary>
            private callbackType thisCallback;
            /// <summary>
            /// AJAX回调对象
            /// </summary>
            protected ajaxType ajax;
            /// <summary>
            /// HTTP响应
            /// </summary>
            protected response response;
            /// <summary>
            /// AJAX回调处理
            /// </summary>
            private Action<fastCSharp.net.returnValue<returnType>> onReturnHandle;
            /// <summary>
            /// AJAX异步回调
            /// </summary>
            protected callback()
            {
                thisCallback = (callbackType)this;
                onReturnHandle = onReturn;
            }
            /// <summary>
            /// AJAX回调处理
            /// </summary>
            /// <param name="value">回调值</param>
            private void onReturn(fastCSharp.net.returnValue<returnType> value)
            {
                try
                {
                    if (value.Type == fastCSharp.net.returnValue.type.Success)
                    {
                        ajax.cancelAsynchronous();
                        onReturnValue(value.Value);
                    }
                    else ajax.serverError500();
                }
                finally
                {
                    ajax = null;
                    response = null;
                    typePool<callbackType>.PushNotNull(thisCallback);
                }
            }
            /// <summary>
            /// AJAX回调处理
            /// </summary>
            /// <param name="value">回调值</param>
            protected abstract void onReturnValue(returnType value);
            /// <summary>
            /// 获取AJAX回调处理
            /// </summary>
            /// <param name="ajax">AJAX回调对象</param>
            /// <param name="response">HTTP响应</param>
            /// <returns>AJAX回调处理</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public Action<fastCSharp.net.returnValue<returnType>> Get(ajaxType ajax, response response)
            {
                this.ajax = ajax;
                this.response = response;
                ajax.setAsynchronous();
                return onReturnHandle;
            }
        }
        /// <summary>
        /// 默认为 false 表示代码生成选择所有函数，否则仅选择申明了 fastCSharp.code.cSharp.ajax 的函数，有效域为当前 class。
        /// </summary>
        public bool IsAttribute;
        /// <summary>
        /// 指定是否搜索该成员的继承链以查找这些特性，参考System.Reflection.MemberInfo.GetCustomAttributes(bool inherit)，有效域为当前 class。
        /// </summary>
        public bool IsBaseTypeAttribute;
        /// <summary>
        /// 成员匹配自定义属性是否可继承，true 表示允许申明 fastCSharp.code.cSharp.ajax 的派生类型并且选择继承深度最小的申明配置，有效域为当前 class。
        /// </summary>
        public bool IsInheritAttribute;
        /// <summary>
        /// AJAX 调用全名称，用于替换默认的调用全名称。
        /// </summary>
        public string FullName;
        /// <summary>
        /// 是否仅支持POST请求
        /// </summary>
        public bool IsOnlyPost = true;
        /// <summary>
        /// 默认为 true 表示需要验证 Referer 的来源页主域名是否匹配当前主域名，用于防止跨域攻击。
        /// </summary>
        public bool IsReferer = true;
        /// <summary>
        /// 默认为 false 表示不生成 TypeScript 调用代理。
        /// </summary>
        public bool IsExportTypeScript;
    }
}
