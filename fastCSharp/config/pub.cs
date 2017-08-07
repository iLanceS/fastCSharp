using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using fastCSharp.reflection;
using fastCSharp.code.cSharp;
using System.Threading;
using fastCSharp;
using System.Runtime.CompilerServices;

namespace fastCSharp.config
{
    /// <summary>
    /// 基本配置
    /// </summary>
    public sealed class pub
    {
        /// <summary>
        /// 程序工作主目录
        /// </summary>
        public string WorkPath { get; private set; }
        /// <summary>
        /// 缓存文件主目录
        /// </summary>
        public string CachePath { get; private set; }
        /// <summary>
        /// 是否调试模式
        /// </summary>
        public bool IsDebug { get; private set; }
        /// <summary>
        /// 是否window服务模式
        /// </summary>
        public bool IsService { get; private set; }

        /// <summary>
        /// 默认分页大小
        /// </summary>
        private int pageSize = 10;
        /// <summary>
        /// 默认分页大小
        /// </summary>
        public int PageSize
        {
            get { return pageSize; }
        }
        /// <summary>
        /// 默认分页大小
        /// </summary>
        private int maxEnumArraySize = 1024;
        /// <summary>
        /// 最大枚举数组数量
        /// </summary>
        public int MaxEnumArraySize
        {
            get { return maxEnumArraySize; }
        }
        /// <summary>
        /// 默认任务线程数
        /// </summary>
        private int taskThreadCount = fastCSharp.pub.CpuCount << 4;
        /// <summary>
        /// 默认任务线程数
        /// </summary>
        public int TaskThreadCount
        {
            get
            {
                if (taskThreadCount > taskMaxThreadCount)
                {
                    log.Error.Add("默认任务线程数[" + taskThreadCount.toString() + "] 超出 任务最大线程数[" + taskMaxThreadCount.toString() + "]", new System.Diagnostics.StackFrame(), false);
                    return taskMaxThreadCount;
                }
                return taskThreadCount;
            }
        }
        /// <summary>
        /// 任务最大线程数
        /// </summary>
        private int taskMaxThreadCount = 65536;
        /// <summary>
        /// 任务最大线程数
        /// </summary>
        public int TaskMaxThreadCount
        {
            get { return taskMaxThreadCount; }
        }
        /// <summary>
        /// 死锁检测分钟数,0表示不检测
        /// </summary>
        private int lockCheckMinutes = 0;
        /// <summary>
        /// 死锁检测分钟数,0表示不检测
        /// </summary>
        public int LockCheckMinutes
        {
            get { return lockCheckMinutes; }
        }
        /// <summary>
        /// 微型线程任务线程数
        /// </summary>
        private int tinyThreadCount = fastCSharp.pub.CpuCount << 6;
        /// <summary>
        /// 微型线程任务线程数
        /// </summary>
        public int TinyThreadCount
        {
            get
            {
                if (tinyThreadCount > taskMaxThreadCount)
                {
                    log.Error.Add("微型线程任务线程数[" + tinyThreadCount.toString() + "] 超出 任务最大线程数[" + taskMaxThreadCount.toString() + "]", new System.Diagnostics.StackFrame(), false);
                    return taskMaxThreadCount;
                }
                return tinyThreadCount;
            }
        }
        /// <summary>
        /// 原始套接字监听缓冲区尺寸(单位:KB)
        /// </summary>
        private int rawSocketBufferSize = 1024;
        /// <summary>
        /// 原始套接字监听缓冲区尺寸(单位:B)
        /// </summary>
        public int RawSocketBufferSize
        {
            get { return Math.Max(1024 << 10, rawSocketBufferSize << 10); }
        }
        /// <summary>
        /// 服务器端套接字单次最大发送数据量(单位:KB)
        /// </summary>
        private int maxServerSocketSendSize = 8;
        /// <summary>
        /// 服务器端套接字单次最大发送数据量(单位:B)
        /// </summary>
        public int MaxServerSocketSendSize
        {
            get { return Math.Max(4 << 10, maxServerSocketSendSize << 10); }
        }
        /// <summary>
        /// Emit名称申请池大小(单位:KB)
        /// </summary>
        private int emitNamePoolSize = 8;
        /// <summary>
        /// Emit名称申请池大小(单位:B)
        /// </summary>
        public int EmitNamePoolSize
        {
            get { return Math.Max(4 << 10, emitNamePoolSize << 10); }
        }
        /// <summary>
        /// 垃圾定时清理定时器触发时间间隔
        /// </summary>
        private int disposeTimerInterval = 1000;
        /// <summary>
        /// 垃圾定时清理定时器触发时间间隔
        /// </summary>
        public double DisposeTimerInterval
        {
            get { return Math.Min(Math.Max(disposeTimerInterval, 1), int.MaxValue); }
        }
        /// <summary>
        /// 基本配置
        /// </summary>
        private pub()
        {
            LoadConfig(this);
            if (WorkPath == null) WorkPath = fastCSharp.pub.ApplicationPath;
            else WorkPath = WorkPath.pathSuffix().fileNameToLower();
            if (CachePath == null || !directory.Create(CachePath = CachePath.pathSuffix().fileNameToLower())) CachePath = fastCSharp.pub.ApplicationPath;
        }
        ///// <summary>
        ///// 获取配置
        ///// </summary>
        ///// <param name="type">配置类型</param>
        ///// <returns>配置</returns>
        //private json.node getConfig(Type type)
        //{
        //    if (configs.Type != json.node.nodeType.Null)
        //    {
        //        string name = type.FullName, fastCSharpName = fastCSharp.pub.fastCSharp + ".";
        //        if (name.StartsWith(fastCSharpName, StringComparison.Ordinal))
        //        {
        //            string fastCSharpConfig = fastCSharpName + "config.";
        //            name = name.Substring(name.StartsWith(fastCSharpConfig, StringComparison.Ordinal) ? fastCSharpConfig.Length : fastCSharpName.Length);
        //        }
        //        json.node config = configs;
        //        foreach (string tagName in name.Split('.'))
        //        {
        //            if (config.Type != json.node.nodeType.Dictionary || (config = config[tagName]).Type == json.node.nodeType.Null)
        //            {
        //                return default(json.node);
        //            }
        //        }
        //        return config;
        //    }
        //    return default(json.node);
        //}
        ///// <summary>
        ///// 配置加载
        ///// </summary>
        ///// <param name="value">配置对象</param>
        ///// <param name="name">配置名称,null表示只匹配类型</param>
        //public valueType LoadConfig<valueType>(string name = null) where valueType : struct
        //{
        //    valueType value = default(valueType);
        //    return loadConfig(ref value, name);
        //}
        /// <summary>
        /// fastCSharp命名空间
        /// </summary>
        private const string fastCSharpName = fastCSharp.pub.fastCSharp + ".";
        /// <summary>
        /// fastCSharp配置命名空间
        /// </summary>
        private const string fastCSharpConfigName = fastCSharpName + "config.";
        /// <summary>
        /// 配置JSON解析参数
        /// </summary>
        private static readonly fastCSharp.emit.jsonParser.config jsonConfig = new fastCSharp.emit.jsonParser.config { MemberFilter = code.memberFilters.Instance };
        /// <summary>
        /// 配置加载
        /// </summary>
        /// <param name="value">配置对象</param>
        /// <param name="name">配置名称,null表示只匹配类型</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType LoadConfig<valueType>(valueType value, string name = null) where valueType : class
        {
            if (value == null) log.Error.Throw(log.exceptionType.Null);
            loadConfig(ref value, name);
            return value;
        }
        /// <summary>
        /// 配置加载
        /// </summary>
        /// <param name="value">配置对象</param>
        /// <param name="name">配置名称,null表示只匹配类型</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static void LoadConfig<valueType>(ref valueType value, string name = null) where valueType : struct
        {
            loadConfig(ref value, name);
        }
        /// <summary>
        /// 配置加载
        /// </summary>
        /// <param name="value">配置对象</param>
        /// <param name="name">配置名称,null表示只匹配类型</param>
        private static void loadConfig<valueType>(ref valueType value, string name)
        {
            if (name == null) name = value.GetType().FullName.replaceNotNull('+', '.');
            else name = value.GetType().FullName.replaceNotNull('+', '.') + "." + name;
            if (name.StartsWith(fastCSharpName, StringComparison.Ordinal))
            {
                name = name.Substring(name.StartsWith(fastCSharpConfigName, StringComparison.Ordinal) ? fastCSharpConfigName.Length : fastCSharpName.Length);
            }
            subString json;
            if (configs.TryGetValue(name, out json)) fastCSharp.emit.jsonParser.Parse(ref json, ref value, jsonConfig);
        }
        /// <summary>
        /// 判断配置名称是否存在
        /// </summary>
        /// <param name="name">配置名称</param>
        /// <returns>配置是否加载存在</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static bool IsConfigName(string name)
        {
            return configs.ContainsKey(name);
        }
        /// <summary>
        /// 配置加载
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="fullName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static valueType LoadConfigName<valueType>(string fullName, valueType defaultValue = default(valueType))
        {
            subString json;
            if (configs.TryGetValue(fullName, out json))
            {
                valueType value = default(valueType);
                fastCSharp.emit.jsonParser.Parse(ref json, ref value, jsonConfig);
                return value;
            }
            return defaultValue;
        }
        /// <summary>
        /// 配置加载
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="value"></param>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public static bool LoadConfigName<valueType>(ref valueType value, string fullName)
        {
            subString json;
            return configs.TryGetValue(fullName, out json) && fastCSharp.emit.jsonParser.Parse(ref json, ref value, jsonConfig);
        }
        /// <summary>
        /// 配置文件加载
        /// </summary>
        private struct loader
        {
            /// <summary>
            /// 历史配置文件
            /// </summary>
            private list<string> files;
            /// <summary>
            /// 错误信息集合
            /// </summary>
            private list<string> errors;
            /// <summary>
            /// 加载配置文件
            /// </summary>
            /// <param name="configFile">配置文件名称</param>
            public void Load(string configFile)
            {
                if (files == null)
                {
                    files = new list<string>();
                    errors = new list<string>();
                }
                try
                {
                    load(new FileInfo(configFile));
                }
                catch (Exception error)
                {
                    log.Error.Real(error, "配置文件加载失败 : " + configFile, false);
                }
            }
            /// <summary>
            /// 加载配置文件
            /// </summary>
            /// <param name="file">配置文件</param>
            private unsafe void load(FileInfo file)
            {
                if (file.Exists)
                {
                    string fileName = file.FullName.fileNameToLower();
                    int count = files.length;
                    if (count != 0)
                    {
                        foreach (string name in files.array)
                        {
                            if (errors.length == 0)
                            {
                                if (name == fileName)
                                {
                                    errors.Add("配置文件循环嵌套");
                                    errors.Add(name);
                                }
                            }
                            else errors.Add(name);
                            if (--count == 0) break;
                        }
                        if (errors.length != 0)
                        {
                            log.Error.Real(errors.joinString(@"
"), new System.Diagnostics.StackFrame(), false);
                            errors.Empty();
                        }
                    }
                    string config = File.ReadAllText(fileName, appSetting.Encoding);
                    fixed (char* configFixed = config)
                    {
                        for (char* current = configFixed, end = configFixed + config.Length; current != end; )
                        {
                            char* start = current;
                            while (*current != '=' && ++current != end) ;
                            if (current == end) break;
                            subString name = subString.Unsafe(config, (int)(start - configFixed), (int)(current - start));
                            if (name.Equals(appSetting.ConfigIncludeName))
                            {
                                for (start = ++current; current != end && *current != '\n'; ++current) ;
                                Load(Path.Combine(file.DirectoryName, config.Substring((int)(start - configFixed), (int)(current - start)).Trim()));
                                if (current == end) break;
                                ++current;
                            }
                            else
                            {
                                for (start = ++current; current != end; ++current)
                                {
                                    if (*current == '\n')
                                    {
                                        while (++current != end && *current == '\n') ;
                                        if (current == end) break;
                                        if (*current != '\t' && *current != ' ') break;
                                    }
                                }
                                hashString nameKey = name;
                                if (configs.ContainsKey(nameKey))
                                {
                                    log.Error.Real("重复的配置名称 : " + name.ToString(), new System.Diagnostics.StackFrame(), false);
                                }
                                else configs.Add(nameKey, subString.Unsafe(config, (int)(start - configFixed), (int)(current - start)));
                            }
                        }
                    }
                }
                else log.Default.Real("找不到配置文件 : " + file.FullName, new System.Diagnostics.StackFrame(), false);
            }
        }
        /// <summary>
        /// 配置集合
        /// </summary>
        private static readonly Dictionary<hashString, subString> configs;
        /// <summary>
        /// 默认基本配置
        /// </summary>
        public static readonly pub Default;
        static unsafe pub()
        {
            configs = dictionary.CreateHashString<subString>();
            new loader().Load(appSetting.ConfigFile);
            Default = new pub();
        }
    }
}
