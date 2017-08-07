using System;
using fastCSharp.code;

namespace fastCSharp.emit
{
    /// <summary>
    /// 二进制序列化配置
    /// </summary>
    public abstract class binarySerialize : memberFilter.instanceField
    {
        /// <summary>
        /// 是否作用于未知派生类型
        /// </summary>
        public bool IsBaseType = true;
        /// <summary>
        /// 当没有JSON序列化成员时是否预留JSON序列化标记
        /// </summary>
        public bool IsJson;
        /// <summary>
        /// 二进制数据序列化成员配置
        /// </summary>
        public sealed class member : ignoreMember
        {
            /// <summary>
            /// 是否采用JSON混合序列化
            /// </summary>
            public bool IsJson;
        }
    }
}
