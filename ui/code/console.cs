using System;
using System.IO;
using fastCSharp.reflection;
using fastCSharp.io;

namespace fastCSharp.code
{
    /// <summary>
    /// 安装控制台
    /// </summary>
    internal sealed class console
    {
        /// <summary>
        /// 启动控制台安装
        /// </summary>
        /// <param name="args">控制台参数</param>
        public static void Start(string[] args)
        {
            if (args.Length >= 4) auto(args[1] != null ? new auto.parameter(args[0].TrimEnd(' '), args[1].TrimEnd(' '), args[2].TrimEnd(' '), args[3].TrimEnd(' '), args.Length > 4) : createParameter(args[0]));
            else ui.ShowSetup();
        }
        /// <summary>
        /// 自动安装
        /// </summary>
        /// <param name="parameter">安装参数</param>
        static void auto(auto.parameter parameter)
        {
            if (parameter.ProjectPath.length() != 0 && Directory.Exists(parameter.ProjectPath))
            {
                try
                {
                    subArray<keyValue<Type, auto>> autos = code.ui.CurrentAssembly.GetTypes()
                        .getFind(type => !type.IsInterface && !type.IsAbstract && typeof(IAuto).IsAssignableFrom(type))
                        .GetArray(type => new keyValue<Type, auto>(type, type.customAttribute<auto>()))
                        .getFind(value => value.Value != null && value.Value.IsAuto);
                    ui.Setup(autos.ToArray(), parameter, false);
                }
                catch (Exception error)
                {
                    code.error.Add(error);
                }
            }
            else code.error.Add("项目路径不存在 : " + parameter.ProjectPath);
            code.error.Open(true);
        }
        /// <summary>
        /// 根据项目名称构造安装参数
        /// </summary>
        /// <param name="projectName">项目名称</param>
        /// <returns>安装参数</returns>
        static auto.parameter createParameter(string projectName)
        {
            string projectPath = null, assemblyPath = null, projectNamespace = null;
            if (projectName.length() != 0)
            {
                projectPath = config.code.Default.ProjectPath + projectName + directory.DirectorySeparator;
                projectNamespace = config.code.Default.BaseNamespace + "." + projectName;
                assemblyPath = checkAssemblyPath(projectPath + @"bin\Release\" + projectNamespace)
                    ?? checkAssemblyPath(projectPath + @"bin\Debug\" + projectNamespace)
                    ?? checkAssemblyPath(projectPath + @"bin\" + projectNamespace)
                    ?? checkAssemblyPath(projectPath + projectNamespace);
            }
            return new auto.parameter(projectName, projectPath, assemblyPath, projectNamespace, false);
        }
        /// <summary>
        /// 检测程序集文件路径
        /// </summary>
        /// <param name="path">程序集文件路径</param>
        /// <returns>程序集文件路径,失败返回null</returns>
        static string checkAssemblyPath(string path)
        {
            path.pathSeparator();
            if (File.Exists(path + ".dll")) return path + ".dll";
            if (File.Exists(path + ".exe")) return path + ".exe";
            return null;
        }
    }
}
