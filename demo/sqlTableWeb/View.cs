using System;

namespace fastCSharp.demo.sqlTableWeb
{
    /// <summary>
    /// 公共全局视图
    /// </summary>
    /// <typeparam name="viewType">视图类型</typeparam>
    abstract class View<viewType> : fastCSharp.code.cSharp.webView.view<viewType>
         where viewType : View<viewType>
    {
        /// <summary>
        /// 公共路径
        /// </summary>
        internal fastCSharp.demo.sqlModel.path.Pub PubPath = new fastCSharp.demo.sqlModel.path.Pub();
        /// <summary>
        /// 全局false
        /// </summary>
        [fastCSharp.code.ignore]
        internal bool FalseFlag;
    }
}
