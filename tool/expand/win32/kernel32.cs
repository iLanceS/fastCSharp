using System;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;
using fastCSharp.win32.safeHandles;
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
        /// 错误句柄值
        /// </summary>
        internal static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        /// <summary>
        /// 释放句柄
        /// </summary>
        /// <param name="handle">句柄</param>
        /// <returns>是否成功</returns>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        internal static extern bool CloseHandle(IntPtr handle);
        /// <summary>
        /// 释放内存
        /// </summary>
        /// <param name="hMem">内存句柄</param>
        /// <returns>IntPtr.Zero表示成功</returns>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport("kernel32.dll")]
        internal static extern IntPtr LocalFree(IntPtr hMem);

        /// <summary>
        /// 进程附加到作业
        /// </summary>
        /// <param name="job">作业句柄</param>
        /// <param name="process">进程句柄</param>
        /// <returns>是否成功</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool AssignProcessToJobObject(IntPtr job, IntPtr process);
        /// <summary>
        /// 设置作业信息
        /// </summary>
        /// <param name="hJob">作业句柄</param>
        /// <param name="infoType">信息类型</param>
        /// <param name="lpJobObjectInfo">信息句柄</param>
        /// <param name="cbJobObjectInfoLength">信息字节长度</param>
        /// <returns>是否成功</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetInformationJobObject(IntPtr hJob, diagnostics.job.informationType infoType, IntPtr lpJobObjectInfo, uint cbJobObjectInfoLength);
        /// <summary>
        /// 创建作业
        /// </summary>
        /// <param name="lpJobAttributes">安全属性SECURITY_ATTRIBUTES</param>
        /// <param name="lpName">作业名称</param>
        /// <returns>作业句柄</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr CreateJobObject(IntPtr lpJobAttributes, string lpName);

        /// <summary>
        /// 创建进程
        /// </summary>
        /// <param name="lpApplicationName">应用名称,可以null</param>
        /// <param name="lpCommandLine">命令行</param>
        /// <param name="lpProcessAttributes">进程安全属性</param>
        /// <param name="lpThreadAttributes">线程安全属性</param>
        /// <param name="bInheritHandles">是否继承句柄</param>
        /// <param name="dwCreationFlags">创建状态标识</param>
        /// <param name="lpEnvironment">环境变量</param>
        /// <param name="lpCurrentDirectory">工作目录</param>
        /// <param name="lpStartupInfo">启动信息</param>
        /// <param name="lpProcessInformation">进程信息</param>
        /// <returns>是否成功</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool CreateProcess([MarshalAs(UnmanagedType.LPTStr)] string lpApplicationName, string lpCommandLine, securityAttributes lpProcessAttributes, securityAttributes lpThreadAttributes, bool bInheritHandles, diagnostics.process.createFlags dwCreationFlags, IntPtr lpEnvironment, [MarshalAs(UnmanagedType.LPTStr)] string lpCurrentDirectory, diagnostics.process.startupInfo lpStartupInfo, diagnostics.process.processInformation lpProcessInformation);
        /// <summary>
        /// 唤醒线程
        /// </summary>
        /// <param name="hThread">线程句柄</param>
        /// <returns>负数表示失败</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int ResumeThread(IntPtr hThread);
        /// <summary>
        /// 创建通道
        /// </summary>
        /// <param name="hReadPipe">读取通道</param>
        /// <param name="hWritePipe">写入通道</param>
        /// <param name="lpPipeAttributes">安全属性</param>
        /// <param name="nSize">缓存区字节数,0表示默认</param>
        /// <returns>是否成功</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool CreatePipe(out SafeFileHandle hReadPipe, out SafeFileHandle hWritePipe, securityAttributes lpPipeAttributes, int nSize);
        /// <summary>
        /// 复制句柄
        /// </summary>
        /// <param name="hSourceProcessHandle">被复制进程句柄,必须有PROCESS_DUP_HANDLE权限</param>
        /// <param name="hSourceHandle">被复制句柄</param>
        /// <param name="hTargetProcess">目标进程句柄，必须有PROCESS_DUP_HANDLE权限</param>
        /// <param name="targetHandle">目标句柄</param>
        /// <param name="dwDesiredAccess">访问权限标识</param>
        /// <param name="bInheritHandle">是否继承</param>
        /// <param name="dwOptions">可选参数</param>
        /// <returns>是否成功</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        internal static extern bool DuplicateHandle(HandleRef hSourceProcessHandle, SafeHandle hSourceHandle, HandleRef hTargetProcess, out SafeFileHandle targetHandle, int dwDesiredAccess, bool bInheritHandle, duplicateHandleOptions dwOptions);
        /// <summary>
        /// 获取当前进程句柄
        /// </summary>
        /// <returns>当前进程句柄</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        internal static extern IntPtr GetCurrentProcess();
        /// <summary>
        /// 获取控制台输入编码
        /// </summary>
        /// <returns>Encoding编码</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetConsoleCP();
        /// <summary>
        /// 获取控制台输出编码
        /// </summary>
        /// <returns>Encoding编码</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetConsoleOutputCP();
        /// <summary>
        /// 获取标准输入输出句柄
        /// </summary>
        /// <param name="whichHandle"></param>
        /// <returns>句柄</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        internal static extern IntPtr GetStdHandle(standardHandle whichHandle);

        /// <summary>
        /// 创建内存映射文件
        /// </summary>
        /// <param name="hFile">文件句柄</param>
        /// <param name="lpAttributes">安全属性</param>
        /// <param name="fProtect"></param>
        /// <param name="dwMaximumSizeHigh">文件大小高32位</param>
        /// <param name="dwMaximumSizeLow">文件大小低32位</param>
        /// <param name="lpName"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern safeMemoryMappedFileHandle CreateFileMapping(SafeFileHandle hFile, securityAttributes lpAttributes, int fProtect, int dwMaximumSizeHigh, int dwMaximumSizeLow, string lpName);
        /// <summary>
        /// 打开内存映射文件
        /// </summary>
        /// <param name="dwDesiredAccess">访问类型</param>
        /// <param name="bInheritHandle">是否继承句柄</param>
        /// <param name="lpName">标识名称</param>
        /// <returns>内存映射文件句柄</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern safeMemoryMappedFileHandle OpenFileMapping(int dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, string lpName);
        /// <summary>
        /// 获取内存映射文件视图
        /// </summary>
        /// <param name="handle">内存映射文件句柄</param>
        /// <param name="dwDesiredAccess">访问类型</param>
        /// <param name="dwFileOffsetHigh">文件大小高32位</param>
        /// <param name="dwFileOffsetLow">文件大小低32位</param>
        /// <param name="dwNumberOfBytesToMap">视图大小</param>
        /// <returns>内存映射文件视图</returns>
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern safeMemoryMappedViewHandle MapViewOfFile(safeMemoryMappedFileHandle handle, int dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, UIntPtr dwNumberOfBytesToMap);
        /// <summary>
        /// 释放内存映射文件视图
        /// </summary>
        /// <param name="lpBaseAddress">内存映射文件视图句柄</param>
        /// <returns>是否成功</returns>
        [DllImport("kernel32.dll", ExactSpelling = true)]
        internal static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);
        /// <summary>
        /// 申请内存
        /// </summary>
        /// <param name="address">内存映射文件视图句柄</param>
        /// <param name="buffer">内存基本信息</param>
        /// <param name="sizeOfBuffer">内存基本信息结构大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr VirtualQuery(safeMemoryMappedViewHandle address, ref memoryBasicInformation buffer, IntPtr sizeOfBuffer);
        /// <summary>
        /// 申请内存
        /// </summary>
        /// <param name="address">内存映射文件视图句柄</param>
        /// <param name="numBytes">区域大小</param>
        /// <param name="commitOrReserve"></param>
        /// <param name="pageProtectionMode">访问类型</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr VirtualAlloc(safeMemoryMappedViewHandle address, UIntPtr numBytes, int commitOrReserve, int pageProtectionMode);

        /// <summary>
        /// 获取系统信息
        /// </summary>
        /// <param name="lpSystemInfo">系统信息</param>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern void GetSystemInfo(ref systemInfo lpSystemInfo);
        /// <summary>
        /// 获取扩展内存状态信息
        /// </summary>
        /// <param name="lpBuffer">扩展内存状态信息</param>
        /// <returns>是否成功</returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool GlobalMemoryStatusEx([In, Out] memoryStatuExpand lpBuffer);

        /// <summary>
        /// 系统信息SYSTEM_INFO
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct systemInfo
        {
            /// <summary>
            /// 系统OEM标识
            /// </summary>
            internal int OemId;
            /// <summary>
            /// 内存分页大小
            /// </summary>
            internal int PageSize;
            /// <summary>
            /// 应用程序最小地址
            /// </summary>
            internal IntPtr MinimumApplicationAddress;
            /// <summary>
            /// 应用程序最大地址
            /// </summary>
            internal IntPtr MaximumApplicationAddress;
            /// <summary>
            /// 活动处理器
            /// </summary>
            internal IntPtr ActiveProcessorMask;
            /// <summary>
            /// 处理器数量
            /// </summary>
            internal int NumberOfProcessors;
            /// <summary>
            /// 处理器类型
            /// </summary>
            internal int ProcessorType;
            /// <summary>
            /// 内存分配粒度
            /// </summary>
            internal int AllocationGranularity;
            /// <summary>
            /// 处理器分级数量
            /// </summary>
            internal short ProcessorLevel;
            /// <summary>
            /// 处理器
            /// </summary>
            internal short ProcessorRevision;
        }
        /// <summary>
        /// 扩展内存状态信息MEMORYSTATUEX
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal class memoryStatuExpand
        {
            /// <summary>
            /// 扩展内存状态信息结构体长度
            /// </summary>
            internal uint Length;
            /// <summary>
            /// 
            /// </summary>
            internal uint MemoryLoad;
            /// <summary>
            /// 物理内存大小
            /// </summary>
            internal ulong TotalPhys;
            /// <summary>
            /// 
            /// </summary>
            internal ulong AvailPhys;
            /// <summary>
            /// 
            /// </summary>
            internal ulong TotalPageFile;
            /// <summary>
            /// 
            /// </summary>
            internal ulong AvailPageFile;
            /// <summary>
            /// 
            /// </summary>
            internal ulong TotalVirtual;
            /// <summary>
            /// 
            /// </summary>
            internal ulong AvailVirtual;
            /// <summary>
            /// 
            /// </summary>
            internal ulong AvailExtendedVirtual;
            /// <summary>
            /// 扩展内存状态信息
            /// </summary>
            public memoryStatuExpand()
            {
                Length = (uint)Marshal.SizeOf(typeof(memoryStatuExpand));
            }
        }
        /// <summary>
        /// 基本内存信息MEMORYBASICINFORMATION
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct memoryBasicInformation
        {
            internal unsafe void* BaseAddress;
            internal unsafe void* AllocationBase;
            internal uint AllocationProtect;
            /// <summary>
            /// 区域大小
            /// </summary>
            internal UIntPtr RegionSize;
            /// <summary>
            /// 内存状态
            /// </summary>
            internal uint State;
            internal uint Protect;
            internal uint Type;
        }
        /// <summary>
        /// 安全属性SECURITY_ATTRIBUTES
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal class securityAttributes
        {
            /// <summary>
            /// securityAttributes所占内存大小
            /// </summary>
            public int Size;
            /// <summary>
            /// 安全描述
            /// </summary>
            public safeLocalMemHandle SecurityDescriptor;
            /// <summary>
            /// 是否继承句柄
            /// </summary>
            public bool IsInheritHandle;
            /// <summary>
            /// 安全属性
            /// </summary>
            public securityAttributes()
            {
                Size = Marshal.SizeOf(typeof(securityAttributes));
                SecurityDescriptor = new safeLocalMemHandle(IntPtr.Zero, false);
            }
        }
        /// <summary>
        /// 复制句柄可选参数
        /// </summary>
        [Flags]
        internal enum duplicateHandleOptions : int
        {
            NONE = 0,
            /// <summary>
            /// 关闭源句柄
            /// </summary>
            DUPLICATE_CLOSE_SOURCE = 1,
            /// <summary>
            /// 忽略dwDesiredAccess，设置同样的访问权限
            /// </summary>
            DUPLICATE_SAME_ACCESS = 2
        }
        /// <summary>
        /// 获取标准输入输出句柄类型
        /// </summary>
        internal enum standardHandle : int
        {
            /// <summary>
            /// 标准输入
            /// </summary>
            StandardInput = -10,
            /// <summary>
            /// 标准输出
            /// </summary>
            StandardOutput = -11,
            /// <summary>
            /// 标准错误
            /// </summary>
            StandardError = -12,
        }
    }
}
