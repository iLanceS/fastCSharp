using System;

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// 永久素材列表
    /// </summary>
    public sealed class mediaList : isValue
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
        /// 永久素材
        /// </summary>
        public struct mediaItem
        {
            /// <summary>
            /// 最后更新时间
            /// </summary>
            public long update_time;
            /// <summary>
            /// 
            /// </summary>
            public string media_id;
            /// <summary>
            /// 文件名称
            /// </summary>
            public string name;
            /// <summary>
            /// 图片的URL
            /// </summary>
            public string url;
        }
        /// <summary>
        /// 永久素材列表
        /// </summary>
        public mediaItem[] item;
    }
}
