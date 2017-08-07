using System;

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// XML返回值
    /// </summary>
    [fastCSharp.emit.xmlSerialize(Filter = fastCSharp.code.memberFilters.InstanceField, IsAllMember = true, IsBaseType = false)]
    public class returnCode
    {
        /// <summary>
        /// 返回信息 返回信息，如非空，为错误原因 签名失败 参数格式校验错误
        /// </summary>
        internal string return_msg;
        /// <summary>
        /// 返回状态码
        /// </summary>
        public enum code : byte
        {
            /// <summary>
            /// 失败
            /// </summary>
            FAIL,
            /// <summary>
            /// 成功
            /// </summary>
            SUCCESS
        }
        /// <summary>
        /// 返回状态码，此字段是通信标识，非交易标识，交易是否成功需要查看result_code来判断[必填]
        /// </summary>
        internal code return_code;
        /// <summary>
        /// 获取成功返回值
        /// </summary>
        /// <returns></returns>
        public static returnCode GetSuccess()
        {
            return new returnCode { return_code = code.SUCCESS };
        }
    }
}
