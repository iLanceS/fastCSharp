using System;
using System.Threading;
using fastCSharp.threading;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 数组池
    /// </summary>
    /// <typeparam name="valueType"></typeparam>
    public struct arrayPool<valueType>
    {
        /// <summary>
        /// 数据集合
        /// </summary>
        internal valueType[] Array;
        /// <summary>
        /// 数据集合
        /// </summary>
        public valueType[] UnsafeArray
        {
            get { return Array; }
        }
        /// <summary>
        /// 数据数量
        /// </summary>
        internal int Count;
        /// <summary>
        /// 集合更新访问锁
        /// </summary>
        private object arrayLock;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="size">容器初始化大小</param>
        public void UnsafeCreate(int size)
        {
            if (arrayLock == null) arrayLock = new object();
            Array = new valueType[size <= 0 ? fastCSharp.config.appSetting.PoolSize : size];
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value"></param>
        public void Push(valueType value)
        {
            Monitor.Enter(arrayLock);
            if (Count == Array.Length)
            {
                try
                {
                    valueType[] newArray = new valueType[Count << 1];
                    System.Array.Copy(Array, 0, newArray, 0, Count);
                    newArray[Count] = value;
                    Array = newArray;
                    ++Count;
                }
                finally { Monitor.Exit(arrayLock); }
                return;
            }
            Array[Count++] = value;
            Monitor.Exit(arrayLock);
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="value"></param>
        /// <returns>是否存在数据</returns>
        public bool TryGet(ref valueType value)
        {
            Monitor.Enter(arrayLock);
            if (Count == 0)
            {
                Monitor.Exit(arrayLock);
                return false;
            }
            value = Array[--Count];
            Monitor.Exit(arrayLock);
            return true;
        }
        /// <summary>
        /// 清除数据集合
        /// </summary>
        /// <param name="count">保留数据数量</param>
        /// <param name="isClear">是否清除数据</param>
        internal void Clear(int count, bool isClear)
        {
            Monitor.Enter(arrayLock);
            int length = Count - count;
            if (length > 0)
            {
                if (isClear) System.Array.Clear(Array, count, length);
                Count = count;
            }
            Monitor.Exit(arrayLock);
        }
        /// <summary>
        /// 清除数据集合
        /// </summary>
        /// <param name="count">保留数据数量</param>
        /// <param name="isClear">是否清除数据</param>
        /// <returns>被清除的数据集合</returns>
        internal valueType[] GetClear(int count, bool isClear)
        {
            Monitor.Enter(arrayLock);
            int length = Count - count;
            if (length > 0)
            {
                valueType[] removeBuffers;
                try
                {
                    removeBuffers = new valueType[length];
                    System.Array.Copy(Array, Count = count, removeBuffers, 0, length);
                    if (isClear) System.Array.Clear(Array, count, length);
                }
                finally { Monitor.Exit(arrayLock); }
                return removeBuffers;
            }
            else Monitor.Exit(arrayLock);
            return nullValue<valueType>.Array;
        }
        /// <summary>
        /// 创建数组池
        /// </summary>
        /// <returns>数组池</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static arrayPool<valueType> Create()
        {
            return create(fastCSharp.config.appSetting.PoolSize);
        }
        /// <summary>
        /// 创建数组池
        /// </summary>
        /// <param name="size">容器初始化大小</param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static arrayPool<valueType> Create(int size)
        {
            return create(size <= 0 ? fastCSharp.config.appSetting.PoolSize : size);
        }
        /// <summary>
        /// 创建数组池
        /// </summary>
        /// <param name="size">容器初始化大小</param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static arrayPool<valueType> create(int size)
        {
            return new arrayPool<valueType> { Array = new valueType[size], arrayLock = new object() };
        }
    }
}
