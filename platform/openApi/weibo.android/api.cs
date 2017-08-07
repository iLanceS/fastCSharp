using System.Threading;
using Android.App;
using Android.Graphics;
using Com.Sina.Weibo.Sdk.Api;
using Com.Sina.Weibo.Sdk.Api.Share;

namespace fastCSharp.weibo.android
{
    /// <summary>
    /// API 包装
    /// </summary>
    public class api
    {
        /// <summary>
        /// UI 上下文
        /// </summary>
        private Activity activity;
        /// <summary>
        /// 请求前缀
        /// </summary>
        private string transactionPrefix;
        /// <summary>
        /// 当前请求标识
        /// </summary>
        private int identity;
        /// <summary>
        /// API
        /// </summary>
        public IWeiboShareAPI Api { get; private set; }
        /// <summary>
        /// API 是否可用
        /// </summary>
        public bool IsApi { get; private set; }
        /// <summary>
        /// API 包装
        /// </summary>
        /// <param name="activity">UI 上下文</param>
        /// <param name="appKey"></param>
        /// <param name="isRegisterApp">是否自动注册</param>
        public api(Activity activity, string appKey, bool isRegisterApp = true)
        {
            this.activity = activity;
            transactionPrefix = ((ulong)date.Now.Ticks).toHex16();
            Api = WeiboShareSDK.CreateWeiboAPI(activity, appKey);
            IsApi = Api.IsWeiboAppInstalled && Api.IsWeiboAppSupportAPI;
            if (isRegisterApp)
            {
                if (!IsApi || !Api.RegisterApp())
                {
                    Api = null;
                    IsApi = false;
                    fastCSharp.log.Default.Add("微博 API 注册失败", new System.Diagnostics.StackFrame(), false);
                }
            }
        }
        /// <summary>
        /// 分享信息
        /// </summary>
        /// <param name="mediaObject">分享信息</param>
        /// <returns>是否发送成功，不等于分享成功</returns>
        public bool Share(BaseMediaObject mediaObject)
        {
            WeiboMessage weiboMessage = new WeiboMessage();
            weiboMessage.MediaObject = mediaObject;
            SendMessageToWeiboRequest request = new SendMessageToWeiboRequest();
            request.Transaction = transactionPrefix + ((uint)Interlocked.Increment(ref identity)).toHex8();
            request.Message = weiboMessage;
            return Api.SendRequest(activity, request);
        }
        /// <summary>
        /// 设置附加信息
        /// </summary>
        /// <param name="mediaObject">分享信息</param>
        /// <param name="url">点击链接 URI</param>
        /// <param name="title">标题</param>
        /// <param name="description">描述</param>
        private static void set(BaseMediaObject mediaObject, string url, string title, string description)
        {
            if (url != null) mediaObject.ActionUrl = url;
            if (title != null) mediaObject.Title = title;
            if (description != null) mediaObject.Description = description;
        }
        /// <summary>
        /// 分享网页
        /// </summary>
        /// <param name="url">点击链接 URI</param>
        /// <param name="title">标题</param>
        /// <param name="description">描述</param>
        /// <returns>是否发送成功，不等于分享成功</returns>
        public bool ShareWebpage(string url, string title = null, string description = null)
        {
            WebpageObject webpageObject = new WebpageObject();
            set(webpageObject, url, title, description);
            return Share(webpageObject);
        }
        /// <summary>
        /// 分享文本
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="url">点击链接 URI</param>
        /// <param name="title">标题</param>
        /// <param name="description">描述</param>
        /// <returns>是否发送成功，不等于分享成功</returns>
        public bool ShareText(string text, string url = null, string title = null, string description = null)
        {
            TextObject textObject = new TextObject();
            textObject.Text = text;
            set(textObject, url, title, description);
            return Share(textObject);
        }
        /// <summary>
        /// 分享图片
        /// </summary>
        /// <param name="bmp">图片</param>
        /// <param name="url">点击链接 URI</param>
        /// <param name="title">标题</param>
        /// <param name="description">描述</param>
        /// <returns>是否发送成功，不等于分享成功</returns>
        public bool ShareImage(Bitmap bmp, string url = null, string title = null, string description = null)
        {
            ImageObject imageObject = new ImageObject();
            imageObject.SetImageObject(bmp);
            set(imageObject, url, title, description);
            return Share(imageObject);
        }
        /// <summary>
        /// 分享图片
        /// </summary>
        /// <param name="imageUrl">图片 URI</param>
        /// <param name="url">点击链接 URI</param>
        /// <param name="title">标题</param>
        /// <param name="description">描述</param>
        /// <returns>是否发送成功，不等于分享成功</returns>
        public bool ShareImage(string imageUrl, string url = null, string title = null, string description = null)
        {
            ImageObject imageObject = new ImageObject();
            imageObject.ImagePath = imageUrl;
            set(imageObject, url, title, description);
            return Share(imageObject);
        }
        /// <summary>
        /// 分享音乐
        /// </summary>
        /// <param name="musicObject">音乐信息</param>
        /// <param name="thumbnail">缩略图</param>
        /// <param name="url">点击链接 URI</param>
        /// <param name="title">标题</param>
        /// <param name="description">描述</param>
        /// <returns>是否发送成功，不等于分享成功</returns>
        public bool ShareMusic(MusicObject musicObject, Bitmap thumbnail = null, string url = null, string title = null, string description = null)
        {
            if (thumbnail != null) musicObject.SetThumbImage(thumbnail);
            set(musicObject, url, title, description);
            return Share(musicObject);
        }
        /// <summary>
        /// 分享语音
        /// </summary>
        /// <param name="voiceObject">语音信息</param>
        /// <param name="thumbnail">缩略图</param>
        /// <param name="url">点击链接 URI</param>
        /// <param name="title">标题</param>
        /// <param name="description">描述</param>
        /// <returns>是否发送成功，不等于分享成功</returns>
        public bool ShareVoice(VoiceObject voiceObject, Bitmap thumbnail = null, string url = null, string title = null, string description = null)
        {
            if (thumbnail != null) voiceObject.SetThumbImage(thumbnail);
            set(voiceObject, url, title, description);
            return Share(voiceObject);
        }
        /// <summary>
        /// 分享视频
        /// </summary>
        /// <param name="videoObject">视频信息</param>
        /// <param name="thumbnail">缩略图</param>
        /// <param name="url">点击链接 URI</param>
        /// <param name="title">标题</param>
        /// <param name="description">描述</param>
        /// <returns>是否发送成功，不等于分享成功</returns>
        public bool ShareVideo(VideoObject videoObject, Bitmap thumbnail = null, string url = null, string title = null, string description = null)
        {
            if (thumbnail != null) videoObject.SetThumbImage(thumbnail);
            set(videoObject, url, title, description);
            return Share(videoObject);
        }
    }
}