using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace fastCSharp.net.tcp.http
{
    /// <summary>
    /// 索引位置
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct bufferIndex
    {
        /// <summary>
        /// 索引位置
        /// </summary>
        [FieldOffset(0)]
        internal int Value;
        /// <summary>
        /// 起始位置
        /// </summary>
        [FieldOffset(0)]
        public short StartIndex;
        /// <summary>
        /// 长度
        /// </summary>
        [FieldOffset(2)]
        public short Length;
        /// <summary>
        /// 结束位置
        /// </summary>
        internal int EndIndex
        {
            get { return StartIndex + Length; }
        }
        /// <summary>
        /// 索引位置
        /// </summary>
        /// <param name="startIndex">起始位置</param>
        /// <param name="length">长度</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Set(long startIndex, long length)
        {
            StartIndex = (short)startIndex;
            Length = (short)length;
        }
        /// <summary>
        /// 索引位置
        /// </summary>
        /// <param name="startIndex">起始位置</param>
        /// <param name="length">长度</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Set(int startIndex, int length)
        {
            StartIndex = (short)startIndex;
            Length = (short)length;
        }
        /// <summary>
        /// 索引位置
        /// </summary>
        /// <param name="startIndex">起始位置</param>
        /// <param name="length">长度</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Set(short startIndex, short length)
        {
            StartIndex = startIndex;
            Length = length;
        }
        /// <summary>
        /// 索引位置置空
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public void Null()
        {
            Value = 0;
        }
        /// <summary>
        /// 移动到下一个位置
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void Next()
        {
            ++StartIndex;
            --Length;
        }
    }
}
