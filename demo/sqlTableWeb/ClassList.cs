using System;

namespace fastCSharp.demo.sqlTableWeb
{
    /// <summary>
    /// 班级列表
    /// </summary>
    [fastCSharp.code.cSharp.webView(IsPool = true)]
    partial class ClassList : View<ClassList>
    {
        /// <summary>
        /// 是否班级列表页面
        /// </summary>
        bool IsClassList = true;
        /// <summary>
        /// 班级信息集合
        /// </summary>
        sqlTableCacheServer.Class[] Classes
        {
            get
            {
                return fastCSharp.demo.sqlTableCacheServer.clientCache.Class.Get(fastCSharp.demo.sqlTableCacheServer.tcpCall.Class.getIds());
            }
        }
    }
}
