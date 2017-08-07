using System;

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// 分组用户数量
    /// </summary>
    public struct groupCount
    {
        /// <summary>
        /// 分组名字，UTF8编码
        /// </summary>
        public string name;
        /// <summary>
        /// 分组id，由微信分配
        /// </summary>
        public int id;
        /// <summary>
        /// 分组内用户数量
        /// </summary>
        public int count;
    }
}
