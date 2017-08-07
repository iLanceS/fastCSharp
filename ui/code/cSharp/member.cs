using System;
using fastCSharp.reflection;

namespace fastCSharp.code
{
    /// <summary>
    /// 自定义属性字段模板生成基类
    /// </summary>
    /// <typeparam name="attributeType">自定义属性类型</typeparam>
    internal abstract class member<attributeType> : cSharper<attributeType>, IAuto
        where attributeType : fastCSharp.code.memberFilter
    {
        /// <summary>
        /// 成员集合
        /// </summary>
        internal code.memberInfo[] Members;
        ///// <summary>
        ///// 成员集合数量
        ///// </summary>
        //public int MemberCount
        //{
        //    get { return Members.Length; }
        //}
        /// <summary>
        /// 获取SQL成员信息集合
        /// </summary>
        /// <returns>SQL成员信息集合</returns>
        protected code.memberInfo[] getMembers()
        {
            return code.memberInfo.GetMembers(type, Attribute.MemberFilter).getFindArray(value => !value.IsIgnore);
        }
        /// <summary>
        /// 获取SQL成员信息集合
        /// </summary>
        /// <typeparam name="memberAttributeType">成员自定义属性类型</typeparam>
        /// <returns>SQL成员信息集合</returns>
        protected code.memberInfo[] getMembers<memberAttributeType>() where memberAttributeType : ignoreMember
        {
            return code.memberInfo.GetMembers<memberAttributeType>(type, Attribute.MemberFilter, Attribute.IsAttribute, Attribute.IsBaseTypeAttribute, Attribute.IsInheritAttribute);
        }
        /// <summary>
        /// 安装完成处理
        /// </summary>
        protected override void onCreated() { }
    }
    ///// <summary>
    ///// 成员自定义属性
    ///// </summary>
    //internal sealed class member : Attribute
    //{
    //    /// <summary>
    //    /// 成员类型
    //    /// </summary>
    //    public Type Type;
    //}
}
