using System;
using System.Collections.Generic;
using fastCSharp.threading;
using System.Threading;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 内存池
    /// </summary>
    public sealed class memoryPool
    {
        /// <summary>
        /// 缓冲数组子串
        /// </summary>
        public struct pushSubArray
        {
            /// <summary>
            /// 数组子串
            /// </summary>
            internal subArray<byte> Value;
            /// <summary>
            /// 数组子串
            /// </summary>
            public subArray<byte> SubArray
            {
                get { return Value; }
            }
            /// <summary>
            /// 数组
            /// </summary>
            public byte[] UnsafeArray
            {
                get { return Value.array; }
            }
            /// <summary>
            /// 数组子串入池处理
            /// </summary>
            internal memoryPool PushPool;
            /// <summary>
            /// 数组子串入池处理
            /// </summary>
            public void Push()
            {
                if (PushPool == null) Value.array = null;
                else
                {
                    try
                    {
                        PushPool.Push(ref Value.array);
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, null, false);
                    }
                }
            }
            /// <summary>
            /// 缓冲数组子串
            /// </summary>
            /// <param name="value">数组子串</param>
            /// <param name="pushPool">数组子串入池处理</param>
            public pushSubArray(subArray<byte> value, memoryPool pushPool)
            {
                Value = value;
                this.PushPool = pushPool;
            }
        }
        /// <summary>
        /// 缓冲区尺寸
        /// </summary>
        public readonly int Size;
        /// <summary>
        /// 清除缓冲区集合
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Clear()
        {
            clear(0);
        }
        /// <summary>
        /// 清除缓冲区集合
        /// </summary>
        /// <param name="count">保留清除缓冲区数量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Clear(int count)
        {
            clear(count <= 0 ? 0 : count);
        }
        /// <summary>
        /// 获取缓冲区
        /// </summary>
        /// <param name="minSize">数据字节长度</param>
        /// <returns>缓冲区,失败返回null</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public byte[] TryGet(int minSize)
        {
            return minSize <= Size ? TryGet() : null;
        }
        /// <summary>
        /// 获取缓冲区
        /// </summary>
        /// <returns>缓冲区</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public byte[] Get()
        {
            byte[] data = TryGet();
            return data ?? new byte[Size];
        }
        /// <summary>
        /// 获取缓冲区
        /// </summary>
        /// <param name="isNew">是否新建缓冲区</param>
        /// <returns>缓冲区</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public byte[] Get(out bool isNew)
        {
            byte[] data = TryGet();
            if (data == null)
            {
                isNew = true;
                return new byte[Size];
            }
            isNew = false;
            return data;
        }
        /// <summary>
        /// 获取缓冲区
        /// </summary>
        /// <param name="minSize">数据字节长度</param>
        /// <returns>缓冲区</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public byte[] Get(int minSize)
        {
            return minSize <= Size ? Get() : new byte[minSize];
        }
        /// <summary>
        /// 获取缓冲区
        /// </summary>
        /// <param name="minSize">数据字节长度</param>
        /// <param name="isNew">是否新建缓冲区</param>
        /// <returns>缓冲区</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public byte[] Get(int minSize, out bool isNew)
        {
            if (minSize <= Size) return Get(out isNew);
            isNew = true;
            return new byte[minSize];
        }
        /// <summary>
        /// 保存缓冲区
        /// </summary>
        /// <param name="buffer"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void PushOnly(subArray<byte> buffer)
        {
            PushOnly(buffer.array);
        }
#if DEBUGPOOL
        /// <summary>
        /// 缓冲区集合
        /// </summary>
        private HashSet<byte[]> buffers;
        /// <summary>
        /// 缓冲区集合访问锁
        /// </summary>
        private readonly object bufferLock = new object();
        /// <summary>
        /// 当前最大缓冲区数量
        /// </summary>
        private int maxCount = config.appSetting.PoolSize;
        /// <summary>
        /// 内存池
        /// </summary>
        /// <param name="size">缓冲区尺寸</param>
        public memoryPool(int size)
        {
            buffers = hashSet.CreateOnly<byte[]>();
            Size = size;
            PushHandle = Push;
            PushSubArray = PushOnly;
        }
        /// <summary>
        /// 清除缓冲区集合
        /// </summary>
        /// <param name="count">保留清除缓冲区数量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void clear(int count)
        {
            Monitor.Enter(bufferLock);
            buffers.Clear();
            Monitor.Exit(bufferLock); 
        }
        /// <summary>
        /// 获取缓冲区
        /// </summary>
        /// <returns>缓冲区,失败返回null</returns>
        public byte[] TryGet()
        {
            byte[] buffer = null;
            Monitor.Enter(bufferLock);
            foreach (byte[] data in buffers)
            {
                buffer = data;
                break;
            }
            if (buffer != null) buffers.Remove(buffer);
            Monitor.Exit(bufferLock); 
            return buffer;
        }
        /// <summary>
        /// 保存缓冲区
        /// </summary>
        /// <param name="buffer">缓冲区</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Push(ref byte[] buffer)
        {
            PushOnly(Interlocked.Exchange(ref buffer, null));
        }
        /// <summary>
        /// 保存缓冲区
        /// </summary>
        /// <param name="buffer">缓冲区</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void PushOnly(byte[] buffer)
        {
            if (buffer != null) PushNotNull(buffer);
        }
        /// <summary>
        /// 保存缓冲区
        /// </summary>
        /// <param name="buffer">缓冲区</param>
        public void PushNotNull(byte[] buffer)
        {
            if (buffer.Length == Size)
            {
                bool isAdd, isMax = false;
                Monitor.Enter(bufferLock);
                try
                {
                    if ((isAdd = buffers.Add(buffer)) && (isMax = buffers.Count > maxCount))
                    {
                        maxCount <<= 1;
                    }
                }
                finally { Monitor.Exit(bufferLock); }
                if (isAdd)
                {
                    if (isMax)
                    {
                        log.Default.Add("内存池扩展实例数量 byte[" + buffers.Count.toString() + "][" + Size.toString() + "]", new System.Diagnostics.StackFrame(), false);
                    }
                }
                else log.Error.Add("内存池释放冲突 " + Size.toString(), null, false);
            }
        }
#else
        /// <summary>
        /// 缓冲区集合
        /// </summary>
        private objectPool<byte[]> pool;
        /// <summary>
        /// 内存池
        /// </summary>
        /// <param name="size">缓冲区尺寸</param>
        public memoryPool(int size)
        {
            pool = objectPool<byte[]>.Create();
            Size = size;
        }
        /// <summary>
        /// 清除缓冲区集合
        /// </summary>
        /// <param name="count">保留清除缓冲区数量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void clear(int count)
        {
            pool.Clear(count);
        }
        /// <summary>
        /// 获取缓冲区
        /// </summary>
        /// <returns>缓冲区,失败返回null</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public byte[] TryGet()
        {
            return pool.Pop();
        }
        /// <summary>
        /// 保存缓冲区
        /// </summary>
        /// <param name="buffer">缓冲区</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Push(ref byte[] buffer)
        {
            byte[] data = Interlocked.Exchange(ref buffer, null);
            if (data != null && data.Length == Size) pool.Push(data);
        }
        /// <summary>
        /// 保存缓冲区
        /// </summary>
        /// <param name="buffer">缓冲区</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void PushOnly(byte[] buffer)
        {
            if (buffer != null && buffer.Length == Size) pool.Push(buffer);
        }
        /// <summary>
        /// 保存缓冲区
        /// </summary>
        /// <param name="buffer">缓冲区</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void PushNotNull(byte[] buffer)
        {
            if (buffer.Length == Size) pool.Push(buffer);
        }
#endif
        /// <summary>
        /// 内存池
        /// </summary>
        private static readonly Dictionary<int, memoryPool> pools;
        /// <summary>
        /// 内存池访问锁
        /// </summary>
        private static readonly object poolLock = new object();
        /// <summary>
        /// 获取内存池[反射引用于 fastCSharp.memoryPoolProxy]
        /// </summary>
        /// <param name="size">缓冲区尺寸</param>
        /// <returns>内存池</returns>
        public static memoryPool GetOrCreate(int size)
        {
            if (size <= 0) log.Error.Throw(log.exceptionType.IndexOutOfRange);
            memoryPool pool;
            Monitor.Enter(poolLock);
            if (pools.TryGetValue(size, out pool)) Monitor.Exit(poolLock);
            else
            {
                try
                {
                    pools.Add(size, pool = new memoryPool(size));
                }
                finally { Monitor.Exit(poolLock); }
            }
            return pool;
        }
        /// <summary>
        /// 清除内存池
        /// </summary>
        /// <param name="count">保留清除缓冲区数量</param>
        public static void ClearPool(int count = 0)
        {
            if (count <= 0) count = 0;
            Monitor.Enter(poolLock);
            foreach (memoryPool pool in pools.Values) pool.clear(count);
            Monitor.Exit(poolLock);
        }
        /// <summary>
        /// 默认临时缓冲区
        /// </summary>
        public static readonly memoryPool TinyBuffers;
        /// <summary>
        /// 默认流缓冲区
        /// </summary>
        public static readonly memoryPool StreamBuffers;
        /// <summary>
        /// 获取临时缓冲区
        /// </summary>
        /// <param name="length">缓冲区字节长度</param>
        /// <returns>临时缓冲区</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static memoryPool GetDefaultPool(int length)
        {
            return length <= unmanagedStreamBase.DefaultLength ? TinyBuffers : StreamBuffers;
        }

        static memoryPool()
        {
            pools = dictionary.CreateInt<memoryPool>();
            pools.Add(unmanagedStreamBase.DefaultLength, TinyBuffers = new memoryPool(unmanagedStreamBase.DefaultLength));
            pools.Add(config.appSetting.StreamBufferSize, StreamBuffers = new memoryPool(config.appSetting.StreamBufferSize));
            if (fastCSharp.config.appSetting.IsCheckMemory) checkMemory.Add(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }
    }
}
