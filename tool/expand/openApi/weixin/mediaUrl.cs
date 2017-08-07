using System;

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// 永久素材
    /// </summary>
    public sealed class mediaUrl : isValue
    {
        /// <summary>
        /// 新增的永久素材的media_id
        /// </summary>
        public string media_id;
        /// <summary>
        /// 新增的图片素材的图片URL（仅新增图片素材时会返回该字段）
        /// </summary>
        public string url;
    }
}
