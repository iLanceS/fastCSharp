using System;
using fastCSharp.emit;
#pragma warning disable

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// 退款查询结果
    /// </summary>
    public sealed class refundResult : returnSign
    {
        /// <summary>
        /// 调用接口提交的终端设备号
        /// </summary>
        internal string device_info;
        /// <summary>
        /// 微信支付订单号
        /// </summary>
        public string transaction_id;
        /// <summary>
        /// 商户系统的订单号，与请求一致。
        /// </summary>
        public string out_trade_no;
        /// <summary>
        /// 订单总金额，单位分
        /// </summary>
        public uint total_fee;
        /// <summary>
        /// 现金支付金额，单位分
        /// </summary>
        public uint cash_fee;
        /// <summary>
        /// 货币类型，符合ISO 4217标准的三位字母代码，其他值列表详见货币类型
        /// </summary>
        public string fee_type;
        /// <summary>
        /// 退款笔数
        /// </summary>
        private uint refund_count;
        /// <summary>
        /// 退款信息集合
        /// </summary>
        public refund[] refunds;
        /// <summary>
        /// 退款信息
        /// </summary>
        public struct refund
        {
            /// <summary>
            /// 商户退款单号
            /// </summary>
            public string out_refund_no;
            /// <summary>
            /// 微信退款单号
            /// </summary>
            public string refund_id;
            /// <summary>
            /// 退款渠道
            /// </summary>
            public enum channel : byte
            {
                /// <summary>
                /// 原路退款
                /// </summary>
                ORIGINAL,
                /// <summary>
                /// 退回到余额
                /// </summary>
                BALANCE
            }
            /// <summary>
            /// 退款渠道
            /// </summary>
            public channel? refund_channel;
            /// <summary>
            /// 退款总金额,单位为分,可以做部分退款
            /// </summary>
            public uint refund_fee;
            /// <summary>
            /// 代金券或立减优惠退款金额，退款金额-代金券或立减优惠退款金额为现金
            /// </summary>
            public uint? coupon_refund_fee;
            /// <summary>
            /// 代金券或立减优惠使用数量
            /// </summary>
            internal uint? coupon_refund_count;
            /// <summary>
            /// 代金券或立减优惠
            /// </summary>
            public coupon[] coupons;
            /// <summary>
            /// 代金券或立减优惠
            /// </summary>
            public struct coupon
            {
                /// <summary>
                /// 批次ID ,$n为下标，$m为下标，从0开始编号
                /// </summary>
                public string coupon_refund_batch_id;
                /// <summary>
                /// 代金券或立减优惠ID, $n为下标，$m为下标，从0开始编号
                /// </summary>
                public string coupon_refund_id;
                /// <summary>
                /// 单个代金券或立减优惠支付金额, $n为下标，$m为下标，从0开始编号
                /// </summary>
                public uint coupon_refund_fee;
            }
            /// <summary>
            /// 退款状态
            /// </summary>
            public enum status : byte
            {
                /// <summary>
                /// 退款成功
                /// </summary>
                SUCCESS,
                /// <summary>
                /// 退款失败
                /// </summary>
                FAIL,
                /// <summary>
                /// 退款处理中
                /// </summary>
                PROCESSING,
                /// <summary>
                /// 未确定，需要商户原退款单号重新发起
                /// </summary>
                NOTSURE,
                /// <summary>
                /// 转入代发，退款到银行发现用户的卡作废或者冻结了，导致原路退款银行卡失败，资金回流到商户的现金帐号，需要商户人工干预，通过线下或者财付通转账的方式进行退款。
                /// </summary>
                CHANGE
            }
            /// <summary>
            /// 退款状态
            /// </summary>
            public status refund_status;
        }
        /// <summary>
        /// 退款信息
        /// </summary>
        /// <param name="parser"></param>
        /// <returns></returns>
        [fastCSharp.emit.xmlSerialize.custom]
        private unsafe static bool parseRefund(xmlParser parser, ref refundResult value, ref pointer.size name)
        {
            return value.parseRefund(parser, name.Char);
        }
        /// <summary>
        /// 退款信息
        /// </summary>
        /// <param name="parser"></param>
        /// <returns></returns>
        private unsafe bool parseRefund(xmlParser parser, char* name)
        {
            int index, couponIndex = 0;
            switch (*name - 'c')
            {
                case 'c' - 'c':
                    switch (*(name + 14) - 'b')
                    {
                        case 'f' - 'b':
                            if ((index = getCouponIndex("coupon_refund_fee_", name, ref couponIndex)) >= 0)
                            {
                                if (couponIndex >= 0) return parser.UnsafeParse(ref refunds[index].coupons[couponIndex].coupon_refund_fee);
                                else return parser.UnsafeParse(ref refunds[index].coupon_refund_fee);
                            }
                            break;
                        case 'c' - 'b':
                            if ((index = getRefundIndex("coupon_refund_count_", name)) >= 0)
                            {
                                return parser.UnsafeParse(ref refunds[index].coupon_refund_count);
                            }
                            break;
                        case 'b' - 'b':
                            if ((index = getCouponIndex("coupon_refund_batch_id_", name, ref couponIndex)) >= 0 && couponIndex >= 0)
                            {
                                return parser.UnsafeParse(ref refunds[index].coupons[couponIndex].coupon_refund_batch_id);
                            }
                            break;
                        case 'i' - 'b':
                            if ((index = getCouponIndex("coupon_refund_id_", name, ref couponIndex)) >= 0 && couponIndex >= 0)
                            {
                                return parser.UnsafeParse(ref refunds[index].coupons[couponIndex].coupon_refund_id);
                            }
                            break;
                    }
                    break;
                case 'r' - 'c':
                    switch (*(name + 7) - 'c')
                    {
                        case 'i' - 'c':
                            if ((index = getRefundIndex("refund_id_", name)) >= 0)
                            {
                                return parser.UnsafeParse(ref refunds[index].refund_id);
                            }
                            break;
                        case 'c' - 'c':
                            if ((index = getRefundIndex("refund_channel_", name)) >= 0)
                            {
                                return parser.UnsafeEnumByte(ref refunds[index].refund_channel);
                            }
                            break;
                        case 'f' - 'c':
                            if ((index = getRefundIndex("refund_fee_", name)) >= 0)
                            {
                                return parser.UnsafeParse(ref refunds[index].refund_fee);
                            }
                            break;
                        case 's' - 'c':
                            if ((index = getRefundIndex("refund_status_", name)) >= 0)
                            {
                                return parser.UnsafeEnumByte(ref refunds[index].refund_status);
                            }
                            break;
                    }
                    break;
                case 'o' - 'c':
                    if ((index = getRefundIndex("out_refund_no_", name)) >= 0)
                    {
                        return parser.UnsafeParse(ref refunds[index].out_refund_no);
                    }
                    break;
            }
            return parser.IgnoreValue();
        }
        /// <summary>
        /// 获取下标
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameStart"></param>
        /// <returns></returns>
        private unsafe int getRefundIndex(string name, char* nameStart)
        {
            if (fastCSharp.unsafer.String.SimpleEqual(name, nameStart))
            {
                int index = *(nameStart += name.Length) - '0';
                do
                {
                    uint number = (uint)(*++nameStart - '0');
                    if (number > 9) break;
                    index = index * 10 + (int)number;
                }
                while (true);
                if (index < refund_count)
                {
                    if (refunds == null) refunds = new refund[refund_count];
                    return index;
                }
            }
            return -1;
        }
        /// <summary>
        /// 获取下标
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameStart"></param>
        /// <returns></returns>
        private unsafe int getCouponIndex(string name, char* nameStart, ref int couponIndex)
        {
            if (fastCSharp.unsafer.String.SimpleEqual(name, nameStart))
            {
                int index = *(nameStart += name.Length) - '0';
                do
                {
                    uint number = (uint)(*++nameStart - '0');
                    if (number > 9) break;
                    index = index * 10 + (int)number;
                }
                while (true);
                if (index < refund_count && index >= 0)
                {
                    if (refunds == null) refunds = new refund[refund_count];
                    if (*nameStart == '_')
                    {
                        couponIndex = *++nameStart - '0';
                        do
                        {
                            uint number = (uint)(*++nameStart - '0');
                            if (number > 9) break;
                            couponIndex = couponIndex * 10 + (int)number;
                        }
                        while (true);
                        if (refunds[index].coupon_refund_count != null && couponIndex < (uint)refunds[index].coupon_refund_count && couponIndex >= 0)
                        {
                            if (refunds[index].coupons == null) refunds[index].coupons = new refund.coupon[(uint)refunds[index].coupon_refund_count];
                            return -1;
                        }
                        return -1;
                    }
                    return index;
                }
            }
            return -1;
        }
        /// <summary>
        /// 签名验证
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        internal bool Verify(config config)
        {
            if (IsValue)
            {
                if (appid == config.appid && mch_id == config.mch_id && sign<refundResult>.Check(this, config.key, sign)) return true;
                log.Error.Add("签名验证错误 " + this.ToJson(), new System.Diagnostics.StackFrame(), false);
            }
            return false;
        }
    }
}
