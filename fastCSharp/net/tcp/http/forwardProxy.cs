using System;
using System.Threading;
using System.Net.Sockets;

namespace fastCSharp.net.tcp.http
{
    /// <summary>
    /// HTTP转发代理
    /// </summary>
    internal sealed class forwardProxy : IDisposable
    {
        /// <summary>
        /// HTTP套接字
        /// </summary>
        private socket socket;
        /// <summary>
        /// HTTP代理服务客户端
        /// </summary>
        private client client;
        /// <summary>
        /// HTTP请求套接字
        /// </summary>
        private Socket requestSocket;
        /// <summary>
        /// HTTP代理套接字
        /// </summary>
        private net.socket proxySocket;
        /// <summary>
        /// HTTP请求数据缓冲区
        /// </summary>
        private byte[] requestBuffer;
        /// <summary>
        /// 接收HTTP请求数据处理
        /// </summary>
        public AsyncCallback onReceiveRequestHandle;
        /// <summary>
        /// HTTP响应数据缓冲区
        /// </summary>
        private byte[] responseBuffer;
        /// <summary>
        /// 接收HTTP响应数据处理
        /// </summary>
        private AsyncCallback onReceiveResponseHandle;
        /// <summary>
        /// 发送HTTP响应数据处理
        /// </summary>
        private AsyncCallback onSendResponseHandle;
        /// <summary>
        /// HTTP响应数据开始位置
        /// </summary>
        private int responseStartIndex;
        /// <summary>
        /// HTTP响应数据结束位置
        /// </summary>
        private int responseEndIndex;
        /// <summary>
        /// 是否已经释放资源
        /// </summary>
        private int isDisposed;
        /// <summary>
        /// 接收数据标识
        /// </summary>
        private volatile int receiveFlag;
        /// <summary>
        /// HTTP转发代理
        /// </summary>
        /// <param name="socket">HTTP套接字</param>
        /// <param name="client">HTTP代理服务客户端</param>
        public forwardProxy(socket socket, client client)
        {
            this.socket = socket;
            this.client = client;
            requestSocket = socket.Socket;
            proxySocket = client.NetSocket;
            requestBuffer = socket.HeaderReceiver.RequestHeader.Buffer;
            responseBuffer = socket.Buffer;
            onReceiveRequestHandle = onReceiveRequest;
            onReceiveResponseHandle = onReceiveResponse;
            onSendResponseHandle = onSendResponse;
        }
        /// <summary>
        /// 开始代理HTTP转发
        /// </summary>
        public void Start()
        {
            onReceiveRequest(socket.HeaderReceiver.ReceiveEndIndex);
            receiveResponse();
        }
        /// <summary>
        /// 接收请求数据回调处理
        /// </summary>
        /// <param name="count">接收数据长度</param>
        private void onReceiveRequest(int count)
        {
            if (isDisposed == 0)
            {
                receiveFlag |= 1;
                try
                {
                    if (proxySocket.send(requestBuffer, 0, count))
                    {
                        if (isDisposed == 0)
                        {
                            SocketError error;
                            requestSocket.BeginReceive(requestBuffer, 0, requestBuffer.Length, SocketFlags.None, out error, onReceiveRequestHandle, this);
                            if (error == SocketError.Success) return;
                        }
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                receiveFlag &= (int.MaxValue - 1);
                Dispose();
            }
        }
        /// <summary>
        /// 接收请求数据回调处理
        /// </summary>
        /// <param name="result">接收数据结果</param>
        private void onReceiveRequest(IAsyncResult result)
        {
            receiveFlag &= (int.MaxValue - 1);
            if (isDisposed == 0)
            {
                try
                {
                    SocketError error;
                    int count = requestSocket.EndReceive(result, out error);
                    if (error == SocketError.Success && count > 0)
                    {
                        onReceiveRequest(count);
                        return;
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                Dispose();
            }
        }
        /// <summary>
        /// 接收HTTP响应数据
        /// </summary>
        private void receiveResponse()
        {
            if (isDisposed == 0)
            {
                receiveFlag |= 2;
                try
                {
                    if (isDisposed == 0)
                    {
                        SocketError error;
                        proxySocket.Socket.BeginReceive(responseBuffer, 0, responseBuffer.Length, SocketFlags.None, out error, onReceiveResponseHandle, this);
                        if (error == SocketError.Success) return;
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                receiveFlag &= int.MaxValue - 2;
                Dispose();
            }
        }
        /// <summary>
        /// 接收响应数据回调处理
        /// </summary>
        /// <param name="result">接收数据结果</param>
        private void onReceiveResponse(IAsyncResult result)
        {
            receiveFlag &= int.MaxValue - 2;
            if (isDisposed == 0)
            {
                try
                {
                    SocketError error;
                    responseEndIndex = proxySocket.Socket.EndReceive(result, out error);
                    if (error == SocketError.Success && responseEndIndex > 0)
                    {
                        responseStartIndex = 0;
                        sendResponse();
                        return;
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                Dispose();
            }
        }
        /// <summary>
        /// 发送HTTP响应数据
        /// </summary>
        private void sendResponse()
        {
            if (isDisposed == 0)
            {
                try
                {
                    SocketError error;
                    requestSocket.BeginSend(responseBuffer, responseStartIndex, responseEndIndex - responseStartIndex, SocketFlags.None, out error, onSendResponseHandle, this);
                    if (error == SocketError.Success) return;
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                Dispose();
            }
        }
        /// <summary>
        /// 发送HTTP响应数据处理
        /// </summary>
        /// <param name="result">发送数据结果</param>
        private void onSendResponse(IAsyncResult result)
        {
            if (isDisposed == 0)
            {
                try
                {
                    SocketError error;
                    int count = requestSocket.EndSend(result, out error);
                    if (error == SocketError.Success && count > 0)
                    {
                        responseStartIndex += count;
                        if (responseStartIndex == responseEndIndex) receiveResponse();
                        else sendResponse();
                        return;
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                Dispose();
            }
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.Increment(ref isDisposed) == 1)
            {
                try
                {
                    pub.Dispose(ref client);
                    requestSocket.shutdownConnected();
                }
                catch (Exception error)
                {
                    log.Default.Add(error, null, false);
                }
                finally
                {
                    fastCSharp.threading.disposeTimer.Default.addSocketClose(requestSocket);
                    while (receiveFlag != 0) Thread.Sleep(1);
                    socket.ProxyEnd();
                }
            }
        }
    }
}
