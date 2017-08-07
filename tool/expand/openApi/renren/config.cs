using System;
using System.Text;
using System.Collections.Specialized;

namespace fastCSharp.openApi.renren
{
    /// <summary>
    /// 应用配置
    /// </summary>
    public class config
    {
        /// <summary>
        /// 编码绑定请求
        /// </summary>
        internal static readonly encodingRequest Request = new encodingRequest(openApi.request.Default, Encoding.UTF8);
        /// <summary>
        /// 默认空表单
        /// </summary>
        private static readonly NameValueCollection defaultForm = new NameValueCollection();
#pragma warning disable 649
        /// <summary>
        /// appid
        /// </summary>
        private string client_id;
        /// <summary>
        /// appkey
        /// </summary>
        private string client_secret;
        /// <summary>
        /// 登陆成功回调地址
        /// </summary>
        private string redirect_uri;
#pragma warning restore 649
        /// <summary>
        /// URL编码 登陆成功回调地址
        /// </summary>
        private string encodeRedirectUri;
        /// <summary>
        /// URL编码 登陆成功回调地址
        /// </summary>
        internal string EncodeRedirectUri
        {
            get
            {
                if (encodeRedirectUri == null) encodeRedirectUri = fastCSharp.web.httpUtility.UrlEncode(redirect_uri);
                return encodeRedirectUri;
            }
        }
        /// <summary>
        /// 获取api调用
        /// </summary>
        /// <param name="code">authorization_code</param>
        /// <returns>API调用,失败返回null</returns>
        public api GetApi(string code)
        {
            token token = Request.RequestForm<token>("https://graph.renren.com/oauth/token?grant_type=authorization_code&client_id=" + client_id + "&redirect_uri=" + EncodeRedirectUri + "&client_secret=" + client_secret + "&code=" + code, defaultForm);
            if (token != null) return new api(this, token);
            return null;
        }
        /// <summary>
        /// 默认配置
        /// </summary>
        public static readonly config Default = fastCSharp.config.pub.LoadConfig<config>(new config());
    }
}
