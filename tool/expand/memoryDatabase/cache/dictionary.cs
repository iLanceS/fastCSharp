using System;
using System.Collections.Generic;
using fastCSharp.code;
using System.Runtime.CompilerServices;

namespace fastCSharp.memoryDatabase.cache
{
    /// <summary>
    /// 字典缓存
    /// </summary>
    /// <typeparam name="valueType"></typeparam>
    /// <typeparam name="modelType"></typeparam>
    /// <typeparam name="keyType"></typeparam>
    public class dictionary<valueType, modelType, keyType> : loadCache<valueType, modelType, keyType>, ILoadCache<valueType, modelType, keyType>
        where valueType : class, modelType
        where modelType : class
        where keyType : IEquatable<keyType>
    {
        /// <summary>
        /// 数据集合
        /// </summary>
        private Dictionary<keyType, cacheValue> values = dictionary<keyType>.Create<cacheValue>(cacheCapacity);
        /// <summary>
        /// 对象数量
        /// </summary>
        public int Count { get { return values.Count; } }
        /// <summary>
        /// 枚举数据集合
        /// </summary>
        public override IEnumerable<valueType> Values
        {
            get
            {
                foreach (cacheValue value in values.Values) yield return value.Value;
            }
        }
        /// <summary>
        /// 获取数组
        /// </summary>
        /// <returns></returns>
        public override subArray<valueType> GetSubArray()
        {
            subArray<valueType> subArray = new subArray<valueType>(Count);
            foreach (cacheValue value in values.Values) subArray.Add(value.Value);
            return subArray;
        }
        /// <summary>
        /// 获取数组
        /// </summary>
        /// <returns></returns>
        public override valueType[] GetArray()
        {
            return GetSubArray().ToArray();
        }
        /// <summary>
        /// 是否存在关键字
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>是否存在对象</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool ContainsKey(keyType key)
        {
            return values.ContainsKey(key);
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>数据,失败返回null</returns>
        public override valueType Get(keyType key)
        {
            cacheValue value;
            return values.TryGetValue(key, out value) ? value.Value : null;
        }
        /// <summary>
        /// 加载日志添加对象
        /// </summary>
        /// <param name="value">添加的对象</param>
        /// <param name="key"></param>
        /// <param name="logSize">日志字节长度</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void LoadInsert(valueType value, keyType key, int logSize)
        {
            values[key] = new cacheValue { Value = value, LogSize = logSize };
        }
        /// <summary>
        /// 加载日志修改对象
        /// </summary>
        /// <param name="value">修改的对象</param>
        /// <param name="key"></param>
        /// <param name="memberMap">修改对象成员位图</param>
        public void LoadUpdate(valueType value, keyType key, memberMap memberMap)
        {
            cacheValue cacheValue;
            if (values.TryGetValue(key, out cacheValue))
            {
                fastCSharp.emit.memberCopyer<modelType>.Copy(cacheValue.Value, value, memberMap);
            }
        }
        /// <summary>
        /// 加载日志删除对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns>日志字节长度</returns>
        public int LoadDelete(keyType key)
        {
            cacheValue cacheValue;
            if (values.TryGetValue(key, out cacheValue))
            {
                values.Remove(key);
                return cacheValue.LogSize;
            }
            return 0;
        }
        /// <summary>
        /// 日志数据加载完成
        /// </summary>
        /// <param name="isLoaded">是否加载成功</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Loaded(bool isLoaded)
        {
            if (!isLoaded) Dispose();
            onLoaded();
        }
        /// <summary>
        /// 添加对象
        /// </summary>
        /// <param name="value">添加的对象</param>
        /// <param name="key"></param>
        /// <param name="logSize">日志字节长度</param>
        /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
        /// <returns>添加的对象</returns>
        protected override valueType insert(valueType value, keyType key, int logSize, bool isCopy)
        {
            if (isCopy) value = value.Clone();
            values[key] = new cacheValue { Value = value, LogSize = logSize };
            return value;
        }
        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="logSize">日志字节长度</param>
        /// <returns>被删除对象,失败返回null</returns>
        protected override valueType delete(keyType key, out int logSize)
        {
            cacheValue cacheValue;
            if (values.TryGetValue(key, out cacheValue))
            {
                values.Remove(key);
                logSize = cacheValue.LogSize;
                return cacheValue.Value;
            }
            logSize = 0;
            return null;
        }
    }
    /// <summary>
    /// 字典缓存
    /// </summary>
    /// <typeparam name="modelType"></typeparam>
    /// <typeparam name="keyType"></typeparam>
    public sealed class dictionary<modelType, keyType> : dictionary<modelType, modelType, keyType>
        where modelType : class
        where keyType : IEquatable<keyType>
    {
    }
}
