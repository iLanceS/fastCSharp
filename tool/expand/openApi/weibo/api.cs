using System;

namespace fastCSharp.openApi.weibo
{
    /// <summary>
    /// API调用http://open.weibo.com/wiki/%E5%BE%AE%E5%8D%9AAPI
    /// </summary>
    public class api
    {
        /// <summary>
        /// 应用配置
        /// </summary>
        private config config;
        /// <summary>
        /// 访问令牌
        /// </summary>
        private token token;
        /// <summary>
        /// 访问令牌+用户身份的标识
        /// </summary>
        public tokenUid TokenUid
        {
            get { return new tokenUid { Token = token.access_token, Uid = token.uid }; }
        }
        /// <summary>
        /// 当前授权用户的UID
        /// </summary>
        public string Uid
        {
            get { return token.uid; }
        }
        /// <summary>
        /// 请求字符串
        /// </summary>
        private string query;
        /// <summary>
        /// API调用
        /// </summary>
        /// <param name="config">应用配置</param>
        /// <param name="token">访问令牌</param>
        internal api(config config, token token)
        {
            this.config = config;
            this.token = token;
            query = "access_token=" + token.access_token;
        }
        /// <summary>
        /// 用户信息
        /// </summary>
        /// <returns>用户信息,失败返回null</returns>
        public user GetUser()
        {
            return config.Request.Request<user>(@"https://api.weibo.com/2/users/show.json?" + query + "&uid=" + token.uid);
        }
        /// <summary>
        /// 表单提交
        /// </summary>
        /// <typeparam name="jsonType">json数据数据类型</typeparam>
        /// <typeparam name="formType">表单数据类型</typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="form">POST表单</param>
        /// <returns>数据对象,失败放回null</returns>
        private jsonType form<jsonType, formType>(string url, formType form)
            where jsonType : class, IValue
            where formType : apiForm
        {
            form.access_token = token.access_token;
            return config.Request.RequestForm<jsonType, formType>(url, form);
        }
        /// <summary>
        /// 发布一条新微博
        /// </summary>
        /// <param name="value">微博信息</param>
        /// <returns>微博,失败返回null</returns>
        public status AddMicroblog(microblog value)
        {
            return form<status, microblog>(@"https://api.weibo.com/2/statuses/update.json", value);
        }
    }
}
