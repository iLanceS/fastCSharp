using System;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 枚举器
    /// </summary>
    /// <typeparam name="valueType"></typeparam>
    static class iEnumerator<valueType>
    {
        /// <summary>
        /// 空枚举器
        /// </summary>
        private struct empty : IEnumerator<valueType>
        {
            /// <summary>
            /// 当前数据元素
            /// </summary>
            valueType IEnumerator<valueType>.Current
            {
                get
                {
                    log.Error.Throw(log.exceptionType.IndexOutOfRange);
                    return default(valueType);
                }
            }
            /// <summary>
            /// 当前数据元素
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    log.Error.Throw(log.exceptionType.IndexOutOfRange);
                    return default(valueType);
                }
            }
            /// <summary>
            /// 转到下一个数据元素
            /// </summary>
            /// <returns>是否存在下一个数据元素</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public bool MoveNext()
            {
                return false;
            }
            /// <summary>
            /// 重置枚举器状态
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Reset() { }
            /// <summary>
            /// 释放枚举器
            /// </summary>
            public void Dispose() { }
        }
        /// <summary>
        /// 空枚举实例
        /// </summary>
        internal static readonly IEnumerator<valueType> Empty = new empty();
        /// <summary>
        /// 数组枚举器
        /// </summary>
        internal struct array : IEnumerator<valueType>
        {
            /// <summary>
            /// 被枚举数组
            /// </summary>
            private valueType[] values;
            /// <summary>
            /// 当前位置
            /// </summary>
            private int currentIndex;
            /// <summary>
            /// 结束位置
            /// </summary>
            private int endIndex;
            /// <summary>
            /// 起始位置
            /// </summary>
            private int startIndex;
            /// <summary>
            /// 数组枚举器
            /// </summary>
            /// <param name="value">双向动态数据</param>
            public array(collection<valueType> value)
            {
                values = value.array;
                startIndex = value.StartIndex;
                endIndex = value.EndIndex;
                currentIndex = startIndex - 1;
                if (endIndex == 0) endIndex = values.Length;
            }
            /// <summary>
            /// 数组枚举器
            /// </summary>
            /// <param name="value">单向动态数据</param>
            public array(list<valueType> value)
            {
                values = value.array;
                startIndex = 0;
                endIndex = value.length;
                currentIndex = -1;
            }
            /// <summary>
            /// 数组枚举器
            /// </summary>
            /// <param name="value">数组子串</param>
            public array(subArray<valueType> value)
            {
                values = value.array;
                startIndex = value.startIndex;
                endIndex = value.EndIndex;
                currentIndex = startIndex - 1;
            }
            /// <summary>
            /// 当前数据元素
            /// </summary>
            valueType IEnumerator<valueType>.Current
            {
                get { return values[currentIndex]; }
            }
            /// <summary>
            /// 当前数据元素
            /// </summary>
            object IEnumerator.Current
            {
                get { return values[currentIndex]; }
            }
            /// <summary>
            /// 转到下一个数据元素
            /// </summary>
            /// <returns>是否存在下一个数据元素</returns>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public bool MoveNext()
            {
                if (++currentIndex != endIndex) return true;
                --currentIndex;
                return false;
            }
            /// <summary>
            /// 重置枚举器状态
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            public void Reset()
            {
                currentIndex = startIndex - 1;
            }
            /// <summary>
            /// 释放枚举器
            /// </summary>
            public void Dispose()
            {
                values = null;
            }
        }
    }
}
