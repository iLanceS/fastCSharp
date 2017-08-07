using System;

namespace fastCSharp.android
{
    /// <summary>
    /// System.IO.MemoryStream À©Õ¹
    /// </summary>
    public static class memoryStream
    {
        /// <summary>
        /// ToArray
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static byte[] toByteArray(this System.IO.MemoryStream stream)
        {
            return stream.ToArray();
        }
    }
}