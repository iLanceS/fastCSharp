using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting;
using fastCSharp.threading;
using fastCSharp.reflection;

namespace fastCSharp
{
    /// <summary>
    /// 动态应用程序域
    /// </summary>
    [Serializable]
    public sealed class dynamicDomain : IDisposable
    {
        /// <summary>
        /// 程序集加载器
        /// </summary>
        private sealed class assemblyLoader : MarshalByRefObject
        {
            /// <summary>
            /// 已加载程序集
            /// </summary>
            [NonSerialized]
            private Dictionary<hashString, Assembly> assemblys = dictionary.CreateHashString<Assembly>();
            /// <summary>
            /// 程序集加载器
            /// </summary>
            public assemblyLoader() { }
            /// <summary>
            /// 加载程序集
            /// </summary>
            /// <param name="assemblyFileName">程序集文件名</param>
            public Assembly Load(string assemblyFileName)
            {
                if (!string.IsNullOrEmpty(assemblyFileName))
                {
                    Assembly assembly;
                    hashString nameKey = assemblyFileName;
                    if (assemblys.TryGetValue(nameKey, out assembly)) return assembly;
                    try
                    {
                        if (assemblyFileName.Length > 4 && assemblyFileName.EndsWith(".dll"))
                        {
                            assembly = Assembly.LoadFrom(assemblyFileName.Substring(0, assemblyFileName.Length - 4));
                        }
                        else assembly = Assembly.LoadFrom(assemblyFileName);
                        assemblys[nameKey] = assembly;
                        return assembly;
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, "动态应用程序域加载程序集 " + assemblyFileName + " 失败", false);
                    }
                }
                return null;
            }
            /// <summary>
            /// 加载程序集获取类型
            /// </summary>
            /// <param name="assemblyFileName">程序集文件名</param>
            /// <param name="typeName">类型名称</param>
            /// <returns>类型</returns>
            public Type LoadType(string assemblyFileName, string typeName)
            {
                Assembly assembly = Load(assemblyFileName);
                return assembly != null ? assembly.GetType(typeName) : null;
            }
            /// <summary>
            /// 加载程序集并创建对象
            /// </summary>
            /// <param name="assemblyFileName">程序集文件名</param>
            /// <param name="typeName">对象类型名称</param>
            /// <returns>创建的对象,失败返回null</returns>
            public object CreateInstance(string assemblyFileName, string typeName)
            {
                Type type = LoadType(assemblyFileName, typeName);
                if (type != null)
                {
                    try
                    {
                        return Activator.CreateInstance(type);
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, null, false);
                    }
                }
                return null;
            }
            /// <summary>
            /// 加载程序集并创建包装对象
            /// </summary>
            /// <param name="assemblyFileName">程序集文件名</param>
            /// <param name="typeName">对象类型名称</param>
            /// <returns>创建的包装对象,失败返回null</returns>
            public ObjectHandle CreateHandle(string assemblyFileName, string typeName)
            {
                try
                {
                    return Activator.CreateInstance(assemblyFileName, typeName);
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                return null;
            }
        }
        /// <summary>
        /// 默认程序集名称
        /// </summary>
        [NonSerialized]
        private static readonly string assemblyName = typeof(dynamicDomain).Assembly.FullName;
        /// <summary>
        /// 默认程序集加载器类型名称
        /// </summary>
        [NonSerialized]
        private static readonly string assemblyLoaderName = typeof(dynamicDomain.assemblyLoader).FullName;
        /// <summary>
        /// 应用程序域
        /// </summary>
        private readonly AppDomainSetup setup = new AppDomainSetup();
        /// <summary>
        /// 应用程序域
        /// </summary>
        [NonSerialized]
        private AppDomain domain;
        /// <summary>
        /// 程序集加载器
        /// </summary>
        [NonSerialized]
        private assemblyLoader loader;
        /// <summary>
        /// 程序集私有目录
        /// </summary>
        [NonSerialized]
        private string privatePath;
        /// <summary>
        /// 初始化动态应用程序域
        /// </summary>
        /// <param name="name">应用程序域名称</param>
        /// <param name="privatePath">程序集加载目录</param>
        /// <param name="configFile">配置文件</param>
        /// <param name="cacheDirectory">应用程序域缓存目录,null表示非缓存</param>
        public dynamicDomain(string name, string privatePath, string configFile, string cacheDirectory)
        {
            if (string.IsNullOrEmpty(privatePath)) privatePath = fastCSharp.pub.ApplicationPath;
            else
            {
                privatePath = new DirectoryInfo(privatePath).fullName().fileNameToLower();
                if (privatePath != fastCSharp.pub.ApplicationPath) this.privatePath = privatePath;
            }
            setup = new AppDomainSetup();
            if (configFile != null && File.Exists(configFile)) setup.ConfigurationFile = configFile;
            setup.ApplicationName = name;
            setup.ApplicationBase = privatePath;
            setup.PrivateBinPath = privatePath;
            if (cacheDirectory != null && cacheDirectory.Length != 0)
            {
                setup.ShadowCopyFiles = "true";
                setup.ShadowCopyDirectories = cacheDirectory;
                setup.CachePath = cacheDirectory;
            }
            domain = AppDomain.CreateDomain(name, null, setup);
            loader = (assemblyLoader)domain.CreateInstanceAndUnwrap(assemblyName, assemblyLoaderName);
        }
        /// <summary>
        /// 卸载应用程序域
        /// </summary>
        public void Dispose()
        {
            if (domain != null)
            {
                try
                {
                    AppDomain.Unload(domain);
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                domain = null;
                loader = null;
            }
        }
        /// <summary>
        /// 加载程序集
        /// </summary>
        /// <param name="assemblyFileName">程序集文件名</param>
        public Assembly LoadAssembly(string assemblyFileName)
        {
            if (privatePath == null) return loader.Load(assemblyFileName);
#pragma warning disable 618
            AppDomain.CurrentDomain.AppendPrivatePath(privatePath);
            try
            {
                return loader.Load(assemblyFileName);
            }
            finally { AppDomain.CurrentDomain.ClearPrivatePath(); }
#pragma warning restore 618
        }
        /// <summary>
        /// 加载程序集获取类型
        /// </summary>
        /// <param name="assemblyFileName">程序集文件名</param>
        /// <param name="typeName">类型名称</param>
        /// <returns>类型</returns>
        public Type LoadType(string assemblyFileName, string typeName)
        {
            if (privatePath == null) return loader.LoadType(assemblyFileName, typeName);
#pragma warning disable 618
            AppDomain.CurrentDomain.AppendPrivatePath(privatePath);
            try
            {
                return loader.LoadType(assemblyFileName, typeName);
            }
            finally { AppDomain.CurrentDomain.ClearPrivatePath(); }
#pragma warning restore 618
        }
        /// <summary>
        /// 加载程序集并创建对象
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        /// <param name="assemblyFileName">对象类型名称</param>
        /// <param name="typeName">对象类型名称,必须表示为Serializable</param>
        /// <returns>创建的对象</returns>
        public valueType CreateInstance<valueType>(string assemblyFileName, string typeName)
        {
            if (privatePath == null) return (valueType)loader.CreateInstance(assemblyFileName, typeName);
#pragma warning disable 618
            AppDomain.CurrentDomain.AppendPrivatePath(privatePath);
            try
            {
                return (valueType)loader.CreateInstance(assemblyFileName, typeName);
            }
            finally { AppDomain.CurrentDomain.ClearPrivatePath(); }
#pragma warning restore 618
        }
        /// <summary>
        /// 加载程序集并创建包装对象
        /// </summary>
        /// <typeparam name="valueType">对象类型</typeparam>
        /// <param name="assemblyFileName">对象类型名称</param>
        /// <param name="typeName">对象类型名称,必须表示为Serializable</param>
        /// <returns>创建的对象</returns>
        public valueType CreateHandle<valueType>(string assemblyFileName, string typeName)
        {
            if (privatePath == null) return (valueType)loader.CreateHandle(assemblyFileName, typeName).Unwrap();
#pragma warning disable 618
            AppDomain.CurrentDomain.AppendPrivatePath(privatePath);
            try
            {
                return (valueType)loader.CreateHandle(assemblyFileName, typeName).Unwrap();
            }
            finally { AppDomain.CurrentDomain.ClearPrivatePath(); }
#pragma warning restore 618
        }
    }
}
