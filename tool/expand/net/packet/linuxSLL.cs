using System;

namespace fastCSharp.net.packet
{
    /// <summary>
    /// linuxSLL数据包
    /// </summary>
    public struct linuxSLL
    {
#pragma warning disable
        /// <summary>
        /// 数据包类型
        /// </summary>
        public enum type : ushort
        {
            PacketSentToUs,
            PacketBroadCast,
            PacketMulticast,
            PacketSentToSomeoneElse,
            PacketSentByUs
        }
#pragma warning restore
        /// <summary>
        /// 数据包头部长度
        /// </summary>
        public const int HeaderSize = 16;
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
        /// 数据包类型
        /// </summary>
        public type Type
        {
            get { return (type)(ushort)((uint)data.UnsafeArray[data.StartIndex] << 8) + data.UnsafeArray[data.StartIndex + 1]; }
        }
        /// <summary>
        /// 地址类型
        /// </summary>
        public uint AddressType
        {
            get { return ((uint)data.UnsafeArray[data.StartIndex + 2] << 8) + data.UnsafeArray[data.StartIndex + 3]; }
        }
        /// <summary>
        /// 地址长度
        /// </summary>
        public uint AddressSize
        {
            get { return ((uint)data.UnsafeArray[data.StartIndex + 4] << 8) + data.UnsafeArray[data.StartIndex + 5]; }
        }
        /// <summary>
        /// 地址
        /// </summary>
        public subArray<byte> Address
        {
            get { return subArray<byte>.Unsafe(data.UnsafeArray, data.StartIndex + 6, (int)AddressSize); }
        }
        /// <summary>
        /// 帧类型
        /// </summary>
        public frame Frame
        {
            get
            {
                return (frame)(ushort)(((uint)data.UnsafeArray[data.StartIndex + 14] << 8) + data.UnsafeArray[data.StartIndex + 15]);
            }
        }
        /// <summary>
        /// linuxSLL数据包
        /// </summary>
        /// <param name="data">数据</param>
        public unsafe linuxSLL(subArray<byte> data) : this(ref data) { }
        /// <summary>
        /// linuxSLL数据包
        /// </summary>
        /// <param name="data">数据</param>
        public unsafe linuxSLL(ref subArray<byte> data)
        {
            if (data.Count >= HeaderSize)
            {
                fixed (byte* dataFixed = data.UnsafeArray)
                {
                    byte* start = dataFixed + data.StartIndex;
                    if (data.Count >= ((uint)*(start + 4) << 8) + *(start + 5) + 6)
                    {
                        this.data = data;
                        return;
                    }
                }
            }
            this.data = default(subArray<byte>);
        }
    }
}
