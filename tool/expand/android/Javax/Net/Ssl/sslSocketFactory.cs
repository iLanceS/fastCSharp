using System;
using Java.Net;
using Javax.Net.Ssl;

namespace fastCSharp.android
{
    /// <summary>
    /// Javax.Net.Ssl.SSLSocketFactory À©Õ¹
    /// </summary>
    public static partial class sslSocketFactory
    {
        /// <summary>
        /// CreateSocket
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="s"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="autoClose"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Socket createSocket(this SSLSocketFactory factory, Socket s, string host, int port, bool autoClose)
        {
            return factory.CreateSocket(s, host, port, autoClose);
        }
    }
}