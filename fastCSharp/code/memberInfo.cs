using System;
using System.Collections.Generic;
using System.Reflection;
using fastCSharp.reflection;
using System.Runtime.InteropServices;
using fastCSharp.emit;
using System.Runtime.CompilerServices;

namespace fastCSharp.code
{
    /// <summary>
    /// 成员信息
    /// </summary>
    public partial class memberInfo : memberIndex
    {
        /// <summary>
        /// 成员类型
        /// </summary>
        public memberType MemberType { get; private set; }
        /// <summary>
        /// 成员名称
        /// </summary>
        public string MemberName { get; protected set; }
        /// <summary>
        /// 成员信息
        /// </summary>
        /// <param name="field">字段信息</param>
        /// <param name="index">成员编号</param>
        public memberInfo(FieldInfo field, int index)
            : base(index)
        {
            Member = field;
            Type = MemberType = field.FieldType;
            MemberName = field.Name;
        }
        /// <summary>
        /// 成员信息
        /// </summary>
        /// <param name="member">成员信息</param>
        internal memberInfo(memberIndex member)
            : base(member)
        {
            MemberType = member.Type;
            MemberName = Member.Name;
            //if (CanGet && CanSet)
            //{
            //    dataMember sqlMember = GetAttribute<dataMember>(true, false);
            //    if (sqlMember != null && sqlMember.DataType != null) MemberType = new memberType(MemberType, sqlMember.DataType);
            //}
        }
        /// <summary>
        /// 成员信息
        /// </summary>
        /// <param name="method">成员方法信息</param>
        /// <param name="filter">选择类型</param>
        protected memberInfo(MethodInfo method, memberFilters filter)
            : base(method, filter, 0)
        {
            Member = method;
            MemberName = method.Name;
        }
        /// <summary>
        /// 成员信息
        /// </summary>
        /// <param name="type">成员类型</param>
        /// <param name="name">成员名称</param>
        protected memberInfo(memberType type, string name)
            : base(0)
        {
            MemberType = type;
            MemberName = name;
        }
        /// <summary>
        /// 类型成员集合缓存
        /// </summary>
        private static readonly Dictionary<Type, memberInfo[]> memberCache = dictionary.CreateOnly<Type, memberInfo[]>();
        /// <summary>
        /// 根据类型获取成员信息集合
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>成员信息集合</returns>
        private static memberInfo[] getMembers(Type type)
        {
            memberInfo[] members;
            if (!memberCache.TryGetValue(type, out members))
            {
                memberIndexGroup group = memberIndexGroup.Get(type);
                memberCache[type] = members =
                    array.concat(group.PublicFields.getArray(value => new memberInfo(value)),
                        group.NonPublicFields.getArray(value => new memberInfo(value)),
                        group.PublicProperties.getArray(value => new memberInfo(value)),
                        group.NonPublicProperties.getArray(value => new memberInfo(value)));
            }
            return members;
        }
        /// <summary>
        /// 根据类型获取成员信息集合
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="filter">选择类型</param>
        /// <returns>成员信息集合</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static memberInfo[] GetMembers(Type type, memberFilters filter)
        {
            return getMembers(type).getFindArray(value => (value.Filter & filter) != 0);
        }
        /// <summary>
        /// 根据类型获取成员信息集合
        /// </summary>
        /// <typeparam name="attributeType">自定义属性类型</typeparam>
        /// <param name="type">类型</param>
        /// <param name="filter">选择类型</param>
        /// <param name="isAttribute">是否匹配自定义属性类型</param>
        /// <param name="isBaseType">是否搜索父类属性</param>
        /// <param name="isInheritAttribute">是否包含继承属性</param>
        /// <returns>成员信息集合</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static memberInfo[] GetMembers<attributeType>(Type type, memberFilters filter, bool isAttribute, bool isBaseType, bool isInheritAttribute)
            where attributeType : fastCSharp.code.ignoreMember
        {
            return find<memberInfo, attributeType>(GetMembers(type, filter), isAttribute, isBaseType, isInheritAttribute);
        }
    }
    ///// <summary>
    ///// 成员信息
    ///// </summary>
    ///// <typeparam name="memberType">成员类型</typeparam>
    //internal abstract class memberInfo<memberType> : memberInfo where memberType : MemberInfo
    //{
    //    /// <summary>
    //    /// 成员信息
    //    /// </summary>
    //    public new memberType Member;
    //    /// <summary>
    //    /// 成员信息
    //    /// </summary>
    //    /// <param name="member">成员信息</param>
    //    protected memberInfo(memberIndex<memberType> member)
    //        : base(member)
    //    {
    //        Member = member.Member;
    //    }
    //}
}
