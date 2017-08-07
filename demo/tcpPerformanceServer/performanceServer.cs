using System;
using System.Threading;
using System.Diagnostics;

namespace fastCSharp.demo.tcpPerformanceServer
{
    /// <summary>
    /// TCP调用IOCP性能测试服务
    /// </summary>
    [fastCSharp.code.cSharp.tcpServer(Service = "tcpPerformance", IsIdentityCommand = true, SendBufferSize = 128 << 10, ClientCheckSeconds = 0, IsRememberIdentityCommand = false)]
    public partial class performanceServer
    {
        /// <summary>
        /// 接收数据字节数
        /// </summary>
        private long receiveSize;
        /// <summary>
        /// 客户端数量
        /// </summary>
        private int clientCount;
        /// <summary>
        /// 客户端计数事件
        /// </summary>
        public event Action<int> OnClient;
        /// <summary>
        /// 停止计数事件
        /// </summary>
        public event Action<long> OnStop;
        /// <summary>
        /// 设置TCP命令服务端
        /// </summary>
        /// <param name="commandServer">TCP命令服务端</param>
        public void SetCommandServer(fastCSharp.net.tcp.commandServer commandServer)
        {
            commandServer.OnClientVerify += onClient;
            commandServer.OnCloseVerifyClient += onClose;
        }
        /// <summary>
        /// 客户端计数事件
        /// </summary>
        /// <param name="client"></param>
        private void onClient(fastCSharp.net.tcp.commandServer.socket client)
        {
            int clientCount = Interlocked.Increment(ref this.clientCount);
            if (OnClient != null) OnClient(clientCount);
        }
        /// <summary>
        /// 停止计数
        /// </summary>
        /// <param name="client"></param>
        private void onClose(fastCSharp.net.tcp.commandServer.socket client)
        {
            int clientCount = Interlocked.Decrement(ref this.clientCount);
            if (OnClient != null) OnClient(clientCount);
            if (clientCount == 0)
            {
                if (OnStop != null) OnStop(receiveSize);
                receiveSize = 0;
            }
        }
        /// <summary>
        /// 接收测试数据并返回
        /// </summary>
        /// <param name="data">接收的数据</param>
        [fastCSharp.code.cSharp.tcpMethod(IsClientCallbackTask = false, IsClientAsynchronous = true, IsClientSynchronous = false, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private void serverAsynchronous(fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer data, Func<fastCSharp.net.returnValue<fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer>, bool> onReturn)
        {
            Interlocked.Add(ref receiveSize, data.Buffer.Count);
            onReturn(data);
        }
        /// <summary>
        /// 接收测试数据并返回
        /// </summary>
        /// <param name="data">接收的数据</param>
        /// <returns></returns>
        [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false, IsClientCallbackTask = false, IsClientAsynchronous = true, IsClientSynchronous = false, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer serverSynchronous(fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer data)
        {
            Interlocked.Add(ref receiveSize, data.Buffer.Count);
            return data;
        }
    }
}
