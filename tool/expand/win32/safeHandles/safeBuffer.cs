using System;
using System.Security;
using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace fastCSharp.win32.safeHandles
{
    /// <summary>
    /// 安全缓冲区
    /// </summary>
    [SecurityCritical]
    internal abstract class safeBuffer : SafeHandleZeroOrMinusOneIsInvalid
    {
        /// <summary>
        /// 缓冲区长度错误值
        /// </summary>
        private static readonly UIntPtr errorLength = UIntPtr.Size == 4 ? (UIntPtr)uint.MaxValue : (UIntPtr)ulong.MaxValue;
        /// <summary>
        /// 缓冲区长度
        /// </summary>
        private UIntPtr bufferLength;
        /// <summary>
        /// 安全缓冲区
        /// </summary>
        /// <param name="ownsHandle">是否拥有句柄</param>
        protected safeBuffer(bool ownsHandle)
            : base(ownsHandle)
        {
            bufferLength = errorLength;
        }
        /// <summary>
        /// 设置缓冲区长度
        /// </summary>
        /// <param name="bufferLength">缓冲区长度</param>
        public void Initialize(ulong bufferLength)
        {
            this.bufferLength = (UIntPtr)bufferLength;
        }
        /// <summary>
        /// 获取缓冲区指针
        /// </summary>
        /// <returns>缓冲区指针</returns>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public unsafe byte* AcquirePointer()
        {
            bool success = false;
            base.DangerousAddRef(ref success);
            return success ? (byte*)base.handle : null;
        }
        /// <summary>
        /// 释放缓冲区指针
        /// </summary>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public void ReleasePointer()
        {
            base.DangerousRelease();
        }
    }
}