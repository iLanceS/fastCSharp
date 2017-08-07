using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 字典扩展操作
    /// </summary>
    public static class dictionary
    {
        /// <summary>
        /// 创建字典
        /// </summary>
        /// <typeparam name="keyType">关键字类型</typeparam>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <returns>字典</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static Dictionary<keyType, valueType> CreateOnly<keyType, valueType>()
             where keyType : class
        {
            return new Dictionary<keyType, valueType>();
        }
        /// <summary>
        /// 创建字典
        /// </summary>
        /// <typeparam name="keyType">关键字类型</typeparam>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="capacity">初始化容器尺寸</param>
        /// <returns>字典</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static Dictionary<keyType, valueType> CreateOnly<keyType, valueType>(int capacity)
             where keyType : class
        {
            return new Dictionary<keyType, valueType>(capacity);
        }
        /// <summary>
        /// 创建字典
        /// </summary>
        /// <typeparam name="keyType">关键字类型</typeparam>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <returns>字典</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static Dictionary<keyType, valueType> Create<keyType, valueType>()
             where keyType : struct, IEquatable<keyType>
        {
#if __IOS__
            return new Dictionary<keyType, valueType>(equalityComparer.comparer<keyType>.Default);
#else
            return new Dictionary<keyType, valueType>();
#endif
        }
        /// <summary>
        /// 创建字典
        /// </summary>
        /// <typeparam name="keyType">关键字类型</typeparam>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="capacity">初始化容器尺寸</param>
        /// <returns>字典</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static Dictionary<keyType, valueType> Create<keyType, valueType>(int capacity)
             where keyType : struct, IEquatable<keyType>
        {
#if __IOS__
            return new Dictionary<keyType, valueType>(capacity, equalityComparer.comparer<keyType>.Default);
#else
            return new Dictionary<keyType, valueType>(capacity);
#endif
        }
        /// <summary>
        /// 创建字典
        /// </summary>
        /// <typeparam name="keyType">关键字类型</typeparam>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="capacity">初始化容器尺寸</param>
        /// <returns>字典</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static Dictionary<keyType, valueType> CreateAny<keyType, valueType>(int capacity)
        {
#if __IOS__
            return new Dictionary<keyType, valueType>(capacity, equalityComparer.comparer<keyType>.Default);
#else
            return new Dictionary<keyType, valueType>(capacity);
#endif
        }
        /// <summary>
        /// 创建字典
        /// </summary>
        /// <typeparam name="keyType">关键字类型</typeparam>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <returns>字典</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static Dictionary<keyType, valueType> CreateAny<keyType, valueType>()
        {
#if __IOS__
            return new Dictionary<keyType, valueType>(equalityComparer.comparer<keyType>.Default);
#else
            return new Dictionary<keyType, valueType>();
#endif
        }
        /// <summary>
        /// 创建字典
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <returns>字典</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static Dictionary<short, valueType> CreateShort<valueType>()
        {
#if __IOS__
            return new Dictionary<short, valueType>(equalityComparer.Short);
#else
            return new Dictionary<short, valueType>();
#endif
        }
        /// <summary>
        /// 创建字典
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <returns>字典</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static Dictionary<int, valueType> CreateInt<valueType>()
        {
#if __IOS__
            return new Dictionary<int, valueType>(equalityComparer.Int);
#else
            return new Dictionary<int, valueType>();
#endif
        }
        /// <summary>
        /// 创建字典
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="capacity">初始化容器尺寸</param>
        /// <returns>字典</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static Dictionary<int, valueType> CreateInt<valueType>(int capacity)
        {
#if __IOS__
            return new Dictionary<int, valueType>(capacity, equalityComparer.Int);
#else
            return new Dictionary<int, valueType>(capacity);
#endif
        }
        /// <summary>
        /// 创建字典
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <returns>字典</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static Dictionary<ulong, valueType> CreateULong<valueType>()
        {
#if __IOS__
            return new Dictionary<ulong, valueType>(equalityComparer.ULong);
#else
            return new Dictionary<ulong, valueType>();
#endif
        }
        /// <summary>
        /// 创建字典
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <returns>字典</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static Dictionary<sessionId, valueType> CreateSessionId<valueType>()
        {
#if __IOS__
            return new Dictionary<sessionId, valueType>(equalityComparer.SessionId);
#else
            return new Dictionary<sessionId, valueType>();
#endif
        }
        /// <summary>
        /// 创建字典
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <returns>字典</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static Dictionary<pointer, valueType> CreatePointer<valueType>()
        {
#if __IOS__
            return new Dictionary<pointer, valueType>(equalityComparer.Pointer);
#else
            return new Dictionary<pointer, valueType>();
#endif
        }
        /// <summary>
        /// 创建字典
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <returns>字典</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static Dictionary<subString, valueType> CreateSubString<valueType>()
        {
#if __IOS__
            return new Dictionary<subString, valueType>(equalityComparer.SubString);
#else
            return new Dictionary<subString, valueType>();
#endif
        }
        /// <summary>
        /// 创建字典
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <returns>字典</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static Dictionary<hashBytes, valueType> CreateHashBytes<valueType>()
        {
#if __IOS__
            return new Dictionary<hashBytes, valueType>(equalityComparer.HashBytes);
#else
            return new Dictionary<hashBytes, valueType>();
#endif
        }
        /// <summary>
        /// 创建字典
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <returns>字典</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static Dictionary<hashString, valueType> CreateHashString<valueType>()
        {
#if __IOS__
            return new Dictionary<hashString, valueType>(equalityComparer.HashString);
#else
            return new Dictionary<hashString, valueType>();
#endif
        }
    }
    /// <summary>
    /// 字典扩展操作
    /// </summary>
    /// <typeparam name="keyType">关键字类型</typeparam>
    public static class dictionary<keyType> where keyType : IEquatable<keyType>
    {
        /// <summary>
        /// 是否值类型
        /// </summary>
        private static readonly bool isValueType = typeof(keyType).IsValueType;
        /// <summary>
        /// 创建字典
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <returns>字典</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static Dictionary<keyType, valueType> Create<valueType>()
        {
#if __IOS__
            if (isValueType) return new Dictionary<keyType, valueType>(equalityComparer.comparer<keyType>.Default);
#endif
            return new Dictionary<keyType, valueType>();
        }
        /// <summary>
        /// 创建字典
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="capacity">初始化容器尺寸</param>
        /// <returns>字典</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static Dictionary<keyType, valueType> Create<valueType>(int capacity)
        {
#if __IOS__
            if (isValueType) return new Dictionary<keyType, valueType>(capacity, equalityComparer.comparer<keyType>.Default);
#endif
            return new Dictionary<keyType, valueType>(capacity);
        }
    }
}
