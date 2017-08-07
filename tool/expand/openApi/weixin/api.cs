using System;
using System.Threading;
using fastCSharp.threading;

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// API调用http://mp.weixin.qq.com/wiki/6/01405db0092f76bb96b12a9f954cd866.html
    /// </summary>
    public sealed class api
    {
        /// <summary>
        /// 超时时间
        /// </summary>
        private DateTime timeout;
        /// <summary>
        /// 应用配置
        /// </summary>
        internal readonly config config;
        /// <summary>
        /// 访问令牌获取
        /// </summary>
        private readonly Func<fastCSharp.net.returnValue<keyValue<string, DateTime>>> getter;
        /// <summary>
        /// 重置访问令牌
        /// </summary>
        private readonly Func<string, fastCSharp.net.returnValue<keyValue<string, DateTime>>> reset;
        /// <summary>
        /// 访问令牌更新锁
        /// </summary>
        private readonly object getTokenLock;
        /// <summary>
        /// 访问令牌
        /// </summary>
        private token token;
        /// <summary>
        /// 访问令牌锁
        /// </summary>
        private int tokenLock;
        /// <summary>
        /// API调用
        /// </summary>
        /// <param name="config">应用配置</param>
        public api(config config = null)
        {
            this.config = config ?? config.Default;
            getTokenLock = new object();
        }
        /// <summary>
        /// API调用
        /// </summary>
        /// <param name="getter">访问令牌获取</param>
        /// <param name="reset">重置令牌委托</param>
        /// <param name="config">应用配置</param>
        public api(Func<fastCSharp.net.returnValue<keyValue<string, DateTime>>> getter
            , Func<string, fastCSharp.net.returnValue<keyValue<string, DateTime>>> reset, config config = null)
        {
            if (getter == null || reset == null) log.Default.Throw(log.exceptionType.Null);
            this.config = config ?? config.Default;
            this.getter = getter;
            this.reset = reset;
        }
        /// <summary>
        /// 获取访问令牌
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private token checkToken(ref DateTime timeout)
        {
            token value;
            interlocked.CompareSetYield(ref tokenLock);
            if (this.timeout > date.NowSecond)
            {
                value = token;
                timeout = this.timeout;
                tokenLock = 0;
            }
            else
            {
                tokenLock = 0;
                value = null;
            }
            return value;
        }
        /// <summary>
        /// 设置访问令牌
        /// </summary>
        /// <param name="tokenTime"></param>
        /// <returns></returns>
        private token setToken(keyValue<string, DateTime> tokenTime)
        {
            if (tokenTime.Value > date.NowSecond)
            {
                token value = this.token;
                if (value == null)
                {
                    value = new token();
                    interlocked.CompareSetYield(ref tokenLock);
                    value.access_token = tokenTime.Key;
                    this.timeout = tokenTime.Value;
                    this.token = value;
                    tokenLock = 0;
                }
                else
                {
                    interlocked.CompareSetYield(ref tokenLock);
                    value.access_token = tokenTime.Key;
                    this.timeout = tokenTime.Value;
                    tokenLock = 0;
                }
                return value;
            }
            return null;
        }
        /// <summary>
        /// 获取访问令牌
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private token getTokenWithLock(ref DateTime timeout)
        {
            token value;
            Monitor.Enter(getTokenLock);
            try
            {
                if ((value = checkToken(ref timeout)) != null) return value;
                if ((value = config.GetToken()) != null && value.IsValue && (timeout = date.NowSecond.AddSeconds(value.expires_in - 60)) > date.NowSecond)
                {
                    interlocked.CompareSetYield(ref tokenLock);
                    token = value;
                    this.timeout = timeout;
                    tokenLock = 0;
                    return value;
                }
            }
            finally { Monitor.Exit(getTokenLock); }
            if (value != null) log.Default.Add("访问令牌获取失败 " + value.Message, new System.Diagnostics.StackFrame(), true);
            return null;
        }
        /// <summary>
        /// 获取访问令牌
        /// </summary>
        /// <returns></returns>
        public keyValue<string, DateTime> GetToken()
        {
            if (getter != null) log.Default.Throw(log.exceptionType.ErrorOperation);
            DateTime timeout = DateTime.MinValue;
            token token = checkToken(ref timeout) ?? getTokenWithLock(ref timeout);
            return token == null ? default(keyValue<string, DateTime>) : new keyValue<string, DateTime>(token.access_token, timeout);
        }
        /// <summary>
        /// 获取访问令牌
        /// </summary>
        /// <returns></returns>
        private string getToken()
        {
            DateTime timeout = DateTime.MinValue;
            token token = checkToken(ref timeout) ?? (getter == null ? getTokenWithLock(ref timeout) : setToken(getter()));
            return token == null ? null : token.access_token;
        }
        /// <summary>
        /// 重置访问令牌
        /// </summary>
        /// <param name="token"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private string checkToken(string token, ref DateTime timeout)
        {
            interlocked.CompareSetYield(ref tokenLock);
            if (this.timeout > date.NowSecond)
            {
                if (token != this.token.access_token)
                {
                    token = this.token.access_token;
                    timeout = this.timeout;
                    tokenLock = 0;
                    return token;
                }
                this.timeout = DateTime.MinValue;
            }
            tokenLock = 0;
            return null;
        }
        /// <summary>
        /// 重置访问令牌
        /// </summary>
        /// <param name="token">访问令牌</param>
        /// <returns></returns>
        private string resetToken(string token)
        {
            DateTime timeout = DateTime.MinValue;
            string newToken = checkToken(token, ref timeout);
            if (newToken != null) return newToken;
            token value = reset == null ? getTokenWithLock(ref timeout) : setToken(reset(token));
            return value == null ? null : value.access_token;
        }
        /// <summary>
        /// 重置访问令牌
        /// </summary>
        /// <param name="token">访问令牌</param>
        /// <returns></returns>
        private keyValue<string, DateTime> ResetToken(string token)
        {
            if (getter != null) log.Default.Throw(log.exceptionType.ErrorOperation);
            DateTime timeout = DateTime.MinValue;
            string newToken = checkToken(token, ref timeout);
            if (newToken != null) return new keyValue<string, DateTime>(newToken, timeout);
            token value = getTokenWithLock(ref timeout);
            return value == null ? default(keyValue<string, DateTime>) : new keyValue<string, DateTime>(value.access_token, timeout);
        }
        /// <summary>
        /// API请求json数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="urlPrefix">请求地址</param>
        /// <returns>数据对象,失败放回null</returns>
        private valueType request<valueType>(string urlPrefix) where valueType : isValue
        {
            string token = getToken();
            if (token != null)
            {
                string url = urlPrefix + token;
                valueType value = config.Request.Request<valueType>(url);
                if (value == null)
                {
                    if ((token = getToken()) != null)
                    {
                        value = config.Request.Request<valueType>(urlPrefix + token);
                        if (value != null && value.IsValue) return value;
                    }
                }
                else
                {
                    if (value.IsValue) return value;
                    if (value.IsBusy)
                    {
                        valueType newValue = config.Request.Request<valueType>(url);
                        if (newValue != null && newValue.IsValue) return newValue;
                    }
                    else if (value.IsTokenError || value.IsTokenExpired)
                    {
                        if ((token = resetToken(token)) != null)
                        {
                            valueType newValue = config.Request.Request<valueType>(urlPrefix + token);
                            if (newValue != null && newValue.IsValue) return newValue;
                        }
                    }
                }
                if (value != null) log.Default.Add("API " + url + " 请求失败 " + value.Message, new System.Diagnostics.StackFrame(), true);
            }
            return null;
        }
        /// <summary>
        /// API请求json数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="formType">表单数据类型</typeparam>
        /// <param name="urlPrefix">请求地址</param>
        /// <param name="form">表单数据</param>
        /// <returns>数据对象,失败放回null</returns>
        private valueType requestJson<valueType, formType>(string urlPrefix, formType form) where valueType : isValue
        {
            string token = getToken();
            if (token != null)
            {
                string url = urlPrefix + token;
                valueType value = config.Request.RequestJson<valueType, formType>(url, form);
                if (value == null)
                {
                    if ((token = getToken()) != null)
                    {
                        value = config.Request.RequestJson<valueType, formType>(urlPrefix + token, form);
                        if (value != null && value.IsValue) return value;
                    }
                }
                else
                {
                    if (value.IsValue) return value;
                    if (value.IsBusy)
                    {
                        valueType newValue = config.Request.RequestJson<valueType, formType>(url, form);
                        if (newValue != null && newValue.IsValue) return newValue;
                    }
                    else if (value.IsTokenError || value.IsTokenExpired)
                    {
                        if ((token = resetToken(token)) != null)
                        {
                            valueType newValue = config.Request.RequestJson<valueType, formType>(urlPrefix + token, form);
                            if (newValue != null && newValue.IsValue) return newValue;
                        }
                    }
                }
                if (value != null) log.Default.Add("API " + url + " 请求失败 " + value.Message, new System.Diagnostics.StackFrame(), true);
            }
            return null;
        }
        /// <summary>
        /// API请求json数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="urlPrefix">请求地址</param>
        /// <param name="data">文件数据</param>
        /// <param name="filename">文件名称</param>
        /// <param name="extensionName">扩展名称</param>
        /// <param name="form">表单数据</param>
        /// <returns>数据对象,失败放回null</returns>
        private valueType requestFile<valueType>(string urlPrefix, byte[] data, string filename = "media", string extensionName = null, keyValue<byte[], byte[]>[] form = null) where valueType : isValue
        {
            string token = getToken();
            if (token != null)
            {
                string url = urlPrefix + token;
                valueType value = config.Request.RequestJson<valueType>(url, data, filename, extensionName, form);
                if (value == null)
                {
                    if ((token = getToken()) != null)
                    {
                        value = config.Request.RequestJson<valueType>(urlPrefix + token, data, filename, extensionName, form);
                        if (value != null && value.IsValue) return value;
                    }
                }
                else
                {
                    if (value.IsValue) return value;
                    if (value.IsBusy)
                    {
                        valueType newValue = config.Request.RequestJson<valueType>(url, data, filename, extensionName, form);
                        if (newValue != null && newValue.IsValue) return newValue;
                    }
                    else if (value.IsTokenError || value.IsTokenExpired)
                    {
                        if ((token = resetToken(token)) != null)
                        {
                            valueType newValue = config.Request.RequestJson<valueType>(urlPrefix + token, data, filename, extensionName, form);
                            if (newValue != null && newValue.IsValue) return newValue;
                        }
                    }
                }
                if (value != null) log.Default.Add("API " + url + " 请求失败 " + value.Message, new System.Diagnostics.StackFrame(), true);
            }
            return null;
        }
        /// <summary>
        /// API请求
        /// </summary>
        /// <typeparam name="formType">表单数据类型</typeparam>
        /// <param name="urlPrefix">请求地址</param>
        /// <param name="form">表单数据</param>
        /// <returns>数据对象,失败放回null</returns>
        private byte[] downloadJson<formType>(string urlPrefix, formType form)
        {
            string token = getToken();
            if (token != null)
            {
                byte[] data = config.Request.DownloadJson<formType>(urlPrefix + token, form);
                if (data != null) return data;
                if ((token = getToken()) != null) return config.Request.DownloadJson<formType>(urlPrefix + token, form);
            }
            return null;
        }
        /// <summary>
        /// API请求
        /// </summary>
        /// <param name="urlPrefix">请求地址</param>
        /// <returns>数据对象,失败放回null</returns>
        private byte[] downloadIsValue(string urlPrefix)
        {
            string token = getToken();
            if (token != null)
            {
                string url = urlPrefix + token;
                byte[] data = config.Request.Download(url);
                isValue value = null;
                if (data == null)
                {
                    if ((token = getToken()) != null && (data = config.Request.Download(urlPrefix + token)) != null && (value = checkMediaData(data)) == null)
                    {
                        return data;
                    }
                }
                else if ((value = checkMediaData(data)) == null) return data;
                else if (value.IsBusy)
                {
                    if ((data = config.Request.Download(url)) != null && checkMediaData(data) == null) return data;
                }
                else if (value.IsTokenError || value.IsTokenExpired)
                {
                    if ((token = resetToken(token)) != null && (data = config.Request.Download(urlPrefix + token)) != null && checkMediaData(data) == null) return data;
                }
                if (value != null) log.Default.Add("API " + url + " 请求失败 " + value.Message, new System.Diagnostics.StackFrame(), true);
            }
            return null;
        }
        /// <summary>
        /// 检测媒体文件数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static unsafe isValue checkMediaData(byte[] data)
        {
            if (data.Length <= 256)
            {
                fixed (byte* dataFixed = data)
                {
                    if (*dataFixed != '{') return null;
                    byte* start = dataFixed, end = dataFixed + data.Length;
                    for (byte* end32 = end - (data.Length & 3); start != end32; start += sizeof(uint))
                    {
                        if ((*(uint*)start & 0x80808080U) != 0) return null;
                    }
                    while (start != end)
                    {
                        if ((*start & 0x80) != 0) return null;
                    }
                    return fastCSharp.emit.jsonParser.Parse<isValue>(String.UnsafeDeSerialize(dataFixed, -data.Length));
                }
            }
            return null;
        }
        /// <summary>
        /// 微信服务器IP地址
        /// </summary>
        private sealed class callbakIP : isValue
        {
#pragma warning disable
            /// <summary>
            /// 微信服务器IP地址列表
            /// </summary>
            public string[] ip_list;
#pragma warning restore
        }
        /// <summary>
        /// 获取微信服务器IP地址
        /// </summary>
        /// <returns></returns>
        public string[] GetCallbackIP()
        {
            callbakIP value = request<callbakIP>("https://api.weixin.qq.com/cgi-bin/getcallbackip?access_token=");
            return value == null ? null : value.ip_list;
        }
        /// <summary>
        /// 添加客服账号(必须先在公众平台官网为公众号设置微信号后才能使用该能力,每个公众号最多添加10个客服账号)
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool AddAccount(account account)
        {
            if (account != null)
            {
                isValue value = requestJson<isValue, account>("https://api.weixin.qq.com/customservice/kfaccount/add?access_token=", account);
                return value != null && value.IsValue;
            }
            return false;
        }
        /// <summary>
        /// 在线客服接待信息
        /// </summary>
        private sealed class onlineAccountResult : isValue
        {
#pragma warning disable
            /// <summary>
            /// 在线客服接待信息
            /// </summary>
            public onlineAccount[] kf_online_list;
#pragma warning restore
        }
        /// <summary>
        /// 获取在线客服接待信息
        /// </summary>
        /// <returns></returns>
        public onlineAccount[] GetOnlineAccount()
        {
            onlineAccountResult value = request<onlineAccountResult>("https://api.weixin.qq.com/cgi-bin/customservice/getonlinekflist?access_token=");
            return value == null ? null : value.kf_online_list;
        }
        /// <summary>
        /// 修改客服帐号
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool UpdateAccount(account account)
        {
            if (account != null)
            {
                isValue value = requestJson<isValue, account>("https://api.weixin.qq.com/customservice/kfaccount/update?access_token=", account);
                return value != null && value.IsValue;
            }
            return false;
        }
        /// <summary>
        /// 删除客服帐号
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool DeleteAccount(account account)
        {
            if (account != null)
            {
                isValue value = requestJson<isValue, account>("https://api.weixin.qq.com/customservice/kfaccount/del?access_token=", account);
                return value != null && value.IsValue;
            }
            return false;
        }
        /// <summary>
        /// 上传客服帐号头像
        /// </summary>
        /// <param name="account"></param>
        /// <param name="data"></param>
        /// <param name="extensionName">扩展名称</param>
        /// <returns></returns>
        public bool UploadAccountImage(string account, byte[] data, string extensionName = null)
        {
            if (!string.IsNullOrEmpty(account) && data.length() != 0)
            {
                isValue value = requestFile<isValue>("http://api.weixin.qq.com/customservice/kfaccount/uploadheadimg?kf_account=" + account + "&access_token=", data, "media", extensionName);
                return value != null && value.IsValue;
            }
            return false;
        }
        /// <summary>
        /// 获取所有客服账号
        /// </summary>
        /// <returns></returns>
        public accountList.account[] GetAccountList()
        {
            accountList value = request<accountList>("https://api.weixin.qq.com/cgi-bin/customservice/getkflist?access_token=");
            return value == null ? null : value.kf_list;
        }
        /// <summary>
        /// 发客服消息
        /// </summary>
        /// <param name="message">客服消息</param>
        /// <returns></returns>
        public bool SendMessage(message message)
        {
            if (message != null)
            {
                isValue value = requestJson<isValue, message>("https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token=", message);
                return value != null && value.IsValue;
            }
            return false;
        }
        /// <summary>
        /// 发客服 文本消息
        /// </summary>
        /// <param name="message">客服消息</param>
        /// <param name="content">文本消息内容</param>
        /// <returns></returns>
        public bool SendText(message message, string content)
        {
            if (message != null && !string.IsNullOrEmpty(content))
            {
                message.msgtype = message.type.text;
                message.text.content = content;
                return SendMessage(message);
            }
            return false;
        }
        /// <summary>
        /// 发客服 图片消息
        /// </summary>
        /// <param name="message">客服消息</param>
        /// <param name="media_id">媒体ID</param>
        /// <returns></returns>
        public bool SendImage(message message, string media_id)
        {
            if (message != null && !string.IsNullOrEmpty(media_id))
            {
                message.msgtype = message.type.image;
                message.image.media_id = media_id;
                return SendMessage(message);
            }
            return false;
        }
        /// <summary>
        /// 发客服 语音消息
        /// </summary>
        /// <param name="message">客服消息</param>
        /// <param name="media_id">媒体ID</param>
        /// <returns></returns>
        public bool SendVoice(message message, string media_id)
        {
            if (message != null && !string.IsNullOrEmpty(media_id))
            {
                message.msgtype = message.type.voice;
                message.voice.media_id = media_id;
                return SendMessage(message);
            }
            return false;
        }
        /// <summary>
        /// 发客服 视频消息
        /// </summary>
        /// <param name="message">客服消息</param>
        /// <param name="video">视频消息</param>
        /// <returns></returns>
        public bool SendVideo(message message, message.videoMessage video)
        {
            if (message != null)
            {
                message.msgtype = message.type.video;
                message.video = video;
                return SendMessage(message);
            }
            return false;
        }
        /// <summary>
        /// 发客服 音乐消息
        /// </summary>
        /// <param name="message">客服消息</param>
        /// <param name="music">音乐消息</param>
        /// <returns></returns>
        public bool SendMusic(message message, message.musicMessage music)
        {
            if (message != null)
            {
                message.msgtype = message.type.music;
                message.music = music;
                return SendMessage(message);
            }
            return false;
        }
        /// <summary>
        /// 发客服 图文消息
        /// </summary>
        /// <param name="message">客服消息</param>
        /// <param name="articles">图文消息</param>
        /// <returns></returns>
        public bool SendNews(message message, message.article[] articles)
        {
            if (message != null && articles.length() != 0)
            {
                message.msgtype = message.type.news;
                message.news.articles = articles;
                return SendMessage(message);
            }
            return false;
        }
        /// <summary>
        /// 发客服 卡券
        /// </summary>
        /// <param name="message">客服消息</param>
        /// <param name="wxcard">卡券</param>
        /// <returns></returns>
        public bool SendCard(message message, message.cardMessage wxcard)
        {
            if (message != null)
            {
                message.msgtype = message.type.wxcard;
                message.wxcard = wxcard;
                return SendMessage(message);
            }
            return false;
        }
        /// <summary>
        /// 发客服 卡券
        /// </summary>
        /// <param name="message"></param>
        /// <param name="card_id"></param>
        /// <param name="api_ticket"></param>
        /// <param name="card_ext"></param>
        /// <returns></returns>
        public bool SendCard(message message, string card_id, string api_ticket, message.card card_ext)
        {
            if (message != null && !string.IsNullOrEmpty(card_id))
            {
                card_ext.SetSignature(card_id, api_ticket);
                message.msgtype = message.type.wxcard;
                message.wxcard.card_id = card_id;
                message.wxcard.card_ext = card_ext;
                return SendMessage(message);
            }
            return false;
        }
        /// <summary>
        /// URL
        /// </summary>
        private sealed class urlResult : isValue
        {
#pragma warning disable
            /// <summary>
            /// URL
            /// </summary>
            public string url;
#pragma warning restore
        }
        /// <summary>
        /// 上传图文消息内的图片获取URL。图片仅支持jpg/png格式，大小必须在1MB以下。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="extensionName"></param>
        /// <returns>图片URL</returns>
        public string UploadNewsImage(byte[] data, string extensionName = null)
        {
            if (data.length() != 0)
            {
                urlResult value = requestFile<urlResult>("https://api.weixin.qq.com/cgi-bin/media/uploadimg?access_token=", data, "media", extensionName);
                if (value != null) return value.url;
            }
            return null;
        }
        /// <summary>
        /// 上传图文消息素材
        /// </summary>
        private struct uploadArticle
        {
            /// <summary>
            /// 图文消息素材
            /// </summary>
            public article[] articles;
        }
        /// <summary>
        /// 上传图文消息素材
        /// </summary>
        /// <param name="articles">支持1到10条图文</param>
        /// <returns></returns>
        public media UploadNews(article[] articles)
        {
            if ((uint)(articles.length() - 1) < 10)
            {
                return requestJson<media, uploadArticle>("https://api.weixin.qq.com/cgi-bin/media/uploadnews?access_token=", new uploadArticle { articles = articles });
            }
            return null;
        }
        /// <summary>
        /// 视频转换
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        public media UploadVideo(sendVideo video)
        {
            if (!string.IsNullOrEmpty(video.media_id))
            {
                return requestJson<media, sendVideo>("https://file.api.weixin.qq.com/cgi-bin/media/uploadvideo?access_token=", video);
            }
            return null;
        }
        /// <summary>
        /// 群发消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public messageId SendMessage(bulkMessage message)
        {
            if (message != null)
            {
                return requestJson<messageId, bulkMessage>("https://api.weixin.qq.com/cgi-bin/message/mass/sendall?access_token=", message);
            }
            return null;
        }
        /// <summary>
        /// 群发图文消息
        /// </summary>
        /// <param name="filter">用于设定图文消息的接收者</param>
        /// <param name="articles">支持1到10条图文</param>
        /// <returns></returns>
        public messageId SendNews(bulkMessage.messageFilter filter, article[] articles)
        {
            media media = UploadNews(articles);
            return media == null ? null : SendNews(filter, media.media_id);
        }
        /// <summary>
        /// 群发图文消息
        /// </summary>
        /// <param name="filter">用于设定图文消息的接收者</param>
        /// <param name="media_id"></param>
        /// <returns></returns>
        public messageId SendNews(bulkMessage.messageFilter filter, string media_id)
        {
            if (!string.IsNullOrEmpty(media_id))
            {
                return SendMessage(new bulkMessage { filter = filter, msgtype = bulkMessage.type.mpnews, mpnews = new messageBase.media { media_id = media_id } });
            }
            return null;
        }
        /// <summary>
        /// 群发文本消息
        /// </summary>
        /// <param name="filter">用于设定图文消息的接收者</param>
        /// <param name="content"></param>
        /// <returns></returns>
        public messageId SendText(bulkMessage.messageFilter filter, string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                return SendMessage(new bulkMessage { filter = filter, msgtype = bulkMessage.type.text, text = new messageBase.textMessage { content = content } });
            }
            return null;
        }
        /// <summary>
        /// 群发语音消息
        /// </summary>
        /// <param name="filter">用于设定图文消息的接收者</param>
        /// <param name="media_id">需通过基础支持中的上传下载多媒体文件来得到</param>
        /// <returns></returns>
        public messageId SendVoice(bulkMessage.messageFilter filter, string media_id)
        {
            if (!string.IsNullOrEmpty(media_id))
            {
                return SendMessage(new bulkMessage { filter = filter, msgtype = bulkMessage.type.voice, voice = new messageBase.media { media_id = media_id } });
            }
            return null;
        }
        /// <summary>
        /// 群发图片消息
        /// </summary>
        /// <param name="filter">用于设定图文消息的接收者</param>
        /// <param name="media_id">需通过基础支持中的上传下载多媒体文件来得到</param>
        /// <returns></returns>
        public messageId SendImage(bulkMessage.messageFilter filter, string media_id)
        {
            if (!string.IsNullOrEmpty(media_id))
            {
                return SendMessage(new bulkMessage { filter = filter, msgtype = bulkMessage.type.image, image = new messageBase.media { media_id = media_id } });
            }
            return null;
        }
        /// <summary>
        /// 群发视频
        /// </summary>
        /// <param name="filter">用于设定图文消息的接收者</param>
        /// <param name="media_id">需通过UploadVideo视频转换来得到</param>
        /// <returns></returns>
        public messageId SendVideo(bulkMessage.messageFilter filter, string media_id)
        {
            if (!string.IsNullOrEmpty(media_id))
            {
                return SendMessage(new bulkMessage { filter = filter, msgtype = bulkMessage.type.mpvideo, mpvideo = new messageBase.media { media_id = media_id } });
            }
            return null;
        }
        /// <summary>
        /// 群发视频
        /// </summary>
        /// <param name="filter">用于设定图文消息的接收者</param>
        /// <param name="video"></param>
        /// <returns></returns>
        public messageId SendVideo(bulkMessage.messageFilter filter, sendVideo video)
        {
            media media = UploadVideo(video);
            return media == null ? null : SendVideo(filter, media.media_id);
        }
        /// <summary>
        /// 群发卡券消息
        /// </summary>
        /// <param name="filter">用于设定图文消息的接收者</param>
        /// <param name="card_id"></param>
        /// <returns></returns>
        public messageId SendCard(bulkMessage.messageFilter filter, string card_id)
        {
            if (!string.IsNullOrEmpty(card_id))
            {
                return SendMessage(new bulkMessage { filter = filter, msgtype = bulkMessage.type.wxcard, wxcard = new bulkMessageBase.cardMessage { card_id = card_id } });
            }
            return null;
        }
        /// <summary>
        /// 群发消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public messageId SendMessage(openIdMessage message)
        {
            if (message != null)
            {
                return requestJson<messageId, openIdMessage>("https://api.weixin.qq.com/cgi-bin/message/mass/send?access_token=", message);
            }
            return null;
        }
        /// <summary>
        /// 群发图文消息
        /// </summary>
        /// <param name="touser">接收者，一串OpenID列表，OpenID最少2个，最多10000个</param>
        /// <param name="articles">支持1到10条图文</param>
        /// <returns></returns>
        public messageId SendNews(string[] touser, article[] articles)
        {
            if (touser.length() != 0)
            {
                media media = UploadNews(articles);
                if(media != null) return SendNews(touser, media.media_id);
            }
            return null;
        }
        /// <summary>
        /// 群发图文消息
        /// </summary>
        /// <param name="touser">接收者，一串OpenID列表，OpenID最少2个，最多10000个</param>
        /// <param name="media_id"></param>
        /// <returns></returns>
        public messageId SendNews(string[] touser, string media_id)
        {
            if (touser.length() != 0 && !string.IsNullOrEmpty(media_id))
            {
                return SendMessage(new openIdMessage { touser = touser, msgtype = bulkMessage.type.mpnews, mpnews = new messageBase.media { media_id = media_id } });
            }
            return null;
        }
        /// <summary>
        /// 群发文本消息
        /// </summary>
        /// <param name="touser">接收者，一串OpenID列表，OpenID最少2个，最多10000个</param>
        /// <param name="content"></param>
        /// <returns></returns>
        public messageId SendText(string[] touser, string content)
        {
            if (touser.length() != 0 && !string.IsNullOrEmpty(content))
            {
                return SendMessage(new openIdMessage { touser = touser, msgtype = bulkMessage.type.text, text = new messageBase.textMessage { content = content } });
            }
            return null;
        }
        /// <summary>
        /// 群发语音消息
        /// </summary>
        /// <param name="touser">接收者，一串OpenID列表，OpenID最少2个，最多10000个</param>
        /// <param name="media_id">需通过基础支持中的上传下载多媒体文件来得到</param>
        /// <returns></returns>
        public messageId SendVoice(string[] touser, string media_id)
        {
            if (touser.length() != 0 && !string.IsNullOrEmpty(media_id))
            {
                return SendMessage(new openIdMessage { touser = touser, msgtype = bulkMessage.type.voice, voice = new messageBase.media { media_id = media_id } });
            }
            return null;
        }
        /// <summary>
        /// 群发图片消息
        /// </summary>
        /// <param name="touser">接收者，一串OpenID列表，OpenID最少2个，最多10000个</param>
        /// <param name="media_id">需通过基础支持中的上传下载多媒体文件来得到</param>
        /// <returns></returns>
        public messageId SendImage(string[] touser, string media_id)
        {
            if (touser.length() != 0 && !string.IsNullOrEmpty(media_id))
            {
                return SendMessage(new openIdMessage { touser = touser, msgtype = bulkMessage.type.image, image = new messageBase.media { media_id = media_id } });
            }
            return null;
        }
        /// <summary>
        /// 群发视频
        /// </summary>
        /// <param name="touser">接收者，一串OpenID列表，OpenID最少2个，最多10000个</param>
        /// <param name="media_id">需通过UploadVideo视频转换来得到</param>
        /// <returns></returns>
        public messageId SendVideo(string[] touser, string media_id)
        {
            if (touser.length() != 0 && !string.IsNullOrEmpty(media_id))
            {
                return SendMessage(new openIdMessage { touser = touser, msgtype = bulkMessage.type.mpvideo, mpvideo = new messageBase.media { media_id = media_id } });
            }
            return null;
        }
        /// <summary>
        /// 群发视频
        /// </summary>
        /// <param name="touser">接收者，一串OpenID列表，OpenID最少2个，最多10000个</param>
        /// <param name="video"></param>
        /// <returns></returns>
        public messageId SendVideo(string[] touser, sendVideo video)
        {
            if (touser.length() != 0)
            {
                media media = UploadVideo(video);
                if(media == null) return SendVideo(touser, media.media_id);
            }
            return null;
        }
        /// <summary>
        /// 群发卡券消息
        /// </summary>
        /// <param name="touser">接收者，一串OpenID列表，OpenID最少2个，最多10000个</param>
        /// <param name="card_id"></param>
        /// <returns></returns>
        public messageId SendCard(string[] touser, string card_id)
        {
            if (touser.length() != 0 && !string.IsNullOrEmpty(card_id))
            {
                return SendMessage(new openIdMessage { touser = touser, msgtype = bulkMessage.type.wxcard, wxcard = new bulkMessageBase.cardMessage { card_id = card_id } });
            }
            return null;
        }
        /// <summary>
        /// 删除消息发送任务的ID
        /// </summary>
        private struct messageIdQuery
        {
            /// <summary>
            /// 消息发送任务的ID
            /// </summary>
            public string msg_id;
        }
        /// <summary>
        /// 删除消息发送任务的ID
        /// </summary>
        /// <param name="msg_id"></param>
        /// <returns></returns>
        public bool DeleteMessage(string msg_id)
        {
            if (!string.IsNullOrEmpty(msg_id))
            {
                isValue value = requestJson<isValue, messageIdQuery>("https://api.weixin.qq.com/cgi-bin/message/mass/delete?access_token=", new messageIdQuery { msg_id = msg_id });
                return value != null && value.IsValue;
            }
            return false;
        }
        /// <summary>
        /// 发送预览消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public messageId SendMessage(previewMessage message)
        {
            if (message != null)
            {
                return requestJson<messageId, previewMessage>("https://api.weixin.qq.com/cgi-bin/message/mass/preview?access_token=", message);
            }
            return null;
        }
        /// <summary>
        /// 图文消息预览
        /// </summary>
        /// <param name="touser">接收者,isOpenId为true表示OPENID,isOpenId为false标识微信用户</param>
        /// <param name="articles">支持1到10条图文</param>
        /// <param name="isOpenId"></param>
        /// <returns></returns>
        public messageId SendNews(string touser, article[] articles, bool isOpenId = true)
        {
            if (!string.IsNullOrEmpty(touser))
            {
                media media = UploadNews(articles);
                if(media != null) return SendNews(touser, media.media_id, isOpenId);
            }
            return null;
        }
        /// <summary>
        /// 图文消息预览
        /// </summary>
        /// <param name="touser">接收者,isOpenId为true表示OPENID,isOpenId为false标识微信用户</param>
        /// <param name="media_id">需通过基础支持中的上传下载多媒体文件来得到</param>
        /// <param name="isOpenId"></param>
        /// <returns></returns>
        public messageId SendNews(string touser, string media_id, bool isOpenId = true)
        {
            if (!string.IsNullOrEmpty(touser) && !string.IsNullOrEmpty(media_id))
            {
                previewMessage message = new previewMessage { msgtype = bulkMessage.type.mpnews, mpnews = new messageBase.media { media_id = media_id } };
                if (isOpenId) message.touser = touser;
                else message.towxname = touser;
                return SendMessage(message);
            }
            return null;
        }
        /// <summary>
        /// 文本消息预览
        /// </summary>
        /// <param name="touser">接收者,isOpenId为true表示OPENID,isOpenId为false标识微信用户</param>
        /// <param name="content"></param>
        /// <param name="isOpenId"></param>
        /// <returns></returns>
        public messageId SendText(string touser, string content, bool isOpenId = true)
        {
            if (!string.IsNullOrEmpty(touser) && !string.IsNullOrEmpty(content))
            {
                previewMessage message = new previewMessage { msgtype = bulkMessage.type.text, text = new messageBase.textMessage { content = content } };
                if (isOpenId) message.touser = touser;
                else message.towxname = touser;
                return SendMessage(message);
            }
            return null;
        }
        /// <summary>
        /// 语音消息预览
        /// </summary>
        /// <param name="touser">接收者,isOpenId为true表示OPENID,isOpenId为false标识微信用户</param>
        /// <param name="media_id">需通过基础支持中的上传下载多媒体文件来得到</param>
        /// <param name="isOpenId"></param>
        /// <returns></returns>
        public messageId SendVoice(string touser, string media_id, bool isOpenId = true)
        {
            if (!string.IsNullOrEmpty(touser) && !string.IsNullOrEmpty(media_id))
            {
                previewMessage message = new previewMessage { msgtype = bulkMessage.type.voice, voice = new messageBase.media { media_id = media_id } };
                if (isOpenId) message.touser = touser;
                else message.towxname = touser;
                return SendMessage(message);
            }
            return null;
        }
        /// <summary>
        /// 图片消息预览
        /// </summary>
        /// <param name="touser">接收者,isOpenId为true表示OPENID,isOpenId为false标识微信用户</param>
        /// <param name="media_id">需通过基础支持中的上传下载多媒体文件来得到</param>
        /// <param name="isOpenId"></param>
        /// <returns></returns>
        public messageId SendImage(string touser, string media_id, bool isOpenId = true)
        {
            if (!string.IsNullOrEmpty(touser) && !string.IsNullOrEmpty(media_id))
            {
                previewMessage message = new previewMessage { msgtype = bulkMessage.type.image, image = new messageBase.media { media_id = media_id } };
                if (isOpenId) message.touser = touser;
                else message.towxname = touser;
                return SendMessage(message);
            }
            return null;
        }
        /// <summary>
        /// 视频预览
        /// </summary>
        /// <param name="touser">接收者,isOpenId为true表示OPENID,isOpenId为false标识微信用户</param>
        /// <param name="media_id">需通过UploadVideo视频转换来得到</param>
        /// <param name="isOpenId"></param>
        /// <returns></returns>
        public messageId SendVideo(string touser, string media_id, bool isOpenId = true)
        {
            if (!string.IsNullOrEmpty(touser) && !string.IsNullOrEmpty(media_id))
            {
                previewMessage message = new previewMessage { msgtype = bulkMessage.type.mpvideo, mpvideo = new messageBase.media { media_id = media_id } };
                if (isOpenId) message.touser = touser;
                else message.towxname = touser;
                return SendMessage(message);
            }
            return null;
        }
        /// <summary>
        /// 视频预览
        /// </summary>
        /// <param name="touser">接收者,isOpenId为true表示OPENID,isOpenId为false标识微信用户</param>
        /// <param name="video">需通过UploadVideo视频转换来得到</param>
        /// <param name="isOpenId"></param>
        /// <returns></returns>
        public messageId SendVideo(string touser, sendVideo video, bool isOpenId = true)
        {
            if (!string.IsNullOrEmpty(touser))
            {
                media media = UploadVideo(video);
                if (media == null) return SendVideo(touser, media.media_id, isOpenId);
            }
            return null;
        }
        /// <summary>
        /// 卡券预览
        /// </summary>
        /// <param name="touser">接收者,isOpenId为true表示OPENID,isOpenId为false标识微信用户</param>
        /// <param name="wxcard"></param>
        /// <param name="isOpenId"></param>
        /// <returns></returns>
        public messageId SendCard(string touser, message.cardMessage wxcard, bool isOpenId = true)
        {
            if (!string.IsNullOrEmpty(touser))
            {
                previewMessage message = new previewMessage { msgtype = bulkMessage.type.wxcard, wxcard = wxcard };
                if (isOpenId) message.touser = touser;
                else message.towxname = touser;
                return SendMessage(message);
            }
            return null;
        }
        /// <summary>
        /// 卡券预览
        /// </summary>
        /// <param name="touser">接收者,isOpenId为true表示OPENID,isOpenId为false标识微信用户</param>
        /// <param name="card_id"></param>
        /// <param name="api_ticket"></param>
        /// <param name="card_ext"></param>
        /// <param name="isOpenId"></param>
        /// <returns></returns>
        public messageId SendCard(string touser, string card_id, string api_ticket, message.card card_ext, bool isOpenId = true)
        {
            if (!string.IsNullOrEmpty(touser) && !string.IsNullOrEmpty(card_id))
            {
                card_ext.SetSignature(card_id, api_ticket);
                return SendCard(touser, new message.cardMessage { card_id = card_id, card_ext = card_ext }, isOpenId);
            }
            return null;
        }
        /// <summary>
        /// 查询群发消息发送状态
        /// </summary>
        /// <param name="msg_id"></param>
        /// <returns></returns>
        public bool GetMessage(long msg_id)
        {
            return GetMessage(msg_id.toString());
        }
        /// <summary>
        /// 群发消息发送状态
        /// </summary>
        private sealed class messageStatus : isValue
        {
#pragma warning disable
            /// <summary>
            /// 群发消息后返回的消息id
            /// </summary>
            public long msg_id;
            /// <summary>
            /// 消息发送后的状态，SEND_SUCCESS表示发送成功
            /// </summary>
            public string msg_status;
#pragma warning restore
        }
        /// <summary>
        /// 查询群发消息发送状态
        /// </summary>
        /// <param name="msg_id"></param>
        /// <returns></returns>
        public bool GetMessage(string msg_id)
        {
            if (!string.IsNullOrEmpty(msg_id))
            {
                messageStatus value = requestJson<messageStatus, messageIdQuery>("https://api.weixin.qq.com/cgi-bin/message/mass/get?access_token=", new messageIdQuery { msg_id = msg_id });
                return value != null && value.msg_status == "SEND_SUCCESS";
            }
            return false;
        }
        /// <summary>
        /// 设置行业
        /// </summary>
        private struct industryQuery
        {
            /// <summary>
            /// 公众号模板消息所属行业编号
            /// </summary>
            public string industry_id1;
            /// <summary>
            /// 公众号模板消息所属行业编号
            /// </summary>
            public string industry_id2;
        }
        /// <summary>
        /// 设置行业
        /// </summary>
        /// <param name="industry1"></param>
        /// <param name="industry2"></param>
        /// <returns></returns>
        public bool SetIndustry(industry industry1, industry industry2)
        {
            isValue value = requestJson<isValue, industryQuery>("https://api.weixin.qq.com/cgi-bin/template/api_set_industry?access_token=", new industryQuery { industry_id1 = ((byte)industry1).toString(), industry_id2 = ((byte)industry2).toString() });
            return value != null && value.IsValue;
        }
        /// <summary>
        /// 获得模板ID
        /// </summary>
        private struct templateQuery
        {
            /// <summary>
            /// 模板库中模板的编号，有“TM**”和“OPENTMTM**”等形式
            /// </summary>
            public string template_id_short;
        }
        /// <summary>
        /// 模板ID
        /// </summary>
        private sealed class templateId : isValue
        {
#pragma warning disable
            /// <summary>
            /// 模板ID
            /// </summary>
            public string template_id;
#pragma warning restore
        }
        /// <summary>
        /// 获得模板ID
        /// </summary>
        /// <param name="template_id_short">模板库中模板的编号，有“TM**”和“OPENTMTM**”等形式</param>
        /// <returns></returns>
        public string GetTemplateId(string template_id_short)
        {
            if (!string.IsNullOrEmpty(template_id_short))
            {
                templateId value = requestJson<templateId, templateQuery>("https://api.weixin.qq.com/cgi-bin/template/api_add_template?access_token=", new templateQuery { template_id_short = template_id_short });
                if (value != null) return value.template_id;
            }
            return null;
        }
        /// <summary>
        /// 发送模板消息
        /// </summary>
        private sealed class sendTemplate : isValue
        {
#pragma warning disable
            /// <summary>
            /// 
            /// </summary>
            public long msgid;
#pragma warning restore
        }
        /// <summary>
        /// 发送模板消息
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="template"></param>
        /// <returns>失败返回0</returns>
        public long SendTemplate<valueType>(template<valueType> template)
        {
            if (template != null)
            {
                sendTemplate value = requestJson<sendTemplate, template<valueType>>("https://api.weixin.qq.com/cgi-bin/message/template/send?access_token=", template);
                if (value != null) return value.msgid;
            }
            return 0;
        }
        /// <summary>
        /// 获取自动回复规则
        /// </summary>
        /// <returns></returns>
        public autoReply GetAutoReply()
        {
            return request<autoReply>("https://api.weixin.qq.com/cgi-bin/get_current_autoreply_info?access_token=");
        }
        /// <summary>
        /// 新增临时素材,媒体文件在后台保存时间为3天，即3天后media_id失效
        /// </summary>
        /// <param name="type">排除图文消息</param>
        /// <param name="data"></param>
        /// <returns></returns>
        public media UploadMedia(media.mediaType type, byte[] data)
        {
            if (data.length() != 0 && type != media.mediaType.news)
            {
                return requestFile<media>("https://api.weixin.qq.com/cgi-bin/media/upload?type=" + type.ToString() + "&access_token=", data);
            }
            return null;
        }
        /// <summary>
        /// 获取临时素材
        /// </summary>
        /// <param name="media_id">媒体文件ID</param>
        /// <param name="isVideo"></param>
        /// <returns></returns>
        public byte[] DownloadMedia(string media_id, bool isVideo = false)
        {
            if (!string.IsNullOrEmpty(media_id))
            {
                if (isVideo) return downloadIsValue("http://api.weixin.qq.com/cgi-bin/media/get?media_id=" + media_id + "&access_token=");
                return downloadIsValue("https://api.weixin.qq.com/cgi-bin/media/get?media_id=" + media_id + "&access_token=");
            }
            return null;
        }
        /// <summary>
        /// 新增永久图文素材
        /// </summary>
        /// <param name="articles"></param>
        /// <returns>media_id</returns>
        public string AddNews(article[] articles)
        {
            if (articles.length() != 0)
            {
                media value = requestJson<media, uploadArticle>("https://api.weixin.qq.com/cgi-bin/material/add_news?access_token=", new uploadArticle { articles = articles });
                if (value != null) return value.media_id;
            }
            return null;
        }
        /// <summary>
        /// 视频描述信息
        /// </summary>
        private static readonly byte[] videoDescriptionData = "description".getBytes();
        /// <summary>
        /// 新增永久素材
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <param name="video"></param>
        /// <returns></returns>
        public mediaUrl UploadMediaUrl(media.mediaType type, byte[] data, uploadVideo video = default(uploadVideo))
        {
            if (data.length() != 0)
            {
                keyValue<byte[], byte[]>[] form = type == media.mediaType.video ? new keyValue<byte[], byte[]>[] { new keyValue<byte[], byte[]>(videoDescriptionData, System.Text.Encoding.UTF8.GetBytes(fastCSharp.emit.jsonSerializer.ToJson(video))) } : null;
                return requestFile<mediaUrl>("https://api.weixin.qq.com/cgi-bin/material/add_material?type=" + type.ToString() + "&access_token=", data, "media", null, form);
            }
            return null;
        }
        /// <summary>
        /// 图文消息素材
        /// </summary>
        private sealed class articles : isValue
        {
#pragma warning disable
            /// <summary>
            /// 图文消息素材
            /// </summary>
            public articleUrl[] news_item;
#pragma warning restore
        }
        /// <summary>
        /// 获取图文消息素材
        /// </summary>
        /// <param name="media_id"></param>
        /// <returns></returns>
        public articleUrl[] GetNews(string media_id)
        {
            if (!string.IsNullOrEmpty(media_id))
            {
                articles value = requestJson<articles, messageBase.media>("https://api.weixin.qq.com/cgi-bin/material/get_material?access_token=", new messageBase.media { media_id = media_id });
                if (value != null) return value.news_item;
            }
            return null;
        }
        /// <summary>
        /// 获取视频消息素材
        /// </summary>
        /// <param name="media_id"></param>
        /// <returns></returns>
        public videoUrl GetVideo(string media_id)
        {
            if (!string.IsNullOrEmpty(media_id))
            {
                return requestJson<videoUrl, messageBase.media>("https://api.weixin.qq.com/cgi-bin/material/get_material?access_token=", new messageBase.media { media_id = media_id });
            }
            return null;
        }
        /// <summary>
        /// 获取永久素材
        /// </summary>
        /// <param name="media_id">媒体文件ID</param>
        /// <returns></returns>
        public byte[] DownloadMediaPost(string media_id)
        {
            if (!string.IsNullOrEmpty(media_id))
            {
                return downloadJson<messageBase.media>("https://api.weixin.qq.com/cgi-bin/material/get_material?access_token=", new messageBase.media { media_id = media_id });
            }
            return null;
        }
        /// <summary>
        /// 删除永久素材
        /// </summary>
        /// <param name="media_id"></param>
        /// <returns></returns>
        public bool DeleteMedia(string media_id)
        {
            if (!string.IsNullOrEmpty(media_id))
            {
                isValue value = requestJson<isValue, messageBase.media>("https://api.weixin.qq.com/cgi-bin/material/del_material?access_token=", new messageBase.media { media_id = media_id });
                return value != null && value.IsValue;
            }
            return false;
        }
        /// <summary>
        /// 修改永久图文素材
        /// </summary>
        /// <param name="news"></param>
        /// <returns></returns>
        public bool UpdateNews(news news)
        {
            if (news != null)
            {
                isValue value = requestJson<isValue, news>("https://api.weixin.qq.com/cgi-bin/material/update_news?access_token=", news);
                return value != null && value.IsValue;
            }
            return false;
        }
        /// <summary>
        /// 获取素材总数
        /// </summary>
        /// <returns></returns>
        public mediaCount GetMediaCount()
        {
            return request<mediaCount>("https://api.weixin.qq.com/cgi-bin/material/get_materialcount?access_token=");
        }
        /// <summary>
        /// 获取永久图文消息素材列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public newsList GetNewsList(mediaQuery query)
        {
            query.type = mediaQuery.mediaType.news;
            return requestJson<newsList, mediaQuery>("https://api.weixin.qq.com/cgi-bin/material/batchget_material?access_token=", query);
        }
        /// <summary>
        /// 获取消息素材列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public mediaList GetMediaList(mediaQuery query)
        {
            if (query.type == mediaQuery.mediaType.news) return null;
            return requestJson<mediaList, mediaQuery>("https://api.weixin.qq.com/cgi-bin/material/batchget_material?access_token=", query);
        }
        /// <summary>
        /// 分组名字
        /// </summary>
        private struct groupName
        {
            /// <summary>
            /// 分组名字（30个字符以内）
            /// </summary>
            public string name;
        }
        /// <summary>
        /// 分组查询
        /// </summary>
        private struct groupQuery
        {
            /// <summary>
            /// 分组查询
            /// </summary>
            public groupName group;
        }
        /// <summary>
        /// 分组
        /// </summary>
        private sealed class groupResult : isValue
        {
            /// <summary>
            /// 分组
            /// </summary>
            public group group;
        }
        /// <summary>
        /// 创建分组
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public group CreateGroup(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                groupResult value = requestJson<groupResult, groupQuery>("https://api.weixin.qq.com/cgi-bin/groups/create?access_token=", new groupQuery { group = new groupName { name = name } });
                if (value != null) return value.group;
            }
            return null;
        }
        /// <summary>
        /// 所有分组
        /// </summary>
        private sealed class groupsResult : isValue
        {
#pragma warning disable
            /// <summary>
            /// 公众平台分组信息列表
            /// </summary>
            public groupCount[] groups;
#pragma warning restore
        }
        /// <summary>
        /// 查询所有分组
        /// </summary>
        /// <returns></returns>
        public groupCount[] GetGroups()
        {
            groupsResult value = request<groupsResult>("https://api.weixin.qq.com/cgi-bin/groups/get?access_token=");
            return value == null ? null : value.groups;
        }
        /// <summary>
        /// 用户的OpenID
        /// </summary>
        private struct openidQuery
        {
            /// <summary>
            /// 用户的OpenID
            /// </summary>
            public string openid;
        }
        /// <summary>
        /// 用户所属的groupid
        /// </summary>
        private sealed class groupIdResult : isValue
        {
#pragma warning disable
            /// <summary>
            /// 用户所属的groupid
            /// </summary>
            public int groupid;
#pragma warning restore
        }
        /// <summary>
        /// 查询用户所在分组
        /// </summary>
        /// <param name="openid"></param>
        /// <returns>失败返回0</returns>
        public int GetGroupId(string openid)
        {
            if (!string.IsNullOrEmpty(openid))
            {
                groupIdResult value = requestJson<groupIdResult, openidQuery>("https://api.weixin.qq.com/cgi-bin/groups/getid?access_token=", new openidQuery { openid = openid });
                if (value == null) return value.groupid;
            }
            return 0;
        }
        /// <summary>
        /// 修改分组名
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool UpdateGroup(group group)
        {
            if (group != null)
            {
                isValue value = requestJson<isValue, groupResult>("https://api.weixin.qq.com/cgi-bin/groups/update?access_token=", new groupResult { group = group });
                return value != null && value.IsValue;
            }
            return false;
        }
        /// <summary>
        /// 移动用户分组
        /// </summary>
        private struct updateGroupQuery
        {
            /// <summary>
            /// 用户唯一标识符
            /// </summary>
            public string openid;
            /// <summary>
            /// 分组id
            /// </summary>
            public int to_groupid;
        }
        /// <summary>
        /// 移动用户分组
        /// </summary>
        /// <param name="openid">用户唯一标识符</param>
        /// <param name="to_groupid">分组id</param>
        /// <returns></returns>
        public bool UpdateGroup(string openid, int to_groupid)
        {
            if (!string.IsNullOrEmpty(openid))
            {
                isValue value = requestJson<isValue, updateGroupQuery>("https://api.weixin.qq.com/cgi-bin/groups/members/update?access_token=", new updateGroupQuery { openid = openid, to_groupid = to_groupid });
                return value != null && value.IsValue;
            }
            return false;
        }
        /// <summary>
        /// 批量移动用户分组
        /// </summary>
        private struct updateGroupsQuery
        {
            /// <summary>
            /// 用户唯一标识符openid的列表
            /// </summary>
            public string[] openid_list;
            /// <summary>
            /// 分组id
            /// </summary>
            public int to_groupid;
        }
        /// <summary>
        /// 批量移动用户分组
        /// </summary>
        /// <param name="openid_list">用户唯一标识符openid的列表（size不能超过50）</param>
        /// <param name="to_groupid">分组id</param>
        /// <returns></returns>
        public bool UpdateGroup(string[] openid_list, int to_groupid)
        {
            if (openid_list.length() != 0)
            {
                isValue value = requestJson<isValue, updateGroupsQuery>("https://api.weixin.qq.com/cgi-bin/groups/members/batchupdate?access_token=", new updateGroupsQuery { openid_list = openid_list, to_groupid = to_groupid });
                return value != null && value.IsValue;
            }
            return false;
        }
        /// <summary>
        /// 删除分组
        /// </summary>
        private struct deleteGroupQuery
        {
            public deleteGroupId group;
        }
        /// <summary>
        /// 分组的id
        /// </summary>
        private struct deleteGroupId
        {
            /// <summary>
            /// 分组的id
            /// </summary>
            public int id;
        }
        /// <summary>
        /// 删除分组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteGroup(int id)
        {
            isValue value = requestJson<isValue, deleteGroupQuery>("https://api.weixin.qq.com/cgi-bin/groups/delete?access_token=", new deleteGroupQuery { group = new deleteGroupId { id = id } });
            return value != null && value.IsValue;
        }
        /// <summary>
        /// 设置备注名
        /// </summary>
        private struct updateRemarkQuery
        {
            /// <summary>
            /// 用户标识
            /// </summary>
            public string openid;
            /// <summary>
            /// 新的备注名，长度必须小于30字符
            /// </summary>
            public string remark;
        }
        /// <summary>
        /// 设置备注名
        /// </summary>
        /// <param name="openid">用户标识</param>
        /// <param name="remark">新的备注名，长度必须小于30字符</param>
        /// <returns></returns>
        public bool UpdateRemark(string openid, string remark)
        {
            if (!string.IsNullOrEmpty(openid))
            {
                isValue value = requestJson<isValue, updateRemarkQuery>("https://api.weixin.qq.com/cgi-bin/groups/delete?access_token=", new updateRemarkQuery { openid = openid, remark = remark ?? string.Empty });
                return value != null && value.IsValue;
            }
            return false;
        }
        /// <summary>
        /// 获取用户基本信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public user GetUser(userLanguage user)
        {
            if (!string.IsNullOrEmpty(user.openid))
            {
                return request<user>("https://api.weixin.qq.com/cgi-bin/user/info?openid=" + user.openid + "&lang=" + user.lang.ToString() + "&access_token=");
            }
            return null;
        }
        /// <summary>
        /// 用户列表
        /// </summary>
        private struct userListQuery
        {
            /// <summary>
            /// 用户列表
            /// </summary>
            public userLanguage[] user_list;
        }
        /// <summary>
        /// 用户列表
        /// </summary>
        private sealed class userListResult : isValue
        {
#pragma warning disable
            /// <summary>
            /// 用户列表
            /// </summary>
            public user[] user_info_list;
#pragma warning restore
        }
        /// <summary>
        /// 获取用户基本信息,最多支持一次拉取100条
        /// </summary>
        /// <param name="user_list"></param>
        /// <returns></returns>
        public user[] GetUserList(userLanguage[] user_list)
        {
            if ((uint)(user_list.length() - 1) < 100)
            {
                userListResult value = requestJson<userListResult, userListQuery>("https://api.weixin.qq.com/cgi-bin/user/info/batchget?access_token=", new userListQuery { user_list = user_list });
                if (value != null) return value.user_info_list;
            }
            return null;
        }
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="next_openid">第一个拉取的OPENID，不填默认从头开始拉取</param>
        /// <returns></returns>
        public openIdList GetOpenIdList(string next_openid = null)
        {
            if (string.IsNullOrEmpty(next_openid)) return request<openIdList>("https://api.weixin.qq.com/cgi-bin/user/get?access_token=");
            return request<openIdList>("https://api.weixin.qq.com/cgi-bin/user/get?next_openid=" + next_openid + "&access_token=");
        }
        /// <summary>
        /// 自定义菜单
        /// </summary>
        private struct menuQeury
        {
            /// <summary>
            /// 自定义菜单
            /// </summary>
            public menu[] button;
        }
        /// <summary>
        /// 创建自定义菜单
        /// </summary>
        /// <param name="button">最多包括3个一级菜单</param>
        /// <returns></returns>
        public bool CreateMenu(menu[] button)
        {
            if (((uint)button.length() - 1) < 3)
            {
                isValue value = requestJson<isValue, menuQeury>("https://api.weixin.qq.com/cgi-bin/menu/create?access_token=", new menuQeury { button = button });
                return value != null && value.IsValue;
            }
            return false;
        }
        /// <summary>
        /// 获取自定义菜单
        /// </summary>
        private sealed class menuResult : isValue
        {
#pragma warning disable
            /// <summary>
            /// 自定义菜单
            /// </summary>
            public menuQeury menu;
#pragma warning restore
        }
        /// <summary>
        /// 获取自定义菜单
        /// </summary>
        /// <returns></returns>
        public menu[] GetMenu()
        {
            menuResult value = request<menuResult>("https://api.weixin.qq.com/cgi-bin/menu/get?access_token=");
            return value == null ? null : value.menu.button;
        }
        /// <summary>
        /// 获取自定义菜单
        /// </summary>
        /// <returns></returns>
        public bool DeleteMenu()
        {
            isValue value = request<isValue>("https://api.weixin.qq.com/cgi-bin/menu/delete?access_token=");
            return value == null && value.IsValue;
        }
        /// <summary>
        /// 自定义菜单配置
        /// </summary>
        private sealed class menuInfoResult : isValue
        {
#pragma warning disable
            /// <summary>
            /// 菜单是否开启，0代表未开启，1代表开启
            /// </summary>
            public byte is_menu_open;
            /// <summary>
            /// 菜单信息
            /// </summary>
            public menuQeury selfmenu_info;
#pragma warning restore
        }
        /// <summary>
        /// 获取自定义菜单配置
        /// </summary>
        /// <param name="is_menu_open">菜单是否开启</param>
        /// <returns></returns>
        public menu[] GetMenuInfo(out bool is_menu_open)
        {
            menuInfoResult value = request<menuInfoResult>("https://api.weixin.qq.com/cgi-bin/get_current_selfmenu_info?access_token=");
            if (value != null)
            {
                is_menu_open = value.is_menu_open != 0;
                return value.selfmenu_info.button;
            }
            is_menu_open = false;
            return null;
        }
        /// <summary>
        /// 生成带参数的二维码
        /// </summary>
        private struct qrCodeQuery
        {
            /// <summary>
            /// 该二维码有效时间，以秒为单位。 最大不超过604800（即7天）
            /// </summary>
            public int expire_seconds;
            /// <summary>
            /// 二维码详细信息
            /// </summary>
            public actionInfo action_info;
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="toJsoner"></param>
            /// <param name="value"></param>
            [fastCSharp.emit.jsonSerialize.custom]
            private static void toJson(fastCSharp.emit.jsonSerializer toJsoner, qrCodeQuery value)
            {
                toJsoner.UnsafeWriteFirstName("action_name");
                if (value.expire_seconds == 0)
                {
                    toJsoner.UnsafeToJsonNotNull(value.action_info.scene.scene_str == null ? "QR_LIMIT_SCENE" : "QR_LIMIT_STR_SCENE");
                }
                else
                {
                    toJsoner.UnsafeToJsonNotNull("QR_SCENE");
                    toJsoner.UnsafeWriteNextName("expire_seconds");
                    toJsoner.UnsafeToJsonNotNull(value.expire_seconds);
                }
                toJsoner.UnsafeWriteNextName("action_info");
                toJsoner.UnsafeToJsonNotNull(value.action_info);
                toJsoner.UnsafeCharStream.Write('}');
            }
        }
        /// <summary>
        /// 二维码详细信息
        /// </summary>
        private struct actionInfo
        {
            /// <summary>
            /// 场景值ID
            /// </summary>
            public sceneId scene;
        }
        /// <summary>
        /// 场景值ID
        /// </summary>
        private struct sceneId
        {
            /// <summary>
            /// 场景值ID，临时二维码时为32位非0整型，永久二维码时最大值为100000（目前参数只支持1--100000）
            /// </summary>
            public uint scene_id;
            /// <summary>
            /// 场景值ID（字符串形式的ID），字符串类型，长度限制为1到64，仅永久二维码支持此字段
            /// </summary>
            public string scene_str;
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="toJsoner"></param>
            /// <param name="value"></param>
            [fastCSharp.emit.jsonSerialize.custom]
            private static void toJson(fastCSharp.emit.jsonSerializer toJsoner, sceneId value)
            {
                if (value.scene_str == null)
                {
                    toJsoner.UnsafeWriteFirstName("scene_id");
                    toJsoner.UnsafeToJsonNotNull(value.scene_id);
                }
                else
                {
                    toJsoner.UnsafeWriteFirstName("scene_str");
                    toJsoner.UnsafeToJsonNotNull(value.scene_str);
                }
                toJsoner.UnsafeCharStream.Write('}');
            }
        }
        /// <summary>
        /// 创建二维码，下载地址https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=TICKET
        /// </summary>
        /// <param name="scene_id">场景值ID，临时二维码时为32位非0整型，永久二维码时最大值为100000（目前参数只支持1--100000）</param>
        /// <param name="expire_seconds">该二维码有效时间，以秒为单位。0表示永久二维码， 最大不超过604800（即7天）。</param>
        /// <returns></returns>
        public qrCode CreateQrCode(uint scene_id, int expire_seconds)
        {
            if (scene_id != 0 && (uint)expire_seconds <= 604800 && (scene_id <= 100000 || expire_seconds != 0))
            {
                return requestJson<qrCode, qrCodeQuery>("https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token=", new qrCodeQuery { expire_seconds = expire_seconds, action_info = new actionInfo { scene = new sceneId { scene_id = scene_id } } });
            }
            return null;
        }
        /// <summary>
        /// 创建永久二维码，下载地址https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=TICKET
        /// </summary>
        /// <param name="scene_str">场景值ID（字符串形式的ID），字符串类型，长度限制为1到64</param>
        /// <returns></returns>
        public qrCode CreateQrCode(string scene_str)
        {
            if (((uint)scene_str.length() - 1) < 64)
            {
                return requestJson<qrCode, qrCodeQuery>("https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token=", new qrCodeQuery { action_info = new actionInfo { scene = new sceneId { scene_str = scene_str } } });
            }
            return null;
        }
        /// <summary>
        /// 长链接
        /// </summary>
        private struct longUrl
        {
            /// <summary>
            /// 此处填long2short，代表长链接转短链接
            /// </summary>
            public string action;
            /// <summary>
            /// 需要转换的长链接，支持http://、https://、weixin://wxpay 格式的url
            /// </summary>
            public string long_url;
        }
        /// <summary>
        /// 短链接
        /// </summary>
        private sealed class shortUrl : isValue
        {
#pragma warning disable
            /// <summary>
            /// 短链接
            /// </summary>
            public string short_url;
#pragma warning restore
        }
        /// <summary>
        /// 长链接转短链接
        /// </summary>
        /// <param name="long_url">需要转换的长链接，支持http://、https://、weixin://wxpay 格式的url</param>
        /// <returns></returns>
        public string GetShortUrl(string long_url)
        {
            if (!string.IsNullOrEmpty(long_url))
            {
                shortUrl value = requestJson<shortUrl, longUrl>("https://api.weixin.qq.com/cgi-bin/shorturl?access_token=", new longUrl { action = "long2short", long_url = long_url });
                if (value != null) return value.short_url;
            }
            return null;
        }
        /// <summary>
        /// 时间跨度
        /// </summary>
        private struct dateRange
        {
            /// <summary>
            /// 获取数据的起始日期，begin_date和end_date的差值需小于“最大时间跨度”（比如最大时间跨度为1时，begin_date和end_date的差值只能为0，才能小于1），否则会报错
            /// </summary>
            public string begin_date;
            /// <summary>
            /// 获取数据的结束日期，end_date允许设置的最大值为昨日
            /// </summary>
            public string end_date;
            /// <summary>
            /// 时间跨度检测
            /// </summary>
            /// <param name="begin_date"></param>
            /// <param name="days"></param>
            /// <param name="maxDays"></param>
            /// <returns></returns>
            public static dateRange Check(DateTime begin_date, byte days, byte maxDays)
            {
                if (--days < (byte)7)
                {
                    DateTime today = date.NowSecond;
                    if (begin_date < (today = new DateTime(today.Year, today.Month, today.Day)))
                    {
                        if (days == 0)
                        {
                            string dateString = begin_date.toDateString();
                            return new dateRange { begin_date = dateString, end_date = dateString };
                        }
                        DateTime endDate = begin_date.AddDays(days);
                        if (endDate >= today) endDate = today.AddDays(-1);
                        return new dateRange { begin_date = begin_date.toDateString(), end_date = endDate.toDateString() };
                    }
                }
                return default(dateRange);
            }
            /// <summary>
            /// 时间检测
            /// </summary>
            /// <param name="begin_date"></param>
            /// <returns></returns>
            public static string ToString(DateTime begin_date)
            {
                DateTime today = date.NowSecond;
                return begin_date < (today = new DateTime(today.Year, today.Month, today.Day)) ? begin_date.toDateString('-') : null;
            }
        }
#pragma warning disable
        /// <summary>
        /// 用户增减数据
        /// </summary>
        private sealed class userSummaryResult : isValue
        {
            /// <summary>
            /// 用户增减数据
            /// </summary>
            public userSummary[] list;
        }
#pragma warning restore
        /// <summary>
        /// 获取用户增减数据
        /// </summary>
        /// <param name="begin_date"></param>
        /// <param name="days">最大时间跨度为7天</param>
        /// <returns></returns>
        public userSummary[] GetUserSummary(DateTime begin_date, byte days = 7)
        {
            dateRange dateRange = dateRange.Check(begin_date, days, 7);
            if (dateRange.begin_date != null)
            {
                userSummaryResult value = requestJson<userSummaryResult, dateRange>("https://api.weixin.qq.com/datacube/getusersummary?access_token=", dateRange);
                if (value != null) return value.list;
            }
            return null;
        }
        /// <summary>
        /// 累计用户数据
        /// </summary>
        private sealed class userCumulateResult : isValue
        {
#pragma warning disable
            /// <summary>
            /// 累计用户数据
            /// </summary>
            public userCumulate[] list;
#pragma warning restore
        }
        /// <summary>
        /// 获取累计用户数据
        /// </summary>
        /// <param name="begin_date"></param>
        /// <param name="days">最大时间跨度为7天</param>
        /// <returns></returns>
        public userCumulate[] GetUserCumulate(DateTime begin_date, byte days = 7)
        {
            dateRange dateRange = dateRange.Check(begin_date, days, 7);
            if (dateRange.begin_date != null)
            {
                userCumulateResult value = requestJson<userCumulateResult, dateRange>("https://api.weixin.qq.com/datacube/getusercumulate?access_token=", dateRange);
                if (value != null) return value.list;
            }
            return null;
        }
        /// <summary>
        /// 图文群发每日数据
        /// </summary>
        private sealed class articleSummaryResult : isValue
        {
#pragma warning disable
            /// <summary>
            /// 图文群发每日数据
            /// </summary>
            public articleSummary[] list;
#pragma warning restore
        }
        /// <summary>
        /// 获取图文群发每日数据，某天所有被阅读过的文章（仅包括群发的文章）在当天的阅读次数等数据
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public articleSummary GetArticleSummary(DateTime date)
        {
            string dateString = dateRange.ToString(date);
            if (dateString != null)
            {
                articleSummaryResult value = requestJson<articleSummaryResult, dateRange>("https://api.weixin.qq.com/datacube/getarticlesummary?access_token=", new dateRange { begin_date = dateString, end_date = dateString });
                if (value != null && value.list.length() != 0) return value.list[0];
            }
            return null;
        }
        /// <summary>
        /// 图文群发总数据
        /// </summary>
        private sealed class articleTotalResult : isValue
        {
#pragma warning disable
            /// <summary>
            /// 图文群发总数据
            /// </summary>
            public articleTotal[] list;
#pragma warning restore
        }
        /// <summary>
        /// 获取图文群发总数据，某天群发的文章，从群发日起到接口调用日（但最多统计发表日后7天数据），每天的到当天的总等数据。例如某篇文章是12月1日发出的，发出后在1日、2日、3日的阅读次数分别为1万，则getarticletotal获取到的数据为，距发出到12月1日24时的总阅读量为1万，距发出到12月2日24时的总阅读量为2万，距发出到12月1日24时的总阅读量为3万。
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public articleTotal GetArticleTotal(DateTime date)
        {
            string dateString = dateRange.ToString(date);
            if (dateString != null)
            {
                articleTotalResult value = requestJson<articleTotalResult, dateRange>("https://api.weixin.qq.com/datacube/getarticletotal?access_token=", new dateRange { begin_date = dateString, end_date = dateString });
                if (value != null && value.list.length() != 0) return value.list[0];
            }
            return null;
        }
        /// <summary>
        /// 图文统计数据
        /// </summary>
        private sealed class userReadResult : isValue
        {
#pragma warning disable
            /// <summary>
            /// 图文统计数据
            /// </summary>
            public userRead[] list;
#pragma warning restore
        }
        /// <summary>
        /// 获取图文统计数据
        /// </summary>
        /// <param name="begin_date"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public userRead[] GetUserRead(DateTime begin_date, byte days = 3)
        {
            dateRange dateRange = dateRange.Check(begin_date, days, 3);
            if (dateRange.begin_date != null)
            {
                userReadResult value = requestJson<userReadResult, dateRange>("https://api.weixin.qq.com/datacube/getuserread?access_token=", dateRange);
                if (value != null) return value.list;
            }
            return null;
        }
        /// <summary>
        /// 图文统计分时数据
        /// </summary>
        private sealed class userReadHourResult : isValue
        {
#pragma warning disable
            /// <summary>
            /// 图文统计分时数据
            /// </summary>
            public userRead.hour[] list;
#pragma warning restore
        }
        /// <summary>
        /// 获取图文统计分时数据
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public userRead.hour[] GetUserReadHour(DateTime date)
        {
            string dateString = dateRange.ToString(date);
            if (dateString != null)
            {
                userReadHourResult value = requestJson<userReadHourResult, dateRange>("https://api.weixin.qq.com/datacube/getuserreadhour?access_token=", new dateRange { begin_date = dateString, end_date = dateString });
                if (value != null) return value.list;
            }
            return null;
        }
        /// <summary>
        /// 图文分享转发数据
        /// </summary>
        private sealed class userShareResult : isValue
        {
#pragma warning disable
            /// <summary>
            /// 图文分享转发数据
            /// </summary>
            public userShare[] list;
#pragma warning restore
        }
        /// <summary>
        /// 获取图文分享转发数据
        /// </summary>
        /// <param name="begin_date"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public userShare[] GetUserShare(DateTime begin_date, byte days = 7)
        {
            dateRange dateRange = dateRange.Check(begin_date, days, 7);
            if (dateRange.begin_date != null)
            {
                userShareResult value = requestJson<userShareResult, dateRange>("https://api.weixin.qq.com/datacube/getusershare?access_token=", dateRange);
                if (value != null) return value.list;
            }
            return null;
        }
        /// <summary>
        /// 图文统计分时数据
        /// </summary>
        private sealed class userShareHourResult : isValue
        {
#pragma warning disable
            /// <summary>
            /// 图文统计分时数据
            /// </summary>
            public userShare.hour[] list;
#pragma warning restore
        }
        /// <summary>
        /// 获取图文分享转发分时数据
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public userShare.hour[] GetUserShareHour(DateTime date)
        {
            string dateString = dateRange.ToString(date);
            if (dateString != null)
            {
                userShareHourResult value = requestJson<userShareHourResult, dateRange>("https://api.weixin.qq.com/datacube/getusersharehour?access_token=", new dateRange { begin_date = dateString, end_date = dateString });
                if (value != null) return value.list;
            }
            return null;
        }
        /// <summary>
        /// 消息发送概况数据
        /// </summary>
        private sealed class messageCountResult : isValue
        {
#pragma warning disable
            /// <summary>
            /// 消息发送概况数据
            /// </summary>
            public messageCount[] list;
#pragma warning restore
        }
        /// <summary>
        /// 获取消息发送概况数据
        /// </summary>
        /// <param name="begin_date"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public messageCount[] GetMessageCount(DateTime begin_date, byte days = 7)
        {
            dateRange dateRange = dateRange.Check(begin_date, days, 7);
            if (dateRange.begin_date != null)
            {
                messageCountResult value = requestJson<messageCountResult, dateRange>("https://api.weixin.qq.com/datacube/getupstreammsg?access_token=", dateRange);
                if (value != null) return value.list;
            }
            return null;
        }
        /// <summary>
        /// 消息分送分时数据
        /// </summary>
        private sealed class messageCountHourResult : isValue
        {
#pragma warning disable
            /// <summary>
            /// 消息分送分时数据
            /// </summary>
            public messageCount.hour[] list;
#pragma warning restore
        }
        /// <summary>
        /// 获取消息分送分时数据
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public messageCount.hour[] GetMessageCountHour(DateTime date)
        {
            string dateString = dateRange.ToString(date);
            if (dateString != null)
            {
                messageCountHourResult value = requestJson<messageCountHourResult, dateRange>("https://api.weixin.qq.com/datacube/getupstreammsghour?access_token=", new dateRange { begin_date = dateString, end_date = dateString });
                if (value != null) return value.list;
            }
            return null;
        }
        /// <summary>
        /// 获取消息发送周数据
        /// </summary>
        /// <param name="begin_date"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public messageCount[] GetMessageCountWeek(DateTime begin_date, byte days = 30)
        {
            dateRange dateRange = dateRange.Check(begin_date, days, 30);
            if (dateRange.begin_date != null)
            {
                messageCountResult value = requestJson<messageCountResult, dateRange>("https://api.weixin.qq.com/datacube/getupstreammsgweek?access_token=", dateRange);
                if (value != null) return value.list;
            }
            return null;
        }
        /// <summary>
        /// 获取消息发送月数据
        /// </summary>
        /// <param name="begin_date"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public messageCount[] GetMessageCountMonth(DateTime begin_date, byte days = 30)
        {
            dateRange dateRange = dateRange.Check(begin_date, days, 30);
            if (dateRange.begin_date != null)
            {
                messageCountResult value = requestJson<messageCountResult, dateRange>("https://api.weixin.qq.com/datacube/getupstreammsgmonth?access_token=", dateRange);
                if (value != null) return value.list;
            }
            return null;
        }
        /// <summary>
        /// 消息发送分布数据
        /// </summary>
        private sealed class messageDistributedResult : isValue
        {
#pragma warning disable
            /// <summary>
            /// 消息发送分布数据
            /// </summary>
            public messageDistributed[] list;
#pragma warning restore
        }
        /// <summary>
        /// 获取消息发送分布数据
        /// </summary>
        /// <param name="begin_date"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public messageDistributed[] GetMessageDistributed(DateTime begin_date, byte days = 15)
        {
            dateRange dateRange = dateRange.Check(begin_date, days, 15);
            if (dateRange.begin_date != null)
            {
                messageDistributedResult value = requestJson<messageDistributedResult, dateRange>("https://api.weixin.qq.com/datacube/getupstreammsgdist?access_token=", dateRange);
                if (value != null) return value.list;
            }
            return null;
        }
        /// <summary>
        /// 获取消息发送分布周数据
        /// </summary>
        /// <param name="begin_date"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public messageDistributed[] GetMessageDistributedWeek(DateTime begin_date, byte days = 30)
        {
            dateRange dateRange = dateRange.Check(begin_date, days, 30);
            if (dateRange.begin_date != null)
            {
                messageDistributedResult value = requestJson<messageDistributedResult, dateRange>("https://api.weixin.qq.com/datacube/getupstreammsgdistweek?access_token=", dateRange);
                if (value != null) return value.list;
            }
            return null;
        }
        /// <summary>
        /// 获取消息发送分布月数据
        /// </summary>
        /// <param name="begin_date"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public messageDistributed[] GetMessageDistributedMonth(DateTime begin_date, byte days = 30)
        {
            dateRange dateRange = dateRange.Check(begin_date, days, 30);
            if (dateRange.begin_date != null)
            {
                messageDistributedResult value = requestJson<messageDistributedResult, dateRange>("https://api.weixin.qq.com/datacube/getupstreammsgdistmonth?access_token=", dateRange);
                if (value != null) return value.list;
            }
            return null;
        }
        /// <summary>
        /// 接口分析数据
        /// </summary>
        private sealed class interfaceSummaryResult : isValue
        {
#pragma warning disable
            /// <summary>
            /// 接口分析数据
            /// </summary>
            public interfaceSummary[] list;
#pragma warning restore
        }
        /// <summary>
        /// 获取接口分析数据
        /// </summary>
        /// <param name="begin_date"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public interfaceSummary[] GetInterfaceSummary(DateTime begin_date, byte days = 30)
        {
            dateRange dateRange = dateRange.Check(begin_date, days, 30);
            if (dateRange.begin_date != null)
            {
                interfaceSummaryResult value = requestJson<interfaceSummaryResult, dateRange>("https://api.weixin.qq.com/datacube/getinterfacesummary?access_token=", dateRange);
                if (value != null) return value.list;
            }
            return null;
        }
        /// <summary>
        /// 接口分析分时数据
        /// </summary>
        private sealed class interfaceSummaryHourResult : isValue
        {
#pragma warning disable
            /// <summary>
            /// 接口分析分时数据
            /// </summary>
            public interfaceSummary.hour[] list;
#pragma warning restore
        }
        /// <summary>
        /// 获取接口分析分时数据
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public interfaceSummary.hour[] GetInterfaceSummaryHour(DateTime date)
        {
            string dateString = dateRange.ToString(date);
            if (dateString != null)
            {
                interfaceSummaryHourResult value = requestJson<interfaceSummaryHourResult, dateRange>("https://api.weixin.qq.com/datacube/getinterfacesummaryhour?access_token=", new dateRange { begin_date = dateString, end_date = dateString });
                if (value != null) return value.list;
            }
            return null;
        }
        /// <summary>
        /// 为多客服的客服工号创建会话
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public bool CreateCustomSession(customSession session)
        {
            isValue value = requestJson<isValue, customSession>("https://api.weixin.qq.com/customservice/kfsession/create?access_token=", session);
            return value != null && value.IsValue;
        }
        /// <summary>
        /// 关闭多客服的客服工号会话
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public bool CloseCustomSession(customSession session)
        {
            isValue value = requestJson<isValue, customSession>("https://api.weixin.qq.com/customservice/kfsession/close?access_token=", session);
            return value != null && value.IsValue;
        }
        /// <summary>
        /// 获取客户的会话状态
        /// </summary>
        /// <param name="openid">客户openid</param>
        /// <returns></returns>
        public customSessionTime GetCustomSession(string openid)
        {
            if (!string.IsNullOrEmpty(openid))
            {
                return request<customSessionTime>("https://api.weixin.qq.com/customservice/kfsession/getsession?openid=" + openid + "&access_token=");
            }
            return null;
        }
        /// <summary>
        /// 客服的会话列表
        /// </summary>
        private sealed class customListResult : isValue
        {
#pragma warning disable
            /// <summary>
            /// 会话列表
            /// </summary>
            public customTime[] sessionlist;
#pragma warning restore
        }
        /// <summary>
        /// 获取客服的会话列表
        /// </summary>
        /// <param name="kf_account">完整客服账号，格式为：账号前缀@公众号微信号，账号前缀最多10个字符，必须是英文或者数字字符。</param>
        /// <returns></returns>
        public customTime[] GetSessionList(string kf_account)
        {
            if (!string.IsNullOrEmpty(kf_account))
            {
                customListResult value = request<customListResult>("https://api.weixin.qq.com/customservice/kfsession/getsessionlist?kf_account=" + kf_account + "&access_token=");
                if (value != null) return value.sessionlist;
            }
            return null;
        }
        /// <summary>
        /// 获取未接入会话列表
        /// </summary>
        /// <returns></returns>
        private waitCustomSession GetWaitSession()
        {
            return request<waitCustomSession>("https://api.weixin.qq.com/customservice/kfsession/getwaitcase?access_token=");
        }
        /// <summary>
        /// 客服聊天记录查询
        /// </summary>
        private struct recordQuery
        {
            /// <summary>
            /// 查询开始时间，UNIX时间戳
            /// </summary>
            public long starttime;
            /// <summary>
            /// 查询结束时间，UNIX时间戳，每次查询不能跨日查询
            /// </summary>
            public long endtime;
            /// <summary>
            /// 查询第几页，从1开始
            /// </summary>
            public int pageindex;
            /// <summary>
            /// 每页大小，每页最多拉取50条
            /// </summary>
            public byte pagesize;
        }
        /// <summary>
        /// 客服聊天记录
        /// </summary>
        private sealed class customRecordList : isValue
        {
#pragma warning disable
            /// <summary>
            /// 客服聊天记录
            /// </summary>
            public customRecord[] recordlist;
#pragma warning restore
        }
        /// <summary>
        /// 获取客服聊天记录
        /// </summary>
        /// <param name="starttime">查询开始时间</param>
        /// <param name="endtime">查询结束时间，每次查询不能跨日查询</param>
        /// <param name="pageindex">查询第几页，从1开始</param>
        /// <param name="pagesize">每页大小，每页最多拉取50条</param>
        /// <returns></returns>
        public customRecord[] GetCustomRecord(DateTime starttime, DateTime endtime, int pageindex = 1, byte pagesize = 50)
        {
            if (((starttime.Year ^ endtime.Year) | (starttime.Month ^ endtime.Month) | (starttime.Day ^ endtime.Day)) == 0 && pageindex > 0 && (uint)(pagesize - 1) < 50)
            {
                customRecordList value = requestJson<customRecordList, recordQuery>("https://api.weixin.qq.com/customservice/msgrecord/getrecord?access_token=", new recordQuery { starttime = (long)(starttime - config.MinTime).TotalSeconds, endtime = (long)(endtime - config.MinTime).TotalSeconds, pageindex = pageindex, pagesize = pagesize });
                if (value != null) return value.recordlist;
            }
            return null;
        }
        /// <summary>
        /// 申请开通摇一摇周边功能。成功提交申请请求后，工作人员会在三个工作日内完成审核。
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool RegisterShakeAround(shakeAroundAccount account)
        {
            isValue value = requestJson<isValue, shakeAroundAccount>("https://api.weixin.qq.com/shakearound/account/register?access_token=", account);
            return value != null && value.IsValue;
        }
        /// <summary>
        /// 查询申请开通摇一摇周边审核状态
        /// </summary>
        private sealed class shakeAroundAccountStatusResult : isValue
        {
#pragma warning disable
            /// <summary>
            /// 查询申请开通摇一摇周边审核状态
            /// </summary>
            public shakeAroundAccountStatus data;
#pragma warning restore
        }
        /// <summary>
        /// 查询申请开通摇一摇周边审核状态
        /// </summary>
        /// <returns></returns>
        public shakeAroundAccountStatus GetShakeAroundAccountStatus()
        {
            shakeAroundAccountStatusResult value = request<shakeAroundAccountStatusResult>("https://api.weixin.qq.com/shakearound/account/auditstatus?access_token=");
            return value == null ? null : value.data;
        }

        /// <summary>
        /// XML序列化配置
        /// </summary>
        private static readonly fastCSharp.emit.xmlSerializer.config xmlSerializeConfig = new emit.xmlSerializer.config { CheckLoopDepth = fastCSharp.config.appSetting.SerializeDepth, Header = null, IsOutputEmptyString = false };
        /// <summary>
        /// 获取交易会话标识
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetPrePayId(prePayIdQuery value)
        {
            value.SetConfig(config);
            prePayId prePayId = config.Request.RequestXml<prePayId, prePayIdQuery>("https://api.mch.weixin.qq.com/pay/unifiedorder", value, xmlSerializeConfig);
            return prePayId != null && prePayId.Verify(config) ? prePayId.prepay_id : null;
        }
        /// <summary>
        /// 交易会话信息
        /// </summary>
        public sealed class appPrePayIdOrder
        {
            /// <summary>
            /// 交易会话信息
            /// </summary>
            /// <param name="config"></param>
            /// <param name="prePayId"></param>
            /// <param name="noncestr"></param>
            internal appPrePayIdOrder(config config, string prePayId, string noncestr)
            {
                appid = config.appid;
                partnerid = config.partnerid;
                prepayid = prePayId;
                this.noncestr = noncestr;
                timestamp = (((date.Now.Ticks) - fastCSharp.web.ajax.JavascriptLocalMinTimeTicks) / date.SecondTicks).toString();
                sign<appPrePayIdOrder>.Set(this, config.key);
            }
            /// <summary>
            /// 公众账号ID
            /// </summary>
            public string appid;
            /// <summary>
            /// 
            /// </summary>
            public string partnerid;
            /// <summary>
            /// 交易会话标识
            /// </summary>
            public string prepayid;
            /// <summary>
            /// 随机字符串
            /// </summary>
            public string noncestr;
            /// <summary>
            /// 时间戳
            /// </summary>
            public string timestamp;
            /// <summary>
            /// 
            /// </summary>
            public readonly string package = "Sign=WXPay";
            /// <summary>
            /// 数据签名
            /// </summary>
            public string sign;
        }
        /// <summary>
        /// 获取交易会话标识
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public appPrePayIdOrder GetPrePayId(appPrePayIdQuery value)
        {
            value.SetConfig(config);
            appPrePayId prePayId = config.Request.RequestXml<appPrePayId, appPrePayIdQuery>("https://api.mch.weixin.qq.com/pay/unifiedorder", value, xmlSerializeConfig);
            return prePayId != null && prePayId.Verify(config) ? new appPrePayIdOrder(config, prePayId.prepay_id, value.nonce_str) : null;
        }
        /// <summary>
        /// 订单查询信息
        /// </summary>
        /// <param name="transaction_id">微信的订单号</param>
        /// <returns></returns>
        public orderResult GetOrder(string transaction_id)
        {
            string xml;
            return getOrder(new orderQuery { transaction_id = transaction_id }, out xml);
        }
        /// <summary>
        /// 订单查询信息
        /// </summary>
        /// <param name="transaction_id">微信的订单号</param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public orderResult GetOrder(string transaction_id, out string xml)
        {
            return getOrder(new orderQuery { transaction_id = transaction_id }, out xml);
        }
        /// <summary>
        /// 订单查询信息
        /// </summary>
        /// <param name="out_trade_no">商户系统内部的订单号</param>
        /// <returns></returns>
        public orderResult GetOrderByLocal(string out_trade_no)
        {
            string xml;
            return getOrder(new orderQuery { out_trade_no = out_trade_no }, out xml);
        }
        /// <summary>
        /// 订单查询信息
        /// </summary>
        /// <param name="out_trade_no">商户系统内部的订单号</param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public orderResult GetOrderByLocal(string out_trade_no, out string xml)
        {
            return getOrder(new orderQuery { out_trade_no = out_trade_no }, out xml);
        }
        /// <summary>
        /// 订单查询信息
        /// </summary>
        /// <param name="value"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        private orderResult getOrder(orderQuery value, out string xml)
        {
            value.SetConfig(config);
            orderResult result = config.Request.RequestXml<orderResult, orderQuery>("https://api.mch.weixin.qq.com/pay/orderquery", value, out xml, xmlSerializeConfig);
            return result != null && result.Verify(config) ? result : null;
        }
        /// <summary>
        /// 关闭订单结果
        /// </summary>
        private sealed class closeOrderResult : returnSign
        {
            /// <summary>
            /// 签名验证
            /// </summary>
            /// <param name="config"></param>
            /// <returns></returns>
            internal bool Verify(config config)
            {
                if (IsValue)
                {
                    if (appid == config.appid && mch_id == config.mch_id && sign<closeOrderResult>.Check(this, config.key, sign)) return true;
                    log.Error.Add("签名验证错误 " + this.ToJson(), new System.Diagnostics.StackFrame() , false);
                }
                return false;
            }
        }
        /// <summary>
        /// 关闭订单，订单生成后不能马上调用关单接口，最短调用时间间隔为5分钟
        /// </summary>
        /// <param name="out_trade_no">商户订单号</param>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public bool CloseOrder(string out_trade_no, out errorCode.code errorCode)
        {
            orderQuery value = new orderQuery { out_trade_no = out_trade_no };
            value.SetConfig(config);
            closeOrderResult result = config.Request.RequestXml<closeOrderResult, orderQuery>("https://api.mch.weixin.qq.com/pay/closeorder", value, xmlSerializeConfig, false);
            if (result != null)
            {
                errorCode = result.err_code == null ? weixin.errorCode.code.Unknown : (errorCode.code)result.err_code;
                if (result.Verify(config)) return true;
            }
            else errorCode = weixin.errorCode.code.Unknown;
            return false;
        }
        //申请退款，请求需要双向证书 https://pay.weixin.qq.com/wiki/doc/api/native.php?chapter=9_4
        /// <summary>
        /// 查询退款
        /// </summary>
        [fastCSharp.emit.xmlSerialize(Filter = fastCSharp.code.memberFilters.InstanceField, IsAllMember = true)]
        private sealed class refundQuery : signQuery
        {
            /// <summary>
            /// 调用接口提交的终端设备号
            /// </summary>
            internal string device_info;
            /// <summary>
            /// 微信支付订单号
            /// </summary>
            internal string transaction_id;
            /// <summary>
            /// 商户系统的订单号，与请求一致。
            /// </summary>
            internal string out_trade_no;
            /// <summary>
            /// 商户退款单号
            /// </summary>
            internal string out_refund_no;
            /// <summary>
            /// 微信退款单号
            /// </summary>
            internal string refund_id;
            /// <summary>
            /// 设置应用配置
            /// </summary>
            /// <param name="config">应用配置</param>
            internal void SetConfig(config config)
            {
                setConfig(config);
                sign<refundQuery>.Set(this, config.key);
            }
        }
        /// <summary>
        /// 查询退款，用零钱支付的退款20分钟内到账，银行卡支付的退款3个工作日后重新查询退款状态。
        /// </summary>
        /// <param name="transaction_id">微信订单号</param>
        /// <param name="device_info">调用接口提交的终端设备号</param>
        /// <returns></returns>
        public refundResult GetRefundByOrder(string transaction_id, string device_info = "WEB")
        {
            return getRefund(new refundQuery { transaction_id = transaction_id, device_info = device_info });
        }
        /// <summary>
        /// 查询退款，用零钱支付的退款20分钟内到账，银行卡支付的退款3个工作日后重新查询退款状态。
        /// </summary>
        /// <param name="out_trade_no">商户订单号</param>
        /// <param name="device_info">调用接口提交的终端设备号</param>
        /// <returns></returns>
        public refundResult GetRefundByLocalOrder(string out_trade_no, string device_info = "WEB")
        {
            return getRefund(new refundQuery { out_trade_no = out_trade_no, device_info = device_info });
        }
        /// <summary>
        /// 查询退款，用零钱支付的退款20分钟内到账，银行卡支付的退款3个工作日后重新查询退款状态。
        /// </summary>
        /// <param name="refund_id">微信生成的退款单号</param>
        /// <param name="device_info">调用接口提交的终端设备号</param>
        /// <returns></returns>
        public refundResult GetRefund(string refund_id, string device_info = "WEB")
        {
            return getRefund(new refundQuery { refund_id = refund_id, device_info = device_info });
        }
        /// <summary>
        /// 查询退款，用零钱支付的退款20分钟内到账，银行卡支付的退款3个工作日后重新查询退款状态。
        /// </summary>
        /// <param name="out_refund_no">商户侧传给微信的退款单号</param>
        /// <param name="device_info">调用接口提交的终端设备号</param>
        /// <returns></returns>
        public refundResult GetRefundByLocal(string out_refund_no, string device_info = "WEB")
        {
            return getRefund(new refundQuery { out_refund_no = out_refund_no, device_info = device_info });
        }
        /// <summary>
        /// 查询退款，用零钱支付的退款20分钟内到账，银行卡支付的退款3个工作日后重新查询退款状态。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private refundResult getRefund(refundQuery value)
        {
            value.SetConfig(config);
            string xml;
            refundResult result = config.Request.RequestXml<refundResult, refundQuery>("https://api.mch.weixin.qq.com/pay/refundquery", value, out xml, xmlSerializeConfig);
            return result != null && result.Verify(config) ? result : null;
        }
        /// <summary>
        /// 下载对账单
        /// </summary>
        [fastCSharp.emit.xmlSerialize(Filter = fastCSharp.code.memberFilters.InstanceField, IsAllMember = true)]
        private sealed class billQuery : signQuery
        {
            /// <summary>
            /// 终端设备号(门店号或收银设备ID)，注意：PC网页或公众号内支付请传"WEB"
            /// </summary>
            public string device_info;
            /// <summary>
            /// 下载对账单的日期，格式：20140603
            /// </summary>
            public string bill_date;
            /// <summary>
            /// 账单类型
            /// </summary>
            public billTotal.billType bill_type;
            /// <summary>
            /// 设置应用配置
            /// </summary>
            /// <param name="config">应用配置</param>
            internal void SetConfig(config config)
            {
                setConfig(config);
                sign<billQuery>.Set(this, config.key);
            }
        }
        /// <summary>
        /// 对账单数据分割
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private unsafe static subArray<subString> splitBill(subString value)
        {
            subArray<subString> values = default(subArray<subString>);
            if (value.Length > 1)
            {
                fixed (char* valueFixed = value.Value)
                {
                    char* start = valueFixed + value.StartIndex;
                    if (*start == '`')
                    {
                        char* end = start + value.Length, data = ++start;
                        while (++start != end)
                        {
                            if (*start == '`' && *(start - 1) == ',')
                            {
                                values.Add(subString.Unsafe(value.Value, (int)(data - valueFixed), (int)(start - data) - 1));
                                data = ++start;
                                if (start == end) break;
                            }
                        }
                        if (*(end - 1) == '\r') --end;
                        values.Add(subString.Unsafe(value.Value, (int)(data - valueFixed), (int)(end - data)));
                    }
                }
            }
            return values;
        }
        /// <summary>
        /// 下载对账单
        /// </summary>
        /// <param name="type"></param>
        /// <param name="date"></param>
        /// <param name="device_info"></param>
        /// <returns>对账单统计数据 https://pay.weixin.qq.com/wiki/doc/api/native.php?chapter=9_6</returns>
        public billTotal DownloadBill(billTotal.billType type, DateTime date, string device_info = "WEB")
        {
            billQuery query = new billQuery { bill_type = type, bill_date = date.ToString("yyyyMMdd"), device_info = device_info };
            query.SetConfig(config);
            string text = config.Request.RequestXml<billQuery>("https://api.mch.weixin.qq.com/pay/downloadbill", query, xmlSerializeConfig);
            returnCode returnCode = fastCSharp.emit.xmlParser.Parse<returnCode>(text);
            if (returnCode == null)
            {
                subArray<subString> rows = text.split('\n');
                if (rows.Count >= 3)
                {
                    subString[] rowArray = rows.UnsafeArray;
                    int endIndex = rows.Count - 1;
                    if (rowArray[endIndex].Length == 0) rows.UnsafeSetLength(endIndex--);
                    if (endIndex >= 2 && billTotal.CheckName(rowArray[endIndex - 1].TrimEnd('\r')) && billTotal.bill.CheckName(rowArray[0].TrimEnd('\r'), type))
                    {
                        try
                        {
                            billTotal.bill[] bills = new billTotal.bill[endIndex - 2];
                            for (int index = 1; index != endIndex; ++index) bills[index - 1] = new billTotal.bill(splitBill(rowArray[index]), type);
                            return new billTotal(splitBill(rowArray[endIndex]), bills);
                        }
                        catch (Exception error)
                        {
                            log.Error.Add(error, text, false);
                        }
                        return null;
                    }
                }
            }
            log.Default.Add(text, new System.Diagnostics.StackFrame(), false);
            return null;
        }
        /// <summary>
        /// 二维码长链接
        /// </summary>
        [fastCSharp.emit.xmlSerialize(Filter = fastCSharp.code.memberFilters.InstanceField, IsAllMember = true)]
        private sealed class qrCodeLongUrl : signQuery
        {
            /// <summary>
            /// 需要转换的URL，签名用原串，传输需URLencode
            /// </summary>
            public string long_url;
            /// <summary>
            /// 设置应用配置
            /// </summary>
            /// <param name="config">应用配置</param>
            internal void SetConfig(config config)
            {
                setConfig(config);
                sign<qrCodeLongUrl>.Set(this, config.key);
            }
        }
        /// <summary>
        /// 二维码短链接
        /// </summary>
        private sealed class qrCodeShortUrl : returnSign
        {
#pragma warning disable
            /// <summary>
            /// 短链接
            /// </summary>
            public string short_url;
#pragma warning restore
            /// <summary>
            /// 签名验证
            /// </summary>
            /// <param name="config"></param>
            /// <returns></returns>
            internal bool Verify(config config)
            {
                if (IsValue)
                {
                    if (appid == config.appid && mch_id == config.mch_id && sign<qrCodeShortUrl>.Check(this, config.key, sign)) return true;
                    log.Error.Add("签名验证错误 " + this.ToJson(), new System.Diagnostics.StackFrame(), false);
                }
                return false;
            }
        }
        /// <summary>
        /// 二维码长链接转短链接
        /// </summary>
        /// <param name="long_url"></param>
        /// <returns></returns>
        public string GetQrCodeShortUrl(string long_url)
        {
            if (!string.IsNullOrEmpty(long_url))
            {
                qrCodeLongUrl url = new qrCodeLongUrl { long_url = long_url };
                url.SetConfig(config);
                qrCodeShortUrl shortUrl = config.Request.RequestXml<qrCodeShortUrl, qrCodeLongUrl>("https://api.mch.weixin.qq.com/tools/shorturl", url, xmlSerializeConfig);
                if (shortUrl != null && shortUrl.Verify(config)) return shortUrl.short_url;
            }
            return null;
        }

    }
}
