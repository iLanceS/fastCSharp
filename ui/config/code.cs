using System;
using System.Reflection;
using System.IO;

namespace fastCSharp.config
{
    /// <summary>
    /// 安装配置
    /// </summary>
    internal class code
    {
        /// <summary>
        /// 安装项目基本命名空间
        /// </summary>
        private string baseNamespace = fastCSharp.pub.fastCSharp;
        /// <summary>
        /// 安装项目基本命名空间
        /// </summary>
        public string BaseNamespace
        {
            get { return baseNamespace; }
        }
        /// <summary>
        /// 安装项目查找路径
        /// </summary>
        private string projectPath;
        /// <summary>
        /// 安装项目查找路径
        /// </summary>
        public string ProjectPath
        {
            get
            {
                if (projectPath == null)
                {
                    projectPath = new DirectoryInfo(fastCSharp.pub.ApplicationPath).Parent.Parent.Parent.fullName() + fastCSharp.pub.fastCSharp + "\\";
                }
                return projectPath;
            }
        }
        /// <summary>
        /// 安装标题
        /// </summary>
        private string setupTitle = string.Empty;
        /// <summary>
        /// 安装标题
        /// </summary>
        public string SetupTitle
        {
            get { return setupTitle; }
        }
        /// <summary>
        /// 安装配置
        /// </summary>
        private code()
        {
            pub.LoadConfig(this);
            projectPath = projectPath.pathSuffix().toLower();
        }
        /// <summary>
        /// 默认安装配置
        /// </summary>
        public static readonly code Default = new code();
    }
}
