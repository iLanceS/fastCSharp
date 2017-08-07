using System;
using System.Threading;
using System.Text;
using fastCSharp;
using fastCSharp.threading;
using fastCSharp.net;
using System.Collections.Specialized;

namespace fastCSharp.openApi.qq
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
#pragma warning disable 649
        /// <summary>
        /// 申请接入时注册的网站名称
        /// </summary>
        internal string site;
        /// <summary>
        /// appid
        /// </summary>
        internal string client_id;
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
        /// 访问令牌 查询字符串
        /// </summary>
        private const string access_token = "access_token=";
        /// <summary>
        /// 有效期，单位为秒 查询字符串
        /// </summary>
        private const string expires_in = "expires_in=";
        /// <summary>
        /// 获取一个新令牌
        /// </summary>
        /// <param name="code">authorization_code</param>
        /// <returns>一个新令牌</returns>
        private token getToken(string code)
        {
            string data = Request.RequestForm("https://graph.qq.com/oauth2.0/token?grant_type=authorization_code&client_id=" + client_id + "&client_secret=" + client_secret + "&code=" + code + "&redirect_uri=" + EncodeRedirectUri);
            token token = new token();
            if (data != null)
            {
                foreach (subString query in data.split('&'))
                {
                    if (query.StartsWith(access_token)) token.access_token = query.Substring(access_token.Length);
                    else if (query.StartsWith(expires_in)) int.TryParse(query.Substring(expires_in.Length), out token.expires_in);
                }
                if (!token.IsToken) log.Default.Add(data, new System.Diagnostics.StackFrame(), false);
            }
            return token;
        }
        /// <summary>
        /// 获取api调用
        /// </summary>
        /// <param name="code">authorization_code</param>
        /// <returns>API调用,失败返回null</returns>
        public api GetApi(string code)
        {
            if (string.IsNullOrEmpty(site)) log.Error.Add("网站名称不能为空", new System.Diagnostics.StackFrame(), true);
            else
            {
                token token = getToken(code);
                openId openId = token.GetOpenId();
                if (openId.openid != null) return new api(this, ref token, ref openId);
            }
            return null;
        }
        /// <summary>
        /// 获取api调用
        /// </summary>
        /// <param name="tokenOpenId">访问令牌+用户身份的标识</param>
        /// <returns>API调用,失败返回null</returns>
        public api GetApi(tokenOpenId tokenOpenId)
        {
            return GetApi(ref tokenOpenId);
        }
        /// <summary>
        /// 获取api调用
        /// </summary>
        /// <param name="tokenOpenId">访问令牌+用户身份的标识</param>
        /// <returns>API调用,失败返回null</returns>
        public api GetApi(ref tokenOpenId tokenOpenId)
        {
            if (string.IsNullOrEmpty(tokenOpenId.Token) || string.IsNullOrEmpty(tokenOpenId.OpenId)) return null;
            token token = new token { access_token = tokenOpenId.Token, expires_in = -1 };
            openId openId = new openId { openid = tokenOpenId.OpenId, client_id = client_id };
            return new api(this, ref token, ref openId);
        }
        /// <summary>
        /// 获取api调用
        /// </summary>
        /// <param name="tokenOpenId">访问令牌+用户身份的标识</param>
        /// <returns>API调用,失败返回null</returns>
        public api GetApiByJson(string tokenOpenId)
        {
            tokenOpenId value = new tokenOpenId();
            return fastCSharp.emit.jsonParser.Parse(tokenOpenId, ref value) ? GetApi(value) : null;
        }
        /// <summary>
        /// 默认配置
        /// </summary>
        public static readonly config Default = fastCSharp.config.pub.LoadConfig<config>(new config());
    }
}
