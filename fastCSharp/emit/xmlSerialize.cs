using System;
using fastCSharp.code;

namespace fastCSharp.emit
{
    /// <summary>
    /// XML序列化类型配置
    /// </summary>
    public sealed class xmlSerialize : memberFilter.publicInstance
    {
        /// <summary>
        /// 默认反序列化类型配置
        /// </summary>
        internal static readonly xmlSerialize AllMember = new xmlSerialize { Filter = code.memberFilters.Instance, IsAllMember = true, IsBaseType = false };
        /// <summary>
        /// 默认序列化类型配置
        /// </summary>
        internal static readonly xmlSerialize Default = new xmlSerialize { IsAllMember = true, IsBaseType = false };
        /// <summary>
        /// 匿名类型序列化配置
        /// </summary>
        internal static readonly xmlSerialize AnonymousTypeMember = new xmlSerialize { IsAllMember = true, IsBaseType = false, Filter = memberFilters.InstanceField };
        /// <summary>
        /// 是否序列化所有成员
        /// </summary>
        public bool IsAllMember;
        /// <summary>
        /// 是否作用与派生类型
        /// </summary>
        public bool IsBaseType = true;
        /// <summary>
        /// XML序列化成员配置
        /// </summary>
        public sealed class member : ignoreMember
        {
            /// <summary>
            /// 集合子节点名称(不能包含非法字符)
            /// </summary>
            public string ItemName;
        }
        /// <summary>
        /// 自定义类型函数标识配置
        /// </summary>
        public sealed class custom : Attribute
        {
        }
        /// <summary>
        /// 未知名称解析函数标识配置
        /// </summary>
        public sealed class unknownName : Attribute
        {
        }
    }
}
