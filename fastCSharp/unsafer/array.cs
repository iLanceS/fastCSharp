using System;
using System.Runtime.CompilerServices;

namespace fastCSharp.unsafer
{
    /// <summary>
    /// 数组扩展操作(非安全,请自行确保数据可靠性)
    /// </summary>
    public static partial class array
    {
        /// <summary>
        /// 移动数据块
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">待处理数组</param>
        /// <param name="index">原始数据位置</param>
        /// <param name="writeIndex">目标数据位置</param>
        /// <param name="count">移动数据数量</param>
        public static void Move<valueType>(valueType[] array, int index, int writeIndex, int count)
        {
            int endIndex = index + count;
            if (index < writeIndex && endIndex > writeIndex)
            {
                for (int writeEndIndex = writeIndex + count; endIndex != index; array[--writeEndIndex] = array[--endIndex]) ;
            }
            else Array.Copy(array, index, array, writeIndex, count);
        }
        /// <summary>
        /// 移除数据
        /// </summary>
        /// <typeparam name="valueType">数据类型</typeparam>
        /// <param name="array">数组数据</param>
        /// <param name="index">移除数据位置</param>
        /// <returns>移除数据后的数组</returns>
        public static valueType[] GetRemoveAt<valueType>(valueType[] array, int index)
        {
            valueType[] newValues = new valueType[array.Length - 1];
            Array.Copy(array, 0, newValues, 0, index);
            Array.Copy(array, index + 1, newValues, index, array.Length - index - 1);
            return newValues;
        }
    }
}
