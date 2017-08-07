using System;

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// 订单查询信息
    /// </summary>
    [fastCSharp.emit.xmlSerialize(Filter = fastCSharp.code.memberFilters.InstanceField, IsAllMember = true)]
    public sealed class orderQuery : signQuery
    {
        /// <summary>
        /// 微信的订单号，优先使用
        /// </summary>
        public string transaction_id;
        /// <summary>
        /// 商户系统内部的订单号，当没提供transaction_id时需要传这个
        /// </summary>
        public string out_trade_no;
        /// <summary>
        /// 设置应用配置
        /// </summary>
        /// <param name="config">应用配置</param>
        internal void SetConfig(config config)
        {
            setConfig(config);
            sign<orderQuery>.Set(this, config.key);
        }
    }
}
