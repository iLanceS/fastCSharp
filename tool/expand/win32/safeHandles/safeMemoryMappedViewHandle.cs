using System;
using System.Security;
using System.Security.Permissions;

namespace fastCSharp.win32.safeHandles
{
    /// <summary>
    /// 内存映射文件视图
    /// </summary>
    //[SecurityCritical(SecurityCriticalScope.Everything)]
    internal sealed class safeMemoryMappedViewHandle : safeBuffer
    {
        /// <summary>
        /// 内存映射文件视图
        /// </summary>
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        internal safeMemoryMappedViewHandle() : base(true) { }
        /// <summary>
        /// 内存映射文件视图
        /// </summary>
        /// <param name="handle">内存句柄</param>
        /// <param name="ownsHandle">是否拥有句柄</param>
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        internal safeMemoryMappedViewHandle(IntPtr handle, bool ownsHandle)
            : base(ownsHandle)
        {
            base.SetHandle(handle);
        }
        /// <summary>
        /// 释放内存映射文件视图句柄
        /// </summary>
        /// <returns>是否成功</returns>
        protected override bool ReleaseHandle()
        {
            if (kernel32.UnmapViewOfFile(base.handle))
            {
                base.handle = IntPtr.Zero;
                return true;
            }
            return false;
        }
    }
}
