using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 字典扩展操作
    /// </summary>
    public static class dictionaryExpand
    {
        /// <summary>
        /// 根据键值获取数据
        /// </summary>
        /// <typeparam name="keyType">键类型</typeparam>
        /// <typeparam name="valueType">值类型</typeparam>
        /// <param name="values">字典</param>
        /// <param name="key">键值</param>
        /// <param name="nullValue">失败值</param>
        /// <returns>数据</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType get<keyType, valueType>
            (this Dictionary<keyType, valueType> values, keyType key, valueType nullValue = default(valueType))
        {
            valueType value;
            return values != null && values.TryGetValue(key, out value) ? value : nullValue;
        }
        /// <summary>
        /// 判断是否存在键值
        /// </summary>
        /// <typeparam name="keyType">键类型</typeparam>
        /// <typeparam name="valueType">值类型</typeparam>
        /// <param name="values">字典</param>
        /// <param name="key">键值</param>
        /// <returns>是否存在键值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static bool containsKey<keyType, valueType>(this Dictionary<keyType, valueType> values, keyType key)
        {
            return values != null && values.ContainsKey(key);
        }
        /// <summary>
        /// 获取键值集合
        /// </summary>
        /// <typeparam name="keyType">键类型</typeparam>
        /// <typeparam name="valueType">值类型</typeparam>
        /// <param name="values">字典</param>
        /// <returns>键值集合</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static ICollection<keyType> keys<keyType, valueType>(this Dictionary<keyType, valueType> values)
        {
            return values != null ? values.Keys : null;
        }
        /// <summary>
        /// 获取数据集合
        /// </summary>
        /// <typeparam name="keyType">键类型</typeparam>
        /// <typeparam name="valueType">值类型</typeparam>
        /// <param name="values">字典</param>
        /// <returns>数据集合</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static ICollection<valueType> values<keyType, valueType>(this Dictionary<keyType, valueType> values)
        {
            return values != null ? values.Values : null;
        }
    }
}
