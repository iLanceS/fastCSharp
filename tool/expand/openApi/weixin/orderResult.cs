using System;
using fastCSharp.emit;

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// 订单查询结果
    /// </summary>
    public sealed class orderResult : payNotify
    {
        /// <summary>
        /// 交易状态
        /// </summary>
        public enum tradeState : byte
        {
            /// <summary>
            /// 未知
            /// </summary>
            Unknown,
            /// <summary>
            /// 支付成功
            /// </summary>
            SUCCESS,
            /// <summary>
            /// 转入退款
            /// </summary>
            REFUND,
            /// <summary>
            /// 未支付
            /// </summary>
            NOTPAY,
            /// <summary>
            /// 已关闭
            /// </summary>
            CLOSED,
            /// <summary>
            /// 已撤销（刷卡支付）
            /// </summary>
            REVOKED,
            /// <summary>
            /// 用户支付中
            /// </summary>
            USERPAYING,
            /// <summary>
            /// 支付失败(其他原因，如银行返回失败)
            /// </summary>
            PAYERROR
        }
        /// <summary>
        /// 交易状态
        /// </summary>
        public tradeState trade_state;
        /// <summary>
        /// `对当前查询订单状态的描述和下一步操作的指引
        /// </summary>
        public string trade_state_desc;
        /// <summary>
        /// 代金券或立减优惠解析
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [fastCSharp.emit.xmlSerialize.custom]
        private unsafe static bool parseCoupon(xmlParser parser, ref orderResult value, ref pointer.size name)
        {
            return value.parseCoupon(parser, name.Char);
        }
        /// <summary>
        /// 签名验证
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public new bool Verify(config config = null)
        {
            if (config == null) config = config.Default;
            if (appid == config.appid && mch_id == config.mch_id && sign<payNotify>.Check(this, config.key, sign)) return true;
            log.Error.Add("签名验证错误 " + this.ToJson(), new System.Diagnostics.StackFrame(), false);
            return false;
        }
    }
}
