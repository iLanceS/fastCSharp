using System;
using fastCSharp.net;
using fastCSharp.reflection;
using fastCSharp.net.tcp;
using System.Threading;
using fastCSharp.threading;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.IO;
using System.Reflection;

namespace fastCSharp.sql
{
    /// <summary>
    /// 日志
    /// </summary>
    public abstract class logStream
    {
        /// <summary>
        /// 日志类型
        /// </summary>
        public enum type : byte
        {
            /// <summary>
            /// 添加数据
            /// </summary>
            Insert,
            /// <summary>
            /// 更新数据
            /// </summary>
            Update,
            /// <summary>
            /// 删除数据
            /// </summary>
            Delete
        }
        /// <summary>
        /// 成员加载计数器
        /// </summary>
        public sealed class memberCount
        {
            /// <summary>
            /// 成员数量
            /// </summary>
            private int count;
            /// <summary>
            /// 成员访问锁
            /// </summary>
            private int memberLock;
            /// <summary>
            /// 日志
            /// </summary>
            private logStream logStream;
            /// <summary>
            /// 成员位图
            /// </summary>
            private fastCSharp.bitMap map;
            /// <summary>
            /// 成员加载计数器
            /// </summary>
            /// <param name="memberIndexs"></param>
            public memberCount(params int[] memberIndexs)
            {
                int maxIndex = memberIndexs.max(0) + 1;
                map = new bitMap(maxIndex);
                foreach (int index in memberIndexs)
                {
                    if (index != -1)
                    {
                        map.Set(index);
                        ++count;
                    }
                }
            }
            /// <summary>
            /// 加载成员处理
            /// </summary>
            /// <param name="memberIndex"></param>
            public void Load(int memberIndex)
            {
                interlocked.CompareSetYield(ref memberLock);
                if (map.Get(memberIndex))
                {
                    map.Clear(memberIndex);
                    if (--count == 0) logStream.isLoaded = true;
                }
                memberLock = 0;
            }
            /// <summary>
            /// 数据加载事件
            /// </summary>
            /// <param name="logStream">日志</param>
            internal void OnLoad(logStream logStream)
            {
                if (count == 0) logStream.isLoaded = true;
                else
                {
                    interlocked.CompareSetYield(ref memberLock);
                    if (count == 0) logStream.isLoaded = true;
                    else this.logStream = logStream;
                    memberLock = 0;
                }
            }
        }
        /// <summary>
        /// 日志流客户端集合
        /// </summary>
        /// <typeparam name="clientType">客户端类型</typeparam>
        public abstract class client<clientType> where clientType : client<clientType>
        {
            /// <summary>
            /// 保存缓存访问锁
            /// </summary>
            private static int saveLock;
            /// <summary>
            /// 保存缓存
            /// </summary>
            public static void CheckSave()
            {
                if (Interlocked.CompareExchange(ref saveLock, 0, 1) == 0)
                {
                    try
                    {
                        foreach (FieldInfo field in typeof(clientType).GetFields(BindingFlags.Public | BindingFlags.Static))
                        {
                            if (typeof(fastCSharp.sql.logStream.client).IsAssignableFrom(field.FieldType))
                            {
                                fastCSharp.sql.logStream.client client = (fastCSharp.sql.logStream.client)field.GetValue(null);
                                if (client != null) client.CheckSave();
                            }
                        }
                    }
                    finally { saveLock = 0; }
                }
            }
            ///// <summary>
            ///// 等待缓存加载
            ///// </summary>
            //protected static void wait()
            //{
            //    foreach (FieldInfo field in typeof(clientType).GetFields(BindingFlags.Public | BindingFlags.Static))
            //    {
            //        if (typeof(fastCSharp.sql.logStream.client).IsAssignableFrom(field.FieldType))
            //        {
            //            do
            //            {
            //                fastCSharp.sql.logStream.client client = (fastCSharp.sql.logStream.client)field.GetValue(null);
            //                if (client == null) break;
            //                if (client.IsNull) Thread.Sleep(1);
            //                else client.Wait();
            //            }
            //            while (true);
            //        }
            //    }
            //}

            /// <summary>
            /// 服务器端加载缓存数据集合
            /// </summary>
            protected static subArray<object> loadTables = new subArray<object>();
            /// <summary>
            /// 服务器端加载缓存数据
            /// </summary>
            /// <typeparam name="tableType"></typeparam>
            protected static void load<tableType>()
            {
                loadTables.Add(fastCSharp.emit.constructor<tableType>.New());
            }
        }
        /// <summary>
        /// 日志流客户端
        /// </summary>
        public abstract class client
        {
            /// <summary>
            /// 单次读取文件长度
            /// </summary>
            protected const int readFileSize = 64 << 10;
            /// <summary>
            /// 日志流处理保持回调
            /// </summary>
            protected commandClient.streamCommandSocket.keepCallback keepCallback;
            /// <summary>
            /// 保存文件名称
            /// </summary>
            protected string cacheFileName;
            /// <summary>
            /// 输出错误信息时间
            /// </summary>
            private DateTime errorTime;
            /// <summary>
            /// 是否输出错误信息
            /// </summary>
            protected bool isErrorTime
            {
                get
                {
                    if (errorTime <= date.NowSecond)
                    {
                        errorTime = date.NowSecond.AddMinutes(1);
                        return true;
                    }
                    return false;
                }
            }
            /// <summary>
            /// 数据加载访问锁
            /// </summary>
            protected int loadLock;
            /// <summary>
            /// 是否已经初步加载日志数据
            /// </summary>
            protected int isLog;
            /// <summary>
            /// 保存文件调用次数
            /// </summary>
            protected int saveCount;
            /// <summary>
            /// 保存文件名称是否采用JSON格式，否则而二进制序列化
            /// </summary>
            protected readonly bool isJsonCacheFile;
            /// <summary>
            /// 是否检测输出成员位图错误信息
            /// </summary>
            protected byte isErrorMemberMap;
            /// <summary>
            /// 是否虚拟
            /// </summary>
            protected bool isNull;
            /// <summary>
            /// 是否存在数据
            /// </summary>
            public bool IsData { get; protected set; }
            /// <summary>
            /// 虚拟空日志流客户端
            /// </summary>
            protected client()
            {
                isNull = true;
            }
            /// <summary>
            /// 日志流客户端
            /// </summary>
            /// <param name="isJsonCacheFile"></param>
            protected client(bool isJsonCacheFile)
            {
                this.isJsonCacheFile = isJsonCacheFile;
            }
            /// <summary>
            /// 保存文件
            /// </summary>
            public abstract void CheckSave();
            ///// <summary>
            ///// 等待加载数据
            ///// </summary>
            //public void Wait()
            //{
            //    while (!IsData) Thread.Sleep(1);
            //}
        }
        /// <summary>
        /// 是否加载完毕
        /// </summary>
        protected bool isLoaded;
        /// <summary>
        /// 创建数据更新成员位图
        /// </summary>
        /// <typeparam name="modelType"></typeparam>
        /// <typeparam name="memberType"></typeparam>
        /// <param name="sqlTable"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public static fastCSharp.code.memberMap<modelType> CreateMemberMap<modelType, memberType>(fastCSharp.emit.sqlTable.sqlTool<modelType> sqlTable, Expression<Func<modelType, memberType>> member)
            where modelType : class
        {
            if (sqlTable == null || member == null) return null;
            fastCSharp.code.memberMap<modelType> memberMap = sqlTable.CreateMemberMap().Append(member);
            fastCSharp.emit.sqlModel<modelType>.SetIdentityOrPrimaryKeyMemberMap(memberMap);
            return memberMap;
        }
    }
    /// <summary>
    /// 日志
    /// </summary>
    /// <typeparam name="valueType"></typeparam>
    /// <typeparam name="modelType"></typeparam>
    public class logStream<valueType, modelType> : logStream
        where valueType : class, modelType
        where modelType : class
    {
        /// <summary>
        /// 日志数据
        /// </summary>
        [fastCSharp.emit.dataSerialize(IsReferenceMember = false, IsMemberMap = false)]
        public struct data
        {
            /// <summary>
            /// 数据库更新记录
            /// </summary>
            public fastCSharp.emit.memberMapValue<modelType, valueType>.deSerializePool Value;
            /// <summary>
            /// 日志类型
            /// </summary>
            public type Type;
        }
        /// <summary>
        /// 缓存标识信息
        /// </summary>
        [fastCSharp.emit.dataSerialize(IsReferenceMember = false, IsMemberMap = false)]
        public struct cacheIdentity
        {
            /// <summary>
            /// 时钟周期标识
            /// </summary>
            public long Ticks;
            /// <summary>
            /// 缓存数据集合
            /// </summary>
            public valueType[] Values;
            /// <summary>
            /// 日志流编号
            /// </summary>
            public int Identity;
        }
        /// <summary>
        /// 客户端自定义绑定
        /// </summary>
        public class clientCustom
        {
            /// <summary>
            /// 重新加载数据
            /// </summary>
            /// <param name="values"></param>
            public virtual void Load(valueType[] values) { }
            /// <summary>
            /// 添加数据
            /// </summary>
            /// <param name="value"></param>
            public virtual void Insert(valueType value) { }
            /// <summary>
            /// 更新数据
            /// </summary>
            /// <param name="value"></param>
            public virtual void Update(valueType value) { }
            /// <summary>
            /// 删除数据
            /// </summary>
            /// <param name="value"></param>
            public virtual void Delete(valueType value) { }
            /// <summary>
            /// 虚拟客户端自定义绑定
            /// </summary>
            internal static readonly clientCustom Null = new clientCustom();
        }
        /// <summary>
        /// 日志流客户端
        /// </summary>
        public abstract new class client : logStream.client
        {
            /// <summary>
            /// 虚拟空日志流客户端
            /// </summary>
            protected client() : base() { }
            /// <summary>
            /// 日志流客户端
            /// </summary>
            /// <param name="isJsonCacheFile"></param>
            protected client(bool isJsonCacheFile) : base(isJsonCacheFile) { }
            /// <summary>
            /// 输出错误信息
            /// </summary>
            protected void error()
            {
                if (isErrorTime) fastCSharp.log.Error.Add(@"日志流客户端初始化失败 " + typeof(valueType).fullName(), null, false);
            }
        }
        /// <summary>
        /// 日志流客户端
        /// </summary>
        /// <typeparam name="keyType"></typeparam>
        public abstract new class client<keyType> : client where keyType : IEquatable<keyType>
        {
            /// <summary>
            /// 获取缓存数据委托
            /// </summary>
            protected readonly Action<Action<fastCSharp.net.returnValue<cacheIdentity>>> getCache;
            /// <summary>
            /// 获取日志数据委托
            /// </summary>
            protected readonly Func<long, int, Action<fastCSharp.net.returnValue<data>>, commandClient.streamCommandSocket.keepCallback> getLog;
            /// <summary>
            /// 获取关键字委托
            /// </summary>
            protected readonly Func<modelType, keyType> getKey;
            /// <summary>
            /// 获取数据委托
            /// </summary>
            protected readonly Func<keyType, fastCSharp.net.returnValue<valueType>> getValue;
            /// <summary>
            /// 临时数据
            /// </summary>
            private Dictionary<randomKey<keyType>, keyValue<valueType, EventWaitHandle>> tempValues;
            /// <summary>
            /// 客户端自定义绑定
            /// </summary>
            protected clientCustom clientCustom;
            /// <summary>
            /// 临时数据访问锁
            /// </summary>
            private readonly object tempValueLock;
            /// <summary>
            /// 虚拟空日志流客户端
            /// </summary>
            protected client() { }

            /// <summary>
            /// 日志流客户端
            /// </summary>
            /// <param name="getCache">获取缓存数据委托</param>
            /// <param name="getLog">获取日志数据委托</param>
            /// <param name="getKey"></param>
            /// <param name="getValue">获取数据委托</param>
            /// <param name="cachePath">缓存文件保存路径</param>
            /// <param name="cacheFileName">缓存文件名称</param>
            /// <param name="isJsonCacheFile">缓存文件是否 JSON 序列化，否则采用二进制序列化</param>
            /// <param name="clientCustom">客户端自定义绑定</param>
            protected client(Action<Action<fastCSharp.net.returnValue<cacheIdentity>>> getCache
                , Func<long, int, Action<fastCSharp.net.returnValue<data>>, commandClient.streamCommandSocket.keepCallback> getLog
                , Func<modelType, keyType> getKey, Func<keyType, fastCSharp.net.returnValue<valueType>> getValue
                , string cachePath, string cacheFileName, bool isJsonCacheFile, clientCustom clientCustom)
                : base(isJsonCacheFile)
            {
                if (getCache == null || getLog == null || getKey == null || getValue == null) log.Error.Throw(log.exceptionType.Null);
                this.getCache = getCache;
                this.getLog = getLog;
                this.getKey = getKey;
                this.getValue = getValue;
                tempValueLock = new object();
                this.clientCustom = clientCustom ?? clientCustom.Null;
                if (cachePath != null) this.cacheFileName = cachePath + (cacheFileName == null ? typeof(valueType).Name : cacheFileName);
            }
            /// <summary>
            /// 保存文件
            /// </summary>
            /// <param name="value"></param>
            protected unsafe void save(valueType[] value)
            {
                FileInfo file = new FileInfo(cacheFileName);
                DirectoryInfo directory = file.Directory;
                if (!directory.Exists) directory.Create();
                byte[] data;
                if (isJsonCacheFile)
                {
                    string json = value.ToJson();
                    data = new byte[json.Length << 1];
                    fixed (byte* dataFixed = data) fastCSharp.unsafer.String.Copy(json, dataFixed);
                }
                else data = fastCSharp.emit.dataSerializer.Serialize(value);
                subArray<byte> compressionData = fastCSharp.io.compression.stream.Deflate.GetCompress(data);
                using (FileStream fileStream = new FileStream(cacheFileName, FileMode.Create)) fileStream.Write(compressionData.UnsafeArray, compressionData.StartIndex, compressionData.Count);
            }
            /// <summary>
            /// 从文件读取缓存
            /// </summary>
            protected unsafe void readCacheFile()
            {
                if (isLog != 0) return;
                FileInfo file = new FileInfo(cacheFileName);
                if (!file.Exists || isLog != 0) return;
                using (FileStream fileStream = file.OpenRead())
                {
                    byte[] fileData = new byte[fileStream.Length];
                    int index = 0, length = fileData.Length & (int.MaxValue - (readFileSize - 1));
                    while (index != length)
                    {
                        if (isLog != 0) return;
                        fileStream.Read(fileData, index, readFileSize);
                        index += readFileSize;
                    }
                    if ((length = fileData.Length & (readFileSize - 1)) != 0)
                    {
                        if (isLog != 0) return;
                        fileStream.Read(fileData, index, length);
                    }
                    if (isLog != 0) return;
                    subArray<byte> data = fastCSharp.io.compression.stream.Deflate.GetDeCompress(fileData);
                    if (isLog != 0) return;
                    if (data.Count != 0)
                    {
                        if (isJsonCacheFile)
                        {
                            valueType[] values = null;
                            fixed (byte* dataFixed = data.UnsafeArray)
                            {
                                if (fastCSharp.emit.jsonParser.UnsafeParse<valueType[]>((char*)dataFixed, data.Count >> 1, ref values) && values != null)
                                {
                                    setFile(values);
                                    return;
                                }
                            }
                        }
                        else
                        {
                            valueType[] values = fastCSharp.emit.dataDeSerializer.DeSerialize<valueType[]>(ref data);
                            if (values != null)
                            {
                                setFile(values);
                                return;
                            }
                        }
                    }
                    fastCSharp.log.Error.Add("日志流客户端缓存恢复失败 " + typeof(valueType).fullName());
                }
            }
            /// <summary>
            /// 设置缓存
            /// </summary>
            /// <param name="values"></param>
            protected abstract void setFile(valueType[] values);
            /// <summary>
            /// 清除临时数据
            /// </summary>
            protected void clearTemp()
            {
                Monitor.Enter(tempValueLock);
                tempValues = null;
                Monitor.Exit(tempValueLock);
            }
            /// <summary>
            /// 获取数据
            /// </summary>
            /// <param name="data"></param>
            protected void load(fastCSharp.net.returnValue<cacheIdentity> data)
            {
                if (data.Type == fastCSharp.net.returnValue.type.Success && data.Value.Values != null)
                {
                    Interlocked.CompareExchange(ref isLog, 1, 0);
                    fastCSharp.threading.threadPool.TinyPool.Start(loaded, ref data);
                    return;
                }
                fastCSharp.threading.timerTask.Default.Add(getCache, load, date.NowSecond.AddSeconds(5), null);
                error();
            }
            /// <summary>
            /// 加载数据
            /// </summary>
            /// <param name="data"></param>
            protected abstract void loaded(fastCSharp.net.returnValue<cacheIdentity> data);
            /// <summary>
            /// 检测成员位图
            /// </summary>
            /// <param name="data"></param>
            protected void checkMemberMap(ref fastCSharp.net.returnValue<data> data)
            {
                if (data.Value.Value.MemberMap == null && isErrorMemberMap == 0)
                {
                    isErrorMemberMap = 1;
                    log.Error.Add("客户端缓存数据缺少成员位图信息 " + typeof(valueType).fullName(), new System.Diagnostics.StackFrame(), false);
                }
            }
            /// <summary>
            /// 获取临时数据
            /// </summary>
            /// <param name="key"></param>
            /// <param name="valueWait"></param>
            /// <returns></returns>
            protected bool get(keyType key, ref keyValue<valueType, EventWaitHandle> valueWait)
            {
                Monitor.Enter(tempValueLock);
                if (tempValues == null)
                {
                    try
                    {
                        tempValues = dictionary.Create<randomKey<keyType>, keyValue<valueType, EventWaitHandle>>();
                        valueWait.Value = new EventWaitHandle(false, EventResetMode.ManualReset);
                        tempValues.Add(key, valueWait);
                    }
                    finally { Monitor.Exit(tempValueLock); }
                    return true;
                }
                if (tempValues.TryGetValue(key, out valueWait))
                {
                    Monitor.Exit(tempValueLock);
                    return false;
                }
                try
                {
                    valueWait.Value = new EventWaitHandle(false, EventResetMode.ManualReset);
                    tempValues.Add(key, valueWait);
                }
                finally { Monitor.Exit(tempValueLock); }
                return true;
            }
            /// <summary>
            /// 设置临时数据
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            protected void set(keyType key, valueType value)
            {
                Monitor.Enter(tempValueLock);
                try
                {
                    if (tempValues != null) tempValues[key] = new keyValue<valueType, EventWaitHandle>(value, null);
                }
                finally { Monitor.Exit(tempValueLock); }
            }
            /// <summary>
            /// 获取临时数据
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            protected valueType get(keyType key)
            {
                keyValue<valueType, EventWaitHandle> value;
                Monitor.Enter(tempValueLock);
                if (tempValues != null && tempValues.TryGetValue(key, out value))
                {
                    Monitor.Exit(tempValueLock);
                    return value.Key;
                }
                Monitor.Exit(tempValueLock);
                return null;
            }
        }
        /// <summary>
        /// 自增标识客户端
        /// </summary>
        public sealed class identityClient : client<int>
        {
            /// <summary>
            /// 虚拟空日志流客户端
            /// </summary>
            public static readonly identityClient Null = new identityClient();
            /// <summary>
            /// 缓存数据
            /// </summary>
            private identityArray<valueType> values;
            /// <summary>
            /// 最大自增标识
            /// </summary>
            private int maxIdentity;
            /// <summary>
            /// 数据数量
            /// </summary>
            public int Count { get; private set; }
            /// <summary>
            /// 虚拟空日志流客户端
            /// </summary>
            private identityClient() { }
            /// <summary>
            /// 日志流客户端
            /// </summary>
            /// <param name="getCache">获取缓存数据委托</param>
            /// <param name="getLog">获取日志数据委托</param>
            /// <param name="getValue">获取数据委托</param>
            /// <param name="cachePath">缓存文件保存路径</param>
            /// <param name="cacheFileName">缓存文件名称</param>
            /// <param name="isJsonCacheFile">缓存文件是否 JSON 序列化，否则采用二进制序列化</param>
            /// <param name="clientCustom">客户端自定义绑定</param>
            public identityClient(Action<Action<fastCSharp.net.returnValue<cacheIdentity>>> getCache
                , Func<long, int, Action<fastCSharp.net.returnValue<data>>, commandClient.streamCommandSocket.keepCallback> getLog
                , Func<int, fastCSharp.net.returnValue<valueType>> getValue
                , string cachePath = null, string cacheFileName = null, bool isJsonCacheFile = true, clientCustom clientCustom = null)
                : base(getCache, getLog, fastCSharp.emit.sqlModel<modelType>.GetIdentity32, getValue, cachePath, cacheFileName, isJsonCacheFile, clientCustom)
            {
                getCache(load);
                if (this.cacheFileName != null) fastCSharp.threading.threadPool.TinyPool.Start(readCacheFile);
            }
            /// <summary>
            /// 日志流客户端
            /// </summary>
            /// <param name="tcpCallType">TCP 调用类型</param>
            /// <param name="cachePath">缓存文件保存路径</param>
            /// <param name="cacheFileName">缓存文件名称</param>
            /// <param name="isJsonCacheFile">缓存文件是否 JSON 序列化，否则采用二进制序列化</param>
            /// <param name="clientCustom">客户端自定义绑定</param>
            public identityClient(Type tcpCallType, string cachePath = null, string cacheFileName = null, bool isJsonCacheFile = true, clientCustom clientCustom = null)
                : this((Action<Action<fastCSharp.net.returnValue<cacheIdentity>>>)Delegate.CreateDelegate(typeof(Action<Action<fastCSharp.net.returnValue<cacheIdentity>>>), tcpCallType.GetMethod("getSqlCache", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(Action<fastCSharp.net.returnValue<cacheIdentity>>) }, null))
                      , (Func<long, int, Action<fastCSharp.net.returnValue<data>>, commandClient.streamCommandSocket.keepCallback>)Delegate.CreateDelegate(typeof(Func<long, int, Action<fastCSharp.net.returnValue<data>>, commandClient.streamCommandSocket.keepCallback>), tcpCallType.GetMethod("onSqlLog", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(long), typeof(int), typeof(Action<fastCSharp.net.returnValue<data>>) }, null))
                      , (Func<int, fastCSharp.net.returnValue<valueType>>)Delegate.CreateDelegate(typeof(Func<int, fastCSharp.net.returnValue<valueType>>), tcpCallType.GetMethod("getSqlCache", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(int) }, null))
                      , cachePath, cacheFileName, isJsonCacheFile, clientCustom)
            {
            }
            /// <summary>
            /// 虚拟客户端创建自增标识客户端
            /// </summary>
            /// <param name="tcpCallType">TCP 调用类型</param>
            /// <param name="cachePath">缓存文件保存路径</param>
            /// <param name="cacheFileName">缓存文件名称</param>
            /// <param name="isJsonCacheFile">缓存文件是否 JSON 序列化，否则采用二进制序列化</param>
            /// <param name="clientCustom">客户端自定义绑定</param>
            /// <returns></returns>
            public identityClient CreateNull(Type tcpCallType, string cachePath = null, string cacheFileName = null, bool isJsonCacheFile = true, clientCustom clientCustom = null)
            {
                return isNull ? new identityClient(tcpCallType, cachePath, cacheFileName, isJsonCacheFile, clientCustom) : this;
            }

            /// <summary>
            /// 获取缓存数据集合
            /// </summary>
            public IEnumerable<valueType> CacheValues
            {
                get
                {
                    if (maxIdentity != 0)
                    {
                        int count = maxIdentity + 1;
                        foreach (valueType[] array in values.LeftArrays)
                        {
                            foreach (valueType value in array)
                            {
                                if (value != null) yield return value;
                            }
                            count -= identityArray.ArraySize;
                        }
                        foreach (valueType value in values.LastArray)
                        {
                            if (value != null) yield return value;
                            if (--count == 0) break;
                        }
                    }
                }
            }
            /// <summary>
            /// 数据
            /// </summary>
            /// <param name="identity"></param>
            /// <returns></returns>
            public valueType this[int identity]
            {
                get { return Get(identity); }
            }
            /// <summary>
            /// 获取数据
            /// </summary>
            /// <param name="identity"></param>
            /// <returns></returns>
            public valueType Get(int identity)
            {
                if (identity <= 0) return null;
                if (values.Length == 0)
                {
                    if (isNull) log.Error.Throw(log.exceptionType.ErrorOperation);
                    keyValue<valueType, EventWaitHandle> valueWait = default(keyValue<valueType, EventWaitHandle>);
                    if (get(identity, ref valueWait))
                    {
                        try
                        {
                            fastCSharp.net.returnValue<valueType> value = getValue(identity);
                            if (values.Length == 0) set(identity, value.Value);
                            return value.Value;
                        }
                        finally { valueWait.Value.Set(); }
                    }
                    if (valueWait.Value == null) return valueWait.Key;
                    valueWait.Value.WaitOne();
                    if (values.Length == 0)
                    {
                        if ((valueWait.Key = get(identity)) != null) return valueWait.Key;
                        if (values.Length == 0) return null;
                    }
                }
                if (identity <= maxIdentity) return values[identity];
                return getValue(identity);
            }
            /// <summary>
            /// 获取数据集合
            /// </summary>
            /// <param name="identitys"></param>
            /// <returns></returns>
            public valueType[] Get(fastCSharp.net.returnValue<int[]> identitys)
            {
                if (identitys.Type == fastCSharp.net.returnValue.type.Success) return Get(identitys.Value);
                return new fastCSharp.net.returnValue<valueType[]> { Type = identitys.Type };
            }
            /// <summary>
            /// 获取数据集合
            /// </summary>
            /// <param name="identitys"></param>
            /// <returns></returns>
            public valueType[] Get(int[] identitys)
            {
                if (IsData)
                {
                    valueType[] values = new valueType[identitys.Length];
                    int index = 0;
                    foreach (int identity in identitys) values[index++] = Get(identity);
                    return values;
                }
                return get(identitys);
            }
            ///// <summary>
            ///// 获取数据集合
            ///// </summary>
            ///// <param name="identitys"></param>
            ///// <returns></returns>
            //public valueType[] GetCache(fastCSharp.net.returnValue<int[]> identitys)
            //{
            //    if (identitys.Type == fastCSharp.net.returnValue.type.Success) return GetCache(identitys.Value);
            //    return new fastCSharp.net.returnValue<valueType[]> { Type = identitys.Type };
            //}
            /// <summary>
            /// 获取数据集合
            /// </summary>
            /// <param name="identitys"></param>
            /// <returns></returns>
            private valueType[] get(int[] identitys)
            {
                if (identitys == null) return null;
                if (identitys.Length == 0) return nullValue<valueType>.Array;
                valueType[] values = new valueType[identitys.Length];
                identityArray<valueType> cacheValues = this.values;
                int index = 0;
                foreach (int identity in identitys)
                {
                    if ((uint)identity < (uint)cacheValues.Length) values[index] = cacheValues[identity];
                    ++index;
                }
                return values;
            }
            ///// <summary>
            ///// 获取数据集合
            ///// </summary>
            ///// <param name="identitys"></param>
            ///// <returns></returns>
            //public valueType[] GetCache(subArray<int> identitys)
            //{
            //    if (identitys.Count == 0) return nullValue<valueType>.Array;
            //    valueType[] values = new valueType[identitys.Count], cacheValues = this.values;
            //    int index = 0;
            //    foreach (int identity in identitys)
            //    {
            //        if ((uint)identity < (uint)cacheValues.Length) values[index] = cacheValues[identity];
            //        ++index;
            //    }
            //    return values;
            //}
            /// <summary>
            /// 保存文件
            /// </summary>
            public unsafe override void CheckSave()
            {
                if (cacheFileName != null && values.Length != 0 && Interlocked.Increment(ref saveCount) == 1) save(CacheValues.getArray());
            }
            /// <summary>
            /// 设置缓存
            /// </summary>
            /// <param name="values"></param>
            protected override void setFile(valueType[] values)
            {
                if (isLog != 0) return;
                int identity = values.maxKey(value => getKey(value), 0);
                identityArray<valueType> newValues = new identityArray<valueType>(identity + 1);
                foreach (valueType value in values) newValues[getKey(value)] = value;
                interlocked.CompareSetYield(ref loadLock);
                if (IsData) loadLock = 0;
                else
                {
                    this.values = newValues;
                    maxIdentity = identity;
                    Count = values.Length;
                    IsData = true;
                    loadLock = 0;
                    clearTemp();
                }
            }
            /// <summary>
            /// 加载数据
            /// </summary>
            /// <param name="data"></param>
            protected override void loaded(fastCSharp.net.returnValue<cacheIdentity> data)
            {
                byte isError = 0;
                try
                {
                    valueType[] values = data.Value.Values;
                    int identity = values.maxKey(value => getKey(value), 0);
                    identityArray<valueType> newValues = new identityArray<valueType>(identity + 1);
                    foreach (valueType value in values) newValues[getKey(value)] = value;
                    interlocked.CompareSetYield(ref loadLock);
                    this.values = newValues;
                    maxIdentity = identity;
                    Count = values.Length;
                    IsData = true;
                    loadLock = 0;
                    clearTemp();
                    clientCustom.Load(values);
                    keepCallback = getLog(data.Value.Ticks, data.Value.Identity, onLog);
                    return;
                }
                catch (Exception error)
                {
                    isError = 1;
                    log.Error.Add(error, "客户端缓存加载失败 " + typeof(valueType).fullName(), true);
                }
                fastCSharp.threading.timerTask.Default.Add(getCache, load, date.NowSecond.AddSeconds(5), null);
                if (isError == 0) base.error();
            }
            /// <summary>
            /// 日志流数据
            /// </summary>
            /// <param name="data"></param>
            private void onLog(fastCSharp.net.returnValue<data> data)
            {
                if (data.Type == fastCSharp.net.returnValue.type.Success)
                {
                    valueType value = data.Value.Value.Value;
                    int identity = getKey(value);
                    switch (data.Value.Type)
                    {
                        case type.Insert:
                            if (identity >= values.Length) values.ToSize(identity + 1);
                            if (values[identity] == null) ++Count;
                            if (identity > maxIdentity) maxIdentity = identity;
                            clientCustom.Insert(values[identity] = value);
                            return;
                        case type.Update:
                            if (identity < values.Length)
                            {
                                valueType clientValue = values[identity];
                                if (clientValue != null)
                                {
                                    checkMemberMap(ref data);
                                    fastCSharp.emit.memberCopyer<modelType>.Copy(clientValue, value, data.Value.Value.MemberMap);
                                    clientCustom.Update(clientValue);
                                }
                            }
                            typePool<valueType>.PushOnly(value);
                            return;
                        case type.Delete:
                            typePool<valueType>.PushOnly(value);
                            if (identity < values.Length)
                            {
                                valueType clientValue = values[identity];
                                if (clientValue != null)
                                {
                                    clientCustom.Delete(clientValue);
                                    checkMemberMap(ref data);
                                    values[identity] = null;
                                    --Count;
                                }
                            }
                            return;
                    }
                }
                pub.Dispose(ref keepCallback);
                fastCSharp.threading.timerTask.Default.Add(getCache, load, date.NowSecond.AddSeconds(5), null);
            }
        }
        /// <summary>
        /// 关键字客户端
        /// </summary>
        /// <typeparam name="keyType"></typeparam>
        public sealed class dictionaryClient<keyType> : client<keyType>
            where keyType : IEquatable<keyType>
        {
            /// <summary>
            /// 虚拟空日志流客户端
            /// </summary>
            public static readonly dictionaryClient<keyType> Null = new dictionaryClient<keyType>();
            /// <summary>
            /// 缓存数据
            /// </summary>
            private Dictionary<randomKey<keyType>, valueType> values;
            /// <summary>
            /// 数据
            /// </summary>
            /// <param name="identity"></param>
            /// <returns></returns>
            public valueType this[keyType key]
            {
                get { return Get(key); }
            }
            /// <summary>
            /// 获取数据
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public valueType Get(keyType key)
            {
                Dictionary<randomKey<keyType>, valueType> values = this.values;
                if (values == null)
                {
                    if (isNull) log.Error.Throw(log.exceptionType.ErrorOperation);
                    keyValue<valueType, EventWaitHandle> valueWait = default(keyValue<valueType, EventWaitHandle>);
                    if (get(key, ref valueWait))
                    {
                        try
                        {
                            fastCSharp.net.returnValue<valueType> value = getValue(key);
                            if (this.values == null) set(key, value.Value);
                            return value.Value;
                        }
                        finally { valueWait.Value.Set(); }
                    }
                    if (valueWait.Value == null) return valueWait.Key;
                    valueWait.Value.WaitOne();
                    if ((values = this.values) == null)
                    {
                        if ((valueWait.Key = get(key)) != null) return valueWait.Key;
                        if ((values = this.values) == null) return null;
                    }
                }
                valueType cacheValue;
                values.TryGetValue(key, out cacheValue);
                return cacheValue;
            }
            /// <summary>
            /// 虚拟空日志流客户端
            /// </summary>
            private dictionaryClient() { }
            /// <summary>
            /// 日志流客户端
            /// </summary>
            /// <param name="getCache">获取缓存数据委托</param>
            /// <param name="getLog">获取日志数据委托</param>
            /// <param name="getValue">获取数据委托</param>
            /// <param name="cachePath">缓存文件保存路径</param>
            /// <param name="cacheFileName">缓存文件名称</param>
            /// <param name="isJsonCacheFile">缓存文件是否 JSON 序列化，否则采用二进制序列化</param>
            /// <param name="clientCustom">客户端自定义绑定</param>
            public dictionaryClient(Action<Action<fastCSharp.net.returnValue<cacheIdentity>>> getCache
                , Func<long, int, Action<fastCSharp.net.returnValue<data>>, commandClient.streamCommandSocket.keepCallback> getLog
                , Func<modelType, keyType> getKey, Func<keyType, fastCSharp.net.returnValue<valueType>> getValue
                , string cachePath = null, string cacheFileName = null, bool isJsonCacheFile = true, clientCustom clientCustom = null)
                : base(getCache, getLog, getKey, getValue, cachePath, cacheFileName, isJsonCacheFile, clientCustom)
            {
                getCache(load);
                if (this.cacheFileName != null) fastCSharp.threading.threadPool.TinyPool.Start(readCacheFile);
            }
            /// <summary>
            /// 日志流客户端
            /// </summary>
            /// <param name="tcpCallType">TCP 调用类型</param>
            /// <param name="cachePath">缓存文件保存路径</param>
            /// <param name="cacheFileName">缓存文件名称</param>
            /// <param name="isJsonCacheFile">缓存文件是否 JSON 序列化，否则采用二进制序列化</param>
            /// <param name="clientCustom">客户端自定义绑定</param>
            public dictionaryClient(Type tcpCallType, string cachePath = null, string cacheFileName = null, bool isJsonCacheFile = true, clientCustom clientCustom = null)
                : this((Action<Action<fastCSharp.net.returnValue<cacheIdentity>>>)Delegate.CreateDelegate(typeof(Action<Action<fastCSharp.net.returnValue<cacheIdentity>>>), tcpCallType.GetMethod("getSqlCache", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(Action<fastCSharp.net.returnValue<cacheIdentity>>) }, null))
                      , (Func<long, int, Action<fastCSharp.net.returnValue<data>>, commandClient.streamCommandSocket.keepCallback>)Delegate.CreateDelegate(typeof(Func<long, int, Action<fastCSharp.net.returnValue<data>>, commandClient.streamCommandSocket.keepCallback>), tcpCallType.GetMethod("onSqlLog", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(long), typeof(int), typeof(Action<fastCSharp.net.returnValue<data>>) }, null))
                      , fastCSharp.emit.sqlModel<modelType>.PrimaryKeyGetter<keyType>()
                      , (Func<keyType, fastCSharp.net.returnValue<valueType>>)Delegate.CreateDelegate(typeof(Func<keyType, fastCSharp.net.returnValue<valueType>>), tcpCallType.GetMethod("getSqlCache", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(keyType) }, null))
                      , cachePath, cacheFileName, isJsonCacheFile, clientCustom)
            {
            }
            /// <summary>
            /// 日志流客户端
            /// </summary>
            /// <param name="tcpCallType">TCP 调用类型</param>
            /// <param name="getValue">获取数据委托</param>
            /// <param name="cachePath">缓存文件保存路径</param>
            /// <param name="cacheFileName">缓存文件名称</param>
            /// <param name="isJsonCacheFile">缓存文件是否 JSON 序列化，否则采用二进制序列化</param>
            /// <param name="clientCustom">客户端自定义绑定</param>
            public dictionaryClient(Type tcpCallType, Func<keyType, fastCSharp.net.returnValue<valueType>> getValue
                , string cachePath = null, string cacheFileName = null, bool isJsonCacheFile = true, clientCustom clientCustom = null)
                : this((Action<Action<fastCSharp.net.returnValue<cacheIdentity>>>)Delegate.CreateDelegate(typeof(Action<Action<fastCSharp.net.returnValue<cacheIdentity>>>), tcpCallType.GetMethod("getSqlCache", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(Action<fastCSharp.net.returnValue<cacheIdentity>>) }, null))
                      , (Func<long, int, Action<fastCSharp.net.returnValue<data>>, commandClient.streamCommandSocket.keepCallback>)Delegate.CreateDelegate(typeof(Func<long, int, Action<fastCSharp.net.returnValue<data>>, commandClient.streamCommandSocket.keepCallback>), tcpCallType.GetMethod("onSqlLog", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(long), typeof(int), typeof(Action<fastCSharp.net.returnValue<data>>) }, null))
                      , fastCSharp.emit.sqlModel<modelType>.PrimaryKeyGetter<keyType>(), getValue, cachePath, cacheFileName, isJsonCacheFile, clientCustom)
            {
            }
            /// <summary>
            /// 虚拟客户端创建关键字客户端
            /// </summary>
            /// <param name="tcpCallType">TCP 调用类型</param>
            /// <param name="cachePath">缓存文件保存路径</param>
            /// <param name="cacheFileName">缓存文件名称</param>
            /// <param name="isJsonCacheFile">缓存文件是否 JSON 序列化，否则采用二进制序列化</param>
            /// <param name="clientCustom">客户端自定义绑定</param>
            /// <returns></returns>
            public dictionaryClient<keyType> CreateNull(Type tcpCallType, string cachePath = null, string cacheFileName = null, bool isJsonCacheFile = true, clientCustom clientCustom = null)
            {
                return isNull ? new dictionaryClient<keyType>(tcpCallType, cachePath, cacheFileName, isJsonCacheFile, clientCustom) : this;
            }
            /// <summary>
            /// 虚拟客户端创建关键字客户端
            /// </summary>
            /// <param name="tcpCallType">TCP 调用类型</param>
            /// <param name="getValue">获取数据委托</param>
            /// <param name="cachePath">缓存文件保存路径</param>
            /// <param name="cacheFileName">缓存文件名称</param>
            /// <param name="isJsonCacheFile">缓存文件是否 JSON 序列化，否则采用二进制序列化</param>
            /// <param name="clientCustom">客户端自定义绑定</param>
            /// <returns></returns>
            public dictionaryClient<keyType> CreateNull(Type tcpCallType, Func<keyType, fastCSharp.net.returnValue<valueType>> getValue
                , string cachePath = null, string cacheFileName = null, bool isJsonCacheFile = true, clientCustom clientCustom = null)
            {
                return isNull ? new dictionaryClient<keyType>(tcpCallType, getValue, cachePath, cacheFileName, isJsonCacheFile, clientCustom) : this;
            }

            /// <summary>
            /// 保存文件
            /// </summary>
            public unsafe override void CheckSave()
            {
                if (cacheFileName != null && values != null && Interlocked.Increment(ref saveCount) == 1) save(values.Values.getArray());
            }
            /// <summary>
            /// 设置缓存
            /// </summary>
            /// <param name="values"></param>
            protected override void setFile(valueType[] values)
            {
                if (isLog != 0) return;
                Dictionary<randomKey<keyType>, valueType> newValues = dictionary.Create<randomKey<keyType>, valueType>(values.Length);
                foreach (valueType value in values) newValues.Add(getKey(value), value);
                interlocked.CompareSetYield(ref loadLock);
                if (IsData) loadLock = 0;
                else
                {
                    this.values = newValues;
                    IsData = true;
                    loadLock = 0;
                    clearTemp();
                }
            }
            /// <summary>
            /// 加载数据
            /// </summary>
            /// <param name="data"></param>
            protected override void loaded(fastCSharp.net.returnValue<cacheIdentity> data)
            {
                byte isError = 0;
                try
                {
                    valueType[] values = data.Value.Values;
                    Dictionary<randomKey<keyType>, valueType> newValues = dictionary.Create<randomKey<keyType>, valueType>(values.Length);
                    foreach (valueType value in values) newValues.Add(getKey(value), value);
                    interlocked.CompareSetYield(ref loadLock);
                    this.values = newValues;
                    IsData = true;
                    loadLock = 0;
                    clearTemp();
                    clientCustom.Load(values);
                    keepCallback = getLog(data.Value.Ticks, data.Value.Identity, onLog);
                    return;
                }
                catch (Exception error)
                {
                    isError = 1;
                    log.Error.Add(error, "客户端缓存加载失败 " + typeof(valueType).fullName(), true);
                }
                fastCSharp.threading.timerTask.Default.Add(getCache, load, date.NowSecond.AddSeconds(5), null);
                if (isError == 0) base.error();
            }
            /// <summary>
            /// 日志流数据
            /// </summary>
            /// <param name="data"></param>
            private void onLog(fastCSharp.net.returnValue<data> data)
            {
                if (data.Type == fastCSharp.net.returnValue.type.Success)
                {
                    valueType value = data.Value.Value.Value, clientValue;
                    keyType key = getKey(value);
                    switch (data.Value.Type)
                    {
                        case type.Insert:
                            clientCustom.Insert(values[key] = value);
                            return;
                        case type.Update:
                            if (values.TryGetValue(key, out clientValue))
                            {
                                checkMemberMap(ref data);
                                fastCSharp.emit.memberCopyer<modelType>.Copy(clientValue, value, data.Value.Value.MemberMap);
                                clientCustom.Update(clientValue);
                            }
                            typePool<valueType>.PushOnly(value);
                            return;
                        case type.Delete:
                            typePool<valueType>.PushOnly(value);
                            if (clientCustom == clientCustom.Null) values.Remove(key);
                            else if (values.TryGetValue(key, out clientValue))
                            {
                                values.Remove(key);
                                clientCustom.Delete(clientValue);
                            }
                            return;
                    }
                }
                pub.Dispose(ref keepCallback);
                fastCSharp.threading.timerTask.Default.Add(getCache, load, date.NowSecond.AddSeconds(5), null);
            }
        }
        /// <summary>
        /// 关键字客户端
        /// </summary>
        /// <typeparam name="cacheType"></typeparam>
        /// <typeparam name="keyType"></typeparam>
        public sealed class dictionaryClient<keyType, cacheType> : client<keyType>
            where keyType : IEquatable<keyType>
        {
            /// <summary>
            /// 虚拟空日志流客户端
            /// </summary>
            public static readonly dictionaryClient<keyType, cacheType> Null = new dictionaryClient<keyType, cacheType>();
            /// <summary>
            /// 获取缓存数据
            /// </summary>
            private Func<modelType, cacheType> getCacheValue;
            /// <summary>
            /// 获取缓存数据
            /// </summary>
            private Func<KeyValuePair<randomKey<keyType>, cacheType>, valueType> fromCacheValue;
            /// <summary>
            /// 缓存数据
            /// </summary>
            private Dictionary<randomKey<keyType>, cacheType> values;
            /// <summary>
            /// 默认空值
            /// </summary>
            private cacheType nullValue;
            /// <summary>
            /// 数据
            /// </summary>
            /// <param name="identity"></param>
            /// <returns></returns>
            public cacheType this[keyType key]
            {
                get { return Get(key); }
            }
            /// <summary>
            /// 获取数据
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public cacheType Get(keyType key)
            {
                Dictionary<randomKey<keyType>, cacheType> values = this.values;
                if (values == null)
                {
                    if (isNull) log.Error.Throw(log.exceptionType.ErrorOperation);
                    keyValue<valueType, EventWaitHandle> valueWait = default(keyValue<valueType, EventWaitHandle>);
                    if (get(key, ref valueWait))
                    {
                        try
                        {
                            fastCSharp.net.returnValue<valueType> value = getValue(key);
                            if (this.values == null) set(key, value.Value);
                            return getCacheValue(value.Value);
                        }
                        finally { valueWait.Value.Set(); }
                    }
                    if (valueWait.Value == null) return getCacheValue(valueWait.Key);
                    valueWait.Value.WaitOne();
                    if ((values = this.values) == null)
                    {
                        if ((valueWait.Key = get(key)) != null) return getCacheValue(valueWait.Key);
                        if ((values = this.values) == null) return nullValue;
                    }
                }
                cacheType cacheValue;
                return values.TryGetValue(key, out cacheValue) ? cacheValue : nullValue;
            }
            /// <summary>
            /// 虚拟空日志流客户端
            /// </summary>
            private dictionaryClient() { }
            /// <summary>
            /// 日志流客户端
            /// </summary>
            /// <param name="getCache">获取缓存数据委托</param>
            /// <param name="getLog">获取日志数据委托</param>
            /// <param name="getValue">获取数据委托</param>
            /// <param name="getCacheValue"></param>
            public dictionaryClient(Action<Action<fastCSharp.net.returnValue<cacheIdentity>>> getCache
                , Func<long, int, Action<fastCSharp.net.returnValue<data>>, commandClient.streamCommandSocket.keepCallback> getLog
                , Func<modelType, keyType> getKey, Func<keyType, fastCSharp.net.returnValue<valueType>> getValue
                , Func<modelType, cacheType> getCacheValue, Func<KeyValuePair<randomKey<keyType>, cacheType>, valueType> fromCacheValue, cacheType nullValue
                , string cachePath = null, string cacheFileName = null, bool isJsonCacheFile = true, clientCustom clientCustom = null)
                : base(getCache, getLog, getKey, getValue, cachePath, cacheFileName, isJsonCacheFile, clientCustom)
            {
                if (getCacheValue == null || fromCacheValue == null) log.Error.Throw(log.exceptionType.Null);
                this.getCacheValue = getCacheValue;
                this.fromCacheValue = fromCacheValue;
                this.nullValue = nullValue;
                getCache(load);
                if (this.cacheFileName != null) fastCSharp.threading.threadPool.TinyPool.Start(readCacheFile);
            }
            /// <summary>
            /// 日志流客户端
            /// </summary>
            /// <param name="tcpCallType"></param>
            public dictionaryClient(Type tcpCallType, Func<modelType, cacheType> getCacheValue, Func<KeyValuePair<randomKey<keyType>, cacheType>, valueType> fromCacheValue, cacheType nullValue
                , string cachePath = null, string cacheFileName = null, bool isJsonCacheFile = true, clientCustom clientCustom = null)
                : this((Action<Action<fastCSharp.net.returnValue<cacheIdentity>>>)Delegate.CreateDelegate(typeof(Action<Action<fastCSharp.net.returnValue<cacheIdentity>>>), tcpCallType.GetMethod("getSqlCache", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(Action<fastCSharp.net.returnValue<cacheIdentity>>) }, null))
                      , (Func<long, int, Action<fastCSharp.net.returnValue<data>>, commandClient.streamCommandSocket.keepCallback>)Delegate.CreateDelegate(typeof(Func<long, int, Action<fastCSharp.net.returnValue<data>>, commandClient.streamCommandSocket.keepCallback>), tcpCallType.GetMethod("onSqlLog", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(long), typeof(int), typeof(Action<fastCSharp.net.returnValue<data>>) }, null))
                      , fastCSharp.emit.sqlModel<modelType>.PrimaryKeyGetter<keyType>()
                      , (Func<keyType, fastCSharp.net.returnValue<valueType>>)Delegate.CreateDelegate(typeof(Func<keyType, fastCSharp.net.returnValue<valueType>>), tcpCallType.GetMethod("getSqlCache", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(keyType) }, null))
                      , getCacheValue, fromCacheValue, nullValue, cachePath, cacheFileName, isJsonCacheFile, clientCustom)
            {
            }
            /// <summary>
            /// 日志流客户端
            /// </summary>
            /// <param name="tcpCallType"></param>
            /// <param name="getValue">获取数据委托</param>
            public dictionaryClient(Type tcpCallType, Func<keyType, fastCSharp.net.returnValue<valueType>> getValue
                , Func<modelType, cacheType> getCacheValue, Func<KeyValuePair<randomKey<keyType>, cacheType>, valueType> fromCacheValue, cacheType nullValue
                , string cachePath = null, string cacheFileName = null, bool isJsonCacheFile = true, clientCustom clientCustom = null)
                : this((Action<Action<fastCSharp.net.returnValue<cacheIdentity>>>)Delegate.CreateDelegate(typeof(Action<Action<fastCSharp.net.returnValue<cacheIdentity>>>), tcpCallType.GetMethod("getSqlCache", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(Action<fastCSharp.net.returnValue<cacheIdentity>>) }, null))
                      , (Func<long, int, Action<fastCSharp.net.returnValue<data>>, commandClient.streamCommandSocket.keepCallback>)Delegate.CreateDelegate(typeof(Func<long, int, Action<fastCSharp.net.returnValue<data>>, commandClient.streamCommandSocket.keepCallback>), tcpCallType.GetMethod("onSqlLog", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(long), typeof(int), typeof(Action<fastCSharp.net.returnValue<data>>) }, null))
                      , fastCSharp.emit.sqlModel<modelType>.PrimaryKeyGetter<keyType>(), getValue
                      , getCacheValue, fromCacheValue, nullValue, cachePath, cacheFileName, isJsonCacheFile, clientCustom)
            {
            }
            /// <summary>
            /// 虚拟客户端创建关键字客户端
            /// </summary>
            /// <param name="tcpCallType"></param>
            /// <param name="getCacheValue"></param>
            /// <param name="fromCacheValue"></param>
            /// <param name="nullValue"></param>
            /// <param name="cachePath"></param>
            /// <param name="cacheFileName"></param>
            /// <param name="isJsonCacheFile"></param>
            /// <param name="clientCustom"></param>
            /// <returns></returns>
            public dictionaryClient<keyType, cacheType> CreateNull(Type tcpCallType
                , Func<modelType, cacheType> getCacheValue, Func<KeyValuePair<randomKey<keyType>, cacheType>, valueType> fromCacheValue, cacheType nullValue
                , string cachePath = null, string cacheFileName = null, bool isJsonCacheFile = true, clientCustom clientCustom = null)
            {
                return isNull ? new dictionaryClient<keyType, cacheType>(tcpCallType, getCacheValue, fromCacheValue, nullValue, cachePath, cacheFileName, isJsonCacheFile, clientCustom) : this;
            }
            /// <summary>
            /// 虚拟客户端创建关键字客户端
            /// </summary>
            /// <param name="tcpCallType"></param>
            /// <param name="getValue">获取数据委托</param>
            /// <param name="getCacheValue"></param>
            /// <param name="fromCacheValue"></param>
            /// <param name="nullValue"></param>
            /// <param name="cachePath"></param>
            /// <param name="cacheFileName"></param>
            /// <param name="isJsonCacheFile"></param>
            /// <param name="clientCustom"></param>
            /// <returns></returns>
            public dictionaryClient<keyType, cacheType> CreateNull(Type tcpCallType, Func<keyType, fastCSharp.net.returnValue<valueType>> getValue
                , Func<modelType, cacheType> getCacheValue, Func<KeyValuePair<randomKey<keyType>, cacheType>, valueType> fromCacheValue, cacheType nullValue
                , string cachePath = null, string cacheFileName = null, bool isJsonCacheFile = true, clientCustom clientCustom = null)
            {
                return isNull ? new dictionaryClient<keyType, cacheType>(tcpCallType, getValue, getCacheValue, fromCacheValue, nullValue, cachePath, cacheFileName, isJsonCacheFile, clientCustom) : this;
            }

            /// <summary>
            /// 保存文件
            /// </summary>
            public unsafe override void CheckSave()
            {
                if (cacheFileName != null && values != null && Interlocked.Increment(ref saveCount) == 1) save(values.getArray(value => fromCacheValue(value)));
            }
            /// <summary>
            /// 设置缓存
            /// </summary>
            /// <param name="values"></param>
            protected override void setFile(valueType[] values)
            {
                if (isLog != 0) return;
                Dictionary<randomKey<keyType>, cacheType> newValues = dictionary.Create<randomKey<keyType>, cacheType>(values.Length);
                foreach (valueType value in values) newValues.Add(getKey(value), getCacheValue(value));
                interlocked.CompareSetYield(ref loadLock);
                if (IsData) loadLock = 0;
                else
                {
                    this.values = newValues;
                    IsData = true;
                    loadLock = 0;
                    clearTemp();
                }
            }
            /// <summary>
            /// 加载数据
            /// </summary>
            /// <param name="data"></param>
            protected override void loaded(fastCSharp.net.returnValue<cacheIdentity> data)
            {
                byte isError = 0;
                try
                {
                    valueType[] values = data.Value.Values;
                    Dictionary<randomKey<keyType>, cacheType> newValues = dictionary.Create<randomKey<keyType>, cacheType>(values.Length);
                    foreach (valueType value in values) newValues.Add(getKey(value), getCacheValue(value));
                    interlocked.CompareSetYield(ref loadLock);
                    this.values = newValues;
                    IsData = true;
                    loadLock = 0;
                    clearTemp();
                    clientCustom.Load(values);
                    keepCallback = getLog(data.Value.Ticks, data.Value.Identity, onLog);
                    return;
                }
                catch (Exception error)
                {
                    isError = 1;
                    log.Error.Add(error, "客户端缓存加载失败 " + typeof(valueType).fullName(), true);
                }
                fastCSharp.threading.timerTask.Default.Add(getCache, load, date.NowSecond.AddSeconds(5), null);
                if (isError == 0) base.error();
            }
            /// <summary>
            /// 日志流数据
            /// </summary>
            /// <param name="data"></param>
            private void onLog(fastCSharp.net.returnValue<data> data)
            {
                if (data.Type == fastCSharp.net.returnValue.type.Success)
                {
                    keyType key = getKey(data.Value.Value.Value);
                    switch (data.Value.Type)
                    {
                        case type.Insert:
                            values[key] = getCacheValue(data.Value.Value.Value);
                            clientCustom.Insert(data.Value.Value.Value);
                            return;
                        case type.Update:
                            values[key] = getCacheValue(data.Value.Value.Value);
                            clientCustom.Update(data.Value.Value.Value);
                            return;
                        case type.Delete:
                            if (clientCustom == clientCustom.Null) values.Remove(key);
                            else
                            {
                                cacheType value;
                                if (values.TryGetValue(key, out value))
                                {
                                    values.Remove(key);
                                    clientCustom.Delete(fromCacheValue(new KeyValuePair<randomKey<keyType>, cacheType>(key, value)));
                                }
                            }
                            return;
                    }
                }
                pub.Dispose(ref keepCallback);
                fastCSharp.threading.timerTask.Default.Add(getCache, load, date.NowSecond.AddSeconds(5), null);
            }
        }
        /// <summary>
        /// 先进先出队列缓存客户端
        /// </summary>
        /// <typeparam name="keyType"></typeparam>
        public sealed class queueClient<keyType> : client where keyType : IEquatable<keyType>
        {
            /// <summary>
            /// 虚拟空日志流客户端
            /// </summary>
            public static readonly queueClient<keyType> Null = new queueClient<keyType>();
            /// <summary>
            /// 获取日志数据委托
            /// </summary>
            private readonly Func<Action<fastCSharp.net.returnValue<data>>, commandClient.streamCommandSocket.keepCallback> getLog;
            /// <summary>
            /// 获取关键字委托
            /// </summary>
            private readonly Func<modelType, keyType> getKey;
            /// <summary>
            /// 获取数据委托
            /// </summary>
            private readonly Func<keyType, fastCSharp.net.returnValue<valueType>> getValue;
            /// <summary>
            /// 缓存数据
            /// </summary>
            private fifoPriorityQueue<randomKey<keyType>, keyValue<valueType, EventWaitHandle>> queue;
            /// <summary>
            /// 最大数量
            /// </summary>
            private int maxCount;
            /// <summary>
            /// 访问锁
            /// </summary>
            private readonly object logLock;
            /// <summary>
            /// 数据
            /// </summary>
            /// <param name="identity"></param>
            /// <returns></returns>
            public valueType this[keyType key]
            {
                get { return Get(key); }
            }
            /// <summary>
            /// 获取数据
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public valueType Get(keyType key)
            {
                if (isNull) log.Error.Throw(log.exceptionType.ErrorOperation);
                if (isLog != 0)
                {
                    keyValue<valueType, EventWaitHandle> value;
                TRYGET:
                    Monitor.Enter(logLock);
                    if (queue.TryGet(key, out value))
                    {
                        Monitor.Exit(logLock);
                        if (value.Value == null) return value.Key;
                        value.Value.WaitOne();
                        goto TRYGET;
                    }
                    try
                    {
                        value.Value = new EventWaitHandle(false, EventResetMode.ManualReset);
                        queue.Set(key, value);
                        if (queue.Count > maxCount) queue.UnsafePopValue();
                    }
                    finally { Monitor.Exit(logLock); }
                    try
                    {
                        value.Key = getValue(key);
                    }
                    finally
                    {
                        keyValue<valueType, EventWaitHandle> cacheValue;
                        Monitor.Enter(logLock);
                        if (queue.TryGetOnly(key, out cacheValue) && cacheValue.Value == value.Value)
                        {
                            try
                            {
                                queue.SetOnly(key, new keyValue<valueType, EventWaitHandle>(value.Key, null));
                            }
                            finally { Monitor.Exit(logLock); }
                        }
                        else Monitor.Exit(logLock);
                        value.Value.Set();
                    }
                    return value.Key;
                }
                return getValue(key);
            }
            /// <summary>
            /// 虚拟空日志流客户端
            /// </summary>
            private queueClient() { }
            /// <summary>
            /// 日志流客户端
            /// </summary>
            /// <param name="getCache">获取缓存数据委托</param>
            /// <param name="getLog">获取日志数据委托</param>
            /// <param name="getValue">获取数据委托</param>
            public queueClient(Func<Action<fastCSharp.net.returnValue<data>>, commandClient.streamCommandSocket.keepCallback> getLog
                , Func<modelType, keyType> getKey, Func<keyType, fastCSharp.net.returnValue<valueType>> getValue, int maxCount)
                : base(true)
            {
                if (getLog == null || getKey == null || getValue == null) log.Error.Throw(log.exceptionType.Null);
                this.getLog = getLog;
                this.getKey = getKey;
                this.getValue = getValue;
                this.maxCount = Math.Max(maxCount, 1);
                logLock = new object();
                queue = new fifoPriorityQueue<randomKey<keyType>, keyValue<valueType, EventWaitHandle>>();
                load();
            }
            /// <summary>
            /// 虚拟客户端创建先进先出队列缓存客户端
            /// </summary>
            /// <param name="getCache">获取缓存数据委托</param>
            /// <param name="getLog">获取日志数据委托</param>
            /// <param name="getValue">获取数据委托</param>
            /// <param name="maxCount"></param>
            /// <returns></returns>
            public queueClient<keyType> CreateNull(Func<Action<fastCSharp.net.returnValue<data>>, commandClient.streamCommandSocket.keepCallback> getLog
                , Func<modelType, keyType> getKey, Func<keyType, fastCSharp.net.returnValue<valueType>> getValue, int maxCount)
            {
                return isNull ? new queueClient<keyType>(getLog, getKey, getValue, maxCount) : this;
            }
            /// <summary>
            /// 保存文件
            /// </summary>
            public override void CheckSave() { }
            /// <summary>
            /// 加载数据
            /// </summary>
            private void load()
            {
                keepCallback = getLog(onLog);
                if (keepCallback == null)
                {
                    //fastCSharp.threading.timerTask.Default.Add(load, date.NowSecond.AddSeconds(5));
                    error();
                }
                else
                {
                    Monitor.Enter(logLock);
                    queue.Clear();
                    isLog = 1;
                    Monitor.Exit(logLock);
                }
            }
            /// <summary>
            /// 日志流数据
            /// </summary>
            /// <param name="data"></param>
            private void onLog(fastCSharp.net.returnValue<data> data)
            {
                if (data.Type == fastCSharp.net.returnValue.type.Success)
                {
                    valueType value = data.Value.Value.Value;
                    keyType key = getKey(data.Value.Value.Value);
                    keyValue<valueType, EventWaitHandle> queueValue;
                    switch (data.Value.Type)
                    {
                        case type.Insert:
                            Monitor.Enter(logLock);
                            if (queue.TryGetOnly(key, out queueValue))
                            {
                                queue.SetOnly(key, new keyValue<valueType, EventWaitHandle>(value, null));
                                Monitor.Exit(logLock);
                            }
                            else
                            {
                                try
                                {
                                    queue.Set(key, new keyValue<valueType, EventWaitHandle>(value, null));
                                    if (queue.Count > maxCount) queue.UnsafePopValue();
                                }
                                finally { Monitor.Exit(logLock); }
                            }
                            return;
                        case type.Update:
                            Monitor.Enter(logLock);
                            if (queue.TryGetOnly(key, out queueValue))
                            {
                                if (queueValue.Value == null)
                                {
                                    try
                                    {
                                        fastCSharp.emit.memberCopyer<modelType>.Copy(queueValue.Key, value, data.Value.Value.MemberMap);
                                    }
                                    finally { Monitor.Exit(logLock); }
                                    if (data.Value.Value.MemberMap == null && isErrorMemberMap == 0)
                                    {
                                        isErrorMemberMap = 1;
                                        log.Error.Add("客户端缓存数据缺少成员位图信息 " + typeof(valueType).fullName(), new System.Diagnostics.StackFrame(), false);
                                    }
                                    return;
                                }
                                queue.Remove(key, out queueValue);
                            }
                            Monitor.Exit(logLock);
                            typePool<valueType>.PushOnly(value);
                            return;
                        case type.Delete:
                            typePool<valueType>.PushOnly(value);
                            Monitor.Enter(logLock);
                            queue.Remove(key, out queueValue);
                            Monitor.Exit(logLock);
                            return;
                    }
                }
                pub.Dispose(ref keepCallback);
                isLog = 0;
                fastCSharp.threading.timerTask.Default.Add(load, date.NowSecond.AddSeconds(2));
            }
        }
        /// <summary>
        /// 缓存
        /// </summary>
        private fastCSharp.sql.cache.whole.events.cache<valueType, modelType> cache;
        /// <summary>
        /// 删除数据成员位图
        /// </summary>
        private fastCSharp.code.memberMap<modelType> deleteMemberMap;
        /// <summary>
        /// 是否支持成员位图
        /// </summary>
        private bool isMemberMap;
        /// <summary>
        /// 日志流
        /// </summary>
        private logStream<data> stream;
        /// <summary>
        /// 日志流当前结束编号
        /// </summary>
        public cacheIdentity Cache
        {
            get
            {
                if (isLoaded)
                {
                    cacheIdentity cacheIdentity = new cacheIdentity { Ticks = stream.Ticks, Identity = stream.EndIdentity };
                    cacheIdentity.Values = cache.Values.getArray();
                    return cacheIdentity;
                }
                return default(cacheIdentity);
            }
        }
        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="cache"></param>
        public logStream(fastCSharp.sql.cache.whole.events.cache<valueType, modelType> cache, memberCount memberCount, bool isMemberMap) : this(cache, memberCount, isMemberMap, true) { }
        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="memberCount"></param>
        /// <param name="isMemberMap">是否支持成员位图</param>
        /// <param name="isLoad"></param>
        protected logStream(fastCSharp.sql.cache.whole.events.cache<valueType, modelType> cache, memberCount memberCount, bool isMemberMap, bool isLoad)
        {
            this.cache = cache;
            this.isMemberMap = isMemberMap;
            if (isMemberMap) deleteMemberMap = fastCSharp.emit.sqlModel<modelType>.GetIdentityOrPrimaryKeyMemberMap();
            stream = new logStream<data>(fastCSharp.config.sql.Default.LogStreamSize);
            if (isLoad)
            {
                cache.OnInserted += onInserted;
                cache.OnUpdated += onUpdated;
                cache.OnDeleted += onDeleted;
                if (memberCount == null) isLoaded = true;
                else memberCount.OnLoad(this);
            }
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value"></param>
        private void onInserted(valueType value)
        {
            if (isLoaded) stream.Append(new data { Type = type.Insert, Value = new fastCSharp.emit.memberMapValue<modelType, valueType>.deSerializePool { Value = value } });
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="value"></param>
        /// <param name="value"></param>
        /// <param name="oldValue"></param>
        /// <param name="memberMap"></param>
        private void onUpdated(valueType cacheValue, valueType value, valueType oldValue, fastCSharp.code.memberMap<modelType> memberMap)
        {
            if (isLoaded)
            {
                if (isMemberMap)
                {
                    (memberMap = memberMap.Copy()).And(cache.MemberMap);
                    if (memberMap.IsAnyMember)
                    {
                        fastCSharp.emit.sqlModel<modelType>.SetIdentityOrPrimaryKeyMemberMap(memberMap);
                        stream.Append(new data { Type = type.Update, Value = new fastCSharp.emit.memberMapValue<modelType, valueType>.deSerializePool { Value = cacheValue, MemberMap = memberMap } });
                    }
                }
                else stream.Append(new data { Type = type.Update, Value = new fastCSharp.emit.memberMapValue<modelType, valueType>.deSerializePool { Value = cacheValue } });
            }
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value"></param>
        private void onDeleted(valueType value)
        {
            if (isLoaded) stream.Append(new data { Type = type.Delete, Value = new fastCSharp.emit.memberMapValue<modelType, valueType>.deSerializePool { Value = value, MemberMap = isMemberMap ? deleteMemberMap : null } });
        }
        /// <summary>
        /// 更新缓存计算字段
        /// </summary>
        /// <param name="value"></param>
        /// <param name="memberMap"></param>
        public void Update(valueType value, fastCSharp.code.memberMap<modelType> memberMap)
        {
            if (isLoaded)
            {
                if (!isMemberMap) log.Error.Throw(log.exceptionType.ErrorOperation);
                stream.Append(new data { Type = type.Update, Value = new fastCSharp.emit.memberMapValue<modelType, valueType>.deSerializePool { Value = value, MemberMap = memberMap } });
            }
        }
        /// <summary>
        /// 日志处理
        /// </summary>
        /// <param name="ticks">时钟周期标识</param>
        /// <param name="identity">日志编号</param>
        /// <param name="onLog"></param>
        public void OnLog(long ticks, int identity, Func<fastCSharp.net.returnValue<data>, bool> onLog)
        {
            if (isLoaded) stream.Get(ticks, identity, onLog);
        }
        /// <summary>
        /// 日志处理(无验证)
        /// </summary>
        /// <param name="onLog"></param>
        public void OnLog(Func<fastCSharp.net.returnValue<data>, bool> onLog)
        {
            if (isLoaded) stream.Get(onLog);
        }
        /// <summary>
        /// 日志+条件过滤
        /// </summary>
        public sealed class where : logStream<valueType, modelType>
        {
            /// <summary>
            /// 条件过滤
            /// </summary>
            private Func<modelType, bool> isValue;
            /// <summary>
            /// 日志流当前结束编号
            /// </summary>
            public new cacheIdentity Cache
            {
                get
                {
                    if (isLoaded)
                    {
                        cacheIdentity cacheIdentity = new cacheIdentity { Ticks = stream.Ticks, Identity = stream.EndIdentity };
                        cacheIdentity.Values = cache.Values.getFindArray(value => isValue(value));
                        return cacheIdentity;
                    }
                    return default(cacheIdentity);
                }
            }
            /// <summary>
            /// 日志+条件过滤
            /// </summary>
            /// <param name="cache"></param>
            public where(fastCSharp.sql.cache.whole.events.cache<valueType, modelType> cache, memberCount memberCount, bool isMemberMap, Func<modelType, bool> isValue)
                : base(cache, memberCount, isMemberMap, false)
            {
                this.isValue = isValue;
                cache.OnInserted += onInserted;
                cache.OnUpdated += onUpdated;
                cache.OnDeleted += onDeleted;
                if (memberCount == null) isLoaded = true;
                else memberCount.OnLoad(this);
            }
            /// <summary>
            /// 添加数据
            /// </summary>
            /// <param name="value"></param>
            private new void onInserted(valueType value)
            {
                if (isLoaded && isValue(value)) stream.Append(new data { Type = type.Insert, Value = new fastCSharp.emit.memberMapValue<modelType, valueType>.deSerializePool { Value = value } });
            }
            /// <summary>
            /// 更新数据
            /// </summary>
            /// <param name="value"></param>
            /// <param name="value"></param>
            /// <param name="oldValue"></param>
            /// <param name="memberMap"></param>
            private new void onUpdated(valueType cacheValue, valueType value, valueType oldValue, fastCSharp.code.memberMap<modelType> memberMap)
            {
                if (isLoaded && (isValue(value) || isValue(oldValue)))
                {
                    if (isValue(value))
                    {
                        if (isValue(oldValue))
                        {
                            if (isMemberMap)
                            {
                                (memberMap = memberMap.Copy()).And(cache.MemberMap);
                                if (memberMap.IsAnyMember)
                                {
                                    fastCSharp.emit.sqlModel<modelType>.SetIdentityOrPrimaryKeyMemberMap(memberMap);
                                    stream.Append(new data { Type = type.Update, Value = new fastCSharp.emit.memberMapValue<modelType, valueType>.deSerializePool { Value = cacheValue, MemberMap = memberMap } });
                                }
                            }
                            else stream.Append(new data { Type = type.Update, Value = new fastCSharp.emit.memberMapValue<modelType, valueType>.deSerializePool { Value = cacheValue } });
                        }
                        else stream.Append(new data { Type = type.Insert, Value = new fastCSharp.emit.memberMapValue<modelType, valueType>.deSerializePool { Value = cacheValue } });
                    }
                    else if (isValue(oldValue))
                    {
                        stream.Append(new data { Type = type.Delete, Value = new fastCSharp.emit.memberMapValue<modelType, valueType>.deSerializePool { Value = oldValue, MemberMap = isMemberMap ? deleteMemberMap : null } });
                    }
                }
            }
            /// <summary>
            /// 删除数据
            /// </summary>
            /// <param name="value"></param>
            private new void onDeleted(valueType value)
            {
                if (isLoaded && isValue(value)) stream.Append(new data { Type = type.Delete, Value = new fastCSharp.emit.memberMapValue<modelType, valueType>.deSerializePool { Value = value, MemberMap = isMemberMap ? deleteMemberMap : null } });
            }
        }
    }
}
