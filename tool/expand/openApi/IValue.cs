using System;

namespace fastCSharp.openApi
{
    /// <summary>
    /// 数据是否有效
    /// </summary>
    public interface IValue
    {
        /// <summary>
        /// 数据是否有效
        /// </summary>
        bool IsValue { get; }
        /// <summary>
        /// 提示信息
        /// </summary>
        string Message { get; }
    }
}
