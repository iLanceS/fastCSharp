using System;

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// XML返回值
    /// </summary>
    public abstract class resultCode : returnCode, IValue
    {
        /// <summary>
        /// 错误代码描述
        /// </summary>
        internal string err_code_des;
        /// <summary>
        /// 业务结果
        /// </summary>
        internal code result_code;

        /// <summary>
        /// 数据是否有效
        /// </summary>
        public bool IsValue
        {
            get { return result_code == code.SUCCESS; }
        }

        /// <summary>
        /// 提示信息
        /// </summary>
        public virtual string Message
        {
            get
            {
                return return_code == code.SUCCESS ? err_code_des : return_msg;
            }
        }
    }
}
