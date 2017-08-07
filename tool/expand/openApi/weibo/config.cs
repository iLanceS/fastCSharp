using System;
using System.Text;
using fastCSharp;

namespace fastCSharp.openApi.weibo
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
        /// appid
        /// </summary>
        protected string client_id;
        /// <summary>
        /// appkey
        /// </summary>
        protected string client_secret;
        /// <summary>
        /// 登陆成功回调地址
        /// </summary>
        protected string redirect_uri;
        /// <summary>
        /// 获取api调用
        /// </summary>
        /// <param name="code">authorization_code</param>
        /// <returns>API调用,失败返回null</returns>
        public api GetApi(string code)
        {
            token token = Request.RequestForm<token, tokenRequest>(@"https://api.weibo.com/oauth2/access_token", new tokenRequest
            {
                client_id = client_id,
                client_secret = client_secret,
                redirect_uri = redirect_uri,
                code = code
            });
            return token != null ? new api(this, token) : null;
        }
        /// <summary>
        /// 获取api调用
        /// </summary>
        /// <param name="tokenUid">访问令牌+用户身份的标识</param>
        /// <returns>API调用,失败返回null</returns>
        public api GetApi(tokenUid tokenUid)
        {
            return GetApi(ref tokenUid);
        }
        /// <summary>
        /// 获取api调用
        /// </summary>
        /// <param name="tokenUid">访问令牌+用户身份的标识</param>
        /// <returns>API调用,失败返回null</returns>
        public api GetApi(ref tokenUid tokenUid)
        {
            if (string.IsNullOrEmpty(tokenUid.Token) || string.IsNullOrEmpty(tokenUid.Uid)) return null;
            return new api(this, new token { access_token = tokenUid.Token, uid = tokenUid.Uid, expires_in = -1 });
        }
        /// <summary>
        /// 获取api调用
        /// </summary>
        /// <param name="tokenOpenId">访问令牌+用户身份的标识</param>
        /// <returns>API调用,失败返回null</returns>
        public api GetApiByJson(string tokenOpenId)
        {
            tokenUid value = new tokenUid();
            return fastCSharp.emit.jsonParser.Parse(tokenOpenId, ref value) ? GetApi(value) : null;
        }
        /// <summary>
        /// 默认配置
        /// </summary>
        public static readonly config Default = fastCSharp.config.pub.LoadConfig<config>(new config());
    }
}
