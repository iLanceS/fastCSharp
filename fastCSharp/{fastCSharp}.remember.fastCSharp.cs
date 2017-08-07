//本文件由程序自动生成,请不要自行修改
using System;
using fastCSharp;

#if NotFastCSharpCode
#else
#pragma warning disable

namespace fastCSharp.tcpRemember
{

            /// <summary>
            /// TCP服务命令序号记忆数据
            /// </summary>
            static class processCopy
            {
                /// <summary>
                /// 命令序号记忆数据
                /// </summary>
                private static fastCSharp.keyValue<string, int>[] _identityCommandNames_()
                {
                    fastCSharp.keyValue<string, int>[] names = new fastCSharp.keyValue<string, int>[4];
                    names[0].Set(@"(fastCSharp.diagnostics.processCopyServer.copyer)copyStart", 0);
                    names[1].Set(@"(fastCSharp.diagnostics.processCopyServer.copyer)guard", 1);
                    names[2].Set(@"(fastCSharp.diagnostics.processCopyServer.copyer)remove", 2);
                    names[3].Set(@"(fastCSharp.net.tcp.commandServer.socket,ulong,byte[],ref long)verify", 3);
                    return names;
                }
            }
}
namespace fastCSharp.tcpRemember
{

            /// <summary>
            /// TCP服务命令序号记忆数据
            /// </summary>
            static class fileBlock
            {
                /// <summary>
                /// 命令序号记忆数据
                /// </summary>
                private static fastCSharp.keyValue<string, int>[] _identityCommandNames_()
                {
                    fastCSharp.keyValue<string, int>[] names = new fastCSharp.keyValue<string, int>[6];
                    names[0].Set(@"(fastCSharp.code.cSharp.tcpBase.subByteArrayEvent,System.Func<fastCSharp.net.returnValue<fastCSharp.code.cSharp.tcpBase.subByteArrayEvent>,bool>)read", 0);
                    names[1].Set(@"(fastCSharp.code.cSharp.tcpBase.subByteUnmanagedStream)writeSynchronous", 1);
                    names[2].Set(@"(fastCSharp.code.cSharp.tcpBase.subByteUnmanagedStream,System.Func<fastCSharp.net.returnValue<long>,bool>)write", 2);
                    names[3].Set(@"()waitBuffer", 3);
                    names[4].Set(@"(bool)flush", 4);
                    names[5].Set(@"(fastCSharp.net.tcp.commandServer.socket,ulong,byte[],ref long)verify", 5);
                    return names;
                }
            }
}
namespace fastCSharp.tcpRemember
{

            /// <summary>
            /// TCP服务命令序号记忆数据
            /// </summary>
            static class memoryDatabasePhysical
            {
                /// <summary>
                /// 命令序号记忆数据
                /// </summary>
                private static fastCSharp.keyValue<string, int>[] _identityCommandNames_()
                {
                    fastCSharp.keyValue<string, int>[] names = new fastCSharp.keyValue<string, int>[11];
                    names[0].Set(@"(string)open", 0);
                    names[1].Set(@"(fastCSharp.memoryDatabase.physicalServer.timeIdentity)close", 1);
                    names[2].Set(@"(fastCSharp.code.cSharp.tcpBase.subByteUnmanagedStream)create", 2);
                    names[3].Set(@"(fastCSharp.memoryDatabase.physicalServer.timeIdentity)loadHeader", 3);
                    names[4].Set(@"(fastCSharp.memoryDatabase.physicalServer.timeIdentity)load", 4);
                    names[5].Set(@"(fastCSharp.memoryDatabase.physicalServer.timeIdentity,bool)loaded", 5);
                    names[6].Set(@"(fastCSharp.code.cSharp.tcpBase.subByteUnmanagedStream)append", 6);
                    names[7].Set(@"(fastCSharp.memoryDatabase.physicalServer.timeIdentity)waitBuffer", 7);
                    names[8].Set(@"(fastCSharp.memoryDatabase.physicalServer.timeIdentity)flush", 8);
                    names[9].Set(@"(fastCSharp.memoryDatabase.physicalServer.timeIdentity,bool)flushFile", 9);
                    names[10].Set(@"(fastCSharp.net.tcp.commandServer.socket,ulong,byte[],ref long)verify", 10);
                    return names;
                }
            }
}
namespace fastCSharp.tcpRemember
{

            /// <summary>
            /// TCP服务命令序号记忆数据
            /// </summary>
            static class httpServer
            {
                /// <summary>
                /// 命令序号记忆数据
                /// </summary>
                private static fastCSharp.keyValue<string, int>[] _identityCommandNames_()
                {
                    fastCSharp.keyValue<string, int>[] names = new fastCSharp.keyValue<string, int>[7];
                    names[0].Set(@"(string,string,fastCSharp.net.tcp.http.domain,bool)start", 0);
                    names[1].Set(@"(string,string,fastCSharp.net.tcp.http.domain[],bool)start", 1);
                    names[2].Set(@"(fastCSharp.net.tcp.http.domain)stop", 2);
                    names[3].Set(@"(fastCSharp.net.tcp.http.domain[])stop", 3);
                    names[4].Set(@"(fastCSharp.net.tcp.host)setForward", 4);
                    names[5].Set(@"()removeForward", 5);
                    names[6].Set(@"(fastCSharp.net.tcp.commandServer.socket,ulong,byte[],ref long)verify", 6);
                    return names;
                }
            }
}
namespace fastCSharp.tcpRemember
{

            /// <summary>
            /// TCP服务命令序号记忆数据
            /// </summary>
            static class session
            {
                /// <summary>
                /// 命令序号记忆数据
                /// </summary>
                private static fastCSharp.keyValue<string, int>[] _identityCommandNames_()
                {
                    fastCSharp.keyValue<string, int>[] names = new fastCSharp.keyValue<string, int>[4];
                    names[0].Set(@"(fastCSharp.sessionId,valueType)Set", 0);
                    names[1].Set(@"(fastCSharp.sessionId,out valueType)TryGet", 1);
                    names[2].Set(@"(fastCSharp.sessionId)Remove", 2);
                    names[3].Set(@"(fastCSharp.net.tcp.commandServer.socket,ulong,byte[],ref long)verify", 3);
                    return names;
                }
            }
}
namespace fastCSharp.tcpRemember
{

            /// <summary>
            /// TCP服务命令序号记忆数据
            /// </summary>
            static class tcpRegister
            {
                /// <summary>
                /// 命令序号记忆数据
                /// </summary>
                private static fastCSharp.keyValue<string, int>[] _identityCommandNames_()
                {
                    fastCSharp.keyValue<string, int>[] names = new fastCSharp.keyValue<string, int>[4];
                    names[0].Set(@"(fastCSharp.net.tcp.tcpRegisterReader.clientId,fastCSharp.net.tcp.tcpRegisterReader.service)register", 0);
                    names[1].Set(@"(fastCSharp.net.tcp.tcpRegisterReader.clientId,string)removeRegister", 1);
                    names[2].Set(@"(fastCSharp.net.tcp.tcpRegisterReader.clientId)removeRegister", 2);
                    names[3].Set(@"(fastCSharp.net.tcp.commandServer.socket,ulong,byte[],ref long)verify", 3);
                    return names;
                }
            }
}
namespace fastCSharp.tcpRemember
{

            /// <summary>
            /// TCP服务命令序号记忆数据
            /// </summary>
            static class tcpRegisterReader
            {
                /// <summary>
                /// 命令序号记忆数据
                /// </summary>
                private static fastCSharp.keyValue<string, int>[] _identityCommandNames_()
                {
                    fastCSharp.keyValue<string, int>[] names = new fastCSharp.keyValue<string, int>[4];
                    names[0].Set(@"()register", 0);
                    names[1].Set(@"(out int)getServices", 1);
                    names[2].Set(@"(fastCSharp.net.tcp.tcpRegisterReader.clientId,int,System.Func<fastCSharp.net.returnValue<fastCSharp.net.tcp.tcpRegisterReader.log>,bool>)getLog", 2);
                    names[3].Set(@"(fastCSharp.net.tcp.commandServer.socket,ulong,byte[],ref long)verify", 3);
                    return names;
                }
            }
}
#endif