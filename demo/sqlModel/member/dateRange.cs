using System;

namespace fastCSharp.demo.sqlModel.member
{
    /// <summary>
    /// 时间范围（自定义组合字段，映射到数据库表格的多个字段）
    /// </summary>
    [fastCSharp.emit.jsonSerialize(IsAllMember = true)]
    [fastCSharp.emit.jsonParse(IsAllMember = true)]
    [fastCSharp.emit.dataSerialize(IsReferenceMember = false, IsMemberMap = false)]
    [fastCSharp.emit.sqlColumn]
    public struct dateRange : fastCSharp.emit.sqlTable.ISqlVerify
    {
        /// <summary>
        /// 开始日期
        /// </summary>
        public fastCSharp.sql.member.intDate Start;
        /// <summary>
        /// 结束日志
        /// </summary>
        public fastCSharp.sql.member.intDate End;
        /// <summary>
        /// 是否通过SQL数据验证
        /// </summary>
        /// <returns></returns>
        public bool IsSqlVeify()
        {
            return End.Value >= Start.Value;
        }
    }
}
