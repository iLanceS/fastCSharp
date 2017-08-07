using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 可枚举相关扩展
    /// </summary>
    public static partial class iEnumerable
    {
        /// <summary>
        /// 转数组
        /// </summary>
        /// <typeparam name="valueType">数据集合类型</typeparam>
        /// <param name="values">数据集合</param>
        /// <returns>数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType[] getArray<valueType>(this IEnumerable<valueType> values)
        {
            return values.getSubArray().ToArray();
        }
        /// <summary>
        /// 转单向动态数组
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="values">数据枚举集合</param>
        /// <returns>单向动态数组</returns>
        public static subArray<valueType> getSubArray<valueType>(this IEnumerable<valueType> values)
        {
            if (values != null)
            {
                int count = 0;
                valueType[] newValues = new valueType[sizeof(int)];
                foreach (valueType value in values)
                {
                    if (count == newValues.Length)
                    {
                        valueType[] nextValues = new valueType[count << 1];
                        newValues.CopyTo(nextValues, 0);
                        newValues = nextValues;
                    }
                    newValues[count++] = value;
                }
                return subArray<valueType>.Unsafe(newValues, 0, count);
            }
            return default(subArray<valueType>);
        }
        /// <summary>
        /// 转单向动态数组
        /// </summary>
        /// <typeparam name="valueType">枚举值类型</typeparam>
        /// <typeparam name="arrayType">返回数组类型</typeparam>
        /// <param name="values">数据枚举集合</param>
        /// <param name="getValue">获取数组值的委托</param>
        /// <returns>单向动态数组</returns>
        public static subArray<arrayType> getSubArray<valueType, arrayType>
            (this IEnumerable<valueType> values, Func<valueType, arrayType> getValue)
        {
            if (values != null)
            {
                if (getValue == null) log.Error.Throw(log.exceptionType.Null);
                int count = 0;
                arrayType[] newValues = new arrayType[sizeof(int)];
                foreach (valueType value in values)
                {
                    if (count == newValues.Length)
                    {
                        arrayType[] nextValues = new arrayType[count << 1];
                        newValues.CopyTo(nextValues, 0);
                        newValues = nextValues;
                    }
                    newValues[count++] = getValue(value);
                }
                return subArray<arrayType>.Unsafe(newValues, 0, count);
            }
            return default(subArray<arrayType>);
        }
        /// <summary>
        /// 转单向动态数组
        /// </summary>
        /// <typeparam name="valueType">枚举值类型</typeparam>
        /// <typeparam name="arrayType">返回数组类型</typeparam>
        /// <param name="values">数据枚举集合</param>
        /// <param name="getValue">获取数组值的委托</param>
        /// <returns>单向动态数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static arrayType[] getArray<valueType, arrayType>
            (this IEnumerable<valueType> values, Func<valueType, arrayType> getValue)
        {
            return values.getSubArray(getValue).ToArray();
        }
        /// <summary>
        /// 转换成字典
        /// </summary>
        /// <typeparam name="valueType">枚举值类型</typeparam>
        /// <typeparam name="keyType">哈希键值类型</typeparam>
        /// <param name="values">值集合</param>
        /// <param name="getKey">键值获取器</param>
        /// <returns>字典</returns>
        public static Dictionary<keyType, valueType> getDictionary<valueType, keyType>
            (this IEnumerable<valueType> values, Func<valueType, keyType> getKey)
            where keyType : IEquatable<keyType>
        {
            if (values != null)
            {
                if (getKey == null) log.Error.Throw(log.exceptionType.Null);
                Dictionary<keyType, valueType> dictionary = dictionary<keyType>.Create<valueType>();
                foreach (valueType value in values) dictionary[getKey(value)] = value;
                return dictionary;
            }
            return null;
        }
        /// <summary>
        /// 转换成字典
        /// </summary>
        /// <typeparam name="valueType">枚举值类型</typeparam>
        /// <typeparam name="keyType">哈希键值类型</typeparam>
        /// <typeparam name="dictionaryValueType">哈希值类型</typeparam>
        /// <param name="values">值集合</param>
        /// <param name="getKey">键值获取器</param>
        /// <param name="getValue">哈希值获取器</param>
        /// <returns>字典</returns>
        public static Dictionary<keyType, dictionaryValueType> getDictionary<valueType, keyType, dictionaryValueType>
            (this IEnumerable<valueType> values, Func<valueType, keyType> getKey, Func<valueType, dictionaryValueType> getValue)
            where keyType : IEquatable<keyType>
        {
            if (values != null)
            {
                if (getKey == null) log.Error.Throw(log.exceptionType.Null);
                Dictionary<keyType, dictionaryValueType> dictionary = dictionary<keyType>.Create<dictionaryValueType>();
                foreach (valueType value in values) dictionary[getKey(value)] = getValue(value);
                return dictionary;
            }
            return null;
        }
        /// <summary>
        /// 查找符合条件的记录集合
        /// </summary>
        /// <typeparam name="valueType">值类型</typeparam>
        /// <param name="values">值集合</param>
        /// <param name="isValue">判断记录是否符合条件的委托</param>
        /// <returns>符合条件的记录集合</returns>
        public static subArray<valueType> getFind<valueType>
            (this IEnumerable<valueType> values, Func<valueType, bool> isValue)
        {
            if (values != null)
            {
                if (isValue == null) log.Error.Throw(log.exceptionType.Null);
                subArray<valueType> value = new subArray<valueType>();
                foreach (valueType nextValue in values)
                {
                    if (isValue(nextValue)) value.Add(nextValue);
                }
                return value;
            }
            return default(subArray<valueType>);
        }
        /// <summary>
        /// 查找符合条件的记录集合
        /// </summary>
        /// <typeparam name="valueType">值类型</typeparam>
        /// <param name="values">值集合</param>
        /// <param name="isValue">判断记录是否符合条件的委托</param>
        /// <returns>符合条件的记录集合</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType[] getFindArray<valueType>
            (this IEnumerable<valueType> values, Func<valueType, bool> isValue)
        {
            return values.getFind<valueType>(isValue).ToArray();
        }
        /// <summary>
        /// 根据集合内容返回哈希表
        /// </summary>
        /// <typeparam name="valueType">枚举值类型</typeparam>
        /// <param name="values">值集合</param>
        /// <returns>哈希表</returns>
        public static HashSet<valueType> getHash<valueType>(this IEnumerable<valueType> values)
        {
            if (values != null)
            {
                HashSet<valueType> newValues = hashSet<valueType>.Create();
                foreach (valueType value in values) newValues.Add(value);
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
        /// <param name="capacity">集合容器大小</param>
        /// <returns>分组计数</returns>
        public static Dictionary<keyType, int> groupCount<valueType, keyType>
            (this IEnumerable<valueType> values, Func<valueType, keyType> getKey, int capacity)
            where keyType : IEquatable<keyType>
        {
            if (values != null)
            {
                if (getKey == null) log.Error.Throw(log.exceptionType.Null);
                int count;
                Dictionary<keyType, int> dictionary = capacity > 0 ? dictionary<keyType>.Create<int>(capacity) : dictionary<keyType>.Create<int>(capacity);
                foreach (valueType value in values)
                {
                    keyType key = getKey(value);
                    if (dictionary.TryGetValue(key, out count)) dictionary[key] = count + 1;
                    else dictionary.Add(key, 1);
                }
                return dictionary;
            }
            return null;
        }
    }
}
