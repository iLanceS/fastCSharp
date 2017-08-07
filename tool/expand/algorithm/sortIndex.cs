using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace fastCSharp.algorithm
{
    /// <summary>
    /// 排序索引
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct dateTimeSortIndex
    {
        /// <summary>
        /// 数值
        /// </summary>
        [FieldOffset(0)]
        public DateTime Value;
        /// <summary>
        /// 位置索引
        /// </summary>
        [FieldOffset(sizeof(long))]
        public int Index;
        /// <summary>
        /// 设置排序索引
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="index">位置索引</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Set(DateTime value, int index)
        {
            Value = value;
            Index = index;
        }
        /// <summary>
        /// 根据数组获取排序索引
        /// </summary>
        /// <typeparam name="valueType">数组类型</typeparam>
        /// <param name="indexFixed">排序索引数组</param>
        /// <param name="values">数组</param>
        /// <param name="getValue">数据排序值获取器</param>
        internal unsafe static void Create<valueType>(dateTimeSortIndex* indexFixed, valueType[] values, Func<valueType, DateTime> getValue)
        {
            int index = 0;
            foreach (valueType value in values) (*indexFixed++).Set(getValue(value), index++);
        }
        /// <summary>
        /// 根据数组获取排序索引
        /// </summary>
        /// <typeparam name="valueType">数组类型</typeparam>
        /// <param name="indexFixed">排序索引数组</param>
        /// <param name="values">数组</param>
        /// <param name="getValue">数据排序值获取器</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">排序数据数量</param>
        internal unsafe static void Create<valueType>(dateTimeSortIndex* indexFixed, valueType[] values, Func<valueType, DateTime> getValue, int startIndex, int count)
        {
            for (int endIndex = startIndex + count; startIndex != endIndex; (*indexFixed++).Set(getValue(values[startIndex]), startIndex++)) ;
        }
        /// <summary>
        /// 根据排序索引获取数组
        /// </summary>
        /// <typeparam name="valueType">数组类型</typeparam>
        /// <param name="indexFixed">排序索引数组</param>
        /// <param name="values">数组</param>
        /// <param name="count">排序数据数量</param>
        /// <returns>排序后的数组</returns>
        internal unsafe static valueType[] Create<valueType>(dateTimeSortIndex* indexFixed, valueType[] values, int count)
        {
            valueType[] newValues = new valueType[count];
            for (int index = 0; index != count; ++index) newValues[index] = values[(*indexFixed++).Index];
            return newValues;
        }
        /// <summary>
        /// 根据数组获取排序索引
        /// </summary>
        /// <typeparam name="valueType">数组类型</typeparam>
        /// <param name="indexFixed">排序索引数组</param>
        /// <param name="values">数组</param>
        /// <param name="getValue">数据排序值获取器</param>
        internal unsafe static void Create<valueType>(longSortIndex* indexFixed, valueType[] values, Func<valueType, DateTime> getValue)
        {
            int index = 0;
            foreach (valueType value in values) (*indexFixed++).Set(getValue(value).Ticks, index++);
        }
        /// <summary>
        /// 根据数组获取排序索引
        /// </summary>
        /// <typeparam name="valueType">数组类型</typeparam>
        /// <param name="indexFixed">排序索引数组</param>
        /// <param name="values">数组</param>
        /// <param name="getValue">数据排序值获取器</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">排序数据数量</param>
        internal unsafe static void Create<valueType>(longSortIndex* indexFixed, valueType[] values, Func<valueType, DateTime> getValue, int startIndex, int count)
        {
            for (int endIndex = startIndex + count; startIndex != endIndex; (*indexFixed++).Set(getValue(values[startIndex]).Ticks, startIndex++)) ;
        }
    }
}
