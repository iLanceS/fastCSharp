using System;
using fastCSharp.code.cSharp;

namespace fastCSharp.demo.sqlTableWeb
{
    /// <summary>
    /// 网站生成配置
    /// </summary>
    sealed class webConfig : fastCSharp.code.webConfig
    {
        /// <summary>
        /// 默认主域名
        /// </summary>
        public override string MainDomain
        {
            get { return "127.0.0.1:8101"; }
        }
        /// <summary>
        /// WEB Path 导出引导类型
        /// </summary>
        public override Type ExportPathType
        {
            get { return typeof(fastCSharp.demo.sqlModel.path.Class); }
        }
        /// <summary>
        /// web视图404重定向
        /// </summary>
        internal static readonly webView.errorPath NotFound404 = new webView.errorPath { ErrorPath = new fastCSharp.demo.sqlModel.path.Pub().NotFound404 };
    }
}
