using System;

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// 分组
    /// </summary>
    public sealed class group
    {
        /// <summary>
        /// 分组名字，UTF8编码
        /// </summary>
        public string name;
        /// <summary>
        /// 分组id，由微信分配
        /// </summary>
        public int id;
    }
}
