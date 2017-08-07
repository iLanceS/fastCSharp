using System;
using Org.Apache.Http;

namespace fastCSharp.android
{
    /// <summary>
    /// Org.Apache.Http.IHttpMessage À©Õ¹
    /// </summary>
    public static class iHttpMessage
    {
        /// <summary>
        /// GetFirstHeader
        /// </summary>
        /// <param name="message"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static IHeader getFirstHeader(this IHttpMessage message, string name)
        {
            return message.GetFirstHeader(name);
        }
    }
}