using System;
using fastCSharp.emit;
#pragma warning disable

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// 扫码支付完成回调通知请求参数
    /// </summary>
    public class payNotify : returnSign
    {
        /// <summary>
        /// 调用接口提交的终端设备号
        /// </summary>
        internal string device_info;
        /// <summary>
        /// 用户标识
        /// </summary>
        public string openid;
        /// <summary>
        /// 是否关注公众账号
        /// </summary>
        public string is_subscribe;
        /// <summary>
        /// 交易类型[必填]
        /// </summary>
        public tradeType trade_type;
        /// <summary>
        /// 银行类型
        /// </summary>
        public enum bankType : byte
        {
            /// <summary>
            /// 未知
            /// </summary>
            Unknown,
            /// <summary>
            /// 工商银行（借记卡）
            /// </summary>
            ICBC_DEBIT,
            /// <summary>
            /// 工商银行（信用卡）
            /// </summary>
            ICBC_CREDIT,
            /// <summary>
            /// 农业银行（借记卡）
            /// </summary>
            ABC_DEBIT,
            /// <summary>
            /// 农业银行 （信用卡）
            /// </summary>
            ABC_CREDIT,
            /// <summary>
            /// 邮政储蓄（借记卡）
            /// </summary>
            PSBC_DEBIT,
            /// <summary>
            /// 邮政储蓄 （信用卡）
            /// </summary>
            PSBC_CREDIT,
            /// <summary>
            /// 建设银行（借记卡）
            /// </summary>
            CCB_DEBIT,
            /// <summary>
            /// 建设银行 （信用卡）
            /// </summary>
            CCB_CREDIT,
            /// <summary>
            /// 招商银行（借记卡）
            /// </summary>
            CMB_DEBIT,
            /// <summary>
            /// 招商银行（信用卡）
            /// </summary>
            CMB_CREDIT,
            /// <summary>
            /// 交通银行（借记卡）
            /// </summary>
            COMM_DEBIT,
            /// <summary>
            /// 中国银行（信用卡）
            /// </summary>
            BOC_CREDIT,
            /// <summary>
            /// 浦发银行（借记卡）
            /// </summary>
            SPDB_DEBIT,
            /// <summary>
            /// 浦发银行 （信用卡）
            /// </summary>
            SPDB_CREDIT,
            /// <summary>
            /// 广发银行（借记卡）
            /// </summary>
            GDB_DEBIT,
            /// <summary>
            /// 广发银行（信用卡）
            /// </summary>
            GDB_CREDIT,
            /// <summary>
            /// 民生银行（借记卡）
            /// </summary>
            CMBC_DEBIT,
            /// <summary>
            /// 民生银行（信用卡）
            /// </summary>
            CMBC_CREDIT,
            /// <summary>
            /// 平安银行（借记卡）
            /// </summary>
            PAB_DEBIT,
            /// <summary>
            /// 平安银行（信用卡）
            /// </summary>
            PAB_CREDIT,
            /// <summary>
            /// 光大银行（借记卡）
            /// </summary>
            CEB_DEBIT,
            /// <summary>
            /// 光大银行（信用卡）
            /// </summary>
            CEB_CREDIT,
            /// <summary>
            /// 兴业银行 （借记卡）
            /// </summary>
            CIB_DEBIT,
            /// <summary>
            /// 兴业银行（信用卡）
            /// </summary>
            CIB_CREDIT,
            /// <summary>
            /// 中信银行（借记卡）
            /// </summary>
            CITIC_DEBIT,
            /// <summary>
            /// 中信银行（信用卡）
            /// </summary>
            CITIC_CREDIT,
            /// <summary>
            /// 深发银行（信用卡）
            /// </summary>
            SDB_CREDIT,
            /// <summary>
            /// 上海银行（借记卡）
            /// </summary>
            BOSH_DEBIT,
            /// <summary>
            /// 上海银行 （信用卡）
            /// </summary>
            BOSH_CREDIT,
            /// <summary>
            /// 华润银行（借记卡）
            /// </summary>
            CRB_DEBIT,
            /// <summary>
            /// 杭州银行（借记卡）
            /// </summary>
            HZB_DEBIT,
            /// <summary>
            /// 杭州银行（信用卡）
            /// </summary>
            HZB_CREDIT,
            /// <summary>
            /// 包商银行（借记卡）
            /// </summary>
            BSB_DEBIT,
            /// <summary>
            /// 包商银行 （信用卡）
            /// </summary>
            BSB_CREDIT,
            /// <summary>
            /// 重庆银行（借记卡）
            /// </summary>
            CQB_DEBIT,
            /// <summary>
            /// 顺德农商行 （借记卡）
            /// </summary>
            SDEB_DEBIT,
            /// <summary>
            /// 深圳农商银行（借记卡）
            /// </summary>
            SZRCB_DEBIT,
            /// <summary>
            /// 哈尔滨银行（借记卡）
            /// </summary>
            HRBB_DEBIT,
            /// <summary>
            /// 成都银行（借记卡）
            /// </summary>
            BOCD_DEBIT,
            /// <summary>
            /// 南粤银行 （借记卡）
            /// </summary>
            GDNYB_DEBIT,
            /// <summary>
            /// 南粤银行 （信用卡）
            /// </summary>
            GDNYB_CREDIT,
            /// <summary>
            /// 广州银行（信用卡）
            /// </summary>
            GZCB_CREDIT,
            /// <summary>
            /// 江苏银行（借记卡）
            /// </summary>
            JSB_DEBIT,
            /// <summary>
            /// 江苏银行（信用卡）
            /// </summary>
            JSB_CREDIT,
            /// <summary>
            /// 宁波银行（借记卡）
            /// </summary>
            NBCB_DEBIT,
            /// <summary>
            /// 宁波银行（信用卡）
            /// </summary>
            NBCB_CREDIT,
            /// <summary>
            /// 南京银行（借记卡）
            /// </summary>
            NJCB_DEBIT,
            /// <summary>
            /// 青岛银行（借记卡）
            /// </summary>
            QDCCB_DEBIT,
            /// <summary>
            /// 浙江泰隆银行（借记卡）
            /// </summary>
            ZJTLCB_DEBIT,
            /// <summary>
            /// 西安银行（借记卡）
            /// </summary>
            XAB_DEBIT,
            /// <summary>
            /// 常熟农商银行 （借记卡）
            /// </summary>
            CSRCB_DEBIT,
            /// <summary>
            /// 齐鲁银行（借记卡）
            /// </summary>
            QLB_DEBIT,
            /// <summary>
            /// 龙江银行（借记卡）
            /// </summary>
            LJB_DEBIT,
            /// <summary>
            /// 华夏银行（借记卡）
            /// </summary>
            HXB_DEBIT,
            /// <summary>
            /// 测试银行借记卡快捷支付 （借记卡）
            /// </summary>
            CS_DEBIT,
            /// <summary>
            /// AE （信用卡）
            /// </summary>
            AE_CREDIT,
            /// <summary>
            /// JCB （信用卡）
            /// </summary>
            JCB_CREDIT,
            /// <summary>
            /// MASTERCARD （信用卡）
            /// </summary>
            MASTERCARD_CREDIT,
            /// <summary>
            /// VISA （信用卡）
            /// </summary>
            VISA_CREDIT
        }
        /// <summary>
        /// 银行类型
        /// </summary>
        public string bank_type;
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
        /// 现金支付货币类型
        /// </summary>
        public string cash_fee_type;
        /// <summary>
        /// 代金券或立减优惠金额 小于等于 订单总金额，订单总金额-代金券或立减优惠金额=现金支付金额，详见支付金额
        /// </summary>
        public uint? coupon_fee;
        /// <summary>
        /// 代金券或立减优惠使用数量
        /// </summary>
        private uint? coupon_count;
        /// <summary>
        /// 代金券或立减优惠
        /// </summary>
        public struct coupon
        {
            /// <summary>
            /// 下标
            /// </summary>
            public int index;
            /// <summary>
            /// 代金券或立减优惠ID
            /// </summary>
            public string coupon_id;
            /// <summary>
            /// 单个代金券或立减优惠支付金额
            /// </summary>
            public uint coupon_fee;
        }
        /// <summary>
        /// 代金券或立减优惠(存在sign验证问题)
        /// </summary>
        public coupon[] coupons;
        /// <summary>
        /// 代金券或立减优惠解析
        /// </summary>
        /// <param name="parser"></param>
        /// <returns></returns>
        [fastCSharp.emit.xmlSerialize.custom]
        private unsafe static bool parseCoupon(xmlParser parser, ref payNotify value, ref pointer.size name)
        {
            return value.parseCoupon(parser, name.Char);
        }
        /// <summary>
        /// 代金券或立减优惠解析
        /// </summary>
        /// <param name="parser"></param>
        /// <returns></returns>
        protected unsafe bool parseCoupon(xmlParser parser, char* name)
        {
            int index;
            char code = *(name + 7);
            if (code == 'i')
            {
                if ((index = getCouponIndex("coupon_id_", name)) >= 0)
                {
                    return parser.UnsafeParse(ref coupons[index].coupon_id);
                }
            }
            else if (code == 'f')
            {
                if ((index = getCouponIndex("coupon_fee_", name)) >= 0)
                {
                    return parser.UnsafeParse(ref coupons[index].coupon_fee);
                }
            }
            return parser.IgnoreValue();
        }
        /// <summary>
        /// 获取下标
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameStart"></param>
        /// <returns></returns>
        private unsafe int getCouponIndex(string name, char* nameStart)
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
                if (coupon_count != null && index < (int)coupon_count)
                {
                    if (coupons == null) coupons = new coupon[(int)coupon_count];
                    return index;
                }
            }
            return -1;
        }
        /// <summary>
        /// 微信支付订单号
        /// </summary>
        public string transaction_id;
        /// <summary>
        /// 商户系统的订单号，与请求一致。
        /// </summary>
        public string out_trade_no;
        /// <summary>
        /// 商家数据包，原样返回
        /// </summary>
        public string attach;
        /// <summary>
        /// 支付完成时间，格式为yyyyMMddHHmmss，如2009年12月25日9点10分10秒表示为20091225091010。其他详见
        /// </summary>
        public string time_end;
        /// <summary>
        /// 微信支付完成回调验证
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool Verify(config config = null)
        {
            if (IsValue)
            {
                if (config == null) config = config.Default;
                if (appid == config.appid && mch_id == config.mch_id && sign<payNotify>.Check(this, config.key, sign)) return true;
                config.PayLog.Add("微信支付回调验证错误 " + this.ToJson(), new System.Diagnostics.StackFrame(), false);
            }
            return false;
        }
        /// <summary>
        /// 微信支付完成回调验证
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        public bool Verify(api api)
        {
            return Verify(api.config);
        }
        /// <summary>
        /// 获取扫码支付回调返回值
        /// </summary>
        /// <param name="return_msg">返回信息 返回信息，如非空，为错误原因 签名失败 参数格式校验错误</param>
        /// <returns></returns>
        public returnCode GetErrorResult(string return_msg)
        {
            config.PayLog.Add(return_msg, new System.Diagnostics.StackFrame(), false);
            return new returnCode { return_msg = return_msg };
        }
    }
}
