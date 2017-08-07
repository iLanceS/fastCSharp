using System;

namespace fastCSharp.data
{
    /// <summary>
    /// 数据源
    /// </summary>
    [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
    internal sealed class dataSource
    {
        /// <summary>
        /// 数据流
        /// </summary>
        public byte[] Data;
        /// <summary>
        /// 字符串集合
        /// </summary>
        public string[] Strings;
        /// <summary>
        /// 字节数组集合
        /// </summary>
        public byte[][] Bytes;
    }
}
