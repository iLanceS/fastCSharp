using System;

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// 视频转换
    /// </summary>
    public struct sendVideo
    {
        /// <summary>
        /// 需通过基础支持中的上传下载多媒体文件来得到
        /// </summary>
        public string media_id;
        /// <summary>
        /// 标题
        /// </summary>
        public string title;
        /// <summary>
        /// 描述
        /// </summary>
        public string description;
    }
}
