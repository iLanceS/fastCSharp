using System;
using Java.Net;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Net.InetSocketAddress À©Õ¹
    /// </summary>
    public static class inetSocketAddress
    {
        /// <summary>
        /// Address
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static InetAddress getAddress(this InetSocketAddress address)
        {
            return address.Address;
        }
    }
}