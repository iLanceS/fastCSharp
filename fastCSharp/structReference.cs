using System;

namespace fastCSharp
{
    /// <summary>
    /// 结构体引用，用于静态变量
    /// </summary>
    /// <typeparam name="valueType"></typeparam>
    internal sealed class structReference<valueType> where valueType : struct
    {
        /// <summary>
        /// 结构体数据
        /// </summary>
        public valueType Value;
    }
}
