using System;
using System.Threading;
using fastCSharp.code;

namespace fastCSharp.memoryDatabase.cache
{
    /// <summary>
    /// 数据加载基本缓存接口
    /// </summary>
    public interface ILoadCache : IDisposable
    {
        /// <summary>
        /// 日志数据加载完成
        /// </summary>
        /// <param name="isLoaded">是否加载成功</param>
        void Loaded(bool isLoaded);
    }
    /// <summary>
    /// 数据加载基本缓存接口
    /// </summary>
    /// <typeparam name="valueType">对象类型</typeparam>
    /// <typeparam name="memberType">数据成员类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    public interface ILoadCache<valueType, memberType, keyType> : ILoadCache
        where valueType : fastCSharp.data.IPrimaryKey<keyType>
        where memberType : fastCSharp.code.cSharp.IMemberMap<memberType>
    {
        /// <summary>
        /// 获取当前数据集合
        /// </summary>
        valueType[] Array { get; }
        /// <summary>
        /// 加载日志添加对象
        /// </summary>
        /// <param name="value">添加的对象</param>
        /// <param name="logSize">日志字节长度</param>
        void LoadInsert(valueType value, int logSize);
        /// <summary>
        /// 加载日志修改对象
        /// </summary>
        /// <param name="value">修改的对象</param>
        /// <param name="memberMap">修改对象成员位图</param>
        void LoadUpdate(valueType value, memberType memberMap);
        /// <summary>
        /// 加载日志删除对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>日志字节长度</returns>
        int LoadDelete(keyType key);
        /// <summary>
        /// 获取操作队列锁
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>操作队列锁</returns>
        object GetLock(keyType key);
        /// <summary>
        /// 添加对象
        /// </summary>
        /// <param name="value">添加的对象</param>
        /// <param name="logSize">日志字节长度</param>
        /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
        /// <returns>添加的对象</returns>
        valueType Insert(valueType value, int logSize, bool isCopy);
        /// <summary>
        /// 修改对象
        /// </summary>
        /// <param name="value">修改的对象</param>
        /// <param name="memberMap">修改对象成员位图</param>
        /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
        /// <returns>修改后的对象</returns>
        valueType Update(valueType value, memberType memberMap, bool isCopy);
        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>被删除的对象值,日志字节长度</returns>
        keyValue<valueType, int> Delete(keyType key);
        /// <summary>
        /// 是否存在关键字
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>是否存在对象</returns>
        bool ContainsKey(keyType key);
    }
    /// <summary>
    /// 自增数据加载基本缓存接口
    /// </summary>
    /// <typeparam name="valueType">对象类型</typeparam>
    /// <typeparam name="memberType">数据成员类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    public interface ILoadIdentityCache<valueType, memberType, keyType> : ILoadCache<valueType, memberType, keyType>
        where valueType : fastCSharp.data.IPrimaryKey<keyType>
        where memberType : fastCSharp.code.cSharp.IMemberMap<memberType>
        where keyType : IEquatable<keyType>
    {
        /// <summary>
        /// 获取下一个自增值
        /// </summary>
        /// <returns>自增值</returns>
        keyType NextIdentity();
        /// <summary>
        /// 取消自增值
        /// </summary>
        /// <param name="key">自增值</param>
        void CancelIdentity(keyType key);
    }
    /// <summary>
    /// 数据加载基本缓存
    /// </summary>
    public abstract class loadCache
    {
        /// <summary>
        /// 默认自增数组长度
        /// </summary>
        protected readonly static int identityArrayLength = fastCSharp.config.memoryDatabase.Default.IdentityCacheArraySize;
        /// <summary>
        /// 删除状态锁
        /// </summary>
        protected readonly static object deleteLock = new object();
        /// <summary>
        /// 缓存操作锁
        /// </summary>
        protected readonly object cacheLock = new object();
        /// <summary>
        /// 缓存操作锁
        /// </summary>
        public object CacheLock { get { return cacheLock; } }
        /// <summary>
        /// 加载等待事件
        /// </summary>
        private readonly EventWaitHandle loadWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        /// <summary>
        /// 是否加载完成
        /// </summary>
        protected bool isLoaded;
        /// <summary>
        /// 是否已经释放资源
        /// </summary>
        protected int isDisposed;
        /// <summary>
        /// 日志数据成功加载完成事件
        /// </summary>
        public event Action OnLoaded;
        /// <summary>
        /// 日志数据加载完成
        /// </summary>
        /// <param name="isLoaded">是否加载成功</param>
        public void Loaded(bool isLoaded)
        {
            if (!this.isLoaded)
            {
                if (isLoaded)
                {
                    try
                    {
                        loaded();
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, null, false);
                        isLoaded = false;
                    }
                }
                if (isLoaded)
                {
                    try
                    {
                        if (OnLoaded != null) OnLoaded();
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, null, false);
                    }
                }
                else dispose();
                this.isLoaded = true;
                loadWaitHandle.Set();
            }
        }
        /// <summary>
        /// 日志数据加载完成
        /// </summary>
        protected abstract void loaded();
        /// <summary>
        /// 等待加载完成
        /// </summary>
        protected void waitLoad()
        {
            if (!isLoaded && isDisposed == 0) loadWaitHandle.WaitOne();
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.Increment(ref isDisposed) == 1)
            {
                Monitor.Enter(cacheLock);
                try
                {
                    dispose();
                }
                finally
                {
                    Monitor.Exit(cacheLock);
                    loadWaitHandle.Set();
                    loadWaitHandle.Close();
                }
            }
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        protected abstract void dispose();
    }
    /// <summary>
    /// 数据加载基本缓存
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    public abstract class loadCache<valueType> : loadCache, ILoadCache where valueType : class
    {
        /// <summary>
        /// 关键字操作锁
        /// </summary>
        protected struct valueLock
        {
            /// <summary>
            /// 数据对象
            /// </summary>
            public valueType Value;
            /// <summary>
            /// 关键字操作锁
            /// </summary>
            public object Lock;
            /// <summary>
            /// 日志字节长度
            /// </summary>
            public int LogSize;
            /// <summary>
            /// 是否删除
            /// </summary>
            public bool IsDelete
            {
                get { return Value == null || Lock == deleteLock; }
            }
            /// <summary>
            /// 是否有效(没有删除)
            /// </summary>
            public bool NotDelete
            {
                get
                {
                    return Value != null && Lock != deleteLock;
                }
            }
            /// <summary>
            /// 设置数据对象
            /// </summary>
            /// <param name="value">数据对象</param>
            /// <param name="logSize">日志字节长度</param>
            public void Set(valueType value, int logSize)
            {
                Value = value;
                LogSize = logSize;
                Lock = null;
            }
            /// <summary>
            /// 设置数据对象
            /// </summary>
            /// <param name="value">数据对象</param>
            /// <param name="logSize">日志字节长度</param>
            /// <returns>是否设置成功</returns>
            public int SetIfNull(valueType value, int logSize)
            {
                if (Value == null)
                {
                    Value = value;
                    LogSize = logSize;
                    return 1;
                }
                return 0;
            }
            /// <summary>
            /// 清除数据
            /// </summary>
            /// <returns>是否清除成功+日志字节长度</returns>
            public keyValue<int, int> Clear()
            {
                Lock = null;
                if (Value != null)
                {
                    Value = null;
                    return new keyValue<int, int>(1, LogSize);
                }
                return default(keyValue<int, int>);
            }
            /// <summary>
            /// 清除数据
            /// </summary>
            /// <returns>被清除数据</returns>
            public keyValue<valueType, int> Remove()
            {
                valueType value = Value;
                int logSize = LogSize;
                Lock = null;
                Value = null;
                LogSize = 0;
                return new keyValue<valueType, int>(value, logSize);
            }
            /// <summary>
            /// 删除数据
            /// </summary>
            /// <returns>被删除数据+日志字节长度</returns>
            public keyValue<valueType, int> Delete()
            {
                Lock = deleteLock;
                return new keyValue<valueType,int>(Value, LogSize);
            }
            /// <summary>
            /// 获取关键字操作锁
            /// </summary>
            /// <returns>关键字操作锁</returns>
            public object GetLock()
            {
                return Value != null ? (Lock ?? (Lock = new object())) : null;
            }
        }
        /// <summary>
        /// 获取当前数据集合
        /// </summary>
        public valueType[] Array
        {
            get
            {
                waitLoad();
                return getArray();
            }
        }
        /// <summary>
        /// 获取当前数据集合
        /// </summary>
        protected abstract valueType[] getArray();
        /// <summary>
        /// 加载日志添加对象
        /// </summary>
        /// <param name="value">添加的对象</param>
        /// <param name="logSize">日志字节长度</param>
        public void LoadInsert(valueType value, int logSize)
        {
            if (!isLoaded && isDisposed == 0) loadInsert(value, logSize);
        }
        /// <summary>
        /// 加载日志添加对象
        /// </summary>
        /// <param name="value">添加的对象</param>
        /// <param name="logSize">日志字节长度</param>
        protected abstract void loadInsert(valueType value, int logSize);
        /// <summary>
        /// 添加对象事件
        /// </summary>
        public event Action<valueType> OnInserted;
        /// <summary>
        /// 添加对象
        /// </summary>
        /// <param name="value">添加的对象</param>
        /// <param name="logSize">日志字节长度</param>
        /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
        /// <returns>添加的对象</returns>
        public valueType Insert(valueType value, int logSize, bool isCopy)
        {
            waitLoad();
            insert(value, logSize);
            Action<valueType> onInserted = OnInserted;
            if (onInserted != null) onInserted(value);
            return isCopy ? fastCSharp.emit.memberCopyer<valueType>.MemberwiseClone(value) : value;
        }
        /// <summary>
        /// 添加对象
        /// </summary>
        /// <param name="value">添加的对象</param>
        /// <param name="logSize">日志字节长度</param>
        protected abstract void insert(valueType value, int logSize);
    }
    /// <summary>
    /// 数据加载基本缓存
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    /// <typeparam name="memberType">数据成员类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    public abstract class loadCache<valueType, memberType, keyType> : loadCache<valueType>, ILoadCache<valueType, memberType, keyType>
        where valueType : class, fastCSharp.data.IPrimaryKey<keyType>
        where memberType : fastCSharp.code.cSharp.IMemberMap<memberType>
    {
        /// <summary>
        /// 复制数据
        /// </summary>
        /// <param name="value">目标数据</param>
        /// <param name="copyValue">被复制数据</param>
        /// <param name="memberMap">复制成员位图</param>
        protected abstract void copy(valueType value, valueType copyValue, memberType memberMap);
        /// <summary>
        /// 加载日志修改对象
        /// </summary>
        /// <param name="value">修改的对象</param>
        /// <param name="memberMap">修改对象成员位图</param>
        public void LoadUpdate(valueType value, memberType memberMap)
        {
            if (!isLoaded) loadUpdate(value, memberMap);
        }
        /// <summary>
        /// 加载日志修改对象
        /// </summary>
        /// <param name="value">修改的对象</param>
        /// <param name="memberMap">修改对象成员位图</param>
        protected abstract void loadUpdate(valueType value, memberType memberMap);
        /// <summary>
        /// 加载日志删除对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>日志字节长度</returns>
        public int LoadDelete(keyType key)
        {
            return isLoaded ? 0 : loadDelete(key);
        }
        /// <summary>
        /// 加载日志删除对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>日志字节长度</returns>
        protected abstract int loadDelete(keyType key);
        /// <summary>
        /// 获取操作队列锁
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>操作队列锁,失败返回null</returns>
        public object GetLock(keyType key)
        {
            waitLoad();
            return getLock(key);
        }
        /// <summary>
        /// 获取操作队列锁
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>操作队列锁,失败返回null</returns>
        protected abstract object getLock(keyType key);
        /// <summary>
        /// 修改对象事件
        /// </summary>
        public event Action<valueType> OnUpdated;
        /// <summary>
        /// 修改对象事件<修改后的值,对象原值>
        /// </summary>
        public event Action<valueType, valueType> OnUpdated2;
        /// <summary>
        /// 修改对象
        /// </summary>
        /// <param name="value">修改的对象</param>
        /// <param name="memberMap">修改对象成员位图</param>
        /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
        /// <returns>修改后的对象值</returns>
        public valueType Update(valueType value, memberType memberMap, bool isCopy)
        {
            waitLoad();
            Action<valueType, valueType> onUpdated2 = OnUpdated2;
            if (onUpdated2 == null)
            {
                if ((value = update(value, memberMap)) != null)
                {
                    Action<valueType> onUpdated = OnUpdated;
                    if (onUpdated != null) onUpdated(value);
                }
            }
            else
            {
                keyValue<valueType, valueType> cacheValue = update2(value, memberMap);
                if (cacheValue.Key != null)
                {
                    Action<valueType> onUpdated = OnUpdated;
                    if (onUpdated != null) onUpdated(cacheValue.Key);
                    onUpdated2(cacheValue.Key, cacheValue.Value);
                }
                return cacheValue.Key;
            }
            return isCopy ? fastCSharp.emit.memberCopyer<valueType>.MemberwiseClone(value) : value;
        }
        /// <summary>
        /// 修改对象
        /// </summary>
        /// <param name="value">修改的对象</param>
        /// <param name="memberMap">修改对象成员位图</param>
        /// <returns>修改后的对象值</returns>
        protected abstract valueType update(valueType value, memberType memberMap);
        /// <summary>
        /// 修改对象
        /// </summary>
        /// <param name="value">修改的对象</param>
        /// <param name="memberMap">修改对象成员位图</param>
        /// <returns>修改后的对象值</returns>
        protected abstract keyValue<valueType, valueType> update2(valueType value, memberType memberMap);
        /// <summary>
        /// 删除对象事件
        /// </summary>
        public event Action<valueType> OnDeleted;
        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>被删除对象+日志字节长度,失败返回null</returns>
        public keyValue<valueType, int> Delete(keyType key)
        {
            waitLoad();
            keyValue<valueType, int> value = delete(key);
            if (value.Key != null)
            {
                Action<valueType> onDeleted = OnDeleted;
                if (onDeleted != null) onDeleted(value.Key);
            }
            return value;
        }
        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>被删除对象+日志字节长度,失败返回null</returns>
        protected abstract keyValue<valueType, int> delete(keyType key);
        /// <summary>
        /// 是否存在关键字
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>是否存在对象</returns>
        public bool ContainsKey(keyType key)
        {
            waitLoad();
            valueType value = null;
            return get(key, ref value);
        }
        /// <summary>
        /// 根据关键字获取数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="defaultValue">数据对象</param>
        /// <returns>是否存在对象</returns>
        public bool TryGetValue(keyType key, out valueType defaultValue)
        {
            waitLoad();
            valueType value = null;
            if (get(key, ref value))
            {
                defaultValue = value;
                return true;
            }
            defaultValue = null;
            return false;
        }
        /// <summary>
        /// 根据关键字获取数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="defaultValue">默认空值</param>
        /// <returns>数据对象</returns>
        public valueType Get(keyType key, valueType defaultValue)
        {
            waitLoad();
            get(key, ref defaultValue);
            return defaultValue;
        }
        /// <summary>
        /// 根据关键字获取数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">默认空值</param>
        /// <returns>数据对象</returns>
        protected abstract bool get(keyType key, ref valueType value);
    }

    /// <summary>
    /// 数据加载基本缓存接口
    /// </summary>
    /// <typeparam name="valueType">对象类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    public interface ILoadCacheNew<valueType, modelType, keyType> : IDisposable
        where valueType : class, modelType
        where modelType : class
        where keyType : IEquatable<keyType>
    {
        /// <summary>
        /// 加载日志添加对象
        /// </summary>
        /// <param name="value">添加的对象</param>
        /// <param name="key">关键字</param>
        /// <param name="logSize">日志字节长度</param>
        void LoadInsert(valueType value, keyType key, int logSize);
        /// <summary>
        /// 加载日志修改对象
        /// </summary>
        /// <param name="value">修改的对象</param>
        /// <param name="key">关键字</param>
        /// <param name="memberMap">修改对象成员位图</param>
        void LoadUpdate(valueType value, keyType key, memberMap memberMap);
        /// <summary>
        /// 加载日志删除对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>日志字节长度</returns>
        int LoadDelete(keyType key);
        /// <summary>
        /// 日志数据加载完成
        /// </summary>
        /// <param name="isLoaded">是否加载成功</param>
        void Loaded(bool isLoaded);
        /// <summary>
        /// 添加对象
        /// </summary>
        /// <param name="value">添加的对象</param>
        /// <param name="key">关键字</param>
        /// <param name="logSize">日志字节长度</param>
        /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
        /// <returns>添加的对象</returns>
        valueType Insert(valueType value, keyType key, int logSize, bool isCopy);
        /// <summary>
        /// 修改对象
        /// </summary>
        /// <param name="value">修改的对象</param>
        /// <param name="key">关键字</param>
        /// <param name="memberMap">修改对象成员位图</param>
        /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
        /// <returns>修改后的对象</returns>
        valueType Update(valueType value, keyType key, memberMap memberMap);
        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="logSize">日志字节长度</param>
        /// <returns>被删除的对象值,日志字节长度</returns>
        valueType Delete(keyType key, out int logSize);
        /// <summary>
        /// 是否存在关键字
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>是否存在对象</returns>
        bool ContainsKey(keyType key);
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>数据,失败返回null</returns>
        valueType Get(keyType key);
    }
    /// <summary>
    /// 自增数据加载基本缓存接口
    /// </summary>
    /// <typeparam name="valueType">对象类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    public interface ILoadIdentityCacheNew<valueType, modelType> : ILoadCacheNew<valueType, modelType, int>
        where valueType : class, modelType
        where modelType : class
    {
        /// <summary>
        /// 获取下一个自增值
        /// </summary>
        /// <returns>自增值</returns>
        int NextIdentity();
    }
    /// <summary>
    /// 数据加载基本缓存
    /// </summary>
    public abstract class loadCacheNew : IDisposable
    {
        /// <summary>
        /// 是否加载完成
        /// </summary>
        protected int isLoaded;
        /// <summary>
        /// 是否已经释放资源
        /// </summary>
        protected int isDisposed;
        /// <summary>
        /// 日志数据成功加载完成事件
        /// </summary>
        public event Action OnLoaded;
        /// <summary>
        /// 日志数据成功加载完成事件
        /// </summary>
        protected void onLoaded()
        {
            if (Interlocked.CompareExchange(ref isLoaded, 1, 0) == 0)
            {
                if (OnLoaded != null) OnLoaded();
            }
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.Increment(ref isDisposed) == 1) dispose();
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        protected virtual void dispose()
        {
            onLoaded();
        }
    }
    /// <summary>
    /// 数据加载基本缓存
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    public abstract class loadCacheNew<valueType> : loadCacheNew
        where valueType : class
    {
        /// <summary>
        /// 数据对象
        /// </summary>
        protected struct arrayValue
        {
            /// <summary>
            /// 数据对象
            /// </summary>
            public valueType Value;
            /// <summary>
            /// 日志字节长度
            /// </summary>
            public int LogSize;
            /// <summary>
            /// 关键字操作锁
            /// </summary>
            public int Lock;
            /// <summary>
            /// 设置数据对象
            /// </summary>
            /// <param name="value"></param>
            /// <param name="logSize"></param>
            public void Set(valueType value, int logSize)
            {
                Value = value;
                LogSize = logSize;
            }
            /// <summary>
            /// 删除数据
            /// </summary>
            /// <param name="logSize"></param>
            /// <returns></returns>
            public valueType Clear(out int logSize)
            {
                valueType value = Value;
                logSize = LogSize;
                Value = null;
                return value;
            }
        }
    }
    /// <summary>
    /// 数据加载基本缓存
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    public abstract class loadCacheNew<valueType, modelType, keyType> : loadCacheNew<valueType>
        where valueType : class, modelType
    {
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>数据,失败返回null</returns>
        public abstract valueType Get(keyType key);
        /// <summary>
        /// 添加对象事件
        /// </summary>
        public event Action<valueType> OnInserted;
        /// <summary>
        /// 添加对象
        /// </summary>
        /// <param name="value">添加的对象</param>
        /// <param name="key">关键字</param>
        /// <param name="logSize">日志字节长度</param>
        /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
        /// <returns>添加的对象</returns>
        public valueType Insert(valueType value, keyType key, int logSize, bool isCopy)
        {
            if (((isLoaded ^ 1) | isDisposed) == 0 && (value = insert(value, key, logSize, isCopy)) != null)
            {
                if (OnInserted != null) OnInserted(value);
                return value;
            }
            return null;
        }
        /// <summary>
        /// 添加对象
        /// </summary>
        /// <param name="value">添加的对象</param>
        /// <param name="key">关键字</param>
        /// <param name="logSize">日志字节长度</param>
        /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
        /// <returns>添加的对象</returns>
        protected abstract valueType insert(valueType value, keyType key, int logSize, bool isCopy);
        /// <summary>
        /// 修改对象事件[修改后的值,对象原值,更新成员位图]
        /// </summary>
        public event Action<valueType, valueType, memberMap> OnUpdated;
        /// <summary>
        /// 修改对象
        /// </summary>
        /// <param name="value">修改的对象</param>
        /// <param name="key">关键字</param>
        /// <param name="memberMap">修改对象成员位图</param>
        /// <returns>修改后的对象值</returns>
        public valueType Update(valueType value, keyType key, memberMap memberMap)
        {
            if (((isLoaded ^ 1) | isDisposed) == 0)
            {
                valueType cacheValue = Get(key);
                if (cacheValue != null)
                {
                    valueType oldValue = fastCSharp.emit.constructor<valueType>.New();
                    fastCSharp.emit.memberCopyer<modelType>.Copy(oldValue, cacheValue, memberMap);
                    fastCSharp.emit.memberCopyer<modelType>.Copy(cacheValue, value, memberMap);
                    if (OnUpdated != null) OnUpdated(cacheValue, oldValue, memberMap);
                    return cacheValue;
                }
            }
            return null;
        }
        /// <summary>
        /// 删除对象事件
        /// </summary>
        public event Action<valueType> OnDeleted;
        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="logSize">日志字节长度</param>
        /// <returns>被删除对象,失败返回null</returns>
        public valueType Delete(keyType key, out int logSize)
        {
            if (((isLoaded ^ 1) | isDisposed) == 0)
            {
                valueType value = delete(key, out logSize);
                if (value != null)
                {
                    if (OnDeleted != null) OnDeleted(value);
                    return value;
                }
            }
            logSize = 0;
            return null;
        }
        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="logSize">日志字节长度</param>
        /// <returns>被删除对象,失败返回null</returns>
        protected abstract valueType delete(keyType key, out int logSize);
    }
}
