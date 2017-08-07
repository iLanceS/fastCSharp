using System;
using Java.Net;
using Javax.Net;

namespace fastCSharp.android
{
    /// <summary>
    /// Javax.Net.SocketFactory À©Õ¹
    /// </summary>
    public static class socketFactory
    {
        /// <summary>
        /// CreateSocket
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Socket createSocket(this SocketFactory factory)
        {
            return factory.CreateSocket();
        }
    }
}