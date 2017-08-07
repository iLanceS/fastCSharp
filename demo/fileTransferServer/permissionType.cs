using System;

namespace fastCSharp.demo.fileTransferServer
{
    /// <summary>
    /// 权限类型
    /// </summary>
    [Flags]
    public enum permissionType : byte
    {
        /// <summary>
        /// 无权限
        /// </summary>
        None = 0,
        /// <summary>
        /// 列表
        /// </summary>
        List = 1,
        /// <summary>
        /// 读取
        /// </summary>
        Read = 2,
        /// <summary>
        /// 写入
        /// </summary>
        Write = 4,
    }
}
