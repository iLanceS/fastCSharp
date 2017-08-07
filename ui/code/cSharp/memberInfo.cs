using System;
#pragma warning disable

namespace fastCSharp.code.cSharp
{
    /// <summary>
    /// 自定义成员信息
    /// </summary>
    internal sealed class memberInfo
    {
        /// <summary>
        /// 成员类型
        /// </summary>
        public Type MemberType;
        /// <summary>
        /// 成员名称
        /// </summary>
        public string MemberName;
        /// <summary>
        /// 成员编号
        /// </summary>
        public int MemberIndex;
        ///// <summary>
        ///// 转换成员信息
        ///// </summary>
        ///// <returns>成员信息</returns>
        //internal code.memberInfo create()
        //{
        //    return new code.memberInfo(MemberType, MemberName, MemberIndex);
        //}
    }
}
