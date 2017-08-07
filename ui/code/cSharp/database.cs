using System;
using fastCSharp.sql;
using fastCSharp.emit;

namespace fastCSharp.code
{
    /// <summary>
    /// 数据库配置
    /// </summary>
    internal abstract class database
    {
        /// <summary>
        /// 数据库代码生成
        /// </summary>
        /// <typeparam name="attributeType">数据库配置类型</typeparam>
        internal abstract class cSharp<attributeType> : member<attributeType> where attributeType : fastCSharp.code.cSharp.database
        {
            /// <summary>
            /// 关键字成员信息
            /// </summary>
            public class primaryKey
            {
                /// <summary>
                /// 成员信息
                /// </summary>
                public code.memberInfo Member;
                /// <summary>
                /// 关键字名称
                /// </summary>
                public string PrimaryKeyName;
                /// <summary>
                /// 是否最后一个关键字
                /// </summary>
                public bool IsLastPrimaryKey;
            }
            /// <summary>
            /// 自增列
            /// </summary>
            public code.memberInfo Identity;
            /// <summary>
            /// 关键字
            /// </summary>
            public primaryKey[] PrimaryKeys;
            /// <summary>
            /// 获取数据库成员信息集合 
            /// </summary>
            /// <param name="type">数据库绑定类型</param>
            /// <param name="database">数据库配置</param>
            /// <returns>数据库成员信息集合</returns>
            public static keyValue<code.memberInfo, dataMember>[] GetMembers(Type type, fastCSharp.code.memberFilter memberFilter)
            {
                return fastCSharp.code.cSharp.database.GetMembers(code.memberInfo.GetMembers<dataMember>(type, memberFilter.MemberFilter, memberFilter.IsAttribute, memberFilter.IsBaseTypeAttribute, memberFilter.IsInheritAttribute));
            }
        }
    }
}
