using System;
using System.Collections.Generic;
using fastCSharp.threading;
using System.Threading;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 非托管内存池
    /// </summary>
    public unsafe sealed class unmanagedPool
    {
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
        public pointer TryGet(int minSize)
        {
            return minSize <= Size ? TryGet() : new pointer();
        }
        /// <summary>
        /// 获取缓冲区
        /// </summary>
        /// <returns>缓冲区</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public pointer Get()
        {
            pointer data = TryGet();
            return data.Data != null ? data : unmanaged.Get(Size, false).Pointer;
        }
        /// <summary>
        /// 获取缓冲区
        /// </summary>
        /// <param name="minSize">数据字节长度</param>
        /// <returns>缓冲区</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public pointer.size Get(int minSize)
        {
            return minSize <= Size ? new pointer.size { data = Get().Data, sizeValue = Size } : unmanaged.Get(minSize, false);
        }
        /// <summary>
        /// 保存缓冲区
        /// </summary>
        /// <param name="buffer">缓冲区</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Push(ref pointer buffer)
        {
            push(ref buffer.Data);
        }
        /// <summary>
        /// 保存缓冲区
        /// </summary>
        /// <param name="buffer">缓冲区</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Push(ref pointer.size buffer)
        {
            if (buffer.sizeValue == Size) push(ref buffer.data);
            else unmanaged.Free(ref buffer);
        }
#if DEBUGPOOL
        /// <summary>
        /// 缓冲区集合
        /// </summary>
        private HashSet<pointer> buffers;
        /// <summary>
        /// 非托管内存数量
        /// </summary>
        private int count { get { return buffers.Count; } }
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
        public unmanagedPool(int size)
        {
            buffers = hashSet.CreatePointer();
            Size = size;
        }
        /// <summary>
        /// 清除缓冲区集合
        /// </summary>
        /// <param name="count">保留清除缓冲区数量</param>
        private void clear(int count)
        {
            Monitor.Enter(bufferLock);
            pointer[] removeBuffers;
            try
            {
                removeBuffers = buffers.getArray();
                buffers.Clear();
            }
            finally { Monitor.Exit(bufferLock); }
            foreach (pointer pointer in removeBuffers)
            {
                try
                {
                    unmanaged.Free(pointer.Data);
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
            }
        }
        /// <summary>
        /// 获取缓冲区
        /// </summary>
        /// <returns>缓冲区,失败返回null</returns>
        public pointer TryGet()
        {
            pointer buffer = new pointer();
            Monitor.Enter(bufferLock);
            foreach (pointer data in buffers)
            {
                buffer = data;
                break;
            }
            if (buffer.Data != null) buffers.Remove(buffer);
            Monitor.Exit(bufferLock);
            return buffer;
        }
        /// <summary>
        /// 保存缓冲区
        /// </summary>
        /// <param name="buffer">缓冲区</param>
        private void push(ref void* buffer)
        {
            if (buffer != null)
            {
                void* data = buffer;
                bool isAdd, isMax = false;
                buffer = null;
                Monitor.Enter(bufferLock);
                try
                {
                    if ((isAdd = buffers.Add(new pointer { Data = data })) && (isMax = buffers.Count > maxCount))
                    {
                        maxCount <<= 1;
                    }
                }
                finally { Monitor.Exit(bufferLock); }
                if (isAdd)
                {
                    if (isMax)
                    {
                        log.Default.Add("非托管内存池扩展实例数量 byte*(" + Size.toString() + ")[" + buffers.Count.toString() + "]", new System.Diagnostics.StackFrame(), false);
                    }
                }
                else log.Error.Add("内存池释放冲突 " + Size.toString(), null, false);
            }
        }
        ///// <summary>
        ///// 保存缓冲区
        ///// </summary>
        ///// <param name="data">缓冲区</param>
        //public override void Push(void* data)
        //{
        //    if (data != null)
        //    {
        //        bool isAdd, isMax = false;
        //        interlocked.NoCheckCompareSetSleep0(ref bufferLock);
        //        try
        //        {
        //            if ((isAdd = buffers.Add(new pointer { Data = data })) && (isMax = buffers.Count > maxCount))
        //            {
        //                maxCount <<= 1;
        //            }
        //        }
        //        finally { bufferLock = 0; }
        //        if (isAdd)
        //        {
        //            if (isMax)
        //            {
        //                log.Default.Add("非托管内存池扩展实例数量 byte*(" + Size.toString() + ")[" + buffers.Count.toString() + "]", false, false);
        //            }
        //        }
        //        else log.Error.Add("内存池释放冲突 " + Size.toString(), true, false);
        //    }
        //}
#else
        /// <summary>
        /// 缓冲区集合
        /// </summary>
        private arrayPool<pointer> pool;
        /// <summary>
        /// 非托管内存数量
        /// </summary>
        private int count { get { return pool.Count; } }
        /// <summary>
        /// 内存池
        /// </summary>
        /// <param name="size">缓冲区尺寸</param>
        public unmanagedPool(int size)
        {
            pool = arrayPool<pointer>.Create();
            Size = size;
        }
        /// <summary>
        /// 清除缓冲区集合
        /// </summary>
        /// <param name="count">保留清除缓冲区数量</param>
        private void clear(int count)
        {
            pointer.size pointerSize = new pointer.size();
            foreach (pointer pointer in pool.GetClear(count, false))
            {
                try
                {
                    pointerSize.Set(pointer.Data, Size);
                    unmanaged.Free(ref pointerSize);
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
            }
        }
        /// <summary>
        /// 获取缓冲区
        /// </summary>
        /// <returns>缓冲区,失败返回null</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public pointer TryGet()
        {
            pointer value = default(pointer);
            pool.TryGet(ref value);
            return value;
        }
        /// <summary>
        /// 保存缓冲区
        /// </summary>
        /// <param name="buffer">缓冲区</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private void push(ref void* buffer)
        {
            if (buffer != null)
            {
                void* data = buffer;
                buffer = null;
                pool.Push(new pointer { Data = data });
            }
        }
#endif
        /// <summary>
        /// 内存池
        /// </summary>
        private static readonly Dictionary<int, unmanagedPool> pools;
        /// <summary>
        /// 内存池访问锁
        /// </summary>
        private static readonly object poolLock = new object();
        /// <summary>
        /// 获取内存池
        /// </summary>
        /// <param name="size">缓冲区尺寸</param>
        /// <returns>内存池</returns>
        public static unmanagedPool GetOrCreate(int size)
        {
            if (size <= 0) log.Error.Throw(log.exceptionType.IndexOutOfRange);
            unmanagedPool pool;
            Monitor.Enter(poolLock);
            if (pools.TryGetValue(size, out pool)) Monitor.Exit(poolLock);
            else
            {
                try
                {
                    pools.Add(size, pool = new unmanagedPool(size));
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
            foreach (unmanagedPool pool in pools.Values) pool.clear(count);
            Monitor.Exit(poolLock);
        }
        /// <summary>
        /// 默认临时缓冲区
        /// </summary>
        public static readonly unmanagedPool TinyBuffers;
        /// <summary>
        /// 默认流缓冲区
        /// </summary>
        public static readonly unmanagedPool StreamBuffers;
        /// <summary>
        /// 获取临时缓冲区
        /// </summary>
        /// <param name="length">缓冲区字节长度</param>
        /// <returns>临时缓冲区</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static unmanagedPool GetDefaultPool(int length)
        {
            return length <= unmanagedStreamBase.DefaultLength ? TinyBuffers : StreamBuffers;
        }
        /// <summary>
        /// 获取所有非托管内存的数量
        /// </summary>
        /// <returns></returns>
        public static int TotalCount
        {
            get
            {
                int count = 0;
                Monitor.Enter(poolLock);
                foreach (unmanagedPool pool in pools.Values) count += pool.count;
                Monitor.Exit(poolLock);
                return count;
            }
        }

        static unmanagedPool()
        {
            pools = dictionary.CreateInt<unmanagedPool>();
            pools.Add(unmanagedStreamBase.DefaultLength, TinyBuffers = new unmanagedPool(unmanagedStreamBase.DefaultLength));
            pools.Add(config.appSetting.StreamBufferSize, StreamBuffers = new unmanagedPool(config.appSetting.StreamBufferSize));
            if (fastCSharp.config.appSetting.IsCheckMemory) checkMemory.Add(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }
    }
}
