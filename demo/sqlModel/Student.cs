using System;

namespace fastCSharp.demo.sqlModel
{
    /// <summary>
    /// 学生数据定义
    /// </summary>
    [fastCSharp.code.cSharp.webView.clientType(PrefixName = pub.WebViewClientTypePrefixName)]
    [fastCSharp.code.cSharp.sqlModel(CacheType = fastCSharp.code.cSharp.sqlModel.cacheType.IdentityArray, LogTcpCallService = pub.LogTcpCallService)]
    public partial class Student
    {
        /// <summary>
        /// 学生标识（默认自增）
        /// </summary>
        [fastCSharp.code.cSharp.webView.outputAjax]
        public int Id;
        /// <summary>
        /// 电子邮箱（关键字）
        /// </summary>
        [fastCSharp.emit.dataMember(PrimaryKeyIndex = 1, MaxStringLength = 256, IsAscii = true)]
        public string Email;
        /// <summary>
        /// 密码
        /// </summary>
        [fastCSharp.emit.dataMember(MaxStringLength = 32)]
        public string Password;
        /// <summary>
        /// 头像
        /// </summary>
        [fastCSharp.emit.dataMember(MaxStringLength = 256, IsAscii = true)]
        [fastCSharp.code.cSharp.webView.outputAjax(BindingName = "Gender")]
        public string Image = string.Empty;
        /// <summary>
        /// 姓名
        /// </summary>
        [fastCSharp.emit.dataMember(MaxStringLength = 64)]
        public string Name;
        /// <summary>
        /// 出生日期（自定义字段）
        /// </summary>
        public fastCSharp.sql.member.intDate Birthday;
        /// <summary>
        /// 性别（自定义 enum 字段）
        /// </summary>
        [fastCSharp.emit.dataMember(DataType = typeof(byte))]
        public member.gender Gender;
        /// <summary>
        /// 按加入时间排序的班级集合（不可识别的字段映射为 JSON 字符串）
        /// </summary>
        [fastCSharp.emit.dataMember(MaxStringLength = 256, IsAscii = true, IsMemberIndex = true)]
        public member.classDate[] Classes;
        /// <summary>
        /// 当前班级标识
        /// </summary>
        public int ClassId
        {
            get
            {
                if (Classes == null) return 0;
                return Classes[Classes.Length - 1].ClassId;
            }
        }
    }
}
