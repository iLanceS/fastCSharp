using System;
using Java.Security;
using Javax.Net.Ssl;

namespace fastCSharp.android
{
    /// <summary>
    /// Javax.Net.Ssl.SSLContext À©Õ¹
    /// </summary>
    public static class sslContext
    {
        /// <summary>
        /// Init
        /// </summary>
        /// <param name="ssl"></param>
        /// <param name="km"></param>
        /// <param name="tm"></param>
        /// <param name="random"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void init(this SSLContext ssl, IKeyManager[] km, ITrustManager[] tm, SecureRandom random)
        {
            ssl.Init(km, tm, random);
        }
        /// <summary>
        /// SocketFactory
        /// </summary>
        /// <param name="ssl"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static SSLSocketFactory getSocketFactory(this SSLContext ssl)
        {
            return ssl.SocketFactory;
        }
    }
}