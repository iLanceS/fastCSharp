using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// IEnumerable泛型转换
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    public struct iEnumerable<valueType> : IEnumerable<valueType>
    {
        /// <summary>
        /// IEnumerable数据集合
        /// </summary>
        private IEnumerable enumerable;
        /// <summary>
        /// IEnumerable泛型转换
        /// </summary>
        /// <param name="enumerable">ICollection数据集合</param>
        public iEnumerable(IEnumerable enumerable)
        {
            this.enumerable = enumerable;
        }
        /// <summary>
        /// 枚举器
        /// </summary>
        /// <returns>枚举器</returns>
        IEnumerator<valueType> IEnumerable<valueType>.GetEnumerator()
        {
            if (enumerable != null)
            {
                foreach (valueType value in enumerable) yield return value;
            }
        }
        /// <summary>
        /// 枚举器
        /// </summary>
        /// <returns>枚举器</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<valueType>)this).GetEnumerator();
        }
    }
    /// <summary>
    /// 可枚举相关扩展
    /// </summary>
    public static class iEnumerableExpand
    {
        /// <summary>
        /// IEnumerable泛型转换
        /// </summary>
        /// <param name="value">数据集合</param>
        /// <returns>泛型数据集合</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static iEnumerable<valueType> toGeneric<valueType>(this IEnumerable value)
        {
            return new iEnumerable<valueType>(value);
        }
        /// <summary>
        /// 单个数据转枚举器
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="value">数据值</param>
        /// <returns>枚举器</returns>
        public static IEnumerable<valueType> toEnumerable<valueType>(this valueType value)
        {
            yield return value;
        }
        /// <summary>
        /// 空值转数组
        /// </summary>
        /// <typeparam name="valueType">数据集合类型</typeparam>
        /// <param name="value">数据集合</param>
        /// <returns>非空数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static IEnumerable<valueType> notNull<valueType>(this IEnumerable<valueType> value)
        {
            return value != null ? value : nullValue<valueType>.Array;
        }
        /// <summary>
        /// 获取数据数量
        /// </summary>
        /// <param name="value">数据集合</param>
        /// <returns>null为0</returns>
        public static int count<valueType>(this IEnumerable<valueType> value)
        {
            if (value != null)
            {
                int count = 0;
                for (IEnumerator<valueType> enumerator = value.GetEnumerator(); enumerator.MoveNext(); ++count) ;
                return count;
            }
            return 0;
        }
        /// <summary>
        /// 转单向动态数组
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="values">数据枚举集合</param>
        /// <returns>单向动态数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static list<valueType> getList<valueType>(this IEnumerable<valueType> values)
        {
            subArray<valueType> newValues = values.getSubArray();
            return newValues.UnsafeArray == null ? null : new list<valueType>(newValues.UnsafeArray, 0, newValues.Count, true);
        }
        /// <summary>
        /// 转双向动态数组
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="values">数据枚举集合</param>
        /// <returns>双向动态数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static collection<valueType> getCollection<valueType>(this IEnumerable<valueType> values)
        {
            return values != null ? new collection<valueType>(values.getSubArray(), true) : null;
        }
        /// <summary>
        /// 根据集合内容返回新的数据集合
        /// </summary>
        /// <typeparam name="valueType">枚举值类型</typeparam>
        /// <typeparam name="arrayType">返回数组类型</typeparam>
        /// <param name="values">值集合</param>
        /// <param name="getValue">获取数组值的委托</param>
        /// <returns>数据集合</returns>
        public static IEnumerable<arrayType> getEnumerable<valueType, arrayType>(this IEnumerable<valueType> values, Func<valueType, arrayType> getValue)
        {
            if (values != null)
            {
                foreach (valueType value in values) yield return getValue(value);
            }
        }
        /// <summary>
        /// 根据集合内容返回双向列表
        /// </summary>
        /// <typeparam name="valueType">枚举值类型</typeparam>
        /// <typeparam name="arrayType">返回数组类型</typeparam>
        /// <param name="values">值集合</param>
        /// <param name="getValue">获取数组值的委托</param>
        /// <returns>双向列表</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static collection<arrayType> getCollection<valueType, arrayType>
            (this IEnumerable<valueType> values, Func<valueType, arrayType> getValue)
        {
            return values != null ? new collection<arrayType>(values.getSubArray(getValue), true) : null;
        }
        /// <summary>
        /// 转换成字典
        /// </summary>
        /// <typeparam name="keyType">哈希键值类型</typeparam>
        /// <typeparam name="valueType">枚举值类型</typeparam>
        /// <param name="keys">键值集合</param>
        /// <param name="getValue">数据获取器</param>
        /// <returns>字典</returns>
        public static Dictionary<keyType, valueType> getDictionaryByKey<keyType, valueType>
            (this IEnumerable<keyType> keys, Func<keyType, valueType> getValue)
            where keyType : IEquatable<keyType>
        {
            if (keys != null)
            {
                if (getValue == null) log.Error.Throw(log.exceptionType.Null);
                Dictionary<keyType, valueType> dictionary = dictionary<keyType>.Create<valueType>();
                foreach (keyType key in keys) dictionary[key] = getValue(key);
                return dictionary;
            }
            return null;
        }
        /// <summary>
        /// 获取匹配数据数量
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="values">数据集合</param>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>null为0</returns>
        public static int count<valueType>(this IEnumerable<valueType> values, Func<valueType, bool> isValue)
        {
            if (isValue == null) log.Error.Throw(log.exceptionType.Null);
            if (values != null)
            {
                int count = 0;
                foreach (valueType value in values)
                {
                    if (isValue(value)) ++count;
                }
                return count;
            }
            return 0;
        }
        /// <summary>
        /// 获取第一个匹配值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="values">数据集合</param>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>第一个匹配值,失败返回default(valueType)</returns>
        public static valueType firstOrDefault<valueType>(this IEnumerable<valueType> values, Func<valueType, bool> isValue)
        {
            foreach (valueType value in values)
            {
                if (isValue(value)) return value;
            }
            return default(valueType);
        }
        /// <summary>
        /// 判断是否存在匹配值
        /// </summary>
        /// <typeparam name="valueType">枚举值类型</typeparam>
        /// <param name="values">值集合</param>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>是否存在匹配值</returns>
        public static bool any<valueType>(this IEnumerable<valueType> values, Func<valueType, bool> isValue)
        {
            if (isValue == null) log.Error.Throw(log.exceptionType.Null);
            foreach (valueType value in values)
            {
                if (isValue(value)) return true;
            }
            return false;
        }
        /// <summary>
        /// 取子集合
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="values">值集合</param>
        /// <param name="index">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns>子集合</returns>
        public static subArray<valueType> getSub<valueType>(this IEnumerable<valueType> values, int index, int count)
        {
            if (values != null)
            {
                if (index < 0 || count < 0) log.Error.Throw(log.exceptionType.IndexOutOfRange);
                if (count != 0)
                {
                    valueType[] newValues = new valueType[count];
                    count = 0;
                    foreach (valueType value in values)
                    {
                        if (index != 0) --index;
                        else
                        {
                            newValues[count] = value;
                            if (++count == newValues.Length) break;
                        }
                    }
                    return subArray<valueType>.Unsafe(newValues, 0, count);
                }
            }
            return default(subArray<valueType>);
        }
        /// <summary>
        /// 求和
        /// </summary>
        /// <param name="values">数据集合</param>
        /// <param name="getValue">获取求和值委托</param>
        /// <returns>和</returns>
        public static long sum<valueType>(this IEnumerable<valueType> values, Func<valueType, int> getValue)
        {
            if (values != null)
            {
                long sum = 0;
                foreach (valueType value in values) sum += getValue(value);
                return sum;
            }
            return 0;
        }
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="values">数组数据</param>
        /// <param name="comparer">比较器</param>
        /// <param name="value">最大值</param>
        /// <returns>是否存在最大值</returns>
        public static bool max<valueType>(this IEnumerable<valueType> values, Func<valueType, valueType, int> comparer, out valueType value)
        {
            value = default(valueType);
            if (values != null)
            {
                if (comparer == null) log.Error.Throw(log.exceptionType.Null);
                int index = -1;
                foreach (valueType nextValue in values)
                {
                    if (++index == 0) value = nextValue;
                    else if (comparer(nextValue, value) > 0) value = nextValue;
                }
                if (index != -1) return true;
            }
            return false;
        }
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="values">数组数据</param>
        /// <param name="getKey">获取键值</param>
        /// <param name="comparer">比较器</param>
        /// <param name="value">最大值</param>
        /// <returns>是否存在最大值</returns>
        public static bool max<valueType, keyType>
            (this IEnumerable<valueType> values, Func<valueType, keyType> getKey, Func<keyType, keyType, int> comparer, out valueType value)
        {
            value = default(valueType);
            if (values != null)
            {
                if (getKey == null || comparer == null) log.Error.Throw(log.exceptionType.Null);
                int index = -1;
                keyType key = default(keyType);
                foreach (valueType nextValue in values)
                {
                    if (++index == 0) key = getKey(value = nextValue);
                    else
                    {
                        keyType nextKey = getKey(nextValue);
                        if (comparer(nextKey, key) > 0)
                        {
                            value = nextValue;
                            key = nextKey;
                        }
                    }
                }
                if (index != -1) return true;
            }
            return false;
        }
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="values">数组数据</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最大值,失败返回默认空值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType max<valueType>(this IEnumerable<valueType> values, valueType nullValue)
            where valueType : IComparable<valueType>
        {
            valueType value;
            return max(values, iComparable<valueType>.CompareToHandle, out value) ? value : nullValue;
        }
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="values">数组数据</param>
        /// <param name="getKey">获取键值</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最大值,失败返回默认空值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType max<valueType, keyType>(this IEnumerable<valueType> values, Func<valueType, keyType> getKey, valueType nullValue)
            where keyType : IComparable<keyType>
        {
            valueType value;
            return max(values, getKey, iComparable<keyType>.CompareToHandle, out value) ? value : nullValue;
        }
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="values">数组数据</param>
        /// <param name="getKey">获取键值</param>
        /// <param name="comparer">比较器</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最大值,失败返回默认空值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType max<valueType, keyType>
            (this IEnumerable<valueType> values, Func<valueType, keyType> getKey, Func<keyType, keyType, int> comparer, valueType nullValue)
        {
            valueType value;
            return max(values, getKey, comparer, out value) ? value : nullValue;
        }
        /// <summary>
        /// 获取最大键值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="values">数组数据</param>
        /// <param name="getKey">获取键值</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最大键值,失败返回默认空值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static keyType maxKey<valueType, keyType>(this IEnumerable<valueType> values, Func<valueType, keyType> getKey, keyType nullValue)
            where keyType : IComparable<keyType>
        {
            valueType value;
            return max(values, getKey, iComparable<keyType>.CompareToHandle, out value) ? getKey(value) : nullValue;
        }
        /// <summary>
        /// 获取最大键值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="values">数组数据</param>
        /// <param name="getKey">获取键值</param>
        /// <param name="comparer">比较器</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最大键值,失败返回默认空值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static keyType maxKey<valueType, keyType>
            (this IEnumerable<valueType> values, Func<valueType, keyType> getKey, Func<keyType, keyType, int> comparer, keyType nullValue)
        {
            valueType value;
            return max(values, getKey, comparer, out value) ? getKey(value) : nullValue;
        }
        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="values">数组数据</param>
        /// <param name="comparer">比较器</param>
        /// <param name="value">最小值</param>
        /// <returns>是否存在最小值</returns>
        public static bool min<valueType>(this IEnumerable<valueType> values, Func<valueType, valueType, int> comparer, out valueType value)
        {
            value = default(valueType);
            if (values != null)
            {
                if (comparer == null) log.Error.Throw(log.exceptionType.Null);
                int index = -1;
                foreach (valueType nextValue in values)
                {
                    if (++index == 0) value = nextValue;
                    else if (comparer(nextValue, value) < 0) value = nextValue;
                }
                if (index != -1) return true;
            }
            return false;
        }
        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="values">数组数据</param>
        /// <param name="getKey">获取键值</param>
        /// <param name="comparer">比较器</param>
        /// <param name="value">最小值</param>
        /// <returns>是否存在最小值</returns>
        public static bool min<valueType, keyType>
            (this IEnumerable<valueType> values, Func<valueType, keyType> getKey, Func<keyType, keyType, int> comparer, out valueType value)
        {
            value = default(valueType);
            if (values != null)
            {
                if (getKey == null || comparer == null) log.Error.Throw(log.exceptionType.Null);
                int index = -1;
                keyType key = default(keyType);
                foreach (valueType nextValue in values)
                {
                    if (++index == 0) key = getKey(value = nextValue);
                    else
                    {
                        keyType nextKey = getKey(nextValue);
                        if (comparer(nextKey, key) < 0)
                        {
                            value = nextValue;
                            key = nextKey;
                        }
                    }
                }
                if (index != -1) return true;
            }
            return false;
        }
        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="values">数组数据</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最小值,失败返回默认空值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType min<valueType>(this IEnumerable<valueType> values, valueType nullValue)
            where valueType : IComparable<valueType>
        {
            valueType value;
            return min(values, iComparable<valueType>.CompareToHandle, out value) ? value : nullValue;
        }
        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="values">数组数据</param>
        /// <param name="getKey">获取键值</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最小值,失败返回默认空值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType min<valueType, keyType>(this IEnumerable<valueType> values, Func<valueType, keyType> getKey, valueType nullValue)
            where keyType : IComparable<keyType>
        {
            valueType value;
            return min(values, getKey, iComparable<keyType>.CompareToHandle, out value) ? value : nullValue;
        }
        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="values">数组数据</param>
        /// <param name="getKey">获取键值</param>
        /// <param name="comparer">比较器</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最小值,失败返回默认空值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType min<valueType, keyType>
            (this IEnumerable<valueType> values, Func<valueType, keyType> getKey, Func<keyType, keyType, int> comparer, valueType nullValue)
        {
            valueType value;
            return min(values, getKey, comparer, out value) ? value : nullValue;
        }
        /// <summary>
        /// 获取最小键值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="values">数组数据</param>
        /// <param name="getKey">获取键值</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最小键值,失败返回默认空值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static keyType minKey<valueType, keyType>(this IEnumerable<valueType> values, Func<valueType, keyType> getKey, keyType nullValue)
            where keyType : IComparable<keyType>
        {
            valueType value;
            return min(values, getKey, iComparable<keyType>.CompareToHandle, out value) ? getKey(value) : nullValue;
        }
        /// <summary>
        /// 获取最小键值
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="keyType">比较键类型</typeparam>
        /// <param name="values">数组数据</param>
        /// <param name="getKey">获取键值</param>
        /// <param name="comparer">比较器</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最小键值,失败返回默认空值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static keyType minKey<valueType, keyType>
            (this IEnumerable<valueType> values, Func<valueType, keyType> getKey, Func<keyType, keyType, int> comparer, keyType nullValue)
        {
            valueType value;
            return min(values, getKey, comparer, out value) ? getKey(value) : nullValue;
        }
        /// <summary>
        /// 根据集合内容返回哈希表
        /// </summary>
        /// <typeparam name="valueType">枚举值类型</typeparam>
        /// <typeparam name="hashType">返回哈希表类型</typeparam>
        /// <param name="values">值集合</param>
        /// <param name="getValue">获取数组值的委托</param>
        /// <returns>哈希表</returns>
        public static HashSet<hashType> getHash<valueType, hashType>
            (this IEnumerable<valueType> values, Func<valueType, hashType> getValue)
        {
            if (values != null)
            {
                if (getValue == null) log.Error.Throw(log.exceptionType.Null);
                HashSet<hashType> newValues = hashSet<hashType>.Create();
                foreach (valueType value in values) newValues.Add(getValue(value));
                return newValues;
            }
            return null;
        }
        /// <summary>
        /// 根据集合内容返回哈希表
        /// </summary>
        /// <typeparam name="valueType">枚举值类型</typeparam>
        /// <typeparam name="hashType">返回哈希表类型</typeparam>
        /// <param name="values">值集合</param>
        /// <param name="hash">哈希表</param>
        /// <param name="getValue">获取数组值的委托</param>
        /// <returns>哈希表</returns>
        public static HashSet<hashType> fillHash<valueType, hashType>
            (this IEnumerable<valueType> values, HashSet<hashType> hash, Func<valueType, hashType> getValue)
        {
            if (values != null)
            {
                if (getValue == null) log.Error.Throw(log.exceptionType.Null);
                if (hash == null) hash = hashSet<hashType>.Create();
                foreach (valueType value in values) hash.Add(getValue(value));
            }
            return hash;
        }
        /// <summary>
        /// 根据集合内容返回哈希表
        /// </summary>
        /// <typeparam name="valueType">枚举值类型</typeparam>
        /// <param name="values">值集合</param>
        /// <param name="hash">哈希表</param>
        /// <returns>哈希表</returns>
        public static HashSet<valueType> fillHash<valueType>(this IEnumerable<valueType> values, HashSet<valueType> hash)
        {
            if (values != null)
            {
                if (hash == null) hash = hashSet<valueType>.Create();
                foreach (valueType value in values) hash.Add(value);
            }
            return hash;
        }
        /// <summary>
        /// 数据分组
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="keyType">分组键值类型</typeparam>
        /// <param name="values">数据集合</param>
        /// <param name="getKey">键值获取器</param>
        /// <returns>分组数据</returns>
        public static Dictionary<keyType, list<valueType>> group<valueType, keyType>
            (this IEnumerable<valueType> values, Func<valueType, keyType> getKey)
            where keyType : IEquatable<keyType>
        {
            if (values != null)
            {
                if (getKey == null) log.Error.Throw(log.exceptionType.Null);
                Dictionary<keyType, list<valueType>> newValues = dictionary<keyType>.Create<list<valueType>>();
                list<valueType> value;
                foreach (valueType nextValue in values)
                {
                    keyType key = getKey(nextValue);
                    if (!newValues.TryGetValue(key, out value)) newValues[key] = value = new list<valueType>();
                    value.Add(nextValue);
                }
                return newValues;
            }
            return null;
        }
        /// <summary>
        /// 数据分组
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="keyType">分组键值类型</typeparam>
        /// <param name="values">数据集合</param>
        /// <param name="getKey">键值获取器</param>
        /// <param name="isLeftValue">唯一值判定器</param>
        /// <returns>分组数据</returns>
        public static Dictionary<keyType, valueType> groupDistinct<valueType, keyType>
           (this IEnumerable<valueType> values, Func<valueType, keyType> getKey, Func<valueType, valueType, bool> isLeftValue)
            where keyType : IEquatable<keyType>
        {
            if (values != null)
            {
                if (getKey == null || isLeftValue == null) log.Error.Throw(log.exceptionType.Null);
                Dictionary<keyType, valueType> newValues = dictionary<keyType>.Create<valueType>();
                valueType value;
                foreach (valueType nextValue in values)
                {
                    keyType key = getKey(nextValue);
                    if (newValues.TryGetValue(key, out value))
                    {
                        if (isLeftValue(nextValue, value)) newValues[key] = nextValue;
                    }
                    else newValues.Add(key, nextValue);
                }
                return newValues;
            }
            return null;
        }
        /// <summary>
        /// 分组计数
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <typeparam name="keyType">分组键值类型</typeparam>
        /// <param name="values">数据集合</param>
        /// <param name="getKey">键值获取器</param>
        /// <returns>分组计数</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static Dictionary<keyType, int> groupCount<valueType, keyType>
            (this IEnumerable<valueType> values, Func<valueType, keyType> getKey)
            where keyType : IEquatable<keyType>
        {
            return values.groupCount(getKey, 0);
        }
        /// <summary>
        /// 连接字符串
        /// </summary>
        /// <param name="values">数据集合</param>
        /// <param name="toString">字符串转换器</param>
        /// <returns>字符串</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static string joinString<valueType>(this IEnumerable<valueType> values, Func<valueType, string> toString)
        {
            return string.Concat(values.getSubArray(toString).ToArray());
        }
        /// <summary>
        /// 连接字符串
        /// </summary>
        /// <param name="values">数据集合</param>
        /// <param name="toString">字符串转换器</param>
        /// <param name="join">连接串</param>
        /// <returns>字符串</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static string joinString<valueType>(this IEnumerable<valueType> values, string join, Func<valueType, string> toString)
        {
            return string.Join(join, values.getSubArray(toString).ToArray());
        }
        /// <summary>
        /// 连接字符串
        /// </summary>
        /// <param name="values">数据集合</param>
        /// <param name="toString">字符串转换器</param>
        /// <param name="join">连接字符</param>
        /// <returns>字符串</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static string joinString<valueType>(this IEnumerable<valueType> values, char join, Func<valueType, string> toString)
        {
            return values.getSubArray(toString).joinString(join);
        }
    }
}
