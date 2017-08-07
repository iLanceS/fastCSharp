using System;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.IO;
using fastCSharp.io;
using fastCSharp.web;
using fastCSharp.threading;
using fastCSharp.code.cSharp;
using System.Text;
using System.Runtime.CompilerServices;

namespace fastCSharp.net.tcp.http
{
    /// <summary>
    /// HTTP套接字
    /// </summary>
    public abstract class socketBase : requestForm.ILoadForm, IDisposable
    {
        /// <summary>
        /// 大数据缓冲区
        /// </summary>
        protected static readonly memoryPool bigBuffers = memoryPool.GetOrCreate(fastCSharp.config.http.Default.BigBufferSize);
        /// <summary>
        /// 获取数据缓冲区
        /// </summary>
        /// <param name="length">缓冲区字节长度</param>
        /// <returns>数据缓冲区</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected static memoryPool getMemoryPool(int length)
        {
            return length <= fastCSharp.config.appSetting.StreamBufferSize ? memoryPool.StreamBuffers : bigBuffers;
        }
        /// <summary>
        /// HTTP头部最大未定义项数
        /// </summary>
        internal static readonly int MaxHeaderCount = fastCSharp.config.http.Default.MaxHeaderCount;
        /// <summary>
        /// URI最大查询参数项数
        /// </summary>
        internal static readonly int MaxQueryCount = fastCSharp.config.http.Default.MaxQueryCount;
        /// <summary>
        /// HTTP头部缓存数据大小
        /// </summary>
        internal static readonly int HeaderBufferLength = fastCSharp.config.http.Default.HeaderBufferLength;
        /// <summary>
        /// HTTP头部参数起始位置
        /// </summary>
        internal static readonly int HeaderNameStartIndex = HeaderBufferLength + sizeof(int);
        /// <summary>
        /// URI查询参数起始位置
        /// </summary>
        internal unsafe static readonly int HeaderQueryStartIndex = HeaderNameStartIndex + MaxHeaderCount * sizeof(bufferIndex) * 2;
        /// <summary>
        /// HTTP套接字数量
        /// </summary>
        protected static int newCount;
        /// <summary>
        /// HTTP套接字数量
        /// </summary>
        public static int NewCount
        {
            get { return newCount; }
        }
        /// <summary>
        /// HTTP套接字安全流数量
        /// </summary>
        protected static int newSslCount;
        /// <summary>
        /// HTTP套接字安全流数量
        /// </summary>
        public static int NewSslCount
        {
            get { return newSslCount; }
        }
        /// <summary>
        /// 
        /// </summary>
        public static int PoolCount
        {
            get { return typePool<socket>.Count(); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static int SslPoolCount
        {
            get { return typePool<sslStream>.Count(); }
        }
        /// <summary>
        /// HTTP套接字队列次数
        /// </summary>
        internal static int queueCount;
        /// <summary>
        /// HTTP套接字队列次数
        /// </summary>
        public static int QueueCount { get { return queueCount; } }
        /// <summary>
        /// HTTP套接字安全流队列次数
        /// </summary>
        internal static int sslQueueCount;
        /// <summary>
        /// HTTP套接字安全流队列次数
        /// </summary>
        public static int SslQueueCount { get { return sslQueueCount; } }
        ///// <summary>
        ///// HTTP套接字超时次数
        ///// </summary>
        //protected static int timeoutCount;
        ///// <summary>
        ///// HTTP套接字超时次数
        ///// </summary>
        //public static int TimeoutCount { get { return timeoutCount; } }
        ///// <summary>
        ///// HTTP套接字安全流超时次数
        ///// </summary>
        //protected static int sslTimeoutCount;
        ///// <summary>
        ///// HTTP套接字安全流超时次数
        ///// </summary>
        //public static int SslTimeoutCount { get { return sslTimeoutCount; } }
        /// <summary>
        /// 每秒请求数量
        /// </summary>
        protected static secondCount secondCount = new secondCount(fastCSharp.config.http.Default.CountSeconds);
        /// <summary>
        /// 每秒请求数量
        /// </summary>
        public static int[] SecondCount
        {
            get { return secondCount.Counts; }
        }
        /// <summary>
        /// 每秒响应数量
        /// </summary>
        private static secondCount responseSecondCount = new secondCount(fastCSharp.config.http.Default.CountSeconds);
        /// <summary>
        /// 每秒响应数量
        /// </summary>
        public static int[] ResponseSecondCount
        {
            get { return responseSecondCount.Counts; }
        }
        /// <summary>
        /// HTTP头部接收超时队列
        /// </summary>
        internal static readonly timeoutQueue ReceiveTimeoutQueue = timeoutQueue.Get(fastCSharp.config.http.Default.ReceiveSeconds);
        /// <summary>
        /// HTTP头部接收超时时钟周期
        /// </summary>
        protected static readonly long receiveTimeoutQueueCallbackTimeoutTicks = ReceiveTimeoutQueue == null ? date.MinutesTicks : ReceiveTimeoutQueue.CallbackTimeoutTicks;
        /// <summary>
        /// HTTP头部二次接收超时队列
        /// </summary>
        internal static readonly timeoutQueue KeepAliveReceiveTimeoutQueue = timeoutQueue.Get(fastCSharp.config.http.Default.KeepAliveReceiveSeconds);
        /// <summary>
        /// HTTP头部二次接收超时时钟周期
        /// </summary>
        protected static readonly long keepAliveReceiveTimeoutQueueCallbackTimeoutTicks = KeepAliveReceiveTimeoutQueue == null ? date.MinutesTicks : KeepAliveReceiveTimeoutQueue.CallbackTimeoutTicks;
        /// <summary>
        /// WebSocket超时队列
        /// </summary>
        internal static readonly timeoutQueue WebSocketReceiveTimeoutQueue = timeoutQueue.Get(fastCSharp.config.http.Default.WebSocketReceiveSeconds);
        /// <summary>
        /// HTTP头部接收器
        /// </summary>
        internal abstract class headerReceiver
        {
            /// <summary>
            /// HTTP头部数据缓冲区
            /// </summary>
            protected byte[] buffer;
            /// <summary>
            /// 超时时间
            /// </summary>
            protected DateTime timeout;
            /// <summary>
            /// 接收数据结束位置
            /// </summary>
            public int ReceiveEndIndex;
            /// <summary>
            /// HTTP头部数据结束位置
            /// </summary>
            public int HeaderEndIndex;
            /// <summary>
            /// HTTP请求头部
            /// </summary>
            public requestHeader RequestHeader;
            /// <summary>
            /// 是否已经统计请求数量
            /// </summary>
            protected int isSecondCount;
            /// <summary>
            /// 接受头部换行数据
            /// </summary>
            protected abstract void receive();
        }
        /// <summary>
        /// HTTP头部接收器
        /// </summary>
        /// <typeparam name="socketType">套接字类型</typeparam>
        internal abstract class headerReceiver<socketType> : headerReceiver where socketType : socketBase
        {
            /// <summary>
            /// HTTP套接字
            /// </summary>
            protected socketType socket;
            /// <summary>
            /// HTTP头部接收器
            /// </summary>
            /// <param name="socket">HTTP套接字</param>
            public headerReceiver(socketType socket)
            {
                this.socket = socket;
                buffer = (RequestHeader = new requestHeader()).Buffer;
                RequestHeader.IsSsl = socket.IsSsl;
            }
            /// <summary>
            /// 开始接收数据
            /// </summary>
            public virtual void Receive()
            {
                //socket.startTime.Restart();
                if (socket.isNextRequest == 0) timeout = date.nowTime.Now.AddTicks(receiveTimeoutQueueCallbackTimeoutTicks);
                else
                {
                    timeout = date.nowTime.Now.AddTicks(keepAliveReceiveTimeoutQueueCallbackTimeoutTicks);
                    int endIndex = HeaderEndIndex + sizeof(int);
                    if ((ReceiveEndIndex -= endIndex) > 0)
                    {
                        System.Buffer.BlockCopy(buffer, endIndex, buffer, 0, ReceiveEndIndex);
                        HeaderEndIndex = 0;
                        secondCount.Add();
                        onReceive();
                        return;
                    }
                }
                ReceiveEndIndex = HeaderEndIndex = isSecondCount = 0;
                receive();
            }
            /// <summary>
            /// 接受头部数据处理
            /// </summary>
            protected unsafe void onReceive()
            {
                int searchEndIndex = ReceiveEndIndex - sizeof(int);
                if (HeaderEndIndex <= searchEndIndex)
                {
                    fixed (byte* dataFixed = buffer)
                    {
                        byte* start = dataFixed + HeaderEndIndex, searchEnd = dataFixed + searchEndIndex, end = dataFixed + ReceiveEndIndex;
                        *end = 13;
                        do
                        {
                            while (*start != 13) ++start;
                            if (start <= searchEnd)
                            {
                                if (*(int*)start == 0x0a0d0a0d)
                                {
                                    HeaderEndIndex = (int)(start - dataFixed);
                                    if (RequestHeader.Parse(HeaderEndIndex, ReceiveEndIndex, socket.servers.IsParseHeader))
                                    {
                                        if (RequestHeader.IsHeaderError) socket.headerError();
                                        else if (RequestHeader.IsRangeError) socket.responseError(response.state.RangeNotSatisfiable416);
                                        else
                                        {
                                            socket.DomainServer = socket.servers.GetServer(dataFixed + RequestHeader.host.StartIndex, RequestHeader.host.Length);
                                            if (socket.DomainServer == null) socket.headerUnknown();
                                            else socket.request();
                                        }
                                    }
                                    else socket.headerUnknown();
                                    return;
                                }
                                ++start;
                            }
                            else
                            {
                                HeaderEndIndex = (int)(start - dataFixed);
                                break;
                            }
                        }
                        while (true);
                    }
                }
                if (ReceiveEndIndex == HeaderBufferLength) socket.headerUnknown();
                else receive();
            }
        }
        /// <summary>
        /// 表单数据接收器
        /// </summary>
        protected abstract class formIdentityReceiver
        {
            /// <summary>
            /// 表单接收缓冲区
            /// </summary>
            protected byte[] buffer;
            /// <summary>
            /// 缓冲区内存池
            /// </summary>
            protected memoryPool memoryPool;
            /// <summary>
            /// 接收数据起始时间
            /// </summary>
            protected DateTime receiveStartTime;
            /// <summary>
            /// 表单数据内容长度
            /// </summary>
            protected int contentLength;
            /// <summary>
            /// 接收数据结束位置
            /// </summary>
            protected int receiveEndIndex;
            /// <summary>
            /// HTTP请求表单
            /// </summary>
            protected requestForm form;
            /// <summary>
            /// HTTP请求表单加载
            /// </summary>
            protected requestForm.ILoadForm loadForm;
        }
        /// <summary>
        /// 表单数据接收器
        /// </summary>
        /// <typeparam name="socketType">套接字类型</typeparam>
        protected abstract class formIdentityReceiver<socketType> : formIdentityReceiver where socketType : socketBase
        {
            /// <summary>
            /// HTTP套接字
            /// </summary>
            protected socketType socket;
            /// <summary>
            /// HTTP表单接收器
            /// </summary>
            /// <param name="socket">HTTP套接字</param>
            public formIdentityReceiver(socketType socket)
            {
                this.socket = socket;
                form = socket.form;
            }
            /// <summary>
            /// 表单接收错误
            /// </summary>
            protected void receiveError()
            {
                try
                {
                    loadForm.OnGetForm(null);
                }
                finally
                {
                    if (memoryPool != null) memoryPool.Push(ref buffer);
                    socket.headerError();
                }
            }
            /// <summary>
            /// 表单正常回调
            /// </summary>
            protected void callback()
            {
                long identity = form.Identity = socket.identity;
                try
                {
                    loadForm.OnGetForm(form);
                    return;
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                if (memoryPool != null) memoryPool.Push(ref buffer);
                socket.ResponseError(identity, response.state.ServerError500);
            }
            /// <summary>
            /// 解析表单数据
            /// </summary>
            /// <param name="dataToType">未知数据流转换类型</param>
            /// <returns></returns>
            public bool Parse(requestHeader.postType dataToType = requestHeader.postType.Data)
            {
                requestHeader header = socket.RequestHeader;
                switch (header.PostType)
                {
                    case requestHeader.postType.Json:
                        return form.Parse(buffer, receiveEndIndex, header.RequestEncoding, fastCSharp.config.web.QueryJsonName);
                    case requestHeader.postType.Xml:
                        return form.Parse(buffer, receiveEndIndex, header.RequestEncoding, fastCSharp.config.web.QueryXmlName);
                    case requestHeader.postType.Data:
                        if (receiveEndIndex != 0)
                        {
                            switch (dataToType)
                            {
                                case requestHeader.postType.Json:
                                    return form.Parse(buffer, receiveEndIndex, header.RequestEncoding, fastCSharp.config.web.QueryJsonName);
                                case requestHeader.postType.Xml:
                                    return form.Parse(buffer, receiveEndIndex, header.RequestEncoding, fastCSharp.config.web.QueryXmlName);
                            }
                        }
                        form.Buffer = buffer;
                        form.BufferSize = receiveEndIndex;
                        return true;
                    default:
                        return form.Parse(buffer, receiveEndIndex);
                }
                //receiveError();
            }
        }
        /// <summary>
        /// 数据接收器
        /// </summary>
        internal abstract class boundaryIdentityReceiver
        {
            /// <summary>
            /// 接受数据处理类型
            /// </summary>
            protected enum onReceiveType : byte
            {
                /// <summary>
                /// 接收第一个分割符
                /// </summary>
                OnFirstBoundary,
                /// <summary>
                /// 接收换行数据
                /// </summary>
                OnEnter,
                /// <summary>
                /// 接收表单值
                /// </summary>
                OnValue
            }
            /// <summary>
            /// 缓存文件名称前缀
            /// </summary>
            protected static readonly string cacheFileName = fastCSharp.config.pub.Default.CachePath + ((ulong)date.nowTime.Now.Ticks).toHex16();
            /// <summary>
            /// 表单接收缓冲区
            /// </summary>
            internal byte[] Buffer;
            /// <summary>
            /// 接收数据起始时间
            /// </summary>
            protected DateTime receiveStartTime;
            /// <summary>
            /// HTTP请求表单
            /// </summary>
            protected requestForm form;
            /// <summary>
            /// 当前处理表单值
            /// </summary>
            protected requestForm.value formValue;
            /// <summary>
            /// 当前表单值文件流
            /// </summary>
            protected FileStream fileStream;
            /// <summary>
            /// HTTP请求表单加载
            /// </summary>
            protected requestForm.ILoadForm loadForm;
            /// <summary>
            /// 数据分割符
            /// </summary>
            protected subArray<byte> boundary;
            /// <summary>
            /// 表单数据内容长度
            /// </summary>
            protected int contentLength;
            /// <summary>
            /// 接收数据结束位置
            /// </summary>
            protected int receiveEndIndex;
            /// <summary>
            /// 数据起始位置
            /// </summary>
            protected int startIndex;
            /// <summary>
            /// 当前数据位置
            /// </summary>
            protected int currentIndex;
            /// <summary>
            /// 当前接收数据字节长度
            /// </summary>
            protected int receiveLength;
            /// <summary>
            /// 表单值当前起始位置换行符标识
            /// </summary>
            protected int valueEnterIndex;
            /// <summary>
            /// 接受数据处理
            /// </summary>
            protected onReceiveType onReceiveData;
            /// <summary>
            /// 开始接收表单数据
            /// </summary>
            protected abstract void receive();
            /// <summary>
            /// 获取表单名称值
            /// </summary>
            /// <param name="dataFixed">数据</param>
            /// <param name="start">数据起始位置</param>
            /// <param name="end">数据结束位置</param>
            /// <returns>表单名称值,失败返回null</returns>
            protected unsafe subArray<byte> getFormNameValue(byte* dataFixed, byte* start, byte* end)
            {
                while (*start == ' ') ++start;
                if (*start == '=')
                {
                    while (*++start == ' ') ;
                    if (*start == '"')
                    {
                        byte* valueStart = ++start;
                        for (*end = (byte)'"'; *start != '"'; ++start) ;
                        if (start != end)
                        {
                            byte[] value = new byte[start - valueStart];
                            System.Buffer.BlockCopy(Buffer, (int)(valueStart - dataFixed), value, 0, value.Length);
                            return subArray<byte>.Unsafe(value, 0, value.Length);
                        }
                    }
                }
                return default(subArray<byte>);
            }
            /// <summary>
            /// 写文件
            /// </summary>
            internal abstract unsafe void WriteFile();
        }
        /// <summary>
        /// 数据接收器
        /// </summary>
        /// <typeparam name="socketType">套接字类型</typeparam>
        internal abstract class boundaryIdentityReceiver<socketType> : boundaryIdentityReceiver where socketType : socketBase
        {
            /// <summary>
            /// HTTP套接字
            /// </summary>
            protected socketType socket;
            /// <summary>
            /// HTTP数据接收器
            /// </summary>
            /// <param name="socket">HTTP套接字</param>
            public boundaryIdentityReceiver(socketType socket)
            {
                this.socket = socket;
                form = socket.form;
            }
            /// <summary>
            /// 数据接收错误
            /// </summary>
            protected void error()
            {
                try
                {
                    form.Clear();
                    loadForm.OnGetForm(null);
                }
                finally
                {
                    bigBuffers.Push(ref Buffer);
                    pub.Dispose(ref fileStream);
                    socket.headerError();
                }
            }
            /// <summary>
            /// 表单数据接收完成
            /// </summary>
            private void boundaryReceiverFinally()
            {
                if (receiveLength == contentLength)
                {
                    long identity = form.Identity = socket.identity;
                    try
                    {
                        pub.Dispose(ref fileStream);
                        form.SetFileValue();
                        loadForm.OnGetForm(form);
                        return;
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, null, false);
                    }
                    bigBuffers.Push(ref Buffer);
                    socket.ResponseError(identity, response.state.ServerError500);
                }
                else this.error();
            }
            /// <summary>
            /// 接收第一个分割符
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected void onFirstBoundary()
            {
                if (receiveEndIndex >= boundary.length + 4) checkFirstBoundary();
                else
                {
                    onReceiveData = onReceiveType.OnFirstBoundary;
                    receive();
                }
            }
            /// <summary>
            /// 检测第一个分隔符
            /// </summary>
            private unsafe void checkFirstBoundary()
            {
                int boundaryLength4 = boundary.length + 4;
                fixed (byte* bufferFixed = Buffer, boundaryFixed = boundary.array)
                {
                    if (*(short*)bufferFixed == '-' + ('-' << 8) && fastCSharp.unsafer.memory.Equal(boundaryFixed + boundary.startIndex, bufferFixed + 2, boundary.length))
                    {
                        int endValue = (int)*(short*)(bufferFixed + 2 + boundary.length);
                        if (endValue == 0x0a0d)
                        {
                            startIndex = currentIndex = boundaryLength4;
                            onEnter();
                            return;
                        }
                        else if (((endValue ^ ('-' + ('-' << 8))) | (receiveEndIndex ^ boundaryLength4)) == 0)
                        {
                            boundaryReceiverFinally();
                            return;
                        }
                    }
                }
                error();
            }
            /// <summary>
            /// 查找换行处理
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private unsafe void onEnter()
            {
                int length = receiveEndIndex - currentIndex;
                if (length > sizeof(int)) checkEnter();
                else receiveEnter();
            }
            /// <summary>
            /// 继续接收换行
            /// </summary>
            private unsafe void receiveEnter()
            {
                int length = receiveEndIndex - startIndex;
                if (length >= HeaderBufferLength) error();
                else
                {
                    if (receiveEndIndex == bigBuffers.Size - sizeof(int))
                    {
                        fixed (byte* bufferFixed = Buffer)
                        {
                            unsafer.memory.Copy(bufferFixed + startIndex, bufferFixed, length);
                        }
                        currentIndex -= startIndex;
                        receiveEndIndex = length;
                        startIndex = 0;
                    }
                    onReceiveData = onReceiveType.OnEnter;
                    receive();
                }
            }
            /// <summary>
            /// 查找换行
            /// </summary>
            private unsafe void checkEnter()
            {
                int searchEndIndex = receiveEndIndex - sizeof(int);
                fixed (byte* dataFixed = Buffer)
                {
                    byte* start = dataFixed + currentIndex, searchEnd = dataFixed + searchEndIndex, end = dataFixed + receiveEndIndex;
                    *end = 13;
                    do
                    {
                        while (*start != 13) ++start;
                        if (start <= searchEnd)
                        {
                            if (*(int*)start == 0x0a0d0a0d)
                            {
                                currentIndex = (int)(start - dataFixed);
                                parseName();
                                return;
                            }
                            ++start;
                        }
                        else
                        {
                            currentIndex = (int)(start - dataFixed);
                            break;
                        }
                    }
                    while (true);
                }
                receiveEnter();
            }
            /// <summary>
            /// 解析表单名称
            /// </summary>
            private unsafe void parseName()
            {
                formValue.Null();
                try
                {
                    fixed (byte* dataFixed = Buffer)
                    {
                        byte* start = dataFixed + startIndex, end = dataFixed + currentIndex;
                        *end = (byte)';';
                        do
                        {
                            while (*start == ' ') ++start;
                            if (start == end) break;
                            if (*(int*)start == ('n' | ('a' << 8) | ('m' << 16) | ('e' << 24)))
                            {
                                formValue.Name = getFormNameValue(dataFixed, start += sizeof(int), end);
                                start += formValue.Name.length + 3;
                            }
                            else if (((*(int*)start ^ ('f' | ('i' << 8) | ('l' << 16) | ('e' << 24)))
                                | (*(int*)(start + sizeof(int)) ^ ('n' | ('a' << 8) | ('m' << 16) | ('e' << 24)))) == 0)
                            {
                                formValue.FileName = getFormNameValue(dataFixed, start += sizeof(int) * 2, end);
                                start += formValue.FileName.length + 3;
                            }
                            for (*end = (byte)';'; *start != ';'; ++start) ;
                        }
                        while (start++ != end);
                        *end = 13;
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                if (formValue.Name.array == null) this.error();
                else
                {
                    startIndex = valueEnterIndex = (currentIndex += 4);
                    onValue();
                }
            }
            /// <summary>
            /// 接收表单值处理
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            private void onValue()
            {
                if (valueEnterIndex >= 0 ? receiveEndIndex - valueEnterIndex >= (boundary.length + 4) : (receiveEndIndex - currentIndex >= (boundary.length + 6)))
                {
                    checkValue();
                }
                else
                {
                    receiveValue();
                }
            }
            /// <summary>
            /// 继续接收数据
            /// </summary>
            private unsafe void receiveValue()
            {
                try
                {
                    if (receiveEndIndex == bigBuffers.Size - sizeof(int))
                    {
                        if (startIndex == 0)
                        {
                            fastCSharp.threading.task.Tiny.Add(this, thread.callType.HttpSocketBoundaryIdentityReceiverWriteFile);
                            return;
                        }
                        int length = receiveEndIndex - startIndex;
                        fixed (byte* bufferFixed = Buffer)
                        {
                            unsafer.memory.Copy(bufferFixed + startIndex, bufferFixed, length);
                        }
                        currentIndex -= startIndex;
                        valueEnterIndex -= startIndex;
                        receiveEndIndex = length;
                        startIndex = 0;
                    }
                    onReceiveData = onReceiveType.OnValue;
                    receive();
                    return;
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                this.error();
            }
            /// <summary>
            /// 写文件
            /// </summary>
            internal override unsafe void WriteFile()
            {
                try
                {
                    if (fileStream == null)
                    {
                        formValue.SaveFileName = loadForm.GetSaveFileName(ref formValue);
                        if (formValue.SaveFileName == null) formValue.SaveFileName = cacheFileName + ((ulong)fastCSharp.pub.Identity).toHex16();
                        fileStream = new FileStream(formValue.SaveFileName, FileMode.CreateNew, FileAccess.Write, FileShare.None, (int)file.BytesPerCluster(formValue.SaveFileName), FileOptions.None);
                    }
                    if (valueEnterIndex > 0)
                    {
                        fileStream.Write(Buffer, 0, valueEnterIndex);
                        fixed (byte* bufferFixed = Buffer)
                        {
                            unsafer.memory.Copy(bufferFixed + valueEnterIndex, bufferFixed, receiveEndIndex -= valueEnterIndex);
                        }
                        valueEnterIndex = 0;
                    }
                    else
                    {
                        fileStream.Write(Buffer, 0, receiveEndIndex);
                        receiveEndIndex = 0;
                        valueEnterIndex = -Buffer.Length;
                    }
                    startIndex = 0;
                    currentIndex = receiveEndIndex;
                    onReceiveData = onReceiveType.OnValue;
                    receive();
                    return;
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                this.error();
            }
            /// <summary>
            /// 接收表单值处理
            /// </summary>
            private unsafe void checkValue()
            {
                int boundaryLength2 = boundary.length + 2;
                fixed (byte* bufferFixed = Buffer, boundaryFixed = boundary.array)
                {
                    byte* boundaryStart = boundaryFixed + boundary.startIndex;
                    byte* start = bufferFixed + currentIndex, end = bufferFixed + receiveEndIndex, last = bufferFixed + valueEnterIndex;
                    *end-- = 13;
                    do
                    {
                        while (*start != 13) ++start;
                        if (start >= end) break;
                        if ((int)(start - last) == boundaryLength2 && *(short*)last == ('-') + ('-' << 8)
                            && fastCSharp.unsafer.memory.Equal(boundaryStart, last + 2, boundary.length) && *(start + 1) == 10)
                        {
                            currentIndex = (int)(last - bufferFixed) - 2;
                            if (getValue())
                            {
                                startIndex = currentIndex = (int)(start - bufferFixed) + 2;
                                onEnter();
                            }
                            else
                            {
                                error();
                            }
                            return;
                        }
                        last = *++start == 10 ? ++start : (bufferFixed - Buffer.Length);
                    }
                    while (true);
                    int hash = (*(int*)(end -= 3) ^ ('-') + ('-' << 8) + 0x0a0d0000);
                    if ((hash | (*(int*)(end -= boundary.length + sizeof(int)) ^ 0x0a0d + ('-' << 16) + ('-' << 24))) == 0
                         && fastCSharp.unsafer.memory.Equal(boundaryStart, end + sizeof(int), boundary.length))
                    {
                        currentIndex = (int)(end - bufferFixed);
                        if (getValue())
                        {
                            boundaryReceiverFinally();
                        }
                        else
                        {
                            error();
                        }
                        return;
                    }
                    valueEnterIndex = (int)(last - bufferFixed);
                    currentIndex = (int)(start - bufferFixed);
                }
                receiveValue();
            }
            /// <summary>
            /// 获取表单值
            /// </summary>
            /// <returns>是否成功</returns>
            private unsafe bool getValue()
            {
                try
                {
                    if (fileStream == null)
                    {
                        byte[] value = new byte[currentIndex - startIndex];
                        System.Buffer.BlockCopy(Buffer, startIndex, value, 0, value.Length);
                        formValue.Value.UnsafeSet(value, 0, value.Length);
                    }
                    else
                    {
                        fileStream.Write(Buffer, startIndex, currentIndex - startIndex);
                        fileStream.Dispose();
                        fileStream = null;
                    }
                    if (formValue.FileName.length == 0)
                    {
                        if (formValue.Name.length == 1)
                        {
                            switch (formValue.Name.array[formValue.Name.startIndex])
                            {
                                case (byte)fastCSharp.config.web.QueryJsonName:
                                    form.Parse(formValue.Value.array, formValue.Value.startIndex, formValue.Value.length, socket.RequestHeader.RequestEncoding, fastCSharp.config.web.QueryJsonName);
                                    break;
                                case (byte)fastCSharp.config.web.QueryXmlName:
                                    form.Parse(formValue.Value.array, formValue.Value.startIndex, formValue.Value.length, socket.RequestHeader.RequestEncoding, fastCSharp.config.web.QueryXmlName);
                                    break;
                                default:
                                    form.FormValues.Add(formValue);
                                    break;
                            }
                        }
                        else form.FormValues.Add(formValue);
                    }
                    else  form.Files.Add(formValue);
                    return true;
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                return false;
            }
            /// <summary>
            /// 接受数据处理
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected void callOnReceiveData()
            {
                switch (onReceiveData)
                {
                    case onReceiveType.OnFirstBoundary: onFirstBoundary(); return;
                    case onReceiveType.OnEnter: onEnter(); return;
                    case onReceiveType.OnValue: onValue(); return;
                }
            }
        }
        /// <summary>
        /// 数据发送器
        /// </summary>
        protected abstract class dataSender
        {
            /// <summary>
            /// 发送数据回调类型
            /// </summary>
            internal enum onSendType : byte
            {
                /// <summary>
                /// 关闭套接字
                /// </summary>
                Close,
                /// <summary>
                /// 发送HTTP响应内容
                /// </summary>
                ResponseBody,
                /// <summary>
                /// 处理下一个请求
                /// </summary>
                Next,
            }
            /// <summary>
            /// HTTP套接字
            /// </summary>
            private socketBase socket;
            /// <summary>
            /// 发送数据起始时间
            /// </summary>
            protected DateTime sendStartTime;
            /// <summary>
            /// 正在发送的文件流
            /// </summary>
            protected FileStream fileStream;
            /// <summary>
            /// 待发送文件数据字节数
            /// </summary>
            protected long fileSize;
            /// <summary>
            /// 缓冲区内存池
            /// </summary>
            protected memoryPool memoryPool;
            /// <summary>
            /// 发送数据缓冲区
            /// </summary>
            protected byte[] buffer;
            /// <summary>
            /// 发送数据起始位置
            /// </summary>
            protected int sendIndex;
            /// <summary>
            /// 发送数据结束位置
            /// </summary>
            protected int sendEndIndex;
            /// <summary>
            /// 已经发送数据长度
            /// </summary>
            protected long sendLength;
            /// <summary>
            /// 发送数据回调类型
            /// </summary>
            internal onSendType onSend;
            /// <summary>
            /// 数据发送器
            /// </summary>
            /// <param name="socket"></param>
            protected dataSender(socketBase socket)
            {
                this.socket = socket;
            }
            /// <summary>
            /// 发送数据完毕
            /// </summary>
            /// <param name="isSend">是否发送成功</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected void send(bool isSend)
            {
                if (memoryPool == null) buffer = null;
                else memoryPool.Push(ref buffer);
                callOnSend(isSend);
            }
            /// <summary>
            /// 读取文件并发送数据
            /// </summary>
            protected void readFile()
            {
                if (fileSize == 0) sendFile(true);
                else
                {
                    try
                    {
                        sendEndIndex = memoryPool.StreamBuffers.Size - sizeof(int);
                        if (sendEndIndex > fileSize) sendEndIndex = (int)fileSize;
                        if (fileStream.Read(buffer, 0, sendEndIndex) == sendEndIndex)
                        {
                            fileSize -= sendEndIndex;
                            sendIndex = 0;
                            sendFile();
                            return;
                        }
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, null, false);
                    }
                    sendFile(false);
                }
            }
            /// <summary>
            /// 开始发送文件数据
            /// </summary>
            protected abstract void sendFile();
            /// <summary>
            /// 文件发送数据完毕
            /// </summary>
            /// <param name="isSend">是否发送成功</param>
            protected void sendFile(bool isSend)
            {
                pub.Dispose(ref fileStream);
                send(isSend);
            }
            /// <summary>
            /// 发送数据回调
            /// </summary>
            /// <param name="isSend"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            protected void callOnSend(bool isSend)
            {
                switch (onSend)
                {
                    case onSendType.Close: socket.headerError(); return;
                    case onSendType.ResponseBody: socket.responseBody(isSend); return;
                    case onSendType.Next:
                        if (isSend) socket.next();
                        else socket.end();
                        return;
                }
            }
            /// <summary>
            /// 发送数据回调
            /// </summary>
            /// <param name="onSend"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void onSendFalse(onSendType onSend)
            {
                switch (onSend)
                {
                    case onSendType.Close: socket.headerError(); return;
                    case onSendType.ResponseBody: socket.responseBody(false); return;
                    case onSendType.Next: socket.end(); return;
                }
            }
        }
        /// <summary>
        /// 数据发送器
        /// </summary>
        /// <typeparam name="socketType">套接字类型</typeparam>
        protected abstract class dataSender<socketType> : dataSender where socketType : socketBase
        {
            /// <summary>
            /// HTTP套接字
            /// </summary>
            protected socketType socket;
            /// <summary>
            /// 数据发送器
            /// </summary>
            /// <param name="socket">HTTP套接字</param>
            public dataSender(socketType socket)
                : base(socket)
            {
                this.socket = socket;
            }
        }
        /// <summary>
        /// WebSocket请求接收器
        /// </summary>
        protected abstract class webSocketIdentityReceiver
        {
            /// <summary>
            /// 发送消息
            /// </summary>
            internal unsafe struct sender
            {
                /// <summary>
                /// 消息缓冲区
                /// </summary>
                public byte[] Buffer;
                /// <summary>
                /// 消息缓冲区
                /// </summary>
                public byte* BufferFixed;
                /// <summary>
                /// 消息缓冲流
                /// </summary>
                public unmanagedStream Stream;
                /// <summary>
                /// 发送消息
                /// </summary>
                /// <param name="message"></param>
                public void Send(ref webSocket.socket.message message)
                {
                    if (message.Message == null) send(message.Data);
                    else send(message.Message);
                }
                /// <summary>
                /// 发送消息
                /// </summary>
                /// <param name="message"></param>
                private void send(string message)
                {
                    int length = System.Text.Encoding.UTF8.GetByteCount(message);
                    if (length > 125)
                    {
                        if (length <= ushort.MaxValue)
                        {
                            Stream.UnsafeWrite((ushort)(0x81 + (126 << 8)));
                            Stream.UnsafeWrite((byte)(length >> 8));
                            Stream.UnsafeWrite((byte)length);
                        }
                        else
                        {
                            Stream.UnsafeWrite(0x81 + (127 << 8));
                            Stream.UnsafeWrite((ushort)0);
                            Stream.UnsafeWrite((byte)(length >> 24));
                            Stream.UnsafeWrite((byte)(length >> 16));
                            Stream.UnsafeWrite((byte)(length >> 8));
                            Stream.UnsafeWrite((byte)length);
                        }
                    }
                    else
                    {
                        Stream.UnsafeWrite((byte)0x81);
                        Stream.UnsafeWrite((byte)length);
                    }
                    Stream.PrepLength(length);
                    fixed (char* stringFixed = message) System.Text.Encoding.UTF8.GetBytes(stringFixed, message.Length, Stream.CurrentData, length);
                    Stream.UnsafeAddLength(length);
                }
                /// <summary>
                /// 发送消息
                /// </summary>
                /// <param name="data"></param>
                private void send(byte[] data)
                {
                    int length = data.Length;
                    if (length > 125)
                    {
                        if (length <= ushort.MaxValue)
                        {
                            Stream.UnsafeWrite((ushort)(0x82 + (126 << 8)));
                            Stream.UnsafeWrite((byte)(length >> 8));
                            Stream.UnsafeWrite((byte)length);
                        }
                        else
                        {
                            Stream.UnsafeWrite(0x82 + (127 << 8));
                            Stream.UnsafeWrite((ushort)0);
                            Stream.UnsafeWrite((byte)(length >> 24));
                            Stream.UnsafeWrite((byte)(length >> 16));
                            Stream.UnsafeWrite((byte)(length >> 8));
                            Stream.UnsafeWrite((byte)length);
                        }
                    }
                    else
                    {
                        Stream.UnsafeWrite((byte)0x82);
                        Stream.UnsafeWrite((byte)length);
                    }
                    Stream.WriteNotNull(data);
                }
                /// <summary>
                /// 获取消息数据
                /// </summary>
                /// <returns></returns>
                public byte[] GetData()
                {
                    if (Stream.Length <= Buffer.Length)
                    {
                        if (Stream.data.data != BufferFixed) fastCSharp.unsafer.memory.Copy(Stream.data.Byte, BufferFixed, Stream.Length);
                        return Buffer;
                    }
                    return Stream.GetArray();
                }
            }
            /// <summary>
            /// 关闭连接数据
            /// </summary>
            protected static readonly byte[] closeData = new byte[] { 0x88, 0 };
            /// <summary>
            /// WebSocket调用
            /// </summary>
            internal webSocket.socket WebSocket;
            /// <summary>
            /// 表单接收缓冲区
            /// </summary>
            protected byte[] buffer;
            /// <summary>
            /// 发送数据访问锁
            /// </summary>
            protected readonly object sendLock = new object();
            /// <summary>
            /// 接收数据结束位置
            /// </summary>
            protected int receiveEndIndex;
            /// <summary>
            /// 开始接收数据
            /// </summary>
            protected abstract void receive();
        }
        /// <summary>
        /// WebSocket请求接收器
        /// </summary>
        /// <typeparam name="socketType">套接字类型</typeparam>
        protected abstract class webSocketIdentityReceiver<socketType> : webSocketIdentityReceiver where socketType : socketBase
        {
            /// <summary>
            /// HTTP套接字
            /// </summary>
            protected socketType socket;
            /// <summary>
            /// WebSocket请求接收器
            /// </summary>
            /// <param name="socket">HTTP套接字</param>
            public webSocketIdentityReceiver(socketType socket)
            {
                this.socket = socket;
                buffer = socket.Buffer;
            }
        }
        /// <summary>
        /// 服务器类型(长度不能为0)
        /// </summary>
        private const string fastCSharpServer = @"Server: fastCSharp.http[C#]/1.0
";
        /// <summary>
        /// HTTP服务版本号
        /// </summary>
        private const string httpVersionString = "HTTP/1.1";
        /// <summary>
        /// HTTP服务版本号
        /// </summary>
        /// <param name="buffer"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe static void writeHttpVersion(byte* buffer)
        {//HTTP/1.1
            *(int*)buffer = 'H' + ('T' << 8) + ('T' << 16) + ('P' << 24);
            *(int*)(buffer + sizeof(int)) = '/' + ('1' << 8) + ('.' << 16) + ('1' << 24);
        }
        /// <summary>
        /// HTTP服务版本号数据长度
        /// </summary>
        private const int httpVersionSize = sizeof(int) * 2;
        /// <summary>
        /// HTTP每4秒最小表单数据接收字节数
        /// </summary>
        protected static readonly int minReceiveSizePerSecond4 = fastCSharp.config.http.Default.MinReceiveSizePerSecond * 4;
        /// <summary>
        /// 错误输出缓存数据
        /// </summary>
        protected static readonly byte[][] errorResponseDatas;
        /// <summary>
        /// 搜索引擎404提示
        /// </summary>
        protected static readonly byte[] searchEngine404Data;
        /// <summary>
        /// 服务器类型
        /// </summary>
        private static pointer responseServer;
        /// <summary>
        /// 服务器类型
        /// </summary>
        private static readonly byte[] responseServerEnd = (fastCSharpServer + @"Content-Length: 0

").getBytes();
        /// <summary>
        /// 100 Continue确认
        /// </summary>
        private static readonly byte[] continue100 = (httpVersionString + fastCSharp.Enum<response.state, response.stateInfo>.Array((byte)response.state.Continue100).Text + @"
").getBytes();
        /// <summary>
        /// WebSocket握手确认
        /// </summary>
        private static readonly byte[] webSocket101 = (httpVersionString + fastCSharp.Enum<response.state, response.stateInfo>.Array((byte)response.state.WebSocket101).Text + @"Connection: Upgrade
Upgrade: WebSocket
" + fastCSharpServer + @"Sec-WebSocket-Accept: ").getBytes();
        /// <summary>
        /// WebSocket确认哈希值
        /// </summary>
        private static readonly byte[] webSocketKey = ("258EAFA5-E914-47DA-95CA-C5AB0DC85B11").getBytes();
        /// <summary>
        /// HTTP响应输出内容长度名称
        /// </summary>
        /// <param name="buffer"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe static void writeContentLength(byte* buffer)
        {//Content-Length
            *(int*)buffer = 'C' + ('o' << 8) + ('n' << 16) + ('t' << 24);
            *(int*)(buffer + sizeof(int)) = 'e' + ('n' << 8) + ('t' << 16) + ('-' << 24);
            *(int*)(buffer + sizeof(int) * 2) = 'L' + ('e' << 8) + ('n' << 16) + ('g' << 24);
            *(int*)(buffer + sizeof(int) * 3) = 't' + ('h' << 8) + (':' << 16) + (' ' << 24);
        }
        /// <summary>
        /// HTTP响应输出内容长度名称数据长度
        /// </summary>
        private const int contentLengthSize = sizeof(int) * 4;
        /// <summary>
        /// HTTP响应输出日期名称
        /// </summary>
        /// <param name="buffer"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe static void writeDate(byte* buffer)
        {//Date: 
            *(int*)buffer = 'D' + ('a' << 8) + ('t' << 16) + ('e' << 24);
            *(short*)(buffer + sizeof(int)) = ':' + (' ' << 8);
        }
        /// <summary>
        /// HTTP响应输出日期名称数据长度
        /// </summary>
        private const int dateSize = sizeof(int) + sizeof(short);
        /// <summary>
        /// HTTP响应输出最后修改名称
        /// </summary>
        /// <param name="buffer"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe static void writeLastModified(byte* buffer)
        {//Last-Modified: 
            *(int*)buffer = 'L' + ('a' << 8) + ('s' << 16) + ('t' << 24);
            *(int*)(buffer + sizeof(int)) = '-' + ('M' << 8) + ('o' << 16) + ('d' << 24);
            *(int*)(buffer + sizeof(int) * 2) = 'i' + ('f' << 8) + ('i' << 16) + ('e' << 24);
            *(int*)(buffer + sizeof(int) * 3) = 'd' + (':' << 8) + (' ' << 16);
        }
        /// <summary>
        /// HTTP响应输出最后修改名称数据长度
        /// </summary>
        private const int lastModifiedSize = sizeof(int) * 3 + 3;
        /// <summary>
        /// 重定向名称
        /// </summary>
        /// <param name="buffer"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe static void writeLocation(byte* buffer)
        {//Location: 
            *(int*)buffer = 'L' + ('o' << 8) + ('c' << 16) + ('a' << 24);
            *(int*)(buffer + sizeof(int)) = 't' + ('i' << 8) + ('o' << 16) + ('n' << 24);
            *(short*)(buffer + sizeof(int) * 2) = ':' + (' ' << 8);
        }
        /// <summary>
        /// 重定向名称数据长度
        /// </summary>
        private const int locationSize = sizeof(int) * 2 + sizeof(short);
        /// <summary>
        /// 缓存参数名称
        /// </summary>
        /// <param name="buffer"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe static void writeCacheControl(byte* buffer)
        {//Cache-Control
            *(int*)buffer = 'C' + ('a' << 8) + ('c' << 16) + ('h' << 24);
            *(int*)(buffer + sizeof(int)) = 'e' + ('-' << 8) + ('C' << 16) + ('o' << 24);
            *(int*)(buffer + sizeof(int) * 2) = 'n' + ('t' << 8) + ('r' << 16) + ('o' << 24);
            *(int*)(buffer + sizeof(int) * 3) = 'l' + (':' << 8) + (' ' << 16);
        }
        /// <summary>
        /// 缓存参数名称数据长度
        /// </summary>
        private const int cacheControlSize = sizeof(int) * 3 + 3;
        /// <summary>
        /// 内容类型名称
        /// </summary>
        /// <param name="buffer"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe static void writeContentType(byte* buffer)
        {//Content-Type
            *(int*)buffer = 'C' + ('o' << 8) + ('n' << 16) + ('t' << 24);
            *(int*)(buffer + sizeof(int)) = 'e' + ('n' << 8) + ('t' << 16) + ('-' << 24);
            *(int*)(buffer + sizeof(int) * 2) = 'T' + ('y' << 8) + ('p' << 16) + ('e' << 24);
            *(short*)(buffer + sizeof(int) * 3) = ':' + (' ' << 8);
        }
        /// <summary>
        /// 内容类型名称数据长度
        /// </summary>
        private const int contentTypeSize = sizeof(int) * 3 + sizeof(short);
        /// <summary>
        /// 内容压缩编码名称
        /// </summary>
        /// <param name="buffer"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe static void writeContentEncoding(byte* buffer)
        {//Content-Encoding
            *(int*)buffer = 'C' + ('o' << 8) + ('n' << 16) + ('t' << 24);
            *(int*)(buffer + sizeof(int)) = 'e' + ('n' << 8) + ('t' << 16) + ('-' << 24);
            *(int*)(buffer + sizeof(int) * 2) = 'E' + ('n' << 8) + ('c' << 16) + ('o' << 24);
            *(int*)(buffer + sizeof(int) * 3) = 'd' + ('i' << 8) + ('n' << 16) + ('g' << 24);
            *(short*)(buffer + sizeof(int) * 4) = ':' + (' ' << 8);
        }
        /// <summary>
        /// 内容压缩编码名称数据长度
        /// </summary>
        private const int contentEncodingSize = sizeof(int) * 4 + sizeof(short);
        /// <summary>
        /// 缓存匹配标识名称
        /// </summary>
        /// <param name="buffer"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe static void writeETag(byte* buffer)
        {//ETag
            *(int*)buffer = 'E' + ('T' << 8) + ('a' << 16) + ('g' << 24);
            *(int*)(buffer + sizeof(int)) = ':' + (' ' << 8) + ('"' << 16);
        }
        /// <summary>
        /// 缓存匹配标识名称数据长度
        /// </summary>
        private const int eTagSize = sizeof(int) + 3;
        /// <summary>
        /// 内容描述名称
        /// </summary>
        /// <param name="buffer"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe static void writeContentDisposition(byte* buffer)
        {//Content-Disposition
            *(int*)buffer = 'C' + ('o' << 8) + ('n' << 16) + ('t' << 24);
            *(int*)(buffer + sizeof(int)) = 'e' + ('n' << 8) + ('t' << 16) + ('-' << 24);
            *(int*)(buffer + sizeof(int) * 2) = 'D' + ('i' << 8) + ('s' << 16) + ('p' << 24);
            *(int*)(buffer + sizeof(int) * 3) = 'o' + ('s' << 8) + ('i' << 16) + ('t' << 24);
            *(int*)(buffer + sizeof(int) * 4) = 'i' + ('o' << 8) + ('n' << 16) + (':' << 24);
            *(buffer + sizeof(int) * 5) = (byte)' ';
        }
        /// <summary>
        /// 内容描述名称数据长度
        /// </summary>
        private const int contentDispositionSize = sizeof(int) * 5 + 1;
        /// <summary>
        /// 跨域权限控制
        /// </summary>
        /// <param name="buffer"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe static void writeAccessControlAllowOrigin(byte* buffer)
        {//Access-Control-Allow-Origin
            *(int*)buffer = 'A' + ('c' << 8) + ('c' << 16) + ('e' << 24);
            *(int*)(buffer + sizeof(int)) = 's' + ('s' << 8) + ('-' << 16) + ('C' << 24);
            *(int*)(buffer + sizeof(int) * 2) = 'o' + ('n' << 8) + ('t' << 16) + ('r' << 24);
            *(int*)(buffer + sizeof(int) * 3) = 'o' + ('l' << 8) + ('-' << 16) + ('A' << 24);
            *(int*)(buffer + sizeof(int) * 4) = 'l' + ('l' << 8) + ('o' << 16) + ('w' << 24);
            *(int*)(buffer + sizeof(int) * 5) = '-' + ('O' << 8) + ('r' << 16) + ('i' << 24);
            *(int*)(buffer + sizeof(int) * 6) = 'g' + ('i' << 8) + ('n' << 16) + (':' << 24);
            *(buffer + sizeof(int) * 7) = (byte)' ';
        }
        /// <summary>
        /// 跨域权限控制数据长度
        /// </summary>
        private const int accessControlAllowOriginSize = sizeof(int) * 7 + 1;
        /// <summary>
        /// 请求范围名称
        /// </summary>
        /// <param name="buffer"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe static void writeRange(byte* buffer)
        {//Accept-Ranges: bytes
            //Content-Range: bytes 
            *(int*)buffer = 'A' + ('c' << 8) + ('c' << 16) + ('e' << 24);
            *(int*)(buffer + sizeof(int)) = 'p' + ('t' << 8) + ('-' << 16) + ('R' << 24);
            *(int*)(buffer + sizeof(int) * 2) = 'a' + ('n' << 8) + ('g' << 16) + ('e' << 24);
            *(int*)(buffer + sizeof(int) * 3) = 's' + (':' << 8) + (' ' << 16) + ('b' << 24);
            *(int*)(buffer + sizeof(int) * 4) = 'y' + ('t' << 8) + ('e' << 16) + ('s' << 24);
            *(int*)(buffer + sizeof(int) * 5) = 0x0a0d + ('C' << 16) + ('o' << 24);
            *(int*)(buffer + sizeof(int) * 6) = 'n' + ('t' << 8) + ('e' << 16) + ('n' << 24);
            *(int*)(buffer + sizeof(int) * 7) = 't' + ('-' << 8) + ('R' << 16) + ('a' << 24);
            *(int*)(buffer + sizeof(int) * 8) = 'n' + ('g' << 8) + ('e' << 16) + (':' << 24);
            *(int*)(buffer + sizeof(int) * 9) = ' ' + ('b' << 8) + ('y' << 16) + ('t' << 24);
            *(short*)(buffer + sizeof(int) * 10) = 'e' + ('s' << 8);
        }
        /// <summary>
        /// 请求范围名称数据长度
        /// </summary>
        private const int rangeSize = sizeof(int) * 10 + sizeof(short);
        /// <summary>
        /// HTTP响应输出保持连接
        /// </summary>
        /// <param name="buffer"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe static void writeKeepAlive(byte* buffer)
        {//Connection: Keep-Alive
            *(int*)buffer = 'C' + ('o' << 8) + ('n' << 16) + ('n' << 24);
            *(int*)(buffer + sizeof(int)) = 'e' + ('c' << 8) + ('t' << 16) + ('i' << 24);
            *(int*)(buffer + sizeof(int) * 2) = 'o' + ('n' << 8) + (':' << 16) + (' ' << 24);
            *(int*)(buffer + sizeof(int) * 3) = 'K' + ('e' << 8) + ('e' << 16) + ('p' << 24);
            *(int*)(buffer + sizeof(int) * 4) = '-' + ('A' << 8) + ('l' << 16) + ('i' << 24);
            *(int*)(buffer + sizeof(int) * 5) = 'v' + ('e' << 8) + 0x0a0d0000;
        }
        /// <summary>
        /// HTTP响应输出保持连接数据长度
        /// </summary>
        private const int keepAliveSize = sizeof(int) * 6;
        /// <summary>
        /// HTTP响应输出Cookie名称
        /// </summary>
        /// <param name="buffer"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe static void writeSetCookie(byte* buffer)
        {//Set-Cookie
            *(int*)buffer = 'S' + ('e' << 8) + ('t' << 16) + ('-' << 24);
            *(int*)(buffer + sizeof(int)) = 'C' + ('o' << 8) + ('o' << 16) + ('k' << 24);
            *(int*)(buffer + sizeof(int) * 2) = 'i' + ('e' << 8) + (':' << 16) + (' ' << 24);
        }
        /// <summary>
        /// HTTP响应输出Cookie名称数据长度
        /// </summary>
        private const int setCookieSize = sizeof(int) * 3;
        /// <summary>
        /// Cookie域名
        /// </summary>
        /// <param name="buffer"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe static void writeCookieDomain(byte* buffer)
        {//; Domain=
            *(int*)buffer = ';' + (' ' << 8) + ('D' << 16) + ('o' << 24);
            *(int*)(buffer + sizeof(int)) = 'm' + ('a' << 8) + ('i' << 16) + ('n' << 24);
            *(buffer + sizeof(int) * 2) = (byte)'=';
        }
        /// <summary>
        /// Cookie域名数据长度
        /// </summary>
        private const int cookieDomainSize = sizeof(int) * 2 + 1;
        /// <summary>
        /// Cookie有效路径
        /// </summary>
        /// <param name="buffer"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe static void writeCookiePath(byte* buffer)
        {//; Path=
            *(int*)buffer = ';' + (' ' << 8) + ('P' << 16) + ('a' << 24);
            *(int*)(buffer + sizeof(int)) = 't' + ('h' << 8) + ('=' << 16);
        }
        /// <summary>
        /// Cookie有效路径数据长度
        /// </summary>
        private const int cookiePathSize = sizeof(int) + 3;
        /// <summary>
        /// Cookie域名
        /// </summary>
        /// <param name="buffer"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe static void writeCookieExpires(byte* buffer)
        {//; Expires=
            *(int*)buffer = ';' + (' ' << 8) + ('E' << 16) + ('x' << 24);
            *(int*)(buffer + sizeof(int)) = 'p' + ('i' << 8) + ('r' << 16) + ('e' << 24);
            *(short*)(buffer + sizeof(int) * 2) = 's' + ('=' << 8);
        }
        /// <summary>
        /// Cookie域名数据长度
        /// </summary>
        private const int cookieExpiresSize = sizeof(int) * 2 + sizeof(short);
        /// <summary>
        /// Cookie最小时间超时时间
        /// </summary>
        /// <param name="buffer"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe static void writeMinTimeCookieExpires(byte* buffer)
        {//Mon, 01 Jan 1900 00:00:00 GMT
            *(int*)buffer = 'M' + ('o' << 8) + ('n' << 16) + (',' << 24);
            *(int*)(buffer + sizeof(int)) = ' ' + ('0' << 8) + ('1' << 16) + (' ' << 24);
            *(int*)(buffer + sizeof(int) * 2) = 'J' + ('a' << 8) + ('n' << 16) + (' ' << 24);
            *(int*)(buffer + sizeof(int) * 3) = '1' + ('9' << 8) + ('0' << 16) + ('0' << 24);
            *(int*)(buffer + sizeof(int) * 4) = ' ' + ('0' << 8) + ('0' << 16) + (':' << 24);
            *(int*)(buffer + sizeof(int) * 5) = '0' + ('0' << 8) + (':' << 16) + ('0' << 24);
            *(int*)(buffer + sizeof(int) * 6) = '0' + (' ' << 8) + ('G' << 16) + ('M' << 24);
            *(buffer + sizeof(int) * 7) = (byte)'T';
        }
        /// <summary>
        /// Cookie安全
        /// </summary>
        /// <param name="buffer"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe static void writeCookieSecure(byte* buffer)
        {//; Secure
            *(int*)buffer = ';' + (' ' << 8) + ('S' << 16) + ('e' << 24);
            *(int*)(buffer + sizeof(int)) = 'c' + ('u' << 8) + ('r' << 16) + ('e' << 24);
        }
        /// <summary>
        /// Cookie安全数据长度
        /// </summary>
        private const int cookieSecureSize = sizeof(int) * 2;
        /// <summary>
        /// Cookie是否http only
        /// </summary>
        /// <param name="buffer"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe static void writeCookieHttpOnly(byte* buffer)
        {//; HttpOnly
            *(int*)buffer = ';' + (' ' << 8) + ('H' << 16) + ('t' << 24);
            *(int*)(buffer + sizeof(int)) = 't' + ('p' << 8) + ('O' << 16) + ('n' << 24);
            *(short*)(buffer + sizeof(int) * 2) = 'l' + ('y' << 8);
        }
        /// <summary>
        /// Cookie是否http only数据长度
        /// </summary>
        private const int cookieHttpOnlySize = sizeof(int) * 2 + sizeof(short);
        /// <summary>
        /// 最后一次生成的时间
        /// </summary>
        private static long dateCacheSecond;
        /// <summary>
        /// 最后一次生成的时间字节数组
        /// </summary>
        private static pointer dateCache;
        /// <summary>
        /// 时间字节数组访问锁
        /// </summary>
        private static readonly object dateCacheLock = new object();
        /// <summary>
        /// 获取当前时间字节数组
        /// </summary>
        /// <param name="data">输出数据起始位置</param>
        private static unsafe void getDate(byte* data)
        {
            DateTime now = date.nowTime.Now;
            if (Monitor.TryEnter(dateCacheLock))
            {
                try
                {
                    byte* cachePoint = dateCache.Byte;
                    long second = now.Ticks / 10000000;
                    if (dateCacheSecond != second)
                    {
                        dateCacheSecond = second;
                        date.ToBytes(now, cachePoint);
                    }
                    *(ulong*)data = *(ulong*)cachePoint;
                    *(ulong*)(data + sizeof(ulong)) = *(ulong*)(cachePoint + sizeof(ulong));
                    *(ulong*)(data + sizeof(ulong) * 2) = *(ulong*)(cachePoint + sizeof(ulong) * 2);
                    *(ulong*)(data + sizeof(ulong) * 3) = *(ulong*)(cachePoint + sizeof(ulong) * 3);
                }
                finally { Monitor.Exit(dateCacheLock); }
            }
            else date.ToBytes(now, data);
        }
        ///// <summary>
        ///// 计时器
        ///// </summary>
        //protected System.Diagnostics.Stopwatch startTime = new System.Diagnostics.Stopwatch();
        ///// <summary>
        ///// 当前使用时间
        ///// </summary>
        //public TimeSpan CurrentTime
        //{
        //    get { return startTime.Elapsed; }
        //}
        /// <summary>
        /// 当前输出HTTP响应
        /// </summary>
        protected response response;
        /// <summary>
        /// 当前输出HTTP响应字节数
        /// </summary>
        protected long responseSize;
        /// <summary>
        /// HTTP内容数据缓冲区
        /// </summary>
        internal byte[] Buffer;
        /// <summary>
        /// 获取HTTP请求头部
        /// </summary>
        public requestHeader RequestHeader { get; protected set; }
        /// <summary>
        /// HTTP请求表单
        /// </summary>
        protected requestForm form;
        /// <summary>
        /// HTTP服务
        /// </summary>
        internal server server;
        /// <summary>
        /// HTTP服务器
        /// </summary>
        protected servers servers;
        /// <summary>
        /// 套接字
        /// </summary>
        internal Socket Socket;
        /// <summary>
        /// 超时标识
        /// </summary>
        protected int timeoutIdentity;
        /// <summary>
        /// TCP调用套接字
        /// </summary>
        public commandServer.socket TcpCommandSocket { get; protected set; }
        /// <summary>
        /// 远程终结点
        /// </summary>
        internal EndPoint RemoteEndPoint
        {
            get { return Socket.RemoteEndPoint; }
        }
        /// <summary>
        /// 客户端IP地址
        /// </summary>
        internal int Ipv4;
        /// <summary>
        /// 客户端IP地址
        /// </summary>
        internal ipv6Hash Ipv6;
        /// <summary>
        /// 域名服务
        /// </summary>
        internal domainServer DomainServer;
        /// <summary>
        /// 获取Session
        /// </summary>
        internal ISession Session { get { return DomainServer.Session; } }
        /// <summary>
        /// 操作标识
        /// </summary>
        protected long identity;
        /// <summary>
        /// 是否加载表单
        /// </summary>
        protected byte isLoadForm;
        /// <summary>
        /// 是否正在处理下一个请求
        /// </summary>
        protected byte isNextRequest;
        /// <summary>
        /// 是否超时
        /// </summary>
        protected byte isTimeout;
        /// <summary>
        /// 是否SSL链接
        /// </summary>
        internal virtual bool IsSsl { get { return false; } }
        /// <summary>
        /// 是否已经释放资源
        /// </summary>
        protected int isDisposed;
        /// <summary>
        /// HTTP套接字
        /// </summary>
        protected socketBase()
        {
            Buffer = new byte[HeaderNameStartIndex];
            form = new requestForm();
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose()
        {
            form.Clear();
            response.Push(ref response);
        }
        /// <summary>
        /// HTTP头部接收错误
        /// </summary>
        protected abstract void headerError();
        /// <summary>
        /// WebSocket结束
        /// </summary>
        protected abstract void webSocketEnd();
        /// <summary>
        /// 处理下一个请求
        /// </summary>
        private void next()
        {
            if (RequestHeader.IsKeepAlive)
            {
                response.Push(ref response);
                isNextRequest = 1;
                isLoadForm = 0;
                form.Clear();
                receiveHeader();
                return;
            }
            end();
        }
        /// <summary>
        /// 请求结束处理
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void end()
        {
            response.Push(ref response);
            headerError();
        }
        /// <summary>
        /// 开始接收头部数据
        /// </summary>
        protected abstract void receiveHeader();
        /// <summary>
        /// 未能识别的HTTP头部
        /// </summary>
        protected abstract void headerUnknown();
        /// <summary>
        /// 获取域名服务信息
        /// </summary>
        private void request()
        {
            long identity = this.identity;
            try
            {
                if (RequestHeader.IsWebSocket) DomainServer.WebSocketRequest(this, identity);
                else DomainServer.Request(this, identity);
                return;
            }
            catch (Exception error)
            {
                log.Error.Add(error, null, false);
            }
            ResponseError(identity, response.state.ServerError500);
        }
        /// <summary>
        /// 检测100 Continue确认
        /// </summary>
        protected bool check100Continue()
        {
            if (RequestHeader.Is100Continue)
            {
                RequestHeader.Is100Continue = false;
                try
                {
                    SocketError error = SocketError.Success;
                    if (Socket.send(continue100, 0, continue100.Length, ref error)) return true;
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                return false;
            }
            return true;
        }
        /// <summary>
        /// 获取请求表单数据
        /// </summary>
        /// <param name="identity">HTTP操作标识</param>
        /// <param name="loadForm">HTTP请求表单加载接口</param>
        internal abstract void GetForm(long identity, requestForm.ILoadForm loadForm);
        /// <summary>
        /// FORM表单解析
        /// </summary>
        /// <param name="dataToType"></param>
        /// <returns></returns>
        internal abstract bool ParseForm(requestHeader.postType dataToType = requestHeader.postType.Data);
        /// <summary>
        /// 输出HTTP响应数据
        /// </summary>
        /// <param name="identity">HTTP操作标识</param>
        /// <param name="response">HTTP响应数据</param>
        public abstract bool Response(long identity, ref response response);
        /// <summary>
        /// 输出HTTP响应数据
        /// </summary>
        /// <param name="identity">HTTP操作标识</param>
        /// <param name="response">HTTP响应数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal unsafe bool Response(long identity, response response)
        {
            return Response(identity, ref response);
        }
        /// <summary>
        /// 输出错误状态
        /// </summary>
        /// <param name="identity">操作标识</param>
        /// <param name="state">错误状态</param>
        public bool ResponseError(long identity, response.state state)
        {
            if (Interlocked.CompareExchange(ref this.identity, identity + 1, identity) == identity)
            {
                responseError(state);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 搜索引擎404提示
        /// </summary>
        /// <param name="identity">操作标识</param>
        /// <returns></returns>
        internal bool ResponseSearchEngine404(long identity)
        {
            if (Interlocked.CompareExchange(ref this.identity, identity + 1, identity) == identity)
            {
                responseSearchEngine404();
                return true;
            }
            return false;
        }
        /// <summary>
        /// 输出错误状态
        /// </summary>
        /// <param name="state">错误状态</param>
        protected abstract void responseError(response.state state);
        /// <summary>
        /// 搜索引擎404提示
        /// </summary>
        protected abstract void responseSearchEngine404();
        /// <summary>
        /// 表单回调处理
        /// </summary>
        /// <param name="form">HTTP请求表单</param>
        public void OnGetForm(requestForm form)
        {
            if (form == null)
            {
                response.Push(ref response);
                headerError();
            }
            else responseHeader();
        }
        /// <summary>
        /// 根据HTTP请求表单值获取内存流最大字节数
        /// </summary>
        /// <param name="value">HTTP请求表单值</param>
        /// <returns>内存流最大字节数</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public int MaxMemoryStreamSize(ref fastCSharp.net.tcp.http.requestForm.value value) { return 0; }
        /// <summary>
        /// 根据HTTP请求表单值获取保存文件全称
        /// </summary>
        /// <param name="value">HTTP请求表单值</param>
        /// <returns>文件全称</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public string GetSaveFileName(ref fastCSharp.net.tcp.http.requestForm.value value) { return null; }
        /// <summary>
        /// HTTP响应头部输出
        /// </summary>
        protected unsafe void responseHeader()
        {
            try
            {
                requestHeader requestHeader = RequestHeader;
                if (response.Body.length != 0)
                {
                    response.BodyFile = null;
                    if (requestHeader.IsKeepAlive && response.CanHeader && !requestHeader.IsRange)
                    {
                        subArray<byte> body = response.HeaderBody;
                        if (body.length != 0)
                        {
                            responseSize = 0;
                            responseSecondCount.Add();
                            responseHeader(ref body, null);
                            return;
                        }
                    }
                }
                long bodySize = response.BodySize;
                if (bodySize != 0) responseSecondCount.Add();
                char* responseSizeFixed = stackalloc char[24 * 4], bodySizeFixed = responseSizeFixed + 24;
                char* rangeStartFixed = responseSizeFixed + 24 * 2, rangeEndFixed = responseSizeFixed + 24 * 3;
                keyValue<int, int> responseSizeIndex = new keyValue<int, int>(), bodySizeIndex = new keyValue<int, int>(), rangeStartIndex = new keyValue<int, int>(), rangeEndIndex = new keyValue<int, int>();
                //byte* responseSizeWrite = responseSizeFixed, bodySizeWrite = responseSizeFixed;
                //byte* rangeStartWrite = rangeStartFixed, rangeEndWrite = rangeEndFixed;
                if (requestHeader.IsRange && response.IsPool)
                {
                    if (requestHeader.IsFormatRange || requestHeader.FormatRange(bodySize))
                    {
                        if (response.State == response.state.Ok200)
                        {
                            responseSize = requestHeader.RangeLength;
                            long rangeStart = requestHeader.RangeStart, rangeEnd = requestHeader.RangeEnd;
                            if (rangeStart != 0) rangeStartIndex = number.toString((ulong)rangeStart, rangeStartFixed);
                            if (rangeEnd != bodySize - 1) rangeEndIndex = number.toString((ulong)rangeEnd, rangeEndFixed);
                            bodySizeIndex = number.toString((ulong)bodySize, bodySizeFixed);
                            response.State = response.state.PartialContent206;
                        }
                        else responseSize = bodySize;
                    }
                    else
                    {
                        response.State = http.response.state.RangeNotSatisfiable416;
                        responseSize = 0;
                    }
                }
                else responseSize = bodySize;
                responseSizeIndex = number.toString((ulong)responseSize, responseSizeFixed);
                response.stateInfo state = fastCSharp.Enum<response.state, response.stateInfo>.Array((byte)response.State);
                if (state == null) state = fastCSharp.Enum<response.state, response.stateInfo>.Array((byte)response.state.ServerError500);
                int index = httpVersionSize + state.Text.Length + contentLengthSize + responseSizeIndex.Value + 2 + 2;
                if (DomainServer.IsResponseServer) index += fastCSharpServer.Length;
                if (response.State == response.state.PartialContent206)
                {
                    index += rangeSize + rangeStartIndex.Value + rangeEndIndex.Value + bodySizeIndex.Value + 2 + 2;
                }
                if (response.Location.length != 0) index += locationSize + response.Location.length + 2;
                if (response.LastModified != null) index += lastModifiedSize + response.LastModified.Length + 2;
                if (DomainServer.IsResponseCacheControl && response.CacheControl != null) index += cacheControlSize + response.CacheControl.Length + 2;
                if (DomainServer.IsResponseContentType && response.ContentType != null) index += contentTypeSize + response.ContentType.Length + 2;
                if (response.ContentEncoding != null) index += contentEncodingSize + response.ContentEncoding.Length + 2;
                if (response.ETag != null) index += eTagSize + response.ETag.Length + 2 + 1;
                if (response.ContentDisposition != null) index += contentDispositionSize + response.ContentDisposition.Length + 2;
                if (response.AccessControlAllowOrigin.Length != 0) index += accessControlAllowOriginSize + response.AccessControlAllowOrigin.Length + 2;
                if (requestHeader.IsKeepAlive) index += keepAliveSize;
                if (DomainServer.IsResponseDate) index += dateSize + date.ToByteLength + 2;
                int cookieCount = response.Cookies.length;
                if (cookieCount != 0)
                {
                    index += (setCookieSize + 3) * cookieCount;
                    foreach (cookie cookie in response.Cookies.array)
                    {
                        index += cookie.Name.Length + cookie.Value.length();
                        if (cookie.Domain.length != 0) index += cookieDomainSize + cookie.Domain.length;
                        if (cookie.Path != null) index += cookiePathSize + cookie.Path.Length;
                        if (cookie.Expires != DateTime.MinValue) index += cookieExpiresSize + date.ToByteLength;
                        if (cookie.IsSecure) index += cookieSecureSize;
                        if (cookie.IsHttpOnly) index += cookieHttpOnlySize;
                        if (--cookieCount == 0) break;
                    }
                }
                int checkIndex = index;
                byte[] buffer;
                memoryPool memoryPool;
                if (responseSize <= 1500 - 20) index += (int)responseSize;
                else if (responseSize < fastCSharp.config.appSetting.StreamBufferSize)
                {
                    index += (int)responseSize;
                    if (index + 3 > fastCSharp.config.appSetting.StreamBufferSize) index -= (int)responseSize;
                }
                if ((index += 3) <= HeaderNameStartIndex)
                {
                    buffer = this.Buffer;
                    memoryPool = null;
                }
                else
                {
                    memoryPool = getMemoryPool(index);
                    buffer = memoryPool.Get(index);
                }
                fixed (byte* bufferFixed = buffer)
                {
                    byte* write = bufferFixed + httpVersionSize;
                    writeHttpVersion(bufferFixed);
                    state.Write(write);
                    writeContentLength(write += state.Text.Length);
                    fastCSharp.unsafer.String.WriteBytes(responseSizeFixed + responseSizeIndex.Key, responseSizeIndex.Value, write += contentLengthSize);
                    *(short*)(write += responseSizeIndex.Value) = 0x0a0d;
                    write += sizeof(short);
                    if (response.State == response.state.PartialContent206)
                    {
                        writeRange(write);
                        fastCSharp.unsafer.String.WriteBytes(rangeStartFixed + rangeStartIndex.Key, rangeStartIndex.Value, write += rangeSize);
                        *(write += rangeStartIndex.Value) = (byte)'-';
                        fastCSharp.unsafer.String.WriteBytes(rangeEndFixed + rangeEndIndex.Key, rangeEndIndex.Value, ++write);
                        *(write += rangeEndIndex.Value) = (byte)'/';
                        fastCSharp.unsafer.String.WriteBytes(bodySizeFixed + bodySizeIndex.Key, bodySizeIndex.Value, ++write);
                        *(short*)(write += bodySizeIndex.Value) = 0x0a0d;
                        write += sizeof(short);
                    }
                    if ((index = response.Location.length) != 0)
                    {
                        writeLocation(write);
                        fixed (byte* locationFixed = response.Location.array) fastCSharp.unsafer.memory.UnsafeSimpleCopy(locationFixed + response.Location.startIndex, write += locationSize, index);
                        *(short*)(write += index) = 0x0a0d;
                        write += sizeof(short);
                    }
                    if (DomainServer.IsResponseCacheControl && response.CacheControl != null)
                    {
                        writeCacheControl(write);
                        write += cacheControlSize;
                        if ((index = response.CacheControl.Length) != 0)
                        {
                            fastCSharp.unsafer.memory.UnsafeSimpleCopy(response.CacheControl, write, index);
                            write += index;
                        }
                        *(short*)write = 0x0a0d;
                        write += sizeof(short);
                    }
                    if (DomainServer.IsResponseContentType && response.ContentType != null)
                    {
                        writeContentType(write);
                        write += contentTypeSize;
                        if ((index = response.ContentType.Length) != 0)
                        {
                            fastCSharp.unsafer.memory.UnsafeSimpleCopy(response.ContentType, write, index);
                            write += index;
                        }
                        *(short*)write = 0x0a0d;
                        write += sizeof(short);
                    }
                    if (response.ContentEncoding != null)
                    {
                        writeContentEncoding(write);
                        write += contentEncodingSize;
                        if ((index = response.ContentEncoding.Length) != 0)
                        {
                            fastCSharp.unsafer.memory.UnsafeSimpleCopy(response.ContentEncoding, write, index);
                            write += index;
                        }
                        *(short*)write = 0x0a0d;
                        write += sizeof(short);
                    }
                    if (response.ETag != null)
                    {
                        writeETag(write);
                        write += eTagSize;
                        if ((index = response.ETag.Length) != 0)
                        {
                            fastCSharp.unsafer.memory.UnsafeSimpleCopy(response.ETag, write, index);
                            write += index;
                        }
                        *(int*)write = '"' + 0x0a0d00;
                        write += 3;
                    }
                    if (response.ContentDisposition != null)
                    {
                        writeContentDisposition(write);
                        write += contentDispositionSize;
                        if ((index = response.ContentDisposition.Length) != 0)
                        {
                            fastCSharp.unsafer.memory.UnsafeSimpleCopy(response.ContentDisposition, write, index);
                            write += index;
                        }
                        *(short*)write = 0x0a0d;
                        write += sizeof(short);
                    }
                    if ((index = response.AccessControlAllowOrigin.Length) != 0)
                    {
                        writeAccessControlAllowOrigin(write);
                        fixed (byte* requestBufferFixed = requestHeader.Buffer) fastCSharp.unsafer.memory.UnsafeSimpleCopy(requestBufferFixed + response.AccessControlAllowOrigin.StartIndex, write += accessControlAllowOriginSize, index);
                        *(short*)(write += index) = 0x0a0d;
                        write += sizeof(short);
                    }
                    if ((cookieCount = response.Cookies.length) != 0)
                    {
                        foreach (cookie cookie in response.Cookies.array)
                        {
                            writeSetCookie(write);
                            write += setCookieSize;
                            if ((index = cookie.Name.Length) != 0)
                            {
                                fastCSharp.unsafer.memory.UnsafeSimpleCopy(cookie.Name, write, index);
                                write += index;
                            }
                            *write++ = (byte)'=';
                            if ((index = cookie.Value.length()) != 0)
                            {
                                fastCSharp.unsafer.memory.UnsafeSimpleCopy(cookie.Value, write, index);
                                write += index;
                            }
                            if ((index = cookie.Domain.length) != 0)
                            {
                                writeCookieDomain(write);
                                fixed (byte* domainFixed = cookie.Domain.array) fastCSharp.unsafer.memory.UnsafeSimpleCopy(domainFixed + cookie.Domain.startIndex, write += cookieDomainSize, index);
                                write += index;
                            }
                            if (cookie.Path != null)
                            {
                                writeCookiePath(write);
                                write += cookiePathSize;
                                if ((index = cookie.Path.Length) != 0)
                                {
                                    fastCSharp.unsafer.memory.UnsafeSimpleCopy(cookie.Path, write, index);
                                    write += index;
                                }
                            }
                            if (cookie.Expires != DateTime.MinValue)
                            {
                                writeCookieExpires(write);
                                write += cookieExpiresSize;
                                if (cookie.Expires == pub.MinTime) writeMinTimeCookieExpires(write);
                                else date.ToBytes(cookie.Expires, write);
                                write += date.ToByteLength;
                            }
                            if (cookie.IsSecure)
                            {
                                writeCookieSecure(write);
                                write += cookieSecureSize;
                            }
                            if (cookie.IsHttpOnly)
                            {
                                writeCookieHttpOnly(write);
                                write += cookieHttpOnlySize;
                            }
                            *(short*)write = 0x0a0d;
                            write += sizeof(short);
                            if (--cookieCount == 0) break;
                        }
                    }
                    if (response.LastModified != null)
                    {
                        writeLastModified(write);
                        write += lastModifiedSize;
                        if ((index = response.LastModified.Length) != 0)
                        {
                            fastCSharp.unsafer.memory.UnsafeSimpleCopy(response.LastModified, write, index);
                            write += index;
                        }
                        *(short*)write = 0x0a0d;
                        write += sizeof(short);
                    }
                    if (requestHeader.IsKeepAlive)
                    {
                        writeKeepAlive(write);
                        write += keepAliveSize;
                    }
                    if (DomainServer.IsResponseServer)
                    {
                        fastCSharp.unsafer.memory.UnsafeSimpleCopy(responseServer.Byte, write, fastCSharpServer.Length);
                        write += fastCSharpServer.Length;
                    }
                    if (DomainServer.IsResponseDate)
                    {
                        writeDate(write);
                        getDate(write += dateSize);
                        *(int*)(write += date.ToByteLength) = 0x0a0d0a0d;
                        write += sizeof(int);
                    }
                    else
                    {
                        *(short*)(write) = 0x0a0d;
                        write += sizeof(short);
                    }
                    index = (int)(write - bufferFixed);
                    if (checkIndex != index)
                    {
                        log.Error.Add("responseHeader checkIndex[" + checkIndex.toString() + "] != index[" + index.toString() + @"]
" + System.Text.Encoding.ASCII.GetString(buffer, 0, index), null, false);
                    }
                    if (response.Body.length != 0)
                    {
                        if (requestHeader.IsKeepAlive && response.CanHeader && (index + sizeof(int)) <= response.Body.startIndex && !requestHeader.IsRange)
                        {
                            fixed (byte* bodyFixed = response.Body.array)
                            {
                                unsafer.memory.Copy(bufferFixed, bodyFixed + response.Body.startIndex - index, index);
                                *(int*)bodyFixed = index;
                            }
                            subArray<byte> headerBody = response.HeaderBody;
                            responseSize = 0;
                            responseHeader(ref headerBody, null);
                            return;
                        }
                        if (buffer.Length - index >= (int)responseSize)
                        {
                            //if (response.Body.Length != responseSize) log.Default.Add("response.Body.Length[" + response.Body.Length.toString() + "] != responseSize[" + responseSize.toString() + "]", true, false);
                            System.Buffer.BlockCopy(response.Body.array, response.State == response.state.PartialContent206 ? response.Body.startIndex + (int)requestHeader.RangeStart : response.Body.startIndex, buffer, index, (int)responseSize);
                            index += (int)responseSize;
                            responseSize = 0;
                        }
                        ////showjim
                        //else if (response.Body.Count > response.Body.array.Length)
                        //{
                        //    fixed (byte* headerBufferFixed = requestHeader.Buffer)
                        //    {
                        //        log.Error.Add(fastCSharp.String.DeSerialize(headerBufferFixed + requestHeader.Uri.StartIndex, -2 - requestHeader.Uri.Count) + fastCSharp.String.DeSerialize(bufferFixed, -index) + response.Body.Count.toString() + " > " + response.Body.array.Length.toString(), true, false);
                        //    }
                        //}
                    }
                }
                subArray<byte> header = subArray<byte>.Unsafe(buffer, 0, index);
                responseHeader(ref header, memoryPool);
                return;
            }
            catch (Exception error)
            {
                log.Error.Add(error, null, false);
                fixed (byte* headerBufferFixed = RequestHeader.Buffer)
                {
                    log.Error.Add(@"responseSize[" + responseSize.toString() + "] body[" + response.Body.array.Length.toString() + "," + response.Body.startIndex.toString() + "," + response.Body.length.toString() + @"]
" + fastCSharp.String.UnsafeDeSerialize(headerBufferFixed, -RequestHeader.EndIndex), null, false);
                }
            }
            headerError();
        }
        /// <summary>
        /// HTTP响应头部输出
        /// </summary>
        /// <param name="buffer">输出数据</param>
        /// <param name="memoryPool">内存池</param>
        protected abstract void responseHeader(ref subArray<byte> buffer, memoryPool memoryPool);
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        protected abstract bool send(byte[] buffer, int index, int length);
        ///// <summary>
        ///// 获取AJAX回调函数
        ///// </summary>
        ///// <param name="identity">操作标识</param>
        ///// <returns>AJAX回调函数,失败返回null</returns>
        //internal abstract subString GetWebSocketCallBack(long identity);
        /// <summary>
        /// WebSocket响应协议输出
        /// </summary>
        /// <param name="identity">HTTP操作标识</param>
        /// <param name="webSocket"></param>
        /// <returns></returns>
        internal unsafe bool ResponseWebSocket101(ref long identity, webSocket.socket webSocket)
        {
            if (Interlocked.CompareExchange(ref this.identity, identity + 1, identity) == identity)
            {
                int index = webSocket101.Length;
                System.Buffer.BlockCopy(webSocket101, 0, Buffer, 0, index);
                subArray<byte> secWebSocketKey = RequestHeader.SecWebSocketKey;
                System.Buffer.BlockCopy(secWebSocketKey.array, secWebSocketKey.startIndex, Buffer, index, secWebSocketKey.length);
                System.Buffer.BlockCopy(webSocketKey, 0, Buffer, index + secWebSocketKey.length, webSocketKey.Length);
                byte[] acceptKey = pub.Sha1(Buffer, index, secWebSocketKey.length + webSocketKey.Length);
                fixed (byte* bufferFixed = Buffer, acceptKeyFixed = acceptKey)
                {
                    byte* write = bufferFixed + webSocket101.Length, keyEnd = acceptKeyFixed + 18, base64 = String.Base64.Byte;
                    for (byte* read = acceptKeyFixed; read != keyEnd; read += 3)
                    {
                        *write++ = *(base64 + (*read >> 2));
                        *write++ = *(base64 + (((*read << 4) | (*(read + 1) >> 4)) & 0x3f));
                        *write++ = *(base64 + (((*(read + 1) << 2) | (*(read + 2) >> 6)) & 0x3f));
                        *write++ = *(base64 + (*(read + 2) & 0x3f));
                    }
                    *write++ = *(base64 + (*keyEnd >> 2));
                    *write++ = *(base64 + (((*keyEnd << 4) | (*(keyEnd + 1) >> 4)) & 0x3f));
                    *write++ = *(base64 + ((*(keyEnd + 1) << 2) & 0x3f));
                    *write++ = (byte)'=';
                    *(int*)write = 0x0a0d0a0d;
                }
                if (send(Buffer, 0, index + 32))
                {
                    webSocket.SetSocketIdentity(++identity);
                    receiveWebSocket(webSocket);
                    return true;
                }
                ResponseError(identity, response.state.ServerError500);
            }
            return false;
        }
        /// <summary>
        /// 开始接收WebSocket数据
        /// </summary>
        /// <param name="webSocket"></param>
        protected abstract void receiveWebSocket(webSocket.socket webSocket);
        /// <summary>
        /// WebSocket发送消息
        /// </summary>
        /// <param name="webSocket"></param>
        internal abstract void WebSocketSend(webSocket.socket webSocket);
        /// <summary>
        /// 发送HTTP响应内容
        /// </summary>
        /// <param name="isSend">是否发送成功</param>
        protected abstract void responseBody(bool isSend);
        /// <summary>
        /// 设置超时
        /// </summary>
        /// <param name="identity">超时标识</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void setTimeout(int identity)
        {
            if (ReceiveTimeoutQueue != null && identity == timeoutIdentity) ReceiveTimeoutQueue.Add(this, Socket, identity, timeoutQueue.callbackType.Http);
        }
        /// <summary>
        /// 设置超时
        /// </summary>
        /// <param name="identity">超时标识</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void setKeepAliveTimeout(int identity)
        {
            if (KeepAliveReceiveTimeoutQueue != null && identity == timeoutIdentity) KeepAliveReceiveTimeoutQueue.Add(this, Socket, identity, timeoutQueue.callbackType.Http);
        }
        /// <summary>
        /// 设置超时
        /// </summary>
        /// <param name="identity">超时标识</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected void setWebSocketTimeout(int identity)
        {
            if (WebSocketReceiveTimeoutQueue != null && identity == timeoutIdentity) WebSocketReceiveTimeoutQueue.Add(this, Socket, identity, timeoutQueue.callbackType.Http);
        }
        /// <summary>
        /// 超时检测
        /// </summary>
        /// <param name="identity">超时标识</param>
        /// <returns>是否超时</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal bool CheckTimeout(int identity)
        {
            if (identity == timeoutIdentity)
            {
                isTimeout = 1;
                //if (IsSsl) Interlocked.Increment(ref sslTimeoutCount);
                //else Interlocked.Increment(ref timeoutCount);
                return true;
            }
            return false;
        }
        unsafe static socketBase()
        {
            int dateCacheSize = (date.ToByteLength + 7) & (int.MaxValue - 7), fastCSharpServerSize = (fastCSharpServer.Length + 7) & (int.MaxValue - 7);
            dateCache = unmanaged.GetStatic(dateCacheSize + fastCSharpServerSize, false);
            responseServer = new pointer { Data = dateCache.Byte + dateCacheSize };
            fixed (char* fastCSharpServerFixed = fastCSharpServer) fastCSharp.unsafer.String.WriteBytes(fastCSharpServerFixed, fastCSharpServer.Length, responseServer.Byte);

            errorResponseDatas = new byte[Enum.GetMaxValue<response.state>(-1) + 1][];
            foreach (response.state type in System.Enum.GetValues(typeof(response.state)))
            {
                response.stateInfo state = fastCSharp.Enum<response.state, response.stateInfo>.Array((int)type);
                if (state != null && state.IsError)
                {
                    string stateData = state.Text;
                    byte[] responseData = new byte[httpVersionSize + stateData.Length + responseServerEnd.Length];
                    fixed (byte* responseDataFixed = responseData)
                    {
                        writeHttpVersion(responseDataFixed);
                        state.Write(responseDataFixed + httpVersionSize);
                    }
                    int index = httpVersionSize + stateData.Length;
                    System.Buffer.BlockCopy(responseServerEnd, 0, responseData, index, responseServerEnd.Length);
                    errorResponseDatas[(int)type] = responseData;
                    if (type == response.state.NotFound404)
                    {
                        byte[] html = Encoding.UTF8.GetBytes("<html><head>" + web.html.GetHtml(web.html.charsetType.Utf8) + "<title>404 Error, 请将链接中的 #! 或者 # 修改为 ?</title></head><body>404 Error, 请将链接中的 #! 或者 # 修改为 ?</body>" + web.html.JsStart + @"document.body.innerHTML=document.title='404 Error';" + web.html.JsEnd + "</html>");
                        byte[] contentLength = Encoding.UTF8.GetBytes(fastCSharpServer + "Content-Length: " + html.Length.toString() + @"

");
                        searchEngine404Data = new byte[httpVersionSize + stateData.Length + contentLength.Length + html.Length];
                        fixed (byte* responseDataFixed = searchEngine404Data)
                        {
                            writeHttpVersion(responseDataFixed);
                            state.Write(responseDataFixed + httpVersionSize);
                        }
                        System.Buffer.BlockCopy(contentLength, 0, searchEngine404Data, index = httpVersionSize + stateData.Length, contentLength.Length);
                        System.Buffer.BlockCopy(html, 0, searchEngine404Data, index += contentLength.Length, html.Length);
                    }
                }
            }
        }
    }
    /// <summary>
    /// HTTP套接字
    /// </summary>
    internal sealed class socket : socketBase
    {
        /// <summary>
        /// HTTP头部接收器
        /// </summary>
        internal new sealed class headerReceiver : headerReceiver<socket>, IDisposable
        {
            /// <summary>
            /// 异步套接字操作
            /// </summary>
            public SocketAsyncEventArgs Async;
            /// <summary>
            /// HTTP头部接收器
            /// </summary>
            /// <param name="socket">HTTP套接字</param>
            public headerReceiver(socket socket)
                : base(socket)
            {
                Async = socketAsyncEventArgs.Get();
                Async.SocketFlags = System.Net.Sockets.SocketFlags.None;
                Async.DisconnectReuseSocket = false;
                Async.Completed += receive;
                Async.UserToken = this;
                Async.SetBuffer(buffer, 0, buffer.Length);
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                Async.Completed -= receive;
                socketAsyncEventArgs.Push(ref Async);
            }
            /// <summary>
            /// 开始接收数据
            /// </summary>
            public override void Receive()
            {
                Async.SocketError = SocketError.Success;
                base.Receive();
            }
            /// <summary>
            /// 开始接收数据(用于TCP调用)
            /// </summary>
            /// <param name="data">已接受数据</param>
            public unsafe void Receive(byte[] data)
            {
                timeout = date.nowTime.Now.AddTicks(receiveTimeoutQueueCallbackTimeoutTicks);
                HeaderEndIndex = 0;
                fixed (byte* dataFixed = data, bufferFixed = buffer) *(int*)bufferFixed = *(int*)dataFixed;
                ReceiveEndIndex = sizeof(int);

                Async.SocketError = SocketError.Success;
                receive();
            }
            /// <summary>
            /// 接受头部换行数据
            /// </summary>
            protected override void receive()
            {
                try
                {
                    int timeoutIdentity = socket.timeoutIdentity;
                    Async.SetBuffer(ReceiveEndIndex, HeaderBufferLength - ReceiveEndIndex);
                    if (socket.Socket.ReceiveAsync(Async))
                    {
                        if (socket.isNextRequest == 0 || ReceiveEndIndex != 0) socket.setTimeout(timeoutIdentity);
                        else socket.setKeepAliveTimeout(timeoutIdentity);
                        return;
                    }
                    if (Async.SocketError == SocketError.Success)
                    {
                        int count = Async.BytesTransferred;
                        if (count > 0)
                        {
                            ReceiveEndIndex += count;
                            if (isSecondCount == 0)
                            {
                                isSecondCount = 1;
                                secondCount.Add();
                            }
                            onReceive();
                            return;
                        }
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                socket.headerError();
            }
            /// <summary>
            /// 接受头部换行数据
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="async">异步回调参数</param>
            private unsafe void receive(object sender, SocketAsyncEventArgs async)
            {
                ++socket.timeoutIdentity;
                int count = 0;
                try
                {
                    if (async.SocketError == SocketError.Success && (count = async.BytesTransferred) > 0)
                    {
                        ReceiveEndIndex += count;
                        if (isSecondCount == 0)
                        {
                            isSecondCount = 1;
                            secondCount.Add();
                        }
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                if (count <= 0 || date.nowTime.Now >= timeout) socket.headerError();
                else onReceive();
            }
        }
        /// <summary>
        /// 表单数据接收器
        /// </summary>
        private new sealed class formIdentityReceiver : formIdentityReceiver<socket>, IDisposable
        {
            /// <summary>
            /// 异步套接字操作
            /// </summary>
            public SocketAsyncEventArgs Async;
            /// <summary>
            /// HTTP表单接收器
            /// </summary>
            /// <param name="socket">HTTP套接字</param>
            public formIdentityReceiver(socket socket)
                : base(socket)
            {
                Async = socketAsyncEventArgs.Get();
                Async.SocketFlags = System.Net.Sockets.SocketFlags.None;
                Async.DisconnectReuseSocket = false;
                Async.Completed += receive;
                Async.UserToken = this;
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                Async.Completed -= receive;
                socketAsyncEventArgs.Push(ref Async);
            }
            /// <summary>
            /// 开始接收表单数据
            /// </summary>
            /// <param name="loadForm">HTTP请求表单加载接口</param>
            public void Receive(requestForm.ILoadForm loadForm)
            {
                headerReceiver headerReceiver = socket.HeaderReceiver;
                requestHeader requestHeader = headerReceiver.RequestHeader;
                this.loadForm = loadForm;
                contentLength = requestHeader.ContentLength;
                if (contentLength < socket.Buffer.Length)
                {
                    buffer = socket.Buffer;
                    memoryPool = null;
                }
                else
                {
                    memoryPool = getMemoryPool(contentLength + 1);
                    buffer = memoryPool.Get(contentLength + 1);
                }
                receiveEndIndex = headerReceiver.ReceiveEndIndex - headerReceiver.HeaderEndIndex - sizeof(int);
                System.Buffer.BlockCopy(requestHeader.Buffer, headerReceiver.HeaderEndIndex + sizeof(int), buffer, 0, receiveEndIndex);
                headerReceiver.ReceiveEndIndex = headerReceiver.HeaderEndIndex;

                if (receiveEndIndex == contentLength) callback();
                else
                {
                    receiveStartTime = date.nowTime.Now.AddTicks(date.SecondTicks);
                    Async.SocketError = SocketError.Success;
                    Async.SetBuffer(buffer, 0, buffer.Length);
                    receive();
                }
            }
            /// <summary>
            /// 开始接收表单数据
            /// </summary>
            private void receive()
            {
                try
                {
                    RECEIVE:
                    int timeoutIdentity = socket.timeoutIdentity;
                    Async.SetBuffer(receiveEndIndex, contentLength - receiveEndIndex);
                    if (socket.Socket.ReceiveAsync(Async))
                    {
                        socket.setTimeout(timeoutIdentity);
                        return;
                    }
                    if (Async.SocketError == SocketError.Success)
                    {
                        int count = Async.BytesTransferred;
                        if (count > 0)
                        {
                            if ((receiveEndIndex += count) == contentLength)
                            {
                                callback();
                                return;
                            }
                            goto RECEIVE;
                        }
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                receiveError();
            }
            /// <summary>
            /// 接收表单数据处理
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="async">异步回调参数</param>
            private unsafe void receive(object sender, SocketAsyncEventArgs async)
            {
                ++socket.timeoutIdentity;
                int count = 0;
                try
                {
                    if (async.SocketError == SocketError.Success && (count = async.BytesTransferred) > 0)
                    {
                        receiveEndIndex += count;
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                if (count <= 0) receiveError();
                else if (receiveEndIndex == contentLength) callback();
                else if (date.nowTime.Now > receiveStartTime && receiveEndIndex < minReceiveSizePerSecond4 * ((int)(date.nowTime.Now - receiveStartTime).TotalSeconds >> 2)) receiveError();
                else receive();
            }
        }
        /// <summary>
        /// 数据接收器
        /// </summary>
        private new sealed class boundaryIdentityReceiver : boundaryIdentityReceiver<socket>, IDisposable
        {
            /// <summary>
            /// 异步套接字操作
            /// </summary>
            public SocketAsyncEventArgs Async;
            /// <summary>
            /// HTTP数据接收器
            /// </summary>
            /// <param name="socket">HTTP套接字</param>
            public boundaryIdentityReceiver(socket socket)
                : base(socket)
            {
                Async = socketAsyncEventArgs.Get();
                Async.SocketFlags = System.Net.Sockets.SocketFlags.None;
                Async.DisconnectReuseSocket = false;
                Async.Completed += receive;
                Async.UserToken = this;
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                Async.Completed -= receive;
                socketAsyncEventArgs.Push(ref Async);
            }
            /// <summary>
            /// 开始接收表单数据
            /// </summary>
            /// <param name="loadForm">HTTP请求表单加载接口</param>
            public void Receive(requestForm.ILoadForm loadForm)
            {
                this.loadForm = loadForm;
                try
                {
                    headerReceiver headerReceiver = socket.HeaderReceiver;
                    requestHeader requestHeader = headerReceiver.RequestHeader;
                    Buffer = bigBuffers.Get();
                    boundary = requestHeader.Boundary;
                    receiveLength = receiveEndIndex = headerReceiver.ReceiveEndIndex - headerReceiver.HeaderEndIndex - sizeof(int);
                    System.Buffer.BlockCopy(requestHeader.Buffer, headerReceiver.HeaderEndIndex + sizeof(int), Buffer, 0, receiveEndIndex);
                    headerReceiver.ReceiveEndIndex = headerReceiver.HeaderEndIndex;
                    contentLength = requestHeader.ContentLength;

                    receiveStartTime = date.nowTime.Now.AddTicks(date.SecondTicks);
                    Async.SocketError = SocketError.Success;
                    Async.SetBuffer(Buffer, 0, Buffer.Length);
                    onFirstBoundary();
                    return;
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                this.error();
            }
            /// <summary>
            /// 开始接收表单数据
            /// </summary>
            protected override void receive()
            {
                try
                {
                    int timeoutIdentity = socket.timeoutIdentity;
                    Async.SetBuffer(receiveEndIndex, bigBuffers.Size - receiveEndIndex - sizeof(int));
                    if (socket.Socket.ReceiveAsync(Async))
                    {
                        socket.setTimeout(timeoutIdentity);
                        return;
                    }
                    if (Async.SocketError == SocketError.Success)
                    {
                        int count = Async.BytesTransferred;
                        if (count > 0 && (receiveLength += count) <= contentLength)
                        {
                            receiveEndIndex += count;
                            callOnReceiveData();
                            return;
                        }
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                this.error();
            }
            /// <summary>
            /// 接收表单数据处理
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="async">异步回调参数</param>
            private unsafe void receive(object sender, SocketAsyncEventArgs async)
            {
                ++socket.timeoutIdentity;
                int count = 0;
                try
                {
                    if (async.SocketError == SocketError.Success && (count = async.BytesTransferred) > 0)
                    {
                        receiveEndIndex += count;
                        receiveLength += count;
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                if (count <= 0 || receiveLength > contentLength
                    || (date.nowTime.Now > receiveStartTime && receiveLength < minReceiveSizePerSecond4 * ((int)(date.nowTime.Now - receiveStartTime).TotalSeconds >> 2)))
                {
                    error();
                }
                else callOnReceiveData();
            }
        }
        /// <summary>
        /// 数据发送器
        /// </summary>
        private new sealed class dataSender : dataSender<socket>, IDisposable
        {
            /// <summary>
            /// 异步套接字操作
            /// </summary>
            public SocketAsyncEventArgs Async;
            /// <summary>
            /// 发送文件异步套接字操作
            /// </summary>
            public SocketAsyncEventArgs FileAsync;
            /// <summary>
            /// 数据发送器
            /// </summary>
            /// <param name="socket">HTTP套接字</param>
            public dataSender(socket socket)
                : base(socket)
            {
                Async = socketAsyncEventArgs.Get();
                Async.SocketFlags = System.Net.Sockets.SocketFlags.None;
                Async.DisconnectReuseSocket = false;
                Async.Completed += send;
                Async.UserToken = this;

                FileAsync = socketAsyncEventArgs.Get();
                FileAsync.SocketFlags = System.Net.Sockets.SocketFlags.None;
                FileAsync.DisconnectReuseSocket = false;
                FileAsync.Completed += sendFile;
                FileAsync.UserToken = this;
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                Async.Completed -= send;
                socketAsyncEventArgs.Push(ref Async);
                FileAsync.Completed -= sendFile;
                socketAsyncEventArgs.Push(ref FileAsync);
            }
            /// <summary>
            /// 发送数据
            /// </summary>
            /// <param name="onSend">发送数据回调处理</param>
            /// <param name="buffer">发送数据缓冲区</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public unsafe void Send(onSendType onSend, subArray<byte> buffer)
            {
                Send(onSend, ref buffer, null);
            }
            /// <summary>
            /// 发送数据
            /// </summary>
            /// <param name="onSend">发送数据回调处理</param>
            /// <param name="buffer">发送数据缓冲区</param>
            /// <param name="memoryPool">发送数据缓冲区内存池</param>
            public unsafe void Send(onSendType onSend, ref subArray<byte> buffer, memoryPool memoryPool)
            {
                this.onSend = onSend;
                sendStartTime = date.nowTime.Now.AddTicks(date.SecondTicks);
                this.memoryPool = memoryPool;
                this.buffer = buffer.array;
                sendIndex = buffer.startIndex;
                sendLength = 0;
                sendEndIndex = sendIndex + buffer.length;
                //showjim
                if (sendEndIndex > this.buffer.Length || sendEndIndex <= sendIndex)
                {
                    requestHeader requestHeader = socket.RequestHeader;
                    fixed (byte* headerBufferFixed = requestHeader.Buffer)
                    {
                        log.Error.Add("buffer[" + this.buffer.Length.toString() + "] sendIndex[" + sendIndex.toString() + "] sendEndIndex[" + sendEndIndex.toString() + "] State[" + socket.response.State.ToString() + "] responseSize[" + socket.responseSize.toString() + "]" + fastCSharp.String.UnsafeDeSerialize(headerBufferFixed + requestHeader.Uri.startIndex, requestHeader.Uri.length + 2), null, false);
                    }
                }

                Async.SocketError = SocketError.Success;
                Async.SetBuffer(this.buffer, 0, this.buffer.Length);
                send();
            }
            /// <summary>
            /// 开始发送数据
            /// </summary>
            private void send()
            {
                try
                {
                    SEND:
                    int timeoutIdentity = socket.timeoutIdentity;
                    Async.SetBuffer(sendIndex, Math.Min(sendEndIndex - sendIndex, net.socket.MaxServerSendSize));
                    if (socket.Socket.SendAsync(Async))
                    {
                        socket.setTimeout(timeoutIdentity);
                        return;
                    }
                    if (Async.SocketError == SocketError.Success)
                    {
                        int count = Async.BytesTransferred;
                        if (count > 0)
                        {
                            sendIndex += count;
                            sendLength += count;
                            if (sendIndex == sendEndIndex)
                            {
                                send(true);
                                return;
                            }
                            goto SEND;
                        }
                    }
                }
                catch (Exception error)
                {
                    //showjim
                    log.Error.Add(error, "sendIndex[" + sendIndex.toString() + "] sendEndIndex[" + sendEndIndex.toString() + "]", false);
                }
                send(false);
            }
            /// <summary>
            /// 发送数据处理
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="async">异步回调参数</param>
            private unsafe void send(object sender, SocketAsyncEventArgs async)
            {
                ++socket.timeoutIdentity;
                int count = 0;
                try
                {
                    if (async.SocketError == SocketError.Success && (count = async.BytesTransferred) > 0)
                    {
                        sendIndex += count;
                        sendLength += count;
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                if (count <= 0) send(false);
                else if (sendIndex == sendEndIndex) send(true);
                else if (date.nowTime.Now > sendStartTime && sendLength < minReceiveSizePerSecond4 * ((int)(date.nowTime.Now - sendStartTime).TotalSeconds >> 2)) send(false);
                else send();
            }
            /// <summary>
            /// 发送文件数据
            /// </summary>
            /// <param name="onSend">发送数据回调处理</param>
            /// <param name="fileName">文件名称</param>
            /// <param name="seek">起始位置</param>
            /// <param name="size">发送字节长度</param>
            public void SendFile(onSendType onSend, string fileName, long seek, long size)
            {
                try
                {
                    fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, fastCSharp.config.appSetting.StreamBufferSize, FileOptions.SequentialScan);
                    if (fileStream.Length >= seek + size)
                    {
                        if (seek != 0) fileStream.Seek(seek, SeekOrigin.Begin);
                        this.onSend = onSend;
                        sendStartTime = date.nowTime.Now.AddTicks(date.SecondTicks);
                        fileSize = size;
                        sendLength = 0;

                        memoryPool = net.socket.ServerSendBuffers;
                        buffer = memoryPool.Get();
                        FileAsync.SetBuffer(buffer, 0, buffer.Length);
                        FileAsync.SocketError = SocketError.Success;
                        readFile();
                        return;
                    }
                    else fileStream.Dispose();
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                onSendFalse(onSend);
            }
            /// <summary>
            /// 开始发送文件数据
            /// </summary>
            protected override void sendFile()
            {
                try
                {
                    SEND:
                    int timeoutIdentity = socket.timeoutIdentity;
                    FileAsync.SetBuffer(sendIndex, sendEndIndex - sendIndex);
                    if (socket.Socket.SendAsync(FileAsync))
                    {
                        socket.setTimeout(timeoutIdentity);
                        return;
                    }
                    if (FileAsync.SocketError == SocketError.Success)
                    {
                        int count = FileAsync.BytesTransferred;
                        if (count > 0)
                        {
                            sendIndex += count;
                            sendLength += count;
                            if (sendIndex == sendEndIndex)
                            {
                                readFile();
                                return;
                            }
                            goto SEND;
                        }
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                sendFile(false);
            }
            /// <summary>
            /// 发送文件数据处理
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="async">异步回调参数</param>
            private unsafe void sendFile(object sender, SocketAsyncEventArgs async)
            {
                ++socket.timeoutIdentity;
                int count = 0;
                try
                {
                    if (async.SocketError == SocketError.Success && (count = async.BytesTransferred) > 0)
                    {
                        sendIndex += count;
                        sendLength += count;
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                if (count <= 0) sendFile(false);
                else if (sendIndex == sendEndIndex) readFile();
                else if (date.nowTime.Now > sendStartTime && sendLength < minReceiveSizePerSecond4 * ((int)(date.nowTime.Now - sendStartTime).TotalSeconds >> 2)) sendFile(false);
                else sendFile();
            }
        }
        /// <summary>
        /// WebSocket请求接收器
        /// </summary>
        private new unsafe sealed class webSocketIdentityReceiver : webSocketIdentityReceiver<socket>, IDisposable
        {
            /// <summary>
            /// 异步套接字操作
            /// </summary>
            public SocketAsyncEventArgs Async;
            /// <summary>
            /// WebSocket请求接收器
            /// </summary>
            /// <param name="socket">HTTP套接字</param>
            public webSocketIdentityReceiver(socket socket)
                : base(socket)
            {
                Async = socketAsyncEventArgs.Get();
                Async.SocketFlags = System.Net.Sockets.SocketFlags.None;
                Async.DisconnectReuseSocket = false;
                Async.Completed += receive;
                Async.UserToken = this;
                Async.SetBuffer(buffer, 0, HeaderBufferLength);
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                Async.Completed -= receive;
                socketAsyncEventArgs.Push(ref Async);
            }
            /// <summary>
            /// 开始接收请求数据
            /// </summary>
            /// <param name="webSocket"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Receive(webSocket.socket webSocket)
            {
                WebSocket = webSocket;
                webSocket.Client = socket.Socket;
                webSocket.Parser.Buffer = buffer;
                receiveEndIndex = 0;
                Async.SocketError = SocketError.Success;
                receive();
            }
            /// <summary>
            /// 开始接收数据
            /// </summary>
            protected override void receive()
            {
                try
                {
                    RECEIVE:
                    int timeoutIdentity = socket.timeoutIdentity;
                    Async.SetBuffer(receiveEndIndex, HeaderBufferLength - receiveEndIndex);
                    if (socket.Socket.ReceiveAsync(Async))
                    {
                        socket.setWebSocketTimeout(timeoutIdentity);
                        return;
                    }
                    if (Async.SocketError == SocketError.Success)
                    {
                        int count = Async.BytesTransferred;
                        if (count > 0)
                        {
                            receiveEndIndex += count;
                            if ((receiveEndIndex = WebSocket.Parser.Parse(receiveEndIndex)) >= 0) goto RECEIVE;
                        }
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                socket.webSocketEnd();
            }
            /// <summary>
            /// WebSocket请求数据处理
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="async">异步回调参数</param>
            private void receive(object sender, SocketAsyncEventArgs async)
            {
                ++socket.timeoutIdentity;
                int count = int.MinValue;
                try
                {
                    if (async.SocketError == SocketError.Success && (count = async.BytesTransferred) >= 0)
                    {
                        receiveEndIndex += count;
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                if (count <= 0) socket.webSocketEnd();
                //else if (date.NowSecond >= timeout)
                //{
                //    Monitor.Enter(sendLock);
                //    try
                //    {
                //        SocketError error = SocketError.Success;
                //        socket.Socket.send(closeData, 0, 2, ref error);
                //    }
                //    catch { }
                //    finally
                //    {
                //        Monitor.Exit(sendLock);
                //        socket.webSocketEnd();
                //    }
                //}
                //else if (count == 0) fastCSharp.threading.timerTask.Default.Add(receiveHandle, date.NowSecond.AddSeconds(1), null);
                else if ((receiveEndIndex = WebSocket.Parser.Parse(receiveEndIndex)) >= 0) receive();
                else socket.webSocketEnd();
            }
            /// <summary>
            /// 发送消息
            /// </summary>
            /// <param name="webSocket"></param>
            internal unsafe void Send(webSocket.socket webSocket)
            {
                byte[] buffer = fastCSharp.memoryPool.StreamBuffers.Get();
                webSocket.socket.message message = default(webSocket.socket.message);
                SocketError error = SocketError.Success;
                try
                {
                    fixed (byte* bufferFixed = buffer)
                    {
                        using (unmanagedStream stream = new unmanagedStream(bufferFixed, buffer.Length))
                        {
                            sender sender = new sender {  Buffer = buffer, BufferFixed = bufferFixed, Stream = stream };
                            do
                            {
                                if (webSocket == this.WebSocket && webSocket.SocketIdentity == socket.identity)
                                {
                                    if (webSocket.GetMessage(ref message))
                                    {
                                        sender.Send(ref message);
                                        if ((stream.Length << 1) > buffer.Length)
                                        {
                                            byte[] data = sender.GetData();
                                            Monitor.Enter(sendLock);
                                            try
                                            {
                                                if (!webSocket.Client.send(data, 0, stream.Length, ref error)) break;
                                                ++socket.timeoutIdentity;
                                            }
                                            finally { Monitor.Exit(sendLock); }
                                            stream.UnsafeSetLength(0);
                                        }
                                        else if (webSocket.MessageCount == 0) Thread.Sleep(0);
                                    }
                                    else
                                    {
                                        if (stream.Length != 0)
                                        {
                                            byte[] data = sender.GetData();
                                            Monitor.Enter(sendLock);
                                            try
                                            {
                                                if (!webSocket.Client.send(data, 0, stream.Length, ref error)) break;
                                                ++socket.timeoutIdentity;
                                            }
                                            finally { Monitor.Exit(sendLock); }
                                        }
                                        return;
                                    }
                                }
                                else break;
                            }
                            while (true);
                        }
                    }
                }
                finally { fastCSharp.memoryPool.StreamBuffers.PushNotNull(buffer); }
                webSocket.Client.shutdown();
            }
        }
        /// <summary>
        /// HTTP头部接收器
        /// </summary>
        internal headerReceiver HeaderReceiver;
        /// <summary>
        /// 表单数据接收器
        /// </summary>
        private formIdentityReceiver formReceiver;
        /// <summary>
        /// 数据接收器
        /// </summary>
        private boundaryIdentityReceiver boundaryReceiver;
        /// <summary>
        /// 数据发送器
        /// </summary>
        private dataSender sender;
        /// <summary>
        /// WebSocket请求接收器
        /// </summary>
        private webSocketIdentityReceiver webSocketReceiver;
        /// <summary>
        /// HTTP套接字
        /// </summary>
        private socket()
        {
            RequestHeader = (HeaderReceiver = new headerReceiver(this)).RequestHeader;
            sender = new dataSender(this);
        }
        /// <summary>
        /// 开始处理新的请求
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void start()
        {
            isLoadForm = isNextRequest = isTimeout = 0;
            DomainServer = null;
            //form.Clear();
            response.Push(ref response);
            RequestHeader.IsKeepAlive = false;
            HeaderReceiver.Receive();
        }
        /// <summary>
        /// 开始处理新的请求
        /// </summary>
        /// <param name="server">HTTP服务</param>
        /// <param name="socket">套接字</param>
        /// <param name="ip"></param>
        internal void Start(server server, Socket socket, ref ipv6Hash ip)
        {
            Ipv6 = ip;
            Ipv4 = 0;
            this.server = server;
            servers = server.Servers;
            Socket = socket;
            isLoadForm = isNextRequest = isTimeout = 0;
            DomainServer = null;
            //form.Clear();
            response.Push(ref response);
            RequestHeader.IsKeepAlive = false;
            HeaderReceiver.Receive();
        }
        /// <summary>
        /// 开始处理新的请求
        /// </summary>
        /// <param name="server">HTTP服务</param>
        /// <param name="socket">套接字</param>
        /// <param name="ip"></param>
        internal void Start(server server, Socket socket, int ip)
        {
            Ipv4 = ip;
            this.server = server;
            servers = server.Servers;
            Socket = socket;
            isLoadForm = isNextRequest = isTimeout = 0;
            DomainServer = null;
            //form.Clear();
            response.Push(ref response);
            RequestHeader.IsKeepAlive = false;
            HeaderReceiver.Receive();
        }
        /// <summary>
        /// 开始处理新的请求
        /// </summary>
        /// <param name="server">HTTP服务</param>
        /// <param name="socket">套接字</param>
        internal void Start(server server, Socket socket)
        {
            this.server = server;
            servers = server.Servers;
            Socket = socket;
            isLoadForm = isNextRequest = isTimeout = 0;
            DomainServer = null;
            //form.Clear();
            response.Push(ref response);
            RequestHeader.IsKeepAlive = false;
            HeaderReceiver.Receive();
        }
        /// <summary>
        /// 开始处理新的请求
        /// </summary>
        /// <param name="socket">TCP调用套接字</param>
        private void start(commandServer.socket socket)
        {
            this.servers = socket.commandSocketProxy.HttpServers;
            TcpCommandSocket = socket;
            Socket = socket.Socket;
            isLoadForm = isNextRequest = isTimeout = 0;
            DomainServer = null;
            //form.Clear();
            response.Push(ref response);
            RequestHeader.IsKeepAlive = false;
            HeaderReceiver.Receive(socket.receiveData);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            if (Interlocked.Increment(ref isDisposed) == 1) Interlocked.Decrement(ref newCount);
            base.Dispose();
        }
        /// <summary>
        /// HTTP头部接收错误
        /// </summary>
        protected override void headerError()
        {
            form.Clear();
            if (TcpCommandSocket == null)
            {
                if (Socket != null) fastCSharp.threading.disposeTimer.Default.addSocketClose(Socket);
                if (Ipv6.Ip == null)
                {
                    if ((Socket = server.SocketEnd(Ipv4)) != null)
                    {
                        start();
                        return;
                    }
                }
                else
                {
                    if ((Socket = server.SocketEnd(ref Ipv6)) != null)
                    {
                        start();
                        return;
                    }
                    Ipv6.Null();
                }
            }
            else
            {
                TcpCommandSocket.commandSocketProxy.SocketEnd();
                TcpCommandSocket.PushPool();
                TcpCommandSocket = null;
            }
            typePool<socket>.PushNotNull(this);
        }
        /// <summary>
        /// HTTP代理结束
        /// </summary>
        internal void ProxyEnd()
        {
            if (Ipv6.Ip == null)
            {
                if ((Socket = server.SocketEnd(Ipv4)) != null)
                {
                    start();
                    return;
                }
            }
            else
            {
                if ((Socket = server.SocketEnd(ref Ipv6)) != null)
                {
                    start();
                    return;
                }
                Ipv6.Null();

            }
            typePool<socket>.PushNotNull(this);
        }
        /// <summary>
        /// WebSocket结束
        /// </summary>
        protected override void webSocketEnd()
        {
            Interlocked.Increment(ref identity);
            webSocket.socket webSocket = webSocketReceiver.WebSocket;
            webSocketReceiver.WebSocket = null;
            if (Socket != null) fastCSharp.threading.disposeTimer.Default.addSocketClose(Socket);
            webSocket.Close();
            if (Ipv6.Ip == null)
            {
                if ((Socket = server.SocketEnd(Ipv4)) != null)
                {
                    start();
                    return;
                }
            }
            else
            {
                if ((Socket = server.SocketEnd(ref Ipv6)) != null)
                {
                    start();
                    return;
                }
                Ipv6.Null();
            }
            typePool<socket>.PushNotNull(this);
        }
        /// <summary>
        /// 未能识别的HTTP头部
        /// </summary>
        protected override void headerUnknown()
        {
            if (isNextRequest == 0)
            {
                try
                {
                    client client = servers.GetForwardClient();
                    if (client != null)
                    {
                        new forwardProxy(this, client).Start();
                        return;
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
            }
            responseError(http.response.state.BadRequest400);
        }
        /// <summary>
        /// 开始接收头部数据
        /// </summary>
        protected override void receiveHeader()
        {
            HeaderReceiver.Receive();
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        protected override bool send(byte[] buffer, int index, int length)
        {
            SocketError error = SocketError.Success;
            return Socket.send(buffer, index, length, ref error) && error == SocketError.Success;
        }
        /// <summary>
        /// 开始接收WebSocket数据
        /// </summary>          
        /// <param name="webSocket"></param>
        protected override void receiveWebSocket(webSocket.socket webSocket)
        {
            if (webSocketReceiver == null) webSocketReceiver = new webSocketIdentityReceiver(this);
            webSocketReceiver.Receive(webSocket);
        }
        /// <summary>
        /// WebSocket发送消息
        /// </summary>
        /// <param name="webSocket"></param>
        internal override void WebSocketSend(webSocket.socket webSocket)
        {
            webSocketReceiver.Send(webSocket);
        }
        /// <summary>
        /// 获取请求表单数据
        /// </summary>
        /// <param name="identity">HTTP操作标识</param>
        /// <param name="loadForm">HTTP请求表单加载接口</param>
        internal override void GetForm(long identity, requestForm.ILoadForm loadForm)
        {
            if (Interlocked.CompareExchange(ref this.identity, identity + 1, identity) == identity)
            {
                if (isLoadForm == 0)
                {
                    isLoadForm = 1;
                    if (check100Continue())
                    {
                        switch (RequestHeader.PostType)
                        {
                            case requestHeader.postType.Json:
                            case requestHeader.postType.Form:
                            case requestHeader.postType.Xml:
                            case requestHeader.postType.Data:
                                if (formReceiver == null) formReceiver = new formIdentityReceiver(this);
                                formReceiver.Receive(loadForm);
                                return;
                            default:
                                if (boundaryReceiver == null) boundaryReceiver = new boundaryIdentityReceiver(this);
                                boundaryReceiver.Receive(loadForm);
                                return;
                        }
                    }
                }
                else log.Error.Add("表单已加载", null, true);
            }
            loadForm.OnGetForm(null);
        }
        /// <summary>
        /// FORM表单解析
        /// </summary>
        /// <param name="dataToType"></param>
        /// <returns></returns>
        internal override bool ParseForm(requestHeader.postType dataToType = requestHeader.postType.Data)
        {
            return formReceiver.Parse(dataToType);
        }
        /// <summary>
        /// HTTP响应头部输出
        /// </summary>
        /// <param name="buffer">输出数据</param>
        /// <param name="memoryPool">内存池</param>
        protected override void responseHeader(ref subArray<byte> buffer, memoryPool memoryPool)
        {
            if (responseSize == 0)
            {
                response.Push(ref response);
                sender.Send(dataSender.onSendType.Next, ref buffer, memoryPool);
            }
            else sender.Send(dataSender.onSendType.ResponseBody, ref buffer, memoryPool);
        }
        /// <summary>
        /// 输出HTTP响应数据
        /// </summary>
        /// <param name="identity">HTTP操作标识</param>
        /// <param name="response">HTTP响应数据</param>
        public override unsafe bool Response(long identity, ref response response)
        {
            if (Interlocked.CompareExchange(ref this.identity, identity + 1, identity) == identity)
            {
                this.response = response;
                response = null;
                if (this.response.LastModified != null)
                {
                    subArray<byte> ifModifiedSince = RequestHeader.IfModifiedSince;
                    if (ifModifiedSince.length == this.response.LastModified.Length)
                    {
                        fixed (byte* buffer = ifModifiedSince.array)
                        {
                            if (unsafer.memory.Equal(this.response.LastModified, buffer + ifModifiedSince.startIndex, ifModifiedSince.length))
                            {
                                response.Push(ref this.response);
                                this.response = response.NotChanged304;
                            }
                        }
                    }
                }
                if (boundaryReceiver != null) bigBuffers.Push(ref boundaryReceiver.Buffer);
                if (RequestHeader.Method == fastCSharp.web.http.methodType.POST && isLoadForm == 0)
                {
                    switch (RequestHeader.PostType)
                    {
                        case requestHeader.postType.Json:
                        case requestHeader.postType.Form:
                        case requestHeader.postType.Xml:
                        case requestHeader.postType.Data:
                            if (formReceiver == null) formReceiver = new formIdentityReceiver(this);
                            formReceiver.Receive(this);
                            return true;
                        default:
                            if (boundaryReceiver == null) boundaryReceiver = new boundaryIdentityReceiver(this);
                            boundaryReceiver.Receive(this);
                            return true;
                    }
                }
                responseHeader();
                return true;
            }
            response.Push(ref response);
            return false;
        }
        /// <summary>
        /// 发送HTTP响应内容
        /// </summary>
        /// <param name="isSend">是否发送成功</param>
        protected override void responseBody(bool isSend)
        {
            if (isSend)
            {
                if (response.BodyFile == null)
                {
                    subArray<byte> body = response.Body;
                    if (response.State == response.state.PartialContent206)
                    {
                        body.UnsafeSet(body.startIndex + (int)RequestHeader.RangeStart, (int)responseSize);
                    }
                    sender.Send(dataSender.onSendType.Next, ref body, null);
                }
                else sender.SendFile(dataSender.onSendType.Next, response.BodyFile, response.State == response.state.PartialContent206 ? RequestHeader.RangeStart : 0, responseSize);
            }
            else headerError();
        }
        /// <summary>
        /// 输出错误状态
        /// </summary>
        /// <param name="state">错误状态</param>
        protected override void responseError(response.state state)
        {
            if (boundaryReceiver != null) bigBuffers.Push(ref boundaryReceiver.Buffer);
            if (DomainServer != null)
            {
                //domainServer domainServer = DomainServer.Server;
                //if (domainServer == null)
                //{
                //    headerError();
                //    return;
                //}
                //response = domainServer.GetErrorResponseData(state, HeaderReceiver.RequestHeader.IsGZip);
                response = DomainServer.GetErrorResponseData(state, RequestHeader.IsGZip);
                if (response != null)
                {
                    if (state != http.response.state.NotFound404 || RequestHeader.Method != web.http.methodType.GET)
                    {
                        RequestHeader.IsKeepAlive = false;
                    }
                    responseHeader();
                    return;
                }
            }
            byte[] data = errorResponseDatas[(int)state];
            if (data != null)
            {
                if (state == http.response.state.NotFound404 && RequestHeader.Method == web.http.methodType.GET)
                {
                    sender.Send(dataSender.onSendType.Next, subArray<byte>.Unsafe(data, 0, data.Length));
                }
                else
                {
                    RequestHeader.IsKeepAlive = false;
                    sender.Send(dataSender.onSendType.Close, subArray<byte>.Unsafe(data, 0, data.Length));
                }
            }
            else headerError();
        }
        /// <summary>
        /// 搜索引擎404提示
        /// </summary>
        protected override void responseSearchEngine404()
        {
            if (RequestHeader.Method == web.http.methodType.GET)
            {
                sender.Send(dataSender.onSendType.Next, subArray<byte>.Unsafe(searchEngine404Data, 0, searchEngine404Data.Length));
            }
            else
            {
                RequestHeader.IsKeepAlive = false;
                sender.Send(dataSender.onSendType.Close, subArray<byte>.Unsafe(searchEngine404Data, 0, searchEngine404Data.Length));
            }
        }
        ///// <summary>
        ///// 开始处理新的请求
        ///// </summary>
        ///// <param name="server">HTTP服务</param>
        ///// <param name="socket">套接字</param>
        ///// <param name="ip">客户端IP</param>
        //internal static void Start(server server, Socket socket, ref ipv6Hash ip)
        //{
        //START:
        //    try
        //    {
        //        socket value = typePool<socket>.Pop() ?? NewSocket();
        //        value.Ipv6 = ip;
        //        value.Ipv4 = 0;
        //        value.Start(server, socket);
        //    }
        //    catch (Exception error)
        //    {
        //        log.Error.Add(error, null, false);
        //        socket.Close();
        //        if ((socket = server.SocketEnd(ref ip)) != null) goto START;
        //    }
        //}
        ///// <summary>
        ///// 开始处理新的请求
        ///// </summary>
        ///// <param name="server">HTTP服务</param>
        ///// <param name="socket">套接字</param>
        ///// <param name="ip">客户端IP</param>
        //internal static void Start(server server, Socket socket, int ip)
        //{
        //START:
        //    try
        //    {
        //        socket value = typePool<socket>.Pop() ?? NewSocket();
        //        value.Ipv4 = ip;
        //        value.Start(server, socket);
        //    }
        //    catch (Exception error)
        //    {
        //        log.Error.Add(error, null, false);
        //        socket.Close();
        //        if ((socket = server.SocketEnd(ip)) != null) goto START;
        //    }
        //}
        /// <summary>
        /// 开始处理新的请求(用于TCP调用)
        /// </summary>
        /// <param name="socket">TCP调用套接字</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static void Start(commandServer.socket socket)
        {
            (typePool<socket>.Pop() ?? NewSocket()).start(socket);
        }
        /// <summary>
        /// 创建套接字
        /// </summary>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static socket NewSocket()
        {
            Interlocked.Increment(ref newCount);
            return new socket();
        }
    }
}
