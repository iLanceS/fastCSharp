using System;
using System.Reflection;
using System.IO;

namespace fastCSharp.code
{
    /// <summary>
    /// 自动安装属性
    /// </summary>
    internal sealed class auto : Attribute
    {
        /// <summary>
        /// 安装属性空值
        /// </summary>
        internal static readonly auto NullAuto = new auto();
        /// <summary>
        /// 自动安装参数
        /// </summary>
        public sealed class parameter
        {
            /// <summary>
            /// 项目名称
            /// </summary>
            public string ProjectName { get; private set; }
            /// <summary>
            /// 项目路径
            /// </summary>
            public string ProjectPath { get; private set; }
            /// <summary>
            /// 程序集文件名全称
            /// </summary>
            public string AssemblyPath { get; private set; }
            /// <summary>
            /// 项目默认命名空间
            /// </summary>
            public string DefaultNamespace { get; private set; }
            /// <summary>
            /// 是否fastCSharp项目
            /// </summary>
            public bool IsFastCSharp { get; private set; }
            /// <summary>
            /// 程序集
            /// </summary>
            private Assembly assembly;
            /// <summary>
            /// 程序集
            /// </summary>
            public Assembly Assembly
            {
                get
                {
                    if (assembly == null)
                    {
                        try
                        {
                            string assemblyFile = AssemblyPath.Substring(AssemblyPath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                            foreach (Assembly value in AppDomain.CurrentDomain.GetAssemblies())
                            {
                                if (value.ManifestModule.Name == assemblyFile) return assembly = value;
                            }
                            //System.AppDomain.CurrentDomain.SetupInformation.PrivateBinPath = new FileInfo(AssemblyPath).DirectoryName;
                            assembly = Assembly.LoadFrom(AssemblyPath);
                        }
                        catch (Exception error)
                        {
                            code.error.Add(error);
                        }
                    }
                    return assembly;
                }
            }
            /// <summary>
            /// 类型集合
            /// </summary>
            private Type[] types;
            /// <summary>
            /// 类型集合
            /// </summary>
            public Type[] Types
            {
                get
                {
                    if (types == null && Assembly != null)
                    {
                        try
                        {
                            types = assembly.GetTypes().sort((left, right) => string.CompareOrdinal(left.FullName, right.FullName));
                        }
                        catch (Exception error)
                        {
                            types = nullValue<Type>.Array;
                            code.error.Add(error);
                        }
                    }
                    return types;
                }
            }
            /// <summary>
            /// 网站生成配置
            /// </summary>
            private webConfig webConfig;
            /// <summary>
            /// 网站生成配置
            /// </summary>
            public webConfig WebConfig
            {
                get
                {
                    if (webConfig == null)
                    {
                        Type type = Assembly.GetType(DefaultNamespace + "." + typeof(webConfig).Name);
                        if (type != null) webConfig = Activator.CreateInstance(type) as webConfig;
                    }
                    return webConfig;
                }
            }
            /// <summary>
            /// 自动安装参数
            /// </summary>
            /// <param name="projectName">项目名称</param>
            /// <param name="projectPath">项目路径</param>
            /// <param name="assemblyPath">程序集文件名全称</param>
            /// <param name="defaultNamespace">项目默认命名空间</param>
            /// <param name="isFastCSharp">是否fastCSharp项目</param>
            public parameter(string projectName, string projectPath, string assemblyPath, string defaultNamespace, bool isFastCSharp)
            {
                ProjectName = projectName;
                ProjectPath = new DirectoryInfo(projectPath).fullName().toLower();
                AssemblyPath = assemblyPath;
                DefaultNamespace = defaultNamespace;
                IsFastCSharp = isFastCSharp;
            }
            /// <summary>
            /// 复制安装参数
            /// </summary>
            /// <returns>安装参数</returns>
            public parameter Copy()
            {
                return new parameter(ProjectName, ProjectPath, AssemblyPath, DefaultNamespace, IsFastCSharp);
            }
        }
        /// <summary>
        /// 代码生成语言扩展
        /// </summary>
        public sealed class languageAttribute : Attribute
        {
            /// <summary>
            /// 模板文件扩展名
            /// </summary>
            public string ExtensionName;
        }
        /// <summary>
        /// 代码生成语言
        /// </summary>
        public enum language: byte
        {
            /// <summary>
            /// C#
            /// </summary>
            [languageAttribute(ExtensionName = "cs")]
            CSharp,
            /// <summary>
            /// JavaScript
            /// </summary>
            [languageAttribute(ExtensionName = "js")]
            JavaScript,
            /// <summary>
            /// TypeScript
            /// </summary>
            [languageAttribute(ExtensionName = "ts.txt")]
            TypeScript,
        }
        /// <summary>
        /// 是否自动安装
        /// </summary>
        public bool IsAuto;
        /// <summary>
        /// 自动安装依赖,指定当前安装必须后于依赖安装
        /// </summary>
        public Type DependType;
        /// <summary>
        /// 安装名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 代码生成语言
        /// </summary>
        public language Language = language.CSharp;
        /// <summary>
        /// 模板文件是否使用嵌套类型名称
        /// </summary>
        public bool IsDeclaringTypeName = true;
        /// <summary>
        /// 是否生成模板代码
        /// </summary>
        public bool IsTemplate = true;
        /// <summary>
        /// 安装名称
        /// </summary>
        public string ShowName(Type type)
        {
            if (Name != null) return Name;
            if (type != null) return type.Name;
            return "未知";
        }
        /// <summary>
        /// 模板文件名称，不包括扩展名
        /// </summary>
        public string Template;
        /// <summary>
        /// 获取模板文件名，不包括扩展名
        /// </summary>
        /// <param name="type">模板数据视图</param>
        /// <returns>模板文件名</returns>
        public string GetFileName(Type type)
        {
            return Template ?? (IsDeclaringTypeName ? type.DeclaringType.Name : type.Name);
        }
    }
    /// <summary>
    /// 自动安装接口
    /// </summary>
    internal interface IAuto
    {
        /// <summary>
        /// 安装入口
        /// </summary>
        /// <param name="parameter">安装参数</param>
        /// <returns>是否安装成功</returns>
        bool Run(auto.parameter parameter);
    }
}
