using System;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.IO.Writer À©Õ¹
    /// </summary>
    public static class writer
    {
        /// <summary>
        /// ¹Ø±Õ Close
        /// </summary>
        /// <param name="stream"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void close(this Java.IO.Writer stream)
        {
            stream.Close();
        }
        /// <summary>
        /// Ð´Èë×Ö·û´® Write
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="str"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void write(this Java.IO.Writer writer, string str)
        {
            writer.Write(str);
        }
        /// <summary>
        /// Write
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="cbuf"></param>
        /// <param name="off"></param>
        /// <param name="len"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void write(this Java.IO.Writer writer, char[] cbuf, int off, int len)
        {
            writer.Write(cbuf, off, len);
        }
        /// <summary>
        /// Ë¢ÐÂ»º³åÇø Flush
        /// </summary>
        /// <param name="writer"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void flush(this Java.IO.Writer writer)
        {
            writer.Flush();
        }
    }
}