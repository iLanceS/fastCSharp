using System;
using System.IO;
using fastCSharp.threading;
using System.Runtime.CompilerServices;
using System.Threading;

namespace fastCSharp.io
{
    /// <summary>
    /// 文件分块写入流
    /// </summary>
    public sealed class fileBlockStream : fileStreamWriter
    {
        /// <summary>
        /// 文件索引
        /// </summary>
        [fastCSharp.emit.sqlColumn]
        public struct index
        {
            /// <summary>
            /// 位置索引
            /// </summary>
            public long Index;
            /// <summary>
            /// 数据大小
            /// </summary>
            public int Size;
            /// <summary>
            /// 扩展值
            /// </summary>
            [fastCSharp.code.ignore]
            internal int Custom;
            /// <summary>
            /// 文件分块结束位置
            /// </summary>
            public long EndIndex
            {
                get { return Index + (Size + sizeof(int)); }
            }
            /// <summary>
            /// 清空数据
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Null()
            {
                Index = 0;
                Size = Custom = 0;
            }
            /// <summary>
            /// 重置文件索引
            /// </summary>
            /// <param name="index"></param>
            /// <param name="size"></param>
            /// <returns></returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal int ReSet(long index, int size)
            {
                if (Index == index)
                {
                    if (Size == size) return 1;
                }
                else Index = index;
                Size = size;
                return 0;
            }
        }
        /// <summary>
        /// 文件读取
        /// </summary>
        internal sealed class reader
        {
            /// <summary>
            /// 读取文件位置
            /// </summary>
            private index index;
            /// <summary>
            /// 文件分块写入流
            /// </summary>
            public fileBlockStream FileStream;
            /// <summary>
            /// 等待缓存写入的文件分块写入流
            /// </summary>
            public fileBlockStream WaitFileStream;
            /// <summary>
            /// 等待缓存写入
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Wait()
            {
                WaitFileStream.wait(this);
                WaitFileStream = null;
            }
            /// <summary>
            /// 读取文件回调函数
            /// </summary>
            private Func<fastCSharp.net.returnValue<fastCSharp.code.cSharp.tcpBase.subByteArrayEvent>, bool> onReaded;
            /// <summary>
            /// 下一个文件读取
            /// </summary>
            public reader Next;
            /// <summary>
            /// 内存池
            /// </summary>
            private memoryPool memoryPool;
            /// <summary>
            /// 文件数据缓冲区
            /// </summary>
            private byte[] buffer;
            /// <summary>
            /// 文件分块结束位置
            /// </summary>
            public long EndIndex
            {
                get { return index.EndIndex; }
            }
            /// <summary>
            /// 开始读取文件
            /// </summary>
            public unsafe void Read()
            {
                do
                {
                    int readSize = index.Size + sizeof(int);
                    try
                    {
                        if (FileStream.isDisposed == 0)
                        {
                            buffer = (memoryPool = memoryPool.GetOrCreate(readSize)).Get();
                            FileStream fileReader = FileStream.fileReader;
                            long offset = fileReader.Position - index.Index;
                            if (offset >= 0 || -offset < index.Index) fileReader.Seek(offset, SeekOrigin.Current);
                            else fileReader.Seek(index.Index, SeekOrigin.Begin);
                            if (fileReader.Read(buffer, 0, readSize) == readSize)
                            {
                                fixed (byte* bufferFixed = buffer)
                                {
                                    if (*(int*)bufferFixed == index.Size) readSize = index.Size;
                                    else log.Default.Add(FileStream.FileName + " index[" + index.Index.toString() + "] size[" + (*(int*)bufferFixed).toString() + "]<>" + index.Size.toString(), new System.Diagnostics.StackFrame(), false);
                                }
                            }
                            else readSize = 0;
                        }
                    }
                    catch (Exception error)
                    {
                        log.Default.Add(error, null, false);
                    }
                    Func<fastCSharp.net.returnValue<fastCSharp.code.cSharp.tcpBase.subByteArrayEvent>, bool> onReaded = this.onReaded;
                    if (readSize == index.Size)
                    {
                        if (onReaded(new fastCSharp.code.cSharp.tcpBase.subByteArrayEvent { Buffer = subArray<byte>.Unsafe(buffer, sizeof(int), index.Size), SerializeEvent = memoryPool })) buffer = null;
                        else memoryPool.Push(ref buffer);
                    }
                    else
                    {
                        onReaded(default(fastCSharp.code.cSharp.tcpBase.subByteArrayEvent));
                        if (memoryPool != null) memoryPool.Push(ref buffer);
                    }
                    reader next = FileStream.next(this);
                    if (next == null)
                    {
                        FileStream = null;
                        onReaded = null;
                        memoryPool = null;
                        typePool<reader>.PushNotNull(this);
                        return;
                    }
                    onReaded = next.onReaded;
                    index = next.index;
                    next.onReaded = null;
                    typePool<reader>.PushNotNull(next);
                }
                while (true);
            }
            /// <summary>
            /// 取消文件读取
            /// </summary>
            public void Cancel()
            {
                Func<fastCSharp.net.returnValue<fastCSharp.code.cSharp.tcpBase.subByteArrayEvent>, bool> onReaded = this.onReaded;
                this.onReaded = null;
                typePool<reader>.PushNotNull(this);
                onReaded(default(fastCSharp.code.cSharp.tcpBase.subByteArrayEvent));
            }
            /// <summary>
            /// 设置文件读取
            /// </summary>
            /// <param name="index">读取文件位置</param>
            /// <param name="onReaded">读取文件回调函数</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Set(Func<fastCSharp.net.returnValue<fastCSharp.code.cSharp.tcpBase.subByteArrayEvent>, bool> onReaded, ref index index)
            {
                this.onReaded = onReaded;
                this.index = index;
            }
            /// <summary>
            /// 文件读取
            /// </summary>
            /// <returns></returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public static reader Get()
            {
                reader reader = typePool<reader>.Pop();
                if (reader == null)
                {
                    try
                    {
                        reader = new reader();
                    }
                    catch { return null; }
                }
                return reader;
            }
        }
        /// <summary>
        /// 文件读取流
        /// </summary>
        private FileStream fileReader;
        /// <summary>
        /// 文件读取
        /// </summary>
        private reader currentReader;
        /// <summary>
        /// 文件读取访问锁
        /// </summary>
        private int readerLock;
        /// <summary>
        /// 文件分块写入流
        /// </summary>
        /// <param name="fileName">文件全名</param>
        /// <param name="fileOption">附加选项</param>
        public fileBlockStream(string fileName, FileOptions fileOption = FileOptions.None)
            : base(fileName, File.Exists(fileName) ? FileMode.Open : FileMode.CreateNew, FileShare.Read, fileOption)
        {
            fileReader = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read, bufferLength, fileOption);
        }
        /// <summary>
        /// 设置文件读取
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private bool set(reader reader)
        {
            interlocked.CompareSetYield(ref readerLock);
            if (currentReader == null)
            {
                currentReader = reader;
                readerLock = 0;
                reader.FileStream = this;
                return true;
            }
            currentReader.Next = reader;
            readerLock = 0;
            return false;
        }
        /// <summary>
        /// 读取下一个文件数据
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private reader next(reader reader)
        {
            interlocked.CompareSetYield(ref readerLock);
            reader nextReader = reader.Next;
            if (nextReader == null) currentReader = null;
            else if ((reader.Next = nextReader.Next) == null) currentReader = reader;
            readerLock = 0;
            return nextReader;
        }
        /// <summary>
        /// 读取文件分块数据//showjim+cache
        /// </summary>
        /// <param name="onReaded"></param>
        /// <param name="index">文件分块数据位置</param>
        internal unsafe void Read(Func<fastCSharp.net.returnValue<fastCSharp.code.cSharp.tcpBase.subByteArrayEvent>, bool> onReaded, ref index index)
        {
            if (onReaded == null) log.Error.Throw(log.exceptionType.Null);
            long endIndex = index.EndIndex;
            if (index.Size > 0 && ((int)index.Index & 3) == 0 && endIndex <= fileBufferLength)
            {
                if (endIndex <= fileLength)
                {
                    reader reader = reader.Get();
                    if (reader != null)
                    {
                        reader.Set(onReaded, ref index);
                        if (set(reader)) fastCSharp.threading.threadPool.TinyPool.FastStart(reader, thread.callType.FileBlockStreamReader);
                        return;
                    }
                }
                else
                {
                    memoryPool memoryPool = null;
                    byte[] buffer = null;
                    int copyedSize = int.MinValue;
                    Monitor.Enter(bufferLock);
                    if (isDisposed == 0)
                    {
                        if (index.Index >= bufferIndex)
                        {
                            index.Index -= bufferIndex;
                            try
                            {
                                buffer = (memoryPool = memoryPool.GetOrCreate(index.Size)).Get(index.Size);
                                foreach (memoryPool.pushSubArray nextData in buffers.array)
                                {
                                    subArray<byte> data = nextData.SubArray;
                                    if (index.Index != 0)
                                    {
                                        if (index.Index >= data.length)
                                        {
                                            index.Index -= data.length;
                                            continue;
                                        }
                                        data.UnsafeSet(data.startIndex + (int)index.Index, data.length - (int)index.Index);
                                        index.Index = 0;
                                    }
                                    if (copyedSize < 0)
                                    {
                                        fixed (byte* dataFixed = data.array)
                                        {
                                            if (*(int*)(dataFixed + data.startIndex) != index.Size) break;
                                        }
                                        if ((copyedSize = data.length - sizeof(int)) == 0) continue;
                                        data.UnsafeSet(data.startIndex + sizeof(int), copyedSize);
                                        copyedSize = 0;
                                    }
                                    int copySize = index.Size - copyedSize;
                                    if (data.length >= copySize)
                                    {
                                        Buffer.BlockCopy(data.array, data.startIndex, buffer, copyedSize, copySize);
                                        copyedSize = index.Size;
                                        break;
                                    }
                                    Buffer.BlockCopy(data.array, data.startIndex, buffer, copyedSize, copySize);
                                    copyedSize += copySize;
                                }
                            }
                            catch (Exception error)
                            {
                                log.Default.Add(error, null, false);
                            }
                            finally { Monitor.Exit(bufferLock); }
                            if (copyedSize == index.Size)
                            {
                                onReaded(new fastCSharp.code.cSharp.tcpBase.subByteArrayEvent { Buffer = subArray<byte>.Unsafe(buffer, 0, index.Size), SerializeEvent = memoryPool });
                                return;
                            }
                        }
                        else
                        {
                            Monitor.Exit(bufferLock);
                            reader reader = reader.Get();
                            if (reader != null)
                            {
                                reader.Set(onReaded, ref index);
                                reader.WaitFileStream = this;
                                fastCSharp.threading.threadPool.TinyPool.FastStart(reader, thread.callType.FileBlockStreamReaderWait);
                                return;
                            }
                        }
                    }
                    else Monitor.Exit(bufferLock);
                }
            }
            onReaded(default(fastCSharp.code.cSharp.tcpBase.subByteArrayEvent));
        }
        /// <summary>
        /// 等待缓存写入
        /// </summary>
        /// <param name="reader"></param>
        private void wait(reader reader)
        {
            long endIndex = reader.EndIndex;
            if (endIndex <= fileLength)
            {
                if (set(reader)) reader.Read();
                return;
            }
            flush(true);
            if (isDisposed == 0)
            {
                if (set(reader)) reader.Read();
            }
            else reader.Cancel();
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        protected override void dispose()
        {
            pub.Dispose(ref fileReader);
        }
    }
}
