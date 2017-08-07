using System;
using fastCSharp.code;

namespace fastCSharp.emit
{
    /// <summary>
    /// web表单类型配置
    /// </summary>
    public sealed class form : memberFilter.instanceField
    {
        /// <summary>
        /// 默认web表单类型配置
        /// </summary>
        internal static readonly form AllMember = new form { IsAllMember = true };
        /// <summary>
        /// 是否序列化所有成员
        /// </summary>
        public bool IsAllMember;
        /// <summary>
        /// web表单成员配置
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
