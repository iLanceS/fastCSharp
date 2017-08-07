//本文件由程序自动生成,请不要自行修改
using System;
using fastCSharp;

#if NotFastCSharpCode
#else
#pragma warning disable
namespace fastCSharp.demo.chatWeb
{
        internal partial class chat : fastCSharp.code.cSharp.webView.IWebView
        {

            /// <summary>
            /// AJAX响应输出处理
            /// </summary>
            /// <param name="js">JS输出流</param>
            protected unsafe override void ajax(charStream js)
            {
                js.WriteNotNull(@"{IsLogin:");
                    {
                        bool _value1_ = IsLogin;
                                    fastCSharp.web.ajax.ToString((bool)_value1_, js);
                    }
                    js.WriteNotNull(@",Messages:");
                    {
                        fastCSharp.demo.chatWeb.data.message[] _value1_ = Messages;
                        if (_value1_ == null) fastCSharp.web.ajax.WriteNull(js);
                        else
                        {
                            js.Write('[');
                    {
                        int _loopIndex1_ = _loopIndex_;
                        _loopIndex_ = 0;
                        foreach (fastCSharp.demo.chatWeb.data.message _value2_ in _value1_)
                        {
                            if (_loopIndex_ == 0)
                            {
                                js.Write(fastCSharp.web.ajax.Quote);
                                js.WriteNotNull("Message,Time,User");
                                js.Write(fastCSharp.web.ajax.Quote);
                            }
                            js.Write(',');
                            if (_value2_ == null) fastCSharp.web.ajax.WriteNull(js);
                            else
                            {
                                js.Write('[');
                    {
                        string _value3_ = _value2_.Message;
                                if (_value3_ == null) fastCSharp.web.ajax.WriteNull(js);
                                else
                                {
                                    fastCSharp.web.ajax.ToString(_value3_, js);
                                }
                    }
                    js.Write(',');
                    {
                        System.DateTime _value3_ = _value2_.Time;
                                    fastCSharp.web.ajax.ToString((System.DateTime)_value3_, js);
                    }
                    js.Write(',');
                    {
                        string _value3_ = _value2_.User;
                                if (_value3_ == null) fastCSharp.web.ajax.WriteNull(js);
                                else
                                {
                                    fastCSharp.web.ajax.ToString(_value3_, js);
                                }
                    }
                    js.Write(']');
                            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex1_;
                    }
                    js.WriteNotNull(@"].FormatView()");
                        }
                    }
                    js.Write('}');
            }

        }

}namespace fastCSharp.demo.chatWeb
{
        internal partial class poll : fastCSharp.code.cSharp.webView.IWebView
        {

            /// <summary>
            /// AJAX响应输出处理
            /// </summary>
            /// <param name="js">JS输出流</param>
            protected unsafe override void ajax(charStream js)
            {
                js.WriteNotNull(@"{Message:");
                    {
                        fastCSharp.demo.chatWeb.poll.message _value1_ = Message;
                        if (_value1_ == null) fastCSharp.web.ajax.WriteNull(js);
                        else
                        {
                            js.WriteNotNull(@"{Messages:");
                    {
                        fastCSharp.demo.chatWeb.data.message[] _value2_ = _value1_.Messages;
                        if (_value2_ == null) fastCSharp.web.ajax.WriteNull(js);
                        else
                        {
                            js.Write('[');
                    {
                        int _loopIndex2_ = _loopIndex_;
                        _loopIndex_ = 0;
                        foreach (fastCSharp.demo.chatWeb.data.message _value3_ in _value2_)
                        {
                            if (_loopIndex_ == 0)
                            {
                                js.Write(fastCSharp.web.ajax.Quote);
                                js.WriteNotNull("Message,Time,User");
                                js.Write(fastCSharp.web.ajax.Quote);
                            }
                            js.Write(',');
                            if (_value3_ == null) fastCSharp.web.ajax.WriteNull(js);
                            else
                            {
                                js.Write('[');
                    {
                        string _value4_ = _value3_.Message;
                                if (_value4_ == null) fastCSharp.web.ajax.WriteNull(js);
                                else
                                {
                                    fastCSharp.web.ajax.ToString(_value4_, js);
                                }
                    }
                    js.Write(',');
                    {
                        System.DateTime _value4_ = _value3_.Time;
                                    fastCSharp.web.ajax.ToString((System.DateTime)_value4_, js);
                    }
                    js.Write(',');
                    {
                        string _value4_ = _value3_.User;
                                if (_value4_ == null) fastCSharp.web.ajax.WriteNull(js);
                                else
                                {
                                    fastCSharp.web.ajax.ToString(_value4_, js);
                                }
                    }
                    js.Write(']');
                            }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex2_;
                    }
                    js.WriteNotNull(@"].FormatView()");
                        }
                    }
                    js.WriteNotNull(@",Users:");
                    {
                        string[] _value2_ = _value1_.Users;
                        if (_value2_ == null) fastCSharp.web.ajax.WriteNull(js);
                        else
                        {
                            js.Write('[');
                    {
                        int _loopIndex2_ = _loopIndex_;
                        _loopIndex_ = 0;
                        foreach (string _value3_ in _value2_)
                        {
                            if (_loopIndex_ != 0) js.Write(',');
                                if (_value3_ == null) fastCSharp.web.ajax.WriteNull(js);
                                else
                                {
                                    fastCSharp.web.ajax.ToString(_value3_, js);
                                }
                            ++_loopIndex_;
                        }
                        _loopIndex_ = _loopIndex2_;
                    }
                    js.Write(']');
                        }
                    }
                    js.WriteNotNull(@",UserVersion:");
                    {
                        int _value2_ = _value1_.UserVersion;
                                    fastCSharp.web.ajax.ToString((int)_value2_, js);
                    }
                    js.Write('}');
                        }
                    }
                    js.Write('}');
            }

            private struct webViewQuery
            {
                [fastCSharp.emit.jsonParse.member(IsDefault = true)]
                public string verify;
                public fastCSharp.demo.chatWeb.poll.clientQuery query;
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
                        return loadView(query.verify, query.query);
                    }
                }
                return false;
            }
        }

}
namespace fastCSharp.demo.chatWeb
{


        /// <summary>
        /// WEB服务器
        /// </summary>
        public partial class webServer : fastCSharp.net.tcp.http.domainServer.viewServer<string>
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
                    names[--count] = "/chat";
                    views[count] = "/chat.html";
                    names[--count] = "/poll";
                    views[count] = "/poll.html";
                    return new keyValue<string[], string[]>(names, views);
                }
            }
            /// <summary>
            /// 网站生成配置
            /// </summary>
            internal new static readonly fastCSharp.code.webConfig WebConfig = new fastCSharp.demo.chatWeb.webConfig();
            /// <summary>
            /// 网站生成配置
            /// </summary>
            /// <returns>网站生成配置</returns>
            protected override fastCSharp.code.webConfig getWebConfig() { return WebConfig; }
        }
}namespace fastCSharp.demo.chatWeb
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
                            fastCSharp.demo.chatWeb.ajax.pub view = fastCSharp.typePool<fastCSharp.demo.chatWeb.ajax.pub>.Pop() ?? new fastCSharp.demo.chatWeb.ajax.pub();
                            _p0 parameter = new _p0();
                            fastCSharp.net.tcp.http.response response = loader.Load(view, ref parameter, true);
                            if (response != null)
                            {
                                int responseIdentity = view.ResponseIdentity;
                                _a0 callback = typePool<_a0>.Pop() ?? new _a0();
                                callback.Parameter = parameter;
                                view.CrawlTitle(parameter.link, callback.Get(view, response));
                            }
                        }
                        return;
                    case 1:
                        {
                            fastCSharp.demo.chatWeb.ajax.pub view = fastCSharp.typePool<fastCSharp.demo.chatWeb.ajax.pub>.Pop() ?? new fastCSharp.demo.chatWeb.ajax.pub();
                            _p1 parameter = new _p1();
                            fastCSharp.net.tcp.http.response response = loader.Load(view, ref parameter, true);
                            if (response != null)
                            {
                                int responseIdentity = view.ResponseIdentity;
                                try
                                {
                                    
                                    parameter.Return =  view.PasteImage(ref parameter.identity);
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
                            fastCSharp.demo.chatWeb.ajax.user view = fastCSharp.typePool<fastCSharp.demo.chatWeb.ajax.user>.Pop() ?? new fastCSharp.demo.chatWeb.ajax.user();
                            _p2 parameter = new _p2();
                            fastCSharp.net.tcp.http.response response = loader.Load(view, ref parameter, true);
                            if (response != null)
                            {
                                int responseIdentity = view.ResponseIdentity;
                                try
                                {
                                    
                                    parameter.Return =  view.Login(parameter.user, ref parameter.version);
                                }
                                finally
                                {
                                    if (responseIdentity == view.ResponseIdentity) view.AjaxResponse(ref parameter, ref response);
                                    else view.AjaxResponse(ref response);
                                }
                            }
                        }
                        return;
                    case 3:
                        {
                            fastCSharp.demo.chatWeb.ajax.user view = fastCSharp.typePool<fastCSharp.demo.chatWeb.ajax.user>.Pop() ?? new fastCSharp.demo.chatWeb.ajax.user();
                            _p3 parameter = new _p3();
                            fastCSharp.net.tcp.http.response response = loader.Load(view, ref parameter, true);
                            if (response != null)
                            {
                                int responseIdentity = view.ResponseIdentity;
                                try
                                {
                                     view.Logout(parameter.user);
                                }
                                finally
                                {
                                    view.AjaxResponse(ref response);
                                }
                            }
                        }
                        return;
                    case 4:
                        {
                            fastCSharp.demo.chatWeb.ajax.user view = fastCSharp.typePool<fastCSharp.demo.chatWeb.ajax.user>.Pop() ?? new fastCSharp.demo.chatWeb.ajax.user();
                            _p4 parameter = new _p4();
                            fastCSharp.net.tcp.http.response response = loader.Load(view, ref parameter, true);
                            if (response != null)
                            {
                                int responseIdentity = view.ResponseIdentity;
                                try
                                {
                                     view.Send(parameter.user, parameter.message, parameter.users);
                                }
                                finally
                                {
                                    view.AjaxResponse(ref response);
                                }
                            }
                        }
                        return;
                    case 5: loader.LoadView(fastCSharp.typePool<fastCSharp.demo.chatWeb.chat>.Pop() ??new fastCSharp.demo.chatWeb.chat(), true); return;
                    case 6: loader.LoadView(fastCSharp.typePool<fastCSharp.demo.chatWeb.poll>.Pop() ??new fastCSharp.demo.chatWeb.poll(), true); return;
                    case 8 - 1: pubError(loader); return;
                }
            }
            [fastCSharp.emit.jsonSerialize(IsAllMember = false)]
            [fastCSharp.emit.jsonParse(IsAllMember = false)]
            struct _p0
            {
                [fastCSharp.emit.jsonParse.member]
                public string link;
                [fastCSharp.emit.jsonSerialize.member]
                public string Return;
            }
            sealed class _a0 : fastCSharp.code.cSharp.ajax.callback<_a0, fastCSharp.demo.chatWeb.ajax.pub, string>
            {
                public _p0 Parameter;
                protected override void onReturnValue(string value)
                {
                    
                    Parameter.Return = value;
                    ajax.AjaxResponse(ref Parameter, ref response);
                }
            }
            [fastCSharp.emit.jsonSerialize(IsAllMember = false)]
            [fastCSharp.emit.jsonParse(IsAllMember = false)]
            struct _p1
            {
                [fastCSharp.emit.jsonSerialize.member]
                [fastCSharp.emit.jsonParse.member]
                public int identity;
                [fastCSharp.emit.jsonSerialize.member]
                public string[] Return;
            }
            [fastCSharp.emit.jsonSerialize(IsAllMember = false)]
            [fastCSharp.emit.jsonParse(IsAllMember = false)]
            struct _p2
            {
                [fastCSharp.emit.jsonParse.member]
                public string user;
                [fastCSharp.emit.jsonSerialize.member]
                [fastCSharp.emit.jsonParse.member]
                public int version;
                [fastCSharp.emit.jsonSerialize.member]
                public string[] Return;
            }
            [fastCSharp.emit.jsonSerialize(IsAllMember = false)]
            [fastCSharp.emit.jsonParse(IsAllMember = false)]
            struct _p3
            {
                [fastCSharp.emit.jsonParse.member]
                public string user;
            }
            [fastCSharp.emit.jsonSerialize(IsAllMember = false)]
            [fastCSharp.emit.jsonParse(IsAllMember = false)]
            struct _p4
            {
                [fastCSharp.emit.jsonParse.member]
                public string user;
                [fastCSharp.emit.jsonParse.member]
                public string message;
                [fastCSharp.emit.jsonParse.member]
                public string[] users;
            }
            static ajaxLoader()
            {
                string[] names = new string[8];
                fastCSharp.code.cSharp.ajax.call[] callMethods = new fastCSharp.code.cSharp.ajax.call[8];
                names[0] = "pub.CrawlTitle";
                callMethods[0] = new fastCSharp.code.cSharp.ajax.call(0, 4194304, 65536, false, true);
                names[1] = "pub.PasteImage";
                callMethods[1] = new fastCSharp.code.cSharp.ajax.call(1, 4194304, 65536, false, true);
                names[2] = "user.Login";
                callMethods[2] = new fastCSharp.code.cSharp.ajax.call(2, 4194304, 65536, false, true);
                names[3] = "user.Logout";
                callMethods[3] = new fastCSharp.code.cSharp.ajax.call(3, 4194304, 65536, false, true);
                names[4] = "user.Send";
                callMethods[4] = new fastCSharp.code.cSharp.ajax.call(4, 4194304, 65536, false, true);
                names[5] = "/chat.html";
                callMethods[5] = new fastCSharp.code.cSharp.ajax.call(5, 4194304, 65536, true, false);
                names[6] = "/poll.html";
                callMethods[6] = new fastCSharp.code.cSharp.ajax.call(6, 4194304, 65536, true, false);
                names[8 - 1] = fastCSharp.code.cSharp.ajax.PubErrorCallName;
                callMethods[8 - 1] = new fastCSharp.code.cSharp.ajax.call(8 - 1, 2048, 0, false, false);
                methods = new fastCSharp.stateSearcher.ascii<fastCSharp.code.cSharp.ajax.call>(names, callMethods, true);
            }
        }
}namespace fastCSharp.demo.chatWeb
{

        /// <summary>
        /// WEB服务器
        /// </summary>
        public partial class webServer : fastCSharp.net.tcp.http.domainServer.viewServer<string>
        {
            /// <summary>
            /// WEB调用处理索引集合
            /// </summary>
            protected override string[] calls
            {
                get
                {
                    string[] names = new string[2];
                    names[0] = "/ajax";
                    names[1] = "/";
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
                        loadAjax<_c0, fastCSharp.demo.chatWeb.ajaxLoader>(socket, socketIdentity, _c0/**/.Get(),  fastCSharp.typePool<fastCSharp.demo.chatWeb.ajaxLoader>.Pop() ?? new fastCSharp.demo.chatWeb.ajaxLoader());
                        return;
                    case 1:
                        load<_c1, fastCSharp.demo.chatWeb.index>(socket, socketIdentity, _c1/**/.Get(),  fastCSharp.typePool<fastCSharp.demo.chatWeb.index>.Pop() ?? new fastCSharp.demo.chatWeb.index(), 4194304, 65536, false, true);
                        return;
                }
            }
            private sealed class _c0 : fastCSharp.code.cSharp.webCall.callPool<_c0, fastCSharp.demo.chatWeb.ajaxLoader>
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
            private sealed class _c1 : fastCSharp.code.cSharp.webCall.callPool<_c1, fastCSharp.demo.chatWeb.index>
            {
                private _c1() : base() { }
                public override bool Call()
                {
                    try
                    {
                            {
                                WebCall.load();
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