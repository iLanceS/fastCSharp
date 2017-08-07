using System;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.IO.Reader À©Õ¹
    /// </summary>
    public static class reader
    {
        /// <summary>
        /// ¹Ø±Õ Close
        /// </summary>
        /// <param name="reader"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void close(this Java.IO.Reader reader)
        {
            reader.Close();
        }
    }
}