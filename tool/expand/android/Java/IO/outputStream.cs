using System;
using System.IO;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.IO.OutputStream 扩展
    /// </summary>
    public static class outputStream
    {
        /// <summary>
        /// 关闭 Close
        /// </summary>
        /// <param name="stream"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void close(this Java.IO.OutputStream stream)
        {
            stream.Close();
        }
        /// <summary>
        /// 写入数据 Write
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="b"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void write(this Java.IO.OutputStream stream, byte[] b)
        {
            stream.Write(b);
        }
        /// <summary>
        /// 写入数据 Write
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="b"></param>
        /// <param name="off"></param>
        /// <param name="len"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void write(this Java.IO.OutputStream stream, byte[] b, int off, int len)
        {
            stream.Write(b, off, len);
        }
    }
}