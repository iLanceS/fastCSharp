using System;
using Org.Apache.Http;

namespace fastCSharp.android
{
    /// <summary>
    /// Org.Apache.Http.IHttpEntity À©Õ¹
    /// </summary>
    public static class iHttpEntity
    {
        /// <summary>
        /// Content
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static System.IO.Stream getContent(this IHttpEntity entity)
        {
            return entity.Content;
        }
        /// <summary>
        /// ContentLength
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static long getContentLength(this IHttpEntity entity)
        {
            return entity.ContentLength;
        }
    }
}