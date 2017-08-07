using System;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.IO.InputStream 扩展
    /// </summary>
    public static class inputStream
    {
        /// <summary>
        /// 小数据流转换
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static System.IO.Stream ToTinyStream(this Java.IO.InputStream stream)
        {
            using (stream)
            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
            {
                byte[] buffer = fastCSharp.memoryPool.TinyBuffers.Get();
                try
                {
                    do
                    {
                        int length = stream.Read(buffer);
                        if (length == -1) break;
                        if (length != 0) memoryStream.Write(buffer, 0, length);
                    }
                    while (true);
                }
                finally { fastCSharp.memoryPool.TinyBuffers.Push(ref buffer); }
                return new System.IO.MemoryStream(memoryStream.GetBuffer(), 0, (int)memoryStream.Position, false);
            }
        }
        /// <summary>
        /// 关闭 Close
        /// </summary>
        /// <param name="stream"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void close(this Java.IO.InputStream stream)
        {
            stream.Close();
        }
        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int read(this Java.IO.InputStream stream, byte[] b)
        {
            return stream.Read(b);
        }
    }
}