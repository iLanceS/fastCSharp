using System;
using System.Collections.Generic;
using Android.App;
using Com.Alipay.Sdk.App;

namespace fastCSharp.alipay.android
{
    /// <summary>
    /// API
    /// </summary>
    public static class api
    {
        /// <summary>
        /// 同步调用支付
        /// </summary>
        /// <param name="activity">UI 上下文</param>
        /// <param name="getOrderString">获取订单字符串</param>
        /// <param name="checkOrderResult">验证订单结果</param>
        /// <param name="isShowPayLoading">用户在商户app内部点击付款，是否需要一个loading做为在钱包唤起之前的过渡，这个值设置为true，将会在调用pay接口的时候直接唤起一个loading，直到唤起H5支付页面或者唤起外部的钱包付款页面loading才消失。（建议将该值设置为true，优化点击付款到支付唤起支付页面的过渡过程。）</param>
        /// <returns>订单结果，null 表示失败</returns>
        public static string Pay(Activity activity, Func<fastCSharp.net.returnValue<string>> getOrderString, Func<string, fastCSharp.net.returnValue<bool>> checkOrderResult, bool isShowPayLoading = true)
        {
            if (activity != null && getOrderString != null && checkOrderResult != null)
            {
                try
                {
                    string orderInfo = getOrderString();
                    if (orderInfo != null)
                    {
                        using (PayTask alipay = new PayTask(activity))
                        {
                            IDictionary<string, string> payResult = alipay.PayV2(orderInfo, isShowPayLoading);
                            if (payResult["resultStatus"] == "9000")
                            {
                                string result = payResult["result"];
                                if (checkOrderResult(result)) return result;
                            }
                        }
                    }
                }
                catch (Exception error)
                {
                    fastCSharp.log.Default.Add(error, null, false);
                }
            }
            return null;
        }
        /// <summary>
        /// 异步支付
        /// </summary>
        public sealed class pay
        {
            /// <summary>
            /// UI 上下文
            /// </summary>
            private Activity activity;
            /// <summary>
            /// 回调
            /// </summary>
            private Action<string> callback;
            /// <summary>
            /// 验证订单结果
            /// </summary>
            private Func<string, fastCSharp.net.returnValue<bool>> checkOrderResult;
            /// <summary>
            /// 用户在商户app内部点击付款，是否需要一个loading做为在钱包唤起之前的过渡，这个值设置为true，将会在调用pay接口的时候直接唤起一个loading，直到唤起H5支付页面或者唤起外部的钱包付款页面loading才消失。（建议将该值设置为true，优化点击付款到支付唤起支付页面的过渡过程。）
            /// </summary>
            private bool isShowPayLoading;
            /// <summary>
            /// 异步支付
            /// </summary>
            /// <param name="activity">UI 上下文</param>
            /// <param name="checkOrderResult">验证订单结果</param>
            /// <param name="callback">回调</param>
            /// <param name="isShowPayLoading">用户在商户app内部点击付款，是否需要一个loading做为在钱包唤起之前的过渡，这个值设置为true，将会在调用pay接口的时候直接唤起一个loading，直到唤起H5支付页面或者唤起外部的钱包付款页面loading才消失。（建议将该值设置为true，优化点击付款到支付唤起支付页面的过渡过程。）</param>
            public pay(Activity activity, Func<string, fastCSharp.net.returnValue<bool>> checkOrderResult, Action<string> callback, bool isShowPayLoading)
            {
                this.activity = activity;
                this.checkOrderResult = checkOrderResult;
                this.callback = callback;
                this.isShowPayLoading = isShowPayLoading;
            }
            /// <summary>
            /// 获取订单字符串后发送支付请求
            /// </summary>
            /// <param name="order"></param>
            public void Send(fastCSharp.net.returnValue<string> order)
            {
                bool isCheck = false;
                if (order.IsReturn && order.Value != null)
                {
                    try
                    {
                        using (PayTask alipay = new PayTask(activity))
                        {
                            IDictionary<string, string> payResult = alipay.PayV2(order.Value, isShowPayLoading);
                            if (payResult["resultStatus"] == "9000")
                            {
                                string result = payResult["result"];
                                if (checkOrderResult(result))
                                {
                                    isCheck = true;
                                    if (callback != null) callback(result);
                                    return;
                                }
                            }
                        }
                    }
                    catch (System.Exception error)
                    {
                        fastCSharp.log.Default.Add(error, null, false);
                    }
                }
                if (!isCheck && callback != null) callback(null);
            }
        }
        /// <summary>
        /// 异步调用支付
        /// </summary>
        /// <param name="activity">UI 上下文</param>
        /// <param name="getOrderString">获取订单字符串</param>
        /// <param name="checkOrderResult">验证订单结果</param>
        /// <param name="callback">回调</param>
        /// <param name="isShowPayLoading">用户在商户app内部点击付款，是否需要一个loading做为在钱包唤起之前的过渡，这个值设置为true，将会在调用pay接口的时候直接唤起一个loading，直到唤起H5支付页面或者唤起外部的钱包付款页面loading才消失。（建议将该值设置为true，优化点击付款到支付唤起支付页面的过渡过程。）</param>
        public static void Pay(Activity activity, Action<Action<fastCSharp.net.returnValue<string>>> getOrderString, Func<string, fastCSharp.net.returnValue<bool>> checkOrderResult, Action<string> callback, bool isShowPayLoading = true)
        {
            if (activity != null && getOrderString != null && checkOrderResult != null)
            {
                try
                {
                    getOrderString(new pay(activity, checkOrderResult, callback, isShowPayLoading).Send);
                    return;
                }
                catch (System.Exception error)
                {
                    fastCSharp.log.Default.Add(error, null, false);
                }
            }
            if (callback != null) callback(null);
        }
    }
}