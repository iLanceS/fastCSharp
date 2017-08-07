using System;
using System.IO;
using System.Threading;
using fastCSharp.io;
using fastCSharp.threading;
using System.Runtime.CompilerServices;

namespace fastCSharp.memoryDatabase
{
    /// <summary>
    /// 内存数据库物理层
    /// </summary>
    internal sealed class physical : IDisposable
    {
        /// <summary>
        /// 文件读取器
        /// </summary>
        internal sealed class fileReader : IDisposable
        {
            /// <summary>
            /// 文件长度
            /// </summary>
            private long size;
            /// <summary>
            /// 内存数据库物理层
            /// </summary>
            private physical physical;
            /// <summary>
            /// 文件流
            /// </summary>
            private FileStream fileStream;
            /// <summary>
            /// 服务器端读取文件等待事件
            /// </summary>
            private autoWaitHandle waitHandle;
            /// <summary>
            /// 客户端读取数据等待事件
            /// </summary>
            private autoWaitHandle clientHandle;
            /// <summary>
            /// 读取数据结果
            /// </summary>
            private subArray<byte> data;
            /// <summary>
            /// 缓存区访问锁
            /// </summary>
            private int bufferLock;
            /// <summary>
            /// 是否等待数据缓冲区
            /// </summary>
            private int isWaitBuffer;
            /// <summary>
            /// 是否释放资源
            /// </summary>
            private int isDisposed;
            /// <summary>
            /// 文件读取器
            /// </summary>
            /// <param name="physical">内存数据库物理层</param>
            public fileReader(physical physical)
            {
                this.physical = physical;
                fileStream = new FileStream(physical.path + physical.fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, fastCSharp.config.appSetting.StreamBufferSize, FileOptions.SequentialScan);
                size = fileStream.Length;
                waitHandle = new autoWaitHandle(false);
                clientHandle = new autoWaitHandle(false);
                if ((size & 3) == 0 && size >= sizeof(int) * 3) threadPool.TinyPool.FastStart(this, thread.callType.MemoryDatabasePhysicalFileReader);
                else
                {
                    Dispose();
                    physical.dataError();
                }
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                if (Interlocked.CompareExchange(ref isDisposed, 1, 0) == 0)
                {
                    waitHandle.Set();
                    clientHandle.Set();
                    pub.Dispose(ref fileStream);
                }
            }
            /// <summary>
            /// 读取数据
            /// </summary>
            /// <returns>读取的数据</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public subArray<byte> ReadHeader()
            {
                if (isDisposed == 0) waitHandle.Wait();
                return data;
            }
            /// <summary>
            /// 读取数据
            /// </summary>
            /// <returns>读取的数据</returns>
            public subArray<byte> Read()
            {
                if (isDisposed == 0)
                {
                    data.Null();
                    interlocked.CompareSetYield(ref bufferLock);
                    if (isWaitBuffer == 0) bufferLock = 0;
                    else
                    {
                        isWaitBuffer = 0;
                        bufferLock = 0;
                        clientHandle.Set();
                    }
                    waitHandle.Wait();
                }
                return data;
            }
            /// <summary>
            /// 等待缓存区
            /// </summary>
            private void waitBuffer()
            {
                if (data.array != null)
                {
                    Thread.Sleep(0);
                    interlocked.CompareSetYield(ref bufferLock);
                    if (data.array == null) bufferLock = 0;
                    else
                    {
                        isWaitBuffer = 1;
                        bufferLock = 0;
                        clientHandle.Wait();
                    }
                }
            }
            /// <summary>
            /// 读取文件线程
            /// </summary>
            internal unsafe void ReadThread()
            {
                int startIndex = 0, nextCount = sizeof(int) * 3;
                try
                {
                    byte[] headerBuffer = new byte[sizeof(int) * 3];
                    if (fileStream.Read(headerBuffer, 0, sizeof(int) * 3) != sizeof(int) * 3) return;
                    fixed (byte* bufferFixed = headerBuffer)
                    {
                        if (*(int*)bufferFixed != fastCSharp.emit.pub.PuzzleValue) return;
                        int headerSize = *(int*)(bufferFixed + sizeof(int));
                        if (headerSize > size || headerSize < sizeof(int) * 3 || (headerSize & 3) != 0) return;
                        int bufferSize = *(int*)(bufferFixed + sizeof(int) * 2);
                        if ((bufferSize & (bufferSize - 1)) != 0 || (bufferSize >> 12) == 0 || headerSize > bufferSize) return;
                        size -= headerSize;
                        physical.setLoadBuffer(bufferSize);
                        if (fileStream.Read(physical.loadBuffer, sizeof(int), headerSize -= sizeof(int) * 3) != headerSize) return;
                        fixed (byte* loaderBufferFixed = physical.loadBuffer) *(int*)loaderBufferFixed = bufferSize;
                        data.UnsafeSet(physical.loadBuffer, 0, headerSize + sizeof(int));
                        waitHandle.Set();
                        nextCount = 0;
                    }
                    byte[] buffer = physical.buffer, bigBuffer = nullValue<byte>.Array;
                    int isBigBuffer = 0;
                    fixed (byte* bufferFixed = buffer)
                    {
                        while (isDisposed == 0 && (size | (long)(uint)nextCount) != 0)
                        {
                            if (nextCount == 0)
                            {
                                if (fileStream.Read(buffer, startIndex = 0, nextCount = (int)Math.Min(buffer.Length, size)) != nextCount) return;
                                size -= nextCount;
                            }
                            byte* dataStart = bufferFixed + startIndex;
                            int dataSize = *(int*)dataStart, bufferSize;
                            if (dataSize < 0)
                            {
                                bufferSize = dataSize & 3;
                                bufferSize += (dataSize = -dataSize);
                            }
                            else
                            {
                                if ((dataSize & 3) != 0) return;
                                bufferSize = dataSize;
                            }
                            if (bufferSize > buffer.Length)
                            {
                                int count = bufferSize - nextCount;
                                if ((size -= count) < 0) return;
                                if (bufferSize > bigBuffer.Length)
                                {
                                    uint bigSize = (uint)(bigBuffer.Length == 0 ? buffer.Length : bigBuffer.Length);
                                    while (bigSize < bufferSize) bigSize <<= 1;
                                    if (bigSize == 0x80000000U) return;
                                    bigBuffer = new byte[bigSize];
                                    isBigBuffer = 0;
                                }
                                if (isBigBuffer == 0)
                                {
                                    Buffer.BlockCopy(buffer, startIndex, bigBuffer, 0, nextCount);
                                    if (fileStream.Read(bigBuffer, nextCount, count) != count) return;
                                    if (*(int*)dataStart < 0)
                                    {
                                        subArray<byte> newBuffer = fastCSharp.io.compression.stream.Deflate.GetDeCompressUnsafe(bigBuffer, sizeof(int), dataSize - sizeof(int), physical.memoryPool);
                                        waitBuffer();
                                        physical.setLoadBuffer(newBuffer.array);
                                        data = newBuffer;
                                    }
                                    else
                                    {
                                        waitBuffer();
                                        physical.setLoadBuffer();
                                        data.UnsafeSet(bigBuffer, sizeof(int), dataSize - sizeof(int));
                                        isBigBuffer = 1;
                                    }
                                }
                                else
                                {
                                    waitBuffer();
                                    Buffer.BlockCopy(buffer, startIndex, bigBuffer, 0, nextCount);
                                    if (fileStream.Read(bigBuffer, nextCount, count) != count) return;
                                    if (*(int*)dataStart < 0)
                                    {
                                        subArray<byte> newBuffer = fastCSharp.io.compression.stream.Deflate.GetDeCompressUnsafe(bigBuffer, sizeof(int), dataSize - sizeof(int), physical.memoryPool);
                                        physical.setLoadBuffer(newBuffer.array);
                                        data = newBuffer;
                                        isBigBuffer = 0;
                                    }
                                    else
                                    {
                                        physical.setLoadBuffer();
                                        data.UnsafeSet(bigBuffer, sizeof(int), dataSize - sizeof(int));
                                    }
                                }
                                waitHandle.Set();
                                nextCount = 0;
                            }
                            else 
                            {
                                int count = bufferSize - nextCount;
                                if (count > 0)
                                {
                                    if (size < count) return;
                                    Buffer.BlockCopy(buffer, startIndex, buffer, 0, nextCount);
                                    if (fileStream.Read(buffer, nextCount, count = (int)Math.Min(buffer.Length - nextCount, size)) != count) return;
                                    dataStart = bufferFixed;
                                    startIndex = 0;
                                    size -= count;
                                    nextCount += count;
                                }
                                if (*(int*)dataStart < 0)
                                {
                                    subArray<byte> newBuffer = fastCSharp.io.compression.stream.Deflate.GetDeCompressUnsafe(buffer, startIndex + sizeof(int), bufferSize - sizeof(int), physical.memoryPool);
                                    waitBuffer();
                                    physical.setLoadBuffer(newBuffer.array);
                                    data = newBuffer;
                                    isBigBuffer = 0;
                                }
                                else
                                {
                                    waitBuffer();
                                    byte[] newBuffer;
                                    if (isBigBuffer == 0)
                                    {
                                        if (bigBuffer.Length == 0) newBuffer = physical.getLoadBuffer();
                                        else
                                        {
                                            newBuffer = bigBuffer;
                                            isBigBuffer = 1;
                                        }
                                    }
                                    else
                                    {
                                        newBuffer = physical.getLoadBuffer();
                                        isBigBuffer = 0;
                                    }
                                    Buffer.BlockCopy(buffer, startIndex + sizeof(int), newBuffer, 0, bufferSize - sizeof(int));
                                    data.UnsafeSet(newBuffer, 0, bufferSize - sizeof(int));
                                }
                                waitHandle.Set();
                                nextCount -= bufferSize;
                                startIndex += bufferSize;
                            }
                        }
                    }
                    if (isDisposed == 0)
                    {
                        waitBuffer();
                        data.UnsafeSet(physical.buffer, 0, 0);
                    }
                }
                finally
                {
                    if (nextCount == 0) Dispose();
                    else
                    {
                        data.Null();
                        Dispose();
                        pub.Dispose(ref physical);
                    }
                }
            }
        }
        /// <summary>
        /// 数据错误
        /// </summary>
        private static readonly Exception dataException = new Exception("数据错误");
        /// <summary>
        /// 文件路径
        /// </summary>
        private readonly string path;
        /// <summary>
        /// 文件名
        /// </summary>
        private readonly string fileName;
        /// <summary>
        /// 文件流写入器
        /// </summary>
        private fileStreamWriter fileWriter;
        /// <summary>
        /// 数据加载文件读取器
        /// </summary>
        private fileReader loader;
        /// <summary>
        /// 数据加载文件读取器
        /// </summary>
        internal bool IsLoader { get { return loader != null; } }
        /// <summary>
        /// 数据加载缓冲区
        /// </summary>
        private byte[] loadBuffer;
        /// <summary>
        /// 内存池
        /// </summary>
        private memoryPool memoryPool;
        /// <summary>
        /// 缓冲区
        /// </summary>
        private byte[] buffer;
        /// <summary>
        /// 最后产生的异常错误
        /// </summary>
        internal Exception LastException { get; private set; }
        /// <summary>
        /// 当前缓冲区索引位置
        /// </summary>
        internal int BufferIndex;
        /// <summary>
        /// 当前操作访问锁
        /// </summary>
        private int currentLock;
        /// <summary>
        /// 是否已经释放资源
        /// </summary>
        private int isDisposed;
        /// <summary>
        /// 是否已经释放资源
        /// </summary>
        internal bool IsDisposed
        {
            get { return isDisposed != 0; }
        }
        /// <summary>
        /// 内存数据库物理层
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="isDomainUnloadDispose"></param>
        public physical(string fileName, bool isDomainUnloadDispose = true)
        {
            try
            {
                FileInfo file = new FileInfo(this.fileName = fileName + ".fmd");
                path = file.Directory.fullName();
                if (file.Exists)
                {
                    currentLock = 1;
                    loader = new fileReader(this);
                }
                else fileWriter = new fileStreamWriter(path + this.fileName, FileMode.CreateNew, FileShare.Read, FileOptions.None, true, null);
                if (isDomainUnloadDispose) fastCSharp.domainUnload.Add(this, domainUnload.unloadType.MemoryDatabasePhysicalDispose);
            }
            catch (Exception error)
            {
                LastException = error;
                currentLock = 2;
                Dispose();
            }
        }
        /// <summary>
        /// 设置缓冲区
        /// </summary>
        /// <param name="bufferSize"></param>
        private void setLoadBuffer(int bufferSize)
        {
            setBuffer(bufferSize);
            loadBuffer = memoryPool.Get();
        }
        /// <summary>
        /// 设置缓冲区
        /// </summary>
        /// <param name="buffer"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void setLoadBuffer(byte[] buffer)
        {
            setLoadBuffer();
            loadBuffer = buffer;
        }
        /// <summary>
        /// 设置缓冲区
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void setLoadBuffer()
        {
            memoryPool.Push(ref loadBuffer);
        }
        /// <summary>
        /// 获取缓冲区
        /// </summary>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private byte[] getLoadBuffer()
        {
            if (loadBuffer == null) return loadBuffer = memoryPool.Get();
            return loadBuffer;
        }
        /// <summary>
        /// 设置缓冲区
        /// </summary>
        /// <param name="bufferSize"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void setBuffer(int bufferSize)
        {
            buffer = (memoryPool = memoryPool.GetOrCreate(bufferSize)).Get();
            BufferIndex = sizeof(int);
        }
        /// <summary>
        /// 数据加载错误
        /// </summary>
        private void loadError()
        {
            LastException = dataException;
            Dispose();
        }
        /// <summary>
        /// 数据错误
        /// </summary>
        private void dataError()
        {
            if (Interlocked.CompareExchange(ref currentLock, 1, 0) == 0)
            {
                LastException = dataException;
                currentLock = 2;
                fastCSharp.threading.task.Tiny.Add(this, thread.callType.MemoryDatabasePhysicalDispose);
            }
        }
        /// <summary>
        /// 检测当前操作
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void checkCurrent()
        {
            if (LastException == null) currentLock = 0;
            else
            {
                currentLock = 2;
                fastCSharp.threading.task.Tiny.Add(this, thread.callType.MemoryDatabasePhysicalDispose);
            }
        }
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public subArray<byte> LoadHeader()
        {
            return loader.ReadHeader();
        }
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public subArray<byte> Load()
        {
            return loader.Read();
        }
        /// <summary>
        /// 读取数据完毕
        /// </summary>
        /// <param name="isLoaded">是否成功</param>
        /// <returns></returns>
        public bool Loaded(bool isLoaded)
        {
            if (currentLock == 1)
            {
                if (isLoaded)
                {
                    try
                    {
                        pub.Dispose(ref loader);
                        memoryPool.Push(ref loadBuffer);
                        fileWriter = new fileStreamWriter(path + fileName, FileMode.Open, FileShare.Read, FileOptions.None, true, null);
                        return true;
                    }
                    catch (Exception error)
                    {
                        LastException = error;
                    }
                    finally { currentLock = 0; }
                }
                else currentLock = 0;
            }
            Dispose();
            return false;
        }
        /// <summary>
        /// 创建文件头
        /// </summary>
        /// <param name="data"></param>
        /// <returns>是否创建成功</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe bool Create(ref subArray<byte> data)
        {
            fixed (byte* dataFixed = data.array) return Create(dataFixed + data.startIndex, data.length);
        }
        /// <summary>
        /// 创建文件头
        /// </summary>
        /// <param name="data"></param>
        /// <param name="size"></param>
        /// <returns>是否创建成功</returns>
        public unsafe bool Create(byte* data, int size)
        {
            if (size >= sizeof(int) * 3 && ((*(int*)data ^ fastCSharp.emit.pub.PuzzleValue) | (*(int*)(data + sizeof(int)) ^ size) | (size & 3)) == 0)
            {
                int bufferSize = *(int*)(data + sizeof(int) * 2);
                if ((bufferSize & (bufferSize - 1)) == 0 && bufferSize >= fastCSharp.config.memoryDatabase.MinPhysicalBufferSize && bufferSize >= size)
                {
                    if (Interlocked.CompareExchange(ref currentLock, 1, 0) == 0)
                    {
                        try
                        {
                            if (fileWriter.UnsafeWrite(data, size) >= 0) setBuffer(bufferSize);
                            else LastException = fileWriter.LastException;
                        }
                        catch (Exception error)
                        {
                            LastException = error;
                        }
                        finally { checkCurrent(); }
                        return LastException == null;
                    }
                    else return false;
                }
            }
            dataError();
            return false;
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns>成功状态，2表示成功需要等待缓存写入</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe int Append(ref subArray<byte> data)
        {
            fixed (byte* dataFixed = data.array) return Append(dataFixed + data.startIndex, data.length);
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="size"></param>
        private unsafe void append(byte* data, int size)
        {
            fixed (byte* bufferFixed = buffer)
            {
                do
                {
                    byte* start = data;
                    int count = buffer.Length - BufferIndex;
                    while (count >= *(int*)data)
                    {
                        size -= *(int*)data;
                        count -= *(int*)data;
                        data += *(int*)data;
                        if (size == 0)
                        {
                            fastCSharp.unsafer.memory.Copy(start, bufferFixed + BufferIndex, count = (int)(data - start));
                            BufferIndex += count;
                            return;
                        }
                        if (*(uint*)data > (uint)size || (*(int*)data & 3) != 0)
                        {
                            dataError();
                            return;
                        }
                    }
                    fastCSharp.unsafer.memory.Copy(start, bufferFixed + BufferIndex, count = (int)(data - start));
                    if ((BufferIndex += count) != sizeof(int))
                    {
                        flush(bufferFixed);
                        if (LastException != null) return;
                    }
                    while (*(int*)data >= buffer.Length)
                    {
                        byte[] currentBuffer = new byte[(count = *(int*)data) + sizeof(int)];
                        fixed (byte* currentBufferFixed = currentBuffer)
                        {
                            fastCSharp.unsafer.memory.Copy(data, currentBufferFixed + sizeof(int), count);
                            size -= count;
                            data += count;
                            subArray<byte> compressData = fastCSharp.io.compression.stream.Deflate.GetCompressUnsafe(currentBuffer, sizeof(int), count, sizeof(int), memoryPool);
                            if (compressData.array == null)
                            {
                                *(int*)currentBufferFixed = count + sizeof(int);
                                if (fileWriter.UnsafeWrite(currentBuffer) < 0)
                                {
                                    LastException = fileWriter.LastException;
                                    return;
                                }
                            }
                            else
                            {
                                fixed (byte* dataFixed = compressData.array)
                                {
                                    count = compressData.length + sizeof(int);
                                    fastCSharp.unsafer.memory.Copy(dataFixed + sizeof(int), currentBufferFixed + sizeof(int), compressData.length);
                                    memoryPool.PushNotNull(compressData.array);
                                    *(int*)currentBufferFixed = -count;
                                    if (fileWriter.UnsafeWrite(currentBuffer, 0, count + (-count & 3)) < 0)
                                    {
                                        LastException = fileWriter.LastException;
                                        return;
                                    }
                                }
                            }
                        }
                        if (size == 0) return;
                        if (*(uint*)data > (uint)size || (*(int*)data & 3) != 0)
                        {
                            dataError();
                            return;
                        }
                    }
                }
                while (true);
            }
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="size"></param>
        /// <returns>成功状态，2表示成功需要等待缓存写入</returns>
        public unsafe int Append(byte* data, int size)
        {
            if (size >= sizeof(int) && *(uint*)data <= (uint)size && ((*(int*)data | size) & 3) == 0)
            {
                if (Interlocked.CompareExchange(ref currentLock, 1, 0) == 0)
                {
                    try
                    {
                        append(data, size);
                    }
                    catch (Exception error)
                    {
                        LastException = error;
                    }
                    finally { checkCurrent(); }
                    if (LastException == null)
                    {
                        fileStreamWriter file = fileWriter;
                        if (file != null && file.IsWaitBuffer) return 2;
                        return 1;
                    }
                }
            }
            else dataError();
            return 0;
        }
        /// <summary>
        /// 本地数据库获取缓冲区
        /// </summary>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal byte[] LocalBuffer()
        {
            if (currentLock == 0)
            {
                if (buffer.Length - BufferIndex < sizeof(int) * 3) Flush();
                if (LastException == null) return buffer;
            }
            return null;
        }
        /// <summary>
        /// 刷新缓存区
        /// </summary>
        /// <param name="bufferFixed"></param>
        private unsafe void flush(byte* bufferFixed)
        {
            subArray<byte> compressData = fastCSharp.io.compression.stream.Deflate.GetCompressUnsafe(buffer, sizeof(int), BufferIndex - sizeof(int), sizeof(int), memoryPool);
            if (compressData.array == null) fileWriter.UnsafeWrite(bufferFixed, *(int*)bufferFixed = BufferIndex);
            else
            {
                fixed (byte* dataFixed = compressData.array)
                {
                    int count = compressData.length + sizeof(int);
                    fastCSharp.unsafer.memory.Copy(dataFixed + sizeof(int), bufferFixed + sizeof(int), compressData.length);
                    *(int*)dataFixed = -count;
                    fileWriter.UnsafeWrite(dataFixed, count + (-count & 3));
                }
                memoryPool.PushNotNull(compressData.array);
            }
            LastException = fileWriter.LastException;
            BufferIndex = sizeof(int);
        }
        /// <summary>
        /// 刷新缓存区
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe void flush()
        {
            if (BufferIndex != sizeof(int))
            {
                fixed (byte* bufferFixed = buffer) flush(bufferFixed);
            }
        }
        /// <summary>
        /// 等待缓存写入
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void WaitBuffer()
        {
            fileWriter.WaitWriteBuffer();
        }
        /// <summary>
        /// 刷新缓存区
        /// </summary>
        /// <returns>是否操作成功</returns>
        public bool Flush()
        {
            if (Interlocked.CompareExchange(ref currentLock, 1, 0) == 0)
            {
                try
                {
                    flush();
                }
                catch (Exception error)
                {
                    LastException = error;
                }
                finally { checkCurrent(); }
                return LastException == null;
            }
            return false;
        }
        /// <summary>
        /// 刷新写入文件缓存区
        /// </summary>
        /// <param name="isWriteFile">是否写入文件</param>
        /// <returns>是否操作成功</returns>
        public bool FlushFile(bool isWriteFile)
        {
            try
            {
                fileWriter.Flush(true);
                return true;
            }
            catch (Exception error)
            {
                LastException = error;
                int value = Interlocked.CompareExchange(ref currentLock, 2, 0);
                while (value == 1)
                {
                    Thread.Sleep(0);
                    value = Interlocked.CompareExchange(ref currentLock, 2, 0);
                }
                if (value == 0) Dispose();
            }
            return false;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.Increment(ref isDisposed) == 1)
            {
                fastCSharp.domainUnload.Remove(this, domainUnload.unloadType.MemoryDatabasePhysicalDispose, false);
                if (currentLock != 2)
                {
                    while (Interlocked.CompareExchange(ref currentLock, 2, 0) == 1) Thread.Sleep(0);
                }
                pub.Dispose(ref loader);
                if (fileWriter != null)
                {
                    try
                    {
                        flush();
                        fileWriter.Flush(true);
                    }
                    catch (Exception error)
                    {
                        LastException = error;
                    }
                    finally
                    {
                        pub.Dispose(ref fileWriter);
                    }
                }
                if (memoryPool != null)
                {
                    memoryPool.Push(ref buffer);
                    memoryPool.Push(ref loadBuffer);
                }
                if (LastException != null) log.Error.Add(LastException, fileName, false);
            }
        }
    }
}
