using System;

namespace fastCSharp.demo.loadBalancingTcpCommandWeb
{
    /// <summary>
    /// 网站生成配置
    /// </summary>
    internal sealed class webConfig : fastCSharp.code.webConfig
    {
        /// <summary>
        /// 测试配置
        /// </summary>
        public sealed class config
        {
            /// <summary>
            /// 域名
            /// </summary>
            public string Domain;
            /// <summary>
            /// 每CPU客户端数量
            /// </summary>
            public int ClientCountPerCPU = 256;
            /// <summary>
            /// 是否长连接
            /// </summary>
            public bool IsKeepAlive;
            /// <summary>
            /// 是否客户端
            /// </summary>
            public bool IsClient;
            /// <summary>
            /// 是否自动启动客户端
            /// </summary>
            public bool IsStartClient;
            /// <summary>
            /// 测试配置
            /// </summary>
            public static readonly config Default = fastCSharp.config.pub.LoadConfig(new config());
        }
        /// <summary>
        /// 默认主域名
        /// </summary>
        public override string MainDomain
        {
            get { return config.Default.Domain; }
        }
        /// <summary>
        /// 是否复制js脚本文件
        /// </summary>
        public override bool IsCopyScript
        {
            get { return false; }
        }
    }
}
