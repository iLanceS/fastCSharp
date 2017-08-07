using System;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.IO.File 扩展
    /// </summary>
    public static class file
    {
        /// <summary>
        /// 文件目录分隔符
        /// </summary>
        public static string separator
        {
            get { return Java.IO.File.Separator; }
        }
        /// <summary>
        /// AbsolutePath
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getAbsolutePath(this Java.IO.File file)
        {
            return file.AbsolutePath;
        }
        /// <summary>
        /// Mkdirs
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool mkdirs(this Java.IO.File file)
        {
            return file.Mkdirs();
        }
        /// <summary>
        /// Exists
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool exists(this Java.IO.File file)
        {
            return file.Exists();
        }
        /// <summary>
        /// Path
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getPath(this Java.IO.File file)
        {
            return file.Path;
        }
        /// <summary>
        /// Name
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getName(this Java.IO.File file)
        {
            return file.Name;
        }
        /// <summary>
        /// ListFiles
        /// </summary>
        /// <param name="file"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.IO.File[] listFiles(this Java.IO.File file, Java.IO.IFileFilter filter)
        {
            return file.ListFiles(filter);
        }
        /// <summary>
        /// ListFiles
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.IO.File[] listFiles(this Java.IO.File file)
        {
            return file.ListFiles();
        }
        /// <summary>
        /// IsDirectory
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool isDirectory(this Java.IO.File file)
        {
            return file.IsDirectory;
        }
        /// <summary>
        /// IsFile
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool isFile(this Java.IO.File file)
        {
            return file.IsFile;
        }
        /// <summary>
        /// AbsoluteFile
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.IO.File getAbsoluteFile(this Java.IO.File file)
        {
            return file.AbsoluteFile;
        }
        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool delete(this Java.IO.File file)
        {
            return file.Delete();
        }
        /// <summary>
        /// ParentFile
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.IO.File getParentFile(this Java.IO.File file)
        {
            return file.ParentFile;
        }
        /// <summary>
        /// Mkdir
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool mkdir(this Java.IO.File file)
        {
            return file.Mkdir();
        }
    }
}