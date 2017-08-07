using System;

namespace fastCSharp.config
{
    /// <summary>
    /// 文件分块相关参数
    /// </summary>
    public sealed class fileBlock
    {
        /// <summary>
        /// 文件分块相关参数
        /// </summary>
        private fileBlock()
        {
            pub.LoadConfig(this);
        }
        /// <summary>
        /// 默认文件分块相关参数
        /// </summary>
        public static readonly fileBlock Default = new fileBlock();
    }
}
