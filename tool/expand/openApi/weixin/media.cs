using System;

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// 上传图文消息素材结果
    /// </summary>
    public sealed class media : isValue
    {
        /// <summary>
        /// 媒体文件/图文消息上传后获取的唯一标识
        /// </summary>
        public string media_id;
        /// <summary>
        /// 媒体文件上传时间
        /// </summary>
        public long created_at;
        /// <summary>
        /// 媒体文件类型
        /// </summary>
        public enum mediaType : byte
        {
            /// <summary>
            /// 图片(1M，支持JPG格式)(不超过2M，支持bmp/png/jpeg/jpg/gif格式)
            /// </summary>
            image,
            /// <summary>
            /// 语音(2M，播放长度不超过60s，支持AMR\MP3格式)(大小不超过5M，长度不超过60秒，支持mp3/wma/wav/amr格式)
            /// </summary>
            voice,
            /// <summary>
            /// 视频(10MB，支持MP4格式)
            /// </summary>
            video,
            /// <summary>
            /// 缩略图(64KB，支持JPG格式)
            /// </summary>
            thumb,
            /// <summary>
            /// 图文消息
            /// </summary>
            news
        }
        /// <summary>
        /// 媒体文件类型
        /// </summary>
        public mediaType type;
    }
}
