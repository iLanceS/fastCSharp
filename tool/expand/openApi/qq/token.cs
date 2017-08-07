using System;
using fastCSharp;
using fastCSharp.code.cSharp;

namespace fastCSharp.openApi.qq
{
    /// <summary>
    /// 访问令牌
    /// </summary>
    internal struct token
    {
        /// <summary>
        /// 访问令牌
        /// </summary>
        public string access_token;
        /// <summary>
        /// 有效期，单位为秒
        /// </summary>
        public int expires_in;
        /// <summary>
        /// 访问令牌是否有效
        /// </summary>
        public bool IsToken
        {
            get
            {
                return access_token != null && expires_in != 0;
            }
        }
        /// <summary>
        /// 获取用户身份的标识
        /// </summary>
        /// <returns>用户身份的标识</returns>
        public openId GetOpenId()
        {
            if (IsToken)
            {
                string json = config.Request.RequestForm(@"https://graph.qq.com/oauth2.0/me?access_token=" + access_token);
                if (json != null)
                {
                    bool isError = false, isJson = false;
                    openId openId = new openId();
                    try
                    {
                        if (fastCSharp.emit.jsonParser.Parse(formatJson(json), ref openId)) isJson = true;
                    }
                    catch (Exception error)
                    {
                        isError = true;
                        log.Error.Add(error, json, false);
                    }
                    if (isJson && openId.openid != null) return openId;
                    if (!isError) log.Default.Add(json, new System.Diagnostics.StackFrame(), false);
                }
            }
            return default(openId);
        }
        /// <summary>
        /// 格式化json，去掉函数调用
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private static subString formatJson(string json)
        {
            int functionIndex = json.IndexOf('(');
            if (functionIndex != -1)
            {
                int objectIndex = json.IndexOf('{');
                if (objectIndex == -1)
                {
                    int arrayIndex = json.IndexOf('[');
                    if (arrayIndex != -1 && functionIndex < arrayIndex)
                    {
                        return new subString(json, ++functionIndex, json.LastIndexOf(')') - functionIndex);
                    }
                }
                else if (functionIndex < objectIndex)
                {
                    int arrayIndex = json.IndexOf('[');
                    if (arrayIndex == -1 || functionIndex < arrayIndex)
                    {
                        return new subString(json, ++functionIndex, json.LastIndexOf(')') - functionIndex);
                    }
                }
            }
            return json;
        }
    }
}
