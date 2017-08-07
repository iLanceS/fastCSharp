using System;
using System.IO;

namespace fastCSharp.config
{
    /// <summary>
    /// 进程复制重启服务配置
    /// </summary>
    public sealed class processCopy
    {
        /// <summary>
        /// 文件监视路径
        /// </summary>
        public string WatcherPath;
        /// <summary>
        /// 文件更新重启检测时间(单位:秒)
        /// </summary>
        private int checkTimeoutSeconds = 5;
        /// <summary>
        /// 文件更新重启检测时间(单位:秒)
        /// </summary>
        public int CheckTimeoutSeconds
        {
            get { return Math.Max(checkTimeoutSeconds, 2); }
        }
        /// <summary>
        /// 文件更新重启复制超时时间(单位:分)
        /// </summary>
        private int copyTimeoutMinutes = 10;
        /// <summary>
        /// 文件更新重启复制超时时间(单位:分)
        /// </summary>
        public int CopyTimeoutMinutes
        {
            get { return Math.Max(copyTimeoutMinutes, 1); }
        }
        /// <summary>
        /// 进程复制重启失败最大休眠秒数
        /// </summary>
        private int maxThreadSeconds = 10;
        /// <summary>
        /// 进程复制重启失败最大休眠秒数
        /// </summary>
        public int MaxThreadSeconds
        {
            get { return Math.Max(maxThreadSeconds, 2); }
        }
        /// <summary>
        /// 进程复制重启服务配置
        /// </summary>
        private processCopy()
        {
            pub.LoadConfig(this);
            if (WatcherPath != null)
            {
                try
                {
                    DirectoryInfo fileWatcherDirectory = new DirectoryInfo(WatcherPath);
                    if (fileWatcherDirectory.Exists) WatcherPath = fileWatcherDirectory.fullName().fileNameToLower();
                    else
                    {
                        WatcherPath = null;
                        log.Error.Add("没有找到文件监视路径 " + WatcherPath, new System.Diagnostics.StackFrame(), false);
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, WatcherPath, false);
                    WatcherPath = null;
                }
            }
        }
        /// <summary>
        /// 默认进程复制重启服务配置
        /// </summary>
        public static readonly processCopy Default = new processCopy();
    }
}
