using System;
using System.Text;
using System.IO;
using System.Threading;
using System.Security.Cryptography;
using fastCSharp.threading;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 常用公共定义
    /// </summary>
    public static class pub
    {
        /// <summary>
        /// 项目常量，不可修改
        /// </summary>
        public const string fastCSharp = "fastCSharp";
        /// <summary>
        /// 基数排序数据量
        /// </summary>
        public const int RadixSortSize = 1 << 9;
        /// <summary>
        /// 64位基数排序数据量
        /// </summary>
        public const int RadixSortSize64 = 4 << 9;
        /// <summary>
        /// 如果可能方法应内联(枚举值)
        /// </summary>
#if INLINE
        public const int MethodImplOptionsAggressiveInlining = 256;
#else
        public const int MethodImplOptionsAggressiveInlining = 0;
#endif
        /// <summary>
        /// 程序执行主目录(小写字母)
        /// </summary>
        public static readonly string ApplicationPath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory ?? System.Environment.CurrentDirectory).fullName().fileNameToLower();
        /// <summary>
        /// CPU核心数量
        /// </summary>
        public static readonly int CpuCount = Math.Max(Environment.ProcessorCount, 1);
        /// <summary>
        /// 默认空字符
        /// </summary>
        public const char NullChar = (char)0;
        /// <summary>
        /// 程序启用时间
        /// </summary>
        public static readonly DateTime StartTime = DateTime.Now;
        /// <summary>
        /// 最小时间值
        /// </summary>
        public static readonly DateTime MinTime = new DateTime(1900, 1, 1);
        /// <summary>
        /// Json转换时间差
        /// </summary>
        public static readonly DateTime JavascriptLocalMinTime;
        /// <summary>
        /// 本地时钟周期
        /// </summary>
        public static readonly long LocalTimeTicks;
        /// <summary>
        /// 默认自增标识
        /// </summary>
        private static int identity32;
        /// <summary>
        /// 默认自增标识
        /// </summary>
        internal static int Identity32
        {
            get { return Interlocked.Increment(ref identity32); }
        }
        /// <summary>
        /// 默认自增标识
        /// </summary>
        private static long identity;
        /// <summary>
        /// 默认自增标识
        /// </summary>
        public static long Identity
        {
            get { return Interlocked.Increment(ref identity); }
        }
        /// <summary>
        /// SHA1哈希加密
        /// </summary>
        private static readonly SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
        /// <summary>
        /// SHA1哈希加密访问锁
        /// </summary>
        private static readonly object sha1Lock = new object();
        /// <summary>
        /// SHA1哈希加密
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <returns>SHA1哈希</returns>
        public static byte[] Sha1(byte[] buffer)
        {
            return Sha1(buffer, 0, buffer.Length);
        }
        /// <summary>
        /// SHA1哈希加密
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="length">数据长度</param>
        /// <returns>SHA1哈希</returns>
        public static byte[] Sha1(byte[] buffer, int startIndex, int length)
        {
            Monitor.Enter(sha1Lock);
            try
            {
                buffer = sha1.ComputeHash(buffer, startIndex, length);
            }
            finally { Monitor.Exit(sha1Lock); }
            return buffer;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <typeparam name="valueType">资源类型</typeparam>
        /// <param name="resource">资源引用</param>
        [MethodImpl((MethodImplOptions)MethodImplOptionsAggressiveInlining)]
        public static void Dispose<valueType>(valueType resource)
            where valueType : class, IDisposable
        {
            if (resource != null)
            {
                try
                {
                    resource.Dispose();
                }
                catch (Exception error)
                {
                    log.Default.Add(error, null, false);
                }
            }
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <typeparam name="valueType">资源类型</typeparam>
        /// <param name="resource">资源引用</param>
        [MethodImpl((MethodImplOptions)MethodImplOptionsAggressiveInlining)]
        public static void Dispose<valueType>(ref valueType resource)
            where valueType : class, IDisposable
        {
            Exception exception = null;
            Dispose(ref resource, ref exception);
        }
        ///// <summary>
        ///// 释放资源
        ///// </summary>
        ///// <typeparam name="valueType">资源类型</typeparam>
        ///// <param name="resource">资源引用</param>
        //public static void Dispose<valueType>(ref valueType[] resource) where valueType : class, IDisposable
        //{
        //    valueType[] values = Interlocked.Exchange(ref resource, null);
        //    if (values != null)
        //    {
        //        foreach (valueType value in values)
        //        {
        //            try
        //            {
        //                value.Dispose();
        //            }
        //            catch (Exception error)
        //            {
        //                log.Default.Add(error, null, false);
        //            }
        //        }
        //    }
        //}
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <typeparam name="valueType">资源类型</typeparam>
        /// <param name="resource">资源引用</param>
        /// <param name="exception"></param>
        public static void Dispose<valueType>(ref valueType resource, ref Exception exception)
            where valueType : class, IDisposable
        {
            valueType value = Interlocked.Exchange(ref resource, null);
            if (value != null)
            {
                try
                {
                    value.Dispose();
                }
                catch (Exception error)
                {
                    exception = error;
                }
            }
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <typeparam name="valueType">资源类型</typeparam>
        /// <param name="accessLock">资源访问锁</param>
        /// <param name="resource">资源引用</param>
        [MethodImpl((MethodImplOptions)MethodImplOptionsAggressiveInlining)]
        public static void Dispose<valueType>(object accessLock, ref valueType resource)
            where valueType : class, IDisposable
        {
            Exception exception = null;
            Dispose(accessLock, ref resource, ref exception);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <typeparam name="valueType">资源类型</typeparam>
        /// <param name="accessLock">资源访问锁</param>
        /// <param name="resource">资源引用</param>
        /// <param name="exception">错误异常引用</param>
        public static void Dispose<valueType>(object accessLock, ref valueType resource, ref Exception exception)
            where valueType : class, IDisposable
        {
            valueType value = resource;
            if (value != null)
            {
                Monitor.Enter(accessLock);
                try
                {
                    if (resource != null)
                    {
                        value.Dispose();
                        resource = null;
                    }
                }
                catch (Exception error)
                {
                    exception = error;
                }
                finally { Monitor.Exit(accessLock); }
            }
        }
        /// <summary>
        /// 函数调用,用于链式编程
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="value">数据</param>
        /// <param name="method">函数调用</param>
        /// <returns>数据</returns>
        [MethodImpl((MethodImplOptions)MethodImplOptionsAggressiveInlining)]
        public static valueType action<valueType>(this valueType value, Action<valueType> method) where valueType : class
        {
            if (method == null) log.Error.Throw(log.exceptionType.Null);
            if (value != null) method(value);
            return value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)MethodImplOptionsAggressiveInlining)]
        public static string fileNameToLower(this string fileName)
        {
#if MONO
            return fileName;
#else
            return fileName.toLower();
#endif
        }
        /// <summary>
        /// 时间转换
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)MethodImplOptionsAggressiveInlining)]
        internal static DateTime localToUniversalTime(this DateTime date)
        {
            return new DateTime(date.Ticks - LocalTimeTicks, DateTimeKind.Utc);
        }
        /// <summary>
        /// 时间转换
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)MethodImplOptionsAggressiveInlining)]
        internal static DateTime toUniversalTime(this DateTime date)
        {
            return date.Kind == DateTimeKind.Utc ? date : new DateTime(date.Ticks - LocalTimeTicks, DateTimeKind.Utc);
        }
        /// <summary>
        /// 时间转换
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)MethodImplOptionsAggressiveInlining)]
        internal static DateTime toLocalTime(this DateTime date)
        {
            return date.Kind == DateTimeKind.Local ? date : new DateTime(date.Ticks + LocalTimeTicks, DateTimeKind.Utc);
        }
        /// <summary>
        /// 内存位长
        /// </summary>
        public static readonly int MemoryBits;
        /// <summary>
        /// 内存字节长度
        /// </summary>
        public static readonly int MemoryBytes;
        static unsafe pub()
        {
            byte* bytes = stackalloc byte[4];
            if (((long)bytes >> 32) == 0)
            {
                MemoryBits = ((long)(bytes + 0x100000000L) >> 32) == 0 ? 32 : 64;
            }
            else MemoryBits = 64;
            MemoryBytes = MemoryBits >> 3;

            DateTime now = DateTime.Now;
            LocalTimeTicks = now.Ticks - now.ToUniversalTime().Ticks;
            JavascriptLocalMinTime = new DateTime(new DateTime(1970, 1, 1).Ticks + LocalTimeTicks, DateTimeKind.Utc);
        }
    }
}
