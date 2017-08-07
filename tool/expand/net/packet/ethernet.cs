using System;

namespace fastCSharp.net.packet
{
    /// <summary>
    /// 以太网数据包
    /// </summary>
    public struct ethernet
    {
        /// <summary>
        /// 以太网数据包头部长度
        /// </summary>
        public const int HeaderSize = 14;
        /// <summary>
        /// 数据
        /// </summary>
        private subArray<byte> data;
        /// <summary>
        /// 数据包是否有效
        /// </summary>
        public bool IsPacket
        {
            get { return data.UnsafeArray != null; }
        }
        /// <summary>
        /// 以太网目的地址
        /// </summary>
        public subArray<byte> DestinationMac
        {
            get { return subArray<byte>.Unsafe(data.UnsafeArray, data.StartIndex, 6); }
        }
        /// <summary>
        /// 以太网源地址
        /// </summary>
        public subArray<byte> SourceMac
        {
            get { return subArray<byte>.Unsafe(data.UnsafeArray, data.StartIndex + 6, 6); }
        }
        /// <summary>
        /// 帧类型
        /// </summary>
        public frame Frame
        {
            get
            {
                return (frame)(ushort)(((uint)data.UnsafeArray[data.StartIndex + 12] << 8) + data.UnsafeArray[data.StartIndex + 13]);
            }
        }
        /// <summary>
        /// 以太网数据包
        /// </summary>
        /// <param name="data">数据</param>
        public ethernet(subArray<byte> data) : this(ref data) { }
        /// <summary>
        /// 以太网数据包
        /// </summary>
        /// <param name="data">数据</param>
        public ethernet(ref subArray<byte> data)
        {
            this.data = data.Count >= HeaderSize ? data : default(subArray<byte>);
        }
    }
}
