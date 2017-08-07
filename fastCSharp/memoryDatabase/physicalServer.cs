using System;
using System.Collections.Generic;
using System.Threading;
using fastCSharp.code;
using fastCSharp.net.tcp;
using System.Runtime.CompilerServices;
using fastCSharp.net;

namespace fastCSharp.memoryDatabase
{
    /// <summary>
    /// 数据库物理层服务
    /// </summary>
#if NotFastCSharpCode
    [fastCSharp.code.cSharp.tcpServer(Service = physicalServer.ServiceName, IsIdentityCommand = true, IsServerAsynchronousReceive = false, IsClientAsynchronousReceive = false)]
#else
    [fastCSharp.code.cSharp.tcpServer(Service = physicalServer.ServiceName, IsIdentityCommand = true, IsServerAsynchronousReceive = false, IsClientAsynchronousReceive = false, VerifyMethodType = typeof(physicalServer.tcpClient.timeVerifyMethod))]
#endif
    public partial class physicalServer : timeVerifyServer
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        internal const string ServiceName = "memoryDatabasePhysical";
        /// <summary>
        /// 数据库物理层唯一标识
        /// </summary>
        [fastCSharp.emit.dataSerialize(IsReferenceMember = false, IsMemberMap = false)]
        public struct timeIdentity
        {
            /// <summary>
            /// 服务器启动时间
            /// </summary>
            public long TimeTick;
            /// <summary>
            /// 数据库物理层集合索引
            /// </summary>
            public int Index;
            /// <summary>
            /// 数据库物理层集合索引编号
            /// </summary>
            public int Identity;
            /// <summary>
            /// 转换成数据库物理层集合唯一标识
            /// </summary>
            /// <returns>数据库物理层集合唯一标识</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal indexIdentity GetIdentity()
            {
                return new indexIdentity { Index = Index, Identity = Identity };
            }
        }
        /// <summary>
        /// 数据库物理层初始化信息
        /// </summary>
        [fastCSharp.emit.dataSerialize(IsReferenceMember = false, IsMemberMap = false)]
        public struct physicalIdentity
        {
            /// <summary>
            /// 数据库物理层唯一标识
            /// </summary>
            public timeIdentity Identity;
            /// <summary>
            /// 是否新建文件
            /// </summary>
            public bool IsLoader;
            /// <summary>
            /// 数据库文件是否成功打开
            /// </summary>
            public bool IsOpen
            {
                get { return Identity.TimeTick != 0; }
            }
        }
        /// <summary>
        /// 打开数据库
        /// </summary>
        /// <param name="fileName">数据文件名</param>
        /// <returns>数据库物理层初始化信息</returns>
        [fastCSharp.code.cSharp.tcpMethod(IsClientAsynchronous = true, IsClientSynchronous = false, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private physicalIdentity open(string fileName)
        {
            return physicalSet.Default.Open(fileName);
        }
        /// <summary>
        /// 关闭数据库文件
        /// </summary>
        /// <param name="identity">数据库物理层唯一标识</param>
        [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void close(timeIdentity identity)
        {
            if (identity.TimeTick == fastCSharp.pub.StartTime.Ticks) physicalSet.Default.Close(identity.GetIdentity(), false);
        }
        ///// <summary>
        ///// 关闭数据库文件
        ///// </summary>
        ///// <param name="identity">数据库物理层唯一标识</param>
        //[fastCSharp.code.cSharp.tcpMethod]
        //private void waitClose(timeIdentity identity)
        //{
        //    if (identity.TimeTick == fastCSharp.pub.StartTime.Ticks) physicalSet.Default.Close(identity.GetIdentity(), true);
        //}
        ///// <summary>
        ///// 关闭数据库文件
        ///// </summary>
        ///// <param name="identity">数据文件名</param>
        //[fastCSharp.code.cSharp.tcpMethod(IsServerAsynchronousTask = false)]
        //private void close(string fileName)
        //{
        //    physicalSet.identity identity = physicalSet.Default.GetIdentity(fileName);
        //    if (identity.IsValid) physicalSet.Default.Close(identity, false);
        //}
        ///// <summary>
        ///// 关闭数据库文件
        ///// </summary>
        ///// <param name="identity">数据文件名</param>
        //[fastCSharp.code.cSharp.tcpMethod]
        //private void waitClose(string fileName)
        //{
        //    physicalSet.identity identity = physicalSet.Default.GetIdentity(fileName);
        //    if (identity.IsValid) physicalSet.Default.Close(identity, true);
        //}
        /// <summary>
        /// 创建数据库文件
        /// </summary>
        /// <param name="stream">文件头数据流</param>
        /// <returns>是否成功</returns>
        [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private bool create(fastCSharp.code.cSharp.tcpBase.subByteUnmanagedStream stream)
        {
            timeIdentity identity = getIdentity(ref stream.Buffer);
            return stream.Buffer.array != null && physicalSet.Default.Create(identity.GetIdentity(), ref stream.Buffer);
        }
        /// <summary>
        /// 数据库文件数据加载
        /// </summary>
        /// <param name="identity">数据库物理层唯一标识</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        [fastCSharp.code.cSharp.tcpMethod(IsClientCallbackTask = false, IsClientAsynchronous = true, IsClientSynchronous = false, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer loadHeader(timeIdentity identity)
        {
            if (identity.TimeTick == fastCSharp.pub.StartTime.Ticks) return physicalSet.Default.LoadHeader(identity.GetIdentity());
            return default(fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer);
        }
        /// <summary>
        /// 数据库文件数据加载
        /// </summary>
        /// <param name="identity">数据库物理层唯一标识</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        [fastCSharp.code.cSharp.tcpMethod(IsClientCallbackTask = false, IsClientAsynchronous = true, IsClientSynchronous = false, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer load(timeIdentity identity)
        {
            if (identity.TimeTick == fastCSharp.pub.StartTime.Ticks) return physicalSet.Default.Load(identity.GetIdentity());
            return default(fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer);
        }
        /// <summary>
        /// 数据库文件加载完毕
        /// </summary>
        /// <param name="identity">数据库物理层唯一标识</param>
        /// <param name="isLoaded">是否加载成功</param>
        /// <returns>是否加载成功</returns>
        [fastCSharp.code.cSharp.tcpMethod(IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private bool loaded(timeIdentity identity, bool isLoaded)
        {
            return identity.TimeTick == fastCSharp.pub.StartTime.Ticks && physicalSet.Default.Loaded(identity.GetIdentity(), isLoaded);
        }
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="dataStream">日志数据</param>
        /// <returns>是否成功写入缓冲区</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false, IsClientCallbackTask = false, IsClientAsynchronous = true, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private unsafe int append(fastCSharp.code.cSharp.tcpBase.subByteUnmanagedStream dataStream)
        {
            timeIdentity identity = getIdentity(ref dataStream.Buffer);
            return dataStream.Buffer.array != null ? physicalSet.Default.Append(identity.GetIdentity(), ref dataStream.Buffer) : 0;
        }
        /// <summary>
        /// 等待缓存写入
        /// </summary>
        /// <param name="identity">数据库物理层唯一标识</param>
        [fastCSharp.code.cSharp.tcpMethod(IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void waitBuffer(timeIdentity identity)
        {
            if(identity.TimeTick == fastCSharp.pub.StartTime.Ticks) physicalSet.Default.WaitBuffer(identity.GetIdentity());
        }
        /// <summary>
        /// 写入缓存
        /// </summary>
        /// <param name="identity">数据库物理层唯一标识</param>
        /// <returns>是否成功</returns>
        [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private bool flush(timeIdentity identity)
        {
            return identity.TimeTick == fastCSharp.pub.StartTime.Ticks && physicalSet.Default.Flush(identity.GetIdentity());
        }
        /// <summary>
        /// 写入缓存
        /// </summary>
        /// <param name="identity">数据库物理层唯一标识</param>
        /// <param name="isDiskFile">是否写入到磁盘文件</param>
        /// <returns>是否成功</returns>
        [fastCSharp.code.cSharp.tcpMethod(IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private bool flushFile(timeIdentity identity, bool isDiskFile)
        {
            return identity.TimeTick == fastCSharp.pub.StartTime.Ticks && physicalSet.Default.FlushFile(identity.GetIdentity(), isDiskFile);
        }
        /// <summary>
        /// 获取数据库物理层唯一标识
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>数据库物理层唯一标识</returns>
        private static unsafe timeIdentity getIdentity(ref subArray<byte> data)
        {
            timeIdentity identity;
            fixed (byte* dataFixed = data.array)
            {
                identity = *(timeIdentity*)(dataFixed + data.startIndex);
                if (identity.TimeTick == fastCSharp.pub.StartTime.Ticks)
                {
                    data.UnsafeSet(data.startIndex + sizeof(timeIdentity), data.length - sizeof(timeIdentity));
                }
                else data.UnsafeSet(null, 0, 0);
            }
            return identity;
        }
    }
}
