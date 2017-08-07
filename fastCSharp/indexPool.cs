using System;
using System.Threading;
using fastCSharp.threading;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 索引池
    /// </summary>
    /// <typeparam name="valueType">对象类型</typeparam>
    public unsafe sealed class indexPool<valueType> : IDisposable where valueType : class
    {
        /// <summary>
        /// 对象池
        /// </summary>
        private valueType[] pool = new valueType[fastCSharp.config.appSetting.PoolSize];
        /// <summary>
        /// 对象池访问锁
        /// </summary>
        private readonly object poolLock = new object();
        /// <summary>
        /// 空闲索引集合
        /// </summary>
        private pointer.size freeIndexs;
        /// <summary>
        /// 当前空闲索引
        /// </summary>
        private int* currentFreeIndex;
        /// <summary>
        /// 空闲索引集合结束位置
        /// </summary>
        private byte* endFreeIndex;
        /// <summary>
        /// 对象池当前索引位置
        /// </summary>
        private int poolIndex;
        /// <summary>
        /// 索引池
        /// </summary>
        public indexPool()
        {
            int size = fastCSharp.config.appSetting.PoolSize * sizeof(int);
            currentFreeIndex = (freeIndexs = unmanaged.Get(size, false)).Int;
            endFreeIndex = freeIndexs.Byte + size;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (currentFreeIndex != null)
            {
                Monitor.Enter(poolLock);
                pointer.size indexs = freeIndexs;
                pool = nullValue<valueType>.Array;
                poolIndex = 0;
                currentFreeIndex = null;
                endFreeIndex = null;
                freeIndexs.Null();
                Monitor.Exit(poolLock);
                unmanaged.Free(ref indexs);
            }
        }
        /// <summary>
        /// 对象入池
        /// </summary>
        /// <param name="value"></param>
        /// <returns>索引位置,失败返回-1</returns>
        public int Push(valueType value)
        {
            int index;
            Monitor.Enter(poolLock);
            if (currentFreeIndex == freeIndexs.data)
            {
                if (poolIndex == pool.Length)
                {
                    if (poolIndex == 0)
                    {
                        Monitor.Exit(poolLock);
                        return -1;
                    }
                    index = -1;
                    try
                    {
                        valueType[] newPool = new valueType[poolIndex << 1];
                        Array.Copy(pool, newPool, poolIndex);
                        pool = newPool;
                        index = poolIndex;
                    }
                    finally
                    {
                        if (index == -1) Monitor.Exit(poolLock);
                    }
                }
                else index = poolIndex;
                ++poolIndex;
            }
            else index = *--currentFreeIndex;
            pool[index] = value;
            Monitor.Exit(poolLock);
            return index;
        }
        /// <summary>
        /// 释放对象
        /// </summary>
        /// <param name="index"></param>
        public void UnsafeFree(int index)
        {
            Monitor.Enter(poolLock);
            if (currentFreeIndex != null)
            {
                if (currentFreeIndex == endFreeIndex)
                {
                    pointer.size oldIndex = new pointer.size();
                    int size = (int)(endFreeIndex - freeIndexs.Byte);
                    try
                    {
                        pointer.size newIndex = unmanaged.Get(size <<= 1, false);
                        byte* newEnd = newIndex.Byte + size;
                        oldIndex = freeIndexs;
                        fastCSharp.unsafer.memory.Copy(oldIndex.data, newIndex.data, size = (int)((byte*)currentFreeIndex - oldIndex.Byte));
                        endFreeIndex = newEnd;
                        freeIndexs = newIndex;
                        currentFreeIndex = (int*)(newIndex.Byte + size);
                    }
                    finally
                    {
                        if (oldIndex.data == null) Monitor.Exit(poolLock);
                    }
                    unmanaged.Free(ref oldIndex);
                }
                pool[index] = null;
                *currentFreeIndex++ = index;
            }
            Monitor.Exit(poolLock);
        }
        /// <summary>
        /// 释放对象测试
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public valueType GetUnsafeFree(int index)
        {
            valueType value = pool[index];
            UnsafeFree(index);
            return value;
        }
        /// <summary>
        /// 获取对象数组
        /// </summary>
        /// <returns></returns>
        public subArray<valueType> GetArray()
        {
            int count = poolIndex;
            if (count != 0)
            {
                subArray<valueType> values = new subArray<valueType>(count);
                foreach (valueType value in pool)
                {
                    if (value != null) values.UnsafeAdd(value);
                    if (--count == 0) break;
                }
                return values;
            }
            return default(subArray<valueType>);
        }
    }
    /// <summary>
    /// 索引池
    /// </summary>
    /// <typeparam name="valueType">对象类型</typeparam>
    public unsafe struct indexValuePool<valueType> where valueType : struct
    {
        /// <summary>
        /// 对象池
        /// </summary>
        internal valueType[] Pool;
        /// <summary>
        /// 对象池
        /// </summary>
        public valueType[] UnsafeArray
        {
            get { return Pool; }
        }
        /// <summary>
        /// 对象池访问锁
        /// </summary>
        private readonly object poolLock;
        /// <summary>
        /// 空闲索引集合
        /// </summary>
        private int[] freeIndexs;
        /// <summary>
        /// 对象池当前索引位置
        /// </summary>
        private int poolIndex;
        /// <summary>
        /// 设置对象池当前索引位置
        /// </summary>
        public int PoolIndex
        {
            get { return poolIndex; }
            internal set { poolIndex = value; }
        }
        /// <summary>
        /// 当前空闲索引
        /// </summary>
        private int freeIndex;
        /// <summary>
        /// 索引池
        /// </summary>
        /// <param name="size"></param>
        public indexValuePool(int size)
        {
            if (size <= 0) size = fastCSharp.config.appSetting.PoolSize;
            poolLock = new object();
            Pool = new valueType[size];
            freeIndexs = new int[size];
            poolIndex = freeIndex = 0;
        }
        /// <summary>
        /// 申请对象池访问锁
        /// </summary>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool Enter()
        {
            Monitor.Enter(poolLock);
            if (Pool == null)
            {
                Monitor.Exit(poolLock);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 释放对象池访问锁
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Exit()
        {
            Monitor.Exit(poolLock);
        }
        /// <summary>
        /// 获取池索引(当前占用锁状态)并保持锁状态
        /// </summary>
        /// <returns></returns>
        public int GetIndexContinue()
        {
            if (freeIndex == 0)
            {
                if (poolIndex == Pool.Length)
                {
                    valueType[] newPool = new valueType[poolIndex << 1];
                    Array.Copy(Pool, newPool, poolIndex);
                    Pool = newPool;
                }
                return poolIndex++;
            }
            return freeIndexs[--freeIndex];
        }
        /// <summary>
        /// 释放对象(当前占用锁状态)
        /// </summary>
        /// <param name="index"></param>
        public void FreeExit(int index)
        {
            if (freeIndex == freeIndexs.Length)
            {
                try
                {
                    int[] newIndexs = new int[freeIndex << 1];
                    Array.Copy(freeIndexs, newIndexs, freeIndex);
                    newIndexs[freeIndex] = index;
                    freeIndexs = newIndexs;
                    ++freeIndex;
                }
                finally { Monitor.Exit(poolLock); }
                return;
            }
            freeIndexs[freeIndex++] = index;
            Monitor.Exit(poolLock);
        }
        /// <summary>
        /// 释放对象(当前占用锁状态)
        /// </summary>
        /// <param name="index"></param>
        internal void FreeContinue(int index)
        {
            if (freeIndex == freeIndexs.Length)
            {
                int[] newIndexs = new int[freeIndex << 1];
                Array.Copy(freeIndexs, newIndexs, freeIndex);
                newIndexs[freeIndex] = index;
                freeIndexs = newIndexs;
                ++freeIndex;
            }
            else freeIndexs[freeIndex++] = index;
        }
        /// <summary>
        /// 清除索引数据
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void ClearIndexContinue()
        {
            poolIndex = freeIndex = 0;
        }
    }
}
