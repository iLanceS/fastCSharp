using System;
using fastCSharp.code;

namespace fastCSharp.emit
{
    /// <summary>
    /// web查询字符串类型配置
    /// </summary>
    public sealed class urlQuery : memberFilter.instanceField
    {
        /// <summary>
        /// 默认web查询字符串类型配置
        /// </summary>
        internal static readonly urlQuery AllMember = new urlQuery { IsAllMember = true };
        /// <summary>
        /// 是否序列化所有成员
        /// </summary>
        public bool IsAllMember;
        /// <summary>
        /// web查询字符串成员配置
        /// </summary>
        public sealed class member : ignoreMember
        {
        }
        /// <summary>
        /// 自定义类型函数标识配置
        /// </summary>
        public sealed class custom : Attribute
        {
        }
    }
}
