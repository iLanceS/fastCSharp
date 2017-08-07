using System;

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// 图文分享转发数据
    /// </summary>
    public class userShare
    {
        /// <summary>
        /// 图文分享转发每日数据
        /// </summary>
        public sealed class hour : userShare
        {
            /// <summary>
            /// 数据的小时，包括从000到2300，分别代表的是[000,100)到[2300,2400)，即每日的第1小时和最后1小时
            /// </summary>
            public short ref_hour;
        }
        /// <summary>
        /// 数据的日期
        /// </summary>
        public string ref_date;
        /// <summary>
        /// 分享的次数
        /// </summary>
        public int share_count;
        /// <summary>
        /// 分享的人数
        /// </summary>
        public int share_user;
        /// <summary>
        /// 分享的场景 1代表好友转发 2代表朋友圈 3代表腾讯微博 255代表其他
        /// </summary>
        public byte share_scene;
    }
}
