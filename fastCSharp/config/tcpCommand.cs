using System;

namespace fastCSharp.config
{
    /// <summary>
    /// TCP调用配置
    /// </summary>
    public sealed class tcpCommand
    {
        /// <summary>
        /// 命令套接字默认超时秒数
        /// </summary>
        private int defaultTimeout = 60;
        /// <summary>
        /// 命令套接字默认超时秒数
        /// </summary>
        public int DefaultTimeout
        {
            get { return defaultTimeout; }
        }
        /// <summary>
        /// 命令套接字大数据缓存字节数(单位:KB)
        /// </summary>
        private int bigBufferSize = 128;
        /// <summary>
        /// 命令套接字大数据缓存字节数
        /// </summary>
        public int BigBufferSize
        {
            get { return Math.Max(bigBufferSize << 10, fastCSharp.config.appSetting.StreamBufferSize); }
        }
        /// <summary>
        /// 命令套接字异步缓存字节数(单位:KB)
        /// </summary>
        private int asyncBufferSize = 0;
        /// <summary>
        /// 命令套接字异步缓存字节数
        /// </summary>
        public int AsyncBufferSize
        {
            get { return Math.Max(asyncBufferSize << 10, fastCSharp.config.appSetting.StreamBufferSize); }
        }
        /// <summary>
        /// 命令套接字客户端标识验证超时秒数
        /// </summary>
        private int clientVerifyTimeout = 15;
        /// <summary>
        /// 命令套接字客户端标识验证超时秒数
        /// </summary>
        public int ClientVerifyTimeout
        {
            get { return clientVerifyTimeout <= 0 ? 15 : clientVerifyTimeout; }
        }
        /// <summary>
        /// TCP流超时秒数
        /// </summary>
        private int tcpStreamTimeout = 60;
        /// <summary>
        /// TCP流超时秒数
        /// </summary>
        public int TcpStreamTimeout
        {
            get { return tcpStreamTimeout <= 0 ? 60 : tcpStreamTimeout; }
        }
        /// <summary>
        /// 服务器端最大批量发送数量
        /// </summary>
        private int maxServerSendCount = 1024;
        /// <summary>
        /// 服务器端最大批量发送数量
        /// </summary>
        public int MaxServerSendCount
        {
            get { return maxServerSendCount <= 0 ? 1024 : maxServerSendCount; }
        }
        /// <summary>
        /// 客户端最大批量发送数量
        /// </summary>
        private int maxClientSendCount = 1024;
        /// <summary>
        /// 客户端最大批量发送数量
        /// </summary>
        public int MaxClientSendCount
        {
            get { return maxClientSendCount <= 0 ? 1024 : maxClientSendCount; }
        }
        /// <summary>
        /// TCP调用配置
        /// </summary>
        private tcpCommand()
        {
            pub.LoadConfig(this);
        }
        /// <summary>
        /// 默认TCP调用配置
        /// </summary>
        public static readonly tcpCommand Default = new tcpCommand();
    }
}
