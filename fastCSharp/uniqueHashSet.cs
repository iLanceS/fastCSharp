using System;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 唯一静态哈希
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    public sealed class uniqueHashSet<valueType> where valueType : struct, IEquatable<valueType>
    {
        /// <summary>
        /// 哈希数据数组
        /// </summary>
        private valueType[] array;
        /// <summary>
        /// 唯一静态哈希
        /// </summary>
        /// <param name="values">数据集合</param>
        /// <param name="size">哈希容器尺寸</param>
        public unsafe uniqueHashSet(valueType[] values, int size)
        {
            if (values.length() > size || size <= 0) log.Error.Throw(log.exceptionType.IndexOutOfRange);
            array = new valueType[size];
            int length = ((size + 31) >> 5) << 2;
            byte* isValue = stackalloc byte[length];
            fixedMap map = new fixedMap(isValue, length, 0);
            foreach (valueType value in values)
            {
                int index = value.GetHashCode();
                if ((uint)index >= size) log.Error.Throw(log.exceptionType.IndexOutOfRange);
                if (map.Get(index)) log.Error.Throw(log.exceptionType.ErrorOperation);
                map.Set(index);
                array[index] = value;
            }
        }
        /// <summary>
        /// 判断是否存在某值
        /// </summary>
        /// <param name="value">待匹配值</param>
        /// <returns>是否存在某值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool Contains(valueType value)
        {
            int index = value.GetHashCode();
            return (uint)index < array.Length && value.Equals(array[index]);
        }
    }
}
