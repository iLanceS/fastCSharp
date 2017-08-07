using System;
using System.Net.Sockets;
using fastCSharp.io.compression;
using fastCSharp.threading;
using fastCSharp.code.cSharp;
using System.Text;
using System.Runtime.CompilerServices;

namespace fastCSharp.net.tcp
{
    /// <summary>
    /// TCP调用套接字
    /// </summary>
    public abstract class commandSocket : net.socket
    {
        /// <summary>
        /// 大数据缓存
        /// </summary>
        internal static readonly memoryPool BigBuffers = memoryPool.GetOrCreate(fastCSharp.config.tcpCommand.Default.BigBufferSize);
        /// <summary>
        /// 异步(包括流式)缓冲区
        /// </summary>
        protected internal static readonly memoryPool asyncBuffers = memoryPool.GetOrCreate(fastCSharp.config.tcpCommand.Default.AsyncBufferSize);
        /// <summary>
        /// 发送命令缓冲区
        /// </summary>
        protected byte[] sendData;
        /// <summary>
        /// 接收数据缓冲区
        /// </summary>
        protected internal byte[] receiveData;
        /// <summary>
        /// 当前处理会话标识
        /// </summary>
        protected internal commandServer.streamIdentity identity;
        /// <summary>
        /// 当前处理会话标识
        /// </summary>
        public commandServer.streamIdentity Identity
        {
            get { return identity; }
        }
        /// <summary>
        /// 当前命令参数
        /// </summary>
        protected internal commandServer.commandFlags flags;
        /// <summary>
        /// 当前命令参数
        /// </summary>
        public commandServer.commandFlags Flags
        {
            get { return flags; }
        }
        /// <summary>
        /// 设置当前处理会话标识与命令参数
        /// </summary>
        /// <param name="start"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected internal unsafe void setIdentityFlags(byte* start)
        {
            flags = (commandServer.commandFlags)(*(uint*)start);
            identity = *(commandServer.streamIdentity*)(start + sizeof(int));
        }
        /// <summary>
        /// 客户端标识
        /// </summary>
        public object ClientUserInfo;
        /// <summary>
        /// HTTP页面信息
        /// </summary>
        public tcpBase.httpPage HttpPage { get; internal set; }
        /// <summary>
        /// 是否通过验证方法
        /// </summary>
        internal bool IsVerifyMethod;
        /// <summary>
        /// 默认HTTP内容编码
        /// </summary>
        internal virtual Encoding HttpEncoding { get { return null; } }
        /// <summary>
        /// TCP客户端套接字
        /// </summary>
        /// <param name="socket">TCP套接字</param>
        /// <param name="sendData">发送数据缓冲区</param>
        /// <param name="receiveData">接收数据缓冲区</param>
        /// <param name="isErrorDispose">操作错误是否自动调用析构函数</param>
        protected commandSocket(Socket socket, byte[] sendData, byte[] receiveData, bool isErrorDispose)
            : base(socket, isErrorDispose)
        {
            this.sendData = sendData;
            currentReceiveData = this.receiveData = receiveData;
        }
        ///// <summary>
        ///// TCP客户端套接字
        ///// </summary>
        ///// <param name="sendData">接收数据缓冲区</param>
        ///// <param name="receiveData">发送数据缓冲区</param>
        //protected commandSocket(byte[] sendData, byte[] receiveData)
        //    : base(false)
        //{
        //    this.sendData = sendData;
        //    currentReceiveData = this.receiveData = receiveData;
        //}
        /// <summary>
        /// 关闭套接字连接
        /// </summary>
        protected override void dispose()
        {
            fastCSharp.memoryPool.StreamBuffers.Push(ref sendData);
            fastCSharp.memoryPool.StreamBuffers.Push(ref receiveData);
        }
        /// <summary>
        /// TCP套接字添加到池
        /// </summary>
        internal abstract void PushPool();
    }
    /// <summary>
    /// TCP调用套接字
    /// </summary>
    /// <typeparam name="commandSocketType">TCP调用类型</typeparam>
    public abstract class commandSocket<commandSocketType> : commandSocket
        where commandSocketType : class, IDisposable
    {
        /// <summary>
        /// TCP调用代理
        /// </summary>
        protected internal commandSocketType commandSocketProxy;
        /// <summary>
        /// TCP客户端套接字
        /// </summary>
        /// <param name="socket">TCP套接字</param>
        /// <param name="sendData">发送数据缓冲区</param>
        /// <param name="receiveData">接收数据缓冲区</param>
        /// <param name="commandSocketProxy">TCP调用类型</param>
        /// <param name="isErrorDispose">操作错误是否自动调用析构函数</param>
        protected commandSocket(Socket socket, byte[] sendData, byte[] receiveData, commandSocketType commandSocketProxy, bool isErrorDispose)
            : base(socket, sendData, receiveData, isErrorDispose)
        {
            this.commandSocketProxy = commandSocketProxy;
        }
        ///// <summary>
        ///// TCP客户端套接字
        ///// </summary>
        ///// <param name="receiveData">接收数据缓冲区</param>
        ///// <param name="commandSocketProxy">TCP调用类型</param>
        //protected commandSocket(byte[] receiveData, commandSocketType commandSocketProxy)
        //    : base(receiveData)
        //{
        //    this.commandSocketProxy = commandSocketProxy;
        //}
    }
}
