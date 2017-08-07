//本文件由程序自动生成,请不要自行修改
using System;
using fastCSharp;

#if NotFastCSharpCode
#else
#pragma warning disable

namespace fastCSharp.expand.tcpRemember
{

            /// <summary>
            /// TCP服务命令序号记忆数据
            /// </summary>
            static class deploy
            {
                /// <summary>
                /// 命令序号记忆数据
                /// </summary>
                private static fastCSharp.keyValue<string, int>[] _identityCommandNames_()
                {
                    fastCSharp.keyValue<string, int>[] names = new fastCSharp.keyValue<string, int>[11];
                    names[0].Set(@"()clear", 0);
                    names[1].Set(@"()create", 1);
                    names[2].Set(@"(fastCSharp.net.indexIdentity)clear", 2);
                    names[3].Set(@"(fastCSharp.net.indexIdentity,System.DateTime)start", 3);
                    names[4].Set(@"(fastCSharp.net.indexIdentity,byte[][])setFileSource", 4);
                    names[5].Set(@"(fastCSharp.net.tcp.deployServer.directory,string)getFileDifferent", 5);
                    names[6].Set(@"(fastCSharp.net.indexIdentity,fastCSharp.net.tcp.deployServer.directory,string)addWeb", 6);
                    names[7].Set(@"(fastCSharp.net.indexIdentity,fastCSharp.keyValue<string,int>[],string)addFiles", 7);
                    names[8].Set(@"(fastCSharp.net.indexIdentity,fastCSharp.keyValue<string,int>[],string,int)addRun", 8);
                    names[9].Set(@"(fastCSharp.net.indexIdentity,int)addWaitRunSwitch", 9);
                    names[10].Set(@"(fastCSharp.net.tcp.commandServer.socket,ulong,byte[],ref long)verify", 10);
                    return names;
                }
            }
}
#endif