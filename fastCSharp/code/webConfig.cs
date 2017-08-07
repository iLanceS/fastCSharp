using System;
using fastCSharp.code.cSharp;
using System.Text;

namespace fastCSharp.code
{
    /// <summary>
    /// 网站生成配置
    /// </summary>
    public abstract class webConfig
    {
        /// <summary>
        /// Session类型
        /// </summary>
        public virtual Type SessionType
        {
            get { return typeof(int); }
        }
        /// <summary>
        /// 文件编码
        /// </summary>
        public virtual Encoding Encoding
        {
            get { return fastCSharp.config.appSetting.Encoding; }
        }
        /// <summary>
        /// 默认主域名
        /// </summary>
        public abstract string MainDomain { get; }
        /// <summary>
        /// 静态文件网站域名
        /// </summary>
        public virtual string StaticFileDomain { get { return MainDomain; } }
        /// <summary>
        /// 图片文件域名
        /// </summary>
        public virtual string ImageDomain { get { return MainDomain; } }
        /// <summary>
        /// 轮询网站域名
        /// </summary>
        public virtual string PollDomain { get { return MainDomain; } }
        /// <summary>
        /// 视图加载失败重定向
        /// </summary>
        public virtual string NoViewLocation { get { return null; } }
        /// <summary>
        /// WEB视图扩展默认目录名称
        /// </summary>
        public virtual string ViewJsDirectory
        {
            get { return "viewJs"; }
        }
        /// <summary>
        /// 附加导入 HTML/JS 目录集合
        /// </summary>
        public virtual string[] IncludeDirectories { get { return null; } }
        /// <summary>
        /// 客户端缓存标识版本
        /// </summary>
        public virtual int ETagVersion
        {
            get { return 0; }
        }
#if MONO
#else
        /// <summary>
        /// 是否忽略大小写
        /// </summary>
        public virtual bool IgnoreCase
        {
            get { return false; }
        }
#endif
        /// <summary>
        /// 文件缓存是否预留HTTP头部空间
        /// </summary>
        public virtual bool IsFileCacheHeader
        {
            get { return true; }
        }
        /// <summary>
        /// 是否进行WebView前期处理（格式化HTML、CSS）
        /// </summary>
        public virtual bool IsWebView
        {
            get { return true; }
        }
        /// <summary>
        /// 是否复制js脚本文件
        /// </summary>
        public virtual bool IsCopyScript
        {
            get { return true; }
        }
        /// <summary>
        /// 是否对 css 缺省域名替换为静态文件域名
        /// </summary>
        public virtual bool IsCssStaticFileDomain
        {
            get { return true; }
        }
        /// <summary>
        /// WEB Path 导出引导类型
        /// </summary>
        public virtual Type ExportPathType
        {
            get { return null; }
        }
        /// <summary>
        /// 默认为 true 表示导出 TypeScript，否则导出 JavaScript
        /// </summary>
        public virtual bool IsExportPathTypeScript
        {
            get { return true; }
        }
    }
}
