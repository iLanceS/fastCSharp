using System;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.IO.bufferedReader À©Õ¹
    /// </summary>
    public static class bufferedReader
    {
        /// <summary>
        /// ReadLine
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string readLine(this Java.IO.BufferedReader reader)
        {
            return reader.ReadLine();
        }
    }
}