using System;
using Org.Apache.Http;

namespace fastCSharp.android
{
    /// <summary>
    /// Org.Apache.Http.IStatusLine À©Õ¹
    /// </summary>
    public static class iStatusLine
    {
        /// <summary>
        /// StatusCode
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int getStatusCode(this IStatusLine status)
        {
            return status.StatusCode;
        }
    }
}