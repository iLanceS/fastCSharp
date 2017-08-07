using System;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using System.Collections.Generic;
using fastCSharp.io.compression;
using fastCSharp.web;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace fastCSharp.net
{
    /// <summary>
    /// WebClient相关操作
    /// </summary>
    public class webClient : WebClient
    {
        /// <summary>
        /// 默认浏览器参数
        /// </summary>
        public const string DefaultUserAgent = @"Mozilla/5.0 (Windows NT 5.2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.93 Safari/537.36";//@"Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1;)";
        /// <summary>
        /// 字符集标识
        /// </summary>
        public const string CharsetName = "charset=";
        /// <summary>
        /// 默认表单提交内容类型
        /// </summary>
        public const string DefaultPostContentType = "application/x-www-form-urlencoded";
        /// <summary>
        /// 空页面地址
        /// </summary>
        public const string BlankUrl = "about:blank";
        /// <summary>
        /// ServicePointManager.Expect100Continue访问锁
        /// </summary>
        public static readonly object Expect100ContinueLock = new object();

        /// <summary>
        /// cookie状态
        /// </summary>
        public CookieContainer Cookies { get; private set; }
        /// <summary>
        /// 超时毫秒数
        /// </summary>
        public int TimeOut;
        /// <summary>
        /// 浏览器参数
        /// </summary>
        public string UserAgent = DefaultUserAgent;
        /// <summary>
        /// HTTP请求
        /// </summary>
        private WebRequest webRequest;
        /// <summary>
        /// HTTP请求
        /// </summary>
        public HttpWebRequest HttpRequest
        {
            get
            {
                return webRequest == null ? null : webRequest as HttpWebRequest;
            }
        }
        /// <summary>
        /// 导入证书
        /// </summary>
        public X509Certificate Certificate;
        ///// <summary>
        ///// HTTP回应
        ///// </summary>
        //private WebResponse response;
        /// <summary>
        /// 是否允许跳转
        /// </summary>
        public bool AllowAutoRedirect = true;
        /// <summary>
        /// 是否保持连接
        /// </summary>
        public bool KeepAlive = true;
        /// <summary>
        /// 获取最后一次操作是否发生重定向
        /// </summary>
        public bool IsRedirect
        {
            get
            {
                return webRequest != null && webRequest is HttpWebRequest
                    && webRequest.RequestUri.Equals((webRequest as HttpWebRequest).Address);
            }
        }
        /// <summary>
        /// 获取最后一次重定向地址
        /// </summary>
        public Uri RedirectUri
        {
            get
            {
                return IsRedirect ? (webRequest as HttpWebRequest).Address : null;
            }
        }
        /// <summary>
        /// HTTP回应压缩流处理
        /// </summary>
        private stream compressionStream
        {
            get
            {
                if (ResponseHeaders != null)
                {
                    string contentEncoding = ResponseHeaders[header.ContentEncoding];
                    if (contentEncoding != null)
                    {
                        if (contentEncoding.Length == 4)
                        {
                            return contentEncoding == "gzip" ? stream.GZip : null;
                        }
                        return contentEncoding == "deflate" ? stream.Deflate : null;
                    }
                }
                return null;
            }
        }
        /// <summary>
        /// HTTP回应编码字符集
        /// </summary>
        public Encoding TextEncoding
        {
            get
            {
                if (ResponseHeaders != null)
                {
                    string contentType = ResponseHeaders[header.ContentType];
                    if (contentType != null) return getEncoding(contentType);
                }
                return null;
            }
        }
        /// <summary>
        /// 获取重定向地址
        /// </summary>
        public string Location
        {
            get
            {
                WebResponse response = webRequest == null ? null : webRequest.GetResponse();
                return response == null ? null : response.Headers[header.Location];
            }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="cookies">cookie状态</param>
        /// <param name="proxy">代理</param>
        public webClient(CookieContainer cookies = null, WebProxy proxy = null)
        {
            Credentials = new CredentialCache();
            Cookies = cookies == null ? new CookieContainer() : cookies;
            Credentials = CredentialCache.DefaultCredentials;

            Proxy = proxy;
            //string header = client.ResponseHeaders[web.header.SetCookie];
            //client.Headers.Add(header.Cookie, header);
        }
        /// <summary>
        /// 获取HTTP请求
        /// </summary>
        /// <param name="address">URI地址</param>
        /// <returns>HTTP请求</returns>
        protected override WebRequest GetWebRequest(Uri address)
        {
            webRequest = base.GetWebRequest(address);
            HttpWebRequest request = HttpRequest;
            if (request != null)
            {
                request.KeepAlive = KeepAlive;
                request.AllowAutoRedirect = AllowAutoRedirect;
                request.CookieContainer = Cookies;
                if (Certificate != null)
                {
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = onValidateCertificate;
                    request.ClientCertificates.Add(Certificate);
                }
                if (TimeOut > 0) request.Timeout = TimeOut;
            }
            return request;
        }
        /// <summary>
        /// 安全连接证书验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        private static bool onValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return sslPolicyErrors == SslPolicyErrors.None;
        }
        ///// <summary>
        ///// 获取HTTP回应
        ///// </summary>
        ///// <param name="request">HTTP请求</param>
        ///// <returns>HTTP回应</returns>
        //protected override WebResponse GetWebResponse(WebRequest request)
        //{
        //    Response = base.GetWebResponse(request);
        //    if (TimeOut > 0)
        //    {
        //        HttpWebResponse response = Response as HttpWebResponse;
        //        if (response != null) response.GetResponseStream().ReadTimeout = TimeOut;
        //    }
        //    return Response;
        //}
        ///// <summary>
        ///// 获取HTTP回应
        ///// </summary>
        ///// <param name="request">HTTP请求</param>
        ///// <param name="result"></param>
        ///// <returns>HTTP回应</returns>
        //protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        //{
        //    Response = base.GetWebResponse(request, result);
        //    if (TimeOut > 0)
        //    {
        //        HttpWebResponse response = Response as HttpWebResponse;
        //        if (response != null) response.GetResponseStream().ReadTimeout = TimeOut;
        //    }
        //    return Response;
        //}
        /// <summary>
        /// 添加COOKIE
        /// </summary>
        /// <param name="address">URI地址</param>
        /// <param name="cookieString">COOKIE字符串</param>
        public void AddCookie(Uri address, string cookieString)
        {
            if (address != null && cookieString != null && cookieString.Length != 0)
            {
                int cookieIndex;
                foreach (subString cookie in cookieString.split(';'))
                {
                    if ((cookieIndex = cookie.IndexOf('=')) > 0)
                    {
                        try
                        {
                            Cookies.Add(address, new Cookie(web.cookie.FormatCookieName(cookie.Substring(0, cookieIndex).Trim()), web.cookie.FormatCookieValue(cookie.Substring(cookieIndex + 1).Trim()), "/"));
                        }
                        catch { }
                    }
                }
            }
        }
        /// <summary>
        /// 合并同域cookie(用于处理跨域BUG)
        /// </summary>
        /// <param name="address">URI地址</param>
        /// <param name="cookies">默认cookie集合信息</param>
        /// <param name="documentCookie">登录后的cookie信息</param>
        /// <param name="httpOnlyCookie">登录后的httpOnly相关cookie信息</param>
        public void MergeDomainCookie(Uri address, list<Cookie> cookies, string documentCookie, string httpOnlyCookie)
        {
            if (cookies != null)
            {
                foreach (Cookie cookie in cookies) Cookies.Add(address, cookie);
            }
            if (address != null)
            {
                list<Cookie> newCookies = new list<Cookie>();
                Dictionary<hashString, int> nameCounts = null;
                list<string> documentCookies = new list<string>(2);
                if (!string.IsNullOrEmpty(documentCookie)) documentCookies.UnsafeAdd(documentCookie);
                if (!string.IsNullOrEmpty(httpOnlyCookie)) documentCookies.UnsafeAdd(httpOnlyCookie);
                if (documentCookies.Count != 0)
                {
                    int index, nextCount, count;
                    string name;
                    Cookie newCookie;
                    Dictionary<hashString, int> nextNameCounts = dictionary.CreateHashString<int>();
                    nameCounts = dictionary.CreateHashString<int>();
                    foreach (string nextCookie in documentCookies)
                    {
                        nextNameCounts.Clear();
                        foreach (subString cookie in nextCookie.Split(';'))
                        {
                            if (cookie.Length != 0 && (index = cookie.IndexOf('=')) != 0)
                            {
                                if (index == -1)
                                {
                                    name = web.cookie.FormatCookieName(cookie.Trim());
                                }
                                else name = web.cookie.FormatCookieName(cookie.Substring(0, index).Trim());
                                hashString nameKey = name;
                                if (nextNameCounts.TryGetValue(nameKey, out nextCount)) nextNameCounts[nameKey] = ++nextCount;
                                else nextNameCounts.Add(nameKey, nextCount = 1);
                                if (!nameCounts.TryGetValue(nameKey, out count)) count = 0;
                                if (nextCount > count)
                                {
                                    if (index == -1) newCookie = new Cookie(name, string.Empty);
                                    else newCookie = new Cookie(name, web.cookie.FormatCookieValue(cookie.Substring(index + 1)));
                                    newCookies.Add(newCookie);
                                    if (count != 0) newCookie.HttpOnly = true;
                                    if (count == 0) nameCounts.Add(nameKey, nextCount);
                                    else nameCounts[nameKey] = nextCount;
                                }
                            }
                        }
                    }
                }
                foreach (Cookie cookie in Cookies.GetCookies(address))
                {
                    if (nameCounts != null && nameCounts.ContainsKey(cookie.Name)) cookie.Expired = true;
                }
                if (newCookies.Count != 0)
                {
                    try
                    {
                        foreach (Cookie cookie in newCookies) Cookies.Add(address, cookie);
                    }
                    catch (Exception error)
                    {
                        log.Default.Add(error, "合并同域cookie失败", true);
                    }
                }
            }
        }
        /// <summary>
        /// 过期时间
        /// </summary>
        private static readonly hashString expiresHash = "expires";
        /// <summary>
        /// 有效路径
        /// </summary>
        private static readonly hashString pathHash = "path";
        /// <summary>
        /// 作用域名
        /// </summary>
        private static readonly hashString domainHash = "domain";
        /// <summary>
        /// 合并同域cookie(用于处理跨域BUG)
        /// </summary>
        /// <param name="address">URI地址</param>
        /// <param name="responseHeaderCookie">HTTP头cookie信息</param>
        /// <param name="replaceCookie">需要替换的cookie</param>
        public void MergeDomainCookie(Uri address, string responseHeaderCookie, string replaceCookie)
        {
            if (address != null)
            {
                int index;
                string name;
                Cookie newCookie;
                CookieCollection cookies = new CookieCollection();
                Dictionary<hashString, Cookie> replaceCookies = null;
                if (responseHeaderCookie != null && responseHeaderCookie.Length != 0)
                {
                    replaceCookies = dictionary.CreateHashString<Cookie>();
                    DateTime expires;
                    string value, domain, path, expiresString;
                    string lastCookie = null;
                    list<string> newCookies = new list<string>();
                    foreach (string cookie in responseHeaderCookie.Split(','))
                    {
                        if (lastCookie == null)
                        {
                            string lowerCookie = cookie.ToLower();
                            index = lowerCookie.IndexOf("; expires=", StringComparison.Ordinal);
                            if (index == -1) index = lowerCookie.IndexOf(";expires=", StringComparison.Ordinal);
                            if (index == -1 || cookie.IndexOf(';', index + 10) != -1) newCookies.Add(cookie);
                            else lastCookie = cookie;
                        }
                        else
                        {
                            newCookies.Add(lastCookie + "," + cookie);
                            lastCookie = null;
                        }
                    }
                    Dictionary<hashString, string> cookieInfo = dictionary.CreateHashString<string>();
                    foreach (string cookie in newCookies)
                    {
                        newCookie = null;
                        foreach (subString values in cookie.Split(';'))
                        {
                            if ((index = values.IndexOf('=')) != 0)
                            {
                                if ((index = values.IndexOf('=')) == -1)
                                {
                                    name = values.Trim();
                                    value = string.Empty;
                                }
                                else
                                {
                                    name = values.Substring(0, index).Trim();
                                    value = values.Substring(index + 1);
                                }
                                if (newCookie == null) newCookie = new Cookie(web.cookie.FormatCookieName(name), web.cookie.FormatCookieValue(value));
                                else cookieInfo[name.toLower()] = value;
                            }
                        }
                        if (cookieInfo.TryGetValue(expiresHash, out expiresString)
                            && DateTime.TryParse(expiresString, out expires))
                        {
                            newCookie.Expires = expires;
                        }
                        if (cookieInfo.TryGetValue(pathHash, out path)) newCookie.Path = path;
                        replaceCookies[newCookie.Name] = newCookie;
                        newCookie = new Cookie(newCookie.Name, newCookie.Value, newCookie.Path);
                        if (cookieInfo.TryGetValue(domainHash, out domain)) newCookie.Domain = domain;
                        Cookies.Add(address, newCookie);
                        cookieInfo.Clear();
                    }
                }
                if (replaceCookie != null && replaceCookie.Length != 0)
                {
                    if (replaceCookies == null) replaceCookies = dictionary.CreateHashString<Cookie>();
                    foreach (subString cookie in replaceCookie.Split(';'))
                    {
                        if ((index = cookie.IndexOf('=')) != 0)
                        {
                            if (index == -1)
                            {
                                name = web.cookie.FormatCookieName(cookie.Trim());
                                newCookie = new Cookie(name, string.Empty);
                            }
                            else
                            {
                                name = web.cookie.FormatCookieName(cookie.Substring(0, index).Trim());
                                newCookie = new Cookie(name, web.cookie.FormatCookieValue(cookie.Substring(index + 1)));
                            }
                            hashString nameKey = name;
                            if (replaceCookies.ContainsKey(nameKey)) replaceCookies[nameKey].Value = newCookie.Value;
                            else replaceCookies.Add(nameKey, newCookie);
                        }
                    }
                }
                bool isCookie;
                foreach (Cookie cookie in Cookies.GetCookies(address))
                {
                    if (isCookie = replaceCookies != null && replaceCookies.ContainsKey(cookie.Name))
                    {
                        newCookie = replaceCookies[cookie.Name];
                    }
                    else newCookie = new Cookie(cookie.Name, cookie.Value, httpUtility.UrlDecode(cookie.Path));
                    cookies.Add(newCookie);
                    if (isCookie) replaceCookies.Remove(cookie.Name);
                    newCookie.Expires = cookie.Expires;
                    cookie.Expired = true;
                }
                if (replaceCookies != null)
                {
                    foreach (Cookie cookie in replaceCookies.Values) cookies.Add(cookie);
                }
                if (cookies.Count != 0)
                {
                    try { Cookies.Add(address, cookies); }
                    catch (Exception error)
                    {
                        log.Default.Add(error, "合并同域cookie失败", true);
                    }
                }
            }
        }
        //public void mergeDomainCookie(Uri address, Cookie[] replaceCookie)
        //{
        //    if (address != null)
        //    {
        //        Cookie newCookie;
        //        CookieCollection cookies = new CookieCollection();
        //        Dictionary<string, Cookie> replaceCookies = null;
        //        if (replaceCookie != null && replaceCookie.Length != 0)
        //        {
        //            replaceCookies = dictionary.CreateOnly<string, Cookie>();
        //            foreach (Cookie cookie in replaceCookie)
        //            {
        //                if (replaceCookies.ContainsKey(cookie.Name)) replaceCookies[cookie.Name] = cookie;
        //                else replaceCookies.Add(cookie.Name, cookie);
        //            }
        //        }
        //        bool isCookie;
        //        foreach (Cookie cookie in cookieContainer.GetCookies(address))
        //        {
        //            cookies.Add(newCookie = (isCookie = replaceCookies != null && replaceCookies.ContainsKey(cookie.Name)) ? replaceCookies[cookie.Name] : new Cookie(cookie.Name, cookie.Value, cookie.Path));
        //            if (isCookie) replaceCookies.Remove(cookie.Name);
        //            newCookie.Expires = cookie.Expires;
        //            cookie.Expired = true;
        //        }
        //        if (replaceCookies != null)
        //        {
        //            foreach (Cookie cookie in replaceCookies.Values) cookies.Add(cookie);
        //        }
        //        if (cookies.Count != 0) cookieContainer.Add(address, cookies);
        //    }
        //}
        //private void BugFix_CookieDomain(CookieContainer cookieContainer)
        //{
        //    Hashtable table = (Hashtable)typeof(CookieContainer).InvokeMember("m_domainTable",
        //                               System.Reflection.BindingFlags.NonPublic |
        //                               System.Reflection.BindingFlags.GetField |
        //                               System.Reflection.BindingFlags.Instance,
        //                               null,
        //                               cookieContainer,
        //                               new object[] { });
        //    ArrayList keys = new ArrayList(table.Keys);
        //    foreach (string keyObj in keys)
        //    {
        //        string key = (keyObj as string);
        //        if (key[0] == '.')
        //        {
        //            string newKey = key.Remove(0, 1);
        //            table[newKey] = table[keyObj];
        //        }
        //    }
        //}
        //public void addCookie(string address, CookieCollection cookies)
        //{
        //    cookieContainer.Add(new Uri(address), cookies);
        //}
        /// <summary>
        /// URI请求信息
        /// </summary>
        public struct request
        {
            /// <summary>
            /// 页面地址
            /// </summary>
            public Uri Uri;
            /// <summary>
            /// 页面地址
            /// </summary>
            public string UriString
            {
#if MONO
                set { Uri = new Uri(value); }
#else
                set { Uri = uri.Create(value); }
#endif
            }
            /// <summary>
            /// POST内容
            /// </summary>
            public NameValueCollection Form;
            /// <summary>
            /// POST内容
            /// </summary>
            public byte[] UploadData;
            /// <summary>
            /// 来源页面地址
            /// </summary>
            public string RefererUrl;
            /// <summary>
            /// 出错时是否写日志
            /// </summary>
            public bool IsErrorOut;
            /// <summary>
            /// 出错时是否输出页面地址
            /// </summary>
            public bool IsErrorOutUri;
            /// <summary>
            /// 清除请求信息
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void Clear()
            {
                Uri = null;
                Form = null;
                UploadData = null;
                RefererUrl = null;
                IsErrorOut = IsErrorOutUri = true;
            }
            /// <summary>
            /// URI隐式转换为请求信息
            /// </summary>
            /// <param name="uri">URI</param>
            /// <returns>请求信息</returns>
            public static implicit operator request(Uri uri) { return new request { Uri = uri, IsErrorOut = true, IsErrorOutUri = true }; }
            /// <summary>
            /// URI隐式转换为请求信息
            /// </summary>
            /// <param name="uri">URI</param>
            /// <returns>请求信息</returns>
            public static implicit operator request(string uri)
            {
                return new request { Uri = new Uri(uri), IsErrorOut = true, IsErrorOutUri = true };
            }
        }
        /// <summary>
        /// 将网页保存到文件
        /// </summary>
        /// <param name="request">URI请求信息</param>
        /// <param name="fileName">保存文件名</param>
        /// <returns>是否保存成功</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool SaveFile(request request, string fileName)
        {
            return SaveFile(ref request, fileName);
        }
        /// <summary>
        /// 将网页保存到文件
        /// </summary>
        /// <param name="request">URI请求信息</param>
        /// <param name="fileName">保存文件名</param>
        /// <returns>是否保存成功</returns>
        public bool SaveFile(ref request request, string fileName)
        {
            if (request.Uri != null && fileName != null)
            {
                try
                {
                    Headers.Add(header.UserAgent, UserAgent);
                    Headers.Add(header.Referer, request.RefererUrl == null || request.RefererUrl.Length == 0 ? request.Uri.AbsoluteUri : request.RefererUrl);
                    DownloadFile(request.Uri, fileName);
                    return true;
                }
                catch (Exception error)
                {
                    if (request.IsErrorOut)
                    {
                        log.Default.Add(error, (request.IsErrorOutUri ? request.Uri.AbsoluteUri : null) + " 抓取失败", !request.IsErrorOutUri);
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 抓取页面字节流
        /// </summary>
        /// <param name="request">URI请求信息</param>
        /// <returns>页面字节流,失败返回null</returns>
        public byte[] CrawlData(request request)
        {
            return CrawlData(ref request);
        }
        /// <summary>
        /// 抓取页面字节流
        /// </summary>
        /// <param name="request">URI请求信息</param>
        /// <returns>页面字节流,失败返回null</returns>
        public byte[] CrawlData(ref request request)
        {
            if (request.Uri != null)
            {
                try
                {
                    Headers.Add(header.UserAgent, UserAgent);
                    Headers.Add(header.Referer, request.RefererUrl == null || request.RefererUrl.Length == 0 ? request.Uri.AbsoluteUri : request.RefererUrl);
                    return deCompress(request.Form == null ? (request.UploadData == null ? DownloadData(request.Uri) : UploadData(request.Uri, web.http.methodType.POST.ToString(), request.UploadData)) : UploadValues(request.Uri, web.http.methodType.POST.ToString(), request.Form), ref request);
                }
                catch (Exception error)
                {
                    onError(error, ref request);
                }
            }
            return null;
        }
        /// <summary>
        /// 异步抓取页面HTML代码
        /// </summary>
        private sealed class dataCrawler
        {
            /// <summary>
            /// Web客户端
            /// </summary>
            private webClient webClient;
            /// <summary>
            /// 抓取回调事件
            /// </summary>
            private Action<byte[]> onCrawlData;
            /// <summary>
            /// URI请求信息
            /// </summary>
            private request request;
            /// <summary>
            /// 抓取回调
            /// </summary>
            private DownloadDataCompletedEventHandler onDownloadHandle;
            /// <summary>
            /// 表单事件
            /// </summary>
            private UploadValuesCompletedEventHandler onPostFormHandle;
            /// <summary>
            /// 上传事件
            /// </summary>
            private UploadDataCompletedEventHandler onUploadHandle;
            /// <summary>
            /// 异步抓取页面HTML代码
            /// </summary>
            private dataCrawler()
            {
                onDownloadHandle = onDownload;
                onPostFormHandle = onPostForm;
                onUploadHandle = onUpload;
            }
            /// <summary>
            /// 抓取页面字节流
            /// </summary>
            /// <param name="webClient">Web客户端</param>
            /// <param name="onCrawlData">抓取回调事件</param>
            /// <param name="request">URI请求信息</param>
            private void crawl(webClient webClient, Action<byte[]> onCrawlData, ref request request)
            {
                this.request = request;
                this.webClient = webClient;
                this.onCrawlData = onCrawlData;
                if (request.Uri != null)
                {
                    try
                    {
                        webClient.Headers.Add(header.UserAgent, webClient.UserAgent);
                        webClient.Headers.Add(header.Referer, request.RefererUrl == null || request.RefererUrl.Length == 0 ? request.Uri.AbsoluteUri : request.RefererUrl);
                        if (request.Form == null)
                        {
                            if (request.UploadData == null) downloadData();
                            else upload();
                        }
                        else postForm();
                        return;
                    }
                    catch (Exception error)
                    {
                        webClient.onError(error, ref request);
                    }
                }
                onCrawl(null);
            }
            /// <summary>
            /// 抓取页面字节流
            /// </summary>
            private void downloadData()
            {
                try
                {
                    webClient.DownloadDataCompleted += onDownloadHandle;
                    webClient.DownloadDataAsync(request.Uri, webClient);
                }
                catch (Exception error)
                {
                    webClient.DownloadDataCompleted -= onDownloadHandle;
                    webClient.onError(error, ref request);
                    onCrawl(null);
                }
            }
            /// <summary>
            /// 抓取页面字节流
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void onDownload(object sender, DownloadDataCompletedEventArgs e)
            {
                try
                {
                    webClient.DownloadDataCompleted -= onDownloadHandle;
                    if (e.Error != null) webClient.onError(e.Error, ref request);
                }
                finally
                {
                    onCrawl(e.Error == null ? webClient.deCompress(e.Result, ref request) : null);
                }
            }
            /// <summary>
            /// 抓取页面字节流
            /// </summary>
            private void postForm()
            {
                try
                {
                    webClient.UploadValuesCompleted += onPostFormHandle;
                    webClient.UploadValuesAsync(request.Uri, web.http.methodType.POST.ToString(), request.Form, webClient);
                }
                catch (Exception error)
                {
                    webClient.UploadValuesCompleted -= onPostFormHandle;
                    webClient.onError(error, ref request);
                    onCrawl(null);
                }
            }
            /// <summary>
            /// 抓取页面字节流
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void onPostForm(object sender, UploadValuesCompletedEventArgs e)
            {
                try
                {
                    webClient.UploadValuesCompleted -= onPostFormHandle;
                    if (e.Error != null) webClient.onError(e.Error, ref request);
                }
                finally
                {
                    onCrawl(e.Error == null ? webClient.deCompress(e.Result, ref request) : null);
                }
            }
            /// <summary>
            /// 抓取页面字节流
            /// </summary>
            private void upload()
            {
                try
                {
                    webClient.UploadDataCompleted += onUploadHandle;
                    webClient.UploadDataAsync(request.Uri, web.http.methodType.POST.ToString(), request.UploadData, webClient);
                }
                catch (Exception error)
                {
                    webClient.UploadDataCompleted -= onUploadHandle;
                    webClient.onError(error, ref request);
                    onCrawl(null);
                }
            }
            /// <summary>
            /// 抓取页面字节流
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void onUpload(object sender, UploadDataCompletedEventArgs e)
            {
                try
                {
                    webClient.UploadDataCompleted -= onUploadHandle;
                    if (e.Error != null) webClient.onError(e.Error, ref request);
                }
                finally
                {
                    onCrawl(e.Error == null ? webClient.deCompress(e.Result, ref request) : null);
                }
            }
            /// <summary>
            /// 抓取回调
            /// </summary>
            /// <param name="data">抓取数据</param>
            private void onCrawl(byte[] data)
            {
                try
                {
                    onCrawlData(data);
                }
                finally
                {
                    webClient = null;
                    onCrawlData = null;
                    request.Clear();
                    typePool<dataCrawler>.PushNotNull(this);
                }
            }
            /// <summary>
            /// 异步抓取页面HTML代码
            /// </summary>
            /// <param name="webClient">Web客户端</param>
            /// <param name="onCrawlData">抓取回调事件</param>
            /// <param name="request">URI请求信息</param>
            public static void Crawl(webClient webClient, Action<byte[]> onCrawlData, ref request request)
            {
                dataCrawler dataCrawler = typePool<dataCrawler>.Pop();
                if (dataCrawler == null)
                {
                    try
                    {
                        dataCrawler = new dataCrawler();
                    }
                    catch (Exception error)
                    {
                        log.Default.Add(error, null, false);
                        onCrawlData(null);
                        return;
                    }
                }
                dataCrawler.crawl(webClient, onCrawlData, ref request);
            }
        }
        /// <summary>
        /// 抓取页面字节流
        /// </summary>
        /// <param name="onCrawlData">异步事件</param>
        /// <param name="request">URI请求信息</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void CrawlData(Action<byte[]> onCrawlData, ref request request)
        {
            dataCrawler.Crawl(this, onCrawlData, ref request);
        }
        /// <summary>
        /// 抓取页面HTML代码
        /// </summary>
        /// <param name="request">URI请求信息</param>
        /// <param name="encoding">页面编码</param>
        /// <returns>页面HTML代码,失败返回null</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public string CrawlHtml(ref request request, Encoding encoding)
        {
            return chineseEncoder.ToString(CrawlData(ref request), encoding ?? this.TextEncoding);
        }
        /// <summary>
        /// 抓取页面HTML代码
        /// </summary>
        /// <param name="request">URI请求信息</param>
        /// <param name="encoding">页面编码</param>
        /// <returns>页面HTML代码,失败返回null</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public string CrawlHtml(request request, Encoding encoding)
        {
            return CrawlHtml(ref request, encoding);
        }
        /// <summary>
        /// 异步抓取页面HTML代码
        /// </summary>
        private struct htmlCrawler
        {
            /// <summary>
            /// Web客户端
            /// </summary>
            public webClient WebClient;
            /// <summary>
            /// 异步事件
            /// </summary>
            public Action<string> OnCrawlHtml;
            /// <summary>
            /// 页面编码
            /// </summary>
            public Encoding Encoding;
            /// <summary>
            /// 抓取页面字节流事件
            /// </summary>
            /// <param name="data">页面字节流</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void onCrawlData(byte[] data)
            {
                OnCrawlHtml(chineseEncoder.ToString(data, Encoding ?? WebClient.TextEncoding));
            }
        }
        /// <summary>
        /// 异步抓取页面HTML代码
        /// </summary>
        /// <param name="onCrawlHtml">异步事件</param>
        /// <param name="request">URI请求信息</param>
        /// <param name="encoding">页面编码</param>
        /// <returns>页面HTML代码,失败返回null</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void CrawlHtml(Action<string> onCrawlHtml, ref request request, Encoding encoding)
        {
            CrawlData(new htmlCrawler { WebClient = this, OnCrawlHtml = onCrawlHtml, Encoding = encoding }.onCrawlData, ref request);
        }
        /// <summary>
        /// 错误处理
        /// </summary>
        /// <param name="error">异常信息</param>
        /// <param name="request">请求信息</param>
        private void onError(Exception error, ref request request)
        {
            if (request.IsErrorOut)
            {
                log.Default.Add(error, (request.IsErrorOutUri ? request.Uri.AbsoluteUri : null) + " 抓取失败", !request.IsErrorOutUri);
            }
        }
        /// <summary>
        /// 数据解压缩
        /// </summary>
        /// <param name="data">原始数据</param>
        /// <param name="request">请求信息</param>
        /// <returns>解压缩数据</returns>
        private byte[] deCompress(byte[] data, ref request request)
        {
            stream compressionStream = this.compressionStream;
            if (compressionStream != null)
            {
                try
                {
                    return compressionStream.GetDeCompress(data).ToArray();
                }
                catch (Exception error)
                {
                    onError(error, ref request);
                    return null;
                }
            }
            return data;
        }
        /// <summary>
        /// 根据提交类型获取编码字符集
        /// </summary>
        /// <param name="contentType">提交类型</param>
        /// <returns>编码字符集</returns>
        private static Encoding getEncoding(string contentType)
        {
            foreach (subString value in contentType.Split(';'))
            {
                subString key = value.Trim();
                if (key.StartsWith(webClient.CharsetName))
                {
                    try
                    {
                        Encoding encoding = Encoding.GetEncoding(key.Substring(webClient.CharsetName.Length));
                        return encoding;
                    }
                    catch { }
                }
            }
            return null;
        }
        ///// <summary>
        ///// 根据HTML代码获取编码字符集
        ///// </summary>
        ///// <param name="contentType">提交类型</param>
        ///// <returns>编码字符集</returns>
        //public static Encoding GetEncoding(string html)
        //{
        //    Encoding encoding = null;
        //    if (html != null)
        //    {
        //        string[] metas = html.ToLower().Split(new string[] { "<meta " }, StringSplitOptions.None);
        //        for (int index = 1, length = metas.Length; index != length; ++index)
        //        {
        //            string meta = metas[index];
        //            int endIndex = meta.IndexOf('>');
        //            if (endIndex != -1)
        //            {
        //                int contentIndex = meta.replace('"', '\'').IndexOf("http-equiv='content-type'",  StringComparison.Ordinal);
        //                if (contentIndex != -1 && contentIndex < endIndex)
        //                {
        //                    string content = "content='";
        //                    contentIndex = meta.IndexOf(content,  StringComparison.Ordinal);
        //                    if (contentIndex != -1 && contentIndex != meta.Length)
        //                    {
        //                        endIndex = meta.IndexOf('\'', contentIndex += content.Length);
        //                        if (endIndex != -1) encoding = GetEncoding(meta.Substring(contentIndex, endIndex - contentIndex));
        //                    }
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    return encoding;
        //}
    }
}
