using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 非托管内存
    /// </summary>
    public unsafe static class unmanaged
    {
        /// <summary>
        /// 非托管内存申请字节数
        /// </summary>
        private static long totalSize;
        /// <summary>
        /// 非托管内存申请字节数
        /// </summary>
        public static long TotalSize
        {
            get { return totalSize; }
        }
        /// <summary>
        /// 申请非托管内存
        /// </summary>
        /// <param name="size">内存字节数</param>
        /// <param name="isClear">是否需要清除</param>
        /// <returns>非托管内存起始指针</returns>
        public static pointer.size Get(int size, bool isClear = true)
        {
            if (size < 0) log.Error.Throw(log.exceptionType.IndexOutOfRange);
            if (size != 0)
            {
                byte* data = (byte*)Marshal.AllocHGlobal(size);
                Interlocked.Add(ref totalSize, size);
                if (isClear) fastCSharp.unsafer.memory.Fill(data, (byte)0, size);
                return new pointer.size { data = data, sizeValue = size };
            }
            return default(pointer.size);
        }
        /// <summary>
        /// 申请非托管内存
        /// </summary>
        /// <param name="size"></param>
        /// <param name="isClear"></param>
        /// <param name="isStaticUnmanaged"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static pointer.size Get(int size, bool isClear, bool isStaticUnmanaged)
        {
            return isStaticUnmanaged ? GetStaticSize(size, isClear) : Get(size, isClear);
        }
        /// <summary>
        /// 不释放的固定内存申请大小
        /// </summary>
        private static long staticSize;
        /// <summary>
        /// 不释放的固定内存申请大小
        /// </summary>
        public static long StaticSize
        {
            get { return staticSize; }
        }
        /// <summary>
        /// 批量申请非托管内存
        /// </summary>
        /// <param name="isClear">是否需要清除</param>
        /// <param name="sizes">内存字节数集合</param>
        /// <returns>非托管内存起始指针</returns>
        internal static pointer[] GetStatic(bool isClear, params int[] sizes)
        {
            if (sizes.length() != 0)
            {
                int sum = 0;
                foreach (int size in sizes)
                {
                    if (size < 0) log.Error.Throw(log.exceptionType.IndexOutOfRange);
                    checked { sum += size; }
                }
                pointer pointer = GetStatic(sum, isClear);
                byte* data = pointer.Byte;
                if (data != null)
                {
                    int index = 0;
                    pointer[] datas = new pointer[sizes.Length];
                    foreach (int size in sizes)
                    {
                        datas[index++] = new pointer { Data = data };
                        data += size;
                    }
                    return datas;
                }
            }
            return null;
        }
        /// <summary>
        /// 静态类型初始化申请非托管内存(不释放的固定内存)
        /// </summary>
        /// <param name="size"></param>
        /// <param name="isClear">是否需要清除</param>
        /// <returns></returns>
        public static pointer GetStatic(int size, bool isClear)
        {
            byte* data = (byte*)Marshal.AllocHGlobal(size);
            Interlocked.Add(ref staticSize, size);
            if (isClear) fastCSharp.unsafer.memory.Fill(data, (byte)0, size);
            return new pointer { Data = data };
        }
        /// <summary>
        /// 静态类型初始化申请非托管内存(不释放的固定内存)
        /// </summary>
        /// <param name="size"></param>
        /// <param name="isClear">是否需要清除</param>
        /// <returns></returns>
        internal static pointer.size GetStaticSize(int size, bool isClear)
        {
            byte* data = (byte*)Marshal.AllocHGlobal(size);
            Interlocked.Add(ref staticSize, size);
            if (isClear) fastCSharp.unsafer.memory.Fill(data, (byte)0, size);
            return new pointer.size { data = data, sizeValue = size };
        }
        /// <summary>
        /// 8个0字节（公用）
        /// </summary>
        internal static readonly pointer.reference NullByte8 = GetStatic(8, true).Reference;
        /// <summary>
        /// 释放内存
        /// </summary>
        /// <param name="data">非托管内存起始指针</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static void Free(ref pointer.size data)
        {
            if (data.data != null)
            {
                Interlocked.Add(ref totalSize, -data.sizeValue);
                Marshal.FreeHGlobal((IntPtr)data.data);
                data.Null();
            }
        }
        /// <summary>
        /// 释放内存
        /// </summary>
        /// <param name="data">非托管内存起始指针</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static void FreeStatic(ref pointer.size data)
        {
            if (data.data != null)
            {
                Interlocked.Add(ref staticSize, -data.sizeValue);
                Marshal.FreeHGlobal((IntPtr)data.data);
                data.Null();
            }
        }
    }
}
