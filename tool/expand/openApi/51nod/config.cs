using System;
using System.Text;
using System.Security.Cryptography;

namespace fastCSharp.openApi._51nod
{
    /// <summary>
    /// 应用配置
    /// </summary>
    public sealed class config
    {
        /// <summary>
        /// 编码绑定请求
        /// </summary>
        internal static readonly encodingRequest Request = new encodingRequest(openApi.request.Default, Encoding.UTF8);
#pragma warning disable
        /// <summary>
        /// 调用域名地址
        /// </summary>
        internal string Domain = "http://www.51nod.com/";
        /// <summary>
        /// 注册邮箱
        /// </summary>
        private string email;
        /// <summary>
        /// 用户密码
        /// </summary>
        private string password;
        /// <summary>
        /// 随机前缀
        /// </summary>
        private string randomPrefix;
        /// <summary>
        /// 令牌超时
        /// </summary>
        private int timeoutSeconds = 60 * 60;
        /// <summary>
        /// 获取令牌超时
        /// </summary>
        private int getTokenTimeoutSeconds;
#pragma warning restore
        /// <summary>
        /// 令牌超时
        /// </summary>
        private long timeoutTicks;
        /// <summary>
        /// 应用配置
        /// </summary>
        public config() { }
        /// <summary>
        /// 测试应用配置
        /// </summary>
        /// <param name="email">注册邮箱</param>
        /// <param name="password">用户密码</param>
        /// <param name="domain">调用域名地址</param>
        public config(string email, string password, string domain = "http://www.51nod.com/")
        {
            this.email = email;
            this.password = password;
            Domain = domain;
        }
        /// <summary>
        /// 获取令牌参数
        /// </summary>
        private struct getParameter
        {
            /// <summary>
            /// 注册邮箱
            /// </summary>
            public string email;
            /// <summary>
            /// 用户密码
            /// </summary>
            public string password;
            /// <summary>
            /// 随机前缀
            /// </summary>
            public string randomPrefix;
            /// <summary>
            /// 令牌超时
            /// </summary>
            public int timeoutSeconds;
            /// <summary>
            /// 获取令牌参数
            /// </summary>
            /// <param name="email">注册邮箱</param>
            /// <param name="password">用户密码</param>
            /// <param name="randomPrefix">随机前缀</param>
            /// <param name="timeoutSeconds">令牌超时</param>
            internal getParameter(string email, string password, string randomPrefix, int timeoutSeconds)
            {
                this.email = email;
                this.randomPrefix = randomPrefix ?? random.Default.NextULong().toHex16();
                this.timeoutSeconds = timeoutSeconds;
                using (MD5 md5 = new MD5CryptoServiceProvider())
                {
                    this.password = md5.ComputeHash(Encoding.Unicode.GetBytes(this.randomPrefix + md5.ComputeHash(Encoding.Unicode.GetBytes(password)).toLowerHex())).toLowerHex();
                }
            }
        }
        /// <summary>
        /// 获取访问令牌
        /// </summary>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        internal string GetToken(out long timeout)
        {
            if (timeoutTicks == 0)
            {
                if (timeoutSeconds <= 0) timeoutSeconds = 60;
                timeoutTicks = (long)timeoutSeconds * date.SecondTicks;
                getTokenTimeoutSeconds = timeoutSeconds + Math.Min(timeoutSeconds, 10 * 60);
                if (getTokenTimeoutSeconds < 0) getTokenTimeoutSeconds = int.MaxValue;
            }
            timeout = (date.UtcNowSecond.Ticks / timeoutTicks) * timeoutTicks + timeoutTicks;
            returnValue<string> value = Request.RequestJson<returnValue<string>, getParameter>(Domain + "ajax?n=pub.GetToken", new getParameter(email, password, randomPrefix, getTokenTimeoutSeconds));
            return value == null ? null : value.Value;
        }

        /// <summary>
        /// 默认配置
        /// </summary>
        public static readonly config Default = fastCSharp.config.pub.LoadConfig<config>(new config());
    }
}
