using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// HASH表扩展操作
    /// </summary>
    public static class hashSet
    {
        /// <summary>
        /// 创建HASH表
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <returns>HASH表</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static HashSet<valueType> CreateOnly<valueType>() where valueType : class
        {
            return new HashSet<valueType>();
        }
        /// <summary>
        /// 创建HASH表
        /// </summary>
        /// <returns>HASH表</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static HashSet<pointer> CreatePointer()
        {
#if __IOS__
            return new HashSet<pointer>(equalityComparer.Pointer);
#else
            return new HashSet<pointer>();
#endif
        }
        /// <summary>
        /// 创建HASH表
        /// </summary>
        /// <returns>HASH表</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static HashSet<subString> CreateSubString()
        {
#if __IOS__
            return new HashSet<subString>(equalityComparer.SubString);
#else
            return new HashSet<subString>();
#endif
        }
        /// <summary>
        /// 创建HASH表
        /// </summary>
        /// <returns>HASH表</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static HashSet<hashString> CreateHashString()
        {
#if __IOS__
            return new HashSet<hashString>(equalityComparer.HashString);
#else
            return new HashSet<hashString>();
#endif
        }
    }
    /// <summary>
    /// HASH表扩展操作
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    public static class hashSet<valueType>
    {
        /// <summary>
        /// 是否值类型
        /// </summary>
        private static readonly bool isValueType = typeof(valueType).IsValueType;
        /// <summary>
        /// 创建字典
        /// </summary>
        /// <returns>字典</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static HashSet<valueType> Create()
        {
#if __IOS__
            if (isValueType) return new HashSet<valueType>(equalityComparer.comparer<valueType>.Default);
#endif
            return new HashSet<valueType>();
        }
    }
}
