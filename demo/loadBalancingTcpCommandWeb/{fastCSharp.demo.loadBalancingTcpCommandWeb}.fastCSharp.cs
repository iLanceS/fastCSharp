//本文件由程序自动生成,请不要自行修改
using System;
using fastCSharp;

#if NotFastCSharpCode
#else
#pragma warning disable
namespace fastCSharp.demo.loadBalancingTcpCommandWeb
{
        internal partial class webView : fastCSharp.code.cSharp.webView.IWebView
        {
            /// <summary>
            /// HTTP请求表单处理
            /// </summary>
            /// <returns>是否成功</returns>
            protected override bool response()
            {
                if (isLoadHtml(@"webView.html", 2))
                {
                    
                    response(htmls[0]);
                        response(Return.ToString());
                    response(htmls[1]);
                    return true;
                }
                return false;
            }

            /// <summary>
            /// AJAX响应输出处理
            /// </summary>
            /// <param name="js">JS输出流</param>
            protected unsafe override void ajax(charStream js)
            {
                js.WriteNotNull(@"{Return:");
                    {
                        int _value1_ = Return;
                                    fastCSharp.web.ajax.ToString((int)_value1_, js);
                    }
                    js.Write('}');
            }

            private struct webViewQuery
            {
                [fastCSharp.emit.jsonParse.member(IsDefault = true)]
                public int left;
                public int right;
                public bool isAsynchronous;
            }
            /// <summary>
            /// 查询参数
            /// </summary>
            private webViewQuery query;
            /// <summary>
            /// WEB视图加载
            /// </summary>
            /// <returns>是否成功</returns>
            protected override bool loadView()
            {
                if (base.loadView())
                {
                    
                    query= default(webViewQuery);
                    if (ParseParameter(ref query))
                    {
                        return loadView(query.left, query.right, query.isAsynchronous);
                    }
                }
                return false;
            }
        }

}
namespace fastCSharp.demo.loadBalancingTcpCommandWeb
{


        /// <summary>
        /// WEB服务器
        /// </summary>
        public partial class webServer : fastCSharp.net.tcp.http.domainServer.viewServer<int>
        {
            
            /// <summary>
            /// WEB视图URL重写路径集合
            /// </summary>
            protected override keyValue<string[], string[]> rewrites
            {
                get
                {
                    int count = 1 + 0 * 2;
                    string[] names = new string[count];
                    string[] views = new string[count];
                    names[--count] = "/webView";
                    views[count] = "/webView.html";
                    return new keyValue<string[], string[]>(names, views);
                }
            }
            /// <summary>
            /// WEB视图URL重写索引集合
            /// </summary>
            protected override string[] viewRewrites
            {
                get
                {
                    string[] names = new string[1];
                    names[0] = "/webView";
                    return names;
                }
            }
            /// <summary>
            /// WEB视图页面索引集合
            /// </summary>
            protected override string[] views
            {
                get
                {
                    string[] names = new string[1];
                    names[0] = "/webView.html";
                    return names;
                }
            }
            /// <summary>
            /// 视图页面处理
            /// </summary>
            /// <param name="viewIndex"></param>
            /// <param name="socket"></param>
            /// <param name="socketIdentity"></param>
            protected override void request(int viewIndex, fastCSharp.net.tcp.http.socketBase socket, long socketIdentity)
            {
                switch (viewIndex)
                {
                    case 0: load(socket, socketIdentity, fastCSharp.typePool<fastCSharp.demo.loadBalancingTcpCommandWeb.webView>.Pop() ?? new fastCSharp.demo.loadBalancingTcpCommandWeb.webView(), true); return;
                }
            }
            /// <summary>
            /// 网站生成配置
            /// </summary>
            internal new static readonly fastCSharp.code.webConfig WebConfig = new fastCSharp.demo.loadBalancingTcpCommandWeb.webConfig();
            /// <summary>
            /// 网站生成配置
            /// </summary>
            /// <returns>网站生成配置</returns>
            protected override fastCSharp.code.webConfig getWebConfig() { return WebConfig; }
        }
}namespace fastCSharp.demo.loadBalancingTcpCommandWeb
{

        /// <summary>
        /// AJAX函数调用
        /// </summary>
        [fastCSharp.code.cSharp.webCall(IsPool = true)]
        [fastCSharp.emit.webView.clearMember(IsIgnoreCurrent = true)]
        public sealed class ajaxLoader : fastCSharp.code.cSharp.ajax.loader<ajaxLoader>
        {
            /// <summary>
            /// AJAX函数调用集合
            /// </summary>
            private static readonly fastCSharp.stateSearcher.ascii<fastCSharp.code.cSharp.ajax.call> methods;
            /// <summary>
            /// AJAX调用
            /// </summary>
            [fastCSharp.code.cSharp.webCall(FullName = "/ajax")]
            public void Load()
            {
                load(methods);
            }
            /// <summary>
            /// AJAX调用
            /// </summary>
            /// <param name="callIndex"></param>
            /// <param name="loader"></param>
            protected override void callAjax(int callIndex, fastCSharp.code.cSharp.ajax.loader loader)
            {
                switch (callIndex)
                {
                    case 0:
                        {
                            fastCSharp.demo.loadBalancingTcpCommandWeb.ajax.loadBalancing view = fastCSharp.typePool<fastCSharp.demo.loadBalancingTcpCommandWeb.ajax.loadBalancing>.Pop() ?? new fastCSharp.demo.loadBalancingTcpCommandWeb.ajax.loadBalancing();
                            _p0 parameter = new _p0();
                            fastCSharp.net.tcp.http.response response = loader.Load(view, true);
                            if (response != null)
                            {
                                int responseIdentity = view.ResponseIdentity;
                                try
                                {
                                    
                                    parameter.Return =  view.Start();
                                }
                                finally
                                {
                                    if (responseIdentity == view.ResponseIdentity) view.AjaxResponse(ref parameter, ref response);
                                    else view.AjaxResponse(ref response);
                                }
                            }
                        }
                        return;
                    case 1:
                        {
                            fastCSharp.demo.loadBalancingTcpCommandWeb.ajax.loadBalancing view = fastCSharp.typePool<fastCSharp.demo.loadBalancingTcpCommandWeb.ajax.loadBalancing>.Pop() ?? new fastCSharp.demo.loadBalancingTcpCommandWeb.ajax.loadBalancing();
                            _p1 parameter = new _p1();
                            fastCSharp.net.tcp.http.response response = loader.Load(view, ref parameter, true);
                            if (response != null)
                            {
                                int responseIdentity = view.ResponseIdentity;
                                try
                                {
                                    
                                    parameter.Return =  view.Add(parameter.left, parameter.right);
                                }
                                finally
                                {
                                    if (responseIdentity == view.ResponseIdentity) view.AjaxResponse(ref parameter, ref response);
                                    else view.AjaxResponse(ref response);
                                }
                            }
                        }
                        return;
                    case 2:
                        {
                            fastCSharp.demo.loadBalancingTcpCommandWeb.ajax.loadBalancing view = fastCSharp.typePool<fastCSharp.demo.loadBalancingTcpCommandWeb.ajax.loadBalancing>.Pop() ?? new fastCSharp.demo.loadBalancingTcpCommandWeb.ajax.loadBalancing();
                            _p2 parameter = new _p2();
                            fastCSharp.net.tcp.http.response response = loader.Load(view, ref parameter, true);
                            if (response != null)
                            {
                                int responseIdentity = view.ResponseIdentity;
                                _a2 callback = typePool<_a2>.Pop() ?? new _a2();
                                callback.Parameter = parameter;
                                view.Xor(parameter.left, parameter.right, callback.Get(view, response));
                            }
                        }
                        return;
                    case 3: loader.LoadView(fastCSharp.typePool<fastCSharp.demo.loadBalancingTcpCommandWeb.webView>.Pop() ??new fastCSharp.demo.loadBalancingTcpCommandWeb.webView(), true); return;
                    case 5 - 1: pubError(loader); return;
                }
            }
            [fastCSharp.emit.jsonSerialize(IsAllMember = false)]
            [fastCSharp.emit.jsonParse(IsAllMember = false)]
            struct _p0
            {
                [fastCSharp.emit.jsonSerialize.member]
                public int Return;
            }
            [fastCSharp.emit.jsonSerialize(IsAllMember = false)]
            [fastCSharp.emit.jsonParse(IsAllMember = false)]
            struct _p1
            {
                [fastCSharp.emit.jsonParse.member]
                public int left;
                [fastCSharp.emit.jsonParse.member]
                public int right;
                [fastCSharp.emit.jsonSerialize.member]
                public int Return;
            }
            [fastCSharp.emit.jsonSerialize(IsAllMember = false)]
            [fastCSharp.emit.jsonParse(IsAllMember = false)]
            struct _p2
            {
                [fastCSharp.emit.jsonParse.member]
                public int left;
                [fastCSharp.emit.jsonParse.member]
                public int right;
                [fastCSharp.emit.jsonSerialize.member]
                public int Return;
            }
            sealed class _a2 : fastCSharp.code.cSharp.ajax.callback<_a2, fastCSharp.demo.loadBalancingTcpCommandWeb.ajax.loadBalancing, int>
            {
                public _p2 Parameter;
                protected override void onReturnValue(int value)
                {
                    
                    Parameter.Return = value;
                    ajax.AjaxResponse(ref Parameter, ref response);
                }
            }
            static ajaxLoader()
            {
                string[] names = new string[5];
                fastCSharp.code.cSharp.ajax.call[] callMethods = new fastCSharp.code.cSharp.ajax.call[5];
                names[0] = "loadBalancing.Start";
                callMethods[0] = new fastCSharp.code.cSharp.ajax.call(0, 4194304, 65536, false, false);
                names[1] = "loadBalancing.Add";
                callMethods[1] = new fastCSharp.code.cSharp.ajax.call(1, 4194304, 65536, false, false, false);
                names[2] = "loadBalancing.Xor";
                callMethods[2] = new fastCSharp.code.cSharp.ajax.call(2, 4194304, 65536, false, false, false);
                names[3] = "/webView.html";
                callMethods[3] = new fastCSharp.code.cSharp.ajax.call(3, 4194304, 65536, true, false, false);
                names[5 - 1] = fastCSharp.code.cSharp.ajax.PubErrorCallName;
                callMethods[5 - 1] = new fastCSharp.code.cSharp.ajax.call(5 - 1, 2048, 0, false, false);
                methods = new fastCSharp.stateSearcher.ascii<fastCSharp.code.cSharp.ajax.call>(names, callMethods, true);
            }
        }
}namespace fastCSharp.demo.loadBalancingTcpCommandWeb
{

        /// <summary>
        /// WEB服务器
        /// </summary>
        public partial class webServer : fastCSharp.net.tcp.http.domainServer.viewServer<int>
        {
            /// <summary>
            /// WEB调用处理索引集合
            /// </summary>
            protected override string[] calls
            {
                get
                {
                    string[] names = new string[3];
                    names[0] = "/ajax";
                    names[1] = "/webCall/Add";
                    names[2] = "/webCall/Xor";
                    return names;
                }
            }
            /// <summary>
            /// WEB调用处理
            /// </summary>
            /// <param name="callIndex"></param>
            /// <param name="socket"></param>
            /// <param name="socketIdentity"></param>
            protected override void call(int callIndex, fastCSharp.net.tcp.http.socketBase socket, long socketIdentity)
            {
                switch (callIndex)
                {
                    case 0:
                        loadAjax<_c0, fastCSharp.demo.loadBalancingTcpCommandWeb.ajaxLoader>(socket, socketIdentity, _c0/**/.Get(),  fastCSharp.typePool<fastCSharp.demo.loadBalancingTcpCommandWeb.ajaxLoader>.Pop() ?? new fastCSharp.demo.loadBalancingTcpCommandWeb.ajaxLoader());
                        return;
                    case 1:
                        load<_c1, fastCSharp.demo.loadBalancingTcpCommandWeb.webCall>(socket, socketIdentity, _c1/**/.Get(),  fastCSharp.typePool<fastCSharp.demo.loadBalancingTcpCommandWeb.webCall>.Pop() ?? new fastCSharp.demo.loadBalancingTcpCommandWeb.webCall(), 4194304, 65536, false, true);
                        return;
                    case 2:
                        load<_c2, fastCSharp.demo.loadBalancingTcpCommandWeb.webCall>(socket, socketIdentity, _c2/**/.Get(),  fastCSharp.typePool<fastCSharp.demo.loadBalancingTcpCommandWeb.webCall>.Pop() ?? new fastCSharp.demo.loadBalancingTcpCommandWeb.webCall(), 4194304, 65536, false, true);
                        return;
                }
            }
            private sealed class _c0 : fastCSharp.code.cSharp.webCall.callPool<_c0, fastCSharp.demo.loadBalancingTcpCommandWeb.ajaxLoader>
            {
                private _c0() : base() { }
                public override bool Call()
                {
                    try
                    {
                            {
                                WebCall.Load();
                                return true;
                            }
                    }
                    finally
                    {
                        WebCall = null;
                        typePool<_c0>.PushNotNull(this);
                    }
                }
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public static _c0 Get()
                {
                    _c0 call = fastCSharp.typePool<_c0>.Pop();
                    if (call == null) call = new _c0();
                    return call;
                }
            }
            struct _p1
            {
                public int left;
                public int right;
            }
            private sealed class _c1 : fastCSharp.code.cSharp.webCall.callPool<_c1, fastCSharp.demo.loadBalancingTcpCommandWeb.webCall, _p1>
            {
                private _c1() : base() { }
                public override bool Call()
                {
                    try
                    {
                            if (WebCall.ParseParameter(ref Parameter))
                            {
                                WebCall.Add(Parameter.left, Parameter.right);
                                return true;
                            }
                    }
                    finally
                    {
                        WebCall = null;
                        typePool<_c1>.PushNotNull(this);
                    }
                    return false;
                }
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public static _c1 Get()
                {
                    _c1 call = fastCSharp.typePool<_c1>.Pop();
                    if (call == null) call = new _c1();
                    else call.Parameter = new _p1();
                    return call;
                }
            }
            struct _p2
            {
                public int left;
                public int right;
            }
            private sealed class _c2 : fastCSharp.code.cSharp.webCall.callPool<_c2, fastCSharp.demo.loadBalancingTcpCommandWeb.webCall, _p2>
            {
                private _c2() : base() { }
                public override bool Call()
                {
                    try
                    {
                            if (WebCall.ParseParameter(ref Parameter))
                            {
                                WebCall.Xor(Parameter.left, Parameter.right);
                                return true;
                            }
                    }
                    finally
                    {
                        WebCall = null;
                        typePool<_c2>.PushNotNull(this);
                    }
                    return false;
                }
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public static _c2 Get()
                {
                    _c2 call = fastCSharp.typePool<_c2>.Pop();
                    if (call == null) call = new _c2();
                    else call.Parameter = new _p2();
                    return call;
                }
            }
        }
}
#endif