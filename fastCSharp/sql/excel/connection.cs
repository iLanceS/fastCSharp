using System;

namespace fastCSharp.sql.excel
{
    /// <summary>
    /// Excel连接信息
    /// </summary>
    public sealed class connection
    {
        /// <summary>
        /// 混合数据处理方式
        /// </summary>
        public enum intermixed : byte
        {
            /// <summary>
            /// 输出模式，此情况下只能用作写入Excel
            /// </summary>
            Write = 0,
            /// <summary>
            /// 输入模式，此情况下只能用作读取Excel，并且始终将Excel数据作为文本类型读取
            /// </summary>
            Read = 1,
            /// <summary>
            /// 连接模式，此情况下既可用作写入、也可用作读取
            /// </summary>
            WriteAndRead = 2
        }
        /// <summary>
        /// 数据接口属性
        /// </summary>
        public sealed class provider : Attribute
        {
            /// <summary>
            /// 连接名称
            /// </summary>
            public string Name;
            /// <summary>
            /// Excel版本号
            /// </summary>
            public string Excel;
        }
        /// <summary>
        /// Excel接口类型
        /// </summary>
        public enum providerType : byte
        {
            /// <summary>
            /// 未知接口类型
            /// </summary>
            Unknown,
            /// <summary>
            /// 只能操作Excel2007之前的.xls文件
            /// </summary>
            [provider(Name = "Microsoft.Jet.OleDb.4.0", Excel = "Excel 8.0")]
            Jet4,
            /// <summary>
            /// 
            /// </summary>
            [provider(Name = "Microsoft.ACE.OLEDB.12.0", Excel = "Excel 12.0")]
            Ace12,
        }
        /// <summary>
        /// 数据接口属性
        /// </summary>
        public providerType Provider = providerType.Ace12;
        /// <summary>
        /// 数据源
        /// </summary>
        public string DataSource;
        /// <summary>
        /// 密码
        /// </summary>
        public string Password;
        /// <summary>
        /// 混合数据处理方式
        /// </summary>
        public intermixed Intermixed = intermixed.WriteAndRead;
        /// <summary>
        /// 第一行是否列名
        /// </summary>
        public bool IsTitleColumn = true;
        /// <summary>
        /// 获取Excel客户端
        /// </summary>
        /// <returns>Excel客户端</returns>
        public unsafe client GetClient()
        {
            provider provider = Enum<providerType, provider>.Array((byte)Provider);
            pointer buffer = fastCSharp.sql.client.SqlBuffers.Get();
            try
            {
                using (charStream connectionStream = new charStream(buffer.Char, fastCSharp.sql.client.SqlBufferSize))
                {
                    connectionStream.SimpleWriteNotNull("Provider=");
                    connectionStream.Write(provider.Name);
                    connectionStream.SimpleWriteNotNull(";Data Source=");
                    connectionStream.Write(DataSource);
                    if (Password != null)
                    {
                        connectionStream.WriteNotNull(";Database Password=");
                        connectionStream.SimpleWriteNotNull(Password);
                    }
                    connectionStream.WriteNotNull(";Extended Properties='");
                    connectionStream.Write(provider.Excel);
                    connectionStream.WriteNotNull(IsTitleColumn ? ";HDR=YES;IMEX=" : ";HDR=NO;IMEX=");
                    number.ToString((byte)Intermixed, connectionStream);
                    connectionStream.Write('\'');
                    return (client)new sql.connection { Type = type.Excel, Connection = connectionStream.ToString() }.Client;
                }
            }
            finally { fastCSharp.sql.client.SqlBuffers.Push(ref buffer); }
        }
    }
}
