using System;

namespace fastCSharp.sql.member
{
    /// <summary>
    /// 整形日期
    /// </summary>
    [fastCSharp.emit.boxSerialize]
    [fastCSharp.emit.jsonSerialize(IsAllMember = true, Filter = code.memberFilters.InstanceField)]
    [fastCSharp.emit.jsonParse(IsAllMember = true)]
    [fastCSharp.emit.dataSerialize(IsReferenceMember = false, IsMemberMap = false)]
    [fastCSharp.emit.dataMember(DataType = typeof(int))]
    public partial struct intDate// : fastCSharp.emit.sqlTable.ISqlString
    {
        /// <summary>
        /// 整形日期值
        /// </summary>
        [fastCSharp.code.cSharp.webView.outputAjax]
        public int Value;
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator intDate(int value) { return new intDate { Value = value }; }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator int(intDate value) { return value.Value; }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator intDate(DateTime value) { return new intDate { Value = value.toInt() }; }
        /// <summary>
        /// 日期时间值
        /// </summary>
        public DateTime DateTime
        {
            get { return date.GetDate(Value); }
        }
        ///// <summary>
        ///// 转换SQL字符串
        ///// </summary>
        ///// <returns></returns>
        //public string ToSqlString()
        //{
        //    return Value.toString();
        //}
    }
}
