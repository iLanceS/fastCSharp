using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace fastCSharp.net
{
    /// <summary>
    /// ICMP回显
    /// </summary>
    public sealed class ping : IDisposable
    {
        /// <summary>
        /// 发送数据包字节长度
        /// </summary>
        private const int packetSize = 32;
        /// <summary>
        /// PING序列号
        /// </summary>
        private static ushort currentIdentity;
        /// <summary>
        /// 获取下一个PING序列号
        /// </summary>
        private static ushort nextIdentity
        {
            get
            {
                ushort value = currentIdentity--;
                while (--value != currentIdentity) value = currentIdentity--;
                return value;
            }
        }
        /// <summary>
        /// 默认发送数据
        /// </summary>
        private static readonly pointer.reference defaultSendData;
        /// <summary>
        /// 发送数据超时毫秒数
        /// </summary>
        private int sendTimeout;
        /// <summary>
        /// 接收数据超时毫秒数
        /// </summary>
        private int receiveTimeout;
        /// <summary>
        /// 套接字
        /// </summary>
        private Socket socket;
        /// <summary>
        /// 数据缓冲区
        /// </summary>
        private byte[] buffer;
        /// <summary>
        /// 数据效验和
        /// </summary>
        private readonly int checkSum;
        /// <summary>
        /// ICMP回显
        /// </summary>
        /// <param name="sendTimeout">发送数据超时毫秒数</param>
        /// <param name="receiveTimeout">接收数据超时毫秒数</param>
        /// <param name="sendData">自定义发送数据</param>
        public unsafe ping(int sendTimeout, int receiveTimeout, byte[] sendData = null)
        {
            this.sendTimeout = sendTimeout;
            this.receiveTimeout = receiveTimeout;
            buffer = fastCSharp.memoryPool.TinyBuffers.Get();
            fixed (byte* bufferFixed = buffer)
            {
                byte* sendBufferFixed = bufferFixed + (buffer.Length - packetSize);
                sendBufferFixed[0] = 8;
                *(short*)(sendBufferFixed + 4) = (short)System.Diagnostics.Process.GetCurrentProcess().Id;
                int index = 12;
                if (sendData != null)
                {
                    int length = sendData.Length;
                    if (length > (packetSize - 12)) length = (packetSize - 12);
                    fastCSharp.unsafer.memory.Copy(sendData, sendBufferFixed + 12, length);
                    index += length;
                }
                if (index == 12) fastCSharp.unsafer.memory.SimpleCopy(defaultSendData.Byte, sendBufferFixed + 12, (packetSize - 12));
                for (byte* sendBuffer = sendBufferFixed + packetSize; sendBuffer != sendBufferFixed; checkSum += *(ushort*)(sendBuffer -= 2)) ;
            }
        }
        /// <summary>
        /// 创建套接字
        /// </summary>
        private void createSocket()
        {
            if (socket == null)
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
                socket.SendTimeout = sendTimeout;
                socket.ReceiveTimeout = receiveTimeout;
                socket.SendBufferSize = packetSize;
                socket.ReceiveBufferSize = buffer.Length;
                socket.DontFragment = true;
            }
        }
        /// <summary>
        /// PING指定IP
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <returns>是否成功</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe bool Ping(System.Net.IPEndPoint ip)
        {
            return ip != null && isPing((System.Net.EndPoint)ip);
        }
        /// <summary>
        /// PING指定IP
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <returns>是否成功</returns>
        private unsafe bool isPing(System.Net.EndPoint ip)
        {
#pragma warning disable 618
            uint ip4 = (uint)((IPEndPoint)ip).Address.Address;
#pragma warning restore 618
            fixed (byte* bufferFixed = buffer)
            {
                byte* sendBufferFixed = bufferFixed + (buffer.Length - packetSize);
                ushort identity = nextIdentity;
                *(ushort*)(sendBufferFixed + 6) = identity;
                *(uint*)(sendBufferFixed + 8) = ip4;
                int sum = checkSum + identity + (ushort)ip4 + (ushort)(ip4 >> 16);
                sum = (sum >> 16) + (sum & 0xffff);
                sum += (sum >> 16);
                *(ushort*)(sendBufferFixed + 2) = (ushort)(~sum);
                createSocket();
                try
                {
                    if (socket.SendTo(buffer, buffer.Length - packetSize, packetSize, SocketFlags.None, ip) == packetSize
                        && socket.ReceiveFrom(buffer, ref ip) == packetSize + packet.ip.DefaultHeaderSize)
                    {
                        fixed (byte* receiveBufferFixed = buffer)
                        {
                            //*(uint*)(receiveBufferFixed + 0xc) = 0;
                            if (*(receiveBufferFixed + packet.ip.DefaultHeaderSize) == 0 && ip4 == *(int*)(receiveBufferFixed + packet.ip.DefaultHeaderSize + 8) 
                                //&& ip4 == *(uint*)(receiveBufferFixed + 0xc)
                                && unsafer.memory.SimpleEqual(receiveBufferFixed + packet.ip.DefaultHeaderSize + 12, sendBufferFixed + 12, packetSize - 12))
                            {
                                return true;
                            }
                        }
                    }
                }
                catch { }
                fastCSharp.threading.disposeTimer.Default.AddSocketClose(ref socket);
            }
            return false;
        }
        /// <summary>
        /// 释放套接字
        /// </summary>
        public void Dispose()
        {
            fastCSharp.threading.disposeTimer.Default.AddSocketClose(ref socket);
            fastCSharp.memoryPool.TinyBuffers.Push(ref buffer);
        }
        static unsafe ping()
        {
            defaultSendData = unmanaged.GetStatic(packetSize - 12, true).Reference;
            byte* data = defaultSendData.Byte + (packetSize - 13);
            for (int value = packetSize - 12; value != 0; *data-- = (byte)('A' + --value)) ;
        }
    }
}
