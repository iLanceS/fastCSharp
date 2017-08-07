using System;
using fastCSharp.net.tcp;
using System.IO;
using System.Runtime.CompilerServices;

namespace fastCSharp.io
{
    /// <summary>
    /// 文件分块服务
    /// </summary>
#if NotFastCSharpCode
    [fastCSharp.code.cSharp.tcpServer(Service = fileBlockServer.ServiceName, IsIdentityCommand = true, IsServerAsynchronousReceive = false, IsClientAsynchronousReceive = false)]
#else
    [fastCSharp.code.cSharp.tcpServer(Service = fileBlockServer.ServiceName, IsIdentityCommand = true, IsServerAsynchronousReceive = false, IsClientAsynchronousReceive = false, VerifyMethodType = typeof(fileBlockServer.tcpClient.timeVerifyMethod))]
#endif
    public partial class fileBlockServer : timeVerifyServer, IDisposable
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        internal const string ServiceName = "fileBlock";
        /// <summary>
        /// 文件分块写入流
        /// </summary>
        private fileBlockStream fileStream;
        /// <summary>
        /// 文件分块服务
        /// </summary>
        public fileBlockServer()
        {
            log.Error.Throw(log.exceptionType.ErrorOperation);
        }
        /// <summary>
        /// 文件分块服务
        /// </summary>
        /// <param name="fileName">文件全名</param>
        public fileBlockServer(string fileName)
        {
            fileStream = new fileBlockStream(fileName);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            pub.Dispose(ref fileStream);
        }
        /// <summary>
        /// 读取文件分块数据
        /// </summary>
        /// <param name="buffer">数据缓冲区</param>
        /// <param name="onReturn">数据缓冲区回调</param>
        [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false, IsClientAsynchronousReturnInputParameter = true, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private unsafe void read(fastCSharp.code.cSharp.tcpBase.subByteArrayEvent buffer, Func<fastCSharp.net.returnValue<fastCSharp.code.cSharp.tcpBase.subByteArrayEvent>, bool> onReturn)
        {
            fileBlockStream fileStream = this.fileStream;
            subArray<byte> data = buffer.Buffer;
            if (fileStream != null && data.length == sizeof(fileBlockStream.index))
            {
                fileBlockStream.index index;
                fixed (byte* bufferFixed = buffer.Buffer.array) index = *(fileBlockStream.index*)bufferFixed;
                fileStream.Read(onReturn, ref index);
            }
            else onReturn(default(fastCSharp.code.cSharp.tcpBase.subByteArrayEvent));
        }
        /// <summary>
        /// 写入文件分块数据(单客户端独占操作)
        /// </summary>
        /// <param name="dataStream">文件分块数据</param>
        /// <returns>写入文件位置</returns>
        [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false, IsClientCallbackTask = false, IsClientAsynchronous = true, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private unsafe long writeSynchronous(fastCSharp.code.cSharp.tcpBase.subByteUnmanagedStream dataStream)
        {
            if (fileStream != null)
            {
                subArray<byte> buffer = dataStream.Buffer;
                if (buffer.length != 0)
                {
                    fixed (byte* bufferFixed = buffer.array)
                    {
                        byte* start = bufferFixed - sizeof(int);
                        *(int*)start = buffer.length;
                        return fileStream.UnsafeWrite(start, buffer.length + (-buffer.length & 3) + sizeof(int));
                    }
                }
            }
            return -1;
        }
        /// <summary>
        /// 写入文件分块数据(多客户端并行操作)
        /// </summary>
        /// <param name="dataStream">文件分块数据</param>
        /// <param name="onReturn">写入文件位置</param>
        [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private unsafe void write(fastCSharp.code.cSharp.tcpBase.subByteUnmanagedStream dataStream, Func<fastCSharp.net.returnValue<long>, bool> onReturn)
        {
            if (fileStream != null)
            {
                subArray<byte> buffer = dataStream.Buffer;
                if (buffer.length != 0)
                {
                    fixed (byte* bufferFixed = buffer.array)
                    {
                        byte* start = bufferFixed - sizeof(int);
                        *(int*)start = buffer.length;
                        fileStream.UnsafeWrite(start, buffer.length + (-buffer.length & 3) + sizeof(int), onReturn);
                        return;
                    }
                }
            }
            onReturn(-1);
        }
        /// <summary>
        /// 等待缓存写入
        /// </summary>
        [fastCSharp.code.cSharp.tcpMethod(IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void waitBuffer()
        {
            if (fileStream != null) fileStream.WaitWriteBuffer();
        }
        /// <summary>
        /// 写入缓存
        /// </summary>
        /// <param name="isDiskFile">是否写入到磁盘文件</param>
        /// <returns>是否成功</returns>
        [fastCSharp.code.cSharp.tcpMethod(IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private bool flush(bool isDiskFile)
        {
            return fileStream != null && fileStream.Flush(isDiskFile) != null;
        }
    }
}
