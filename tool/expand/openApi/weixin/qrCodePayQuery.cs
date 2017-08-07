using System;

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// 扫码支付回调请求参数
    /// </summary>
    public sealed class qrCodePayQuery : signQuery
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public string product_id;
        /// <summary>
        /// 用户标识
        /// </summary>
        public string openid;
        /// <summary>
        /// 是否关注公众账号
        /// </summary>
        public string is_subscribe;
        /// <summary>
        /// 微信支付回调验证
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool Verify(config config = null)
        {
            if (config == null) config = config.Default;
            if (appid == config.appid && mch_id == config.mch_id && sign<qrCodePayQuery>.Check(this, config.key, sign)) return true;
            config.PayLog.Add("微信支付回调验证错误 " + this.ToJson(), new System.Diagnostics.StackFrame(), false);
            return false;
        }
        /// <summary>
        /// 获取扫码支付回调返回值
        /// </summary>
        /// <param name="prepay_id"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public qrCodePayResult GetResult(string prepay_id, config config = null)
        {
            qrCodePayResult value = new qrCodePayResult { return_code = returnCode.code.SUCCESS, prepay_id = prepay_id, appid = appid, mch_id = mch_id, nonce_str = nonce_str, result_code = returnCode.code.SUCCESS };
            sign<qrCodePayResult>.Set(value, (config ?? config.Default).key);
            return value;
        }
        /// <summary>
        /// 获取扫码支付回调返回值
        /// </summary>
        /// <param name="err_code_des">错误代码描述</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public qrCodePayResult GetErrorResult(string err_code_des, config config = null)
        {
            config.PayLog.Add(err_code_des, new System.Diagnostics.StackFrame(), false);
            qrCodePayResult value = new qrCodePayResult { return_code = returnCode.code.SUCCESS, err_code_des = err_code_des, appid = appid, mch_id = mch_id, nonce_str = nonce_str };
            sign<qrCodePayResult>.Set(value, (config ?? config.Default).key);
            return value;
        }
    }
}
