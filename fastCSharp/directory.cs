using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 目录相关操作
    /// </summary>
    public static class directory
    {
        /// <summary>
        /// 目录分隔符
        /// </summary>
        public static readonly string DirectorySeparator = Path.DirectorySeparatorChar.ToString();
        /// <summary>
        /// 目录分隔符\替换
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>替换\后的路径</returns>
        public static string pathSeparator(this string path)
        {
            if (Path.DirectorySeparatorChar != '\\') path.replace('\\', Path.DirectorySeparatorChar);
            return path;
        }
        /// <summary>
        /// 取以\结尾的路径全名
        /// </summary>
        /// <param name="path">目录</param>
        /// <returns>\结尾的路径全名</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static string fullName(this DirectoryInfo path)
        {
            return path != null ? path.FullName.pathSuffix() : null;
        }
        /// <summary>
        /// 路径补全结尾的\
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>路径</returns>
        public static string pathSuffix(this string path)
        {
            if (string.IsNullOrEmpty(path)) return DirectorySeparator;
            return unsafer.String.Last(path) == Path.DirectorySeparatorChar ? path : (path + DirectorySeparator);
        }
        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="path">目录</param>
        /// <returns>是否创建成功</returns>
        public static bool Create(string path)
        {
            if (path != null)
            {
                if (Directory.Exists(path)) return true;
                try
                {
                    Directory.CreateDirectory(path);
                    return true;
                }
                catch (Exception error)
                {
                    log.Error.Add(error, "目录创建失败 : " + path, false);
                }
            }
            return false;
        }
    }
}
