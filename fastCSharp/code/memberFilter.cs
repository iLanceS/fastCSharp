using System;
using System.Reflection;

namespace fastCSharp.code
{
    /// <summary>
    /// 成员选择
    /// </summary>
    public abstract class memberFilter : Attribute
    {
        /// <summary>
        /// 成员选择类型
        /// </summary>
        public abstract code.memberFilters MemberFilter { get; }
        /// <summary>
        /// 成员是否匹配自定义属性类型，默认为 false 表示选择所有成员。
        /// </summary>
        public bool IsAttribute;
        /// <summary>
        /// 指定是否搜索该成员的继承链以查找这些特性，参考System.Reflection.MemberInfo.GetCustomAttributes(bool inherit)。
        /// </summary>
        public bool IsBaseTypeAttribute;
        /// <summary>
        /// 成员匹配自定义属性是否可继承，true 表示允许申明默认配置类型的派生类型并且选择继承深度最小的申明配置。
        /// </summary>
        public bool IsInheritAttribute;
        /// <summary>
        /// 默认公有动态成员
        /// </summary>
        public abstract class instance : memberFilter
        {
            /// <summary>
            /// 成员选择类型
            /// </summary>
            public code.memberFilters Filter = code.memberFilters.Instance;
            /// <summary>
            /// 成员选择类型
            /// </summary>
            public override code.memberFilters MemberFilter
            {
                get { return Filter & code.memberFilters.Instance; }
            }
            ///// <summary>
            ///// 获取字段选择
            ///// </summary>
            //internal BindingFlags FieldBindingFlags
            //{
            //    get
            //    {
            //        BindingFlags flags = BindingFlags.Default;
            //        if ((Filter & code.memberFilter.PublicInstanceField) == code.memberFilter.PublicInstanceField) flags |= BindingFlags.Public;
            //        if ((Filter & code.memberFilter.NonPublicInstanceField) == code.memberFilter.NonPublicInstanceField) flags |= BindingFlags.NonPublic;
            //        return flags == BindingFlags.Default ? flags : (flags | BindingFlags.Instance);
            //    }
            //}
            ///// <summary>
            ///// 获取属性选择
            ///// </summary>
            //internal BindingFlags PropertyBindingFlags
            //{
            //    get
            //    {
            //        BindingFlags flags = BindingFlags.Default;
            //        if ((Filter & code.memberFilter.PublicInstanceProperty) == code.memberFilter.PublicInstanceProperty) flags |= BindingFlags.Public;
            //        if ((Filter & code.memberFilter.NonPublicInstanceProperty) == code.memberFilter.NonPublicInstanceProperty) flags |= BindingFlags.NonPublic;
            //        return flags == BindingFlags.Default ? flags : (flags | BindingFlags.Instance);
            //    }
            //}
        }
        /// <summary>
        /// 默认公有动态成员
        /// </summary>
        public abstract class publicInstance : memberFilter
        {
            /// <summary>
            /// 成员选择类型
            /// </summary>
            public code.memberFilters Filter = code.memberFilters.PublicInstance;
            /// <summary>
            /// 成员选择类型
            /// </summary>
            public override code.memberFilters MemberFilter
            {
                get { return Filter & code.memberFilters.Instance; }
            }
            /// <summary>
            /// 获取字段选择
            /// </summary>
            internal BindingFlags FieldBindingFlags
            {
                get
                {
                    BindingFlags flags = BindingFlags.Default;
                    if ((Filter & code.memberFilters.PublicInstanceField) == code.memberFilters.PublicInstanceField) flags |= BindingFlags.Public;
                    if ((Filter & code.memberFilters.NonPublicInstanceField) == code.memberFilters.NonPublicInstanceField) flags |= BindingFlags.NonPublic;
                    return flags == BindingFlags.Default ? flags : (flags | BindingFlags.Instance);
                }
            }
            /// <summary>
            /// 获取属性选择
            /// </summary>
            internal BindingFlags PropertyBindingFlags
            {
                get
                {
                    BindingFlags flags = BindingFlags.Default;
                    if ((Filter & code.memberFilters.PublicInstanceProperty) == code.memberFilters.PublicInstanceProperty) flags |= BindingFlags.Public;
                    if ((Filter & code.memberFilters.NonPublicInstanceProperty) == code.memberFilters.NonPublicInstanceProperty) flags |= BindingFlags.NonPublic;
                    return flags == BindingFlags.Default ? flags : (flags | BindingFlags.Instance);
                }
            }
        }
        /// <summary>
        /// 默认公有动态字段成员
        /// </summary>
        public abstract class publicInstanceField : memberFilter
        {
            /// <summary>
            /// 成员选择类型
            /// </summary>
            public code.memberFilters Filter = code.memberFilters.PublicInstanceField;
            /// <summary>
            /// 成员选择类型
            /// </summary>
            public override code.memberFilters MemberFilter
            {
                get { return Filter & code.memberFilters.Instance; }
            }
            /// <summary>
            /// 获取字段选择
            /// </summary>
            internal BindingFlags FieldBindingFlags
            {
                get
                {
                    BindingFlags flags = BindingFlags.Default;
                    if ((Filter & code.memberFilters.PublicInstanceField) == code.memberFilters.PublicInstanceField) flags |= BindingFlags.Public;
                    if ((Filter & code.memberFilters.NonPublicInstanceField) == code.memberFilters.NonPublicInstanceField) flags |= BindingFlags.NonPublic;
                    return flags == BindingFlags.Default ? flags : (flags | BindingFlags.Instance);
                }
            }
        }
        /// <summary>
        /// 默认动态字段成员
        /// </summary>
        public abstract class instanceField : memberFilter
        {
            /// <summary>
            /// 成员选择类型
            /// </summary>
            public code.memberFilters Filter = code.memberFilters.InstanceField;
            /// <summary>
            /// 成员选择类型
            /// </summary>
            public override code.memberFilters MemberFilter
            {
                get { return Filter & code.memberFilters.Instance; }
            }
            /// <summary>
            /// 获取字段选择
            /// </summary>
            public BindingFlags FieldBindingFlags
            {
                get
                {
                    BindingFlags flags = BindingFlags.Default;
                    if ((Filter & code.memberFilters.PublicInstanceField) == code.memberFilters.PublicInstanceField) flags |= BindingFlags.Public;
                    if ((Filter & code.memberFilters.NonPublicInstanceField) == code.memberFilters.NonPublicInstanceField) flags |= BindingFlags.NonPublic;
                    return flags == BindingFlags.Default ? flags : (flags | BindingFlags.Instance);
                }
            }
        }
        /// <summary>
        /// 默认动态属性成员
        /// </summary>
        public abstract class publicInstanceProperty : memberFilter
        {
            /// <summary>
            /// 成员选择类型
            /// </summary>
            public code.memberFilters Filter = code.memberFilters.PublicInstanceProperty;
            /// <summary>
            /// 成员选择类型
            /// </summary>
            public override code.memberFilters MemberFilter
            {
                get { return Filter & code.memberFilters.Instance; }
            }
            /// <summary>
            /// 获取字段选择
            /// </summary>
            public BindingFlags FieldBindingFlags
            {
                get
                {
                    BindingFlags flags = BindingFlags.Default;
                    if ((Filter & code.memberFilters.PublicInstanceProperty) == code.memberFilters.PublicInstanceProperty) flags |= BindingFlags.Public;
                    return flags == BindingFlags.Default ? flags : (flags | BindingFlags.Instance);
                }
            }
        }
    }
}
