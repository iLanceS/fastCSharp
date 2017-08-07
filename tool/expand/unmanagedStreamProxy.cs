using System;
using System.IO;

namespace fastCSharp
{
    /// <summary>
    /// 非托管内存数据流代理
    /// </summary>
    public sealed unsafe class unmanagedStreamProxy : unmanagedStream
    {
        /// <summary>
        /// 目标数据流
        /// </summary>
        private Stream stream;
        /// <summary>
        /// 预申请调用深度
        /// </summary>
        private int prepDepth;
        /// <summary>
        /// 非托管内存数据流代理
        /// </summary>
        /// <param name="stream">目标数据流</param>
        /// <param name="length">容器初始尺寸</param>
        public unmanagedStreamProxy(Stream stream, int length = DefaultLength)
            : base(length)
        {
            if (stream == null) log.Error.Throw(log.exceptionType.Null);
            this.stream = stream;
        }
        /// <summary>
        /// 非托管内存数据流代理
        /// </summary>
        /// <param name="stream">目标数据流</param>
        /// <param name="data">无需释放的数据</param>
        /// <param name="dataLength">容器初始尺寸</param>
        public unmanagedStreamProxy(Stream stream, byte* data, int dataLength)
            : base(data, dataLength)
        {
            if (stream == null) log.Error.Throw(log.exceptionType.Null);
            this.stream = stream;
        }
        /// <summary>
        /// 预增数据流长度
        /// </summary>
        /// <param name="length">增加长度</param>
        public override void PrepLength(int length)
        {
            if (prepDepth == 0) writeStream();
            prepLength(length);
            ++prepDepth;
        }
        /// <summary>
        /// 预增数据流结束
        /// </summary>
        public override void PrepLength()
        {
            --prepDepth;
        }
        /// <summary>
        /// 释放数据容器
        /// </summary>
        public override void Close()
        {
            base.Close();
            offset = 0;
        }
        /// <summary>
        /// 清空数据
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            offset = 0;
        }
        /// <summary>
        /// 写入数据
        /// </summary>
        private void writeStream()
        {
            if (length != 0)
            {
                offset += length;
                byte* start = Data;
                byte[] buffer = fastCSharp.memoryPool.StreamBuffers.Get();
                try
                {
                    fixed (byte* bufferFixed = buffer)
                    {
                        while (length > buffer.Length)
                        {
                            fastCSharp.unsafer.memory.Copy(start, bufferFixed, buffer.Length);
                            stream.Write(buffer, 0, buffer.Length);
                            length -= buffer.Length;
                            start += buffer.Length;
                        }
                        fastCSharp.unsafer.memory.Copy(start, bufferFixed, length);
                        stream.Write(buffer, 0, length);
                    }
                }
                finally { fastCSharp.memoryPool.StreamBuffers.PushNotNull(buffer); }
                length = 0;
            }
        }
        /// <summary>
        /// 释放数据容器
        /// </summary>
        public override void Dispose()
        {
            try
            {
                writeStream();
            }
            finally { base.Dispose(); }
        }
    }
}
