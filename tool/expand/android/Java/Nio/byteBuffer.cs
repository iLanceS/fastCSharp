using System;
using Java.Nio;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Nio.ByteBuffer À©Õ¹
    /// </summary>
    public static class byteBuffer
    {
        /// <summary>
        /// PutInt
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static ByteBuffer putInt(this ByteBuffer buffer, int value)
        {
            return buffer.PutInt(value);
        }
    }
}