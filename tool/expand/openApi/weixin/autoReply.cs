using System;

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// 自动回复规则
    /// </summary>
    public sealed class autoReply : isValue
    {
        /// <summary>
        /// 关注后自动回复是否开启，0代表未开启，1代表开启
        /// </summary>
        public byte is_add_friend_reply_open;
        /// <summary>
        /// 消息自动回复是否开启，0代表未开启，1代表开启
        /// </summary>
        public byte is_autoreply_open;
        /// <summary>
        /// 自动回复的类型
        /// </summary>
        public enum type : byte
        {
            /// <summary>
            /// 文本
            /// </summary>
            text,
            /// <summary>
            /// 图片
            /// </summary>
            img,
            /// <summary>
            /// 语音
            /// </summary>
            voice,
            /// <summary>
            /// 视频
            /// </summary>
            video,
            /// <summary>
            /// 关键词自动回复图文消息
            /// </summary>
            news,
        }
        /// <summary>
        /// 自动回复的信息
        /// </summary>
        public class typeContent
        {
            /// <summary>
            /// 自动回复的类型
            /// </summary>
            public type type;
            /// <summary>
            /// 对于文本类型，content是文本内容，对于图文、图片、语音、视频类型，content是mediaID
            /// </summary>
            public string content;
        }
        /// <summary>
        /// 关注后自动回复的信息
        /// </summary>
        public typeContent add_friend_autoreply_info;
        /// <summary>
        /// 消息自动回复的信息
        /// </summary>
        public typeContent message_default_autoreply_info;
        /// <summary>
        /// 匹配模式
        /// </summary>
        public enum matchMode : byte
        {
            /// <summary>
            /// 消息中含有该关键词即可
            /// </summary>
            contain,
            /// <summary>
            /// 消息内容必须和关键词严格相同
            /// </summary>
            equal
        }
        /// <summary>
        /// 匹配的关键词
        /// </summary>
        public sealed class keyword : typeContent
        {
            /// <summary>
            /// 匹配模式
            /// </summary>
            public matchMode match_mode;
        }
        /// <summary>
        /// 图文消息
        /// </summary>
        public sealed class news
        {
            /// <summary>
            /// 标题
            /// </summary>
            public string title;
            /// <summary>
            /// 作者
            /// </summary>
            public string author;
            /// <summary>
            /// 摘要
            /// </summary>
            public string digest;
            /// <summary>
            /// 封面图片的URL
            /// </summary>
            public string cover_url;
            /// <summary>
            /// 正文的URL
            /// </summary>
            public string content_url;
            /// <summary>
            /// 原文的URL，若置空则无查看原文入口
            /// </summary>
            public string source_url;
            /// <summary>
            /// 是否显示封面，0为不显示，1为显示
            /// </summary>
            public byte show_cover;
        }
        /// <summary>
        /// 图文消息列表
        /// </summary>
        public sealed class newsList
        {
            /// <summary>
            /// 图文消息列表
            /// </summary>
            public news[] list;
        }
        /// <summary>
        /// 自动回复
        /// </summary>
        public sealed class reply : typeContent
        {
            /// <summary>
            /// 图文消息的信息
            /// </summary>
            public newsList news_info;
        }
        /// <summary>
        /// 回复模式
        /// </summary>
        public enum replyMode : byte
        {
            /// <summary>
            /// 全部回复
            /// </summary>
            reply_all,
            /// <summary>
            /// 随机回复其中一条
            /// </summary>
            random_one
        }
        /// <summary>
        /// 关键词自动回复
        /// </summary>
        public sealed class keywordAutoReply
        {
            /// <summary>
            /// 创建时间
            /// </summary>
            public long create_time;
            /// <summary>
            /// 规则名称
            /// </summary>
            public string rule_name;
            /// <summary>
            /// 回复模式
            /// </summary>
            public replyMode reply_mode;
            /// <summary>
            /// 匹配的关键词列表
            /// </summary>
            public keyword[] keyword_list_info;
            /// <summary>
            /// 自动回复列表
            /// </summary>
            public reply[] reply_list_info;
        }
        /// <summary>
        /// 关键词自动回复列表
        /// </summary>
        public sealed class keywordAutoReplyList
        {
            /// <summary>
            /// 关键词自动回复列表
            /// </summary>
            public keywordAutoReply[] list;
        }
        /// <summary>
        /// 关键词自动回复的信息
        /// </summary>
        public keywordAutoReplyList keyword_autoreply_info;
    }
}
