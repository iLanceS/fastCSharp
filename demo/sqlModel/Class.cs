using System;

namespace fastCSharp.demo.sqlModel
{
    /// <summary>
    /// 班级数据定义
    /// </summary>
    [fastCSharp.code.cSharp.webView.clientType(PrefixName = pub.WebViewClientTypePrefixName)]
    [fastCSharp.code.cSharp.sqlModel(CacheType = fastCSharp.code.cSharp.sqlModel.cacheType.CreateIdentityArray, IsMemberCache = true, LogTcpCallService = pub.LogTcpCallService)]
    public partial class Class
    {
        /// <summary>
        /// 班级标识（默认自增）
        /// </summary>
        [fastCSharp.code.cSharp.webView.outputAjax]
        public int Id;
        /// <summary>
        /// 班级名称
        /// </summary>
        [fastCSharp.emit.dataMember(MaxStringLength = 32)]
        public string Name;
        /// <summary>
        /// 开始日期+结束日期（自定义组合字段，映射到数据库表格的多个字段）
        /// </summary>
        public member.dateRange DateRange;
        /// <summary>
        /// 专业（自定义 enum 字段）
        /// </summary>
        public member.discipline Discipline;

        #region 计算列（不映射到数据库表格，由缓存数据实时计算结果）
        /// <summary>
        /// 当前学生数量
        /// </summary>
        [fastCSharp.emit.dataMember(IsIgnoreCurrent = true, SqlStreamCountType = typeof(Student))]
        public int StudentCount;
        #endregion
    }
}
