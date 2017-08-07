using System;
using System.Text;

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// 消息
    /// </summary>
    public abstract class messageBase
    {
        /// <summary>
        /// 文本消息
        /// </summary>
        public struct textMessage
        {
            /// <summary>
            /// 文本消息内容
            /// </summary>
            public string content;
        }
        /// <summary>
        /// 文本消息
        /// </summary>
        public textMessage text;
        /// <summary>
        /// 图片消息/语音消息
        /// </summary>
        public struct media
        {
            /// <summary>
            /// 发送的图片/语音/视频的媒体ID
            /// </summary>
            public string media_id;
        }
        /// <summary>
        /// 图片消息
        /// </summary>
        public media image;
        /// <summary>
        /// 语音消息
        /// </summary>
        public media voice;
    }
    /// <summary>
    /// 客服消息
    /// </summary>
    public sealed class message : messageBase
    {
        /// <summary>
        /// 普通用户openid
        /// </summary>
        public string touser;
        /// <summary>
        /// 消息类型
        /// </summary>
        public enum type : byte
        {
            /// <summary>
            /// 文本消息
            /// </summary>
            text,
            /// <summary>
            /// 图片消息
            /// </summary>
            image,
            /// <summary>
            /// 语音消息
            /// </summary>
            voice,
            /// <summary>
            /// 视频消息
            /// </summary>
            video,
            /// <summary>
            /// 音乐消息
            /// </summary>
            music,
            /// <summary>
            /// 图文消息 图文消息条数限制在10条以内，注意，如果图文数超过10，则将会无响应
            /// </summary>
            news,
            /// <summary>
            /// 卡券
            /// </summary>
            wxcard,
        }
        /// <summary>
        /// 消息类型
        /// </summary>
        public type msgtype;
        /// <summary>
        /// 视频消息
        /// </summary>
        public struct videoMessage
        {
            /// <summary>
            /// 发送的图片/语音/视频的媒体ID
            /// </summary>
            public string media_id;
            /// <summary>
            /// 缩略图的媒体ID
            /// </summary>
            public string thumb_media_id;
            /// <summary>
            /// 消息的标题
            /// </summary>
            public string title;
            /// <summary>
            /// 消息的描述
            /// </summary>
            public string description;
        }
        /// <summary>
        /// 视频消息
        /// </summary>
        public videoMessage video;
        /// <summary>
        /// 音乐消息
        /// </summary>
        public struct musicMessage
        {
            /// <summary>
            /// 消息的标题
            /// </summary>
            public string title;
            /// <summary>
            /// 消息的描述
            /// </summary>
            public string description;
            /// <summary>
            /// 音乐链接
            /// </summary>
            public string musicurl;
            /// <summary>
            /// 高品质音乐链接，wifi环境优先使用该链接播放音乐
            /// </summary>
            public string hqmusicurl;
            /// <summary>
            /// 缩略图的媒体ID
            /// </summary>
            public string thumb_media_id;
        }
        /// <summary>
        /// 音乐消息
        /// </summary>
        public musicMessage music;
        /// <summary>
        /// 图文消息
        /// </summary>
        public struct newsMessage
        {
            /// <summary>
            /// 图文消息
            /// </summary>
            public article[] articles;
        }
        /// <summary>
        /// 图文消息
        /// </summary>
        public struct article
        {
            /// <summary>
            /// 消息的标题
            /// </summary>
            public string title;
            /// <summary>
            /// 消息的描述
            /// </summary>
            public string description;
            /// <summary>
            /// 图文消息被点击后跳转的链接
            /// </summary>
            public string url;
            /// <summary>
            /// 图文消息的图片链接，支持JPG、PNG格式，较好的效果为大图640*320，小图80*80
            /// </summary>
            public string picurl;
        }
        /// <summary>
        /// 图文消息
        /// </summary>
        public newsMessage news;
        /// <summary>
        /// 卡券
        /// </summary>
        public struct cardMessage
        {
            /// <summary>
            /// 
            /// </summary>
            public string card_id;
            /// <summary>
            /// 卡券扩展
            /// </summary>
            public card card_ext;
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="toJsoner"></param>
            /// <param name="value"></param>
            [fastCSharp.emit.jsonSerialize.custom]
            private static unsafe void toJson(fastCSharp.emit.jsonSerializer toJsoner, cardMessage value)
            {
                if (value.card_ext.signature == null)
                {
                    log.Default.Add("卡券扩展 签名为空", new System.Diagnostics.StackFrame(), true);
                    fastCSharp.web.ajax.WriteObject(toJsoner.UnsafeCharStream);
                }
                else
                {
                    toJsoner.UnsafeWriteFirstName("card_id");
                    toJsoner.UnsafeToJson(value.card_id);
                    toJsoner.UnsafeWriteNextName("card_ext");
                    fastCSharp.emit.jsonSerializer cardToJsoner = typePool<fastCSharp.emit.jsonSerializer>.Pop() ?? new fastCSharp.emit.jsonSerializer();
                    pointer buffer = fastCSharp.unmanagedPool.StreamBuffers.Get();
                    try
                    {
                        using (charStream cardJsonStream = cardToJsoner.UnsafeCharStream)
                        {
                            cardJsonStream.UnsafeReset((byte*)buffer.Char, fastCSharp.unmanagedPool.StreamBuffers.Size);
                            cardToJsoner.UnsafeToJsonNotNull(value.card_ext, toJsoner.UnsafeConfig);
                            fastCSharp.web.ajax.UnsafeFormatJavascript(cardJsonStream, toJsoner.UnsafeCharStream);
                        }
                    }
                    finally
                    {
                        fastCSharp.unmanagedPool.StreamBuffers.Push(ref buffer);
                        toJsoner.UnsafeFree();
                    }
                    toJsoner.UnsafeCharStream.Write('}');
                }
            }
        }
        /// <summary>
        /// 卡券扩展
        /// </summary>
        public struct card
        {
            /// <summary>
            /// 指定的卡券code码，只能被领一次。use_custom_code字段为true的卡券必须填写，非自定义code不必填写。
            /// </summary>
            public string code;
            /// <summary>
            /// 指定领取者的openid，只有该用户能领取。bind_openid字段为true的卡券必须填写，bind_openid字段为false不必填写。
            /// </summary>
            public string openid;
            /// <summary>
            /// 时间戳，商户生成从1970年1月1日00:00:00至今的秒数,即当前的时间,且最终需要转换为字符串形式;由商户生成后传入,不同添加请求的时间戳须动态生成，若重复将会导致领取失败！
            /// </summary>
            public string timestamp;
            /// <summary>
            /// 随机字符串，由开发者设置传入，加强签名的安全性。随机字符串，不长于32位。推荐使用大小写字母和数字，不同添加请求的nonce须动态生成，若重复将会导致领取失败！
            /// </summary>
            public string nonce_str;
            /// <summary>
            /// 签名，商户将接口列表中的参数按照指定方式进行签名,签名方式使用SHA1,具体签名方案参见下文;由商户按照规范签名后传入。
            /// </summary>
            public string signature;
            /// <summary>
            /// 卡券扩展http://mp.weixin.qq.com/wiki/7/aaa137b55fb2e0456bf8dd9148dd613f.html#.E9.99.84.E5.BD.954-.E5.8D.A1.E5.88.B8.E6.89.A9.E5.B1.95.E5.AD.97.E6.AE.B5.E5.8F.8A.E7.AD.BE.E5.90.8D.E7.94.9F.E6.88.90.E7.AE.97.E6.B3.95
            /// </summary>
            /// <param name="api_ticket"></param>
            /// <param name="card_id"></param>
            /// <param name="code"></param>
            /// <param name="openid"></param>
            /// <param name="timestamp"></param>
            /// <param name="nonce_str"></param>
            public card(string api_ticket, string card_id, string code, string openid, string timestamp, string nonce_str)
            {
                signature = getSignature(new string[] { api_ticket, card_id, this.code = code, this.openid = openid, this.timestamp = timestamp, this.nonce_str = nonce_str });
            }
            /// <summary>
            /// 卡券扩展
            /// </summary>
            /// <param name="api_ticket"></param>
            /// <param name="card_id"></param>
            public void SetSignature(string api_ticket, string card_id)
            {
                signature = getSignature(new string[] { api_ticket, card_id, code, openid, timestamp, nonce_str });
            }
            /// <summary>
            /// 获取签名
            /// </summary>
            /// <param name="values"></param>
            /// <returns></returns>
            private static string getSignature(string[] values)
            {
                Array.Sort(values, stringExpand.OrdinalComparisonHandle);
                return fastCSharp.pub.Sha1(fastCSharp.unsafer.String.ConcatBytes(values)).toLowerHex();
            }
        }
        /// <summary>
        /// 卡券
        /// </summary>
        public cardMessage wxcard;
        /// <summary>
        /// 指定客服帐号
        /// </summary>
        public struct account
        {
            /// <summary>
            /// 
            /// </summary>
            public string kf_account;
        }
        /// <summary>
        /// 以某个客服帐号来发消息
        /// </summary>
        public account customservice;
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="toJsoner"></param>
        /// <param name="value"></param>
        [fastCSharp.emit.jsonSerialize.custom]
        private static void toJson(fastCSharp.emit.jsonSerializer toJsoner, message value)
        {
            toJsoner.UnsafeWriteFirstName("touser");
            toJsoner.UnsafeToJson(value.touser);
            toJsoner.UnsafeWriteNextName("msgtype");
            toJsoner.UnsafeToJsonNotNull(value.msgtype.ToString());
            toJsoner.UnsafeWriteNextName(value.msgtype.ToString());
            switch (value.msgtype)
            {
                case type.news:
                    toJsoner.UnsafeToJsonNotNull(value.news);
                    break;
                case type.text:
                    toJsoner.UnsafeToJsonNotNull(value.text);
                    break;
                case type.image:
                    toJsoner.UnsafeToJsonNotNull(value.image);
                    break;
                case type.voice:
                    toJsoner.UnsafeToJsonNotNull(value.voice);
                    break;
                case type.video:
                    toJsoner.UnsafeToJsonNotNull(value.video);
                    break;
                case type.music:
                    toJsoner.UnsafeToJsonNotNull(value.music);
                    break;
                case type.wxcard:
                    toJsoner.UnsafeToJsonNotNull(value.wxcard);
                    break;
            }
            if (value.customservice.kf_account != null)
            {
                toJsoner.UnsafeWriteNextName("customservice");
                toJsoner.UnsafeToJsonNotNull(value.customservice);
            }
            toJsoner.UnsafeCharStream.Write('}');
        }
    }
}
