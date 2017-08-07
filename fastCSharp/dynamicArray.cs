using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using fastCSharp.reflection;
using fastCSharp.threading;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 动态数组信息
    /// </summary>
    internal static class dynamicArray
    {
        /// <summary>
        /// 是否需要清除数组
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>需要清除数组</returns>
        public static bool IsClearArray(Type type)
        {
            if (type.IsPointer) return false;
            if (type.IsClass || type.IsInterface) return true;
            if (type.IsEnum) return false;
            if (type.IsValueType)
            {
                bool isClear;
                Monitor.Enter(isClearArrayLock);
                try
                {
                    if (isClearArrayCache.TryGetValue(type, out isClear)) return isClear;
                    isClearArrayCache.Add(type, isClear = isClearArray(type));
                }
                finally { Monitor.Exit(isClearArrayLock); }
                return isClear;
            }
            else
            {
                log.Default.Add(type.fullName(), new System.Diagnostics.StackFrame(), false);
                return true;
            }
        }
        /// <summary>
        /// 是否需要清除数组
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>需要清除数组</returns>
        private static bool isClearArray(Type type)
        {
            foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                Type fieldType = field.FieldType;
                if (fieldType != type && !fieldType.IsPointer)
                {
                    if (fieldType.IsClass || fieldType.IsInterface) return true;
                    if (!fieldType.IsEnum)
                    {
                        if (fieldType.IsValueType)
                        {
                            bool isClear;
                            if (!isClearArrayCache.TryGetValue(fieldType, out isClear))
                            {
                                isClearArrayCache.Add(fieldType, isClear = isClearArray(fieldType));
                            }
                            if (isClear) return true;
                        }
                        else
                        {
                            log.Default.Add(fieldType.fullName(), new System.Diagnostics.StackFrame(), false);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 是否需要清除数组缓存 访问锁
        /// </summary>
        private static readonly object isClearArrayLock = new object();
        /// <summary>
        /// 是否需要清除数组缓存信息
        /// </summary>
        private static readonly Dictionary<Type, bool> isClearArrayCache = dictionary.CreateOnly<Type, bool>();
    }
    /// <summary>
    /// 动态数组基类
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    public abstract class dynamicArray<valueType>
    {
        /// <summary>
        /// 是否需要清除数组
        /// </summary>
        internal static readonly bool IsClearArray = dynamicArray.IsClearArray(typeof(valueType));
        /// <summary>
        /// 创建新数组
        /// </summary>
        /// <param name="length">数组长度</param>
        /// <returns>数组</returns>
        internal static valueType[] GetNewArray(int length)
        {
            if (length > 0 && (length & (length - 1)) != 0) length = 1 << ((uint)length).bits();
            if (length <= 0) length = int.MaxValue;
            return new valueType[length];
        }

        /// <summary>
        /// 数据数组
        /// </summary>
        protected internal valueType[] array;
        /// <summary>
        /// 数据数组
        /// </summary>
        public valueType[] UnsafeArray
        {
            get { return array; }
            set { array = value; }
        }
        /// <summary>
        /// 是否只读
        /// </summary>
        public bool IsReadOnly { get { return false; } }
        /// <summary>
        /// 数据数量
        /// </summary>
        protected abstract int ValueCount { get; }
        
        /// <summary>
        /// 添加数据集合
        /// </summary>
        /// <param name="values">数据集合</param>
        /// <param name="index">起始位置</param>
        /// <param name="count">数量</param>
        public abstract void Add(valueType[] values, int index, int count);
        /// <summary>
        /// 添加数据集合
        /// </summary>
        /// <param name="values">数据集合</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Add(IEnumerable<valueType> values)
        {
            if (values != null) Add(values.getSubArray());
        }
        /// <summary>
        /// 添加数据集合
        /// </summary>
        /// <param name="values">数据集合</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Add(valueType[] values)
        {
            if (values != null) Add(values, 0, values.Length);
        }
        /// <summary>
        /// 添加数据集合
        /// </summary>
        /// <param name="value">数据集合</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Add(subArray<valueType> value)
        {
            if (value.length != 0) Add(value.array, value.startIndex, value.length);
        }
        /// <summary>
        /// 添加数据集合
        /// </summary>
        /// <param name="value">数据集合</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Add(list<valueType> value)
        {
            if (value.count() != 0) Add(value.array, 0, value.length);
        }
        /// <summary>
        /// 添加数据集合
        /// </summary>
        /// <param name="value">数据集合</param>
        /// <param name="index">起始位置</param>
        /// <param name="count">数量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Add(list<valueType> value, int index, int count)
        {
            if (value.count() != 0) Add(value.array, index, count);
        }
        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="value">数据</param>
        /// <returns>是否存在移除数据</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool Remove(valueType value)
        {
            int index = IndexOf(value);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 判断是否存在匹配
        /// </summary>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>是否存在匹配</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool Any(Func<valueType, bool> isValue)
        {
            if (isValue == null) log.Error.Throw(log.exceptionType.Null);
            return indexOf(isValue) != -1;
        }
        /// <summary>
        /// 获取匹配数据位置
        /// </summary>
        /// <param name="value">匹配数据</param>
        /// <returns>匹配位置,失败为-1</returns>
        public abstract int IndexOf(valueType value);
        /// <summary>
        /// 判断是否存在数据
        /// </summary>
        /// <param name="value">匹配数据</param>
        /// <returns>是否存在数据</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool Contains(valueType value)
        {
            return IndexOf(value) != -1;
        }
        /// <summary>
        /// 获取第一个匹配值
        /// </summary>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>匹配值,失败为 default(valueType)</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType FirstOrDefault(Func<valueType, bool> isValue)
        {
            if (isValue == null) log.Error.Throw(log.exceptionType.Null);
            int index = indexOf(isValue);
            return index != -1 ? array[index] : default(valueType);
        }
        /// <summary>
        /// 获取匹配值集合
        /// </summary>
        /// <param name="isValue">数据匹配器</param>
        /// <param name="map">匹配结果位图</param>
        /// <returns>匹配值集合</returns>
        protected abstract valueType[] getFindArray(Func<valueType, bool> isValue, fixedMap map);
        /// <summary>
        /// 获取匹配值集合
        /// </summary>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>匹配值集合</returns>
        public unsafe valueType[] GetFindArray(Func<valueType, bool> isValue)
        {
            if (isValue == null) log.Error.Throw(log.exceptionType.Null);
            int length = ValueCount;
            if (length != 0)
            {
                memoryPool pool = fastCSharp.memoryPool.GetDefaultPool(length = ((length + 31) >> 5) << 2);
                byte[] data = pool.Get(length);
                try
                {
                    fixed (byte* dataFixed = data)
                    {
                        Array.Clear(data, 0, length);
                        return getFindArray(isValue, new fixedMap(dataFixed));
                    }
                }
                finally { pool.PushNotNull(data); }
            }
            return nullValue<valueType>.Array;
        }
        /// <summary>
        /// 获取匹配位置
        /// </summary>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>匹配位置,失败为-1</returns>
        public abstract int IndexOf(Func<valueType, bool> isValue);
        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="index">数据位置</param>
        /// <returns>被移除数据</returns>
        public abstract valueType GetRemoveAt(int index);
        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="index">数据位置</param>
        /// <returns>被移除数据</returns>
        public abstract void RemoveAt(int index);
        /// <summary>
        /// 移除匹配值
        /// </summary>
        /// <param name="isValue">数据匹配器</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void RemoveFirst(Func<valueType, bool> isValue)
        {
            if (isValue == null) log.Error.Throw(log.exceptionType.Null);
            int index = IndexOf(isValue);
            if (index != -1) RemoveAt(index);
        }
        /// <summary>
        /// 移除匹配值
        /// </summary>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>被移除的数据元素,失败返回default(valueType)</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType GetRemoveFirst(Func<valueType, bool> isValue)
        {
            if (isValue == null) log.Error.Throw(log.exceptionType.Null);
            int index = IndexOf(isValue);
            return index != -1 ? GetRemoveAt(index) : default(valueType);
        }
        /// <summary>
        /// 获取数组中的匹配位置
        /// </summary>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>数组中的匹配位置,失败为-1</returns>
        protected abstract int indexOf(Func<valueType, bool> isValue);
        /// <summary>
        /// 替换数据
        /// </summary>
        /// <param name="value">新数据</param>
        /// <param name="isValue">数据匹配器</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void ReplaceFirst(valueType value, Func<valueType, bool> isValue)
        {
            if (isValue == null) log.Error.Throw(log.exceptionType.Null);
            int index = indexOf(isValue);
            if (index != -1) array[index] = value;
        }
        /// <summary>
        /// 获取数据范围
        /// </summary>
        /// <typeparam name="arrayType">目标数据类型</typeparam>
        /// <param name="index">起始位置</param>
        /// <param name="count">数量</param>
        /// <param name="getValue">数据转换器</param>
        /// <returns>目标数据</returns>
        protected abstract arrayType[] getRange<arrayType>(int index, int count, Func<valueType, arrayType> getValue);
        /// <summary>
        /// 获取数据分页
        /// </summary>
        /// <typeparam name="arrayType">目标数据类型</typeparam>
        /// <param name="pageSize">分页尺寸</param>
        /// <param name="currentPage">页号</param>
        /// <param name="getValue">数据转换器</param>
        /// <returns>目标数据</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public arrayType[] GetPage<arrayType>(int pageSize, int currentPage, Func<valueType, arrayType> getValue)
        {
            if (getValue == null) log.Error.Throw(log.exceptionType.Null);
            array.page page = new array.page(ValueCount, pageSize, currentPage);
            return page.SkipCount < page.Count ? getRange(page.SkipCount, page.CurrentPageSize, getValue) : nullValue<arrayType>.Array;
        }
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <param name="comparer">比较器</param>
        /// <param name="value">最大值</param>
        /// <returns>是否存在最大值</returns>
        public abstract bool Max(Func<valueType, valueType, int> comparer, out valueType value);
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="getKey">获取键值</param>
        /// <param name="comparer">比较器</param>
        /// <param name="value">最大值</param>
        /// <returns>是否存在最大值</returns>
        public abstract bool Max<keyType>
            (Func<valueType, keyType> getKey, Func<keyType, keyType, int> comparer, out valueType value);
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="getKey">获取键值</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最大值,失败返回默认空值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType Max<keyType>(Func<valueType, keyType> getKey, valueType nullValue)
            where keyType : IComparable<keyType>
        {
            valueType value;
            return Max(getKey, iComparable<keyType>.CompareToHandle, out value) ? value : nullValue;
        }
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="getKey">获取键值</param>
        /// <param name="comparer">比较器</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最大值,失败返回默认空值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType Max<keyType>
            (Func<valueType, keyType> getKey, Func<keyType, keyType, int> comparer, valueType nullValue)
        {
            valueType value;
            return Max(getKey, comparer, out value) ? value : nullValue;
        }
        /// <summary>
        /// 获取最大键值
        /// </summary>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="getKey">获取键值</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最大键值,失败返回默认空值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public keyType MaxKey<keyType>(Func<valueType, keyType> getKey, keyType nullValue)
            where keyType : IComparable<keyType>
        {
            valueType value;
            return Max(getKey, iComparable<keyType>.CompareToHandle, out value) ? getKey(value) : nullValue;
        }
        /// <summary>
        /// 获取最大键值
        /// </summary>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="getKey">获取键值</param>
        /// <param name="comparer">比较器</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最大键值,失败返回默认空值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public keyType MaxKey<keyType>
            (Func<valueType, keyType> getKey, Func<keyType, keyType, int> comparer, keyType nullValue)
        {
            valueType value;
            return Max(getKey, comparer, out value) ? getKey(value) : nullValue;
        }
        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <param name="comparer">比较器</param>
        /// <param name="value">最小值</param>
        /// <returns>是否存在最小值</returns>
        public abstract bool Min(Func<valueType, valueType, int> comparer, out valueType value);
        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="getKey">获取键值</param>
        /// <param name="comparer">比较器</param>
        /// <param name="value">最小值</param>
        /// <returns>是否存在最小值</returns>
        public abstract bool Min<keyType>
            (Func<valueType, keyType> getKey, Func<keyType, keyType, int> comparer, out valueType value);
        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="getKey">获取键值</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最小值,失败返回默认空值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType Min<keyType>(Func<valueType, keyType> getKey, valueType nullValue)
            where keyType : IComparable<keyType>
        {
            valueType value;
            return Min(getKey, iComparable<keyType>.CompareToHandle, out value) ? value : nullValue;
        }
        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="getKey">获取键值</param>
        /// <param name="comparer">比较器</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最小值,失败返回默认空值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public valueType Min<keyType>(Func<valueType, keyType> getKey, Func<keyType, keyType, int> comparer, valueType nullValue)
        {
            valueType value;
            return Min(getKey, comparer, out value) ? value : nullValue;
        }
        /// <summary>
        /// 获取最小键值
        /// </summary>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="getKey">获取键值</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最小键值,失败返回默认空值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public keyType MinKey<keyType>(Func<valueType, keyType> getKey, keyType nullValue)
            where keyType : IComparable<keyType>
        {
            valueType value;
            return Min(getKey, iComparable<keyType>.CompareToHandle, out value) ? getKey(value) : nullValue;
        }
        /// <summary>
        /// 获取最小键值
        /// </summary>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="getKey">获取键值</param>
        /// <param name="comparer">比较器</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最小键值,失败返回默认空值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public keyType MinKey<keyType>
            (Func<valueType, keyType> getKey, Func<keyType, keyType, int> comparer, keyType nullValue)
        {
            valueType value;
            return Min(getKey, comparer, out value) ? getKey(value) : nullValue;
        }
        /// <summary>
        /// 转换数据集合
        /// </summary>
        /// <typeparam name="arrayType">目标数据类型</typeparam>
        /// <param name="getValue">数据转换器</param>
        /// <returns>数据集合</returns>
        public abstract arrayType[] GetArray<arrayType>(Func<valueType, arrayType> getValue);
        /// <summary>
        /// 转换键值对集合
        /// </summary>
        /// <typeparam name="keyType">键类型</typeparam>
        /// <param name="getKey">键值获取器</param>
        /// <returns>键值对数组</returns>
        public abstract keyValue<keyType, valueType>[] GetKeyValueArray<keyType>(Func<valueType, keyType> getKey);
        /// <summary>
        /// 连接字符串
        /// </summary>
        /// <param name="toString">字符串转换器</param>
        /// <returns>字符串</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public string JoinString(Func<valueType, string> toString)
        {
            return string.Concat(GetArray(toString));
        }
        /// <summary>
        /// 连接字符串
        /// </summary>
        /// <param name="toString">字符串转换器</param>
        /// <param name="join">连接串</param>
        /// <returns>字符串</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public string JoinString(string join, Func<valueType, string> toString)
        {
            return string.Join(join, GetArray(toString));
        }
        /// <summary>
        /// 连接字符串
        /// </summary>
        /// <param name="toString">字符串转换器</param>
        /// <param name="join">连接字符</param>
        /// <returns>字符串</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public string JoinString(char join, Func<valueType, string> toString)
        {
            return GetArray(toString).joinString(join);
        }
    }
}
