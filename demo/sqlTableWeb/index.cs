using System;

namespace fastCSharp.demo.sqlTableWeb
{
    /// <summary>
    /// 公共基本调用
    /// </summary>
    [fastCSharp.code.cSharp.webCall(IsPool = true)]
    class index : fastCSharp.code.cSharp.webCall.call<index>
    {
        /// <summary>
        /// 首页重定向
        /// </summary>
        [fastCSharp.code.cSharp.webCall(FullName = "")]
        public void Load()
        {
            location(new fastCSharp.demo.sqlModel.path.Pub().ClassList);
        }
    }
}
