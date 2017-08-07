using System;
using System.Threading;
using fastCSharp.code;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace fastCSharp.memoryDatabase.cache
{
#if NOJIT
    /// <summary>
    /// 数据加载基本缓存接口
    /// </summary>
    public interface ILoadCache : IDisposable
    {
        /// <summary>
        /// 对象数量
        /// </summary>
        int Count { get; }
        /// <summary>
        /// 枚举数据集合
        /// </summary>
        IEnumerable<object> Values { get; }
        /// <summary>
        /// 获取数据集合
        /// </summary>
        /// <returns></returns>
        subArray<object> GetSubArray();
        /// <summary>
        /// 获取数据集合
        /// </summary>
        /// <returns></returns>
        object[] GetArray();
        /// <summary>
        /// 日志数据加载完成
        /// </summary>
        /// <param name="isLoaded">是否加载成功</param>
        void Loaded(bool isLoaded);
        /// <summary>
        /// 等待缓存加载结束
        /// </summary>
        void WaitLoad();
        /// <summary>
        /// 添加对象事件
        /// </summary>
        event Action<object> OnInserted;
        /// <summary>
        /// 修改对象事件[修改后的值,对象原值,更新成员位图]
        /// </summary>
        event Action<object, object, memberMap> OnUpdated;
        /// <summary>
        /// 删除对象事件
        /// </summary>
        event Action<object> OnDeleted;
    }
    /// <summary>
    /// 数据加载基本缓存接口
    /// </summary>
    public interface ILoadCacheKey : ILoadCache, IDisposable
    {
        /// <summary>
        /// 是否存在关键字
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>是否存在对象</returns>
        bool ContainsKey(object key);
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>数据,失败返回null</returns>
        object Get(object key);
        /// <summary>
        /// 加载日志添加对象
        /// </summary>
        /// <param name="value">添加的对象</param>
        /// <param name="key">关键字</param>
        /// <param name="logSize">日志字节长度</param>
        void LoadInsert(object value, object key, int logSize);
        /// <summary>
        /// 加载日志修改对象
        /// </summary>
        /// <param name="value">修改的对象</param>
        /// <param name="key">关键字</param>
        /// <param name="memberMap">修改对象成员位图</param>
        void LoadUpdate(object value, object key, memberMap memberMap);
        /// <summary>
        /// 加载日志删除对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>日志字节长度</returns>
        int LoadDelete(object key);
        /// <summary>
        /// 添加对象
        /// </summary>
        /// <param name="value">添加的对象</param>
        /// <param name="key">关键字</param>
        /// <param name="logSize">日志字节长度</param>
        /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
        /// <returns>添加的对象</returns>
        object Insert(object value, object key, int logSize, bool isCopy);
        /// <summary>
        /// 修改对象
        /// </summary>
        /// <param name="value">修改的对象</param>
        /// <param name="key">关键字</param>
        /// <param name="memberMap">修改对象成员位图</param>
        /// <returns>修改后的对象</returns>
        object Update(object value, object key, memberMap memberMap);
        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="logSize">日志字节长度</param>
        /// <returns>被删除的对象值,日志字节长度</returns>
        object Delete(object key, out int logSize);
    }
    /// <summary>
    /// 自增数据加载基本缓存接口
    /// </summary>
    /// <typeparam name="valueType">对象类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    public interface ILoadIdentityCache : ILoadCacheKey
    {
        /// <summary>
        /// 获取下一个自增值
        /// </summary>
        /// <returns>自增值</returns>
        int NextIdentity();
    }
#else
    /// <summary>
    /// 数据加载基本缓存接口
    /// </summary>
    /// <typeparam name="valueType">对象类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    public interface ILoadCache<valueType, modelType> : IDisposable
        where valueType : class, modelType
        where modelType : class
    {
        /// <summary>
        /// 对象数量
        /// </summary>
        int Count { get; }
        /// <summary>
        /// 枚举数据集合
        /// </summary>
        IEnumerable<valueType> Values { get; }
        /// <summary>
        /// 获取数据集合
        /// </summary>
        /// <returns></returns>
        subArray<valueType> GetSubArray();
        /// <summary>
        /// 获取数据集合
        /// </summary>
        /// <returns></returns>
        valueType[] GetArray();
        /// <summary>
        /// 日志数据加载完成
        /// </summary>
        /// <param name="isLoaded">是否加载成功</param>
        void Loaded(bool isLoaded);
        /// <summary>
        /// 等待缓存加载结束
        /// </summary>
        void WaitLoad();
        /// <summary>
        /// 添加对象事件
        /// </summary>
        event Action<valueType> OnInserted;
        /// <summary>
        /// 修改对象事件[修改后的值,对象原值,更新成员位图]
        /// </summary>
        event Action<valueType, valueType, memberMap> OnUpdated;
        /// <summary>
        /// 删除对象事件
        /// </summary>
        event Action<valueType> OnDeleted;
    }
    /// <summary>
    /// 数据加载基本缓存接口
    /// </summary>
    /// <typeparam name="valueType">对象类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    public interface ILoadCache<valueType, modelType, keyType> : ILoadCache<valueType, modelType>, IDisposable
        where valueType : class, modelType
        where modelType : class
    {
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
        /// <returns>修改后的对象</returns>
        valueType Update(valueType value, keyType key, memberMap memberMap);
        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="logSize">日志字节长度</param>
        /// <returns>被删除的对象值,日志字节长度</returns>
        valueType Delete(keyType key, out int logSize);
    }
    /// <summary>
    /// 自增数据加载基本缓存接口
    /// </summary>
    /// <typeparam name="valueType">对象类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    public interface ILoadIdentityCache<valueType, modelType> : ILoadCache<valueType, modelType, int>
        where valueType : class, modelType
        where modelType : class
    {
        /// <summary>
        /// 获取下一个自增值
        /// </summary>
        /// <returns>自增值</returns>
        int NextIdentity();
    }
#endif
    /// <summary>
    /// 数据加载基本缓存
    /// </summary>
    public abstract class loadCache : IDisposable
    {
        /// <summary>
        /// 自增缓存数组默认容器尺寸
        /// </summary>
        protected readonly static int cacheCapacity = fastCSharp.config.memoryDatabase.Default.CacheCapacity;
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
        /// 等待缓存加载结束
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void WaitLoad()
        {
            while (isLoaded == 0) Thread.Sleep(1);
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
    public abstract class loadCache<valueType> : loadCache
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
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
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
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public valueType Clear(out int logSize)
            {
                valueType value = Value;
                logSize = LogSize;
                Value = null;
                return value;
            }
        }
        /// <summary>
        /// 缓存数据
        /// </summary>
        protected sealed class cacheValue
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
            /// 获取数据委托
            /// </summary>
            public static readonly Func<cacheValue, valueType> GetValue = getValue;
            /// <summary>
            /// 获取数据
            /// </summary>
            /// <param name="value"></param>
            private static valueType getValue(cacheValue value)
            {
                return value.Value;
            }
        }
        /// <summary>
        /// 枚举数据集合
        /// </summary>
        public abstract IEnumerable<valueType> Values { get; }
        /// <summary>
        /// 获取数据集合
        /// </summary>
        /// <returns></returns>
        public abstract subArray<valueType> GetSubArray();
        /// <summary>
        /// 获取数据集合
        /// </summary>
        /// <returns></returns>
        public abstract valueType[] GetArray();
    }
    /// <summary>
    /// 数据加载基本缓存
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    public abstract class loadCache<valueType, modelType, keyType> : loadCache<valueType>
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
                    fastCSharp.emit.memberCopyer<modelType>.Copy(oldValue, cacheValue);
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
