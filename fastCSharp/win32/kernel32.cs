using System;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;
using Microsoft.Win32.SafeHandles;

namespace fastCSharp.win32
{
    /// <summary>
    /// kernel32.dll API
    /// </summary>
    internal static class kernel32
    {
        /// <summary>
        /// 内存复制
        /// </summary>
        /// <param name="dest">目标位置</param>
        /// <param name="src">源位置</param>
        /// <param name="length">字节长度</param>
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static unsafe extern void RtlMoveMemory(void* dest, void* src, int length);
        /// <summary>
        /// 获取指定磁盘的信息，包括磁盘的可用空间。
        /// </summary>
        /// <param name="bootPath">磁盘根目录。如：@"C:\"</param>
        /// <param name="sectorsPerCluster">每个簇所包含的扇区个数</param>
        /// <param name="bytesPerSector">每个扇区所占的字节数</param>
        /// <param name="numberOfFreeClusters">空闲的簇的个数</param>
        /// <param name="totalNumberOfClusters">簇的总个数</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool GetDiskFreeSpace(string bootPath, out uint sectorsPerCluster, out uint bytesPerSector, out uint numberOfFreeClusters, out uint totalNumberOfClusters);
    }
}
