using System;
using Java.Net;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Net.InetAddress À©Õ¹
    /// </summary>
    public static class inetAddress
    {
        /// <summary>
        /// HostAddress
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getHostAddress(this InetAddress address)
        {
            return address.HostAddress;
        }
    }
}