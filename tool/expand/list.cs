using System;
using System.Collections.Generic;
using fastCSharp.algorithm;
using System.Reflection.Emit;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 单向动态数组扩展操作
    /// </summary>
    /// <typeparam name="valueType"></typeparam>
    internal static partial class listExtension<valueType>
    {
        /// <summary>
        /// 获取数组
        /// </summary>
        internal static readonly Func<List<valueType>, valueType[]> GetItems = emit.pub.GetField<List<valueType>, valueType[]>("_items");
    }
    /// <summary>
    /// 单向动态数组扩展操作
    /// </summary>
    public static partial class listExtension
    {
        /// <summary>
        /// 转换成数组子串
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray<valueType> toSubArray<valueType>(this List<valueType> list)
        {
            if (list == null) return default(subArray<valueType>);
            return subArray<valueType>.Unsafe(listExtension<valueType>.GetItems(list), 0, list.Count);
        }
    }
}
