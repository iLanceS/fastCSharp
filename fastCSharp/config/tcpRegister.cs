using System;

namespace fastCSharp.config
{
    /// <summary>
    /// TCP注册服务配置
    /// </summary>
    public sealed class tcpRegister
    {
        /// <summary>
        /// TCP注册服务名称
        /// </summary>
        private string serviceName = "fastCSharp.tcpResiter";
        /// <summary>
        /// TCP注册服务名称
        /// </summary>
        public string ServiceName
        {
            get { return string.IsNullOrEmpty(serviceName) ? "fastCSharp.tcpResiter" : serviceName; }
        }
        /// <summary>
        /// TCP注册服务用户名
        /// </summary>
        public string Username;
        /// <summary>
        /// TCP注册服务密码
        /// </summary>
        public string Password;
        /// <summary>
        /// TCP注册服务依赖
        /// </summary>
        public string[] DependedOn;
        /// <summary>
        /// TCP注册服务启动后启动的进程
        /// </summary>
        public string[] OnStartProcesses;
        /// <summary>
        /// TCP服务注册起始端口号
        /// </summary>
        private int portStart = 9000;
        /// <summary>
        /// TCP服务注册起始端口号
        /// </summary>
        public int PortStart
        {
            get { return portStart; }
        }
        /// <summary>
        /// TCP服务注册无响应超时秒数
        /// </summary>
        private int registerTimeoutSeconds = 10;
        /// <summary>
        /// TCP服务注册无响应超时秒数
        /// </summary>
        public int RegisterTimeoutSeconds
        {
            get { return registerTimeoutSeconds; }
        }
        /// <summary>
        /// 日志流数量
        /// </summary>
        private int logStreamSize = 256;
        /// <summary>
        /// 日志流数量
        /// </summary>
        public int LogStreamSize
        {
            get { return logStreamSize; }
        }
        /// <summary>
        /// TCP注册服务配置
        /// </summary>
        private tcpRegister()
        {
            pub.LoadConfig(this);
        }
        /// <summary>
        /// 默认TCP注册服务配置
        /// </summary>
        public static readonly tcpRegister Default = new tcpRegister();
    }
}
