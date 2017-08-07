using System;
using System.Text;

namespace fastCSharp
{
    /// <summary>
    /// 编码
    /// </summary>
    public static class encoding
    {
        /// <summary>
        /// gb2312编码
        /// </summary>
        public static readonly Encoding Gb2312 = Encoding.GetEncoding("gb2312");
        /// <summary>
        /// gb18030编码
        /// </summary>
        public static readonly Encoding Gb18030 = Encoding.GetEncoding("gb18030");
        /// <summary>
        /// gbk编码
        /// </summary>
        public static readonly Encoding Gbk = Encoding.GetEncoding("gbk");
        /// <summary>
        /// big5编码
        /// </summary>
        public static readonly Encoding Big5 = Encoding.GetEncoding("big5");
    }
}
