using System;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 唯一静态哈希字典
    /// </summary>
    /// <typeparam name="keyType">键值类型</typeparam>
    /// <typeparam name="valueType">数据类型</typeparam>
    public sealed class uniqueDictionary<keyType, valueType> where keyType : struct, IEquatable<keyType>
    {
        /// <summary>
        /// 哈希数据数组
        /// </summary>
        private keyValue<keyType, valueType>[] array;
        /// <summary>
        /// 唯一静态哈希字典
        /// </summary>
        /// <param name="keys">键值数据集合</param>
        /// <param name="getValue">数据获取器</param>
        /// <param name="size">哈希容器尺寸</param>
        public unsafe uniqueDictionary(keyType[] keys, Func<keyType, valueType> getValue, int size)
        {
            int count = keys.length();
            if (count > size || size <= 0) log.Error.Throw(log.exceptionType.IndexOutOfRange);
            if (getValue == null) log.Error.Throw(log.exceptionType.Null);
            array = new keyValue<keyType, valueType>[size];
            if (count != 0)
            {
                int length = ((size + 31) >> 5) << 2;
                byte* isValue = stackalloc byte[length];
                fixedMap map = new fixedMap(isValue, length, 0);
                foreach (keyType key in keys)
                {
                    int index = key.GetHashCode();
                    if ((uint)index >= size) log.Error.Throw(log.exceptionType.IndexOutOfRange);
                    if (map.Get(index)) log.Error.Throw(log.exceptionType.ErrorOperation);
                    map.Set(index);
                    array[index] = new keyValue<keyType, valueType>(key, getValue(key));
                }
            }
        }
        /// <summary>
        /// 唯一静态哈希字典
        /// </summary>
        /// <param name="values">数据集合</param>
        /// <param name="size">哈希容器尺寸</param>
        public uniqueDictionary(list<keyValue<keyType, valueType>> values, int size)
        {
            int count = values.count();
            if (count > size || size <= 0) log.Error.Throw(log.exceptionType.IndexOutOfRange);
            array = new keyValue<keyType, valueType>[size];
            if (count != 0) fromArray(values.array, count, size);
        }
        /// <summary>
        /// 唯一静态哈希字典
        /// </summary>
        /// <param name="values">数据集合</param>
        /// <param name="size">哈希容器尺寸</param>
        public uniqueDictionary(keyValue<keyType, valueType>[] values, int size)
        {
            int count = values.length();
            if (count > size || size <= 0) log.Error.Throw(log.exceptionType.IndexOutOfRange);
            array = new keyValue<keyType, valueType>[size];
            if (count != 0) fromArray(values, count, size);
        }
        /// <summary>
        /// 唯一静态哈希字典
        /// </summary>
        /// <param name="values">数据集合</param>
        /// <param name="count">数据数量</param>
        /// <param name="size">哈希容器尺寸</param>
        private unsafe void fromArray(keyValue<keyType, valueType>[] values, int count, int size)
        {
            int length = ((size + 31) >> 5) << 2;
            byte* isValue = stackalloc byte[length];
            fixedMap map = new fixedMap(isValue, length, 0);
            do
            {
                keyValue<keyType, valueType> value = values[--count];
                int index = value.Key.GetHashCode();
                if ((uint)index >= size) log.Error.Throw(log.exceptionType.IndexOutOfRange);
                if (map.Get(index)) log.Error.Throw(log.exceptionType.ErrorOperation);
                map.Set(index);
                array[index] = value;
            }
            while (count != 0);
        }
        /// <summary>
        /// 判断是否存在某键值
        /// </summary>
        /// <param name="key">待匹配键值</param>
        /// <returns>是否存在某键值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool ContainsKey(keyType key)
        {
            int index = key.GetHashCode();
            return (uint)index < array.Length && key.Equals(array[index].Key);
        }
        /// <summary>
        /// 获取匹配数据
        /// </summary>
        /// <param name="key">哈希键值</param>
        /// <param name="nullValue">默认空值</param>
        /// <returns>匹配数据,失败返回默认空值</returns>
        public valueType Get(keyType key, valueType nullValue)
        {
            int index = key.GetHashCode();
            if ((uint)index < array.Length)
            {
                keyValue<keyType, valueType> value = array[index];
                if (key.Equals(value.Key)) return value.Value;
            }
            return nullValue;
        }
    }
}