using System;
using System.Threading;
using fastCSharp.threading;
using fastCSharp.reflection;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 数组池
    /// </summary>
    /// <typeparam name="valueType"></typeparam>
    public struct objectPool<valueType> where valueType : class
    {
        /// <summary>
        /// 释放资源委托
        /// </summary>
        internal static readonly Action<valueType> Dispose;
        /// <summary>
        /// 数据集合
        /// </summary>
        private array.value<valueType>[] array;
        /// <summary>
        /// 数据集合访问锁
        /// </summary>
        private object arrayLock;
        /// <summary>
        /// 数据数量
        /// </summary>
        internal int Count;
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Push(valueType value)
        {
            Monitor.Enter(arrayLock);
            if (Count == array.Length) create(value);
            else
            {
                array[Count++].Value = value;
                Monitor.Exit(arrayLock);
            }
        }
        /// <summary>
        /// 重建数据集合
        /// </summary>
        /// <param name="value"></param>
        private void create(valueType value)
        {
            try
            {
                array.value<valueType>[] newArray = new array.value<valueType>[Count << 1];
                System.Array.Copy(array, 0, newArray, 0, Count);
                newArray[Count].Value = value;
                array = newArray;
                ++Count;
            }
            finally { Monitor.Exit(arrayLock); }
        }
        ///// <summary>
        ///// 添加类型对象集合
        ///// </summary>
        ///// <param name="values"></param>
        ///// <param name="count"></param>
        //internal void Push(array.value<valueType>[] values, int count)
        //{
        //PUSH:
        //    arrayLock.CompareSetYieldSleep0();
        //    if (Count + count > array.Length)
        //    {
        //        int length = array.Length;
        //        arrayLock.Exit();
        //        Monitor.Enter(newLock);
        //        if (length == array.Length)
        //        {
        //            try
        //            {
        //                array.value<valueType>[] newArray = new array.value<valueType>[length + Math.Max(length, count)];
        //                arrayLock.NoCheckCompareSetSleep0();
        //                System.Array.Copy(array, 0, newArray, 0, Count);
        //                System.Array.Copy(values, 0, newArray, Count, count);
        //                array = newArray;
        //                Count += count;
        //                arrayLock.Exit();
        //            }
        //            finally { Monitor.Exit(newLock); }
        //            return;
        //        }
        //        Monitor.Exit(newLock);
        //        goto PUSH;
        //    }
        //    Array.Copy(values, 0, array, Count, count);
        //    Count += count;
        //    arrayLock.Exit();
        //}
        /// <summary>
        /// 获取类型对象
        /// </summary>
        /// <returns>类型对象</returns>
        public valueType Pop()
        {
            Monitor.Enter(arrayLock);
            if (Count == 0)
            {
                Monitor.Exit(arrayLock);
                return null;
            }
            valueType value = array[--Count].Free();
            Monitor.Exit(arrayLock);
            return value;
        }
        ///// <summary>
        ///// 获取类型对象集合
        ///// </summary>
        ///// <param name="values"></param>
        ///// <returns></returns>
        //internal int Pop(array.value<valueType>[] values)
        //{
        //    arrayLock.CompareSetYieldSleep0();
        //    if (Count == 0)
        //    {
        //        arrayLock.Exit();
        //        return 0;
        //    }
        //    int count = Math.Min(Count, values.Length);
        //    Array.Copy(array, Count -= count, values, 0, count);
        //    Array.Clear(array, Count, count);
        //    arrayLock.Exit();
        //    return count;
        //}
        /// <summary>
        /// 清除数据集合
        /// </summary>
        /// <param name="count">保留数据数量</param>
        /// <returns>被清除的数据集合</returns>
        internal void Clear(int count)
        {
            if (Dispose == null)
            {
                Monitor.Enter(arrayLock);
                int length = Count - count;
                if (length > 0)
                {
                    System.Array.Clear(array, count, length);
                    Count = count;
                }
                Monitor.Exit(arrayLock);
            }
            else
            {
                foreach (array.value<valueType> value in GetClear(count))
                {
                    try
                    {
                        Dispose(value.Value);
                    }
                    catch (Exception error)
                    {
                        log.Default.Add(error, null, false);
                    }
                }
            }
        }
        /// <summary>
        /// 清除数据集合
        /// </summary>
        /// <param name="count">保留数据数量</param>
        /// <returns>被清除的数据集合</returns>
        internal array.value<valueType>[] GetClear(int count)
        {
            Monitor.Enter(arrayLock);
            int length = Count - count;
            if (length > 0)
            {
                array.value<valueType>[] removeBuffers;
                try
                {
                    removeBuffers = new array.value<valueType>[length];
                    System.Array.Copy(array, Count = count, removeBuffers, 0, length);
                    System.Array.Clear(array, count, length);
                }
                finally { Monitor.Exit(arrayLock); }
                return removeBuffers;
            }
            else Monitor.Exit(arrayLock);
            return nullValue<array.value<valueType>>.Array;
        }
        /// <summary>
        /// 创建数组池
        /// </summary>
        /// <returns>数组池</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static objectPool<valueType> Create()
        {
            return new objectPool<valueType> { array = new array.value<valueType>[fastCSharp.config.appSetting.PoolSize], arrayLock = new object() };
        }
        /// <summary>
        /// 创建数组池
        /// </summary>
        /// <param name="size">容器初始化大小</param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static objectPool<valueType> Create(int size)
        {
            return new objectPool<valueType> { array = new array.value<valueType>[size <= 0 ? fastCSharp.config.appSetting.PoolSize : size], arrayLock = new object() };
        }
        static objectPool()
        {
            Type type = typeof(valueType);
            if (typeof(IDisposable).IsAssignableFrom(type))
            {
                Dispose = (Action<valueType>)Delegate.CreateDelegate(typeof(Action<valueType>), type.GetMethod("Dispose", BindingFlags.Public | BindingFlags.Instance, null, nullValue<Type>.Array, null));
            }
        }
    }
}
