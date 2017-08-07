using System;
using Org.Apache.Http;

namespace fastCSharp.android
{
    /// <summary>
    /// Org.Apache.Http.IHttpResponse À©Õ¹
    /// </summary>
    public static class iHttpResponse
    {
        /// <summary>
        /// StatusLine
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static IStatusLine getStatusLine(this IHttpResponse response)
        {
            return response.StatusLine;
        }
        /// <summary>
        /// Entity
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static IHttpEntity getEntity(this IHttpResponse response)
        {
            return response.Entity;
        }

    }
}