using System;

namespace fastCSharp.openApi._51nod
{
    /// <summary>
    /// 回调类型
    /// </summary>
    public enum callbackType
    {
        /// <summary>
        /// 未知类型，可能反序列化失败
        /// </summary>
        Unknown,
        /// <summary>
        /// 外部提交回调
        /// </summary>
        OpenJudge
    }
    /// <summary>
    /// API回调返回值
    /// </summary>
    /// <typeparam name="valueType"></typeparam>
    public struct callback<valueType>
    {
        /// <summary>
        /// 回调类型
        /// </summary>
        public callbackType Type;
        /// <summary>
        /// 回调数据
        /// </summary>
        public valueType Value;
    }
}
