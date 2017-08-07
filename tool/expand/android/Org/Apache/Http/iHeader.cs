using System;
using Org.Apache.Http;

namespace fastCSharp.android
{
    /// <summary>
    /// Org.Apache.Http.IHeader À©Õ¹
    /// </summary>
    public static class iHeader
    {
        /// <summary>
        /// Value
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getValue(this IHeader header)
        {
            return header.Value;
        }
    }
}