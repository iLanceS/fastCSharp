using System;
/*Type:ulong;long;uint;int;ushort;short;byte;sbyte;double;float;DateTime*/

namespace fastCSharp
{
    /// <summary>
    /// 集合相关扩展
    /// </summary>
    public static partial class iCollection
    {
        /// <summary>
        /// 根据集合内容返回数组
        /// </summary>
        /// <param name="values">值集合</param>
        /// <returns>数组</returns>
        public unsafe static /*Type[0]*/ulong/*Type[0]*/[] getArray
            (this System.Collections.Generic.ICollection</*Type[0]*/ulong/*Type[0]*/> values)
        {
            if (values.count() != 0)
            {
                /*Type[0]*/
                ulong/*Type[0]*/[] newValues = new /*Type[0]*/ulong/*Type[0]*/[values.Count];
                fixed (/*Type[0]*/ulong/*Type[0]*/* newValueFixed = newValues)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/* write = newValueFixed;
                    foreach (/*Type[0]*/ulong/*Type[0]*/ value in values) *write++ = value;
                }
                return newValues;
            }
            return nullValue</*Type[0]*/ulong/*Type[0]*/>.Array;
        }
        /// <summary>
        /// 根据集合内容返回数组
        /// </summary>
        /// <typeparam name="valueType">枚举值类型</typeparam>
        /// <param name="values">值集合</param>
        /// <param name="getValue">获取数组值的委托</param>
        /// <returns>数组</returns>
        public unsafe static /*Type[0]*/ulong/*Type[0]*/[] getArray<valueType>
            (this System.Collections.Generic.ICollection<valueType> values, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getValue)
        {
            if (values.count() != 0)
            {
                if (getValue == null) log.Error.Throw(log.exceptionType.Null);
                /*Type[0]*/
                ulong/*Type[0]*/[] newValues = new /*Type[0]*/ulong/*Type[0]*/[values.Count];
                fixed (/*Type[0]*/ulong/*Type[0]*/* newValueFixed = newValues)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/* write = newValueFixed;
                    foreach (valueType value in values) *write++ = getValue(value);
                }
                return newValues;
            }
            return nullValue</*Type[0]*/ulong/*Type[0]*/>.Array;
        }
        /// <summary>
        /// 根据集合内容返回单向列表
        /// </summary>
        /// <typeparam name="valueType">枚举值类型</typeparam>
        /// <param name="values">值集合</param>
        /// <param name="getValue">获取数组值的委托</param>
        /// <returns>单向列表</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static list</*Type[0]*/ulong/*Type[0]*/> getList<valueType>
            (this System.Collections.Generic.ICollection<valueType> values, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getValue)
        {
            return values != null ? new list</*Type[0]*/ulong/*Type[0]*/>(getArray(values, getValue), true) : null;
        }
        /// <summary>
        /// 根据集合内容返回单向列表
        /// </summary>
        /// <param name="values">值集合</param>
        /// <returns>单向列表</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static list</*Type[0]*/ulong/*Type[0]*/> getList(this System.Collections.Generic.ICollection</*Type[0]*/ulong/*Type[0]*/> values)
        {
            return values != null ? new list</*Type[0]*/ulong/*Type[0]*/>(getArray(values), true) : null;
        }
        /// <summary>
        /// 根据集合内容返回双向列表
        /// </summary>
        /// <typeparam name="valueType">枚举值类型</typeparam>
        /// <param name="values">值集合</param>
        /// <param name="getValue">获取数组值的委托</param>
        /// <returns>双向列表</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static collection</*Type[0]*/ulong/*Type[0]*/> getCollection<valueType>
            (this System.Collections.Generic.ICollection<valueType> values, Func<valueType, /*Type[0]*/ulong/*Type[0]*/> getValue)
        {
            return values != null ? new collection</*Type[0]*/ulong/*Type[0]*/>(getArray(values, getValue), true) : null;
        }
        /// <summary>
        /// 根据集合内容返回双向列表
        /// </summary>
        /// <param name="values">值集合</param>
        /// <returns>双向列表</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static collection</*Type[0]*/ulong/*Type[0]*/> getCollection
            (this System.Collections.Generic.ICollection</*Type[0]*/ulong/*Type[0]*/> values)
        {
            return values != null ? new collection</*Type[0]*/ulong/*Type[0]*/>(getArray(values), true) : null;
        }
        /// <summary>
        /// 查找符合条件的记录集合
        /// </summary>
        /// <param name="values">值集合</param>
        /// <param name="isValue">判断记录是否符合条件的委托</param>
        /// <returns>符合条件的记录集合</returns>
        public unsafe static subArray</*Type[0]*/ulong/*Type[0]*/> getFind
            (this System.Collections.Generic.ICollection</*Type[0]*/ulong/*Type[0]*/> values, Func</*Type[0]*/ulong/*Type[0]*/, bool> isValue)
        {
            if (values.count() != 0)
            {
                if (isValue == null) log.Error.Throw(log.exceptionType.Null);
                /*Type[0]*/
                ulong/*Type[0]*/[] newValues = new /*Type[0]*/ulong/*Type[0]*/[values.Count];
                fixed (/*Type[0]*/ulong/*Type[0]*/* newValueFixed = newValues)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/* write = newValueFixed;
                    foreach (/*Type[0]*/ulong/*Type[0]*/ value in values)
                    {
                        if (isValue(value)) *write++ = value;
                    }
                    return subArray</*Type[0]*/ulong/*Type[0]*/>.Unsafe(newValues, 0, (int)(write - newValueFixed));
                }
            }
            return default(subArray</*Type[0]*/ulong/*Type[0]*/>);
        }
        /// <summary>
        /// 查找符合条件的记录集合
        /// </summary>
        /// <param name="values">值集合</param>
        /// <param name="isValue">判断记录是否符合条件的委托</param>
        /// <returns>符合条件的记录集合</returns>
        public unsafe static subArray</*Type[0]*/ulong/*Type[0]*/> getFind
            (this System.Collections.ICollection values, Func</*Type[0]*/ulong/*Type[0]*/, bool> isValue)
        {
            if (values != null)
            {
                if (isValue == null) log.Error.Throw(log.exceptionType.Null);
                /*Type[0]*/
                ulong/*Type[0]*/[] newValues = new /*Type[0]*/ulong/*Type[0]*/[values.Count];
                fixed (/*Type[0]*/ulong/*Type[0]*/* newValueFixed = newValues)
                {
                    /*Type[0]*/
                    ulong/*Type[0]*/* write = newValueFixed;
                    foreach (/*Type[0]*/ulong/*Type[0]*/ value in values)
                    {
                        if (isValue(value)) *write++ = value;
                    }
                    return subArray</*Type[0]*/ulong/*Type[0]*/>.Unsafe(newValues, 0, (int)(write - newValueFixed));
                }
            }
            return default(subArray</*Type[0]*/ulong/*Type[0]*/>);
        }
        /// <summary>
        /// 查找符合条件的记录集合
        /// </summary>
        /// <param name="values">值集合</param>
        /// <param name="isValue">判断记录是否符合条件的委托</param>
        /// <returns>符合条件的记录集合</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static /*Type[0]*/ulong/*Type[0]*/[] getFindArray
            (this System.Collections.Generic.ICollection</*Type[0]*/ulong/*Type[0]*/> values, Func</*Type[0]*/ulong/*Type[0]*/, bool> isValue)
        {
            return values.getFind(isValue).ToArray().notNull();
        }
        /// <summary>
        /// 查找符合条件的记录集合
        /// </summary>
        /// <param name="values">值集合</param>
        /// <param name="isValue">判断记录是否符合条件的委托</param>
        /// <returns>符合条件的记录集合</returns>
        [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static /*Type[0]*/ulong/*Type[0]*/[] getFindArray
            (this System.Collections.ICollection values, Func</*Type[0]*/ulong/*Type[0]*/, bool> isValue)
        {
            return values.getFind(isValue).ToArray().notNull();
        }
    }
}
