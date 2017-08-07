using System;

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// 图文消息素材列表
    /// </summary>
    public sealed class newsList : isValue
    {
        /// <summary>
        /// 素材的总数
        /// </summary>
        public int total_count;
        /// <summary>
        /// 本次调用获取的素材的数量
        /// </summary>
        public int item_count;
        /// <summary>
        /// 图文消息素材
        /// </summary>
        public struct newsItem
        {
            /// <summary>
            /// 图文消息素材
            /// </summary>
            public articleUrl[] news_item;
        }
        /// <summary>
        /// 图文消息素材列表
        /// </summary>
        public struct items
        {
            /// <summary>
            /// 这篇图文消息素材的最后更新时间
            /// </summary>
            public long update_time;
            /// <summary>
            /// 
            /// </summary>
            public string media_id;
            /// <summary>
            /// 图文消息素材
            /// </summary>
            public newsItem content;
        }
        /// <summary>
        /// 图文消息素材列表
        /// </summary>
        public items[] item;
    }
}
