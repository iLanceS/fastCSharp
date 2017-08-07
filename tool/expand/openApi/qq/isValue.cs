using System;
using fastCSharp;

namespace fastCSharp.openApi.qq
{
    /// <summary>
    /// 数据是否有效
    /// </summary>
    public abstract class isValue : IValue
    {
        /// <summary>
        /// 返回码，0表示正确返回
        /// </summary>
        public int ret = -1;
#pragma warning disable
        /// <summary>
        /// 如果ret小于0，会有相应的错误信息提示
        /// </summary>
        public string msg;
#pragma warning restore
        /// <summary>
        /// 数据是否有效
        /// </summary>
        public bool IsValue
        {
            get { return ret == 0; }
        }
        /// <summary>
        /// 提示信息
        /// </summary>
        public virtual string Message
        {
            get
            {
                return "[" + ret.toString() + "]" + msg;
            }
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="value">数据是否有效</param>
        /// <returns>数据是否有效</returns>
        public static implicit operator bool(isValue value) { return value != null && value.IsValue; }
    }
}
