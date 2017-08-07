using System;
using fastCSharp.threading;
using System.Threading;

namespace fastCSharp.emit
{
    /// <summary>
    /// 名称申请池
    /// </summary>
    internal unsafe static class namePool
    {
        /// <summary>
        /// 申请池起始位置
        /// </summary>
        private static char* start;
        /// <summary>
        /// 申请池结束未知
        /// </summary>
        private static char* end;
        /// <summary>
        /// 申请池创建访问锁
        /// </summary>
        private static readonly object createLock = new object();
        /// <summary>
        /// 申请池获取访问锁
        /// </summary>
        private static int getLock;
        /// <summary>
        /// 申请池大小
        /// </summary>
        private static readonly int poolSize = fastCSharp.config.pub.Default.EmitNamePoolSize;
        /// <summary>
        /// 获取名称空间
        /// </summary>
        /// <param name="length">字符串长度</param>
        /// <returns></returns>
        public static char* GetChar(int length)
        {
            interlocked.CompareSetYield(ref getLock);
            char* value = start;
            if ((start += length) <= end)
            {
                getLock = 0;
                return value;
            }
            getLock = 0;
            Monitor.Enter(createLock);
            interlocked.CompareSetYield(ref getLock);
            if ((start += length) <= end)
            {
                value = start - length;
                getLock = 0;
                Monitor.Exit(createLock);
                return value;
            }
            getLock = 0;
            try
            {
                value = unmanaged.GetStatic(poolSize, false).Char;
                interlocked.CompareSetYield(ref getLock);
                start = value + length;
                end = value + (poolSize >> 1);
                getLock = 0;
            }
            finally { Monitor.Exit(createLock); }
            return value;
        }
        /// <summary>
        /// 获取名称空间
        /// </summary>
        /// <param name="name"></param>
        /// <param name="seek">前缀字符长度</param>
        /// <param name="endSize">后缀字符长度</param>
        /// <returns></returns>
        public static char* Get(string name, int seek, int endSize)
        {
            char* value = GetChar(name.Length + (seek + endSize));
            fixed (char* nameFixed = name) fastCSharp.unsafer.memory.Copy(nameFixed, value + seek, name.Length);
            return value;
        }
    }
}
