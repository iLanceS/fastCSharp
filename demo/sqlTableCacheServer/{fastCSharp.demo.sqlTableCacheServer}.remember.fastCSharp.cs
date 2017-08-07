//本文件由程序自动生成,请不要自行修改
using System;
using fastCSharp;

#if NotFastCSharpCode
#else
#pragma warning disable

namespace fastCSharp.demo.sqlTableCacheServer.tcpRemember
{

        /// <summary>
        /// TCP服务
        /// </summary>
        public partial class DataReader
        {
            /// <summary>
            /// 命令序号记忆数据
            /// </summary>
            private static fastCSharp.keyValue<string, int>[] _identityCommandNames_()
            {
                fastCSharp.keyValue<string, int>[] names = new fastCSharp.keyValue<string, int>[5];
                names[0].Set(@"fastCSharp.demo.sqlTableCacheServer.Class(int)Get", 0);
                names[1].Set(@"fastCSharp.demo.sqlTableCacheServer.Class()getIds", 1);
                names[2].Set(@"fastCSharp.demo.sqlTableCacheServer.Class(int)GetStudentIds", 2);
                names[3].Set(@"fastCSharp.demo.sqlTableCacheServer.Student(int)Get", 3);
                names[4].Set(@"fastCSharp.net.tcp.timeVerifyServer.tcpCall<fastCSharp.demo.sqlTableCacheServer.dataReaderTcpCallVerify>(fastCSharp.net.tcp.commandServer.socket,ulong,byte[],ref long)verify", 4);
                return names;
            }
        }
}
namespace fastCSharp.demo.sqlTableCacheServer.tcpRemember
{

        /// <summary>
        /// TCP服务
        /// </summary>
        public partial class DataLog
        {
            /// <summary>
            /// 命令序号记忆数据
            /// </summary>
            private static fastCSharp.keyValue<string, int>[] _identityCommandNames_()
            {
                fastCSharp.keyValue<string, int>[] names = new fastCSharp.keyValue<string, int>[7];
                names[0].Set(@"fastCSharp.demo.sqlModel.Class.sqlModel<fastCSharp.demo.sqlTableCacheServer.Class,fastCSharp.demo.sqlTableCacheServer.Class.memberCache>()getSqlCache", 0);
                names[1].Set(@"fastCSharp.demo.sqlModel.Class.sqlModel<fastCSharp.demo.sqlTableCacheServer.Class,fastCSharp.demo.sqlTableCacheServer.Class.memberCache>(long,int,System.Func<fastCSharp.net.returnValue<fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Class,fastCSharp.demo.sqlModel.Class>.data>,bool>)onSqlLog", 1);
                names[2].Set(@"fastCSharp.demo.sqlModel.Student.sqlModel<fastCSharp.demo.sqlTableCacheServer.Student>()getSqlCache", 2);
                names[3].Set(@"fastCSharp.demo.sqlModel.Student.sqlModel<fastCSharp.demo.sqlTableCacheServer.Student>(long,int,System.Func<fastCSharp.net.returnValue<fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Student,fastCSharp.demo.sqlModel.Student>.data>,bool>)onSqlLog", 3);
                names[4].Set(@"fastCSharp.net.tcp.timeVerifyServer.tcpCall<fastCSharp.demo.sqlTableCacheServer.dataLogTcpCallVerify>(fastCSharp.net.tcp.commandServer.socket,ulong,byte[],ref long)verify", 4);
                names[5].Set(@"fastCSharp.demo.sqlModel.Class.sqlModel<fastCSharp.demo.sqlTableCacheServer.Class,fastCSharp.demo.sqlTableCacheServer.Class.memberCache>(int)getSqlCache", 5);
                names[6].Set(@"fastCSharp.demo.sqlModel.Student.sqlModel<fastCSharp.demo.sqlTableCacheServer.Student>(int)getSqlCache", 6);
                return names;
            }
        }
}
#endif