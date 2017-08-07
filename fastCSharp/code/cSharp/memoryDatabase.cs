using System;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using System.Text;
using System.IO;
using fastCSharp.memoryDatabase;
using fastCSharp.threading;
using fastCSharp.reflection;
using fastCSharp.io;
using fastCSharp.memoryDatabase.cache;

namespace fastCSharp.code.cSharp
{
    /// <summary>
    /// 内存数据库配置
    /// </summary>
    public partial class memoryDatabase : database
    {
        /// <summary>
        /// 数据库成员信息
        /// </summary>
        internal partial struct dataMember
        {
            /// <summary>
            /// 类型序号集合
            /// </summary>
            private readonly static Dictionary<Type, int> typeIndexs;
            /// <summary>
            /// 成员名称
            /// </summary>
            public string Name;
            /// <summary>
            /// 类型名称
            /// </summary>
            public string TypeName;
            /// <summary>
            /// 类型序号
            /// </summary>
            public int TypeIndex;
            /// <summary>
            /// 成员序号
            /// </summary>
            public int MemberIndex;
            /// <summary>
            /// 数据库成员信息
            /// </summary>
            /// <param name="member">成员信息</param>
            public dataMember(memberIndex member)
            {
                Name = member.Member.Name;
                TypeName = typeIndexs.TryGetValue(member.Type, out TypeIndex) ? null : member.Type.fullName();
                MemberIndex = member.MemberIndex;
            }
            /// <summary>
            /// 获取类型序号
            /// </summary>
            /// <param name="type">类型</param>
            /// <returns>类型序号</returns>
            public static int GetMemberTypeIndex(Type type)
            {
                int index;
                return typeIndexs.TryGetValue(type, out index) ? index : 0;
            }
            static dataMember()
            {
                typeIndexs = dictionary.CreateOnly<Type, int>();
                int index = 0;
                typeIndexs.Add(typeof(bool), ++index);
                typeIndexs.Add(typeof(bool?), ++index);
                typeIndexs.Add(typeof(byte), ++index);
                typeIndexs.Add(typeof(byte?), ++index);
                typeIndexs.Add(typeof(sbyte), ++index);
                typeIndexs.Add(typeof(sbyte?), ++index);
                typeIndexs.Add(typeof(short), ++index);
                typeIndexs.Add(typeof(short?), ++index);
                typeIndexs.Add(typeof(ushort), ++index);
                typeIndexs.Add(typeof(ushort?), ++index);
                typeIndexs.Add(typeof(int), ++index);
                typeIndexs.Add(typeof(int?), ++index);
                typeIndexs.Add(typeof(uint), ++index);
                typeIndexs.Add(typeof(uint?), ++index);
                typeIndexs.Add(typeof(long), ++index);
                typeIndexs.Add(typeof(long?), ++index);
                typeIndexs.Add(typeof(ulong), ++index);
                typeIndexs.Add(typeof(ulong?), ++index);
                typeIndexs.Add(typeof(DateTime), ++index);
                typeIndexs.Add(typeof(DateTime?), ++index);
                typeIndexs.Add(typeof(float), ++index);
                typeIndexs.Add(typeof(float?), ++index);
                typeIndexs.Add(typeof(double), ++index);
                typeIndexs.Add(typeof(double?), ++index);
                typeIndexs.Add(typeof(decimal), ++index);
                typeIndexs.Add(typeof(decimal?), ++index);
                typeIndexs.Add(typeof(Guid), ++index);
                typeIndexs.Add(typeof(Guid?), ++index);
                typeIndexs.Add(typeof(char), ++index);
                typeIndexs.Add(typeof(char?), ++index);
                typeIndexs.Add(typeof(string), ++index);
            }
        }
        /// <summary>
        /// 数据库表格
        /// </summary>
        public abstract class table : IDisposable
        {
            /// <summary>
            /// 数据文件头标识
            /// </summary>
            private const int dataFileHeader = fastCSharp.emit.pub.PuzzleValue;
            /// <summary>
            /// 日志文件头标识
            /// </summary>
            protected static readonly byte[] logFileHeaderData = BitConverter.GetBytes(0x060c5113);
            /// <summary>
            /// 数据库日志文件最大刷新比例(:KB)
            /// </summary>
            private static readonly int maxRefreshPerKB = fastCSharp.config.memoryDatabase.Default.MaxRefreshPerKB;
            /// <summary>
            /// 日志类型
            /// </summary>
            protected enum logType
            {
                /// <summary>
                /// 未知
                /// </summary>
                Unknown,
                /// <summary>
                /// 添加对象
                /// </summary>
                Insert,
                /// <summary>
                /// 修改对象
                /// </summary>
                Update,
                /// <summary>
                /// 删除对象
                /// </summary>
                Delete,
                /// <summary>
                /// 成员变换
                /// </summary>
                MemberData,
                /// <summary>
                /// 日志文件头
                /// </summary>
                LogHeader = 0x060c5113,
            }
            /// <summary>
            /// 序列化数据流
            /// </summary>
            protected struct serializeStream
            {
                /// <summary>
                /// 序列化数据缓冲区
                /// </summary>
                public byte[] Buffer;
                /// <summary>
                /// 序列化数据流
                /// </summary>
                public unmanagedStream Stream;
                /// <summary>
                /// 需要释放缓冲区的内存数据流
                /// </summary>
                public tcpBase.bufferUnmanagedStream BufferUnmanagedStream
                {
                    get { return new tcpBase.bufferUnmanagedStream { Stream = Stream, Buffer = Buffer, Free = fastCSharp.memoryDatabase.physicalOld.Buffers.PushHandle }; }
                }
            }
            /// <summary>
            /// 数据加载器
            /// </summary>
            private struct loader
            {
                /// <summary>
                /// 数据库表格
                /// </summary>
                public table Table;
                /// <summary>
                /// 当前处理数据
                /// </summary>
                private subArray<byte> currentData;
                /// <summary>
                /// 当前加载数据
                /// </summary>
                private subArray<byte> newData;
                /// <summary>
                /// 当前日志数据缓冲
                /// </summary>
                private byte[] buffer;
                /// <summary>
                /// 当前日志长度
                /// </summary>
                private int logLength;
                /// <summary>
                /// 当前文件未读取字节大小
                /// </summary>
                private long fileSize;
                /// <summary>
                /// 异步加载数据
                /// </summary>
                public void LoadTask()
                {
                    threadPool.TinyPool.FastStart(load, null, null);
                }
                /// <summary>
                /// 异步加载数据
                /// </summary>
                private void load()
                {
                    bool isLoaded = false;
                    try
                    {
                        if (Load() && Table.checkIndexSerializeMemberData()) isLoaded = true;
                    }
                    catch (Exception error)
                    {
                        fastCSharp.log.Error.Add(error, null, false);
                    }
                    Table.loaded(isLoaded);
                }
                /// <summary>
                /// 同步加载数据
                /// </summary>
                /// <returns>数据加载是否成功</returns>
                public unsafe bool Load()
                {
                    byte[] currentDataBuffer = null, newDataBuffer = null;
                    if (!Table.isLocalTable)
                    {
                        currentData.UnsafeSet(currentDataBuffer = fastCSharp.memoryPool.StreamBuffers.Get(), 0, 0);
                        newData.UnsafeSet(newDataBuffer = fastCSharp.memoryPool.StreamBuffers.Get(), 0, 0);
                    }
                    buffer = fastCSharp.memoryDatabase.physicalOld.Buffers.Get();
                    try
                    {
                        Table.load(ref currentData);
                        if (currentData.Count == 0) return false;
                        fileSize = Table.physicalIdentity.FileSize - currentData.Count;
                        int index = sizeof(int) + sizeof(int) + sizeof(int), dataLength = currentData.Count - index, memberLength, cmpLength = 0;
                        byte[] memberData = null;
                        fixed (byte* dataFixed = currentData.Array)
                        {
                            if (Table.memoryDatabase.IsIndexSerialize)
                            {
                                if (((*(int*)dataFixed ^ dataFileHeader) | (*(int*)(dataFixed + sizeof(int)) ^ 1)) != 0) return false;
                                memberData = new byte[memberLength = *(int*)(dataFixed + sizeof(int) + sizeof(int))];
                            }
                            else
                            {
                                if (((*(int*)dataFixed ^ dataFileHeader) | *(int*)(dataFixed + sizeof(int)) | (*(int*)(dataFixed + sizeof(int) + sizeof(int)) ^ Table.memberData.Length)) != 0) return false;
                                memberData = new byte[memberLength = Table.memberData.Length];
                            }
                            cmpLength = dataLength >= memberLength ? memberLength : dataLength;
                            unsafer.memory.Copy(dataFixed + index, memberData, cmpLength);
                            index += cmpLength;
                            memberLength -= cmpLength;
                        }
                        if (memberLength != 0)
                        {
                            fixed (byte* memberFixed = memberData)
                            {
                                byte* currentMember = memberFixed + cmpLength;
                                do
                                {
                                    Table.load(ref currentData);
                                    if (currentData.Count != 0)
                                    {
                                        fileSize -= currentData.Count;
                                        unsafer.memory.Copy(currentData.Array, currentMember, index = currentData.Count >= memberLength ? memberLength : currentData.Count);
                                        memberLength -= index;
                                        currentMember += index;
                                    }
                                    else return false;
                                }
                                while (memberLength != 0);
                            }
                        }
                        if (Table.memoryDatabase.IsIndexSerialize)
                        {
                            Table.setIndexSerializeMemberNames(memberData);
                            return load(index);
                        }
                        else if (Table.checkMemberData(memberData)) return load(index);
                        return false;
                    }
                    finally
                    {
                        fastCSharp.memoryDatabase.physicalOld.Buffers.Push(ref buffer);
                        fastCSharp.memoryPool.StreamBuffers.Push(ref currentDataBuffer);
                        fastCSharp.memoryPool.StreamBuffers.Push(ref newDataBuffer);
                    }
                }
                /// <summary>
                /// 检测日志数据缓冲
                /// </summary>
                /// <param name="logLength">日志字节长度</param>
                private void checkBuffer(int logLength)
                {
                    int bufferLength = logLength;
                    if (bufferLength < logLength)
                    {
                        fastCSharp.memoryDatabase.physicalOld.Buffers.Push(ref buffer);
                        buffer = new byte[Math.Max(bufferLength << 1, logLength)];
                    }
                }
                /// <summary>
                /// 加载数据
                /// </summary>
                /// <param name="index">数据起始位置</param>
                /// <returns>是否加载成功</returns>
                private bool load(int index)
                {
                    this.currentData.UnsafeSetLength(this.currentData.Count - index);
                    if ((this.currentData.Count == 0 || loadData(index)) && loadData())
                    {
                        Table.load(ref currentData);
                        if (currentData.Count >= logFileHeaderData.Length)
                        {
                            if (!unsafer.memory.Equal(logFileHeaderData, currentData.Array, logFileHeaderData.Length)) return false;
                            fileSize = Table.physicalIdentity.LogFileSize - currentData.Count;
                            this.currentData.UnsafeSetLength(this.currentData.Count - logFileHeaderData.Length);
                            return (this.currentData.Count == 0 || log(logFileHeaderData.Length)) && log() && Table.loaded();
                        }
                    }
                    return false;
                }
                /// <summary>
                /// 加载数据日志
                /// </summary>
                /// <returns>数据日志加载是否成功</returns>
                private unsafe bool loadData()
                {
                    do
                    {
                        Table.load(ref newData);
                        if (newData.Array != null)
                        {
                            if (newData.Count != 0)
                            {
                                fileSize -= newData.Count;
                                if (this.currentData.Count == 0)
                                {
                                    byte[] data = currentData.Array;
                                    this.currentData.UnsafeSet(newData.Array, newData.StartIndex, newData.Count);
                                    newData.UnsafeSet(data, 0, 0);
                                    if (!loadData(0)) return false;
                                }
                                else if (this.currentData.Count == sizeof(int) && newData.Count >= logLength)
                                {
                                    logLength += sizeof(int);
                                    byte[] data = currentData.Array;
                                    this.currentData.UnsafeSet(newData.Array, 0, newData.Count + sizeof(int));
                                    newData.UnsafeSet(data, 0, 0);
                                    if (!loadData(-sizeof(int))) return false;
                                }
                                else
                                {
                                    this.currentData.UnsafeSetLength(this.currentData.Count - sizeof(int));
                                    if (this.currentData.Count == 0) checkBuffer(logLength);
                                    fixed (byte* bufferFixed = buffer)
                                    {
                                        byte* currentBuffer = bufferFixed + this.currentData.Count;
                                        int length = logLength - this.currentData.Count;
                                        while (true)
                                        {
                                            if (newData.Count >= length)
                                            {
                                                unsafer.memory.Copy(newData.Array, currentBuffer, length);
                                                if (Table.loadData(subArray<byte>.Unsafe(buffer, 0, logLength)))
                                                {
                                                    byte[] data = currentData.Array;
                                                    this.currentData.UnsafeSet(newData.Array, 0, newData.Count - length);
                                                    newData.UnsafeSet(data, 0, 0);
                                                    if (this.currentData.Count != 0 && !loadData(length)) return false;
                                                    break;
                                                }
                                                else return false;
                                            }
                                            else
                                            {
                                                unsafer.memory.Copy(newData.Array, currentBuffer, newData.Count);
                                                currentBuffer += newData.Count;
                                                length -= newData.Count;
                                                Table.load(ref newData);
                                                if (newData.Count != 0) fileSize -= newData.Count;
                                                else return false;
                                            }
                                        }
                                    }
                                }
                            }
                            else return this.currentData.Count == 0 && fileSize == 0;
                        }
                        else return false;
                    }
                    while (true);
                }
                /// <summary>
                /// 加载数据日志
                /// </summary>
                /// <param name="index">当前数据起始位置</param>
                /// <returns>数据日志加载是否成功</returns>
                private unsafe bool loadData(int index)
                {
                    fixed (byte* dataFixed = currentData.Array)
                    {
                        if (index >= 0) logLength = *(int*)(dataFixed + index);
                        while (currentData.Count >= logLength)
                        {
                            if (Table.loadData(subArray<byte>.Unsafe(currentData.Array, index + sizeof(int), logLength - sizeof(int))))
                            {
                                currentData.UnsafeSetLength(currentData.Count - logLength);
                                index += logLength;
                                if (currentData.Count >= sizeof(int)) logLength = *(int*)(dataFixed + index);
                                else break;
                            }
                            else return false;
                        }
                        if (currentData.Count != 0)
                        {
                            logLength = *(int*)(dataFixed + index) - sizeof(int);
                            int length = currentData.Count - sizeof(int);
                            if (length > 0)
                            {
                                checkBuffer(logLength);
                                unsafer.memory.Copy(dataFixed + index + sizeof(int), buffer, length);
                            }
                        }
                    }
                    return true;
                }
                /// <summary>
                /// 加载日志数据
                /// </summary>
                /// <returns>日志加载是否成功</returns>
                private unsafe bool log()
                {
                    do
                    {
                        Table.load(ref newData);
                        if (newData.Array != null)
                        {
                            if (newData.Count != 0)
                            {
                                fileSize -= newData.Count;
                                if (this.currentData.Count == 0)
                                {
                                    byte[] data = currentData.Array;
                                    this.currentData.UnsafeSet(newData.Array, newData.StartIndex, newData.Count);
                                    newData.UnsafeSet(data, 0, 0);
                                    if (!log(0)) return false;
                                }
                                else if (this.currentData.Count == sizeof(int) && newData.Count >= logLength)
                                {
                                    logLength += sizeof(int);
                                    byte[] data = currentData.Array;
                                    this.currentData.UnsafeSet(newData.Array, 0, newData.Count + sizeof(int));
                                    newData.UnsafeSet(data, 0, 0);
                                    if (!log(-sizeof(int))) return false;
                                }
                                else
                                {
                                    this.currentData.UnsafeSetLength(this.currentData.Count - sizeof(int));
                                    if (this.currentData.Count == 0) checkBuffer(logLength);
                                    fixed (byte* bufferFixed = buffer)
                                    {
                                        byte* currentBuffer = bufferFixed + this.currentData.Count;
                                        int length = logLength - this.currentData.Count;
                                        while (true)
                                        {
                                            if (newData.Count >= length)
                                            {
                                                unsafer.memory.Copy(newData.Array, currentBuffer, length);
                                                if (Table.loadLog(subArray<byte>.Unsafe(buffer, sizeof(uint), logLength - sizeof(uint)), *(uint*)bufferFixed))
                                                {
                                                    byte[] data = currentData.Array;
                                                    this.currentData.UnsafeSet(newData.Array, 0, newData.Count - length);
                                                    newData.UnsafeSet(data, 0, 0);
                                                    if (this.currentData.Count != 0 && !log(length)) return false;
                                                    break;
                                                }
                                                else return false;
                                            }
                                            else
                                            {
                                                unsafer.memory.Copy(newData.Array, currentBuffer, newData.Count);
                                                currentBuffer += newData.Count;
                                                length -= newData.Count;
                                                Table.load(ref newData);
                                                if (newData.Count != 0) fileSize -= newData.Count;
                                                else return false;
                                            }
                                        }
                                    }
                                }
                            }
                            else return this.currentData.Count == 0 && fileSize == 0;
                        }
                        else return false;
                    }
                    while (true);
                }
                /// <summary>
                /// 加载日志
                /// </summary>
                /// <param name="index">当前数据起始位置</param>
                /// <returns>日志加载是否成功</returns>
                private unsafe bool log(int index)
                {
                    fixed (byte* dataFixed = currentData.Array)
                    {
                        if (index >= 0) logLength = *(int*)(dataFixed + index);
                        while (currentData.Count >= logLength)
                        {
                            int logIndex = index + sizeof(int);
                            if (Table.loadLog(subArray<byte>.Unsafe(currentData.Array, logIndex + sizeof(uint), logLength - (sizeof(int) + sizeof(uint))), *(uint*)(dataFixed + logIndex)))
                            {
                                currentData.UnsafeSetLength(currentData.Count - logLength);
                                index += logLength;
                                if (currentData.Count >= sizeof(int)) logLength = *(int*)(dataFixed + index);
                                else break;
                            }
                            else return false;
                        }
                        if (currentData.Count != 0)
                        {
                            logLength = *(int*)(dataFixed + index) - sizeof(int);
                            int length = currentData.Count - sizeof(int);
                            if (length > 0)
                            {
                                checkBuffer(logLength);
                                unsafer.memory.Copy(dataFixed + index + sizeof(int), buffer, length);
                            }
                        }
                    }
                    return true;
                }
            }
            /// <summary>
            /// 内存数据库配置
            /// </summary>
            protected readonly memoryDatabase memoryDatabase;
            /// <summary>
            /// 数据库文件名
            /// </summary>
            protected readonly string fileName;
            /// <summary>
            /// 数据加载基本缓存
            /// </summary>
            private fastCSharp.memoryDatabase.cache.ILoadCache cache;
            /// <summary>
            /// 数据库日志文件最小刷新字节数
            /// </summary>
            private long minRefreshSize;
            /// <summary>
            /// 自增成员
            /// </summary>
            protected readonly int identityIndex;
            /// <summary>
            /// 关键字成员集合
            /// </summary>
            protected readonly int[] primaryKeyIndexs;
            /// <summary>
            /// 成员描述数据
            /// </summary>
            protected readonly byte[] memberData;
            /// <summary>
            /// 成员集合
            /// </summary>
            private dataMember[] dataMembers;
            /// <summary>
            /// 当前成员描述数据
            /// </summary>
            private byte[] currentIndexSerializeMemberData;
            /// <summary>
            /// 成员位图索引
            /// </summary>
            protected int[] indexSerializeMemberIndexs;
            /// <summary>
            /// 物理层初始化信息
            /// </summary>
            private physicalServer.physicalIdentity physicalIdentity;
            /// <summary>
            /// 数据加载访问锁
            /// </summary>
            private readonly EventWaitHandle loadWaitHandle;
            /// <summary>
            /// 当前数据字节数
            /// </summary>
            protected long dataSize;
            /// <summary>
            /// 当前日志字节数
            /// </summary>
            protected long logSize;
            /// <summary>
            /// 当前刷新日志字节数
            /// </summary>
            protected long refreshLogSize;
            ///// <summary>
            ///// 序列化数据预留起始位置
            ///// </summary>
            //protected int logSerializeStartIndex;
            /// <summary>
            /// 是否正在刷新日志
            /// </summary>
            protected bool isRefresh;
            /// <summary>
            /// 是否已经打开数据库
            /// </summary>
            protected int isOpen;
            /// <summary>
            /// 数据库文件是否成功打开
            /// </summary>
            public bool IsOpen
            {
                get { return physicalIdentity.IsOpen; }
            }
            /// <summary>
            /// 数据是否加载完毕
            /// </summary>
            private bool isLoaded;
            /// <summary>
            /// 数据是否加载成功
            /// </summary>
            public bool IsLoad { get; private set; }
            /// <summary>
            /// 是否本地表格
            /// </summary>
            protected bool isLocalTable;
            /// <summary>
            /// 数据库表格
            /// </summary>
            /// <param name="type">对象类型</param>
            /// <param name="cache">基本缓存</param>
            protected table(Type type, fastCSharp.memoryDatabase.cache.ILoadCache cache)
            {
                this.cache = cache;
                if (cache == null)
                {
                    fastCSharp.log.Error.Add(type.fullName() + " 缺少基本缓存", false, false);
                    error();
                    return;
                }
                memoryDatabase = type.customAttribute<memoryDatabase>(false, false);
                if (memoryDatabase == null)
                {
                    fastCSharp.log.Error.Add(type.fullName() + " 缺少内存数据库配置", false, false);
                    error();
                    return;
                }
                this.fileName = memoryDatabase.FileName ?? type.onlyName();
                minRefreshSize = (long)(memoryDatabase.MinRefreshSize < fastCSharp.config.memoryDatabase.DefaultMinRefreshSize ? fastCSharp.config.memoryDatabase.Default.MinRefreshSize : memoryDatabase.MinRefreshSize) << 10;

                keyValue<memberIndex, fastCSharp.emit.dataMember>[] dataMembers = database.GetMemberIndexs(type, memoryDatabase);
                memberIndex identity = database.GetIdentity(dataMembers);
                identityIndex = identity != null ? identity.MemberIndex : NullIdentityMemberIndex;
                primaryKeyIndexs = database.GetPrimaryKey(dataMembers).getArray(member => member.MemberIndex);
                this.dataMembers = dataMembers.getArray(member => new dataMember(member.Key));
                memberData = currentIndexSerializeMemberData = code.cSharp.serialize.dataSerialize.Get(this.dataMembers, code.memberFilters.InstanceField, default(memberMap<dataMember[]>));

                if (memoryDatabase.IsAsynchronousLoad) loadWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset, null);
            }
            /// <summary>
            /// 初始化错误
            /// </summary>
            protected void error()
            {
                isOpen = 1;
                if (cache != null) cache.Loaded(false);
                Dispose();
            }
            /// <summary>
            /// 打开数据库
            /// </summary>
            /// <returns>是否成功</returns>
            public bool Open()
            {
                if (Interlocked.Increment(ref isOpen) == 1)
                {
                    isLoaded = true;
                    try
                    {
                        if ((physicalIdentity = open()).IsOpen)
                        {
                            if (physicalIdentity.IsNewCreate)
                            {
                                if (create() && log(logFileHeaderData) && checkIndexSerializeMemberData()) IsLoad = true;
                            }
                            else if (memoryDatabase.IsAsynchronousLoad)
                            {
                                isLoaded = false;
                                new loader { Table = this }.LoadTask();
                            }
                            else if (new loader { Table = this }.Load() && checkIndexSerializeMemberData()) IsLoad = true;
                            else fastCSharp.log.Error.Add("数据库 " + fileName + " 加载失败", false, false);
                        }
                        else fastCSharp.log.Error.Add("数据库 " + fileName + " 打开失败", false, false);
                    }
                    catch (Exception error)
                    {
                        isLoaded = true;
                        fastCSharp.log.Error.Add(error, fileName, false);
                    }
                    if (isLoaded)
                    {
                        cache.Loaded(IsLoad);
                        if (!IsLoad) Dispose();
                    }
                }
                return IsOpen;
            }
            /// <summary>
            /// 设置成员变换名称
            /// </summary>
            /// <param name="members">成员数据</param>
            protected void setIndexSerializeMemberNames(byte[] indexSerializeMemberData)
            {
                currentIndexSerializeMemberData = indexSerializeMemberData;
                indexSerializeMemberIndexs = GetIndexSerializeMemberIndexs(code.cSharp.serialize.deSerialize.Get<dataMember[]>(indexSerializeMemberData));
            }
            /// <summary>
            /// 检测成员变量
            /// </summary>
            /// <param name="memberData">成员数据</param>
            /// <returns>是否检测成功</returns>
            private bool checkMemberData(byte[] memberData)
            {
                dataMember[] dataMembers = code.cSharp.serialize.deSerialize.Get<dataMember[]>(memberData);
                if (dataMembers.Length == this.dataMembers.Length)
                {
                    int index = 0;
                    foreach (dataMember member in this.dataMembers)
                    {
                        dataMember dataMember = dataMembers[index++];
                        if (member.Name != dataMember.Name || ((member.MemberIndex ^ dataMember.MemberIndex) | (member.TypeIndex ^ dataMember.TypeIndex)) != 0) return false;
                        if (member.TypeIndex == 0 && member.TypeName != dataMember.TypeName) return false;
                    }
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 获取成员位图索引
            /// </summary>
            /// <param name="members">成员信息集合</param>
            /// <returns>成员位图索引</returns>
            internal abstract int[] GetIndexSerializeMemberIndexs(dataMember[] members);
            /// <summary>
            /// 打开数据库文件
            /// </summary>
            /// <returns>数据库初始化信息</returns>
            protected abstract physicalServer.physicalIdentity open();
            /// <summary>
            /// 关闭数据库文件
            /// </summary>
            protected abstract void close();
            /// <summary>
            /// 创建数据库文件
            /// </summary>
            /// <returns>是否创建成功</returns>
            protected abstract bool create();
            /// <summary>
            /// 获取文件头数据
            /// </summary>
            /// <param name="stream">文件数据流</param>
            protected unsafe void getHeaderData(unmanagedStream stream)
            {
                int length = memberData.Length + sizeof(int) + sizeof(int) + sizeof(int);
                stream.PrepLength(length);
                byte* data = stream.CurrentData;
                *(int*)data = dataFileHeader;
                *(int*)(data + sizeof(int)) = memoryDatabase.IsIndexSerialize ? 1 : 0;
                *(int*)(data + sizeof(int) + sizeof(int)) = memberData.Length;
                unsafer.memory.Copy(memberData, data + sizeof(int) + sizeof(int) + sizeof(int), memberData.Length);
                stream.Unsafer.AddLength(length);
            }
            /// <summary>
            /// 加载文件数据
            /// </summary>
            /// <param name="data">文件数据,空数组表示结束,null表示失败</param>
            internal abstract void load(ref subArray<byte> data);
            /// <summary>
            /// 加载数据日志
            /// </summary>
            /// <param name="data">日志数据</param>
            /// <returns>数据日志加载是否成功</returns>
            protected abstract bool loadData(subArray<byte> data);
            /// <summary>
            /// 加载日志
            /// </summary>
            /// <param name="data">日志数据</param>
            /// <param name="type">日志类型</param>
            /// <returns>日志加载是否成功</returns>
            protected abstract bool loadLog(subArray<byte> data, uint type);
            /// <summary>
            /// 文件数据加载完毕
            /// </summary>
            /// <returns>加载是否成功结束</returns>
            protected abstract bool loaded();
            /// <summary>
            /// 文件数据异步加载回调处理
            /// </summary>
            /// <param name="isLoad">是否加载成功</param>
            private void loaded(bool isLoad)
            {
                if (isLoad) IsLoad = true;
                else fastCSharp.log.Error.Add("数据库 " + fileName + " 加载失败", false, false);
                cache.Loaded(isLoad);
                isLoaded = true;
                loadWaitHandle.Set();
            }
            /// <summary>
            /// 成员位图索引检测
            /// </summary>
            /// <returns>是否成功</returns>
            private unsafe bool checkIndexSerializeMemberData()
            {
                if (memoryDatabase.IsIndexSerialize)
                {
                    indexSerializeMemberIndexs = GetIndexSerializeMemberIndexs(dataMembers);
                    if (checkMemberData(currentIndexSerializeMemberData)) return true;
                    else
                    {
                        int dataLength = memberData.Length + sizeof(int) + sizeof(int);
                        byte[] buffer = fastCSharp.memoryDatabase.physicalOld.Buffers.Get(dataLength);
                        fixed (byte* bufferFixed = buffer)
                        {
                            using (unmanagedStream memoryStream = new unmanagedStream(bufferFixed, buffer.Length))
                            {
                                *(int*)(memoryStream.Data + sizeof(physicalServer.timeIdentity)) = dataLength;
                                *(uint*)(memoryStream.Data + sizeof(physicalServer.timeIdentity) + sizeof(int)) = (uint)logType.MemberData;
                                unsafer.memory.Copy(memberData, memoryStream.Data + sizeof(physicalServer.timeIdentity) + sizeof(int) + sizeof(int), memberData.Length);
                                memoryStream.Unsafer.SetLength(memoryStream.DataLength);
                                return log(new serializeStream { Buffer = buffer, Stream = memoryStream }, true);
                            }
                        }
                    }
                }
                return true;
            }
            /// <summary>
            /// 等待文件数据异步加载
            /// </summary>
            protected void waitLoad()
            {
                if (!isLoaded) loadWaitHandle.WaitOne();
            }
            /// <summary>
            /// 添加日志
            /// </summary>
            /// <param name="data">日志数据</param>
            /// <returns>是否添加成功</returns>
            protected abstract bool log(byte[] data);
            /// <summary>
            /// 添加日志
            /// </summary>
            /// <param name="serializeStream">序列化数据流</param>
            /// <param name="isLog">是否日志数据</param>
            /// <returns>是否添加成功</returns>
            protected abstract bool log(serializeStream serializeStream, bool isLog);
            /// <summary>
            /// 添加日志
            /// </summary>
            /// <param name="onReturn">添加日志回调</param>
            /// <param name="serializeStream">序列化数据流</param>
            /// <param name="isLog">是否日志数据</param>
            protected abstract void log(Action<asynchronousMethod.returnValue<bool>> onReturn, serializeStream serializeStream, bool isLog);
            /// <summary>
            /// 检测日志文件刷新
            /// </summary>
            /// <param name="size">增加字节数</param>
            /// <returns>是否需要刷新</returns>
            protected bool checkRefreshSize(int size)
            {
                Interlocked.Add(ref logSize, size);
                if (logSize < minRefreshSize || isRefresh) return false;
                long maxSize = (dataSize >> 10) * fastCSharp.config.memoryDatabase.Default.MaxRefreshPerKB;
                if (maxSize > logSize)
                {
                    minRefreshSize = maxSize;
                    return false;
                }
                return true;
            }
            /// <summary>
            /// 写入缓存
            /// </summary>
            /// <param name="isDiskFile">是否写入到磁盘文件</param>
            public abstract bool Flush(bool isDiskFile);
            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                pub.Dispose(ref cache);
                close();
                if (loadWaitHandle != null)
                {
                    loadWaitHandle.Set();
                    loadWaitHandle.Close();
                }
            }
            /// <summary>
            /// 反序列化关键字
            /// </summary>
            /// <param name="data">数据</param>
            /// <param name="key">关键字</param>
            /// <returns>是否成功</returns>
            protected internal unsafe static bool deSerializeKey(subArray<byte> data, out int key)
            {
                if (data.Count == sizeof(int))
                {
                    fixed (byte* dataFixed = data.Array) key = *(int*)(dataFixed + data.StartIndex);
                    return true;
                }
                key = int.MinValue;
                return false;
            }
            /// <summary>
            /// 反序列化关键字
            /// </summary>
            /// <param name="data">数据</param>
            /// <param name="key">关键字</param>
            /// <returns>是否成功</returns>
            protected internal unsafe static bool deSerializeKey(subArray<byte> data, out uint key)
            {
                if (data.Count == sizeof(uint))
                {
                    fixed (byte* dataFixed = data.Array) key = *(uint*)(dataFixed + data.StartIndex);
                    return true;
                }
                key = uint.MinValue;
                return false;
            }
            /// <summary>
            /// 反序列化关键字
            /// </summary>
            /// <param name="data">数据</param>
            /// <param name="key">关键字</param>
            /// <returns>是否成功</returns>
            protected internal unsafe static bool deSerializeKey(subArray<byte> data, out long key)
            {
                if (data.Count == sizeof(long))
                {
                    fixed (byte* dataFixed = data.Array) key = *(long*)(dataFixed + data.StartIndex);
                    return true;
                }
                key = long.MinValue;
                return false;
            }
            /// <summary>
            /// 反序列化关键字
            /// </summary>
            /// <param name="data">数据</param>
            /// <param name="key">关键字</param>
            /// <returns>是否成功</returns>
            protected internal unsafe static bool deSerializeKey(subArray<byte> data, out ulong key)
            {
                if (data.Count == sizeof(ulong))
                {
                    fixed (byte* dataFixed = data.Array) key = *(ulong*)(dataFixed + data.StartIndex);
                    return true;
                }
                key = ulong.MinValue;
                return false;
            }
            /// <summary>
            /// 反序列化关键字
            /// </summary>
            /// <param name="data">数据</param>
            /// <param name="key">关键字</param>
            /// <returns>是否成功</returns>
            protected internal unsafe static bool deSerializeKey(subArray<byte> data, out DateTime key)
            {
                if (data.Count == sizeof(long))
                {
                    fixed (byte* dataFixed = data.Array) key = new DateTime(*(long*)(dataFixed + data.StartIndex));
                    return true;
                }
                key = DateTime.MinValue;
                return false;
            }
            /// <summary>
            /// 反序列化关键字
            /// </summary>
            /// <param name="data">数据</param>
            /// <param name="key">关键字</param>
            /// <returns>是否成功</returns>
            protected internal unsafe static bool deSerializeKey(subArray<byte> data, out string key)
            {
                if (data.Count >= sizeof(int))
                {
                    fixed (byte* dataFixed = data.Array)
                    {
                        byte* start = dataFixed + data.StartIndex;
                        int length = *(int*)start;
                        if ((((length >= 0 ? length : -length) + (3 + sizeof(int))) & (int.MaxValue - 3)) == data.Count)
                        {
                            key = String.DeSerialize(start + sizeof(int), length);
                            return true;
                        }
                    }
                }
                key = null;
                return false;
            }
            /// <summary>
            /// 反序列化关键字
            /// </summary>
            /// <param name="data">数据</param>
            /// <param name="key">关键字</param>
            /// <returns>是否成功</returns>
            protected internal unsafe static bool deSerializeKey(subArray<byte> data, out Guid key)
            {
                if (data.Count == sizeof(Guid))
                {
                    fixed (byte* dataFixed = data.Array) key = *(Guid*)(dataFixed + data.StartIndex);
                    return true;
                }
                key = default(Guid);
                return false;
            }
            /// <summary>
            /// 用于代码生成
            /// </summary>
            /// <typeparam name="valueType"></typeparam>
            /// <param name="data"></param>
            /// <param name="key"></param>
            /// <returns></returns>
            public static bool deSerializeKey<valueType>(subArray<byte> data, out valueType key)
            {
                fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.ErrorOperation);
                key = default(valueType);
                return false;
            }
            /// <summary>
            /// 字符串日志
            /// </summary>
            private unsafe struct stringLog
            {
                /// <summary>
                /// 当前写入位置
                /// </summary>
                public byte* Write;
                /// <summary>
                /// 添加字符串
                /// </summary>
                /// <param name="key">字符串关键字</param>
                public void Append(string key)
                {
                    int length = key.Length, dataLength = (((length << 1) + 3) & (int.MaxValue - 3)) + (sizeof(int) + sizeof(uint) + sizeof(int));
                    if (length != 0)
                    {
                        length = unsafer.String.Serialize(key, Write + (sizeof(int) + sizeof(uint) + sizeof(int)));
                        if (length == key.Length)
                        {
                            *(int*)(Write + (sizeof(int) + sizeof(uint))) = -length;
                            dataLength = ((length + 3) & (int.MaxValue - 3)) + (sizeof(int) + sizeof(uint) + sizeof(int));
                        }
                        else *(int*)(Write + (sizeof(int) + sizeof(uint))) = length;
                    }
                    *(uint*)(Write + sizeof(int)) = (uint)logType.Delete;
                    *(int*)Write = dataLength;
                    Write += dataLength;
                }
            }
            /// <summary>
            /// 获取删除对象的日志数据
            /// </summary>
            /// <param name="key">关键字</param>
            /// <param name="memoryStream">日志内存流</param>
            protected internal unsafe static void deleteLog(int key, unmanagedStream memoryStream)
            {
                *(int*)(memoryStream.Data + sizeof(physicalServer.timeIdentity)) = sizeof(int) + sizeof(uint) + sizeof(int);
                *(uint*)(memoryStream.Data + sizeof(physicalServer.timeIdentity) + sizeof(int)) = (uint)logType.Delete;
                *(int*)(memoryStream.Data + sizeof(physicalServer.timeIdentity) + sizeof(int) + sizeof(uint)) = key;
                memoryStream.Unsafer.SetLength(sizeof(physicalServer.timeIdentity) + sizeof(int) + sizeof(uint) + sizeof(int));
            }
            /// <summary>
            /// 获取删除对象的日志数据
            /// </summary>
            /// <param name="key">关键字</param>
            /// <param name="memoryStream">日志内存流</param>
            protected internal static void deleteLog(uint key, unmanagedStream memoryStream)
            {
                deleteLog((int)key, memoryStream);
            }
            /// <summary>
            /// 获取删除对象的日志数据
            /// </summary>
            /// <param name="key">关键字</param>
            /// <param name="memoryStream">日志内存流</param>
            protected internal unsafe static void deleteLog(long key, unmanagedStream memoryStream)
            {
                *(int*)(memoryStream.Data + sizeof(physicalServer.timeIdentity)) = sizeof(int) + sizeof(uint) + sizeof(long);
                *(uint*)(memoryStream.Data + sizeof(physicalServer.timeIdentity) + sizeof(int)) = (uint)logType.Delete;
                *(long*)(memoryStream.Data + sizeof(physicalServer.timeIdentity) + sizeof(int) + sizeof(uint)) = key;
                memoryStream.Unsafer.SetLength(sizeof(physicalServer.timeIdentity) + sizeof(int) + sizeof(uint) + sizeof(long));
            }
            /// <summary>
            /// 获取删除对象的日志数据
            /// </summary>
            /// <param name="key">关键字</param>
            /// <param name="memoryStream">日志内存流</param>
            protected internal static void deleteLog(ulong key, unmanagedStream memoryStream)
            {
                deleteLog((long)key, memoryStream);
            }
            /// <summary>
            /// 获取删除对象的日志数据
            /// </summary>
            /// <param name="key">关键字</param>
            /// <param name="memoryStream">日志内存流</param>
            protected internal static void deleteLog(DateTime key, unmanagedStream memoryStream)
            {
                deleteLog(key.Ticks, memoryStream);
            }
            /// <summary>
            /// 获取删除对象的日志数据
            /// </summary>
            /// <param name="key">关键字</param>
            /// <param name="memoryStream">日志内存流</param>
            protected internal unsafe static void deleteLog(string key, unmanagedStream memoryStream)
            {
                int length = sizeof(physicalServer.timeIdentity) + (((key.Length << 1) + 3) & (int.MaxValue - 3)) + (sizeof(int) + sizeof(uint) + sizeof(int));
                memoryStream.PrepLength(length);
                stringLog stringLog = new stringLog { Write = memoryStream.Data + sizeof(physicalServer.timeIdentity) };
                stringLog.Append(key);
                memoryStream.Unsafer.SetLength(length);
            }
            /// <summary>
            /// 获取删除对象的日志数据
            /// </summary>
            /// <param name="key">关键字</param>
            /// <param name="memoryStream">日志内存流</param>
            protected internal unsafe static void deleteLog(Guid key, unmanagedStream memoryStream)
            {
                *(int*)(memoryStream.Data + sizeof(physicalServer.timeIdentity)) = sizeof(int) + sizeof(uint) + sizeof(Guid);
                *(uint*)(memoryStream.Data + sizeof(physicalServer.timeIdentity) + sizeof(int)) = (uint)logType.Delete;
                *(Guid*)(memoryStream.Data + sizeof(physicalServer.timeIdentity) + sizeof(int) + sizeof(uint)) = key;
                memoryStream.Unsafer.SetLength(sizeof(physicalServer.timeIdentity) + sizeof(int) + sizeof(uint) + sizeof(Guid));
            }
            /// <summary>
            /// 用于代码生成
            /// </summary>
            /// <param name="key">关键字</param>
            /// <param name="memoryStream">日志内存流</param>
            public static void deleteLog(object key, unmanagedStream memoryStream)
            {
                fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.ErrorOperation);
            }
            /// <summary>
            /// 获取删除对象的日志数据
            /// </summary>
            /// <param name="keys">关键字集合</param>
            /// <param name="count">关键字数量</param>
            /// <param name="memoryStream">日志内存流</param>
            private unsafe static void deleteLog(int* keys, int count, unmanagedStream memoryStream)
            {
                int length = sizeof(physicalServer.timeIdentity) + count * (sizeof(int) + sizeof(uint) + sizeof(int));
                memoryStream.PrepLength(length);
                byte* write = memoryStream.Data + sizeof(physicalServer.timeIdentity);
                for (int* end = keys + count; keys != end; write += sizeof(int) + sizeof(uint) + sizeof(int))
                {
                    *(int*)(write + sizeof(int) + sizeof(uint)) = *keys++;
                    *(uint*)(write + sizeof(int)) = (uint)logType.Delete;
                    *(int*)write = sizeof(int) + sizeof(uint) + sizeof(int);
                }
                memoryStream.Unsafer.SetLength(length);
            }
            /// <summary>
            /// 获取删除对象的日志数据
            /// </summary>
            /// <param name="keys">关键字集合</param>
            /// <param name="memoryStream">日志内存流</param>
            protected internal unsafe static void deleteLog(subArray<int> keys, unmanagedStream memoryStream)
            {
                fixed (int* keyFixed = keys.array) deleteLog(keyFixed + keys.StartIndex, keys.Count, memoryStream);
            }
            /// <summary>
            /// 获取删除对象的日志数据
            /// </summary>
            /// <param name="keys">关键字集合</param>
            /// <param name="memoryStream">日志内存流</param>
            protected internal unsafe static void deleteLog(subArray<uint> keys, unmanagedStream memoryStream)
            {
                fixed (uint* keyFixed = keys.array) deleteLog(((int*)keyFixed) + keys.StartIndex, keys.Count, memoryStream);
            }
            /// <summary>
            /// 获取删除对象的日志数据
            /// </summary>
            /// <param name="keys">关键字集合</param>
            /// <param name="count">关键字数量</param>
            /// <param name="memoryStream">日志内存流</param>
            private unsafe static void deleteLog(long* keys, int count, unmanagedStream memoryStream)
            {
                int length = sizeof(physicalServer.timeIdentity) + count * (sizeof(int) + sizeof(uint) + sizeof(long));
                memoryStream.PrepLength(length);
                byte* write = memoryStream.Data + sizeof(physicalServer.timeIdentity);
                for (long* end = keys + count; keys != end; write += sizeof(int) + sizeof(uint) + sizeof(long))
                {
                    *(long*)(write + sizeof(int) + sizeof(uint)) = *keys++;
                    *(uint*)(write + sizeof(int)) = (uint)logType.Delete;
                    *(int*)write = sizeof(int) + sizeof(uint) + sizeof(long);
                }
                memoryStream.Unsafer.SetLength(length);
            }
            /// <summary>
            /// 获取删除对象的日志数据
            /// </summary>
            /// <param name="keys">关键字集合</param>
            /// <param name="memoryStream">日志内存流</param>
            protected internal unsafe static void deleteLog(subArray<long> keys, unmanagedStream memoryStream)
            {
                fixed (long* keyFixed = keys.array) deleteLog(keyFixed + keys.StartIndex, keys.Count, memoryStream);
            }
            /// <summary>
            /// 获取删除对象的日志数据
            /// </summary>
            /// <param name="keys">关键字集合</param>
            /// <param name="memoryStream">日志内存流</param>
            protected internal unsafe static void deleteLog(subArray<ulong> keys, unmanagedStream memoryStream)
            {
                fixed (ulong* keyFixed = keys.array) deleteLog(((long*)keyFixed) + keys.StartIndex, keys.Count, memoryStream);
            }
            /// <summary>
            /// 获取删除对象的日志数据
            /// </summary>
            /// <param name="keys">关键字集合</param>
            /// <param name="memoryStream">日志内存流</param>
            protected internal unsafe static void deleteLog(subArray<DateTime> keys, unmanagedStream memoryStream)
            {
                int count = keys.Count, length = sizeof(physicalServer.timeIdentity) + count * (sizeof(int) + sizeof(uint) + sizeof(long));
                memoryStream.PrepLength(length);
                fixed (DateTime* keyFixed = keys.array)
                {
                    byte* write = memoryStream.Data + sizeof(physicalServer.timeIdentity);
                    for (DateTime* read = keyFixed + keys.StartIndex, end = read + count; read != end; write += sizeof(int) + sizeof(uint) + sizeof(long))
                    {
                        *(long*)(write + sizeof(int) + sizeof(uint)) = (*read++).Ticks;
                        *(uint*)(write + sizeof(int)) = (uint)logType.Delete;
                        *(int*)write = sizeof(int) + sizeof(uint) + sizeof(long);
                    }
                }
                memoryStream.Unsafer.SetLength(length);
            }
            /// <summary>
            /// 获取删除对象的日志数据
            /// </summary>
            /// <param name="keys">关键字集合</param>
            /// <param name="memoryStream">日志内存流</param>
            protected internal unsafe static void deleteLog(subArray<string> keys, unmanagedStream memoryStream)
            {
                int count = keys.Count, dataLength = sizeof(physicalServer.timeIdentity) + count * (sizeof(int) + sizeof(uint) + sizeof(int));
                foreach (string key in keys)
                {
                    dataLength += ((key.Length << 1) + 3) & (int.MaxValue - 3);
                    if (--count == 0) break;
                }
                memoryStream.PrepLength(dataLength);
                stringLog stringLog = new stringLog { Write = memoryStream.Data + sizeof(physicalServer.timeIdentity) };
                count = keys.Count;
                foreach (string key in keys)
                {
                    stringLog.Append(key);
                    if (--count == 0) break;
                }
                memoryStream.Unsafer.SetLength(dataLength);
            }
            /// <summary>
            /// 获取删除对象的日志数据
            /// </summary>
            /// <param name="keys">关键字集合</param>
            /// <param name="memoryStream">日志内存流</param>
            protected internal unsafe static void deleteLog(subArray<Guid> keys, unmanagedStream memoryStream)
            {
                int count = keys.Count, length = sizeof(physicalServer.timeIdentity) + count * (sizeof(int) + sizeof(uint) + sizeof(Guid));
                memoryStream.PrepLength(length);
                fixed (Guid* keyFixed = keys.array)
                {
                    byte* write = memoryStream.Data + sizeof(physicalServer.timeIdentity);
                    for (Guid* read = keyFixed + keys.StartIndex, end = read + count; read != end; write += sizeof(int) + sizeof(uint) + sizeof(Guid))
                    {
                        *(Guid*)(write + sizeof(int) + sizeof(uint)) = *read++;
                        *(uint*)(write + sizeof(int)) = (uint)logType.Delete;
                        *(int*)write = sizeof(int) + sizeof(uint) + sizeof(Guid);
                    }
                }
                memoryStream.Unsafer.SetLength(length);
            }
            /// <summary>
            /// 打开数据库
            /// </summary>
            /// <typeparam name="valueType">数据类型</typeparam>
            /// <param name="memoryDatabase">数据库</param>
            /// <returns>是否成功</returns>
            public static valueType Open<valueType>(valueType memoryDatabase) where valueType : table
            {
                if (memoryDatabase.Open()) return memoryDatabase;
                fastCSharp.log.Error.Add("内存数据库 " + typeof(valueType).fullName() + " 打开失败", false, false);
                return null;
            }
            unsafe static table()
            {
                fixed (byte* dataFixed = logFileHeaderData = new byte[8])
                {
                    *(int*)dataFixed = 8;
                    *(uint*)(dataFixed + sizeof(int)) = (uint)logType.LogHeader;
                }
            }
        }
        /// <summary>
        /// 数据库表格
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="memberType">数据成员类型</typeparam>
        /// <typeparam name="keyType">关键字类型</typeparam>
        public abstract class table<valueType, memberType, keyType> : table
            where valueType : class, fastCSharp.data.IPrimaryKey<keyType>
            where memberType : fastCSharp.code.cSharp.IMemberMap<memberType>
            where keyType : IEquatable<keyType>
        {
            /// <summary>
            /// 事件缓存
            /// </summary>
            public readonly fastCSharp.memoryDatabase.cache.ILoadCache<valueType, memberType, keyType> Cache;
            /// <summary>
            /// 关键字成员位图
            /// </summary>
            private memberType primaryKeyMemberMap = default(memberType);
            /// <summary>
            /// 数据库表格
            /// </summary>
            /// <param name="cache">事件缓存</param>
            protected table(fastCSharp.memoryDatabase.cache.ILoadCache<valueType, memberType, keyType> cache)
                : base(typeof(valueType), cache)
            {
                if (isOpen == 0)
                {
                    Cache = cache;
                    if (identityIndex != NullIdentityMemberIndex) primaryKeyMemberMap.SetMember(identityIndex);
                    else
                    {
                        foreach (int memberIndex in primaryKeyIndexs) primaryKeyMemberMap.SetMember(memberIndex);
                    }
                }
            }
            /// <summary>
            /// 获取成员位图索引
            /// </summary>
            /// <param name="members">成员信息集合</param>
            /// <returns>成员位图索引</returns>
            internal unsafe override int[] GetIndexSerializeMemberIndexs(dataMember[] members)
            {
                int[] indexSerializeMemberIndexs = new int[memberMap<valueType>.MemberCount];
                fixed (int* indexFixed = indexSerializeMemberIndexs)
                {
                    for (int* start = indexFixed, end = indexFixed + indexSerializeMemberIndexs.Length; start != end; *start++ = fastCSharp.emit.binarySerializer.NullValue) ;
                    Dictionary<subString, int> names = memberMap<valueType>.NameIndexs;
                    int index;
                    foreach (dataMember member in members)
                    {
                        if (names.TryGetValue(member.Name, out index)
                            && (member.TypeIndex == 0 ? memberMap<valueType>.GetMemberType(index).fullName() == member.TypeName : dataMember.GetMemberTypeIndex(memberMap<valueType>.GetMemberType(index)) == member.TypeIndex))
                        {
                            indexFixed[member.MemberIndex] = index;
                        }
                    }
                }
                return indexSerializeMemberIndexs;
            }
            /// <summary>
            /// 加载数据日志
            /// </summary>
            /// <param name="data">日志数据</param>
            /// <returns>数据日志加载是否成功</returns>
            protected override bool loadData(subArray<byte> data)
            {
                valueType value = deSerialize(data);
                if (value != null)
                {
                    Cache.LoadInsert(value, data.Count + sizeof(int));
                    dataSize += data.Count + sizeof(int);
                    return true;
                }
                fastCSharp.log.Error.Add("加载日志数据反序列化错误", false, false);
                return false;
            }
            /// <summary>
            /// 加载日志
            /// </summary>
            /// <param name="data">日志数据</param>
            /// <param name="type">日志类型</param>
            /// <returns>日志加载是否成功</returns>
            protected override bool loadLog(subArray<byte> data, uint type)
            {
                int logLength = data.Count + (sizeof(int) + sizeof(uint));
                bool isDataError = true;
                logSize += logLength;
                switch (type)
                {
                    case (uint)logType.Insert:
                        valueType value = deSerialize(data);
                        if (value != null)
                        {
                            Cache.LoadInsert(value, logLength);
                            dataSize += logLength;
                            logSize -= logLength;
                            return true;
                        }
                        break;
                    case (uint)logType.Update:
                        valueType updateValue = newValue();
                        memberType memberMap;
                        int endIndex;
                        if (deSerialize(updateValue, data, out endIndex, out memberMap) && endIndex == data.StartIndex + data.Count)
                        {
                            if (!memberMap.IsDefault) memberMap.Xor(primaryKeyMemberMap);
                            Cache.LoadUpdate(updateValue, memberMap);
                            memberMap.PushPool();
                            return true;
                        }
                        break;
                    case (uint)logType.Delete:
                        keyType key;
                        if (deSerialize(data, out key))
                        {
                            logSize = Cache.LoadDelete(key);
                            Interlocked.Add(ref logSize, logSize);
                            Interlocked.Add(ref dataSize, -logSize);
                            return true;
                        }
                        break;
                    case (uint)logType.MemberData:
                        if (memoryDatabase.IsIndexSerialize)
                        {
                            setIndexSerializeMemberNames(data.ToArray());
                            return true;
                        }
                        break;
                    case (uint)logType.LogHeader:
                        if (data.Count == 0) return true;
                        break;
                    default:
                        isDataError = false;
                        fastCSharp.log.Error.Add("未能识别的日志类型 " + type.toString(), false, false);
                        break;
                }
                if (isDataError) fastCSharp.log.Error.Add("加载日志数据反序列化错误", false, false);
                return false;
            }
            /// <summary>
            /// 新建对象数据
            /// </summary>
            /// <returns>对象数据</returns>
            protected abstract valueType newValue();
            /// <summary>
            /// 对象反序列化
            /// </summary>
            /// <param name="data">数据</param>
            /// <returns>对象值,失败返回null</returns>
            private valueType deSerialize(subArray<byte> data)
            {
                valueType value = newValue();
                int endIndex;
                memberType memberMap;
                if(!deSerialize(value, data, out endIndex, out memberMap) || endIndex != data.StartIndex + data.Count) value = null;
                memberMap.PushPool();
                return value;
            }
            /// <summary>
            /// 反序列化关键字
            /// </summary>
            /// <param name="data">数据</param>
            /// <param name="key">关键字</param>
            /// <returns>是否成功</returns>
            protected abstract bool deSerialize(subArray<byte> data, out keyType key);
            /// <summary>
            /// 反序列化关键字
            /// </summary>
            /// <param name="data">数据</param>
            /// <param name="key">关键字</param>
            /// <returns>是否成功</returns>
            protected bool deSerializeKey(subArray<byte> data, out keyType key)
            {
                valueType value = deSerialize(data);
                if (value != null)
                {
                    key = value.PrimaryKey;
                    return true;
                }
                key = default(keyType);
                return false;
            }
            /// <summary>
            /// 添加对象
            /// </summary>
            /// <param name="value">对象值</param>
            /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
            /// <returns>添加的对象,日志缓存是否添加失败返回null</returns>
            public unsafe valueType Insert(valueType value, bool isCopy = true)
            {
                if (value != null)
                {
                    if (IsLoad) return insert(value, isCopy);
                    waitLoad();
                    if (IsLoad) return insert(value, isCopy);
                }
                return null;
            }
            /// <summary>
            /// 添加对象
            /// </summary>
            /// <param name="onInserted">添加的对象回调委托,日志缓存是否添加失败返回null</param>
            /// <param name="value">对象值</param>
            /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
            public unsafe void Insert(Action<valueType> onInserted, valueType value, bool isCopy = true)
            {
                if (value != null)
                {
                    try
                    {
                        waitLoad();
                        if (IsLoad)
                        {
                            insert(onInserted, value, isCopy);
                            return;
                        }
                    }
                    catch (Exception error)
                    {
                        fastCSharp.log.Error.Add(error, null, false);
                    }
                }
                if (onInserted != null) onInserted(null);
            }
            /// <summary>
            /// 添加对象
            /// </summary>
            /// <param name="value">对象值</param>
            /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
            /// <returns>添加的对象,日志缓存是否添加失败返回null</returns>
            protected virtual valueType insert(valueType value, bool isCopy)
            {
                return Cache.ContainsKey(value.PrimaryKey) ? null : insertLog(value, isCopy);
            }
            /// <summary>
            /// 添加对象
            /// </summary>
            /// <param name="value">对象值</param>
            /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
            /// <returns>添加的对象,日志缓存是否添加失败返回null</returns>
            protected unsafe valueType insertLog(valueType value, bool isCopy)
            {
                byte[] buffer = fastCSharp.memoryDatabase.physicalOld.Buffers.Get();
                fixed (byte* bufferFixed = buffer)
                {
                    using (unmanagedStream memoryStream = new unmanagedStream(bufferFixed, buffer.Length))
                    {
                        log(memoryStream, (uint)logType.Insert, value, default(memberType));
                        return log(new serializeStream { Buffer = buffer, Stream = memoryStream }, false) ? Cache.Insert(value, memoryStream.Length - sizeof(physicalServer.timeIdentity), isCopy) : null;
                    }
                }
            }
            /// <summary>
            /// 添加对象回调
            /// </summary>
            private sealed class insertCallback : threading.callbackActionPool<insertCallback, valueType>
            {
                /// <summary>
                /// 数据库表格
                /// </summary>
                private table<valueType, memberType, keyType> table;
                /// <summary>
                /// 对象值
                /// </summary>
                public valueType Value;
                /// <summary>
                /// 日志数据长度
                /// </summary>
                public int LogLenth;
                /// <summary>
                /// 是否浅复制缓存对象值,否则返回缓存对象
                /// </summary>
                public bool IsCopy;
                /// <summary>
                /// 添加对象回调
                /// </summary>
                public Action<asynchronousMethod.returnValue<bool>> OnReturn;
                /// <summary>
                /// 添加对象回调
                /// </summary>
                /// <param name="table">数据库表格</param>
                public insertCallback(table<valueType, memberType, keyType> table)
                {
                    this.table = table;
                    OnReturn = onReturn;
                }
                /// <summary>
                /// 添加对象回调
                /// </summary>
                /// <param name="returnValue">添加对象结果</param>
                private unsafe void onReturn(asynchronousMethod.returnValue<bool> returnValue)
                {
                    valueType value = null;
                    if (returnValue.IsReturn && returnValue.Value)
                    {
                        try
                        {
                            value = table.Cache.Insert(Value, LogLenth - sizeof(physicalServer.timeIdentity), IsCopy);
                        }
                        catch (Exception error)
                        {
                            fastCSharp.log.Error.Add(error, null, false);
                        }
                    }
                    push(this, value);
                }
            }
            /// <summary>
            /// 添加对象
            /// </summary>
            /// <param name="onInserted">添加的对象回调委托,日志缓存是否添加失败返回null</param>
            /// <param name="value">对象值</param>
            /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
            protected unsafe void insertLog(Action<valueType> onInserted, valueType value, bool isCopy)
            {
                insertCallback insertCallback = null;
                byte[] buffer = null;
                bool isLog = false;
                try
                {
                    fixed (byte* bufferFixed = buffer = fastCSharp.memoryDatabase.physicalOld.Buffers.Get())
                    {
                        using (unmanagedStream memoryStream = new unmanagedStream(bufferFixed, buffer.Length))
                        {
                            log(memoryStream, (uint)logType.Insert, value, default(memberType));
                            insertCallback = typePool<insertCallback>.Pop() ?? new insertCallback(this);
                            insertCallback.Callback = onInserted;
                            insertCallback.Value = value;
                            insertCallback.IsCopy = isCopy;
                            insertCallback.LogLenth = memoryStream.Length;
                            isLog = true;
                            log(insertCallback.OnReturn, new serializeStream { Buffer = buffer, Stream = memoryStream }, false);
                            return;
                        }
                    }
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
                if (!isLog)
                {
                    fastCSharp.memoryDatabase.physicalOld.Buffers.Push(ref buffer);
                    if (insertCallback == null)
                    {
                        if (onInserted != null) onInserted(null);
                    }
                    else insertCallback.OnReturn(new asynchronousMethod.returnValue<bool> { IsReturn = false });
                }
            }
            /// <summary>
            /// 添加对象
            /// </summary>
            /// <param name="value">对象值</param>
            /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
            /// <param name="cache">自增数据加载基本缓存</param>
            /// <returns>添加的对象,日志缓存是否添加失败返回null</returns>
            protected valueType insert(valueType value, bool isCopy, fastCSharp.memoryDatabase.cache.ILoadIdentityCache<valueType, memberType, keyType> cache)
            {
                value.PrimaryKey = cache.NextIdentity();
                bool isCancelIdentity = false;
                try
                {
                    valueType newValue = insertLog(value, isCopy);
                    if (newValue == null) isCancelIdentity = true;
                    return newValue;
                }
                finally
                {
                    if (isCancelIdentity) cache.CancelIdentity(value.PrimaryKey);
                }
            }
            /// <summary>
            /// 添加对象回调
            /// </summary>
            private sealed class insertIdentityCallback : threading.callbackActionPool<insertIdentityCallback, valueType>
            {
                /// <summary>
                /// 自增数据加载基本缓存
                /// </summary>
                private ILoadIdentityCache<valueType, memberType, keyType> cache;
                /// <summary>
                /// 自增值
                /// </summary>
                public keyType Identity;
                /// <summary>
                /// 添加对象回调
                /// </summary>
                /// <param name="value">添加的对象</param>
                public Action<valueType> OnReturn;
                /// <summary>
                /// 添加对象回调
                /// </summary>
                /// <param name="cache">自增数据加载基本缓存</param>
                public insertIdentityCallback(ILoadIdentityCache<valueType, memberType, keyType> cache)
                {
                    this.cache = cache;
                    OnReturn = onReturn;
                }
                /// <summary>
                /// 添加对象回调
                /// </summary>
                /// <param name="value">添加的对象</param>
                private void onReturn(valueType value)
                {
                    if (value == null)
                    {
                        try
                        {
                            cache.CancelIdentity(Identity);
                        }
                        catch (Exception error)
                        {
                            fastCSharp.log.Error.Add(error, null, false);
                        }
                    }
                    push(this, value);
                }
            }
            /// <summary>
            /// 添加对象
            /// </summary>
            /// <param name="onInserted">添加的对象回调委托,日志缓存是否添加失败返回null</param>
            /// <param name="value">对象值</param>
            /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
            /// <param name="cache">自增数据加载基本缓存</param>
            protected void insert(Action<valueType> onInserted, valueType value, bool isCopy, ILoadIdentityCache<valueType, memberType, keyType> cache)
            {
                insertIdentityCallback insertCallback = typePool<insertIdentityCallback>.Pop();
                bool isInsert = false;
                try
                {
                    if (insertCallback == null) insertCallback = new insertIdentityCallback(cache);
                    insertCallback.Callback = onInserted;
                    insertCallback.Identity = cache.NextIdentity();
                    value.PrimaryKey = insertCallback.Identity;
                    isInsert = true;
                    insertLog(insertCallback.OnReturn, value, isCopy);
                    return;
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
                if (!isInsert)
                {
                    if (insertCallback == null)
                    {
                        if (onInserted != null) onInserted(null);
                    }
                    else insertCallback.OnReturn(null);
                }
            }
            /// <summary>
            /// 添加对象
            /// </summary>
            /// <param name="onInserted">添加的对象回调委托,日志缓存是否添加失败返回null</param>
            /// <param name="value">对象值</param>
            /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
            protected virtual void insert(Action<valueType> onInserted, valueType value, bool isCopy)
            {
                bool isCache = true;
                try
                {
                    isCache = Cache.ContainsKey(value.PrimaryKey);
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
                if (isCache)
                {
                    if (onInserted != null) onInserted(null);
                }
                else insertLog(onInserted, value, isCopy);
            }
            /// <summary>
            /// 批量添加对象
            /// </summary>
            /// <param name="values">待添加的对象集合</param>
            /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
            /// <returns>添加的对象集合,日志缓存是否添加失败返回null</returns>
            public valueType[] Insert(IEnumerable<valueType> values, bool isCopy)
            {
                return Insert(values.getArray(), isCopy);
            }
            /// <summary>
            /// 批量添加对象
            /// </summary>
            /// <param name="onInserted">添加的对象回调委托,日志缓存是否添加失败返回null</param>
            /// <param name="values">待添加的对象集合</param>
            /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
            public void Insert(Action<valueType[]> onInserted, IEnumerable<valueType> values, bool isCopy)
            {
                Insert(onInserted, values.getArray(), isCopy);
            }
            /// <summary>
            /// 批量添加对象
            /// </summary>
            /// <param name="values">待添加的对象集合</param>
            /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
            /// <returns>添加的对象集合,日志缓存是否添加失败返回null</returns>
            public valueType[] Insert(ICollection<valueType> values, bool isCopy)
            {
                return Insert(values.getArray(), isCopy);
            }
            /// <summary>
            /// 批量添加对象
            /// </summary>
            /// <param name="onInserted">添加的对象回调委托,日志缓存是否添加失败返回null</param>
            /// <param name="values">待添加的对象集合</param>
            /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
            public void Insert(Action<valueType[]> onInserted, ICollection<valueType> values, bool isCopy)
            {
                Insert(onInserted, values.getArray(), isCopy);
            }
            /// <summary>
            /// 批量添加对象
            /// </summary>
            /// <param name="values">待添加的对象集合</param>
            /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
            /// <returns>添加的对象集合,日志缓存是否添加失败返回null</returns>
            public unsafe valueType[] Insert(valueType[] values, bool isCopy = true)
            {
                if (values.length() != 0)
                {
                    waitLoad();
                    if (IsLoad)
                    {
                        foreach (valueType value in values) if (value == null) return null;
                        return insert(values, isCopy);
                    }
                }
                return null;
            }
            /// <summary>
            /// 批量添加对象
            /// </summary>
            /// <param name="onInserted">添加的对象回调委托,日志缓存是否添加失败返回null</param>
            /// <param name="values">待添加的对象集合</param>
            /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
            public unsafe void Insert(Action<valueType[]> onInserted, valueType[] values, bool isCopy = true)
            {
                if (values.length() != 0)
                {
                    try
                    {
                        waitLoad();
                        if (IsLoad)
                        {
                            bool isValue = true;
                            foreach (valueType value in values)
                            {
                                if (value == null)
                                {
                                    isValue = false;
                                    break;
                                }
                            }
                            if (isValue)
                            {
                                insert(onInserted, values, isCopy);
                                return;
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        fastCSharp.log.Error.Add(error, null, false);
                    }
                }
                if (onInserted != null) onInserted(null);
            }
            /// <summary>
            /// 批量添加对象
            /// </summary>
            /// <param name="values">待添加的对象集合</param>
            /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
            /// <returns>添加的对象集合,日志缓存是否添加失败返回null</returns>
            protected virtual valueType[] insert(valueType[] values, bool isCopy)
            {
                foreach (valueType value in values)
                {
                    if (Cache.ContainsKey(value.PrimaryKey)) return null;
                }
                return insertLog(values, isCopy);
            }
            /// <summary>
            /// 批量添加对象
            /// </summary>
            /// <param name="onInserted">添加的对象回调委托,日志缓存是否添加失败返回null</param>
            /// <param name="values">待添加的对象集合</param>
            /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
            protected virtual void insert(Action<valueType[]> onInserted, valueType[] values, bool isCopy)
            {
                try
                {
                    bool isValue = true;
                    foreach (valueType value in values)
                    {
                        if (Cache.ContainsKey(value.PrimaryKey))
                        {
                            isValue = false;
                            break;
                        }
                    }
                    if (isValue)
                    {
                        insertLog(onInserted, values, isCopy);
                        return;
                    }
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
                if (onInserted != null) onInserted(null);
            }
            /// <summary>
            /// 批量添加对象
            /// </summary>
            /// <param name="values">待添加的对象集合</param>
            /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
            /// <returns>添加的对象集合,日志缓存是否添加失败返回null</returns>
            protected unsafe valueType[] insertLog(valueType[] values, bool isCopy)
            {
                byte[] buffer = fastCSharp.memoryDatabase.physicalOld.Buffers.Get();
                fixed (byte* bufferFixed = buffer)
                {
                    using (unmanagedStream memoryStream = new unmanagedStream(bufferFixed, buffer.Length))
                    {
                        int[] logSizes = insertLog(memoryStream, (uint)logType.Insert, values, default(memberType));
                        if (log(new serializeStream { Buffer = buffer, Stream = memoryStream }, false))
                        {
                            fixed (int* logSizeFixed = logSizes)
                            {
                                int* logSize = logSizeFixed;
                                for (int index = 0; index != values.Length; ++index) values[index] = Cache.Insert(values[index], *logSize++, isCopy);
                            }
                            return values;
                        }
                    }
                }
                return null;
            }
            /// <summary>
            /// 添加对象回调
            /// </summary>
            private sealed class insertManyCallback : threading.callbackActionPool<insertManyCallback, valueType[]>
            {
                /// <summary>
                /// 数据库表格
                /// </summary>
                private table<valueType, memberType, keyType> table;
                /// <summary>
                /// 对象值
                /// </summary>
                public valueType[] Values;
                /// <summary>
                /// 日志数据长度
                /// </summary>
                public int[] LogSizes;
                /// <summary>
                /// 是否浅复制缓存对象值,否则返回缓存对象
                /// </summary>
                public bool IsCopy;
                /// <summary>
                /// 添加对象回调
                /// </summary>
                public Action<asynchronousMethod.returnValue<bool>> OnReturn;
                /// <summary>
                /// 添加对象回调
                /// </summary>
                /// <param name="table">数据库表格</param>
                public insertManyCallback(table<valueType, memberType, keyType> table)
                {
                    this.table = table;
                    OnReturn = onReturn;
                }
                /// <summary>
                /// 添加对象回调
                /// </summary>
                /// <param name="returnValue">添加对象结果</param>
                private unsafe void onReturn(asynchronousMethod.returnValue<bool> returnValue)
                {
                    valueType[] values = null;
                    if (returnValue.IsReturn && returnValue.Value)
                    {
                        try
                        {
                            fixed (int* logSizeFixed = LogSizes)
                            {
                                int* logSize = logSizeFixed;
                                for (int index = 0; index != Values.Length; ++index) Values[index] = table.Cache.Insert(Values[index], *logSize++, IsCopy);
                                values = Values;
                            }
                        }
                        catch (Exception error)
                        {
                            fastCSharp.log.Error.Add(error, null, false);
                        }
                    }
                    push(this, values);
                }
            }
            /// <summary>
            /// 批量添加对象
            /// </summary>
            /// <param name="onInserted">添加的对象回调委托,日志缓存是否添加失败返回null</param>
            /// <param name="values">待添加的对象集合</param>
            /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
            protected unsafe void insertLog(Action<valueType[]> onInserted, valueType[] values, bool isCopy)
            {
                insertManyCallback insertCallback = null;
                byte[] buffer = null;
                bool isLog = false;
                try
                {
                    fixed (byte* bufferFixed = buffer = fastCSharp.memoryDatabase.physicalOld.Buffers.Get())
                    {
                        using (unmanagedStream memoryStream = new unmanagedStream(bufferFixed, buffer.Length))
                        {
                            int[] logSizes = insertLog(memoryStream, (uint)logType.Insert, values, default(memberType));
                            insertCallback = typePool<insertManyCallback>.Pop() ?? new insertManyCallback(this);
                            insertCallback.Callback = onInserted;
                            insertCallback.Values = values;
                            insertCallback.LogSizes = logSizes;
                            insertCallback.IsCopy = isCopy;
                            isLog = true;
                            log(insertCallback.OnReturn, new serializeStream { Buffer = buffer, Stream = memoryStream }, false);
                        }
                    }
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
                if (!isLog)
                {
                    fastCSharp.memoryDatabase.physicalOld.Buffers.Push(ref buffer);
                    if (insertCallback == null)
                    {
                        if (onInserted != null) onInserted(null);
                    }
                    else insertCallback.OnReturn(new asynchronousMethod.returnValue<bool> { IsReturn = false });
                }
            }
            /// <summary>
            /// 批量添加对象
            /// </summary>
            /// <param name="values">待添加的对象集合</param>
            /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
            /// <param name="cache">自增数据加载基本缓存</param>
            /// <returns>添加的对象集合,日志缓存是否添加失败返回null</returns>
            protected valueType[] insert(valueType[] values, bool isCopy, fastCSharp.memoryDatabase.cache.ILoadIdentityCache<valueType, memberType, keyType> cache)
            {
                foreach (valueType value in values) value.PrimaryKey = cache.NextIdentity();
                bool isCancelIdentity = false;
                try
                {
                    valueType[] newValues = insertLog(values, isCopy);
                    if (newValues == null) isCancelIdentity = true;
                    return newValues;
                }
                finally
                {
                    if (isCancelIdentity)
                    {
                        foreach (valueType value in values) cache.CancelIdentity(value.PrimaryKey);
                    }
                }
            }
            /// <summary>
            /// 添加对象集合回调
            /// </summary>
            private sealed class insertManyIdentityCallback : threading.callbackActionPool<insertManyIdentityCallback, valueType[]>
            {
                /// <summary>
                /// 自增数据加载基本缓存
                /// </summary>
                private ILoadIdentityCache<valueType, memberType, keyType> cache;
                /// <summary>
                /// 待添加对象集合
                /// </summary>
                public valueType[] Values;
                /// <summary>
                /// 添加对象回调
                /// </summary>
                /// <param name="value">添加的对象</param>
                public Action<valueType[]> OnReturn;
                /// <summary>
                /// 添加对象回调
                /// </summary>
                /// <param name="cache">自增数据加载基本缓存</param>
                public insertManyIdentityCallback(ILoadIdentityCache<valueType, memberType, keyType> cache)
                {
                    this.cache = cache;
                    OnReturn = onReturn;
                }
                /// <summary>
                /// 添加对象回调
                /// </summary>
                /// <param name="values">添加的对象集合</param>
                private void onReturn(valueType[] values)
                {
                    if (values == null)
                    {
                        try
                        {
                            foreach (valueType value in Values) cache.CancelIdentity(value.PrimaryKey);
                        }
                        catch (Exception error)
                        {
                            fastCSharp.log.Default.Add(error, null, false);
                        }
                    }
                    push(this, values);
                }
            }
            /// <summary>
            /// 批量添加对象
            /// </summary>
            /// <param name="onInserted">添加的对象回调委托,日志缓存是否添加失败返回null</param>
            /// <param name="values">待添加的对象集合</param>
            /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
            /// <param name="cache">自增数据加载基本缓存</param>
            protected void insert(Action<valueType[]> onInserted, valueType[] values, bool isCopy, fastCSharp.memoryDatabase.cache.ILoadIdentityCache<valueType, memberType, keyType> cache)
            {
                insertManyIdentityCallback insertCallback = typePool<insertManyIdentityCallback>.Pop();
                bool isInsert = false;
                try
                {
                    if (insertCallback == null) insertCallback = new insertManyIdentityCallback(cache);
                    insertCallback.Callback = onInserted;
                    insertCallback.Values = values;
                    foreach (valueType value in values) value.PrimaryKey = cache.NextIdentity();
                    isInsert = true;
                    insertLog(insertCallback.OnReturn, values, isCopy);
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
                if (!isInsert)
                {
                    if (insertCallback == null)
                    {
                        if (onInserted != null) onInserted(null);
                    }
                    else insertCallback.OnReturn(null);
                }
            }
            /// <summary>
            /// 获取日志数据
            /// </summary>
            /// <param name="memoryStream">日志内存流</param>
            /// <param name="logType">日志类型</param>
            /// <param name="values">对象</param>
            /// <param name="memberMap">成员位图</param>
            /// <returns>日志数据</returns>
            private unsafe int[] insertLog(unmanagedStream memoryStream, uint logType, valueType[] values, memberType memberMap)
            {
                int[] logSize = new int[values.Length];
                int index = sizeof(physicalServer.timeIdentity);
                memoryStream.Unsafer.SetLength(sizeof(physicalServer.timeIdentity));
                fixed (int* logSizeFixed = logSize)
                {
                    int* currentLogSize = logSizeFixed;
                    foreach (valueType value in values)
                    {
                        memoryStream.PrepLength(sizeof(int) + sizeof(uint));
                        memoryStream.Unsafer.AddLength(sizeof(int) + sizeof(uint));
                        serialize(memoryStream, value, memberMap);
                        int logLength = memoryStream.Length - index;
                        byte* write = memoryStream.Data + index;
                        *currentLogSize++ = *(int*)write = logLength;
                        *(uint*)(write + sizeof(int)) = logType;
                        index = memoryStream.Length;
                    }
                }
                return logSize;
            }
            /// <summary>
            /// 修改对象
            /// </summary>
            /// <param name="value">对象值</param>
            /// <param name="memberMap">成员位图</param>
            /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
            /// <returns>修改以后的对象值,日志缓存是否添加失败返回null</returns>
            public unsafe valueType Update(valueType value, memberType memberMap, bool isCopy = true)
            {
                if (value != null)
                {
                    waitLoad();
                    object cacheLock = Cache.GetLock(value.PrimaryKey);
                    if (cacheLock != null)
                    {
                        if (!memberMap.IsDefault) memberMap.Or(primaryKeyMemberMap);

                        byte[] buffer = fastCSharp.memoryDatabase.physicalOld.Buffers.Get();
                        fixed (byte* bufferFixed = buffer)
                        {
                            using (unmanagedStream memoryStream = new unmanagedStream(bufferFixed, buffer.Length))
                            {
                                log(memoryStream, (uint)logType.Update, value, memberMap);
                                Monitor.Enter(cacheLock);
                                try
                                {
                                    if (log(new serializeStream { Buffer = buffer, Stream = memoryStream }, true)) return Cache.Update(value, memberMap, isCopy);
                                }
                                finally { Monitor.Exit(cacheLock); }
                            }
                        }
                    }
                }
                return null;
            }
            /// <summary>
            /// 批量修改对象
            /// </summary>
            /// <param name="values">待修改对象集合</param>
            /// <param name="memberMap">成员位图</param>
            /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
            /// <returns>修改以后的对象对象集合,日志缓存是否添加失败返回null</returns>
            public unsafe valueType[] Update(valueType[] values, memberType memberMap, bool isCopy = true)
            {
                waitLoad();
                subArray<valueType> valueList = new subArray<valueType>(values).Remove(value => !Cache.ContainsKey(value.PrimaryKey));
                if (valueList.Count != 0)
                {
                    if (!memberMap.IsDefault) memberMap.Or(primaryKeyMemberMap);
                    byte[] buffer = fastCSharp.memoryDatabase.physicalOld.Buffers.Get();
                    fixed (byte* bufferFixed = buffer)
                    {
                        using (unmanagedStream memoryStream = new unmanagedStream(bufferFixed, buffer.Length))
                        {
                            log(memoryStream, (uint)logType.Update, values = valueList.ToArray(), memberMap);
                            if (log(new serializeStream { Buffer = buffer, Stream = memoryStream }, true))
                            {
                                for (int index = 0; index != values.Length; ++index) values[index] = Cache.Update(values[index], memberMap, isCopy);
                                return values;
                            }
                        }
                    }
                }
                return null;
            }
            /// <summary>
            /// 批量修改对象
            /// </summary>
            /// <param name="values">待修改对象集合</param>
            /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
            /// <returns>修改以后的对象对象集合,日志缓存是否添加失败返回null</returns>
            public unsafe valueType[] Update(keyValue<valueType, memberType>[] values, bool isCopy = true)
            {
                waitLoad();
                subArray<keyValue<valueType, memberType>> valueList = new subArray<keyValue<valueType, memberType>>(values).Remove(value => !Cache.ContainsKey(value.Key.PrimaryKey));
                if (valueList.Count != 0)
                {
                    values = valueList.ToArray();
                    for (int index = 0; index != values.Length; ++index)
                    {
                        if (!values[index].Value.IsDefault) values[index].Value.Or(primaryKeyMemberMap);
                    }
                    byte[] buffer = fastCSharp.memoryDatabase.physicalOld.Buffers.Get();
                    fixed (byte* bufferFixed = buffer)
                    {
                        using (unmanagedStream memoryStream = new unmanagedStream(bufferFixed, buffer.Length))
                        {
                            log(memoryStream, (uint)logType.Update, values);
                            if (log(new serializeStream { Buffer = buffer, Stream = memoryStream }, true))
                            {
                                valueType[] newValues = new valueType[values.Length];
                                for (int index = 0; index != values.Length; ++index)
                                {
                                    keyValue<valueType, memberType> value = values[index];
                                    newValues[index] = Cache.Update(value.Key, value.Value, isCopy);
                                }
                                return newValues;
                            }
                        }
                    }
                }
                return null;
            }
            /// <summary>
            /// 删除记录日志
            /// </summary>
            /// <param name="length">日志长度</param>
            protected abstract void deleteLogSize(int length);
            /// <summary>
            /// 删除对象
            /// </summary>
            /// <param name="key">关键字</param>
            /// <returns>被删除的对象值,日志缓存是否添加失败返回null</returns>
            public unsafe valueType Delete(keyType key)
            {
                waitLoad();
                if (IsLoad && Cache.ContainsKey(key))
                {
                    byte[] buffer = fastCSharp.memoryDatabase.physicalOld.Buffers.Get();
                    fixed (byte* bufferFixed = buffer)
                    {
                        using (unmanagedStream memoryStream = new unmanagedStream(bufferFixed, buffer.Length))
                        {
                            return delete(key, new serializeStream { Buffer = buffer, Stream = memoryStream });
                        }
                    }
                }
                return null;
            }
            /// <summary>
            /// 删除对象
            /// </summary>
            /// <param name="onDeleted">删除对象回调委托,日志缓存是否添加失败返回null</param>
            /// <param name="key">关键字</param>
            /// <returns>被删除的对象值,日志缓存是否添加失败返回null</returns>
            public unsafe void Delete(Action<valueType> onDeleted, keyType key)
            {
                try
                {
                    waitLoad();
                    if (IsLoad && Cache.ContainsKey(key))
                    {
                        byte[] buffer = fastCSharp.memoryDatabase.physicalOld.Buffers.Get();
                        fixed (byte* bufferFixed = buffer)
                        {
                            using (unmanagedStream memoryStream = new unmanagedStream(bufferFixed, buffer.Length))
                            {
                                delete(onDeleted, key, new serializeStream { Buffer = buffer, Stream = memoryStream });
                                return;
                            }
                        }
                    }
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
                if (onDeleted != null) onDeleted(null);
            }
            /// <summary>
            /// 删除对象
            /// </summary>
            /// <param name="key">关键字</param>
            /// <param name="serializeStream">序列化数据流</param>
            /// <returns>被删除的对象值,日志缓存是否添加失败返回null</returns>
            protected unsafe virtual valueType delete(keyType key, serializeStream serializeStream)
            {
                deleteLog(key, serializeStream.Stream);
                return getDelete(key, serializeStream);
            }
            /// <summary>
            /// 删除对象
            /// </summary>
            /// <param name="onDeleted">删除对象回调委托,日志缓存是否添加失败返回null</param>
            /// <param name="key">关键字</param>
            /// <param name="serializeStream">序列化数据流</param>
            protected unsafe virtual void delete(Action<valueType> onDeleted, keyType key, serializeStream serializeStream)
            {
                try
                {
                    deleteLog(key, serializeStream.Stream);
                    getDelete(onDeleted, key, serializeStream);
                    return;
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
                if (onDeleted != null) onDeleted(null);
            }
            /// <summary>
            /// 删除对象
            /// </summary>
            /// <param name="key">关键字</param>
            /// <param name="serializeStream">序列化数据流</param>
            /// <returns>被删除的对象值,日志缓存是否添加失败返回null</returns>
            protected valueType getDelete(keyType key, serializeStream serializeStream)
            {
                if (log(serializeStream, true))
                {
                    keyValue<valueType, int> value = Cache.Delete(key);
                    deleteLogSize(value.Value);
                    return value.Key;
                }
                return null;
            }
            /// <summary>
            /// 删除对象回调
            /// </summary>
            private sealed class deleteCallback : threading.callbackActionPool<deleteCallback, valueType>
            {
                /// <summary>
                /// 数据库表格
                /// </summary>
                private table<valueType, memberType, keyType> table;
                /// <summary>
                /// 删除对象关键字
                /// </summary>
                public keyType Key;
                /// <summary>
                /// 删除对象回调
                /// </summary>
                public Action<asynchronousMethod.returnValue<bool>> OnReturn;
                /// <summary>
                /// 删除对象回调
                /// </summary>
                /// <param name="table">数据库表格</param>
                public deleteCallback(table<valueType, memberType, keyType> table)
                {
                    this.table = table;
                    OnReturn = onReturn;
                }
                /// <summary>
                /// 删除对象回调
                /// </summary>
                /// <param name="returnValue">删除对象结果</param>
                private unsafe void onReturn(asynchronousMethod.returnValue<bool> returnValue)
                {
                    keyValue<valueType, int> value = default(keyValue<valueType, int>);
                    if (returnValue.IsReturn && returnValue.Value)
                    {
                        try
                        {
                            keyValue<valueType, int> cacheValue = table.Cache.Delete(Key);
                            table.deleteLogSize(value.Value);
                        }
                        catch (Exception error)
                        {
                            fastCSharp.log.Error.Add(error, null, false);
                        }
                    }
                    push(this, value.Key);
                }
            }
            /// <summary>
            /// 删除对象
            /// </summary>
            /// <param name="onDeleted">删除对象回调委托,日志缓存是否添加失败返回null</param>
            /// <param name="key">关键字</param>
            /// <param name="serializeStream">序列化数据流</param>
            protected void getDelete(Action<valueType> onDeleted, keyType key, serializeStream serializeStream)
            {
                deleteCallback deleteCallback = typePool<deleteCallback>.Pop();
                bool isLog = false;
                try
                {
                    if (deleteCallback == null) deleteCallback = new deleteCallback(this);
                    deleteCallback.Callback = onDeleted;
                    deleteCallback.Key = key;
                    isLog = true;
                    log(deleteCallback.OnReturn, serializeStream, true);
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
                if (!isLog)
                {
                    fastCSharp.memoryDatabase.physicalOld.Buffers.Push(ref serializeStream.Buffer);
                    if (deleteCallback == null)
                    {
                        if (onDeleted != null) onDeleted(null);
                    }
                    else deleteCallback.OnReturn(new asynchronousMethod.returnValue<bool> { IsReturn = false });
                }
            }
            /// <summary>
            /// 获取删除对象的日志数据
            /// </summary>
            /// <param name="key">关键字</param>
            /// <param name="memoryStream">日志内存流</param>
            protected unsafe void deleteLog(keyType key, unmanagedStream memoryStream)
            {
                valueType value = newValue();
                value.PrimaryKey = key;
                log(memoryStream, (uint)logType.Delete, value, primaryKeyMemberMap);
            }
            /// <summary>
            /// 批量删除对象
            /// </summary>
            /// <param name="keys">关键字集合</param>
            /// <returns>被删除对象集合,日志缓存是否添加失败返回null</returns>
            public unsafe valueType[] Delete(keyType[] keys)
            {
                if (keys.length() != 0)
                {
                    waitLoad();
                    if (IsLoad)
                    {
                        subArray<keyType> keyList = new subArray<keyType>(keys).Remove(key => !Cache.ContainsKey(key));
                        if (keyList.Count != 0)
                        {
                            byte[] buffer = fastCSharp.memoryDatabase.physicalOld.Buffers.Get();
                            fixed (byte* bufferFixed = buffer)
                            {
                                using (unmanagedStream memoryStream = new unmanagedStream(bufferFixed, buffer.Length))
                                {
                                    return delete(new serializeStream { Buffer = buffer, Stream = memoryStream }, keyList);
                                }
                            }
                        }
                    }
                }
                return null;
            }
            /// <summary>
            /// 批量删除对象
            /// </summary>
            /// <param name="onDeleted">删除对象回调委托,日志缓存是否添加失败返回null</param>
            /// <param name="keys">关键字集合</param>
            public unsafe void Delete(Action<valueType[]> onDeleted, keyType[] keys)
            {
                if (keys.length() != 0)
                {
                    try
                    {
                        waitLoad();
                        if (IsLoad)
                        {
                            subArray<keyType> keyList = new subArray<keyType>(keys).Remove(key => !Cache.ContainsKey(key));
                            if (keyList.Count != 0)
                            {
                                byte[] buffer = fastCSharp.memoryDatabase.physicalOld.Buffers.Get();
                                fixed (byte* bufferFixed = buffer)
                                {
                                    using (unmanagedStream memoryStream = new unmanagedStream(bufferFixed, buffer.Length))
                                    {
                                        delete(onDeleted, new serializeStream { Buffer = buffer, Stream = memoryStream }, keyList);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        fastCSharp.log.Error.Add(error, null, false);
                    }
                }
                if (onDeleted != null) onDeleted(null);
            }
            /// <summary>
            /// 批量删除对象
            /// </summary>
            /// <param name="serializeStream">序列化数据流</param>
            /// <param name="keys">关键字集合</param>
            /// <returns>被删除的对象集合,日志缓存是否添加失败返回null</returns>
            protected unsafe virtual valueType[] delete(serializeStream serializeStream, subArray<keyType> keys)
            {
                return delete(serializeStream, keys, deleteLog(serializeStream.Stream, keys));
            }
            /// <summary>
            /// 批量删除对象
            /// </summary>
            /// <param name="onDeleted">删除对象回调委托,日志缓存是否添加失败返回null</param>
            /// <param name="serializeStream">序列化数据流</param>
            /// <param name="keys">关键字集合</param>
            protected unsafe virtual void delete(Action<valueType[]> onDeleted, serializeStream serializeStream, subArray<keyType> keys)
            {
                try
                {
                    delete(onDeleted, serializeStream, keys, deleteLog(serializeStream.Stream, keys));
                    return;
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
                if (onDeleted != null) onDeleted(null);
            }
            /// <summary>
            /// 批量删除对象
            /// </summary>
            /// <param name="serializeStream">序列化数据流</param>
            /// <param name="keys">关键字集合</param>
            /// <param name="values">删除对象集合</param>
            /// <returns>被删除的对象集合,日志缓存是否添加失败返回null</returns>
            protected valueType[] delete(serializeStream serializeStream, subArray<keyType> keys, valueType[] values)
            {
                if (log(serializeStream, true))
                {
                    int logSize = 0;
                    if (values == null)
                    {
                        int index = 0;
                        values = new valueType[keys.Count];
                        foreach (keyType key in keys)
                        {
                            keyValue<valueType, int> value = Cache.Delete(key);
                            logSize -= value.Value;
                            values[index++] = value.Key;
                            if (index == values.Length) break;
                        }
                    }
                    else
                    {
                        for (int index = 0; index != values.Length; ++index)
                        {
                            keyValue<valueType, int> value = Cache.Delete(values[index].PrimaryKey);
                            logSize -= value.Value;
                            values[index] = value.Key;
                        }
                    }
                    deleteLogSize(logSize);
                    return values;
                }
                return null;
            }
            /// <summary>
            /// 删除对象回调
            /// </summary>
            private sealed class deleteManyCallback : threading.callbackActionPool<deleteManyCallback, valueType[]>
            {
                /// <summary>
                /// 数据库表格
                /// </summary>
                private table<valueType, memberType, keyType> table;
                /// <summary>
                /// 删除对象集合
                /// </summary>
                public valueType[] Values;
                /// <summary>
                /// 删除关键字集合
                /// </summary>
                public subArray<keyType> Keys;
                /// <summary>
                /// 删除对象回调
                /// </summary>
                public Action<asynchronousMethod.returnValue<bool>> OnReturn;
                /// <summary>
                /// 删除对象回调
                /// </summary>
                /// <param name="table">数据库表格</param>
                public deleteManyCallback(table<valueType, memberType, keyType> table)
                {
                    this.table = table;
                    OnReturn = onReturn;
                }
                /// <summary>
                /// 删除对象回调
                /// </summary>
                /// <param name="returnValue">删除对象结果</param>
                private unsafe void onReturn(asynchronousMethod.returnValue<bool> returnValue)
                {
                    valueType[] values = null;
                    if (returnValue.IsReturn && returnValue.Value)
                    {
                        try
                        {
                            int logSize = 0;
                            if (Values == null)
                            {
                                int index = 0;
                                Values = new valueType[Keys.Count];
                                foreach (keyType key in Keys)
                                {
                                    keyValue<valueType, int> value = table.Cache.Delete(key);
                                    logSize -= value.Value;
                                    Values[index++] = value.Key;
                                    if (index == Values.Length) break;
                                }
                            }
                            else
                            {
                                for (int index = 0; index != Values.Length; ++index)
                                {
                                    keyValue<valueType, int> value = table.Cache.Delete(Values[index].PrimaryKey);
                                    logSize -= value.Value;
                                    Values[index] = value.Key;
                                }
                            }
                            table.deleteLogSize(logSize);
                            values = Values;
                        }
                        catch (Exception error)
                        {
                            fastCSharp.log.Error.Add(error, null, false);
                        }
                    }
                    push(this, values);
                }
            }
            /// <summary>
            /// 批量删除对象
            /// </summary>
            /// <param name="onDeleted">删除对象回调委托,日志缓存是否添加失败返回null</param>
            /// <param name="serializeStream">序列化数据流</param>
            /// <param name="keys">关键字集合</param>
            /// <param name="values">删除对象集合</param>
            protected void delete(Action<valueType[]> onDeleted, serializeStream serializeStream, subArray<keyType> keys, valueType[] values)
            {
                deleteManyCallback deleteCallback = typePool<deleteManyCallback>.Pop();
                bool isLog = false;
                try
                {
                    if (deleteCallback == null) deleteCallback = new deleteManyCallback(this);
                    deleteCallback.Callback = onDeleted;
                    deleteCallback.Values = values;
                    deleteCallback.Keys = keys;
                    isLog = true;
                    log(deleteCallback.OnReturn, serializeStream, true);
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
                if (!isLog)
                {
                    fastCSharp.memoryDatabase.physicalOld.Buffers.Push(ref serializeStream.Buffer);
                    if (deleteCallback == null)
                    {
                        if (onDeleted != null) onDeleted(null);
                    }
                    else deleteCallback.OnReturn(new asynchronousMethod.returnValue<bool> { IsReturn = false });
                }
            }
            /// <summary>
            /// 获取删除对象的日志数据
            /// </summary>
            /// <param name="memoryStream">日志内存流</param>
            /// <param name="keys">关键字集合</param>
            /// <returns>日志数据</returns>
            protected unsafe valueType[] deleteLog(unmanagedStream memoryStream, subArray<keyType> keys)
            {
                int index = 0;
                valueType[] values = new valueType[keys.Count];
                foreach (keyType key in keys)
                {
                    valueType value = newValue();
                    values[index++] = value;
                    value.PrimaryKey = key;
                    if (index == values.Length) break;
                }
                log(memoryStream, (uint)logType.Delete, values, primaryKeyMemberMap);
                return values;
            }
            /// <summary>
            /// 获取日志数据
            /// </summary>
            /// <param name="memoryStream">日志内存流</param>
            /// <param name="logType">日志类型</param>
            /// <param name="value">对象</param>
            /// <param name="memberMap">成员位图</param>
            /// <returns>日志数据</returns>
            private unsafe void log(unmanagedStream memoryStream, uint logType, valueType value, memberType memberMap)
            {
                memoryStream.Unsafer.SetLength(sizeof(physicalServer.timeIdentity) + sizeof(int) + sizeof(uint));
                serialize(memoryStream, value, memberMap);
                byte* write = memoryStream.Data + sizeof(physicalServer.timeIdentity);
                *(int*)write = memoryStream.Length - sizeof(physicalServer.timeIdentity);
                *(uint*)(write + sizeof(int)) = logType;
            }
            /// <summary>
            /// 获取日志数据
            /// </summary>
            /// <param name="memoryStream">内存日志流</param>
            /// <param name="logType">日志类型</param>
            /// <param name="values">对象</param>
            /// <param name="memberMap">成员位图</param>
            private unsafe void log(unmanagedStream memoryStream, uint logType, valueType[] values, memberType memberMap)
            {
                int index = sizeof(physicalServer.timeIdentity);
                memoryStream.Unsafer.SetLength(sizeof(physicalServer.timeIdentity));
                foreach (valueType value in values)
                {
                    memoryStream.PrepLength(sizeof(int) + sizeof(uint));
                    memoryStream.Unsafer.AddLength(sizeof(int) + sizeof(uint));
                    serialize(memoryStream, value, memberMap);
                    int logLength = memoryStream.Length - index;
                    byte* write = memoryStream.Data + index;
                    *(int*)write = logLength;
                    *(uint*)(write + sizeof(int)) = logType;
                    index = memoryStream.Length;
                }
            }
            /// <summary>
            /// 获取日志数据
            /// </summary>
            /// <param name="memoryStream">内存日志流</param>
            /// <param name="logType">日志类型</param>
            /// <param name="values">对象</param>
            private unsafe void log(unmanagedStream memoryStream, uint logType, keyValue<valueType, memberType>[] values)
            {
                int index = sizeof(physicalServer.timeIdentity);
                memoryStream.Unsafer.SetLength(sizeof(physicalServer.timeIdentity));
                foreach (keyValue<valueType, memberType> value in values)
                {
                    memoryStream.PrepLength(sizeof(int) + sizeof(uint));
                    memoryStream.Unsafer.AddLength(sizeof(int) + sizeof(uint));
                    serialize(memoryStream, value.Key, value.Value);
                    int logLength = memoryStream.Length - index;
                    byte* write = memoryStream.Data + index;
                    *(int*)write = logLength;
                    *(uint*)(write + sizeof(int)) = logType;
                    index = memoryStream.Length;
                }
            }
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="memoryStream">内存数据流</param>
            /// <param name="value">对象值</param>
            /// <param name="memberMap">成员位图</param>
            protected abstract void serialize(unmanagedStream memoryStream, valueType value, memberType memberMap);
            /// <summary>
            /// 对象反序列化
            /// </summary>
            /// <param name="value">目标对象</param>
            /// <param name="data">序列化数据</param>
            /// <param name="endIndex">结束位置</param>
            /// <param name="memberMap">成员位图</param>
            /// <returns>是否成功</returns>
            protected abstract bool deSerialize(valueType value, subArray<byte> data, out int endIndex, out memberType memberMap);
        }
        /// <summary>
        /// 远程数据库表格
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="memberType">数据成员类型</typeparam>
        /// <typeparam name="keyType">关键字类型</typeparam>
        public abstract class remoteTable<valueType, memberType, keyType> : table<valueType, memberType, keyType>
            where valueType : class, fastCSharp.data.IPrimaryKey<keyType>
            where memberType : fastCSharp.code.cSharp.IMemberMap<memberType>
            where keyType : IEquatable<keyType>
        {
            /// <summary>
            /// 数据库物理层服务客户端
            /// </summary>
            private readonly fastCSharp.tcpClient.memoryDatabasePhysical client;
            /// <summary>
            /// 数据库物理层唯一标识
            /// </summary>
            private physicalServer.timeIdentity physicalIdentity;
            /// <summary>
            /// 远程数据库表格
            /// </summary>
            /// <param name="client">数据库物理层服务客户端</param>
            /// <param name="cache">事件缓存</param>
            protected unsafe remoteTable(fastCSharp.memoryDatabase.IPhysicalClient client
                , fastCSharp.memoryDatabase.cache.ILoadCache<valueType, memberType, keyType> cache)
                : base(cache)
            {
                if (isOpen == 0)
                {
                    this.client = client.GetClient();
                    //logSerializeStartIndex = sizeof(int) * 7 + sizeof(physicalServer.timeIdentity);
                }
            }
            /// <summary>
            /// 打开数据库文件
            /// </summary>
            /// <returns>数据库初始化信息</returns>
            protected override physicalServer.physicalIdentity open()
            {
                physicalServer.physicalIdentity identity = client.open(fileName).Value;
                physicalIdentity = identity.Identity;
                return identity;
            }
            /// <summary>
            /// 关闭数据库文件
            /// </summary>
            protected unsafe override void close()
            {
                client.close(physicalIdentity, true);
            }
            /// <summary>
            /// 创建数据库文件
            /// </summary>
            /// <returns>是否创建成功</returns>
            protected override unsafe bool create()
            {
                int length = memberData.Length + sizeof(physicalServer.timeIdentity) + sizeof(int) + sizeof(int) + sizeof(int);
                byte[] buffer = fastCSharp.memoryDatabase.physicalOld.Buffers.Get(length);
                fixed (byte* bufferFixed = buffer)
                {
                    *(physicalServer.timeIdentity*)bufferFixed = physicalIdentity;
                    using (unmanagedStream stream = new unmanagedStream(bufferFixed, buffer.Length))
                    {
                        stream.Unsafer.SetLength(sizeof(physicalServer.timeIdentity));
                        getHeaderData(stream);
                        return client.create(new tcpBase.bufferUnmanagedStream { Stream = stream, Free = fastCSharp.memoryDatabase.physicalOld.Buffers.PushHandle }).Value;
                    }
                }
            }
            /// <summary>
            /// 加载文件数据
            /// </summary>
            /// <param name="data">文件数据,空数组表示结束,null表示失败</param>
            internal unsafe override void load(ref subArray<byte> data)
            {
                fixed (byte* dataFixed = data.Array) *(physicalServer.timeIdentity*)dataFixed = physicalIdentity;
                tcpBase.refSubByteArray buffer = new tcpBase.refSubByteArray { IsClient = true };
                buffer.Buffer.UnsafeSet(data.Array, 0, sizeof(physicalServer.timeIdentity));
                client.load(ref buffer);
                data = buffer.Buffer;
            }
            /// <summary>
            /// 文件数据加载完毕
            /// </summary>
            /// <returns>加载是否成功结束</returns>
            protected override bool loaded()
            {
                return client.loaded(physicalIdentity).Value;
            }
            /// <summary>
            /// 添加日志
            /// </summary>
            /// <param name="data">日志数据</param>
            /// <returns>是否添加成功</returns>
            protected override bool log(byte[] data)
            {
                return client.log(physicalIdentity, data).Value;
            }
            /// <summary>
            /// 添加日志
            /// </summary>
            /// <param name="serializeStream">序列化数据流</param>
            /// <param name="isLog">是否日志数据</param>
            /// <returns>是否添加成功</returns>
            protected unsafe override bool log(serializeStream serializeStream, bool isLog)
            {
                unmanagedStream memoryStream = serializeStream.Stream;
                *(physicalServer.timeIdentity*)memoryStream.Data = physicalIdentity;
                if (memoryStream.DataLength != serializeStream.Buffer.Length)
                {
                    fastCSharp.memoryDatabase.physicalOld.Buffers.Push(ref serializeStream.Buffer);
                }
                if (client.log(serializeStream.BufferUnmanagedStream).Value)
                {
                    int length = memoryStream.Length - sizeof(physicalServer.timeIdentity);
                    if (isLog) log(length);
                    else Interlocked.Add(ref dataSize, length);
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 日志写入回调
            /// </summary>
            private sealed class logCallback : threading.callbackActionPool<logCallback, asynchronousMethod.returnValue<bool>>
            {
                /// <summary>
                /// 远程数据库表格
                /// </summary>
                private remoteTable<valueType, memberType, keyType> table;
                /// <summary>
                /// 日志数据长度
                /// </summary>
                public int LogLength;
                /// <summary>
                /// 是否日志数据
                /// </summary>
                public bool IsLog;
                /// <summary>
                /// 日志写入回调
                /// </summary>
                public Action<asynchronousMethod.returnValue<bool>> OnReturn;
                /// <summary>
                /// 日志写入回调
                /// </summary>
                /// <param name="table">日志写入回调</param>
                public logCallback(remoteTable<valueType, memberType, keyType> table)
                {
                    this.table = table;
                    OnReturn = onReturn;
                }
                /// <summary>
                /// 日志写入回调
                /// </summary>
                /// <param name="returnValue">日志写入结果</param>
                private unsafe void onReturn(asynchronousMethod.returnValue<bool> returnValue)
                {
                    if (returnValue.IsReturn)
                    {
                        try
                        {
                            if (IsLog) table.log(LogLength - sizeof(physicalServer.timeIdentity));
                            else Interlocked.Add(ref table.dataSize, LogLength - sizeof(physicalServer.timeIdentity));
                        }
                        catch (Exception error)
                        {
                            fastCSharp.log.Error.Add(error, null, false);
                        }
                    }
                    push(this, returnValue);
                }
            }
            /// <summary>
            /// 添加日志
            /// </summary>
            /// <param name="onReturn">添加日志回调</param>
            /// <param name="serializeStream">序列化数据流</param>
            /// <param name="isLog">是否日志数据</param>
            protected unsafe override void log(Action<asynchronousMethod.returnValue<bool>> onReturn, serializeStream serializeStream, bool isLog)
            {
                logCallback logCallback = typePool<logCallback>.Pop();
                bool isBuffer = false;
                try
                {
                    if (logCallback == null) logCallback = new logCallback(this);
                    logCallback.Callback = onReturn;
                    unmanagedStream memoryStream = serializeStream.Stream;
                    logCallback.LogLength = memoryStream.Length;
                    logCallback.IsLog = isLog;
                    byte[] data;
                    if (memoryStream.Length <= serializeStream.Buffer.Length)
                    {
                        fixed (byte* bufferFixed = serializeStream.Buffer)
                        {
                            *(physicalServer.timeIdentity*)bufferFixed = physicalIdentity;
                            if (memoryStream.DataLength != serializeStream.Buffer.Length)
                            {
                                unsafer.memory.Copy(memoryStream.Data + sizeof(physicalServer.timeIdentity), bufferFixed + sizeof(physicalServer.timeIdentity), memoryStream.Length - sizeof(physicalServer.timeIdentity));
                            }
                        }
                        data = serializeStream.Buffer;
                    }
                    else
                    {
                        *(physicalServer.timeIdentity*)memoryStream.Data = physicalIdentity;
                        data = memoryStream.GetArray();
                        fastCSharp.memoryDatabase.physicalOld.Buffers.Push(ref serializeStream.Buffer);
                    }
                    isBuffer = true;
                    client.log(new tcpBase.subByteArrayBuffer { Buffer = subArray<byte>.Unsafe(data, 0, logCallback.LogLength), Free = serializeStream.Buffer != null ? fastCSharp.memoryDatabase.physicalOld.Buffers.PushHandle : null }, logCallback.OnReturn);
                    return;
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
                if (!isBuffer)
                {
                    fastCSharp.memoryDatabase.physicalOld.Buffers.Push(ref serializeStream.Buffer);
                    if (logCallback == null) onReturn(new asynchronousMethod.returnValue<bool> { IsReturn = false });
                    else logCallback.OnReturn(new asynchronousMethod.returnValue<bool> { IsReturn = false });
                }
            }
            /// <summary>
            /// 添加日志字节数
            /// </summary>
            /// <param name="length">字节数</param>
            private void log(int length)
            {
                if (checkRefreshSize(length))
                {
                    bool isRefresh = false;
                    try
                    {
                        if (client.lockRefresh(physicalIdentity).Value) this.isRefresh = isRefresh = true;
                    }
                    catch (Exception error)
                    {
                        fastCSharp.log.Error.Add(error, null, false);
                    }
                    if (isRefresh) threadPool.TinyPool.FastStart(refresh, null, null);
                }
            }
            /// <summary>
            /// 删除记录日志
            /// </summary>
            /// <param name="length">日志长度</param>
            protected override void deleteLogSize(int length)
            {
                Interlocked.Add(ref dataSize, -length);
                log(length);
            }
            /// </summary>
            /// 刷新日志文件
            /// </summary>
            private unsafe void refresh()
            {
                Interlocked.Add(ref logSize, -(refreshLogSize = logSize));
                pointer buffer = new pointer();
                bool isRefreshEnd = false;
                try
                {
                    Flush(true);
                    if (client.createRefresh(physicalIdentity, logFileHeaderData).Value)
                    {
                        bool isRefresh = true;
                        buffer = fastCSharp.unmanagedPool.StreamBuffers.Get();
                        int bufferSize = fastCSharp.unmanagedPool.StreamBuffers.Size;
                        using (unmanagedStream memoryStream = new unmanagedStream(buffer.Byte, bufferSize))
                        {
                            unmanagedStream.unsafer memoryStreamUnsafer = memoryStream.Unsafer;
                            memoryStreamUnsafer.SetLength(sizeof(physicalServer.timeIdentity));
                            getHeaderData(memoryStream);
                            foreach (valueType value in Cache.Array)
                            {
                                int length = memoryStream.Length;
                                memoryStream.PrepLength(sizeof(int));
                                memoryStreamUnsafer.AddLength(sizeof(int));
                                serialize(memoryStream, value, default(memberType));
                                byte* write = memoryStream.Data + length;
                                *(int*)write = memoryStream.Length - length;
                                if (memoryStream.Length >= bufferSize)
                                {
                                    *(physicalServer.timeIdentity*)memoryStream.Data = physicalIdentity;
                                    if (!client.refresh(new tcpBase.bufferUnmanagedStream { Stream = memoryStream }).Value)
                                    {
                                        isRefresh = false;
                                        break;
                                    }
                                    memoryStreamUnsafer.SetLength(sizeof(physicalServer.timeIdentity));
                                }
                            }
                            if (memoryStream.Length != sizeof(physicalServer.timeIdentity) + sizeof(int))
                            {
                                *(physicalServer.timeIdentity*)memoryStream.Data = physicalIdentity;
                                if (!client.refresh(new tcpBase.bufferUnmanagedStream { Stream = memoryStream }).Value) isRefresh = false;
                            }
                        }
                        if (isRefresh) isRefreshEnd = client.refreshEnd(physicalIdentity).Value;
                    }
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
                finally
                {
                    if (!isRefreshEnd) Interlocked.Add(ref logSize, refreshLogSize);
                    this.isRefresh = false;
                    fastCSharp.unmanagedPool.StreamBuffers.Push(ref buffer);
                }
            }
            /// <summary>
            /// 写入缓存
            /// </summary>
            /// <param name="isDiskFile">是否写入到磁盘文件</param>
            public override bool Flush(bool isDiskFile)
            {
                return client.flush(physicalIdentity, isDiskFile).Value;
            }
        }
        /// <summary>
        /// 本地数据库表格
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="memberType">数据成员类型</typeparam>
        /// <typeparam name="keyType">关键字类型</typeparam>
        public abstract class localTable<valueType, memberType, keyType> : table<valueType, memberType, keyType>
            where valueType : class, fastCSharp.data.IPrimaryKey<keyType>
            where memberType : fastCSharp.code.cSharp.IMemberMap<memberType>
            where keyType : IEquatable<keyType>
        {
            /// <summary>
            /// 数据库物理层集合唯一标识
            /// </summary>
            private physicalSet.identity physicalIdentity;
            /// <summary>
            /// 本地数据库表格
            /// </summary>
            /// <param name="cache">事件缓存</param>
            protected localTable(fastCSharp.memoryDatabase.cache.ILoadCache<valueType, memberType, keyType> cache)
                : base(cache)
            {
                isLocalTable = true;
            }
            /// <summary>
            /// 打开数据库文件
            /// </summary>
            /// <returns>数据库初始化信息</returns>
            protected override physicalServer.physicalIdentity open()
            {
                physicalServer.physicalIdentity identity = physicalSet.Default.Open(fileName);
                physicalIdentity = identity.Identity.GetIdentity();
                return identity;
            }
            /// <summary>
            /// 关闭数据库文件
            /// </summary>
            protected override void close()
            {
                physicalSet.Default.Close(physicalIdentity, true);
            }
            /// <summary>
            /// 创建数据库文件
            /// </summary>
            /// <returns>是否创建成功</returns>
            protected unsafe override bool create()
            {
                int length = memberData.Length + sizeof(int) + sizeof(int) + sizeof(int);
                byte[] buffer = fastCSharp.memoryDatabase.physicalOld.Buffers.Get(length);
                try
                {
                    fixed (byte* bufferFixed = buffer)
                    {
                        using (unmanagedStream stream = new unmanagedStream(bufferFixed, buffer.Length))
                        {
                            getHeaderData(stream);
                            return physicalSet.Default.Create(physicalIdentity, subArray<byte>.Unsafe(buffer, 0, length));
                        }
                    }
                }
                finally
                {
                    fastCSharp.memoryDatabase.physicalOld.Buffers.Push(ref buffer);
                }
            }
            /// <summary>
            /// 加载文件数据
            /// </summary>
            /// <param name="data">文件数据,空数组表示结束,null表示失败</param>
            internal override void load(ref subArray<byte> data)
            {
                physicalSet.Default.Load(physicalIdentity, ref data);
            }
            /// <summary>
            /// 文件数据加载完毕
            /// </summary>
            /// <returns>加载是否成功结束</returns>
            protected override bool loaded()
            {
                return physicalSet.Default.Loaded(physicalIdentity);
            }
            /// <summary>
            /// 添加日志
            /// </summary>
            /// <param name="data">日志数据</param>
            /// <returns>是否添加成功</returns>
            protected override bool log(byte[] data)
            {
                return physicalSet.Default.Log(physicalIdentity, new memoryPool.pushSubArray { Value = subArray<byte>.Unsafe(data, 0, data.Length) });
            }
            /// <summary>
            /// 添加日志
            /// </summary>
            /// <param name="serializeStream">序列化数据流</param>
            /// <param name="isLog">是否日志数据</param>
            /// <returns>是否添加成功</returns>
            protected unsafe override bool log(serializeStream serializeStream, bool isLog)
            {
                unmanagedStream memoryStream = serializeStream.Stream;
                int length = memoryStream.Length - sizeof(physicalServer.timeIdentity);
                if (memoryStream.Length <= serializeStream.Buffer.Length)
                {
                    if (memoryStream.DataLength != serializeStream.Buffer.Length)
                    {
                        fixed (byte* bufferFixed = serializeStream.Buffer)
                        {
                            unsafer.memory.Copy(memoryStream.Data + sizeof(physicalServer.timeIdentity), bufferFixed + sizeof(physicalServer.timeIdentity), length);
                        }
                    }
                    if (physicalSet.Default.Log(physicalIdentity, new memoryPool.pushSubArray { Value = subArray<byte>.Unsafe(serializeStream.Buffer, sizeof(physicalServer.timeIdentity), length), PushPool = fastCSharp.memoryDatabase.physicalOld.Buffers.PushHandle }))
                    {
                        if (isLog) log(length);
                        else Interlocked.Add(ref dataSize, length);
                        return true;
                    }
                }
                else
                {
                    byte[] data = memoryStream.GetArray(sizeof(physicalServer.timeIdentity));
                    fastCSharp.memoryDatabase.physicalOld.Buffers.Push(ref serializeStream.Buffer);
                    if (physicalSet.Default.Log(physicalIdentity, new memoryPool.pushSubArray { Value = subArray<byte>.Unsafe(data, sizeof(physicalServer.timeIdentity), length) }))
                    {
                        if (isLog) log(length);
                        else Interlocked.Add(ref dataSize, length);
                        return true;
                    }
                }
                return false;
            }
            /// <summary>
            /// 添加日志
            /// </summary>
            /// <param name="onReturn">添加日志回调</param>
            /// <param name="serializeStream">序列化数据流</param>
            /// <param name="isLog">是否日志数据</param>
            protected unsafe override void log(Action<asynchronousMethod.returnValue<bool>> onReturn, serializeStream serializeStream, bool isLog)
            {
                bool isReturn = false;
                try
                {
                    unmanagedStream memoryStream = serializeStream.Stream;
                    int length = memoryStream.Length - sizeof(physicalServer.timeIdentity);
                    if (memoryStream.Length <= serializeStream.Buffer.Length)
                    {
                        if (memoryStream.DataLength != serializeStream.Buffer.Length)
                        {
                            fixed (byte* bufferFixed = serializeStream.Buffer)
                            {
                                unsafer.memory.Copy(memoryStream.Data + sizeof(physicalServer.timeIdentity), bufferFixed + sizeof(physicalServer.timeIdentity), length);
                            }
                        }
                        subArray<byte> data = subArray<byte>.Unsafe(serializeStream.Buffer, sizeof(physicalServer.timeIdentity), length);
                        if (physicalSet.Default.Log(physicalIdentity, new memoryPool.pushSubArray { Value = data, PushPool = fastCSharp.memoryDatabase.physicalOld.Buffers.PushHandle }))
                        {
                            isReturn = true;
                        }
                    }
                    else
                    {
                        byte[] data = memoryStream.GetArray(sizeof(physicalServer.timeIdentity));
                        fastCSharp.memoryDatabase.physicalOld.Buffers.Push(ref serializeStream.Buffer);
                        if (physicalSet.Default.Log(physicalIdentity, new memoryPool.pushSubArray { Value = subArray<byte>.Unsafe(data, sizeof(physicalServer.timeIdentity), length) }))
                        {
                            isReturn = true;
                        }
                    }
                    if (isReturn)
                    {
                        if (isLog) log(length);
                        else Interlocked.Add(ref dataSize, length);
                    }
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
                finally { onReturn(isReturn); }
            }
            /// <summary>
            /// 添加日志字节数
            /// </summary>
            /// <param name="length">字节数</param>
            private void log(int length)
            {
                if (checkRefreshSize(length))
                {
                    bool isRefresh = false;
                    try
                    {
                        if (physicalSet.Default.LockRefresh(physicalIdentity)) this.isRefresh = isRefresh = true;
                    }
                    catch (Exception error)
                    {
                        fastCSharp.log.Error.Add(error, null, false);
                    }
                    if (isRefresh) threadPool.TinyPool.FastStart(refresh, null, null);
                }
            }
            /// <summary>
            /// 删除记录日志
            /// </summary>
            /// <param name="length">日志长度</param>
            protected override void deleteLogSize(int length)
            {
                Interlocked.Add(ref dataSize, -length);
                log(length);
            }
            /// <summary>
            /// 刷新日志文件
            /// </summary>
            private unsafe void refresh()
            {
                Interlocked.Add(ref logSize, -(refreshLogSize = logSize));
                pointer buffer = new pointer();
                bool isRefreshEnd = false;
                try
                {
                    if (physicalSet.Default.CreateRefresh(physicalIdentity, logFileHeaderData))
                    {
                        bool isRefresh = true;
                        buffer = fastCSharp.unmanagedPool.StreamBuffers.Get();
                        int bufferSize = fastCSharp.unmanagedPool.StreamBuffers.Size;
                        using (unmanagedStream memoryStream = new unmanagedStream(buffer.Byte, bufferSize))
                        {
                            getHeaderData(memoryStream);
                            unmanagedStream.unsafer memoryStreamUnsafer = memoryStream.Unsafer;
                            foreach (valueType value in Cache.Array)
                            {
                                int length = memoryStream.Length;
                                memoryStream.PrepLength(sizeof(int));
                                memoryStreamUnsafer.AddLength(sizeof(int));
                                serialize(memoryStream, value, default(memberType));
                                byte* write = memoryStream.Data + length;
                                *(int*)write = memoryStream.Length - length;
                                if (memoryStream.Length >= bufferSize)
                                {
                                    if (!physicalSet.Default.Refresh(physicalIdentity, memoryStream))
                                    {
                                        isRefresh = false;
                                        break;
                                    }
                                    memoryStream.Clear();
                                }
                            }
                            if (memoryStream.Length != 0 && !physicalSet.Default.Refresh(physicalIdentity, memoryStream)) isRefresh = false;
                        }
                        if (isRefresh) isRefreshEnd = physicalSet.Default.RefreshEnd(physicalIdentity);
                    }
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
                finally
                {
                    if (!isRefreshEnd) Interlocked.Add(ref logSize, refreshLogSize);
                    this.isRefresh = false;
                    fastCSharp.unmanagedPool.StreamBuffers.Push(ref buffer);
                }
            }
            /// <summary>
            /// 写入缓存
            /// </summary>
            /// <param name="isDiskFile">是否写入到磁盘文件</param>
            public override bool Flush(bool isDiskFile)
            {
                return physicalSet.Default.Flush(physicalIdentity, isDiskFile);
            }
        }
        /// <summary>
        /// 数据库表格关键字(反射模式)
        /// </summary>
        private static class tableKey
        {
            /// <summary>
            /// 反序列化关键字
            /// </summary>
            /// <typeparam name="keyType">关键字类型</typeparam>
            /// <param name="data">数据</param>
            /// <param name="key">关键字</param>
            /// <returns>是否成功</returns>
            public delegate bool deSerialize<keyType>(subArray<byte> data, out keyType key);
            /// <summary>
            /// 关键字处理委托
            /// </summary>
            public struct delegates
            {
                /// <summary>
                /// 反序列化
                /// </summary>
                public Delegate DeSerialize;
                /// <summary>
                /// 删除日志
                /// </summary>
                public Delegate DeleteLog;
                /// <summary>
                /// 批量删除日志
                /// </summary>
                public Delegate DeleteLogs;
            }
            /// <summary>
            /// 关键字处理委托集合
            /// </summary>
            private static readonly Dictionary<Type, delegates> cache;
            /// <summary>
            /// 获取关键字处理委托
            /// </summary>
            /// <param name="type">关键字类型</param>
            /// <returns>关键字处理委托</returns>
            public static delegates Get(Type type)
            {
                delegates delegates;
                return cache.TryGetValue(type, out delegates) ? delegates : default(delegates);
            }
            static tableKey()
            {
                cache = dictionary.CreateOnly<Type, delegates>();
                cache.Add(typeof(DateTime), new delegates { DeSerialize = (deSerialize<DateTime>)table.deSerializeKey, DeleteLog = (Action<DateTime, unmanagedStream>)table.deleteLog, DeleteLogs = (Action<subArray<DateTime>, unmanagedStream>)table.deleteLog });
                cache.Add(typeof(Guid), new delegates { DeSerialize = (deSerialize<Guid>)table.deSerializeKey, DeleteLog = (Action<Guid, unmanagedStream>)table.deleteLog, DeleteLogs = (Action<subArray<Guid>, unmanagedStream>)table.deleteLog });
                cache.Add(typeof(int), new delegates { DeSerialize = (deSerialize<int>)table.deSerializeKey, DeleteLog = (Action<int, unmanagedStream>)table.deleteLog, DeleteLogs = (Action<subArray<int>, unmanagedStream>)table.deleteLog });
                cache.Add(typeof(uint), new delegates { DeSerialize = (deSerialize<uint>)table.deSerializeKey, DeleteLog = (Action<uint, unmanagedStream>)table.deleteLog, DeleteLogs = (Action<subArray<uint>, unmanagedStream>)table.deleteLog });
                cache.Add(typeof(long), new delegates { DeSerialize = (deSerialize<long>)table.deSerializeKey, DeleteLog = (Action<long, unmanagedStream>)table.deleteLog, DeleteLogs = (Action<subArray<long>, unmanagedStream>)table.deleteLog });
                cache.Add(typeof(ulong), new delegates { DeSerialize = (deSerialize<ulong>)table.deSerializeKey, DeleteLog = (Action<ulong, unmanagedStream>)table.deleteLog, DeleteLogs = (Action<subArray<ulong>, unmanagedStream>)table.deleteLog });
                cache.Add(typeof(string), new delegates { DeSerialize = (deSerialize<string>)table.deSerializeKey, DeleteLog = (Action<string, unmanagedStream>)table.deleteLog, DeleteLogs = (Action<subArray<string>, unmanagedStream>)table.deleteLog });
            }
        }
        /// <summary>
        /// 远程数据库表格(反射模式)
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="keyType">关键字类型</typeparam>
        public class remoteTable<valueType, keyType> : remoteTable<valueType, memberMap<valueType>, keyType>
            where valueType : class, fastCSharp.data.IPrimaryKey<keyType>
            where keyType : IEquatable<keyType>
        {
            /// <summary>
            /// 反序列化关键字委托
            /// </summary>
            private new static readonly tableKey.deSerialize<keyType> deSerializeKey;
            /// <summary>
            /// 删除对象日志委托
            /// </summary>
            private new static readonly Action<keyType, unmanagedStream> deleteLog;
            /// <summary>
            /// 批量删除对象日志委托
            /// </summary>
            private static readonly Action<subArray<keyType>, unmanagedStream> deleteLogs;
            /// <summary>
            /// 自增表基本缓存
            /// </summary>
            private fastCSharp.memoryDatabase.cache.ILoadIdentityCache<valueType, memberMap<valueType>, keyType> cache;
            /// <summary>
            /// 远程数据库表格
            /// </summary>
            /// <param name="client">数据库物理层服务客户端</param>
            /// <param name="cache">事件缓存</param>
            public remoteTable(fastCSharp.memoryDatabase.IPhysicalClient client
                , fastCSharp.memoryDatabase.cache.ILoadCache<valueType, memberMap<valueType>, keyType> cache)
                : base(client, cache) { }
            /// <summary>
            /// 远程数据库自增表格
            /// </summary>
            /// <param name="client">数据库物理层服务客户端</param>
            /// <param name="cache">事件缓存</param>
            public remoteTable(fastCSharp.memoryDatabase.IPhysicalClient client
                , fastCSharp.memoryDatabase.cache.ILoadIdentityCache<valueType, memberMap<valueType>, keyType> cache)
                : base(client, cache)
            {
                this.cache = cache;
            }
            /// <summary>
            /// 添加对象
            /// </summary>
            /// <param name="value">对象值</param>
            /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
            /// <returns>是否添加成功</returns>
            protected override valueType insert(valueType value, bool isCopy)
            {
                if (this.cache == null) return base.insert(value, isCopy);
                if (memoryDatabase.IsCheckIdentity && !value.PrimaryKey.Equals(default(keyType))) return null;
                return insert(value, isCopy, cache);
            }
            /// <summary>
            /// 添加对象
            /// </summary>
            /// <param name="onInserted">添加的对象回调委托,日志缓存是否添加失败返回null</param>
            /// <param name="value">对象值</param>
            /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
            protected override void insert(Action<valueType> onInserted, valueType value, bool isCopy)
            {
                if (this.cache == null) base.insert(onInserted, value, isCopy);
                else if (memoryDatabase.IsCheckIdentity && !value.PrimaryKey.Equals(default(keyType)))
                {
                    if (onInserted != null) onInserted(null);
                }
                else insert(onInserted, value, isCopy, cache);
            }
            /// <summary>
            /// 批量添加对象
            /// </summary>
            /// <param name="values">对象值</param>
            /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
            /// <returns>是否添加成功</returns>
            protected override valueType[] insert(valueType[] values, bool isCopy)
            {
                if (this.cache == null) return base.insert(values, isCopy);
                if (memoryDatabase.IsCheckIdentity)
                {
                    foreach (valueType value in values) if (!value.PrimaryKey.Equals(default(keyType))) return null;
                }
                return insert(values, isCopy, cache);
            }
            /// <summary>
            /// 批量添加对象
            /// </summary>
            /// <param name="onInserted">添加的对象回调委托,日志缓存是否添加失败返回null</param>
            /// <param name="values">对象值</param>
            /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
            protected override void insert(Action<valueType[]> onInserted, valueType[] values, bool isCopy)
            {
                if (this.cache == null)
                {
                    base.insert(onInserted, values, isCopy);
                    return;
                }
                try
                {
                    bool isValue = true;
                    if (memoryDatabase.IsCheckIdentity)
                    {
                        foreach (valueType value in values)
                        {
                            if (!value.PrimaryKey.Equals(default(keyType)))
                            {
                                isValue = false;
                                break;
                            }
                        }
                    }
                    if (isValue)
                    {
                        insert(onInserted, values, isCopy, cache);
                        return;
                    }
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
                if (onInserted != null) onInserted(null);
            }
            /// <summary>
            /// 删除对象
            /// </summary>
            /// <param name="key">关键字</param>
            /// <param name="serializeStream">序列化数据流</param>
            /// <returns>删除的对象,日志缓存是否添加失败返回null</returns>
            protected unsafe override valueType delete(keyType key, serializeStream serializeStream)
            {
                if (deleteLog == null) base.deleteLog(key, serializeStream.Stream);
                else deleteLog(key, serializeStream.Stream);
                return getDelete(key, serializeStream);
            }
            /// <summary>
            /// 删除对象
            /// </summary>
            /// <param name="onDeleted">删除对象回调委托,日志缓存是否添加失败返回null</param>
            /// <param name="key">关键字</param>
            /// <param name="serializeStream">序列化数据流</param>
            protected unsafe override void delete(Action<valueType> onDeleted, keyType key, serializeStream serializeStream)
            {
                try
                {
                    if (deleteLog == null) base.deleteLog(key, serializeStream.Stream);
                    else deleteLog(key, serializeStream.Stream);
                    getDelete(onDeleted, key, serializeStream);
                    return;
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
                if (onDeleted != null) onDeleted(null);
            }
            /// <summary>
            /// 批量删除对象
            /// </summary>
            /// <param name="serializeStream">序列化数据流</param>
            /// <param name="keys">关键字集合</param>
            /// <returns>删除的对象集合,日志缓存是否添加失败返回null</returns>
            protected unsafe override valueType[] delete(serializeStream serializeStream, subArray<keyType> keys)
            {
                if (deleteLog == null) return delete(serializeStream, keys, base.deleteLog(serializeStream.Stream, keys));
                deleteLogs(keys, serializeStream.Stream);
                return delete(serializeStream, keys, null);
            }
            /// <summary>
            /// 批量删除对象
            /// </summary>
            /// <param name="onDeleted">删除对象回调委托,日志缓存是否添加失败返回null</param>
            /// <param name="serializeStream">序列化数据流</param>
            /// <param name="keys">关键字集合</param>
            protected unsafe override void delete(Action<valueType[]> onDeleted, serializeStream serializeStream, subArray<keyType> keys)
            {
                try
                {
                    if (deleteLog == null) delete(onDeleted, serializeStream, keys, base.deleteLog(serializeStream.Stream, keys));
                    else
                    {
                        deleteLogs(keys, serializeStream.Stream);
                        delete(onDeleted, serializeStream, keys, null);
                    }
                    return;
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
                if (onDeleted != null) onDeleted(null);
            }
            /// <summary>
            /// 新建对象数据
            /// </summary>
            /// <returns>对象数据</returns>
            protected override valueType newValue()
            {
                return fastCSharp.emit.constructor<valueType>.New();
            }
            /// <summary>
            /// 反序列化关键字
            /// </summary>
            /// <param name="data">数据</param>
            /// <param name="key">关键字</param>
            /// <returns>是否成功</returns>
            protected override bool deSerialize(subArray<byte> data, out keyType key)
            {
                return deSerializeKey != null ? deSerializeKey(data, out key) : base.deSerializeKey(data, out key);
            }
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="memoryStream">内存数据流</param>
            /// <param name="value">对象值</param>
            /// <param name="memberMap">成员位图</param>
            protected override void serialize(unmanagedStream memoryStream, valueType value, memberMap<valueType> memberMap)
            {
                if (memoryDatabase.IsIndexSerialize)
                {
                    indexSerialize.dataSerialize.Get<valueType>(memoryStream, value, memoryDatabase.MemberFilter, memberMap);
                }
                else
                {
                    fastCSharp.code.cSharp.serialize.dataSerialize.Get<valueType>(memoryStream, value, memoryDatabase.MemberFilter, memberMap);
                }
            }
            /// <summary>
            /// 对象反序列化
            /// </summary>
            /// <param name="value">目标对象</param>
            /// <param name="data">序列化数据</param>
            /// <param name="startIndex">起始位置</param>
            /// <param name="endIndex">结束位置</param>
            /// <param name="memberMap">成员位图</param>
            /// <returns>是否成功</returns>
            protected override bool deSerialize(valueType value, subArray<byte> data, out int endIndex, out memberMap<valueType> memberMap)
            {
                if (memoryDatabase.IsIndexSerialize) return indexSerialize.deSerialize.Get(ref value, data, out endIndex, out memberMap, indexSerializeMemberIndexs);
                return code.cSharp.serialize.deSerialize.Get(ref value, data.Array, data.StartIndex, out endIndex, out memberMap);
            }
            static remoteTable()
            {
                tableKey.delegates delegates = tableKey.Get(typeof(keyType));
                if (delegates.DeSerialize != null)
                {
                    deSerializeKey = (tableKey.deSerialize<keyType>)delegates.DeSerialize;
                    deleteLog = (Action<keyType, unmanagedStream>)delegates.DeleteLog;
                    deleteLogs = (Action<subArray<keyType>, unmanagedStream>)delegates.DeleteLogs;
                }
            }
        }
        /// <summary>
        /// 本地数据库表格(反射模式)
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="keyType">关键字类型</typeparam>
        public class localTable<valueType, keyType> : localTable<valueType, memberMap<valueType>, keyType>
            where valueType : class, fastCSharp.data.IPrimaryKey<keyType>
            where keyType : IEquatable<keyType>
        {
            /// <summary>
            /// 反序列化关键字委托
            /// </summary>
            private new static readonly tableKey.deSerialize<keyType> deSerializeKey;
            /// <summary>
            /// 删除对象日志委托
            /// </summary>
            private new static readonly Action<keyType, unmanagedStream> deleteLog;
            /// <summary>
            /// 批量删除对象日志委托
            /// </summary>
            private static readonly Action<subArray<keyType>, unmanagedStream> deleteLogs;
            /// <summary>
            /// 自增表基本缓存
            /// </summary>
            private fastCSharp.memoryDatabase.cache.ILoadIdentityCache<valueType, memberMap<valueType>, keyType> cache;
            /// <summary>
            /// 本地数据库表格
            /// </summary>
            /// <param name="client">数据库物理层服务客户端</param>
            /// <param name="cache">事件缓存</param>
            public localTable(fastCSharp.memoryDatabase.cache.ILoadCache<valueType, memberMap<valueType>, keyType> cache)
                : base(cache) { }
            /// <summary>
            /// 本地数据库自增表格
            /// </summary>
            /// <param name="client">数据库物理层服务客户端</param>
            /// <param name="cache">事件缓存</param>
            public localTable(fastCSharp.memoryDatabase.cache.ILoadIdentityCache<valueType, memberMap<valueType>, keyType> cache)
                : base(cache)
            {
                this.cache = cache;
            }
            /// <summary>
            /// 添加对象
            /// </summary>
            /// <param name="memoryStream">内存日志流</param>
            /// <param name="value">对象值</param>
            /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
            /// <returns>是否添加成功</returns>
            protected override valueType insert(valueType value, bool isCopy)
            {
                if (this.cache == null) return base.insert(value, isCopy);
                if (memoryDatabase.IsCheckIdentity && !value.PrimaryKey.Equals(default(keyType))) return null;
                return insert(value, isCopy, cache);
            }
            /// <summary>
            /// 添加对象
            /// </summary>
            /// <param name="onInserted">添加的对象回调委托,日志缓存是否添加失败返回null</param>
            /// <param name="value">对象值</param>
            /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
            protected override void insert(Action<valueType> onInserted, valueType value, bool isCopy)
            {
                if (this.cache == null) base.insert(onInserted, value, isCopy);
                else if (memoryDatabase.IsCheckIdentity && !value.PrimaryKey.Equals(default(keyType)))
                {
                    if (onInserted != null) onInserted(null);
                }
                else insert(onInserted, value, isCopy, cache);
            }
            /// <summary>
            /// 批量添加对象
            /// </summary>
            /// <param name="values">对象值</param>
            /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
            /// <returns>是否添加成功</returns>
            protected override valueType[] insert(valueType[] values, bool isCopy)
            {
                if (this.cache == null) return base.insert(values, isCopy);
                if (memoryDatabase.IsCheckIdentity)
                {
                    foreach (valueType value in values) if (!value.PrimaryKey.Equals(default(keyType))) return null;
                }
                return insert(values, isCopy, cache);
            }
            /// <summary>
            /// 批量添加对象
            /// </summary>
            /// <param name="onInserted">添加的对象回调委托,日志缓存是否添加失败返回null</param>
            /// <param name="values">对象值</param>
            /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
            protected override void insert(Action<valueType[]> onInserted, valueType[] values, bool isCopy)
            {
                if (this.cache == null)
                {
                    base.insert(onInserted, values, isCopy);
                    return;
                }
                try
                {
                    bool isValue = true;
                    if (memoryDatabase.IsCheckIdentity)
                    {
                        foreach (valueType value in values)
                        {
                            if (!value.PrimaryKey.Equals(default(keyType)))
                            {
                                isValue = false;
                                break;
                            }
                        }
                    }
                    if (isValue)
                    {
                        insert(onInserted, values, isCopy, cache);
                        return;
                    }
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
                if (onInserted != null) onInserted(null);
            }
            /// <summary>
            /// 删除对象
            /// </summary>
            /// <param name="key">关键字</param>
            /// <param name="serializeStream">序列化数据流</param>
            /// <returns>删除的对象,日志缓存是否添加失败返回null</returns>
            protected unsafe override valueType delete(keyType key, serializeStream serializeStream)
            {
                if (deleteLog == null) base.deleteLog(key, serializeStream.Stream);
                else deleteLog(key, serializeStream.Stream);
                return getDelete(key, serializeStream);
            }
            /// <summary>
            /// 删除对象
            /// </summary>
            /// <param name="onDeleted">删除对象回调委托,日志缓存是否添加失败返回null</param>
            /// <param name="key">关键字</param>
            /// <param name="serializeStream">序列化数据流</param>
            protected unsafe override void delete(Action<valueType> onDeleted, keyType key, serializeStream serializeStream)
            {
                try
                {
                    if (deleteLog == null) base.deleteLog(key, serializeStream.Stream);
                    else deleteLog(key, serializeStream.Stream);
                    getDelete(onDeleted, key, serializeStream);
                    return;
                }
                catch (Exception error)
                {
                    fastCSharp.log.Default.Add(error, null, false);
                }
                if (onDeleted != null) onDeleted(null);
            }
            /// <summary>
            /// 批量删除对象
            /// </summary>
            /// <param name="memoryStream">日志内存流</param>
            /// <param name="keys">关键字集合</param>
            /// <returns>删除的对象集合,日志缓存是否添加失败返回null</returns>
            protected unsafe override valueType[] delete(serializeStream serializeStream, subArray<keyType> keys)
            {
                if (deleteLog == null) return delete(serializeStream, keys, base.deleteLog(serializeStream.Stream, keys));
                deleteLogs(keys, serializeStream.Stream);
                return delete(serializeStream, keys, null);
            }
            /// <summary>
            /// 批量删除对象
            /// </summary>
            /// <param name="onDeleted">删除对象回调委托,日志缓存是否添加失败返回null</param>
            /// <param name="serializeStream">序列化数据流</param>
            /// <param name="keys">关键字集合</param>
            protected unsafe override void delete(Action<valueType[]> onDeleted, serializeStream serializeStream, subArray<keyType> keys)
            {
                try
                {
                    if (deleteLog == null) delete(onDeleted, serializeStream, keys, base.deleteLog(serializeStream.Stream, keys));
                    else
                    {
                        deleteLogs(keys, serializeStream.Stream);
                        delete(onDeleted, serializeStream, keys, null);
                    }
                    return;
                }
                catch (Exception error)
                {
                    fastCSharp.log.Error.Add(error, null, false);
                }
                if (onDeleted != null) onDeleted(null);
            }
            /// <summary>
            /// 新建对象数据
            /// </summary>
            /// <returns>对象数据</returns>
            protected override valueType newValue()
            {
                return fastCSharp.emit.constructor<valueType>.New();
            }
            /// <summary>
            /// 反序列化关键字
            /// </summary>
            /// <param name="data">数据</param>
            /// <param name="key">关键字</param>
            /// <returns>是否成功</returns>
            protected override bool deSerialize(subArray<byte> data, out keyType key)
            {
                return deSerializeKey != null ? deSerializeKey(data, out key) : base.deSerializeKey(data, out key);
            }
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="memoryStream">内存数据流</param>
            /// <param name="value">对象值</param>
            /// <param name="memberMap">成员位图</param>
            protected override void serialize(unmanagedStream memoryStream, valueType value, memberMap<valueType> memberMap)
            {
                if (memoryDatabase.IsIndexSerialize)
                {
                    indexSerialize.dataSerialize.Get<valueType>(memoryStream, value, memoryDatabase.MemberFilter, memberMap);
                }
                else
                {
                    fastCSharp.code.cSharp.serialize.dataSerialize.Get<valueType>(memoryStream, value, memoryDatabase.MemberFilter, memberMap);
                }
            }
            /// <summary>
            /// 对象反序列化
            /// </summary>
            /// <param name="value">目标对象</param>
            /// <param name="data">序列化数据</param>
            /// <param name="startIndex">起始位置</param>
            /// <param name="endIndex">结束位置</param>
            /// <param name="memberMap">成员位图</param>
            /// <returns>是否成功</returns>
            protected override bool deSerialize(valueType value, subArray<byte> data, out int endIndex, out memberMap<valueType> memberMap)
            {
                if (memoryDatabase.IsIndexSerialize) return indexSerialize.deSerialize.Get(ref value, data, out endIndex, out memberMap, indexSerializeMemberIndexs);
                return code.cSharp.serialize.deSerialize.Get(ref value, data.Array, data.StartIndex, out endIndex, out memberMap);
            }
            static localTable()
            {
                tableKey.delegates delegates = tableKey.Get(typeof(keyType));
                if (delegates.DeSerialize != null)
                {
                    deSerializeKey = (tableKey.deSerialize<keyType>)delegates.DeSerialize;
                    deleteLog = (Action<keyType, unmanagedStream>)delegates.DeleteLog;
                    deleteLogs = (Action<subArray<keyType>, unmanagedStream>)delegates.DeleteLogs;
                }
            }
        }
        /// <summary>
        /// 本地数据库表格实例(反射模式)
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="keyType">关键字类型</typeparam>
        public abstract class identityArrayLocalTable<valueType>
            where valueType : class, fastCSharp.data.IPrimaryKey<int>
        {
            /// <summary>
            /// 内存数据库
            /// </summary>
            private static localTable<valueType, int> memoryDatabase;
            /// <summary>
            /// 内存数据库
            /// </summary>
            public static localTable<valueType, int> MemoryDatabase
            {
                get
                {
                    if (memoryDatabase == null)
                    {
                        memoryDatabase = table.Open(new localTable<valueType, int>(new identityArray<valueType>()));
                        fastCSharp.domainUnload.Add(memoryDatabase.Dispose);
                    }
                    return memoryDatabase;
                }
            }
            /// <summary>
            /// 内存数据库事件缓存
            /// </summary>
            private static identityArray<valueType> memoryDatabaseCache;
            /// <summary>
            /// 内存数据库事件缓存
            /// </summary>
            public static identityArray<valueType> MemoryDatabaseCache
            {
                get
                {
                    if (memoryDatabaseCache == null) memoryDatabaseCache = (identityArray<valueType>)MemoryDatabase.Cache;
                    return memoryDatabaseCache;
                }
            }
        }
        /// <summary>
        /// 本地数据库表格实例(反射模式)
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="keyType">关键字类型</typeparam>
        public abstract class dictionaryLocalTable<valueType, keyType>
            where valueType : class, fastCSharp.data.IPrimaryKey<keyType>
            where keyType : IEquatable<keyType>
        {
            /// <summary>
            /// 内存数据库
            /// </summary>
            private static localTable<valueType, keyType> memoryDatabase;
            /// <summary>
            /// 内存数据库
            /// </summary>
            public static localTable<valueType, keyType> MemoryDatabase
            {
                get
                {
                    if (memoryDatabase == null)
                    {
                        memoryDatabase = table.Open(new localTable<valueType, keyType>(new dictionary<valueType, keyType>()));
                        fastCSharp.domainUnload.Add(memoryDatabase.Dispose);
                    }
                    return memoryDatabase;
                }
            }
            /// <summary>
            /// 内存数据库事件缓存
            /// </summary>
            private static dictionary<valueType, keyType> memoryDatabaseCache;
            /// <summary>
            /// 内存数据库事件缓存
            /// </summary>
            public static dictionary<valueType, keyType> MemoryDatabaseCache
            {
                get
                {
                    if (memoryDatabaseCache == null) memoryDatabaseCache = (dictionary<valueType, keyType>)MemoryDatabase.Cache;
                    return memoryDatabaseCache;
                }
            }
        }
        /// <summary>
        /// 数据库文件名
        /// </summary>
        public string FileName;
        /// <summary>
        /// 内存数据库表格类型
        /// </summary>
        public string TableTypeName
        {
            get
            {
                return typeof(table).Namespace + ".memoryDatabase." + ((memberType)(IsEmbed ? typeof(localTable<,,>) : typeof(remoteTable<,,>))).TypeOnlyName;
            }
        }
        /// <summary>
        /// 客户端获取接口类型,必须继承fastCSharp.memoryDatabase.IPhysicalClient,IsEmbed为false时有效
        /// </summary>
        public Type ClientType;
        /// <summary>
        /// 基本缓存类型,必须实现fastCSharp.memoryDatabase.cache.IEventCache[valueType, memberType, keyType]
        /// </summary>
        public Type CacheType;
        /// <summary>
        /// 数据库日志文件最小刷新尺寸(单位:KB)
        /// </summary>
        public int MinRefreshSize = fastCSharp.config.memoryDatabase.Default.MinRefreshSize;
        /// <summary>
        /// 是否异步加载文件数据
        /// </summary>
        public bool IsAsynchronousLoad;
        /// <summary>
        /// 是否嵌入式(本地模式)
        /// </summary>
        public bool IsEmbed;
        /// <summary>
        /// 是否使用成员索引标识序列化,否则使用二进制序列化(用于稳定数据)
        /// </summary>
        public bool IsIndexSerialize = true;
        /// <summary>
        /// 是否检测添加对象的自增值
        /// </summary>
        public bool IsCheckIdentity;
    }
}
