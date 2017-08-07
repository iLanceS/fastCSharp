using System;

namespace fastCSharp.demo.sqlTableWeb
{
    /// <summary>
    /// 班级页面
    /// </summary>
    [fastCSharp.code.cSharp.webView(IsPool = true)]
    partial class Class : View<Class>
    {
        /// <summary>
        /// 班级信息
        /// </summary>
        [fastCSharp.emit.webView.clearMember]
        sqlTableCacheServer.Class ClassInfo;
        /// <summary>
        /// 班级页面
        /// </summary>
        /// <param name="ClassId">班级标识</param>
        /// <returns>是否成功</returns>
        private bool loadView(int ClassId)
        {
            ClassInfo = fastCSharp.demo.sqlTableCacheServer.clientCache.Class[ClassId];
            if (ClassInfo != null) return true;
            AjaxResponse(webConfig.NotFound404);
            return false;
        }
    }
}
