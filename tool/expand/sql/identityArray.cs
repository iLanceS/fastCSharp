using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql
{
    /// <summary>
    /// 缓存专用分段二维数组参数
    /// </summary>
    internal static class identityArray
    {
        /// <summary>
        /// 数组长度的有效二进制位数
        /// </summary>
        internal const int ArrayShift = 16;
        /// <summary>
        /// 数组长度
        /// </summary>
        internal const int ArraySize = 1 << ArrayShift;
        /// <summary>
        /// 数组索引计算 And 值
        /// </summary>
        internal const int ArraySizeAnd = ArraySize - 1;
    }
    /// <summary>
    /// 缓存专用分段二维数组，分段大小为 65536
    /// </summary>
    /// <typeparam name="valueType"></typeparam>
    public struct identityArray<valueType> where valueType : class
    {
        /// <summary>
        /// 二维数组
        /// </summary>
        private valueType[][] arrays;
        /// <summary>
        /// 有效数组数量
        /// </summary>
        private int arrayCount;
        /// <summary>
        /// 当前数组容量
        /// </summary>
        internal int Length;
        /// <summary>
        /// 获取或者设置数据(不检测位置有效性)
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal valueType this[int index]
        {
            get { return arrays[index >> identityArray.ArrayShift][index & identityArray.ArraySizeAnd]; }
            set { arrays[index >> identityArray.ArrayShift][index & identityArray.ArraySizeAnd] = value; }
        }
        /// <summary>
        /// 数组枚举(排除最后一个数组)
        /// </summary>
        internal IEnumerable<valueType[]> LeftArrays
        {
            get
            {
                int count = arrayCount - 1;
                if (count > 0)
                {
                    foreach (valueType[] array in arrays)
                    {
                        yield return array;
                        if (--count == 0) break;
                    }
                }
            }
        }
        /// <summary>
        /// 最后一个数组
        /// </summary>
        internal valueType[] LastArray
        {
            get { return arrays[arrayCount - 1]; }
        }
        /// <summary>
        /// 数组枚举
        /// </summary>
        internal IEnumerable<valueType[]> Arrays
        {
            get
            {
                int count = arrayCount;
                foreach (valueType[] array in arrays)
                {
                    yield return array;
                    if (--count == 0) break;
                }
            }
        }
        /// <summary>
        /// 缓存专用分段二维数组
        /// </summary>
        /// <param name="size"></param>
        internal identityArray(int size)
        {
            int newArrayCount = (size + identityArray.ArraySizeAnd) >> identityArray.ArrayShift;
            arrays = new valueType[Math.Max(newArrayCount, 4)][];
            for (arrayCount = 0; arrayCount < newArrayCount; arrays[arrayCount++] = new valueType[identityArray.ArraySize]) ;
            Length = arrayCount << identityArray.ArrayShift;
        }
        /// <summary>
        /// 增加数组扩展容量到指定数量
        /// </summary>
        /// <param name="size"></param>
        internal void ToSize(int size)
        {
            int newArrayCount = (size + identityArray.ArraySizeAnd) >> identityArray.ArrayShift;
            if (arrays == null) arrays = new valueType[Math.Max(newArrayCount, 4)][];
            else if (arrays.Length < newArrayCount)
            {
                valueType[][] newArrays = new valueType[Math.Max(arrays.Length << 1, newArrayCount)][];
                Array.Copy(arrays, 0, newArrays, 0, arrayCount);
                arrays = newArrays;
            }
            while (arrayCount < newArrayCount) arrays[arrayCount++] = new valueType[identityArray.ArraySize];
            Length = arrayCount << identityArray.ArrayShift;
        }
        /// <summary>
        /// 获取并清除数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal valueType GetRemove(int index)
        {
            valueType[] array = arrays[index >> identityArray.ArrayShift];
            valueType value = array[index &= identityArray.ArraySizeAnd];
            array[index] = null;
            return value;
        }
    }
}
