using System;

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// 素材总数
    /// </summary>
    public sealed class mediaCount : isValue
    {
        /// <summary>
        /// 语音总数量
        /// </summary>
        public int voice_count;
        /// <summary>
        /// 视频总数量
        /// </summary>
        public int video_count;
        /// <summary>
        /// 图片总数量
        /// </summary>
        public int image_count;
        /// <summary>
        /// 图文总数量
        /// </summary>
        public int news_count;
    }
}
