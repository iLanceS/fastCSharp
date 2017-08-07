using System;
#pragma warning disable 649

namespace fastCSharp.code.cSharp.template
{
    class webView : pub
    {
        #region PART CLASS
        #region NOT IsServer
        /*NOTE*/
        public abstract partial class /*NOTE*/@TypeNameDefinition : /*NOTE*/fastCSharp.code.cSharp.webView.view<TypeNameDefinition>, /*NOTE*/fastCSharp.code.cSharp.webView.IWebView
        {
            #region NOTE
            public const int HtmlCount = 0;
            #endregion NOTE
            #region IF Attribute.IsPage
            /// <summary>
            /// HTTP请求表单处理
            /// </summary>
            /// <returns>是否成功</returns>
            protected override bool response()
            {
                if (isLoadHtml(@"@HtmlFile", @HtmlCount))
                {
                    /*AT:PageCode*/
                    return true;
                }
                return false;
            }
            #endregion IF Attribute.IsPage

            #region IF Attribute.IsAjax
            /// <summary>
            /// AJAX响应输出处理
            /// </summary>
            /// <param name="js">JS输出流</param>
            protected unsafe override void ajax(charStream js)
            {
                /*AT:AjaxCode*/
            }
            #endregion IF Attribute.IsAjax

            #region IF LoadMethod
            private struct webViewQuery
            {
                #region IF LoadMethod.Parameters.Length
                [fastCSharp.emit.jsonParse.member(IsDefault = true)]
                #endregion IF LoadMethod.Parameters.Length
                #region LOOP LoadMethod.Parameters
                #region IF XmlDocument
                /// <summary>
                /// @XmlDocument
                /// </summary>
                #endregion IF XmlDocument
                public @ParameterType.FullName @ParameterName;
                #endregion LOOP LoadMethod.Parameters
                #region LOOP QueryMembers
                #region IF XmlDocument
                /// <summary>
                /// @XmlDocument
                /// </summary>
                #endregion IF XmlDocument
                public @MemberType.FullName @MemberName;
                #endregion LOOP QueryMembers
                #region NOTE
                public object ParameterJoinName;
                #endregion NOTE
            }
            #region IF LoadAttribute.QueryName
            /// <summary>
            /// 查询参数
            /// </summary>
            private webViewQuery /*PUSH:LoadAttribute*/@QueryName/*PUSH:LoadAttribute*/;
            #endregion IF LoadAttribute.QueryName
            /// <summary>
            /// WEB视图加载
            /// </summary>
            /// <returns>是否成功</returns>
            protected override bool loadView()
            {
                if (base.loadView())
                {
                    #region NOTE
                    MemberType.FullName MemberName;
                    #endregion NOTE
                    #region IF LoadAttribute.QueryName
                    /*PUSH:LoadAttribute*/
                    @QueryName/*PUSH:LoadAttribute*/= default(webViewQuery);
                    if (ParseParameter(ref /*PUSH:LoadAttribute*/@QueryName/*PUSH:LoadAttribute*/))
                    {
                        #region LOOP QueryMembers
                        @MemberName = /*PUSH:LoadAttribute*/@QueryName/*PUSH:LoadAttribute*/.@MemberName;
                        #endregion LOOP QueryMembers
                        return loadView(/*LOOP:LoadMethod.Parameters*//*PUSH:LoadAttribute*/@QueryName/*PUSH:LoadAttribute*/.@ParameterJoinName/*LOOP:LoadMethod.Parameters*/);
                    }
                    #endregion IF LoadAttribute.QueryName
                    #region NOT LoadAttribute.QueryName
                    webViewQuery query = new webViewQuery();
                    if (ParseParameter(ref query))
                    {
                        #region LOOP QueryMembers
                        @MemberName = query.@MemberName;
                        #endregion LOOP QueryMembers
                        return loadView(/*LOOP:LoadMethod.Parameters*/query.@ParameterJoinName/*LOOP:LoadMethod.Parameters*/);
                    }
                    #endregion NOT LoadAttribute.QueryName
                }
                return false;
            }
            #region NOTE
            bool loadView(object _) { return false; }
            #endregion NOTE
            #endregion IF LoadMethod
        }
        #endregion NOT IsServer

        #region IF IsServer
        /// <summary>
        /// WEB服务器
        /// </summary>
        public partial class webServer : fastCSharp.net.tcp.http.domainServer.viewServer<@SessionType.FullName>
        {
            #region NOTE
            static readonly FullName[] Views;
            const int ViewPageCount = 0;
            const int PageIndex = 0;
            const int RewriteViewCount = 0;
            const bool IsPool = false;
            #endregion NOTE
            
            /// <summary>
            /// WEB视图URL重写路径集合
            /// </summary>
            protected override keyValue<string[], string[]> rewrites
            {
                get
                {
                    int count = @Views.Length + @RewriteViewCount * 2;
                    string[] names = new string[count];
                    string[] views = new string[count];
                    #region LOOP Views
                    names[--count] = "@RewritePath";
                    views[count] = "@CallName";
                    #region IF Attribute.RewritePath
                    names[--count] = @"@Attribute.RewritePath/*IF:Attribute.IsRewriteHtml*/.html/*IF:Attribute.IsRewriteHtml*/";
                    views[count] = "@CallName";
                    names[--count] = @"@Attribute.RewritePath/*NOTE*//*NOTE*/.js";
                    views[count] = "@RewriteJs";
                    #endregion IF Attribute.RewritePath
                    #endregion LOOP Views
                    return new keyValue<string[], string[]>(names, views);
                }
            }
            #region IF ViewPageCount
            /// <summary>
            /// WEB视图URL重写索引集合
            /// </summary>
            protected override string[] viewRewrites
            {
                get
                {
                    string[] names = new string[@ViewPageCount];
                    #region LOOP Views
                    #region IF Attribute.IsPage
                    names[@PageIndex] = "@RewritePath";
                    #endregion IF Attribute.IsPage
                    #endregion LOOP Views
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
                    string[] names = new string[@ViewPageCount];
                    #region LOOP Views
                    #region IF Attribute.IsPage
                    names[@PageIndex] = "@CallName";
                    #endregion IF Attribute.IsPage
                    #endregion LOOP Views
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
                    #region LOOP Views
                    #region IF Attribute.IsPage
                    case @PageIndex: load(socket, socketIdentity, /*IF:Attribute.IsPool*/fastCSharp.typePool<@WebViewMethodType.FullName>.Pop() ?? /*IF:Attribute.IsPool*/new @WebViewMethodType.FullName()/*PUSH:Attribute*/, @IsPool/*PUSH:Attribute*/); return;
                    #endregion IF Attribute.IsPage
                    #endregion LOOP Views
                }
            }
            #endregion IF ViewPageCount
            /// <summary>
            /// 网站生成配置
            /// </summary>
            internal new static readonly fastCSharp.code.webConfig WebConfig = new /*PUSH:AutoParameter*/@DefaultNamespace/*PUSH:AutoParameter*/.webConfig();
            /// <summary>
            /// 网站生成配置
            /// </summary>
            /// <returns>网站生成配置</returns>
            protected override fastCSharp.code.webConfig getWebConfig() { return WebConfig; }
        }
        #endregion IF IsServer
        #endregion PART CLASS
    }
    /// <summary>
    /// CSharp模板公用模糊类型
    /// </summary>
    internal partial class pub
    {
        /// <summary>
        /// 默认命名空间
        /// </summary>
        public partial class DefaultNamespace
        {
            /// <summary>
            /// 网站生成配置
            /// </summary>
            public sealed class webConfig : fastCSharp.code.webConfig
            {
                /// <summary>
                /// 默认主域名
                /// </summary>
                public override string MainDomain
                {
                    get { return null; }
                }
                /// <summary>
                /// 视图加载失败重定向
                /// </summary>
                public override string NoViewLocation
                {
                    get { return null; }
                }
            }
        }
        /// <summary>
        /// 类型全名
        /// </summary>
        public partial class FullName
        {
            /// <summary>
            /// HTTP套接字接口设置
            /// </summary>
            public fastCSharp.net.tcp.http.socketBase Socket { set { } }
            /// <summary>
            /// 域名服务设置
            /// </summary>
            public fastCSharp.net.tcp.http.domainServer DomainServer { set { } }
            /// <summary>
            /// 根据HTTP请求表单值获取保存文件全称
            /// </summary>
            /// <param name="value">HTTP请求表单值</param>
            /// <returns>文件全称</returns>
            public string GetSaveFileName(ref fastCSharp.net.tcp.http.requestForm.value value) { return null; }
            /// <summary>
            /// 套接字请求编号
            /// </summary>
            public long SocketIdentity { get { return 0; } }
        }
        /// <summary>
        /// 获取该函数的类型
        /// </summary>
        public class WebViewMethodType
        {
            /// <summary>
            /// 类型全名
            /// </summary>
            public partial class FullName : fastCSharp.code.cSharp.webView.view<FullName>, fastCSharp.code.cSharp.webView.IWebView
            {
                /// <summary>
                /// web调用
                /// </summary>
                /// <param name="value">调用参数</param>
                /// <returns>返回值</returns>
                public object MethodGenericName(params object[] value)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// Session类型
        /// </summary>
        public class SessionType : pub { }
    }
}
