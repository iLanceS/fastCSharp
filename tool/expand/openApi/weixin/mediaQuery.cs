using System;

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// 获取素材列表查询
    /// </summary>
    public struct mediaQuery
    {
        /// <summary>
        /// 素材的类型
        /// </summary>
        public enum mediaType : byte
        {
            /// <summary>
            /// 图文
            /// </summary>
            news,
            /// <summary>
            /// 图片
            /// </summary>
            image,
            /// <summary>
            /// 视频
            /// </summary>
            video,
            /// <summary>
            /// 语音
            /// </summary>
            voice
        }
        /// <summary>
        /// 素材的类型
        /// </summary>
        public mediaType type;
        /// <summary>
        /// 从全部素材的该偏移位置开始返回，0表示从第一个素材 返回
        /// </summary>
        public int offset;
        /// <summary>
        /// 返回素材的数量，取值在1到20之间
        /// </summary>
        public int count;
    }
}
