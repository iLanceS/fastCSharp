using System;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;
using fastCSharp.threading;
using System.Runtime.CompilerServices;

namespace fastCSharp.reflection
{
    /// <summary>
    /// 程序集扩展操作
    /// </summary>
    public static class assembly
    {
        /// <summary>
        /// 根据程序集名称获取程序集
        /// </summary>
        /// <param name="fullName">程序集名称</param>
        /// <returns>程序集,失败返回null</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static Assembly Get(string fullName)
        {
            if (fullName != null)
            {
                Assembly value;
                if (nameCache.TryGetValue(fullName, out value)) return value;
            }
            return null;
        }
        /// <summary>
        /// 获取类型信息
        /// </summary>
        /// <param name="assembly">程序集信息</param>
        /// <param name="fullName">类型全名</param>
        /// <returns>类型信息</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static Type getType(this Assembly assembly, string fullName)
        {
            return assembly != null ? assembly.GetType(fullName) : null;
        }
        /// <summary>
        /// 程序集名称缓存
        /// </summary>
        private static readonly interlocked.dictionary<string, Assembly> nameCache = new interlocked.dictionary<string, Assembly>();
        /// <summary>
        /// 加载程序集
        /// </summary>
        /// <param name="assembly">程序集</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static void loadAssembly(Assembly assembly)
        {
            nameCache.Set(assembly.FullName, assembly);
        }
        /// <summary>
        /// 加载程序集
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void loadAssembly(object sender, AssemblyLoadEventArgs args)
        {
            loadAssembly(args.LoadedAssembly);
        }
        static assembly()
        {
            AppDomain.CurrentDomain.AssemblyLoad += loadAssembly;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) nameCache.Set(assembly.FullName, assembly);
        }
    }
}
