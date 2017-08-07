using System;
using fastCSharp.code;

namespace fastCSharp.emit
{
    /// <summary>
    /// 二进制数据序列化类型配置(内存数据库专用)
    /// </summary>
    public sealed class indexSerialize : binarySerialize
    {
        /// <summary>
        /// 默认二进制数据序列化类型配置
        /// </summary>
        internal static readonly indexSerialize Default = new indexSerialize();
        ///// <summary>
        ///// 是否检测相同的引用成员
        ///// </summary>
        //public bool IsReferenceMember = true;
    }
}
