using System;
using Java.Security.Cert;
using Javax.Net.Ssl;

namespace fastCSharp.android
{
    /// <summary>
    /// Javax.Net.Ssl.IX509TrustManager À©Õ¹
    /// </summary>
    public static class iX509TrustManager
    {
        /// <summary>
        /// CheckClientTrusted
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="chain"></param>
        /// <param name="authType"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void checkClientTrusted(this IX509TrustManager manager, X509Certificate[] chain, string authType)
        {
            manager.CheckClientTrusted(chain, authType);
        }
        /// <summary>
        /// CheckServerTrusted
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="chain"></param>
        /// <param name="authType"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void checkServerTrusted(this IX509TrustManager manager, X509Certificate[] chain, string authType)
        {
            manager.CheckServerTrusted(chain, authType);
        }
        /// <summary>
        /// GetAcceptedIssuers
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static X509Certificate[] getAcceptedIssuers(this IX509TrustManager manager)
        {
            return manager.GetAcceptedIssuers();
        }
    }
}