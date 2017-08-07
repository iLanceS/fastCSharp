using System;
using fastCSharp.code;

namespace fastCSharp.emit
{
    /// <summary>
    /// Json解析类型配置
    /// </summary>
    public sealed class jsonParse : memberFilter.instance
    {
        /// <summary>
        /// 默认解析所有成员
        /// </summary>
        internal static readonly jsonParse AllMember = new jsonParse { Filter = code.memberFilters.Instance, IsAllMember = true, IsBaseType = false };
        /// <summary>
        /// 是否解析所有成员
        /// </summary>
        public bool IsAllMember;
        /// <summary>
        /// 是否作用与派生类型
        /// </summary>
        public bool IsBaseType = true;
        /// <summary>
        /// Json解析成员配置
        /// </summary>
        public sealed class member : ignoreMember
        {
            /// <summary>
            /// 是否默认解析成员
            /// </summary>
            public bool IsDefault;
        }
        /// <summary>
        /// 自定义类型解析函数标识配置
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
