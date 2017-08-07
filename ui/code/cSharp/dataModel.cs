using System;
using fastCSharp.emit;
using System.Collections.Generic;

namespace fastCSharp.code
{
    /// <summary>
    /// 数据库表格模型
    /// </summary>
    internal partial class dataModel
    {
        /// <summary>
        /// 数据库表格模型码生成
        /// </summary
        internal abstract class cSharp<modelType> : member<modelType>
            where modelType : fastCSharp.code.cSharp.dataModel
        {
            /// <summary>
            /// 自增成员
            /// </summary>
            public code.memberInfo Identity;
            /// <summary>
            /// 关键字集合
            /// </summary>
            public subArray<code.memberInfo> PrimaryKeys;
            /// <summary>
            /// 第一个关键字
            /// </summary>
            public code.memberInfo PrimaryKey0
            {
                get { return PrimaryKeys[0]; }
            }
            /// <summary>
            /// 后续关键字
            /// </summary>
            public subArray<code.memberInfo> NextPrimaryKeys
            {
                get { return PrimaryKeys.GetSub(1, PrimaryKeys.Count - 1); }
            }
            /// <summary>
            /// 是否多关键字
            /// </summary>
            public bool IsManyPrimaryKey
            {
                get { return PrimaryKeys.Count > 1; }
            }
            /// <summary>
            /// 关键字类型名称
            /// </summary>
            public string PrimaryKeyType
            {
                get { return IsManyPrimaryKey ? "primaryKey" : PrimaryKey0.MemberType.FullName; }
            }
            /// <summary>
            /// 是否生成代码
            /// </summary>
            protected virtual bool IsCreate
            {
                get { return true; }
            }
            /// <summary>
            /// 安装下一个类型
            /// </summary>
            protected override void nextCreate()
            {
                Identity = fastCSharp.code.cSharp.dataModel.GetIdentity<modelType>(type, Attribute);
                PrimaryKeys = fastCSharp.code.cSharp.dataModel.GetPrimaryKeys<modelType>(type, Attribute);
                if (IsCreate) create(true);
            }
        }
    }
}

namespace fastCSharp.code.cSharp.template
{
    /// <summary>
    /// CSharp模板公用模糊类型
    /// </summary>
    internal partial class pub
    {
        /// <summary>
        /// 关键字类型
        /// </summary>
        public struct PrimaryKeyType : IEquatable<PrimaryKeyType>
        {
            /// <summary>
            /// 关键字比较
            /// </summary>
            /// <param name="other">关键字</param>
            /// <returns>是否相等</returns>
            public bool Equals(PrimaryKeyType other)
            {
                return false;
            }
            /// <summary>
            /// 哈希编码
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return 0;
            }
        }
    }
}