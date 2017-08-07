using System;
using System.Security;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace fastCSharp.win32.safeHandles
{
    /// <summary>
    /// 内存映射文件
    /// </summary>
    //[SecurityCritical(SecurityCriticalScope.Everything)]
    internal sealed class safeMemoryMappedFileHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        /// <summary>
        /// 内存映射文件
        /// </summary>
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        internal safeMemoryMappedFileHandle() : base(true) { }
        /// <summary>
        /// 内存映射文件
        /// </summary>
        /// <param name="handle">内存句柄</param>
        /// <param name="ownsHandle">是否拥有句柄</param>
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        internal safeMemoryMappedFileHandle(IntPtr handle, bool ownsHandle)
            : base(ownsHandle) { base.SetHandle(handle); }
        /// <summary>
        /// 释放句柄
        /// </summary>
        /// <returns>是否成功</returns>
        protected override bool ReleaseHandle()
        {
            return kernel32.CloseHandle(base.handle);
        }
    }
}
