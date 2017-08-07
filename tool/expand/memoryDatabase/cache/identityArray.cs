using System;
using fastCSharp.memoryDatabase.cache;
using fastCSharp.code;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace fastCSharp.memoryDatabase.cache
{
    /// <summary>
    /// 自增数组缓存
    /// </summary>
    /// <typeparam name="valueType"></typeparam>
    /// <typeparam name="modelType"></typeparam>
    public class identityArray<valueType, modelType> : loadCache<valueType, modelType, int>, ILoadIdentityCache<valueType, modelType>
        where valueType : class, modelType
        where modelType : class
    {
        /// <summary>
        /// 根据自增值计算数组长度
        /// </summary>
        /// <param name="identity">自增值</param>
        /// <returns>数组长度</returns>
        private static int getArrayLength(int identity)
        {
            uint value = (uint)cacheCapacity;
            while (value <= (uint)identity) value <<= 1;
            if (value == 0x80000000U)
            {
                if (identity == int.MaxValue) log.Error.Throw(log.exceptionType.IndexOutOfRange);
                return int.MaxValue;
            }
            return (int)value;
        }
        /// <summary>
        /// 数据集合
        /// </summary>
        private arrayValue[] array;
        /// <summary>
        /// 对象数量
        /// </summary>
        public int Count { get; private set; }
        /// <summary>
        /// 枚举数据集合
        /// </summary>
        public override IEnumerable<valueType> Values
        {
            get
            {
                foreach (arrayValue value in array)
                {
                    if (value.Value != null) yield return value.Value;
                }
            }
        }
        /// <summary>
        /// 获取数组
        /// </summary>
        /// <returns></returns>
        public override subArray<valueType> GetSubArray()
        {
            subArray<valueType> subArray = new subArray<valueType>(Count);
            foreach (arrayValue value in array)
            {
                if (value.Value != null) subArray.Add(value.Value);
            }
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
        /// 当前自增值
        /// </summary>
        private int currentIdentity;
        /// <summary>
        /// 获取下一个自增值
        /// </summary>
        /// <returns>自增值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public int NextIdentity()
        {
            if (((isLoaded ^ 1) | isDisposed) == 0) return Interlocked.Increment(ref currentIdentity);
            log.Error.Throw(log.exceptionType.ErrorOperation);
            return int.MinValue;
        }
        /// <summary>
        /// 是否存在关键字
        /// </summary>
        /// <param name="identity">关键字</param>
        /// <returns>是否存在对象</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool ContainsKey(int identity)
        {
            return (uint)identity < (uint)array.Length && array[identity].Value != null;
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="identity">关键字</param>
        /// <returns>数据,失败返回null</returns>
        public override valueType Get(int identity)
        {
            return (uint)identity < (uint)array.Length ? array[identity].Value : null;
        }
        /// <summary>
        /// 新建对象数组
        /// </summary>
        /// <param name="identity">自增值</param>
        private void newArray(int identity)
        {
            int length = array.Length << 1;
            arrayValue[] newArray = new arrayValue[length > identity ? length : getArrayLength(identity)];
            Array.Copy(array, 0, newArray, 0, array.Length);
            array = newArray;
        }
        /// <summary>
        /// 加载日志添加对象
        /// </summary>
        /// <param name="value">添加的对象</param>
        /// <param name="identity">自增值</param>
        /// <param name="logSize">日志字节长度</param>
        public void LoadInsert(valueType value, int identity, int logSize)
        {
            if (array == null) array = new arrayValue[getArrayLength(identity)];
            else if (identity >= array.Length) newArray(identity);
            array[identity].Set(value, logSize);
            ++Count;
            if (identity > currentIdentity) currentIdentity = identity;
        }
        /// <summary>
        /// 加载日志修改对象
        /// </summary>
        /// <param name="value">修改的对象</param>
        /// <param name="identity">自增值</param>
        /// <param name="memberMap">修改对象成员位图</param>
        public void LoadUpdate(valueType value, int identity, memberMap memberMap)
        {
            if ((uint)identity < (uint)array.Length)
            {
                valueType cacheValue = array[identity].Value;
                if (cacheValue != null) fastCSharp.emit.memberCopyer<modelType>.Copy(cacheValue, value, memberMap);
            }
        }
        /// <summary>
        /// 加载日志删除对象
        /// </summary>
        /// <param name="identity">自增值</param>
        /// <returns>日志字节长度</returns>
        public int LoadDelete(int identity)
        {
            if ((uint)identity < (uint)array.Length)
            {
                int logSize;
                valueType value = array[identity].Clear(out logSize);
                if (value != null)
                {
                    --Count;
                    return logSize;
                }
            }
            return 0;
        }
        /// <summary>
        /// 日志数据加载完成
        /// </summary>
        /// <param name="isLoaded">是否加载成功</param>
        public void Loaded(bool isLoaded)
        {
            try
            {
                if (isLoaded)
                {
                    if (array == null) array = new arrayValue[cacheCapacity];
                }
                else Dispose();
            }
            catch (Exception error)
            {
                log.Error.Add(error, null, false);
                Dispose();
            }
            finally { onLoaded(); }
        }
        /// <summary>
        /// 添加对象
        /// </summary>
        /// <param name="value">添加的对象</param>
        /// <param name="identity">自增值</param>
        /// <param name="logSize">日志字节长度</param>
        /// <param name="isCopy">是否浅复制缓存对象值,否则返回缓存对象</param>
        /// <returns>添加的对象</returns>
        protected override valueType insert(valueType value, int identity, int logSize, bool isCopy)
        {
            if (identity >= array.Length) newArray(identity);
            if (isCopy) value = value.Clone();
            array[identity].Set(value, logSize);
            ++Count;
            return value;
        }
        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="identity">自增值</param>
        /// <param name="logSize">日志字节长度</param>
        /// <returns>被删除对象,失败返回null</returns>
        protected override valueType delete(int identity, out int logSize)
        {
            if ((uint)identity < (uint)array.Length)
            {
                valueType value = array[identity].Clear(out logSize);
                if (value != null)
                {
                    --Count;
                    return value;
                }
            }
            logSize = 0;
            return null;
        }
    }
    /// <summary>
    /// 自增数组缓存
    /// </summary>
    /// <typeparam name="modelType"></typeparam>
    public sealed class identityArray<modelType> : identityArray<modelType, modelType>
        where modelType : class
    {
    }
}
