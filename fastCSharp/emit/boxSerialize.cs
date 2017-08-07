using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fastCSharp.emit
{
    /// <summary>
    /// 值类型序列化包装处理
    /// </summary>
    public sealed class boxSerialize : Attribute
    {
        ///// <summary>
        ///// 二进制数据序列化
        ///// </summary>
        //public bool IsData = true;
        /// <summary>
        /// JSON序列化
        /// </summary>
        public bool IsJson = true;
        /// <summary>
        /// XML序列化
        /// </summary>
        public bool IsXml = true;
    }
}
