using System;

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// 累计用户数据
    /// </summary>
    public struct userCumulate
    {
        /// <summary>
        /// 数据的日期
        /// </summary>
        public string ref_date;
        /// <summary>
        /// 总用户量
        /// </summary>
        public long cumulate_user;
    }
}
