using System;
using fastCSharp.code;

namespace fastCSharp.emit
{
    /// <summary>
    /// JSON序列化类型配置
    /// </summary>
    public sealed class jsonSerialize : memberFilter.publicInstance
    {
        /// <summary>
        /// 默认序列化类型配置
        /// </summary>
        internal static readonly jsonSerialize AllMember = new jsonSerialize { IsAllMember = true, IsBaseType = false };
        /// <summary>
        /// 匿名类型序列化配置
        /// </summary>
        internal static readonly jsonSerialize AnonymousTypeMember = new jsonSerialize { IsAllMember = true, IsBaseType = false, Filter = memberFilters.InstanceField };
        /// <summary>
        /// 是否序列化所有成员
        /// </summary>
        public bool IsAllMember;
        /// <summary>
        /// 是否作用与派生类型
        /// </summary>
        public bool IsBaseType = true;
        /// <summary>
        /// Json序列化成员配置
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
