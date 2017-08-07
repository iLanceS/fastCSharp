using System;

namespace fastCSharp.demo.udpPortServer
{
    /// <summary>
    /// UDP穿透端口服务
    /// </summary>
    [fastCSharp.code.cSharp.tcpServer(Host = "127.0.0.1", Service = "portServer", Port = 12345, IsIdentityCommand = true, ReceiveBufferSize = 256, SendBufferSize = 256, IsRememberIdentityCommand = false)]
    public partial class portServer
    {
        /// <summary>
        /// UPD服务端
        /// </summary>
        internal udpServer UpdServer;
        /// <summary>
        /// 根据名称获取IP端口信息
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>IP端口信息</returns>
        [fastCSharp.code.cSharp.tcpMethod(InputParameterMaxLength = udpServer.MaxPacketSize)]
        private ipPort get(fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer name)
        {
            return UpdServer.Get(name.Buffer);
        }
    }
}
