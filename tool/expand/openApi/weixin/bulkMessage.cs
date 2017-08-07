using System;

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// 群发消息
    /// </summary>
    public abstract class bulkMessageBase : messageBase
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public enum type : byte
        {
            /// <summary>
            /// 图文消息
            /// </summary>
            mpnews,
            /// <summary>
            /// 文本
            /// </summary>
            text,
            /// <summary>
            /// 语音（注意此处media_id需通过基础支持中的上传下载多媒体文件来得到）
            /// </summary>
            voice,
            /// <summary>
            /// 图片（注意此处media_id需通过基础支持中的上传下载多媒体文件来得到）
            /// </summary>
            image,
            /// <summary>
            /// 视频
            /// </summary>
            mpvideo,
            /// <summary>
            /// 卡券消息
            /// </summary>
            wxcard
        }
        /// <summary>
        /// 消息类型
        /// </summary>
        public type msgtype;
        /// <summary>
        /// 图文消息
        /// </summary>
        public media mpnews;
        /// <summary>
        /// 视频
        /// </summary>
        public media mpvideo;
        /// <summary>
        /// 卡券消息
        /// </summary>
        public struct cardMessage
        {
            /// <summary>
            /// 
            /// </summary>
            public string card_id;
        }
        /// <summary>
        /// 卡券消息
        /// </summary>
        public cardMessage wxcard;
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="toJsoner"></param>
        protected void toJson(fastCSharp.emit.jsonSerializer toJsoner)
        {
            toJsoner.UnsafeWriteNextName("msgtype");
            toJsoner.UnsafeToJsonNotNull(msgtype.ToString());
            toJsoner.UnsafeWriteNextName(msgtype.ToString());
            switch (msgtype)
            {
                case type.mpnews:
                    toJsoner.UnsafeToJsonNotNull(mpnews);
                    break;
                case type.text:
                    toJsoner.UnsafeToJsonNotNull(text);
                    break;
                case type.image:
                    toJsoner.UnsafeToJsonNotNull(image);
                    break;
                case type.voice:
                    toJsoner.UnsafeToJsonNotNull(voice);
                    break;
                case type.mpvideo:
                    toJsoner.UnsafeToJsonNotNull(mpvideo);
                    break;
                case type.wxcard:
                    toJsoner.UnsafeToJsonNotNull(wxcard);
                    break;
            }
            toJsoner.UnsafeCharStream.Write('}');
        }
    }
    /// <summary>
    /// 群发消息
    /// </summary>
    public sealed class bulkMessage : bulkMessageBase
    {
        /// <summary>
        /// 用于设定图文消息的接收者
        /// </summary>
        public struct messageFilter
        {
            /// <summary>
            /// 群发到的分组的group_id，参加用户管理中用户分组接口，若is_to_all值为true，可不填写group_id
            /// </summary>
            public string group_id;
            /// <summary>
            /// 用于设定是否向全部用户发送，值为true或false，选择true该消息群发给所有用户，选择false可根据group_id发送给指定群组的用户
            /// </summary>
            public bool is_to_all;
        }
        /// <summary>
        /// 用于设定图文消息的接收者
        /// </summary>
        public messageFilter filter;
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="toJsoner"></param>
        /// <param name="value"></param>
        [fastCSharp.emit.jsonSerialize.custom]
        private static void toJson(fastCSharp.emit.jsonSerializer toJsoner, bulkMessage value)
        {
            toJsoner.UnsafeWriteFirstName("filter");
            toJsoner.UnsafeToJson(value.filter);
            value.toJson(toJsoner);
        }
    }
}
