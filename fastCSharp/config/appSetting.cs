using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
#if XAMARIN
#else
using System.Configuration;
#endif

namespace fastCSharp.config
{
    /// <summary>
    /// .NET配置文件
    /// </summary>
    public static class appSetting
    {
        /// <summary>
        /// 默认配置文件
        /// </summary>
        private const string defaultConfigFile = fastCSharp.pub.fastCSharp + ".config";
        /// <summary>
        /// fastCSharp配置文件嵌套名称
        /// </summary>
        private const string defaultConfigIncludeName = "@";
        /// <summary>
        /// .NET配置
        /// </summary>
        private static readonly NameValueCollection settings;
        /// <summary>
        /// 获取.NET配置内容
        /// </summary>
        /// <param name="name">配置名称</param>
        /// <returns></returns>
        public static string Get(string name)
        {
            return settings[name];
        }
        /// <summary>
        /// fastCSharp配置文件
        /// </summary>
        public static readonly string ConfigFile;
        /// <summary>
        /// fastCSharp配置文件嵌套名称
        /// </summary>
        public static readonly string ConfigIncludeName;
        /// <summary>
        /// 配置文件路径
        /// </summary>
        public static readonly string ConfigPath;
        /// <summary>
        /// 日志文件主目录
        /// </summary>
        public static readonly string LogPath;
        /// <summary>
        /// 日志文件默认最大字节数
        /// </summary>
        public static readonly int MaxLogSize;
        /// <summary>
        /// 最大缓存日志数量
        /// </summary>
        public static readonly int MaxLogCacheCount;
        /// <summary>
        /// 默认日志是否重定向到控制台
        /// </summary>
        public static readonly bool IsLogConsole;
        /// <summary>
        /// 是否创建单独的错误日志文件
        /// </summary>
        public static readonly bool IsErrorLog;
        /// <summary>
        /// 序列化/循环检测最大深度
        /// </summary>
        public static readonly int SerializeDepth;
        /// <summary>
        /// 全局默认编码
        /// </summary>
        public static readonly Encoding Encoding;
        /// <summary>
        /// 默认微型线程池线程堆栈大小
        /// </summary>
        public static readonly int TinyThreadStackSize;
        /// <summary>
        /// 默认线程池线程堆栈大小
        /// </summary>
        public static readonly int ThreadStackSize;
        /// <summary>
        /// 对象池初始大小
        /// </summary>
        public static readonly int PoolSize;
        ///// <summary>
        ///// 对象池是否采用纠错模式
        ///// </summary>
        //public static readonly bool IsPoolDebug;
        /// <summary>
        /// 流缓冲区字节尺寸
        /// </summary>
        public static readonly int StreamBufferSize;
        /// <summary>
        /// 是否默认添加内存检测类型
        /// </summary>
        public static readonly bool IsCheckMemory;
        /// <summary>
        /// 成员位图内存池字节数
        /// </summary>
        public static int MemberMapPoolSize;
        /// <summary>
        /// 成员位图内存池支持最大成员数量
        /// </summary>
        public static int MaxMemberMapCount;
#if __ANDROID__
        /// <summary>
        /// 获取日志文件目录
        /// </summary>
        /// <returns></returns>
        private static string getLogPath()
        {
            Android.Content.Context context = Android.App.Application.Context;
            try
            {
                foreach (Java.IO.File file in context.GetExternalCacheDirs())
                {
                    if (file.CanWrite()) return new DirectoryInfo(file.AbsolutePath).fullName();
                }
            }
            catch { }
            {
                Java.IO.File file = context.GetExternalFilesDir(null);
                if (file != null && file.CanWrite()) new DirectoryInfo(file.AbsolutePath).fullName();
            }
            try
            {
                foreach (Java.IO.File file in context.GetExternalFilesDirs(null))
                {
                    if (file.CanWrite()) return new DirectoryInfo(file.AbsolutePath).fullName();
                }
            }
            catch { }
            return null;
        }
#endif
        static appSetting()
        {
#if XAMARIN
            settings = new NameValueCollection();
            MaxLogSize = 0;
            ConfigPath = fastCSharp.pub.ApplicationPath;
            ConfigFile = ConfigPath + defaultConfigFile;
            ConfigIncludeName = defaultConfigIncludeName;
            MaxLogCacheCount = 1 << 6;
            SerializeDepth = 64;
            Encoding = Encoding.UTF8;
            TinyThreadStackSize = 128 << 10;
            ThreadStackSize = 1 << 20;
            PoolSize = 4;
            StreamBufferSize = 4 << 10;
            MemberMapPoolSize = 8 << 10;
            MaxMemberMapCount = 1024;
#if __ANDROID__
            LogPath = getLogPath() ?? fastCSharp.pub.ApplicationPath;
#else
#if __IOS__
            LogPath = fastCSharp.pub.ApplicationPath;
#else
            LogPath = fastCSharp.pub.ApplicationPath;
#endif
#endif
#else
            try
            {
                settings = ConfigurationManager.AppSettings;

                ConfigFile = settings["configFile"];
                if (ConfigFile == null)
                {
                    ConfigFile = (ConfigPath = fastCSharp.pub.ApplicationPath) + defaultConfigFile;
                }
                else if (ConfigFile.IndexOf(':') == -1)
                {
                    ConfigFile = (ConfigPath = fastCSharp.pub.ApplicationPath) + ConfigFile.pathSeparator();
                }
                else ConfigPath = new FileInfo(ConfigFile.pathSeparator()).Directory.fullName().fileNameToLower();

                ConfigIncludeName = settings["configIncludeName"];
                if (ConfigIncludeName == null) ConfigIncludeName = defaultConfigIncludeName;

                LogPath = settings["logPath"];
                if (LogPath == null || !directory.Create(LogPath = LogPath.pathSuffix().toLower())) LogPath = fastCSharp.pub.ApplicationPath;

                string maxLogSize = settings["maxLogSize"];
                if (!int.TryParse(maxLogSize, out MaxLogSize)) MaxLogSize = 1 << 20;

                string maxLogCacheCount = settings["maxLogCacheCount"];
                if (!int.TryParse(maxLogCacheCount, out MaxLogCacheCount)) MaxLogCacheCount = 1 << 10;

                string jsonParseDepth = settings["serializeDepth"];
                if (!int.TryParse(jsonParseDepth, out SerializeDepth)) SerializeDepth = 64;

                if (settings["isLogConsole"] != null) IsLogConsole = true;
                if (settings["isErrorLog"] != null) IsErrorLog = true;

                string encoding = settings["encoding"];
                if (encoding != null)
                {
                    try
                    {
                        Encoding = Encoding.GetEncoding(encoding);
                    }
                    catch (Exception error)
                    {
                        Console.WriteLine(error.ToString());
                        Encoding = Encoding.UTF8;
                    }
                }
                if (Encoding == null) Encoding = Encoding.UTF8;

                string tinyThreadStackSize = settings["tinyThreadStackSize"];
                if (!int.TryParse(tinyThreadStackSize, out TinyThreadStackSize)) TinyThreadStackSize = 128 << 10;

                string threadStackSize = settings["threadStackSize"];
                if (!int.TryParse(threadStackSize, out ThreadStackSize)) ThreadStackSize = 1 << 20;

                string poolSize = settings["poolSize"];
                if (!int.TryParse(poolSize, out PoolSize)) PoolSize = 4;

                //if (settings["isPoolDebug"] != null) IsPoolDebug = true;

                string streamBufferSize = settings["streamBufferSize"];
                if (!int.TryParse(streamBufferSize, out StreamBufferSize)) StreamBufferSize = 4 << 10;

                string memberMapPoolSize = settings["memberMapPoolSize"];
                if (!int.TryParse(memberMapPoolSize, out MemberMapPoolSize)) MemberMapPoolSize = 8 << 10;

                string maxMemberMapCount = settings["maxMemberMapCount"];
                if (int.TryParse(maxMemberMapCount, out MaxMemberMapCount)) MaxMemberMapCount = (Math.Max(1024, MaxMemberMapCount) + 63) & 0x7fffffc0;
                else MaxMemberMapCount = 1024;

                if (settings["isCheckMemory"] != null) IsCheckMemory = true;
            }
            catch (Exception error)
            {
                settings = new NameValueCollection();
                ConfigPath = LogPath = fastCSharp.pub.ApplicationPath;
                MaxLogSize = 1 << 20;
                ConfigFile = ConfigPath + defaultConfigFile;
                ConfigIncludeName = defaultConfigIncludeName;
                MaxLogCacheCount = 1 << 10;
                SerializeDepth = 64;
                Encoding = Encoding.UTF8;
                TinyThreadStackSize = 128 << 10;
                ThreadStackSize = 1 << 20;
                PoolSize = 4;
                StreamBufferSize = 4 << 10;
                MemberMapPoolSize = 8 << 10;
                MaxMemberMapCount = 1024;
                Console.WriteLine(error.ToString());
            }
#endif
        }
    }
}
