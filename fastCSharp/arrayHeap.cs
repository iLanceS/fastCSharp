using System;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 数组模拟最小堆
    /// </summary>
    /// <typeparam name="keyType">关键字类型</typeparam>
    /// <typeparam name="valueType">数据类型</typeparam>
    public unsafe class arrayHeap<keyType, valueType> : IDisposable
        where keyType : IComparable<keyType>
    {
        /// <summary>
        /// 默认数组长度
        /// </summary>
        private const int defaultArrayLength = 256;
        /// <summary>
        /// 数据数组
        /// </summary>
        protected keyValue<keyType, valueType>[] array;
        /// <summary>
        /// 最小堆索引
        /// </summary>
        protected pointer.size heap;
        /// <summary>
        /// 是否固定内存申请
        /// </summary>
        private bool isStaticUnmanaged;
        /// <summary>
        /// 数据数量
        /// </summary>
        public int Count
        {
            get { return *heap.Int; }
        }
        /// <summary>
        /// 数组模拟最小堆
        /// </summary>
        public arrayHeap() : this(false) { }
        /// <summary>
        /// 数组模拟最小堆
        /// </summary>
        /// <param name="isStaticUnmanaged">是否固定内存申请</param>
        internal arrayHeap(bool isStaticUnmanaged)
        {
            this.isStaticUnmanaged = isStaticUnmanaged;
            array = new keyValue<keyType, valueType>[defaultArrayLength];
            heap = isStaticUnmanaged ? unmanaged.GetStaticSize(defaultArrayLength * sizeof(int), false) : unmanaged.Get(defaultArrayLength * sizeof(int), false);
            int* heapFixed = heap.Int;
            for (int index = defaultArrayLength; index != 0; heapFixed[index] = index) --index;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            array = null;
            if(isStaticUnmanaged) unmanaged.FreeStatic(ref heap);
            else unmanaged.Free(ref heap);
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">数据值</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Add(keyType key, valueType value)
        {
            Push(key, value);
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">数据值</param>
        public unsafe void Push(keyType key, valueType value)
        {
            int* heapFixed = heap.Int;
            if (*heapFixed == 0) array[heapFixed[1]].Set(key, value);
            else
            {
                int heapIndex = *heapFixed + 1;
                if (heapIndex == array.Length)
                {
                    create();
                    heapFixed = heap.Int;
                }
                int valueIndex = heapFixed[heapIndex];
                array[valueIndex].Set(key, value);
                heapFixed[getPushIndex(key, heapIndex)] = valueIndex;
            }
            ++*heapFixed;
        }
        /// <summary>
        /// 重建数据
        /// </summary>
        protected void create()
        {
            int heapIndex = array.Length, newCount = heapIndex << 1, newHeapSize = newCount * sizeof(int);
            keyValue<keyType, valueType>[] newArray = new keyValue<keyType, valueType>[newCount];
            pointer.size newHeap = isStaticUnmanaged ? unmanaged.GetStaticSize(newHeapSize, false) : unmanaged.Get(newHeapSize, false), oldHeap = heap;
            int* newHeapFixed = newHeap.Int;
            array.CopyTo(newArray, 0);
            fastCSharp.unsafer.memory.Copy(heap.Byte, newHeapFixed, newHeapSize >> 1);
            do
            {
                --newCount;
                newHeapFixed[newCount] = newCount;
            }
            while (newCount != heapIndex);
            array = newArray;
            heap = newHeap;
            if (isStaticUnmanaged) unmanaged.FreeStatic(ref oldHeap);
            else unmanaged.Free(ref oldHeap);
        }
        ///// <summary>
        ///// 弹出堆顶数据
        ///// </summary>
        ///// <returns>堆顶数据</returns>
        //public unsafe keyValue<keyType, valueType> Pop()
        //{
        //    if (Count == 0) throw new IndexOutOfRangeException();
        //    keyValue<keyType, valueType> value = array[heap.Int[0]];
        //    RemoveTop();
        //    return value;
        //}
        /// <summary>
        /// 弹出堆顶数据
        /// </summary>
        /// <param name="value">堆顶数据</param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe bool Pop(out keyValue<keyType, valueType> value)
        {
            int* heapFixed = heap.Int;
            if (*heapFixed == 0)
            {
                value = default(keyValue<keyType, valueType>);
                return false;
            }
            value = array[heapFixed[1]];
            RemoveTop();
            return true;
        }
        ///// <summary>
        ///// 获取堆顶数据,不弹出
        ///// </summary>
        ///// <returns>堆顶数据</returns>
        //[MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        //public keyValue<keyType, valueType> Top()
        //{
        //    if (Count == 0) throw new IndexOutOfRangeException();
        //    return array[heap.Int[0]];
        //}
        /// <summary>
        /// 获取堆顶数据,不弹出
        /// </summary>
        /// <param name="value">堆顶数据</param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool Top(out keyValue<keyType, valueType> value)
        {
            int* heapFixed = heap.Int;
            if (*heapFixed == 0)
            {
                value = default(keyValue<keyType, valueType>);
                return false;
            }
            value = array[heapFixed[1]];
            return true;
        }
        /// <summary>
        /// 获取堆顶数据,不弹出
        /// </summary>
        /// <returns>堆顶数据</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal keyValue<keyType, valueType> UnsafeTop()
        {
            return array[heap.Int[1]];
        }
        /// <summary>
        /// 获取添加数据位置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="heapIndex"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private int getPushIndex(keyType key, int heapIndex)
        {
            int* heapFixed = heap.Int;
        START:
            int parentValueIndex = heapFixed[heapIndex >> 1];
            if (key.CompareTo(array[parentValueIndex].Key) < 0)
            {
                heapFixed[heapIndex] = parentValueIndex;
                if ((heapIndex >>= 1) != 1) goto START;
                return 1;
            }
            return heapIndex;
        }
        /// <summary>
        /// 删除堆顶数据
        /// </summary>
        internal void RemoveTop()
        {
            int* heapFixed = heap.Int;
            int heapIndex = 1, lastHeapIndex = *heapFixed, lastValueIndex = heapFixed[lastHeapIndex];
            array[heapFixed[lastHeapIndex] = heapFixed[1]].Null();
            for (int maxHeapIndex = (lastHeapIndex + 1) >> 1; heapIndex < maxHeapIndex; )
            {
                int left = heapIndex << 1, right = left + 1;
                if (right != lastHeapIndex)
                {
                    if (array[heapFixed[left]].Key.CompareTo(array[heapFixed[right]].Key) < 0)
                    {
                        heapFixed[heapIndex] = heapFixed[left];
                        heapIndex = left;
                    }
                    else
                    {
                        heapFixed[heapIndex] = heapFixed[right];
                        heapIndex = right;
                    }
                }
                else
                {
                    heapFixed[heapIndex] = heapFixed[left];
                    heapIndex = left;
                    break;
                }
            }
            heapFixed[heapIndex == 1 ? 1 : getPushIndex(array[lastValueIndex].Key, heapIndex)] = lastValueIndex;
            --*heapFixed;
        }
    }
    /// <summary>
    /// 数组模拟最小堆
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    internal unsafe sealed class arrayHeap<valueType> : arrayHeap<long, valueType>
    {
        /// <summary>
        /// 数组模拟最小堆
        /// </summary>
        /// <param name="isStaticUnmanaged">是否固定内存申请</param>
        internal arrayHeap(bool isStaticUnmanaged) : base(isStaticUnmanaged) { }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">数据值</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public new void Add(long key, valueType value)
        {
            Push(key, value);
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">数据值</param>
        public unsafe new void Push(long key, valueType value)
        {
            int* heapFixed = heap.Int;
            if (*heapFixed == 0) array[heapFixed[1]].Set(key, value);
            else
            {
                int heapIndex = *heapFixed + 1;
                if (heapIndex == array.Length)
                {
                    create();
                    heapFixed = heap.Int;
                }
                int valueIndex = heapFixed[heapIndex];
                array[valueIndex].Set(key, value);
                heapFixed[getPushIndex(key, heapIndex)] = valueIndex;
            }
            ++*heapFixed;
        }
        /// <summary>
        /// 弹出堆顶数据
        /// </summary>
        /// <param name="value">堆顶数据</param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public new unsafe bool Pop(out keyValue<long, valueType> value)
        {
            int* heapFixed = heap.Int;
            if (*heapFixed == 0)
            {
                value = default(keyValue<long, valueType>);
                return false;
            }
            value = array[heapFixed[1]];
            RemoveTop();
            return true;
        }
        /// <summary>
        /// 获取添加数据位置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="heapIndex"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private int getPushIndex(long key, int heapIndex)
        {
            int* heapFixed = heap.Int;
        START:
            int parentValueIndex = heapFixed[heapIndex >> 1];
            if (key < array[parentValueIndex].Key)
            {
                heapFixed[heapIndex] = parentValueIndex;
                if ((heapIndex >>= 1) != 1) goto START;
                return 1;
            }
            return heapIndex;
        }
        /// <summary>
        /// 删除堆顶数据
        /// </summary>
        internal new void RemoveTop()
        {
            int* heapFixed = heap.Int;
            int heapIndex = 1, lastHeapIndex = *heapFixed, lastValueIndex = heapFixed[lastHeapIndex];
            array[heapFixed[lastHeapIndex] = heapFixed[1]].Null();
            for (int maxHeapIndex = (lastHeapIndex + 1) >> 1; heapIndex < maxHeapIndex; )
            {
                int left = heapIndex << 1, right = left + 1;
                if (right != lastHeapIndex)
                {
                    if (array[heapFixed[left]].Key < array[heapFixed[right]].Key)
                    {
                        heapFixed[heapIndex] = heapFixed[left];
                        heapIndex = left;
                    }
                    else
                    {
                        heapFixed[heapIndex] = heapFixed[right];
                        heapIndex = right;
                    }
                }
                else
                {
                    heapFixed[heapIndex] = heapFixed[left];
                    heapIndex = left;
                    break;
                }
            }
            heapFixed[heapIndex == 1 ? 1 : getPushIndex(array[lastValueIndex].Key, heapIndex)] = lastValueIndex;
            --*heapFixed;
        }
    }
}
