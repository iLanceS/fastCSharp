using System;
using Java.Net;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Net.Socket À©Õ¹
    /// </summary>
    public static class socket
    {
        /// <summary>
        /// Connect
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="endpoint"></param>
        /// <param name="timeout"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void connect(this Socket socket, SocketAddress endpoint, int timeout)
        {
            socket.Connect(endpoint, timeout);
        }
        /// <summary>
        /// Close
        /// </summary>
        /// <param name="socket"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void close(this Socket socket)
        {
            socket.Close();
        }
    }
}