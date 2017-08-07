using Android.Content;

namespace fastCSharp.weixin.android
{
    /// <summary>
    /// 广播接收(不是必须的)
    /// </summary>
    public abstract class appRegister : BroadcastReceiver
    {
        /// <summary>
        /// 微信通用广播
        /// </summary>
        public const string ActionRefreshWxApp = "com.tencent.mm.plugin.openapi.Intent.ACTION_REFRESH_WXAPP";
        ///// <summary>
        ///// 微信支付成功广播
        ///// </summary>
        //public const string ActionMessageWxPaySuccess = "com.tencent.mm.plugin.openapi.Intent.ACTION_MESSAGE_WXPAY_SUCCESS";

        /// <summary>
        /// API 包装
        /// </summary>
        private api api;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="intent"></param>
        public override void OnReceive(Context context, Intent intent)
        {
            
            switch (intent.Action)
            {
                case ActionRefreshWxApp:
                    api = createApi(context, intent);
                    actionRefreshWxApp();
                    return;
                //case ActionMessageWxPaySuccess:
                //    api = createApi(context, intent);
                //    actionMessageWxPaySuccess();
                //    return;
            }
        }
        /// <summary>
        /// 创建 API 包装
        /// </summary>
        /// <param name="context"></param>
        /// <param name="intent"></param>
        /// <returns></returns>
        protected abstract api createApi(Context context, Intent intent);
        /// <summary>
        /// 微信通用广播
        /// </summary>
        protected virtual void actionRefreshWxApp() { }
        ///// <summary>
        ///// 微信支付成功广播
        ///// </summary>
        //protected virtual void actionMessageWxPaySuccess() { }

        /// <summary>
        /// 默认广播接收注册
        /// </summary>
        public static readonly IntentFilter DefaultIntentFilter;
        /// <summary>
        /// 默认注册广播
        /// </summary>
        /// <param name="context"></param>
        /// <param name="appRegister">广播接收</param>
        public static void Register(Context context, appRegister appRegister)
        {
            context.RegisterReceiver(appRegister, DefaultIntentFilter);
        }
        static appRegister()
        {
            DefaultIntentFilter = new IntentFilter();
            DefaultIntentFilter.AddAction(ActionRefreshWxApp);
            //DefaultIntentFilter.AddAction(ActionMessageWxPaySuccess);
        }
    }
}