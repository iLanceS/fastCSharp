using System;
using Org.Apache.Http.Conn.Ssl;

namespace fastCSharp.android
{
    /// <summary>
    /// Org.Apache.Http.Conn.Ssl.SSLSocketFactory À©Õ¹
    /// </summary>
    public static partial class sslSocketFactory
    {
        /// <summary>
        /// HostnameVerifier
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="verifier"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setHostnameVerifier(this SSLSocketFactory factory, IX509HostnameVerifier verifier)
        {
            factory.HostnameVerifier = verifier;
        }

    }
}