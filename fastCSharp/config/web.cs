using System;
using System.Reflection;
using System.IO;
using fastCSharp.code.cSharp;

namespace fastCSharp.config
{
    /// <summary>
    /// 网站模块相关参数
    /// </summary>
    public sealed class web
    {
        /// <summary>
        /// AJAX回调函数名称
        /// </summary>
        public const char AjaxCallBackName = 'c';
        /// <summary>
        /// 查询编码名称
        /// </summary>
        public const char EncodingName = 'e';
        /// <summary>
        /// json查询对象名称
        /// </summary>
        public const char QueryJsonName = 'j';
        /// <summary>
        /// 第一次加载页面缓存名称
        /// </summary>
        public const char LoadPageCache = 'l';
        /// <summary>
        /// 重新加载视图查询名称（忽略主列表）
        /// </summary>
        public const char MobileReViewName = 'm';
        /// <summary>
        ///AJAX调用函数名称
        /// </summary>
        public const char AjaxCallName = 'n';
        /// <summary>
        /// 重新加载视图查询名称
        /// </summary>
        public const char ReViewName = 'v';
        /// <summary>
        /// XML查询对象名称
        /// </summary>
        public const char QueryXmlName = 'x';

        /// <summary>
        /// web视图重新加载禁用输出成员名称
        /// </summary>
        public const string ViewOnlyName = "ViewOnly";
        /// <summary>
        /// fastCSharp Typescipt文件路径
        /// </summary>
        private string fastCSharpTypesciptPath;
        /// <summary>
        /// fastCSharp Typescipt文件路径
        /// </summary>
        public string FastCSharpTypesciptPath
        {
            get
            {
                if (fastCSharpTypesciptPath != null && !Directory.Exists(fastCSharpTypesciptPath)) fastCSharpTypesciptPath = null;
                if (fastCSharpTypesciptPath == null)
                {
                    try
                    {
                        string tsPath = new DirectoryInfo(fastCSharp.pub.ApplicationPath).Parent.Parent.fullName() + @"js\";
                        if (Directory.Exists(tsPath)) fastCSharpTypesciptPath = new DirectoryInfo(tsPath).fullName().fileNameToLower();
                    }
                    catch (Exception error)
                    {
                        log.Default.Add(error, "没有找到Typescipt文件路径", false);
                    }
                }
                return fastCSharpTypesciptPath;
            }
        }
        /// <summary>
        /// AJAX调用名称
        /// </summary>
        private string ajaxWebCallName = "/ajax";
        /// <summary>
        /// AJAX调用名称
        /// </summary>
        public string AjaxWebCallName
        {
            get { return ajaxWebCallName; }
        }
        /// <summary>
        /// AJAX是否判定Referer来源
        /// </summary>
        private bool isAjaxReferer = true;
        /// <summary>
        /// AJAX是否判定Referer来源
        /// </summary>
        public bool IsAjaxReferer
        {
            get { return isAjaxReferer; }
        }
        /// <summary>
        /// web视图默认查询参数名称
        /// </summary>
        private string viewQueryName = "query";
        /// <summary>
        /// web视图默认查询参数名称
        /// </summary>
        public string ViewQueryName
        {
            get { return viewQueryName; }
        }
        ///// <summary>
        ///// Json转换时间差
        ///// </summary>
        //private DateTime parseJavascriptMinTime = new DateTime(1970, 1, 1, 0, 0, 0);
        ///// <summary>
        ///// Json转换时间差
        ///// </summary>
        //public DateTime ParseJavascriptMinTime
        //{
        //    get { return parseJavascriptMinTime; }
        //}
        /// <summary>
        /// 客户端缓存时间(单位:秒)
        /// </summary>
        private int clientCacheSeconds = 0;
        /// <summary>
        /// 客户端缓存时间(单位:秒)
        /// </summary>
        public int ClientCacheSeconds
        {
            get { return clientCacheSeconds < 0 ? 0 : clientCacheSeconds; }
        }
        /// <summary>
        /// 最大文件缓存字节数(单位KB)
        /// </summary>
        private int maxCacheFileSize = 1 << 8;
        /// <summary>
        /// 最大文件缓存字节数(单位KB)
        /// </summary>
        public int MaxCacheFileSize
        {
            get { return maxCacheFileSize < 0 ? 0 : maxCacheFileSize; }
        }
        /// <summary>
        /// 最大缓存字节数(单位MB)
        /// </summary>
        private int maxCacheSize = 1 << 10;
        /// <summary>
        /// 最大缓存字节数(单位MB)
        /// </summary>
        public int MaxCacheSize
        {
            get { return maxCacheSize < 0 ? 0 : maxCacheSize; }
        }
        /// <summary>
        /// HTTP服务路径
        /// </summary>
        private string httpServerPath;
        /// <summary>
        /// HTTP服务路径
        /// </summary>
        public string HttpServerPath
        {
            get
            {
                if (httpServerPath == null) httpServerPath = pub.Default.WorkPath;
                return httpServerPath;
            }
        }
        /// <summary>
        /// 公用错误缓存最大字节数
        /// </summary>
        private int pubErrorMaxSize = 1 << 10;
        /// <summary>
        /// 公用错误缓存最大字节数
        /// </summary>
        public int PubErrorMaxSize
        {
            get { return pubErrorMaxSize; }
        }
        /// <summary>
        /// 公用错误处理最大缓存数量
        /// </summary>
        private int pubErrorMaxCacheCount = 1 << 10;
        /// <summary>
        /// 公用错误处理最大缓存数量
        /// </summary>
        public int PubErrorMaxCacheCount
        {
            get { return pubErrorMaxCacheCount; }
        }
        /// <summary>
        /// 网站模块相关参数
        /// </summary>
        private web()
        {
            pub.LoadConfig(this);
        }
        /// <summary>
        /// 默认网站模块相关参数
        /// </summary>
        public static readonly web Default = new web();
    }
}
