using System;

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// 一级菜单数组，个数应为1~3个
    /// </summary>
    public struct menu
    {
        /// <summary>
        /// 菜单的响应动作类型
        /// </summary>
        public enum menuType : byte
        {
            /// <summary>
            /// 未知菜单的响应动作类型
            /// </summary>
            none = 0,
            /// <summary>
            /// 点击推事件，微信服务器会通过消息接口推送消息类型为event	的结构给开发者（参考消息接口指南），并且带上按钮中开发者填写的key值，开发者可以通过自定义的key值与用户进行交互
            /// </summary>
            click = 1,
            /// <summary>
            /// 跳转URL，微信客户端将会打开开发者在按钮中填写的网页URL，可与网页授权获取用户基本信息接口结合，获得用户基本信息
            /// </summary>
            view = 2,
            /// <summary>
            /// 扫码推事件，微信客户端将调起扫一扫工具，完成扫码操作后显示扫描结果（如果是URL，将进入URL），且会将扫码的结果传给开发者，开发者可以下发消息
            /// </summary>
            scancode_push = 3,
            /// <summary>
            /// 扫码推事件且弹出“消息接收中”提示框，微信客户端将调起扫一扫工具，完成扫码操作后，将扫码的结果传给开发者，同时收起扫一扫工具，然后弹出“消息接收中”提示框，随后可能会收到开发者下发的消息
            /// </summary>
            scancode_waitmsg = 4,
            /// <summary>
            /// 弹出系统拍照发图，微信客户端将调起系统相机，完成拍照操作后，会将拍摄的相片发送给开发者，并推送事件给开发者，同时收起系统相机，随后可能会收到开发者下发的消息
            /// </summary>
            pic_sysphoto = 5,
            /// <summary>
            /// 弹出拍照或者相册发图，微信客户端将弹出选择器供用户选择“拍照”或者“从手机相册选择”。用户选择后即走其他两种流程
            /// </summary>
            pic_photo_or_album = 6,
            /// <summary>
            /// 弹出微信相册发图器，微信客户端将调起微信相册，完成选择操作后，将选择的相片发送给开发者的服务器，并推送事件给开发者，同时收起相册，随后可能会收到开发者下发的消息
            /// </summary>
            pic_weixin = 7,
            /// <summary>
            /// 弹出地理位置选择器，微信客户端将调起地理位置选择工具，完成选择操作后，将选择的地理位置发送给开发者的服务器，同时收起位置选择工具，随后可能会收到开发者下发的消息
            /// </summary>
            location_select = 8,
            /// <summary>
            /// 下发消息（除文本消息），微信服务器会将开发者填写的永久素材id对应的素材下发给用户，永久素材类型可以是图片、音频、视频、图文消息。请注意：永久素材id必须是在“素材管理/新增永久素材”接口上传后获得的合法id
            /// </summary>
            media_id = 9,
            /// <summary>
            /// 跳转图文消息URL，微信客户端将打开开发者在按钮中填写的永久素材id对应的图文消息URL，永久素材类型只支持图文消息。请注意：永久素材id必须是在“素材管理/新增永久素材”接口上传后获得的合法id
            /// </summary>
            view_limited = 10,
            /// <summary>
            /// 自定义菜单配置
            /// </summary>
            news,
        }
        /// <summary>
        /// 菜单标题，不超过16个字节
        /// </summary>
        public string name;
        /// <summary>
        /// 菜单KEY值，用于消息接口推送，不超过128字节(click等点击类型必须)
        /// </summary>
        public string key;
        /// <summary>
        /// 网页链接，用户点击菜单可打开链接，不超过256字节(view类型必须)
        /// </summary>
        public string url;
        /// <summary>
        /// 调用新增永久素材接口返回的合法media_id(media_id类型和view_limited类型必须)
        /// </summary>
        public string media_id;
        /// <summary>
        /// 对于不同的菜单类型，value的值意义不同
        /// 官网上设置的自定义菜单：Text:保存文字到value； Img、voice：保存mediaID到value； Video：保存视频下载链接到value； News：保存图文消息到news_info，同时保存mediaID到value； View：保存链接到url。
        /// 使用API设置的自定义菜单：click、scancode_push、scancode_waitmsg、pic_sysphoto、pic_photo_or_album、	pic_weixin、location_select：保存值到key；view：保存链接到url
        /// </summary>
        public string value;
        /// <summary>
        /// 二级菜单数组，个数应为1~5个
        /// </summary>
        public struct subMenu
        {
            /// <summary>
            /// 菜单标题，子菜单不超过40个字节
            /// </summary>
            public string name;
            /// <summary>
            /// 菜单KEY值，用于消息接口推送，不超过128字节(click等点击类型必须)
            /// </summary>
            public string key;
            /// <summary>
            /// 网页链接，用户点击菜单可打开链接，不超过256字节(view类型必须)
            /// </summary>
            public string url;
            /// <summary>
            /// 调用新增永久素材接口返回的合法media_id(media_id类型和view_limited类型必须)
            /// </summary>
            public string media_id;
            /// <summary>
            /// 对于不同的菜单类型，value的值意义不同
            /// 官网上设置的自定义菜单：Text:保存文字到value； Img、voice：保存mediaID到value； Video：保存视频下载链接到value； News：保存图文消息到news_info，同时保存mediaID到value； View：保存链接到url。
            /// 使用API设置的自定义菜单：click、scancode_push、scancode_waitmsg、pic_sysphoto、pic_photo_or_album、	pic_weixin、location_select：保存值到key；view：保存链接到url
            /// </summary>
            public string value;
            /// <summary>
            /// 图文消息列表
            /// </summary>
            public autoReply.newsList news_info;
            /// <summary>
            /// 菜单的响应动作类型
            /// </summary>
            public menuType type;
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="toJsoner"></param>
            /// <param name="value"></param>
            [fastCSharp.emit.jsonSerialize.custom]
            private static void toJson(fastCSharp.emit.jsonSerializer toJsoner, subMenu value)
            {
                toJsoner.UnsafeWriteFirstName("name");
                toJsoner.UnsafeToJson(value.name);
                if (value.type != menuType.none)
                {
                    toJsoner.UnsafeWriteNextName("type");
                    toJsoner.UnsafeToJsonNotNull(value.type);
                }
                if (!string.IsNullOrEmpty(value.key))
                {
                    toJsoner.UnsafeWriteNextName("key");
                    toJsoner.UnsafeToJsonNotNull(value.key);
                }
                if (!string.IsNullOrEmpty(value.url))
                {
                    toJsoner.UnsafeWriteNextName("url");
                    toJsoner.UnsafeToJsonNotNull(value.url);
                }
                if (!string.IsNullOrEmpty(value.media_id))
                {
                    toJsoner.UnsafeWriteNextName("media_id");
                    toJsoner.UnsafeToJsonNotNull(value.media_id);
                }
                toJsoner.UnsafeCharStream.Write('}');
            }
        }
        /// <summary>
        /// 二级菜单数组，个数应为1~5个
        /// </summary>
        public subMenu[] sub_button;
        /// <summary>
        /// 菜单的响应动作类型
        /// </summary>
        public menuType type;
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="toJsoner"></param>
        /// <param name="value"></param>
        [fastCSharp.emit.jsonSerialize.custom]
        private static void toJson(fastCSharp.emit.jsonSerializer toJsoner, menu value)
        {
            toJsoner.UnsafeWriteFirstName("name");
            toJsoner.UnsafeToJson(value.name);
            if (value.type != menuType.none)
            {
                toJsoner.UnsafeWriteNextName("type");
                toJsoner.UnsafeToJsonNotNull(value.type);
            }
            if (!string.IsNullOrEmpty(value.key))
            {
                toJsoner.UnsafeWriteNextName("key");
                toJsoner.UnsafeToJsonNotNull(value.key);
            }
            if (!string.IsNullOrEmpty(value.url))
            {
                toJsoner.UnsafeWriteNextName("url");
                toJsoner.UnsafeToJsonNotNull(value.url);
            }
            if (!string.IsNullOrEmpty(value.media_id))
            {
                toJsoner.UnsafeWriteNextName("media_id");
                toJsoner.UnsafeToJsonNotNull(value.media_id);
            }
            if (value.sub_button.length() != 0)
            {
                toJsoner.UnsafeWriteNextName("sub_button");
                toJsoner.UnsafeToJsonNotNull(value.sub_button);
            }
            toJsoner.UnsafeCharStream.Write('}');
        }
    }
}
