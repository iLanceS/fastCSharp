using System;

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// 统一下单参数 https://pay.weixin.qq.com/wiki/doc/api/app/app.php?chapter=9_1
    /// </summary>
    [fastCSharp.emit.xmlSerialize(Filter = fastCSharp.code.memberFilters.InstanceField, IsAllMember = true)]
    public sealed class appPrePayIdQuery : prePayIdQueryBase
    {
        /// <summary>
        /// 签名类型，目前支持HMAC-SHA256和MD5，默认为MD5
        /// </summary>
        internal readonly string sign_type = "MD5";
        /// <summary>
        /// 交易类型[必填]
        /// </summary>
        internal readonly tradeType trade_type = tradeType.APP;

        /// <summary>
        /// 设置应用配置
        /// </summary>
        /// <param name="config">应用配置</param>
        internal void SetConfig(config config)
        {
            setConfig(config);
            sign<appPrePayIdQuery>.Set(this, config.key);
        }
    }
}
