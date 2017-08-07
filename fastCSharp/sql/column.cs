using System;
using System.Data;
using fastCSharp.code;
using fastCSharp.code.cSharp;

namespace fastCSharp.sql
{
    /// <summary>
    /// 数据列
    /// </summary>
    internal sealed class column
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string Name;
        /// <summary>
        /// SQL名称
        /// </summary>
        public string SqlName
        {
            get
            {
                return fastCSharp.code.cSharp.sqlModel.ToSqlName(Name);
            }
        }
        /// <summary>
        /// 备注说明
        /// </summary>
        public string Remark;
        /// <summary>
        /// 数据库类型
        /// </summary>
        public SqlDbType DbType;
        /// <summary>
        /// 表格列类型
        /// </summary>
        public Type SqlColumnType;
        /// <summary>
        /// 列长
        /// </summary>
        public int Size;
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue;
        /// <summary>
        /// 新增字段时的计算子查询
        /// </summary>
        public string UpdateValue;
        /// <summary>
        /// 是否允许为空
        /// </summary>
        public bool IsNull;

        /// <summary>
        /// 判断是否匹配数据列
        /// </summary>
        /// <param name="value">数据列</param>
        /// <param name="isIgnoreCase">是否忽略大小写</param>
        /// <returns>是否匹配</returns>
        internal bool IsMatch(column value, bool isIgnoreCase)
        {
            return value != null && (isIgnoreCase ? Name.ToLower() == value.Name.ToLower() : Name == value.Name) && DbType == value.DbType && Size == value.Size && IsNull == value.IsNull;
        }
    }
}
