using System;

namespace fastCSharp.demo.sqlTableCacheServer
{
    /// <summary>
    /// TCP调用验证
    /// </summary>
#if NotFastCSharpCode
    [fastCSharp.code.cSharp.tcpCall(Service = pub.DataReaderServer, IsServerAsynchronousReceive = false, IsIdentityCommand = true, IsServer = true, IsMarkData = true, IsTryJsonSerializable = true)]
#else
    [fastCSharp.code.cSharp.tcpCall(Service = pub.DataReaderServer, VerifyMethodType = typeof(tcpCall.dataReaderTcpCallVerify), IsServerAsynchronousReceive = false, IsIdentityCommand = true, IsServer = true, IsMarkData = true, IsTryJsonSerializable = true)]
#endif
    internal partial class dataReaderTcpCallVerify : fastCSharp.net.tcp.timeVerifyServer.tcpCall<dataReaderTcpCallVerify>
    {
    }
    /// <summary>
    /// TCP调用验证
    /// </summary>
#if NotFastCSharpCode
    [fastCSharp.code.cSharp.tcpCall(Service = fastCSharp.demo.sqlModel.pub.LogTcpCallService, IsServerAsynchronousReceive = true, IsIdentityCommand = true, IsServer = true, IsMarkData = true)]
#else
    [fastCSharp.code.cSharp.tcpCall(Service = fastCSharp.demo.sqlModel.pub.LogTcpCallService, VerifyMethodType = typeof(tcpCall.dataLogTcpCallVerify), IsServerAsynchronousReceive = true, IsIdentityCommand = true, IsServer = true, IsMarkData = true)]
#endif
    internal partial class dataLogTcpCallVerify : fastCSharp.net.tcp.timeVerifyServer.tcpCall<dataLogTcpCallVerify>
    {
    }
}
