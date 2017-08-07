using System;
using Android.Content;
using Android.Graphics;
using Com.Tencent.MM.Sdk.Openapi;
using Com.Tencent.MM.Sdk.Modelmsg;
using Com.Tencent.MM.Sdk.Modelpay;

namespace fastCSharp.weixin.android
{
    /// <summary>
    /// API 包装
    /// </summary>
    public class api
    {
        /// <summary>
        /// API
        /// </summary>
        public IWXAPI Api { get; private set; }
        /// <summary>
        /// API 是否可用
        /// </summary>
        public bool IsApi { get; private set; }
        /// <summary>
        /// API 包装
        /// </summary>
        /// <param name="context"></param>
        /// <param name="appId"></param>
        /// <param name="isRegisterApp">是否自动注册</param>
        public api(Context context, string appId, bool isRegisterApp = true)
        {
            if (isRegisterApp)
            {
                Api = WXAPIFactory.CreateWXAPI(context, null);
                IsApi = Api.IsWXAppInstalled && Api.IsWXAppSupportAPI;
                if (!IsApi || !Api.RegisterApp(appId))
                {
                    Api = null;
                    IsApi = false;
                    fastCSharp.log.Default.Add("微信 API 注册失败", new System.Diagnostics.StackFrame(), false);
                }
            }
            else
            {
                Api = WXAPIFactory.CreateWXAPI(context, appId);
                IsApi = Api.IsWXAppInstalled && Api.IsWXAppSupportAPI;
            } 
        }

        /// <summary>
        /// 分享场景
        /// </summary>
        public enum shareScene
        {
            /// <summary>
            /// 朋友圈
            /// </summary>
            WXSceneSession = 0,
            /// <summary>
            /// 时间线
            /// </summary>
            WXSceneTimeline = 1,
            /// <summary>
            /// 收藏
            /// </summary>
            WXSceneFavorite = 2
        }
        /// <summary>
        /// 分享
        /// </summary>
        /// <param name="type">分享类型</param>
        /// <param name="msg">分享信息</param>
        /// <param name="scene">分享场景</param>
        /// <returns>是否发送成功，不等于分享成功</returns>
        private bool share(string type, WXMediaMessage msg, shareScene scene)
        {
            SendMessageToWX.Req req = new SendMessageToWX.Req();
            req.Transaction = type;
            req.Message = msg;
            req.Scene = (int)scene;
            return Api.SendReq(req);
        }
        /// <summary>
        /// 分享文本
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="description">描述</param>
        /// <param name="scene">分享场景</param>
        /// <returns>是否发送成功，不等于分享成功</returns>
        public bool Share(string text, string description = null, shareScene scene = shareScene.WXSceneSession)
        {
            if (IsApi)
            {
                WXMediaMessage msg = new WXMediaMessage(new WXTextObject(text));
                msg.Description = description ?? text;
                return share("text", msg, scene);
            }
            return false;
        }
        /// <summary>
        /// 分享图片
        /// </summary>
        /// <param name="bmp">图片</param>
        /// <param name="thumbnail">缩略图</param>
        /// <param name="thumbnailNeedRecycle">缩略图是否需要回收</param>
        /// <param name="scene">分享场景</param>
        /// <returns>是否发送成功，不等于分享成功</returns>
        public bool ShareImage(Bitmap bmp, Bitmap thumbnail, bool thumbnailNeedRecycle = true, shareScene scene = shareScene.WXSceneSession)
        {
            if (IsApi)
            {
                WXMediaMessage msg = new WXMediaMessage(new WXImageObject(bmp));
                msg.ThumbData = bmpToByteArray(thumbnail, thumbnailNeedRecycle);
                share("img", msg, scene);
            }
            return false;
        }
        /// <summary>
        /// 缩略图转 JPEG 字节数组
        /// </summary>
        /// <param name="bmp">图片</param>
        /// <param name="needRecycle">是否需要回收</param>
        /// <returns>字节数组</returns>
        private static byte[] bmpToByteArray(Bitmap bmp, bool needRecycle)
        {
            using (System.IO.MemoryStream output = new System.IO.MemoryStream())
            {
                bmp.Compress(Bitmap.CompressFormat.Jpeg, 100, output);
                if (needRecycle) bmp.Recycle();
                return output.ToArray();
            }
        }
        /// <summary>
        /// 创建缩略图
        /// </summary>
        /// <param name="bmp">图片</param>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        /// <param name="needRecycle">原图是否需要回收</param>
        /// <returns>缩略图</returns>
        public static Bitmap CreateThumbnail(Bitmap bmp, int width, int height, bool needRecycle = true)
        {
            Bitmap thumbnail = Bitmap.CreateScaledBitmap(bmp, width, height, true);
            if (needRecycle) bmp.Recycle();
            return thumbnail;
        }
        /// <summary>
        /// 分享多媒体
        /// </summary>
        /// <param name="type">分享类型</param>
        /// <param name="mediaObject">媒体信息</param>
        /// <param name="title">标题</param>
        /// <param name="description">描述</param>
        /// <param name="thumbnail">缩略图</param>
        /// <param name="thumbnailNeedRecycle">缩略图是否需要回收</param>
        /// <param name="scene">分享场景</param>
        /// <returns>是否发送成功，不等于分享成功</returns>
        private bool share(string type, WXMediaMessage.IMediaObject mediaObject, string title, string description, Bitmap thumbnail, bool thumbnailNeedRecycle, shareScene scene)
        {
            WXMediaMessage msg = new WXMediaMessage(mediaObject);
            msg.Title = title;
            msg.Description = description ?? string.Empty;
            if (thumbnail != null) msg.ThumbData = bmpToByteArray(thumbnail, thumbnailNeedRecycle);
            return share(type, msg, scene);
        }
        /// <summary>
        /// 分享音乐
        /// </summary>
        /// <param name="url">音乐链接地址</param>
        /// <param name="title">标题</param>
        /// <param name="description">描述</param>
        /// <param name="thumbnail">缩略图</param>
        /// <param name="thumbnailNeedRecycle">缩略图是否需要回收</param>
        /// <param name="scene">分享场景</param>
        /// <returns>是否发送成功，不等于分享成功</returns>
        public bool ShareMusic(string url, string title, string description = null, Bitmap thumbnail = null, bool thumbnailNeedRecycle = true, shareScene scene = shareScene.WXSceneSession)
        {
            if (IsApi)
            {
                WXMusicObject music = new WXMusicObject();
                music.MusicUrl = url;
                return share("music", music, title, description, thumbnail, thumbnailNeedRecycle, scene);
            }
            return false;
        }
        /// <summary>
        /// 分享视频
        /// </summary>
        /// <param name="url">视频链接地址</param>
        /// <param name="title">标题</param>
        /// <param name="description">描述</param>
        /// <param name="thumbnail">缩略图</param>
        /// <param name="thumbnailNeedRecycle">缩略图是否需要回收</param>
        /// <param name="scene">分享场景</param>
        /// <returns>是否发送成功，不等于分享成功</returns>
        public bool ShareVideo(string url, string title, string description = null, Bitmap thumbnail = null, bool thumbnailNeedRecycle = true, shareScene scene = shareScene.WXSceneSession)
        {
            if (IsApi)
            {
                WXVideoObject video = new WXVideoObject();
                video.VideoUrl = url;
                return share("video", video, title, description, thumbnail, thumbnailNeedRecycle, scene);
            }
            return false;
        }
        /// <summary>
        /// 分享网页
        /// </summary>
        /// <param name="url">网页 URI</param>
        /// <param name="title">标题</param>
        /// <param name="description">描述</param>
        /// <param name="thumbnail">缩略图</param>
        /// <param name="thumbnailNeedRecycle">缩略图是否需要回收</param>
        /// <param name="scene">分享场景</param>
        /// <returns>是否发送成功，不等于分享成功</returns>
        public bool ShareWeb(string url, string title, string description = null, Bitmap thumbnail = null, bool thumbnailNeedRecycle = true, shareScene scene = shareScene.WXSceneSession)
        {
            if (IsApi)
            {
                WXWebpageObject web = new WXWebpageObject(url);
                return share("webpage", web, title, description, thumbnail, thumbnailNeedRecycle, scene);
            }
            return false;
        }
        /// <summary>
        /// 发送支付请求
        /// </summary>
        /// <param name="order">交易会话信息</param>
        /// <returns>是否发送成功，不等于支付成功</returns>
        public bool SendPayReq(fastCSharp.openApi.weixin.api.appPrePayIdOrder order)
        {
            return order != null && SendPayReq(new PayReq
            {
                AppId = order.appid,
                PartnerId = order.partnerid,
                PrepayId = order.prepayid,
                NonceStr = order.noncestr,
                TimeStamp = order.timestamp,
                PackageValue = order.package,
                Sign = order.sign
            });
        }
        /// <summary>
        /// 发送支付请求
        /// </summary>
        /// <param name="req">支付请求信息</param>
        /// <returns>是否发送成功，不等于支付成功</returns>
        public bool SendPayReq(PayReq req)
        {
            req.SignType = "MD5";
            return IsApi && Api.SendReq(req);
        }
        /// <summary>
        /// 同步调用支付
        /// </summary>
        /// <param name="getOrder">获取交易会话信息</param>
        /// <returns>是否发送成功，不等于支付成功</returns>
        public bool Pay(Func<fastCSharp.net.returnValue<fastCSharp.openApi.weixin.api.appPrePayIdOrder>> getOrder)
        {
            if (getOrder != null && IsApi)
            {
                try
                {
                    fastCSharp.net.returnValue<fastCSharp.openApi.weixin.api.appPrePayIdOrder> order = getOrder();
                    if (order.IsReturn && SendPayReq(order)) return true;
                }
                catch (System.Exception error)
                {
                    fastCSharp.log.Default.Add(error, null, false);
                }
            }
            return false;
        }
        /// <summary>
        /// 异步支付
        /// </summary>
        protected sealed class pay
        {
            /// <summary>
            /// API 包装
            /// </summary>
            public api Api;
            /// <summary>
            /// 回调
            /// </summary>
            public Action<bool> Callback;
            /// <summary>
            /// 获取交易会话信息后发送支付请求
            /// </summary>
            /// <param name="order"></param>
            public void Send(fastCSharp.net.returnValue<fastCSharp.openApi.weixin.api.appPrePayIdOrder> order)
            {
                bool isSend = false;
                if (order.IsReturn && order.Value != null)
                {
                    try
                    {
                        if (Api.SendPayReq(order))
                        {
                            Callback(isSend = true);
                            return;
                        }
                    }
                    catch (System.Exception error)
                    {
                        fastCSharp.log.Default.Add(error, null, false);
                    }
                }
                if (!isSend && Callback != null) Callback(false);
            }
        }
        /// <summary>
        /// 异步调用支付
        /// </summary>
        /// <param name="getOrder">获取交易会话信息</param>
        /// <param name="callback">回调</param>
        public void Pay(Action<Action<fastCSharp.net.returnValue<fastCSharp.openApi.weixin.api.appPrePayIdOrder>>> getOrder, Action<bool> callback)
        {
            if (getOrder != null && !IsApi)
            {
                try
                {
                    getOrder(new pay { Api = this, Callback = callback }.Send);
                    return;
                }
                catch (System.Exception error)
                {
                    fastCSharp.log.Default.Add(error, null, false);
                }
            }
            if (callback != null) callback(false);
        }
    }
}