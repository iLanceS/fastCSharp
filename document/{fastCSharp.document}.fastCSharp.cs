//本文件由程序自动生成,请不要自行修改
using System;
using fastCSharp;

#if NotFastCSharpCode
#else
#pragma warning disable
namespace fastCSharp.document.include
{
        internal partial class codeMenu : fastCSharp.code.cSharp.webView.IWebView
        {

            /// <summary>
            /// AJAX响应输出处理
            /// </summary>
            /// <param name="js">JS输出流</param>
            protected unsafe override void ajax(charStream js)
            {
                js.WriteNotNull(@"{Item:");
                    {
                        fastCSharp.document.include.codeMenu.item _value1_ = Item;
                        if (_value1_ == null) fastCSharp.web.ajax.WriteNull(js);
                        else
                        {
                            js.WriteNotNull(@"{File:");
                    {
                        System.IO.FileInfo _value2_ = _value1_.File;
                        if (_value2_ == null) fastCSharp.web.ajax.WriteNull(js);
                        else
                        {
                            js.WriteNotNull(@"{FullName:");
                    {
                        string _value3_ = _value2_.FullName;
                        if (_value3_ == null) fastCSharp.web.ajax.WriteNull(js);
                        else
                        {
                                    fastCSharp.web.ajax.ToString(_value3_, js);
                        }
                    }
                    js.Write('}');
                        }
                    }
                    js.Write('}');
                        }
                    }
                    js.Write('}');
            }

            private struct webViewQuery
            {
                [fastCSharp.emit.jsonParse.member(IsDefault = true)]
                public string file;
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
                        return loadView(query.file);
                    }
                }
                return false;
            }
        }

}namespace fastCSharp.document
{
        internal partial class index : fastCSharp.code.cSharp.webView.IWebView
        {

            /// <summary>
            /// AJAX响应输出处理
            /// </summary>
            /// <param name="js">JS输出流</param>
            protected unsafe override void ajax(charStream js)
            {
                js.WriteNotNull(@"{At:");
                    {
                        string _value1_ = At;
                        if (_value1_ == null) fastCSharp.web.ajax.WriteNull(js);
                        else
                        {
                                    fastCSharp.web.ajax.ToString(_value1_, js);
                        }
                    }
                    js.WriteNotNull(@",Environment:");
                    {
                        fastCSharp.document.environment _value1_ = Environment;
                        if (_value1_ == null) fastCSharp.web.ajax.WriteNull(js);
                        else
                        {
                            js.WriteNotNull(@"{FastCSharpPath:");
                    {
                        string _value2_ = _value1_.FastCSharpPath;
                        if (_value2_ == null) fastCSharp.web.ajax.WriteNull(js);
                        else
                        {
                                    fastCSharp.web.ajax.ToString(_value2_, js);
                        }
                    }
                    js.WriteNotNull(@",VS2010:");
                    {
                        bool _value2_ = _value1_.VS2010;
                                    fastCSharp.web.ajax.ToString((bool)_value2_, js);
                    }
                    js.Write('}');
                        }
                    }
                    js.WriteNotNull(@",WorkPath:");
                    {
                        string _value1_ = WorkPath;
                        if (_value1_ == null) fastCSharp.web.ajax.WriteNull(js);
                        else
                        {
                                    fastCSharp.web.ajax.ToString(_value1_, js);
                        }
                    }
                    js.Write('}');
            }

        }

}
namespace fastCSharp.document
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
                    int count = 2 + 0 * 2;
                    string[] names = new string[count];
                    string[] views = new string[count];
                    names[--count] = "/include/codeMenu";
                    views[count] = "/include/codeMenu.html";
                    names[--count] = "/";
                    views[count] = "/index.html";
                    return new keyValue<string[], string[]>(names, views);
                }
            }
            /// <summary>
            /// 网站生成配置
            /// </summary>
            internal new static readonly fastCSharp.code.webConfig WebConfig = new fastCSharp.document.webConfig();
            /// <summary>
            /// 网站生成配置
            /// </summary>
            /// <returns>网站生成配置</returns>
            protected override fastCSharp.code.webConfig getWebConfig() { return WebConfig; }
        }
}namespace fastCSharp.document
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
                            fastCSharp.document.ajax.pub view = fastCSharp.typePool<fastCSharp.document.ajax.pub>.Pop() ?? new fastCSharp.document.ajax.pub();
                            _p0 parameter = new _p0();
                            fastCSharp.net.tcp.http.response response = loader.Load(view, ref parameter, true);
                            if (response != null)
                            {
                                int responseIdentity = view.ResponseIdentity;
                                try
                                {
                                     view.OpenFile(parameter.file);
                                }
                                finally
                                {
                                    view.AjaxResponse(ref response);
                                }
                            }
                        }
                        return;
                    case 1: loader.LoadView(fastCSharp.typePool<fastCSharp.document.include.codeMenu>.Pop() ??new fastCSharp.document.include.codeMenu(), true); return;
                    case 2: loader.LoadView(fastCSharp.typePool<fastCSharp.document.index>.Pop() ??new fastCSharp.document.index(), true); return;
                    case 4 - 1: pubError(loader); return;
                }
            }
            [fastCSharp.emit.jsonSerialize(IsAllMember = false)]
            [fastCSharp.emit.jsonParse(IsAllMember = false)]
            struct _p0
            {
                [fastCSharp.emit.jsonParse.member]
                public string file;
            }
            static ajaxLoader()
            {
                string[] names = new string[4];
                fastCSharp.code.cSharp.ajax.call[] callMethods = new fastCSharp.code.cSharp.ajax.call[4];
                names[0] = "pub.OpenFile";
                callMethods[0] = new fastCSharp.code.cSharp.ajax.call(0, 4194304, 65536, false, true);
                names[1] = "/include/codeMenu.html";
                callMethods[1] = new fastCSharp.code.cSharp.ajax.call(1, 4194304, 65536, true, false, false);
                names[2] = "/index.html";
                callMethods[2] = new fastCSharp.code.cSharp.ajax.call(2, 4194304, 65536, true, false, false);
                names[4 - 1] = fastCSharp.code.cSharp.ajax.PubErrorCallName;
                callMethods[4 - 1] = new fastCSharp.code.cSharp.ajax.call(4 - 1, 2048, 0, false, false);
                methods = new fastCSharp.stateSearcher.ascii<fastCSharp.code.cSharp.ajax.call>(names, callMethods, true);
            }
        }
}namespace fastCSharp.document
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
                    string[] names = new string[2];
                    names[0] = "/";
                    names[1] = "/ajax";
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
                        load<_c0, fastCSharp.document.Index>(socket, socketIdentity, _c0/**/.Get(),  fastCSharp.typePool<fastCSharp.document.Index>.Pop() ?? new fastCSharp.document.Index(), 4194304, 65536, false, true);
                        return;
                    case 1:
                        loadAjax<_c1, fastCSharp.document.ajaxLoader>(socket, socketIdentity, _c1/**/.Get(),  fastCSharp.typePool<fastCSharp.document.ajaxLoader>.Pop() ?? new fastCSharp.document.ajaxLoader());
                        return;
                }
            }
            private sealed class _c0 : fastCSharp.code.cSharp.webCall.callPool<_c0, fastCSharp.document.Index>
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
            private sealed class _c1 : fastCSharp.code.cSharp.webCall.callPool<_c1, fastCSharp.document.ajaxLoader>
            {
                private _c1() : base() { }
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
                        typePool<_c1>.PushNotNull(this);
                    }
                }
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public static _c1 Get()
                {
                    _c1 call = fastCSharp.typePool<_c1>.Pop();
                    if (call == null) call = new _c1();
                    return call;
                }
            }
        }
}
#endif