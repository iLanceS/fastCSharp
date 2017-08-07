using System;
using Java.Net;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Net.URL À©Õ¹
    /// </summary>
    public static class url
    {
        /// <summary>
        /// Host
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getHost(this URL url)
        {
            return url.Host;
        }
        /// <summary>
        /// Query
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getQuery(this URL url)
        {
            return url.Query;
        }
        /// <summary>
        /// Ref
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getRef(this URL url)
        {
            return url.Ref;
        }
        /// <summary>
        /// OpenConnection
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static URLConnection openConnection(this URL url)
        {
            return url.OpenConnection();
        }
    }
}