using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;

namespace fastCSharp.io.compression
{
    /// <summary>
    /// 压缩流处理
    /// </summary>
    public abstract class stream
    {
        /// <summary>
        /// 获取压缩流
        /// </summary>
        /// <param name="dataStream">原始数据流</param>
        /// <returns>压缩流</returns>
        protected abstract Stream getStream(Stream dataStream);
        /// <summary>
        /// 获取解压缩流
        /// </summary>
        /// <param name="dataStream">压缩数据流</param>
        /// <returns>解压缩流</returns>
        protected abstract Stream getDecompressStream(Stream dataStream);
        /// <summary>
        /// 压缩数据
        /// </summary>
        /// <param name="data">原始数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">压缩字节数</param>
        /// <param name="seek">起始位置</param>
        /// <param name="memoryPool">数据缓冲区内存池</param>
        /// <returns>压缩后的数据,失败返回null</returns>
        internal subArray<byte> GetCompressUnsafe(byte[] data, int startIndex, int count, int seek = 0, memoryPool memoryPool = null)
        {
            int length = count + seek;
            if (memoryPool == null)
            {
                using (MemoryStream dataStream = new MemoryStream())
                {
                    if (seek != 0) dataStream.Seek(seek, SeekOrigin.Begin);
                    using (Stream compressStream = getStream(dataStream))
                    {
                        compressStream.Write(data, startIndex, count);
                    }
                    if (dataStream.Position < length)
                    {
                        return subArray<byte>.Unsafe(dataStream.GetBuffer(), seek, (int)dataStream.Position - seek);
                    }
                }
            }
            else
            {
                byte[] buffer = memoryPool.Get();
                try
                {
                    using (MemoryStream dataStream = memoryStream.UnsafeNew(buffer))
                    {
                        if (seek != 0) dataStream.Seek(seek, SeekOrigin.Begin);
                        using (Stream compressStream = getStream(dataStream))
                        {
                            compressStream.Write(data, startIndex, count);
                        }
                        if (dataStream.Position < length)
                        {
                            byte[] streamBuffer = dataStream.GetBuffer();
                            if (streamBuffer == buffer) buffer = null;
                            return subArray<byte>.Unsafe(streamBuffer, seek, (int)dataStream.Position - seek);
                        }
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                finally
                {
                    memoryPool.PushOnly(buffer);
                }
            }
            return default(subArray<byte>);
        }
        /// <summary>
        /// 压缩数据
        /// </summary>
        /// <param name="data">原始数据</param>
        /// <param name="seek">起始位置</param>
        /// <param name="memoryPool">数据缓冲区内存池</param>
        /// <returns>压缩后的数据,失败返回null</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public subArray<byte> GetCompress(byte[] data, int seek = 0, memoryPool memoryPool = null)
        {
            if (seek < 0) log.Error.Throw(log.exceptionType.IndexOutOfRange);
            if (data != null && data.Length != 0)
            {
                return GetCompressUnsafe(data, 0, data.Length, seek, memoryPool);
            }
            return default(subArray<byte>);
        }
        /// <summary>
        /// 压缩数据
        /// </summary>
        /// <param name="data">原始数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">压缩字节数</param>
        /// <param name="seek">起始位置</param>
        /// <param name="memoryPool">数据缓冲区内存池</param>
        /// <returns>压缩后的数据,失败返回null</returns>
        public subArray<byte> GetCompress(byte[] data, int startIndex, int count, int seek = 0, memoryPool memoryPool = null)
        {
            if (seek >= 0)
            {
                if (count == 0) return subArray<byte>.Unsafe(nullValue<byte>.Array, 0, 0);
                array.range range = new array.range(data.length(), startIndex, count);
                if (count == range.GetCount) return GetCompressUnsafe(data, startIndex, count, seek, memoryPool);
            }
            log.Error.Throw(log.exceptionType.IndexOutOfRange);
            return default(subArray<byte>);
        }
        /// <summary>
        /// 解压器
        /// </summary>
        private struct deCompressor
        {
            /// <summary>
            /// 压缩输出流
            /// </summary>
            public Stream CompressStream;
            /// <summary>
            /// 输出数据流
            /// </summary>
            private unmanagedStream dataStream;
            /// <summary>
            /// 获取解压数据
            /// </summary>
            /// <returns>解压数据</returns>
            public unsafe subArray<byte> Get(memoryPool memoryPool)
            {
                if (memoryPool == null)
                {
                    pointer data = fastCSharp.unmanagedPool.StreamBuffers.Get();
                    try
                    {
                        using (dataStream = new unmanagedStream(data.Byte, fastCSharp.unmanagedPool.StreamBuffers.Size))
                        {
                            get();
                            return new subArray<byte>(dataStream.GetArray());
                        }
                    }
                    finally { fastCSharp.unmanagedPool.StreamBuffers.Push(ref data); }
                }
                else
                {
                    byte[] data = memoryPool.Get();
                    try
                    {
                        fixed (byte* dataFixed = data)
                        {
                            using (dataStream = new unmanagedStream(dataFixed, data.Length))
                            {
                                get();
                                if (dataStream.data.data == dataFixed)
                                {
                                    byte[] buffer = data;
                                    data = null;
                                    return subArray<byte>.Unsafe(buffer, 0, dataStream.Length);
                                }
                                return new subArray<byte>(dataStream.GetArray());
                            }
                        }
                    }
                    finally { memoryPool.PushOnly(data); }
                }
            }
            /// <summary>
            /// 获取解压数据
            /// </summary>
            private unsafe void get()
            {
                byte[] buffer = fastCSharp.memoryPool.StreamBuffers.Get();
                try
                {
                    int bufferLength = buffer.Length;
                    fixed (byte* bufferFixed = buffer)
                    {
                        int length = CompressStream.Read(buffer, 0, bufferLength);
                        while (length != 0)
                        {
                            dataStream.Write(bufferFixed, length);
                            length = CompressStream.Read(buffer, 0, bufferLength);
                        }
                    }
                }
                finally { fastCSharp.memoryPool.StreamBuffers.PushNotNull(buffer); }
            }
        }
        /// <summary>
        /// 解压缩数据
        /// </summary>
        /// <param name="compressData">压缩数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">解压缩字节数</param>
        /// <param name="memoryPool">数据缓冲区内存池</param>
        /// <returns>解压缩后的数据</returns>
        internal subArray<byte> GetDeCompressUnsafe(byte[] compressData, int startIndex, int count, memoryPool memoryPool)
        {
            using (Stream memoryStream = new MemoryStream(compressData, startIndex, count))
            using (Stream compressStream = getDecompressStream(memoryStream))
            {
                return new deCompressor { CompressStream = compressStream }.Get(memoryPool);
            }
        }
        /// <summary>
        /// 解压缩数据
        /// </summary>
        /// <param name="stream">数据流</param>
        /// <param name="memoryPool">数据缓冲区内存池</param>
        /// <returns>解压缩后的数据</returns>
        public subArray<byte> GetDeCompress(Stream stream, memoryPool memoryPool = null)
        {
            if (stream != null)
            {
                using (Stream compressStream = getDecompressStream(stream))
                {
                    return new deCompressor { CompressStream = compressStream }.Get(memoryPool);
                }
            }
            return default(subArray<byte>);
        }
        /// <summary>
        /// 解压缩数据
        /// </summary>
        /// <param name="compressData">压缩数据</param>
        /// <param name="memoryPool">数据缓冲区内存池</param>
        /// <returns>解压缩后的数据</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public subArray<byte> GetDeCompress(byte[] compressData, memoryPool memoryPool = null)
        {
            if (compressData.length() > 0)
            {
                return GetDeCompressUnsafe(compressData, 0, compressData.Length, memoryPool);
            }
            return default(subArray<byte>);
        }
        /// <summary>
        /// 解压缩数据
        /// </summary>
        /// <param name="compressData">压缩数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">解压缩字节数</param>
        /// <param name="memoryPool">数据缓冲区内存池</param>
        /// <returns>解压缩后的数据</returns>
        public subArray<byte> GetDeCompress(byte[] compressData, int startIndex, int count, memoryPool memoryPool = null)
        {
            if (count > 0)
            {
                array.range range = new array.range(compressData.length(), startIndex, count);
                if (count == range.GetCount) return GetDeCompressUnsafe(compressData, range.SkipCount, count, memoryPool);
            }
            log.Error.Throw(log.exceptionType.IndexOutOfRange);
            return default(subArray<byte>);
        }
        /// <summary>
        /// GZip压缩流处理
        /// </summary>
        private class gZipStream : stream
        {
            /// <summary>
            /// 获取压缩流
            /// </summary>
            /// <param name="dataStream">原始数据流</param>
            /// <returns>压缩流</returns>
            protected override Stream getStream(Stream dataStream)
            {
                return new GZipStream(dataStream, CompressionMode.Compress, true);
            }
            /// <summary>
            /// 获取解压缩流
            /// </summary>
            /// <param name="dataStream">压缩数据流</param>
            /// <returns>解压缩流</returns>
            protected override Stream getDecompressStream(Stream dataStream)
            {
                return new GZipStream(dataStream, CompressionMode.Decompress, false);
            }
        }
        /// <summary>
        /// deflate压缩流处理
        /// </summary>
        private class deflateStream : stream
        {
            /// <summary>
            /// 获取压缩流
            /// </summary>
            /// <param name="dataStream">原始数据流</param>
            /// <returns>压缩流</returns>
            protected override Stream getStream(Stream dataStream)
            {
                return new DeflateStream(dataStream, CompressionMode.Compress, true);
            }
            /// <summary>
            /// 获取解压缩流
            /// </summary>
            /// <param name="dataStream">压缩数据流</param>
            /// <returns>解压缩流</returns>
            protected override Stream getDecompressStream(Stream dataStream)
            {
                return new DeflateStream(dataStream, CompressionMode.Decompress, false);
            }
        }
        /// <summary>
        /// GZip压缩流处理
        /// </summary>
        public static readonly stream GZip = new gZipStream();
        /// <summary>
        /// deflate压缩流处理
        /// </summary>
        public static readonly stream Deflate = new deflateStream();
    }
}
