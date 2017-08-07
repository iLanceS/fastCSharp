using System;
using System.IO;
using System.Threading;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using fastCSharp.threading;
using fastCSharp.reflection;
using System.Runtime.CompilerServices;

namespace fastCSharp.io
{
    /// <summary>
    /// 文件流写入器
    /// </summary>
    public class fileStreamWriter : IDisposable
    {
        /// <summary>
        /// 最大文件缓存集合字节数
        /// </summary>
        private const int maxBufferSize = 1 << 20;
        /// <summary>
        /// 文件共享方式
        /// </summary>
        private FileShare fileShare;
        /// <summary>
        /// 附加选项
        /// </summary>
        private FileOptions fileOption;
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; private set; }
        /// <summary>
        /// 文件流
        /// </summary>
        private FileStream fileStream;
        /// <summary>
        /// 刷新检测周期
        /// </summary>
        private long checkFlushTicks = date.SecondTicks << 1;
        /// <summary>
        /// 缓存刷新检测秒数
        /// </summary>
        public uint CheckFlushSecond
        {
            set { checkFlushTicks = (long)value * date.SecondTicks; }
        }
        /// <summary>
        /// 缓存刷新检测毫秒数
        /// </summary>
        public uint CheckFlushMillisecond
        {
            set { checkFlushTicks = (long)value * date.MillisecondTicks; }
        }
        /// <summary>
        /// 文件流长度
        /// </summary>
        public long FileSize
        {
            get
            {
                FileStream fileStream = this.fileStream;
                return fileStream != null ? fileStream.Length : -1;
            }
        }
        /// <summary>
        /// 文件有效长度(已经写入)
        /// </summary>
        protected long fileLength;
        /// <summary>
        /// 当前写入缓存后的文件长度
        /// </summary>
        protected long fileBufferLength;
        /// <summary>
        /// 未写入文件缓存集合字节数
        /// </summary>
        private long bufferSize;
        /// <summary>
        /// 待写入文件缓存集合位置索引
        /// </summary>
        protected long bufferIndex;
        /// <summary>
        /// 是否需要等待写入缓存
        /// </summary>
        public bool IsWaitBuffer
        {
            get { return bufferSize > maxBufferSize; }
        }
        /// <summary>
        /// 文件编码
        /// </summary>
        private Encoding encoding;
        /// <summary>
        /// 文件写入缓冲区
        /// </summary>
        private byte[] buffer;
        /// <summary>
        /// 文件写入缓冲字节长度
        /// </summary>
        protected int bufferLength;
        /// <summary>
        /// 文件写入缓冲起始位置
        /// </summary>
        private int startIndex;
        /// <summary>
        /// 当前写入位置
        /// </summary>
        private int currentIndex;
        /// <summary>
        /// 待写入文件缓存集合
        /// </summary>
        protected list<memoryPool.pushSubArray> buffers = new list<memoryPool.pushSubArray>(sizeof(int));
        /// <summary>
        /// 正在写入文件缓存集合
        /// </summary>
        private list<memoryPool.pushSubArray> currentBuffers = new list<memoryPool.pushSubArray>(sizeof(int));
        /// <summary>
        /// 缓存刷新等待事件
        /// </summary>
        private EventWaitHandle flushWait;
        /// <summary>
        /// 缓存刷新等待数量
        /// </summary>
        protected int flushCount;
        /// <summary>
        /// 缓存操作锁
        /// </summary>
        protected readonly object bufferLock = new object();
        /// <summary>
        /// 最后一次异常错误
        /// </summary>
        public Exception LastException { get; private set; }
        /// <summary>
        /// 刷新检测时间
        /// </summary>
        private DateTime checkFlushTime;
        /// <summary>
        /// 是否正在检测刷新
        /// </summary>
        private int isCheckFlush;
        /// <summary>
        /// 是否写日志
        /// </summary>
        private bool isLog;
        /// <summary>
        /// 是否正在刷新
        /// </summary>
        private byte isFlush;
        /// <summary>
        /// 是否正在写文件
        /// </summary>
        private byte isWritting;
        /// <summary>
        /// 最后一个数据是否数据复制缓冲区
        /// </summary>
        private volatile byte isCopyBuffer;
        /// <summary>
        /// 是否已经释放资源
        /// </summary>
        protected byte isDisposed;
        /// <summary>
        /// 内存池
        /// </summary>
        private memoryPool memoryPool;
        /// <summary>
        /// 文件流写入器
        /// </summary>
        /// <param name="fileName">文件全名</param>
        /// <param name="mode">打开方式</param>
        /// <param name="fileShare">共享访问方式</param>
        /// <param name="fileOption">附加选项</param>
        /// <param name="isLog">是否写日志</param>
        /// <param name="encoding">文件编码</param>
        public fileStreamWriter(string fileName, FileMode mode = FileMode.CreateNew, FileShare fileShare = FileShare.None, FileOptions fileOption = FileOptions.None, bool isLog = true, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(fileName)) log.Error.Throw(log.exceptionType.Null);
            FileName = fileName;
            this.isLog = isLog;
            this.fileShare = fileShare;
            this.fileOption = fileOption;
            this.encoding = encoding;
            memoryPool = memoryPool.GetOrCreate(bufferLength = (int)file.BytesPerCluster(fileName));
            buffer = memoryPool.Get();
            open(mode);
            flushWait = new EventWaitHandle(true, EventResetMode.ManualReset);
        }
        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="mode">打开方式</param>
        private unsafe void open(FileMode mode)
        {
            startIndex = currentIndex = 0;
            fileStream = new FileStream(FileName, mode, FileAccess.Write, fileShare, bufferLength, fileOption);
            fileLength = fileBufferLength = bufferIndex = fileStream.Length;
            if (fileLength != 0)
            {
                fileStream.Seek(0, SeekOrigin.End);
                startIndex = currentIndex = (int)(fileLength % bufferLength);
            }
            else if (encoding != null)
            {
                file.bom bom = file.GetBom(encoding);
                if ((currentIndex = bom.Length) != 0)
                {
                    bufferIndex = (fileBufferLength += currentIndex);
                    fixed (byte* bufferFixed = buffer) *(uint*)bufferFixed = bom.Bom;
                }
            }
        }
        ///// <summary>
        ///// 写入数据
        ///// </summary>
        ///// <param name="value">数据</param>
        ///// <returns>写入位置,失败返回-1</returns>
        //public long Write(string value)
        //{
        //    return value.length() != 0 ? UnsafeWrite(value) : 0;
        //}
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="count">写入字节数</param>
        /// <returns>写入位置,失败返回-1</returns>
        public long Write(byte[] data, int count)
        {
            if (count > data.length() || count < 0) log.Error.Throw(log.exceptionType.IndexOutOfRange);
            if (count != 0)
            {
                memoryPool.pushSubArray pushData = default(memoryPool.pushSubArray);
                pushData.Value.UnsafeSet(data, 0, count);
                return UnsafeWrite(ref pushData);
            }
            return 0;
        }
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="index">起始位置</param>
        /// <param name="count">写入字节数</param>
        /// <returns>写入位置,失败返回-1</returns>
        public long Write(byte[] data, int index, int count)
        {
            if (index + count > data.length() || index < 0 || count < 0) log.Error.Throw(log.exceptionType.IndexOutOfRange);
            if(count != 0)
            {
                memoryPool.pushSubArray pushData = default(memoryPool.pushSubArray);
                pushData.Value.UnsafeSet(data, index, count);
                return UnsafeWrite(ref pushData);
            }
            return 0;
        }
        ///// <summary>
        ///// 字符串转换成字节数组
        ///// </summary>
        ///// <param name="value">字符串</param>
        ///// <returns>字节数组+缓冲区入池调用</returns>
        //[MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        //internal unsafe memoryPool.pushSubArray GetBytes(string value)
        //{
        //    return GetBytes(value, this.encoding ?? fastCSharp.config.appSetting.Encoding);
        //}
        /// <summary>
        /// 字符串转换成字节数组
        /// </summary>
        /// <param name="value">字符串</param>
        /// <param name="encoding"></param>
        /// <returns>字节数组+缓冲区入池调用</returns>
        internal static unsafe memoryPool.pushSubArray GetBytes(string value, Encoding encoding)
        {
            int length = encoding.CodePage == Encoding.Unicode.CodePage ? value.Length << 1 : encoding.GetByteCount(value);
            memoryPool pool = memoryPool.GetDefaultPool(length);
            byte[] data = pool.Get(length);
            if (encoding.CodePage == Encoding.Unicode.CodePage)
            {
                fixed (byte* dataFixed = data) unsafer.String.Copy(value, dataFixed);
            }
            else encoding.GetBytes(value, 0, value.Length, data, 0);
            return new memoryPool.pushSubArray { Value = subArray<byte>.Unsafe(data, 0, length), PushPool = pool };
        }
        ///// <summary>
        ///// 写入数据
        ///// </summary>
        ///// <param name="value">数据</param>
        ///// <returns>写入位置,失败返回-1</returns>
        //internal unsafe long UnsafeWrite(string value)
        //{
        //    return UnsafeWrite(GetBytes(value));
        //}
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>写入位置,失败返回-1</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal long UnsafeWrite(byte[] data)
        {
            memoryPool.pushSubArray pushData = default(memoryPool.pushSubArray);
            pushData.Value.UnsafeSet(data, 0, data.Length);
            return UnsafeWrite(ref pushData);
        }
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="index">起始位置</param>
        /// <param name="count">写入字节数</param>
        /// <returns>写入位置,失败返回-1</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal long UnsafeWrite(byte[] data, int index, int count)
        {
            memoryPool.pushSubArray pushData = default(memoryPool.pushSubArray);
            pushData.Value.UnsafeSet(data, index, count);
            return UnsafeWrite(ref pushData);
        }
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>写入位置,失败返回-1</returns>
        internal long UnsafeWrite(ref memoryPool.pushSubArray data)
        {
            subArray<byte> dataArray = data.Value;
            Monitor.Enter(bufferLock);
            if (isDisposed == 0)
            {
                long fileBufferLength = this.fileBufferLength;
                this.fileBufferLength += dataArray.length;
                if (isWritting == 0)
                {
                    int length = currentIndex + dataArray.length;
                    if (length < bufferLength && flushCount == 0)
                    {
                        Buffer.BlockCopy(dataArray.array, dataArray.startIndex, buffer, currentIndex, dataArray.length);
                        checkFlushTime = date.nowTime.Now.AddTicks(checkFlushTicks);
                        currentIndex = length;
                        bufferIndex = this.fileBufferLength;
                        flushWait.Reset();
                        Monitor.Exit(bufferLock);
                        data.Push();
                        setCheckFlush();
                    }
                    else
                    {
                        buffers.array[0] = data;
                        buffers.UnsafeAddLength(1);
                        bufferSize += dataArray.length;
                        isFlush = 0;
                        isWritting = 1;
                        isCopyBuffer = 0;
                        flushWait.Reset();
                        Monitor.Exit(bufferLock);
                        threadPool.TinyPool.FastStart(this, thread.callType.FileStreamWriteFile);
                    }
                }
                else
                {
                    try
                    {
                        buffers.Add(ref data);
                        bufferSize += dataArray.length;
                        isCopyBuffer = 0;
                        flushWait.Reset();
                    }
                    finally { Monitor.Exit(bufferLock); }
                }
                return fileBufferLength;
            }
            Monitor.Exit(bufferLock);
            data.Push();
            return -1;
        }
        ///// <summary>
        ///// 复制数据并写入
        ///// </summary>
        ///// <param name="data">数据</param>
        ///// <returns>写入位置,失败返回-1</returns>
        //internal long UnsafeWriteCopy(subArray<byte> data)
        //{
        //    byte isWriteBuffer = 0, isWrite = 0;
        //    interlocked.NoCheckCompareSetSleep0(ref bufferLock);
        //    long fileBufferLength = this.fileBufferLength;
        //    byte isDisposed = this.isDisposed;
        //    if (isDisposed == 0)
        //    {
        //        this.fileBufferLength += data.Count;
        //        if (isWritting == 0)
        //        {
        //            int length = currentIndex + data.Count;
        //            if (length < bufferLength)
        //            {
        //                Buffer.BlockCopy(data.Array, data.StartIndex, buffer, currentIndex, data.Count);
        //                currentIndex = length;
        //                bufferLock = 0;
        //                isWriteBuffer = 1;
        //            }
        //            else
        //            {
        //                try
        //                {
        //                    copy(data.array, data.StartIndex, data.Count);
        //                    bufferSize += data.Count;
        //                    isWritting = 1;
        //                }
        //                finally { bufferLock = 0; }
        //                isWrite = 1;
        //            }
        //        }
        //        else
        //        {
        //            try
        //            {
        //                copy(data.array, data.StartIndex, data.Count);
        //                bufferSize += data.Count;
        //            }
        //            finally { bufferLock = 0; }
        //        }
        //    }
        //    else bufferLock = 0;
        //    if (isDisposed == 0)
        //    {
        //        if (isWrite == 0)
        //        {
        //            if (isWriteBuffer != 0) setCheckFlush();
        //        }
        //        else threadPool.TinyPool.FastStart(writeFileHandle, null, null);
        //        return fileBufferLength;
        //    }
        //    return -1;
        //}
        ///// <summary>
        ///// 复制数据到缓冲区
        ///// </summary>
        ///// <param name="data">数据</param>
        ///// <param name="startIndex">数据其实位置</param>
        ///// <param name="length">数据长度</param>
        //private void copy(byte[] data, int startIndex, int length)
        //{
        //    if (isCopyBuffer != 0)
        //    {
        //        memoryPool.pushSubArray[] bufferArray = buffers.array;
        //        int bufferIndex = buffers.Count - 1;
        //        subArray<byte> copyBuffer = bufferArray[bufferIndex].Value;
        //        int freeLength = copyBuffer.FreeLength;
        //        if (length <= freeLength)
        //        {
        //            Buffer.BlockCopy(data, startIndex, copyBuffer.array, copyBuffer.EndIndex, length);
        //            bufferArray[bufferIndex].Value.UnsafeSetLength(copyBuffer.Count + length);
        //            if (length == freeLength) isCopyBuffer = 0;
        //            return;
        //        }
        //        Buffer.BlockCopy(data, startIndex, copyBuffer.array, copyBuffer.EndIndex, freeLength);
        //        bufferArray[bufferIndex].Value.UnsafeSetLength(copyBuffer.array.Length);
        //        startIndex += freeLength;
        //        length -= freeLength;
        //    }
        //    do
        //    {
        //        byte[] buffer = memoryPool.TryGet();
        //        if (buffer == null)
        //        {
        //            if (length <= memoryPool.Size)
        //            {
        //                Buffer.BlockCopy(data, startIndex, buffer = memoryPool.Get(), 0, length);
        //                isCopyBuffer = length == buffer.Length ? (byte)0 : (byte)1;
        //                buffers.Add(new memoryPool.pushSubArray { Value = subArray<byte>.Unsafe(buffer, 0, length), PushPool = memoryPool.PushHandle });
        //                return;
        //            }
        //            Buffer.BlockCopy(data, startIndex, buffer = new byte[length], 0, length);
        //            buffers.Add(new memoryPool.pushSubArray { Value = subArray<byte>.Unsafe(buffer, 0, length) });
        //            isCopyBuffer = 0;
        //            return;
        //        }
        //        if (length <= buffer.Length)
        //        {
        //            Buffer.BlockCopy(data, startIndex, buffer, 0, length);
        //            isCopyBuffer = length == buffer.Length ? (byte)0 : (byte)1;
        //            buffers.Add(new memoryPool.pushSubArray { Value = subArray<byte>.Unsafe(buffer, 0, length), PushPool = memoryPool.PushHandle });
        //            return;
        //        }
        //        Buffer.BlockCopy(data, startIndex, buffer, 0, buffer.Length);
        //        startIndex += buffer.Length;
        //        length -= buffer.Length;
        //        buffers.Add(new memoryPool.pushSubArray { Value = subArray<byte>.Unsafe(buffer, 0, buffer.Length), PushPool = memoryPool.PushHandle });
        //    }
        //    while (true);
        //}
        /// <summary>
        /// 写入数据(单客户端独占操作)
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="dataLength">数据长度</param>
        /// <returns>写入位置,失败返回-1</returns>
        public unsafe long UnsafeWrite(byte* data, int dataLength)
        {
            Exception exception = null;
            fixed (byte* bufferFixed = buffer)
            {
                Monitor.Enter(bufferLock);
                if (isDisposed == 0)
                {
                    long fileBufferLength = this.fileBufferLength;
                    this.fileBufferLength += dataLength;
                    if (isWritting == 0)
                    {
                        int length = currentIndex + dataLength;
                        if (length < bufferLength && flushCount == 0)
                        {
                            unsafer.memory.Copy(data, bufferFixed + currentIndex, dataLength);
                            checkFlushTime = date.nowTime.Now.AddTicks(checkFlushTicks);
                            currentIndex = length;
                            bufferIndex = this.fileBufferLength;
                            flushWait.Reset();
                            Monitor.Exit(bufferLock);
                            setCheckFlush();
                        }
                        else
                        {
                            try
                            {
                                copy(data, dataLength);
                                bufferSize += dataLength;
                                isFlush = 0;
                                isWritting = 1;
                                flushWait.Reset();
                            }
                            catch (Exception error) { exception = error; }
                            finally { Monitor.Exit(bufferLock); }
                            if (exception == null) threadPool.TinyPool.FastStart(this, thread.callType.FileStreamWriteFile);
                            else
                            {
                                this.error(exception);
                                return -1;
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            copy(data, dataLength);
                            bufferSize += dataLength;
                            flushWait.Reset();
                        }
                        catch (Exception error) { exception = error; }
                        finally { Monitor.Exit(bufferLock); }
                        if (exception != null)
                        {
                            this.error(exception);
                            return -1;
                        }
                    }
                    return fileBufferLength;
                }
                Monitor.Exit(bufferLock);
            }
            return -1;
        }
        /// <summary>
        /// 写入数据(多客户端并行操作)
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="dataLength">数据长度</param>
        /// <param name="onReturn">写入位置,失败返回-1</param>
        internal unsafe void UnsafeWrite(byte* data, int dataLength, Func<fastCSharp.net.returnValue<long>, bool> onReturn)
        {
            //showjim
            onReturn(-1);
        }
        /// <summary>
        /// 复制数据到缓冲区
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="length">数据长度</param>
        private unsafe void copy(byte* data, int length)
        {
            if (isCopyBuffer != 0)
            {
                memoryPool.pushSubArray[] bufferArray = buffers.array;
                int bufferIndex = buffers.length - 1;
                subArray<byte> copyBuffer = bufferArray[bufferIndex].Value;
                int freeLength = copyBuffer.FreeLength;
                if (length <= freeLength)
                {
                    fixed (byte* bufferFixed = copyBuffer.array) unsafer.memory.Copy(data, bufferFixed + copyBuffer.EndIndex, length);
                    bufferArray[bufferIndex].Value.UnsafeSetLength(copyBuffer.length + length);
                    if (length == freeLength) isCopyBuffer = 0;
                    return;
                }
                fixed (byte* bufferFixed = copyBuffer.array) unsafer.memory.Copy(data, bufferFixed + copyBuffer.EndIndex, freeLength);
                bufferArray[bufferIndex].Value.UnsafeSetLength(copyBuffer.array.Length);
                data += freeLength;
                length -= freeLength;
            }
            do
            {
                byte[] buffer = memoryPool.TryGet();
                if (buffer == null)
                {
                    if (length <= memoryPool.Size)
                    {
                        unsafer.memory.Copy(data, buffer = memoryPool.Get(), length);
                        isCopyBuffer = length == buffer.Length ? (byte)0 : (byte)1;
                        buffers.Add(new memoryPool.pushSubArray { Value = subArray<byte>.Unsafe(buffer, 0, length), PushPool = memoryPool });
                        return;
                    }
                    unsafer.memory.Copy(data, buffer = new byte[length], length);
                    buffers.Add(new memoryPool.pushSubArray { Value = subArray<byte>.Unsafe(buffer, 0, length) });
                    isCopyBuffer = 0;
                    return;
                }
                if (length <= buffer.Length)
                {
                    unsafer.memory.Copy(data, buffer, length);
                    isCopyBuffer = length == buffer.Length ? (byte)0 : (byte)1;
                    buffers.Add(new memoryPool.pushSubArray { Value = subArray<byte>.Unsafe(buffer, 0, length), PushPool = memoryPool });
                    return;
                }
                unsafer.memory.Copy(data, buffer, buffer.Length);
                data += buffer.Length;
                length -= buffer.Length;
                buffers.Add(new memoryPool.pushSubArray { Value = subArray<byte>.Unsafe(buffer, 0, buffer.Length), PushPool = memoryPool });
            }
            while (true);
        }
        /// <summary>
        /// 写入文件数据
        /// </summary>
        internal void WriteFile()
        {
            try
            {
                do
                {
                    Monitor.Enter(bufferLock);
                    int bufferCount = buffers.length;
                    if (bufferCount == 0)
                    {
                        if ((flushCount | isFlush) == 0)
                        {
                            checkFlushTime = date.nowTime.Now.AddTicks(checkFlushTicks);
                            isWritting = 0;
                            //if (currentIndex == startIndex) Monitor.Exit(bufferLock);
                            //else
                            //{
                            //    Monitor.Exit(bufferLock);
                            //    setCheckFlush();
                            //}
                            Monitor.Exit(bufferLock);
                            setCheckFlush();
                            return;
                        }
                        isFlush = 0;
                        int writeSize = currentIndex - startIndex;
                        if (writeSize == 0)
                        {
                            Monitor.Exit(bufferLock);
                            if (buffers.length == 0)
                            {
                                fileStream.Flush();
                                if (buffers.length == 0) flushWait.Set();
                            }
                            continue;
                        }
                        Monitor.Exit(bufferLock);
                        fileStream.Write(buffer, startIndex, writeSize);
                        
                        Monitor.Enter(bufferLock);
                        
                        fileLength += writeSize;
                        Monitor.Exit(bufferLock);
                        startIndex = currentIndex;
                        if (buffers.length == 0)
                        {
                            fileStream.Flush();
                            if (buffers.length == 0) flushWait.Set();
                        }
                        continue;
                    }
                    list<memoryPool.pushSubArray> datas = buffers;
                    isCopyBuffer = 0;
                    buffers = currentBuffers;
                    bufferIndex = fileBufferLength;
                    currentBuffers = datas;
                    Monitor.Exit(bufferLock);
                    foreach (memoryPool.pushSubArray data in datas.array)
                    {
                        int dataSize = data.Value.length, writeSize = writeFile(data.Value);
                        Monitor.Enter(bufferLock);
                        fileLength += writeSize;
                        bufferSize -= dataSize;
                        Monitor.Exit(bufferLock);
                        data.Push();
                        if (--bufferCount == 0) break;
                    }
                    Array.Clear(datas.array, 0, datas.length);
                    datas.Empty();
                    if (isCopyBuffer != 0 && buffers.length == 0) Thread.Sleep(0);
                }
                while (true);
            }
            catch (Exception error)
            {
                this.error(error);
            }
        }
        /// <summary>
        /// 写入文件数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>写入文件字节数</returns>
        private int writeFile(subArray<byte> data)
        {
            int count = data.length, length = currentIndex + count;
            if (length < bufferLength)
            {
                Buffer.BlockCopy(data.array, data.startIndex, buffer, currentIndex, count);
                currentIndex = length;
                return 0;
            }
            byte[] dataArray = data.array;
            int index = data.startIndex;
            length = bufferLength - currentIndex;
            if (currentIndex == startIndex)
            {
                fileStream.Write(dataArray, index, length += ((count - length) / bufferLength) * bufferLength);
                index += length;
                count -= length;
            }
            else
            {
                Buffer.BlockCopy(dataArray, index, buffer, currentIndex, length);
                index += length;
                count -= length;
                fileStream.Write(buffer, startIndex, length = bufferLength - startIndex);
                int size = count / bufferLength;
                if (size != 0)
                {
                    fileStream.Write(dataArray, index, size *= bufferLength);
                    index += size;
                    count -= size;
                    length += size;
                }
            }
            Buffer.BlockCopy(dataArray, index, buffer, startIndex = 0, currentIndex = count);
            return length;
        }
        ///// <summary>
        ///// 同步写入文件
        ///// </summary>
        ///// <param name="fileName">文件名</param>
        ///// <param name="startIndex">读取文件起始位置</param>
        //internal void WriteFile(string fileName, int startIndex)
        //{
        //    if (File.Exists(fileName))
        //    {
        //        try
        //        {
        //            using (FileStream readFileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, bufferLength, FileOptions.SequentialScan))
        //            {
        //                readFileStream.Seek(startIndex, SeekOrigin.Begin);
        //                int length = readFileStream.Read(buffer, currentIndex, bufferLength - currentIndex);
        //                fileBufferLength += length;
        //                if ((currentIndex += length) == bufferLength)
        //                {
        //                    fileStream.Write(buffer, this.startIndex, length = bufferLength - this.startIndex);
        //                    this.startIndex = 0;
        //                    fileLength += length;
        //                    do
        //                    {
        //                        currentIndex = readFileStream.Read(buffer, 0, bufferLength);
        //                        fileBufferLength += currentIndex;
        //                        if (currentIndex == bufferLength)
        //                        {
        //                            fileStream.Write(buffer, 0, bufferLength);
        //                            fileLength += currentIndex;
        //                        }
        //                        else break;
        //                    }
        //                    while (true);
        //                }
        //            }
        //        }
        //        catch (Exception error)
        //        {
        //            this.error(error);
        //        }
        //    }
        //}
        /// <summary>
        /// 等待缓存写入
        /// </summary>
        /// <param name="isDiskFile">是否写入到磁盘文件</param>
        /// <returns>最后一次异常错误</returns>
        public Exception Flush(bool isDiskFile)
        {
            flush(true);
            if (isDiskFile)
            {
                FileStream fileStream = this.fileStream;
                if (fileStream != null) fileStream.Flush(true);
            }
            return LastException;
        }
        /// <summary>
        /// 设置刷新检测
        /// </summary>
        private void setCheckFlush()
        {
            if (Interlocked.CompareExchange(ref isCheckFlush, 1, 0) == 0)
            {
                timerTask.Default.Add(this, thread.callType.FileStreamWriterCheckFlush, checkFlushTime);
            }
        }
        /// <summary>
        /// 刷新检测
        /// </summary>
        internal void CheckFlush()
        {
            if (isWritting == 0)
            {
                if (checkFlushTime <= date.nowTime.Now)
                {
                    try
                    {
                        flush(false);
                    }
                    finally { isCheckFlush = 0; }
                }
                else timerTask.Default.Add(this, thread.callType.FileStreamWriterCheckFlush, checkFlushTime);
            }
            else isCheckFlush = 0;
        }
        /// <summary>
        /// 等待缓存写入
        /// </summary>
        /// <param name="isWait"></param>
        protected void flush(bool isWait)
        {
            if (LastException == null)
            {
                Monitor.Enter(bufferLock);
                if (isWritting == 0 && fileBufferLength != fileLength)
                {
                    isWritting = isFlush = 1;
                    Monitor.Exit(bufferLock);
                    WriteFile();
                    if (isWait)
                    {
                        Interlocked.Increment(ref flushCount);
                        flushWait.WaitOne();
                        Interlocked.Decrement(ref flushCount);
                    }
                }
                else Monitor.Exit(bufferLock);
            }
        }
        /// <summary>
        /// 等待缓存写入
        /// </summary>
        public void WaitWriteBuffer()
        {
            if (bufferSize > maxBufferSize)
            {
                Thread.Sleep(0);
                while (bufferSize > maxBufferSize) Thread.Sleep(1);
            }
        }
        /// <summary>
        /// 写文件错误
        /// </summary>
        /// <param name="error">错误异常</param>
        private void error(Exception error)
        {
            if (isLog) log.Default.Add(error, null, false);
            LastException = error;
            Monitor.Enter(bufferLock);
            byte isDisposed = this.isDisposed;
            currentIndex = startIndex;
            isWritting = 0;
            fileBufferLength = fileLength = 0;
            this.isDisposed = 1;
            Monitor.Exit(bufferLock);
            dispose();
            pub.Dispose(ref fileStream);
            list<memoryPool.pushSubArray> buffers = null;
            Interlocked.Exchange(ref buffers, this.buffers);
            try
            {
                if (buffers != null && buffers.length != 0)
                {
                    memoryPool.pushSubArray[] dataArray = buffers.array;
                    for (int index = buffers.length; index != 0; dataArray[--index].Push()) ;
                }
            }
            finally
            {
                currentBuffers = null;
                memoryPool.Push(ref buffer);
                if (isDisposed == 0)
                {
                    flushWait.Set();
                    flushWait.Close();
                }
            }
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        protected virtual void dispose() { }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Monitor.Enter(bufferLock);
            if (isDisposed == 0)
            {
                isDisposed = 1;
                Monitor.Exit(bufferLock);
                while (LastException == null)
                {
                    Interlocked.Increment(ref flushCount);
                    flushWait.WaitOne();
                    Interlocked.Decrement(ref flushCount);
                    if (LastException == null)
                    {
                        Monitor.Enter(bufferLock);
                        if (fileBufferLength == fileLength)
                        {
                            Monitor.Exit(bufferLock);
                            break;
                        }
                        Monitor.Exit(bufferLock);
                    }
                }
                dispose();
                pub.Dispose(ref fileStream);
                Monitor.Enter(bufferLock);
                buffers.Null();
                currentBuffers.Null();
                Monitor.Exit(bufferLock);
                memoryPool.Push(ref buffer);
                flushWait.Set();
                flushWait.Close();
            }
            else Monitor.Exit(bufferLock);
        }
    }
}
