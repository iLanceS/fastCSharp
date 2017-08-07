using System;

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// 预览消息
    /// </summary>
    public sealed class previewMessage : messageBase
    {
        /// <summary>
        /// 微信号,优先于touser
        /// </summary>
        public string towxname;
        /// <summary>
        /// 普通用户openid
        /// </summary>
        public string touser;
        /// <summary>
        /// 消息类型
        /// </summary>
        public bulkMessageBase.type msgtype;
        /// <summary>
        /// 图文消息
        /// </summary>
        public media mpnews;
        /// <summary>
        /// 视频
        /// </summary>
        public media mpvideo;
        /// <summary>
        /// 卡券
        /// </summary>
        public message.cardMessage wxcard;
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="toJsoner"></param>
        /// <param name="value"></param>
        [fastCSharp.emit.jsonSerialize.custom]
        private static void toJson(fastCSharp.emit.jsonSerializer toJsoner, previewMessage value)
        {
            if (string.IsNullOrEmpty(value.towxname))
            {
                toJsoner.UnsafeWriteFirstName("touser");
                toJsoner.UnsafeToJson(value.touser);
            }
            else
            {
                toJsoner.UnsafeWriteFirstName("towxname");
                toJsoner.UnsafeToJson(value.towxname);
            }
            toJsoner.UnsafeWriteNextName("msgtype");
            toJsoner.UnsafeToJsonNotNull(value.msgtype.ToString());
            toJsoner.UnsafeWriteNextName(value.msgtype.ToString());
            switch (value.msgtype)
            {
                case bulkMessageBase.type.text:
                    toJsoner.UnsafeToJsonNotNull(value.text);
                    break;
                case bulkMessageBase.type.image:
                    toJsoner.UnsafeToJsonNotNull(value.image);
                    break;
                case bulkMessageBase.type.voice:
                    toJsoner.UnsafeToJsonNotNull(value.voice);
                    break;
                case bulkMessageBase.type.mpvideo:
                    toJsoner.UnsafeToJsonNotNull(value.mpvideo);
                    break;
                case bulkMessageBase.type.mpnews:
                    toJsoner.UnsafeToJsonNotNull(value.mpnews);
                    break;
                case bulkMessageBase.type.wxcard:
                    toJsoner.UnsafeToJsonNotNull(value.wxcard);
                    break;
            }
            toJsoner.UnsafeCharStream.Write('}');
        }
    }
}
