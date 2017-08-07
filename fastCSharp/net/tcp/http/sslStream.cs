using System;
using System.Net.Sockets;
using System.Net.Security;
using System.Net;
using fastCSharp.io;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
using System.Threading;
using System.Runtime.CompilerServices;
using fastCSharp.code.cSharp;

namespace fastCSharp.net.tcp.http
{
    /// <summary>
    /// HTTP安全流
    /// </summary>
    internal sealed class sslStream : socketBase
    {
        /// <summary>
        /// TCP服务端口证书信息
        /// </summary>
        internal struct certificate
        {
            /// <summary>
            /// TCP服务端口信息
            /// </summary>
            public host Host;
            /// <summary>
            /// 安全证书文件
            /// </summary>
            public string FileName;
#pragma warning disable 649
            /// <summary>
            /// 安全证书文件密码
            /// </summary>
            public string Password;
            /// <summary>
            /// 协议
            /// </summary>
            internal SslProtocols Protocol;
#pragma warning restore 649
            /// <summary>
            /// 安全证书 
            /// </summary>
            internal X509Certificate Certificate;
            /// <summary>
            /// 获取安全证书
            /// </summary>
            /// <param name="protocol"></param>
            /// <returns></returns>
            internal X509Certificate Get(out SslProtocols protocol)
            {
                if (Certificate == null)
                {
                    if (FileName != null)
                    {
                        if (File.Exists(FileName))
                        {
                            try
                            {
                                protocol = Protocol;
                                return Certificate = Password == null ? X509Certificate2.CreateFromCertFile(FileName) : new X509Certificate2(File.ReadAllBytes(FileName), Password);
                            }
                            catch (Exception error)
                            {
                                fastCSharp.log.Error.Add(error, null, false);
                            }
                        }
                        else fastCSharp.log.Error.Add("没有找到安全证书文件 [" + Host.Host + ":" + Host.Port.toString() + "] " + FileName, new System.Diagnostics.StackFrame(), false);
                        FileName = null;
                    }
                }
                protocol = Protocol;
                return Certificate;
            }
        }
        /// <summary>
        /// HTTP头部接收器
        /// </summary>
        internal new sealed class headerReceiver : headerReceiver<sslStream>
        {
            /// <summary>
            /// 接受头部换行数据
            /// </summary>
            private AsyncCallback receiveCallback;
            /// <summary>
            /// HTTP头部接收器
            /// </summary>
            /// <param name="sslStream">HTTP安全流</param>
            public headerReceiver(sslStream sslStream)
                : base(sslStream)
            {
                receiveCallback = receive;
            }
            /// <summary>
            /// 接受头部换行数据
            /// </summary>
            protected override void receive()
            {
                try
                {
                    int timeoutIdentity = socket.timeoutIdentity;
                    socket.SslStream.BeginRead(buffer, ReceiveEndIndex, socketBase.HeaderBufferLength - ReceiveEndIndex, receiveCallback, this);
                    if (socket.isNextRequest == 0 || ReceiveEndIndex != 0) socket.setTimeout(timeoutIdentity);
                    else socket.setKeepAliveTimeout(timeoutIdentity);
                    return;
                }
                catch (Exception error)
                {
                    log.Default.Add(error, null, false);
                }
                socket.headerError();
            }
            /// <summary>
            /// 接受头部换行数据
            /// </summary>
            /// <param name="result">异步操作状态</param>
            private unsafe void receive(IAsyncResult result)
            {
                ++socket.timeoutIdentity;
                int count = 0;
                try
                {
                    if ((count = socket.SslStream.EndRead(result)) > 0)
                    {
                        ReceiveEndIndex += count;
                        if (isSecondCount == 0)
                        {
                            isSecondCount = 1;
                            secondCount.Add();
                        }
                    }
                }
                catch (Exception error)
                {
                    if (socket.isTimeout == 0) log.Default.Add(error, null, false);
                }
                if (count <= 0 || date.nowTime.Now >= timeout) socket.headerError();
                else onReceive();
            }
        }
        /// <summary>
        /// 表单数据接收器
        /// </summary>
        private new sealed class formIdentityReceiver : formIdentityReceiver<sslStream>
        {
            /// <summary>
            /// 接收表单数据处理
            /// </summary>
            private AsyncCallback receiveCallback;
            /// <summary>
            /// HTTP表单接收器
            /// </summary>
            /// <param name="socket">HTTP套接字</param>
            public formIdentityReceiver(sslStream socket)
                : base(socket)
            {
                receiveCallback = receive;
            }
            /// <summary>
            /// 开始接收表单数据
            /// </summary>
            /// <param name="loadForm">HTTP请求表单加载接口</param>
            public void Receive(requestForm.ILoadForm loadForm)
            {
                headerReceiver headerReceiver = socket.HeaderReceiver;
                requestHeader requestHeader = headerReceiver.RequestHeader;
                this.loadForm = loadForm;
                contentLength = requestHeader.ContentLength;
                if (contentLength < socket.Buffer.Length)
                {
                    buffer = socket.Buffer;
                    memoryPool = null;
                }
                else
                {
                    memoryPool = getMemoryPool(contentLength + 1);
                    buffer = memoryPool.Get(contentLength + 1);
                }
                receiveEndIndex = headerReceiver.ReceiveEndIndex - headerReceiver.HeaderEndIndex - sizeof(int);
                System.Buffer.BlockCopy(requestHeader.Buffer, headerReceiver.HeaderEndIndex + sizeof(int), buffer, 0, receiveEndIndex);
                headerReceiver.ReceiveEndIndex = headerReceiver.HeaderEndIndex;

                if (receiveEndIndex == contentLength) callback();
                else
                {
                    receiveStartTime = date.nowTime.Now.AddTicks(date.SecondTicks);
                    receive();
                }
            }
            /// <summary>
            /// 开始接收表单数据
            /// </summary>
            private void receive()
            {
                try
                {
                    int timeoutIdentity = socket.timeoutIdentity;
                    socket.SslStream.BeginRead(buffer, receiveEndIndex, contentLength - receiveEndIndex, receiveCallback, this);
                    socket.setTimeout(timeoutIdentity);
                    return;
                }
                catch (Exception error)
                {
                    log.Default.Add(error, null, false);
                }
                receiveError();
            }
            /// <summary>
            /// 接收表单数据处理
            /// </summary>
            /// <param name="result">异步操作状态</param>
            private unsafe void receive(IAsyncResult result)
            {
                ++socket.timeoutIdentity;
                int count = 0;
                try
                {
                    if ((count = socket.SslStream.EndRead(result)) > 0)
                    {
                        receiveEndIndex += count;
                    }
                }
                catch (Exception error)
                {
                    if (socket.isTimeout == 0) log.Default.Add(error, null, false);
                }
                if (count <= 0) receiveError();
                else if (receiveEndIndex == contentLength) callback();
                else if (date.nowTime.Now > receiveStartTime && receiveEndIndex < minReceiveSizePerSecond4 * ((int)(date.nowTime.Now - receiveStartTime).TotalSeconds >> 2)) receiveError();
                else receive();
            }
        }
        /// <summary>
        /// 数据接收器
        /// </summary>
        private new sealed class boundaryIdentityReceiver : boundaryIdentityReceiver<sslStream>
        {
            /// <summary>
            /// 接收表单数据处理
            /// </summary>
            private AsyncCallback receiveCallback;
            /// <summary>
            /// HTTP数据接收器
            /// </summary>
            /// <param name="socket">HTTP套接字</param>
            public boundaryIdentityReceiver(sslStream socket)
                : base(socket)
            {
                receiveCallback = receive;
            }
            /// <summary>
            /// 开始接收表单数据
            /// </summary>
            /// <param name="loadForm">HTTP请求表单加载接口</param>
            public void Receive(requestForm.ILoadForm loadForm)
            {
                this.loadForm = loadForm;
                try
                {
                    headerReceiver headerReceiver = socket.HeaderReceiver;
                    requestHeader requestHeader = headerReceiver.RequestHeader;
                    Buffer = bigBuffers.Get();
                    boundary = requestHeader.Boundary;
                    receiveLength = receiveEndIndex = headerReceiver.ReceiveEndIndex - headerReceiver.HeaderEndIndex - sizeof(int);
                    System.Buffer.BlockCopy(requestHeader.Buffer, headerReceiver.HeaderEndIndex + sizeof(int), Buffer, 0, receiveEndIndex);
                    headerReceiver.ReceiveEndIndex = headerReceiver.HeaderEndIndex;
                    contentLength = requestHeader.ContentLength;

                    receiveStartTime = date.nowTime.Now.AddTicks(date.SecondTicks);
                    onFirstBoundary();
                    return;
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                this.error();
            }
            /// <summary>
            /// 开始接收表单数据
            /// </summary>
            protected override void receive()
            {
                try
                {
                    int timeoutIdentity = socket.timeoutIdentity;
                    socket.SslStream.BeginRead(Buffer, receiveEndIndex, bigBuffers.Size - receiveEndIndex - sizeof(int), receiveCallback, this);
                    socket.setTimeout(timeoutIdentity);
                    return;
                }
                catch (Exception error)
                {
                    log.Default.Add(error, null, false);
                }
                this.error();
            }
            /// <summary>
            /// 接收表单数据处理
            /// </summary>
            /// <param name="result">异步操作状态</param>
            private unsafe void receive(IAsyncResult result)
            {
                ++socket.timeoutIdentity;
                int count = 0;
                try
                {
                    if ((count = socket.SslStream.EndRead(result)) > 0)
                    {
                        receiveEndIndex += count;
                        receiveLength += count;
                    }
                }
                catch (Exception error)
                {
                    if (socket.isTimeout == 0) log.Default.Add(error, null, false);
                }
                if (count <= 0 || receiveLength > contentLength
                    || (date.nowTime.Now > receiveStartTime && receiveLength < minReceiveSizePerSecond4 * ((int)(date.nowTime.Now - receiveStartTime).TotalSeconds >> 2)))
                {
                    error();
                }
                else callOnReceiveData();
            }
        }
        /// <summary>
        /// 数据发送器
        /// </summary>
        private new sealed class dataSender : dataSender<sslStream>
        {
            /// <summary>
            /// 发送数据处理
            /// </summary>
            private AsyncCallback sendCallback;
            /// <summary>
            /// 发送文件数据处理
            /// </summary>
            private AsyncCallback sendFileCallback;
            /// <summary>
            /// 当前发送字节数
            /// </summary>
            private int sendSize;
            /// <summary>
            /// 数据发送器
            /// </summary>
            /// <param name="socket">HTTP套接字</param>
            public dataSender(sslStream socket)
                : base(socket)
            {
                sendCallback = send;
                sendFileCallback = sendFile;
            }
            /// <summary>
            /// 发送数据
            /// </summary>
            /// <param name="onSend">发送数据回调处理</param>
            /// <param name="buffer">发送数据缓冲区</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Send(onSendType onSend, subArray<byte> buffer)
            {
                Send(onSend, ref buffer, null);
            }
            /// <summary>
            /// 发送数据
            /// </summary>
            /// <param name="onSend">发送数据回调处理</param>
            /// <param name="buffer">发送数据缓冲区</param>
            /// <param name="memoryPool">发送数据缓冲区内存池</param>
            public void Send(onSendType onSend, ref subArray<byte> buffer, memoryPool memoryPool)
            {
                this.onSend = onSend;
                sendStartTime = date.nowTime.Now.AddTicks(date.SecondTicks);
                this.memoryPool = memoryPool;
                this.buffer = buffer.array;
                sendIndex = buffer.startIndex;
                sendLength = 0;
                sendEndIndex = sendIndex + buffer.length;

                send();
            }
            /// <summary>
            /// 开始发送数据
            /// </summary>
            private void send()
            {
                try
                {
                    int timeoutIdentity = socket.timeoutIdentity;
                    socket.SslStream.BeginWrite(buffer, sendIndex, sendSize = Math.Min(sendEndIndex - sendIndex, net.socket.MaxServerSendSize), sendCallback, this);
                    socket.setTimeout(timeoutIdentity);
                    return;
                }
                catch (Exception error)
                {
                    log.Default.Add(error, null, false);
                }
                send(false);
            }
            /// <summary>
            /// 发送数据处理
            /// </summary>
            /// <param name="result"></param>
            private unsafe void send(IAsyncResult result)
            {
                ++socket.timeoutIdentity;
                try
                {
                    socket.SslStream.EndWrite(result);
                    sendIndex += sendSize;
                    sendLength += sendSize;
                }
                catch (Exception error)
                {
                    send(false);
                    if (socket.isTimeout == 0) log.Default.Add(error, null, false);
                    return;
                }
                if (sendIndex == sendEndIndex) send(true);
                else if (date.nowTime.Now > sendStartTime && sendLength < minReceiveSizePerSecond4 * ((int)(date.nowTime.Now - sendStartTime).TotalSeconds >> 2)) send(false);
                else send();
            }
            /// <summary>
            /// 发送文件数据
            /// </summary>
            /// <param name="onSend">发送数据回调处理</param>
            /// <param name="fileName">文件名称</param>
            /// <param name="seek">起始位置</param>
            /// <param name="size">发送字节长度</param>
            public void SendFile(onSendType onSend, string fileName, long seek, long size)
            {
                try
                {
                    fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, fastCSharp.config.appSetting.StreamBufferSize, FileOptions.SequentialScan);
                    if (fileStream.Length >= seek + size)
                    {
                        if (seek != 0) fileStream.Seek(seek, SeekOrigin.Begin);
                        this.onSend = onSend;
                        sendStartTime = date.nowTime.Now.AddTicks(date.SecondTicks);
                        fileSize = size;
                        sendLength = 0;

                        memoryPool = net.socket.ServerSendBuffers;
                        buffer = memoryPool.Get();
                        readFile();
                        return;
                    }
                    else fileStream.Dispose();
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                onSendFalse(onSend);
            }
            /// <summary>
            /// 开始发送文件数据
            /// </summary>
            protected override void sendFile()
            {
                try
                {
                    int timeoutIdentity = socket.timeoutIdentity;
                    socket.SslStream.BeginWrite(buffer, sendIndex, sendSize = sendEndIndex - sendIndex, sendFileCallback, this);
                    socket.setTimeout(timeoutIdentity);
                    return;
                }
                catch (Exception error)
                {
                    log.Default.Add(error, null, false);
                }
                sendFile(false);
            }
            /// <summary>
            /// 发送文件数据处理
            /// </summary>
            /// <param name="result">异步操作状态</param>
            private unsafe void sendFile(IAsyncResult result)
            {
                ++socket.timeoutIdentity;
                try
                {
                    socket.SslStream.EndWrite(result);
                    sendIndex += sendSize;
                    sendLength += sendSize;
                }
                catch (Exception error)
                {
                    sendFile(false);
                    if (socket.isTimeout == 0) log.Default.Add(error, null, false);
                    return;
                }
                if (sendIndex == sendEndIndex) readFile();
                else if (date.nowTime.Now > sendStartTime && sendLength < minReceiveSizePerSecond4 * ((int)(date.nowTime.Now - sendStartTime).TotalSeconds >> 2)) sendFile(false);
                else sendFile();
            }
        }
        /// <summary>
        /// WebSocket请求接收器
        /// </summary>
        private new unsafe sealed class webSocketIdentityReceiver : webSocketIdentityReceiver<sslStream>
        {
            /// <summary>
            /// WebSocket请求数据处理
            /// </summary>
            private AsyncCallback receiveCallback;
            /// <summary>
            /// WebSocket请求接收器
            /// </summary>
            /// <param name="socket">HTTP套接字</param>
            public webSocketIdentityReceiver(sslStream socket)
                : base(socket)
            {
                receiveCallback = receive;
            }
            /// <summary>
            /// 开始接收请求数据
            /// </summary>
            /// <param name="webSocket"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Receive(webSocket.socket webSocket)
            {
                this.WebSocket = webSocket;
                webSocket.SslStream = socket.SslStream;
                webSocket.Parser.Buffer = buffer;
                receiveEndIndex = 0;
                receive();
            }
            /// <summary>
            /// 开始接收数据
            /// </summary>
            protected override void receive()
            {
                try
                {
                    int timeoutIdentity = socket.timeoutIdentity;
                    socket.SslStream.BeginRead(buffer, receiveEndIndex, socketBase.HeaderBufferLength - receiveEndIndex, receiveCallback, this);
                    socket.setWebSocketTimeout(timeoutIdentity);
                    return;
                }
                catch (Exception error)
                {
                    log.Default.Add(error, null, false);
                }
                socket.webSocketEnd();
            }
            /// <summary>
            /// WebSocket请求数据处理
            /// </summary>
            /// <param name="result">异步调用状态</param>
            private void receive(IAsyncResult result)
            {
                ++socket.timeoutIdentity;
                int count = int.MinValue;
                try
                {
                    if ((count = socket.SslStream.EndRead(result)) > 0)
                    {
                        receiveEndIndex += count;
                    }
                }
                catch (Exception error)
                {
                    if (socket.isTimeout == 0) log.Default.Add(error, null, false);
                }
                if (count <= 0) socket.webSocketEnd();
                //else if (date.NowSecond >= timeout)
                //{
                //    Monitor.Enter(sendLock);
                //    try
                //    {
                //        socket.SslStream.Write(closeData, 0, 2);
                //    }
                //    catch { }
                //    finally
                //    {
                //        Monitor.Exit(sendLock);
                //        socket.webSocketEnd();
                //    }
                //}
                //else if (count == 0) fastCSharp.threading.timerTask.Default.Add(receiveHandle, date.NowSecond.AddSeconds(1), null);
                else if ((receiveEndIndex = WebSocket.Parser.Parse(receiveEndIndex)) >= 0) receive();
                else socket.webSocketEnd();
            }
            /// <summary>
            /// 发送消息
            /// </summary>
            /// <param name="webSocket"></param>
            internal unsafe void Send(webSocket.socket webSocket)
            {
                byte[] buffer = fastCSharp.memoryPool.StreamBuffers.Get();
                webSocket.socket.message message = default(webSocket.socket.message);
                try
                {
                    fixed (byte* bufferFixed = buffer)
                    {
                        using (unmanagedStream stream = new unmanagedStream(bufferFixed, buffer.Length))
                        {
                            sender sender = new sender { Buffer = buffer, BufferFixed = bufferFixed, Stream = stream };
                            do
                            {
                                if (webSocket == this.WebSocket && webSocket.SocketIdentity == socket.identity)
                                {
                                    if (webSocket.GetMessage(ref message))
                                    {
                                        sender.Send(ref message);
                                        if ((stream.Length << 1) > buffer.Length)
                                        {
                                            byte[] data = sender.GetData();
                                            Monitor.Enter(sendLock);
                                            try
                                            {
                                                webSocket.SslStream.Write(data, 0, stream.Length);
                                                ++socket.timeoutIdentity;
                                            }
                                            finally { Monitor.Exit(sendLock); }
                                            stream.UnsafeSetLength(0);
                                        }
                                        else if (webSocket.MessageCount == 0) Thread.Sleep(0);
                                    }
                                    else
                                    {
                                        if (stream.Length != 0)
                                        {
                                            byte[] data = sender.GetData();
                                            Monitor.Enter(sendLock);
                                            try
                                            {
                                                webSocket.SslStream.Write(data, 0, stream.Length);
                                                ++socket.timeoutIdentity;
                                            }
                                            finally { Monitor.Exit(sendLock); }
                                        }
                                        return;
                                    }
                                }
                                else break;
                            }
                            while (true);
                        }
                    }
                }
                catch { }
                finally { fastCSharp.memoryPool.StreamBuffers.PushNotNull(buffer); }
                webSocket.Client.shutdown();
            }
        }
        /// <summary>
        /// HTTP头部接收器
        /// </summary>
        internal headerReceiver HeaderReceiver;
        /// <summary>
        /// 表单数据接收器
        /// </summary>
        private formIdentityReceiver formReceiver;
        /// <summary>
        /// 数据接收器
        /// </summary>
        private boundaryIdentityReceiver boundaryReceiver;
        /// <summary>
        /// 数据发送器
        /// </summary>
        private dataSender sender;
        /// <summary>
        /// WebSocket请求接收器
        /// </summary>
        private webSocketIdentityReceiver webSocketReceiver;
        /// <summary>
        /// 身份验证完成处理
        /// </summary>
        private AsyncCallback authenticateCallback;
        /// <summary>
        /// 网络流
        /// </summary>
        private NetworkStream networkStream;
        /// <summary>
        /// 安全网络流
        /// </summary>
        internal SslStream SslStream;
        /// <summary>
        /// 是否SSL链接
        /// </summary>
        internal override bool IsSsl { get { return true; } }
        /// <summary>
        /// HTTP安全流
        /// </summary>
        private sslStream()
        {
            RequestHeader = (HeaderReceiver = new headerReceiver(this)).RequestHeader;
            sender = new dataSender(this);
            authenticateCallback = onAuthenticate;
        }
        /// <summary>
        /// 开始处理新的请求
        /// </summary>
        private void start()
        {
            try
            {
                SslStream = new SslStream(networkStream = new NetworkStream(Socket, true), false);
                SslStream.BeginAuthenticateAsServer(server.Certificate, false, server.Protocol, false, authenticateCallback, this);
                return;
            }
            catch (Exception error)
            {
                log.Default.Add(error, null, false);
            }
            headerError();
        }
        /// <summary>
        /// 开始处理新的请求
        /// </summary>
        /// <param name="server">HTTP服务</param>
        /// <param name="socket">套接字</param>
        /// <param name="ip"></param>
        internal void Start(server server, Socket socket, ref ipv6Hash ip)
        {
            Ipv6 = ip;
            Ipv4 = 0;
            this.server = server;
            servers = server.Servers;
            Socket = socket;
            try
            {
                SslStream = new SslStream(networkStream = new NetworkStream(socket, true), false);
                SslStream.BeginAuthenticateAsServer(server.Certificate, false, server.Protocol, false, authenticateCallback, this);
                return;
            }
            catch (Exception error)
            {
                log.Default.Add(error, null, false);
            }
            headerError();
        }
        /// <summary>
        /// 开始处理新的请求
        /// </summary>
        /// <param name="server">HTTP服务</param>
        /// <param name="socket">套接字</param>
        /// <param name="ip"></param>
        internal void Start(server server, Socket socket, int ip)
        {
            Ipv4 = ip;
            this.server = server;
            servers = server.Servers;
            Socket = socket;
            try
            {
                SslStream = new SslStream(networkStream = new NetworkStream(socket, true), false);
                SslStream.BeginAuthenticateAsServer(server.Certificate, false, server.Protocol, false, authenticateCallback, this);
                return;
            }
            catch (Exception error)
            {
                log.Default.Add(error, null, false);
            }
            headerError();
        }
        /// <summary>
        /// 开始处理新的请求
        /// </summary>
        /// <param name="server">HTTP服务</param>
        /// <param name="socket">套接字</param>
        internal void Start(server server, Socket socket)
        {
            this.server = server;
            servers = server.Servers;
            Socket = socket;
            try
            {
                SslStream = new SslStream(networkStream = new NetworkStream(socket, true), false);
                SslStream.BeginAuthenticateAsServer(server.Certificate, false, server.Protocol, false, authenticateCallback, this);
                return;
            }
            catch (Exception error)
            {
                log.Default.Add(error, null, false);
            }
            headerError();
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            if (Interlocked.Increment(ref isDisposed) == 1) Interlocked.Decrement(ref newSslCount);
            base.Dispose();
        }
        /// <summary>
        /// 身份验证完成处理
        /// </summary>
        /// <param name="result">异步操作状态</param>
        private void onAuthenticate(IAsyncResult result)
        {
            try
            {
                SslStream.EndAuthenticateAsServer(result);
                isLoadForm = isNextRequest = isTimeout = 0;
                DomainServer = null;
                //form.Clear();
                response.Push(ref response);
                RequestHeader.IsKeepAlive = false;
                HeaderReceiver.Receive();
                return;
            }
            catch (Exception error)
            {
                log.Error.Add(error, null, false);
            }
            headerError();
        }
        /// <summary>
        /// HTTP头部接收错误
        /// </summary>
        protected override void headerError()
        {
            form.Clear();
            pub.Dispose(ref SslStream);
            pub.Dispose(ref networkStream);
            if (Socket != null) fastCSharp.threading.disposeTimer.Default.addSocketClose(Socket);
            if (Ipv6.Ip == null)
            {
                if ((Socket = server.SocketEnd(Ipv4)) != null)
                {
                    start();
                    return;
                }
            }
            else
            {
                if ((Socket = server.SocketEnd(ref Ipv6)) != null)
                {
                    start();
                    return;
                }
                Ipv6.Null();
            }
            typePool<sslStream>.PushNotNull(this);
        }
        /// <summary>
        /// WebSocket结束
        /// </summary>
        protected override void webSocketEnd()
        {
            Interlocked.Increment(ref identity);
            webSocket.socket webSocket = webSocketReceiver.WebSocket;
            webSocketReceiver.WebSocket = null;
            pub.Dispose(ref SslStream);
            pub.Dispose(ref networkStream);
            if (Socket != null) fastCSharp.threading.disposeTimer.Default.addSocketClose(Socket);
            webSocket.Close();
            if (Ipv6.Ip == null)
            {
                if ((Socket = server.SocketEnd(Ipv4)) != null)
                {
                    start();
                    return;
                }
            }
            else
            {
                if ((Socket = server.SocketEnd(ref Ipv6)) != null)
                {
                    start();
                    return;
                }
                Ipv6.Null();
            }
            typePool<sslStream>.PushNotNull(this);
        }
        /// <summary>
        /// 未能识别的HTTP头部
        /// </summary>
        protected override void headerUnknown()
        {
            responseError(http.response.state.BadRequest400);
        }
        /// <summary>
        /// 开始接收头部数据
        /// </summary>
        protected override void receiveHeader()
        {
            HeaderReceiver.Receive();
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        protected override bool send(byte[] buffer, int index, int length)
        {
            SslStream.Write(buffer, index, length);
            return true;
        }
        /// <summary>
        /// 开始接收WebSocket数据
        /// </summary>
        /// <param name="webSocket"></param>
        protected override void receiveWebSocket(webSocket.socket webSocket)
        {
            if (webSocketReceiver == null) webSocketReceiver = new webSocketIdentityReceiver(this);
            webSocketReceiver.Receive(webSocket);
        }
        /// <summary>
        /// WebSocket发送消息
        /// </summary>
        /// <param name="webSocket"></param>
        internal override void WebSocketSend(webSocket.socket webSocket)
        {
            webSocketReceiver.Send(webSocket);
        }
        /// <summary>
        /// 获取请求表单数据
        /// </summary>
        /// <param name="identity">HTTP操作标识</param>
        /// <param name="loadForm">HTTP请求表单加载接口</param>
        internal override void GetForm(long identity, requestForm.ILoadForm loadForm)
        {
            if (Interlocked.CompareExchange(ref this.identity, identity + 1, identity) == identity)
            {
                if (isLoadForm == 0)
                {
                    isLoadForm = 1;
                    if (check100Continue())
                    {
                        switch (RequestHeader.PostType)
                        {
                            case requestHeader.postType.Json:
                            case requestHeader.postType.Form:
                            case requestHeader.postType.Xml:
                            case requestHeader.postType.Data:
                                if (formReceiver == null) formReceiver = new formIdentityReceiver(this);
                                formReceiver.Receive(loadForm);
                                return;
                            default:
                                if (boundaryReceiver == null) boundaryReceiver = new boundaryIdentityReceiver(this);
                                boundaryReceiver.Receive(loadForm);
                                return;
                        }
                    }
                }
                else log.Error.Add("表单已加载", null, true);
            }
            loadForm.OnGetForm(null);
        }
        /// <summary>
        /// FORM表单解析
        /// </summary>
        /// <param name="dataToType"></param>
        /// <returns></returns>
        internal override bool ParseForm(requestHeader.postType dataToType = requestHeader.postType.Data)
        {
            return formReceiver.Parse(dataToType);
        }
        /// <summary>
        /// HTTP响应头部输出
        /// </summary>
        /// <param name="buffer">输出数据</param>
        /// <param name="memoryPool">内存池</param>
        protected override void responseHeader(ref subArray<byte> buffer, memoryPool memoryPool)
        {
            if (responseSize == 0)
            {
                response.Push(ref response);
                sender.Send(dataSender.onSendType.Next, ref buffer, memoryPool);
            }
            else sender.Send(dataSender.onSendType.ResponseBody, ref buffer, memoryPool);
        }
        /// <summary>
        /// 输出HTTP响应数据
        /// </summary>
        /// <param name="identity">HTTP操作标识</param>
        /// <param name="response">HTTP响应数据</param>
        public override unsafe bool Response(long identity, ref response response)
        {
            if (Interlocked.CompareExchange(ref this.identity, identity + 1, identity) == identity)
            {
                this.response = response;
                response = null;
                if (this.response.LastModified != null)
                {
                    subArray<byte> ifModifiedSince = RequestHeader.IfModifiedSince;
                    if (ifModifiedSince.length == this.response.LastModified.Length)
                    {
                        fixed (byte* buffer = ifModifiedSince.array)
                        {
                            if (unsafer.memory.Equal(this.response.LastModified, buffer + ifModifiedSince.startIndex, ifModifiedSince.length))
                            {
                                response.Push(ref this.response);
                                this.response = response.NotChanged304;
                            }
                        }
                    }
                }
                if (boundaryReceiver != null) bigBuffers.Push(ref boundaryReceiver.Buffer);
                if (RequestHeader.Method == fastCSharp.web.http.methodType.POST && isLoadForm == 0)
                {
                    switch (RequestHeader.PostType)
                    {
                        case requestHeader.postType.Json:
                        case requestHeader.postType.Form:
                        case requestHeader.postType.Xml:
                        case requestHeader.postType.Data:
                            if (formReceiver == null) formReceiver = new formIdentityReceiver(this);
                            formReceiver.Receive(this);
                            return true;
                        default:
                            if (boundaryReceiver == null) boundaryReceiver = new boundaryIdentityReceiver(this);
                            boundaryReceiver.Receive(this);
                            return true;
                    }
                }
                responseHeader();
                return true;
            }
            response.Push(ref response);
            return false;
        }
        /// <summary>
        /// 发送HTTP响应内容
        /// </summary>
        /// <param name="isSend">是否发送成功</param>
        protected override void responseBody(bool isSend)
        {
            if (isSend)
            {
                if (response.BodyFile == null)
                {
                    subArray<byte> body = response.Body;
                    if (response.State == response.state.PartialContent206)
                    {
                        body.UnsafeSet(body.startIndex + (int)RequestHeader.RangeStart, (int)responseSize);
                    }
                    sender.Send(dataSender.onSendType.Next, ref body, null);
                }
                else sender.SendFile(dataSender.onSendType.Next, response.BodyFile, response.State == response.state.PartialContent206 ? RequestHeader.RangeStart : 0, responseSize);
            }
            else headerError();
        }
        /// <summary>
        /// 输出错误状态
        /// </summary>
        /// <param name="state">错误状态</param>
        protected override void responseError(response.state state)
        {
            if (boundaryReceiver != null) bigBuffers.Push(ref boundaryReceiver.Buffer);
            if (DomainServer != null)
            {
                response = DomainServer.GetErrorResponseData(state, RequestHeader.IsGZip);
                if (response != null)
                {
                    if (state != http.response.state.NotFound404 || RequestHeader.Method != web.http.methodType.GET)
                    {
                        RequestHeader.IsKeepAlive = false;
                    }
                    responseHeader();
                    return;
                }
            }
            byte[] data = errorResponseDatas[(int)state];
            if (data != null)
            {
                if (state == http.response.state.NotFound404 && RequestHeader.Method == web.http.methodType.GET)
                {
                    sender.Send(dataSender.onSendType.Next, subArray<byte>.Unsafe(data, 0, data.Length));
                }
                else
                {
                    RequestHeader.IsKeepAlive = false;
                    sender.Send(dataSender.onSendType.Close, subArray<byte>.Unsafe(data, 0, data.Length));
                }
            }
            else headerError();
        }
        /// <summary>
        /// 搜索引擎404提示
        /// </summary>
        protected override void responseSearchEngine404()
        {
            if (RequestHeader.Method == web.http.methodType.GET)
            {
                sender.Send(dataSender.onSendType.Next, subArray<byte>.Unsafe(searchEngine404Data, 0, searchEngine404Data.Length));
            }
            else
            {
                RequestHeader.IsKeepAlive = false;
                sender.Send(dataSender.onSendType.Close, subArray<byte>.Unsafe(searchEngine404Data, 0, searchEngine404Data.Length));
            }
        }
        ///// <summary>
        ///// 开始处理新的请求
        ///// </summary>
        ///// <param name="server">HTTP服务</param>
        ///// <param name="socket">套接字</param>
        ///// <param name="ip">客户端IP</param>
        //internal static void Start(server server, Socket socket, ref ipv6Hash ip)
        //{
        //START:
        //    try
        //    {
        //        sslStream stream = typePool<sslStream>.Pop() ?? NewStream();
        //        stream.Ipv6 = ip;
        //        stream.Ipv4 = 0;
        //        stream.Start(server, socket);
        //    }
        //    catch (Exception error)
        //    {
        //        log.Error.Add(error, null, false);
        //        socket.Close();
        //        if ((socket = server.SocketEnd(ref ip)) != null) goto START;
        //    }
        //}
        ///// <summary>
        ///// 开始处理新的请求
        ///// </summary>
        ///// <param name="server">HTTP服务</param>
        ///// <param name="socket">套接字</param>
        ///// <param name="ip">客户端IP</param>
        //internal static void Start(server server, Socket socket, int ip)
        //{
        //START:
        //    try
        //    {
        //        sslStream stream = typePool<sslStream>.Pop() ?? NewStream();
        //        stream.Ipv4 = ip;
        //        stream.Start(server, socket);
        //    }
        //    catch (Exception error)
        //    {
        //        log.Error.Add(error, null, false);
        //        socket.Close();
        //        if ((socket = server.SocketEnd(ip)) != null) goto START;
        //    }
        //}
        /// <summary>
        /// 创建套接字安全流
        /// </summary>
        /// <returns></returns>
        internal static sslStream NewStream()
        {
            Interlocked.Increment(ref newSslCount);
            return new sslStream();
        }
    }
}
