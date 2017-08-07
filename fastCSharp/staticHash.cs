using System;

namespace fastCSharp
{
    /// <summary>
    /// 静态哈希基类
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    public abstract class staticHash<valueType> : staticHashIndex
    {
        /// <summary>
        /// 哈希数据数组
        /// </summary>
        protected valueType[] array;
        /// <summary>
        /// 是否空集合
        /// </summary>
        /// <returns>是否空集合</returns>
        public unsafe bool IsEmpty()
        {
            fixed (range* indexFixed = indexs)
            {
                for (range* index = indexFixed + indexs.Length; index != indexFixed; )
                {
                    --index;
                    if ((*index).StartIndex != (*index).EndIndex) return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 是否存在匹配数据
        /// </summary>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>是否存在匹配数据</returns>
        public unsafe bool IsExist(Func<valueType, bool> isValue)
        {
            fixed (range* indexFixed = indexs)
            {
                for (range* index = indexFixed + indexs.Length; index != indexFixed; )
                {
                    for (range range = *--index; range.StartIndex != range.EndIndex; ++range.StartIndex)
                    {
                        if (isValue(array[range.StartIndex])) return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 移除数据
        /// </summary>
        /// <typeparam name="keyType">哈希键类型</typeparam>
        /// <param name="key">哈希键值</param>
        /// <param name="getKey">哈希键值获取器</param>
        /// <param name="value">被移除数据</param>
        /// <returns>是否存在被移除数据</returns>
        protected bool remove<keyType>(keyType key, Func<valueType, keyType> getKey, out valueType value)
            where keyType : IEquatable<keyType>
        {
            int index = (getHashCode(key) & indexAnd);
            for (range range = indexs[index]; range.StartIndex != range.EndIndex; ++range.StartIndex)
            {
                value = array[range.StartIndex];
                if (getKey(value).Equals(key))
                {
                    indexs[index].EndIndex = --range.EndIndex;
                    array[range.StartIndex] = array[range.EndIndex];
                    return true;
                }
            }
            value = default(valueType);
            return false;
        }
        /// <summary>
        /// 数据转换成单向动态数组
        /// </summary>
        /// <returns>单向动态数组</returns>
        public unsafe subArray<valueType> GetList()
        {
            subArray<valueType> values = new subArray<valueType>(array.Length);
            fixed (range* indexFixed = indexs)
            {
                for (range* index = indexFixed + indexs.Length; index != indexFixed; )
                {
                    for (range range = *--index; range.StartIndex != range.EndIndex; ++range.StartIndex)
                    {
                        values.UnsafeAdd(array[range.StartIndex]);
                    }
                }
            }
            return values;
        }
        /// <summary>
        /// 获取匹配的数据集合
        /// </summary>
        /// <param name="isValue">数据匹配器</param>
        /// <returns>匹配的数据集合</returns>
        protected unsafe subArray<valueType> getList(Func<valueType, bool> isValue)
        {
            subArray<valueType> values = new subArray<valueType>(array.Length);
            fixed (range* indexFixed = indexs)
            {
                for (range* index = indexFixed + indexs.Length; index != indexFixed; )
                {
                    for (range range = *--index; range.StartIndex != range.EndIndex; ++range.StartIndex)
                    {
                        valueType value = array[range.StartIndex];
                        if (isValue(value)) values.UnsafeAdd(value);
                    }
                }
            }
            return values;
        }
        /// <summary>
        /// 获取匹配的数据集合
        /// </summary>
        /// <typeparam name="listType">目标数据类型</typeparam>
        /// <param name="getValue">数据转换器</param>
        /// <returns>匹配的数据集合</returns>
        protected unsafe subArray<listType> getList<listType>(Func<valueType, listType> getValue)
        {
            subArray<listType> values = new subArray<listType>(array.Length);
            fixed (range* indexFixed = indexs)
            {
                for (range* index = indexFixed + indexs.Length; index != indexFixed; )
                {
                    for (range range = *--index; range.StartIndex != range.EndIndex; ++range.StartIndex)
                    {
                        values.UnsafeAdd(getValue(array[range.StartIndex]));
                    }
                }
            }
            return values;
        }
    }
}
