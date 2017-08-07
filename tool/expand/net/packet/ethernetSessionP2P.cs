using System;

namespace fastCSharp.net.packet
{
    /// <summary>
    /// 以太网会话点到点协议数据包
    /// </summary>
    public struct ethernetSessionP2P
    {
#pragma warning disable
        /// <summary>
        /// 以太网会话点到点协议数据包代码
        /// </summary>
        public enum code : byte
        {
            ActiveDiscoveryInitiation = 9,
            ActiveDiscoveryOffer = 7,
            ActiveDiscoveryTerminate = 0xa7,
            SessionStage = 0
        }
        /// <summary>
        /// 点到点协议
        /// </summary>
        public enum protocol : byte
        {
            IPv4 = 0x21,
            IPv6 = 0x57,
            Padding = 1
        }
#pragma warning restore
        /// <summary>
        /// 数据包头长度
        /// </summary>
        public const int HeaderSize = 8;
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
        /// 版本号
        /// </summary>
        public int Version
        {
            get { return (data.UnsafeArray[data.StartIndex] >> 4) & 240; }
        }
        /// <summary>
        /// 类型
        /// </summary>
        public int Type
        {
            get { return data.UnsafeArray[data.StartIndex] & 15; }
        }
        /// <summary>
        /// 代码类型
        /// </summary>
        public code Code
        {
            get { return (code)data.UnsafeArray[data.StartIndex + 1]; }
        }
        /// <summary>
        /// 标识
        /// </summary>
        public ushort SessionId
        {
            get { return fastCSharp.unsafer.memory.GetUShort(data.UnsafeArray, data.StartIndex + 2); }
        }
        /// <summary>
        /// 数据包长度(单位未知)
        /// </summary>
        public uint packetSize
        {
            get { return ((uint)data.UnsafeArray[data.StartIndex + 4] << 8) + data.UnsafeArray[data.StartIndex + 5]; }
        }
        /// <summary>
        /// 帧类型
        /// </summary>
        public frame Frame
        {
            get
            {
                if (data.UnsafeArray[data.StartIndex + 6] == 0)
                {
                    switch (data.UnsafeArray[data.StartIndex + 7])
                    {
                        case (byte)protocol.IPv4:
                            return frame.IpV4;
                        case (byte)protocol.IPv6:
                            return frame.IpV6;
                    }
                }
                return frame.None;
            }
        }
        /// <summary>
        /// 以太网会话点到点协议数据包
        /// </summary>
        /// <param name="data">数据</param>
        public unsafe ethernetSessionP2P(subArray<byte> data) : this(ref data) { }
        /// <summary>
        /// 以太网会话点到点协议数据包
        /// </summary>
        /// <param name="data">数据</param>
        public unsafe ethernetSessionP2P(ref subArray<byte> data)
        {
            if (data.Count >= HeaderSize)
            {
                fixed (byte* dataFixed = data.UnsafeArray)
                {
                    byte* start = dataFixed + data.StartIndex;
                    uint packetSize = ((uint)*(start + 4) << 8) + *(start + 5);
                    if (data.Count >= packetSize)
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
