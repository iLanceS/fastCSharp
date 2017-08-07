using System;
using Java.Security;
using Javax.Net.Ssl;

namespace fastCSharp.android
{
    /// <summary>
    /// Javax.Net.Ssl.TrustManagerFactory À©Õ¹
    /// </summary>
    public static class trustManagerFactory
    {
        /// <summary>
        /// Init
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="ks"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void init(this TrustManagerFactory factory, KeyStore ks)
        {
            factory.Init(ks);
        }
        /// <summary>
        /// GetTrustManagers
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static ITrustManager[] getTrustManagers(this TrustManagerFactory factory)
        {
            return factory.GetTrustManagers();
        }
    }
}