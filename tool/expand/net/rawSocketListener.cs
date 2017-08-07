using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using fastCSharp.threading;

namespace fastCSharp.net
{
    /// <summary>
    /// 原始套接字监听
    /// </summary>
    public sealed class rawSocketListener : socket
    {
        /// <summary>
        /// 默认获取的数据包的字节数(默认为以太网)
        /// </summary>
        private const int defaultBufferSize = 1500;
        /// <summary>
        /// 输入参数
        /// </summary>
        private static readonly byte[] optionIn = new byte[4];
        /// <summary>
        /// 缓冲区
        /// </summary>
        private static readonly memoryPool buffers = memoryPool.GetOrCreate(fastCSharp.config.pub.Default.RawSocketBufferSize);
        /// <summary>
        /// 数据包处理委托
        /// </summary>
        private Action<subArray<byte>> getPacket;
        /// <summary>
        /// 数据包字节数
        /// </summary>
        private int packetSize;
        /// <summary>
        /// 缓冲区最大可用索引
        /// </summary>
        private int maxBufferIndex;
        /// <summary>
        /// 当前接收数据缓冲区
        /// </summary>
        private byte[] buffer;
        /// <summary>
        /// 缓冲区起始位置
        /// </summary>
        private int bufferIndex;
        /// <summary>
        /// 接收数据序号
        /// </summary>
        private int receiveIdentity;
        /// <summary>
        /// 数据包处理序号
        /// </summary>
        private volatile int packetIdentity;
        /// <summary>
        /// 异步套接字操作
        /// </summary>
        private SocketAsyncEventArgs async;
        /// <summary>
        /// 初始化原始套接字监听
        /// </summary>
        /// <param name="getPacket">数据包处理委托</param>
        /// <param name="ipAddress">监听地址</param>
        /// <param name="packetSize">数据包字节数</param>
        public unsafe rawSocketListener(Action<subArray<byte>> getPacket, IPAddress ipAddress, int packetSize = defaultBufferSize)
            : base(true)
        {
            if (getPacket == null) log.Error.Throw(log.exceptionType.Null);
            if (packetSize <= 0 || packetSize > buffers.Size - sizeof(int)) log.Error.Throw(log.exceptionType.IndexOutOfRange);
            try
            {
                //在发送时必须提供完整的IP标头，所接收的数据报在返回时会保持其IP标头和选项不变。
                Socket = new Socket(ipAddress.AddressFamily, SocketType.Raw, ipAddress.AddressFamily == AddressFamily.InterNetworkV6 ? ProtocolType.IPv6 : ProtocolType.IP);
                Socket.Blocking = false;
                Socket.Bind(new IPEndPoint(ipAddress, 0));
                Socket.SetSocketOption(ipAddress.AddressFamily == AddressFamily.InterNetworkV6 ? SocketOptionLevel.IPv6 : SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);
                fixed (byte* optionInFixed = optionIn)
                {
                    Monitor.Enter(optionIn);
                    try
                    {
                        *(int*)optionInFixed = 1;
                        Socket.IOControl(IOControlCode.ReceiveAll, optionIn, null);
                    }
                    finally { Monitor.Exit(optionIn); }
                }
                this.getPacket = getPacket;
                this.packetSize = packetSize;
                maxBufferIndex = buffers.Size - packetSize;
                fastCSharp.threading.threadPool.TinyPool.Start(start);
                return;
            }
            catch (Exception error)
            {
                log.Error.Add(lastException = error, "监听初始化失败", false);
            }
            Dispose();
        }
        /// <summary>
        /// 开始监听
        /// </summary>
        private void start()
        {
            async = new SocketAsyncEventArgs();
            async.SocketFlags = System.Net.Sockets.SocketFlags.None;
            async.DisconnectReuseSocket = false;
            async.UserToken = this;
            async.Completed += onReceive;
            receive();
        }
        /// <summary>
        /// 继续接收数据
        /// </summary>
        /// <returns>是否接收成功</returns>
        private unsafe void receive()
        {
            if (isDisposed == 0)
            {
                try
                {
                    if (this.buffer == null)
                    {
                        byte[] buffer = buffers.Get();
                        bufferIndex = sizeof(int);
                        fixed (byte* bufferFixed = buffer) *(int*)bufferFixed = 1;
                        this.buffer = buffer;
                    }
                    async.SetBuffer(this.buffer, bufferIndex, packetSize);
                    if (Socket.ReceiveAsync(async)) return;
                    if (async.SocketError != SocketError.Success) socketError = async.SocketError;
                }
                catch (Exception error)
                {
                    lastException = error;
                }
                Dispose();
            }
        }
        /// <summary>
        /// 数据接收完成后的回调委托
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="async">异步回调参数</param>
        private unsafe void onReceive(object sender, SocketAsyncEventArgs async)
        {
            int isSuccess = 0;
            try
            {
                if (async.SocketError == SocketError.Success)
                {
                    int count = async.BytesTransferred;
                    if (count == 0) receive();
                    else
                    {
                        subArray<byte> packet = subArray<byte>.Unsafe(buffer, bufferIndex, count);
                        bufferIndex += (count + 3) & (int.MaxValue - 3);
                        int receiveIdentity = this.receiveIdentity++;
                        if (bufferIndex <= maxBufferIndex || Interlocked.Exchange(ref buffer, null) == null)
                        {
                            receive();
                            Thread.Sleep(0);
                            fixed (byte* bufferFixed = packet.UnsafeArray) Interlocked.Increment(ref *(int*)bufferFixed);
                        }
                        else
                        {
                            receive();
                            Thread.Sleep(0);
                        }
                        while (packetIdentity != receiveIdentity)
                        {
                            Thread.Sleep(1);
                            if (packetIdentity != receiveIdentity) Thread.Sleep(0);
                        }
                        isSuccess = 1;
                        try
                        {
                            getPacket(packet);
                        }
                        finally { ++packetIdentity; }
                    }
                    return;
                }
                socketError = async.SocketError;
            }
            catch (Exception error)
            {
                if (isSuccess == 0) lastException = error;
                else log.Error.Add(error, null, false);
            }
            if (isSuccess == 0) Dispose();
        }
        /// <summary>
        /// 释放缓冲区
        /// </summary>
        /// <param name="buffer">缓冲区</param>
        public static unsafe void FreeOnly(ref subArray<byte> buffer)
        {
            if (buffer.UnsafeArray.length() == buffers.Size)
            {
                fixed (byte* bufferFixed = buffer.UnsafeArray)
                {
                    if (Interlocked.Decrement(ref *(int*)bufferFixed) == 0) buffers.PushNotNull(buffer.UnsafeArray);
                }
            }
        }
        /// <summary>
        /// 关闭套接字
        /// </summary>
        protected unsafe override void close()
        {
            base.close();
            SocketAsyncEventArgs async = Interlocked.Exchange(ref this.async, null);
            if (async != null)
            {
                async.Completed -= onReceive;
#if NOEXPAND
                socketAsyncEventArgs.Push(ref async);
#else
                socketAsyncEventArgsProxy.Push(ref async);
#endif
            }
            byte[] buffer = Interlocked.Exchange(ref this.buffer, null);
            fixed (byte* bufferFixed = buffer)
            {
                if (Interlocked.Decrement(ref *(int*)bufferFixed) == 0) buffers.PushNotNull(buffer);
            }
        }
    }
}
