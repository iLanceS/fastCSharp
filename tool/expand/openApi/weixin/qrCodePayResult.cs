using System;
using System.Security.Cryptography;
#pragma warning disable

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// 扫码支付回调返回值
    /// </summary>
    [fastCSharp.emit.xmlSerialize(Filter = fastCSharp.code.memberFilters.InstanceField, IsAllMember = true)]
    public sealed class qrCodePayResult : resultCode
    {
        /// <summary>
        /// 公众账号ID[必填]
        /// </summary>
        public string appid;
        /// <summary>
        /// 商户号[必填]
        /// </summary>
        public string mch_id;
        /// <summary>
        /// 微信返回的随机字符串[必填]
        /// </summary>
        public string nonce_str;
        /// <summary>
        /// 调用统一下单接口生成的预支付ID[必填]
        /// </summary>
        public string prepay_id;
        /// <summary>
        /// 返回数据签名[必填]
        /// </summary>
        internal string sign;
        /// <summary>
        /// 设置应用配置
        /// </summary>
        /// <param name="config">应用配置</param>
        public void SetConfig(config config = null)
        {
            if (config == null) config = fastCSharp.openApi.weixin.config.Default;
            appid = config.appid;
            mch_id = config.appid;
            if (result_code != code.FAIL) err_code_des = null;
            sign<qrCodePayResult>.Set(this, config.key);
        }
    }
}
