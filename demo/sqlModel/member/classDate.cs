using System;

namespace fastCSharp.demo.sqlModel.member
{
    /// <summary>
    /// 班级+日期
    /// </summary>
    [fastCSharp.emit.jsonSerialize(IsAllMember = true)]
    [fastCSharp.emit.jsonParse(IsAllMember = true)]
    [fastCSharp.emit.dataSerialize(IsReferenceMember = false, IsMemberMap = false)]
    public struct classDate
    {
        /// <summary>
        /// 班级标识
        /// </summary>
        public int ClassId;
        /// <summary>
        /// 日期
        /// </summary>
        public fastCSharp.sql.member.intDate Date;
    }
}
